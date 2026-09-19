using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J170：`TEnvirnoment` 对象收集族与门查询 1:1 测试。
/// **过滤强度谱系 1/2/4/5 用穷举包含关系固证、
/// bo2B9 被城堡门开关注反、GetEvent 恒把全局留假。**
/// </summary>
public sealed class EnvirObjectCollectCoreTests
{
    // ===================== 一、bo2B9 =====================

    [Fact]
    public void Bo2B9Basics()
    {
        Assert.True(EnvirObjectCollectCore.Bo2B9IsPublicField());
        Assert.Equal(204, EnvirObjectCollectCore.Bo2B9DeclLine);
        Assert.True(EnvirObjectCollectCore.CommentIsOffsetOnly());
        Assert.Equal("0x2B9", EnvirObjectCollectCore.Bo2B9Comment);
    }

    [Fact]
    public void Bo2B9Counts()
    {
        Assert.Equal(18, EnvirObjectCollectCore.ReadCount());
        Assert.True(EnvirObjectCollectCore.EighteenReads());
        Assert.Equal(3, EnvirObjectCollectCore.WriteCount());
        Assert.True(EnvirObjectCollectCore.ThreeWrites());
    }

    [Fact]
    public void Bo2B9Semantics()
    {
        Assert.True(EnvirObjectCollectCore.MeaningIsCollectable());
        Assert.True(EnvirObjectCollectCore.CastleDoorOpenClearsIt());
        Assert.True(EnvirObjectCollectCore.CastleDoorCloseSetsIt());
        Assert.True(EnvirObjectCollectCore.InvertedByDoorState());
    }

    [Fact]
    public void DoorStateValues()
    {
        // **开门置假、关门置真**
        Assert.True(EnvirObjectCollectCore.DoorStateValues());
        Assert.False(EnvirObjectCollectCore.Bo2B9(true));
        Assert.True(EnvirObjectCollectCore.Bo2B9(false));
    }

    [Fact]
    public void WriteSites()
    {
        Assert.True(EnvirObjectCollectCore.ThreeWriteSites());
        Assert.Equal(3, EnvirObjectCollectCore.WriteSites.Length);
        Assert.True(EnvirObjectCollectCore.WriteSites[0].Value);
        Assert.False(EnvirObjectCollectCore.WriteSites[1].Value);
        Assert.True(EnvirObjectCollectCore.WriteSites[2].Value);
    }

    [Fact]
    public void UsedAsGate()
    {
        Assert.True(EnvirObjectCollectCore.UsedAsGeneralGate());
        Assert.Equal(14, EnvirObjectCollectCore.AndSiteCount());
        Assert.True(EnvirObjectCollectCore.FourteenAndSites());
        Assert.True(EnvirObjectCollectCore.RestAreOtherStyles());
    }

    // ===================== 二、收集族过滤强度 =====================

    [Fact]
    public void FilterSpectrum()
    {
        Assert.True(EnvirObjectCollectCore.ThreeCollectorsSameShape());
        Assert.True(EnvirObjectCollectCore.FilterCountTwoFourFive());
        Assert.True(EnvirObjectCollectCore.FourCollectors());
        Assert.True(EnvirObjectCollectCore.StrictlyIncreasing());
        Assert.True(EnvirObjectCollectCore.StrictlyNested());
        Assert.True(EnvirObjectCollectCore.FilterCountSpectrum());
        Assert.Equal(new[] { 1, 2, 4, 5 }, new[]
        {
            EnvirObjectCollectCore.FilterCounts[0].Conditions,
            EnvirObjectCollectCore.FilterCounts[1].Conditions,
            EnvirObjectCollectCore.FilterCounts[2].Conditions,
            EnvirObjectCollectCore.FilterCounts[3].Conditions,
        });
    }

    [Fact]
    public void NestingDepths()
    {
        Assert.True(EnvirObjectCollectCore.PlayHasExtraNestingLevel());
        Assert.True(EnvirObjectCollectCore.NestingDepths());
        Assert.Equal(3, EnvirObjectCollectCore.NestingDepth.Length);
    }

    [Fact]
    public void AdditiveConditions()
    {
        Assert.True(EnvirObjectCollectCore.BaseAddsTwoOverItem());
        Assert.True(EnvirObjectCollectCore.PlayAddsRaceOverBase());
    }

    [Fact]
    public void PlayIsBasePlusRace()
    {
        // **穷举验证：玩家对象过滤 = 基础对象过滤 且 种族是玩家**
        Assert.True(EnvirObjectCollectCore.PlayIsBasePlusRace());
        Assert.True(EnvirObjectCollectCore.PlayIsSubsetOfBase());
    }

    [Fact]
    public void FilterBoundaries()
    {
        Assert.True(EnvirObjectCollectCore.BaseIsNarrowerThanItemViaBo2B9());
        Assert.True(EnvirObjectCollectCore.Bo2B9BlocksActorCollection());
        Assert.True(EnvirObjectCollectCore.GhostAlwaysRejected());
        Assert.True(EnvirObjectCollectCore.NonActorRejected());
    }

    [Fact]
    public void DeathClamp()
    {
        Assert.True(EnvirObjectCollectCore.DeathClampOnlyWhenTrue());
        Assert.True(EnvirObjectCollectCore.BothUseDeathClamp());
        Assert.True(EnvirObjectCollectCore.SameClampExpression());
        Assert.True(EnvirObjectCollectCore.ExcludeWhenTrue());
        Assert.True(EnvirObjectCollectCore.ClampTextShape());
    }

    [Fact]
    public void FilterModels()
    {
        // 活玩家：物品不收（类型）、基础与玩家都收
        var livePlayer = new EnvirObjectCollectCore.Candidate(1, 0, false, true, false);
        Assert.False(EnvirObjectCollectCore.ItemFilter(livePlayer));
        Assert.True(EnvirObjectCollectCore.BaseFilter(livePlayer, false));
        Assert.True(EnvirObjectCollectCore.PlayFilter(livePlayer, false));

        // 活怪物：基础收、玩家不收
        var liveMonster = new EnvirObjectCollectCore.Candidate(1, 80, false, true, false);
        Assert.True(EnvirObjectCollectCore.BaseFilter(liveMonster, false));
        Assert.False(EnvirObjectCollectCore.PlayFilter(liveMonster, false));

        // 非幽灵物品：物品收、基础与玩家不收
        var item = new EnvirObjectCollectCore.Candidate(2, 0, false, true, false);
        Assert.True(EnvirObjectCollectCore.ItemFilter(item));
        Assert.False(EnvirObjectCollectCore.BaseFilter(item, false));
    }

    // ===================== 三、GetEvent =====================

    [Fact]
    public void EventBasics()
    {
        Assert.True(EnvirObjectCollectCore.EventIsOneCondition());
        Assert.True(EnvirObjectCollectCore.EventResetsGlobal());
        Assert.True(EnvirObjectCollectCore.EventReturnsLast());
        Assert.True(EnvirObjectCollectCore.EventHasNoGate());
        Assert.Equal(3, EnvirObjectCollectCore.ObjEvent);
    }

    [Fact]
    public void EventGlobalAlwaysFalse()
    {
        // **它写全局却从不置真 —— 调用后恒假**
        Assert.True(EnvirObjectCollectCore.NeverSetsTrue());
        Assert.True(EnvirObjectCollectCore.EventAlwaysLeavesFalse());
        Assert.True(EnvirObjectCollectCore.AlwaysFalseAfterEvent());
        Assert.False(EnvirObjectCollectCore.GlobalAfterEventValue());
    }

    [Fact]
    public void EventFilter()
    {
        Assert.True(EnvirObjectCollectCore.FiltersEventType());
        Assert.True(EnvirObjectCollectCore.NonEventRejected());
        Assert.True(EnvirObjectCollectCore.EventIgnoresGhost());
    }

    [Fact]
    public void EventLastValues()
    {
        Assert.True(EnvirObjectCollectCore.EventLastValues());
        Assert.Equal(9, EnvirObjectCollectCore.LastOf(new List<int> { 5, 9 }));
        Assert.Null(EnvirObjectCollectCore.LastOf(new List<int>()));
    }

    // ===================== 四、GetDoor =====================

    [Fact]
    public void DoorBasics()
    {
        Assert.True(EnvirObjectCollectCore.GetDoorScansGlobalList());
        Assert.True(EnvirObjectCollectCore.NotCellBased());
        Assert.True(EnvirObjectCollectCore.MatchesXY());
        Assert.True(EnvirObjectCollectCore.ReturnsFirstMatch());
        Assert.True(EnvirObjectCollectCore.GetDoorUnlocked());
    }

    [Fact]
    public void DoorFirstMatch()
    {
        // **同坐标多门时返回第一个**
        Assert.True(EnvirObjectCollectCore.FirstMatchValues());

        var doors = new List<(int, int)> { (5, 5), (5, 5), (7, 7) };
        Assert.Equal(0, EnvirObjectCollectCore.FindDoor(doors, 5, 5));
        Assert.Equal(2, EnvirObjectCollectCore.FindDoor(doors, 7, 7));
        Assert.Null(EnvirObjectCollectCore.FindDoor(doors, 9, 9));
    }

    [Fact]
    public void DoorComplexity()
    {
        Assert.True(EnvirObjectCollectCore.OppositeComplexity());
        Assert.True(EnvirObjectCollectCore.LinearInDoorCount());
        Assert.True(EnvirObjectCollectCore.ScanStepsGrow());
        Assert.Equal(100, EnvirObjectCollectCore.ScanSteps(100));
    }

    // ===================== 五、两个有效对象判定 =====================

    [Fact]
    public void ValidatorBasics()
    {
        Assert.True(EnvirObjectCollectCore.TwoValidatorsSameShape());
        Assert.True(EnvirObjectCollectCore.ExAddsSkeletonCheck());
        Assert.True(EnvirObjectCollectCore.IdentityComparison());
        Assert.True(EnvirObjectCollectCore.IdentityNotCoordinate());
        Assert.True(EnvirObjectCollectCore.UsedToValidateCachedRefs());
    }

    [Fact]
    public void ValidatorValues()
    {
        Assert.True(EnvirObjectCollectCore.PlainValidatorValues());
        Assert.True(EnvirObjectCollectCore.ExValidatorValues());
        Assert.True(EnvirObjectCollectCore.OnlyDifferOnSkeleton());

        // 命中非骷髅：两者都真
        Assert.True(EnvirObjectCollectCore.IsValid(true, false, false));
        Assert.True(EnvirObjectCollectCore.IsValid(true, false, true));

        // 命中骷髅：不查者真、查者假
        Assert.True(EnvirObjectCollectCore.IsValid(true, true, false));
        Assert.False(EnvirObjectCollectCore.IsValid(true, true, true));

        // 未命中：都假
        Assert.False(EnvirObjectCollectCore.IsValid(false, false, false));
        Assert.False(EnvirObjectCollectCore.IsValid(false, false, true));
    }

    [Fact]
    public void PerCellLocking()
    {
        Assert.True(EnvirObjectCollectCore.UnlockedPerCell());
        Assert.True(EnvirObjectCollectCore.PerCellLockInline());
        Assert.True(EnvirObjectCollectCore.SameEffectDifferentStyle());
        Assert.True(EnvirObjectCollectCore.TwoLockStyles());
        Assert.Equal(2, EnvirObjectCollectCore.LockStyles.Length);
    }

    [Fact]
    public void LockCycles()
    {
        // **与上一批一致：半径二给二十五**
        Assert.True(EnvirObjectCollectCore.LockCyclesValues());
        Assert.Equal(25, EnvirObjectCollectCore.LockCycles(2));
        Assert.Equal(9, EnvirObjectCollectCore.LockCycles(1));
        Assert.Equal(1, EnvirObjectCollectCore.LockCycles(0));
    }

    [Fact]
    public void SquareNotCircle()
    {
        Assert.True(EnvirObjectCollectCore.SquareNotCircle());
        Assert.True(EnvirObjectCollectCore.SquareVsCircle());
    }

    [Fact]
    public void SkeletonOpaque()
    {
        Assert.True(EnvirObjectCollectCore.SkeletonCheckedOnParameter());
        Assert.True(EnvirObjectCollectCore.EquivalentButOpaque());
    }

    // ===================== 行数 =====================

    [Fact]
    public void LineCounts()
    {
        Assert.True(EnvirObjectCollectCore.SixMethods());
        Assert.Equal(6, EnvirObjectCollectCore.MethodLineCounts.Length);
        Assert.Equal(new[] { 37, 39, 29, 17, 38, 37 }, EnvirObjectCollectCore.MethodLineCounts);
    }

    [Fact]
    public void RelativeLengths()
    {
        Assert.True(EnvirObjectCollectCore.BaseShorterThanPlay());
        Assert.True(EnvirObjectCollectCore.ValidatorsNearlyEqual());
        Assert.True(EnvirObjectCollectCore.DoorIsShortest());
        Assert.Equal(2, EnvirObjectCollectCore.MethodLineCounts[1] - EnvirObjectCollectCore.MethodLineCounts[0]);
        Assert.Equal(1, EnvirObjectCollectCore.MethodLineCounts[4] - EnvirObjectCollectCore.MethodLineCounts[5]);
    }

    [Fact]
    public void Totals()
    {
        Assert.True(EnvirObjectCollectCore.TotalLinesValues());
        Assert.Equal(197, EnvirObjectCollectCore.TotalLines());
        Assert.Equal(76, EnvirObjectCollectCore.CollectorLines());
        Assert.True(EnvirObjectCollectCore.CollectorLinesIs76());
        Assert.True(EnvirObjectCollectCore.CollectorShareIs38());
    }
}
