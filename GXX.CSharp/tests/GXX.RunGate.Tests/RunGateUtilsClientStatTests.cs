using System.Collections.Generic;
using Xunit;

namespace GXX.RunGate.Tests;

/// <summary>
/// RunGateUtilsClientStat.cs 测试 —— 覆盖 RunGateUtils.pas:3901-3967（GetClientDate）
/// 及其被注释掉的结构孪生 GetClientRunGateIP（3831-3899）。
/// </summary>
public class RunGateUtilsClientStatTests
{
    private static List<TIPCheckInfo> Make(int n, System.Func<int, (int date, int runGateIp)> f)
    {
        var list = new List<TIPCheckInfo>();
        for (int i = 0; i < n; i++)
        {
            var (d, g) = f(i);
            list.Add(new TIPCheckInfo { IP = (uint)(0x01000000 + i), Date = d, RunGateIP = g });
        }
        return list;
    }

    private static List<TIPCheckInfo> Uniform(int n, int date, int runGateIp = 0)
        => Make(n, _ => (date, runGateIp));

    // ---------------- 样本数门限 ----------------

    [Fact]
    public void FewerThan20Samples_ReturnsZero_EvenIfUnanimous()
    {
        // 原 3910：if List.Count < 20 then Exit
        Assert.Equal(0, RunGateClientStat.GetClientDate(Uniform(19, 20260920)));
        Assert.Equal(20260920, RunGateClientStat.GetClientDate(Uniform(20, 20260920)));
    }

    [Fact]
    public void NullOrEmpty_ReturnsZero()
    {
        Assert.Equal(0, RunGateClientStat.GetClientDate(null));
        Assert.Equal(0, RunGateClientStat.GetClientDate(new List<TIPCheckInfo>()));
    }

    [Fact]
    public void FewerThan20ValidValues_ReturnsZero_EvenWith20Samples()
    {
        // 原 3919/3946：非 0 样本数也要 >= 20
        var list = Uniform(20, 0);
        list[0] = new TIPCheckInfo { Date = 12345 };
        Assert.Equal(0, RunGateClientStat.GetClientDate(list));

        // 恰好 20 个有效值 → 通过
        Assert.Equal(12345, RunGateClientStat.GetClientDate(Uniform(20, 12345)));
    }

    [Fact]
    public void ZeroValuedEntries_AreExcludedFromAllCount()
    {
        // 25 个样本，其中 6 个 Date=0 → AllCount = 19 < 20 → 0（尽管 19 个完全一致）
        var list = Uniform(25, 777);
        for (int i = 0; i < 6; i++) list[i] = new TIPCheckInfo { Date = 0 };
        Assert.Equal(0, RunGateClientStat.GetClientDate(list));

        // 25 个样本，其中 5 个 0 → AllCount = 20 → 通过（重新构造，避免残留）
        var list2 = Uniform(25, 777);
        for (int i = 0; i < 5; i++) list2[i] = new TIPCheckInfo { Date = 0 };
        Assert.Equal(777, RunGateClientStat.GetClientDate(list2));
    }

    // ---------------- 表决语义 ----------------

    [Fact]
    public void HalfThreshold_IsIntegerFloorDivision()
    {
        // AllCount = 21 → half = 10（21 div 2）。10 就够，不需要 11。
        var list = Make(21, i => (i < 10 ? 111 : 222, 0));
        Assert.Equal(111, RunGateClientStat.GetClientDate(list));
    }

    /// <summary>
    /// 核心差异断言：返回值是**首个计数 &gt;= AllCount div 2 的桶**，不是"最大票数"。
    /// </summary>
    [Fact]
    public void Differential_ReturnsFirstBucketReachingHalf_NotTheMaximum()
    {
        // 21 个样本：A×10、B×11。AllCount=21，half=10。
        // 第一个桶（A）就有 10 >= 10 → 返回 A，**尽管 B 的 11 票更多**。
        var list = Make(21, i => (i < 10 ? 0xAAAA : 0xBBBB, 0));
        Assert.Equal(0xAAAA, RunGateClientStat.GetClientDate(list));

        // 若把 A 降到 9 票（剩下 12 票给 B）→ A 不达标，B 达标 → B
        list = Make(21, i => (i < 9 ? 0xAAAA : 0xBBBB, 0));
        Assert.Equal(0xBBBB, RunGateClientStat.GetClientDate(list));
    }

    [Fact]
    public void Tie_ReturnsTheEarliestAppearingValue()
    {
        var list = Make(20, i => (i < 10 ? 111 : 222, 0));
        Assert.Equal(111, RunGateClientStat.GetClientDate(list));

        // 调换出现顺序 → 结果跟着换（证明"首次出现顺序"是语义的一部分）
        var swapped = Make(20, i => (i < 10 ? 222 : 111, 0));
        Assert.Equal(222, RunGateClientStat.GetClientDate(swapped));
    }

    [Fact]
    public void NoBucketReachesHalf_ReturnsZero()
    {
        // 21 个样本：6 / 6 / 9 → 最大 9 < 10 → 0
        var list = Make(21, i => (i < 6 ? 1 : (i < 12 ? 2 : 3), 0));
        Assert.Equal(0, RunGateClientStat.GetClientDate(list));
    }

    [Fact]
    public void BucketOrderIsByFirstAppearance_NotByValue()
    {
        // 交错出现：3,1,3,1,... 首个出现的值是 3
        var list = Make(20, i => (i % 2 == 0 ? 3 : 1, 0));   // 3 出现 10 次，1 出现 10 次
        Assert.Equal(3, RunGateClientStat.GetClientDate(list));
    }

    // ---------------- 孪生函数差异 ----------------

    [Fact]
    public void Twin_GetClientRunGateIP_StatisticsUseRunGateIPField()
    {
        var list = Make(20, i => (date: 5000, runGateIp: i < 12 ? 0x0A0A0A0A : 0x0B0B0B0B));
        // Date 字段全部相同 → GetClientDate 返回它
        Assert.Equal(5000, RunGateClientStat.GetClientDate(list));
        // RunGateIP：12 >= 10 → 返回第一个出现的 0x0A0A0A0A
        Assert.Equal(0x0A0A0A0A, RunGateClientStat.GetClientRunGateIP(list));
    }

    [Fact]
    public void Twin_BothFieldsZero_ReturnZero()
    {
        Assert.Equal(0, RunGateClientStat.GetClientDate(Uniform(30, 0, 0)));
        Assert.Equal(0, RunGateClientStat.GetClientRunGateIP(Uniform(30, 0, 0)));
    }

    [Fact]
    public void Twin_RunGateIPIgnoresDateFieldEntirely()
    {
        // RunGateIP 全 0 → 0；Date 有值也不影响
        var list = Uniform(30, 0x9999, 0);
        Assert.Equal(0x9999, RunGateClientStat.GetClientDate(list));
        Assert.Equal(0, RunGateClientStat.GetClientRunGateIP(list));
    }

    [Fact]
    public void Majority_ExplicitFieldSelection_MatchesConvenienceWrappers()
    {
        var list = Make(20, i => (111, 222));
        Assert.Equal(RunGateClientStat.Majority(list, TIPCheckField.Date), RunGateClientStat.GetClientDate(list));
        Assert.Equal(RunGateClientStat.Majority(list, TIPCheckField.RunGateIP), RunGateClientStat.GetClientRunGateIP(list));
    }

    [Fact]
    public void StructLayout_TIPCheckInfo_Is12BytesPacked()
    {
        // 原 3819-3823：packed record Cardinal+Integer+Integer
        Assert.Equal(12, System.Runtime.InteropServices.Marshal.SizeOf<TIPCheckInfo>());
        Assert.Equal(8, System.Runtime.InteropServices.Marshal.SizeOf<TCheckItemInfo>());
    }
}
