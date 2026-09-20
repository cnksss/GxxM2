using System;

// 源：Source/RunGate/Common/IocpUtils.pas（实测 1,646 物理行；审计文档写 1,383，以实际为准）
//   以及 Common/IocpCommon.pas（共享池/临界区/MAX_* 常量）
// ============================================================================
// 对照审计结论（详见 docs/并行报告-p2-rungate-impl.md 映射表）：
//   IocpUtils.pas 的 **IOCP 管道本体**（CreateIoCompletionPort / GetQueuedCompletionStatus /
//   WSARecv / WSASend / PostQueuedCompletionStatus / 工作线程池 / OVERLAPPEDEx 内存池）
//   已被 `src/GXX.GatewayKit/GatewayProtocol.cs` 的 `SocketAsyncEventArgs` 实现等价覆盖
//   （.NET 的 SocketAsyncEventArgs 底层就是 Windows IOCP）→ **不复制第二份**。
//   下面只移植 .NET 侧没有对应物、且值得单测的**纯策略**：
//     - 发送缓存分块/回收判据（CheckPostWSASendCache 336-412 / 414-486）
//     - 良性 Winsock 错误码白名单（939 / 735 / 1212）
//     - 接收循环护栏（DoRecvBuffer 600-625）
//     - 工作线程数策略（752-773 / 1519-1533）
// ============================================================================

namespace GXX.RunGate;

/// <summary>
/// 发送缓存策略（IocpUtils.pas:336-412 <c>CheckPostWSASendCache</c> 与 414-486
/// <c>CheckPostWSASendCacheBeforeLock</c> 的判定部分；两者**判定完全相同**，只差一把锁）。
/// </summary>
public static class IocpSendCachePolicy
{
    /// <summary>IocpCommon.pas:24 —— <c>MAX_OVERLAPPEDEX_BUFFER_SIZE = 5</c>（KB）。</summary>
    public const uint MaxOverlappedExBufferSizeKb = 5;

    /// <summary>IocpUtils.pas:364 —— <c>MAX_OVERLAPPEDEX_BUFFER_SIZE shl 10</c> = 5120 字节/次投递。</summary>
    public const uint MaxChunkBytes = MaxOverlappedExBufferSizeKb << 10;

    /// <summary>IocpCommon.pas:26 —— 预分配内存上限。</summary>
    public const int MaxPreallocatedMemorySize = 1024;

    /// <summary>IocpUtils.pas:342/420 —— <c>ERROR_SUCCESS</c>。</summary>
    public const int ErrorSuccess = 0;

    /// <summary>
    /// IocpUtils.pas:361-367 —— 本次可投递的字节数。
    /// 条件：<c>BufSize &gt; 0</c> 且 <c>Position &lt; BufSize</c>；上限 <see cref="MaxChunkBytes"/>。
    /// 不满足条件返回 0（原文是整段 if 跳过，不投递）。
    /// </summary>
    public static uint ComputeChunk(uint bufSize, uint position)
    {
        if (bufSize == 0 || position >= bufSize) return 0;
        uint readCount = bufSize - position;
        return readCount > MaxChunkBytes ? MaxChunkBytes : readCount;
    }

    /// <summary>
    /// IocpUtils.pas:397 / 474 —— <c>(BufSize = 0) or (BufSize = Position)</c> → 从缓存移除并释放。
    /// 注意判据是 <b>等号</b>：<c>Position &gt; BufSize</c>（理论上不该出现）**不会**触发回收。
    /// </summary>
    public static bool ShouldRetire(uint bufSize, uint position) => bufSize == 0 || bufSize == position;

    /// <summary>
    /// IocpUtils.pas:397-406 —— 投递一轮：算出本次块大小并推进 Position，返回是否应回收。
    /// 这是把 336-412 的副作用序列抽成的可测状态迁移（不改网络）。
    /// </summary>
    public static bool Advance(ref uint position, uint bufSize, out uint sentBytes)
    {
        sentBytes = ComputeChunk(bufSize, position);
        position += sentBytes;
        return ShouldRetire(bufSize, position);
    }

    /// <summary>
    /// IocpUtils.pas:343-347 / 421-425 —— <c>FIsPostedCloseQuest</c> 为真时直接成功返回、不做任何投递。
    /// </summary>
    public static int CheckResultWhenClosing(bool isPostedCloseQuest) => isPostedCloseQuest ? ErrorSuccess : -1;
}

/// <summary>
/// 接收侧策略（IocpUtils.pas:600-625 / 699-742 / 883-943）。
/// </summary>
public static class IocpRecvPolicy
{
    /// <summary>IocpUtils.pas:763 —— <c>FRecvDataDefaultSize := 512</c>（每次 WSARecv 的缓冲大小）。</summary>
    public const int RecvDataDefaultSize = 512;

    /// <summary>IocpUtils.pas:619 —— <c>DoCheckRecvBuffer</c> 连续 1000 次不宣告"还有得处理"就抛异常。</summary>
    public const int MaxCheckRecvIterations = 1000;

    /// <summary>WSA_IO_PENDING（10035 / ERROR_IO_PENDING 997 的 Winsock 值）。</summary>
    public const int WsaIoPending = 997;

    /// <summary>IocpCommon.pas:11 —— <c>IOCP_QUEUED_SHUTDOWN = $FFFFFFFF</c>（投递退出用的哨兵）。</summary>
    public const uint IocpQueuedShutdown = 0xFFFFFFFFu;

    /// <summary>
    /// IocpUtils.pas:735 / 939 / 1212 —— 这四个错误**不记日志**（对端正常离开/被重置/已关闭）。
    /// 10053 = WSAECONNABORTED，10054 = WSAECONNRESET，10058 = WSAESHUTDOWN，10038 = WSAENOTSOCK。
    /// </summary>
    public static bool IsBenignWsaError(int error)
        => error == 10053 || error == 10054 || error == 10058 || error == 10038;

    /// <summary>IocpUtils.pas:936-941 —— 非 pending 且非良性才报错；两种情况都要 PostWSAClose。</summary>
    public static bool ShouldReportRecvError(int error) => error != WsaIoPending && !IsBenignWsaError(error);

    /// <summary>IocpUtils.pas:708-712 —— Socket 已失效时 DoZeroBytesRead 直接返回 False 并刷新 tick。</summary>
    public static bool ZeroBytesReadCanPost(bool socketValid) => socketValid;

    /// <summary>
    /// IocpUtils.pas:1096-1099 —— <c>DWORD(lvIOData) = IOCP_QUEUED_SHUTDOWN</c> 表示"通知工作线程退出"。
    /// 托管侧用 <c>SocketAsyncEventArgs.UserToken == null</c> 或本哨兵表达。
    /// </summary>
    public static bool IsShutdownSentinel(uint overlappedLow32) => overlappedLow32 == IocpQueuedShutdown;

    /// <summary>
    /// IocpUtils.pas:611-621 —— 收包循环护栏判定：返回 true 表示"这一轮已处理完，可继续"。
    /// 调用方在 <paramref name="iteration"/> 达到 <see cref="MaxCheckRecvIterations"/> 时应抛
    /// <c>DoCheckRecvBuffer no result False</c>。
    /// </summary>
    public static bool ShouldContinueLoop(bool socketValid, int bufferLength, int iteration)
    {
        if (!socketValid) return false;
        if (bufferLength == 0) return false;
        if (iteration >= MaxCheckRecvIterations)
            throw new InvalidOperationException("DoCheckRecvBuffer no result False");
        return true;
    }
}

/// <summary>
/// 工作线程数策略（IocpUtils.pas:752-773 构造 + 1519-1533 <c>SetWorkerThreadCount</c>）。
/// </summary>
public static class IocpWorkerThreadPolicy
{
    /// <summary>
    /// IocpUtils.pas:768 / 1531 —— <c>Count &lt;= 0</c> 时取 <c>dwNumberOfProcessors * 2</c>。
    /// 与 GatewayKit 当前的 <c>SocketAsyncEventArgs</c> 实现无关，纯策略。
    /// </summary>
    public static int ResolveWorkerThreadCount(int requested, int processorCount)
        => requested <= 0 ? Math.Max(1, processorCount * 2) : requested;

    /// <summary>
    /// IocpUtils.pas:1534-1548 —— 需要新增的线程数（<c>Count &gt; 现有</c> 时）。
    /// 注意原文这里是 <c>Count := Count - FWorkerThreadList.Count</c> 后按差值创建。
    /// </summary>
    public static int ThreadsToCreate(int requested, int current) => Math.Max(0, requested - current);
}
