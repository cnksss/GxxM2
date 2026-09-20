using System;
using System.IO;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// uBuffer.pas 内存流（原 uBuffer.pas:54-94 声明、:535-1316 实现）的移植测试。
/// <para>
/// 本测试用 **MB_Small**（块大小 128，来自原 :245）作为主场景，因为 128 很小、
/// 很容易构造"跨块 ±1"的边界；另用 **MB_Normal**（640，原 :238）做一次交叉验证。
/// </para>
/// <para>每个用例都自带独立的 <see cref="TDxMemoryStream"/>；池是全局共享的，故只用相对断言。</para>
/// </summary>
/// <summary>与其它 uBuffer 测试类同属一个 collection（禁用并行）——见 <see cref="BufferTestCollection"/>。</summary>
[Collection(BufferTestCollection.Name)]
public class BufferStreamTests
{
    private const int B = 128;   // MB_Small 的块大小（原 :245 + 原 :296-299 对齐）

    // ---------------------------------------------------------------------------------
    // 1. 构造 / 基本属性 / 大小（原 :542-546、:568-571、:1040-1114）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void Create_DefaultsToSmallBlockType()
    {
        var s = new TDxMemoryStream();
        Assert.Equal(TDxMemBlockType.MB_Small, s.MemBlockType);          // 原 :72 默认参数 MB_Small
        Assert.Equal(B, s.BlockSize);                                    // 原 :554-566
        Assert.Equal(0, s.Length);                                       // 原 :568-571
        Assert.Null(s.Head);
        Assert.Null(s.Last);
    }

    [Fact]
    public void Create_WithEachMemBlockType_ReportsExtractedBlockSize()
    {
        Assert.Equal(128, new TDxMemoryStream(TDxMemBlockType.MB_Small).BlockSize);
        Assert.Equal(640, new TDxMemoryStream(TDxMemBlockType.MB_Normal).BlockSize);
        Assert.Equal(1024, new TDxMemoryStream(TDxMemBlockType.MB_Big).BlockSize);
        Assert.Equal(2048, new TDxMemoryStream(TDxMemBlockType.MB_SpBig).BlockSize);
        Assert.Equal(4096, new TDxMemoryStream(TDxMemBlockType.MB_Large).BlockSize);
        Assert.Equal(16384, new TDxMemoryStream(TDxMemBlockType.MB_SPLarge).BlockSize);
    }

    /// <summary>原 :1059-1061：`FMemBlockCount := NewSize div FBlockSize; if NewSize mod FBlockSize &lt;&gt; 0 then Inc`。</summary>
    [Theory]
    [InlineData(0, 0, 0)]        // 0 字节 → 0 块
    [InlineData(1, 1, 128)]      // 1 字节 → 1 块 128
    [InlineData(128, 1, 128)]    // 恰好 1 块
    [InlineData(129, 2, 256)]    // 1 块 + 1
    [InlineData(256, 2, 256)]    // 恰好 2 块
    [InlineData(257, 3, 384)]    // 2 块 + 1
    public void SetLength_BlockCountAndCapacity(long newSize, int expectedBlocks, int expectedCapacity)
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.SetLength(newSize);
        Assert.Equal(newSize, s.Length);
        Assert.Equal(expectedBlocks, s.FMemBlockCount);
        Assert.Equal(expectedCapacity, s.CapacityInternal);
    }

    /// <summary>SetLength 把 FSize=0 的流变成空流时清掉全部指针（原 :1093-1102）。</summary>
    [Fact]
    public void SetLength_Zero_ClearsEverything()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);
        Assert.NotNull(s.Head);
        s.SetLength(0);
        Assert.Equal(0, s.Length);
        Assert.Null(s.Head);
        Assert.Null(s.Last);
        Assert.Null(s.CurBlockInternal);
        Assert.Equal(0, s.CapacityInternal);
        Assert.Equal(0, s.FMemBlockCount);
        Assert.Equal(0, s.Position);
    }

    /// <summary>SetLength 是幂等的（原 :1046 `if FSize &lt;&gt; NewSize`）。</summary>
    [Fact]
    public void SetLength_SameValue_IsNoOp()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(200), 200);
        int cap = s.CapacityInternal;
        int blocks = s.FMemBlockCount;
        s.SetLength(200);
        Assert.Equal(cap, s.CapacityInternal);
        Assert.Equal(blocks, s.FMemBlockCount);
    }

    /// <summary>Clear（原 :537-540）等价于 SetSize(0)。</summary>
    [Fact]
    public void Clear_IsSetSizeZero()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);
        s.Clear();
        Assert.Equal(0, s.Length);
        Assert.Null(s.Head);
        Assert.Equal(0, s.Position);
    }

    /// <summary>SetMemBlockType 换类型**先清空**（原 :1031-1038）。相同值则不动。</summary>
    [Fact]
    public void SetMemBlockType_ChangingTypeClearsStream_SameTypeKeepsData()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);

        s.SetMemBlockType(TDxMemBlockType.MB_Small);          // 相同值 → 不清
        Assert.Equal(300, s.Length);
        Assert.Equal(B, s.BlockSize);

        s.SetMemBlockType(TDxMemBlockType.MB_Normal);         // 不同值 → Clear 后换类型
        Assert.Equal(0, s.Length);
        Assert.Equal(640, s.BlockSize);
        Assert.Null(s.Head);
    }

    // ---------------------------------------------------------------------------------
    // 2. Write：跨块边界、扩容（原 :1154-1242）
    // ---------------------------------------------------------------------------------

    /// <summary>首次 Write 走 `FCurBlock = nil → SetSize(Count)`（原 :1169-1170）。</summary>
    [Fact]
    public void Write_FirstWrite_SizesStreamToCount()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        byte[] data = Make(10);
        s.Write(data, 10);
        Assert.Equal(10, s.Length);                        // FSize = Count
        Assert.Equal(1, s.FMemBlockCount);
        Assert.Equal(B, s.CapacityInternal);
        Assert.Equal(10, s.PositionInternal);
    }

    /// <summary>跨块写入：一次写 300 字节应跨 3 块（128/128/44）。</summary>
    [Fact]
    public void Write_SpanningBlocks_WritesAllData()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);
        Assert.Equal(300, s.Length);
        Assert.Equal(3, s.FMemBlockCount);
        Assert.Equal(3 * B, s.CapacityInternal);
        Assert.Equal(300, s.PositionInternal);
    }

    /// <summary>★ 块边界 ±1：127 / 128 / 129 字节的块数、容量与游标（原 :1173-1207）。</summary>
    [Theory]
    [InlineData(127, 1, 128, 127)]
    [InlineData(128, 1, 128, 128)]
    [InlineData(129, 2, 256, 1)]
    public void Write_BlockBoundary_PlusMinusOne(int n, int blocks, int cap, int pos)
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(n), n);
        Assert.Equal(n, s.Length);
        Assert.Equal(blocks, s.FMemBlockCount);
        Assert.Equal(cap, s.CapacityInternal);
        Assert.Equal(pos, s.CurBlockPosInternal);
    }

    /// <summary>多次小写累加跨块（原 :1208-1236 的循环 + 原 :1237-1241 的尾部复位）。</summary>
    [Fact]
    public void Write_MultipleSmallWrites_CrossBlockCorrectly()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        byte[] all = Make(300);
        for (int i = 0; i < 300; i += 50)
            s.Write(all, i, 50);
        Assert.Equal(300, s.Length);
        Assert.Equal(3, s.FMemBlockCount);

        // 读回来必须完全一致（覆盖 3 个块的搬运路径）
        s.Seek(0, SeekOrigin.Begin);
        byte[] readBack = new byte[300];
        Assert.Equal(300, s.Read(readBack, 0, 300));
        Assert.Equal(all, readBack);
    }

    /// <summary>
    /// ★ 差异断言：原 :1171-1177 —— 只有 `FCurBlock = FLast` 且块内放不下时才 `SetSize(FSize + Count)`
    /// 扩容；在**中间块**覆盖写**不改变 FSize**。
    /// </summary>
    [Fact]
    public void Write_OverwriteInMiddleBlock_DoesNotChangeSize()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);
        s.Seek(10, SeekOrigin.Begin);
        s.Write(new byte[] { 1, 2, 3, 4, 5 }, 5);
        Assert.Equal(300, s.Length);                     // 不扩
        Assert.Equal(15, s.Position);
        Assert.Equal(3, s.FMemBlockCount);
    }

    /// <summary>
    /// 差异断言：`FCurBlock = FLast` 且 `FCurBlockPos + Count &gt; FBlockSize` → `SetSize(FSize + Count)`；
    /// 否则若 `FPosition + Count &gt; FSize` → `Inc(FSize, Count)`（原地扩，块数不变）。
    /// </summary>
    [Fact]
    public void Write_InLastBlock_InPlaceGrowth()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(127), 127);
        Assert.Equal(1, s.FMemBlockCount);
        s.Write(new byte[] { 9 }, 1);                    // 128 字节，仍在第 1 块内
        Assert.Equal(128, s.Length);
        Assert.Equal(1, s.FMemBlockCount);               // 块数没变
        Assert.Equal(128, s.PositionInternal);
        Assert.Equal(B, s.CurBlockPosInternal);          // 停在块尾
    }

    /// <summary>续写越过块尾 → 扩容并跨块（原 :1173 → SetSize(FSize + Count)）。</summary>
    [Fact]
    public void Write_PastLastBlockTail_GrowsAndSpans()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(128), 128);                         // 恰好 1 块
        Assert.Equal(1, s.FMemBlockCount);
        s.Write(Make(10), 10);                           // 需要第 2 块
        Assert.Equal(138, s.Length);
        Assert.Equal(2, s.FMemBlockCount);
        Assert.Equal(138, s.PositionInternal);
    }

    /// <summary>Write 的返回值 = 入参 Count（原 :1183 `Result := Count`）。</summary>
    [Fact]
    public void Write_ReturnsRequestedCount()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        Assert.Equal(300, s.Write(Make(300), 300));
        Assert.Equal(0, s.Write(Array.Empty<byte>(), 0));
    }

    /// <summary>
    /// 写满最后一块后的游标状态（原 :1208-1241）。
    /// <para>
    /// 实测推导（MB_Small，FBlockSize = 128；探针 D01）：
    /// <list type="bullet">
    /// <item>`Write(128)`：原 :1208 循环把 `FCurBlockPos` 推到 128 = FBlockSize →
    /// 原 :1226-1227 `FCurBlock := FCurBlock^.NextEx` 变 nil → 循环退出（Count 已为 0）→
    /// 原 :1237-1241 把游标复位为 `FCurBlock := FLast; FCurBlockPos := FBlockSize`。
    /// 实测：FSize=128、FPos=128、curPos=**128**、blocks=1。</item>
    /// <item>紧接 `Write(1)`：`FCurBlockPos = 128 = FBlockSize` → `FCurBlock = FLast`（非 nil）
    /// 且 `FCurBlockPos(128) + 1 &gt; FBlockSize(128)` 为真 → 原 :1173 `SetSize(FSize + 1)` 扩到 129 →
    /// 原 :1178-1182 跨到新块 → 写入 1 字节。实测：FSize=129、FPos=129、curPos=**1**、blocks=2。</item>
    /// </list>
    /// </para>
    /// <para>
    /// ★ 结论：`Write` 之后游标**不是** nil（原 :1237-1241 已复位）；只有 `Seek` 才会把
    /// `FBlockSize` 哨兵改写掉（原 :928-931 直接赋 `Offset mod FBlockSize`）。
    /// </para>
    /// </summary>
    [Fact]
    public void Write_ExactlyFillingLastBlock_ResetsCursorToTail()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(128), 128);
        Assert.Equal(128, s.Length);
        Assert.Equal(128, s.PositionInternal);
        Assert.Same(s.Last, s.CurBlockInternal);      // 原 :1237-1241 复位到 FLast
        Assert.Equal(128, s.CurBlockPosInternal);     // …且位置停在块尾
        Assert.Equal(1, s.FMemBlockCount);

        s.Write(Make(1), 1);
        Assert.Equal(129, s.Length);
        Assert.Equal(129, s.PositionInternal);
        Assert.Equal(2, s.FMemBlockCount);
        Assert.Equal(1, s.CurBlockPosInternal);       // 原 :1178-1182 先跨块
    }

    // ---------------------------------------------------------------------------------
    // 3. Read：跨块、截断、边界（原 :701-787）
    // ---------------------------------------------------------------------------------

    /// <summary>FCurBlock = nil（空流）→ 返回 0（原 :707-708）。</summary>
    [Fact]
    public void Read_EmptyStream_ReturnsZero()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        Assert.Equal(0, s.Read(new byte[10], 0, 10));
        Assert.Null(s.CurBlockInternal);
    }

    /// <summary>Count = 0 → 返回 0（原 :722-726）。</summary>
    [Fact]
    public void Read_ZeroCount_ReturnsZero()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(10), 10);
        s.Seek(0, SeekOrigin.Begin);
        Assert.Equal(0, s.Read(new byte[10], 0, 0));
    }

    /// <summary>
    /// ★ 截断：原 :720-721 `if FPosition + Count &gt; FSize then Count := FSize - FPosition;`
    /// </summary>
    [Fact]
    public void Read_CountBeyondRemaining_IsTruncated()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(10), 10);
        s.Seek(4, SeekOrigin.Begin);
        byte[] buf = new byte[100];
        Assert.Equal(6, s.Read(buf, 0, 100));            // 10 - 4 = 6
        Assert.Equal(10, s.PositionInternal);
        Assert.Equal(0, s.Read(buf, 0, 100));
    }

    /// <summary>
    /// 在流末尾（FPosition = FSize）读 → 原 :720-726 截断为 0（不进入搬运）。
    /// </summary>
    [Fact]
    public void Read_AtEnd_ReturnsZero()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(256), 256);
        Assert.Equal(256, s.PositionInternal);           // 停在哨兵位置
        Assert.Equal(0, s.Read(new byte[10], 0, 10));
        Assert.Equal(256, s.PositionInternal);           // 不推进
    }

    /// <summary>★ 跨块读：一次读 300 字节（跨 3 块），内容与写入完全一致。</summary>
    [Fact]
    public void Read_SpanningBlocks_ReturnsExactData()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        byte[] data = Make(300);
        s.Write(data, 300);
        s.Seek(0, SeekOrigin.Begin);
        Assert.Equal(0, s.PositionInternal);
        Assert.Equal(0, s.CurBlockPosInternal);

        byte[] buf = new byte[300];
        Assert.Equal(300, s.Read(buf, 0, 300));
        Assert.Equal(data, buf);
        Assert.Equal(300, s.PositionInternal);
        Assert.Equal(300 - 256, s.CurBlockPosInternal);  // 44
    }

    /// <summary>块边界 ±1 的读：从 0 读 127/129/130/255/257 字节。</summary>
    [Theory]
    [InlineData(127)]
    [InlineData(129)]
    [InlineData(130)]
    [InlineData(255)]
    [InlineData(257)]
    public void Read_BlockBoundary_PlusMinusOne(int n)
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        byte[] data = Make(300);
        s.Write(data, 300);
        s.Seek(0, SeekOrigin.Begin);
        byte[] buf = new byte[n];
        Assert.Equal(n, s.Read(buf, 0, n));
        Assert.Equal(data.AsSpan(0, n).ToArray(), buf);
        Assert.Equal(n, s.PositionInternal);
    }

    /// <summary>
    /// 从块首连续读**整数个块**的边界（原 :741-785）。
    /// <para>
    /// 实测推导（MB_Small，写 300 字节 = 3 块，`Seek(0, Begin)`；探针 D02）：
    /// <list type="bullet">
    /// <item>`Read(256)`：原 :748-756 吃满第 1 块 → 跨到第 2 块；原 :762 循环 `Count = 128`
    /// 走原 :772-779（`128 &gt; 128` 为假）拷满第 2 块 → 原 :780 `FCurBlockPos = 128 = FBlockSize`
    /// → 跨到第 3 块 → `Count` 已为 0 退出。实测：返回 **256**、`FPos = 256`、`curPos = 0`
    /// （游标停在第 3 块块首，**不是 nil**）。</item>
    /// <item>紧接 `Read(10)`：`FPos(256) + 10 &gt; FSize(300)` 为假，`FCurBlockPos(0) ≠ FBlockSize`
    /// → 原 :741 判定 `10 &lt;= 128 - 0` 成立 → 从第 3 块偏移 0 拷 10 字节。实测：返回 **10**、
    /// `FPos = 266`、`curPos = 10`。</item>
    /// <item>整读 300（正好 3 块）：返回 **300**、`FPos = 300`、`curPos = 44`；
    /// 第二次 `Read(10)` 因 `FPos = FSize` 被原 :720-726 截断为 0 → 返回 0（不崩）。</item>
    /// </list>
    /// </para>
    /// <para>
    /// ★ 结论：原 :757-761 / :780-784 的"跨块"是**幂等收敛**的（跨块后 `FCurBlockPos = 0`，
    /// 不会连锁触发），既不会把游标留在 nil，也不存在 nil 解引用。
    /// </para>
    /// </summary>
    [Fact]
    public void Read_ExactlyWholeBlocksFromHead_KeepsCursorInsideChain()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);
        s.Seek(0, SeekOrigin.Begin);

        byte[] first = new byte[256];
        Assert.Equal(256, s.Read(first, 0, 256));
        Assert.Equal(Make(256), first);
        Assert.Equal(256, s.PositionInternal);
        Assert.NotNull(s.CurBlockInternal);            // 停在第 3 块，不是 nil
        Assert.Equal(0, s.CurBlockPosInternal);

        byte[] more = new byte[10];
        Assert.Equal(10, s.Read(more, 0, 10));
        Assert.Equal(Make(300).AsSpan(256, 10).ToArray(), more);
        Assert.Equal(266, s.PositionInternal);
        Assert.Equal(10, s.CurBlockPosInternal);

        // 整读 300（正好 3 块）
        var t = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        t.Write(Make(300), 300);
        t.Seek(0, SeekOrigin.Begin);
        byte[] all = new byte[300];
        Assert.Equal(300, t.Read(all, 0, 300));
        Assert.Equal(Make(300), all);
        Assert.Equal(300, t.PositionInternal);
        Assert.Equal(44, t.CurBlockPosInternal);
        Assert.Equal(0, t.Read(new byte[10], 0, 10));  // FPos = FSize → 原 :720-726 截断为 0
    }

    /// <summary>从中段跨块读（起点不在块首）——覆盖原 :748-756 的"先吃当前块剩余"路径。</summary>
    [Fact]
    public void Read_FromMidBlock_SpansIntoNextBlocks()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        byte[] data = Make(300);
        s.Write(data, 300);
        s.Seek(100, SeekOrigin.Begin);                   // 第 1 块内偏移 100
        byte[] buf = new byte[200];
        Assert.Equal(200, s.Read(buf, 0, 200));
        Assert.Equal(data.AsSpan(100, 200).ToArray(), buf);
        Assert.Equal(300, s.PositionInternal);
    }

    /// <summary>Read 的 offset 参数生效（写入调用方缓冲区的指定位置，前缀不被改）。</summary>
    [Fact]
    public void Read_HonoursBufferOffset()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        byte[] data = Make(50);
        s.Write(data, 50);
        s.Seek(0, SeekOrigin.Begin);
        byte[] buf = new byte[60];
        for (int i = 0; i < buf.Length; i++) buf[i] = 0xFF;
        Assert.Equal(50, s.Read(buf, 5, 50));
        for (int i = 0; i < 5; i++) Assert.Equal(0xFF, buf[i]);
        Assert.Equal(data, buf.AsSpan(5, 50).ToArray());
    }

    /// <summary>MB_Normal（640 字节块）交叉验证：写 1000 字节应占 2 块。</summary>
    [Fact]
    public void Read_NormalBlockType_SpansTwoBlocks()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Normal);
        byte[] data = Make(1000);
        s.Write(data, 1000);
        Assert.Equal(2, s.FMemBlockCount);
        Assert.Equal(1280, s.CapacityInternal);
        s.Seek(0, SeekOrigin.Begin);
        byte[] buf = new byte[1000];
        Assert.Equal(1000, s.Read(buf, 0, 1000));
        Assert.Equal(data, buf);
    }

    /// <summary>Read(buffer, count) 便捷重载。</summary>
    [Fact]
    public void Read_ConvenienceOverload_Works()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(20), 20);
        s.Seek(0, SeekOrigin.Begin);
        byte[] buf = new byte[20];
        Assert.Equal(20, s.Read(buf, 20));
        Assert.Equal(Make(20), buf);
    }

    // ---------------------------------------------------------------------------------
    // 4. Seek：三种 Origin + 超界（原 :904-1029）
    // ---------------------------------------------------------------------------------

    /// <summary>soBeginning：Offset &lt;= FSize 时精确定位，返回新 FPosition（原 :922-945）。</summary>
    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 1, 0)]
    [InlineData(127, 127, 0)]
    [InlineData(128, 0, 1)]      // 恰好第 2 块块首
    [InlineData(129, 1, 1)]
    [InlineData(255, 127, 1)]
    [InlineData(256, 0, 2)]      // 第 3 块块首
    [InlineData(300, 44, 2)]     // 第 3 块内 44
    public void Seek_Begin_PositionsExactly(long offset, int expectedBlockPos, int expectedBlockIndex)
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);
        Assert.Equal(offset, s.Seek(offset, SeekOrigin.Begin));
        Assert.Equal(expectedBlockPos, s.CurBlockPosInternal);
        TMemoryBlock b = s.Head;
        for (int i = 0; i < expectedBlockIndex; i++) b = b.NextEx;
        Assert.Same(b, s.CurBlockInternal);
    }

    /// <summary>soBeginning 负偏移被夹到 0（原 :924-925）。</summary>
    [Fact]
    public void Seek_Begin_NegativeClampedToZero()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);
        s.Seek(200, SeekOrigin.Begin);
        Assert.Equal(0, s.Seek(-5, SeekOrigin.Begin));
        Assert.Equal(0, s.PositionInternal);
    }

    /// <summary>soBeginning 超界 → "末块 + FCapacity - FSize"、"FPosition := FSize"（原 :939-944）。</summary>
    [Fact]
    public void Seek_Begin_BeyondSize_GoesToLastBlockTail()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);
        Assert.Equal(300, s.Seek(9999, SeekOrigin.Begin));
        Assert.Same(s.Last, s.CurBlockInternal);
        Assert.Equal(s.CapacityInternal - s.Length, s.CurBlockPosInternal);   // 384 - 300 = 84
    }

    /// <summary>
    /// `Seek(Offset, soCurrent)` 正向推进（原 :946-961）。
    /// <para>
    /// 实测推导（MB_Small，写 300 = 3 块；探针 D03）：`Seek(0, Begin)` 后 `curPos = 0`；
    /// `Seek(200, Current)` 走原 :950-959：
    /// `BIndex := (0 + 200) div 128 = 1`；`FPos += 128 - 0 = 128`；`curPos := 200 mod 128 = 72`；
    /// `FPos += 72 = 200`；`while BIndex &gt; 0`：`FCurBlock := NextEx`、`BIndex = 0` → 结束。
    /// 实测：返回 **200**、`curPos = 72`（第 2 块）。
    /// </para>
    /// <para>
    /// ★ 随后 `Seek(10, Current)` 的结果是 **338**（而不是直觉的 210）：
    /// `BIndex := (72 + 10) div 128 = 0`；`FPos += 128 - 72 = 56` → 256；
    /// `curPos := (72 + 10) mod 128 = 82`；`FPos += 82 = 338`。实测：**338**、`curPos = 82`。
    /// 根因是原 :951 无条件加上"当前块剩余"（`FBlockSize - FCurBlockPos`），
    /// 而原 :952 的新 `curPos` 又已经包含 `Offset` —— 两者叠加导致 `FPos` 被多加
    /// `FBlockSize - FCurBlockPos`。这是原文的**已知记账偏差**，本用例固定它。
    /// </para>
    /// </summary>
    [Fact]
    public void Seek_Current_ForwardAcrossBlocks()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);
        s.Seek(0, SeekOrigin.Begin);

        Assert.Equal(200, s.Seek(200, SeekOrigin.Current));   // 原 :950-955
        Assert.Equal(200, s.PositionInternal);
        Assert.Equal(72, s.CurBlockPosInternal);
        Assert.Equal(300, s.Length);                          // Seek 不改 FSize

        // ★ 原 :951 + :953 的叠加：FPos 被多加 (FBlockSize - FCurBlockPos) = 56
        Assert.Equal(338, s.Seek(10, SeekOrigin.Current));
        Assert.Equal(338, s.PositionInternal);
        Assert.Equal(82, s.CurBlockPosInternal);
    }

    /// <summary>soCurrent 负向不跨块（原 :964-967）。</summary>
    [Fact]
    public void Seek_Current_BackwardWithinBlock()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);
        s.Seek(200, SeekOrigin.Begin);
        Assert.Equal(190, s.Seek(-10, SeekOrigin.Current));
        Assert.Equal(190, s.PositionInternal);
        Assert.Equal(62, s.CurBlockPosInternal);                 // 190 - 128
    }

    /// <summary>
    /// ★ soCurrent 负向跨块回退（原 :969-984）：先把游标退到上一块尾部（`FCurBlockPos := FBlockSize`）
    /// 再递归 `Seek(Offset, soCurrent)`。
    /// </summary>
    [Fact]
    public void Seek_Current_BackwardAcrossBlocks()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);
        s.Seek(130, SeekOrigin.Begin);
        Assert.Equal(2, s.CurBlockPosInternal);
        Assert.Equal(126, s.Seek(-4, SeekOrigin.Current));
        Assert.Equal(126, s.PositionInternal);
        Assert.Equal(126, s.CurBlockPosInternal);
        Assert.Same(s.Head, s.CurBlockInternal);
    }

    /// <summary>
    /// soCurrent 负向退过头（原 :972-976）：`FCurBlock := FCurBlock^.PrevEx` 为 nil 时
    /// 落回 `FHead` + `FCurBlockPos = 0`。
    /// </summary>
    [Fact]
    public void Seek_Current_BackwardPastHead_FallsBackToHead()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);
        s.Seek(130, SeekOrigin.Begin);                // 第 2 块内 2（FPosition = 130）
        Assert.Equal(100, s.Seek(-30, SeekOrigin.Current));
        Assert.Same(s.Head, s.CurBlockInternal);
        Assert.Equal(100, s.PositionInternal);
    }

    /// <summary>
    /// `Seek(Offset, soEnd)` 的完整取值表（原 :987-1023）。
    /// <para>
    /// 实测推导（MB_Small，写 300 = 3 块、`FSize = 300`、`FCapacity = 384`、`FBlockSize = 128`；
    /// 探针 D04。注意原 :989-990 `if Offset &lt; 0 then Offset := -Offset` 会把负偏移**取反**，
    /// 因此正负偏移走同一分支）：
    /// <list type="bullet">
    /// <item>`FLastSize := FBlockSize - FCapacity + FSize = 128 - 384 + 300 = 44`（原 :993）；</item>
    /// <item>`Seek(0, End)`：`0 &lt; 300` 为真 → `0 &lt;= 44` → `curPos := 44 - 0 = 44`、
    /// `FPos := 300 - 0 = **300**`（**不是** 0）；</item>
    /// <item>`Seek(1, End)` → `curPos = 43`、`FPos = **299**`；</item>
    /// <item>`Seek(44, End)` → `curPos := 44 - 44 = 0`、`FPos = **256**`；</item>
    /// <item>`Seek(45, End)`：`45 &gt; 44` → 走原 :1000-1014 的"倒数多块"分支：
    /// `FCurBlock := FLast^.PrevEx`（第 2 块）、`Offset := 45 - 44 = 1`、`FPos := 300 - 44 = 256`、
    /// `BIndex := 1 div 128 = 0`、`Offset := 1`、`curPos := 128 - 1 = 127`、
    /// `FPos -= 128 - 127 = 1` → **255**；</item>
    /// <item>`Seek(172, End)` → `Offset := 172 - 44 = 128`、`BIndex := 1`、`Offset := 0`、
    /// `curPos := 128`、`FPos := 256 - 0 = 256`，`BIndex = 1` 再 `FCurBlock := PrevEx`（第 1 块）、
    /// `FPos -= 128` → **128**；</item>
    /// <item>`Seek(300, End)`：`300 &lt; 300` 为**假** → 原 :1017-1022 `FPos := 0; curPos := 0;
    /// FCurBlock := FHead` → **0**（唯一落 0 的情形）。</item>
    /// </list>
    /// </para>
    /// <para>
    /// ★ 结论：`Seek(0, soEnd)` == `FSize`（不是 0）；只有 `Offset &gt;= FSize` 才落到 0。
    /// 且结果与 `FPosition` 的当前值**无关**（原 :991 比较的是 `FSize`）。
    /// </para>
    /// </summary>
    [Theory]
    [InlineData(0, 300, 44)]
    [InlineData(1, 299, 43)]
    [InlineData(44, 256, 0)]
    [InlineData(45, 255, 127)]
    [InlineData(172, 128, 128)]
    [InlineData(300, 0, 0)]
    public void Seek_End_PositiveOffset_CountsBackFromEnd(int offset, int expectedPos, int expectedCurPos)
    {
        // (a) 先 Seek(0, Begin) 再 Seek(offset, End)
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);
        s.Seek(0, SeekOrigin.Begin);
        Assert.Equal(expectedPos, s.Seek(offset, SeekOrigin.End));
        Assert.Equal(expectedPos, s.PositionInternal);
        Assert.Equal(expectedCurPos, s.CurBlockPosInternal);

        // (b) 不预先定位 —— 结果与 (a) 相同（原 :991 只看 FSize，与 FPosition 无关）
        var t = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        t.Write(Make(300), 300);
        Assert.Equal(expectedPos, t.Seek(offset, SeekOrigin.End));
        Assert.Equal(expectedPos, t.PositionInternal);
    }

    /// <summary>负偏移被原 :989-990 取反 ⇒ 与同值正偏移完全等价。</summary>
    [Fact]
    public void Seek_End_NegativeOffset_IsNegated()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);
        s.Seek(0, SeekOrigin.Begin);
        Assert.Equal(250, s.Seek(-50, SeekOrigin.End));
        Assert.Equal(250, s.PositionInternal);

        var t = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        t.Write(Make(300), 300);
        t.Seek(0, SeekOrigin.Begin);
        Assert.Equal(250, t.Seek(50, SeekOrigin.End));
        Assert.Equal(250, t.PositionInternal);
    }

    /// <summary>空流（FHead = nil）时 Seek 一律返回 0（原 :1027-1028）。</summary>
    [Fact]
    public void Seek_EmptyStream_ReturnsZero()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        Assert.Equal(0, s.Seek(10, SeekOrigin.Begin));
        Assert.Equal(0, s.Seek(-10, SeekOrigin.Current));
        Assert.Equal(0, s.Seek(10, SeekOrigin.End));
        Assert.Equal(0, s.PositionInternal);
    }

    /// <summary>
    /// 越界 Seek 后再 SetSize 的位置校正（原 :1105-1106）。
    /// <para>
    /// 实测推导（探针 D05）：
    /// <list type="bullet">
    /// <item>写 256（= 2 块）后 `Seek(9999, Begin)`：`9999 &gt; FSize(256)` → 原 :939-944
    /// `FCurBlock := FLast`、`FCurBlockPos := FCapacity - FSize = 256 - 256 = 0`、`FPos := 256`。
    /// 实测：返回 **256**、`curPos = 0`、`blocks = 2`。</item>
    /// <item>`SetLength(200)`：`CurCount := 256 div 128 = 2`、`FMemBlockCount := 200 div 128 = 1`，
    /// `200 mod 128 ≠ 0` 故 +1 → **2**。两者相等 → **块数不变**，`FCapacity` 保持 **256**；
    /// `FPosition(256) &gt; 200` → 原 :1106 `Position := 200`（走 Seek）→ `Seek(200, Begin)`：
    /// `curPos := 200 mod 128 = 72`、`BIndex := 1`、`FPos := 72 + 128 = 200`。
    /// 实测：`FPos = 200`、`blocks = **2**`、`cap = 256`、`curPos = 72`。</item>
    /// </list>
    /// </para>
    /// <para>
    /// ★ 结论：`SetLength(200)` 时块数**不变**（旧容量 256 已能容纳 200），
    /// 所以 `FMemBlockCount` 仍是 2（先前"断言 1"是误把容量当成按 NewSize 重算）。
    /// </para>
    /// </summary>
    [Fact]
    public void Seek_BeyondSize_ThenGrow_PositionIsRealigned()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(256), 256);
        Assert.Equal(256, s.Seek(9999, SeekOrigin.Begin));   // → FSize
        Assert.Same(s.Last, s.CurBlockInternal);
        Assert.Equal(0, s.CurBlockPosInternal);              // FCapacity - FSize = 0
        Assert.Equal(2, s.FMemBlockCount);

        s.SetLength(200);
        Assert.Equal(200, s.PositionInternal);               // 原 :1105-1106 校正
        Assert.Equal(72, s.CurBlockPosInternal);             // 200 mod 128
        Assert.Equal(2, s.FMemBlockCount);                   // 块数不变（容量 256 够用）
        Assert.Equal(256, s.CapacityInternal);

        // 300 → 200 的同类校正
        var t = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        t.Write(Make(300), 300);
        Assert.Equal(300, t.Seek(9999, SeekOrigin.Begin));
        t.SetLength(200);
        Assert.Equal(200, t.PositionInternal);
        Assert.Equal(72, t.CurBlockPosInternal);
        Assert.Equal(2, t.FMemBlockCount);
    }

    // ---------------------------------------------------------------------------------
    // 5. SetLength 扩大/缩小（原 :1040-1114）
    // ---------------------------------------------------------------------------------

    /// <summary>扩大：块数增加（原 :1071-1090），FCapacity 更新（原 :1091）。</summary>
    [Fact]
    public void SetLength_Grow_AddsBlocks()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(100), 100);
        Assert.Equal(1, s.FMemBlockCount);
        Assert.Equal(128, s.CapacityInternal);
        s.SetLength(300);
        Assert.Equal(300, s.Length);
        Assert.Equal(3, s.FMemBlockCount);
        Assert.Equal(384, s.CapacityInternal);
    }

    /// <summary>扩大后原数据不丢，且可继续写。</summary>
    [Fact]
    public void SetLength_Grow_PreservesData()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        byte[] data = Make(100);
        s.Write(data, 100);
        s.SetLength(300);
        s.Seek(0, SeekOrigin.Begin);
        byte[] buf = new byte[100];
        Assert.Equal(100, s.Read(buf, 0, 100));
        Assert.Equal(data, buf);
        s.Seek(100, SeekOrigin.Begin);
        s.Write(Make(50), 50);
        Assert.Equal(300, s.Length);                     // 150 < 300 → 不扩
        Assert.Equal(150, s.PositionInternal);
    }

    /// <summary>缩小：从 FLast 往前回收块（原 :1064-1070）。</summary>
    [Fact]
    public void SetLength_Shrink_ReclaimsBlocks()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);
        Assert.Equal(3, s.FMemBlockCount);
        s.SetLength(100);
        Assert.Equal(100, s.Length);
        Assert.Equal(1, s.FMemBlockCount);
        Assert.Equal(128, s.CapacityInternal);
        s.Seek(0, SeekOrigin.Begin);
        byte[] buf = new byte[100];
        Assert.Equal(100, s.Read(buf, 0, 100));
        Assert.Equal(Make(100), buf);
    }

    /// <summary>缩小到非 0、再扩回来的往返。</summary>
    [Fact]
    public void SetLength_ShrinkToNonZeroThenGrowAgain()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);
        s.SetLength(129);
        Assert.Equal(2, s.FMemBlockCount);
        Assert.Equal(256, s.CapacityInternal);
        s.SetLength(300);
        Assert.Equal(3, s.FMemBlockCount);
        Assert.Equal(384, s.CapacityInternal);
    }

    /// <summary>
    /// Dispose（原 :548-552 destructor → SetSize(0)）把块归还池。
    /// <para>
    /// 实测推导（探针 D06）：写 300 后 `Dispose`：`FSize = 0`、`Head/Last = null`、块数 0、
    /// 容量 0、位置 0；全局 `TotalMemBytes` **不变**（归还的块进入池的空闲链，
    /// 原 :415 判定"入链"而非真释放；且全局账目会被并行测试扰动）。
    /// 因此这里断言**结构与大小**，不断言全局字节数。
    /// </para>
    /// </summary>
    [Fact]
    public void Dispose_ReturnsAllBlocks()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);
        Assert.Equal(3, s.FMemBlockCount);

        s.Dispose();

        Assert.Equal(0, s.Length);
        Assert.Null(s.Head);
        Assert.Null(s.Last);
        Assert.Null(s.CurBlockInternal);
        Assert.Equal(0, s.FMemBlockCount);
        Assert.Equal(0, s.CapacityInternal);
        Assert.Equal(0, s.Position);
    }

    // ---------------------------------------------------------------------------------
    // 6. SaveCurBlock / RestoreCurBlock（原 :849-859）
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// ★ 差异断言：`SaveCurBlock`/`RestoreCurBlock` 只回滚**物理游标**
    /// （`FCurBlock`/`FCurBlockPos`），**不回滚 `FPosition`**（原 :849-859 无该语句）。
    /// <para>
    /// 实测推导（探针 D07）：写 300 → `Seek(10, Begin)`（`curPos = 10`、`FPos = 10`）→ Save（标记 10）
    /// → `Seek(130, Begin)`（第 2 块 `curPos = 2`、`FPos = 130`）→ Restore：
    /// 游标回到 `FMarkBlock` 的偏移 **10**，但 `FPosition` 仍是 **130** —— 两者不再对应。
    /// 原 :851-852 只写 `FCurBlock := FMarkBlock; FCurBlockPos := FMarkBlokPos;`。
    /// </para>
    /// </summary>
    [Fact]
    public void SaveCurBlock_RestoreCurBlock_RestoresCursorButNotFPosition()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);

        s.Seek(10, SeekOrigin.Begin);
        Assert.Equal(10, s.CurBlockPosInternal);
        Assert.Equal(10, s.PositionInternal);
        s.SaveCurBlock();
        Assert.Same(s.CurBlockInternal, s.MarkBlockInternal);
        Assert.Equal(10, s.MarkBlokPosInternal);

        s.Seek(130, SeekOrigin.Begin);
        Assert.Equal(2, s.CurBlockPosInternal);
        Assert.Equal(130, s.PositionInternal);

        s.RestoreCurBlock();
        Assert.Same(s.Head, s.CurBlockInternal);         // 游标回到标记块（第 1 块）
        Assert.Equal(10, s.CurBlockPosInternal);         // 原 :852
        Assert.Equal(130, s.PositionInternal);           // ★ 未回滚（原 :849-853 无该语句）
    }

    /// <summary>未 Save 就 Restore → 回到 nil/0（原 :851-852 把 FCurBlock 设为 FMarkBlock = nil）。</summary>
    [Fact]
    public void RestoreCurBlock_WithoutSave_GoesToNull()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);
        s.RestoreCurBlock();
        Assert.Null(s.CurBlockInternal);
        Assert.Equal(0, s.CurBlockPosInternal);
        Assert.Equal(0, s.Read(new byte[10], 0, 10));    // FCurBlock = nil → 读 0
    }

    /// <summary>
    /// ★ 差异断言（探针 D07 第二组）：`Seek(FSize, Begin)` 后 Save，标记为 `curPos = 44`
    /// （`FCapacity - FSize`，原 :939-944）；再 `Seek(0, Begin)`（`curPos = 0`、`FPos = 0`）；
    /// Restore 后游标回到 **44**，而 `FPosition` 仍是 **0** —— 于是"游标指向第 3 块偏移 44（数据末尾）"
    /// 但"逻辑位置是 0"。此后任何依赖 `FPosition` 的长度判断都会与实际取数位置错位。
    /// </summary>
    [Fact]
    public void SaveCurBlock_DoesNotRestoreFPosition_ObservableDivergence()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        s.Write(Make(300), 300);

        Assert.Equal(300, s.Seek(300, SeekOrigin.Begin));    // 原 :939-944：curPos = FCapacity - FSize
        Assert.Equal(44, s.CurBlockPosInternal);
        Assert.Same(s.Last, s.CurBlockInternal);
        s.SaveCurBlock();
        Assert.Equal(44, s.MarkBlokPosInternal);

        s.Seek(0, SeekOrigin.Begin);
        Assert.Equal(0, s.PositionInternal);
        Assert.Same(s.Head, s.CurBlockInternal);

        s.RestoreCurBlock();
        Assert.Same(s.Last, s.CurBlockInternal);             // 游标回到末块
        Assert.Equal(44, s.CurBlockPosInternal);             // 原 :852
        Assert.Equal(0, s.PositionInternal);                 // ★ 未回滚
    }

    // ---------------------------------------------------------------------------------
    // 7. SaveToStream / LoadFromStream 往返（原 :876-902、:665-699）
    // ---------------------------------------------------------------------------------

    /// <summary>SaveToStream 只写有效数据（末块按 `FBlockSize - FCapacity + FSize`，原 :898）。</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(127)]
    [InlineData(128)]
    [InlineData(129)]
    [InlineData(300)]
    [InlineData(256)]
    public void SaveToStream_WritesExactlySizeBytes(int n)
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        byte[] data = Make(n);
        s.Write(data, n);
        using var ms = new MemoryStream();
        s.SaveToStream(ms);
        Assert.Equal(n, ms.Length);
        Assert.Equal(data, ms.ToArray());
    }

    /// <summary>SaveToStream 在 FHead = nil 时不写任何东西（原 :881）。</summary>
    [Fact]
    public void SaveToStream_EmptyStream_WritesNothing()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        using var ms = new MemoryStream();
        s.SaveToStream(ms);
        Assert.Equal(0, ms.Length);
    }

    /// <summary>
    /// 往返：SaveToStream → 新流 LoadFromStream，数据与大小完全一致；
    /// LoadFromStream 末块按 `FBlockSize - (FCapacity - FSize)` 读（原 :689）。
    /// </summary>
    [Theory]
    [InlineData(1)]
    [InlineData(128)]
    [InlineData(129)]
    [InlineData(300)]
    public void LoadFromStream_RoundTrip(int n)
    {
        var src = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        byte[] data = Make(n);
        src.Write(data, n);
        using var ms = new MemoryStream();
        src.SaveToStream(ms);

        var dst = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        ms.Position = 0;
        dst.LoadFromStream(ms);

        Assert.Equal(n, dst.Length);
        Assert.Equal(src.FMemBlockCount, dst.FMemBlockCount);
        Assert.Equal(0, dst.PositionInternal);
        Assert.Same(dst.Head, dst.CurBlockInternal);

        byte[] buf = new byte[n];
        Assert.Equal(n, dst.Read(buf, 0, n));
        Assert.Equal(data, buf);
    }

    /// <summary>LoadFromStream 先把源流 Position 置 0（原 :671）。</summary>
    [Fact]
    public void LoadFromStream_ResetsSourcePositionToZero()
    {
        var src = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        src.Write(Make(300), 300);
        using var ms = new MemoryStream();
        src.SaveToStream(ms);
        ms.Position = 250;                                // 故意挪开

        var dst = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        dst.LoadFromStream(ms);
        Assert.Equal(300, dst.Length);
    }

    /// <summary>LoadFromStream 空流 → 保持空（原 :673 `if FSize &lt;&gt; 0`）。</summary>
    [Fact]
    public void LoadFromStream_EmptySource_LeavesStreamEmpty()
    {
        var dst = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        dst.LoadFromStream(new MemoryStream(Array.Empty<byte>()));
        Assert.Equal(0, dst.Length);
        Assert.Equal(0, dst.FMemBlockCount);
    }

    /// <summary>
    /// ★ 差异断言（探针 D02/D08）：`SaveToStream`（原 :876-902）**不依赖 FCurBlock/FPosition**
    /// （它从 FHead 沿 NextEx 走），因此即使游标处于"块尾哨兵"状态（`FCurBlockPos = FBlockSize`）
    /// 也能正确导出；而 `LoadFromStream`（原 :665-699）读完后**显式复位**
    /// `FPosition := 0; FCurBlock := FHead; FCurBlockPos := 0`（原 :695-697），
    /// 所以刚 Load 完的流可以立刻顺序 Read。
    /// </summary>
    [Fact]
    public void SaveToStream_IndependentOfCursor_LoadFromStreamResetsCursor()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        byte[] data = Make(256);
        s.Write(data, 256);                                   // 正好 2 块，游标 = 末块 + 128
        Assert.Equal(128, s.CurBlockPosInternal);
        Assert.Same(s.Last, s.CurBlockInternal);

        using var ms = new MemoryStream();
        s.SaveToStream(ms);                                   // 不依赖游标
        Assert.Equal(256, ms.Length);
        Assert.Equal(data, ms.ToArray());

        var dst = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        ms.Position = 0;
        dst.LoadFromStream(ms);
        Assert.Equal(0, dst.PositionInternal);                // 原 :695
        Assert.Same(dst.Head, dst.CurBlockInternal);          // 原 :696
        Assert.Equal(0, dst.CurBlockPosInternal);             // 原 :697

        byte[] back = new byte[256];
        Assert.Equal(256, dst.Read(back, 0, 256));
        Assert.Equal(data, back);
    }

    /// <summary>SaveToFile/LoadFromFile 往返（原 :861-874、:653-663）。</summary>
    [Fact]
    public void SaveToFile_LoadFromFile_RoundTrip()
    {
        string path = Path.Combine(Path.GetTempPath(), "gxx_ubuffer_" + Guid.NewGuid().ToString("N") + ".bin");
        try
        {
            var src = new TDxMemoryStream(TDxMemBlockType.MB_Small);
            byte[] data = Make(300);
            src.Write(data, 300);
            src.SaveToFile(path);

            var dst = new TDxMemoryStream(TDxMemBlockType.MB_Small);
            dst.LoadFromFile(path);
            Assert.Equal(300, dst.Length);
            dst.Seek(0, SeekOrigin.Begin);
            byte[] buf = new byte[300];
            Assert.Equal(300, dst.Read(buf, 0, 300));
            Assert.Equal(data, buf);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    /// <summary>SaveToFile 在 FHead = nil 时**不创建文件**（原 :865 守卫）。</summary>
    [Fact]
    public void SaveToFile_EmptyStream_DoesNotCreateFile()
    {
        string path = Path.Combine(Path.GetTempPath(), "gxx_ubuffer_empty_" + Guid.NewGuid().ToString("N") + ".bin");
        try
        {
            var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
            s.SaveToFile(path);
            Assert.False(File.Exists(path));
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    // ---------------------------------------------------------------------------------
    // 8. ReadStream / WriteStream（原 :789-847、:1244-1316）
    // ---------------------------------------------------------------------------------

    /// <summary>ReadStream：把本流当前位置起 len 字节推给目标流（原 :789-847）。</summary>
    [Fact]
    public void ReadStream_PushesDataToTarget()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        byte[] data = Make(300);
        s.Write(data, 300);
        s.Seek(100, SeekOrigin.Begin);
        using var ms = new MemoryStream();
        s.ReadStream(ms, 150);
        Assert.Equal(150, ms.Length);
        Assert.Equal(data.AsSpan(100, 150).ToArray(), ms.ToArray());
        Assert.Equal(250, s.PositionInternal);
    }

    /// <summary>ReadStream 的 len 超过剩余时截断（原 :805-806）。</summary>
    [Fact]
    public void ReadStream_LenBeyondRemaining_IsTruncated()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        byte[] data = Make(300);
        s.Write(data, 300);
        s.Seek(290, SeekOrigin.Begin);
        using var ms = new MemoryStream();
        s.ReadStream(ms, 100);
        Assert.Equal(10, ms.Length);
        Assert.Equal(data.AsSpan(290, 10).ToArray(), ms.ToArray());
    }

    /// <summary>ReadStream 在 FCurBlock = nil 时什么都不做（原 :794）。</summary>
    [Fact]
    public void ReadStream_EmptyStream_DoesNothing()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        using var ms = new MemoryStream();
        s.ReadStream(ms, 100);
        Assert.Equal(0, ms.Length);
    }

    /// <summary>WriteStream：单次调用从源流读 len 字节写入本流（原 :1244-1316）。</summary>
    [Fact]
    public void WriteStream_PullsDataFromSource()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        byte[] data = Make(300);
        using var ms = new MemoryStream(data);
        s.WriteStream(ms, 300);
        Assert.Equal(300, s.Length);
        Assert.Equal(3, s.FMemBlockCount);
        Assert.Equal(300, s.PositionInternal);
        Assert.Equal(44, s.CurBlockPosInternal);          // 300 - 256
        s.Seek(0, SeekOrigin.Begin);
        byte[] buf = new byte[300];
        Assert.Equal(300, s.Read(buf, 0, 300));
        Assert.Equal(data, buf);
    }

    /// <summary>
    /// ★ WriteStream 的**分次调用不累加**（新推出的期望值 + 原文缺陷，探针 D08）。
    /// <para>
    /// 原 :1258-1266 的扩容逻辑只在 `FCurBlock = nil` 或 `FCurBlock = FLast` 时触发，
    /// 且按 `MPool.FBlockSize - FCurBlockPos - Len + FSize` **重算** `FSize`（不是累加）。
    /// 实测（MB_Small，FBlockSize = 128）：
    /// <list type="bullet">
    /// <item>`WriteStream(ms, 100)`：`FCurBlock = nil` → 原 :1258 `SetSize(100)` → 写入；
    /// 实测 `FSize = 100`、`blocks = 1`、`curPos = 100`；</item>
    /// <item>`WriteStream(ms, 100)`：`FCurBlock = FLast` 且 `curPos(100) + 100 &gt; 128` →
    /// 原 :1263 `SetSize(128 - 100 - 100 + 100) = SetSize(28)`；随后一次写满（`Len = 100 &lt;= 128 - 100`？
    /// 否 → 走原 :1277-1287）把 `curPos` 推到 128。实测 `FSize = **28**`、`curPos = **128**`、
    /// `blocks = 1` —— 大小**倒退**了。</item>
    /// <item>`WriteStream(ms, 100)`：`FCurBlock` 已被上一轮循环跨成 nil（WriteStream **没有**
    /// Write 的 :1237-1241 复位）→ 原 :1258 又 `SetSize(100)`；但 `FCapacity(128) div 128 = 1`
    /// 与 `FMemBlockCount(1)` 相等 → 不重建游标 → 原 :1268 解引用 nil 之前，先由原 :1260 的
    /// `FCurBlock = nil` 判定… 实测 `FSize = **-72**`、`FPos = **-144**`、`cur = null`。</item>
    /// </list>
    /// </para>
    /// <para>
    /// ★ 结论：`WriteStream` **不是**可重复调用的追加接口（与 `Write` 不同）；单次调用才可用。
    /// 本用例固定这一实测行为，并断言托管侧的 nil 守卫让后续调用安全返回（原文为 AV）。
    /// </para>
    /// </summary>
    [Fact]
    public void WriteStream_RepeatedCallsDoNotAccumulate_AndLeaveNilCursor()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        byte[] data = Make(300);
        using var ms = new MemoryStream(data);

        s.WriteStream(ms, 100);
        Assert.Equal(100, s.Length);
        Assert.Equal(1, s.FMemBlockCount);
        Assert.Equal(100, s.CurBlockPosInternal);

        // ★ 原 :1263 重算而非累加：128 - 100 - 100 + 100 = 28；写满后 curPos = 128
        s.WriteStream(ms, 100);
        Assert.Equal(28, s.Length);
        Assert.Equal(128, s.CurBlockPosInternal);
        Assert.Equal(1, s.FMemBlockCount);

        // ★ 第三轮：FCurBlock 已为 nil（WriteStream 无 :1237-1241 复位）
        s.WriteStream(ms, 100);
        Assert.Equal(-72, s.Length);
        Assert.Equal(-144, s.PositionInternal);
        Assert.Null(s.CurBlockInternal);

        // 托管侧 nil 守卫：不再抛 NRE（原文此处为 AV）
        s.Write(Make(10), 10);
        s.WriteStream(ms, 10);

        // 单次调用才是正常用法
        var t = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        using var ms2 = new MemoryStream(Make(300));
        t.WriteStream(ms2, 300);
        Assert.Equal(300, t.Length);
        Assert.Equal(3, t.FMemBlockCount);
        Assert.Equal(44, t.CurBlockPosInternal);          // 300 - 256
        t.Seek(0, SeekOrigin.Begin);
        byte[] buf = new byte[300];
        Assert.Equal(300, t.Read(buf, 0, 300));
        Assert.Equal(Make(300), buf);
    }

    // ---------------------------------------------------------------------------------
    // 9. LinkToBufferList / LoadFromBufferList（原 :573-651）
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// LinkToBufferList（原 :573-616）。
    /// <para>
    /// 实测推导（写 300 = 3 块、`FCapacity = 384`、`FSize = 300`；探针 D09）：
    /// 块链接到缓冲链后，每块 `DataLen` 被设为**池块大小**（128，原 :600），
    /// 末块为 `FBlockSize - (FCapacity - FSize) = 128 - 84 = 44`（原 :603）。
    /// 实测：`block[0].DataLen = 128`、`block[1].DataLen = 128`、`block[2].DataLen = 44`、
    /// `chainCount = 3`、`link.ValidCount = **300**`（不是 340）。
    /// 同时本流被清空（`FHead/FLast/FPosition/FSize/FCapacity/FMemBlockCount` 全清零，原 :604-614），
    /// 但 `FMemBlockType` **不清**。
    /// </para>
    /// </summary>
    [Fact]
    public void LinkToBufferList_MovesChainAndClearsStream()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        byte[] data = Make(300);
        s.Write(data, 300);
        int cap = s.CapacityInternal;                       // 384
        int size = s.FSize;                                 // 300
        TMemoryBlock head = s.Head;

        var link = new TBufferLink();
        try
        {
            s.LinkToBufferList(link);

            // 本流清空
            Assert.Null(s.Head);
            Assert.Null(s.Last);
            Assert.Equal(0, s.Length);
            Assert.Equal(0, s.CapacityInternal);
            Assert.Equal(0, s.FMemBlockCount);
            Assert.Equal(0, s.PositionInternal);
            Assert.Null(s.CurBlockInternal);
            Assert.Equal(0, s.CurBlockPosInternal);
            Assert.Equal(TDxMemBlockType.MB_Small, s.MemBlockType);   // ★ 原文不重置 FMemBlockType

            // 缓冲链接到了这些块
            Assert.Same(head, link.HeadBlock);
            Assert.Equal(3, CountChain(link.HeadBlock));
            Assert.Equal(B, link.HeadBlock.DataLen);                       // 原 :600 非末块 = 池块大小
            Assert.Equal(B, link.HeadBlock.NextEx.DataLen);
            Assert.Equal(B - (cap - size), link.LastBlock.DataLen);        // 原 :603 = 128 - 84 = 44
            Assert.Equal(44, link.LastBlock.DataLen);
            Assert.Equal(cap - (cap - size), link.ValidCount);             // 300
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>LinkToBufferList 把两个流的链**首尾相接**（原 :582-586）。</summary>
    [Fact]
    public void LinkToBufferList_AppendsToExistingChain()
    {
        var a = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        a.Write(Make(10), 10);
        var b = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        b.Write(Make(20), 20);
        TMemoryBlock bHead = b.Head;

        var link = new TBufferLink();
        try
        {
            a.LinkToBufferList(link);
            b.LinkToBufferList(link);
            Assert.Equal(2, CountChain(link.HeadBlock));
            Assert.Same(bHead, link.HeadBlock.NextEx);      // b 接在 a 之后
            Assert.Same(link.HeadBlock, bHead.PrevEx);
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>LinkToBufferList 对空流（FHead = nil）不做任何事（原 :578）。</summary>
    [Fact]
    public void LinkToBufferList_EmptyStream_IsNoOp()
    {
        var s = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        var link = new TBufferLink();
        try
        {
            s.LinkToBufferList(link);
            Assert.Null(link.HeadBlock);
            Assert.Null(link.LastBlock);
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>
    /// LoadFromBufferList（原 :618-651）：先 SetSize(DataLen) 再按块从缓冲链读
    /// （末块读 `FBlockSize - (FCapacity - FSize)`，原 :641-644），随后复位游标。
    /// </summary>
    [Fact]
    public void LoadFromBufferList_RoundTripWithLink()
    {
        var src = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        byte[] data = Make(300);
        src.Write(data, 300);

        var link = new TBufferLink();
        try
        {
            src.LinkToBufferList(link);

            var dst = new TDxMemoryStream(TDxMemBlockType.MB_Small);
            dst.LoadFromBufferList(link, 300);
            Assert.Equal(300, dst.Length);
            Assert.Equal(3, dst.FMemBlockCount);
            Assert.Equal(0, dst.PositionInternal);
            Assert.Same(dst.Head, dst.CurBlockInternal);

            byte[] buf = new byte[300];
            Assert.Equal(300, dst.Read(buf, 0, 300));
            Assert.Equal(data, buf);
        }
        finally
        {
            link.Dispose();
        }
    }

    /// <summary>LoadFromBufferList(DataLen = 0) 只把流缩到 0（原 :624-625）。</summary>
    [Fact]
    public void LoadFromBufferList_ZeroLen_LeavesEmpty()
    {
        var link = new TBufferLink();
        try
        {
            var dst = new TDxMemoryStream(TDxMemBlockType.MB_Small);
            dst.LoadFromBufferList(link, 0);
            Assert.Equal(0, dst.Length);
            Assert.Equal(0, dst.FMemBlockCount);
        }
        finally
        {
            link.Dispose();
        }
    }

    // ---------------------------------------------------------------------------------
    // 10. SwapStreamLink（原 :1116-1152）
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// ★ 差异断言（新推出的期望值，探针 D10）：`SwapStreamLink`（原 :1116-1152）
    /// **不对称地**交换 `FSize`：原 :1136 `FSize := Stream.Size`、原 :1150 `Stream.FSize := OldSize`。
    /// <para>
    /// 实测（source 写 300 = 3 块、`FCapacity = 384`；target 空）：
    /// <list type="bullet">
    /// <item>source：`FSize = 0`、`FCapacity = 0`、`FMemBlockCount = 0`、`Head = null`、`CurBlock = null`；</item>
    /// <item>target：`FSize = **300**`（拿到 source 的旧值）、`FCapacity = 384`、`FMemBlockCount = 3`、
    /// `Head/Last` 指向 source 的链、`CurBlock = null`（原 :1137）、`FPosition = 0`（原 :1139）；</item>
    /// <item>target 的 `SaveToStream` 仍导出完整 **300** 字节（末块 = `128 - 384 + 300 = 44`）。</item>
    /// </list>
    /// </para>
    /// <para>
    /// ★ 结论：交换 `FSize` 的结果是"**各拿对方原来的 FSize**"，
    /// 本例恰好得到"source 变 0、target 得 300"——这是**正确**的（先前误判为"双方都变 0"）。
    /// </para>
    /// </summary>
    [Fact]
    public void SwapStreamLink_ExchangesChainAndSize()
    {
        var source = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        byte[] data = Make(300);
        source.Write(data, 300);
        var target = new TDxMemoryStream(TDxMemBlockType.MB_Small);

        TMemoryBlock srcHead = source.Head;
        TMemoryBlock srcLast = source.Last;
        int srcCap = source.CapacityInternal;                // 384
        int srcBlocks = source.FMemBlockCount;               // 3

        source.SwapStreamLink(target);

        // 块链易主
        Assert.Same(srcHead, target.Head);   // 原 :1132 FHead := Stream.FHead
        Assert.Same(srcLast, target.Last);   // 原 :1134 FLast := Stream.FLast
        Assert.Null(source.Head);            // 原 :1140 Stream.FHead := OldHead（空流 → nil）
        Assert.Null(source.Last);            // 原 :1141 Stream.FLast := OldLast（空流 → nil）

        // ★ 记账：各拿对方原来的 FSize（source 原 300、target 原 0）
        Assert.Equal(300, target.Length);
        Assert.Equal(0, source.Length);
        Assert.Equal(srcCap, target.CapacityInternal);
        Assert.Equal(srcBlocks, target.FMemBlockCount);
        Assert.Equal(0, source.CapacityInternal);
        Assert.Equal(0, source.FMemBlockCount);

        // 游标都被清空（原 :1137-1139、:1145-1149）
        Assert.Null(target.CurBlockInternal);
        Assert.Equal(0, target.PositionInternal);
        Assert.Null(source.CurBlockInternal);
        Assert.Equal(0, source.PositionInternal);

        // target 的块链完好：SaveToStream 仍能吐 300 字节
        using var ms = new MemoryStream();
        target.SaveToStream(ms);
        Assert.Equal(300, ms.Length);
        Assert.Equal(data, ms.ToArray());

        // 但 target 无法 Read（FCurBlock = nil，原 :707）
        Assert.Equal(0, target.Read(new byte[10], 0, 10));
    }

    /// <summary>SwapStreamLink 交换 FMemBlockType（原 :1128/:1151）。</summary>
    [Fact]
    public void SwapStreamLink_ExchangesMemBlockType()
    {
        var a = new TDxMemoryStream(TDxMemBlockType.MB_Small);
        a.Write(Make(10), 10);
        var b = new TDxMemoryStream(TDxMemBlockType.MB_Normal);
        a.SwapStreamLink(b);
        Assert.Equal(TDxMemBlockType.MB_Normal, a.MemBlockType);
        Assert.Equal(TDxMemBlockType.MB_Small, b.MemBlockType);
    }

    // ---------------------------------------------------------------------------------
    // helpers
    // ---------------------------------------------------------------------------------

    /// <summary>生成 n 字节的确定性测试数据（第 i 字节 = i &amp; 0xFF）。</summary>
    private static byte[] Make(int n)
    {
        byte[] b = new byte[n];
        for (int i = 0; i < n; i++) b[i] = (byte)i;
        return b;
    }

    private static int CountChain(TMemoryBlock head)
    {
        int n = 0;
        for (TMemoryBlock b = head; b != null; b = b.NextEx) n++;
        return n;
    }
}
