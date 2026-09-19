using System.Runtime.InteropServices;

namespace GXX.Configurator;

internal static class Program
{
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool AttachConsole(int dwProcessId);
    const int ATTACH_PARENT_PROCESS = -1;

    [STAThread]
    static void Main(string[] args)
    {
        bool cli = args.Length >= 1 && (args[0] is "--build" or "-b" or "--dump" or "--save-ini" or "--help" or "-h");
        if (cli)
        {
            // WinExe 默认不带控制台；附着到父进程控制台，命令行模式才有输出
            AttachConsole(ATTACH_PARENT_PROCESS);
            RunCli(args);
            return;
        }

        ApplicationConfiguration.Initialize();

        // 崩溃时把异常写到日志，便于定位（GUI 无控制台）
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) => LogCrash(e.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, e) => LogCrash(e.ExceptionObject as Exception);
        try
        {
            Application.Run(new MainForm());
        }
        catch (Exception ex)
        {
            LogCrash(ex);
            throw;
        }
    }

    static void LogCrash(Exception ex)
    {
        try
        {
            string p = Path.Combine(AppContext.BaseDirectory, "crash.log");
            File.AppendAllText(p, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex}{Environment.NewLine}{new string('-', 60)}{Environment.NewLine}");
        }
        catch { }
    }

    static void RunCli(string[] args)
    {
        string cmd = args[0];
        if (cmd is "--help" or "-h")
        {
            Console.WriteLine();
            Console.WriteLine("GXX 登录器配置器 —— 命令行用法");
            Console.WriteLine("  图形界面：");
            Console.WriteLine("      GXX登录器配置器.exe");
            Console.WriteLine("  生成配置数据：");
            Console.WriteLine("      GXX登录器配置器.exe --build <模板ClientData.dat> <输出.dat> [settings.ini]");
            Console.WriteLine("      省略 settings.ini 时，直接沿用模板内的设置（可用于无损校验）");
            Console.WriteLine("  导出设置（可编辑后再用于 --build）：");
            Console.WriteLine("      GXX登录器配置器.exe --save-ini <ClientData.dat> <输出.ini>");
            Console.WriteLine("  导出已知字段：");
            Console.WriteLine("      GXX登录器配置器.exe --dump <ClientData.dat> <输出.txt>");
            Console.WriteLine();
            Environment.ExitCode = 0;
            return;
        }

        if (cmd == "--save-ini")
        {
            if (args.Length < 3) { Console.Error.WriteLine("用法: --save-ini <ClientData.dat> <输出.ini>"); Environment.ExitCode = 2; return; }
            try
            {
                var s = GXX.Core.Launcher.ClientDataBuilder.ReadSettings(args[1]);
                s.SaveIni(args[2]);
                Console.WriteLine($"已导出设置到 {args[2]}");
                Console.WriteLine($"  补丁文件=\"{s.GamePlanFile}\"  Resources目录=\"{s.ResourcesDir}\"  " +
                                  $"RunGate密码=\"{s.RunGatePassword}\"  版本号=\"{s.Version}\"  " +
                                  $"复选框 {s.ClientConfigs.Count(x => x)}/{s.ClientConfigs.Length} 为真");
                Environment.ExitCode = 0;
            }
            catch (Exception ex) { Console.Error.WriteLine("导出失败：" + ex.Message); Environment.ExitCode = 2; }
            return;
        }

        if (cmd == "--dump")
        {
            if (args.Length < 3) { Console.Error.WriteLine("用法: --dump <ClientData.dat> <输出.txt>"); Environment.ExitCode = 2; return; }
            try
            {
                var doc = GXX.Core.Launcher.ClientDataFile.Decode(File.ReadAllBytes(args[1]));
                File.WriteAllText(args[2], doc.DumpKnownFields(), System.Text.Encoding.UTF8);
                Console.WriteLine($"已导出已知字段到 {args[2]}");
                Console.WriteLine($"记录 {doc.Record.Length:N0} 字节，payload {doc.Payload.Length:N0} 字节");
                Environment.ExitCode = 0;
            }
            catch (Exception ex) { Console.Error.WriteLine("导出失败：" + ex.Message); Environment.ExitCode = 2; }
            return;
        }

        // --build
        if (args.Length < 3) { Console.Error.WriteLine("用法: --build <模板> <输出> [settings.ini]"); Environment.ExitCode = 2; return; }
        try
        {
            var settings = args.Length >= 4 && File.Exists(args[3])
                ? GXX.Core.Launcher.LauncherSettings.LoadIni(args[3])
                : GXX.Core.Launcher.ClientDataBuilder.ReadSettings(args[1]);

            var r = GXX.Core.Launcher.ClientDataBuilder.Build(args[1], settings, args[2]);
            foreach (var n in r.Notes) Console.WriteLine("· " + n);
            foreach (var w in r.Warnings) Console.WriteLine("⚠ " + w);
            Console.WriteLine(r.Ok ? "生成成功。" : "生成完成但有警告。");
            Environment.ExitCode = r.Ok ? 0 : 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("生成失败：" + ex.Message);
            Environment.ExitCode = 2;
        }
    }
}
