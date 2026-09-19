using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J159：`TEnvirnoment` 取物品与取移动对象接口 1:1 测试。
/// **锁号体系由程序化枚举核对、`bo2B9` 的含义由源码三处赋值反推、
/// 三档严格度与 GM 保护全部经探针实测。**
/// </summary>
public sealed class EnvirObjectQueryCoreTests
{
    // ===================== 锁号体系 =====================

    [Fact]
    public void LockIdEnumeration()
    {
        // **28 处调用、27 个不同编号、最大 56**
        Assert.True(EnvirObjectQueryCore.LockIdEnumeratedProgrammatically());
        Assert.Equal(28, EnvirObjectQueryCore.AllEnvirLockCallCount());
        Assert.Equal(27, EnvirObjectQueryCore.DistinctEnvirLockIds());
        Assert.Equal(56, EnvirObjectQueryCore.MaxEnvirLockId());
    }

    [Fact]
    public void LockIdsSkipThirtyFive()
    {
        // **33、34、36、37 —— 跳过 35**
        Assert.True(EnvirObjectQueryCore.LockIdsSkipThirtyFive());
        Assert.Equal(new[] { 33, 34, 36, 37 }, EnvirObjectQueryCore.MovingObjectLockIds);
        Assert.Equal(new[] { 38, 39, 40 }, EnvirObjectQueryCore.GetItemExLockIds);
    }

    [Fact]
    public void LockThirtyIsDuplicated()
    {
        // **唯一重复的编号是 30，被两个不同方法共用**
        Assert.True(EnvirObjectQueryCore.LockThirtyIsDuplicated());
        Assert.True(EnvirObjectQueryCore.ThirtyIsOnlyDuplicate());
        Assert.True(EnvirObjectQueryCore.LockThirtyTwoUsers());
        Assert.Equal(2, EnvirObjectQueryCore.LockThirtyUsers.Length);
    }

    [Fact]
    public void LockIdHolesAndDuplicates()
    {
        // **有空洞也有重复 —— 手工分配、不是自动递增**
        Assert.True(EnvirObjectQueryCore.MissingLockIdCountValues());
        Assert.Equal(29, EnvirObjectQueryCore.MissingLockIdCount());
        Assert.True(EnvirObjectQueryCore.ManualLockIdAllocation());
    }

    [Fact]
    public void LockFamiliesAdjacent()
    {
        // **取物品的锁号紧接在取对象之后（37 → 38）**
        Assert.True(EnvirObjectQueryCore.TwoFamiliesAdjacent());
    }

    // ===================== 一、五条件门 =====================

    [Fact]
    public void FiveConditionGate()
    {
        Assert.True(EnvirObjectQueryCore.FiveConditionGate(true, true, true, true, false, true));
        Assert.False(EnvirObjectQueryCore.FiveConditionGate(false, true, true, true, false, true));
        Assert.False(EnvirObjectQueryCore.FiveConditionGate(true, true, true, false, false, true));
    }

    [Fact]
    public void DeathGateCanBeDisabled()
    {
        // **boFlag=False 时整条死亡判定被短路、死人也会被返回**
        Assert.True(EnvirObjectQueryCore.DeathGateCanBeDisabled());
        Assert.True(EnvirObjectQueryCore.FlagFalseShortCircuitsDeath());
        Assert.True(EnvirObjectQueryCore.DeathGateTruthTable());
    }

    [Fact]
    public void SecondAndThirdConditionsPresent()
    {
        // **②非幽灵、③bo2B9 都不可或缺**
        Assert.True(EnvirObjectQueryCore.SecondAndThirdConditionsPresent());
    }

    [Fact]
    public void ActorFilter()
    {
        Assert.True(EnvirObjectQueryCore.NullCheckAfterActorFilter());
        Assert.True(EnvirObjectQueryCore.ActorFilterValues());
    }

    // ===================== bo2B9 =====================

    [Fact]
    public void Bo2B9RawOffsetName()
    {
        // **用原始内存偏移当地址名**
        Assert.True(EnvirObjectQueryCore.Bo2B9RawOffsetName());
        Assert.True(EnvirObjectQueryCore.RawOffsetNamedField());
        Assert.Contains("0x2B9", EnvirObjectQueryCore.Bo2B9Declaration);
    }

    [Fact]
    public void Bo2B9DefaultsTrue()
    {
        Assert.True(EnvirObjectQueryCore.Bo2B9DefaultsTrue());
        Assert.True(EnvirObjectQueryCore.DefaultBo2B9);
    }

    [Fact]
    public void Bo2B9SetByCastleDoorOnly()
    {
        // **三处赋值：默认 True、开门 False、关门 True**
        Assert.True(EnvirObjectQueryCore.Bo2B9SetByCastleDoorOnly());
        Assert.True(EnvirObjectQueryCore.ThreeBo2B9Assignments());
        Assert.Equal(3, EnvirObjectQueryCore.Bo2B9AssignmentSites.Length);
        Assert.True(EnvirObjectQueryCore.Bo2B9FollowsDoorState());
    }

    [Fact]
    public void Bo2B9Meaning()
    {
        // **城门是否可被选中**
        Assert.True(EnvirObjectQueryCore.Bo2B9MeaningKnown());
        Assert.True(EnvirObjectQueryCore.Bo2B9ExplainsNaming());
        Assert.False(EnvirObjectQueryCore.CastleDoorOpenSetsBo2B9());
        Assert.True(EnvirObjectQueryCore.CastleDoorCloseSetsBo2B9());
    }

    // ===================== 二、四个重载 =====================

    [Fact]
    public void FourOverloadsOneGate()
    {
        Assert.True(EnvirObjectQueryCore.FourOverloadsOneGate());
        Assert.True(EnvirObjectQueryCore.OnlyThreeDifferences());
        Assert.True(EnvirObjectQueryCore.ThreeOverloadDifferences());
        Assert.Equal(3, EnvirObjectQueryCore.OverloadDifferences.Length);
    }

    [Fact]
    public void CollectVersusFirst()
    {
        // **第一个收集全部、其余取首个**
        Assert.True(EnvirObjectQueryCore.FirstOverloadCollectsAll());
        Assert.True(EnvirObjectQueryCore.CollectAllCountsEveryHit());
        Assert.True(EnvirObjectQueryCore.SecondReturnsFirst());
        Assert.True(EnvirObjectQueryCore.FirstMatchStops());
        Assert.True(EnvirObjectQueryCore.CollectAndFirstDiverge());
    }

    [Fact]
    public void ThirdExcludesOneObject()
    {
        // **是"排除"而不是"只选"**
        Assert.True(EnvirObjectQueryCore.ThirdExcludesOneObject());
        Assert.True(EnvirObjectQueryCore.ExcludesRatherThanSelects());
        Assert.True(EnvirObjectQueryCore.NotTheSpecifiedObject("b", "a"));
        Assert.False(EnvirObjectQueryCore.NotTheSpecifiedObject("a", "a"));
    }

    [Fact]
    public void FourthAddsProperTarget()
    {
        Assert.True(EnvirObjectQueryCore.FourthAddsProperTarget());
    }

    [Fact]
    public void NilListStillCounts()
    {
        // **nil 列表仍然计数、只是不收集**
        Assert.True(EnvirObjectQueryCore.NilListStillCounts());
        Assert.True(EnvirObjectQueryCore.AddBeforeIncrement());
        Assert.True(EnvirObjectQueryCore.AddFailurePreventsCount());
    }

    [Fact]
    public void FourthOverloadRenamesVariable()
    {
        // **形参 BaseObject、局部 ABaseObject**
        Assert.True(EnvirObjectQueryCore.FourthOverloadRenamesVariable());
        Assert.True(EnvirObjectQueryCore.TwoNamesInFourth());
        Assert.True(EnvirObjectQueryCore.FirstThreeShareVariableName());
        Assert.Equal(2, EnvirObjectQueryCore.FourthOverloadNames.Length);
    }

    // ===================== 三、GetMovingObjectEx =====================

    [Fact]
    public void ProperTargetEarlyExits()
    {
        Assert.True(EnvirObjectQueryCore.IsProperTargetFourEarlyExits());
        Assert.True(EnvirObjectQueryCore.FourEarlyExits());
        Assert.True(EnvirObjectQueryCore.EarlyExitValues());
        Assert.Equal(4, EnvirObjectQueryCore.ProperTargetEarlyExits.Length);
    }

    [Fact]
    public void ProperTargetErrorCodeLadder()
    {
        // **nErrorCode 从 0 走到 9**
        Assert.True(EnvirObjectQueryCore.IsProperTargetErrorCodeLadder());
        Assert.True(EnvirObjectQueryCore.TenErrorCodes());
        Assert.True(EnvirObjectQueryCore.MaxErrorCodeIsNine());
        Assert.Equal(10, EnvirObjectQueryCore.ErrorCodeLadder.Length);
    }

    [Fact]
    public void ProperTargetExceptionHandler()
    {
        // **"方法名 + 错误码"族里带两个字段的变体**
        Assert.True(EnvirObjectQueryCore.IsProperTargetExceptionHandler());
        Assert.True(EnvirObjectQueryCore.ErrorFormatHasTwoFields());
        Assert.True(EnvirObjectQueryCore.SecondFieldVariant());
    }

    [Fact]
    public void ProperTargetMasterBranch()
    {
        Assert.True(EnvirObjectQueryCore.IsProperTargetMasterBranch());
        Assert.True(EnvirObjectQueryCore.TwoMasterPaths());
        Assert.Equal(2, EnvirObjectQueryCore.MasterBranchPaths.Length);
    }

    [Fact]
    public void GamePetException()
    {
        Assert.True(EnvirObjectQueryCore.GamePetException());
        Assert.True(EnvirObjectQueryCore.GamePetExemptValues());
    }

    [Fact]
    public void SafeZoneVeto()
    {
        // **是"或"而不是"与"**
        Assert.True(EnvirObjectQueryCore.IsProperTargetSafeZoneVeto());
        Assert.True(EnvirObjectQueryCore.SafeZoneVetoIsOr());
    }

    [Fact]
    public void ProperTargetCommentedLine()
    {
        Assert.True(EnvirObjectQueryCore.IsProperTargetCommentedLine());
        Assert.True(EnvirObjectQueryCore.ProperTargetCommentedLinePresent());
    }

    [Fact]
    public void GmProtection()
    {
        // **三个条件缺一不可**
        Assert.True(EnvirObjectQueryCore.GmProtectionThreeConditions());
        Assert.True(EnvirObjectQueryCore.GmProtectionServerGate());
        Assert.True(EnvirObjectQueryCore.GmProtectionTargetGate());
        Assert.True(EnvirObjectQueryCore.GmProtectionRequiresHuman());
    }

    [Fact]
    public void GmProtectionUsesContinue()
    {
        // **跳过 GM、继续找后面的合法目标**
        Assert.True(EnvirObjectQueryCore.GmProtectionUsesContinue());
        Assert.True(EnvirObjectQueryCore.GmProtectionSkipsNotAborts());
        Assert.True(EnvirObjectQueryCore.AllGmReturnsNull());
        Assert.True(EnvirObjectQueryCore.GmCheckBeforeAssign());
    }

    // ===================== 四、三个 GetItemEx =====================

    [Fact]
    public void ThreeTierStrictness()
    {
        // **全部 actor 否决 → 只否决人物 → 都不否决**
        Assert.True(EnvirObjectQueryCore.ThreeTierStrictness());
        Assert.True(EnvirObjectQueryCore.ThreeTiers());
        Assert.True(EnvirObjectQueryCore.StrictnessOrder());
        Assert.Equal(3, EnvirObjectQueryCore.StrictnessTiers.Length);
    }

    [Fact]
    public void TiersDiverge()
    {
        // **在"怪物未死亡"时三档结果不同**
        Assert.True(EnvirObjectQueryCore.TiersDivergeOnLiveMonster());
        Assert.True(EnvirObjectQueryCore.TiersAgreeOnLivePlayer());
        Assert.True(EnvirObjectQueryCore.TiersAgreeOnDeath());
    }

    [Fact]
    public void TierBoundaries()
    {
        Assert.True(EnvirObjectQueryCore.ActorVetoes(1, 0, false));
        Assert.True(EnvirObjectQueryCore.ActorVetoes(2, 0, false));
        Assert.False(EnvirObjectQueryCore.ActorVetoes(3, 0, false));
        Assert.True(EnvirObjectQueryCore.ActorVetoes(1, 50, false));
        Assert.False(EnvirObjectQueryCore.ActorVetoes(2, 50, false));
    }

    [Fact]
    public void GetItemEx2CommentedOldCondition()
    {
        // **旧的"三类种族"条件被注释掉、收窄成只有人物**
        Assert.True(EnvirObjectQueryCore.GetItemEx2CommentedOldCondition());
        Assert.True(EnvirObjectQueryCore.ThreeOldRaces());
        Assert.True(EnvirObjectQueryCore.NarrowedToOneRace());
        Assert.True(EnvirObjectQueryCore.SetSyntaxVersusEqualitySyntax());
        Assert.Equal(3, EnvirObjectQueryCore.OldRaceSet.Length);
    }

    [Fact]
    public void GetItemEx3HasNoActorBranch()
    {
        Assert.True(EnvirObjectQueryCore.GetItemEx3HasNoActorBranch());
        Assert.True(EnvirObjectQueryCore.GetItemEx3OmitsBaseObjectVar());
        Assert.True(EnvirObjectQueryCore.LineCountDifferenceExplained());
    }

    [Fact]
    public void ResultIsLastItemNotFirst()
    {
        // **与 GetMovingObject 的"首个命中"正好相反**
        Assert.True(EnvirObjectQueryCore.ResultIsLastItemNotFirst());
        Assert.True(EnvirObjectQueryCore.LastWins());
        Assert.True(EnvirObjectQueryCore.OppositeOfMovingObject());
    }

    [Fact]
    public void CountAllItems()
    {
        Assert.True(EnvirObjectQueryCore.CountAllItems());
        Assert.True(EnvirObjectQueryCore.CountItemsValues());
    }

    [Fact]
    public void ThreeLineInitialization()
    {
        Assert.True(EnvirObjectQueryCore.ThreeLineInitializationOrder());
        Assert.True(EnvirObjectQueryCore.ThreeInitLines());
        Assert.Equal(3, EnvirObjectQueryCore.InitLines.Length);
    }

    [Fact]
    public void Bo2CSemantics()
    {
        // **此格可以放东西**
        Assert.True(EnvirObjectQueryCore.Bo2CSemantics());
        Assert.True(EnvirObjectQueryCore.Bo2CMeaningKnown());
        Assert.True(EnvirObjectQueryCore.ChFlagMustBeZero());
    }

    [Fact]
    public void GateVeto()
    {
        // **门永远否决、三档都一样**
        Assert.True(EnvirObjectQueryCore.GateAlwaysVetoes());
        Assert.True(EnvirObjectQueryCore.GateIsFour());
        Assert.True(EnvirObjectQueryCore.GateVetoIdenticalAcrossTiers());
        Assert.True(EnvirObjectQueryCore.ItemBranchIdenticalAcrossTiers());
    }

    // ===================== 五、共性 =====================

    [Fact]
    public void CommonLockPattern()
    {
        Assert.True(EnvirObjectQueryCore.CommonLockPattern());
        Assert.True(EnvirObjectQueryCore.FiveLockPatternParts());
        Assert.True(EnvirObjectQueryCore.WholeBlockVanishWhenSingleThread());
        Assert.Equal(5, EnvirObjectQueryCore.LockPatternParts.Length);
    }

    [Fact]
    public void UnconditionalInitialization()
    {
        Assert.True(EnvirObjectQueryCore.UnconditionalInitialization());
        Assert.True(EnvirObjectQueryCore.SevenInitialValues());
        Assert.True(EnvirObjectQueryCore.OnlyCollectorStartsAtZero());
        Assert.Equal(7, EnvirObjectQueryCore.InitialValues.Length);
    }

    [Fact]
    public void CellFlagRequirement()
    {
        // **只有取物品系列要求 chFlag = 0**
        Assert.True(EnvirObjectQueryCore.CellFlagRequiredOnlyByItems());
        Assert.True(EnvirObjectQueryCore.RequiresChFlagValues());
        Assert.True(EnvirObjectQueryCore.SameObjListCheck());
    }

    // ===================== 行数 =====================

    [Fact]
    public void LineCounts()
    {
        Assert.True(EnvirObjectQueryCore.SevenMethods());
        Assert.Equal(new[] { 44, 41, 42, 46, 52, 53, 44 }, EnvirObjectQueryCore.MethodLineCounts);
        Assert.True(EnvirObjectQueryCore.TotalLinesValues());
        Assert.Equal(322, EnvirObjectQueryCore.TotalLines());
    }

    [Fact]
    public void LongestAndShortest()
    {
        Assert.True(EnvirObjectQueryCore.LongestAndShortest());
        Assert.True(EnvirObjectQueryCore.ItemFamilyNotMonotonic());
    }

    [Fact]
    public void FamilySums()
    {
        // **取对象合计 173、取物品合计 149**
        Assert.True(EnvirObjectQueryCore.FamilySums());
        Assert.True(EnvirObjectQueryCore.MovingFamilyIsLonger());
        Assert.True(EnvirObjectQueryCore.PerMethodVersusTotalDiffer());
    }
}
