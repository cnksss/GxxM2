using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J199：`ObjMon.pas` 中 `TChickenDeer` 1:1 测试（三方法合计 81 行）。
/// **本批是迄今单文件内缺陷密度最高的一批** —— 72 行的 `Run` 里有三处真实缺陷：
/// ① 1456 行同一表达式重复两次（纵轴条件被误写成横轴）；
/// ② 同一行用了循环泄漏变量 `BaseObject` 而非 `m_TargetCret`、空时必然空指针；
/// ③ `m_boRunAwayMode` 被反义使用（找到目标时置真、基类却把真当作"跳过移动"）。
/// </summary>
public sealed class ObjMonChickenDeerCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(1382, ObjMonChickenDeerCore.CreateStart);
        Assert.Equal(5, ObjMonChickenDeerCore.CreateLines);
        Assert.Equal(1388, ObjMonChickenDeerCore.DestroyStart);
        Assert.Equal(4, ObjMonChickenDeerCore.DestroyLines);
        Assert.Equal(1393, ObjMonChickenDeerCore.RunStart);
        Assert.Equal(1464, ObjMonChickenDeerCore.RunEnd);
        Assert.Equal(72, ObjMonChickenDeerCore.RunLines);
        Assert.Equal(81, ObjMonChickenDeerCore.TotalLines);
        Assert.Equal(25, ObjMonChickenDeerCore.ClassDeclStart);
        Assert.Equal(30, ObjMonChickenDeerCore.ClassDeclEnd);
        Assert.Equal(5, ObjMonChickenDeerCore.ViewRange);
        Assert.Equal(9999, ObjMonChickenDeerCore.DistanceSeed);
        Assert.Equal(6, ObjMonChickenDeerCore.NearThreshold);
        Assert.Equal(5, ObjMonChickenDeerCore.NextPositionParam);
        Assert.Equal(0, ObjMonChickenDeerCore.RC_PLAYOBJECT);
        Assert.Equal(11, ObjMonChickenDeerCore.Bo554InMonster);
        Assert.Equal(182, ObjMonChickenDeerCore.Bo554InFox);
        Assert.Equal(821, ObjMonChickenDeerCore.RunAwayModeDeclLine);
        Assert.Equal(7, ObjMonChickenDeerCore.RunAwayAssignCount);
        Assert.Equal(1, ObjMonChickenDeerCore.RunAwayTrueCount);
        Assert.Equal(6, ObjMonChickenDeerCore.RunAwayFalseCount);
        Assert.Equal(189, ObjMonChickenDeerCore.RemainingImplCount);
        Assert.Equal(54, ObjMonChickenDeerCore.RemainingClassCount);
        Assert.Equal(1456, ObjMonChickenDeerCore.DuplicatedConditionLine);
        Assert.Equal(1444, ObjMonChickenDeerCore.RunAwayTrueLine);
        Assert.Equal(1191, ObjMonChickenDeerCore.BaseRunAwayGuardLine);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonChickenDeerCore.SpanMatches());
        Assert.True(ObjMonChickenDeerCore.TotalLinesAddUp());
        Assert.True(ObjMonChickenDeerCore.StartsAscending());
        Assert.True(ObjMonChickenDeerCore.WithinUnit());
        Assert.True(ObjMonChickenDeerCore.NoInstrumentation());
    }

    // ===================== 缺陷一：重复表达式 =====================

    [Fact]
    public void DuplicatedAxisCondition()
    {
        Assert.True(ObjMonChickenDeerCore.DuplicatedAxisCondition());
        Assert.True(ObjMonChickenDeerCore.XCountedTwice());
        Assert.True(ObjMonChickenDeerCore.YCountedZeroTimes());
        Assert.True(ObjMonChickenDeerCore.IntentWasBothAxes());
        Assert.True(ObjMonChickenDeerCore.ConditionEffectivelyOnlyX());
        Assert.True(ObjMonChickenDeerCore.DuplicatedLineExtracted());
    }

    [Fact]
    public void YConstraintIsLost()
    {
        Assert.True(ObjMonChickenDeerCore.YConstraintLost());
        Assert.True(ObjMonChickenDeerCore.AbsurdYStillPasses());
        Assert.True(ObjMonChickenDeerCore.FarXBothFail());
        Assert.True(ObjMonChickenDeerCore.BothNearBothPass());

        // **纵轴取极大值：误写版仍为真、本意版为假**
        Assert.True(ObjMonChickenDeerCore.WrittenCondition(0, 9999));
        Assert.False(ObjMonChickenDeerCore.IntendedCondition(0, 9999));

        // **横轴超出：两者都假**
        Assert.False(ObjMonChickenDeerCore.WrittenCondition(7, 0));
        Assert.False(ObjMonChickenDeerCore.IntendedCondition(7, 0));
    }

    // ===================== 缺陷二：循环泄漏变量 =====================

    [Fact]
    public void LeakedLoopVariable()
    {
        Assert.True(ObjMonChickenDeerCore.UsesLeakedLoopVariable());
        Assert.True(ObjMonChickenDeerCore.ShouldUseTargetCret());
        Assert.True(ObjMonChickenDeerCore.LoopVarSurvivesLastIteration());
        Assert.True(ObjMonChickenDeerCore.NilWhenNoVisibleActors());
        Assert.True(ObjMonChickenDeerCore.NullDerefWhenEmpty());
        Assert.True(ObjMonChickenDeerCore.NeighborsUseTargetCret());
        Assert.True(ObjMonChickenDeerCore.LeakedVarLineExtracted());
        Assert.True(ObjMonChickenDeerCore.Stage2HasNilGuard());
        Assert.True(ObjMonChickenDeerCore.Stage2InnerLacksGuard());
    }

    [Fact]
    public void LeakedVariableLeavesNull()
    {
        Assert.True(ObjMonChickenDeerCore.AllSkippedLeavesNull());
        Assert.True(ObjMonChickenDeerCore.EmptyListLeavesNull());
        Assert.True(ObjMonChickenDeerCore.LastValueSurvives());
        Assert.True(ObjMonChickenDeerCore.SkippedDoesNotUpdate());

        // **空列表残留 null => 必然空指针**
        Assert.Null(ObjMonChickenDeerCore.LeakedLoopVariableValue(new List<int?>()));

        // **全被 Continue 跳过也残留 null**
        Assert.Null(ObjMonChickenDeerCore.LeakedLoopVariableValue(
            new List<int?> { null, null }));

        // **正常遍历残留最后一个**
        Assert.Equal(3, ObjMonChickenDeerCore.LeakedLoopVariableValue(
            new List<int?> { 1, 2, 3 }));
    }

    // ===================== 缺陷三：反义使用 =====================

    [Fact]
    public void InvertedRunAwaySemantics()
    {
        Assert.True(ObjMonChickenDeerCore.InvertedRunAwaySemantics());
        Assert.True(ObjMonChickenDeerCore.SetTrueOnTargetFound());
        Assert.True(ObjMonChickenDeerCore.BaseSkipsMoveWhenTrue());
        Assert.True(ObjMonChickenDeerCore.OnlyTrueSiteIs1444());
        Assert.True(ObjMonChickenDeerCore.SixFalseSites());
        Assert.True(ObjMonChickenDeerCore.TwoIncompatibleMeanings());
        Assert.True(ObjMonChickenDeerCore.SevenAssignSites());
        Assert.True(ObjMonChickenDeerCore.AssignSitesExtracted());
        Assert.True(ObjMonChickenDeerCore.ExactlyOneTrue());
        Assert.True(ObjMonChickenDeerCore.ThreeTimeoutSites());
        Assert.True(ObjMonChickenDeerCore.TimeoutSitesExtracted());
        Assert.True(ObjMonChickenDeerCore.TrueSiteNotTimeout());
        Assert.True(ObjMonChickenDeerCore.BaseGuardLineExtracted());
        Assert.True(ObjMonChickenDeerCore.SemanticsConflict());
    }

    [Fact]
    public void AssignSites()
    {
        Assert.Equal(new[] { 769, 1075, 1332, 1444, 1449, 5517, 5911 },
            ObjMonChickenDeerCore.RunAwayAssignSites);
        Assert.Equal(new[] { 769, 5517, 5911 },
            ObjMonChickenDeerCore.RunAwayTimeoutSites);
    }

    [Fact]
    public void SemanticsAreOpposed()
    {
        // **基类：真 => 不移动**
        Assert.False(ObjMonChickenDeerCore.BaseProceedsWithMove(true));
        Assert.True(ObjMonChickenDeerCore.BaseProceedsWithMove(false));

        // **本类：真 => 已锁定目标**
        Assert.True(ObjMonChickenDeerCore.HasLockedTarget(true));

        // **两者在真值上冲突**
        Assert.NotEqual(ObjMonChickenDeerCore.BaseProceedsWithMove(true),
            ObjMonChickenDeerCore.HasLockedTarget(true));
    }

    // ===================== 二、两段结构 =====================

    [Fact]
    public void TwoStageStructure()
    {
        Assert.True(ObjMonChickenDeerCore.TwoStagesSamePredicate());
        Assert.True(ObjMonChickenDeerCore.StageLinesExtracted());
        Assert.True(ObjMonChickenDeerCore.DelayResetTwice());
        Assert.True(ObjMonChickenDeerCore.GreaterEqualNotGreater());
        Assert.True(ObjMonChickenDeerCore.SameTickTwoGuards());
        Assert.True(ObjMonChickenDeerCore.ProperLockUnlock());
        Assert.True(ObjMonChickenDeerCore.TryFinallyGuarantees());
        Assert.True(ObjMonChickenDeerCore.TwoLevelNilChecks());

        Assert.Equal(new[] { 1404, 1453 }, ObjMonChickenDeerCore.StageGuardLines);
    }

    [Fact]
    public void WalkPredicateBoundaries()
    {
        // **用的是 >= ：恰好相等即通过（与 J198 的 > 相反）**
        Assert.True(ObjMonChickenDeerCore.ExactlyEqualPasses());
        Assert.True(ObjMonChickenDeerCore.NotYetBlocked());
        Assert.True(ObjMonChickenDeerCore.ExceededPasses());
        Assert.True(ObjMonChickenDeerCore.DelayPostpones());

        Assert.True(ObjMonChickenDeerCore.CanWalk(0, 500, 500, 0));
        Assert.False(ObjMonChickenDeerCore.CanWalk(0, 499, 500, 0));
        Assert.True(ObjMonChickenDeerCore.CanWalk(0, 501, 500, 0));
    }

    [Fact]
    public void ManhattanDistanceFacts()
    {
        Assert.True(ObjMonChickenDeerCore.ManhattanDistance());
        Assert.True(ObjMonChickenDeerCore.Hardcoded9999());
        Assert.True(ObjMonChickenDeerCore.ClosestWins());
        Assert.True(ObjMonChickenDeerCore.NotEuclidean());
        Assert.True(ObjMonChickenDeerCore.DiagonalCountsTwo());
        Assert.True(ObjMonChickenDeerCore.StraightOneIsOne());
        Assert.True(ObjMonChickenDeerCore.EuclideanDiffers());

        Assert.Equal(2, ObjMonChickenDeerCore.Manhattan(0, 0, 1, 1));
        Assert.Equal(1, ObjMonChickenDeerCore.Manhattan(0, 0, 1, 0));
        Assert.Equal(0, ObjMonChickenDeerCore.Manhattan(5, 5, 5, 5));
    }

    [Fact]
    public void ChooseClosestBehavior()
    {
        Assert.True(ObjMonChickenDeerCore.ChoosesSmallest());
        Assert.True(ObjMonChickenDeerCore.EmptyReturnsMinusOne());
        Assert.True(ObjMonChickenDeerCore.TieKeepsFirst());

        Assert.Equal(2, ObjMonChickenDeerCore.ChooseClosest(
            new List<(int, int)> { (10, 1), (3, 2), (7, 3) }));
        Assert.Equal(-1, ObjMonChickenDeerCore.ChooseClosest(
            new List<(int, int)>()));

        // **严格小于：相等时保留先出现者**
        Assert.Equal(1, ObjMonChickenDeerCore.ChooseClosest(
            new List<(int, int)> { (5, 1), (5, 2) }));
    }

    [Fact]
    public void FilterFacts()
    {
        Assert.True(ObjMonChickenDeerCore.ThreeFilters());
        Assert.True(ObjMonChickenDeerCore.OfflinePlayerFilter());
        Assert.True(ObjMonChickenDeerCore.HideModeNeedsCoolEye());
        Assert.True(ObjMonChickenDeerCore.NotHiddenVisible());
        Assert.True(ObjMonChickenDeerCore.HiddenNoCoolEyeInvisible());
        Assert.True(ObjMonChickenDeerCore.HiddenWithCoolEyeVisible());
        Assert.True(ObjMonChickenDeerCore.OfflineSkipped());
        Assert.True(ObjMonChickenDeerCore.ConfigOffNotSkipped());
        Assert.True(ObjMonChickenDeerCore.NonPlayerNotSkipped());
    }

    // ===================== 三、Create / Destroy =====================

    [Fact]
    public void CreateDestroyFacts()
    {
        Assert.True(ObjMonChickenDeerCore.OnlySetsViewRange());
        Assert.True(ObjMonChickenDeerCore.ViewRangeIsFive());
        Assert.True(ObjMonChickenDeerCore.InheritedCalledFirst());
        Assert.True(ObjMonChickenDeerCore.DestroyIsEmptyShell());
        Assert.True(ObjMonChickenDeerCore.RedundantInherited());
        Assert.True(ObjMonChickenDeerCore.SameAsJ197Operate());
        Assert.True(ObjMonChickenDeerCore.NoThinkOverride());
        Assert.True(ObjMonChickenDeerCore.TargetScanInRunNotThink());
        Assert.True(ObjMonChickenDeerCore.ThreeDeclaredMethods());
        Assert.True(ObjMonChickenDeerCore.FieldDeclaredTwice());
        Assert.True(ObjMonChickenDeerCore.ReadsInheritedField());
        Assert.True(ObjMonChickenDeerCore.NeverAssignedInThisClass());
    }

    // ===================== 四、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonChickenDeerCore.TwoDifferentThresholds());
        Assert.True(ObjMonChickenDeerCore.SixVsFive());
        Assert.True(ObjMonChickenDeerCore.ManyMoreSubclasses());
        Assert.True(ObjMonChickenDeerCore.FirstSubclassOnly());
        Assert.True(ObjMonChickenDeerCore.RemainingClassCountIs54());
    }
}
