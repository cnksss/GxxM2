using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J178：客户端动作默认帧与默认动作推进 1:1 测试。
/// **两条大分支、三态钳制折叠到零、六段外观号集合的区间边界、
/// 以及 DefaultMotion 的先比较后赋值。**
/// </summary>
public sealed class ClientDefaultFrameCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(10000, ClientDefaultFrameCore.CustomThreshold);
        Assert.Equal(156, ClientDefaultFrameCore.CustomMonsterRace);
        Assert.Equal(1, ClientDefaultFrameCore.StateStoneMode);
        Assert.Equal(3, ClientDefaultFrameCore.NpcDirModulo);
        Assert.Equal(246, ClientDefaultFrameCore.ExcludeLow);
        Assert.Equal(272, ClientDefaultFrameCore.ExcludeHigh);
        Assert.Equal(4000, ClientDefaultFrameCore.WarModeTimeout);
        Assert.Equal(60000, ClientDefaultFrameCore.LoadThrottle);
    }

    [Fact]
    public void ShiftArguments()
    {
        Assert.Equal(new[] { 0, 0, 1, 1 }, ClientDefaultFrameCore.ShiftArgs);
        Assert.True(ClientDefaultFrameCore.ShiftArgsAreDirZeroOneOne());
        Assert.True(ClientDefaultFrameCore.StepIsZero());
        Assert.True(ClientDefaultFrameCore.LimitIsOne());
    }

    // ===================== 一、两条大分支 =====================

    [Fact]
    public void BranchShape()
    {
        Assert.True(ClientDefaultFrameCore.Race156SingleElementSet());
        Assert.True(ClientDefaultFrameCore.TwoMutuallyExclusiveBranches());
        Assert.True(ClientDefaultFrameCore.SingleElementSetInsteadOfEquality());
    }

    [Fact]
    public void BranchSelection()
    {
        Assert.True(ClientDefaultFrameCore.BranchSelection());
        Assert.True(ClientDefaultFrameCore.ChangeApprMustBeNonNegative());

        Assert.Equal(1, ClientDefaultFrameCore.Branch(156, 0));
        Assert.Equal(1, ClientDefaultFrameCore.Branch(156, 5));
        Assert.Equal(2, ClientDefaultFrameCore.Branch(156, -1));
        Assert.Equal(2, ClientDefaultFrameCore.Branch(155, 0));
    }

    // ---------- 三态钳制 ----------

    [Fact]
    public void ClampShape()
    {
        Assert.True(ClientDefaultFrameCore.ThreeWayClamp());
        Assert.True(ClientDefaultFrameCore.OutOfRangeFoldsToZero());
        Assert.True(ClientDefaultFrameCore.NotClampedToUpperBound());
        Assert.True(ClientDefaultFrameCore.SameClampInBothBranches());
    }

    [Fact]
    public void ClampValues()
    {
        Assert.True(ClientDefaultFrameCore.ClampValues());

        // **负值与越界值都折叠到零**
        Assert.Equal(0, ClientDefaultFrameCore.ClampFrame(-5, 10));
        Assert.Equal(0, ClientDefaultFrameCore.ClampFrame(0, 10));
        Assert.Equal(9, ClientDefaultFrameCore.ClampFrame(9, 10));
        Assert.Equal(0, ClientDefaultFrameCore.ClampFrame(10, 10));
        Assert.Equal(0, ClientDefaultFrameCore.ClampFrame(99, 10));
    }

    [Fact]
    public void FoldsToZeroNotLimit()
    {
        Assert.True(ClientDefaultFrameCore.FoldsToZeroNotLimit());
        Assert.NotEqual(9, ClientDefaultFrameCore.ClampFrame(99, 10));
    }

    // ---------- 死亡与石化 ----------

    [Fact]
    public void DeathAndStoneShape()
    {
        Assert.True(ClientDefaultFrameCore.DeathUsesLastFrame());
        Assert.True(ClientDefaultFrameCore.DeathMinusOne());
        Assert.True(ClientDefaultFrameCore.StandUsesCurrentFrame());
        Assert.True(ClientDefaultFrameCore.StoneIgnoresCurrentFrame());
        Assert.True(ClientDefaultFrameCore.StoneStaysAtFirstFrame());
        Assert.True(ClientDefaultFrameCore.SkeletonSingleFrame());
    }

    [Fact]
    public void DeathIndex()
    {
        Assert.True(ClientDefaultFrameCore.DeathIndexUsesLast());
        Assert.Equal(3, ClientDefaultFrameCore.DeathIndex(0, 0, 4, 2));
        // **末帧 = 起始 + 方向*宽度 + (播放数-1)**
        Assert.Equal(100 + 12 + 3, ClientDefaultFrameCore.DeathIndex(100, 2, 4, 2));
    }

    [Fact]
    public void StandIndex()
    {
        Assert.True(ClientDefaultFrameCore.StandIndexUsesCurrent());
        Assert.Equal(102, ClientDefaultFrameCore.FinalIndex(100, 0, 4, 2, 2));
    }

    [Fact]
    public void StoneAndSkeletonIndex()
    {
        Assert.True(ClientDefaultFrameCore.StoneIndexNoFrameTerm());
        Assert.Equal(112, ClientDefaultFrameCore.StoneIndex(100, 2, 4, 2));
        Assert.True(ClientDefaultFrameCore.SkeletonIndexValues());
        Assert.Equal(50, ClientDefaultFrameCore.SkeletonIndex(50));
    }

    [Fact]
    public void DirStride()
    {
        Assert.True(ClientDefaultFrameCore.DirStrideIsPlayPlusEmpty());
        Assert.True(ClientDefaultFrameCore.DirStrideValues());
        Assert.Equal(6, ClientDefaultFrameCore.DirStride(4, 2));
    }

    [Fact]
    public void CalcDir()
    {
        Assert.True(ClientDefaultFrameCore.CalcDirGatesDirection());
        Assert.True(ClientDefaultFrameCore.CalcDirFalseMeansZero());
        Assert.True(ClientDefaultFrameCore.EffectiveDirValues());
        Assert.Equal(3, ClientDefaultFrameCore.EffectiveDir(true, 3));
        Assert.Equal(0, ClientDefaultFrameCore.EffectiveDir(false, 3));
    }

    [Fact]
    public void NilRace()
    {
        Assert.True(ClientDefaultFrameCore.NilRaceReturnsZero());
        Assert.True(ClientDefaultFrameCore.ExitAfterZeroInit());
    }

    // ===================== 二、计数的赋值点 =====================

    [Fact]
    public void CountSites()
    {
        Assert.True(ClientDefaultFrameCore.DefFrameCountAssignedSixTimes());
        Assert.Equal(6, ClientDefaultFrameCore.DefFrameCountAssignments());
        Assert.True(ClientDefaultFrameCore.FourCountSiteEntries());
        Assert.Equal(4, ClientDefaultFrameCore.CountSites.Length);
    }

    [Fact]
    public void CountNeverAssignedOnSomePaths()
    {
        Assert.True(ClientDefaultFrameCore.CustomPathDoesNotAssign());
        Assert.True(ClientDefaultFrameCore.NeverAssignedInBranchOne());
        Assert.True(ClientDefaultFrameCore.CarriesStaleValue());
        Assert.True(ClientDefaultFrameCore.NotAssignedOnDeathPath());
        Assert.True(ClientDefaultFrameCore.ExactlyTwoDoNotAssign());
        Assert.True(ClientDefaultFrameCore.ThreePlayOneStand());
    }

    // ===================== 三、NPC 取帧 =====================

    [Fact]
    public void NpcShape()
    {
        Assert.True(ClientDefaultFrameCore.SameThresholdAsDrawChr());
        Assert.True(ClientDefaultFrameCore.SimplerThanBase());
        Assert.True(ClientDefaultFrameCore.SameFinalFormula());
    }

    [Fact]
    public void DuplicatedModulo()
    {
        Assert.True(ClientDefaultFrameCore.SameModuloAsJ177());
        Assert.True(ClientDefaultFrameCore.DuplicatedDirCompression());
        Assert.True(ClientDefaultFrameCore.SameExclusionRange());
        Assert.True(ClientDefaultFrameCore.ModuloBoundary());
    }

    [Fact]
    public void ModuloBoundaryModel()
    {
        Assert.True(ClientDefaultFrameCore.AppliesModulo(245));
        Assert.False(ClientDefaultFrameCore.AppliesModulo(246));
        Assert.False(ClientDefaultFrameCore.AppliesModulo(272));
        Assert.True(ClientDefaultFrameCore.AppliesModulo(273));
    }

    [Fact]
    public void ConfigDir()
    {
        Assert.True(ClientDefaultFrameCore.ConfigDirCountModulo());
        Assert.True(ClientDefaultFrameCore.OneOrLessForcesZero());
        Assert.True(ClientDefaultFrameCore.NotModuloByOne());
        Assert.True(ClientDefaultFrameCore.ConfigDirValues());
        Assert.True(ClientDefaultFrameCore.SameResultAsModuloByOne());

        Assert.Equal(1, ClientDefaultFrameCore.ConfigDir(5, 4));
        Assert.Equal(0, ClientDefaultFrameCore.ConfigDir(5, 1));
        Assert.Equal(0, ClientDefaultFrameCore.ConfigDir(5, 0));
    }

    // ---------- 六段外观号集合 ----------

    [Fact]
    public void ZeroDirRangeShape()
    {
        Assert.True(ClientDefaultFrameCore.SixSpecialRanges());
        Assert.Equal(6, ClientDefaultFrameCore.ZeroDirRanges.Length);
        Assert.True(ClientDefaultFrameCore.ForcesDirZero());
        Assert.True(ClientDefaultFrameCore.ZeroDirRangesDisjoint());
    }

    [Fact]
    public void ZeroDirRangeValues()
    {
        Assert.Equal((54, 59), ClientDefaultFrameCore.ZeroDirRanges[0]);
        Assert.Equal((70, 75), ClientDefaultFrameCore.ZeroDirRanges[1]);
        Assert.Equal((81, 84), ClientDefaultFrameCore.ZeroDirRanges[2]);
        Assert.Equal((90, 92), ClientDefaultFrameCore.ZeroDirRanges[3]);
        Assert.Equal((94, 101), ClientDefaultFrameCore.ZeroDirRanges[4]);
        Assert.Equal((211, 225), ClientDefaultFrameCore.ZeroDirRanges[5]);
    }

    [Fact]
    public void ZeroDirBoundaries()
    {
        Assert.True(ClientDefaultFrameCore.ZeroDirBoundaries());

        Assert.True(ClientDefaultFrameCore.InZeroDirSet(54));
        Assert.True(ClientDefaultFrameCore.InZeroDirSet(59));
        Assert.False(ClientDefaultFrameCore.InZeroDirSet(53));
        Assert.False(ClientDefaultFrameCore.InZeroDirSet(60));

        Assert.True(ClientDefaultFrameCore.InZeroDirSet(211));
        Assert.True(ClientDefaultFrameCore.InZeroDirSet(225));
        Assert.False(ClientDefaultFrameCore.InZeroDirSet(210));
        Assert.False(ClientDefaultFrameCore.InZeroDirSet(226));
    }

    [Fact]
    public void ZeroDirSetSize()
    {
        // **逐段宽度累加：6+6+4+3+8+15 = 42**
        Assert.True(ClientDefaultFrameCore.ZeroDirSetSizeIs42());
        Assert.Equal(42, ClientDefaultFrameCore.ZeroDirSetSize());

        Assert.Equal(6, 59 - 54 + 1);
        Assert.Equal(6, 75 - 70 + 1);
        Assert.Equal(4, 84 - 81 + 1);
        Assert.Equal(3, 92 - 90 + 1);
        Assert.Equal(8, 101 - 94 + 1);
        Assert.Equal(15, 225 - 211 + 1);
    }

    [Fact]
    public void ZeroDirRangeWidths()
    {
        Assert.True(ClientDefaultFrameCore.FirstRangeIsSixWide());
        Assert.True(ClientDefaultFrameCore.FifthRangeIsEightWide());
    }

    [Fact]
    public void RangesDifferFromJ177()
    {
        Assert.True(ClientDefaultFrameCore.DiffersFromJ177Ranges());
        Assert.True(ClientDefaultFrameCore.BroaderUpperBounds());
        Assert.True(ClientDefaultFrameCore.MoreRangesHere());
        Assert.True(ClientDefaultFrameCore.RangesDiffer());

        // **J177 的 54..58 与本处 54..59 上界不同**
        Assert.Equal((54, 58), ClientDefaultFrameCore.J177Ranges[0]);
        Assert.NotEqual(ClientDefaultFrameCore.J177Ranges[0].High,
            ClientDefaultFrameCore.ZeroDirRanges[0].High);

        // **J177 的 94..98 与本处 94..101 上界不同**
        Assert.Equal((94, 98), ClientDefaultFrameCore.J177Ranges[1]);
        Assert.NotEqual(ClientDefaultFrameCore.J177Ranges[1].High,
            ClientDefaultFrameCore.ZeroDirRanges[4].High);
    }

    [Fact]
    public void J177RangesContainedButBroader()
    {
        Assert.True(ClientDefaultFrameCore.J177RangesContained());
        Assert.True(ClientDefaultFrameCore.ExtraValuesCovered());
        Assert.True(ClientDefaultFrameCore.InZeroDirSet(59));
        Assert.True(ClientDefaultFrameCore.InZeroDirSet(99));
        Assert.True(ClientDefaultFrameCore.InZeroDirSet(101));
    }

    [Fact]
    public void FinalIndex()
    {
        Assert.True(ClientDefaultFrameCore.ZeroDirFeedsIntoIndex());
        Assert.True(ClientDefaultFrameCore.FinalIndexValues());
        Assert.True(ClientDefaultFrameCore.ZeroDirDrawsFirstGroup());
        Assert.Equal(115, ClientDefaultFrameCore.FinalIndex(100, 2, 4, 2, 3));
    }

    [Fact]
    public void Statuary()
    {
        Assert.True(ClientDefaultFrameCore.StatuaryReturnsZero());
        Assert.True(ClientDefaultFrameCore.EmptyOverride());
        Assert.True(ClientDefaultFrameCore.StatuaryDrawsSeparately());
    }

    // ===================== 四、DefaultMotion =====================

    [Fact]
    public void MotionShape()
    {
        Assert.True(ClientDefaultFrameCore.WarModeExpiresAfter4s());
        Assert.True(ClientDefaultFrameCore.ClearsTwoFlags());
        Assert.True(ClientDefaultFrameCore.OneFlagTested());
        Assert.True(ClientDefaultFrameCore.ReturnsFrameChanged());
        Assert.True(ClientDefaultFrameCore.AssignsCurrentFrameLast());
        Assert.True(ClientDefaultFrameCore.CompareBeforeAssign());
    }

    [Fact]
    public void WmodeUnused()
    {
        Assert.True(ClientDefaultFrameCore.WmodeUnusedInBase());
        Assert.True(ClientDefaultFrameCore.ParameterNeverRead());
    }

    [Fact]
    public void ShiftDecl()
    {
        Assert.True(ClientDefaultFrameCore.ShiftDeclaredNotDefinedHere());
    }

    [Fact]
    public void WarModeBoundary()
    {
        Assert.True(ClientDefaultFrameCore.WarModeBoundary());

        // **恰好四秒不超时（严格大于）**
        Assert.False(ClientDefaultFrameCore.WarModeExpired(4000, 0));
        Assert.True(ClientDefaultFrameCore.WarModeExpired(4001, 0));
    }

    [Fact]
    public void CommentedCondition()
    {
        Assert.True(ClientDefaultFrameCore.RawSubtraction());
        Assert.True(ClientDefaultFrameCore.CommentedExtraCondition());
        Assert.True(ClientDefaultFrameCore.ConditionRemoved());
    }

    [Fact]
    public void MotionModel()
    {
        Assert.True(ClientDefaultFrameCore.MotionModel());
        Assert.True(ClientDefaultFrameCore.MotionChanged());

        var same = ClientDefaultFrameCore.DefaultMotion(5, 5);
        Assert.False(same.Changed);
        Assert.Equal(5, same.CurrentFrame);

        var chg = ClientDefaultFrameCore.DefaultMotion(5, 7);
        Assert.True(chg.Changed);
        Assert.Equal(7, chg.CurrentFrame);
    }

    // ---------- 加载节流 ----------

    [Fact]
    public void LoadThrottle()
    {
        Assert.True(ClientDefaultFrameCore.LoadTickBackdated60s());
        Assert.True(ClientDefaultFrameCore.ForcesExpiry());
        Assert.True(ClientDefaultFrameCore.BackdatedModel());
    }

    [Fact]
    public void LoadAllowedModel()
    {
        // **回溯六十秒后首次立即允许**
        Assert.True(ClientDefaultFrameCore.LoadAllowed(60000, 0));
        Assert.False(ClientDefaultFrameCore.LoadAllowed(59999, 0));
    }

    // ===================== 五、衔接 =====================

    [Fact]
    public void Connection()
    {
        Assert.True(ClientDefaultFrameCore.ConnectToJ177());
        Assert.True(ClientDefaultFrameCore.SharedConcepts());
        Assert.True(ClientDefaultFrameCore.RangeInconsistencyAcrossMethods());
    }

    // ===================== 行数 =====================

    [Fact]
    public void LineCounts()
    {
        Assert.True(ClientDefaultFrameCore.FourMethods());
        Assert.Equal(4, ClientDefaultFrameCore.MethodLineCounts.Length);
        Assert.Equal(new[] { 68, 47, 4, 15 }, ClientDefaultFrameCore.MethodLineCounts);
    }

    [Fact]
    public void LengthComparison()
    {
        Assert.True(ClientDefaultFrameCore.BaseIsLongest());
        Assert.True(ClientDefaultFrameCore.StatuaryIsShortest());
        Assert.True(ClientDefaultFrameCore.BaseShareIs50());
        Assert.True(ClientDefaultFrameCore.BaseExceedsNpcBy21());
    }

    [Fact]
    public void Totals()
    {
        Assert.True(ClientDefaultFrameCore.TotalLinesValues());
        Assert.Equal(134, ClientDefaultFrameCore.TotalLines());
    }
}
