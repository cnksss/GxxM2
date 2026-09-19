using GXX.Core;
using GXX.Core.Protocol;
using GXX.Client.HGE;

namespace GXX.Client;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        EncodingInit.Ensure();
        // 对应 Client.dpr：初始化 HGE → 显示主窗体（ClMain TfrmMain）
        System.Windows.Forms.Application.EnableVisualStyles();
        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
        System.Windows.Forms.Application.Run(new FrmMain());
    }
}
