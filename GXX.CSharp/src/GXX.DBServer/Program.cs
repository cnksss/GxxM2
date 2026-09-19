using System;
using System.Windows.Forms;
using GXX.Core;
using GXX.GatewayKit;

namespace GXX.DBServer;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        EncodingInit.Ensure();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // 对应 uFrmMain.pas TFrmMain（数据库服务器）
        var service = new DBServerUiService(@".\RoleData\Roles.db");
        Application.Run(new GateMainForm(service));
    }
}

/// <summary>DBServer 的 UI 服务适配。</summary>
public class DBServerUiService : IGateUiService
{
    private readonly DBServerService _srv;

    public DBServerUiService(string dbFile)
    {
        _srv = new DBServerService(dbFile);
        _srv.OnLogMsg += (m, l) => OnLogMsg?.Invoke(m, l);
        GatePort = _srv.GatePort;
    }

    public string ServiceName => _srv.ServiceName;
    public string GateAddr => _srv.GateAddr;
    public int GatePort { get; set; }
    public string ServerAddr => _srv.ServerAddr;
    public int ServerPort => _srv.ServerPort;
    public bool ServiceStarted => _srv.ServiceStarted;
    public int OnlineCount => _srv.OnlineCount;

    public event Action<string, int>? OnLogMsg;

    public bool StartService() => _srv.StartService();
    public void StopService() => _srv.StopService();
    public void OnKeepAliveTimer() { }
    public void Dispose() => _srv.Dispose();
}
