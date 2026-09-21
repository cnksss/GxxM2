using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J247：`ObjMon.pas` 中 `TScultureKingMonster`（祖玛教主）六方法的 1:1 测试（136 行）。
/// **本批最有价值的发现**：
/// ① `Destroy` 是本系列那个**少数派析构**（先释放成员再 `inherited`）的**第二个落地点** ——
///    2549 正是 J220 用脚本归纳的 **3 个例外之一**（2549/6564/6936）；
/// ② **2598-2599 是 J212 别名谱系的第五个真现场**（13 处里已有五处落地），
///    且它调 `HitMagAttackTarget` 时**第一个实参传 `0`**（J243 传的是 `nPower div 2` 两次）；
/// ③ 危险度判据 `m_WAbil.HP / m_WAbil.MaxHP * 5` 因**整除**只取 `0` 或 `5`
///    ⇒ **"满血"那一支永假（判据退化）** —— 形态⑥ 的**第四种后果**；
/// ④ 同一个 `TGameEvent.Create` 的半径实参有**三个取值（1/1/6）**，
///    且本处被注释的两行里出现的**外观值 `218`** 正是 J246 的"魔龙教主"。
/// </summary>
public sealed class ObjMonScultureKingCoreTests
{
    [Fact]
    public void Constants()
    {
        Assert.Equal(2537, ObjMonScultureKingCore.CreateStart);
        Assert.Equal(2547, ObjMonScultureKingCore.CreateEnd);
        Assert.Equal(11, ObjMonScultureKingCore.CreateLines);
        Assert.Equal(2549, ObjMonScultureKingCore.DestroyStart);
        Assert.Equal(2553, ObjMonScultureKingCore.DestroyEnd);
        Assert.Equal(5, ObjMonScultureKingCore.DestroyLines);
        Assert.Equal(2555, ObjMonScultureKingCore.MeltStart);
        Assert.Equal(2568, ObjMonScultureKingCore.MeltEnd);
        Assert.Equal(14, ObjMonScultureKingCore.MeltLines);
        Assert.Equal(2570, ObjMonScultureKingCore.CallStart);
        Assert.Equal(2589, ObjMonScultureKingCore.CallEnd);
        Assert.Equal(20, ObjMonScultureKingCore.CallLines);
        Assert.Equal(2591, ObjMonScultureKingCore.AttackStart);
        Assert.Equal(2604, ObjMonScultureKingCore.AttackEnd);
        Assert.Equal(14, ObjMonScultureKingCore.AttackLines);
        Assert.Equal(2606, ObjMonScultureKingCore.RunStart);
        Assert.Equal(2677, ObjMonScultureKingCore.RunEnd);
        Assert.Equal(72, ObjMonScultureKingCore.RunLines);
        Assert.Equal(136, ObjMonScultureKingCore.TotalLines);
        Assert.Equal(6, ObjMonScultureKingCore.MethodCount);
        Assert.Equal(1, ObjMonScultureKingCore.ClassCount);

        Assert.Equal(2551, ObjMonScultureKingCore.FreeLine);
        Assert.Equal(2552, ObjMonScultureKingCore.DestroyInheritedLine);
        Assert.Equal(2546, ObjMonScultureKingCore.ListCreateLine);
        Assert.Equal(29, ObjMonScultureKingCore.TotalDestroys);
        Assert.Equal(26, ObjMonScultureKingCore.InheritedFirstCount);
        Assert.Equal(3, ObjMonScultureKingCore.ExceptionCount);

        Assert.Equal(8, ObjMonScultureKingCore.ViewRange);
        Assert.Equal(7, ObjMonScultureKingCore.SiblingViewRange);
        Assert.Equal(5, ObjMonScultureKingCore.DangerLevelStart);
        Assert.Equal(5, ObjMonScultureKingCore.DirectionValue);
        Assert.Equal(2540, ObjMonScultureKingCore.SearchTimeLine);
        Assert.Equal(2542, ObjMonScultureKingCore.StoneModeLine);
        Assert.Equal(2545, ObjMonScultureKingCore.DangerSetLine);

        Assert.Equal(2598, ObjMonScultureKingCore.AliasLine);
        Assert.Equal(2599, ObjMonScultureKingCore.PowerLine);
        Assert.Equal(2602, ObjMonScultureKingCore.HitMagLine);
        Assert.Equal(2596, ObjMonScultureKingCore.NilGuardLine);
        Assert.Equal(2563, ObjMonScultureKingCore.EventCreateLine);
        Assert.Equal(6, ObjMonScultureKingCore.EventRadius);
        Assert.Equal(1, ObjMonScultureKingCore.OtherEventRadius);
        Assert.Equal(300000, ObjMonScultureKingCore.EventDurationMs);

        Assert.Equal(2577, ObjMonScultureKingCore.CountRollLine);
        Assert.Equal(6, ObjMonScultureKingCore.CountBound);
        Assert.Equal(6, ObjMonScultureKingCore.CountBase);
        Assert.Equal(30, ObjMonScultureKingCore.SlaveCap);
        Assert.Equal(2581, ObjMonScultureKingCore.CapLine);
        Assert.Equal(2578, ObjMonScultureKingCore.FrontPosLine);
        Assert.Equal(2583, ObjMonScultureKingCore.RegenLine);
        Assert.Equal(4, ObjMonScultureKingCore.ZumaTypes);
        Assert.Equal(2586, ObjMonScultureKingCore.AddLine);
        Assert.Equal(2588, ObjMonScultureKingCore.ForEndLine);

        Assert.Equal(2618, ObjMonScultureKingCore.LockLine);
        Assert.Equal(2643, ObjMonScultureKingCore.FinallyLine);
        Assert.Equal(2644, ObjMonScultureKingCore.UnlockLine);
        Assert.Equal(2, ObjMonScultureKingCore.RevealRadius);
        Assert.Equal(2636, ObjMonScultureKingCore.MeltCallLine);
        Assert.Equal(2655, ObjMonScultureKingCore.DangerCheckLine);
        Assert.Equal(2657, ObjMonScultureKingCore.DecLine);
        Assert.Equal(2658, ObjMonScultureKingCore.CallSlaveLine);
        Assert.Equal(2661, ObjMonScultureKingCore.ResetLine);
        Assert.Equal(2654, ObjMonScultureKingCore.CommentedTestLine);
        Assert.Equal(2664, ObjMonScultureKingCore.CleanupLoopLine);
        Assert.Equal(2672, ObjMonScultureKingCore.DeleteLine);
        Assert.Equal(2674, ObjMonScultureKingCore.CleanupEndLine);
        Assert.Equal(2676, ObjMonScultureKingCore.RunInheritedLine);

        Assert.Equal(431, ObjMonScultureKingCore.ClassDeclLine);
        Assert.Equal(432, ObjMonScultureKingCore.DangerFieldLine);
        Assert.Equal(433, ObjMonScultureKingCore.SlaveFieldLine);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonScultureKingCore.SpanMatches());
        Assert.True(ObjMonScultureKingCore.TotalLinesAddUp());
        Assert.True(ObjMonScultureKingCore.MethodsAscending());
        Assert.True(ObjMonScultureKingCore.MethodsContiguous());
        Assert.True(ObjMonScultureKingCore.WithinUnit());
        Assert.True(ObjMonScultureKingCore.NoInstrumentation());
    }

    // ===================== 一、少数派析构 =====================

    [Fact]
    public void MinorityDestructorFacts()
    {
        Assert.True(ObjMonScultureKingCore.MinorityDestructor());
        Assert.True(ObjMonScultureKingCore.FreesBeforeInherited());
        Assert.True(ObjMonScultureKingCore.OneOfTheThreeRecordedByJ220());
        Assert.True(ObjMonScultureKingCore.FirstOfTheThreePorted());
        Assert.True(ObjMonScultureKingCore.SharedReasonIsOwnedList());
        Assert.True(ObjMonScultureKingCore.NotAPureShell());
        Assert.True(ObjMonScultureKingCore.CreateBuildsTheList());
        Assert.True(ObjMonScultureKingCore.PairedFreeExists());
        Assert.True(ObjMonScultureKingCore.ContrastWithJ230J246());
        Assert.True(ObjMonScultureKingCore.MinorityLinesChecked());
        Assert.True(ObjMonScultureKingCore.CountsAddUp());
        Assert.True(ObjMonScultureKingCore.MajorityIsTheOtherWay());

        Assert.Equal(new[] { 2549, 6564, 6936 },
            ObjMonScultureKingCore.MinorityDestroyLines);
        Assert.Equal("free-then-inherited", ObjMonScultureKingCore.DestroyOrder());
    }

    [Fact]
    public void CreateFieldFacts()
    {
        Assert.True(ObjMonScultureKingCore.SevenFieldsSet());
        Assert.True(ObjMonScultureKingCore.ViewRangeEightHereSevenThere());
        Assert.True(ObjMonScultureKingCore.DangerLevelStartsFive());
        Assert.True(ObjMonScultureKingCore.SiblingDiffersByOneField());
        Assert.True(ObjMonScultureKingCore.DeclLinesChecked());
    }

    // ===================== 二、别名谱系第五现场 =====================

    [Fact]
    public void AliasLineageFacts()
    {
        Assert.True(ObjMonScultureKingCore.FifthTrueSite());
        Assert.True(ObjMonScultureKingCore.Site2598IsJ212sOwn());
        Assert.True(ObjMonScultureKingCore.FiveOfThirteenPorted());
        Assert.True(ObjMonScultureKingCore.ConfirmsTheRefinedCorrelation());
        Assert.True(ObjMonScultureKingCore.AllTrueSitesInTable());

        Assert.Equal(13, ObjMonScultureKingCore.J212AliasLines.Length);
        Assert.Contains(2598, ObjMonScultureKingCore.J212AliasLines);
        Assert.Equal(new[] { 7810, 8708, 1995, 2107, 2598 },
            ObjMonScultureKingCore.TrueSites);
    }

    [Fact]
    public void PowerBoundaries()
    {
        Assert.True(ObjMonScultureKingCore.DifferWhenInverted());
        Assert.True(ObjMonScultureKingCore.SameWhenNormal());

        Assert.Equal(5, ObjMonScultureKingCore.PowerNoMax(10, 5));
        Assert.Equal(11, ObjMonScultureKingCore.PowerWithMax(10, 5));
        Assert.Equal(ObjMonScultureKingCore.PowerNoMax(5, 10),
            ObjMonScultureKingCore.PowerWithMax(5, 10));
    }

    [Fact]
    public void ArgFacts()
    {
        Assert.True(ObjMonScultureKingCore.FirstArgIsZero());
        Assert.True(ObjMonScultureKingCore.ContrastWithJ243());
        Assert.True(ObjMonScultureKingCore.SameCalleeDifferentArgs());
        Assert.True(ObjMonScultureKingCore.ArgSetsDiffer());
        Assert.True(ObjMonScultureKingCore.NDirIgnored());
        Assert.True(ObjMonScultureKingCore.IgnoredInBothClasses());
        Assert.True(ObjMonScultureKingCore.HasNilGuard());
        Assert.True(ObjMonScultureKingCore.J243HadNoGuard());

        Assert.Equal(new[] { 2, 2 }, ObjMonScultureKingCore.J243Args);
        Assert.Equal(new[] { 0, 1 }, ObjMonScultureKingCore.ThisArgs);
    }

    // ===================== 三、MeltStone 与 CallSlave =====================

    [Fact]
    public void MeltStoneFacts()
    {
        Assert.True(ObjMonScultureKingCore.FifthCopyOfTheUnstoneIdiom());
        Assert.True(ObjMonScultureKingCore.VerbatimAgain());
        Assert.True(ObjMonScultureKingCore.EventRadiusSix());
        Assert.True(ObjMonScultureKingCore.ThreeValuesOfTheSameArg());
        Assert.True(ObjMonScultureKingCore.RadiusDiffersFromOthers());
        Assert.True(ObjMonScultureKingCore.CommentedLightAdjustment());
        Assert.True(ObjMonScultureKingCore.AppearanceValue218Recurs());
        Assert.True(ObjMonScultureKingCore.HereItIsCommented());
        Assert.True(ObjMonScultureKingCore.InJ246ItWasLive());
        Assert.True(ObjMonScultureKingCore.TwoBatchesSameAppearance());

        Assert.Equal(new[] { 2559, 2560, 2561, 2562 },
            ObjMonScultureKingCore.UnstoneLines);
        Assert.Equal(new[] { 8313, 8314, 8315, 8316 },
            ObjMonScultureKingCore.J233UnstoneLines);
        Assert.Equal(new[] { 1, 1, 6 }, ObjMonScultureKingCore.EventRadiusValues);
        Assert.Equal(new[] { 2565, 2566, 2567 },
            ObjMonScultureKingCore.CommentedLightLines);
    }

    [Fact]
    public void CallSlaveFacts()
    {
        Assert.True(ObjMonScultureKingCore.RandomSixPlusSix());
        Assert.True(ObjMonScultureKingCore.CapThirty());
        Assert.True(ObjMonScultureKingCore.FrontPosition());
        Assert.True(ObjMonScultureKingCore.FourZumaTypes());
        Assert.True(ObjMonScultureKingCore.OnlySuccessfulAdded());
        Assert.True(ObjMonScultureKingCore.EndTaggedAgain());
        Assert.True(ObjMonScultureKingCore.CountRange());
        Assert.True(ObjMonScultureKingCore.MaxEleven());
        Assert.True(ObjMonScultureKingCore.CapIsHigherThanOneBatch());
        Assert.True(ObjMonScultureKingCore.NeedsSeveralBatches());
        Assert.True(ObjMonScultureKingCore.NotNullAdded());
        Assert.True(ObjMonScultureKingCore.NullNotAdded());
        Assert.True(ObjMonScultureKingCore.ExactlyThirtyBreaks());
        Assert.True(ObjMonScultureKingCore.TwentyNineContinues());

        Assert.Equal(6, ObjMonScultureKingCore.SlaveCount(0));
        Assert.Equal(11, ObjMonScultureKingCore.SlaveCount(5));
        Assert.Equal(11, ObjMonScultureKingCore.MaxPerBatch());
    }

    [Fact]
    public void SlaveCapBoundaries()
    {
        Assert.True(ObjMonScultureKingCore.ShouldBreak(30));
        Assert.True(ObjMonScultureKingCore.ShouldBreak(31));
        Assert.False(ObjMonScultureKingCore.ShouldBreak(29));
        Assert.False(ObjMonScultureKingCore.ShouldBreak(0));
    }

    // ===================== 四、Run 的危险度机制 =====================

    [Fact]
    public void IntegerDivisionFacts()
    {
        Assert.True(ObjMonScultureKingCore.IntegerDivisionMakesHalfTheTestInert());
        Assert.True(ObjMonScultureKingCore.HpOverMaxHpIsZeroUnlessFull());
        Assert.True(ObjMonScultureKingCore.ExpressionOnlyZeroOrFive());
        Assert.True(ObjMonScultureKingCore.FullHpBranchIsDead());
        Assert.True(ObjMonScultureKingCore.FourthConsequenceOfShape6());
        Assert.True(ObjMonScultureKingCore.ZeroWhenNotFull());
        Assert.True(ObjMonScultureKingCore.FiveWhenFull());
        Assert.True(ObjMonScultureKingCore.NoIntermediateValues());
        Assert.True(ObjMonScultureKingCore.CorrectOrderWouldBeContinuous());
    }

    [Fact]
    public void DangerExpressionBoundaries()
    {
        // **表达式只取 0 或 5（穷举 0..100）**
        Assert.Equal(0, ObjMonScultureKingCore.DangerExpr(99, 100));
        Assert.Equal(5, ObjMonScultureKingCore.DangerExpr(100, 100));
        Assert.Equal(0, ObjMonScultureKingCore.DangerExpr(0, 100));

        // **写成 `HP * 5 / MaxHP` 才会连续**
        Assert.Equal(25, 500 * 5 / 100);
    }

    [Fact]
    public void SummonBoundaries()
    {
        Assert.True(ObjMonScultureKingCore.NotFullSummons());
        Assert.True(ObjMonScultureKingCore.FullNeverSummons());
        Assert.True(ObjMonScultureKingCore.ZeroDangerNoSummon());

        Assert.True(ObjMonScultureKingCore.ShouldSummon(5, 99, 100));
        Assert.False(ObjMonScultureKingCore.ShouldSummon(5, 100, 100));
        Assert.False(ObjMonScultureKingCore.ShouldSummon(0, 99, 100));
    }

    [Fact]
    public void ResetFacts()
    {
        Assert.True(ObjMonScultureKingCore.ResetsOnFullHp());
        Assert.True(ObjMonScultureKingCore.KeepsWhenNotFull());

        Assert.Equal(5, ObjMonScultureKingCore.DangerAfterFullHp(1, true));
        Assert.Equal(1, ObjMonScultureKingCore.DangerAfterFullHp(1, false));
    }

    [Fact]
    public void TestResidueFacts()
    {
        Assert.True(ObjMonScultureKingCore.CommentedTestCall());
        Assert.True(ObjMonScultureKingCore.MarkedAsForTesting());
        Assert.True(ObjMonScultureKingCore.BothActiveAndCommented());
    }

    [Fact]
    public void RevealAndThrottleFacts()
    {
        Assert.True(ObjMonScultureKingCore.FifthCopyOfTheRevealScan());
        Assert.True(ObjMonScultureKingCore.RadiusTwoHereAgain());
        Assert.True(ObjMonScultureKingCore.ActionIsSingleMelt());
        Assert.True(ObjMonScultureKingCore.ActionDiffersFromJ246());
        Assert.True(ObjMonScultureKingCore.UsesLockTryFinally());
        Assert.True(ObjMonScultureKingCore.DangerLogicInsideThrottle());
        Assert.True(ObjMonScultureKingCore.BoundToSearchCadence());
        Assert.True(ObjMonScultureKingCore.UnrelatedMechanismsBound());
        Assert.True(ObjMonScultureKingCore.SearchAfterEight());
        Assert.True(ObjMonScultureKingCore.SearchAfterOne());
    }

    [Fact]
    public void ThrottleBoundaries()
    {
        Assert.True(ObjMonScultureKingCore.ShouldSearch(8001, true));
        Assert.False(ObjMonScultureKingCore.ShouldSearch(8000, true));
        Assert.True(ObjMonScultureKingCore.ShouldSearch(1001, false));
        Assert.False(ObjMonScultureKingCore.ShouldSearch(1000, false));
    }

    [Fact]
    public void CleanupFacts()
    {
        Assert.True(ObjMonScultureKingCore.ReverseCleanupLoop());
        Assert.True(ObjMonScultureKingCore.DeletesDeadAndGhost());
        Assert.True(ObjMonScultureKingCore.DeadDeleted());
        Assert.True(ObjMonScultureKingCore.GhostDeleted());
        Assert.True(ObjMonScultureKingCore.AliveKept());
        Assert.True(ObjMonScultureKingCore.RedundantCountCheckInside());
        Assert.True(ObjMonScultureKingCore.EndTaggedForAgain());
        Assert.True(ObjMonScultureKingCore.SameTagStyleAsJ246());

        Assert.True(ObjMonScultureKingCore.ShouldDelete(true, false));
        Assert.True(ObjMonScultureKingCore.ShouldDelete(false, true));
        Assert.False(ObjMonScultureKingCore.ShouldDelete(false, false));
    }

    [Fact]
    public void RunTailFacts()
    {
        Assert.True(ObjMonScultureKingCore.GreaterEqualHere());
        Assert.True(ObjMonScultureKingCore.FourthOccurrence());
        Assert.True(ObjMonScultureKingCore.InheritedOutsideGuard());
    }

    // ===================== 五、其余 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonScultureKingCore.ConsistentWithJ244J245J246());
        Assert.True(ObjMonScultureKingCore.ScultureKingClosed());
        Assert.True(ObjMonScultureKingCore.OneRowToFlip());
        Assert.True(ObjMonScultureKingCore.ElectronicScolpionRemains());
        Assert.True(ObjMonScultureKingCore.FiveApprValuesRecorded());
        Assert.True(ObjMonScultureKingCore.SameBaseDifferentFeatureSet());
        Assert.True(ObjMonScultureKingCore.SlaveMechanismOnlyHere());

        Assert.Equal(new[] { 614, 619, 622, 628, 638 },
            ObjMonScultureKingCore.ElectronicApprValues);
    }
}

