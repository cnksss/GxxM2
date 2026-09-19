using GXX.DBServer;
using Xunit;

namespace GXX.DBServer.Tests;

/// <summary>uRunGateList.pas:1-142 TRunGateInfo / TRunGateList 语义（含 DoSort 的降序与陈旧状态）。</summary>
public class uRunGateListTests : TempDirTest
{
    [Fact]
    public void Add_写入全部字段_LastResponseTick_取时钟_IsConnect_保持0()
    {
        DelphiTick.GetTickCount = () => 12345;
        var list = new TRunGateList();
        TRunGateInfo info = list.Add(1, "10.0.0.1", 7200, 7300, 7);

        Assert.Equal(1, list.Count);
        Assert.Equal((byte)1, info.Enabled);
        Assert.Equal("10.0.0.1", info.IP.Value);
        Assert.Equal((ushort)7200, info.Port);
        Assert.Equal((ushort)7300, info.DBPort);
        Assert.Equal(7, info.Level);
        Assert.Equal(12345u, info.LastResponseTick);
        // 原文如此：Add 不初始化 IsConnect（保持 New 出来的 False）
        Assert.Equal((byte)0, info.IsConnect);
    }

    [Fact]
    public void Add_不写入SortItems_排序前SortItems为空()
    {
        var list = new TRunGateList();
        list.Add(1, "1.1.1.1", 1, 1, 10);

        Assert.Equal(1, list.Count);
        Assert.Null(list.SortItems(0));   // FSortList 尚未 Assign
    }

    [Fact]
    public void Items与SortItems_越界返回null_与Delphi相同()
    {
        var list = new TRunGateList();
        list.Add(1, "1.1.1.1", 1, 1, 10);
        list.Add(1, "2.2.2.2", 2, 2, 20);
        list.DoSort();

        Assert.NotNull(list.Items(0));
        Assert.Null(list.Items(-1));
        Assert.Null(list.Items(2));
        Assert.NotNull(list.SortItems(0));
        Assert.Null(list.SortItems(-1));
        Assert.Null(list.SortItems(2));
    }

    [Fact]
    public void DoSort_按Level降序_且SortItems与Items共享同一批对象()
    {
        var list = new TRunGateList();
        var a = list.Add(1, "a", 1, 1, 10);
        var b = list.Add(1, "b", 2, 2, 30);
        var c = list.Add(1, "c", 3, 3, 20);

        list.DoSort();

        Assert.Same(b, list.SortItems(0));
        Assert.Same(c, list.SortItems(1));
        Assert.Same(a, list.SortItems(2));
        // FList 的插入序不变
        Assert.Same(a, list.Items(0));
        Assert.Same(b, list.Items(1));
        Assert.Same(c, list.Items(2));
    }

    [Fact]
    public void DoSort_相同Level保持相对次序的稳定性由QuickSort决定_此处锁定实际结果()
    {
        var list = new TRunGateList();
        var a = list.Add(1, "a", 1, 1, 10);
        var b = list.Add(1, "b", 2, 2, 10);
        var c = list.Add(1, "c", 3, 3, 10);
        list.DoSort();

        // 三个同值：pivot = 中间元素，QuickSort 会把首尾互换 → 实际顺序是 c, b, a（原文算法如此）
        Assert.Same(c, list.SortItems(0));
        Assert.Same(b, list.SortItems(1));
        Assert.Same(a, list.SortItems(2));
    }

    [Fact]
    public void DoSort_元素个数小于等于1时不重建SortItems_原文如此()
    {
        var list = new TRunGateList();
        var a = list.Add(1, "a", 1, 1, 10);
        var b = list.Add(1, "b", 2, 2, 30);
        list.DoSort();
        Assert.Same(b, list.SortItems(0));

        list.Clear();
        list.Add(1, "solo", 1, 1, 5);
        list.DoSort();                     // FList.Count = 1 → 不重建

        Assert.Equal(1, list.Count);
        Assert.Null(list.SortItems(0));    // 仍是 Clear 后的空 FSortList
    }

    [Fact]
    public void Clear_清空两个列表()
    {
        var list = new TRunGateList();
        list.Add(1, "a", 1, 1, 10);
        list.Add(1, "b", 2, 2, 20);
        list.DoSort();
        list.Clear();

        Assert.Equal(0, list.Count);
        Assert.Null(list.Items(0));
        Assert.Null(list.SortItems(0));
    }

    [Fact]
    public void DoSort_大数据量下SortItems严格降序()
    {
        var list = new TRunGateList();
        var rnd = new System.Random(20250914);
        for (int i = 0; i < 200; i++) list.Add(1, "ip", 1, 1, rnd.Next(0, 50));
        list.DoSort();

        for (int i = 1; i < list.Count; i++)
            Assert.True(list.SortItems(i - 1).Level >= list.SortItems(i).Level,
                $"index {i}: {list.SortItems(i - 1).Level} < {list.SortItems(i).Level}");
    }
}
