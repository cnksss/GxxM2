using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.GatewayKit;
using GXX.LoginGate;
using Xunit;

namespace GXX.Integration.Tests;

/// <summary>LoginGate ↔ 模拟 LoginSrv ↔ 客户端 全链路集成测试。</summary>
[Collection("Sequential")]
public class LoginGateIntegrationTests
{
    [Fact]
    public void LoginGate_ForwardsClientToServer_AndBack()
        // 端口 TOCTOU 竞态 / 等待超时属于瞬时失败，整场景重试；断言失败不重试（见 IntegrationRetry 注释）
        => IntegrationRetry.Run(RunOnce);

    private static void RunOnce()
    {
        int gatePort = IntegrationRetry.FreePort();
        int srvPort = IntegrationRetry.FreePort();

        // ---- 模拟 LoginSrv ----
        var receivedFrames = new ConcurrentQueue<(uint SockId, ushort Cmd, byte[] Data)>();
        using var gotFrame = new AutoResetEvent(false);
        GateSession? srvClient = null;
        var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(new IPEndPoint(IPAddress.Loopback, srvPort));
        listener.Listen(5);
        Socket? srvSock = null;
        var acceptEvt = new ManualResetEventSlim(false);
        var acceptThread = new Thread(() =>
        {
            srvSock = listener.Accept();
            var head = new byte[64];
            int n = srvSock.Receive(head);
            // GM_OPEN 帧
            var pack = StructBytes.FromBytes<GatewayProtocol.TSvrCmdPack>(head, 0);
            receivedFrames.Enqueue((pack.SockID, pack.Cmd, Array.Empty<byte>()));
            gotFrame.Set();
            acceptEvt.Set();

            // 持续接收
            var buf = new byte[4096];
            while (true)
            {
                int n2;
                try { n2 = srvSock.Receive(buf); } catch { break; }
                if (n2 == 0) break;
                var p = StructBytes.FromBytes<GatewayProtocol.TSvrCmdPack>(buf, 0);
                byte[] data = new byte[Math.Max(p.DataLen, 0)];
                if (p.DataLen > 0) Array.Copy(buf, 20, data, 0, Math.Min(p.DataLen, n2 - 20));
                receivedFrames.Enqueue((p.SockID, p.Cmd, data));
                gotFrame.Set();

                // 回发：GM_DATA → 同一 SockID
                byte[] reply = GatewayProtocol.BuildServerPacket(p.SockID, GatewayProtocol.GM_DATA, 0, data, data.Length);
                try { srvSock.Send(reply); } catch { break; }
            }
        });
        acceptThread.IsBackground = true;
        acceptThread.Start();

        // ---- 启动网关 ----
        using var gate = new LoginGateService();
        gate.GateAddr = "127.0.0.1";
        gate.GatePort = gatePort;
        gate.ServerAddr = "127.0.0.1";
        gate.ServerPort = srvPort;
        IntegrationRetry.RequireStarted(gate.StartService(), "LoginGateService");

        // ---- 客户端 ----
        using var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        client.Connect(IPAddress.Loopback, gatePort);

        // 等待 GM_OPEN（此前可能先收到 GM_CHECKSERVER 心跳帧）
        var frame = IntegrationRetry.RequireFrame(receivedFrames, gotFrame, GatewayProtocol.GM_OPEN, 5, "GM_OPEN");
        Assert.Equal(GatewayProtocol.GM_OPEN, frame.Cmd);

        // 客户端发送 CM_IDPASSWORD（6Bit 编码后的 TDefaultMessage 帧）
        TDefaultMessage msg = default;
        msg.Ident = 2001; // CM_IDPASSWORD
        msg.Recog = 42;
        byte[] payload = EDcode.EncodeMessage(msg);
        client.Send(payload);

        frame = IntegrationRetry.RequireFrame(receivedFrames, gotFrame, GatewayProtocol.GM_DATA, 5, "GM_DATA");
        Assert.Equal(GatewayProtocol.GM_DATA, frame.Cmd);
        TDefaultMessage back = EDcode.DecodeMessage(frame.Data);
        Assert.Equal(2001, back.Ident);
        Assert.Equal(42L, back.Recog);

        // 服务器回发数据 → 应到达客户端
        var clientBuf = new byte[256];
        client.ReceiveTimeout = 5000;
        int n3 = client.Receive(clientBuf);
        Assert.Equal(payload.Length, n3);
        TDefaultMessage echoed = EDcode.DecodeMessage(clientBuf.AsSpan(0, n3).ToArray());
        Assert.Equal(2001, echoed.Ident);

        // 客户端断开 → GM_CLOSE
        client.Close();
        frame = IntegrationRetry.RequireFrame(receivedFrames, gotFrame, GatewayProtocol.GM_CLOSE, 5, "GM_CLOSE");
        Assert.Equal(GatewayProtocol.GM_CLOSE, frame.Cmd);

        listener.Close();
    }
}
