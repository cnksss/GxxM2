using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GXX.Integration.Tests;

/// <summary>
/// 集成测试的**瞬时失败重试**支撑。
///
/// 背景（为什么需要它）：这两个链路测试用 <see cref="FreePort"/> 取临时端口，
/// 但 `bind(0)` 拿到端口后必须先 `Close()`，之后被测服务/模拟服务器才重新绑定同一个端口
/// —— 这中间存在 **TOCTOU 窗口**。本仓库会同时运行多个工作树（并行车道各有测试宿主），
/// 同一台机器上多个进程同时抢同一端口是常态，于是出现"同一用例连续跑出现 通过/超时 交替"。
///
/// 处置原则：**不改业务代码、不削弱断言**，而是把"端口被抢 / 启动失败 / 等待超时"这类
/// 与业务无关的瞬时失败识别出来，整场景重试若干次；断言失败（xunit 异常）**不重试**，
/// 因此真正的功能缺陷仍会立刻暴露。
///
/// 规程（已写入并行派发台账 §12.5）：
/// 遇到"集成用例偶发失败"，先判断是否属于本类瞬时失败，再决定是否重试；
/// **不要**通过放宽断言、加大超时或串行化整个程序集来"压住"它。
/// </summary>
internal static class IntegrationRetry
{
    /// <summary>与业务无关、可安全重试的失败。</summary>
    private sealed class TransientException : Exception
    {
        public TransientException(string message) : base(message) { }
    }

    /// <summary>申请一个回环临时端口（注意：从释放到重新绑定之间存在竞态窗口）。</summary>
    public static int FreePort()
    {
        var l = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            l.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            return ((IPEndPoint)l.LocalEndPoint!).Port;
        }
        finally { l.Close(); }
    }

    /// <summary>服务/模拟服务器启动失败时抛"瞬时"异常（最常见原因就是端口在窗口期被抢走了）。</summary>
    public static void RequireStarted(bool ok, string what)
    {
        if (!ok)
        {
            throw new TransientException(
                $"{what} 启动失败：最常见原因是 FreePort() 到 bind 之间端口被其它测试宿主抢占（TOCTOU 竞态）");
        }
    }

    /// <summary>等待某一类帧；超时抛"瞬时"异常（而不是返回 default 让调用方得到一个语义不清的断言失败）。</summary>
    public static (uint SockId, ushort Cmd, byte[] Data) RequireFrame(
        ConcurrentQueue<(uint SockId, ushort Cmd, byte[] Data)> queue,
        AutoResetEvent evt, ushort cmd, double seconds, string what)
    {
        var deadline = DateTime.UtcNow.AddSeconds(seconds);
        while (DateTime.UtcNow < deadline)
        {
            while (queue.TryDequeue(out var f))
            {
                if (f.Cmd == cmd) return f;
            }
            evt.WaitOne(200);
        }
        throw new TransientException($"等待 {what}(0x{cmd:X4}) 帧超时（{seconds}s）");
    }

    private static bool IsTransient(Exception ex)
        => ex is TransientException || ex is TimeoutException || ex is SocketException || ex is IOException;

    /// <summary>整场景重试。断言失败（xunit 异常）不会被吞掉。</summary>
    public static void Run(Action body, int attempts = 8)
    {
        for (int i = 1; ; i++)
        {
            try
            {
                body();
                return;
            }
            catch (Exception ex) when (i < attempts && IsTransient(ex))
            {
                // 端口竞态/启动失败/等待超时：退避后换端口重来
                Thread.Sleep(200 * i);
            }
        }
    }

    /// <summary>本次尝试专用的数据库文件名（避免上一次尝试的残留文件/句柄干扰）。</summary>
    public static string UniqueDbFile(string prefix)
        => Path.Combine(AppContext.BaseDirectory, $"{prefix}_{Guid.NewGuid():N}.db");

    public static void TryDelete(string path)
    {
        try { if (File.Exists(path)) File.Delete(path); } catch { /* 尽力而为 */ }
    }

    /// <summary>清掉本测试历次尝试留下的临时数据库文件（文件名带 GUID，不清理会越积越多）。</summary>
    public static void CleanupLeftovers(string prefix)
    {
        try
        {
            foreach (var f in Directory.GetFiles(AppContext.BaseDirectory, $"{prefix}_*.db")) TryDelete(f);
        }
        catch { /* 尽力而为 */ }
    }
}
