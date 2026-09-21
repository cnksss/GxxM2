using System;
using System.Collections.Generic;
using System.Text;
using GXX.Core.Rtl;
using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>
/// 接缝替身：`TcpLink` 的传输面。**不建真 socket**（本工程规程：真实 socket 的端到端用例不引入）。
/// 事件由测试用 <see cref="Feed"/> / <see cref="RaiseDisconnect"/> 手动驱动。
/// </summary>
public sealed class FakeIDSocTransport : IIDSocTransport
{
    public bool IsConnected;
    public string HostName = "10.1.1.1";
    public int PortNo = 5600;
    public bool ConnectResult = true;
    public int ConnectCount;
    public int CloseCount;
    public readonly List<byte[]> Sent = new();

    public bool Connected => IsConnected;
    public string Host => HostName;
    public int Port => PortNo;

    public void SetTarget(string host, int port)
    {
        HostName = host;
        PortNo = port;
    }

    public bool Connect()
    {
        ConnectCount++;
        if (!ConnectResult) return false;
        IsConnected = true;
        OnConnected?.Invoke();
        return true;
    }

    public void Send(byte[] data)
    {
        if (!IsConnected) return;                       // 与 TcpLink.Send 一致：未连通 no-op
        Sent.Add(data);
    }

    public void Close()
    {
        CloseCount++;
        IsConnected = false;
    }

    public event Action<byte[], int, int>? OnReceive;
    public event Action? OnConnected;
    public event Action? OnDisconnected;

    /// <summary>投递一段文本（latin-1 字节串 → 原始字节，模拟网线到达）。</summary>
    public void Feed(string byteStr)
    {
        byte[] b = Encoding.Latin1.GetBytes(byteStr);
        OnReceive?.Invoke(b, 0, b.Length);
    }

    public void RaiseDisconnect()
    {
        IsConnected = false;
        OnDisconnected?.Invoke();
    }
}

/// <summary>
/// `IDSocCli.Adapter.cs` 的测试：`TcpLink` 适配器 + `IDSocCliHost` 宿主引导。
/// ★ 全部走**替身传输**，不碰真 socket、不起真定时器（定时器只验证"开关接线"与"DFM 常量"）。
/// </summary>
public class IDSocCliAdapterTests : TempDirTest
{
    private readonly FakeIDSocTransport _tx = new();
    private readonly List<string> Logs = new();
    private readonly List<bool> KeepAliveSwitches = new();
    private readonly List<bool> Timer1Switches = new();
    private readonly List<string> ModuleAdded = new();

    public IDSocCliAdapterTests()
    {
        // 共享静态接缝：本车道新增的必须自己复位（规程：不能只依赖 TestReset.All）
        IDSocCliSeam.Reset();
        SelectClientModuleSeam.Reset();
        RoleDbSeam.MainOutMessage = s => Logs.Add(s);
        IDSocCliSeam.Timer1Enabled = v => Timer1Switches.Add(v);
        IDSocCliSeam.KeepAliveTimerEnabled = v => KeepAliveSwitches.Add(v);
        SelectClientModuleSeam.AddModule = (m, n, a, b) => { ModuleAdded.Add(a); return new IntPtr(1); };
        DelphiTick.GetTickCount = () => 0;
    }

    private IDSocClientSocketAdapter NewAdapter(out TFrmIDSoc frm, bool syncDispatcher = true)
    {
        frm = new TFrmIDSoc();
        var a = new IDSocClientSocketAdapter(frm, _tx);
        if (syncDispatcher) a.ConnectDispatcher = act => act();     // 测试用同步派发（生产是线程池）
        a.LocalPortProvider = () => 5601;                           // 默认会留痕，测试里显式注入
        // ★ 必须登记到接缝：`TFrmIDSoc.IDSocketRead/Connect/Disconnect` 是**从接缝取 socket** 的
        //   （原文读的是全局 `IDSocket`）—— 不登记的话回灌会撞 `RequireIDSocket` 的"未接线即抛"。
        IDSocCliSeam.IDSocket = a;
        return a;
    }

    // =====================================================================================
    // 端点映射（IIDSocSocketEndPoint）
    // =====================================================================================

    [Fact]
    public void 端点_Connected映射到底层()
    {
        var a = NewAdapter(out var frm);
        using (frm)
        {
            Assert.False(a.Socket.Connected);
            _tx.IsConnected = true;
            Assert.True(a.Socket.Connected);
        }
    }

    [Fact]
    public void 端点_远端地址与端口取底层()
    {
        var a = NewAdapter(out var frm);
        using (frm)
        {
            Assert.Equal("10.1.1.1", a.Socket.RemoteAddress);
            Assert.Equal(5600, a.Socket.RemotePort);
            Assert.Equal(5601, a.Socket.LocalPort);
        }
    }

    [Fact]
    public void 端点_LocalPort未注入时返回0并留痕()
    {
        // §25.2：不静默。TcpLink 不暴露本端端点 ⇒ 默认值必须留一条日志。
        var a = NewAdapter(out var frm);
        using (frm)
        {
            var a2 = new IDSocClientSocketAdapter(frm, _tx);        // 不注入 LocalPortProvider
            Assert.Equal(0, a2.Socket.LocalPort);
            Assert.Contains(Logs, s => s.Contains("LocalPort 未注入"));
            Assert.NotEqual(0, a.Socket.LocalPort);                 // 注入过的那个仍是 5601
        }
    }

    [Fact]
    public void 端点_Socket恒定返回同一对象()
    {
        var a = NewAdapter(out var frm);
        using (frm) Assert.Same(a.Socket, a.Socket);
    }

    [Fact]
    public void 端点_SendText按latin1字节串发出()
    {
        var a = NewAdapter(out var frm);
        using (frm)
        {
            _tx.IsConnected = true;
            a.Socket.SendText("(1030/GeeM2/99/7)");
            Assert.Single(_tx.Sent);
            Assert.Equal("(1030/GeeM2/99/7)", Encoding.Latin1.GetString(_tx.Sent[0]));
        }
    }

    [Fact]
    public void 端点_SendText保留双字节原样_不做编码转换()
    {
        // D-p7-9：字节串 → 字节，必须原样。用 GBK 的 "中文" 字节验。
        var a = NewAdapter(out var frm);
        using (frm)
        {
            _tx.IsConnected = true;
            byte[] gbk = Encoding.GetEncoding(936).GetBytes("中文");
            a.Socket.SendText(Encoding.Latin1.GetString(gbk));
            Assert.Equal(gbk, _tx.Sent[0]);                     // 逐字节相同
        }
    }

    [Fact]
    public void 端点_Close转发到底层()
    {
        var a = NewAdapter(out var frm);
        using (frm)
        {
            a.Socket.Close();
            Assert.Equal(1, _tx.CloseCount);
        }
    }

    // =====================================================================================
    // ReceiveText 缓冲（取走并清空）
    // =====================================================================================

    [Fact]
    public void ReceiveText_被IDSocketRead取走后为空()
    {
        // VCL 语义：`ReceiveText` 取走即清空；而每次到达都**立刻**回灌 `IDSocketRead`
        // ⇒ 从外部观察到的是"读完之后必为空"，数据已经进了 `TFrmIDSoc.m_sSockMsg`。
        var a = NewAdapter(out var frm);
        using (frm)
        {
            _tx.Feed("(1000/acct/42/0/x/1.1.1.1)");
            Assert.Equal("", a.Socket.ReceiveText);
            Assert.Equal(1, frm.GlobaSessionCount);             // 数据确实入了 frm 并成帧
        }
    }

    [Fact]
    public void ReceiveText_未成帧的片段留在frm缓冲里_补齐后才成帧()
    {
        var a = NewAdapter(out var frm);
        using (frm)
        {
            _tx.Feed("(1000/acct");
            Assert.Equal(0, frm.GlobaSessionCount);              // 缺 ')' ⇒ 不解包
            _tx.Feed("/42/0/x/1.1.1.1)");
            Assert.Equal(1, frm.GlobaSessionCount);              // 拼接后成帧
            Assert.Equal("", a.Socket.ReceiveText);              // 已被取走
        }
    }

    [Fact]
    public void Address与Port_设置会真的改掉连接目标_不是死属性()
    {
        // ★ 回归锁定：`TcpLink` 的 `Host`/`Port` 是**只读**的。若适配器把目标固定在构造时，
        //   `IDSocket.Address`/`Port` 就成了"写进去没人读"的死属性 —— 而原文 :94-95/:419-420
        //   恰恰靠这两个属性决定"连哪个 ID 服务器"。
        var a = NewAdapter(out var frm);
        using (frm)
        {
            a.Address = "10.9.9.9";
            a.Port = 5900;
            a.Active = true;

            Assert.Equal("10.9.9.9", _tx.Host);                 // 传输层真的被改了
            Assert.Equal(5900, _tx.Port);
            Assert.Equal("10.9.9.9", a.Socket.RemoteAddress);   // 端点读到的也是新目标
            Assert.Equal(5900, a.Socket.RemotePort);
        }
    }

    [Fact]
    public void ReceiveText_多段到达时累加()
    {
        var a = NewAdapter(out var frm);
        using (frm)
        {
            // 注意：每次 Feed 都会回灌 IDSocketRead ⇒ 有 ')' 时会被消费掉；
            // 这里喂不含 ')' 的片段，验证"累加"本身。
            _tx.Feed("(1000/acct");
            _tx.Feed("/42/0/x/1.1.1.1)");
            Assert.Equal(1, frm.GlobaSessionCount);             // 拼接后成帧
            Assert.Equal("", a.Socket.ReceiveText);             // 已被 IDSocketRead 取走
        }
    }

    // =====================================================================================
    // 事件回灌（原文由 VCL 组件触发）
    // =====================================================================================

    [Fact]
    public void 收到数据_回灌IDSocketRead_会话表被真实填充()
    {
        // ★★ 这就是"接通后会话校验才真正生效"的最小证明：
        //    LoginSrv 推一帧 (1000/账号/会话号/0/x/IP) ⇒ 经适配器 ⇒ TFrmIDSoc 建会话 ⇒ CheckSession 为真。
        var a = NewAdapter(out var frm);
        using (frm)
        {
            _tx.Feed("(1000/acct/42/0/x/1.1.1.1)");

            Assert.Equal(1, frm.GlobaSessionCount);
            Assert.True(frm.CheckSession("acct", "1.1.1.1", 42));
        }
    }

    [Fact]
    public void 收到数据_粘包两帧都建()
    {
        var a = NewAdapter(out var frm);
        using (frm)
        {
            _tx.Feed("(1000/a1/1/0/x/1.1.1.1)(1000/a2/2/0/x/1.1.1.2)");
            Assert.Equal(2, frm.GlobaSessionCount);
        }
    }

    [Fact]
    public void 收到取消会话帧_会话被删掉()
    {
        var a = NewAdapter(out var frm);
        using (frm)
        {
            _tx.Feed("(1000/acct/42/0/x/1.1.1.1)");
            _tx.Feed("(1010/acct/42)");
            Assert.Equal(0, frm.GlobaSessionCount);
        }
    }

    [Fact]
    public void 连接事件_回灌IDSocketConnect_开保活定时器()
    {
        var a = NewAdapter(out var frm);
        using (frm)
        {
            _tx.Connect();
            Assert.Equal(new[] { true }, KeepAliveSwitches);    // :448 KeepAliveTimerEnabled(True)
            Assert.Equal(new IntPtr(1), frm.m_Module);          // :453 m_Module := AddModule(...)
        }
    }

    [Fact]
    public void 连接事件_登记模块表_地址串用远端与本端端口()
    {
        var a = NewAdapter(out var frm);
        using (frm)
        {
            _tx.Connect();
            Assert.Single(ModuleAdded);
            Assert.Equal("10.1.1.1:5601 → 10.1.1.1:5600", ModuleAdded[0]);   // 原文 :451 的 Format
            Assert.Equal(new IntPtr(1), frm.m_Module);
        }
    }

    [Fact]
    public void 断开事件_回灌IDSocketDisconnect_关保活定时器()
    {
        var a = NewAdapter(out var frm);
        using (frm)
        {
            _tx.Connect();
            KeepAliveSwitches.Clear();
            _tx.RaiseDisconnect();
            Assert.Equal(new[] { false }, KeepAliveSwitches);
            Assert.Equal(IntPtr.Zero, frm.m_Module);
        }
    }

    [Fact]
    public void 回灌抛异常_被宿主边界吞掉并留痕_不让接收线程崩()
    {
        // D-p7-11 同款宿主边界：模块表接缝未接线时 ProcessGetOnlineCount 会抛，
        // 绝不能让异常逃出接收线程。
        var a = NewAdapter(out var frm);
        using (frm)
        {
            SelectClientModuleSeam.AddModule = (_, __, ___, ____) =>
                throw new NotSupportedException("接缝：AddModule 未移植");

            _tx.Connect();                                      // IDSocketConnect 内部会抛 ⇒ 应被 Guard 吞掉

            Assert.Contains(Logs, s => s.Contains("事件回灌抛异常") && s.Contains("D-p7-11"));
        }
    }

    [Fact]
    public void Dispose_解绑事件()
    {
        var a = NewAdapter(out var frm);
        using (frm)
        {
            _tx.Feed("(1000/acct/42/0/x/1.1.1.1)");
            a.Dispose();
            _tx.Feed("(1000/acct/43/0/x/1.1.1.2)");
            Assert.Equal(1, frm.GlobaSessionCount);             // 第二帧不再回灌
        }
    }

    // =====================================================================================
    // Active 语义（原文 `IDSocket.Active`）
    // =====================================================================================

    [Fact]
    public void Active_读值等于底层连通状态()
    {
        var a = NewAdapter(out var frm);
        using (frm)
        {
            Assert.False(a.Active);
            _tx.IsConnected = true;
            Assert.True(a.Active);
        }
    }

    [Fact]
    public void Active置真_派发连接并触发OnConnected()
    {
        var a = NewAdapter(out var frm);
        using (frm)
        {
            a.Active = true;
            Assert.Equal(1, _tx.ConnectCount);
            Assert.True(a.Active);
            Assert.Equal(new[] { true }, KeepAliveSwitches);    // OnConnected 回灌到了 IDSocketConnect
        }
    }

    [Fact]
    public void Active置真_不内联连接_而是走派发器()
    {
        // ★ 偏差 D-p7-16 的可执行证明：`Active := True` 在原文是**非阻塞**异步连接
        //   （DFM ClientType=ctNonBlocking），而 TcpLink.Connect() 是阻塞的。
        //   这里注入一个"只记录不执行"的派发器 ⇒ 断言 setter 返回时**还没有**连。
        var a = NewAdapter(out var frm, syncDispatcher: false);
        using (frm)
        {
            Action? deferred = null;
            a.ConnectDispatcher = act => deferred = act;

            a.Active = true;

            Assert.Equal(0, _tx.ConnectCount);                  // ★ 没有内联连接
            Assert.NotNull(deferred);
            deferred!();                                        // 模拟后台线程执行
            Assert.Equal(1, _tx.ConnectCount);
            Assert.Equal(new[] { true }, KeepAliveSwitches);
        }
    }

    [Fact]
    public void Active置真_幂等_已连上不再重连()
    {
        var a = NewAdapter(out var frm);
        using (frm)
        {
            a.Active = true;
            a.Active = true;
            Assert.Equal(1, _tx.ConnectCount);
        }
    }

    [Fact]
    public void Active置真_连接失败时留痕()
    {
        _tx.ConnectResult = false;
        var a = NewAdapter(out var frm);
        using (frm)
        {
            a.Active = true;
            Assert.Contains(Logs, s => s.Contains("连接 ID 服务器失败"));
            Assert.False(a.Active);
        }
    }

    [Fact]
    public void Active置假_关闭链路()
    {
        var a = NewAdapter(out var frm);
        using (frm)
        {
            a.Active = true;
            a.Active = false;
            Assert.False(a.Active);
            Assert.True(_tx.CloseCount >= 1);
        }
    }

    // =====================================================================================
    // IDSocCliHost（宿主引导：四组设施）
    // =====================================================================================

    [Fact]
    public void Host_装好四组宿主设施()
    {
        using var frm = new TFrmIDSoc();
        using var host = new IDSocCliHost(frm, () => 7, "10.2.2.2", 5700, _tx);
        try
        {
            Assert.Same(host.Adapter, IDSocCliSeam.IDSocket);
            Assert.Equal(7, IDSocCliSeam.GetSelectCharCount());
            IDSocCliSeam.Timer1Enabled(true);                   // 已接线 ⇒ 不抛
            IDSocCliSeam.Timer1Enabled(false);
            IDSocCliSeam.KeepAliveTimerEnabled(true);
            IDSocCliSeam.KeepAliveTimerEnabled(false);
        }
        finally { IDSocCliSeam.Reset(); }
    }

    [Fact]
    public void Host_构造不发起连接()
    {
        using var frm = new TFrmIDSoc();
        using var host = new IDSocCliHost(frm, () => 0, "10.2.2.2", 5700, _tx);
        try
        {
            Assert.Equal(0, _tx.ConnectCount);                  // ★ 惰性：真正连出去在 OpenConnect
            Assert.Equal(0, _tx.CloseCount);
        }
        finally { IDSocCliSeam.Reset(); }
    }

    [Fact]
    public void Host_Dispose后接缝还原为未接线即抛()
    {
        using var frm = new TFrmIDSoc();
        var host = new IDSocCliHost(frm, () => 0, "10.2.2.2", 5700, _tx);
        host.Dispose();

        Assert.Null(IDSocCliSeam.IDSocket);
        Assert.Throws<NotSupportedException>(() => IDSocCliSeam.RequireIDSocket);
        Assert.Throws<NotSupportedException>(() => IDSocCliSeam.Timer1Enabled(true));
        Assert.Throws<NotSupportedException>(() => IDSocCliSeam.KeepAliveTimerEnabled(true));
        Assert.Throws<NotSupportedException>(() => IDSocCliSeam.GetSelectCharCount());
    }

    [Fact]
    public void Host_OpenConnect与Timer1Timer_按配置设地址端口再激活()
    {
        using var frm = new TFrmIDSoc();
        using var host = new IDSocCliHost(frm, () => 0, "10.3.3.3", 5800, _tx);
        try
        {
            host.Adapter.ConnectDispatcher = act => act();
            // `IDSocCliHost` 会**接管** Timer1Enabled ⇒ 在它之后再包一层记录器（仍会调它真正的实现）
            var innerTimer1 = IDSocCliSeam.Timer1Enabled;
            IDSocCliSeam.Timer1Enabled = v => { Timer1Switches.Add(v); innerTimer1(v); };
            // ★ 原文 :419-420 取的是**全局** `g_sIDServerAddr`/`g_nIDServerPort`（不是构造参数）
            IDSocCliSeam.g_sIDServerAddr = "10.3.3.3";
            IDSocCliSeam.g_nIDServerPort = 5800;

            frm.OpenConnect();                                  // 原文 :415-422
            Assert.Equal("10.3.3.3", host.Adapter.Address);
            Assert.Equal(5800, host.Adapter.Port);
            Assert.True(host.Adapter.Active);
            Assert.Equal(1, _tx.ConnectCount);
            Assert.Equal(new[] { true }, Timer1Switches);        // :417 Timer1.Enabled := True

            frm.CloseConnect();                                 // 原文 :388-393
            Assert.False(host.Adapter.Active);
            Assert.Equal(new[] { true, false }, Timer1Switches); // :390 Timer1.Enabled := False
        }
        finally { IDSocCliSeam.Reset(); }
    }

    [Fact]
    public void Host_定时器间隔取自DFM()
    {
        Assert.Equal(3000, IDSocCliHost.DFM_TIMER1_INTERVAL);           // DFM: vaInt16 B8 0B
        Assert.Equal(10, IDSocCliHost.DFM_KEEPALIVE_TIMER_INTERVAL);    // DFM: vaInt8 0A
    }

    [Fact]
    public void Host_未启用定时器时不触发重连()
    {
        // 构造后两个定时器都是"已创建但未启用"（对应 DFM Enabled=False）⇒ 不应该有任何连接尝试。
        using var frm = new TFrmIDSoc();
        using var host = new IDSocCliHost(frm, () => 0, "10.2.2.2", 5700, _tx);
        try
        {
            host.Adapter.ConnectDispatcher = act => act();
            System.Threading.Thread.Sleep(120);                 // 远小于 3000ms
            Assert.Equal(0, _tx.ConnectCount);
        }
        finally { IDSocCliSeam.Reset(); }
    }

    [Fact]
    public void TcpLinkTransport_转发TcpLink的面()
    {
        // 只验"接线正确"，不验真连接：构造 TcpLinkTransport 不应抛、不应建 socket。
        var t = new TcpLinkTransport("127.0.0.1", 5600);
        Assert.Equal("127.0.0.1", t.Host);
        Assert.Equal(5600, t.Port);
        Assert.False(t.Connected);
        t.Send(new byte[] { 1, 2, 3 });                         // 未连通 ⇒ no-op，不抛
        t.Close();
    }
}
