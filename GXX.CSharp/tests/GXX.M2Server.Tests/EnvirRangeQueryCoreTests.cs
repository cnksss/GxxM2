using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J160：`TEnvirnoment` 范围取物族 1:1 测试。
/// **累加契约由 32 处调用点的程序化核对验证、范围形状与圆做逐格数值对照、
/// 两族死亡门的"形状相同极性相反"经探针实测。**
/// </summary>
public sealed class EnvirRangeQueryCoreTests
{
    // ===================== 锁号 =====================

    [Fact]
    public void CellLockIds()
    {
        // **43、44、45 连续、且紧接 J159 的 38/39/40**
        Assert.True(EnvirRangeQueryCore.CellLockIdsContiguous());
        Assert.True(EnvirRangeQueryCore.FollowsJ159LockIds());
        Assert.Equal(new[] { 43, 44, 45 }, EnvirRangeQueryCore.CellLevelLockIds);
    }

    [Fact]
    public void LocksOnlyAtCellLevel()
    {
        // **范围包装一把锁都没有**
        Assert.True(EnvirRangeQueryCore.WrapperNoLock());
        Assert.True(EnvirRangeQueryCore.LocksOnlyAtCellLevel());
        Assert.Equal(3, EnvirRangeQueryCore.WrapperCount());
        Assert.Equal(3, EnvirRangeQueryCore.CellLevelCount());
    }

    // ===================== 一、累加契约 =====================

    [Fact]
    public void ResultIsTotalNotDelta()
    {
        Assert.True(EnvirRangeQueryCore.ResultIsTotalNotDelta());
        Assert.Equal(5, EnvirRangeQueryCore.ResultIsListCount(5));
    }

    [Fact]
    public void AccumulatesAcrossCalls()
    {
        // **连续调用返回值单调不减**
        Assert.True(EnvirRangeQueryCore.AccumulatesAcrossCalls());
        Assert.True(EnvirRangeQueryCore.DoesNotClearList());
    }

    [Fact]
    public void CallersRelyOnAccumulation()
    {
        // **32 处调用点里只有 1 处清空**
        Assert.True(EnvirRangeQueryCore.OnlyOneCallerClears());
        Assert.Equal(32, EnvirRangeQueryCore.TotalRangeCallers());
        Assert.Equal(1, EnvirRangeQueryCore.CallersThatClear());
        Assert.True(EnvirRangeQueryCore.CallersRelyOnAccumulation());
        Assert.True(EnvirRangeQueryCore.NotClearingIsIntentional());
    }

    [Fact]
    public void EngineCallerClearsBecauseReusing()
    {
        // **唯一清空的那处是引擎自身的振动调用点**
        Assert.True(EnvirRangeQueryCore.EngineCallerClearsBecauseReusing());
        Assert.True(EnvirRangeQueryCore.EngineCallerSiteKnown());
        Assert.True(EnvirRangeQueryCore.ReuseRequiresClear());
    }

    // ===================== 二、范围形状 =====================

    [Fact]
    public void SquareNotCircle()
    {
        Assert.True(EnvirRangeQueryCore.SquareNotCircle());
        Assert.True(EnvirRangeQueryCore.SquareCellCountValues());
        Assert.Equal(9, EnvirRangeQueryCore.SquareCellCount(1));
        Assert.Equal(25, EnvirRangeQueryCore.SquareCellCount(2));
        Assert.Equal(49, EnvirRangeQueryCore.SquareCellCount(3));
    }

    [Fact]
    public void SquareExceedsCircle()
    {
        // **方形严格多于同半径的圆**
        Assert.True(EnvirRangeQueryCore.SquareExceedsCircle());
        Assert.True(EnvirRangeQueryCore.RadiusOneGap());
        Assert.True(EnvirRangeQueryCore.RadiusTwoGap());
        Assert.Equal(5, EnvirRangeQueryCore.CircleCellCount(1));
        Assert.Equal(13, EnvirRangeQueryCore.CircleCellCount(2));
    }

    [Fact]
    public void CornerDistance()
    {
        // **角点距离是半径的根号二倍、超出范围**
        Assert.True(EnvirRangeQueryCore.CornerDistanceIsSqrt2());
        Assert.True(EnvirRangeQueryCore.CornerExceedsRadius());
    }

    [Fact]
    public void RangeLoopShape()
    {
        // **边界含头含尾；X 外层、Y 内层；半径 0 退化成一格**
        Assert.True(EnvirRangeQueryCore.RangeBoundsInclusive());
        Assert.True(EnvirRangeQueryCore.XOuterYInner());
        Assert.True(EnvirRangeQueryCore.ZeroRadiusSingleCell());
    }

    [Fact]
    public void ThreeRangeWrappersIdentical()
    {
        Assert.True(EnvirRangeQueryCore.ThreeRangeWrappersIdentical());
    }

    // ---------- 缺陷① 丢弃返回值 ----------

    [Fact]
    public void ReturnValueDiscarded()
    {
        // **范围包装丢弃了格子级方法的返回值**
        Assert.True(EnvirRangeQueryCore.ReturnValueDiscarded());
        Assert.True(EnvirRangeQueryCore.DiscardedCallHasNoAssignment());
        Assert.True(EnvirRangeQueryCore.EquivalentButWasteful());
        Assert.True(EnvirRangeQueryCore.DiscardedIsEquivalent());
        Assert.True(EnvirRangeQueryCore.IntentLostInWrapper());
    }

    [Fact]
    public void OverlappingResultSemantics()
    {
        // **缺陷②：内层增量即使想用也用不上**
        Assert.True(EnvirRangeQueryCore.OverlappingResultSemantics());
        Assert.True(EnvirRangeQueryCore.DeltaUnavailableFromCellLevel());
    }

    // ---------- 缺陷③ 锁粒度 ----------

    [Fact]
    public void CellLevelLocking()
    {
        // **缺陷③：锁在每格粒度反复加解、整个范围扫描不原子**
        Assert.True(EnvirRangeQueryCore.WrapperNoLock2());
        Assert.True(EnvirRangeQueryCore.CellLevelLocking());
        Assert.True(EnvirRangeQueryCore.RangeScanNotAtomic());
        Assert.True(EnvirRangeQueryCore.LockOperationsValues());
        Assert.Equal(49, EnvirRangeQueryCore.LockOperations(3));
    }

    [Fact]
    public void LockCostComparison()
    {
        // **半径 3 要加解 49 次锁，范围级加锁只需 1 次**
        Assert.True(EnvirRangeQueryCore.RangeLevelWouldBeOne());
        Assert.True(EnvirRangeQueryCore.LockCostComparison());
    }

    // ===================== 三、GetBaseObjects 与 GetPlayObjects =====================

    [Fact]
    public void TwoMethodsDifferByOneGate()
    {
        // **只差一层种族判定、行数差 3**
        Assert.True(EnvirRangeQueryCore.TwoMethodsDifferByOneGate());
        Assert.True(EnvirRangeQueryCore.LineDifferenceIsThree());
        Assert.True(EnvirRangeQueryCore.PlayObjectsOnlyHumans());
    }

    [Fact]
    public void RaceGate()
    {
        // **RC_PLAYOBJECT = 0；怪物、英雄、NPC 都被拒**
        Assert.True(EnvirRangeQueryCore.PlayObjectIsZero());
        Assert.True(EnvirRangeQueryCore.OnlyHumansPass());
        Assert.True(EnvirRangeQueryCore.RaceGate(0));
        Assert.False(EnvirRangeQueryCore.RaceGate(80));
    }

    [Fact]
    public void ThreeNestedLayers()
    {
        Assert.True(EnvirRangeQueryCore.ThreeNestedLayers());
        Assert.True(EnvirRangeQueryCore.ThreeNestedLayerCount());
        Assert.Equal(3, EnvirRangeQueryCore.NestedLayers.Length);
    }

    [Fact]
    public void Bo2B9AlsoGatesRangeQueries()
    {
        // **开着的城门不会被任何范围取物收进去 —— 与 J159 互相印证**
        Assert.True(EnvirRangeQueryCore.Bo2B9AlsoGatesRangeQueries());
        Assert.True(EnvirRangeQueryCore.ConsistentWithJ159());
        Assert.True(EnvirRangeQueryCore.OpenCastleDoorExcluded());
        Assert.True(EnvirRangeQueryCore.LayerTwoValues());
    }

    [Fact]
    public void DeathGateShapeIdenticalPolarityOpposite()
    {
        // **形状完全相同、但"哪个取值代表包含死亡"相反**
        Assert.True(EnvirRangeQueryCore.DeathGateShapeIdentical());
        Assert.True(EnvirRangeQueryCore.DeathGateShapeIdenticalPolarityOpposite());
        Assert.True(EnvirRangeQueryCore.PolarityOppositeOnDeadObject());
        Assert.True(EnvirRangeQueryCore.ExcludeTriggerFlagValueIsOpposite());
    }

    [Fact]
    public void DeathGateSemantics()
    {
        // **两个参数都"True 即排除死亡"**
        Assert.True(EnvirRangeQueryCore.J159BoFlagTrueExcludes());
        Assert.True(EnvirRangeQueryCore.IncDeathObjectTrueExcludes());
        Assert.True(EnvirRangeQueryCore.BothTrueMeansExcludeButOnlyOneIsMisleading());
    }

    [Fact]
    public void NameOppositeToMeaning()
    {
        // **IncDeathObject 名字像"包含"、实际是"排除"**
        Assert.True(EnvirRangeQueryCore.NameOppositeToMeaning());
        Assert.True(EnvirRangeQueryCore.IncDeathObjectMeaningKnown());
        Assert.True(EnvirRangeQueryCore.IncDeathObjectIsSelfDocumenting());
        Assert.True(EnvirRangeQueryCore.BoFlagIsNot());
    }

    [Fact]
    public void SourceComment()
    {
        // **紧邻范围包装的三行注释解释 boFlag**
        Assert.True(EnvirRangeQueryCore.ThreeLineCommentExplainsBoFlag());
        Assert.True(EnvirRangeQueryCore.ThreeCommentLines());
        Assert.Equal(3, EnvirRangeQueryCore.BoFlagComment.Length);
        Assert.True(EnvirRangeQueryCore.CommentPlacedAfterWrapper());
    }

    [Fact]
    public void SameConceptTwoNames()
    {
        // **包装层叫 boFlag、实现层叫 IncDeathObject**
        Assert.True(EnvirRangeQueryCore.SameConceptTwoNames());
        Assert.True(EnvirRangeQueryCore.TwoParamNamesCount());
        Assert.Equal(2, EnvirRangeQueryCore.TwoParamNames.Length);
        Assert.True(EnvirRangeQueryCore.CommentDescribesSharedParameter());
        Assert.True(EnvirRangeQueryCore.CommentExplainsOnlyOneName());
    }

    [Fact]
    public void CommentMatchesImplementation()
    {
        // **注释说 FALSE 包括死亡对象、实现一致**
        Assert.True(EnvirRangeQueryCore.CommentMatchesImplementation());
        Assert.True(EnvirRangeQueryCore.CommentConsistencyCheck());
    }

    // ===================== 四、GetItemObjects =====================

    [Fact]
    public void ItemFilterDiffers()
    {
        Assert.True(EnvirRangeQueryCore.ItemFilterDiffers());
        Assert.True(EnvirRangeQueryCore.ItemIsTwo());
        Assert.True(EnvirRangeQueryCore.ItemGate(true, 2));
        Assert.False(EnvirRangeQueryCore.ItemGate(true, 1));
    }

    [Fact]
    public void NoBo2B9ForItems()
    {
        // **物品版没有 bo2B9、没有死亡判定，只有"非幽灵"一个条件**
        Assert.True(EnvirRangeQueryCore.NoBo2B9ForItems());
        Assert.True(EnvirRangeQueryCore.ItemSurvivalSingleCondition());
    }

    [Fact]
    public void ResultInsideLock()
    {
        Assert.True(EnvirRangeQueryCore.ResultInsideLock());
        Assert.True(EnvirRangeQueryCore.AllSixReturnCount());
    }

    [Fact]
    public void SixMethods()
    {
        Assert.True(EnvirRangeQueryCore.SixMethods());
        Assert.Equal(6, EnvirRangeQueryCore.MethodNames.Length);
    }

    // ===================== 五、共性 =====================

    [Fact]
    public void NoEarlyInitialization()
    {
        // **六个方法都没有开头初始化 —— 与 J159 七个方法形成对比**
        Assert.True(EnvirRangeQueryCore.NoEarlyInitialization());
        Assert.True(EnvirRangeQueryCore.ContrastWithJ159());
        Assert.True(EnvirRangeQueryCore.J159InitializesAll());
        Assert.True(EnvirRangeQueryCore.NoExitPathSoNoInitNeeded());
    }

    [Fact]
    public void NoExitStatements()
    {
        // **六个方法都没有 Exit**
        Assert.True(EnvirRangeQueryCore.NoExitStatements());
        Assert.True(EnvirRangeQueryCore.J159HasExit());
        Assert.True(EnvirRangeQueryCore.OnlyBranchFallsThrough());
        Assert.True(EnvirRangeQueryCore.EmptyPathReturnsZero());
        Assert.Equal(0, EnvirRangeQueryCore.EmptyPathResult());
    }

    [Fact]
    public void AllReturnInteger()
    {
        // **全部返回数量、列表通过引用参数带出**
        Assert.True(EnvirRangeQueryCore.AllReturnInteger());
        Assert.True(EnvirRangeQueryCore.ListPassedByReference());
        Assert.True(EnvirRangeQueryCore.NoListReturningMethod());
    }

    // ===================== 行数 =====================

    [Fact]
    public void MethodLineCounts()
    {
        Assert.Equal(new[] { 34, 12, 12, 15, 37, 40 }, EnvirRangeQueryCore.MethodLineCounts);
    }

    [Fact]
    public void WrapperLines()
    {
        Assert.True(EnvirRangeQueryCore.WrappersAreShort());
        Assert.True(EnvirRangeQueryCore.TwoWrappersTiedShortest());
        Assert.True(EnvirRangeQueryCore.RangePlayObjectHasThreeExtraLines());
        Assert.True(EnvirRangeQueryCore.WrapperCodeIdenticalLength());
    }

    [Fact]
    public void TotalLines()
    {
        Assert.True(EnvirRangeQueryCore.TotalLinesValues());
        Assert.Equal(150, EnvirRangeQueryCore.TotalLines());
        Assert.True(EnvirRangeQueryCore.PlayObjectsIsLongest());
    }

    [Fact]
    public void FamilySums()
    {
        // **格子级 111 行、范围级 39 行**
        Assert.True(EnvirRangeQueryCore.CellVersusWrapperSums());
    }
}
