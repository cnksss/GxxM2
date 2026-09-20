using System;
using System.Collections.Generic;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// RunGateUtilsThreads.cs 测试 —— 覆盖 RunGateUtils.pas:3791-3814（线程数账目）与
/// 4526-4532（OnIocpError 日志格式）。
/// </summary>
public class RunGateUtilsThreadsTests
{
    [Fact]
    public void Constants_MatchSource()
    {
        Assert.Equal(8, RunGateThreadAccounting.MirContextRunThreadCount);      // MirClientContext.pas:300
        Assert.Equal(1, RunGateThreadAccounting.AcceptListenerThreadsPerRunGate);
        Assert.Equal(1, RunGateThreadAccounting.FullServiceMsgProcessThreads);
    }

    [Fact]
    public void Total_DocumentedExample()
    {
        // 假想：2 个 RunGate，各自 IocpCore 工作线程 4；M2 链路 IOCP 2 个
        // 期望 = (4+1)*2 + 2 + 1 + 8 = 10 + 11 = 21
        int got = RunGateThreadAccounting.TotalWorkerThreads(new[] { 4, 4 }, 2);
        Assert.Equal(21, got);
    }

    [Fact]
    public void Total_AcceptListenerIsPerRunGate_ClientIocpIsProcessWide()
    {
        // 差异断言：RunGate 数量翻倍时，只有"每 RunGate 的 +1"跟着翻倍
        int one = RunGateThreadAccounting.TotalWorkerThreads(new[] { 4 }, 2);     // 4+1+2+1+8 = 16
        int two = RunGateThreadAccounting.TotalWorkerThreads(new[] { 4, 4 }, 2);  // 10+2+1+8 = 21
        Assert.Equal(16, one);
        Assert.Equal(21, two);
        Assert.Equal(5, two - one);          // 4（新 RunGate 的工作线程）+ 1（它的 accept listener）
    }

    [Fact]
    public void Total_EmptyList_StillCountsSharedThreads()
    {
        // 没有 RunGate 时仍有：M2 链路 IOCP + FullServiceMsg 线程 + 8 个 ClientContextRunThread
        Assert.Equal(0 + 0 + 1 + 8, RunGateThreadAccounting.TotalWorkerThreads(Array.Empty<int>(), 0));
        Assert.Equal(0 + 5 + 1 + 8, RunGateThreadAccounting.TotalWorkerThreads(Array.Empty<int>(), 5));
    }

    [Fact]
    public void Total_NullList_TreatedAsEmpty()
    {
        Assert.Equal(9, RunGateThreadAccounting.TotalWorkerThreads(null, 0));
    }

    [Fact]
    public void Total_LargeValues_DoNotOverflowInt()
    {
        var many = new List<int>();
        for (int i = 0; i < 1000; i++) many.Add(64);
        Assert.Equal(1000 * 65 + 64 + 1 + 8, RunGateThreadAccounting.TotalWorkerThreads(many, 64));
    }

    [Fact]
    public void TotalLegacy_UsesRunGateCountInsteadOfClientIocp()
    {
        // 原 3806-3808 死分支：+ FRunGateList.Count（每个 RunGate 一个接收线程），且无 8+1
        Assert.Equal((4 + 1) * 3 + 3, RunGateThreadAccounting.TotalWorkerThreadsLegacy(new[] { 4, 4, 4 }));  // 18
        Assert.Equal(0, RunGateThreadAccounting.TotalWorkerThreadsLegacy(Array.Empty<int>()));
        Assert.Equal(0, RunGateThreadAccounting.TotalWorkerThreadsLegacy(null));
    }

    [Fact]
    public void Differential_LegacyDiffersFromLive()
    {
        var gates = new[] { 4, 4 };
        Assert.NotEqual(RunGateThreadAccounting.TotalWorkerThreads(gates, 2),
                        RunGateThreadAccounting.TotalWorkerThreadsLegacy(gates));
        Assert.Equal(12, RunGateThreadAccounting.TotalWorkerThreadsLegacy(gates));   // (4+1)*2 + 2
    }

    // ---------------- OnIocpError ----------------

    [Fact]
    public void IocpError_InfoIsRawMessage()
    {
        // 原 4528-4529：Iocpet_Info → AddIocpLogMsg(ErrorStr)（不加前缀）
        Assert.Equal("客户端断开", RunGateIocpErrorLog.Format(true, "客户端断开", 10054));
        Assert.Equal("", RunGateIocpErrorLog.Format(true, "", 0));
    }

    [Fact]
    public void IocpError_ErrorIsPrefixedWithCode()
    {
        // 原 4531：Format('[%s:%d] %s', ['错误', ErrorCode, ErrorStr])
        Assert.Equal("[错误:10054] 连接被重置", RunGateIocpErrorLog.Format(false, "连接被重置", 10054));
        Assert.Equal("[错误:0] x", RunGateIocpErrorLog.Format(false, "x", 0));
        Assert.Equal("[错误:-1] x", RunGateIocpErrorLog.Format(false, "x", -1));
    }

    [Fact]
    public void IocpError_InfoAndErrorDifferForSameInput_Differential()
    {
        Assert.NotEqual(RunGateIocpErrorLog.Format(true, "m", 7),
                        RunGateIocpErrorLog.Format(false, "m", 7));
        Assert.Equal("m", RunGateIocpErrorLog.Format(true, "m", 7));
    }
}
