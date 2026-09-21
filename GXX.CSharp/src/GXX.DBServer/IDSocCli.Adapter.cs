using System;
using System.Threading;
using GXX.GatewayKit;
using TThreadTimer = System.Threading.Timer;

namespace GXX.DBServer;

// ============================================================================================
// `IDSocCli.pas` 的**宿主设施**：把 `GXX.GatewayKit.TcpLink` 适配成 JSocket.pas 的 TClientSocket，
// 并把 DFM 里的两个 TTimer 与 `uFrmMain.GetSelectCharCount` 真正接起来。
//
// 【为什么这个文件在 `GXX.DBServer` 而不是 `GXX.GatewayKit`】
//   它实现的是 **`GXX.DBServer` 自己的接缝**（`IIDSocClientSocket` / `IIDSocSocketEndPoint` /
//   `IDSocCliSeam`），这些类型定义在 `GXX.DBServer` 里；而 `GXX.GatewayKit` **不引用** `GXX.DBServer`
//   （依赖方向是 DBServer → GatewayKit）。放进 GatewayKit 会造成**反向依赖**。
//   `GXX.DBServer` 本来就在用 `TcpLink`（`DBServerService` 的 SelGate/M2 链路），项目引用齐备。
//
// 【为什么多一层 `IIDSocTransport`】
//   `TcpLink` 是**具体类且内部持有真 socket** ⇒ 单测无法替身；而本工程规程明确
//   「真实 socket 的端到端用例不引入」（既有集成测试有 TOCTOU 端口竞态前科）。
//   ⇒ 把"传输"再抽一层：**只有 `TcpLinkTransport` 碰真 socket**，测试注入 `FakeTransport`。
//
// 【原文语义的两处必须显式对齐（都记在偏差里）】
//   · `IDSocket.Active := True`（DFM `ClientType = ctNonBlocking`）是**非阻塞**的异步连接；
//     而 `TcpLink.Connect()` 是**阻塞**的 ⇒ 见 `IDSocClientSocketAdapter.ConnectDispatcher`（D-p7-16）。
//   · `Socket.LocalPort`：`TcpLink` 不暴露本端端点 ⇒ 见 `LocalPortProvider`（D-p7-17）。
// ============================================================================================

/// <summary>
/// 接缝：`TcpLink` 的**最小传输面**（本文件唯一需要替身的东西）。
/// 成员一一对应 `IIDSocSocketEndPoint` / `IIDSocClientSocket` 真正会用到的那几个。
/// </summary>
public interface IIDSocTransport
{
    /// <summary>底层链路是否已连通（对应 `IDSocket.Socket.Connected`）。</summary>
    bool Connected { get; }

    /// <summary>对端地址（对应 `Socket.RemoteAddress`；对客户端链路就是连出去的目标）。</summary>
    string Host { get; }

    /// <summary>对端端口（对应 `Socket.RemotePort`）。</summary>
    int Port { get; }

    /// <summary>
    /// 设定连接目标。
    /// ★ 必须有它：原文 `IDSocket.Address`/`Port`（:94-95、:419-420）是**决定连哪里**的属性，
    ///   而 `TcpLink` 的 `Host`/`Port` 是**只读**的 ⇒ 适配器必须把目标下推到这里，
    ///   否则 `Address`/`Port` 会变成"写进去没人读"的**死属性**。
    /// </summary>
    void SetTarget(string host, int port);

    /// <summary>发起连接。返回 false = 失败（调用方负责留痕）。</summary>
    bool Connect();

    /// <summary>发送（未连通时应为 no-op，与 `TcpLink.Send` 一致）。</summary>
    void Send(byte[] data);

    /// <summary>关闭链路。</summary>
    void Close();

    event Action<byte[], int, int>? OnReceive;
    event Action? OnConnected;
    event Action? OnDisconnected;
}

/// <summary>
/// 接缝实现：用 <see cref="TcpLink"/> 承担真实传输（**本文件唯一碰真 socket 的地方**）。
///
/// ★ `TcpLink` 的 `Host`/`Port` 是构造时固定的只读属性 ⇒ 本类把目标存在自己身上，
/// **每次 `Connect()` 现建一个 `TcpLink`**。这样 `SetTarget` 才真的能改掉"连哪里"，
/// 也才配得上原文 `IDSocket.Address/Port` 的语义（见 `IIDSocTransport.SetTarget`）。
/// </summary>
public sealed class TcpLinkTransport : IIDSocTransport
{
    private TcpLink? _link;
    private string _host;
    private int _port;

    public TcpLinkTransport(string host, int port)
    {
        _host = host;
        _port = port;
    }

    public bool Connected => _link?.Connected ?? false;
    public string Host => _host;
    public int Port => _port;

    public void SetTarget(string host, int port)
    {
        _host = host;
        _port = port;
    }

    public bool Connect()
    {
        Close();                                                    // 与 `TcpLink.Connect()` 内部的"先 Close"一致
        var link = new TcpLink(_host, _port);
        link.OnReceive += (buf, off, len) => OnReceive?.Invoke(buf, off, len);
        link.OnConnected += () => OnConnected?.Invoke();
        link.OnDisconnected += () => OnDisconnected?.Invoke();
        _link = link;
        return link.Connect();
    }

    public void Send(byte[] data) => _link?.Send(data);

    public void Close()
    {
        TcpLink? link = _link;
        _link = null;
        link?.Close();
    }

    public event Action<byte[], int, int>? OnReceive;
    public event Action? OnConnected;
    public event Action? OnDisconnected;
}

/// <summary>
/// 适配器：<see cref="IIDSocTransport"/> → `IDSocCli.pas` 期望的 `TClientSocket` 面
/// （<see cref="IIDSocClientSocket"/> 与它的 <see cref="IIDSocSocketEndPoint"/>）。
///
/// 除了转发，它还把**原文由 VCL 组件触发的那三个事件**回灌到 `TFrmIDSoc`：
/// `OnReceive → IDSocketRead()`、`OnConnected → IDSocketConnect()`、`OnDisconnected → IDSocketDisconnect()`。
/// </summary>
public sealed class IDSocClientSocketAdapter : IIDSocClientSocket, IDisposable
{
    private readonly TFrmIDSoc _frm;
    private readonly IIDSocTransport _transport;
    private readonly IIDSocSocketEndPoint _endPoint;

    /// <summary>待读文本（`Socket.ReceiveText` 的语义是"取走并清空"）。以 **latin-1 字节串**承载（D-p7-9）。</summary>
    private string _pending = "";

    /// <summary>连接中标志：防止 3000ms 的 `Timer1` 反复重入发起连接。</summary>
    private volatile bool _connecting;

    public IDSocClientSocketAdapter(TFrmIDSoc frm, IIDSocTransport transport)
    {
        _frm = frm;
        _transport = transport;
        _endPoint = new EndPoint(this);
        _transport.OnReceive += OnTransportReceive;
        _transport.OnConnected += OnTransportConnected;
        _transport.OnDisconnected += OnTransportDisconnected;
    }

    /// <summary>
    /// 连接派发器。**可注入**：生产用线程池，测试用同步派发（`a =&gt; a()`）。
    ///
    /// <para><b>为什么需要它</b>（偏差 **D-p7-16**）：原文 `IDSocket.Active := True` 走的是
    /// `ClientType = ctNonBlocking` 的 **TClientSocket**，即**异步连接** —— 调用方立刻返回。
    /// 而 `TcpLink.Connect()` 是 `Socket.Connect()` 的**阻塞**实现。
    /// 若直接阻塞，`TFrmIDSoc.OpenConnect()`（由 `StartService` 调用，对应 `uFrmMain.pas:909`）
    /// 会把**服务启动**卡在 TCP 超时上。⇒ 放到后台线程，保持"设完就返回"的原文语义。</para>
    /// </summary>
    public Action<Action> ConnectDispatcher = a => System.Threading.Tasks.Task.Run(a);

    /// <summary>
    /// `Socket.LocalPort` 的提供者（偏差 **D-p7-17**）。
    /// ★ `TcpLink`（**只读、非本车道**）不暴露本端端点 ⇒ 默认返回 0 **并留一条日志**（§25.2：不静默）。
    /// 它只进入 `IDSocketConnect` 的模块表显示串（`ModuleInfo.Address`），**不参与任何判定**。
    /// </summary>
    public Func<int> LocalPortProvider = DefaultLocalPort;

    private static int DefaultLocalPort()
    {
        RoleDbSeam.MainOutMessage(
            "[WARN] IDSocCli 适配器：LocalPort 未注入（TcpLink 不暴露本端端点）⇒ 模块表 Address 显示为 0。"
            + "它只用于显示，不参与任何判定。");
        return 0;
    }

    // ==========================================================================================
    // IIDSocClientSocket（= DFM 的 IDSocket: TClientSocket）
    // ==========================================================================================

    /// <summary>
    /// DFM `IDSocket.Active`。
    /// 读：底层是否连通。写 <c>true</c>：发起连接（**异步**，见 <see cref="ConnectDispatcher"/>）；
    /// 写 <c>false</c>：关闭（对应 `Active := False`）。
    /// </summary>
    public bool Active
    {
        get => _transport.Connected;
        set
        {
            if (!value)
            {
                _connecting = false;
                _transport.Close();
                return;
            }
            if (_transport.Connected || _connecting) return;         // 幂等：已连上/正在连就不再发一次
            _connecting = true;
            ConnectDispatcher(() =>
            {
                try
                {
                    if (!_transport.Connect())
                    {
                        // 原文的失败由 VCL 的 OnError 上报；这里等价地留痕（Timer1 每 3000ms 会重试）。
                        RoleDbSeam.MainOutMessage(
                            "[WARN] IDSocCli 适配器：连接 ID 服务器失败（" + _transport.Host + ":" + _transport.Port
                            + "）；Timer1 会在下个周期重试。");
                    }
                }
                catch (Exception ex)
                {
                    RoleDbSeam.MainOutMessage("[ERROR] IDSocCli 适配器：连接 ID 服务器抛异常：" + ex.Message);
                }
                finally
                {
                    _connecting = false;
                }
            });
        }
    }

    /// <summary>
    /// DFM `IDSocket.Address`。原文 :94/:419 用它定"连哪个 ID 服务器" —— 取值来源 `g_sIDServerAddr`。
    /// ★ 必须真的下推到传输层（见 <see cref="IIDSocTransport.SetTarget"/>），否则就是死属性。
    /// </summary>
    public string Address
    {
        get => _transport.Host;
        set => _transport.SetTarget(value, _transport.Port);
    }

    /// <summary>DFM `IDSocket.Port`。原文 :95/:420 用它定端口 —— 取值来源 `g_nIDServerPort`。</summary>
    public int Port
    {
        get => _transport.Port;
        set => _transport.SetTarget(_transport.Host, value);
    }

    /// <summary>DFM `IDSocket.Socket`。</summary>
    public IIDSocSocketEndPoint Socket => _endPoint;

    // ==========================================================================================
    // 事件回灌（原文由 VCL 组件触发）
    // ==========================================================================================

    private void OnTransportReceive(byte[] buf, int offset, int len)
    {
        _pending += SelectClientAnsi.StrOf(buf, offset, len);
        Guard(() => _frm.IDSocketRead());
    }

    private void OnTransportConnected() => Guard(() => _frm.IDSocketConnect());

    private void OnTransportDisconnected() => Guard(() => _frm.IDSocketDisconnect());

    /// <summary>
    /// 宿主边界（与 <c>SelectClientGateWiring.FeedSafe</c> 的 **D-p7-11** 同一处置）：
    /// 这三个回调跑在**接收线程**上，异常若逃出去会打断接收循环；且被回灌的代码里还挂着
    /// 尚未接线的接缝（如模块表）⇒ 记日志后吞掉，**绝不让接收线程崩**。
    /// </summary>
    private static void Guard(Action a)
    {
        try { a(); }
        catch (Exception ex)
        {
            RoleDbSeam.MainOutMessage("[ERROR] IDSocCli 适配器：事件回灌抛异常（宿主边界，D-p7-11 同款）：" + ex);
        }
    }

    public void Dispose()
    {
        _transport.OnReceive -= OnTransportReceive;
        _transport.OnConnected -= OnTransportConnected;
        _transport.OnDisconnected -= OnTransportDisconnected;
        _transport.Close();
    }

    // ==========================================================================================
    // IIDSocSocketEndPoint（= IDSocket.Socket）
    // ==========================================================================================

    private sealed class EndPoint : IIDSocSocketEndPoint
    {
        private readonly IDSocClientSocketAdapter _o;

        public EndPoint(IDSocClientSocketAdapter o) => _o = o;

        public bool Connected => _o._transport.Connected;

        /// <summary>`Socket.ReceiveText`：**取走并清空**已到达的文本（VCL 语义）。</summary>
        public string ReceiveText
        {
            get
            {
                string s = _o._pending;
                _o._pending = "";
                return s;
            }
        }

        public string RemoteAddress => _o._transport.Host;
        public int LocalPort => _o.LocalPortProvider();
        public int RemotePort => _o._transport.Port;

        /// <summary>`Socket.SendText`：latin-1 字节串 → 原始字节（D-p7-9）。</summary>
        public void SendText(string sText) => _o._transport.Send(SelectClientAnsi.BytesOf(sText));

        public void Close() => _o._transport.Close();
    }
}

/// <summary>
/// 宿主引导：把 `IDSocCli` 需要的 **4 组宿主设施**一次性装好并登记，`Dispose` 时全部还原为"未接线即抛"。
///
/// <list type="number">
///   <item><see cref="IDSocCliSeam.IDSocket"/> ← <see cref="IDSocClientSocketAdapter"/>；</item>
///   <item><see cref="IDSocCliSeam.Timer1Enabled"/> ← DFM `Timer1`（**Interval = 3000**）；</item>
///   <item><see cref="IDSocCliSeam.KeepAliveTimerEnabled"/> ← DFM `KeepAliveTimer`（**Interval = 10**）；</item>
///   <item><see cref="IDSocCliSeam.GetSelectCharCount"/> ← 宿主给的委托（本仓 = SelGate 连接数）。</item>
/// </list>
///
/// ★ **两个 Interval 不是猜的**，是从 `IDSocCli.dfm` 的二进制属性值里解出来的：
/// `Timer1.Interval` = `vaInt16(0x03) B8 0B` = **3000**；`KeepAliveTimer.Interval` = `vaInt8(0x02) 0A` = **10**；
/// 两个 `Enabled` 都是 `vaFalse(0x08)`（与 `FormCreate` 里再置 False 一致）。
/// `KeepAliveTimer` 每 10ms 醒一次，但 `SendKeepAlivePacket` 自带 3000ms 节流（原文 :377）—— 这是原文设计。
/// </summary>
public sealed class IDSocCliHost : IDisposable
{
    /// <summary>DFM `Timer1.Interval`（自动重连轮询）。</summary>
    public const int DFM_TIMER1_INTERVAL = 3000;

    /// <summary>DFM `KeepAliveTimer.Interval`（保活轮询；真正发送由 `SendKeepAlivePacket` 的 3000ms 节流决定）。</summary>
    public const int DFM_KEEPALIVE_TIMER_INTERVAL = 10;

    private readonly TFrmIDSoc _frm;
    private readonly TThreadTimer _timer1;
    private readonly TThreadTimer _keepAliveTimer;
    private readonly Func<int> _selectCharCount;
    private bool _disposed;

    public IDSocClientSocketAdapter Adapter { get; }

    /// <param name="idServerAddr">
    /// 传输层的**初始**目标。★ 注意：`TFrmIDSoc.OpenConnect`/`Timer1Timer` 在连接前会把
    /// `IDSocket.Address/Port` 覆盖成 `g_sIDServerAddr`/`g_nIDServerPort`（原文 :94-95/:419-420）
    /// ⇒ 常规路径下**真正生效的是那两个全局**，本参数只在"不经 OpenConnect 直接激活"时起作用。
    /// </param>
    public IDSocCliHost(TFrmIDSoc frm, Func<int> selectCharCount,
                        string idServerAddr, int idServerPort,
                        IIDSocTransport? transport = null)
    {
        _frm = frm;
        _selectCharCount = selectCharCount;
        Adapter = new IDSocClientSocketAdapter(frm, transport ?? new TcpLinkTransport(idServerAddr, idServerPort));

        // DFM 的两个 TTimer 都是 `Enabled = False`；`System.Threading.Timer` 构造即开始计时
        // ⇒ 用 Infinite 造出"已创建但未启用"的等价物。
        _timer1 = new TThreadTimer(_ => Adapter.ConnectDispatcher(() => Guard(_frm.Timer1Timer)),
                                   null, Timeout.Infinite, Timeout.Infinite);
        _keepAliveTimer = new TThreadTimer(_ => Guard(_frm.KeepAliveTimerTimer),
                                           null, Timeout.Infinite, Timeout.Infinite);

        IDSocCliSeam.IDSocket = Adapter;
        IDSocCliSeam.Timer1Enabled = v => SetTimer(_timer1, DFM_TIMER1_INTERVAL, v);
        IDSocCliSeam.KeepAliveTimerEnabled = v => SetTimer(_keepAliveTimer, DFM_KEEPALIVE_TIMER_INTERVAL, v);
        IDSocCliSeam.GetSelectCharCount = _selectCharCount;
    }

    /// <summary>`TTimer.Enabled := v`：True ⇒ 按 Interval 周期触发（首次在 Interval 之后，与 VCL 一致）。</summary>
    private static void SetTimer(TThreadTimer t, int interval, bool enabled)
        => t.Change(enabled ? interval : Timeout.Infinite, enabled ? interval : Timeout.Infinite);

    /// <summary>宿主边界：定时器回调跑在线程池线程上，异常不得逃逸。</summary>
    private static void Guard(Action a)
    {
        try { a(); }
        catch (Exception ex)
        {
            RoleDbSeam.MainOutMessage("[ERROR] IDSocCliHost：定时器回调抛异常（宿主边界）：" + ex);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _timer1.Dispose();
        _keepAliveTimer.Dispose();
        Adapter.Dispose();

        // 还原为"未接线即抛"（§25.2）—— 只还原**仍然指向自己**的那些，避免抹掉别人的接线。
        if (ReferenceEquals(IDSocCliSeam.IDSocket, Adapter)) IDSocCliSeam.IDSocket = null;
        if (ReferenceEquals(IDSocCliSeam.GetSelectCharCount, _selectCharCount))
            IDSocCliSeam.GetSelectCharCount = UnwiredSelectCharCount;
        IDSocCliSeam.Timer1Enabled = UnwiredTimer1;
        IDSocCliSeam.KeepAliveTimerEnabled = UnwiredKeepAliveTimer;
    }

    private static readonly Func<int> UnwiredSelectCharCount = () => throw new NotSupportedException(
        "接缝：uFrmMain.pas:434-453 GetSelectCharCount 未接线。接入点：宿主把 SelGate 连接数赋给 IDSocCliSeam.GetSelectCharCount。");

    private static readonly Action<bool> UnwiredTimer1 = _ => throw new NotSupportedException(
        "接缝：VCL TTimer（IDSocCli.pas 的 Timer1）未接线。接入点：宿主把定时器开关赋给 IDSocCliSeam.Timer1Enabled。");

    private static readonly Action<bool> UnwiredKeepAliveTimer = _ => throw new NotSupportedException(
        "接缝：VCL TTimer（IDSocCli.pas 的 KeepAliveTimer）未接线。接入点：宿主把定时器开关赋给 IDSocCliSeam.KeepAliveTimerEnabled。");
}
