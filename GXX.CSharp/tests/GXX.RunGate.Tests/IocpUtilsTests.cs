using System;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// IocpUtilsPolicy.cs 测试 —— 覆盖 Source/RunGate/Common/IocpUtils.pas:336-412（发送缓存分块/回收）、
/// 600-625（接收循环护栏）、699-742 / 883-943（良性错误码白名单）、1519-1533（工作线程数）。
/// IOCP 管道本体已被 GatewayKit 的 SocketAsyncEventArgs 覆盖，不在本文件测试范围内。
/// </summary>
public class IocpUtilsTests
{
    // ---------------- 发送缓存分块 ----------------

    [Fact]
    public void MaxChunkBytes_Is5ShiftedBy10()
    {
        Assert.Equal(5u, IocpSendCachePolicy.MaxOverlappedExBufferSizeKb);
        Assert.Equal(5120u, IocpSendCachePolicy.MaxChunkBytes);
        Assert.Equal(1024, IocpSendCachePolicy.MaxPreallocatedMemorySize);
        Assert.Equal(0, IocpSendCachePolicy.ErrorSuccess);
    }

    [Fact]
    public void ComputeChunk_SmallRemainder_ReturnsRemainder()
    {
        // 原 363：ReadCount := BufSize - Position
        Assert.Equal(100u, IocpSendCachePolicy.ComputeChunk(400, 300));
        Assert.Equal(1u, IocpSendCachePolicy.ComputeChunk(1, 0));
    }

    [Fact]
    public void ComputeChunk_CapsAt5120()
    {
        Assert.Equal(5120u, IocpSendCachePolicy.ComputeChunk(5120, 0));
        Assert.Equal(5120u, IocpSendCachePolicy.ComputeChunk(5121, 0));
        Assert.Equal(5120u, IocpSendCachePolicy.ComputeChunk(100000, 0));
        // 剩余恰好 5121 且已经发过 1 字节 → 仍取 5120
        Assert.Equal(5120u, IocpSendCachePolicy.ComputeChunk(5121, 1));
    }

    [Fact]
    public void ComputeChunk_BoundariesAtEndOfBuffer()
    {
        // Position == BufSize → 已发完 → 0（原 361 的 Position < BufSize 条件不成立）
        Assert.Equal(0u, IocpSendCachePolicy.ComputeChunk(100, 100));
        Assert.Equal(0u, IocpSendCachePolicy.ComputeChunk(100, 101));
        // BufSize == 0 → 0
        Assert.Equal(0u, IocpSendCachePolicy.ComputeChunk(0, 0));
        Assert.Equal(0u, IocpSendCachePolicy.ComputeChunk(0, 5));
    }

    [Fact]
    public void ShouldRetire_UsesEqualityOnly_Differential()
    {
        // 原 397/474：`(BufSize = 0) or (BufSize = Position)`
        Assert.True(IocpSendCachePolicy.ShouldRetire(0, 0));
        Assert.True(IocpSendCachePolicy.ShouldRetire(5, 5));
        Assert.False(IocpSendCachePolicy.ShouldRetire(5, 4));
        // 差异断言：Position **超过** BufSize 时**不**回收（不是 >=）
        Assert.False(IocpSendCachePolicy.ShouldRetire(5, 6));
        Assert.False(IocpSendCachePolicy.ShouldRetire(5, 100));
    }

    [Fact]
    public void Advance_SendsMultipleChunksThenRetires()
    {
        uint pos = 0;
        uint size = 5120 + 100;                       // 5220

        Assert.False(IocpSendCachePolicy.Advance(ref pos, size, out uint first));
        Assert.Equal(5120u, first);
        Assert.Equal(5120u, pos);

        Assert.True(IocpSendCachePolicy.Advance(ref pos, size, out uint second));
        Assert.Equal(100u, second);
        Assert.Equal(5220u, pos);
    }

    [Fact]
    public void Advance_ZeroSizeBuffer_RetiresImmediatelyWithoutSending()
    {
        uint pos = 0;
        Assert.True(IocpSendCachePolicy.Advance(ref pos, 0, out uint sent));
        Assert.Equal(0u, sent);
        Assert.Equal(0u, pos);
    }

    [Fact]
    public void Advance_NeverExceedsBuffer()
    {
        uint pos = 0;
        uint size = 100;
        int guard = 0;
        while (!IocpSendCachePolicy.Advance(ref pos, size, out _))
        {
            Assert.True(++guard < 10, "应在一轮内完成");
        }
        Assert.Equal(100u, pos);
    }

    [Fact]
    public void ClosingContext_ShortCircuitsToSuccess()
    {
        // 原 343-347 / 421-425：FIsPostedCloseQuest → 直接返回 ERROR_SUCCESS，不投递
        Assert.Equal(0, IocpSendCachePolicy.CheckResultWhenClosing(true));
        Assert.NotEqual(0, IocpSendCachePolicy.CheckResultWhenClosing(false));
    }

    // ---------------- 接收侧 ----------------

    [Fact]
    public void RecvDefaults()
    {
        Assert.Equal(512, IocpRecvPolicy.RecvDataDefaultSize);
        Assert.Equal(1000, IocpRecvPolicy.MaxCheckRecvIterations);
        Assert.Equal(997, IocpRecvPolicy.WsaIoPending);
        Assert.Equal(0xFFFFFFFFu, IocpRecvPolicy.IocpQueuedShutdown);
    }

    [Fact]
    public void BenignErrors_AreTheFourWhitelistedCodes()
    {
        // 原 735 / 939 / 1212：10053 10054 10058 10038
        Assert.True(IocpRecvPolicy.IsBenignWsaError(10053));
        Assert.True(IocpRecvPolicy.IsBenignWsaError(10054));
        Assert.True(IocpRecvPolicy.IsBenignWsaError(10058));
        Assert.True(IocpRecvPolicy.IsBenignWsaError(10038));

        Assert.False(IocpRecvPolicy.IsBenignWsaError(0));
        Assert.False(IocpRecvPolicy.IsBenignWsaError(10035));   // WSAEWOULDBLOCK 不在白名单
        Assert.False(IocpRecvPolicy.IsBenignWsaError(10060));
        Assert.False(IocpRecvPolicy.IsBenignWsaError(997));     // WSA_IO_PENDING 另有判据
    }

    [Fact]
    public void ShouldReportRecvError_ExcludesPendingAndBenign()
    {
        Assert.False(IocpRecvPolicy.ShouldReportRecvError(997));     // pending → 正常
        Assert.False(IocpRecvPolicy.ShouldReportRecvError(10053));
        Assert.False(IocpRecvPolicy.ShouldReportRecvError(10054));
        Assert.False(IocpRecvPolicy.ShouldReportRecvError(10058));
        Assert.False(IocpRecvPolicy.ShouldReportRecvError(10038));
        Assert.True(IocpRecvPolicy.ShouldReportRecvError(10014));    // WSAEFAULT（对齐错误）要报
        Assert.True(IocpRecvPolicy.ShouldReportRecvError(10055));    // WSAENOBUFS 要报
    }

    [Fact]
    public void ZeroBytesRead_OnlyWhenSocketValid()
    {
        Assert.True(IocpRecvPolicy.ZeroBytesReadCanPost(true));
        Assert.False(IocpRecvPolicy.ZeroBytesReadCanPost(false));
    }

    [Fact]
    public void ShutdownSentinel_Recognised()
    {
        Assert.True(IocpRecvPolicy.IsShutdownSentinel(0xFFFFFFFFu));
        Assert.True(IocpRecvPolicy.IsShutdownSentinel(uint.MaxValue));
        Assert.False(IocpRecvPolicy.IsShutdownSentinel(0xFFFFFFFEu));
        Assert.False(IocpRecvPolicy.IsShutdownSentinel(0));
    }

    [Fact]
    public void ShouldContinueLoop_StopsOnClosedSocketOrEmptyBuffer()
    {
        // 原 611-616：Socket 无效或缓冲已空 → Break / Exit
        Assert.False(IocpRecvPolicy.ShouldContinueLoop(false, 100, 0));
        Assert.False(IocpRecvPolicy.ShouldContinueLoop(true, 0, 0));
        Assert.True(IocpRecvPolicy.ShouldContinueLoop(true, 1, 0));
    }

    [Fact]
    public void ShouldContinueLoop_ThrowsAt1000Iterations()
    {
        // 原 619-620：I >= 1000 → raise Exception.Create('DoCheckRecvBuffer no result False')
        Assert.True(IocpRecvPolicy.ShouldContinueLoop(true, 10, 999));
        var ex = Assert.Throws<InvalidOperationException>(
            () => IocpRecvPolicy.ShouldContinueLoop(true, 10, 1000));
        Assert.Contains("DoCheckRecvBuffer no result False", ex.Message);
        Assert.Throws<InvalidOperationException>(() => IocpRecvPolicy.ShouldContinueLoop(true, 10, 1001));
    }

    // ---------------- 工作线程数 ----------------

    [Fact]
    public void WorkerThreadCount_DefaultsToDoubleProcessors()
    {
        // 原 768 / 1531：Count <= 0 → dwNumberOfProcessors * 2
        Assert.Equal(8, IocpWorkerThreadPolicy.ResolveWorkerThreadCount(0, 4));
        Assert.Equal(2, IocpWorkerThreadPolicy.ResolveWorkerThreadCount(-1, 1));
        Assert.Equal(2, IocpWorkerThreadPolicy.ResolveWorkerThreadCount(int.MinValue, 1));
    }

    [Fact]
    public void WorkerThreadCount_ExplicitValueWins()
    {
        Assert.Equal(16, IocpWorkerThreadPolicy.ResolveWorkerThreadCount(16, 4));
        Assert.Equal(1, IocpWorkerThreadPolicy.ResolveWorkerThreadCount(1, 64));
    }

    [Fact]
    public void WorkerThreadCount_NeverBelowOneEvenWithZeroProcessors()
    {
        // 防御：GetSystemInfo 理论上不会返回 0，但 0 会算出 0 线程 → 死锁
        Assert.Equal(1, IocpWorkerThreadPolicy.ResolveWorkerThreadCount(0, 0));
    }

    [Fact]
    public void ThreadsToCreate_OnlyPositiveDelta()
    {
        // 原 1534-1548
        Assert.Equal(4, IocpWorkerThreadPolicy.ThreadsToCreate(10, 6));
        Assert.Equal(0, IocpWorkerThreadPolicy.ThreadsToCreate(10, 10));
        Assert.Equal(0, IocpWorkerThreadPolicy.ThreadsToCreate(10, 20));
    }
}
