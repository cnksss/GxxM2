using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J183：基类 `TActor.Run` 1:1 测试（282 行，经 J182 确证只为 NPC 版服务）。
/// **十九级错误码插桩、除以一点八与乘三分之二不等价、
/// 以及传送门 NPC 的两处互补守卫。**
/// </summary>
public sealed class ClientActorRunCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(0, ClientActorRunCore.MinErrorCode);
        Assert.Equal(18, ClientActorRunCore.MaxErrorCode);
        Assert.Equal(19, ClientActorRunCore.ErrorCodeCount);
        Assert.Equal(1.8, ClientActorRunCore.FastDivisor);
        Assert.Equal(500, ClientActorRunCore.FixedStandFrameTime);
        Assert.Equal(200, ClientActorRunCore.SmoothMoveGate);
        Assert.Equal(200, ClientActorRunCore.PortalStandFrameTime);
        Assert.Equal(54, ClientActorRunCore.PortalRange1Low);
        Assert.Equal(58, ClientActorRunCore.PortalRange1High);
        Assert.Equal(94, ClientActorRunCore.PortalRange2Low);
        Assert.Equal(98, ClientActorRunCore.PortalRange2High);
        Assert.Equal(50, ClientActorRunCore.PortalRace);
        Assert.Equal(-10, ClientActorRunCore.DefFrameSentinelValue);
        Assert.Equal(2, ClientActorRunCore.SpellFrameLeadTwo);
        Assert.Equal(1, ClientActorRunCore.SpellFrameReserve);
        Assert.Equal(300, ClientActorRunCore.CustomMagicCount);
    }

    // ===================== 一、错误码插桩 =====================

    [Fact]
    public void ErrorCodeShape()
    {
        Assert.True(ClientActorRunCore.NineteenErrorCodes());
        Assert.True(ClientActorRunCore.RangeZeroTo18());
        Assert.True(ClientActorRunCore.CodesAreSequential());
        Assert.True(ClientActorRunCore.NoGapsNoDuplicates());
        Assert.True(ClientActorRunCore.CodeEqualsStepIndex());
        Assert.True(ClientActorRunCore.ZeroAtEntry());
        Assert.Equal(19, ClientActorRunCore.ErrorCodes.Length);
    }

    [Fact]
    public void ErrorCodeValues()
    {
        Assert.Equal(0, ClientActorRunCore.ErrorCodes[0]);
        Assert.Equal(18, ClientActorRunCore.ErrorCodes[18]);
        Assert.Equal(13, ClientActorRunCore.ErrorCodes[13]);
    }

    [Fact]
    public void InstrumentationPurpose()
    {
        Assert.True(ClientActorRunCore.ExceptPrintsCode());
        Assert.True(ClientActorRunCore.InstrumentationNotHandling());
        Assert.True(ClientActorRunCore.PinpointsFailingStep());
    }

    [Fact]
    public void LogFormat()
    {
        Assert.True(ClientActorRunCore.LogLineValues());
        Assert.Equal("TActor.Run:0", ClientActorRunCore.LogLine(0));
        Assert.Equal("TActor.Run:18", ClientActorRunCore.LogLine(18));
    }

    [Fact]
    public void CodesOnCommentedCalls()
    {
        // **末尾两个码落在被注释的调用上、编号不回收**
        Assert.True(ClientActorRunCore.CodesOnCommentedCalls());
        Assert.True(ClientActorRunCore.NumberingNotRecycled());
        Assert.Equal(new[] { 17, 18 }, ClientActorRunCore.CodesOnCommented);
    }

    // ===================== 二、帧时长 =====================

    [Fact]
    public void ThreeSites()
    {
        Assert.True(ClientActorRunCore.ThreeFrameTimeSites());
        Assert.True(ClientActorRunCore.DivideBy1_8());
        Assert.True(ClientActorRunCore.MultiplyTwoThirds());
        Assert.True(ClientActorRunCore.FallbackRaw());
    }

    [Fact]
    public void FactorsAreNotEquivalent()
    {
        // **除以一点八等于乘九分之五、而乘三分之二是另一回事**
        Assert.True(ClientActorRunCore.FactorIs5Over9());
        Assert.True(ClientActorRunCore.FactorIs2Over3());
        Assert.True(ClientActorRunCore.NotEquivalent());
        Assert.True(ClientActorRunCore.TwelvePercentApart());
        Assert.True(ClientActorRunCore.RelativeDiffIsOneSixth());
    }

    [Fact]
    public void FactorValues()
    {
        // **三千毫秒下：快路径 167、次快路径 200 —— 差 33 毫秒**
        Assert.Equal(167, ClientActorRunCore.FrameTimeFast(300));
        Assert.Equal(200, ClientActorRunCore.FrameTimeTwoThirds(300));
        Assert.Equal(56, ClientActorRunCore.FrameTimeFast(100));
        Assert.Equal(67, ClientActorRunCore.FrameTimeTwoThirds(100));
    }

    [Fact]
    public void FastIsShorter()
    {
        Assert.True(ClientActorRunCore.FastIsShorter());
        Assert.True(ClientActorRunCore.BothShorterThanRaw());
    }

    [Fact]
    public void PrioritiesAndRounding()
    {
        Assert.True(ClientActorRunCore.HighestPriority());
        Assert.True(ClientActorRunCore.TripleAndCondition());
        Assert.True(ClientActorRunCore.FastestPathConditions());
        Assert.True(ClientActorRunCore.BothRounded());
        Assert.True(ClientActorRunCore.BankersRounding());
    }

    [Fact]
    public void SelectFrameTimeModel()
    {
        Assert.True(ClientActorRunCore.SelectFrameTimeValues());

        // **非本地施法且外观普通 → 最快**
        Assert.Equal(167, ClientActorRunCore.SelectFrameTime(300, false, true, 0, false, 0));
        // **消息过多 → 次快**
        Assert.Equal(200, ClientActorRunCore.SelectFrameTime(300, false, false, 0, true, 0));
        // **本地玩家 → 原值**
        Assert.Equal(300, ClientActorRunCore.SelectFrameTime(300, true, false, 0, false, 0));
    }

    [Fact]
    public void PortalExcludedFromTwoThirds()
    {
        Assert.True(ClientActorRunCore.PortalExcludedFromTwoThirds());
        Assert.Equal(300, ClientActorRunCore.SelectFrameTime(300, true, false, 54, true, 50));
    }

    // ===================== 三、传送门 NPC =====================

    [Fact]
    public void PortalFacts()
    {
        Assert.True(ClientActorRunCore.AppearanceRangeFourTimes());
        Assert.True(ClientActorRunCore.MapsToMA54());
        Assert.True(ClientActorRunCore.MA54StandTime200());
        Assert.True(ClientActorRunCore.ComplementaryGuards());
        Assert.True(ClientActorRunCore.SamePredicateNegatedAndAffirmed());
        Assert.True(ClientActorRunCore.PortalNpcSpecialCased());
    }

    [Fact]
    public void PortalRangeBoundaries()
    {
        Assert.True(ClientActorRunCore.PortalRangeBoundaries());
        Assert.True(ClientActorRunCore.RangeHasGap());

        Assert.False(ClientActorRunCore.InPortalRange(53));
        Assert.True(ClientActorRunCore.InPortalRange(54));
        Assert.True(ClientActorRunCore.InPortalRange(58));
        Assert.False(ClientActorRunCore.InPortalRange(59));
        Assert.False(ClientActorRunCore.InPortalRange(93));
        Assert.True(ClientActorRunCore.InPortalRange(94));
        Assert.True(ClientActorRunCore.InPortalRange(98));
        Assert.False(ClientActorRunCore.InPortalRange(99));
    }

    [Fact]
    public void PortalPredicateModel()
    {
        Assert.True(ClientActorRunCore.ComplementaryValues());

        Assert.True(ClientActorRunCore.IsPortalNpc(50, 54));
        Assert.False(ClientActorRunCore.IsPortalNpc(50, 53));
        Assert.False(ClientActorRunCore.IsPortalNpc(49, 54));
    }

    [Fact]
    public void FixComment()
    {
        Assert.True(ClientActorRunCore.FixCommentPresent());
        Assert.True(ClientActorRunCore.RecordsTheChange());
    }

    [Fact]
    public void StandTiming()
    {
        Assert.True(ClientActorRunCore.MA54UsesTableTime());
        Assert.True(ClientActorRunCore.OthersUse500());
        Assert.True(ClientActorRunCore.ThreeFiveHundreds());
    }

    // ===================== 四、站立分支三层回退 =====================

    [Fact]
    public void ThreeTierFallback()
    {
        Assert.True(ClientActorRunCore.ThreeTierFallback());
        Assert.True(ClientActorRunCore.PortalThenCustomThenFixed());
        Assert.True(ClientActorRunCore.StdTimeStrictlyPositive());
        Assert.True(ClientActorRunCore.ZeroMeansUnset());
    }

    [Fact]
    public void SelectStandFrameTimeModel()
    {
        Assert.True(ClientActorRunCore.SelectStandFrameTimeValues());
        Assert.True(ClientActorRunCore.ZeroStdTimeFallsThrough());

        // **传送门 → 表内二百**
        Assert.Equal(200, ClientActorRunCore.SelectStandFrameTime(50, 54, false, 0));
        // **自定义且有正时长 → 用该时长**
        Assert.Equal(123, ClientActorRunCore.SelectStandFrameTime(0, 0, true, 123));
        // **其余 → 固定五百**
        Assert.Equal(500, ClientActorRunCore.SelectStandFrameTime(0, 0, false, 0));
    }

    [Fact]
    public void SmoothMoveBypass()
    {
        Assert.True(ClientActorRunCore.SmoothMoveBypass());
        Assert.True(ClientActorRunCore.TwoHundredGate());
        Assert.True(ClientActorRunCore.TwoDifferentNumbers());

        Assert.NotEqual(ClientActorRunCore.SmoothMoveGate, ClientActorRunCore.FixedStandFrameTime);
    }

    [Fact]
    public void ReloadFlagAndLoop()
    {
        Assert.True(ClientActorRunCore.ReloadFlagInEachTier());
        Assert.True(ClientActorRunCore.LineTriplicated());
        Assert.True(ClientActorRunCore.StandActionLoops());
        Assert.True(ClientActorRunCore.ContrastsWithEffect());
    }

    [Fact]
    public void AdvanceStandFrameModel()
    {
        Assert.True(ClientActorRunCore.AdvanceStandFrameValues());

        // **循环归零**
        Assert.Equal(1, ClientActorRunCore.AdvanceStandFrame(0, 10));
        Assert.Equal(9, ClientActorRunCore.AdvanceStandFrame(8, 10));
        Assert.Equal(0, ClientActorRunCore.AdvanceStandFrame(9, 10));
    }

    // ===================== 五、动作推进 =====================

    [Fact]
    public void SpellAdvance()
    {
        Assert.True(ClientActorRunCore.SpellStopsOneFrameEarly());
        Assert.True(ClientActorRunCore.EndFrameMinusOne());
        Assert.True(ClientActorRunCore.SpellFrameMinusTwo());
        Assert.True(ClientActorRunCore.OrMagicTimeout());
    }

    [Fact]
    public void SpellReserveModel()
    {
        Assert.True(ClientActorRunCore.ShouldAdvanceSpellValues());

        Assert.True(ClientActorRunCore.ShouldAdvanceSpell(0, 10));
        Assert.True(ClientActorRunCore.ShouldAdvanceSpell(8, 10));
        // **末帧保留**
        Assert.False(ClientActorRunCore.ShouldAdvanceSpell(9, 10));
        Assert.False(ClientActorRunCore.ShouldAdvanceSpell(10, 10));
    }

    [Fact]
    public void ActionEnd()
    {
        Assert.True(ClientActorRunCore.LocalWaitsForServer());
        Assert.True(ClientActorRunCore.RemoteEndsUnconditionally());
        Assert.True(ClientActorRunCore.LocalMayStall());
        Assert.True(ClientActorRunCore.ThreeEndActions());
        Assert.True(ClientActorRunCore.IdenticalCleanup());
    }

    [Fact]
    public void DeleteAfterFinished()
    {
        Assert.True(ClientActorRunCore.DeleteAfterFinished());
        Assert.True(ClientActorRunCore.ThreeDeleteFields());
    }

    [Fact]
    public void DefFrame()
    {
        Assert.True(ClientActorRunCore.UsesDefFrameSentinel());
        Assert.True(ClientActorRunCore.DefFrameSentinelIsMinus10());
        Assert.True(ClientActorRunCore.MinusTenForThreeAppearances());
        Assert.True(ClientActorRunCore.ZeroForOthers());
        Assert.True(ClientActorRunCore.DefFrameValues());
    }

    [Fact]
    public void DefFrameModel()
    {
        Assert.Equal(-10, ClientActorRunCore.DefFrameFor(0));
        Assert.Equal(-10, ClientActorRunCore.DefFrameFor(1));
        Assert.Equal(-10, ClientActorRunCore.DefFrameFor(43));
        Assert.Equal(0, ClientActorRunCore.DefFrameFor(2));
        Assert.Equal(0, ClientActorRunCore.DefFrameFor(42));
    }

    // ===================== 六、收尾 =====================

    [Fact]
    public void EndingChecks()
    {
        Assert.True(ClientActorRunCore.FiveChecksWithTwoCommented());
        Assert.True(ClientActorRunCore.ThreeCommentedTotal());
        Assert.True(ClientActorRunCore.CodesRetainedForCommented());
        Assert.True(ClientActorRunCore.BaseHasFourActiveChecks());
        Assert.True(ClientActorRunCore.DifferInActiveChecks());
        Assert.Equal(3, ClientActorRunCore.CommentedCalls.Length);
    }

    [Fact]
    public void CommentedCallValues()
    {
        Assert.Equal(
            new[]
            {
                "CheckLoadActorIcon then LoadActorIcons",
                "CheckLoadHealthNumber then LoadHealthNumber",
                "CheckLoadPlayEffect then LoadPlayEffectSurface",
            },
            ClientActorRunCore.CommentedCalls);
    }

    [Fact]
    public void ActiveCheckValues()
    {
        Assert.Equal(
            new[]
            {
                "CheckLoadUserName", "CheckLoadNumberLable",
                "CheckLoadSay", "CheckLoadFengHaoSurface",
            },
            ClientActorRunCore.BaseActiveChecks);
    }

    [Fact]
    public void ReloadCondition()
    {
        Assert.True(ClientActorRunCore.ReloadConditionIdentical());
        Assert.True(ClientActorRunCore.VerbatimSame());
        Assert.True(ClientActorRunCore.NeedsReloadValues());
    }

    [Fact]
    public void ReloadModel()
    {
        Assert.True(ClientActorRunCore.NeedsReload(1, 2, 0, 0));
        Assert.True(ClientActorRunCore.NeedsReload(1, 1, 0, 1));
        Assert.False(ClientActorRunCore.NeedsReload(1, 1, 0, 0));
    }

    // ===================== 七、行数与跨批次 =====================

    [Fact]
    public void LineCounts()
    {
        Assert.True(ClientActorRunCore.LineCounts());
        Assert.True(ClientActorRunCore.HumLongerBy154());
        Assert.True(ClientActorRunCore.StillNoInherited());
        Assert.True(ClientActorRunCore.AllOwnLogic());

        Assert.Equal(282, ClientActorRunCore.BaseRunLines);
        Assert.Equal(436, ClientActorRunCore.HumRunLines);
    }

    [Fact]
    public void Density()
    {
        Assert.True(ClientActorRunCore.ErrorCodeDensity());
        Assert.True(ClientActorRunCore.OneCodePerStep());
        Assert.Equal(14, ClientActorRunCore.BaseRunLines / ClientActorRunCore.ErrorCodeCount);
    }
}
