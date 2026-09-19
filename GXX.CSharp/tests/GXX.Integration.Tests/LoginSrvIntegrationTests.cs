using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.LoginGate;
using GXX.LoginSrv;
using Xunit;

namespace GXX.Integration.Tests;

/// <summary>真实链路：客户端 → LoginGate → LoginSrv（注册+登录）。</summary>
[Collection("Sequential")]
public class LoginSrvIntegrationTests
{
    private static int FreePort()
    {
        var l = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        l.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        int port = ((IPEndPoint)l.LocalEndPoint!).Port;
        l.Close();
        return port;
    }

    [Fact]
    public void Client_ThroughGate_ToLoginSrv_RegisterAndLogin()
    {
        int gatePort = FreePort();
        int srvPort = FreePort();

        // ---- 启动 LoginSrv ----
        string dbFile = System.IO.Path.Combine(AppContext.BaseDirectory, "test_account.db");
        if (System.IO.File.Exists(dbFile)) System.IO.File.Delete(dbFile);
        using var loginSrv = new LoginSrvService(dbFile);
        loginSrv.GatePort = srvPort;
        Assert.True(loginSrv.StartService());

        // ---- 启动 LoginGate ----
        using var gate = new LoginGateService();
        gate.GateAddr = "127.0.0.1";
        gate.GatePort = gatePort;
        gate.ServerAddr = "127.0.0.1";
        gate.ServerPort = srvPort;
        Assert.True(gate.StartService());

        // ---- 客户端 ----
        using var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        client.Connect(IPAddress.Loopback, gatePort);
        client.ReceiveTimeout = 30000; // 并行测试负载下留足余量
        var buf = new byte[4096];

        // 1. 注册账号 CM_ADDNEWUSER(2002)：account/password
        client.Send(BuildCm(2002, "testacct\ttestpass"));
        int n = ReceiveBlock(client, buf);
        TDefaultMessage r1 = DecodeFirst(buf, n);
        Assert.Equal(504 /* SM_NEWID_SUCCESS */, r1.Ident);

        // 2. 重复注册 → 失败
        client.Send(BuildCm(2002, "testacct\ttestpass"));
        n = ReceiveBlock(client, buf);
        r1 = DecodeFirst(buf, n);
        Assert.Equal(505 /* SM_NEWID_FAIL */, r1.Ident);

        // 3. 登录（密码正确）CM_IDPASSWORD(2001)
        client.Send(BuildCm(2001, "testacct\ttestpass"));
        n = ReceiveBlock(client, buf);
        r1 = DecodeFirst(buf, n);
        Assert.Equal(529 /* SM_PASSOK_SELECTSERVER */, r1.Ident);

        // 4. 登录（密码错误）
        client.Send(BuildCm(2001, "testacct\twrongpass"));
        n = ReceiveBlock(client, buf);
        r1 = DecodeFirst(buf, n);
        Assert.Equal(503 /* SM_PASSWD_FAIL */, r1.Ident);

        // 5. 修改密码
        client.Send(BuildCm(2003, "testacct\ttestpass\tnewpass"));
        n = ReceiveBlock(client, buf);
        r1 = DecodeFirst(buf, n);
        Assert.Equal(506 /* SM_CHGPASSWD_SUCCESS */, r1.Ident);

        // 6. 新密码登录成功
        client.Send(BuildCm(2001, "testacct\tnewpass"));
        n = ReceiveBlock(client, buf);
        r1 = DecodeFirst(buf, n);
        Assert.Equal(529, r1.Ident);
    }

    /// <summary>带重试的接收：SocketException/超时时重发触发（幂等命令场景由外层保证）。</summary>
    private static int ReceiveBlock(Socket client, byte[] buf)
    {
        var deadline = DateTime.UtcNow.AddSeconds(30);
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                return client.Receive(buf);
            }
            catch (SocketException)
            {
                // 超时，继续等待
            }
        }
        throw new TimeoutException("等待网关响应超时");
    }

    private static string PathCwd => AppContext.BaseDirectory;

    private static byte[] BuildCm(ushort ident, string bodyText)
    {
        var msg = TDefaultMessage.Make(ident, 0, 0, 0, 0);
        byte[] head = EDcode.EncodeMessage(msg);
        byte[] body = EDcode.EncodeString(bodyText);
        byte[] buf = new byte[head.Length + body.Length];
        Array.Copy(head, buf, head.Length);
        Array.Copy(body, 0, buf, head.Length, body.Length);
        return buf;
    }

    private static TDefaultMessage DecodeFirst(byte[] buf, int len)
        => EDcode.DecodeMessage(buf.AsSpan(0, Math.Min(len, 22)).ToArray());
}
