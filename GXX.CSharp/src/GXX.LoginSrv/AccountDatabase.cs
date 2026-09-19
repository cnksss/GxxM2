using System;
using System.IO;
using GXX.Core;
using GXX.Core.Protocol;
using Microsoft.Data.Sqlite;

namespace GXX.LoginSrv;

/// <summary>
/// AccountDB.pas / SqliteAccountDB.pas → AccountDatabase.cs
/// 账号数据库：SQLite 存储 + TAccountInfo wire 结构互转（GBK 定长字段）。
/// </summary>
public class AccountDatabase : IDisposable
{
    private readonly SqliteConnection _db;
    private readonly object _lock = new();

    public AccountDatabase(string dbFile)
    {
        string dir = Path.GetDirectoryName(Path.GetFullPath(dbFile)) ?? ".";
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        _db = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = dbFile }.ToString());
        _db.Open();
        EnsureSchema();
    }

    private void EnsureSchema()
    {
        using var cmd = _db.CreateCommand();
        // 对应 SqliteCreateTableSql.pas 的账号表结构
        cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS AccountInfo (
    AccountName TEXT PRIMARY KEY,
    IsDisable INTEGER DEFAULT 0,
    Password TEXT,
    UserName TEXT,
    IDCard TEXT,
    BirthDay TEXT,
    Questions1 TEXT, Answers1 TEXT,
    Questions2 TEXT, Answers2 TEXT,
    Phone TEXT, MobilePhone TEXT, Mail TEXT,
    L2Password TEXT,
    CreateDate INTEGER DEFAULT 0,
    LoginDate INTEGER DEFAULT 0,
    LoginMac TEXT,
    LoginIP INTEGER DEFAULT 0,
    LastActionTick INTEGER DEFAULT 0,
    ErrorCount INTEGER DEFAULT 0,
    Memo TEXT,
    UID TEXT, CID TEXT
);";
        cmd.ExecuteNonQuery();
    }

    public bool Exists(string account)
    {
        lock (_lock)
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = "SELECT COUNT(1) FROM AccountInfo WHERE AccountName=@a";
            cmd.Parameters.AddWithValue("@a", account);
            long n = (long)(cmd.ExecuteScalar() ?? 0);
            return n > 0;
        }
    }

    public TAccountInfo? Find(string account)
    {
        lock (_lock)
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = "SELECT * FROM AccountInfo WHERE AccountName=@a";
            cmd.Parameters.AddWithValue("@a", account);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            var info = new TAccountInfo();
            info.AccountNameStr = GetString(reader, "AccountName");
            info.IsDisable = GetInt(reader, "IsDisable") != 0 ? (byte)1 : (byte)0;
            info.PasswordStr = GetString(reader, "Password");
            info.UserNameStr = GetString(reader, "UserName");
            info.IDCardStr = GetString(reader, "IDCard");
            info.BirthDayStr = GetString(reader, "BirthDay");
            info.Questions1Str = GetString(reader, "Questions1");
            info.Answers1Str = GetString(reader, "Answers1");
            info.Questions2Str = GetString(reader, "Questions2");
            info.Answers2Str = GetString(reader, "Answers2");
            info.PhoneStr = GetString(reader, "Phone");
            info.MobilePhoneStr = GetString(reader, "MobilePhone");
            info.MailStr = GetString(reader, "Mail");
            info.L2PasswordStr = GetString(reader, "L2Password");
            info.CreateDate = GetInt(reader, "CreateDate");
            info.LoginDate = GetInt(reader, "LoginDate");
            info.LoginMacStr = GetString(reader, "LoginMac");
            info.LoginIP = GetInt(reader, "LoginIP");
            info.LastActionTick = (uint)GetInt(reader, "LastActionTick");
            info.ErrorCount = GetInt(reader, "ErrorCount");
            info.MemoStr = GetString(reader, "Memo");
            info.UidStr = GetString(reader, "UID");
            info.CidStr = GetString(reader, "CID");
            return info;
        }
    }

    public bool Add(TAccountInfo info)
    {
        lock (_lock)
        {
            if (Exists(info.AccountNameStr)) return false;
            using var cmd = _db.CreateCommand();
            cmd.CommandText = @"INSERT INTO AccountInfo
(AccountName, IsDisable, Password, UserName, IDCard, BirthDay, Questions1, Answers1, Questions2, Answers2,
 Phone, MobilePhone, Mail, L2Password, CreateDate, LoginDate, LoginMac, LoginIP, LastActionTick, ErrorCount, Memo, UID, CID)
VALUES (@a,@d,@p,@u,@i,@b,@q1,@a1,@q2,@a2,@ph,@mp,@m,@l2,@cd,@ld,@mac,@ip,@tick,@err,@memo,@uid,@cid)";
            FillParams(cmd, info);
            cmd.ExecuteNonQuery();
            return true;
        }
    }

    public bool Update(TAccountInfo info)
    {
        lock (_lock)
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = @"UPDATE AccountInfo SET IsDisable=@d, Password=@p, UserName=@u, IDCard=@i, BirthDay=@b,
Questions1=@q1, Answers1=@a1, Questions2=@q2, Answers2=@a2, Phone=@ph, MobilePhone=@mp, Mail=@m, L2Password=@l2,
CreateDate=@cd, LoginDate=@ld, LoginMac=@mac, LoginIP=@ip, LastActionTick=@tick, ErrorCount=@err, Memo=@memo, UID=@uid, CID=@cid
WHERE AccountName=@a";
            FillParams(cmd, info);
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public bool Delete(string account)
    {
        lock (_lock)
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = "DELETE FROM AccountInfo WHERE AccountName=@a";
            cmd.Parameters.AddWithValue("@a", account);
            return cmd.ExecuteNonQuery() > 0;
        }
    }

    public int ChangePassword(string account, string oldPwd, string newPwd)
    {
        TAccountInfo? found = Find(account);
        if (found == null) return -1;                    // 账号不存在
        TAccountInfo info = found.Value;
        if (!string.Equals(info.PasswordStr, oldPwd, StringComparison.Ordinal)) return 0; // 密码错误
        info.PasswordStr = newPwd;
        return Update(info) ? 1 : 0;
    }

    private static void FillParams(SqliteCommand cmd, in TAccountInfo info)
    {
        cmd.Parameters.AddWithValue("@a", info.AccountNameStr);
        cmd.Parameters.AddWithValue("@d", info.IsDisable);
        cmd.Parameters.AddWithValue("@p", info.PasswordStr);
        cmd.Parameters.AddWithValue("@u", info.UserNameStr);
        cmd.Parameters.AddWithValue("@i", info.IDCardStr);
        cmd.Parameters.AddWithValue("@b", info.BirthDayStr);
        cmd.Parameters.AddWithValue("@q1", info.Questions1Str);
        cmd.Parameters.AddWithValue("@a1", info.Answers1Str);
        cmd.Parameters.AddWithValue("@q2", info.Questions2Str);
        cmd.Parameters.AddWithValue("@a2", info.Answers2Str);
        cmd.Parameters.AddWithValue("@ph", info.PhoneStr);
        cmd.Parameters.AddWithValue("@mp", info.MobilePhoneStr);
        cmd.Parameters.AddWithValue("@m", info.MailStr);
        cmd.Parameters.AddWithValue("@l2", info.L2PasswordStr);
        cmd.Parameters.AddWithValue("@cd", info.CreateDate);
        cmd.Parameters.AddWithValue("@ld", info.LoginDate);
        cmd.Parameters.AddWithValue("@mac", info.LoginMacStr);
        cmd.Parameters.AddWithValue("@ip", info.LoginIP);
        cmd.Parameters.AddWithValue("@tick", info.LastActionTick);
        cmd.Parameters.AddWithValue("@err", info.ErrorCount);
        cmd.Parameters.AddWithValue("@memo", info.MemoStr);
        cmd.Parameters.AddWithValue("@uid", info.UidStr);
        cmd.Parameters.AddWithValue("@cid", info.CidStr);
    }

    private static string GetString(SqliteDataReader r, string col)
    {
        int i = r.GetOrdinal(col);
        return r.IsDBNull(i) ? "" : r.GetString(i);
    }

    private static int GetInt(SqliteDataReader r, string col)
    {
        int i = r.GetOrdinal(col);
        return r.IsDBNull(i) ? 0 : Convert.ToInt32(r.GetValue(i));
    }

    public void Dispose() => _db.Dispose();
}
