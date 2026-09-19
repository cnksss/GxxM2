using System;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.LoginSrv;

/// <summary>
/// MySqlAccountDB.pas TDBServerConfig（字段 1:1）。
/// </summary>
public sealed class TDBServerConfig
{
    public string DBServer = "";
    public ushort DBPort;
    public string DBUser = "";
    public string DBPassword = "";
    public string DataBase = "";
}

/// <summary>
/// ★ 接缝：MySQLDataBase.pas / MySQLWrap.pas / MySQLCli.pas 未移植。
/// 原文直接调用 libmysql-32.dll（TMySQLLib.Load('', 'libmysql-32.dll')）；
/// 本车道以 IMySqlDatabaseFactory 抽象 DB 访问，单测用内存实现，**不连真实 MySQL**。
/// SQL 语句文本逐字保留（见 TMySqlAccountDB.DoInit）。
/// </summary>
public interface IMySqlDatabaseFactory
{
    /// <summary>对应 TMySQLLib.Create(nil) + TMySQLLib.Load('', 'libmysql-32.dll')。</summary>
    IMySqlLib CreateLib();

    /// <summary>对应 TMySQLDataBase.Create(FMySqlLib)。</summary>
    IMySqlDatabase CreateDatabase(IMySqlLib lib);
}

/// <summary>接缝：TMySQLLib（libmysql 动态库句柄）。</summary>
public interface IMySqlLib
{
}

/// <summary>接缝：TMySQLDataBase。</summary>
public interface IMySqlDatabase
{
    /// <summary>TMySQLDataBase.Init。</summary>
    void Init();

    /// <summary>TMySQLDataBase.Connect（对应原文 FDB.Connect(...CLIENT_MULTI_STATEMENTS)）。</summary>
    void Connect(string server, string user, string password, string database, ushort port, uint flags);

    /// <summary>TMySQLDataBase.CharacterSetName（原文赋 'utf8'）。</summary>
    string CharacterSetName { get; set; }

    /// <summary>对应 FDB.MySQL（未连接时为 null）。</summary>
    IMySqlClient? MySql { get; }

    /// <summary>TMySQLDataBase.Statements.AddSQLStatement。</summary>
    IMySqlStatement AddSQLStatement(string name);

    void StartTransaction();
    void Commit();
    void Rollback();
    void Exec(string sql);

    /// <summary>对应 FDB.OnRequest（每次请求回调，用于刷新 FLastRequestTick）。</summary>
    event Action<object?>? OnRequest;
}

/// <summary>接缝：TMySQLCli（连接实例句柄）。</summary>
public interface IMySqlClient
{
}

/// <summary>接缝：TMySqlStatement。</summary>
public interface IMySqlStatement
{
    string Sql { get; set; }
    void Prepare();

    /// <summary>对应 Delphi TMySqlStatement.Finalize（C# Finalize 与 Object 冲突，改名）。</summary>
    void FinalizeStatement();
    void Reset();

    void BindParamText(int index, string value);

    /// <summary>对应 OrderBindParamText（按顺序绑定到下一个占位符）。</summary>
    void BindParamText(string value);

    /// <summary>对应 OrderBindParamInt（按顺序绑定到下一个占位符）。</summary>
    void BindParamInt(int value);

    bool Query();
    bool Fetch();
    bool Step();
    int FieldCount { get; }
    string GetColumnValueText(int index);
    bool GetColumnValueBool(int index);
    int GetColumnValueInt(int index);

    /// <summary>对应 OrderGetColumnValueText（取第 0 列文本）。</summary>
    string OrderGetColumnValueText { get; }

    /// <summary>对应 OrderGetColumnValueInt（取第 0 列整数）。</summary>
    int OrderGetColumnValueInt { get; }
}

/// <summary>接缝默认工厂：真实 libmysql 未移植 → 直接抛出，提示接入点。</summary>
public sealed class MySqlNativeSeamFactory : IMySqlDatabaseFactory
{
    public static readonly MySqlNativeSeamFactory Instance = new();

    public IMySqlLib CreateLib()
        => throw new NotSupportedException("接缝：MySQLWrap.pas/MySQLCli.pas 未移植（libmysql-32.dll）。请注入 IMySqlDatabaseFactory。");

    public IMySqlDatabase CreateDatabase(IMySqlLib lib)
        => throw new NotSupportedException("接缝：MySQLDataBase.pas 未移植。请注入 IMySqlDatabaseFactory。");
}

/// <summary>
/// MySqlAccountDB.pas TMySqlAccountDB 1:1 移植（源码 594 行）。
/// 仅把 TMySQLDataBase/TMySqlStatement 的原生调用换成 IMySqlDatabase/IMySqlStatement 接缝；
/// SQL 文本、分支顺序、断言文案、参数绑定顺序全部逐字保留。
/// </summary>
public sealed class TMySqlAccountDB : TAccountDB
{
    /// <summary>MySQLCli.pas CLIENT_MULTI_STATEMENTS（原文 FDB.Connect 末参）。</summary>
    public const uint CLIENT_MULTI_STATEMENTS = 65536;

    private uint FLastRequestTick;

    private readonly IMySqlLib FMySqlLib;
    private readonly IMySqlDatabase FDB;
    private readonly TDBServerConfig FDBConfig;

    private IMySqlStatement? FStatementGet;
    private IMySqlStatement? FStatementGetByPhone;
    private IMySqlStatement? FStatementGetByQuick;
    private IMySqlStatement? FStatementFind;

    private IMySqlStatement? FStatementNew;
    private IMySqlStatement? FStatementCheckExists;
    private IMySqlStatement? FStatementUpdatePartField;
    private IMySqlStatement? FStatementUpdateAllField;
    private IMySqlStatement? FStatementUnLockAccount;

    private IMySqlStatement? FStatementGetAllAccount;
    private IMySqlStatement? FStatementEnabledAccount;

    /// <summary>MySqlAccountDB.pas:69 构造函数。</summary>
    public TMySqlAccountDB(TDBServerConfig DBServerConfig, IMySqlDatabaseFactory? factory = null)
        : base("")
    {
        IMySqlDatabaseFactory f = factory ?? MySqlNativeSeamFactory.Instance;

        FDBConfig = DBServerConfig;

        FMySqlLib = f.CreateLib();                     // 原文 TMySQLLib.Create(nil) + Load('libmysql-32.dll')
        FDB = f.CreateDatabase(FMySqlLib);             // 原文 TMySQLDataBase.Create(FMySqlLib)
        FDB.Init();
        FDB.OnRequest += OnRequest;

        FStatementGet = null;
        FStatementGetByPhone = null;
        FStatementGetByQuick = null;
        FStatementFind = null;

        FStatementNew = null;
        FStatementCheckExists = null;
        FStatementUpdatePartField = null;
        FStatementUpdateAllField = null;
        FStatementUnLockAccount = null;

        FStatementGetAllAccount = null;
        FStatementEnabledAccount = null;

        FLastRequestTick = DelphiRTL.GetTickCount();
    }

    /// <summary>MySqlAccountDB.pas:106 DoInit。SQL 文本逐字保留（跨行 '+' 拼接与原文一致）。</summary>
    protected override void DoInit()
    {
        FDB.Connect(FDBConfig.DBServer, FDBConfig.DBUser, FDBConfig.DBPassword, FDBConfig.DataBase,
                    FDBConfig.DBPort, CLIENT_MULTI_STATEMENTS);
        FDB.CharacterSetName = "utf8";

        FStatementGet = FDB.AddSQLStatement("select_Account");
        FStatementGet.Sql =
            "select Account, Disable, Password, UserName, IDCard, BirthDay, " +
              "Questions1, Answers1, Questions2, Answers2, Phone, MobilePhone, Mail, " +
              "L2Password, CreateDate, LoginDate, LoginMac, LoginIP, LastActionTick, " +
              "ErrorCount, Memo " +
            "from Account where Account = ?";

        FStatementGetByPhone = FDB.AddSQLStatement("select_Account_phone");
        FStatementGetByPhone.Sql =
            "select Account, Disable, Password, UserName, IDCard, BirthDay, " +
              "Questions1, Answers1, Questions2, Answers2, Phone, MobilePhone, Mail, " +
              "L2Password, CreateDate, LoginDate, LoginMac, LoginIP, LastActionTick, " +
              "ErrorCount, Memo " +
            "from Account where MobilePhone = ?";

        FStatementGetByQuick = FDB.AddSQLStatement("select_Account_quick");
        FStatementGetByQuick.Sql =
            "select Account, Disable, Password, UserName, IDCard, BirthDay, " +
              "Questions1, Answers1, Questions2, Answers2, Phone, MobilePhone, Mail, " +
              "L2Password, CreateDate, LoginDate, LoginMac, LoginIP, LastActionTick, " +
              "ErrorCount, Memo, UID, CID " +
            "from Account where UID = ? and CID = ?";

        FStatementFind = FDB.AddSQLStatement("find_Account");
        FStatementFind.Sql =
            "select Account, Disable, Password, UserName, IDCard, BirthDay, " +
              "Questions1, Answers1, Questions2, Answers2, Phone, MobilePhone, Mail, " +
              "L2Password, CreateDate, LoginDate, LoginMac, LoginIP, LastActionTick, " +
              "ErrorCount, Memo " +
            "from Account where Account LIKE ?";

        FStatementNew = FDB.AddSQLStatement("new_Account");
        FStatementNew.Sql =
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
            "values(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

        FStatementCheckExists = FDB.AddSQLStatement("exists_Account");
        FStatementCheckExists.Sql =
            "select 1 from Account where Account = ?;";       // COLLATE NOCASE

        FStatementUpdatePartField = FDB.AddSQLStatement("update_Account_1");
        FStatementUpdatePartField.Sql =
            "update Account set " +
              "L2Password = ?, " +
              "LoginDate = ?, " +
              "LoginMac = ?, " +
              "LoginIP = ?, " +
              "LastActionTick = ?, " +
              "ErrorCount = ? " +
            "where Account = ? ";

        FStatementUpdateAllField = FDB.AddSQLStatement("update_Account_2");
        FStatementUpdateAllField.Sql =
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
            "where Account = ? ";

        FStatementUnLockAccount = FDB.AddSQLStatement("unlock_Account");
        FStatementUnLockAccount.Sql =
            "update Account set " +
              "LastActionTick = 0, " +
              "ErrorCount = 0 " +
            "where Account = ? ";

        FStatementGetAllAccount = FDB.AddSQLStatement("get_all_Account");
        FStatementGetAllAccount.Sql =
            "select Account, Disable from Account";

        FStatementEnabledAccount = FDB.AddSQLStatement("enabled_Account");
        FStatementEnabledAccount.Sql =
            "update Account set Disable = ? where Account = ? ";

        FStatementGet.Prepare();
        FStatementGetByPhone.Prepare();
        FStatementGetByQuick.Prepare();
        FStatementFind.Prepare();

        FStatementNew.Prepare();
        FStatementCheckExists.Prepare();
        FStatementUpdatePartField.Prepare();
        FStatementUpdateAllField.Prepare();
        FStatementUnLockAccount.Prepare();
        FStatementGetAllAccount.Prepare();
        FStatementEnabledAccount.Prepare();
    }

    /// <summary>MySqlAccountDB.pas:232 DoFinal。</summary>
    protected override void DoFinal()
    {
        if (FStatementGet != null) FStatementGet.FinalizeStatement();
        if (FStatementGetByPhone != null) FStatementGetByPhone.FinalizeStatement();
        if (FStatementGetByQuick != null) FStatementGetByQuick.FinalizeStatement();
        if (FStatementFind != null) FStatementFind.FinalizeStatement();

        if (FStatementNew != null) FStatementNew.FinalizeStatement();
        if (FStatementCheckExists != null) FStatementCheckExists.FinalizeStatement();
        if (FStatementUpdatePartField != null) FStatementUpdatePartField.FinalizeStatement();
        if (FStatementUpdateAllField != null) FStatementUpdateAllField.FinalizeStatement();
        if (FStatementUnLockAccount != null) FStatementUnLockAccount.FinalizeStatement();
        if (FStatementGetAllAccount != null) FStatementGetAllAccount.FinalizeStatement();
        if (FStatementEnabledAccount != null) FStatementEnabledAccount.FinalizeStatement();
    }

    /// <summary>MySqlAccountDB.pas:250 DoFindAccount（LIKE '账号%'）。</summary>
    protected override int DoFindAccount(string AccountName, TAccountList AccountList)
    {
        int Result = 0;
        if (FStatementFind == null) return Result;

        AccountList.Clear();
        try
        {
            FStatementFind.Reset();
            FStatementFind.BindParamText(0, AccountName + "%");
            if (FStatementFind.Query())
            {
                if (FStatementFind.FieldCount != 21)
                    throw new Exception("FindAccount column count error.");   // Delphi Assert 文案

                while (FStatementFind.Fetch())
                {
                    TAccountInfo AccountInfo = default;
                    AccountInfo.AccountNameStr = FStatementFind.GetColumnValueText(0);
                    AccountInfo.IsDisable = FStatementFind.GetColumnValueBool(1) ? (byte)1 : (byte)0;
                    AccountInfo.PasswordStr = FStatementFind.GetColumnValueText(2);
                    AccountInfo.UserNameStr = FStatementFind.GetColumnValueText(3);
                    AccountInfo.IDCardStr = FStatementFind.GetColumnValueText(4);
                    AccountInfo.BirthDayStr = FStatementFind.GetColumnValueText(5);
                    AccountInfo.Questions1Str = FStatementFind.GetColumnValueText(6);
                    AccountInfo.Answers1Str = FStatementFind.GetColumnValueText(7);
                    AccountInfo.Questions2Str = FStatementFind.GetColumnValueText(8);
                    AccountInfo.Answers2Str = FStatementFind.GetColumnValueText(9);
                    AccountInfo.PhoneStr = FStatementFind.GetColumnValueText(10);
                    AccountInfo.MobilePhoneStr = FStatementFind.GetColumnValueText(11);
                    AccountInfo.MailStr = FStatementFind.GetColumnValueText(12);
                    AccountInfo.L2PasswordStr = FStatementFind.GetColumnValueText(13);
                    AccountInfo.CreateDate = FStatementFind.GetColumnValueInt(14);
                    AccountInfo.LoginDate = FStatementFind.GetColumnValueInt(15);
                    AccountInfo.LoginMacStr = FStatementFind.GetColumnValueText(16);
                    AccountInfo.LoginIP = FStatementFind.GetColumnValueInt(17);
                    AccountInfo.LastActionTick = (uint)FStatementFind.GetColumnValueInt(18);
                    AccountInfo.ErrorCount = FStatementFind.GetColumnValueInt(19);
                    AccountInfo.MemoStr = FStatementFind.GetColumnValueText(20);

                    AccountList.Add(AccountInfo);
                }
            }

            Result = AccountList.Count;
        }
        finally
        {
            FStatementFind.Reset();
        }
        return Result;
    }

    /// <summary>MySqlAccountDB.pas:300 DoGetAccount。</summary>
    protected override bool DoGetAccount(string AccountName, ref TAccountInfo AccountInfo)
    {
        bool Result = false;
        if (FStatementGet == null) return Result;

        try
        {
            FStatementGet.Reset();
            FStatementGet.BindParamText(0, AccountName);
            if (FStatementGet.Query() && FStatementGet.Fetch())
            {
                if (FStatementGet.FieldCount != 21)
                    throw new Exception("GetAccount column count error.");   // Delphi Assert 文案

                Result = true;
                AccountInfo.AccountNameStr = FStatementGet.GetColumnValueText(0);
                AccountInfo.IsDisable = FStatementGet.GetColumnValueBool(1) ? (byte)1 : (byte)0;
                AccountInfo.PasswordStr = FStatementGet.GetColumnValueText(2);
                AccountInfo.UserNameStr = FStatementGet.GetColumnValueText(3);
                AccountInfo.IDCardStr = FStatementGet.GetColumnValueText(4);
                AccountInfo.BirthDayStr = FStatementGet.GetColumnValueText(5);
                AccountInfo.Questions1Str = FStatementGet.GetColumnValueText(6);
                AccountInfo.Answers1Str = FStatementGet.GetColumnValueText(7);
                AccountInfo.Questions2Str = FStatementGet.GetColumnValueText(8);
                AccountInfo.Answers2Str = FStatementGet.GetColumnValueText(9);
                AccountInfo.PhoneStr = FStatementGet.GetColumnValueText(10);
                AccountInfo.MobilePhoneStr = FStatementGet.GetColumnValueText(11);
                AccountInfo.MailStr = FStatementGet.GetColumnValueText(12);
                AccountInfo.L2PasswordStr = FStatementGet.GetColumnValueText(13);
                AccountInfo.CreateDate = FStatementGet.GetColumnValueInt(14);
                AccountInfo.LoginDate = FStatementGet.GetColumnValueInt(15);
                AccountInfo.LoginMacStr = FStatementGet.GetColumnValueText(16);
                AccountInfo.LoginIP = FStatementGet.GetColumnValueInt(17);
                AccountInfo.LastActionTick = (uint)FStatementGet.GetColumnValueInt(18);
                AccountInfo.ErrorCount = FStatementGet.GetColumnValueInt(19);
                AccountInfo.MemoStr = FStatementGet.GetColumnValueText(20);
            }
        }
        finally
        {
            FStatementGet.Reset();
        }
        return Result;
    }

    /// <summary>MySqlAccountDB.pas:341 DoGetAccountByQuick（UID + CID，列数 23）。</summary>
    protected override bool DoGetAccountByQuick(string UID, string ID, ref TAccountInfo AccountInfo)
    {
        bool Result = false;
        if (FStatementGetByQuick == null) return Result;

        try
        {
            FStatementGetByQuick.Reset();
            FStatementGetByQuick.BindParamText(0, UID);
            FStatementGetByQuick.BindParamText(1, ID);
            if (FStatementGetByQuick.Query() && FStatementGetByQuick.Fetch())
            {
                if (FStatementGetByQuick.FieldCount != 23)
                    throw new Exception("GetAccount column count error.");   // 原文断言文案如此（341-383）

                Result = true;
                AccountInfo.AccountNameStr = FStatementGetByQuick.GetColumnValueText(0);
                AccountInfo.IsDisable = FStatementGetByQuick.GetColumnValueBool(1) ? (byte)1 : (byte)0;
                AccountInfo.PasswordStr = FStatementGetByQuick.GetColumnValueText(2);
                AccountInfo.UserNameStr = FStatementGetByQuick.GetColumnValueText(3);
                AccountInfo.IDCardStr = FStatementGetByQuick.GetColumnValueText(4);
                AccountInfo.BirthDayStr = FStatementGetByQuick.GetColumnValueText(5);
                AccountInfo.Questions1Str = FStatementGetByQuick.GetColumnValueText(6);
                AccountInfo.Answers1Str = FStatementGetByQuick.GetColumnValueText(7);
                AccountInfo.Questions2Str = FStatementGetByQuick.GetColumnValueText(8);
                AccountInfo.Answers2Str = FStatementGetByQuick.GetColumnValueText(9);
                AccountInfo.PhoneStr = FStatementGetByQuick.GetColumnValueText(10);
                AccountInfo.MobilePhoneStr = FStatementGetByQuick.GetColumnValueText(11);
                AccountInfo.MailStr = FStatementGetByQuick.GetColumnValueText(12);
                AccountInfo.L2PasswordStr = FStatementGetByQuick.GetColumnValueText(13);
                AccountInfo.CreateDate = FStatementGetByQuick.GetColumnValueInt(14);
                AccountInfo.LoginDate = FStatementGetByQuick.GetColumnValueInt(15);
                AccountInfo.LoginMacStr = FStatementGetByQuick.GetColumnValueText(16);
                AccountInfo.LoginIP = FStatementGetByQuick.GetColumnValueInt(17);
                AccountInfo.LastActionTick = (uint)FStatementGetByQuick.GetColumnValueInt(18);
                AccountInfo.ErrorCount = FStatementGetByQuick.GetColumnValueInt(19);
                AccountInfo.MemoStr = FStatementGetByQuick.GetColumnValueText(20);
                AccountInfo.UidStr = FStatementGetByQuick.GetColumnValueText(21);
                AccountInfo.CidStr = FStatementGetByQuick.GetColumnValueText(22);
            }
        }
        finally
        {
            FStatementGetByQuick.Reset();
        }
        return Result;
    }

    /// <summary>MySqlAccountDB.pas:385 DoGetAccountByPhone（手机号查账号）。</summary>
    protected override bool DoGetAccountByPhone(string Phone, ref TAccountInfo AccountInfo)
    {
        bool Result = false;
        if (FStatementGetByPhone == null) return Result;

        try
        {
            FStatementGetByPhone.Reset();
            FStatementGetByPhone.BindParamText(0, Phone);
            if (FStatementGetByPhone.Query() && FStatementGetByPhone.Fetch())
            {
                if (FStatementGetByPhone.FieldCount != 21)
                    throw new Exception("GetAccount column count error.");   // Delphi Assert 文案

                Result = true;
                AccountInfo.AccountNameStr = FStatementGetByPhone.GetColumnValueText(0);
                AccountInfo.IsDisable = FStatementGetByPhone.GetColumnValueBool(1) ? (byte)1 : (byte)0;
                AccountInfo.PasswordStr = FStatementGetByPhone.GetColumnValueText(2);
                AccountInfo.UserNameStr = FStatementGetByPhone.GetColumnValueText(3);
                AccountInfo.IDCardStr = FStatementGetByPhone.GetColumnValueText(4);
                AccountInfo.BirthDayStr = FStatementGetByPhone.GetColumnValueText(5);
                AccountInfo.Questions1Str = FStatementGetByPhone.GetColumnValueText(6);
                AccountInfo.Answers1Str = FStatementGetByPhone.GetColumnValueText(7);
                AccountInfo.Questions2Str = FStatementGetByPhone.GetColumnValueText(8);
                AccountInfo.Answers2Str = FStatementGetByPhone.GetColumnValueText(9);
                AccountInfo.PhoneStr = FStatementGetByPhone.GetColumnValueText(10);
                AccountInfo.MobilePhoneStr = FStatementGetByPhone.GetColumnValueText(11);
                AccountInfo.MailStr = FStatementGetByPhone.GetColumnValueText(12);
                AccountInfo.L2PasswordStr = FStatementGetByPhone.GetColumnValueText(13);
                AccountInfo.CreateDate = FStatementGetByPhone.GetColumnValueInt(14);
                AccountInfo.LoginDate = FStatementGetByPhone.GetColumnValueInt(15);
                AccountInfo.LoginMacStr = FStatementGetByPhone.GetColumnValueText(16);
                AccountInfo.LoginIP = FStatementGetByPhone.GetColumnValueInt(17);
                AccountInfo.LastActionTick = (uint)FStatementGetByPhone.GetColumnValueInt(18);
                AccountInfo.ErrorCount = FStatementGetByPhone.GetColumnValueInt(19);
                AccountInfo.MemoStr = FStatementGetByPhone.GetColumnValueText(20);
            }
        }
        finally
        {
            FStatementGetByPhone.Reset();
        }
        return Result;
    }

    /// <summary>MySqlAccountDB.pas:426 DoUpdateAccount（ufPartField / 其它 → ufAllField 语义）。</summary>
    protected override bool DoUpdateAccount(TAccountInfo AccountInfo, TAccountUpdateField UpdateField)
    {
        bool Result = false;

        if (UpdateField == TAccountUpdateField.ufPartField)
        {
            if (FStatementUpdatePartField == null) return Result;

            try
            {
                FStatementUpdatePartField.Reset();
                FStatementUpdatePartField.BindParamText(AccountInfo.L2PasswordStr);
                FStatementUpdatePartField.BindParamInt(AccountInfo.LoginDate);
                FStatementUpdatePartField.BindParamText(AccountInfo.LoginMacStr);
                FStatementUpdatePartField.BindParamInt(AccountInfo.LoginIP);
                FStatementUpdatePartField.BindParamInt((int)AccountInfo.LastActionTick);
                FStatementUpdatePartField.BindParamInt(AccountInfo.ErrorCount);
                FStatementUpdatePartField.BindParamText(AccountInfo.AccountNameStr);
                Result = FStatementUpdatePartField.Step();
            }
            finally
            {
                FStatementUpdatePartField.Reset();
            }
        }
        else
        {
            if (FStatementUpdateAllField == null) return Result;

            try
            {
                FStatementUpdateAllField.Reset();
                FStatementUpdateAllField.BindParamText(AccountInfo.PasswordStr);
                FStatementUpdateAllField.BindParamText(AccountInfo.UserNameStr);
                FStatementUpdateAllField.BindParamText(AccountInfo.IDCardStr);
                FStatementUpdateAllField.BindParamText(AccountInfo.BirthDayStr);
                FStatementUpdateAllField.BindParamText(AccountInfo.Questions1Str);
                FStatementUpdateAllField.BindParamText(AccountInfo.Answers1Str);
                FStatementUpdateAllField.BindParamText(AccountInfo.Questions2Str);
                FStatementUpdateAllField.BindParamText(AccountInfo.Answers2Str);
                FStatementUpdateAllField.BindParamText(AccountInfo.PhoneStr);
                FStatementUpdateAllField.BindParamText(AccountInfo.MobilePhoneStr);
                FStatementUpdateAllField.BindParamText(AccountInfo.MailStr);
                FStatementUpdateAllField.BindParamText(AccountInfo.L2PasswordStr);
                FStatementUpdateAllField.BindParamText(AccountInfo.MemoStr);
                FStatementUpdateAllField.BindParamText(AccountInfo.AccountNameStr);
                Result = FStatementUpdateAllField.Step();
            }
            finally
            {
                FStatementUpdateAllField.Reset();
            }
        }
        return Result;
    }

    /// <summary>MySqlAccountDB.pas:475 DoCheckAccountExists。</summary>
    protected override bool DoCheckAccountExists(string AccountName)
    {
        bool Result = false;
        if (FStatementCheckExists == null) return Result;

        FStatementCheckExists.Reset();
        try
        {
            FStatementCheckExists.BindParamText(AccountName);
            Result = FStatementCheckExists.Query() && FStatementCheckExists.Fetch();
        }
        finally
        {
            FStatementCheckExists.Reset();
        }
        return Result;
    }

    /// <summary>MySqlAccountDB.pas:489 DoAddAccount（CreateDate = Date2MyDate(Now)）。</summary>
    protected override bool DoAddAccount(TAccountInfo AccountInfo)
    {
        bool Result = false;
        if (FStatementNew == null) return Result;

        try
        {
            FStatementNew.Reset();
            FStatementNew.BindParamText(AccountInfo.AccountNameStr);
            FStatementNew.BindParamText(AccountInfo.PasswordStr);
            FStatementNew.BindParamText(AccountInfo.UserNameStr);
            FStatementNew.BindParamText(AccountInfo.IDCardStr);
            FStatementNew.BindParamText(AccountInfo.BirthDayStr);
            FStatementNew.BindParamText(AccountInfo.Questions1Str);
            FStatementNew.BindParamText(AccountInfo.Answers1Str);
            FStatementNew.BindParamText(AccountInfo.Questions2Str);
            FStatementNew.BindParamText(AccountInfo.Answers2Str);
            FStatementNew.BindParamText(AccountInfo.PhoneStr);
            FStatementNew.BindParamText(AccountInfo.MobilePhoneStr);
            FStatementNew.BindParamText(AccountInfo.MailStr);
            FStatementNew.BindParamText(AccountInfo.L2PasswordStr);
            FStatementNew.BindParamInt(LoginSrvShare.Date2MyDate(DateTime.Now));   // CreateDate
            FStatementNew.BindParamText(AccountInfo.MemoStr);
            FStatementNew.BindParamText(AccountInfo.UidStr);
            FStatementNew.BindParamText(AccountInfo.CidStr);
            Result = FStatementNew.Step();
        }
        finally
        {
            FStatementNew.Reset();
        }
        return Result;
    }

    /// <summary>MySqlAccountDB.pas:520 DoUnLockAccount。</summary>
    protected override bool DoUnLockAccount(string AccountName)
    {
        bool Result = false;
        if (FStatementUnLockAccount == null) return Result;

        try
        {
            FStatementUnLockAccount.Reset();
            FStatementUnLockAccount.BindParamText(AccountName);
            Result = FStatementUnLockAccount.Step();
        }
        finally
        {
            FStatementUnLockAccount.Reset();
        }
        return Result;
    }

    /// <summary>
    /// MySqlAccountDB.pas:534 DoGetAllAccount。
    /// Objects[] 存 Disable 的 Integer（对应原文 TObject(OrderGetColumnValueInt)）。
    /// </summary>
    protected override void DoGetAllAccount(TStringList AccountList)
    {
        if (FStatementGetAllAccount == null) return;

        AccountList.Clear();
        try
        {
            FStatementGetAllAccount.Reset();
            if (FStatementGetAllAccount.Query())
            {
                while (FStatementGetAllAccount.Fetch())
                {
                    AccountList.AddObject(FStatementGetAllAccount.OrderGetColumnValueText,
                                          FStatementGetAllAccount.OrderGetColumnValueInt);
                }
            }
        }
        finally
        {
            FStatementGetAllAccount.Reset();
        }
    }

    /// <summary>
    /// MySqlAccountDB.pas:553 DoEnabledAccounts。
    /// ★ 原文 `OrderBindParamInt(Integer(not Enabled))`：Delphi Boolean 取反 → -1 / 0，1:1 换算。
    /// </summary>
    protected override bool DoEnabledAccounts(TStringList AccountList, bool Enabled)
    {
        bool Result = false;
        if (FStatementEnabledAccount == null) return Result;

        Result = false;
        FDB.StartTransaction();
        try
        {
            for (int I = 0; I <= AccountList.Count - 1; I++)
            {
                FStatementEnabledAccount.Reset();
                FStatementEnabledAccount.BindParamInt(Enabled ? 0 : -1);   // Integer(not Enabled)
                FStatementEnabledAccount.BindParamText(AccountList[I]);

                FStatementEnabledAccount.Step();
            }
            FDB.Commit();

            Result = true;
        }
        catch
        {
            FDB.Rollback();
        }
        return Result;
    }

    /// <summary>MySqlAccountDB.pas:580 Run（10 分钟无请求则执行 select 1 保活）。</summary>
    public override void Run()
    {
        base.Run();
        if ((FDB != null) && (FDB.MySql != null) && (unchecked(DelphiRTL.GetTickCount() - FLastRequestTick) >= 10 * 60000))
        {
            FDB.Exec("select 1");
        }
    }

    /// <summary>MySqlAccountDB.pas:589 OnRequest。</summary>
    private void OnRequest(object? Sender)
    {
        FLastRequestTick = DelphiRTL.GetTickCount();
    }
}
