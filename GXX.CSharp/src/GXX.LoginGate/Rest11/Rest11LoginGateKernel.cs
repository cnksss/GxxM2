using System;
using System.Collections.Generic;
using GXX.Core.Rtl;
using GXX.GatewayKit;
using GXX.GatewayKit.Rest11;

namespace GXX.LoginGate.Rest11;

/// <summary>
/// 车道 p11-logingate-filter 的 **LoginGate 侧接线**（`SOURCE/LoginGate` 残部落到 LoginGate 进程侧）。
///
/// <para>
/// 本文件**只新建**，不改 `src/GXX.LoginGate/**` 任何既有文件；
/// 与之配套的通用实现位于 `src/GXX.GatewayKit/Rest11/**`（命名空间 `GXX.GatewayKit.Rest11`），
/// 单测位于 `tests/GXX.GatewayKit.Tests/Rest11*.cs`。
/// </para>
///
/// <para>
/// ⚠ **opt-in 且默认不生效**：<see cref="Rest11LoginGateKernel.Enabled"/> 默认 false；
/// 宿主（`LoginGateService`）只有显式构造本 kernel 并调用
/// <see cref="Rest11LoginGateKernel.OnClientAccepted"/> / <see cref="Rest11LoginGateKernel.OnTick"/>
/// / <see cref="Rest11LoginGateKernel.OnClientClosed"/> 才会启用本设施。
/// 既有 `GateService.CheckIP` / `GateService._perIP` 语义**一字不改**。
/// </para>
///
/// <para>
/// SelGate 侧三处头注（`SelGateIPAddrFilter.cs:17-25`、`SelGateMisc.cs:11-16`、`SelGateSession.cs:17-24`）
/// 写明"与 GatewayKit 的差异必须保留"⇒ 本文件**不引用** `GXX.SelGate` 的任何类型，
/// `GXX.SelGate` 也**不引用**本文件。
/// </para>
/// </summary>
public sealed class Rest11LoginGateKernel : IDisposable
{
    /// <summary>`ConfigManager.pas:52 g_pConfig: TConfigMgr`。</summary>
    public Rest11LoginGateConfig g_pConfig { get; }

    /// <summary>`IPAddrFilter.pas:8-15` 的全部全局量（独立实例，不与 SelGate 共享）。</summary>
    public Rest11LoginGateIpFilter IPAddrFilter { get; }

    /// <summary>`FuncForComm.pas:77 g_ProcMsgThread: TProcMsgThread`。</summary>
    public Rest11ProcMsgThread g_ProcMsgThread { get; }

    /// <summary>`ClientSession.pas:47 g_UserList: array[0..USER_ARRAY_COUNT-1] of TSessionObj`。</summary>
    public List<IRest11SessionObj?> g_UserList { get; }

    /// <summary>`FuncForComm.pas:78 g_CurrIPaddrList: TAddressListEx`。</summary>
    public TAddressListEx g_CurrIPaddrList { get; }

    /// <summary>执法副作用通道（`SHSocket.FreeSocket` / `SendDefMessage` / 黑名单 / 日志）。</summary>
    public IRest11EnforcementChannel Channel { get; }

    /// <summary>`g_fServiceStarted`（Protocol.pas:109）。</summary>
    public volatile bool g_fServiceStarted;

    /// <summary>是否启用本设施（默认 false ⇒ 既有行为不变）。</summary>
    public bool Enabled { get; set; }

    /// <summary>`Misc.pas:141` `SendGameCenterMsg` 的通道（可选，未提供时调用是空操作）。</summary>
    public IRest11GameCenterChannel? GameCenterChannel { get; set; }

    public Rest11LoginGateKernel(Rest11LoginGateConfig config, IRest11EnforcementChannel channel,
                                 int userArrayCount = 1000 + 48)
    {
        g_pConfig = config;
        Channel = channel;
        IPAddrFilter = new Rest11LoginGateIpFilter { Config = config };
        g_ProcMsgThread = new Rest11ProcMsgThread();
        g_UserList = new List<IRest11SessionObj?>(userArrayCount);
        for (int i = 0; i < userArrayCount; i++) g_UserList.Add(null); // 原文 FillUserList 填同一占位对象；此处留 null 槽
        g_CurrIPaddrList = new TAddressListEx();
    }

    // =====================================================================================
    // FuncForComm.pas:358-427 StartService —— 只移植**与 LoginGate 残部相关**的部分：
    //   g_fServiceStarted := True / g_ProcMsgThread.Enabled := True / g_pConfig.LoadConfig /
    //   IPAddrFilter.ClearConnectOfIP / SetTimer(_IDM_TIMER_THREAD_INFO, 1000)
    // UI（FormMain/Caption/菜单/MENU_VIEW_HELP_ABOUTClick）与 VER_TYPE=1 授权块**不在本车道**。
    // =====================================================================================
    public void StartService(bool loadConfig = true)
    {
        g_fServiceStarted = true;                        // :366
        g_ProcMsgThread.Enabled = true;                  // :368
        if (loadConfig)
            g_pConfig.LoadConfig();                      // :373
        IPAddrFilter.ClearConnectOfIP();                 // :375
        // :404 SetTimer(g_hMainWnd, _IDM_TIMER_THREAD_INFO, 1000, Pointer(@OnTimerProc))
        //      ⇒ 宿主每秒调用一次 Rest11LoginGateKernel.OnTimer(_IDM_TIMER_THREAD_INFO)
        TimerIntervalMs[Rest11LoginGateConstants._IDM_TIMER_THREAD_INFO] = 1000;
    }

    /// <summary>`FuncForComm.pas:429-465 StopService` 的残部（清连接表 + 存黑名单 + 停线程）。</summary>
    public void StopService()
    {
        g_fServiceStarted = false;                       // :433
        IPAddrFilter.ClearConnectOfIP();                 // :457
        IPAddrFilter.SaveBlockIPList();                  // :459 SaveBlockIPList()
        IPAddrFilter.SaveBlockIPAreaList();              // :460 SaveBlockIPAreaList()
        g_ProcMsgThread.Enabled = false;                 // :462
    }

    /// <summary>原文 `SetTimer(g_hMainWnd, id, ms, ...)` 的登记表（供宿主决定调度周期）。</summary>
    public readonly Dictionary<int, int> TimerIntervalMs = new();

    // =====================================================================================
    // 会话进出（对应 ClientSession.UserEnter/UserLeave 的残部）
    // =====================================================================================

    /// <summary>
    /// 对应 `AppMain.pas:580` 附近 `g_UserList[n] := UserOBJ` 的登记（原文定长数组 0..1047，
    /// 这里按 `SocketId % Count` 落槽以免无限增长；**这是托管侧的容量语义偏离**，
    /// 见报告 §6 D-P11-01）。
    /// </summary>
    public void OnClientAccepted(IRest11SessionObj session)
    {
        if (!Enabled || !g_fServiceStarted) return;
        int slot = session.Socket % g_UserList.Count;
        if (slot < 0) slot += g_UserList.Count;
        g_UserList[slot] = session;
        // 车道 p14-logingate-wire 修正：`g_CurrIPaddrList` 原属 **AppMain.pas:760-980 的
        // 服务器消息记账块**（`g_CurrIPaddrList.Lock/Find/Add`，用于账号错/密码错/验证攻击计数），
        // 而那一整块至今**未移植**。p11 曾在此处 `Add(session.IPText)`，属**伪造的调用点**
        // （原文该表不由"客户端接入"填充），本车道予以删除以免制造不存在的语义。
        // `g_CurrIPaddrList` 字段本身保留（原文 `FuncForComm.pas:78` 的全局量），
        // 待 AppMain 记账块移植后由它使用。
        g_ProcMsgThread.AddSession(session);                        // ClientSession.pas:713
    }

    /// <summary>对应 `ClientSession.pas:752-753` `DeleteConnectOfIP(Self.m_pUserOBJ.nIPAddr)`。</summary>
    public void OnClientClosed(IRest11SessionObj session)
    {
        if (!Enabled) return;
        IPAddrFilter.DeleteConnectOfIP(session.IPAddr);             // :753
        g_ProcMsgThread.DelSession(session);                        // :778
        for (int i = 0; i < g_UserList.Count; i++)
            if (ReferenceEquals(g_UserList[i], session)) g_UserList[i] = null;
    }

    // =====================================================================================
    // FuncForComm.pas:563-587 OnTimerProc 的托管入口（宿主计时器驱动）
    // =====================================================================================

    /// <summary>对应 `SetTimer(g_hMainWnd, _IDM_TIMER_KEEP_ALIVE, ...)` 每次到期的 `KeepAlive()`。</summary>
    public void OnKeepAliveTimer() => g_ProcMsgThread.Run(Channel, g_pConfig);   // :580

    /// <summary>
    /// 对应 `_IDM_TIMER_THREAD_INFO`（1000ms）到期的 `ShowThreadInfo()`（:584）。
    /// 传入 <paramref name="src"/> 为 null 时等价于 `g_fServiceStarted = False` 的早退（原文 :514-515）。
    /// </summary>
    public void OnThreadInfoTimer(IRest11ThreadInfoSource? src)
    {
        if (!g_fServiceStarted) return;                             // :514
        if (src == null) return;
        Rest11LoginGateProcMsgFormat.ShowThreadInfo(src);            // :517-560
    }

    // =====================================================================================
    // Misc.pas 的 8 个执法例程在 LoginGate kernel 上的显式出口（对应原文全局函数）
    // =====================================================================================

    /// <summary>`Misc.pas:155-179 CloseIPConnect(nRemoteIP)`。</summary>
    public void CloseIPConnect(int nRemoteIP)
        => Rest11LoginGateMisc.CloseIPConnect(nRemoteIP, g_UserList, Channel);

    /// <summary>`Misc.pas:181-205 KickUser(nRemoteIP): Boolean`。</summary>
    public bool KickUser(int nRemoteIP)
        => Rest11LoginGateMisc.KickUser(nRemoteIP, g_pConfig, g_UserList, Channel);

    /// <summary>`Misc.pas:207-224 KickUser(const UserObj)`。</summary>
    public void KickUser(IRest11SessionObj UserObj)
        => Rest11LoginGateMisc.KickUser(UserObj, g_pConfig, Channel);

    /// <summary>`Misc.pas:226-242 BlockUser(const UserObj)`。</summary>
    public void BlockUser(IRest11SessionObj UserObj)
        => Rest11LoginGateMisc.BlockUser(UserObj, g_pConfig, Channel);

    /// <summary>`Misc.pas:275-286 SendGameCenterMsg(wIdent, sSendMsg)`。</summary>
    public void SendGameCenterMsg(ushort wIdent, string sSendMsg)
    {
        if (GameCenterChannel == null) return;
        Rest11LoginGateMisc.SendGameCenterMsg(wIdent, sSendMsg, GameCenterChannel);
    }

    public void Dispose() => StopService();
}

/// <summary>
/// 把既有 <see cref="GateSession"/>（`GatewayProtocol.cs:84-130`）+ LoginGate 独有残部状态
/// （`m_dwProtocolPassword` / 二级密码 / `DelayClose` / `RotateBits`）投影成
/// <see cref="IRest11SessionObj"/>。
///
/// <para>
/// **不造第三份会话实现**（§14.2）：会话的收发/缓冲/节流仍由 `GateSession` 承担；
/// 本类只补 LoginGate 副本独有的 4 项字段与 2 个方法（`RotateBits` 为静态，见
/// <see cref="Rest11LoginGateSession.RotateBits"/>）。
/// </para>
/// </summary>
public sealed class Rest11GateSessionAdapter : IRest11SessionObj
{
    private readonly GateSession _session;

    public Rest11GateSessionAdapter(GateSession session, int ipAddr, string ipText)
    {
        _session = session;
        IPAddr = ipAddr;
        IPText = ipText;
    }

    /// <summary>被包装的既有会话（`GXX.GatewayKit.GateSession`）。</summary>
    public GateSession Raw => _session;

    public int IPAddr { get; }
    public string IPText { get; }

    /// <summary>`m_tLastGameSvr <> nil and m_tLastGameSvr.Active`（LoginSrv 链路是否可用）。</summary>
    public bool LastGameSvrActive { get; set; }

    public int Socket => _session.SocketId;                    // m_pUserOBJ._SendObj.Socket
    public bool KickFlag { get; set; }                         // m_fKickFlag

    /// <summary>
    /// `ClientSession.pas:19 m_fHandleLogin: Byte`。
    /// ★ 可写：原文 `ProcessCltData:462` 每次处理客户端包都**无条件**置 2
    /// （车道 p14-logingate-wire 接线后由 `Rest11LoginGatePacketGate.DispatchCommand` 承担）。
    /// </summary>
    byte IRest11SessionObj.HandleLogin { get; set; }

    /// <summary>`ClientSession.pas:22 m_nSvrObject`（`SM_OUTOFCONNECTION` 的 `nRecog` 实参）。</summary>
    public int SvrObject { get; set; }

    public uint dwClientTimeOutTick { get; set; } = DelphiRTL.GetTickCount();  // :68
    public bool IsDelayClose { get; set; }                     // :78 m_IsDelayClose
    public uint dwDelayCloseTick { get; set; } = DelphiRTL.GetTickCount();     // :79

    // ---- ClientSession.pas 的 LoginGate 独有残部（SelGate 副本没有）----
    public uint m_dwProtocolPassword;                          // :27

    /// <summary>`ClientSession.pas:28 m_IsCanSetL2Password`（显式接口实现；底层字段同名）。</summary>
    public bool m_IsCanSetL2Password;                          // :28

    /// <summary>`ClientSession.pas:29 m_IsCanCheckL2Password`（显式接口实现；底层字段同名）。</summary>
    public bool m_IsCanCheckL2Password;                        // :29

    /// <summary>`IRest11SessionObj.m_IsCanSetL2Password` 的显式实现（`AppMain.pas:748` 置位 / `:502` 消费）。</summary>
    bool IRest11SessionObj.m_IsCanSetL2Password { get => m_IsCanSetL2Password; set => m_IsCanSetL2Password = value; }

    /// <summary>`IRest11SessionObj.m_IsCanCheckL2Password` 的显式实现（`AppMain.pas:750` 置位 / `:518` 消费）。</summary>
    bool IRest11SessionObj.m_IsCanCheckL2Password { get => m_IsCanCheckL2Password; set => m_IsCanCheckL2Password = value; }

    /// <summary>
    /// `ClientSession.pas:785-789 DelayClose(DelayTick)`：
    /// `m_dwDelayCloseTick := GetTickCount + DelayTick; m_IsDelayClose := True;`
    /// （到期判定在 `FuncForComm.TProcMsgThread.Run` :213）。
    /// </summary>
    public void DelayClose(uint DelayTick)
    {
        dwDelayCloseTick = DelphiRTL.GetTickCount() + DelayTick; // :787
        IsDelayClose = true;                                     // :788
    }
}
