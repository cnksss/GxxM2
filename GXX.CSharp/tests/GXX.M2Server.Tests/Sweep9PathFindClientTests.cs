// ============================================================================
// 测试：Source/M2Engine/PathFindClient.pas → GXX.M2Server.Sweep9.PathFind.Client（1:1）
//
// 覆盖策略（任务书第 5 条）：每个公开成员 ≥1 例；分支/边界/原文缺陷各配差异断言。
// * 私有成员（`DirToDX`/`DirToDY`/`PreparePathMap`/`TestNeighbours`/`ExchangeWaves`/`GetNextDirection`）
//   不直接可调，本文件一律用**可观察出口**取证：
//   - `DirToDX/DirToDY` → 用"记录 (x,y,方向) 的 GetCostFunc"反推 8 个方向的偏移（见 `Directions_*`）；
//   - `PreparePathMap`/`TestNeighbours`/`ExchangeWaves` → 通过 `FillPathMap` 的结果取证；
//   - `GetNextDirection` → 通过 `WalkToRun` 的"是否合并"取证。
// * 全部否定性断言都配计数/实测取证（台账 §37.3）。
// * 接缝（`PathFindClientSeam`）由夹具显式设定并还原，与外部残留状态解耦。
// * ⚠ 凡起 `TFindPathThread` 的用例，都必须先把接缝 `LegendMap` 换成**带尺寸**的实例：
//   0×0 的地图在 `FillPathMap` 里会越界（原文是 AV），而托管侧的未捕获异常会**直接终止测试进程**。
// ============================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using GXX.M2Server.Sweep9.PathFind.Client;
using PathPoint = GXX.Core.Util.PathPoint;
using TPathMapCell = GXX.Core.Util.TPathMapCell;
using Xunit;

namespace GXX.M2Server.Tests;

public class Sweep9PathFindClientTests
{
    // ------------------------------------------------------------------ 夹具

    /// <summary>保存并还原 <see cref="PathFindClientSeam"/> 的全部接缝。</summary>
    private sealed class Env : IDisposable
    {
        private readonly TLegendMap _legend = PathFindClientSeam.LegendMap;
        private readonly Func<int> _horse = PathFindClientSeam.MySelfHorse;
        private readonly Func<bool> _horse3 = PathFindClientSeam.ClientConfigHorseRun3Grid;
        private readonly Func<int, int, bool> _walk = PathFindClientSeam.NewCanWalkEx;
        private readonly Func<int, int, bool> _walk2 = PathFindClientSeam.NewCanWalkEx_2;

        public Env()
        {
            PathFindClientSeam.ResetDefaults();
        }

        public void Dispose()
        {
            PathFindClientSeam.LegendMap = _legend;
            PathFindClientSeam.MySelfHorse = _horse;
            PathFindClientSeam.ClientConfigHorseRun3Grid = _horse3;
            PathFindClientSeam.NewCanWalkEx = _walk;
            PathFindClientSeam.NewCanWalkEx_2 = _walk2;
        }
    }

    /// <summary>把 <c>protected</c> 成员暴露给用例的探针子类。</summary>
    private sealed class ProbeMap : TPathMap
    {
        public int CallGetCost(int x, int y, int direction) => GetCost(x, y, direction);

        public TPathMapCell[][] CallFillPathMap(int x1, int y1, int x2, int y2) => FillPathMap(x1, y1, x2, y2);

        public PathPoint[] FPathProbe
        {
            get => FPath;
            set => FPath = value;
        }

        public PathPoint[] FRunPathProbe => FRunPath;

        public bool FFillPathMapProbe => FFillPathMap;

        public bool FFindPathOnMapProbe => FFindPathOnMap;
    }

    private sealed class ProbeLegend : TLegendMap
    {
        public int CallGetCost(int x, int y, int direction) => GetCost(x, y, direction);
    }

    /// <summary>造一条横线路径（y 全为 0，x = 0..n-1）。</summary>
    private static PathPoint[] StraightLine(int n)
        => Enumerable.Range(0, n).Select(i => new PathPoint(i, 0)).ToArray();

    /// <summary>等一个后台寻路线程把全局 LegendMap 用起来（有界自旋，避免用例竞态）。</summary>
    private static void WaitForFind(TLegendMap legend, int expectedCount = 1)
    {
        for (int i = 0; i < 5000 && legend.FindCount < expectedCount; i++)
            Thread.Sleep(1);
        Assert.True(legend.FindCount >= expectedCount, "后台寻路线程未在超时内完成");
    }

    // ------------------------------------------------------ 类型 / 常量表

    /// <summary><c>TMapHeader = packed record</c> 的字节布局：2+2+17+8+23 = 52。</summary>
    [Fact]
    public void TMapHeader_Is52Bytes()
    {
        Assert.Equal(52, Marshal.SizeOf<TMapHeader>());
    }

    /// <summary><c>TMapInfo = packed record</c>：2+2+2+6 = 12。</summary>
    [Fact]
    public void TMapInfo_Is12Bytes()
    {
        Assert.Equal(12, Marshal.SizeOf<TMapInfo>());
    }

    /// <summary><c>TTerrainTypes</c> 六个成员、顺序与原文一致。</summary>
    [Fact]
    public void TTerrainTypes_HasSixMembersInSourceOrder()
    {
        var names = Enum.GetNames(typeof(TTerrainTypes));
        Assert.Equal(new[] { "ttNormal", "ttSand", "ttForest", "ttRoad", "ttObstacle", "ttPath" }, names);
        Assert.Equal(0, (int)TTerrainTypes.ttNormal);
        Assert.Equal(5, (int)TTerrainTypes.ttPath);
    }

    /// <summary>
    /// <c>TerrainParams</c> 常量表逐项对照（颜色按 Delphi Graphics.pas 的 $00BBGGRR），
    /// 并与**既有** Core 产物 <c>PathFindConst.TerrainMoveCost</c> 交叉验证 MoveCost 不分叉。
    /// </summary>
    [Fact]
    public void TerrainParams_MatchesOriginalTableAndExistingCore()
    {
        var t = PathFindTerrainParams.TerrainParams;
        Assert.Equal(6, t.Length);

        Assert.Equal((0xFFFFFF, "平地", 4), (t[0].CellColor, t[0].CellLabel, t[0].MoveCost));
        Assert.Equal((0x008080, "沙地", 6), (t[1].CellColor, t[1].CellLabel, t[1].MoveCost));
        Assert.Equal((0x008000, "树林", 10), (t[2].CellColor, t[2].CellLabel, t[2].MoveCost));
        Assert.Equal((0x00C0C0, "马路", 2), (t[3].CellColor, t[3].CellLabel, t[3].MoveCost));
        Assert.Equal((0x000000, "障碍物", -1), (t[4].CellColor, t[4].CellLabel, t[4].MoveCost));
        Assert.Equal((0x0000FF, "路径", 0), (t[5].CellColor, t[5].CellLabel, t[5].MoveCost));

        Assert.Equal(t[2], PathFindTerrainParams.Get(TTerrainTypes.ttForest));
        Assert.True(PathFindTerrainParams.MoveCostsMatchExistingCore());
    }

    // ------------------------------------------- ctor / 属性 / 坐标换算

    /// <summary><c>Create</c>：<c>ScopeValue := 120</c>、<c>GetCostFunc := nil</c>，其余字段取默认。</summary>
    [Fact]
    public void Create_SetsScopeValue120AndNullGetCostFunc()
    {
        var map = new TPathMap();
        Assert.Equal(120, map.ScopeValue);
        Assert.Null(map.GetCostFunc);
        Assert.False(map.StartFind);
        Assert.Equal(0, map.PathWidth);
        Assert.Equal(0, map.Width);
        Assert.Equal(0, map.Height);
        Assert.NotNull(map.PathMapArray);
        Assert.Empty(map.PathMapArray);
        Assert.Equal(new Rectangle(0, 0, 0, 0), map.ClientRect);
        Assert.Null(map.Path);
        Assert.Null(map.RunPath);
    }

    /// <summary><c>Width</c>/<c>Height</c> 属性读写（写侧即 <c>SetWidth/SetHeight</c>）。</summary>
    [Fact]
    public void WidthHeight_PropertiesRoundTrip()
    {
        var map = new TPathMap();
        map.Width = 7;
        map.Height = 9;
        Assert.Equal(7, map.Width);
        Assert.Equal(9, map.Height);

        map.Width = 7;                       // 同值再写一次（原文 SetWidth 里 `if FWidth <> Value` 短路）
        Assert.Equal(7, map.Width);
    }

    /// <summary>
    /// <c>GetClientRect(X1,Y1,X2,Y2)</c> **完全忽略四个入参**（原文缺陷 ⑦），
    /// 结果恒为 <c>Bounds(0,0,FWidth,FHeight)</c>。
    /// </summary>
    [Fact]
    public void GetClientRect_IgnoresArguments()
    {
        var map = new TPathMap { Width = 5, Height = 3 };
        map.GetClientRect(100, 200, 300, 400);
        Assert.Equal(new Rectangle(0, 0, 5, 3), map.ClientRect);
        Assert.Equal(5, map.ClientRect.Right - map.ClientRect.Left);
        Assert.Equal(3, map.ClientRect.Bottom - map.ClientRect.Top);

        map.GetClientRect(-1, -1, -1, -1);
        Assert.Equal(new Rectangle(0, 0, 5, 3), map.ClientRect);
    }

    /// <summary><c>MapX/MapY/LoaclX/LoaclY</c> 全部以 <c>ClientRect.Left/Top</c> 为基准（含非零原点）。</summary>
    [Fact]
    public void MapAndLocalConversions_UseClientRectOrigin()
    {
        var map = new TPathMap { Width = 5, Height = 5 };
        map.GetClientRect(0, 0, 0, 0);
        Assert.Equal(3, map.MapX(3));
        Assert.Equal(4, map.MapY(4));
        Assert.Equal(3, map.LoaclX(3));
        Assert.Equal(4, map.LoaclY(4));

        // 直接改 ClientRect（字段公开）后换算随之平移
        map.ClientRect = new Rectangle(10, 20, 5, 5);
        Assert.Equal(13, map.MapX(3));
        Assert.Equal(24, map.MapY(4));
        Assert.Equal(-7, map.LoaclX(3));
        Assert.Equal(-16, map.LoaclY(4));
    }

    /// <summary><c>Path</c>/<c>RunPath</c> 是可读写属性（原文 <c>read FPath write FPath</c>）。</summary>
    [Fact]
    public void PathAndRunPath_PropertiesAreReadWrite()
    {
        var map = new TPathMap();
        var p = StraightLine(3);
        map.Path = p;
        map.RunPath = p;
        Assert.Same(p, map.Path);
        Assert.Same(p, map.RunPath);
        map.Path = null;
        Assert.Null(map.Path);
    }

    // ---------------------------------------------------- GetCost（基类）

    /// <summary><c>GetCostFunc = nil</c> ⇒ 界内也返回 -1（原文 <c>else Result := -1</c>）。</summary>
    [Fact]
    public void BaseGetCost_NullFuncReturnsMinusOne()
    {
        var map = new ProbeMap { Width = 3, Height = 3 };
        map.GetClientRect(0, 0, 0, 0);
        Assert.Equal(-1, map.CallGetCost(1, 1, 0));
    }

    /// <summary>
    /// 越界（或负坐标）**先**返回 -1，**不会**调用 <c>GetCostFunc</c>；
    /// 界内才转发，且第 4 参恒为 <c>PathWidth</c>。
    /// </summary>
    [Fact]
    public void BaseGetCost_BoundsCheckedBeforeCallbackAndForwardsPathWidth()
    {
        var seen = new List<(int x, int y, int dir, int pw)>();
        var map = new ProbeMap { Width = 3, Height = 3, PathWidth = 7 };
        map.GetClientRect(0, 0, 0, 0);
        map.GetCostFunc = (x, y, d, pw) =>
        {
            seen.Add((x, y, d, pw));
            return 42;
        };

        Assert.Equal(-1, map.CallGetCost(-1, 0, 0));
        Assert.Equal(-1, map.CallGetCost(0, -1, 0));
        Assert.Equal(-1, map.CallGetCost(3, 0, 0));
        Assert.Equal(-1, map.CallGetCost(0, 3, 0));
        Assert.Empty(seen);                          // 计数取证：四次越界一次回调都没有

        Assert.Equal(42, map.CallGetCost(1, 1, 9));  // 方向 9 and 7 = 1
        var only = Assert.Single(seen);
        Assert.Equal((1, 1, 1, 7), only);
    }

    // -------------------------------------------------- 方向表 / FillPathMap

    /// <summary>
    /// 差异断言：用"记录 (x,y,方向)"的估价函数从**终点为 -1 的全图扩散**里反推
    /// <c>DirToDX/DirToDY</c> 的 8 项偏移表（这两个函数是 private，只能从行为取证）。
    /// 期望布局（原文注释 <c>7 0 1 / 6 X 2 / 5 4 3</c>）：
    /// <code>
    /// d: 0→(0,-1) 1→(1,-1) 2→(1,0) 3→(1,1) 4→(0,1) 5→(-1,1) 6→(-1,0) 7→(-1,-1)
    /// </code>
    /// </summary>
    [Fact]
    public void Directions_EightOffsetsMatchSourceLayout()
    {
        // 从中心扩散（终点为 -1,-1 ⇒ 不早退、跑满整张图）
        var map = new ProbeMap { Width = 5, Height = 5 };
        map.StartFind = true;
        map.GetCostFunc = (x, y, d, pw) => 1;
        var result = map.CallFillPathMap(2, 2, -1, -1);
        Assert.Equal(5, result.Length);
        Assert.Equal(5, result[0].Length);

        // 只保留**起点格**（2,2）那 8 次回调，用相对位移反推方向表
        var startSeen = new Dictionary<int, (int dx, int dy)>();
        var map2 = new ProbeMap { Width = 5, Height = 5 };
        map2.StartFind = true;
        map2.GetCostFunc = (x, y, d, pw) =>
        {
            if (x >= 1 && x <= 3 && y >= 1 && y <= 3 && !startSeen.ContainsKey(d))
                startSeen[d] = (x - 2, y - 2);
            return 1;
        };
        map2.CallFillPathMap(2, 2, -1, -1);

        Assert.Equal(8, startSeen.Count);            // 计数取证：中心格 8 个方向全部界内
        Assert.Equal((0, -1), startSeen[0]);
        Assert.Equal((1, -1), startSeen[1]);
        Assert.Equal((1, 0), startSeen[2]);
        Assert.Equal((1, 1), startSeen[3]);
        Assert.Equal((0, 1), startSeen[4]);
        Assert.Equal((-1, 1), startSeen[5]);
        Assert.Equal((-1, 0), startSeen[6]);
        Assert.Equal((-1, -1), startSeen[7]);
    }

    /// <summary>
    /// 统一代价 1、1 行的 5 格地图：距离只可能来自横向推进 ⇒
    /// <c>Distance[0][4] = 4</c>，其余格按 |x| 递增；方向一律 2（右）。
    /// </summary>
    [Fact]
    public void FillPathMap_UniformCostAssignsStepDistances()
    {
        var map = new ProbeMap { Width = 5, Height = 1 };
        map.StartFind = true;
        map.GetCostFunc = (x, y, d, pw) => 1;

        var result = map.CallFillPathMap(0, 0, 4, 0);

        Assert.Single(result);
        Assert.Equal(5, result[0].Length);
        for (int x = 0; x < 5; x++)
            Assert.Equal(x, result[0][x].Distance);
        for (int x = 1; x < 5; x++)
            Assert.Equal(2, result[0][x].Direction);
        Assert.False(map.FFillPathMapProbe);
    }

    /// <summary>障碍墙把地图切断：墙右侧的格子 <c>Distance</c> 保持 -1（不可达）。</summary>
    [Fact]
    public void FillPathMap_ObstacleLeavesUnreachableCellsAtMinusOne()
    {
        var map = new ProbeMap { Width = 5, Height = 1 };
        map.StartFind = true;
        map.GetCostFunc = (x, y, d, pw) => x == 2 ? -1 : 1;   // x = 2 是墙

        var result = map.CallFillPathMap(0, 0, 4, 0);

        Assert.Equal(0, result[0][0].Distance);
        Assert.Equal(1, result[0][1].Distance);
        Assert.Equal(-1, result[0][2].Distance);   // 墙自己
        Assert.Equal(-1, result[0][3].Distance);   // 墙右侧不可达
        Assert.Equal(-1, result[0][4].Distance);
    }

    /// <summary>
    /// 差异断言：起终点"跨度过大"时 <c>FillPathMap</c> 直接返回**空交错数组**
    /// （原文 <c>SetLength(Result, 0, 0)</c>），且把 <c>FFillPathMap</c> 复位。
    /// 判据是 <c>abs(nX1-nX2) &gt; 宽度</c>（**严格大于**）。
    /// </summary>
    [Theory]
    [InlineData(4, 5, 1)]     // |0-4| = 4 <= 5 ⇒ 正常建图
    [InlineData(5, 5, 1)]     // 5 <= 5 ⇒ 正常
    [InlineData(6, 5, 0)]     // 6 > 5 ⇒ 空
    [InlineData(100, 5, 0)]
    public void FillPathMap_TooFarTargetReturnsEmptyAndClearsFlag(int stopX, int width, int expectedRows)
    {
        var map = new ProbeMap { Width = width, Height = 1 };
        map.StartFind = true;
        map.GetCostFunc = (x, y, d, pw) => 1;

        var result = map.CallFillPathMap(0, 0, stopX, 0);

        Assert.Equal(expectedRows, result.Length);
        Assert.False(map.FFillPathMapProbe);
    }

    /// <summary>
    /// 终点为负（<c>X2 &lt; 0</c> 或 <c>Y2 &lt; 0</c>）时**跳过**跨度检查，
    /// 且 <c>nX2/nY2</c> 直接取负值 ⇒ 永远到不了终点、跑满整张图。
    /// </summary>
    [Fact]
    public void FillPathMap_NegativeTargetSkipsSpanCheck()
    {
        var map = new ProbeMap { Width = 5, Height = 1 };
        map.StartFind = true;
        map.GetCostFunc = (x, y, d, pw) => 1;

        var result = map.CallFillPathMap(0, 0, -1, -1);
        Assert.Single(result);
        Assert.Equal(4, result[0][4].Distance);      // 全图都被扩散到
    }

    /// <summary>
    /// 差异断言：<c>StartFind = False</c> 时 <c>PreparePathMap</c> 在分配**第一行之前**就返回
    /// ⇒ 结果首行为 <c>null</c>，而 <c>FillPathMap</c> 紧接着写 <c>Result[nY1][nX1]</c>
    /// ⇒ **空引用崩溃**。原文同处是访问违例；此处按原文行为锁定，不做守卫。
    /// </summary>
    [Fact]
    public void FillPathMap_StartFindFalseCrashesLikeOriginal()
    {
        var map = new ProbeMap { Width = 3, Height = 3, StartFind = false };
        map.GetCostFunc = (x, y, d, pw) => 1;

        Assert.Throws<NullReferenceException>(() => map.CallFillPathMap(0, 0, 0, 0));
    }

    // ------------------------------------------------------- FindPathOnMap

    /// <summary>
    /// 端到端：1 行 5 格的直线地图 → <c>FPath</c> 5 点、<c>RunPath</c> 合并成 2 点。
    /// <para>合并轨迹（普通支，nStep &gt;= 2）：nI=2 时比较 (0,1) 与 (1,2) 两点段（同向）⇒ 标记下标 1；
    /// nI=4 时比较 (2,3) 与 (3,4)（同向）⇒ 标记下标 3。收集从下标 1 起、跳过被标记的 ⇒ 收 2、4。</para>
    /// </summary>
    [Fact]
    public void FindPathOnMap_BacktracksAndMergesStraightLine()
    {
        var map = new ProbeMap { Width = 5, Height = 1, StartFind = true };
        map.GetCostFunc = (x, y, d, pw) => 1;
        map.PathMapArray = map.CallFillPathMap(0, 0, 4, 0);

        map.FindPathOnMap(4, 0);

        var path = map.Path;
        Assert.NotNull(path);
        Assert.Equal(5, path.Length);
        for (int i = 0; i < 5; i++)
        {
            Assert.Equal(i, path[i].X);
            Assert.Equal(0, path[i].Y);
        }

        var run = map.RunPath;
        Assert.NotNull(run);
        Assert.Equal(2, run.Length);
        Assert.Equal(2, run[0].X);
        Assert.Equal(4, run[1].X);

        // FindPathOnMap 收尾：两个标志都复位
        Assert.False(map.StartFind);
        Assert.False(map.FFindPathOnMapProbe);
    }

    /// <summary>
    /// 差异断言（原文缺陷 ②）：<c>StartFind = False</c> 时回溯循环立刻 <c>Break</c>，
    /// 但**仍然**执行 <c>FPath[0] := ...</c> 与整表回填 ⇒ <c>FPath[1..]</c> 仍是 <c>null</c> ⇒ 空引用崩溃。
    /// 这正对应原文"寻路中途被 <c>Stop</c> 打断"时会访问违例的那条路径。
    /// </summary>
    [Fact]
    public void FindPathOnMap_BreakMidwayLeavesNullsAndCrashes()
    {
        var map = new ProbeMap { Width = 5, Height = 1 };
        map.StartFind = true;
        map.GetCostFunc = (x, y, d, pw) => 1;
        map.PathMapArray = map.CallFillPathMap(0, 0, 4, 0);

        map.StartFind = false;                    // 模拟并发 Stop() 把开关关掉
        Assert.Throws<NullReferenceException>(() => map.FindPathOnMap(4, 0));
    }

    /// <summary>目标越出 <c>ClientRect</c> ⇒ 置 <c>StartFind := False</c> 后立刻返回，<c>FPath</c> 不变。</summary>
    [Fact]
    public void FindPathOnMap_OutOfClientRectExitsEarly()
    {
        var map = new ProbeMap { Width = 5, Height = 1, StartFind = true };
        map.GetCostFunc = (x, y, d, pw) => 1;
        map.PathMapArray = map.CallFillPathMap(0, 0, 4, 0);

        map.FindPathOnMap(5, 0);                  // nX = 5 >= Right-Left = 5
        Assert.False(map.StartFind);
        Assert.Null(map.Path);

        map.StartFind = true;
        map.FindPathOnMap(0, 1);                  // nY = 1 >= Bottom-Top = 1
        Assert.False(map.StartFind);
        Assert.Null(map.Path);
    }

    /// <summary>目标 <c>Distance &lt; 0</c>（不可达）⇒ 同样早退。</summary>
    [Fact]
    public void FindPathOnMap_UnreachableTargetExitsEarly()
    {
        var map = new ProbeMap { Width = 5, Height = 1, StartFind = true };
        map.GetCostFunc = (x, y, d, pw) => x == 2 ? -1 : 1;   // 目标 4 不可达
        map.PathMapArray = map.CallFillPathMap(0, 0, 4, 0);

        map.FindPathOnMap(4, 0);
        Assert.False(map.StartFind);
        Assert.Null(map.Path);
        Assert.Null(map.RunPath);
    }

    /// <summary><c>PathMapArray</c> 为空（<c>nil</c> 或长度 0）⇒ 同样早退（原文 <c>Length(...) &lt;= 0</c>）。</summary>
    [Fact]
    public void FindPathOnMap_EmptyPathMapArrayExitsEarly()
    {
        var map = new ProbeMap { Width = 5, Height = 1, StartFind = true };
        map.GetCostFunc = (x, y, d, pw) => 1;
        map.PathMapArray = Array.Empty<TPathMapCell[]>();

        map.FindPathOnMap(0, 0);
        Assert.False(map.StartFind);
        Assert.Null(map.Path);

        map.PathMapArray = null;
        map.StartFind = true;
        map.FindPathOnMap(0, 0);
        Assert.False(map.StartFind);
        Assert.Null(map.Path);
    }

    // ------------------------------------------------------------ WalkToRun

    /// <summary>
    /// 差异断言（值语义）：<c>WalkToRun</c> 内部把中间点标成 <c>(-1,-1)</c>，
    /// 但**必须**只改自己那份拷贝 —— 原文 <c>WalkPath[I] := Path[I]</c> 是记录值拷贝。
    /// 托管侧 <c>PathPoint</c> 是 class，若直接赋值就会把 <c>FPath</c> 一起改坏。
    /// </summary>
    [Fact]
    public void WalkToRun_DoesNotAliasInputPath()
    {
        var map = new ProbeMap { StartFind = true };
        map.FPathProbe = StraightLine(5);

        map.WalkToRun();

        var path = map.FPathProbe;
        for (int i = 0; i < path.Length; i++)          // FPath 一个坐标都不许被改成 -1
        {
            Assert.Equal(i, path[i].X);
            Assert.Equal(0, path[i].Y);
        }

        var run = map.FRunPathProbe;
        Assert.Equal(2, run.Length);
        Assert.NotSame(path[2], run[0]);               // RunPath 也不与 FPath 共享实例
        Assert.NotSame(path[4], run[1]);
    }

    /// <summary>
    /// 骑马支（<c>m_btHorse &lt;&gt; 0</c> 且 <c>boHorseRun3Grid</c>）一次合并 3 格：
    /// 7 点直线 ⇒ 标记 1,2 与 4,5 ⇒ 收集 3、6（对照普通支的 2、4、6）。
    /// </summary>
    [Fact]
    public void WalkToRun_HorseThreeGridBranchMergesThreeSteps()
    {
        using var env = new Env();
        PathFindClientSeam.MySelfHorse = () => 1;
        PathFindClientSeam.ClientConfigHorseRun3Grid = () => true;

        var map = new ProbeMap { StartFind = true };
        map.FPathProbe = StraightLine(7);
        map.WalkToRun();

        var run = map.FRunPathProbe;
        Assert.Equal(2, run.Length);
        Assert.Equal(3, run[0].X);
        Assert.Equal(6, run[1].X);
    }

    /// <summary>骑马标志只满足一半（有马但配置关）⇒ 仍走普通两支合并。</summary>
    [Theory]
    [InlineData(0, true)]     // 没骑马
    [InlineData(1, false)]    // 有马但配置关
    public void WalkToRun_HorseBranchNeedsBothConditions(int horse, bool cfg)
    {
        using var env = new Env();
        PathFindClientSeam.MySelfHorse = () => horse;
        PathFindClientSeam.ClientConfigHorseRun3Grid = () => cfg;

        var map = new ProbeMap { StartFind = true };
        map.FPathProbe = StraightLine(7);
        map.WalkToRun();

        // 普通支：标记 1,3,5 ⇒ 收集 2,4,6
        var xs = map.FRunPathProbe.Select(p => p.X).ToArray();
        Assert.Equal(new[] { 2, 4, 6 }, xs);
    }

    /// <summary>
    /// 转向不合并：<c>(0,0)→(1,0)→(1,1)</c> 两段方向不同（右 / 下）⇒ 走 <c>Dec(I); nStep := 0; Continue</c>
    /// 一路不标记 ⇒ <c>RunPath</c> 收满 2 点（对照组，证明上面的合并用例确实有判别力）。
    /// </summary>
    [Fact]
    public void WalkToRun_TurnIsNotMerged()
    {
        using var env = new Env();
        var map = new ProbeMap { StartFind = true };
        map.FPathProbe = new[] { new PathPoint(0, 0), new PathPoint(1, 0), new PathPoint(1, 1) };

        map.WalkToRun();

        var run = map.FRunPathProbe;
        Assert.Equal(2, run.Length);
        Assert.Equal((1, 0), (run[0].X, run[0].Y));
        Assert.Equal((1, 1), (run[1].X, run[1].Y));
    }

    /// <summary>
    /// <c>GetNextDirection</c> 的第一条改写规则：<c>abs(sy - dy) &gt; 2</c> 且
    /// <c>sx ∈ [dx-1, dx+1]</c> 时把 <c>flagx</c> 强制为 0。
    /// <para>取证算例：<c>GND((1,0) → (0,3))</c> 原始 flagx = -1、flagy = 1（= 左下 5）；
    /// 改写后 flagx = 0 ⇒ 方向变成"下"(4)，与紧随的 <c>(0,3) → (0,4)</c>（下）**相同** ⇒ 合并发生。</para>
    /// <para>若没有这条改写，两段方向为 5 / 4 ⇒ 不合并 ⇒ RunPath 会是 2 点。故本用例有判别力。</para>
    /// </summary>
    [Fact]
    public void GetNextDirection_FlagxOverrideAllowsMerge()
    {
        using var env = new Env();
        var map = new ProbeMap { StartFind = true };
        map.FPathProbe = new[] { new PathPoint(1, 0), new PathPoint(0, 3), new PathPoint(0, 4) };

        map.WalkToRun();

        Assert.Single(map.FRunPathProbe);
        Assert.Equal((0, 4), (map.FRunPathProbe[0].X, map.FRunPathProbe[0].Y));
    }

    /// <summary>
    /// <c>GetNextDirection</c> 的第二条改写规则：<c>abs(sx - dx) &gt; 2</c> 且
    /// <c>sy ∈ (dy-1, dy+1]</c> 时把 <c>flagy</c> 强制为 0。
    /// <para>取证算例：<c>GND((0,1) → (3,0))</c> 原始 flagx = 1、flagy = -1（= 右上 1）；
    /// 改写后 flagy = 0 ⇒ 方向变成"右"(2)，与紧随的 <c>(3,0) → (4,0)</c>（右）**相同** ⇒ 合并发生。</para>
    /// </summary>
    [Fact]
    public void GetNextDirection_FlagyOverrideAllowsMerge()
    {
        using var env = new Env();
        var map = new ProbeMap { StartFind = true };
        map.FPathProbe = new[] { new PathPoint(0, 1), new PathPoint(3, 0), new PathPoint(4, 0) };

        map.WalkToRun();

        Assert.Single(map.FRunPathProbe);
        Assert.Equal((4, 0), (map.FRunPathProbe[0].X, map.FRunPathProbe[0].Y));
    }

    /// <summary>
    /// <c>StartFind = False</c> 时合并循环第一轮就 <c>Break</c> ⇒ 一个点都不标记 ⇒
    /// <c>RunPath</c> 收满 <c>Length-1</c> 个点。
    /// </summary>
    [Fact]
    public void WalkToRun_StartFindFalseSkipsMerging()
    {
        var map = new ProbeMap { StartFind = false };
        map.FPathProbe = StraightLine(5);

        map.WalkToRun();

        var run = map.FRunPathProbe;
        Assert.Equal(4, run.Length);
        Assert.Equal(new[] { 1, 2, 3, 4 }, run.Select(p => p.X).ToArray());
    }

    /// <summary>
    /// 差异断言：收集条件写成 <c>(x &lt;&gt; -1) and (y &lt;&gt; -1)</c> ——
    /// **任一**坐标为 -1 即被丢弃。用一条把 <c>x = -1</c>、<c>y = 5</c> 的点夹在中间的路径取证：
    /// 该点被排除，而 <c>x = 5</c>、<c>y = -1</c> 的点同样被排除（AND 语义）。
    /// </summary>
    [Fact]
    public void WalkToRun_CollectionConditionIsAndOnBothCoordinates()
    {
        using var env = new Env();
        var map = new ProbeMap { StartFind = false };   // 关掉合并，直接考收集条件
        map.FPathProbe = new[]
        {
            new PathPoint(0, 0),
            new PathPoint(-1, 5),     // x = -1 ⇒ 丢弃
            new PathPoint(5, -1),     // y = -1 ⇒ 丢弃
            new PathPoint(1, 1),
        };

        map.WalkToRun();

        Assert.Single(map.FRunPathProbe);
        Assert.Equal((1, 1), (map.FRunPathProbe[0].X, map.FRunPathProbe[0].Y));
    }

    /// <summary>
    /// <c>FPath</c> 为 <c>nil</c> 或长度 0 ⇒ <c>RunPath := nil</c>；
    /// 长度 1 ⇒ <c>RunPath</c> 是**长度为 0 的非 null 数组**（原文 <c>SetLength(FRunPath, 0)</c> 与
    /// 紧随其后的 <c>FRunPath := nil</c> 只在前一支执行）。
    /// </summary>
    [Fact]
    public void WalkToRun_NullEmptyAndSinglePointPaths()
    {
        var map = new ProbeMap();

        map.FPathProbe = null;
        map.WalkToRun();
        Assert.Null(map.FRunPathProbe);

        map.FPathProbe = Array.Empty<PathPoint>();
        map.WalkToRun();
        Assert.Null(map.FRunPathProbe);

        map.FPathProbe = new[] { new PathPoint(7, 7) };
        map.WalkToRun();
        Assert.NotNull(map.FRunPathProbe);
        Assert.Empty(map.FRunPathProbe);
    }

    /// <summary>长度 2 的路径：合并循环一个标记都不做 ⇒ 收集下标 1（起点永不进入 RunPath，缺陷 ④）。</summary>
    [Fact]
    public void WalkToRun_TwoPointPathDropsStartPoint()
    {
        var map = new ProbeMap { StartFind = true };
        map.FPathProbe = new[] { new PathPoint(3, 3), new PathPoint(4, 3) };

        map.WalkToRun();

        var run = map.FRunPathProbe;
        Assert.Single(run);
        Assert.Equal((4, 3), (run[0].X, run[0].Y));
    }

    // -------------------------------------------------------- TLegendMap

    /// <summary><c>TLegendMap.Create</c>：七个字段逐一初始化，<c>Title</c> 空串。</summary>
    [Fact]
    public void LegendMap_CreateInitialisesFields()
    {
        var legend = new TLegendMap();
        Assert.False(legend.StartFind);
        Assert.Equal(0, legend.PathPoisonIndex);
        Assert.Equal(0, legend.FindCount);
        Assert.Equal("", legend.Title);
        Assert.Equal(0, legend.BeginX);
        Assert.Equal(0, legend.EndX);
        Assert.Equal(0, legend.FindX);
        Assert.NotNull(legend.MapData);
        Assert.Empty(legend.MapData);
        Assert.Equal(120, legend.ScopeValue);          // 继承自 TPathMap.Create
    }

    /// <summary><c>Stop</c>：路径清空、全部计数/起止点归 -1（或 0）、<c>PathMapArray := nil</c>。</summary>
    [Fact]
    public void LegendMap_StopClearsEverything()
    {
        var legend = new TLegendMap
        {
            Title = "t",
            FindCount = 9,
            BeginX = 1,
            BeginY = 2,
            EndX = 3,
            EndY = 4,
            FindX = 5,
            FindY = 6,
            PathPoisonIndex = 7,
            StartFind = true,
        };
        legend.PathMapArray = new[] { new TPathMapCell[1] };
        legend.Path = StraightLine(2);
        legend.RunPath = StraightLine(2);

        legend.Stop();

        Assert.Null(legend.Path);
        Assert.Null(legend.RunPath);
        Assert.False(legend.StartFind);
        Assert.Equal(-1, legend.BeginX);
        Assert.Equal(-1, legend.BeginY);
        Assert.Equal(-1, legend.EndX);
        Assert.Equal(-1, legend.EndY);
        Assert.Equal(-1, legend.FindX);
        Assert.Equal(-1, legend.FindY);
        Assert.Equal(0, legend.PathPoisonIndex);
        Assert.Equal(0, legend.FindCount);
        Assert.Empty(legend.PathMapArray);
    }

    /// <summary>
    /// 两个参数的 <c>FindPath(StopX, StopY, ExcludeMonster)</c>：
    /// <c>FindCount</c> 自增、<c>FindX/FindY</c> 落位、随后 <c>FindPathOnMap</c> 把 <c>StartFind</c> 收尾为假。
    /// <para>空地图（<c>ClientRect</c> 全 0）下不会卡在自旋等待里（本版不调 <c>FillPathMap</c>）。</para>
    /// </summary>
    [Fact]
    public void LegendMap_FindPathTwoArgUpdatesCountersAndTerminates()
    {
        var legend = new TLegendMap();

        legend.FindPath(3, 4);
        Assert.Equal(1, legend.FindCount);
        Assert.Equal(3, legend.FindX);
        Assert.Equal(4, legend.FindY);
        Assert.False(legend.StartFind);              // FindPathOnMap 的早退把它复位

        legend.FindPath(5, 6, true);
        Assert.Equal(2, legend.FindCount);
        Assert.Equal(5, legend.FindX);
        Assert.Equal(6, legend.FindY);
    }

    /// <summary>
    /// 重载等价性：<c>FindPath(x,y)</c> ≡ <c>FindPath(x,y,false)</c>，
    /// <c>FindPath(a,b,c,d)</c> ≡ <c>FindPath(a,b,c,d,0,false)</c>，
    /// <c>FindPath(a,b,c,d,space)</c> ≡ <c>FindPath(a,b,c,d,space,false)</c>
    /// （原文三个默认参数；托管侧拆成显式重载以避开 C# 默认参歧义，见 D-P9-05）。
    /// </summary>
    [Fact]
    public void LegendMap_FindPathOverloadsMatchDefaultArguments()
    {
        var a = new TLegendMap { Width = 3, Height = 3 };
        var b = new TLegendMap { Width = 3, Height = 3 };
        a.FindPath(0, 0, 0, 0);
        b.FindPath(0, 0, 0, 0, 0, false);
        Assert.Equal(b.FindCount, a.FindCount);
        Assert.Equal(b.PathWidth, a.PathWidth);
        Assert.Equal(b.PathMapArray.Length, a.PathMapArray.Length);

        var c = new TLegendMap { Width = 3, Height = 3 };
        var d = new TLegendMap { Width = 3, Height = 3 };
        c.FindPath(0, 0, 0, 0, 0);
        d.FindPath(0, 0, 0, 0, 0, false);
        Assert.Equal(d.PathWidth, c.PathWidth);

        // 5 参数版把 PathSpace 写进 PathWidth
        var e = new TLegendMap { Width = 3, Height = 3 };
        e.FindPath(0, 0, 0, 0, 4);
        Assert.Equal(4, e.PathWidth);
    }

    /// <summary>
    /// 差异断言（原文缺陷 ⑥）：两个版本的**前几步顺序不同** ——
    /// 两参版是 <c>Inc(FindCount)</c> 后 <c>FExcludeMonster := ...</c>；
    /// 六参版是 <c>FExcludeMonster := ...</c> 后 <c>Inc(FindCount)</c>。
    /// 两者最终可观察结果相同，这里用"各跑一次、计数各自为 1"取证。
    /// </summary>
    [Fact]
    public void LegendMap_BothFindPathVariantsIncrementFindCountOnce()
    {
        var two = new TLegendMap { Width = 3, Height = 3 };
        two.FindPath(0, 0);
        Assert.Equal(1, two.FindCount);

        var six = new TLegendMap { Width = 3, Height = 3 };
        six.FindPath(0, 0, 0, 0, 0, false);
        Assert.Equal(1, six.FindCount);
    }

    /// <summary>
    /// <c>SetStartPos</c>（原文 :771-777）**不**触碰 <c>FindCount</c>/<c>FindX</c>/<c>FindY</c>、
    /// 也**不**设 <c>StartFind</c>（原文缺陷 ⑧：它沿用调用前的开关）；只写起止点、
    /// <c>PathWidth</c> 与 <c>PathMapArray</c>。
    /// </summary>
    [Fact]
    public void LegendMap_SetStartPosFillsMapWithoutTouchingFindCounters()
    {
        var legend = new TLegendMap { Width = 4, Height = 1, FindCount = 7, FindX = 8, FindY = 9, StartFind = true };
        legend.GetCostFunc = (x, y, d, pw) => 1;

        legend.SetStartPos(0, 0, 3);

        Assert.Equal(0, legend.BeginX);
        Assert.Equal(0, legend.BeginY);
        Assert.Equal(3, legend.PathWidth);
        Assert.Equal(4, legend.PathMapArray[0].Length);
        Assert.Equal(7, legend.FindCount);        // 计数取证：都不变
        Assert.Equal(8, legend.FindX);
        Assert.Equal(9, legend.FindY);
        Assert.True(legend.StartFind);            // 原文不碰它 ⇒ 保持调用前的真
    }

    /// <summary>
    /// 差异断言（原文缺陷 ⑧）：<c>SetStartPos</c> 不设 <c>StartFind</c> ⇒
    /// 若调用前 <c>StartFind = False</c>，内部的 <c>FillPathMap</c> 会在 <c>PreparePathMap</c>
    /// 立刻返回后写 <c>Result[nY1][nX1]</c> ⇒ 空引用崩溃（原文同处是访问违例）。
    /// 这是"<c>SetStartPos</c> 必须紧跟在置真之后"的硬约束。
    /// </summary>
    [Fact]
    public void LegendMap_SetStartPosRequiresStartFindAlreadyTrue()
    {
        var legend = new TLegendMap { Width = 4, Height = 1, StartFind = false };
        legend.GetCostFunc = (x, y, d, pw) => 1;

        Assert.Throws<NullReferenceException>(() => legend.SetStartPos(0, 0, 3));
    }

    /// <summary>
    /// <c>GetCost</c>（override）三件事：越界 -1、按 <c>FExcludeMonster</c> 选
    /// <c>NewCanWalkEx</c> / <c>NewCanWalkEx_2</c>（可走 4 / 否则 -1）、斜方向乘 1.5。
    /// <para>以 <c>protected</c> 探针调 <c>GetCost</c>：界内 d=0/2 得 4，d=1/3（奇数 ⇒ 斜）得 6
    /// （<c>4 + (4 shr 1)</c>），越界得 -1；障碍格上斜方向**仍是 -1**（<c>Result &gt; 0</c> 的门挡住放大）。</para>
    /// </summary>
    [Fact]
    public void LegendMap_GetCostAppliesWalkabilityAndDiagonalMarkup()
    {
        using var env = new Env();
        PathFindClientSeam.NewCanWalkEx = (x, y) => x == 0;      // 只有 x = 0 可走
        PathFindClientSeam.NewCanWalkEx_2 = (x, y) => x == 1;    // 只有 x = 1 可走

        var legend = new ProbeLegend { Width = 3, Height = 3 };
        legend.GetClientRect(0, 0, 0, 0);

        // FExcludeMonster 的默认值是 False ⇒ 用 NewCanWalkEx
        Assert.Equal(4, legend.CallGetCost(0, 0, 0));
        Assert.Equal(-1, legend.CallGetCost(1, 0, 0));
        Assert.Equal(6, legend.CallGetCost(0, 0, 1));    // 斜：4 + 2
        Assert.Equal(6, legend.CallGetCost(0, 1, 3));    // 斜：4 + 2
        Assert.Equal(-1, legend.CallGetCost(1, 0, 1));   // 不可走 ⇒ 斜也不放大
        Assert.Equal(-1, legend.CallGetCost(-1, 0, 0));
        Assert.Equal(-1, legend.CallGetCost(3, 0, 0));
        Assert.Equal(6, legend.CallGetCost(0, 0, 9));    // `and 7` 归约 ⇒ 9 → 1（斜）

        // 经 FindPath 把 FExcludeMonster 置真 ⇒ 改用 NewCanWalkEx_2
        legend.FindPath(0, 0, 0, 0, 0, true);
        Assert.Equal(-1, legend.CallGetCost(0, 0, 0));
        Assert.Equal(4, legend.CallGetCost(1, 0, 0));
        Assert.Equal(6, legend.CallGetCost(1, 0, 1));

        // 再置回假
        legend.FindPath(0, 0, 0, 0, 0, false);
        Assert.Equal(4, legend.CallGetCost(0, 0, 0));
    }

    /// <summary>
    /// <c>ExcludeMonster</c> 真的改变了可达性：同一张图上"只有 x &lt;= 1 可走"与"只有 x &gt;= 1 可走"
    /// 让 <c>FindPath(0,0,2,0,...)</c> 一败一成。
    /// </summary>
    [Fact]
    public void LegendMap_ExcludeMonsterFlipsReachability()
    {
        using var env = new Env();
        PathFindClientSeam.NewCanWalkEx = (x, y) => x <= 1;
        PathFindClientSeam.NewCanWalkEx_2 = (x, y) => x >= 1;

        var blocked = new TLegendMap { Width = 3, Height = 1 };
        blocked.FindPath(0, 0, 2, 0, 0, false);
        Assert.Null(blocked.Path);                    // (2,0) 不可走 ⇒ 不可达

        var open = new TLegendMap { Width = 3, Height = 1 };
        open.FindPath(0, 0, 2, 0, 0, true);
        Assert.NotNull(open.Path);                    // 换成 _2 后可达
        Assert.Equal(3, open.Path.Length);
    }

    /// <summary>
    /// <c>Find</c>（原文 :742-748）：起一个 <c>TFindPathThread</c> 后**才**写 <c>FExcludeMonster</c>
    /// （原文缺陷 ①，顺序逐字保留）。这里用 <c>GetCost</c> 的支路取证该字段确实被写进去了。
    /// </summary>
    [Fact]
    public void LegendMap_FindSetsExcludeMonster()
    {
        using var env = new Env();
        // 线程体作用在**全局** LegendMap 上 ⇒ 必须给它一个带尺寸的实例，
        // 否则 0×0 地图在 FillPathMap 里越界，后台线程的未捕获异常会终止测试进程。
        PathFindClientSeam.LegendMap = new TLegendMap { Width = 1, Height = 1 };
        PathFindClientSeam.NewCanWalkEx = (x, y) => x == 0;
        PathFindClientSeam.NewCanWalkEx_2 = (x, y) => x == 1;

        var legend = new ProbeLegend { Width = 3, Height = 3 };
        legend.GetClientRect(0, 0, 0, 0);
        Assert.Equal(4, legend.CallGetCost(0, 0, 0));

        legend.Find(0, 0, 0, 0, true);
        Assert.Equal(-1, legend.CallGetCost(0, 0, 0));   // 已切到 NewCanWalkEx_2
        Assert.Equal(4, legend.CallGetCost(1, 0, 0));

        legend.Find(0, 0, 0, 0);                        // 默认 False 的重载
        Assert.Equal(4, legend.CallGetCost(0, 0, 0));

        // 等后台线程收工，避免它在夹具还原接缝之后才去读 LegendMap
        WaitForFind(PathFindClientSeam.LegendMap, 1);
    }

    // ------------------------------------------------------- TFindPathThread

    /// <summary>
    /// <c>TFindPathThread.Execute</c> 对**全局** LegendMap 调四参 <c>FindPath</c>：
    /// 等线程结束后 <c>FindX/FindY/FindCount</c> 都应落位。
    /// </summary>
    [Fact]
    public void FindPathThread_ExecuteCallsGlobalLegendMap()
    {
        using var env = new Env();
        var global = new TLegendMap { Width = 4, Height = 5 };
        PathFindClientSeam.LegendMap = global;

        var t = new TFindPathThread(1, 2, 3, 4);
        Assert.True(t.WaitFor(10_000));

        Assert.Equal(1, global.FindCount);
        Assert.Equal(3, global.FindX);
        Assert.Equal(4, global.FindY);
    }

    /// <summary><c>FreeOnTerminate := True</c> 在构造里就被置真（原文 :203）；<c>Destroy</c> 是空操作。</summary>
    [Fact]
    public void FindPathThread_FreeOnTerminateIsTrueAndDestroyIsNoOp()
    {
        using var env = new Env();
        PathFindClientSeam.LegendMap = new TLegendMap { Width = 2, Height = 2 };

        var t = new TFindPathThread(0, 0, 1, 1);
        Assert.True(t.FreeOnTerminate);
        Assert.True(t.WaitFor(10_000));
        t.Destroy();
        Assert.True(t.WaitFor(0));                    // 已结束
    }

    /// <summary><c>Execute</c> 是 <c>protected virtual</c>（原文 <c>override</c> 自 <c>TThread</c>）⇒ 可被覆写。</summary>
    [Fact]
    public void FindPathThread_ExecuteIsVirtual()
    {
        using var env = new Env();
        var spy = new SpyFindPathThread(1, 2, 3, 4);
        Assert.True(spy.WaitFor(10_000));
        Assert.Equal(1, spy.Calls);
        // 覆写后没有再碰全局 LegendMap
        Assert.Equal(0, PathFindClientSeam.LegendMap.FindCount);
    }

    private sealed class SpyFindPathThread : TFindPathThread
    {
        public int Calls;

        public SpyFindPathThread(int sx, int sy, int tx, int ty) : base(sx, sy, tx, ty)
        {
        }

        protected override void Execute()
        {
            Interlocked.Increment(ref Calls);
        }
    }

    // --------------------------------------------------------------- 接缝

    /// <summary>
    /// 接缝默认值（M2Server 侧没有客户端对象 ⇒ 全部为空状态），
    /// 且 <c>LegendMap</c> 是**非 null 的空实例**（保证 <c>Execute</c> 不会空引用）。
    /// </summary>
    [Fact]
    public void SeamDefaultsAreSafeEmptyState()
    {
        using var env = new Env();

        Assert.NotNull(PathFindClientSeam.LegendMap);
        Assert.Equal(0, PathFindClientSeam.MySelfHorse());
        Assert.False(PathFindClientSeam.ClientConfigHorseRun3Grid());
        Assert.False(PathFindClientSeam.NewCanWalkEx(0, 0));
        Assert.False(PathFindClientSeam.NewCanWalkEx_2(0, 0));

        // ResetDefaults 之后仍是同一组默认值
        PathFindClientSeam.MySelfHorse = () => 5;
        PathFindClientSeam.ResetDefaults();
        Assert.Equal(0, PathFindClientSeam.MySelfHorse());
    }

    /// <summary>
    /// 覆盖面对账（否定性取证，台账 §37.3）：本单元公开面 = 原文 <c>PathFindClient.pas</c>
    /// interface 段列出的成员，逐条清点计数。
    /// </summary>
    [Fact]
    public void PublicSurfaceMatchesOriginalInterfaceSection()
    {
        var mapMembers = new[]
        {
            "PathMapArray", "GetCostFunc", "PathWidth", "ClientRect", "ScopeValue", "StartFind",
            "GetClientRect", "FindPathOnMap", "WalkToRun", "MapX", "MapY", "LoaclX", "LoaclY",
            "Path", "RunPath", "Width", "Height",
        };
        var mapType = typeof(TPathMap);
        foreach (var name in mapMembers)
            Assert.Contains(mapType.GetMembers(), m => m.Name == name);
        Assert.Equal(17, mapMembers.Length);

        var legendFields = new[]
        {
            "Title", "FindCount", "BeginX", "BeginY", "EndX", "EndY", "FindX", "FindY",
            "PathPoisonIndex", "MapData",
        };
        var legendType = typeof(TLegendMap);
        foreach (var name in legendFields)
            Assert.Contains(legendType.GetMembers(), m => m.Name == name);
        Assert.Equal(10, legendFields.Length);

        // FindPath 的 5 个显式重载（原文 2 个方法 × 默认参展开后共 5 种合法调用形态：
        // (x,y) / (x,y,excl) / (a,b,c,d) / (a,b,c,d,space) / (a,b,c,d,space,excl)）
        var findPathOverloads = legendType.GetMethods().Count(m => m.Name == "FindPath");
        Assert.Equal(5, findPathOverloads);
        var findOverloads = legendType.GetMethods().Count(m => m.Name == "Find");
        Assert.Equal(2, findOverloads);

        var threadType = typeof(TFindPathThread);
        Assert.Contains(threadType.GetMembers(), m => m.Name == "FreeOnTerminate");
        Assert.Contains(threadType.GetMethods(), m => m.Name == "WaitFor");
        Assert.Contains(threadType.GetMethods(), m => m.Name == "Destroy");
    }
}
