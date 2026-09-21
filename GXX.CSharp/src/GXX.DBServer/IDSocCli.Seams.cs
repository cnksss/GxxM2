using System;

namespace GXX.DBServer;

// ============================================================================================
// 接缝：`Source\DBServer\IDSocCli.pas`（464 行）依赖、而**未移植**的外部面。
//
// 【本文件的归属变更】`ITFrmIDSoc` 与 `IDSocCliSeam` 原先暂放在 `SelectClient.Seams.cs`
//   （当时 IDSocCli 尚未移植，SelectClient 只是它的一个消费者）。IDSocCli 移植后**搬到这里**，
//   让"IDSocCli 的接缝"与"IDSocCli 的实现"同处一个文件族（`IDSocCli*.cs`）。
//   `SelectClient.cs` 的调用点**一行都不用改**（同命名空间）。
//
// ★ 台账 §25.2：本文件里所有"宿主面"接缝的默认实现**一律抛 NotSupportedException**，
//   绝不静默返回中性值。它们不是"未移植算法"，而是"运行时宿主必须提供的设施"
//   （socket / 定时器 / 模块表 / 全局配置），没有中性默认值在语义上是成立的。
// ============================================================================================

/// <summary>
/// 接缝：`IDSocCli.pas:36-60` <c>TFrmIDSoc</c> 的公开面中，**被本车道之外消费**的部分。
/// （`SelectClient.pas` 只用到其中 6 个；`TFrmIDSoc` 本体实现了全部 9 个公开方法。）
/// </summary>
public interface ITFrmIDSoc
{
    /// <summary>IDSocCli.pas:40 `function CheckSession(sAccount, sIPaddr: string; nSessionID: Integer): Boolean;`</summary>
    bool CheckSession(string sAccount, string sIPaddr, int nSessionID);

    /// <summary>IDSocCli.pas:45 `procedure SetGlobaSessionNoPlay(nSessionID: Integer);`</summary>
    void SetGlobaSessionNoPlay(int nSessionID);

    /// <summary>IDSocCli.pas:46 `procedure SetGlobaSessionPlay(nSessionID: Integer);`</summary>
    void SetGlobaSessionPlay(int nSessionID);

    /// <summary>IDSocCli.pas:47 `function GetGlobaSessionStatus(nSessionID: Integer): Boolean;`</summary>
    bool GetGlobaSessionStatus(int nSessionID);

    /// <summary>IDSocCli.pas:39 `procedure SendSocketMsg(wIdent: Word; sMsg: string);`</summary>
    void SendSocketMsg(ushort wIdent, string sMsg);

    /// <summary>IDSocCli.pas:48 `procedure CloseSession(sAccount: string; nSessionID: Integer);`</summary>
    void CloseSession(string sAccount, int nSessionID);
}

// ============================================================================================
// 接缝：JSocket.pas / ScktComp.pas 的 `TClientSocket`（**未移植**）。
//   IDSocCli.pas 只用它的 10 个成员；拆成内外两层是为了让移植后的代码保持
//   `IDSocket.Socket.XXX` 的原文形状（见 `TFrmIDSoc` 各处）。
// ============================================================================================

/// <summary>接缝：<c>IDSocket.Socket</c>（TClientSocket.Socket: TCustomWinSocket）在本单元用到的面。</summary>
public interface IIDSocSocketEndPoint
{
    /// <summary>`IDSocket.Socket.Connected`（:141/:380 的发包前置判定）。</summary>
    bool Connected { get; }

    /// <summary>`IDSocket.Socket.ReceiveText`（:103 `m_sSockMsg + Socket.ReceiveText`）。</summary>
    string ReceiveText { get; }

    /// <summary>`IDSocket.Socket.RemoteAddress`（:447）。</summary>
    string RemoteAddress { get; }

    /// <summary>`IDSocket.Socket.LocalPort`（:451 模块地址串）。</summary>
    int LocalPort { get; }

    /// <summary>`IDSocket.Socket.RemotePort`（:451）。</summary>
    int RemotePort { get; }

    /// <summary>`IDSocket.Socket.SendText(sSendText)`（:142/:382）。</summary>
    void SendText(string sText);

    /// <summary>`IDSocket.Socket.Close`（:325 `IDSocketError` 里强制关闭）。</summary>
    void Close();
}

/// <summary>接缝：`JSocket.pas` 的 <c>TClientSocket</c>（原文 DFM 的 `IDSocket`，ClientType=ctNonBlocking）。</summary>
public interface IIDSocClientSocket
{
    /// <summary>`IDSocket.Active`（:92 读；:96/:391/:418/:421 写）。</summary>
    bool Active { get; set; }

    /// <summary>`IDSocket.Address`（:94/:419，取自 `g_sIDServerAddr`）。</summary>
    string Address { get; set; }

    /// <summary>`IDSocket.Port`（:95/:420，取自 `g_nIDServerPort`）。</summary>
    int Port { get; set; }

    /// <summary>`IDSocket.Socket`。</summary>
    IIDSocSocketEndPoint Socket { get; }
}

/// <summary>
/// 接缝宿主：原文的 unit 级变量与 DFM 组件（DBShare.pas 全局 + uFrmMain 的 `GetSelectCharCount`）。
///
/// <para><b>默认值约定</b>：</para>
/// <list type="bullet">
///   <item><see cref="IDSocket"/> / <see cref="Timer1Enabled"/> / <see cref="KeepAliveTimerEnabled"/> /
///         <see cref="GetSelectCharCount"/> 是**宿主设施** ⇒ 默认**抛**（§25.2）。</item>
///   <item><c>g_sIDServerAddr</c> / <c>g_nIDServerPort</c> / <c>g_sServerName</c> 是**带原文初值的数据全局**
///         （DBShare.pas:128-131）⇒ 给原文初值，不抛（与 <see cref="SelectClientGlobals"/> 同一处理）。</item>
/// </list>
/// </summary>
public static class IDSocCliSeam
{
    // ---------------- 宿主设施（默认抛） ----------------

    /// <summary>DFM 的 `IDSocket: TClientSocket`（JSocket.pas 未移植）。</summary>
    public static IIDSocClientSocket? IDSocket;

    /// <summary>取 <c>IDSocket</c>；未接线直接抛（指名接入点）。</summary>
    public static IIDSocClientSocket RequireIDSocket
        => IDSocket ?? throw new NotSupportedException(
            "接缝：JSocket.pas 的 TClientSocket（IDSocCli.pas 的 IDSocket）未接线。"
            + "接入点：宿主实现 IIDSocClientSocket 并赋给 IDSocCliSeam.IDSocket（可用 GXX.GatewayKit.TcpLink 适配，"
            + "但需把 OnReceive/OnConnect/OnDisconnect/OnError 回灌到 TFrmIDSoc.IDSocketRead/Connect/Disconnect/Error）。");

    /// <summary>DFM 的 `Timer1: TTimer`（自动重连定时器）的 <c>Enabled</c>。</summary>
    public static Action<bool> Timer1Enabled = _ => throw new NotSupportedException(
        "接缝：VCL TTimer（IDSocCli.pas 的 Timer1）未接线。接入点：宿主把定时器开关赋给 IDSocCliSeam.Timer1Enabled。");

    /// <summary>DFM 的 `KeepAliveTimer: TTimer`（保活定时器）的 <c>Enabled</c>。</summary>
    public static Action<bool> KeepAliveTimerEnabled = _ => throw new NotSupportedException(
        "接缝：VCL TTimer（IDSocCli.pas 的 KeepAliveTimer）未接线。接入点：宿主把定时器开关赋给 IDSocCliSeam.KeepAliveTimerEnabled。");

    /// <summary>
    /// uFrmMain.pas:434-453 `function TFrmMain.GetSelectCharCount: Integer;`
    /// —— 在 `DBSUSETHREAD = 0`（DBShare.pas:19）下就是 `SelectSocket.Socket.ActiveConnections`，
    /// 即**当前连上来的角色网关连接数**（不是在线玩家数）。
    /// </summary>
    public static Func<int> GetSelectCharCount = () => throw new NotSupportedException(
        "接缝：uFrmMain.pas:434-453 GetSelectCharCount 未接线。接入点：宿主把 SelGate 连接数赋给 IDSocCliSeam.GetSelectCharCount。");

    // ---------------- 数据全局（带原文初值） ----------------

    /// <summary>DBShare.pas:128 `g_nIDServerPort: Integer = 5600;`（:903 可由 Dbsrc.ini 覆盖）。</summary>
    public static int g_nIDServerPort = 5600;

    /// <summary>DBShare.pas:129 `g_sIDServerAddr: string = '127.0.0.1';`（:902 可由 Dbsrc.ini 覆盖）。</summary>
    public static string g_sIDServerAddr = "127.0.0.1";

    /// <summary>DBShare.pas:131 `g_sServerName: string = 'GeeM2';`（:905 可由 Dbsrc.ini 覆盖；保活包用它）。</summary>
    public static string g_sServerName = "GeeM2";

    // ---------------- FrmIDSoc 接缝（原有） ----------------

    /// <summary>DBShare.pas 的 <c>FrmIDSoc: TFrmIDSoc</c>（`DBServer.dpr:36 Application.CreateForm` 建）。</summary>
    public static ITFrmIDSoc? FrmIDSoc;

    /// <summary>取 <c>FrmIDSoc</c>；未接线直接抛（指名接入点）。</summary>
    public static ITFrmIDSoc Require
        => FrmIDSoc ?? throw new NotSupportedException(
            "接缝：IDSocCli.pas 的 TFrmIDSoc 未接线（FrmIDSoc = nil）。接入点：DBServerService 构造 TFrmIDSoc 并赋给 IDSocCliSeam.FrmIDSoc。");

    /// <summary>
    /// ★★ **唯一的窄口子**（偏差 **D-p7-13**）**不在这里** —— 它在
    /// <c>TSelectClient.CloseUserSessionCleanup</c>（`SelectClient.cs`）。
    ///
    /// 曾经设想在这里放一个 <c>CloseUser_ShouldCloseSession</c> 谓词，**已废弃**，原因值得记下来：
    /// 把"能不能清理"写成"<c>FrmIDSoc == null || IDSocket == null</c>"是**放错了层** ——
    /// <see cref="IDSocket"/> 只有**本单元自己的** <c>TFrmIDSoc.SendSocketMsg</c> 才需要；
    /// 一个合法的 <see cref="ITFrmIDSoc"/> 替身完全可以自带发送通道而根本不用它。
    /// 在那个层判空，会把"宿主用替身"也误判成"链路不可用"（实测：它一次性打红了 2 个既有用例）。
    ///
    /// 现在的形态更精确：窄口子**只包住原文 :714-718 那一个块**，
    /// 捕获块内出现的 <c>NotSupportedException</c>（无论来自 <c>FrmIDSoc</c> 还是它内部的 socket 接缝），
    /// 记日志后跳过 —— 见 `TSelectClient.CloseUserSessionCleanup`。
    /// </summary>

    /// <summary>复位全部可注入项（单测用）。数据全局一并回到 DBShare.pas 的声明初值。</summary>
    public static void Reset()
    {
        FrmIDSoc = null;
        IDSocket = null;
        Timer1Enabled = UnwiredTimer1;
        KeepAliveTimerEnabled = UnwiredKeepAliveTimer;
        GetSelectCharCount = UnwiredGetSelectCharCount;
        g_nIDServerPort = 5600;
        g_sIDServerAddr = "127.0.0.1";
        g_sServerName = "GeeM2";
    }

    private static readonly Action<bool> UnwiredTimer1 = _ => throw new NotSupportedException(
        "接缝：VCL TTimer（IDSocCli.pas 的 Timer1）未接线。接入点：宿主把定时器开关赋给 IDSocCliSeam.Timer1Enabled。");

    private static readonly Action<bool> UnwiredKeepAliveTimer = _ => throw new NotSupportedException(
        "接缝：VCL TTimer（IDSocCli.pas 的 KeepAliveTimer）未接线。接入点：宿主把定时器开关赋给 IDSocCliSeam.KeepAliveTimerEnabled。");

    private static readonly Func<int> UnwiredGetSelectCharCount = () => throw new NotSupportedException(
        "接缝：uFrmMain.pas:434-453 GetSelectCharCount 未接线。接入点：宿主把 SelGate 连接数赋给 IDSocCliSeam.GetSelectCharCount。");
}
