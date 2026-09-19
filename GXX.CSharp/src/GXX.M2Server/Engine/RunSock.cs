using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GXX.Core.Protocol;
using GXX.GatewayKit;

namespace GXX.M2Server.Engine;

/// <summary>
/// RunSock.pas TGateManager 1:1 核心转换：接受 RunGate/SelGate/LoginGate 链接，
/// GM_* 帧协议（与网关侧 GatewayProtocol 对应），把客户端消息投递给引擎。
/// </summary>
public class GateManager : IDisposable
{
    public class GateInfo
    {
        public TcpLink Link;
        public int GateIdx;
        public bool boConnected;
    }

    private readonly List<GateInfo> _gates = new();
    private readonly object _gateLock = new();
    private Socket? _listener;
    private Thread? _acceptThread;
    private volatile bool _running;

    public int Port { get; set; } = 5600;
    public bool Running => _running;
    public int GateCount { get { lock (_gateLock) return _gates.Count; } }

    public event Action<string, int>? OnLogMsg;
    public event Action<int, byte[]>? OnClientData;      // (SockId, 数据)
    public event Action<int>? OnClientDisconnect;
    public event Action? OnGateConnect;

    public bool Start()
    {
        if (_running) return true;
        try
        {
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(new IPEndPoint(IPAddress.Any, Port));
            _listener.Listen(16);
            _running = true;
            _acceptThread = new Thread(AcceptLoop) { IsBackground = true, Name = "M2-GateAccept" };
            _acceptThread.Start();
            SendLog($"M2 网关监听端口 {Port} 启动");
            return true;
        }
        catch (Exception ex)
        {
            SendLog("网关监听启动失败: " + ex.Message, 1);
            return false;
        }
    }

    public void Stop()
    {
        _running = false;
        try { _listener?.Close(); } catch { }
        lock (_gateLock)
        {
            foreach (var g in _gates) g.Link.Close();
            _gates.Clear();
        }
    }

    private void AcceptLoop()
    {
        while (_running)
        {
            try
            {
                var client = _listener!.Accept();
                var link = new TcpLink(client);
                var gate = new GateInfo { Link = link, GateIdx = 0, boConnected = true };
                link.OnReceive += (buf, off, len) =>
                {
                    link.Accumulate(buf, off, len);
                    ProcessGateData(gate, link);
                };
                link.OnDisconnected += () =>
                {
                    gate.boConnected = false;
                    lock (_gateLock) _gates.Remove(gate);
                    SendLog("一个网关断开连接");
                };
                lock (_gateLock)
                {
                    gate.GateIdx = _gates.Count + 1;
                    _gates.Add(gate);
                }
                SendLog($"网关 #{gate.GateIdx} 已连接");
                OnGateConnect?.Invoke();
            }
            catch { if (!_running) return; }
        }
    }

    private void ProcessGateData(GateInfo gate, TcpLink link)
    {
        while (link.AccumLength >= GatewayProtocol.SizeOfTSvrCmdPack)
        {
            var header = StructBytes.FromBytes<GatewayProtocol.TSvrCmdPack>(link.AccumBuffer, 0);
            if (header.Flag != GatewayProtocol.RUNGATECODE)
            {
                link.ConsumeAccum(link.AccumLength);
                return;
            }
            int dataLen = header.DataLen;
            if (dataLen < 0 || dataLen > 1024 * 1024)
            {
                link.ConsumeAccum(link.AccumLength);
                return;
            }
            if (link.AccumLength < GatewayProtocol.SizeOfTSvrCmdPack + dataLen)
                return;

            byte[]? data = null;
            if (dataLen > 0)
            {
                data = new byte[dataLen];
                Array.Copy(link.AccumBuffer, GatewayProtocol.SizeOfTSvrCmdPack, data, 0, dataLen);
            }
            link.ConsumeAccum(GatewayProtocol.SizeOfTSvrCmdPack + dataLen);

            switch (header.Cmd)
            {
                case GatewayProtocol.GM_OPEN:
                    SendLog($"网关会话打开 #{header.SockID}");
                    break;
                case GatewayProtocol.GM_CLOSE:
                    OnClientDisconnect?.Invoke((int)header.SockID);
                    break;
                case GatewayProtocol.GM_CHECKSERVER:
                    // 心跳回应
                    SendToGate(gate, GatewayProtocol.BuildServerPacket(0, GatewayProtocol.GM_CHECKSERVER, 0, null, 0));
                    break;
                case GatewayProtocol.GM_DATA:
                    if (data != null)
                        OnClientData?.Invoke((int)header.SockID, data);
                    break;
            }
        }
    }

    private void SendToGate(GateInfo gate, byte[] data)
    {
        if (gate.boConnected) gate.Link.Send(data);
    }

    /// <summary>向指定会话发送数据（供引擎回发客户端）。</summary>
    public bool SendToClient(int sockId, byte[] data)
    {
        GateInfo? found = null;
        lock (_gateLock)
        {
            foreach (var g in _gates)
            {
                if (g.boConnected) { found = g; break; }
            }
        }
        if (found == null) return false;
        byte[] frame = GatewayProtocol.BuildServerPacket((uint)sockId, GatewayProtocol.GM_DATA, 0, data, data.Length);
        found.Link.Send(frame);
        return true;
    }

    /// <summary>通知网关踢掉一个会话。</summary>
    public void KickClient(int sockId)
    {
        GateInfo? found = null;
        lock (_gateLock)
        {
            foreach (var g in _gates)
                if (g.boConnected) { found = g; break; }
        }
        if (found != null)
            found.Link.Send(GatewayProtocol.BuildServerPacket((uint)sockId, GatewayProtocol.GM_KICK, 0, null, 0));
    }

    private void SendLog(string msg, int level = 3) => OnLogMsg?.Invoke(msg, level);

    public void Dispose() => Stop();
}
