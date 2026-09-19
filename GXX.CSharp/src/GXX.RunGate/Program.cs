using System;
using System.Windows.Forms;
using GXX.Core;
using GXX.GatewayKit;

namespace GXX.RunGate;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        EncodingInit.Ensure();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // 对应 uFrmMain.pas TFrmMain
        var service = new RunGateService();
        Application.Run(new GateMainForm(service));
    }
}
