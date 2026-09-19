using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GXX.Core;
using GXX.Core.Rtl;

namespace GXX.LogDataServer;

/// <summary>
/// LogDataServer.pas（LogDataMain/LDShare/LogManage/FileSearchPool）→ LogDataService.cs
/// 日志服务器：接受各程序 TCP 日志提交，按 [日期/来源] 写入文本文件；支持目录检索查询。
/// </summary>
public class LogDataService : IDisposable
{
    private Socket? _listener;
    private Thread? _acceptThread;
    private volatile bool _running;
    private int _logCount;

    public int Port { get; set; } = 10000;
    public string LogDir { get; set; } = @".\Log";
    public bool ServiceStarted => _running;
    public int LogCount => _logCount;

    public event Action<string, int>? OnLogMsg;

    public bool StartService()
    {
        if (_running) return true;
        try
        {
            Directory.CreateDirectory(LogDir);
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(new IPEndPoint(IPAddress.Any, Port));
            _listener.Listen(16);
            _running = true;
            _acceptThread = new Thread(AcceptLoop) { IsBackground = true, Name = "LogServer" };
            _acceptThread.Start();
            SendLog($"日志服务器启动，端口 {Port}，目录 {Path.GetFullPath(LogDir)}");
            return true;
        }
        catch (Exception ex)
        {
            SendLog("启动失败: " + ex.Message, 1);
            return false;
        }
    }

    public void StopService()
    {
        _running = false;
        try { _listener?.Close(); } catch { }
    }

    private void AcceptLoop()
    {
        var buf = new byte[64 * 1024];
        while (_running)
        {
            try
            {
                var client = _listener!.Accept();
                // 日志为短连接：一次接收后按 [来源]\t[文本] 写入
                int n = 0;
                try
                {
                    client.ReceiveTimeout = 3000;
                    n = client.Receive(buf);
                }
                catch { }
                client.Close();
                if (n <= 0) continue;
                string line = EncodingInit.GBK.GetString(buf, 0, n);
                WriteLog(line);
            }
            catch { if (!_running) return; }
        }
    }

    /// <summary>写入一条日志（对应原 LogDataMain 的按日期归档）。</summary>
    public void WriteLog(string line)
    {
        string source = "General";
        string text = line;
        int tab = line.IndexOf('\t');
        if (tab > 0)
        {
            source = line.Substring(0, tab);
            text = line.Substring(tab + 1);
        }
        string file = Path.Combine(LogDir, DateTime.Now.ToString("yyyy-MM-dd") + "_" + SanFile(source) + ".log");
        File.AppendAllText(file, DateTime.Now.ToString("HH:mm:ss ") + text + "\r\n", EncodingInit.GBK);
        Interlocked.Increment(ref _logCount);
    }

    /// <summary>检索日志（对应 uFrmRemoteQuerySetting 远程查询）。</summary>
    public int Search(string keyword, DateTime date, System.Text.StringBuilder resultSink)
    {
        string file = Path.Combine(LogDir, date.ToString("yyyy-MM-dd") + "_General.log");
        if (!File.Exists(file)) return 0;
        int count = 0;
        foreach (string ln in File.ReadAllLines(file, EncodingInit.GBK))
        {
            if (ln.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                resultSink.AppendLine(ln);
                count++;
            }
        }
        return count;
    }

    private static string SanFile(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name;
    }

    private void SendLog(string msg, int level = 3) => OnLogMsg?.Invoke(msg, level);

    public void Dispose() => StopService();
}
