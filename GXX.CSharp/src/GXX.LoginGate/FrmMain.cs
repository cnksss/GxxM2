using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using GXX.Core;

namespace GXX.LoginGate;

/// <summary>
/// AppMain.pas TFormMain → FrmMain：登录网关主窗体（启动/停止 + 状态显示 + 日志）。
/// 布局对应原 DFM：顶部按钮区 + 中部网关状态 ListView + 底部日志 Memo。
/// </summary>
public class FrmMain : Form
{
    private readonly LoginGateService _service;
    private readonly Button _btnStart;
    private readonly Button _btnStop;
    private readonly Label _lblStatus;
    private readonly ListBox _logList;
    private readonly System.Windows.Forms.Timer _keepAliveTimer;

    public FrmMain(LoginGateService service)
    {
        _service = service;
        Text = "登录网关 (C#)";
        Size = new Size(640, 480);
        StartPosition = FormStartPosition.CenterScreen;

        _btnStart = new Button { Text = "启动服务", Left = 12, Top = 12, Width = 90 };
        _btnStop = new Button { Text = "停止服务", Left = 110, Top = 12, Width = 90, Enabled = false };
        _lblStatus = new Label { Text = "已停止", Left = 210, Top = 18, Width = 380 };

        _logList = new ListBox
        {
            Left = 12,
            Top = 48,
            Width = 600,
            Height = 380,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
        };

        _btnStart.Click += async (s, e) =>
        {
            _btnStart.Enabled = false;
            bool ok = await Task.Run(_service.StartService);
            _btnStop.Enabled = ok;
            _lblStatus.Text = ok
                ? $"运行中  网关 {_service.GateAddr}:{_service.GatePort}  →  {_service.ServerAddr}:{_service.ServerPort}"
                : "启动失败";
        };
        _btnStop.Click += (s, e) =>
        {
            _service.StopService();
            _btnStart.Enabled = true;
            _btnStop.Enabled = false;
            _lblStatus.Text = "已停止";
        };

        _service.OnLogMsg += (msg, level) => BeginInvoke(() => _logList.Items.Add(msg));

        Controls.AddRange(new Control[] { _btnStart, _btnStop, _lblStatus, _logList });

        _keepAliveTimer = new System.Windows.Forms.Timer { Interval = 5000 };
        _keepAliveTimer.Tick += (s, e) => _service.SendKeepAlive();
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        _keepAliveTimer.Start();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _keepAliveTimer.Stop();
        _service.Dispose();
        base.OnFormClosed(e);
    }
}
