using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GXX.Core;
using GXX.GatewayKit;
using Xunit;

namespace GXX.GatewayKit.Tests;

/// <summary>测试用最小网关：客户端回声 + 记录服务器链路。</summary>
public class TestEchoGate : GateService
{
    public GateSession? LastAccepted;
    public byte[] LastServerData = Array.Empty<byte>();

    public TestEchoGate() : base(".\\test_gate.ini")
    {
        GateAddr = "127.0.0.1";
        GatePort = FreePort();
        ServerAddr = "127.0.0.1";
        ServerPort = FreePort();
    }

    protected override int GateDefaultPort => 7000;
    protected override int ServerDefaultPort => 7100;
    public override string ServiceName => "TestGate";

    protected override void OnClientAccept(GateSession session)
    {
        base.OnClientAccept(session);
        LastAccepted = session;
        // 网关回声：把客户端发来的数据原样发回
        ClientIocp.Send(session, EncodingInit.GBK.GetBytes("WELCOME"));
    }

    protected override void OnClientReceive(GateSession session, byte[] buf, int offset, int len)
    {
        ClientIocp.Send(session, buf.AsSpan(offset, len).ToArray());
    }

    protected override void OnServerData(TcpLink link)
    {
        // 简单协议：帧长 = 4 字节头(小端) + 载荷
        while (link.AccumLength >= 4)
        {
            int frameLen = BitConverter.ToInt32(link.AccumBuffer, 0);
            if (frameLen <= 0 || frameLen > 64 * 1024) { link.ConsumeAccum(link.AccumLength); return; }
            if (link.AccumLength < 4 + frameLen) return;
            byte[] payload = new byte[frameLen];
            Array.Copy(link.AccumBuffer, 4, payload, 0, frameLen);
            LastServerData = payload;
            link.ConsumeAccum(4 + frameLen);
        }
    }

    internal static int FreePort()
    {
        var l = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        l.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        int port = ((IPEndPoint)l.LocalEndPoint!).Port;
        l.Close();
        return port;
    }
}

public class GatewayKitTests
{
    [Fact]
    public void GateService_ClientEcho_RoundTrip()
    {
        using var gate = new TestEchoGate();
        Assert.True(gate.StartService());

        // 客户端连接
        using var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        client.Connect(IPAddress.Loopback, gate.GatePort);
        var buf = new byte[256];
        int n = client.Receive(buf);
        Assert.Equal("WELCOME", EncodingInit.GBK.GetString(buf, 0, n));

        // 回声
        byte[] payload = EncodingInit.GBK.GetBytes("ECHO_ME_123");
        client.Send(payload);
        int n2 = client.Receive(buf);
        Assert.Equal("ECHO_ME_123", EncodingInit.GBK.GetString(buf, 0, n2));

        Assert.Equal(1, gate.OnlineCount);
        gate.StopService();
        Assert.False(gate.ServiceStarted);
    }

    [Fact]
    public void TcpLink_ConnectSendReceive()
    {
        int port = TestEchoGate.FreePort();
        byte[]? received = null;
        using var ready = new ManualResetEventSlim(false);

        // 模拟上游服务器
        var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(new IPEndPoint(IPAddress.Loopback, port));
        listener.Listen(5);
        var acceptThread = new Thread(() =>
        {
            var s = listener.Accept();
            var b = new byte[128];
            int n = s.Receive(b);
            received = b.AsSpan(0, n).ToArray();
            ready.Set();
            s.Close();
        });
        acceptThread.Start();

        using var link = new TcpLink("127.0.0.1", port);
        Assert.True(link.Connect());
        link.Send(EncodingInit.GBK.GetBytes("PING"));

        Assert.True(ready.Wait(5000), "服务器未收到数据");
        Assert.Equal("PING", EncodingInit.GBK.GetString(received!));
        listener.Close();
    }

    [Fact]
    public void ServerPacket_FrameLayout()
    {
        // TSvrCmdPack = 4+4+2+2+4+4 = 20
        Assert.Equal(20, GatewayProtocol.SizeOfTSvrCmdPack);
        byte[] frame = GatewayProtocol.BuildServerPacket(77, GatewayProtocol.GM_DATA, 3, new byte[] { 1, 2, 3 }, 3);
        Assert.Equal(23, frame.Length);
        Assert.Equal(0x55, frame[0]);   // $AA55AA55 小端首字节
        Assert.Equal(0xAA, frame[1]);
        // Flag(4)+SockID(4)+Seq(2)=10 → Cmd 在偏移 10，GGSock 在偏移 12
        Assert.Equal(GatewayProtocol.GM_DATA, BitConverter.ToUInt16(frame, 10));
        Assert.Equal(3, BitConverter.ToInt32(frame, 12));
    }
}
