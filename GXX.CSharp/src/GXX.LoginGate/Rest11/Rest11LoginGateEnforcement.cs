using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.GatewayKit;
using GXX.GatewayKit.Rest11;

namespace GXX.LoginGate.Rest11;

/// <summary>
/// 车道 p14-logingate-wire：把 `Rest11LoginGateKernel` 那一批 opt-in 设施**真正接到**
/// `LoginGateService` / `GateService` 的运行路径上。
///
/// <para>
/// ⚠ **opt-in 且默认不生效**：本类只在 `LoginGateService.Rest11Options != null`
/// 时被构造。`Rest11LoginGateOptions` 默认全 false、`LoginGateService` 默认
/// `Rest11Options == null` ⇒ 本类**根本不存在**，`GateService.CheckIP` / `LoadConfig` /
/// `OnClientAccept` / `OnClientDisconnect` / `OnClientReceive` 的默认路径不执行任何新代码。
/// </para>
///
/// <para>
/// 本类**不是**第三份实现（§14.2）：
/// <list type="bullet">
///   <item>黑名单两张表 / IP 段表 / 每 IP 连接数 / 换 ID 频率 —— 复用
///         <see cref="Rest11LoginGateIpFilter"/>（`IPAddrFilter.pas` 的 1:1 移植）；</item>
///   <item>执法副作用（`FreeSocket` / `AddToBlockIPList` / `SendDefMessage` / 日志）——
///         实现 <see cref="IRest11EnforcementChannel"/>，全部落到**既有**
///         `IocpManager.CloseSession` / `LoginGateService.SendRawToClient` / `SendLog`；</item>
///   <item>会话 —— 复用既有 <see cref="GateSession"/>（<see cref="Rest11GateSessionAdapter"/> 只是投影）；</item>
///   <item>装帧 —— 复用 <see cref="EDcode.EncodeBuffer"/> 与
///         <see cref="Rest11LoginGateSession.BuildDefMessageFrame"/>；</item>
///   <item>踢线/封禁 —— 复用 <see cref="Rest11LoginGateMisc.KickUser(IRest11SessionObj, Rest11LoginGateConfig, IRest11EnforcementChannel)"/>。</item>
/// </list>
/// </para>
///
/// <para>
/// ⚠ **不改 SelGate / RunGate**：本类只被 `GXX.LoginGate` 引用；`GXX.GatewayKit` 经
/// <see cref="GateService.IRest11GateEnforcement"/> 接缝访问它（`GateService.Rest11Kernel`
/// 声明为 `object?`），因此 GatewayKit **不引用** LoginGate，依赖方向保持 GatewayKit ← LoginGate。
/// SelGate 侧三处头注（`SelGateIPAddrFilter.cs:17-25`、`SelGateMisc.cs:11-16`、
/// `SelGateSession.cs:17-24`）要求的差异不受影响（`GXX.SelGate.Tests` 门禁全绿）。
/// </para>
///
/// <para>
/// 原文缺陷**照抄**（逐条断言见 `tests/GXX.GatewayKit.Tests/Rest14*.cs`）：
/// <list type="number">
///   <item>`OverConnectOfIP` 判据 `Count + 1 > Max` 且超限**不自增**（`IPAddrFilter.pas:193-196`）；
///         既有 `GateService._perIP` 是 `Count > Max`（先自增再比）⇒ 两者**并存**、不统一；</item>
///   <item>`KickUser` 的 `mDisconnect` 分支**不写任何黑名单**（`Misc.pas:186-189`）；</item>
///   <item>`KickUser(const UserObj)` **先 `FreeSocket` 再置 `m_fKickFlag`**（`Misc.pas:211-212`），
///         与 `BlockUser` 顺序相反；</item>
///   <item>`ProcessCltData` 的 `:219-229` 对**任何**含 `'$'` 的包踢线（与 `m_fDefenceCCPacket` 无关）；</item>
///   <item>`:462 m_fHandleLogin := 2` 是**无条件**赋值（不在 `if` 内）；</item>
///   <item>`:489-520` 的 L2 门控是 `if/else if` 链 ⇒ `CM_CHECKL2PASSWORD` 恒**不**消费
///         `m_IsCanSetL2Password`，`CM_SETL2PASSWORD` 恒**不**消费 `m_IsCanCheckL2Password`；</item>
///   <item>`else` 分支（`:679-685`）对**未列入白名单**的 `CM_*` 一律踢线。</item>
/// </list>
/// </para>
/// </summary>
public sealed class Rest11LoginGateEnforcement
    : IRest11EnforcementChannel, GateService.IRest11GateEnforcement, IRest11PacketGateHost, IRest11GateLogger
{
    private readonly LoginGateService _owner;

    /// <summary>永久黑名单的运行时镜像（真值在 `IPAddrFilter.g_BlockIPList`）。</summary>
    private readonly HashSet<int> _blockList = new();

    /// <summary>临时黑名单的运行时镜像（真值在 `IPAddrFilter.g_TempBlockIPList`）。</summary>
    private readonly HashSet<int> _tempBlockList = new();

    /// <summary>会话适配器缓存（`SocketId → Rest11GateSessionAdapter`）。</summary>
    private readonly Dictionary<int, Rest11GateSessionAdapter> _adapters = new();

    /// <summary>`ClientSession.pas:47 g_UserList`（原文 `array[0..USER_ARRAY_COUNT-1]`，此处定长数组）。</summary>
    private readonly IRest11SessionObj?[] _sessionSlots;

    /// <summary>原文 `FuncForComm.pas:77 g_ProcMsgThread: TProcMsgThread`。</summary>
    public Rest11ProcMsgThread g_ProcMsgThread { get; }

    /// <summary>原文 `ConfigManager.pas:52 g_pConfig: TConfigMgr`（19 字段，`[LoginGate]` 原文段名）。</summary>
    public Rest11LoginGateConfig g_pConfig { get; }

    /// <summary>`IPAddrFilter.pas:8-15` 的全部全局表（1:1 移植）。</summary>
    public Rest11LoginGateIpFilter IPAddrFilter { get; }

    /// <summary>`ClientSession.pas:47 g_UserList` 的只读投影。</summary>
    public IReadOnlyList<IRest11SessionObj?> g_UserList => _sessionSlots;

    /// <summary>`Protocol.pas:109 g_fServiceStarted`。</summary>
    public volatile bool g_fServiceStarted;

    /// <summary>本设施是否启用；由宿主按 `Rest11Options` 置位。默认 **false**。</summary>
    public bool Enabled { get; set; }

    /// <summary>`SendMessage(g_hGameCenterHandle, WM_COPYDATA, ...)` 的通道（可选，由宿主提供）。</summary>
    public IRest11GameCenterChannel? GameCenterChannel { get; set; }

    /// <summary>`FuncForComm.pas:507-561 ShowThreadInfo` 的服务器信息源（可选，未提供时该计时器空转）。</summary>
    public IRest11ThreadInfoSource? ThreadInfoSource { get; set; }

    /// <summary>
    /// 客户端超时巡检（`FuncForComm.pas:180-257 TProcMsgThread.Run`）的驱动周期（毫秒）。
    ///
    /// <para>
    /// ★ **偏离登记 D-P14-01**：原文 `:106 Self.Interval := 1`（1ms）。托管默认 **20ms** ——
    /// 一次巡检的实际开销很小（遍历会话快照），但 1ms 周期在真实服务器上会持续占用一个核心；
    /// 20ms 对 `m_nClientTimeOutTime`（下限 10 秒）与 `DelayClose` 的判定等价（误差 &lt;0.2%）。
    /// 需要逐字对齐时把本属性置 1，或把宿主计时器的周期设为 1ms。
    /// </para>
    /// </summary>
    public int KeepAliveIntervalMs { get; set; } = 20;

    /// <summary>
    /// `ClientSession.pas:157-401` 的包头门控 + `:462-687` 的 `CM_*` 分派。
    /// 落在 `GXX.GatewayKit.Rest11`（可被 `GXX.GatewayKit.Tests` 直接单测），
    /// 本类作为它的 <see cref="IRest11PacketGateHost"/> 提供四个既有出口。
    /// </summary>
    public Rest11LoginGatePacketGate PacketGate { get; }

    public Rest11LoginGateEnforcement(LoginGateService owner)
    {
        _owner = owner;
        g_pConfig = new Rest11LoginGateConfig(Rest11ConfigPath);
        IPAddrFilter = new Rest11LoginGateIpFilter
        {
            Config = g_pConfig,
            BaseDirectory = Rest11ConfigDirectory
        };
        g_ProcMsgThread = new Rest11ProcMsgThread();
        _sessionSlots = new IRest11SessionObj?[Rest11LoginGateOptions.Rest11DefaultUserArrayCount];
        PacketGate = new Rest11LoginGatePacketGate(g_pConfig, this, () => g_UserList)
        {
            Host = this,
            Logger = this
        };
    }

    // =====================================================================================
    // 落点：原文 `Protocol.pas:20 _STR_CONFIG_FILE = '.\Config.ini'`
    //       `IPAddrFilter.pas:21 _STR_BLOCK_FILE = '.\BlockIPList.txt'`
    //       `IPAddrFilter.pas:22 _STR_BLOCK_AREA_FILE = '.\BlockIPAreaList.txt'`
    // 相对路径基准取**既有配置文件的真实目录**（而不是进程 CWD）：外部传入的
    // `.\Config.ini` 展开结果与实际 CWD 一致时两者等价；不同时以配置文件所在目录为准，
    // 避免宿主 CWD 漂移导致黑名单文件落到别处。
    // =====================================================================================

    /// <summary>`.\Config.ini` 的全路径（与 `LoginGateService` 的既有配置文件同一份）。</summary>
    public string Rest11ConfigPath => Path.GetFullPath(_owner.ConfigFileName);

    /// <summary>黑名单等相对路径的解析基准。</summary>
    public string Rest11ConfigDirectory =>
        Path.GetDirectoryName(Rest11ConfigPath) is { Length: > 0 } d ? d : ".";

    // =====================================================================================
    // FuncForComm.pas:358-427 StartService（残部）
    //   :364 GetTickCount / :366 g_fServiceStarted := True / :368 g_ProcMsgThread.Enabled := True /
    //   :361 LoadBlockIPList / :362 LoadBlockIPAreaList / :373 g_pConfig.LoadConfig /
    //   :375 IPAddrFilter.ClearConnectOfIP / :404 SetTimer(_IDM_TIMER_THREAD_INFO, 1000)
    // FuncForComm.pas:429-465 StopService（残部）
    //   :433 g_fServiceStarted := False / :457 ClearConnectOfIP / :459 SaveBlockIPList /
    //   :460 SaveBlockIPAreaList / :462 Enabled := False
    // =====================================================================================

    /// <summary>对应 `LoginGateService.StartService` **成功之后**的接线（见 D-P14-03）。</summary>
    public void OnServiceStarted()
    {
        if (!Enabled) return;

        g_fServiceStarted = true;                                  // :366
        g_ProcMsgThread.Enabled = true;                            // :368
        g_ProcMsgThread.Interval = KeepAliveIntervalMs;             // :106 托管周期（见 D-P14-01）

        IPAddrFilter.LoadBlockIPList();                            // :361
        SyncPermanentBlockMirror();
        IPAddrFilter.LoadBlockIPAreaList();                        // :362

        if (_owner.Rest11Options?.EnableLoginGateIniSections == true)
            g_pConfig.LoadConfig();                                // :373（**在 5 键之后**，见 D-P14-05）

        if (g_pConfig.m_fCheckNullSession)
            IPAddrFilter.ClearConnectOfIP();                       // :375（开关为假时原文自身 Exit）
    }

    /// <summary>对应 `LoginGateService.StopService` 的接线（`FuncForComm.pas:429-465` 残部）。</summary>
    public void OnServiceStopping()
    {
        if (!Enabled) return;

        g_fServiceStarted = false;                                 // :433
        if (g_pConfig.m_fCheckNullSession)
            IPAddrFilter.ClearConnectOfIP();                       // :457
        IPAddrFilter.SaveBlockIPList();                            // :459
        IPAddrFilter.SaveBlockIPAreaList();                        // :460
        g_ProcMsgThread.Enabled = false;                           // :462
    }

    /// <summary>
    /// `FuncForComm.pas:563-587 OnTimerProc` 的 `_IDM_TIMER_KEEP_ALIVE` 分支（`:578-580`）：
    /// 一次 `KeepAlive()` ⇒ `TProcMsgThread.Run` 的客户端超时踢线 + `DelayClose` 到期关闭。
    /// </summary>
    public void OnKeepAliveTick() => g_ProcMsgThread.Run(this, g_pConfig);

    /// <summary>`_IDM_TIMER_THREAD_INFO`（1000ms，`:582-584`）分支。</summary>
    public void OnThreadInfoTick()
    {
        if (!g_fServiceStarted) return;                            // ShowThreadInfo :514
        if (ThreadInfoSource == null) return;
        Rest11LoginGateProcMsgFormat.ShowThreadInfo(ThreadInfoSource);
    }

    // =====================================================================================
    // 会话进出（原文 AppMain.pas 的 g_UserList 登记 + ClientSession.pas:706-783）
    // =====================================================================================

    /// <summary>
    /// 客户端接入（`GateService.OnClientAccept` 里 `CheckIP` 通过**之后**调用）。
    /// 会话落槽用 `SocketId % USER_ARRAY_COUNT`（原文 `g_UserList` 是定长数组、
    /// 由 AcceptEx 的 socket 序号决定下标；托管取模以免越界 ⇒ **容量语义偏离 D-P14-04**）。
    /// </summary>
    public void OnClientAccepted(GateSession session)
    {
        if (!Enabled) return;

        var adapter = new Rest11GateSessionAdapter(session, IpAddrOf(session), session.RemoteIP)
        {
            // 原文判据 `m_tLastGameSvr <> nil and m_tLastGameSvr.Active`：能接受客户端即上游链路在
            LastGameSvrActive = true
        };

        lock (_adapters) _adapters[session.SocketId] = adapter;

        lock (_sessionSlots)
        {
            int slot = session.SocketId % _sessionSlots.Length;
            if (slot < 0) slot += _sessionSlots.Length;
            _sessionSlots[slot] = adapter;
        }

        g_ProcMsgThread.AddSession(adapter);                       // ClientSession.pas:713
    }

    /// <summary>客户端断开（`GateService.OnClientDisconnect` 之后）。对应 `ClientSession.pas:752-753/778`。</summary>
    public void OnClientClosed(GateSession session)
    {
        if (!Enabled) return;

        Rest11GateSessionAdapter? adapter;
        lock (_adapters)
        {
            if (!_adapters.TryGetValue(session.SocketId, out adapter)) return;
            _adapters.Remove(session.SocketId);

            IPAddrFilter.DeleteConnectOfIP(adapter.IPAddr);        // :753
            g_ProcMsgThread.DelSession(adapter);                   // :778
        }

        lock (_sessionSlots)
        {
            for (int i = 0; i < _sessionSlots.Length; i++)
                if (ReferenceEquals(_sessionSlots[i], adapter)) _sessionSlots[i] = null;
        }
    }

    /// <summary>关闭一个连接（`Misc.pas` 的 `SHSocket.FreeSocket` 落点）。</summary>
    public void FreeSocket(int socket)
    {
        Rest11GateSessionAdapter? adapter;
        lock (_adapters) _adapters.TryGetValue(socket, out adapter);
        if (adapter?.Raw is { } raw) _owner.CloseClientSession(raw);   // 既有 IocpManager.CloseSession
    }

    /// <summary>`Misc.pas:172` `UserObj.SendDefMessage(SM_OUTOFCONNECTION, m_nSvrObject, 0,0,0,'')`。</summary>
    public void SendOutOfConnection(IRest11SessionObj session, int nSvrObject)
    {
        if (!session.LastGameSvrActive) return;                    // SendDefMessage :129-130 早退

        byte[] frame = Rest11LoginGateSession.BuildDefMessageFrame(
            Grobal2Const.SM_OUTOFCONNECTION, nSvrObject, 0, 0, 0, "");
        _owner.SendRawToClient(session.Socket, frame);
    }

    // =====================================================================================
    // IRest11EnforcementChannel —— 执法副作用全部落到既有设施
    // =====================================================================================

    /// <summary>`g_fServiceStarted`（`Misc.pas:160` / `FuncForComm.pas:188` 的早退判据）。</summary>
    bool IRest11EnforcementChannel.ServiceStarted => g_fServiceStarted;

    void IRest11EnforcementChannel.FreeSocket(int socket) => FreeSocket(socket);

    void IRest11EnforcementChannel.SendOutOfConnection(IRest11SessionObj session, int nSvrObject)
        => SendOutOfConnection(session, nSvrObject);

    /// <summary>`IPAddrFilter.pas:126-147 AddToTempBlockIPList(nIP)`（临时表只存在于内存）。</summary>
    public void AddToTempBlockIPList(int nIP)
    {
        lock (_tempBlockList) _tempBlockList.Add(nIP);
        IPAddrFilter.AddToTempBlockIPList(nIP);                    // 同步进移植后的表（IsBlockIP 用它）
    }

    /// <summary>`IPAddrFilter.pas:91-112 AddToBlockIPList(nIP)`（永久表，随 `SaveBlockIPList` 落盘）。</summary>
    public void AddToBlockIPList(int nIP)
    {
        lock (_blockList) _blockList.Add(nIP);
        IPAddrFilter.AddToBlockIPList(nIP);
    }

    /// <summary>`g_pLogMgr.CheckLevel(nLevel)`：以 `m_nShowLogLevel`（`ConfigManager.pas:20/151`）为门槛。</summary>
    public bool CheckLevel(int level) => g_pConfig.m_nShowLogLevel >= level;

    /// <summary>`g_pLogMgr.Add(sMsg)`。</summary>
    public void AddLog(string sMsg) => _owner.AddRest11Log(sMsg);

    // ---- IRest11GateLogger（`ProcessCltData` 包头门控用的同一套门）----
    bool IRest11GateLogger.CheckLevel(int level) => CheckLevel(level);
    void IRest11GateLogger.AddLog(string sMsg) => AddLog(sMsg);

    // =====================================================================================
    // GateService.IRest11GateEnforcement —— CheckIP / LoadConfig 判定链接缝
    // =====================================================================================

    /// <summary>
    /// `IPAddrFilter.pas:149-176 IsBlockIP`（永久 + 临时两张表，**并存于既有 `_blockList` 之外**）。
    ///
    /// <para>
    /// 入参是**点分字符串**（见 `GateService.CheckIP` 的 D-P14-08）：原文这三处传的是
    /// `pRemoteSockaddr.sin_addr.S_addr`（`in_addr` 的网络序 DWORD），它与 `IPAddrFilter`
    /// 表里存的 `inet_addr()` 返回值**同口径**；而既有 `Share.MakeIPToInt` 与 `inet_addr`
    /// 逐字节相反 ⇒ 统一在 Rest11 侧用 `Rest11LoginGateNet.InetAddr` 换算。
    /// </para>
    /// </summary>
    public bool IsBlockIP(string remoteIP) => IPAddrFilter.IsBlockIP(Rest11LoginGateNet.InetAddr(remoteIP));

    /// <summary>`IPAddrFilter.pas:315-335 IsBlockIPArea`（IP 段表，`ReverseIP` 后闭区间）。</summary>
    public bool IsBlockIPArea(string remoteIP) => IPAddrFilter.IsBlockIPArea(Rest11LoginGateNet.InetAddr(remoteIP));

    /// <summary>`IPAddrFilter.pas:178-207 OverConnectOfIP`（原文 `Count + 1 > Max`）。</summary>
    public bool OverConnectOfIP(string remoteIP) => IPAddrFilter.OverConnectOfIP(Rest11LoginGateNet.InetAddr(remoteIP));

    /// <summary>`AcceptExWorkedThread.pas:579-580`：`if g_pLogMgr.CheckLevel(5) then Add('Block IP: %s')`。</summary>
    public void LogBlockIP(string szRemoteIP)
    {
        if (CheckLevel(5)) AddLog("Block IP: " + szRemoteIP);
    }

    /// <summary>`AcceptExWorkedThread.pas:590-591`：`'Block IP Area: %s'`。</summary>
    public void LogBlockIPArea(string szRemoteIP)
    {
        if (CheckLevel(5)) AddLog("Block IP Area: " + szRemoteIP);
    }

    /// <summary>`AcceptExWorkedThread.pas:600-601`：`Format('超过每IP连接[%d]: %s', [m_nMaxConnectOfIP, szRemoteIP])`。</summary>
    public void LogOverConnectOfIP(string szRemoteIP)
    {
        if (CheckLevel(5))
            AddLog("超过每IP连接[" + DelphiRTL.IntToStr(g_pConfig.m_nMaxConnectOfIP) + "]: " + szRemoteIP);
    }

    /// <summary>`ConfigManager.pas:143-210 LoadConfig`：19 字段 + `[LoginGate]/[Integer]/[Switch]/[Method]`。</summary>
    public void LoadLoginGateConfigSections() => g_pConfig.LoadConfig();

    // =====================================================================================
    // IRest11PacketGateHost —— 让 GatewayKit 侧的 1:1 分派落到既有设施
    //
    // 四个出口在 LoginGate 侧全部已有：
    //   SendRawToClient      → IocpManager.Send           （原文 m_tIOCPSender.SendData）
    //   CloseClientSession   → IocpManager.CloseSession   （原文 SHSocket.FreeSocket）
    //   ForwardFrameToServer → GateService.SendToServer(GM_DATA)（原文 m_tLastGameSvr.SendBuffer）
    //   AddRest11Log         → GateService.SendLog        （原文 g_pLogMgr.Add）
    // =====================================================================================

    /// <summary>`ClientSession.pas:443`/`:533` 的 `m_tIOCPSender.SendData`。</summary>
    public void SendRawToClient(int socket, byte[] frame) => _owner.SendRawToClient(socket, frame);

    /// <summary>原文 `Succeed := False` 后由宿主 `SHSocket.FreeSocket` 关连接。</summary>
    public void CloseClientSession(IRest11SessionObj session)
    {
        if (session is Rest11GateSessionAdapter adapter) _owner.CloseClientSession(adapter.Raw);
    }

    /// <summary>原文 `Succeed := False` 后由宿主 `SHSocket.FreeSocket` 关连接（按 socket）。</summary>
    public void CloseClientSession(int socket)
    {
        if (GetAdapter(socket)?.Raw is { } raw) _owner.CloseClientSession(raw);
    }

    /// <summary>`ClientSession.pas:578-579` 的 `m_tLastGameSvr.SendBuffer('A%d/#1%s!$')`。</summary>
    void IRest11PacketGateHost.ForwardFrameToServer(IRest11SessionObj session, byte[] frame)
        => _owner.ForwardFrameToServer(session.Socket, frame);

    /// <summary>`g_pLogMgr.Add(sMsg)`。</summary>
    public void AddRest11Log(string msg) => _owner.AddRest11Log(msg);

    // =====================================================================================
    // 内部工具
    // =====================================================================================

    /// <summary>永久黑名单镜像 ← `IPAddrFilter.g_BlockIPList` 的单向同步（载入后调用一次）。</summary>
    private void SyncPermanentBlockMirror()
    {
        var list = IPAddrFilter.g_BlockIPList;
        var snapshot = new List<int>();
        for (int i = 0; i < list.Count; i++)
        {
            if (list.GetObject(i) is int n) snapshot.Add(n);
        }
        lock (_blockList)
        {
            _blockList.Clear();
            foreach (int n in snapshot) _blockList.Add(n);
        }
    }

    /// <summary>临时表镜像（供测试与报告取证；`IsBlockIP` 走 `IPAddrFilter`）。</summary>
    public IReadOnlyCollection<int> TempBlockIps { get { lock (_tempBlockList) return new List<int>(_tempBlockList); } }

    /// <summary>永久表镜像（同上）。</summary>
    public IReadOnlyCollection<int> PermanentBlockIps { get { lock (_blockList) return new List<int>(_blockList); } }

    /// <summary>`pRemoteSockaddr.sin_addr.S_addr`（`AcceptExWorkedThread.pas:576`）的等价整数。</summary>
    public static int IpAddrOf(GateSession session) => Rest11LoginGateNet.InetAddr(session.RemoteIP);

    /// <summary>取会话适配器（测试与宿主用）。</summary>
    public Rest11GateSessionAdapter? GetAdapter(int sockId)
    {
        lock (_adapters) return _adapters.TryGetValue(sockId, out var a) ? a : null;
    }

    /// <summary>按会话对象取适配器（`IRest11PacketGateHost` 的关闭出口用）。</summary>
    public Rest11GateSessionAdapter? GetAdapterOf(IRest11SessionObj session)
    {
        lock (_adapters)
        {
            foreach (var kv in _adapters)
                if (ReferenceEquals(kv.Value, session)) return kv.Value;
        }
        return null;
    }

    /// <summary>当前在册会话数（测试用）。</summary>
    public int AdapterCount { get { lock (_adapters) return _adapters.Count; } }
}
