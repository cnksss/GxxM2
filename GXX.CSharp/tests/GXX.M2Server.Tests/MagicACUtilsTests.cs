using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J59：uMagicACUtils.pas TMagicACList / TMagicACInfo 1:1 测试。
/// Add 去重与 MagicID=12 特例、越界访问、首次排序门控、QuickSort 稳定性与 Get 二分/线性两路。
/// </summary>
public sealed class MagicACUtilsTests : IDisposable
{
    public MagicACUtilsTests() => MagicACUtils.ResetForTests();

    public void Dispose() => MagicACUtils.ResetForTests();

    [Fact]
    public void Add_DedupesByMagicId_ReturnsExistingInstance()
    {
        var list = new TMagicACList("t");
        var a = list.Add(5, "火球术");
        Assert.Equal(1, list.Count);
        Assert.Equal(5, a.wMagicId);
        Assert.Equal("火球术", a.sMagicName);
        Assert.False(a.boEnabled);
        Assert.Equal(0, a.btHum);

        // 重复 ID → 返回既有记录，数量不变，名字不被覆盖
        var again = list.Add(5, "另一个名字");
        Assert.Same(a, again);
        Assert.Equal(1, list.Count);
        Assert.Equal("火球术", again.sMagicName);
    }

    [Fact]
    public void Add_MagicId12_HasSpecialDefaults()
    {
        var list = new TMagicACList();
        var info = list.Add(12, "施毒术");
        Assert.True(info.boEnabled);
        Assert.Equal(100, info.btHum);
        Assert.Equal(100, info.btMon);
        Assert.Equal(100, info.btHero);
        Assert.Equal(0, info.btDefenceHum);
        Assert.Equal(0, info.btDefenceMon);
        Assert.Equal(0, info.btDefenceHero);

        // 非 12 全 0 且未启用
        var other = list.Add(13, "其他");
        Assert.False(other.boEnabled);
        Assert.Equal(0, other.btHum);
    }

    [Fact]
    public void Indexer_OutOfRangeReturnsNull()
    {
        var list = new TMagicACList();
        list.Add(1, "A");
        list.Add(2, "B");
        Assert.Null(list[-1]);
        Assert.NotNull(list[0]);
        Assert.NotNull(list[1]);
        Assert.Null(list[2]);
    }

    [Fact]
    public void Sort_OrdersByMagicId_AndLocksSortFlag()
    {
        var list = new TMagicACList();
        list.Add(30, "C");
        list.Add(10, "A");
        list.Add(20, "B");
        Assert.False(list.IsSorted);

        list.Sort();
        Assert.True(list.IsSorted);
        Assert.Equal(10, list[0]!.wMagicId);
        Assert.Equal(20, list[1]!.wMagicId);
        Assert.Equal(30, list[2]!.wMagicId);

        // FIsSort 已置真 → 再次 Sort 不重排（原文 if not FIsSort 门控）：新项追加在尾部
        list.Add(5, "D");
        list.Sort();
        Assert.Equal(new ushort[] { 10, 20, 30, 5 },
            new[] { list[0]!.wMagicId, list[1]!.wMagicId, list[2]!.wMagicId, list[3]!.wMagicId });
    }

    [Fact]
    public void Sort_SingleOrEmptyList_KeepsFlagFalse()
    {
        var empty = new TMagicACList();
        empty.Sort();
        Assert.False(empty.IsSorted);

        var one = new TMagicACList();
        one.Add(7, "X");
        one.Sort();
        Assert.False(one.IsSorted);   // Count 不大于 1
    }

    [Fact]
    public void Sort_LargeShuffledList_FullyOrdered()
    {
        var list = new TMagicACList();
        var rnd = new Random(20260918);
        var expected = new List<ushort>();
        for (int i = 0; i < 200; i++)
        {
            ushort id = (ushort)rnd.Next(1, 5000);
            if (expected.Contains(id))
                continue;
            expected.Add(id);
            list.Add(id, "M" + id);
        }
        list.Sort();
        expected.Sort();

        Assert.Equal(expected.Count, list.Count);
        for (int i = 0; i < expected.Count; i++)
            Assert.Equal(expected[i], list[i]!.wMagicId);
    }

    [Fact]
    public void Get_UsesBinarySearchAfterSort_LinearBefore()
    {
        var list = new TMagicACList();
        list.Add(50, "E");
        list.Add(10, "A");
        list.Add(30, "C");

        // 未排序：线性查找仍命中
        Assert.False(list.IsSorted);
        Assert.Equal("C", list.Get(30)!.sMagicName);
        Assert.Null(list.Get(99));

        list.Sort();
        Assert.True(list.IsSorted);
        // 排序后二分
        Assert.Equal("A", list.Get(10)!.sMagicName);
        Assert.Equal("E", list.Get(50)!.sMagicName);
        Assert.Null(list.Get(20));
        Assert.Null(list.Get(0));
        Assert.Null(list.Get(65535));
    }

    [Fact]
    public void CustomSort_UsesSuppliedComparator_AndMarksSorted()
    {
        var list = new TMagicACList();
        list.Add(10, "A");
        list.Add(30, "C");
        list.Add(20, "B");

        // 降序比较器
        list.CustomSort((a, b) => b.wMagicId - a.wMagicId);
        Assert.True(list.IsSorted);
        Assert.Equal(30, list[0]!.wMagicId);
        Assert.Equal(20, list[1]!.wMagicId);
        Assert.Equal(10, list[2]!.wMagicId);

        // 排序后 Get 的二分按 wMagicId 差值比较（与自定义比较器无关）：降序表下 20 命中
        Assert.NotNull(list.Get(20));
    }

    [Fact]
    public void Clear_EmptiesList()
    {
        var list = new TMagicACList();
        list.Add(1, "A");
        list.Add(2, "B");
        Assert.Equal(2, list.Count);
        list.Clear();
        Assert.Equal(0, list.Count);
        Assert.Null(list[0]);
        Assert.Null(list.Get(1));
    }

    [Fact]
    public void UnitGlobal_MagicACList_AndReset()
    {
        MagicACUtils.MagicACList.Add(3, "火墙");
        Assert.Equal(1, MagicACUtils.MagicACList.Count);
        MagicACUtils.ResetForTests();
        Assert.Equal(0, MagicACUtils.MagicACList.Count);
        Assert.False(MagicACUtils.MagicACList.IsSorted);
    }
}
