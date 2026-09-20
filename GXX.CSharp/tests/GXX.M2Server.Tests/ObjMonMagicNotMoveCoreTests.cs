using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J220：`ObjMon.pas` 中 `TMagicAttackNotMoveMonster`（真狐月天珠）
/// **四个方法**的 1:1 测试（`Create`/`Destroy`/`CallSlave`/`Run`，合计 91 行）。
/// 最大的 `AttackTarget`（277 行）留待批次J221。
/// **本批最有价值的发现**：
/// ① `CallSlave` 的注释说 `3--7格`、而 `3 + Random(4)` 只给 `3..6`；
/// ② `m_boCalledSlave := True` 无条件执行 —— 四只全失败也会永久阻止重试；
/// ③ `Run` 里有两处死判据（`Max` 之后的负值检查、`downto` 循环里的 `Count <= 0`）。
/// </summary>
public sealed class ObjMonMagicNotMoveCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(6555, ObjMonMagicNotMoveCore.CreateStart);
        Assert.Equal(6562, ObjMonMagicNotMoveCore.CreateEnd);
        Assert.Equal(8, ObjMonMagicNotMoveCore.CreateLines);
        Assert.Equal(6564, ObjMonMagicNotMoveCore.DestroyStart);
        Assert.Equal(6568, ObjMonMagicNotMoveCore.DestroyEnd);
        Assert.Equal(5, ObjMonMagicNotMoveCore.DestroyLines);
        Assert.Equal(6571, ObjMonMagicNotMoveCore.CallStart);
        Assert.Equal(6602, ObjMonMagicNotMoveCore.CallEnd);
        Assert.Equal(32, ObjMonMagicNotMoveCore.CallLines);
        Assert.Equal(6881, ObjMonMagicNotMoveCore.RunStart);
        Assert.Equal(6926, ObjMonMagicNotMoveCore.RunEnd);
        Assert.Equal(46, ObjMonMagicNotMoveCore.RunLines);
        Assert.Equal(91, ObjMonMagicNotMoveCore.TotalLines);
        Assert.Equal(367, ObjMonMagicNotMoveCore.ClassTotalLines);
        Assert.Equal(276, ObjMonMagicNotMoveCore.J221AttackLines);
        Assert.Equal(6604, ObjMonMagicNotMoveCore.AttackStart);
        Assert.Equal(6880, ObjMonMagicNotMoveCore.AttackEnd);

        Assert.Equal(6557, ObjMonMagicNotMoveCore.CreateInheritedLine);
        Assert.Equal(6558, ObjMonMagicNotMoveCore.LastStepInitLine);
        Assert.Equal(6559, ObjMonMagicNotMoveCore.FrozenTickInitLine);
        Assert.Equal(6560, ObjMonMagicNotMoveCore.CalledSlaveInitLine);
        Assert.Equal(6561, ObjMonMagicNotMoveCore.ListCreateLine);
        Assert.Equal(6566, ObjMonMagicNotMoveCore.DestroyFreeLine);
        Assert.Equal(6567, ObjMonMagicNotMoveCore.DestroyInheritedLine);
        Assert.Equal(29, ObjMonMagicNotMoveCore.DestructorCount);
        Assert.Equal(26, ObjMonMagicNotMoveCore.DestroyInheritedFirstCount);
        Assert.Equal(3, ObjMonMagicNotMoveCore.DestroyInheritedNotFirstCount);
        Assert.Equal(239, ObjMonMagicNotMoveCore.FrozenTickDeclLine);
        Assert.Equal(240, ObjMonMagicNotMoveCore.SlaveListDeclLine);
        Assert.Equal(241, ObjMonMagicNotMoveCore.CalledSlaveDeclLine);
        Assert.Equal(238, ObjMonMagicNotMoveCore.LastStepDeclLine);
        Assert.Equal(4, ObjMonMagicNotMoveCore.PrivateFieldCount);

        Assert.Equal(6577, ObjMonMagicNotMoveCore.CallGuardLine);
        Assert.Equal(6579, ObjMonMagicNotMoveCore.RangeLine);
        Assert.Equal(3, ObjMonMagicNotMoveCore.RangeBase);
        Assert.Equal(4, ObjMonMagicNotMoveCore.RangeBound);
        Assert.Equal(3, ObjMonMagicNotMoveCore.RangeMin);
        Assert.Equal(6, ObjMonMagicNotMoveCore.RangeMax);
        Assert.Equal(7, ObjMonMagicNotMoveCore.CommentRangeMax);
        Assert.Equal(5, ObjMonMagicNotMoveCore.J219RangeBound);
        Assert.Equal(7, ObjMonMagicNotMoveCore.J219RangeMax);
        Assert.Equal(6580, ObjMonMagicNotMoveCore.FrontPosLine);
        Assert.Equal(4, ObjMonMagicNotMoveCore.SlotCount);
        Assert.Equal(2806, ObjMonMagicNotMoveCore.FoxBeasDeclLine);
        Assert.Equal(5393, ObjMonMagicNotMoveCore.FoxBeasDefaultLine);
        Assert.Equal(248, ObjMonMagicNotMoveCore.RegenDeclLine);
        Assert.Equal(6601, ObjMonMagicNotMoveCore.CalledSlaveSetLine);
        Assert.Equal(524, ObjMonMagicNotMoveCore.FrontPosDeclLine);
        Assert.Equal(131, ObjMonMagicNotMoveCore.WAbilDeclLine);

        Assert.Equal(6887, ObjMonMagicNotMoveCore.GuardLine);
        Assert.Equal(6906, ObjMonMagicNotMoveCore.GuardEndLine);
        Assert.Equal(6889, ObjMonMagicNotMoveCore.ThrottleLine);
        Assert.Equal(8000, ObjMonMagicNotMoveCore.SearchWithTargetMs);
        Assert.Equal(1000, ObjMonMagicNotMoveCore.SearchWithoutTargetMs);
        Assert.Equal(6893, ObjMonMagicNotMoveCore.SearchTargetLine);
        Assert.Equal(6896, ObjMonMagicNotMoveCore.AttackCallLine);
        Assert.Equal(6897, ObjMonMagicNotMoveCore.StepCommentLine);
        Assert.Equal(6898, ObjMonMagicNotMoveCore.StepComputeLine);
        Assert.Equal(6899, ObjMonMagicNotMoveCore.DeadNegativeLine);
        Assert.Equal(6901, ObjMonMagicNotMoveCore.StepCompareLine);
        Assert.Equal(6903, ObjMonMagicNotMoveCore.StepSendLine);
        Assert.Equal(6904, ObjMonMagicNotMoveCore.StepCacheLine);
        Assert.Equal(5, ObjMonMagicNotMoveCore.StepCount);
        Assert.Equal(4, ObjMonMagicNotMoveCore.StepBase);
        Assert.Equal(5, ObjMonMagicNotMoveCore.HpDivisor);
        Assert.Equal(20234, ObjMonMagicNotMoveCore.RM_EFFECTSTEP);
        Assert.Equal(1188, ObjMonMagicNotMoveCore.EffectStepDeclLine);
        Assert.Equal(6907, ObjMonMagicNotMoveCore.CleanupStart);
        Assert.Equal(6921, ObjMonMagicNotMoveCore.CleanupEnd);
        Assert.Equal(6908, ObjMonMagicNotMoveCore.CleanupNilCheckLine);
        Assert.Equal(6910, ObjMonMagicNotMoveCore.CleanupLoopLine);
        Assert.Equal(6912, ObjMonMagicNotMoveCore.DeadBreakLine);
        Assert.Equal(6918, ObjMonMagicNotMoveCore.DeleteLine);
        Assert.Equal(6922, ObjMonMagicNotMoveCore.FinalInheritedLine);

        Assert.Equal(251, ObjMonMagicNotMoveCore.SiblingClassLine);
        Assert.Equal(236, ObjMonMagicNotMoveCore.ClassDeclLine);
        Assert.Equal(235, ObjMonMagicNotMoveCore.FactionCommentLine);
        Assert.Equal(250, ObjMonMagicNotMoveCore.SiblingFactionCommentLine);
        Assert.Equal(6927, ObjMonMagicNotMoveCore.SiblingCreateLine);
        Assert.Equal(257, ObjMonMagicNotMoveCore.SiblingExtraFieldLine);
        Assert.Equal(6942, ObjMonMagicNotMoveCore.SiblingInitializeLine);
        Assert.Equal(26, ObjMonMagicNotMoveCore.ClassesCovered);
        Assert.Equal(28, ObjMonMagicNotMoveCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonMagicNotMoveCore.SpanMatches());
        Assert.True(ObjMonMagicNotMoveCore.TotalLinesAddUp());
        Assert.True(ObjMonMagicNotMoveCore.ClassTotalAddsUp());
        Assert.True(ObjMonMagicNotMoveCore.MethodsAscending());
        Assert.True(ObjMonMagicNotMoveCore.MethodsContiguous());
        Assert.True(ObjMonMagicNotMoveCore.WithinUnit());
        Assert.True(ObjMonMagicNotMoveCore.NoInstrumentation());
        Assert.True(ObjMonMagicNotMoveCore.CleanupSpanMatches());
    }

    // ===================== 一、Create 与 Destroy =====================

    [Fact]
    public void CreateAndDestroyOrderFacts()
    {
        Assert.True(ObjMonMagicNotMoveCore.InheritedFirstInCreate());
        Assert.True(ObjMonMagicNotMoveCore.NormalCreate());
        Assert.True(ObjMonMagicNotMoveCore.FourFieldsInitialised());
        Assert.True(ObjMonMagicNotMoveCore.ListCreatedLast());
        Assert.True(ObjMonMagicNotMoveCore.FieldDeclsAscending());
        Assert.True(ObjMonMagicNotMoveCore.FieldCountMatches());
        Assert.True(ObjMonMagicNotMoveCore.FreesSlaveListBeforeInherited());
        Assert.True(ObjMonMagicNotMoveCore.ThreeOfTwentyNine());
        Assert.True(ObjMonMagicNotMoveCore.AllThreeStartWithSlaveListFree());
        Assert.True(ObjMonMagicNotMoveCore.SharedReason());
        Assert.True(ObjMonMagicNotMoveCore.CorrectDelphiOrder());
        Assert.True(ObjMonMagicNotMoveCore.ContrastWithJ219Create());
        Assert.True(ObjMonMagicNotMoveCore.SlaveListFreeLinesChecked());
        Assert.True(ObjMonMagicNotMoveCore.DestructorCountsAddUp());
        Assert.True(ObjMonMagicNotMoveCore.RatioIsTwentySixToThree());
        Assert.True(ObjMonMagicNotMoveCore.FrozenTickWriteOnlyInThisBatch());
        Assert.True(ObjMonMagicNotMoveCore.NoConclusionUntilJ221());
    }

    [Fact]
    public void DestroyOrderingArithmetic()
    {
        // **26 + 3 = 29**
        Assert.Equal(ObjMonMagicNotMoveCore.DestructorCount,
            ObjMonMagicNotMoveCore.DestroyInheritedFirstCount
            + ObjMonMagicNotMoveCore.DestroyInheritedNotFirstCount);

        // **三个少数派的行号**
        Assert.Equal(3, ObjMonMagicNotMoveCore.SlaveListFreeFirstLines.Length);
        Assert.Equal(2549, ObjMonMagicNotMoveCore.SlaveListFreeFirstLines[0]);
        Assert.Equal(6564, ObjMonMagicNotMoveCore.SlaveListFreeFirstLines[1]);
        Assert.Equal(6936, ObjMonMagicNotMoveCore.SlaveListFreeFirstLines[2]);

        // **`Free` 在 `inherited` 之前**
        Assert.True(ObjMonMagicNotMoveCore.DestroyFreeLine
            < ObjMonMagicNotMoveCore.DestroyInheritedLine);

        // **`Create` 里 `inherited` 在前（与 J219 相反）**
        Assert.True(ObjMonMagicNotMoveCore.CreateInheritedLine
            < ObjMonMagicNotMoveCore.LastStepInitLine);
    }

    // ===================== 二、CallSlave =====================

    [Fact]
    public void RangeCommentMismatchFacts()
    {
        Assert.True(ObjMonMagicNotMoveCore.CommentSaysThreeToSeven());
        Assert.True(ObjMonMagicNotMoveCore.CodeGivesThreeToSix());
        Assert.True(ObjMonMagicNotMoveCore.OffByOneInComment());
        Assert.True(ObjMonMagicNotMoveCore.CommentIsTheWrongOne());
        Assert.True(ObjMonMagicNotMoveCore.MaxIsNotSeven());
        Assert.True(ObjMonMagicNotMoveCore.J219AlsoThreeToSeven());
        Assert.True(ObjMonMagicNotMoveCore.J219UsesDifferentBound());
        Assert.True(ObjMonMagicNotMoveCore.CopiedCommentNotFormula());
    }

    [Fact]
    public void RangeBoundaries()
    {
        Assert.True(ObjMonMagicNotMoveCore.MinRange());
        Assert.True(ObjMonMagicNotMoveCore.MaxRange());

        Assert.Equal(3, ObjMonMagicNotMoveCore.Range(0));
        Assert.Equal(4, ObjMonMagicNotMoveCore.Range(1));
        Assert.Equal(5, ObjMonMagicNotMoveCore.Range(2));
        Assert.Equal(6, ObjMonMagicNotMoveCore.Range(3));

        // **恰好差一格**
        Assert.Equal(1, ObjMonMagicNotMoveCore.CommentRangeMax
            - ObjMonMagicNotMoveCore.RangeMax);
        Assert.Equal(1, ObjMonMagicNotMoveCore.J219RangeBound
            - ObjMonMagicNotMoveCore.RangeBound);
    }

    [Fact]
    public void BeastSlotFacts()
    {
        Assert.True(ObjMonMagicNotMoveCore.FourSlotsNamedAfterBeasts());
        Assert.True(ObjMonMagicNotMoveCore.DefaultAllSameMonster());
        Assert.True(ObjMonMagicNotMoveCore.AllFourDefaultsIdentical());
        Assert.True(ObjMonMagicNotMoveCore.SlotsAreConfigurable());
        Assert.True(ObjMonMagicNotMoveCore.CommentPromisesFour());

        Assert.Equal(new[] { "青龙", "白虎", "朱雀", "玄武" },
            ObjMonMagicNotMoveCore.BeastNames);
        Assert.Equal(4, ObjMonMagicNotMoveCore.FoxBeasDefault.Length);
        Assert.All(ObjMonMagicNotMoveCore.FoxBeasDefault,
            s => Assert.Equal("MON33-7", s));
    }

    [Fact]
    public void SpawnPatternFacts()
    {
        Assert.True(ObjMonMagicNotMoveCore.CrossPattern());
        Assert.True(ObjMonMagicNotMoveCore.EastWestSouthNorthOrder());
        Assert.True(ObjMonMagicNotMoveCore.NotClockwise());
        Assert.True(ObjMonMagicNotMoveCore.SameAsJ219NonClockwise());
        Assert.True(ObjMonMagicNotMoveCore.CenteredOnFrontPosition());
        Assert.True(ObjMonMagicNotMoveCore.SpawnOffsetsDisjoint());
        Assert.True(ObjMonMagicNotMoveCore.RegenLinesExtracted());

        Assert.Equal(4, ObjMonMagicNotMoveCore.SpawnOffsets.Length);
        Assert.Equal((1, 0), (ObjMonMagicNotMoveCore.SpawnOffsets[0].Dx,
            ObjMonMagicNotMoveCore.SpawnOffsets[0].Dy));
        Assert.Equal((-1, 0), (ObjMonMagicNotMoveCore.SpawnOffsets[1].Dx,
            ObjMonMagicNotMoveCore.SpawnOffsets[1].Dy));
        Assert.Equal((0, 1), (ObjMonMagicNotMoveCore.SpawnOffsets[2].Dx,
            ObjMonMagicNotMoveCore.SpawnOffsets[2].Dy));
        Assert.Equal((0, -1), (ObjMonMagicNotMoveCore.SpawnOffsets[3].Dx,
            ObjMonMagicNotMoveCore.SpawnOffsets[3].Dy));

        Assert.Equal(new[] { 6581, 6586, 6591, 6596 },
            ObjMonMagicNotMoveCore.RegenLines);
    }

    [Fact]
    public void TotalFailureFlagFacts()
    {
        Assert.True(ObjMonMagicNotMoveCore.NilCheckedBeforeAdd());
        Assert.True(ObjMonMagicNotMoveCore.FlagSetUnconditionally());
        Assert.True(ObjMonMagicNotMoveCore.FlagOutsideAnyIf());
        Assert.True(ObjMonMagicNotMoveCore.GuardNeverRetriesOnTotalFailure());
        Assert.True(ObjMonMagicNotMoveCore.FlagMeansAttemptedNotSucceeded());
        Assert.True(ObjMonMagicNotMoveCore.ContrastWithJ215WonderingEx());
        Assert.True(ObjMonMagicNotMoveCore.TotalFailureBlocksRetry());
        Assert.True(ObjMonMagicNotMoveCore.DoubleGuard());
        Assert.True(ObjMonMagicNotMoveCore.HalvesAreRedundant());
        Assert.True(ObjMonMagicNotMoveCore.CountCheckNeverDecisiveAlone());

        Assert.Equal(6601, ObjMonMagicNotMoveCore.CalledSlaveSetLine);
    }

    [Fact]
    public void CallGuardBoundaries()
    {
        Assert.True(ObjMonMagicNotMoveCore.NonEmptyListExits());
        Assert.True(ObjMonMagicNotMoveCore.FlagExits());
        Assert.True(ObjMonMagicNotMoveCore.OnlyBothFalseProceeds());

        Assert.True(ObjMonMagicNotMoveCore.ShouldExitCall(true, false));
        Assert.True(ObjMonMagicNotMoveCore.ShouldExitCall(false, true));
        Assert.True(ObjMonMagicNotMoveCore.ShouldExitCall(true, true));
        Assert.False(ObjMonMagicNotMoveCore.ShouldExitCall(false, false));
    }

    // ===================== 三、Run =====================

    [Fact]
    public void RunSkeletonFacts()
    {
        Assert.True(ObjMonMagicNotMoveCore.SameFiveFoldGuard());
        Assert.True(ObjMonMagicNotMoveCore.SameThrottle());
        Assert.True(ObjMonMagicNotMoveCore.AttackTargetWithoutParens());
        Assert.True(ObjMonMagicNotMoveCore.AllTrueRuns());
        Assert.True(ObjMonMagicNotMoveCore.AnyBlocks());
        Assert.True(ObjMonMagicNotMoveCore.SearchAfterEightWithTarget());
        Assert.True(ObjMonMagicNotMoveCore.ExactlyEightBlocks());
    }

    [Fact]
    public void RunGuardAndSearchBoundaries()
    {
        Assert.True(ObjMonMagicNotMoveCore.CanRun(false, false, false, false, true));
        Assert.False(ObjMonMagicNotMoveCore.CanRun(true, false, false, false, true));
        Assert.False(ObjMonMagicNotMoveCore.CanRun(false, true, false, false, true));
        Assert.False(ObjMonMagicNotMoveCore.CanRun(false, false, true, false, true));
        Assert.False(ObjMonMagicNotMoveCore.CanRun(false, false, false, true, true));
        Assert.False(ObjMonMagicNotMoveCore.CanRun(false, false, false, false, false));

        Assert.True(ObjMonMagicNotMoveCore.ShouldSearch(8001, true));
        Assert.False(ObjMonMagicNotMoveCore.ShouldSearch(8000, true));
        Assert.True(ObjMonMagicNotMoveCore.ShouldSearch(1001, false));
        Assert.False(ObjMonMagicNotMoveCore.ShouldSearch(1000, false));
    }

    // ---------- 两处死判据 ----------

    [Fact]
    public void DeadCheckFacts()
    {
        Assert.True(ObjMonMagicNotMoveCore.DeadNegativeCheck());
        Assert.True(ObjMonMagicNotMoveCore.MaxAlreadyClamps());
        Assert.True(ObjMonMagicNotMoveCore.DeadBreakCheck());
        Assert.True(ObjMonMagicNotMoveCore.UnreachableInDowntoLoop());
        Assert.True(ObjMonMagicNotMoveCore.DescendingDeleteIsCorrect());
        Assert.True(ObjMonMagicNotMoveCore.SecondDeadCheckInThisBatch());
        Assert.True(ObjMonMagicNotMoveCore.DeadChecksThirteenApart());
        Assert.True(ObjMonMagicNotMoveCore.ClampedNeverNegative());
        Assert.True(ObjMonMagicNotMoveCore.NegativeCheckAlwaysFalse());
        Assert.True(ObjMonMagicNotMoveCore.CountNeverZeroInside());
        Assert.True(ObjMonMagicNotMoveCore.NonEmptyAlwaysAtLeastOne());

        // **两处死判据的间距**
        Assert.Equal(13, ObjMonMagicNotMoveCore.DeadBreakLine
            - ObjMonMagicNotMoveCore.DeadNegativeLine);
    }

    // ---------- 除零 ----------

    [Fact]
    public void DivisionRiskFacts()
    {
        Assert.True(ObjMonMagicNotMoveCore.DivisorIsMaxHpDivFive());
        Assert.True(ObjMonMagicNotMoveCore.ZeroWhenMaxHpBelowFive());
        Assert.True(ObjMonMagicNotMoveCore.DivisionByZeroRisk());
        Assert.True(ObjMonMagicNotMoveCore.ReachableViaWeakMonster());
        Assert.True(ObjMonMagicNotMoveCore.NormalDivisor());
        Assert.True(ObjMonMagicNotMoveCore.WeakMonsterGivesZero());
        Assert.True(ObjMonMagicNotMoveCore.FiveIsSafeLowerBound());

        Assert.Equal(20, ObjMonMagicNotMoveCore.Divisor(100));
        Assert.Equal(0, ObjMonMagicNotMoveCore.Divisor(4));
        Assert.Equal(1, ObjMonMagicNotMoveCore.Divisor(5));
    }

    // ---------- 阶段值 ----------

    [Fact]
    public void StepValueFacts()
    {
        Assert.True(ObjMonMagicNotMoveCore.RangeZeroToFour());
        Assert.True(ObjMonMagicNotMoveCore.FiveStages());
        Assert.True(ObjMonMagicNotMoveCore.InverseToHp());
        Assert.True(ObjMonMagicNotMoveCore.FullHpIsZero());
        Assert.True(ObjMonMagicNotMoveCore.EmptyHpIsFour());
        Assert.True(ObjMonMagicNotMoveCore.InitZeroMatchesFullHp());
        Assert.True(ObjMonMagicNotMoveCore.NoMessageWhileFullHp());
        Assert.True(ObjMonMagicNotMoveCore.SendsOnlyAfterDamage());
        Assert.True(ObjMonMagicNotMoveCore.StepSendLineChecked());
        Assert.True(ObjMonMagicNotMoveCore.SendsZeroCoordinates());
        Assert.True(ObjMonMagicNotMoveCore.ContrastWithOtherSendRefMsg());
        Assert.True(ObjMonMagicNotMoveCore.EffectStepIs20234());
        Assert.True(ObjMonMagicNotMoveCore.EffectStepDeclChecked());
        Assert.True(ObjMonMagicNotMoveCore.StepCachedToAvoidResend());
    }

    [Fact]
    public void StepBoundaries()
    {
        Assert.Equal(0, ObjMonMagicNotMoveCore.Step(100, 100));
        Assert.Equal(2, ObjMonMagicNotMoveCore.Step(50, 100));
        Assert.Equal(4, ObjMonMagicNotMoveCore.Step(0, 100));

        // **档位不超过 4**
        Assert.True(ObjMonMagicNotMoveCore.Step(0, 5) <= ObjMonMagicNotMoveCore.StepBase);
        Assert.True(ObjMonMagicNotMoveCore.Step(5, 5) >= 0);
    }

    // ---------- 清理段 ----------

    [Fact]
    public void CleanupFacts()
    {
        Assert.True(ObjMonMagicNotMoveCore.CleanupOutsideGuard());
        Assert.True(ObjMonMagicNotMoveCore.InheritedUnconditional());
        Assert.True(ObjMonMagicNotMoveCore.Deliberate());
        Assert.True(ObjMonMagicNotMoveCore.ContrastWithJ213ConditionalInherited());
        Assert.True(ObjMonMagicNotMoveCore.CleanupNilChecked());
        Assert.True(ObjMonMagicNotMoveCore.DeadRemoved());
        Assert.True(ObjMonMagicNotMoveCore.GhostRemoved());
        Assert.True(ObjMonMagicNotMoveCore.AliveKept());

        Assert.True(ObjMonMagicNotMoveCore.ShouldRemoveSlave(true, false));
        Assert.True(ObjMonMagicNotMoveCore.ShouldRemoveSlave(false, true));
        Assert.True(ObjMonMagicNotMoveCore.ShouldRemoveSlave(true, true));
        Assert.False(ObjMonMagicNotMoveCore.ShouldRemoveSlave(false, false));
    }

    // ===================== 四、整体与姊妹类 =====================

    [Fact]
    public void SiblingFacts()
    {
        Assert.True(ObjMonMagicNotMoveCore.TwentySixClassesCovered());
        Assert.True(ObjMonMagicNotMoveCore.RemainingApprox());
        Assert.True(ObjMonMagicNotMoveCore.PartialUntilJ221());
        Assert.True(ObjMonMagicNotMoveCore.SiblingClassFollows());
        Assert.True(ObjMonMagicNotMoveCore.SameFourFields());
        Assert.True(ObjMonMagicNotMoveCore.ExtraFieldAndMethod());
        Assert.True(ObjMonMagicNotMoveCore.AnotherSiblingPair());
        Assert.True(ObjMonMagicNotMoveCore.IdenticalAbilityComment());
        Assert.True(ObjMonMagicNotMoveCore.DifferentFactionComment());
        Assert.True(ObjMonMagicNotMoveCore.FactionCommentsFifteenApart());
        Assert.True(ObjMonMagicNotMoveCore.SameFamilyAsJ218());
        Assert.True(ObjMonMagicNotMoveCore.ClassDeclChecked());
        Assert.True(ObjMonMagicNotMoveCore.SiblingCreateChecked());
    }
}
