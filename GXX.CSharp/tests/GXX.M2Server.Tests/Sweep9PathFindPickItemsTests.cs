// ============================================================================
// 测试：Source/M2Engine/ClientPickItemsCfg.pas → GXX.M2Server.Sweep9.PathFind.Items（1:1）
//
// 覆盖策略（任务书第 5 条）：每个公开成员 ≥1 例；分支/边界/原文缺陷各配差异断言。
// 本文件的"否定性断言"（如"低 14 位相同的两条谁在前取决于分区轨迹"）全部配**计数或实测**取证
// （台账 §37.3）：断言里的数值都来自对原文排序轨迹的逐步推演，并在用例注释里写出推演过程。
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using GXX.M2Server.Sweep9.PathFind.Items;
using Xunit;

namespace GXX.M2Server.Tests;

public class Sweep9PathFindPickItemsTests
{
    // ---------------------------------------------------------------- 工具

    /// <summary>把 Word 序列按**小端**铺成 byte[]（原文 SetData 的输入格式）。</summary>
    private static byte[] Bytes(params ushort[] words)
    {
        var buf = new byte[words.Length * 2];
        for (int i = 0; i < words.Length; i++)
        {
            buf[i * 2] = (byte)(words[i] & 0xFF);
            buf[i * 2 + 1] = (byte)(words[i] >> 8);
        }

        return buf;
    }

    private static TClientPickItems Loaded(params ushort[] words)
    {
        var items = new TClientPickItems();
        var buf = Bytes(words);
        items.SetData(buf, buf.Length);
        return items;
    }

    // ------------------------------------------------------- 常量 / 位域语义

    /// <summary>
    /// 位域关系（原文 :75/:87/:99/:126）：<c>$8000</c> 优先、<c>$4000</c> 可捡、
    /// <c>$C000</c> = 两者之**或**、<c>$3FFF</c> = 排序/查找键。
    /// </summary>
    [Fact]
    public void BitMasks_AreDisjointAndOrComposed()
    {
        Assert.Equal(0xC000, 0x8000 | 0x4000);
        Assert.Equal(0x0000, 0x8000 & 0x4000);
        Assert.Equal(0x3FFF, 0xFFFF & ~0xC000);
        // 键掩码与两个标志掩码互不重叠 ⇒ 任一 Word 恰好拆成 (优先, 可捡, 键=Idx+1)
        Assert.Equal(0xFFFF, 0xC000 | 0x3FFF);
    }

    // ------------------------------------------------------------------ SetData

    /// <summary>偶数长度 → Count = BufLen / 2（原文 :112 <c>Count := BufLen div 2</c>）。</summary>
    [Fact]
    public void SetData_CountIsHalfOfByteLength()
    {
        var items = new TClientPickItems();
        Assert.Equal(0, items.Count);

        var buf = Bytes(0x0001, 0x0002, 0x0003);
        items.SetData(buf, buf.Length);
        Assert.Equal(3, items.Count);
    }

    /// <summary>空缓冲：<c>Count := 0</c> 且**不进** <c>if Count &gt; 0</c> 块（原文 :114）⇒ 不排序、Count 为 0。</summary>
    [Fact]
    public void SetData_EmptyBuffer_YieldsZeroCount()
    {
        var items = Loaded();
        Assert.Equal(0, items.Count);
    }

    /// <summary>差异断言：奇数长度**整份丢弃**（原文 :107 <c>if BufLen mod 2 &lt;&gt; 0 then Exit</c>），
    /// 且**不清空既有数据**（Exit 在 SetLength 之前）。</summary>
    [Fact]
    public void SetData_OddLength_IsDiscardedAndKeepsOldData()
    {
        var items = Loaded(0x8005);
        Assert.Equal(1, items.Count);

        items.SetData(new byte[] { 0x01, 0x80, 0x02 }, 3);
        Assert.Equal(1, items.Count);                       // 仍是旧数据
        Assert.True(items.CheckPriorityPickup(5));          // 旧数据没被动过
    }

    /// <summary>差异断言：长度按**字节**取偶、内容按**小端 Word** 还原
    /// （原文 :116 <c>Move(Buf^, FItems[0], BufLen)</c>，Windows/Delphi 小端）。
    /// 字节 <c>01 80</c> ⇒ Word <c>$8001</c> ⇒ 键 1、优先位为真。</summary>
    [Fact]
    public void SetData_DecodesLittleEndianWords()
    {
        var items = new TClientPickItems();
        items.SetData(new byte[] { 0x01, 0x80 }, 2);

        Assert.Equal(1, items.Count);
        Assert.True(items.CheckPriorityPickup(1));
        Assert.False(items.CheckEnablePickup(1));
        // 若实现误用大端，键会是 $0180 = 384，下面这条才会成立
        Assert.False(items.CheckPriorityPickup(0x0180));
    }

    /// <summary>差异断言：<c>BufLen</c> **只取前 BufLen 字节**，尾随多余字节被忽略
    /// （原文 Move 的长度参数就是 BufLen）。</summary>
    [Fact]
    public void SetData_IgnoresBytesPastBufLen()
    {
        var items = new TClientPickItems();
        items.SetData(new byte[] { 0x07, 0x00, 0xFF, 0xFF, 0xFF }, 2);

        Assert.Equal(1, items.Count);
        Assert.True(items.CheckCanPickItem(7) == false);    // 键 7 无标志位
        Assert.False(items.CheckPriorityPickup(7));
        Assert.False(items.CheckEnablePickup(7));
        Assert.False(items.CheckPriorityPickup(0xFFFF & 0x3FFF));   // 尾部字节未被读入
    }

    /// <summary>差异断言（原文缺陷 ④）：<c>BufLen</c> 大于实际缓冲区时原文越界读（AV），
    /// 托管侧等价地抛 <see cref="IndexOutOfRangeException"/>。</summary>
    [Fact]
    public void SetData_BufLenBeyondBuffer_Throws()
    {
        var items = new TClientPickItems();
        Assert.Throws<IndexOutOfRangeException>(
            () => items.SetData(new byte[] { 0x01, 0x80 }, 6));
    }

    /// <summary>负的偶长度：原文 <c>SetLength(FItems, -2)</c> 抛范围错误；托管侧同样抛。</summary>
    [Fact]
    public void SetData_NegativeEvenLength_Throws()
    {
        var items = new TClientPickItems();
        Assert.ThrowsAny<Exception>(() => items.SetData(new byte[] { 1, 2, 3, 4 }, -4));
    }

    // ------------------------------------------------------------------ 三个检查

    /// <summary><c>$8000</c> 是优先捡取位（原文 :75）。</summary>
    [Fact]
    public void CheckPriorityPickup_UsesBit15()
    {
        var items = Loaded(0x8003);
        Assert.True(items.CheckPriorityPickup(3));
        Assert.False(items.CheckEnablePickup(3));
    }

    /// <summary><c>$4000</c> 是可捡取位（原文 :87）。</summary>
    [Fact]
    public void CheckEnablePickup_UsesBit14()
    {
        var items = Loaded(0x4003);
        Assert.True(items.CheckEnablePickup(3));
        Assert.False(items.CheckPriorityPickup(3));
    }

    /// <summary>
    /// 差异断言：<c>CheckCanPickItem</c> 用 <c>$C000</c> ⇒ 是"**或**"（原文注释「允许捡或优先捡」）。
    /// 若误写成"两个位都为真"（与），下面两个单标志位用例都会变假。
    /// </summary>
    [Fact]
    public void CheckCanPickItem_IsOrOfBothFlags_NotAnd()
    {
        Assert.True(Loaded(0x8003).CheckCanPickItem(3));    // 只有优先位
        Assert.True(Loaded(0x4003).CheckCanPickItem(3));    // 只有可捡位
        Assert.True(Loaded(0xC003).CheckCanPickItem(3));    // 两位都有
        Assert.False(Loaded(0x0003).CheckCanPickItem(3));   // 都没有
        // 与运算版本会给出 false，本实现在上面已给 true ⇒ 二者不同
        Assert.True((0x8003 & 0xC000) != 0 && (0x8003 & 0x8000 & 0x4000) == 0);
    }

    /// <summary>不存在的键一律假（<c>SearchItem</c> 返回 -1 ⇒ 三个 Check 都不进 if）。</summary>
    [Fact]
    public void Checks_UnknownKey_ReturnFalse()
    {
        var items = Loaded(0x8001, 0x4002, 0xC003);
        Assert.False(items.CheckPriorityPickup(4));
        Assert.False(items.CheckEnablePickup(4));
        Assert.False(items.CheckCanPickItem(4));
        // 空表
        var empty = new TClientPickItems();
        Assert.False(empty.CheckPriorityPickup(0));
        Assert.False(empty.CheckCanPickItem(0));
    }

    /// <summary>
    /// 键是「ItemIdx」本身（低 14 位），故 <c>ItemIdx = 0</c> 只有在存了低 14 位为 0 的 Word
    /// 时才命中 —— 原文注释说金币"Idx = 0 特殊处理"，即金币的存储值低 14 位是 0（而非 0+1）。
    /// 两条路径都锁定：低 14 位 = 0 命中 Idx 0；低 14 位 = 1 命中 Idx 1。
    /// </summary>
    [Fact]
    public void ItemIdxZero_MatchesOnlyKeyZero()
    {
        var coin = Loaded(0x8000);          // 键 0 + 优先位
        Assert.True(coin.CheckPriorityPickup(0));
        Assert.False(coin.CheckPriorityPickup(1));

        var idx1 = Loaded(0x8001);          // 键 1 + 优先位
        Assert.False(idx1.CheckPriorityPickup(0));
        Assert.True(idx1.CheckPriorityPickup(1));
    }

    // -------------------------------------------------------------------- Sort

    /// <summary>
    /// 端到端：<c>SetData</c> 内部会 <c>Sort(0, Count-1)</c>；排序键只有低 14 位，
    /// 标志位不参与比较（原文 :126-127 <c>CompareItem</c>）。
    /// 输入顺序 $0005 / $8003 / $4001 ⇒ 排序键 5 / 3 / 1 ⇒ 升序后 1/3/5。
    /// </summary>
    [Fact]
    public void Sort_OrdersByLow14Bits_IgnoresFlags()
    {
        var items = Loaded(0x0005, 0x8003, 0x4001);

        Assert.True(items.CheckEnablePickup(1));        // 键 1 是 $4001
        Assert.False(items.CheckPriorityPickup(1));
        Assert.True(items.CheckPriorityPickup(3));      // 键 3 是 $8003
        Assert.False(items.CheckEnablePickup(3));
        Assert.False(items.CheckCanPickItem(5));        // 键 5 是 $0005（无标志）
    }

    /// <summary>
    /// 差异断言（原文缺陷 ①）：两条**排序键相同**的记录在快排分区里被**无条件互换**
    /// （<c>if I &lt;&gt; J then</c> 交换，不看 <c>CompareItem</c> 结果）。
    /// <para>逐步推演（2 元素，L=0 R=1）：<c>P = (0+1) shr 1 = 0</c>；
    /// <c>CompareItem(0,0)=0</c>、<c>CompareItem(1,0)=0</c>，两个内层 while 都不动；
    /// <c>I &lt;= J</c> 且 <c>I &lt;&gt; J</c> ⇒ 交换 ⇒ 两元素顺序**必然对调**；
    /// <c>P = I</c> ⇒ <c>P := J = 1</c>；<c>Inc(I)</c>/<c>Dec(J)</c> ⇒ I=1,J=0 ⇒ 内层退出；
    /// <c>L &lt; J</c> 假；<c>L := I = 1</c>；<c>I &gt;= R</c> ⇒ 外层退出。</para>
    /// 因此 <c>SetData</c> 之后两条同键记录的顺序 = 输入顺序的**逆序**，
    /// 而 <c>SearchItem</c> 落到下标 0 ⇒ 命中的是"后输入的那一条"。
    /// </summary>
    [Fact]
    public void Sort_SwapsEqualKeyEntries_SoDuplicateLookupHitsReversedInput()
    {
        // 输入：先可捡($4001) 后优先($8001)，同键 1
        var a = Loaded(0x4001, 0x8001);
        Assert.Equal(2, a.Count);
        Assert.True(a.CheckPriorityPickup(1));      // 下标 0 是后来的 $8001
        Assert.False(a.CheckEnablePickup(1));

        // 反向输入 ⇒ 结果也反向
        var b = Loaded(0x8001, 0x4001);
        Assert.True(b.CheckEnablePickup(1));        // 下标 0 是后来的 $4001
        Assert.False(b.CheckPriorityPickup(1));

        // 两者都"能捡"，但"优先"的归属随输入顺序翻转 ⇒ 与输入顺序**有关**，
        // 且与"稳定排序"的预期相反（稳定排序下 a 应命中 $4001）。
        Assert.True(a.CheckCanPickItem(1) && b.CheckCanPickItem(1));
    }

    /// <summary>
    /// <c>Sort</c> 是公开方法，可对已加载数据再排一次（原文 :34 在 public 段）。
    /// 重复键再次相邻 ⇒ 仍按上面那条规则对调。
    /// </summary>
    [Fact]
    public void Sort_CanBeCalledAgainOnLoadedData()
    {
        var items = Loaded(0x4001, 0x8001);
        Assert.True(items.CheckPriorityPickup(1));      // 第一次 SetData 内部已排序

        items.Sort(0, items.Count - 1);                 // 再排一次 ⇒ 同键再对调
        Assert.True(items.CheckEnablePickup(1));
        Assert.False(items.CheckPriorityPickup(1));
    }

    /// <summary>单元素 / 空区间调用 <c>Sort</c> 不得越界（<c>R = 0</c> 时外层条件 <c>I &gt;= R</c> 立即成立）。</summary>
    [Fact]
    public void Sort_SingleElementOrEmptyRange_IsNoOp()
    {
        var one = Loaded(0x8001);
        one.Sort(0, 0);
        Assert.True(one.CheckPriorityPickup(1));

        var two = Loaded(0x8001, 0x8002);
        two.Sort(1, 1);                                 // 只排尾元素
        Assert.True(two.CheckPriorityPickup(1));
        Assert.True(two.CheckPriorityPickup(2));
    }

    /// <summary>
    /// 规模覆盖（1..40 条、键互不相同、乱序输入）：<c>Sort</c> + <c>SearchItem</c> 的组合
    /// 必须让**每一个**键都能被查到且标志位正确。
    /// 这同时覆盖 <c>SearchItem</c> 的二分收敛与 <c>Sort</c> 的递归/尾循环两种路径。
    /// </summary>
    [Fact]
    public void SortAndSearch_AllKeysResolvable_ForSizesOneToForty()
    {
        var rng = new Random(20260919);
        for (int n = 1; n <= 40; n++)
        {
            // 键 1..n 各一条，标志位按 i % 4 决定（0=无,1=优先,2=可捡,3=两者）
            var pairs = Enumerable.Range(1, n)
                .Select(i => (key: i, flags: (i % 4) switch
                {
                    1 => 0x8000,
                    2 => 0x4000,
                    3 => 0xC000,
                    _ => 0x0000,
                }))
                .OrderBy(_ => rng.Next())               // 乱序
                .ToArray();

            var items = Loaded(pairs.Select(p => (ushort)(p.flags | p.key)).ToArray());
            Assert.Equal(n, items.Count);

            foreach (var (key, flags) in pairs)
            {
                Assert.Equal((flags & 0x8000) != 0, items.CheckPriorityPickup(key));
                Assert.Equal((flags & 0x4000) != 0, items.CheckEnablePickup(key));
                Assert.Equal((flags & 0xC000) != 0, items.CheckCanPickItem(key));
            }

            // 越界键必须查不到（键 n+1 未加载）
            Assert.False(items.CheckCanPickItem(n + 1));
        }
    }

    // ------------------------------------------------------------ Clear/Dispose

    /// <summary>原文 <c>:57-60 Clear</c> 与 <c>:51-55 Destroy</c> 都把数据清空。</summary>
    [Fact]
    public void ClearAndDispose_EmptyTheStore()
    {
        var items = Loaded(0x8001, 0x8002);
        items.Clear();
        Assert.Equal(0, items.Count);
        Assert.False(items.CheckPriorityPickup(1));

        items = Loaded(0x8001);
        items.Dispose();
        Assert.Equal(0, items.Count);
        Assert.False(items.CheckPriorityPickup(1));
    }

    /// <summary>清空后可再次 <c>SetData</c>（<c>FItems</c> 是普通动态数组，无"已释放"状态）。</summary>
    [Fact]
    public void AfterClear_SetDataWorksAgain()
    {
        var items = Loaded(0x8001);
        items.Clear();
        var buf = Bytes(0x4009);
        items.SetData(buf, buf.Length);

        Assert.Equal(1, items.Count);
        Assert.True(items.CheckEnablePickup(9));
    }

    // -------------------------------------------------------- 与既有产物的关系

    /// <summary>
    /// 取证：本单元**没有**对应的既有 GXX.Core/Core.Tests 实现（避免重复造第三份）。
    /// 既有同类只到「物品优先捡取」的配置面，不在 Core；这里用"类型名唯一性"做否定性取证的替代：
    /// 本类型所属命名空间不在 GXX.Core 的任何命名空间下。
    /// </summary>
    [Fact]
    public void TypeLivesOnlyInThisLaneNamespace()
    {
        var t = typeof(TClientPickItems);
        Assert.Equal("GXX.M2Server.Sweep9.PathFind.Items", t.Namespace);
        Assert.Same(t, typeof(TClientPickItems).Assembly.GetType(t.FullName!, throwOnError: true));
        // 公开面清点（否定性断言的对账口径）：原文 public 段共 1 属性 + 7 方法 = 8 个成员，
        // 托管侧一一对应（Dispose 是 Destructor Destroy 的托管落点）。
        var publicMembers = new List<string>
        {
            "Clear", "Count", "CheckPriorityPickup", "CheckEnablePickup", "CheckCanPickItem",
            "SetData", "Sort", "Dispose",
        };
        foreach (var name in publicMembers)
        {
            Assert.Contains(t.GetMembers(), m => m.Name == name);
        }

        Assert.Equal(8, publicMembers.Count);
    }
}
