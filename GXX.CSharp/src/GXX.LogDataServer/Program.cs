using System;
using System.Windows.Forms;
using GXX.Core;
using GXX.GatewayKit;

namespace GXX.LogDataServer;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        EncodingInit.Ensure();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // 对应 LogDataMain.pas TFrmLogData
        var service = new LogDataUiService();
        Application.Run(new GateMainForm(service));
    }
}

/// <summary>LogDataServer 的 UI 适配。</summary>
public class LogDataUiService : GXX.GatewayKit.IGateUiService
{
    private readonly LogDataService _srv;

    public LogDataUiService()
    {
        _srv = new LogDataService();
        _srv.OnLogMsg += (m, l) => OnLogMsg?.Invoke(m, l);
    }

    public string ServiceName => "日志服务器";
    public string GateAddr => "0.0.0.0";
    public int GatePort { get => _srv.Port; set => _srv.Port = value; }
    public string ServerAddr => "-";
    public int ServerPort => 0;
    public bool ServiceStarted => _srv.ServiceStarted;
    public int OnlineCount => _srv.LogCount;

    public event Action<string, int>? OnLogMsg;

    public bool StartService() => _srv.StartService();
    public void StopService() => _srv.StopService();
    public void OnKeepAliveTimer() { }
    public void Dispose() => _srv.Dispose();
}
