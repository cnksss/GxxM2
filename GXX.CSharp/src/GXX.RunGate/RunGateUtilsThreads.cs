using System;
using System.Collections.Generic;

// 源：Source/RunGate/RunGateUtils.pas
//   TRunGateManager.GetWorkerThreadCount   3791-3814（线程数账目）
//   TRunGateManager.OnIocpError            4526-4532（IOCP 错误日志格式）
//   TRunGate.GetOnlineUser 相关            4903-4921（按 RunGate 汇总在线列表）
//   MIRCONTEXT_RUN_THREAD_COUNT             MirClientContext.pas:300
// 纯账目/格式逻辑，无 socket / 线程。

namespace GXX.RunGate;

/// <summary>
/// RunGateUtils.pas:3791-3814 —— 工作线程数账目。
/// <para>
/// 原文口径（<c>UseIocpClient = 1</c> + <c>MultiThreadRunContext = 1</c> 的活分支）：
/// <c>Σ(每个 RunGate 的 IocpCore.WorkerThreadCount + 1[TAcceptListenerThread])
///  + FIocpClient.IocpCore.WorkerThreadCount
///  + 1[FFullServiceMsgProcessThread] + MIRCONTEXT_RUN_THREAD_COUNT[TClientContextRunThread]</c>
/// </para>
/// <para>
/// 差异易错点：<c>+1</c> 在**循环内部**（每个 RunGate 一个 accept listener），
/// 而 <c>+FIocpClient...</c> 在循环**外**（整进程只有一个 M2 链路）；
/// 死分支 <c>UseIocpClient = 0</c> 时改成 <c>+ FRunGateList.Count</c>（每个 RunGate 一个接收线程）。
/// </para>
/// </summary>
public static class RunGateThreadAccounting
{
    /// <summary>MirClientContext.pas:300 —— <c>MIRCONTEXT_RUN_THREAD_COUNT = 8</c>。</summary>
    public const int MirContextRunThreadCount = 8;

    /// <summary>RunGateUtils.pas:3801 —— 每个 RunGate 一个 TAcceptListenerThread。</summary>
    public const int AcceptListenerThreadsPerRunGate = 1;

    /// <summary>RunGateUtils.pas:3811 —— 整进程一个 TFullServiceMsgProcessThread。</summary>
    public const int FullServiceMsgProcessThreads = 1;

    /// <summary>
    /// 活分支（UseIocpClient=1 / MultiThreadRunContext=1）的总线程数。
    /// </summary>
    public static int TotalWorkerThreads(IReadOnlyList<int> runGateIocpWorkerThreads, int clientIocpWorkerThreads)
    {
        int total = 0;
        if (runGateIocpWorkerThreads != null)
            foreach (int n in runGateIocpWorkerThreads)
                total += n + AcceptListenerThreadsPerRunGate;

        total += clientIocpWorkerThreads;
        total += FullServiceMsgProcessThreads;
        total += MirContextRunThreadCount;
        return total;
    }

    /// <summary>
    /// 死分支（UseIocpClient=0）的总线程数：把 M2 链路的 IOCP 工作线程换成
    /// 「每个 RunGate 一个 TProcessServerReceiveThread」，并且**没有** MultiThreadRunContext 的额外线程。
    /// </summary>
    public static int TotalWorkerThreadsLegacy(IReadOnlyList<int> runGateIocpWorkerThreads)
    {
        int total = 0;
        if (runGateIocpWorkerThreads != null)
            foreach (int n in runGateIocpWorkerThreads)
                total += n + AcceptListenerThreadsPerRunGate;

        total += (runGateIocpWorkerThreads?.Count ?? 0);          // 原 3807
        return total;
    }
}

/// <summary>RunGateUtils.pas:4526-4532 —— <c>OnIocpError</c> 的日志内容。</summary>
public static class RunGateIocpErrorLog
{
    /// <summary>IocpErrorType.Iocpet_Info —— 原样输出，不加前缀。</summary>
    public const string ErrorPrefix = "错误";

    /// <summary>
    /// 生成日志文本。<paramref name="isInfo"/> 为真直接返回消息；
    /// 否则格式化为 <c>[错误:&lt;code&gt;] &lt;msg&gt;</c>（原文用 <c>Format('[%s:%d] %s', ['错误', ErrorCode, ErrorStr])</c>）。
    /// </summary>
    public static string Format(bool isInfo, string errorStr, int errorCode)
        => isInfo ? errorStr : "[" + ErrorPrefix + ":" + errorCode + "] " + errorStr;
}
