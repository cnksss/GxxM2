using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J188：`THumMon.ActThink`（103 行）1:1 测试。
/// **本批最重要的产出是发现一处**已存在的移植偏差**：
/// Delphi 的 `GetNextPosition` 用 `case` 加显式标签（非法方向原地不动），
/// 而 C# 侧改用数组查表加 `Math.Min` 夹紧（非法方向被当作 `DR_UPLEFT` 移动）。
/// 另含防重叠段的三重嵌套守卫与职业分派。**
/// </summary>
public sealed class HumMonActThinkCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(103, HumMonActThinkCore.ActThinkLines);
        Assert.Equal(840, HumMonActThinkCore.StartLine);
        Assert.Equal(942, HumMonActThinkCore.EndLine);
        Assert.Equal(5, HumMonActThinkCore.LocalCount);
        Assert.Equal(2000, HumMonActThinkCore.DupModeThrottle);
        Assert.Equal(2, HumMonActThinkCore.ObjCountThreshold);
        Assert.Equal(1, HumMonActThinkCore.NpcCountThreshold);
        Assert.Equal(500, HumMonActThinkCore.DefaultWalkTime);
        Assert.Equal(1, HumMonActThinkCore.AdjacentRadius);
        Assert.Equal(3, HumMonActThinkCore.PatrolRadius);
        Assert.Equal(5, HumMonActThinkCore.ThinkTickSites);
        Assert.Equal(2, HumMonActThinkCore.LastActThinkTickSites);
        Assert.Equal(2, HumMonActThinkCore.ObjCountSites);
        Assert.Equal(1, HumMonActThinkCore.NpcCountSites);
        Assert.Equal(3, HumMonActThinkCore.RandomSites);
        Assert.Equal(0, HumMonActThinkCore.JobWarrior);
        Assert.Equal(0, HumMonActThinkCore.RC_PLAYOBJECT);
        Assert.Equal(8, HumMonActThinkCore.DirectionCount);
        Assert.Equal(0, HumMonActThinkCore.DR_UP);
        Assert.Equal(7, HumMonActThinkCore.DR_UPLEFT);
        Assert.Equal(2020, HumMonActThinkCore.FixYear);
    }

    [Fact]
    public void SpanMatches()
    {
        Assert.True(HumMonActThinkCore.SpanMatchesLineCount());
        Assert.Equal(103, HumMonActThinkCore.EndLine - HumMonActThinkCore.StartLine + 1);
    }

    // ===================== 一、移植偏差 =====================

    [Fact]
    public void DivergenceFacts()
    {
        Assert.True(HumMonActThinkCore.DelphiCaseHasExplicitLabels());
        Assert.True(HumMonActThinkCore.CSideClampsInstead());
        Assert.True(HumMonActThinkCore.MinusOneBlockedInDelphi());
        Assert.True(HumMonActThinkCore.MinusOneMovesUpLeftInCsharp());
        Assert.True(HumMonActThinkCore.DivergenceConfirmed());
    }

    [Fact]
    public void NotFixedHere()
    {
        // **该偏差属既有共享助手、本批只记录不修改**
        Assert.True(HumMonActThinkCore.NotFixedInThisBatch());
        Assert.True(HumMonActThinkCore.SharedHelperRisk());
        Assert.True(HumMonActThinkCore.RecordedForLater());
    }

    [Fact]
    public void IllegalDirectionDiverges()
    {
        Assert.True(HumMonActThinkCore.IllegalDirectionDiverges());

        // **Delphi：非法方向原地不动**
        var delphi = HumMonActThinkCore.DelphiGetNextPosition(10, 10, -1, 1);
        Assert.Equal(10, delphi.X);
        Assert.Equal(10, delphi.Y);

        // **C# 现状：非法方向当左上走**
        var csharp = HumMonActThinkCore.CsharpGetNextPosition(10, 10, -1, 1);
        Assert.Equal(9, csharp.X);
        Assert.Equal(9, csharp.Y);
    }

    [Fact]
    public void LegalDirectionsAgree()
    {
        // **偏差只在非法输入上显现；八个合法方向完全一致**
        Assert.True(HumMonActThinkCore.LegalDirectionsAgree());
        Assert.True(HumMonActThinkCore.AllEightCovered());
    }

    [Fact]
    public void ClampMechanism()
    {
        Assert.True(HumMonActThinkCore.ByteMinusOneIs255());
        Assert.True(HumMonActThinkCore.ClampMapsTo7());

        Assert.Equal(255, unchecked((byte)(-1)));
        Assert.Equal(7, Math.Min(unchecked((byte)(-1)), (byte)7));
    }

    // ---------- 减一与加一的不对称 ----------

    [Fact]
    public void LeftTurnAsymmetry()
    {
        Assert.True(HumMonActThinkCore.CorrectLeftTurnIsPlus7());
        Assert.True(HumMonActThinkCore.DiffersOnlyAtZero());
        Assert.True(HumMonActThinkCore.PlusOneIsSafe());
        Assert.True(HumMonActThinkCore.AsymmetricPair());
    }

    [Fact]
    public void LeftTurnValues()
    {
        Assert.True(HumMonActThinkCore.MinusOneOnlyFailsAtZero());
        Assert.True(HumMonActThinkCore.PlusOneAlwaysInRange());
        Assert.True(HumMonActThinkCore.Plus7MatchesForNonZero());
        Assert.True(HumMonActThinkCore.SevenOfEightAgree());

        // **零号方向：原文得 -1、正确得 7**
        Assert.Equal(-1, HumMonActThinkCore.LeftTurnAsWritten(0));
        Assert.Equal(7, HumMonActThinkCore.LeftTurnCorrect(0));

        // **其余七个方向两者一致**
        Assert.Equal(0, HumMonActThinkCore.LeftTurnAsWritten(1));
        Assert.Equal(0, HumMonActThinkCore.LeftTurnCorrect(1));
        Assert.Equal(6, HumMonActThinkCore.LeftTurnAsWritten(7));
    }

    [Fact]
    public void RightTurnIsSafe()
    {
        Assert.Equal(1, HumMonActThinkCore.RightTurn(0));
        Assert.Equal(0, HumMonActThinkCore.RightTurn(7));
    }

    // ===================== 二、防重叠段 =====================

    [Fact]
    public void DupModeFacts()
    {
        Assert.True(HumMonActThinkCore.ThreeWayDisjunct());
        Assert.True(HumMonActThinkCore.OnlyOneCombinationFallsThrough());
        Assert.True(HumMonActThinkCore.TruthTableVerified());
    }

    [Fact]
    public void DisjunctTruthTable()
    {
        Assert.True(HumMonActThinkCore.DisjunctTruthTable());

        // **仅当 有主人 且 在安全区 且 主人是玩家 时为假**
        Assert.False(HumMonActThinkCore.ShouldCheckObjCount(true, true, 0));
        // **其余七种都为真**
        Assert.True(HumMonActThinkCore.ShouldCheckObjCount(false, true, 0));
        Assert.True(HumMonActThinkCore.ShouldCheckObjCount(true, false, 0));
        Assert.True(HumMonActThinkCore.ShouldCheckObjCount(true, true, 1));
    }

    [Fact]
    public void TwoCounts()
    {
        Assert.True(HumMonActThinkCore.TwoDifferentCounts());
        Assert.True(HumMonActThinkCore.ThresholdsTwoVersusOne());
        Assert.True(HumMonActThinkCore.CountSites());
        Assert.True(HumMonActThinkCore.NpcCountUniqueHere());
        Assert.True(HumMonActThinkCore.CountThresholdBoundaries());

        Assert.True(HumMonActThinkCore.DupByObjCount(2));
        Assert.False(HumMonActThinkCore.DupByObjCount(1));
        Assert.True(HumMonActThinkCore.DupByNpcCount(1));
        Assert.False(HumMonActThinkCore.DupByNpcCount(0));
    }

    [Fact]
    public void DatedFix()
    {
        Assert.True(HumMonActThinkCore.DatedFix2020());
        Assert.True(HumMonActThinkCore.PreciseTimestamp());
        Assert.Equal(2020, HumMonActThinkCore.FixYear);
    }

    [Fact]
    public void DupWalk()
    {
        Assert.True(HumMonActThinkCore.RandomEightDirection());
        Assert.True(HumMonActThinkCore.CompareCoordsToDetect());
        Assert.True(HumMonActThinkCore.EarlyExitOnSuccess());
        Assert.True(HumMonActThinkCore.DupWalkSucceededValues());

        Assert.True(HumMonActThinkCore.DupWalkSucceeded(5, 5, 6, 5));
        Assert.True(HumMonActThinkCore.DupWalkSucceeded(5, 5, 5, 6));
        // **原地不算成功**
        Assert.False(HumMonActThinkCore.DupWalkSucceeded(5, 5, 5, 5));
    }

    [Fact]
    public void Throttle()
    {
        Assert.True(HumMonActThinkCore.TwoSecondThrottle());
        Assert.True(HumMonActThinkCore.TickRefreshedBeforeCheck());
        Assert.True(HumMonActThinkCore.ThrottlesEvenWhenNoAction());
        Assert.True(HumMonActThinkCore.DupThrottleBoundary());

        // **含端点：恰好两千毫秒即通过**
        Assert.False(HumMonActThinkCore.DupThrottleElapsed(1999, 0));
        Assert.True(HumMonActThinkCore.DupThrottleElapsed(2000, 0));
    }

    [Fact]
    public void ThinkTick()
    {
        Assert.True(HumMonActThinkCore.ThinkTickSitesCount());
        Assert.True(HumMonActThinkCore.PerClassCopies());
        Assert.Equal(new[] { 24, 84, 129, 854, 856 }, HumMonActThinkCore.ThinkTickLines);
    }

    // ===================== 三、职业分派 =====================

    [Fact]
    public void JobCaseFacts()
    {
        Assert.True(HumMonActThinkCore.ThirdOccurrence());
        Assert.True(HumMonActThinkCore.MonsterWalkTimesNotHero());
        Assert.True(HumMonActThinkCore.DefaultFiveHundredAgain());
        Assert.True(HumMonActThinkCore.SameDefaultDifferentGroups());
        Assert.True(HumMonActThinkCore.WalkTimeByJobValues());
    }

    [Fact]
    public void WalkTimeByJob()
    {
        Assert.Equal(700, HumMonActThinkCore.WalkTimeByJob(0, 700, 800, 900));
        Assert.Equal(800, HumMonActThinkCore.WalkTimeByJob(1, 700, 800, 900));
        Assert.Equal(900, HumMonActThinkCore.WalkTimeByJob(2, 700, 800, 900));
        // **缺省五百（第三次出现同一缺省）**
        Assert.Equal(500, HumMonActThinkCore.WalkTimeByJob(3, 700, 800, 900));
    }

    [Fact]
    public void Thresholds()
    {
        Assert.True(HumMonActThinkCore.TwoThresholds());
        Assert.True(HumMonActThinkCore.BothStrict());
        Assert.True(HumMonActThinkCore.DifferentBaseFields());
        Assert.True(HumMonActThinkCore.MoveTimeBoundary());

        // **严格大于：恰好等于行走时间不通过**
        Assert.False(HumMonActThinkCore.MoveTimeElapsed(500, 0, 500));
        Assert.True(HumMonActThinkCore.MoveTimeElapsed(501, 0, 500));
    }

    [Fact]
    public void LastActThinkTick()
    {
        Assert.True(HumMonActThinkCore.LastActThinkTickTwoRefs());
        Assert.True(HumMonActThinkCore.HighlySpecialized());
        Assert.Equal(new[] { 893, 914 }, HumMonActThinkCore.LastActThinkTickLines);
    }

    // ===================== 四、绕目标攻击与收尾 =====================

    [Fact]
    public void CircleFacts()
    {
        Assert.True(HumMonActThinkCore.FourConjuncts());
        Assert.True(HumMonActThinkCore.JobZeroOnly());
        Assert.True(HumMonActThinkCore.CircleRequiresAllFour());
    }

    [Fact]
    public void CircleModel()
    {
        // **四条件全满足**
        Assert.True(HumMonActThinkCore.ShouldCircleTarget(0, true, 1000, 0, 500, 0));
        // **逐个破坏即不成立**
        Assert.False(HumMonActThinkCore.ShouldCircleTarget(1, true, 1000, 0, 500, 0));
        Assert.False(HumMonActThinkCore.ShouldCircleTarget(0, false, 1000, 0, 500, 0));
        Assert.False(HumMonActThinkCore.ShouldCircleTarget(0, true, 500, 0, 500, 0));
        Assert.False(HumMonActThinkCore.ShouldCircleTarget(0, true, 1000, 0, 500, 1));
    }

    [Fact]
    public void Adjacency()
    {
        Assert.True(HumMonActThinkCore.ChebyshevOne());
        Assert.True(HumMonActThinkCore.NineCells());
        Assert.True(HumMonActThinkCore.AdjacentCoversNineCells());
        Assert.True(HumMonActThinkCore.DiagonalCountsAsAdjacent());

        // **对角相邻算贴身（切比雪夫）**
        Assert.True(HumMonActThinkCore.IsAdjacent(1, 1));
        Assert.True(HumMonActThinkCore.IsAdjacent(1, 0));
        // **曼哈顿距离二但切比雪夫二 —— 不算**
        Assert.False(HumMonActThinkCore.IsAdjacent(2, 0));
    }

    [Fact]
    public void Deflection()
    {
        Assert.True(HumMonActThinkCore.RandomLeftOrRight());
        Assert.True(HumMonActThinkCore.WalkabilityCheckedLater());
        Assert.True(HumMonActThinkCore.DeflectValues());
        // **零号方向左偏即产生非法值**
        Assert.True(HumMonActThinkCore.DeflectCanProduceIllegal());

        Assert.Equal(4, HumMonActThinkCore.Deflect(3, 0));
        Assert.Equal(2, HumMonActThinkCore.Deflect(3, 1));
        Assert.Equal(-1, HumMonActThinkCore.Deflect(0, 1));
    }

    [Fact]
    public void CircleStep()
    {
        Assert.True(HumMonActThinkCore.SetThenGoto());
        Assert.True(HumMonActThinkCore.MustDifferFromCurrent());
        Assert.True(HumMonActThinkCore.CircleStepViableValues());

        Assert.True(HumMonActThinkCore.CircleStepViable(6, 5, 5, 5, true));
        // **原地不可行**
        Assert.False(HumMonActThinkCore.CircleStepViable(5, 5, 5, 5, true));
        // **不可走不可行**
        Assert.False(HumMonActThinkCore.CircleStepViable(6, 5, 5, 5, false));
    }

    [Fact]
    public void TailDispatch()
    {
        Assert.True(HumMonActThinkCore.JobZeroDelegatesToInherited());
        Assert.True(HumMonActThinkCore.NonZeroPatrolsRadius3());
        Assert.True(HumMonActThinkCore.ProtectModeGatesPatrol());
        Assert.True(HumMonActThinkCore.MovePointRadius3());
        Assert.True(HumMonActThinkCore.IndexResetToZero());
        Assert.True(HumMonActThinkCore.InheritedOnlyForJobZero());
        Assert.True(HumMonActThinkCore.TailDispatchValues());
        Assert.True(HumMonActThinkCore.JobZeroIgnoresProtectMode());

        Assert.Equal("inherited", HumMonActThinkCore.TailDispatch(0, false));
        Assert.Equal("patrol", HumMonActThinkCore.TailDispatch(1, true));
        Assert.Equal("none", HumMonActThinkCore.TailDispatch(1, false));
    }

    [Fact]
    public void ExceptionLog()
    {
        Assert.True(HumMonActThinkCore.TwoLogLines());
        Assert.True(HumMonActThinkCore.NoErrCodeHere());
        Assert.True(HumMonActThinkCore.ConsistentWithJ187NotJ186());
        Assert.True(HumMonActThinkCore.BuildExceptionLogValues());
        // **原文方法名后有尾随空格**
        Assert.True(HumMonActThinkCore.TrailingSpaceInMessage());

        string[] log = HumMonActThinkCore.BuildExceptionLog("boom");
        Assert.Equal("[Exception] THumMon:ActThink ", log[0]);
        Assert.Equal("boom", log[1]);
    }

    [Fact]
    public void TryScope()
    {
        Assert.True(HumMonActThinkCore.TryScopeIsTailOnly());
        Assert.True(HumMonActThinkCore.EarlierSectionsUnprotected());
    }

    [Fact]
    public void ResultsAndRandoms()
    {
        Assert.True(HumMonActThinkCore.ThreeResultTruePaths());
        Assert.True(HumMonActThinkCore.EachTerminal());
        Assert.True(HumMonActThinkCore.ThreeRandoms());
        Assert.True(HumMonActThinkCore.DifferentModuliFromJ186());
        Assert.True(HumMonActThinkCore.FiveLocals());

        Assert.Equal(3, HumMonActThinkCore.RandomModuli.Length);
    }
}
