using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J187：`TCopyMon.Run`（385 行）1:1 测试。
/// **与 J186 同单元、同职责但写法迥异**：冷却门槛一处读配置一处写死、
/// 比较一处含端点一处不含、插桩一处四十二处一处全无。
/// 另含两个真实缺陷：十四行死代码里孤立的 `else if`，
/// 以及 `or` 未被括号包住导致空目标也能进入分支。
/// </summary>
public sealed class CopyMonRunCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(385, CopyMonRunCore.RunLines);
        Assert.Equal(1802, CopyMonRunCore.RunStartLine);
        Assert.Equal(2186, CopyMonRunCore.RunEndLine);
        Assert.Equal(7, CopyMonRunCore.LocalCount);
        Assert.Equal(0, CopyMonRunCore.ErrCodeCount);
        Assert.Equal(8, CopyMonRunCore.InheritedSites);
        Assert.Equal(3, CopyMonRunCore.CommentedDebugPrints);
        Assert.Equal(14, CopyMonRunCore.DeadBlockLines);
        Assert.Equal(20000, CopyMonRunCore.HardcodedCooldown);
        Assert.Equal(6, CopyMonRunCore.LiteralSites);
        Assert.Equal(6, CopyMonRunCore.SkillFlagCount);
        Assert.Equal(500, CopyMonRunCore.DefaultWalkTime);
        Assert.Equal(3, CopyMonRunCore.JobBranches);
        Assert.Equal(5, CopyMonRunCore.CaseSites);
        Assert.Equal(3, CopyMonRunCore.CaseWith500);
        Assert.Equal(4, CopyMonRunCore.HardcodedNearDistance);
        Assert.Equal(1, CopyMonRunCore.WizardJob);
        Assert.Equal(19, CopyMonRunCore.InlineComments);
        Assert.Equal(7, CopyMonRunCore.DatedComments);
        Assert.Equal(6, CopyMonRunCore.J186DatedComments);
        Assert.Equal(2021, CopyMonRunCore.LatestCommentYear);
        Assert.Equal(2019, CopyMonRunCore.J186LatestCommentYear);
        Assert.Equal(2013, CopyMonRunCore.EarliestCommentYear);
        Assert.Equal(42, CopyMonRunCore.J186ErrCodes);
        Assert.Equal(499, CopyMonRunCore.J186RunLines);
        Assert.Equal(158, CopyMonRunCore.J184RunLines);
    }

    [Fact]
    public void SpanMatches()
    {
        Assert.True(CopyMonRunCore.SpanMatchesLineCount());
        Assert.Equal(385, CopyMonRunCore.RunEndLine - CopyMonRunCore.RunStartLine + 1);
    }

    // ===================== 一、与 J186 的对照 =====================

    [Fact]
    public void TwoCooldownStyles()
    {
        Assert.True(CopyMonRunCore.TwoCooldownStyles());
        Assert.True(CopyMonRunCore.HumUsesConfig());
        Assert.True(CopyMonRunCore.CopyUsesLiteral());
        Assert.True(CopyMonRunCore.InclusiveVersusStrict());
        Assert.True(CopyMonRunCore.SixLiteralSites());
        Assert.True(CopyMonRunCore.OnlyInCopyRun());
        Assert.True(CopyMonRunCore.NeverExtracted());
        Assert.True(CopyMonRunCore.SameSixSkills());
    }

    [Fact]
    public void CooldownBoundaryDiverges()
    {
        // **二十秒整：人形怪（含端点）清标志、分身（严格大于）不清**
        Assert.True(CopyMonRunCore.BoundaryDiffers());
        Assert.True(CopyMonRunCore.BeyondBoundaryBothTrue());

        Assert.True(CopyMonRunCore.HumCooldownElapsed(20000, 0, 20000));
        Assert.False(CopyMonRunCore.CopyCooldownElapsed(20000, 0));
        Assert.True(CopyMonRunCore.CopyCooldownElapsed(20001, 0));
    }

    [Fact]
    public void InstrumentationContrast()
    {
        Assert.True(CopyMonRunCore.CopyHasNoErrCode());
        Assert.True(CopyMonRunCore.HumHasFortyTwo());
        Assert.True(CopyMonRunCore.StarkContrast());

        Assert.Equal(0, CopyMonRunCore.ErrCodeCount);
        Assert.Equal(42, CopyMonRunCore.J186ErrCodes);
    }

    [Fact]
    public void InheritedShape()
    {
        Assert.True(CopyMonRunCore.EightInherited());
        Assert.True(CopyMonRunCore.NoQualifiedInheritance());
        Assert.True(CopyMonRunCore.ThreeAfterCommentedDebug());

        Assert.Equal(8, CopyMonRunCore.InheritedLines.Length);
        Assert.Equal(new[] { 1824, 1840, 1849, 2088, 2126, 2146, 2166, 2184 }, CopyMonRunCore.InheritedLines);
    }

    [Fact]
    public void SkillIds()
    {
        Assert.Equal(new[] { 26, 56, 42, 66, 113, 115 }, CopyMonRunCore.SkillIds);
    }

    // ===================== 二、被注释的调试打印 =====================

    [Fact]
    public void DebugPrints()
    {
        Assert.True(CopyMonRunCore.ThreeDebugPrints());
        Assert.True(CopyMonRunCore.EachFollowedByInherited());
        Assert.True(CopyMonRunCore.BranchEntryMarkers());
        Assert.True(CopyMonRunCore.ThreeContents());
        Assert.True(CopyMonRunCore.CoverThreeEarlyExits());
        Assert.True(CopyMonRunCore.BatchOfDebugging());
        Assert.True(CopyMonRunCore.SameFamilyAsJ186());
    }

    [Fact]
    public void DebugPrintLines()
    {
        Assert.Equal(new[] { 1823, 1839, 1848 }, CopyMonRunCore.DebugPrintLines);
        Assert.Equal(3, CopyMonRunCore.DebugPrintTexts.Length);
        Assert.True(CopyMonRunCore.DebugTextsDistinct());
    }

    [Fact]
    public void DebugTexts()
    {
        Assert.Contains("// MainOutMessage('Think:'+m_sCharName);", CopyMonRunCore.DebugPrintTexts);
        Assert.Contains("// MainOutMessage('m_Master.m_boDeath:'+m_sCharName);", CopyMonRunCore.DebugPrintTexts);
        Assert.Contains("// MainOutMessage('(m_Master = nil):'+m_sCharName);", CopyMonRunCore.DebugPrintTexts);
    }

    // ===================== 三、死代码与括号缺陷 =====================

    [Fact]
    public void DeadBlock()
    {
        Assert.True(CopyMonRunCore.FourteenLineDeadBlock());
        Assert.True(CopyMonRunCore.OrphanedElseIf());
        Assert.True(CopyMonRunCore.UncommentingWouldNotCompile());
        Assert.True(CopyMonRunCore.ConfigValueVersusLiteral4());
        Assert.True(CopyMonRunCore.RevertedToHardcode());

        Assert.Equal(new[] { 1973, 1986 }, CopyMonRunCore.DeadBlockSpan);
        Assert.Equal(14, CopyMonRunCore.DeadBlockSpan[1] - CopyMonRunCore.DeadBlockSpan[0] + 1);
    }

    [Fact]
    public void ParenBug()
    {
        Assert.True(CopyMonRunCore.ParenBugPresent());
        Assert.True(CopyMonRunCore.OrOutsideTheGroup());
        Assert.True(CopyMonRunCore.NullOrBranchReachable());
        Assert.True(CopyMonRunCore.SameBugInDeadCode());
        Assert.True(CopyMonRunCore.NotIntroducedByEdit());
    }

    [Fact]
    public void ParenBugDemonstrated()
    {
        // **目标为空但纵向超过四格：缺陷版为真、意图版为假**
        Assert.True(CopyMonRunCore.ParenBugDemonstrated());

        Assert.True(CopyMonRunCore.NearMasterBuggy(false, 0, 9));
        Assert.False(CopyMonRunCore.NearMasterIntended(false, 0, 9));
    }

    [Fact]
    public void ParenBugOnlyWithNullTarget()
    {
        // **目标存在时两者结论一致 —— 缺陷只在空目标时显现**
        Assert.True(CopyMonRunCore.AgreeWhenTargetExists());

        Assert.True(CopyMonRunCore.NearMasterBuggy(true, 9, 0));
        Assert.True(CopyMonRunCore.NearMasterIntended(true, 9, 0));
        Assert.False(CopyMonRunCore.NearMasterBuggy(true, 0, 0));
        Assert.False(CopyMonRunCore.NearMasterIntended(true, 0, 0));
    }

    [Fact]
    public void NearDistanceBoundary()
    {
        // **严格大于：恰好四格不算超距**
        Assert.True(CopyMonRunCore.ExactFourIsNotNear());
        Assert.False(CopyMonRunCore.NearMasterBuggy(true, 4, 4));
        Assert.True(CopyMonRunCore.NearMasterBuggy(true, 5, 0));
    }

    [Fact]
    public void DeadCodeSameBug()
    {
        Assert.True(CopyMonRunCore.DeadCodeHasSameBug());
    }

    // ===================== 四、行走时间 =====================

    [Fact]
    public void WalkTimeFacts()
    {
        Assert.True(CopyMonRunCore.TwoSources());
        Assert.True(CopyMonRunCore.SwitchSelects());
        Assert.True(CopyMonRunCore.ClampedAtZero());
        Assert.True(CopyMonRunCore.JobThreeWay());
        Assert.True(CopyMonRunCore.DefaultFiveHundred());
        Assert.True(CopyMonRunCore.JobMapping());
        Assert.True(CopyMonRunCore.OnlyThreeJobs());
    }

    [Fact]
    public void WalkTimeFromFrame()
    {
        Assert.True(CopyMonRunCore.WalkTimeFromFrameValues());
        Assert.True(CopyMonRunCore.ClampPreventsNegative());

        Assert.Equal(600, CopyMonRunCore.WalkTimeFromFrame(600, 0, 10));
        Assert.Equal(500, CopyMonRunCore.WalkTimeFromFrame(600, 10, 10));
        // **夹到零、不会为负**
        Assert.Equal(0, CopyMonRunCore.WalkTimeFromFrame(100, 999, 10));
    }

    [Fact]
    public void WalkTimeByJob()
    {
        Assert.True(CopyMonRunCore.WalkTimeByJobValues());

        Assert.Equal(111, CopyMonRunCore.WalkTimeByJob(0, 111, 222, 333));
        Assert.Equal(222, CopyMonRunCore.WalkTimeByJob(1, 111, 222, 333));
        Assert.Equal(333, CopyMonRunCore.WalkTimeByJob(2, 111, 222, 333));
        // **缺省五百**
        Assert.Equal(500, CopyMonRunCore.WalkTimeByJob(3, 111, 222, 333));
    }

    [Fact]
    public void CaseSites()
    {
        Assert.True(CopyMonRunCore.FiveCaseSites());
        Assert.True(CopyMonRunCore.OnlyThreeWith500());
        Assert.True(CopyMonRunCore.OthersDiffer());

        Assert.Equal(new[] { 880, 1071, 1186, 1934, 2119 }, CopyMonRunCore.CaseLines);
        Assert.Equal(new[] { 888, 1079, 1942 }, CopyMonRunCore.FiveHundredLines);
        Assert.Equal(2, CopyMonRunCore.CaseSites - CopyMonRunCore.CaseWith500);
    }

    [Fact]
    public void MagicNumberReuse()
    {
        Assert.True(CopyMonRunCore.SameNumberDifferentMeaning());
        Assert.True(CopyMonRunCore.NotTheSameConstant());
        Assert.True(CopyMonRunCore.ConfigSwitchTwice());
        Assert.True(CopyMonRunCore.BothInThisMethod());
    }

    // ===================== 五、开盾与攻击修正 =====================

    [Fact]
    public void Shield()
    {
        Assert.True(CopyMonRunCore.ShieldOnlyForWizard());
        Assert.True(CopyMonRunCore.StatusTimeZeroGate());
        Assert.True(CopyMonRunCore.ShieldPreemptsEverything());
        Assert.True(CopyMonRunCore.StateTimeAsFlag());
        Assert.True(CopyMonRunCore.NoBooleanFlag());
        Assert.True(CopyMonRunCore.ShouldCastShieldValues());
    }

    [Fact]
    public void ShieldModel()
    {
        Assert.True(CopyMonRunCore.ShouldCastShield(1, true, 0));
        // **非法师不开盾**
        Assert.False(CopyMonRunCore.ShouldCastShield(0, true, 0));
        Assert.False(CopyMonRunCore.ShouldCastShield(2, true, 0));
        Assert.False(CopyMonRunCore.ShouldCastShield(1, false, 0));
        // **状态剩余时间非零不开盾**
        Assert.False(CopyMonRunCore.ShouldCastShield(1, true, 1));
    }

    [Fact]
    public void HitTickReset()
    {
        Assert.True(CopyMonRunCore.HitTickReset());
        Assert.True(CopyMonRunCore.OnlyForNonPlayerTargets());
        Assert.True(CopyMonRunCore.DeliberatelySkippedForPlayers());
        Assert.True(CopyMonRunCore.ShouldResetHitTickValues());

        // **对玩家与英雄刻意跳过**
        Assert.False(CopyMonRunCore.ShouldResetHitTick(0, 0, 1));
        Assert.False(CopyMonRunCore.ShouldResetHitTick(1, 0, 1));
        // **对其他种族重置**
        Assert.True(CopyMonRunCore.ShouldResetHitTick(80, 0, 1));
    }

    [Fact]
    public void CancelledInherited()
    {
        Assert.True(CopyMonRunCore.CommentedInheritedExit());
        Assert.True(CopyMonRunCore.CancelledDueToSideEffect());
    }

    [Fact]
    public void Comments()
    {
        Assert.True(CopyMonRunCore.NineteenComments());
        Assert.True(CopyMonRunCore.SevenDated());
        Assert.True(CopyMonRunCore.LaterThanJ186());
        Assert.True(CopyMonRunCore.OneMoreDatedThanJ186());
        Assert.True(CopyMonRunCore.CommentSpanIs8());

        Assert.Equal(8, CopyMonRunCore.LatestCommentYear - CopyMonRunCore.EarliestCommentYear);
        Assert.True(CopyMonRunCore.LatestCommentYear > CopyMonRunCore.J186LatestCommentYear);
    }

    // ===================== 六、跨批次对照 =====================

    [Fact]
    public void CrossBatch()
    {
        Assert.True(CopyMonRunCore.SixRunsInstrumentation());
        Assert.True(CopyMonRunCore.OnlyTwoInstrumented());
        Assert.True(CopyMonRunCore.SixRunsInherited());
        Assert.True(CopyMonRunCore.ServerInheritsClientDoesNot());
        Assert.True(CopyMonRunCore.LineCountOrdering());
        Assert.True(CopyMonRunCore.SevenLocals());
    }

    [Fact]
    public void SixRunTables()
    {
        Assert.Equal(new[] { 19, 0, 0, 42, 0 }, CopyMonRunCore.SixRunErrCodes);
        Assert.Equal(new[] { 0, 0, 1, 11, 8 }, CopyMonRunCore.SixRunInherited);

        // **六条 Run 里只有两条有插桩**
        int instrumented = 0;
        foreach (int v in CopyMonRunCore.SixRunErrCodes)
        {
            if (v > 0) instrumented++;
        }

        Assert.Equal(2, instrumented);
    }
}
