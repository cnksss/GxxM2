using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J186：`THumMon.Run`（499 行）1:1 测试。
/// **本单元最长方法、也是目前最长的服务端方法。**
/// 核心是插桩的五个缺口与三处重复、十一个 `inherited`（与客户端零次形成对照）、
/// 以及四处完全相同的被注释条件。
/// </summary>
public sealed class HumMonRunCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(499, HumMonRunCore.RunLines);
        Assert.Equal(943, HumMonRunCore.RunStartLine);
        Assert.Equal(1441, HumMonRunCore.RunEndLine);
        Assert.Equal(11, HumMonRunCore.LocalCount);
        Assert.Equal(42, HumMonRunCore.ErrCodeAssignments);
        Assert.Equal(43, HumMonRunCore.ErrCodeMax);
        Assert.Equal(19, HumMonRunCore.J183Assignments);
        Assert.Equal(11, HumMonRunCore.InheritedSites);
        Assert.Equal(4, HumMonRunCore.InheritedWithExit);
        Assert.Equal(6, HumMonRunCore.SkillFlagCount);
        Assert.Equal(4, HumMonRunCore.CommentedMasterConditions);
        Assert.Equal(19, HumMonRunCore.InlineComments);
        Assert.Equal(6, HumMonRunCore.DatedComments);
        Assert.Equal(5, HumMonRunCore.EnvironmentResets);
        Assert.Equal(1000, HumMonRunCore.SearchTargetInterval);
        Assert.Equal(-1, HumMonRunCore.TargetSentinel);
        Assert.Equal(26, HumMonRunCore.SkillFireSword);
        Assert.Equal(2019, HumMonRunCore.LatestCommentYear);
        Assert.Equal(2013, HumMonRunCore.EarliestCommentYear);
    }

    [Fact]
    public void SpanMatches()
    {
        Assert.True(HumMonRunCore.SpanMatchesLineCount());
        Assert.Equal(499, HumMonRunCore.RunEndLine - HumMonRunCore.RunStartLine + 1);
    }

    // ===================== 一、插桩 =====================

    [Fact]
    public void InstrumentationFacts()
    {
        Assert.True(HumMonRunCore.FortyTwoAssignments());
        Assert.True(HumMonRunCore.RangeZeroTo43());
        Assert.True(HumMonRunCore.DenserThanJ183());
        Assert.True(HumMonRunCore.FiveGaps());
        Assert.True(HumMonRunCore.J183HasNoGaps());
        Assert.True(HumMonRunCore.DifferentDiscipline());
        Assert.True(HumMonRunCore.DuplicatesAre12And24And25());
    }

    [Fact]
    public void ErrCodeTable()
    {
        Assert.Equal(42, HumMonRunCore.ErrCodes.Length);
        Assert.Equal(0, HumMonRunCore.ErrCodes[0]);
        Assert.Equal(43, HumMonRunCore.ErrCodes[^1]);
        Assert.Equal(new[] { 6, 7, 26, 27, 38 }, HumMonRunCore.ErrCodeGaps);
        Assert.Equal(new[] { 12, 24, 25 }, HumMonRunCore.ErrCodeDuplicates);
    }

    [Fact]
    public void ComputedTablesMatchHandwritten()
    {
        // **手写表必须与程序化计算一致**
        Assert.True(HumMonRunCore.GapsMatchComputed());
        Assert.True(HumMonRunCore.DuplicatesMatchComputed());

        Assert.Equal(new[] { 6, 7, 26, 27, 38 }, HumMonRunCore.ComputeGaps());
        Assert.Equal(new[] { 12, 24, 25 }, HumMonRunCore.ComputeDuplicates());
    }

    [Fact]
    public void ContinuousCounterExample()
    {
        // **J183 的十九级对照：零到十八连续无缺口**
        Assert.True(HumMonRunCore.ContinuousCounterExample());
    }

    [Fact]
    public void SkillFlagDefect()
    {
        Assert.True(HumMonRunCore.SixSkillFlags());
        Assert.True(HumMonRunCore.OnlyFiveCodes());
        Assert.True(HumMonRunCore.SixthHasNoOwnCode());
        Assert.True(HumMonRunCore.InheritsPreviousCode());
        Assert.True(HumMonRunCore.SixthIsMissing());
        Assert.True(HumMonRunCore.FiveHaveSixthDoesNot());
    }

    [Fact]
    public void SkillIds()
    {
        Assert.Equal(new[] { 26, 56, 42, 66, 113, 115 }, HumMonRunCore.SkillIds);
        Assert.Equal(6, HumMonRunCore.SkillIds.Length);
        // **一百一十五是第六个、缺编号的那个**
        Assert.Equal(115, HumMonRunCore.SkillIds[5]);
        Assert.True(HumMonRunCore.NotSorted());
        Assert.True(HumMonRunCore.WideSpan());
        Assert.Equal(89, HumMonRunCore.SkillIds[^1] - HumMonRunCore.SkillIds[0]);
    }

    [Fact]
    public void HomeBranches()
    {
        Assert.True(HumMonRunCore.TwoHomeBranches());
        Assert.True(HumMonRunCore.SharedCodes());
        Assert.True(HumMonRunCore.SymmetricShape());
        Assert.True(HumMonRunCore.BranchACommented());
        Assert.True(HumMonRunCore.BranchBActive());
        Assert.True(HumMonRunCore.OnlyDifferenceIsCoordinate());
    }

    [Fact]
    public void HomeBranchModel()
    {
        Assert.True(HumMonRunCore.TwoHomeBranchValues());
        Assert.True(HumMonRunCore.BranchesAreDistinct());

        // **甲：无主人且超范围**
        Assert.True(HumMonRunCore.HomeBranchA(false, true));
        Assert.False(HumMonRunCore.HomeBranchA(true, true));
        Assert.False(HumMonRunCore.HomeBranchA(false, false));

        // **乙：无主人且无目标且保护模式**
        Assert.True(HumMonRunCore.HomeBranchB(false, false, true));
        Assert.False(HumMonRunCore.HomeBranchB(false, true, true));
        Assert.False(HumMonRunCore.HomeBranchB(true, false, true));
    }

    [Fact]
    public void ExceptionLog()
    {
        Assert.True(HumMonRunCore.TwoLogLines());
        Assert.True(HumMonRunCore.MoreThanJ183());
        Assert.True(HumMonRunCore.BuildExceptionLogValues());

        string[] log = HumMonRunCore.BuildExceptionLog(24, "boom");
        Assert.Equal(2, log.Length);
        Assert.Equal("[Exception] THumMon:Run; Code =24", log[0]);
        Assert.Equal("boom", log[1]);
    }

    // ===================== 二、inherited =====================

    [Fact]
    public void InheritedFacts()
    {
        Assert.True(HumMonRunCore.ElevenCallSites());
        Assert.True(HumMonRunCore.FourPairedWithExit());
        Assert.True(HumMonRunCore.TwoQualifiedWondering());
        Assert.True(HumMonRunCore.OneUnconditionalAtEnd());
        Assert.True(HumMonRunCore.QualifiedInheritance());
        Assert.True(HumMonRunCore.ForWanderingPath());
    }

    [Fact]
    public void ClientServerContrast()
    {
        Assert.True(HumMonRunCore.ContrastsWithClient());
        Assert.True(HumMonRunCore.ClientRunsRarelyInherit());
        Assert.True(HumMonRunCore.ServerRunInheritsHeavily());
        Assert.True(HumMonRunCore.FourWayInheritCountsValues());
        Assert.True(HumMonRunCore.ServerIsTheOutlier());
        Assert.True(HumMonRunCore.ClientTotalIsOne());

        // **零、零、一、十一**
        Assert.Equal(new[] { 0, 0, 1, 11 }, HumMonRunCore.FourWayInheritCounts);
        Assert.True(HumMonRunCore.FourWayInheritCounts[3] > HumMonRunCore.FourWayInheritCounts[2]);
    }

    // ===================== 三、被注释的条件 =====================

    [Fact]
    public void CommentedConditions()
    {
        Assert.True(HumMonRunCore.FourCommentedConditions());
        Assert.True(HumMonRunCore.IdenticalText());
        Assert.True(HumMonRunCore.DeliberateGlobalEdit());
        Assert.True(HumMonRunCore.MasterConditionRemoved());
        Assert.True(HumMonRunCore.ThreeSemantics());
        Assert.True(HumMonRunCore.TwoPredicateKinds());
        Assert.True(HumMonRunCore.CommentedConditionText());
        Assert.True(HumMonRunCore.TwoConjunctsRemain());

        Assert.Equal(4, HumMonRunCore.CommentedConditionLines.Length);
        Assert.Equal(new[] { 1065, 1177, 1243, 1316 }, HumMonRunCore.CommentedConditionLines);
        Assert.Equal("{ (m_Master <> nil) and }", HumMonRunCore.CommentedCondition);
    }

    [Fact]
    public void ChangeAbility()
    {
        // **废止"有主人"后：无主人时也能成立**
        Assert.True(HumMonRunCore.ChangeAbilityActive(true, 1));
        Assert.True(HumMonRunCore.WorksWithoutMaster());
        Assert.False(HumMonRunCore.ChangeAbilityActive(false, 1));
        Assert.False(HumMonRunCore.ChangeAbilityActive(true, 0));
    }

    [Fact]
    public void BlockComment()
    {
        Assert.True(HumMonRunCore.BlockComment());
        Assert.True(HumMonRunCore.DuplicatesActiveCode());
        Assert.True(HumMonRunCore.DatedComment());
    }

    // ===================== 四、环境切换与技能 =====================

    [Fact]
    public void EnvironmentChange()
    {
        Assert.True(HumMonRunCore.EnvironmentChangeResets());
        Assert.True(HumMonRunCore.FiveResets());
        Assert.True(HumMonRunCore.BothDirectionsToMinusOne());
        Assert.True(HumMonRunCore.EnvironmentChangedValues());

        Assert.False(HumMonRunCore.EnvironmentChanged(1, 1));
        Assert.True(HumMonRunCore.EnvironmentChanged(1, 2));
    }

    [Fact]
    public void ThinkAndTemplates()
    {
        Assert.True(HumMonRunCore.ThinkShortCircuits());
        Assert.True(HumMonRunCore.ThinkBeforeMove());
        Assert.True(HumMonRunCore.SixIdenticalTemplates());
        Assert.True(HumMonRunCore.OnlySkillIdDiffers());
        Assert.True(HumMonRunCore.SymbolicThenNumeric());
        Assert.True(HumMonRunCore.MixedStyle());
    }

    [Fact]
    public void SkillCooldown()
    {
        Assert.True(HumMonRunCore.SkillCooldownValues());
        Assert.True(HumMonRunCore.CooldownInclusive());

        Assert.False(HumMonRunCore.SkillCooldownElapsed(true, 100, 90, 11));
        // **大于等于：恰好等于冷却即清标志**
        Assert.True(HumMonRunCore.SkillCooldownElapsed(true, 100, 90, 10));
        Assert.False(HumMonRunCore.SkillCooldownElapsed(false, 100, 90, 10));
    }

    [Fact]
    public void TargetSentinelAndSearch()
    {
        Assert.True(HumMonRunCore.TargetSentinelMinusOne());
        Assert.True(HumMonRunCore.BothCoordinates());
        Assert.True(HumMonRunCore.SearchGate1000());
        Assert.True(HumMonRunCore.NotWhileRestricted());
        Assert.True(HumMonRunCore.ShouldSearchTargetValues());

        // **严格大于一千**
        Assert.False(HumMonRunCore.ShouldSearchTarget(1000, 0, false));
        Assert.True(HumMonRunCore.ShouldSearchTarget(1001, 0, false));
        Assert.False(HumMonRunCore.ShouldSearchTarget(1001, 0, true));
    }

    // ===================== 五、随攻击跑动与随机 =====================

    [Fact]
    public void RunWithAttack()
    {
        Assert.True(HumMonRunCore.RunWithAttackTwice());
        Assert.True(HumMonRunCore.IdenticalAgain());
        Assert.True(HumMonRunCore.RunWithAttackValues());

        Assert.True(HumMonRunCore.RunWithAttack(true, 0));
        Assert.False(HumMonRunCore.RunWithAttack(true, 1));
        Assert.False(HumMonRunCore.RunWithAttack(false, 0));
    }

    [Fact]
    public void Randoms()
    {
        Assert.True(HumMonRunCore.RandomEqualsZero());
        Assert.True(HumMonRunCore.ThreeModuli());
        Assert.True(HumMonRunCore.RandomModuliValues());

        Assert.Equal(3, HumMonRunCore.RandomModuli.Length);
        Assert.Contains(2, HumMonRunCore.RandomModuli);
        Assert.Contains(20, HumMonRunCore.RandomModuli);
        Assert.Contains(5, HumMonRunCore.RandomModuli);
    }

    [Fact]
    public void Comments()
    {
        Assert.True(HumMonRunCore.NineteenInlineComments());
        Assert.True(HumMonRunCore.SixDated());
        Assert.True(HumMonRunCore.LongTermMaintenance());
        Assert.True(HumMonRunCore.LatestComment2019());
        Assert.True(HumMonRunCore.CadaverFix());
        Assert.True(HumMonRunCore.FixAtTailBranch());
        Assert.True(HumMonRunCore.CommentSpanIs6());

        Assert.Equal(6, HumMonRunCore.LatestCommentYear - HumMonRunCore.EarliestCommentYear);
    }

    // ===================== 六、跨批次对照 =====================

    [Fact]
    public void CrossBatch()
    {
        Assert.True(HumMonRunCore.TwoInstrumentationStyles());
        Assert.True(HumMonRunCore.BaseIsDisciplined());
        Assert.True(HumMonRunCore.HumMonIsNot());
        Assert.True(HumMonRunCore.LongestServerMethod());
        Assert.True(HumMonRunCore.StillShorterThanLoadSurface());
        Assert.True(HumMonRunCore.ElevenLocals());
    }
}
