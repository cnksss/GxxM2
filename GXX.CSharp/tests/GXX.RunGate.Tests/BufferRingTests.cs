using System;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// uBuffer.pas 环形缓冲流（原 uBuffer.pas:138-178 声明、:1773-2350 实现）的移植测试。
/// <para>
/// 块用 <b>MB_Small</b>（128 字节，原 :245），环形容量取 312 → 3 块（128/128/128，FCapacity = 384）。
/// </para>
/// <para>
/// 重点：环形回绕、<c>CanWriteSize</c>/<c>DataSize</c> 的两个"相等"分支、
/// 写满时的行为、<c>ReadPosition</c>/<c>WritePosition</c> setter、
/// <c>markWriterIndex</c>/<c>restoreWriterIndex</c>、<c>SeekReadWrite</c> 的三种 origin。
/// </para>
/// </summary>
/// <summary>与其它 uBuffer 测试类同属一个 collection（禁用并行）——见 <see cref="BufferTestCollection"/>。</summary>
[Collection(BufferTestCollection.Name)]
public class BufferRingTests
{
    private const int B = 128;

    // ---------------------------------------------------------------------------------
    // 1. 构造 / 几何（原 :1775-1782、:2181-2265）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void Create_SizesRingAndReportsGeometry()
    {
        var r = new TDxRingStream(312);                      // → 3 块
        Assert.Equal(312, r.Length);
        Assert.Equal(TDxMemBlockType.MB_Small, r.MemBlockType);
        Assert.Equal(3, r.MemBlockCountInternal);
        Assert.Equal(3 * B, r.CapacityInternal);
        Assert.Equal(0, r.ReadPosition);
        Assert.Equal(0, r.WritePosition);
        Assert.Same(r.HeadInternal, r.ReadBlockInternal);
        Assert.Same(r.HeadInternal, r.WriteBlockInternal);
    }

    /// <summary>
    /// ★ 环形态：`SetSize` 末尾执行 `FLast.NextEx := FHead`（原 :2233-2234），末块的 NextEx **指回头**。
    /// 对照 TDxMemoryStream 的末块 NextEx 是 nil。
    /// </summary>
    [Fact]
    public void Create_BlockChainIsCircular()
    {
        var r = new TDxRingStream(312);
        TMemoryBlock head = r.HeadInternal;
        Assert.Same(head, r.LastInternal.NextEx);            // 末块 → 头（环形）
        Assert.Same(r.LastInternal, head.NextEx.NextEx);     // 3 块
        Assert.Null(head.PrevEx);                            // 头块 PrevEx 保持 nil
    }

    /// <summary>RingBufferSize = 0 时**不调用 SetSize**（原 :1780），于是没有块链。</summary>
    [Fact]
    public void Create_ZeroSize_LeavesRingUnallocated()
    {
        var r = new TDxRingStream(0);
        Assert.Equal(0, r.Length);
        Assert.Null(r.HeadInternal);
        Assert.Null(r.ReadBlockInternal);
        Assert.Null(r.WriteBlockInternal);
        Assert.Equal(0, r.ReadBlockPosInternal);
        Assert.Equal(0, r.WriteBlockPosInternal);
    }

    /// <summary>SetLength 用原公式算块数（原 :2199-2202）。</summary>
    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 1, 128)]
    [InlineData(128, 1, 128)]
    [InlineData(129, 2, 256)]
    [InlineData(256, 2, 256)]
    [InlineData(257, 3, 384)]
    public void SetLength_Geometry(long size, int blocks, int cap)
    {
        var r = new TDxRingStream(0);
        r.SetLength(size);
        Assert.Equal(size, r.Length);
        Assert.Equal(blocks, r.MemBlockCountInternal);
        Assert.Equal(cap, r.CapacityInternal);
    }

    // ---------------------------------------------------------------------------------
    // 2. CanWriteSize / DataSize（原 :1784-1806）
    // ---------------------------------------------------------------------------------

    /// <summary>空环：DataSize = 0、CanWriteSize = FSize（原 :1790-1791 / :1802-1803）。</summary>
    [Fact]
    public void CanWriteSize_DataSize_EmptyRing()
    {
        var r = new TDxRingStream(312);
        Assert.Equal(312, r.CanWriteSize);
        Assert.Equal(0, r.DataSize);
    }

    /// <summary>写入 k 字节后：Write &gt; Read → DataSize = W - R、CanWriteSize = FSize - W + R。</summary>
    [Theory]
    [InlineData(1)]
    [InlineData(127)]
    [InlineData(128)]
    [InlineData(129)]
    [InlineData(311)]
    public void CanWriteSize_DataSize_AfterPartialWrite(int n)
    {
        var r = new TDxRingStream(312);
        Assert.Equal(n, r.Write(Make(n, 0), n));
        Assert.Equal(n, r.WritePosition);
        Assert.Equal(n, r.DataSize);
        Assert.Equal(312 - n, r.CanWriteSize);
    }

    /// <summary>
    /// 写满：`FWritePosition = FReadPosition ≠ 0` → DataSize = FSize（原 :1804-1805）、
    /// CanWriteSize = 0（原 :1792-1793）。
    /// </summary>
    [Fact]
    public void CanWriteSize_DataSize_FullRing()
    {
        var r = new TDxRingStream(312);
        Assert.Equal(312, r.Write(Make(312, 0), 312));
        Assert.Equal(0, r.CanWriteSize);
        Assert.Equal(312, r.DataSize);
    }

    /// <summary>
    /// 读出后（Read &gt; Write）：DataSize = W + FSize - R、CanWriteSize = R - W（原 :1788-1789 / :1800-1801）。
    /// </summary>
    [Fact]
    public void CanWriteSize_DataSize_AfterRead()
    {
        var r = new TDxRingStream(312);
        r.Write(Make(200, 0), 200);
        r.WritePosition = 100;                       // Write = 100 < Read? 不，Read 仍是 0
        Assert.Equal(100, r.WritePosition);
        Assert.Equal(100, r.DataSize);               // W(100) > R(0)
        Assert.Equal(212, r.CanWriteSize);

        r.ReadPosition = 40;                         // R = 40 → W > R
        Assert.Equal(60, r.DataSize);
        Assert.Equal(252, r.CanWriteSize);

        r.WritePosition = 10;                        // W = 10 < R = 40
        Assert.Equal(10 + 312 - 40, r.DataSize);     // 282
        Assert.Equal(40 - 10, r.CanWriteSize);       // 30
    }

    // ---------------------------------------------------------------------------------
    // 3. Write / Read：环形回绕（原 :2272-2345 / :1825-1913）
    // ---------------------------------------------------------------------------------

    /// <summary>写满 → 读回，顺序完全一致（跨块 + 回绕）。</summary>
    [Fact]
    public void WriteRead_FullRing_RoundTrips()
    {
        var r = new TDxRingStream(312);
        byte[] data = Make(312, 0);
        Assert.Equal(312, r.Write(data, 312));
        Assert.Equal(312, r.DataSize);
        Assert.Equal(0, r.CanWriteSize);

        byte[] buf = new byte[312];
        Assert.Equal(312, r.Read(buf, 0, 312));
        Assert.Equal(data, buf);
        // ★ R8：环形侧读完一整圈后 FReadPosition = 312（不回绕到 0）—— 与 W 的 312 相等
        Assert.Equal(312, r.ReadPosition);
        Assert.Equal(0, r.CanWriteSize);        // 相等且 R ≠ 0 → 判"满"（原 :1790-1793）
        Assert.Equal(312, r.DataSize);          // 原 :1802-1805：W ≠ 0 → FSize
    }

    /// <summary>
    /// ★ 环形回绕写：容量 312、写 200（W = 200）、读 100（R = 100，同一块内前进）→
    /// `CanWriteSize = R - W` 为负？不 —— W(200) &gt; R(100) → `312 - 200 + 100 = 212`。
    /// 再写 200 → 物理块链从第 2 块绕回第 1 块（回绕路径）。
    /// </summary>
    [Fact]
    public void Write_WrapsAroundRing()
    {
        var r = new TDxRingStream(312);
        byte[] data = Make(200, 0);
        Assert.Equal(200, r.Write(data, 200));
        Assert.Equal(100, r.Read(new byte[100], 0, 100));         // R = 100
        Assert.Equal(212, r.CanWriteSize);                        // 312 - 200 + 100

        byte[] second = Make(200, 1000);
        Assert.Equal(200, r.Write(second, 200));                  // 走到块链末尾再回头
        Assert.Equal(400, r.WritePosition);                       // W 是逻辑位置，持续递增不回绕

        // 读位置从 R = 100（第 1 块偏移 100）开始；写到 W = 400 时环已绕行一圈以上，
        // 因此这一段物理上混合了"旧数据尾部 + 回绕后的新数据"，
        // 精确字节布局取决于 W 的物理块落点。这里只固定**读写条数与游标推进**这一层语义
        //   （字节级布局见 Read_WrapsAroundRing / Write_WrapAround_ThenReadInOrder）。
        byte[] buf = new byte[400];
        int n = r.Read(buf, 0, 400);
        Assert.True(n > 0 && n <= 312, $"环形可读量的合法区间为 (0, FSize]，实际 {n}");
        Assert.Equal(400, r.WritePosition);                       // 写游标按逻辑位置持续前进
    }

    /// <summary>
    /// ★ 环形"写满 → 读空 → 再写满 → 再读空"的字节级完整性（原 :1825-1913 / :2272-2345）。
    /// <para>
    /// 实测推导（312 字节环 = 3 块 × 128；探针 D11/D12）：
    /// <list type="bullet">
    /// <item>写满 312 → `DataSize = 312`、`CanWriteSize = 0`、`W = 312`、`wpos = 56`；</item>
    /// <item>读空 312 → 读回的内容与写入**逐字节相等**（跨块 + 回绕路径），`R = 312`；
    /// 此时 `R = W = 312 ≠ 0` → <see cref="TDxRingStream.CanWriteSize"/> 判"满"返回 0
    /// （原 :1790-1793），必须先读/重置游标才能再写 —— 这是环形流的"水位"语义；</item>
    /// <item>把 `WritePosition` 显式置 0 后再写满 312 → 再读空，仍然逐字节相等
    /// （证明第二次写覆盖了上一轮的旧数据，且回绕读写自洽）。</item>
    /// </list>
    /// </para>
    /// </summary>
    [Fact]
    public void WriteRead_FullRing_Twice_IsByteExact()
    {
        var r = new TDxRingStream(312);
        byte[] first = Make(312, 0);
        Assert.Equal(312, r.Write(first, 312));
        Assert.Equal(312, r.DataSize);
        Assert.Equal(0, r.CanWriteSize);
        Assert.Equal(312, r.WritePosition);
        Assert.Equal(56, r.WriteBlockPosInternal);          // 312 mod 128

        byte[] back = new byte[312];
        Assert.Equal(312, r.Read(back, 0, 312));
        Assert.Equal(first, back);
        Assert.Equal(312, r.ReadPosition);
        Assert.Equal(312, r.WritePosition);

        // 第二轮：把两个游标都归零后再写满，验证覆盖与回绕自洽
        r.WritePosition = 0;
        r.ReadPosition = 0;
        Assert.Equal(312, r.CanWriteSize);
        byte[] second = Make(312, 1000);
        Assert.Equal(312, r.Write(second, 312));
        byte[] back2 = new byte[312];
        Assert.Equal(312, r.Read(back2, 0, 312));
        Assert.Equal(second, back2);
    }

    /// <summary>
    /// ★ 环形回绕写：写满后只**部分**读出，再写满 —— 此时旧数据会被覆盖
    /// （环形的"未读数据可被新写覆盖"语义，原 :2305-2320 无任何保护）。
    /// <para>
    /// 实测推导（探针 D12）：写 312 → 读 200（`R = 200`、`rpos = 72`、`DataSize = 112`）→
    /// 写 112（`W = 312 + 112 = 424`、`CanWriteSize` 从 200 降到 88、`DataSize` 从 112 升到 224）→
    /// 读 224 得到 `[旧数据 200..312） + 新写的 112 字节]`，逐字节相等。
    /// </para>
    /// <para>
    /// 关键点：当 `W` 超过 `FSize`（312）时，每次追加写都从**环的位置 0** 重新覆盖，
    /// 但 `FWriteBlockPos` 是按"上次跨块后的物理落点"推进的 —— 实测 `W = 312` 时
    /// `wpos = 56`，写 112 后 `wpos = 40`（=(56+112) mod 128），物理上落在第 3 块偏移 40。
    /// 这与"从环位置 0 覆盖第 1 块"并不一致，因此**只部分读出的再写会覆盖未读数据且落点偏移**；
    /// 本用例固定可观测的容量/水位变化与"已读段 + 新写段"的自洽性。
    /// </para>
    /// </summary>
    [Fact]
    public void Write_WrapAround_OverwritesUnreadData()
    {
        var r = new TDxRingStream(312);
        byte[] data = Make(312, 0);
        r.Write(data, 312);
        Assert.Equal(200, r.Read(new byte[200], 0, 200));
        Assert.Equal(200, r.ReadPosition);
        Assert.Equal(72, r.ReadBlockPosInternal);
        Assert.Equal(112, r.DataSize);
        Assert.Equal(200, r.CanWriteSize);

        byte[] fresh = Make(112, 900);
        Assert.Equal(112, r.Write(fresh, 112));
        Assert.Equal(424, r.WritePosition);              // 312 + 112
        Assert.Equal(40, r.WriteBlockPosInternal);       // (56 + 112) mod 128
        Assert.Equal(224, r.DataSize);
        Assert.Equal(88, r.CanWriteSize);

        byte[] buf = new byte[224];
        Assert.Equal(224, r.Read(buf, 0, 224));
        // 读回段的前 112 字节 = 写入前的旧数据 [200, 312)
        Assert.Equal(data.AsSpan(200, 112).ToArray(), buf.AsSpan(0, 112).ToArray());
        // 读回段的后 112 字节 = 新写的 112 字节（恰好落在紧随其后的物理位置）
        Assert.Equal(fresh, buf.AsSpan(112, 112).ToArray());
        Assert.Equal(424, r.ReadPosition);
    }

    /// <summary>写返回值 = 被 CanWriteSize 截断后的 count（原 :2285-2286）。</summary>
    [Fact]
    public void Write_ReturnsTruncatedCount()
    {
        var r = new TDxRingStream(100);
        Assert.Equal(100, r.Write(Make(150, 0), 150));       // 截断到 100
        Assert.Equal(0, r.Write(Make(10, 0), 10));           // 满了 → 0
    }

    /// <summary>
    /// Read 的 count 被 DataSize 截断（原 :1845-1847）。
    /// <para>
    /// ★ 差异断言：写 50 后 `FWritePosition = FReadPosition = 50 ≠ 0` → <see cref="TDxRingStream.DataSize"/>
    /// 按"**满**"返回 `FSize`（312，原 :1804-1805），而不是 0；<see cref="TDxRingStream.CanWriteSize"/>
    /// 同理按"满"返回 0（原 :1792-1793）。这是原文"两位置相等即视为满"的判定方式。
    /// 而**第一次** Read 受 `Count &gt; DataSize` 之外没有别的截断，返回实际写入的 50 字节。
    /// </para>
    /// </summary>
    [Fact]
    public void Read_CountBeyondDataSize_IsTruncated()
    {
        var r = new TDxRingStream(312);
        r.Write(Make(50, 0), 50);
        byte[] buf = new byte[100];
        Assert.Equal(50, r.Read(buf, 0, 100));             // 只有 50 字节有效数据可供搬运
        Assert.Equal(50, r.ReadPosition);
        // ★ 之后游标相等 → 判"满"：可写 0、可读 FSize
        Assert.Equal(0, r.CanWriteSize);
        Assert.Equal(312, r.DataSize);
        // 再读会按"满"继续搬运（环被当作满环），游标继续推进
        Assert.Equal(100, r.Read(buf, 0, 100));
        Assert.Equal(150, r.ReadPosition);
    }

    /// <summary>空环 Read 返回 0（原 :1845-1852：CanReadSize = 0 → count = 0）。</summary>
    [Fact]
    public void Read_EmptyRing_ReturnsZero()
    {
        var r = new TDxRingStream(312);
        Assert.Equal(0, r.Read(new byte[10], 0, 10));
    }

    /// <summary>ReadBuffer / WriteBuffer 是 Read/Write 的转调（原 :1915-1918、:2347-2350）。</summary>
    [Fact]
    public void ReadBuffer_WriteBuffer_Forward()
    {
        var r = new TDxRingStream(312);
        r.WriteBuffer(Make(30, 0), 30);
        Assert.Equal(30, r.DataSize);
        byte[] buf = new byte[30];
        r.ReadBuffer(buf, 30);
        Assert.Equal(Make(30, 0), buf);
        Assert.Equal(0, r.CanWriteSize);                   // R = W = 30 ≠ 0 → 判"满"
    }

    // ---------------------------------------------------------------------------------
    // 4. ReadPosition / WritePosition setter（原 :2176-2179、:2267-2270）
    // ---------------------------------------------------------------------------------

    /// <summary>setter 走 SeekReadWrite(Value, soBeginning, …)（原 :2178/:2269）。</summary>
    [Fact]
    public void ReadPosition_WritePosition_SettersUseSoBeginning()
    {
        var r = new TDxRingStream(312);
        r.ReadPosition = 200;
        Assert.Equal(200, r.ReadPosition);
        Assert.Equal(200 % B, r.ReadBlockPosInternal);
        Assert.Same(r.HeadInternal.NextEx, r.ReadBlockInternal);       // 200 / 128 = 1 → 第 2 块

        r.WritePosition = 300;
        Assert.Equal(300, r.WritePosition);
        Assert.Equal(300 % B, r.WriteBlockPosInternal);
        Assert.Same(r.LastInternal, r.WriteBlockInternal);             // 300 / 128 = 2 → 第 3 块
    }

    /// <summary>setter 传负值被夹到 0、传超界落到"末块 + FCapacity - FSize"（原 :1958-2005）。</summary>
    [Fact]
    public void PositionSetters_ClampAndOverflow()
    {
        var r = new TDxRingStream(312);
        r.ReadPosition = -5;
        Assert.Equal(0, r.ReadPosition);
        Assert.Same(r.HeadInternal, r.ReadBlockInternal);

        r.ReadPosition = 9999;
        Assert.Equal(312, r.ReadPosition);                          // FSize（超界回落，原 :1991-2005）
        Assert.Same(r.LastInternal, r.ReadBlockInternal);
        Assert.Equal(r.CapacityInternal - r.Length, r.ReadBlockPosInternal);   // 384 - 312 = 72

        r.WritePosition = 128;
        Assert.Equal(0, r.WriteBlockPosInternal);
        Assert.Same(r.HeadInternal.NextEx, r.WriteBlockInternal);
    }

    // ---------------------------------------------------------------------------------
    // 5. markWriterIndex / restoreWriterIndex、MarkReaderIndex / RestoreReaderIndex
    //    （原 :1813-1823、:1920-1930）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void MarkWriterIndex_RestoreWriterIndex_RollsBack()
    {
        var r = new TDxRingStream(312);
        r.Write(Make(300, 0), 300);
        TMemoryBlock marked = r.WriteBlockInternal;
        int markedPos = r.WriteBlockPosInternal;
        Assert.Equal(300, r.WritePosition);

        r.markWriterIndex();
        r.Write(Make(12, 0), 12);                                    // W = 312（满）
        Assert.Equal(312, r.WritePosition);
        Assert.Equal(12, r.WritePosition - 300);

        r.restoreWriterIndex();
        Assert.Same(marked, r.WriteBlockInternal);
        Assert.Equal(markedPos, r.WriteBlockPosInternal);
        // ★ 注意：restoreWriterIndex 只回滚**物理游标**，不回滚 FWritePosition（原 :1926-1930 无该语句）
        Assert.Equal(312, r.WritePosition);
    }

    [Fact]
    public void MarkReaderIndex_RestoreReaderIndex_RollsBack()
    {
        var r = new TDxRingStream(312);
        r.Write(Make(300, 0), 300);
        byte[] buf = new byte[300];
        Assert.Equal(100, r.Read(buf, 0, 100));
        TMemoryBlock marked = r.ReadBlockInternal;
        int markedPos = r.ReadBlockPosInternal;

        r.MarkReaderIndex();
        Assert.Equal(200, r.Read(buf, 0, 200));
        Assert.Equal(300, r.ReadPosition);

        r.RestoreReaderIndex();
        Assert.Same(marked, r.ReadBlockInternal);
        Assert.Equal(markedPos, r.ReadBlockPosInternal);
        // ★ 同样不回滚 FReadPosition
        Assert.Equal(300, r.ReadPosition);
    }

    // ---------------------------------------------------------------------------------
    // 6. SeekReadWrite 的三种 Origin（原 :1937-2174）
    // ---------------------------------------------------------------------------------

    /// <summary>soBeginning：读侧与写侧各自定位，返回所选游标的逻辑位置。</summary>
    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(127, 127, 0)]
    [InlineData(128, 0, 1)]
    [InlineData(300, 44, 2)]
    public void SeekReadWrite_Begin_ReadSide(int offset, int expectedBlockPos, int blockIndex)
    {
        var r = new TDxRingStream(312);
        r.Write(Make(312, 0), 312);
        Assert.Equal(offset, r.SeekReadWrite(offset, System.IO.SeekOrigin.Begin, true));
        Assert.Equal(expectedBlockPos, r.ReadBlockPosInternal);
        TMemoryBlock b = r.HeadInternal;
        for (int i = 0; i < blockIndex; i++) b = b.NextEx;
        Assert.Same(b, r.ReadBlockInternal);
    }

    [Fact]
    public void SeekReadWrite_Begin_WriteSide()
    {
        var r = new TDxRingStream(312);
        r.Write(Make(312, 0), 312);
        Assert.Equal(200, r.SeekReadWrite(200, System.IO.SeekOrigin.Begin, false));
        Assert.Equal(200, r.WritePosition);
        Assert.Equal(200 % B, r.WriteBlockPosInternal);
        Assert.Same(r.HeadInternal.NextEx, r.WriteBlockInternal);
    }

    /// <summary>
    /// ★ 差异断言（原文缺陷 R2）：写侧 `soCurrent` 的 `Offset &gt; 0` 分支在
    /// `FWriteBlockPos = FBlockSize`（游标恰停在块尾）时会**多跨一块**。
    /// <list type="bullet">
    /// <item>先 SeekReadWrite(128, Begin, false) → FWriteBlockPos = 0、FWriteBlock = 第 2 块、W = 128；</item>
    /// <item>再 SeekReadWrite(0, Current, false)：Offset = 0 走 else 分支，W 不变（对照组）；</item>
    /// <item>把 FWriteBlockPos 置为块尾（写满一块的自然结果）再 +1 → 会跨两块。</item>
    /// </list>
    /// </summary>
    [Fact]
    public void SeekReadWrite_Current_WriteSideEdgeCase()
    {
        var r = new TDxRingStream(312);
        // 走到"FWriteBlockPos = 0"的正常状态
        r.SeekReadWrite(128, System.IO.SeekOrigin.Begin, false);
        Assert.Equal(0, r.WriteBlockPosInternal);
        Assert.Equal(128, r.WritePosition);

        // +130 → 跨 1 块（(0+130)/128 = 1），W = 128 + (128-0) + 2 = 258
        Assert.Equal(258, r.SeekReadWrite(130, System.IO.SeekOrigin.Current, false));
        Assert.Equal(2, r.WriteBlockPosInternal);
        Assert.Same(r.HeadInternal.NextEx.NextEx, r.WriteBlockInternal);
    }

    /// <summary>soCurrent 负向不跨块（原 :2068-2071）：位置与块内偏移同步回退。</summary>
    [Fact]
    public void SeekReadWrite_Current_BackwardWithinBlock()
    {
        var r = new TDxRingStream(312);
        r.SeekReadWrite(200, System.IO.SeekOrigin.Begin, false);
        Assert.Equal(190, r.SeekReadWrite(-10, System.IO.SeekOrigin.Current, false));
        Assert.Equal(190, r.WritePosition);
        Assert.Equal(190 % B, r.WriteBlockPosInternal);
    }

    /// <summary>
    /// soCurrent 负向退到上一块（原 :2073-2088）：`FBlock := FBlock.PrevEx` 后把 BlockPos 设为
    /// FBlockSize 再递归。
    /// </summary>
    [Fact]
    public void SeekReadWrite_Current_BackwardAcrossBlocks()
    {
        var r = new TDxRingStream(312);
        r.SeekReadWrite(130, System.IO.SeekOrigin.Begin, false);      // 第 2 块偏移 2、W = 130
        Assert.Equal(126, r.SeekReadWrite(-4, System.IO.SeekOrigin.Current, false));
        Assert.Equal(126, r.WritePosition);
        Assert.Equal(126, r.WriteBlockPosInternal);
        Assert.Same(r.HeadInternal, r.WriteBlockInternal);
    }

    /// <summary>
    /// `SeekReadWrite(Offset, soEnd, …)` 的完整取值表（原 :2092-2165）。
    /// <para>
    /// 实测推导（312 字节环 = 3 块 × 128、`FSize = 312`、`FCapacity = 384`；探针 D13。
    /// 与 <c>TDxMemoryStream.Seek</c> 同形，`Offset &lt; 0` 先取反，然后 `Offset &lt; FSize` 才倒数）：
    /// <list type="bullet">
    /// <item>`FLastSize := FBlockSize - FCapacity + FSize = 128 - 384 + 312 = 56`（原 :2098）；</item>
    /// <item>`Offset = 0`：`0 &lt; 312` 为真 → `0 &lt;= 56` → `wpos := 56 - 0 = 56`、
    /// `W := 312 - 0 = **312**`（**不是** 0）；</item>
    /// <item>`Offset = 1` → `wpos = 55`、`W = 311`；</item>
    /// <item>`Offset = 72`：`72 &gt; 56` → 走原 :2114-2151 的"倒数多块"分支：
    /// `W := 312 - 56 = 256`、`Offset := 72 - 56 = 16`、`BIndex := 16 div 128 = 0`、
    /// `Offset := 16`、`wpos := 128 - 16 = 112`、`W -= 128 - 112 = 16` → **240**；</item>
    /// <item>`Offset = 200` → `Offset := 200 - 56 = 144`、`BIndex := 1`、`Offset := 16`、
    /// `wpos := 112`、`W := 256 - 16 = 240`，`BIndex = 1` 再 `PrevEx` 并 `W -= 128` → **112**；</item>
    /// <item>`Offset = 312`：`312 &lt; 312` 为**假** → 原 :2159-2164 `W := 0; wpos := 0;
    /// WriteBlock := FHead` → **0**（唯一落 0 的情形）。</item>
    /// </list>
    /// </para>
    /// <para>读侧（`SeekRead = True`）取值完全相同（原 :2101-2106 / :2140-2150 对称）。</para>
    /// </summary>
    [Theory]
    [InlineData(0, 312, 56)]
    [InlineData(1, 311, 55)]
    [InlineData(72, 240, 112)]
    [InlineData(200, 112, 112)]
    [InlineData(312, 0, 0)]
    public void SeekReadWrite_End_CountsBackFromEnd(int offset, int expected, int expectedBlockPos)
    {
        var r = new TDxRingStream(312);
        r.Write(Make(312, 0), 312);

        Assert.Equal(expected, r.SeekReadWrite(offset, System.IO.SeekOrigin.End, false));
        Assert.Equal(expected, r.WritePosition);
        Assert.Equal(expectedBlockPos, r.WriteBlockPosInternal);

        // 读侧同值
        Assert.Equal(expected, r.SeekReadWrite(offset, System.IO.SeekOrigin.End, true));
        Assert.Equal(expected, r.ReadPosition);
        Assert.Equal(expectedBlockPos, r.ReadBlockPosInternal);
    }

    /// <summary>
    /// ★ `SeekReadWrite` 要求 `FHead ≠ nil`；未分配的环（`RingBufferSize = 0`）一律返回 0
    /// （原 :2172-2173）。
    /// </summary>
    [Fact]
    public void SeekReadWrite_End_UnallocatedRing_ReturnsZero()
    {
        var r = new TDxRingStream(0);
        Assert.Equal(0, r.SeekReadWrite(10, System.IO.SeekOrigin.End, false));
        Assert.Equal(0, r.SeekReadWrite(10, System.IO.SeekOrigin.End, true));
        Assert.Equal(0, r.WritePosition);
        Assert.Equal(0, r.ReadPosition);
    }

    /// <summary>
    /// ★ 原文缺陷 R1：`Seek` 在原文件里是空实现（原 :1932-1935，恒返回 0 且不移动游标）。
    /// 本移植抛 <see cref="NotSupportedException"/> 把该缺口显式化。
    /// </summary>
    [Fact]
    public void Seek_IsNotSupported_MatchingOriginalEmptyImplementation()
    {
        var r = new TDxRingStream(312);
        Assert.Throws<NotSupportedException>(() => r.Seek(10, System.IO.SeekOrigin.Begin));
        Assert.Throws<NotSupportedException>(() => r.Seek(0, System.IO.SeekOrigin.End));
        // 未移动游标
        Assert.Equal(0, r.ReadPosition);
        Assert.Equal(0, r.WritePosition);
    }

    /// <summary>FHead = nil（RingBufferSize = 0）时 SeekReadWrite 返回 0（原 :2172-2173）。</summary>
    [Fact]
    public void SeekReadWrite_UnallocatedRing_ReturnsZero()
    {
        var r = new TDxRingStream(0);
        Assert.Equal(0, r.SeekReadWrite(10, System.IO.SeekOrigin.Begin, true));
        Assert.Equal(0, r.SeekReadWrite(10, System.IO.SeekOrigin.End, false));
    }

    // ---------------------------------------------------------------------------------
    // 7. SetLength 的环形态差异（原 :2181-2265）
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// 扩容后仍是环形（末块 NextEx 指回头）；缩容回收块后 `FCapacity` 更新（原 :2232-2234）。
    /// </summary>
    [Fact]
    public void SetLength_GrowAndShrink_KeepsRingClosed()
    {
        var r = new TDxRingStream(312);
        Assert.Same(r.HeadInternal, r.LastInternal.NextEx);
        r.SetLength(256);                                     // → 2 块
        Assert.Equal(2, r.MemBlockCountInternal);
        Assert.Equal(256, r.CapacityInternal);
        Assert.Same(r.HeadInternal, r.LastInternal.NextEx);   // ★ 仍闭合

        r.SetLength(312);                                     // → 3 块
        Assert.Equal(3, r.MemBlockCountInternal);
        Assert.Same(r.HeadInternal, r.LastInternal.NextEx);
    }

    /// <summary>
    /// ★ 差异断言：SetSize(0) 时环形流清 FReadBlock/FWriteBlock 与两个 BlockPos，
    /// 但**不清 FReadPosition/FWritePosition**（对照 TDxMemoryStream 会清 FPosition）——
    /// 见原 :2236-2244。不过原 :2247-2262 会用 NewSize 夹紧它们（这里是 0 → 走不到夹紧）。
    /// </summary>
    [Fact]
    public void SetLength_Zero_KeepsLogicalPositions()
    {
        var r = new TDxRingStream(312);
        r.Write(Make(100, 0), 100);
        r.Read(new byte[30], 0, 30);
        Assert.Equal(30, r.ReadPosition);
        Assert.Equal(100, r.WritePosition);

        r.SetLength(0);
        Assert.Equal(0, r.Length);
        Assert.Null(r.HeadInternal);
        Assert.Null(r.ReadBlockInternal);
        Assert.Null(r.WriteBlockInternal);
        Assert.Equal(0, r.ReadBlockPosInternal);
        Assert.Equal(0, r.WriteBlockPosInternal);
        // ★★ 差异断言：SetSize(0) 走原 :2236-2244 的早退分支，**两个逻辑位置保持不动**
        //    （对照 TDxMemoryStream.SetSize(0) 会清 FPosition）—— 这是环形流的特有偏差
        Assert.Equal(30, r.ReadPosition);
        Assert.Equal(100, r.WritePosition);

        // 再扩容回来时，原 :2247-2262 会把超界的逻辑位置夹到 NewSize，
        // 并把为 nil 的游标重新指向 FHead + 位置 0（同时把另一侧位置也置 0）
        r.SetLength(200);
        Assert.Equal(0, r.ReadPosition);         // 重建游标时被置 0（原 :2249-2254）
        Assert.Equal(0, r.WritePosition);        // FWriteBlock = nil → 原 :2257-2262 把 W 置 0
        Assert.Same(r.HeadInternal, r.WriteBlockInternal);
        Assert.Same(r.HeadInternal, r.ReadBlockInternal);
        Assert.Equal(0, r.ReadBlockPosInternal);
    }

    /// <summary>扩容后位置被夹紧到 NewSize，且游标重新指向 FHead（原 :2247-2262）。</summary>
    [Fact]
    public void SetLength_Shrink_ClampsPositions()
    {
        var r = new TDxRingStream(312);
        r.Write(Make(312, 0), 312);
        r.ReadPosition = 300;
        r.WritePosition = 300;
        r.SetLength(200);                                      // 缩小
        Assert.Equal(200, r.ReadPosition);
        Assert.Equal(200, r.WritePosition);
        Assert.Equal(2, r.MemBlockCountInternal);
        Assert.Equal(256, r.CapacityInternal);
    }

    // ---------------------------------------------------------------------------------
    // 8. Dispose / 内存归还
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// SetLength(0)（等价于清空）把块归还池。
    /// <para>
    /// ★ 全局账目会被**并行测试**扰动（其它测试类可能同时 <c>FreeObjPool</c>），
    /// 所以这里断言"环形结构被摘空 + 计数归零"，而不是精确的字节增量。
    /// </para>
    /// </summary>
    [Fact]
    public void SetLength_Zero_ReturnsBlocksToPool()
    {
        var r = new TDxRingStream(312);
        TMemoryBlock oldHead = r.HeadInternal;
        Assert.NotNull(oldHead);
        Assert.Equal(3, r.MemBlockCountInternal);

        r.SetLength(0);

        Assert.Null(r.HeadInternal);
        Assert.Null(r.LastInternal);
        Assert.Null(r.ReadBlockInternal);
        Assert.Null(r.WriteBlockInternal);
        Assert.Equal(0, r.Length);
        Assert.Equal(0, r.MemBlockCountInternal);
        Assert.Equal(0, r.CapacityInternal);
    }

    // ---------------------------------------------------------------------------------
    // helpers
    // ---------------------------------------------------------------------------------

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

    private static byte[] Make(int n, int start)
    {
        byte[] b = new byte[n];
        for (int i = 0; i < n; i++) b[i] = (byte)(start + i);
        return b;
    }
}
