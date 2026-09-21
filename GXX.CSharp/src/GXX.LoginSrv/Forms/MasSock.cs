// ============================================================================
// MasSock.pas（1017 行）→ Forms/MasSock.cs
// 单元：LoginSrv/MasSock.pas；DFM：Source/LoginSrv/MasSock.dfm（**二进制 DFM**，567 字节）
//
// ── DFM 对账（Objects=2 / Events=6）────────────────────────────────────────
// 二进制 DFM 结构（实测）：`FF 0A 00` + ShortString('TFRMMASSOC') + ... + 流头 `TPF0`，
// 20 + 547 = 567 = 文件长度（自洽）。流内文本（Latin1 解码）：
//   object TFrmMasSoc: TFrmMasSoc        name = FrmMasSoc（根节点）
//        Left=780 Top=172 Caption='FrmMasSoc' ClientHeight=107 ClientWidth=137
//        Color=clBtnFace Font.Charset=DEFAULT_CHARSET Font.Color=clWindowText
//        Font.Height=-11 Font.Name='MS Sans Serif' Font.Style=[]
//        OldCreateOrder=False PixelsPerInch=96 TextHeight=13
//        OnCreate = FormCreate         ← 绑定 1
//        OnDestroy = FormDestroy       ← 绑定 2
//     object MSocket: TServerSocket
//        Active=False Address='0.0.0.0' Port=0 ServerType=stNonBlocking Left=40 Top=32
//        OnClientConnect    = MSocketClientConnect     ← 绑定 3
//        OnClientDisconnect = MSocketClientDisconnect  ← 绑定 4
//        OnClientRead       = MSocketClientRead        ← 绑定 5
//        OnClientError      = MSocketClientError       ← 绑定 6
//   ⇒ 2 objects（1 窗体 + 1 组件）/ 6 event bindings。负数断言一律计数取证（台账 §37.3）。
//
// ── {$IFDEF LOG_SESSION}（原文 67-130 / 770-793 / 1009-1015）不移植 ──────────
// 取证（对 D:\chuanqi\daima\GXX原版_Delphi7\Source 全树 *.pas/*.dpr/*.inc 计数）：
//   LOG_SESSION 共出现 14 次，全部是 `{$IFDEF LOG_SESSION}` 条件编译指令或
//   `{.$DEFINE LOG_SESSION}`（**被大括号注释掉的 define**）；全树 **0 处**生效的
//   `{$DEFINE LOG_SESSION}` ⇒ LOG_SESSION 未定义 ⇒ `TSessionOperate` / `g_LockLogSession`
//   / `LogSession` 与 initialization/finalization 段**均不参与编译**，故不移植。
//   （另一处出现在 M2Engine/Forms/IdSrvClient.pas，同样是 `{ .$DEFINE LOG_SESSION }` 注释态。）
//
// ── 命名空间（D-P10-15 同源理由，本文件继续沿用）────────────────────────────
// `GXX.LoginSrv` 里已有 `TMsgServerInfo`（LoginSrvShare.cs:287，**4 字段的不完整接缝**）
// 与 `GXX.LoginSrv` 根的其它同名风险类型。C# 简单名解析**先查外层命名空间再查 using**，
// 若把本单元放进 `GXX.LoginSrv` 就会 CS0101 / 静默绑错类型 ⇒ 本窗体族统一放
// `GXX.LoginSrv.Forms`（与 GrobalSession.cs / p9-m2-forms 车道一致）。
//
// ── 接缝总表（哪些复用、哪些必须自建，均有据可查）───────────────────────────
// 复用（GXX.Core / GXX.LoginSrv 既有真源）：
//   HUtil32.GetValidStr3 / TagCount       Common/HUtil32.pas 的既有托管真源
//   DelphiRTL.Trim/Copy/Pos/IntToStr/StrToIntDef/GetTickCount/Format
//   EDcode.GetDecodeSize/DecodeString、EncodingInit.GBK、TAccountInfo/TAccountInfo2
//   CommonConst.SS_*（SS_SOFTOUTSESSION/SERVERINFO/PASSWORDSUCCESS/OPENSESSION/
//                     GetAccountInfo(Ret)/ChangeAccountInfo(Ret)/UNKNOWMSG/KEEPALIVE）
//   LoginSrvShare.g_Config / g_DisablePasswordList / g_AccountDB / MainOutMessage
//   LoginSrvForms.ShowMessage（= Application.MessageBox 接缝；可注入，不阻塞测试）
//   TAccountDB.GetAccount/UpdateAccount + TAccountUpdateField.ufAllField
// 自建接缝（原文依赖的 JSocket / LSShare / LMain 面在托管侧**不存在**）：
//   TServerSocket / TCustomWinSocket / TErrorEvent / TServerType   ← JSocket（三方，未移植）
//   MasSockGlobals.g_ServerAddr / g_ServerAddrCount / nOnlineCountMin / nOnlineCountMax
//                                           ← LSShare.pas:339/340/370/371（未移植）
//   MasSockGlobals.GetSessionID()           ← LSShare.pas:496-504（未移植；**未接线即抛**）
//   MasSockGlobals.CloseUser()              ← LMain.pas:854-884（未移植；**未接线即抛**）
//   MasSockGlobals.ServerAddrFileName / UserLimitFileName ← 原文硬编码 `.\xxx.txt`
//
// ── ★ 照抄的原文缺陷（逐条有断言锁死，见 P10MasSockTests.cs）────────────────
//  F1  :628 `MsgServer.sReceiveMsg := sReviceMsg;` 写在 `if MsgServer.Socket = Socket` **之外**
//      ⇒ 每个未命中的服务器条目也被写入同一份 sReviceMsg（首次未命中时是编译器零初始化的 ''）
//      ⇒ 无关服务器的半包缓冲被清空/串包。
//  F2  `MSocketClientRead` 的两处 `Exit`（:383 / :622）退出**整个过程**（不是循环）
//      ⇒ 后面的帧被丢弃，且 :628 的回写被跳过。
//  F3  `LoadServerAddr` :820 用**行号 I（0-based）当 1-based 字符下标** `sLineText[I]` 判 `;` 注释
//      ⇒ 只有第 1 行（I=1）的注释判定是"对"的；其余行的注释判定是错位的。
//  F4  `LoadServerAddr` :830 `g_ServerAddrCount := nServerIdx;` 写在 **for 体内**
//      ⇒ (a) 0 行时完全不赋值（保留上次的值，而数组已被 FillChar 清空 —— 悬垂计数）；
//         (b) 满 100 行 `break` 时计数停在 99（少 1）。
//  F5  `LoadUserLimit` :939-943 写 `UserLimit[nC]` **无上界检查**（LoadServerAddr 有 100 保护）
//      ⇒ 原文写越界内存；托管侧表现为 IndexOutOfRangeException。
//  F6  `LoadUserLimit` 不清空 `UserLimit` 数组 ⇒ 第二次装载条目变少时残留旧条目仍被
//      LimitName/IsNotUserFull/ServerStatus 扫描到（这三处扫的是整个 0..99，不看 nUserLimit）。
//  F7  `LoadUserLimit` 文件缺失时只 ShowMessage，**不重置 nUserLimit / UserLimit**。
//  F8  `FormDestroy` `m_ServerList.Free` 后**未置 nil**（悬垂指针）。
//  F9  `StartService` / `RefServerLimit` / `SortServerList` / `LimitName` / `ServerStatus` /
//      `SendServerMsg` / `GetOnlineHumCount` 全是**空 except 吞异常**（只留一行日志）。
//      `SendServerMsgA` 稍好：`on e: Exception` 会把 E.Message 也记一行。
//  F10 `ServerStatus` :979-999 的 4 级判定里 `div 2` / `div 5` 全用整数截断；
//      且 `nLimitCountMax = 0` 时第一条 `Min <= 0` 恒真 ⇒ 恒返回 1（空闲）。
//  F11 `LoadUserLimit` 的 MobilePhone 数字校验**标志取反**（:541-556 `boTemp := False` +
//      "非数字"置 True）且错误码与上面 CheckStringValid 失败**重复用 -13**。
//  F12 SS_ChangeAccountInfo 里帐号名合法性校验整块被 `{...}` 注释掉（:389-398）
//      ⇒ 修改注册信息时帐号名不过 CheckAccountValid/长度/字符校验。
//  F13 `m_ServerList.Items[I]` 的硬转型（:781/:884 `pTMsgServerInfo(...)`）与 nil Socket
//      解引用（:782/:885）无保护 ⇒ 一个坏条目让整轮循环中断。
//  F14 `SendServerMsg` :784 的名字过滤是 `CompareText`（**忽略大小写**），而
//      `RefServerLimit`/`IsNotUserFull`/`ServerStatus`/`ServerLimit` 的名字比较用 `=`（区分大小写）
//      —— 同一单元两套比较语义并存（原文如此，不统一）。
// ============================================================================

using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.LoginSrv.Forms;

// ---------------------------------------------------------------------------
// MasSock.pas:9-25 记录类型（Delphi 以 pXxx 指针 + New/Dispose 存取；
// 托管侧用引用类型（class）承载 ⇒ New/Dispose 退化为 new/GC，字段与语义 1:1）
// ---------------------------------------------------------------------------

/// <summary>
/// MasSock.pas:9-17 <c>TMsgServerInfo</c>（7 字段 1:1）。
/// 原文以 <c>pTMsgServerInfo</c>（指针）在 <c>TList</c> 里存取；托管侧 TList → List&lt;T&gt;，
/// 条目是引用类型（与指针等价：SortServerList 的"取出→删除→插回"依赖**同一实例**）。
/// </summary>
public sealed class TMsgServerInfo
{
    public string sReceiveMsg = "";                 // :10
    public TCustomWinSocket Socket;                 // :11（原文 TCustomWinSocket，裸指针）
    public string sServerName = "";                 // :12  //0x08
    public int nServerIndex;                        // :13  //0x0C
    public int nOnlineCount;                        // :14  //0x10
    public uint dwKeepAliveTick;                     // :15  //0x14
    public string sIPaddr = "";                     // :16
}

/// <summary>MasSock.pas:19-24 <c>TLimitServerUserInfo</c>（4 字段 1:1）。</summary>
public sealed class TLimitServerUserInfo
{
    public string sServerName = "";                 // :20
    public string sName = "";                       // :21
    public int nLimitCountMin;                      // :22
    public int nLimitCountMax;                      // :23
}

// ---------------------------------------------------------------------------
// 单元级全局（MasSock.pas:56-59）+ 本单元依赖但托管侧尚未落地的 LSShare/LMain 面
// ---------------------------------------------------------------------------

/// <summary>
/// MasSock.pas 单元级 var 段（:56-59）与它依赖的 LSShare/LMain 全局/函数的托管宿主。
/// 项目约定：窗体单元的全局变量集中到静态宿主类，并提供 <see cref="ResetForTests"/>。
/// </summary>
public static class MasSockGlobals
{
    // ---------------- MasSock.pas:56-59 单元级全局 ----------------

    /// <summary>MasSock.pas:57 <c>FrmMasSoc: TFrmMasSoc</c>（VCL 由 Application.CreateForm 装配；托管侧由宿主/测试注入）。</summary>
    public static TFrmMasSoc FrmMasSoc;

    /// <summary>MasSock.pas:58 <c>nUserLimit: Integer</c>（全局初值 0）。</summary>
    public static int nUserLimit;

    /// <summary>MasSock.pas:59 <c>UserLimit: array[0..99] of TLimitServerUserInfo</c>（Delphi 静态数组零初始化）。</summary>
    public static TLimitServerUserInfo[] UserLimit = NewUserLimit();

    /// <summary>建一个 Delphi 零初始化的 <c>array[0..99]</c>（每格都是有效对象，字符串空、整数 0）。</summary>
    public static TLimitServerUserInfo[] NewUserLimit()
    {
        var a = new TLimitServerUserInfo[100];
        for (int i = 0; i < a.Length; i++) a[i] = new TLimitServerUserInfo();
        return a;
    }

    // ---------------- 接缝：LSShare.pas 的全局（LoginSrvShare.cs 里**没有**；本区不得改动它） ----------------

    /// <summary>
    /// 接缝：LSShare.pas:370 <c>g_ServerAddr: array[0..99] of string[15]</c>。
    /// 默认值 = 原文初值（**空串**，Delphi 静态 ShortString 数组零初始化后读出即 ''，不是 nil），
    /// 不是"中性占位"。写入方是 <see cref="TFrmMasSoc.LoadServerAddr"/>（本单元自己）。
    /// </summary>
    public static string[] g_ServerAddr = NewServerAddr();

    /// <summary>Delphi <c>array[0..99] of string[15]</c> 的零初始化（100 个空串）。</summary>
    public static string[] NewServerAddr()
    {
        var a = new string[100];
        for (int i = 0; i < a.Length; i++) a[i] = "";
        return a;
    }

    /// <summary>接缝：LSShare.pas:371 <c>g_ServerAddrCount: Integer = 0</c>（typed constant 初值 0）。</summary>
    public static int g_ServerAddrCount = 0;

    /// <summary>接缝：LSShare.pas:339 <c>nOnlineCountMin: Integer</c>（LMain 的状态栏/心跳用；初值 0）。</summary>
    public static int nOnlineCountMin = 0;

    /// <summary>接缝：LSShare.pas:340 <c>nOnlineCountMax: Integer</c>（初值 0）。</summary>
    public static int nOnlineCountMax = 0;

    // ---------------- 接缝：原文硬编码的文件名（默认值即原文字面量） ----------------

    /// <summary>接缝（**D-P10-19**）：MasSock.pas:810 <c>sFileName := '.\!ServerAddr.txt'</c>。默认值即原文字面量；测试可指向临时目录。</summary>
    public static string ServerAddrFileName = @".\!ServerAddr.txt";

    /// <summary>接缝（**D-P10-19**）：MasSock.pas:926 <c>sFileName := '.\!UserLimit.txt'</c>。默认值即原文字面量；测试可指向临时目录。</summary>
    public static string UserLimitFileName = @".\!UserLimit.txt";

    // ---------------- 接缝：LSShare.pas / LMain.pas 的函数（未接线即显式抛，台账 §25.2） ----------------

    /// <summary>测试/宿主注入：LSShare.pas:496 <c>GetSessionID(): Integer</c>。</summary>
    public static Func<int> GetSessionIDHandler;

    /// <summary>测试/宿主注入：LMain.pas:854 <c>CloseUser(sServerName, sAccount, nSessionID)</c>。</summary>
    public static Action<string, string, int> CloseUserHandler;

    /// <summary>
    /// LSShare.pas:496-504 <c>GetSessionID</c> 的接缝访问器。
    /// 未接线**显式抛**（不静默返回 0：0 会被当成合法会话号发出去）。
    /// </summary>
    public static int GetSessionID()
    {
        if (GetSessionIDHandler == null)
            throw new InvalidOperationException(
                "未接线：LSShare.pas:496 GetSessionID() 尚未接入（LSShare 整单元未移植，见报告 B-P10-16）。");
        return GetSessionIDHandler();
    }

    /// <summary>
    /// LMain.pas:854-884 <c>CloseUser</c> 的接缝访问器。
    /// 未接线**显式抛**（不静默忽略：忽略会让"顶号"静默失效）。
    /// </summary>
    public static void CloseUser(string sServerName, string sAccount, int nSessionID)
    {
        if (CloseUserHandler == null)
            throw new InvalidOperationException(
                "未接线：LMain.pas:854 CloseUser() 尚未接入（LMain 整单元未移植，见报告 B-P10-16）。");
        CloseUserHandler(sServerName, sAccount, nSessionID);
    }

    /// <summary>测试/宿主复位：把单元级全局恢复到 Delphi 的初值状态。</summary>
    public static void ResetForTests()
    {
        FrmMasSoc = null;
        nUserLimit = 0;                       // Delphi Integer 全局初值
        UserLimit = NewUserLimit();            // Delphi 静态数组零初始化
        g_ServerAddr = NewServerAddr();        // LSShare.pas:370（100 个空串）
        g_ServerAddrCount = 0;                 // LSShare.pas:371 = 0
        nOnlineCountMin = 0;
        nOnlineCountMax = 0;
        ServerAddrFileName = @".\!ServerAddr.txt";
        UserLimitFileName = @".\!UserLimit.txt";
        GetSessionIDHandler = null;
        CloseUserHandler = null;
    }
}

// ---------------------------------------------------------------------------
// MasSock.pas 实现段里的单元级函数（:217-266）
// ---------------------------------------------------------------------------

/// <summary>MasSock.pas 实现段的独立函数（非 TFrmMasSoc 成员）。</summary>
public static class MasSockFns
{
    /// <summary>
    /// MasSock.pas:217-232 <c>function CheckAccountValid(Account: WideString): Boolean</c>
    /// —— 只允许 <c>0-9 a-z A-Z</c>（Delphi 的字符集合区间成员判定）。
    /// </summary>
    public static bool CheckAccountValid(string Account)
    {
        bool Result = true;
        for (int I = 1; I <= Account.Length; I++)
        {
            char WC = Account[I - 1];
            if (!((WC >= '0' && WC <= '9') || (WC >= 'a' && WC <= 'z') || (WC >= 'A' && WC <= 'Z')))
            {
                Result = false;
                break;                                  // 原：Exit
            }
        }
        return Result;
    }

    /// <summary>
    /// MasSock.pas:234-249 <c>function CheckStringValid(S: WideString): Boolean</c>
    /// —— 禁止 <c>/ @ $ &lt; &gt;</c>。
    /// </summary>
    public static bool CheckStringValid(string S)
    {
        bool Result = true;
        for (int I = 1; I <= S.Length; I++)
        {
            char WC = S[I - 1];
            if (WC == '/' || WC == '@' || WC == '$' || WC == '<' || WC == '>')
            {
                Result = false;
                break;                                  // 原：Exit
            }
        }
        return Result;
    }

    /// <summary>
    /// MasSock.pas:251-266 <c>function CheckStringValid2(S: WideString): Boolean</c>
    /// —— 禁止 <c>/ $ &lt; &gt;</c>（**比 CheckStringValid 少了 '@'**，原文如此：
    /// 邮箱字段用这一版，所以邮箱里的 '@' 是允许的）。
    /// </summary>
    public static bool CheckStringValid2(string S)
    {
        bool Result = true;
        for (int I = 1; I <= S.Length; I++)
        {
            char WC = S[I - 1];
            if (WC == '/' || WC == '$' || WC == '<' || WC == '>')
            {
                Result = false;
                break;                                  // 原：Exit
            }
        }
        return Result;
    }

    /// <summary>
    /// Delphi <c>SysUtils.CompareText</c>（忽略大小写，返回 &lt;0/0/&gt;0）。
    /// 托管代偿（**D-P10-20**）：<c>StringComparison.OrdinalIgnoreCase</c> —— 工程内既有唯一处置
    /// （GXX.M2Server/MonGenLoadCore.AnsiCompareText、GXX.RunGate/GateShareContainers.cs:499
    /// 的"GXX.Core 未提供 SameText"同一处置）。非 ASCII 段与 Delphi 的 ANSI UpCase 表
    /// 可能有差异（原文服务器名多为 ASCII 串），登记为代偿差异。
    /// </summary>
    public static int CompareText(string a, string b)
        => string.Compare(a, b, StringComparison.OrdinalIgnoreCase);

    /// <summary>Delphi <c>SysUtils.SameText(a, b)</c> = <c>CompareText(a, b) = 0</c>。</summary>
    public static bool SameText(string a, string b) => CompareText(a, b) == 0;

    /// <summary>
    /// Delphi <b>AnsiString</b> 的 1-based 单字符读取 <c>S[Index]</c> 的等价语义。
    /// <para>
    /// 需要它的原因：MasSock.pas:820 把**行号 <c>I</c>（0-based）当 1-based 字符下标**用
    /// （<c>sLineText[I]</c>，★原文缺陷），而 Delph 的越界/0 号下标读取**不抛异常**：
    /// AnsiString 的 <c>S[0]</c> 落在长度字段的高字节（长度 &lt; 256 时恒为 #0），
    /// 越界读取通常拿到串尾 #0 或残留字节。C# 的 <c>s[0]</c> 是**首字符**且越界会抛
    /// ⇒ 不显式复刻就会得到不同分支。此处按 Delphi 语义复刻为：越界/0 号 → <c>'\0'</c>（**D-P10-22**）。
    /// </para>
    /// </summary>
    public static char AnsiStringCharAt(string s, int index)
        => (index >= 1 && index <= s.Length) ? s[index - 1] : '\0';

    /// <summary>Delphi <c>string[15]</c> 赋值截断（MasSock.pas:824 <c>g_ServerAddr[nServerIdx] := sLineText</c>）。</summary>
    public static string TruncateToShortString15(string value)
        => value != null && value.Length > 15 ? value.Substring(0, 15) : (value ?? "");

    // ---- 原 `MasSockFns.ArrestStringExAnsi` 的逐字复刻已删除（B-P10-17 关闭）----
    //
    // 历史（D-P10-17）：本单元 `:302` 调用 `HUtil32.ArrestStringEx_Ansi`，而托管
    // `GXX.Core.Util.HUtil32.ArrestStringEx` 当时有**两处**与 `HUtil32.pas:1761-1805` 不同的语义
    // （`Result` 初值写成 `""`；"找不到 SearchEnd"分支把剩余整串塞进 `ArrestStr`），
    // 而 `MSocketClientRead` 正是把返回值回写进 `sReviceMsg` 的**半包累加器** ⇒ 语义差异可观测。
    // 集成方已按原文修 Core（台账 §48.2：`Result := Source` 起手 + 删掉 else 改写）并合入本车道，
    // ⇒ 本地复刻无必要，**直接转调 `GXX.Core.Util.HUtil32.ArrestStringEx_Ansi`**（调用点见 `MSocketClientRead`）。

    /// <summary>
    /// Delphi <c>SizeOf(TAccountInfo2)</c>。
    /// <para>
    /// Common/Grobal2.pas:4674-4688 的 <c>TAccountInfo2</c> 是 **packed record**：
    /// 8(PlayObject) + 8(Npc) + (15+11+21+11+21+13+21+13+14+41+21) = **218**。
    /// 托管类型 <c>GXX.Core.Protocol.TAccountInfo2</c>（Grobal2.Types6.cs:176-202）的字段顺序与
    /// 容量与之逐字段相同，**实测 <c>sizeof(TAccountInfo2) = 218</c> 且各字段偏移完全重合**
    /// （取证：P10MasSockTests.<c>AccountInfo2_...</c> 断言 sizeof=218，
    ///   以及 SS_ChangeAccountInfo 的全部字段往返用例）。
    /// 线格式必须用 218（M2 侧 IdSrvClient.pas:326
    /// <c>EncodeBuffer(PAnsiChar(AccountInfo), SizeOf(TAccountInfo2))</c> 发 218 字节 ⇒
    /// 编码后 291 字符，<c>GetDecodeSize(291) = 218</c>）。
    /// 用常量而非 <c>sizeof</c> 只是为了让"线格式 = 218"在源码里**显式可见**，并避免在这个热路径里引入 unsafe
    /// （**D-P10-18**；取证用例：P10MasSockTests.<c>AccountInfo2_ManagedSizeEqualsDelphiPackedSize</c>）。
    /// </para>
    /// </summary>
    public const int TAccountInfo2PackedSize = 218;
}

// ---------------------------------------------------------------------------
// 接缝：JSocket（三方单元，仓库内无源码）的 TServerSocket / TCustomWinSocket 最小面
// ---------------------------------------------------------------------------

/// <summary>
/// JSocket 的 <c>TServerType</c>（ScktComp 兼容枚举）。
/// MasSock.dfm 里写的是 <c>ServerType = stNonBlocking</c>，本单元**从不读取**该属性
/// （只由 DFM 赋值）⇒ 枚举成员面不是行为承重项。
/// </summary>
public enum TServerType
{
    stNonBlocking,
    stThreadBlocking,
}

/// <summary>
/// JSocket 的 <c>TErrorEvent</c>（ScktComp 兼容枚举）。
/// <para>
/// ⚠ JSocket.pas 是三方单元，仓库 <c>Source/</c> 下**没有**它（实测 grep 全树无匹配），
/// 故成员表取自同形 API ScktComp 的 <c>TErrorEvent</c>。本单元对它的唯一动作是
/// <c>MSocketClientError</c> 把 ErrorCode 置 0 后 Close —— **从不检查 ErrorEvent**
/// ⇒ 成员表不影响行为，登记为代偿。
/// </para>
/// </summary>
public enum TErrorEvent
{
    eeGeneral,
    eeSend,
    eeReceive,
    eeConnect,
    eeDisconnect,
    eeAccept,
}

/// <summary>
/// JSocket 事件参数（对应 <c>TSocketEvent</c> / <c>TSocketErrorEvent</c>）。
/// 字段而非属性：<c>MSocketClientError</c> 的形参是 <c>var ErrorCode</c>，
/// 绑定处需要 <c>ref e.ErrorCode</c>（属性不能 ref）。
/// </summary>
public sealed class TClientSocketEventArgs : EventArgs
{
    public readonly TCustomWinSocket Socket;
    public readonly TErrorEvent ErrorEvent;
    public int ErrorCode;

    public TClientSocketEventArgs(TCustomWinSocket socket, TErrorEvent errorEvent = TErrorEvent.eeGeneral)
    {
        Socket = socket;
        ErrorEvent = errorEvent;
    }
}

/// <summary>
/// 接缝：JSocket <c>TCustomWinSocket</c> 的最小面 —— 只含 MasSock.pas 真正用到的成员：
/// <c>RemoteAddress</c>(:163/:344)、<c>ReceiveText</c>(:299)、<c>SendText(string)</c>(:885)、
/// <c>SendText(string, Boolean)</c>(:786)、<c>Connected</c>(:782/:885)、<c>Close</c>(:185/:213)。
/// <c>MsgServer.Socket = Socket</c>（:199/:297）是**指针相等**⇒ 托管侧用引用相等（class 的 ==）。
/// </summary>
public class TCustomWinSocket
{
    /// <summary>JSocket <c>TCustomWinSocket.RemoteAddress</c>。</summary>
    public string RemoteAddress = "0.0.0.0";

    /// <summary>JSocket <c>TCustomWinSocket.Connected</c>。</summary>
    public bool Connected;

    /// <summary>JSocket <c>TCustomWinSocket.ReceiveText</c>（本次待处理文本；读取即取走）。</summary>
    public string ReceiveText = "";

    /// <summary>接缝取证用：所有 <c>SendText</c> 的文本，按调用顺序。</summary>
    public readonly List<string> SentTexts = new();

    /// <summary>接缝取证用：<c>SendText(s, True)</c>（第二参 bSecure 为真）的文本。</summary>
    public readonly List<string> SentTextsSecure = new();

    /// <summary>接缝取证用：<c>Close</c> 是否被调用过。</summary>
    public bool CloseCalled;

    /// <summary>JSocket <c>SendText(const AText: string)</c>（不带加密标志的重载）。</summary>
    public virtual void SendText(string s)
    {
        SentTexts.Add(s);
    }

    /// <summary>JSocket <c>SendText(const AText: string; ASecure: Boolean)</c>（原文 :786 传 True）。</summary>
    public virtual void SendText(string s, bool bSecure)
    {
        SentTexts.Add(s);
        if (bSecure) SentTextsSecure.Add(s);
    }

    /// <summary>JSocket <c>Close</c>。</summary>
    public virtual void Close()
    {
        CloseCalled = true;
        Connected = false;
    }
}

/// <summary>
/// 接缝：JSocket <c>TServerSocket</c>（DFM <c>MSocket</c>）。
/// <para>
/// 派生自 <c>System.Windows.Forms.Control</c> 的理由（D-P10-16）：Delphi 的 TServerSocket 是
/// **非可视组件**（TComponent），而本车道 DFM 对账工具 <c>P10FormReconcile</c> 只遍历
/// <c>Control.Controls</c> 控件树（台账 §37.3/§41.3 规定的唯一计数口径）⇒ 若做成普通
/// Component，DFM 的 2 objects / MSocket 名下 4 个事件绑定**无法计数取证**（会得 1/2）。
/// DFM 也为它写了 <c>Left=40 Top=32</c>，即原文就按"可视位置"建模。
/// 故：派生 Control + 不可见 + 零尺寸，行为面不变，仅让对账口径可及。
/// </para>
/// <para>
/// ⚠ 台账 §25.2：接缝默认实现不得静默返回中性值。JSocket 未移植 ⇒
/// <c>Active := True</c>（真正开始监听）**显式抛"未接线"**；<c>Active := False</c> 允许
/// （DFM 初值即 False，且"停止监听"是幂等无害操作）。宿主/测试用
/// <see cref="TFrmMasSoc.SocketFactory"/> 注入替身。
/// </para>
/// </summary>
public class TServerSocket : System.Windows.Forms.Control
{
    private bool _active;

    /// <summary>JSocket <c>TServerSocket.Active</c>（DFM: Active=False）。</summary>
    public virtual bool Active
    {
        get => _active;
        set
        {
            if (value) ThrowUnwiredActive();
            _active = value;
        }
    }

    /// <summary>承载 Active 属性值，供替身复用（默认实现里 Active=True 会抛）。</summary>
    protected bool ActiveState
    {
        get => _active;
        set => _active = value;
    }

    /// <summary>未接线时的显式失败（§25.2）。</summary>
    protected static void ThrowUnwiredActive()
        => throw new InvalidOperationException(
            "未接线：JSocket.TServerSocket 未移植，Active := True 无法真正监听（见 D-P10-16 / B-P10-19）。");

    /// <summary>JSocket <c>TServerSocket.Address</c>（DFM: '0.0.0.0'）。</summary>
    public virtual string Address { get; set; } = "0.0.0.0";

    /// <summary>JSocket <c>TServerSocket.Port</c>（DFM: 0）。</summary>
    public virtual int Port { get; set; }

    /// <summary>JSocket <c>TServerSocket.ServerType</c>（DFM: stNonBlocking）。</summary>
    public virtual TServerType ServerType { get; set; } = TServerType.stNonBlocking;

    // ---- 4 个 DFM 事件（名字与 DFM 属性名 1:1）----
    public event EventHandler<TClientSocketEventArgs> OnClientConnect;
    public event EventHandler<TClientSocketEventArgs> OnClientDisconnect;
    public event EventHandler<TClientSocketEventArgs> OnClientRead;
    public event EventHandler<TClientSocketEventArgs> OnClientError;

    // ---- 接缝：驱动事件（无真实 socket；测试/宿主用，也让"DFM 绑定确实接上了"可取证）----

    /// <summary>触发 <c>OnClientConnect</c>。</summary>
    public void RaiseClientConnect(TCustomWinSocket socket)
        => OnClientConnect?.Invoke(this, new TClientSocketEventArgs(socket));

    /// <summary>触发 <c>OnClientDisconnect</c>。</summary>
    public void RaiseClientDisconnect(TCustomWinSocket socket)
        => OnClientDisconnect?.Invoke(this, new TClientSocketEventArgs(socket));

    /// <summary>触发 <c>OnClientRead</c>。</summary>
    public void RaiseClientRead(TCustomWinSocket socket)
        => OnClientRead?.Invoke(this, new TClientSocketEventArgs(socket));

    /// <summary>
    /// 触发 <c>OnClientError</c>；返回值 = 处理器改过的 ErrorCode（原文 var 参数）。
    /// <paramref name="errorCode"/> 是事件参数的初值（测试用：给非 0 初值即可证明处理器确实改写过它）。
    /// </summary>
    public int RaiseClientError(TCustomWinSocket socket, TErrorEvent errorEvent, int errorCode = 0)
    {
        var e = new TClientSocketEventArgs(socket, errorEvent) { ErrorCode = errorCode };
        OnClientError?.Invoke(this, e);
        return e.ErrorCode;
    }
}

// ---------------------------------------------------------------------------
// MasSock.pas:26-54 TFrmMasSoc
// ---------------------------------------------------------------------------

/// <summary>
/// MasSock.pas:26-54 <c>TFrmMasSoc</c> 1:1（LoginSrv 主控 socket 窗体：维护游戏/登录服务器列表）。
/// <para>原文类名就是 <c>TFrmMasSoc</c>（无尾 k），DFM 根对象类型同名 —— 原文如此，不改。</para>
/// </summary>
public sealed class TFrmMasSoc : System.Windows.Forms.Form
{
    // ---- DFM 组件（名称与 DFM 1:1）----

    /// <summary>DFM: <c>object MSocket: TServerSocket</c>（左 40 / 上 32；Active=False Address='0.0.0.0' Port=0 ServerType=stNonBlocking）。</summary>
    public TServerSocket MSocket;

    public TFrmMasSoc()
    {
        InitializeComponent();
    }

    /// <summary>
    /// 测试/宿主注入：替换 DFM 的 MSocket 实例（默认接缝在 <c>Active := True</c> 时抛"未接线"）。
    /// 必须在构造窗体**之前**设置（DFM 的 4 个事件绑定在 InitializeComponent 里挂接）。
    /// </summary>
    public static Func<TServerSocket> SocketFactory;

    /// <summary>
    /// MasSock.pas:45 <c>m_ServerList: TList</c>（TList.Create 于 FormCreate；FormDestroy 里 Free）。
    /// 托管等价：<c>List&lt;TMsgServerInfo&gt;</c>（条目为引用类型 ⇒ 与 TList 的指针语义一致）。
    /// </summary>
    public List<TMsgServerInfo> m_ServerList;

    private void InitializeComponent()
    {
        // DFM 窗体属性（Caption/ClientHeight/ClientWidth/Left/Top；DFM **无** BorderStyle/Position ⇒ 不设）
        Text = "FrmMasSoc";                                     // Caption = 'FrmMasSoc'
        StartPosition = System.Windows.Forms.FormStartPosition.Manual;
        Location = new System.Drawing.Point(780, 172);           // Left = 780  Top = 172
        ClientSize = new System.Drawing.Size(137, 107);          // ClientWidth = 137  ClientHeight = 107

        // object MSocket: TServerSocket
        MSocket = SocketFactory != null ? SocketFactory() : new TServerSocket();
        MSocket.Name = "MSocket";
        MSocket.Left = 40;                                       // DFM: Left = 40
        MSocket.Top = 32;                                        // DFM: Top = 32
        MSocket.Active = false;                                  // DFM: Active = False
        MSocket.Address = "0.0.0.0";                             // DFM: Address = '0.0.0.0'
        MSocket.Port = 0;                                        // DFM: Port = 0
        MSocket.ServerType = TServerType.stNonBlocking;          // DFM: ServerType = stNonBlocking

        // DFM 事件绑定（4 条，逐字对齐 DFM 的 OnClientXxx = MSocketClientXxx）
        MSocket.OnClientConnect += (s, e) => MSocketClientConnect(s, e.Socket);
        MSocket.OnClientDisconnect += (s, e) => MSocketClientDisconnect(s, e.Socket);
        MSocket.OnClientRead += (s, e) => MSocketClientRead(s, e.Socket);
        MSocket.OnClientError += (s, e) => MSocketClientError(s, e.Socket, e.ErrorEvent, ref e.ErrorCode);

        Controls.Add(MSocket);

        // DFM: OnCreate = FormCreate → 本工程既有窗体统一约定 `Load += (s, e) => FormCreate(s);`
        // （见 GXX.DBServer/AddrEdit.cs:272、Forms/GrobalSession.cs:154）
        Load += (s, e) => FormCreate(s);
        // DFM: OnDestroy = FormDestroy
        FormClosed += (s, e) => FormDestroy(s);
    }

    // ---------------- 窗体生命周期 ----------------

    /// <summary>MasSock.pas:133-138 <c>procedure TFrmMasSoc.FormCreate(Sender: TObject)</c>。</summary>
    public void FormCreate(object Sender)
    {
        m_ServerList = new List<TMsgServerInfo>();     // 原：TList.Create
        LoadServerAddr();
        LoadUserLimit();
    }

    /// <summary>
    /// MasSock.pas:632-643 <c>procedure TFrmMasSoc.FormDestroy(Sender: TObject)</c>。
    /// <para>★ F8 原文如此：<c>m_ServerList.Free</c> 之后**不置 nil**（悬垂指针）。
    /// 托管侧用 <c>m_ServerList = null</c> 表达"已释放但仍被字段引用"，于是后续访问变成
    /// 可断言的 NullReferenceException（Delphi 侧是读已释放内存的未定义行为）。</para>
    /// <para>★ 与原文同：<c>m_ServerList</c> 为 nil 时（未跑 FormCreate 就销毁）直接访问 .Count
    /// ⇒ 原文是访问违例，托管侧是 NullReferenceException。</para>
    /// </summary>
    public void FormDestroy(object Sender)
    {
        for (int I = 0; I <= m_ServerList.Count - 1; I++)
        {
            TMsgServerInfo MsgServer = m_ServerList[I];
            // 原：Dispose(MsgServer) —— 托管侧条目由 GC 回收（New/Dispose 配对语义等价）
        }
        m_ServerList = null;                           // 原：m_ServerList.Free（字段未置 nil，原文如此；D-P10-21）
    }

    // ---------------- 服务启动 ----------------

    /// <summary>
    /// MasSock.pas:140-153 <c>procedure TFrmMasSoc.StartService</c>。
    /// ★ F9 原文如此：<c>except</c> 无 <c>on e:</c> ⇒ 吞掉一切异常，只留一行日志。
    /// </summary>
    public void StartService()
    {
        TConfig Config = LoginSrvShare.g_Config;        // 原：Config := @g_Config（指针 → 引用）
        MSocket.Address = Config.sServerAddr;
        MSocket.Port = Config.nServerPort;
        try
        {
            MSocket.Active = true;
            LoginSrvShare.MainOutMessage(DelphiRTL.Format("游戏中心服务启动成功(%s:%d)...",
                new object[] { Config.sServerAddr, Config.nServerPort }));
        }
        catch
        {
            LoginSrvShare.MainOutMessage("TFrmMasSoc.StartService");   // 原文如此：异常信息被丢弃
        }
    }

    // ---------------- socket 事件（DFM 的 4 条绑定） ----------------

    /// <summary>
    /// MasSock.pas:155-187 <c>procedure TFrmMasSoc.MSocketClientConnect</c> —— 只接受
    /// <c>g_ServerAddr[0..g_ServerAddrCount-1]</c> 白名单内的远端地址。
    /// <para>★ 原文如此：循环上界直接取 <c>g_ServerAddrCount</c>，**不夹紧到数组长度 100**；
    /// 计数被外部写大时原文读数组外内存，托管侧抛 IndexOutOfRangeException（差异锁定用例）。</para>
    /// </summary>
    public void MSocketClientConnect(object Sender, TCustomWinSocket Socket)
    {
        string sRemoteAddr = Socket.RemoteAddress;
        bool boAllowed = false;
        for (int I = 0; I <= MasSockGlobals.g_ServerAddrCount - 1; I++)     // 原：Low(g_ServerAddr) .. Count-1
        {
            if (sRemoteAddr == MasSockGlobals.g_ServerAddr[I])              // 区分大小写的逐字节比较
            {
                boAllowed = true;
                break;
            }
        }

        if (boAllowed)
        {
            var MsgServer = new TMsgServerInfo();      // 原：New(MsgServer) + FillChar(#0)
            MsgServer.sReceiveMsg = "";                // 原文如此：FillChar 之后又冗余赋一次空串
            MsgServer.Socket = Socket;
            m_ServerList.Add(MsgServer);
        }
        else
        {
            LoginSrvShare.MainOutMessage("非法地址连接:" + sRemoteAddr);
            Socket.Close();
        }
    }

    /// <summary>
    /// MasSock.pas:190-206 <c>procedure TFrmMasSoc.MSocketClientDisconnect</c>。
    /// <para>★ 原文如此：比较是 <c>MsgServer.Socket = Socket</c>（指针相等）⇒ 两个 nil 也相等，
    /// 因此 <c>Socket = nil</c> 调用会**删掉第一个 Socket 为 nil 的条目**（用例锁定）。
    /// 同族原文缺失：Dispose 后未把 Socket 置 nil，仅 Delete 条目。</para>
    /// </summary>
    public void MSocketClientDisconnect(object Sender, TCustomWinSocket Socket)
    {
        for (int I = 0; I <= m_ServerList.Count - 1; I++)
        {
            TMsgServerInfo MsgServer = m_ServerList[I];
            if (MsgServer.Socket == Socket)
            {
                // 原：Dispose(MsgServer)（托管侧由 GC 回收）
                m_ServerList.RemoveAt(I);
                break;                                  // 原：break
            }
        }
    }

    /// <summary>MasSock.pas:208-214 <c>procedure TFrmMasSoc.MSocketClientError</c>（ErrorCode := 0; Socket.Close）。</summary>
    public void MSocketClientError(object Sender, TCustomWinSocket Socket, TErrorEvent ErrorEvent, ref int ErrorCode)
    {
        ErrorCode = 0;
        Socket.Close();
    }

    /// <summary>
    /// MasSock.pas:268-630 <c>procedure TFrmMasSoc.MSocketClientRead</c> —— 主协议解析（'(code/body)' 逐帧）。
    /// <para>
    /// 照抄要点（均有用例锁定）：
    /// * F1 :628 的 <c>MsgServer.sReceiveMsg := sReviceMsg</c> 在 <c>if Socket = Socket</c> **之外**；
    /// * F2 :383 / :622 的 <c>Exit</c> 退出**整个过程**（C# <c>return</c>），后面的帧与回写都被跳过；
    /// * F12 :389-398 的帐号名校验整块被注释 ⇒ 不校验；
    /// * F11 :541-556 手机号"非数字"标志取反且复用错误码 -13；
    /// * 未接线的 <c>GetSessionID</c> / <c>CloseUser</c> / <c>g_AccountDB</c> 直接调用 ⇒ 显式失败/异常
    ///   （Delphi 侧分别是 LSShare 全局可用、LMain 函数可用、g_AccountDB 为 nil 时的访问违例）。
    /// </para>
    /// </summary>
    public void MSocketClientRead(object Sender, TCustomWinSocket Socket)
    {
        const string sFormatMsg = "(%d/%s)";            // resourcestring :271（本过程内未使用，原文如此）
        // ---- 局部变量（原文 :272-292 的 var 段；Delphi 对托管类型局部变量在序言里零初始化）----
        int I;
        int II;
        TMsgServerInfo MsgServer;
        string sReviceMsg = "";                         // 原文如此：编译器零初始化（首次未命中时写回 ""）
        string sMsg = "";
        string sCode = "";
        string sAccount = "";
        string sServerName = "";
        string sIndex = "";
        string sOnlineCount = "";
        int nCode;

        int nSessionID = 0;
        TAccountInfo AccountInfo = default;             // 原：AccountInfo: TAccountInfo
        TAccountInfo2 ChangeAccountInfo;
        bool bo21 = false;
        int nErrCode = 0;
        string WS = "";
        char WC = '\0';
        bool boTemp = false;
        string sTemp = "";

        for (I = 0; I <= m_ServerList.Count - 1; I++)
        {
            MsgServer = m_ServerList[I];
            if (MsgServer.Socket == Socket)             // 原：指针相等 → 托管引用相等
            {
                sReviceMsg = MsgServer.sReceiveMsg + Socket.ReceiveText;
                while (DelphiRTL.Pos(")", sReviceMsg) > 0)                      // 原 :300
                {
                    // 原 :302 用的是 HUtil32.ArrestStringEx_Ansi（**未命中时保留原串**）。
                    // B-P10-17 已关闭：Core 已按原文修正，故直接转调核心实现，不再本地复刻。
                    sReviceMsg = HUtil32.ArrestStringEx_Ansi(sReviceMsg, '(', ')', ref sMsg);
                    if (sMsg == "") break;                                      // 原 :303
                    sMsg = HUtil32.GetValidStr3(sMsg, ref sCode, new[] { '/' }); // 原 :304
                    nCode = DelphiRTL.StrToIntDef(sCode, -1);                    // 原 :305
                    switch (nCode)
                    {
                        case CommonConst.SS_SOFTOUTSESSION:                      // 原 :307
                        {
                            sMsg = HUtil32.GetValidStr3(sMsg, ref sAccount, new[] { '/' });
                            MasSockGlobals.CloseUser(MsgServer.sServerName, sAccount, DelphiRTL.StrToIntDef(sMsg, 0));
                            break;
                        }
                        case CommonConst.SS_SERVERINFO:                          // 原 :312
                        {
                            sMsg = HUtil32.GetValidStr3(sMsg, ref sServerName, new[] { '/' });
                            sMsg = HUtil32.GetValidStr3(sMsg, ref sIndex, new[] { '/' });
                            sMsg = HUtil32.GetValidStr3(sMsg, ref sOnlineCount, new[] { '/' });
                            MsgServer.sServerName = sServerName;
                            MsgServer.nServerIndex = DelphiRTL.StrToIntDef(sIndex, 0);
                            MsgServer.nOnlineCount = DelphiRTL.StrToIntDef(sOnlineCount, 0);
                            MsgServer.dwKeepAliveTick = DelphiRTL.GetTickCount();
                            SortServerList(I);
                            MasSockGlobals.nOnlineCountMin = GetOnlineHumCount();
                            if (MasSockGlobals.nOnlineCountMin > MasSockGlobals.nOnlineCountMax)
                                MasSockGlobals.nOnlineCountMax = MasSockGlobals.nOnlineCountMin;
                            SendServerMsgA((ushort)CommonConst.SS_KEEPALIVE, DelphiRTL.IntToStr(MasSockGlobals.nOnlineCountMin));
                            RefServerLimit(sServerName);
                            break;
                        }
                        case CommonConst.UNKNOWMSG:                              // 原 :327
                            SendServerMsgA((ushort)CommonConst.UNKNOWMSG, sMsg);
                            break;
                        case CommonConst.SS_PASSWORDSUCCESS:                     // 原 :328
                        {
                            sAccount = sMsg;
                            nSessionID = MasSockGlobals.GetSessionID();
                            // 原文 :332-338 的 SessionAdd(...) 整块被注释掉 ⇒ 不移植
                            // 原文 :340-342 的"直接 SendText 回包"也整块被注释掉 ⇒ 不移植
                            SendServerMsg((ushort)CommonConst.SS_OPENSESSION, MsgServer.sServerName,
                                sAccount + "/" + DelphiRTL.IntToStr(nSessionID) + "/"
                                // 原：IntToStr(Integer(True))；Integer(True) = 1（非 -1）
                                // 取证见 GXX.Core/Util/HUtil32.cs:672
                                + DelphiRTL.IntToStr(1) + "/" + DelphiRTL.IntToStr(5) + "/" + Socket.RemoteAddress);
                            break;
                        }
                        case CommonConst.SS_GetAccountInfo:                      // 原 :348（M2 获取用户注册信息）
                        {
                            sMsg = HUtil32.GetValidStr3(sMsg, ref sAccount, new[] { '/' });   // sMsg 用户名
                            if (sAccount.Length > 0)
                            {
                                // 原文直接解引用 g_AccountDB（nil 时是访问违例）⇒ 托管侧不遮蔽 NRE
                                if (LoginSrvShare.g_AccountDB.GetAccount(sAccount, ref AccountInfo))
                                {
                                    SendServerMsg((ushort)CommonConst.SS_GetAccountInfoRet, MsgServer.sServerName,
                                        sMsg + " /" +
                                        AccountInfo.UserNameStr + " \t" +                  // 原：' '#9
                                        AccountInfo.PasswordStr + " \t" +
                                        AccountInfo.Questions1Str + " \t" +
                                        AccountInfo.Answers1Str + " \t" +
                                        AccountInfo.Questions2Str + " \t" +
                                        AccountInfo.Answers2Str + " \t" +
                                        AccountInfo.BirthDayStr + " \t" +
                                        AccountInfo.MobilePhoneStr + " \t" +
                                        AccountInfo.PhoneStr + " \t" +
                                        AccountInfo.MailStr + " \t");
                                }
                            }
                            break;
                        }
                        case CommonConst.SS_ChangeAccountInfo:                   // 原 :374（M2 修改注册信息）
                        {
                            // 原文：GetDecodeSize(Length(sMsg)) = SizeOf(TAccountInfo2) = 218
                            if (EDcode.GetDecodeSize(sMsg.Length) == MasSockFns.TAccountInfo2PackedSize)
                            {
                                byte[] src = EncodingInit.GBK.GetBytes(sMsg);
                                byte[] decoded = new byte[MasSockFns.TAccountInfo2PackedSize];
                                EDcode.DecodeString(src, decoded, decoded.Length);
                                // 原：DecodeString(sMsg, @ChangeAccountInfo, SizeOf(ChangeAccountInfo))
                                unsafe
                                {
                                    fixed (byte* p = decoded) ChangeAccountInfo = *(TAccountInfo2*)p;
                                }

                                if (!LoginSrvShare.g_AccountDB.GetAccount(ChangeAccountInfo.AccountNameStr, ref AccountInfo))
                                {
                                    SendServerMsg((ushort)CommonConst.SS_ChangeAccountInfoRet, MsgServer.sServerName, "-1/" + sMsg);
                                    return;                                  // 原 :383 Exit（退出整个过程）
                                }

                                nErrCode = 0;
                                bo21 = true;

                                // ★ F12 原文如此：帐号名合法性校验整块被 {...} 注释掉（:389-398）⇒ 不移植

                                if (bo21)
                                {
                                    if ((ChangeAccountInfo.PasswordStr.Length < 3) ||
                                        (!MasSockFns.CheckStringValid(ChangeAccountInfo.PasswordStr)))
                                    {
                                        bo21 = false;
                                        nErrCode = -2;
                                    }
                                    /* TODO -ochongchong -c修改 : 禁止ID密码相同 【2013-08-28】 */
                                    else if (LoginSrvShare.g_Config.boDisableIDSamePassword &&
                                             MasSockFns.SameText(ChangeAccountInfo.AccountNameStr, ChangeAccountInfo.PasswordStr))
                                    {
                                        bo21 = false;
                                        nErrCode = -3;
                                    }
                                    else
                                    {
                                        if (LoginSrvShare.g_Config.boDisablePwdSameChr)
                                        {
                                            WS = ChangeAccountInfo.PasswordStr;
                                            WC = MasSockFns.AnsiStringCharAt(WS, 1);     // 原：WS[1]（WideString 1-based）
                                            boTemp = true;
                                            for (II = 2; II <= WS.Length; II++)
                                            {
                                                if (WS[II - 1] != WC)
                                                {
                                                    boTemp = false;
                                                    break;
                                                }
                                            }

                                            if (boTemp)
                                            {
                                                bo21 = false;
                                                nErrCode = -4;
                                            }
                                        }

                                        if (bo21)
                                        {
                                            if (LoginSrvShare.g_Config.boDisablePwdAllNum)
                                            {
                                                boTemp = true;
                                                WS = ChangeAccountInfo.PasswordStr;
                                                for (II = 1; II <= WS.Length; II++)
                                                {
                                                    if (!(WS[II - 1] >= '0' && WS[II - 1] <= '9'))
                                                    {
                                                        boTemp = false;
                                                        break;
                                                    }
                                                }

                                                if (boTemp)
                                                {
                                                    bo21 = false;
                                                    nErrCode = -5;
                                                }
                                            }
                                        }

                                        if (bo21)
                                        {
                                            if (LoginSrvShare.g_Config.boDisablePwdAllLetter)
                                            {
                                                boTemp = true;
                                                WS = ChangeAccountInfo.PasswordStr;
                                                for (II = 1; II <= WS.Length; II++)
                                                {
                                                    if (!((WS[II - 1] >= 'a' && WS[II - 1] <= 'z') ||
                                                          (WS[II - 1] >= 'A' && WS[II - 1] <= 'Z')))
                                                    {
                                                        boTemp = false;
                                                        break;
                                                    }
                                                }

                                                if (boTemp)
                                                {
                                                    bo21 = false;
                                                    nErrCode = -6;
                                                }
                                            }
                                        }

                                        if (bo21)
                                        {
                                            if (LoginSrvShare.g_DisablePasswordList.Count > 0)
                                            {
                                                for (II = 0; II <= LoginSrvShare.g_DisablePasswordList.Count - 1; II++)
                                                {
                                                    sTemp = LoginSrvShare.g_DisablePasswordList[II];   // 原：.Strings[II]
                                                    if ((sTemp.Length > 0) && (DelphiRTL.Pos(sTemp, ChangeAccountInfo.PasswordStr) > 0))
                                                    {
                                                        bo21 = false;
                                                        nErrCode = -7;
                                                        // 原文如此：无 break（继续扫完）
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                if (bo21 && (!MasSockFns.CheckStringValid(ChangeAccountInfo.UserNameStr)))
                                {
                                    bo21 = false;
                                    nErrCode = -8;
                                }

                                if (bo21 && (!MasSockFns.CheckStringValid(ChangeAccountInfo.Questions1Str)))
                                {
                                    bo21 = false;
                                    nErrCode = -9;
                                }

                                if (bo21 && (!MasSockFns.CheckStringValid(ChangeAccountInfo.Answers1Str)))
                                {
                                    bo21 = false;
                                    nErrCode = -10;
                                }

                                if (bo21 && (!MasSockFns.CheckStringValid(ChangeAccountInfo.Questions2Str)))
                                {
                                    bo21 = false;
                                    nErrCode = -11;
                                }

                                if (bo21 && (!MasSockFns.CheckStringValid(ChangeAccountInfo.Answers2Str)))
                                {
                                    bo21 = false;
                                    nErrCode = -12;
                                }

                                if (bo21 && (!MasSockFns.CheckStringValid(ChangeAccountInfo.MobilePhoneStr)))
                                {
                                    bo21 = false;
                                    nErrCode = -13;                                  // 与下方"非数字"分支**同码**（原文如此）
                                }

                                if (bo21)
                                {
                                    // ★ F11 原文如此：标志初始化取反 —— boTemp=False，遇到"非数字"才置 True
                                    boTemp = false;
                                    WS = ChangeAccountInfo.MobilePhoneStr;
                                    for (II = 1; II <= WS.Length; II++)
                                    {
                                        if (!(WS[II - 1] >= '0' && WS[II - 1] <= '9'))
                                        {
                                            boTemp = true;
                                            break;
                                        }
                                    }

                                    if (boTemp)
                                    {
                                        bo21 = false;
                                        nErrCode = -13;
                                    }
                                }

                                if (bo21 && (!MasSockFns.CheckStringValid2(ChangeAccountInfo.MailStr)))   // 邮箱用 2 版（允许 '@'）
                                {
                                    bo21 = false;
                                    nErrCode = -14;
                                }

                                if (bo21 && LoginSrvShare.g_Config.boDisableQuizSameAnswer &&
                                    MasSockFns.SameText(ChangeAccountInfo.Questions1Str, ChangeAccountInfo.Answers1Str))
                                {
                                    bo21 = false;
                                    nErrCode = -15;
                                }

                                if (bo21 && LoginSrvShare.g_Config.boDisableQuizSameAnswer &&
                                    MasSockFns.SameText(ChangeAccountInfo.Questions2Str, ChangeAccountInfo.Answers2Str))
                                {
                                    bo21 = false;
                                    nErrCode = -16;
                                }

                                if (bo21 && (!MasSockFns.CheckStringValid(ChangeAccountInfo.L2PasswordStr)))
                                {
                                    bo21 = false;
                                    nErrCode = -17;
                                }

                                // 禁止帐户和二级密码一致
                                if (bo21 && (ChangeAccountInfo.L2PasswordStr.Length > 0))
                                {
                                    if (LoginSrvShare.g_Config.boDisableIDSameL2Password &&
                                        MasSockFns.SameText(AccountInfo.AccountNameStr, ChangeAccountInfo.L2PasswordStr))
                                    {
                                        bo21 = false;
                                        nErrCode = -18;
                                    }
                                }

                                if (bo21 && (ChangeAccountInfo.L2PasswordStr.Length > 0))
                                {
                                    if (LoginSrvShare.g_Config.boDisableL2SamePassword &&
                                        MasSockFns.SameText(ChangeAccountInfo.PasswordStr, ChangeAccountInfo.L2PasswordStr))
                                    {
                                        bo21 = false;
                                        nErrCode = -19;
                                    }
                                }

                                if (bo21)
                                {
                                    AccountInfo.PasswordStr = ChangeAccountInfo.PasswordStr;
                                    AccountInfo.UserNameStr = ChangeAccountInfo.UserNameStr;
                                    AccountInfo.BirthDayStr = ChangeAccountInfo.BirthDayStr;
                                    AccountInfo.Questions1Str = ChangeAccountInfo.Questions1Str;
                                    AccountInfo.Answers1Str = ChangeAccountInfo.Answers1Str;
                                    AccountInfo.Questions2Str = ChangeAccountInfo.Questions2Str;
                                    AccountInfo.Answers2Str = ChangeAccountInfo.Answers2Str;
                                    AccountInfo.MobilePhoneStr = ChangeAccountInfo.MobilePhoneStr;
                                    AccountInfo.MailStr = ChangeAccountInfo.MailStr;
                                    AccountInfo.L2PasswordStr = ChangeAccountInfo.L2PasswordStr;

                                    if (LoginSrvShare.g_AccountDB.UpdateAccount(AccountInfo, TAccountUpdateField.ufAllField))
                                        nErrCode = 0;
                                    else
                                        nErrCode = -20;
                                }

                                SendServerMsg((ushort)CommonConst.SS_ChangeAccountInfoRet, MsgServer.sServerName,
                                    DelphiRTL.IntToStr(nErrCode) + "/" + sMsg);
                                return;                                      // 原 :622 Exit（退出整个过程）
                            }
                            break;
                        }
                    }
                }
            }
            // ★ F1 原文如此（:628）：这一行在 `if MsgServer.Socket = Socket` **之外**，
            //   即每个未命中的条目也会被写入同一份 sReviceMsg（首次未命中时是 ""）。
            MsgServer.sReceiveMsg = sReviceMsg;
        }
    }

    // ---------------- 服务器列表维护 ----------------

    /// <summary>
    /// MasSock.pas:647-672 <c>procedure TFrmMasSoc.RefServerLimit</c>（统计同名在线数 → 写 nLimitCountMin）。
    /// ★ F9 原文如此：空 except。
    /// </summary>
    private void RefServerLimit(string sServerName)
    {
        try
        {
            int nCount = 0;
            for (int I = 0; I <= m_ServerList.Count - 1; I++)
            {
                TMsgServerInfo MsgServer = m_ServerList[I];
                if ((MsgServer.nServerIndex != 99) && (MsgServer.sServerName == sServerName))
                    nCount += MsgServer.nOnlineCount;
            }
            for (int I = 0; I <= MasSockGlobals.UserLimit.Length - 1; I++)          // 原：Low..High(UserLimit)
            {
                if (MasSockGlobals.UserLimit[I].sServerName == sServerName)
                {
                    MasSockGlobals.UserLimit[I].nLimitCountMin = nCount;
                    break;
                }
            }
        }
        catch
        {
            LoginSrvShare.MainOutMessage("TFrmMasSoc.RefServerLimit");              // 原文如此：异常信息被丢弃
        }
    }

    /// <summary>
    /// MasSock.pas:677-691 <c>function TFrmMasSoc.IsNotUserFull(sServerName): Boolean</c>。
    /// 未命中 ⇒ True（原文如此：无 UserLimit 条目的服务器被判为"未满"）。
    /// </summary>
    public bool IsNotUserFull(string sServerName)
    {
        bool Result = true;
        for (int I = 0; I <= MasSockGlobals.UserLimit.Length - 1; I++)
        {
            if (MasSockGlobals.UserLimit[I].sServerName == sServerName)
            {
                if (MasSockGlobals.UserLimit[I].nLimitCountMin > MasSockGlobals.UserLimit[I].nLimitCountMax)
                    Result = false;
                break;                                                              // 首个同名条目定胜负
            }
        }
        return Result;
    }

    /// <summary>
    /// MasSock.pas:694-755 <c>procedure TFrmMasSoc.SortServerList(nIndex)</c>
    /// —— 取出第 nIndex 条、按 (sServerName, nServerIndex) 插回，并删除插入点之后的重复项。
    /// ★ F9 原文如此：空 except（插入越界时 Delphi 抛 EListError，托管抛 ArgumentOutOfRangeException）。
    /// </summary>
    private void SortServerList(int nIndex)
    {
        try
        {
            if (m_ServerList.Count <= nIndex) return;                  // 原 :702 exit
            TMsgServerInfo MsgServerSort = m_ServerList[nIndex];
            m_ServerList.RemoveAt(nIndex);                             // 原：Delete(nIndex)
            for (int nC = 0; nC <= m_ServerList.Count - 1; nC++)
            {
                TMsgServerInfo MsgServer = m_ServerList[nC];
                if (MsgServer.sServerName == MsgServerSort.sServerName)
                {
                    if (MsgServer.nServerIndex < MsgServerSort.nServerIndex)
                    {
                        m_ServerList.Insert(nC, MsgServerSort);
                        return;                                        // 原 :713 exit
                    }
                    else
                    {
                        int nNewIndex = nC + 1;
                        if (nNewIndex < m_ServerList.Count)
                        {
                            for (int n10 = nNewIndex; n10 <= m_ServerList.Count - 1; n10++)
                            {
                                MsgServer = m_ServerList[n10];
                                if (MsgServer.sServerName == MsgServerSort.sServerName)
                                {
                                    if (MsgServer.nServerIndex < MsgServerSort.nServerIndex)
                                    {
                                        m_ServerList.Insert(n10, MsgServerSort);
                                        for (int n14 = n10 + 1; n14 <= m_ServerList.Count - 1; n14++)
                                        {
                                            MsgServer = m_ServerList[n14];
                                            if ((MsgServer.sServerName == MsgServerSort.sServerName) &&
                                                (MsgServer.nServerIndex == MsgServerSort.nServerIndex))
                                            {
                                                m_ServerList.RemoveAt(n14);          // 删重复
                                                return;                             // 原 :734 exit
                                            }
                                        }
                                        return;                                     // 原 :737 exit
                                    }
                                    else
                                    {
                                        nNewIndex = n10 + 1;
                                    }
                                }
                            }
                            m_ServerList.Insert(nNewIndex, MsgServerSort);
                            return;                                             // 原 :746 exit
                        }
                    }
                }
            }
            m_ServerList.Add(MsgServerSort);                                    // 原 :751
        }
        catch
        {
            LoginSrvShare.MainOutMessage("TFrmMasSoc.SortServerList");          // 原文如此：异常信息被丢弃
        }
    }

    // ---------------- 发送 ----------------

    /// <summary>
    /// MasSock.pas:760-800 <c>procedure TFrmMasSoc.SendServerMsg(wIdent, sServerName, sMsg)</c>
    /// —— 按 LimitName(sServerName) 过滤后广播 <c>(wIdent/sMsg)</c>。
    /// <para>★ F13 原文如此：<c>MsgServer.Socket.Connected</c> 无 nil 保护 ⇒ 一个坏条目让整轮中断。</para>
    /// <para>★ F9 原文如此：空 except。<c>{$IFDEF LOG_SESSION}</c> 块（:770-775 / :788-793）未编译 ⇒ 不移植。</para>
    /// </summary>
    public void SendServerMsg(ushort wIdent, string sServerName, string sMsg)
    {
        const string sFormatMsg = "(%d/%s)";                    // resourcestring :766-767
        try
        {
            string s18 = LimitName(sServerName);                // 原 :777
            string sSendMsg = DelphiRTL.Format(sFormatMsg, new object[] { wIdent, sMsg });
            for (int I = 0; I <= m_ServerList.Count - 1; I++)
            {
                TMsgServerInfo MsgServer = m_ServerList[I];     // 原：pTMsgServerInfo(m_ServerList.Items[I]) 硬转型
                if (MsgServer.Socket.Connected)
                {
                    // ★ F14 原文如此：名字比较用 CompareText（忽略大小写）
                    if ((s18 == "") || (MsgServer.sServerName == "") ||
                        (MasSockFns.CompareText(MsgServer.sServerName, s18) == 0) ||
                        (MsgServer.nServerIndex == 99))
                    {
                        MsgServer.Socket.SendText(sSendMsg, true);      // 原 :786 第二参 True
                    }
                }
            }
        }
        catch
        {
            LoginSrvShare.MainOutMessage("TFrmMasSoc.SendServerMsg");   // 原文如此：异常信息被丢弃
        }
    }

    /// <summary>
    /// MasSock.pas:872-894 <c>procedure TFrmMasSoc.SendServerMsgA(wIdent, sMsg)</c> —— 不过滤，全体广播。
    /// ★ 与同单元其它方法不同：这里是 <c>on e: Exception</c>，会把 <c>E.Message</c> 也记一行（原文如此）。
    /// </summary>
    public void SendServerMsgA(ushort wIdent, string sMsg)
    {
        const string sFormatMsg = "(%d/%s)";                    // resourcestring :877-878
        try
        {
            string sSendMsg = DelphiRTL.Format(sFormatMsg, new object[] { wIdent, sMsg });
            for (int I = 0; I <= m_ServerList.Count - 1; I++)
            {
                TMsgServerInfo MsgServer = m_ServerList[I];     // 原：硬转型
                if (MsgServer.Socket.Connected) MsgServer.Socket.SendText(sSendMsg);   // 原 :885 不带 bSecure
            }
        }
        catch (Exception e)
        {
            LoginSrvShare.MainOutMessage("TFrmMasSoc.SendServerMsgA");
            LoginSrvShare.MainOutMessage(e.Message);
        }
    }

    /// <summary>
    /// MasSock.pas:897-914 <c>function TFrmMasSoc.LimitName(sServerName): string</c>
    /// —— 取授权名（CompareText 忽略大小写，首个命中胜出）。★ F9 空 except。
    /// </summary>
    private string LimitName(string sServerName)
    {
        try
        {
            string Result = "";
            for (int i = 0; i <= MasSockGlobals.UserLimit.Length - 1; i++)
            {
                if (MasSockFns.CompareText(MasSockGlobals.UserLimit[i].sServerName, sServerName) == 0)
                {
                    Result = MasSockGlobals.UserLimit[i].sName;
                    break;
                }
            }
            return Result;
        }
        catch
        {
            LoginSrvShare.MainOutMessage("TFrmMasSoc.LimitName");       // 原文如此
            return "";                                                   // 原：Result 此时仍是 ''
        }
    }

    // ---------------- 装载 ----------------

    /// <summary>
    /// MasSock.pas:803-834 <c>procedure TFrmMasSoc.LoadServerAddr()</c> —— 读 <c>.\!ServerAddr.txt</c>
    /// 里"带 3 个点"的行填 <c>g_ServerAddr</c>（容量 100）。
    /// <para>★ F3 原文如此：<c>sLineText[I]</c> 把**行号**当 1-based 字符下标判 <c>;</c> 注释。</para>
    /// <para>★ F4 原文如此：<c>g_ServerAddrCount := nServerIdx</c> 写在 for 体内（0 行不赋值；满 100 break 时停在 99）。</para>
    /// <para>★ 与原文同：文件缺失时 <c>FillChar</c> 已把数组清空，但计数**保留旧值**（悬垂计数）。</para>
    /// </summary>
    public void LoadServerAddr()
    {
        string sFileName = MasSockGlobals.ServerAddrFileName;       // 原：'.\!ServerAddr.txt'
        int nServerIdx = 0;
        // 原：FillChar(g_ServerAddr, SizeOf(g_ServerAddr), #0)
        // ⇒ Delphi 静态 ShortString 数组被清成 100 个**空串**（读出来是 ''，不是 nil）
        for (int i = 0; i < MasSockGlobals.g_ServerAddr.Length; i++) MasSockGlobals.g_ServerAddr[i] = "";
        if (File.Exists(sFileName))
        {
            TStringList LoadList = new();                            // 原 :815 TStringList.Create
            LoadList.LoadFromFile(sFileName);
            for (int I = 0; I <= LoadList.Count - 1; I++)
            {
                string sLineText = DelphiRTL.Trim(LoadList[I]);       // 原 :819 Trim(LoadList.Strings[i])
                // ★ F3：I 是行号（0-based），原文当 1-based 字符下标用 ⇒ 用 Delphi AnsiString 语义复刻
                if ((sLineText != "") && (MasSockFns.AnsiStringCharAt(sLineText, I) != ';'))
                {
                    if (HUtil32.TagCount(sLineText, '.') == 3)
                    {
                        // 原：g_ServerAddr[nServerIdx] := sLineText（string[15] 赋值截断）
                        MasSockGlobals.g_ServerAddr[nServerIdx] = MasSockFns.TruncateToShortString15(sLineText);
                        nServerIdx++;
                        if (nServerIdx >= 100) break;                     // 原 :826
                    }
                }

                // ★ F4：这一行在 for 体内（原文如此）
                MasSockGlobals.g_ServerAddrCount = nServerIdx;
            }
            // 原：LoadList.Free（托管侧由 GC 回收）
        }
    }

    /// <summary>
    /// MasSock.pas:917-951 <c>procedure TFrmMasSoc.LoadUserLimit()</c> —— 读 <c>.\!UserLimit.txt</c>
    /// （<c>服务器名 显示名 上限</c>，上限缺省 3000）填 <c>UserLimit</c>，并写 <c>nUserLimit</c>。
    /// <para>★ F5 原文如此：<c>UserLimit[nC]</c> **无上界检查**（&gt;100 行 ⇒ 原文写越界内存，
    /// 托管抛 IndexOutOfRangeException；**D-P10-23**，差异锁定用例
    /// <c>LoadUserLimit_Over100Entries_Throws_OriginalFlaw</c>）。</para>
    /// <para>★ F6 原文如此：装载前**不清空** UserLimit ⇒ 残留旧条目仍会被扫描到。</para>
    /// <para>★ F7 原文如此：文件缺失时只 ShowMessage，不改 nUserLimit / UserLimit。</para>
    /// </summary>
    private void LoadUserLimit()
    {
        int nC = 0;
        string sFileName = MasSockGlobals.UserLimitFileName;         // 原：'.\!UserLimit.txt'
        if (File.Exists(sFileName))
        {
            TStringList LoadList = new();                            // 原 :929
            LoadList.LoadFromFile(sFileName);
            for (int i = 0; i <= LoadList.Count - 1; i++)
            {
                string sLineText = LoadList[i];                      // 原 :933（**不 Trim**，与 LoadServerAddr 不同）
                // Delphi 局部 string 由序言零初始化；C# ref 形参要求明确赋值 ⇒ 显式给初值（等价）
                string sServerName = "", s10 = "", s14 = "";
                sLineText = HUtil32.GetValidStr3(sLineText, ref sServerName, new[] { ' ', '\t' });
                sLineText = HUtil32.GetValidStr3(sLineText, ref s10, new[] { ' ', '\t' });
                sLineText = HUtil32.GetValidStr3(sLineText, ref s14, new[] { ' ', '\t' });
                if (sServerName != "")
                {
                    MasSockGlobals.UserLimit[nC].sServerName = sServerName;
                    MasSockGlobals.UserLimit[nC].sName = s10;
                    MasSockGlobals.UserLimit[nC].nLimitCountMax = DelphiRTL.StrToIntDef(s14, 3000);
                    MasSockGlobals.UserLimit[nC].nLimitCountMin = 0;
                    nC++;
                }
            }
            MasSockGlobals.nUserLimit = nC;                          // 原 :946（写在 if FileExists 内）
            // 原：LoadList.Free
        }
        else
        {
            // 原：Application.MessageBox（走 LoginSrvForms 接缝，测试可注入，不阻塞）
            LoginSrvForms.ShowMessage(@"[Critical Failure] file not found. .\!UserLimit.txt");
        }
    }

    // ---------------- 查询 ----------------

    /// <summary>
    /// MasSock.pas:839-857 <c>function TFrmMasSoc.GetOnlineHumCount(): Integer</c>
    /// —— 累加 <c>nServerIndex &lt;&gt; 99</c> 的 nOnlineCount。★ F9 空 except（失败返回 0）。
    /// </summary>
    public int GetOnlineHumCount()
    {
        int Result = 0;
        try
        {
            int nCount = 0;
            for (int i = 0; i <= m_ServerList.Count - 1; i++)
            {
                TMsgServerInfo MsgServer = m_ServerList[i];
                if (MsgServer.nServerIndex != 99)
                    nCount += MsgServer.nOnlineCount;
            }
            Result = nCount;
        }
        catch
        {
            LoginSrvShare.MainOutMessage("TFrmMasSoc.GetOnlineHumCount");   // 原文如此
        }
        return Result;
    }

    /// <summary>
    /// MasSock.pas:860-868 <c>function TFrmMasSoc.CheckReadyServers(): Boolean</c>
    /// —— <c>m_ServerList.Count &gt;= g_Config.nReadyServers</c>。
    /// 注意 <c>nReadyServers</c> 初值 0 ⇒ 空列表也返回 True（原文如此，用例锁定）。
    /// </summary>
    public bool CheckReadyServers()
    {
        TConfig Config = LoginSrvShare.g_Config;                // 原：Config := @g_Config
        bool Result = false;
        if (m_ServerList.Count >= Config.nReadyServers)
            Result = true;
        return Result;
    }

    /// <summary>
    /// MasSock.pas:953-1007 <c>function TFrmMasSoc.ServerStatus(sServerName): Integer</c>
    /// —— 1 空闲 / 2 良好 / 3 繁忙 / 4 满员；服务器不在线（或查不到授权条目）返回 0。
    /// ★ F10 原文如此：判定全用整数 <c>div</c>；<c>Max = 0</c> 时第一条恒真（恒返回 1）。
    /// ★ F9 原文如此：空 except。
    /// </summary>
    public int ServerStatus(string sServerName)
    {
        int Result = 0;

        try
        {
            bool boServerOnLine = false;
            for (int I = 0; I <= m_ServerList.Count - 1; I++)
            {
                TMsgServerInfo MsgServer = m_ServerList[I];
                if ((MsgServer.nServerIndex != 99) && (MsgServer.sServerName == sServerName))
                {
                    boServerOnLine = true;                      // 原文如此：不 break，扫完全表
                }
            }
            if (!boServerOnLine) return Result;                  // 原 :972 exit（Result 仍为 0）

            int nStatus = 0;
            for (int I = 0; I <= MasSockGlobals.UserLimit.Length - 1; I++)
            {
                if (MasSockGlobals.UserLimit[I].sServerName == sServerName)
                {
                    if (MasSockGlobals.UserLimit[I].nLimitCountMin <= MasSockGlobals.UserLimit[I].nLimitCountMax / 2)
                    {
                        nStatus = 1;                                               // 空闲
                        break;
                    }

                    if (MasSockGlobals.UserLimit[I].nLimitCountMin <=
                        MasSockGlobals.UserLimit[I].nLimitCountMax - (MasSockGlobals.UserLimit[I].nLimitCountMax / 5))
                    {
                        nStatus = 2;                                               // 良好
                        break;
                    }
                    if (MasSockGlobals.UserLimit[I].nLimitCountMin < MasSockGlobals.UserLimit[I].nLimitCountMax)
                    {
                        nStatus = 3;                                               // 繁忙
                        break;
                    }
                    if (MasSockGlobals.UserLimit[I].nLimitCountMin >= MasSockGlobals.UserLimit[I].nLimitCountMax)
                    {
                        nStatus = 4;                                               // 满员
                        break;
                    }
                }
            }

            Result = nStatus;
        }
        catch
        {
            LoginSrvShare.MainOutMessage("TFrmMasSoc.ServerStatus");   // 原文如此
        }
        return Result;
    }
}
