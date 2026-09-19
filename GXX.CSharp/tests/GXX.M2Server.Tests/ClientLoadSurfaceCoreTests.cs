using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J179：客户端 `LoadSurface` 与 `GetOffset` 1:1 测试。
/// **九个 case 标签无 else、两个空分支、三重判据校验的字段与使用的字段不同、
/// 着色十四选三、以及 GetOffset 两层嵌套 case 的硬编码表（种族十八无 else）。**
/// </summary>
public sealed class ClientLoadSurfaceCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(156, ClientLoadSurfaceCore.CustomMonsterRace);
        Assert.Equal(1, ClientLoadSurfaceCore.StateStoneMode);
        Assert.Equal(100000, ClientLoadSurfaceCore.AppearanceBase);
        Assert.Equal(900, ClientLoadSurfaceCore.GhostExcludeLow);
        Assert.Equal(906, ClientLoadSurfaceCore.GhostExcludeHigh);
        Assert.Equal(1000, ClientLoadSurfaceCore.OffsetShortCircuit);
        Assert.Equal(360, ClientLoadSurfaceCore.OffsetShortCircuitMultiplier);
        Assert.Equal(14, ClientLoadSurfaceCore.ColorEffectCount);
        Assert.Equal(9, ClientLoadSurfaceCore.CaseLabelCount);
    }

    [Fact]
    public void ActionConstants()
    {
        Assert.Equal(10, ClientLoadSurfaceCore.SM_TURN);
        Assert.Equal(11, ClientLoadSurfaceCore.SM_WALK);
        Assert.Equal(6, ClientLoadSurfaceCore.SM_RUSH);
        Assert.Equal(7, ClientLoadSurfaceCore.SM_RUSHKUNG);
        Assert.Equal(9, ClientLoadSurfaceCore.SM_BACKSTEP);
        Assert.Equal(20, ClientLoadSurfaceCore.SM_DIGUP);
        Assert.Equal(14, ClientLoadSurfaceCore.SM_HIT);
        Assert.Equal(31, ClientLoadSurfaceCore.SM_STRUCK);
        Assert.Equal(32, ClientLoadSurfaceCore.SM_DEATH);
        Assert.Equal(34, ClientLoadSurfaceCore.SM_NOWDEATH);
        Assert.Equal(33, ClientLoadSurfaceCore.SM_SKELETON);
        Assert.Equal(1445, ClientLoadSurfaceCore.SM_LIGHTINGEX);
    }

    [Fact]
    public void ActionConstantsTable()
    {
        Assert.True(ClientLoadSurfaceCore.TwelveActionConstants());
        Assert.True(ClientLoadSurfaceCore.ActionConstantsDistinct());
        Assert.Equal(12, ClientLoadSurfaceCore.ActionConstants.Length);
    }

    // ===================== 一、两条大分支 =====================

    [Fact]
    public void BranchShape()
    {
        Assert.True(ClientLoadSurfaceCore.CanvasGuardFirst());
        Assert.True(ClientLoadSurfaceCore.TwoBranches());
        Assert.True(ClientLoadSurfaceCore.SameRacePredicateAsJ178());
        Assert.True(ClientLoadSurfaceCore.DuplicatedCustomMonsterPredicate());
    }

    [Fact]
    public void BranchSelection()
    {
        Assert.True(ClientLoadSurfaceCore.BranchSelection());
        Assert.Equal(1, ClientLoadSurfaceCore.Branch(156, 0));
        Assert.Equal(2, ClientLoadSurfaceCore.Branch(156, -1));
        Assert.Equal(2, ClientLoadSurfaceCore.Branch(155, 0));
    }

    [Fact]
    public void LoadSequence()
    {
        Assert.True(ClientLoadSurfaceCore.GuardBeforeTimestamp());
        Assert.True(ClientLoadSurfaceCore.NoThrottleConsumedWhenInactive());
        Assert.True(ClientLoadSurfaceCore.RetriesImmediately());
        Assert.True(ClientLoadSurfaceCore.InactiveLeavesBothUntouched());
        Assert.True(ClientLoadSurfaceCore.ActiveUpdatesBoth());

        var off = ClientLoadSurfaceCore.LoadSequence(false);
        Assert.False(off.TimestampUpdated);
        Assert.False(off.FlagCleared);

        var on = ClientLoadSurfaceCore.LoadSequence(true);
        Assert.True(on.TimestampUpdated);
        Assert.True(on.FlagCleared);
    }

    // ===================== 二、九个 case 标签 =====================

    [Fact]
    public void CaseShape()
    {
        Assert.True(ClientLoadSurfaceCore.NineCaseLabels());
        Assert.True(ClientLoadSurfaceCore.NineLabelsExtracted());
        Assert.True(ClientLoadSurfaceCore.NoElseBranch());
        Assert.True(ClientLoadSurfaceCore.UnlistedActionLeavesNil());
        Assert.True(ClientLoadSurfaceCore.NilLeadsToEmptyBody());
        Assert.True(ClientLoadSurfaceCore.SilentlyNoBody());
    }

    [Fact]
    public void EmptyBranches()
    {
        Assert.True(ClientLoadSurfaceCore.TwoEmptyBranches());
        Assert.True(ClientLoadSurfaceCore.EmptyEqualsUnlisted());
        Assert.True(ClientLoadSurfaceCore.LightingExEmpty());
        Assert.True(ClientLoadSurfaceCore.SkeletonEmpty());
        Assert.True(ClientLoadSurfaceCore.EmptyBranchesValue());
    }

    [Fact]
    public void FourMovesShareWalk()
    {
        Assert.True(ClientLoadSurfaceCore.FourMovesShareOneBranch());
        Assert.True(ClientLoadSurfaceCore.AllMapToWalk());
        Assert.True(ClientLoadSurfaceCore.RushAndBackstepUseWalk());

        Assert.Equal(ClientLoadSurfaceCore.MonAction.Walk, ClientLoadSurfaceCore.MapAction(ClientLoadSurfaceCore.SM_WALK, false));
        Assert.Equal(ClientLoadSurfaceCore.MonAction.Walk, ClientLoadSurfaceCore.MapAction(ClientLoadSurfaceCore.SM_RUSH, false));
        Assert.Equal(ClientLoadSurfaceCore.MonAction.Walk, ClientLoadSurfaceCore.MapAction(ClientLoadSurfaceCore.SM_RUSHKUNG, false));
        Assert.Equal(ClientLoadSurfaceCore.MonAction.Walk, ClientLoadSurfaceCore.MapAction(ClientLoadSurfaceCore.SM_BACKSTEP, false));
    }

    [Fact]
    public void TurnAndStone()
    {
        Assert.True(ClientLoadSurfaceCore.TurnSharesWithZero());
        Assert.True(ClientLoadSurfaceCore.StoneOverridesTurn());
        Assert.True(ClientLoadSurfaceCore.StoneUsesRevive());

        Assert.Equal(ClientLoadSurfaceCore.MonAction.Stand, ClientLoadSurfaceCore.MapAction(ClientLoadSurfaceCore.SM_TURN, false));
        Assert.Equal(ClientLoadSurfaceCore.MonAction.StoneRevive, ClientLoadSurfaceCore.MapAction(ClientLoadSurfaceCore.SM_TURN, true));
    }

    [Fact]
    public void DeathAndDigup()
    {
        Assert.True(ClientLoadSurfaceCore.DeathAndNowDeathShareDie());
        Assert.True(ClientLoadSurfaceCore.DigupUsesRevive());
        Assert.True(ClientLoadSurfaceCore.SameActionAsStone());
        Assert.True(ClientLoadSurfaceCore.HitUsesDefAttack());

        Assert.Equal(ClientLoadSurfaceCore.MonAction.Struck, ClientLoadSurfaceCore.MapAction(ClientLoadSurfaceCore.SM_STRUCK, false));
    }

    [Fact]
    public void UnlistedIsNone()
    {
        Assert.True(ClientLoadSurfaceCore.UnlistedIsNone());
        Assert.True(ClientLoadSurfaceCore.NoneIsIndistinguishable());
        Assert.Equal(ClientLoadSurfaceCore.MonAction.None, ClientLoadSurfaceCore.MapAction(12345, false));
    }

    // ===================== 三、三重判据 =====================

    [Fact]
    public void GuardShape()
    {
        Assert.True(ClientLoadSurfaceCore.TripleGuard());
        Assert.True(ClientLoadSurfaceCore.StartIndexNonNegative());
        Assert.True(ClientLoadSurfaceCore.PlayCountPositive());
        Assert.True(ClientLoadSurfaceCore.FailureZerosBoth());
        Assert.True(ClientLoadSurfaceCore.GuardValues());
        Assert.True(ClientLoadSurfaceCore.ZeroPlayCountRejected());
    }

    [Fact]
    public void GuardModel()
    {
        Assert.True(ClientLoadSurfaceCore.ActionAccepted(true, 0, 1));
        Assert.False(ClientLoadSurfaceCore.ActionAccepted(false, 0, 1));
        Assert.False(ClientLoadSurfaceCore.ActionAccepted(true, -1, 1));
        Assert.False(ClientLoadSurfaceCore.ActionAccepted(true, 0, 0));
    }

    [Fact]
    public void GuardedVsUsedFields()
    {
        // **判据校验 StartIndex 与 PlayCount，取图却用 ActionFile 与当前帧**
        Assert.True(ClientLoadSurfaceCore.GuardsDifferentFieldsThanUsed());
        Assert.True(ClientLoadSurfaceCore.StartIndexNeverUsedForIndexing());
        Assert.True(ClientLoadSurfaceCore.PlayCountNeverUsedForIndexing());
        Assert.True(ClientLoadSurfaceCore.ActionFileUnvalidated());
    }

    [Fact]
    public void CustomOffset()
    {
        Assert.True(ClientLoadSurfaceCore.OffsetIsRawCurrentFrame());
        Assert.True(ClientLoadSurfaceCore.NoStartFrameSubtraction());
        Assert.True(ClientLoadSurfaceCore.DiffersFromGlobalPath());
        Assert.True(ClientLoadSurfaceCore.CustomOffsetValues());
        Assert.Equal(37, ClientLoadSurfaceCore.CustomOffset(37));
    }

    [Fact]
    public void ActionFileRange()
    {
        Assert.True(ClientLoadSurfaceCore.ActionFileRangeChecked());
        Assert.True(ClientLoadSurfaceCore.ActionFileBoundaries());

        Assert.True(ClientLoadSurfaceCore.ActionFileValid(0, 10));
        Assert.True(ClientLoadSurfaceCore.ActionFileValid(9, 10));
        Assert.False(ClientLoadSurfaceCore.ActionFileValid(-1, 10));
        Assert.False(ClientLoadSurfaceCore.ActionFileValid(10, 10));
    }

    [Fact]
    public void FallbackIndex()
    {
        Assert.True(ClientLoadSurfaceCore.FallbackIndexMinus100000());
        Assert.True(ClientLoadSurfaceCore.AppearanceBaseIs100000());
        Assert.True(ClientLoadSurfaceCore.FallbackIndexValues());
        Assert.Equal(5, ClientLoadSurfaceCore.FallbackIndex(100005));
    }

    // ===================== 四、着色效果 =====================

    [Fact]
    public void ColorShape()
    {
        Assert.True(ClientLoadSurfaceCore.ThreeColorBranches());
        Assert.True(ClientLoadSurfaceCore.GrayHasTwoMembers());
        Assert.True(ClientLoadSurfaceCore.ElseIsNormal());
        Assert.True(ClientLoadSurfaceCore.FourteenEnumMembers());
        Assert.True(ClientLoadSurfaceCore.OnlyThreeDistinguished());
        Assert.True(ClientLoadSurfaceCore.ElevenMembersMapToNormal());
    }

    [Fact]
    public void ThreeGetters()
    {
        Assert.True(ClientLoadSurfaceCore.ThreeDifferentGetters());
        Assert.True(ClientLoadSurfaceCore.ColorEffectIndexes());
        Assert.True(ClientLoadSurfaceCore.ElevenNormalExhaustive());

        Assert.Equal(ClientLoadSurfaceCore.Getter.Gray, ClientLoadSurfaceCore.GetterFor(1));
        Assert.Equal(ClientLoadSurfaceCore.Getter.Gray, ClientLoadSurfaceCore.GetterFor(13));
        Assert.Equal(ClientLoadSurfaceCore.Getter.Bright, ClientLoadSurfaceCore.GetterFor(2));
        Assert.Equal(ClientLoadSurfaceCore.Getter.Normal, ClientLoadSurfaceCore.GetterFor(0));
    }

    [Fact]
    public void ElevenNormal()
    {
        // **穷举十四个成员，其中十一个走普通**
        int normal = 0;

        for (int c = 0; c < ClientLoadSurfaceCore.ColorEffectCount; c++)
        {
            if (ClientLoadSurfaceCore.GetterFor(c) == ClientLoadSurfaceCore.Getter.Normal)
                normal++;
        }

        Assert.Equal(11, normal);
    }

    [Fact]
    public void ReverseFrame()
    {
        Assert.True(ClientLoadSurfaceCore.GuardedByNotReverse());
        Assert.True(ClientLoadSurfaceCore.ReverseDrawsNothingInCustom());
        Assert.True(ClientLoadSurfaceCore.ReverseHasOwnBranchInGlobal());
        Assert.True(ClientLoadSurfaceCore.ReverseIndexFormula());
        Assert.True(ClientLoadSurfaceCore.ReverseIndexValues());
        Assert.True(ClientLoadSurfaceCore.ForwardDiffersFromReverse());
    }

    [Fact]
    public void ReverseIndexModel()
    {
        Assert.Equal(106, ClientLoadSurfaceCore.ReverseIndex(100, 8, 3, 1));
        Assert.Equal(103, ClientLoadSurfaceCore.ForwardIndex(100, 3));
        Assert.NotEqual(ClientLoadSurfaceCore.ForwardIndex(100, 3),
            ClientLoadSurfaceCore.ReverseIndex(100, 8, 3, 1));
    }

    // ===================== 五、隐藏鬼魂 =====================

    [Fact]
    public void GhostShape()
    {
        Assert.True(ClientLoadSurfaceCore.FourWayConjunction());
        Assert.True(ClientLoadSurfaceCore.HideGhostShortCircuit());
        Assert.True(ClientLoadSurfaceCore.CallsFinalizeNotDraw());
        Assert.True(ClientLoadSurfaceCore.MutuallyExclusive());
    }

    [Fact]
    public void GhostConjunction()
    {
        Assert.True(ClientLoadSurfaceCore.FourWayConjunctionValues());
        Assert.True(ClientLoadSurfaceCore.AllTrueHides());

        Assert.True(ClientLoadSurfaceCore.ShouldHideGhost(true, true, true, true, 100));
        Assert.False(ClientLoadSurfaceCore.ShouldHideGhost(false, true, true, true, 100));
        Assert.False(ClientLoadSurfaceCore.ShouldHideGhost(true, false, true, true, 100));
        Assert.False(ClientLoadSurfaceCore.ShouldHideGhost(true, true, false, true, 100));
        Assert.False(ClientLoadSurfaceCore.ShouldHideGhost(true, true, true, false, 100));
    }

    [Fact]
    public void GhostExclusion()
    {
        Assert.True(ClientLoadSurfaceCore.AppearanceExclusion900To906());
        Assert.True(ClientLoadSurfaceCore.ExclusionIsSevenWide());
        Assert.True(ClientLoadSurfaceCore.ExclusionBoundaries());

        Assert.True(ClientLoadSurfaceCore.ShouldHideGhost(true, true, true, true, 899));
        Assert.False(ClientLoadSurfaceCore.ShouldHideGhost(true, true, true, true, 900));
        Assert.False(ClientLoadSurfaceCore.ShouldHideGhost(true, true, true, true, 906));
        Assert.True(ClientLoadSurfaceCore.ShouldHideGhost(true, true, true, true, 907));
    }

    // ===================== 六、GetOffset =====================

    [Fact]
    public void OffsetShape()
    {
        Assert.True(ClientLoadSurfaceCore.ShortCircuitAt1000());
        Assert.True(ClientLoadSurfaceCore.Modulo10Times360());
        Assert.True(ClientLoadSurfaceCore.CommentExplainsFix());
        Assert.True(ClientLoadSurfaceCore.RaceIsDiv10());
        Assert.True(ClientLoadSurfaceCore.PosIsMod10());
    }

    [Fact]
    public void ShortCircuit()
    {
        Assert.True(ClientLoadSurfaceCore.ShortCircuitValues());
        Assert.True(ClientLoadSurfaceCore.ShortCircuitTakesPrecedence());

        Assert.Equal(1800, ClientLoadSurfaceCore.GetOffset(1005));
        Assert.Equal(0, ClientLoadSurfaceCore.GetOffset(1000));
        Assert.Equal(360, ClientLoadSurfaceCore.GetOffset(1001));
    }

    [Fact]
    public void Below1000()
    {
        Assert.True(ClientLoadSurfaceCore.Below1000UsesDiv10());

        // **种族零要用小于十的外观号**
        Assert.Equal(5 * 280, ClientLoadSurfaceCore.GetOffset(5));
        Assert.Equal(4 * 280, ClientLoadSurfaceCore.GetOffset(4));
        Assert.True(ClientLoadSurfaceCore.Race0Times280());
    }

    [Fact]
    public void Race1()
    {
        Assert.True(ClientLoadSurfaceCore.Race1Times230());
        Assert.Equal(3 * 230, ClientLoadSurfaceCore.GetOffset(13));
    }

    [Fact]
    public void Race5And6()
    {
        Assert.True(ClientLoadSurfaceCore.Race5And6());
        Assert.Equal(3 * 430, ClientLoadSurfaceCore.GetOffset(53));
        Assert.Equal(3 * 440, ClientLoadSurfaceCore.GetOffset(63));
    }

    [Fact]
    public void SharedRaces()
    {
        Assert.True(ClientLoadSurfaceCore.SharedBranchRaces());
        Assert.True(ClientLoadSurfaceCore.SharedRacesAll360());
    }

    [Fact]
    public void Race4()
    {
        Assert.True(ClientLoadSurfaceCore.Race4OverridesToOne());
        Assert.True(ClientLoadSurfaceCore.Race4OneIs600());
        Assert.True(ClientLoadSurfaceCore.Race4Values());

        Assert.Equal(600, ClientLoadSurfaceCore.GetOffset(41));
        Assert.Equal(2 * 360, ClientLoadSurfaceCore.GetOffset(42));
    }

    [Fact]
    public void NestedRaces()
    {
        Assert.True(ClientLoadSurfaceCore.NestedCaseForFourRaces());
        Assert.True(ClientLoadSurfaceCore.FourNestedRaces());
        Assert.Equal(new[] { 13, 17, 18, 19 }, ClientLoadSurfaceCore.NestedRaces);
        Assert.True(ClientLoadSurfaceCore.NestedRacesNotPureMultiplication());
    }

    [Fact]
    public void Race13()
    {
        Assert.True(ClientLoadSurfaceCore.Race13Values());
        Assert.True(ClientLoadSurfaceCore.Race13Else());
        Assert.True(ClientLoadSurfaceCore.Race13TwoIsSmaller());

        Assert.Equal(0, ClientLoadSurfaceCore.GetOffset(130));
        Assert.Equal(360, ClientLoadSurfaceCore.GetOffset(131));
        Assert.Equal(440, ClientLoadSurfaceCore.GetOffset(132));
        Assert.Equal(550, ClientLoadSurfaceCore.GetOffset(133));
        Assert.Equal(4 * 360, ClientLoadSurfaceCore.GetOffset(134));
    }

    [Fact]
    public void Race17()
    {
        Assert.True(ClientLoadSurfaceCore.Race17CoversTwoAndThree());
        Assert.True(ClientLoadSurfaceCore.Race17ElseTimes350());
        Assert.True(ClientLoadSurfaceCore.Race17Values());

        Assert.Equal(920, ClientLoadSurfaceCore.GetOffset(172));
        Assert.Equal(1280, ClientLoadSurfaceCore.GetOffset(173));
        Assert.Equal(4 * 350, ClientLoadSurfaceCore.GetOffset(174));
    }

    [Fact]
    public void Race18Table()
    {
        Assert.True(ClientLoadSurfaceCore.Race18EightEntries());
        Assert.Equal(new[] { 0, 520, 950, 1574, 1934, 2294, 2654, 3014 },
            ClientLoadSurfaceCore.Race18Table);
    }

    [Fact]
    public void Race18Gaps()
    {
        // **逐项差值：520、430、624，然后才是 360 重复**
        Assert.True(ClientLoadSurfaceCore.Race18GapValues());
        Assert.True(ClientLoadSurfaceCore.ArithmeticAfterFirst());
        Assert.True(ClientLoadSurfaceCore.OnlyLastFourAreArithmetic());
        Assert.True(ClientLoadSurfaceCore.FirstThreeGapsAreNot360());
        Assert.True(ClientLoadSurfaceCore.CommonDifference360());
        Assert.True(ClientLoadSurfaceCore.FirstGapDiffers());
        Assert.True(ClientLoadSurfaceCore.FirstGapIs520());

        Assert.Equal(520, ClientLoadSurfaceCore.Race18Gap(1));
        Assert.Equal(430, ClientLoadSurfaceCore.Race18Gap(2));
        Assert.Equal(624, ClientLoadSurfaceCore.Race18Gap(3));
        Assert.Equal(360, ClientLoadSurfaceCore.Race18Gap(4));
    }

    [Fact]
    public void Race18NoElse()
    {
        Assert.True(ClientLoadSurfaceCore.Race18NoElse());
        Assert.True(ClientLoadSurfaceCore.ThreeHaveElse());
        Assert.True(ClientLoadSurfaceCore.Race18OutOfRangeStaysZero());
        Assert.True(ClientLoadSurfaceCore.Race18Values());
        Assert.True(ClientLoadSurfaceCore.Race18OutOfRange());
        Assert.True(ClientLoadSurfaceCore.OutOfRangeZeroIsObservable());

        Assert.Equal(0, ClientLoadSurfaceCore.GetOffset(180));
        Assert.Equal(520, ClientLoadSurfaceCore.GetOffset(181));
        Assert.Equal(3014, ClientLoadSurfaceCore.GetOffset(187));
        // **越界（无 else）返回零**
        Assert.Equal(0, ClientLoadSurfaceCore.GetOffset(188));
        Assert.Equal(0, ClientLoadSurfaceCore.GetOffset(189));
    }

    [Fact]
    public void Races14To16()
    {
        Assert.True(ClientLoadSurfaceCore.Races14To16());
        Assert.Equal(1440, ClientLoadSurfaceCore.GetOffset(144));
    }

    [Fact]
    public void UnlistedRace()
    {
        Assert.True(ClientLoadSurfaceCore.UnlistedRaceZero());
        Assert.Equal(0, ClientLoadSurfaceCore.GetOffset(205));
        Assert.Equal(0, ClientLoadSurfaceCore.GetOffset(999));
    }

    [Fact]
    public void CommentContradiction()
    {
        Assert.True(ClientLoadSurfaceCore.CommentContradictsCode());
        Assert.True(ClientLoadSurfaceCore.CommentedLineAbove());
    }

    // ===================== 七、ActionChanged =====================

    [Fact]
    public void ActionChanged()
    {
        Assert.True(ClientLoadSurfaceCore.CallsActionChangedLast());
        Assert.True(ClientLoadSurfaceCore.ActionChangedIsEmpty());
        Assert.True(ClientLoadSurfaceCore.HookForSubclasses());
    }

    [Fact]
    public void RedundantNil()
    {
        Assert.True(ClientLoadSurfaceCore.BodySurfaceNilTwice());
        Assert.True(ClientLoadSurfaceCore.RedundantSecondNil());
    }

    // ===================== 行数 =====================

    [Fact]
    public void LineCounts()
    {
        Assert.True(ClientLoadSurfaceCore.TwoFragments());
        Assert.Equal(2, ClientLoadSurfaceCore.MethodLineCounts.Length);
        Assert.Equal(new[] { 115, 79 }, ClientLoadSurfaceCore.MethodLineCounts);
    }

    [Fact]
    public void LengthComparison()
    {
        Assert.True(ClientLoadSurfaceCore.LoadSurfaceIsLonger());
        Assert.True(ClientLoadSurfaceCore.GetOffsetIs79());
        Assert.True(ClientLoadSurfaceCore.LoadSurfaceShareIs59());
        Assert.True(ClientLoadSurfaceCore.LoadSurfaceExceedsBy36());
    }

    [Fact]
    public void Totals()
    {
        Assert.True(ClientLoadSurfaceCore.TotalLinesValues());
        Assert.Equal(194, ClientLoadSurfaceCore.TotalLines());
    }
}
