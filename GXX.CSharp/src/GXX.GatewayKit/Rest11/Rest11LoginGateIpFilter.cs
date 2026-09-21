using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core.Util;

namespace GXX.GatewayKit.Rest11;

/// <summary>
/// `Source/LoginGate/IPAddrFilter.pas` → `Rest11LoginGateIpFilter.cs`
/// 1:1 逐字移植（:40-380）：
///   永久/临时 IP 黑名单（`TStringList` + `Objects` 存活 inet_addr 结果）的加载/保存/增删查、
///   IP 段过滤（`TIPArea` 低/高端，`Misc.ReverseIP` 后比较）、每 IP 连接数限制、每 IP 换 ID 频率限制。
///
/// <para>
/// ⚠ 与既有设施的关系（**并存**，不合并、不"统一"——`SelGateIPAddrFilter.cs:17-25` 的三条差异要求）：
/// <list type="number">
///   <item>`GXX.GatewayKit.GateService.CheckIP`（GateService.cs:203-222）只做"黑名单 HashSet + 计数"，
///         且判据是 `rec.Count &gt; Max`（**先自增再比**）；原版是 **两张表**（永久 + 临时）
///         且判据是 `PerIPAddr.Count + 1 &gt; Max`（**超限时不自增**：:193-196）。本类照抄原版。</item>
///   <item>`m_fCheckNullSession = false` 时 `OverConnectOfIP`/`DeleteConnectOfIP`/`ClearConnectOfIP`
///         直接 `Exit` 不做任何事（:184、:214、:242）；`GateService` **没有**这个开关。</item>
///   <item>IP 段过滤（`LoadBlockIPAreaList` :254 / `IsBlockIPArea` :315）与换 ID 频率限制
///         （`CheckNewIDOfIP` :337）`GatewayKit` **完全没有**。</item>
///   <item>`GXX.SelGate.CSelGateIPFilter` 是**同一份 `.pas`（与 LoginGate 副本 SHA256 相同）的 SelGate 投影**，
///         它绑 `SelGate.CConfigMgr`/`SelGateGlobals`/`SelGateProtocol._STR_*`；本类绑 LoginGate 的
///         `Rest11LoginGateConfig`/`IRest11EnforcementChannel`/`_STR_*`。两份投影**同时存在**、
///         **互不引用**，以保证 SelGate 语义一字不改（`SelGate.Tests` 全绿）。</item>
/// </list>
/// </para>
///
/// <para>
/// 原文缺陷（照抄，未顺手修）：
/// <list type="number">
///   <item>`LoadBlockIPAreaList`（:278-279）把 `inet_addr` 的结果先 cast 成 `DWORD` 再 `ReverseIP`；
///         `inet_addr` 失败返回 `INADDR_NONE`（$FFFFFFFF），`ReverseIP($FFFFFFFF)` 仍是 $FFFFFFFF，
///         所以 :280/:282 的 `= INADDR_NONE` 检查**恰好**仍能拦住非法段。照抄（含该巧合）。</item>
///   <item>`AddToBlockIPList(szIP)`（:79-89）用 `g_BlockIPList.IndexOf(szIP)`（**字符串**去重），
///         而 `AddToBlockIPList(nIP)`（:91-112）用 `Objects[i]`（**整数**去重）——两条重载的
///         去重键不同 ⇒ 同一地址可能以不同写法各占一行。照抄。</item>
///   <item>`LoadBlockIPList`（:49-50）先 `Clear` 再 `LoadFromFile`；若文件**不存在**，
///         `sList.SaveToFile` 先建空文件（:47），随后 `LoadFromFile` 读到空表。照抄。</item>
///   <item>`ClearConnectOfIP`（:246-248）遍历后 `Clear`，遍历体只有 `DisPose`（无其他作用）。照抄。</item>
/// </list>
/// </para>
/// </summary>
public class Rest11LoginGateIpFilter
{
    /// <summary>`Windows.INADDR_NONE`（LongInt 视角 = -1）。</summary>
    public const int INADDR_NONE = Rest11LoginGateNet.INADDR_NONE;

    // ---- IPAddrFilter.pas:11-15 全局量 ----
    public readonly List<TPerIPAddr> g_ConnectOfIPList = new();                   // :11 TList of pTPerIPAddr
    public readonly List<TNewIDAddr> g_NewIDOfIPList = new();                     // :12 TList of pTNewIDAddr
    public readonly TStringList g_BlockIPList = new();                            // :13
    public readonly TStringList g_TempBlockIPList = new();                        // :14
    public readonly TStringList g_BlockIPAreaList = new();                        // :15

    private readonly object _connectOfIPLock = new();                             // :9  g_ConnectOfIPLock
    private readonly object _newIDOfIPLock = new();                               // :10 g_NewIDOfIPLock

    /// <summary>`_STR_BLOCK_FILE` / `_STR_BLOCK_AREA_FILE`（`Protocol.pas:21-22`）的相对路径解析基准。</summary>
    public string BaseDirectory { get; set; } = ".";

    /// <summary>配置源（原版全程读 `g_pConfig` 的开关与阈值）。</summary>
    public Rest11LoginGateConfig? Config { get; set; }

    /// <summary>`GetTickCount` 注入点（测试用固定时钟替代；默认 `DelphiRTL.GetTickCount`）。</summary>
    public Func<uint> TickCount { get; set; } = GXX.Core.Rtl.DelphiRTL.GetTickCount;

    public string BlockFilePath => Resolve(BaseDirectory, Rest11LoginGateConstants._STR_BLOCK_FILE);
    public string BlockAreaFilePath => Resolve(BaseDirectory, Rest11LoginGateConstants._STR_BLOCK_AREA_FILE);

    /// <summary>把 `Protocol.pas` 的 `'.\Xxx'` 相对路径按 <see cref="BaseDirectory"/> 展开。</summary>
    public static string Resolve(string baseDir, string relative)
        => Path.IsPathRooted(relative)
            ? relative
            : Path.GetFullPath(Path.Combine(baseDir, relative.Replace('\\', Path.DirectorySeparatorChar)));

    // =====================================================================================
    // IPAddrFilter.pas:40-61 LoadBlockIPList
    // =====================================================================================
    public void LoadBlockIPList()
    {
        var sList = new TStringList();                                    // :45
        string file = BlockFilePath;
        if (!File.Exists(file))                                           // :46
            sList.SaveToFile(file);                                       // :47（写出空文件）

        g_BlockIPList.Clear();                                            // :49
        sList.LoadFromFile(file);                                         // :50
        for (int i = 0; i <= sList.Count - 1; i++)                        // :51
        {
            if (sList[i] == "")                                           // :53
                continue;                                                 // :54
            int nIP = Rest11LoginGateNet.InetAddr(sList[i]);              // :55
            if (nIP == INADDR_NONE)                                       // :56
                continue;                                                 // :57
            g_BlockIPList.AddObject(sList[i], nIP);                       // :58
        }
    }

    // =====================================================================================
    // IPAddrFilter.pas:63-77 SaveBlockIPList（跳过空串行）
    // =====================================================================================
    public void SaveBlockIPList()
    {
        var sList = new TStringList();                                    // :68
        for (int i = 0; i <= g_BlockIPList.Count - 1; i++)                // :69
        {
            if (g_BlockIPList[i] == "")                                   // :71
                continue;                                                 // :72
            sList.Add(g_BlockIPList[i]);                                  // :73
        }
        sList.SaveToFile(BlockFilePath);                                  // :75
    }

    // =====================================================================================
    // IPAddrFilter.pas:79-89 AddToBlockIPList(szIP) —— IndexOf 字符串去重 + inet_addr 校验
    // =====================================================================================
    public void AddToBlockIPList(string szIP)
    {
        if (g_BlockIPList.IndexOf(szIP) < 0)                              // :83
        {
            int nIP = Rest11LoginGateNet.InetAddr(szIP);                  // :85
            if (nIP != INADDR_NONE)                                       // :86
                g_BlockIPList.AddObject(szIP, nIP);                       // :87
        }
    }

    // =====================================================================================
    // IPAddrFilter.pas:91-112 AddToBlockIPList(nIP) —— Objects 整数去重 + inet_ntoa
    // =====================================================================================
    public void AddToBlockIPList(int nIP)
    {
        bool fExists = false;                                             // :97
        for (int i = 0; i <= g_BlockIPList.Count - 1; i++)                // :98
        {
            if (ToInt(g_BlockIPList.GetObject(i)) == nIP)                 // :100
            {
                fExists = true;                                           // :102
                break;                                                    // :103
            }
        }
        if (!fExists)                                                     // :106
        {
            string? pszIP = Rest11LoginGateNet.InetNtoa(nIP);             // :108
            if (pszIP != null)                                            // :109
                g_BlockIPList.AddObject(pszIP, nIP);                      // :110
        }
    }

    // =====================================================================================
    // IPAddrFilter.pas:114-124 AddToTempBlockIPList(szIP)
    // =====================================================================================
    public void AddToTempBlockIPList(string szIP)
    {
        if (g_TempBlockIPList.IndexOf(szIP) < 0)                          // :118
        {
            int nIP = Rest11LoginGateNet.InetAddr(szIP);                  // :120
            if (nIP != INADDR_NONE)                                       // :121
                g_TempBlockIPList.AddObject(szIP, nIP);                   // :122
        }
    }

    // =====================================================================================
    // IPAddrFilter.pas:126-147 AddToTempBlockIPList(nIP)
    // =====================================================================================
    public void AddToTempBlockIPList(int nIP)
    {
        bool fExists = false;                                             // :132
        for (int i = 0; i <= g_TempBlockIPList.Count - 1; i++)            // :133
        {
            if (ToInt(g_TempBlockIPList.GetObject(i)) == nIP)             // :135
            {
                fExists = true;                                           // :137
                break;                                                    // :138
            }
        }
        if (!fExists)                                                     // :141
        {
            string? pszIP = Rest11LoginGateNet.InetNtoa(nIP);             // :143
            if (pszIP != null)                                            // :144
                g_TempBlockIPList.AddObject(pszIP, nIP);                  // :145
        }
    }

    // =====================================================================================
    // IPAddrFilter.pas:149-176 IsBlockIP
    // 永久表命中 Exit（:161），临时表命中 Break（:172）—— 原文如此，两条路径写法不同
    // =====================================================================================
    public bool IsBlockIP(int nRemoteIP)
    {
        bool Result = false;                                              // :153
        if (g_BlockIPList.Count > 0)                                      // :154
        {
            for (int i = 0; i <= g_BlockIPList.Count - 1; i++)            // :156
            {
                if (nRemoteIP == ToInt(g_BlockIPList.GetObject(i)))       // :158
                {
                    Result = true;                                        // :160
                    return Result;                                        // :161 Exit
                }
            }
        }
        if (g_TempBlockIPList.Count > 0)                                  // :165
        {
            for (int i = 0; i <= g_TempBlockIPList.Count - 1; i++)        // :167
            {
                if (nRemoteIP == ToInt(g_TempBlockIPList.GetObject(i)))   // :169
                {
                    Result = true;                                        // :171
                    break;                                                // :172
                }
            }
        }
        return Result;
    }

    // =====================================================================================
    // IPAddrFilter.pas:178-207 OverConnectOfIP
    // 判定为 `Count + 1 > Max`，**超限时不自增**（原文如此 :193-196）
    // =====================================================================================
    public bool OverConnectOfIP(int Addr)
    {
        bool Result = false;                                              // :183
        if (!(Config?.m_fCheckNullSession ?? false))                      // :184（m_fCheckNullSession 为假直接 Exit）
            return Result;

        lock (_connectOfIPLock)                                           // :186 EnterCriticalSection(g_ConnectOfIPLock)
        {
            for (int i = 0; i <= g_ConnectOfIPList.Count - 1; i++)        // :188
            {
                TPerIPAddr PerIPAddr = g_ConnectOfIPList[i];              // :190
                if (PerIPAddr.IPaddr == Addr)                             // :191
                {
                    if (PerIPAddr.Count + 1 > (Config?.m_nMaxConnectOfIP ?? 0)) // :193
                    {
                        Result = true;                                    // :194
                    }
                    else
                    {
                        PerIPAddr.Count++;                                // :196 Inc(PerIPAddr.Count)
                        g_ConnectOfIPList[i] = PerIPAddr;                 // 结构体语义：回写以模拟指针
                    }
                    return Result;                                        // :197 Exit
                }
            }
            var created = new TPerIPAddr();                               // :200 New(PerIPAddr)
            created.IPaddr = Addr;                                        // :201
            created.Count = 1;                                            // :202
            g_ConnectOfIPList.Add(created);                               // :203
            return Result;
        }
    }

    // =====================================================================================
    // IPAddrFilter.pas:209-236 DeleteConnectOfIP
    // =====================================================================================
    public void DeleteConnectOfIP(int Addr)
    {
        if (!(Config?.m_fCheckNullSession ?? false))                      // :214
            return;
        lock (_connectOfIPLock)                                           // :216
        {
            for (int i = 0; i <= g_ConnectOfIPList.Count - 1; i++)        // :218
            {
                TPerIPAddr PerIPAddr = g_ConnectOfIPList[i];              // :220
                if (PerIPAddr.IPaddr == Addr)                             // :221
                {
                    if (PerIPAddr.Count > 0)                              // :223
                        PerIPAddr.Count--;                                // :224 Dec(PerIPAddr.Count)
                    if (PerIPAddr.Count <= 0)                             // :225
                    {
                        // :227 DisPose(PerIPAddr) —— 托管无需释放
                        g_ConnectOfIPList.RemoveAt(i);                    // :228 g_ConnectOfIPList.Delete(i)
                    }
                    else
                    {
                        g_ConnectOfIPList[i] = PerIPAddr;                 // 结构体语义回写
                    }
                    break;                                                // :230
                }
            }
        }
    }

    // =====================================================================================
    // IPAddrFilter.pas:238-252 ClearConnectOfIP
    // =====================================================================================
    public void ClearConnectOfIP()
    {
        if (!(Config?.m_fCheckNullSession ?? false))                      // :242
            return;
        lock (_connectOfIPLock)                                           // :244
        {
            for (int i = 0; i <= g_ConnectOfIPList.Count - 1; i++)        // :246
            {
                // :247 DisPose(pTPerIPAddr(g_ConnectOfIPList[i])) —— 托管无需释放
            }
            g_ConnectOfIPList.Clear();                                    // :248
        }
    }

    // =====================================================================================
    // IPAddrFilter.pas:254-297 LoadBlockIPAreaList
    // =====================================================================================
    public void LoadBlockIPAreaList()
    {
        var sList = new TStringList();                                    // :263
        string file = BlockAreaFilePath;
        if (!File.Exists(file))                                           // :264
            sList.SaveToFile(file);                                       // :265

        for (int i = 0; i <= g_BlockIPAreaList.Count - 1; i++)            // :267
        {
            // :268 DisPose(PInt64(g_BlockIPAreaList.Objects[i])) —— 托管无需释放
        }
        g_BlockIPAreaList.Clear();                                        // :269

        sList.LoadFromFile(file);                                         // :271
        for (int i = 0; i <= sList.Count - 1; i++)                        // :272
        {
            string szIPArea = sList[i];                                   // :274
            if (szIPArea == "")                                           // :275
                continue;                                                 // :276

            // :277 szIPHigh := GetValidStr3(szIPArea, szIPLow, ['-'])
            var szIPLowRef = new Rest11Ref<string>("");
            string szIPHigh = Rest11LoginGateNet.GetValidStr3(szIPArea, szIPLowRef, new[] { '-' });
            string szIPLow = szIPLowRef.Value;

            uint dwIPLow = Rest11LoginGateNet.ReverseIP(unchecked((uint)Rest11LoginGateNet.InetAddr(szIPLow)));   // :278
            uint dwIPHigh = Rest11LoginGateNet.ReverseIP(unchecked((uint)Rest11LoginGateNet.InetAddr(szIPHigh))); // :279
            if (dwIPLow == unchecked((uint)INADDR_NONE))                  // :280
                continue;                                                 // :281
            if (dwIPHigh == unchecked((uint)INADDR_NONE))                 // :282
                continue;                                                 // :283
            if (dwIPLow > dwIPHigh)                                       // :284
            {
                uint dwtmp = dwIPLow;                                     // :286
                dwIPLow = dwIPHigh;                                       // :287
                dwIPHigh = dwtmp;                                         // :288
            }

            var pIPArea = new TIPArea();                                  // :291 New(pIPArea)
            pIPArea.Low = dwIPLow;                                        // :292
            pIPArea.High = dwIPHigh;                                      // :293
            g_BlockIPAreaList.AddObject(szIPArea, pIPArea);               // :294
        }
    }

    // =====================================================================================
    // IPAddrFilter.pas:299-313 SaveBlockIPAreaList
    // =====================================================================================
    public void SaveBlockIPAreaList()
    {
        var sList = new TStringList();                                    // :304
        for (int i = 0; i <= g_BlockIPAreaList.Count - 1; i++)            // :305
        {
            if (g_BlockIPAreaList[i] == "")                               // :307
                continue;                                                 // :308
            sList.Add(g_BlockIPAreaList[i]);                              // :309
        }
        sList.SaveToFile(BlockAreaFilePath);                              // :311
    }

    // =====================================================================================
    // IPAddrFilter.pas:315-335 IsBlockIPArea —— 用 ReverseIP 后的 DWORD 落在 [Low, High] 闭区间
    // =====================================================================================
    public bool IsBlockIPArea(int nRemoteIP)
    {
        bool Result = false;                                              // :321
        if (g_BlockIPAreaList.Count > 0)                                  // :322
        {
            uint dwReverseIP = Rest11LoginGateNet.ReverseIP((uint)nRemoteIP); // :324 Misc.ReverseIP(DWORD(nRemoteIP))
            for (int i = 0; i <= g_BlockIPAreaList.Count - 1; i++)        // :325
            {
                var pIPArea = (TIPArea)g_BlockIPAreaList.GetObject(i);    // :327
                if (dwReverseIP >= pIPArea.Low && dwReverseIP <= pIPArea.High) // :328
                {
                    Result = true;                                        // :330
                    return Result;                                        // :331 Exit
                }
            }
        }
        return Result;
    }

    // =====================================================================================
    // IPAddrFilter.pas:337-380 CheckNewIDOfIP
    // 4 秒窗口（:352）、超阈值返回 True（:356）、窗口过期则刷新 tick 并 Dec（:360-367）
    // =====================================================================================
    public bool CheckNewIDOfIP(int Addr)
    {
        bool Result = false;                                              // :342
        if (!(Config?.m_fCheckNewIDOfIP ?? false))                        // :343
            return Result;

        lock (_newIDOfIPLock)                                             // :345
        {
            for (int i = 0; i <= g_NewIDOfIPList.Count - 1; i++)          // :347
            {
                TNewIDAddr NewIDAddr = g_NewIDOfIPList[i];                // :349
                if (NewIDAddr.IPaddr == Addr)                             // :350
                {
                    if (TickCount() - NewIDAddr.dwIDCountTick < 4 * 1000)  // :352
                    {
                        NewIDAddr.Count++;                                // :354 Inc(NewIDAddr.Count)
                        if (NewIDAddr.Count > (Config?.m_nCheckNewIDOfIP ?? 0)) // :355
                            Result = true;                                // :356
                        g_NewIDOfIPList[i] = NewIDAddr;
                    }
                    else
                    {
                        NewIDAddr.dwIDCountTick = TickCount();            // :360
                        if (NewIDAddr.Count > 0)                          // :361
                            NewIDAddr.Count--;                            // :362 Dec(NewIDAddr.Count)
                        if (NewIDAddr.Count <= 0)                         // :363
                        {
                            // :365 DisPose(NewIDAddr)
                            g_NewIDOfIPList.RemoveAt(i);                  // :366 g_NewIDOfIPList.Delete(i)
                        }
                        else
                        {
                            g_NewIDOfIPList[i] = NewIDAddr;
                        }
                    }
                    return Result;                                        // :369 Exit
                }
            }
            var created = new TNewIDAddr();                               // :372 New(NewIDAddr)
            created.IPaddr = Addr;                                        // :373
            created.Count = 1;                                            // :374
            created.dwIDCountTick = TickCount();                          // :375
            g_NewIDOfIPList.Add(created);                                 // :376
            return Result;
        }
    }

    /// <summary>`TStringList.Objects[i]` 的 `Integer(TObject)` 还原。</summary>
    private static int ToInt(object o)
    {
        if (o is int i) return i;
        if (o is uint u) return unchecked((int)u);
        return Convert.ToInt32(o, System.Globalization.CultureInfo.InvariantCulture);
    }
}

/// <summary>
/// `Protocol.pas:9-23` 中 LoginGate 副本特有的字符串常量（`_STR_NOW_START` = "正在启动**登陆**网关..."，
/// SelGate 副本是"角色网关"）与文件路径常量。
/// </summary>
public static class Rest11LoginGateConstants
{
    // Protocol.pas:10-14
    public const string _STR_GRID_INDEX = "网关";              // :10
    public const string _STR_GRID_IP = "网关地址";              // :11
    public const string _STR_GRID_PORT = "端口";                // :12
    public const string _STR_GRID_CONNECT_STATUS = "连接状态";   // :13
    public const string _STR_GRID_ONLINE_USER = "通讯";          // :14

    // Protocol.pas:16-18 —— LoginGate 副本用"登陆网关"（SelGate 副本是"角色网关"）
    public const string _STR_NOW_START = "正在启动登陆网关...";   // :16
    public const string _STR_STARTED = "登陆网关启动完成...";     // :17
    public const string _STR_NOW_STOP = "在线";                  // :18（原文如此：名为 NOW_STOP 值却是"在线"）

    // Protocol.pas:20-23
    public const string _STR_CONFIG_FILE = @".\Config.ini";              // :20
    public const string _STR_BLOCK_FILE = @".\BlockIPList.txt";          // :21
    public const string _STR_BLOCK_AREA_FILE = @".\BlockIPAreaList.txt"; // :22
    public const string _STR_USER_NAME_FILTER_FILE = @".\NewChrNameFilter.txt"; // :23

    // Protocol.pas:25-29（WM_USER = $0400 = 1024）
    public const int WM_USER = 0x0400;
    public const int _IDM_SERVERSOCK_MSG = WM_USER + 1000;              // :25 = 2024
    public const int _IDM_TIMER_STARTSERVICE = _IDM_SERVERSOCK_MSG + 1; // :26 = 2025
    public const int _IDM_TIMER_STOPSERVICE = _IDM_SERVERSOCK_MSG + 2;  // :27 = 2026
    public const int _IDM_TIMER_KEEP_ALIVE = _IDM_SERVERSOCK_MSG + 3;   // :28 = 2027
    public const int _IDM_TIMER_THREAD_INFO = _IDM_SERVERSOCK_MSG + 4;  // :29 = 2028

    /// <summary>`Protocol.pas:32 FIRST_PAKCET_MAX_LEN = 0080`（原文八进制）= 64 十进制。</summary>
    public const int FIRST_PAKCET_MAX_LEN = 64;
}
