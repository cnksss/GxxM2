// ============================================================================
// 测试：Source/M2Engine/PathFind_Hero.pas → GXX.M2Server.Sweep9.PathFind.Hero（1:1）
//
// 覆盖策略（任务书第 5 条）：每个公开成员 ≥1 例；分支/边界/原文缺陷各配差异断言。
// * 本单元是**服务端版**：`GetCost` 带 `boFlag`、`FillPathMap` 带两道 2000 次上限、
//   `TFindPath` 用 `TEnvirnoment` + 临界区 + 三重重载。
// * 私有成员（`DirToDX/DirToDY/PreparePathMap/TestNeighbours/ExchangeWaves/GetNextDirection`）
//   一律走**可观察出口**取证（记录回调 / FillPathMap 结果 / WalkToRun 是否合并）。
// * 接缝（`PathFindHeroSeam`）与 `M2Config` 的三个开关由夹具显式钉死并还原。
// ============================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GXX.M2Server.Engine;
using GXX.M2Server.Sweep9.PathFind.Hero;
using PathPoint = GXX.Core.Util.PathPoint;
using TPathMapCell = GXX.Core.Util.TPathMapCell;
using Xunit;

namespace GXX.M2Server.Tests;

public class Sweep9PathFindHeroTests
{
    // ------------------------------------------------------------------ 夹具

    private sealed class Env : IDisposable
    {
        private readonly Func<bool> _multi = PathFindHeroSeam.MultiThreadRun;
        private readonly Func<TEnvirnoment, int, int, bool, bool> _walk = PathFindHeroSeam.CanWalkEx;
        private readonly Func<TEnvirnoment, TCreature, int, int, bool, bool> _walkObj = PathFindHeroSeam.CanWalkExObject;
        private readonly Func<TCreature, int> _perm = PathFindHeroSeam.GetPermission;
        private readonly Func<TCreature, bool> _safe = PathFindHeroSeam.InSafeZone;

        private readonly bool _diableHumanRun = M2Config.boDiableHumanRun;
        private readonly bool _gmRunAll = M2Config.boGMRunAll;
        private readonly bool _safeAreaLimited = M2Config.boSafeAreaLimited;

        public Env()
        {
            PathFindHeroSeam.ResetDefaults();
            M2Config.boDiableHumanRun = false;
            M2Config.boGMRunAll = false;
            M2Config.boSafeAreaLimited = false;
        }

        public void Dispose()
        {
            PathFindHeroSeam.MultiThreadRun = _multi;
            PathFindHeroSeam.CanWalkEx = _walk;
            PathFindHeroSeam.CanWalkExObject = _walkObj;
            PathFindHeroSeam.GetPermission = _perm;
            PathFindHeroSeam.InSafeZone = _safe;

            M2Config.boDiableHumanRun = _diableHumanRun;
            M2Config.boGMRunAll = _gmRunAll;
            M2Config.boSafeAreaLimited = _safeAreaLimited;
        }
    }

    /// <summary>
    /// 可记录回调 + 可调 <c>protected</c> 成员的探针（代价恒定 1）。
    /// <para>⚠ 覆写 <c>GetCost</c> 会**绕过基类的越界判断**（基类只在 <c>TFindPath</c> 里判界；
    /// <c>TPathMap.GetCost</c> 本体返回 0、**不判界**）⇒ 探针必须自己复刻 <c>TFindPath.GetCost</c>
    /// 的越界 -1 规则，否则 <c>TestNeighbours</c> 会拿负坐标索引 <c>Result</c>。</para>
    /// </summary>
    private sealed class RecordingMap : TPathMap
    {
        public readonly List<(int x, int y, int dir, bool boFlag)> Calls = new();

        protected override int GetCost(int X, int Y, int Direction, bool boFlag)
        {
            Calls.Add((X, Y, Direction, boFlag));
            if (X < 0 || X >= ClientRect.Right - ClientRect.Left ||
                Y < 0 || Y >= ClientRect.Bottom - ClientRect.Top)
                return -1;
            return 1;
        }

        public TPathMapCell[][] CallFillPathMap(int x1, int y1, int x2, int y2, bool boFlag)
            => FillPathMap(x1, y1, x2, y2, boFlag);

        public int CallGetCost(int x, int y, int d, bool boFlag) => GetCost(x, y, d, boFlag);
    }

    private sealed class ProbeFindPath : TFindPath
    {
        public int CallGetCost(int x, int y, int d, bool boFlag) => GetCost(x, y, d, boFlag);
    }

    private static PathPoint[] StraightLine(int n)
        => Enumerable.Range(0, n).Select(i => new PathPoint(i, 0)).ToArray();

    // ------------------------------------------------------------- TWave

    /// <summary><c>Create</c> 只调 <c>Clear</c> ⇒ 初始为空、<c>MinCost = High(Integer)</c>。</summary>
    [Fact]
    public void Wave_CreateIsCleared()
    {
        var wave = new TWave();
        Assert.False(wave.Start());
        Assert.Equal(int.MaxValue, wave.MinCost);
        Assert.False(wave.Next());
    }

    /// <summary><c>Add</c> 逐个入队并维护 <c>FMinCost</c>；<c>Start/Next/Item</c> 构成顺序遍历。</summary>
    [Fact]
    public void Wave_AddStartNextItemAndMinCost()
    {
        var wave = new TWave();
        wave.Add(1, 2, 5, 0);
        wave.Add(3, 4, 3, 2);
        wave.Add(5, 6, 9, 7);

        Assert.Equal(3, wave.MinCost);

        Assert.True(wave.Start());
        Assert.Equal((1, 2, 5, 0), (wave.Item.X, wave.Item.Y, wave.Item.Cost, wave.Item.Direction));
        Assert.True(wave.Next());
        Assert.Equal((3, 4, 3, 2), (wave.Item.X, wave.Item.Y, wave.Item.Cost, wave.Item.Direction));
        Assert.True(wave.Next());
        Assert.Equal((5, 6, 9, 7), (wave.Item.X, wave.Item.Y, wave.Item.Cost, wave.Item.Direction));
        Assert.False(wave.Next());
    }

    /// <summary>容量按 <c>+30</c> 增长（原文 <c>SetLength(FData, Length(FData) + 30)</c>）：加 100 条不越界。</summary>
    [Fact]
    public void Wave_AddGrowsCapacityInStepsOfThirty()
    {
        var wave = new TWave();
        for (int i = 0; i < 100; i++)
            wave.Add(i, i, 1, i & 7);

        Assert.True(wave.Start());
        Assert.Equal(0, wave.Item.X);
        int seen = 1;
        while (wave.Next()) seen++;
        Assert.Equal(100, seen);                 // 计数取证：100 条一条不漏
    }

    /// <summary>
    /// <c>Clear</c> 把 <c>FPos/FCount/FData/FMinCost</c> 全部复位（本版比 PathFindClient / Core 版**多一句
    /// <c>FData := nil</c>**，差异仅在容量复用，见 D-P9-12）；清空后可继续 <c>Add</c>。
    /// </summary>
    [Fact]
    public void Wave_ClearResetsEverythingAndAllowsReuse()
    {
        var wave = new TWave();
        wave.Add(1, 1, 4, 0);
        wave.Clear();

        Assert.False(wave.Start());
        Assert.Equal(int.MaxValue, wave.MinCost);

        wave.Add(2, 2, 6, 3);
        Assert.True(wave.Start());
        Assert.Equal((2, 2, 6, 3), (wave.Item.X, wave.Item.Y, wave.Item.Cost, wave.Item.Direction));
        Assert.Equal(6, wave.MinCost);
    }

    /// <summary><c>Destructor Destroy</c> 只把 <c>FData</c> 置空（原文 <c>:130-134</c>）——
    /// <b>不清 <c>FCount</c></b> ⇒ <c>Start()</c> 仍返回真（因为它只判 <c>FCount &gt; 0</c>），
    /// 而 <c>Item</c> 会越界。原文如此，逐条锁定。
    /// </summary>
    [Fact]
    public void Wave_DestroyEmptiesDataButLeavesCount()
    {
        var wave = new TWave();
        wave.Add(1, 1, 1, 0);
        wave.Destroy();
        Assert.True(wave.Start());                                   // FCount 仍是 1
        Assert.Throws<IndexOutOfRangeException>(() => _ = wave.Item); // 数据已没了
    }

    // --------------------------------------------------- TPathMap 基础

    /// <summary><c>Create</c>：<c>ScopeValue := 24 * 4 = 96</c>；<c>GetCostFunc := nil</c>。</summary>
    [Fact]
    public void Map_CreateSetsScopeValue96()
    {
        var map = new TPathMap();
        Assert.Equal(96, map.ScopeValue);
        Assert.Null(map.GetCostFunc);
        Assert.False(map.StartFind);
        Assert.Equal(0, map.Width);
        Assert.Equal(0, map.Height);
        Assert.Equal(new Rectangle(0, 0, 0, 0), map.ClientRect);
        Assert.NotNull(map.PathMapArray);
        Assert.Empty(map.PathMapArray);
    }

    /// <summary><c>GetClientRect</c>（**无参**）恒为 <c>Bounds(0,0,Width,Height)</c>，读的是公开字段。</summary>
    [Fact]
    public void Map_GetClientRectUsesPublicWidthHeightFields()
    {
        var map = new TPathMap { Width = 4, Height = 6 };
        map.GetClientRect();
        Assert.Equal(new Rectangle(0, 0, 4, 6), map.ClientRect);

        map.Height = 7;
        map.GetClientRect();
        Assert.Equal(new Rectangle(0, 0, 4, 7), map.ClientRect);
    }

    /// <summary><c>MapX/MapY/LoaclX/LoaclY</c> 以 <c>ClientRect.Left/Top</c> 为基准。</summary>
    [Fact]
    public void Map_CoordinateConversions()
    {
        var map = new TPathMap { Width = 5, Height = 5 };
        map.GetClientRect();
        Assert.Equal(3, map.MapX(3));
        Assert.Equal(4, map.MapY(4));
        Assert.Equal(3, map.LoaclX(3));
        Assert.Equal(4, map.LoaclY(4));

        map.ClientRect = new Rectangle(10, 20, 5, 5);
        Assert.Equal(13, map.MapX(3));
        Assert.Equal(24, map.MapY(4));
        Assert.Equal(-7, map.LoaclX(3));
        Assert.Equal(-16, map.LoaclY(4));
    }

    /// <summary>
    /// 差异断言（原文缺陷 ⑦）：<c>TPathMap.GetCost</c> 基类返回 **0**（不是 -1）
    /// ⇒ 只要不覆写，`TestNeighbours` 的 `C &gt;= 0` 恒真、任何格子都被当作可走。
    /// </summary>
    [Fact]
    public void Map_BaseGetCostReturnsZeroNotMinusOne()
    {
        // 通过一个只暴露基类行为的子类调用（基类是 protected）
        var probe = new BaseProbe { Width = 3, Height = 3 };
        probe.GetClientRect();
        Assert.Equal(0, probe.CallGetCost(1, 1, 0, false));
        Assert.Equal(0, probe.CallGetCost(99, 99, 5, true));   // 基类连越界都不判
    }

    private sealed class BaseProbe : TPathMap
    {
        public int CallGetCost(int x, int y, int d, bool boFlag) => GetCost(x, y, d, boFlag);
    }

    // ------------------------------------------------ 方向表 / FillPathMap

    /// <summary>
    /// 用"记录 (x,y,方向)"的覆写从全图扩散里反推 <c>DirToDX/DirToDY</c> 的 8 项偏移表。
    /// </summary>
    [Fact]
    public void Directions_EightOffsetsMatchSourceLayout()
    {
        var map = new RecordingMap { Width = 5, Height = 5 };
        map.StartFind = true;
        map.CallFillPathMap(2, 2, -1, -1, false);

        // 起点格 (2,2) 的前 8 次回调即 8 个方向（每次 TestNeighbours 都是 D = 0..7）
        var first8 = map.Calls.Take(8).ToArray();
        Assert.Equal(8, first8.Length);
        Assert.Equal(new[] { 0, 1, 2, 3, 4, 5, 6, 7 }, first8.Select(c => c.dir).ToArray());

        // 用相对位移断言方向 → 偏移（按原文注释 7 0 1 / 6 X 2 / 5 4 3）
        var offsets = first8.ToDictionary(c => c.dir, c => (dx: c.x - 2, dy: c.y - 2));
        Assert.Equal((0, -1), offsets[0]);
        Assert.Equal((1, -1), offsets[1]);
        Assert.Equal((1, 0), offsets[2]);
        Assert.Equal((1, 1), offsets[3]);
        Assert.Equal((0, 1), offsets[4]);
        Assert.Equal((-1, 1), offsets[5]);
        Assert.Equal((-1, 0), offsets[6]);
        Assert.Equal((-1, -1), offsets[7]);
    }

    /// <summary>
    /// 差异断言（原文缺陷 ①）：两道越界守卫**维数错位** ——
    /// `nX1 &gt;= Length(Result)`（列号比行数）与 `nY1 &gt;= Length(Result[0])`（行号比列数）。
    /// <para>取 <c>Width=2, Height=10</c>（rows=10, cols=2）：
    /// 起点 <c>(0,2)</c> 是**合法**格（行 2 &lt; 10、列 0 &lt; 2），却因 <c>2 &gt;= 2</c> 被判非法 ⇒ 空图；
    /// 起点 <c>(5,0)</c> 是**非法**格（列 5 ≧ 2），却因 <c>5 &gt;= 10</c> 为假而被放行 ⇒ 越界崩溃。</para>
    /// </summary>
    [Fact]
    public void FillPathMap_TransposedGuardsRejectLegalStartAndAcceptIllegalOne()
    {
        // 合法格被拒
        var a = new RecordingMap { Width = 2, Height = 10 };
        a.StartFind = true;
        var empty = a.CallFillPathMap(0, 2, -1, -1, false);
        Assert.Empty(empty);

        // 非法格被放行 ⇒ 越界
        var b = new RecordingMap { Width = 2, Height = 10 };
        b.StartFind = true;
        Assert.ThrowsAny<Exception>(() => b.CallFillPathMap(5, 0, -1, -1, false));

        // 对照组：合法且行号 < 列数 的起点能正常建图
        var c = new RecordingMap { Width = 2, Height = 10 };
        c.StartFind = true;
        var ok = c.CallFillPathMap(0, 1, -1, -1, false);
        Assert.Equal(10, ok.Length);             // rows = Height
        Assert.Equal(2, ok[0].Length);           // cols = Width
    }

    /// <summary>统一代价 1、1 行的地图：距离按列递增、方向恒 2（右）。</summary>
    [Fact]
    public void FillPathMap_UniformCostDistances()
    {
        var map = new RecordingMap { Width = 5, Height = 1 };
        map.StartFind = true;

        var result = map.CallFillPathMap(0, 0, 4, 0, false);

        Assert.Single(result);
        Assert.Equal(5, result[0].Length);
        for (int x = 0; x < 5; x++)
            Assert.Equal(x, result[0][x].Distance);
        for (int x = 1; x < 5; x++)
            Assert.Equal(2, result[0][x].Direction);
    }

    /// <summary>跨度检查（<c>abs(nX1-nX2) &gt; 宽度</c>，严格大于）⇒ 过远目标返回空图。</summary>
    [Theory]
    [InlineData(4, 1)]
    [InlineData(5, 1)]
    [InlineData(6, 0)]
    public void FillPathMap_SpanCheckIsStrictlyGreater(int stopX, int expectedRows)
    {
        var map = new RecordingMap { Width = 5, Height = 1 };
        map.StartFind = true;

        var result = map.CallFillPathMap(0, 0, stopX, 0, false);
        Assert.Equal(expectedRows, result.Length);
    }

    /// <summary>障碍墙切断地图 ⇒ 墙右侧保持 -1。</summary>
    [Fact]
    public void FillPathMap_ObstacleLeavesUnreachableAtMinusOne()
    {
        var map = new ObstacleMap { Width = 5, Height = 1, WallX = 2 };
        map.StartFind = true;

        var result = map.CallFillPathMap(0, 0, 4, 0, false);

        Assert.Equal(0, result[0][0].Distance);
        Assert.Equal(1, result[0][1].Distance);
        Assert.Equal(-1, result[0][2].Distance);
        Assert.Equal(-1, result[0][4].Distance);
    }

    private sealed class ObstacleMap : TPathMap
    {
        public int WallX = -1;

        protected override int GetCost(int X, int Y, int Direction, bool boFlag)
        {
            if (X < 0 || X >= ClientRect.Right - ClientRect.Left ||
                Y < 0 || Y >= ClientRect.Bottom - ClientRect.Top)
                return -1;
            return X == WallX ? -1 : 1;
        }

        public TPathMapCell[][] CallFillPathMap(int x1, int y1, int x2, int y2, bool boFlag)
            => FillPathMap(x1, y1, x2, y2, boFlag);
    }

    // ------------------------------------------------------- FindPathOnMap

    /// <summary>
    /// <c>FindPathOnMap(X, Y, Run)</c>：回溯出路径后**把 <c>PathMapArray</c> 释放掉**；
    /// <c>Run=False</c> 时不合并。
    /// </summary>
    [Fact]
    public void FindPathOnMap_ReturnsPathAndReleasesMapArray()
    {
        var map = new RecordingMap { Width = 5, Height = 1 };
        map.StartFind = true;
        map.PathMapArray = map.CallFillPathMap(0, 0, 4, 0, false);

        var path = map.FindPathOnMap(4, 0, false);

        Assert.NotNull(path);
        Assert.Equal(5, path.Length);
        for (int i = 0; i < 5; i++)
            Assert.Equal((i, 0), (path[i].X, path[i].Y));
        Assert.Empty(map.PathMapArray);            // 原文 PathMapArray := nil
    }

    /// <summary><c>Run=True</c> ⇒ 再经 <c>WalkToRun</c> 合并：直线 5 点收成 2 点（下标 2、4）。</summary>
    [Fact]
    public void FindPathOnMap_RunTrueMergesToRunPath()
    {
        var map = new RecordingMap { Width = 5, Height = 1 };
        map.StartFind = true;
        map.PathMapArray = map.CallFillPathMap(0, 0, 4, 0, false);

        var run = map.FindPathOnMap(4, 0, true);

        Assert.NotNull(run);
        Assert.Equal(2, run.Length);
        Assert.Equal(2, run[0].X);
        Assert.Equal(4, run[1].X);
    }

    /// <summary>目标越界 / <c>PathMapArray</c> 为空 / 目标不可达：三种早退都返回 <c>nil</c> 并清空图。</summary>
    [Fact]
    public void FindPathOnMap_EarlyExits()
    {
        var map = new RecordingMap { Width = 5, Height = 1 };
        map.StartFind = true;
        map.PathMapArray = map.CallFillPathMap(0, 0, 4, 0, false);

        Assert.Null(map.FindPathOnMap(5, 0, false));       // 列越界
        Assert.Empty(map.PathMapArray);

        map.PathMapArray = Array.Empty<TPathMapCell[]>();
        Assert.Null(map.FindPathOnMap(0, 0, false));

        var blocked = new ObstacleMap { Width = 5, Height = 1, WallX = 2 };
        blocked.StartFind = true;
        blocked.PathMapArray = blocked.CallFillPathMap(0, 0, 4, 0, false);
        Assert.Null(blocked.FindPathOnMap(4, 0, false));   // 不可达
        Assert.Empty(blocked.PathMapArray);
    }

    /// <summary>
    /// 差异断言（原文缺陷 ③）：回溯循环中途 <c>Break</c>（<c>StartFind = False</c>）后
    /// <c>Result[1..]</c> 仍是 <c>null</c>，紧接着的整表回填会崩（原文 AV）。
    /// </summary>
    [Fact]
    public void FindPathOnMap_BreakMidwayCrashes()
    {
        var map = new RecordingMap { Width = 5, Height = 1 };
        map.StartFind = true;
        map.PathMapArray = map.CallFillPathMap(0, 0, 4, 0, false);

        map.StartFind = false;
        Assert.Throws<NullReferenceException>(() => map.FindPathOnMap(4, 0, false));
    }

    // ----------------------------------------------------------- WalkToRun

    /// <summary>
    /// <c>WalkToRun</c> 是**纯函数**：返回新数组、不改入参（原文 `WalkPath[I] := Path[I]` 是记录值拷贝，
    /// 托管侧 <c>PathPoint</c> 是 class ⇒ 必须显式 new，否则原地标记 -1 会改坏入参）。
    /// </summary>
    [Fact]
    public void WalkToRun_IsPureAndDropsStartPoint()
    {
        var map = new TPathMap { StartFind = true };
        var input = StraightLine(5);

        var run = map.WalkToRun(input);

        for (int i = 0; i < input.Length; i++)      // 入参一个坐标都不许变
            Assert.Equal((i, 0), (input[i].X, input[i].Y));

        Assert.Equal(2, run.Length);
        Assert.Equal(2, run[0].X);
        Assert.Equal(4, run[1].X);
        Assert.NotSame(input[2], run[0]);
    }

    /// <summary>转向不合并（对照组）：<c>(0,0)→(1,0)→(1,1)</c> ⇒ 结果 2 点。</summary>
    [Fact]
    public void WalkToRun_TurnIsNotMerged()
    {
        var map = new TPathMap { StartFind = true };
        var run = map.WalkToRun(new[]
        {
            new PathPoint(0, 0), new PathPoint(1, 0), new PathPoint(1, 1),
        });

        Assert.Equal(2, run.Length);
        Assert.Equal((1, 0), (run[0].X, run[0].Y));
        Assert.Equal((1, 1), (run[1].X, run[1].Y));
    }

    /// <summary>普通支 7 点直线 ⇒ 标记 1,3,5 ⇒ 收 2、4、6。</summary>
    [Fact]
    public void WalkToRun_SevenPointLineMergesEveryOtherPoint()
    {
        var map = new TPathMap { StartFind = true };
        var run = map.WalkToRun(StraightLine(7));
        Assert.Equal(new[] { 2, 4, 6 }, run.Select(p => p.X).ToArray());
    }

    /// <summary><c>StartFind = False</c> ⇒ 一个点都不标记 ⇒ 收满 <c>Length-1</c>。</summary>
    [Fact]
    public void WalkToRun_StartFindFalseSkipsMerging()
    {
        var map = new TPathMap { StartFind = false };
        var run = map.WalkToRun(StraightLine(5));
        Assert.Equal(new[] { 1, 2, 3, 4 }, run.Select(p => p.X).ToArray());
    }

    /// <summary>
    /// <c>nil</c> / 空 / 单点 三种输入：单点得到**长度为 0 的非 null 数组**，
    /// <c>nil</c> 与空数组得到 <c>nil</c>（原文两支的 <c>SetLength(Result, 0)</c> 后再 <c>Result := nil</c>）。
    /// </summary>
    [Fact]
    public void WalkToRun_NullEmptyAndSinglePoint()
    {
        var map = new TPathMap();

        Assert.Null(map.WalkToRun(null));
        Assert.Null(map.WalkToRun(Array.Empty<PathPoint>()));
        var single = map.WalkToRun(new[] { new PathPoint(7, 7) });
        Assert.NotNull(single);
        Assert.Empty(single);
    }

    /// <summary>
    /// 差异断言：收集条件 `(x &lt;&gt; -1) and (y &lt;&gt; -1)` —— 任一坐标为 -1 即被丢弃
    /// （与 PathFindClient 版同一写法，缺陷 ⑥）。
    /// </summary>
    [Fact]
    public void WalkToRun_CollectionConditionIsAnd()
    {
        var map = new TPathMap { StartFind = false };
        var run = map.WalkToRun(new[]
        {
            new PathPoint(0, 0),
            new PathPoint(-1, 5),
            new PathPoint(5, -1),
            new PathPoint(1, 1),
        });

        Assert.Single(run);
        Assert.Equal((1, 1), (run[0].X, run[0].Y));
    }

    /// <summary>
    /// <c>GetNextDirection</c> 的第一条改写（<c>abs(sy-dy) &gt; 2</c> ⇒ flagx := 0）：
    /// <c>(1,0)→(0,3)</c> 得"下"（4）而非"左下"（5），从而与 <c>(0,3)→(0,4)</c> 合并。
    /// </summary>
    [Fact]
    public void GetNextDirection_FlagxOverrideAllowsMerge()
    {
        var map = new TPathMap { StartFind = true };
        var run = map.WalkToRun(new[]
        {
            new PathPoint(1, 0), new PathPoint(0, 3), new PathPoint(0, 4),
        });

        Assert.Single(run);
        Assert.Equal((0, 4), (run[0].X, run[0].Y));
    }

    /// <summary>
    /// <c>GetNextDirection</c> 的第二条改写（<c>abs(sx-dx) &gt; 2</c> ⇒ flagy := 0）：
    /// <c>(0,1)→(3,0)</c> 得"右"（2）而非"右上"（1），从而与 <c>(3,0)→(4,0)</c> 合并。
    /// </summary>
    [Fact]
    public void GetNextDirection_FlagyOverrideAllowsMerge()
    {
        var map = new TPathMap { StartFind = true };
        var run = map.WalkToRun(new[]
        {
            new PathPoint(0, 1), new PathPoint(3, 0), new PathPoint(4, 0),
        });

        Assert.Single(run);
        Assert.Equal((4, 0), (run[0].X, run[0].Y));
    }

    // ----------------------------------------------------------- TFindPath

    /// <summary><c>TFindPath.Create</c>：<c>FEnvir</c>/<c>FBaseObject</c> 为 <c>nil</c>、<c>StartFind = False</c>、临界区就位。</summary>
    [Fact]
    public void FindPath_CreateInitialises()
    {
        using var env = new Env();
        using var finder = new TFindPath();

        Assert.Null(finder.Envir);
        Assert.Null(finder.BaseObject);
        Assert.False(finder.StartFind);
        Assert.Equal(96, finder.ScopeValue);
        Assert.Equal(0, finder.BeginX);
        Assert.Equal(0, finder.EndX);
        Assert.Equal("", finder.Title);
    }

    /// <summary><c>Envir</c>/<c>BaseObject</c> 是可读写属性（原文 <c>read FEnvir write FEnvir</c>）。</summary>
    [Fact]
    public void FindPath_EnvirAndBaseObjectProperties()
    {
        using var env = new Env();
        using var finder = new TFindPath();
        var envir = new TEnvirnoment { nWidth = 3, nHeight = 3 };

        finder.Envir = envir;
        Assert.Same(envir, finder.Envir);
        finder.BaseObject = null;
        Assert.Null(finder.BaseObject);
    }

    /// <summary><c>Stop</c>：清空起止点与图；<c>g_MultiThreadRun</c> 开/关两条路径都不能抛。</summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void FindPath_StopClearsState(bool multiThread)
    {
        using var env = new Env();
        PathFindHeroSeam.MultiThreadRun = () => multiThread;
        using var finder = new TFindPath { BeginX = 1, BeginY = 2, EndX = 3, EndY = 4, StartFind = true };
        finder.PathMapArray = new[] { new TPathMapCell[1] };

        finder.Stop();

        Assert.False(finder.StartFind);
        Assert.Equal(-1, finder.BeginX);
        Assert.Equal(-1, finder.BeginY);
        Assert.Equal(-1, finder.EndX);
        Assert.Equal(-1, finder.EndY);
        Assert.Empty(finder.PathMapArray);
    }

    /// <summary><c>Envir = nil</c> ⇒ 六参 <c>FindPath</c> 直接返回 <c>nil</c>（且不建图）。</summary>
    [Fact]
    public void FindPath_NullEnvirReturnsNull()
    {
        using var env = new Env();
        using var finder = new TFindPath();

        Assert.Null(finder.FindPath(null, 0, 0, 4, 0, false, false));
        Assert.False(finder.StartFind);
    }

    /// <summary>
    /// 完整链路：<c>FindPath(Envir, StartX.., Run: Boolean, boFlag)</c> 会把
    /// <c>Width/Height</c> 从 <c>Envir.nWidth/nHeight</c> 抄过来、置 <c>StartFind := True</c>、
    /// 建图并回溯；<c>boFlag</c> 一路传到 <c>GetCost</c>。
    /// </summary>
    [Fact]
    public void FindPath_WithEnvirBuildsAndReturnsPath()
    {
        using var env = new Env();
        PathFindHeroSeam.CanWalkExObject = (_, _, _, _, _) => true;
        var envir = new TEnvirnoment { nWidth = 5, nHeight = 1 };
        using var finder = new TFindPath();

        var path = finder.FindPath(envir, 0, 0, 4, 0, false, false);

        Assert.NotNull(path);
        Assert.Equal(5, path.Length);
        Assert.Same(envir, finder.Envir);
        Assert.Equal(5, finder.Width);           // 从 Envir.nWidth 抄来
        Assert.Equal(1, finder.Height);          // 从 Envir.nHeight 抄来
        Assert.Equal(0, finder.BeginX);
        Assert.Equal(4, finder.EndX);
        Assert.Equal(0, finder.EndY);
        Assert.Empty(finder.PathMapArray);        // FindPathOnMap 用完即释放
    }

    /// <summary><c>Run = True</c> ⇒ 结果再经 <c>WalkToRun</c> 合并。</summary>
    [Fact]
    public void FindPath_WithEnvirRunTrueReturnsRunPath()
    {
        using var env = new Env();
        PathFindHeroSeam.CanWalkExObject = (_, _, _, _, _) => true;
        var envir = new TEnvirnoment { nWidth = 5, nHeight = 1 };
        using var finder = new TFindPath();

        var run = finder.FindPath(envir, 0, 0, 4, 0, true, false);

        Assert.NotNull(run);
        Assert.Equal(2, run.Length);
        Assert.Equal(new[] { 2, 4 }, run.Select(p => p.X).ToArray());
    }

    /// <summary>
    /// <c>FindPath(..., boFlag): Boolean</c> 这一重载**自己不拿锁**、转调 6 参版并只回"路径非空"；
    /// 它强制 <c>Run = False</c>。
    /// </summary>
    [Fact]
    public void FindPath_BooleanOverloadReturnsWhetherPathExists()
    {
        using var env = new Env();
        PathFindHeroSeam.CanWalkExObject = (_, _, _, _, _) => true;
        var envir = new TEnvirnoment { nWidth = 5, nHeight = 1 };
        using var finder = new TFindPath();

        Assert.True(finder.FindPath(envir, 0, 0, 4, 0, false));
        Assert.Equal(4, finder.EndX);

        // 不可走 ⇒ 假
        PathFindHeroSeam.CanWalkExObject = (_, _, _, _, _) => false;
        Assert.False(finder.FindPath(envir, 0, 0, 4, 0, false));

        // Envir = nil ⇒ 假（内层返回 nil）
        Assert.False(finder.FindPath(null, 0, 0, 4, 0, false));
    }

    /// <summary>
    /// 两参 <c>FindPath(StopX, StopY, Run, boFlag)</c> **不重建图**：沿用上一次的 <c>PathMapArray</c>，
    /// 只更新 <c>EndX/EndY</c>。故先用 <c>SetStartPos</c> 建图、再用两参版取路径。
    /// <para>⚠ 本用例同时钉死两条原文硬约束：① <c>SetStartPos</c> 自己**不置 <c>StartFind</c>**（缺陷 ⑧）；
    /// ② <c>GetCost</c> 在 <c>FEnvir = nil</c> 时**无条件返回 -1**（原文 <c>:682-684</c>），
    /// 故必须先把 <c>Envir</c> 设好，否则图里除起点外全是 -1。</para>
    /// </summary>
    [Fact]
    public void FindPath_TwoArgOverloadReusesExistingMap()
    {
        using var env = new Env();
        PathFindHeroSeam.CanWalkExObject = (_, _, _, _, _) => true;
        using var finder = new TFindPath
        {
            Width = 5,
            Height = 1,
            StartFind = true,
            Envir = new TEnvirnoment { nWidth = 5, nHeight = 1 },
        };

        // SetStartPos 建图（StartX,StartY 为起点、终点 -1,-1 ⇒ 跑满全图）
        finder.SetStartPos(0, 0);
        Assert.Single(finder.PathMapArray);
        Assert.Equal(0, finder.BeginX);

        var path = finder.FindPath(4, 0, false, false);

        Assert.NotNull(path);
        Assert.Equal(5, path.Length);
        Assert.Equal(4, finder.EndX);
        Assert.Equal(0, finder.EndY);
        Assert.Empty(finder.PathMapArray);       // FindPathOnMap 用完后释放
    }

    /// <summary>
    /// 差异断言（原文缺陷 ⑨）：<c>TFindPath.GetCost</c> 在 <c>FEnvir = nil</c> 时**直接返回 -1**
    /// ⇒ 此时 <c>SetStartPos</c> 建出的图里除起点格（<c>Distance = 0</c>）外全是 -1，
    /// 且 <c>CanWalkEx</c> 一族接缝**一次都不会被调用**（计数取证）。
    /// </summary>
    [Fact]
    public void FindPath_SetStartPosWithoutEnvirConsultsNothing()
    {
        using var env = new Env();
        int calls = 0;
        PathFindHeroSeam.CanWalkExObject = (_, _, _, _, _) =>
        {
            calls++;
            return true;
        };
        using var finder = new TFindPath { Width = 5, Height = 1, StartFind = true };

        finder.SetStartPos(0, 0);

        Assert.Equal(0, calls);                                     // 一次都没问
        Assert.Equal(0, finder.PathMapArray[0][0].Distance);
        for (int x = 1; x < 5; x++)
            Assert.Equal(-1, finder.PathMapArray[0][x].Distance);
    }

    /// <summary>
    /// 差异断言（原文缺陷 ⑧）：<c>SetStartPos</c> 前若 <c>StartFind = False</c>，
    /// <c>FillPathMap</c> 的主循环第一轮就 <c>Break</c> ⇒ 图里只有起点格（<c>Distance = 0</c>），
    /// 其余格子全是 -1 ⇒ 之后对远处目标取路径必然返回 <c>nil</c>（对起点自己则能拿到 1 点路径）。
    /// </summary>
    [Fact]
    public void FindPath_SetStartPosWithoutStartFindLeavesOnlyStartCell()
    {
        using var env = new Env();
        PathFindHeroSeam.CanWalkExObject = (_, _, _, _, _) => true;
        using var finder = new TFindPath
        {
            Width = 5,
            Height = 1,
            StartFind = false,
            Envir = new TEnvirnoment { nWidth = 5, nHeight = 1 },
        };

        finder.SetStartPos(0, 0);

        Assert.Equal(5, finder.PathMapArray[0].Length);
        Assert.Equal(0, finder.PathMapArray[0][0].Distance);
        for (int x = 1; x < 5; x++)
            Assert.Equal(-1, finder.PathMapArray[0][x].Distance);   // 未被扩散到

        Assert.Null(finder.FindPath(4, 0, false, false));           // 远处不可达
    }

    /// <summary>
    /// <c>SetStartPos</c> 只写起点与图（<c>boFlag</c> 硬编码 <c>False</c>、不设 <c>StartFind</c>）。
    /// <para>必须先设 <c>Envir</c>，否则 <c>GetCost</c> 直接返回 -1、一次接缝都不会被问到
    /// （见 <c>FindPath_SetStartPosWithoutEnvirConsultsNothing</c>）。</para>
    /// </summary>
    [Fact]
    public void FindPath_SetStartPosFillsMapWithBoFlagFalse()
    {
        using var env = new Env();
        var seenFlags = new List<bool>();
        PathFindHeroSeam.CanWalkExObject = (_, _, _, _, flag) =>
        {
            seenFlags.Add(flag);
            return true;
        };
        using var finder = new TFindPath
        {
            Width = 3,
            Height = 1,
            StartFind = true,
            Envir = new TEnvirnoment { nWidth = 3, nHeight = 1 },
        };

        finder.SetStartPos(0, 0);

        Assert.Equal(0, finder.BeginX);
        Assert.Equal(0, finder.BeginY);
        Assert.Equal(3, finder.PathMapArray[0].Length);
        Assert.NotEmpty(seenFlags);
        Assert.All(seenFlags, f => Assert.False(f));     // boFlag 传的是 False
    }

    // ------------------------------------------------------ TFindPath.GetCost

    /// <summary><c>FEnvir = nil</c> ⇒ <c>GetCost</c> 无条件 -1（原文 <c>else Result := -1</c>）。</summary>
    [Fact]
    public void GetCost_NullEnvirReturnsMinusOne()
    {
        using var env = new Env();
        using var finder = new ProbeFindPath { Width = 3, Height = 3 };
        finder.GetClientRect();

        Assert.Equal(-1, finder.CallGetCost(1, 1, 0, false));
        Assert.Equal(-1, finder.CallGetCost(1, 1, 0, true));
    }

    /// <summary>
    /// <c>boFlag = True</c> ⇒ 走 <c>FEnvir.CanWalkEx(nX, nY, boFlag)</c> 那一支：
    /// 可走 4、否则 -1；斜方向（奇数）且为正时 ×1.5。
    /// </summary>
    [Fact]
    public void GetCost_BoFlagTrueUsesFirstOverloadAndAppliesDiagonalMarkup()
    {
        using var env = new Env();
        var calls = new List<(int x, int y, bool boFlag)>();
        PathFindHeroSeam.CanWalkEx = (_, x, y, boFlag) =>
        {
            calls.Add((x, y, boFlag));
            return x == 1;
        };
        using var finder = new ProbeFindPath { Width = 3, Height = 3 };
        finder.Envir = new TEnvirnoment { nWidth = 3, nHeight = 3 };
        finder.GetClientRect();

        Assert.Equal(4, finder.CallGetCost(1, 0, 0, true));     // 直方向
        Assert.Equal(-1, finder.CallGetCost(0, 0, 0, true));    // 不可走
        Assert.Equal(6, finder.CallGetCost(1, 0, 1, true));     // 斜：4 + 2
        Assert.Equal(-1, finder.CallGetCost(0, 0, 1, true));    // 不可走 ⇒ 斜也不放大
        Assert.Equal(-1, finder.CallGetCost(-1, 0, 0, true));   // 越界
        Assert.Equal(-1, finder.CallGetCost(3, 0, 0, true));

        Assert.All(calls, c => Assert.True(c.boFlag));          // 这一支的 boFlag 恒为真
        Assert.NotEmpty(calls);
    }

    /// <summary>
    /// <c>boFlag = False</c> ⇒ 走带"跑动权限"的重载，第四个实参是
    /// <c>boDiableHumanRun or (permission &gt; 9 and boGMRunAll)</c>；实测四种配置组合。
    /// </summary>
    [Theory]
    [InlineData(false, 0, false, false)]     // 全关 ⇒ False
    [InlineData(true, 0, false, true)]       // boDiableHumanRun ⇒ True
    [InlineData(false, 10, true, true)]      // 权限 > 9 且 boGMRunAll ⇒ True
    [InlineData(false, 10, false, false)]    // 权限够但开关关 ⇒ False
    [InlineData(false, 9, true, false)]      // 权限恰好 9 ⇒ False（严格大于）
    public void GetCost_BoFlagFalseComputesRunFlag(bool diable, int permission, bool gmRunAll, bool expected)
    {
        using var env = new Env();
        M2Config.boDiableHumanRun = diable;
        M2Config.boGMRunAll = gmRunAll;
        PathFindHeroSeam.GetPermission = _ => permission;
        bool? seen = null;
        PathFindHeroSeam.CanWalkExObject = (_, _, _, _, flag) =>
        {
            seen = flag;
            return true;
        };
        using var finder = new ProbeFindPath { Width = 3, Height = 3 };
        finder.Envir = new TEnvirnoment { nWidth = 3, nHeight = 3 };
        finder.GetClientRect();

        Assert.Equal(4, finder.CallGetCost(1, 0, 0, false));
        Assert.NotNull(seen);
        Assert.Equal(expected, seen!.Value);
    }

    /// <summary>
    /// <c>boFlag = False</c> 那一支还**或上** <c>boSafeAreaLimited and InSafeZone</c>：
    /// 即使 <c>CanWalkEx</c> 为假，安全区条件成立时也算可走；且 <c>boSafeAreaLimited</c> 为假时
    /// **短路**、根本不查 <c>InSafeZone</c>（计数取证）。
    /// </summary>
    [Fact]
    public void GetCost_BoFlagFalseOrsSafeAreaAndShortCircuits()
    {
        using var env = new Env();
        PathFindHeroSeam.CanWalkExObject = (_, _, _, _, _) => false;
        int safeCalls = 0;
        PathFindHeroSeam.InSafeZone = _ =>
        {
            safeCalls++;
            return true;
        };
        using var finder = new ProbeFindPath { Width = 3, Height = 3 };
        finder.Envir = new TEnvirnoment { nWidth = 3, nHeight = 3 };
        finder.GetClientRect();

        // 开关关 ⇒ 短路，InSafeZone 一次都不查
        M2Config.boSafeAreaLimited = false;
        Assert.Equal(-1, finder.CallGetCost(1, 0, 0, false));
        Assert.Equal(0, safeCalls);

        // 开关开 ⇒ 查一次，且结果为真 ⇒ 4
        M2Config.boSafeAreaLimited = true;
        Assert.Equal(4, finder.CallGetCost(1, 0, 0, false));
        Assert.Equal(1, safeCalls);

        // 安全区为假 ⇒ 仍 -1
        PathFindHeroSeam.InSafeZone = _ => false;
        Assert.Equal(-1, finder.CallGetCost(1, 0, 0, false));

        // boFlag = True 那一支**不**看安全区
        PathFindHeroSeam.CanWalkEx = (_, _, _, _) => false;
        Assert.Equal(-1, finder.CallGetCost(1, 0, 0, true));
    }

    /// <summary><c>GetCost</c> 先把本地坐标经 <c>MapX/MapY</c> 平移后再问接缝（非零原点也要平移）。</summary>
    [Fact]
    public void GetCost_TranslatesThroughMapXMapY()
    {
        using var env = new Env();
        var seen = new List<(int x, int y)>();
        PathFindHeroSeam.CanWalkEx = (_, x, y, _) =>
        {
            seen.Add((x, y));
            return true;
        };
        using var finder = new ProbeFindPath { Width = 3, Height = 3 };
        finder.Envir = new TEnvirnoment { nWidth = 3, nHeight = 3 };
        finder.ClientRect = new Rectangle(10, 20, 3, 3);      // 直接设 ClientRect（字段公开）

        Assert.Equal(4, finder.CallGetCost(1, 1, 0, true));
        var only = Assert.Single(seen);
        Assert.Equal((11, 21), only);
    }

    /// <summary><c>Direction and 7</c> 归约后再判奇偶（斜方向标记）。</summary>
    [Fact]
    public void GetCost_NormalisesDirectionToThreeBits()
    {
        using var env = new Env();
        PathFindHeroSeam.CanWalkEx = (_, _, _, _) => true;
        using var finder = new ProbeFindPath { Width = 3, Height = 3 };
        finder.Envir = new TEnvirnoment { nWidth = 3, nHeight = 3 };
        finder.GetClientRect();

        Assert.Equal(6, finder.CallGetCost(1, 0, 9, true));    // 9 and 7 = 1 ⇒ 斜
        Assert.Equal(4, finder.CallGetCost(1, 0, 8, true));    // 8 and 7 = 0 ⇒ 直
        Assert.Equal(6, finder.CallGetCost(1, 0, 15, true));   // 15 and 7 = 7 ⇒ 斜
    }

    // --------------------------------------------------------------- 接缝

    /// <summary>
    /// 差异断言：<c>MultiThreadRun</c> 的默认实现**转发**到既有全局
    /// <c>Forms.CustomMagic.CustomMagicFormGlobals.g_MultiThreadRun</c>（不另造第二份存储）。
    /// </summary>
    [Fact]
    public void SeamMultiThreadRunForwardsToExistingGlobal()
    {
        using var env = new Env();
        var saved = GXX.M2Server.Forms.CustomMagic.CustomMagicFormGlobals.g_MultiThreadRun;
        try
        {
            GXX.M2Server.Forms.CustomMagic.CustomMagicFormGlobals.g_MultiThreadRun = true;
            Assert.True(PathFindHeroSeam.MultiThreadRun());

            GXX.M2Server.Forms.CustomMagic.CustomMagicFormGlobals.g_MultiThreadRun = false;
            Assert.False(PathFindHeroSeam.MultiThreadRun());
        }
        finally
        {
            GXX.M2Server.Forms.CustomMagic.CustomMagicFormGlobals.g_MultiThreadRun = saved;
        }
    }

    /// <summary>其余接缝默认值：可走判定一律假、权限 0（<c>null</c> 对象）。</summary>
    [Fact]
    public void SeamDefaultsAreSafeEmptyState()
    {
        using var env = new Env();
        var envir = new TEnvirnoment();

        Assert.False(PathFindHeroSeam.CanWalkEx(envir, 0, 0, false));
        Assert.False(PathFindHeroSeam.CanWalkExObject(envir, null, 0, 0, false));
        Assert.False(PathFindHeroSeam.InSafeZone(null));
        Assert.Equal(0, PathFindHeroSeam.GetPermission(null));

        PathFindHeroSeam.CanWalkEx = (_, _, _, _) => true;
        PathFindHeroSeam.ResetDefaults();
        Assert.False(PathFindHeroSeam.CanWalkEx(envir, 0, 0, false));
    }

    /// <summary>公开面清点（否定性取证，台账 §37.3）：<c>FindPath</c> 三重载 + <c>Stop</c>/<c>SetStartPos</c> 等。</summary>
    [Fact]
    public void PublicSurfaceMatchesOriginalInterfaceSection()
    {
        var mapMembers = new[]
        {
            "PathMapArray", "Height", "Width", "GetCostFunc", "ClientRect", "ScopeValue", "StartFind",
            "GetClientRect", "FindPathOnMap", "WalkToRun", "MapX", "MapY", "LoaclX", "LoaclY",
        };
        var mapType = typeof(TPathMap);
        foreach (var name in mapMembers)
            Assert.Contains(mapType.GetMembers(), m => m.Name == name);
        Assert.Equal(14, mapMembers.Length);

        var findType = typeof(TFindPath);
        foreach (var name in new[] { "Title", "BeginX", "BeginY", "EndX", "EndY", "Envir", "BaseObject", "Stop", "SetStartPos" })
            Assert.Contains(findType.GetMembers(), m => m.Name == name);
        Assert.Equal(3, findType.GetMethods().Count(m => m.Name == "FindPath"));
        Assert.Equal(2, findType.GetMethods().Count(m => m.Name == "Dispose" || m.Name == "Stop"));

        var waveType = typeof(TWave);
        foreach (var name in new[] { "Item", "MinCost", "Add", "Clear", "Start", "Next", "Destroy" })
            Assert.Contains(waveType.GetMembers(), m => m.Name == name);
    }
}
