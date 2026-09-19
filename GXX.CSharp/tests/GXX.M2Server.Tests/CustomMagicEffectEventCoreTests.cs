using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J128：自定义魔法特效事件 `TCustomMagicEffectEvent` 与地图魔法事件 `TMapMagicGameEvent`
/// （GameEvent.pas 155-194 声明、986-1010 构造、1012-1175 Run、1179-1183 构造、1185-1235 Run）1:1 测试。
/// </summary>
public sealed class CustomMagicEffectEventCoreTests
{
    // ===================== 常量与枚举 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.Equal(20108, CustomMagicEffectEventCore.RmShowEvent);
        Assert.Equal(20048, CustomMagicEffectEventCore.RmStruck);
        Assert.Equal(125, CustomMagicEffectEventCore.EtSpringsLight);
        Assert.Equal(12, CustomMagicEffectEventCore.StateFrozen);
        Assert.Equal(200, CustomMagicEffectEventCore.StruckLiteralArg);
        Assert.Equal(3, CustomMagicEffectEventCore.FixedStatusDuration);
        Assert.Equal(10, CustomMagicEffectEventCore.StatusChanceModulus);
    }

    [Fact]
    public void EnumValuesAreSequential()
    {
        Assert.True(CustomMagicEffectEventCore.EnumValuesAreSequential());
    }

    [Fact]
    public void SixEnumValues()
    {
        Assert.Equal(6, CustomMagicEffectEventCore.AllFeatures.Length);
        Assert.Equal(5, CustomMagicEffectEventCore.CaseCoveredFeatures.Length);
    }

    [Fact]
    public void FiveBranchesOutOfSixEnumValues()
    {
        Assert.True(CustomMagicEffectEventCore.FiveBranchesOutOfSixEnumValues());
        Assert.True(CustomMagicEffectEventCore.AfNoneHasNoBranch());
    }

    [Fact]
    public void AfNoneNotInCaseCoverage()
    {
        Assert.DoesNotContain(
            CustomMagicEffectEventCore.AdditionalFeature.AfNone,
            CustomMagicEffectEventCore.CaseCoveredFeatures);
    }

    [Fact]
    public void BaseDelayAlsoDeadHere()
    {
        Assert.True(CustomMagicEffectEventCore.BaseDelayAlsoDeadHere());
        Assert.Equal(300u, CustomMagicEffectEventCore.BaseDelay);
    }

    // ===================== 一、Rate 语义相反（核心） =====================

    [Fact]
    public void RateSemanticsAreOppositeBetweenTwins()
    {
        Assert.True(CustomMagicEffectEventCore.RateSemanticsAreOppositeBetweenTwins());
        Assert.True(CustomMagicEffectEventCore.J127RateIsDenominator());
        Assert.True(CustomMagicEffectEventCore.J128RateIsPercentage());
    }

    [Fact]
    public void RateZeroIsOpposite()
    {
        // **Rate = 0：J127 必定触发、J128 永不触发**
        Assert.True(CustomMagicEffectEventCore.J127ZeroRateAlwaysFires());
        Assert.True(CustomMagicEffectEventCore.J128ZeroRateNeverFires());
    }

    [Fact]
    public void J128RateAboveHundredAlwaysFires()
    {
        Assert.True(CustomMagicEffectEventCore.J128RateAboveHundredAlwaysFires());
    }

    [Fact]
    public void RandomHundredMaxIsNinetyNine()
    {
        Assert.True(CustomMagicEffectEventCore.RandomHundredMaxIsNinetyNine());
    }

    [Fact]
    public void RateTableIsInverted()
    {
        Assert.True(CustomMagicEffectEventCore.RateTableIsInverted());
        Assert.Equal(3, CustomMagicEffectEventCore.RateComparison.Length);
    }

    [Fact]
    public void TwoSidesIndependent()
    {
        // 姊妹类的 Rate 语义在边界三点上完全相反（对照表）
        var t = CustomMagicEffectEventCore.RateComparison;

        Assert.Equal((0, "必定触发", "永不触发"), t[0]);
        Assert.Equal((1, "必定触发", "1%"), t[1]);
        Assert.Equal((100, "1%", "必定触发"), t[2]);
    }

    [Fact]
    public void ThisRateIsPercentageBoundaries()
    {
        Assert.True(CustomMagicEffectEventCore.ThisRateIsPercentage(100, 0));
        Assert.True(CustomMagicEffectEventCore.ThisRateIsPercentage(100, 99));
        Assert.False(CustomMagicEffectEventCore.ThisRateIsPercentage(50, 50));
        Assert.True(CustomMagicEffectEventCore.ThisRateIsPercentage(50, 49));
    }

    // ===================== 二、伤害段整段被注释 =====================

    [Fact]
    public void DamageBlockIsEntirelyCommentedOut()
    {
        Assert.True(CustomMagicEffectEventCore.DamageBlockIsEntirelyCommentedOut());
        Assert.Equal((1044, 1077), CustomMagicEffectEventCore.CommentedDamageRange);
    }

    [Fact]
    public void SixDamagePartsDisabled()
    {
        Assert.True(CustomMagicEffectEventCore.SixDamagePartsDisabled());
        Assert.Equal(6, CustomMagicEffectEventCore.CommentedDamageParts.Length);
    }

    [Fact]
    public void J128DamageIsRawValue()
    {
        // **伤害恒等于构造传入的原始值**
        Assert.True(CustomMagicEffectEventCore.J128DamageIsRawValue());
        Assert.Equal(123, CustomMagicEffectEventCore.ComputeDamage(123));
        Assert.Equal(0, CustomMagicEffectEventCore.ComputeDamage(0));
    }

    [Fact]
    public void IgnoreDefenceAndMagStruckHaveNoEffect()
    {
        // 注释块内的分支不生效 → 两参数怎么变都返回原值
        Assert.Equal(100, CustomMagicEffectEventCore.RunCustomMagicDamage(100, true, 999));
        Assert.Equal(100, CustomMagicEffectEventCore.RunCustomMagicDamage(100, false, 999));
    }

    [Fact]
    public void J127AppliesDamageModifiers()
    {
        Assert.True(CustomMagicEffectEventCore.J127AppliesDamageModifiers());
    }

    [Fact]
    public void SuckDamageFeatureDisabledByComment()
    {
        Assert.True(CustomMagicEffectEventCore.SuckDamageFeatureDisabledByComment());
    }

    [Fact]
    public void CommentedApiSignatureIsStale()
    {
        // 注释块内的 GetMagStruckDamage 只传两参（与 J127 三参版不同）
        Assert.True(CustomMagicEffectEventCore.CommentedApiSignatureIsStale());
    }

    [Fact]
    public void ClientRecalcCommentExplainsWhy()
    {
        Assert.True(CustomMagicEffectEventCore.ClientRecalcCommentHasDate());
        Assert.True(CustomMagicEffectEventCore.ClientRecalcCommentExplainsWhy());
        Assert.Contains("ClientMagStruck", CustomMagicEffectEventCore.ClientRecalcComment);
    }

    // ===================== 三、无发起者死亡处理 =====================

    [Fact]
    public void NoOwnerDeathHandling()
    {
        Assert.True(CustomMagicEffectEventCore.NoOwnerDeathHandling());
        Assert.True(CustomMagicEffectEventCore.ConfigBlockHasNoElseBranch());
    }

    [Fact]
    public void J128FreeOfJ127NilBug()
    {
        // J127 的 812/974 and/or 缺陷在此文件不存在
        Assert.True(CustomMagicEffectEventCore.J128FreeOfJ127NilBug());
        Assert.True(CustomMagicEffectEventCore.NoBugLinesHere());
        Assert.Equal(new[] { 812, 974 }, CustomMagicEffectEventCore.J127BugLines);
    }

    // ===================== 四、姊妹类相同之处 =====================

    [Fact]
    public void SameSingleClampAsTwin()
    {
        Assert.True(CustomMagicEffectEventCore.SameSingleClampAsTwin());
        Assert.Equal(1, CustomMagicEffectEventCore.ClampAttackInterval(0));
        Assert.Equal(1, CustomMagicEffectEventCore.ClampAttackInterval(-3));
        Assert.Equal(5, CustomMagicEffectEventCore.ClampAttackInterval(5));
    }

    [Fact]
    public void ThrottleIdenticalToTwin()
    {
        Assert.True(CustomMagicEffectEventCore.ThrottleIdenticalToTwin());

        // 严格 `>`：3000 > 3000 为假
        Assert.False(CustomMagicEffectEventCore.AttackDue(3000, 0, 3));
        Assert.True(CustomMagicEffectEventCore.AttackDue(3001, 0, 3));
    }

    [Fact]
    public void TargetDispatchIdenticalToTwin()
    {
        Assert.True(CustomMagicEffectEventCore.TargetDispatchIdenticalToTwin());
        Assert.Equal("GetMovingObject", CustomMagicEffectEventCore.TargetApiFor(0));
        Assert.Equal("GetRangeBaseObject", CustomMagicEffectEventCore.TargetApiFor(5));
    }

    [Fact]
    public void TargetLoopConditionIdenticalToTwin()
    {
        Assert.True(CustomMagicEffectEventCore.TargetLoopConditionIdenticalToTwin());
    }

    [Fact]
    public void ConfigBlockIsomorphicToTwin()
    {
        Assert.True(CustomMagicEffectEventCore.ConfigBlockIsomorphicToTwin());
        Assert.Equal("boDisableChangeMapFireCross", CustomMagicEffectEventCore.ConfigKey);
    }

    [Fact]
    public void SuperManCommentAppearsThreeTimes()
    {
        // 607(J126) / 967(J127) / 1169(J128) —— 第三份副本
        Assert.True(CustomMagicEffectEventCore.ThirdCopyOfSuperManComment());
        Assert.True(CustomMagicEffectEventCore.SuperManCommentAppearsThreeTimes());
        Assert.Equal(new[] { 607, 967, 1169 }, CustomMagicEffectEventCore.SuperManCommentLines);
    }

    // ===================== 五、1009 独有构造语句 =====================

    [Fact]
    public void J128SeedsRunTickAtCtor()
    {
        Assert.True(CustomMagicEffectEventCore.J128SeedsRunTickAtCtor());
        Assert.True(CustomMagicEffectEventCore.J127DoesNotSeedRunTick());
    }

    [Fact]
    public void FirstRunTimingDiffersBetweenTwins()
    {
        Assert.True(CustomMagicEffectEventCore.FirstRunTimingDiffersBetweenTwins());
    }

    [Fact]
    public void J128FirstFireIsRelativeToCtor()
    {
        Assert.True(CustomMagicEffectEventCore.J128FirstFireIsRelativeToCtor());
        Assert.False(CustomMagicEffectEventCore.J128FirstFireAt(10_000, 12_000, 2));
        Assert.True(CustomMagicEffectEventCore.J128FirstFireAt(10_000, 13_001, 2));
    }

    [Fact]
    public void J127FirstFireDependsOnInitialFiveHundred()
    {
        Assert.True(CustomMagicEffectEventCore.J127FirstFireDependsOnInitialFiveHundred());
    }

    // ===================== 六、TMapMagicGameEvent 门控 =====================

    [Fact]
    public void MapMagicUsesTickAsDurationLikeSafeEvent()
    {
        Assert.True(CustomMagicEffectEventCore.MapMagicUsesTickAsDurationLikeSafeEvent());
    }

    [Fact]
    public void MapMagicNeverAutoCloses()
    {
        Assert.True(CustomMagicEffectEventCore.MapMagicNeverAutoCloses());
        Assert.False(CustomMagicEffectEventCore.ShouldExpire(false));
    }

    [Fact]
    public void MapMagicUsesGreaterOrEqual()
    {
        Assert.True(CustomMagicEffectEventCore.MapMagicUsesGreaterOrEqual());
    }

    [Fact]
    public void GreaterOrEqualFiresAtBoundary()
    {
        // **`>=` 与 `>` 的唯一差异点**
        Assert.True(CustomMagicEffectEventCore.GreaterOrEqualFiresAtBoundary());
        Assert.True(CustomMagicEffectEventCore.MapMagicGate(false, 3000, 0, 3));
        Assert.False(CustomMagicEffectEventCore.AttackDue(3000, 0, 3));
    }

    [Fact]
    public void MapMagicGateRequiresEnvir()
    {
        Assert.True(CustomMagicEffectEventCore.MapMagicGate(false, 3000, 0, 3));
        Assert.False(CustomMagicEffectEventCore.MapMagicGate(true, 3000, 0, 3));
    }

    [Fact]
    public void MapMagicGateThrottle()
    {
        Assert.False(CustomMagicEffectEventCore.MapMagicGate(false, 2999, 0, 3));
        Assert.True(CustomMagicEffectEventCore.MapMagicGate(false, 3000, 0, 3));
    }

    // ===================== 七、环境为 nil 的短路差异 =====================

    [Fact]
    public void NullEnvirDoesNotRefreshTickHere()
    {
        // 1191 把环境判定并入 `and` 左侧 → 短路、连 tick 都不刷新
        Assert.True(CustomMagicEffectEventCore.NullEnvirDoesNotRefreshTickHere());
        Assert.True(CustomMagicEffectEventCore.CustomClassesBurnWindowButThisDoesNot());
    }

    [Fact]
    public void RefreshesTickWhenEnvirNull()
    {
        // mapMagicStyle = true → **不刷新**；两个 Custom 类 → 刷新
        Assert.False(CustomMagicEffectEventCore.RefreshesTickWhenEnvirNull(true));
        Assert.True(CustomMagicEffectEventCore.RefreshesTickWhenEnvirNull(false));
    }

    // ===================== 八、种族与目标判定 =====================

    [Fact]
    public void OnlyPlayersAffected()
    {
        Assert.True(CustomMagicEffectEventCore.OnlyPlayersAffected(0));    // RC_PLAYOBJECT
        Assert.False(CustomMagicEffectEventCore.OnlyPlayersAffected(1));   // RC_HEROOBJECT
        Assert.False(CustomMagicEffectEventCore.OnlyPlayersAffected(80));  // RC_MONSTER
        Assert.False(CustomMagicEffectEventCore.OnlyPlayersAffected(10));  // RC_NPC
    }

    [Fact]
    public void OnlyPlayersAffectedFlag()
    {
        Assert.True(CustomMagicEffectEventCore.OnlyPlayersAffectedFlag());
        Assert.True(CustomMagicEffectEventCore.NoIsProperTargetCheck());
    }

    // ===================== 九、五个 case 分支 =====================

    [Fact]
    public void BranchTable()
    {
        Assert.Equal(5, CustomMagicEffectEventCore.BranchTable.Length);
    }

    [Fact]
    public void ThreeBranchesUseRandomTen()
    {
        Assert.True(CustomMagicEffectEventCore.ThreeBranchesUseRandomTen());
        Assert.Equal(new[] { "afPalsy", "afFreeze", "afCobweb" },
            CustomMagicEffectEventCore.RandomTenBranches);
    }

    [Fact]
    public void TwoPoisonBranchesHaveNoChance()
    {
        Assert.True(CustomMagicEffectEventCore.TwoPoisonBranchesHaveNoChance());
        Assert.Equal(new[] { "afGreenPoison", "afRedPoison" },
            CustomMagicEffectEventCore.NoChanceBranches);
    }

    [Fact]
    public void RandomTenChanceIsOneInTen()
    {
        Assert.True(CustomMagicEffectEventCore.RandomTenChanceIsOneInTen());
    }

    [Fact]
    public void DamageReusedAsPoisonDuration()
    {
        // **"伤害值"在此处被复用为"毒持续时间"**
        Assert.True(CustomMagicEffectEventCore.DamageReusedAsPoisonDuration());
        Assert.Contains("m_nDamage", CustomMagicEffectEventCore.BranchTable[1].Effect);
        Assert.Contains("m_nDamage", CustomMagicEffectEventCore.BranchTable[2].Effect);
    }

    [Fact]
    public void TwoStatusQueryMechanisms()
    {
        Assert.True(CustomMagicEffectEventCore.TwoStatusQueryMechanisms());
        Assert.True(CustomMagicEffectEventCore.StatusQueriesSplitIntoTwoGroups());
        Assert.Equal(4, CustomMagicEffectEventCore.StatusArrBranches.Length);
        Assert.Single(CustomMagicEffectEventCore.BooleanStatusBranches);
    }

    [Fact]
    public void CobwebUsesBooleanStatus()
    {
        // afCobweb 查布尔字段而非状态计时数组
        Assert.Contains("m_boCobwebWindingStatus", CustomMagicEffectEventCore.GuardFor("afCobweb"));
    }

    [Fact]
    public void StatusArrBranchesUseTimeArr()
    {
        Assert.Contains("m_wStatusTimeArr", CustomMagicEffectEventCore.GuardFor("afPalsy"));
        Assert.Contains("m_wStatusTimeArr", CustomMagicEffectEventCore.GuardFor("afFreeze"));
    }

    [Fact]
    public void FixedDurationIsThree()
    {
        Assert.True(CustomMagicEffectEventCore.FixedDurationIsThree());
    }

    // ===================== 十、发送语义 =====================

    [Fact]
    public void MakeWordPacksTypeAndParam()
    {
        Assert.True(CustomMagicEffectEventCore.MakeWordPacksTypeAndParam());
    }

    [Fact]
    public void MakeWordTruncatesToByte()
    {
        // 低字节保留、高字节左移 8 位
        Assert.True(CustomMagicEffectEventCore.MakeWordTruncatesToByte());
        Assert.Equal((ushort)0x0104, CustomMagicEffectEventCore.MakeWord(4, 1));
    }

    [Fact]
    public void ShowEventGatedByKeepVisible()
    {
        Assert.True(CustomMagicEffectEventCore.ShowEventGatedByKeepVisible());
        Assert.True(CustomMagicEffectEventCore.ShouldSendShowEvent(false));
        Assert.False(CustomMagicEffectEventCore.ShouldSendShowEvent(true));
    }

    [Fact]
    public void SpringsLightNotGatedByKeepVisible()
    {
        // **泉水补发不受 FKeepVisible 门控**
        Assert.True(CustomMagicEffectEventCore.SpringsLightNotGatedByKeepVisible());
        Assert.True(CustomMagicEffectEventCore.ShouldSendSpringsLight(122, keepVisible: true));
        Assert.True(CustomMagicEffectEventCore.ShouldSendSpringsLight(122, keepVisible: false));
    }

    [Fact]
    public void SpringsRangeIsThree()
    {
        Assert.True(CustomMagicEffectEventCore.SpringsRangeIsThree());
    }

    [Fact]
    public void SpringsLightNotSentForOtherTypes()
    {
        Assert.False(CustomMagicEffectEventCore.ShouldSendSpringsLight(4, false));
        Assert.False(CustomMagicEffectEventCore.ShouldSendSpringsLight(125, false));
    }

    [Fact]
    public void DamageHealthSourceIsNull()
    {
        Assert.True(CustomMagicEffectEventCore.DamageHealthSourceIsNull());
    }

    [Fact]
    public void StruckLiteralTwoHundred()
    {
        Assert.True(CustomMagicEffectEventCore.StruckLiteralTwoHundred());
    }

    [Fact]
    public void NativeIntArgDiffersBetweenSends()
    {
        // 1205 传 Self、1209 传 nil
        Assert.True(CustomMagicEffectEventCore.NativeIntArgDiffersBetweenSends());
        Assert.True(CustomMagicEffectEventCore.ShowEventPassesSelf());
    }

    [Fact]
    public void VisibleTickAssignedPerTarget()
    {
        Assert.True(CustomMagicEffectEventCore.VisibleTickAssignedPerTarget());
        Assert.Equal(300u, CustomMagicEffectEventCore.FinalVisibleTick(new List<uint> { 100, 200, 300 }));
        Assert.Equal(0u, CustomMagicEffectEventCore.FinalVisibleTick(new List<uint>()));
    }

    [Fact]
    public void InheritedOnlyClearsOwnerHere()
    {
        Assert.True(CustomMagicEffectEventCore.CallsInheritedUnconditionally());
        Assert.True(CustomMagicEffectEventCore.InheritedOnlyClearsOwnerHere());
        Assert.True(CustomMagicEffectEventCore.BaseExpirySegmentInactive(false));
    }

    // ===================== 十一、顶层仿真 =====================

    [Fact]
    public void MapMagicRoundThrottled()
    {
        uint runTick = 0;

        var r = CustomMagicEffectEventCore.RunMapMagicRound(
            envirNull: false, now: 2999, ref runTick, attackTime: 3,
            raceServers: new List<int> { 0 }, eventType: 4, keepVisible: false);

        Assert.False(r.GateOpen);
        Assert.Equal(0u, runTick);
    }

    [Fact]
    public void MapMagicRoundNullEnvirDoesNotRefresh()
    {
        uint runTick = 0;

        var r = CustomMagicEffectEventCore.RunMapMagicRound(
            envirNull: true, now: 5000, ref runTick, attackTime: 3,
            raceServers: new List<int> { 0 }, eventType: 4, keepVisible: false);

        Assert.False(r.GateOpen);
        Assert.Equal(0u, runTick);   // **未刷新**
    }

    [Fact]
    public void MapMagicRoundOnlyPlayers()
    {
        uint runTick = 0;

        var r = CustomMagicEffectEventCore.RunMapMagicRound(
            envirNull: false, now: 3000, ref runTick, attackTime: 3,
            raceServers: new List<int> { 0, 80, 10, 0 }, eventType: 4, keepVisible: false);

        Assert.True(r.GateOpen);
        Assert.Equal(2, r.PlayerCount);      // 只有两个 RC_PLAYOBJECT
        Assert.Equal(2, r.StruckCount);
        Assert.Equal(2, r.ShowEventCount);
        Assert.Equal(3000u, runTick);
    }

    [Fact]
    public void MapMagicRoundKeepVisibleSuppressesShowEvent()
    {
        uint runTick = 0;

        var r = CustomMagicEffectEventCore.RunMapMagicRound(
            envirNull: false, now: 3000, ref runTick, attackTime: 3,
            raceServers: new List<int> { 0 }, eventType: 4, keepVisible: true);

        Assert.Equal(1, r.PlayerCount);
        Assert.Equal(0, r.ShowEventCount);   // 被门控
        Assert.Equal(1, r.StruckCount);
    }

    [Fact]
    public void MapMagicRoundSpringsAddsSecondShowEvent()
    {
        uint runTick = 0;

        var r = CustomMagicEffectEventCore.RunMapMagicRound(
            envirNull: false, now: 3000, ref runTick, attackTime: 3,
            raceServers: new List<int> { 0 }, eventType: 122, keepVisible: true);

        // 主发送被门控，但泉水补发不受门控 → 仍是 1 条
        Assert.Equal(1, r.ShowEventCount);
    }

    [Fact]
    public void MapMagicRoundSpringsBothWhenVisible()
    {
        uint runTick = 0;

        var r = CustomMagicEffectEventCore.RunMapMagicRound(
            envirNull: false, now: 3000, ref runTick, attackTime: 3,
            raceServers: new List<int> { 0 }, eventType: 123, keepVisible: false);

        // 主发送 + 泉水补发 = 两条
        Assert.Equal(2, r.ShowEventCount);
    }

    [Fact]
    public void MapMagicRoundNonSpringsNoExtra()
    {
        uint runTick = 0;

        var r = CustomMagicEffectEventCore.RunMapMagicRound(
            envirNull: false, now: 3000, ref runTick, attackTime: 3,
            raceServers: new List<int> { 0 }, eventType: 4, keepVisible: false);

        Assert.Equal(1, r.ShowEventCount);
    }
}
