using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J162：`TEnvirnoment` 计数与合法性族 1:1 测试。
/// **重复条件的"只在这两个重载里"由程序化核对确认、
/// 门真值表用 32 组全枚举验证"带重复与去重逐组等价"、
/// 五个方法的严格度差异用边界输入交叉验证。**
/// </summary>
public sealed class EnvirCountValidateCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.True(EnvirCountValidateCore.ConstantsMatchSource());
        Assert.Equal(1, EnvirCountValidateCore.ObjActor);
        Assert.Equal(0, EnvirCountValidateCore.RC_PLAYOBJECT);
        Assert.Equal(1, EnvirCountValidateCore.RC_HEROOBJECT);
        Assert.Equal(150, EnvirCountValidateCore.RC_PLAYMOSTER);
    }

    [Fact]
    public void NpcRaceConstants()
    {
        Assert.True(EnvirCountValidateCore.ThreeNpcRaceValues());
        Assert.Equal(10, EnvirCountValidateCore.RC_NPC);
        Assert.Equal(15, EnvirCountValidateCore.RC_PEACENPC);
        Assert.Equal(50, EnvirCountValidateCore.RC_MERCHANT);
    }

    [Fact]
    public void MerchantIsAliasOfAnimal()
    {
        // **RC_MERCHANT = RC_ANIMAL（别名赋值）**
        Assert.True(EnvirCountValidateCore.MerchantIsAliasOfAnimal());
        Assert.True(EnvirCountValidateCore.MerchantAliasLinePresent());
        Assert.Contains("RC_ANIMAL", EnvirCountValidateCore.MerchantAliasLine);
    }

    [Fact]
    public void PlayMonsterValueDiverges()
    {
        // **配置对话框里是 60、Grobal2.pas 里是 150**
        Assert.True(EnvirCountValidateCore.PlayMonsterValueDiverges());
        Assert.True(EnvirCountValidateCore.TwoPlayMonsterValues());
        Assert.True(EnvirCountValidateCore.LiveValueIs150());
        Assert.Equal(new[] { 60, 150 }, EnvirCountValidateCore.PlayMonsterValues);
    }

    // ===================== 一、锁号 =====================

    [Fact]
    public void LockIds()
    {
        Assert.Equal(new[] { 30, 31, 30, 0, 41, 42, 47, 0 }, EnvirCountValidateCore.LockIds);
        Assert.True(EnvirCountValidateCore.LockIdsInThisBatch());
        Assert.Equal(new[] { 30, 31, 41, 42, 47 }, EnvirCountValidateCore.LockIdsInThisBatchSet);
    }

    [Fact]
    public void LockIdThirtyIsShared()
    {
        // **J159 枚举发现的现场：30 被两个方法共用**
        Assert.True(EnvirCountValidateCore.LockIdThirtyIsSharedBy());
        Assert.True(EnvirCountValidateCore.TwoLockThirtyMethods());
        Assert.True(EnvirCountValidateCore.SharedLockConsequence());
        Assert.Equal(2, EnvirCountValidateCore.LockThirtyMethods.Length);
    }

    [Fact]
    public void ManualAllocationEvidence()
    {
        Assert.True(EnvirCountValidateCore.ManualAllocationEvidence());
        Assert.True(EnvirCountValidateCore.LockIdDistribution());
    }

    [Fact]
    public void TwoMethodsUnlocked()
    {
        Assert.True(EnvirCountValidateCore.TwoMethodsUnlocked());
    }

    [Fact]
    public void LockGranularity()
    {
        // **两个 IsValid 把锁加在最内层循环里、每格一次**
        Assert.True(EnvirCountValidateCore.LockInsideDoubleLoop());
        Assert.True(EnvirCountValidateCore.LockPerCellNotPerCall());
        Assert.True(EnvirCountValidateCore.FortyNineLocksAtRadiusThree());
        Assert.Equal(49, EnvirCountValidateCore.LockCountAtRadiusThree());
    }

    [Fact]
    public void TwoLockGranularities()
    {
        Assert.True(EnvirCountValidateCore.TwoLockGranularities());
        Assert.True(EnvirCountValidateCore.TwoGranularityKinds());
        Assert.Equal(2, EnvirCountValidateCore.LockGranularities.Length);
    }

    // ===================== 二、重复条件 =====================

    [Fact]
    public void DuplicateCondition()
    {
        // **门里 not boTempFixedHideMode 出现两次**
        Assert.True(EnvirCountValidateCore.DuplicateConditionInGate());
        Assert.True(EnvirCountValidateCore.SixGateConditions());
        Assert.True(EnvirCountValidateCore.FirstAndLastConditionIdentical());
        Assert.Equal(6, EnvirCountValidateCore.GateConditions.Length);
    }

    [Fact]
    public void DuplicateIsRedundant()
    {
        // **去重后只有五条、且逐组等价**
        Assert.True(EnvirCountValidateCore.FiveDistinctConditions());
        Assert.Equal(5, EnvirCountValidateCore.DistinctGateConditions());
        Assert.True(EnvirCountValidateCore.SecondCheckIsTautology());
        Assert.True(EnvirCountValidateCore.DuplicateIsRedundant());
    }

    [Fact]
    public void DuplicateOnlyInTheseTwo()
    {
        // **只有这两个重载写重了（14 处的拆解）**
        Assert.True(EnvirCountValidateCore.DuplicateOnlyInTheseTwoOverloads());
        Assert.True(EnvirCountValidateCore.FourDuplicateLines());
        Assert.True(EnvirCountValidateCore.FourteenAccountsFor());
        Assert.Equal(14, EnvirCountValidateCore.TotalTempFixedHideModeOccurrences());
        Assert.Equal(new[] { 4430, 4432, 4478, 4480 }, EnvirCountValidateCore.DuplicateLines);
    }

    [Fact]
    public void GateEquivalent()
    {
        // **带重复与去重逐组等价（32 组）**
        Assert.True(EnvirCountValidateCore.GateEquivalent());
        Assert.True(EnvirCountValidateCore.GateLooksStricterThanItIs());
    }

    // ---------- boTempFixedHideMode 表达式 ----------

    [Fact]
    public void TempFixedHideExpression()
    {
        Assert.True(EnvirCountValidateCore.TempFixedHideModeExpression());
        Assert.True(EnvirCountValidateCore.TempFixedHideTickStrictPositive());
        Assert.True(EnvirCountValidateCore.TempFixedHideHorseLogic());
    }

    [Fact]
    public void TempFixedHideBoundaries()
    {
        // **计时严格大于零；骑马且非马主才算**
        Assert.False(EnvirCountValidateCore.TempFixedHideMode(0, false, false));
        Assert.True(EnvirCountValidateCore.TempFixedHideMode(1, false, false));
        Assert.True(EnvirCountValidateCore.TempFixedHideMode(0, true, false));
        Assert.False(EnvirCountValidateCore.TempFixedHideMode(0, true, true));
    }

    [Fact]
    public void OnlyThreeRacesCompute()
    {
        // **其它种族一律置假**
        Assert.True(EnvirCountValidateCore.OnlyThreeRacesCompute());
        Assert.True(EnvirCountValidateCore.OthersForcedFalse());
        Assert.True(EnvirCountValidateCore.PlayerRaceSet());
    }

    [Fact]
    public void InvitedToRideComment()
    {
        Assert.True(EnvirCountValidateCore.InvitedToRideCommentPresent());
        Assert.True(EnvirCountValidateCore.InvitedToRideCommentContent());
        Assert.Contains("被人邀请骑马", EnvirCountValidateCore.InvitedToRideComment);
    }

    [Fact]
    public void ChangeModeExTickArray()
    {
        // **array[0..13]、只用下标 1**
        Assert.True(EnvirCountValidateCore.ChangeModeExTickArraySize());
        Assert.Equal(14, EnvirCountValidateCore.ChangeModeExTickLength());
        Assert.True(EnvirCountValidateCore.OnlyIndexOneUsed());
        Assert.Equal(1, EnvirCountValidateCore.UsedIndex());
        Assert.True(EnvirCountValidateCore.IndexOneInRange());
    }

    // ---------- 两个重载的差异 ----------

    [Fact]
    public void TwoOverloadDifferences()
    {
        Assert.True(EnvirCountValidateCore.TwoOverloadsDifferByProperTarget());
        Assert.True(EnvirCountValidateCore.OnlyTwoDifferences());
        Assert.True(EnvirCountValidateCore.TwoOverloadDifferenceCount());
        Assert.Equal(2, EnvirCountValidateCore.OverloadDifferences.Length);
    }

    [Fact]
    public void FirstOverloadIsWeaker()
    {
        // **第二个重载多一条 IsProperTarget**
        Assert.True(EnvirCountValidateCore.FirstOverloadIsWeaker());
        Assert.True(EnvirCountValidateCore.GateWithDuplicate(true, true, true, true, true));
        Assert.False(EnvirCountValidateCore.GateWithProperTarget(true, true, true, true, true, false));
    }

    [Fact]
    public void CommentedResidue()
    {
        // **残留注释引用了第一个重载不存在的形参**
        Assert.True(EnvirCountValidateCore.CommentedResidueReferencesNonexistentParam());
        Assert.True(EnvirCountValidateCore.FirstOverloadHasNoAObject());
        Assert.True(EnvirCountValidateCore.ResidueContent());
    }

    // ---------- 与取对象族的对比 ----------

    [Fact]
    public void CountingStricterThanCollection()
    {
        Assert.True(EnvirCountValidateCore.CountingStricterThanCollection());
        Assert.True(EnvirCountValidateCore.TwoExtraConditionCount());
        Assert.Equal(2, EnvirCountValidateCore.TwoExtraConditions.Length);
        Assert.True(EnvirCountValidateCore.CollectionGateHasThree());
    }

    [Fact]
    public void HiddenModeCountedOut()
    {
        // **隐蔽模式的目标被计数排除、却仍被收走**
        Assert.True(EnvirCountValidateCore.HiddenModeCountedOut());
        Assert.True(EnvirCountValidateCore.TempFixedHideCountedOut());
        Assert.True(EnvirCountValidateCore.BothExcludeIt());
    }

    // ===================== 三、GetXYNpcObjCount =====================

    [Fact]
    public void NpcRaceSet()
    {
        Assert.True(EnvirCountValidateCore.NpcCountThreeRaces());
        Assert.True(EnvirCountValidateCore.NpcRaceSet());
        Assert.True(EnvirCountValidateCore.TwoRaceSetsDisjoint());
    }

    [Fact]
    public void NpcGateIsLooser()
    {
        // **只有两条条件、且不看四条**
        Assert.True(EnvirCountValidateCore.NpcCountIgnoresFourConditions());
        Assert.True(EnvirCountValidateCore.NpcGateIsLooser());
        Assert.True(EnvirCountValidateCore.NpcGateHasTwoConditions());
        Assert.Equal(2, EnvirCountValidateCore.NpcGateConditionCount());
    }

    [Fact]
    public void DeadNpcStillCounted()
    {
        // **死掉的 NPC 仍被数进去（与玩家计数相反）**
        Assert.True(EnvirCountValidateCore.DeadNpcStillCounted());
        Assert.True(EnvirCountValidateCore.DeathDoesNotAffectNpcCount());
        Assert.True(EnvirCountValidateCore.PlayerCountRequiresAlive());
    }

    [Fact]
    public void NpcCountHasNoDuplicate()
    {
        Assert.True(EnvirCountValidateCore.NpcCountHasNoDuplicate());
    }

    // ===================== 四、IsCheapStuff =====================

    [Fact]
    public void MissingSemicolon()
    {
        // **那行没有结尾分号**
        Assert.True(EnvirCountValidateCore.MissingSemicolon());
        Assert.True(EnvirCountValidateCore.LineHasNoSemicolon());
        Assert.True(EnvirCountValidateCore.SingleLineBody());
        Assert.False(EnvirCountValidateCore.CheapStuffLine.EndsWith(";", StringComparison.Ordinal));
    }

    [Fact]
    public void NameUnrelatedToImplementation()
    {
        // **名字与实现完全不符**
        Assert.True(EnvirCountValidateCore.NameUnrelatedToImplementation());
        Assert.True(EnvirCountValidateCore.NameHasNoQuestHint());
        Assert.Equal("IsCheapStuff", EnvirCountValidateCore.CheapStuffName);
    }

    [Fact]
    public void QuestListNotEmpty()
    {
        Assert.True(EnvirCountValidateCore.QuestListNotEmptyValues());
        Assert.False(EnvirCountValidateCore.QuestListNotEmpty(0));
        Assert.True(EnvirCountValidateCore.QuestListNotEmpty(1));
    }

    [Fact]
    public void MisleadingNameFamily()
    {
        Assert.True(EnvirCountValidateCore.SameFamilyAsBo2B9AndTypo());
        Assert.True(EnvirCountValidateCore.ThreeMisleadingNames());
        Assert.Equal(3, EnvirCountValidateCore.MisleadingNameFamily.Length);
    }

    // ===================== 五、IsValidObject =====================

    [Fact]
    public void SkeletonGate()
    {
        // **只差一个骷髅判定**
        Assert.True(EnvirCountValidateCore.TwoMethodsDifferBySkeletonGate());
        Assert.True(EnvirCountValidateCore.SkeletonExcludedByEx());
        Assert.True(EnvirCountValidateCore.NonSkeletonBothTrue());
        Assert.True(EnvirCountValidateCore.SkeletonAddedInSameIf());
    }

    [Fact]
    public void SkeletonGateTruthTable()
    {
        Assert.True(EnvirCountValidateCore.IsValidObject(true, true));
        Assert.True(EnvirCountValidateCore.IsValidObjectEx(true, true, true));
        Assert.False(EnvirCountValidateCore.IsValidObjectEx(true, true, false));
        Assert.False(EnvirCountValidateCore.IsValidObject(false, true));
        Assert.False(EnvirCountValidateCore.IsValidObject(true, false));
    }

    [Fact]
    public void SkeletonField()
    {
        Assert.True(EnvirCountValidateCore.SkeletonFieldDeclaredInActor());
        Assert.True(EnvirCountValidateCore.SkeletonDeclarationKnown());
        Assert.True(EnvirCountValidateCore.SkeletonSetInMultiplePlaces());
        Assert.True(EnvirCountValidateCore.SixSkeletonAssignments());
        Assert.Equal(6, EnvirCountValidateCore.SkeletonAssignmentSites.Length);
    }

    [Fact]
    public void ContrastWithRangeFamily()
    {
        // **比指针相等、命中即退出**
        Assert.True(EnvirCountValidateCore.PointerIdentityComparison());
        Assert.True(EnvirCountValidateCore.ExitOnFirstMatch());
        Assert.True(EnvirCountValidateCore.ContrastWithRangeFamily());
        Assert.True(EnvirCountValidateCore.TwoContrastEntries());
    }

    [Fact]
    public void LockPlacement()
    {
        // **try/finally 在最内层、函数级没有 finally**
        Assert.True(EnvirCountValidateCore.ResultInitializedBeforeLoops());
        Assert.True(EnvirCountValidateCore.TryFinallyInsideInnermost());
        Assert.True(EnvirCountValidateCore.NoFunctionLevelFinally());
        Assert.True(EnvirCountValidateCore.EachIterationLocks());
        Assert.True(EnvirCountValidateCore.NineLocksAtRadiusOne());
    }

    // ===================== 六、GetXYHuman =====================

    [Fact]
    public void GetXYHumanGate()
    {
        // **只有一个条件：种族恰好是玩家**
        Assert.True(EnvirCountValidateCore.GetXYHumanSingleCondition());
        Assert.True(EnvirCountValidateCore.GetXYHumanGateValues());
        Assert.True(EnvirCountValidateCore.GetXYHumanGate(0));
        Assert.False(EnvirCountValidateCore.GetXYHumanGate(1));
    }

    [Fact]
    public void IgnoresGhostAndDeath()
    {
        Assert.True(EnvirCountValidateCore.IgnoresGhostAndDeath());
        Assert.True(EnvirCountValidateCore.HeroNotCounted());
        Assert.True(EnvirCountValidateCore.PlayMonsterNotCounted());
        Assert.True(EnvirCountValidateCore.BreakOnFirst());
    }

    [Fact]
    public void HumanIsLoosest()
    {
        // **一条对六条**
        Assert.True(EnvirCountValidateCore.OneConditionVersusSix());
        Assert.Equal(1, EnvirCountValidateCore.GetXYHumanConditionCount());
        Assert.True(EnvirCountValidateCore.HumanIsLoosest());
    }

    // ===================== 七、sub_4B5FC8 =====================

    [Fact]
    public void ChecksChFlagEqualsTwo()
    {
        // **唯一直接检查 chFlag = 2 的地方**
        Assert.True(EnvirCountValidateCore.ChecksChFlagEqualsTwo());
        Assert.True(EnvirCountValidateCore.IsBlockedValues());
        Assert.False(EnvirCountValidateCore.IsBlocked(true, 2));
        Assert.True(EnvirCountValidateCore.IsBlocked(true, 0));
    }

    [Fact]
    public void ConfirmsTwoIsBlocked()
    {
        // **与 SetMapXYFlag 配对**
        Assert.True(EnvirCountValidateCore.ConfirmsTwoIsBlocked());
        Assert.True(EnvirCountValidateCore.PairsWithSetMapXYFlag());
        Assert.True(EnvirCountValidateCore.RoundTripBlocked());
        Assert.True(EnvirCountValidateCore.RoundTripPassable());
    }

    [Fact]
    public void RoundTrip()
    {
        // **写 2 → 被阻挡；写 0 → 未阻挡**
        Assert.Equal(2, EnvirCountValidateCore.SetMapXYFlagWrites(false));
        Assert.Equal(0, EnvirCountValidateCore.SetMapXYFlagWrites(true));
        Assert.False(EnvirCountValidateCore.IsBlocked(true, EnvirCountValidateCore.SetMapXYFlagWrites(false)));
        Assert.True(EnvirCountValidateCore.IsBlocked(true, EnvirCountValidateCore.SetMapXYFlagWrites(true)));
    }

    [Fact]
    public void AddressDerivedName()
    {
        // **函数名是反编译地址**
        Assert.True(EnvirCountValidateCore.AddressDerivedName());
        Assert.True(EnvirCountValidateCore.SubNameShape());
        Assert.True(EnvirCountValidateCore.OnlySubPrefixedMethod());
        Assert.True(EnvirCountValidateCore.SameFamilyButMoreExtreme());
        Assert.Equal("sub_4B5FC8", EnvirCountValidateCore.SubName);
    }

    [Fact]
    public void AddressDerivedFamily()
    {
        Assert.True(EnvirCountValidateCore.ThreeAddressDerivedItems());
        Assert.Equal(3, EnvirCountValidateCore.AddressDerivedFamily.Length);
    }

    [Fact]
    public void NoLockOnSharedField()
    {
        Assert.True(EnvirCountValidateCore.NoLockOnSharedField());
        Assert.True(EnvirCountValidateCore.TwoSharedFieldNames());
        Assert.Equal(2, EnvirCountValidateCore.SharedFieldNames.Length);
    }

    [Fact]
    public void OutOfBoundsReturnsTrue()
    {
        // **界外视为"没有被阻挡"**
        Assert.True(EnvirCountValidateCore.OutOfBoundsReturnsTrue());
        Assert.True(EnvirCountValidateCore.OutOfBoundsReturnsTrueValues());
        Assert.True(EnvirCountValidateCore.OutOfBoundsIsPassable());
    }

    // ===================== 八、共性 =====================

    [Fact]
    public void SameDoublePrecondition()
    {
        Assert.True(EnvirCountValidateCore.SameDoublePrecondition());
        Assert.True(EnvirCountValidateCore.DoublePrecondition(true, true));
        Assert.False(EnvirCountValidateCore.DoublePrecondition(true, false));
    }

    [Fact]
    public void OnlyCellLevelQuery()
    {
        Assert.True(EnvirCountValidateCore.OnlyCellLevelQuery());
        Assert.True(EnvirCountValidateCore.DoesNotTouchObjectList());
        Assert.True(EnvirCountValidateCore.SevenToOne());
        Assert.Equal(7, EnvirCountValidateCore.ObjectListMethodCount());
        Assert.Equal(1, EnvirCountValidateCore.CellLevelMethodCount());
    }

    // ===================== 行数 =====================

    [Fact]
    public void MethodLineCounts()
    {
        Assert.Equal(new[] { 47, 49, 34, 5, 38, 38, 37, 9 }, EnvirCountValidateCore.MethodLineCounts);
        Assert.True(EnvirCountValidateCore.EightMethods());
    }

    [Fact]
    public void LineExtremes()
    {
        Assert.True(EnvirCountValidateCore.SecondOverloadIsLongest());
        Assert.True(EnvirCountValidateCore.CheapStuffIsShortest());
        Assert.True(EnvirCountValidateCore.TwoIsValidTied());
        Assert.True(EnvirCountValidateCore.TiedDespiteExtraCondition());
        Assert.True(EnvirCountValidateCore.TwoCountOverloadsDifferByTwo());
    }

    [Fact]
    public void TotalLines()
    {
        Assert.True(EnvirCountValidateCore.TotalLinesValues());
        Assert.Equal(257, EnvirCountValidateCore.TotalLines());
    }
}
