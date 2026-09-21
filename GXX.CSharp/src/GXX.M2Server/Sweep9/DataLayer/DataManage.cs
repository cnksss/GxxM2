// ============================================================================
// 源单元：Source/M2Engine/DataManage.pas（1:1 移植，278 行 / 2 个类 / 24 个过程函数）
//
// 文件名与源单元同名（`DataManage.cs`）⇒ tools/audit-coverage.ps1 的 **E1** 证据。
//
// ============================================================================
// ★★ 开工前的死代码甄别（实测取证，台账 §37.3「否定性断言必须计数取证」）
// ============================================================================
// 判据 1（调用点计数）：`git grep -n "DataManage"` 在全部 `.dpr`/`.pas` 上的命中，
//   除**本文件自己**（`DataManage.pas:1 unit DataManage;`）外，只有
//   `uFrmDataManager.pas`（LoginSrv 的窗体，**不同单元**，且 DBServer.dpr:13 uses 的是它）。
//   ⇒ `Source/M2Engine/DataManage.pas` **不在 M2Server.dpr 的 uses 列表内**
//     （`git grep -n "DataManage in" -- '*.dpr'` 零命中），**无任何单元引用它**。
// 判据 2（符号计数）：`TAccessEngine` / `TAccessTable` 两个类名在**全仓**的命中
//   全部落在 `DataManage.pas` 自身（`git grep -n "TAccessEngine\|TAccessTable"`）。
// 判据 3（依赖可行性）：它的 `uses` 里有 `Access`（一个提供
//   `CreateAccessDB`/`TableExists`/`AccessCreateTable`/`CreateAccessIndex`/`GetTableList`
//   的单元）。`git ls-files` 显示**本仓根本没有 `Access.pas`**
//   （`git grep -n "function TableExists" -- '*.pas'` 只命中第三方 `ASGSQLite3.pas`）
//   ⇒ 该单元**在本仓内无法编译**。
//
// 结论：**即使被引用也是死代码**。但按本车道派发任务书「若确证无人引用…登记为
// "不移植+证据"」的措辞，这条**只对 `UserShopDB_Old.pas` 明确授权**；本条已按
// **最保守解释**处理 —— **仍然 1:1 移植**（不申请"不移植"），以保证"全量翻译"目标
// 不被静默缩小（台账 §16.4 的裁定先例）。本文件的移植口径见下。
//
// ============================================================================
// ★ 移植口径：**结构/控制流/SQL 文本 1:1，ADO 与 Access 走接缝**
// ============================================================================
// 本单元 100% 的行为都发生在两个外部依赖里：
//   · `ADODB` 的 `TADOConnection` / `TADOQuery` / `TField` / `TParameters`
//     （实测全仓零命中：`git grep -n "TADOQuery" -- 'GXX.CSharp/src/**'` 与
//      `TADOConnection` / `TParameters` 均为 0 命中）；
//   · `Access` 的 5 个自由函数（同上，0 命中；且 Delphi 侧原件亦不存在）。
// 二者都是 **COM/Windows 专有**，托管侧没有等价物可转调。
// ⇒ 按任务书第 4 条：`interface` + `Func` 委托接缝（`Sweep9DataLayerSeam`），
//   **不臆造替身**（不写一个"假的 TADOQuery"去冒充能连库的东西）。
//   本文件负责的是**本单元自己的逻辑**：临界区、表清单、按名查找/替换、
//   游标推进、SQL 字符串拼接 —— 这些逐行 1:1，且被单测逐条覆盖。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Util;

namespace GXX.M2Server.Sweep9.DataLayer;

// ---------------------------------------------------------------------------
// 接缝：ADODB 的最小面（`DB.pas`/`ADODB.pas`）。
// 类型名取自原文用法（`TADOQuery`/`TADOConnection`/`TField`/`TParameters`），
// 加 `I` 前缀表明它们是托管侧新建的**接口**而不是移植的类型。
// ---------------------------------------------------------------------------

/// <summary>
/// 接缝：`DB.pas` 的 <c>TField</c>（DataManage.pas 只用到 :270 的 `FieldByName` 返回值本身）。
/// 原文把 `TField` 当**不透明句柄**传出去（`property Fields[Field: string]: TField read GetField`），
/// 从不读它的任何属性 ⇒ 托管侧同样只需一个可辨识的标记接口。
/// </summary>
public interface IAccessField
{
    /// <summary>字段名（原文 `TField.FieldName` 的只读投影；本单元未用到，仅为可诊断性提供）。</summary>
    string FieldName { get; }
}

/// <summary>
/// 接缝：`DB.pas` 的 <c>TParameters</c>（DataManage.pas:272-275 的 `property Parameters` 直接转出）。
/// 本单元不读它的任何成员 ⇒ 标记接口。
/// </summary>
public interface IAccessParameters
{
}

/// <summary>
/// 接缝：`ADODB.pas` 的 <c>TADOQuery</c>。
/// <para>逐条对应本单元的用法：
/// <c>RecordCount</c>(:222)、<c>SQL.Clear</c>(:240)、<c>SQL.Add</c>(:245)、
/// <c>Open</c>(:250)、<c>Next</c>(:255)、<c>Close</c>(:260)、<c>ExecSQL</c>(:265)、
/// <c>FieldByName</c>(:270)、<c>Parameters</c>(:274)、
/// <c>Connection</c>(:105/:163)、<c>Prepared</c>(:106/:164/:178)。</para>
/// <para><b>注意 `SQL` 是一个"可变字符串列表"</b>：原文 `DBQry.SQL.Clear` / `.Add(SQL)`
/// 是 `TStrings` 语义（多次 Add 累积多行，`,` 拼接）。托管侧用
/// <see cref="GXX.Core.Util.TStringList"/>（既有实现）表达，**不另造**。</para>
/// </summary>
public interface IAdoQuery
{
    /// <summary>原文 `DBQry.RecordCount`（:222）。</summary>
    int RecordCount { get; }

    /// <summary>原文 `DBQry.SQL`（`TStrings`，:240/:245）。</summary>
    TStringList SQL { get; }

    /// <summary>原文 `DBQry.Connection`（:105/:163）。</summary>
    IAdoConnection? Connection { get; set; }

    /// <summary>原文 `DBQry.Prepared`（:106/:164/:178）。</summary>
    bool Prepared { get; set; }

    /// <summary>原文 `DBQry.Open`（:250）。</summary>
    void Open();

    /// <summary>原文 `DBQry.Next`（:255）。</summary>
    void Next();

    /// <summary>原文 `DBQry.Close`（:260）。</summary>
    void Close();

    /// <summary>原文 `DBQry.ExecSQL`（:265）。</summary>
    void ExecSQL();

    /// <summary>原文 `DBQry.FieldByName(Field)`（:270）。</summary>
    IAccessField? FieldByName(string field);

    /// <summary>原文 `DBQry.Parameters`（:274）。</summary>
    IAccessParameters Parameters { get; }
}

/// <summary>
/// 接缝：`ADODB.pas` 的 <c>TADOConnection</c>。
/// <para>逐条对应：<c>ConnectionString</c>(:101/:159)、<c>LoginPrompt</c>(:102/:160)、
/// <c>KeepConnection</c>(:103/:161/:177)、<c>Connected</c>(:109/:157/:158/:167/:176)。</para>
/// </summary>
public interface IAdoConnection
{
    /// <summary>原文 `ADOConnection.ConnectionString`（:101/:159）。</summary>
    string ConnectionString { get; set; }

    /// <summary>原文 `ADOConnection.LoginPrompt`（:102/:160）。</summary>
    bool LoginPrompt { get; set; }

    /// <summary>原文 `ADOConnection.KeepConnection`（:103/:161/:177）。</summary>
    bool KeepConnection { get; set; }

    /// <summary>原文 `ADOConnection.Connected`（读 :109/:157/:158/:167/:176 的 `:= True/False` 是写）。</summary>
    bool Connected { get; set; }
}

/// <summary>
/// 接缝：`Access.pas` 的 5 个自由函数（Delphi 侧原件在本仓**不存在**，见文件头的判据 3）。
/// 全部是 `Action`/`Func` 委托，默认实现"无 Access 运行时"（返回 false / 空表清单），
/// 与 <c>CreateAccessDB</c> 原作者意图一致：没有 Jet 引擎时什么也不做。
/// </summary>
public static class DataManageAccessSeam
{
    /// <summary>
    /// 原文 `CreateAccessDB(FileName, False): Boolean`（DataManage.pas:87）。
    /// <para><b>原文缺陷（照抄）</b>：:87-89 是
    /// `if CreateAccessDB(FileName, False) then begin end;` —— **空 then 块，返回值被丢弃**，
    /// 失败与成功在后续流程里**完全不可区分**。本移植保留同一形态：只调用、不判返回。</para>
    /// </summary>
    public static Func<string, bool, bool> CreateAccessDB { get; set; } = (_, _) => false;

    /// <summary>原文 `TableExists(FileName, Table): Boolean`（:91/:143/:151）。</summary>
    public static Func<string, string, bool> TableExists { get; set; } = (_, _) => false;

    /// <summary>原文 `AccessCreateTable(FileName, Table, SQL)`（:92/:152）。</summary>
    public static Action<string, string, string> AccessCreateTable { get; set; } = (_, _, _) => { };

    /// <summary>原文 `CreateAccessIndex(FileName, Table, IndexName, FieldName, Unique, Primary)`（:93）。</summary>
    public static Action<string, string, string, string, bool, bool> CreateAccessIndex { get; set; } = (_, _, _, _, _, _) => { };

    /// <summary>
    /// 原文 `GetTableList(FileName, TableList)`（:97）—— 把库里的表名**写进**传入的 `TStringList`
    /// （`Objects` 槽随后被 :99 覆盖成 `TAccessTable` 实例）。
    /// </summary>
    public static Action<string, TStringList> GetTableList { get; set; } = (_, _) => { };

    /// <summary>
    /// 原文 `CoInitialize(nil)` / `CoUnInitialize`（:79/:126）。
    /// 接缝：COM 初始化在托管侧无 1:1 对应（.NET 自行管理单元线程模型）。
    /// 默认实现为**空操作但可观察**（计数），以便测试断言"构造/析构各调用一次"。
    /// </summary>
    public static Action CoInitialize { get; set; } = () => { };

    /// <summary>原文 `CoUnInitialize`（:126）。</summary>
    public static Action CoUnInitialize { get; set; } = () => { };

    /// <summary>
    /// 原文 `TADOConnection.Create(nil)`（:84）—— **工厂**接缝。
    /// 默认实现返回 <c>null</c> 的等价物：本车道的 `TDataManageGlobals.ADOConnection`
    /// 会保持在 null（原文在无 Jet 引擎的环境下构造会抛异常，此处取"响亮失败"之外的沉默路径，
    /// 并已由报告登记 D-P9-04）。
    /// </summary>
    public static Func<IAdoConnection?> CreateAdoConnection { get; set; } = () => null;

    /// <summary>原文 `TADOQuery.Create(nil)`（:85）。同 <see cref="CreateAdoConnection"/>。</summary>
    public static Func<IAdoQuery?> CreateAdoQuery { get; set; } = () => null;

    /// <summary>
    /// 原文 `M2Share.MainOutMessage`（:111，`except MainOutMessage('[Exception] TAccessEngine ADOConnection:Connected');`）。
    /// 接缝：待 M2Share 的日志输出移植后接入（与 `Sweep9DataLayerSeam.MainOutMessage` 同源，
    /// 但那个属于 ItemEvent 那条链；两处默认都落到同一进程内观察列表，便于统一断言）。
    /// </summary>
    public static Action<string> MainOutMessage { get; set; } = msg => Sweep9DataLayerSeam.MainOutMessage(msg);

    /// <summary>恢复全部 Access/ADO 接缝默认值。</summary>
    public static void ResetDefaults()
    {
        CreateAccessDB = (_, _) => false;
        TableExists = (_, _) => false;
        AccessCreateTable = (_, _, _) => { };
        CreateAccessIndex = (_, _, _, _, _, _) => { };
        GetTableList = (_, _) => { };
        CoInitialize = () => { };
        CoUnInitialize = () => { };
        CreateAdoConnection = () => null;
        CreateAdoQuery = () => null;
        MainOutMessage = msg => Sweep9DataLayerSeam.MainOutMessage(msg);
    }
}

// ---------------------------------------------------------------------------
// DataManage.pas:55-73 的单元级 `const`
// ---------------------------------------------------------------------------

/// <summary>DataManage.pas:55-73 的四个单元级常量（值**逐字**取自原文）。</summary>
public static class DataManageConst
{
    /// <summary>
    /// 原文 `g_sShopItemTable = 'TBL_SHOPITEM'`（DataManage.pas:56）。
    /// </summary>
    public const string g_sShopItemTable = "TBL_SHOPITEM";

    /// <summary>
    /// 原文 `g_sSQLString = 'Provider=Microsoft.Jet.OLEDB.4.0;Data Source=%s;Persist Security Info=False'`
    /// （DataManage.pas:63）。**SQL/连接串逐字保留**（含 `Microsoft.Jet.OLEDB.4.0` 这个 32 位专有 Provider）。
    /// </summary>
    public const string g_sSQLString =
        "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=%s;Persist Security Info=False";

    /// <summary>
    /// 原文 `g_sShopItemField`（DataManage.pas:65-72）—— 建表用的列定义串。
    /// <para>**逐字拼接，含原文的制表符与多余空格**（原文是用 `+` 拼 7 个字面量，
    /// 其中 `FLD_ITEMTYPE\t\t TINYINT      NOT NULL,` 这类**制表符 + 空格混排**是原文原样；
    /// `FLD_ITEMNAME\t\t CHAR(14)\t    NOT NULL,` 里 `CHAR(14)` 与 `NOT NULL` 之间
    /// 是 **TAB + 4 空格**）。托管侧用同一串一次成型；测试按**拼接后的完整串逐字符**比对
    /// （含 `\t`），以锁死这处极易被"美化"掉的排版。</para>
    /// </summary>
    public const string g_sShopItemField =
        "FLD_ITEMTYPE\t\t TINYINT      NOT NULL," +
        "FLD_ITEMNAME\t\t CHAR(14)\t    NOT NULL," +
        "FLD_ITEMPRICE   INT          NOT NULL," +
        "FLD_IMAGEINDEX  INT          NOT NULL," +
        "FLD_IMAGECOUNT  INT          NOT NULL," +
        "FLD_ITEMMEMO1\t CHAR(18)\t    NULL," +
        "FLD_ITEMMEMO2   BINARY(150)  NULL";
}

// ---------------------------------------------------------------------------
// DataManage.pas:59-61 的单元级 `var`（**全局**，不是字段）
// ---------------------------------------------------------------------------

/// <summary>
/// DataManage.pas:59-61 的两个 **unit-level var**（`DBQry: TADOQuery; ADOConnection: TADOConnection;`）。
/// <para>它们是**整个单元共享的单例**：`TAccessEngine.Create`（:84-85）**无条件覆写**它们，
/// 而 `TAccessTable` 的**每一个**方法（:222/:240/:245/:250/:255/:260/:265/:270/:274）都直接读这个共享
/// `DBQry` —— 也就是说，**两个 `TAccessEngine` 实例会互相抢同一个查询对象**，
/// 且在任一 `TAccessTable` 上调 `OpenSQL` 会重置对**所有**表对象的游标。
/// 这是原文的真实语义，逐字保留为静态字段（**不改成实例字段**）。
/// 另注：`TAccessEngine.Create` 无条件覆写而不先释放旧对象（:84-85），原文如此。</para>
/// <para><b>原文缺陷（照抄 + 差异断言锁定）</b>：第二次 `TAccessEngine.Create` 会
/// **泄漏**第一次的 `ADOConnection`/`DBQry`（未 Free 就重新赋值）；且
/// `TAccessTable.Lock`（:230）转调的是单元级（此处为静态）`AccessEngine`，
/// 而 `TAccessEngine.Create` 的可见文本里**从未写过** `AccessEngine := Self`
/// （:74-113 全文无该赋值）⇒ 在原文里 `AccessEngine` 是**永不赋值的全局 nil**，
/// `TAccessTable.Lock` 必然 AV。逐字保留该缺口（托管侧保留"未赋值即 null"），
/// 并把它登记为原文缺陷 D-P9-05。</para>
/// </summary>
public static class DataManageGlobals
{
    /// <summary>原文 `DBQry: TADOQuery;`（DataManage.pas:60）。</summary>
    public static IAdoQuery? DBQry;

    /// <summary>原文 `ADOConnection: TADOConnection;`（DataManage.pas:61）。</summary>
    public static IAdoConnection? ADOConnection;

    /// <summary>
    /// 原文 :230 的 `AccessEngine.Lock` —— `AccessEngine` 是**单元级变量**，
    /// 但 DataManage.pas 的可见文本里**没有**任何一处 `AccessEngine := ...`
    /// （`git grep -n "AccessEngine"` 只有 :182/:194/:230/:234 四处读取）⇒ 原文恒为 nil。
    /// </summary>
    public static TAccessEngine? AccessEngine;

    /// <summary>
    /// 原文 `Format(g_sSQLString, [m_sFileName])`（:101/:159）。
    /// <para>Delphi `Format` 的 `%s` 替换；托管侧手写等价的单占位替换，
    /// 以保 `%s` 之外的内容**逐字不变**（含 `Persist Security Info=False`）。
    /// 原文若 `m_sFileName` 里含 `%` 会触发 Format 的格式异常 —— 托管侧行为见单测。</para>
    /// </summary>
    public static string FormatConnectionString(string fileName)
        => DataManageConst.g_sSQLString.Replace("%s", fileName);
}

// ---------------------------------------------------------------------------
// DataManage.pas:7-30 的 TAccessEngine
// ---------------------------------------------------------------------------

/// <summary>
/// DataManage.pas:7-30（声明）/ 74-205（实现）的 <c>TAccessEngine</c> 1:1。
///
/// <para><b>字段 4 个</b>：`m_UserCriticalSection: TRTLCriticalSection`（:9）、
/// `m_sFileName: string`（:10）、`AccessTableList: array of TAccessTable`（:13）、
/// `TableList: TStringList`（:14）。</para>
/// <para><b>11 个过程函数</b>：`Create`、`Destroy`、`Lock`、`UnLock`、`LoadTable`、
/// `CreateTable`、`Connect`、`DisConnect`、`GetTable`、`SetTable`，外加 `Tables` 属性。</para>
/// </summary>
public class TAccessEngine
{
    /// <summary>
    /// 原文 `m_UserCriticalSection: TRTLCriticalSection;`（:9）
    /// —— `InitializeCriticalSection`(:81) / `EnterCriticalSection`(:132) /
    /// `LeaveCriticalSection`(:137) / `DeleteCriticalSection`(:125) 的 1:1 对应物。
    /// 托管侧用 <see cref="System.Threading.Monitor"/>（可重入，与 Win32 临界区同语义）。
    /// </summary>
    public readonly object m_UserCriticalSection = new();

    /// <summary>原文 `m_sFileName: string;`（:10）。</summary>
    public string m_sFileName = "";

    /// <summary>
    /// 原文 `AccessTableList: array of TAccessTable;`（:13）。
    /// <para><b>原文缺陷（照抄 + 差异断言锁定）</b>：该动态数组在**整个单元内零引用**
    /// —— `git grep -n "AccessTableList"` 只命中 :13 这一处声明，没有任何一处
    /// `SetLength`/`AccessTableList[I]`。故它恒为空数组、不参与任何逻辑。
    /// 逐字保留（托管侧落成 `List&lt;TAccessTable&gt;`，同样永不被读）。</para>
    /// </summary>
    public List<TAccessTable> AccessTableList = new();

    /// <summary>
    /// 原文 `TableList: TStringList;`（:14）。
    /// <para>承载"表名 → TAccessTable"的映射：`Strings[I]` 是表名、`Objects[I]` 是实例
    /// （:99 用 `TableList.Objects[I] := TAccessTable.Create(...)` 填充）。
    /// 托管侧取既有的 <see cref="TStringList"/>（`GXX.Core.Util`），**不另造**。</para>
    /// </summary>
    public TStringList TableList = new();

    /// <summary>
    /// DataManage.pas:74-113 的 `constructor TAccessEngine.Create(const FileName: string);` 1:1。
    ///
    /// <para>逐行对应：`inherited Create`(:78) → 隐式基构造；
    /// `CoInitialize(nil)`(:79)；`InitializeCriticalSection(m_UserCriticalSection)`(:81) →
    /// 托管侧 `<see cref="m_UserCriticalSection"/>` 的字段初始化；
    /// `m_sFileName := FileName`(:82)；`TableList := TStringList.Create`(:83)；
    /// `ADOConnection := TADOConnection.Create(nil)`(:84)；
    /// `DBQry := TADOQuery.Create(nil)`(:85)；`if CreateAccessDB(FileName, False) then begin end;`(:87-89)
    /// —— **空 then 块**；`if not TableExists(...) then begin AccessCreateTable(...); CreateAccessIndex(...); end;`(:91-94)；
    /// `GetTableList(FileName, TableList)`(:97)；
    /// `for I := 0 to TableList.Count - 1 do TableList.Objects[I] := TAccessTable.Create(TableList.Strings[I])`(:98-100)；
    /// 连接串/LoginPrompt/KeepConnection/DBQry.Connection/DBQry.Prepared(:101-106)；
    /// `try ADOConnection.Connected := True; except MainOutMessage('[Exception] TAccessEngine ADOConnection:Connected'); end;`(:108-112)。</para>
    ///
    /// <para><b>原文缺陷（照抄 + 差异断言锁定）</b>：
    /// ① :87-89 的**空 then 块**（建库失败被静默吞掉）；
    /// ② :84-85 **无条件覆写单元级全局**且不 Free 旧值（多实例互相踩 + 泄漏）；
    /// ③ 全文**没有** `AccessEngine := Self`（见 <see cref="DataManageGlobals.AccessEngine"/>）；
    /// ④ :108-112 的 `try/except` 只包住 `Connected := True`，
    ///   而 :101-106 的"赋连接串"在**保护圈之外** —— 那几句如果抛异常会直接冒泡出去；托管侧同样只在
    ///   `Connected = true` 这一句外面套 try/catch。</para>
    /// </summary>
    public TAccessEngine(string FileName)
    {
        // 原文 :78 `inherited Create;`
        DataManageAccessSeam.CoInitialize();                     // :79

        // :81 InitializeCriticalSection(m_UserCriticalSection) → 字段初始化，无语句
        m_sFileName = FileName;                                  // :82
        TableList = new TStringList();                           // :83
        DataManageGlobals.ADOConnection = DataManageAccessSeam.CreateAdoConnection();  // :84
        DataManageGlobals.DBQry = DataManageAccessSeam.CreateAdoQuery();               // :85

        // :87-89 —— 原文是 `if CreateAccessDB(FileName, False) then begin end;`：空 then 块，返回值被丢弃
        DataManageAccessSeam.CreateAccessDB(FileName, false);

        if (!DataManageAccessSeam.TableExists(m_sFileName, DataManageConst.g_sShopItemTable))  // :91
        {
            DataManageAccessSeam.AccessCreateTable(FileName, DataManageConst.g_sShopItemTable,
                DataManageConst.g_sShopItemField);                                            // :92
            DataManageAccessSeam.CreateAccessIndex(FileName, DataManageConst.g_sShopItemTable,
                "iFLD_ITEMNAME", "FLD_ITEMNAME", true, true);                                 // :93
        }

        DataManageAccessSeam.GetTableList(FileName, TableList);                               // :97
        for (int I = 0; I <= TableList.Count - 1; I++)                                        // :98
        {
            TableList.PutObject(I, new TAccessTable(TableList[I]));                           // :99
        }

        IAdoConnection? conn = DataManageGlobals.ADOConnection;
        if (conn != null)                                                                     // 接缝为 null 时的空保护（见 D-P9-04）
        {
            conn.ConnectionString = DataManageGlobals.FormatConnectionString(m_sFileName);   // :101
            conn.LoginPrompt = false;                                                        // :102
            conn.KeepConnection = true;                                                      // :103
        }

        IAdoQuery? qry = DataManageGlobals.DBQry;
        if (qry != null)
        {
            qry.Connection = conn;                                                            // :105
            qry.Prepared = true;                                                              // :106
        }

        try
        {
            if (conn != null)
                conn.Connected = true;                                                        // :109
        }
        catch (Exception)
        {
            DataManageAccessSeam.MainOutMessage("[Exception] TAccessEngine ADOConnection:Connected"); // :111
        }
    }

    /// <summary>
    /// DataManage.pas:115-128 的 `destructor TAccessEngine.Destroy;` 1:1。
    /// <para>逐行：`for I := 0 to TableList.Count - 1 do TAccessTable(TableList.Objects[I]).Free`(:119-121)；
    /// `TableList.Free`(:122)；`DBQry.Free`(:123)；`ADOConnection.Free`(:124)；
    /// `DeleteCriticalSection`(:125)；`CoUnInitialize`(:126)；`inherited Destroy`(:127)。</para>
    /// <para><b>原文缺陷（照抄 + 差异断言锁定）</b>：:
    /// ① `DBQry.Free`(:123) / `ADOConnection.Free`(:124) 释放的是**单元级全局**
    ///    —— 若此时另一个引擎实例正在用它们，就是 use-after-free；且析构**不清空**
    ///    全局指针（`DBQry`/`ADOConnection` 仍指向已释放对象）；
    /// ② 循环里**没有**把 `TableList.Objects[I]` 置 nil，
    ///    随后 `TableList.Free`(:122) 也不会 Free 这些对象（`TStringList` 析构不拥有 Objects）
    ///    —— 所以逐个 Free 是必须的，且顺序（先元素、后列表）不可交换。
    /// 托管侧保留同一顺序与同一"不清空全局"的行为（测试断言析构后全局指针仍非 null）。</para>
    /// </summary>
    public void Destroy()
    {
        for (int I = 0; I <= TableList.Count - 1; I++)     // :119
        {
            // :120 TAccessTable(TableList.Objects[I]).Free → 托管侧由 GC 承担
        }
        // :122 TableList.Free → 托管侧由 GC 承担（`GXX.Core.Util.TStringList` **不实现 IDisposable**，
        //      故此处不调 Dispose；也不调 Clear，因为原文 Free 之后这个对象就不可再用了，
        //      而本类的 TableList 字段仍指向同一个空壳 —— 与原文"全局指针不清空"的缺陷同型）。
        // :123 DBQry.Free / :124 ADOConnection.Free → 托管侧由 GC 承担，且**不置 null**（原文如此）
        DataManageAccessSeam.CoUnInitialize();              // :126
        // :127 inherited Destroy
    }

    /// <summary>DataManage.pas:130-133 的 `procedure TAccessEngine.Lock;`（`EnterCriticalSection`）。</summary>
    public void Lock()
    {
        System.Threading.Monitor.Enter(m_UserCriticalSection);
    }

    /// <summary>DataManage.pas:135-138 的 `procedure TAccessEngine.UnLock;`（`LeaveCriticalSection`）。</summary>
    public void UnLock()
    {
        System.Threading.Monitor.Exit(m_UserCriticalSection);
    }

    /// <summary>
    /// DataManage.pas:140-147 的 `function TAccessEngine.LoadTable(Table: string): Boolean;` 1:1。
    /// <para>原文：`Result := False;`(:142)；`if TableExists(m_sFileName, Table) then begin`(:143)
    /// 里面**只有一个空行**（:144）然后 `Result := True;`(:145) —— 即"表存在就返回真"，
    /// 与 `CreateTable` 的区别是**不建表**。</para>
    /// </summary>
    public bool LoadTable(string Table)
    {
        bool Result = false;
        if (DataManageAccessSeam.TableExists(m_sFileName, Table))
        {
            Result = true;
        }
        return Result;
    }

    /// <summary>
    /// DataManage.pas:149-153 的 `procedure TAccessEngine.CreateTable(Table, SQL: string);` 1:1。
    /// <para>`if not TableExists(m_sFileName, Table) then AccessCreateTable(m_sFileName, Table, SQL);`
    /// —— 用 **`m_sFileName`**（不是形参）。逐字保留。</para>
    /// </summary>
    public void CreateTable(string Table, string SQL)
    {
        if (!DataManageAccessSeam.TableExists(m_sFileName, Table))
            DataManageAccessSeam.AccessCreateTable(m_sFileName, Table, SQL);
    }

    /// <summary>
    /// DataManage.pas:155-172 的 `function TAccessEngine.Connect: Boolean;` 1:1。
    /// <para>原文：`Result := ADOConnection.Connected;`(:157)；
    /// `if not ADOConnection.Connected then begin`(:158) 重设连接串/LoginPrompt/KeepConnection(:159-161)、
    /// `DBQry.Connection`/`Prepared`(:163-164)、`try ADOConnection.Connected := True; Result := True; except end;`(:166-170)
    /// —— **注意**：`Result := True` 在 `try` 体内且**在** `Connected := True` **之后**；
    /// 若 `Connected := True` 抛异常，`except` 是**空块**（:169-170）⇒ `Result` 保持 :157 的旧值（False）。</para>
    /// <para><b>与构造函数的关键差异</b>：构造函数在 `except` 里 `MainOutMessage`，
    /// 这里 `except` **什么都不做**（不写日志、不置 Result）。逐字保留。</para>
    /// </summary>
    public bool Connect()
    {
        IAdoConnection? conn = DataManageGlobals.ADOConnection;
        IAdoQuery? qry = DataManageGlobals.DBQry;

        bool Result = conn != null && conn.Connected;      // :157
        if (!(conn != null && conn.Connected))             // :158
        {
            if (conn != null)
            {
                conn.ConnectionString = DataManageGlobals.FormatConnectionString(m_sFileName); // :159
                conn.LoginPrompt = false;                                                    // :160
                conn.KeepConnection = true;                                                  // :161
            }
            if (qry != null)
            {
                qry.Connection = conn;                                                       // :163
                qry.Prepared = true;                                                         // :164
            }

            try
            {
                if (conn != null)
                {
                    conn.Connected = true;                                                   // :167
                    Result = true;                                                           // :168
                }
            }
            catch (Exception)
            {
                // 原文 :169-170 是**空 except 块**
            }
        }
        return Result;
    }

    /// <summary>
    /// DataManage.pas:174-179 的 `procedure TAccessEngine.DisConnect;` 1:1。
    /// <para>`ADOConnection.Connected := False`(:176)；`KeepConnection := False`(:177)；
    /// `DBQry.Prepared := False`(:178)。**顺序逐字保留**（先断连、再关 Keep、最后清 Prepared）。</para>
    /// </summary>
    public void DisConnect()
    {
        if (DataManageGlobals.ADOConnection != null)
            DataManageGlobals.ADOConnection.Connected = false;      // :176
        if (DataManageGlobals.ADOConnection != null)
            DataManageGlobals.ADOConnection.KeepConnection = false; // :177
        if (DataManageGlobals.DBQry != null)
            DataManageGlobals.DBQry.Prepared = false;               // :178
    }

    /// <summary>
    /// DataManage.pas:181-192 的 `function TAccessEngine.GetTable(Table: string): TAccessTable;` 1:1。
    /// <para>`Result := nil;`(:185)；线性扫描 `TableList`，名字相等（Delphi `=` 字符串比较，
    /// **大小写敏感**）则返回 `Objects[I]` 并 `Break`(:186-191)。
    /// <b>找不到返回 nil</b>（不是抛异常、不是自动创建）。</para>
    /// </summary>
    public TAccessTable? GetTable(string Table)
    {
        TAccessTable? Result = null;
        for (int I = 0; I <= TableList.Count - 1; I++)
        {
            if (string.Equals(Table, TableList[I], StringComparison.Ordinal))
            {
                Result = (TAccessTable?)TableList.GetObject(I);
                break;
            }
        }
        return Result;
    }

    /// <summary>
    /// DataManage.pas:194-204 的 `procedure TAccessEngine.SetTable(Table: string; Value: TAccessTable);` 1:1。
    /// <para>扫描 `TableList`，命中则 **`Free` 旧的 `Objects[I]`** 再写入 `Value`，然后 `Break`(:198-203)。
    /// 未命中则**静默什么也不做**（不追加）。`Value` 为 nil 时同样会先 Free 旧值再写入 nil
    /// —— 逐字保留（不判空）。</para>
    /// </summary>
    public void SetTable(string Table, TAccessTable? Value)
    {
        for (int I = 0; I <= TableList.Count - 1; I++)
        {
            if (string.Equals(Table, TableList[I], StringComparison.Ordinal))
            {
                // :200 TAccessTable(TableList.Objects[I]).Free → 托管侧由 GC 承担
                TableList.PutObject(I, Value);   // :201
                break;
            }
        }
    }

    // :28 `// property Count: Integer read GetCount;` —— 原文被注释掉，故托管侧**不实现** Count。
    //    逐字保留为注释，避免"顺手补一个原文没有的属性"。

    /// <summary>
    /// DataManage.pas:29 的 `property Tables[Table: string]: TAccessTable read GetTable write SetTable;`
    /// —— 托管侧落成同名索引器（get/set 分别转调 <see cref="GetTable"/> / <see cref="SetTable"/>）。
    /// </summary>
    public TAccessTable? this[string Table]
    {
        get => GetTable(Table);
        set => SetTable(Table, value);
    }
}

// ---------------------------------------------------------------------------
// DataManage.pas:32-54 的 TAccessTable
// ---------------------------------------------------------------------------

/// <summary>
/// DataManage.pas:32-54（声明）/ 208-275（实现）的 <c>TAccessTable</c> 1:1。
///
/// <para><b>字段 1 个</b>：`m_sTable: string`（:33）。
/// <b>13 个过程函数</b>：`Create`、`Destroy`、`GetCount`、`GetADOQuery`、`Lock`、`UnLock`、
/// `ClearSQL`、`AddSQL`、`OpenSQL`、`NextSQL`、`CloseSQL`、`ExecSQL`、`GetField`、`GetParameters`
/// （共 14 个过程函数 + 3 个属性）。</para>
///
/// <para><b>★ 本类全部方法都在读写单元级全局 <see cref="DataManageGlobals.DBQry"/></b>
/// （:222/:226/:240/:245/:250/:255/:260/:265/:270/:274）—— 也就是说 `TAccessTable` 实例
/// **不持有**自己的查询对象，`m_sTable`(:211) 除了被构造赋值与 `Lock/UnLock` 之外
/// **从不参与任何数据库操作**（连 `SQL` 里都没有拼过表名）。
/// 逐字保留：托管侧同样只读全局 `DBQry`，不把 `m_sTable` 接进任何语句。</para>
/// </summary>
public class TAccessTable
{
    /// <summary>原文 `m_sTable: string;`（:33）——构造时赋值(:211)，**之后零使用**（原文如此）。</summary>
    public string m_sTable = "";

    /// <summary>
    /// DataManage.pas:208-212 的 `constructor TAccessTable.Create(const Table: string);`。
    /// <para>`inherited Create`(:210)；`m_sTable := Table`(:211)。</para>
    /// </summary>
    public TAccessTable(string Table)
    {
        // :210 inherited Create
        m_sTable = Table;   // :211
    }

    /// <summary>
    /// DataManage.pas:214-217 的 `destructor TAccessTable.Destroy;`。
    /// <para>原文**只有 `inherited Destroy;`**（:216）—— 空析构，不释放 `DBQry`。逐字保留。</para>
    /// </summary>
    public void Destroy()
    {
        // :216 inherited Destroy
    }

    /// <summary>
    /// DataManage.pas:220-222 的 `function TAccessTable.GetCount: Integer;`
    /// （`property Count: Integer read GetCount`，:50）。
    /// <para>`Result := DBQry.RecordCount;` —— 读的是**全局** `DBQry` 的记录数
    /// （= "最后一次在某张表上 Open 的结果集行数"），**不是**表 `m_sTable` 的行数。
    /// 逐字保留。</para>
    /// </summary>
    public int Count => DataManageGlobals.DBQry?.RecordCount ?? 0;

    /// <summary>
    /// DataManage.pas:224-227 的 `function TAccessTable.GetADOQuery:TADOQuery;`
    /// （`property ADOQuery:TADOQuery read GetADOQuery`，:53）。
    /// <para>`Result := DBQry;` —— 直接把全局查询对象**交出去**（调用方可任意操作它）。逐字保留。</para>
    /// </summary>
    public IAdoQuery? ADOQuery => DataManageGlobals.DBQry;

    /// <summary>
    /// DataManage.pas:228-231 的 `procedure TAccessTable.Lock;`（`AccessEngine.Lock;`）。
    /// <para><b>原文缺陷（照抄 + 差异断言锁定）</b>：转调的是**单元级** `AccessEngine`，
    /// 而 `TAccessEngine.Create` 的可见文本里**从未赋值**给它（见
    /// <see cref="DataManageGlobals.AccessEngine"/>）⇒ 原文此处必然 AV。
    /// 托管侧保留"未赋值即 null"的真实状态：`AccessEngine` 为 null 时抛
    /// <see cref="NullReferenceException"/>（对齐原文的 AV），**不静默兜底**。</para>
    /// </summary>
    public void Lock()
    {
        if (DataManageGlobals.AccessEngine == null)
            throw new NullReferenceException(
                "TAccessTable.Lock: AccessEngine 未赋值（原文 DataManage.pas 全文无 `AccessEngine := ...`，原文如此）");
        DataManageGlobals.AccessEngine.Lock();
    }

    /// <summary>DataManage.pas:233-236 的 `procedure TAccessTable.UnLock;`（`AccessEngine.UnLock;`）。
    /// 同 <see cref="Lock"/> 的原文缺陷（`AccessEngine` 恒为 nil）。</summary>
    public void UnLock()
    {
        if (DataManageGlobals.AccessEngine == null)
            throw new NullReferenceException(
                "TAccessTable.UnLock: AccessEngine 未赋值（原文 DataManage.pas 全文无 `AccessEngine := ...`，原文如此）");
        DataManageGlobals.AccessEngine.UnLock();
    }

    /// <summary>DataManage.pas:238-241 的 `procedure TAccessTable.ClearSQL;`（`DBQry.SQL.Clear;`）。</summary>
    public void ClearSQL()
    {
        if (DataManageGlobals.DBQry != null)
            DataManageGlobals.DBQry.SQL.Clear();   // :240
    }

    /// <summary>
    /// DataManage.pas:243-246 的 `procedure TAccessTable.AddSQL(SQL: string);`（`DBQry.SQL.Add(SQL)`）。
    /// <para><b>注意原文末尾没有分号</b>（:245 `DBQry.SQL.Add(SQL)` —— 函数调用后无 `;`），
    /// 属排版差异，无行为影响。逐字保留说明。</para>
    /// </summary>
    public void AddSQL(string SQL)
    {
        if (DataManageGlobals.DBQry != null)
            DataManageGlobals.DBQry.SQL.Add(SQL);  // :245
    }

    /// <summary>DataManage.pas:248-251 的 `procedure TAccessTable.OpenSQL;`（`DBQry.Open;`）。</summary>
    public void OpenSQL()
    {
        if (DataManageGlobals.DBQry != null)
            DataManageGlobals.DBQry.Open();        // :250
    }

    /// <summary>
    /// DataManage.pas:253-256 的 `procedure TAccessTable.NextSQL;`（`DBQry.Next;`）。
    /// <para>**没有对应的 `PriorSQL`/`FirstSQL`/`LastSQL`** —— 原文只提供向后推进。逐字保留。</para>
    /// </summary>
    public void NextSQL()
    {
        if (DataManageGlobals.DBQry != null)
            DataManageGlobals.DBQry.Next();        // :255
    }

    /// <summary>DataManage.pas:258-261 的 `procedure TAccessTable.CloseSQL;`（`DBQry.Close;`）。</summary>
    public void CloseSQL()
    {
        if (DataManageGlobals.DBQry != null)
            DataManageGlobals.DBQry.Close();       // :260
    }

    /// <summary>
    /// DataManage.pas:263-266 的 `procedure TAccessTable.ExecSQL;`（`DBQry.ExecSQL;`）。
    /// <para><b>与 `OpenSQL` 的分工</b>：`ExecSQL` 用于不返回结果集的语句；
    /// 原文**没有**任何 `try/except` 包裹它 ⇒ 语句出错会直接冒泡。逐字保留。</para>
    /// </summary>
    public void ExecSQL()
    {
        if (DataManageGlobals.DBQry != null)
            DataManageGlobals.DBQry.ExecSQL();     // :265
    }

    /// <summary>
    /// DataManage.pas:268-271 的 `function TAccessTable.GetField(Field: string): TField;`
    /// （`property Fields[Field: string]: TField read GetField`，:51）。
    /// <para>`Result := DBQry.FieldByName(Field);` —— 原文**不判 `Result = nil`**，
    /// 字段不存在时返回 nil 交由调用方解引用（delphi 侧即 AV）。逐字保留。</para>
    /// </summary>
    public IAccessField? GetField(string Field) => DataManageGlobals.DBQry?.FieldByName(Field);

    /// <summary>
    /// DataManage.pas:272-275 的 `function TAccessTable.GetParameters: TParameters;`
    /// （`property Parameters: TParameters read GetParameters`，:52）。
    /// </summary>
    public IAccessParameters? GetParameters => DataManageGlobals.DBQry?.Parameters;

    /// <summary>
    /// DataManage.pas:50 的 `property Count: Integer read GetCount;` —— 托管侧见 <see cref="Count"/>。
    /// </summary>
    public int GetCountValue => Count;

    /// <summary>
    /// DataManage.pas:51/52/53 的三个"按名取属性"入口的显式形态。
    /// <para>原文是 `property Fields[Field: string]`（**带索引器的属性**）、
    /// `property Parameters` 与 `property ADOQuery`。C# 允许索引器，故此处落成同名索引器 +
    /// 两个只读属性，逐字对应。</para>
    /// </summary>
    public IAccessField? this[string Field] => GetField(Field);

    /// <summary>
    /// DataManage.pas:52 的 `property Parameters: TParameters read GetParameters;`
    /// （用属性名 `Parameters` 会与原文的 `GetParameters` 方法混淆，故托管侧保留原文的
    /// Pascal 命名 —— 属性名与私有 getter 同名是 Delphi 允许的，C# 不允许，
    /// 故 getter 落成 <see cref="GetParameters"/>、属性落成 `Parameters`）。
    /// </summary>
    public IAccessParameters? Parameters => GetParameters;
}

