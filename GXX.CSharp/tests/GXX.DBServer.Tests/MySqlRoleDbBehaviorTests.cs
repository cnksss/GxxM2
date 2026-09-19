using System;
using System.Collections.Generic;
using System.Linq;
using GXX.Core.Protocol;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>
/// MySqlRoleDB.pas 各方法的**行为**测试（全程走内存接缝，不连数据库）。
///
/// 约定：每个断言都对应原文某一行；"差异断言"专指 Sqlite 版 vs MySql 版的真实不同（见测试名 Diff_）。
/// </summary>
[Collection("MySqlRoleDb")]
public class MySqlRoleDbBehaviorTests
{
    private static (TMySqlRoleDB Db, FakeMySqlFactory F, FakeMySqlDatabase D) NewDb(bool init = true)
    {
        // 时钟/随机先定死，否则 DoInit 里的 GetTickCount 取真实值会让 Run 的 10 分钟判据提前命中
        DelphiTick.GetTickCount = () => 0;
        var f = new FakeMySqlFactory();
        var db = new TMySqlRoleDB(f);
        if (init) db.DoInit();
        return (db, f, f.Database);
    }

    // ==========================================================================================
    // DoInit（MySqlRoleDB.pas:256-644 / 3463-3806 / 6035-6155）
    // ==========================================================================================

    [Fact]
    public void DoInit_ConnectsWithClientMultiStatements_AndUtf8()
    {
        var (db, f, d) = NewDb();
        Assert.Equal(1, d.InitCount);
        Assert.Equal(1, f.CreateLibCount);
        Assert.Equal(1, f.CreateDatabaseCount);
        Assert.Equal("utf8", d.CharacterSet);
        Assert.Single(d.Connected);
        // 末参固定是 CLIENT_MULTI_STATEMENTS = 65536
        Assert.EndsWith("|65536", d.Connected[0]);
    }

    [Fact]
    public void DoInit_RegistersExactly140Statements_AndPreparesEachOnce()
    {
        var (db, f, d) = NewDb();
        Assert.Equal(140, d.Statements.Count);
        Assert.All(d.Statements, s => Assert.Equal(1, s.PrepareCount));
        Assert.All(d.Statements, s => Assert.False(string.IsNullOrEmpty(s.Sql)));
    }

    [Fact]
    public void DoInit_SetsEachStatementSqlFromTheExtractedConstants()
    {
        var (db, f, d) = NewDb();
        Assert.Equal(MySqlRoleDBStatements.Human.FStatementGetID, d.Stmt("HumanGetID").Sql);
        Assert.Equal(MySqlRoleDBStatements.Human.FStatementUpdateHuman, d.Stmt("HumanUpdate").Sql);
        Assert.Equal(MySqlRoleDBStatements.Hero.FStatementGetHero, d.Stmt("HeroSelect").Sql);
        Assert.Equal(MySqlRoleDBStatements.Hero.FStatementUpdateHero, d.Stmt("HeroUpdate").Sql);
    }

    [Fact]
    public void DoInit_ClearsStatements_AfterReadingVersion()
    {
        // 原文 6064 `FDB.Statements.Clear`（finally）→ 所以 get_db_constant_value 之后只剩 140 条业务语句
        var (db, f, d) = NewDb();
        Assert.Equal(1, d.ClearStatementsCount);
        Assert.False(d.HasStatement("get_db_constant_value"));
    }

    [Fact]
    public void DoInit_SecondCall_IsNoOp_BecauseFdbAlreadyAssigned()
    {
        // 原文 6041 `if not Assigned(FDB) then` 包住整段连接/升级，
        // 但 `inherited`（重建 HumanDB/HeroDB）在 if 之外 → 第二次调用会再准备 140 条语句。
        var (db, f, d) = NewDb();
        int statementsAfterFirst = d.Statements.Count;
        db.DoInit();
        Assert.Equal(1, d.InitCount);                       // 不再连接
        Assert.Equal(statementsAfterFirst, d.Statements.Count);  // 同名语句复用同一实例
        Assert.All(d.Statements, s => Assert.Equal(2, s.PrepareCount));   // 但 Prepare 又跑了一遍（原文如此）
    }

    [Fact]
    public void DoInit_ExposesHumanDbAndHeroDb()
    {
        var (db, f, d) = NewDb();
        Assert.NotNull(db.HumanDB);
        Assert.NotNull(db.HeroDB);
        Assert.True(db.IsOpen);
    }

    [Fact]
    public void DoInit_WithNativeSeamFactory_Throws_AndNamesTheSeam()
    {
        var db = new TMySqlRoleDB(RoleMySqlNativeSeamFactory.Instance);
        var ex = Assert.Throws<NotSupportedException>(() => db.DoInit());
        Assert.Contains("libmysql-32.dll", ex.Message);
    }

    // ==========================================================================================
    // 版本升级阶梯（MySqlRoleDB.pas:6067-6150 + 5848-6033）
    // ==========================================================================================

    [Theory]
    [InlineData(0, new int[0])]
    [InlineData(1, new int[0])]
    [InlineData(20180719, new[] { 1, 3, 4, 5, 6, 7, 8, 9, 10 })]
    [InlineData(20190314, new[] { 3, 4, 5, 6, 7, 8, 9, 10 })]
    [InlineData(20190318, new[] { 2, 3, 4, 5, 6, 7, 8, 9, 10 })]
    [InlineData(20190319, new[] { 4, 5, 6, 7, 8, 9, 10 })]
    [InlineData(20190512, new[] { 5, 6, 7, 8, 9, 10 })]
    [InlineData(20190606, new[] { 6, 7, 8, 9, 10 })]
    [InlineData(20190802, new[] { 7, 8, 9, 10 })]
    [InlineData(20190928, new[] { 8, 9, 10 })]
    [InlineData(20200813, new[] { 9, 10 })]
    [InlineData(20220105, new[] { 10 })]
    public void MigrationLadder_MatchesSourceIfElseChain(int dbVersion, int[] expected)
    {
        Assert.Equal(expected, MySqlRoleDbMigration.StepsFor(dbVersion));
    }

    [Fact]
    public void Migration_StepsRunInSourceOrder_WhenVersionIs20180719()
    {
        var f = new FakeMySqlFactory();
        // 先让版本查询返回 20180719，且让 Statements.Clear 之后语句仍可查
        f.Database.AddSQLStatement("get_db_constant_value");
        ((FakeMySqlStatement)f.Database.Stmt("get_db_constant_value")).EnqueueResult(
            FakeResultSet.Of(new object?[] { 20180719 }));

        var db = new TMySqlRoleDB(f);
        db.DoInit();

        // UpdateDB_1 4 条 ALTER + REPLACE；UpdateDB_3 是单条多语句 Exec（S 变量）
        Assert.Contains("ALTER TABLE HumanItemProperty ADD COLUMN `Value2` INTEGER DEFAULT 0;", f.Database.Executed);
        Assert.Contains("ALTER TABLE HeroItemProperty ADD COLUMN `Value3` INTEGER DEFAULT 0;", f.Database.Executed);
        Assert.Contains(f.Database.Executed, s => s.StartsWith("CREATE TABLE `HumanSkillPower`"));
        // UpdateDB_10 的建表语句
        Assert.Contains(f.Database.Executed, s => s.StartsWith("CREATE TABLE `HumanMoney`"));
        // 版本占位符必须已被替换
        Assert.DoesNotContain("__MYSQL_DBVERSION__", string.Join("|", f.Database.Executed));
        Assert.Contains(f.Database.Executed, s => s.Contains("role_version"));
    }

    [Fact]
    public void Migration_UnknownVersion_ExecutesNothing()
    {
        var f = new FakeMySqlFactory();
        f.Database.AddSQLStatement("get_db_constant_value");
        ((FakeMySqlStatement)f.Database.Stmt("get_db_constant_value")).EnqueueResult(
            FakeResultSet.Of(new object?[] { 12345 }));
        var db = new TMySqlRoleDB(f);
        db.DoInit();
        Assert.Empty(f.Database.Executed);
    }

    [Fact]
    public void Migration_Expand_ReplacesPlaceholderWithDbVersionConstant()
    {
        string expanded = MySqlRoleDbMigration.Expand("values (\"role_version\", __MYSQL_DBVERSION__);");
        Assert.Contains(GXX.Core.Data.MySqlCreateTableSql.MYSQL_DBVERSION, expanded);
        Assert.DoesNotContain("__MYSQL_DBVERSION__", expanded);
    }

    [Fact]
    public void Migration_EachStepWrapsItselfInATransaction()
    {
        var f = new FakeMySqlFactory();
        var db = new TMySqlRoleDB(f);
        db.DoInit();
        int before = f.Database.StartTransactionCount;
        int commitsBefore = f.Database.CommitCount;
        db.RunUpdateDB(1);
        Assert.Equal(before + 1, f.Database.StartTransactionCount);
        Assert.Equal(commitsBefore + 1, f.Database.CommitCount);
    }

    [Fact]
    public void Migration_StepRollsBack_WhenExecThrows()
    {
        var f = new FakeMySqlFactory();
        var db = new TMySqlRoleDB(f);
        db.DoInit();
        int rbBefore = f.Database.RollBackCount;
        f.Database.ThrowOnExec = new InvalidOperationException("boom");
        db.RunUpdateDB(4);   // 原文 except 分支只 RollBack，不重抛
        Assert.Equal(rbBefore + 1, f.Database.RollBackCount);
    }

    [Fact]
    public void Migration_UpdateDb2_DropsTheTwoSkillPowerTables()
    {
        var f = new FakeMySqlFactory();
        var db = new TMySqlRoleDB(f);
        db.DoInit();
        db.RunUpdateDB(2);
        Assert.Contains(f.Database.Executed, s => s.Contains("DROP TABLE IF EXISTS `HumanSkillPower`;"));
        Assert.Contains(f.Database.Executed, s => s.Contains("DROP TABLE IF EXISTS `HeroSkillPower`;"));
        // 原文 (* *) 注释掉的那段 create table 不得出现
        Assert.DoesNotContain(f.Database.Executed, s => s.Contains("`AttackValue`"));
    }

    // ==========================================================================================
    // DoFainal / Run / OnRequest（MySqlRoleDB.pas:6157-6202）
    // ==========================================================================================

    [Fact]
    public void DoFainal_ReleasesDbAndClearsHumanHero()
    {
        var (db, f, d) = NewDb();
        db.DoFainal();
        Assert.False(db.IsOpen);
        Assert.Null(db.HumanDB);
        Assert.Null(db.HeroDB);
    }

    [Fact]
    public void Run_DoesNothing_WhenNotEnoughTicksElapsed()
    {
        var (db, f, d) = NewDb();
        d.Executed.Clear();
        db.Run();
        Assert.Empty(d.Executed);
    }

    [Fact]
    public void Run_SendsKeepAlive_AfterTenMinutes()
    {
        var (db, f, d) = NewDb();
        uint now = 0;
        DelphiTick.GetTickCount = () => now;
        db.DoInit();                       // FLastRequestTick = now
        d.Executed.Clear();
        now = 10 * 60000 - 1;
        db.Run();
        Assert.Empty(d.Executed);          // 差 1ms 不发
        now = 10 * 60000;
        db.Run();
        Assert.Equal(new[] { "select 1" }, d.Executed);   // 原文判据是 >=
    }

    [Fact]
    public void Run_DoesNothing_WhenClientHandleIsNull()
    {
        var (db, f, d) = NewDb();
        d.FakeClient = null!;
        d.Executed.Clear();
        DelphiTick.GetTickCount = () => 10 * 60000;
        db.Run();
        Assert.Empty(d.Executed);
    }

    [Fact]
    public void OnRequest_RefreshesLastRequestTick()
    {
        var (db, f, d) = NewDb();
        uint now = 1000;
        DelphiTick.GetTickCount = () => now;
        d.RaiseOnRequest();
        Assert.Equal(1000u, db.LastRequestTick);
        now = 2000;
        d.RaiseOnRequest();
        Assert.Equal(2000u, db.LastRequestTick);
    }

    // ==========================================================================================
    // 公开包装的 except 语义（RoleDB.pas:533-1005）
    // ==========================================================================================

    [Fact]
    public void HumanDb_GetID_ReturnsNoId_WhenQueryThrows()
    {
        // RoleDB.pas:533-549：初值 NO_ID，异常只 MainOutMessage
        var (db, f, d) = NewDb();
        d.Stmt("HumanGetID").EnqueueResult(FakeResultSet.Throwing(new InvalidOperationException("db down")));
        var messages = new List<string>();
        RoleDbSeam.MainOutMessage = messages.Add;
        try
        {
            Assert.Equal(RoleDbConst.NO_ID, db.HumanDB!.GetID("nobody"));
            Assert.Equal(new[] { "db down" }, messages);
        }
        finally { RoleDbSeam.MainOutMessage = _ => { }; }
    }

    [Fact]
    public void HumanDb_Get_ReturnsFalse_WhenQueryThrows()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanSelect").EnqueueResult(FakeResultSet.Throwing(new InvalidOperationException("x")));
        var hum = new THumData();
        bool ok = db.HumanDB!.Get("acc", "chr", ref hum, out int id);
        Assert.False(ok);
        Assert.Equal(0, id);
    }

    [Fact]
    public void HeroDb_GetID_ReturnsNoId_WhenNoRow()
    {
        var (db, f, d) = NewDb();
        Assert.Equal(RoleDbConst.NO_ID, db.HeroDB!.GetID("nobody"));
    }

    // ==========================================================================================
    // 简单读取（MySqlRoleDB.pas:839-1180）
    // ==========================================================================================

    [Fact]
    public void HumanDb_GetID_ReturnsFirstColumn()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanGetID").EnqueueResult(FakeResultSet.Of(new object?[] { 4242 }));
        Assert.Equal(4242, db.HumanDB!.GetID("chr"));
        // 绑定顺序：仅 HumanName
        Assert.Equal(new[] { "chr" }, d.Stmt("HumanGetID").AllBoundTexts);
    }

    [Fact]
    public void HumanDb_GetID_AlwaysResetsTwice()
    {
        // 原文 try/finally 各一次 Reset
        var (db, f, d) = NewDb();
        var s = d.Stmt("HumanGetID");
        int before = s.ResetCount;
        db.HumanDB!.GetID("chr");
        Assert.Equal(before + 2, s.ResetCount);
    }

    [Fact]
    public void HumanDb_CheckHumanExists_TrueWhenRowReturned()
    {
        var (db, f, d) = NewDb();
        d.Stmt("CheckHumanExists").EnqueueResult(FakeResultSet.Of(new object?[] { 7 }));
        Assert.True(db.HumanDB!.CheckHumanExists("acc", "chr"));
        Assert.Equal(new[] { "acc", "chr" }, d.Stmt("CheckHumanExists").AllBoundTexts);
    }

    [Fact]
    public void HumanDb_CheckHumanExists_FalseWhenNoRow()
    {
        var (db, f, d) = NewDb();
        Assert.False(db.HumanDB!.CheckHumanExists("acc", "chr"));
    }

    [Fact]
    public void HumanDb_GetHumanCount_ReturnsZero_WhenNoRow()
    {
        var (db, f, d) = NewDb();
        Assert.Equal(0, db.HumanDB!.GetHumanCount("acc"));
    }

    [Fact]
    public void HumanDb_GetHumanCount_ReturnsCountColumn()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanGetCount").EnqueueResult(FakeResultSet.Of(new object?[] { 3 }));
        Assert.Equal(3, db.HumanDB!.GetHumanCount("acc"));
    }

    [Fact]
    public void HumanDb_GetOtherHumanName_ReturnsEmptyWhenNoRow()
    {
        var (db, f, d) = NewDb();
        Assert.Equal("", db.HumanDB!.GetOtherHumanName("acc", "chr"));
    }

    [Fact]
    public void HumanDb_GetHumanHeroName_BindsOnlyHumanName_Diff_FromSignature()
    {
        // 原文缺陷（逐字保留）：Account 形参完全没用到，只绑 HumanName。
        var (db, f, d) = NewDb();
        d.Stmt("HumanGetHumanHeroName").EnqueueResult(FakeResultSet.Of(new object?[] { "HeroA", "HeroB" }));
        bool ok = db.HumanDB!.GetHumanHeroName("account-is-ignored", "chr", out string hero, out string deputy);
        Assert.True(ok);
        Assert.Equal("HeroA", hero);
        Assert.Equal("HeroB", deputy);
        Assert.Equal(new[] { "chr" }, d.Stmt("HumanGetHumanHeroName").AllBoundTexts);
    }

    [Fact]
    public void HumanDb_GetHumanHeroName_EmptyResult_KeepsEmptyOuts()
    {
        var (db, f, d) = NewDb();
        bool ok = db.HumanDB!.GetHumanHeroName("a", "chr", out string hero, out string deputy);
        Assert.False(ok);
        Assert.Equal("", hero);
        Assert.Equal("", deputy);
    }

    [Fact]
    public void HumanDb_GetBaseInfo_ReadsFourColumns()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanGetBaseInfo").EnqueueResult(FakeResultSet.Of(new object?[] { 1, 2, 33, 20240101 }));
        Assert.True(db.HumanDB!.GetBaseInfo("chr", out int sex, out int job, out int level, out int lastLogin));
        Assert.Equal(1, sex);
        Assert.Equal(2, job);
        Assert.Equal(33, level);
        Assert.Equal(20240101, lastLogin);
    }

    [Fact]
    public void HumanDb_QueryHumans_BindsIsDeleteZero_AndClampsSexJob()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanQuery").EnqueueResult(FakeResultSet.Of(
            new object?[] { "A", true, 9, 7, 3, 10 },      // Sex 9 越界 → 0；Job 7 越界 → 0
            new object?[] { "B", false, 1, 2, 4, 20 }));
        var list = new TQueryHumanList();
        Assert.Equal(2, db.HumanDB!.QueryHumans("acc", list));
        Assert.Equal(new object?[] { "acc", 0 }, d.Stmt("HumanQuery").BindHistory.Select(b => b.Value).ToArray());
        Assert.Equal(0, list.Items(0)!.Sex);
        Assert.Equal(0, list.Items(0)!.Job);
        Assert.True(list.Items(0)!.IsSelect);   // 第 2 列 IsSelect 为 True
        Assert.Equal(1, list.Items(1)!.Sex);
        Assert.Equal(2, list.Items(1)!.Job);
    }

    [Fact]
    public void HumanDb_QueryDeleteHumans_BindsIsDeleteOne_AndDoesNotClamp()
    {
        // 差异断言：原文 DoQueryDeleteHumans 这一支**没有** Sex/Job 钳位
        var (db, f, d) = NewDb();
        d.Stmt("HumanQuery").EnqueueResult(FakeResultSet.Of(new object?[] { "A", false, 9, 7, 3, 10 }));
        var list = new TQueryHumanList();
        Assert.Equal(1, db.HumanDB!.QueryDeleteHumans("acc", list));
        Assert.Equal(new object?[] { "acc", 1 }, d.Stmt("HumanQuery").BindHistory.Select(b => b.Value).ToArray());
        Assert.Equal(9, list.Items(0)!.Sex);
        Assert.Equal(7, list.Items(0)!.Job);
    }

    [Fact]
    public void HumanDb_SearchByAccount_Complete_UsesPlainAccount()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanSearchByAccount1").EnqueueResult(FakeResultSet.Of(new object?[] { "acc", "chr", 0, 1, 2, 30 }));
        var list = new TSerarchRoleList();
        Assert.Equal(1, db.HumanDB!.SearchByAccount("acc", TSearchMatchType.smtComplete, list));
        Assert.Equal(new[] { "acc" }, d.Stmt("HumanSearchByAccount1").AllBoundTexts);
        Assert.False(list.Items(0)!.IsHero);
        Assert.Equal(30u, list.Items(0)!.Level);
    }

    [Fact]
    public void HumanDb_SearchByAccount_Fuzzy_WrapsWithPercent()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanSearchByAccount2").EnqueueResult(FakeResultSet.Of(new object?[] { "xaccx", "chr", 1, 0, 0, 1 }));
        var list = new TSerarchRoleList();
        Assert.Equal(1, db.HumanDB!.SearchByAccount("acc", TSearchMatchType.smtFuzzy, list));
        Assert.Equal(new[] { "%acc%" }, d.Stmt("HumanSearchByAccount2").AllBoundTexts);
    }

    [Fact]
    public void HumanDb_SearchByName_Fuzzy_WrapsWithPercent()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanSearchByName2").EnqueueResult(FakeResultSet.Of(new object?[] { "acc", "xchrx", 0, 0, 0, 1 }));
        var list = new TSerarchRoleList();
        Assert.Equal(1, db.HumanDB!.SearchByName("chr", TSearchMatchType.smtFuzzy, list));
        Assert.Equal(new[] { "%chr%" }, d.Stmt("HumanSearchByName2").AllBoundTexts);
    }

    [Fact]
    public void HumanDb_SearchByLevel_BindsMinLevelThenLimit()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanSearchByLevel").EnqueueResult(FakeResultSet.Of(new object?[] { "acc", "chr", 0, 1, 1, 40 }));
        var list = new TSerarchRoleList();
        Assert.Equal(1, db.HumanDB!.SearchByLevel(50, 20, list));
        Assert.Equal(new[] { 20, 50 }, d.Stmt("HumanSearchByLevel").AllBoundNumbers);
        Assert.Equal(40u, list.Items(0)!.Level);
    }

    [Fact]
    public void HumanDb_SearchByLevel_ResetsTheWrongStatement_InFinally()
    {
        // 原文缺陷（逐字保留）：finally 里 Reset 的是 FStatementSearchByNameMatchFuzzy
        var (db, f, d) = NewDb();
        var byLevel = d.Stmt("HumanSearchByLevel");
        var byNameFuzzy = d.Stmt("HumanSearchByName2");
        int byLevelBefore = byLevel.ResetCount;
        int byNameBefore = byNameFuzzy.ResetCount;
        db.HumanDB!.SearchByLevel(10, 1, new TSerarchRoleList());
        Assert.Equal(byLevelBefore + 1, byLevel.ResetCount);        // 进入时的 Reset
        Assert.Equal(byNameBefore + 1, byNameFuzzy.ResetCount);     // finally 里 Reset 的是它
    }

    [Fact]
    public void HumanDb_GetMobileNumbers_Unbound_UsesStatement1()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanGetMobileNumbers1").EnqueueResult(FakeResultSet.Of(
            new object?[] { "13800000000" }, new object?[] { "13900000000" }));
        var list = new List<string>();
        Assert.Equal(2, db.HumanDB!.GetMobileNumbers(false, list));
        Assert.Equal(new[] { "13800000000", "13900000000" }, list);
        Assert.Equal(0, d.Stmt("HumanGetMobileNumbers2").QueryCount);
    }

    [Fact]
    public void HumanDb_GetMobileNumbers_BoundOnly_UsesStatement2()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanGetMobileNumbers2").EnqueueResult(FakeResultSet.Of(new object?[] { "13800000000" }));
        var list = new List<string>();
        Assert.Equal(1, db.HumanDB!.GetMobileNumbers(true, list));
        Assert.Equal(0, d.Stmt("HumanGetMobileNumbers1").QueryCount);
    }

    [Fact]
    public void HumanDb_Select_SetsThenUnsets_AndCommits()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanSetSelect").EnqueueResult(FakeResultSet.Step(true));
        Assert.True(db.HumanDB!.Select("acc", "chr"));
        Assert.Equal(1, d.CommitCount);
        Assert.Equal(0, d.RollBackCount);
        Assert.Equal(new[] { "acc", "chr" }, d.Stmt("HumanSetSelect").AllBoundTexts);
        Assert.Equal(new[] { "acc", "chr" }, d.Stmt("HumanSetUnSelect").AllBoundTexts);
    }

    [Fact]
    public void HumanDb_Select_FalseWhenStepFails_ButStillUnselectsAndCommits()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanSetSelect").EnqueueResult(FakeResultSet.Step(false));
        Assert.False(db.HumanDB!.Select("acc", "chr"));
        Assert.Equal(1, d.Stmt("HumanSetUnSelect").StepCount);   // 原文无条件执行
        Assert.Equal(1, d.CommitCount);
    }

    [Fact]
    public void HumanDb_Select_RollsBackOnException()
    {
        var (db, f, d) = NewDb();
        d.Stmt("HumanSetSelect").EnqueueResult(FakeResultSet.Throwing(new InvalidOperationException("boom")));
        Assert.False(db.HumanDB!.Select("acc", "chr"));
        Assert.Equal(1, d.RollBackCount);
        Assert.Equal(0, d.CommitCount);
    }

    [Fact]
    public void HumanDb_Select_WrapsBothStatementsInOneTransaction()
    {
        // 原文 1183 BeginTransaction → Select/UnSelect → 1196/1197 finally Reset → 1200 Commit
        var (db, f, d) = NewDb();
        Assert.True(db.HumanDB!.Select("a", "b"));
        Assert.Equal(1, d.StartTransactionCount);
        Assert.Equal(1, d.CommitCount);
        Assert.Equal(0, d.RollBackCount);
    }
}
