using System;
using System.Windows.Forms;
using GXX.Core;
using GXX.GatewayKit;

namespace GXX.LoginSrv;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        EncodingInit.Ensure();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // 对应 LMain.pas TFrmMain
        var service = new LoginSrvUiService(@".\AccountDB\Account.db");
        Application.Run(new GateMainForm(service));
    }
}

/// <summary>LoginSrv 的 UI 服务适配（LoginSrv 不经 GateService 基类，直接监听网关连接）。</summary>
public class LoginSrvUiService : IGateUiService
{
    private readonly LoginSrvService _srv;

    public LoginSrvUiService(string dbFile)
    {
        _srv = new LoginSrvService(dbFile);
        _srv.OnLogMsg += (m, l) => OnLogMsg?.Invoke(m, l);
        GatePort = _srv.GatePort;
    }

    public string ServiceName => "登录服务器";
    public string GateAddr => "0.0.0.0";
    public int GatePort { get; set; }
    public string ServerAddr => "-";
    public int ServerPort => 0;
    public bool ServiceStarted => _srv.ServiceStarted;
    public int OnlineCount => _srv.GateCount;

    public event Action<string, int>? OnLogMsg;

    public bool StartService() => _srv.StartService();
    public void StopService() => _srv.StopService();
    public void OnKeepAliveTimer() { }
    public void Dispose() => _srv.Dispose();
}
