using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.GatewayKit;

/// <summary>
/// 网关公共协议常量与结构（对应三网关各自 Protocol/GateShare 单元的公共部分）。
/// </summary>
public static class GatewayProtocol
{
    // ---- 网关间链路常量（Grobal2）----
    public const uint RUNGATECODE = Grobal2Const.RUNGATECODE;      // $AA55AA55
    public const uint RUNGATECODEX = Grobal2Const.RUN_GATE_MSG_CODE;

    public const int MAX_GATE_SOCLIMIT = 1024;

    // GM_* 服务器↔网关 命令（Common.pas）
    public const int GM_OPEN = CommonConst.GM_OPEN;
    public const int GM_CLOSE = CommonConst.GM_CLOSE;
    public const int GM_CHECKSERVER = CommonConst.GM_CHECKSERVER;
    public const int GM_CHECKCLIENT = CommonConst.GM_CHECKCLIENT;
    public const int GM_DATA = CommonConst.GM_DATA;
    public const int GM_SERVERUSERINDEX = CommonConst.GM_SERVERUSERINDEX;
    public const int GM_RECEIVE_OK = CommonConst.GM_RECEIVE_OK;
    public const int GM_CLOSECONNECT = CommonConst.GM_CLOSECONNECT;
    public const int GM_DATA_CACHE = CommonConst.GM_DATA_CACHE;
    public const int GM_KICK = CommonConst.GM_KICK;
    public const int GM_RUN_GATE_VER = CommonConst.GM_RUN_GATE_VER;

    // SS_* 登录会话命令
    public const int SS_OPENSESSION = CommonConst.SS_OPENSESSION;
    public const int SS_CLOSESESSION = CommonConst.SS_CLOSESESSION;
    public const int SS_SOFTOUTSESSION = CommonConst.SS_SOFTOUTSESSION;
    public const int SS_SERVERINFO = CommonConst.SS_SERVERINFO;
    public const int SS_KEEPALIVE = CommonConst.SS_KEEPALIVE;
    public const int SS_KICKUSER = CommonConst.SS_KICKUSER;
    public const int SS_SERVERLOAD = CommonConst.SS_SERVERLOAD;

    /// <summary>TSvrCmdPack：网关↔服务器链路帧头（packed, 20 字节）。</summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
    public struct TSvrCmdPack
    {
        public uint Flag;      // RUNGATECODE
        public uint SockID;
        public ushort Seq;
        public ushort Cmd;     // GM_*
        public int GGSock;
        public int DataLen;
    }

    public const int SizeOfTSvrCmdPack = 20;

    /// <summary>构造服务器链路帧（Flag + TCmdPack 头 + 数据）。</summary>
    public static byte[] BuildServerPacket(uint sockId, ushort cmd, int ggSock, byte[] data, int dataLen)
    {
        var pack = new TSvrCmdPack
        {
            Flag = RUNGATECODE,
            SockID = sockId,
            Seq = 0,
            Cmd = cmd,
            GGSock = ggSock,
            DataLen = dataLen
        };
        byte[] head = StructBytes.BytesOf(pack);
        byte[] buf = new byte[SizeOfTSvrCmdPack + Math.Max(dataLen, 0)];
        Array.Copy(head, 0, buf, 0, head.Length);
        if (data != null && dataLen > 0)
            Array.Copy(data, 0, buf, head.Length, Math.Min(dataLen, data.Length));
        return buf;
    }
}

/// <summary>网关会话（对应 ClientSession/SHSocket 的公共语义：缓冲重组 + 节流 + 发送队列）。</summary>
public class GateSession
{
    private const int DATA_BUFSIZE = Grobal2Const.DATA_BUFSIZE; // 8192

    public int SocketId;                        // nSocketHandle / 唯一 ID
    public Socket? Socket;
    public string RemoteIP = "";
    public long RecogId;                        // 64位会话标识
    public bool IsVerified;                     // 已通过网关验证
    public uint LastRecvTick;
    public uint LastSendTick;
    public uint ConnectTick;

    // 收包缓冲（对应 SHSocket 的 recv buffer 重组）
    public byte[] Buffer = new byte[DATA_BUFSIZE * 2];
    public int BufferLen;

    // 节流
    public int PacketCount;
    public uint PacketSpeedTick;
    public bool SpeedLimited;

    // 发送队列（对应 SendQueue.pas）
    private readonly ConcurrentQueue<byte[]> _sendQueue = new();
    private int _sending;

    public void EnqueueSend(byte[] packet) => _sendQueue.Enqueue(packet);

    public bool TryDequeueSend(out byte[] packet) => _sendQueue.TryDequeue(out packet!);

    public int PendingSendCount => _sendQueue.Count;

    /// <summary>重组收到的字节流 → 完整帧列表（分隔规则由网关各自解释，此处输出原始帧）。</summary>
    public void AppendBuffer(byte[] data, int offset, int count)
    {
        if (BufferLen + count > Buffer.Length)
        {
            // 溢出保护（对应原 DATA_BUFSIZE 超限断开）
            BufferLen = 0;
            return;
        }
        Array.Copy(data, offset, Buffer, BufferLen, count);
        BufferLen += count;
    }

    public void ResetBuffer() => BufferLen = 0;
}

/// <summary>
/// IocpManager 1:1 托管实现：.NET SocketAsyncEventArgs（底层即 Windows IOCP）。
/// 管理 Accept 池 + 读写池（DATA_BUFSIZE），对应原 IOCPManager.pas + AcceptExWorkedThread.pas。
/// </summary>
public class IocpManager : IDisposable
{
    private Socket? _listenSocket;
    private readonly ConcurrentDictionary<int, GateSession> _sessions = new();
    private int _nextSocketId = 1;
    private SocketAsyncEventArgs? _acceptArgs;
    private readonly int _backlog;
    private volatile bool _running;

    public event Action<GateSession>? OnAccept;
    public event Action<GateSession, byte[], int, int>? OnReceive;
    public event Action<GateSession>? OnDisconnect;

    public IocpManager(Socket? listenSocket = null, int backlog = 100)
    {
        _listenSocket = listenSocket;
        _backlog = backlog;
    }

    public int SessionCount => _sessions.Count;

    public GateSession? GetSession(int id) => _sessions.TryGetValue(id, out var s) ? s : null;

    public void Start()
    {
        if (_listenSocket == null) return;
        _running = true;
        try { _listenSocket.Listen(_backlog); } catch { }
        for (int i = 0; i < 4; i++)
            StartAccept();
    }

    /// <summary>绑定外部创建并已 Bind 的监听套接字后启动。</summary>
    public void StartListen(Socket listenSocket)
    {
        _listenSocket = listenSocket;
        _running = true;
        listenSocket.Listen(_backlog);
        for (int i = 0; i < 4; i++)
            StartAccept();
    }

    public void Stop()
    {
        _running = false;
        try { _listenSocket?.Close(); } catch { }
        foreach (var kv in _sessions)
            CloseSession(kv.Value);
        _sessions.Clear();
    }

    private void StartAccept()
    {
        if (!_running || _listenSocket == null) return;
        var args = new SocketAsyncEventArgs();
        args.Completed += (s, e) => ProcessAccept(e);
        bool pending = false;
        try { pending = _listenSocket.AcceptAsync(args); } catch { }
        if (!pending) ProcessAccept(args);
    }

    private void ProcessAccept(SocketAsyncEventArgs e)
    {
        if (_running && e.SocketError == SocketError.Success && e.AcceptSocket != null)
        {
            var session = new GateSession
            {
                SocketId = Interlocked.Increment(ref _nextSocketId),
                Socket = e.AcceptSocket,
                ConnectTick = DelphiRTL.GetTickCount(),
                LastRecvTick = DelphiRTL.GetTickCount()
            };
            try
            {
                var ep = (IPEndPoint?)e.AcceptSocket.RemoteEndPoint;
                session.RemoteIP = ep?.Address.ToString() ?? "";
            }
            catch { }
            _sessions[session.SocketId] = session;
            OnAccept?.Invoke(session);

            var recvArgs = new SocketAsyncEventArgs();
            recvArgs.SetBuffer(new byte[Grobal2Const.DATA_BUFSIZE], 0, Grobal2Const.DATA_BUFSIZE);
            recvArgs.UserToken = session;
            recvArgs.Completed += (s, a) => ProcessReceive(a);
            bool pending = false;
            try { pending = e.AcceptSocket.ReceiveAsync(recvArgs); } catch { CloseSession(session); }
            if (!pending) ProcessReceive(recvArgs);
        }
        else
        {
            e.AcceptSocket?.Dispose();
        }
        StartAccept(); // 继续接受下一连接（AcceptEx 池语义）
    }

    private void ProcessReceive(SocketAsyncEventArgs e)
    {
        var session = (GateSession?)e.UserToken;
        if (session == null || !_running) return;
        if (e.SocketError != SocketError.Success || e.BytesTransferred == 0)
        {
            CloseSession(session);
            return;
        }
        session.LastRecvTick = DelphiRTL.GetTickCount();
        OnReceive?.Invoke(session, e.Buffer!, e.Offset, e.BytesTransferred);

        bool pending = false;
        try { pending = session.Socket!.ReceiveAsync(e); }
        catch { CloseSession(session); return; }
        if (!pending) ProcessReceive(e);
    }

    /// <summary>发送（SendQueue 语义：直接异步发送；排队由 GateSession 完成）。</summary>
    public void Send(GateSession session, byte[] data)
    {
        if (session.Socket == null || !_running) return;
        try
        {
            var args = new SocketAsyncEventArgs();
            args.SetBuffer(data, 0, data.Length);
            args.Completed += (s, a) =>
            {
                if (a.SocketError != SocketError.Success)
                    CloseSession(session);
                ((SocketAsyncEventArgs)a).Dispose();
            };
            if (!session.Socket.SendAsync(args))
            {
                if (args.SocketError != SocketError.Success) CloseSession(session);
                args.Dispose();
            }
        }
        catch
        {
            CloseSession(session);
        }
    }

    public void CloseSession(GateSession session)
    {
        if (_sessions.TryRemove(session.SocketId, out _))
        {
            try { session.Socket?.Shutdown(SocketShutdown.Both); } catch { }
            try { session.Socket?.Close(); } catch { }
            session.Socket = null;
            OnDisconnect?.Invoke(session);
        }
    }

    public void Dispose() => Stop();
}
