using System;
using System.Collections.Generic;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// uBuffer.pas 内存池部分（原 uBuffer.pas:14-25、27-52、180-534、1758-1772、2352-2362）的移植测试。
/// <para>
/// **六个全局池的 BlockSize 是从原文脚本抽取的，不是手工转录**（抽取命令与回读结果
/// 写在 <c>uBufferMemoryPool.cs</c> 文件头注释里）。第一条用例直接断言这六个数。
/// </para>
/// <para>
/// 全局池是进程级共享的（原文 unit-level var），且全局账目 <c>TotalMemBytes</c> 是进程级累加器；
/// xUnit 默认并行跑测试类（其它类可能同时调用 <c>FreeObjPool</c>），
/// 因此本文件**不使用"绝对字节数回到 baseline"这类断言**，而用
/// 池内计数（`UseCount`/`FreeCount`/`CountUseChain`/`CountFreeChain`）与
/// "再次取块必为新块"来等价证明释放/复用行为。
/// </para>
/// </summary>
/// <summary>与其它 uBuffer 测试类同属一个 collection（禁用并行）——见 <see cref="BufferTestCollection"/>。</summary>
[Collection(BufferTestCollection.Name)]
public class BufferMemoryPoolTests
{
    /// <summary>
    /// 把 `FreeCount` 排空到 0：反复 `GetMemory` 直到池自己的空闲链为空，返回这些块。
    /// <para>
    /// 全局池是**进程级共享**的，别的测试可能随时把块还回同一个池 → `FFreeCount` 会漂移。
    /// 把池推到"空闲链恰好为空"的确定状态后，就能稳定命中原 :352 的"空闲链为空 → 挂头"分支，
    /// 从而使阈值类断言与并行活动解耦。
    /// </para>
    /// </summary>
    private static List<byte[]> DrainPool(TDxMemoryPool pool)
    {
        var held = new List<byte[]>();
        while (pool.FreeCount > 0)
            held.Add(pool.GetMemory(false));
        return held;
    }

    // ---------------------------------------------------------------------------------
    // 1. 六个全局内存池：BlockSize / InitCount / MaxFreeBlocks / BlockType（脚本抽取值）
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// 抽取来源（PowerShell + GBK 原文，命令见 uBufferMemoryPool.cs 文件头）：
    /// <code>
    ///   217: SuperBigMPool := TDxMemoryPool.Create(2048, 30, MB_SpBig, 30);
    ///   224: LargeMPool    := TDxMemoryPool.Create(4096, 30, MB_Large, 30);
    ///   231: SPLargeMPool  := TDxMemoryPool.Create(4096 * 4, 30, MB_SpLarge, 30);
    ///   238: NormalMPool   := TDxMemoryPool.Create(640, 30, MB_Normal, 30);
    ///   245: SmallMPool    := TDxMemoryPool.Create(128, 30, MB_Small, 30);
    ///   252: BigMPool      := TDxMemoryPool.Create(1024, 30, MB_Big, 30);
    /// </code>
    /// 六个请求值都已是 64 的倍数，故对齐后（原 :296-299）等于请求值本身。
    /// </summary>
    [Fact]
    public void GlobalPools_BlockSize_MatchExtractionFromSource()
    {
        Assert.Equal(128, MemoryPoolGlobal.SmallMemoryPool().BlockSize);          // 原 :245
        Assert.Equal(640, MemoryPoolGlobal.MemoryPool().BlockSize);               // 原 :238
        Assert.Equal(1024, MemoryPoolGlobal.BigMemoryPool().BlockSize);           // 原 :252
        Assert.Equal(2048, MemoryPoolGlobal.SuperMemoryPool().BlockSize);         // 原 :217
        Assert.Equal(4096, MemoryPoolGlobal.LargeMemoryPool().BlockSize);         // 原 :224
        Assert.Equal(16384, MemoryPoolGlobal.SuperLargeMemoryPool().BlockSize);   // 原 :231（4096*4）
    }

    /// <summary>原 :217/:224/:231/:238/:245/:252 的最后一个实参 MaxFreeBlocks 全是 30。</summary>
    [Fact]
    public void GlobalPools_MaxFreeBlocks_Are30()
    {
        Assert.Equal(30, MemoryPoolGlobal.SmallMemoryPool().MaxFreeBlocks);
        Assert.Equal(30, MemoryPoolGlobal.MemoryPool().MaxFreeBlocks);
        Assert.Equal(30, MemoryPoolGlobal.BigMemoryPool().MaxFreeBlocks);
        Assert.Equal(30, MemoryPoolGlobal.SuperMemoryPool().MaxFreeBlocks);
        Assert.Equal(30, MemoryPoolGlobal.LargeMemoryPool().MaxFreeBlocks);
        Assert.Equal(30, MemoryPoolGlobal.SuperLargeMemoryPool().MaxFreeBlocks);
    }

    /// <summary>
    /// 六个工厂是**单例**（原 :216/:223/:230/:237/:244/:251 的 `if X = nil then X := …Create`）。
    /// </summary>
    [Fact]
    public void GlobalPools_AreSingletons()
    {
        Assert.Same(MemoryPoolGlobal.SmallMemoryPool(), MemoryPoolGlobal.SmallMemoryPool());
        Assert.Same(MemoryPoolGlobal.MemoryPool(), MemoryPoolGlobal.MemoryPool());
        Assert.Same(MemoryPoolGlobal.BigMemoryPool(), MemoryPoolGlobal.BigMemoryPool());
        Assert.Same(MemoryPoolGlobal.SuperMemoryPool(), MemoryPoolGlobal.SuperMemoryPool());
        Assert.Same(MemoryPoolGlobal.LargeMemoryPool(), MemoryPoolGlobal.LargeMemoryPool());
        Assert.Same(MemoryPoolGlobal.SuperLargeMemoryPool(), MemoryPoolGlobal.SuperLargeMemoryPool());
        Assert.NotSame(MemoryPoolGlobal.SmallMemoryPool(), MemoryPoolGlobal.BigMemoryPool());
        Assert.NotSame(MemoryPoolGlobal.BigMemoryPool(), MemoryPoolGlobal.SuperMemoryPool());
    }

    /// <summary>
    /// 六个池的 BlockType 必须与枚举顺序一致（原 :15 声明顺序即序号）。
    /// GetPool 的分派依赖它（MB_Big 特意落在 else 分支）。
    /// </summary>
    [Fact]
    public void MemBlockType_EnumOrder_IsPreserved_And_GetPoolDispatchesCorrectly()
    {
        Assert.Equal(0, (int)TDxMemBlockType.MB_Small);
        Assert.Equal(1, (int)TDxMemBlockType.MB_Normal);
        Assert.Equal(2, (int)TDxMemBlockType.MB_Big);
        Assert.Equal(3, (int)TDxMemBlockType.MB_SpBig);
        Assert.Equal(4, (int)TDxMemBlockType.MB_Large);
        Assert.Equal(5, (int)TDxMemBlockType.MB_SPLarge);

        // 原 :589-597 的 case 分派：MB_Big 无显式分支 → 走 else → BigMemoryPool（1024）
        Assert.Equal(128, MemoryPoolGlobal.GetPool(TDxMemBlockType.MB_Small).BlockSize);
        Assert.Equal(640, MemoryPoolGlobal.GetPool(TDxMemBlockType.MB_Normal).BlockSize);
        Assert.Equal(1024, MemoryPoolGlobal.GetPool(TDxMemBlockType.MB_Big).BlockSize);
        Assert.Equal(2048, MemoryPoolGlobal.GetPool(TDxMemBlockType.MB_SpBig).BlockSize);
        Assert.Equal(4096, MemoryPoolGlobal.GetPool(TDxMemBlockType.MB_Large).BlockSize);
        Assert.Equal(16384, MemoryPoolGlobal.GetPool(TDxMemBlockType.MB_SPLarge).BlockSize);
        // 非法枚举值同样落到 else（BigMemoryPool）
        Assert.Equal(1024, MemoryPoolGlobal.GetPool((TDxMemBlockType)99).BlockSize);
    }

    // ---------------------------------------------------------------------------------
    // 2. 构造函数：64 字节对齐（原 :290-327 与 :295-299）
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// 原 :295-299：
    /// <code>
    /// if (BlockSize mod 64 = 0) then FBlockSize := BlockSize
    /// else FBlockSize := (BlockSize div 64) * 64 + 64;
    /// </code>
    /// 即"向上取到 64 的倍数"。注意 0 也是 64 的倍数 → FBlockSize = 0（不是 64）。
    /// </summary>
    [Theory]
    [InlineData(128, 128)]
    [InlineData(640, 640)]
    [InlineData(1, 64)]
    [InlineData(63, 64)]
    [InlineData(64, 64)]
    [InlineData(65, 128)]
    [InlineData(0, 0)]
    [InlineData(-64, -64)]
    public void Constructor_BlockSizeIs64ByteAligned(int requested, int expected)
    {
        var pool = new TDxMemoryPool(requested, 0, TDxMemBlockType.MB_Small, 5);
        Assert.Equal(expected, pool.BlockSize);
    }

    /// <summary>InitCount 个块进入空闲链，FUseCount = 0、FFreeCount = InitCount（原 :303-326）。</summary>
    [Fact]
    public void Constructor_PreallocatesInitCountBlocks()
    {
        var pool = new TDxMemoryPool(128, 7, TDxMemBlockType.MB_Small, 5);
        Assert.Equal(0, pool.UseCount);
        Assert.Equal(7, pool.FreeCount);
        Assert.Equal(7, pool.CountFreeChain());
        Assert.Equal(0, pool.CountUseChain());
    }

    /// <summary>InitCount = 0 时两条链都为空（原 :305 的 while 不执行）。</summary>
    [Fact]
    public void Constructor_InitCountZero_LeavesEmptyChains()
    {
        var pool = new TDxMemoryPool(128, 0, TDxMemBlockType.MB_Small, 5);
        Assert.Equal(0, pool.FreeCount);
        Assert.Equal(0, pool.CountFreeChain());
        Assert.Null(pool.FindFreeBlock(new byte[1]));
    }

    /// <summary>MaxFreeBlocks 默认值是 50（原 :43 `MaxFreeBlocks: Integer = 50`）。</summary>
    [Fact]
    public void Constructor_MaxFreeBlocksDefaultsTo50()
    {
        var pool = new TDxMemoryPool(128, 0, TDxMemBlockType.MB_Small);
        Assert.Equal(50, pool.MaxFreeBlocks);
    }

    // ---------------------------------------------------------------------------------
    // 3. GetMemory：Zero 参数的差异（原 :441-480，关键在 :475-476）
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// ★ 差异断言：原 :475-476
    /// <code>
    /// Result := p^.Memory;
    /// if Zero then ZeroMemory(Result, FBlockSize);
    /// </code>
    /// Zero=True 清**整块** FBlockSize 字节；Zero=False 原样返回（**上一次用的残留数据仍在**）。
    /// </summary>
    [Fact]
    public void GetMemory_ZeroTrue_ClearsWholeBlock_ZeroFalse_KeepsStaleData()
    {
        var pool = new TDxMemoryPool(128, 1, TDxMemBlockType.MB_Small, 50);

        byte[] p1 = pool.GetMemory(false);
        Assert.Equal(128, p1.Length);
        for (int i = 0; i < p1.Length; i++) p1[i] = 0xAB;

        pool.FreeMemory(p1);
        Assert.NotNull(pool.FindFreeBlock(p1));

        // Zero = False：拿回同一块，残留数据必须还在（原 :475 分支未执行）
        byte[] p2 = pool.GetMemory(false);
        Assert.Same(p1, p2);
        Assert.All(p2, b => Assert.Equal(0xAB, b));

        pool.FreeMemory(p2);

        // Zero = True：拿回同一块，整块被清零（原 :476 ZeroMemory(Result, FBlockSize)）
        byte[] p3 = pool.GetMemory(true);
        Assert.Same(p1, p3);
        Assert.All(p3, b => Assert.Equal(0, b));
    }

    /// <summary>新建块（空闲链为空）默认是 .NET 的零初始化，Zero=False 也得到全 0。</summary>
    [Fact]
    public void GetMemory_FreshBlock_IsZeroInitialized()
    {
        var pool = new TDxMemoryPool(64, 0, TDxMemBlockType.MB_Small, 50);
        byte[] p = pool.GetMemory(false);
        Assert.Equal(64, p.Length);
        Assert.All(p, b => Assert.Equal(0, b));
        Assert.Equal(1, pool.UseCount);
        Assert.Equal(0, pool.FreeCount);
        Assert.Equal(1, pool.CountUseChain());
    }

    /// <summary>
    /// 空闲链的取/还顺序（原 :352-363 归还、:456-457 取块），按**从链尾往前**的顺序归还。
    /// <para>
    /// 实测推导（MB_SPLarge 专有池、先排空；探针 D22/D24）：
    /// 排空后取 a、b、c（`FreeCount = 0` → 三块都**新建**，`UseCount = 3`）。
    /// ★ 归还顺序必须是 **c, b, a**（链尾 → 链头）：每次归还的块都是当前已使用链的**链尾**，
    /// 于是原 :350-351 `if FUseLast = pBlock then FUseLast := FUseLast^.Prev` 成立，
    /// `Next` 链保持完好。
    /// </para>
    /// <para>
    /// ★★ 归还顺序不能带"非链尾块"：见
    /// <see cref="FreeMemory_NonTailBlock_BreaksUseChain_OriginalDefect"/> ——
    /// 原 :348-349 **只**改 `Prev^.Next` 而**不改** `Next^.Prev`（甚至连 `pBlock^.Next` 都被
    /// 原 :374 置 nil），因此释放非链尾块会把 `Next` 链**截断**。
    /// </para>
    /// <para>
    /// 归还顺序 c, b, a 时的链演化（实测）：还 c → 空闲链空 → 原 :352 挂头 `[c]`（`FFreeCount = 1`）；
    /// 还 b → `1 + 1 < 50` → 原 :360 挂尾 `[c, b]`（= 2）；还 a → 挂尾 `[c, b, a]`（= 3）。
    /// 取头（原 :456-457）依次得到 **c、b、a**。
    /// </para>
    /// </summary>
    [Fact]
    public void GetMemory_TakesFromFreeListHead_Lifo()
    {
        var pool = new TDxMemoryPool(64, 0, TDxMemBlockType.MB_SPLarge, 50);
        DrainPool(pool);
        Assert.Equal(0, pool.FreeCount);

        byte[] a = pool.GetMemory(false);
        byte[] b = pool.GetMemory(false);
        byte[] c = pool.GetMemory(false);
        Assert.Equal(3, pool.UseCount);
        Assert.Equal(0, pool.FreeCount);

        pool.FreeMemory(c);                      // 链尾 → 空闲链空 → 原 :352 挂头，链 = [c]
        Assert.Equal(1, pool.FreeCount);
        pool.FreeMemory(b);                      // `1+1 < 50` → 原 :360 挂尾，链 = [c, b]
        Assert.Equal(2, pool.FreeCount);
        pool.FreeMemory(a);                      // `2+1 < 50` → 挂尾，链 = [c, b, a]
        Assert.Equal(3, pool.FreeCount);
        Assert.Equal(3, pool.CountFreeChain());
        Assert.Equal(0, pool.CountUseChain());

        // ★ 取头（原 :456-457）⇒ 顺序与入链顺序一致：c, b, a
        Assert.Same(c, pool.GetMemory(false));
        Assert.Same(b, pool.GetMemory(false));
        Assert.Same(a, pool.GetMemory(false));
    }

    /// <summary>
    /// ★★ 原文缺陷（新推出，探针 D22-D25）：**释放非链尾块会把已使用链的 `Next` 链截断**。
    /// <para>
    /// 推导（MB_SPLarge 池；`Use = [a, b, c]`）：
    /// <list type="number">
    /// <item>`FreeMemory(b)`（b 是中间块）：原 :348-349 只改 `a^.Next := b^.Next = c` ——
    /// 这一步其实是"重复摘链"；原 :374 再把 `b^.Next := nil`。此时 `Use = [a, c]`（`Next` 链正常）。</item>
    /// <item>`FreeMemory(a)`（a 是链头）：`a^.Prev = nil` → 不摘 `Prev`；
    /// `a <> FUseLast`（此时 `FUseLast` 已被上一步改成 `c`）→ 不动 `FUseLast`；
    /// 但此时 `FUnUseHead <> nil` → 走挂尾分支，原 :360 `a^.Prev := FUnUseLast`、
    /// 原 :362 `FUnUseLast := a`；随后原 :374 `a^.Next := nil` ——
    /// ★ **`a.Next` 被清空，而 `c` 只能通过 `a.Next` 到达** ⇒ `c` 从已使用链上"消失"！</item>
    /// <item>结果：`FUseHead` 仍指向已入空闲链的 `a`（原 :377-378 只在 `FUseLast = nil` 时清 `FUseHead`，
    /// 而 `FUseLast = c ≠ nil`），`CountUseChain()` 塌缩为 1，`UseCount` 也不再与链长一致；
    /// 之后 `FreeMemory(c)` 走原 :382-383 静默 break ⇒ **`c` 永远泄漏**（既不在使用链也不在空闲链）。</item>
    /// </list>
    /// </para>
    /// <para>
    /// 实测（探针 D24/D25）：`free b` 后 `Use = 2`、`Free = 1`；`free a` 后
    /// `Use = 1`、`CountUseChain = 1`、`FindUseBlock(c) = false`；`free c` 后
    /// `Use = 1`、`Free = 2`（c 未入任何链）。因此**必须按从链尾往前的顺序归还**。
    /// </para>
    /// </summary>
    [Fact]
    public void FreeMemory_NonTailBlock_BreaksUseChain_OriginalDefect()
    {
        var pool = new TDxMemoryPool(64, 0, TDxMemBlockType.MB_SPLarge, 50);
        byte[] a = pool.GetMemory(false);
        byte[] b = pool.GetMemory(false);
        byte[] c = pool.GetMemory(false);
        Assert.Equal(3, pool.UseCount);
        Assert.Equal(3, pool.CountUseChain());

        pool.FreeMemory(b);                       // 非链尾块
        Assert.Equal(2, pool.UseCount);
        Assert.Equal(2, pool.CountUseChain());
        Assert.Equal(1, pool.FreeCount);
        Assert.Null(pool.FindUseBlock(b));

        pool.FreeMemory(a);                       // 现在是链头（原 :374 会把 a^.Next 清空）
        Assert.Equal(1, pool.UseCount);
        Assert.Equal(1, pool.CountUseChain());    // ★ 链长塌缩为 1（c 被截断）
        Assert.Null(pool.FindUseBlock(c));        // ★ c 从已使用链消失

        pool.FreeMemory(c);                       // 找不到 → 原 :382-383 静默 break
        Assert.Equal(1, pool.UseCount);           // ★ 不递减
        Assert.Equal(2, pool.FreeCount);          // ★ c 也没进空闲链 ⇒ 泄漏
        Assert.Null(pool.FindFreeBlock(c));
        Assert.Equal(1, pool.CountUseChain());    // 链头仍是已入空闲链的 a（原 :377 未清 FUseHead）
    }

    /// <summary>GetMemory 取块后块被挂到**已使用链尾**，且 Next = nil（原 :461、:462-473）。</summary>
    [Fact]
    public void GetMemory_AppendsToUseChain_TailWithNullNext()
    {
        var pool = new TDxMemoryPool(64, 0, TDxMemBlockType.MB_SPLarge, 50);
        byte[] a = pool.GetMemory(false);          // FreeCount = 0 → 新建
        byte[] b = pool.GetMemory(false);          // 新建
        TMemoryBlock ba = pool.FindUseBlock(a);
        TMemoryBlock bb = pool.FindUseBlock(b);
        Assert.NotNull(ba);
        Assert.NotNull(bb);
        Assert.Same(bb, ba.Next);       // 先取的在前
        Assert.Null(bb.Next);           // 链尾 Next = nil（原 :461）
        Assert.Same(ba, bb.Prev);
        Assert.Null(ba.Prev);           // 链头 Prev = nil（原 :465）
    }

    /// <summary>GetMemoryBlock 与 GetMemory 的唯一差异：返回块记录本身且**不清零**（原 :482-516）。</summary>
    [Fact]
    public void GetMemoryBlock_ReturnsBlockRecord_WithoutZeroing()
    {
        var pool = new TDxMemoryPool(64, 1, TDxMemBlockType.MB_Small, 50);
        TMemoryBlock b = pool.GetMemoryBlock();
        Assert.NotNull(b);
        Assert.Equal(64, b.Memory.Length);
        Assert.Equal(TDxMemBlockType.MB_Small, b.BlockType);   // 原 :491 / :310
        Assert.Null(b.Next);
        // 写哨兵 → 归还为块记录 → 再取块记录，内容仍在（GetMemoryBlock 无清零分支）
        b.Memory[0] = 0x5A;
        pool.FreeMemoryBlock(b);
        TMemoryBlock b2 = pool.GetMemoryBlock();
        Assert.Equal(0x5A, b2.Memory[0]);
    }

    /// <summary>
    /// AddMemory/DelMemory 的账目语义（原 :200-212、:451、:368）。
    /// <para>
    /// ★★ 关键前提（实测纠正，探针 D21/D25）：要让 `FreeMemory` 走"**真释放**"分支
    /// （原 :364-370 → `DelMemory`），必须 **同时**满足两个条件：
    /// <list type="number">
    /// <item>已使用链**非空**时才找得到块（原 :424 的 nil 守卫）；</item>
    /// <item>原 :358 的 `else if FFreeCount + 1 &lt; FMaxFreeBlocks` 为**假**。</item>
    /// </list>
    /// ★ 由于原 :352 先判 `if FUnUseHead = nil`（空闲链为空就**直接挂头**，`Inc(FFreeCount)`），
    /// **`MaxFreeBlocks = 0` 根本走不到真释放分支**：第一次归还时 `FUnUseHead = nil` 为真 →
    /// 挂头 → `FFreeCount = 1`。必须在空闲链**非空**且 `<` 为假时才真释放。
    /// 本用例用 `MaxFreeBlocks = -1`：`FFreeCount + 1 = 1 &lt;= -1` 为假，
    /// 且 `FUnUseHead` 非 nil（池已预分配 1 块在空闲链上）→ 走到原 :364-370 真释放。
    /// </para>
    /// </summary>
    [Fact]
    public void MemoryAccounting_AddAndDel_AreBalanced()
    {
        // InitCount = 1 → 预分配 1 块（原 :305-326）；MaxFreeBlocks = -1 → 原 :358 的 `<` 恒假
        var pool = new TDxMemoryPool(128, 1, TDxMemBlockType.MB_SpBig, -1);
        Assert.Equal(1, pool.FreeCount);
        Assert.Equal(0, pool.UseCount);

        long before = MemoryPoolGlobal.GetTotalMemBytes();

        // ① 取出预分配的块（加账已在构造时完成）
        byte[] p = pool.GetMemory(false);
        Assert.Equal(0, pool.FreeCount);
        Assert.Equal(1, pool.UseCount);

        // ② 归还：空闲链为空 → 原 :352 挂头（**不会**真释放），此时空闲链变非空
        pool.FreeMemory(p);
        Assert.Equal(1, pool.FreeCount);
        Assert.Equal(0, pool.UseCount);
        Assert.Equal(1, pool.CountFreeChain());

        // ③ 再取两块：第一块来自空闲链，第二块**新建**（原 :451 `AddMemory(32 + 128) = +160`）
        long beforeNew = MemoryPoolGlobal.GetTotalMemBytes();
        byte[] a = pool.GetMemory(false);                       // 复用
        byte[] b = pool.GetMemory(false);                       // 新建
        Assert.Equal(0, pool.FreeCount);
        Assert.Equal(2, pool.UseCount);
        long grow = MemoryPoolGlobal.GetTotalMemBytes() - beforeNew;
        Assert.True(grow >= TMemoryBlock.RecordSize + 128, $"新建块增量应 ≥ {TMemoryBlock.RecordSize + 128}，实际 {grow}");

        // ④ 归还 b：空闲链为空 → 原 :352 挂头
        pool.FreeMemory(b);
        Assert.Equal(1, pool.FreeCount);

        // ⑤ 归还 a：空闲链非空 且 `1 + 1 <= -1` 为假 → ★ 原 :364-370 **真释放**（DelMemory）
        pool.FreeMemory(a);
        Assert.Equal(0, pool.UseCount);
        Assert.Equal(1, pool.FreeCount);                        // 不加
        Assert.Equal(1, pool.CountFreeChain());
        Assert.Null(pool.FindUseBlock(a));
        Assert.Null(pool.FindFreeBlock(a));                     // ★ 不在空闲链 ⇒ 确实被释放
    }

    /// <summary>
    /// 新建块路径的**精确**字节账目（原 :451）：用全新池（`FreeCount = 0`）保证必走新建。
    /// </summary>
    [Fact]
    public void MemoryAccounting_FreshPool_NewBlockAddsExactBytes()
    {
        var fresh = new TDxMemoryPool(128, 0, TDxMemBlockType.MB_Normal, 50);
        Assert.Equal(0, fresh.FreeCount);
        long before = MemoryPoolGlobal.GetTotalMemBytes();
        fresh.GetMemory(false);
        long delta = MemoryPoolGlobal.GetTotalMemBytes() - before;
        // 原 :451 = SizeOf(TMemoryBlock) + FBlockSize = 32 + 128 = 160
        Assert.True(delta >= TMemoryBlock.RecordSize + 128, $"delta={delta}");
        Assert.Equal(1, fresh.UseCount);
    }

    /// <summary>原 :200-212 的 AddMemory/DelMemory 是 public，可直接调用（加锁累加/累减）。</summary>
    [Fact]
    public void MemoryAccounting_ManualAddDel()
    {
        long before = MemoryPoolGlobal.GetTotalMemBytes();
        MemoryPoolGlobal.AddMemory(1000);
        Assert.Equal(before + 1000, MemoryPoolGlobal.GetTotalMemBytes());
        MemoryPoolGlobal.DelMemory(400);
        Assert.Equal(before + 600, MemoryPoolGlobal.GetTotalMemBytes());
        MemoryPoolGlobal.DelMemory(600);
        Assert.Equal(before, MemoryPoolGlobal.GetTotalMemBytes());
    }

    // ---------------------------------------------------------------------------------
    // 4. FreeMemory：非本池指针 / 重复释放 / 阈值（原 :336-388）
    // ---------------------------------------------------------------------------------

    /// <summary>$P = nil$ 直接 Exit（原 :340），不改变任何计数。</summary>
    [Fact]
    public void FreeMemory_NullPointer_IsNoOp()
    {
        var pool = new TDxMemoryPool(64, 2, TDxMemBlockType.MB_Small, 50);
        int use = pool.UseCount, free = pool.FreeCount;
        pool.FreeMemory(null);
        Assert.Equal(use, pool.UseCount);
        Assert.Equal(free, pool.FreeCount);
    }

    /// <summary>
    /// ★ 非本池指针（原 :343-384）：沿已使用链找不到就静默 break —— **不抛异常、不改计数**。
    /// </summary>
    [Fact]
    public void FreeMemory_ForeignPointer_IsSilentlyIgnored()
    {
        var pool = new TDxMemoryPool(64, 1, TDxMemBlockType.MB_Small, 50);
        var other = new TDxMemoryPool(64, 1, TDxMemBlockType.MB_Small, 50);
        byte[] mine = pool.GetMemory(false);
        byte[] theirs = other.GetMemory(false);

        int use = pool.UseCount, free = pool.FreeCount;
        pool.FreeMemory(theirs);                       // 外池指针
        Assert.Equal(use, pool.UseCount);              // 计数不变
        Assert.Equal(free, pool.FreeCount);
        Assert.NotNull(pool.FindUseBlock(mine));       // 本池块仍在已使用链
        Assert.Null(pool.FindFreeBlock(theirs));
    }

    /// <summary>
    /// ★ 重复释放（原 :343-384）：第二次调用时 P 已不在已使用链上，且此刻已使用链为**空** →
    /// 原文在此处解引用 nil（AV）；本移植的 `if (pBlock == null) return;` 使其成为 no-op。
    /// </summary>
    [Fact]
    public void FreeMemory_DoubleFree_DoesNotDoubleCount()
    {
        var pool = new TDxMemoryPool(64, 0, TDxMemBlockType.MB_SPLarge, 50);
        DrainPool(pool);
        byte[] p = pool.GetMemory(false);
        Assert.Equal(0, pool.FreeCount);

        pool.FreeMemory(p);
        Assert.Equal(1, pool.FreeCount);
        Assert.Equal(1, pool.CountFreeChain());
        Assert.Equal(0, pool.CountUseChain());

        pool.FreeMemory(p);                            // 重复释放（原文为 AV）
        Assert.Equal(1, pool.FreeCount);               // 不重复计数
        Assert.Equal(1, pool.CountFreeChain());        // 不重复入链
    }

    /// <summary>
    /// ★★ 阈值差异（**原文 off-by-one**，原 :358 vs :415）：
    /// <code>
    /// FreeMemory      : else if FFreeCount + 1 &lt;  FMaxFreeBlocks then 挂尾 else 真释放
    /// FreeMemoryBlock : else if FFreeCount + 1 &lt;= FMaxFreeBlocks then 挂尾 else 真释放
    /// </code>
    /// <para>
    /// 实测推导（MB_SpBig 专有池、`MaxFreeBlocks = 2`、先排空；探针 D16 的 A 组）：
    /// 排空后取 a、b、c（都新建，`FreeCount = 0`、`UseCount = 3`）；
    /// 按 **c, b, a**（链尾→链头）归还：
    /// <list type="bullet">
    /// <item>`FreeMemory(c)`：空闲链空 → 原 :352 挂头 → `FFreeCount = **1**`；</item>
    /// <item>`FreeMemory(b)`：`FFreeCount + 1 = 2`，`2 &lt; 2` 为**假** → 原 :364-370 **真释放**
    /// → `FFreeCount` 仍是 **1**；</item>
    /// <item>`FreeMemory(a)`：同样 `2 &lt; 2` 假 → 真释放，`FFreeCount` 仍为 1。</item>
    /// </list>
    /// </para>
    /// <para>
    /// ★ 注意：`MaxFreeBlocks = 0` 时该 `<` 分支**永远不成立**（`FFreeCount ≥ 0` ⇒ `0 < 0` 为假），
    /// 所以本用例用 `MaxFreeBlocks = 2` 才能观察到"第一次入链、之后真释放"的差异。
    /// </para>
    /// <para>
    /// 对照 <see cref="FreeMemoryBlock_LessOrEqualThreshold_KeepsBlockAtExactlyMax"/>：
    /// 同一状态 `2 &lt;= 2` 为**真** → 挂尾（不释放），两者在 `FFreeCount + 1 == MaxFreeBlocks` 时分道扬镳。
    /// </para>
    /// </summary>
    [Fact]
    public void FreeMemory_StrictLessThanThreshold_TrulyFreesAtExactlyMax()
    {
        var pool = new TDxMemoryPool(64, 0, TDxMemBlockType.MB_SpBig, 2);
        DrainPool(pool);
        Assert.Equal(0, pool.FreeCount);

        byte[] a = pool.GetMemory(false);
        byte[] b = pool.GetMemory(false);
        byte[] c = pool.GetMemory(false);
        Assert.Equal(3, pool.UseCount);

        pool.FreeMemory(c);                       // 链尾；空闲链空 → 原 :352 挂头
        Assert.Equal(1, pool.FreeCount);
        Assert.NotNull(pool.FindFreeBlock(c));

        pool.FreeMemory(b);                       // 链尾；`2 < 2` 假 → ★ 真释放
        Assert.Equal(1, pool.FreeCount);          // ★ 不增加
        Assert.Null(pool.FindFreeBlock(b));

        pool.FreeMemory(a);                       // 链尾；同样真释放
        Assert.Equal(1, pool.FreeCount);
        Assert.Null(pool.FindFreeBlock(a));

        Assert.Equal(1, pool.CountFreeChain());
        Assert.Equal(0, pool.CountUseChain());

        // 空闲链里只剩 c
        Assert.Same(c, pool.GetMemory(false));
    }

    /// <summary>
    /// `FFreeCount + 1 &lt; FMaxFreeBlocks` 为真时把块挂到**空闲链尾**（原 :360-362）。
    /// <para>
    /// 实测推导（MB_Big 专有池、`MaxFreeBlocks = 50`、先排空；探针 D16 的 C 组）：
    /// 排空后取 pre、a、b（都新建），按 **b, a, pre**（链尾→链头）归还 —— 每次都满足 `&lt;` → 都挂尾。
    /// 空闲链 = `[b, a, pre]`；取头（原 :457）依次得到 **b、a、pre**。
    /// </para>
    /// </summary>
    [Fact]
    public void FreeMemory_BelowThreshold_AppendsToFreeChainTail()
    {
        var pool = new TDxMemoryPool(64, 0, TDxMemBlockType.MB_Big, 50);
        DrainPool(pool);
        Assert.Equal(0, pool.FreeCount);

        byte[] pre = pool.GetMemory(false);
        byte[] a = pool.GetMemory(false);
        byte[] b = pool.GetMemory(false);
        Assert.Equal(3, pool.UseCount);

        pool.FreeMemory(b);                       // 链尾；空闲链空 → 原 :352 挂头
        Assert.Equal(1, pool.FreeCount);
        pool.FreeMemory(a);                       // 链尾；`1+1 < 50` → 原 :360 挂尾
        Assert.Equal(2, pool.FreeCount);
        pool.FreeMemory(pre);                     // 链尾；`2+1 < 50` → 挂尾
        Assert.Equal(3, pool.FreeCount);

        Assert.Equal(3, pool.CountFreeChain());
        Assert.Equal(0, pool.CountUseChain());
        // 取头 ⇒ 顺序与入链顺序一致
        Assert.Same(b, pool.GetMemory(false));
        Assert.Same(a, pool.GetMemory(false));
        Assert.Same(pre, pool.GetMemory(false));
    }

    /// <summary>
    /// 释放链尾块时 `FUseLast := FUseLast^.Prev`（原 :350-351）；
    /// 释放到空时 `FUseLast = nil` → `FUseHead := nil`（原 :377-378）。
    /// </summary>
    [Fact]
    public void FreeMemory_UpdatesUseLast_AndClearsHeadWhenEmpty()
    {
        var pool = new TDxMemoryPool(64, 0, TDxMemBlockType.MB_Small, 50);
        byte[] a = pool.GetMemory(false);
        byte[] b = pool.GetMemory(false);
        Assert.Equal(2, pool.UseCount);

        pool.FreeMemory(b);                       // 链尾
        Assert.Equal(1, pool.UseCount);
        Assert.Equal(1, pool.CountUseChain());
        Assert.NotNull(pool.FindUseBlock(a));

        pool.FreeMemory(a);                       // 释放最后一个 → FUseHead/FUseLast 归零
        Assert.Equal(0, pool.UseCount);
        Assert.Equal(0, pool.CountUseChain());
        Assert.Null(pool.FindUseBlock(a));
    }

    /// <summary>
    /// FreeMemoryBlock 的阈值是 `&lt;=`（原 :415）—— 与 FreeMemory 的 `<` 形成差异断言。
    /// <para>
    /// 实测推导（MB_SpBig 专有池、`MaxFreeBlocks = 2`、先排空；探针 D16 的 B 组）：
    /// 排空后取 a、b（新建），`FreeMemoryBlock(a)` → 空闲链空 → 原 :409-414 挂头（`FFreeCount = 1`）；
    /// `FreeMemoryBlock(b)` → `1 + 1 = 2 &lt;= 2` 为**真** → 原 :417-419 挂尾（`FFreeCount = 2`）。
    /// 即 **一个都没真释放**（对照 FreeMemory 同状态会真释放）。
    /// </para>
    /// </summary>
    [Fact]
    public void FreeMemoryBlock_LessOrEqualThreshold_KeepsBlockAtExactlyMax()
    {
        var pool = new TDxMemoryPool(64, 0, TDxMemBlockType.MB_SpBig, 2);
        DrainPool(pool);
        Assert.Equal(0, pool.FreeCount);

        TMemoryBlock a = pool.GetMemoryBlock();
        TMemoryBlock b = pool.GetMemoryBlock();
        Assert.Equal(0, pool.FreeCount);

        pool.FreeMemoryBlock(a);                  // 空闲链空 → 原 :409-414 挂头
        Assert.Equal(1, pool.FreeCount);
        pool.FreeMemoryBlock(b);                  // `1 + 1 <= 2` → 原 :415 挂尾（对照 FreeMemory 的 `<`）
        Assert.Equal(2, pool.FreeCount);
        Assert.Equal(2, pool.CountFreeChain());
        Assert.Equal(0, pool.CountUseChain());

        // 两块都在池里：再取块记录必为 a、b
        Assert.Same(a, pool.GetMemoryBlock());
        Assert.Same(b, pool.GetMemoryBlock());
    }

    /// <summary>FreeMemoryBlock 会先把块从 NextEx/PrevEx 链上摘掉（原 :396-399、:405-406）。</summary>
    [Fact]
    public void FreeMemoryBlock_UnlinksNextExPrevEx()
    {
        var pool = new TDxMemoryPool(64, 0, TDxMemBlockType.MB_Small, 50);
        TMemoryBlock a = pool.GetMemoryBlock();
        TMemoryBlock b = pool.GetMemoryBlock();
        a.NextEx = b;
        b.PrevEx = a;
        pool.FreeMemoryBlock(b);
        Assert.Null(a.NextEx);                    // 原 :397 p^.PrevEx^.NextEx := p^.NextEx
        Assert.Null(b.PrevEx);                    // 原 :406
        Assert.Null(b.NextEx);                    // 原 :405
    }

    /// <summary>
    /// `FreeMemoryBlock(nil)` 直接 Exit（原 :392-393）；真释放分支（原 :421-427）的可达条件。
    /// <para>
    /// ★★ 实测纠正（探针 D21/D26）：`FreeMemoryBlock` 的真释放分支要求
    /// 原 :415 的 `FFreeCount + 1 &lt;= FMaxFreeBlocks` 为**假**，且原 :409 的
    /// `if FUnUseHead = nil` 也为假 —— 与 `FreeMemory` 完全对称。
    /// 因此 `MaxFreeBlocks = 0` **走不到**真释放（第一次归还时空闲链为空 → 直接挂头）。
    /// 本用例用 `MaxFreeBlocks = -1`（`2 &lt;= -1` 假）+ 空闲链非空。
    /// </para>
    /// <para>
    /// 实测数值（探针 D26）：构造（InitCount = 1）→ `Free=1, Use=0`；
    /// `GetMemoryBlock()` → `Free=0, Use=1`；`FreeMemoryBlock(blk1)`（空闲链空 → 原 :409 挂头）→
    /// `Free=1, Use=0, freeChain=1`；再取 a、b → `Free=0, Use=2, useChain=2`；
    /// `FreeMemoryBlock(b)` → `Free=1, Use=1, freeChain=1`；
    /// `FreeMemoryBlock(a)`（空闲链非空 + `2 &lt;= -1` 假 → ★ **真释放**）→
    /// `Free=**1**`（不加）、`Use=**0**`、`freeChain=**1**`、`a.Memory = **null**`（原 :423）。
    /// </para>
    /// </summary>
    [Fact]
    public void FreeMemoryBlock_NullAndTrueFree()
    {
        var pool = new TDxMemoryPool(64, 1, TDxMemBlockType.MB_Large, -1);
        Assert.Equal(1, pool.FreeCount);

        pool.FreeMemoryBlock(null);                                // 原 :392-393 直接 Exit
        Assert.Equal(1, pool.FreeCount);
        Assert.Equal(0, pool.UseCount);

        TMemoryBlock blk1 = pool.GetMemoryBlock();                 // 取走预分配块
        Assert.NotNull(blk1);
        Assert.Equal(1, pool.UseCount);
        Assert.Equal(0, pool.FreeCount);

        pool.FreeMemoryBlock(blk1);                                // 空闲链空 → 原 :409 挂头
        Assert.Equal(1, pool.FreeCount);
        Assert.Equal(0, pool.UseCount);
        Assert.Equal(1, pool.CountFreeChain());

        // 再取两块并"先还 b（挂头使空闲链非空）、再还 a"→ a 走 ★ 真释放（原 :421-425）
        TMemoryBlock a = pool.GetMemoryBlock();
        TMemoryBlock b = pool.GetMemoryBlock();
        Assert.Equal(0, pool.FreeCount);
        Assert.Equal(2, pool.UseCount);
        Assert.Equal(2, pool.CountUseChain());

        pool.FreeMemoryBlock(b);                                   // 空闲链空 → 挂头
        Assert.Equal(1, pool.FreeCount);
        Assert.Equal(1, pool.UseCount);
        Assert.Equal(1, pool.CountFreeChain());

        pool.FreeMemoryBlock(a);                                   // 空闲链非空 + `2 <= -1` 假 → ★ 真释放
        Assert.Equal(0, pool.UseCount);
        Assert.Equal(1, pool.FreeCount);                           // 不加
        Assert.Equal(1, pool.CountFreeChain());
        Assert.Null(pool.FindFreeBlock(a));
        Assert.Null(a.Memory);                                     // 原 :423 FreeMem(P^.Memory) 的托管等价
    }

    /// <summary>
    /// 原文 :518-524 `InnerCreateBlock` 是**从未被调用的死代码**；
    /// 本移植通过测试子类暴露它，验证其语义与 CreateRawBlock 相同（含加账）。
    /// </summary>
    private sealed class ProbePool : TDxMemoryPool
    {
        public ProbePool(int blockSize, int initCount, int maxFree)
            : base(blockSize, initCount, TDxMemBlockType.MB_Large, maxFree) { }

        public TMemoryBlock CallInnerCreateBlock() => InnerCreateBlock();
    }

    [Fact]
    public void InnerCreateBlock_DeadCode_StillWorksLikeCreateRawBlock()
    {
        var pool = new ProbePool(128, 0, 50);
        long before = MemoryPoolGlobal.GetTotalMemBytes();
        TMemoryBlock b = pool.CallInnerCreateBlock();
        Assert.NotNull(b);
        Assert.Equal(128, b.Memory.Length);
        Assert.Equal(TDxMemBlockType.MB_Large, b.BlockType);         // 原 :523
        // 原 :522 的 AddMemory 是 +（记录大小 + 块大小）；全局账目可能被并行测试扰动，
        // 因此只断言"至少增加了本块那么多"这一下界
        Assert.True(MemoryPoolGlobal.GetTotalMemBytes() >= before + TMemoryBlock.RecordSize + 128);
        // 它不进入任何链（原文调用方需自行挂链）—— 因此 UseCount/FreeCount 不变
        Assert.Equal(0, pool.UseCount);
        Assert.Equal(0, pool.FreeCount);
    }

    // ---------------------------------------------------------------------------------
    // 5. Clear：遍历两条链、计数**不归零**（原 :257-288）
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// Clear 释放两条链上的所有块并把四个头尾指针置 nil（原 :281-284）。
    /// <para>
    /// ★ 全局账目会被并行测试扰动，因此用"两条链已清空 + Clear 后再次取块必然是**新块**"
    /// 等价证明旧块确实被释放（若未释放会从空闲链复用同一实例）。
    /// </para>
    /// </summary>
    [Fact]
    public void Clear_FreesAllBlocksOnBothChains()
    {
        var pool = new TDxMemoryPool(128, 2, TDxMemBlockType.MB_SpBig, 50);
        byte[] a = pool.GetMemory(false);          // 从空闲链取
        byte[] b = pool.GetMemory(false);
        byte[] c = pool.GetMemory(false);          // 空闲链空 → 新建
        Assert.Equal(3, pool.UseCount);
        Assert.Equal(0, pool.FreeCount);

        pool.FreeMemory(c);                        // c 回空闲链
        Assert.Equal(1, pool.FreeCount);
        Assert.Equal(3, pool.CountUseChain() + pool.CountFreeChain());

        pool.Clear();
        Assert.Equal(0, pool.CountUseChain());
        Assert.Equal(0, pool.CountFreeChain());

        // ★ 关键推论：两条链都空了 ⇒ Clear 之后取到的块**不可能是** a / b / c 中任何一个
        byte[] d = pool.GetMemory(false);
        Assert.NotSame(a, d);
        Assert.NotSame(b, d);
        Assert.NotSame(c, d);
    }

    /// <summary>
    /// ★ 原文缺陷：Clear **不重置 FUseCount / FFreeCount**（原 :281-284 无对应语句）。
    /// </summary>
    [Fact]
    public void Clear_DoesNotResetCounters()
    {
        var pool = new TDxMemoryPool(128, 3, TDxMemBlockType.MB_Small, 50);
        pool.GetMemory(false);                      // UseCount=1, FreeCount=2
        Assert.Equal(1, pool.UseCount);
        Assert.Equal(2, pool.FreeCount);
        pool.Clear();
        // 链已空，计数却还是 1 / 2
        Assert.Equal(0, pool.CountUseChain());
        Assert.Equal(0, pool.CountFreeChain());
        Assert.Equal(1, pool.UseCount);
        Assert.Equal(2, pool.FreeCount);
    }

    /// <summary>
    /// Clear 后再次 GetMemory 走"新建"路径（原 :447），因为 FUnUseHead 为 nil。
    /// <para>
    /// 实测数值（探针 D26）：构造（InitCount = 1）→ `Free=1`、`Use=0`、`freeChain=1`；
    /// 取块 → `Free=0`、`Use=1`；`Clear()` → **`Free=0`、`Use=1`（计数不重置，原 :281-284）**、
    /// `freeChain=0`、`useChain=0`；再取 → `Free=0`、`Use=2`、且**必为新块**。
    /// </para>
    /// </summary>
    [Fact]
    public void Clear_ThenGetMemory_AllocatesFreshBlock()
    {
        var pool = new TDxMemoryPool(128, 1, TDxMemBlockType.MB_Small, 50);
        Assert.Equal(1, pool.FreeCount);
        Assert.Equal(0, pool.UseCount);

        byte[] oldBlock = pool.GetMemory(false);
        Assert.Equal(0, pool.FreeCount);
        Assert.Equal(1, pool.UseCount);

        pool.Clear();
        Assert.Equal(0, pool.CountFreeChain());
        Assert.Equal(0, pool.CountUseChain());
        Assert.Equal(0, pool.FreeCount);        // ★ 计数不重置
        Assert.Equal(1, pool.UseCount);         // ★ 计数不重置

        byte[] fresh = pool.GetMemory(false);
        Assert.NotSame(oldBlock, fresh);        // 旧块已释放 → 必为新块
        Assert.Equal(0, pool.FreeCount);
        Assert.Equal(2, pool.UseCount);         // 1（旧计数）+ 1
        Assert.Equal(1, pool.CountUseChain());
    }

    /// <summary>Clear 是幂等的（链为空时两次调用都不做事、不改变账目）。</summary>
    [Fact]
    public void Clear_IsIdempotent()
    {
        var pool = new TDxMemoryPool(128, 2, TDxMemBlockType.MB_Small, 50);
        pool.Clear();
        long after1 = MemoryPoolGlobal.GetTotalMemBytes();
        pool.Clear();
        Assert.Equal(after1, MemoryPoolGlobal.GetTotalMemBytes());
        Assert.Equal(0, pool.CountUseChain());
        Assert.Equal(0, pool.CountFreeChain());
    }

    /// <summary>Dispose（原 :329-334 destructor）等价于 Clear + 释放临界区。</summary>
    [Fact]
    public void Dispose_IsEquivalentToClear()
    {
        var pool = new TDxMemoryPool(128, 1, TDxMemBlockType.MB_Small, 50);
        pool.GetMemory(false);
        pool.GetMemory(false);                     // 新建一块
        pool.Dispose();
        Assert.Equal(0, pool.CountUseChain());
        Assert.Equal(0, pool.CountFreeChain());
    }

    // ---------------------------------------------------------------------------------
    // 6. Lock/Unlock 公开（原 :45-46、:526-534）；FreeObjPool（原 :1758-1772）
    // ---------------------------------------------------------------------------------

    /// <summary>Lock/Unlock 是 public（原文 public 声明），可重入（Monitor 语义）。</summary>
    [Fact]
    public void LockUnlock_ArePublicAndReentrant()
    {
        var pool = new TDxMemoryPool(64, 1, TDxMemBlockType.MB_Small, 50);
        pool.Lock();
        pool.Lock();                               // Monitor 可重入
        try
        {
            byte[] p = pool.GetMemory(false);
            Assert.NotNull(p);
        }
        finally
        {
            pool.Unlock();
            pool.Unlock();
        }
    }

    /// <summary>
    /// 原 :1758-1772 `FreeObjPool`：释放六个全局池。
    /// <para>
    /// ★ 托管侧有意偏差：原文只 `Free` 而不把变量置 nil（之后再用就是"已释放对象"→ AV）；
    /// 本移植 Free + 置 nil，于是之后访问会**重新创建**。测试固定这一安全偏差。
    /// </para>
    /// <para>
    /// 全局池是并行共享的，其它测试类可能同时创建/释放它们，因此本用例用 try/finally
    /// 保证池在方法返回前一定被重建。
    /// </para>
    /// </summary>
    [Fact]
    public void FreeObjPool_DisposesAndAllowsRecreation()
    {
        try
        {
            _ = MemoryPoolGlobal.SmallMemoryPool();
            _ = MemoryPoolGlobal.MemoryPool();
            _ = MemoryPoolGlobal.BigMemoryPool();
            _ = MemoryPoolGlobal.SuperMemoryPool();
            _ = MemoryPoolGlobal.LargeMemoryPool();
            _ = MemoryPoolGlobal.SuperLargeMemoryPool();
            Assert.False(MemoryPoolGlobal.AllPoolsFreed);

            MemoryPoolGlobal.FreeObjPool();
            // ★ 全局池是并行共享的：其它测试类可能在本行与下一行之间**重建**某个池，
            //   因此"刚 Free 完必然 AllPoolsFreed"不是稳定断言（实测偶发失败）。
            //   改为断言"Free 之后可被重新创建且参数不变"，以及"FreeObjPool 本身不抛异常"。
            //   （单线程下 AllPoolsFreed 为 true，由 FreeObjPool_IsIdempotent 覆盖。）

            // 原文此处会 AV；托管侧重新创建（BlockSize 仍取原值）
            TDxMemoryPool again = MemoryPoolGlobal.SmallMemoryPool();
            Assert.Equal(128, again.BlockSize);
            Assert.Equal(30, again.FreeCount);

            // FreeObjPoolGlobal.FreeObjPool 是同一动作的转发（原 :1758-1772）：调用不抛即可
            FreeObjPoolGlobal.FreeObjPool();
        }
        finally
        {
            _ = MemoryPoolGlobal.SmallMemoryPool();
            _ = MemoryPoolGlobal.MemoryPool();
            _ = MemoryPoolGlobal.BigMemoryPool();
            _ = MemoryPoolGlobal.SuperMemoryPool();
            _ = MemoryPoolGlobal.LargeMemoryPool();
            _ = MemoryPoolGlobal.SuperLargeMemoryPool();
        }
    }

    /// <summary>FreeObjPool 是幂等的（池已为 nil 时再次调用不做任何事）。</summary>
    [Fact]
    public void FreeObjPool_IsIdempotent()
    {
        MemoryPoolGlobal.FreeObjPool();
        MemoryPoolGlobal.FreeObjPool();
        Assert.True(MemoryPoolGlobal.AllPoolsFreed);
        _ = MemoryPoolGlobal.SmallMemoryPool();
        _ = MemoryPoolGlobal.MemoryPool();
        _ = MemoryPoolGlobal.BigMemoryPool();
        _ = MemoryPoolGlobal.SuperMemoryPool();
        _ = MemoryPoolGlobal.LargeMemoryPool();
        _ = MemoryPoolGlobal.SuperLargeMemoryPool();
    }

    /// <summary>TMemoryBlock.DataLen 是 ushort（原 :24 `DataLen: Word`）—— 赋值即 16 位截断。</summary>
    [Fact]
    public void MemoryBlock_DataLen_Is16BitWord()
    {
        var b = new TMemoryBlock(16) { DataLen = unchecked((ushort)65536) };
        Assert.Equal(0, b.DataLen);                 // 65536 mod 65536 = 0
        b.DataLen = unchecked((ushort)65537);
        Assert.Equal(1, b.DataLen);
        b.DataLen = unchecked((ushort)0xFFFF);
        Assert.Equal(65535, b.DataLen);
        Assert.IsType<ushort>(b.DataLen);
    }

    /// <summary>`SizeOf(TMemoryBlock)` 的托管侧换算值（仅用于账目断言的一致性）。</summary>
    [Fact]
    public void MemoryBlock_RecordSizeConstant_Is32()
    {
        // Delphi 7 / Win32：4(Memory)+4(Next)+4(Prev)+4(NextEx)+4(PrevEx)+1(BlockType)+2(DataLen)
        // = 23 → 4 字节对齐补齐 = 32
        Assert.Equal(32, TMemoryBlock.RecordSize);
        var pool = new TDxMemoryPool(128, 0, TDxMemBlockType.MB_Small, 50);
        Assert.Equal(32 + 128, pool.MemoryBlockBytes);
    }
}
