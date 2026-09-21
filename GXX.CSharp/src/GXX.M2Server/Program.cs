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

    // ========================================================================
    // 宿主接线接缝（车道 p17-m2-hostwire，台账 §58.5）
    //
    // 背景：§58.5 裁定"没有任何接缝真正接上宿主 ⇒ 无端到端行为被验证"。
    // 本区两处接缝是**本车道**为"宿主可观测 / 可注入"加的最小面，遵循
    // p14-logingate-wire 的既定做法：**opt-in + 默认（null）时第一行短路，
    // 默认路径逐字节不变**（台账 §59.7 的教训 —— 不许把默认分支的语句搬进
    // opt-in 分支）。
    //
    // 与既有设施的关系（不造第三份实现，§14.2）：
    //   * 主循环节拍本来只有 `MainLoop` 的 `Thread.Sleep(10)`，原文
    //     `svMain.pas` 的 `dwProcessTime` 无对应回调 ⇒ 本接缝只是**观测点**，
    //     不复制任何计时/调度实现（计时仍走 `GXX.Core.Rtl.DelphiRTL.GetTickCount`）。
    //   * 消息派发仍完全由既有的 `OnGateClientData` 默认 `switch` 承担；本接缝
    //     只在**默认分派之前**插一个"我消费掉了"的出口，供把某条已移植的
    //     `DeCodeUserMsg`/`HandleCmds` 实现接进真宿主。
    // ========================================================================

    /// <summary>
    /// 主循环节拍接缝：每个 <see cref="MainLoop"/> 迭代开始时调用一次，参数为当轮
    /// `GetTickCount()`（uint，与原文回绕语义一致）。
    /// <para>
    /// **默认 <c>null</c> ⇒ 连 `GetTickCount()` 都不取**（第一行短路），行为与接线前逐字节一致。
    /// </para>
    /// <para>
    /// ⚠ 这是**实例**字段（不是 static）：既避免污染进程级静态全局，也免于
    /// <c>M2ConfigIsolationState</c> 的静态快照口径是否覆盖本类型的不确定性。
    /// 注入方必须在退出前置回 <c>null</c>。
    /// </para>
    /// </summary>
    public Action<uint>? MainLoopTickHook;

    /// <summary>
    /// 网关消息派发接缝（opt-in）：<see cref="OnGateClientData"/> 在
    /// **解码之后、进入默认 `CM_*` 分派之前**调用。
    /// 返回 <c>true</c> = "本帧已被消费，跳过默认分派"；返回 <c>false</c> = 落到默认分派。
    /// <para>
    /// **默认 <c>null</c> ⇒ 第一行短路，默认 `CM_*` 分派路径逐字节不变**（含既有的
    /// `data.Length &lt; 22` 提前返回与 `EDcode.DecodeMessage`，二者都**留在默认路径上**，
    /// 不得搬进本接缝分支 —— 这正是 §59.7 那次真实回归的形态）。
    /// </para>
    /// <para>参数：`sockId`（原文 `nSocket`/会话号）+ 已解码的 `TDefaultMessage`。</para>
    /// </summary>
    public Func<int, TDefaultMessage, bool>? GateMessageDispatchHook;

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
                // 接缝：默认 null ⇒ 第一行短路（连 GetTickCount 都不取）⇒ 默认路径不变。
                var tickHook = MainLoopTickHook;
                if (tickHook != null) tickHook(DelphiRTL.GetTickCount());

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

        // 接缝：默认 null ⇒ 第一行短路；返回 false 也落默认分派。
        // ⚠ 上面两行（长度门控 + 解码）**必须留在默认路径**，不得移入本分支（§59.7）。
        var dispatchHook = GateMessageDispatchHook;
        if (dispatchHook != null && dispatchHook(sockId, msg)) return;

        switch (msg.Ident)
        {
            case Grobal2Const.CM_WALK:
            case Grobal2Const.CM_RUN:
            {
                var player = FindPlayerBySocket(sockId);
                if (player != null)
                {
                    // ★ D-P17-01（车道 p17-m2-hostwire 修）：原实现读 `msg.Recog` 当方向。
                    //   `Recog` 是**对象标识**（原文 `MakeDefaultMsg(SM_USERNAME, NativeInt(Target), ...)`
                    //   把 `TBaseObject` 指针放这一格；本文件自己的**出站**帧
                    //   `Make(SM_TURN, player.m_nRecogId, dir, x, y)` 也把它当对象标识）
                    //   ⇒ 用 `Recog` 当方向 **自相矛盾**，且 `WalkTo` 里
                    //   `s_DirX[Math.Min(dir, 7)]` 会把任何越界方向**静默夹到 7**，
                    //   于是"走错方向"不报错、不抛异常（§58.5 说的"无端到端行为被验证"的典型后果）。
                    //   方向在原文取 `ProcessMsg.wParam`（`ObjPlayer.pas:17337` `ProcessMsg.wParam { dir }`）
                    //   ⇒ 线上对应 `TDefaultMessage.Param`。
                    //   ⚠ 本分支仍是**近似物**：原文 `TPlayObject.ClientWalk`
                    //   （`ObjPlayer.pas:17464`，1,100+ 行）从 `nParam1{x}`/`nParam2{y}`
                    //   反解方向并做速度控制/`CanParaly`/延时投递；那些**未移植**，
                    //   已登记为 `docs/接线工单表.md` 的未接条目。
                    byte dir = (byte)msg.Param;
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
                    // ★ D-P17-01：同上，方向取自 `Param`（原文 `ProcessMsg.wParam { dir }`，
                    //   `ObjPlayer.pas:17337`）；原实现读 `msg.Recog` 与自己的出站帧自相矛盾。
                    byte dir = (byte)msg.Param;
                    if (dir > Grobal2Const.DR_UPLEFT)
                    {
                        // 原文 `ClientChangeDir`（`ObjPlayer.pas:17269`）：
                        //   `if not(nDir in [DR_UP .. DR_UPLEFT]) then begin Result := True; Exit; end;`
                        //   ⇒ 越界方向**原样接受、原地不动**（Result := True = "已处理"）。照抄。
                        break;
                    }
                    player.m_btDirection = dir;
                    byte[] ack = EDcode.EncodeMessage(TDefaultMessage.Make(Grobal2Const.SM_TURN, player.m_nRecogId, dir, (ushort)player.m_nCurrX, (ushort)player.m_nCurrY));
                    GateMgr.SendToClient(sockId, ack);
                }
                break;
            }
            case Grobal2Const.CM_QUERYUSERNAME:
            {
                // ⚠ 未 1:1（登记在 `docs/接线工单表.md`，不假装已接线）：
                //   原文 `TPlayObject.ClientQueryUserName`（`ObjPlayer.pas:20189-20207`）读的是
                //   `nParam1`(=目标对象) / `nParam2`(=X) / `nParam3`(=Y)，并要过
                //   `CretInNearXY(Target, X, Y)` 门控，再发 `MakeDefaultMsg(SM_USERNAME,
                //   NativeInt(Target), GetCharColor(Target), 0, 0)` + `SendSocket(@Def, GetShowName(...))`。
                //   托管侧缺 `CretInNearXY` / `GetCharColor` / `GetShowName` / `SendSocket` 面
                //   ⇒ 这里只回一个 `SM_USERNAME` 空名帧，**不是**原文行为。
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
