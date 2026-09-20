using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J213：`ObjMon.pas` 中 `TIcePeakMonster`（雪域卫士）
/// 四个方法 1:1 测试（合计 103 行）。
/// **本批最有价值的发现**：`MeltStoneAll`（5305-5329）整段复制自
/// `TScultureMonster.MeltStoneAll`（2415-2439），
/// 复制时把 `is TScultureMonster` 改成了 `is TIcePeakMonster`、
/// **却没把强转的 `TScultureMonster(...)` 一起改掉** ——
/// 它之所以不报错纯属侥幸（姊妹类没有实例字段 + 硬转换不做运行时检查），
/// 而后果是"被同伴唤醒的雪域卫士"拿不到静默期与冰峰事件。
/// </summary>
public sealed class ObjMonIcePeakCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(5279, ObjMonIcePeakCore.CreateStart);
        Assert.Equal(5287, ObjMonIcePeakCore.CreateEnd);
        Assert.Equal(9, ObjMonIcePeakCore.CreateLines);
        Assert.Equal(5289, ObjMonIcePeakCore.MeltStart);
        Assert.Equal(5303, ObjMonIcePeakCore.MeltEnd);
        Assert.Equal(15, ObjMonIcePeakCore.MeltLines);
        Assert.Equal(5305, ObjMonIcePeakCore.MeltAllStart);
        Assert.Equal(5329, ObjMonIcePeakCore.MeltAllEnd);
        Assert.Equal(25, ObjMonIcePeakCore.MeltAllLines);
        Assert.Equal(5331, ObjMonIcePeakCore.RunStart);
        Assert.Equal(5384, ObjMonIcePeakCore.RunEnd);
        Assert.Equal(54, ObjMonIcePeakCore.RunLines);
        Assert.Equal(103, ObjMonIcePeakCore.TotalLines);

        Assert.Equal(5321, ObjMonIcePeakCore.IsCheckLine);
        Assert.Equal(5323, ObjMonIcePeakCore.CastCallLine);
        Assert.Equal("TIcePeakMonster", ObjMonIcePeakCore.IsClassName);
        Assert.Equal("TScultureMonster", ObjMonIcePeakCore.CastClassName);
        Assert.Equal(2431, ObjMonIcePeakCore.SiblingIsLine);
        Assert.Equal(2433, ObjMonIcePeakCore.SiblingCastLine);
        Assert.Equal(2407, ObjMonIcePeakCore.SiblingMeltStart);
        Assert.Equal(2413, ObjMonIcePeakCore.SiblingMeltEnd);
        Assert.Equal(2415, ObjMonIcePeakCore.SiblingMeltAllStart);
        Assert.Equal(2439, ObjMonIcePeakCore.SiblingMeltAllEnd);
        Assert.Equal(420, ObjMonIcePeakCore.SiblingDeclStart);
        Assert.Equal(429, ObjMonIcePeakCore.SiblingDeclEnd);
        Assert.Equal(2555, ObjMonIcePeakCore.KingMeltLine);
        Assert.Equal(0, ObjMonIcePeakCore.SiblingNewFields);
        Assert.Equal(1, ObjMonIcePeakCore.SelfNewFields);

        Assert.Equal(5293, ObjMonIcePeakCore.StoneGuardLine);
        Assert.Equal(5295, ObjMonIcePeakCore.TickExtendLine);
        Assert.Equal(2000, ObjMonIcePeakCore.QuietPeriodMs);
        Assert.Equal(5300, ObjMonIcePeakCore.EventCreateLine);
        Assert.Equal(5301, ObjMonIcePeakCore.EventAddLine);
        Assert.Equal(4, ObjMonIcePeakCore.SiblingActions);
        Assert.Equal(3, ObjMonIcePeakCore.ExtraActionsHere);

        Assert.Equal(5312, ObjMonIcePeakCore.ListCreateLine);
        Assert.Equal(5313, ObjMonIcePeakCore.GetMapLine);
        Assert.Equal(7, ObjMonIcePeakCore.WakeAllRadius);
        Assert.Equal(5328, ObjMonIcePeakCore.FreeLine);

        Assert.Equal(5337, ObjMonIcePeakCore.GuardLine);
        Assert.Equal(5343, ObjMonIcePeakCore.LockLine);
        Assert.Equal(5344, ObjMonIcePeakCore.TryLine);
        Assert.Equal(5368, ObjMonIcePeakCore.FinallyLine);
        Assert.Equal(5369, ObjMonIcePeakCore.UnLockLine);
        Assert.Equal(5359, ObjMonIcePeakCore.RangeCheckLine);
        Assert.Equal(5374, ObjMonIcePeakCore.SearchThrottleLine);
        Assert.Equal(5378, ObjMonIcePeakCore.SearchTargetLine);
        Assert.Equal(5382, ObjMonIcePeakCore.InheritedLine);
        Assert.Equal(5383, ObjMonIcePeakCore.InheritedCallLine);
        Assert.Equal(2, ObjMonIcePeakCore.TriggerRadius);
        Assert.Equal(3, ObjMonIcePeakCore.TimeIdiomCount);
        Assert.Equal(7, ObjMonIcePeakCore.NoParenCalls);
        Assert.Equal(1, ObjMonIcePeakCore.ParenCalls);

        Assert.Equal(20099, ObjMonIcePeakCore.RM_DIGUP);
        Assert.Equal(10, ObjMonIcePeakCore.ET_ICEPEAK);
        Assert.Equal(1, ObjMonIcePeakCore.STATE_STONE_MODE);
        Assert.Equal("0x345", ObjMonIcePeakCore.StoneModeOffset);
        Assert.Equal("0x408", ObjMonIcePeakCore.VisibleActorsOffset);
        Assert.Equal("2019-11-07", ObjMonIcePeakCore.OptimizeTodoDate);
        Assert.Equal(5283, ObjMonIcePeakCore.ViewRangeLine);
        Assert.Equal(7, ObjMonIcePeakCore.ViewRange);
        Assert.Equal(3589, ObjMonIcePeakCore.MyGetTickCountDeclLine);
        Assert.Equal(60000, ObjMonIcePeakCore.OneMinuteMs);
        Assert.Equal(18, ObjMonIcePeakCore.ClassesCovered);
        Assert.Equal(36, ObjMonIcePeakCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonIcePeakCore.SpanMatches());
        Assert.True(ObjMonIcePeakCore.TotalLinesAddUp());
        Assert.True(ObjMonIcePeakCore.MethodsAscending());
        Assert.True(ObjMonIcePeakCore.MethodsContiguous());
        Assert.True(ObjMonIcePeakCore.SiblingSpansMatch());
        Assert.True(ObjMonIcePeakCore.SiblingDeclSpanMatches());
        Assert.True(ObjMonIcePeakCore.WithinUnit());
        Assert.True(ObjMonIcePeakCore.NoInstrumentation());
    }

    // ===================== 一、类型错配 =====================

    [Fact]
    public void CheckAndCastMismatchFacts()
    {
        Assert.True(ObjMonIcePeakCore.CheckAndCastMismatch());
        Assert.True(ObjMonIcePeakCore.IsSaysIcePeak());
        Assert.True(ObjMonIcePeakCore.CastSaysSculture());
        Assert.True(ObjMonIcePeakCore.CopiedFromSibling());
        Assert.True(ObjMonIcePeakCore.PartialRename());
        Assert.True(ObjMonIcePeakCore.CheckAndCastExtracted());
        Assert.True(ObjMonIcePeakCore.SourceWasConsistent());
        Assert.True(ObjMonIcePeakCore.CheckLinesAdjacent());
        Assert.True(ObjMonIcePeakCore.SiblingLinesAdjacent());
        Assert.True(ObjMonIcePeakCore.SameBodyLength());
    }

    [Fact]
    public void CheckAndCastTable()
    {
        Assert.Equal(2, ObjMonIcePeakCore.CheckAndCast.Length);

        // **来源处一致**
        Assert.Equal("TScultureMonster", ObjMonIcePeakCore.CheckAndCast[0].IsClass);
        Assert.Equal("TScultureMonster", ObjMonIcePeakCore.CheckAndCast[0].CastClass);

        // **本处不一致：检查雪域卫士、强转祖玛雕像**
        Assert.Equal("TIcePeakMonster", ObjMonIcePeakCore.CheckAndCast[1].IsClass);
        Assert.Equal("TScultureMonster", ObjMonIcePeakCore.CheckAndCast[1].CastClass);
        Assert.NotEqual(ObjMonIcePeakCore.CheckAndCast[1].IsClass,
            ObjMonIcePeakCore.CheckAndCast[1].CastClass);
    }

    [Fact]
    public void WorksByAccidentFacts()
    {
        Assert.True(ObjMonIcePeakCore.NoNewFieldsInSculture());
        Assert.True(ObjMonIcePeakCore.SelfHasOneField());
        Assert.True(ObjMonIcePeakCore.SiblingsShareBase());
        Assert.True(ObjMonIcePeakCore.TouchedFieldsAllInherited());
        Assert.True(ObjMonIcePeakCore.HardCastNoCheck());
        Assert.True(ObjMonIcePeakCore.WorksByAccident());
        Assert.True(ObjMonIcePeakCore.WouldBreakWithAnyField());

        // **本类加了一个字段、姊妹类一个都没有 —— 这正是错转能跑的原因**
        Assert.Equal(1, ObjMonIcePeakCore.SelfNewFields);
        Assert.Equal(0, ObjMonIcePeakCore.SiblingNewFields);
    }

    [Fact]
    public void TypeCheckBoundaries()
    {
        Assert.True(ObjMonIcePeakCore.IcePeakPasses());
        Assert.True(ObjMonIcePeakCore.ScultureFailsCheck());
        Assert.True(ObjMonIcePeakCore.OnlySameClassAffected());

        Assert.True(ObjMonIcePeakCore.PassesTypeCheck("TIcePeakMonster"));
        Assert.False(ObjMonIcePeakCore.PassesTypeCheck("TScultureMonster"));
        Assert.False(ObjMonIcePeakCore.PassesTypeCheck("TMonster"));
    }

    [Fact]
    public void SemanticsDifferFacts()
    {
        Assert.True(ObjMonIcePeakCore.SemanticsDiffer());
        Assert.True(ObjMonIcePeakCore.OnlySelfGetsTickExtend());
        Assert.True(ObjMonIcePeakCore.OnlySelfGetsEvent());
        Assert.True(ObjMonIcePeakCore.OnlySelfHasGuard());
        Assert.True(ObjMonIcePeakCore.MeltActionsExtracted());
        Assert.True(ObjMonIcePeakCore.FourSharedActions());
        Assert.True(ObjMonIcePeakCore.ThreeExtraHere());
        Assert.True(ObjMonIcePeakCore.PeersWakeDifferently());
        Assert.True(ObjMonIcePeakCore.SelfCorrectPeersWrong());
        Assert.True(ObjMonIcePeakCore.RunDependsOnThatField());
        Assert.True(ObjMonIcePeakCore.PeerKeepsCreateValue());
        Assert.True(ObjMonIcePeakCore.NoQuietPeriodForPeers());
        Assert.True(ObjMonIcePeakCore.SemicolonAdded());
        Assert.True(ObjMonIcePeakCore.LineWasEdited());
        Assert.True(ObjMonIcePeakCore.EditedButStillWrong());
    }

    [Fact]
    public void MeltActionTable()
    {
        Assert.Equal(7, ObjMonIcePeakCore.MeltActions.Length);
        Assert.Equal(4, ObjMonIcePeakCore.SharedActions());

        // **前四项两边都有**
        Assert.True(ObjMonIcePeakCore.MeltActions[0].InSculture);
        Assert.True(ObjMonIcePeakCore.MeltActions[1].InSculture);
        Assert.True(ObjMonIcePeakCore.MeltActions[2].InSculture);
        Assert.True(ObjMonIcePeakCore.MeltActions[3].InSculture);

        // **后三项只有本类有**
        Assert.True(ObjMonIcePeakCore.MeltActions[4].InIcePeak);
        Assert.False(ObjMonIcePeakCore.MeltActions[4].InSculture);
        Assert.True(ObjMonIcePeakCore.MeltActions[5].InIcePeak);
        Assert.False(ObjMonIcePeakCore.MeltActions[5].InSculture);
        Assert.True(ObjMonIcePeakCore.MeltActions[6].InIcePeak);
        Assert.False(ObjMonIcePeakCore.MeltActions[6].InSculture);
    }

    // ===================== 二、三种时间写法 =====================

    [Fact]
    public void ThreeTimeIdiomsFacts()
    {
        Assert.True(ObjMonIcePeakCore.ThreeTimeIdioms());
        Assert.True(ObjMonIcePeakCore.TickDiffForWalk());
        Assert.True(ObjMonIcePeakCore.RawSubtractForSearch());
        Assert.True(ObjMonIcePeakCore.RawCompareForGate());
        Assert.True(ObjMonIcePeakCore.BeatsJ210TwoIdioms());
        Assert.True(ObjMonIcePeakCore.TimeIdiomsExtracted());
        Assert.True(ObjMonIcePeakCore.OnlyOneWrapSafe());
        Assert.True(ObjMonIcePeakCore.TwoUnsafe());

        Assert.Equal(3, ObjMonIcePeakCore.TimeIdioms.Length);
        Assert.Equal(5337, ObjMonIcePeakCore.TimeIdioms[0].Line);
        Assert.True(ObjMonIcePeakCore.TimeIdioms[0].WrapSafe);
        Assert.Equal(5374, ObjMonIcePeakCore.TimeIdioms[1].Line);
        Assert.False(ObjMonIcePeakCore.TimeIdioms[1].WrapSafe);
        Assert.Equal(5382, ObjMonIcePeakCore.TimeIdioms[2].Line);
        Assert.False(ObjMonIcePeakCore.TimeIdioms[2].WrapSafe);
    }

    [Fact]
    public void WraparoundFacts()
    {
        Assert.True(ObjMonIcePeakCore.TimeGetTimeWraps());
        Assert.True(ObjMonIcePeakCore.FortyNineDays());
        Assert.True(ObjMonIcePeakCore.SameMethodHasTheRightTool());

        // **裸比较：正常顺序为真**
        Assert.True(ObjMonIcePeakCore.NormalOrderTrue());
        Assert.True(ObjMonIcePeakCore.RawCompareGate(5000, 3000));

        // **回绕后长期为假 —— 这就是缺陷**
        Assert.True(ObjMonIcePeakCore.FalseAfterWrap());
        Assert.False(ObjMonIcePeakCore.RawCompareGate(100, 4000000000u));

        // **同一情形下 tick_diff 仍然正确**
        Assert.True(ObjMonIcePeakCore.TickDiffSurvivesWrap());
    }

    [Fact]
    public void ParenStyleFacts()
    {
        Assert.True(ObjMonIcePeakCore.ParensOptional());
        Assert.True(ObjMonIcePeakCore.SevenWithoutOneWith());
        Assert.True(ObjMonIcePeakCore.StyleInconsistency());
        Assert.True(ObjMonIcePeakCore.CallCountsAddUp());

        Assert.Equal(8, ObjMonIcePeakCore.NoParenCalls + ObjMonIcePeakCore.ParenCalls);
    }

    [Fact]
    public void CooldownBoundaryFacts()
    {
        Assert.True(ObjMonIcePeakCore.GreaterOrEqualHere());
        Assert.True(ObjMonIcePeakCore.GreaterInSiblings());
        Assert.True(ObjMonIcePeakCore.BoundaryDiffersByOne());
        Assert.True(ObjMonIcePeakCore.ExactlyAtThresholdWalks());
        Assert.True(ObjMonIcePeakCore.ExactlyAtThresholdSiblingBlocks());
        Assert.True(ObjMonIcePeakCore.OppositeAtBoundary());

        // **恰好等阈值：本类（`>=`）允许移动、兄弟类（`>`）阻断**
        Assert.True(ObjMonIcePeakCore.CanWalkHere(1000, 1500, 500, 0));
        Assert.False(ObjMonIcePeakCore.CanWalkSibling(1000, 1500, 500, 0));

        // **超一毫秒则两版都允许**
        Assert.True(ObjMonIcePeakCore.CanWalkHere(1000, 1501, 500, 0));
        Assert.True(ObjMonIcePeakCore.CanWalkSibling(1000, 1501, 500, 0));
    }

    // ===================== 三、保护不一致 =====================

    [Fact]
    public void ProtectionInconsistencyFacts()
    {
        Assert.True(ObjMonIcePeakCore.ListUnprotected());
        Assert.True(ObjMonIcePeakCore.VisibleActorsProtected());
        Assert.True(ObjMonIcePeakCore.SameClassContrast());
        Assert.True(ObjMonIcePeakCore.StrongerThanJ209());
        Assert.True(ObjMonIcePeakCore.DifferentProtectionTargets());
        Assert.True(ObjMonIcePeakCore.LockThenTry());
        Assert.True(ObjMonIcePeakCore.FinallyAfterLoop());
        Assert.True(ObjMonIcePeakCore.UnLockAfterFinally());
        Assert.True(ObjMonIcePeakCore.LoopInsideTry());
        Assert.True(ObjMonIcePeakCore.BareFree());
    }

    // ===================== 四、Run 骨架与分支 =====================

    [Fact]
    public void GuardFacts()
    {
        Assert.True(ObjMonIcePeakCore.FourFoldGuard());
        Assert.True(ObjMonIcePeakCore.OrderDiffersFromJ206());
        Assert.True(ObjMonIcePeakCore.CooldownInGuardHere());
        Assert.True(ObjMonIcePeakCore.AllTrueRuns());
        Assert.True(ObjMonIcePeakCore.GhostBlocks());
        Assert.True(ObjMonIcePeakCore.DeathBlocks());
        Assert.True(ObjMonIcePeakCore.CannotMoveBlocks());
        Assert.True(ObjMonIcePeakCore.CooldownBlocks());

        Assert.True(ObjMonIcePeakCore.CanRun(false, false, true, true));
        Assert.False(ObjMonIcePeakCore.CanRun(true, false, true, true));
        Assert.False(ObjMonIcePeakCore.CanRun(false, true, true, true));
        Assert.False(ObjMonIcePeakCore.CanRun(false, false, false, true));
        Assert.False(ObjMonIcePeakCore.CanRun(false, false, true, false));
    }

    [Fact]
    public void BranchFacts()
    {
        Assert.True(ObjMonIcePeakCore.ResetsDelayBeforeBranch());
        Assert.True(ObjMonIcePeakCore.BothBranchesReset());
        Assert.True(ObjMonIcePeakCore.MutuallyExclusiveDuties());
        Assert.True(ObjMonIcePeakCore.StoneWaitsForPrey());
        Assert.True(ObjMonIcePeakCore.AwakeSearches());
        Assert.True(ObjMonIcePeakCore.NoOverlap());
        Assert.True(ObjMonIcePeakCore.StoneBranchFirst());
        Assert.True(ObjMonIcePeakCore.StoneBranchNoSearch());
        Assert.True(ObjMonIcePeakCore.AwakeBranchNoVisibleScan());
    }

    [Fact]
    public void RadiusBoundaries()
    {
        Assert.True(ObjMonIcePeakCore.TwoSquareThreshold());
        Assert.True(ObjMonIcePeakCore.SquareNotCircular());
        Assert.True(ObjMonIcePeakCore.TriggerTwoVsEffectSeven());
        Assert.True(ObjMonIcePeakCore.AsymmetricRadii());
        Assert.True(ObjMonIcePeakCore.InsideTwoTriggers());
        Assert.True(ObjMonIcePeakCore.OutsideTwoNoTrigger());
        Assert.True(ObjMonIcePeakCore.WakeIs225());
        Assert.True(ObjMonIcePeakCore.RadiiAreaRatio());

        // **触发半径 2 是 5x5 = 25 格、效果半径 7 是 15x15 = 225 格**
        Assert.True(ObjMonIcePeakCore.WithinTrigger(2, 2));
        Assert.True(ObjMonIcePeakCore.WithinTrigger(2, 0));
        Assert.False(ObjMonIcePeakCore.WithinTrigger(3, 0));
        Assert.False(ObjMonIcePeakCore.WithinTrigger(0, 3));
        Assert.Equal(225, ObjMonIcePeakCore.WakeCellCount());
    }

    [Fact]
    public void FilterFacts()
    {
        Assert.True(ObjMonIcePeakCore.AcceptFormIdiom());
        Assert.True(ObjMonIcePeakCore.OppositeOfJ212());
        Assert.True(ObjMonIcePeakCore.VisiblePasses());
        Assert.True(ObjMonIcePeakCore.HiddenWithCoolEyePasses());
        Assert.True(ObjMonIcePeakCore.HiddenNoCoolEyeBlocked());
        Assert.True(ObjMonIcePeakCore.ConvertThenCheckNil());
        Assert.True(ObjMonIcePeakCore.EquivalentButReversed());
        Assert.True(ObjMonIcePeakCore.SkipsDead());
        Assert.True(ObjMonIcePeakCore.BreaksAfterWaking());
        Assert.True(ObjMonIcePeakCore.OnlyFirstWakes());

        Assert.True(ObjMonIcePeakCore.AcceptForm(false, false));
        Assert.True(ObjMonIcePeakCore.AcceptForm(true, true));
        Assert.False(ObjMonIcePeakCore.AcceptForm(true, false));
    }

    [Fact]
    public void SearchThrottleBoundaries()
    {
        Assert.True(ObjMonIcePeakCore.TwoTierThrottle());
        Assert.True(ObjMonIcePeakCore.EightSecondsWithTarget());
        Assert.True(ObjMonIcePeakCore.OneSecondWithoutTarget());
        Assert.True(ObjMonIcePeakCore.SameNumbersAsJ206());

        // **有目标：严格大于 8000**
        Assert.True(ObjMonIcePeakCore.SearchAfterEightWithTarget());
        Assert.True(ObjMonIcePeakCore.ExactlyEightBlocks());

        // **无目标：严格大于 1000**
        Assert.True(ObjMonIcePeakCore.SearchAfterOneWithoutTarget());
        Assert.True(ObjMonIcePeakCore.ExactlyOneBlocks());
    }

    [Fact]
    public void ConditionalInheritedBoundaries()
    {
        Assert.True(ObjMonIcePeakCore.ConditionalInherited());
        Assert.True(ObjMonIcePeakCore.ThreeWayGate());
        Assert.True(ObjMonIcePeakCore.QuietPeriodWhenAwake());
        Assert.True(ObjMonIcePeakCore.StoneModeCallsInherited());
        Assert.True(ObjMonIcePeakCore.DeathCallsInherited());
        Assert.True(ObjMonIcePeakCore.AwakeExpiredCallsInherited());
        Assert.True(ObjMonIcePeakCore.AwakeUnexpiredSkipsInherited());
        Assert.True(ObjMonIcePeakCore.DeathStillCallsInherited());
        Assert.True(ObjMonIcePeakCore.DeliberateButConfusing());
        Assert.True(ObjMonIcePeakCore.MissingNot());
        Assert.True(ObjMonIcePeakCore.GuardAndGateDisagreeOnDeath());

        // **三选一的门**
        Assert.True(ObjMonIcePeakCore.ShouldCallInherited(0, 1000, true, false));
        Assert.True(ObjMonIcePeakCore.ShouldCallInherited(0, 1000, false, true));
        Assert.True(ObjMonIcePeakCore.ShouldCallInherited(2000, 1000, false, false));
        Assert.False(ObjMonIcePeakCore.ShouldCallInherited(500, 1000, false, false));

        // **守卫用 `not m_boDeath` 挡住死亡、而门却让死亡进场 —— 两者相反**
        Assert.True(ObjMonIcePeakCore.DeathBlocks());
        Assert.True(ObjMonIcePeakCore.DeathCallsInherited());
    }

    // ===================== 五、Create 与 MeltStone =====================

    [Fact]
    public void CreateFacts()
    {
        Assert.True(ObjMonIcePeakCore.FiveFields());
        Assert.True(ObjMonIcePeakCore.SearchTimeWriteOnly());
        Assert.True(ObjMonIcePeakCore.ContinuesJ206Finding());
        Assert.True(ObjMonIcePeakCore.Random1500To2999());
        Assert.True(ObjMonIcePeakCore.OneTimeInit());
        Assert.True(ObjMonIcePeakCore.UnusedFieldInitialized());
        Assert.True(ObjMonIcePeakCore.RealFieldComputedInRun());
        Assert.True(ObjMonIcePeakCore.ViewRangeSeven());
        Assert.True(ObjMonIcePeakCore.EighthSite());
        Assert.True(ObjMonIcePeakCore.AlreadyInJ206Table());

        // **搜索时间初值范围 1500..2999**
        Assert.Equal(1500, ObjMonIcePeakCore.SearchTimeInit(0));
        Assert.Equal(2999, ObjMonIcePeakCore.SearchTimeInit(1499));
    }

    [Fact]
    public void StatusRepresentationFacts()
    {
        Assert.True(ObjMonIcePeakCore.TwoRepresentations());
        Assert.True(ObjMonIcePeakCore.StatusExNotState());
        Assert.True(ObjMonIcePeakCore.ClientUsesItOnState());
        Assert.True(ObjMonIcePeakCore.ConstantReusedOnDifferentField());
        Assert.True(ObjMonIcePeakCore.StateStoneModeIsOne());

        Assert.Equal(1, ObjMonIcePeakCore.STATE_STONE_MODE);
    }

    [Fact]
    public void RawOffsetFieldFacts()
    {
        Assert.True(ObjMonIcePeakCore.RawOffsetFieldFamily());
        Assert.True(ObjMonIcePeakCore.SameAsJ159Bo2B9());
        Assert.True(ObjMonIcePeakCore.StoneModeOffsetExtracted());
        Assert.True(ObjMonIcePeakCore.VisibleActorsOffsetExtracted());
        Assert.True(ObjMonIcePeakCore.BothHaveOffsets());
        Assert.True(ObjMonIcePeakCore.TodoCommentFrom2019());
        Assert.True(ObjMonIcePeakCore.StillUnoptimized());
    }

    [Fact]
    public void IdempotencyFacts()
    {
        Assert.True(ObjMonIcePeakCore.IdempotentHere());
        Assert.True(ObjMonIcePeakCore.SiblingNotIdempotent());
        Assert.True(ObjMonIcePeakCore.GuardMakesIdempotent());
    }

    [Fact]
    public void MessageFacts()
    {
        Assert.True(ObjMonIcePeakCore.RmDigup());
        Assert.True(ObjMonIcePeakCore.ZeroAsNativeInt());
        Assert.True(ObjMonIcePeakCore.DirectionDoubledIntoEvent());
        Assert.True(ObjMonIcePeakCore.EtIcePeakIsTen());

        Assert.Equal(20099, ObjMonIcePeakCore.RM_DIGUP);
        Assert.Equal(10, ObjMonIcePeakCore.ET_ICEPEAK);
    }

    // ===================== 六、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonIcePeakCore.EighteenClassesCovered());
        Assert.True(ObjMonIcePeakCore.RemainingApprox());
        Assert.True(ObjMonIcePeakCore.EventRunCoveredInJ126());
        Assert.True(ObjMonIcePeakCore.OneMinuteContinue());
        Assert.True(ObjMonIcePeakCore.CrossBatchReference());
    }
}
