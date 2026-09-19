using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J208：`ObjMon.pas` 中 `TLineMagicAttackMonster`（直线魔法攻击怪物）
/// 两个方法 1:1 测试（合计 56 行）。
/// **本批最有价值的发现**：4912 用了 `btDir`、而它在 `else` 分支里**没有**被赋值，
/// 初看像未初始化读取；但追进 `GetAttackDir` 的两个重载后发现
/// 三参版（27050）**无条件**写 `btDir`、而它正是 4895 的第一个调用 ——
/// 所以 `btDir` 此刻**恰好**是"朝向目标的方向"、正是 4912 想要的。
/// 而 4908 那行**被注释掉的**代码内容恰恰是**同一个公式**。
/// </summary>
public sealed class ObjMonLineMagicCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(4884, ObjMonLineMagicCore.MagicStart);
        Assert.Equal(4935, ObjMonLineMagicCore.MagicEnd);
        Assert.Equal(52, ObjMonLineMagicCore.MagicLines);
        Assert.Equal(4937, ObjMonLineMagicCore.RunStart);
        Assert.Equal(4940, ObjMonLineMagicCore.RunEnd);
        Assert.Equal(4, ObjMonLineMagicCore.RunLines);
        Assert.Equal(56, ObjMonLineMagicCore.TotalLines);

        Assert.Equal(4895, ObjMonLineMagicCore.ThreeAttemptsLine);
        Assert.Equal(3, ObjMonLineMagicCore.LineRange1);
        Assert.Equal(2, ObjMonLineMagicCore.LineRange2);
        Assert.Equal(3, ObjMonLineMagicCore.AttemptCount);
        Assert.Equal(713, ObjMonLineMagicCore.OverloadDecl2Arg);
        Assert.Equal(714, ObjMonLineMagicCore.OverloadDecl3Arg);
        Assert.Equal(27050, ObjMonLineMagicCore.OverloadImpl3Arg);
        Assert.Equal(27062, ObjMonLineMagicCore.OverloadImpl2Arg);
        Assert.Equal(27055, ObjMonLineMagicCore.BtDirWriteLine);

        Assert.Equal(4906, ObjMonLineMagicCore.ElseStart);
        Assert.Equal(4917, ObjMonLineMagicCore.ElseEnd);
        Assert.Equal(12, ObjMonLineMagicCore.ElseLines);
        Assert.Equal(4908, ObjMonLineMagicCore.CommentedBtDirLine);
        Assert.Equal(4909, ObjMonLineMagicCore.CommentedMinLine1);
        Assert.Equal(4910, ObjMonLineMagicCore.CommentedMinLine2);
        Assert.Equal(4911, ObjMonLineMagicCore.LiveMinLine);
        Assert.Equal(4912, ObjMonLineMagicCore.NextPosLine);
        Assert.Equal(4914, ObjMonLineMagicCore.SetTargetXYLine);
        Assert.Equal(3, ObjMonLineMagicCore.CommentedLines);
        Assert.Equal(2, ObjMonLineMagicCore.HardcodedStep);
        Assert.Equal(1, ObjMonLineMagicCore.OldClampMin);

        Assert.Equal(4897, ObjMonLineMagicCore.CooldownLine1);
        Assert.Equal(4919, ObjMonLineMagicCore.CooldownLine2);
        Assert.Equal(2, ObjMonLineMagicCore.CooldownChecks);
        Assert.Equal(4925, ObjMonLineMagicCore.OuterComplementLine);
        Assert.Equal(3, ObjMonLineMagicCore.EngageRange);
        Assert.Equal(13, ObjMonLineMagicCore.ClassesCovered);
        Assert.Equal(41, ObjMonLineMagicCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonLineMagicCore.SpanMatches());
        Assert.True(ObjMonLineMagicCore.TotalLinesAddUp());
        Assert.True(ObjMonLineMagicCore.RunAfterMagic());
        Assert.True(ObjMonLineMagicCore.WithinUnit());
        Assert.True(ObjMonLineMagicCore.NoInstrumentation());
        Assert.True(ObjMonLineMagicCore.ElseSpanMatches());
        Assert.True(ObjMonLineMagicCore.ElseInsideMethod());
        Assert.True(ObjMonLineMagicCore.ImplsAfterDecls());
    }

    // ===================== 一、三连判 =====================

    [Fact]
    public void ThreeAttemptFacts()
    {
        Assert.True(ObjMonLineMagicCore.ThreeAttempts());
        Assert.True(ObjMonLineMagicCore.ShortCircuitOrder());
        Assert.True(ObjMonLineMagicCore.BtDirWrittenByAllThree());
        Assert.True(ObjMonLineMagicCore.AttemptsExtracted());
        Assert.True(ObjMonLineMagicCore.OrderIsWidening());
        Assert.True(ObjMonLineMagicCore.ThreeArgIsLine());
        Assert.True(ObjMonLineMagicCore.TwoArgIsAdjacency());
        Assert.True(ObjMonLineMagicCore.TwoOverloads());
        Assert.True(ObjMonLineMagicCore.OneLineUsesBoth());
        Assert.True(ObjMonLineMagicCore.ThirdCallIsTwoArg());
        Assert.True(ObjMonLineMagicCore.OverloadDeclsExtracted());

        Assert.Equal(3, ObjMonLineMagicCore.Attempts.Length);
        Assert.True(ObjMonLineMagicCore.Attempts[0].IsThreeArg);
        Assert.Equal(3, ObjMonLineMagicCore.Attempts[0].Range);
        Assert.True(ObjMonLineMagicCore.Attempts[1].IsThreeArg);
        Assert.Equal(2, ObjMonLineMagicCore.Attempts[1].Range);
        Assert.False(ObjMonLineMagicCore.Attempts[2].IsThreeArg);
        Assert.Equal(0, ObjMonLineMagicCore.Attempts[2].Range);
    }

    [Fact]
    public void ShortCircuitBoundaries()
    {
        Assert.True(ObjMonLineMagicCore.FirstSucceedsOneCall());
        Assert.True(ObjMonLineMagicCore.SecondSucceedsTwoCalls());
        Assert.True(ObjMonLineMagicCore.AllFailThreeCalls());
        Assert.True(ObjMonLineMagicCore.ShortCircuitSavesCalls());
        Assert.True(ObjMonLineMagicCore.ElseNeedsAllThreeFail());

        // **短路：第一次成功只调一次**
        Assert.Equal(1, ObjMonLineMagicCore.CallsMade(true, true, true));
        Assert.Equal(2, ObjMonLineMagicCore.CallsMade(false, true, true));
        Assert.Equal(3, ObjMonLineMagicCore.CallsMade(false, false, true));
        Assert.Equal(3, ObjMonLineMagicCore.CallsMade(false, false, false));

        // **求值结果**
        Assert.True(ObjMonLineMagicCore.Engage(true, false, false));
        Assert.True(ObjMonLineMagicCore.Engage(false, true, false));
        Assert.True(ObjMonLineMagicCore.Engage(false, false, true));
        Assert.False(ObjMonLineMagicCore.Engage(false, false, false));
    }

    // ===================== 二、btDir 的"过期但已定义" =====================

    [Fact]
    public void StaleBtDirFacts()
    {
        Assert.True(ObjMonLineMagicCore.StaleNotUndefined());
        Assert.True(ObjMonLineMagicCore.FirstCallAlwaysWrites());
        Assert.True(ObjMonLineMagicCore.VarParamSideEffect());
        Assert.True(ObjMonLineMagicCore.CommentedLineIsRedundant());
        Assert.True(ObjMonLineMagicCore.IdenticalFormula());
        Assert.True(ObjMonLineMagicCore.AuthorKnewItWasRedundant());
        Assert.True(ObjMonLineMagicCore.AccidentallyCorrect());
        Assert.True(ObjMonLineMagicCore.NoLiveWriteInElse());
        Assert.True(ObjMonLineMagicCore.ElseHasNoBtDirAssign());
        Assert.True(ObjMonLineMagicCore.CommentEqualsFirstCall());
        Assert.True(ObjMonLineMagicCore.BaseObjectIsTarget());
        Assert.True(ObjMonLineMagicCore.BtDirWriteLineExtracted());
        Assert.True(ObjMonLineMagicCore.WriteIsEarly());
    }

    [Fact]
    public void BtDirSiteTable()
    {
        Assert.True(ObjMonLineMagicCore.BtDirSitesExtracted());
        Assert.True(ObjMonLineMagicCore.OneBtDirDeclaration());
        Assert.True(ObjMonLineMagicCore.BtDirReadTwice());

        Assert.Equal(5, ObjMonLineMagicCore.BtDirSites.Length);
        Assert.Equal(4886, ObjMonLineMagicCore.BtDirSites[0].Line);
        Assert.Equal("declare", ObjMonLineMagicCore.BtDirSites[0].Kind);
        Assert.Equal(4902, ObjMonLineMagicCore.BtDirSites[2].Line);
        Assert.Equal("read-by-Attack", ObjMonLineMagicCore.BtDirSites[2].Kind);
        Assert.Equal(4908, ObjMonLineMagicCore.BtDirSites[3].Line);
        Assert.Equal("commented-out", ObjMonLineMagicCore.BtDirSites[3].Kind);
        Assert.Equal(4912, ObjMonLineMagicCore.BtDirSites[4].Line);
    }

    // ===================== 三、MinValue 的退化 =====================

    [Fact]
    public void MinValueDegeneration()
    {
        Assert.True(ObjMonLineMagicCore.TwoCommentedOneLive());
        Assert.True(ObjMonLineMagicCore.ElseHasThreeComments());
        Assert.True(ObjMonLineMagicCore.AllElseCommentsAreMinRelated());
        Assert.True(ObjMonLineMagicCore.OldWasDynamic());
        Assert.True(ObjMonLineMagicCore.NewIsHardcodedTwo());
        Assert.True(ObjMonLineMagicCore.TwoIsOneOfTheRange());
        Assert.True(ObjMonLineMagicCore.MinValueSitesExtracted());
        Assert.True(ObjMonLineMagicCore.CommentsAdjacentToLive());
        Assert.True(ObjMonLineMagicCore.OldAlwaysAtLeastOne());
        Assert.True(ObjMonLineMagicCore.OldSpans1To3());
        Assert.True(ObjMonLineMagicCore.NewIsConstant());
        Assert.True(ObjMonLineMagicCore.VersionsDiffer());
        Assert.True(ObjMonLineMagicCore.Degenerated());

        Assert.Equal(5, ObjMonLineMagicCore.MinValueSites.Length);
        Assert.Equal(4888, ObjMonLineMagicCore.MinValueSites[0].Line);
        Assert.Equal(4911, ObjMonLineMagicCore.MinValueSites[3].Line);
        Assert.Equal("live-hardcoded-2", ObjMonLineMagicCore.MinValueSites[3].Kind);
        Assert.Equal(4912, ObjMonLineMagicCore.MinValueSites[4].Line);
    }

    [Fact]
    public void MinValueDomainCollapse()
    {
        Assert.True(ObjMonLineMagicCore.DifferAtThreeThree());
        Assert.True(ObjMonLineMagicCore.AgreeOnlyWhenMinIsTwo());
        Assert.True(ObjMonLineMagicCore.OldDomainIsOneToThree());
        Assert.True(ObjMonLineMagicCore.DomainCollapsedToConstant());
        Assert.True(ObjMonLineMagicCore.DifferAtOneThree());

        // **旧版值域 1..3**
        Assert.Equal(1, ObjMonLineMagicCore.OldMinValue(0, 0));
        Assert.Equal(1, ObjMonLineMagicCore.OldMinValue(0, 3));
        Assert.Equal(1, ObjMonLineMagicCore.OldMinValue(1, 3));
        Assert.Equal(2, ObjMonLineMagicCore.OldMinValue(2, 3));
        Assert.Equal(2, ObjMonLineMagicCore.OldMinValue(2, 2));

        // **修正后的关键对照：角落处旧版得 3、新版恒为 2**
        Assert.Equal(3, ObjMonLineMagicCore.OldMinValue(3, 3));
        Assert.NotEqual(ObjMonLineMagicCore.HardcodedStep,
            ObjMonLineMagicCore.OldMinValue(3, 3));
        Assert.Equal(2, ObjMonLineMagicCore.HardcodedStep);
    }

    // ===================== 四、GetNextPosition 与落地 =====================

    [Fact]
    public void NextPositionFacts()
    {
        Assert.True(ObjMonLineMagicCore.FlagIsStepDistance());
        Assert.True(ObjMonLineMagicCore.ReturnsFalseWhenBlocked());
        Assert.True(ObjMonLineMagicCore.MovesUp());
        Assert.True(ObjMonLineMagicCore.UpBlockedAtEdge());
        Assert.True(ObjMonLineMagicCore.MovesLeft());
        Assert.True(ObjMonLineMagicCore.LeftBlockedAtEdge());
        Assert.True(ObjMonLineMagicCore.MovesDown());
        Assert.True(ObjMonLineMagicCore.MovesRight());
        Assert.True(ObjMonLineMagicCore.StepIsMultiCell());

        // **边界：撞边时坐标不动且返回假**
        Assert.False(ObjMonLineMagicCore.NextPositionSimple(
            10, 1, 0, 2, 100, 100, out int ux, out int uy));
        Assert.Equal(10, ux);
        Assert.Equal(1, uy);

        // **向下恰好贴边**
        Assert.False(ObjMonLineMagicCore.NextPositionSimple(
            10, 98, 1, 2, 100, 100, out int dx, out int dy));
        Assert.Equal(98, dy);
    }

    [Fact]
    public void LandingPointFacts()
    {
        Assert.True(ObjMonLineMagicCore.PureOutParams());
        Assert.True(ObjMonLineMagicCore.SetsLandingPoint());
        Assert.True(ObjMonLineMagicCore.ExplainsClassName());
        Assert.True(ObjMonLineMagicCore.ComposesWithBaseRun());
        Assert.True(ObjMonLineMagicCore.NxNySitesExtracted());
        Assert.True(ObjMonLineMagicCore.SilentFailureFallsThrough());
        Assert.True(ObjMonLineMagicCore.NoExitOnFalse());
        Assert.True(ObjMonLineMagicCore.DiffersFromJ206J207());

        Assert.Equal(3, ObjMonLineMagicCore.NxNySites.Length);
        Assert.Equal(4887, ObjMonLineMagicCore.NxNySites[0].Line);
        Assert.Equal(4912, ObjMonLineMagicCore.NxNySites[1].Line);
        Assert.Equal("out-param", ObjMonLineMagicCore.NxNySites[1].Kind);
        Assert.Equal(4914, ObjMonLineMagicCore.NxNySites[2].Line);
    }

    // ===================== 五、冷却检查两次 =====================

    [Fact]
    public void DoubleCooldownFacts()
    {
        Assert.True(ObjMonLineMagicCore.CooldownCheckedTwice());
        Assert.True(ObjMonLineMagicCore.SharedTimer());
        Assert.True(ObjMonLineMagicCore.SecondCheckStarved());
        Assert.True(ObjMonLineMagicCore.SilentlySkippedAfterAttack());
        Assert.True(ObjMonLineMagicCore.FirstCheckInsideInner());
        Assert.True(ObjMonLineMagicCore.SecondCheckAfterElse());
        Assert.True(ObjMonLineMagicCore.ChecksAreSeparate());
        Assert.True(ObjMonLineMagicCore.BothResetTick());
    }

    [Fact]
    public void CooldownBoundaries()
    {
        // **刚刷新 -> 假（这正是"第二次检查被饿死"的成因）**
        Assert.True(ObjMonLineMagicCore.JustResetIsFalse());
        Assert.False(ObjMonLineMagicCore.CooldownElapsed(1000, 1000, 500, 0));

        // **超过阈值 -> 真**
        Assert.True(ObjMonLineMagicCore.AfterThresholdIsTrue());

        // **恰好等阈值 -> 假（严格大于）**
        Assert.True(ObjMonLineMagicCore.ExactlyAtThresholdIsFalse());
        Assert.False(ObjMonLineMagicCore.CooldownElapsed(1000, 1500, 500, 0));
        Assert.True(ObjMonLineMagicCore.CooldownElapsed(1000, 1501, 500, 0));
    }

    [Fact]
    public void HardcodedThreeFacts()
    {
        Assert.True(ObjMonLineMagicCore.HardcodedThreeTwice());
        Assert.True(ObjMonLineMagicCore.ComplementaryBounds());
        Assert.True(ObjMonLineMagicCore.CoupledMagicNumber());
        Assert.True(ObjMonLineMagicCore.ComplementOfEngage());

        // **进入判据：|dx|<=3 且 |dy|<=3**
        Assert.True(ObjMonLineMagicCore.EngageInRange(3, 3));
        Assert.False(ObjMonLineMagicCore.EngageInRange(4, 0));

        // **补集：任一轴 > 3 则需要靠近**
        Assert.True(ObjMonLineMagicCore.InsideNeedsNoApproach());
        Assert.True(ObjMonLineMagicCore.ExactlyThreeNoApproach());
        Assert.True(ObjMonLineMagicCore.FourNeedsApproach());
    }

    // ===================== 六、与 J207 对照 =====================

    [Fact]
    public void FamilyComparisonFacts()
    {
        Assert.True(ObjMonLineMagicCore.SameBase());
        Assert.True(ObjMonLineMagicCore.NoPoisonNoHealNoGroup());
        Assert.True(ObjMonLineMagicCore.SingleTargetOnly());
        Assert.True(ObjMonLineMagicCore.PureInheritedShellAgain());
        Assert.True(ObjMonLineMagicCore.VerbatimSameAsJ207());
        Assert.True(ObjMonLineMagicCore.EleventhOccurrence());
        Assert.True(ObjMonLineMagicCore.TwoMethodsOnly());
        Assert.True(ObjMonLineMagicCore.NoCreate());
        Assert.True(ObjMonLineMagicCore.SameShapeAsJ207());
        Assert.True(ObjMonLineMagicCore.ThreeStageShape());
        Assert.True(ObjMonLineMagicCore.UniqueElseBranch());
        Assert.True(ObjMonLineMagicCore.FamilyComparison());
        Assert.True(ObjMonLineMagicCore.NoRandomAtAll());
        Assert.True(ObjMonLineMagicCore.DeterministicEngage());
        Assert.True(ObjMonLineMagicCore.FitsLineSemantics());
        Assert.True(ObjMonLineMagicCore.ThirteenClassesCovered());
        Assert.True(ObjMonLineMagicCore.RemainingApprox());
    }
}
