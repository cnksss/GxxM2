// ============================================================================
// 测试：Source/M2Engine/StruckDamageAbsorbUtils.pas → GXX.M2Server.Sweep9.PathFind.Damage（1:1）
//
// 覆盖策略（任务书第 5 条）：每个公开成员 ≥1 例；分支/边界/原文缺陷各配差异断言。
// 时间源（`SweepSeam.MyGetTickCount`）与随机源（`PathFindDamageSeam.Random`）在本文件里
// 由 `Env` 夹具**显式钉死并还原**，故本类用例与外部残留静态状态完全解耦。
// 所有"否定性断言"都配计数取证（台账 §37.3）。
// ============================================================================

using System;
using System.Linq;
using GXX.M2Server.Sweep;
using GXX.M2Server.Sweep9.PathFind.Damage;
using Xunit;

namespace GXX.M2Server.Tests;

public class Sweep9PathFindAbsorbTests
{
    /// <summary>钉死时间源与随机源；Dispose 时还原（避免污染同进程其它测试类）。</summary>
    private sealed class Env : IDisposable
    {
        private readonly Func<uint> _savedTick = SweepSeam.MyGetTickCount;
        private readonly Func<int, int> _savedRandom = PathFindDamageSeam.Random;

        public uint Tick;

        public Env()
        {
            SweepSeam.MyGetTickCount = () => Tick;
            PathFindDamageSeam.Random = _ => 0;
        }

        public void SetRandom(int value) => PathFindDamageSeam.Random = _ => value;

        public void Dispose()
        {
            SweepSeam.MyGetTickCount = _savedTick;
            PathFindDamageSeam.Random = _savedRandom;
        }
    }

    // ------------------------------------------------------------------ Add

    /// <summary>插入按 <c>AnsiCompareText</c>（大小写不敏感）有序（原文 :101 <c>FList.Insert(Index, ...)</c>）。</summary>
    [Fact]
    public void Add_KeepsListSortedCaseInsensitively()
    {
        using var env = new Env();
        var mgr = new TStruckDamageAbsorbMgr();

        mgr.Add("BBB", 10, 10, 0);
        mgr.Add("aaa", 10, 10, 0);
        mgr.Add("CCC", 10, 10, 0);

        Assert.Equal(3, mgr.Count);
        Assert.Equal("aaa", mgr[0].MonsterName);
        Assert.Equal("BBB", mgr[1].MonsterName);
        Assert.Equal("CCC", mgr[2].MonsterName);
    }

    /// <summary>原文 :76-79：率与值都**只裁上界 100**（<c>&gt; 100 then := 100</c>）。</summary>
    [Theory]
    [InlineData(200, 200, 100, 100)]
    [InlineData(255, 101, 100, 100)]
    [InlineData(100, 100, 100, 100)]
    [InlineData(101, 99, 100, 99)]
    [InlineData(99, 101, 99, 100)]
    public void Add_ClampsBothFieldsAtHundred(byte rateIn, byte valueIn, byte rateOut, byte valueOut)
    {
        using var env = new Env();
        var mgr = new TStruckDamageAbsorbMgr();

        var item = mgr.Add("m", rateIn, valueIn, 7);

        Assert.NotNull(item);
        Assert.Equal(rateOut, item.AbsorbDamageRate);
        Assert.Equal(valueOut, item.AbsorbDamageValue);
        Assert.Equal(7u, item.EffectiveTime);
    }

    /// <summary>
    /// 差异断言（原文缺陷 ②）：新名字上 <c>Rate &lt;= 0</c> 或 <c>Value &lt;= 0</c>
    /// ⇒ 直接返回 nil 且**不新增**（原文 :94-98，插入点已算出但被丢弃）。
    /// </summary>
    [Fact]
    public void Add_ZeroRateOrValue_OnNewName_AddsNothing()
    {
        using var env = new Env();
        var mgr = new TStruckDamageAbsorbMgr();

        Assert.Null(mgr.Add("m", 0, 50, 0));
        Assert.Null(mgr.Add("m", 50, 0, 0));
        Assert.Null(mgr.Add("m", 0, 0, 0));
        Assert.Equal(0, mgr.Count);          // 计数取证：三次调用一条都没落表
    }

    /// <summary>
    /// 差异断言（原文缺陷 ②/③）：**已存在**的名字上传 0 ⇒ 该条被**移除**（原文 :84-90）。
    /// 这是"删除"这一语义在原文里唯一的入口 —— 没有单独的 Delete/Remove 方法。
    /// </summary>
    [Fact]
    public void Add_ZeroOnExistingName_RemovesEntry()
    {
        using var env = new Env();
        var mgr = new TStruckDamageAbsorbMgr();

        mgr.Add("m", 50, 50, 0);
        mgr.Add("other", 50, 50, 0);
        Assert.Equal(2, mgr.Count);

        Assert.Null(mgr.Add("m", 0, 50, 0));
        Assert.Equal(1, mgr.Count);
        Assert.Equal("other", mgr[0].MonsterName);

        // Value = 0 走同一条删除路径
        Assert.Null(mgr.Add("other", 50, 0, 0));
        Assert.Equal(0, mgr.Count);
    }

    /// <summary>
    /// 差异断言（原文缺陷 ③）：已存在且率/值都 &gt; 0 时**原地改写**同一个对象
    /// （<c>Result := FList.Items[Index]</c> 后直接赋字段），并**刷新 StartTime**。
    /// 用例用引用恒等 + 计时器推进取证。
    /// </summary>
    [Fact]
    public void Add_OnExistingName_UpdatesInPlaceAndRefreshesStartTime()
    {
        using var env = new Env();
        var mgr = new TStruckDamageAbsorbMgr();

        env.Tick = 100;
        var first = mgr.Add("m", 10, 20, 5);
        Assert.Equal(100u, first.StartTime);

        env.Tick = 500;
        var second = mgr.Add("m", 30, 40, 6);

        Assert.Same(first, second);                 // 同一个对象，不是替换
        Assert.Equal(1, mgr.Count);
        Assert.Equal((byte)30, second.AbsorbDamageRate);
        Assert.Equal((byte)40, second.AbsorbDamageValue);
        Assert.Equal(6u, second.EffectiveTime);
        Assert.Equal(500u, second.StartTime);       // 生命周期被刷新
    }

    /// <summary><c>StartTime := MyGetTickCount</c>（原文 :108），是新增时的时刻。</summary>
    [Fact]
    public void Add_StampsStartTimeFromTickSource()
    {
        using var env = new Env();
        env.Tick = 4242;
        var mgr = new TStruckDamageAbsorbMgr();

        var item = mgr.Add("m", 1, 1, 0);
        Assert.Equal(4242u, item.StartTime);
        Assert.Equal("m", item.MonsterName);
    }

    // ------------------------------------------------------- Count / 索引器

    /// <summary>
    /// <c>Count</c> = 表长；索引器越界返回 <c>null</c>（原文 :118 的双侧判定，
    /// 注意左界是 <c>Index &gt;= 0</c>、右界是 <c>Index &lt;= Count - 1</c>）。
    /// </summary>
    [Fact]
    public void CountAndIndexer_OutOfRangeReturnsNull()
    {
        using var env = new Env();
        var mgr = new TStruckDamageAbsorbMgr();
        Assert.Equal(0, mgr.Count);
        Assert.Null(mgr[0]);
        Assert.Null(mgr[-1]);

        mgr.Add("m", 10, 10, 0);
        Assert.Equal(1, mgr.Count);
        Assert.NotNull(mgr[0]);
        Assert.Null(mgr[1]);
        Assert.Null(mgr[-1]);
        Assert.Null(mgr[int.MaxValue]);
    }

    // ------------------------------------------------------ Clear / Dispose

    /// <summary><c>Clear</c>（原文 :58-69）清空整表；<c>Dispose</c>（原文 :51-56）先 Clear 再释放。</summary>
    [Fact]
    public void ClearAndDispose_EmptyTheTable()
    {
        using var env = new Env();
        var mgr = new TStruckDamageAbsorbMgr();
        mgr.Add("a", 10, 10, 0);
        mgr.Add("b", 10, 10, 0);

        mgr.Clear();
        Assert.Equal(0, mgr.Count);
        Assert.Null(mgr[0]);

        mgr.Add("c", 10, 10, 0);
        mgr.Dispose();
        Assert.Equal(0, mgr.Count);

        // 清空后仍可继续使用（FList 是普通 List，无"已释放"状态）
        mgr.Add("d", 10, 10, 0);
        Assert.Equal(1, mgr.Count);
    }

    // ------------------------------------------------------------------ Run

    /// <summary>
    /// <c>Run</c>（原文 :124-138）：<c>EffectiveTime = 0</c> 的条目**永不过期**；
    /// 非 0 时按 <c>tick_diff(StartTime, now) &gt;= EffectiveTime * 1000</c> 判定。
    /// </summary>
    [Fact]
    public void Run_NeverExpiresZeroEffectiveTime()
    {
        using var env = new Env();
        env.Tick = 0;
        var mgr = new TStruckDamageAbsorbMgr();
        mgr.Add("forever", 10, 10, 0);          // EffectiveTime = 0

        env.Tick = uint.MaxValue;               // 任意大的时间差
        mgr.Run();

        Assert.Equal(1, mgr.Count);
        Assert.Equal("forever", mgr[0].MonsterName);
    }

    /// <summary>
    /// 差异断言（边界为 <c>&gt;=</c>）：<c>EffectiveTime = 2</c> 秒 ⇒ 门限 2000ms，
    /// <c>1999</c> 保留、<c>2000</c> 移除。
    /// </summary>
    [Theory]
    [InlineData(1999u, 1)]
    [InlineData(2000u, 0)]
    [InlineData(2001u, 0)]
    public void Run_ExpiryGateIsGreaterOrEqual(uint elapsed, int expectedCount)
    {
        using var env = new Env();
        env.Tick = 10_000;
        var mgr = new TStruckDamageAbsorbMgr();
        mgr.Add("m", 10, 10, 2);                // 2 秒

        env.Tick = 10_000 + elapsed;
        mgr.Run();

        Assert.Equal(expectedCount, mgr.Count);
    }

    /// <summary>
    /// 回绕路径 + 既有 <c>tick_diff</c> 的 <c>High(Cardinal)</c> 偏差
    /// （<c>MonsterListBuildCore.TickDiff</c> 已登记"回绕分支比真实差值小 1"）。
    /// <para>构造：<c>EffectiveTime = 1</c>（门限 1000ms）、<c>StartTime = MaxValue - 999</c>、
    /// <c>now = 0</c>。此时**真实**流逝是 1000ms（正好到点、本应过期），
    /// 而 <c>tick_diff</c> 只给 999 ⇒ **判定为未过期**。再推进 1ms（<c>now = 1</c>）
    /// <c>tick_diff</c> 给 1000 ⇒ 过期。<b>这一条把"回绕分支少 1"的偏差钉死在行为上。</b></para>
    /// </summary>
    [Fact]
    public void Run_WrapAround_InheritsTickDiffOffByOne()
    {
        using var env = new Env();
        env.Tick = 0;
        var mgr = new TStruckDamageAbsorbMgr();
        mgr.Add("m", 100, 100, 1);
        mgr[0].StartTime = uint.MaxValue - 999;    // 与 now = 0 之间真实相差 1000ms

        env.Tick = 0;
        mgr.Run();
        Assert.Equal(1, mgr.Count);                // tick_diff = 999 < 1000 ⇒ 保留

        // 取证：同一对时刻的 tick_diff 与真实差值
        Assert.Equal(999u, GXX.M2Server.MonsterListBuildCore.TickDiff(uint.MaxValue - 999, 0));
        Assert.Equal(1000u, unchecked(0u - (uint.MaxValue - 999)));   // 真实流逝 1000ms

        env.Tick = 1;
        mgr.Run();
        Assert.Equal(0, mgr.Count);                // tick_diff = 1000 >= 1000 ⇒ 过期
    }

    /// <summary>倒序遍历 ⇒ 一次 <c>Run</c> 可以同时删掉多条（含相邻下标），不会漏删。</summary>
    [Fact]
    public void Run_RemovesMultipleExpiredEntries()
    {
        using var env = new Env();
        env.Tick = 0;
        var mgr = new TStruckDamageAbsorbMgr();
        mgr.Add("a", 10, 10, 1);
        mgr.Add("b", 10, 10, 1);
        mgr.Add("c", 10, 10, 1);
        mgr.Add("d", 10, 10, 0);                // 永不过期
        Assert.Equal(4, mgr.Count);

        env.Tick = 1000;                        // 恰好到点
        mgr.Run();

        Assert.Equal(1, mgr.Count);
        Assert.Equal("d", mgr[0].MonsterName);
    }

    // -------------------------------------------------------- GetStruckDamage

    /// <summary>无命中项 ⇒ 原样返回伤害（原文 :174 <c>Result := nDamage</c>）。</summary>
    [Fact]
    public void GetStruckDamage_NoEntry_ReturnsDamageUnchanged()
    {
        using var env = new Env();
        env.SetRandom(0);
        var mgr = new TStruckDamageAbsorbMgr();
        mgr.Add("zzz", 100, 100, 0);

        Assert.Equal(500, mgr.GetStruckDamage("aaa", 500));
        // 空表同样原样返回
        var empty = new TStruckDamageAbsorbMgr();
        Assert.Equal(500, empty.GetStruckDamage("aaa", 500));
    }

    /// <summary>
    /// 触发门是 <c>Random(100) &lt; AbsorbDamageRate</c>（原文 :185）：
    /// 率 50 ⇒ roll 49 生效、roll 50 不生效（严格小于）。
    /// </summary>
    [Theory]
    [InlineData(49, 50)]      // 命中 ⇒ 吸收一半
    [InlineData(50, 100)]     // 差 1 不命中
    [InlineData(99, 100)]
    public void GetStruckDamage_TriggerGateIsStrictlyLess(int roll, int expected)
    {
        using var env = new Env();
        var mgr = new TStruckDamageAbsorbMgr();
        mgr.Add("m", 50, 50, 0);

        env.SetRandom(roll);
        Assert.Equal(expected, mgr.GetStruckDamage("m", 100));
    }

    /// <summary>
    /// 公式 <c>nDamage - Round(nDamage / 100 * AbsorbDamageValue)</c>（原文 :187）：
    /// <c>/</c> 是浮点除法、<c>Round</c> 是"四舍六入五成双"（<c>MidpointRounding.ToEven</c>）。
    /// 三个 .5 算例把银行家舍入钉死：0.5→0、1.5→2、2.5→2。
    /// </summary>
    [Theory]
    [InlineData(150, 50, 75)]     // 75.0 → 75
    [InlineData(50, 1, 50)]       // 0.5 → ToEven → 0 ⇒ 50-0
    [InlineData(150, 1, 148)]     // 1.5 → ToEven → 2 ⇒ 150-2
    [InlineData(250, 1, 248)]     // 2.5 → ToEven → 2 ⇒ 250-2
    [InlineData(100, 100, 0)]     // 全吸收
    [InlineData(3, 100, 0)]
    public void GetStruckDamage_FormulaUsesFloatingDivideAndBankersRounding(int damage, byte value, int expected)
    {
        using var env = new Env();
        var mgr = new TStruckDamageAbsorbMgr();
        mgr.Add("m", 100, value, 0);

        env.SetRandom(99);        // 率 100 ⇒ roll 0..99 全部命中
        Assert.Equal(expected, mgr.GetStruckDamage("m", damage));
    }

    /// <summary>结果下限钳 0（原文 :188-189）—— 负伤害（治疗/反伤链）取吸收后仍不为负。</summary>
    [Fact]
    public void GetStruckDamage_ClampsNegativeResultToZero()
    {
        using var env = new Env();
        var mgr = new TStruckDamageAbsorbMgr();
        mgr.Add("m", 100, 50, 0);
        env.SetRandom(0);

        // nDamage = -50 ⇒ -50 - round(-25.0) = -25 ⇒ 钳 0
        Assert.Equal(0, mgr.GetStruckDamage("m", -50));
        // 未触发时负值原样返回（钳位只在触发分支内）
        mgr.Clear();
        mgr.Add("m", 100, 0, 0);       // 0 ⇒ 不新增（原文缺陷 ②）
        Assert.Equal(0, mgr.Count);
        Assert.Equal(-50, mgr.GetStruckDamage("m", -50));
    }

    /// <summary>
    /// 差异断言（原文缺陷 ⑥）：查不到精确名时退到通配名 <c>'*'</c>；
    /// <c>'*'</c> 必须**真的在表里**才生效（它只是普通字符串）。
    /// </summary>
    [Fact]
    public void GetStruckDamage_FallsBackToWildcardNameOnlyIfPresent()
    {
        using var env = new Env();
        env.SetRandom(0);
        var mgr = new TStruckDamageAbsorbMgr();

        // 表里没有 '*' ⇒ 不生效
        mgr.Add("aaa", 100, 100, 0);
        Assert.Equal(100, mgr.GetStruckDamage("bbb", 100));

        // 加入 '*' 后生效
        mgr.Add("*", 100, 50, 0);
        Assert.Equal(50, mgr.GetStruckDamage("bbb", 100));
    }

    /// <summary>精确名优先于通配名（原文 :176 先 <c>Search(MonsterName)</c>、再 <c>Search('*')</c>）。</summary>
    [Fact]
    public void GetStruckDamage_ExactNameBeatsWildcard()
    {
        using var env = new Env();
        env.SetRandom(0);
        var mgr = new TStruckDamageAbsorbMgr();
        mgr.Add("*", 100, 90, 0);        // 通配：吸收 90%
        mgr.Add("boss", 100, 10, 0);     // 精确：吸收 10%

        Assert.Equal(90, mgr.GetStruckDamage("boss", 100));   // 用精确那条
        Assert.Equal(10, mgr.GetStruckDamage("other", 100));  // 其余走通配
    }

    /// <summary>
    /// 差异断言（原文缺陷 ②的推论）：<c>Add</c> 拒绝率 0 ⇒ 表里**不可能存在率 0 的条目**，
    /// 于是 <c>Random(100) &lt; 0</c> 这条"永不触发"的分支在公开 API 下不可达。
    /// 计数取证：先证 <c>Add(rate=0)</c> 一条都进不去，再证此时任何查询都不吸收。
    /// </summary>
    [Fact]
    public void ZeroRateEntryIsUnreachableThroughPublicApi()
    {
        using var env = new Env();
        env.SetRandom(0);
        var mgr = new TStruckDamageAbsorbMgr();

        Assert.Null(mgr.Add("m", 0, 100, 0));
        Assert.Equal(0, mgr.Count);                              // 0 条
        Assert.Equal(100, mgr.GetStruckDamage("m", 100));        // 因此不吸收

        // 率 1 ⇒ roll 0 命中（唯一一个能通过的 roll）
        mgr.Add("m", 1, 100, 0);
        Assert.Equal(1, mgr.Count);
        Assert.Equal(0, mgr.GetStruckDamage("m", 100));

        // roll 1 ⇒ 不命中（率 1 只覆盖 roll = 0）
        env.SetRandom(1);
        Assert.Equal(100, mgr.GetStruckDamage("m", 100));
    }

    /// <summary>更新已有条目会重新排序键值以外的字段，故查表仍然命中（不产生重复项）。</summary>
    [Fact]
    public void Add_RepeatedSameName_NeverDuplicates()
    {
        using var env = new Env();
        env.SetRandom(0);
        var mgr = new TStruckDamageAbsorbMgr();

        for (int i = 1; i <= 30; i++)
        {
            mgr.Add("m", (byte)i, (byte)i, 0);
        }

        Assert.Equal(1, mgr.Count);              // 计数取证：30 次调用只留 1 条
        Assert.Equal((byte)30, mgr[0].AbsorbDamageRate);
    }

    /// <summary>规模覆盖：乱序插入 40 个名字后整表有序、且每个名字都能被查表命中。</summary>
    [Fact]
    public void AddAndSearch_SortedAndFindable_ForFortyNames()
    {
        using var env = new Env();
        env.SetRandom(0);
        var mgr = new TStruckDamageAbsorbMgr();

        var names = Enumerable.Range(0, 40).Select(i => $"mon{i:D2}").ToArray();
        // 确定性乱序（按十位分组 ⇒ 0,10,20,30,1,11,…），不依赖进程级字符串哈希种子
        var shuffled = names.OrderBy(n => n[3]).ThenBy(n => n[4]).ToArray();
        foreach (var n in shuffled)
        {
            mgr.Add(n, 100, 100, 0);
        }

        Assert.Equal(40, mgr.Count);
        for (int i = 0; i < 40; i++)
        {
            Assert.Equal(names[i], mgr[i].MonsterName);          // 已按 OrdinalIgnoreCase 升序
        }

        foreach (var n in names)
        {
            Assert.Equal(0, mgr.GetStruckDamage(n, 100));        // 全部命中 ⇒ 全吸收
        }

        Assert.Equal(100, mgr.GetStruckDamage("nope", 100));     // 未插入的名字不受影响
    }
}
