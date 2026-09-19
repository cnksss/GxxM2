using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J165：`TEnvirnoment` 构造销毁与地图查询族 1:1 测试。
/// **标志极性互补用全值域真值表验证、竞态窗口用时序模型论证、
/// 次序耦合用"加锁即崩"的双态模型固化。**
/// </summary>
public sealed class EnvirLifecycleCoreTests
{
    // ===================== 常量与锁 =====================

    [Fact]
    public void WeatherArraySize()
    {
        Assert.Equal(22, EnvirLifecycleCore.MaxMapWeatherEffect);
        Assert.True(EnvirLifecycleCore.WeatherArrayLengthIs22());
        Assert.Equal(22, EnvirLifecycleCore.WeatherArrayLength());
        Assert.Equal(21, EnvirLifecycleCore.WeatherArrayHighIndex());
        Assert.True(EnvirLifecycleCore.HighIndexIs21());
    }

    [Fact]
    public void NewLockIds()
    {
        // **28 与 29 —— 相邻但不同**
        Assert.True(EnvirLifecycleCore.TwoAdjacentDistinctLocks());
        Assert.Equal(new[] { 28, 29 }, EnvirLifecycleCore.NewLockIds);
        Assert.True(EnvirLifecycleCore.TwentyEightIsWriteLock());
        Assert.True(EnvirLifecycleCore.TwentyNineIsReadLock());
        Assert.True(EnvirLifecycleCore.ReadLockProtectsListNotObject());
    }

    // ===================== 一、AddToMapMineEvent =====================

    [Fact]
    public void RequiresNonZeroFlag()
    {
        // **标志不等于零才允许加**
        Assert.True(EnvirLifecycleCore.RequiresNonZeroFlag());
        Assert.False(EnvirLifecycleCore.MineEventGate(0));
        Assert.True(EnvirLifecycleCore.MineEventGate(1));
        Assert.True(EnvirLifecycleCore.MineEventGate(2));
    }

    [Fact]
    public void OppositeToAddToMap()
    {
        // **与 AddToMap 恰好相反**
        Assert.True(EnvirLifecycleCore.OppositeToAddToMap());
        Assert.True(EnvirLifecycleCore.SameFieldTwoPolarities());
        Assert.True(EnvirLifecycleCore.ComplementValues());
    }

    [Fact]
    public void SameFieldTwoPolaritiesExhaustive()
    {
        // **全值域穷举互补**
        for (int f = -2; f <= 5; f++)
        {
            Assert.NotEqual(EnvirLifecycleCore.MineEventGate(f), EnvirLifecycleCore.AddToMapGate(f));
        }
    }

    [Fact]
    public void ConfirmsJ130Record()
    {
        Assert.True(EnvirLifecycleCore.ConfirmsJ130Record());
        Assert.True(EnvirLifecycleCore.J130ConclusionContent());
        Assert.Contains("恰好相反", EnvirLifecycleCore.J130Conclusion);
    }

    [Fact]
    public void MineEventOnBlockedCell()
    {
        Assert.True(EnvirLifecycleCore.MineEventOnBlockedCell());
        Assert.True(EnvirLifecycleCore.ReasonableButMisleading());
        Assert.True(EnvirLifecycleCore.TwoFlagPurposeCount());
        Assert.Equal(2, EnvirLifecycleCore.TwoFlagPurposes.Length);
    }

    [Fact]
    public void InvalidMapReturnsNull()
    {
        Assert.True(EnvirLifecycleCore.InvalidMapReturnsNull());
        Assert.True(EnvirLifecycleCore.InvalidMapWins());
        Assert.Null(EnvirLifecycleCore.MineEventResult(true, true));
        Assert.NotNull(EnvirLifecycleCore.MineEventResult(false, true));
    }

    [Fact]
    public void UsesWriteLock()
    {
        Assert.True(EnvirLifecycleCore.UsesWriteLock28());
    }

    [Fact]
    public void SwallowsException()
    {
        Assert.True(EnvirLifecycleCore.SwallowsException());
        Assert.True(EnvirLifecycleCore.SwallowVerified());
        Assert.False(EnvirLifecycleCore.TryCatchSwallows(true));
    }

    [Fact]
    public void ExceptionMessages()
    {
        // **一条带尾空格、一条没有**
        Assert.True(EnvirLifecycleCore.MessageEndsWithSpace());
        Assert.True(EnvirLifecycleCore.MineEventMsgEndsWithSpace());
        Assert.True(EnvirLifecycleCore.VerifyMessageNoTrailingSpace());
        Assert.True(EnvirLifecycleCore.VerifyMsgDoesNotEndWithSpace());
        Assert.True(EnvirLifecycleCore.TrailingSpaceIsInconsistent());
    }

    [Fact]
    public void MessageLiterals()
    {
        Assert.Equal("[Exception] TEnvirnoment.AddToMapMineEvent ", EnvirLifecycleCore.MineEventExceptionMsg);
        Assert.Equal("[Exception] TEnvirnoment.VerifyMapTime", EnvirLifecycleCore.VerifyMapTimeExceptionMsg);
        Assert.True(EnvirLifecycleCore.TwoMessageLengthsDifferByFive());
        Assert.True(EnvirLifecycleCore.SamePrefix());
        Assert.True(EnvirLifecycleCore.BothAreBareMethodNames());
    }

    [Fact]
    public void CommentedDetailedLog()
    {
        Assert.True(EnvirLifecycleCore.CommentedDetailedLog());
        Assert.True(EnvirLifecycleCore.SevenCommentedLogFields());
        Assert.True(EnvirLifecycleCore.CommentedLogIsRicher());
        Assert.Equal(7, EnvirLifecycleCore.CommentedLogFields.Length);
    }

    [Fact]
    public void LockPlacement()
    {
        // **加锁在 try 之前、解锁在 finally 里**
        Assert.True(EnvirLifecycleCore.LockOutsideTryUnlockInFinally());
        Assert.True(EnvirLifecycleCore.ExceptionPathStillUnlocks());
        Assert.True(EnvirLifecycleCore.UnlockAlwaysRunsVerified());
        Assert.True(EnvirLifecycleCore.UnlockAlwaysRuns(true));
    }

    // ===================== 二、VerifyMapTime =====================

    [Fact]
    public void VerifyFindsByPointer()
    {
        Assert.True(EnvirLifecycleCore.VerifyFindsByPointer());
        Assert.True(EnvirLifecycleCore.VerifyMatchTruthTable());
        Assert.True(EnvirLifecycleCore.VerifyMatch(EnvirLifecycleCore.ObjActor, true, true));
        Assert.False(EnvirLifecycleCore.VerifyMatch(EnvirLifecycleCore.ObjActor, true, false));
    }

    [Fact]
    public void UsesReadLock()
    {
        Assert.True(EnvirLifecycleCore.UsesReadLock29());
    }

    [Fact]
    public void AddToMapOutsideLock()
    {
        // **补挂在解锁之后 —— 存在竞态窗口**
        Assert.True(EnvirLifecycleCore.AddToMapOutsideLock());
        Assert.True(EnvirLifecycleCore.RaceWindowExists());
        Assert.True(EnvirLifecycleCore.RaceWindowModel());
        Assert.True(EnvirLifecycleCore.BothThreadsAdd());
    }

    [Fact]
    public void NestedTryShapes()
    {
        Assert.True(EnvirLifecycleCore.OuterTryExcept());
        Assert.True(EnvirLifecycleCore.NestedTryShapes());
        Assert.True(EnvirLifecycleCore.TwoTryLayers());
        Assert.Equal(2, EnvirLifecycleCore.NestedTryShape.Length);
    }

    [Fact]
    public void PossibleReasonForOutsideLock()
    {
        // **放锁外可能正是为回避同类型锁嵌套**
        Assert.True(EnvirLifecycleCore.PossibleReasonForOutsideLock());
        Assert.True(EnvirLifecycleCore.WouldNestSameTypeLock());
        Assert.True(EnvirLifecycleCore.J163SaysNoNesting());
        Assert.True(EnvirLifecycleCore.SequentialAvoidsNesting());
        Assert.True(EnvirLifecycleCore.NestedWouldReachDepthTwo());
    }

    [Fact]
    public void AddTimeSemantics()
    {
        // **把存活计时清零**
        Assert.True(EnvirLifecycleCore.AddTimeIsResidenceTimer());
        Assert.True(EnvirLifecycleCore.VerifyMeaningResetTimer());
        Assert.True(EnvirLifecycleCore.ResetAddTimeValues());
        Assert.Equal(123456, EnvirLifecycleCore.ResetAddTime(123456));
    }

    [Fact]
    public void SameFieldFamily()
    {
        Assert.True(EnvirLifecycleCore.SameFieldAsJ153());
        Assert.True(EnvirLifecycleCore.DifferentFieldsButSameFamily());
    }

    [Fact]
    public void BreaksOnFirstMatch()
    {
        Assert.True(EnvirLifecycleCore.BreaksOnFirstMatch());
        Assert.True(EnvirLifecycleCore.AddsOnlyWhenNotFound());
        Assert.True(EnvirLifecycleCore.VerifyOutcomeValues());
        Assert.Equal("reset", EnvirLifecycleCore.VerifyOutcome(true));
        Assert.Equal("add", EnvirLifecycleCore.VerifyOutcome(false));
    }

    [Fact]
    public void DoubleCellLookupOnMiss()
    {
        // **未命中时查格两次**
        Assert.True(EnvirLifecycleCore.BoVerifyStartsFalse());
        Assert.True(EnvirLifecycleCore.CellLookupFailureAlsoAdds());
        Assert.True(EnvirLifecycleCore.DoubleCellLookupOnMiss());
        Assert.True(EnvirLifecycleCore.CellLookupCountValues());
        Assert.Equal(1, EnvirLifecycleCore.CellLookupCount(true));
        Assert.Equal(2, EnvirLifecycleCore.CellLookupCount(false));
    }

    // ===================== 三、GetMainMap =====================

    [Fact]
    public void GetMainMap()
    {
        Assert.True(EnvirLifecycleCore.TwoBranches());
        Assert.True(EnvirLifecycleCore.MainMapValues());
        Assert.Equal("M1", EnvirLifecycleCore.MainMap(true, "M1", "M2"));
        Assert.Equal("M2", EnvirLifecycleCore.MainMap(false, "M1", "M2"));
    }

    [Fact]
    public void NoGuardsAtAll()
    {
        Assert.True(EnvirLifecycleCore.NoGuardsAtAll());
        Assert.True(EnvirLifecycleCore.NoNullCheckNoInvalidNoLock());
        Assert.True(EnvirLifecycleCore.UninitialisedReturnsEmpty());
        Assert.Equal("", EnvirLifecycleCore.MainMap(false, "", ""));
    }

    [Fact]
    public void PrefixFamily()
    {
        // **两个字段都没有 m_ 前缀**
        Assert.True(EnvirLifecycleCore.BothFieldsLackPrefix());
        Assert.True(EnvirLifecycleCore.NeitherStartsWithMPrefix());
        Assert.True(EnvirLifecycleCore.PrefixFamilyThirdInstance());
        Assert.True(EnvirLifecycleCore.ThreePrefixFamilyInstances());
        Assert.Equal(new[] { "sMainMapName", "sMapName" }, EnvirLifecycleCore.MainMapFields);
        Assert.Equal(3, EnvirLifecycleCore.PrefixFamily.Length);
    }

    // ===================== 四、Create =====================

    [Fact]
    public void ListCreation()
    {
        Assert.True(EnvirLifecycleCore.FiveListsCreated());
        Assert.True(EnvirLifecycleCore.FiveCreatedListNames());
        Assert.True(EnvirLifecycleCore.SceneShakeListIsTGList());
        Assert.True(EnvirLifecycleCore.SixListsCreated());
        Assert.Equal(5, EnvirLifecycleCore.CreatedLists.Length);
        Assert.Equal(6, EnvirLifecycleCore.TotalListsCreated());
    }

    [Fact]
    public void TwoAllocationStrategies()
    {
        // **六个立即创建、三个延迟创建（置 nil）**
        Assert.True(EnvirLifecycleCore.ThreePointersSetToNil());
        Assert.True(EnvirLifecycleCore.ThreeNilListNames());
        Assert.True(EnvirLifecycleCore.TwoAllocationStrategies());
        Assert.True(EnvirLifecycleCore.TwoStrategyNames());
        Assert.Equal(3, EnvirLifecycleCore.NilLists.Length);
        Assert.Equal(2, EnvirLifecycleCore.AllocationStrategies.Length);
    }

    [Fact]
    public void GuardsMatchStrategies()
    {
        // **延迟创建的三个在 Destroy 里都有判空**
        Assert.True(EnvirLifecycleCore.DelayedListsHaveNullGuardInDestroy());
        Assert.True(EnvirLifecycleCore.ImmediateListsHaveNoNullGuard());
        Assert.True(EnvirLifecycleCore.GuardsMatchStrategies());
    }

    [Fact]
    public void CommentedOldWeatherInit()
    {
        // **十二行整块被注释掉的旧版天气初始化**
        Assert.True(EnvirLifecycleCore.CommentedOldWeatherInit());
        Assert.True(EnvirLifecycleCore.TwelveCommentedLines());
        Assert.Equal(12, EnvirLifecycleCore.CommentedWeatherLines());
    }

    [Fact]
    public void OldThreeSlotDesign()
    {
        // **三槽扩到二十二槽、每槽四字段增到五字段**
        Assert.True(EnvirLifecycleCore.OldThreeSlotDesign());
        Assert.True(EnvirLifecycleCore.ExpandedFromThreeToTwentyTwo());
        Assert.True(EnvirLifecycleCore.TwelveEqualsThreeTimesFour());
        Assert.Equal(3, EnvirLifecycleCore.OldSlotCount());
        Assert.Equal(4, EnvirLifecycleCore.OldFieldsPerSlot());
        Assert.Equal(12, EnvirLifecycleCore.OldSlotCount() * EnvirLifecycleCore.OldFieldsPerSlot());
    }

    [Fact]
    public void NewSlotFields()
    {
        Assert.True(EnvirLifecycleCore.FieldsGrewFromFourToFive());
        Assert.Equal(5, EnvirLifecycleCore.NewFieldsPerSlot());
        Assert.True(EnvirLifecycleCore.NewFieldsMatchRecord());
        Assert.Equal(
            new[] { "boIsUsed", "boIsDark", "dwTick", "dwTime", "sMusic" },
            EnvirLifecycleCore.NewSlotFields);
        Assert.True(EnvirLifecycleCore.SameFamilyAsMakeMapMagic());
    }

    [Fact]
    public void NegativeInitialValues()
    {
        // **两个 -1 初值**
        Assert.True(EnvirLifecycleCore.TwoNegativeInitialValues());
        Assert.True(EnvirLifecycleCore.BothNegativeOnes());
        Assert.Equal(2, EnvirLifecycleCore.NegativeInitialValues.Length);
        Assert.Equal(-1, EnvirLifecycleCore.NegativeInitialValues[0].Value);
    }

    [Fact]
    public void ExpressionInitialValue()
    {
        // **唯一带表达式的初值：三十秒写成乘积**
        Assert.True(EnvirLifecycleCore.ExpressionNotLiteral());
        Assert.True(EnvirLifecycleCore.ThirtySecondsWrittenAsProduct());
        Assert.True(EnvirLifecycleCore.RevivalCheckTimeIsThirtyThousand());
        Assert.Equal(30000, EnvirLifecycleCore.RevivalCheckTime());
    }

    [Fact]
    public void FbInitialValues()
    {
        Assert.True(EnvirLifecycleCore.NoHumClearMinIsTen());
        Assert.True(EnvirLifecycleCore.FbDelayZeroClearTen());
        Assert.Equal(0, EnvirLifecycleCore.FbInitialValues[0].Value);
        Assert.Equal(10, EnvirLifecycleCore.FbInitialValues[1].Value);
    }

    [Fact]
    public void OrderHazard()
    {
        // **ResetGuardianLevel 在临界区创建之前 —— 现在安全、加锁后危险**
        Assert.True(EnvirLifecycleCore.ResetBeforeCriticalSection());
        Assert.True(EnvirLifecycleCore.SafeCurrently());
        Assert.True(EnvirLifecycleCore.FutureHazardIfLockAdded());
        Assert.True(EnvirLifecycleCore.OrderHazardVerified());
        Assert.True(EnvirLifecycleCore.OrderHazardModel(false));
        Assert.False(EnvirLifecycleCore.OrderHazardModel(true));
    }

    [Fact]
    public void CreateTailOrder()
    {
        Assert.True(EnvirLifecycleCore.CriticalSectionCreatedLast());
        Assert.True(EnvirLifecycleCore.ResetIsSecondToLast());
        Assert.True(EnvirLifecycleCore.ThreeCreateTailSteps());
        Assert.Equal(3, EnvirLifecycleCore.CreateTailSteps.Length);
    }

    // ===================== 五、Destroy =====================

    [Fact]
    public void DestroySizeGuard()
    {
        // **与传奇3 装载器同样的守卫**
        Assert.True(EnvirLifecycleCore.DestroyHasSizeGuard());
        Assert.True(EnvirLifecycleCore.SameGuardAsEILoader());
        Assert.True(EnvirLifecycleCore.DestroyGuard(2, 2));
        Assert.False(EnvirLifecycleCore.DestroyGuard(1, 1));
    }

    [Fact]
    public void OneByOneLeaksForever()
    {
        // **一乘一的地图永远不被清理**
        Assert.True(EnvirLifecycleCore.OneByOneLeaksForever());
        Assert.True(EnvirLifecycleCore.OneByOneGuardBlocksCleanup());
        Assert.False(EnvirLifecycleCore.DestroyGuard(1, 100));
    }

    [Fact]
    public void DestroyUsesAccessor()
    {
        Assert.True(EnvirLifecycleCore.GetMapCellInfoCalledInDestroy());
        Assert.True(EnvirLifecycleCore.UsesAccessorNotDirectIndex());
    }

    [Fact]
    public void CommentedObjectFreeBlock()
    {
        // **外层花括号 + 内层行首斜杠 = 双重注释**
        Assert.True(EnvirLifecycleCore.CommentedObjectFreeBlock());
        Assert.True(EnvirLifecycleCore.TwoOfThreeCommented());
        Assert.True(EnvirLifecycleCore.WholeBlockIsBraceComment());
        Assert.True(EnvirLifecycleCore.DoubleCommented());
        Assert.Equal(3, EnvirLifecycleCore.CommentedFreeCases.Length);
    }

    [Fact]
    public void WrongPrefixInComment()
    {
        // **注释里写的是 g_ 前缀、真实枚举是 Obj_ 前缀**
        Assert.True(EnvirLifecycleCore.WrongPrefixInComment());
        Assert.True(EnvirLifecycleCore.ThreeEnumNamePairs());
        Assert.True(EnvirLifecycleCore.PrefixMismatchInComment());
        Assert.Equal(new[] { "Obj_Item", "Obj_Event", "Obj_Gate" }, EnvirLifecycleCore.RealEnumNames);
        Assert.Equal(new[] { "g_Item", "g_Event", "g_Gate" }, EnvirLifecycleCore.CommentEnumNames);
    }

    [Fact]
    public void DoorStatusRefCount()
    {
        // **引用计数递减、到零或以下才释放**
        Assert.True(EnvirLifecycleCore.DoorStatusRefCounted());
        Assert.True(EnvirLifecycleCore.DoorStatusReleaseValues());
        Assert.True(EnvirLifecycleCore.ReleaseUsesLessOrEqual());
        Assert.True(EnvirLifecycleCore.ExplainsLoadTimeIncrement());

        var (n2, d2) = EnvirLifecycleCore.ReleaseDoorStatus(2);
        Assert.Equal(1, n2);
        Assert.False(d2);

        var (n1, d1) = EnvirLifecycleCore.ReleaseDoorStatus(1);
        Assert.Equal(0, n1);
        Assert.True(d1);
    }

    [Fact]
    public void NoNullCheckOnDoorStatus()
    {
        Assert.True(EnvirLifecycleCore.NoNullCheckOnDoorStatus());
        Assert.True(EnvirLifecycleCore.PossiblyNullStatus());
        Assert.True(EnvirLifecycleCore.NormalPathAlwaysNonNull());
    }

    [Fact]
    public void DestroyOrder()
    {
        Assert.True(EnvirLifecycleCore.ElevenDestroySteps());
        Assert.Equal(11, EnvirLifecycleCore.DestroyOrder.Length);
        Assert.True(EnvirLifecycleCore.CriticalSectionReleasedSecondToLast());
    }

    [Fact]
    public void CreateDestroySymmetric()
    {
        // **临界区最后创建、倒数第二释放 —— 次序对称**
        Assert.True(EnvirLifecycleCore.SymmetricWithCreate());
        Assert.True(EnvirLifecycleCore.CreateDestroySymmetric());
    }

    [Fact]
    public void ClearBeforeFree()
    {
        // **先清空再释放容器，否则泄漏记录**
        Assert.True(EnvirLifecycleCore.ClearBeforeFree());
        Assert.True(EnvirLifecycleCore.WrongOrderWouldLeak());
        Assert.True(EnvirLifecycleCore.MatchesJ163());
    }

    [Fact]
    public void DisposeSites()
    {
        Assert.True(EnvirLifecycleCore.ThreeDisposeSites());
        Assert.True(EnvirLifecycleCore.ThreeDisposeTargets());
        Assert.True(EnvirLifecycleCore.OnlyDoorStatusIsRefCounted());
        Assert.True(EnvirLifecycleCore.OthersAreOneToOne());
        Assert.Equal(3, EnvirLifecycleCore.DisposeTargets.Length);
    }

    // ===================== 六、ResetGuardianLevel =====================

    [Fact]
    public void GuardianBooleanFlags()
    {
        Assert.True(EnvirLifecycleCore.FourBooleanFlagsCleared());
        Assert.True(EnvirLifecycleCore.FourGuardianFlags());
        Assert.Equal(4, EnvirLifecycleCore.GuardianBooleanFlags.Length);
    }

    [Fact]
    public void SuccesMisspelling()
    {
        // **Succes 少一个 s**
        Assert.True(EnvirLifecycleCore.SuccesIsMisspelled());
        Assert.True(EnvirLifecycleCore.MisspellingConfirmed());
        Assert.True(EnvirLifecycleCore.SameFamilyAsPorcess());
        Assert.True(EnvirLifecycleCore.ThreeMisspellingInstances());
        Assert.Equal(3, EnvirLifecycleCore.MisspellingFamily.Length);
    }

    [Fact]
    public void TwoGuardianSpellings()
    {
        // **GuardianLevel 与 GuardinaLevel 两种拼法并存**
        Assert.True(EnvirLifecycleCore.GuardinaMisspelled());
        Assert.True(EnvirLifecycleCore.TwoSpellingsCoexist());
        Assert.True(EnvirLifecycleCore.BothSpellingsInSameFunction());
        Assert.Equal(new[] { "GuardianLevel", "GuardinaLevel" }, EnvirLifecycleCore.GuardianSpellings);
    }

    [Fact]
    public void GuardianArraySizes()
    {
        // **两个长四、两个长四十**
        Assert.True(EnvirLifecycleCore.TwoSizesFourAndForty());
        Assert.Equal(4, EnvirLifecycleCore.GuardianArraySizes.Length);
        Assert.Equal(4, EnvirLifecycleCore.GuardianArraySizes[0].Length);
        Assert.Equal(40, EnvirLifecycleCore.GuardianArraySizes[1].Length);
    }

    [Fact]
    public void ButchItemsShape()
    {
        // **二维 40x4 = 160 个整数**
        Assert.True(EnvirLifecycleCore.ButchItemsIsTwoDimensional());
        Assert.True(EnvirLifecycleCore.ButchItemsIs160Ints());
        Assert.True(EnvirLifecycleCore.ButchIs2DHasIs1D());
        var (r, c) = EnvirLifecycleCore.ButchItemsShape();
        Assert.Equal(40, r);
        Assert.Equal(4, c);
        Assert.Equal(160, r * c);
    }

    [Fact]
    public void BatchSpelling()
    {
        // **Butch 与 Batch 两种拼法**
        Assert.True(EnvirLifecycleCore.ButchVsBatchSpelling());
        Assert.True(EnvirLifecycleCore.CorrectBatchSpellingElsewhere());
        Assert.True(EnvirLifecycleCore.ThreeBatchSpellings());
        Assert.Equal(3, EnvirLifecycleCore.BatchSpellings.Length);
    }

    [Fact]
    public void ThreeClearingTechniques()
    {
        Assert.True(EnvirLifecycleCore.ThreeClearingTechniques());
        Assert.True(EnvirLifecycleCore.ThreeClearingTechniqueNames());
        Assert.True(EnvirLifecycleCore.FillCharOnTwoArrays());
        Assert.Equal(3, EnvirLifecycleCore.ClearingTechniques.Length);
    }

    [Fact]
    public void FillCharBytes()
    {
        // **六百五十六字节**
        Assert.True(EnvirLifecycleCore.FillCharBytesValue());
        Assert.Equal(656, EnvirLifecycleCore.FillCharBytes());
        Assert.True(EnvirLifecycleCore.StringsUseLoopNotFillChar());
    }

    [Fact]
    public void DoubleSemicolonTypo()
    {
        // **两个分号**
        Assert.True(EnvirLifecycleCore.DoubleSemicolonTypo());
        Assert.True(EnvirLifecycleCore.DoubleSemicolonConfirmed());
        Assert.True(EnvirLifecycleCore.LegalInDelphi());
        Assert.True(EnvirLifecycleCore.SameFamilyAsCheapStuff());
        Assert.True(EnvirLifecycleCore.TwoSemicolonTypos());
        Assert.EndsWith(";;", EnvirLifecycleCore.DoubleSemicolonLine, StringComparison.Ordinal);
        Assert.Equal(2, EnvirLifecycleCore.SemicolonTypos.Length);
    }

    [Fact]
    public void NilObjectFields()
    {
        Assert.True(EnvirLifecycleCore.TwoNilObjectFields());
        Assert.True(EnvirLifecycleCore.TwoNilObjectFieldNames());
        Assert.True(EnvirLifecycleCore.StatueFieldAlsoMisspelled());
        Assert.Equal(2, EnvirLifecycleCore.NilObjectFields.Length);
    }

    [Fact]
    public void GuardianIntFields()
    {
        // **六个整数清零**
        Assert.True(EnvirLifecycleCore.SixIntClearings());
        Assert.True(EnvirLifecycleCore.SixGuardianIntFields());
        Assert.Equal(6, EnvirLifecycleCore.GuardianIntFields.Length);
    }

    // ===================== 行数 =====================

    [Fact]
    public void MethodLineCounts()
    {
        Assert.Equal(new[] { 8, 40, 42, 106, 87, 28 }, EnvirLifecycleCore.MethodLineCounts);
        Assert.True(EnvirLifecycleCore.SixMethods());
    }

    [Fact]
    public void LineExtremes()
    {
        Assert.True(EnvirLifecycleCore.CreateIsLongest());
        Assert.True(EnvirLifecycleCore.GetMainMapIsShortest());
    }

    [Fact]
    public void TotalLines()
    {
        Assert.True(EnvirLifecycleCore.TotalLinesValues());
        Assert.Equal(311, EnvirLifecycleCore.TotalLines());
        Assert.True(EnvirLifecycleCore.ConstructDestroyShareIs62());
    }
}
