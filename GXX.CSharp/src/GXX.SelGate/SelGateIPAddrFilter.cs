using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using GXX.Core;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.SelGate;

/// <summary>
/// SelGate IPAddrFilter.pas → SelGateIPAddrFilter.cs
/// 1:1 逐字移植（:40-380）：
///   永久/临时 IP 黑名单（TStringList + Objects 存 inet_addr 结果）的加载/保存/增删查、
///   IP 段过滤（TIPArea 低/高端，ReverseIP 后比较）、每 IP 连接数限制、每 IP 换 ID 频率限制。
///
/// 与 GatewayKit 的差异（必须保留，不得被 GateService 的简化实现"统一"掉）：
///   1) GatewayKit.GateService.CheckIP（GateService.cs:203-222）只做"黑名单 HashSet + 计数"；
///      原版把黑名单分成 **永久 g_BlockIPList + 临时 g_TempBlockIPList 两张表**，
///      且 Objects[i] 存的是 **inet_addr 结果（网络序整数）**，比较用的是 nRemoteIP 整数；
///   2) 原版 OverConnectOfIP 的判定是 `PerIPAddr.Count + 1 > Max`（:193）——超限时**不**自增；
///      GatewayKit 是 `rec.Count > Max`（先 ++ 再比，GateService.cs:214-219）；
///   3) 原版 m_fCheckNullSession=false 时 OverConnectOfIP/DeleteConnectOfIP/ClearConnectOfIP
///      直接 Exit 不做任何事（:184、:214、:242）；GatewayKit 无此开关；
///   4) 原版有 IP 段过滤与换 ID 频率限制（CheckNewIDOfIP :337-380），GatewayKit 完全没有。
///
/// 全局量 g_ConnectOfIPList/g_NewIDOfIPList/g_BlockIPList/g_TempBlockIPList/g_BlockIPAreaList
/// （IPAddrFilter.pas:8-15）以实例字段表达；Delphi 的初始化发生在 unit initialization（:382-389），
/// 因此 <see cref="SelGateIPFilterGlobal"/> 在静态构造时创建默认实例。
/// </summary>
public class CSelGateIPFilter
{
    /// <summary>WinSock INADDR_NONE = $FFFFFFFF（LongInt 视角 = -1）。</summary>
    public const int INADDR_NONE = -1;

    // ---- 全局量 IPAddrFilter.pas:11-15 ----
    public readonly List<SelPerIPAddr> g_ConnectOfIPList = new();   // :11 TList of pTPerIPAddr
    public readonly List<TNewIDAddr> g_NewIDOfIPList = new();       // :12 TList of pTNewIDAddr
    public readonly GXX.Core.Util.TStringList g_BlockIPList = new();      // :13
    public readonly GXX.Core.Util.TStringList g_TempBlockIPList = new();  // :14
    public readonly GXX.Core.Util.TStringList g_BlockIPAreaList = new();  // :15

    private readonly object _connectLock = new();  // :9 g_ConnectOfIPLock
    private readonly object _newIdLock = new();    // :10 g_NewIDOfIPLock

    /// <summary>相对文件名 _STR_BLOCK_FILE / _STR_BLOCK_AREA_FILE 的解析基准目录。</summary>
    public string BaseDirectory { get; set; } = ".";

    /// <summary>配置源（IPAddrFilter 全程读 g_pConfig 的开关与阈值）。</summary>
    public CConfigMgr? Config { get; set; }

    /// <summary>GetTickCount 注入点（测试可替换为固定时钟）。</summary>
    public Func<uint> TickCount { get; set; } = DelphiRTL.GetTickCount;

    public CSelGateIPFilter()
    {
        // IPAddrFilter.pas:382-389 initialization：Delphi 在建全局量时并不会加载文件，
        // 加载由 FuncForComm.StartService（:199）/ AppMain 显式调用 LoadBlockIPList。
    }

    public string BlockFilePath => Path.Combine(BaseDirectory, ".\\BlockIPList.txt");       // _STR_BLOCK_FILE
    public string BlockAreaFilePath => Path.Combine(BaseDirectory, ".\\BlockIPAreaList.txt"); // _STR_BLOCK_AREA_FILE

    /// <summary>Resolve: 把 Protocol.pas 的 '.\Xxx' 相对路径按 <see cref="BaseDirectory"/> 展开。</summary>
    public static string Resolve(string baseDir, string relative)
        => Path.IsPathRooted(relative) ? relative : Path.GetFullPath(Path.Combine(baseDir, relative.Replace('\\', Path.DirectorySeparatorChar)));

    // =====================================================================================
    // IPAddrFilter.pas:40-61 LoadBlockIPList
    // =====================================================================================
    public void LoadBlockIPList()
    {
        var sList = new GXX.Core.Util.TStringList();                                  // :45
        string file = Resolve(BaseDirectory, SelGateProtocol._STR_BLOCK_FILE);        // _STR_BLOCK_FILE = '.\BlockIPList.txt'
        if (!File.Exists(file))                                                       // :46
            sList.SaveToFile(file);                                                   // :47（写出空文件）

        g_BlockIPList.Clear();                                                        // :49
        sList.LoadFromFile(file);                                                     // :50
        for (int i = 0; i <= sList.Count - 1; i++)                                    // :51
        {
            if (sList[i] == "")                                                       // :53
                continue;                                                             // :54
            int nIP = InetAddr(sList[i]);                                             // :55
            if (nIP == INADDR_NONE)                                                   // :56
                continue;                                                             // :57
            g_BlockIPList.AddObject(sList[i], nIP);                                   // :58
        }
    }

    // =====================================================================================
    // IPAddrFilter.pas:63-77 SaveBlockIPList（跳过空串行，GBK CRLF 落盘）
    // =====================================================================================
    public void SaveBlockIPList()
    {
        var sList = new GXX.Core.Util.TStringList();                                  // :68
        for (int i = 0; i <= g_BlockIPList.Count - 1; i++)                            // :69
        {
            if (g_BlockIPList[i] == "")                                               // :71
                continue;                                                             // :72
            sList.Add(g_BlockIPList[i]);                                              // :73
        }
        string file = Resolve(BaseDirectory, SelGateProtocol._STR_BLOCK_FILE);
        sList.SaveToFile(file);                                                       // :75
    }

    // =====================================================================================
    // IPAddrFilter.pas:79-89 AddToBlockIPList(szIP) —— 先 IndexOf 去重，再 inet_addr 校验
    // =====================================================================================
    public void AddToBlockIPList(string szIP)
    {
        if (g_BlockIPList.IndexOf(szIP) < 0)                                          // :83
        {
            int nIP = InetAddr(szIP);                                                 // :85
            if (nIP != INADDR_NONE)                                                   // :86
                g_BlockIPList.AddObject(szIP, nIP);                                   // :87
        }
    }

    // =====================================================================================
    // IPAddrFilter.pas:91-112 AddToBlockIPList(nIP) —— 按 Objects 里的整数去重，再 inet_ntoa
    // =====================================================================================
    public void AddToBlockIPList(int nIP)
    {
        bool fExists = false;                                                         // :97
        for (int i = 0; i <= g_BlockIPList.Count - 1; i++)                            // :98
        {
            if (ToInt(g_BlockIPList.GetObject(i)) == nIP)                             // :100
            {
                fExists = true;                                                       // :102
                break;                                                                // :103
            }
        }
        if (!fExists)                                                                 // :106
        {
            string? pszIP = InetNtoa(nIP);                                            // :108
            if (pszIP != null)                                                        // :109
                g_BlockIPList.AddObject(pszIP, nIP);                                  // :110
        }
    }

    // =====================================================================================
    // IPAddrFilter.pas:114-124 AddToTempBlockIPList(szIP)
    // =====================================================================================
    public void AddToTempBlockIPList(string szIP)
    {
        if (g_TempBlockIPList.IndexOf(szIP) < 0)                                      // :118
        {
            int nIP = InetAddr(szIP);                                                 // :120
            if (nIP != INADDR_NONE)                                                   // :121
                g_TempBlockIPList.AddObject(szIP, nIP);                               // :122
        }
    }

    // =====================================================================================
    // IPAddrFilter.pas:126-147 AddToTempBlockIPList(nIP)
    // =====================================================================================
    public void AddToTempBlockIPList(int nIP)
    {
        bool fExists = false;                                                         // :132
        for (int i = 0; i <= g_TempBlockIPList.Count - 1; i++)                        // :133
        {
            if (ToInt(g_TempBlockIPList.GetObject(i)) == nIP)                         // :135
            {
                fExists = true;                                                       // :137
                break;                                                                // :138
            }
        }
        if (!fExists)                                                                 // :141
        {
            string? pszIP = InetNtoa(nIP);                                            // :143
            if (pszIP != null)                                                        // :144
                g_TempBlockIPList.AddObject(pszIP, nIP);                              // :145
        }
    }

    // =====================================================================================
    // IPAddrFilter.pas:149-176 IsBlockIP
    // 永久表命中 Exit（:161），临时表命中 Break（:172）—— 原文如此，两条路径写法不同
    // =====================================================================================
    public bool IsBlockIP(int nRemoteIP)
    {
        bool Result = false;                                                          // :153
        if (g_BlockIPList.Count > 0)                                                  // :154
        {
            for (int i = 0; i <= g_BlockIPList.Count - 1; i++)                        // :156
            {
                if (nRemoteIP == ToInt(g_BlockIPList.GetObject(i)))                   // :158
                {
                    Result = true;                                                    // :160
                    return Result;                                                    // :161 Exit
                }
            }
        }
        if (g_TempBlockIPList.Count > 0)                                              // :165
        {
            for (int i = 0; i <= g_TempBlockIPList.Count - 1; i++)                    // :167
            {
                if (nRemoteIP == ToInt(g_TempBlockIPList.GetObject(i)))               // :169
                {
                    Result = true;                                                    // :171
                    break;                                                            // :172
                }
            }
        }
        return Result;
    }

    // =====================================================================================
    // IPAddrFilter.pas:178-207 OverConnectOfIP
    // 注意：判定为 Count + 1 > Max，超限时 **不** 自增（原文如此 :193-196）
    // =====================================================================================
    public bool OverConnectOfIP(int Addr)
    {
        bool Result = false;                                                          // :183
        if (!(Config?.m_fCheckNullSession ?? false))                                  // :184（m_fCheckNullSession 为假直接 Exit）
            return Result;

        lock (_connectLock)                                                           // :186
        {
            for (int i = 0; i <= g_ConnectOfIPList.Count - 1; i++)                    // :188
            {
                SelPerIPAddr PerIPAddr = g_ConnectOfIPList[i];                        // :190
                if (PerIPAddr.IPaddr == Addr)                                         // :191
                {
                    if (PerIPAddr.Count + 1 > (Config?.m_nMaxConnectOfIP ?? 0))       // :193
                    {
                        Result = true;                                                // :194
                    }
                    else
                    {
                        PerIPAddr.Count++;                                            // :196 Inc(PerIPAddr.Count)
                        g_ConnectOfIPList[i] = PerIPAddr; // 记录为结构体，回写以模拟指针语义
                    }
                    return Result;                                                    // :197 Exit
                }
            }
            var created = new SelPerIPAddr();                                         // :200 New(PerIPAddr)
            created.IPaddr = Addr;                                                    // :201
            created.Count = 1;                                                        // :202
            g_ConnectOfIPList.Add(created);                                           // :203
            return Result;
        }
    }

    // =====================================================================================
    // IPAddrFilter.pas:209-236 DeleteConnectOfIP
    // =====================================================================================
    public void DeleteConnectOfIP(int Addr)
    {
        if (!(Config?.m_fCheckNullSession ?? false))                                  // :214
            return;
        lock (_connectLock)                                                           // :216
        {
            for (int i = 0; i <= g_ConnectOfIPList.Count - 1; i++)                    // :218
            {
                SelPerIPAddr PerIPAddr = g_ConnectOfIPList[i];                        // :220
                if (PerIPAddr.IPaddr == Addr)                                         // :221
                {
                    if (PerIPAddr.Count > 0)                                          // :223
                        PerIPAddr.Count--;                                            // :224 Dec(PerIPAddr.Count)
                    if (PerIPAddr.Count <= 0)                                         // :225
                    {
                        // :227 DisPose(PerIPAddr) —— 托管无需释放
                        g_ConnectOfIPList.RemoveAt(i);                                // :228
                    }
                    else
                    {
                        g_ConnectOfIPList[i] = PerIPAddr;                             // 指针语义回写
                    }
                    break;                                                            // :230
                }
            }
        }
    }

    // =====================================================================================
    // IPAddrFilter.pas:238-252 ClearConnectOfIP
    // =====================================================================================
    public void ClearConnectOfIP()
    {
        if (!(Config?.m_fCheckNullSession ?? false))                                  // :242
            return;
        lock (_connectLock)                                                           // :244
        {
            for (int i = 0; i <= g_ConnectOfIPList.Count - 1; i++)                    // :246
            {
                // :247 DisPose(pTPerIPAddr(...))
            }
            g_ConnectOfIPList.Clear();                                                // :248
        }
    }

    // =====================================================================================
    // IPAddrFilter.pas:254-297 LoadBlockIPAreaList
    // =====================================================================================
    public void LoadBlockIPAreaList()
    {
        var sList = new GXX.Core.Util.TStringList();                                  // :263
        string file = Resolve(BaseDirectory, SelGateProtocol._STR_BLOCK_AREA_FILE);
        if (!File.Exists(file))                                                       // :264
            sList.SaveToFile(file);                                                   // :265

        for (int i = 0; i <= g_BlockIPAreaList.Count - 1; i++)                        // :267
        {
            // :268 DisPose(PInt64(g_BlockIPAreaList.Objects[i]))
        }
        g_BlockIPAreaList.Clear();                                                    // :269

        sList.LoadFromFile(file);                                                     // :271
        for (int i = 0; i <= sList.Count - 1; i++)                                    // :272
        {
            string szIPArea = sList[i];                                               // :274
            if (szIPArea == "")                                                       // :275
                continue;                                                             // :276
            // :277 szIPHigh := GetValidStr3(szIPArea, szIPLow, ['-'])
            // 用逐字复刻版而非 GXX.Core.Util.HUtil32.GetValidStr3（后者未跳过分隔符，见 SelGatePacketRule.GetValidStr3Ex 注释）
            (string szIPLow, string szIPHigh) = CSelPacketRule.GetValidStr3Ex(szIPArea, new[] { '-' });
            uint dwIPLow = ReverseIP(unchecked((uint)InetAddr(szIPLow)));             // :278
            uint dwIPHigh = ReverseIP(unchecked((uint)InetAddr(szIPHigh)));           // :279
            if (dwIPLow == unchecked((uint)INADDR_NONE))                              // :280
                continue;                                                             // :281
            if (dwIPHigh == unchecked((uint)INADDR_NONE))                             // :282
                continue;                                                             // :283
            if (dwIPLow > dwIPHigh)                                                   // :284
            {
                uint dwtmp = dwIPLow;                                                 // :286
                dwIPLow = dwIPHigh;                                                   // :287
                dwIPHigh = dwtmp;                                                     // :288
            }

            var pIPArea = new TIPArea();                                              // :291 New(pIPArea)
            pIPArea.Low = dwIPLow;                                                    // :292
            pIPArea.High = dwIPHigh;                                                  // :293
            g_BlockIPAreaList.AddObject(szIPArea, pIPArea);                           // :294
        }
    }

    // =====================================================================================
    // IPAddrFilter.pas:299-313 SaveBlockIPAreaList
    // =====================================================================================
    public void SaveBlockIPAreaList()
    {
        var sList = new GXX.Core.Util.TStringList();                                  // :304
        for (int i = 0; i <= g_BlockIPAreaList.Count - 1; i++)                        // :305
        {
            if (g_BlockIPAreaList[i] == "")                                           // :307
                continue;                                                             // :308
            sList.Add(g_BlockIPAreaList[i]);                                          // :309
        }
        string file = Resolve(BaseDirectory, SelGateProtocol._STR_BLOCK_AREA_FILE);
        sList.SaveToFile(file);                                                       // :311
    }

    // =====================================================================================
    // IPAddrFilter.pas:315-335 IsBlockIPArea —— 用 ReverseIP 后的 DWORD 落在 [Low, High] 闭区间
    // =====================================================================================
    public bool IsBlockIPArea(int nRemoteIP)
    {
        bool Result = false;                                                          // :321
        if (g_BlockIPAreaList.Count > 0)                                              // :322
        {
            uint dwReverseIP = ReverseIP((uint)nRemoteIP);                            // :324
            for (int i = 0; i <= g_BlockIPAreaList.Count - 1; i++)                    // :325
            {
                var pIPArea = (TIPArea)g_BlockIPAreaList.GetObject(i);                // :327
                if (dwReverseIP >= pIPArea.Low && dwReverseIP <= pIPArea.High)         // :328
                {
                    Result = true;                                                    // :330
                    return Result;                                                    // :331 Exit
                }
            }
        }
        return Result;
    }

    // =====================================================================================
    // IPAddrFilter.pas:337-380 CheckNewIDOfIP
    // 4 秒窗口（:352），超阈值返回 True（:356），窗口过期则刷新 tick 并 Dec（:360-367）
    // =====================================================================================
    public bool CheckNewIDOfIP(int Addr)
    {
        bool Result = false;                                                          // :342
        if (!(Config?.m_fCheckNewIDOfIP ?? false))                                    // :343
            return Result;

        lock (_newIdLock)                                                             // :345
        {
            for (int i = 0; i <= g_NewIDOfIPList.Count - 1; i++)                      // :347
            {
                TNewIDAddr NewIDAddr = g_NewIDOfIPList[i];                            // :349
                if (NewIDAddr.IPaddr == Addr)                                         // :350
                {
                    if (TickCount() - NewIDAddr.dwIDCountTick < 4 * 1000)              // :352
                    {
                        NewIDAddr.Count++;                                            // :354 Inc(NewIDAddr.Count)
                        if (NewIDAddr.Count > (Config?.m_nCheckNewIDOfIP ?? 0))        // :355
                            Result = true;                                            // :356
                        g_NewIDOfIPList[i] = NewIDAddr;
                    }
                    else
                    {
                        NewIDAddr.dwIDCountTick = TickCount();                        // :360
                        if (NewIDAddr.Count > 0)                                      // :361
                            NewIDAddr.Count--;                                        // :362 Dec(NewIDAddr.Count)
                        if (NewIDAddr.Count <= 0)                                     // :363
                        {
                            // :365 DisPose(NewIDAddr)
                            g_NewIDOfIPList.RemoveAt(i);                              // :366
                        }
                        else
                        {
                            g_NewIDOfIPList[i] = NewIDAddr;
                        }
                    }
                    return Result;                                                    // :369 Exit
                }
            }
            var created = new TNewIDAddr();                                           // :372 New(NewIDAddr)
            created.IPaddr = Addr;                                                    // :373
            created.Count = 1;                                                        // :374
            created.dwIDCountTick = TickCount();                                       // :375
            g_NewIDOfIPList.Add(created);                                             // :376
            return Result;
        }
    }

    // =====================================================================================
    // Misc.pas:131-137 ReverseIP —— 4 字节反转（DWORD）
    // =====================================================================================
    public static uint ReverseIP(uint dwIP)
        => ((uint)(byte)(dwIP & 0xFF) << 24)          // :133 LOBYTE(LOWORD(dwIP)) shl 24
         | ((uint)(byte)((dwIP >> 8) & 0xFF) << 16)   // :134 HIBYTE(LOWORD(dwIP)) shl 16
         | ((uint)(byte)((dwIP >> 16) & 0xFF) << 8)   // :135 LOBYTE(HIWORD(dwIP)) shl 8
         | (uint)(byte)((dwIP >> 24) & 0xFF);         // :136 HIBYTE(HIWORD(dwIP))

    // =====================================================================================
    // WinSock inet_addr（ws2tcpip/ws2_32 语义）：支持 a.b.c.d / a.b.c / a.b / a
    //   · 数值段按 C 字面量解析（0x 十六进制、前导 0 八进制）
    //   · 非法输入返回 INADDR_NONE（$FFFFFFFF / -1）
    //   · 段数不足时高位补 0（"1.2.3" → 1.2.0.3；"1.2" → 1.0.0.2）
    //   · 返回值为 **网络序** 数值（"1.2.3.4" → 0x01020304）；
    //     小端机内存视图 [04 03 02 01] 即 Share.MakeIPToInt 的字节布局（Share.cs:44）
    // =====================================================================================
    public static int InetAddr(string s)
    {
        if (string.IsNullOrEmpty(s)) return INADDR_NONE;

        // 先剥掉首尾空格（原版 inet_addr 自身不剥，但调用点大多已 Trim；保留严格行为：不剥）
        string[] parts = s.Split('.');
        if (parts.Length > 4) return INADDR_NONE;

        long[] v = new long[4];
        for (int i = 0; i < parts.Length; i++)
        {
            if (!TryParseCLong(parts[i], out long parsed)) return INADDR_NONE;
            v[i] = parsed;
        }

        long addr;
        switch (parts.Length)
        {
            case 1:
                if (v[0] > 0xFFFFFFFFL) return INADDR_NONE;
                addr = v[0];
                break;
            case 2:
                if (v[0] > 0xFF || v[1] > 0xFFFFFFL) return INADDR_NONE;
                addr = (v[0] << 24) | v[1];
                break;
            case 3:
                if (v[0] > 0xFF || v[1] > 0xFF || v[2] > 0xFFFFL) return INADDR_NONE;
                addr = (v[0] << 24) | (v[1] << 16) | v[2];
                break;
            default:
                if (v[0] > 0xFF || v[1] > 0xFF || v[2] > 0xFF || v[3] > 0xFF) return INADDR_NONE;
                addr = (v[0] << 24) | (v[1] << 16) | (v[2] << 8) | v[3];
                break;
        }

        // WinSock inet_addr 返回"网络序"整数：数值 = (a<<24)|(b<<16)|(c<<8)|d，
        // 小端机上的内存字节序为 [d,c,b,a]。此处直接返回该数值（不再做字节反转）。
        return unchecked((int)addr);
    }

    /// <summary>C 字符串数值字面量："10"(十进制) / "0x1f"(十六进制) / "017"(八进制)。</summary>
    private static bool TryParseCLong(string s, out long value)
    {
        value = 0;
        if (string.IsNullOrEmpty(s)) return false;
        string t = s.Trim();
        if (t.Length == 0) return false;

        int radix = 10;
        if (t.Length > 2 && (t.StartsWith("0x") || t.StartsWith("0X")))
        {
            radix = 16;
            t = t.Substring(2);
        }
        else if (t.Length > 1 && t[0] == '0')
        {
            radix = 8;
            t = t.Substring(1);
        }
        if (t.Length == 0) { value = 0; return true; }

        long acc = 0;
        foreach (char c in t)
        {
            int d;
            if (c >= '0' && c <= '9') d = c - '0';
            else if (c >= 'a' && c <= 'f') d = c - 'a' + 10;
            else if (c >= 'A' && c <= 'F') d = c - 'A' + 10;
            else return false;
            if (d >= radix) return false;
            acc = acc * radix + d;
            if (acc > 0xFFFFFFFFL) return false;
        }
        value = acc;
        return true;
    }

    /// <summary>
    /// WinSock inet_ntoa（网络序整数 → 点分字符串）：数值高字节是 a，低字节是 d。
    /// 与 Share.MakeIntToIP（Share.cs:21-29，取低字节为 A）字节序一致。
    /// 永不返回 null（原版仅在地址非法时返回 nil，而 inet_ntoa 对任意 DWORD 都能格式化）。
    /// </summary>
    public static string? InetNtoa(int nIP)
    {
        uint v = (uint)nIP;
        return $"{(byte)((v >> 24) & 0xFF)}.{(byte)((v >> 16) & 0xFF)}.{(byte)((v >> 8) & 0xFF)}.{(byte)(v & 0xFF)}";
    }

    /// <summary>TStringList.Objects[i] 的 Integer(TObject) 还原。</summary>
    private static int ToInt(object o)
    {
        if (o is int i) return i;
        if (o is uint u) return unchecked((int)u);
        return Convert.ToInt32(o, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Misc.pas:41-66 CloseIPConnect 的**过滤部分**（关闭动作经回调注入）。
    /// 说明：Delphi 版遍历全局 g_UserList（ClientSession.pas:44 的 USER_ARRAY_COUNT 定长数组），
    /// 命中条件 = m_tLastGameSvr&lt;&gt;nil 且 Active 且 m_pUserOBJ.nIPAddr = nRemoteIP；
    /// 命中后按 m_fHandleLogin&gt;=2 分派：发 SM_OUTOFCONNECTION + 置 KickFlag，否则直接 FreeSocket。
    /// </summary>
    public static void CloseIPConnect(int nRemoteIP, IReadOnlyList<ISelSessionHost?> g_UserList,
                                      Action<ISelSessionHost, int> sendOutOfConnection,
                                      Action<ISelSessionHost> freeSocket)
    {
        if (!SelGateGlobals.g_fServiceStarted)                                        // Misc.pas:46
            return;
        for (int n = 0; n <= g_UserList.Count - 1; n++)                               // Misc.pas:48
        {
            ISelSessionHost? UserObj = g_UserList[n];                                 // Misc.pas:50
            if (UserObj != null && UserObj.Active && UserObj.IPAddr == nRemoteIP)     // Misc.pas:51-54
            {
                if (UserObj.HandleLogin >= 2)                                         // Misc.pas:56
                {
                    sendOutOfConnection(UserObj, UserObj.SvrObject);                  // Misc.pas:58-59
                    UserObj.KickFlag = true;                                          // Misc.pas:60
                }
                else
                {
                    freeSocket(UserObj);                                              // Misc.pas:63
                }
            }
        }
    }
}

/// <summary>
/// ClientSession.pas TSessionObj 中参与 IPAddrFilter/Misc 判定所需的只读接缝。
/// 接缝：待 ClientSession.pas 其余部分（IOCP 发送队列/会话生命周期）移植后接入。
/// </summary>
public interface ISelSessionHost
{
    int IPAddr { get; }               // m_pUserOBJ.nIPAddr
    string IPText { get; }            // m_pUserOBJ.pszIPAddr（PacketRuleConfig.pas:201 Trim(pszIPAddr)）
    bool Active { get; }              // m_tLastGameSvr.Active
    int HandleLogin { get; }          // m_fHandleLogin
    int SvrObject { get; }            // m_nSvrObject
    bool KickFlag { get; set; }       // m_fKickFlag
}

/// <summary>
/// IPAddrFilter.pas:382-389 全局单例（对应 unit 级 var 的初始化时机）。
/// 测试可替换 <see cref="Instance"/>，生产由 AppMain 装配。
/// </summary>
public static class SelGateIPFilterGlobal
{
    public static CSelGateIPFilter Instance { get; set; } = new CSelGateIPFilter();
}
