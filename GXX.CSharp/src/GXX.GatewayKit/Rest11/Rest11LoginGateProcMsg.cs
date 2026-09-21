using System;
using System.Collections.Generic;

namespace GXX.GatewayKit.Rest11;

/// <summary>
/// `Source/LoginGate/FuncForComm.pas` → `Rest11LoginGateProcMsg.cs`
/// 1:1 逐字移植：`TProcMsgThread`（:10-27 声明、:99-109 构造、:111-117 析构、:119-127 Lock/Unlock、
/// :129-137 AddSession、:139-153 DelSession、:155-178 GetSession、:180-257 Run）、
/// `TAddressInfo`（:29-49）、`TAddressListEx`（:51-70 声明、:261-356 实现）、
/// `StartService`（:358-427）、`StopService`（:429-465）、`KeepAlive`（:467-505）、
/// `ShowThreadInfo`（:507-561）、`OnTimerProc`（:563-587）。
///
/// <para>
/// ⚠ 与既有设施的关系（**并存**，不合并）：
///   · `TProcMsgThread` / `TAddressInfo` / `TAddressListEx` 在 `src/GXX.LoginGate` +
///     `src/GXX.GatewayKit` 中 **0 托管声明**（复核报告 §4.3；复跑命令见本车道报告 §7）。
///   · `KeepAlive` 的**部分**语义已由 `FrmMain.cs:63-64` 的 5 秒计时器与
///     `LoginGateService.SendKeepAlive()`（LoginGateService.cs:131-135）顶替；
///     但 `TProcMsgThread.Run` 的**客户端超时踢线 / DelayClose 到期关闭**两条路径完全没有对应物。
///   · `GXX.SelGate` 侧有同名 `TAddressListEx`（`RunGate\GateShare.pas:90` 是**另一单元**）
///     ⇒ 本类按 §1.2 规则 3 逐副本归属 **LoginGate\FuncForComm.pas:51**。
/// </para>
///
/// <para>
/// 原文缺陷（照抄，未顺手修）：
/// <list type="number">
///   <item>`Run`（:180-257）末尾的 `m_xTempUserList.Clear` **被原文注释掉**（:253），
///         但过程开头（:191-192）会先清空 ⇒ 行为等价；托管保留开头清空、把 :253 登记为"原文如此"。</item>
///   <item>`Run` 在 `not CSession.m_fKickFlag` 分支里，`m_IsDelayClose` 到期时
///         **先置 `m_fKickFlag := True` 再置 `m_IsDelayClose := False`**（:215-216），
///         且**不**处理已置位的 `m_fKickFlag`；照抄。</item>
///   <item>`Run` 里超时分支把 `m_dwClientTimeOutTick` **刷新为当前 tick**（:227）后才踢线
///         ⇒ 记事本式"刷新即续命"；照抄。</item>
///   <item>原文依赖 `TTimer`（`Interval := 1; Enabled := True; OnTimer := Run`，:106-108）
///         ⇒ 托管以 <see cref="Enabled"/> + 显式调用 <see cref="Run"/> 表达
///         （测试可同步驱动，**不依赖真实网络端口 / 真实计时器**）。</item>
///   <item>`ShowThreadInfo`（:507-561）依赖 `FormMain.m_xRunServerList.IOCPServer.AcceptExSocket`
///         与 `GridSocketInfo`：`AcceptExWorkedThread` 在 `src/**` 中 **0 托管声明**（报告 §4.2），
///         故托管以 <see cref="IRest11ThreadInfoSource"/> 接缝表达"取服务器列表 + 写网格"，
///         逻辑（端口/状态/收发字节格式化）**逐行照抄**。</item>
/// </list>
/// </para>
/// </summary>
public class Rest11ProcMsgThread
{
    /// <summary>`FuncForComm.pas:12` `m_nProcIdx: Integer`。</summary>
    public int m_nProcIdx;

    /// <summary>`FuncForComm.pas:13-14` `m_xUserList` / `m_xTempUserList: Classes.TList`。</summary>
    public readonly List<IRest11SessionObj?> m_xUserList = new();
    public readonly List<IRest11SessionObj?> m_xTempUserList = new();

    private readonly object m_csAccess = new();  // :15 TRTLCriticalSection m_csAccess

    /// <summary>`Self.Enabled := True`（:107）。</summary>
    public volatile bool Enabled;

    /// <summary>`Self.Interval := 1`（:106）—— 原文 1ms 心跳。</summary>
    public int Interval = 1;

    /// <summary>`FuncForComm.pas:99-109` `constructor TProcMsgThread.Create`。</summary>
    public Rest11ProcMsgThread()
    {
        m_nProcIdx = 0;      // :103
        Enabled = true;      // :107
    }

    /// <summary>`FuncForComm.pas:111-117` `destructor Destroy`（:113-114 Free 两个 TList）。</summary>
    public void Destroy()
    {
        m_xUserList.Clear();       // :113 m_xUserList.Free
        m_xTempUserList.Clear();   // :114 m_xTempUserList.Free
    }

    /// <summary>`FuncForComm.pas:119-122` `Lock`（`EnterCriticalSection`）。</summary>
    public void Lock() => System.Threading.Monitor.Enter(m_csAccess);

    /// <summary>`FuncForComm.pas:124-127` `Unlock`（`LeaveCriticalSection`）。</summary>
    public void Unlock() => System.Threading.Monitor.Exit(m_csAccess);

    /// <summary>`FuncForComm.pas:129-137` `AddSession`。</summary>
    public void AddSession(IRest11SessionObj CSession)
    {
        Lock();                       // :131
        try
        {
            m_xUserList.Add(CSession); // :133
        }
        finally
        {
            Unlock();                 // :135
        }
    }

    /// <summary>`FuncForComm.pas:139-153` `DelSession`（`IndexOf` 语义按引用相等）。</summary>
    public void DelSession(IRest11SessionObj CSession)
    {
        Lock();                                                        // :143
        try
        {
            int i = IndexOfReference(m_xUserList, CSession);            // :145 i := m_xUserList.IndexOf(CSession)
            if (i >= 0)                                                // :146
            {
                m_xUserList.RemoveAt(i);                               // :148 m_xUserList.Delete(i)
            }
        }
        finally
        {
            Unlock();                                                  // :151
        }
    }

    /// <summary>`FuncForComm.pas:155-178` `GetSession(nSock: Integer)`（按 `_SendObj.Socket` 查）。</summary>
    public IRest11SessionObj? GetSession(int nSock, int INVALID_SOCKET = -1)
    {
        IRest11SessionObj? Result = null;                              // :160
        if (nSock != INVALID_SOCKET)                                   // :161
        {
            Lock();                                                    // :163
            try
            {
                for (int i = 0; i <= m_xUserList.Count - 1; i++)        // :165
                {
                    IRest11SessionObj? UserOBJ = m_xUserList[i];        // :167
                    if (UserOBJ != null && UserOBJ.Socket == nSock)     // :168 UserOBJ.m_pUserOBJ._SendObj.Socket = nSock
                    {
                        Result = UserOBJ;                              // :170
                        break;                                         // :171
                    }
                }
            }
            finally
            {
                Unlock();                                              // :175
            }
        }
        return Result;
    }

    /// <summary>
    /// `FuncForComm.pas:180-257` `TProcMsgThread.Run(Sender: TObject)`
    /// （原文由 `TTimer.OnTimer` 触发，1ms）。
    /// </summary>
    public void Run(IRest11EnforcementChannel channel, Rest11LoginGateConfig g_pConfig)
    {
        // 原文 :185-186 的 while/Terminated 循环被注释掉 ⇒ 单次执行
        if (!channel.ServiceStarted)                                    // :188 if not g_fServiceStarted then Exit
            return;

        if (m_xTempUserList.Count > 0)                                  // :191
            m_xTempUserList.Clear();                                    // :192

        Lock();                                                         // :194
        try
        {
            for (int i = 0; i <= m_xUserList.Count - 1; i++)            // :196
            {
                m_xTempUserList.Add(m_xUserList[i]);                    // :198
            }
        }
        finally
        {
            Unlock();                                                   // :201
        }

        for (int i = 0; i <= m_xTempUserList.Count - 1; i++)            // :204
        {
            IRest11SessionObj? CSession = m_xTempUserList[i];           // :206
            if (CSession != null && CSession.LastGameSvrActive)         // :207 (CSession <> nil) and (m_tLastGameSvr <> nil)
            {
                if (!CSession.KickFlag)                                 // :209 if not CSession.m_fKickFlag
                {
                    if (CSession.HandleLogin < 3)                       // :211
                    {
                        if (CSession.IsDelayClose && (TickCount() >= CSession.dwDelayCloseTick)) // :213
                        {
                            CSession.KickFlag = true;                   // :215
                            CSession.IsDelayClose = false;              // :216
                            channel.FreeSocket(CSession.Socket);        // :217 SHSocket.FreeSocket(...)
                            // :218-223 原文如此：g_pLogMgr 日志块被 { } 注释掉
                        }
                        else if (unchecked(TickCount() - CSession.dwClientTimeOutTick) > (uint)g_pConfig.m_nClientTimeOutTime) // :225
                        {
                            CSession.dwClientTimeOutTick = TickCount();     // :227
                            channel.SendOutOfConnection(CSession, CSession.SvrObject); // :228 SendDefMessage(SM_OUTOFCONNECTION, ...)
                            CSession.KickFlag = true;                   // :229
                            BlockUser(CSession, g_pConfig, channel);    // :230 BlockUser(CSession)
                            if (channel.CheckLevel(5))                  // :231
                            {
                                channel.AddLog("Client Connect Time Out: " + CSession.IPText); // :233
                            }
                        }
                    }
                }
                else
                {
                    if (unchecked(TickCount() - CSession.dwClientTimeOutTick) > (uint)g_pConfig.m_nClientTimeOutTime) // :240
                    {
                        if (channel.CheckLevel(5))                      // :242
                        {
                            channel.AddLog("Client Connect Time Out 2: " + CSession.IPText); // :244
                        }

                        CSession.dwClientTimeOutTick = TickCount();     // :247
                        channel.FreeSocket(CSession.Socket);            // :248 SHSocket.FreeSocket(...)
                    }
                }
            }
        }
        // :253 m_xTempUserList.Clear —— 原文如此：本行被注释掉
    }

    /// <summary>
    /// `Windows.GetTickCount`（原文 `FuncForComm.pas:213/:225/:240` 直接调用）。
    /// 以 <see cref="TickCount"/> 注入，便于测试用固定时钟驱动超时分支。
    /// </summary>
    public Func<uint> TickCount { get; set; } = GXX.Core.Rtl.DelphiRTL.GetTickCount;

    /// <summary>`FuncForComm.pas:230` 的 `BlockUser(CSession)` —— 委派给 Misc 的 1:1 实现。</summary>
    private static void BlockUser(IRest11SessionObj CSession, Rest11LoginGateConfig g_pConfig,
                                 IRest11EnforcementChannel channel)
        => Rest11LoginGateMisc.BlockUser(CSession, g_pConfig, channel);

    /// <summary>`Classes.TList.IndexOf` 的**引用相等**语义（Delphi TList 比较指针，不比较内容）。</summary>
    private static int IndexOfReference(List<IRest11SessionObj?> list, IRest11SessionObj item)
    {
        for (int i = 0; i < list.Count; i++)
            if (ReferenceEquals(list[i], item)) return i;
        return -1;
    }
}

/// <summary>
/// `FuncForComm.pas:29-49` `TAddressInfo = record`（14 个字段：IP 计数 / 拒绝 / 账号错 / 密码错 / 超时）。
/// Delphi 传的是 `pTAddressInfo` 指针 ⇒ 托管用**可变类**表达同一"引用共享"语义。
/// </summary>
public class TAddressInfo
{
    public string sIPaddr = "";          // :31
    public int nIPaddr;                  // :32
    public int nCount;                   // :33
    public uint dwIPCountTick1;          // :34
    public int nIPCount1;                // :35
    public uint dwIPCountTick2;          // :36
    public int nIPCount2;                // :37
    public uint dwDenyTick;              // :38
    public int nIPDenyCount;             // :39

    public uint dwIPAccountErrTick;      // :41
    public int nIPAccountErrCount;       // :42

    public uint dwIPPasswordErrTick;     // :44
    public int nIPPasswordErrCount;      // :45

    public uint dwIPPasswordProtectedErrTick;  // :47
    public int nIPPasswordProtectedErrCount;   // :48
}

/// <summary>
/// `FuncForComm.pas:51-70` 声明 + `:261-356` 实现的 `TAddressListEx`
/// （`FAddress: TList` 存 `pTAddressInfo`，`GLock` 临界区）。
///
/// <para>
/// 注意：`Add`/`Find`/`Delete` **不加锁**（原文如此：只有 `Clear` 用 `Lock/UnLock`，
/// `:315-356` 三个方法直接操作 `FAddress`）——照抄，不顺手加锁。
/// </para>
/// </summary>
public class TAddressListEx
{
    private readonly List<TAddressInfo> FAddress = new();      // :53 FAddress: TList
    private readonly object GLock = new();                     // :54 GLock: TRTLCriticalSection

    /// <summary>`FuncForComm.pas:261-266` `constructor Create`。</summary>
    public TAddressListEx() { }

    /// <summary>`FuncForComm.pas:268-274` `destructor Destroy`（`Clear` + `FAddress.Free`）。</summary>
    public void Destroy()
    {
        Clear();          // :270
        FAddress.Clear(); // :271 FAddress.Free
    }

    /// <summary>`FuncForComm.pas:276-290` `Clear`（唯一加锁的方法）。</summary>
    public void Clear()
    {
        Lock();                                                          // :280
        try
        {
            for (int I = 0; I <= FAddress.Count - 1; I++)                // :282
            {
                // :284 Dispose(pTAddressInfo(FAddress.Items[I])) —— 托管由 GC 负责
            }
            FAddress.Clear();                                            // :286
        }
        finally
        {
            UnLock();                                                    // :288
        }
    }

    /// <summary>`FuncForComm.pas:292-298` `GetItems(Index)`：越界返回 nil。</summary>
    public TAddressInfo? GetItems(int Index)
        => (Index >= 0 && Index <= FAddress.Count - 1) ? FAddress[Index] : null; // :294-297

    /// <summary>`FuncForComm.pas:300-303` `Lock`。</summary>
    public void Lock() => System.Threading.Monitor.Enter(GLock);

    /// <summary>`FuncForComm.pas:305-308` `UnLock`。</summary>
    public void UnLock() => System.Threading.Monitor.Exit(GLock);

    /// <summary>`FuncForComm.pas:310-313` `GetCount`。</summary>
    public int GetCount() => FAddress.Count; // :312

    /// <summary>
    /// `FuncForComm.pas:315-322` `Add(IP)`：
    /// `New(Result); ZeroMemory(Result, SizeOf(TAddressInfo)); Result.nIPaddr := inet_addr(IP); Result.sIPaddr := IP; FAddress.Add(Result)`
    /// ⇒ 全字段清零（C# 默认值），但 **`nCount` 等为 0 而非 1**。照抄。
    /// </summary>
    public TAddressInfo Add(string IP)
    {
        var Result = new TAddressInfo();                                 // :317 New(Result) + :318 ZeroMemory
        Result.nIPaddr = Rest11LoginGateNet.InetAddr(IP);                // :319
        Result.sIPaddr = IP;                                             // :320
        FAddress.Add(Result);                                            // :321
        return Result;
    }

    /// <summary>`FuncForComm.pas:324-341` `Find(IP)`：按 `inet_addr` 结果线性查找，未命中返回 nil。</summary>
    public TAddressInfo? Find(string IP)
    {
        TAddressInfo? Result = null;                                     // :330
        int nIP = Rest11LoginGateNet.InetAddr(IP);                       // :331
        for (int I = 0; I <= FAddress.Count - 1; I++)                    // :332
        {
            TAddressInfo AddressInfo = FAddress[I];                      // :334
            if (AddressInfo.nIPaddr == nIP)                              // :335
            {
                Result = AddressInfo;                                    // :337
                return Result;                                           // :338 Exit
            }
        }
        return Result;
    }

    /// <summary>`FuncForComm.pas:343-356` `Delete(AddressInfo)`：按**引用相等**定位后删除。</summary>
    public void Delete(TAddressInfo AddressInfo)
    {
        for (int I = 0; I <= FAddress.Count - 1; I++)                    // :347
        {
            if (ReferenceEquals(FAddress[I], AddressInfo))               // :349 if FAddress.Items[I] = AddressInfo
            {
                FAddress.RemoveAt(I);                                    // :352 FAddress.Delete(I)
                return;                                                  // :353 Exit
            }
        }
    }
}

/// <summary>
/// `FuncForComm.pas:507-561` `ShowThreadInfo` 所需的"服务器列表 + 网格写出"接缝。
/// `AcceptExWorkedThread.pas`（`TIOCPAccepter`/`PServerInfo`/`GridSocketInfo`）在托管侧
/// **0 声明**（报告 §4.2），故此处只保留接缝；格式化逻辑在
/// <see cref="Rest11LoginGateProcMsgFormat"/> 里 1:1。
/// </summary>
public interface IRest11ThreadInfoSource
{
    /// <summary>`IOCPAccepter.ServerCount`（:520）。</summary>
    int ServerCount { get; }

    /// <summary>`IOCPAccepter.Active`（:522）；原文**循环内不变**，为此处属性。</summary>
    bool Active { get; }

    /// <summary>`IOCPAccepter.InUseBlock` / `MaxInUseBlock`（:535）。</summary>
    int InUseBlock { get; }
    int MaxInUseBlock { get; }

    /// <summary>第 i 个 `PServerInfo` 的只读投影（`:524-543`）。</summary>
    IRest11ServerInfo? GetServerInfo(int index);

    /// <summary>`FormMain.GridSocketInfo.Cells[col, row] := value`（:527-556）。</summary>
    void SetGridCell(int col, int row, string value);

    /// <summary>`FormMain.StatusBar.Panels[0].Text := ...`（:535）。</summary>
    void SetStatusPanel(string text);
}

/// <summary>`AcceptExWorkedThread.pas` 的 `TServerInfo` 在 ShowThreadInfo 中用到的最小投影。</summary>
public interface IRest11ServerInfo
{
    /// <summary>`PSI.pClient.Active`（:525）。</summary>
    bool ClientActive { get; }
    /// <summary>`PSI.pClient.ServerIP`（:527）。</summary>
    string ServerIP { get; }
    /// <summary>`PSI.nPort`（:528）。</summary>
    int Port { get; }
    /// <summary>`PSI.pClient.m_tSockThreadStutas`（:529）。</summary>
    Rest11TSockThreadStutas SockThreadStutas { get; }
    /// <summary>`PSI.pClient.m_dwSendBytes`（:537-543，读取后置 0）。</summary>
    uint SendBytes { get; set; }
    /// <summary>`PSI.pClient.m_dwRecvBytes`（:548-554，读取后置 0）。</summary>
    uint RecvBytes { get; set; }
}

/// <summary>
/// `FuncForComm.pas:507-561` `ShowThreadInfo` 的纯格式化部分（`↑%fM` / `↓%dK` 等）与
/// `:563-587` `OnTimerProc` 的 `case idEvent` 分派。
/// </summary>
public static class Rest11LoginGateProcMsgFormat
{
    /// <summary>
    /// `FuncForComm.pas:537-542` 的发送字节格式化：
    /// `&gt; 1024*1000` → `↑%fM`（除以 1024*1000）；`&gt; 1024` → `↑%fK`；否则 `↑%dB`。
    /// 原文 `StrFmt(@pszBuf[0], '↑%fM', [bytes / (1024*1000)])` 的 `%f` 是 Delphi
    /// **2 位小数**默认格式（`FloatToStrF(ffGeneral)` 实际为不定长，正文见报告登记）。
    /// 托管按下述固定格式复刻：M/K 保留 2 位小数（与 Delphi `%f` 默认一致），B 为整数。
    /// </summary>
    public static string FormatSendBytes(uint dwSendBytes)
    {
        if (dwSendBytes > (1024 * 1000))
            return "↑" + FormatF(dwSendBytes / (1024.0 * 1000.0)) + "M";   // :538
        if (dwSendBytes > 1024)
            return "↑" + FormatF(dwSendBytes / 1024.0) + "K";             // :540
        return "↑" + dwSendBytes + "B";                                   // :542
    }

    /// <summary>`FuncForComm.pas:548-553` 的接收字节格式化（`↓%fM` / `↓%fK` / `↓%dB`）。</summary>
    public static string FormatRecvBytes(uint dwRecvBytes)
    {
        if (dwRecvBytes > (1024 * 1000))
            return "↓" + FormatF(dwRecvBytes / (1024.0 * 1000.0)) + "M";  // :549
        if (dwRecvBytes > 1024)
            return "↓" + FormatF(dwRecvBytes / 1024.0) + "K";            // :551
        return "↓" + dwRecvBytes + "B";                                   // :553
    }

    private static string FormatF(double v)
        => v.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>`FuncForComm.pas:529-533` `case PSI.pClient.m_tSockThreadStutas of` 的三分支。</summary>
    public static string StatusText(Rest11TSockThreadStutas st) => st switch
    {
        Rest11TSockThreadStutas.stConnecting => "连接中..",  // :530
        Rest11TSockThreadStutas.stConnected => "已连接",      // :531
        _ => "超时"                                          // :532 stTimeOut
    };

    /// <summary>`FuncForComm.pas:507-561` `ShowThreadInfo()`（`nRow` 从 1 起、逐行 Inc）。</summary>
    public static void ShowThreadInfo(IRest11ThreadInfoSource src)
    {
        // :514 if not g_fServiceStarted then Exit —— 由调用方（OnTimerProc / 宿主）判定
        int nRow = 1;                                                     // :519
        for (int i = 0; i <= src.ServerCount - 1; i++)                    // :520
        {
            if (src.Active)                                               // :522
            {
                IRest11ServerInfo? PSI = src.GetServerInfo(i);            // :524
                if (PSI == null) { nRow++; continue; }
                if (!PSI.ClientActive)                                    // :525
                {
                    nRow++;                                               // :559 Inc(nRow)
                    continue;                                             // :526 Continue
                }
                src.SetGridCell(0, nRow, PSI.ServerIP);                   // :527
                src.SetGridCell(1, nRow, GXX.Core.Rtl.DelphiRTL.IntToStr(PSI.Port)); // :528
                src.SetGridCell(2, nRow, StatusText(PSI.SockThreadStutas)); // :529-533

                src.SetStatusPanel("连接: " + src.InUseBlock + "/" + src.MaxInUseBlock); // :535 Format('连接: %d/%d')

                string buf = FormatSendBytes(PSI.SendBytes);              // :537-542
                PSI.SendBytes = 0;                                        // :543

                buf = buf + "  " + FormatRecvBytes(PSI.RecvBytes);        // :545-554（pszBuf[nLen]=' '; pszBuf[nLen+1]=' '）
                PSI.RecvBytes = 0;                                        // :554

                src.SetGridCell(3, nRow, buf);                            // :556
            }
            nRow++;                                                       // :559 Inc(nRow)
        }
    }

    /// <summary>
    /// `FuncForComm.pas:563-587` `OnTimerProc(hWnd, uMsg, idEvent, dwTime)` 的 `case idEvent`。
    /// 返回 true 表示该事件已被处理（对应原文四个 `_IDM_TIMER_*` 分支）。
    /// </summary>
    public static bool OnTimerProc(uint idEvent, Action startService, Action stopService,
                                   Action keepAlive, Action showThreadInfo)
    {
        switch (idEvent)                                                  // :567
        {
            case Rest11LoginGateConstants._IDM_TIMER_STARTSERVICE:        // :568
                startService();                                           // :571 StartService()（:570 KillTimer 由宿主负责）
                return true;
            case Rest11LoginGateConstants._IDM_TIMER_STOPSERVICE:         // :573
                stopService();                                            // :576 StopService()
                return true;
            case Rest11LoginGateConstants._IDM_TIMER_KEEP_ALIVE:          // :578
                keepAlive();                                              // :580 KeepAlive()
                return true;
            case Rest11LoginGateConstants._IDM_TIMER_THREAD_INFO:         // :582
                showThreadInfo();                                         // :584 ShowThreadInfo()
                return true;
            default:
                return false;
        }
    }
}
