using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.GatewayKit;
using GXX.GatewayKit.Rest11;
using GXX.LoginGate.Rest11;

namespace GXX.LoginGate;

/// <summary>
/// LoginGate.pas → LoginGateService.cs
/// 登录网关服务：客户端 ↔ 本网关 ↔ LoginSrv。
/// - 客户端方向：TCmdPack(TDefaultMessage) 6-Bit 编码帧（CM_* 消息）。
/// - 服务器方向：TSvrCmdPack 帧（RUNGATECODE + GM_* 命令），含会话打开/关闭/转发。
/// 对应原 AppMain.pas + ClientSession.pas + ClientThread.pas + FuncForComm.pas。
///
/// <para>
/// ★ 车道 **p14-logingate-wire** 把 p11 移植的 LoginGate 执法面（`GXX.GatewayKit.Rest11` +
/// `GXX.LoginGate.Rest11`）**接上**了运行路径。接线**默认关闭**：
/// <see cref="Rest11Options"/> 为 null 时，本类与 <see cref="GateService"/> 的既有行为
/// 逐字节不变（既有 5 键配置、既有 `CheckIP`、既有透传转发）。启用方式见
/// <see cref="Rest11Options"/> 的注释。
/// </para>
/// </summary>
public class LoginGateService : GateService
{
    // 会话表：SockID → 会话（对应 ClientSession.pas 的 TSessionInfo 列表）
    private readonly ConcurrentDictionary<int, GateSession> _sessions = new();
    private long _seq;

    public int SessionCount => _sessions.Count;

    /// <summary>既有配置文件路径（`Protocol.pas:20 _STR_CONFIG_FILE = '.\Config.ini'`）。</summary>
    public string ConfigFileName { get; }

    /// <summary>
    /// Rest11 执法面的开关集合。**默认 null（全部关闭）**。
    ///
    /// <para>
    /// 宿主显式赋值（必须在 <see cref="StartService"/> **之前**）即启用；
    /// 赋 <see cref="Rest11LoginGateOptions.All"/> 或逐项打开：
    /// <code>
    /// var svc = new LoginGateService(Rest11LoginGateOptions.All);
    /// svc.StartService();
    /// </code>
    /// 打开后：`GateService.CheckIP` 走原文两张黑名单表 + IP 段表 + `Count+1>Max` 每 IP 连接数；
    /// `LoadConfig` 额外读 `[LoginGate]/[Integer]/[Switch]/[Method]` 19 字段；
    /// 客户端帧走 `ProcessCltData` 的包头门控与 `CM_*` 分派；`TProcMsgThread.Run` 驱动
    /// 客户端超时踢线与 `DelayClose` 到期关闭。
    /// </para>
    /// <para>
    /// ★ 集成方修复（台账 §62.1 / X-P17-01）：本属性此前是 `{ get; set; }` **自动属性**，
    /// 它**隐藏**（而非覆写）基类 `GateService.Rest11Options` ⇒ **基类内部** `LoadConfig`/`CheckIP`
    /// 那三处判定读到的仍是基类恒 null 视图 ⇒ IP 黑名单/段表/超连判定**全部不可达且静默**。
    /// 现在改为**代理基类的可写访问器**（`Rest11OptionsValue`），对外公开 API 一字不变。
    /// </para>
    /// </summary>
    public Rest11LoginGateOptions? Rest11Options
    {
        get => Rest11OptionsValue;
        set => Rest11OptionsValue = value;
    }

    // ---- Rest11 接线（null 时全部路径与接线前逐字节一致）----
    private Rest11LoginGateEnforcement? _rest11Wire;
    private Thread? _rest11TickThread;
    private volatile bool _rest11TickRunning;

    /// <summary>已接到 `GateService` 判定链上的 Rest11 kernel（默认 null ⇒ `CheckIP` 短路）。</summary>
    protected override object? Rest11Kernel => _rest11Wire;

    /// <summary>已接线的 Rest11 kernel（供宿主/测试读取；未启用时为 null）。</summary>
    public Rest11LoginGateEnforcement? Rest11Wire => _rest11Wire;

    /// <summary>`ClientSession.pas:47 g_UserList` 用的会话槽数（`USER_ARRAY_COUNT = 1000 + 48`）。</summary>
    public static int Rest11UserArrayCount => Rest11LoginGateOptions.Rest11DefaultUserArrayCount;

    public LoginGateService() : this(null, @".\Config.ini")
    {
    }

    /// <summary>
    /// <paramref name="rest11Options"/> 为 null（默认）时**不构造任何 Rest11 设施**。
    /// </summary>
    public LoginGateService(Rest11LoginGateOptions? rest11Options, string configFileName = @".\Config.ini")
        : base(configFileName)
    {
        ConfigFileName = configFileName;
        Rest11Options = rest11Options;
        AttachRest11Kernel();
    }

    protected override int GateDefaultPort => 7000;   // 原默认登录网关端口
    protected override int ServerDefaultPort => 5600; // LoginSrv 网关接受端口
    public override string ServiceName => "登录网关";

    // =====================================================================================
    // Rest11 装配与生命周期（全部 opt-in；默认整段不执行）
    // =====================================================================================

    /// <summary>
    /// 按 <see cref="Rest11Options"/> 装配 kernel 并挂到 `GateService.Rest11Kernel`。
    ///
    /// <para>
    /// ⚠ 构造时 `Rest11Options == null` ⇒ 直接返回，**不构造 kernel、不建目录、不读 INI、
    /// 不起线程** ⇒ `GateService.CheckIP` / `LoadConfig` 的 Rest11 分支永不进入。
    /// </para>
    /// <para>
    /// ⚠ 一旦启用则 `Enabled = true`：**不能**在运行中改回 false 而不重启
    /// （`GateService.CheckIP` 的判定链已按"启用即走原文语义"固化）。需要回到默认语义时
    /// 用 `new LoginGateService()`（或重启进程）。
    /// </para>
    /// </summary>
    private void AttachRest11Kernel()
    {
        if (Rest11Options == null) return;

        _rest11Wire = new Rest11LoginGateEnforcement(this)
        {
            Enabled = true,
            KeepAliveIntervalMs = 20          // 见 D-P14-01（原文 FuncForComm.pas:106 是 1ms）
        };

        // GateService.LoadConfig 在基类构造里已经跑过（那时 _rest11Wire 还是 null）。
        // 此处按选项补一次原文段名读取：LoadConfig 的 Rest11 分支与这里的落点**同一个方法**，
        // 保证"打开即加载"不依赖构造顺序（见 D-P14-05）。
        if (Rest11Options.EnableLoginGateIniSections)
            _rest11Wire.LoadLoginGateConfigSections();
    }

    /// <summary>
    /// 启动 Rest11 的 `_IDM_TIMER_KEEP_ALIVE` / `_IDM_TIMER_THREAD_INFO` 驱动。
    ///
    /// <para>
    /// ★ **偏离登记 D-P14-06**：原文用 `SetTimer(g_hMainWnd, id, ms, @OnTimerProc)`（Win32 消息定时器），
    /// 托管用后台线程按 <see cref="Rest11LoginGateEnforcement.KeepAliveIntervalMs"/> 驱动。
    /// 既有 `GateMainForm` 的 5 秒 `OnKeepAliveTimer` **不参与**本设施（否则 UI 不在时设施失效），
    /// `GateService.OnKeepAliveTimer()` 的既有实现一字未改。
    /// </para>
    /// </summary>
    private void StartRest11TickLoop()
    {
        if (_rest11Wire == null || !_rest11Wire.Enabled) return;
        if (_rest11TickRunning) return;

        _rest11TickRunning = true;
        int period = _rest11Wire.KeepAliveIntervalMs;
        var t = new Thread(() =>
        {
            while (_rest11TickRunning)
            {
                try
                {
                    _rest11Wire.OnKeepAliveTick();      // FuncForComm.pas:578-580
                    _rest11Wire.OnThreadInfoTick();     // FuncForComm.pas:582-584
                }
                catch
                {
                    // 巡检异常不得打断服务（原文由 TTimer 边界兜底）
                }
                if (_rest11TickRunning) Thread.Sleep(period);
            }
        })
        {
            IsBackground = true,
            Name = ServiceName + "-Rest11Tick"
        };
        _rest11TickThread = t;
        t.Start();
    }

    private void StopRest11TickLoop()
    {
        _rest11TickRunning = false;
        try { _rest11TickThread?.Join(500); } catch (ThreadInterruptedException) { }
        _rest11TickThread = null;
    }

    // ---------------- 客户端方向 ----------------

    protected override void OnClientAccept(GateSession session)
    {
        base.OnClientAccept(session);
        _sessions[session.SocketId] = session;

        // ★ Rest11 会话登记（`CheckIP` 通过之后；null 时整支不执行）。
        //   原文落点：`AppMain.pas:1053 InterlockedExchange(Integer(g_UserList[m_dwSessionID]), Integer(CSession))`
        //   + `:1054 UserEnter()`（`ClientSession.pas:706-737` 的 `g_ProcMsgThread.AddSession`）。
        _rest11Wire?.OnClientAccepted(session);

        // 通知 LoginSrv 打开会话（SS_OPENSESSION）
        SendToServer(GatewayProtocol.BuildServerPacket((uint)session.SocketId, GatewayProtocol.GM_OPEN, 0, null, 0));
        SendLog($"新客户端连接 #{session.SocketId} ({session.RemoteIP})");
    }

    protected override void OnClientDisconnect(GateSession session)
    {
        base.OnClientDisconnect(session);
        if (_sessions.TryRemove(session.SocketId, out _))
        {
            // ★ Rest11 注销（`ClientSession.pas:739-783 UserLeave` 的 :753/:778）
            _rest11Wire?.OnClientClosed(session);

            // 通知 LoginSrv 关闭会话（SS_CLOSESESSION）
            SendToServer(GatewayProtocol.BuildServerPacket((uint)session.SocketId, GatewayProtocol.GM_CLOSE, 0, null, 0));
            SendLog($"客户端断开 #{session.SocketId}");
        }
    }

    /// <summary>每个会话的 `#`…`!` 切帧残片（对应原文 `UserReadBuffer` 的 `BufLen` 回写）。</summary>
    private readonly ConcurrentDictionary<int, byte[]> _recvResidual = new();

    protected override void OnClientReceive(GateSession session, byte[] buf, int offset, int len)
    {
        if (!IsRest11FrameGateActive)
        {
            // ★ 默认路径：与接线前逐字节一致（base 追加缓冲 **加** 透传客户端数据 → LoginSrv 的 GM_DATA 帧）。
            //
            // 【集成方修复 · 台账 §59.7】车道 p14-logingate-wire 曾把 GM_DATA 透传**搬进了 Rest11 分支**，
            // 默认路径只剩 `base.OnClientReceive`（仅追加缓冲、不转发）⇒ 关闭 Rest11 时网关**什么都不转发**，
            // 端到端集成用例卡在"等待 GM_DATA(0x0005) 帧超时"。
            // 证据：**接线前**提交 b61112bc 上同一集成用例 **2/2 通过（80 ms）**；接线后 **2/2 超时**。
            // 修法：把原文属于默认路径的两条语句放回这里（Rest11 分支不变，它有自己的转发路径）。
            base.OnClientReceive(session, buf, offset, len);
            var data = new byte[len];
            Array.Copy(buf, offset, data, 0, len);
            SendToServer(GatewayProtocol.BuildServerPacket((uint)session.SocketId, GatewayProtocol.GM_DATA, 0, data, len));
            return;
        }

        // 既有累加语义保持不变（`GateSession.AppendBuffer`：溢出即 ResetBuffer）
        base.OnClientReceive(session, buf, offset, len);

        // 原文 `AppMain.pas:1070-1127 UserReadBuffer`：按 `#`…`!` 切帧并逐帧 ProcessCltData；
        // 登录网关的默认转发（`A%d/#1%s!$`）也在这里做（`:577-579`）。
        var frames = ExtractFrames(session, buf, offset, len);
        if (frames.Count == 0) return;

        Rest11GateSessionAdapter? adapter = _rest11Wire!.GetAdapter(session.SocketId);
        if (adapter == null) return;

        if (_rest11Wire.PacketGate.ProcessClientFrames(adapter, frames))
        {
            _recvResidual.TryRemove(session.SocketId, out _);
            return;   // 已关连接：不再转发
        }
    }

    /// <summary>Rest11 的帧门控是否生效（未启用时为 false ⇒ 走默认透传）。</summary>
    private bool IsRest11FrameGateActive => _rest11Wire is { Enabled: true };

    /// <summary>
    /// `AppMain.pas:1086-1124` 的切帧：找 `'#'`，再找其后的 `'!'`，
    /// 载荷为两者**之间**的字节（`:1105-1106 Inc(pTRBuf, 2); iLen := pTRBuffer - pTRBuf`）。
    /// 未闭合的尾部保留到下一次接收（TCP 粘包/拆包；对应原文 `:1126 BufLen := pTRBuf - Buffer` 的回写）。
    /// </summary>
    private List<byte[]> ExtractFrames(GateSession session, byte[] buf, int offset, int len)
    {
        const byte HASH = 0x23;  // '#'
        const byte BANG = 0x21;  // '!'

        _recvResidual.TryGetValue(session.SocketId, out byte[]? residual);
        int residualLen = residual?.Length ?? 0;

        byte[] data;
        if (residualLen == 0)
        {
            data = new byte[len];
            Array.Copy(buf, offset, data, 0, len);
        }
        else
        {
            data = new byte[residualLen + len];
            Array.Copy(residual!, 0, data, 0, residualLen);
            Array.Copy(buf, offset, data, residualLen, len);
        }

        var frames = new List<byte[]>();
        int pos = 0;
        while (pos < data.Length)
        {
            // 找 '#'
            while (pos < data.Length && data[pos] != HASH) pos++;
            if (pos >= data.Length) break;

            // 找 '!'（原文 `:1094 if (dwEnd - DWORD(pTRBuf)) <= 2 then Break`）
            int bang = pos + 1;
            while (bang < data.Length && data[bang] != BANG) bang++;
            if (bang >= data.Length) break;                       // 未闭合 ⇒ 留作残片

            int payloadOff = pos + 1;                             // 原文 Inc(pTRBuf, 2) 的等效起点
            int payloadLen = bang - payloadOff;                   // :1106 iLen := pTRBuffer - pTRBuf
            if (payloadLen > 0)
            {
                var frame = new byte[payloadLen];
                Array.Copy(data, payloadOff, frame, 0, payloadLen);
                frames.Add(frame);
            }
            pos = bang + 1;
        }

        if (pos < data.Length)
        {
            var tail = new byte[data.Length - pos];
            Array.Copy(data, pos, tail, 0, tail.Length);
            _recvResidual[session.SocketId] = tail;
        }
        else
        {
            _recvResidual.TryRemove(session.SocketId, out _);
        }
        return frames;
    }

    // ---------------- 服务器方向 ----------------

    protected override void SendServerOpenGate()
    {
        // 对应 ClientThread.pas 连接成功后发送的网关注册信息（GM_CHECKSERVER 心跳由计时器周期发送）
        SendToServer(GatewayProtocol.BuildServerPacket(0, GatewayProtocol.GM_CHECKSERVER, 0, null, 0));
    }

    protected override void OnServerData(TcpLink link)
    {
        // 帧格式：TSvrCmdPack(20B) + Data
        while (link.AccumLength >= GatewayProtocol.SizeOfTSvrCmdPack)
        {
            var header = StructBytes.FromBytes<GatewayProtocol.TSvrCmdPack>(link.AccumBuffer, 0);
            if (header.Flag != GatewayProtocol.RUNGATECODE)
            {
                // 流失步：丢弃缓冲（对应原网关 Flag 校验失败断链）
                SendLog("上游帧 Flag 校验失败，重置链路缓冲", 1);
                link.ConsumeAccum(link.AccumLength);
                return;
            }
            int dataLen = header.DataLen;
            if (dataLen < 0 || dataLen > 64 * 1024)
            {
                link.ConsumeAccum(link.AccumLength);
                return;
            }
            if (link.AccumLength < GatewayProtocol.SizeOfTSvrCmdPack + dataLen)
                return; // 等待更多数据

            if (dataLen > 0)
            {
                byte[] data = new byte[dataLen];
                Array.Copy(link.AccumBuffer, GatewayProtocol.SizeOfTSvrCmdPack, data, 0, dataLen);
                DeliverToClient((int)header.SockID, header.Cmd, data);
            }
            else
            {
                DeliverToClient((int)header.SockID, header.Cmd, null);
            }
            link.ConsumeAccum(GatewayProtocol.SizeOfTSvrCmdPack + dataLen);
        }
    }

    private void DeliverToClient(int sockId, ushort cmd, byte[]? data)
    {
        switch (cmd)
        {
            case GatewayProtocol.GM_CLOSE:
                // 服务器要求断开该客户端
                if (_sessions.TryGetValue(sockId, out var sClose))
                    ClientIocp.CloseSession(sClose);
                break;
            case GatewayProtocol.GM_KICK:
                if (_sessions.TryGetValue(sockId, out var sKick))
                    ClientIocp.CloseSession(sKick);
                break;
            case GatewayProtocol.GM_DATA:
            default:
                if (data != null && data.Length > 0 && _sessions.TryGetValue(sockId, out var sData))
                {
                    // ★ Rest11：`AppMain.pas:735-753` 的 SM_* 二级密码许可 / DelayClose(8000)
                    //   （null 时整支不执行；未启用时既有转发逐字节不变）
                    NotifyRest11ServerMessage(sockId, data);
                    ClientIocp.Send(sData, data);
                }
                break;
        }
    }

    /// <summary>`AppMain.pas:747-752`：从服务器下发的包体里解出 `SM_*` 包头并置位 L2 许可。</summary>
    private void NotifyRest11ServerMessage(int sockId, byte[] data)
    {
        if (_rest11Wire is not { Enabled: true }) return;

        Rest11GateSessionAdapter? adapter = _rest11Wire.GetAdapter(sockId);
        if (adapter == null) return;

        var pkt = new byte[GatewayProtocol.SizeOfTSvrCmdPack];
        int n = Math.Min(data.Length, pkt.Length);
        Array.Copy(data, 0, pkt, 0, n);
        TDefaultMessage msg = EDcode.DecodeMessage(pkt);          // 原文 DecodeMessage(S)
        _rest11Wire.PacketGate.OnServerMessage(adapter, msg);
    }

    /// <summary>GM_CHECKSERVER 心跳（原 _IDM_TIMER_KEEP_ALIVE）。</summary>
    public void SendKeepAlive()
    {
        if (_serverConnected)
            SendToServer(GatewayProtocol.BuildServerPacket(0, GatewayProtocol.GM_CHECKSERVER, 0, null, 0));
    }

    // =====================================================================================
    // 启动 / 停止（在基类既有序列之外，仅做 Rest11 的 opt-in 附加）
    // =====================================================================================

    public override bool StartService()
    {
        bool ok = base.StartService();
        if (ok)
        {
            // ★ 偏离登记 D-P14-03：原文 `FuncForComm.pas:366-375` 的残部（`g_fServiceStarted` /
            //   `Enabled` / `LoadConfig` / `ClearConnectOfIP` / 两张黑名单表载入）挂在
            //   `StartService` 成功之后 —— 既有 `GateService.StartService` 已经先建好监听与上游线程，
            //   故这里**在基类之后**接线（不改变基类任何一步）。
            _rest11Wire?.OnServiceStarted();
            StartRest11TickLoop();
        }
        return ok;
    }

    public override void StopService()
    {
        if (!ServiceStarted) return;

        StopRest11TickLoop();
        _rest11Wire?.OnServiceStopping();     // FuncForComm.pas:433/457/459/460/462

        base.StopService();

        _recvResidual.Clear();
    }

    // =====================================================================================
    // 供 Rest11 kernel 回调的既有设施出口（无 Rest11 时不被调用）
    // =====================================================================================

    /// <summary>把一帧客户端载荷按原文 `A%d/#1%s!$`（`:577-579`）转发给 LoginSrv。</summary>
    internal void ForwardFrameToServer(int socketId, byte[] frame)
    {
        SendToServer(GatewayProtocol.BuildServerPacket(
            (uint)socketId, GatewayProtocol.GM_DATA, 0, frame, frame.Length));
    }

    /// <summary>直接给某个客户端会话发原始帧（`Rest11` 的 `SendDefMessage` 出口）。</summary>
    internal void SendRawToClient(int socketId, byte[] frame)
    {
        if (_sessions.TryGetValue(socketId, out var session))
            ClientIocp.Send(session, frame);
    }

    /// <summary>关闭某个客户端会话（`Rest11` 的 `SHSocket.FreeSocket` 出口）。</summary>
    internal void CloseClientSession(GateSession session) => ClientIocp.CloseSession(session);

    /// <summary>`Rest11` 的 `g_pLogMgr.Add(sMsg)` 出口（落到既有的 `GateService.SendLog`）。</summary>
    internal void AddRest11Log(string msg) => SendLog(msg, 5);
}
