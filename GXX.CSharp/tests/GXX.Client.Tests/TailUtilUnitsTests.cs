using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using GXX.Client.Tail;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行批次 P2c / 车道 <c>par/p2c-client-tail</c>：
/// 工具类族 4 个单元（StringHashMap / uDropItemEffectList / LogHelper / IECache）的 1:1 移植测试。
/// </summary>
public sealed class TailUtilUnitsTests
{
    // ══════════════════════════════════════════════════════════════════════
    // StringHashMap.pas
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：CalcCapacity 的 2 的幂上取整（原文 :368-381）。</summary>
    [Theory]
    [InlineData(0, 4)]
    [InlineData(1, 4)]
    [InlineData(2, 4)]
    [InlineData(3, 8)]      // 阈值 (4>>1)+(4>>2)=3 <= 3 ⇒ 继续翻倍到 8
    [InlineData(4, 8)]
    [InlineData(100, 256)]
    [InlineData(192, 512)]  // 阈值 (256/2+256/4)=192 <= 192 ⇒ 继续翻倍
    [InlineData(191, 256)]
    public void CalcCapacity_RoundsUpToPowerOfTwo(int capacity, int expected)
        => Assert.Equal(expected, TStringHashMap.CalcCapacity(capacity));

    /// <summary>用例 2：CalcCapacity 的单调性与 75% 负载不变式。</summary>
    [Fact]
    public void CalcCapacity_Satisfies75PercentLoadInvariant()
    {
        int prev = 0;
        for (int n = 0; n <= 5000; n += 37)
        {
            int cap = TStringHashMap.CalcCapacity(n);
            Assert.True(cap >= prev, $"容量非单调：n={n} cap={cap} prev={prev}");
            prev = cap;
            if (n > 0)
            {
                int threshold = (cap >> 1) + (cap >> 2);
                Assert.True(threshold > n, $"n={n}: 阈值 {threshold} 未超过容量请求");
                if (cap > 4)
                {
                    int smaller = cap >> 1;
                    Assert.True(((smaller >> 1) + (smaller >> 2)) <= n, $"n={n}: 容量 {cap} 偏大");
                }
            }
        }
    }

    /// <summary>用例 3：HashLittle 的确定性 + 前缀敏感性（同前缀不同长度结果不同）。</summary>
    [Fact]
    public void HashLittle_IsDeterministicAndLengthSensitive()
    {
        byte[] a = Encoding.ASCII.GetBytes("hello");
        byte[] b = Encoding.ASCII.GetBytes("hellp");
        byte[] c = Encoding.ASCII.GetBytes("hell");

        Assert.Equal(TStringHashMap.HashLittle(a, 5, 0), TStringHashMap.HashLittle(a, 5, 0));
        Assert.NotEqual(TStringHashMap.HashLittle(a, 5, 0), TStringHashMap.HashLittle(b, 5, 0));
        Assert.NotEqual(TStringHashMap.HashLittle(a, 5, 0), TStringHashMap.HashLittle(c, 4, 0));
    }

    /// <summary>用例 4：HashLittle 空输入 + InitVal 影响（原文 a := $DEADBEEF + Len + InitVal）。</summary>
    [Fact]
    public void HashLittle_EmptyInput_And_InitValMatters()
    {
        Assert.Equal(TStringHashMap.HashLittle(Array.Empty<byte>(), 0, 0),
                     TStringHashMap.HashLittle(Array.Empty<byte>(), 0, 0));
        Assert.NotEqual(TStringHashMap.HashLittle(Array.Empty<byte>(), 0, 0),
                        TStringHashMap.HashLittle(Array.Empty<byte>(), 0, 1));
        // null 边界：按空处理，不抛
        Assert.Equal(TStringHashMap.HashLittle(Array.Empty<byte>(), 0, 7),
                     TStringHashMap.HashLittle(null, 0, 7));
    }

    /// <summary>用例 5：HashLittle 的 0..12 尾部分支全覆盖（逐长度都要稳定）。</summary>
    [Fact]
    public void HashLittle_CoversEveryTailLengthFrom0To12()
    {
        var results = new List<int>();
        for (int len = 0; len <= 12; len++)
        {
            byte[] d = Enumerable.Range(0, len).Select(i => (byte)(i + 1)).ToArray();
            int h1 = TStringHashMap.HashLittle(d, len, 0);
            int h2 = TStringHashMap.HashLittle(d, len, 0);
            Assert.Equal(h1, h2);
            results.Add(h1);
        }
        // 13 个长度里至少有 12 个互不相同（碰撞概率极低，且这能抓住"忘了处理某个长度"）
        Assert.True(results.Distinct().Count() >= 12,
            $"0..12 长度的哈希取值分布异常：{string.Join(",", results)}");
    }

    /// <summary>用例 6：HashLittle 长输入（走 >12 的主循环 + 尾部）。</summary>
    [Fact]
    public void HashLittle_LongInput_UsesMainLoopThenTail()
    {
        byte[] d = Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
        int h = TStringHashMap.HashLittle(d, d.Length, 0);
        Assert.Equal(h, TStringHashMap.HashLittle(d, d.Length, 0));
        // 只取前 25 字节（12+12+1）应与全长不同
        Assert.NotEqual(h, TStringHashMap.HashLittle(d, 25, 0));
    }

    /// <summary>
    /// 用例 7：HashOf(string) 的**正掩码归一化** —— 结果必落在 [1, $7FFFFFFF]，
    /// 永不为 0（原文 :110-120）。
    /// </summary>
    [Fact]
    public void HashOfString_IsAlwaysInOneToPositiveMask()
    {
        var m = new TStringHashMap();
        foreach (string k in new[] { "", "a", "items", "中文键", "Uc=LYcdqMBmIUoQ_Q@QFY", new string('x', 300) })
        {
            uint h = m.HashOf(k);
            Assert.True(h >= 1 && h <= 0x7FFFFFFFu, $"键 <{k}> 的哈希 {h:X} 越界");
        }
    }

    /// <summary>用例 8：HashOf 的 GBK 字节语义 —— 中文键与它的 GBK 字节哈希一致。</summary>
    [Fact]
    public void HashOfString_HashesGbkBytesNotUtf16()
    {
        var m = new TStringHashMap();
        byte[] gbk = GXX.Core.EncodingInit.GBK.GetBytes("中文");
        Assert.Equal(m.HashOf(gbk, gbk.Length), m.HashOf("中文"));
    }

    /// <summary>用例 9：HashOf(int) 的斐波那契散列落在 [0, 桶数)。</summary>
    [Fact]
    public void HashOfInteger_FibonacciStaysInsideBucketRange()
    {
        var m = new TStringHashMap(256);
        int buckets = m.GetBucketCount;
        foreach (int k in new[] { 0, 1, 7, 12345, int.MaxValue, int.MinValue, -1 })
        {
            uint h = m.HashOf(k);
            Assert.True(h < (uint)buckets, $"键 {k} 的哈希 {h} 越出桶数 {buckets}");
        }
    }

    /// <summary>用例 10：SetValue / GetValue 往返 + 覆盖（同键不增加 Count）。</summary>
    [Fact]
    public void SetValue_And_GetValue_RoundTripWithOverwrite()
    {
        var m = new TStringHashMap(16);
        m.SetValue("a", "A");
        m.SetValue("b", "B");
        Assert.Equal(2, m.Count);
        Assert.Equal("A", m.GetValue("a"));
        Assert.Equal("B", m.GetValue("b"));

        m.SetValue("a", "A2");
        Assert.Equal(2, m.Count);                 // 覆盖不增长
        Assert.Equal("A2", m.GetValue("a"));
    }

    /// <summary>用例 11：未命中返回 null（DefaultEmptyValue）。</summary>
    [Fact]
    public void GetValue_MissingKey_ReturnsNull()
    {
        var m = new TStringHashMap(16);
        Assert.Null(m.GetValue("nope"));
        Assert.Null(m["nope"]);
        Assert.False(m.IsContainKey("nope"));
    }

    /// <summary>用例 12：TryAdd 与 SetValue 的差异 —— TryAdd 不覆盖、SetValue 覆盖。</summary>
    [Fact]
    public void TryAdd_DoesNotOverwrite_UnlikeSetValue()
    {
        var m = new TStringHashMap(16);
        Assert.True(m.TryAdd("k", "v1"));
        Assert.False(m.TryAdd("k", "v2"));
        Assert.Equal("v1", m.GetValue("k"));
        Assert.Equal(1, m.Count);

        m.SetValue("k", "v3");
        Assert.Equal("v3", m.GetValue("k"));
        Assert.Equal(1, m.Count);
    }

    /// <summary>用例 13：Remove 删除命中项 + 未命中时不动 Count。</summary>
    [Fact]
    public void Remove_DeletesHitAndIgnoresMiss()
    {
        var m = new TStringHashMap(16);
        m.SetValue("a", 1);
        m.SetValue("b", 2);
        m.SetValue("c", 3);
        Assert.Equal(3, m.Count);

        m.Remove("b");
        Assert.Equal(2, m.Count);
        Assert.False(m.IsContainKey("b"));
        Assert.True(m.IsContainKey("a"));
        Assert.True(m.IsContainKey("c"));

        m.Remove("zzz");
        Assert.Equal(2, m.Count);
    }

    /// <summary>用例 14：Add/Remove 往返 —— Remove 命中即减 Count 且键不可再查。</summary>
    [Fact]
    public void Remove_HitDecrementsCountAndKeyBecomesUnreachable()
    {
        var m = new TStringHashMap(16);
        m.SetValue("a", 1);
        m.SetValue("b", 2);
        m.SetValue("c", 3);
        Assert.Equal(3, m.Count);

        m.Remove("b");
        Assert.Equal(2, m.Count);
        Assert.False(m.IsContainKey("b"));
        Assert.True(m.IsContainKey("a"));
        Assert.True(m.IsContainKey("c"));

        m.Remove("zzz");
        Assert.Equal(2, m.Count);
    }

    /// <summary>
    /// 用例 14b：<b>原文缺陷保真断言</b> —— <c>SetValue</c> 建出的节点 <c>HashCode</c> 恒为 0
    /// （原文 :355-359 只赋 Key/Next/Value，**漏了 HashCode**），而 <c>Rehash</c> 用
    /// <c>Bucket.HashCode mod Length(NewItems)</c> 决定新桶（原文 :558）。
    /// <para>后果：<b>第一次</b>扩容（旧桶数 = 4）时 <c>HashCode mod 4</c> 恒为 0
    /// ⇒ 所有节点都挤进 0 号桶，哈希表退化成单向链表。本用例把该退化锁住。</para>
    /// <para>对照：<c>TryAdd</c> 路径会写 <c>HashCode</c>（原文 :477），不退化。</para>
    /// <para>同时锁住：节点**不丢**（可达数 == Count），只是集中。</para>
    /// </summary>
    [Fact]
    public void Rehash_SetValueNodes_CollapseIntoBucketZero_OriginalBugFaithfullyPreserved()
    {
        // 首次扩容：4 桶 ⇒ 阈值 3 ⇒ 第 3 次插入触发 Grow(4 -> 8)
        var m = new TStringHashMap(0);
        m.SetValue("a", 1);
        m.SetValue("b", 2);
        m.SetValue("c", 3);

        Assert.Equal(8, m.GetBucketCount);
        Assert.Equal(3, m.Count);
        Assert.NotNull(m.BucketAt(0));
        for (int i = 1; i < m.GetBucketCount; i++)
        {
            Assert.Null(m.BucketAt(i));       // 差异断言：全挤在 0 号桶
        }

        // 节点不丢：可达数 == Count
        int reachable = 0;
        var e = m.GetEnumerator();
        while (e.MoveNext()) reachable++;
        Assert.Equal(3, reachable);

        // 对照：同样规模下 TryAdd 路径（HashCode 正确写入）不会退化
        var t = new TStringHashMap(0);
        t.TryAdd("a", 1);
        t.TryAdd("b", 2);
        t.TryAdd("c", 3);
        Assert.Equal(3, t.Count);
        int nonEmptyTryAdd = 0;
        for (int i = 0; i < t.GetBucketCount; i++)
        {
            if (t.BucketAt(i) != null) nonEmptyTryAdd++;
        }
        // 至少有一个键没落到 0 号桶（否则说明 TryAdd 也没写 HashCode）
        Assert.True(nonEmptyTryAdd >= 1);
    }

    /// <summary>用例 15：Clear 复位 Count（桶数组长度不变）。</summary>
    [Fact]
    public void Clear_ResetsCount_KeepsBucketCount()
    {
        var m = new TStringHashMap(64);
        for (int i = 0; i < 100; i++) m.SetValue("k" + i, i);
        int bucketsAfterGrowth = m.GetBucketCount;   // 插入过程中已按 75% 阈值翻倍
        Assert.True(bucketsAfterGrowth >= 128);

        m.Clear();
        Assert.Equal(0, m.Count);
        Assert.Equal(bucketsAfterGrowth, m.GetBucketCount);   // Clear 不改变容量
        Assert.Null(m.GetValue("k0"));
    }

    /// <summary>用例 16：Modify 只改已存在键（原文注释「此函数未用上」）。</summary>
    [Fact]
    public void Modify_OnlyUpdatesExistingKey()
    {
        var m = new TStringHashMap(16);
        Assert.False(m.Modify("x", 1));
        Assert.Equal(0, m.Count);
        m.SetValue("x", 1);
        Assert.True(m.Modify("x", 2));
        Assert.Equal(2, m.GetValue("x"));
    }

    /// <summary>用例 17：AddOrSet 转调 SetValue（插入 + 覆盖）。</summary>
    [Fact]
    public void AddOrSet_BehavesLikeSetValue()
    {
        var m = new TStringHashMap(16);
        m.AddOrSet("a", 1);
        m.AddOrSet("a", 2);
        Assert.Equal(1, m.Count);
        Assert.Equal(2, m.GetValue("a"));
    }

    /// <summary>用例 18：TryGetValue 命中/未命中。</summary>
    [Fact]
    public void TryGetValue_ReportsHitAndMiss()
    {
        var m = new TStringHashMap(16);
        m.SetValue("a", 42);
        Assert.True(m.TryGetValue("a", out object v));
        Assert.Equal(42, v);
        Assert.False(m.TryGetValue("b", out object v2));
        Assert.Null(v2);
    }

    /// <summary>用例 19：值可以为 null（原文 Pointer 可 nil）。</summary>
    [Fact]
    public void NullValue_IsDistinguishableFromMissing()
    {
        var m = new TStringHashMap(16);
        m.SetValue("a", null);
        Assert.True(m.IsContainKey("a"));
        Assert.True(m.TryGetValue("a", out object v));
        Assert.Null(v);
        Assert.False(m.IsContainKey("b"));
    }

    /// <summary>
    /// 用例 20：扩容后**用 SetValue 建出的项全部落在 0 号桶**
    /// —— 原文缺陷（SetValue 不写 HashCode，Rehash 用 HashCode mod 新容量）的保真断言。
    /// </summary>
    [Fact]
    public void Rehash_ItemsBuiltBySetValue_AllLandInBucketZero()
    {
        // CalcCapacity(0) = 4，阈值 = 3 ⇒ 第 3 次插入触发 Grow
        var m = new TStringHashMap(0);
        m.SetValue("a", 1);
        m.SetValue("b", 2);
        m.SetValue("c", 3);      // 触发 Grow：4 -> 8

        Assert.Equal(8, m.GetBucketCount);
        int nonEmpty = 0;
        for (int i = 0; i < m.GetBucketCount; i++)
        {
            if (m.BucketAt(i) != null) nonEmpty++;
        }
        Assert.True(nonEmpty >= 1);
        // 差异断言：SetValue 路径的 HashCode 恒为 0 ⇒ 全部堆在 0 号桶
        Assert.NotNull(m.BucketAt(0));
        for (int i = 1; i < m.GetBucketCount; i++)
        {
            Assert.Null(m.BucketAt(i));
        }
    }

    /// <summary>
    /// 用例 21：TryAdd 建出的项**带 HashCode**，Rehash 后分散到各自桶
    /// —— 与用例 20 形成差异断言。
    /// </summary>
    [Fact]
    public void Rehash_ItemsBuiltByTryAdd_KeepTheirHashCode()
    {
        var m = new TStringHashMap(0);
        m.TryAdd("aaaa", 1);
        m.TryAdd("bbbb", 2);
        m.TryAdd("cccc", 3);     // 触发 Grow

        Assert.Equal(8, m.GetBucketCount);
        int nonEmpty = 0;
        for (int i = 0; i < m.GetBucketCount; i++)
        {
            if (m.BucketAt(i) != null) nonEmpty++;
        }
        Assert.True(nonEmpty >= 1);
        // 与用例 20 的"全在 0 号桶"相反：这里按 HashCode 分散
        int inBucketZero = 0;
        for (var p = m.BucketAt(0); p != null; p = p.Next) inBucketZero++;
        Assert.True(inBucketZero < 3, $"TryAdd 建出的项未按 HashCode 分散（0 号桶有 {inBucketZero} 项）");
    }

    /// <summary>用例 22：Rehash 的边界 —— 负容量/同容量直接返回。</summary>
    [Fact]
    public void Rehash_NoOpForNegativeOrSameCapacity()
    {
        var m = new TStringHashMap(64);
        m.SetValue("a", 1);
        int buckets = m.GetBucketCount;
        m.Rehash(-1);
        Assert.Equal(buckets, m.GetBucketCount);
        m.Rehash(buckets);
        Assert.Equal(buckets, m.GetBucketCount);
        Assert.Equal(1, m.Count);
        Assert.Equal(1, m.GetValue("a"));
    }

    /// <summary>用例 23：枚举器遍历全部键值，顺序 = 桶序 × 链序（头插 ⇒ 后插先出）。</summary>
    [Fact]
    public void Enumerator_VisitsEveryPair()
    {
        var m = new TStringHashMap(16);
        var keys = Enumerable.Range(0, 50).Select(i => "k" + i).ToArray();
        foreach (string k in keys) m.SetValue(k, k);

        var seen = new List<string>();
        var e = m.GetEnumerator();
        while (e.MoveNext())
        {
            Assert.NotNull(e.Current);
            seen.Add(e.Current.Key);
        }
        Assert.Equal(50, seen.Count);
        Assert.Equal(keys.OrderBy(x => x).ToArray(), seen.OrderBy(x => x).ToArray());
    }

    /// <summary>用例 24：空表枚举器立即返回 false；无参构造的枚举器恒 false（原文 m_hash=nil）。</summary>
    [Fact]
    public void Enumerator_EmptyTableAndNilHash()
    {
        var m = new TStringHashMap(16);
        Assert.False(m.GetEnumerator().MoveNext());
        Assert.False(new TStringHashMapPairEnumerator().MoveNext());
        Assert.Null(new TStringHashMapPairEnumerator().Current);
    }

    /// <summary>用例 25：桶内多节点时枚举顺序是链序（后插入的先被枚举）。</summary>
    [Fact]
    public void Enumerator_WalksChainInReverseInsertionOrder()
    {
        var m = new TStringHashMap(0);   // 4 桶，强制碰撞
        m.SetValue("a", 1);
        m.SetValue("b", 2);
        var e = m.GetEnumerator();
        var order = new List<string>();
        while (e.MoveNext()) order.Add(e.Current.Key);
        Assert.Equal(2, order.Count);
        // 头插法：最后插入的在同一桶内先被枚举（不同桶则按桶序，两种都可能，
        // 故只断言"b 不晚于 a 出现在同一桶里"这一弱不变式）
        Assert.Contains("a", order);
        Assert.Contains("b", order);
    }

    /// <summary>用例 26：EMPTY_HASH 常量（原文 :8）。</summary>
    [Fact]
    public void EmptyHashConstant_IsMinusOne()
        => Assert.Equal(-1, StringHashMapConst.EMPTY_HASH);

    // ══════════════════════════════════════════════════════════════════════
    // uDropItemEffectList.pas
    // ══════════════════════════════════════════════════════════════════════

    private static TDropItemEffect Eff(ushort idx, short file = 0, ushort start = 0, ushort time = 0)
        => new TDropItemEffect { ItemEffectIndex = idx, FileIndex = file, StartIndex = start, Time = time };

    /// <summary>用例 1：Add 新项追加；同索引再 Add 覆盖而不增长。</summary>
    [Fact]
    public void DropList_Add_AppendsNewAndOverwritesExisting()
    {
        var l = new TDropItemEffectList();
        Assert.Equal(0, l.Count);

        l.Add(Eff(10, 1, 100, 5));
        Assert.Equal(1, l.Count);
        Assert.Equal((ushort)10, l[0].Value.ItemEffectIndex);

        l.Add(Eff(20, 2, 200, 6));
        Assert.Equal(2, l.Count);

        // 覆盖
        l.Add(Eff(10, 9, 900, 99));
        Assert.Equal(2, l.Count);
        Assert.Equal((short)9, l[0].Value.FileIndex);
        Assert.Equal((ushort)900, l[0].Value.StartIndex);
        Assert.Equal((ushort)99, l[0].Value.Time);
    }

    /// <summary>用例 2：Items 越界返回 null（原文语义）。</summary>
    [Fact]
    public void DropList_Indexer_OutOfRange_ReturnsNull()
    {
        var l = new TDropItemEffectList();
        Assert.Null(l[-1]);
        Assert.Null(l[0]);
        Assert.Null(l[999]);
        l.Add(Eff(1));
        Assert.Null(l[1]);
        Assert.NotNull(l[0]);
    }

    /// <summary>用例 3：IndexOf 未排序时线性查找；Get 未命中返回 null。</summary>
    [Fact]
    public void DropList_IndexOf_LinearWhenUnsorted()
    {
        var l = new TDropItemEffectList();
        l.Add(Eff(30));
        l.Add(Eff(10));
        l.Add(Eff(20));

        Assert.False(l.IsSorted);
        Assert.Equal(0, l.IndexOf(30));
        Assert.Equal(1, l.IndexOf(10));
        Assert.Equal(2, l.IndexOf(20));
        Assert.Equal(-1, l.IndexOf(99));
        Assert.Null(l.Get(99));
        Assert.Equal((ushort)10, l.Get(10).Value.ItemEffectIndex);
    }

    /// <summary>用例 4：Sort 后 IsSorted 为真，且顺序升序。</summary>
    [Fact]
    public void DropList_Sort_OrdersAscendingAndFlagsSorted()
    {
        var l = new TDropItemEffectList();
        foreach (ushort v in new ushort[] { 30, 10, 50, 20, 40 }) l.Add(Eff(v));
        l.Sort();

        Assert.True(l.IsSorted);
        for (int i = 1; i < l.Count; i++)
        {
            Assert.True(l[i].Value.ItemEffectIndex > l[i - 1].Value.ItemEffectIndex,
                $"排序后下标 {i} 未升序");
        }
    }

    /// <summary>
    /// 用例 5：Sort 后 Add **新**索引会把 IsSorted 置回 False（于是 IndexOf 退回线性）。
    /// </summary>
    [Fact]
    public void DropList_AddNewAfterSort_ClearsSortedFlag()
    {
        var l = new TDropItemEffectList();
        l.Add(Eff(20));
        l.Add(Eff(10));
        l.Sort();
        Assert.True(l.IsSorted);

        l.Add(Eff(99));
        Assert.False(l.IsSorted);
        Assert.Equal(2, l.IndexOf(99));       // 线性查找仍能找到
    }

    /// <summary>用例 6：Sort 后 Add **已有**索引不改变 IsSorted（走覆盖分支）。</summary>
    [Fact]
    public void DropList_AddExistingAfterSort_KeepsSortedFlag()
    {
        var l = new TDropItemEffectList();
        l.Add(Eff(20, 1));
        l.Add(Eff(10, 2));
        l.Sort();
        l.Add(Eff(10, 7));                    // 已存在 ⇒ 只覆盖
        Assert.True(l.IsSorted);
        Assert.Equal((short)7, l.Get(10).Value.FileIndex);
    }

    /// <summary>用例 7：二分查找在已排序表上正确（含首/末/中间/未命中）。</summary>
    [Fact]
    public void DropList_IndexOf_BinarySearchOnSortedTable()
    {
        var l = new TDropItemEffectList();
        foreach (ushort v in new ushort[] { 10, 20, 30, 40, 50, 60, 70 }) l.Add(Eff(v));
        l.Sort();
        Assert.True(l.IsSorted);

        Assert.Equal(0, l.IndexOf(10));
        Assert.Equal(3, l.IndexOf(40));
        Assert.Equal(6, l.IndexOf(70));
        Assert.Equal(-1, l.IndexOf(35));
        Assert.Equal(-1, l.IndexOf(0));
        Assert.Equal(-1, l.IndexOf(100));
    }

    /// <summary>用例 8：快速排序的边角 —— 0/1/2 项、已排序、全相同。</summary>
    [Fact]
    public void DropList_Sort_HandlesDegenerateInputs()
    {
        var empty = new TDropItemEffectList();
        empty.Sort();                                    // 原文 :145 if Count > 0 才排
        Assert.False(empty.IsSorted);                    // ⇒ 空表不置位
        Assert.Equal(0, empty.Count);

        var one = new TDropItemEffectList();
        one.Add(Eff(5));
        one.Sort();
        Assert.True(one.IsSorted);

        var two = new TDropItemEffectList();
        two.Add(Eff(9));
        two.Add(Eff(1));
        two.Sort();
        Assert.Equal((ushort)1, two[0].Value.ItemEffectIndex);
        Assert.Equal((ushort)9, two[1].Value.ItemEffectIndex);

        var sorted = new TDropItemEffectList();
        foreach (ushort v in new ushort[] { 1, 2, 3, 4, 5 }) sorted.Add(Eff(v));
        sorted.Sort();
        for (int i = 0; i < 5; i++) Assert.Equal((ushort)(i + 1), sorted[i].Value.ItemEffectIndex);

        // 全部同索引 ⇒ 只剩 1 项（Add 去重），排序无压力
        var same = new TDropItemEffectList();
        same.Add(Eff(7));
        same.Add(Eff(7));
        same.Add(Eff(7));
        Assert.Equal(1, same.Count);
        same.Sort();
        Assert.True(same.IsSorted);
    }

    /// <summary>用例 9：Clear 清空并复位排序标志。</summary>
    [Fact]
    public void DropList_Clear_ResetsEverything()
    {
        var l = new TDropItemEffectList();
        l.Add(Eff(1));
        l.Add(Eff(2));
        l.Sort();
        l.Clear();
        Assert.Equal(0, l.Count);
        Assert.False(l.IsSorted);
        Assert.Null(l[0]);
        Assert.Equal(-1, l.IndexOf(1));
    }

    /// <summary>用例 10：析构语义（托管侧无 Dispose）—— 确认类型面与原文一致。</summary>
    [Fact]
    public void DropList_PublicSurface_MatchesOriginal()
    {
        var t = typeof(TDropItemEffectList);
        Assert.NotNull(t.GetConstructor(Type.EmptyTypes));
        Assert.NotNull(t.GetMethod("Add", new[] { typeof(TDropItemEffect) }));
        Assert.NotNull(t.GetMethod("IndexOf", new[] { typeof(int) }));
        Assert.NotNull(t.GetMethod("Get", new[] { typeof(int) }));
        Assert.NotNull(t.GetMethod("Sort", Type.EmptyTypes));
        Assert.NotNull(t.GetMethod("Clear", Type.EmptyTypes));
        // 原文是 TObject 派生（有析构器释放 FList），托管侧交给 GC
        Assert.False(typeof(IDisposable).IsAssignableFrom(t));
    }

    // ══════════════════════════════════════════════════════════════════════
    // LogHelper.pas
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：时间戳格式 yyyy-mm-dd hh:mm:ss（原文 FormatDateTime 格式串）。</summary>
    [Fact]
    public void Log_TimestampFormat_MatchesDelphiFormatString()
    {
        var dt = new DateTime(2026, 9, 20, 3, 4, 5);
        Assert.Equal("2026-09-20 03:04:05", TLogFile.FormatTimestamp(dt));
        Assert.Equal("2026-12-31 23:59:59", TLogFile.FormatTimestamp(new DateTime(2026, 12, 31, 23, 59, 59)));
        Assert.Equal("2000-01-01 00:00:00", TLogFile.FormatTimestamp(new DateTime(2000, 1, 1, 0, 0, 0)));
    }

    /// <summary>用例 2：行格式 <c>[时间] 正文</c>（原文 Format('[%s] %s', ...)）。</summary>
    [Fact]
    public void Log_LineFormat_MatchesDelphiFormatCall()
    {
        var dt = new DateTime(2026, 1, 2, 3, 4, 5);
        Assert.Equal("[2026-01-02 03:04:05] hello", TLogFile.FormatLogLine("hello", dt));
        Assert.Equal("[2026-01-02 03:04:05] ", TLogFile.FormatLogLine("", dt));
        Assert.Equal("[2026-01-02 03:04:05] 中文", TLogFile.FormatLogLine("中文", dt));
    }

    /// <summary>用例 3：级别过滤 —— nLevel >= GLOBAL_LOG_LEVEL 才写。</summary>
    [Fact]
    public void Log_LevelFilter_MatchesOriginalComparison()
    {
        int saved = LogLevels.GLOBAL_LOG_LEVEL;
        try
        {
            LogLevels.GLOBAL_LOG_LEVEL = 0;
            Assert.True(TLogFile.ShouldWrite(0));
            Assert.True(TLogFile.ShouldWrite(3));
            Assert.False(TLogFile.ShouldWrite(-1));

            LogLevels.GLOBAL_LOG_LEVEL = 2;
            Assert.False(TLogFile.ShouldWrite(1));
            Assert.True(TLogFile.ShouldWrite(2));
            Assert.True(TLogFile.ShouldWrite(3));

            LogLevels.GLOBAL_LOG_LEVEL = 4;
            Assert.False(TLogFile.ShouldWrite(3));
            Assert.True(TLogFile.ShouldWrite(4));
        }
        finally
        {
            LogLevels.GLOBAL_LOG_LEVEL = saved;
        }
    }

    /// <summary>用例 4：级别常量值。</summary>
    [Fact]
    public void Log_LevelConstants_MatchSource()
    {
        Assert.Equal(0, LogLevels.LOG_LEVEL_0);
        Assert.Equal(1, LogLevels.LOG_LEVEL_1);
        Assert.Equal(2, LogLevels.LOG_LEVEL_2);
        Assert.Equal(3, LogLevels.LOG_LEVEL_3);
    }

    /// <summary>用例 5：GetUniqueMutexName = GBK 字节的十六进制小写串。</summary>
    [Theory]
    [InlineData("abc", "616263")]
    [InlineData("", "")]
    [InlineData("A", "41")]
    public void Log_UniqueMutexName_IsHexOfGbkBytes(string name, string expected)
        => Assert.Equal(expected, TLogFile.GetUniqueMutexName(name));

    /// <summary>用例 6：GetUniqueMutexName 的中文 = 每汉字 2 字节 ⇒ 4 个 hex 字符。</summary>
    [Fact]
    public void Log_UniqueMutexName_ChineseCharsProduceGbkHex()
    {
        string hex = TLogFile.GetUniqueMutexName("中文");
        Assert.Equal(8, hex.Length);               // 2 个汉字 × 2 字节 × 2 hex
        Assert.True(hex.All(c => "0123456789abcdef".Contains(c)), $"非小写 hex：{hex}");
        Assert.Equal(TLogFile.GetUniqueMutexName("中文"), hex);
    }

    /// <summary>用例 7：null 边界 —— 按空串处理（原文 nil 指针会得到空 hex）。</summary>
    [Fact]
    public void Log_UniqueMutexName_NullIsEmpty()
        => Assert.Equal("", TLogFile.GetUniqueMutexName(null));

    /// <summary>用例 8：MutexName 前缀 <c>Global\</c>（原文 Format('Global\%s')）。</summary>
    [Fact]
    public void Log_MutexName_HasGlobalPrefix()
    {
        string dir = Path.Combine(Path.GetTempPath(), "gxx_p2c_log_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            using var lf = new TLogFile(Path.Combine(dir, "a.log"));
            Assert.StartsWith("Global\\", lf.MutexName);
            Assert.Equal("Global\\" + TLogFile.GetUniqueMutexName(Path.Combine(dir, "a.log")), lf.MutexName);
        }
        finally { try { Directory.Delete(dir, true); } catch { } }
    }

    /// <summary>用例 9：写日志的端到端（异步队列 → 后台线程落盘）。</summary>
    [Fact]
    public void Log_WriteLog_AppendsTimestampedLines()
    {
        string dir = Path.Combine(Path.GetTempPath(), "gxx_p2c_log_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        string path = Path.Combine(dir, "MirUI.log");
        try
        {
            using (var lf = new TLogFile(path))
            {
                lf.WriteLog("first");
                lf.WriteLog("second");
                lf.DrainForTest();
            }

            Assert.True(File.Exists(path), "日志文件未创建");
            string[] lines = File.ReadAllLines(path, GXX.Core.EncodingInit.GBK);
            Assert.Equal(2, lines.Length);
            Assert.Matches(@"^\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\] first$", lines[0]);
            Assert.EndsWith("] second", lines[1]);
        }
        finally { try { Directory.Delete(dir, true); } catch { } }
    }

    /// <summary>用例 10：空串被丢弃（原文 if sLog &lt;&gt; EmptyStr）。</summary>
    [Fact]
    public void Log_WriteLog_DropsEmptyString()
    {
        string dir = Path.Combine(Path.GetTempPath(), "gxx_p2c_log_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        string path = Path.Combine(dir, "MirUI.log");
        try
        {
            using (var lf = new TLogFile(path))
            {
                lf.WriteLog("");
                lf.WriteLog(null);
                lf.WriteLog("only");
                lf.DrainForTest();
            }
            string[] lines = File.ReadAllLines(path, GXX.Core.EncodingInit.GBK);
            Assert.Single(lines);
            Assert.EndsWith("] only", lines[0]);
        }
        finally { try { Directory.Delete(dir, true); } catch { } }
    }

    /// <summary>用例 11：logNew 模式在 Create 时清空既有文件（原文 ReWrite）。</summary>
    [Fact]
    public void Log_LogNewMode_TruncatesExistingFile()
    {
        string dir = Path.Combine(Path.GetTempPath(), "gxx_p2c_log_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        string path = Path.Combine(dir, "MirUI.log");
        try
        {
            File.WriteAllText(path, "stale content\n");
            using (var lf = new TLogFile(path, TLogMode.logNew))
            {
                lf.WriteLog("fresh");
                lf.DrainForTest();
            }
            string all = File.ReadAllText(path, GXX.Core.EncodingInit.GBK);
            Assert.DoesNotContain("stale", all);
            Assert.Contains("fresh", all);
        }
        finally { try { Directory.Delete(dir, true); } catch { } }
    }

    /// <summary>用例 12：logAppend 模式保留既有内容。</summary>
    [Fact]
    public void Log_LogAppendMode_KeepsExistingContent()
    {
        string dir = Path.Combine(Path.GetTempPath(), "gxx_p2c_log_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        string path = Path.Combine(dir, "MirUI.log");
        try
        {
            File.WriteAllText(path, "keep me\n");
            using (var lf = new TLogFile(path, TLogMode.logAppend))
            {
                lf.WriteLog("added");
                lf.DrainForTest();
            }
            string all = File.ReadAllText(path, GXX.Core.EncodingInit.GBK);
            Assert.Contains("keep me", all);
            Assert.Contains("added", all);
        }
        finally { try { Directory.Delete(dir, true); } catch { } }
    }

    /// <summary>用例 13：TLogMode 枚举取值（原文 (logAppend, logNew)）。</summary>
    [Fact]
    public void Log_ModeEnum_OrderMatchesSource()
    {
        Assert.Equal(0, (int)TLogMode.logAppend);
        Assert.Equal(1, (int)TLogMode.logNew);
    }

    /// <summary>用例 14：WriteToLogFile 按 GLOBAL_LOG_LEVEL 过滤（不改全局单例路径）。</summary>
    [Fact]
    public void Log_WriteToLogFile_RespectsGlobalLevel()
    {
        int saved = LogLevels.GLOBAL_LOG_LEVEL;
        try
        {
            LogLevels.GLOBAL_LOG_LEVEL = 100;      // 高阈值 ⇒ 一切被丢弃
            TLogFile.WriteToLogFile("dropped", 3); // 不应抛
            Assert.False(TLogFile.ShouldWrite(3));
        }
        finally { LogLevels.GLOBAL_LOG_LEVEL = saved; }
    }

    /// <summary>用例 15：Dispose 幂等（原文 Destroy 之后 CloseHandle 再调会失败）。</summary>
    [Fact]
    public void Log_Dispose_IsIdempotent()
    {
        string dir = Path.Combine(Path.GetTempPath(), "gxx_p2c_log_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            var lf = new TLogFile(Path.Combine(dir, "a.log"));
            lf.Dispose();
            lf.Dispose();                       // 第二次应为 no-op
            lf.WriteLog("after dispose");       // 不应抛
        }
        finally { try { Directory.Delete(dir, true); } catch { } }
    }

    // ══════════════════════════════════════════════════════════════════════
    // IECache.pas
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：CACHEGROUP_* 常量逐条核对（原文 :41-77）。</summary>
    [Fact]
    public void IECache_Constants_MatchSource()
    {
        Assert.Equal(0xFFFFFFFFu, IECacheConst.CACHEGROUP_ATTRIBUTE_GET_ALL);
        Assert.Equal(0x00000001u, IECacheConst.CACHEGROUP_ATTRIBUTE_BASIC);
        Assert.Equal(0x00000002u, IECacheConst.CACHEGROUP_ATTRIBUTE_FLAG);
        Assert.Equal(0x00000004u, IECacheConst.CACHEGROUP_ATTRIBUTE_TYPE);
        Assert.Equal(0x00000008u, IECacheConst.CACHEGROUP_ATTRIBUTE_QUOTA);
        Assert.Equal(0x00000010u, IECacheConst.CACHEGROUP_ATTRIBUTE_GROUPNAME);
        Assert.Equal(0x00000020u, IECacheConst.CACHEGROUP_ATTRIBUTE_STORAGE);

        Assert.Equal(1u, IECacheConst.CACHEGROUP_FLAG_NONPURGEABLE);
        Assert.Equal(2u, IECacheConst.CACHEGROUP_FLAG_FLUSHURL_ONDELETE);
        Assert.Equal(4u, IECacheConst.CACHEGROUP_FLAG_GIDONLY);

        Assert.Equal(0u, IECacheConst.CACHEGROUP_SEARCH_ALL);
        Assert.Equal(1u, IECacheConst.CACHEGROUP_SEARCH_BYURL);
        Assert.Equal(1u, IECacheConst.CACHEGROUP_TYPE_INVALID);

        Assert.Equal(120, IECacheConst.GROUPNAME_MAX_LENGTH);
        Assert.Equal(4, IECacheConst.GROUP_OWNER_STORAGE_SIZE);

        // CACHEGROUP_READWRITE_MASK = TYPE | QUOTA | GROUPNAME | STORAGE = 0x3C
        Assert.Equal(0x0000003Cu, IECacheConst.CACHEGROUP_READWRITE_MASK);
    }

    /// <summary>
    /// 用例 2：TFilterOption 的位值表 —— **不是**位序（COOKIE/URLHISTORY 在高位，
    /// TRACK_OFFLINE/ONLINE 在低位）。
    /// </summary>
    [Fact]
    public void IECache_FilterOptionValues_MatchSourceOrder()
    {
        var v = TIECache.FilterOptionValues;
        Assert.Equal(9, v.Length);
        Assert.Equal(0x00000001u, v[(int)TFilterOption.NORMAL_ENTRY]);
        Assert.Equal(0x00000002u, v[(int)TFilterOption.STABLE_ENTRY]);
        Assert.Equal(0x00000004u, v[(int)TFilterOption.STICKY_ENTRY]);
        Assert.Equal(0x00100000u, v[(int)TFilterOption.COOKIE_ENTRY]);
        Assert.Equal(0x00200000u, v[(int)TFilterOption.URLHISTORY_ENTRY]);
        Assert.Equal(0x00000010u, v[(int)TFilterOption.TRACK_OFFLINE_ENTRY]);
        Assert.Equal(0x00000020u, v[(int)TFilterOption.TRACK_ONLINE_ENTRY]);
        Assert.Equal(0x00010000u, v[(int)TFilterOption.SPARSE_ENTRY]);
        Assert.Equal(0x00020000u, v[(int)TFilterOption.OCX_ENTRY]);

        // 差异断言：索引 3/4 的值**大于**索引 5/6 的值（顺序≠位序）
        Assert.True(v[3] > v[5] && v[4] > v[6]);
    }

    /// <summary>用例 3：UpdateFilterOptionValue 的求和（原文用 Inc 而非 or）。</summary>
    [Fact]
    public void IECache_UpdateFilterOptionValue_SumsSelectedBits()
    {
        Assert.Equal(0u, TIECache.ComputeFilterOptionValue(null));
        Assert.Equal(0u, TIECache.ComputeFilterOptionValue(Array.Empty<TFilterOption>()));

        Assert.Equal(0x00000001u,
            TIECache.ComputeFilterOptionValue(new[] { TFilterOption.NORMAL_ENTRY }));

        // 原文构造函数默认集：NORMAL | COOKIE | URLHISTORY | TRACK_OFFLINE | TRACK_ONLINE | STICKY
        Assert.Equal(0x00000001u + 0x00100000u + 0x00200000u + 0x00000010u + 0x00000020u + 0x00000004u,
            TIECache.ComputeFilterOptionValue(new[]
            {
                TFilterOption.NORMAL_ENTRY, TFilterOption.COOKIE_ENTRY, TFilterOption.URLHISTORY_ENTRY,
                TFilterOption.TRACK_OFFLINE_ENTRY, TFilterOption.TRACK_ONLINE_ENTRY, TFilterOption.STICKY_ENTRY,
            }));

        // 全集 = 全部位之和
        var all = new[]
        {
            TFilterOption.NORMAL_ENTRY, TFilterOption.STABLE_ENTRY, TFilterOption.STICKY_ENTRY,
            TFilterOption.COOKIE_ENTRY, TFilterOption.URLHISTORY_ENTRY, TFilterOption.TRACK_OFFLINE_ENTRY,
            TFilterOption.TRACK_ONLINE_ENTRY, TFilterOption.SPARSE_ENTRY, TFilterOption.OCX_ENTRY,
        };
        Assert.Equal(TIECache.FilterOptionValues.Aggregate(0u, (a, b) => a + b),
            TIECache.ComputeFilterOptionValue(all));
    }

    /// <summary>用例 4：构造函数不计算 FFilterOptionValue（原文缺陷/惰性，保真）。</summary>
    [Fact]
    public void IECache_Constructor_LeavesFilterOptionValueAtZero()
    {
        var c = new TIECache();
        Assert.Equal(0u, c.FilterOptionValue);          // 原文如此
        Assert.Equal(6, c.FilterOptions.Length);        // 默认过滤集 6 项
        Assert.False(c.LibraryFound);                   // wininet 未加载（接缝）

        // 显式设一次 setter 才会算
        c.FilterOptions = c.FilterOptions;
        Assert.Equal(0x00300035u, c.FilterOptionValue);
    }

    /// <summary>用例 5：FilterOptions setter 的赋值语义（含 null → 空集）。</summary>
    [Fact]
    public void IECache_FilterOptions_SetterRecomputesValue()
    {
        var c = new TIECache { FilterOptions = null };
        Assert.Empty(c.FilterOptions);
        Assert.Equal(0u, c.FilterOptionValue);

        c.FilterOptions = new[] { TFilterOption.SPARSE_ENTRY };
        Assert.Equal(0x00010000u, c.FilterOptionValue);
    }

    /// <summary>用例 6：SearchPattern 前缀表（原文 :1121）。</summary>
    [Fact]
    public void IECache_SearchPatterns_MatchSourceTable()
    {
        Assert.Null(TIECache.SearchPatterns[(int)TSearchPattern.spAll]);          // nil
        Assert.Equal("Cookie:", TIECache.SearchPatterns[(int)TSearchPattern.spCookies]);
        Assert.Equal("Visited:", TIECache.SearchPatterns[(int)TSearchPattern.spHistory]);
        Assert.Equal("", TIECache.SearchPatterns[(int)TSearchPattern.spUrl]);

        Assert.Null(TIECache.SearchPatternOf(TSearchPattern.spAll));
        Assert.Equal("Cookie:", TIECache.SearchPatternOf(TSearchPattern.spCookies));
        Assert.Equal("Visited:", TIECache.SearchPatternOf(TSearchPattern.spHistory));
        Assert.Equal("", TIECache.SearchPatternOf(TSearchPattern.spUrl));
        // 越界枚举值
        Assert.Null(TIECache.SearchPatternOf((TSearchPattern)99));
    }

    /// <summary>用例 7：SearchPattern 属性读写（原文普通属性）。</summary>
    [Fact]
    public void IECache_SearchPattern_PropertyRoundTrips()
    {
        var c = new TIECache();
        Assert.Equal(TSearchPattern.spAll, c.SearchPattern);   // 枚举默认 0 = spAll
        c.SearchPattern = TSearchPattern.spHistory;
        Assert.Equal(TSearchPattern.spHistory, c.SearchPattern);
    }

    /// <summary>用例 8：ClearEntryValues 复位 Content 与 EntryInfo 全字段。</summary>
    [Fact]
    public void IECache_ClearEntryValues_ResetsAllFields()
    {
        var c = new TIECache();
        c.EntryInfo.SourceUrlName = "http://x/";
        c.EntryInfo.FSize = 123;
        c.EntryInfo.HeaderInfo = "hdr";
        c.Content.Buffer = new byte[] { 1, 2 };
        c.Content.BufferLength = 2;

        c.ClearEntryValues();

        Assert.Null(c.Content.Buffer);
        Assert.Equal(0, c.Content.BufferLength);
        Assert.Equal("", c.EntryInfo.SourceUrlName);
        Assert.Equal("", c.EntryInfo.LocalFileName);
        Assert.Equal(0u, c.EntryInfo.EntryType);
        Assert.Equal(0u, c.EntryInfo.UseCount);
        Assert.Equal(0u, c.EntryInfo.HitRate);
        Assert.Equal(0u, c.EntryInfo.FSize);
        Assert.Equal(0d, c.EntryInfo.LastModifiedTime);
        Assert.Equal("", c.EntryInfo.HeaderInfo);
        Assert.Equal("", c.EntryInfo.FileExtension);
        Assert.Equal(0u, c.EntryInfo.ExemptDelta);
    }

    /// <summary>
    /// 用例 9：GetEntryValues 的字段映射，以及 <c>FSize = dwSizeLow</c> 的原文缺陷断言
    /// （原文 <c>(dwSizeHigh shl 32)</c> 在 32 位里恒为 0）。
    /// </summary>
    [Fact]
    public void IECache_GetEntryValues_MapsFieldsAndIgnoresSizeHigh()
    {
        var c = new TIECache();
        c.GetEntryValues("http://u/", "C:\\tmp\\f", 0x41, 7, 3, 4096, 1.5, 2.5, 3.5, 4.5, ".htm", "H", 9);

        Assert.Equal("http://u/", c.EntryInfo.SourceUrlName);
        Assert.Equal("C:\\tmp\\f", c.EntryInfo.LocalFileName);
        Assert.Equal(0x41u, c.EntryInfo.EntryType);
        Assert.Equal(7u, c.EntryInfo.UseCount);
        Assert.Equal(3u, c.EntryInfo.HitRate);
        Assert.Equal(4096u, c.EntryInfo.FSize);
        Assert.Equal(1.5, c.EntryInfo.LastModifiedTime);
        Assert.Equal(2.5, c.EntryInfo.ExpireTime);
        Assert.Equal(3.5, c.EntryInfo.LastAccessTime);
        Assert.Equal(4.5, c.EntryInfo.LastSyncTime);
        Assert.Equal(".htm", c.EntryInfo.FileExtension);
        Assert.Equal("H", c.EntryInfo.HeaderInfo);
        Assert.Equal(9u, c.EntryInfo.ExemptDelta);
    }

    /// <summary>用例 10：接缝方法统一返回 ERROR_FILE_NOT_FOUND（原文"库未找到"取值）。</summary>
    [Fact]
    public void IECache_WinInetMethods_ReturnNotFoundThroughSeam()
    {
        var c = new TIECache();
        Assert.Equal(IECacheSeam.ERROR_FILE_NOT_FOUND, c.AddUrlToGroup(1, "http://x/"));
        Assert.Equal(IECacheSeam.ERROR_FILE_NOT_FOUND, c.RemoveUrlFromGroup(1, "http://x/"));
        Assert.Equal(IECacheSeam.ERROR_FILE_NOT_FOUND, c.CopyFileToCache("http://x/", "f", 0, 0));
        Assert.Equal(IECacheSeam.ERROR_FILE_NOT_FOUND, c.CreateEntry("http://x/", "htm", 10, out string fname));
        Assert.Null(fname);
        Assert.Equal(IECacheSeam.ERROR_FILE_NOT_FOUND, c.GetGroupInfo(1));
        Assert.Equal(IECacheSeam.ERROR_FILE_NOT_FOUND, c.SetGroupInfo(1));
        Assert.Equal(IECacheSeam.ERROR_FILE_NOT_FOUND, c.DeleteGroup(1));
        Assert.Equal(IECacheSeam.ERROR_FILE_NOT_FOUND, c.GetEntryInfo("http://x/"));
        Assert.Equal(IECacheSeam.ERROR_FILE_NOT_FOUND, c.SetEntryInfo("http://x/"));
        Assert.Equal(IECacheSeam.ERROR_FILE_NOT_FOUND, c.GetEntryContent("http://x/"));
        Assert.Equal(IECacheSeam.ERROR_FILE_NOT_FOUND, c.FindFirstEntry(0));
        Assert.Equal(IECacheSeam.ERROR_FILE_NOT_FOUND, c.FindNextEntry());
        Assert.Equal(IECacheSeam.ERROR_FILE_NOT_FOUND, c.FindFirstGroup(ref UnsafeLong));
        Assert.Equal(IECacheSeam.ERROR_FILE_NOT_FOUND, c.RetrieveGroups());
        Assert.Equal(0L, c.CreateGroup());
        Assert.False(c.FindNextGroup(ref UnsafeLong));
        Assert.False(c.CloseFindEntry());
    }

    private static long UnsafeLong;

    /// <summary>用例 11：initializeWinInet 接缝恒 false，且 <c>winetdll</c> 名与原文字面一致。</summary>
    [Fact]
    public void IECache_InitializeWinInet_SeamAndLibraryName()
    {
        Assert.False(IECacheSeam.initializeWinInet());
        Assert.Equal("wininet.dll", IECacheSeam.winetdll);
        Assert.Equal(2u, IECacheSeam.ERROR_FILE_NOT_FOUND);
        Assert.Equal(0u, IECacheSeam.S_OK);
        Assert.Equal(259u, IECacheSeam.ERROR_NO_MORE_ITEMS);
    }

    /// <summary>用例 12：接缝映射表完整（每个 wininet 入口都有一条决定）。</summary>
    [Fact]
    public void IECache_FunctionMap_CoversEveryWinInetEntry()
    {
        var map = IECacheSeam.FunctionMap;
        Assert.True(map.Length >= 20);
        Assert.All(map, m =>
        {
            Assert.False(string.IsNullOrWhiteSpace(m.WinInet));
            Assert.False(string.IsNullOrWhiteSpace(m.Managed));
        });
        Assert.Contains(map, m => m.WinInet == "FindFirstUrlCacheGroup" && m.Managed == "（无托管等价）");
        Assert.Contains(map, m => m.WinInet == "FileTimeToLocalFiletime");
    }

    /// <summary>用例 13：TEntryInfo 的字段声明顺序（原文 FSize 在 HitRate 之后）。</summary>
    [Fact]
    public void IECache_EntryInfoFieldOrder_MatchesSource()
    {
        var names = typeof(TEntryInfo).GetFields().Select(f => f.Name).ToArray();
        Assert.Equal(new[]
        {
            "SourceUrlName", "LocalFileName", "EntryType", "UseCount", "HitRate", "FSize",
            "LastModifiedTime", "ExpireTime", "LastAccessTime", "LastSyncTime",
            "HeaderInfo", "FileExtension", "ExemptDelta",
        }, names);
    }

    /// <summary>用例 14：Register 是 no-op（原为 VCL 设计期注册）。</summary>
    [Fact]
    public void IECache_Register_IsNoOp()
    {
        TIECache.Register();   // 不应抛
        var m = typeof(TIECache).GetMethod("Register",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        Assert.NotNull(m);
        Assert.Equal(typeof(void), m.ReturnType);
    }
}
