using System;
using System.IO;
using GXX.Core;
using GXX.Core.Protocol;
using Microsoft.Data.Sqlite;

namespace GXX.DBServer;

/// <summary>
/// RoleDB.pas / SqliteRoleDB.pas → RoleDatabase.cs
/// 角色数据库：THumData/THeroData 以二进制（StructBytes 序列化，字节级兼容 Delphi wire 布局）存入 SQLite。
/// 表结构对应 SqliteCreateTableSql.pas 的角色表。
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

    private void EnsureSchema()
    {
        using var cmd = _db.CreateCommand();
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

    /// <summary>CM_QUERYCHR：列出账号的角色（QueryChr 返回包体）。</summary>
    public int QueryChr(string account, byte[] outBuf)
    {
        lock (_lock)
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = "SELECT ChrName, Job, Gender, Level FROM Roles WHERE AccountName=@a AND IsHero=0";
            cmd.Parameters.AddWithValue("@a", account);
            using var reader = cmd.ExecuteReader();
            int offset = 0;
            while (reader.Read())
            {
                string name = reader.GetString(0);
                byte job = Convert.ToByte(reader.GetValue(1));
                byte gender = Convert.ToByte(reader.GetValue(2));
                int level = Convert.ToInt32(reader.GetValue(3));
                var info = new TDeleteHumanInfo();
                info.ChrName = name;
                info.nLevel = level;
                info.btJob = job;
                info.btSex = gender;
                byte[] wire = StructBytes.BytesOf(info);
                if (offset + wire.Length > outBuf.Length) break;
                Array.Copy(wire, 0, outBuf, offset, wire.Length);
                offset += wire.Length;
            }
            return offset;
        }
    }

    public bool ChrExists(string account, string chrName, bool isHero)
    {
        lock (_lock)
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = "SELECT COUNT(1) FROM Roles WHERE AccountName=@a AND ChrName=@c AND IsHero=@h";
            cmd.Parameters.AddWithValue("@a", account);
            cmd.Parameters.AddWithValue("@c", chrName);
            cmd.Parameters.AddWithValue("@h", isHero ? 1 : 0);
            return (long)(cmd.ExecuteScalar() ?? 0) > 0;
        }
    }

    public bool ChrNameUsed(string chrName)
    {
        lock (_lock)
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = "SELECT COUNT(1) FROM Roles WHERE ChrName=@c";
            cmd.Parameters.AddWithValue("@c", chrName);
            return (long)(cmd.ExecuteScalar() ?? 0) > 0;
        }
    }

    /// <summary>CM_NEWCHR：创建角色（THumData 初始化）。</summary>
    public bool NewChr(string account, string chrName, byte job, byte gender)
    {
        lock (_lock)
        {
            if (ChrExists(account, chrName, false) || ChrNameUsed(chrName)) return false;
            var data = new THumData();
            data.Account = account;
            data.ChrName = chrName;
            data.btJob = job;
            data.btSex = gender;
            data.CurMap = "0";
            data.Abil = new TOAbility { Level = 1, MaxHP = 50, HP = 50, MaxMP = 30, MP = 30 };

            using var cmd = _db.CreateCommand();
            cmd.CommandText = "INSERT INTO Roles (AccountName, ChrName, IsHero, Job, Gender, Level, Data) VALUES (@a,@c,0,@j,@g,1,@d)";
            cmd.Parameters.AddWithValue("@a", account);
            cmd.Parameters.AddWithValue("@c", chrName);
            cmd.Parameters.AddWithValue("@j", job);
            cmd.Parameters.AddWithValue("@g", gender);
            cmd.Parameters.AddWithValue("@d", StructBytes.BytesOf(data));
            cmd.ExecuteNonQuery();
            return true;
        }
    }

    public bool DelChr(string account, string chrName)
    {
        lock (_lock)
        {
            using var cmd = _db.CreateCommand();
            cmd.CommandText = "DELETE FROM Roles WHERE AccountName=@a AND ChrName=@c AND IsHero=0";
            cmd.Parameters.AddWithValue("@a", account);
            cmd.Parameters.AddWithValue("@c", chrName);
            return cmd.ExecuteNonQuery() > 0;
        }
    }

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

    public void Dispose() => _db.Dispose();
}
