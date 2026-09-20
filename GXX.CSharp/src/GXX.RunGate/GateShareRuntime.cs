// =====================================================================================
// 源单元：Source\RunGate\GateShare.pas（Delphi 7，GBK）—— **判定谓词 / 定时 / 口令 / 版本 / GameCenter**
//   实测 LF = 3595 行。本文件覆盖：
//     :14        `tRunGate = 8`（SendGameCenterMsg 的 MakeLong 低位）
//     :250-255   TAntiPlugActionMode 的顺序（InitIntervals 下标真源；已在 uFrmGameSpeedLogic.cs）
//     :2523-2534 SendGameCenterMsg
//     :2992-3010 CheckInFYDenyIPList     :3012-3030 CheckInFYPassIPList
//     :3032-3075 IsBlockIP               :3077-3116 IsBlockMac
//     :3118-3168 IsConnLimited           :3170-3183 GetAttackCountOfIP
//     :3185-3198 GetConnectCountOfIP     :3200-3224 InitIntervals
//     :3389-3393 InputPassword           :3395-3473 InputPasswordEx
//     :3496-3502 MyGetTickCount
//     :3505-3536 GetFileVersionNumber    :3539-3543 GetFileVersionStr
//   `:3475-3494 EncodeRunGateMsg` 已由 `RunGateUtilsProtocol.cs:157-…` 覆盖，本文件不重复。
//   `:1506-1535 CheckInWhiteList` **死代码**（VERSION_TYPE = 2）→ 不移植，见图 GateShareLists.cs 文件头。
//
// ── 原文缺陷登记（照抄语义 + 差异断言）───────────────────────────────────────────────
//   H. [:3202-3214] `InitActionIntervals: array[TAntiPlugActionMode] of Word` 只有 **25** 个初始值，
//      而枚举有 **27** 个成员 → 最后两个（`amSpellConcurrent`=25、`amMoveConcurrent`=26）落回 **0**。
//      同时这 25 行的**行尾注释与下标系统性错位**（注释按"6 基础 + 2×10 转换 + 3 并发"的旧枚举序写，
//      而当前枚举在 6 个基础动作后先插了 `amTurn/amCutMeat` 相关的交错项）：
//        例：`:3207` 注释写"走路到魔法, 魔法到走路"，但下标 8/9 实为 `amRunToHit/amHitToRun`；
//            `:3214` 注释写"攻击并发, 魔法并发, 移动并发"，但下标 22/23/24 实为
//            `amMoveToCutMeat/amCutMeatToMove/amHitConcurrent`。
//      本实现**只按数组序**照抄（下标正确、注释错），并由
//      `InitIntervals_CommentsAreShifted_ValuesFollowArrayOrder` 差异断言固定。
//   I. [:3123] `IsConnLimited` 会**回写** `g_dwDefenseLevel`：`if g_dwDefenseLevel = 0 then g_dwDefenseLevel := 1;`
//      —— 该"修正"发生在开头的 `Lock` **之外**（`:3125` 才 Lock），且是全局可变副作用。
//   J. [:3135/:3147] `LongWord(AddressInfo.nIPCount1) >= g_dwIPCountLimit1 * g_dwDefenseLevel`
//      —— 乘法在 **LongWord 域**（回绕），比较也在 LongWord 域；`nIPCount1: Integer` 被强转。
//   K. [:3132/:3140/:3144/:3152] `MyGetTickCount` 在**一次调用里被求值 4 次**（原文写的是无括号的函数调用）。
//   L. [:2541] 原文取文件版本时 `Result.Minor := dwFileVersionMS;`（**不是** `shr 16`）
//      —— 把 32 位值直接赋给 `Word` 字段 → 低 16 位截断，结果与 `and $FFFF` 相同（巧合正确）；
//      `Result.Build := dwFileVersionLS;` 同理。
//   M. [:2953(SuFrmMain)] 非本文件。略。
//   N. [:2525-2533] `TCopyDataStruct.dwData` **未初始化**（只赋值 cbData/lpData）。
//      接收方 `uFrmMain.pas:2991` 读的是 `HiWord(MsgData.From)`（= wParam 高 16 位），
//      故 `dwData` 的栈垃圾**不影响**语义。托管侧显式置 0 并登记该观察。
// =====================================================================================

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using GXX.Core.Rtl;

namespace GXX.RunGate;

/// <summary>GateShare.pas 的判定谓词 / 定时 / 口令 / 版本 / GameCenter（1:1）。</summary>
public static class GateShareRuntime
{
    /// <summary>原文 :14 `tRunGate = 8;`（SendGameCenterMsg 的 MakeLong 低位）。</summary>
    public const int tRunGate = 8;

    /// <summary>原文 :15 `g_sUpdateTime = '2023-07-01';`。</summary>
    public const string g_sUpdateTime = "2023-07-01";

    // ==================================================================================
    // 防御名单命中判定
    // ==================================================================================

    /// <summary>原文 :2992-3010 `function CheckInFYDenyIPList(sIPAddr: string): Boolean;`
    /// 先查本地读取的 Deny 名单，未命中再查下载的 DownDeny 名单。判据是 `Find(...) &lt;&gt; nil`。</summary>
    public static bool CheckInFYDenyIPList(string sIPAddr)
    {
        bool Result;
        GateShareGlobals.g_FYDenyIPList.Lock();                          // 原 :2994
        try
        {
            Result = GateShareGlobals.g_FYDenyIPList.Find(sIPAddr) != null;   // 原 :2996
        }
        finally
        {
            GateShareGlobals.g_FYDenyIPList.UnLock();                    // 原 :2998
        }

        if (!Result)                                                     // 原 :3001
        {
            GateShareGlobals.g_FYDownDenyIPList.Lock();                  // 原 :3003
            try
            {
                Result = GateShareGlobals.g_FYDownDenyIPList.Find(sIPAddr) != null;   // 原 :3005
            }
            finally
            {
                GateShareGlobals.g_FYDownDenyIPList.UnLock();            // 原 :3007
            }
        }
        return Result;
    }

    /// <summary>原文 :3012-3030 `function CheckInFYPassIPList(sIPAddr: string): Boolean;`（同构）。</summary>
    public static bool CheckInFYPassIPList(string sIPAddr)
    {
        bool Result;
        GateShareGlobals.g_FYPassIPList.Lock();                          // 原 :3014
        try
        {
            Result = GateShareGlobals.g_FYPassIPList.Find(sIPAddr) != null;   // 原 :3016
        }
        finally
        {
            GateShareGlobals.g_FYPassIPList.UnLock();                    // 原 :3018
        }

        if (!Result)                                                     // 原 :3021
        {
            GateShareGlobals.g_FYDownPassIPList.Lock();                  // 原 :3023
            try
            {
                Result = GateShareGlobals.g_FYDownPassIPList.Find(sIPAddr) != null;   // 原 :3025
            }
            finally
            {
                GateShareGlobals.g_FYDownPassIPList.UnLock();            // 原 :3027
            }
        }
        return Result;
    }

    // ==================================================================================
    // 禁止名单命中判定
    // ==================================================================================

    /// <summary>原文 :3032-3075 `function IsBlockIP(sIPaddr: string): Boolean;`
    /// 三级短路：临时名单 → 永久名单 → IP 段区间。判据用 `FindIndex(...) &gt;= 0`（**不是** `&lt;&gt; -1`）。</summary>
    public static bool IsBlockIP(string sIPaddr)
    {
        bool Result;
        GateShareGlobals.g_TempIPList.Lock();                            // 原 :3038
        try
        {
            Result = GateShareGlobals.g_TempIPList.FindIndex(sIPaddr) >= 0;   // 原 :3040
        }
        finally
        {
            GateShareGlobals.g_TempIPList.UnLock();                      // 原 :3042
        }

        if (!Result)                                                     // 原 :3046
        {
            GateShareGlobals.g_BlockIPList.Lock();                       // 原 :3048
            try
            {
                Result = GateShareGlobals.g_BlockIPList.FindIndex(sIPaddr) >= 0;   // 原 :3050
            }
            finally
            {
                GateShareGlobals.g_BlockIPList.UnLock();                 // 原 :3052
            }
        }

        if (!Result)                                                     // 原 :3056
        {
            uint nIP = GateShareInet.IP2Long(sIPaddr);                   // 原 :3058

            GateShareGlobals.LockIPSectionList();                        // 原 :3060
            try
            {
                for (int I = 0; I < GateShareGlobals.IPSectionListCount; I++)   // 原 :3062
                {
                    var IPSection = GateShareGlobals.GetIPSection(I);    // 原 :3064
                    if ((nIP >= IPSection.nBeginAddr) && (nIP <= IPSection.nEndAddr))   // 原 :3065
                    {
                        Result = true;                                   // 原 :3067
                        break;                                           // 原 :3068
                    }
                }
            }
            finally
            {
                GateShareGlobals.UnLockIPSectionList();                  // 原 :3072
            }
        }
        return Result;
    }

    /// <summary>原文 :3077-3116 `function IsBlockMac(sMac: string): Boolean;`
    /// 四级短路：TempMac → BlockMac → FYDenyMAC → FYDownDenyMAC。
    /// 前两者是 `TSafeStringList`（`TStringList.IndexOf`，大小写**不敏感**）；
    /// 后两者是 `TSafeHashStringList`（`THashedStringList`，同样不敏感）。</summary>
    public static bool IsBlockMac(string sMac)
    {
        bool Result;
        GateShareGlobals.g_TempMacList.Lock();                           // 原 :3079
        try
        {
            Result = GateShareGlobals.g_TempMacList.IndexOf(sMac) >= 0;  // 原 :3081
        }
        finally
        {
            GateShareGlobals.g_TempMacList.UnLock();                     // 原 :3083
        }

        if (!Result)                                                     // 原 :3087
        {
            GateShareGlobals.g_BlockMacList.Lock();                      // 原 :3089
            try
            {
                Result = GateShareGlobals.g_BlockMacList.IndexOf(sMac) >= 0;   // 原 :3091
            }
            finally
            {
                GateShareGlobals.g_BlockMacList.UnLock();                // 原 :3093
            }
        }

        if (!Result)                                                     // 原 :3097
        {
            GateShareGlobals.g_FYDenyMACList.Lock();                     // 原 :3099
            try
            {
                Result = GateShareGlobals.g_FYDenyMACList.IndexOf(sMac) >= 0;   // 原 :3101
            }
            finally
            {
                GateShareGlobals.g_FYDenyMACList.UnLock();               // 原 :3103
            }
        }

        if (!Result)                                                     // 原 :3107
        {
            GateShareGlobals.g_FYDownDenyMACList.Lock();                 // 原 :3109
            try
            {
                Result = GateShareGlobals.g_FYDownDenyMACList.IndexOf(sMac) >= 0;   // 原 :3111
            }
            finally
            {
                GateShareGlobals.g_FYDownDenyMACList.UnLock();           // 原 :3113
            }
        }
        return Result;
    }

    // ==================================================================================
    // 连接频率限制
    // ==================================================================================

    /// <summary>`MyGetTickCount` 的接缝（默认 = `timeGetTime` 的等价物 `Environment.TickCount`）。</summary>
    public static Func<uint> TickProvider = DefaultTick;

    private static uint DefaultTick() => unchecked((uint)Environment.TickCount);

    /// <summary>原文 :3496-3502 `function MyGetTickCount: DWORD;`
    /// 原文用 `timeBeginPeriod(1) … timeGetTime … timeEndPeriod(1)` 提高定时器精度；
    /// 托管侧用 `Environment.TickCount`（同为 32 位毫秒计数，约 49.7 天回绕）。
    /// 精度调整（`timeBeginPeriod`）在托管侧无等价物且会带来全局副作用，**不复制**（已登记）。</summary>
    public static uint MyGetTickCount() => TickProvider();

    /// <summary>原文 :3118-3168 `function IsConnLimited(sIPaddr: string): Boolean;`
    /// ★ 缺陷 I：入口会回写全局 `g_dwDefenseLevel`；★ 缺陷 J：乘法在 LongWord 域；
    /// ★ 缺陷 K：`MyGetTickCount` 在一次调用里被求值 4 次（此处照抄）。
    ///
    /// 逻辑：对 `g_CurrIPList` 里的该 IP，`nCount++`；两个滑动窗口各自累计 `nIPCount1/2`，
    /// 超阈值或 `nCount &gt; g_nMaxConnOfIPaddr` 即返回 True；IP 不在表里则新建并置 `nCount := 1`。</summary>
    public static bool IsConnLimited(string sIPaddr)
    {
        bool Result = false;                                             // 原 :3122
        // 原 :3123（★ 缺陷 I：在 Lock 之前回写全局）
        if (FormGlobals.g_dwDefenseLevel == 0) FormGlobals.g_dwDefenseLevel = 1;

        GateShareGlobals.g_CurrIPList.Lock();                             // 原 :3125
        try
        {
            var AddressInfo = GateShareGlobals.g_CurrIPList.Find(sIPaddr);   // 原 :3127
            if (AddressInfo != null)                                     // 原 :3128
            {
                AddressInfo.nCount++;                                // 原 :3130

                // 原 :3132
                if (tick_diff(AddressInfo.dwIPCountTick1, MyGetTickCount()) < FormGlobals.g_dwIPCountLimitTime1)
                {
                    AddressInfo.nIPCount1++;                         // 原 :3134
                    // 原 :3135（★ 缺陷 J：LongWord 域乘法与比较）
                    if (unchecked((uint)AddressInfo.nIPCount1)
                        >= unchecked(FormGlobals.g_dwIPCountLimit1 * FormGlobals.g_dwDefenseLevel))
                        Result = true;
                }
                else
                {
                    AddressInfo.dwIPCountTick1 = MyGetTickCount();   // 原 :3140
                    AddressInfo.nIPCount1 = 0;                       // 原 :3141
                }

                // 原 :3144
                if (tick_diff(AddressInfo.dwIPCountTick2, MyGetTickCount()) < FormGlobals.g_dwIPCountLimitTime2)
                {
                    AddressInfo.nIPCount2++;                         // 原 :3146
                    // 原 :3147
                    if (unchecked((uint)AddressInfo.nIPCount2)
                        >= unchecked(FormGlobals.g_dwIPCountLimit2 * FormGlobals.g_dwDefenseLevel))
                        Result = true;
                }
                else
                {
                    AddressInfo.dwIPCountTick2 = MyGetTickCount();   // 原 :3152
                    AddressInfo.nIPCount2 = 0;                       // 原 :3153
                }

                // 原 :3156（`{* Integer(g_dwDefenseLevel)}` 被原文注释掉了）
                if (AddressInfo.nCount > FormGlobals.g_nMaxConnOfIPaddr)
                    Result = true;
            }
            else
            {
                AddressInfo = GateShareGlobals.g_CurrIPList.Add(sIPaddr);   // 原 :3161
                if (AddressInfo != null)
                    AddressInfo.nCount = 1;                          // 原 :3163
            }
        }
        finally
        {
            GateShareGlobals.g_CurrIPList.UnLock();                   // 原 :3166
        }
        return Result;
    }

    /// <summary>原文 :1458-1464 `function tick_diff(tick_start, tick_end: Cardinal): Cardinal;`
    /// （与 `RunGateUtilsHeartbeat.cs` 里的同名实现同源；此处为 32 位无回绕差值）。</summary>
    public static uint tick_diff(uint tick_start, uint tick_end)
        => tick_end >= tick_start ? tick_end - tick_start : uint.MaxValue - tick_start + tick_end;

    /// <summary>原文 :3170-3183 `function GetAttackCountOfIP(sIPaddr: string): Integer;`
    /// 只在 `g_AttackIPaddrList` 里查，未命中返回 0。</summary>
    public static int GetAttackCountOfIP(string sIPaddr)
    {
        int Result = 0;                                                  // 原 :3174
        GateShareGlobals.g_AttackIPaddrList.Lock();                       // 原 :3175
        try
        {
            var IPaddr = GateShareGlobals.g_AttackIPaddrList.Find(sIPaddr);   // 原 :3177
            if (IPaddr != null)
                Result = IPaddr.nAttackCount;                        // 原 :3179
        }
        finally
        {
            GateShareGlobals.g_AttackIPaddrList.UnLock();             // 原 :3181
        }
        return Result;
    }

    /// <summary>原文 :3185-3198 `function GetConnectCountOfIP(sIPaddr: string): Integer;`</summary>
    public static int GetConnectCountOfIP(string sIPaddr)
    {
        int Result = 0;                                                  // 原 :3189
        GateShareGlobals.g_CurrIPList.Lock();                             // 原 :3190
        try
        {
            var AddressInfo = GateShareGlobals.g_CurrIPList.Find(sIPaddr);   // 原 :3192
            if (AddressInfo != null)
                Result = AddressInfo.nCount;                         // 原 :3194
        }
        finally
        {
            GateShareGlobals.g_CurrIPList.UnLock();                   // 原 :3196
        }
        return Result;
    }

    // ==================================================================================
    // 速度间隔初始化
    // ==================================================================================

    /// <summary>原文 :3202-3214 的 `InitActionIntervals` 字面量 —— **只有 25 个值**（缺陷 H），
    /// 下标 25/26（`amSpellConcurrent`/`amMoveConcurrent`）在 Delphi 里落回 0。
    /// 本数组即为"下标 → 值"的真源（原文注释错位，见文件头缺陷 H）。</summary>
    public static readonly ushort[] InitActionIntervals =
    {
        1000, 1000, 1000,       // 原 :3203  下标 0,1,2
        1000, 0, 0,             // 原 :3204  下标 3,4,5
        540, 600,               // 原 :3206  下标 6,7
        540, 1200,              // 原 :3207  下标 8,9
        540, 1200,              // 原 :3208  下标 10,11
        250, 0,                 // 原 :3209  下标 12,13
        250, 0,                 // 原 :3210  下标 14,15
        250, 250,               // 原 :3211  下标 16,17
        0, 250,                 // 原 :3212  下标 18,19
        0, 250,                 // 原 :3213  下标 20,21
        0, 0, 0,                // 原 :3214  下标 22,23,24；25/26 无初始值 → 0
    };

    /// <summary>原文 :3200-3224 `procedure InitIntervals;`
    /// 把每个动作模式的 401 个间隔槽**全部**填成该模式的单值。</summary>
    public static void InitIntervals()
    {
        for (int ActionMode = 0; ActionMode < RunGateConst.ActionModeCount; ActionMode++)   // 原 :3219
        {
            // 原 :3222 —— 下标 25/26 的 InitActionIntervals 为 0（缺陷 H）
            ushort v = ActionMode < InitActionIntervals.Length ? InitActionIntervals[ActionMode] : (ushort)0;
            for (int I = 0; I < RunGateConst.SpeedIntervalsCount; I++)                       // 原 :3221
                FormGlobals.g_wActionSpeedIntervals[ActionMode][I] = v;
        }
    }

    // ==================================================================================
    // 口令输入
    // ==================================================================================

    /// <summary>口令输入框接缝。默认实现走 `MessageBoxSeam.InputQueryWithValue`
    /// （原文 :3395-3473 自建 `TForm` + `TLabel` + `TEdit{PasswordChar='*', MaxLength=255, SelectAll}`
    ///  + 确定/取消按钮 + `ShowModal = IDOK`）。
    /// ★ 与前任在 `uFrmSafeFilter` 的 `InputQueryEx` 处置一致：**自绘布局未复刻**，只保留三参数语义。
    /// 测试必须把 `MessageBoxSeam.UiEnabled` 置 false 或改本委托，否则真实模态循环会挂死 testhost。</summary>
    public static MessageBoxSeam.InputQueryHandler InputPasswordQuery = DefaultInputPasswordQuery;

    private static bool DefaultInputPasswordQuery(string caption, string prompt, ref string value)
        => MessageBoxSeam.InputQueryWithValue(caption, prompt, ref value);

    /// <summary>原文 :3389-3393 `function InputPassword(const ACaption, APrompt: string; ADefault: string = ''): string;`
    /// ★ 注意原文**忽略** `InputPasswordEx` 的布尔返回值：
    /// 取消时返回 `ADefault`，确定时返回输入值（靠 `var` 出参回写）。</summary>
    public static string InputPassword(string ACaption, string APrompt, string ADefault = "")
    {
        // 原 :3391 `Result := ADefault;` —— Delphi 里 nil 字符串等价于 '，托管侧显式归一
        string Result = ADefault ?? "";
        InputPasswordEx(ACaption, APrompt, ref Result);                  // 原 :3392
        return Result;
    }

    /// <summary>原文 :3395-3473 `function InputPasswordEx(const ACaption, APrompt: string; var Value: string): Boolean;`
    /// 确定 → `Value := Edit.Text; Result := True`；取消 → `Result := False` 且 `Value` 不变。</summary>
    public static bool InputPasswordEx(string ACaption, string APrompt, ref string Value)
    {
        bool Result = false;                                             // 原 :3413
        string local = Value ?? "";                                      // 原 :3442 `Edit.Text := Value;`
        if (InputPasswordQuery(ACaption, APrompt, ref local))            // 原 :3465 `if ShowModal = IDOK then`
        {
            Value = local;                                               // 原 :3467
            Result = true;                                               // 原 :3468
        }
        return Result;
    }

    // ==================================================================================
    // 文件版本
    // ==================================================================================

    /// <summary>原文 :37-43 `TVersionNumber = packed record Major, Minor, Release, Build: Word; end;`
    /// （packed，SizeOf = 8；见 `RunGateUtilsProtocol` 的布局对照）。</summary>
    public struct TVersionNumber
    {
        public ushort Major;
        public ushort Minor;
        public ushort Release;
        public ushort Build;
    }

    /// <summary>文件版本读取接缝（默认用 `System.Diagnostics.FileVersionInfo`）；
    /// 返回 null 表示"取不到版本信息"（对应原文 `GetFileVersionInfoSize = 0` / 异常 → 全 0）。</summary>
    public static Func<string, TVersionNumber?> FileVersionProvider = DefaultFileVersion;

    private static TVersionNumber? DefaultFileVersion(string fileName)
    {
        try
        {
            var vi = FileVersionInfo.GetVersionInfo(fileName);
            return new TVersionNumber
            {
                Major = (ushort)vi.FileMajorPart,
                Minor = (ushort)vi.FileMinorPart,
                Release = (ushort)vi.FileBuildPart,
                Build = (ushort)vi.FilePrivatePart,
            };
        }
        catch
        {
            return null;                                                  // 原 :3518 `if … = 0 then Exit;` / :3527 `except Exit;`
        }
    }

    /// <summary>原文 :3505-3536 `function GetFileVersionNumber(const FileName: string): TVersionNumber;`
    /// 文件不存在 / 无版本资源 / 读取异常 → **全 0**。
    /// ★ 缺陷 L：原文 `Result.Minor := dwFileVersionMS;` 与 `Result.Build := dwFileVersionLS;`
    /// 是把 32 位值直接赋给 `Word` 字段（低 16 位截断），**不是** `shr 16` —— 结果与 `and $FFFF` 相同。</summary>
    public static TVersionNumber GetFileVersionNumber(string FileName)
    {
        var Result = new TVersionNumber();                               // 原 :3513 `FillChar(Result, 0)`
        if (!File.Exists(FileName)) return Result;                       // 原 :3514-3515

        var v = FileVersionProvider(FileName);                            // 原 :3517-3525
        if (v == null) return Result;                                     // 原 :3518-3519
        return v.Value;                                                  // 原 :3529-3532
    }

    /// <summary>原文 :3539-3543 `function GetFileVersionStr(const FileName: string): string;`
    /// `Format('%d.%d.%d.%d', [Major, Minor, Release, Build])`。</summary>
    public static string GetFileVersionStr(string FileName)
    {
        var v = GetFileVersionNumber(FileName);                          // 原 :3541
        return v.Major + "." + v.Minor + "." + v.Release + "." + v.Build;   // 原 :3542
    }

    // ==================================================================================
    // GameCenter WM_COPYDATA
    // ==================================================================================

    /// <summary>`SendGameCenterMsg` 的发送接缝。
    /// 参数顺序：目标窗口句柄、wParam（`MakeLong(tRunGate, wIdent)`）、cbData（**字符数**，含结尾 NUL）、
    /// 负载（原文是 `StrCopy` 到 `GetMem` 缓冲的 AnsiString 副本，含结尾 #0）。</summary>
    public delegate void GameCenterCopyDataSender(IntPtr hwnd, int wParam, int cbData, byte[] data);

    /// <summary>默认实现走 `user32!SendMessageW/‌A` 的 `WM_COPYDATA`（见 <see cref="SendMessageCopyData"/>）。</summary>
    public static GameCenterCopyDataSender SendCopyData = SendMessageCopyData;

    /// <summary>原文 :2523-2534 `procedure SendGameCenterMsg(wIdent: Word; sSendMsg: string);`
    /// ★ 观察 N：`dwData` 未初始化（原文只写 cbData/lpData）；接收方读 wParam 的高 16 位，故无影响。
    /// 托管侧把 `dwData` 显式置 0（不复制栈垃圾）。</summary>
    public static void SendGameCenterMsg(ushort wIdent, string sSendMsg)
    {
        // 原 :2528 `nParam := MakeLong(Word(tRunGate), wIdent);`
        int nParam = tRunGate | (wIdent << 16);
        // 原 :2529 `SendData.cbData := Length(sSendMsg) + 1;`（**字符数** + 结尾 NUL）
        int cbData = (sSendMsg ?? "").Length + 1;

        // 原 :2530-2531 `GetMem(SendData.lpData, cbData); StrCopy(SendData.lpData, PChar(sSendMsg));`
        byte[] gbk = GXX.Core.EncodingInit.GBK.GetBytes(sSendMsg ?? "");
        var payload = new byte[gbk.Length + 1];
        Array.Copy(gbk, payload, gbk.Length);        // 末字节已是 0（StrCopy 的结尾 #0）

        // 原 :2532 `SendMessage(g_dwGameCenterHandle, WM_COPYDATA, nParam, Cardinal(@SendData));`
        SendCopyData(GateShareGlobals.g_dwGameCenterHandle, nParam, cbData, payload);
    }

    /// <summary>原文 :2525-2532 用到的 `TCopyDataStruct`。</summary>
    [StructLayout(LayoutKind.Sequential)]
    private struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        public IntPtr lpData;
    }

    private const int WM_COPYDATA = 0x004A;

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "SendMessageW")]
    private static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

    /// <summary>默认发送实现：真 `SendMessage(hwnd, WM_COPYDATA, wParam, @cds)`。
    /// 测试请替换 <see cref="SendCopyData"/>，不要调用本方法。</summary>
    public static void SendMessageCopyData(IntPtr hwnd, int wParam, int cbData, byte[] data)
    {
        IntPtr buf = IntPtr.Zero;
        try
        {
            buf = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, buf, data.Length);
            var cds = new COPYDATASTRUCT
            {
                dwData = IntPtr.Zero,     // ★ 原文字段未初始化；托管侧显式 0（观察 N）
                cbData = cbData,
                lpData = buf,
            };
            SendMessageW(hwnd, WM_COPYDATA, (IntPtr)wParam, ref cds);
        }
        finally
        {
            if (buf != IntPtr.Zero) Marshal.FreeHGlobal(buf);   // 原 :2533 `FreeMem(SendData.lpData);`
        }
    }
}
