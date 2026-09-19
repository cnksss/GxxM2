using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J135：弓箭守卫继承链 —— `TGuardUnit` / `TArcherGuard` / `TArcherPolice` / `TMoveArcherGuard`
/// （ObjMon2.pas 77-146、1373-1765、2221-2250）1:1 测试。
/// </summary>
public sealed class ArcherGuardCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(ArcherGuardCore.ConstantsMatchSource());
        Assert.Equal(112, ArcherGuardCore.RcArcherGuard);
        Assert.Equal(142, ArcherGuardCore.RcMoveArcherGuard);
        Assert.Equal(9999, ArcherGuardCore.ArcherInitialRange);
        Assert.Equal(20, ArcherGuardCore.ArcherPoliceRace);
        Assert.Equal(120000, ArcherGuardCore.TwoMinuteMs);
        Assert.Equal(20, ArcherGuardCore.MaxLocatorCode);
    }

    [Fact]
    public void TwoArcherRacesDistinct()
    {
        Assert.True(ArcherGuardCore.TwoArcherRacesDistinct());
        Assert.True(ArcherGuardCore.TwoOwnKindRaces());
        Assert.Equal(new[] { 112, 142 }, ArcherGuardCore.OwnKindRaces);
    }

    [Fact]
    public void ExceptionMessageTemplate()
    {
        Assert.True(ArcherGuardCore.ExceptionMessageTemplate());
        Assert.Contains("TArcherGuard:Run", ArcherGuardCore.ExceptionTemplate);
        Assert.True(ArcherGuardCore.SameTechniqueAsDeleteFromMap());
    }

    [Fact]
    public void ThreeCommentedFields()
    {
        Assert.True(ArcherGuardCore.ThreeCommentedFields());
        Assert.Equal(3, ArcherGuardCore.CommentedFields.Length);
        Assert.Contains("0x54C", ArcherGuardCore.CommentedFields[0]);
        Assert.Contains("0x550", ArcherGuardCore.CommentedFields[1]);
        Assert.Contains("0x554", ArcherGuardCore.CommentedFields[2]);
    }

    [Fact]
    public void OffsetsAreContiguous()
    {
        Assert.True(ArcherGuardCore.OffsetsAreContiguous());
        Assert.True(ArcherGuardCore.OffsetsStepByFour());
        Assert.Equal("// 0x558", ArcherGuardCore.DirectionOffsetComment);
    }

    [Fact]
    public void OffsetCommentsAdjacent()
    {
        Assert.True(ArcherGuardCore.OffsetCommentsAdjacent());
        Assert.True(ArcherGuardCore.Bo2B0AndTickOffsetsStepByFour());
        Assert.Equal((0x2B0, 0x2B4), ArcherGuardCore.FieldOffsets());
    }

    // ===================== 一、PKLevel 与 m_nPkPoint =====================

    [Fact]
    public void PkLevelIsPointDiv100()
    {
        // **PKLevel = m_nPkPoint div 100**
        Assert.True(ArcherGuardCore.PkLevelIsPointDiv100());
        Assert.Equal(0, ArcherGuardCore.PkLevel(0));
        Assert.Equal(1, ArcherGuardCore.PkLevel(199));
        Assert.Equal(2, ArcherGuardCore.PkLevel(200));
    }

    [Fact]
    public void PkLevelTwoMeans200Points()
    {
        // **PKLevel >= 2 等价于点数 >= 200**
        Assert.True(ArcherGuardCore.PkLevelTwoMeans200Points());
        Assert.True(ArcherGuardCore.TwoPkFieldsDifferentUnits());
        Assert.True(ArcherGuardCore.MisreadingChangesScale());
    }

    [Fact]
    public void RedNameBoundary()
    {
        Assert.True(ArcherGuardCore.RedNameBoundary());
        Assert.False(ArcherGuardCore.IsRedName(199));
        Assert.True(ArcherGuardCore.IsRedName(200));
    }

    [Fact]
    public void UnconfiguredThresholdIsZero()
    {
        Assert.True(ArcherGuardCore.UnconfiguredThresholdIsZero());
        Assert.False(ArcherGuardCore.PassesPointThreshold(1, 0));
        Assert.True(ArcherGuardCore.PassesPointThreshold(0, 0));
    }

    [Fact]
    public void InitializeReadsTable()
    {
        // **未命中时阈值被回写为 0**
        Assert.True(ArcherGuardCore.InitializeReadsTable());
        Assert.True(ArcherGuardCore.TableHitReturnsValue());
        Assert.True(ArcherGuardCore.SameTableDecidesBoth());
    }

    [Fact]
    public void GetArcherGuardPkMonValues()
    {
        Assert.Equal((false, 0), ArcherGuardCore.GetArcherGuardPkMon(false, 999));
        Assert.Equal((true, 150), ArcherGuardCore.GetArcherGuardPkMon(true, 150));
    }

    // ===================== 二、TGuardUnit 两套规则 =====================

    [Fact]
    public void TwoRuleSetsByCastle()
    {
        Assert.True(ArcherGuardCore.TwoRuleSetsByCastle());
        Assert.True(ArcherGuardCore.CastleVersionHasGuildRules());
        Assert.True(ArcherGuardCore.NoCastleVersionRequiresPkLevel());
        Assert.True(ArcherGuardCore.RuleSetsCanDisagree());
    }

    [Fact]
    public void UnderWarAlwaysProper()
    {
        Assert.True(ArcherGuardCore.UnderWarAlwaysProper());
        Assert.True(ArcherGuardCore.UnderWarStillVetoedByAdmin());
    }

    [Fact]
    public void RaceRangeTenToFiftyVetoed()
    {
        Assert.True(ArcherGuardCore.RaceRangeTenToFiftyVetoed());
    }

    [Fact]
    public void NoCastlePkBoundary()
    {
        Assert.True(ArcherGuardCore.NoCastlePkBoundary());
    }

    [Fact]
    public void NoCastleAcceptsArcherGuardAttacker()
    {
        Assert.True(ArcherGuardCore.NoCastleAcceptsArcherGuardAttacker());
    }

    [Fact]
    public void Bo2B0BoundaryIsStrict()
    {
        // **恰好 120000 即失效**
        Assert.True(ArcherGuardCore.Bo2B0BoundaryIsStrict());
        Assert.True(ArcherGuardCore.Bo2B0WithinTwoMinutes());
        Assert.True(ArcherGuardCore.Bo2B0ExpiresAfterTwoMinutes());
    }

    [Fact]
    public void TargetCastleClearsFlag()
    {
        // **目标是城堡成员时标记立即失效**
        Assert.True(ArcherGuardCore.TargetCastleClearsFlag());
    }

    [Fact]
    public void GuardCastleProperRules()
    {
        // 全假输入 → 假
        Assert.False(ArcherGuardCore.GuardCastleProper(new ArcherGuardCore.GuardTarget()));

        // 打过我 → 真
        Assert.True(ArcherGuardCore.GuardCastleProper(new ArcherGuardCore.GuardTarget { IsLastHiter = true }));

        // 攻城 → 真
        Assert.True(ArcherGuardCore.GuardCastleProper(new ArcherGuardCore.GuardTarget { CastleUnderWar = true }));
    }

    [Fact]
    public void GuardNoCastleProperRules()
    {
        Assert.False(ArcherGuardCore.GuardNoCastleProper(new ArcherGuardCore.GuardTarget()));
        Assert.True(ArcherGuardCore.GuardNoCastleProper(new ArcherGuardCore.GuardTarget { IsLastHiter = true }));
        Assert.True(ArcherGuardCore.GuardNoCastleProper(
            new ArcherGuardCore.GuardTarget { TargetCretIsArcherGuard = true }));
    }

    [Fact]
    public void GuardProperDispatch()
    {
        var t = new ArcherGuardCore.GuardTarget { RaceServer = 0, PkPoints = 200 };

        Assert.True(ArcherGuardCore.GuardProper(t, false));
        Assert.False(ArcherGuardCore.GuardProper(t, true));
    }

    [Fact]
    public void GuildRulesClearResult()
    {
        // 攻城判真后，同行会且非 LastHiter → 被改回假
        var t = new ArcherGuardCore.GuardTarget
        {
            CastleUnderWar = true, HasMasterGuild = true, SameOrAllyGuild = true,
        };

        Assert.False(ArcherGuardCore.GuardCastleProper(t));
    }

    [Fact]
    public void GuildRuleExceptionForLastHiter()
    {
        // 若我打过他，则行会例外不生效
        var t = new ArcherGuardCore.GuardTarget
        {
            CastleUnderWar = true, HasMasterGuild = true, SameOrAllyGuild = true, IsLastHiter = true,
        };

        Assert.True(ArcherGuardCore.GuardCastleProper(t));
    }

    // ===================== 三、TArcherGuard 规则集 =====================

    [Fact]
    public void AttackTypeSelectsRuleSet()
    {
        Assert.True(ArcherGuardCore.AttackTypeSelectsRuleSet());
        Assert.True(ArcherGuardCore.CastleBranchNilGuardDiffers());
        Assert.True(ArcherGuardCore.NoCastleBranchIsVerbatimCopy());
    }

    [Fact]
    public void ThresholdVersionDiffersFromRedName()
    {
        // **点数 50：阈值版判真、红名版判假**
        Assert.True(ArcherGuardCore.ThresholdVersionDiffersFromRedName());
    }

    [Fact]
    public void ZeroThresholdOnlyZeroPoint()
    {
        Assert.True(ArcherGuardCore.ZeroThresholdOnlyZeroPoint());
    }

    [Fact]
    public void AttackTypeVetoesAdmin()
    {
        Assert.True(ArcherGuardCore.AttackTypeVetoesAdmin());
    }

    [Fact]
    public void AttackTypeProperRules()
    {
        var t = new ArcherGuardCore.GuardTarget { RaceServer = 0, PkPoints = 100 };

        Assert.True(ArcherGuardCore.AttackTypeProper(t, 100));
        Assert.False(ArcherGuardCore.AttackTypeProper(t, 99));

        // 默认结构体的 RaceServer = 0 = RC_PLAYOBJECT、PkPoints = 0，且 0 <= 100
        // → **默认值即满足玩家 + 阈值条件，故为真**（不是假）
        Assert.True(ArcherGuardCore.AttackTypeProper(new ArcherGuardCore.GuardTarget(), 100));

        // 用非玩家种族才能得到假
        Assert.False(ArcherGuardCore.AttackTypeProper(
            new ArcherGuardCore.GuardTarget { RaceServer = 80, PkPoints = 0 }, 100));
    }

    // ===================== 四、Run =====================

    [Fact]
    public void TwentyValueLocator()
    {
        Assert.True(ArcherGuardCore.TwentyValueLocator());
        Assert.Equal(21, ArcherGuardCore.LocatorAssignmentCount());
        Assert.True(ArcherGuardCore.LocatorCodesAreContiguous());
    }

    [Fact]
    public void InitialRangeIs9999()
    {
        // **9999 对比 J134 的 10**
        Assert.True(ArcherGuardCore.InitialRangeIs9999());
        Assert.True(ArcherGuardCore.AlmostAlwaysSelects());
        Assert.True(ArcherGuardCore.DifferentInitialRanges());
    }

    [Fact]
    public void ArchersSkipOwnKind()
    {
        // **弓箭手不打同类**
        Assert.True(ArcherGuardCore.ArchersSkipOwnKind());
        Assert.True(ArcherGuardCore.TruckAndOwnKindFiltered());
    }

    [Fact]
    public void ArcherLoopFilterValues()
    {
        Assert.False(ArcherGuardCore.ArcherLoopFilter(128));
        Assert.False(ArcherGuardCore.ArcherLoopFilter(112));
        Assert.False(ArcherGuardCore.ArcherLoopFilter(142));
        Assert.True(ArcherGuardCore.ArcherLoopFilter(80));
        Assert.True(ArcherGuardCore.ArcherLoopFilter(0));
    }

    [Fact]
    public void HasGhostFilterInLoop()
    {
        // **多一道 m_boGhost 过滤**
        Assert.True(ArcherGuardCore.HasGhostFilterInLoop());
    }

    [Fact]
    public void SameTickStructureAsIcicle()
    {
        Assert.True(ArcherGuardCore.SameTickStructureAsIcicle());
        Assert.True(ArcherGuardCore.BothHaveTwoTicks());
        Assert.True(ArcherGuardCore.TickSemanticsIdentical());
    }

    [Fact]
    public void TickDiffWrapsShort()
    {
        Assert.Equal(100u, ArcherGuardCore.TickDiff(100, 200));
        Assert.Equal(0u, ArcherGuardCore.TickDiff(uint.MaxValue, 0));
    }

    [Fact]
    public void ShouldTurnValues()
    {
        Assert.True(ArcherGuardCore.ShouldTurnValues());
        Assert.True(ArcherGuardCore.TurnsWhenNoTarget());
    }

    // ===================== 五、管线顺序差异 =====================

    [Fact]
    public void ArcherLacksPowerRateAdd()
    {
        // **弓箭手没有 GetPowerRateAdd 这一步**
        Assert.True(ArcherGuardCore.ArcherLacksPowerRateAdd());
        Assert.True(ArcherGuardCore.IcicleHasPowerRateAdd());
    }

    [Fact]
    public void ElementAddOrderIsInverted()
    {
        // **两者的元素增伤与封顶次序相反**
        Assert.True(ArcherGuardCore.ElementAddOrderIsInverted());
        Assert.True(ArcherGuardCore.IcicleElementAddBeforeCap());
        Assert.True(ArcherGuardCore.ArcherElementAddAfterCap());
        Assert.True(ArcherGuardCore.OrderAffectsWhetherCapIncludesElement());
    }

    [Fact]
    public void ArcherCanExceedCap()
    {
        // **次序差异导致弓箭手可突破封顶**
        Assert.True(ArcherGuardCore.ArcherCanExceedCap());
        Assert.True(ArcherGuardCore.IcicleRespectsCap());
        Assert.Equal((100, 120), ArcherGuardCore.ApplyCapAndElement(90, 100, 30));
    }

    [Fact]
    public void EndMessageDiffers()
    {
        Assert.True(ArcherGuardCore.EndMessageDiffers());
        Assert.True(ArcherGuardCore.ArcherUsesFlyAxeWithDirection());
        Assert.True(ArcherGuardCore.IcicleUsesLightingLiteralOne());
        Assert.True(ArcherGuardCore.TwoEndMessages());
    }

    [Fact]
    public void SharedPipelinePieces()
    {
        Assert.True(ArcherGuardCore.ElevenSharedSteps());
        Assert.True(ArcherGuardCore.ThreePipelineDifferences());
        Assert.Equal(11, ArcherGuardCore.SharedPipelineSteps.Length);
        Assert.Equal(3, ArcherGuardCore.PipelineDifferences.Length);
    }

    [Fact]
    public void SharedDelayFormula()
    {
        Assert.True(ArcherGuardCore.SharedDelayFormula());
        Assert.True(ArcherGuardCore.DelayValuesIdentical());
        Assert.Equal(600, ArcherGuardCore.DelayOf(0, 0));
        Assert.Equal(750, ArcherGuardCore.DelayOf(2, 3));
    }

    [Fact]
    public void SharedReboundMarker()
    {
        Assert.True(ArcherGuardCore.SharedReboundMarker());
        Assert.Equal("FT", ArcherGuardCore.ReboundMarker());
        Assert.Equal("", ArcherGuardCore.MainMarker());
    }

    [Fact]
    public void SharedParalysisGate()
    {
        Assert.True(ArcherGuardCore.SharedParalysisGate());
        Assert.True(ArcherGuardCore.ParalysisSameAsIcicle());
    }

    // ===================== 六、Struck 与 bo2B0 =====================

    [Fact]
    public void StruckSetsTwoMinuteMark()
    {
        // **需有城堡才打标记**
        Assert.True(ArcherGuardCore.StruckSetsTwoMinuteMark());
        Assert.True(ArcherGuardCore.StruckRequiresCastle());
        Assert.Equal((true, 500u), ArcherGuardCore.GuardStruck(true, 500));
        Assert.Equal((false, 0u), ArcherGuardCore.GuardStruck(false, 500));
    }

    [Fact]
    public void LazyExpiryNotTimer()
    {
        Assert.True(ArcherGuardCore.LazyExpiryNotTimer());
        Assert.True(ArcherGuardCore.ExpiryHappensOnNextCheck());
    }

    [Fact]
    public void Bo2B0DefaultsFalse()
    {
        Assert.True(ArcherGuardCore.Bo2B0DefaultsFalse());
        Assert.True(ArcherGuardCore.Bo2B0OffsetComment());
    }

    // ===================== 七、TArcherPolice 与构造 =====================

    [Fact]
    public void ArcherPoliceOnlySetsRace20()
    {
        // **只改一句种族为字面量 20**
        Assert.True(ArcherGuardCore.ArcherPoliceOnlySetsRace20());
        Assert.True(ArcherGuardCore.RaceTwentyIsLiteralNotConstant());
        Assert.True(ArcherGuardCore.NearbyRaces());
    }

    [Fact]
    public void ArcherCreateInitializers()
    {
        Assert.True(ArcherGuardCore.ArcherCreateInitializers());
        Assert.Equal((12, true, false, -1, 112, false, 0), ArcherGuardCore.ArcherCreateInit());
    }

    [Fact]
    public void DirectionSentinelIsMinusOne()
    {
        Assert.True(ArcherGuardCore.DirectionSentinelIsMinusOne());
        Assert.True(ArcherGuardCore.ViewRangeIsTwelve());
        Assert.Equal(-1, ArcherGuardCore.ArcherCreateInit().Direction);
    }

    [Fact]
    public void DefaultGoesNoCastleBranch()
    {
        Assert.True(ArcherGuardCore.DefaultGoesNoCastleBranch());
    }

    [Fact]
    public void MoveArcherCreateInitializers()
    {
        Assert.True(ArcherGuardCore.MoveArcherCreateInitializers());
        Assert.True(ArcherGuardCore.BothViewRangeTwelve());
        Assert.True(ArcherGuardCore.MoveArcherHasThreeCounters());
        Assert.Equal(-1, ArcherGuardCore.MoveArcherCreateInit().TargetX);
    }

    // ===================== 八、仿真 =====================

    [Fact]
    public void ArcherSelectsNearest()
    {
        Assert.True(ArcherGuardCore.ArcherSelectsNearest());
    }

    [Fact]
    public void ArcherSkipsOwnKindAndTruck()
    {
        // **同类与镖车被跳过**
        Assert.True(ArcherGuardCore.ArcherSkipsOwnKindAndTruck());
    }

    [Fact]
    public void ArcherSkipsGhost()
    {
        Assert.True(ArcherGuardCore.ArcherSkipsGhost());
    }

    [Fact]
    public void FarTargetAcceptedByArcher()
    {
        // **9999 上限使远距离目标仍被接受**
        Assert.True(ArcherGuardCore.FarTargetAcceptedByArcher());
    }

    [Fact]
    public void ArcherSelectGuards()
    {
        var v = new (bool, bool, bool, int, bool, int)[]
        {
            (false, false, false, 80, true, 100),
        };

        Assert.Equal(-1, ArcherGuardCore.ArcherSelect(true, false, true, true, v));
        Assert.Equal(-1, ArcherGuardCore.ArcherSelect(false, true, true, true, v));
        Assert.Equal(-1, ArcherGuardCore.ArcherSelect(false, false, false, true, v));
        Assert.Equal(-1, ArcherGuardCore.ArcherSelect(false, false, true, false, v));
        Assert.Equal(0, ArcherGuardCore.ArcherSelect(false, false, true, true, v));
    }

    [Fact]
    public void ArcherSelectSkipsDead()
    {
        var v = new (bool, bool, bool, int, bool, int)[]
        {
            (false, true, false, 80, true, 1),
            (false, false, false, 80, true, 900),
        };

        Assert.Equal(1, ArcherGuardCore.ArcherSelect(false, false, true, true, v));
    }

    [Fact]
    public void ArcherSelectTieKeepsFirst()
    {
        var v = new (bool, bool, bool, int, bool, int)[]
        {
            (false, false, false, 80, true, 50),
            (false, false, false, 80, true, 50),
        };

        Assert.Equal(0, ArcherGuardCore.ArcherSelect(false, false, true, true, v));
    }
}
