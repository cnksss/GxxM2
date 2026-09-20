// =====================================================================================
// 源单元：Source\RunGate\GateShare.pas（Delphi 7，GBK）—— **单元级全局量（本轮新增部分）**
//   实测 LF = 3595 行。本文件覆盖 :596-1345 里 **FormGlobals 尚未登记**的那部分，
//   以及 :3545-3582 `initialization` 段的等价构造/初值。
//
// ★ 与 `uFrmGameSpeedLogic.cs` 的 `FormGlobals` 的分工（**必须一起看**）：
//   `FormGlobals`（窗体族车道先落地）已持有窗体族读写的那一批 g_*；本文件只补 GateShare 的
//   名单/日志/验证/反外挂状态。**同一 Delphi 全局量只允许在一处定义**，本文件头部列出交叉引用：
//
//     已在 FormGlobals（本文件不重复）：
//        g_sIniFileName / g_WordFilterList / g_sWordFilterFileName / g_ProcessBlackList
//        g_ProcessBlacklistStr / g_ProcessBlacklistMD5 / g_MagicCDList / g_MagicCDListFileName
//        g_wActionSpeedIntervals / g_boSendSpeedIntervalsToClient / g_sActionIntervalsFileNames
//        g_boFilterSayMsg / g_FilterSayMsgMode / g_WarnSayMsg / g_boFilterSayTriggerScript
//        g_nMaxConnOfIPaddr / g_BlockMethod / g_dwAttackTick / g_nAttackCount /
//        g_nMaxClientPacketSize / g_nMaxClientPacketCount / g_boKickOverPacketSize /
//        g_dwKeepConnectTimeOut / g_boCheckClientPacketLegal / g_nCheckClientPacketCount /
//        g_boSayMsgControl / g_dwSayMaxLen / g_dwSayTime / g_dwSayMaxCount / g_dwSayDisableTime /
//        g_dwIPCountLimitTime1 / g_dwIPCountLimit1 / g_dwIPCountLimitTime2 / g_dwIPCountLimit2 /
//        g_dwDefenseLevel / g_boDefenseToLevel1 / g_dwDefenseToLevel1 / g_boResotreDefense /
//        g_dwResotreDefense / g_boAutoClearTemp / g_dwAutoClearTemp / g_boAddAllToTemp /
//        g_dwAddAllToTemp / g_boOpenCheckClient / g_CheckClientFailBlockMethod /
//        g_sFYReadDenyIPFile / g_dwFYReadDenyIPTime / g_sFYDownDenyIPUrl / g_dwFYDownDenyIPTime /
//        g_sFYReadPassIPFile / g_dwFYReadPassIPTime / g_sFYDownPassIPUrl / g_dwFYDownPassIPTime /
//        g_sFYReadDenyMACFile / g_dwFYReadDenyMACTime / g_sFYDownDenyMACUrl / g_dwFYDownDenyMACTick* /
//        g_boAntiPlugAutoUpdateCheck / g_wAntiPlugUpdateCheckInterval / g_sAntiPlugUpdateConfigUrl /
//        g_nLogClientPacketType / g_boLogClientPacket / g_sLogClientPakcetUserFile / g_LogClientPacketUser
//        g_OnlyWhiteListLink / g_sHitIntervalsFileName / g_dwHitIntervals / g_Config / g_DefaultConfig
//
// ★ 已知口径差异（登记，未擅自"修正" FormGlobals）：
//   FormGlobals 把一批标量置成 0/False/""（它是"窗体测试的新建状态"，由主窗体 LoadConfig 填充），
//   而原文 `:1218-1277` 的**内联初值**是非 0（如 `g_nMaxConnOfIPaddr = 50`、`g_dwDefenseLevel = 1`、
//   `g_dwIPCountLimitTime1 = 1000`、`g_sFYReadDenyIPFile = 'D:\MirServer\...\KickList.txt'`）。
//   本轮新增的全局量一律**按原文内联初值**给出（见各字段注释），测试需要时显式覆盖。
//   两者的差异由 `GateShareGlobalsTests.OriginalInlineDefaults_MatchSourceLiterals` 固定。
//
// ★ `g_IPSectionList: TSafeList`（:1130）的处置：
//   原文类型 `TSafeList` 来自 `Common/IocpCommon.pas`（不在本车道文件分区，且**本工程尚无同名
//   C# 类型**）。为避免跨车道重名事故（已发生 5 次），本文件**不声明** `TSafeList`，
//   改为 `List<TIPSection>` + `GateShareGlobals.LockIPSectionList()/UnLockIPSectionList()`
//   这一对显式门；语义与 `TSafeList` 的 Lock/Add/Clear/Count/Items 使用子集一致。
// =====================================================================================

using System;
using System.Collections.Generic;
using System.Threading;

namespace GXX.RunGate;

/// <summary>GateShare.pas 中 **FormGlobals 未登记** 的单元级全局量（1:1，按原文内联初值）。</summary>
public static class GateShareGlobals
{
    // ================= :1118-1119 日志缓冲 =================
    /// <summary>原文 :1118 `g_MainLogStrings: TSafeStringList;`（initialization :3557 构造）。</summary>
    public static readonly TSafeStringList g_MainLogStrings = new TSafeStringList();

    /// <summary>原文 :1119 `g_IOCPLogStrings: TSafeStringList;`（initialization :3559 构造）。</summary>
    public static readonly TSafeStringList g_IOCPLogStrings = new TSafeStringList();

    /// <summary>原文 :1172 `g_btShowLogLevel: Byte = 3;`（AddMainLogMsg 的门限）。</summary>
    public static byte g_btShowLogLevel = 3;

    // ================= :1127-1131 连接/IP 名单 =================
    /// <summary>原文 :1127 `g_CurrIPList: TAddressListEx;`（当前连接的 IP）。</summary>
    public static readonly TAddressListEx g_CurrIPList = new TAddressListEx();

    /// <summary>原文 :1128 `g_TempIPList: TAddressList;`（临时禁止连接 IP 列表）。</summary>
    public static readonly TAddressList g_TempIPList = new TAddressList();

    /// <summary>原文 :1129 `g_BlockIPList: TAddressList;`（禁止连接 IP 列表）。</summary>
    public static readonly TAddressList g_BlockIPList = new TAddressList();

    /// <summary>原文 :1131 `g_AttackIPaddrList: TAddressList;`（攻击 IP 临时列表）。</summary>
    public static readonly TAddressList g_AttackIPaddrList = new TAddressList();

    /// <summary>原文 :1130 `g_IPSectionList: TSafeList;`（过滤 IP 段）—— 见文件头处置说明。</summary>
    public static readonly List<TIPSection> g_IPSectionList = new List<TIPSection>();

    private static readonly object IPSectionGate = new object();

    /// <summary>`TSafeList.Lock` 的等价门。</summary>
    public static void LockIPSectionList() => Monitor.Enter(IPSectionGate);

    /// <summary>`TSafeList.UnLock` 的等价门。</summary>
    public static void UnLockIPSectionList() => Monitor.Exit(IPSectionGate);

    /// <summary>`TSafeList.Count`。</summary>
    public static int IPSectionListCount => g_IPSectionList.Count;

    /// <summary>`TSafeList.Items[Index]`（越界返回 null，与其它容器的判据风格一致）。</summary>
    public static TIPSection GetIPSection(int Index)
        => (Index >= 0) && (Index <= g_IPSectionList.Count - 1) ? g_IPSectionList[Index] : null;

    /// <summary>`TSafeList.Add(IPSection)`。</summary>
    public static void AddIPSection(TIPSection item) => g_IPSectionList.Add(item);

    /// <summary>`TSafeList.Clear`（`for ... Dispose` 在托管侧由 GC 承担）。</summary>
    public static void ClearIPSectionList() => g_IPSectionList.Clear();

    // ================= :1133-1134 MAC 名单 =================
    /// <summary>原文 :1133 `g_TempMacList: TSafeStringList;`。</summary>
    public static readonly TSafeStringList g_TempMacList = new TSafeStringList();

    /// <summary>原文 :1134 `g_BlockMacList: TSafeStringList;`。</summary>
    public static readonly TSafeStringList g_BlockMacList = new TSafeStringList();

    // ================= :1287-1315 防御设置（FY）名单 =================
    /// <summary>原文 :1290 `g_FYDenyIPList: TAddressList;`。</summary>
    public static readonly TAddressList g_FYDenyIPList = new TAddressList();

    /// <summary>原文 :1289 `g_dwFYReadDenyIPTick: LongWord;`。</summary>
    public static uint g_dwFYReadDenyIPTick;

    /// <summary>原文 :1295 `g_FYPassIPList: TAddressList;`。</summary>
    public static readonly TAddressList g_FYPassIPList = new TAddressList();

    /// <summary>原文 :1294 `g_dwFYReadPassIPTick: LongWord;`。</summary>
    public static uint g_dwFYReadPassIPTick;

    /// <summary>原文 :1300 `g_FYDenyMACList: TSafeHashStringList;`。</summary>
    public static readonly TSafeHashStringList g_FYDenyMACList = new TSafeHashStringList();

    /// <summary>原文 :1299 `g_dwFYReadDenyMACTick: LongWord;`。</summary>
    public static uint g_dwFYReadDenyMACTick;

    /// <summary>原文 :1305 `g_FYDownDenyIPList: TAddressList;`。</summary>
    public static readonly TAddressList g_FYDownDenyIPList = new TAddressList();

    /// <summary>原文 :1304 `g_dwFYDownDenyIPTick: LongWord;`。</summary>
    public static uint g_dwFYDownDenyIPTick;

    /// <summary>原文 :1310 `g_FYDownPassIPList: TAddressList;`。</summary>
    public static readonly TAddressList g_FYDownPassIPList = new TAddressList();

    /// <summary>原文 :1309 `g_dwFYDownPassIPTick: LongWord;`。</summary>
    public static uint g_dwFYDownPassIPTick;

    /// <summary>原文 :1315 `g_FYDownDenyMACList: TSafeHashStringList;`。</summary>
    public static readonly TSafeHashStringList g_FYDownDenyMACList = new TSafeHashStringList();

    /// <summary>原文 :1314 `g_dwFYDownDenyMACTick: LongWord;`。</summary>
    public static uint g_dwFYDownDenyMACTick;

    // ================= :1319 DB 地址表 =================
    /// <summary>原文 :1319 `g_DBAddressList: TStringList;`（复用 GXX.Core 的 TStringList）。</summary>
    public static readonly GXX.Core.Util.TStringList g_DBAddressList = new GXX.Core.Util.TStringList();

    // ================= :630 / :628 用户名单 =================
    /// <summary>原文 :630 `g_VerifyFailUserList: TSafeHashStringList;`（初始化 :3565）。</summary>
    public static readonly TSafeHashStringList g_VerifyFailUserList = new TSafeHashStringList();

    /// <summary>原文 :628 `g_LockUserList: TSafeHashStringList;`（初始化 :3563）。</summary>
    public static readonly TSafeHashStringList g_LockUserList = new TSafeHashStringList();

    /// <summary>原文 :1182 `g_LoginMACPlayerList: TSafeHashStringList;`（初始化 :3581，**注意 :3581 没有传名字**）。</summary>
    public static readonly TSafeHashStringList g_LoginMACPlayerList = new TSafeHashStringList();

    /// <summary>原文 :1180 `g_boOneMACLimitePlayer: Boolean = False;`。</summary>
    public static bool g_boOneMACLimitePlayer = false;

    /// <summary>原文 :1181 `g_nOneMACLimitePlayerCount: Integer = 3;`。</summary>
    public static int g_nOneMACLimitePlayerCount = 3;

    // ================= :1148-1159 验证码/免验证名单 =================
    /// <summary>原文 :1148 `g_boOpenVerifyCode: Boolean = False;`。</summary>
    public static bool g_boOpenVerifyCode = false;

    /// <summary>原文 :1149 `g_nVerifyCodeErrCount: Integer = 3;`。</summary>
    public static int g_nVerifyCodeErrCount = 3;

    /// <summary>原文 :1150 `g_nVerifyCodeRefreshCount: Integer = 4;`。</summary>
    public static int g_nVerifyCodeRefreshCount = 4;

    /// <summary>原文 :1151 `g_nVerifyCodeWaitTime: Integer = 60;`。</summary>
    public static int g_nVerifyCodeWaitTime = 60;

    /// <summary>原文 :1152 `g_dwVerifyCodeInterval1: LongWord = 30;`。</summary>
    public static uint g_dwVerifyCodeInterval1 = 30;

    /// <summary>原文 :1153 `g_dwVerifyCodeInterval2: LongWord = 50;`。</summary>
    public static uint g_dwVerifyCodeInterval2 = 50;

    /// <summary>原文 :1154 `g_dwVerifySuccessAddInterval: LongWord = 0;`。</summary>
    public static uint g_dwVerifySuccessAddInterval = 0;

    /// <summary>原文 :1155 `g_boVerifyFailTriggerScript: Boolean = False;`。</summary>
    public static bool g_boVerifyFailTriggerScript = false;

    /// <summary>原文 :1156 `g_boVerifyFailLoginVerify: Boolean = False;`。</summary>
    public static bool g_boVerifyFailLoginVerify = false;

    /// <summary>原文 :1157 `g_boVerifyCodeExcludeMap: Boolean = True;`。</summary>
    public static bool g_boVerifyCodeExcludeMap = true;

    /// <summary>原文 :1158 `g_sVerifyCodeExcludeMapFileName: string = '';`（initialization :3577 赋值为 exe 目录 + 'VerifyCodeExcludeMap.txt'）。</summary>
    public static string g_sVerifyCodeExcludeMapFileName = "";

    /// <summary>原文 :1159 `g_VerifyCodeMapList: TSafeHashStringList;`。</summary>
    public static readonly TSafeHashStringList g_VerifyCodeMapList = new TSafeHashStringList();

    /// <summary>原文 :1161 `g_boAutoLoadNoVerifyChrList: Boolean = False;`。</summary>
    public static bool g_boAutoLoadNoVerifyChrList = false;

    /// <summary>原文 :1162 `g_sLoadNoVerifyChrListFile: string = 'D:\MirServer\Mir200\Envir\QuestDiary\白名单用户.txt';`。原文**硬编码绝对路径**，照抄。</summary>
    public static string g_sLoadNoVerifyChrListFile = @"D:\MirServer\Mir200\Envir\QuestDiary\白名单用户.txt";

    /// <summary>原文 :1163 `g_nAutoLoadNoVerifyChrListInterval: Integer = 300;`。</summary>
    public static int g_nAutoLoadNoVerifyChrListInterval = 300;

    /// <summary>原文 :1164 `g_dwAutoLoadNoVerifyChrListTick: LongWord;`。</summary>
    public static uint g_dwAutoLoadNoVerifyChrListTick;

    /// <summary>原文 :1165 `g_LoadNoVerifyChrList: TSafeHashStringList;`。</summary>
    public static readonly TSafeHashStringList g_LoadNoVerifyChrList = new TSafeHashStringList();

    // ================= :1214 / :1140 / :1209 路径 =================
    /// <summary>原文 :1214 `g_sPluginDir: string;`（initialization :3567）。</summary>
    public static string g_sPluginDir = "";

    /// <summary>原文 :1140 `g_ScreenshotPath: string;`（initialization :3573）。</summary>
    public static string g_ScreenshotPath = "";

    /// <summary>原文 :1209 `g_sLogClientPacketDir: string;`（initialization :3569）。</summary>
    public static string g_sLogClientPacketDir = "";

    // ================= :1216 过滤替换字符 =================
    /// <summary>原文 :1216 `g_sReplaceWord: Char = '*';`。</summary>
    public static char g_sReplaceWord = '*';

    // ================= :1230 GameCenter 句柄 =================
    /// <summary>原文 :1230 `g_dwGameCenterHandle: THandle;`（未赋值时 0 → SendMessage 到 0 号窗口）。</summary>
    public static IntPtr g_dwGameCenterHandle = IntPtr.Zero;

    // ================= :1144-1146 / :1170 端口与地址 =================
    /// <summary>原文 :1142 `g_sTitleName: string = '游戏网关';`。</summary>
    public static string g_sTitleName = "游戏网关";

    /// <summary>原文 :1143 `g_sServerAddr: string = '127.0.0.1';`。</summary>
    public static string g_sServerAddr = "127.0.0.1";

    /// <summary>原文 :1144 `g_wdServerPort: Word = 5000;`。</summary>
    public static ushort g_wdServerPort = 5000;

    /// <summary>原文 :1145 `g_sGateAddr: string = '0.0.0.0';`。</summary>
    public static string g_sGateAddr = "0.0.0.0";

    /// <summary>原文 :1146 `g_wdGatePort: Word = 7200;`。</summary>
    public static ushort g_wdGatePort = 7200;

    /// <summary>原文 :1170 `g_wdDBPort: Word = 27201;`。</summary>
    public static ushort g_wdDBPort = 27201;

    // ================= :597-622 客户端反外挂模块状态 =================
    /// <summary>原文 :597 `g_ClientAntiPlugDllBlockSize: Integer = 0;`。</summary>
    public static int g_ClientAntiPlugDllBlockSize = 0;

    /// <summary>原文 :598 `g_ClientAntiPlugDllSendInterval: Integer = 0;`。</summary>
    public static int g_ClientAntiPlugDllSendInterval = 0;

    /// <summary>原文 :600 `g_ClientAntiPlugDllSize: Integer;`（0）。</summary>
    public static int g_ClientAntiPlugDllSize;

    /// <summary>原文 :601 `g_ClientAntiPlugVersion: LongWord = 0;`。</summary>
    public static uint g_ClientAntiPlugVersion = 0;

    /// <summary>原文 :602 `g_ClientAntiPlugDllString: string;`。</summary>
    public static string g_ClientAntiPlugDllString = "";

    /// <summary>原文 :603 `g_ClientAntiPlugDllStringCRC: LongWord = 0;`。</summary>
    public static uint g_ClientAntiPlugDllStringCRC = 0;

    /// <summary>原文 :604 `g_ClientAntiPlugDllBlockCount: Integer = 0;`。</summary>
    public static int g_ClientAntiPlugDllBlockCount = 0;

    /// <summary>原文 :606 `g_ClientAntiPlugStream: TMemoryStream;`（延迟到首次使用再构造）。</summary>
    public static System.IO.MemoryStream g_ClientAntiPlugStream;

    // 原文 :608 `g_boAntiPlugAutoUpdateCheck` / :609 `g_wAntiPlugUpdateCheckInterval` /
    // :610 `g_dwAntiPlugUpdateCheckTick` / :611 `g_sAntiPlugUpdateConfigUrl` ——
    // 这 4 个量已在 `FormGlobals` 登记（窗体族车道先落地），本文件**不重复定义**；
    // 但原文 :609 的内联初值是 **5**，FormGlobals 里是 1（差异见 `Original_wAntiPlugUpdateCheckInterval`）。

    /// <summary>原文 :614 `g_sRunGatePlusDllName: string = 'RunGatePlug.dll';`。</summary>
    public static string g_sRunGatePlusDllName = "RunGatePlug.dll";

    /// <summary>原文 :615 `g_RunGatePlugDllHandle: THandle = 0;`。</summary>
    public static IntPtr g_RunGatePlugDllHandle = IntPtr.Zero;

    /// <summary>原文 :609 `g_wAntiPlugUpdateCheckInterval: Word = 5;`。
    /// ★ 注意与 `FormGlobals.g_wAntiPlugUpdateCheckInterval`（值 1，窗体族用）**数值不同** —— 原文只有一份（:609 = 5）。</summary>
    public const ushort Original_wAntiPlugUpdateCheckInterval = 5;

    /// <summary>原文 :610 `g_dwAntiPlugUpdateCheckTick: LongWord;`。</summary>
    public static uint g_dwAntiPlugUpdateCheckTick;

    /// <summary>原文 :1218 `g_nMaxConnOfIPaddr: Integer = 50;` 的**原文内联初值**
    /// （实际存放处是 `FormGlobals.g_nMaxConnOfIPaddr`，那里被置 0；此处只登记真值）。</summary>
    public const int Original_nMaxConnOfIPaddr = 50;

    /// <summary>原文 :1264 `g_dwDefenseLevel: LongWord = 1;` 的原文内联初值。</summary>
    public const uint Original_dwDefenseLevel = 1;

    /// <summary>初值审计：原文 `var` 段里带内联初值的那些量（供测试逐项比对）。</summary>
    public static readonly (string Name, string Literal)[] OriginalInlineLiterals =
    {
        ("g_btShowLogLevel", "3"),                 // :1172
        ("g_boOneMACLimitePlayer", "False"),       // :1180
        ("g_nOneMACLimitePlayerCount", "3"),       // :1181
        ("g_boOpenVerifyCode", "False"),           // :1148
        ("g_nVerifyCodeErrCount", "3"),            // :1149
        ("g_nVerifyCodeRefreshCount", "4"),        // :1150
        ("g_nVerifyCodeWaitTime", "60"),           // :1151
        ("g_dwVerifyCodeInterval1", "30"),         // :1152
        ("g_dwVerifyCodeInterval2", "50"),         // :1153
        ("g_dwVerifySuccessAddInterval", "0"),     // :1154
        ("g_boVerifyFailTriggerScript", "False"),  // :1155
        ("g_boVerifyFailLoginVerify", "False"),    // :1156
        ("g_boVerifyCodeExcludeMap", "True"),      // :1157
        ("g_sVerifyCodeExcludeMapFileName", "''"), // :1158
        ("g_boAutoLoadNoVerifyChrList", "False"),  // :1161
        ("g_sLoadNoVerifyChrListFile", @"'D:\MirServer\Mir200\Envir\QuestDiary\白名单用户.txt'"),   // :1162
        ("g_nAutoLoadNoVerifyChrListInterval", "300"),  // :1163
        ("g_sReplaceWord", "'*'"),                 // :1216
        ("g_sTitleName", "'游戏网关'"),             // :1142
        ("g_sServerAddr", "'127.0.0.1'"),          // :1143
        ("g_wdServerPort", "5000"),                // :1144
        ("g_sGateAddr", "'0.0.0.0'"),              // :1145
        ("g_wdGatePort", "7200"),                  // :1146
        ("g_wdDBPort", "27201"),                   // :1170
        ("g_ClientAntiPlugDllBlockSize", "0"),     // :597
        ("g_ClientAntiPlugDllSendInterval", "0"),  // :598
        ("g_ClientAntiPlugVersion", "0"),          // :601
        ("g_ClientAntiPlugDllStringCRC", "0"),     // :603
        ("g_ClientAntiPlugDllBlockCount", "0"),    // :604
        ("g_sRunGatePlusDllName", "'RunGatePlug.dll'"),  // :614
        ("g_nMaxConnOfIPaddr(原文)", "50"),         // :1218
        ("g_dwDefenseLevel(原文)", "1"),            // :1264
    };

    /// <summary>测试辅助：把本文件登记的全局量复位到原文内联初值（不含 FormGlobals 那一批）。</summary>
    public static void ResetForTest()
    {
        g_btShowLogLevel = 3;
        g_MainLogStrings.Clear();
        g_IOCPLogStrings.Clear();

        g_CurrIPList.Clear();
        g_TempIPList.Clear();
        g_BlockIPList.Clear();
        g_AttackIPaddrList.Clear();
        ClearIPSectionList();

        g_TempMacList.Clear();
        g_BlockMacList.Clear();

        g_FYDenyIPList.Clear(); g_dwFYReadDenyIPTick = 0;
        g_FYPassIPList.Clear(); g_dwFYReadPassIPTick = 0;
        g_FYDenyMACList.Clear(); g_dwFYReadDenyMACTick = 0;
        g_FYDownDenyIPList.Clear(); g_dwFYDownDenyIPTick = 0;
        g_FYDownPassIPList.Clear(); g_dwFYDownPassIPTick = 0;
        g_FYDownDenyMACList.Clear(); g_dwFYDownDenyMACTick = 0;

        g_DBAddressList.Clear();

        g_VerifyFailUserList.Clear();
        g_LockUserList.Clear();
        g_LoginMACPlayerList.Clear();
        g_boOneMACLimitePlayer = false;
        g_nOneMACLimitePlayerCount = 3;

        g_boOpenVerifyCode = false;
        g_nVerifyCodeErrCount = 3;
        g_nVerifyCodeRefreshCount = 4;
        g_nVerifyCodeWaitTime = 60;
        g_dwVerifyCodeInterval1 = 30;
        g_dwVerifyCodeInterval2 = 50;
        g_dwVerifySuccessAddInterval = 0;
        g_boVerifyFailTriggerScript = false;
        g_boVerifyFailLoginVerify = false;
        g_boVerifyCodeExcludeMap = true;
        g_sVerifyCodeExcludeMapFileName = "";
        g_VerifyCodeMapList.Clear();

        g_boAutoLoadNoVerifyChrList = false;
        g_sLoadNoVerifyChrListFile = @"D:\MirServer\Mir200\Envir\QuestDiary\白名单用户.txt";
        g_nAutoLoadNoVerifyChrListInterval = 300;
        g_dwAutoLoadNoVerifyChrListTick = 0;
        g_LoadNoVerifyChrList.Clear();

        g_sPluginDir = "";
        g_ScreenshotPath = "";
        g_sLogClientPacketDir = "";
        g_sReplaceWord = '*';
        g_dwGameCenterHandle = IntPtr.Zero;

        g_sTitleName = "游戏网关";
        g_sServerAddr = "127.0.0.1";
        g_wdServerPort = 5000;
        g_sGateAddr = "0.0.0.0";
        g_wdGatePort = 7200;
        g_wdDBPort = 27201;

        g_ClientAntiPlugDllBlockSize = 0;
        g_ClientAntiPlugDllSendInterval = 0;
        g_ClientAntiPlugDllSize = 0;
        g_ClientAntiPlugVersion = 0;
        g_ClientAntiPlugDllString = "";
        g_ClientAntiPlugDllStringCRC = 0;
        g_ClientAntiPlugDllBlockCount = 0;
        g_ClientAntiPlugStream = null;
        g_sRunGatePlusDllName = "RunGatePlug.dll";
        g_RunGatePlugDllHandle = IntPtr.Zero;
        g_dwAntiPlugUpdateCheckTick = 0;
    }
}
