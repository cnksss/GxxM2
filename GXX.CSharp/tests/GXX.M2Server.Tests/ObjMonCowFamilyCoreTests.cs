using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J243：`ObjMon.pas` 中牛族三兄弟（`TCowMonster`、`TMagCowMonster`、`TCowKingMonster`）
/// 十个方法的 1:1 测试（235 行）。
/// **本批最有价值的发现**：
/// ① 同一个 86 行的方法体有**两种装配方式** —— J242 把 `sub_4A9C78` 做成**模板方法**
///    （`virtual`+`override`+`inherited`），本批的 `sub_4A9F6C` 是**减配私有复制**
///    （`private`、非虚），且**少了两样**：命中判据与 `CanStone` 石化段；
/// ② **1995-1996 是 J212 别名谱系的第三个真现场**（前两处 J230 的 7810、J235 的 8708）——
///    而 1732/1870 虽也是别名行、后面却是内联掷骰 ⇒ **"13 处别名"与"省 `Max`"是两个不同集合**；
/// ③ 注释说"发狂10秒"、代码判的是 `< 8000`（**8 秒**）—— 两个数都是本地写的；
/// ④ 血量档位 `7 - HP div (MaxHP div 7)` 在 `MaxHP < 7` 时**除零**。
/// </summary>
public sealed class ObjMonCowFamilyCoreTests
{
    [Fact]
    public void Constants()
    {
        Assert.Equal(1838, ObjMonCowFamilyCore.CowCreateStart);
        Assert.Equal(1842, ObjMonCowFamilyCore.CowCreateEnd);
        Assert.Equal(5, ObjMonCowFamilyCore.CowCreateLines);
        Assert.Equal(1844, ObjMonCowFamilyCore.CowDestroyStart);
        Assert.Equal(1847, ObjMonCowFamilyCore.CowDestroyEnd);
        Assert.Equal(4, ObjMonCowFamilyCore.CowDestroyLines);
        Assert.Equal(1850, ObjMonCowFamilyCore.MagCreateStart);
        Assert.Equal(1854, ObjMonCowFamilyCore.MagCreateEnd);
        Assert.Equal(5, ObjMonCowFamilyCore.MagCreateLines);
        Assert.Equal(1856, ObjMonCowFamilyCore.MagDestroyStart);
        Assert.Equal(1859, ObjMonCowFamilyCore.MagDestroyEnd);
        Assert.Equal(4, ObjMonCowFamilyCore.MagDestroyLines);
        Assert.Equal(1861, ObjMonCowFamilyCore.SubStart);
        Assert.Equal(1945, ObjMonCowFamilyCore.SubEnd);
        Assert.Equal(85, ObjMonCowFamilyCore.SubLines);
        Assert.Equal(1947, ObjMonCowFamilyCore.MagAttackStart);
        Assert.Equal(1974, ObjMonCowFamilyCore.MagAttackEnd);
        Assert.Equal(28, ObjMonCowFamilyCore.MagAttackLines);
        Assert.Equal(1977, ObjMonCowFamilyCore.KingCreateStart);
        Assert.Equal(1988, ObjMonCowFamilyCore.KingCreateEnd);
        Assert.Equal(12, ObjMonCowFamilyCore.KingCreateLines);
        Assert.Equal(1990, ObjMonCowFamilyCore.KingAttackStart);
        Assert.Equal(2003, ObjMonCowFamilyCore.KingAttackEnd);
        Assert.Equal(14, ObjMonCowFamilyCore.KingAttackLines);
        Assert.Equal(2004, ObjMonCowFamilyCore.InitStart);
        Assert.Equal(2011, ObjMonCowFamilyCore.InitEnd);
        Assert.Equal(8, ObjMonCowFamilyCore.InitLines);
        Assert.Equal(2012, ObjMonCowFamilyCore.RunStart);
        Assert.Equal(2081, ObjMonCowFamilyCore.RunEnd);
        Assert.Equal(70, ObjMonCowFamilyCore.RunLines);
        Assert.Equal(235, ObjMonCowFamilyCore.TotalLines);
        Assert.Equal(10, ObjMonCowFamilyCore.MethodCount);
        Assert.Equal(3, ObjMonCowFamilyCore.ClassCount);

        Assert.Equal(86, ObjMonCowFamilyCore.J242SubLines);
        Assert.Equal(1, ObjMonCowFamilyCore.LineDelta);
        Assert.Equal(1747, ObjMonCowFamilyCore.J242AccuracyGateLine);
        Assert.Equal(1788, ObjMonCowFamilyCore.J242StoneGateLine);
        Assert.Equal(343, ObjMonCowFamilyCore.SubDeclLine);
        Assert.Equal(347, ObjMonCowFamilyCore.MagAttackDeclLine);
        Assert.Equal("FFEA", ObjMonCowFamilyCore.J242Slot);
        Assert.Equal(12, ObjMonCowFamilyCore.TemplateConfirmations);
        Assert.Equal(1962, ObjMonCowFamilyCore.SubCallLine);
        Assert.Equal(1963, ObjMonCowFamilyCore.BreakSeizeLine);
        Assert.Equal(1965, ObjMonCowFamilyCore.ResultTrueLine);
        Assert.Equal(1969, ObjMonCowFamilyCore.SameMapLine);

        Assert.Equal(1870, ObjMonCowFamilyCore.AliasLine);
        Assert.Equal(1876, ObjMonCowFamilyCore.OriginalCommentLine);
        Assert.Equal(1995, ObjMonCowFamilyCore.KingAliasLine);
        Assert.Equal(1996, ObjMonCowFamilyCore.KingPowerLine);
        Assert.Equal(2000, ObjMonCowFamilyCore.HitMagLine);
        Assert.Equal(2001, ObjMonCowFamilyCore.CommentedInheritedLine);
        Assert.Equal(2, ObjMonCowFamilyCore.HalvingDivisor);
        Assert.Equal(2006, ObjMonCowFamilyCore.SnapshotLines[0]);
        Assert.Equal(2007, ObjMonCowFamilyCore.SnapshotLines[1]);
        Assert.Equal(2009, ObjMonCowFamilyCore.InitInheritedLine);
        Assert.Equal(37, ObjMonCowFamilyCore.ConstructorsWithInheritedFirst);
        Assert.Equal(38, ObjMonCowFamilyCore.TotalConstructors);

        Assert.Equal(2016, ObjMonCowFamilyCore.RunGuardLine);
        Assert.Equal(2018, ObjMonCowFamilyCore.JumpTimerLine);
        Assert.Equal(30000, ObjMonCowFamilyCore.JumpIntervalMs);
        Assert.Equal(5, ObjMonCowFamilyCore.SiegeThreshold);
        Assert.Equal(2023, ObjMonCowFamilyCore.GetBackPosLine);
        Assert.Equal(2025, ObjMonCowFamilyCore.CanWalkLine);
        Assert.Equal(2026, ObjMonCowFamilyCore.SpaceMoveLine);
        Assert.Equal(2028, ObjMonCowFamilyCore.MapRandomMoveLine);
        Assert.Equal(2030, ObjMonCowFamilyCore.JumpExitLine);
        Assert.Equal(2034, ObjMonCowFamilyCore.RunTimerLine);
        Assert.Equal(2000, ObjMonCowFamilyCore.RunIntervalMs);
        Assert.Equal(2037, ObjMonCowFamilyCore.HpBucketLine);
        Assert.Equal(7, ObjMonCowFamilyCore.BucketDivisor);
        Assert.Equal(7, ObjMonCowFamilyCore.BucketBase);
        Assert.Equal(2040, ObjMonCowFamilyCore.BucketStoreLine);
        Assert.Equal(2, ObjMonCowFamilyCore.WarnThreshold);
        Assert.Equal(2043, ObjMonCowFamilyCore.WarnEnterLine);
        Assert.Equal(2044, ObjMonCowFamilyCore.WarnTickLine);
        Assert.Equal(5000, ObjMonCowFamilyCore.WarnDurationMs);
        Assert.Equal(2051, ObjMonCowFamilyCore.RestoreCommentLine);
        Assert.Equal(2052, ObjMonCowFamilyCore.RestoreLine);
        Assert.Equal(2057, ObjMonCowFamilyCore.RageEnterLine);
        Assert.Equal(2058, ObjMonCowFamilyCore.RageTickLine);
        Assert.Equal(2064, ObjMonCowFamilyCore.RageCommentLine);
        Assert.Equal(8000, ObjMonCowFamilyCore.RageCodeMs);
        Assert.Equal(10000, ObjMonCowFamilyCore.RageCommentMs);
        Assert.Equal(500, ObjMonCowFamilyCore.RageHitTime);
        Assert.Equal(400, ObjMonCowFamilyCore.RageWalkSpeed);
        Assert.Equal(2080, ObjMonCowFamilyCore.RunInheritedLine);
        Assert.Equal(2705, ObjMonCowFamilyCore.J242Bo554Line);

        Assert.Equal(1981, ObjMonCowFamilyCore.SearchTimeLine);
        Assert.Equal(500, ObjMonCowFamilyCore.KingSearchBase);
        Assert.Equal(1500, ObjMonCowFamilyCore.FamilySearchBase);
        Assert.Equal(1500, ObjMonCowFamilyCore.SearchBound);
        Assert.Equal(1984, ObjMonCowFamilyCore.MagStruckKeepLine);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonCowFamilyCore.SpanMatches());
        Assert.True(ObjMonCowFamilyCore.TotalLinesAddUp());
        Assert.True(ObjMonCowFamilyCore.MethodsAscending());
        Assert.True(ObjMonCowFamilyCore.WithinUnit());
        Assert.True(ObjMonCowFamilyCore.NoInstrumentation());
    }

    // ===================== 一、减配版 =====================

    [Fact]
    public void TrimmedCopyFacts()
    {
        Assert.True(ObjMonCowFamilyCore.TrimmedCopyOfJ242());
        Assert.True(ObjMonCowFamilyCore.MissingAccuracyGate());
        Assert.True(ObjMonCowFamilyCore.MissingStoneEffect());
        Assert.True(ObjMonCowFamilyCore.NotVirtualHere());
        Assert.True(ObjMonCowFamilyCore.BothMissing());
        Assert.True(ObjMonCowFamilyCore.SameScopeDifferentSubset());
        Assert.True(ObjMonCowFamilyCore.LineDeltaIsOne());
        Assert.True(ObjMonCowFamilyCore.DiceRollSameButShifted());
    }

    [Fact]
    public void EmptyCommentFacts()
    {
        Assert.True(ObjMonCowFamilyCore.EmptyTrailingComments());
        Assert.True(ObjMonCowFamilyCore.ThreeOfThem());
        Assert.True(ObjMonCowFamilyCore.SeventhCommentUsage());
        Assert.True(ObjMonCowFamilyCore.PurelyCosmetic());
        Assert.True(ObjMonCowFamilyCore.SemanticallyInert());
        Assert.True(ObjMonCowFamilyCore.ThreeConsecutive());
        Assert.True(ObjMonCowFamilyCore.IsSeventh());

        Assert.Equal(new[] { 1931, 1932, 1933 }, ObjMonCowFamilyCore.EmptyCommentLines);
        Assert.Equal(6, ObjMonCowFamilyCore.PriorCommentUsages);
    }

    [Fact]
    public void AssemblyStyleFacts()
    {
        Assert.True(ObjMonCowFamilyCore.PrivateNotVirtual());
        Assert.True(ObjMonCowFamilyCore.NoInheritedChain());
        Assert.True(ObjMonCowFamilyCore.TwoAssembliesOfTheSameBody());
        Assert.True(ObjMonCowFamilyCore.TwelfthTemplateConfirmation());
        Assert.True(ObjMonCowFamilyCore.BothGatesCutAgain());
    }

    // ===================== 二、别名谱系 =====================

    [Fact]
    public void AliasLineageFacts()
    {
        Assert.True(ObjMonCowFamilyCore.ThirdTrueSite());
        Assert.True(ObjMonCowFamilyCore.Site1995IsJ212sOwn());
        Assert.True(ObjMonCowFamilyCore.ThreeOfThirteenPorted());
        Assert.True(ObjMonCowFamilyCore.AliasLine1870InTable());
        Assert.True(ObjMonCowFamilyCore.AliasAloneIsNotEnough());
        Assert.True(ObjMonCowFamilyCore.RefinesTheCorrelation());

        Assert.Equal(13, ObjMonCowFamilyCore.J212AliasLines.Length);
        Assert.Equal(new[] { 7810, 8708, 1995 }, ObjMonCowFamilyCore.TrueSites);
        Assert.Contains(1995, ObjMonCowFamilyCore.J212AliasLines);
        Assert.Contains(1870, ObjMonCowFamilyCore.J212AliasLines);
    }

    [Fact]
    public void PowerBoundaries()
    {
        Assert.True(ObjMonCowFamilyCore.DifferWhenInverted());
        Assert.True(ObjMonCowFamilyCore.SameWhenNormal());

        Assert.Equal(5, ObjMonCowFamilyCore.PowerNoMax(10, 5));
        Assert.Equal(11, ObjMonCowFamilyCore.PowerWithMax(10, 5));
        Assert.Equal(ObjMonCowFamilyCore.PowerNoMax(5, 10),
            ObjMonCowFamilyCore.PowerWithMax(5, 10));
    }

    [Fact]
    public void AttackAssemblyFacts()
    {
        Assert.True(ObjMonCowFamilyCore.DoesNotCallInherited());
        Assert.True(ObjMonCowFamilyCore.InheritedCommentedOut());
        Assert.True(ObjMonCowFamilyCore.UsesHitMagAttackTarget());
        Assert.True(ObjMonCowFamilyCore.PowerHalvedByIntegerDivision());
        Assert.True(ObjMonCowFamilyCore.SameValuePassedTwice());
        Assert.True(ObjMonCowFamilyCore.FourthAssemblyStyle());
    }

    [Fact]
    public void HalvingBoundaries()
    {
        Assert.True(ObjMonCowFamilyCore.OddIsTruncated());
        Assert.True(ObjMonCowFamilyCore.EvenIsExact());
        Assert.True(ObjMonCowFamilyCore.BothArgsEqual());

        Assert.Equal(3, ObjMonCowFamilyCore.Halve(7));
        Assert.Equal(4, ObjMonCowFamilyCore.Halve(8));
        Assert.Equal(0, ObjMonCowFamilyCore.Halve(1));
    }

    [Fact]
    public void InitializeFacts()
    {
        Assert.True(ObjMonCowFamilyCore.InheritedLastHere());
        Assert.True(ObjMonCowFamilyCore.SnapshotsBeforeInherited());
        Assert.True(ObjMonCowFamilyCore.ExplainsTheJ220Exception());
        Assert.True(ObjMonCowFamilyCore.OrderIsSemantic());
        Assert.True(ObjMonCowFamilyCore.TwoSnapshotFields());
        Assert.True(ObjMonCowFamilyCore.SnapshotIsAttackAndWalk());
        Assert.True(ObjMonCowFamilyCore.RarityRecorded());

        Assert.Equal(new[] { "dw56C", "dw570" }, ObjMonCowFamilyCore.SnapshotFields);
    }

    // ===================== 三、Run 的状态机 =====================

    [Fact]
    public void TimerFacts()
    {
        Assert.True(ObjMonCowFamilyCore.TwoIndependentTimers());
        Assert.True(ObjMonCowFamilyCore.JumpEveryThirtySeconds());
        Assert.True(ObjMonCowFamilyCore.SpaceMoveOrMapRandomMove());
        Assert.True(ObjMonCowFamilyCore.ExitsAfterJump());
        Assert.True(ObjMonCowFamilyCore.RunTimerEveryTwoSeconds());
        Assert.True(ObjMonCowFamilyCore.SiegeInspectionThresholdFive());
        Assert.True(ObjMonCowFamilyCore.FirstSiegeInspection());
    }

    [Fact]
    public void JumpBoundaries()
    {
        Assert.True(ObjMonCowFamilyCore.AllThreeJumps());
        Assert.True(ObjMonCowFamilyCore.FewSiegersNoJump());
        Assert.True(ObjMonCowFamilyCore.NoTargetNoJump());
        Assert.True(ObjMonCowFamilyCore.TooEarlyNoJump());
        Assert.True(ObjMonCowFamilyCore.ExactlyThirtyJumps());

        Assert.True(ObjMonCowFamilyCore.ShouldJump(true, 5, 30000));
        Assert.False(ObjMonCowFamilyCore.ShouldJump(true, 4, 30000));
        Assert.False(ObjMonCowFamilyCore.ShouldJump(false, 9, 30000));
        Assert.False(ObjMonCowFamilyCore.ShouldJump(true, 9, 29999));
    }

    [Fact]
    public void HpBucketFacts()
    {
        Assert.True(ObjMonCowFamilyCore.FullHpIsZero());
        Assert.True(ObjMonCowFamilyCore.EmptyHpIsSeven());
        Assert.True(ObjMonCowFamilyCore.BucketRisesAsHpFalls());
        Assert.True(ObjMonCowFamilyCore.BucketRange());
        Assert.True(ObjMonCowFamilyCore.DoubleIntegerDivision());
        Assert.True(ObjMonCowFamilyCore.DivisionByZeroWhenMaxHpBelowSeven());
        Assert.True(ObjMonCowFamilyCore.SecondShapeSixHazard());
        Assert.True(ObjMonCowFamilyCore.MaxHpSevenIsSafe());
    }

    [Fact]
    public void HpBucketBoundaries()
    {
        Assert.Equal(0, ObjMonCowFamilyCore.HpBucket(700, 700));
        Assert.Equal(7, ObjMonCowFamilyCore.HpBucket(0, 700));
        Assert.Equal(6, ObjMonCowFamilyCore.HpBucket(100, 700));
        Assert.Equal(1, ObjMonCowFamilyCore.HpBucket(600, 700));
    }

    [Fact]
    public void CommentMismatchFacts()
    {
        Assert.True(ObjMonCowFamilyCore.CommentSaysTenSeconds());
        Assert.True(ObjMonCowFamilyCore.CodeChecksEightSeconds());
        Assert.True(ObjMonCowFamilyCore.CommentDisagreesWithCode());
        Assert.True(ObjMonCowFamilyCore.NumbersBothLocal());
        Assert.True(ObjMonCowFamilyCore.CommentWouldKeepRaging());
    }

    [Fact]
    public void RageBoundaries()
    {
        Assert.True(ObjMonCowFamilyCore.RageWithinEight());
        Assert.True(ObjMonCowFamilyCore.ExactlyEightExits());

        Assert.True(ObjMonCowFamilyCore.StillRaging(7999));
        Assert.False(ObjMonCowFamilyCore.StillRaging(8000));
        Assert.False(ObjMonCowFamilyCore.StillRaging(10000));
    }

    [Fact]
    public void StateMachineFacts()
    {
        Assert.True(ObjMonCowFamilyCore.ThreeStageMachine());
        Assert.True(ObjMonCowFamilyCore.WarnFiveSeconds());
        Assert.True(ObjMonCowFamilyCore.WarnRestoresNormal());
        Assert.True(ObjMonCowFamilyCore.OppositeDirectionsOfWarnAndRage());
        Assert.True(ObjMonCowFamilyCore.EdgeTriggeredBucket());
        Assert.True(ObjMonCowFamilyCore.LevelDrivenState());
        Assert.True(ObjMonCowFamilyCore.TwoTriggerStyles());
    }

    [Fact]
    public void HardcodedRageFacts()
    {
        Assert.True(ObjMonCowFamilyCore.HardcodedFiveHundred());
        Assert.True(ObjMonCowFamilyCore.HardcodedFourHundred());
        Assert.True(ObjMonCowFamilyCore.RestoresFromSnapshot());
        Assert.True(ObjMonCowFamilyCore.SnapshotIsThePoint());
        Assert.True(ObjMonCowFamilyCore.RageAttacksFaster());
        Assert.True(ObjMonCowFamilyCore.RageWalksFaster());

        Assert.Equal(new[] { 2067, 2068 }, ObjMonCowFamilyCore.RageSetLines);
        Assert.Equal(new[] { 2073, 2074 }, ObjMonCowFamilyCore.RageRestoreLines);
    }

    [Fact]
    public void Bo554Facts()
    {
        Assert.True(ObjMonCowFamilyCore.Bo554Again());
        Assert.True(ObjMonCowFamilyCore.ConfirmsItIsABaseField());
        Assert.True(ObjMonCowFamilyCore.SecondConfirmation());
        Assert.True(ObjMonCowFamilyCore.TwoDifferentClasses());
    }

    [Fact]
    public void InheritedPlacementFacts()
    {
        Assert.True(ObjMonCowFamilyCore.InheritedOutsideGuard());
        Assert.True(ObjMonCowFamilyCore.JumpExitSkipsInherited());
        Assert.True(ObjMonCowFamilyCore.SameAsJ220());
    }

    // ===================== 四、Create 与字段 =====================

    [Fact]
    public void CreateFacts()
    {
        Assert.True(ObjMonCowFamilyCore.SevenInCreateTwoInInitialize());
        Assert.True(ObjMonCowFamilyCore.SearchTimeBaseFiveHundred());
        Assert.True(ObjMonCowFamilyCore.TwoUninitializedTimestamps());
        Assert.True(ObjMonCowFamilyCore.AssignedOnlyInRun());
        Assert.True(ObjMonCowFamilyCore.SearchTimeBaseDiffers());
        Assert.True(ObjMonCowFamilyCore.ThreeUseFifteenHundred());
        Assert.True(ObjMonCowFamilyCore.OneUsesFiveHundred());
        Assert.True(ObjMonCowFamilyCore.NotATypo());

        Assert.Equal(7, ObjMonCowFamilyCore.CreateSetLines.Length);
        Assert.Equal(1981, ObjMonCowFamilyCore.CreateSetLines[0]);
        Assert.Equal(1987, ObjMonCowFamilyCore.CreateSetLines[6]);
    }

    [Fact]
    public void SearchTimeRanges()
    {
        Assert.True(ObjMonCowFamilyCore.KingRange());
        Assert.True(ObjMonCowFamilyCore.FamilyRange());

        Assert.Equal(500, ObjMonCowFamilyCore.SearchTime(0, 500));
        Assert.Equal(1999, ObjMonCowFamilyCore.SearchTime(1499, 500));
        Assert.Equal(1500, ObjMonCowFamilyCore.SearchTime(0, 1500));
        Assert.Equal(2999, ObjMonCowFamilyCore.SearchTime(1499, 1500));
    }

    [Fact]
    public void ClassDifferenceFacts()
    {
        Assert.True(ObjMonCowFamilyCore.TwoIdenticalCreates());
        Assert.True(ObjMonCowFamilyCore.OnlyAttackDiffers());
        Assert.True(ObjMonCowFamilyCore.CowIsNotMagic());
        Assert.True(ObjMonCowFamilyCore.CowHasNoAttackTarget());
        Assert.True(ObjMonCowFamilyCore.KingOverridesAttack());
    }

    [Fact]
    public void DestroyFacts()
    {
        Assert.True(ObjMonCowFamilyCore.ThreePureShellDestroys());
        Assert.True(ObjMonCowFamilyCore.TwentySevenTotal());
    }

    // ===================== 五、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonCowFamilyCore.CowFamilyClosed());
        Assert.True(ObjMonCowFamilyCore.ThreeRowsToFlip());
        Assert.True(ObjMonCowFamilyCore.DeclLinesChecked());
        Assert.True(ObjMonCowFamilyCore.AllSameBase());

        Assert.Equal(new[] { 335, 341, 350 }, ObjMonCowFamilyCore.DeclLines);
    }
}
