using System;
using System.Collections.Generic;
using System.Linq;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.DBServer;
using Microsoft.Data.Sqlite;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>
/// `SelectClientHumanDb` / `SelectClientHeroDb`（`SelectClient.pas` 的 `g_RoleDB.HumanDB/HeroDB` 接线）
/// 以及它们依赖的 `RoleDatabase` 新增数据操作。
///
/// **不连任何外部数据库**：用临时目录下的真实 SQLite 文件（与既有 `RoleDatabase` 用法一致）。
/// 每条 SQL 都对齐 `MySqlRoleDB` 的原样语句（见 `RoleDatabase.cs` 的逐条注释）。
/// </summary>
public class SelectClientRoleDbAdapterTests : SelectClientTestBase
{
    private RoleDatabase Open(string name = "roles.db") => new RoleDatabase(Path2(name));

    /// <summary>只为"直接改某列取值"而开的一次性连接（验证列真的写进了库），不参与被测逻辑。</summary>
    private void ExecRaw(string sql, string name = "roles.db")
    {
        using var raw = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Path2(name) }.ToString());
        raw.Open();
        using var cmd = raw.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    private object? ScalarRaw(string sql, string name = "roles.db")
    {
        using var raw = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Path2(name) }.ToString());
        raw.Open();
        using var cmd = raw.CreateCommand();
        cmd.CommandText = sql;
        return cmd.ExecuteScalar();
    }

    private static TQueryHumanList Collect(RoleDatabase db, string account, bool deleted)
    {
        var l = new TQueryHumanList();
        if (deleted) db.QueryDeleteHumans(account, l);
        else db.QueryHumans(account, l);
        return l;
    }

    // =====================================================================================
    // GetID（原文 `select HumanID from Human where HumanName = ?;`）
    // =====================================================================================

    [Fact]
    public void GetID_查不到返回NO_ID()
    {
        using var db = Open();
        Assert.Equal(RoleDbConst.NO_ID, db.GetHumanId("Nope"));
    }

    [Fact]
    public void GetID_找到返回正数rowid()
    {
        using var db = Open();
        Assert.True(db.AddHuman("acct", "Aaaa", false, 0, 1, 2));
        Assert.True(db.GetHumanId("Aaaa") > 0);
    }

    [Fact]
    public void GetID_已删角色仍算存在_原文不过滤IsDelete()
    {
        // MySqlRoleDB `DoGetID` = `select HumanID from Human where HumanName = ?;` —— **没有** IsDelete 条件。
        // 这正是 SelectClient.pas:964 查重的真实语义：删掉的名字**仍然占用**。
        using var db = Open();
        db.AddHuman("acct", "Aaaa", false, 0, 1, 2);
        db.DeleteHuman("acct", "Aaaa");

        Assert.NotEqual(RoleDbConst.NO_ID, db.GetHumanId("Aaaa"));
        Assert.Equal(0, db.GetHumanCount("acct"));                               // 但不再计入"角色数"
    }

    [Fact]
    public void HeroGetID_只认IsHero为1的行()
    {
        using var db = Open();
        db.AddHuman("acct", "Aaaa", false, 0, 1, 2);                              // IsHero = 0

        Assert.Equal(RoleDbConst.NO_ID, db.GetHeroId("Aaaa"));
        Assert.NotEqual(RoleDbConst.NO_ID, db.GetHumanId("Aaaa"));
    }

    // =====================================================================================
    // GetHumanCount（原文 `select count(*) from Human where (Account = ?) and (IsDelete = 0);`）
    // =====================================================================================

    [Fact]
    public void GetHumanCount_只数本账号未删的角色()
    {
        using var db = Open();
        db.AddHuman("acct", "Aaaa", false, 0, 0, 0);
        db.AddHuman("acct", "Bbbb", false, 0, 0, 0);
        db.AddHuman("other", "Cccc", false, 0, 0, 0);
        db.DeleteHuman("acct", "Bbbb");

        Assert.Equal(1, db.GetHumanCount("acct"));
        Assert.Equal(1, db.GetHumanCount("other"));
        Assert.Equal(0, db.GetHumanCount("nobody"));
    }

    // =====================================================================================
    // Add（原文 `insert into Human(Account, HumanName, IsDelete, IsSelect, CreateDate, Sex, Job, Hair) ...`）
    // =====================================================================================

    [Fact]
    public void Add_写入了可被M2数据端读回的THumData()
    {
        // ★ 本表比原文多一列 Data（THumData 的 wire 二进制）；
        //   ProcessM2Data.DB_LOADHUMANRCD 要求它的长度**恰好**等于 StructBytes.SizeOf<THumData>()，否则返回 null。
        //
        // ★★ **不能用 `db.LoadHum(...).Value` 反复取值**：THumData 是 >10KB 的巨型结构
        //    （Grobal2.Types4.cs:128，含 TUserItemArray206 / ShortStr100Array500×2 …），
        //    每次 `.Value` 都是整块**值复制**；一帧里取 8 次就会 **栈溢出**（实测：测试宿主
        //    "Stack overflow" / Test Run Aborted，477 例通过后中止）。
        //    本仓既有约定见 `GXX.Core.Tests/CoreTests.cs:194-198`：在字节缓冲上用 `Unsafe.As` 建别名。
        using var db = Open();
        Assert.True(db.AddHuman("acct", "Aaaa", false, 3, 2, 1));

        byte[] wire = (byte[])ScalarRaw("SELECT Data FROM Roles WHERE ChrName='Aaaa'")!;
        Assert.Equal(StructBytes.SizeOf<THumData>(), wire.Length);                 // M2 数据端能读回的前提
        ref THumData h = ref System.Runtime.CompilerServices.Unsafe.As<byte, THumData>(ref wire[0]);

        Assert.Equal("acct", h.Account);
        Assert.Equal("Aaaa", h.ChrName);
        Assert.Equal((byte)2, h.btJob);
        Assert.Equal((byte)3, h.btSex);
        Assert.Equal("0", h.CurMap);
        Assert.Equal(1, h.Abil.Level);
        Assert.Equal(50u, h.Abil.MaxHP);
        Assert.Equal(30u, h.Abil.MaxMP);
    }

    [Fact]
    public void THumData是巨型结构_值复制是真实的栈开销()
    {
        // 把上面的教训固化成断言：任何"在栈上多复制几次 THumData"的写法都要先想想。
        Assert.True(StructBytes.SizeOf<THumData>() > 10000);
    }

    [Fact]
    public void Add_CreateDate按原文写成YYYYMMDD()
    {
        using var db = Open();
        RoleDbDate.Now = () => new DateTime(2026, 9, 14);
        try
        {
            db.AddHuman("acct", "Aaaa", false, 0, 0, 0);
            Assert.Equal(20260914, Convert.ToInt32(ScalarRaw("SELECT CreateDate FROM Roles WHERE ChrName='Aaaa'")));
            Assert.Equal(0, Convert.ToInt32(ScalarRaw("SELECT IsDelete FROM Roles WHERE ChrName='Aaaa'")));
            Assert.Equal(0, Convert.ToInt32(ScalarRaw("SELECT Hair FROM Roles WHERE ChrName='Aaaa'")));
        }
        finally
        {
            RoleDbDate.Now = () => DateTime.Now;
        }
    }

    [Fact]
    public void Add_IsSelect按参数写入()
    {
        using var db = Open();
        db.AddHuman("acct", "Aaaa", true, 0, 0, 0);
        db.AddHuman("acct", "Bbbb", false, 0, 0, 0);

        var l = Collect(db, "acct", deleted: false);
        Assert.True(l.Items(0)!.IsSelect);
        Assert.False(l.Items(1)!.IsSelect);
    }

    [Fact]
    public void Add_Hair被写入并在查询里读回()
    {
        using var db = Open();
        db.AddHuman("acct", "Aaaa", false, 0, 0, 7);
        Assert.Equal(7, Collect(db, "acct", false).Items(0)!.Hair);
    }

    // =====================================================================================
    // QueryHumans / QueryDeleteHumans（原文同一条 SQL，只有 IsDelete 参数不同）
    // =====================================================================================

    [Fact]
    public void QueryHumans_六列依次为名_选中_性别_职业_发型_等级()
    {
        using var db = Open();
        db.AddHuman("acct", "Aaaa", true, 1, 2, 3);
        ExecRaw("UPDATE Roles SET Level=42 WHERE ChrName='Aaaa'");

        var l = Collect(db, "acct", false);
        Assert.Equal(1, l.Count);
        var h = l.Items(0)!;
        Assert.Equal("Aaaa", h.HumanName);
        Assert.True(h.IsSelect);
        Assert.Equal(1, h.Sex);
        Assert.Equal(2, h.Job);
        Assert.Equal(3, h.Hair);
        Assert.Equal(42, h.Level);
    }

    [Fact]
    public void QueryHumans_性别职业越界被钳位到0()
    {
        // MySqlRoleDB.pas:934-968 `DoQueryHumans`：`Sex < 0 or > 1 → 0`；`Job < 0 or > 2 → 0`
        using var db = Open();
        db.AddHuman("acct", "Aaaa", false, 0, 0, 0);
        ExecRaw("UPDATE Roles SET Gender=9, Job=9 WHERE ChrName='Aaaa'");

        var h = Collect(db, "acct", false).Items(0)!;
        Assert.Equal(0, h.Sex);
        Assert.Equal(0, h.Job);
    }

    [Fact]
    public void QueryDeleteHumans_不做钳位_与QueryHumans的真实差异()
    {
        // ★★ MySqlRoleDB.pas:970-998 `DoQueryDeleteHumans` **没有**这两行钳位（原文的真实差异，测试锁定）。
        using var db = Open();
        db.AddHuman("acct", "Aaaa", false, 0, 0, 0);
        ExecRaw("UPDATE Roles SET Gender=9, Job=9, IsDelete=1 WHERE ChrName='Aaaa'");

        var h = Collect(db, "acct", deleted: true).Items(0)!;
        Assert.Equal(9, h.Sex);
        Assert.Equal(9, h.Job);
    }

    [Fact]
    public void QueryHumans_与QueryDeleteHumans按IsDelete分流()
    {
        using var db = Open();
        db.AddHuman("acct", "Live", false, 0, 0, 0);
        db.AddHuman("acct", "Gone", false, 0, 0, 0);
        db.DeleteHuman("acct", "Gone");

        Assert.Equal("Live", Collect(db, "acct", false).Items(0)!.HumanName);
        Assert.Equal("Gone", Collect(db, "acct", true).Items(0)!.HumanName);
        Assert.Equal(1, Collect(db, "acct", false).Count);
        Assert.Equal(1, Collect(db, "acct", true).Count);
    }

    [Fact]
    public void QueryHumans_只查本账号()
    {
        using var db = Open();
        db.AddHuman("acct", "Aaaa", false, 0, 0, 0);
        db.AddHuman("other", "Bbbb", false, 0, 0, 0);

        Assert.Equal(1, Collect(db, "acct", false).Count);
        Assert.Equal("Aaaa", Collect(db, "acct", false).Items(0)!.HumanName);
    }

    // =====================================================================================
    // Delete / DeleteRestore（原文 `update Human set IsDelete = ? where (Account = ?) and (HumanName = ?);`）
    // =====================================================================================

    [Fact]
    public void Delete_是软删_名字仍被占用()
    {
        using var db = Open();
        db.AddHuman("acct", "Aaaa", false, 0, 0, 0);

        Assert.True(db.DeleteHuman("acct", "Aaaa"));
        Assert.Equal(0, db.GetHumanCount("acct"));
        Assert.NotEqual(RoleDbConst.NO_ID, db.GetHumanId("Aaaa"));
        Assert.True(db.GetBaseInfo("Aaaa", out _, out _, out _, out _));          // 已删角色仍取得到基础信息
    }

    [Fact]
    public void DeleteRestore_把IsDelete清回0()
    {
        using var db = Open();
        db.AddHuman("acct", "Aaaa", false, 0, 0, 0);
        db.DeleteHuman("acct", "Aaaa");

        Assert.True(db.DeleteRestoreHuman("acct", "Aaaa"));
        Assert.Equal(1, db.GetHumanCount("acct"));
        Assert.Equal(0, Collect(db, "acct", true).Count);
    }

    [Fact]
    public void Delete_目标不存在时返回false()
    {
        using var db = Open();
        Assert.False(db.DeleteHuman("acct", "Nope"));
    }

    // =====================================================================================
    // Select（原文两条 update：IsSelect=1 / IsSelect=0 且 HumanName <> ?）
    // =====================================================================================

    [Fact]
    public void Select_置选中并清掉同账号其它角色的选中()
    {
        using var db = Open();
        db.AddHuman("acct", "Aaaa", true, 0, 0, 0);
        db.AddHuman("acct", "Bbbb", false, 0, 0, 0);

        Assert.True(db.SelectHuman("acct", "Bbbb"));

        var l = Collect(db, "acct", false);
        Assert.False(l.Items(0)!.IsSelect);
        Assert.True(l.Items(1)!.IsSelect);
    }

    [Fact]
    public void Select_只影响本账号()
    {
        using var db = Open();
        db.AddHuman("acct", "Aaaa", true, 0, 0, 0);
        db.AddHuman("other", "Bbbb", true, 0, 0, 0);

        db.SelectHuman("acct", "Aaaa");

        Assert.True(Collect(db, "other", false).Items(0)!.IsSelect);
    }

    [Fact]
    public void Select_角色不存在时返回false但返回值语义照原文()
    {
        // 原文 `Result := FStatementSelect.Step()` —— IsSelect=1 那条的 Step 结果即返回值。
        using var db = Open();
        Assert.False(db.SelectHuman("acct", "Nope"));
    }

    // =====================================================================================
    // GetBaseInfo（原文 `select Sex, Job, Level, LoginDate from Human where HumanName = ?;`）
    // =====================================================================================

    [Fact]
    public void GetBaseInfo_读回四列()
    {
        using var db = Open();
        db.AddHuman("acct", "Aaaa", false, 1, 2, 3);
        ExecRaw("UPDATE Roles SET Level=45, LoginDate=20260101 WHERE ChrName='Aaaa'");

        Assert.True(db.GetBaseInfo("Aaaa", out int sex, out int job, out int level, out int lastLogin));
        Assert.Equal(1, sex);
        Assert.Equal(2, job);
        Assert.Equal(45, level);
        Assert.Equal(20260101, lastLogin);
    }

    [Fact]
    public void GetBaseInfo_查不到返回false且四个out都为0()
    {
        using var db = Open();
        Assert.False(db.GetBaseInfo("Nope", out int sex, out int job, out int level, out int lastLogin));
        Assert.Equal(0, sex);
        Assert.Equal(0, job);
        Assert.Equal(0, level);
        Assert.Equal(0, lastLogin);
    }

    // =====================================================================================
    // 适配器 ↔ THumanDBBase 包装
    // =====================================================================================

    [Fact]
    public void 适配器_九个Do星号都接到RoleDatabase()
    {
        using var db = Open();
        var human = new SelectClientHumanDb(db);
        var hero = new SelectClientHeroDb(db);

        Assert.True(human.Add("acct", "Aaaa", true, 1, 2, 3));
        Assert.True(human.GetID("Aaaa") > 0);
        Assert.Equal(1, human.GetHumanCount("acct"));
        Assert.True(human.GetBaseInfo("Aaaa", out int sex, out int job, out int level, out int lastLogin));
        Assert.Equal(1, sex);
        Assert.Equal(2, job);
        Assert.Equal(1, level);
        Assert.Equal(0, lastLogin);

        var q = new TQueryHumanList();
        Assert.Equal(1, human.QueryHumans("acct", q));
        Assert.True(human.Select("acct", "Aaaa"));
        Assert.True(human.Delete("acct", "Aaaa"));
        var qd = new TQueryHumanList();
        Assert.Equal(1, human.QueryDeleteHumans("acct", qd));
        Assert.True(human.DeleteRestore("acct", "Aaaa"));
        Assert.Equal(RoleDbConst.NO_ID, hero.GetID("Aaaa"));
    }

    [Fact]
    public void 适配器_未接线的Do成员被THumanDBBase包装吞成日志与初值()
    {
        // ★ 诚实登记：`THumanDBBase` 的公开包装是
        //   `try { Do* } catch (Exception E) { RoleDbSeam.MainOutMessage(E.Message); }`（MySqlRoleDB.Base.cs:17-27）
        //   ⇒ 适配器里未接线的成员**不会**把异常抛给调用方，而是"记一条日志 + 返回初值"。
        //   本用例把这个行为**锁死**，以免后来人以为它会抛。
        using var db = Open();
        var human = new SelectClientHumanDb(db);

        var logs = new List<string>();
        var saved = RoleDbSeam.MainOutMessage;
        RoleDbSeam.MainOutMessage = s => logs.Add(s);
        try
        {
            int n = human.SearchByLevel(10, 1, new TSerarchRoleList());
            Assert.Equal(0, n);                                                   // 初值
            Assert.Contains(logs, s => s.Contains("SelectClientHumanDb") && s.Contains("SearchByLevel"));
        }
        finally
        {
            RoleDbSeam.MainOutMessage = saved;
        }
    }

    [Fact]
    public void 适配器_直接调Do成员才会抛_消息指名未接线()
    {
        using var db = Open();
        var human = new SelectClientHumanDb(db);
        var ex = Assert.Throws<System.Reflection.TargetInvocationException>(
            () => human.GetType().GetMethod("DoSearchByLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                     .Invoke(human, new object[] { 10, 1, new TSerarchRoleList() }));
        var inner = Assert.IsType<NotSupportedException>(ex.InnerException);
        Assert.Contains("SearchByLevel", inner.Message);
        Assert.Contains("SelectClientHumanDb", inner.Message);
    }

    // =====================================================================================
    // schema 迁移
    // =====================================================================================

    [Fact]
    public void 迁移_旧库缺增量列时自动补齐()
    {
        // 先用**旧 schema**（只有最初 7 列）建库，再用 RoleDatabase 打开 —— 应当自动 ALTER 补齐。
        string file = Path2("legacy.db");
        using (var raw = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = file }.ToString()))
        {
            raw.Open();
            using var cmd = raw.CreateCommand();
            cmd.CommandText = @"
CREATE TABLE Roles (
    AccountName TEXT NOT NULL, ChrName TEXT NOT NULL, IsHero INTEGER DEFAULT 0,
    Job INTEGER DEFAULT 0, Gender INTEGER DEFAULT 0, Level INTEGER DEFAULT 1, Data BLOB,
    PRIMARY KEY (AccountName, ChrName, IsHero));
INSERT INTO Roles (AccountName, ChrName, IsHero, Job, Gender, Level) VALUES ('acct','Old1',0,1,0,1);";
            cmd.ExecuteNonQuery();
        }

        using var db = new RoleDatabase(file);
        Assert.Equal(1, db.GetHumanCount("acct"));                                // 旧行 IsDelete 缺省为 0
        Assert.True(db.AddHuman("acct", "New1", false, 0, 0, 0));                 // 新列可用
        Assert.Equal(2, db.GetHumanCount("acct"));
        Assert.Equal("Old1", Collect(db, "acct", false).Items(0)!.HumanName);
    }
}
