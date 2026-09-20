// 源单元（文件头证据）：
//   Source/M2Engine/SqliteAuctionDB.pas
//   Source/M2Engine/M2DataCommon.pas（TAuctionDB 的 public 包装层：317-454 / 1230-1613）
//
// 行为保真测试（**不连真库**，全部注入 DbLayerTestKit 的内存接缝）：
//   * 每个 public 方法 ≥3 个用例（空结果集 / 0 / 负数 / 超界页码 / 超界参数 / 语句抛错 / SQL 注入字符）。
//   * 期望值全部来自原文，注释标注 `SqliteAuctionDB.pas:<行>`；不为好写而迁就实现。
//   * 锁死并行报告 §7 列出的原文缺陷（每条至少 1 个用例）：
//       1. DoDeleteAuctionItem 的 Items 段多一个 ';'（原文 638-639）
//       2. DoAddAuctionItem 的 `if AuctionID > 0` 无 else（545-591）→ GetMaxAuctionID 返 0 时静默返回 0
//       3. StdMode = 28 死分支（490-491 先吃进 igSpecial，534-535 的"马牌"永不命中）
//       4. UserItem.btValue[13]（不是 btValue[0]）改名判据（568）
//       5. DoGetMyItemsPageCount 无 except、只有 finally（1185-1198）
//       6. DoHumanRename 的 3 个 Reset 在 try 之外（1567-1569）
//       7. DoRun 的 `except end;` 空吞异常（1543-1544）
//       8. DoJoinItemBid 调**基类** AddAttentionItem（带锁），与 DoAddAttentionItem（无额外锁）不同（727）
//
// 抛错注入手法：DbLayerTestKit 的 ScriptedSqliteStatement 本身不带 throw 钩子（该文件只读），
// 因此这里用 <see cref="HookDatabase"/> 包一层 —— AddSQLStatement 转给 FakeSqliteDatabase
// （保留按名复用与全部观测），但把指定 label 的语句换成抛异常的 ISqliteStatement。
//
// 行语义提示：ScriptedSqliteStatement 按 sqlite3_step 语义 —— 无预设行时首次 Step 即返回 DONE，
// 有 N 行时返回 N 次 ROW 后 DONE。所以"无结果集"不需要（也不应）显式设 StepCode。

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using GXX.Core.Protocol;
using GXX.M2Server.DbLayer;

namespace GXX.M2Server.Tests;

public class DbLayerAuctionBehaviorSqliteTests
{
    // ================================================================== 测试夹具

    /// <summary>把某个 label 的语句换成"调用即抛"的桩；SQL/绑定/Reset 仍记在 Fake 侧。</summary>
    private sealed class HookDatabase : ISqliteDatabase
    {
        private readonly FakeSqliteDatabase _inner;
        private readonly Dictionary<string, Exception> _stepThrows;

        public Exception? ExecuteThrow { get; set; }

        public HookDatabase(FakeSqliteDatabase inner, Dictionary<string, Exception> stepThrows)
        {
            _inner = inner;
            _stepThrows = stepThrows;
        }

        public ISqliteStatement AddSQLStatement(string name)
            => _stepThrows.TryGetValue(name, out Exception? e)
                ? new ThrowingStatement(_inner.For(name), e)
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

    /// <summary>观测仍在 Fake 语句上，但 Step 抛指定异常（模拟 driver 失败）。</summary>
    private sealed class ThrowingStatement : ISqliteStatement
    {
        private readonly ISqliteStatement _inner;
        private readonly Exception _boom;

        public ThrowingStatement(ISqliteStatement inner, Exception boom)
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

    /// <summary>原文在方法内用 <c>AddSQLStatement</c> 现建的临时语句 label（不在 DoInit 的 21 条里）。
    /// 这些语句必须先建出来才能用 <c>db.For(label)</c> 预置结果集或 StepCode。</summary>
    private static readonly string[] TemporaryStatementLabels =
    {
        "Auction_QueryAllItems",        // 原文 773
        "Auction_GetAllItemsCount",     // 原文 1100
    };

    /// <summary>建一个已完成 DoInit 的 SQLite 拍卖库（显式注入 db）。
    /// <paramref name="preRegisterTemporary"/> = true 时**在 DoInit 之前**预建两个临时语句
    /// —— 必须先建，这样方法内 <c>_fdb.AddSQLStatement(label)</c> 才会按名复用返回**同一实例**
    /// （FakeSqliteDatabase.AddSQLStatement 同名复用，DbLayerTestKit.cs:257-267），
    /// 否则方法内新建的实例上不会有我们用 <c>db.For(label).AddRow(...)</c> 预置的结果集。</summary>
    private static (TSqliteAuctionDB Unit, FakeSqliteDatabase Db, FakeDbLayerHost Host, FakeDbLayerEnvironment Env,
        CapturingDbLayerLog Log) NewSqlite(bool preRegisterTemporary = true)
    {
        var (env, log) = DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        var host = new FakeDbLayerHost { DataBase = db };
        var unit = new TSqliteAuctionDB(host, db);
        if (preRegisterTemporary)
        {
            foreach (string label in TemporaryStatementLabels) db.AddSQLStatement(label);
        }
        unit.DoInit();
        return (unit, db, host, env, log);
    }

    /// <summary>只跑 DoInit、**不**预建临时语句（用于断言 DoInit 之后恰好只有 21 条语句的用例）。</summary>
    private static (TSqliteAuctionDB Unit, FakeSqliteDatabase Db, FakeDbLayerHost Host, FakeDbLayerEnvironment Env,
        CapturingDbLayerLog Log) NewSqliteWithoutTemporary()
        => NewSqlite(preRegisterTemporary: false);

    /// <summary>建一个"指定 label 的语句 Step() 会抛异常"的库；<paramref name="executeBoom"/> 让 Execute 抛。</summary>
    private static (TSqliteAuctionDB Unit, FakeSqliteDatabase Db, FakeDbLayerHost Host, CapturingDbLayerLog Log)
        NewSqliteThrowing(string label, Exception boom, Exception? executeBoom = null)
    {
        var (_, log) = DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        // HookDatabase 在 AddSQLStatement 时就会调用 _inner.For(label)，
        // 所以抛错语句必须**先建出来**（临时语句尤其如此），否则 For() 会抛 KeyNotFoundException。
        db.AddSQLStatement(label);
        var hooks = new Dictionary<string, Exception> { [label] = boom };
        var wrapper = new HookDatabase(db, hooks) { ExecuteThrow = executeBoom };
        var host = new FakeDbLayerHost { DataBase = db };
        var unit = new TSqliteAuctionDB(host, wrapper);
        unit.DoInit();
        return (unit, db, host, log);
    }

    /// <summary>可编排的 StdItem 视图（对应原文 UserEngine.GetStdItem 的返回值）。</summary>
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

    /// <summary>可编排的在线玩家视图（对应原文 UserEngine.GetPlayObject 的返回值）。</summary>
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

    // ---------------------------------------------------------------- 行构造辅助

    /// <summary>DoQueryAllItems 的 13 列（原文 775-780 的 select 列表，含末尾 IsAttention）。
    /// 列序：AuctionID, HumanName, AddDateTime, AuctionTime, TimeLeft, StartingPrice, SellingPrice,
    ///       CurrencyType, LastBidPrice, LastBidder, TradingStatus, IsItemGive, IsAttention。</summary>
    private static object?[] AllItemsRow(int auctionId, string human, int addDate, int auctionTime, int timeLeft,
        int starting, int selling, int currency, int lastBidPrice, string lastBidder, int tradingStatus,
        int isItemGive, int isAttention)
        => new object?[] { auctionId, human, addDate, auctionTime, timeLeft, starting, selling, currency,
            lastBidPrice, lastBidder, tradingStatus, isItemGive, isAttention };

    /// <summary>DoQueryMyItems / DoQueryOneItem 的 12 列（原文 221-224 / 242-245，无 IsAttention）。
    /// 列序：AuctionID, HumanName, AddDateTime, AuctionTime, TimeLeft, StartingPrice, SellingPrice,
    ///       CurrencyType, LastBidPrice, LastBidder, TradingStatus, IsItemGive。</summary>
    private static object?[] AuctionRow(int auctionId, string human, int addDate, int auctionTime, int timeLeft,
        int starting, int selling, int currency, int lastBidPrice, string lastBidder, int tradingStatus, int isItemGive)
        => new object?[] { auctionId, human, addDate, auctionTime, timeLeft, starting, selling, currency,
            lastBidPrice, lastBidder, tradingStatus, isItemGive };

    /// <summary>★ <c>Auction_QueryOneItem</c> / <c>Auction_QueryAuctionItems</c> 的 SELECT 列序 ——
    /// 原文 242-245 / 221-224 逐字：<c>HumanName, AddDateTime, AuctionTime, TimeLeft, StartingPrice,
    /// SellingPrice, CurrencyType, LastBidPrice, LastBidder, TradingStatus, IsItemGive</c>（共 11 列，
    /// **没有 AuctionID 列**；AuctionID 来自方法入参 Index，原文 1028/1063）。
    /// 注意 <c>Auction_QueryAuctionItems</c> 在 HumanName 前多一列 AuctionID（221 行首列），
    /// 故该语句用 <see cref="MyItemsRow"/>。</summary>
    private static object?[] OneItemRow(string human, int addDate, int auctionTime, int timeLeft,
        int starting, int selling, int currency, int lastBidPrice, string lastBidder, int tradingStatus, int isItemGive)
        => new object?[] { human, addDate, auctionTime, timeLeft, starting, selling, currency,
            lastBidPrice, lastBidder, tradingStatus, isItemGive };

    /// <summary><c>Auction_QueryAuctionItems</c> 的 12 列（原文 221-224，首列是 AuctionID）。</summary>
    private static object?[] MyItemsRow(int auctionId, string human, int addDate, int auctionTime, int timeLeft,
        int starting, int selling, int currency, int lastBidPrice, string lastBidder, int tradingStatus, int isItemGive)
        => new object?[] { auctionId, human, addDate, auctionTime, timeLeft, starting, selling, currency,
            lastBidPrice, lastBidder, tradingStatus, isItemGive };

    /// <summary>DoRun 的 Auction_QueryAuctionItemSuccess 9 列（原文 214 的 select）。
    /// 列序：A.AuctionID, A.HumanName, A.CurrencyType, A.StartingPrice, A.SellingPrice,
    ///       A.LastBidder, A.LastBidPrice, B.DBIndex, B.MakeIndex。</summary>
    private static object?[] SuccessRow(int auctionId, string human, int currency, int starting, int selling,
        string lastBidder, int lastBidPrice, int dbIndex, int makeIndex)
        => new object?[] { auctionId, human, currency, starting, selling, lastBidder, lastBidPrice, dbIndex, makeIndex };

    /// <summary>原文 <c>AUCTION_ITEM_TYPE = 5</c>（M2DataCommon.pas:12）。</summary>
    private const int AuctionItemType = 5;

    /// <summary>可安全用于任何绑定位置的注入串（原文绑定参数都不拼进 SQL）。</summary>
    private const string Injection = "'; drop table Items; --";

    /// <summary>把 "ig…" 名称换成序数（Grobal2.Types5.cs TItemGroup，原文顺序）。</summary>
    private static readonly Dictionary<string, int> ItemGroupOrdinal = new()
    {
        ["igAll"] = 0, ["igWeapon"] = 1, ["igDress"] = 2, ["igHelmet"] = 3, ["igNecklace"] = 4,
        ["igArmRing"] = 5, ["igRing"] = 6, ["igBelt"] = 7, ["igBoots"] = 8, ["igFashion"] = 9,
        ["igDrug"] = 10, ["igSpecial"] = 11, ["igOther"] = 12,
    };

    // ==================================================================
    // 1. Init —— 原文 198-336
    // ==================================================================

    [Fact]
    public void Init_RegistersTheTwentyOneStatementsInOriginalOrder()
    {
        var (_, db, _, _, _) = NewSqliteWithoutTemporary();

        // 原文 DoInit 的 21 条 AddSQLStatement，顺序见 205-299。
        Assert.Equal(21, db.AddedNames.Count);
        Assert.Equal(new[]
        {
            "Auction_UpdateAuctionItemFail", "Auction_UpdateAuctionItemSuccess", "Auction_QueryAuctionItemSuccess",
            "Auction_QueryAuctionItems", "Auction_GetMyItemsCount", "Auction_QueryAttentionItems",
            "Auction_GetMyAttentionItemsCount", "Auction_QueryOneItem", "Auction_GetMyAuctioningItemsCount",
            "Auction_GetMySellFailItemsCount", "Auction_GetMyBuyOKItemsCount", "Auction_GetMaxAuctionID",
            "Auction_InsertAuctionItem", "Auction_JoinItemBid", "Auction_BuyItem", "Auction_AddAttentionItem",
            "Auction_CheckInAttentionItem", "Auction_DeleteAuctionItem", "Auction_HumanRename",
            "Auction_LastBidderRename", "Auction_AttentionRename",
        }, db.AddedNames.ToArray());
    }

    /// <summary>原文 DoInit（SqliteAuctionDB.pas:198-336）登记的 21 条语句 label，按原文顺序。</summary>
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

    [Fact]
    public void Init_PreparesEveryStatement()
    {
        var (_, db, _, _, _) = NewSqlite();

        // 原文 301-334 的 try 块逐条 Prepare（21 条）。本夹具预建了 2 个**动态**临时语句
        // （Auction_QueryAllItems / Auction_GetAllItemsCount，用于预置结果集），它们不在 DoInit 里注册，
        // 所以只对 DoInit 登记的那 21 条断言。
        foreach (string label in DoInitStatementLabels) Assert.Equal(1, db.For(label).PrepareCount);
        Assert.Equal(21, db.Created.Count(s => s.PrepareCount > 0));
    }

    [Fact]
    public void Init_DoesNotFallBackToOwnerDataBase_WhenDbInjectedExplicitly()
    {
        // 显式注入优先于原文 202-203 的 is 判定回退。
        var injected = new FakeSqliteDatabase();
        var other = new FakeSqliteDatabase();
        var host = new FakeDbLayerHost { DataBase = other };
        DbLayerTestKit.Isolate();
        var unit = new TSqliteAuctionDB(host, injected);

        unit.DoInit();

        Assert.Equal(21, injected.AddedNames.Count);
        Assert.Empty(other.AddedNames);
    }

    [Fact]
    public void Init_FallsBackToOwnerDataBase_WhenNoDbInjected()
    {
        // 原文 202-203：if (Owner.DataBase <> nil) and (Owner.DataBase is TSqlite3DataBase) then FDB := ... as ...
        DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteAuctionDB(new FakeDbLayerHost { DataBase = db });

        unit.DoInit();

        Assert.Equal(21, db.AddedNames.Count);
    }

    [Fact]
    public void Init_WithoutAnyDatabase_ThrowsFromTheUnavailableSeam()
    {
        // 未注入且宿主无库 → 构造默认 UnavailableSqliteDatabase → AddSQLStatement 抛 NotSupportedException。
        DbLayerTestKit.Isolate();
        var unit = new TSqliteAuctionDB(new FakeDbLayerHost());

        Assert.Throws<NotSupportedException>(() => unit.DoInit());
    }

    /// <summary>AddSQLStatement 正常、Prepare 抛异常的库（验原文 DoInit 的 except 分支 329-333）。</summary>
    private sealed class ThrowingPrepareDatabase : ISqliteDatabase
    {
        public ISqliteStatement AddSQLStatement(string name) => new ThrowingPrepareStatement();
        public void BeginTransaction() { }
        public void Commit() { }
        public void RollBack() { }
        public void Execute(string sql) { }
        public bool Connected { get; set; }
        public string Database { get; set; } = "";
        public bool MustExist { get; set; }
        public bool UseThreadMode { get; set; }
        public int ErrorCode => 0;
    }

    private sealed class ThrowingPrepareStatement : ISqliteStatement
    {
        public string Sql { get; set; } = "";
        public void Prepare() => throw new InvalidOperationException("prepare-boom");
        public void StatementFinalize() { }
        public void Reset() { }
        public void OrderBindInt(int value) { }
        public void OrderBindInt64(long value) { }
        public void OrderBindBool(bool value) { }
        public void OrderBindText(string value) { }
        public void OrderBindDouble(double value) { }
        public int Step() => SqliteCodes.SQLITE_DONE;
        public int OrderGetColumnValueInt => 0;
        public long OrderGetColumnValueInt64 => 0;
        public bool OrderGetColumnValueBool => false;
        public string OrderGetColumnValueText => "";
        public double OrderGetColumnValueDouble => 0;
    }

    [Fact]
    public void Init_SwallowsPrepareExceptionsAndLogsThem()
    {
        var (_, log) = DbLayerTestKit.Isolate();
        var unit = new TSqliteAuctionDB(new FakeDbLayerHost { DataBase = new ThrowingPrepareDatabase() });

        unit.DoInit();

        Assert.Contains("prepare-boom", log.Messages);
    }

    // ==================================================================
    // 2. Final —— 原文 337-465
    // ==================================================================

    [Fact]
    public void Final_FinalizesAllTwentyOneStatements()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.DoFinal();

        // 只看 DoInit 登记的那 21 条（夹具预建的 2 个临时语句没被 Prepare 过，不参与本断言）。
        var doInitStatements = db.Created.Where(s => s.PrepareCount > 0).ToList();
        Assert.Equal(21, doInitStatements.Count);
        Assert.All(doInitStatements, s => Assert.Equal(1, s.FinalizeCount));
    }

    [Fact]
    public void Final_IsIdempotent_SecondCallFinalizesNothing()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.DoFinal();
        unit.DoFinal();

        // 原文 340-464 每段都判 <> nil 并置 nil，所以第二次是空转。
        Assert.All(db.Created.Where(s => s.PrepareCount > 0), s => Assert.Equal(1, s.FinalizeCount));
    }

    [Fact]
    public void Final_WithoutInit_DoesNotThrow()
    {
        DbLayerTestKit.Isolate();
        var db = new FakeSqliteDatabase();
        var unit = new TSqliteAuctionDB(new FakeDbLayerHost { DataBase = db }, db);

        unit.Final();   // 21 个字段全 nil → 21 个 if 都不进

        Assert.Empty(db.Created);
    }

    [Fact]
    public void Final_ThenReInit_PreparesAgain()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.DoFinal();
        unit.DoInit();

        // 原文 205-299 的 21 个字段在 DoFinal 里被置 nil，所以第二次 DoInit 会重新
        // AddSQLStatement 并 Prepare。FakeSqliteDatabase 按名复用返回**同一实例** →
        // PrepareCount 从 1 变 2（DbLayerTestKit.cs:257-267 的同名复用语义）。
        Assert.All(db.Created.Where(s => s.PrepareCount > 0), s => Assert.Equal(2, s.PrepareCount));
        Assert.Equal(21, db.Created.Count(s => s.PrepareCount > 0));
    }

    // ==================================================================
    // 3. QueryAllItems —— 原文 761-914（public 包装层 DbBases.cs:701-730）
    // ==================================================================

    [Fact]
    public void QueryAllItems_EmptyResult_ReturnsZeroAndEmptyList()
    {
        // 不预建临时语句：用来证明它确实不在 DoInit 的 21 条里。
        var (unit, db, _, _, _) = NewSqliteWithoutTemporary();
        var list = new TAuctionItemList();

        int n = unit.QueryAllItems("", -1, "", 1, -1, 0, 0, true, 0, 0, 0, list);

        Assert.Equal(0, n);
        Assert.Equal(0, list.Count);
        // 原文 773：语句是临时 AddSQLStatement('Auction_QueryAllItems')（不在 DoInit 的 21 条里）。
        Assert.True(db.Has("Auction_QueryAllItems"));
        Assert.DoesNotContain("Auction_QueryAllItems", db.AddedNames.Take(21));
        Assert.Equal(1, db.For("Auction_QueryAllItems").StepCount);
    }

    [Fact]
    public void QueryAllItems_TwoRows_ReturnsTwoRecordsInColumnOrder()
    {
        var (unit, db, host, _, _) = NewSqlite();
        db.For("Auction_QueryAllItems").AddRow(
            AllItemsRow(11, "Alice", 1700000000, 24, 3600, 100, 500, 2, 150, "Bob", 0, 0, 1));
        db.For("Auction_QueryAllItems").AddRow(
            AllItemsRow(12, "Carol", 1700099999, 12, -5, 200, 900, 1, 0, "", 1, 1, 0));
        var list = new TAuctionItemList();

        int n = unit.QueryAllItems("", -1, "Alice", 1, -1, 0, 0, true, 0, 0, 0, list);

        Assert.Equal(2, n);
        Assert.Equal(2, list.Count);
        Assert.Equal(11, list[0]!.AuctionID);
        Assert.Equal("Alice", list[0]!.HumanName);
        Assert.Equal(24, list[0]!.AuctionTime);
        Assert.Equal(3600, list[0]!.TimeLeft);
        Assert.Equal(100u, list[0]!.StartingPrice);
        Assert.Equal(500u, list[0]!.SellingPrice);
        Assert.Equal(2, list[0]!.CurrencyType);
        Assert.Equal(150, list[0]!.LastBidPrice);
        Assert.Equal("Bob", list[0]!.LastBidder);
        Assert.Equal(0, list[0]!.TradingStatus);
        Assert.False(list[0]!.IsItemGive);
        Assert.True(list[0]!.IsAttention);       // 第 13 列
        Assert.Equal(12, list[1]!.AuctionID);
        Assert.True(list[1]!.IsItemGive);
        Assert.False(list[1]!.IsAttention);
        // 原文 897：每行都 Owner.LoadItemFromDB(@ActionItem, AuctionID, AUCTION_ITEM_TYPE, 0) → ItemType=5/ItemIndex=0。
        Assert.Equal(new[] { "11/5/0", "12/5/0" }, host.LoadedItems.ToArray());
    }

    [Fact]
    public void QueryAllItems_BindsHumanNameThenPageCountThenOffset()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.QueryAllItems("", -1, "Alice", 3, -1, 0, 0, true, 0, 0, 0, new TAuctionItemList());

        // 原文 876-878：OrderBindText(HumanName); OrderBindInt(AUCTION_PAGE_COUNT); OrderBindInt((nPage-1)*AUCTION_PAGE_COUNT)。
        var binds = db.For("Auction_QueryAllItems").LastBinds;
        Assert.Equal(new[] { "text", "int", "int" }, binds.Select(b => b.Kind).ToArray());
        Assert.Equal("Alice", binds[0].Text);
        Assert.Equal(7, Convert.ToInt32(binds[1].Value));            // Grobal2Const.AUCTION_PAGE_COUNT = 7
        Assert.Equal(14, Convert.ToInt32(binds[2].Value));           // (3-1)*7
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void QueryAllItems_OutOfRangePage_DoesNotClampTheOffset(int nPage)
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.QueryAllItems("", -1, "H", nPage, -1, 0, 0, true, 0, 0, 0, new TAuctionItemList());

        // 原文 878 无下界校验：offset 就是 (nPage - 1) * 7。
        Assert.Equal(unchecked((nPage - 1) * 7), Convert.ToInt32(db.For("Auction_QueryAllItems").LastBinds[2].Value));
    }

    [Fact]
    public void QueryAllItems_SortFieldOutOfRange_LeavesOrderByEmpty()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.QueryAllItems("", -1, "H", 1, -1, 0, 99, true, 0, 0, 0, new TAuctionItemList());

        // 原文 846-870：sortField 不在 0..3 时 sOrderBy 保持空串（原文如此）。
        string sql = db.For("Auction_QueryAllItems").Sql;
        Assert.DoesNotContain(" order by ", sql);
        Assert.EndsWith(" limit ? offset ?;", sql);
    }

    [Theory]
    [InlineData(1, true, " order by ifnull(A.LastBidPrice, A.StartingPrice)")]
    [InlineData(1, false, " order by ifnull(A.LastBidPrice, A.StartingPrice) desc")]
    [InlineData(2, true, " order by A.SellingPrice")]
    [InlineData(2, false, " order by A.SellingPrice desc")]
    [InlineData(0, true, " order by AuctionID desc")]        // 原文 867-870：SortField=0 走固定串
    public void QueryAllItems_SortField_ProducesTheOriginalOrderBy(int sortField, bool asc, string expected)
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.QueryAllItems("", -1, "H", 1, -1, 0, sortField, asc, 0, 0, 0, new TAuctionItemList());

        Assert.Contains(expected, db.For("Auction_QueryAllItems").Sql);
    }

    [Fact]
    public void QueryAllItems_SortField3_UsesTheStrtimeNowExpression()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.QueryAllItems("", -1, "H", 1, -1, 0, 3, true, 0, 0, 0, new TAuctionItemList());

        // 原文 863/865 逐字。
        Assert.Contains(" order by (A.AddDateTime + A.AuctionTime * 3600) - strftime(\"%s\", \"now\")",
            db.For("Auction_QueryAllItems").Sql);
    }

    [Fact]
    public void QueryAllItems_TopmostAuctionIdAndFilters_AreConcatenatedVerbatim()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.QueryAllItems("剑", 5, "H", 1, 42, 0, 0, true, 3, 11, 99, new TAuctionItemList());

        string sql = db.For("Auction_QueryAllItems").Sql;
        // 原文 780-781：and (A.AuctionID <> 42) ；785：and (ItemGroup = 5) ；828：A.CurrencyType = 2（MoneyType-1）
        // 833/838：A.SellingPrice >= 11 / <= 99 ；843：like "%剑%" ×2。
        Assert.Contains("and (A.AuctionID <> 42) ", sql);
        Assert.Contains(" and (ItemGroup = 5)", sql);
        Assert.Contains(" and A.CurrencyType = 2", sql);
        Assert.Contains(" and A.SellingPrice >= 11", sql);
        Assert.Contains(" and A.SellingPrice <= 99", sql);
        Assert.Contains(" and ((A.ItemDBName like \"%剑%\") or (A.ItemName like \"%剑%\"))", sql);
    }

    [Theory]
    [InlineData(1, "37")]
    [InlineData(2, "37")]
    [InlineData(3, "37,37")]
    [InlineData(63, "37,37,37,37,37,37")]
    public void QueryAllItems_ItemColorsBitmask_BuildsTheColorListWithTrailingCommaTrimmed(int mask, string expected)
    {
        var (unit, db, _, env, _) = NewSqlite();
        env.btAuctionItemColors = 37;

        unit.QueryAllItems("", -1, "H", 1, -1, mask, 0, true, 0, 0, 0, new TAuctionItemList());

        // 原文 788-824：按 1/2/4/8/16/32 逐位加 "<color>,"，再用 Copy(sColors,1,Length-1) 去掉尾逗号。
        Assert.Contains(" and ItemColor in (" + expected + ")", db.For("Auction_QueryAllItems").Sql);
    }

    [Fact]
    public void QueryAllItems_ItemColorsZero_SkipsTheColorClause()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.QueryAllItems("", -1, "H", 1, -1, 0, 0, true, 0, 0, 0, new TAuctionItemList());

        // 原文 788：if ItemColors <> 0 then ...
        Assert.DoesNotContain("ItemColor in", db.For("Auction_QueryAllItems").Sql);
    }

    [Fact]
    public void QueryAllItems_SqlInjectionCharactersInItemName_AreConcatenatedNotBound()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.QueryAllItems(Injection, -1, "H", 1, -1, 0, 0, true, 0, 0, 0, new TAuctionItemList());

        // 原文 843 把 ItemName 直接拼进 SQL（本参数不过绑定）——逐字锁死该形态，防止"顺手改成绑定"。
        var stmt = db.For("Auction_QueryAllItems");
        Assert.Contains("like \"%" + Injection + "%\"", stmt.Sql);
        Assert.DoesNotContain(Injection, stmt.LastBinds.Select(b => b.Text));
    }

    [Fact]
    public void QueryAllItems_SqlInjectionCharactersInHumanName_AreBound()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.QueryAllItems("", -1, Injection, 1, -1, 0, 0, true, 0, 0, 0, new TAuctionItemList());

        // 原文 876：OrderBindText(HumanName) —— HumanName 是绑定参数，不进 SQL 文本。
        var stmt = db.For("Auction_QueryAllItems");
        Assert.Equal(Injection, stmt.LastBinds[0].Text);
        Assert.DoesNotContain(Injection, stmt.Sql);
    }

    [Fact]
    public void QueryAllItems_StepThrows_LogsAndReturnsZero()
    {
        var (unit, db, _, log) = NewSqliteThrowing("Auction_QueryAllItems", new InvalidOperationException("step-boom"));
        var list = new TAuctionItemList();

        int n = unit.QueryAllItems("", -1, "H", 1, -1, 0, 0, true, 0, 0, 0, list);

        // 原文 908-913：except on E: Exception do MainOutMessage(E.Message)；Result 保持 0。
        Assert.Equal(0, n);
        Assert.Equal(0, list.Count);
        Assert.Contains("step-boom", log.Messages);
        // 原文 904-907：finally 里 Reset + Finalize 仍执行。
        Assert.Equal(1, db.For("Auction_QueryAllItems").FinalizeCount);
    }

    [Fact]
    public void QueryAllItems_PublicWrapperLocksAndUnlocks()
    {
        var (unit, _, host, _, _) = NewSqlite();

        unit.QueryAllItems("", -1, "H", 1, -1, 0, 0, true, 0, 0, 0, new TAuctionItemList());

        // DbBases.cs:701-730 的 public 包装层（原文 M2DataCommon.pas:1236-1264）。
        Assert.Equal(1, host.LockCount);
        Assert.Equal(1, host.UnLockCount);
    }

    // ==================================================================
    // 4. QueryMyItems —— 原文 916-962
    // ==================================================================

    [Fact]
    public void QueryMyItems_EmptyResult_ReturnsZero()
    {
        var (unit, _, _, _, _) = NewSqlite();
        var list = new TAuctionItemList();

        int n = unit.QueryMyItems("Alice", 1, list);

        Assert.Equal(0, n);
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void QueryMyItems_TwoRows_ReturnsRecordsWithoutAttentionColumn()
    {
        var (unit, db, host, _, _) = NewSqlite();
        db.For("Auction_QueryAuctionItems").AddRow(
            MyItemsRow(21, "Alice", 1700000000, 24, 100, 10, 20, 0, 15, "Dave", 0, 0));
        db.For("Auction_QueryAuctionItems").AddRow(
            MyItemsRow(22, "Alice", 1700000001, 48, -100, 30, 40, 1, 0, "", 2, 1));
        var list = new TAuctionItemList();

        int n = unit.QueryMyItems("Alice", 1, list);

        Assert.Equal(2, n);
        Assert.Equal(21, list[0]!.AuctionID);
        Assert.Equal(10u, list[0]!.StartingPrice);
        Assert.Equal("Dave", list[0]!.LastBidder);
        // 原文 931-944 的 12 列里没有 IsAttention → 保持默认 false。
        Assert.False(list[0]!.IsAttention);
        Assert.Equal(2, list[1]!.TradingStatus);
        Assert.True(list[1]!.IsItemGive);
        Assert.Equal(new[] { "21/5/0", "22/5/0" }, host.LoadedItems.ToArray());
    }

    [Fact]
    public void QueryMyItems_BindsHumanNamePageAndOffset()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.QueryMyItems("Alice", 2, new TAuctionItemList());

        // 原文 926-928。
        var binds = db.For("Auction_QueryAuctionItems").LastBinds;
        Assert.Equal("Alice", binds[0].Text);
        Assert.Equal(7, Convert.ToInt32(binds[1].Value));
        Assert.Equal(7, Convert.ToInt32(binds[2].Value));   // (2-1)*7
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void QueryMyItems_NegativeOrZeroPage_PassesTheRawOffsetThrough(int nPage)
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.QueryMyItems("H", nPage, new TAuctionItemList());

        Assert.Equal(unchecked((nPage - 1) * 7), Convert.ToInt32(db.For("Auction_QueryAuctionItems").LastBinds[2].Value));
    }

    [Fact]
    public void QueryMyItems_SqlInjectionInHumanName_IsBoundNotConcatenated()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.QueryMyItems(Injection, 1, new TAuctionItemList());

        var stmt = db.For("Auction_QueryAuctionItems");
        Assert.Equal(Injection, stmt.LastBinds[0].Text);
        Assert.DoesNotContain(Injection, stmt.Sql);
    }

    [Fact]
    public void QueryMyItems_StepThrows_LogsAndReturnsZero()
    {
        var (unit, _, _, log) = NewSqliteThrowing("Auction_QueryAuctionItems",
            new InvalidOperationException("my-items-boom"));

        int n = unit.QueryMyItems("H", 1, new TAuctionItemList());

        Assert.Equal(0, n);
        Assert.Contains("my-items-boom", log.Messages);
    }

    [Fact]
    public void QueryMyItems_AlwaysResetsTheStatement()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.QueryMyItems("H", 1, new TAuctionItemList());

        // 原文 925 Reset（进循环前）+ 954 finally Reset → 2 次。
        Assert.Equal(2, db.For("Auction_QueryAuctionItems").ResetCount);
    }

    // ==================================================================
    // 5. QueryMyAttentionItems —— 原文 964-1017
    // ==================================================================

    [Fact]
    public void QueryMyAttentionItems_EmptyResult_ReturnsZero()
    {
        var (unit, _, _, _, _) = NewSqlite();
        var list = new TAuctionItemList();

        int n = unit.QueryMyAttentionItems("Alice", 1, list);

        Assert.Equal(0, n);
        Assert.Equal(0, list.Count);
    }

    [Fact]
    public void QueryMyAttentionItems_OneId_ThenLoadsTheFullRecordViaQueryOneItem()
    {
        var (unit, db, host, _, _) = NewSqlite();
        // 原文 976-1005：先取 AuctionID，再用 FStatementQueryOneItem 查该条完整信息。
        db.For("Auction_QueryAttentionItems").AddRow(31);
        db.For("Auction_QueryOneItem").AddRow(
            OneItemRow("Seller", 1700000000, 6, 60, 7, 8, 3, 9, "Bidder", 0, 0));
        var list = new TAuctionItemList();

        int n = unit.QueryMyAttentionItems("Alice", 1, list);

        Assert.Equal(1, n);
        Assert.Equal(31, list[0]!.AuctionID);
        Assert.Equal("Seller", list[0]!.HumanName);          // 来自 QueryOneItem
        Assert.Equal(7u, list[0]!.StartingPrice);
        Assert.Equal("Bidder", list[0]!.LastBidder);
        Assert.Equal(3, list[0]!.CurrencyType);
        Assert.Equal(1, db.For("Auction_QueryOneItem").StepCount);
        Assert.Equal(new[] { "31/5/0" }, host.LoadedItems.ToArray());
    }

    [Fact]
    public void QueryMyAttentionItems_AuctionIdStillAdded_WhenQueryOneItemFindsNothing()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_QueryAttentionItems").AddRow(41);
        // QueryOneItem 无行 → 只有 AuctionID 被填，其余保持默认（原文 984-997 不判 else）。
        var list = new TAuctionItemList();

        int n = unit.QueryMyAttentionItems("Alice", 1, list);

        Assert.Equal(1, n);
        Assert.Equal(41, list[0]!.AuctionID);
        Assert.Equal("", list[0]!.HumanName);
        Assert.Equal(0u, list[0]!.StartingPrice);
    }

    [Fact]
    public void QueryMyAttentionItems_BindsHumanNamePageAndOffset()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.QueryMyAttentionItems("Alice", 2, new TAuctionItemList());

        // 原文 973-975。
        var binds = db.For("Auction_QueryAttentionItems").LastBinds;
        Assert.Equal("Alice", binds[0].Text);
        Assert.Equal(7, Convert.ToInt32(binds[1].Value));
        Assert.Equal(7, Convert.ToInt32(binds[2].Value));
    }

    [Fact]
    public void QueryMyAttentionItems_ResetsBothStatements()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_QueryAttentionItems").AddRow(51);

        unit.QueryMyAttentionItems("H", 1, new TAuctionItemList());

        var one = db.For("Auction_QueryOneItem");
        var att = db.For("Auction_QueryAttentionItems");
        // 原文 982 Reset（每行）+ 1008 finally Reset → 有 1 行时 2 次。
        Assert.Equal(2, one.ResetCount);
        Assert.Equal(2, att.ResetCount);    // 972 + 1009 finally
    }

    [Fact]
    public void QueryMyAttentionItems_StepThrows_LogsAndReturnsZero()
    {
        var (unit, _, _, log) = NewSqliteThrowing("Auction_QueryAttentionItems",
            new InvalidOperationException("att-boom"));

        int n = unit.QueryMyAttentionItems("H", 1, new TAuctionItemList());

        Assert.Equal(0, n);
        Assert.Contains("att-boom", log.Messages);
    }

    [Fact]
    public void QueryMyAttentionItems_SqlInjectionInHumanName_IsBound()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.QueryMyAttentionItems(Injection, 1, new TAuctionItemList());

        Assert.Equal(Injection, db.For("Auction_QueryAttentionItems").LastBinds[0].Text);
    }

    // ==================================================================
    // 6. GetAllItemsPageCount —— 原文 1092-1183
    // ==================================================================

    [Fact]
    public void GetAllItemsPageCount_NoRow_ReturnsZero()
    {
        var (unit, _, _, _, _) = NewSqlite();

        int n = unit.GetAllItemsPageCount("", -1, 0, 0, 0, 0);

        Assert.Equal(0, n);
    }

    [Theory]
    [InlineData(0, 0)]      // 0 条 → 0 页
    [InlineData(1, 1)]      // 1 条 → 1 页
    [InlineData(7, 1)]      // 恰好 1 页（AUCTION_PAGE_COUNT = 7）
    [InlineData(8, 2)]      // 8 条 → 2 页
    [InlineData(14, 2)]
    [InlineData(15, 3)]
    public void GetAllItemsPageCount_CeilingDivisionBySeven(int count, int expected)
    {
        var (unit, db, _, _, _) = NewSqlite();
        // 原文 1172：(Count + AUCTION_PAGE_COUNT - 1) div AUCTION_PAGE_COUNT。
        db.For("Auction_GetAllItemsCount").AddRow(count);

        int n = unit.GetAllItemsPageCount("", -1, 0, 0, 0, 0);

        Assert.Equal(expected, n);
    }

    [Fact]
    public void GetAllItemsPageCount_UsesItsOwnTemporaryStatementAndFinalizesIt()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.GetAllItemsPageCount("", -1, 0, 0, 0, 0);

        // 原文 1100：临时 AddSQLStatement('Auction_GetAllItemsCount')；1175：finally sm.Finalize。
        Assert.True(db.Has("Auction_GetAllItemsCount"));
        Assert.Equal(1, db.For("Auction_GetAllItemsCount").FinalizeCount);
        // 本夹具为预置结果集在 DoInit 之前就登记了它（Fake 同名复用），故不再断言 AddedNames 的前 21 条；
        // "它是方法内现建的临时语句"由 FinalizeCount==1（只在本方法里 Finalize 过）体现。
    }

    [Fact]
    public void GetAllItemsPageCount_Filters_AreConcatenatedVerbatim()
    {
        var (unit, db, _, env, _) = NewSqlite();
        env.btAuctionItemColors = 5;

        unit.GetAllItemsPageCount("剑", 5, 3, 1, 11, 99);

        string sql = db.For("Auction_GetAllItemsCount").Sql;
        // 原文 1108/1145/1151/1156/1161/1166（注意这里的列名没有 A. 前缀，与 DoQueryAllItems 不同）。
        Assert.Contains(" and ItemGroup = 5", sql);
        Assert.Contains(" and ItemColor in (5,5)", sql);
        Assert.Contains(" and CurrencyType = 0", sql);      // MoneyType - 1
        Assert.Contains(" and SellingPrice >= 11", sql);
        Assert.Contains(" and SellingPrice <= 99", sql);
        Assert.Contains(" and ((ItemDBName like \"%剑%\") or (ItemName like \"%剑%\"))", sql);
    }

    [Fact]
    public void GetAllItemsPageCount_SwapsMinAndMax_WhenMinGreaterThanMax()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_GetAllItemsCount").AddRow(1);

        // DbBases.cs:787-792（原文 M2DataCommon.pas:1302-1328）：MinPrices > MaxPrices 时交换。
        unit.GetAllItemsPageCount("", -1, 0, 0, 999, 11);

        string sql = db.For("Auction_GetAllItemsCount").Sql;
        Assert.Contains(" and SellingPrice >= 11", sql);
        Assert.Contains(" and SellingPrice <= 999", sql);
    }

    [Fact]
    public void GetAllItemsPageCount_ZeroPrices_SkipBothPriceClauses()
    {
        var (unit, db, _, _, _) = NewSqlite();

        // 原文 1154/1159：MinPrices/MaxPrices 都是 > 0 才拼（0 不参与）。
        unit.GetAllItemsPageCount("", -1, 0, 0, 0, 0);

        string sql = db.For("Auction_GetAllItemsCount").Sql;
        Assert.DoesNotContain("SellingPrice >=", sql);
        Assert.DoesNotContain("SellingPrice <=", sql);
    }

    [Fact]
    public void GetAllItemsPageCount_SqlInjectionInItemName_IsConcatenated()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.GetAllItemsPageCount(Injection, -1, 0, 0, 0, 0);

        Assert.Contains("like \"%" + Injection + "%\"", db.For("Auction_GetAllItemsCount").Sql);
    }

    [Fact]
    public void GetAllItemsPageCount_StepThrows_LogsAndReturnsZero()
    {
        var (unit, _, _, log) = NewSqliteThrowing("Auction_GetAllItemsCount",
            new InvalidOperationException("count-boom"));

        int n = unit.GetAllItemsPageCount("", -1, 0, 0, 0, 0);

        Assert.Equal(0, n);
        Assert.Contains("count-boom", log.Messages);
    }

    // ==================================================================
    // 7. GetMyItemsPageCount —— 原文 1185-1198（★ 无 except）
    // ==================================================================

    [Fact]
    public void GetMyItemsPageCount_NoRow_ReturnsZero()
    {
        var (unit, _, _, _, _) = NewSqlite();

        Assert.Equal(0, unit.GetMyItemsPageCount("Alice"));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(7, 1)]
    [InlineData(8, 2)]
    [InlineData(21, 3)]
    public void GetMyItemsPageCount_CeilingDivisionBySeven(int count, int expected)
    {
        var (unit, db, _, _, _) = NewSqlite();
        // 原文 1193：(Count + AUCTION_PAGE_COUNT - 1) div AUCTION_PAGE_COUNT。
        db.For("Auction_GetMyItemsCount").AddRow(count);

        Assert.Equal(expected, unit.GetMyItemsPageCount("Alice"));
    }

    [Fact]
    public void GetMyItemsPageCount_BindsHumanNameOnly()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.GetMyItemsPageCount(Injection);

        // 原文 1190：只绑 HumanName（无 limit/offset）。
        var binds = db.For("Auction_GetMyItemsCount").LastBinds;
        Assert.Single(binds);
        Assert.Equal(Injection, binds[0].Text);
    }

    [Fact]
    public void GetMyItemsPageCount_HasNoExceptOnlyFinally_StepErrorEscapesToTheWrapper()
    {
        var (unit, db, _, log) = NewSqliteThrowing("Auction_GetMyItemsCount",
            new InvalidOperationException("my-count-boom"));

        // ★ 原文缺陷（SqliteAuctionDB.pas:1185-1198）：本方法**没有** except 块，只有 finally，
        // 所以异常会穿过 DoGetMyItemsPageCount，被基类 DbBases.cs:808-819 的包装层捕获并记日志。
        int n = unit.GetMyItemsPageCount("H");

        Assert.Equal(0, n);
        Assert.Contains(log.Messages, m => m.Contains("TAuctionDB:GetMyItemsPageCount") && m.Contains("my-count-boom"));
        // finally 仍然 Reset（原文 1195-1197）→ 前置 1189 + finally 1196 = 2 次。
        Assert.Equal(2, db.For("Auction_GetMyItemsCount").ResetCount);
    }

    // ==================================================================
    // 8. GetMyAttentionPageCount —— 原文 1200-1220（对比：有 except）
    // ==================================================================

    [Fact]
    public void GetMyAttentionPageCount_NoRow_ReturnsZero()
    {
        var (unit, _, _, _, _) = NewSqlite();

        Assert.Equal(0, unit.GetMyAttentionPageCount("Alice"));
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(7, 1)]
    [InlineData(8, 2)]
    public void GetMyAttentionPageCount_CeilingDivisionBySeven(int count, int expected)
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_GetMyAttentionItemsCount").AddRow(count);

        Assert.Equal(expected, unit.GetMyAttentionPageCount("Alice"));
    }

    [Fact]
    public void GetMyAttentionPageCount_BindsHumanNameAndResetsInFinally()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.GetMyAttentionPageCount(Injection);

        var stmt = db.For("Auction_GetMyAttentionItemsCount");
        Assert.Single(stmt.LastBinds);
        Assert.Equal(Injection, stmt.LastBinds[0].Text);
        Assert.Equal(2, stmt.ResetCount);   // 原文 1205 进 try 前 + 1212 finally
    }

    [Fact]
    public void GetMyAttentionPageCount_HasItsOwnExcept_SoTheErrorIsLoggedHere()
    {
        var (unit, _, _, log) = NewSqliteThrowing("Auction_GetMyAttentionItemsCount",
            new InvalidOperationException("att-count-boom"));

        // 对比 GetMyItemsPageCount：原文 1214-1218 有 except → 在 Do* 内部就吞掉，
        // 包装层（DbBases.cs:821-832）不会再记 "TAuctionDB:GetMyAttentionPageCount" 前缀。
        int n = unit.GetMyAttentionPageCount("H");

        Assert.Equal(0, n);
        Assert.Contains("att-count-boom", log.Messages);
        Assert.DoesNotContain(log.Messages, m => m.Contains("TAuctionDB:GetMyAttentionPageCount"));
    }

    // ==================================================================
    // 9. GetMyAuctioningItemsCount —— 原文 1222-1242
    // ==================================================================

    [Fact]
    public void GetMyAuctioningItemsCount_NoRow_ReturnsZero()
    {
        var (unit, _, _, _, _) = NewSqlite();

        Assert.Equal(0, unit.GetMyAuctioningItemsCount("Alice"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(int.MaxValue)]
    public void GetMyAuctioningItemsCount_ReturnsRawCountWithoutPageDivision(int count)
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_GetMyAuctioningItemsCount").AddRow(count);

        // 原文 1231：Result := OrderGetColumnValueInt（**不**做除法，与 PageCount 不同）。
        Assert.Equal(count, unit.GetMyAuctioningItemsCount("Alice"));
    }

    [Fact]
    public void GetMyAuctioningItemsCount_BindsHumanNameAndResets()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.GetMyAuctioningItemsCount(Injection);

        var stmt = db.For("Auction_GetMyAuctioningItemsCount");
        Assert.Single(stmt.LastBinds);
        Assert.Equal(Injection, stmt.LastBinds[0].Text);
        Assert.Equal(2, stmt.ResetCount);   // 原文 1227 + 1234 finally
    }

    [Fact]
    public void GetMyAuctioningItemsCount_StepThrows_LogsAndReturnsZero()
    {
        var (unit, _, _, log) = NewSqliteThrowing("Auction_GetMyAuctioningItemsCount",
            new InvalidOperationException("auc-count-boom"));

        Assert.Equal(0, unit.GetMyAuctioningItemsCount("H"));
        Assert.Contains("auc-count-boom", log.Messages);
    }

    // ==================================================================
    // 10. GetMySellFailItemsCount —— 原文 1244-1265
    // ==================================================================

    [Fact]
    public void GetMySellFailItemsCount_NoRow_ReturnsZero()
    {
        var (unit, _, _, _, _) = NewSqlite();

        Assert.Equal(0, unit.GetMySellFailItemsCount("Alice"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    public void GetMySellFailItemsCount_ReturnsRawCount(int count)
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_GetMySellFailItemsCount").AddRow(count);

        Assert.Equal(count, unit.GetMySellFailItemsCount("Alice"));
    }

    [Fact]
    public void GetMySellFailItemsCount_BindsHumanName()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.GetMySellFailItemsCount(Injection);

        Assert.Equal(Injection, db.For("Auction_GetMySellFailItemsCount").LastBinds[0].Text);
    }

    [Fact]
    public void GetMySellFailItemsCount_StepThrows_LogsAndReturnsZero()
    {
        var (unit, _, _, log) = NewSqliteThrowing("Auction_GetMySellFailItemsCount",
            new InvalidOperationException("fail-boom"));

        Assert.Equal(0, unit.GetMySellFailItemsCount("H"));
        Assert.Contains("fail-boom", log.Messages);
    }

    // ==================================================================
    // 11. GetMyBuyOKItemsCount —— 原文 1267-1288
    // ==================================================================

    [Fact]
    public void GetMyBuyOKItemsCount_NoRow_ReturnsZero()
    {
        var (unit, _, _, _, _) = NewSqlite();

        Assert.Equal(0, unit.GetMyBuyOKItemsCount("Alice"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9)]
    public void GetMyBuyOKItemsCount_ReturnsRawCount(int count)
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_GetMyBuyOKItemsCount").AddRow(count);

        Assert.Equal(count, unit.GetMyBuyOKItemsCount("Alice"));
    }

    [Fact]
    public void GetMyBuyOKItemsCount_BindsHumanName()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.GetMyBuyOKItemsCount(Injection);

        Assert.Equal(Injection, db.For("Auction_GetMyBuyOKItemsCount").LastBinds[0].Text);
    }

    [Fact]
    public void GetMyBuyOKItemsCount_StepThrows_LogsAndReturnsZero()
    {
        var (unit, _, _, log) = NewSqliteThrowing("Auction_GetMyBuyOKItemsCount",
            new InvalidOperationException("buyok-boom"));

        Assert.Equal(0, unit.GetMyBuyOKItemsCount("H"));
        Assert.Contains("buyok-boom", log.Messages);
    }

    // ==================================================================
    // 12. AddAuctionItem —— 原文 467-592
    // ==================================================================

    private static TUserItem MakeUserItem(int btValue13, string name, byte color)
    {
        var item = new TUserItem();
        M2ItemDbAccess.SetValue(ref item, 13, btValue13);
        item.NameStr = name;
        item.btColor = color;
        return item;
    }

    [Fact]
    public void AddAuctionItem_HappyPath_InsertsSavesAndCommits_ReturningTheAuctionId()
    {
        var (unit, db, host, _, _) = NewSqlite();
        db.For("Auction_GetMaxAuctionID").AddRow(77);
        var std = new FakeAuctionStdItem { StdMode = 5, Color = 9, DBName = "WoodenSword" };

        int id = unit.AddAuctionItem("Alice", 24, 100, 500, 2, MakeUserItem(0, "", 0), std);

        Assert.Equal(77, id);
        // 原文 547-583：BeginTransaction → Insert Step 成功 → SaveItemToDB → Commit。
        Assert.Equal(new[] { "begin", "commit" }, db.Transactions.ToArray());
        Assert.Equal(new[] { "77/5/0/0" }, host.SavedItems.ToArray());
    }

    [Fact]
    public void AddAuctionItem_BindsTenParametersInOriginalOrder()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_GetMaxAuctionID").AddRow(10);
        var std = new FakeAuctionStdItem { StdMode = 5, Color = 9, DBName = "Sword" };

        unit.AddAuctionItem("Alice", 24, 100, 500, 2, MakeUserItem(0, "", 0), std);

        // 原文 551-571：AuctionID, HumanName, ItemGroup, ItemColor, AuctionTime,
        //               StartingPrice, SellingPrice, CurrencyType, DBName, ItemName。
        var b = db.For("Auction_InsertAuctionItem").LastBinds;
        Assert.Equal(new[] { "int", "text", "int", "int", "int", "int", "int", "int", "text", "text" },
            b.Select(x => x.Kind).ToArray());
        Assert.Equal(10, Convert.ToInt32(b[0].Value));
        Assert.Equal("Alice", b[1].Text);
        Assert.Equal(1, Convert.ToInt32(b[2].Value));   // StdMode=5 → igWeapon = 1
        Assert.Equal(9, Convert.ToInt32(b[3].Value));   // btColor=0 → 用 StdItem.Color
        Assert.Equal(24, Convert.ToInt32(b[4].Value));
        Assert.Equal(100, Convert.ToInt32(b[5].Value));
        Assert.Equal(500, Convert.ToInt32(b[6].Value));
        Assert.Equal(2, Convert.ToInt32(b[7].Value));
        Assert.Equal("Sword", b[8].Text);
        Assert.Equal("", b[9].Text);
    }

    [Theory]
    // 原文 486-543 的分组判定（枚举序数见 Grobal2.Types5.cs TItemGroup）。
    [InlineData(10, 0, "igDress")]        // 486-487 衣服
    [InlineData(11, 0, "igDress")]
    [InlineData(5, 0, "igWeapon")]        // 488-489 武器
    [InlineData(6, 0, "igWeapon")]
    [InlineData(28, 0, "igSpecial")]      // ★ 490-491「照明物」先吃 28 → 534-535「马牌」是死代码
    [InlineData(30, 0, "igSpecial")]
    [InlineData(19, 0, "igNecklace")]     // 492-498 项链（OverLap 不在 2/4/6）
    [InlineData(19, 2, "igSpecial")]      // OverLap in [2,4,6] → igSpecial
    [InlineData(15, 0, "igHelmet")]       // 499-505 头盔
    [InlineData(15, 4, "igSpecial")]      // StdMode=15 且 OverLap in [2,4,6]
    [InlineData(78, 2, "igHelmet")]       // StdMode=78 时 OverLap 不参与判定
    [InlineData(24, 0, "igArmRing")]      // 506-512 手镯
    [InlineData(26, 4, "igSpecial")]
    [InlineData(22, 0, "igRing")]         // 513-519 戒指
    [InlineData(23, 6, "igSpecial")]
    [InlineData(25, 0, "igSpecial")]      // 520-521 符毒
    [InlineData(51, 0, "igSpecial")]
    [InlineData(54, 0, "igBelt")]         // 522-523 腰带
    [InlineData(64, 0, "igBelt")]
    [InlineData(52, 0, "igBoots")]        // 524-525 靴子
    [InlineData(62, 0, "igBoots")]
    [InlineData(53, 0, "igSpecial")]      // 526-527 宝石
    [InlineData(63, 0, "igSpecial")]
    [InlineData(7, 0, "igSpecial")]
    [InlineData(66, 0, "igFashion")]      // 528-529 时装（66..89）
    [InlineData(89, 0, "igFashion")]
    [InlineData(65, 0, "igSpecial")]      // 532-533 军鼓
    [InlineData(16, 0, "igSpecial")]      // 530-531 斗笠
    [InlineData(12, 0, "igSpecial")]      // 536-537 盾牌
    [InlineData(90, 0, "igSpecial")]      // 538-539 灵玉
    [InlineData(4, 0, "igSpecial")]       // 542-543 技能书籍
    [InlineData(0, 0, "igDrug")]          // 540-541 药品
    [InlineData(1, 0, "igDrug")]
    [InlineData(99, 0, "igOther")]        // 任何分支都不命中 → igOther
    public void AddAuctionItem_MapsStdModeToItemGroup(int stdMode, int overLap, string expectedGroupName)
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_GetMaxAuctionID").AddRow(1);
        var std = new FakeAuctionStdItem { StdMode = stdMode, OverLap = overLap, DBName = "X" };

        unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0), std);

        // 原文 485 起的分支链 → OrderBindInt(Integer(ItemGroup))（553）。
        Assert.Equal(ItemGroupOrdinal[expectedGroupName],
            Convert.ToInt32(db.For("Auction_InsertAuctionItem").LastBinds[2].Value));
    }

    [Fact]
    public void AddAuctionItem_DrugRequiresShape12_WhenStdModeIsThree()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_GetMaxAuctionID").AddRow(1);

        // 原文 540：(StdMode = 3) and (StdItem.Shape = 12) → igDrug（10）
        unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0), new FakeAuctionStdItem { StdMode = 3, Shape = 12 });
        Assert.Equal(10, Convert.ToInt32(db.For("Auction_InsertAuctionItem").LastBinds[2].Value));

        // Shape<>12 → 不命中药品，也无其它分支 → igOther（12）
        unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0), new FakeAuctionStdItem { StdMode = 3, Shape = 99 });
        Assert.Equal(12, Convert.ToInt32(db.For("Auction_InsertAuctionItem").LastBinds[2].Value));
    }

    [Fact]
    public void AddAuctionItem_StdMode28_IsSpecialNotHorseCard_MirroringTheDeadBranch()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_GetMaxAuctionID").AddRow(1);

        // ★ 原文缺陷（490-491 vs 534-535）：28 在第三个分支（照明物，与 30 同组）就被判为 igSpecial，
        // 第十四分支的「马牌」（534-535，也是 igSpecial）因此**永不可达**。
        // 两者结果恰巧相同（都是 11），用 11 锁死；再用 30 做同分支对照证明 28 已被照明物吃掉。
        unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0), new FakeAuctionStdItem { StdMode = 28 });
        Assert.Equal(11, Convert.ToInt32(db.For("Auction_InsertAuctionItem").LastBinds[2].Value));

        unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0), new FakeAuctionStdItem { StdMode = 30 });
        Assert.Equal(11, Convert.ToInt32(db.For("Auction_InsertAuctionItem").LastBinds[2].Value));
    }

    [Fact]
    public void AddAuctionItem_UserItemColorWinsWhenGreaterThanZero()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_GetMaxAuctionID").AddRow(1);
        var std = new FakeAuctionStdItem { StdMode = 5, Color = 9, DBName = "S" };

        // 原文 555-558：if UserItem.btColor > 0 then 绑 btColor else 绑 StdItem.Color。
        unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 200), std);
        Assert.Equal(200, Convert.ToInt32(db.For("Auction_InsertAuctionItem").LastBinds[3].Value));

        unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0), std);
        Assert.Equal(9, Convert.ToInt32(db.For("Auction_InsertAuctionItem").LastBinds[3].Value));
    }

    [Fact]
    public void AddAuctionItem_ChangeNameUsedWhenBtValue13IsOneAndNameIsNotEmpty()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_GetMaxAuctionID").AddRow(1);
        var renamed = "";
        DbLayerRunSeam.ProcessItemName = n => { renamed = n; return "[改名]" + n; };

        try
        {
            // 原文 568：if (UserItem.btValue[13] = 1) and (Length(UserItem.Name) > 0) then
            //             ChangeName := ProcessItemName(UserItem.Name);
            unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(1, "神剑", 0),
                new FakeAuctionStdItem { StdMode = 5, DBName = "S" });

            Assert.Equal("神剑", renamed);
            Assert.Equal("[改名]神剑", db.For("Auction_InsertAuctionItem").LastBinds[9].Text);
        }
        finally
        {
            DbLayerRunSeam.ResetDefaults();
        }
    }

    [Fact]
    public void AddAuctionItem_BtValue13IsTheJudgement_NotBtValue0()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_GetMaxAuctionID").AddRow(1);
        DbLayerRunSeam.ProcessItemName = _ => throw new InvalidOperationException("ProcessItemName must not be called");

        try
        {
            // ★ 原文缺陷（568）：判据是 btValue[13]，**不是** btValue[0]。
            // 这里 btValue[0]=1 而 btValue[13]=0 → 不查改名（若实现误用 btValue[0] 会抛异常）。
            var item = MakeUserItem(0, "神剑", 0);
            M2ItemDbAccess.SetValue(ref item, 0, 1);

            unit.AddAuctionItem("H", 1, 1, 1, 0, item, new FakeAuctionStdItem { StdMode = 5, DBName = "S" });

            Assert.Equal("", db.For("Auction_InsertAuctionItem").LastBinds[9].Text);
        }
        finally
        {
            DbLayerRunSeam.ResetDefaults();
        }
    }

    [Fact]
    public void AddAuctionItem_ChangeNameEmptyWhenBtValue13IsOneButNameIsEmpty()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_GetMaxAuctionID").AddRow(1);
        DbLayerRunSeam.ProcessItemName = _ => throw new InvalidOperationException("ProcessItemName must not be called");

        try
        {
            // 原文 568 的第二个合取项：Length(UserItem.Name) > 0 必须同时成立。
            unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(1, "", 0),
                new FakeAuctionStdItem { StdMode = 5, DBName = "S" });

            Assert.Equal("", db.For("Auction_InsertAuctionItem").LastBinds[9].Text);
        }
        finally
        {
            DbLayerRunSeam.ResetDefaults();
        }
    }

    [Fact]
    public void AddAuctionItem_NoMaxRow_UsesOneAndInserts()
    {
        var (unit, db, host, _, _) = NewSqlite();
        // 无预设行 → Step 返 SQLITE_DONE → 原文 480：AuctionID := 1。
        var std = new FakeAuctionStdItem { StdMode = 5, DBName = "S" };

        int id = unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0), std);

        Assert.Equal(1, id);
        Assert.Equal(1, Convert.ToInt32(db.For("Auction_InsertAuctionItem").LastBinds[0].Value));
        Assert.Single(host.SavedItems);
    }

    [Fact]
    public void AddAuctionItem_ZeroAuctionId_SilentlyReturnsZeroWithoutInsert_MirroringTheMissingElse()
    {
        var (unit, db, host, _, _) = NewSqlite();
        // ★ 原文缺陷（545-591）：`if AuctionID > 0 then begin ... end;` **没有 else**。
        // 让 GetMaxAuctionID 返回列值 0 → 插入块整段跳过：
        // → 不插入、不开事务、不保存物品、不报错，Result 保持 0。
        db.For("Auction_GetMaxAuctionID").AddRow(0);

        int id = unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0),
            new FakeAuctionStdItem { StdMode = 5, DBName = "S" });

        Assert.Equal(0, id);
        Assert.Empty(db.Transactions);
        Assert.Equal(0, db.For("Auction_InsertAuctionItem").StepCount);
        Assert.Empty(host.SavedItems);
    }

    [Fact]
    public void AddAuctionItem_InsertStepThrows_RollsBackAndReturnsZero()
    {
        var (unit, db, host, log) = NewSqliteThrowing("Auction_InsertAuctionItem",
            new InvalidOperationException("insert-boom"));
        db.For("Auction_GetMaxAuctionID").AddRow(5);

        int id = unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0),
            new FakeAuctionStdItem { StdMode = 5, DBName = "S" });

        // 原文 584-590：except → MainOutMessage + FDB.RollBack；Result 保持 0。
        Assert.Equal(0, id);
        Assert.Contains("insert-boom", log.Messages);
        Assert.Equal(new[] { "begin", "rollback" }, db.Transactions.ToArray());
        Assert.Empty(host.SavedItems);
    }

    [Fact]
    public void AddAuctionItem_InsertStepReturnsDone_StillCountsAsSuccess()
    {
        var (unit, db, host, _, _) = NewSqlite();
        db.For("Auction_GetMaxAuctionID").AddRow(5);
        // 原文 573：if Step in [SQLITE_OK, SQLITE_DONE] then ...（DONE 是 Insert 的常态，算成功）。
        db.For("Auction_InsertAuctionItem").StepCode = SqliteCodes.SQLITE_DONE;

        int id = unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0),
            new FakeAuctionStdItem { StdMode = 5, DBName = "S" });

        Assert.Equal(5, id);
        Assert.Equal(new[] { "begin", "commit" }, db.Transactions.ToArray());
        Assert.Single(host.SavedItems);
    }

    [Fact]
    public void AddAuctionItem_InsertStepReturnsRow_IsNotCountedAsSuccess()
    {
        var (unit, db, host, _, _) = NewSqlite();
        db.For("Auction_GetMaxAuctionID").AddRow(5);
        // 原文 573 只认 SQLITE_OK(0) / SQLITE_DONE(101)；给一行结果集让 Step 返回 ROW(100) → 不成功、不保存物品。
        db.For("Auction_InsertAuctionItem").AddRow(1);

        int id = unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0),
            new FakeAuctionStdItem { StdMode = 5, DBName = "S" });

        Assert.Equal(0, id);
        Assert.Empty(host.SavedItems);
        // 原文 583 仍在 try 内、finally 之后无条件 Commit（原文如此）。
        Assert.Equal(new[] { "begin", "commit" }, db.Transactions.ToArray());
    }

    [Fact]
    public void AddAuctionItem_SqlInjectionInHumanNameAndDbName_AreBound()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_GetMaxAuctionID").AddRow(1);

        unit.AddAuctionItem(Injection, 1, 1, 1, 0, MakeUserItem(0, "", 0),
            new FakeAuctionStdItem { StdMode = 5, DBName = Injection });

        // 原文 552/565：HumanName 与 StdItem.DBName 都走 OrderBindText。
        var b = db.For("Auction_InsertAuctionItem").LastBinds;
        Assert.Equal(Injection, b[1].Text);
        Assert.Equal(Injection, b[8].Text);
        Assert.DoesNotContain(Injection, db.For("Auction_InsertAuctionItem").Sql);
    }

    [Fact]
    public void AddAuctionItem_ResetsTheInsertStatementInFinally()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_GetMaxAuctionID").AddRow(1);

        unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0), new FakeAuctionStdItem { StdMode = 5 });

        // 原文 550 Reset + 579-581 finally Reset。
        Assert.Equal(2, db.For("Auction_InsertAuctionItem").ResetCount);
    }

    [Fact]
    public void AddAuctionItem_SavesWithAuctionItemTypeFive()
    {
        var (unit, db, host, _, _) = NewSqlite();
        db.For("Auction_GetMaxAuctionID").AddRow(33);

        unit.AddAuctionItem("H", 1, 1, 1, 0, MakeUserItem(0, "", 0), new FakeAuctionStdItem { StdMode = 5 });

        // 原文 575：Owner.SaveItemToDB(UserItem, AuctionID, AUCTION_ITEM_TYPE, 0) —— AUCTION_ITEM_TYPE = 5。
        Assert.Equal("33/5/0/0", host.SavedItems[0]);
        Assert.Equal(AuctionItemType, 5);
    }

    // ==================================================================
    // 13. CancelAuctionItem —— 原文 594-608
    // ==================================================================

    [Fact]
    public void CancelAuctionItem_ExecutesTheOriginalUpdateAndReturnsTrue()
    {
        var (unit, db, _, _, _) = NewSqlite();

        bool ok = unit.CancelAuctionItem("Alice", 42);

        Assert.True(ok);
        // 原文 599-600 逐字（HumanName 用双引号包裹 —— SQLite 方言下原样拼串）。
        Assert.Equal("update AuctionData set TradingStatus = 1 where AuctionID = 42 and HumanName = \"Alice\";",
            db.ExecutedSql.Single());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    public void CancelAuctionItem_PassesAnyAuctionIdThrough(int auctionId)
    {
        var (unit, db, _, _, _) = NewSqlite();

        Assert.True(unit.CancelAuctionItem("H", auctionId));
        Assert.Contains("where AuctionID = " + auctionId + " ", db.ExecutedSql.Single());
    }

    [Fact]
    public void CancelAuctionItem_SqlInjectionInHumanName_IsConcatenatedNotBound()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.CancelAuctionItem(Injection, 1);

        // 原文 599-600 是 FDB.Execute 的直拼串（**无绑定**）——逐字锁死该风险点。
        Assert.Contains("and HumanName = \"" + Injection + "\";", db.ExecutedSql.Single());
    }

    [Fact]
    public void CancelAuctionItem_ExecuteThrows_LogsAndReturnsFalse()
    {
        var (unit, _, _, log) = NewSqliteThrowing("Auction_UpdateAuctionItemSuccess",
            new InvalidOperationException("unused"), executeBoom: new InvalidOperationException("cancel-boom"));

        bool ok = unit.CancelAuctionItem("H", 1);

        // 原文 602-607：except → MainOutMessage；Result 保持 False。
        Assert.False(ok);
        Assert.Contains("cancel-boom", log.Messages);
    }

    [Fact]
    public void CancelAuctionItem_DoesNotUseTransactions()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.CancelAuctionItem("H", 1);

        // 原文 594-608 无 BeginTransaction/Commit。
        Assert.Empty(db.Transactions);
    }

    // ==================================================================
    // 14. RetrieveAuctionItem —— 原文 610-623
    // ==================================================================

    [Fact]
    public void RetrieveAuctionItem_ExecutesTheOriginalUpdateAndReturnsTrue()
    {
        var (unit, db, _, _, _) = NewSqlite();

        Assert.True(unit.RetrieveAuctionItem(42));
        Assert.Equal("update AuctionData set IsItemGive = 1 where AuctionID = 42;", db.ExecutedSql.Single());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-7)]
    public void RetrieveAuctionItem_PassesAnyAuctionIdThrough(int auctionId)
    {
        var (unit, db, _, _, _) = NewSqlite();

        Assert.True(unit.RetrieveAuctionItem(auctionId));
        Assert.Contains("where AuctionID = " + auctionId + ";", db.ExecutedSql.Single());
    }

    [Fact]
    public void RetrieveAuctionItem_ExecuteThrows_LogsAndReturnsFalse()
    {
        var (unit, _, _, log) = NewSqliteThrowing("Auction_UpdateAuctionItemSuccess",
            new InvalidOperationException("unused"), executeBoom: new InvalidOperationException("retrieve-boom"));

        Assert.False(unit.RetrieveAuctionItem(1));
        Assert.Contains("retrieve-boom", log.Messages);
    }

    [Fact]
    public void RetrieveAuctionItem_DoesNotUseTransactions()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.RetrieveAuctionItem(1);

        Assert.Empty(db.Transactions);
    }

    // ==================================================================
    // 15. DeleteAuctionItem —— 原文 625-653（★ Items 段多一个 ';'）
    // ==================================================================

    [Fact]
    public void DeleteAuctionItem_BuildsTheTwelveStatementScriptInOriginalOrder()
    {
        var (unit, db, _, _, _) = NewSqlite();

        bool ok = unit.DeleteAuctionItem("Alice", 42);

        Assert.True(ok);
        string sql = db.ExecutedSql.Single();
        // 原文 635-640：9 张 Items 系表 + AuctionAttention + AuctionData，用 sLineBreak（CRLF）分隔。
        int iElement = sql.IndexOf("DELETE FROM ItemElementAdd", StringComparison.Ordinal);
        int iByte = sql.IndexOf("DELETE FROM ItemAddDataByte", StringComparison.Ordinal);
        int iText = sql.IndexOf("DELETE FROM ItemAddDataText", StringComparison.Ordinal);
        int iItems = sql.IndexOf("DELETE FROM Items", StringComparison.Ordinal);
        int iAttention = sql.IndexOf("DELETE FROM AuctionAttention", StringComparison.Ordinal);
        int iData = sql.IndexOf("DELETE FROM AuctionData", StringComparison.Ordinal);
        Assert.True(iElement >= 0 && iElement < iByte && iByte < iText && iText < iItems
            && iItems < iAttention && iAttention < iData);
        // sWhere（原文 632）用到 AUCTION_ITEM_TYPE = 5 与 ItemIndex = 0。
        Assert.Contains(" WHERE ParentID = 42 and ItemType = 5 and ItemIndex = 0;", sql);
        Assert.Contains("DELETE FROM AuctionAttention WHERE AuctionID = 42;", sql);
        Assert.Contains("DELETE FROM AuctionData WHERE AuctionID = 42;", sql);
        // sLineBreak 是 CRLF。
        Assert.Contains("\r\n", sql);
    }

    [Fact]
    public void DeleteAuctionItem_SegmentsAreJoinedBySWhereAndSLineBreak_WithASingleSemicolon()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.DeleteAuctionItem("Alice", 42);

        // SqliteAuctionDB.pas:632 sWhere = ' WHERE ParentID = %d and ItemType = 5 and ItemIndex = 0;'
        // （**自带尾分号**）；638-639 是 'DELETE FROM Items' + sWhere + sLineBreak + 'DELETE FROM AuctionAttention …'
        // —— 原文**没有**多余的分号，所以是**单**分号 + CRLF。（上一版测试误期望 `;;`，已按原文订正。）
        string sql = db.ExecutedSql.Single();
        Assert.Contains("DELETE FROM Items WHERE ParentID = 42 and ItemType = 5 and ItemIndex = 0;\r\n"
            + "DELETE FROM AuctionAttention", sql, StringComparison.Ordinal);
        Assert.DoesNotContain("0;;", sql, StringComparison.Ordinal);
        // 9 段删除逐个用同一 sWhere 拼接（原文 635-638 共 9 次 DELETE … + sWhere）。
        Assert.Equal(9, sql.Split(" WHERE ParentID = 42 and ItemType = 5 and ItemIndex = 0;", StringSplitOptions.None).Length - 1);
    }

    [Fact]
    public void DeleteAuctionItem_UsesBeginCommitSequence()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.DeleteAuctionItem("Alice", 42);

        // 原文 633 BeginTransaction，642-643 Execute + Commit。
        Assert.Equal(new[] { "begin", "commit" }, db.Transactions.ToArray());
    }

    [Fact]
    public void DeleteAuctionItem_ExecuteThrows_RollsBackAndReturnsFalse()
    {
        var (unit, db, _, log) = NewSqliteThrowing("Auction_UpdateAuctionItemSuccess",
            new InvalidOperationException("unused"), executeBoom: new InvalidOperationException("delete-boom"));

        bool ok = unit.DeleteAuctionItem("H", 1);

        // 原文 646-651：except → MainOutMessage + RollBack。
        Assert.False(ok);
        Assert.Contains("delete-boom", log.Messages);
        Assert.Equal(new[] { "begin", "rollback" }, db.Transactions.ToArray());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void DeleteAuctionItem_PassesAnyAuctionIdThrough(int auctionId)
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.DeleteAuctionItem("H", auctionId);

        Assert.Contains(" WHERE ParentID = " + auctionId + " and ItemType = 5", db.ExecutedSql.Single());
        Assert.Contains("DELETE FROM AuctionData WHERE AuctionID = " + auctionId + ";", db.ExecutedSql.Single());
    }

    [Fact]
    public void DeleteAuctionItem_HumanNameParameterIsUnusedInTheScript()
    {
        var (unit, db, _, _, _) = NewSqlite();

        // 原文 626 的 HumanName 形参在 632-640 里**完全没用到**（只有 AuctionID）——锁死该冗余。
        unit.DeleteAuctionItem("ZZZ", 7);

        Assert.DoesNotContain("ZZZ", db.ExecutedSql.Single());
    }

    [Fact]
    public void DeleteAuctionItem_SqlInjectionInHumanName_HasNoEffect()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.DeleteAuctionItem(Injection, 7);

        Assert.DoesNotContain(Injection, db.ExecutedSql.Single());
    }

    // ==================================================================
    // 16. AddAttentionItem —— 原文 655-687
    // ==================================================================

    [Fact]
    public void AddAttentionItem_NotYetAttentioned_Inserts()
    {
        var (unit, db, _, _, _) = NewSqlite();
        // 原文 662-665：先查存在性（无行 → IsAttentioned = False）→ 再插入。
        bool ok = unit.AddAttentionItem("Alice", 9);

        Assert.True(ok);
        Assert.Equal(1, db.For("Auction_CheckInAttentionItem").StepCount);
        var insert = db.For("Auction_AddAttentionItem");
        Assert.Equal(1, insert.StepCount);
        Assert.Equal(new[] { "text", "int" }, insert.LastBinds.Select(b => b.Kind).ToArray());
        Assert.Equal("Alice", insert.LastBinds[0].Text);
        Assert.Equal(9, Convert.ToInt32(insert.LastBinds[1].Value));
    }

    [Fact]
    public void AddAttentionItem_AlreadyAttentioned_SkipsTheInsert()
    {
        var (unit, db, _, _, _) = NewSqlite();
        // 原文 665：IsAttentioned := Step = SQLITE_ROW（有行 → 已关注）。
        db.For("Auction_CheckInAttentionItem").AddRow(1);

        bool ok = unit.AddAttentionItem("Alice", 9);

        // 原文 670-680：if not IsAttentioned then ... —— 已关注时 Result 保持初始 False（原文如此）。
        Assert.False(ok);
        Assert.Equal(0, db.For("Auction_AddAttentionItem").StepCount);
    }

    [Fact]
    public void AddAttentionItem_BindsHumanNameThenIndexInTheCheckStatement()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.AddAttentionItem("Alice", 9);

        // 原文 663-664：CheckInAttentionItem 绑 (HumanName, Index)。
        var chk = db.For("Auction_CheckInAttentionItem").LastBinds;
        Assert.Equal(new[] { "text", "int" }, chk.Select(b => b.Kind).ToArray());
        Assert.Equal("Alice", chk[0].Text);
        Assert.Equal(9, Convert.ToInt32(chk[1].Value));
    }

    [Fact]
    public void AddAttentionItem_InsertStepReturnsRow_IsNotCountedAsSuccess()
    {
        var (unit, db, _, _, _) = NewSqlite();
        // 原文 676：Result := Step in [SQLITE_OK, SQLITE_DONE] —— ROW(100) 不算成功。
        // 要让 Step() 返回 ROW，必须有结果集行（无行时 Step 直接给 DONE）。
        db.For("Auction_AddAttentionItem").AddRow(1);

        Assert.False(unit.AddAttentionItem("H", 1));
    }

    [Fact]
    public void AddAttentionItem_SqlInjectionInHumanName_IsBound()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.AddAttentionItem(Injection, 1);

        Assert.Equal(Injection, db.For("Auction_CheckInAttentionItem").LastBinds[0].Text);
        Assert.Equal(Injection, db.For("Auction_AddAttentionItem").LastBinds[0].Text);
    }

    [Fact]
    public void AddAttentionItem_CheckStepThrows_LogsAndReturnsFalse()
    {
        var (unit, _, _, log) = NewSqliteThrowing("Auction_CheckInAttentionItem",
            new InvalidOperationException("att-add-boom"));

        Assert.False(unit.AddAttentionItem("H", 1));
        Assert.Contains("att-add-boom", log.Messages);
    }

    [Fact]
    public void AddAttentionItem_InsertStepThrows_LogsAndReturnsFalse()
    {
        var (unit, db, _, log) = NewSqliteThrowing("Auction_AddAttentionItem",
            new InvalidOperationException("att-insert-boom"));

        Assert.False(unit.AddAttentionItem("H", 1));
        Assert.Contains("att-insert-boom", log.Messages);
        // 存在性检查仍然跑过。
        Assert.Equal(1, db.For("Auction_CheckInAttentionItem").StepCount);
    }

    [Fact]
    public void AddAttentionItem_ResetsBothStatements()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.AddAttentionItem("H", 1);

        // 原文 662 + 667 finally / 673 + 678 finally。
        Assert.Equal(2, db.For("Auction_CheckInAttentionItem").ResetCount);
        Assert.Equal(2, db.For("Auction_AddAttentionItem").ResetCount);
    }

    // ==================================================================
    // 17. DeleteAttentionItem —— 原文 689-721
    // ==================================================================

    [Fact]
    public void DeleteAttentionItem_Attentioned_Deletes()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_CheckInAttentionItem").AddRow(1);   // 已关注

        bool ok = unit.DeleteAttentionItem("Alice", 9);

        Assert.True(ok);
        var del = db.For("Auction_DeleteAuctionItem");
        Assert.Equal(1, del.StepCount);
        Assert.Equal(new[] { "text", "int" }, del.LastBinds.Select(b => b.Kind).ToArray());
        Assert.Equal("Alice", del.LastBinds[0].Text);
        Assert.Equal(9, Convert.ToInt32(del.LastBinds[1].Value));
    }

    [Fact]
    public void DeleteAttentionItem_NotAttentioned_SkipsTheDelete()
    {
        var (unit, db, _, _, _) = NewSqlite();
        // 无预设行 → IsAttentioned = False → 原文 704：if IsAttentioned then ...
        bool ok = unit.DeleteAttentionItem("Alice", 9);

        Assert.False(ok);
        Assert.Equal(0, db.For("Auction_DeleteAuctionItem").StepCount);
    }

    [Fact]
    public void DeleteAttentionItem_UsesTheAttentionDeleteStatementNotTheAuctionItemOne()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_CheckInAttentionItem").AddRow(1);

        unit.DeleteAttentionItem("H", 1);

        // 原文 289：label 是 'Auction_DeleteAuctionItem'（名字像删拍卖品，实际删 AuctionAttention）——
        // 这是原文的命名陷阱，用语句 SQL 文本确认它删的是 AuctionAttention。
        Assert.Contains("delete from AuctionAttention", db.For("Auction_DeleteAuctionItem").Sql);
    }

    [Fact]
    public void DeleteAttentionItem_SqlInjectionInHumanName_IsBound()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_CheckInAttentionItem").AddRow(1);

        unit.DeleteAttentionItem(Injection, 1);

        Assert.Equal(Injection, db.For("Auction_DeleteAuctionItem").LastBinds[0].Text);
    }

    [Fact]
    public void DeleteAttentionItem_CheckStepThrows_LogsAndReturnsFalse()
    {
        var (unit, _, _, log) = NewSqliteThrowing("Auction_CheckInAttentionItem",
            new InvalidOperationException("att-del-boom"));

        Assert.False(unit.DeleteAttentionItem("H", 1));
        Assert.Contains("att-del-boom", log.Messages);
    }

    [Fact]
    public void DeleteAttentionItem_DeleteStepReturnsRow_IsNotCountedAsSuccess()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_CheckInAttentionItem").AddRow(1);
        // 原文 710：Result := Step in [SQLITE_OK, SQLITE_DONE]；给一行结果集让 Step 返回 ROW(100)。
        db.For("Auction_DeleteAuctionItem").AddRow(1);

        Assert.False(unit.DeleteAttentionItem("H", 1));
    }

    // ==================================================================
    // 18. JoinItemBid —— 原文 723-759（★ 调基类 AddAttentionItem → 带锁）
    // ==================================================================

    [Fact]
    public void JoinItemBid_IsSell_UsesTheBuyItemStatement()
    {
        var (unit, db, _, _, _) = NewSqlite();

        bool ok = unit.JoinItemBid("Alice", 7, 500, isSell: true);

        Assert.True(ok);
        var buy = db.For("Auction_BuyItem");
        Assert.Equal(1, buy.StepCount);
        Assert.Equal(0, db.For("Auction_JoinItemBid").StepCount);
        // 原文 733-735：LastBidder, LastBidPrice, AuctionID。
        Assert.Equal(new[] { "text", "int", "int" }, buy.LastBinds.Select(b => b.Kind).ToArray());
        Assert.Equal("Alice", buy.LastBinds[0].Text);
        Assert.Equal(500, Convert.ToInt32(buy.LastBinds[1].Value));
        Assert.Equal(7, Convert.ToInt32(buy.LastBinds[2].Value));
    }

    [Fact]
    public void JoinItemBid_NotSell_UsesTheJoinItemBidStatement()
    {
        var (unit, db, _, _, _) = NewSqlite();

        bool ok = unit.JoinItemBid("Alice", 7, 120, isSell: false);

        Assert.True(ok);
        var bid = db.For("Auction_JoinItemBid");
        Assert.Equal(1, bid.StepCount);
        Assert.Equal(0, db.For("Auction_BuyItem").StepCount);
        Assert.Equal("Alice", bid.LastBinds[0].Text);
        Assert.Equal(120, Convert.ToInt32(bid.LastBinds[1].Value));
        Assert.Equal(7, Convert.ToInt32(bid.LastBinds[2].Value));
    }

    [Fact]
    public void JoinItemBid_AlwaysAddsAttentionFirst()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.JoinItemBid("Alice", 7, 500, isSell: false);

        // 原文 727：AddAttentionItem(HumanName, Index) 在两种分支之前无条件调用。
        Assert.Equal(1, db.For("Auction_CheckInAttentionItem").StepCount);
        Assert.Equal(1, db.For("Auction_AddAttentionItem").StepCount);
    }

    [Fact]
    public void JoinItemBid_LocksTwice_BecauseItCallsThePublicAddAttentionItemWrapper()
    {
        var (unit, _, host, _, _) = NewSqlite();

        unit.JoinItemBid("Alice", 7, 500, isSell: false);

        // ★ 原文 727 调的是**基类 public** AddAttentionItem（DbBases.cs:932-943 的包装层带 Lock），
        // 而 JoinItemBid 自己的包装层（DbBases.cs:958-969）也带 Lock → 同一次调用 Lock 两次（嵌套加锁）。
        // 对比：直接调 AddAttentionItem 只 Lock 一次（见下一个测试）。
        Assert.Equal(2, host.LockCount);
        Assert.Equal(2, host.UnLockCount);
    }

    [Fact]
    public void AddAttentionItem_DirectCall_LocksOnlyOnce_UnlikeJoinItemBid()
    {
        var (unit, _, host, _, _) = NewSqlite();

        unit.AddAttentionItem("Alice", 7);

        // 与 JoinItemBid 的差异对照：DoAddAttentionItem 内部不再调带锁的 public 包装层。
        Assert.Equal(1, host.LockCount);
        Assert.Equal(1, host.UnLockCount);
    }

    [Fact]
    public void JoinItemBid_StepReturnsRow_IsNotSuccess()
    {
        var (unit, db, _, _, _) = NewSqlite();
        // 原文 736/748：Result := Step in [SQLITE_OK, SQLITE_DONE]；给一行结果集让 Step 返回 ROW(100)。
        db.For("Auction_JoinItemBid").AddRow(1);

        Assert.False(unit.JoinItemBid("H", 1, 1, isSell: false));
    }

    [Fact]
    public void JoinItemBid_NegativeAndZeroPrices_ArePassedThrough()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.JoinItemBid("H", -1, -5, isSell: false);

        // 原文 745-747：Index 与 Prices 都不做校验。
        Assert.Equal(-5, Convert.ToInt32(db.For("Auction_JoinItemBid").LastBinds[1].Value));
        Assert.Equal(-1, Convert.ToInt32(db.For("Auction_JoinItemBid").LastBinds[2].Value));
    }

    [Fact]
    public void JoinItemBid_SqlInjectionInHumanName_IsBound()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.JoinItemBid(Injection, 1, 1, isSell: true);

        Assert.Equal(Injection, db.For("Auction_BuyItem").LastBinds[0].Text);
    }

    [Fact]
    public void JoinItemBid_BidStepThrows_LogsAndReturnsFalse()
    {
        var (unit, _, _, log) = NewSqliteThrowing("Auction_JoinItemBid", new InvalidOperationException("bid-boom"));

        Assert.False(unit.JoinItemBid("H", 1, 1, isSell: false));
        Assert.Contains("bid-boom", log.Messages);
    }

    [Fact]
    public void JoinItemBid_ResetsTheChosenStatementInFinally()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.JoinItemBid("H", 1, 1, isSell: true);

        Assert.Equal(2, db.For("Auction_BuyItem").ResetCount);   // 732 + 738 finally
    }

    // ==================================================================
    // 19. GetAuctionInfo —— 原文 1019-1052
    // ==================================================================

    [Fact]
    public void GetAuctionInfo_NoRow_ReturnsFalse()
    {
        var (unit, _, _, _, _) = NewSqlite();

        bool ok = unit.GetAuctionInfo(5, out TAuctionInfo info);

        Assert.False(ok);
        Assert.NotNull(info);
        Assert.Equal(0, info.AuctionID);
    }

    [Fact]
    public void GetAuctionInfo_Row_ReturnsTrueAndFillsAllFields()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_QueryOneItem").AddRow(
            OneItemRow("Seller", 1700000000, 24, 3600, 100, 500, 2, 150, "Bidder", 1, 0));

        bool ok = unit.GetAuctionInfo(77, out TAuctionInfo info);

        Assert.True(ok);
        // 原文 1028：AuctionInfo.AuctionID := Index（**不是**读列）。
        Assert.Equal(77, info.AuctionID);
        Assert.Equal("Seller", info.HumanName);
        Assert.Equal(24, info.AuctionTime);
        Assert.Equal(3600, info.TimeLeft);
        Assert.Equal(100u, info.StartingPrice);
        Assert.Equal(500u, info.SellingPrice);
        Assert.Equal(2, info.CurrencyType);
        Assert.Equal(150, info.LastBidPrice);
        Assert.Equal("Bidder", info.LastBidder);
        Assert.Equal(1, info.TradingStatus);
        Assert.False(info.IsItemGive);
    }

    [Fact]
    public void GetAuctionInfo_AuctionIdComesFromTheParameterNotTheFirstColumn()
    {
        var (unit, db, _, _, _) = NewSqlite();
        // Auction_QueryOneItem 的 select 列表**没有** AuctionID 列（原文 242-245 共 11 列，首列是 HumanName）。
        // 故意让首列值是 "999" 这个"看起来像 ID"的串：若实现误把首列当 AuctionID，info.AuctionID 会变成 0
        // （文本转 int 失败）或别的值，而不会等于入参 12345。原文 1028 用入参 Index，1063 同理。
        db.For("Auction_QueryOneItem").AddRow(
            OneItemRow("999", 1, 2, 3, 4, 5, 6, 7, "B", 0, 0));

        unit.GetAuctionInfo(12345, out TAuctionInfo info);

        Assert.Equal(12345, info.AuctionID);
        Assert.Equal("999", info.HumanName);   // 首列就是 HumanName
    }

    [Fact]
    public void GetAuctionInfo_BindsIndexAndResetsInFinally()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.GetAuctionInfo(9, out _);

        var stmt = db.For("Auction_QueryOneItem");
        Assert.Single(stmt.LastBinds);
        Assert.Equal(9, Convert.ToInt32(stmt.LastBinds[0].Value));
        Assert.Equal(2, stmt.ResetCount);   // 1024 + 1044 finally
    }

    [Fact]
    public void GetAuctionInfo_StepThrows_LogsAndReturnsFalse()
    {
        var (unit, _, _, log) = NewSqliteThrowing("Auction_QueryOneItem", new InvalidOperationException("info-boom"));

        bool ok = unit.GetAuctionInfo(1, out _);

        Assert.False(ok);
        Assert.Contains("info-boom", log.Messages);
    }

    [Fact]
    public void GetAuctionInfo_NegativeIndex_IsBoundAsIs()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.GetAuctionInfo(-1, out _);

        Assert.Equal(-1, Convert.ToInt32(db.For("Auction_QueryOneItem").LastBinds[0].Value));
    }

    // ==================================================================
    // 20. GetAuctionRecord —— 原文 1054-1090
    // ==================================================================

    [Fact]
    public void GetAuctionRecord_NoRow_ReturnsFalse()
    {
        var (unit, _, _, _, _) = NewSqlite();

        Assert.False(unit.GetAuctionRecord(5, out TAuctionRecord rec));
        Assert.NotNull(rec);
    }

    [Fact]
    public void GetAuctionRecord_Row_ReturnsTrueAndLoadsTheItem()
    {
        var (unit, db, host, _, _) = NewSqlite();
        db.For("Auction_QueryOneItem").AddRow(
            OneItemRow("Seller", 1700000000, 48, -10, 11, 22, 3, 33, "Bidder", 2, 1));

        bool ok = unit.GetAuctionRecord(88, out TAuctionRecord rec);

        Assert.True(ok);
        Assert.Equal(88, rec.AuctionID);
        Assert.Equal("Seller", rec.HumanName);
        Assert.Equal(11u, rec.StartingPrice);
        Assert.Equal(22u, rec.SellingPrice);
        Assert.Equal(3, rec.CurrencyType);
        Assert.Equal(33, rec.LastBidPrice);
        Assert.Equal("Bidder", rec.LastBidder);
        Assert.Equal(2, rec.TradingStatus);
        Assert.True(rec.IsItemGive);
        // 原文 1075：AuctionRecord.IsAttention := False（该语句没有关注列）。
        Assert.False(rec.IsAttention);
        // 原文 1077：LoadItemFromDB(@ActionItem, AuctionID, AUCTION_ITEM_TYPE, 0)。
        Assert.Equal(new[] { "88/5/0" }, host.LoadedItems.ToArray());
    }

    [Fact]
    public void GetAuctionRecord_NoRow_DoesNotLoadAnyItem()
    {
        var (unit, _, host, _, _) = NewSqlite();

        unit.GetAuctionRecord(5, out _);

        // 原文 1077 在 if Step = SQLITE_ROW 块内 → 无行则不 LoadItemFromDB。
        Assert.Empty(host.LoadedItems);
    }

    [Fact]
    public void GetAuctionRecord_BindsIndexAndResetsInFinally()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.GetAuctionRecord(13, out _);

        var stmt = db.For("Auction_QueryOneItem");
        Assert.Single(stmt.LastBinds);
        Assert.Equal(13, Convert.ToInt32(stmt.LastBinds[0].Value));
        Assert.Equal(2, stmt.ResetCount);   // 1059 + 1082 finally
    }

    [Fact]
    public void GetAuctionRecord_StepThrows_LogsAndReturnsFalse()
    {
        var (unit, _, _, log) = NewSqliteThrowing("Auction_QueryOneItem", new InvalidOperationException("record-boom"));

        Assert.False(unit.GetAuctionRecord(1, out _));
        Assert.Contains("record-boom", log.Messages);
    }

    [Fact]
    public void GetAuctionRecord_IsAttentionIsAlwaysFalse()
    {
        var (unit, db, _, _, _) = NewSqlite();
        // 即使第 12 列为 1（IsItemGive），IsAttention 仍是无条件 False（原文 1075）。
        db.For("Auction_QueryOneItem").AddRow(
            OneItemRow("S", 1, 2, 3, 4, 5, 6, 7, "B", 8, 1));

        unit.GetAuctionRecord(1, out TAuctionRecord rec);

        Assert.False(rec.IsAttention);
    }

    [Fact]
    public void GetAuctionRecord_AuctionIdComesFromTheParameter()
    {
        var (unit, db, _, _, _) = NewSqlite();
        // 首列是 HumanName（原文 242），所以列值 999 不会被当成 AuctionID。
        db.For("Auction_QueryOneItem").AddRow(
            OneItemRow("999", 1, 2, 3, 4, 5, 6, 7, "B", 0, 0));

        unit.GetAuctionRecord(4242, out TAuctionRecord rec);

        // 原文 1063：AuctionRecord.AuctionID := Index。
        Assert.Equal(4242, rec.AuctionID);
    }

    // ==================================================================
    // 21. HumanRename —— 原文 1564-1601（★ 3 个 Reset 在 try 之外）
    // ==================================================================

    [Fact]
    public void HumanRename_UpdatesAllThreeTablesInOneTransaction()
    {
        var (unit, db, _, _, _) = NewSqlite();

        bool ok = unit.HumanRename("Old", "New");

        Assert.True(ok);
        // 原文 1572 BeginTransaction / 1586 Commit。
        Assert.Equal(new[] { "begin", "commit" }, db.Transactions.ToArray());
        // 三条语句各 Step 一次（不判返回值）。
        Assert.Equal(1, db.For("Auction_HumanRename").StepCount);
        Assert.Equal(1, db.For("Auction_LastBidderRename").StepCount);
        Assert.Equal(1, db.For("Auction_AttentionRename").StepCount);
    }

    [Fact]
    public void HumanRename_BindsNewNameThenOldNameForEachStatement()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.HumanRename("Old", "New");

        // 原文 1574-1584：每条都是 OrderBindText(NewName) 然后 OrderBindText(OldName)。
        foreach (string label in new[] { "Auction_HumanRename", "Auction_LastBidderRename", "Auction_AttentionRename" })
        {
            var b = db.For(label).LastBinds;
            Assert.Equal(new[] { "text", "text" }, b.Select(x => x.Kind).ToArray());
            Assert.Equal("New", b[0].Text);
            Assert.Equal("Old", b[1].Text);
        }
    }

    [Fact]
    public void HumanRename_ResetsEachStatementTwice()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.HumanRename("Old", "New");

        // ★ 原文缺陷（1567-1569）：3 个 Reset 在 try **之外**；加上 1597-1599 的 finally 3 次 → 每条 2 次。
        Assert.Equal(2, db.For("Auction_HumanRename").ResetCount);
        Assert.Equal(2, db.For("Auction_LastBidderRename").ResetCount);
        Assert.Equal(2, db.For("Auction_AttentionRename").ResetCount);
    }

    [Fact]
    public void HumanRename_StepThrows_RollsBackAndReturnsFalse_ButStillResets()
    {
        var (unit, db, _, log) = NewSqliteThrowing("Auction_LastBidderRename",
            new InvalidOperationException("rename-boom"));

        bool ok = unit.HumanRename("Old", "New");

        // 原文 1589-1594：except → MainOutMessage + RollBack；Result 保持 False。
        Assert.False(ok);
        Assert.Contains("rename-boom", log.Messages);
        Assert.Equal(new[] { "begin", "rollback" }, db.Transactions.ToArray());
        // 原文 1596-1600 的 finally 仍然把 3 条 Reset（前置 + finally = 2）。
        Assert.Equal(2, db.For("Auction_HumanRename").ResetCount);
        Assert.Equal(2, db.For("Auction_AttentionRename").ResetCount);
    }

    [Fact]
    public void HumanRename_DoesNotCheckStepResults_MirroringTheOriginal()
    {
        var (unit, db, _, _, _) = NewSqlite();
        // 原文 1576/1580/1584 只调 Step，**不**判返回值 → 即使没有结果集（Step 返回 DONE）也照样 Commit + True。
        bool ok = unit.HumanRename("Old", "New");

        Assert.True(ok);
        Assert.Equal(new[] { "begin", "commit" }, db.Transactions.ToArray());
    }

    [Fact]
    public void HumanRename_SqlInjectionInNames_AreBound()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.HumanRename(Injection, Injection);

        var b = db.For("Auction_HumanRename").LastBinds;
        Assert.Equal(Injection, b[0].Text);
        Assert.Equal(Injection, b[1].Text);
        Assert.DoesNotContain(Injection, db.For("Auction_HumanRename").Sql);
    }

    [Fact]
    public void HumanRename_EmptyNames_AreBoundAsEmptyText()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.HumanRename("", "");

        var b = db.For("Auction_HumanRename").LastBinds;
        Assert.Equal("", b[0].Text);
        Assert.Equal("", b[1].Text);
    }

    // ==================================================================
    // 22. Run —— 原文 1290-1562（★ 1543-1544 空 except）
    // ==================================================================

    [Fact]
    public void Run_UpdatesExpiredAuctionsFirst_ThenQueriesTheSuccesses()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.Run();

        // 原文 1423-1427：先 Reset + Step 更新过期状态（不判返回值，只有 finally Reset）。
        var fail = db.For("Auction_UpdateAuctionItemFail");
        Assert.Equal(1, fail.StepCount);
        Assert.Equal(2, fail.ResetCount);      // 1423 + 1426 finally
        // 原文 1430-1431：再查成功列表（首次 Step 无行）。
        Assert.Equal(1, db.For("Auction_QueryAuctionItemSuccess").StepCount);
    }

    [Fact]
    public void Run_NoSuccessfulRows_DoesNothingElse()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.Run();

        // 无行 → while 不进入 → 不更新状态、不开事务。
        Assert.Equal(0, db.For("Auction_UpdateAuctionItemSuccess").StepCount);
        Assert.Empty(db.Transactions);
    }

    [Fact]
    public void Run_OneSuccessfulRow_PaysTheSellerAndMarksItTradingStatusTwo()
    {
        var (unit, db, _, env, _) = NewSqlite();
        env.dwAuctionGameGoldTaxRate = 10;    // 元宝税率 10%
        db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(5, "Seller", 0, 100, 500, "Bidder", 1000, 12, 34));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "Sword", DBName = "SwordDb" };
        var player = new FakeAuctionPlayer { m_nGameGold = 0 };
        AuctionDbRunSeam.GetPlayObject = _ => player;

        try
        {
            unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        // 原文 1467：IncPlayerGameMoney(HumanName, MakeIndex, CurrencyType,
        //             LastBidPrice - Round(LastBidPrice / 100 * nAuctionTaxRate), ...)
        //           = 1000 - Round(1000/100*10) = 1000 - 100 = 900
        Assert.Equal(900, player.m_nGameGold);
        Assert.Equal(1, player.GameGoldChangedCount);
        // 原文 1546-1548：UpdateAuctionItemSuccess 绑 AuctionID（TradingStatus = 2 在 SQL 里）。
        var upd = db.For("Auction_UpdateAuctionItemSuccess");
        Assert.Equal(1, upd.StepCount);
        Assert.Equal(5, Convert.ToInt32(upd.LastBinds[0].Value));
    }

    [Fact]
    public void Run_RoundUsesBankersRounding_LikeDelphiRound()
    {
        var (unit, db, _, env, _) = NewSqlite();
        env.dwAuctionGameGoldTaxRate = 5;    // 5% → 250 * 0.05 = 12.5
        db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "S", 0, 1, 2, "B", 250, 1, 1));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "S" };
        var player = new FakeAuctionPlayer { m_nGameGold = 0 };
        AuctionDbRunSeam.GetPlayObject = _ => player;

        try
        {
            unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        // 原文 1467 用 Delphi Round（银行家舍入 ToEven）：12.5 → 12，故得 250 - 12 = 238。
        // （若用 MidpointRounding.AwayFromZero 会得 250 - 13 = 237。）
        Assert.Equal(238, player.m_nGameGold);
    }

    [Fact]
    public void Run_GoldCurrency_ClampsToHumanMaxGold()
    {
        var (unit, db, _, env, _) = NewSqlite();
        env.nHumanMaxGold = 500;              // 金币上限
        db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "Seller", 2, 100, 500, "Bidder", 1000, 1, 1));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "S" };
        var player = new FakeAuctionPlayer { m_nGold = 0 };
        AuctionDbRunSeam.GetPlayObject = _ => player;

        try
        {
            unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        // 原文 1322-1325：nValue := Int64(m_nGold) + Prices;
        //                 if nValue > g_Config.nHumanMaxGold then nValue := nHumanMaxGold
        // （税率 0 → Prices = 1000 > 500 → 截到 500），然后 GoldChanged()。
        Assert.Equal(500, player.m_nGold);
        Assert.Equal(1, player.GoldChangedCount);
        Assert.Equal(0, player.GameGoldChangedCount);
    }

    [Fact]
    public void Run_OfflinePlayer_GoesThroughHumanChangeGoldSeam()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "Seller", 0, 100, 500, "Bidder", 100, 1, 1));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "S" };
        AuctionDbRunSeam.GetPlayObject = _ => null;      // 玩家不在线
        TDBChangeGoldType captured = default;
        string capturedName = "";
        int capturedPrices = -1;
        AuctionDbRunSeam.HumanChangeGold = (t, n, p) => { captured = t; capturedName = n; capturedPrices = p; };

        try
        {
            unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        // 原文 1352-1355 + 1366：CurrencyType=0 → cgtGameGold；
        // DataEngine.HumanChangeGold(nil, nil, GoldType, PlayerName, PlayerName, Prices)。
        Assert.Equal(TDBChangeGoldType.cgtGameGold, captured);
        Assert.Equal("Seller", capturedName);
        Assert.Equal(100, capturedPrices);
    }

    [Theory]
    [InlineData(0, TDBChangeGoldType.cgtGameGold)]
    [InlineData(1, TDBChangeGoldType.cgtGamePoint)]
    [InlineData(2, TDBChangeGoldType.cgtGold)]
    [InlineData(3, TDBChangeGoldType.cgtGameDiamond)]
    [InlineData(4, TDBChangeGoldType.cgtGameGird)]
    public void Run_OfflinePlayer_MapsCurrencyTypeToGoldType(int currency, TDBChangeGoldType expected)
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "Seller", currency, 1, 2, "Bidder", 10, 1, 1));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "S" };
        AuctionDbRunSeam.GetPlayObject = _ => null;
        TDBChangeGoldType captured = default;
        AuctionDbRunSeam.HumanChangeGold = (t, _, _) => captured = t;

        try
        {
            unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        // 原文 1352-1361 的 case 映射。
        Assert.Equal(expected, captured);
    }

    [Fact]
    public void Run_UnknownCurrency_OfflineBranchExitsBeforeHumanChangeGold()
    {
        var (unit, db, _, _, _) = NewSqlite();
        // 原文 1362-1363：case 的 else Exit —— 非 0..4 的货币类型直接 Exit（离开 IncPlayerGameMoney）。
        db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "Seller", 99, 100, 500, "Bidder", 100, 1, 1));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "S" };
        AuctionDbRunSeam.GetPlayObject = _ => null;
        bool called = false;
        AuctionDbRunSeam.HumanChangeGold = (_, _, _) => called = true;

        try
        {
            unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        Assert.False(called);
        // Exit 只离开 IncPlayerGameMoney 这个内嵌过程，不离开 DoRun 的循环 →
        // 原文 1546 的 UpdateAuctionItemSuccess 仍然执行。
        Assert.Equal(1, db.For("Auction_UpdateAuctionItemSuccess").StepCount);
    }

    [Fact]
    public void Run_NullStdItem_StillPaysAndLogsWithEmptyName()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "Seller", 0, 100, 500, "Bidder", 100, 1, 1));
        AuctionDbRunSeam.GetStdItem = _ => null;      // 原文 1459-1464：StdItem = nil → ItemName := ''
        AuctionDbRunSeam.GetPlayObject = _ => null;
        string? logAdd = null;
        AuctionDbRunSeam.AddGameDataLog = args => { if (args.Remark == "拍卖行-到期") logAdd ??= args.LogAdd; };
        AuctionDbRunSeam.GBoGameLogGameGold = true;

        try
        {
            unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        // 原文 1467-1468：'卖出物品: ' + ItemName（ItemName 为空串）。
        Assert.Equal("卖出物品: ", logAdd);
    }

    [Fact]
    public void Run_NpcGotoLableSectionExceptionsAreSwallowedByTheEmptyExcept()
    {
        var (unit, db, _, _, log) = NewSqlite();
        db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "Seller", 0, 100, 500, "Bidder", 100, 1, 1));
        // ★ 原文缺陷（1543-1544）：`except end;`（空处理）。
        // 让 try 块内的 GotoLable 抛异常，验证异常被吞掉且不影响后续的 UpdateSuccess。
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "S", NeedIdentify = 1 };
        AuctionDbRunSeam.GetPlayObject = _ => new FakeAuctionPlayer();
        AuctionDbRunSeam.GotoLable = (_, _) => throw new InvalidOperationException("goto-boom");

        try
        {
            unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        // 异常被 except end; 吞掉：
        //   1) 不冒泡到基类包装层（日志里没有 TAuctionDB:Run 前缀）；
        //   2) 循环继续走到 UpdateAuctionItemSuccess。
        Assert.DoesNotContain(log.Messages, m => m.Contains("TAuctionDB:Run"));
        Assert.DoesNotContain("goto-boom", log.Messages);
        Assert.Equal(1, db.For("Auction_UpdateAuctionItemSuccess").StepCount);
    }

    [Fact]
    public void Run_OuterException_IsCaughtByTheOriginalExceptAndLogged()
    {
        var (unit, db, _, log) = NewSqliteThrowing("Auction_UpdateAuctionItemFail",
            new InvalidOperationException("fail-update-boom"));

        // 原文 1556-1560：外层 except on E: Exception do MainOutMessage(E.Message)。
        unit.Run();

        Assert.Contains("fail-update-boom", log.Messages);
        // 外层捕获后不再往下走（异常发生在第一个 try 里，被最外层 except 接住）。
        Assert.Equal(0, db.For("Auction_QueryAuctionItemSuccess").StepCount);
    }

    [Fact]
    public void Run_ResetsQueryAndUpdateStatementsInFinally()
    {
        var (unit, db, _, _, _) = NewSqlite();

        unit.Run();

        // 原文 1552-1555：finally Reset(QueryAuctionItemSuccess); Reset(UpdateAuctionItemSuccess)。
        Assert.Equal(2, db.For("Auction_QueryAuctionItemSuccess").ResetCount);
        Assert.Equal(1, db.For("Auction_UpdateAuctionItemSuccess").ResetCount);
    }

    [Fact]
    public void Run_TwoRows_ProcessesBoth()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "S1", 0, 1, 2, "B1", 10, 1, 1));
        db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(2, "S2", 0, 1, 2, "B2", 20, 2, 2));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "S" };
        AuctionDbRunSeam.GetPlayObject = _ => null;

        try
        {
            unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        Assert.Equal(2, db.For("Auction_UpdateAuctionItemSuccess").StepCount);
        var binds = db.For("Auction_UpdateAuctionItemSuccess").LastBinds;
        Assert.Equal(2, Convert.ToInt32(binds[0].Value));      // 第二行的 AuctionID
    }

    [Fact]
    public void Run_TickIsRecordedButRunItselfDoesNotThrottle()
    {
        var (unit, _, _, _, _) = NewSqlite();

        // DbBases.cs:1027-1038：Run 本身**不判** RunTick2，只把 _runTick 更新为当前 tick
        // （节流由调用方按原文 M2DataCommon.pas:1602-1613 的方式自己比 RunTick2）。
        Assert.Equal(0u, unit.RunTick2);      // Isolate() 把 GetTickCount 固定为 0
        unit.Run();
        Assert.Equal(0u, unit.RunTick2);      // 同一时钟 → 仍为 0
    }

    [Fact]
    public void Run_GotoLableSellThenBuyLabelForSellerAndBidder()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "Seller", 0, 10, 20, "Bidder", 30, 1, 1));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "S", NeedIdentify = 1 };
        var seller = new FakeAuctionPlayer();
        var bidder = new FakeAuctionPlayer();
        AuctionDbRunSeam.GetPlayObject = n => n == "Seller" ? seller : bidder;
        var labels = new List<string>();
        AuctionDbRunSeam.GotoLable = (_, label) => labels.Add(label);

        try
        {
            unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        // 原文 1498 @AuctionSellItem（拍卖者）/ 1531 @AuctionBuyItem（竞拍者）。
        Assert.Equal(new[] { "@AuctionSellItem", "@AuctionBuyItem" }, labels.ToArray());
        // 原文 1500-1508 / 1533-1541：GotoLable 之后把玩家上的拍卖临时字段全部清空。
        Assert.Equal("", seller.m_sAuctionItemName);
        Assert.Equal(0, seller.m_nAuctionItemFinaPrice);
        Assert.False(seller.m_boAuctionItemSelled);
        Assert.Equal("", bidder.m_sAuctionItemHumanName);
        Assert.Equal(0, bidder.m_nAuctionItemStartPrice);
        Assert.False(bidder.m_boAuctionItemSelled);
    }

    [Fact]
    public void Run_SellerFieldsArePopulatedBeforeGotoLable()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_QueryAuctionItemSuccess").AddRow(
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
            unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        // 原文 1481-1498：m_nScriptGotoCount := 0；m_sAuctionItemName := StdItem.Name；
        // 拍卖者=HumanName、出价者=LastBidder、底价=StartingPrice、一口价=SellingPrice、
        // 成交价=LastBidPrice、失效价=0、货币=CurrencyType、Selled=True。
        Assert.Equal("SwordName|Seller|Bidder|11|22|33|0|4|True|0", snapshot.Single());
    }

    [Fact]
    public void Run_NeedIdentifyNotOne_SkipsTheItemLogsButStillRunsTheNpcFlow()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "Seller", 0, 10, 20, "Bidder", 30, 1, 1));
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "S", NeedIdentify = 0 };
        var player = new FakeAuctionPlayer();
        AuctionDbRunSeam.GetPlayObject = _ => player;
        var logTypes = new List<int>();
        AuctionDbRunSeam.AddGameDataLog = args => logTypes.Add(args.LogType);

        try
        {
            unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        // 原文 1470：只有 NeedIdentify = 1 才写 LOG_ItemSell(10)/LOG_ItemBuy(11)。
        Assert.DoesNotContain(AuctionLogTypes.LOG_ItemSell, logTypes);
        Assert.DoesNotContain(AuctionLogTypes.LOG_ItemBuy, logTypes);
        // 但 NPC 流程照走（原文 1478 起与 NeedIdentify 无关）。
        Assert.Equal(0, player.m_nScriptGotoCount);
    }

    [Fact]
    public void Run_NeedIdentifyOne_WritesSellAndBuyItemLogs()
    {
        var (unit, db, _, _, _) = NewSqlite();
        db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "Seller", 0, 10, 20, "Bidder", 30, 1, 1));
        // 按订正后的接缝：StdItem.Name / NeedIdentify（M2DataDbSupport.cs:240/244）。
        AuctionDbRunSeam.GetStdItem = _ => new FakeAuctionStdItem { Name = "屠龙", NeedIdentify = 1 };
        AuctionDbRunSeam.GetPlayObject = _ => null;
        var args = new List<AuctionGameDataLogArgs>();
        AuctionDbRunSeam.AddGameDataLog = a => args.Add(a);

        try
        {
            unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        // 原文 1472-1475：LOG_ItemSell / LOG_ItemBuy，物品名用 StdItem.Name。
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

    [Fact]
    public void Run_PublicWrapperDoesNotLock()
    {
        var (unit, _, host, _, _) = NewSqlite();

        unit.Run();

        // DbBases.cs:1027-1038 的 Run 有 try/except 但**没有** Lock（原文 M2DataCommon.pas:1602-1613 同样不加锁）。
        Assert.Equal(0, host.LockCount);
        Assert.Equal(0, host.UnLockCount);
    }

    // ==================================================================
    // 23★ 补的 2 条边角用例（p3-m2-dbdata 车道追加；未改动上面任何既有用例）
    // ==================================================================

    /// <summary>SqliteAuctionDB.pas:1292-1346（DoRun 的内嵌过程 IncPlayerGameMoney）——
    /// <c>if nValue &gt; High(LongWord) then nValue := High(LongWord)</c> 的钳位分支（1305/1314/1332/1341）
    /// 与随后的 <c>Player.m_nGameXxx := nValue</c>（Int64 → Integer 截断，1307/1316/1334/1343）。
    /// <para>构造：税率取默认 0 → 原文 1467-1468 的 Prices = LastBidPrice - Round(...) = LastBidPrice；
    /// 让 LastBidPrice = int.MaxValue 且余额也是 int.MaxValue，得
    /// nValue = (long)int.MaxValue + int.MaxValue = 4294967294，**恰好比 High(LongWord)=4294967295 小 1**
    /// → 钳位分支不可达，直接截断成 unchecked((int)4294967294) = -2。
    /// （若钳位真的命中，结果会是 (int)4294967295 = -1；本用例正是用这个差值锁死"比较是 off-by-one 的死分支"。）</para>
    /// <para>元宝(0)/游戏点(1)/金刚石(3)/灵符(4) 四条分支各一行，金币(2) 分支另有上限语义
    /// （g_Config.nHumanMaxGold，1323-1324）不在此列。</para></summary>
    [Fact]
    public void Run_IncPlayerGameMoney_HighLongWordClampIsOffByOne_AndTheValueTruncates()
    {
        var (unit, db, _, _, _) = NewSqlite();
        // 原文 1441-1444 的行列序：AuctionID, HumanName, CurrencyType, StartingPrice, SellingPrice,
        //                          LastBidder, LastBidPrice, DBIndex, MakeIndex。
        db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(1, "S0", 0, 1, 2, "B0", int.MaxValue, 1, 1));   // 元宝
        db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(2, "S1", 1, 1, 2, "B1", int.MaxValue, 1, 1));   // 游戏点
        db.For("Auction_QueryAuctionItemSuccess").AddRow(
            SuccessRow(3, "S2", 3, 1, 2, "B2", int.MaxValue, 1, 1));   // 金刚石
        db.For("Auction_QueryAuctionItemSuccess").AddRow(
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
            unit.Run();
        }
        finally
        {
            AuctionDbRunSeam.ResetDefaults();
        }

        Assert.Equal(-2, s0.m_nGameGold);                 // 1304-1307
        Assert.Equal(1, s0.GameGoldChangedCount);         // 1309
        Assert.Equal(-2, s1.m_nGamePoint);                // 1313-1316
        Assert.Equal(1, s1.GameGoldChangedCount);         // 1318
        Assert.Equal(-2, s2.m_nGameDiamond);              // 1331-1334
        Assert.Equal(1, s2.NewGamePointChangedCount);     // 1336
        Assert.Equal(-2, s3.m_nGameGird);                 // 1340-1343
        Assert.Equal(1, s3.NewGamePointChangedCount);     // 1345
        // 四行都走完了到期结算的尾部（原文 1547 的 UpdateAuctionItemSuccess）。
        Assert.Equal(4, db.For("Auction_UpdateAuctionItemSuccess").StepCount);
    }

    /// <summary>SqliteAuctionDB.pas:964-1017（DoQueryMyAttentionItems）—— ★ 多行交错时
    /// <c>FStatementQueryOneItem.Reset</c>（982）在**每一行**都重新武装一次，随后才轮到外层
    /// <c>FStatementQueryMyAttentionItems.Step</c>（1003）取下一行；finally（1008/1009）再各 Reset 一次。
    /// <para>断言交错顺序的可观测证据：
    /// 外层 Reset 2（972 + 1009）、Step 3（976 + 1003×2）；
    /// 内层 Reset 3（982×2 + 1008）、Step 2（984×2）、Reads 22 条且列号严格是 0..10 重复两遍
    /// （证明内层被重新武装，而不是连续读完两行）；
    /// 内层 <c>LastBinds[0]</c> 是**第二个** AuctionID（41），证明它在第一行处理完之后被重新绑定；
    /// itemList 收到 2 条且两条都拿到内层结果（HumanName = "Only"）
    /// —— 若内层只 Reset 一次，第二条只会剩 AuctionID。</para></summary>
    [Fact]
    public void QueryMyAttentionItems_TwoRows_InterleavesTheInnerResetBeforeEachOuterStep()
    {
        var (unit, db, host, _, _) = NewSqlite();
        // 外层只取 AuctionID（原文 980）。
        db.For("Auction_QueryAttentionItems").AddRow(31);
        db.For("Auction_QueryAttentionItems").AddRow(41);
        // 内层只有一行；靠"每行 Reset 一次"才能被两行复用（原文 982）。
        db.For("Auction_QueryOneItem").AddRow(
            OneItemRow("Only", 1700000000, 6, 60, 7, 8, 3, 9, "Bidder", 0, 0));
        var list = new TAuctionItemList();

        int n = unit.QueryMyAttentionItems("Alice", 1, list);

        Assert.Equal(2, n);
        Assert.Equal(2, list.Count);
        Assert.Equal(new[] { 31, 41 }, list.Snapshot().Select(r => r.AuctionID).ToArray());
        // 两条都拿到了内层完整记录 → 内层在第二次进入循环前被 Reset 过。
        Assert.Equal(new[] { "Only", "Only" }, list.Snapshot().Select(r => r.HumanName).ToArray());
        Assert.Equal(7u, list[1]!.StartingPrice);
        Assert.Equal(new[] { "31/5/0", "41/5/0" }, host.LoadedItems.ToArray());

        var outer = db.For("Auction_QueryAttentionItems");
        var inner = db.For("Auction_QueryOneItem");
        // 外层：972 Reset + 1009 finally=Reset → 2；976 首次 Step + 1003 每行末 Step → 3。
        Assert.Equal(2, outer.ResetCount);
        Assert.Equal(3, outer.StepCount);
        // 内层：982 每行 Reset ×2 + 1008 finally → 3；984 每行 Step ×2。
        Assert.Equal(3, inner.ResetCount);
        Assert.Equal(2, inner.StepCount);
        // 内层被重新绑定到**第二行**的 AuctionID（交错顺序证据）。
        Assert.Equal("int", inner.LastBinds[0].Kind);
        Assert.Equal(41, Convert.ToInt32(inner.LastBinds[0].Value));
        // 内层每次 Step 后都从头读 11 列（0..10），重复两遍 → 两轮重新武装。
        Assert.Equal(22, inner.Reads.Count);
        Assert.Equal(Enumerable.Range(0, 11).Concat(Enumerable.Range(0, 11)).ToArray(),
            inner.Reads.Select(r => r.ColumnIndex).ToArray());
        // 外层每行只读第 0 列（AuctionID），共 2 次。
        Assert.Equal(2, outer.Reads.Count);
        Assert.All(outer.Reads, r => Assert.Equal(0, r.ColumnIndex));
    }
}
