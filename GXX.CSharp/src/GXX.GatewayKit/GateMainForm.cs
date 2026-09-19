using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GXX.GatewayKit;

/// <summary>
/// 三网关共用的主窗体（对应各网关 AppMain.pas/uFrmMain.pas 主窗体：启动/停止 + 状态 + 日志）。
/// </summary>
public class GateMainForm : Form
{
    private readonly IGateUiService _service;
    private readonly Button _btnStart;
    private readonly Button _btnStop;
    private readonly Label _lblStatus;
    private readonly ListBox _logList;
    private readonly System.Windows.Forms.Timer _keepAliveTimer;

    public GateMainForm(IGateUiService service)
    {
        _service = service;
        Text = service.ServiceName + " (C#)";
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
                ? $"运行中  网关 {_service.GateAddr}:{_service.GatePort}  →  {_service.ServerAddr}:{_service.ServerPort}  在线: {_service.OnlineCount}"
                : "启动失败";
        };
        _btnStop.Click += (s, e) =>
        {
            _service.StopService();
            _btnStart.Enabled = true;
            _btnStop.Enabled = false;
            _lblStatus.Text = "已停止";
        };

        _service.OnLogMsg += (msg, level) =>
        {
            try { BeginInvoke(() => _logList.Items.Add(msg)); } catch { }
        };

        Controls.AddRange(new Control[] { _btnStart, _btnStop, _lblStatus, _logList });

        _keepAliveTimer = new System.Windows.Forms.Timer { Interval = 5000 };
        _keepAliveTimer.Tick += (s, e) =>
        {
            _service.OnKeepAliveTimer();
            _lblStatus.Text = $"运行中  在线: {_service.OnlineCount}";
        };
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
