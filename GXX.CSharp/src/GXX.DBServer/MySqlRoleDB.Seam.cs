using System;
using System.Collections.Generic;

namespace GXX.DBServer;

// ============================================================================================
// 接缝：MySQLDataBase.pas / MySQLWrap.pas / MySQLCli.pas / libmysql-32.dll **未移植**。
//
// 原文 MySqlRoleDB.pas 直接调用原生 libmysql：
//     FMySqlLib := TMySQLLib.Create(nil);
//     FMySqlLib.Load('', 'libmysql-32.dll');
//     FDB := TMySQLDataBase.Create(FMySqlLib);
//     FDB.Init; FDB.OnRequest := OnRequest;
//     FDB.Connect(g_sDataSaveDBServer, ..., CLIENT_MULTI_STATEMENTS);
//     FDB.CharacterSetName := 'utf8';
//     Stms := FDB.Statements;  FStatement := Stms.AddSQLStatement('Name');  FStatement.Prepare;
//
// 本车道把「DB 访问」全部收敛到下面这几个接口后面（**数据库访问绝不进单测**），
// 单测注入内存实现；SQL 语句文本、参数绑定顺序、分支与返回码逐字保留。
//
// 命名刻意与 GXX.LoginSrv 的 IMySqlStatement（车道6）区分：
//   · 车道6 的接缝只覆盖 AccountDB 需要的 8 个成员；
//   · 本车道需要 OrderBindParamBool / OrderBindParamDouble / OrderGetColumnValueDouble 等，
//     若复用同名接口就必须改他人文件（违反铁律1），故独立一套。
//   两个项目互不引用，类型不冲突。
// ============================================================================================

/// <summary>MySQLCli.pas：CLIENT_MULTI_STATEMENTS（原文 FDB.Connect 末参）。</summary>
public static class MySqlClientFlags
{
    public const uint CLIENT_MULTI_STATEMENTS = 65536;
}

/// <summary>接缝：TMySQLLib（libmysql 动态库句柄）。</summary>
public interface IRoleMySqlLib
{
}

/// <summary>接缝：TMySQLCli（连接实例句柄，原文 FDB.MySQL）。</summary>
public interface IRoleMySqlClient
{
}

/// <summary>
/// 接缝：TMySqlStatement。
/// 成员名与 Delphi 原文一一对应（OrderBindParamBool / GetColumnValueDouble 等按原文补齐）；
/// 因 C# 的 <c>object.Finalize</c> 冲突，Delphi 的 <c>Finalize</c> 改名为 <see cref="FinalizeStatement"/>。
/// </summary>
public interface IRoleMySqlStatement
{
    /// <summary>FStatement.Sql。</summary>
    string Sql { get; set; }

    /// <summary>FStatement.Prepare。</summary>
    void Prepare();

    /// <summary>FStatement.Finalize（C# 改名）。</summary>
    void FinalizeStatement();

    /// <summary>FStatement.Reset。</summary>
    void Reset();

    /// <summary>FStatement.OrderBindParamText（顺序绑到下一个占位符）。</summary>
    void OrderBindParamText(string value);

    /// <summary>FStatement.OrderBindParamInt（顺序绑到下一个占位符）。</summary>
    void OrderBindParamInt(int value);

    /// <summary>FStatement.OrderBindParamBool（顺序绑到下一个占位符）。</summary>
    void OrderBindParamBool(bool value);

    /// <summary>FStatement.OrderBindParamDouble（顺序绑到下一个占位符）。</summary>
    void OrderBindParamDouble(double value);

    /// <summary>FStatement.Query。</summary>
    bool Query();

    /// <summary>FStatement.Fetch。</summary>
    bool Fetch();

    /// <summary>FStatement.Step（写语句执行；返回是否成功）。</summary>
    bool Step();

    /// <summary>FStatement.GetColumnValueText(Index)。</summary>
    string GetColumnValueText(int index);

    /// <summary>FStatement.GetColumnValueInt(Index)。</summary>
    int GetColumnValueInt(int index);

    /// <summary>FStatement.OrderGetColumnValueText（取当前行第 0 列文本），同时把列游标前移一格。</summary>
    string OrderGetColumnValueText { get; }

    /// <summary>FStatement.OrderGetColumnValueInt（取当前行第 0 列整数），同时把列游标前移一格。</summary>
    int OrderGetColumnValueInt { get; }

    /// <summary>FStatement.OrderGetColumnValueBool（取当前行第 0 列布尔），同时把列游标前移一格。</summary>
    bool OrderGetColumnValueBool { get; }

    /// <summary>FStatement.OrderGetColumnValueDouble（取当前行第 0 列浮点），同时把列游标前移一格。</summary>
    double OrderGetColumnValueDouble { get; }
}

/// <summary>接缝：TMySQLDataBase。</summary>
public interface IRoleMySqlDatabase
{
    /// <summary>TMySQLDataBase.Init。</summary>
    void Init();

    /// <summary>TMySQLDataBase.Connect（原文末参 CLIENT_MULTI_STATEMENTS）。</summary>
    void Connect(string server, string user, string password, string database, int port, uint flags);

    /// <summary>TMySQLDataBase.CharacterSetName（原文赋 'utf8'）。</summary>
    string CharacterSetName { get; set; }

    /// <summary>对应 FDB.MySQL（未连接时为 null）。原文 MySqlRoleDB.Run 用它判空。</summary>
    IRoleMySqlClient? MySql { get; }

    /// <summary>TMySQLDataBase.Statements.AddSQLStatement。</summary>
    IRoleMySqlStatement AddSQLStatement(string name);

    /// <summary>TMySQLDataBase.Statements.Clear。</summary>
    void ClearStatements();

    /// <summary>TMySQLDataBase.Disconnect（原文 DoFainal 调用）。</summary>
    void Disconnect();

    void StartTransaction();
    void Commit();
    void RollBack();
    void Exec(string sql);

    /// <summary>对应 FDB.OnRequest（每次请求回调，用于刷新 FLastRequestTick）。</summary>
    event Action<object?>? OnRequest;
}

/// <summary>
/// 接缝工厂：真实 libmysql 未移植。
/// </summary>
public interface IRoleMySqlDatabaseFactory
{
    /// <summary>对应 TMySQLLib.Create(nil) + TMySQLLib.Load('', 'libmysql-32.dll')。</summary>
    IRoleMySqlLib CreateLib();

    /// <summary>对应 TMySQLDataBase.Create(FMySqlLib)。</summary>
    IRoleMySqlDatabase CreateDatabase(IRoleMySqlLib lib);
}

/// <summary>默认工厂：接缝未接入 → 直接抛出并指明接入点（与 MySqlAccountDB 的做法一致）。</summary>
public sealed class RoleMySqlNativeSeamFactory : IRoleMySqlDatabaseFactory
{
    public static readonly RoleMySqlNativeSeamFactory Instance = new();

    public IRoleMySqlLib CreateLib()
        => throw new NotSupportedException("接缝：MySQLWrap.pas / MySQLCli.pas 未移植（libmysql-32.dll）。请注入 IRoleMySqlDatabaseFactory。");

    public IRoleMySqlDatabase CreateDatabase(IRoleMySqlLib lib)
        => throw new NotSupportedException("接缝：MySQLDataBase.pas 未移植。请注入 IRoleMySqlDatabaseFactory。");
}

/// <summary>MySqlRoleDB.pas 用到的 RoleDB.pas 单元级常量。</summary>
public static class RoleDbConst
{
    /// <summary>RoleDB.pas:9 `NO_ID = -1`。</summary>
    public const int NO_ID = -1;
}

/// <summary>
/// 接缝：DBShare.pas:87 `procedure MainOutMessage(sMsg: string)`（未移植）。
/// RoleDB.pas 的每个公开包装都在 `except on E: Exception do MainOutMessage(E.Message)` 里吞异常，
/// 单测用 <see cref="Sink"/> 记录被吞掉的异常消息。
/// </summary>
public static class RoleDbSeam
{
    /// <summary>默认哨兵（未接入时静默，与原文"只写主消息队列、不重抛"的语义一致）。</summary>
    public static Action<string> MainOutMessage = _ => { };

    /// <summary>Delphi Rtl `SameText`（Windows/Classes.pas：忽略大小写比较，用当前 ANSI 区域）。</summary>
    public static bool SameText(string a, string b)
        => string.Equals(a ?? "", b ?? "", StringComparison.OrdinalIgnoreCase);

    /// <summary>Delphi `Format('%d', [V])`（Integer）。</summary>
    public static string FormatInt(int v) => v.ToString(System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>Delphi `IntToStr`。</summary>
    public static string IntToStr(int v) => v.ToString(System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>Grobal2.pas `CUSTOM_MAGIC_START_ID = 1000`（原文 MySqlRoleDB.pas:1522/2573 直接用）。</summary>
    public const int CUSTOM_MAGIC_START_ID = 1000;
}

/// <summary>
/// 接缝：DBShare.pas 里的 MySQL 连接参数全局（原文 MySqlRoleDB.pas:6050 `FDB.Connect(...)` 直接引用）。
/// DBShareSeam 是既有文件（车道4 产物，本车道不改），故这几个变量放在本车道自己的文件里，
/// 类型/命名与 DBShare.pas 一致（<c>g_sDataSaveDBServer</c> 等）。
/// </summary>
public static class MySqlRoleDbGlobals
{
    /// <summary>DBShare.pas 的 g_sDataSaveDBServer。</summary>
    public static string g_sDataSaveDBServer = "127.0.0.1";

    /// <summary>DBShare.pas 的 g_sDataSaveDBUser。</summary>
    public static string g_sDataSaveDBUser = "root";

    /// <summary>DBShare.pas 的 g_sDataSaveDBPassword。</summary>
    public static string g_sDataSaveDBPassword = "";

    /// <summary>DBShare.pas 的 g_sDataSaveDataBase。</summary>
    public static string g_sDataSaveDataBase = "Mir2";

    /// <summary>DBShare.pas 的 g_wDataSaveDBPort（Word）。</summary>
    public static ushort g_wDataSaveDBPort = 3306;

    /// <summary>复位为 DBShare.pas 的声明初值。</summary>
    public static void Reset()
    {
        g_sDataSaveDBServer = "127.0.0.1";
        g_sDataSaveDBUser = "root";
        g_sDataSaveDBPassword = "";
        g_sDataSaveDataBase = "Mir2";
        g_wDataSaveDBPort = 3306;
    }
}

/// <summary>
/// RoleDB.pas:18-27 `TQueryHumanData`（record；由 TQueryHumanList 持有指针）。
/// 托管侧用 class，保持"以引用交给列表、随后被就地改写"的原文语义。
/// </summary>
public sealed class TQueryHumanData
{
    public string HumanName = "";
    public bool IsSelect;
    public int Sex;
    public int Job;
    public int Hair;
    public int Level;
}

/// <summary>
/// RoleDB.pas:68-82 `TQueryHumanList`（TList 包装 + Add/GetItems/Clear/SetCapacity）。
/// 注意原文 `GetItems` 越界返回 nil，且 `Clear` 会 Dispose 全部元素。
/// </summary>
public sealed class TQueryHumanList
{
    private readonly List<TQueryHumanData> FList = new List<TQueryHumanData>();

    public int Count => FList.Count;

    /// <summary>RoleDB.pas:389-395 `GetItems`：越界返回 nil。</summary>
    public TQueryHumanData? Items(int Index)
        => (Index >= 0 && Index < FList.Count) ? FList[Index] : null;

    /// <summary>RoleDB.pas:366-371 `Add`：返回加入的指针。</summary>
    public TQueryHumanData Add(TQueryHumanData QueryHumanData)
    {
        FList.Add(QueryHumanData);
        return QueryHumanData;
    }

    /// <summary>RoleDB.pas:373-382 `Clear`：逐个 Dispose 后清空。</summary>
    public void Clear() => FList.Clear();

    /// <summary>RoleDB.pas:397-401 `SetCapacity`。</summary>
    public void SetCapacity(int Value) => FList.Capacity = Value;
}

/// <summary>
/// RoleDB.pas:29-38 `TSerarchRoleData`（record）。
/// 字段与 GXX.LoginSrv.RoleDBSeam.TSerarchRoleData 同名同型（不同命名空间，互补冲突）。
/// </summary>
public sealed class TSerarchRoleData
{
    public string Account = "";
    public string RoleName = "";
    public int IsDelete;
    public bool IsHero;
    public int Sex;
    public int Job;
    public uint Level;
}

/// <summary>RoleDB.pas:84-98 `TSerarchRoleList`（TList 包装）。</summary>
public sealed class TSerarchRoleList
{
    private readonly List<TSerarchRoleData> FList = new List<TSerarchRoleData>();

    public int Count => FList.Count;

    public List<TSerarchRoleData> RawItems => FList;

    /// <summary>RoleDB.pas:440-446 `GetItems`：越界返回 nil。</summary>
    public TSerarchRoleData? Items(int Index)
        => (Index >= 0 && Index < FList.Count) ? FList[Index] : null;

    /// <summary>RoleDB.pas:417-422 `Add`。</summary>
    public TSerarchRoleData Add(TSerarchRoleData SerarchRoleData)
    {
        FList.Add(SerarchRoleData);
        return SerarchRoleData;
    }

    /// <summary>RoleDB.pas:424-433 `Clear`。</summary>
    public void Clear() => FList.Clear();

    /// <summary>RoleDB.pas:448-452 `SetCapacity`。</summary>
    public void SetCapacity(int Value) => FList.Capacity = Value;
}

/// <summary>RoleDB.pas:24 `TSearchMatchType`（未移植单元的最小接缝）。</summary>
public enum TSearchMatchType
{
    smtComplete = 0,
    smtFuzzy = 1,
}
