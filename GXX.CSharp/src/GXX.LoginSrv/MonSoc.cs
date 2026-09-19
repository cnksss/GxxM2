using System;
using System.Collections.Generic;
using GXX.Core.Rtl;

namespace GXX.LoginSrv;

/// <summary>
/// MonSoc.pas TFrmMonSoc（服务器状态监视 socket，1:1）。
/// 窗体源码：MonSoc.pas:32-92；DFM：MonSoc.dfm（Left=745,Top=167,Caption='FrmMonSoc',ClientWidth=194,ClientHeight=124）。
/// 接缝：JSocket(TServerSocket) 未移植 → IMonSocket/IMonConnection。
/// </summary>
public sealed class TFrmMonSoc : System.Windows.Forms.Form
{
    // DFM: MonSocket TServerSocket Active=False Address='0.0.0.0' Port=0 ServerType=stNonBlocking Left=64 Top=48
    public IMonSocket MonSocket = new TServerSocketSeam();

    // DFM: MonTimer TTimer Interval=5000 Left=96 Top=48
    public System.Windows.Forms.Timer MonTimer = null!;

    private bool _formCreated;

    public TFrmMonSoc()
    {
        InitializeComponent();
        FormCreate(this);
    }

    private void InitializeComponent()
    {
        Text = "FrmMonSoc";                       // DFM: Caption = 'FrmMonSoc'
        StartPosition = System.Windows.Forms.FormStartPosition.Manual;
        Location = new System.Drawing.Point(745, 167);   // DFM: Left = 745  Top = 167
        ClientSize = new System.Drawing.Size(194, 124);  // DFM: ClientWidth = 194  ClientHeight = 124
        MonTimer = new System.Windows.Forms.Timer { Interval = 5000 };  // DFM: Interval = 5000
        MonTimer.Tick += (s, e) => MonTimerTimer(s);
    }

    /// <summary>MonSoc.pas:32 FormCreate（MonSocket.Active := False）。</summary>
    public void FormCreate(object? Sender)
    {
        _formCreated = true;
        MonSocket.Active = false;
    }

    /// <summary>
    /// MonSoc.pas:43 StartService。
    /// ★ 原文第 47 行即 `exit;`，其后 MonSocket 配置为**死代码**（原文如此，保留）。
    /// </summary>
    public void StartService()
    {
        if (true) return;   // 原文如此（MonSoc.pas:47 `exit;`）

        TConfig Config = LoginSrvShare.g_Config;
        MonSocket.Active = false;
        MonSocket.Address = Config.sMonAddr;
        MonSocket.Port = Config.nMonPort;
        MonSocket.Active = true;
    }

    /// <summary>MonSoc.pas:55 MonTimerTimer。</summary>
    public void MonTimerTimer(object? Sender)
    {
        string sMsg = "";
        int nC = LoginSrvShare.FrmMasSoc.m_ServerList.Count;
        for (int I = 0; I <= LoginSrvShare.FrmMasSoc.m_ServerList.Count - 1; I++)
        {
            TMsgServerInfo MsgServer = LoginSrvShare.FrmMasSoc.m_ServerList[I];
            string sServerName = MsgServer.sServerName;
            if (!string.Equals(sServerName, ""))
            {
                sMsg = sMsg + sServerName + "/" + DelphiRTL.IntToStr(MsgServer.nServerIndex) + "/"
                     + DelphiRTL.IntToStr(MsgServer.nOnlineCount) + "/";
                // Delphi: (GetTickCount - dwKeepAliveTick) < 30000 —— Cardinal 回绕减法，1:1
                if (unchecked(DelphiRTL.GetTickCount() - MsgServer.dwKeepAliveTick) < 30000)
                    sMsg = sMsg + "正常 ;";
                else
                    sMsg = sMsg + "超时 ;";
            }
            else
            {
                sMsg = "-/-/-/-;";
            }
        }
        for (int I = 0; I <= MonSocket.ActiveConnections - 1; I++)
        {
            MonSocket.Connections(I).SendText(DelphiRTL.IntToStr(nC) + ";" + sMsg);
        }
    }

    /// <summary>MonSoc.pas:86 MonSocketClientError（ErrorCode := 0; Socket.Close）。</summary>
    public void MonSocketClientError(object? Sender, IMonConnection Socket, ref int ErrorCode)
    {
        ErrorCode = 0;
        Socket.Close();
    }

    /// <summary>测试/宿主可读：FormCreate 是否已执行。</summary>
    public bool FormCreated => _formCreated;
}

/// <summary>接缝：JSocket.TServerSocket 最小面（MonSoc.pas 依赖）。</summary>
public interface IMonSocket
{
    bool Active { get; set; }
    string Address { get; set; }
    int Port { get; set; }
    int ActiveConnections { get; }
    IMonConnection Connections(int index);
}

/// <summary>接缝：TCustomWinSocket 最小面。</summary>
public interface IMonConnection
{
    void SendText(string text);
    void Close();
    bool Closed { get; }
}

/// <summary>接缝默认实现（无真实监听；待 JSocket 移植后接入）。</summary>
public sealed class TServerSocketSeam : IMonSocket
{
    private readonly List<IMonConnection> _connections = new();

    public bool Active { get; set; } = false;   // DFM: MonSocket.Active = False
    public string Address { get; set; } = "0.0.0.0";
    public int Port { get; set; } = 0;
    public int ActiveConnections => _connections.Count;
    public IMonConnection Connections(int index) => _connections[index];

    public List<IMonConnection> ConnectionList => _connections;

    private sealed class Conn : IMonConnection
    {
        public readonly List<string> Sent = new();
        public bool Closed { get; private set; }
        public void SendText(string text) => Sent.Add(text);
        public void Close() => Closed = true;
    }

    /// <summary>测试/宿主注入一个已连接客户端。</summary>
    public IMonConnection AddConnection()
    {
        var c = new Conn();
        _connections.Add(c);
        return c;
    }

    public IReadOnlyList<string> SentOf(int index) => ((Conn)_connections[index]).Sent;
}
