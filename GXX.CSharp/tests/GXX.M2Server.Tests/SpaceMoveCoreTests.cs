using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J148：`TBaseObject.SpaceMove`（22448-22670）与两个同名 `GetRandXY` 1:1 测试。
/// **所有环路边界的期望值均由临时探针实测后写入，非手推。**
/// </summary>
public sealed class SpaceMoveCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(SpaceMoveCore.ConstantsMatchSource());
        Assert.Equal(128, SpaceMoveCore.RcTruckObject);
        Assert.Equal(201, SpaceMoveCore.TryLimit);
    }

    [Fact]
    public void MessageIds()
    {
        Assert.Equal(20053, SpaceMoveCore.RmUserName);
        Assert.Equal(20088, SpaceMoveCore.RmClearObjects);
        Assert.Equal(20089, SpaceMoveCore.RmChangeMap);
        Assert.Equal(20104, SpaceMoveCore.RmSpaceMoveShow);
        Assert.Equal(20106, SpaceMoveCore.RmSpaceMoveShow2);
        Assert.Equal(656, SpaceMoveCore.SmChangeNameColor);
        Assert.Equal(8900, SpaceMoveCore.SmFbTime);
    }

    [Fact]
    public void SecretFlags()
    {
        Assert.Equal(2, SpaceMoveCore.SecretFlagNoChangNameColor);
        Assert.Equal(8, SpaceMoveCore.SecretFlagShowEqualName);
        Assert.True(SpaceMoveCore.SecretFlagsAreDistinctBits());
    }

    // ===================== 一、四道开场门 =====================

    [Fact]
    public void ShopStallAndTruckGate()
    {
        Assert.True(SpaceMoveCore.TruckAlwaysBlocked());
        Assert.True(SpaceMoveCore.ShopStallOnlyPlayer());
    }

    [Fact]
    public void OpeningGates()
    {
        Assert.True(SpaceMoveCore.FourOpeningGates());
        Assert.True(SpaceMoveCore.HeroClearedOnlyWhenNotTargeting());
        Assert.True(SpaceMoveCore.HorseNeedsSecondRider());
        Assert.True(SpaceMoveCore.DummyNeedsMapPermission());
    }

    [Fact]
    public void AnyGateExitsTruthTable()
    {
        Assert.True(SpaceMoveCore.AnyGateExitsTruthTable());
        Assert.False(SpaceMoveCore.AnyGateExits(
            0, false, false, false, false, false, false, false));
        Assert.True(SpaceMoveCore.AnyGateExits(
            0, true, false, false, false, false, false, false));
    }

    [Fact]
    public void TruckBlockedRegardlessOfStall()
    {
        // **押镖车是无条件挡住的，与摆摊标志无关**
        Assert.True(SpaceMoveCore.GateShopStallOrTruck(128, false));
        Assert.True(SpaceMoveCore.GateShopStallOrTruck(128, true));
    }

    // ===================== 二、两条主分支 =====================

    [Fact]
    public void NullMapNoOpAndSameServer()
    {
        Assert.True(SpaceMoveCore.NullMapNoOp());
        Assert.True(SpaceMoveCore.IsSameServerValues());
    }

    [Fact]
    public void SameServerSwapFlow()
    {
        Assert.True(SpaceMoveCore.SameServerSwapFlowTruthTable());
        Assert.True(SpaceMoveCore.SameServerSwapFlow(true, true, true));
    }

    [Fact]
    public void RollbackConditions()
    {
        Assert.True(SpaceMoveCore.RollbackConditions());
        Assert.True(SpaceMoveCore.RollbackRestoresAllThree());
        Assert.True(SpaceMoveCore.AddToMapMustReturnSelf());
    }

    [Fact]
    public void DeleteFailureAborts()
    {
        // **摘除失败直接放弃，连回滚都不需要**
        Assert.True(SpaceMoveCore.DeleteFailureAborts(false));
        Assert.False(SpaceMoveCore.DeleteFailureAborts(true));
        Assert.True(SpaceMoveCore.DeleteFailureLeavesBo21False());
    }

    [Fact]
    public void CrossServerBranches()
    {
        Assert.True(SpaceMoveCore.CrossServerTwoBranches());
        Assert.True(SpaceMoveCore.CrossServerPlayerWritesNineFields());
        Assert.True(SpaceMoveCore.CrossServerNonPlayerKickedValues());
        Assert.True(SpaceMoveCore.DisappearAPrecedesWrites());
        Assert.Equal(9, SpaceMoveCore.CrossServerPlayerFields.Length);
    }

    // ===================== 三、成功后收尾 =====================

    [Fact]
    public void TempAdminNotRefreshedWhenSet()
    {
        Assert.True(SpaceMoveCore.TempAdminNotRefreshedWhenSet());
    }

    [Fact]
    public void EnterMapGates()
    {
        Assert.True(SpaceMoveCore.EnterMapOnlyOnMapChangeValues());
        Assert.True(SpaceMoveCore.QuestNpcClickOnEnterValues());
        Assert.True(SpaceMoveCore.MvalClearedOnEnter());
    }

    [Fact]
    public void TimeMapGate()
    {
        Assert.True(SpaceMoveCore.TimeMapGateTwoConditions());
        Assert.True(SpaceMoveCore.TimeMapGate(100000, 100000 - 61 * 1000, 1, "L1"));
        Assert.False(SpaceMoveCore.TimeMapGate(100000, 100000 - 61 * 1000, 1, ""));
    }

    [Fact]
    public void FbTimeAlwaysSent()
    {
        // **副本时间消息是无条件发的，与限时门无关**
        Assert.True(SpaceMoveCore.FbTimeAlwaysSentValues());
        Assert.True(SpaceMoveCore.FbTimeAlwaysSent(true, true));
        Assert.False(SpaceMoveCore.FbTimeAlwaysSent(false, true));

        Assert.Equal(180000, SpaceMoveCore.FbTimeArgs(3)[0]);
    }

    [Fact]
    public void HeroEnterBranch()
    {
        Assert.True(SpaceMoveCore.HeroEnterOnlyThreeConditions());
    }

    [Fact]
    public void PlayerNotifyGateAndHeroComplement()
    {
        Assert.True(SpaceMoveCore.PlayerNotifyGate());
        // **两处英雄清理条件恰好奇互补**
        Assert.True(SpaceMoveCore.TwoHeroGatesAreComplements());
        Assert.True(SpaceMoveCore.HeroClearedWhenTargeting(0, true, true));
        Assert.False(SpaceMoveCore.HeroClearedWhenTargeting(0, true, false));
    }

    [Fact]
    public void CastleAndChangeMap()
    {
        Assert.True(SpaceMoveCore.CastleChangePkStatus());
        Assert.True(SpaceMoveCore.ChangeMapTwoForms());
        Assert.Equal("A", SpaceMoveCore.ChangeMapText("A", "A", true));
        Assert.Equal("A\r\nB", SpaceMoveCore.ChangeMapText("A", "B", false));
        Assert.True(SpaceMoveCore.SearchViewRangeAfterMove());
    }

    [Fact]
    public void SecretFlagComparisons()
    {
        Assert.True(SpaceMoveCore.SecretFlagComparesOneBit());
        Assert.True(SpaceMoveCore.OtherBitsDoNotMatter());
        Assert.True(SpaceMoveCore.TwoSecretFlagsIndependently());
        Assert.True(SpaceMoveCore.SecretFlagPairChangedTruthTable());
        Assert.True(SpaceMoveCore.TwoSecretMessages());
    }

    [Fact]
    public void SpaceMoveShowTwoMessages()
    {
        Assert.True(SpaceMoveCore.SpaceMoveShowTwoMessages());
        Assert.Equal(20106, SpaceMoveCore.SpaceMoveShowMessage(1));
        Assert.Equal(20104, SpaceMoveCore.SpaceMoveShowMessage(0));
    }

    [Fact]
    public void DeferredLabelsAndFinalSteps()
    {
        Assert.True(SpaceMoveCore.DeferredLabelsAfterMove());
        Assert.True(SpaceMoveCore.FinalStepsOrdered());
        Assert.True(SpaceMoveCore.TwoTempFlagsInitFalse());
        Assert.Equal(4, SpaceMoveCore.FinalSteps.Length);
    }

    // ===================== 四、两个同名 GetRandXY =====================

    [Fact]
    public void Ladders()
    {
        Assert.True(SpaceMoveCore.StepLadderThreeTiers());
        Assert.True(SpaceMoveCore.BoundaryLadderThreeTiers());
        Assert.True(SpaceMoveCore.BoundaryNestedOrder150Then50());
    }

    [Fact]
    public void LadderExactValues()
    {
        // 探针实测
        Assert.Equal(3, SpaceMoveCore.StepX(79));
        Assert.Equal(10, SpaceMoveCore.StepX(80));
        Assert.Equal(10, SpaceMoveCore.StepX(1000));

        Assert.Equal(2, SpaceMoveCore.BoundaryY(49));
        Assert.Equal(15, SpaceMoveCore.BoundaryY(50));
        Assert.Equal(15, SpaceMoveCore.BoundaryY(149));
        Assert.Equal(50, SpaceMoveCore.BoundaryY(150));
        Assert.Equal(50, SpaceMoveCore.BoundaryY(1000));
    }

    [Fact]
    public void TwoIdenticalGetRandXY()
    {
        Assert.True(SpaceMoveCore.TwoIdenticalGetRandXY());
        Assert.True(SpaceMoveCore.TryLimit201());
        Assert.True(SpaceMoveCore.TryLimitLeavesFalse());
        Assert.True(SpaceMoveCore.NestedWritesFieldClassWritesParam());
    }

    [Fact]
    public void ImmediateWalkFound()
    {
        // 探针实测：X=5 Y=5 tries=0
        Assert.True(SpaceMoveCore.ImmediateWalkFound());
    }

    [Fact]
    public void XAdvancesByStep()
    {
        // 探针实测：X=15 Y=5 tries=1
        Assert.True(SpaceMoveCore.XAdvancesByStep());

        var r = SpaceMoveCore.FindRandXy(100, 100, 5, 5, (x, _) => x >= 15, _ => 0);

        Assert.Equal(15, r.X);
        Assert.Equal(5, r.Y);
        Assert.Equal(1, r.Tries);
    }

    [Fact]
    public void YAdvanceOnlyWhenXExhausted()
    {
        // 探针实测：X 被重置为 0，Y 推进到 15，tries=1
        Assert.True(SpaceMoveCore.YAdvanceOnlyWhenXExhausted());

        var r = SpaceMoveCore.FindRandXy(100, 100, 84, 5, (_, y) => y >= 15, _ => 0);

        Assert.Equal(0, r.X);
        Assert.Equal(15, r.Y);
        Assert.Equal(1, r.Tries);
    }

    [Fact]
    public void XResetsWhenExhausted()
    {
        // 探针实测：X=7（重置值），Y 同时被推进到 15
        Assert.True(SpaceMoveCore.XResetsWhenExhausted());

        var r = SpaceMoveCore.FindRandXy(100, 100, 84, 5, (x, _) => x == 7, _ => 7);

        Assert.Equal(7, r.X);
        Assert.Equal(15, r.Y);
    }

    [Fact]
    public void XBoundaryExact()
    {
        // 探针实测：宽 79、高 49 → n18=3、n1C=2、X 边界 76
        // **在 75 时仍推进、到 76 才重置**
        var at75 = SpaceMoveCore.FindRandXy(79, 49, 75, 5, (_, _) => false, _ => 0);

        Assert.False(at75.Found);
        Assert.Equal(201, at75.Tries);

        var at76 = SpaceMoveCore.FindRandXy(79, 49, 76, 5, (x, _) => x == 0, _ => 0);

        Assert.True(at76.Found);
        Assert.Equal(0, at76.X);
    }

    [Fact]
    public void SmallMapStepIsThree()
    {
        // 探针实测：宽 79 → 步长 3；从 5 到 11 需 2 次
        var r = SpaceMoveCore.FindRandXy(79, 49, 5, 5, (x, _) => x >= 11, _ => 0);

        Assert.True(r.Found);
        Assert.Equal(11, r.X);
        Assert.Equal(2, r.Tries);
    }

    [Fact]
    public void NormalMapStepIsTen()
    {
        // 探针实测：宽 80 → 步长 10；从 5 到 15 需 1 次
        var r = SpaceMoveCore.FindRandXy(80, 150, 5, 5, (x, _) => x >= 15, _ => 0);

        Assert.True(r.Found);
        Assert.Equal(15, r.X);
        Assert.Equal(1, r.Tries);
    }

    [Fact]
    public void YResetWhenBothExhausted()
    {
        // 探针实测：X 与 Y 都到边界时，两者都被重置为随机值
        var r = SpaceMoveCore.FindRandXy(100, 100, 84, 84, (_, y) => y == 9, _ => 9);

        Assert.True(r.Found);
        Assert.Equal(9, r.X);
        Assert.Equal(9, r.Y);
    }

    [Fact]
    public void ExhaustedReturnsFalse()
    {
        // 探针实测：tries 恰为 201
        Assert.True(SpaceMoveCore.ExhaustedReturnsFalse());

        var r = SpaceMoveCore.FindRandXy(1000, 1000, 0, 0, (_, _) => false, max => max - 1);

        Assert.False(r.Found);
        Assert.Equal(201, r.Tries);
    }

    [Fact]
    public void DifferentCallSites()
    {
        // **同服用嵌套局部函数、跨服用类方法**
        Assert.True(SpaceMoveCore.DifferentCallSites());
        Assert.StartsWith("nested", SpaceMoveCore.GetRandXyCallSite(true));
        Assert.StartsWith("class-method", SpaceMoveCore.GetRandXyCallSite(false));
    }
}
