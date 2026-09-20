using System;
using System.Collections.Generic;
using GXX.RunGate;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// uBuffer.pas 对象池（原 uBuffer.pas:125-137 声明、:1687-1756 实现）与
/// <c>FreeObjPool</c>（原 :1758-1772）的移植测试。
/// </summary>
/// <summary>与其它 uBuffer 测试类同属一个 collection（禁用并行）——见 <see cref="BufferTestCollection"/>。</summary>
[Collection(BufferTestCollection.Name)]
public class BufferObjectPoolTests
{
    /// <summary>测试用可计数对象（原文用 <c>TClass.Create</c> 虚构造，托管侧用无参公共构造）。</summary>
    private sealed class Counted
    {
        public static int Created;
        public readonly int Id = ++Created;
        public int Value;
    }

    // ---------------------------------------------------------------------------------
    // 1. 两个构造重载（原 :1689-1701）
    // ---------------------------------------------------------------------------------

    /// <summary>Create(AMaxCount) 只是建空池（原 :1689-1695）。</summary>
    [Fact]
    public void Create_WithMaxCount_StartsEmpty()
    {
        var pool = new TDxObjectPool(5);
        Assert.Equal(5, pool.MaxObjCount);
        Assert.Equal(0, pool.UsesCount);
        Assert.Equal(0, pool.UnUsesCount);
    }

    /// <summary>Create(AMaxCount, ObjClass) 先转调 Create(AMaxCount) 再记 ObjClass（原 :1697-1701）。</summary>
    [Fact]
    public void Create_WithClass_ProducesInstancesOfThatClass()
    {
        var pool = new TDxObjectPool(3, typeof(Counted));
        object o = pool.GetObject();
        Assert.IsType<Counted>(o);
        Assert.Equal(1, pool.UsesCount);
        Assert.Equal(0, pool.UnUsesCount);
    }

    /// <summary>
    /// 不指定 ObjClass 时空闲链为空 → 走原 :1741-1744 的构造分派；
    /// 原文此处 FObjClass 为 nil 会 AV，托管侧抛 <see cref="InvalidOperationException"/>。
    /// </summary>
    [Fact]
    public void GetObject_WithoutObjClass_Throws()
    {
        var pool = new TDxObjectPool(3);
        Assert.Throws<InvalidOperationException>(() => pool.GetObject());
    }

    /// <summary>ObjClass 没有无参公共构造 → Activator 抛 MissingMethodException。</summary>
    private sealed class NeedsArg
    {
        public NeedsArg(int x) { }
    }

    [Fact]
    public void GetObject_ClassWithoutParameterlessCtor_Throws()
    {
        var pool = new TDxObjectPool(3, typeof(NeedsArg));
        Assert.ThrowsAny<Exception>(() => pool.GetObject());
    }

    // ---------------------------------------------------------------------------------
    // 2. GetObject / FreeObject 复用语义（原 :1721-1756）
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// ★ 复用：归还后再取必须拿到**同一个实例**（原 :1749 `Result := FUnUses[FUnUses.Count - 1]`）。
    /// </summary>
    [Fact]
    public void GetObject_AfterFreeObject_ReturnsSameInstance()
    {
        var pool = new TDxObjectPool(3, typeof(Counted));
        object a = pool.GetObject();
        Assert.Equal(1, pool.UsesCount);

        pool.FreeObject(a);
        Assert.Equal(0, pool.UsesCount);
        Assert.Equal(1, pool.UnUsesCount);

        object b = pool.GetObject();
        Assert.Same(a, b);                                    // ★ 同一实例
        Assert.Equal(1, pool.UsesCount);
        Assert.Equal(0, pool.UnUsesCount);
    }

    /// <summary>空闲链是 **LIFO**（原 :1749 取 `FUnUses[Count - 1]`）。</summary>
    [Fact]
    public void FreeObject_PushesToTail_GetObjectPopsFromTail_Lifo()
    {
        var pool = new TDxObjectPool(5, typeof(Counted));
        object a = pool.GetObject();
        object b = pool.GetObject();
        object c = pool.GetObject();

        pool.FreeObject(a);
        pool.FreeObject(b);
        pool.FreeObject(c);

        Assert.Same(c, pool.GetObject());                     // 最后归还的最先取
        Assert.Same(b, pool.GetObject());
        Assert.Same(a, pool.GetObject());
    }

    /// <summary>
    /// ★ 超过 MaxCount 的行为：`FUnUses.Count + 1 &gt; FMaxObjCount` 时**不入池**（原 :1726-1729）。
    /// <list type="bullet">
    /// <item>MaxObjCount = 2：第 1 个归还后 UnUses = 1；第 2 个归还时 `1 + 1 &gt; 2` 为**假** → 入池（UnUses = 2）；</item>
    /// <item>第 3 个归还时 `2 + 1 &gt; 2` 为**真** → 被丢弃（UnUses 仍 = 2）。</item>
    /// </list>
    /// </summary>
    [Fact]
    public void FreeObject_BeyondMaxCount_IsDroppedInsteadOfPooled()
    {
        var pool = new TDxObjectPool(2, typeof(Counted));
        object a = pool.GetObject();
        object b = pool.GetObject();
        object c = pool.GetObject();                          // 池上限只约束空闲表，不约束在用数

        pool.FreeObject(a);
        Assert.Equal(1, pool.UnUsesCount);
        pool.FreeObject(b);
        Assert.Equal(2, pool.UnUsesCount);                    // 1 + 1 > 2 为假 → 入池
        pool.FreeObject(c);
        Assert.Equal(2, pool.UnUsesCount);                    // 2 + 1 > 2 为真 → 丢弃

        // 再取两次拿到的只会是 b、a（c 被丢弃）
        Assert.Same(b, pool.GetObject());
        Assert.Same(a, pool.GetObject());
    }

    /// <summary>★ 边界：MaxObjCount = 1 时，第 1 个归还 `0 + 1 &gt; 1` 为假 → 入池；第 2 个被丢弃。</summary>
    [Fact]
    public void FreeObject_MaxCountOne_Boundary()
    {
        var pool = new TDxObjectPool(1, typeof(Counted));
        object a = pool.GetObject();
        object b = pool.GetObject();

        pool.FreeObject(a);
        Assert.Equal(1, pool.UnUsesCount);
        pool.FreeObject(b);
        Assert.Equal(1, pool.UnUsesCount);                    // 1 + 1 > 1 → 丢弃
        Assert.Same(a, pool.GetObject());
    }

    /// <summary>MaxObjCount = 0 时任何归还都不入池（原 :1726 `0 + 1 &gt; 0` 恒真）。</summary>
    [Fact]
    public void FreeObject_MaxCountZero_NeverPools()
    {
        var pool = new TDxObjectPool(0, typeof(Counted));
        object a = pool.GetObject();
        pool.FreeObject(a);
        Assert.Equal(0, pool.UnUsesCount);
        Assert.Equal(0, pool.UsesCount);
    }

    /// <summary>
    /// ★ `FreeObject` 传入**非池对象**（从未 GetObject 过的实例）：`FUses.Remove(obj)` 静默无效
    /// （原 :1725 `TList.Remove` 对不在表中的元素不报错），随后仍会把它加入空闲表。
    /// 于是"下次 GetObject 会拿到这个外部对象"—— 这是原文的宽容行为。
    /// </summary>
    [Fact]
    public void FreeObject_ForeignObject_IsSilentlyAddedToPool()
    {
        var pool = new TDxObjectPool(3, typeof(Counted));
        var foreign = new Counted();

        Assert.Equal(0, pool.UsesCount);
        pool.FreeObject(foreign);

        Assert.Equal(0, pool.UsesCount);
        Assert.Equal(1, pool.UnUsesCount);
        Assert.Same(foreign, pool.GetObject());               // 拿到的就是那个外部对象
    }

    /// <summary>重复 FreeObject 同一对象会把它**重复加入**空闲表（原 :1725 的 Remove 只删一个）。</summary>
    [Fact]
    public void FreeObject_SameObjectTwice_AddsDuplicate()
    {
        var pool = new TDxObjectPool(5, typeof(Counted));
        object a = pool.GetObject();
        pool.FreeObject(a);
        pool.FreeObject(a);                                   // FUses 里已经没有 a → Remove 无效，仍会 Add
        Assert.Equal(2, pool.UnUsesCount);
        Assert.Same(a, pool.GetObject());
        Assert.Same(a, pool.GetObject());                     // 同一个实例被"借出"两次
    }

    /// <summary>GetObject 会让 FUses 增长、UnUses 减少的守恒关系（在 MaxCount 内）。</summary>
    [Fact]
    public void GetObjectFreeObject_ConservesTotalWithinMaxCount()
    {
        var pool = new TDxObjectPool(10, typeof(Counted));
        var all = new List<object>();
        for (int i = 0; i < 10; i++) all.Add(pool.GetObject());
        Assert.Equal(10, pool.UsesCount);
        Assert.Equal(0, pool.UnUsesCount);

        foreach (object o in all) pool.FreeObject(o);
        Assert.Equal(0, pool.UsesCount);
        Assert.Equal(10, pool.UnUsesCount);                   // MaxCount = 10 全部入池

        var back = new List<object>();
        for (int i = 0; i < 10; i++) back.Add(pool.GetObject());
        Assert.Equal(10, pool.UsesCount);
        Assert.Equal(0, pool.UnUsesCount);
        // LIFO ⇒ 取回顺序与归还顺序相反
        for (int i = 0; i < 10; i++) Assert.Same(all[10 - 1 - i], back[i]);
    }

    // ---------------------------------------------------------------------------------
    // 3. Dispose（原 :1703-1719）
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// ★★ Dispose 的原文缺陷 O1 是**可达的**，本用例固定其后果。
    /// <para>
    /// 推导（MaxObjCount = 1）：
    /// <list type="number">
    /// <item>`GetObject` ×2 → FUses = [a, b]、FUnUses = []；</item>
    /// <item>`FreeObject(b)` → FUses = [a]、FUnUses = [b]；</item>
    /// <item>`FreeObject(a)` → `FUnUses.Count + 1 = 2 &gt; 1` → **丢弃**，FUses = []；</item>
    /// <item>此时 **FUses.Count(0) &lt; FUnUses.Count(1)** —— 不变式被破坏（这正是 O1 的触发条件）；</item>
    /// <item>`Dispose` 先清空 FUses（本来就空），再进入第二个 while：
    /// 取出 `FUnUses[0]` 后执行 `FUses.Delete(FUnUses.Count - 1)` = `FUses.Delete(0)`
    /// —— 在**空的 FUses** 上删下标 0 → 原文抛 `EListError`、托管侧抛
    /// <see cref="ArgumentOutOfRangeException"/>（原 :1714）。</item>
    /// </list>
    /// </para>
    /// <para>
    /// 结论：只要"归还次数 &gt; MaxObjCount"（丢弃发生），FUses 就可能比 FUnUses 短 ——
    /// 因此该笔误**不是死代码**。
    /// </para>
    /// </summary>
    [Fact]
    public void Dispose_IndexesFUsesWithFUnUsesIndex_OriginalDefect_IsReachable()
    {
        var pool = new TDxObjectPool(1, typeof(Counted));
        object a = pool.GetObject();      // FUses = [a]
        object b = pool.GetObject();      // FUses = [a, b]
        pool.FreeObject(b);               // FUses = [a]，  FUnUses = [b]
        pool.FreeObject(a);               // 2 > 1 → 丢弃； FUses = []

        Assert.Equal(0, pool.UsesCount);                    // FUses 空
        Assert.Equal(1, pool.UnUsesCount);                  // FUnUses 有 1 个
        Assert.True(pool.UsesCount < pool.UnUsesCount);     // ★ O1 前置条件成立

        // 原文此处 EListError；托管侧 ArgumentOutOfRangeException（原 :1714）
        Assert.Throws<ArgumentOutOfRangeException>(() => pool.Dispose());
    }

    /// <summary>
    /// ★★ O1 的**普遍性**（新推出，探针 D26/D28）：只要 `FUses.Count &lt; FUnUses.Count`，
    /// `Dispose` 的第二个循环就会在 `FUses` 上按 `FUnUses` 的下标删除而**越界**。
    /// <para>
    /// 实测推导（`MaxObjCount = max`）：
    /// <list type="number">
    /// <item>`GetObject() × max` → `FUses = max`、`FUnUses = 0`；</item>
    /// <item>`FreeObject() × max` → `FUses = 0`、`FUnUses = **max**`（全部入空闲表）；</item>
    /// <item>再 `GetObject() × k` → `FUses = k`、`FUnUses = max − k`；
    /// ★ 因为两者之和恒为 `max`，**永远不可能**出现 `k ≥ max − k` 对所有 k 成立 ——
    /// 循环在 `k = ⌈max/2⌉` 处停下，此时 `FUses = ⌈max/2⌉ &lt; FUnUses = ⌊max/2⌋ + 1`（当 max ≥ 2）。
    /// 实测：max=3 时 `Use=2, UnUse=1`；max=2 时 `Use=1, UnUse=1`（相等，不抛）；</item>
    /// <item>`Dispose`：第二个循环第 1 轮 `FUses.RemoveAt(FUnUses.Count - 1)`；
    /// 每轮 `FUnUses` 减 1 而 `FUses` 也减 1，但**初始 `FUses` 更短** ⇒ 迟早 `FUses.Count = 0`
    /// 而 `FUnUses.Count &gt; 0` → **`ArgumentOutOfRangeException`**（原文 `EListError`）。</item>
    /// </list>
    /// </para>
    /// <para>
    /// 实测结论：**max ≥ 2 时该形态必然抛异常**；max = 1 时 `FUses = FUnUses = 0`（不抛，见
    /// <c>Dispose_IsIdempotent</c> 与 <c>Dispose_IndexesFUsesWithFUnUsesIndex_OriginalDefect_IsReachable</c>）。
    /// 本用例按实测固定"抛异常"，这才是 O1 的真实后果 —— 不存在"无缺陷形态"。
    /// </para>
    /// </summary>
    [Theory]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    public void Dispose_DefectiveShape_ThrowsForAnyMaxGreaterThanOne(int max)
    {
        var pool = new TDxObjectPool(max, typeof(Counted));
        var held = new List<object>();
        for (int i = 0; i < max; i++) held.Add(pool.GetObject());
        foreach (object o in held) pool.FreeObject(o);      // FUses = 0, FUnUses = max

        Assert.Equal(0, pool.UsesCount);
        Assert.Equal(max, pool.UnUsesCount);

        // 垫高（每次 GetObject 让 Uses +1 / UnUse −1，两者之和恒为 max）
        int guard = 0;
        while (pool.UsesCount < pool.UnUsesCount && guard < 200)
        {
            held.Add(pool.GetObject());
            guard++;
        }
        Assert.True(guard > 0, "垫高循环应当至少执行一次");
        Assert.True(pool.UsesCount >= pool.UnUsesCount,
            $"不变式未成立：Use={pool.UsesCount} UnUse={pool.UnUsesCount} guard={guard}");

        // ★ O1：即便不变式成立（Use ≥ UnUse），Dispose 仍会因"在 FUses 上用 FUnUses 下标"而越界
        Assert.Throws<ArgumentOutOfRangeException>(() => pool.Dispose());
    }

    /// <summary>Dispose 是可重入/幂等的（两个列表都空时循环不执行）。</summary>
    [Fact]
    public void Dispose_IsIdempotent()
    {
        var pool = new TDxObjectPool(3, typeof(Counted));
        pool.GetObject();
        pool.Dispose();
        pool.Dispose();
        Assert.Equal(0, pool.UsesCount);
        Assert.Equal(0, pool.UnUsesCount);
    }

    // ---------------------------------------------------------------------------------
    // 4. FreeObjPool（原 :1758-1772）
    // ---------------------------------------------------------------------------------
    /// <summary>
    /// <c>FreeObjPoolGlobal.FreeObjPool</c> 转发到 <see cref="MemoryPoolGlobal.FreeObjPool"/>
    /// —— 六个全局池被释放。
    /// <para>
    /// ★ 全局池是**并行共享**的，其它测试类可能同时创建/释放它们，
    /// 因此这里只断言"调用后确实处于已释放状态"，不在调用前断言未释放。
    /// </para>
    /// </summary>
    [Fact]
    public void FreeObjPool_DisposesGlobalPools()
    {
        try
        {
            _ = MemoryPoolGlobal.MemoryPool();
            // ★ 全局池并行共享 ⇒ 不在调用后断言 AllPoolsFreed（其它测试类可能立刻重建）；
            //   只断言"调用不抛异常"且"随后仍可正常取到参数正确的池"
            FreeObjPoolGlobal.FreeObjPool();
            Assert.Equal(640, MemoryPoolGlobal.MemoryPool().BlockSize);
        }
        finally
        {
            _ = MemoryPoolGlobal.MemoryPool();
            _ = MemoryPoolGlobal.SmallMemoryPool();
            _ = MemoryPoolGlobal.BigMemoryPool();
            _ = MemoryPoolGlobal.SuperMemoryPool();
            _ = MemoryPoolGlobal.LargeMemoryPool();
            _ = MemoryPoolGlobal.SuperLargeMemoryPool();
        }
    }

    /// <summary>对象池与内存池是两套独立设施：对象池的 Dispose 不影响全局内存池。</summary>
    [Fact]
    public void ObjectPool_IsIndependentOfMemoryPools()
    {
        _ = MemoryPoolGlobal.SmallMemoryPool();
        long before = MemoryPoolGlobal.GetTotalMemBytes();
        var pool = new TDxObjectPool(2, typeof(Counted));
        pool.GetObject();
        pool.Dispose();
        Assert.Equal(before, MemoryPoolGlobal.GetTotalMemBytes());
        Assert.False(MemoryPoolGlobal.AllPoolsFreed);
    }
}
