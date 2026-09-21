using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.GatewayKit;

/// <summary>网关/服务器 UI 绑定接口（供 GateMainForm 使用）。</summary>
public interface IGateUiService : IDisposable
{
    string ServiceName { get; }
    string GateAddr { get; }
    int GatePort { get; }
    string ServerAddr { get; }
    int ServerPort { get; }
    bool ServiceStarted { get; }
    int OnlineCount { get; }
    bool StartService();
    void StopService();
    event Action<string, int>? OnLogMsg;

    /// <summary>心跳计时器触发（对应 _IDM_TIMER_KEEP_ALIVE）。</summary>
    void OnKeepAliveTimer();
}

/// <summary>
/// 网关服务基类（对应 AppMain.pas/uFrmMain.pas + ClientThread.pas + IPAddrFilter.pas + ConfigManager.pas 公共语义）。
/// 链路：客户端 ↔ 本网关(端口 nGatePort) ↔ 上游服务器(端口 nServerPort)。
/// </summary>
public abstract class GateService : IGateUiService
{
    protected readonly TFastIniFile Config;
    protected readonly IocpManager ClientIocp;
    private GateSession? ServerSession;

    public string GateAddr { get; set; } = "0.0.0.0";
    public int GatePort { get; set; }
    public string ServerAddr { get; set; } = "127.0.0.1";
    public int ServerPort { get; set; }
    public bool ServiceStarted { get; protected set; }

    public int OnlineCount => ClientIocp.SessionCount;

    // IP 过滤（IPAddrFilter.pas）
    private readonly HashSet<uint> _blockList = new();
    private readonly Dictionary<uint, TPerIPAddr> _perIP = new();
    public TBlockIPMethod BlockIPMethod { get; set; } = TBlockIPMethod.mDisconnect;
    public int MaxConnOfIPaddr { get; set; } = 20;

    private static readonly object IpLock = new();

    protected GateService(string configFileName)
    {
        Config = new TFastIniFile(configFileName);
        ClientIocp = new IocpManager();
        ClientIocp.OnAccept += OnClientAccept;
        ClientIocp.OnReceive += OnClientReceive;
        ClientIocp.OnDisconnect += OnClientDisconnect;
        LoadConfig();
    }

    protected virtual void LoadConfig()
    {
        GateAddr = Config.ReadString("Gateway", "GateAddr", "0.0.0.0");
        GatePort = Config.ReadInteger("Gateway", "GatePort", GateDefaultPort);
        ServerAddr = Config.ReadString("Server", "ServerAddr", "127.0.0.1");
        ServerPort = Config.ReadInteger("Server", "ServerPort", ServerDefaultPort);
        MaxConnOfIPaddr = Config.ReadInteger("PacketRule", "MaxConnOfIPaddr", 20);

        // ★ opt-in 的 LoginGate 原文段名（[LoginGate]/[Integer]/[Switch]/[Method]，19 字段）
        //   —— 见本文件末尾 "可选（opt-in）LoginGate 执法面接线" 一节。
        //   默认（Rest11Kernel == null）不进入；**在既有 5 键之后**执行，故既有 5 键的读取
        //   与回写序列逐字节不变（Rest11 写的是另一组段名/键名，两者并存不冲突）。
        if (Rest11GateHook is { Enabled: true } k && (Rest11Options?.EnableLoginGateIniSections ?? false))
            k.LoadLoginGateConfigSections();
    }

    protected abstract int GateDefaultPort { get; }
    protected abstract int ServerDefaultPort { get; }
    public abstract string ServiceName { get; }

    /// <summary>日志输出（原 SendLogMsg → GameCenter/窗体）。</summary>
    public event Action<string, int>? OnLogMsg;

    protected void SendLog(string msg, int level = 3) => OnLogMsg?.Invoke(msg, level);

    // ---------------- 启动/停止 ----------------

    public virtual bool StartService()
    {
        if (ServiceStarted) return true;
        try
        {
            var listen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var addr = GateAddr == "0.0.0.0" ? IPAddress.Any : IPAddress.Parse(GateAddr);
            listen.Bind(new IPEndPoint(addr, GatePort));
            ClientIocp.StartListen(listen);

            var serverThread = new Thread(ServerLinkLoop)
            {
                IsBackground = true,
                Name = ServiceName + "-ServerLink"
            };
            serverThread.Start();

            ServiceStarted = true;
            SendLog(ServiceName + " 启动完成 " + GateAddr + ":" + GatePort);
            return true;
        }
        catch (Exception ex)
        {
            SendLog(ServiceName + " 启动失败: " + ex.Message, 1);
            return false;
        }
    }

    public virtual void StopService()
    {
        if (!ServiceStarted) return;
        ServiceStarted = false;
        ClientIocp.Stop();
        _serverLink?.Close();
        SendLog(ServiceName + " 已停止");
    }

    // ---------------- 上游服务器链路（ClientThread.pas 语义：断线重连）----------------

    private TcpLink? _serverLink;
    protected volatile bool _serverConnected;

    private void ServerLinkLoop()
    {
        var link = new TcpLink(ServerAddr, ServerPort);
        _serverLink = link;
        link.OnConnected += () =>
        {
            _serverConnected = true;
            ServerSession = new GateSession { SocketId = 0 };
            SendLog("已连接上游服务器 " + ServerAddr + ":" + ServerPort);
            SendServerOpenGate();
        };
        link.OnDisconnected += () =>
        {
            _serverConnected = false;
            ServerSession = null;
            SendLog("与上游服务器连接断开，将自动重连", 2);
        };
        link.OnReceive += (buf, off, len) =>
        {
            link.Accumulate(buf, off, len);
            OnServerData(link);
        };

        while (ServiceStarted)
        {
            if (!link.Connect())
            {
                SendLog("连接上游服务器失败，5 秒后重试", 2);
                try { Thread.Sleep(5000); } catch (ThreadInterruptedException) { return; }
                continue;
            }
            while (ServiceStarted && link.Connected)
                Thread.Sleep(500);
            link.Close();
            if (!ServiceStarted) return;
            try { Thread.Sleep(1000); } catch (ThreadInterruptedException) { return; }
        }
    }

    protected virtual void SendServerOpenGate()
    {
        // 对应 GM_OPEN / KeepAlive 注册帧（各网关差异由子类覆写）
    }

    protected void SendToServer(byte[] data) => _serverLink?.Send(data);

    protected abstract void OnServerData(TcpLink link);

    // ---------------- 客户端会话 ----------------

    protected virtual void OnClientAccept(GateSession session)
    {
        if (!CheckIP(session))
        {
            ClientIocp.CloseSession(session);
            return;
        }
    }

    protected virtual void OnClientDisconnect(GateSession session)
    {
        lock (IpLock)
        {
            uint ip = (uint)Share.MakeIPToInt(session.RemoteIP);
            if (ip != 0xFFFFFFFF && _perIP.TryGetValue(ip, out var rec))
                rec.Count--;
        }
    }

    protected virtual void OnClientReceive(GateSession session, byte[] buf, int offset, int len)
    {
        session.AppendBuffer(buf, offset, len);
    }

    private bool CheckIP(GateSession session)
    {
        uint ip = (uint)Share.MakeIPToInt(session.RemoteIP);

        // ★ opt-in 的 LoginGate 原文三连判定（`AcceptExWorkedThread.pas:576/587/598`）。
        //   默认（Rest11Kernel == null）在第一行短路 ⇒ 下行**一行都不执行**，
        //   既有 `_blockList`/`_perIP` 判定链与 SelGate/RunGate 行为逐字节不变。
        //   关闭时不做任何新的计数、不写任何表、不发任何日志。
        if (Rest11GateHook is { Enabled: true } rest11 && (Rest11Options?.EnableIpAddrFilterResidual ?? false))
        {
            if (rest11.IsBlockIP((int)ip))              // :576 IPAddrFilter.IsBlockIP（两张表）
            {
                rest11.LogBlockIP(session.RemoteIP);    // :579-580 if g_pLogMgr.CheckLevel(5)
                return false;                           // :582 bClose := True
            }
            if (rest11.IsBlockIPArea((int)ip))          // :587 IPAddrFilter.IsBlockIPArea（IP 段表）
            {
                rest11.LogBlockIPArea(session.RemoteIP);// :590-591
                return false;                           // :593 bClose := True
            }
            if (rest11.OverConnectOfIP((int)ip))        // :598 IPAddrFilter.OverConnectOfIP（Count+1 > Max）
            {
                rest11.LogOverConnectOfIP(session.RemoteIP); // :600-601
                return false;                           // :603 bClose := True
            }
        }

        lock (IpLock)
        {
            if (_blockList.Contains(ip)) return false;
            if (!_perIP.TryGetValue(ip, out var rec))
            {
                rec = new TPerIPAddr { IPaddr = (int)ip, Count = 0 };
                _perIP[ip] = rec;
            }
            rec.Count++;
            if (MaxConnOfIPaddr > 0 && rec.Count > MaxConnOfIPaddr)
            {
                SendLog("IP 超过连接限制: " + session.RemoteIP, 2);
                return false;
            }
        }
        return true;
    }

    public void AddBlockIP(string ip)
    {
        lock (IpLock) _blockList.Add((uint)Share.MakeIPToInt(ip));
    }

    // =====================================================================================
    // 可选（opt-in）LoginGate 执法面接线 —— 车道 p14-logingate-wire
    //
    // 背景：车道 p11-logingate-filter 把 `Source/LoginGate` 的 5 个 C 类/残部单元
    // （`Misc.pas` / `FuncForComm.pas` / `IPAddrFilter.pas` / `ConfigManager.pas` /
    // `ClientSession.pas`）1:1 移植成了**并存设施**（`GXX.GatewayKit.Rest11` +
    // `GXX.LoginGate.Rest11`），但当时**没有接线**（kernal 默认不构造）。
    // 本节把"可接线"变成"已接线"：`GateService.CheckIP`（:203）与 `LoadConfig`（:67）
    // 的判定链上提供**默认不生效**的插入点。
    //
    // ★★ 三条硬约束（本节的每一个成员都必须满足）：
    //   1. **默认 OFF**：`Rest11Options` 默认返回 null ⇒ `CheckIP` / `LoadConfig` 里
    //      `Rest11Kernel is { }` 在第一行短路，**不执行任何新分支、不做任何新计数、
    //      不写任何文件**；默认路径行为与接线前逐字节一致。
    //   2. **绝不改变 SelGate/RunGate 行为**：这两个网关不覆写 `Rest11Options`/`Rest11Kernel`
    //      ⇒ 本节对本类新增的成员对它们是**死代码**。SelGate 侧三处头注
    //      （`SelGateIPAddrFilter.cs:17-25`、`SelGateMisc.cs:11-16`、`SelGateSession.cs:17-24`）
    //      要求的"与 GatewayKit 的差异必须保留"由此满足；门禁含 `GXX.SelGate.Tests` 全绿。
    //   3. **虚分派必须保留**：`CheckIP` 是 `private` 且只被 `OnClientAccept` 调用，
    //      这里**不改它的可见性与调用点**，只在方法体内插入提前返回分支。
    //
    // Rest11 设施与既有设施的关系是**并存**，不替换：
    //   · 既有 `_blockList` + `_perIP`（判据 `rec.Count > Max`，**先自增再比**）继续生效；
    //   · Rest11 另有**两张**表（永久 + 临时）与判据 `Count + 1 > Max`（超限**不自增**），
    //     以及既有设施**完全没有**的 IP 段过滤（`IsBlockIPArea`）与换 ID 频率限制
    //     （`CheckNewIDOfIP`）—— 逐项对照见
    //     `GXX.CSharp/docs/并行报告-p14-logingate-wire.md` 的接线对照表。
    // =====================================================================================

    /// <summary>
    /// Rest11 设施的开关集合。**默认 null**（= 全部关闭）。
    ///
    /// <para>
    /// 只有 `GXX.LoginGate.LoginGateService` 覆写它；`SelGate`/`RunGate` 不覆写 ⇒
    /// 本类新增的所有 opt-in 分支对它们永不进入。
    /// </para>
    /// </summary>
    protected virtual GXX.GatewayKit.Rest11.Rest11LoginGateOptions? Rest11Options => null;

    /// <summary>
    /// Rest11 LoginGate 执法 kernel（`GXX.LoginGate.Rest11.Rest11LoginGateKernel`）。
    /// **默认 null**；由 `LoginGateService` 在构造末尾按选项装配。
    ///
    /// <para>
    /// 返回类型是 `object?` 而不是具体类型，是为了让 `GXX.GatewayKit` **不引用**
    /// `GXX.LoginGate`（依赖方向保持 GatewayKit ← LoginGate）。实际使用时由
    /// `LoginGateService` 覆写为协变返回 `Rest11LoginGateKernel?`，
    /// `GateService` 内部经 <see cref="IRest11GateEnforcement"/> 接缝访问。
    /// </para>
    /// </summary>
    protected virtual object? Rest11Kernel => null;

    /// <summary>
    /// `GateService` 侧访问 Rest11 kernel 的**最小接缝**（只列 `CheckIP` 判定链与
    /// `LoadConfig` 判定链需要的成员）。由 `GXX.LoginGate.Rest11.Rest11LoginGateKernel` 实现，
    /// 从而在 GatewayKit 内不出现对 LoginGate 项目类型的静态引用。
    /// </summary>
    public interface IRest11GateEnforcement
    {
        /// <summary>本设施是否启用（默认 false ⇒ 不进入任何 Rest11 分支）。</summary>
        bool Enabled { get; }

        /// <summary>`IPAddrFilter.pas:149-176 IsBlockIP`（永久表 + 临时表）。</summary>
        bool IsBlockIP(int nRemoteIP);

        /// <summary>`IPAddrFilter.pas:315-335 IsBlockIPArea`（IP 段表，`ReverseIP` 后闭区间）。</summary>
        bool IsBlockIPArea(int nRemoteIP);

        /// <summary>`IPAddrFilter.pas:178-207 OverConnectOfIP`（每 IP 连接数，`Count + 1 > Max`）。</summary>
        bool OverConnectOfIP(int Addr);

        /// <summary>`AcceptExWorkedThread.pas:579-580` 的日志（`CheckLevel(5)` 门）。</summary>
        void LogBlockIP(string szRemoteIP);

        /// <summary>`AcceptExWorkedThread.pas:590-591` 的日志（`CheckLevel(5)` 门）。</summary>
        void LogBlockIPArea(string szRemoteIP);

        /// <summary>`AcceptExWorkedThread.pas:600-601` 的日志（`CheckLevel(5)` 门）。</summary>
        void LogOverConnectOfIP(string szRemoteIP);

        /// <summary>`ConfigManager.pas:143-210 LoadConfig` 的 19 字段 + 原文段名。</summary>
        void LoadLoginGateConfigSections();
    }

    /// <summary>
    /// 便捷出口：把 <see cref="Rest11Kernel"/> 按接缝取出。
    /// 返回 null 表示"未接线/未启用"，调用方据此短路。
    /// </summary>
    protected IRest11GateEnforcement? Rest11GateHook => Rest11Kernel as IRest11GateEnforcement;

    public void Dispose()
    {
        StopService();
        ClientIocp.Dispose();
    }

    /// <summary>IGateUiService.OnKeepAliveTimer：子类可覆写发送心跳。</summary>
    public virtual void OnKeepAliveTimer() { }
}

public struct TPerIPAddr
{
    public int IPaddr;
    public int Count;
}

public enum TBlockIPMethod : byte
{
    mDisconnect,
    mBlock,
    mBlockList
}
