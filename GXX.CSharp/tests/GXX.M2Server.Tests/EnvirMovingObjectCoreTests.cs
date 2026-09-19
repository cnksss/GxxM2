using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J171：`TEnvirnoment` 移动对象查询族与坐标对象计数族 1:1 测试。
/// **`boFlag` 真值表、权限豁免三条件缺一不可、重复条件冗余性穷举、
/// 临时定身三变体全部由模型实测。**
/// </summary>
public sealed class EnvirMovingObjectCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(1, EnvirMovingObjectCore.ObjActor);
        Assert.Equal(0, EnvirMovingObjectCore.RcPlayObject);
        Assert.Equal(1, EnvirMovingObjectCore.RcHeroObject);
        Assert.Equal(150, EnvirMovingObjectCore.RcPlayMoster);
        Assert.Equal(50, EnvirMovingObjectCore.RcMerchant);
        Assert.Equal(10, EnvirMovingObjectCore.RcNpc);
        Assert.Equal(15, EnvirMovingObjectCore.RcPeaceNpc);
    }

    [Fact]
    public void RaceSets()
    {
        Assert.True(EnvirMovingObjectCore.ThreeRaces());
        Assert.True(EnvirMovingObjectCore.InThreeRaces(0));
        Assert.True(EnvirMovingObjectCore.InThreeRaces(1));
        Assert.True(EnvirMovingObjectCore.InThreeRaces(150));
        Assert.False(EnvirMovingObjectCore.InThreeRaces(10));
    }

    // ===================== 一、boFlag 语义 =====================

    [Fact]
    public void BoFlagSemantics()
    {
        Assert.True(EnvirMovingObjectCore.BoFlagTrueExcludesDead());
        Assert.True(EnvirMovingObjectCore.BoFlagFalseIncludesDead());
        Assert.True(EnvirMovingObjectCore.BoFlagValues());
    }

    [Fact]
    public void DeathClampTruthTable()
    {
        // **真即排除死者（与收集族的 IncDeathObject 同义）**
        Assert.False(EnvirMovingObjectCore.DeathAllowed(true, true));
        Assert.True(EnvirMovingObjectCore.DeathAllowed(true, false));
        Assert.True(EnvirMovingObjectCore.DeathAllowed(false, true));
        Assert.True(EnvirMovingObjectCore.DeathAllowed(false, false));
    }

    [Fact]
    public void TwoNamesOneMeaning()
    {
        Assert.True(EnvirMovingObjectCore.SameAsCollectorButNeutralName());
        Assert.True(EnvirMovingObjectCore.TwoNamesOneMeaning());
        Assert.True(EnvirMovingObjectCore.EquivalentToCollectorClamp());
        Assert.True(EnvirMovingObjectCore.TwoDeathClampSpellings());
        Assert.Equal(2, EnvirMovingObjectCore.DeathClampSpellings.Length);
    }

    [Fact]
    public void StagedEditing()
    {
        Assert.True(EnvirMovingObjectCore.ExtraParenthesisation());
        Assert.True(EnvirMovingObjectCore.StagedEditing());
        Assert.True(EnvirMovingObjectCore.ThreeCommentMarkers());
        Assert.Equal(3, EnvirMovingObjectCore.CommentMarkers.Length);
    }

    // ===================== 二、四个重载 =====================

    [Fact]
    public void OverloadStructure()
    {
        Assert.True(EnvirMovingObjectCore.FourOverloadsGrowingConditions());
        Assert.Equal(4, EnvirMovingObjectCore.OverloadCount());
        Assert.True(EnvirMovingObjectCore.FourOverloads());
    }

    [Fact]
    public void OverloadBehaviours()
    {
        Assert.True(EnvirMovingObjectCore.FirstCollectsAll());
        Assert.True(EnvirMovingObjectCore.SecondTakesFirst());
        Assert.True(EnvirMovingObjectCore.ThirdAddsIdentity());
        Assert.True(EnvirMovingObjectCore.ExAddsProperTarget());
        Assert.True(EnvirMovingObjectCore.ExAddsPermissionExemption());
    }

    [Fact]
    public void ConditionSpectrum()
    {
        Assert.True(EnvirMovingObjectCore.ConditionSpectrum());
        Assert.True(EnvirMovingObjectCore.FourConditionEntries());
        Assert.True(EnvirMovingObjectCore.FirstTwoEqual());
        Assert.True(EnvirMovingObjectCore.LastTwoIncrease());
        Assert.Equal(new[] { 4, 4, 5, 6 }, new[]
        {
            EnvirMovingObjectCore.ConditionCounts[0].Conditions,
            EnvirMovingObjectCore.ConditionCounts[1].Conditions,
            EnvirMovingObjectCore.ConditionCounts[2].Conditions,
            EnvirMovingObjectCore.ConditionCounts[3].Conditions,
        });
    }

    [Fact]
    public void ContinueStyle()
    {
        // **只有 Ex 用 Continue**
        Assert.True(EnvirMovingObjectCore.OnlyExUsesContinue());
        Assert.True(EnvirMovingObjectCore.EquivalentButExtraStep());
    }

    // ---------- 权限豁免 ----------

    [Fact]
    public void PermissionConstants()
    {
        Assert.True(EnvirMovingObjectCore.PermissionThresholdIsTen());
        Assert.True(EnvirMovingObjectCore.DefaultStartPermissionIsZero());
        Assert.True(EnvirMovingObjectCore.ExemptionAlwaysActiveByDefault());
        Assert.Equal(10, EnvirMovingObjectCore.PermissionThreshold);
        Assert.Equal(0, EnvirMovingObjectCore.DefaultStartPermission);
    }

    [Fact]
    public void ExemptionValues()
    {
        Assert.True(EnvirMovingObjectCore.ExemptValues());
        Assert.True(EnvirMovingObjectCore.NotExemptedLowPermission());
        Assert.True(EnvirMovingObjectCore.NotExemptedHighStart());
        Assert.True(EnvirMovingObjectCore.NotExemptedNonPlayer());
    }

    [Fact]
    public void ExemptionNeedsAllThree()
    {
        // **起始权限、种族、权限三者缺一不可**
        Assert.True(EnvirMovingObjectCore.ExemptionNeedsAllThree());

        Assert.True(EnvirMovingObjectCore.Exempted(0, 0, 10));
        Assert.False(EnvirMovingObjectCore.Exempted(10, 0, 10));
        Assert.False(EnvirMovingObjectCore.Exempted(0, 10, 99));
        Assert.False(EnvirMovingObjectCore.Exempted(0, 0, 9));
    }

    // ---------- 收集过滤 ----------

    [Fact]
    public void CollectFilter()
    {
        Assert.True(EnvirMovingObjectCore.CollectDeathBehavior());
        Assert.True(EnvirMovingObjectCore.CollectGhostRejected());
        Assert.True(EnvirMovingObjectCore.CollectRequiresBo2B9());
        Assert.True(EnvirMovingObjectCore.CollectNonActorRejected());
    }

    [Fact]
    public void CollectFilterValues()
    {
        // 合格
        Assert.True(EnvirMovingObjectCore.CollectFilter(1, false, true, false, false));

        // 死者：boFlag 假则收、真则拒
        Assert.True(EnvirMovingObjectCore.CollectFilter(1, false, true, false, true));
        Assert.False(EnvirMovingObjectCore.CollectFilter(1, false, true, true, true));

        // 幽灵 / 无 bo2B9 / 非演员
        Assert.False(EnvirMovingObjectCore.CollectFilter(1, true, true, false, false));
        Assert.False(EnvirMovingObjectCore.CollectFilter(1, false, false, false, false));
        Assert.False(EnvirMovingObjectCore.CollectFilter(2, false, true, false, false));
    }

    // ===================== 三、重复条件 =====================

    [Fact]
    public void DuplicatedCondition()
    {
        Assert.True(EnvirMovingObjectCore.DuplicatedConditionInFirstOverload());
        Assert.Equal(2, EnvirMovingObjectCore.DuplicateCount());
        Assert.True(EnvirMovingObjectCore.TwoOccurrences());
        Assert.True(EnvirMovingObjectCore.LogicallyRedundant());
        Assert.True(EnvirMovingObjectCore.RedundancyHolds());
    }

    [Fact]
    public void SecondOverloadUsesTarget()
    {
        Assert.True(EnvirMovingObjectCore.SecondOverloadUsesAObject());
        Assert.True(EnvirMovingObjectCore.CommentedResidueLine());
        Assert.True(EnvirMovingObjectCore.ResidueIsTruncated());
        Assert.True(EnvirMovingObjectCore.HalfFinishedEdit());
        Assert.EndsWith(".i", EnvirMovingObjectCore.ResidueComment, StringComparison.Ordinal);
    }

    [Fact]
    public void CountFilterValues()
    {
        Assert.True(EnvirMovingObjectCore.CountFilterValues());

        Assert.True(EnvirMovingObjectCore.CountFilter(0, false, true, false, false, false));
        Assert.False(EnvirMovingObjectCore.CountFilter(0, true, true, false, false, false));
        Assert.False(EnvirMovingObjectCore.CountFilter(0, false, true, true, false, false));
        Assert.False(EnvirMovingObjectCore.CountFilter(0, false, true, false, true, false));
        Assert.False(EnvirMovingObjectCore.CountFilter(0, false, true, false, false, true));
    }

    // ---------- 临时定身三变体 ----------

    [Fact]
    public void TempHideVariants()
    {
        Assert.True(EnvirMovingObjectCore.ThreeVariants());
        Assert.True(EnvirMovingObjectCore.ThreeTempHideVariants());
        Assert.Equal(3, EnvirMovingObjectCore.TempHideVariants.Length);
    }

    [Fact]
    public void TempHideSites()
    {
        // **2029 处是短式、3089 处注释掉一半且判另一个字段**
        Assert.True(EnvirMovingObjectCore.Site2029ShortForm());
        Assert.True(EnvirMovingObjectCore.Site3089HalfCommented());
        Assert.True(EnvirMovingObjectCore.UsesDifferentField());
        Assert.True(EnvirMovingObjectCore.FieldNamesDiffer());
        Assert.Equal(2, EnvirMovingObjectCore.TwoFieldNames.Length);
    }

    [Fact]
    public void TempHideCounts()
    {
        Assert.Equal(12, EnvirMovingObjectCore.EnvirSiteCount());
        Assert.True(EnvirMovingObjectCore.TwelveSitesInEnvir());
        Assert.True(EnvirMovingObjectCore.ElevenIdenticalOneDiffers());
    }

    [Fact]
    public void ChangeModeTick()
    {
        Assert.True(EnvirMovingObjectCore.OnlyIndexOneUsed());
        Assert.Equal(14, EnvirMovingObjectCore.ChangeModeExTickLength());
        Assert.True(EnvirMovingObjectCore.OneOfFourteenUsed());
    }

    [Fact]
    public void TempHideModel()
    {
        // **三种族内且（变身计时或骑马非马主）**
        Assert.True(EnvirMovingObjectCore.TempFixedHideValues());

        Assert.True(EnvirMovingObjectCore.TempFixedHide(true, true, false, false));
        Assert.True(EnvirMovingObjectCore.TempFixedHide(true, false, true, false));
        Assert.False(EnvirMovingObjectCore.TempFixedHide(true, false, true, true));
        Assert.False(EnvirMovingObjectCore.TempFixedHide(false, true, true, false));
        Assert.False(EnvirMovingObjectCore.TempFixedHide(true, false, false, false));
    }

    // ===================== 五、NPC 计数 =====================

    [Fact]
    public void NpcCountStructure()
    {
        Assert.True(EnvirMovingObjectCore.NpcCountIsTwoConditions());
        Assert.True(EnvirMovingObjectCore.ThreeCountFunctionsConditions());
        Assert.True(EnvirMovingObjectCore.ThreeCountEntries());
        Assert.True(EnvirMovingObjectCore.NpcCountMuchLooser());
        Assert.True(EnvirMovingObjectCore.LastTwoDifferByOne());
        Assert.Equal(3, EnvirMovingObjectCore.CountConditions.Length);
    }

    [Fact]
    public void NpcCountValues()
    {
        Assert.True(EnvirMovingObjectCore.NpcCountValues());

        Assert.True(EnvirMovingObjectCore.NpcCountFilter(50, false));
        Assert.True(EnvirMovingObjectCore.NpcCountFilter(10, false));
        Assert.True(EnvirMovingObjectCore.NpcCountFilter(15, false));
        Assert.False(EnvirMovingObjectCore.NpcCountFilter(80, false));
        Assert.False(EnvirMovingObjectCore.NpcCountFilter(10, true));
    }

    [Fact]
    public void DedupedCounts()
    {
        // **六与七里各含一处重复，去重后是五与六**
        Assert.True(EnvirMovingObjectCore.BothCountsHaveDuplicate());
        Assert.True(EnvirMovingObjectCore.DedupedValues());
        Assert.Equal(new[] { 2, 5, 6 }, EnvirMovingObjectCore.DedupedCounts);
    }

    [Fact]
    public void LockCollision()
    {
        // **两个函数共用锁编号三十**
        Assert.True(EnvirMovingObjectCore.SharedLockId30());
        Assert.True(EnvirMovingObjectCore.LockIdCollision());
        Assert.True(EnvirMovingObjectCore.TwoLockId30Users());
        Assert.Equal(2, EnvirMovingObjectCore.LockId30Users.Length);
    }

    [Fact]
    public void MerchantAlias()
    {
        // **商人是动物的别名 —— 所以动物被算作 NPC**
        Assert.True(EnvirMovingObjectCore.MerchantIsAnimalAlias());
        Assert.True(EnvirMovingObjectCore.AliasEqual());
        Assert.True(EnvirMovingObjectCore.AnimalsCountAsNpc());
        Assert.Equal(50, EnvirMovingObjectCore.RcMerchant);
    }

    // ===================== 行数 =====================

    [Fact]
    public void LineCounts()
    {
        Assert.True(EnvirMovingObjectCore.SevenMethods());
        Assert.Equal(7, EnvirMovingObjectCore.MethodLineCounts.Length);
        Assert.Equal(new[] { 43, 40, 41, 45, 46, 48, 33 }, EnvirMovingObjectCore.MethodLineCounts);
    }

    [Fact]
    public void RelativeLengths()
    {
        Assert.True(EnvirMovingObjectCore.NpcCountIsShortest());
        Assert.True(EnvirMovingObjectCore.ObjectCountsLonger());
        Assert.True(EnvirMovingObjectCore.MovingSpreadIsFive());
        Assert.Equal(5, EnvirMovingObjectCore.MovingSpread());
    }

    [Fact]
    public void Totals()
    {
        Assert.True(EnvirMovingObjectCore.TotalLinesValues());
        Assert.Equal(296, EnvirMovingObjectCore.TotalLines());
        Assert.Equal(169, EnvirMovingObjectCore.MovingLines());
        Assert.True(EnvirMovingObjectCore.MovingLinesIs169());
        Assert.Equal(94, EnvirMovingObjectCore.CountLines());
        Assert.True(EnvirMovingObjectCore.CountLinesIs94());
    }
}
