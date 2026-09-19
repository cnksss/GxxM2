using System;
using System.Windows.Forms;
using GXX.Core;
using GXX.GatewayKit;

namespace GXX.SelGate;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        EncodingInit.Ensure();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // 对应 SelGate AppMain.pas TFrmMain
        var service = new SelGateService();
        Application.Run(new GateMainForm(service));
    }
}
