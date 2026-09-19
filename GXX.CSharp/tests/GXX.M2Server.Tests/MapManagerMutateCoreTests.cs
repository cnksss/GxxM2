using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J156：`TMapManager` 九个增删方法 1:1 测试。
/// **门对象默认值、`-1` 模式选择器、两遍扫描回退、三种删除粒度、
/// 随机点阶梯阈值与正/倒序删除差异全部经探针实测。**
/// </summary>
public sealed class MapManagerMutateCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(MapManagerMutateCore.ConstantsMatchSource());
        Assert.Equal(-1, MapManagerMutateCore.SourceModeSentinel);
        Assert.Equal(201, MapManagerMutateCore.RandRetryLimit);
        Assert.Equal(53, MapManagerMutateCore.AppearanceOffset);
    }

    // ===================== 一、TGateObject 默认值 =====================

    [Fact]
    public void GateNameDefaultsToAddress()
    {
        // **默认名字是对象地址的十进制字符串，两个门必不相同**
        Assert.True(MapManagerMutateCore.GateNameDefaultsToAddress());
    }

    [Fact]
    public void GateDefaults()
    {
        Assert.True(MapManagerMutateCore.GateBoCenterDefaultsTrue());
        Assert.True(MapManagerMutateCore.GateCoordDefaultsMinusOne());
        Assert.True(MapManagerMutateCore.GateOtherDefaults());
    }

    [Fact]
    public void DefaultEqualsSentinel()
    {
        // **默认坐标 -1 恰好等于 GetGate 的模式哨兵值**
        Assert.True(MapManagerMutateCore.DefaultEqualsSentinel());
        Assert.Equal(MapManagerMutateCore.SourceModeSentinel, MapManagerMutateCore.GateDefaultCoord);
    }

    // ===================== 二、GetGate =====================

    [Fact]
    public void GetGateModeSelector()
    {
        Assert.True(MapManagerMutateCore.GetGateModeSelector());
        Assert.True(MapManagerMutateCore.ModeSelectorOnlyChecksSMapX());
        Assert.True(MapManagerMutateCore.TargetModeIgnoresSMapY());

        Assert.Equal("目标模式", MapManagerMutateCore.ModeOf(-1));
        Assert.Equal("源模式", MapManagerMutateCore.ModeOf(0));
    }

    [Fact]
    public void SourceModeMinusOneAmbiguity()
    {
        // **源模式传 -1 会误入目标模式**
        Assert.True(MapManagerMutateCore.SourceModeMinusOneAmbiguity());
    }

    [Fact]
    public void FourthModeSelectorInstance()
    {
        Assert.True(MapManagerMutateCore.FourthModeSelectorInstance());
        Assert.True(MapManagerMutateCore.FourModeSelectorInstances());
        Assert.Equal(4, MapManagerMutateCore.ModeSelectorInstances.Length);
        Assert.Contains("GetGate 的 nSMapX = -1", MapManagerMutateCore.ModeSelectorInstances);
    }

    [Fact]
    public void GetGateLookup()
    {
        Assert.True(MapManagerMutateCore.GetGateLinearFirstMatch());
        Assert.True(MapManagerMutateCore.TargetModeMatches());
        Assert.True(MapManagerMutateCore.SourceModeMatches());
    }

    [Fact]
    public void TargetModeIgnoresSMapYValues()
    {
        // **目标模式下 nSMapY 传什么都不影响结果**
        Assert.True(MapManagerMutateCore.TargetModeIgnoresSMapYValues());
    }

    [Fact]
    public void GetGateCaseSensitive()
    {
        // **名字比较大小写敏感**
        Assert.True(MapManagerMutateCore.GetGateCaseSensitive());
        Assert.True(MapManagerMutateCore.MixedNameComparisonStrategies());
    }

    // ===================== 三、GetMapGateInfo =====================

    [Fact]
    public void TwoPassFallback()
    {
        Assert.True(MapManagerMutateCore.TwoPassFallback());
        Assert.True(MapManagerMutateCore.FirstPassCenterOnly());
        Assert.True(MapManagerMutateCore.SecondPassRelaxed());
    }

    [Fact]
    public void ExitVersusBreak()
    {
        // **第一遍 Exit、第二遍 Break**
        Assert.True(MapManagerMutateCore.ExitVersusBreak());
        Assert.True(MapManagerMutateCore.JumpStylesDiffer());
        Assert.Equal("Exit", MapManagerMutateCore.JumpStyle(1));
        Assert.Equal("Break", MapManagerMutateCore.JumpStyle(2));
    }

    [Fact]
    public void OverloadFilterStrengthDiffers()
    {
        // **3 参版第二遍只看名字，2 参版仍筛源地图号**
        Assert.True(MapManagerMutateCore.OverloadFilterStrengthDiffers());
        Assert.True(MapManagerMutateCore.SecondPassConditionsDiffer());
        Assert.Equal("名字 + 源地图号", MapManagerMutateCore.SecondPassCondition("2arg"));
        Assert.Equal("只有名字", MapManagerMutateCore.SecondPassCondition("3arg"));
    }

    [Fact]
    public void PrefersCenterGate()
    {
        // **优先取中心门**
        Assert.True(MapManagerMutateCore.PrefersCenterGate());
        Assert.True(MapManagerMutateCore.FallsBackToAnyGate());
    }

    [Fact]
    public void SecondPassFiltering()
    {
        Assert.True(MapManagerMutateCore.ThreeArgIgnoresSourceMapInSecondPass());
        Assert.True(MapManagerMutateCore.TwoArgStillFiltersSourceMap());
        Assert.True(MapManagerMutateCore.NoMatchReturnsNull());
    }

    [Fact]
    public void OutputsPresetToSentinel()
    {
        Assert.True(MapManagerMutateCore.OutputsPresetToSentinel());
        Assert.True(MapManagerMutateCore.TwoArgPresetAllMinusOne());
        Assert.True(MapManagerMutateCore.ThreeArgPresetTwoMinusOne());
        Assert.Equal(4, MapManagerMutateCore.TwoArgPresetInts.Length);
    }

    [Fact]
    public void GateInfoCaseSensitive()
    {
        Assert.True(MapManagerMutateCore.GateInfoCaseSensitive());
    }

    // ===================== 四、三个 DelMapRoute =====================

    [Fact]
    public void ThreeDeleteGranularities()
    {
        Assert.True(MapManagerMutateCore.ThreeDeleteGranularities());
        Assert.Equal(3, MapManagerMutateCore.DeleteGranularities.Length);
    }

    [Fact]
    public void ReverseIterationForSafeDelete()
    {
        Assert.True(MapManagerMutateCore.ReverseIterationForSafeDelete());
        Assert.True(MapManagerMutateCore.ReverseDeleteIsSafe());
    }

    [Fact]
    public void ForwardDeleteSkipsOnlyWhenAdjacent()
    {
        // **探针实测：待删项不相邻时正序与倒序结果相同**
        Assert.True(MapManagerMutateCore.ForwardMatchesReverseWhenSparse());

        // **连续两项都该删时才漏**
        Assert.True(MapManagerMutateCore.ForwardDeleteSkips());
    }

    [Fact]
    public void DeleteFromMapBeforeListDelete()
    {
        Assert.True(MapManagerMutateCore.DeleteFromMapBeforeListDelete());
        Assert.True(MapManagerMutateCore.FailedCellDeleteLeavesGhost());
        Assert.True(MapManagerMutateCore.SuccessfulCellDeleteRemoves());
    }

    [Fact]
    public void FindMapCachedVersusPerItem()
    {
        // **一个缓存一次、一个每个门都查**
        Assert.True(MapManagerMutateCore.FindMapCachedVersusPerItem());
        Assert.True(MapManagerMutateCore.FindMapCallCountsDiffer());
        Assert.Equal(1, MapManagerMutateCore.FindMapCalls("sName,sSMapNO", 5));
        Assert.Equal(5, MapManagerMutateCore.FindMapCalls("sName", 5));
    }

    [Fact]
    public void NilHandlingDiffers()
    {
        Assert.True(MapManagerMutateCore.NilHandlingDiffersAcrossOverloads());
        Assert.True(MapManagerMutateCore.NilBehavioursAllDiffer());
        Assert.True(MapManagerMutateCore.EnvirOverloadDerefsWithoutNilCheck());

        Assert.Equal("直接崩溃", MapManagerMutateCore.NilBehaviour("sName,Envir"));
    }

    // ===================== 五、DelMap =====================

    [Fact]
    public void DelMapLocks()
    {
        Assert.True(MapManagerMutateCore.DelMapOnlyLocks());
        Assert.True(MapManagerMutateCore.DelMapReferenceEquality());
    }

    [Fact]
    public void DelMapSemantics()
    {
        Assert.True(MapManagerMutateCore.DelMapSetsResultOnlyWhenDeleted());
        Assert.True(MapManagerMutateCore.DelMapValueEqualButDifferentRef());
        Assert.True(MapManagerMutateCore.DelMapDoesNotFree());
        Assert.True(MapManagerMutateCore.FreeHappensInDestroy());
    }

    // ===================== 六、5 参 AddMapRoute =====================

    [Fact]
    public void TodoComment()
    {
        // **内存泄露已修复、TODO 未删**
        Assert.True(MapManagerMutateCore.TodoMemoryLeakFix());
        Assert.True(MapManagerMutateCore.TodoCommentRemains());
        Assert.True(MapManagerMutateCore.IdeTodoFormat());
        Assert.True(MapManagerMutateCore.OldFormLackedElseFree());
        Assert.True(MapManagerMutateCore.NewFormAddsElseFree());
    }

    [Fact]
    public void FiveArgFields()
    {
        Assert.True(MapManagerMutateCore.FiveArgFillsSevenFields());
        Assert.True(MapManagerMutateCore.FiveArgFieldsCount());
        Assert.True(MapManagerMutateCore.FiveArgLeavesDefaults());
        Assert.True(MapManagerMutateCore.FiveArgKeepsBoCenterTrue());
        Assert.Equal(7, MapManagerMutateCore.FiveArgFields.Length);
    }

    [Fact]
    public void TwoConstructionSitesDiffer()
    {
        Assert.True(MapManagerMutateCore.TwoConstructionSitesDiffer());
        Assert.True(MapManagerMutateCore.ConstructionProfilesDiffer());
        Assert.True(MapManagerMutateCore.FiveArgNeedsBothMaps());
    }

    // ===================== 七、8 参 AddMapRoute =====================

    [Fact]
    public void RandThresholds()
    {
        // **探针实测边界：79/80 与 49/50/149/150**
        Assert.True(MapManagerMutateCore.GetRandXYTwoTierThresholds());
        Assert.True(MapManagerMutateCore.GetRandXYThreeTierThresholds());
        Assert.True(MapManagerMutateCore.StepVersusThresholdDimensions());

        Assert.Equal(3, MapManagerMutateCore.RandStep(79));
        Assert.Equal(10, MapManagerMutateCore.RandStep(80));
        Assert.Equal(2, MapManagerMutateCore.RandMargin(49));
        Assert.Equal(15, MapManagerMutateCore.RandMargin(50));
        Assert.Equal(15, MapManagerMutateCore.RandMargin(149));
        Assert.Equal(50, MapManagerMutateCore.RandMargin(150));
    }

    [Fact]
    public void SixCombinations()
    {
        Assert.True(MapManagerMutateCore.SixCombinations());
    }

    [Fact]
    public void NyAdjustNestedInNxElse()
    {
        // **nX 未触边界时 nY 完全不动**
        Assert.True(MapManagerMutateCore.NyAdjustNestedInNxElse());
        Assert.True(MapManagerMutateCore.NyUntouchedWhenNxAdvances());
        Assert.True(MapManagerMutateCore.NyMovesWhenNxResets());
    }

    [Fact]
    public void OneGridMargin()
    {
        Assert.True(MapManagerMutateCore.OneGridMargin());
        Assert.True(MapManagerMutateCore.MarginIsWidthMinusMarginMinusOne());
    }

    [Fact]
    public void RetryLimit()
    {
        Assert.True(MapManagerMutateCore.RetryLimit201());
        Assert.True(MapManagerMutateCore.RetryBreaksAt201());
        Assert.True(MapManagerMutateCore.FailureKeepsLastValues());
    }

    [Fact]
    public void NegativeCoordTriggersRandom()
    {
        Assert.True(MapManagerMutateCore.NegativeCoordConditionIsOr());
        Assert.True(MapManagerMutateCore.FailedRandomKeepsNegative());
    }

    [Fact]
    public void GatePlacement()
    {
        Assert.True(MapManagerMutateCore.DeletesSameNameRouteFirst());
        Assert.True(MapManagerMutateCore.TargetMustBeWalkable());
        Assert.True(MapManagerMutateCore.SquareAreaOfGates());
        Assert.True(MapManagerMutateCore.GateCountFormula());
        Assert.Equal(9, MapManagerMutateCore.MaxGateCount(1));
        Assert.Equal(25, MapManagerMutateCore.MaxGateCount(2));
        Assert.Equal(1, MapManagerMutateCore.MaxGateCount(0));
    }

    [Fact]
    public void OnlyCenterKeepsBoCenterTrue()
    {
        Assert.True(MapManagerMutateCore.OnlyCenterKeepsBoCenterTrue());
        Assert.True(MapManagerMutateCore.CenterFlagValues());
    }

    [Fact]
    public void EightArgFields()
    {
        Assert.True(MapManagerMutateCore.EightArgFieldsCount());
        Assert.Equal(9, MapManagerMutateCore.EightArgFields.Length);
        Assert.Contains("m_boFlag", MapManagerMutateCore.EightArgFields);
        Assert.Contains("m_DEnvir", MapManagerMutateCore.EightArgFields);
    }

    [Fact]
    public void RunTimeFields()
    {
        Assert.True(MapManagerMutateCore.RunTickUsesLongWordCast());
        Assert.True(MapManagerMutateCore.RunTimeConversion());
        Assert.Equal(5000, MapManagerMutateCore.RunTimeFromSeconds(5));
    }

    [Fact]
    public void DoorIndexAndAppearance()
    {
        // **区间闭、外观 +53 恰好落在传送门区间 [54..58]**
        Assert.True(MapManagerMutateCore.DoorIndexRangeOneToFive());
        Assert.True(MapManagerMutateCore.AppearanceMatchesPortalRange());

        Assert.Equal(54, MapManagerMutateCore.MerchantAppearance(1));
        Assert.Equal(58, MapManagerMutateCore.MerchantAppearance(5));
        Assert.False(MapManagerMutateCore.DoorIndexInRange(0));
        Assert.False(MapManagerMutateCore.DoorIndexInRange(6));
    }

    [Fact]
    public void MerchantBinding()
    {
        Assert.True(MapManagerMutateCore.MerchantBoundToGate());
        Assert.True(MapManagerMutateCore.MerchantFieldsCount());
        Assert.True(MapManagerMutateCore.AddThenInitialize());
        Assert.True(MapManagerMutateCore.MerchantCallOrderValues());
        Assert.Equal(11, MapManagerMutateCore.MerchantAssignedFieldCount());
    }

    [Fact]
    public void TwoDeletionOrders()
    {
        // **同名方法先删格、内置例程直接删列表**
        Assert.True(MapManagerMutateCore.DeleteMapGateSkipsMapCell());
        Assert.True(MapManagerMutateCore.TwoDeletionOrders());
        Assert.True(MapManagerMutateCore.DeletionOrdersDiffer());
        Assert.True(MapManagerMutateCore.DeleteMapGateShape());
        Assert.True(MapManagerMutateCore.DeleteMapGateValues());
    }

    [Fact]
    public void NestedRoutines()
    {
        Assert.True(MapManagerMutateCore.TwoNestedRoutines());
        Assert.Equal(new[] { "GetRandXY", "DeleteMapGate" }, MapManagerMutateCore.NestedRoutines);
    }

    // ===================== 行数 =====================

    [Fact]
    public void LineCounts()
    {
        Assert.True(MapManagerMutateCore.NineMethods());
        Assert.Equal(new[] { 31, 28, 39, 51, 24, 21, 21, 19, 147 }, MapManagerMutateCore.MethodLineCounts);
    }

    [Fact]
    public void LongestAndShortest()
    {
        Assert.True(MapManagerMutateCore.EightArgIsLongest());
        Assert.True(MapManagerMutateCore.EnvirOverloadIsShortest());
    }

    [Fact]
    public void TotalLines()
    {
        Assert.True(MapManagerMutateCore.TotalLinesValues());
        Assert.Equal(381, MapManagerMutateCore.TotalLines());
    }
}
