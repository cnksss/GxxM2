using System;
using System.Threading;
using System.Windows.Forms;
using GXX.Core;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.M2Server.Engine;

namespace GXX.M2Server;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        EncodingInit.Ensure();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // 对应 svMain.pas TfrmMain：主窗体承载引擎
        var engine = new M2EngineService();
        Application.Run(new FrmMain(engine));
    }
}

/// <summary>
/// M2Server 引擎服务（svMain.pas + UserEngine 主循环 + RunSock 网关管理）。
/// </summary>
public class M2EngineService : IDisposable
{
    public readonly TUserEngine UserEngine = new();
    public readonly GateManager GateMgr = new();

    private Thread? _mainLoopThread;
    private volatile bool _running;

    public string MapDir { get; set; } = @".\Map";
    public int GatePort { get; set; } = 5600;
    public bool ServiceStarted => _running;
    public int OnlineCount => UserEngine.PlayObjectCount;
    public string ServerName => "GXX";
    public string ServerAddr => "0.0.0.0";
    public int ServerPort => GatePort;

    public event Action<string, int>? OnLogMsg;

    public bool StartService()
    {
        if (_running) return true;
        GateMgr.Port = GatePort;
        GateMgr.OnLogMsg += (m, l) => OnLogMsg?.Invoke(m, l);
        GateMgr.OnClientData += OnGateClientData;

        if (!UserEngine.LoadMaps(MapDir))
            OnLogMsg?.Invoke($"地图目录 {MapDir} 未加载到地图（空引擎启动）", 2);

        if (!GateMgr.Start())
            return false;

        _running = true;
        _mainLoopThread = new Thread(MainLoop) { IsBackground = true, Name = "M2-MainLoop" };
        _mainLoopThread.Start();
        OnLogMsg?.Invoke($"游戏引擎启动完成（地图 {UserEngine.MapCount} 张）", 3);
        return true;
    }

    public void StopService()
    {
        _running = false;
        try { _mainLoopThread?.Join(1000); } catch { }
        GateMgr.Stop();
        UserEngine.Dispose();
        OnLogMsg?.Invoke("游戏引擎已停止", 3);
    }

    private void MainLoop()
    {
        while (_running)
        {
            try
            {
                UserEngine.Process();
            }
            catch (Exception ex)
            {
                OnLogMsg?.Invoke("主循环异常: " + ex.Message, 1);
            }
            Thread.Sleep(10); // ~100 FPS 主循环节拍（对应原 dwProcessTime）
        }
    }

    /// <summary>网关客户端数据 → CM_* 消息处理（对应 HandleCmds 入口）。</summary>
    private void OnGateClientData(int sockId, byte[] data)
    {
        if (data.Length < 22) return;
        TDefaultMessage msg = EDcode.DecodeMessage(data);
        switch (msg.Ident)
        {
            case Grobal2Const.CM_WALK:
            case Grobal2Const.CM_RUN:
            {
                var player = FindPlayerBySocket(sockId);
                if (player != null)
                {
                    byte dir = (byte)msg.Recog;
                    if (player.WalkTo(dir))
                    {
                        // 广播 RM_WALK（此处简化为回执本人）
                        byte[] ack = EDcode.EncodeMessage(TDefaultMessage.Make(Grobal2Const.SM_WALK, player.m_nRecogId, dir, (ushort)player.m_nCurrX, (ushort)player.m_nCurrY));
                        GateMgr.SendToClient(sockId, ack);
                    }
                    else
                    {
                        byte[] fail = EDcode.EncodeMessage(TDefaultMessage.Make(Grobal2Const.SM_MOVEFAIL, player.m_nRecogId, 0, 0, 0));
                        GateMgr.SendToClient(sockId, fail);
                    }
                }
                break;
            }
            case Grobal2Const.CM_TURN:
            {
                var player = FindPlayerBySocket(sockId);
                if (player != null)
                {
                    player.m_btDirection = (byte)msg.Recog;
                    byte[] ack = EDcode.EncodeMessage(TDefaultMessage.Make(Grobal2Const.SM_TURN, player.m_nRecogId, (byte)msg.Recog, (ushort)player.m_nCurrX, (ushort)player.m_nCurrY));
                    GateMgr.SendToClient(sockId, ack);
                }
                break;
            }
            case Grobal2Const.CM_QUERYUSERNAME:
            {
                byte[] ack = EDcode.EncodeMessage(TDefaultMessage.Make(Grobal2Const.SM_USERNAME, msg.Recog, 0, 0, 0));
                GateMgr.SendToClient(sockId, ack);
                break;
            }
        }
    }

    private TPlayObject? FindPlayerBySocket(int sockId)
    {
        // 本实现 sockId 即 m_nSocket
        foreach (var p in UserEngine.PlayObjects)
            if (p.m_nSocket == sockId) return p;
        return null;
    }

    public void Dispose()
    {
        StopService();
    }
}

/// <summary>svMain.pas TfrmMain：引擎主窗体（启动/停止 + 状态日志）。</summary>
public class FrmMain : Form
{
    private readonly M2EngineService _engine;
    private readonly Button _btnStart;
    private readonly Button _btnStop;
    private readonly Label _lblStatus;
    private readonly ListBox _log;
    private readonly System.Windows.Forms.Timer _statTimer;

    public FrmMain(M2EngineService engine)
    {
        _engine = engine;
        Text = "M2Server 游戏引擎 (C#)";
        Size = new System.Drawing.Size(700, 500);
        StartPosition = FormStartPosition.CenterScreen;

        _btnStart = new Button { Text = "启动引擎", Left = 12, Top = 12, Width = 90 };
        _btnStop = new Button { Text = "停止引擎", Left = 110, Top = 12, Width = 90, Enabled = false };
        _lblStatus = new Label { Text = "已停止", Left = 210, Top = 18, Width = 440 };
        _log = new ListBox
        {
            Left = 12, Top = 48, Width = 660, Height = 390,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
        };

        _btnStart.Click += (s, e) =>
        {
            if (_engine.StartService())
            {
                _btnStart.Enabled = false;
                _btnStop.Enabled = true;
                _lblStatus.Text = $"运行中  网关端口 {_engine.GatePort}  地图 {_engine.UserEngine.MapCount} 张";
            }
            else
            {
                _lblStatus.Text = "启动失败";
            }
        };
        _btnStop.Click += (s, e) =>
        {
            _engine.StopService();
            _btnStart.Enabled = true;
            _btnStop.Enabled = false;
            _lblStatus.Text = "已停止";
        };
        _engine.OnLogMsg += (m, l) =>
        {
            try { BeginInvoke(() => _log.Items.Add(m)); } catch { }
        };

        Controls.AddRange(new Control[] { _btnStart, _btnStop, _lblStatus, _log });

        _statTimer = new System.Windows.Forms.Timer { Interval = 2000 };
        _statTimer.Tick += (s, e) =>
        {
            if (_engine.ServiceStarted)
                _lblStatus.Text = $"运行中  在线 {_engine.OnlineCount}  怪物 {_engine.UserEngine.MonsterCount}  地图 {_engine.UserEngine.MapCount}";
        };
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        _statTimer.Start();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _statTimer.Stop();
        _engine.Dispose();
        base.OnFormClosed(e);
    }
}
