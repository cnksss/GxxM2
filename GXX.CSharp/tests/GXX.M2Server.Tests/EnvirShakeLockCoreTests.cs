using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J163：`TEnvirnoment` 场景抖动与锁封装族 1:1 测试。
/// **占位符与调用次数全部由程序化统计写入（并做两处独立计数互证）、
/// 节流与回收边界用显式 now 参数穷举、两套锁机制的差异用模型验证。**
/// </summary>
public sealed class EnvirShakeLockCoreTests
{
    // ===================== 常量与分类 =====================

    [Fact]
    public void MethodCategories()
    {
        Assert.True(EnvirShakeLockCore.MethodCategoriesCount());
        Assert.Equal(4, EnvirShakeLockCore.MethodCategories.Length);
        Assert.True(EnvirShakeLockCore.NoTryFinallyAnywhereInBatch());
    }

    [Fact]
    public void ShakeInterval()
    {
        Assert.Equal(320, EnvirShakeLockCore.ShakeIntervalMs);
        Assert.True(EnvirShakeLockCore.ThrottleConditionIs320());
    }

    // ===================== 一、AddSceneShake =====================

    [Fact]
    public void ZeroIsSentinel()
    {
        // **零次抖动不建记录**
        Assert.True(EnvirShakeLockCore.ZeroIsSentinel());
        Assert.True(EnvirShakeLockCore.OnlyZeroRejected());
        Assert.False(EnvirShakeLockCore.ShouldCreateRecord(0));
        Assert.True(EnvirShakeLockCore.ShouldCreateRecord(1));
    }

    [Fact]
    public void NegativeAcceptedThenReclaimed()
    {
        // **负数照常建记录，但下一帧就被回收**
        Assert.True(EnvirShakeLockCore.NegativeAccepted());
        Assert.True(EnvirShakeLockCore.NegativeAcceptedThenImmediatelyReclaimed());
        Assert.True(EnvirShakeLockCore.ShouldCreateRecord(-1));
        Assert.True(EnvirShakeLockCore.ShouldCreateRecord(-5));
    }

    [Fact]
    public void ReclaimCondition()
    {
        // **初值 CurCount = 0，Count = -1 → 0 >= -1 成立、立刻回收**
        Assert.True(EnvirShakeLockCore.NegativeReclaimedImmediately());
        Assert.True(EnvirShakeLockCore.ReclaimCondition(0, -1));
        Assert.False(EnvirShakeLockCore.ReclaimCondition(0, 1));
    }

    [Fact]
    public void PositiveNotImmediatelyReclaimed()
    {
        Assert.True(EnvirShakeLockCore.PositiveNotImmediatelyReclaimed());
        Assert.False(EnvirShakeLockCore.ReclaimCondition(0, 100));
    }

    [Fact]
    public void ZeroNeverEntersList()
    {
        Assert.True(EnvirShakeLockCore.ZeroNeverEntersList());
    }

    // ---------- 五个字段初值 ----------

    [Fact]
    public void InitialValues()
    {
        Assert.True(EnvirShakeLockCore.LastTickInitialisedToZero());
        Assert.True(EnvirShakeLockCore.TwoZeroInitialValues());
        Assert.Equal(0, EnvirShakeLockCore.InitialLastTick());
        Assert.Equal(0, EnvirShakeLockCore.InitialCurCount());
    }

    [Fact]
    public void RecordFields()
    {
        Assert.True(EnvirShakeLockCore.FiveRecordFields());
        Assert.Equal(5, EnvirShakeLockCore.RecordFields.Length);
        Assert.Equal(
            new[] { "LastTick", "Count", "CurCount", "PlayerName", "EnableClientOption" },
            EnvirShakeLockCore.RecordFields);
    }

    // ---------- 节流 ----------

    [Fact]
    public void ThrottleBoundary()
    {
        // **初值零：开机超过 320 毫秒时首帧即处理**
        Assert.True(EnvirShakeLockCore.ZeroInitialTickMeansImmediateFirstRun());
        Assert.False(EnvirShakeLockCore.ShouldSkipByThrottle(320, 0));
        Assert.False(EnvirShakeLockCore.ShouldSkipByThrottle(100000, 0));
    }

    [Fact]
    public void BootWindowDelays()
    {
        // **开机不足 320 毫秒时会被推迟**
        Assert.True(EnvirShakeLockCore.BootWindowUnder320MsDelays());
        Assert.True(EnvirShakeLockCore.ShouldSkipByThrottle(0, 0));
        Assert.True(EnvirShakeLockCore.ShouldSkipByThrottle(319, 0));
        Assert.True(EnvirShakeLockCore.BootWindowBoundary());
    }

    [Fact]
    public void ThrottleIsStrictLessThan()
    {
        // **严格小于：恰好 320 不跳过**
        Assert.True(EnvirShakeLockCore.ShouldSkipByThrottle(319, 0));
        Assert.False(EnvirShakeLockCore.ShouldSkipByThrottle(320, 0));
    }

    // ---------- 名字近义 ----------

    [Fact]
    public void CountVersusCurCount()
    {
        Assert.True(EnvirShakeLockCore.CountVersusCurCountNaming());
        Assert.True(EnvirShakeLockCore.ThreeLetterDifference());
        Assert.True(EnvirShakeLockCore.TwoCountFieldNames());
        Assert.Equal(new[] { "Count", "CurCount" }, EnvirShakeLockCore.CountFieldNames);
    }

    // ---------- 两套锁 ----------

    [Fact]
    public void TwoLockMechanisms()
    {
        Assert.True(EnvirShakeLockCore.TwoLockMechanismsCoexist());
        Assert.True(EnvirShakeLockCore.TwoLockMechanismCount());
        Assert.Equal(2, EnvirShakeLockCore.LockMechanisms.Length);
    }

    [Fact]
    public void TGListLock()
    {
        Assert.True(EnvirShakeLockCore.ShakeUsesTGListLock());
        Assert.True(EnvirShakeLockCore.TGListLockHasNoId());
        Assert.True(EnvirShakeLockCore.TGListLockHasNoReadWriteSplit());
        Assert.True(EnvirShakeLockCore.TGListLockIsEnterCriticalSection());
        Assert.True(EnvirShakeLockCore.TwoTGListLockCalls());
        Assert.Equal(new[] { "EnterCriticalSection", "LeaveCriticalSection" }, EnvirShakeLockCore.TGListLockCalls);
    }

    // ---------- 异常保护 ----------

    [Fact]
    public void AllocFreePairing()
    {
        Assert.True(EnvirShakeLockCore.NoExceptionGuardAroundNew());
        Assert.True(EnvirShakeLockCore.DisposePairsWithNew());
        Assert.True(EnvirShakeLockCore.TwoAllocFreePrimitives());
        Assert.Equal(new[] { "New(Result)", "Dispose(Info)" }, EnvirShakeLockCore.AllocFreePrimitives);
    }

    // ===================== 二、ClearSceneShakeList =====================

    [Fact]
    public void ForwardDisposeThenClear()
    {
        Assert.True(EnvirShakeLockCore.ForwardDisposeThenClear());
        Assert.True(EnvirShakeLockCore.ForwardVersusReverseJustified());
        Assert.True(EnvirShakeLockCore.TwoIterationReasons());
        Assert.Equal(2, EnvirShakeLockCore.IterationDirectionReason.Length);
    }

    [Fact]
    public void NoTryFinallyInClear()
    {
        Assert.True(EnvirShakeLockCore.NoTryFinallyInClear());
        Assert.True(EnvirShakeLockCore.DisposesAllElements());
        Assert.True(EnvirShakeLockCore.MustBeLastUser());
    }

    [Fact]
    public void DisposeCount()
    {
        Assert.True(EnvirShakeLockCore.DisposeCountValues());
        Assert.Equal(0, EnvirShakeLockCore.DisposeCount(0));
        Assert.Equal(3, EnvirShakeLockCore.DisposeCount(3));
        Assert.Equal(7, EnvirShakeLockCore.DisposeCount(7));
    }

    // ---------- 两条回收路径 ----------

    [Fact]
    public void TwoReclaimPaths()
    {
        Assert.True(EnvirShakeLockCore.TwoReclaimPaths());
        Assert.True(EnvirShakeLockCore.TwoReclaimPathCount());
        Assert.Equal(2, EnvirShakeLockCore.ReclaimPaths.Length);
    }

    [Fact]
    public void GlobalShake()
    {
        // **空玩家名是全局抖动、不会被"玩家离开"回收**
        Assert.True(EnvirShakeLockCore.EmptyPlayerNameIsGlobalShake());
        Assert.True(EnvirShakeLockCore.GlobalShakeNotReclaimedByPlayerLeave());
        Assert.True(EnvirShakeLockCore.IsGlobalShake(""));
        Assert.False(EnvirShakeLockCore.IsGlobalShake("张三"));
    }

    [Fact]
    public void ReclaimByPlayerLeave()
    {
        Assert.True(EnvirShakeLockCore.ReclaimByPlayerLeaveValues());
        Assert.False(EnvirShakeLockCore.ShouldReclaimByPlayerLeave("", false, false));
        Assert.False(EnvirShakeLockCore.ShouldReclaimByPlayerLeave("张三", true, true));
        Assert.True(EnvirShakeLockCore.ShouldReclaimByPlayerLeave("张三", false, false));
        Assert.True(EnvirShakeLockCore.ShouldReclaimByPlayerLeave("张三", true, false));
    }

    [Fact]
    public void IsAllShake()
    {
        // **是置真而不是赋值**
        Assert.True(EnvirShakeLockCore.IsAllShakeSetNotAssigned());
        Assert.True(EnvirShakeLockCore.IsAllShakeSemantics());
    }

    [Fact]
    public void EnableClientOptionLastWins()
    {
        Assert.True(EnvirShakeLockCore.EnableClientOptionLastWins());
        Assert.True(EnvirShakeLockCore.EnableClientOptionOverwrite());
    }

    // ===================== 三、四个锁封装 =====================

    [Fact]
    public void FourPureForwards()
    {
        Assert.True(EnvirShakeLockCore.FourPureForwards());
        Assert.True(EnvirShakeLockCore.FourForwardEntries());
        Assert.Equal(4, EnvirShakeLockCore.ForwardMap.Length);
    }

    [Fact]
    public void ForwardMapContents()
    {
        Assert.Equal(new[] { "LockR(LockID)", "FCriticalSection.LockR(LockID)" }, EnvirShakeLockCore.ForwardMap[0]);
        Assert.Equal(new[] { "UnLockR", "FCriticalSection.UnLockR" }, EnvirShakeLockCore.ForwardMap[1]);
        Assert.Equal(new[] { "LockW(LockID)", "FCriticalSection.LockW(LockID)" }, EnvirShakeLockCore.ForwardMap[2]);
        Assert.Equal(new[] { "UnLockW", "FCriticalSection.UnLockW" }, EnvirShakeLockCore.ForwardMap[3]);
    }

    [Fact]
    public void AsymmetricSignature()
    {
        // **加锁带编号、解锁不带**
        Assert.True(EnvirShakeLockCore.AsymmetricSignature());
        Assert.True(EnvirShakeLockCore.UnlockTakesNoId());
        Assert.Equal(1, EnvirShakeLockCore.LockParamCount());
        Assert.Equal(0, EnvirShakeLockCore.UnlockParamCount());
    }

    [Fact]
    public void NoNestedSameTypeLock()
    {
        Assert.True(EnvirShakeLockCore.ImpliesCurrentIdState());
        Assert.True(EnvirShakeLockCore.NoNestedSameTypeLock());
        Assert.True(EnvirShakeLockCore.PerCellLockingIsSequentialNotNested());
        Assert.True(EnvirShakeLockCore.SequentialLockModel());
        Assert.True(EnvirShakeLockCore.NestedLockModelImbalanced());
    }

    // ---------- Invalidity ----------

    [Fact]
    public void Invalidity()
    {
        Assert.True(EnvirShakeLockCore.InvaliditySetsBothFields());
        Assert.True(EnvirShakeLockCore.InvalidityValues());
        var (inv, tick) = EnvirShakeLockCore.Invalidity(12345);
        Assert.True(inv);
        Assert.Equal(12345, tick);
    }

    [Fact]
    public void InvalidFields()
    {
        Assert.True(EnvirShakeLockCore.OnlyWriterOfInvalidFields());
        Assert.True(EnvirShakeLockCore.InvalidityUnlocked());
        Assert.True(EnvirShakeLockCore.TwoInvalidFieldNames());
        Assert.Equal(new[] { "m_boInvalid", "m_dwInvalidTick" }, EnvirShakeLockCore.InvalidFieldNames);
    }

    [Fact]
    public void InvalidReadSites()
    {
        // **至少四处读取**
        Assert.True(EnvirShakeLockCore.InvalidReadAtFourSites());
        Assert.True(EnvirShakeLockCore.FourInvalidReadSites());
        Assert.Equal(new[] { 1638, 1655, 1784, 1996 }, EnvirShakeLockCore.InvalidReadLines);
    }

    [Fact]
    public void CanFlyOppositeDirection()
    {
        // **CanFly 对无效地图的方向与其它检查相反**
        Assert.True(EnvirShakeLockCore.CanFlyOppositeDirection());
        Assert.True(EnvirShakeLockCore.InvalidDirectionComparison());
    }

    // ===================== 四、GetEnvirInfo =====================

    [Fact]
    public void FormatStringParts()
    {
        Assert.True(EnvirShakeLockCore.TwoFormatStringParts());
        Assert.Equal(29, EnvirShakeLockCore.FirstPartPlaceholders());
        Assert.Equal(19, EnvirShakeLockCore.SecondPartPlaceholders());
    }

    [Fact]
    public void ArgCountIs48()
    {
        // **总数 48 —— 我最初肉眼写成 43、被程序化核对抓出**
        Assert.True(EnvirShakeLockCore.ArgCountIs48());
        Assert.True(EnvirShakeLockCore.TwoIndependentCountsAgree());
        Assert.Equal(48, EnvirShakeLockCore.ArgCount());
    }

    [Fact]
    public void BoolToCStrCallCount()
    {
        // **29 —— 我最初肉眼写成 24、被程序化核对抓出**
        Assert.True(EnvirShakeLockCore.TwentyNineBoolCalls());
        Assert.Equal(29, EnvirShakeLockCore.BoolToCStrCallCount());
        Assert.True(EnvirShakeLockCore.BoolPlusNonBoolEqualsTotal());
    }

    [Fact]
    public void MostlyArgumentListNotLogic()
    {
        Assert.True(EnvirShakeLockCore.MostlyArgumentListNotLogic());
        Assert.True(EnvirShakeLockCore.BoolToCStrIsStandaloneFunction());
        Assert.True(EnvirShakeLockCore.ThreeBoolHelpers());
        Assert.Equal(new[] { "BoolToCStr", "BooleanToStr", "BoolToInt" }, EnvirShakeLockCore.BoolHelpers);
    }

    [Fact]
    public void ExpRateIsFloatDivision()
    {
        // **EXPRATE 用浮点除法、占位符是 %f**
        Assert.True(EnvirShakeLockCore.ExpRateIsFloatDivision());
        Assert.True(EnvirShakeLockCore.ExpRatePlaceholderIsF());
        Assert.True(EnvirShakeLockCore.ExpRateValues());
        Assert.True(EnvirShakeLockCore.ExpRateNotTruncated());
    }

    [Fact]
    public void ExpRateValues()
    {
        Assert.Equal(2.5, EnvirShakeLockCore.ExpRateValue(250));
        Assert.Equal(1.0, EnvirShakeLockCore.ExpRateValue(100));
        Assert.Equal(0.0, EnvirShakeLockCore.ExpRateValue(0));
        Assert.Equal(0.5, EnvirShakeLockCore.ExpRateValue(50));
    }

    [Fact]
    public void DefaultPrecisionTwoDecimals()
    {
        Assert.True(EnvirShakeLockCore.DefaultPrecisionTwoDecimals());
        Assert.Equal("2.50", EnvirShakeLockCore.ExpRateValue(250).ToString("F2"));
        Assert.Equal("1.00", EnvirShakeLockCore.ExpRateValue(100).ToString("F2"));
    }

    [Fact]
    public void ToggleFormat()
    {
        Assert.True(EnvirShakeLockCore.ThreeValuesPerToggle());
        Assert.True(EnvirShakeLockCore.ToggleFormatShapePresent());
        Assert.Equal("%s(%d/%d)", EnvirShakeLockCore.ToggleFormatShape);
    }

    [Fact]
    public void DecIncPairs()
    {
        Assert.True(EnvirShakeLockCore.FourDecIncPairs());
        Assert.True(EnvirShakeLockCore.PairsDifferOnlyByDecInc());
        Assert.Equal(4, EnvirShakeLockCore.DecIncFieldNamePairs.Length);
    }

    [Fact]
    public void FightZoneNumbering()
    {
        Assert.True(EnvirShakeLockCore.FightZoneNumberingStartsAtOne());
        Assert.True(EnvirShakeLockCore.ThreeFightZoneLabels());
        Assert.True(EnvirShakeLockCore.ThreeFightZoneFields());
        Assert.Equal(new[] { "FIGHT", "FIGHT3", "FIGHT4" }, EnvirShakeLockCore.FightZoneLabels);
    }

    [Fact]
    public void CapitalisationInconsistency()
    {
        // **`Dec`/`DEC` 两种写法并存**
        Assert.True(EnvirShakeLockCore.InconsistentCapitalisation());
        Assert.True(EnvirShakeLockCore.FourGoldFieldNameStyles());
        Assert.True(EnvirShakeLockCore.FourGoldFieldNames());
        Assert.True(EnvirShakeLockCore.BothDecStylesPresent());
        Assert.Equal(4, EnvirShakeLockCore.GoldFieldNames.Length);
    }

    [Fact]
    public void NoPrefixField()
    {
        // **有一处字段没有 m_ 前缀**
        Assert.True(EnvirShakeLockCore.NoPrefixFieldInClass());
        Assert.True(EnvirShakeLockCore.NoPrefixFieldConfirmed());
        Assert.Equal("sNoReconnectMap", EnvirShakeLockCore.NoPrefixField);
        Assert.False(EnvirShakeLockCore.NoPrefixField.StartsWith("m_", StringComparison.Ordinal));
    }

    [Fact]
    public void PrefixInconsistencyFamily()
    {
        Assert.True(EnvirShakeLockCore.PrefixInconsistencyFamily());
        Assert.True(EnvirShakeLockCore.TwoPrefixInconsistencyInstances());
        Assert.Equal(2, EnvirShakeLockCore.PrefixInconsistencyInstances.Length);
    }

    // ===================== 五、范围修正 =====================

    [Fact]
    public void SafeZoneNotInEnvir()
    {
        // **InSafeZone/InSafeArea 不是 TEnvirnoment 的方法**
        Assert.True(EnvirShakeLockCore.SafeZoneNotInEnvir());
        Assert.True(EnvirShakeLockCore.SafeZoneLivesInObjBaseAndManager());
        Assert.True(EnvirShakeLockCore.FourSafeZoneOwners());
        Assert.Equal(4, EnvirShakeLockCore.SafeZoneOwners.Length);
    }

    [Fact]
    public void SafeAreaIsSeparateManagerUnit()
    {
        Assert.True(EnvirShakeLockCore.SafeAreaIsSeparateManagerUnit());
        Assert.True(EnvirShakeLockCore.DeferredToLaterBatch());
    }

    // ===================== 行数 =====================

    [Fact]
    public void MethodLineCounts()
    {
        Assert.Equal(new[] { 16, 19, 4, 4, 4, 4, 5, 101 }, EnvirShakeLockCore.MethodLineCounts);
        Assert.True(EnvirShakeLockCore.EightMethods());
    }

    [Fact]
    public void FourForwardsAreFourLines()
    {
        Assert.True(EnvirShakeLockCore.FourForwardsAreFourLines());
    }

    [Fact]
    public void LineCountSkew()
    {
        // **GetEnvirInfo 101 行、其余七个合计 56 行**
        Assert.True(EnvirShakeLockCore.LineCountSkew());
        Assert.True(EnvirShakeLockCore.TotalLinesValues());
        Assert.Equal(157, EnvirShakeLockCore.TotalLines());
    }
}
