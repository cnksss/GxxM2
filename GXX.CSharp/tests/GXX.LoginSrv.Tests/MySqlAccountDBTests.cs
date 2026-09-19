using System;
using System.Reflection;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;
using GXX.LoginSrv;
using Xunit;

namespace GXX.LoginSrv.Tests;

/// <summary>
/// MySqlAccountDB.pas TMySqlAccountDB 1:1 测试。
/// DB 访问走 IMySqlDatabaseFactory 接缝（内存实现，不连真实 MySQL）；SQL 文本逐字断言。
/// </summary>
public sealed class MySqlAccountDBTests : IDisposable
{
    private readonly FakeMySqlFactory _factory = new();
    private TMySqlAccountDB _db = null!;

    public MySqlAccountDBTests()
    {
        LoginSrvShare.ResetForTests();
    }

    public void Dispose()
    {
        LoginSrvShare.ResetForTests();
    }

    private FakeMySqlDatabase NewDb(TDBServerConfig? cfg = null)
    {
        _db = new TMySqlAccountDB(cfg ?? new TDBServerConfig
        {
            DBServer = "127.0.0.1",
            DBUser = "root",
            DBPassword = "pw",
            DataBase = "mir",
            DBPort = 3306,
        }, _factory);
        _db.Init();
        return _factory.Last!;
    }

    // ------------------------------------------------------------------
    // 1) DoInit：连接参数 + SQL 文本逐字
    // ------------------------------------------------------------------

    [Fact]
    public void DoInit_ConnectsWithDelphiParameters()
    {
        var fake = NewDb();

        Assert.Equal(1, fake.InitCount);
        Assert.Equal("127.0.0.1|root|pw|mir|3306|65536", Assert.Single(fake.ConnectLog));
        Assert.Equal("utf8", fake.CharacterSetName);
        Assert.NotNull(fake.MySql);
    }

    [Fact]
    public void DoInit_PreservesSqlTextVerbatim()
    {
        var fake = NewDb();

        Assert.Equal(
            "select Account, Disable, Password, UserName, IDCard, BirthDay, " +
            "Questions1, Answers1, Questions2, Answers2, Phone, MobilePhone, Mail, " +
            "L2Password, CreateDate, LoginDate, LoginMac, LoginIP, LastActionTick, " +
            "ErrorCount, Memo " +
            "from Account where Account = ?", fake.Statements["select_Account"].Sql);

        Assert.Equal(
            "select Account, Disable, Password, UserName, IDCard, BirthDay, " +
            "Questions1, Answers1, Questions2, Answers2, Phone, MobilePhone, Mail, " +
            "L2Password, CreateDate, LoginDate, LoginMac, LoginIP, LastActionTick, " +
            "ErrorCount, Memo " +
            "from Account where MobilePhone = ?", fake.Statements["select_Account_phone"].Sql);

        Assert.Equal(
            "select Account, Disable, Password, UserName, IDCard, BirthDay, " +
            "Questions1, Answers1, Questions2, Answers2, Phone, MobilePhone, Mail, " +
            "L2Password, CreateDate, LoginDate, LoginMac, LoginIP, LastActionTick, " +
            "ErrorCount, Memo, UID, CID " +
            "from Account where UID = ? and CID = ?", fake.Statements["select_Account_quick"].Sql);

        Assert.Equal(
            "select Account, Disable, Password, UserName, IDCard, BirthDay, " +
            "Questions1, Answers1, Questions2, Answers2, Phone, MobilePhone, Mail, " +
            "L2Password, CreateDate, LoginDate, LoginMac, LoginIP, LastActionTick, " +
            "ErrorCount, Memo " +
            "from Account where Account LIKE ?", fake.Statements["find_Account"].Sql);

        Assert.Equal(
            "insert into Account(" +
            "Account, " +
            "Password, " +
            "UserName, " +
            "IDCard, " +
            "BirthDay, " +
            "Questions1, " +
            "Answers1, " +
            "Questions2, " +
            "Answers2, " +
            "Phone, " +
            "MobilePhone," +
            "Mail, " +
            "L2Password, " +
            "CreateDate, " +
            "Memo, " +
            "UID, " +
            "CID) " +
            "values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
            fake.Statements["new_Account"].Sql);

        Assert.Equal("select 1 from Account where Account = ?;",
            fake.Statements["exists_Account"].Sql);

        Assert.Equal(
            "update Account set " +
            "L2Password = ?, " +
            "LoginDate = ?, " +
            "LoginMac = ?, " +
            "LoginIP = ?, " +
            "LastActionTick = ?, " +
            "ErrorCount = ? " +
            "where Account = ? ", fake.Statements["update_Account_1"].Sql);

        Assert.Equal(
            "update Account set " +
            "Password = ?, " +
            "UserName = ?, " +
            "IDCard = ?, " +
            "BirthDay = ?, " +
            "Questions1 = ?, " +
            "Answers1 = ?, " +
            "Questions2 = ?, " +
            "Answers2 = ?, " +
            "Phone = ?, " +
            "MobilePhone = ?, " +
            "Mail = ?, " +
            "L2Password = ?, " +
            "Memo = ? " +
            "where Account = ? ", fake.Statements["update_Account_2"].Sql);

        Assert.Equal(
            "update Account set " +
            "LastActionTick = 0, " +
            "ErrorCount = 0 " +
            "where Account = ? ", fake.Statements["unlock_Account"].Sql);

        Assert.Equal("select Account, Disable from Account",
            fake.Statements["get_all_Account"].Sql);

        Assert.Equal("update Account set Disable = ? where Account = ? ",
            fake.Statements["enabled_Account"].Sql);
    }

    [Fact]
    public void DoInit_PreparesAllElevenStatements()
    {
        var fake = NewDb();
        Assert.Equal(11, fake.Statements.Count);
        foreach (var kv in fake.Statements)
            Assert.Equal(1, kv.Value.PrepareCount);
    }

    // ------------------------------------------------------------------
    // 2) DoGetAccount（21 列）
    // ------------------------------------------------------------------

    private static FakeRow FullRow()
    {
        var r = new FakeRow { Account = "alice", Disable = 1 };
        r.Cols[2] = "pwd";
        r.Cols[3] = "张三";
        r.Cols[4] = "110101199001011234";
        r.Cols[5] = "19900101";
        r.Cols[6] = "q1";
        r.Cols[7] = "a1";
        r.Cols[8] = "q2";
        r.Cols[9] = "a2";
        r.Cols[10] = "010-1234";
        r.Cols[11] = "13800000000";
        r.Cols[12] = "a@b.c";
        r.Cols[13] = "l2pwd";
        r.Cols[14] = "20240101";
        r.Cols[15] = "20240202";
        r.Cols[16] = "AABBCCDDEEFF00112233445566778899";
        r.Cols[17] = "16909060";
        r.Cols[18] = "123456";
        r.Cols[19] = "3";
        r.Cols[20] = "memo";
        return r;
    }

    [Fact]
    public void GetAccount_MapsAll21Columns()
    {
        var fake = NewDb();
        fake.Rows.Add(FullRow());

        TAccountInfo info = default;
        Assert.True(_db.GetAccount("alice", ref info));

        Assert.Equal("alice", info.AccountNameStr);
        Assert.Equal(1, info.IsDisable);
        Assert.Equal("pwd", info.PasswordStr);
        Assert.Equal("张三", info.UserNameStr);
        Assert.Equal("110101199001011234", info.IDCardStr);
        Assert.Equal("19900101", info.BirthDayStr);
        Assert.Equal("q1", info.Questions1Str);
        Assert.Equal("a1", info.Answers1Str);
        Assert.Equal("q2", info.Questions2Str);
        Assert.Equal("a2", info.Answers2Str);
        Assert.Equal("010-1234", info.PhoneStr);
        Assert.Equal("13800000000", info.MobilePhoneStr);
        Assert.Equal("a@b.c", info.MailStr);
        Assert.Equal("l2pwd", info.L2PasswordStr);
        Assert.Equal(20240101, info.CreateDate);
        Assert.Equal(20240202, info.LoginDate);
        Assert.Equal("AABBCCDDEEFF00112233445566778899", info.LoginMacStr);
        Assert.Equal(16909060, info.LoginIP);
        Assert.Equal(123456u, info.LastActionTick);
        Assert.Equal(3, info.ErrorCount);
        Assert.Equal("memo", info.MemoStr);
    }

    [Fact]
    public void GetAccount_NotFound_ReturnsFalse_AndLeavesInfoUntouched()
    {
        var fake = NewDb();
        fake.Rows.Add(FullRow());
        TAccountInfo info = default;
        info.AccountNameStr = "keep";

        Assert.False(_db.GetAccount("bob", ref info));
        Assert.Equal("keep", info.AccountNameStr);
    }

    [Fact]
    public void GetAccount_StatementResetsAfterEachCall()
    {
        var fake = NewDb();
        fake.Rows.Add(FullRow());
        TAccountInfo info = default;
        _db.GetAccount("alice", ref info);
        int after = fake.Statements["select_Account"].ResetCount;
        _db.GetAccount("alice", ref info);
        Assert.True(fake.Statements["select_Account"].ResetCount > after);
    }

    /// <summary>
    /// 原文 Assert(FieldCount = 21) 失败时抛异常（文案 1:1）；
    /// 异常被 TAccountDB.FindAccount 的 except 吞掉并 MainOutMessage，返回值 0。
    /// </summary>
    [Fact]
    public void FindAccount_ColumnCountMismatch_ReportsDelphiAssertText()
    {
        var fake = NewDb();
        fake.Rows.Add(FullRow());
        fake.ForceFieldCount = ("find_Account", 23);   // 人为让列数 = 23 ≠ 21

        int n = _db.FindAccount("al", new TAccountList());

        Assert.Equal(0, n);
        Assert.Contains(LoginSrvShare.g_MainMsgList.AsEnumerable(),
            s => s.Contains("FindAccount column count error.", StringComparison.Ordinal));
    }

    /// <summary>GetAccount 的断言文案与列数 21。</summary>
    [Fact]
    public void GetAccount_ColumnCountMismatch_ReportsDelphiAssertText()
    {
        var fake = NewDb();
        fake.Rows.Add(FullRow());
        fake.ForceFieldCount = ("select_Account", 22);

        TAccountInfo info = default;
        Assert.False(_db.GetAccount("alice", ref info));
        Assert.Contains(LoginSrvShare.g_MainMsgList.AsEnumerable(),
            s => s.Contains("GetAccount column count error.", StringComparison.Ordinal));
    }

    /// <summary>GetAccountByQuick 的断言文案仍是 "GetAccount column count error."（原文如此）但列数为 23。</summary>
    [Fact]
    public void GetAccountByQuick_ColumnCountMismatch_ReportsDelphiAssertText()
    {
        var fake = NewDb();
        var row = FullRow();
        row.UID = "u";
        row.CID = "c";
        fake.Rows.Add(row);
        fake.ForceFieldCount = ("select_Account_quick", 21);

        TAccountInfo info = default;
        Assert.False(_db.GetAccountByQuick("u", "c", ref info));
        Assert.Contains(LoginSrvShare.g_MainMsgList.AsEnumerable(),
            s => s.Contains("GetAccount column count error.", StringComparison.Ordinal));
    }

    // ------------------------------------------------------------------
    // 3) DoGetAccountByQuick / ByPhone
    // ------------------------------------------------------------------

    [Fact]
    public void GetAccountByQuick_MapsUidAndCid()
    {
        var fake = NewDb();
        var row = FullRow();
        row.UID = "UID-1";
        row.CID = "CID-2";
        fake.Rows.Add(row);

        TAccountInfo info = default;
        Assert.True(_db.GetAccountByQuick("UID-1", "CID-2", ref info));
        Assert.Equal("alice", info.AccountNameStr);
        Assert.Equal("UID-1", info.UidStr);
        Assert.Equal("CID-2", info.CidStr);
    }

    [Fact]
    public void GetAccountByQuick_WrongCid_ReturnsFalse()
    {
        var fake = NewDb();
        var row = FullRow();
        row.UID = "UID-1";
        row.CID = "CID-2";
        fake.Rows.Add(row);

        TAccountInfo info = default;
        Assert.False(_db.GetAccountByQuick("UID-1", "CID-X", ref info));
    }

    [Fact]
    public void GetAccountByPhone_MatchesMobilePhone()
    {
        var fake = NewDb();
        fake.Rows.Add(FullRow());

        TAccountInfo info = default;
        Assert.True(_db.GetAccountByPhone("13800000000", ref info));
        Assert.Equal("alice", info.AccountNameStr);

        TAccountInfo info2 = default;
        Assert.False(_db.GetAccountByPhone("13900000000", ref info2));
    }

    // ------------------------------------------------------------------
    // 4) DoFindAccount（LIKE）
    // ------------------------------------------------------------------

    [Fact]
    public void FindAccount_AppendsPercent_AndClearsListFirst()
    {
        var fake = NewDb();
        fake.Rows.Add(new FakeRow { Account = "alice" });
        fake.Rows.Add(new FakeRow { Account = "alan" });
        fake.Rows.Add(new FakeRow { Account = "bob" });

        var list = new TAccountList();
        list.Add(default);   // 先塞一个旧元素，验证 FindAccount 先 Clear

        int n = _db.FindAccount("al", list);

        Assert.Equal(2, n);
        Assert.Equal(2, list.Count);
        Assert.Equal("al%", fake.Statements["find_Account"].LastQueryIndexed[0]);
        Assert.Equal("alice", list[0].AccountNameStr);
        Assert.Equal("alan", list[1].AccountNameStr);
    }

    [Fact]
    public void FindAccount_NoMatch_ReturnsZero()
    {
        var fake = NewDb();
        fake.Rows.Add(new FakeRow { Account = "alice" });
        var list = new TAccountList();
        Assert.Equal(0, _db.FindAccount("zz", list));
    }

    // ------------------------------------------------------------------
    // 5) DoUpdateAccount
    // ------------------------------------------------------------------

    [Fact]
    public void UpdateAccount_PartField_BindsSevenParamsInOrder()
    {
        var fake = NewDb();
        fake.Rows.Add(new FakeRow { Account = "alice" });

        TAccountInfo info = default;
        info.AccountNameStr = "alice";
        info.L2PasswordStr = "l2";
        info.LoginDate = 20240303;
        info.LoginMacStr = "MAC";
        info.LoginIP = 7;
        info.LastActionTick = 42;
        info.ErrorCount = 5;

        Assert.True(_db.UpdateAccount(info, TAccountUpdateField.ufPartField));

        var ordered = fake.Statements["update_Account_1"].LastStepParams;
        Assert.Equal(new object[] { "l2", 20240303, "MAC", 7, 42, 5, "alice" }, ordered);
        var row = fake.Rows[0];
        Assert.Equal("20240303", row.Cols[15]);
        Assert.Equal("42", row.Cols[18]);
        Assert.Equal("5", row.Cols[19]);
    }

    [Fact]
    public void UpdateAccount_AllField_BindsFourteenParamsInOrder()
    {
        var fake = NewDb();
        fake.Rows.Add(new FakeRow { Account = "alice" });

        TAccountInfo info = default;
        info.AccountNameStr = "alice";
        info.PasswordStr = "p";
        info.UserNameStr = "u";
        info.IDCardStr = "i";
        info.BirthDayStr = "b";
        info.Questions1Str = "q1";
        info.Answers1Str = "a1";
        info.Questions2Str = "q2";
        info.Answers2Str = "a2";
        info.PhoneStr = "ph";
        info.MobilePhoneStr = "mp";
        info.MailStr = "m";
        info.L2PasswordStr = "l2";
        info.MemoStr = "memo";

        Assert.True(_db.UpdateAccount(info, TAccountUpdateField.ufAllField));

        var ordered = fake.Statements["update_Account_2"].LastStepParams;
        Assert.Equal(new object[] { "p", "u", "i", "b", "q1", "a1", "q2", "a2", "ph", "mp", "m", "l2", "memo", "alice" }, ordered);
    }

    /// <summary>ufPartField 以外的任何值都走 else 分支 = 全字段更新（原文 if UpdateField = ufPartField / else）。</summary>
    [Fact]
    public void UpdateAccount_NonPartField_FallsIntoAllFieldBranch()
    {
        var fake = NewDb();
        fake.Rows.Add(new FakeRow { Account = "alice" });
        TAccountInfo info = default;
        info.AccountNameStr = "alice";
        info.PasswordStr = "p";

        Assert.True(_db.UpdateAccount(info, (TAccountUpdateField)99));
        Assert.Equal(14, fake.Statements["update_Account_2"].LastStepParams.Count);
        Assert.Empty(fake.Statements["update_Account_1"].LastStepParams);
    }

    [Fact]
    public void UpdateAccount_UnknownAccount_StepReturnsFalse()
    {
        var fake = NewDb();
        TAccountInfo info = default;
        info.AccountNameStr = "ghost";
        Assert.False(_db.UpdateAccount(info, TAccountUpdateField.ufPartField));
    }

    // ------------------------------------------------------------------
    // 6) DoCheckAccountExists / DoAddAccount / DoUnLockAccount
    // ------------------------------------------------------------------

    [Fact]
    public void CheckAccountExists_QueryAndFetch()
    {
        var fake = NewDb();
        fake.Rows.Add(new FakeRow { Account = "alice" });
        Assert.True(_db.CheckAccountExists("alice"));
        Assert.False(_db.CheckAccountExists("bob"));
    }

    [Fact]
    public void AddAccount_BindsSeventeenParams_AndCreateDateIsDate2MyDateNow()
    {
        var fake = NewDb();

        TAccountInfo info = default;
        info.AccountNameStr = "newbie";
        info.PasswordStr = "p";
        info.UserNameStr = "u";
        info.IDCardStr = "i";
        info.BirthDayStr = "b";
        info.Questions1Str = "q1";
        info.Answers1Str = "a1";
        info.Questions2Str = "q2";
        info.Answers2Str = "a2";
        info.PhoneStr = "ph";
        info.MobilePhoneStr = "mp";
        info.MailStr = "m";
        info.L2PasswordStr = "l2";
        info.MemoStr = "memo";
        info.UidStr = "uid";
        info.CidStr = "cid";

        Assert.True(_db.AddAccount(info));

        var ordered = fake.Statements["new_Account"].LastStepParams;
        Assert.Equal(17, ordered.Count);
        Assert.Equal("newbie", ordered[0]);
        Assert.Equal("p", ordered[1]);
        Assert.Equal("cid", ordered[16]);
        // 第 14 个参数是 CreateDate = Date2MyDate(Now)（LSShare.pas:432）
        Assert.Equal(LoginSrvShare.Date2MyDate(DateTime.Now), Convert.ToInt32(ordered[13]));
        Assert.Equal("uid", ordered[15]);
        Assert.Single(fake.Rows);
        Assert.Equal("newbie", fake.Rows[0].Account);
    }

    [Fact]
    public void UnLockAccount_BindsAccountAndZeroesCounters()
    {
        var fake = NewDb();
        var row = new FakeRow { Account = "alice" };
        row.Cols[18] = "999";
        row.Cols[19] = "9";
        fake.Rows.Add(row);

        Assert.True(_db.UnLockAccount("alice"));
        Assert.Equal(new object[] { "alice" }, fake.Statements["unlock_Account"].LastStepParams);
        Assert.Equal("0", fake.Rows[0].Cols[18]);
        Assert.Equal("0", fake.Rows[0].Cols[19]);
    }

    [Fact]
    public void UnLockAccount_Unknown_ReturnsFalse()
    {
        var fake = NewDb();
        Assert.False(_db.UnLockAccount("ghost"));
    }

    // ------------------------------------------------------------------
    // 7) DoGetAllAccount / DoEnabledAccounts
    // ------------------------------------------------------------------

    [Fact]
    public void GetAllAccount_PutsDisableIntoObjects()
    {
        var fake = NewDb();
        fake.Rows.Add(new FakeRow { Account = "a1", Disable = 0 });
        fake.Rows.Add(new FakeRow { Account = "a2", Disable = 1 });

        var list = new TStringList();
        list.Add("stale");
        _db.GetAllAccount(list);

        Assert.Equal(2, list.Count);
        Assert.Equal("a1", list[0]);
        Assert.Equal(0, Convert.ToInt32(list.GetObject(0)));
        Assert.Equal("a2", list[1]);
        Assert.Equal(1, Convert.ToInt32(list.GetObject(1)));
    }

    /// <summary>
    /// ★ 差异断言：`OrderBindParamInt(Integer(not Enabled))` ——
    /// Enabled=False → Integer(not False) = Integer(True) = **-1**（Delphi Boolean 全 1 位）；
    /// Enabled=True → Integer(not True) = 0。写入 Disable 列因此是 -1 / 0。
    /// </summary>
    [Fact]
    public void EnabledAccounts_BindsNotEnabledAsMinusOneOrZero_InTransaction()
    {
        var fake = NewDb();
        fake.Rows.Add(new FakeRow { Account = "a1", Disable = 0 });
        fake.Rows.Add(new FakeRow { Account = "a2", Disable = 0 });

        var list = new TStringList();
        list.Add("a1");
        list.Add("a2");

        Assert.True(_db.EnabledAccounts(list, false));
        Assert.Equal(1, fake.CommitCount);
        Assert.Equal(0, fake.RollbackCount);
        Assert.Equal(-1, fake.Rows[0].Disable);
        Assert.Equal(-1, fake.Rows[1].Disable);
        // LastStepParams 是最后一次绑定的快照（循环最后一项 a2）
        Assert.Equal(new object[] { -1, "a2" }, fake.Statements["enabled_Account"].LastStepParams);

        var list2 = new TStringList();
        list2.Add("a1");
        Assert.True(_db.EnabledAccounts(list2, true));
        Assert.Equal(0, fake.Rows[0].Disable);
        Assert.Equal(new object[] { 0, "a1" }, fake.Statements["enabled_Account"].LastStepParams);
    }

    /// <summary>Step 返回 False 不算异常（不存在的帐号）→ 仍 Commit（原文只捕获异常）。</summary>
    [Fact]
    public void EnabledAccounts_MissingAccount_StillCommits()
    {
        var fake = NewDb();
        // 不存在的帐号 → Step 返回 false（不抛异常）→ 仍 Commit
        var list = new TStringList();
        list.Add("ghost");
        Assert.True(_db.EnabledAccounts(list, false));
        Assert.Equal(1, fake.CommitCount);
    }

    [Fact]
    public void EnabledAccounts_RollsBackWhenStatementThrows()
    {
        var fake = NewDb();
        fake.ThrowOnStep = true;
        var list = new TStringList();
        list.Add("a1");

        Assert.False(_db.EnabledAccounts(list, false));
        Assert.Equal(0, fake.CommitCount);
        Assert.Equal(1, fake.RollbackCount);
    }

    // ------------------------------------------------------------------
    // 8) DoFinal / Run / OnRequest / 接缝
    // ------------------------------------------------------------------

    [Fact]
    public void DoFinal_FinalizesAllStatements_AndFainalIsAlias()
    {
        var fake = NewDb();
        _db.Fainal();
        foreach (var kv in fake.Statements)
            Assert.Equal(1, kv.Value.FinalizeCount);
    }

    [Fact]
    public void Run_ImmediatelyAfterInit_DoesNotExec()
    {
        var fake = NewDb();
        _db.Run();
        Assert.Empty(fake.ExecLog);
    }

    /// <summary>10 分钟无请求 → Exec('select 1') 保活（私有 FLastRequestTick 用反射回拨）。</summary>
    [Fact]
    public void Run_WhenIdleOverTenMinutes_ExecsSelect1()
    {
        var fake = NewDb();
        SetLastRequestTick(_db, unchecked(DelphiRTL.GetTickCount() - 600001));

        _db.Run();

        Assert.Equal(new[] { "select 1" }, fake.ExecLog);
    }

    /// <summary>FDB.MySQL = nil（未连接）时即使超时也不保活。</summary>
    [Fact]
    public void Run_WhenNotConnected_DoesNotExec()
    {
        var fake = NewDb();
        fake.SetConnectedForTest(false);
        SetLastRequestTick(_db, unchecked(DelphiRTL.GetTickCount() - 600001));

        _db.Run();

        Assert.Empty(fake.ExecLog);
    }

    /// <summary>任意一次请求（Query/Step）都会经 OnRequest 刷新 FLastRequestTick。</summary>
    [Fact]
    public void OnRequest_RefreshesLastRequestTick()
    {
        var fake = NewDb();
        fake.Rows.Add(new FakeRow { Account = "alice" });
        SetLastRequestTick(_db, unchecked(DelphiRTL.GetTickCount() - 600001));

        TAccountInfo info = default;
        _db.GetAccount("alice", ref info);   // → Query → OnRequest → 刷新 tick

        _db.Run();
        Assert.Empty(fake.ExecLog);
    }

    private static void SetLastRequestTick(TMySqlAccountDB db, uint value)
    {
        FieldInfo f = typeof(TMySqlAccountDB).GetField("FLastRequestTick", BindingFlags.NonPublic | BindingFlags.Instance)!;
        f.SetValue(db, value);
    }

    /// <summary>未注入 factory 时使用默认接缝 → 明确抛出（libmysql 未移植）。</summary>
    [Fact]
    public void DefaultFactory_ThrowsNotSupported()
    {
        Assert.Throws<NotSupportedException>(() => new TMySqlAccountDB(new TDBServerConfig()));
        Assert.Throws<NotSupportedException>(() => MySqlNativeSeamFactory.Instance.CreateLib());
    }
}
