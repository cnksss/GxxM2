using System;
using System.Collections.Generic;
using GXX.GatewayKit;

namespace GXX.DBServer;

// ============================================================================================
// SelectClient.pas 的**宿主接线**：把「一条 SelGate 连接」与「一个 TSelectClient」绑起来。
//
// 对应 uFrmMain.pas（DBServer 主窗体）的 TServerSocket(SelectSocket) 四个事件 —— 原文逐个列出：
//   · OnGetSocket        :303-306  `ClientSocket := TSelectClient.Create(Socket, SelectSocket.Socket);`
//   · OnClientConnect    :308-333  IP 白名单（CheckServerIP）+ 模块表登记（那段被注释掉了）
//   · OnClientDisconnect :335-338  `RemoveModule(Socket)`
//   · OnClientRead       :340-353  `sReceiveText := Socket.ReceiveText; ExecGateBuffers(sReceiveText);`
//
// 本类放在本车道分区内（`SelectClient*.cs`），使 `DBServerService` 只需 3 行即可接管；
// 同时让「每连接一个会话表 + 出站路由 + 收包喂入」这一层**可被单测覆盖** ——
// 在此之前，SelGate 命令路径（原 `ProcessGateData` 的 4 条命令）**一条测试都没有**（§28.3 双根搜索结果，见报告）。
//
// ★ 为什么出站要按实例路由：原文 `TSelectClient` **本身就是 socket 对象**
//   （`OpenUser:697` 写的是 `UserInfo.Socket := Self`），所以 `SendText` 天然知道发给谁。
//   托管侧把 `SendText`/`RemoteAddress` 收敛成静态接缝后，就**必须**在这里按实例反查绑定的连接，
//   否则所有连接的发包会挤到同一个出口（这正是"接缝做过头"的典型形态，台账 §18.8）。
// ============================================================================================

/// <summary>
/// SelGate 连接 ↔ <see cref="TSelectClient"/> 的绑定表。
///
/// 用法（`DBServerService.GateAcceptLoop`）：
/// <code>
/// var remote = (client.RemoteEndPoint as IPEndPoint)?.Address.ToString() ?? "";
/// var select = SelectClientGateWiring.AttachTcpLink(link, remote);   // = OnGetSocket + OnClientRead + OnClientDisconnect
/// </code>
/// </summary>
public static class SelectClientGateWiring
{
    /// <summary>一条连接的两个出口（原文由 TServerClientWinSocket 自己持有）。</summary>
    private sealed class LinkBinding
    {
        public Action<byte[]> SendToGate = _ => { };
        public string RemoteAddress = "";
    }

    private static readonly object Gate = new object();
    private static readonly Dictionary<TSelectClient, LinkBinding> Bindings = new Dictionary<TSelectClient, LinkBinding>();

    /// <summary>当前已接线（未 Detach）的连接数（单测/诊断用）。</summary>
    public static int Count
    {
        get { lock (Gate) return Bindings.Count; }
    }

    // ==========================================================================================
    // 接线 / 断线
    // ==========================================================================================

    /// <summary>
    /// uFrmMain.pas:303-306 `SelectSocketGetSocket`：
    /// 为一条 SelGate 连接建一个 <see cref="TSelectClient"/>（该连接的 1000 槽会话表就挂在它身上）。
    /// </summary>
    /// <param name="sendToGate">该连接的发包出口（真实场景传 <c>TcpLink.Send</c>）。</param>
    /// <param name="remoteAddress">该连接的对端地址（原文 <c>Self.RemoteAddress</c>，ExecGateBuffers 的 'S' 分支用）。</param>
    public static TSelectClient Attach(Action<byte[]> sendToGate, string remoteAddress)
    {
        var client = new TSelectClient();
        lock (Gate)
        {
            // ★ 每次 Attach 都重装（而不是"只装一次"）：测试基类会先装自己的记录桩，
            //   若这里只装一次，后续 Attach 出的连接就会静默走错出口。
            TSelectClient.SendTextSink = SinkSendText;
            TSelectClient.RemoteAddressSink = SinkRemoteAddress;
            Bindings[client] = new LinkBinding
            {
                SendToGate = sendToGate ?? (_ => { }),
                RemoteAddress = remoteAddress ?? "",
            };
        }
        return client;
    }

    /// <summary>
    /// uFrmMain.pas:303-306 + :335-338 + :340-353 的完整接线（推荐入口）：
    /// 建会话 + 把 <see cref="TcpLink.OnReceive"/> 接到解帧器 + 把 <see cref="TcpLink.OnDisconnected"/> 接到 Detach。
    ///
    /// ★ 原文的 `Socket.ReceiveText` 已经是"本次收到的全部字节"；**不需要**外部再拼累计缓冲 ——
    ///   半包由 `ExecGateBuffers` 自己的 `m_sReceiveText` 保留（原文 :456/:460 的 `Break` 分支）。
    /// </summary>
    public static TSelectClient AttachTcpLink(TcpLink link, string remoteAddress)
    {
        if (link == null) throw new ArgumentNullException(nameof(link));
        TSelectClient client = Attach(link.Send, remoteAddress);
        link.OnReceive += (buf, off, len) => Feed(client, buf, off, len);
        link.OnDisconnected += () => Detach(client);
        return client;
    }

    /// <summary>uFrmMain.pas:335-338 `SelectSocketClientDisconnect` → `RemoveModule(Socket)`。</summary>
    public static void Detach(TSelectClient client)
    {
        if (client == null) return;
        lock (Gate) Bindings.Remove(client);
    }

    /// <summary>清空全部绑定（宿主 StopService / 单测隔离）。</summary>
    public static void DetachAll()
    {
        lock (Gate) Bindings.Clear();
    }

    // ==========================================================================================
    // 收包
    // ==========================================================================================

    /// <summary>
    /// uFrmMain.pas:340-353 `SelectSocketClientRead`：
    /// <c>sReceiveText := Socket.ReceiveText;</c> → <c>ExecGateBuffers(sReceiveText);</c>
    ///
    /// 走 string 重载（原文真正生效的那一支）；<c>offset/len</c> 先按 latin-1 切片，
    /// 与原文把整段 AnsiString 交给 ExecGateBuffers 等价。
    /// </summary>
    public static void Feed(TSelectClient client, byte[] buffer, int offset, int len)
    {
        if (client == null || buffer == null || len <= 0) return;
        client.ExecGateBuffers(SelectClientAnsi.StrOf(buffer, offset, len));
    }

    // ==========================================================================================
    // 静态接缝实现（按实例反查绑定）
    // ==========================================================================================

    private static void SinkSendText(TSelectClient client, byte[] sMsg)
    {
        LinkBinding? binding;
        lock (Gate) Bindings.TryGetValue(client, out binding);
        if (binding == null)
        {
            // ★ 绝不静默丢弃（台账 §25.2）：没接线的连接发包 = 接线漏了，必须立刻可见。
            throw new InvalidOperationException(
                "接线：TSelectClient 未通过 SelectClientGateWiring.Attach 绑定到 SelGate 连接（或已 Detach）。" +
                "接入点：DBServerService.GateAcceptLoop 用 SelectClientGateWiring.AttachTcpLink(link, remote) 建会话。");
        }
        binding.SendToGate(sMsg);
    }

    private static string SinkRemoteAddress(TSelectClient client)
    {
        LinkBinding? binding;
        lock (Gate) Bindings.TryGetValue(client, out binding);
        if (binding == null)
        {
            throw new InvalidOperationException(
                "接线：TSelectClient 未通过 SelectClientGateWiring.Attach 绑定到 SelGate 连接（或已 Detach）。" +
                "接入点：DBServerService.GateAcceptLoop 用 SelectClientGateWiring.AttachTcpLink(link, remote) 建会话。");
        }
        return binding.RemoteAddress;
    }
}
