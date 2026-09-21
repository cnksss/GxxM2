using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.GatewayKit;

namespace GXX.DBServer;

/// <summary>
/// DBServer.pas（uFrmMain/SelectClient/IDSocCli/RoleDB）→ DBServerService.cs
/// 数据库服务器：
/// - 网关端口：接受 SelGate 链接 → **每连接一个 <see cref="TSelectClient"/>**（= 原文 `OnGetSocket`），
///   收包交给 `ExecGateBuffers` → `DeCodeUserMsg` 分派（CM_QUERYCHR/CM_RANDOMNAME/CM_NEWCHR/CM_DELCHR/
///   CM_SELCHR/CM_QUERYDELCHR/CM_GETBACKDELCHR + else）。
/// - 数据端口：接受 M2Server 链接，处理 DB_LOADHUMANRCD / DB_SAVEHUMANRCD（TDBMsgHeader 帧）。
///
/// <para>
/// 【接线边界 —— 诚实登记】以下三条依赖**尚未移植**，接缝默认**抛 NotSupportedException**（不放行）：
/// <list type="number">
///   <item>`IDSocCli.pas`（**384 行**，会话状态机 / LoginSrv 客户端）→ <see cref="IDSocCliSeam.FrmIDSoc"/>。
///         受影响命令：CM_QUERYCHR / CM_RANDOMNAME / CM_NEWCHR / CM_DELCHR / CM_SELCHR（经 `CheckSession`），
///         以及任何 `%X` 帧与非命中槽的 `CloseUser`（经 `GetGlobaSessionStatus`）。</item>
///   <item>`DBShare.pas:1043-1280` 的人物名校验族（**约 240 行**：CheckDenyChrName / CheckChrName /
///         CheckSpecialChar / CheckNumberName / CheckLetterName / CheckFilterNewHumanChrName）
///         → <see cref="SelectClientDbShareSeam"/>。受影响命令：CM_NEWCHR。</item>
///   <item>`DBShare.pas:731-870` GateActiveRouteIP / CheckActiveRunGate（**约 140 行**）。
///         仅当 `g_boUseActiveRunGage = True`（默认 False）时可达。</item>
/// </list>
/// **本服务刻意不为它们提供"放行桩"**：用未实现的校验去"接受"角色名，是安静的错行为（台账 §25.2）。
/// </para>
/// </summary>
public class DBServerService : IGateUiService
{
    private readonly RoleDatabase _roles;
    private Socket? _gateListener;
    private Socket? _m2Listener;
    private Thread? _gateThread;
    private Thread? _m2Thread;
    private volatile bool _running;

    /// <summary>
    /// `TFrmIDSoc` 实例（原文 `FrmIDSoc`）。原文在 `DBServer.dpr:36` 建、由 `uFrmMain.pas` 启停：
    /// `:909 FrmIDSoc.OpenConnect`（服务启动）/ `:960 CloseConnect`（服务停止）/ `:941`（启动异常）/ `:1090`。
    /// ★ **本类当前只建实例，不自动 OpenConnect**：`IIDSocClientSocket` 适配器尚未提供，
    ///   调用它只会抛（那是接缝的正确默认）。启停接线留给宿主，见报告 §14.4。
    /// </summary>
    /// <summary>
    /// `IDSocCli` 的四组宿主设施（socket / 两个定时器 / 在线数）—— 见构造里的说明。
    /// </summary>
    private readonly IDSocCliHost _idSocHost;

    public TFrmIDSoc FrmIDSoc { get; }

    private readonly ConcurrentDictionary<int, TcpLink> _gateLinks = new();
    private readonly ConcurrentDictionary<int, TcpLink> _m2Links = new();

    public int GatePort { get; set; } = 5100;
    public int M2Port { get; set; } = 6000;
    public bool ServiceStarted => _running;
    public int OnlineCount => _gateLinks.Count + _m2Links.Count;
    public string ServiceName => "数据库服务器";
    public string GateAddr => "0.0.0.0";
    public string ServerAddr => "-";

    public event Action<string, int>? OnLogMsg;

    public DBServerService(string dbFile)
    {
        _roles = new RoleDatabase(dbFile);
        // RoleDB 全局 `g_RoleDB.HumanDB/HeroDB`（DBShare.pas:115）—— 接上 SelGate 选人端真正需要的数据操作。
        SelectClientRoleDbSeam.AttachRoleDatabase(_roles);

        // `FrmIDSoc: TFrmIDSoc`（DBShare.pas 全局；原文 DBServer.dpr:36 `Application.CreateForm(TFrmIDSoc, FrmIDSoc)`）。
        // 做完这一步，`CM_QUERYCHR`/`CM_RANDOMNAME`/`CM_NEWCHR`/`CM_DELCHR`/`CM_SELCHR` 不再抛
        // NotSupportedException，而是按会话表**真实判定**；`%X` 也回到原文的清理分支。
        //
        // ★ 不调用 `FrmIDSoc.FormCreate()`：那两行要动**定时器接缝**（宿主设施，默认抛），
        //   而构造阶段"定时器尚未安装"本身就是 `Enabled = False` 的同一状态 ⇒ 见偏差 D-p7-15。
        FrmIDSoc = new TFrmIDSoc();
        IDSocCliSeam.FrmIDSoc = FrmIDSoc;

        // ★★ 2026 第 5 轮：`IDSocCli` 的 **4 组宿主设施全部接线**（`IDSocCliHost`）——
        //   `IDSocket`（TcpLink 适配器）+ `Timer1`(3000ms) + `KeepAliveTimer`(10ms) + `GetSelectCharCount`。
        //   ⇒ 全局会话表**会被 LoginSrv 的真实推送填充**，6 条走 `CheckSession` 的命令不再被无条件拒绝。
        //   `GetSelectCharCount` 取 SelGate 连接数 —— 对应原文 `DBSUSETHREAD=0` 分支下的
        //   `SelectSocket.Socket.ActiveConnections`（uFrmMain.pas:434-453）。
        //   **注意**：构造**不发起连接**（`TcpLink` 构造是惰性的）；真正连出去发生在 `StartService` → `OpenConnect`。
        _idSocHost = new IDSocCliHost(FrmIDSoc, () => _gateLinks.Count,
                                      IDSocCliSeam.g_sIDServerAddr, IDSocCliSeam.g_nIDServerPort);
    }

    public RoleDatabase Roles => _roles;

    public int ServerPort => M2Port;

    public bool StartService()
    {
        if (_running) return true;
        try
        {
            _gateListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _gateListener.Bind(new IPEndPoint(IPAddress.Any, GatePort));
            _gateListener.Listen(16);

            _m2Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _m2Listener.Bind(new IPEndPoint(IPAddress.Any, M2Port));
            _m2Listener.Listen(16);

            _running = true;
            _gateThread = new Thread(GateAcceptLoop) { IsBackground = true, Name = "DBServer-Gate" };
            _gateThread.Start();
            _m2Thread = new Thread(M2AcceptLoop) { IsBackground = true, Name = "DBServer-M2" };
            _m2Thread.Start();

            // `uFrmMain.pas:909 FrmIDSoc.OpenConnect;`（服务启动 ⇒ 连 LoginSrv/ID 服务器并开 Timer1）。
            // ★ 非阻塞：适配器把阻塞的 `TcpLink.Connect()` 派发到后台线程（偏差 D-p7-16）。
            FrmIDSoc.OpenConnect();

            SendLog($"数据库服务器启动（SelGate 端口 {GatePort}，M2 数据端口 {M2Port}）");
            return true;
        }
        catch (Exception ex)
        {
            SendLog("启动失败: " + ex.Message, 1);
            return false;
        }
    }

    public void StopService()
    {
        _running = false;
        // `uFrmMain.pas:960 FrmIDSoc.CloseConnect;`（服务停止 ⇒ 关 Timer1 并断开 ID 服务器连接）。
        // ★ **必须在 `_idSocHost.Dispose()` 之前**：`CloseConnect` 要动定时器与 socket 接缝。
        // 本方法会被 `Dispose()` 调用，届时 `_idSocHost` 仍然存活 ⇒ 正常路径不会撞"未接线即抛"。
        try { FrmIDSoc.CloseConnect(); }
        catch (NotSupportedException ex)
        {
            // 不静默：只有"宿主已把接缝拆掉之后又调 StopService"才会走到这里（正常路径不可达）。
            SendLog("停止服务：IDSocCli 接缝未接线，跳过 CloseConnect —— " + ex.Message, 2);
        }
        try { _gateListener?.Close(); } catch { }
        try { _m2Listener?.Close(); } catch { }
        foreach (var kv in _gateLinks) kv.Value.Close();
        foreach (var kv in _m2Links) kv.Value.Close();
        _gateLinks.Clear();
        _m2Links.Clear();
        SelectClientGateWiring.DetachAll();
    }

    // ---------------- SelGate 端 ----------------

    /// <summary>
    /// uFrmMain.pas:303-306 `SelectSocketGetSocket` + :335-338 `SelectSocketClientDisconnect`
    /// + :340-353 `SelectSocketClientRead` —— 三件事都由 <see cref="SelectClientGateWiring.AttachTcpLink"/> 一次接完。
    ///
    /// ★ 原文一个 SelGate 连接 = 一个 `TSelectClient`（内含 1000 槽会话表，多个玩家共用同一连接）。
    /// </summary>
    private void GateAcceptLoop()
    {
        while (_running)
        {
            try
            {
                var client = _gateListener!.Accept();
                var link = new TcpLink(client);
                int idx = _gateLinks.Count + 1;
                link.OnDisconnected += () => _gateLinks.TryRemove(idx, out _);
                _gateLinks[idx] = link;

                // 原文 `Self.RemoteAddress`（SelectClient.pas:516 的 'S' 分支用它）。
                // TcpLink(Socket) 的 Host 是空串（见 TcpLink.cs:40），故从 Socket 端点取。
                string remote = (client.RemoteEndPoint as IPEndPoint)?.Address.ToString() ?? "";
                SelectClientGateWiring.AttachTcpLink(link, remote);
            }
            catch { if (!_running) return; }
        }
    }

    // 原文 uFrmMain.pas 的 SelGate 收包事件只做一件事：
    //   `sReceiveText := Socket.ReceiveText;  ExecGateBuffers(sReceiveText);`
    // 具体分派（CM_100/101/102/103/105/106/3006 + else）在 `TSelectClient.DeCodeUserMsg` 里，
    // 原先本文件自造的 4 路 `switch`（含**字段位置错误**的 SM_QUERYCHR，见报告 §11.3）已删除。

    // ---------------- M2Server 数据端 ----------------

    private void M2AcceptLoop()
    {
        while (_running)
        {
            try
            {
                var client = _m2Listener!.Accept();
                var link = new TcpLink(client);
                int idx = _m2Links.Count + 1;
                link.OnReceive += (buf, off, len) =>
                {
                    link.Accumulate(buf, off, len);
                    ProcessM2Data(link);
                };
                link.OnDisconnected += () => _m2Links.TryRemove(idx, out _);
                _m2Links[idx] = link;
                SendLog("M2Server 已连接数据端口");
            }
            catch { if (!_running) return; }
        }
    }

    private void ProcessM2Data(TcpLink link)
    {
        // M2 数据协议：TDBMsgHeader(dwCode=$AA55AA00+idx, dwCrc, DefMsg: TDefaultMessage, nLength) + Data
        int headSize = StructBytes.SizeOf<TDBMsgHeader>();
        while (link.AccumLength >= headSize)
        {
            var header = StructBytes.FromBytes<TDBMsgHeader>(link.AccumBuffer, 0);
            if (header.nLength < 0 || header.nLength > 4 * 1024 * 1024)
            {
                link.ConsumeAccum(link.AccumLength);
                return;
            }
            if (link.AccumLength < headSize + header.nLength)
                return;

            byte[]? data = null;
            if (header.nLength > 0)
            {
                data = new byte[header.nLength];
                Array.Copy(link.AccumBuffer, headSize, data, 0, header.nLength);
            }
            link.ConsumeAccum(headSize + header.nLength);

            HandleM2Request(link, header, data);
        }
    }

    private void HandleM2Request(TcpLink link, TDBMsgHeader header, byte[]? data)
    {
        switch (header.DefMsg.Ident)
        {
            case CommonConst.DB_CHECKCONNECT:
            {
                SendM2Reply(link, header, CommonConst.DB_CHECKCONNECT, ((long)1).ToBytes());
                break;
            }
            case CommonConst.DB_LOADHUMANRCD:
            {
                if (data == null || data.Length < StructBytes.SizeOf<TDBLoadHuman>()) return;
                var req = StructBytes.FromBytes<TDBLoadHuman>(data);
                var hum = _roles.LoadHum(req.Account, req.HumanName);
                byte[] payload = hum != null ? StructBytes.BytesOf(hum.Value) : Array.Empty<byte>();
                SendM2Reply(link, header, CommonConst.DB_LOADHUMANRCD, payload);
                break;
            }
            case CommonConst.DB_SAVEHUMANRCD:
            {
                if (data == null || data.Length < 8 + StructBytes.SizeOf<THumData>()) return;
                // TDBSaveHuman: nSessionID(4) + PlayObject(8 语义上 TObject→Int64 但紧凑写法 4+4) — 兼容：按 M2 侧发送格式解析
                // 本实现采用 [nSessionID(4)][PlayObject(8)][THumData]
                int sid = BitConverter.ToInt32(data, 0);
                var hum = StructBytes.FromBytes<THumData>(data, 12);
                bool ok = _roles.SaveHum(hum.Account, hum.ChrName, hum);
                SendM2Reply(link, header, CommonConst.DB_SAVEHUMANRCD, ok ? new byte[] { 1 } : new byte[] { 0 });
                break;
            }
            case CommonConst.DB_LOADHERORCD:
            {
                if (data == null || data.Length < StructBytes.SizeOf<TDBLoadHero>()) return;
                var req = StructBytes.FromBytes<TDBLoadHero>(data);
                var hero = _roles.LoadHero(req.Account, req.HumanName);
                byte[] payload = hero != null ? StructBytes.BytesOf(hero.Value) : Array.Empty<byte>();
                SendM2Reply(link, header, CommonConst.DB_LOADHERORCD, payload);
                break;
            }
            case CommonConst.DB_SAVEHERORCD:
            {
                if (data == null || data.Length < 12 + StructBytes.SizeOf<THeroData>()) return;
                var hero = StructBytes.FromBytes<THeroData>(data, 12);
                bool ok = _roles.SaveHero(hero.Account, hero.ChrName, hero);
                SendM2Reply(link, header, CommonConst.DB_SAVEHERORCD, ok ? new byte[] { 1 } : new byte[] { 0 });
                break;
            }
        }
    }

    private void SendM2Reply(TcpLink link, TDBMsgHeader req, ushort ident, byte[] data)
    {
        var reply = new TDBMsgHeader
        {
            dwCode = req.dwCode,
            dwCrc = req.dwCrc,
            DefMsg = TDefaultMessage.Make(ident, req.DefMsg.Recog, 0, 0, 0),
            nLength = data.Length
        };
        byte[] head = StructBytes.BytesOf(reply);
        byte[] buf = new byte[head.Length + data.Length];
        Array.Copy(head, buf, head.Length);
        Array.Copy(data, 0, buf, head.Length, data.Length);
        link.Send(buf);
    }

    private void SendLog(string msg, int level = 3) => OnLogMsg?.Invoke(msg, level);

    public void OnKeepAliveTimer() { }

    public void Dispose()
    {
        // 顺序关键：`StopService` 里要调 `FrmIDSoc.CloseConnect()`（动定时器与 socket 接缝）
        // ⇒ 必须在 `_idSocHost.Dispose()`（会把接缝还原成"未接线即抛"）**之前**。
        StopService();
        _idSocHost.Dispose();

        // 断开 `g_RoleDB.HumanDB/HeroDB` 与 `FrmIDSoc`，避免留下指向已释放对象的悬垂接缝。
        SelectClientRoleDbSeam.DetachRoleDatabase();
        if (ReferenceEquals(IDSocCliSeam.FrmIDSoc, FrmIDSoc)) IDSocCliSeam.FrmIDSoc = null;
        FrmIDSoc.FormDestroy();
        FrmIDSoc.Dispose();
        _roles.Dispose();
    }
}

internal static class BytesExt
{
    public static byte[] ToBytes(this long v) => BitConverter.GetBytes(v);
}
