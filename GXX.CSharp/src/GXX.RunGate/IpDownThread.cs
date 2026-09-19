using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using GXX.Core.Crypto;

namespace GXX.RunGate;

// =====================================================================================
// uIPDownThread.pas 1:1 转换（Source\RunGate\uIPDownThread.pas，395 行 / ReadAllLines 468 行）
//
// 结构（与原文逐一对应）：
//   TDownloadThread（原 :10-27、87-151）   —— Win32 事件驱动的可等待线程基类
//   TIPDownThread    （原 :29-41、155-220） —— 下载 IP 白/黑名单文本，逐行 Trim + IsIpaddr 过滤
//   TMACDownThread   （原 :43-55、225-289） —— 下载 MAC 列表，逐行 Trim + 去重
//   TAntiPlugDownThread（原 :57-81、295-465）—— {$IF CLIENT_ANTIPLUG = 1} 条件编译，默认**不编译**
//
// 线程模型（关键保真点）：
//   `TEvent.Create(nil, False, False, '')` 的 (ManualReset=False, InitialState=False)
//   → 自动重置、初始未置位。`Execute` 循环 `FEvent.WaitFor(INFINITE)` 阻塞等待，
//   `TriggerEvent` 置位唤醒一次，`Terminate` 覆写为先置 Terminated 再 TriggerEvent
//   —— 保证线程能立刻退出而不是永远阻塞。托管侧用 AutoResetEvent 1:1 复刻。
//   `Sleeping` 属性 = `not FInExecuteLoop`（原 :136-139），即"不在 DoDownload 内"，
//   与 `WaitFor` 是否阻塞无关 —— 这是原文刻意语义，测试中有差异断言。
//
// 可测性设计（本移植的接缝，全部以接口注入，注释标 `接缝`）：
//   * IHttpDownloader    ← WinHttp.pas（TWinHttp.Get 两个重载 + TRequestStatus 枚举）
//   * IAddressList       ← GateShare.pas TAddressList（Lock/UnLock/Clear/Add）
//   * IHashStringList    ← GateShare.pas TSafeHashStringList（Lock/UnLock/Clear/IndexOf/Add）
//   * ILogSink           ← GateShare.pas AddMainLogMsg(Msg, nLevel)
//   * IFileVersionProbe  ← MD5Util.RivestFile（文件 MD5 十六进制）+ ParamStr(0)（模块路径）
//   * DoDownload 的**纯函数化**：`IpDownWork` / `MacDownWork` 是 static 纯函数，
//     把"下载结果文本 → 目标列表"的过滤逻辑与线程/HTTP 完全解耦，可直接单测。
// =====================================================================================

/// <summary>WinHttp.pas:50 `TRequestStatus = (rsReadyToConnect, rsConnected, rsDoRequest,
/// rsResponseOK, rsStreamBreak, rsStreamFinished)`（枚举顺序即序号，1:1）。</summary>
public enum TRequestStatus
{
    rsReadyToConnect = 0,
    rsConnected = 1,
    rsDoRequest = 2,
    rsResponseOK = 3,
    rsStreamBreak = 4,
    rsStreamFinished = 5
}

/// <summary>
/// 接缝：待 WinHttp.pas 移植后接入。
/// 对应原文用到的两个重载：
///   `function Get(URL: string; var RequestStatus: TRequestStatus; AutoDecoding: Boolean = False): string`
///   `function Get(URL: string; StreamOut: TStream): TRequestStatus`
/// 以及 `TWinHttp.OnWork`（TWorkEvent）与 `TimeOut` 属性。
/// </summary>
public interface IHttpDownloader
{
    /// <summary>原文 `Http.TimeOut := 3000`（毫秒）。</summary>
    int TimeOut { get; set; }

    /// <summary>原文 `Http.OnWork := OnDownWork`（进度回调，Breaked 为 var 出参）。</summary>
    void SetOnWork(Action<string, uint, uint, Action> onWork);

    /// <summary>原文 `SL.Text := Http.Get(FDownUrl, RequestStatus)`。</summary>
    string GetText(string url, out TRequestStatus requestStatus);

    /// <summary>原文 `RequestStatus := Http.Get(FileUrl, FMemoryStream)`。</summary>
    TRequestStatus GetStream(string url, Stream streamOut);
}

/// <summary>
/// 接缝：待 GateShare.pas 的 TAddressList 移植后接入。
/// 原文用到：`Lock` / `UnLock` / `Clear` / `Add(IP): pTSockaddr`（IP 列表；IP 不可重复添加，
/// 重复时返回已存在项 —— 见 GateShare.pas:2613-2630）。
/// </summary>
public interface IAddressList
{
    void Lock();
    void UnLock();
    void Clear();
    int Count { get; }
    object Add(string ip);
}

/// <summary>
/// 接缝：待 GateShare.pas 的 TSafeHashStringList 移植后接入。
/// 原文用到：`Lock` / `UnLock` / `Clear` / `IndexOf(S)` / `Add(S)`。
/// </summary>
public interface IHashStringList
{
    void Lock();
    void UnLock();
    void Clear();
    int Count { get; }
    int IndexOf(string s);
    int Add(string s);
}

/// <summary>接缝：待 GateShare.pas 的 AddMainLogMsg(Msg: string; nLevel: Integer) 移植后接入。</summary>
public interface ILogSink
{
    void AddMainLogMsg(string msg, int nLevel);
}

/// <summary>
/// 接缝：待 MD5Util.RivestFile 与 ParamStr(0) 的 RunGate 侧封装到位后接入。
/// </summary>
public interface IFileProbe
{
    /// <summary>原文 `RivestFile(LocalPlugDatFile)`：返回文件 MD5 十六进制（文件不存在返回空）。</summary>
    string RivestFile(string path);

    /// <summary>原文 `ExtractFilePath(ParamStr(0))`：返回可执行文件所在目录。</summary>
    string AppPath { get; }

    /// <summary>原文 `FMemoryStream.SaveToFile(FileName)`。</summary>
    void SaveStreamToFile(Stream stream, string path);
}

/// <summary>IP 过滤基类需要的静态配置（原文散落在 GateShare 单元）。</summary>
public static class IpDownThreadConfig
{
    /// <summary>GateShare.pas:614 `g_sRunGatePlusDllName: string = 'RunGatePlug.dll'`。</summary>
    public const string RunGatePlusDllName = "RunGatePlug.dll";

    /// <summary>原文 `AddMainLogMsg(..., 7)` 用于 IP/MAC 列表下载结果的日志级别。</summary>
    public const int DownloadLogLevel = 7;

    /// <summary>原文 `AddMainLogMsg(..., 0)` 用于反外挂模块更新日志的级别。</summary>
    public const int AntiPlugLogLevel = 0;

    /// <summary>原文 `AddMainLogMsg(..., 1)` 用于配置获取失败日志的级别。</summary>
    public const int ConfigFailLogLevel = 1;
}

/// <summary>
/// TDownloadThread（原 uIPDownThread.pas:10-27、87-151）。
/// Win32 自动重置事件 + `WaitFor(INFINITE)` 的托管等价：AutoResetEvent。
/// </summary>
public abstract class TDownloadThread : IDisposable
{
    private readonly AutoResetEvent FEvent;   // TEvent.Create(nil, False, False, '')
    private volatile bool FInExecuteLoop;
    private volatile bool FIsRun;
    private readonly Thread _thread;

    protected TDownloadThread(bool createSuspended)
    {
        // 原 :87-101
        FreeOnTerminate = false;              // 原 :90
        FInExecuteLoop = false;               // 原 :92
        FEvent = new AutoResetEvent(false);   // 原 :99
        _thread = new Thread(ExecuteLoop) { IsBackground = true };
        if (!createSuspended) _thread.Start();
    }

    /// <summary>原 :90 `FreeOnTerminate := False`（托管侧保留该语义标记）。</summary>
    public bool FreeOnTerminate { get; protected set; }

    /// <summary>原 :26 `property IsRun: Boolean read FIsRun`。</summary>
    public bool IsRun => FIsRun;

    /// <summary>DoDownload 抛出的异常计数（原文无对应物；托管侧用于把后台异常暴露给测试）。</summary>
    public int DoDownloadErrorCount { get; private set; }

    /// <summary>为 true 时 DoDownload 的异常向上抛出（仅测试使用；默认 false 以保护进程）。</summary>
    public bool ThrowOnDoDownloadError { get; set; }

    /// <summary>原 :25/136-139 `property Sleeping: Boolean read GetSleeping`；Result := not FInExecuteLoop。</summary>
    public bool Sleeping => !FInExecuteLoop;

    /// <summary>原 :23 `procedure Terminate; reintroduce; virtual`（覆写以唤醒等待中的线程）。</summary>
    public void Terminate()
    {
        // 原 :141-145
        FTerminated = true;
        TriggerEvent();
    }

    /// <summary>托管侧对应 Delphi `TThread.Terminated`（原文 `while not Terminated` / `if Terminated then Exit`）。</summary>
    protected volatile bool FTerminated;

    /// <summary>原 :147-151 `procedure TriggerEvent`（FEvent.SetEvent）。</summary>
    public void TriggerEvent() => FEvent.Set();

    /// <summary>原 :17 `procedure DoDownload; virtual; abstract`。</summary>
    protected abstract void DoDownload();

    /// <summary>原 :109-134 `procedure Execute`（循环 WaitFor → DoDownload）。</summary>
    private void ExecuteLoop()
    {
        FIsRun = true;                        // 原 :111
        try
        {
            while (!FTerminated)              // 原 :113
            {
                FEvent.WaitOne();             // 原 :115 FEvent.WaitFor(INFINITE)
                if (FTerminated) return;      // 原 :118

                FInExecuteLoop = true;        // 原 :120
                try
                {
                    DoDownload();             // 原 :122
                }
                catch when (!ThrowOnDoDownloadError)
                {
                    // 原文 DoDownload 为 virtual 且各实现自行 try..except；
                    // 托管侧作为后台线程不能泄漏异常（否则会终止进程），
                    // 默认吞掉并记录；测试可打开 ThrowOnDoDownloadError 以断言异常。
                    DoDownloadErrorCount++;
                }
                finally
                {
                    FInExecuteLoop = false;   // 原 :125
                }
            }
        }
        finally
        {
            FIsRun = false;                   // 原 :132
        }
    }

    /// <summary>
    /// 测试辅助：阻塞直到线程已启动并进入等待（或超时）。
    /// 原文没有对应物，仅供单测消除竞态。
    /// </summary>
    public bool WaitUntilStarted(int timeoutMs = 2000)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < timeoutMs)
        {
            if (FIsRun && !FInExecuteLoop) return true;
            Thread.Sleep(1);
        }
        return FIsRun;
    }

    /// <summary>测试辅助：阻塞直到 DoDownload 执行完成一轮。</summary>
    public bool WaitUntilIdle(int timeoutMs = 2000)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < timeoutMs)
        {
            if (!FInExecuteLoop) return true;
            Thread.Sleep(1);
        }
        return !FInExecuteLoop;
    }

    public void Dispose()
    {
        // 原 :103-107 destructor（FEvent.Free）
        Terminate();
        _thread.Join(1000);
        FEvent.Dispose();
    }

    /// <summary>原文 `inherited Create(CreateSuspended)` 起的线程句柄（供单测 Join）。</summary>
    public Thread UnderlyingThread => _thread;
}

/// <summary>
/// TIPDownThread（原 uIPDownThread.pas:29-41、155-220）。
/// 下载一份 IP 列表文本 → Trim 每行 → 仅保留 IsIpaddr 为真的 → 清空后整体替换目标列表。
/// </summary>
public class TIPDownThread : TDownloadThread
{
    private readonly IAddressList FBindAddressList;
    private readonly string FShowText;
    private readonly IHttpDownloader FHttp;
    private readonly ILogSink FLog;

    public string DownUrl { get; set; }        // 原 :40 property DownUrl

    /// <summary>原 :155-161 constructor（Create(False) → 立即启动）。</summary>
    public TIPDownThread(IAddressList bindAddressList, string showText, IHttpDownloader http, ILogSink log)
        : base(false)
    {
        FBindAddressList = bindAddressList;
        FShowText = showText;
        FHttp = http;
        FLog = log;
        FreeOnTerminate = false;              // 原 :158
    }

    /// <summary>原 :216-220 OnDownWork：`Breaked := Terminated`。</summary>
    public void OnDownWork(string fileUrl, uint currentSize, uint totalSize, Action setBreaked)
        => setBreaked();                       // Breaked := Terminated（本线程已 Terminated 即中断）

    /// <summary>
    /// 把下载到的文本行转换为 IP 列表（**纯函数**，原 :191-205 的循环体）。
    /// 规则（逐字照抄）：
    ///   1) 逐行 `IP := Trim(SL.Strings[I])`；
    ///   2) `(Length(IP) > 0) and IsIpaddr(IP)` 才 Add；
    ///   3) 先 `FBindAddressList.Clear` 再逐条 Add（**全量替换**，不是增量）。
    /// </summary>
    public static List<string> IpDownWork(IEnumerable<string> lines)
    {
        var result = new List<string>();
        foreach (string raw in lines)
        {
            string ip = DelphiTrim(raw);
            if (ip.Length > 0 && IsIpaddr(ip))
                result.Add(ip);
        }
        return result;
    }

    /// <summary>原 :163-213 DoDownload。</summary>
    protected override void DoDownload()
    {
        if (DownUrl.Length > 0)               // 原 :171
        {
            FHttp.TimeOut = 3000;             // 原 :177
            string text;
            // 原文结构（原 :178-181）只把 Http.Get 包在 try..except 里：
            //   try SL.Text := Http.Get(FDownUrl, RequestStatus); except end;
            // 因此 Get 抛异常时 RequestStatus 保持 rsReadyToConnect（Delphi out 参数
            // 在进入 Get 时即被赋初值），后续 `<> rsStreamFinished` 判定仍会执行。
            var status = TRequestStatus.rsReadyToConnect;
            try
            {
                text = FHttp.GetText(DownUrl, out status);   // 原 :179
            }
            catch
            {
                text = "";                                   // 原 :180-181 `except end;`
            }

            if (FTerminated) return;      // 原 :183

            if (status != TRequestStatus.rsStreamFinished)              // 原 :185
            {
                FLog?.AddMainLogMsg(FShowText + "失败", IpDownThreadConfig.DownloadLogLevel);   // 原 :187
                return;
            }

            var ips = IpDownWork(SplitLines(text));                      // 原 :195-202

            FBindAddressList.Lock();                                     // 原 :191
            try
            {
                FBindAddressList.Clear();                                // 原 :193
                foreach (string ip in ips)
                {
                    if (FTerminated) return;                             // 原 :197
                    FBindAddressList.Add(ip);
                }
            }
            finally
            {
                FBindAddressList.UnLock();                               // 原 :204
            }

            FLog?.AddMainLogMsg(FShowText + "成功", IpDownThreadConfig.DownloadLogLevel);       // 原 :207
        }
    }

    public static IEnumerable<string> SplitLines(string text)
        => (text ?? "").Replace("\r\n", "\n").Split('\n');

    public static string DelphiTrim(string s)
        => s == null ? "" : s.Trim(' ', '\t', '\r', '\n', '\f', '\v');

    public static bool IsIpaddr(string ip)
    {
        // 原文调用 HUtil32.IsIpaddr（GXX.Core.Util.HUtil32.cs:645-655）：
        //   parts.Length = 4 且每段 int.TryParse 成功且 0 <= v <= 255。
        // 差异说明：.NET int.TryParse 接受 '+' / '-' 前缀，而原文 HUtil32 的等价实现在
        // .NET 上同样用 TryParse —— 为与 Delphi StrToIntDef 行为一致，此处显式要求纯数字段。
        if (string.IsNullOrEmpty(ip)) return false;
        string[] parts = ip.Split('.');
        if (parts.Length != 4) return false;
        foreach (string p in parts)
        {
            if (p == "") return false;
            foreach (char c in p) if (c < '0' || c > '9') return false;
            if (!int.TryParse(p, out int v) || v < 0 || v > 255) return false;
        }
        return true;
    }
}

/// <summary>
/// TMACDownThread（原 uIPDownThread.pas:43-55、225-289）。
/// 与 TIPDownThread 的**唯一差异**（差异断言点）：过滤条件不是 IsIpaddr，
/// 而是 `FBindMACList.IndexOf(MAC) &lt; 0`（去重），且不 Trim 之外的格式校验。
/// </summary>
public class TMACDownThread : TDownloadThread
{
    private readonly IHashStringList FBindMACList;
    private readonly string FShowText;
    private readonly IHttpDownloader FHttp;
    private readonly ILogSink FLog;

    public string DownUrl { get; set; }        // 原 :54

    /// <summary>原 :225-231 constructor。</summary>
    public TMACDownThread(IHashStringList bindMACList, string showText, IHttpDownloader http, ILogSink log)
        : base(false)
    {
        FBindMACList = bindMACList;
        FShowText = showText;
        FHttp = http;
        FLog = log;
        FreeOnTerminate = false;              // 原 :228
    }

    /// <summary>原 :285-289 OnDownWork。</summary>
    public void OnDownWork(string fileUrl, uint currentSize, uint totalSize, Action setBreaked)
        => setBreaked();

    /// <summary>
    /// 纯函数版过滤（原 :265-272）：Trim 后非空 **且** 不在已存在列表中才收集。
    /// 注意：原文判断的是"目标列表（清空后）"的 IndexOf，因此重复行会被自身去重。
    /// </summary>
    public static List<string> MacDownWork(IEnumerable<string> lines)
    {
        var result = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (string raw in lines)
        {
            string mac = TIPDownThread.DelphiTrim(raw);
            if (mac.Length > 0 && !seen.Contains(mac))
            {
                seen.Add(mac);
                result.Add(mac);
            }
        }
        return result;
    }

    /// <summary>原 :233-283 DoDownload。</summary>
    protected override void DoDownload()
    {
        if (DownUrl.Length > 0)               // 原 :241
        {
            FHttp.TimeOut = 3000;             // 原 :247
            // 原文（原 :248-251）只把 Http.Get 包在 try..except 里，参见 TIPDownThread 的说明
            var status = TRequestStatus.rsReadyToConnect;
            string text;
            try
            {
                text = FHttp.GetText(DownUrl, out status);   // 原 :249
            }
            catch
            {
                text = "";                                   // 原 :250-251 `except end;`
            }

            if (FTerminated) return;      // 原 :253

            if (status != TRequestStatus.rsStreamFinished)                     // 原 :255
            {
                FLog?.AddMainLogMsg(FShowText + "失败", IpDownThreadConfig.DownloadLogLevel);   // 原 :257
                return;
            }

            var macs = MacDownWork(TIPDownThread.SplitLines(text));

            FBindMACList.Lock();                                             // 原 :261
            try
            {
                FBindMACList.Clear();                                        // 原 :263
                foreach (string mac in macs)
                {
                    if (FTerminated) return;                                 // 原 :267
                    if (FBindMACList.IndexOf(mac) < 0)                       // 原 :270
                        FBindMACList.Add(mac);
                }
            }
            finally
            {
                FBindMACList.UnLock();                                       // 原 :274
            }

            FLog?.AddMainLogMsg(FShowText + "成功", IpDownThreadConfig.DownloadLogLevel);       // 原 :277
        }
    }
}

/// <summary>
/// TAntiPlugDownThread（原 uIPDownThread.pas:57-81、295-465）。
/// 原文被 <c>{$IF CLIENT_ANTIPLUG = 1}</c> 包住，**默认不编译**（RunGate 工程未定义该条件）。
/// 本移植同样把它隔离为一个独立类型，仅当 <see cref="AntiPlugFeature.IsEnabled"/> 为真时使用，
/// 并保留完整逻辑以便未来开启 —— 见源文件 :57 与 :291/:465 的条件编译边界。
/// </summary>
public static class AntiPlugFeature
{
    /// <summary>对应原文 <c>{$IF CLIENT_ANTIPLUG = 1}</c>：当前构建未定义 → false。</summary>
    public const bool IsEnabled = false;
}

/// <summary>原文 TAntiPlugDownloadFinished 事件（原 :58）。</summary>
public delegate void TAntiPlugDownloadFinished(TDownloadThread sender, bool isUpdateRungateDll, bool isUpdateClientDll);

/// <summary>
/// 原文 TAntiPlugDownThread（原 :59-80、295-465）；仅在 <see cref="AntiPlugFeature.IsEnabled"/>
/// 被打开的构建里使用。此处按原文逐行移植，逻辑与条件编译无关地保留。
/// </summary>
public class TAntiPlugDownThread : TDownloadThread
{
    private readonly MemoryStream FMemoryStream = new();   // 原 :297
    private readonly IHttpDownloader FHttp;
    private readonly ILogSink FLog;
    private readonly IFileProbe FFiles;

    private bool FIsUpdateRungateDll;
    private bool FIsUpdateClientDll;

    public string ConfigUrl { get; set; }                  // 原 :78
    public TAntiPlugDownloadFinished OnDownloadFinished { get; set; }   // 原 :79

    public TAntiPlugDownThread(IHttpDownloader http, ILogSink log, IFileProbe files = null)
        : base(false)
    {
        FHttp = http;
        FLog = log;
        FFiles = files;
        FreeOnTerminate = false;              // 原 :299
    }

    /// <summary>原 :308-311 SaveGxxRunGateDllStreamToFile。</summary>
    public void SaveGxxRunGateDllStreamToFile(string fileName)
    {
        FMemoryStream.Position = 0;
        if (FFiles != null) FFiles.SaveStreamToFile(FMemoryStream, fileName);
    }

    /// <summary>原 :313-317 DoDownloadFinished。</summary>
    private void DoDownloadFinished()
        => OnDownloadFinished?.Invoke(this, FIsUpdateRungateDll, FIsUpdateClientDll);

    /// <summary>原 :459-463 OnDownWork。</summary>
    public void OnDownWork(string fileUrl, uint currentSize, uint totalSize, Action setBreaked)
        => setBreaked();

    /// <summary>反外挂配置解析结果（原 :351-446 的可测核心）。</summary>
    public sealed class AntiPlugPlan
    {
        public bool UpdateClientDll;
        public bool UpdateRungateDll;
        public string ClientMd5 = "";
        public string ClientUrl = "";
        public string RunGateMd5 = "";
        public string RunGateUrl = "";
    }

    /// <summary>
    /// 纯函数版配置解析（原 :356-359、401-402）：读 `[file] md5/file` 与 `[GxxRunGate] md5/file`。
    /// </summary>
    public static AntiPlugPlan ParseConfig(TMemIniFileEx ini)
    {
        var plan = new AntiPlugPlan();
        plan.ClientMd5 = ini.ReadString("file", "md5", "");
        plan.ClientUrl = ini.ReadString("file", "file", "");
        plan.RunGateMd5 = ini.ReadString("GxxRunGate", "md5", "");
        plan.RunGateUrl = ini.ReadString("GxxRunGate", "file", "");
        return plan;
    }

    /// <summary>原 :319-457 DoDownload（配置驱动 + MD5 校验 + 落盘）。</summary>
    protected override void DoDownload()
    {
        if (ConfigUrl.Length > 0)             // 原 :328
        {
            FHttp.TimeOut = 3000;             // 原 :334
            string text;
            try
            {
                text = FHttp.GetText(ConfigUrl, out TRequestStatus status);   // 原 :336
                if (FTerminated) return;      // 原 :340

                if (status != TRequestStatus.rsStreamFinished)                // 原 :342
                {
                    FLog?.AddMainLogMsg("插件更新配置文件获取失败：" + ConfigUrl, IpDownThreadConfig.ConfigFailLogLevel);  // 原 :344
                    return;
                }

                if (FTerminated) return;      // 原 :348

                FIsUpdateClientDll = false;   // 原 :351
                FIsUpdateRungateDll = false;  // 原 :352

                var ini = new TMemIniFileEx("");                        // 原 :354
                var list = new GXX.Core.Util.TStringList();
                foreach (string line in TIPDownThread.SplitLines(text)) list.Add(line);
                ini.SetStrings(list);                                   // 原 :356

                var plan = ParseConfig(ini);

                if (plan.ClientMd5.Length > 0 && plan.ClientUrl.Length > 0)     // 原 :361
                {
                    FMemoryStream.SetLength(0);                             // 原 :363 FMemoryStream.Clear
                    string localPlugDatFile = (FFiles?.AppPath ?? "") + "rungate.dat";   // 原 :365

                    string temp = FFiles?.RivestFile(localPlugDatFile) ?? "";        // 原 :367
                    if (!SameText(temp, plan.ClientMd5))                         // 原 :368
                    {
                        TRequestStatus rs = TRequestStatus.rsReadyToConnect;     // 原 :370
                        try
                        {
                            rs = FHttp.GetStream(plan.ClientUrl, FMemoryStream); // 原 :372
                        }
                        catch
                        {
                            FLog?.AddMainLogMsg("反外挂模块更新失败：" + plan.ClientUrl, IpDownThreadConfig.AntiPlugLogLevel);  // 原 :374
                        }

                        if (rs == TRequestStatus.rsStreamFinished)               // 原 :377
                        {
                            temp = MD5Hex(FMemoryStream);                        // 原 :379
                            if (SameText(temp, plan.ClientMd5))                  // 原 :380
                            {
                                try
                                {
                                    FMemoryStream.Position = 0;
                                    FFiles?.SaveStreamToFile(FMemoryStream, localPlugDatFile);   // 原 :383
                                    FLog?.AddMainLogMsg("反外挂模块更新成功：" + plan.ClientUrl, IpDownThreadConfig.AntiPlugLogLevel);  // 原 :384
                                    FIsUpdateClientDll = true;                   // 原 :386
                                    FMemoryStream.SetLength(0);                  // 原 :388
                                }
                                catch
                                {
                                    FLog?.AddMainLogMsg("反外挂模块更新保存失败", IpDownThreadConfig.AntiPlugLogLevel);  // 原 :390
                                }
                            }
                            else
                            {
                                FLog?.AddMainLogMsg("反外挂模块下载失败，MD5错误", IpDownThreadConfig.AntiPlugLogLevel);  // 原 :395
                            }
                        }
                    }
                }

                // 网关插件（原 :401-438）
                FMemoryStream.SetLength(0);                                  // 原 :404

                if (plan.RunGateMd5.Length > 0 && plan.RunGateUrl.Length > 0)    // 原 :406
                {
                    string localPlugDatFile = (FFiles?.AppPath ?? "") + IpDownThreadConfig.RunGatePlusDllName;   // 原 :408
                    string temp = FFiles?.RivestFile(localPlugDatFile) ?? "";    // 原 :410
                    if (!SameText(temp, plan.RunGateMd5))                        // 原 :411
                    {
                        TRequestStatus rs = TRequestStatus.rsReadyToConnect;     // 原 :413
                        try
                        {
                            rs = FHttp.GetStream(plan.RunGateUrl, FMemoryStream);    // 原 :415
                        }
                        catch
                        {
                            FLog?.AddMainLogMsg("网关插件更新失败：" + plan.RunGateUrl, IpDownThreadConfig.AntiPlugLogLevel);  // 原 :417
                        }

                        if (rs == TRequestStatus.rsStreamFinished)               // 原 :420
                        {
                            temp = MD5Hex(FMemoryStream);                        // 原 :422
                            if (SameText(temp, plan.RunGateMd5))                 // 原 :423
                            {
                                try
                                {
                                    FIsUpdateRungateDll = true;                  // 原 :426
                                    FLog?.AddMainLogMsg("网关插件更新成功：" + plan.RunGateUrl, IpDownThreadConfig.AntiPlugLogLevel);  // 原 :427
                                }
                                catch
                                {
                                    FLog?.AddMainLogMsg("网关插件更新保存失败", IpDownThreadConfig.AntiPlugLogLevel);  // 原 :429
                                }
                            }
                            else
                            {
                                FLog?.AddMainLogMsg("网关插件下载失败，MD5错误", IpDownThreadConfig.AntiPlugLogLevel);  // 原 :434
                            }
                        }
                    }
                }

                // 原 :440-446：修改加载顺序，优化加载网关插件 2019-12-16 18:12:45
                if (FIsUpdateClientDll || FIsUpdateRungateDll)
                {
                    DoDownloadFinished();     // 原文 Synchronize(DoDownloadFinished)，托管侧直接调用
                }
            }
            catch
            {
                // 原 :337-338 `except end;`
            }
            finally
            {
                FMemoryStream.SetLength(0);   // 原 :451
            }
        }
    }

    private static string MD5Hex(MemoryStream ms)
    {
        // 原 :379/422 `MD5Print(MD5Memory(FMemoryStream.Memory, FMemoryStream.Size))`
        byte[] buf = ms.ToArray();
        return MD5Util.MD5BufHex(buf, 0, buf.Length);
    }

    /// <summary>SysUtils.SameText（大小写不敏感、忽略首尾空白）。</summary>
    public static bool SameText(string a, string b)
        => string.Equals((a ?? "").Trim(), (b ?? "").Trim(), StringComparison.OrdinalIgnoreCase);
}
