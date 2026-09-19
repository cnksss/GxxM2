using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J158：`TEnvirnoment` 地图格与运行接口 1:1 测试。
/// **列优先索引、两个 `GetDropPosition` 的五处差异、`Run` 的节流与两分支
/// 全部经探针实测（含两种索引序重合判据的完整推导与枚举）。**
/// </summary>
public sealed class EnvirMapCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.True(EnvirMapCore.ConstantsMatchSource());
        Assert.Equal(1000, EnvirMapCore.RunThrottleMs);
        Assert.Equal(320, EnvirMapCore.ShakeIntervalMs);
        Assert.Equal(999, EnvirMapCore.ItemCountSentinel);
    }

    // ===================== 一、GetMapCellInfo 列优先索引 =====================

    [Fact]
    public void ColumnMajorIndexing()
    {
        // **探针实测 (1,0) 列优先得 10、行优先得 1**
        Assert.True(EnvirMapCore.ColumnMajorIndexing());
        Assert.Equal(10, EnvirMapCore.CellIndex(1, 0, 5, 10));
        Assert.Equal(1, EnvirMapCore.RowMajorIndex(1, 0, 5, 10));
    }

    [Fact]
    public void IndexFormula()
    {
        // **换列偏移整个高度、换行只偏移 1**
        Assert.True(EnvirMapCore.IndexFormula());
        Assert.True(EnvirMapCore.SameColumnContiguous());
        Assert.True(EnvirMapCore.IndexIsBijection());
    }

    [Fact]
    public void NotRowMajor()
    {
        Assert.True(EnvirMapCore.NotRowMajor());
        Assert.True(EnvirMapCore.DiagonalCoincidesWhenSquare());
    }

    [Fact]
    public void CoincidenceCriterion()
    {
        // **两种序重合 ⟺ nX*(h-1) == nY*(w-1)**
        Assert.True(EnvirMapCore.CriterionMatchesIndexEquality());
        Assert.True(EnvirMapCore.CoincidenceCriterion(0, 0, 5, 10));
        Assert.True(EnvirMapCore.CoincidenceCriterion(4, 9, 5, 10));
        Assert.False(EnvirMapCore.CoincidenceCriterion(1, 1, 5, 10));
    }

    [Fact]
    public void TwoCoincidentPointsInRectangle()
    {
        // **5×10 时恰好两个重合点：原点与最远角 (4,9)**
        Assert.True(EnvirMapCore.TwoCoincidentPointsInRectangle());
        Assert.True(EnvirMapCore.FarCornerIsLastIndex());
        Assert.True(EnvirMapCore.OriginCoincidesAlways());
    }

    [Fact]
    public void DiffersEverywhereWhenRectangular()
    {
        Assert.True(EnvirMapCore.DiffersEverywhereWhenRectangular());
    }

    [Fact]
    public void BoundsChecks()
    {
        Assert.True(EnvirMapCore.BoundsInclusiveLowerExclusiveUpper());
        Assert.True(EnvirMapCore.NegativeRejected());
        Assert.True(EnvirMapCore.NullArrayRejected());
        Assert.True(EnvirMapCore.WidthWithXHeightWithY());
    }

    [Fact]
    public void FailureLeavesOutputUntouched()
    {
        Assert.True(EnvirMapCore.FailureLeavesOutputUntouched());
        Assert.True(EnvirMapCore.CommentedIndexMatchesLive());
        Assert.True(EnvirMapCore.CommentedDiagnostics());
    }

    // ===================== 二、Initialize =====================

    [Fact]
    public void InitializeGuards()
    {
        // **两个维度都要严格大于 1**
        Assert.True(EnvirMapCore.InitializeGuardValues());
        Assert.True(EnvirMapCore.InitializeGuardsBothDimensions(2, 2));
        Assert.False(EnvirMapCore.InitializeGuardsBothDimensions(1, 2));
    }

    [Fact]
    public void AllocOrder()
    {
        // **必须先赋尺寸再分配，否则会按旧尺寸分配**
        Assert.True(EnvirMapCore.OldSizeFreesNewSizeAllocs());
        Assert.True(EnvirMapCore.AssignSizeBeforeAlloc());
        Assert.True(EnvirMapCore.AllocOrderMatters());
        Assert.True(EnvirMapCore.AllocBytesFormula());
    }

    [Fact]
    public void InitializeLifecycle()
    {
        Assert.True(EnvirMapCore.CleanupUsesAccessor());
        Assert.True(EnvirMapCore.InvalidSizeLeavesEverything());
        Assert.True(EnvirMapCore.BoInitializeLifecycleValues());
        Assert.True(EnvirMapCore.BoInitializeLifecycle(true));
        Assert.False(EnvirMapCore.BoInitializeLifecycle(false));
    }

    [Fact]
    public void ObjListReleaseStyles()
    {
        Assert.True(EnvirMapCore.TwoObjListReleaseStyles());
        Assert.True(EnvirMapCore.ReleaseStylesEquivalent());
        Assert.Equal(2, EnvirMapCore.ObjListReleaseStyles.Length);
    }

    // ===================== 三、三个小方法 =====================

    [Fact]
    public void AddDoorToMap()
    {
        // **用目标地图坐标 m_nMapX/m_nMapY**
        Assert.True(EnvirMapCore.DoorUsesTargetCoords());
        Assert.True(EnvirMapCore.DoorUsesSecondPair());
        Assert.Equal(2, EnvirMapCore.GateCoordPairs.Length);
    }

    [Fact]
    public void AddHumBBCount()
    {
        // **加号无保护、减号有保护**
        Assert.True(EnvirMapCore.AddHumBBCountAsymmetricGuard());
        Assert.True(EnvirMapCore.MonCountNeverNegative());
        Assert.True(EnvirMapCore.AddHumBBCountReasonPresent());
    }

    [Fact]
    public void AddHumBBCountValues()
    {
        var (h, m) = EnvirMapCore.AddHumBBCount(0, 5);

        Assert.Equal(1, h);
        Assert.Equal(4, m);
    }

    [Fact]
    public void CanAddToMapPosition()
    {
        Assert.True(EnvirMapCore.CanAddValues());
        Assert.True(EnvirMapCore.CanAddGuardsInvalid(false));
        Assert.False(EnvirMapCore.CanAddGuardsInvalid(true));
        Assert.True(EnvirMapCore.CanAddRequiresZeroChFlag());
    }

    // ===================== 四、两个 GetDropPosition =====================

    [Fact]
    public void FourTripleLoops()
    {
        Assert.True(EnvirMapCore.FourNearIdenticalTripleLoops());
        Assert.Equal(4, EnvirMapCore.TripleLoopCopies());
    }

    [Fact]
    public void SquareLayerExpansion()
    {
        Assert.True(EnvirMapCore.SquareLayerExpansion());
        Assert.True(EnvirMapCore.LayerCellCountValues());
        Assert.Equal(9, EnvirMapCore.LayerCellCount(1));
        Assert.Equal(25, EnvirMapCore.LayerCellCount(2));
    }

    [Fact]
    public void InefficientReprocessing()
    {
        // **探针实测：3 层共 83 次访问、唯一格子只有 49 个**
        Assert.True(EnvirMapCore.InefficientReprocessing());
        Assert.True(EnvirMapCore.ReprocessingCounts());
        Assert.Equal(83, EnvirMapCore.TotalVisits(3));
    }

    [Fact]
    public void LayerOrder()
    {
        Assert.True(EnvirMapCore.LayerOrder());
        Assert.True(EnvirMapCore.SecondIsYThirdIsX());

        var (dx, dy) = EnvirMapCore.Offset(3, -2);

        Assert.Equal(3, dx);
        Assert.Equal(-2, dy);
    }

    [Fact]
    public void FiveDifferences()
    {
        // **全函数仅 5 处实质不同（逐行文本对比得出）**
        Assert.True(EnvirMapCore.OnlyFiveDifferences());
        Assert.True(EnvirMapCore.DifferencesCount());
        Assert.Equal(5, EnvirMapCore.Differences.Length);
    }

    [Fact]
    public void OriginExclusionSites()
    {
        // **四处候选里出现三次、唯独漏一处**
        Assert.True(EnvirMapCore.OriginExclusionPresentThreeOfFourSites());
        Assert.True(EnvirMapCore.OneSiteMissingOriginExclusion());
        Assert.Equal(3, EnvirMapCore.SitesWithExclusion());
        Assert.Equal(4, EnvirMapCore.TotalCandidateSites());
        Assert.True(EnvirMapCore.MissingSiteIsInFirstFunction());
    }

    [Fact]
    public void OriginExclusionHelper()
    {
        Assert.False(EnvirMapCore.NotOrigin(5, 5, 5, 5));
        Assert.True(EnvirMapCore.NotOrigin(5, 5, 6, 5));
        Assert.True(EnvirMapCore.NotOrigin(5, 5, 5, 6));
    }

    [Fact]
    public void Thresholds()
    {
        // **8 与 20、且不是简单新旧关系**
        Assert.True(EnvirMapCore.ThresholdDiffersEightVersusTwenty());
        Assert.Equal(8, EnvirMapCore.DropThreshold1);
        Assert.Equal(20, EnvirMapCore.DropThreshold2);
        Assert.True(EnvirMapCore.NotSimpleOldNewRelation());
        Assert.True(EnvirMapCore.NotSimpleOldNewReason());
    }

    [Fact]
    public void Sentinel()
    {
        Assert.True(EnvirMapCore.Sentinel999());
        Assert.True(EnvirMapCore.SentinelMustExceedReal());
    }

    [Fact]
    public void BreakChain()
    {
        Assert.True(EnvirMapCore.BreakChainStyle());
        Assert.True(EnvirMapCore.ThreeBreakLevels());
        Assert.Equal(3, EnvirMapCore.BreakChainLevels.Length);
    }

    [Fact]
    public void ImmediateSuccess()
    {
        Assert.True(EnvirMapCore.ImmediateSuccessValues());
        Assert.True(EnvirMapCore.ImmediateSuccessOnEmptyCell(true, true));
        Assert.False(EnvirMapCore.ImmediateSuccessOnEmptyCell(true, false));
        Assert.False(EnvirMapCore.ImmediateSuccessOnEmptyCell(false, true));
    }

    [Fact]
    public void TrackFewest()
    {
        Assert.True(EnvirMapCore.TrackFewestStrict());
        Assert.True(EnvirMapCore.TrackFewestItems(5, 4));
        Assert.False(EnvirMapCore.TrackFewestItems(5, 5));
    }

    [Fact]
    public void FallbackToOrigin()
    {
        Assert.True(EnvirMapCore.FallbackToOrigin());
        Assert.True(EnvirMapCore.ThresholdStrictLess());
    }

    [Fact]
    public void ResultFalseOnBestEffortPath()
    {
        // **"用最优坐标"这条路径 Result 仍是假**
        Assert.True(EnvirMapCore.ResultFalseOnBestEffortPath());
        Assert.True(EnvirMapCore.ResultSemantics());
        Assert.True(EnvirMapCore.CallerMustCompareCoords());
    }

    [Fact]
    public void MagicNames()
    {
        Assert.True(EnvirMapCore.ThreeMagicNames());
        Assert.True(EnvirMapCore.OriginDateCommentPresent());
        Assert.True(EnvirMapCore.BothResetState());
        Assert.True(EnvirMapCore.ThreeResetVariables());
    }

    // ===================== 五、Run =====================

    [Fact]
    public void Throttle()
    {
        // **恰好 1000 毫秒仍然跳过**
        Assert.True(EnvirMapCore.ThrottleOneThousandMs());
        Assert.True(EnvirMapCore.ThrottleStrictlyGreaterToProceed());
        Assert.True(EnvirMapCore.ThrottleSkips(1000, 0));
        Assert.False(EnvirMapCore.ThrottleSkips(1001, 0));
        Assert.True(EnvirMapCore.TimestampWrittenAfterThrottle());
        Assert.True(EnvirMapCore.TimestampOrder());
    }

    [Fact]
    public void Weather()
    {
        Assert.True(EnvirMapCore.WeatherArrayBounds());
        Assert.Equal(10, EnvirMapCore.WeatherArrayLength());
        Assert.True(EnvirMapCore.WeatherExpiryStrictGreater());
        Assert.True(EnvirMapCore.WeatherNotUsedNeverExpires());
        Assert.True(EnvirMapCore.WeatherChangedOnlyWhenChanged());
    }

    [Fact]
    public void WeatherArrayReplacesThreeCopies()
    {
        // **旧版三份重复被数组循环取代 —— 少见的主动去重复**
        Assert.True(EnvirMapCore.OldWeatherLogicCommented());
        Assert.True(EnvirMapCore.ThreeOldWeatherEffects());
        Assert.True(EnvirMapCore.ArrayLoopReplacedThreeCopies());
        Assert.True(EnvirMapCore.RareDeduplication());
        Assert.Equal(3, EnvirMapCore.OldWeatherEffects.Length);
    }

    [Fact]
    public void ShakeIteration()
    {
        Assert.True(EnvirMapCore.ShakeReverseIteration());
        Assert.True(EnvirMapCore.ShakeReverseIsSafe());
        Assert.True(EnvirMapCore.ShakeCompletionValues());
        Assert.True(EnvirMapCore.ShakeCountCompletion(3, 3));
        Assert.False(EnvirMapCore.ShakeCountCompletion(2, 3));
    }

    [Fact]
    public void ShakeInterval()
    {
        // **严格小于 320 才跳过（恰好 320 不跳过）**
        Assert.True(EnvirMapCore.ShakeIntervalStrictLess());
        Assert.True(EnvirMapCore.ShakeSkipInterval320(319, 0));
        Assert.False(EnvirMapCore.ShakeSkipInterval320(320, 0));
    }

    [Fact]
    public void ShakePlayerLookup()
    {
        Assert.True(EnvirMapCore.ShakePlayerLookup("Alice"));
        Assert.False(EnvirMapCore.ShakePlayerLookup(""));
        Assert.True(EnvirMapCore.ShakeRemoveValues());
    }

    [Fact]
    public void ShakeBranches()
    {
        // **全图优先于范围**
        Assert.True(EnvirMapCore.AllShakeBranch(""));
        Assert.False(EnvirMapCore.AllShakeBranch("Alice"));
        Assert.True(EnvirMapCore.AllShakeTakesPrecedence());
        Assert.True(EnvirMapCore.ShakeBranchValues());
        Assert.True(EnvirMapCore.EmptyTempListSkipsRangeBranch());
    }

    [Fact]
    public void FiveConditionFilter()
    {
        // **32 组全枚举零差异**
        Assert.True(EnvirMapCore.FiveConditionFilterDuplicated());
        Assert.True(EnvirMapCore.FiveConditionsCount());
        Assert.True(EnvirMapCore.AllFiveConditionsRequired());
        Assert.True(EnvirMapCore.OnlyAllTruePasses());
        Assert.Equal(5, EnvirMapCore.FiveConditions.Length);
    }

    [Fact]
    public void SecondCopyPaste()
    {
        Assert.True(EnvirMapCore.SecondCopyPasteInSameFunction());
        Assert.True(EnvirMapCore.TempList2Reused());
        Assert.True(EnvirMapCore.ReuseVersusAllocate());
        Assert.True(EnvirMapCore.PlayerVariableReused());
        Assert.True(EnvirMapCore.PlayerReuseIsHarmless());
        Assert.True(EnvirMapCore.PlayerReuseReason());
    }

    [Fact]
    public void Continues()
    {
        // **三处 Continue、都发生在倒序遍历里**
        Assert.True(EnvirMapCore.TwoContinuesInReverseLoop());
        Assert.True(EnvirMapCore.TwoContinueReasons());
        Assert.True(EnvirMapCore.ThirdContinue());
        Assert.True(EnvirMapCore.ThreeContinues());
        Assert.Equal(3, EnvirMapCore.ContinueCount());
    }

    [Fact]
    public void AdvanceOnlyWhenNotSkipped()
    {
        Assert.True(EnvirMapCore.CountOnlyAdvancedWhenNotSkipped());
        Assert.True(EnvirMapCore.AdvanceOnlyWhenNotSkipped());
    }

    [Fact]
    public void BranchLocking()
    {
        // **全图分支带锁 56、范围分支不加锁**
        Assert.True(EnvirMapCore.AllShakeBranchLocks());
        Assert.Equal(56, EnvirMapCore.AllShakeLockId());
        Assert.True(EnvirMapCore.RangeBranchNoLock());
        Assert.True(EnvirMapCore.BranchLockDiffers());
    }

    [Fact]
    public void CleanupAndGuardian()
    {
        Assert.True(EnvirMapCore.ShakeCleanupOrder());
        Assert.True(EnvirMapCore.ThreeCleanupSteps());
        Assert.Equal(3, EnvirMapCore.CleanupSteps.Length);
        Assert.True(EnvirMapCore.GuardianCallAlwaysRuns());
        Assert.True(EnvirMapCore.GuardianCallAfterBothBranches());
        Assert.True(EnvirMapCore.FinalStepUnconditional());
    }

    [Fact]
    public void GuardianMisspelling()
    {
        // **拼错但声明与调用两处一致**
        Assert.True(EnvirMapCore.GuardianMethodNameMisspelled());
        Assert.True(EnvirMapCore.GuardianCorrectSpelling());
        Assert.True(EnvirMapCore.MisspellingIsConsistent());
        Assert.Equal("PorcessGuardianLevelInfo", EnvirMapCore.GuardianMethodName);
        Assert.Equal("ProcessGuardianLevelInfo", EnvirMapCore.GuardianMethodNameCorrect);
    }

    // ===================== 行数 =====================

    [Fact]
    public void LineCounts()
    {
        Assert.True(EnvirMapCore.EightMethods());
        Assert.Equal(new[] { 12, 90, 92, 13, 8, 11, 36, 140 }, EnvirMapCore.MethodLineCounts);
        Assert.True(EnvirMapCore.RunIsLongest());
        Assert.True(EnvirMapCore.AddHumBBCountIsShortest());
        Assert.True(EnvirMapCore.TotalLinesValues());
        Assert.Equal(402, EnvirMapCore.TotalLines());
    }

    [Fact]
    public void ClosestCopyPastePair()
    {
        // **90 与 92 —— 本工程最接近的复制粘贴对**
        Assert.True(EnvirMapCore.TwoDropPositionsSimilarLength());
        Assert.True(EnvirMapCore.ClosestCopyPastePair());
    }
}
