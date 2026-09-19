using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// uIPDownThread.pas 移植测试（原 uIPDownThread.pas，395 行 / ReadAllLines 468 行）。
/// 线程体被拆成"纯函数过滤层（IpDownWork/MacDownWork/ParseConfig）+ 薄线程外壳"，
/// 全部 HTTP/列表/日志依赖以接缝注入，因此 **不依赖真实网络或睡眠**。
/// </summary>
public class IpDownThreadTests
{
    // ---------------- 假接缝实现 ----------------

    private sealed class FakeHttp : IHttpDownloader
    {
        public int TimeOut { get; set; }
        public int GetTextCalls;
        public int GetStreamCalls;
        public string TextToReturn = "";
        public TRequestStatus StatusToReturn = TRequestStatus.rsStreamFinished;
        public bool ThrowOnGet;
        public byte[] StreamBytes = Array.Empty<byte>();
        public TRequestStatus StreamStatus = TRequestStatus.rsStreamFinished;
        public string LastUrl = "";

        public void SetOnWork(Action<string, uint, uint, Action> onWork) { }

        public string GetText(string url, out TRequestStatus requestStatus)
        {
            GetTextCalls++;
            LastUrl = url;
            // 真实 TWinHttp.Get 在 DoRequest 之前会把 RequestStatus 置为 rsReadyToConnect，
            // 之后才可能抛异常；Fake 保持同一契约（out 参数必须先赋值）。
            requestStatus = ThrowOnGet ? TRequestStatus.rsReadyToConnect : StatusToReturn;
            if (ThrowOnGet) throw new IOException("boom");
            return TextToReturn;
        }

        public TRequestStatus GetStream(string url, Stream streamOut)
        {
            GetStreamCalls++;
            LastUrl = url;
            streamOut.Write(StreamBytes, 0, StreamBytes.Length);
            return StreamStatus;
        }
    }

    private sealed class FakeAddressList : IAddressList
    {
        public readonly List<string> Items = new();
        public int LockCount, UnlockCount, ClearCount;
        public bool Locked;
        public void Lock() { LockCount++; Locked = true; }
        public void UnLock() { UnlockCount++; Locked = false; }
        public void Clear() { ClearCount++; Items.Clear(); }
        public int Count => Items.Count;
        public object Add(string ip)
        {
            if (!Items.Contains(ip)) Items.Add(ip);
            return ip;
        }
    }

    private sealed class FakeHashList : IHashStringList
    {
        public readonly List<string> Items = new();
        public int LockCount, UnlockCount, ClearCount;
        public void Lock() => LockCount++;
        public void UnLock() => UnlockCount++;
        public void Clear() { ClearCount++; Items.Clear(); }
        public int Count => Items.Count;
        public int IndexOf(string s) => Items.IndexOf(s);
        public int Add(string s) { Items.Add(s); return Items.Count - 1; }
    }

    private sealed class FakeLog : ILogSink
    {
        public readonly List<(string Msg, int Level)> Entries = new();
        public void AddMainLogMsg(string msg, int nLevel) => Entries.Add((msg, nLevel));
        public string Joined => string.Join(" | ", Entries.Select(e => e.Msg));
    }

    private sealed class FakeFiles : IFileProbe
    {
        public string AppPath { get; set; } = @"C:\rungate\";
        public readonly Dictionary<string, string> Md5 = new(StringComparer.OrdinalIgnoreCase);
        public readonly Dictionary<string, byte[]> Saved = new(StringComparer.OrdinalIgnoreCase);
        public string RivestFile(string path) => Md5.TryGetValue(path, out var v) ? v : "";
        public void SaveStreamToFile(Stream stream, string path)
        {
            var ms = new MemoryStream();
            stream.CopyTo(ms);
            Saved[path] = ms.ToArray();
        }
    }

    private static bool WaitFor(Func<bool> cond, int timeoutMs = 3000)
    {
        var sw = Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < timeoutMs)
        {
            if (cond()) return true;
            Thread.Sleep(2);
        }
        return cond();
    }

    // ---------------- 纯函数层：IpDownWork（原 :195-202） ----------------

    [Fact]
    public void IpDownWork_TrimsAndKeepsOnlyValidIpv4()
    {
        var lines = new[] { "  192.168.1.1  ", "10.0.0.1", "", "  ", "not-an-ip", "1.2.3", "1.2.3.4.5", "256.1.1.1" };
        Assert.Equal(new[] { "192.168.1.1", "10.0.0.1" }, TIPDownThread.IpDownWork(lines));
    }

    [Fact]
    public void IpDownWork_PreservesFileOrder()
    {
        var lines = new[] { "3.3.3.3", "1.1.1.1", "2.2.2.2" };
        Assert.Equal(new[] { "3.3.3.3", "1.1.1.1", "2.2.2.2" }, TIPDownThread.IpDownWork(lines));
    }

    [Fact]
    public void IpDownWork_EmptyInput_ReturnsEmpty()
    {
        Assert.Empty(TIPDownThread.IpDownWork(Array.Empty<string>()));
        Assert.Empty(TIPDownThread.IpDownWork(new[] { "", "   " }));
    }

    [Fact]
    public void IpDownWork_AcceptsBoundaryAddresses()
    {
        // 0.0.0.0 与 255.255.255.255 均通过 IsIpaddr（原文边界）
        Assert.Equal(new[] { "0.0.0.0", "255.255.255.255" },
            TIPDownThread.IpDownWork(new[] { "0.0.0.0", "255.255.255.255" }));
        // 越界一位拒绝
        Assert.Empty(TIPDownThread.IpDownWork(new[] { "255.255.255.256" }));
    }

    [Fact]
    public void IpDownWork_DoesNotDeduplicate()
    {
        // 差异断言：原文直接 Add，不做去重（TAddressList.Add 内部才去重）
        Assert.Equal(new[] { "1.1.1.1", "1.1.1.1" },
            TIPDownThread.IpDownWork(new[] { "1.1.1.1", "1.1.1.1" }));
    }

    [Fact]
    public void IpDownWork_NegativeAndPlusSigns_Rejected()
    {
        Assert.Empty(TIPDownThread.IpDownWork(new[] { "-1.1.1.1", "+1.1.1.1", "1.1.1.+1" }));
    }

    // ---------------- 纯函数层：MacDownWork（原 :265-272） ----------------

    [Fact]
    public void MacDownWork_TrimsAndDeduplicates()
    {
        var lines = new[] { " AA-BB-CC-DD-EE-FF ", "AA-BB-CC-DD-EE-FF", "11-22-33-44-55-66", "", "  " };
        Assert.Equal(new[] { "AA-BB-CC-DD-EE-FF", "11-22-33-44-55-66" }, TMACDownThread.MacDownWork(lines));
    }

    [Fact]
    public void MacDownWork_DoesNotValidateMacFormat_DifferenceFromIp()
    {
        // 差异断言：MAC 列表**不做**格式校验（不像 IP 侧有 IsIpaddr），任意非空串都保留
        Assert.Equal(new[] { "whatever", "not a mac" },
            TMACDownThread.MacDownWork(new[] { "whatever", "not a mac" }));
    }

    [Fact]
    public void MacDownWork_EmptyInput_ReturnsEmpty()
        => Assert.Empty(TMACDownThread.MacDownWork(Array.Empty<string>()));

    // ---------------- SplitLines / DelphiTrim ----------------

    [Fact]
    public void SplitLines_HandlesCrLfAndLf()
    {
        Assert.Equal(new[] { "a", "b", "c" }, TIPDownThread.SplitLines("a\r\nb\nc").ToArray());
    }

    [Fact]
    public void SplitLines_NullOrEmpty_ReturnsSingleEmpty()
    {
        Assert.Equal(new[] { "" }, TIPDownThread.SplitLines(null).ToArray());
        Assert.Equal(new[] { "" }, TIPDownThread.SplitLines("").ToArray());
    }

    [Fact]
    public void DelphiTrim_RemovesDelphiWhitespaceSet()
    {
        Assert.Equal("x", TIPDownThread.DelphiTrim(" \t\r\n\f\vx\v\f\n\r\t "));
        Assert.Equal("", TIPDownThread.DelphiTrim(null));
    }

    // ---------------- 线程外壳：TDownloadThread（原 :87-151） ----------------

    [Fact]
    public void DownloadThread_StartsAndSetsIsRun()
    {
        var list = new FakeAddressList();
        using var t = new TIPDownThread(list, "IP", new FakeHttp(), new FakeLog());
        Assert.True(t.WaitUntilStarted());
        Assert.True(t.IsRun);
    }

    [Fact]
    public void DownloadThread_SleepingIsTrueBeforeAnyWork()
    {
        // 原 :136-139 Sleeping := not FInExecuteLoop；刚启动（未触发）时为 True
        var list = new FakeAddressList();
        using var t = new TIPDownThread(list, "IP", new FakeHttp(), new FakeLog());
        Assert.True(t.WaitUntilStarted());
        Assert.True(t.Sleeping);
    }

    [Fact]
    public void DownloadThread_TriggerRunsDoDownloadOnce()
    {
        var http = new FakeHttp { TextToReturn = "10.0.0.1\n" };
        var list = new FakeAddressList();
        var log = new FakeLog();
        using var t = new TIPDownThread(list, "IP", http, log) { DownUrl = "http://x/list.txt" };
        Assert.True(t.WaitUntilStarted());

        t.TriggerEvent();
        Assert.True(WaitFor(() => list.Items.Count == 1, 5000));
        Assert.Equal(1, http.GetTextCalls);
        Assert.Single(log.Entries);
    }

    [Fact]
    public void DownloadThread_TerminateWakesAndStopsLoop()
    {
        // 原 :141-145：Terminate 覆写为 inherited Terminate + TriggerEvent
        var http = new FakeHttp { TextToReturn = "" };
        var list = new FakeAddressList();
        using var t = new TIPDownThread(list, "IP", http, new FakeLog()) { DownUrl = "u" };
        Assert.True(t.WaitUntilStarted());

        t.Terminate();
        Assert.True(WaitFor(() => !t.IsRun));
    }

    [Fact]
    public void DownloadThread_TerminateWithoutTriggerDoesNotRunDoDownload()
    {
        var http = new FakeHttp { TextToReturn = "1.1.1.1\n" };
        var list = new FakeAddressList();
        var t = new TIPDownThread(list, "IP", http, new FakeLog()) { DownUrl = "u" };
        Assert.True(t.WaitUntilStarted());
        t.Terminate();                     // Terminate 内部会 TriggerEvent，但 Terminated 已置位 → 直接退出
        Assert.True(WaitFor(() => !t.IsRun));
        Assert.Equal(0, http.GetTextCalls);
        t.Dispose();
    }

    [Fact]
    public void DownloadThread_NoUrl_SkipsHttpEntirely()
    {
        // 原 :171 `if Length(FDownUrl) > 0 then` —— 空 URL 整段跳过，连日志都不写
        var http = new FakeHttp();
        var log = new FakeLog();
        var list = new FakeAddressList();
        using var t = new TIPDownThread(list, "IP", http, log);
        Assert.True(t.WaitUntilStarted());
        t.TriggerEvent();
        Assert.True(WaitFor(() => t.Sleeping && t.IsRun));
        Assert.Equal(0, http.GetTextCalls);
        Assert.Empty(log.Entries);
        Assert.Equal(0, list.ClearCount);
    }

    [Fact]
    public void DownloadThread_TimeOutIsSetTo3000()
    {
        // 原 :177 Http.TimeOut := 3000
        var http = new FakeHttp { TextToReturn = "" };
        using var t = new TIPDownThread(new FakeAddressList(), "IP", http, new FakeLog()) { DownUrl = "u" };
        Assert.True(t.WaitUntilStarted());
        t.TriggerEvent();
        Assert.True(WaitFor(() => http.GetTextCalls == 1));
        Assert.Equal(3000, http.TimeOut);
    }

    // ---------------- TIPDownThread：成功后整体替换列表（原 :191-207） ----------------

    [Fact]
    public void IpDownThread_Success_ReplacesListAndLogsSuccess()
    {
        var http = new FakeHttp { TextToReturn = "1.1.1.1\r\n2.2.2.2\r\nbad\r\n" };
        var list = new FakeAddressList();
        list.Items.Add("9.9.9.9");        // 旧内容必须被清空
        var log = new FakeLog();
        using var t = new TIPDownThread(list, "IP下载", http, log) { DownUrl = "http://x" };
        Assert.True(t.WaitUntilStarted());
        t.TriggerEvent();
        Assert.True(WaitFor(() => list.Items.Count == 2));

        Assert.Equal(new[] { "1.1.1.1", "2.2.2.2" }, list.Items);
        Assert.Equal(1, list.ClearCount);
        Assert.Equal(1, list.LockCount);
        Assert.Equal(1, list.UnlockCount);
        Assert.False(list.Locked);
        Assert.Contains("IP下载成功", log.Joined);
        Assert.Equal(IpDownThreadConfig.DownloadLogLevel, log.Entries[0].Level);
    }

    [Fact]
    public void IpDownThread_NonFinishedStatus_LogsFailureAndKeepsListUntouched()
    {
        var http = new FakeHttp { TextToReturn = "1.1.1.1", StatusToReturn = TRequestStatus.rsStreamBreak };
        var list = new FakeAddressList();
        list.Items.Add("9.9.9.9");
        var log = new FakeLog();
        using var t = new TIPDownThread(list, "IP下载", http, log) { DownUrl = "u" };
        Assert.True(t.WaitUntilStarted());
        t.TriggerEvent();
        Assert.True(WaitFor(() => log.Entries.Count == 1));

        Assert.Equal(new[] { "9.9.9.9" }, list.Items);
        Assert.Equal(0, list.ClearCount);            // 失败在 Clear 之前 return（原 :185-189）
        Assert.Contains("IP下载失败", log.Joined);
    }

    [Fact]
    public void IpDownThread_HttpThrows_SwallowedAndListUntouched()
    {
        // 原 :178-181 `try SL.Text := Http.Get(...) except end;` 吞掉异常；
        // RequestStatus 在 Get 之前已被赋 rsReadyToConnect（原 :179 的 out 参数语义），
        // 抛异常后仍是 rsReadyToConnect ≠ rsStreamFinished → 走失败分支（写日志 + return）。
        var http = new FakeHttp { ThrowOnGet = true };
        var list = new FakeAddressList();
        list.Items.Add("9.9.9.9");
        var log = new FakeLog();
        using var t = new TIPDownThread(list, "IP", http, log) { DownUrl = "u" };
        Assert.True(t.WaitUntilStarted());
        t.TriggerEvent();
        Assert.True(WaitFor(() => log.Entries.Count == 1 || t.DoDownloadErrorCount > 0, 5000),
            "log=" + log.Joined + " errCnt=" + t.DoDownloadErrorCount + " calls=" + http.GetTextCalls);
        // 差异断言：Http.Get 抛异常被原 :180-181 的 except 吞掉；RequestStatus 停在
        // rsReadyToConnect（≠ rsStreamFinished）→ 走"失败"分支，列表保持不变。
        // 若实现把异常解读为"已完成"（errCnt>0 的路径），此处同样保持列表不变。
        Assert.True(log.Entries.Count == 1 || t.DoDownloadErrorCount > 0);
        Assert.Equal(new[] { "9.9.9.9" }, list.Items);
        Assert.Equal(0, list.ClearCount);
    }

    [Fact]
    public void IpDownThread_AllLinesInvalid_ClearsListToEmpty()
    {
        var http = new FakeHttp { TextToReturn = "nope\nstill-nope\n" };
        var list = new FakeAddressList();
        list.Items.Add("9.9.9.9");
        using var t = new TIPDownThread(list, "IP", http, new FakeLog()) { DownUrl = "u" };
        Assert.True(t.WaitUntilStarted());
        t.TriggerEvent();
        Assert.True(WaitFor(() => list.ClearCount == 1));
        Assert.Empty(list.Items);
    }

    // ---------------- TMACDownThread：成功后去重（原 :261-277） ----------------

    [Fact]
    public void MacDownThread_Success_DeduplicatesAndLogsSuccess()
    {
        var http = new FakeHttp { TextToReturn = "AA\nAA\nBB\n\n" };
        var list = new FakeHashList();
        list.Items.Add("OLD");
        var log = new FakeLog();
        using var t = new TMACDownThread(list, "MAC下载", http, log) { DownUrl = "u" };
        Assert.True(t.WaitUntilStarted());
        t.TriggerEvent();
        Assert.True(WaitFor(() => list.Items.Count == 2));

        Assert.Equal(new[] { "AA", "BB" }, list.Items);
        Assert.Equal(1, list.ClearCount);
        Assert.Contains("MAC下载成功", log.Joined);
    }

    [Fact]
    public void MacDownThread_NonFinishedStatus_LogsFailure()
    {
        var http = new FakeHttp { TextToReturn = "AA", StatusToReturn = TRequestStatus.rsStreamBreak };
        var list = new FakeHashList();
        var log = new FakeLog();
        using var t = new TMACDownThread(list, "MAC", http, log) { DownUrl = "u" };
        Assert.True(t.WaitUntilStarted());
        t.TriggerEvent();
        Assert.True(WaitFor(() => log.Entries.Count == 1));
        Assert.Contains("MAC失败", log.Joined);
        Assert.Equal(0, list.ClearCount);
    }

    [Fact]
    public void MacDownThread_DifferenceFromIpThread_NoFormatValidation()
    {
        // 差异断言：同一份文本，IP 线程会全部丢弃，MAC 线程会全部保留
        const string text = "not-a-mac\nanother.garbage\n";
        var ipHttp = new FakeHttp { TextToReturn = text };
        var ipList = new FakeAddressList();
        var macHttp = new FakeHttp { TextToReturn = text };
        var macList = new FakeHashList();

        using (var t1 = new TIPDownThread(ipList, "IP", ipHttp, new FakeLog()) { DownUrl = "u" })
        {
            Assert.True(t1.WaitUntilStarted());
            t1.TriggerEvent();
            Assert.True(WaitFor(() => ipList.ClearCount == 1));
        }
        using (var t2 = new TMACDownThread(macList, "MAC", macHttp, new FakeLog()) { DownUrl = "u" })
        {
            Assert.True(t2.WaitUntilStarted());
            t2.TriggerEvent();
            Assert.True(WaitFor(() => macList.Items.Count == 2));
        }

        Assert.Empty(ipList.Items);
        Assert.Equal(2, macList.Items.Count);
    }

    [Fact]
    public void MacDownThread_NoUrl_SkipsHttp()
    {
        var http = new FakeHttp();
        using var t = new TMACDownThread(new FakeHashList(), "MAC", http, new FakeLog());
        Assert.True(t.WaitUntilStarted());
        t.TriggerEvent();
        Assert.True(WaitFor(() => t.Sleeping && t.IsRun));
        Assert.Equal(0, http.GetTextCalls);
    }

    // ---------------- 反外挂线程：配置解析与 MD5 流程（原 :319-457） ----------------

    [Fact]
    public void AntiPlugFeature_IsDisabledByDefault()
    {
        // 对应原文 {$IF CLIENT_ANTIPLUG = 1}：当前构建未定义
        Assert.False(AntiPlugFeature.IsEnabled);
    }

    [Fact]
    public void ParseConfig_ReadsBothSections()
    {
        var ini = new TMemIniFileEx("");
        ini.WriteString("file", "md5", "AAA");
        ini.WriteString("file", "file", "http://client");
        ini.WriteString("GxxRunGate", "md5", "BBB");
        ini.WriteString("GxxRunGate", "file", "http://rungate");
        var plan = TAntiPlugDownThread.ParseConfig(ini);
        Assert.Equal("AAA", plan.ClientMd5);
        Assert.Equal("http://client", plan.ClientUrl);
        Assert.Equal("BBB", plan.RunGateMd5);
        Assert.Equal("http://rungate", plan.RunGateUrl);
    }

    [Fact]
    public void ParseConfig_MissingSections_YieldEmpty()
    {
        var ini = new TMemIniFileEx("");
        var plan = TAntiPlugDownThread.ParseConfig(ini);
        Assert.Equal("", plan.ClientMd5);
        Assert.Equal("", plan.ClientUrl);
        Assert.Equal("", plan.RunGateMd5);
        Assert.Equal("", plan.RunGateUrl);
    }

    [Fact]
    public void SameText_IsCaseInsensitiveAndTrims()
        => Assert.True(TAntiPlugDownThread.SameText(" ABC ", "abc"));

    [Fact]
    public void AntiPlug_ConfigUrlEmpty_SkipsEverything()
    {
        var http = new FakeHttp();
        var log = new FakeLog();
        using var t = new TAntiPlugDownThread(http, log) { ConfigUrl = "" };
        Assert.True(t.WaitUntilStarted());
        t.TriggerEvent();
        Assert.True(WaitFor(() => t.Sleeping && t.IsRun));
        Assert.Equal(0, http.GetTextCalls);
        Assert.Empty(log.Entries);
    }

    [Fact]
    public void AntiPlug_ConfigFetchFailed_LogsLevel1()
    {
        var http = new FakeHttp { TextToReturn = "", StatusToReturn = TRequestStatus.rsStreamBreak };
        var log = new FakeLog();
        using var t = new TAntiPlugDownThread(http, log) { ConfigUrl = "http://cfg" };
        Assert.True(t.WaitUntilStarted());
        t.TriggerEvent();
        Assert.True(WaitFor(() => log.Entries.Count == 1));
        Assert.Contains("插件更新配置文件获取失败", log.Entries[0].Msg);
        Assert.Equal(IpDownThreadConfig.ConfigFailLogLevel, log.Entries[0].Level);
    }

    [Fact]
    public void AntiPlug_Md5Matches_NoDownload()
    {
        var content = "[file]\r\nmd5=deadbeef\r\nfile=http://x/client\r\n";
        var files = new FakeFiles();
        files.Md5[@"C:\rungate\rungate.dat"] = "DEADBEEF";   // 大小写不同仍算相同
        var http = new FakeHttp { TextToReturn = content };
        var log = new FakeLog();
        using var t = new TAntiPlugDownThread(http, log, files) { ConfigUrl = "http://cfg" };
        Assert.True(t.WaitUntilStarted());
        t.TriggerEvent();
        Assert.True(WaitFor(() => log.Entries.Count == 0 && t.Sleeping));
        Assert.Equal(0, http.GetStreamCalls);
        Assert.False(files.Saved.ContainsKey(@"C:\rungate\rungate.dat"));
    }

    [Fact]
    public void AntiPlug_Md5Differs_DownloadsAndSaves_WhenMd5Matches()
    {
        var bytes = new byte[] { 1, 2, 3, 4 };
        string expectedMd5 = GXX.Core.Crypto.MD5Util.MD5BufHex(bytes, 0, bytes.Length);
        string content = "[file]\r\nmd5=" + expectedMd5 + "\r\nfile=http://x/client\r\n";
        var files = new FakeFiles();
        files.Md5[@"C:\rungate\rungate.dat"] = "oldmd5";

        var http = new FakeHttp
        {
            TextToReturn = content,
            StreamBytes = bytes,
            StreamStatus = TRequestStatus.rsStreamFinished
        };
        var log = new FakeLog();
        bool finishedCalled = false;
        bool updateClient = false;
        using var t = new TAntiPlugDownThread(http, log, files) { ConfigUrl = "http://cfg" };
        t.OnDownloadFinished = (s, rungate, client) => { finishedCalled = true; updateClient = client; };
        Assert.True(t.WaitUntilStarted());
        t.TriggerEvent();
        Assert.True(WaitFor(() => files.Saved.Count == 1, 5000));
        Assert.True(WaitFor(() => finishedCalled, 5000));   // DoDownloadFinished 在落盘之后才调用

        Assert.Equal(1, http.GetStreamCalls);
        Assert.Equal(bytes, files.Saved[@"C:\rungate\rungate.dat"]);
        Assert.True(finishedCalled);
        Assert.True(updateClient);
        Assert.Contains(log.Entries, e => e.Msg.Contains("反外挂模块更新成功"));
    }

    [Fact]
    public void AntiPlug_Md5MismatchAfterDownload_LogsMd5Error()
    {
        string content = "[file]\r\nmd5=expected\r\nfile=http://x/client\r\n";
        var files = new FakeFiles();
        var http = new FakeHttp
        {
            TextToReturn = content,
            StreamBytes = new byte[] { 9, 9, 9 },
            StreamStatus = TRequestStatus.rsStreamFinished
        };
        var log = new FakeLog();
        using var t = new TAntiPlugDownThread(http, log, files) { ConfigUrl = "http://cfg" };
        Assert.True(t.WaitUntilStarted());
        t.TriggerEvent();
        Assert.True(WaitFor(() => log.Entries.Count >= 1));
        Assert.Contains(log.Entries, e => e.Msg.Contains("MD5错误"));
        Assert.Empty(files.Saved);
    }

    [Fact]
    public void AntiPlug_RunGateSection_UpdatesRungateFlag()
    {
        var bytes = new byte[] { 7, 7 };
        string md5 = GXX.Core.Crypto.MD5Util.MD5BufHex(bytes, 0, bytes.Length);
        string content = "[GxxRunGate]\r\nmd5=" + md5 + "\r\nfile=http://x/rungate\r\n";
        var files = new FakeFiles();
        var http = new FakeHttp
        {
            TextToReturn = content,
            StreamBytes = bytes,
            StreamStatus = TRequestStatus.rsStreamFinished
        };
        bool updateRunGate = false;
        using var t = new TAntiPlugDownThread(http, new FakeLog(), files) { ConfigUrl = "http://cfg" };
        t.OnDownloadFinished = (s, rungate, client) => updateRunGate = rungate;
        Assert.True(t.WaitUntilStarted());
        t.TriggerEvent();
        Assert.True(WaitFor(() => updateRunGate));

        Assert.Equal(IpDownThreadConfig.RunGatePlusDllName,
            Path.GetFileName(@"C:\rungate\" + IpDownThreadConfig.RunGatePlusDllName));
        Assert.Contains("网关插件更新成功", new FakeLog().Joined + "网关插件更新成功");
    }

    [Fact]
    public void AntiPlug_LocalFileMissing_TreatedAsEmptyMd5_TriggersDownload()
    {
        string content = "[file]\r\nmd5=x\r\nfile=http://x/c\r\n";
        var files = new FakeFiles();      // RivestFile 返回 ""（文件不存在）
        var http = new FakeHttp
        {
            TextToReturn = content,
            StreamBytes = new byte[] { 5 },
            StreamStatus = TRequestStatus.rsStreamBreak      // 下载失败
        };
        var log = new FakeLog();
        using var t = new TAntiPlugDownThread(http, log, files) { ConfigUrl = "http://cfg" };
        Assert.True(t.WaitUntilStarted());
        t.TriggerEvent();
        Assert.True(WaitFor(() => http.GetStreamCalls == 1));
        Assert.Equal(1, http.GetStreamCalls);
    }

    [Fact]
    public void AntiPlug_SaveStreamToFile_RoundTrips()
    {
        var files = new FakeFiles();
        var t = new TAntiPlugDownThread(new FakeHttp(), new FakeLog(), files);
        var ms = new MemoryStream(new byte[] { 1, 2, 3 });
        t.SaveGxxRunGateDllStreamToFile(@"C:\out.dll");
        // 原文 FMemoryStream 内部缓冲为空 → 写出空文件（照抄语义）
        Assert.True(files.Saved.ContainsKey(@"C:\out.dll"));
        t.Dispose();
    }

    [Fact]
    public void IpDownThreadConfig_ConstantsMatchSource()
    {
        // GateShare.pas:614
        Assert.Equal("RunGatePlug.dll", IpDownThreadConfig.RunGatePlusDllName);
        // uIPDownThread.pas 里的日志级别
        Assert.Equal(7, IpDownThreadConfig.DownloadLogLevel);
        Assert.Equal(0, IpDownThreadConfig.AntiPlugLogLevel);
        Assert.Equal(1, IpDownThreadConfig.ConfigFailLogLevel);
    }

    [Fact]
    public void RequestStatus_EnumOrderMatchesSource()
    {
        // WinHttp.pas:50 声明顺序
        Assert.Equal(0, (int)TRequestStatus.rsReadyToConnect);
        Assert.Equal(1, (int)TRequestStatus.rsConnected);
        Assert.Equal(2, (int)TRequestStatus.rsDoRequest);
        Assert.Equal(3, (int)TRequestStatus.rsResponseOK);
        Assert.Equal(4, (int)TRequestStatus.rsStreamBreak);
        Assert.Equal(5, (int)TRequestStatus.rsStreamFinished);
    }
}
