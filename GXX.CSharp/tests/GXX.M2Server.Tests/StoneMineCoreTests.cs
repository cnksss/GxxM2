using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J129：挖矿（石矿事件）机制 —— `TStoneMineEvent` / `TPileStones` / `AddToMapMineEvent`
/// 与消费端两份副本（ObjPlayer.pas 12290-12367 与 18537-18620+）1:1 测试。
/// </summary>
public sealed class StoneMineCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(StoneMineCore.ConstantsMatchSource());
        Assert.Equal(11, StoneMineCore.EtStoneMine);
        Assert.Equal(3, StoneMineCore.EtPileStones);
        Assert.Equal(20007, StoneMineCore.RmHeavyHit);
        Assert.Equal(4, StoneMineCore.DefaultMakeMineHitRate);
        Assert.Equal(12, StoneMineCore.DefaultMakeMineRate);
    }

    [Fact]
    public void TimerConstants()
    {
        Assert.Equal(600_000u, StoneMineCore.ReplenishIntervalMs);
        Assert.Equal(300_000, StoneMineCore.PileStonesLifetimeMs);
        Assert.Equal(200, StoneMineCore.MineCountModulus);
        Assert.Equal(80, StoneMineCore.AddStoneCountModulus);
    }

    [Fact]
    public void MeDoMineIsLastEnumValue()
    {
        Assert.True(StoneMineCore.MeDoMineIsLastEnumValue());
        Assert.Equal(8, StoneMineCore.MeDoMineOrdinal);
    }

    [Fact]
    public void ConfigKeysAreSetupSection()
    {
        Assert.True(StoneMineCore.ConfigKeysAreSetupSection());
        Assert.Equal("Setup", StoneMineCore.ConfigSection);
        Assert.Equal("MakeMineHitRate", StoneMineCore.HitRateConfigKey);
        Assert.Equal("MakeMineRate", StoneMineCore.MineRateConfigKey);
    }

    [Fact]
    public void TwoDifferentLockIndices()
    {
        Assert.True(StoneMineCore.TwoDifferentLockIndices());
        Assert.Equal(4, StoneMineCore.ConsumerLockIndex);
        Assert.Equal(28, StoneMineCore.MapAddLockIndex);
    }

    [Fact]
    public void EtStoneMineCollidesWithHolyCurtain2()
    {
        // J126 已记录 ET_STONEMINE 与 ET_HOLYCURTAIN2 同为 11
        Assert.Equal(11, StoneMineCore.EtStoneMine);
    }

    // ===================== 一、两份副本的差异 =====================

    [Fact]
    public void TwoCopiesDifferInOneCall()
    {
        Assert.True(StoneMineCore.TwoCopiesDifferInOneCall());
        Assert.True(StoneMineCore.OnlyDifferenceIsMapNotify());
    }

    [Fact]
    public void NotNestedForm()
    {
        // 副本 A 是独立方法、副本 B 是嵌套函数
        Assert.True(StoneMineCore.NotNestedForm());
        Assert.Contains("独立方法", StoneMineCore.CopyADescription);
        Assert.Contains("嵌套函数", StoneMineCore.CopyBDescription);
    }

    [Fact]
    public void MapNotifyMissingFromOneCopy()
    {
        // **副本 A 缺少 OnMapNotifyEvent(Self, meDoMine)**
        Assert.True(StoneMineCore.MapNotifyMissingFromOneCopy());
        Assert.True(StoneMineCore.CopyBHasMapNotify());
        Assert.True(StoneMineCore.CopyALacksMapNotify());
    }

    [Fact]
    public void MapNotifyLineNumber()
    {
        Assert.Equal(18602, StoneMineCore.MapNotifyLine);
    }

    [Fact]
    public void CosmeticDifferencesOnly()
    {
        Assert.True(StoneMineCore.CosmeticDifferencesOnly());
        Assert.True(StoneMineCore.TwoCosmeticDifferences());
        Assert.Equal(2, StoneMineCore.CosmeticDiff.Length);
    }

    [Fact]
    public void BothCopiesAreLarge()
    {
        Assert.True(StoneMineCore.BothCopiesAreLarge());
        Assert.Equal(2, StoneMineCore.CopyRanges.Length);
        Assert.Equal(12290, StoneMineCore.CopyRanges[0].StartLine);
        Assert.Equal(18537, StoneMineCore.CopyRanges[1].StartLine);
    }

    // ===================== 二、TStoneMineEvent.Create =====================

    [Fact]
    public void SixCtorInitializers()
    {
        Assert.True(StoneMineCore.SixCtorInitializers());
        Assert.True(StoneMineCore.FourDistinctDefaultStrategies());
    }

    [Fact]
    public void CtorDefaultTable()
    {
        var t = StoneMineCore.CtorDefaults;

        Assert.Equal("m_boVisible", t[0].Field);
        Assert.Contains("冗余", t[0].Strategy);
        Assert.Equal("m_nMineCount", t[1].Field);
        Assert.Contains("Random(200)", t[1].Initial);
        Assert.Equal("m_dwAddStoneMineTick", t[2].Field);
        Assert.Contains("当前时刻", t[2].Strategy);
        Assert.Equal("m_nAddStoneCount", t[4].Field);
        Assert.Contains("Random(80)", t[4].Initial);
        Assert.Equal("m_boAllowClose", t[5].Field);
    }

    [Fact]
    public void MineCountBounds()
    {
        Assert.True(StoneMineCore.MineCountBounds());
        Assert.Equal(0, StoneMineCore.MineCountValue(0));
        Assert.Equal(199, StoneMineCore.MineCountValue(199));
    }

    [Fact]
    public void AddStoneCountBounds()
    {
        Assert.True(StoneMineCore.AddStoneCountBounds());
        Assert.Equal(0, StoneMineCore.AddStoneCountValue(0));
        Assert.Equal(79, StoneMineCore.AddStoneCountValue(79));
    }

    [Fact]
    public void TwoDifferentModuli()
    {
        Assert.True(StoneMineCore.TwoDifferentModuli());
    }

    [Fact]
    public void ReplenishCanBeLessThanInitial()
    {
        // **补充上限 79 < 初始上限 199 → 补充后可能比初始更少**
        Assert.True(StoneMineCore.ReplenishCanBeLessThanInitial());
    }

    [Fact]
    public void VisibleFalseIsRedundant()
    {
        Assert.True(StoneMineCore.VisibleFalseIsRedundant());
    }

    [Fact]
    public void InheritedArgsIncludeZeroTimeAndFalseVisible()
    {
        Assert.Equal(6, StoneMineCore.InheritedArgs.Length);
        Assert.Equal("0", StoneMineCore.InheritedArgs[4].Value);
        Assert.Equal("False", StoneMineCore.InheritedArgs[5].Value);
    }

    [Fact]
    public void NeverExpiresNaturally()
    {
        Assert.True(StoneMineCore.NeverExpiresNaturally());
        Assert.False(StoneMineCore.ShouldExpireNaturally(false));
        Assert.True(StoneMineCore.ShouldExpireNaturally(true));
    }

    [Fact]
    public void CommentedSelfRegistration()
    {
        Assert.True(StoneMineCore.CommentedSelfRegistration());
        Assert.Contains("AddToMapMineEvent", StoneMineCore.CommentedLine);
    }

    [Fact]
    public void ActiveAlwaysFalseAtCtor()
    {
        Assert.True(StoneMineCore.ActiveAlwaysFalseAtCtor());
    }

    // ===================== 三、AddToMapMineEvent =====================

    [Fact]
    public void ResultIsNilOnFailure()
    {
        Assert.True(StoneMineCore.ResultIsNilOnFailure());
        Assert.True(StoneMineCore.SuccessComparisonsEqualToEvent());
    }

    [Fact]
    public void AddSucceededSemantics()
    {
        var e = new object();

        Assert.True(StoneMineCore.AddSucceeded(e, e));
        Assert.False(StoneMineCore.AddSucceeded(null, e));
        Assert.True(StoneMineCore.BareNilIsFailure());
        Assert.True(StoneMineCore.DifferentObjectIsFailure());
    }

    [Fact]
    public void InvalidMapExitsEarly()
    {
        Assert.True(StoneMineCore.InvalidMapExitsEarly());
    }

    [Fact]
    public void AddGateAllConditions()
    {
        Assert.True(StoneMineCore.RequiresValidCellWithNonZeroFlag());
        Assert.True(StoneMineCore.AllConditionsPass());
        Assert.True(StoneMineCore.ZeroFlagCellRejects());
        Assert.True(StoneMineCore.InvalidMapRejects());
        Assert.True(StoneMineCore.CellNotFoundRejects());
    }

    [Fact]
    public void AddGateTruthTable()
    {
        Assert.True(StoneMineCore.AddGate(false, true, 1));
        Assert.False(StoneMineCore.AddGate(true, true, 1));
        Assert.False(StoneMineCore.AddGate(false, false, 1));
        Assert.False(StoneMineCore.AddGate(false, true, 0));
    }

    [Fact]
    public void GetMapCellInfoShortCircuits()
    {
        Assert.True(StoneMineCore.GetMapCellInfoShortCircuits());
    }

    [Fact]
    public void TwoStorageModes()
    {
        Assert.True(StoneMineCore.TwoStorageModes());
        Assert.Equal("TSafeList", StoneMineCore.StorageMode(false));
        Assert.Equal("动态数组 SetLength", StoneMineCore.StorageMode(true));
    }

    [Fact]
    public void LazyListCreation()
    {
        Assert.True(StoneMineCore.LazyListCreation());
        Assert.True(StoneMineCore.CreatesListWhenNull(true));
        Assert.False(StoneMineCore.CreatesListWhenNull(false));
    }

    [Fact]
    public void NilListCheckNeededForListMode()
    {
        Assert.True(StoneMineCore.NilListCheckNeededForListMode());
    }

    [Fact]
    public void ExceptionLooksLikeFailure()
    {
        // 异常与门不满足对调用方是同一结果（nil）
        Assert.True(StoneMineCore.ExceptionLooksLikeFailure());
        Assert.True(StoneMineCore.ExceptionKeepsNil());
        Assert.Equal(0, StoneMineCore.AddResultCount(true, true));
        Assert.Equal(1, StoneMineCore.AddResultCount(true, false));
    }

    [Fact]
    public void ExceptionMsgConstant()
    {
        Assert.True(StoneMineCore.ExceptionMsgConstant());
        Assert.Equal("[Exception] TEnvirnoment.AddToMapMineEvent ", StoneMineCore.AddToMapExceptionMsg);
    }

    [Fact]
    public void ThreadLockUsesIndex28()
    {
        Assert.True(StoneMineCore.ThreadLockUsesIndex28());
    }

    // ===================== 四、消费端流程 =====================

    [Fact]
    public void FirstGate()
    {
        Assert.True(StoneMineCore.FirstGate(true, true));
        Assert.False(StoneMineCore.FirstGate(true, false));
        Assert.False(StoneMineCore.FirstGate(false, true));
    }

    [Fact]
    public void TwoDifferentMapFlags()
    {
        Assert.True(StoneMineCore.TwoDifferentMapFlags());
        Assert.True(StoneMineCore.MineFlagIsPerMap());
        Assert.Equal(2, StoneMineCore.TwoMapFlags.Length);
        Assert.Contains("m_boMINE", StoneMineCore.TwoMapFlags[0]);
        Assert.Contains("m_boInvalid", StoneMineCore.TwoMapFlags[1]);
    }

    [Fact]
    public void NoBreakInSearchLoop()
    {
        Assert.True(StoneMineCore.NoBreakInSearchLoop());
        Assert.True(StoneMineCore.LastMatchWins());
    }

    [Fact]
    public void LastMatchWinsVerified()
    {
        // 不 Break → 多个匹配取最后一个
        Assert.True(StoneMineCore.LastMatchWinsVerified());
        Assert.Equal(2, StoneMineCore.FindLastEventMatch(new[] { true, false, true }));
        Assert.Equal(0, StoneMineCore.FindLastEventMatch(new[] { true, false, false }));
        Assert.True(StoneMineCore.NoMatchReturnsMinusOne());
    }

    [Fact]
    public void DynamicCreationGatedOnBo10()
    {
        Assert.True(StoneMineCore.DynamicCreationGatedOnBo10());
        Assert.True(StoneMineCore.ExistingEventSkipsCreation());
        Assert.True(StoneMineCore.FreeOnAddFailure());
        Assert.True(StoneMineCore.AddEventFalseMeansMineList());
    }

    [Fact]
    public void SecondGateTruthTable()
    {
        Assert.True(StoneMineCore.SecondGate(true, true, 1));
        Assert.False(StoneMineCore.SecondGate(false, true, 1));
        Assert.False(StoneMineCore.SecondGate(true, false, 1));
        Assert.False(StoneMineCore.SecondGate(true, true, 0));
    }

    [Fact]
    public void ThirdGate()
    {
        Assert.True(StoneMineCore.ThirdGate(true, StoneMineCore.EtStoneMine));
        Assert.True(StoneMineCore.RightTypePasses());
        Assert.True(StoneMineCore.WrongTypeSkips());
        Assert.False(StoneMineCore.ThirdGate(false, StoneMineCore.EtStoneMine));
    }

    // ===================== 五、两个分支 =====================

    [Fact]
    public void BranchSelection()
    {
        Assert.True(StoneMineCore.GoMining(1));
        Assert.True(StoneMineCore.GoMining(199));
        Assert.True(StoneMineCore.ZeroCountGoesToReplenish());
        Assert.True(StoneMineCore.NegativeCountGoesToReplenish());
    }

    [Fact]
    public void DecrementHappensBeforeHitRoll()
    {
        Assert.True(StoneMineCore.DecrementHappensBeforeHitRoll());
        Assert.True(StoneMineCore.UnhitStillDecrements());
    }

    [Fact]
    public void MineStepBoundaries()
    {
        var (c1, h1) = StoneMineCore.MineStep(5, true);
        Assert.Equal(4, c1);
        Assert.True(h1);

        var (c2, h2) = StoneMineCore.MineStep(5, false);
        Assert.Equal(4, c2);
        Assert.False(h2);

        var (c3, h3) = StoneMineCore.MineStep(0, true);
        Assert.Equal(0, c3);
        Assert.False(h3);   // 采空时不消耗
    }

    [Fact]
    public void HitRateIsDenominator()
    {
        Assert.True(StoneMineCore.HitRateIsDenominator());
        Assert.True(StoneMineCore.DefaultHitRateIsFour());
        Assert.Equal("1/4", StoneMineCore.HitChanceDescription(4));
    }

    [Fact]
    public void HitRollSemantics()
    {
        Assert.True(StoneMineCore.HitRoll(4, 0));
        Assert.False(StoneMineCore.HitRoll(4, 1));
    }

    [Fact]
    public void UsesCurrCoordsNotParam()
    {
        // 用 m_nCurrX/m_nCurrY 而非传入的 nX/nY
        Assert.True(StoneMineCore.UsesCurrCoordsNotParam());
        Assert.True(StoneMineCore.PileQueryUsesCurrentCoords());
        Assert.Equal((10, 20), StoneMineCore.PileQueryCoords(10, 20));
    }

    [Fact]
    public void PileStonesLifetimeIsFiveMinutes()
    {
        Assert.True(StoneMineCore.PileStonesLifetimeIsFiveMinutes());
    }

    [Fact]
    public void ExistingPileOnlyIncrements()
    {
        Assert.True(StoneMineCore.ExistingPileOnlyIncrements());
        Assert.True(StoneMineCore.PileParamStartsAtOne());
        Assert.True(StoneMineCore.PileParamCapIsFive());
    }

    [Fact]
    public void AddEventParamIncrementsUpToFive()
    {
        Assert.True(StoneMineCore.PileParamIncrementsUpToFive());
        Assert.Equal(2, StoneMineCore.AddEventParam(1));
        Assert.Equal(5, StoneMineCore.AddEventParam(4));
        Assert.Equal(5, StoneMineCore.AddEventParam(5));
    }

    [Fact]
    public void PileParamAboveCapUnchanged()
    {
        // 超上限者**原样返回**（与 J126 的 AddEventParam 同型）
        Assert.True(StoneMineCore.PileParamAboveCapUnchanged());
        Assert.Equal(99, StoneMineCore.AddEventParam(99));
    }

    [Fact]
    public void TwoSequentialRolls()
    {
        Assert.True(StoneMineCore.TwoSequentialRolls());
        Assert.True(StoneMineCore.DefaultMineRateIsTwelve());
        Assert.True(StoneMineCore.CombinedProbabilityIsOneInFortyEight());
        Assert.Equal("1/48", StoneMineCore.CombinedProbability(4, 12));
    }

    [Fact]
    public void BothRollsRequired()
    {
        Assert.True(StoneMineCore.BothRollsRequired());
        Assert.True(StoneMineCore.MakesMine(0, 0));
        Assert.False(StoneMineCore.MakesMine(0, 1));
        Assert.False(StoneMineCore.MakesMine(1, 0));
    }

    [Fact]
    public void WeaponDamageRange()
    {
        Assert.True(StoneMineCore.WeaponDamageRange());
        Assert.Equal(5, StoneMineCore.WeaponDamage(0));
        Assert.Equal(19, StoneMineCore.WeaponDamage(14));
        Assert.True(StoneMineCore.FifteenWeaponDamageValues());
    }

    [Fact]
    public void ReplenishGating()
    {
        Assert.True(StoneMineCore.ReplenishOnlyWhenDepleted());
        Assert.True(StoneMineCore.ReplenishIntervalTenMinutes());
    }

    [Fact]
    public void ReplenishBoundaries()
    {
        // 严格 `>`：恰好 10 分钟不补充
        Assert.True(StoneMineCore.ReplenishBoundaries());
        Assert.False(StoneMineCore.ReplenishDue(600_000, 0));
        Assert.True(StoneMineCore.ReplenishDue(600_001, 0));
    }

    [Fact]
    public void ReplenishIsIdempotent()
    {
        Assert.True(StoneMineCore.ReplenishIsIdempotent());
        Assert.True(StoneMineCore.AddStoneCountFrozenAtCtor());
        Assert.True(StoneMineCore.ReplenishTwiceSameCount());
    }

    [Fact]
    public void AddStoneMineAssignsBothFields()
    {
        var (count, tick) = StoneMineCore.AddStoneMine(37, 12345);

        Assert.Equal(37, count);
        Assert.Equal(12345u, tick);
        Assert.True(StoneMineCore.ReplenishDoesNotReactivate());
    }

    // ===================== 六、发送与返回 =====================

    [Fact]
    public void AlwaysSendsHeavyHit()
    {
        Assert.True(StoneMineCore.AlwaysSendsHeavyHit());
        Assert.True(StoneMineCore.SendsHeavyHit(true));
        Assert.True(StoneMineCore.SendsHeavyHit(false));
    }

    [Fact]
    public void HitSignalledViaStringParam()
    {
        Assert.True(StoneMineCore.HitSignalledViaStringParam());
        Assert.True(StoneMineCore.HitParamValues());
        Assert.True(StoneMineCore.HitParamInitialIsEmpty());
    }

    [Fact]
    public void ResultIsHitFlag()
    {
        Assert.True(StoneMineCore.ReplenishPathReturnsFalse());
        Assert.True(StoneMineCore.ResultIsHitFlag(true));
        Assert.False(StoneMineCore.ResultIsHitFlag(false));
        Assert.True(StoneMineCore.MiningWithoutHitReturnsFalse());
    }

    [Fact]
    public void HeavyHitConstant()
    {
        Assert.True(StoneMineCore.HeavyHitConstant());
    }

    [Fact]
    public void ConsumerLockIndexFour()
    {
        Assert.True(StoneMineCore.ConsumerLockIndexFour());
    }

    // ===================== 七、顶层仿真 =====================

    [Fact]
    public void TryMineFirstGateFails()
    {
        var r = StoneMineCore.TryMine(
            "A", boMine: false, cellFound: true,
            existingEventFound: false, existingEventType: 0,
            chFlag: 1, mineCount: 10, addStoneCount: 20, addTick: 0,
            now: 0, hitRate: 4, mineRate: 12, hitRoll: 0, mineRoll: 0, weaponRoll: 0);

        Assert.False(r.EventCreated);
        Assert.False(r.WentMining);
        Assert.False(r.Result);
        Assert.Equal("", r.HitParam);
    }

    [Fact]
    public void TryMineCreatesEventThenMines()
    {
        var r = StoneMineCore.TryMine(
            "A", boMine: true, cellFound: true,
            existingEventFound: false, existingEventType: 0,
            chFlag: 1, mineCount: 10, addStoneCount: 20, addTick: 0,
            now: 0, hitRate: 4, mineRate: 12, hitRoll: 0, mineRoll: 0, weaponRoll: 3);

        Assert.True(r.EventCreated);
        Assert.True(r.AddSucceeded);
        Assert.True(r.WentMining);
        Assert.True(r.Hit);
        Assert.True(r.MadeMine);
        Assert.True(r.PileTouched);
        Assert.Equal(9, r.RemainingCount);
        Assert.Equal(8, r.WeaponDamage);
        Assert.Equal("1", r.HitParam);
        Assert.True(r.Result);
    }

    [Fact]
    public void TryMineHitButNoOre()
    {
        var r = StoneMineCore.TryMine(
            "A", boMine: true, cellFound: true,
            existingEventFound: false, existingEventType: 0,
            chFlag: 1, mineCount: 10, addStoneCount: 20, addTick: 0,
            now: 0, hitRate: 4, mineRate: 12, hitRoll: 0, mineRoll: 5, weaponRoll: 0);

        Assert.True(r.Hit);
        Assert.False(r.MadeMine);      // 第二道未过
        Assert.True(r.PileTouched);    // 但矿石堆仍出现
        Assert.True(r.Result);
    }

    [Fact]
    public void TryMineUnhitStillDecrements()
    {
        var r = StoneMineCore.TryMine(
            "A", boMine: true, cellFound: true,
            existingEventFound: false, existingEventType: 0,
            chFlag: 1, mineCount: 10, addStoneCount: 20, addTick: 0,
            now: 0, hitRate: 4, mineRate: 12, hitRoll: 3, mineRoll: 0, weaponRoll: 0);

        Assert.False(r.Hit);
        Assert.Equal(9, r.RemainingCount);   // 未命中但已消耗
        Assert.Equal(0, r.WeaponDamage);     // 未命中不损耗
        Assert.False(r.Result);
    }

    [Fact]
    public void TryMineReplenishPath()
    {
        var r = StoneMineCore.TryMine(
            "A", boMine: true, cellFound: true,
            existingEventFound: true, existingEventType: StoneMineCore.EtStoneMine,
            chFlag: 1, mineCount: 0, addStoneCount: 37, addTick: 0,
            now: 600_001, hitRate: 4, mineRate: 12, hitRoll: 0, mineRoll: 0, weaponRoll: 0);

        Assert.True(r.WentReplenish);
        Assert.False(r.WentMining);
        Assert.Equal(37, r.RemainingCount);
        Assert.False(r.Result);              // **补充分支返回假**
        Assert.Equal("", r.HitParam);
    }

    [Fact]
    public void TryMineReplenishNotDue()
    {
        var r = StoneMineCore.TryMine(
            "A", boMine: true, cellFound: true,
            existingEventFound: true, existingEventType: StoneMineCore.EtStoneMine,
            chFlag: 1, mineCount: 0, addStoneCount: 37, addTick: 0,
            now: 600_000, hitRate: 4, mineRate: 12, hitRoll: 0, mineRoll: 0, weaponRoll: 0);

        Assert.True(r.WentReplenish);
        Assert.Equal(0, r.RemainingCount);   // 未到 10 分钟
    }

    [Fact]
    public void CopyBDoesSendMapNotify()
    {
        var r = StoneMineCore.TryMine(
            "B", boMine: true, cellFound: true,
            existingEventFound: true, existingEventType: StoneMineCore.EtStoneMine,
            chFlag: 1, mineCount: 10, addStoneCount: 20, addTick: 0,
            now: 0, hitRate: 4, mineRate: 12, hitRoll: 0, mineRoll: 0, weaponRoll: 0);

        Assert.True(r.SentMapNotify);
    }

    [Fact]
    public void CopyADoesNotSendMapNotify()
    {
        var r = StoneMineCore.TryMine(
            "A", boMine: true, cellFound: true,
            existingEventFound: true, existingEventType: StoneMineCore.EtStoneMine,
            chFlag: 1, mineCount: 10, addStoneCount: 20, addTick: 0,
            now: 0, hitRate: 4, mineRate: 12, hitRoll: 0, mineRoll: 0, weaponRoll: 0);

        // **两份副本唯一的语义差异**
        Assert.False(r.SentMapNotify);
        Assert.True(r.Hit);
    }

    [Fact]
    public void TryMineWrongEventTypeSkipped()
    {
        var r = StoneMineCore.TryMine(
            "A", boMine: true, cellFound: true,
            existingEventFound: true, existingEventType: StoneMineCore.EtPileStones,
            chFlag: 1, mineCount: 10, addStoneCount: 20, addTick: 0,
            now: 0, hitRate: 4, mineRate: 12, hitRoll: 0, mineRoll: 0, weaponRoll: 0);

        Assert.False(r.WentMining);
        Assert.False(r.WentReplenish);
        Assert.False(r.Result);
    }

    [Fact]
    public void TryMineZeroFlagNoCreation()
    {
        var r = StoneMineCore.TryMine(
            "A", boMine: true, cellFound: true,
            existingEventFound: false, existingEventType: 0,
            chFlag: 0, mineCount: 10, addStoneCount: 20, addTick: 0,
            now: 0, hitRate: 4, mineRate: 12, hitRoll: 0, mineRoll: 0, weaponRoll: 0);

        Assert.False(r.EventCreated);   // 第二层门被 chFlag 挡住
        Assert.False(r.Result);
    }
}
