using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J126：事件子类的 `Run` 与构造
/// （GameEvent.pas 502-521 TSafeEventEx、525-546 TIcePeakEvent、549-613 TFireBurnEvent、
/// 693-724 TMapEffectEvent、728-752 TPileStones/StoneMine/MapEffectEx、202-207 TFlowerEvent）1:1 测试。
/// </summary>
public sealed class GameEventSubclassCoreTests
{
    // ===================== 一、守卫与判据总结 =====================

    [Fact]
    public void GuardPresenceVariesBySubclass()
    {
        Assert.True(GameEventSubclassCore.GuardPresenceVariesBySubclass());

        var byName = new Dictionary<string, bool>();

        foreach (var s in GameEventSubclassCore.RunSummaries)
            byName[s.Class] = s.HasClosedGuard;

        Assert.True(byName["TSafeEventEx"]);
        Assert.True(byName["TMapEffectEvent"]);
        Assert.False(byName["TFireBurnEvent"]);
        Assert.False(byName["TIcePeakEvent"]);
        Assert.False(byName["TGameEvent"]);   // **基类也无守卫**
    }

    [Fact]
    public void ThreeComparisonOperators()
    {
        Assert.True(GameEventSubclassCore.ThreeComparisonOperators());
    }

    [Fact]
    public void SafeEventExUsesGreaterOrEqual()
    {
        Assert.True(GameEventSubclassCore.SafeEventExUsesGreaterOrEqual());
    }

    [Fact]
    public void ThreeTimeBases()
    {
        Assert.True(GameEventSubclassCore.ThreeTimeBases());
    }

    [Fact]
    public void FiveRunSummaries()
    {
        Assert.Equal(5, GameEventSubclassCore.RunSummaries.Length);
    }

    // ===================== 二、TMapEffectEvent =====================

    [Fact]
    public void SpeedTickComputedAtCtor()
    {
        // 706：now + speedTime * imageCount * loopCount
        uint tick = GameEventSubclassCore.ComputeSpeedTick(1000, 50, 10, 3);

        Assert.Equal(1000u + 1500u, tick);
    }

    [Fact]
    public void MapEffectExpiresStrictlyAfter()
    {
        Assert.True(GameEventSubclassCore.MapEffectExpired(2001, 2000));
        Assert.False(GameEventSubclassCore.MapEffectExpired(2000, 2000));
        Assert.False(GameEventSubclassCore.MapEffectExpired(1999, 2000));
    }

    [Fact]
    public void SpeedTickProductOverflowsAsInteger()
    {
        // **Integer 溢出使 m_dwSpeedTick 变成极大值 → 几乎永不到期**
        Assert.True(GameEventSubclassCore.SpeedTickProductOverflowsAsInteger());
    }

    [Fact]
    public void NegativeLoopCountNeverAutoCloses()
    {
        Assert.True(GameEventSubclassCore.NegativeLoopCountNeverAutoCloses());
        Assert.False(GameEventSubclassCore.MapEffectAllowClose(-1));
        Assert.True(GameEventSubclassCore.MapEffectAllowClose(0));
        Assert.True(GameEventSubclassCore.MapEffectAllowClose(5));
    }

    [Fact]
    public void ZeroProductExpiresImmediately()
    {
        Assert.True(GameEventSubclassCore.ZeroProductExpiresImmediately());
    }

    [Fact]
    public void ZeroImageCountGivesZeroProduct()
    {
        uint tick = GameEventSubclassCore.ComputeSpeedTick(5000, 100, 0, 5);

        Assert.Equal(5000u, tick);
    }

    [Fact]
    public void BlendStoredTwice()
    {
        Assert.True(GameEventSubclassCore.BlendStoredTwice());
    }

    [Fact]
    public void ObjGameOverriddenBySubclass()
    {
        Assert.True(GameEventSubclassCore.ObjGameOverriddenBySubclass());
        Assert.Equal(1, GameEventSubclassCore.ObjMapEffect);
    }

    [Fact]
    public void AllowCloseOverridesBaseDefault()
    {
        Assert.True(GameEventSubclassCore.AllowCloseOverridesBaseDefault());
    }

    // ===================== 三、TIcePeakEvent =====================

    [Fact]
    public void GhostExtendsLifetimeByOneMinute()
    {
        Assert.True(GameEventSubclassCore.GhostExtendsLifetimeByOneMinute());
        Assert.Equal(60_000u, GameEventSubclassCore.IcePeakGhostContinueTime);
    }

    [Fact]
    public void GhostResetsOpenStartTick()
    {
        Assert.True(GameEventSubclassCore.GhostResetsOpenStartTick());
    }

    [Fact]
    public void InheritedOnlyWhenOwnerNull()
    {
        Assert.True(GameEventSubclassCore.InheritedOnlyWhenOwnerNull());
    }

    [Fact]
    public void GhostRoundDoesNotExpireImmediately()
    {
        Assert.True(GameEventSubclassCore.GhostRoundDoesNotExpireImmediately());
    }

    [Fact]
    public void IcePeakHoldsOwnerIndefinitely()
    {
        uint start = 0;
        uint cont = 1000;
        bool ownerNull = false;
        uint now = 999_999;

        var step = GameEventSubclassCore.EvaluateIcePeak(
            ownerNull, ownerGhost: false, now, ref start, ref cont, ref ownerNull);

        // **有主时不计时**
        Assert.Equal(GameEventSubclassCore.IcePeakStep.HoldingOwner, step);
    }

    [Fact]
    public void IcePeakGhostExtends()
    {
        uint start = 0;
        uint cont = 1000;
        bool ownerNull = false;
        uint now = 50_000;

        var step = GameEventSubclassCore.EvaluateIcePeak(
            ownerNull, ownerGhost: true, now, ref start, ref cont, ref ownerNull);

        Assert.Equal(GameEventSubclassCore.IcePeakStep.Extended, step);
        Assert.Equal(50_000u, start);        // 重置起点
        Assert.Equal(60_000u, cont);         // 改为一分钟
        Assert.True(ownerNull);              // 解引用
    }

    [Fact]
    public void IcePeakStateMachine()
    {
        uint start = 0;
        uint cont = 1000;
        bool ownerNull = false;

        // 第 1 轮：拥有者变幽灵 → 续命
        var s1 = GameEventSubclassCore.EvaluateIcePeak(
            ownerNull, true, 50_000, ref start, ref cont, ref ownerNull);
        Assert.Equal(GameEventSubclassCore.IcePeakStep.Extended, s1);

        // 第 2 轮：同一时刻 → 未到期
        var s2 = GameEventSubclassCore.EvaluateIcePeak(
            ownerNull, false, 50_000, ref start, ref cont, ref ownerNull);
        Assert.Equal(GameEventSubclassCore.IcePeakStep.Pending, s2);

        // 第 3 轮：超过 1 分钟 → 到期
        var s3 = GameEventSubclassCore.EvaluateIcePeak(
            ownerNull, false, 110_001, ref start, ref cont, ref ownerNull);
        Assert.Equal(GameEventSubclassCore.IcePeakStep.Expired, s3);
    }

    [Fact]
    public void IcePeakNullOwnerPendingThenExpires()
    {
        uint start = 0;
        uint cont = 1000;
        bool ownerNull = true;

        var pending = GameEventSubclassCore.EvaluateIcePeak(
            true, false, 1000, ref start, ref cont, ref ownerNull);
        Assert.Equal(GameEventSubclassCore.IcePeakStep.Pending, pending);

        var expired = GameEventSubclassCore.EvaluateIcePeak(
            true, false, 1001, ref start, ref cont, ref ownerNull);
        Assert.Equal(GameEventSubclassCore.IcePeakStep.Expired, expired);
    }

    [Fact]
    public void EventParamIsOwnerDirection()
    {
        Assert.True(GameEventSubclassCore.EventParamIsOwnerDirection());
    }

    [Fact]
    public void IcePeakRunTickIsDeadField()
    {
        Assert.True(GameEventSubclassCore.IcePeakRunTickIsDeadField());
    }

    // ===================== 四、TFireBurnEvent =====================

    [Fact]
    public void FireBurnIntervalIsThreeSeconds()
    {
        Assert.Equal(3000u, GameEventSubclassCore.FireBurnIntervalMs);
    }

    [Fact]
    public void FireBurnDueStrictlyAfter()
    {
        Assert.True(GameEventSubclassCore.FireBurnDue(3501, 500));
        Assert.False(GameEventSubclassCore.FireBurnDue(3500, 500));
    }

    [Fact]
    public void FirstDamageDependsOnRunTickInitial()
    {
        Assert.True(GameEventSubclassCore.FirstDamageDependsOnRunTickInitial());
    }

    [Fact]
    public void MagicIndexHardcodedToTwentyTwo()
    {
        Assert.True(GameEventSubclassCore.MagicIndexHardcodedToTwentyTwo());
        Assert.Equal(22, GameEventSubclassCore.FireBurnMagicIndex);
    }

    [Fact]
    public void UsesSkillPowerForThreeRaces()
    {
        Assert.True(GameEventSubclassCore.UsesSkillPower(0));    // RC_PLAYOBJECT
        Assert.True(GameEventSubclassCore.UsesSkillPower(1));    // RC_HEROOBJECT
        Assert.True(GameEventSubclassCore.UsesSkillPower(150));  // RC_PLAYMOSTER
        Assert.False(GameEventSubclassCore.UsesSkillPower(80));  // RC_MONSTER
        Assert.False(GameEventSubclassCore.UsesSkillPower(10));  // RC_NPC
    }

    [Fact]
    public void NonPlayerRacesUseRawDamage()
    {
        Assert.True(GameEventSubclassCore.NonPlayerRacesUseRawDamage());
    }

    [Fact]
    public void UnFireCrossSkipsMessageEntirely()
    {
        Assert.True(GameEventSubclassCore.UnFireCrossSkipsMessageEntirely());
        Assert.False(GameEventSubclassCore.ShouldSendFireDamage(true));
        Assert.True(GameEventSubclassCore.ShouldSendFireDamage(false));
    }

    [Fact]
    public void CobwebAppliesEvenWhenFireCrossImmune()
    {
        // **蛛网在 UnFireCross 判断之外** → 免疫火墙者仍中蛛网
        Assert.True(GameEventSubclassCore.CobwebAppliesEvenWhenFireCrossImmune());

        Assert.True(GameEventSubclassCore.TargetGetsCobweb(true));
        Assert.False(GameEventSubclassCore.TargetGetsCobweb(false));
    }

    [Fact]
    public void CobwebArgumentIsFive()
    {
        Assert.Equal(5, GameEventSubclassCore.CobwebWindingArg);
    }

    [Fact]
    public void UnFireCrossFixCommentHasDate()
    {
        Assert.True(GameEventSubclassCore.UnFireCrossFixCommentHasDate());
        Assert.Contains("防火墙无效", GameEventSubclassCore.UnFireCrossFixComment);
    }

    [Fact]
    public void FireBurnCallsInheritedUnconditionally()
    {
        Assert.True(GameEventSubclassCore.FireBurnCallsInheritedUnconditionally());
    }

    [Fact]
    public void ChangeMapCleanupIsConfigGated()
    {
        Assert.True(GameEventSubclassCore.ChangeMapCleanupIsConfigGated());
    }

    [Fact]
    public void OwnerLeftMapBoundaries()
    {
        var envir = new object();

        Assert.True(GameEventSubclassCore.OwnerLeftMap(null, envir));
        Assert.True(GameEventSubclassCore.OwnerLeftMap(new object(), envir));
        Assert.False(GameEventSubclassCore.OwnerLeftMap(envir, envir));
    }

    [Fact]
    public void OwnerLeavingMapClosesFire()
    {
        Assert.True(GameEventSubclassCore.OwnerLeavingMapClosesFire());
    }

    [Fact]
    public void SuperManExemptionCommentedOut()
    {
        Assert.True(GameEventSubclassCore.SuperManExemptionCommentedOut());
        Assert.True(GameEventSubclassCore.CommentMentionsRobotExemption());
        Assert.Contains("boSuperMan", GameEventSubclassCore.CommentedSuperManExemption);
    }

    [Fact]
    public void ThreeSegmentsOrdered()
    {
        Assert.True(GameEventSubclassCore.ThreeSegmentsOrdered());
        Assert.Equal(3, GameEventSubclassCore.FireBurnSegmentOrder.Length);
    }

    [Fact]
    public void ChangeMapSegmentShortCircuitedByClosed()
    {
        Assert.True(GameEventSubclassCore.ChangeMapSegmentShortCircuitedByClosed());
    }

    [Fact]
    public void SingleCellQueryUsesTrue()
    {
        Assert.True(GameEventSubclassCore.SingleCellQueryUsesTrue());
    }

    [Fact]
    public void OwnerNullCheckedTwice()
    {
        Assert.True(GameEventSubclassCore.OwnerNullCheckedTwice());
    }

    [Fact]
    public void RequiresProperTarget()
    {
        Assert.True(GameEventSubclassCore.RequiresProperTarget());
    }

    [Fact]
    public void ShouldDamageTargetFourConditions()
    {
        Assert.True(GameEventSubclassCore.ShouldDamageTarget(false, false, true, false));

        Assert.False(GameEventSubclassCore.ShouldDamageTarget(true, false, true, false));
        Assert.False(GameEventSubclassCore.ShouldDamageTarget(false, true, true, false));
        Assert.False(GameEventSubclassCore.ShouldDamageTarget(false, false, false, false));
        Assert.False(GameEventSubclassCore.ShouldDamageTarget(false, false, true, true));
    }

    // ===================== 五、TSafeEvent / TSafeEventEx =====================

    [Fact]
    public void SafeEventNeverExpires()
    {
        Assert.True(GameEventSubclassCore.SafeEventNeverExpires());
    }

    [Fact]
    public void SafeEventUsesTickAsDurationSuspicious()
    {
        Assert.True(GameEventSubclassCore.SafeEventUsesTickAsDurationSuspicious());
    }

    [Fact]
    public void AllowCloseOverriddenInSubclassChain()
    {
        Assert.True(GameEventSubclassCore.AllowCloseOverriddenInSubclassChain());
    }

    [Fact]
    public void SafeEventExCtorChain()
    {
        Assert.True(GameEventSubclassCore.SafeEventExCtorChain());
    }

    [Fact]
    public void GreaterEqualFiresAtExactBoundary()
    {
        // **`>=` 与 `>` 的唯一差异点**
        Assert.True(GameEventSubclassCore.GreaterEqualFiresAtExactBoundary());
    }

    [Fact]
    public void SafeEventExDueBoundaries()
    {
        Assert.True(GameEventSubclassCore.SafeEventExDue(1000, 0, 1));   // 恰好 1000ms
        Assert.False(GameEventSubclassCore.SafeEventExDue(999, 0, 1));
        Assert.True(GameEventSubclassCore.SafeEventExDue(1001, 0, 1));
    }

    [Fact]
    public void ShowTimeAndCreateTickRecordedAtCtor()
    {
        Assert.True(GameEventSubclassCore.ShowTimeAndCreateTickRecordedAtCtor());
    }

    // ===================== 六、无 Run 的子类 =====================

    [Fact]
    public void SevenSubclassesHaveNoRun()
    {
        Assert.True(GameEventSubclassCore.SevenSubclassesHaveNoRun());
        Assert.Equal(7, GameEventSubclassCore.SubclassesWithoutRunOverride.Length);
    }

    [Fact]
    public void FourSubclassesOverrideRun()
    {
        Assert.True(GameEventSubclassCore.FourSubclassesOverrideRun());
        Assert.Equal(4, GameEventSubclassCore.RunOverridingSubclasses.Length);
        Assert.Contains("TFireBurnEvent", GameEventSubclassCore.RunOverridingSubclasses);
        Assert.Contains("TIcePeakEvent", GameEventSubclassCore.RunOverridingSubclasses);
    }

    [Fact]
    public void NoOverlapBetweenRunAndNoRun()
    {
        var noRun = new HashSet<string>(GameEventSubclassCore.SubclassesWithoutRunOverride);

        foreach (var c in GameEventSubclassCore.RunOverridingSubclasses)
            Assert.DoesNotContain(c, noRun);
    }

    [Fact]
    public void PileStonesParamStartsAtOne()
    {
        Assert.True(GameEventSubclassCore.PileStonesParamStartsAtOne());
    }

    [Fact]
    public void AddEventParamCapsAtFive()
    {
        Assert.True(GameEventSubclassCore.AddEventParamCapsAtFive());
    }

    [Fact]
    public void AddEventParamIncrements()
    {
        Assert.Equal(2, GameEventSubclassCore.AddEventParam(1));
        Assert.Equal(5, GameEventSubclassCore.AddEventParam(4));
        Assert.Equal(5, GameEventSubclassCore.AddEventParam(5));   // 严格 `<` → 5 不变
        Assert.Equal(99, GameEventSubclassCore.AddEventParam(99)); // **已超上限则原样返回**
    }

    [Fact]
    public void AddEventParamNotInRun()
    {
        Assert.True(GameEventSubclassCore.AddEventParamNotInRun());
    }

    [Fact]
    public void AddStoneMineUpdatesDifferentTickThanManagerUses()
    {
        Assert.True(GameEventSubclassCore.AddStoneMineUpdatesDifferentTickThanManagerUses());
        Assert.True(GameEventSubclassCore.TwoDistinctAddTicks());
    }

    [Fact]
    public void MapEffectExUsesLiteralTenSeconds()
    {
        Assert.True(GameEventSubclassCore.MapEffectExUsesLiteralTenSeconds());
        Assert.True(GameEventSubclassCore.MapEffectExConstantIsTenThousand());
        Assert.Equal(10_000, GameEventSubclassCore.MapEffectExContinueTime);
    }

    [Fact]
    public void GarbledCommentRetained()
    {
        Assert.True(GameEventSubclassCore.GarbledCommentRetained());
        Assert.True(GameEventSubclassCore.GarbledCommentMatchesSource());
    }

    [Fact]
    public void GarbledCommentKeepsQuestionsAndDate()
    {
        // **中文已因编码丢失，保留问号、不可臆造原意**
        Assert.Contains("?", GameEventSubclassCore.GarbledComment);
        Assert.Contains("2013-11-14", GameEventSubclassCore.GarbledComment);
    }

    [Fact]
    public void FlowerEventIsThinWrapper()
    {
        Assert.True(GameEventSubclassCore.FlowerEventIsThinWrapper());
    }

    [Fact]
    public void FlowerCommentHasDate()
    {
        Assert.True(GameEventSubclassCore.FlowerCommentHasDate());
        Assert.Contains("烟花", GameEventSubclassCore.FlowerComment);
    }

    [Fact]
    public void BaseShouldExpireBoundary()
    {
        Assert.True(GameEventSubclassCore.BaseShouldExpire(1001, 0, 1000));
        Assert.False(GameEventSubclassCore.BaseShouldExpire(1000, 0, 1000));
    }
}
