// 源单元：Source/Client-HGE/LogHelper.pas（原文 183 行 / CRLF 计入 217 行）
// 原文 uses：Windows, Classes, SysUtils, StrUtils
// 原文无同名 .dfm（纯日志工具，无窗体）。
//
// 移植策略（任务书：网络/线程/GDI 不写进单测，把纯逻辑抽成可测函数）：
//   · 纯逻辑 1:1 移植（可单测）：
//       - WriteToLogFile 的**级别过滤**（原文 :65-70）
//       - TLogFile.WriteLog 的**时间戳格式**与**行格式**（原文 :134-137 / :152-155）
//       - GetUniqueMutexName 的 **BinToHex**（原文 :204-208）
//       - 互斥体名常量前缀 'Global\%s'（原文 :172）
//   · 线程/文件 I/O 收敛为接缝：
//       原文在 Create 时 BeginThread 起一个线程，用 QueueUserAPC 把 APC 排队，
//       APC 里 CreateMutex + WaitForSingleObject(5s) + Append/ReWrite 写文件。
//       托管侧改为「后台写线程 + 阻塞队列」，**语义等价**（异步、串行、按序、
//       文件不存在则新建目录），但不再用 Win32 APC/互斥体（.NET 无 APC 概念，
//       且进程内单写线程天然串行，无需跨进程互斥体）。
//   · initialization/finalization 段的全局 g_LogFile → 显式静态单例 + 惰性初始化，
//     并把原文的 ParamStr(0) 目录改为可注入（原文 :211 是
//     ExtractFilePath(ParamStr(0)) + 'MirUI.log'）。
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;

namespace GXX.Client.Tail;

/// <summary>原文 :47-53 的日志级别常量 + :48 的全局阈值。</summary>
public static class LogLevels
{
    /// <summary>原文 :48 — <c>GLOBAL_LOG_LEVEL = 0;</c>（默认放行全部级别）</summary>
    public static int GLOBAL_LOG_LEVEL = 0;

    /// <summary>原文 :50 — <c>LOG_LEVEL_0 = 0;</c></summary>
    public const int LOG_LEVEL_0 = 0;
    /// <summary>原文 :51 — <c>LOG_LEVEL_1 = 1;</c></summary>
    public const int LOG_LEVEL_1 = 1;
    /// <summary>原文 :52 — <c>LOG_LEVEL_2 = 2;</c></summary>
    public const int LOG_LEVEL_2 = 2;
    /// <summary>原文 :53 — <c>LOG_LEVEL_3 = 3;</c></summary>
    public const int LOG_LEVEL_3 = 3;
}

/// <summary>原文 :30 — <c>TLogFile.TLogMode = (logAppend, logNew);</c></summary>
public enum TLogMode
{
    /// <summary>原文 :30 — <c>logAppend</c>（追加；Create 不截断）</summary>
    logAppend = 0,
    /// <summary>原文 :30 — <c>logNew</c>（Create 时 ReWrite 清空）</summary>
    logNew = 1,
}

/// <summary>原文 :20-25 — <c>TApcParam</c>（原为 APC 参数记录，托管侧改为队列元素）。</summary>
public sealed class TApcParam
{
    /// <summary>原文 :21 — <c>sLogFile:string;</c></summary>
    public string sLogFile;
    /// <summary>原文 :22 — <c>sLog:string;</c></summary>
    public string sLog;
    /// <summary>原文 :23 — <c>sMutexName:string;</c></summary>
    public string sMutexName;
}

/// <summary>
/// LogHelper.pas 的 <c>TLogFile</c> 1:1 移植（线程/文件部分为接缝）。
///
/// <para>可用性：<c>WriteLog</c> 只入队（与原文 QueueUserAPC 的异步语义一致），
/// 后台线程串行落盘；<c>Dispose</c> 会等待队列排空后收尾（对应原文
/// <c>Destroy</c> 的 <c>SetEvent + WaitForSingleObject(m_hThread, INFINITE)</c>）。
/// 测试若要立刻读到文件，用 <see cref="DrainForTest"/>。</para>
/// </summary>
public class TLogFile : IDisposable
{
    // 原文 :32-36 的私有字段
    private readonly string m_sLogFile;
    private readonly string m_sMutexName;

    private readonly BlockingCollection<TApcParam> m_queue =
        new BlockingCollection<TApcParam>(new ConcurrentQueue<TApcParam>());

    private readonly Thread m_thread;

    private volatile bool m_disposed;

    /// <summary>
    /// 原文 :163-193 <c>constructor TLogFile.Create(sLogFile: string; LogMode: TLogMode);</c>
    /// </summary>
    public TLogFile(string sLogFile, TLogMode LogMode = TLogMode.logAppend)
    {
        // 原文如此（LogHelper.pas:169-172）：
        //   sMutexName := LowerCase(sLogFile);
        //   //sMutexName := System.Hash.THashMD5.GetHashString(sMutexName);
        //   sMutexName := GetUniqueMutexName(sLogFile);
        //   m_sMutexName := Format('Global\%s', [sMutexName]);
        m_sLogFile = sLogFile;
        m_sMutexName = string.Format("Global\\{0}", GetUniqueMutexName(sLogFile));

        // 原文 :176-178：if DirectoryExists(ExtractFileDir(sLogFile)) then ForceDirectories(...)
        // 原文如此（LogHelper.pas:176）：条件写反了 —— DirectoryExists 为真时才去
        // ForceDirectories（而 ForceDirectories 对已存在目录是 no-op）。本移植按
        // **能落盘**这一实际意图实现（目录不存在则创建），差异已登记。
        try
        {
            string dir = Path.GetDirectoryName(Path.GetFullPath(sLogFile));
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        catch (Exception)
        {
            // 原文 :186-188 的 except 分支只打 OutputDebugString('Clean File Failed')，不抛出。
        }

        try
        {
            if (LogMode == TLogMode.logNew)
            {
                // 原文 :181-185：AssignFile/ReWrite/CloseFile（清空或新建）
                using var _ = new FileStream(sLogFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            }
        }
        catch (Exception)
        {
            // 原文如此（LogHelper.pas:186-188）：except OutputDebugString('Clean File Failed');
        }

        m_thread = new Thread(LogFileProc) { IsBackground = true, Name = "MirUI-LogFile" };
        m_thread.Start();
    }

    /// <summary>原文 <c>m_sLogFile</c>（测试可见的只读视图）</summary>
    public string LogFile => m_sLogFile;

    /// <summary>原文 <c>m_sMutexName</c>（测试可见的只读视图）</summary>
    public string MutexName => m_sMutexName;

    /// <summary>
    /// 原文 :108-124 + :73-106 —— 后台写线程（原为 APC 回调）。
    /// 阻塞取队列 → 逐条落盘；<c>CompleteAdding</c> 后退出。
    /// </summary>
    private void LogFileProc()
    {
        try
        {
            foreach (var pApc in m_queue.GetConsumingEnumerable())
            {
                if (pApc == null) continue;
                ApcProc(pApc);
            }
        }
        catch (ObjectDisposedException)
        {
            // 收尾竞态，忽略
        }
        catch (InvalidOperationException)
        {
            // CompleteAdding 之后的 GetConsumingEnumerable 退出路径
        }
    }

    /// <summary>
    /// 原文 :73-106 <c>ApcProc(param:Pointer)</c>：把一行追加/新建到日志文件。
    /// <para>原文用跨进程命名互斥体 + 5 秒等待来串行化多进程写同一文件；
    /// 托管侧单写线程已串行，故省略互斥体（语义等价，差异已登记）。</para>
    /// </summary>
    private void ApcProc(TApcParam pApc)
    {
        try
        {
            if (File.Exists(pApc.sLogFile))
            {
                // 原文 :86-90：Append
                using var fs = new FileStream(pApc.sLogFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                WriteLine(fs, pApc.sLog);
            }
            else
            {
                // 原文 :91-99
                string dir = Path.GetDirectoryName(Path.GetFullPath(pApc.sLogFile));
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                using var fs = new FileStream(pApc.sLogFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                WriteLine(fs, pApc.sLog);
            }
        }
        catch (Exception)
        {
            // 原文 ApcProc 无 except，写失败会直接崩线程；托管侧吞掉（差异已登记）。
        }
    }

    /// <summary>原文 <c>WriteLn(fText, sLog)</c> 的字节语义（GBK + CRLF）。</summary>
    private static void WriteLine(FileStream fs, string s)
    {
        byte[] body = GXX.Core.EncodingInit.GBK.GetBytes(s ?? string.Empty);
        fs.Write(body, 0, body.Length);
        fs.WriteByte(0x0D);
        fs.WriteByte(0x0A);
    }

    /// <summary>
    /// 原文 :128-143 <c>procedure TLogFile.WriteLog(sLog: string);</c>
    /// <para><c>if sLog &lt;&gt; EmptyStr then</c> 才写（空串被丢弃）；格式
    /// <c>Format('[%s] %s', [wstrTime, sLog])</c>，时间戳
    /// <c>FormatDateTime('yyyy-mm-dd hh:mm:ss', now())</c>。</para>
    /// </summary>
    public void WriteLog(string sLog) => WriteLog(sLog, null);

    /// <summary>
    /// 原文 :145-161 <c>procedure TLogFile.WriteLog(sLog: string; Args: array of const);</c>
    /// <para>先 <c>sLog := Format(sLog, Args)</c> 再套时间戳。托管侧用
    /// <c>string.Format</c>；Delphi 与 .NET 的格式串差异（<c>%s</c>/<c>%d</c> → <c>{0}</c>）
    /// 由调用方承担，本方法不做自动改写 —— 差异已登记。</para>
    /// </summary>
    public void WriteLog(string sLog, object[] Args)
    {
        if (m_disposed) return;

        // 原文如此（LogHelper.pas:133 与 :150）：if sLog <> EmptyStr then
        if (sLog == string.Empty || sLog == null) return;

        string formatted = sLog;
        if (Args != null && Args.Length > 0)
        {
            formatted = string.Format(sLog, Args);
        }

        var pApc = new TApcParam
        {
            sLogFile = m_sLogFile,
            sLog = FormatLogLine(formatted, DateTime.Now),
            sMutexName = m_sMutexName,
        };

        try
        {
            m_queue.Add(pApc);
        }
        catch (InvalidOperationException)
        {
            // 原文 :139-141：if not QueueUserAPC(...) then Dispose(pApc); —— 池已关闭即丢弃
        }
    }

    /// <summary>原文 :137 / :155 — <c>Format('[%s] %s', [wstrTime, sLog])</c>（纯逻辑，供测试）</summary>
    public static string FormatLogLine(string sLog, DateTime now)
        => string.Format("[{0}] {1}", FormatTimestamp(now), sLog);

    /// <summary>原文 :134 / :152 — <c>FormatDateTime('yyyy-mm-dd hh:mm:ss', now())</c>（纯逻辑，供测试）</summary>
    public static string FormatTimestamp(DateTime now)
        => now.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>
    /// 原文 :204-208 <c>function TLogFile.GetUniqueMutexName(sName: string): string;</c>
    /// <para><c>SetLength(Result, Length(sName) * 2); BinToHex(PChar(sName), @Result[1], Length(sName));</c>
    /// —— 取 **GBK 字节**的十六进制小写串，长度 = <c>Length(sName) * 2</c>（注意
    /// Delphi 的 <c>Length(AnsiString)</c> 是**字节数**，所以中文名字符数不等于这里的"Length"）。
    /// 本移植按 GBK 字节实现。</para>
    /// </summary>
    public static string GetUniqueMutexName(string sName)
    {
        byte[] bytes = GXX.Core.EncodingInit.GBK.GetBytes(sName ?? string.Empty);
        var sb = new StringBuilder(bytes.Length * 2);
        foreach (byte b in bytes)
        {
            sb.Append(b.ToString("x2"));   // Delphi Classes.BinToHex 输出小写
        }
        return sb.ToString();
    }

    /// <summary>原文 :65-70 <c>WriteToLogFile(sLog, nLevel)</c> 的级别过滤（纯逻辑，供测试）。</summary>
    public static bool ShouldWrite(int nLevel) => nLevel >= LogLevels.GLOBAL_LOG_LEVEL;

    /// <summary>
    /// 原文 :65-70 <c>procedure WriteToLogFile(sLog:string; nLevel:Integer);</c>
    /// —— <c>if nLevel &gt;= GLOBAL_LOG_LEVEL then g_LogFile.WriteLog(sLog);</c>
    /// </summary>
    public static void WriteToLogFile(string sLog, int nLevel)
    {
        if (ShouldWrite(nLevel))
        {
            GlobalLogFile.WriteLog(sLog);
        }
    }

    /// <summary>原文 :58-59 — <c>var g_LogFile:TLogFile;</c>（模块级单例）</summary>
    public static TLogFile GlobalLogFile => GlobalLogFileHolder.Instance;

    /// <summary>
    /// 原文 :210-214 的 initialization/finalization：
    /// <code>
    /// initialization
    ///   g_LogFile := TLogFile.Create(ExtractFilePath(ParamStr(0)) + 'MirUI.log');
    /// finalization
    ///   g_LogFile.Free;
    /// </code>
    /// <para>托管侧改为惰性单例 + 可注入路径（<see cref="GlobalLogFileHolder.ResetForTest"/>），
    /// 因为 .NET 没有"单元初始化"时机。</para>
    /// </summary>
    public static class GlobalLogFileHolder
    {
        private static TLogFile _instance;
        private static readonly object _lock = new object();
        private static string _pathOverride;

        /// <summary>原文 <c>ExtractFilePath(ParamStr(0)) + 'MirUI.log'</c></summary>
        public static string DefaultPath =>
            Path.Combine(AppContext.BaseDirectory, "MirUI.log");

        public static TLogFile Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new TLogFile(_pathOverride ?? DefaultPath);
                    return _instance;
                }
            }
        }

        /// <summary>测试接缝：重置单例（原文无此操作，仅用于隔离）。</summary>
        public static void ResetForTest(string path = null)
        {
            lock (_lock)
            {
                _instance?.Dispose();
                _instance = null;
                _pathOverride = path;
            }
        }
    }

    /// <summary>
    /// 原文 <c>destructor TLogFile.Destroy;</c>（:195-202）：
    /// <c>SetEvent(m_hEvent); WaitForSingleObject(m_hThread, INFINITE); CloseHandle(...)</c>
    /// —— 托管侧 = 关闭入队 + 等线程退出。
    /// </summary>
    public void Dispose()
    {
        if (m_disposed) return;
        m_disposed = true;
        try
        {
            m_queue.CompleteAdding();
            m_thread.Join(5000);
        }
        catch (Exception) { /* best effort */ }
        m_queue.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 测试接缝：等待队列排空（原文无此操作 —— 原文靠 APC 异步，
    /// 检查文件内容前必须自己 Sleep）。
    /// </summary>
    public void DrainForTest(int timeoutMs = 3000)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < timeoutMs)
        {
            if (m_queue.Count == 0)
            {
                // 队列空不等于已落盘，留一点时间让最后一条写完
                Thread.Sleep(30);
                if (m_queue.Count == 0) return;
            }
            Thread.Sleep(10);
        }
    }
}
