using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J185：`ObjSmartMon.pas` 智能怪物 1:1 测试。
/// **本批首次移植该单元**（此前 C# 侧零引用、清单零提及）。
/// 核心是 `LoadMonitems` 的 `SL.Free` 双写（资源生命周期缺陷）、
/// 逐行解析的两套分隔符集与出现判据、以及挖取功能的派生开关。
/// </summary>
public sealed class SmartMonCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(72, SmartMonCore.LoadMonitemsLines);
        Assert.Equal(130, SmartMonCore.MysteryShape1);
        Assert.Equal(131, SmartMonCore.MysteryShape2);
        Assert.Equal(132, SmartMonCore.MysteryShape3);
        Assert.Equal(-1, SmartMonCore.IntSentinel);
        Assert.Equal(19, SmartMonCore.RandomSites);
        Assert.Equal(6, SmartMonCore.RandomComparisonForms);
        Assert.Equal(2, SmartMonCore.ButchDerivationSites);
        Assert.Equal(3, SmartMonCore.ButchSubFlags);
        Assert.Equal(3, SmartMonCore.ButchChargeFields);
        Assert.Equal(2, SmartMonCore.MysteryShapeSites);
        Assert.Equal(8, SmartMonCore.StdModeCount);
    }

    // ===================== 一、资源缺陷 =====================

    [Fact]
    public void DoubleFreeFacts()
    {
        Assert.True(SmartMonCore.FreeTwice());
        Assert.True(SmartMonCore.ExceptNotFinally());
        Assert.True(SmartMonCore.DoubleFreeWindow());
        Assert.True(SmartMonCore.CorrectOnlyIfThrowsEarly());
        Assert.True(SmartMonCore.ShouldBeFinally());
        Assert.True(SmartMonCore.MixingConcerns());
        Assert.True(SmartMonCore.ReliesOnUnstatedAssumption());
    }

    [Fact]
    public void FreeOrder()
    {
        Assert.True(SmartMonCore.FreeBeforeLog());
        Assert.True(SmartMonCore.OrderAmplifiesRisk());
    }

    [Fact]
    public void FreeCountModel()
    {
        // **正常一次、异常早一次、异常晚两次**
        Assert.Equal(1, SmartMonCore.FreeCount(false, false));
        Assert.Equal(1, SmartMonCore.FreeCount(true, true));
        Assert.Equal(2, SmartMonCore.FreeCount(true, false));

        Assert.True(SmartMonCore.NormalPathFreesOnce());
        Assert.True(SmartMonCore.EarlyThrowFreesOnce());
        Assert.True(SmartMonCore.LateThrowFreesTwice());
        Assert.True(SmartMonCore.WindowExists());
    }

    [Fact]
    public void ResultSemantics()
    {
        Assert.True(SmartMonCore.AccumulatesSuccesses());
        Assert.True(SmartMonCore.ExceptLeavesPartialResult());
        Assert.True(SmartMonCore.NotFailFast());
        Assert.True(SmartMonCore.MissingFileReturnsZero());
        Assert.True(SmartMonCore.IndistinguishableFromEmpty());
    }

    [Fact]
    public void ResultModel()
    {
        Assert.True(SmartMonCore.SimulateResultValues());
        Assert.True(SmartMonCore.SameZeroBothWays());

        Assert.Equal(0, SmartMonCore.SimulateResult(false, 0, false));
        Assert.Equal(0, SmartMonCore.SimulateResult(true, 0, false));
        // **异常时保留已加载的件数**
        Assert.Equal(7, SmartMonCore.SimulateResult(true, 7, true));
    }

    // ===================== 二、逐行解析 =====================

    [Fact]
    public void SplitShape()
    {
        Assert.True(SmartMonCore.FourSplits());
        Assert.True(SmartMonCore.TwoDelimiterSets());
        Assert.True(SmartMonCore.SlashOnlyInFirstTwo());
        Assert.True(SmartMonCore.DelimiterDifferenceIsSlash());
        Assert.True(SmartMonCore.DelimiterSetSizes());
    }

    [Fact]
    public void DelimiterValues()
    {
        Assert.Equal(new[] { ' ', '/', '\t' }, SmartMonCore.DelimsFirstTwo);
        Assert.Equal(new[] { ' ', '\t' }, SmartMonCore.DelimsLastTwo);
    }

    [Fact]
    public void QuotingAndSentinel()
    {
        Assert.True(SmartMonCore.QuotedItemName());
        Assert.True(SmartMonCore.ArrestInsideQuotes());
        Assert.True(SmartMonCore.SentinelMinusOne());
        Assert.True(SmartMonCore.PositiveGuardExcludesIt());
        Assert.True(SmartMonCore.NoExplicitValidation());
    }

    [Fact]
    public void TripleGuard()
    {
        Assert.True(SmartMonCore.TripleGuard());
        Assert.True(SmartMonCore.StrictlyPositiveNumbers());
        Assert.True(SmartMonCore.LineAcceptedValues());

        Assert.True(SmartMonCore.LineAccepted(1, 1, "x"));
        Assert.False(SmartMonCore.LineAccepted(0, 1, "x"));
        Assert.False(SmartMonCore.LineAccepted(1, 0, "x"));
        Assert.False(SmartMonCore.LineAccepted(1, 1, ""));
        // **哨兵负一被正向判据排除**
        Assert.False(SmartMonCore.LineAccepted(-1, 1, "x"));
    }

    // ---------- 出现判据 ----------

    [Fact]
    public void AppearanceSemantics()
    {
        Assert.True(SmartMonCore.ThresholdIsCountMinusOne());
        Assert.True(SmartMonCore.GuaranteedWhenCountGeRate());
        Assert.True(SmartMonCore.TrueRandomWhenCountLessRate());
        Assert.True(SmartMonCore.InclusiveCompare());
        Assert.True(SmartMonCore.HitCountEqualsCount());
    }

    [Fact]
    public void AppearanceModel()
    {
        Assert.True(SmartMonCore.GuaranteedCase());
        Assert.True(SmartMonCore.CountExceedsRateStillGuaranteed());
        Assert.True(SmartMonCore.HitCountMatchesCount());
        Assert.True(SmartMonCore.CountOneOnlyZeroHits());

        // **阈值是数量减一、且小于等于**
        Assert.True(SmartMonCore.ShouldAppear(0, 1));
        Assert.False(SmartMonCore.ShouldAppear(1, 1));
        Assert.True(SmartMonCore.ShouldAppear(2, 3));
        Assert.False(SmartMonCore.ShouldAppear(3, 3));
    }

    [Fact]
    public void GoldExclusion()
    {
        Assert.True(SmartMonCore.GoldExcludedExplicitly());
        Assert.True(SmartMonCore.SkippedNotErrored());
        Assert.True(SmartMonCore.GoldCheckValues());

        Assert.False(SmartMonCore.IsNotGold("金币", "金币"));
        Assert.True(SmartMonCore.IsNotGold("屠龙", "金币"));
    }

    [Fact]
    public void MysteryShapes()
    {
        Assert.True(SmartMonCore.MysteryShapeThreeValues());
        Assert.True(SmartMonCore.TwiceInUnit());
        Assert.True(SmartMonCore.MysteryShapeValues());

        Assert.Equal(new[] { 130, 131, 132 }, SmartMonCore.MysteryShapes);
        Assert.True(SmartMonCore.IsMysteryShape(131));
        Assert.False(SmartMonCore.IsMysteryShape(133));
    }

    // ---------- StdMode 集合 ----------

    [Fact]
    public void StdModeSet()
    {
        Assert.True(SmartMonCore.EightStdModes());
        Assert.True(SmartMonCore.NineteenToTwentyFourContiguous());
        Assert.True(SmartMonCore.MissingEighteen());
        Assert.True(SmartMonCore.Has15And26());
        Assert.True(SmartMonCore.StdModesAscendingDistinct());

        Assert.Equal(new[] { 15, 19, 20, 21, 22, 23, 24, 26 }, SmartMonCore.StdModes);
    }

    [Fact]
    public void StdModeGaps()
    {
        // **十八缺失、二十五缺失**
        Assert.True(SmartMonCore.GapBetween18And19());
        Assert.True(SmartMonCore.GapAt25());

        Assert.False(SmartMonCore.StdModeIncluded(18));
        Assert.True(SmartMonCore.StdModeIncluded(19));
        Assert.True(SmartMonCore.StdModeIncluded(24));
        Assert.False(SmartMonCore.StdModeIncluded(25));
        Assert.True(SmartMonCore.StdModeIncluded(26));
    }

    [Fact]
    public void OwnershipExits()
    {
        Assert.True(SmartMonCore.TwoOwnershipExits());
        Assert.True(SmartMonCore.NoSingleCleanupPoint());
    }

    // ===================== 三、保护范围 =====================

    [Fact]
    public void RestrictRangeFacts()
    {
        Assert.True(SmartMonCore.OnlyWhenNoMaster());
        Assert.True(SmartMonCore.MasterOverridesProtection());
        Assert.True(SmartMonCore.ProtectModeAndThree());
        Assert.True(SmartMonCore.StrictlyGreater());
        Assert.True(SmartMonCore.AxisIndependentNotEuclidean());
        Assert.True(SmartMonCore.LockAttackInSameDisjunct());
    }

    [Fact]
    public void RestrictRangeModel()
    {
        Assert.True(SmartMonCore.MasterCase());
        Assert.True(SmartMonCore.NotProtectModeCase());
        Assert.True(SmartMonCore.HorizontalTriggers());
        Assert.True(SmartMonCore.VerticalTriggers());
        Assert.True(SmartMonCore.ExactRangeIsInside());
        Assert.True(SmartMonCore.DiagonalAtRangeIsInside());
        Assert.True(SmartMonCore.LockAttackAloneTriggers());
    }

    [Fact]
    public void RestrictRangeBoundary()
    {
        // **严格大于：恰好等于范围不算越界**
        Assert.False(SmartMonCore.OutOfRestrictRange(false, true, 0, 5, 0, 0, 5, false));
        Assert.True(SmartMonCore.OutOfRestrictRange(false, true, 0, 6, 0, 0, 5, false));

        // **分轴：对角线 (4,4) 在范围 5 内**
        Assert.False(SmartMonCore.OutOfRestrictRange(false, true, 0, 4, 0, 4, 5, false));
    }

    [Fact]
    public void MirrorAndDefault()
    {
        Assert.True(SmartMonCore.TargetMirror());
        Assert.True(SmartMonCore.SelfVersusTarget());
        Assert.True(SmartMonCore.DefaultParameter());
        Assert.True(SmartMonCore.ShowLowMpDefaultFalse());
    }

    // ===================== 四、挖取功能 =====================

    [Fact]
    public void ButchDerivation()
    {
        Assert.True(SmartMonCore.DerivedNotConfigured());
        Assert.True(SmartMonCore.OrOfTwoSubFlags());
        Assert.True(SmartMonCore.ExpressionTwice());
        Assert.True(SmartMonCore.DeriveButchValues());
    }

    [Fact]
    public void ButchDerivationModel()
    {
        Assert.False(SmartMonCore.DeriveButch(false, false));
        Assert.True(SmartMonCore.DeriveButch(true, false));
        Assert.True(SmartMonCore.DeriveButch(false, true));
        Assert.True(SmartMonCore.DeriveButch(true, true));
    }

    [Fact]
    public void ButchSubFlags()
    {
        Assert.True(SmartMonCore.ThreeSubFlags());
        Assert.True(SmartMonCore.TriggerNotInDerivation());

        Assert.Equal(3, SmartMonCore.AllButchSubFlags.Length);
        Assert.Equal(2, SmartMonCore.DerivedSubFlags.Length);
        Assert.DoesNotContain("boButchItemTrigger", SmartMonCore.DerivedSubFlags);
    }

    [Fact]
    public void ButchCharge()
    {
        Assert.True(SmartMonCore.ChargeThreeFields());
        Assert.True(SmartMonCore.BothFromConfig());
        Assert.True(SmartMonCore.NoHardcodedFallback());
    }

    [Fact]
    public void ButchRandomSemantics()
    {
        // **与 LoadMonitems 同形不同义**
        Assert.True(SmartMonCore.RandomZeroCompare());
        Assert.True(SmartMonCore.DifferentFromLoadMonitems());
        Assert.True(SmartMonCore.SameFormDifferentMeaning());
        Assert.True(SmartMonCore.ButchOnlyZeroHits());
        Assert.True(SmartMonCore.ThresholdsDiffer());

        Assert.True(SmartMonCore.ButchHits(0));
        Assert.False(SmartMonCore.ButchHits(1));
        // **同一个值 1：行判据命中、挖取判据不命中**
        Assert.True(SmartMonCore.ShouldAppear(1, 3));
        Assert.False(SmartMonCore.ButchHits(1));
    }

    // ===================== 五、配置读取与跨批次 =====================

    [Fact]
    public void ConfigReading()
    {
        Assert.True(SmartMonCore.CommentedThenReimplemented());
        Assert.True(SmartMonCore.OldFieldCommented());
        Assert.True(SmartMonCore.NewFieldActive());
        Assert.True(SmartMonCore.SwitchPlusRatePattern());
        Assert.True(SmartMonCore.ThreeInstances());
    }

    [Fact]
    public void HumanFields()
    {
        Assert.True(SmartMonCore.ReusesHumanAppearanceFields());
        Assert.True(SmartMonCore.ThreeHumanFields());

        Assert.Equal(new[] { "Job", "Gender", "Hair" }, SmartMonCore.HumanAppearanceFields);
    }

    [Fact]
    public void RandomForms()
    {
        Assert.True(SmartMonCore.NineteenRandoms());
        Assert.True(SmartMonCore.SixComparisonForms());
        Assert.True(SmartMonCore.InconsistentUsage());
        Assert.True(SmartMonCore.RandomFormsDistinct());

        Assert.Equal(6, SmartMonCore.RandomForms.Length);
        Assert.Contains("= 0", SmartMonCore.RandomForms);
        Assert.Contains("<= 0", SmartMonCore.RandomForms);
    }

    [Fact]
    public void LineCounts()
    {
        Assert.True(SmartMonCore.LineCounts());
        Assert.True(SmartMonCore.UnitLineCount());
        Assert.True(SmartMonCore.AuditReported469());

        Assert.Equal(3540, SmartMonCore.UnitLines);
    }
}
