using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J222：`ObjMon.pas` 中 `TMagicAttackNotMoveMonster2`（狐狸天珠）
/// **五个方法**的 1:1 测试（`Create`/`Destroy`/`Initialize`/`CallSlave`/`Run`，103 行）。
/// 最大的 `AttackTarget`（338 行）留待批次J223。
/// **本批最有价值的发现**：
/// ① `CallSlave` 第四个召出点读的是**另一个类的配置数组**（6976 `sFoxBeas[2]` 少了 `2`）；
/// ② 与上一条合起来：三个槽位"该读的没读、不该读的读了"；
/// ③ 范围注释 `3--7格` 在两个姊妹类里都是错的、而且**错法不同**。
/// </summary>
public sealed class ObjMonMagicNotMove2CoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(6927, ObjMonMagicNotMove2Core.CreateStart);
        Assert.Equal(6934, ObjMonMagicNotMove2Core.CreateEnd);
        Assert.Equal(8, ObjMonMagicNotMove2Core.CreateLines);
        Assert.Equal(6936, ObjMonMagicNotMove2Core.DestroyStart);
        Assert.Equal(6940, ObjMonMagicNotMove2Core.DestroyEnd);
        Assert.Equal(5, ObjMonMagicNotMove2Core.DestroyLines);
        Assert.Equal(6942, ObjMonMagicNotMove2Core.InitStart);
        Assert.Equal(6946, ObjMonMagicNotMove2Core.InitEnd);
        Assert.Equal(5, ObjMonMagicNotMove2Core.InitLines);
        Assert.Equal(6949, ObjMonMagicNotMove2Core.CallStart);
        Assert.Equal(6982, ObjMonMagicNotMove2Core.CallEnd);
        Assert.Equal(34, ObjMonMagicNotMove2Core.CallLines);
        Assert.Equal(7323, ObjMonMagicNotMove2Core.RunStart);
        Assert.Equal(7373, ObjMonMagicNotMove2Core.RunEnd);
        Assert.Equal(51, ObjMonMagicNotMove2Core.RunLines);
        Assert.Equal(103, ObjMonMagicNotMove2Core.TotalLines);
        Assert.Equal(441, ObjMonMagicNotMove2Core.ClassTotalLines);
        Assert.Equal(338, ObjMonMagicNotMove2Core.J223AttackLines);
        Assert.Equal(6984, ObjMonMagicNotMove2Core.AttackStart);
        Assert.Equal(7321, ObjMonMagicNotMove2Core.AttackEnd);

        Assert.Equal(6555, ObjMonMagicNotMove2Core.SibCreateStart);
        Assert.Equal(6562, ObjMonMagicNotMove2Core.SibCreateEnd);
        Assert.Equal(6564, ObjMonMagicNotMove2Core.SibDestroyStart);
        Assert.Equal(6568, ObjMonMagicNotMove2Core.SibDestroyEnd);
        Assert.Equal(6881, ObjMonMagicNotMove2Core.SibRunStart);
        Assert.Equal(6926, ObjMonMagicNotMove2Core.SibRunEnd);
        Assert.Equal(46, ObjMonMagicNotMove2Core.SibRunLines);
        Assert.Equal(7, ObjMonMagicNotMove2Core.CreateIdenticalLines);
        Assert.Equal(4, ObjMonMagicNotMove2Core.DestroyIdenticalLines);
        Assert.Equal(3, ObjMonMagicNotMove2Core.RunDiffs);
        Assert.Equal(5, ObjMonMagicNotMove2Core.RunLineDelta);
        Assert.Equal(6908, ObjMonMagicNotMove2Core.SibCleanupNilGuardLine);

        Assert.Equal(5, ObjMonMagicNotMove2Core.PrivateFieldCount);
        Assert.Equal(4, ObjMonMagicNotMove2Core.CreateInitialisedFields);
        Assert.Equal(253, ObjMonMagicNotMove2Core.LastStepDeclLine);
        Assert.Equal(254, ObjMonMagicNotMove2Core.FrozenTickDeclLine);
        Assert.Equal(255, ObjMonMagicNotMove2Core.SlaveListDeclLine);
        Assert.Equal(256, ObjMonMagicNotMove2Core.CalledSlaveDeclLine);
        Assert.Equal(257, ObjMonMagicNotMove2Core.OldHitTimeDeclLine);
        Assert.Equal(6929, ObjMonMagicNotMove2Core.CreateInheritedLine);
        Assert.Equal(6933, ObjMonMagicNotMove2Core.CreateLastFieldLine);
        Assert.Equal(6938, ObjMonMagicNotMove2Core.DestroyFreeLine);
        Assert.Equal(6939, ObjMonMagicNotMove2Core.DestroyInheritedLine);

        Assert.Equal(6944, ObjMonMagicNotMove2Core.InitInheritedLine);
        Assert.Equal(6945, ObjMonMagicNotMove2Core.SnapshotLine);
        Assert.Equal(768, ObjMonMagicNotMove2Core.BaseInitDeclLine);
        Assert.Equal(32881, ObjMonMagicNotMove2Core.BaseInitImplLine);
        Assert.Equal(261, ObjMonMagicNotMove2Core.InitDeclLine);

        Assert.Equal(6955, ObjMonMagicNotMove2Core.CallGuardLine);
        Assert.Equal(6957, ObjMonMagicNotMove2Core.RangeLine);
        Assert.Equal(4, ObjMonMagicNotMove2Core.RangeBase);
        Assert.Equal(3, ObjMonMagicNotMove2Core.RangeBound);
        Assert.Equal(4, ObjMonMagicNotMove2Core.RangeMin);
        Assert.Equal(6, ObjMonMagicNotMove2Core.RangeMax);
        Assert.Equal(3, ObjMonMagicNotMove2Core.CommentRangeMin);
        Assert.Equal(7, ObjMonMagicNotMove2Core.CommentRangeMax);
        Assert.Equal(3, ObjMonMagicNotMove2Core.SibRangeBase);
        Assert.Equal(4, ObjMonMagicNotMove2Core.SibRangeBound);
        Assert.Equal(6, ObjMonMagicNotMove2Core.SibRangeMax);
        Assert.Equal(6958, ObjMonMagicNotMove2Core.FrontPosLine);
        Assert.Equal(6964, ObjMonMagicNotMove2Core.BlockCommentStart);
        Assert.Equal(6970, ObjMonMagicNotMove2Core.BlockCommentEnd);
        Assert.Equal(4, ObjMonMagicNotMove2Core.FoxBeasCount);
        Assert.Equal(3, ObjMonMagicNotMove2Core.FoxBeas2Count);
        Assert.Equal(2806, ObjMonMagicNotMove2Core.FoxBeasDeclLine);
        Assert.Equal(2808, ObjMonMagicNotMove2Core.FoxBeas2DeclLine);
        Assert.Equal(5393, ObjMonMagicNotMove2Core.FoxBeasDefaultLine);
        Assert.Equal(5395, ObjMonMagicNotMove2Core.FoxBeas2DefaultLine);
        Assert.Equal(6976, ObjMonMagicNotMove2Core.WrongArrayLine);
        Assert.Equal(6981, ObjMonMagicNotMove2Core.CalledSlaveSetLine);

        Assert.Equal(7329, ObjMonMagicNotMove2Core.GuardLine);
        Assert.Equal(7359, ObjMonMagicNotMove2Core.GuardEndLine);
        Assert.Equal(7331, ObjMonMagicNotMove2Core.ThrottleLine);
        Assert.Equal(7335, ObjMonMagicNotMove2Core.SearchTargetLine);
        Assert.Equal(7338, ObjMonMagicNotMove2Core.AttackCallLine);
        Assert.Equal(7339, ObjMonMagicNotMove2Core.StepCommentLine);
        Assert.Equal(7340, ObjMonMagicNotMove2Core.HalfHpLine);
        Assert.Equal(7342, ObjMonMagicNotMove2Core.StepOneLine);
        Assert.Equal(7343, ObjMonMagicNotMove2Core.ScaleCheckLine);
        Assert.Equal(7344, ObjMonMagicNotMove2Core.ScaleAssignLine);
        Assert.Equal(7348, ObjMonMagicNotMove2Core.StepZeroLine);
        Assert.Equal(7349, ObjMonMagicNotMove2Core.RestoreCheckLine);
        Assert.Equal(7351, ObjMonMagicNotMove2Core.RestoreAssignLine);
        Assert.Equal(7354, ObjMonMagicNotMove2Core.StepCompareLine);
        Assert.Equal(7356, ObjMonMagicNotMove2Core.StepSendLine);
        Assert.Equal(7357, ObjMonMagicNotMove2Core.StepCacheLine);
        Assert.Equal(2, ObjMonMagicNotMove2Core.StepCount);
        Assert.Equal(5, ObjMonMagicNotMove2Core.SibStepCount);
        Assert.Equal(20234, ObjMonMagicNotMove2Core.RM_EFFECTSTEP);
        Assert.Equal(7360, ObjMonMagicNotMove2Core.CleanupCommentLine);
        Assert.Equal(7361, ObjMonMagicNotMove2Core.CleanupLoopLine);
        Assert.Equal(7363, ObjMonMagicNotMove2Core.DeadBreakLine);
        Assert.Equal(6912, ObjMonMagicNotMove2Core.SibDeadBreakLine);
        Assert.Equal(7369, ObjMonMagicNotMove2Core.DeleteLine);
        Assert.Equal(7372, ObjMonMagicNotMove2Core.FinalInheritedLine);

        Assert.Equal(251, ObjMonMagicNotMove2Core.ClassDeclLine);
        Assert.Equal(250, ObjMonMagicNotMove2Core.FactionCommentLine);
        Assert.Equal(263, ObjMonMagicNotMove2Core.AttackDeclLine);
        Assert.Equal(268, ObjMonMagicNotMove2Core.NextClassLine);
        Assert.Equal(7377, ObjMonMagicNotMove2Core.FireSpiritLine);
        Assert.Equal(28, ObjMonMagicNotMove2Core.ClassesCovered);
        Assert.Equal(26, ObjMonMagicNotMove2Core.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonMagicNotMove2Core.SpanMatches());
        Assert.True(ObjMonMagicNotMove2Core.TotalLinesAddUp());
        Assert.True(ObjMonMagicNotMove2Core.ClassTotalAddsUp());
        Assert.True(ObjMonMagicNotMove2Core.MethodsAscending());
        Assert.True(ObjMonMagicNotMove2Core.MethodsContiguous());
        Assert.True(ObjMonMagicNotMove2Core.RunSpansMatch());
        Assert.True(ObjMonMagicNotMove2Core.WithinUnit());
        Assert.True(ObjMonMagicNotMove2Core.NoInstrumentation());
    }

    // ===================== 一、CallSlave 的跨类配置 =====================

    [Fact]
    public void CrossClassConfigFacts()
    {
        Assert.True(ObjMonMagicNotMove2Core.ReadsFromTwoArrays());
        Assert.True(ObjMonMagicNotMove2Core.MissingTheTwo());
        Assert.True(ObjMonMagicNotMove2Core.CrossClassConfigAliasing());
        Assert.True(ObjMonMagicNotMove2Core.BothArraysHaveIndexTwo());
        Assert.True(ObjMonMagicNotMove2Core.NoBoundsErrorJustSilentWrongSource());
        Assert.True(ObjMonMagicNotMove2Core.SpawnSitesExtracted());
        Assert.True(ObjMonMagicNotMove2Core.OnlyFourthIsWrong());
        Assert.True(ObjMonMagicNotMove2Core.TigerAndBirdShareIndexOne());
        Assert.True(ObjMonMagicNotMove2Core.IndexTwoNeverRead());
        Assert.True(ObjMonMagicNotMove2Core.FoxBeas2ThirdSlotUseless());
        Assert.True(ObjMonMagicNotMove2Core.ShouldHaveReadNotRead());
        Assert.True(ObjMonMagicNotMove2Core.ShouldNotHaveReadDidRead());
        Assert.True(ObjMonMagicNotMove2Core.OnlyZeroAndOneUsed());
    }

    [Fact]
    public void SpawnSiteTable()
    {
        Assert.Equal(4, ObjMonMagicNotMove2Core.SpawnSites.Length);
        Assert.Equal("6959", ObjMonMagicNotMove2Core.SpawnSites[0].Line);
        Assert.Equal("青龙", ObjMonMagicNotMove2Core.SpawnSites[0].Slot);
        Assert.Equal("sFoxBeas2[0]", ObjMonMagicNotMove2Core.SpawnSites[0].Config);
        Assert.False(ObjMonMagicNotMove2Core.SpawnSites[0].Commented);

        Assert.Equal("白虎", ObjMonMagicNotMove2Core.SpawnSites[1].Slot);
        Assert.Equal("sFoxBeas2[1]", ObjMonMagicNotMove2Core.SpawnSites[1].Config);
        Assert.True(ObjMonMagicNotMove2Core.SpawnSites[1].Commented);

        Assert.Equal("朱雀", ObjMonMagicNotMove2Core.SpawnSites[2].Slot);
        Assert.Equal("sFoxBeas2[1]", ObjMonMagicNotMove2Core.SpawnSites[2].Config);
        Assert.False(ObjMonMagicNotMove2Core.SpawnSites[2].Commented);

        Assert.Equal("玄武", ObjMonMagicNotMove2Core.SpawnSites[3].Slot);
        Assert.Equal("sFoxBeas[2]", ObjMonMagicNotMove2Core.SpawnSites[3].Config);
        Assert.False(ObjMonMagicNotMove2Core.SpawnSites[3].Commented);

        Assert.Equal(new[] { 6959, 6965, 6971, 6976 },
            ObjMonMagicNotMove2Core.RegenLines);
        Assert.True(ObjMonMagicNotMove2Core.RegenLinesExtracted());
    }

    [Fact]
    public void SpawnCountFacts()
    {
        Assert.True(ObjMonMagicNotMove2Core.TigerSpawnCommentedOut());
        Assert.True(ObjMonMagicNotMove2Core.AtMostThree());
        Assert.True(ObjMonMagicNotMove2Core.CommentClaimsFour());
        Assert.True(ObjMonMagicNotMove2Core.SiblingSpawnsAllFour());

        Assert.Equal(3, ObjMonMagicNotMove2Core.CountActiveSpawns());
        Assert.Equal(4, ObjMonMagicNotMove2Core.BeastNames.Length);
        Assert.Equal(new[] { "青龙", "白虎", "朱雀", "玄武" },
            ObjMonMagicNotMove2Core.BeastNames);
    }

    [Fact]
    public void IndexUsageFacts()
    {
        Assert.Equal(new[] { 0, 1, 1 }, ObjMonMagicNotMove2Core.UsedIndexes);

        // **sFoxBeas2 有三个槽位、但 [2] 从未被读**
        Assert.Equal(3, ObjMonMagicNotMove2Core.FoxBeas2Count);
        Assert.True(ObjMonMagicNotMove2Core.IndexTwoNeverRead());

        // **两个数组的下标 2 都合法 => 不越界**
        Assert.True(ObjMonMagicNotMove2Core.FoxBeasCount > 2);
        Assert.True(ObjMonMagicNotMove2Core.FoxBeas2Count > 2);
    }

    // ===================== 二、范围注释与两个姊妹类 =====================

    [Fact]
    public void RangeCommentFacts()
    {
        Assert.True(ObjMonMagicNotMove2Core.SameWrongCommentDifferentFormula());
        Assert.True(ObjMonMagicNotMove2Core.BaseFourInsteadOfThree());
        Assert.True(ObjMonMagicNotMove2Core.BoundThreeInsteadOfFour());
        Assert.True(ObjMonMagicNotMove2Core.WrongAtBothEnds());
        Assert.True(ObjMonMagicNotMove2Core.SiblingWrongDifferently());
        Assert.True(ObjMonMagicNotMove2Core.CommentTracksNoFormula());
        Assert.True(ObjMonMagicNotMove2Core.SameUpperBound());
        Assert.True(ObjMonMagicNotMove2Core.DifferentLowerBound());
        Assert.True(ObjMonMagicNotMove2Core.BothUpperBoundsOffByOne());
        Assert.True(ObjMonMagicNotMove2Core.ThisLowerBoundAlsoOffByOne());
        Assert.True(ObjMonMagicNotMove2Core.SibLowerBoundMatches());
    }

    [Fact]
    public void RangeBoundariesBothSiblings()
    {
        // **本类：4..6**
        Assert.True(ObjMonMagicNotMove2Core.MinRange());
        Assert.True(ObjMonMagicNotMove2Core.MaxRange());
        Assert.Equal(4, ObjMonMagicNotMove2Core.Range(0));
        Assert.Equal(5, ObjMonMagicNotMove2Core.Range(1));
        Assert.Equal(6, ObjMonMagicNotMove2Core.Range(2));

        // **姊妹类：3..6**
        Assert.True(ObjMonMagicNotMove2Core.SibMinRange());
        Assert.True(ObjMonMagicNotMove2Core.SibMaxRange());
        Assert.Equal(3, ObjMonMagicNotMove2Core.SibRange(0));
        Assert.Equal(6, ObjMonMagicNotMove2Core.SibRange(3));

        // **注释声称 3..7、两个都错、且本类上下界都错**
        Assert.Equal(1, ObjMonMagicNotMove2Core.CommentRangeMax
            - ObjMonMagicNotMove2Core.RangeMax);
        Assert.Equal(1, ObjMonMagicNotMove2Core.RangeMin
            - ObjMonMagicNotMove2Core.CommentRangeMin);
    }

    [Fact]
    public void CallGuardBoundaries()
    {
        Assert.True(ObjMonMagicNotMove2Core.NonEmptyListExits());
        Assert.True(ObjMonMagicNotMove2Core.FlagExits());
        Assert.True(ObjMonMagicNotMove2Core.OnlyBothFalseProceeds());
        Assert.True(ObjMonMagicNotMove2Core.DoubleGuard());

        Assert.True(ObjMonMagicNotMove2Core.ShouldExitCall(true, false));
        Assert.True(ObjMonMagicNotMove2Core.ShouldExitCall(false, true));
        Assert.True(ObjMonMagicNotMove2Core.ShouldExitCall(true, true));
        Assert.False(ObjMonMagicNotMove2Core.ShouldExitCall(false, false));
    }

    // ===================== 三、Create / Destroy 与姊妹类 =====================

    [Fact]
    public void VerbatimSiblingFacts()
    {
        Assert.True(ObjMonMagicNotMove2Core.CreateVerbatimExceptName());
        Assert.True(ObjMonMagicNotMove2Core.SevenOfEightIdentical());
        Assert.True(ObjMonMagicNotMove2Core.SameFieldOrder());
        Assert.True(ObjMonMagicNotMove2Core.InheritedFirstInCreate());
        Assert.True(ObjMonMagicNotMove2Core.DestroyVerbatimExceptName());
        Assert.True(ObjMonMagicNotMove2Core.FourOfFiveIdentical());
        Assert.True(ObjMonMagicNotMove2Core.ThirdOfTheThreeMinority());
        Assert.True(ObjMonMagicNotMove2Core.FreesSlaveListBeforeInherited());
    }

    [Fact]
    public void CreateSkipsFifthFieldFacts()
    {
        Assert.True(ObjMonMagicNotMove2Core.FiveFields());
        Assert.True(ObjMonMagicNotMove2Core.FiveFieldsButFourInitialised());
        Assert.True(ObjMonMagicNotMove2Core.FifthOnlyInInitialize());
        Assert.True(ObjMonMagicNotMove2Core.RunReadsBeforeInitializePossible());
        Assert.True(ObjMonMagicNotMove2Core.GapLeftByCopyPaste());
        Assert.True(ObjMonMagicNotMove2Core.FieldCountMatches());
        Assert.True(ObjMonMagicNotMove2Core.FieldDeclsAscending());
        Assert.True(ObjMonMagicNotMove2Core.CreateSkipsFifth());
    }

    [Fact]
    public void CreateAndDestroyArithmetic()
    {
        // **8 - 7 = 1（只差函数名）**
        Assert.Equal(1, ObjMonMagicNotMove2Core.CreateLines
            - ObjMonMagicNotMove2Core.CreateIdenticalLines);

        // **5 - 4 = 1**
        Assert.Equal(1, ObjMonMagicNotMove2Core.DestroyLines
            - ObjMonMagicNotMove2Core.DestroyIdenticalLines);

        // **姊妹类的 Create/Destroy 同长度**
        Assert.Equal(ObjMonMagicNotMove2Core.CreateLines,
            ObjMonMagicNotMove2Core.SibCreateEnd
            - ObjMonMagicNotMove2Core.SibCreateStart + 1);
        Assert.Equal(ObjMonMagicNotMove2Core.DestroyLines,
            ObjMonMagicNotMove2Core.SibDestroyEnd
            - ObjMonMagicNotMove2Core.SibDestroyStart + 1);

        // **三处少数派析构**
        Assert.Equal(new[] { 2549, 6564, 6936 },
            ObjMonMagicNotMove2Core.SlaveListFreeFirstLines);
    }

    // ===================== 四、Initialize =====================

    [Fact]
    public void InitializeFacts()
    {
        Assert.True(ObjMonMagicNotMove2Core.InitializeOnlyInThisClass());
        Assert.True(ObjMonMagicNotMove2Core.SnapshotsHitTime());
        Assert.True(ObjMonMagicNotMove2Core.UsedForScaleAndRestore());
        Assert.True(ObjMonMagicNotMove2Core.OverrideIsCorrect());
        Assert.True(ObjMonMagicNotMove2Core.BaseImplChecked());
        Assert.True(ObjMonMagicNotMove2Core.InitInheritedFirst());
        Assert.True(ObjMonMagicNotMove2Core.OldHitTimeLinesChecked());
        Assert.True(ObjMonMagicNotMove2Core.OneWriteFourReads());
        Assert.True(ObjMonMagicNotMove2Core.OnlyUnderHalfHp());

        Assert.Equal(new[] { 6945, 7343, 7344, 7349, 7351 },
            ObjMonMagicNotMove2Core.OldHitTimeLines);
    }

    // ===================== 五、Run 与姊妹类的差异 =====================

    [Fact]
    public void RunDifferenceFacts()
    {
        Assert.True(ObjMonMagicNotMove2Core.StageBlockDiffers());
        Assert.True(ObjMonMagicNotMove2Core.TwoStagesVsFive());
        Assert.True(ObjMonMagicNotMove2Core.HalfHpThreshold());
        Assert.True(ObjMonMagicNotMove2Core.ExtraHitTimeScaling());
        Assert.True(ObjMonMagicNotMove2Core.CleanupMissingNilGuard());
        Assert.True(ObjMonMagicNotMove2Core.SiblingHasIt());
        Assert.True(ObjMonMagicNotMove2Core.ThreeSubstantiveDiffs());
        Assert.True(ObjMonMagicNotMove2Core.RunLineDeltaIsFive());
    }

    [Fact]
    public void StageValueBoundaries()
    {
        Assert.True(ObjMonMagicNotMove2Core.ExactlyHalfIsOne());
        Assert.True(ObjMonMagicNotMove2Core.AboveHalfIsZero());
        Assert.True(ObjMonMagicNotMove2Core.EmptyIsOne());
        Assert.True(ObjMonMagicNotMove2Core.FullIsZero());
        Assert.True(ObjMonMagicNotMove2Core.OnlyTwoValues());

        Assert.Equal(1, ObjMonMagicNotMove2Core.Step(50, 100));
        Assert.Equal(0, ObjMonMagicNotMove2Core.Step(51, 100));
        Assert.Equal(1, ObjMonMagicNotMove2Core.Step(0, 100));
        Assert.Equal(0, ObjMonMagicNotMove2Core.Step(100, 100));
    }

    [Fact]
    public void ScalingFacts()
    {
        Assert.True(ObjMonMagicNotMove2Core.FloatingScaleDown());
        Assert.True(ObjMonMagicNotMove2Core.IntegerRestoreBack());
        Assert.True(ObjMonMagicNotMove2Core.AsymmetricOnPurpose());
        Assert.True(ObjMonMagicNotMove2Core.HardcodedPointEight());
        Assert.True(ObjMonMagicNotMove2Core.ScaleHundredGivesEighty());
        Assert.True(ObjMonMagicNotMove2Core.RestoreGivesOriginal());
        Assert.True(ObjMonMagicNotMove2Core.AsymmetricBeginEnd());
        Assert.True(ObjMonMagicNotMove2Core.IfBranchBare());
        Assert.True(ObjMonMagicNotMove2Core.ElseBranchWrapped());

        Assert.Equal(80, ObjMonMagicNotMove2Core.ScaledHitTime(100));
        Assert.Equal(40, ObjMonMagicNotMove2Core.ScaledHitTime(50));
        Assert.Equal(0, ObjMonMagicNotMove2Core.ScaledHitTime(0));
    }

    [Fact]
    public void SharedShellFacts()
    {
        Assert.True(ObjMonMagicNotMove2Core.SharedShellVerbatim());
        Assert.True(ObjMonMagicNotMove2Core.SameGuard());
        Assert.True(ObjMonMagicNotMove2Core.SameThrottle());
        Assert.True(ObjMonMagicNotMove2Core.SameAttackCallNoParens());
        Assert.True(ObjMonMagicNotMove2Core.SameZeroCoordinateSend());
        Assert.True(ObjMonMagicNotMove2Core.SameStepCache());
        Assert.True(ObjMonMagicNotMove2Core.ShellSameKernelDiffers());
        Assert.True(ObjMonMagicNotMove2Core.AllTrueRuns());
        Assert.True(ObjMonMagicNotMove2Core.AnyBlocks());
        Assert.True(ObjMonMagicNotMove2Core.SearchAfterEightWithTarget());
        Assert.True(ObjMonMagicNotMove2Core.ExactlyEightBlocks());
    }

    [Fact]
    public void RunBoundaries()
    {
        Assert.True(ObjMonMagicNotMove2Core.CanRun(false, false, false, false, true));
        Assert.False(ObjMonMagicNotMove2Core.CanRun(true, false, false, false, true));
        Assert.False(ObjMonMagicNotMove2Core.CanRun(false, true, false, false, true));
        Assert.False(ObjMonMagicNotMove2Core.CanRun(false, false, true, false, true));
        Assert.False(ObjMonMagicNotMove2Core.CanRun(false, false, false, true, true));
        Assert.False(ObjMonMagicNotMove2Core.CanRun(false, false, false, false, false));

        Assert.True(ObjMonMagicNotMove2Core.ShouldSearch(8001, true));
        Assert.False(ObjMonMagicNotMove2Core.ShouldSearch(8000, true));
        Assert.True(ObjMonMagicNotMove2Core.ShouldSearch(1001, false));
        Assert.False(ObjMonMagicNotMove2Core.ShouldSearch(1000, false));
    }

    [Fact]
    public void CleanupFacts()
    {
        Assert.True(ObjMonMagicNotMove2Core.SameDeadBreak());
        Assert.True(ObjMonMagicNotMove2Core.CopiedDeadCode());
        Assert.True(ObjMonMagicNotMove2Core.DescendingDeleteCorrect());
        Assert.True(ObjMonMagicNotMove2Core.CleanupOutsideGuard());
        Assert.True(ObjMonMagicNotMove2Core.InheritedUnconditional());
        Assert.True(ObjMonMagicNotMove2Core.DesignChoiceConsistent());
        Assert.True(ObjMonMagicNotMove2Core.DeadRemoved());
        Assert.True(ObjMonMagicNotMove2Core.GhostRemoved());
        Assert.True(ObjMonMagicNotMove2Core.AliveKept());

        Assert.True(ObjMonMagicNotMove2Core.ShouldRemoveSlave(true, false));
        Assert.True(ObjMonMagicNotMove2Core.ShouldRemoveSlave(false, true));
        Assert.True(ObjMonMagicNotMove2Core.ShouldRemoveSlave(true, true));
        Assert.False(ObjMonMagicNotMove2Core.ShouldRemoveSlave(false, false));
    }

    // ===================== 六、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonMagicNotMove2Core.TwentyEightClassesCovered());
        Assert.True(ObjMonMagicNotMove2Core.RemainingApprox());
        Assert.True(ObjMonMagicNotMove2Core.PartialUntilJ223());
        Assert.True(ObjMonMagicNotMove2Core.MoreSiblingsFollow());
        Assert.True(ObjMonMagicNotMove2Core.FrozenFieldCopiedAround());
        Assert.True(ObjMonMagicNotMove2Core.NotRefactoredToBase());
        Assert.True(ObjMonMagicNotMove2Core.ClassDeclChecked());
        Assert.True(ObjMonMagicNotMove2Core.AttackDeclChecked());
    }
}
