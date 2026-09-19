using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J173：`TEnvirnoment` 掉落位置推算 1:1 测试。
/// **兜底分支"改输出不改返回值"用模型固证、
/// 扫描次数逐层枚举、原点守卫零处对三处、
/// 阈值八与二十的严格小于边界。**
/// </summary>
public sealed class EnvirDropPositionCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(999, EnvirDropPositionCore.InitialMinCount);
        Assert.Equal(8, EnvirDropPositionCore.Threshold1);
        Assert.Equal(20, EnvirDropPositionCore.Threshold2);
    }

    // ===================== 一、骨架 =====================

    [Fact]
    public void Skeleton()
    {
        Assert.True(EnvirDropPositionCore.SameSkeleton());
        Assert.True(EnvirDropPositionCore.FourDifferences());
        Assert.True(EnvirDropPositionCore.FourDifferenceEntries());
        Assert.Equal(4, EnvirDropPositionCore.Differences.Length);
    }

    [Fact]
    public void Differences()
    {
        Assert.True(EnvirDropPositionCore.OriginGuardZeroVsThree());
        Assert.True(EnvirDropPositionCore.FirstPassItemBranchHasNoGuardInEither());
        Assert.True(EnvirDropPositionCore.ThresholdDiffers());
        Assert.Equal(0, EnvirDropPositionCore.OriginGuardCount1());
        Assert.Equal(3, EnvirDropPositionCore.OriginGuardCount2());
    }

    [Fact]
    public void Shape()
    {
        Assert.True(EnvirDropPositionCore.TwoPassSkeleton());
        Assert.True(EnvirDropPositionCore.SquareNotRing());
        Assert.True(EnvirDropPositionCore.InnerCellsRescanned());
        Assert.True(EnvirDropPositionCore.CandidateKeepsFewestItems());
    }

    // ---------- 扫描次数 ----------

    [Fact]
    public void RingCells()
    {
        Assert.Equal(9, EnvirDropPositionCore.RingCells(1));
        Assert.Equal(25, EnvirDropPositionCore.RingCells(2));
        Assert.Equal(49, EnvirDropPositionCore.RingCells(3));
    }

    [Fact]
    public void RadiusOne()
    {
        Assert.True(EnvirDropPositionCore.RadiusOneIs9());
        Assert.Equal(9, EnvirDropPositionCore.TotalScans(1));
        Assert.Equal(8, EnvirDropPositionCore.UniqueCells(1));
    }

    [Fact]
    public void RadiusTwo()
    {
        Assert.True(EnvirDropPositionCore.RadiusTwoIs34());
        Assert.Equal(34, EnvirDropPositionCore.TotalScans(2));
        Assert.Equal(24, EnvirDropPositionCore.UniqueCells(2));
    }

    [Fact]
    public void RadiusThree()
    {
        // **八十三（我最初误算成七十三）**
        Assert.True(EnvirDropPositionCore.RadiusThreeIs83());
        Assert.Equal(83, EnvirDropPositionCore.TotalScans(3));
        Assert.Equal(48, EnvirDropPositionCore.UniqueCells(3));
    }

    [Fact]
    public void ScanFormulas()
    {
        Assert.True(EnvirDropPositionCore.TotalScansFormula());
        Assert.True(EnvirDropPositionCore.ScansExceedCells());
        Assert.True(EnvirDropPositionCore.ZeroRangeScansNothing());
        Assert.Equal(0, EnvirDropPositionCore.TotalScans(0));
    }

    // ===================== 二、成功判据 =====================

    [Fact]
    public void SuccessSemantics()
    {
        Assert.True(EnvirDropPositionCore.SuccessNeedsBothEmptyAndBo2C());
        Assert.True(EnvirDropPositionCore.EmptyButBlockedRejected());
        Assert.True(EnvirDropPositionCore.TwoIndependentConditions());
    }

    [Fact]
    public void SuccessBranchValues()
    {
        // **两个条件缺一不可**
        Assert.True(EnvirDropPositionCore.SuccessBranchValues());

        Assert.True(EnvirDropPositionCore.SuccessBranch(true, true, false, false));
        Assert.False(EnvirDropPositionCore.SuccessBranch(true, false, false, false));
        Assert.False(EnvirDropPositionCore.SuccessBranch(false, true, false, false));
        Assert.False(EnvirDropPositionCore.SuccessBranch(false, false, false, false));
    }

    [Fact]
    public void OriginGuardRejects()
    {
        Assert.True(EnvirDropPositionCore.OriginGuardRejectsOrigin());
        Assert.False(EnvirDropPositionCore.SuccessBranch(true, true, true, true));
        Assert.True(EnvirDropPositionCore.SuccessBranch(true, true, true, false));
    }

    [Fact]
    public void Bo2CReads()
    {
        Assert.True(EnvirDropPositionCore.Bo2CReadPerCell());
        Assert.True(EnvirDropPositionCore.NoCrossCellStale());
        Assert.True(EnvirDropPositionCore.ShortCircuitSingleRead());
    }

    // ===================== 三、兜底契约缺陷 =====================

    [Fact]
    public void FallbackDefect()
    {
        // **兜底改输出却不改返回值**
        Assert.True(EnvirDropPositionCore.CandidateNotRechecked());
        Assert.True(EnvirDropPositionCore.FallbackLeavesResultFalse());
        Assert.True(EnvirDropPositionCore.SuccessFlagNeverSetInFallback());
        Assert.True(EnvirDropPositionCore.CallerMayMisread());
        Assert.True(EnvirDropPositionCore.OutputValidButReturnsFalse());
    }

    [Fact]
    public void FallbackResultAlwaysFalse()
    {
        Assert.True(EnvirDropPositionCore.FallbackResultAlwaysFalse());

        var a = EnvirDropPositionCore.Fallback(false, 3, 7, 8, 5, 5, 8);
        Assert.False(a.Result);
    }

    [Fact]
    public void FallbackWritesOutputs()
    {
        Assert.True(EnvirDropPositionCore.FallbackWritesCandidate());
        Assert.True(EnvirDropPositionCore.FallbackWritesOrigin());
        Assert.True(EnvirDropPositionCore.OutputsAlwaysDefined());
    }

    [Fact]
    public void FallbackModel()
    {
        // 候选合格 → 输出候选格、结果仍假
        var ok = EnvirDropPositionCore.Fallback(false, 3, 7, 8, 5, 5, 8);
        Assert.False(ok.Result);
        Assert.Equal(7, ok.X);
        Assert.Equal(8, ok.Y);

        // 候选不合格 → 输出原点
        var bad = EnvirDropPositionCore.Fallback(false, 900, 7, 8, 5, 5, 8);
        Assert.False(bad.Result);
        Assert.Equal(5, bad.X);
        Assert.Equal(5, bad.Y);
    }

    // ---------- 阈值 ----------

    [Fact]
    public void Thresholds()
    {
        Assert.True(EnvirDropPositionCore.ThresholdIsStrictLess());
        Assert.True(EnvirDropPositionCore.EightMeansAtMostSeven());
        Assert.True(EnvirDropPositionCore.TwentyMeansAtMostNineteen());
        Assert.True(EnvirDropPositionCore.ThresholdBoundaries());
        Assert.True(EnvirDropPositionCore.ThresholdGapIsTwelve());
        Assert.Equal(12, EnvirDropPositionCore.Threshold2 - EnvirDropPositionCore.Threshold1);
    }

    [Fact]
    public void ThresholdEdge()
    {
        // 七通过、八不通过（阈值八）
        Assert.Equal(7, EnvirDropPositionCore.Fallback(false, 7, 7, 8, 5, 5, 8).X);
        Assert.Equal(5, EnvirDropPositionCore.Fallback(false, 8, 7, 8, 5, 5, 8).X);

        // 十九通过、二十不通过（阈值二十）
        Assert.Equal(7, EnvirDropPositionCore.Fallback(false, 19, 7, 8, 5, 5, 20).X);
        Assert.Equal(5, EnvirDropPositionCore.Fallback(false, 20, 7, 8, 5, 5, 20).X);
    }

    [Fact]
    public void NothingFound()
    {
        Assert.True(EnvirDropPositionCore.NothingFoundGivesOrigin());
        Assert.True(EnvirDropPositionCore.IndistinguishableOutcomes());
        Assert.True(EnvirDropPositionCore.BothCollapseToOrigin());
        Assert.True(EnvirDropPositionCore.InitialCountExceedsBothThresholds());
        Assert.True(EnvirDropPositionCore.FirstCandidateAlwaysUpdates());
    }

    // ===================== 四、原点守卫 =====================

    [Fact]
    public void OriginScanning()
    {
        Assert.True(EnvirDropPositionCore.OriginAlwaysScanned());
        Assert.True(EnvirDropPositionCore.OriginAtRangeOne());
        Assert.True(EnvirDropPositionCore.EnumeratesOrigin(1));
        Assert.False(EnvirDropPositionCore.EnumeratesOrigin(0));
    }

    [Fact]
    public void OriginIntent()
    {
        Assert.True(EnvirDropPositionCore.OriginAllowedInFirst());
        Assert.True(EnvirDropPositionCore.LaterRejectedOrigin());
        Assert.True(EnvirDropPositionCore.IntentChanged());
    }

    // ===================== 五、两趟两策略 =====================

    [Fact]
    public void TwoPasses()
    {
        Assert.True(EnvirDropPositionCore.FirstPassStrictSecondLoose());
        Assert.True(EnvirDropPositionCore.MatchesJ169Policies());
        Assert.True(EnvirDropPositionCore.TwoPassFunctions());
        Assert.True(EnvirDropPositionCore.TwoStageSearch());
        Assert.Equal(2, EnvirDropPositionCore.PassFunctions.Length);
    }

    [Fact]
    public void SecondPassConditional()
    {
        Assert.True(EnvirDropPositionCore.SecondPassOnlyIfFirstFails());
        Assert.True(EnvirDropPositionCore.FirstSuccessSkipsSecond());
        Assert.False(EnvirDropPositionCore.SecondPassRuns(true));
        Assert.True(EnvirDropPositionCore.SecondPassRuns(false));
        Assert.True(EnvirDropPositionCore.SecondPassResetsCandidate());
    }

    // ===================== 六、性能 =====================

    [Fact]
    public void Locking()
    {
        Assert.True(EnvirDropPositionCore.LockPerCellAgain());
        Assert.True(EnvirDropPositionCore.RadiusTwoTwoPassesIs68());
        Assert.True(EnvirDropPositionCore.RadiusThreeTwoPassesIs166());
        Assert.True(EnvirDropPositionCore.WorstCaseIsTwoPasses());
        Assert.True(EnvirDropPositionCore.LockCyclesValues());
        Assert.Equal(68, EnvirDropPositionCore.TotalScans(2) * 2);
    }

    [Fact]
    public void LockCyclesModel()
    {
        Assert.Equal(83, EnvirDropPositionCore.LockCycles(3, true));
        Assert.Equal(166, EnvirDropPositionCore.LockCycles(3, false));
        Assert.Equal(9, EnvirDropPositionCore.LockCycles(1, true));
        Assert.Equal(18, EnvirDropPositionCore.LockCycles(1, false));
    }

    // ===================== 七、遗留注释与异常 =====================

    [Fact]
    public void DateComments()
    {
        Assert.Equal(4, EnvirDropPositionCore.DateCommentCount());
        Assert.True(EnvirDropPositionCore.FourDateComments());
        Assert.True(EnvirDropPositionCore.DateCommentShape());
        Assert.Contains("09/10", EnvirDropPositionCore.DateComment, StringComparison.Ordinal);
    }

    [Fact]
    public void NoExceptionHandling()
    {
        Assert.True(EnvirDropPositionCore.NoTryExcept());
    }

    // ===================== 行数 =====================

    [Fact]
    public void LineCounts()
    {
        Assert.True(EnvirDropPositionCore.TwoMethods());
        Assert.Equal(2, EnvirDropPositionCore.MethodLineCounts.Length);
        Assert.Equal(new[] { 89, 91 }, EnvirDropPositionCore.MethodLineCounts);
    }

    [Fact]
    public void LengthComparison()
    {
        Assert.True(EnvirDropPositionCore.SecondIsLonger());
        Assert.True(EnvirDropPositionCore.SpreadIsTwo());
        Assert.Equal(2, EnvirDropPositionCore.LineSpread());
    }

    [Fact]
    public void Totals()
    {
        Assert.True(EnvirDropPositionCore.TotalLinesValues());
        Assert.Equal(180, EnvirDropPositionCore.TotalLines());
        Assert.True(EnvirDropPositionCore.ComparedToJ169());
    }
}
