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
    }

    protected abstract int GateDefaultPort { get; }
    protected abstract int ServerDefaultPort { get; }
    public abstract string ServiceName { get; }

    /// <summary>日志输出（原 SendLogMsg → GameCenter/窗体）。</summary>
    public event Action<string, int>? OnLogMsg;

    protected void SendLog(string msg, int level = 3) => OnLogMsg?.Invoke(msg, level);

    // ---------------- 启动/停止 ----------------

    public bool StartService()
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

    public void StopService()
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
