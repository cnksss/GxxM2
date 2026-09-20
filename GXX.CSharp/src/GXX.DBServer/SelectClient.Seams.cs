using System;
using System.Text;
using GXX.Core.Protocol;
using GXX.Core.Util;

namespace GXX.DBServer;

// ============================================================================================
// 接缝：Source\DBServer\SelectClient.pas（1,248 行）中本单元**依赖但未移植**的外部面。
//
// 本文件只声明"最小可达面"，不移植任何被依赖单元的本体（转换开发文档 §2.3 / 车道纪律 2）。
// 每一处接缝都写明「原文位置 → 具名类型/函数 → 未移植原因 → 接入方式」。
//
// ★ 台账 §25.2 规程：接缝的默认实现**不得**是"静默返回中性值"。
//   本文件里所有"有语义返回值"的接缝默认一律**抛 NotSupportedException 并指名接入点**；
//   只有「原文本身就不抛、只做副作用」的成员（MainOutMessage）才允许静默。
// ============================================================================================

/// <summary>
/// Delphi **AnsiString（字节串）↔ C# string** 的显式双向映射。
///
/// 为什么需要它：SelectClient.pas 的整条收包/解析链（<c>m_sReceiveText</c>、<c>s10</c>、<c>s18</c>、
/// <c>sChrName</c>…）都是 Delphi <c>string = AnsiString</c>，即"1 个字符 = 1 个字节"。
/// 原文对它们的 <c>Length</c>/<c>S[I]</c>/<c>in TextChars</c> 全部是**按字节**语义：
///   · <c>NewChr</c>(:932) <c>Length(sChrName) &lt; MIN_CHAR_NAME_LEN(4)</c> —— 两个汉字 = **4 字节**，会**通过**；
///     若按 UTF-16 计长则是 2，会**误判为名字过短**。
///   · <c>NewChr</c>(:935-938) <c>if not (sChrName[I] in [#32..#255]) then Delete(...)</c> —— 按**字节**过滤；
///     若按 UTF-16 过滤，任何汉字（码点 &gt; 255）都会被**整字删掉**。
/// 故本车道对这两条链路统一用 **latin-1（ISO-8859-1）逐字节映射**：
///   byte b (0..255) ↔ char (char)b，Length 与索引均与 AnsiString 完全一致。
/// 需要真正的 GBK 文本时（调用 DB / FrmIDSoc 等托管侧 API）再显式转换（<see cref="AnsiTextOf"/>）。
/// </summary>
internal static class SelectClientAnsi
{
    /// <summary>Delphi AnsiString → 托管字节串（1 字节 = 1 char）。</summary>
    public static string StrOf(byte[] bytes, int offset = 0, int count = -1)
    {
        if (bytes == null) return "";
        if (count < 0) count = bytes.Length - offset;
        if (offset < 0) offset = 0;
        if (count <= 0 || offset >= bytes.Length) return "";
        if (offset + count > bytes.Length) count = bytes.Length - offset;
        return Encoding.Latin1.GetString(bytes, offset, count);
    }

    /// <summary>托管字节串 → byte[]（1 char = 1 字节；char &gt; 255 会被截断，本车道不外传这种字符）。</summary>
    public static byte[] BytesOf(string byteStr)
    {
        if (string.IsNullOrEmpty(byteStr)) return Array.Empty<byte>();
        return Encoding.Latin1.GetBytes(byteStr);
    }

    /// <summary>Delphi <c>A + B</c>（AnsiString 拼接，按字节）。</summary>
    public static byte[] Concat(params byte[][] parts)
    {
        int total = 0;
        foreach (byte[] p in parts) total += p?.Length ?? 0;
        byte[] result = new byte[total];
        int at = 0;
        foreach (byte[] p in parts)
        {
            if (p == null || p.Length == 0) continue;
            Array.Copy(p, 0, result, at, p.Length);
            at += p.Length;
        }
        return result;
    }

    /// <summary>把字节串按 **GBK** 解成托管文本（原文里"这个 AnsiString 就是 GBK 文本"的场合）。</summary>
    public static string AnsiTextOf(string byteStr) => GXX.Core.Rtl.DelphiRTL.AnsiString(BytesOf(byteStr));
}

// ============================================================================================
// 接缝：ServerClient.pas（未移植）里 TSelectClient 用到的两个基类。
//   SelectClient.pas:55  TSelectClient = class(TServerClientWinSocket)   ← DBShare.pas:19 DBSUSETHREAD = 0
//   SelectClient.pas:18  TUserInfo.Socket: TCustomWinSocket
//   SelectClient.pas:307(线程分支) ClientSocket / :516 Self.RemoteAddress / :695-697 UserInfo.Socket := Self
// ============================================================================================

/// <summary>接缝：ScktComp.pas <c>TCustomWinSocket</c>（本单元只用于"槽位是否为 nil"的判定）。</summary>
public abstract class TCustomWinSocket
{
}

/// <summary>
/// 接缝：ServerClient.pas:9-... <c>TServerClientWinSocket</c>（未移植）。
/// 只声明本单元实际触达的两个成员：<c>SendText</c>(原文 AnsiString) 与 <c>RemoteAddress</c>。
/// </summary>
public abstract class TServerClientWinSocket : TCustomWinSocket
{
    /// <summary>
    /// 原文 <c>procedure SendText(const sMsg: string)</c>（ScktComp，参数是承载协议字节的 AnsiString）。
    /// 托管侧按转换开发文档 §3.1「协议层一律 byte[]」以 <see cref="byte"/>[] 表达，
    /// 使单测能逐字节锁定**组包结果**（这是本单元唯一可无头验证的出口）。
    /// </summary>
    public abstract void SendText(byte[] sMsg);

    /// <summary>原文 <c>property RemoteAddress: string</c>（SelectClient.pas:516）。</summary>
    public abstract string RemoteAddress { get; }
}

// ============================================================================================
// 接缝：IDSocCli.pas 的 TFrmIDSoc（未移植）。
//   原文全局：DBShare.pas `FrmIDSoc: TFrmIDSoc`（IDSocCli.pas 的窗体实例）。
//   本单元的调用点（**全部**列出，§28.3：src 与 tests 都搜过）：
//     SelectClient.pas:714  FrmIDSoc.GetGlobaSessionStatus(nSessionID): Boolean
//     SelectClient.pas:716  FrmIDSoc.SendSocketMsg(SS_SOFTOUTSESSION, sAccount + '/' + IntToStr(nSessionID))
//     SelectClient.pas:717  FrmIDSoc.CloseSession(sAccount, nSessionID)
//     SelectClient.pas:760  FrmIDSoc.CheckSession(sAccount, sUserIPaddr, nSessionID): Boolean
//     SelectClient.pas:784  FrmIDSoc.CheckSession(...)
//     SelectClient.pas:807  FrmIDSoc.CheckSession(...)
//     SelectClient.pas:829  FrmIDSoc.CheckSession(...)
//     SelectClient.pas:1146 FrmIDSoc.SetGlobaSessionPlay(nSessionID)
//     SelectClient.pas:1171 FrmIDSoc.SetGlobaSessionPlay(nSessionID)
//     SelectClient.pas:1198 FrmIDSoc.CheckSession(...)
//     SelectClient.pas:1200 FrmIDSoc.SetGlobaSessionNoPlay(nSessionID)
// ============================================================================================

/// <summary>接缝：IDSocCli.pas:36-60 <c>TFrmIDSoc</c> 的最小面（成员名沿用原文）。</summary>
public interface ITFrmIDSoc
{
    /// <summary>IDSocCli.pas:40 `function CheckSession(sAccount, sIPaddr: string; nSessionID: Integer): Boolean;`</summary>
    bool CheckSession(string sAccount, string sIPaddr, int nSessionID);

    /// <summary>IDSocCli.pas:45 `procedure SetGlobaSessionNoPlay(nSessionID: Integer);`</summary>
    void SetGlobaSessionNoPlay(int nSessionID);

    /// <summary>IDSocCli.pas:46 `procedure SetGlobaSessionPlay(nSessionID: Integer);`</summary>
    void SetGlobaSessionPlay(int nSessionID);

    /// <summary>IDSocCli.pas:47 `function GetGlobaSessionStatus(nSessionID: Integer): Boolean;`</summary>
    bool GetGlobaSessionStatus(int nSessionID);

    /// <summary>IDSocCli.pas:39 `procedure SendSocketMsg(wIdent: Word; sMsg: string);`</summary>
    void SendSocketMsg(ushort wIdent, string sMsg);

    /// <summary>IDSocCli.pas（CloseSession(sAccount, nSessionID)）。</summary>
    void CloseSession(string sAccount, int nSessionID);
}

/// <summary>
/// 接缝宿主：原文 unit 级变量 <c>FrmIDSoc</c>。
/// **默认 nil**（不接线）⇒ 任何一次访问都抛 <see cref="NotSupportedException"/>，
/// 绝不静默返回 false/true（§25.2：否则会把"字段语义错"伪装成"分支没命中"）。
/// </summary>
public static class IDSocCliSeam
{
    /// <summary>DBShare.pas 的 <c>FrmIDSoc: TFrmIDSoc</c>。宿主用 <see cref="TDBServerHost"/> 注入。</summary>
    public static ITFrmIDSoc? FrmIDSoc;

    /// <summary>取 <c>FrmIDSoc</c>；未接线直接抛（指名接入点）。</summary>
    public static ITFrmIDSoc Require
        => FrmIDSoc ?? throw new NotSupportedException(
            "接缝：IDSocCli.pas 的 TFrmIDSoc 未接线（FrmIDSoc = nil）。接入点：DBServerService 需实现 ITFrmIDSoc 并赋给 IDSocCliSeam.FrmIDSoc。");

    public static void Reset() => FrmIDSoc = null;
}

// ============================================================================================
// 接缝：RoleDB.pas / DBShare.pas:115 `g_RoleDB: TRoleDB`（未接线）。
//   本单元的调用点：
//     SelectClient.pas:964  g_RoleDB.HumanDB.GetID / :964 g_RoleDB.HeroDB.GetID
//     SelectClient.pas:970  g_RoleDB.HumanDB.GetHumanCount
//     SelectClient.pas:973  g_RoleDB.HumanDB.Add
//     SelectClient.pas:1018 g_RoleDB.HumanDB.QueryDeleteHumans
//     SelectClient.pas:1059 g_RoleDB.HumanDB.GetHumanCount
//     SelectClient.pas:1062 g_RoleDB.HumanDB.DeleteRestore
//     SelectClient.pas:1095 g_RoleDB.HumanDB.GetBaseInfo
//     SelectClient.pas:1101 g_RoleDB.HumanDB.Delete
//     SelectClient.pas:1132 g_RoleDB.HumanDB.Select
//     SelectClient.pas:1210 g_RoleDB.HumanDB.QueryHumans
//   托管侧已有 THumanDBBase / THeroDBBase 真实现（MySqlRoleDB.*.cs），只缺 unit 级全局实例。
//   ⇒ 这里只放两个注入点，**不新建 DB 实现**（那是 p2-dbserver-mysql 车道的产物）。
// ============================================================================================

/// <summary>接缝：<c>g_RoleDB.HumanDB</c> / <c>g_RoleDB.HeroDB</c>（RoleDB 全局实例注入点）。</summary>
public static class SelectClientRoleDbSeam
{
    /// <summary>DBShare.pas:115 `g_RoleDB.HumanDB`（原文 <c>TRoleDB.HumanDB: THumanDB</c>）。</summary>
    public static THumanDBBase? HumanDB;

    /// <summary>DBShare.pas:115 `g_RoleDB.HeroDB`。</summary>
    public static THeroDBBase? HeroDB;

    public static THumanDBBase RequireHuman
        => HumanDB ?? throw new NotSupportedException(
            "接缝：g_RoleDB.HumanDB 未接线（RoleDB.pas / DBShare.pas:115）。接入点：宿主把 RoleDatabase/TMySqlRoleDB.HumanDB 赋给 SelectClientRoleDbSeam.HumanDB。");

    public static THeroDBBase RequireHero
        => HeroDB ?? throw new NotSupportedException(
            "接缝：g_RoleDB.HeroDB 未接线（RoleDB.pas / DBShare.pas:115）。接入点：宿主把 TMySqlRoleDB.HeroDB 赋给 SelectClientRoleDbSeam.HeroDB。");

    public static void Reset()
    {
        HumanDB = null;
        HeroDB = null;
    }
}

// ============================================================================================
// DBShare.pas 的 unit 级全局 / 常量中，DBShareSeam.cs（车道4 产物，**只读**）尚未收录、
// 而本单元用到的部分。声明放本车道自己的文件里，类型与初值照抄原文。
// ============================================================================================

/// <summary>DBShare.pas 中本单元用到、而 <see cref="DBShareSeam"/> 未收录的全局。</summary>
public static class SelectClientGlobals
{
    /// <summary>DBShare.pas:152 `g_boDynamicIPMode: Boolean = False;`（:1142/:1155 读）。</summary>
    public static byte g_boDynamicIPMode = 0;

    /// <summary>DBShare.pas:174 `g_boShowQuryChrLog: Boolean = False;`（:1203 读）。</summary>
    public static byte g_boShowQuryChrLog = 0;

    /// <summary>DBShare.pas:203 `g_nCreateHumCount: Integer = 0;`（:976 Inc）。</summary>
    public static int g_nCreateHumCount = 0;

    /// <summary>DBShare.pas:244 `g_FirstName: TStringList;`（:895-896 RandomName）。</summary>
    public static TStringList g_FirstName = new TStringList();

    /// <summary>DBShare.pas:245 `g_LastName: TStringList;`（:897-898 RandomName）。</summary>
    public static TStringList g_LastName = new TStringList();

    /// <summary>复位为 DBShare.pas 的声明初值（单测用）。</summary>
    public static void Reset()
    {
        g_boDynamicIPMode = 0;
        g_boShowQuryChrLog = 0;
        g_nCreateHumCount = 0;
        g_FirstName = new TStringList();
        g_LastName = new TStringList();
    }
}

/// <summary>
/// 接缝：DBShare.pas 的人物名校验族与主动网关路由族（**均未移植**）。
///
/// ★ 参数表示（关键）：<c>CheckChrName</c>/<c>CheckSpecialChar</c>/<c>CheckDenyChrName</c>/
///   <c>CheckNumberName</c>/<c>CheckLetterName</c>/<c>CheckFilterNewHumanChrName</c> 在原文里接收的是
///   Delphi <c>AnsiString</c>，函数体按**字节**判定（DBShare.pas:1204-1249 <c>Chr := sChrName[I]</c>
///   与 <c>#$81..#$FE</c> 比较，即 GBK 首字节区间）。
///   ⇒ 托管侧接缝一律传 **latin-1 字节串**（见 <see cref="SelectClientAnsi"/>），
///     将来 DBShare.pas 正式移植时可直接逐字节照抄函数体。
///   <c>CheckSpecialChar(sChrName: WideString)</c>（DBShare.pas:1251）是**唯一**收 WideString 的一个，
///     按原文 AnsiString→WideString 的隐式转换，传 **GBK 文本**。
/// </summary>
public static class SelectClientDbShareSeam
{
    /// <summary>DBShare.pas:16 `TextChars = [#32..#255];`（AnsiChar 集合；:937 `sChrName[I] in TextChars`）。</summary>
    public const byte TextCharsFirst = 32;
    /// <summary>DBShare.pas:16 的上界 255。</summary>
    public const byte TextCharsLast = 255;

    /// <summary>DBShare.pas:21 `MIN_CHAR_NAME_LEN = 4;`（:932）。</summary>
    public const int MIN_CHAR_NAME_LEN = 4;

    /// <summary>DBShare.pas:22 `MAX_CHAR_NAME_LEN = 14;`（:957）。</summary>
    public const int MAX_CHAR_NAME_LEN = 14;

    /// <summary>DBShare.pas:1204 `function CheckChrName(sChrName: string): Boolean;`（入参：字节串）。</summary>
    public static Func<string, bool> CheckChrName =
        _ => throw new NotSupportedException("接缝：DBShare.pas:1204-1249 CheckChrName 未移植。");

    /// <summary>DBShare.pas:1251 `function CheckSpecialChar(sChrName: WideString): Boolean;`（入参：GBK 文本）。</summary>
    public static Func<string, bool> CheckSpecialChar =
        _ => throw new NotSupportedException("接缝：DBShare.pas:1251-1280 CheckSpecialChar 未移植。");

    /// <summary>DBShare.pas:1043 `function CheckDenyChrName(sChrName: string): Boolean;`（入参：字节串）。</summary>
    public static Func<string, bool> CheckDenyChrName =
        _ => throw new NotSupportedException("接缝：DBShare.pas:1043-1056 CheckDenyChrName 未移植。");

    /// <summary>DBShare.pas:1103 `function CheckNumberName(sChrName: string): Boolean;`（入参：字节串）。</summary>
    public static Func<string, bool> CheckNumberName =
        _ => throw new NotSupportedException("接缝：DBShare.pas:1103-1122 CheckNumberName 未移植。");

    /// <summary>DBShare.pas:1124 `function CheckLetterName(sChrName: string): Boolean;`（入参：字节串）。</summary>
    public static Func<string, bool> CheckLetterName =
        _ => throw new NotSupportedException("接缝：DBShare.pas:1124-1150 CheckLetterName 未移植。");

    /// <summary>DBShare.pas:1058 `function CheckFilterNewHumanChrName(sChrName: string): Boolean;`（入参：字节串）。</summary>
    public static Func<string, bool> CheckFilterNewHumanChrName =
        _ => throw new NotSupportedException("接缝：DBShare.pas:1058-1077 CheckFilterNewHumanChrName 未移植。");

    /// <summary>
    /// DBShare.pas:751-870 `function GateActiveRouteIP(sGateIP: string; var nPort: Integer): string;`
    /// （未移植：约 120 行，依赖 g_RouteInfo.RunGate2List/TRunGateInfo，属 DBShare 车道）。
    /// </summary>
    public delegate string TGateActiveRouteIP(string sGateIP, out int nPort);

    /// <summary>上文委托的注入点（:1152 `sRouteIP := GateActiveRouteIP(m_sGateAddr, nRoutePort)`）。</summary>
    public static TGateActiveRouteIP GateActiveRouteIP =
        (string _, out int nPort) => throw new NotSupportedException("接缝：DBShare.pas:751-870 GateActiveRouteIP 未移植。");

    /// <summary>DBShare.pas:731-749 `function CheckActiveRunGate(sGateIP: string; nPort: Integer): Boolean;`（:1158）。</summary>
    public static Func<string, int, bool> CheckActiveRunGate =
        (_, __) => throw new NotSupportedException("接缝：DBShare.pas:731-749 CheckActiveRunGate 未移植。");

    /// <summary>把所有可注入项复位（单测用）。</summary>
    public static void Reset()
    {
        CheckChrName = _ => throw new NotSupportedException("接缝：DBShare.pas:1204-1249 CheckChrName 未移植。");
        CheckSpecialChar = _ => throw new NotSupportedException("接缝：DBShare.pas:1251-1280 CheckSpecialChar 未移植。");
        CheckDenyChrName = _ => throw new NotSupportedException("接缝：DBShare.pas:1043-1056 CheckDenyChrName 未移植。");
        CheckNumberName = _ => throw new NotSupportedException("接缝：DBShare.pas:1103-1122 CheckNumberName 未移植。");
        CheckLetterName = _ => throw new NotSupportedException("接缝：DBShare.pas:1124-1150 CheckLetterName 未移植。");
        CheckFilterNewHumanChrName = _ => throw new NotSupportedException("接缝：DBShare.pas:1058-1077 CheckFilterNewHumanChrName 未移植。");
        GateActiveRouteIP = (string _, out int nPort) => throw new NotSupportedException("接缝：DBShare.pas:751-870 GateActiveRouteIP 未移植。");
        CheckActiveRunGate = (_, __) => throw new NotSupportedException("接缝：DBShare.pas:731-749 CheckActiveRunGate 未移植。");
    }
}

// ============================================================================================
// 接缝：Delphi System.pas `Random`（随机源必须可注入 —— 车道任务要点 3）
// ============================================================================================

/// <summary>
/// Delphi <c>System.Random</c> / <c>Random(Range: Integer): Integer</c>。
///
/// 原文（System.pas）语义：
/// <code>
/// function Random(Range: Integer): Integer;
/// begin
///   if Range = 0 then Result := 0
///   else Result := Trunc(Random * Range);   // Random: Real ∈ [0,1)
/// end;
/// </code>
/// ★ 注意 <c>Range &lt; 0</c> 时原文**不抛异常**（<c>Trunc(负数小数) = 0</c>）；
///   本仓既有的 <see cref="DelphiRandom"/>（DBServer 车道4 产物，只读）走 <c>.NET Random.Next(-1)</c>
///   会抛 <see cref="ArgumentOutOfRangeException"/> —— 语义不同，故本单元自带一份**逐字复刻**，
///   而不是转调 <see cref="DelphiRandom"/>。
/// <c>RandomName</c>(SelectClient.pas:895/897) 正是 <c>Random(g_FirstName.Count - 1)</c>：
///   名单为空时 <c>Count-1 = -1</c> ⇒ 原文取索引 0 ⇒ 在空 TStringList 上**抛 EStringListError**。
/// </summary>
public static class SelectClientRandom
{
    private static readonly System.Random Rng = new System.Random();

    /// <summary>注入点：Delphi <c>System.Random: Real</c>（[0,1)）。默认 .NET <c>NextDouble</c>。</summary>
    public static Func<double> NextDouble = () => Rng.NextDouble();

    /// <summary>Delphi `Random(Range: Integer): Integer`。</summary>
    public static int Random(int Range)
        => Range == 0 ? 0 : (int)Math.Truncate(NextDouble() * Range);

    /// <summary>复位为真随机源（单测用）。</summary>
    public static void Reset() => NextDouble = () => Rng.NextDouble();
}

// ============================================================================================
// 接缝：DBShare.pas:98-100 的模块表（AddModule/RemoveModule/UpdateModule）与 uFrmMain 的宿主面。
// ============================================================================================

/// <summary>
/// 接缝：DBShare.pas:41-47 <c>TModuleInfo</c> + :98-100 <c>AddModule/RemoveModule/UpdateModule</c>（未移植）。
/// 本单元只有一个触达点：SelectClient.pas:380-381
/// <code>if m_Module &lt;&gt; nil then pTModuleInfo(m_Module).Buffer := Format('%d/%d', [min, max]);</code>
/// 原文里 <c>m_Module</c> 由 uFrmMain.pas:330 赋值 —— 而那段**已被注释掉**（uFrmMain.pas:321-332），
/// 且 uFrmMain.pas 的 SelectSocketGetSocket(:305) 走 DBSUSETHREAD=0 分支时**根本不赋值** m_Module，
/// ⇒ 在本配置下该分支**恒不执行**（m_Module 恒为 nil）。
/// 默认实现仍按 §25.2 抛异常：一旦宿主真的接上 m_Module 却没接这个委托，必须立刻可见。
/// </summary>
public static class SelectClientModuleSeam
{
    /// <summary>原文 <c>pTModuleInfo(m_Module).Buffer := sBuffer</c>。</summary>
    public static Action<IntPtr, string> UpdateModuleBuffer =
        (_, __) => throw new NotSupportedException(
            "接缝：DBShare.pas:41-47/98-100 TModuleInfo/AddModule/UpdateModule 未移植（SelectClient.pas:381）。");

    public static void Reset() => UpdateModuleBuffer =
        (_, __) => throw new NotSupportedException(
            "接缝：DBShare.pas:41-47/98-100 TModuleInfo/AddModule/UpdateModule 未移植（SelectClient.pas:381）。");
}
