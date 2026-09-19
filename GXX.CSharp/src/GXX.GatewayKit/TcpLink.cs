using System;
using System.Net.Sockets;
using System.Threading;
using GXX.Core.Protocol;
using GXX.Core.Rtl;

namespace GXX.GatewayKit;

/// <summary>
/// 主动发起的 TCP 链路（对应 Delphi TClientSocket / ServerClient.pas / IDSocCli.pas 的阻塞语义）。
/// 断线自动重连、收发缓冲。
/// </summary>
public class TcpLink : IDisposable
{
    private Socket? _socket;
    private SocketAsyncEventArgs? _recvArgs;
    private readonly byte[] _recvBuffer = new byte[Grobal2Const.DATA_BUFSIZE];
    private readonly byte[] _accum = new byte[Grobal2Const.DATA_BUFSIZE * 4];
    private int _accumLen;
    private readonly object _sendLock = new();
    private volatile bool _connected;

    public string Host { get; }
    public int Port { get; }
    public bool Connected => _connected;

    public event Action? OnConnected;
    public event Action<byte[], int, int>? OnReceive;
    public event Action? OnDisconnected;

    public TcpLink(string host, int port)
    {
        Host = host;
        Port = port;
    }

    /// <summary>以已连接的 Socket 构造（LoginSrv 接受网关连接的场景）。</summary>
    public TcpLink(Socket existingSocket)
    {
        Host = "";
        Port = 0;
        _socket = existingSocket;
        _connected = true;
        _accumLen = 0;
        _recvArgs = new SocketAsyncEventArgs();
        _recvArgs.SetBuffer(_recvBuffer, 0, _recvBuffer.Length);
        _recvArgs.Completed += (s, e) => ProcessReceive(e);
        bool pending = _socket.ReceiveAsync(_recvArgs);
        if (!pending) ProcessReceive(_recvArgs);
    }

    public void StartReceiveLoop()
    {
        if (_socket == null || !_connected || _recvArgs != null && _attached) return;
        _attached = true;
        _recvArgs = new SocketAsyncEventArgs();
        _recvArgs.SetBuffer(_recvBuffer, 0, _recvBuffer.Length);
        _recvArgs.Completed += (s, e) => ProcessReceive(e);
        bool pending = _socket.ReceiveAsync(_recvArgs);
        if (!pending) ProcessReceive(_recvArgs);
    }

    private bool _attached;

    public bool Connect()
    {
        try
        {
            Close();
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.NoDelay = true;
            _socket.Connect(Host, Port);
            _connected = true;
            _accumLen = 0;

            _recvArgs = new SocketAsyncEventArgs();
            _recvArgs.SetBuffer(_recvBuffer, 0, _recvBuffer.Length);
            _recvArgs.Completed += (s, e) => ProcessReceive(e);
            bool pending = _socket.ReceiveAsync(_recvArgs);
            if (!pending) ProcessReceive(_recvArgs);

            OnConnected?.Invoke();
            return true;
        }
        catch
        {
            Close();
            return false;
        }
    }

    private void ProcessReceive(SocketAsyncEventArgs e)
    {
        if (!_connected) return;
        if (e.SocketError != SocketError.Success || e.BytesTransferred == 0)
        {
            _connected = false;
            OnDisconnected?.Invoke();
            return;
        }
        OnReceive?.Invoke(e.Buffer!, e.Offset, e.BytesTransferred);
        try
        {
            bool pending = _socket!.ReceiveAsync(e);
            if (!pending) ProcessReceive(e);
        }
        catch
        {
            _connected = false;
            OnDisconnected?.Invoke();
        }
    }

    public void Send(byte[] data)
    {
        if (!_connected || _socket == null) return;
        lock (_sendLock)
        {
            try
            {
                _socket.Send(data);
            }
            catch
            {
                _connected = false;
                OnDisconnected?.Invoke();
            }
        }
    }

    /// <summary>从累计缓冲中提取完整帧：返回提取的字节数（帧长由判定函数给出）。</summary>
    public void Accumulate(byte[] data, int offset, int len)
    {
        if (_accumLen + len > _accum.Length)
        {
            _accumLen = 0; // 溢出保护
            return;
        }
        Array.Copy(data, offset, _accum, _accumLen, len);
        _accumLen += len;
    }

    public byte[] AccumBuffer => _accum;
    public int AccumLength => _accumLen;

    public void ConsumeAccum(int n)
    {
        if (n >= _accumLen) { _accumLen = 0; return; }
        Array.Copy(_accum, n, _accum, 0, _accumLen - n);
        _accumLen -= n;
    }

    public void Close()
    {
        _connected = false;
        try { _socket?.Shutdown(SocketShutdown.Both); } catch { }
        try { _socket?.Close(); } catch { }
        _socket = null;
        _recvArgs?.Dispose();
        _recvArgs = null;
        _accumLen = 0;
    }

    public void Dispose() => Close();
}
