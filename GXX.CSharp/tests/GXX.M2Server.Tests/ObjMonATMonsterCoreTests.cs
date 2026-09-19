using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J200：`ObjMon.pas` 中 `TATMonster` 1:1 测试（三方法合计 32 行）。
/// **本批最重要的发现是"两个搜索计时字段被混用"** ——
/// `Create` 设 `m_dwSearchTime`、而 `Run` 读的是另一个字段
/// `m_dwSearchEnemyTick`（偏移 0x360 与 0x400）。
/// **若只读 `Create` 就断言"搜索间隔 1500..2999 毫秒"、结论就是错的**
/// （真实阈值是硬编码的 1 秒 / 8 秒两档）。
/// </summary>
public sealed class ObjMonATMonsterCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(1466, ObjMonATMonsterCore.ClassCommentLine);
        Assert.Equal(1467, ObjMonATMonsterCore.CreateStart);
        Assert.Equal(5, ObjMonATMonsterCore.CreateLines);
        Assert.Equal(1473, ObjMonATMonsterCore.DestroyStart);
        Assert.Equal(4, ObjMonATMonsterCore.DestroyLines);
        Assert.Equal(1478, ObjMonATMonsterCore.RunStart);
        Assert.Equal(1500, ObjMonATMonsterCore.RunEnd);
        Assert.Equal(23, ObjMonATMonsterCore.RunLines);
        Assert.Equal(32, ObjMonATMonsterCore.TotalLines);
        Assert.Equal(32, ObjMonATMonsterCore.ClassDeclStart);
        Assert.Equal(1500, ObjMonATMonsterCore.SearchTimeRandomBound);
        Assert.Equal(1500, ObjMonATMonsterCore.SearchTimeBase);
        Assert.Equal(1500, ObjMonATMonsterCore.SearchTimeMin);
        Assert.Equal(2999, ObjMonATMonsterCore.SearchTimeMax);
        Assert.Equal(8000, ObjMonATMonsterCore.RescanWithTargetMs);
        Assert.Equal(1000, ObjMonATMonsterCore.RescanWithoutTargetMs);
        Assert.Equal(20, ObjMonATMonsterCore.TargetKeepRadius);
        Assert.Equal(16, ObjMonATMonsterCore.SearchTimeAssignCount);
        Assert.Equal(0, ObjMonATMonsterCore.SearchTimeReadCountInFile);
        Assert.Equal(14, ObjMonATMonsterCore.DominantFormCount);
        Assert.Equal(39, ObjMonATMonsterCore.SearchEnemyTickSites);
        Assert.Equal(5, ObjMonATMonsterCore.SearchTimeFormCount);
        Assert.Equal(7, ObjMonATMonsterCore.SevenFoldGuardCount);
        Assert.Equal(3, ObjMonATMonsterCore.EmptyBlockIdiomCount);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonATMonsterCore.SpanMatches());
        Assert.True(ObjMonATMonsterCore.TotalLinesAddUp());
        Assert.True(ObjMonATMonsterCore.StartsAscending());
        Assert.True(ObjMonATMonsterCore.CommentBeforeCreate());
        Assert.True(ObjMonATMonsterCore.WithinUnit());
        Assert.True(ObjMonATMonsterCore.NoInstrumentation());
    }

    // ===================== 一、两个字段被混用 =====================

    [Fact]
    public void TwoTimerFieldsAreDistinct()
    {
        Assert.True(ObjMonATMonsterCore.CreateSetsSearchTime());
        Assert.True(ObjMonATMonsterCore.RunUsesSearchEnemyTick());
        Assert.True(ObjMonATMonsterCore.TwoDifferentFields());
        Assert.True(ObjMonATMonsterCore.ThreeFieldsThreeOffsets());
        Assert.True(ObjMonATMonsterCore.OffsetsAllDistinct());
        Assert.True(ObjMonATMonsterCore.SearchTimeNeverReadHere());
        Assert.True(ObjMonATMonsterCore.RealReadersElsewhere());
        Assert.True(ObjMonATMonsterCore.ReadersExtracted());
        Assert.True(ObjMonATMonsterCore.ReadersUseSearchTickNotEnemyTick());
        Assert.True(ObjMonATMonsterCore.WriteOnlyInThisClass());
        Assert.True(ObjMonATMonsterCore.DoNotInferFromAssignment());
        Assert.True(ObjMonATMonsterCore.SixteenAssignsZeroReads());
        Assert.True(ObjMonATMonsterCore.AssignSitesExtracted());
        Assert.True(ObjMonATMonsterCore.ThisClassAssignAt1470());
    }

    [Fact]
    public void Offsets()
    {
        Assert.Equal("0x360", ObjMonATMonsterCore.SearchTimeOffset);
        Assert.Equal("0x364", ObjMonATMonsterCore.SearchTickOffset);
        Assert.Equal("0x400", ObjMonATMonsterCore.SearchEnemyTickOffset);

        // **三个字段三个偏移、互不相同**
        Assert.NotEqual(ObjMonATMonsterCore.SearchTimeOffset,
            ObjMonATMonsterCore.SearchEnemyTickOffset);
        Assert.NotEqual(ObjMonATMonsterCore.SearchTimeOffset,
            ObjMonATMonsterCore.SearchTickOffset);
    }

    [Fact]
    public void AssignSitesAndReaders()
    {
        Assert.Equal(new[]
        {
            559, 796, 1470, 1529, 1713, 1841, 1853, 1981,
            2088, 2168, 2256, 2396, 2540, 3070, 5282, 5538,
        }, ObjMonATMonsterCore.SearchTimeAssignSites);

        Assert.Equal(6, ObjMonATMonsterCore.SearchTimeReaders.Length);
        Assert.Equal("ObjMon2.pas:1360", ObjMonATMonsterCore.SearchTimeReaders[0]);
        Assert.Equal("UsrEngn.pas:4242", ObjMonATMonsterCore.SearchTimeReaders[5]);

        // **读取点无一在本文件**
        for (int i = 0; i < ObjMonATMonsterCore.SearchTimeReaders.Length; i++)
        {
            Assert.DoesNotContain("ObjMon.pas",
                ObjMonATMonsterCore.SearchTimeReaders[i]);
        }
    }

    [Fact]
    public void FormsTable()
    {
        Assert.True(ObjMonATMonsterCore.FiveFormsInFile());
        Assert.True(ObjMonATMonsterCore.NoUnifiedPolicy());
        Assert.True(ObjMonATMonsterCore.FourteenIsDominantForm());
        Assert.True(ObjMonATMonsterCore.CopyPasteTemplate());

        Assert.Equal("3000 + Random(2000)", ObjMonATMonsterCore.SearchTimeForms[0]);
        Assert.Equal("Random(1500) + 1500", ObjMonATMonsterCore.SearchTimeForms[1]);
        Assert.Equal("Random(1500) + 500", ObjMonATMonsterCore.SearchTimeForms[2]);
        Assert.Equal("Random(1500) + 2500", ObjMonATMonsterCore.SearchTimeForms[3]);
    }

    // ===================== 二、随机区间 =====================

    [Fact]
    public void SearchTimeRangeBoundaries()
    {
        Assert.True(ObjMonATMonsterCore.RangeIs1500To2999());
        Assert.True(ObjMonATMonsterCore.UpperBoundIs2999Not3000());
        Assert.True(ObjMonATMonsterCore.RandomExclusiveUpper());
        Assert.True(ObjMonATMonsterCore.MinValueIs1500());
        Assert.True(ObjMonATMonsterCore.MaxValueIs2999());
        Assert.True(ObjMonATMonsterCore.AllValuesInRange());
        Assert.True(ObjMonATMonsterCore.NeverReaches3000());

        // **取值为 1500..2999**
        Assert.Equal(1500, ObjMonATMonsterCore.SearchTimeValue(0));
        Assert.Equal(2999, ObjMonATMonsterCore.SearchTimeValue(1499));

        // **上界是 2999、不是 3000**
        Assert.NotEqual(3000, ObjMonATMonsterCore.SearchTimeValue(1499));
    }

    // ===================== 三、空分支与七重合取 =====================

    [Fact]
    public void EmptyBranchAndGuard()
    {
        Assert.True(ObjMonATMonsterCore.EmptyThenBranchAgain());
        Assert.True(ObjMonATMonsterCore.SameIdiomAsJ198());
        Assert.True(ObjMonATMonsterCore.ThirdOccurrenceInFile());
        Assert.True(ObjMonATMonsterCore.SevenFoldGuard());
        Assert.True(ObjMonATMonsterCore.SameMapRequired());
        Assert.True(ObjMonATMonsterCore.RadiusTwentyAgain());
        Assert.True(ObjMonATMonsterCore.SameRadiusAsJ198());
        Assert.True(ObjMonATMonsterCore.SevenFoldExtracted());
        Assert.True(ObjMonATMonsterCore.FourValidityConditions());
        Assert.True(ObjMonATMonsterCore.AllSevenPass());
        Assert.True(ObjMonATMonsterCore.NoFlagInvalid());
        Assert.True(ObjMonATMonsterCore.DifferentMapInvalid());
        Assert.True(ObjMonATMonsterCore.BeyondRadiusInvalid());
        Assert.True(ObjMonATMonsterCore.ExactlyTwentyValid());
        Assert.True(ObjMonATMonsterCore.DeadInvalid());
        Assert.True(ObjMonATMonsterCore.OnlyDecidesRescan());
        Assert.True(ObjMonATMonsterCore.MovementDelegatedToBase());
    }

    [Fact]
    public void TargetValidityBoundaries()
    {
        Assert.Equal("m_boTarget", ObjMonATMonsterCore.SevenFoldConditions[0]);
        Assert.Equal("AbsDy20", ObjMonATMonsterCore.SevenFoldConditions[6]);

        // **恰好二十格仍有效、二十一格无效**
        Assert.True(ObjMonATMonsterCore.IsTargetStillValid(
            true, false, false, false, true, 20, 20));
        Assert.False(ObjMonATMonsterCore.IsTargetStillValid(
            true, false, false, false, true, 21, 0));
        Assert.False(ObjMonATMonsterCore.IsTargetStillValid(
            true, false, false, false, true, 0, 21));

        // **两轴都必须在半径内**
        Assert.True(ObjMonATMonsterCore.IsTargetStillValid(
            true, false, false, false, true, -20, -20));
    }

    // ===================== 四、两档阈值 =====================

    [Fact]
    public void TwoTierRescan()
    {
        Assert.True(ObjMonATMonsterCore.TwoTierThreshold());
        Assert.True(ObjMonATMonsterCore.EightSecondsWithTarget());
        Assert.True(ObjMonATMonsterCore.OneSecondWithoutTarget());
        Assert.True(ObjMonATMonsterCore.RetargetIsLazy());
        Assert.True(ObjMonATMonsterCore.NoMiddleTier());
        Assert.True(ObjMonATMonsterCore.StaleTargetKeptUpTo8s());
    }

    [Fact]
    public void RescanBoundaries()
    {
        // **无目标：一秒后重搜（严格大于）**
        Assert.True(ObjMonATMonsterCore.ShouldRescan(0, 1001, true));
        Assert.False(ObjMonATMonsterCore.ShouldRescan(0, 1000, true));
        Assert.True(ObjMonATMonsterCore.NoTargetRescansAfter1s());
        Assert.True(ObjMonATMonsterCore.NoTargetNotYetAt1s());

        // **有目标：一秒不重搜、八秒后重搜**
        Assert.False(ObjMonATMonsterCore.ShouldRescan(0, 1001, false));
        Assert.True(ObjMonATMonsterCore.ShouldRescan(0, 8001, false));
        Assert.False(ObjMonATMonsterCore.ShouldRescan(0, 8000, false));
        Assert.True(ObjMonATMonsterCore.WithTargetNotAt1s());
        Assert.True(ObjMonATMonsterCore.WithTargetRescansAfter8s());
        Assert.True(ObjMonATMonsterCore.WithTargetNotAtExactly8s());

        // **无目标时八秒当然也重搜**
        Assert.True(ObjMonATMonsterCore.NoTargetAlsoRescansAt8s());
    }

    [Fact]
    public void RawSubtractionFacts()
    {
        Assert.True(ObjMonATMonsterCore.RawSubtractionAgain());
        Assert.True(ObjMonATMonsterCore.NoWraparoundCompensation());
        Assert.True(ObjMonATMonsterCore.ThreeClassesTwoStyles());
        Assert.True(ObjMonATMonsterCore.TickReadTwice());
        Assert.True(ObjMonATMonsterCore.SameExpressionTwoReads());
    }

    // ===================== 五、与基类的分工 =====================

    [Fact]
    public void DelegationFacts()
    {
        Assert.True(ObjMonATMonsterCore.DestroyEmptyShellAgain());
        Assert.True(ObjMonATMonsterCore.ThirdOccurrence());
        Assert.True(ObjMonATMonsterCore.SearchTargetProvidedElsewhere());
        Assert.True(ObjMonATMonsterCore.CalledButNotDefinedHere());
        Assert.True(ObjMonATMonsterCore.NoThinkOverrideAgain());
        Assert.True(ObjMonATMonsterCore.RunCarriesExtraDuty());
        Assert.True(ObjMonATMonsterCore.SameFourFoldGuard());
        Assert.True(ObjMonATMonsterCore.VerbatimWithJ199());
        Assert.True(ObjMonATMonsterCore.CopiedAcrossClasses());
        Assert.True(ObjMonATMonsterCore.SiblingRepeatsTemplate());
        Assert.True(ObjMonATMonsterCore.VerbatimAt1529());
    }

    [Fact]
    public void FourFoldGuardBoundaries()
    {
        Assert.True(ObjMonATMonsterCore.AllFourPass());
        Assert.True(ObjMonATMonsterCore.DeathBlocks());
        Assert.True(ObjMonATMonsterCore.Bo554Blocks());
        Assert.True(ObjMonATMonsterCore.GhostBlocks());
        Assert.True(ObjMonATMonsterCore.CannotMoveBlocks());

        Assert.True(ObjMonATMonsterCore.CanRun(false, false, false, true));
        Assert.False(ObjMonATMonsterCore.CanRun(true, false, false, true));
        Assert.False(ObjMonATMonsterCore.CanRun(false, true, false, true));
        Assert.False(ObjMonATMonsterCore.CanRun(false, false, true, true));
        Assert.False(ObjMonATMonsterCore.CanRun(false, false, false, false));
    }
}
