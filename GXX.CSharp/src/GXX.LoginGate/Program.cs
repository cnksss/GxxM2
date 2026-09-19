using System;
using System.Windows.Forms;
using GXX.Core;

namespace GXX.LoginGate;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        EncodingInit.Ensure();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // 对应 AppMain.pas TFormMain：主窗体承载网关服务
        var service = new LoginGateService();
        var mainForm = new FrmMain(service);
        Application.Run(mainForm);
    }
}
