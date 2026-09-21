using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core;
using GXX.Core.Protocol;
using Microsoft.Data.Sqlite;

namespace GXX.DBServer;

/// <summary>
/// RoleDB.pas / SqliteRoleDB.pas / MySqlRoleDB.pas → RoleDatabase.cs
/// 角色数据库：THumData/THeroData 以二进制（StructBytes 序列化，字节级兼容 Delphi wire 布局）存入 SQLite。
///
/// <para>
/// 【本文件的两块职责】（并行车道 `p7-db-selectclient` 接线时对齐原文语义）
/// <list type="number">
///   <item><b>M2Server 数据端</b>：<see cref="LoadHum"/> / <see cref="SaveHum"/> / <see cref="LoadHero"/> /
///         <see cref="SaveHero"/> —— 由 <c>DBServerService.ProcessM2Data</c> 使用，**未改动**。</item>
///   <item><b>SelGate 选人端的数据操作</b>：<see cref="GetHumanId"/> / <see cref="GetHeroId"/> /
///         <see cref="GetHumanCount"/> / <see cref="QueryHumans"/> / <see cref="QueryDeleteHumans"/> /
///         <see cref="GetBaseInfo"/> / <see cref="SelectHuman"/> / <see cref="AddHuman"/> /
///         <see cref="DeleteHuman"/> / <see cref="DeleteRestoreHuman"/> ——
///         由 <c>SelectClientHumanDb</c>/<c>SelectClientHeroDb</c>（`SelectClient.RoleDbAdapter.cs`）适配成
///         <c>THumanDBBase</c>/<c>THeroDBBase</c> 后接到 <c>SelectClientRoleDbSeam</c>。</item>
/// </list>
/// </para>
///
/// <para>
/// 【为什么不沿用原来的 <c>QueryChr/NewChr/DelChr/ChrExists/ChrNameUsed</c>】
/// 那 5 个方法是为**旧 <c>ProcessGateData</c> 的 4 条命令**写的，语义与原文不一致（详见报告 §6.2）：
/// <list type="bullet">
///   <item><c>QueryChr</c> 发的是 <c>TDeleteHumanInfo</c> 6-bit 二进制；原文 SM_QUERYCHR 的包体是
///         **文本**串接 <c>[*]名/职业/发型/等级/性别/</c>（SelectClient.pas:1217-1222）。</item>
///   <item><c>NewChr</c>/<c>DelChr</c> 各自实现了一套查重/等级策略；原文这两件事都在
///         <c>TSelectClient</c> 里（:964 双库查重、:1097 `Level &gt; 45 → nCode -3`）。</item>
/// </list>
/// 接线后这 5 个方法**零调用方**（`src` + `tests` 双根已搜），留着就是"零调用方的错实现" ⇒ 删除，
/// 由下面这批按原文 SQL 逐字对齐的操作取代。**登记为偏差 D-p7-10**。
/// </para>
/// </summary>
public class RoleDatabase : IDisposable
{
    private readonly SqliteConnection _db;
    private readonly object _lock = new();

    public RoleDatabase(string dbFile)
    {
        string dir = Path.GetDirectoryName(Path.GetFullPath(dbFile)) ?? ".";
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        _db = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = dbFile }.ToString());
        _db.Open();
        EnsureSchema();
    }

    /// <summary>供适配器做事务（原文 <c>TRoleDB.Lock/UnLock</c> 的等价物已由 <c>THumanDBBase</c> 负责）。</summary>
    internal object SyncRoot => _lock;

    private void EnsureSchema()
    {
        using (var cmd = _db.CreateCommand())
        {
            cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS Roles (
    AccountName TEXT NOT NULL,
    ChrName TEXT NOT NULL,
    IsHero INTEGER DEFAULT 0,
    Job INTEGER DEFAULT 0,
    Gender INTEGER DEFAULT 0,
    Level INTEGER DEFAULT 1,
    Data BLOB,
    PRIMARY KEY (AccountName, ChrName, IsHero)
);";
            cmd.ExecuteNonQuery();
        }

        // ---- 增量列：对齐原文 Human 表的列（MySqlRoleDB 原样 SQL 见下）----
        //   FStatementQueryHumans   : select a.HumanName, a.IsSelect, a.Sex, a.Job, a.Hair, a.Level from Human a
        //                             where (Account = ?) and (IsDelete = ?);
        //   FStatementGetHumanCount : select count(*) from Human where (Account = ?) and (IsDelete = 0);
        //   FStatementGetHumanBaseInfo: select Sex, Job, Level, LoginDate from Human where HumanName = ?;
        //   FStatementAddHuman      : insert into Human(Account, HumanName, IsDelete, IsSelect, CreateDate,
        //                                              Sex, Job, Hair) values(?, ?, ?, ?, ?, ?, ?, ?);
        //   FStatementDeleteOrRestore: update Human set IsDelete = ? where (Account = ?) and (HumanName = ?);
        AddColumnIfMissing("IsDelete", "INTEGER NOT NULL DEFAULT 0");
        AddColumnIfMissing("IsSelect", "INTEGER NOT NULL DEFAULT 0");
        AddColumnIfMissing("Hair", "INTEGER NOT NULL DEFAULT 0");
        AddColumnIfMissing("CreateDate", "INTEGER NOT NULL DEFAULT 0");
        AddColumnIfMissing("LoginDate", "INTEGER NOT NULL DEFAULT 0");
    }

    /// <summary>
    /// 增量列迁移：只有真的缺列才 <c>ALTER TABLE</c>（`CREATE TABLE IF NOT EXISTS` 不会改既有库）。
    /// </summary>
    private void AddColumnIfMissing(string column, string decl)
    {
        using (var check = _db.CreateCommand())
        {
            check.CommandText = "PRAGMA table_info(Roles);";
            using var reader = check.ExecuteReader();
            while (reader.Read())
            {
                if (string.Equals(reader.GetString(1), column, StringComparison.OrdinalIgnoreCase)) return;
            }
        }
        using var alter = _db.CreateCommand();
        alter.CommandText = "ALTER TABLE Roles ADD COLUMN " + column + " " + decl + ";";
        alter.ExecuteNonQuery();
    }

    // ==========================================================================================
    // 一、M2Server 数据端（未改动）
    // ==========================================================================================

    /// <summary>CM_SELCHR：读取角色数据（DB_LOADHUMANRCD）。</summary>
    public THumData? LoadHum(string account, string chrName)
    {
        lock (_lock)
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = "SELECT Data FROM Roles WHERE AccountName=@a AND ChrName=@c AND IsHero=0";
            cmd.Parameters.AddWithValue("@a", account);
            cmd.Parameters.AddWithValue("@c", chrName);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            int i = reader.GetOrdinal("Data");
            if (reader.IsDBNull(i)) return null;
            byte[] wire = (byte[])reader.GetValue(i);
            if (wire.Length != StructBytes.SizeOf<THumData>()) return null;
            return StructBytes.FromBytes<THumData>(wire);
        }
    }

    /// <summary>DB_SAVEHUMANRCD：保存角色。</summary>
    public bool SaveHum(string account, string chrName, in THumData data)
    {
        lock (_lock)
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = "UPDATE Roles SET Data=@d, Level=@l WHERE AccountName=@a AND ChrName=@c AND IsHero=0";
            cmd.Parameters.AddWithValue("@d", StructBytes.BytesOf(data));
            cmd.Parameters.AddWithValue("@l", (int)data.Abil.Level);
            cmd.Parameters.AddWithValue("@a", account);
            cmd.Parameters.AddWithValue("@c", chrName);
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    /// <summary>英雄数据（DB_LOADHERORCD / DB_SAVEHERORCD）。</summary>
    public THeroData? LoadHero(string account, string chrName)
    {
        lock (_lock)
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = "SELECT Data FROM Roles WHERE AccountName=@a AND ChrName=@c AND IsHero=1";
            cmd.Parameters.AddWithValue("@a", account);
            cmd.Parameters.AddWithValue("@c", chrName);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            int i = reader.GetOrdinal("Data");
            if (reader.IsDBNull(i)) return null;
            byte[] wire = (byte[])reader.GetValue(i);
            return StructBytes.FromBytes<THeroData>(wire);
        }
    }

    public bool SaveHero(string account, string chrName, in THeroData data)
    {
        lock (_lock)
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = "UPDATE Roles SET Data=@d WHERE AccountName=@a AND ChrName=@c AND IsHero=1";
            cmd.Parameters.AddWithValue("@d", StructBytes.BytesOf(data));
            cmd.Parameters.AddWithValue("@a", account);
            cmd.Parameters.AddWithValue("@c", chrName);
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    // ==========================================================================================
    // 二、SelGate 选人端的数据操作（对齐 MySqlRoleDB 的原样 SQL）
    // ==========================================================================================

    /// <summary>
    /// MySqlRoleDB `DoGetID`：`select HumanID from Human where HumanName = ?;`
    /// ⇒ 本表用 `rowid` 代替 `HumanID`，并补 `IsHero = 0`（原文本表就是"只有人类"）。
    ///
    /// ★ 原文**不过滤 IsDelete** ⇒ 已删角色的名字**仍被视为占用**（这是 <c>NewChr</c> 查重的真实语义）。
    /// 查不到返回 <see cref="RoleDbConst.NO_ID"/>。
    /// </summary>
    public int GetHumanId(string humanName)
    {
        lock (_lock)
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = "SELECT rowid FROM Roles WHERE ChrName=@c AND IsHero=0 ORDER BY rowid LIMIT 1";
            cmd.Parameters.AddWithValue("@c", humanName);
            object? v = cmd.ExecuteScalar();
            return v == null || v is DBNull ? RoleDbConst.NO_ID : Convert.ToInt32(v);
        }
    }

    /// <summary>MySqlRoleDB(Hero) `DoGetID`：`select HeroID from Hero where HeroName = ?;` ⇒ 本表 <c>IsHero = 1</c>。</summary>
    public int GetHeroId(string heroName)
    {
        lock (_lock)
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = "SELECT rowid FROM Roles WHERE ChrName=@c AND IsHero=1 ORDER BY rowid LIMIT 1";
            cmd.Parameters.AddWithValue("@c", heroName);
            object? v = cmd.ExecuteScalar();
            return v == null || v is DBNull ? RoleDbConst.NO_ID : Convert.ToInt32(v);
        }
    }

    /// <summary>
    /// MySqlRoleDB `DoGetHumanCount`：`select count(*) from Human where (Account = ?) and (IsDelete = 0);`
    /// 查不到 → 0。
    /// </summary>
    public int GetHumanCount(string account)
    {
        lock (_lock)
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = "SELECT count(*) FROM Roles WHERE AccountName=@a AND IsHero=0 AND IsDelete=0";
            cmd.Parameters.AddWithValue("@a", account);
            object? v = cmd.ExecuteScalar();
            return v == null || v is DBNull ? 0 : Convert.ToInt32(v);
        }
    }

    /// <summary>
    /// MySqlRoleDB `DoGetBaseInfo`：`select Sex, Job, Level, LoginDate from Human where HumanName = ?;`
    /// 列索引 0..3 依次是 Sex/Job/Level/LoginDate；查不到 → False 且 4 个 out 全 0。
    /// ★ 同样**不过滤 IsDelete**。
    /// </summary>
    public bool GetBaseInfo(string humanName, out int sex, out int job, out int level, out int lastLogin)
    {
        sex = 0; job = 0; level = 0; lastLogin = 0;
        lock (_lock)
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = "SELECT Gender, Job, Level, LoginDate FROM Roles WHERE ChrName=@c AND IsHero=0 ORDER BY rowid LIMIT 1";
            cmd.Parameters.AddWithValue("@c", humanName);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return false;
            sex = reader.IsDBNull(0) ? 0 : Convert.ToInt32(reader.GetValue(0));
            job = reader.IsDBNull(1) ? 0 : Convert.ToInt32(reader.GetValue(1));
            level = reader.IsDBNull(2) ? 0 : Convert.ToInt32(reader.GetValue(2));
            lastLogin = reader.IsDBNull(3) ? 0 : Convert.ToInt32(reader.GetValue(3));
            return true;
        }
    }

    /// <summary>
    /// MySqlRoleDB `DoQueryHumans`（<c>IsDelete = 0</c>）：
    /// `select a.HumanName, a.IsSelect, a.Sex, a.Job, a.Hair, a.Level from Human a where (Account = ?) and (IsDelete = ?);`
    ///
    /// ★ 原文**只在这一支**做钳位：`Sex &lt; 0 or &gt; 1 → 0`、`Job &lt; 0 or &gt; 2 → 0`
    /// （<see cref="QueryDeleteHumans"/> **不做**，与原文的真实差异，测试里锁定）。
    /// </summary>
    public int QueryHumans(string account, TQueryHumanList humanList)
    {
        return QueryHumansCore(account, 0, clamp: true, humanList);
    }

    /// <summary>MySqlRoleDB `DoQueryDeleteHumans`（<c>IsDelete = 1</c>）：同 SQL，**无 Sex/Job 钳位**。</summary>
    public int QueryDeleteHumans(string account, TQueryHumanList humanList)
    {
        return QueryHumansCore(account, 1, clamp: false, humanList);
    }

    private int QueryHumansCore(string account, int isDelete, bool clamp, TQueryHumanList humanList)
    {
        int result = 0;
        lock (_lock)
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = "SELECT ChrName, IsSelect, Gender, Job, Hair, Level FROM Roles " +
                              "WHERE AccountName=@a AND IsHero=0 AND IsDelete=@d ORDER BY rowid";
            cmd.Parameters.AddWithValue("@a", account);
            cmd.Parameters.AddWithValue("@d", isDelete);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var queryData = new TQueryHumanData
                {
                    HumanName = reader.IsDBNull(0) ? "" : reader.GetString(0),
                    IsSelect = !reader.IsDBNull(1) && Convert.ToInt32(reader.GetValue(1)) != 0,
                    Sex = reader.IsDBNull(2) ? 0 : Convert.ToInt32(reader.GetValue(2)),
                    Job = reader.IsDBNull(3) ? 0 : Convert.ToInt32(reader.GetValue(3)),
                    Hair = reader.IsDBNull(4) ? 0 : Convert.ToInt32(reader.GetValue(4)),
                    Level = reader.IsDBNull(5) ? 0 : Convert.ToInt32(reader.GetValue(5)),
                };
                if (clamp)
                {
                    if (queryData.Sex < 0 || queryData.Sex > 1) queryData.Sex = 0;
                    if (queryData.Job < 0 || queryData.Job > 2) queryData.Job = 0;
                }
                humanList.Add(queryData);
                result++;
            }
        }
        return result;
    }

    /// <summary>
    /// MySqlRoleDB `DoSelect`：先 `update Human set IsSelect = 1 where (Account = ?) and (HumanName = ?);`
    /// （**该条的 Step 结果即返回值**），再 `update Human set IsSelect = 0 where (Account = ?) and (HumanName &lt;&gt; ?);`。
    /// 两条在一个事务里。
    /// </summary>
    public bool SelectHuman(string account, string humanName)
    {
        lock (_lock)
        {
            using var tx = _db.BeginTransaction();
            try
            {
                bool result;
                using (var sel = _db.CreateCommand())
                {
                    sel.Transaction = tx;
                    sel.CommandText = "UPDATE Roles SET IsSelect=1 WHERE AccountName=@a AND ChrName=@c AND IsHero=0";
                    sel.Parameters.AddWithValue("@a", account);
                    sel.Parameters.AddWithValue("@c", humanName);
                    result = sel.ExecuteNonQuery() > 0;
                }
                using (var unsel = _db.CreateCommand())
                {
                    unsel.Transaction = tx;
                    unsel.CommandText = "UPDATE Roles SET IsSelect=0 WHERE AccountName=@a AND ChrName<>@c AND IsHero=0";
                    unsel.Parameters.AddWithValue("@a", account);
                    unsel.Parameters.AddWithValue("@c", humanName);
                    unsel.ExecuteNonQuery();
                }
                tx.Commit();
                return result;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }

    /// <summary>
    /// MySqlRoleDB `DoAdd`：
    /// `insert into Human(Account, HumanName, IsDelete, IsSelect, CreateDate, Sex, Job, Hair) values(?,?,?,?,?,?,?,?);`
    /// 参数顺序：Account, HumanName, IsDelete=False, IsSelect, CreateDate=<c>RoleDbDate.Date2MyDate(Now())</c>, Sex, Job, Hair。
    ///
    /// ★ 本表比原文多一列 <c>Data</c>（THumData 的 wire 二进制）—— <c>ProcessM2Data.DB_LOADHUMANRCD</c> 要求它
    ///   是**合法的 THumData 长度**，否则返回 null。故新增角色时必须一并写入初始 <c>THumData</c>。
    ///   这段初始化逐字搬自被删除的旧 <c>RoleDatabase.NewChr</c>（本车道未改其值）。
    /// </summary>
    public bool AddHuman(string account, string humanName, bool isSelect, byte sex, byte job, byte hair)
    {
        lock (_lock)
        {
            var data = new THumData();
            data.Account = account;
            data.ChrName = humanName;
            data.btJob = job;
            data.btSex = sex;
            data.CurMap = "0";
            data.Abil = new TOAbility { Level = 1, MaxHP = 50, HP = 50, MaxMP = 30, MP = 30 };

            using var cmd = _db.CreateCommand();
            cmd.CommandText = "INSERT INTO Roles (AccountName, ChrName, IsHero, IsDelete, IsSelect, CreateDate, " +
                              "Job, Gender, Hair, Level, Data) " +
                              "VALUES (@a, @c, 0, 0, @s, @d, @j, @g, @h, 1, @blob)";
            cmd.Parameters.AddWithValue("@a", account);
            cmd.Parameters.AddWithValue("@c", humanName);
            cmd.Parameters.AddWithValue("@s", isSelect ? 1 : 0);
            cmd.Parameters.AddWithValue("@d", RoleDbDate.Date2MyDate(RoleDbDate.Now()));
            cmd.Parameters.AddWithValue("@j", (int)job);
            cmd.Parameters.AddWithValue("@g", (int)sex);
            cmd.Parameters.AddWithValue("@h", (int)hair);
            cmd.Parameters.AddWithValue("@blob", StructBytes.BytesOf(data));
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    /// <summary>MySqlRoleDB `DoDelete`：`update Human set IsDelete = 1 where (Account = ?) and (HumanName = ?);`（软删）。</summary>
    public bool DeleteHuman(string account, string humanName) => SetDeleted(account, humanName, 1);

    /// <summary>MySqlRoleDB `DoDeleteRestore`：同一条语句，`IsDelete = 0`。</summary>
    public bool DeleteRestoreHuman(string account, string humanName) => SetDeleted(account, humanName, 0);

    private bool SetDeleted(string account, string humanName, int isDelete)
    {
        lock (_lock)
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = "UPDATE Roles SET IsDelete=@d WHERE AccountName=@a AND ChrName=@c AND IsHero=0";
            cmd.Parameters.AddWithValue("@d", isDelete);
            cmd.Parameters.AddWithValue("@a", account);
            cmd.Parameters.AddWithValue("@c", humanName);
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public void Dispose() => _db.Dispose();
}
