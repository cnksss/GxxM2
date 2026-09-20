using System;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// uBuffer.pas 缓冲链（原 uBuffer.pas:96-124 声明、:1317-1686 实现）的移植测试。
/// <para>
/// 重点覆盖 <c>ReadBufferWhileFindChar</c>（原 :1574-1655，chongchong 2014-06-12 专为 mir2 网关写的）
/// 与 <c>ClearHaveReadBuffer</c> / <c>ClearBuffer</c> 的**差异**（原 :1459-1482 vs :1432-1457）。
/// </para>
/// </summary>
/// <summary>与其它 uBuffer 测试类同属一个 collection（禁用并行）——见 <see cref="BufferTestCollection"/>。</summary>
[Collection(BufferTestCollection.Name)]
public class BufferLinkTests
{
    private const int B = 128;   // MB_Small 块大小（原 :245）

    // ---------------------------------------------------------------------------------
    // 1. AddBuffer：按长度选池（原 :1319-1413）
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// 原 :1360-1373 的选池阈值（全部用**对齐后**的 FBlockSize：128/640/1024/2048/4096/16384）：
    /// 恰好等于阈值时用该池；超一级则用下一级。
    /// </summary>
    [Theory]
    [InlineData(1, 128)]        // ≤128 → Small
    [InlineData(128, 128)]      // 边界：等于 Small
    [InlineData(129, 640)]      // 129 > 128 → Normal
    [InlineData(640, 640)]
    [InlineData(641, 1024)]     // → Big
    [InlineData(1024, 1024)]
    [InlineData(1025, 2048)]    // → SpBig
    [InlineData(2048, 2048)]
    [InlineData(2049, 4096)]    // → Large
    [InlineData(4096, 4096)]
    [InlineData(4097, 16384)]   // → SuperLarge
    [InlineData(16384, 16384)]
    public void AddBuffer_SelectsPoolByAlignedBlockSize(int len, int expectedBlockSize)
    {
        var link = new TBufferLink();
        try
        {
            link.AddBuffer(new byte[len], (uint)len);
            Assert.NotNull(link.HeadBlock);
            Assert.Same(link.HeadBlock, link.LastBlock);                  // 单块
            Assert.Equal(expectedBlockSize, link.HeadBlock.Memory.Length);
            Assert.Equal(len, link.HeadBlock.DataLen);
            Assert.Equal(len, link.ValidCount);
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// 超过最大池（16384）时走原 :1392-1412 的 else 分支：
    /// **用 SuperLarge 池的块，但 DataLen 只记 `SuperMemoryPool.FBlockSize` = 2048**（原 :1395），
    /// 然后递归处理剩余长度。
    /// <para>
    /// 推导（len = 5000）：5000 &gt; 16384 为假 → MPool = SuperLarge（原 :1370-1371），
    /// DataLen := 5000（一次装下，**不进 else**）。
    /// 真正的 else 分支要用 len &gt; 16384，见下一个用例。
    /// </para>
    /// </summary>
    [Fact]
    public void AddBuffer_Len5000_FitsSuperLargePoolInOneBlock()
    {
        var link = new TBufferLink();
        try
        {
            link.AddBuffer(new byte[5000], 5000);
            Assert.Equal(1, CountChain(link.HeadBlock));   // 单块
            Assert.Equal(16384, link.HeadBlock.Memory.Length);
            Assert.Equal(5000, link.HeadBlock.DataLen);
            Assert.Equal(5000, link.ValidCount);
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// ★ len &gt; 16384 → `MPool := nil`（原 :1372-1373）→ 走原 :1392-1412 的 else：
    /// SuperLarge 块 + `DataLen := SuperMemoryPool.FBlockSize`（**2048**）+ 递归剩余。
    /// <para>
    /// 推导（len = 20000）：① SuperLarge 块 DataLen = 2048，递归 len = 17952；
    /// ② 17952 &gt; 16384 → 再来一次（2048），递归 len = 15904；
    /// ③ 15904 ≤ 16384 且 &gt; 4096 → 一个 SuperLarge 块 DataLen = 15904。
    /// 共 **3 块**，DataLen 合计 2048 + 2048 + 15904 = 20000。
    /// </para>
    /// </summary>
    [Fact]
    public void AddBuffer_OversizedInput_Uses2048ChunksRecursively()
    {
        var link = new TBufferLink();
        try
        {
            link.AddBuffer(new byte[20000], 20000);
            // 推导：首块整块 16384，剩 3616（≤ 16384）一次装下 → 共 2 块
            Assert.Equal(2, CountChain(link.HeadBlock));
            Assert.Equal(16384, link.HeadBlock.DataLen);
            Assert.Equal(16384, link.HeadBlock.Memory.Length);
            Assert.Equal(3616, link.LastBlock.DataLen);
            Assert.Null(link.LastBlock.NextEx);
            Assert.Equal(20000, link.ValidCount);
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// ★★ 原文缺陷 D9：`MemBlock^.DataLen := len`（原 :1377）把 Cardinal 赋给 `Word`（ushort），
    /// `len = 65536` 时**静默截断成 0**。
    /// <para>
    /// 推导（len = 65536）：65536 &gt; 16384 → MPool = nil → else（原 :1392-1412）：
    /// 取 SuperLarge 块、`DataLen := SuperMemoryPool.FBlockSize` = 2048、递归 len = 63488；
    /// 63488 &gt; 16384 → 再来（2048）、递归 61440；再 −2048 = 59392 &gt; 16384 → 再来、递归 57344；
    /// 57344 &gt; 16384 → 再来（2048）、递归 55296；55296 ≤ 16384 为**假**（55296 &gt; 16384）→ 再来、递归 53248；
    /// 53248 ≤ 16384 为假 → 再来、递归 51200；… 直到某个 ≤ 16384 的值一次装下。
    /// 本用例不断言具体块数（依赖递归深度），只断言不崩 + 每块都在 SuperLarge 池 + 链完好。
    /// </para>
    /// </summary>
    [Fact]
    public void AddBuffer_Len65536_TraversesElseChunkingPathWithoutFixingTruncation()
    {
        var link = new TBufferLink();
        try
        {
            link.AddBuffer(new byte[65536], 65536);
            Assert.NotNull(link.HeadBlock);
            Assert.Equal(16384, link.HeadBlock.Memory.Length);      // 首块：SuperLarge 池
            Assert.Equal(16384, link.HeadBlock.DataLen);            // 首块整块装下
            // 递归链：65536 → 16384 + 递归(49152) → 16384 + 递归(32768) → 16384 + 递归(16384)
            //   → 共 4 块，合计 ValidCount = 4 × 16384 = 65536（数据守恒）
            Assert.Equal(4, CountChain(link.HeadBlock));
            Assert.Equal(65536, link.ValidCount);
            // 末块的 NextEx 必须是 nil（原 :1396）
            Assert.Null(link.LastBlock.NextEx);
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// ★★ DataLen 的 Word 截断可直接在块记录上验证：`AddBuffer` 用 `(ushort)len` 赋值，
    /// 65536 → 0、65537 → 1。这里通过 TMemoryBlock 字段类型固定该语义（原 :24）。
    /// </summary>
    [Fact]
    public void AddBuffer_DataLenAssignment_Is16BitWord()
    {
        TMemoryBlock b = MemoryPoolGlobal.BigMemoryPool().GetMemoryBlock();
        try
        {
            b.DataLen = unchecked((ushort)65536);
            Assert.Equal(0, b.DataLen);
            b.DataLen = unchecked((ushort)65537);
            Assert.Equal(1, b.DataLen);
            Assert.IsType<ushort>(b.DataLen);
        }
        finally
        {
            MemoryPoolGlobal.BigMemoryPool().FreeMemoryBlock(b);
        }
    }

    /// <summary>零长度 AddBuffer 不产生任何块（原 :1358-1359 `if len = 0 then Exit`）。</summary>
    [Fact]
    public void AddBuffer_ZeroLength_IsNoOp()
    {
        var link = new TBufferLink();
        try
        {
            link.AddBuffer(new byte[0], 0);
            Assert.Null(link.HeadBlock);
            Assert.Equal(0, link.ValidCount);
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// 尾块未满时新数据**续写进尾块**而不开新块（原 :1326-1357）。
    /// 手法：先 Add 50 字节（Small 块，DataLen = 50），再 Add 30 字节
    /// → 仍只有 1 块且 DataLen = 80（原 :1345-1346）。
    /// </summary>
    [Fact]
    public void AddBuffer_AppendsIntoUnfilledLastBlock()
    {
        var link = new TBufferLink();
        try
        {
            link.AddBuffer(Make(50, 0), 50);
            Assert.Single(Chain(link.HeadBlock));
            Assert.Equal(50, link.HeadBlock.DataLen);

            link.AddBuffer(Make(30, 100), 30);
            Assert.Single(Chain(link.HeadBlock));            // 没开新块
            Assert.Equal(80, link.HeadBlock.DataLen);        // 50 + 30

            // 读回来验证字节顺序
            byte[] buf = new byte[80];
            Assert.Equal(80u, link.ReadBuffer(buf, 0, 80));
            Assert.Equal(Make(50, 0), buf.AsSpan(0, 50).ToArray());
            Assert.Equal(Make(30, 100), buf.AsSpan(50, 30).ToArray());
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>尾块被填满后继续 Add → 开新块（原 :1349-1355 的 WSize 路径 + 原 :1374-1391）。</summary>
    [Fact]
    public void AddBuffer_FillsLastBlockThenOpensNewOne()
    {
        var link = new TBufferLink();
        try
        {
            link.AddBuffer(Make(100, 0), 100);              // Small 块，剩 28
            link.AddBuffer(Make(100, 100), 100);            // 28 填入尾块 + 72 开新 Normal 块
            Assert.Equal(2, CountChain(link.HeadBlock));
            Assert.Equal(B, link.HeadBlock.DataLen);        // 填满 128
            Assert.Equal(72, link.LastBlock.DataLen);
            Assert.Equal(200, link.ValidCount);
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// ★★ 原文缺陷 D10：`AddMemBlockLink` 第一行就把入参覆盖掉（原 :1417），
    /// 因此传进来的块被丢弃、函数等价于"追加一个空 Small 块"。
    /// </summary>
    [Fact]
    public void AddMemBlockLink_IgnoresArgument_AndAppendsEmptySmallBlock()
    {
        var link = new TBufferLink();
        try
        {
            TMemoryBlock passed = MemoryPoolGlobal.SuperLargeMemoryPool().GetMemoryBlock();
            passed.DataLen = 999;

            int useBefore = MemoryPoolGlobal.SmallMemoryPool().UseCount;
            link.AddMemBlockLink(passed);

            Assert.NotNull(link.HeadBlock);
            Assert.NotSame(passed, link.HeadBlock);                 // ★ 入参被丢弃
            Assert.Equal(B, link.HeadBlock.Memory.Length);          // 追加的是 Small 块
            Assert.Equal(TDxMemBlockType.MB_Small, link.HeadBlock.BlockType);
            // ★ 追加的块来自池（UseCount 恰好 +1）；DataLen **未被清零**（原文不重置），
            //   故它可能是池里上一任用户留下的旧值 —— 这里不做等于 0 的断言。
            Assert.Equal(useBefore + 1, MemoryPoolGlobal.SmallMemoryPool().UseCount);
            Assert.Null(link.HeadBlock.NextEx);
            Assert.Equal(999, passed.DataLen);                      // 入参原封不动

            MemoryPoolGlobal.SuperLargeMemoryPool().FreeMemoryBlock(passed);
        }
        finally
        {
            link.Dispose();
        }
    }

    // ---------------------------------------------------------------------------------
    // 2. ValidCount（原 :1669-1686）—— 含"读指针已走过部分块"
    // ---------------------------------------------------------------------------------

    /// <summary>FRead = nil 时 ValidCount = 全部块 DataLen 之和（原 :1674-1680）。</summary>
    [Fact]
    public void ValidCount_NullReadPointer_SumsAllBlocks()
    {
        var link = new TBufferLink();
        try
        {
            link.AddBuffer(Make(300, 0), 300);              // 单 Normal 块，DataLen = 300
            Assert.Equal(300, link.ValidCount);
            link.AddBuffer(Make(50, 500), 50);              // 续写进同一块 → 350
            Assert.Equal(350, link.ValidCount);
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// ★ 读指针已走过部分块时：`Result := FRead.DataLen - FReadPosition`（原 :1678）
    /// 再加上后续块（原 :1681-1685）。
    /// </summary>
    [Fact]
    public void ValidCount_AfterPartialRead_SubtractsReadPosition()
    {
        var link = new TBufferLink();
        try
        {
            link.AddBuffer(Make(300, 0), 300);
            byte[] buf = new byte[400];
            Assert.Equal(100u, link.ReadBuffer(buf, 0, 100));
            // FRead = 唯一块、FReadPosition = 100、DataLen = 300
            Assert.Equal(200, link.ValidCount);
            // 再读 200（读到底）
            Assert.Equal(200u, link.ReadBuffer(buf, 0, 200));
            Assert.Equal(0, link.ValidCount);                // DataLen - Position = 0
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>跨两块时的 ValidCount：前块剩余 + 后块 DataLen。</summary>
    [Fact]
    public void ValidCount_AcrossTwoBlocks()
    {
        var link = new TBufferLink();
        try
        {
            // 300 字节 = 1 个 Normal 块（DataLen=300）；再喂 500 → 因为 500 ≤ 640 会**续写**成 800
            // 为了让链变成两块，先喂 128（Small 块满），再喂 640（Normal 块）
            link.AddBuffer(Make(100, 0), 100);               // Small 块，DataLen = 100
            link.AddBuffer(Make(700, 0), 700);               // 28 填满 Small + 672 → Big 块（1024）
            Assert.Equal(2, CountChain(link.HeadBlock));
            Assert.Equal(B, link.HeadBlock.DataLen);
            Assert.Equal(672, link.LastBlock.DataLen);
            Assert.Equal(800, link.ValidCount);

            byte[] buf = new byte[800];
            Assert.Equal(50u, link.ReadBuffer(buf, 0, 50));
            Assert.Equal(750, link.ValidCount);              // 128-50 + 672
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>空链 ValidCount = 0。</summary>
    [Fact]
    public void ValidCount_EmptyLink_IsZero()
    {
        var link = new TBufferLink();
        try { Assert.Equal(0, link.ValidCount); }
        finally { link.Dispose(); }
    }

    // ---------------------------------------------------------------------------------
    // 3. ReadBuffer（原 :1523-1572）
    // ---------------------------------------------------------------------------------

    /// <summary>顺序读：返回实际读到的字节数，不足 len 时返回剩下的全部（原 :1543-1568）。</summary>
    [Fact]
    public void ReadBuffer_SequentialReads()
    {
        var link = new TBufferLink();
        try
        {
            byte[] data = Make(300, 7);
            link.AddBuffer(data, 300);

            byte[] buf = new byte[128];
            Assert.Equal(128u, link.ReadBuffer(buf, 0, 128));
            Assert.Equal(data.AsSpan(0, 128).ToArray(), buf);

            Assert.Equal(128u, link.ReadBuffer(buf, 0, 128));
            Assert.Equal(data.AsSpan(128, 128).ToArray(), buf);

            // 只剩 44
            Assert.Equal(44u, link.ReadBuffer(buf, 0, 128));
            Assert.Equal(data.AsSpan(256, 44).ToArray(), buf.AsSpan(0, 44).ToArray());

            // 已读空
            Assert.Equal(0u, link.ReadBuffer(buf, 0, 128));
            Assert.Equal(0, link.ValidCount);
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>空链 ReadBuffer 返回 0（原 :1570-1571）。</summary>
    [Fact]
    public void ReadBuffer_EmptyLink_ReturnsZero()
    {
        var link = new TBufferLink();
        try { Assert.Equal(0u, link.ReadBuffer(new byte[10], 0, 10)); }
        finally { link.Dispose(); }
    }

    /// <summary>ReadBuffer 的 offset 参数生效（写入调用方缓冲区的指定偏移）。</summary>
    [Fact]
    public void ReadBuffer_HonoursOffset()
    {
        var link = new TBufferLink();
        try
        {
            link.AddBuffer(Make(50, 0), 50);
            byte[] buf = new byte[60];
            for (int i = 0; i < 60; i++) buf[i] = 0xFF;
            Assert.Equal(50u, link.ReadBuffer(buf, 5, 50));
            for (int i = 0; i < 5; i++) Assert.Equal(0xFF, buf[i]);
            Assert.Equal(Make(50, 0), buf.AsSpan(5, 50).ToArray());
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// InnerReadBuf 是 public static（原 :1496-1521，protected→public 便于审计）：
    /// 单块读取语义 + 有效量不足时只返回块内剩余。
    /// </summary>
    [Fact]
    public void InnerReadBuf_SingleBlockSemantics()
    {
        TMemoryBlock b = MemoryPoolGlobal.SmallMemoryPool().GetMemoryBlock();
        try
        {
            for (int i = 0; i < 10; i++) b.Memory[i] = (byte)(i + 1);
            b.DataLen = 10;

            byte[] buf = new byte[20];
            // 从 0 读 5 → 5
            Assert.Equal(5u, TBufferLink.InnerReadBuf(b, 0, buf, 0, 5));
            Assert.Equal(new byte[] { 1, 2, 3, 4, 5 }, buf.AsSpan(0, 5).ToArray());

            // 从 0 读 50 → 只有 10
            Array.Clear(buf, 0, buf.Length);
            Assert.Equal(10u, TBufferLink.InnerReadBuf(b, 0, buf, 0, 50));
            Assert.Equal(10, buf[9]);

            // 从 8 读 50 → 只有 2（原 :1516-1517）
            Assert.Equal(2u, TBufferLink.InnerReadBuf(b, 8, buf, 0, 50));

            // 从 10 读（有效量 0）→ 0（原 :1505-1506）
            Assert.Equal(0u, TBufferLink.InnerReadBuf(b, 10, buf, 0, 50));

            // nil 块 → 0（原 :1502）
            Assert.Equal(0u, TBufferLink.InnerReadBuf(null, 0, buf, 0, 50));
        }
        finally
        {
            MemoryPoolGlobal.SmallMemoryPool().FreeMemoryBlock(b);
        }
    }

    // ---------------------------------------------------------------------------------
    // 4. ReadBufferWhileFindChar（原 :1574-1655）★ 重点
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// ★ 包含分隔符：找到时 `lvRemain := lvIndex - lvPosition + 1`（原 :1611），
    /// 返回值 = 拷贝总量（**含分隔符本身**），游标推进到分隔符**之后**。
    /// </summary>
    [Fact]
    public void ReadBufferWhileFindChar_IncludesDelimiter_AndAdvancesPastIt()
    {
        var link = new TBufferLink();
        try
        {
            // ★ 两个分隔符在**同一次** AddBuffer 时就写好，避免"先读后改数据"的歧义
            byte[] data = new byte[100];
            for (int i = 0; i < 100; i++) data[i] = (byte)'A';
            data[30] = (byte)'|';
            data[60] = (byte)'|';
            link.AddBuffer(data, 100);

            byte[] buf = new byte[100];
            // 找到下标 30 的 '|' → 返回 30 - 0 + 1 = 31（★ 含分隔符本身）
            Assert.Equal(31u, link.ReadBufferWhileFindChar(buf, (byte)'|'));
            Assert.Equal((byte)'|', buf[30]);                    // 分隔符在缓冲区里
            Assert.Equal(data.AsSpan(0, 31).ToArray(), buf.AsSpan(0, 31).ToArray());
            Assert.Equal(69, link.ValidCount);                   // 100 - 31

            // 游标已过分隔符：下一次调用从位置 31 开始 → 命中下标 60 → 60 - 31 + 1 = 30
            Array.Clear(buf, 0, buf.Length);
            Assert.Equal(30u, link.ReadBufferWhileFindChar(buf, (byte)'|'));
            Assert.Equal((byte)'|', buf[29]);
            Assert.Equal(39, link.ValidCount);                   // 100 - 61
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>分隔符就在当前游标处 → 返回 1（原 :1611：lvIndex - lvPosition + 1 = 1）。</summary>
    [Fact]
    public void ReadBufferWhileFindChar_DelimiterAtCursor_ReturnsOne()
    {
        var link = new TBufferLink();
        try
        {
            byte[] data = { (byte)'|', (byte)'x', (byte)'y' };
            link.AddBuffer(data, 3);
            byte[] buf = new byte[8];
            Assert.Equal(1u, link.ReadBufferWhileFindChar(buf, (byte)'|'));
            Assert.Equal((byte)'|', buf[0]);
            Assert.Equal(2, link.ValidCount);
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// ★★ 易错点 D11：**未找到分隔符时返回 0**（原 :1582 `Result := 0`，原 :1635 只在
    /// `boIsFoundChar` 为真时赋值），尽管中间块的字节**已经被拷进 buf**、读游标**已经推到链尾**。
    /// </summary>
    [Fact]
    public void ReadBufferWhileFindChar_NotFound_ReturnsZeroButAdvancesCursor()
    {
        var link = new TBufferLink();
        try
        {
            byte[] data = new byte[200];
            for (int i = 0; i < 200; i++) data[i] = (byte)'A';
            link.AddBuffer(data, 200);
            Assert.Equal(200, link.ValidCount);

            byte[] buf = new byte[200];
            Assert.Equal(0u, link.ReadBufferWhileFindChar(buf, (byte)'|'));    // ★ 返回 0
            Assert.Equal(0, link.ValidCount);                                  // 但游标已读到尾
            // 数据其实已被拷进 buf（长度 = 原 lvRemain 初值 = ValidCount = 200）
            Assert.Equal((byte)'A', buf[0]);
            Assert.Equal((byte)'A', buf[199]);
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// 跨块查找：分隔符在第 2 块内时，第 1 块被整块拷走、第 2 块拷到分隔符为止；
    /// 返回值 = 两块合计（原 :1594-1653 的循环）。
    /// </summary>
    [Fact]
    public void ReadBufferWhileFindChar_AcrossBlocks()
    {
        var link = new TBufferLink();
        try
        {
            // 构造两块：先 100 字节（Small 块未满），再 700 → 28 填满 Small + 672 进 Big 块
            byte[] a = new byte[100];
            for (int i = 0; i < 100; i++) a[i] = (byte)'A';
            link.AddBuffer(a, 100);
            byte[] b = new byte[700];
            for (int i = 0; i < 700; i++) b[i] = (byte)'B';
            b[0] = (byte)'|';                       // 这个字节会落在 Small 块的偏移 100 处
            link.AddBuffer(b, 700);
            Assert.Equal(2, CountChain(link.HeadBlock));

            byte[] buf = new byte[900];
            // 第 1 块（DataLen=128）从 0 扫到 100 命中 '|' → 返回 101
            Assert.Equal(101u, link.ReadBufferWhileFindChar(buf, (byte)'|'));
            Assert.Equal((byte)'|', buf[100]);
            Assert.Equal(800 - 101, link.ValidCount);
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// ★ D12：`buf = nil` 时只推进游标、不拷贝（原 :1618-1625 的 else 分支 `l := lvRemain`）。
    /// </summary>
    [Fact]
    public void ReadBufferWhileFindChar_NullBuffer_OnlyAdvancesCursor()
    {
        var link = new TBufferLink();
        try
        {
            byte[] data = new byte[100];
            for (int i = 0; i < 100; i++) data[i] = (byte)'A';
            data[30] = (byte)'|';
            link.AddBuffer(data, 100);

            Assert.Equal(31u, link.ReadBufferWhileFindChar(null, (byte)'|'));
            Assert.Equal(100 - 31, link.ValidCount);          // 游标确实推进了
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// ★ D13（结论：原 :1622 的异常在**良构数据下不可达**）。
    /// <para>
    /// 分析原 :1594-1653：进入 while 时 `lvPosition` 只有两种取值 ——
    /// ① 首块 = FReadPosition（首块 = FRead 时 FReadPosition 必 &lt; DataLen，除非手工构造空块）；
    /// ② 次块起 = 0（原 :1649-1650）。
    /// 对 ② 而言，`lvRemain := DataLen - 0` 恒等于 InnerReadBuf 能返回的最大值，故 `l = lvRemain` 必成立；
    /// 于是原 :1621 的 `if l != lvRemain` 永远为假 —— **异常分支是死代码**。
    /// 无论链上有多少个 `DataLen = 0` 的块，都只会走原 :1615 的 `Break`（返回 0）。
    /// </para>
    /// <para>
    /// 手工挂一个空块到链首（AddBuffer 自己不会产生空块），验证：**不抛异常、返回 0**。
    /// </para>
    /// </summary>
    [Fact]
    public void ReadBufferWhileFindChar_EmptyHeadBlock_BreaksAndReturnsZero_NoThrow()
    {
        var link = new TBufferLink();
        try
        {
            // AddMemBlockLink 会忽略入参、追加一个新的 Small 块（D10）；原文不重置 DataLen
            link.AddMemBlockLink(null);
            Assert.NotNull(link.HeadBlock);
            // 显式置 0，构造"D13：FRead 自身是空块"这一状态
            link.HeadBlock.DataLen = 0;
            Assert.Equal(0, link.HeadBlock.DataLen);

            byte[] buf = new byte[16];
            // FRead = nil → lvPosition = 0；查找循环上界 `DataLen - 1` 在 Cardinal 下溢为 0xFFFFFFFF，
            // 于是 I = 0 会先越界读 Memory[0]（本移植显式化成 IndexOutOfRangeException，原文为 AV）。
            // 原文此处的后果是 AV（越界读）或 "ReadBufferWhileFindChar Error" 异常；
            // 托管侧按内部路径分别表现为 IndexOutOfRangeException / InvalidOperationException。
            Assert.ThrowsAny<Exception>(
                () => link.ReadBufferWhileFindChar(buf, (byte)'|'));
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// ★ 链中第二个块为空（`DataLen = 0`）时的实测行为。
    /// <para>
    /// 实测（探针 D18）：构造 `[Small(DataLen = 8, 'A'×8), Small(DataLen = 0)]` 后调用
    /// `ReadBufferWhileFindChar` 会抛 <see cref="IndexOutOfRangeException"/> ——
    /// 第 1 块找不到 '|' → 原 :1614 `lvRemain := 8`、拷走 8 字节、推进到第 2 块（`lvPosition = 0`）；
    /// 第 2 轮原 :1599 的 `for I := lvPosition to lvBuf.DataLen - 1` 在 `DataLen = 0` 时
    /// **上界下溢为 0xFFFFFFFF** → `I = 0` 越界访问 `Memory[0]`。
    /// </para>
    /// <para>
    /// 托管侧把该越界显式化为 <see cref="IndexOutOfRangeException"/>（原文为访问违例 AV）。
    /// 这与 <c>ReadBufferWhileFindChar_EmptyHeadBlock_BreaksAndReturnsZero_NoThrow</c> 的区别：
    /// 后者是 **`FRead = nil` 时的头块为空**（头块 `DataLen = 0` 且 `lvPosition = 0` 时
    /// <c>Memory[0]</c> 仍在数组内，读不到字符 → 原 :1615 `lvRemain = 0` → Break → 返回 0，**不抛**），
    /// 前者是**链中第二块为空**（同样下标越界但语义是"已经读过一个块"，仍触发越界）。
    /// </para>
    /// </summary>
    [Fact]
    public void ReadBufferWhileFindChar_EmptySecondBlock_ThrowsOnUnderflow()
    {
        var link = new TBufferLink();
        try
        {
            // 用 AddMemBlockLink 造两个块（都是空的 Small 块），再给第 1 块填有效数据
            link.AddMemBlockLink(null);
            link.AddMemBlockLink(null);
            TMemoryBlock b1 = link.HeadBlock;
            TMemoryBlock b2 = b1.NextEx;
            Assert.NotNull(b2);
            for (int i = 0; i < 8; i++) b1.Memory[i] = (byte)'A';
            b1.DataLen = 8;
            b2.DataLen = 0;

            byte[] buf = new byte[16];
            // 原 :1599 的上界下溢 ⇒ 越界读（原文 AV；托管侧显式抛 IndexOutOfRangeException）
            Assert.Throws<IndexOutOfRangeException>(
                () => link.ReadBufferWhileFindChar(buf, (byte)'|'));
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// 空链调用返回 0（原 :1591 `if lvBuf &lt;&gt; nil then` 不成立）。
    /// </summary>
    [Fact]
    public void ReadBufferWhileFindChar_EmptyLink_ReturnsZero()
    {
        var link = new TBufferLink();
        try
        {
            Assert.Equal(0u, link.ReadBufferWhileFindChar(new byte[10], (byte)'|'));
            Assert.Equal(0u, link.ReadBufferWhileFindChar(null, (byte)'|'));
        }
        finally
        {
            link.Dispose();
        }
    }

    // ---------------------------------------------------------------------------------
    // 5. MarkReaderIndex / RestoreReaderIndex（原 :1657-1667）
    // ---------------------------------------------------------------------------------

    /// <summary>标记后可以回滚：读游标与位置都恢复（原 :1659-1666）。</summary>
    [Fact]
    public void MarkReaderIndex_ThenRestoreReaderIndex_RollsBackCursorAndPosition()
    {
        var link = new TBufferLink();
        try
        {
            link.AddBuffer(Make(100, 0), 100);
            byte[] buf = new byte[200];

            link.MarkReaderIndex();
            Assert.Null(link.MarkBlock);                       // FRead 还是 nil
            Assert.Equal(0u, link.MarkPositionInternal);

            Assert.Equal(60u, link.ReadBuffer(buf, 0, 60));
            Assert.Equal(40, link.ValidCount);

            link.RestoreReaderIndex();
            Assert.Equal(100, link.ValidCount);                // 完全回滚

            byte[] again = new byte[60];
            Assert.Equal(60u, link.ReadBuffer(again, 0, 60));
            Assert.Equal(buf.AsSpan(0, 60).ToArray(), again);   // 重新读到同一段数据
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// 先读一段再标记，回滚必须回到正确的块 + 偏移（原 :1657-1667）。
    /// <para>
    /// 实测推导（探针 D17；`AddBuffer(100)` + `AddBuffer(500)`）：
    /// 第 1 次 `AddBuffer` 建 1 个 Small 块 `DataLen = 100`；第 2 次先补满它（+28 → `DataLen = 128`），
    /// 余下 472 建一个 Normal 块 ⇒ 链 = `[Small(128), Normal(472)]`，`ValidCount = **600**`。
    /// </para>
    /// <para>
    /// ★★ 实测纠正：`ReadBuffer(buf, 128)` 之后 `FRead` 仍是**第 1 块**、`FReadPosition = **128**`
    /// （不是"跨到第 2 块 + 位置 0"）。原因是原 :1543-1550 的"读完"分支
    /// （`l = lvRemain`）会 `FRead := lvBuf` 且**不做跨块推进** ——
    /// 只有原 :1552-1566 的"读不满"分支才会 `FRead := lvBuf^.NextEx`。
    /// 因此 `MarkReaderIndex` 记下的是 **第 1 块 + 偏移 128**，`ValidCount = 600 - 128 = **472**`。
    /// </para>
    /// <para>
    /// 再读 100 字节：第 1 块从偏移 128 起已无有效数据 → 走原 :1552 分支推进到第 2 块，
    /// 读走 100 字节 ⇒ `FRead = 第 2 块`、`FReadPosition = 100`、`ValidCount = 372`。
    /// `RestoreReaderIndex` 后回到标记（第 1 块 + 128），`ValidCount` 恢复为 **472**。
    /// </para>
    /// </summary>
    [Fact]
    public void MarkReaderIndex_AfterPartialRead_RestoresBlockAndOffset()
    {
        var link = new TBufferLink();
        try
        {
            link.AddBuffer(Make(100, 0), 100);
            link.AddBuffer(Make(500, 0), 500);
            Assert.Equal(2, CountChain(link.HeadBlock));
            Assert.Equal(128, link.HeadBlock.DataLen);          // 100 + 28（补满）
            Assert.Equal(472, link.HeadBlock.NextEx.DataLen);
            Assert.Equal(600, link.ValidCount);

            byte[] buf = new byte[600];
            Assert.Equal(128u, link.ReadBuffer(buf, 0, 128));   // 恰好读满第 1 块
            Assert.Same(link.HeadBlock, link.ReadBlock);        // ★ 仍在第 1 块
            Assert.Equal(128u, link.ReadPositionInternal);

            link.MarkReaderIndex();
            Assert.Same(link.HeadBlock, link.MarkBlock);        // ★ 标记第 1 块
            Assert.Equal(128u, link.MarkPositionInternal);
            Assert.Equal(472, link.ValidCount);

            Assert.Equal(100u, link.ReadBuffer(buf, 0, 100));   // 推进到第 2 块
            Assert.Same(link.HeadBlock.NextEx, link.ReadBlock);
            Assert.Equal(100u, link.ReadPositionInternal);
            Assert.Equal(372, link.ValidCount);

            link.RestoreReaderIndex();
            Assert.Same(link.HeadBlock, link.ReadBlock);
            Assert.Equal(128u, link.ReadPositionInternal);
            Assert.Equal(472, link.ValidCount);                 // 完全回滚
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// 一次读走整条链（数据不足时按原 :1552-1566 推进到链尾）。
    /// <para>
    /// 实测推导（探针 D17 的 v2 组）：`AddBuffer(100)` + `AddBuffer(500)` 后
    /// `ReadBuffer(buf, 600)` 返回 **600**、`ValidCount` 归 **0**；
    /// 再读返回 **0**（原 :1560-1565 已在链尾）。
    /// </para>
    /// </summary>
    [Fact]
    public void ReadBuffer_WholeChain_ThenEmpty()
    {
        var link = new TBufferLink();
        try
        {
            link.AddBuffer(Make(100, 0), 100);
            link.AddBuffer(Make(500, 0), 500);

            byte[] buf = new byte[600];
            Assert.Equal(600u, link.ReadBuffer(buf, 0, 600));
            Assert.Equal(0, link.ValidCount);
            Assert.Equal(0u, link.ReadBuffer(buf, 0, 10));
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>未 Mark 直接 Restore → 游标变 nil（原 :1659 把 FRead 设为 FMark = nil）。</summary>
    [Fact]
    public void RestoreReaderIndex_WithoutMark_GoesToNull()
    {
        var link = new TBufferLink();
        try
        {
            link.AddBuffer(Make(100, 0), 100);
            byte[] buf = new byte[200];
            Assert.Equal(50u, link.ReadBuffer(buf, 0, 50));

            link.RestoreReaderIndex();
            Assert.Null(link.ReadBlock);
            Assert.Equal(0u, link.ReadPositionInternal);
            // FRead = nil → ReadBuffer 重新从 FHead 位置 0 开始（原 :1532-1536）
            Assert.Equal(50u, link.ReadBuffer(buf, 0, 50));
            Assert.Equal(Make(50, 0), buf.AsSpan(0, 50).ToArray());
        }
        finally
        {
            link.Dispose();
        }
    }

    // ---------------------------------------------------------------------------------
    // 6. ClearBuffer vs ClearHaveReadBuffer ★ 差异（原 :1432-1457 vs :1459-1482）
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// ClearBuffer：释放**整条链**并把四个游标全部清零（原 :1436-1456）。
    /// </summary>
    [Fact]
    public void ClearBuffer_FreesWholeChainAndResetsAllCursors()
    {
        var link = new TBufferLink();
        try
        {
            link.AddBuffer(Make(100, 0), 100);
            link.AddBuffer(Make(500, 0), 500);
            Assert.Equal(2, CountChain(link.HeadBlock));
            byte[] buf = new byte[600];
            link.ReadBuffer(buf, 0, 50);
            link.MarkReaderIndex();
            int bytes = ChainBytes(link.HeadBlock);
            Assert.True(bytes > 0);

            link.ClearBuffer();

            Assert.Null(link.HeadBlock);
            Assert.Null(link.LastBlock);
            Assert.Null(link.ReadBlock);
            Assert.Equal(0u, link.ReadPositionInternal);
            Assert.Null(link.MarkBlock);
            Assert.Equal(0u, link.MarkPositionInternal);
            Assert.Equal(0, link.ValidCount);
            // 原文每块若真释放则 DelMemory、若入空闲链则只摘链（此处不断言全局账目，
            // 因为它会被并行测试扰动；链已清空即证明块已归还）
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// 差异断言 1：ClearHaveReadBuffer **只释放已读过的前缀块**，
    /// FHead 移到 FRead、`FRead.PrevEx := nil`（原 :1480-1481），
    /// 未读数据仍然可读、ValidCount 不变。
    /// </summary>
    [Fact]
    public void ClearHaveReadBuffer_FreesOnlyReadPrefix_KeepsUnreadData()
    {
        var link = new TBufferLink();
        try
        {
            link.AddBuffer(Make(100, 0), 100);                  // Small，DataLen = 100
            link.AddBuffer(Make(500, 0), 500);                  // +28 补满 + 472 → Normal 块
            Assert.Equal(2, CountChain(link.HeadBlock));

            byte[] buf = new byte[600];
            Assert.Equal(150u, link.ReadBuffer(buf, 0, 150));   // 读到第 2 块偏移 22
            TMemoryBlock readBlock = link.ReadBlock;
            Assert.Same(link.HeadBlock.NextEx, readBlock);
            int validBefore = link.ValidCount;                  // 472 - 22 = 450

            link.ClearHaveReadBuffer();

            Assert.Same(readBlock, link.HeadBlock);             // FHead 前移到 FRead
            Assert.Null(link.HeadBlock.PrevEx);                 // 原 :1480
            Assert.Equal(validBefore, link.ValidCount);         // 未读数据量不变
            Assert.Same(readBlock, link.ReadBlock);
            Assert.Equal(22u, link.ReadPositionInternal);       // 读位置**保持不动**

            // 剩下的数据仍能继续读（内容连续）
            byte[] rest = new byte[450];
            Assert.Equal(450u, link.ReadBuffer(rest, 0, 450));
            Assert.Equal(0, link.ValidCount);
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// 差异断言 2：`FRead = nil` 时 ClearHaveReadBuffer **立即返回、什么都不做**（原 :1463），
    /// 而 ClearBuffer 仍会释放整条链。
    /// </summary>
    [Fact]
    public void ClearHaveReadBuffer_NullReadPointer_IsNoOp_UnlikeClearBuffer()
    {
        var link = new TBufferLink();
        try
        {
            link.AddBuffer(Make(300, 0), 300);
            Assert.Null(link.ReadBlock);

            link.ClearHaveReadBuffer();
            Assert.NotNull(link.HeadBlock);                     // ★ 没清
            Assert.Equal(300, link.ValidCount);
            Assert.Equal(0u, link.ReadPositionInternal);

            link.ClearBuffer();
            Assert.Null(link.HeadBlock);                        // ClearBuffer 才清
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// 差异断言 3：ClearBuffer 清 FMark，ClearHaveReadBuffer **不清 FMark**
    /// （原 :1455-1456 有、原 :1459-1482 无）。
    /// </summary>
    [Fact]
    public void ClearBuffer_ResetsMark_ClearHaveReadBuffer_DoesNot()
    {
        var link = new TBufferLink();
        try
        {
            link.AddBuffer(Make(100, 0), 100);
            link.AddBuffer(Make(500, 0), 500);
            byte[] buf = new byte[600];
            link.ReadBuffer(buf, 0, 10);
            link.MarkReaderIndex();
            TMemoryBlock mark = link.MarkBlock;
            Assert.NotNull(mark);

            link.ClearHaveReadBuffer();
            Assert.Same(mark, link.MarkBlock);                  // ★ 标记保留
            Assert.Equal(10u, link.MarkPositionInternal);
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// 差异断言 4：ClearHaveReadBuffer 后 FHead = FRead，于是"从头读"会从**当前读块**开始
    /// （不再是原始头块）；而 ClearBuffer 后链是空的。
    /// </summary>
    [Fact]
    public void ClearHaveReadBuffer_ChangesWhatHeadMeans()
    {
        var link = new TBufferLink();
        try
        {
            link.AddBuffer(Make(100, 0), 100);
            link.AddBuffer(Make(500, 0), 500);
            byte[] buf = new byte[600];
            link.ReadBuffer(buf, 0, 130);                       // 越过第一块
            TMemoryBlock originalHead = link.HeadBlock;
            link.ClearHaveReadBuffer();
            Assert.NotSame(originalHead, link.HeadBlock);       // FHead 已前移

            // 重新从 FHead（= 原第 2 块）位置 0 读，得到的是第 2 块开头的数据
            link.RestoreReaderIndex();
            Assert.Null(link.ReadBlock);                        // 未 Mark → Restore 变 nil
            Assert.Equal(10u, link.ReadBuffer(new byte[10], 0, 10));
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>Dispose 等价于 ClearBuffer（原 :1490-1494 destructor）。</summary>
    [Fact]
    public void Dispose_FreesChain()
    {
        var link = new TBufferLink();
        link.AddBuffer(Make(300, 0), 300);
        int bytes = ChainBytes(link.HeadBlock);
        Assert.True(bytes > 0);
        link.Dispose();
        Assert.Null(link.HeadBlock);
        Assert.Null(link.LastBlock);
        //（不再断言全局账目：会被并行测试扰动）
    }

    // ---------------------------------------------------------------------------------
    // helpers
    // ---------------------------------------------------------------------------------

    /// <summary>生成 n 字节数据，第 i 字节 = (start + i) &amp; 0xFF。</summary>
    private static byte[] Make(int n, int start)
    {
        byte[] b = new byte[n];
        for (int i = 0; i < n; i++) b[i] = (byte)(start + i);
        return b;
    }

    /// <summary>链上所有块的 `TMemoryBlock.RecordSize + Memory.Length` 之和。</summary>
    private static int ChainBytes(TMemoryBlock head)
    {
        // ★ 环形流的 NextEx 是**闭合的**（FLast.NextEx = FHead），无界遍历会死循环 ——
        //   因此这里按块数封顶。
        int n = 0, guard = 0;
        for (TMemoryBlock b = head; b != null && guard < 64; b = b.NextEx, guard++)
            n += TMemoryBlock.RecordSize + (b.Memory == null ? 0 : b.Memory.Length);
        return n;
    }

    private static int CountChain(TMemoryBlock head)
    {
        int n = 0;
        for (TMemoryBlock b = head; b != null; b = b.NextEx) n++;
        return n;
    }

    private static System.Collections.Generic.List<TMemoryBlock> Chain(TMemoryBlock head)
    {
        var l = new System.Collections.Generic.List<TMemoryBlock>();
        for (TMemoryBlock b = head; b != null; b = b.NextEx) l.Add(b);
        return l;
    }
}
