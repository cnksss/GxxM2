using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J168：`TEnvirnoment` 物品/魔法放行判定与任务创建 1:1 测试。
/// **三个判定的"默认值两两相反"用真值表穷举固证、
/// 二分查找与线性查找结果一致性用穷举验证、单向钳制用负数反例。**
/// </summary>
public sealed class EnvirAllowQuestCoreTests
{
    // ===================== 常量与默认值 =====================

    [Fact]
    public void ThreeDefaults()
    {
        Assert.True(EnvirAllowQuestCore.ThreeDefaults());
        Assert.Equal(3, EnvirAllowQuestCore.Defaults.Length);
        Assert.True(EnvirAllowQuestCore.TwoTrueOneFalse());
        Assert.True(EnvirAllowQuestCore.DropDefaultsFalse());
        Assert.True(EnvirAllowQuestCore.StdAndMagicDefaultTrue());
    }

    [Fact]
    public void DefaultValues()
    {
        Assert.True(EnvirAllowQuestCore.Defaults[0].Default);    // AllowStdItems
        Assert.False(EnvirAllowQuestCore.Defaults[1].Default);   // AllowDropToBagItem
        Assert.True(EnvirAllowQuestCore.Defaults[2].Default);    // AllowMagics
    }

    [Fact]
    public void GateStructure()
    {
        // **掉落那个完全没有布尔门**
        Assert.True(EnvirAllowQuestCore.DropHasNoBooleanGate());
        Assert.True(EnvirAllowQuestCore.StdGateIsSeparateFlag());
        Assert.True(EnvirAllowQuestCore.BuildAndUseConditionsDiffer());
        Assert.True(EnvirAllowQuestCore.TwoFlagsFromMapFlag());
        Assert.True(EnvirAllowQuestCore.NoFlagForDrop());
    }

    [Fact]
    public void MapFlagSources()
    {
        Assert.True(EnvirAllowQuestCore.TwoMapFlagSources());
        Assert.True(EnvirAllowQuestCore.TwoDifferentNegationSpellings());
        Assert.Equal(2, EnvirAllowQuestCore.MapFlagSources.Length);
    }

    // ===================== 一、三个判定模型 =====================

    [Fact]
    public void SharedLoopBody()
    {
        Assert.True(EnvirAllowQuestCore.SameLoopBodyOps());
        Assert.True(EnvirAllowQuestCore.BreakNotExit());
        Assert.True(EnvirAllowQuestCore.BreakAndExitEquivalentHere());
        Assert.True(EnvirAllowQuestCore.BreakExitEquivalenceExhaustive());
        Assert.True(EnvirAllowQuestCore.SearchMatchesLinear());
    }

    [Fact]
    public void StdItemsCases()
    {
        Assert.True(EnvirAllowQuestCore.StdFlagOffAllows());
        Assert.True(EnvirAllowQuestCore.StdNullListAllows());
        Assert.True(EnvirAllowQuestCore.StdListedRejected());
        Assert.True(EnvirAllowQuestCore.StdUnlistedAllowed());
    }

    [Fact]
    public void DropCases()
    {
        Assert.True(EnvirAllowQuestCore.DropNullListRejects());
        Assert.True(EnvirAllowQuestCore.DropListedAllowed());
        Assert.True(EnvirAllowQuestCore.DropUnlistedRejected());
    }

    [Fact]
    public void MagicCases()
    {
        Assert.True(EnvirAllowQuestCore.MagicFlagOffAllows());
        Assert.True(EnvirAllowQuestCore.MagicNullListAllows());
        Assert.True(EnvirAllowQuestCore.MagicListedRejected());
    }

    [Fact]
    public void StdAndDropOpposite()
    {
        // **在同一个列表上两者结果恰好相反**
        Assert.True(EnvirAllowQuestCore.StdAndDropAreOpposite());

        var list = new List<int> { 5, 9 };
        Assert.True(EnvirAllowQuestCore.AllowStdItems(list, true, 5) == false);
        Assert.True(EnvirAllowQuestCore.AllowDropToBagItem(list, 5) == true);
    }

    [Fact]
    public void StdAndMagicIdentical()
    {
        // **物品与魔法判定真值完全一致**
        Assert.True(EnvirAllowQuestCore.StdAndMagicIdentical());
        Assert.True(EnvirAllowQuestCore.OnlyDropDiffers());
    }

    [Fact]
    public void SearchModel()
    {
        var list = new List<int> { 5, 9, 13 };
        Assert.True(EnvirAllowQuestCore.ContainsSorted(list, 5));
        Assert.True(EnvirAllowQuestCore.ContainsSorted(list, 13));
        Assert.False(EnvirAllowQuestCore.ContainsSorted(list, 7));
        Assert.False(EnvirAllowQuestCore.ContainsSorted(new List<int>(), 5));
    }

    // ===================== 二、列表构建与排序 =====================

    [Fact]
    public void Sorts()
    {
        Assert.True(EnvirAllowQuestCore.ThreeSortsPresent());
        Assert.Equal(3, EnvirAllowQuestCore.SortCount());
        Assert.True(EnvirAllowQuestCore.SortCountIsThree());
        Assert.True(EnvirAllowQuestCore.SortPreconditionHolds());
    }

    [Fact]
    public void SortingMatters()
    {
        // **未排序时二分确实会漏 —— 所以那三处 Sort 是必需的**
        Assert.True(EnvirAllowQuestCore.UnsortedWouldFailSilently());
        Assert.True(EnvirAllowQuestCore.UnsortedBinarySearchMisses());
        Assert.True(EnvirAllowQuestCore.SortedFindsIt());
    }

    [Fact]
    public void ObjectContainers()
    {
        Assert.True(EnvirAllowQuestCore.ObjectsAsIntContainers());
        Assert.True(EnvirAllowQuestCore.RoundTripThroughObject());
        Assert.Equal(12345, EnvirAllowQuestCore.RoundTrip(12345));
        Assert.Equal(-7, EnvirAllowQuestCore.RoundTrip(-7));
    }

    [Fact]
    public void Guards()
    {
        Assert.True(EnvirAllowQuestCore.StdGuardsByIndex());
        Assert.True(EnvirAllowQuestCore.MagicGuardsByNonNull());
        Assert.True(EnvirAllowQuestCore.MagicAlsoGuardsEmptyText());
        Assert.True(EnvirAllowQuestCore.ThreeDifferentGuards());
        Assert.True(EnvirAllowQuestCore.ThreeGuardStyles());
        Assert.Equal(3, EnvirAllowQuestCore.GuardStyles.Length);
    }

    [Fact]
    public void Separators()
    {
        Assert.True(EnvirAllowQuestCore.SameSeparatorSet());
        Assert.True(EnvirAllowQuestCore.FourSeparators());
        Assert.True(EnvirAllowQuestCore.DiffersFromJ166Separators());
        Assert.Equal(new[] { '|', '\\', '/', ',' }, EnvirAllowQuestCore.Separators);
    }

    // ===================== 三、CreateQuest =====================

    [Fact]
    public void FlagGate()
    {
        Assert.True(EnvirAllowQuestCore.NegativeFlagRejected());
        Assert.True(EnvirAllowQuestCore.ZeroFlagAccepted());
        Assert.True(EnvirAllowQuestCore.MinusOneRejected());
        Assert.True(EnvirAllowQuestCore.FlagAccepted(0));
        Assert.False(EnvirAllowQuestCore.FlagAccepted(-1));
    }

    [Fact]
    public void ValueClamp()
    {
        // **单向钳制：只压大、不抬负**
        Assert.True(EnvirAllowQuestCore.NValueClampIsOneSided());
        Assert.True(EnvirAllowQuestCore.ClampValues());
        Assert.True(EnvirAllowQuestCore.ZeroAndOnePassThrough());
        Assert.True(EnvirAllowQuestCore.NegativeValuePassesThrough());
        Assert.True(EnvirAllowQuestCore.OnlyZeroAndOneOrNegative());
    }

    [Fact]
    public void ClampBoundaries()
    {
        Assert.Equal(1, EnvirAllowQuestCore.ClampValue(5));
        Assert.Equal(1, EnvirAllowQuestCore.ClampValue(2));
        Assert.Equal(1, EnvirAllowQuestCore.ClampValue(1));
        Assert.Equal(0, EnvirAllowQuestCore.ClampValue(0));
        Assert.Equal(-1, EnvirAllowQuestCore.ClampValue(-1));
        Assert.Equal(-99, EnvirAllowQuestCore.ClampValue(-99));
    }

    [Fact]
    public void StarReplacement()
    {
        Assert.True(EnvirAllowQuestCore.StarBecomesEmpty());
        Assert.True(EnvirAllowQuestCore.StarToEmptyValues());
        Assert.Equal("", EnvirAllowQuestCore.StarToEmpty("*"));
        Assert.Equal("abc", EnvirAllowQuestCore.StarToEmpty("abc"));
        Assert.Equal("", EnvirAllowQuestCore.StarToEmpty(""));
    }

    [Fact]
    public void StarInconsistency()
    {
        // **第三个参数被用了两次：先用星号找 NPC、再把星号改成空串**
        Assert.True(EnvirAllowQuestCore.ThirdParamUsedTwice());
        Assert.True(EnvirAllowQuestCore.StarCreatesNpcNamedStar());
        Assert.True(EnvirAllowQuestCore.InconsistencyWhenStar());
        Assert.True(EnvirAllowQuestCore.ConsistentWhenNotStar());

        var (npc, stored) = EnvirAllowQuestCore.StarInconsistency();
        Assert.Equal("*", npc);
        Assert.Equal("", stored);
        Assert.NotEqual(npc, stored);
    }

    [Fact]
    public void ThirdReplacementDiscarded()
    {
        Assert.True(EnvirAllowQuestCore.StarReplacementOnThree());
        Assert.Equal(3, EnvirAllowQuestCore.StarReplacementCount());
        Assert.True(EnvirAllowQuestCore.ThreeStarReplacements());
        Assert.True(EnvirAllowQuestCore.ThirdReplacementDiscarded());
        Assert.True(EnvirAllowQuestCore.NoFieldForThirdString());
    }

    [Fact]
    public void RecordFields()
    {
        Assert.Equal(6, EnvirAllowQuestCore.QuestRecordFields.Length);
        Assert.True(EnvirAllowQuestCore.SixRecordFields());
        Assert.True(EnvirAllowQuestCore.OnlyTwoStringsStored());
        Assert.DoesNotContain("s2C", EnvirAllowQuestCore.QuestRecordFields);
    }

    [Fact]
    public void StringZeroMapName()
    {
        // **地图名用字符串零而不是空串**
        Assert.True(EnvirAllowQuestCore.MapNameIsStringZero());
        Assert.True(EnvirAllowQuestCore.MapNameIsNotEmpty());
        Assert.Equal("0", EnvirAllowQuestCore.FakeNpcMapName);
        Assert.NotEqual("", EnvirAllowQuestCore.FakeNpcMapName);
    }

    [Fact]
    public void TwoWaysToMeanNone()
    {
        Assert.True(EnvirAllowQuestCore.TwoWaysToMeanNone());
        Assert.True(EnvirAllowQuestCore.TwoNoneRepresentations());
        Assert.Equal(2, EnvirAllowQuestCore.NoneRepresentations.Length);
    }

    [Fact]
    public void HardcodedFields()
    {
        Assert.True(EnvirAllowQuestCore.NineHardcodedFields());
        Assert.True(EnvirAllowQuestCore.NineFakeNpcFields());
        Assert.Equal(9, EnvirAllowQuestCore.FakeNpcFields.Length);
    }

    [Fact]
    public void FieldComposition()
    {
        // **五个零值、两个布尔（我最初凭肉眼数成三个零、程序化清点为五个）**
        Assert.True(EnvirAllowQuestCore.FieldComposition());
        Assert.Equal(5, EnvirAllowQuestCore.ZeroValueFieldCount());
        Assert.True(EnvirAllowQuestCore.FiveZeroValueFields());
        Assert.Equal(2, EnvirAllowQuestCore.BooleanFieldCount());
        Assert.True(EnvirAllowQuestCore.TwoBooleanFields());
        Assert.True(EnvirAllowQuestCore.CompositionAddsUp());
    }

    [Fact]
    public void FilePath()
    {
        Assert.True(EnvirAllowQuestCore.FilePathIsRelativeDir());
        Assert.True(EnvirAllowQuestCore.PathEndsWithBackslash());
        Assert.Equal("MapQuest_def\\", EnvirAllowQuestCore.FakeNpcFields[6].Value);
    }

    [Fact]
    public void BooleanFlags()
    {
        // **隐藏为真、是任务为假**
        Assert.True(EnvirAllowQuestCore.HiddenIsTrue());
        Assert.True(EnvirAllowQuestCore.QuestFlagIsFalse());
        Assert.True(EnvirAllowQuestCore.HiddenButNotQuestFlagged());
    }

    [Fact]
    public void ExistingNpcNotCorrected()
    {
        // **找到已存在的 NPC 时九个字段全部跳过**
        Assert.True(EnvirAllowQuestCore.ExistingNpcNotCorrected());
        Assert.True(EnvirAllowQuestCore.NineFieldsOnlyOnCreate());
        Assert.True(EnvirAllowQuestCore.FieldsSetValues());
        Assert.Equal(0, EnvirAllowQuestCore.FieldsSet(true));
        Assert.Equal(9, EnvirAllowQuestCore.FieldsSet(false));
    }

    [Fact]
    public void OptimisationComment()
    {
        Assert.True(EnvirAllowQuestCore.FindIsLaterOptimisation());
        Assert.True(EnvirAllowQuestCore.CommentExplainsWhy());
        Assert.Contains("过多创建", EnvirAllowQuestCore.OptimisationComment, StringComparison.Ordinal);
    }

    [Fact]
    public void CreateResult()
    {
        Assert.True(EnvirAllowQuestCore.ReturnsTrueOnSuccess());
        Assert.True(EnvirAllowQuestCore.CreateResultValues());
        Assert.False(EnvirAllowQuestCore.CreateResult(-1, true));
        Assert.True(EnvirAllowQuestCore.CreateResult(0, true));
        Assert.False(EnvirAllowQuestCore.CreateResult(0, false));
    }

    [Fact]
    public void CreateSteps()
    {
        Assert.True(EnvirAllowQuestCore.FindNpcBeforeAlloc());
        Assert.True(EnvirAllowQuestCore.NineCreateSteps());
        Assert.Equal(9, EnvirAllowQuestCore.CreateSteps.Length);
    }

    // ===================== 四、与本工程其它手写二分的关系 =====================

    [Fact]
    public void SearchCopies()
    {
        Assert.True(EnvirAllowQuestCore.SixCopiesOfSameSearch());
        Assert.Equal(6, EnvirAllowQuestCore.SearchCopyCount());
        Assert.True(EnvirAllowQuestCore.SixCopies());
        Assert.True(EnvirAllowQuestCore.CopiesAddUp());
    }

    [Fact]
    public void TwoSearchStyles()
    {
        Assert.True(EnvirAllowQuestCore.TwoStylesOfSameSearch());
        Assert.True(EnvirAllowQuestCore.TwoSearchStyles());
        Assert.True(EnvirAllowQuestCore.SafeAreaStyleIsMoreConvoluted());
        Assert.Equal(2, EnvirAllowQuestCore.SearchStyles.Length);
        Assert.Equal(3, EnvirAllowQuestCore.SearchSites.Length);

        int total = 0;

        foreach (var (_, c) in EnvirAllowQuestCore.SearchSites)
        {
            total += c;
        }

        Assert.Equal(6, total);
    }

    // ===================== 行数 =====================

    [Fact]
    public void MethodLineCounts()
    {
        Assert.Equal(new[] { 31, 28, 29, 38 }, EnvirAllowQuestCore.MethodLineCounts);
        Assert.True(EnvirAllowQuestCore.FourMethods());
    }

    [Fact]
    public void JudgmentLineSpread()
    {
        // **三个判定几乎等长（极差三）**
        Assert.True(EnvirAllowQuestCore.ThreeJudgmentsNearlyEqual());
        Assert.Equal(3, EnvirAllowQuestCore.JudgmentLineSpread());
        Assert.True(EnvirAllowQuestCore.SpreadIsThree());
    }

    [Fact]
    public void Extremes()
    {
        Assert.True(EnvirAllowQuestCore.CreateQuestIsLongest());
        Assert.True(EnvirAllowQuestCore.DropIsShortest());
    }

    [Fact]
    public void TotalLines()
    {
        Assert.True(EnvirAllowQuestCore.TotalLinesValues());
        Assert.Equal(126, EnvirAllowQuestCore.TotalLines());
        Assert.Equal(88, EnvirAllowQuestCore.JudgmentLines());
        Assert.True(EnvirAllowQuestCore.JudgmentLinesIs88());
    }
}
