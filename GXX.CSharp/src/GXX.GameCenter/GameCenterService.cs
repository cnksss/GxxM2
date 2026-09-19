using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GXX.Core;
using GXX.Core.Util;

namespace GXX.GameCenter;

/// <summary>
/// GameCenter.pas（GMain/GShare/GHeroDB/DataBackUp/uSqliteDB）→ GameCenterService.cs
/// 引擎控制台：管理 9 个服务端进程（启动/停止/看护）+ 配置生成 + 数据备份。
/// </summary>
public class GameCenterService : IDisposable
{
    /// <summary>受管进程定义（对应 GShare.pas 的 TProgInfo 列表）。</summary>
    public class ProgInfo
    {
        public string Name = "";
        public string ExeFile = "";
        public Process? Process;
        public bool AutoStart;
        public string Args = "";
    }

    public readonly List<ProgInfo> Programs = new();
    public string BaseDir { get; set; } = AppContext.BaseDirectory;

    public event Action<string, int>? OnLogMsg;

    public GameCenterService()
    {
        // 对应 GShare 中默认的程序清单
        Programs.Add(new ProgInfo { Name = "数据库服务器", ExeFile = "DBServer.exe", AutoStart = true });
        Programs.Add(new ProgInfo { Name = "登录服务器", ExeFile = "LoginSrv.exe", AutoStart = true });
        Programs.Add(new ProgInfo { Name = "登录网关", ExeFile = "LoginGate.exe", AutoStart = true });
        Programs.Add(new ProgInfo { Name = "角色网关", ExeFile = "SelGate.exe", AutoStart = true });
        Programs.Add(new ProgInfo { Name = "游戏网关", ExeFile = "RunGate.exe", AutoStart = true });
        Programs.Add(new ProgInfo { Name = "游戏引擎", ExeFile = "M2Server.exe", AutoStart = true });
        Programs.Add(new ProgInfo { Name = "日志服务器", ExeFile = "LogDataServer.exe" });
    }

    /// <summary>启动单个程序（CreateProcess → Process.Start）。</summary>
    public bool StartProg(ProgInfo prog)
    {
        if (prog.Process != null && !prog.Process.HasExited)
            return true;
        try
        {
            string exe = Path.Combine(BaseDir, prog.ExeFile);
            var psi = new ProcessStartInfo
            {
                FileName = exe,
                WorkingDirectory = Path.GetDirectoryName(exe) ?? BaseDir,
                Arguments = prog.Args,
                UseShellExecute = true
            };
            prog.Process = Process.Start(psi);
            SendLog($"已启动 {prog.Name} ({prog.ExeFile})");
            return true;
        }
        catch (Exception ex)
        {
            SendLog($"启动 {prog.Name} 失败: {ex.Message}", 1);
            return false;
        }
    }

    /// <summary>启动全部（对应 GMain 的 StartEngine）。</summary>
    public bool StartAll()
    {
        bool ok = true;
        foreach (var p in Programs)
            if (p.AutoStart)
                ok = StartProg(p) && ok;
        return ok;
    }

    /// <summary>停止全部（对应 StopEngine）。</summary>
    public void StopAll()
    {
        foreach (var p in Programs)
        {
            StopProg(p);
        }
    }

    public void StopProg(ProgInfo prog)
    {
        try
        {
            if (prog.Process != null && !prog.Process.HasExited)
            {
                prog.Process.Kill(true);
                SendLog($"已停止 {prog.Name}");
            }
        }
        catch { }
        prog.Process = null;
    }

    /// <summary>数据备份（对应 DataBackUp.pas：角色库压缩备份）。</summary>
    public bool BackupData(string destDir)
    {
        try
        {
            Directory.CreateDirectory(destDir);
            string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            foreach (string src in Directory.GetFiles(BaseDir, "*.db", SearchOption.AllDirectories))
            {
                string rel = Path.GetRelativePath(BaseDir, src);
                string dst = Path.Combine(destDir, stamp + "_" + rel.Replace('\\', '_'));
                File.Copy(src, dst, true);
            }
            SendLog("数据备份完成 → " + destDir);
            return true;
        }
        catch (Exception ex)
        {
            SendLog("备份失败: " + ex.Message, 1);
            return false;
        }
    }

    /// <summary>生成引擎配置（对应 GMain 的配置写入）。</summary>
    public void WriteConfig(string iniFile, string serverName, string gateAddr, int loginGatePort, int selGatePort, int runGatePort)
    {
        var ini = new TFastIniFile(iniFile);
        ini.WriteString("Server", "ServerName", serverName);
        ini.WriteString("Server", "GateAddr", gateAddr);
        ini.WriteInteger("LoginGate", "GatePort", loginGatePort);
        ini.WriteInteger("SelGate", "GatePort", selGatePort);
        ini.WriteInteger("RunGate", "GatePort", runGatePort);
        ini.UpdateFile();
        SendLog("配置已写入 " + iniFile);
    }

    /// <summary>单实例检测（对应 CheckPrevious.pas）。</summary>
    public static bool IsAlreadyRunning(string mutexName)
    {
        if (_namedMutex == null)
        {
            _namedMutex = new Mutex(true, mutexName, out bool createdNew);
            return !createdNew;
        }
        return true;
    }

    private static Mutex? _namedMutex;

    private void SendLog(string msg, int level = 3) => OnLogMsg?.Invoke(msg, level);

    public void Dispose()
    {
        StopAll();
    }
}
