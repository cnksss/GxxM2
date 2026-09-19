using System;
using System.Drawing;
using System.Windows.Forms;
using GXX.Core;

namespace GXX.GameCenter;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        EncodingInit.Ensure();
        // 对应 CheckPrevious.RestoreIfRunning
        if (GameCenterService.IsAlreadyRunning("GXX_GameCenter_Mutex"))
            return;

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new FrmMain());
    }
}

/// <summary>GMain.pas TfrmMain：引擎控制台主窗体（进程管理 + 日志）。</summary>
public class FrmMain : Form
{
    private readonly GameCenterService _service = new();
    private readonly ListView _lv;
    private readonly ListBox _log;
    private readonly Button _btnStartAll;
    private readonly Button _btnStopAll;
    private readonly Button _btnBackup;

    public FrmMain()
    {
        Text = "引擎控制台 (C#)";
        Size = new Size(720, 520);
        StartPosition = FormStartPosition.CenterScreen;

        _btnStartAll = new Button { Text = "启动引擎", Left = 12, Top = 12, Width = 90 };
        _btnStopAll = new Button { Text = "停止引擎", Left = 110, Top = 12, Width = 90 };
        _btnBackup = new Button { Text = "备份数据", Left = 208, Top = 12, Width = 90 };

        _lv = new ListView
        {
            Left = 12, Top = 48, Width = 680, Height = 200,
            View = View.Details,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        _lv.Columns.Add("程序", 160);
        _lv.Columns.Add("文件", 220);
        _lv.Columns.Add("状态", 120);
        foreach (var p in _service.Programs)
        {
            var item = new ListViewItem(p.Name);
            item.SubItems.Add(p.ExeFile);
            item.SubItems.Add("未启动");
            _lv.Items.Add(item);
        }

        _log = new ListBox
        {
            Left = 12, Top = 256, Width = 680, Height = 210,
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
        };

        _service.OnLogMsg += (m, l) =>
        {
            try { BeginInvoke(() => _log.Items.Add(m)); } catch { }
        };

        _btnStartAll.Click += (s, e) =>
        {
            _service.StartAll();
            foreach (ListViewItem it in _lv.Items) it.SubItems[2].Text = "启动中";
        };
        _btnStopAll.Click += (s, e) =>
        {
            _service.StopAll();
            foreach (ListViewItem it in _lv.Items) it.SubItems[2].Text = "未启动";
        };
        _btnBackup.Click += (s, e) => _service.BackupData(@".\Backup");

        Controls.AddRange(new Control[] { _btnStartAll, _btnStopAll, _btnBackup, _lv, _log });
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _service.Dispose();
        base.OnFormClosed(e);
    }
}
