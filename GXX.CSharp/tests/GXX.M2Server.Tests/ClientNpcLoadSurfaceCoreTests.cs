using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J180：`TNpcActor.LoadSurface` 1:1 测试（578 行、本单元最长方法）。
/// **三条出口、着色分支一百零八处里灰度成员九对四十五的不一致、
/// 原外观号对相对外观号、以及三个表面无条件清空。**
/// </summary>
public sealed class ClientNpcLoadSurfaceCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(10000, ClientNpcLoadSurfaceCore.CustomThreshold);
        Assert.Equal(2000, ClientNpcLoadSurfaceCore.SpecialThreshold);
        Assert.Equal(200, ClientNpcLoadSurfaceCore.BodySplitPoint);
        Assert.Equal(50, ClientNpcLoadSurfaceCore.NpcRace);
        Assert.Equal(4, ClientNpcLoadSurfaceCore.StartFrameGuard);
        Assert.Equal(226, ClientNpcLoadSurfaceCore.Range226To245Low);
        Assert.Equal(245, ClientNpcLoadSurfaceCore.Range226To245High);
    }

    [Fact]
    public void ColorStatistics()
    {
        Assert.Equal(108, ClientNpcLoadSurfaceCore.TotalColorCases);
        Assert.Equal(9, ClientNpcLoadSurfaceCore.GrayWithTwo);
        Assert.Equal(45, ClientNpcLoadSurfaceCore.GrayOnlyOne);
        Assert.Equal(54, ClientNpcLoadSurfaceCore.BrightCases);
    }

    // ===================== 一、骨架与出口 =====================

    [Fact]
    public void Skeleton()
    {
        Assert.True(ClientNpcLoadSurfaceCore.ThreeExits());
        Assert.True(ClientNpcLoadSurfaceCore.CustomPathExitsEarly());
        Assert.True(ClientNpcLoadSurfaceCore.ThreeSurfacesCleared());
        Assert.True(ClientNpcLoadSurfaceCore.SixFieldsInitialized());
    }

    [Fact]
    public void PathSelection()
    {
        Assert.True(ClientNpcLoadSurfaceCore.PathSelection());
        Assert.True(ClientNpcLoadSurfaceCore.CustomPathBoundary());
        Assert.True(ClientNpcLoadSurfaceCore.SpecialPathBoundary());

        Assert.Equal(1, ClientNpcLoadSurfaceCore.Path(10000));
        Assert.Equal(2, ClientNpcLoadSurfaceCore.Path(9999));
        Assert.Equal(2, ClientNpcLoadSurfaceCore.Path(2000));
        Assert.Equal(3, ClientNpcLoadSurfaceCore.Path(1999));
    }

    [Fact]
    public void LoadActorIcons()
    {
        Assert.True(ClientNpcLoadSurfaceCore.LoadActorIconsThreeSites());
        Assert.True(ClientNpcLoadSurfaceCore.CustomPathSkipsIcons());
        Assert.True(ClientNpcLoadSurfaceCore.TwoNormalPathsBothCall());
        Assert.True(ClientNpcLoadSurfaceCore.IconCallCounts());
        Assert.True(ClientNpcLoadSurfaceCore.IconsInBothNormalPaths());

        // **自定义路径零次、两条普通路径各一次**
        Assert.Equal(0, ClientNpcLoadSurfaceCore.IconCalls(1));
        Assert.Equal(1, ClientNpcLoadSurfaceCore.IconCalls(2));
        Assert.Equal(1, ClientNpcLoadSurfaceCore.IconCalls(3));
    }

    [Fact]
    public void Race50Duplication()
    {
        Assert.True(ClientNpcLoadSurfaceCore.Race50Twice());
        Assert.True(ClientNpcLoadSurfaceCore.DuplicatedRacePredicate());
    }

    [Fact]
    public void ClearBeforeBranch()
    {
        Assert.True(ClientNpcLoadSurfaceCore.UnconditionalClear());
        Assert.True(ClientNpcLoadSurfaceCore.BeforeAnyBranch());
        Assert.True(ClientNpcLoadSurfaceCore.CommentedFourthClear());
    }

    // ===================== 二、自定义路径 =====================

    [Fact]
    public void DirCompression()
    {
        Assert.True(ClientNpcLoadSurfaceCore.DirCountModuloAgain());
        Assert.True(ClientNpcLoadSurfaceCore.ThirdOccurrence());
        Assert.True(ClientNpcLoadSurfaceCore.SameAsJ178AndJ179());
        Assert.True(ClientNpcLoadSurfaceCore.CompressDirValues());

        Assert.Equal(1, ClientNpcLoadSurfaceCore.CompressDir(5, 4));
        Assert.Equal(0, ClientNpcLoadSurfaceCore.CompressDir(5, 1));
    }

    [Fact]
    public void KeepHasExtraTimeGuard()
    {
        Assert.True(ClientNpcLoadSurfaceCore.KeepHasFiveGuards());
        Assert.True(ClientNpcLoadSurfaceCore.BodyAndEffHaveFour());
        Assert.True(ClientNpcLoadSurfaceCore.KeepExtraTimeGuard());
        Assert.True(ClientNpcLoadSurfaceCore.TimeGuardOnlyForKeep());

        // **保留层的时间判据：零被拒**
        Assert.False(ClientNpcLoadSurfaceCore.KeepAccepted(0, 10, 0, 1, 0));
        Assert.True(ClientNpcLoadSurfaceCore.KeepAccepted(0, 10, 0, 1, 1));
        // **身体层不判时间**
        Assert.True(ClientNpcLoadSurfaceCore.BodyAccepted(0, 10, 0, 1));
    }

    // ---------- 着色分支统计 ----------

    [Fact]
    public void ColorTotals()
    {
        Assert.True(ClientNpcLoadSurfaceCore.ColorCaseTotal108());
        Assert.True(ClientNpcLoadSurfaceCore.NineIncludeGray2());
        Assert.True(ClientNpcLoadSurfaceCore.FortyFiveOmitGray2());
        Assert.True(ClientNpcLoadSurfaceCore.FiftyFourBright());
        Assert.True(ClientNpcLoadSurfaceCore.PartsSumToTotal());
        Assert.True(ClientNpcLoadSurfaceCore.ColorCasesAre108());
    }

    [Fact]
    public void GrayInconsistency()
    {
        // **漏写是含全的五倍**
        Assert.True(ClientNpcLoadSurfaceCore.GrayInconsistencyIsMajorityOmission());
        Assert.True(ClientNpcLoadSurfaceCore.OmissionIsFiveTimes());
        Assert.True(ClientNpcLoadSurfaceCore.NineOutOf108());
        Assert.True(ClientNpcLoadSurfaceCore.DriftRatioIs8Percent());

        Assert.Equal(5, ClientNpcLoadSurfaceCore.GrayOnlyOne / ClientNpcLoadSurfaceCore.GrayWithTwo);
        Assert.Equal(8, ClientNpcLoadSurfaceCore.GrayWithTwo * 100 / ClientNpcLoadSurfaceCore.TotalColorCases);
    }

    [Fact]
    public void GrayDistribution()
    {
        Assert.True(ClientNpcLoadSurfaceCore.NineSpreadAcrossThreePaths());
        Assert.True(ClientNpcLoadSurfaceCore.DistributionSumsToNine());
        Assert.True(ClientNpcLoadSurfaceCore.NoPathIsSelfConsistent());
        Assert.Equal(3, ClientNpcLoadSurfaceCore.GrayTwoDistribution.Length);

        Assert.Equal(("自定义 NPC 路径", 2), ClientNpcLoadSurfaceCore.GrayTwoDistribution[0]);
        Assert.Equal(("外观号大于等于二千且种族五十", 2), ClientNpcLoadSurfaceCore.GrayTwoDistribution[1]);
        Assert.Equal(("外观号小于二千", 5), ClientNpcLoadSurfaceCore.GrayTwoDistribution[2]);
    }

    [Fact]
    public void KeepNeverGray2()
    {
        Assert.True(ClientNpcLoadSurfaceCore.KeepGrayOnlyOne());
        Assert.True(ClientNpcLoadSurfaceCore.KeepNeverGray2());
        Assert.True(ClientNpcLoadSurfaceCore.KeepGray2FallsThrough());
        Assert.True(ClientNpcLoadSurfaceCore.BodyGray2IsGray());
        Assert.True(ClientNpcLoadSurfaceCore.LayersDiffer());

        // **保留层只认一号灰度；灰度二走普通**
        Assert.Equal("gray", ClientNpcLoadSurfaceCore.KeepColorBranch(1));
        Assert.Equal("normal", ClientNpcLoadSurfaceCore.KeepColorBranch(13));
        Assert.Equal("bright", ClientNpcLoadSurfaceCore.KeepColorBranch(2));
    }

    [Fact]
    public void BodyAcceptsGray2()
    {
        Assert.Equal("gray", ClientNpcLoadSurfaceCore.BodyColorBranch(1));
        Assert.Equal("gray", ClientNpcLoadSurfaceCore.BodyColorBranch(13));
        Assert.Equal("bright", ClientNpcLoadSurfaceCore.BodyColorBranch(2));
        Assert.Equal("normal", ClientNpcLoadSurfaceCore.BodyColorBranch(0));
    }

    [Fact]
    public void EffIndexBase()
    {
        Assert.True(ClientNpcLoadSurfaceCore.EffIndexMinusActIndex());
        Assert.True(ClientNpcLoadSurfaceCore.BodyUsesRawFrame());
        Assert.True(ClientNpcLoadSurfaceCore.TwoDifferentIndexBases());
        Assert.True(ClientNpcLoadSurfaceCore.EffIndexValues());
        Assert.True(ClientNpcLoadSurfaceCore.IndexBasesDiffer());

        Assert.Equal(103, ClientNpcLoadSurfaceCore.EffIndex(100, 5, 2));
        Assert.Equal(5, ClientNpcLoadSurfaceCore.BodyIndex(5));
    }

    [Fact]
    public void CommentedGuards()
    {
        Assert.True(ClientNpcLoadSurfaceCore.NonNegativeCheckCommentedTwice());
        Assert.True(ClientNpcLoadSurfaceCore.CommentExplainsUnsignedType());
        Assert.True(ClientNpcLoadSurfaceCore.TwoCommentedGuards());
        Assert.True(ClientNpcLoadSurfaceCore.EffUsesActCount());
        Assert.True(ClientNpcLoadSurfaceCore.NotEffOwnCount());
    }

    // ===================== 三、"大于等于二千"路径 =====================

    [Fact]
    public void RelativeAppearance()
    {
        Assert.True(ClientNpcLoadSurfaceCore.RelativeAppearance());
        Assert.True(ClientNpcLoadSurfaceCore.Minus2000());
        Assert.True(ClientNpcLoadSurfaceCore.TwoRaceBlocks());
        Assert.True(ClientNpcLoadSurfaceCore.RelativeValues());

        Assert.Equal(68, ClientNpcLoadSurfaceCore.Relative(2068));
    }

    [Fact]
    public void BodySplit()
    {
        Assert.True(ClientNpcLoadSurfaceCore.BodySplitAt200());
        Assert.True(ClientNpcLoadSurfaceCore.Index10Below());
        Assert.True(ClientNpcLoadSurfaceCore.Index11Above());
        Assert.True(ClientNpcLoadSurfaceCore.BodyImageIndexBoundary());

        Assert.Equal(10, ClientNpcLoadSurfaceCore.BodyImageIndex(199));
        Assert.Equal(11, ClientNpcLoadSurfaceCore.BodyImageIndex(200));
    }

    [Fact]
    public void AppearanceBranches()
    {
        Assert.True(ClientNpcLoadSurfaceCore.FifteenAppearanceBranches());
        Assert.True(ClientNpcLoadSurfaceCore.EqualsAndRangesMixed());
        Assert.True(ClientNpcLoadSurfaceCore.ThirteenEqualsTwoRanges());
        Assert.True(ClientNpcLoadSurfaceCore.AppearanceBranchesDistinct());
        Assert.True(ClientNpcLoadSurfaceCore.SinglesOutsideRanges());
        Assert.Equal(15, ClientNpcLoadSurfaceCore.AppearanceBranches.Length);
    }

    [Fact]
    public void AppearanceBranchValues()
    {
        Assert.Equal("= 68", ClientNpcLoadSurfaceCore.AppearanceBranches[0]);
        Assert.Equal("in [70..75]", ClientNpcLoadSurfaceCore.AppearanceBranches[1]);
        Assert.Equal("= 84", ClientNpcLoadSurfaceCore.AppearanceBranches[2]);
        Assert.Equal("= 209", ClientNpcLoadSurfaceCore.AppearanceBranches[5]);
        Assert.Equal("= 100", ClientNpcLoadSurfaceCore.AppearanceBranches[14]);
    }

    [Fact]
    public void StartFrameInChain()
    {
        Assert.True(ClientNpcLoadSurfaceCore.StartFrameConditionInChain());
        Assert.True(ClientNpcLoadSurfaceCore.NotAppearanceBased());
        Assert.True(ClientNpcLoadSurfaceCore.StartFrameGuardIs4());
        Assert.True(ClientNpcLoadSurfaceCore.StartFrameBoundary());

        Assert.False(ClientNpcLoadSurfaceCore.StartFrameOk(3));
        Assert.True(ClientNpcLoadSurfaceCore.StartFrameOk(4));
    }

    [Fact]
    public void NestedRange()
    {
        Assert.True(ClientNpcLoadSurfaceCore.Nested64To67());
        Assert.True(ClientNpcLoadSurfaceCore.NestedBoundary());

        Assert.False(ClientNpcLoadSurfaceCore.In64To67(63));
        Assert.True(ClientNpcLoadSurfaceCore.In64To67(64));
        Assert.True(ClientNpcLoadSurfaceCore.In64To67(67));
        Assert.False(ClientNpcLoadSurfaceCore.In64To67(68));
    }

    [Fact]
    public void DecrementIndex()
    {
        Assert.True(ClientNpcLoadSurfaceCore.OnlyOneDecrementIndex());
        Assert.True(ClientNpcLoadSurfaceCore.DecrementValues());
        Assert.True(ClientNpcLoadSurfaceCore.Add12Add20Add30());
        Assert.True(ClientNpcLoadSurfaceCore.Fixed3540And3660());

        Assert.Equal(95, ClientNpcLoadSurfaceCore.DecrementIndex(100, 5));
    }

    [Fact]
    public void IndexForms()
    {
        Assert.True(ClientNpcLoadSurfaceCore.ThirteenDistinctIndexForms());
        Assert.Equal(13, ClientNpcLoadSurfaceCore.IndexForms.Length);
        Assert.True(ClientNpcLoadSurfaceCore.DominantIsEffectFrame());
        Assert.True(ClientNpcLoadSurfaceCore.EffectFrameIs72());
        Assert.True(ClientNpcLoadSurfaceCore.IndexFormsSortedDescending());
        Assert.True(ClientNpcLoadSurfaceCore.TopThreeDominate());
    }

    [Fact]
    public void IndexFormValues()
    {
        Assert.Equal(("m_nBodyOffset + m_nEffectFrame", 72), ClientNpcLoadSurfaceCore.IndexForms[0]);
        Assert.Equal(("m_nBodyOffset + m_nCurrentFrame", 21), ClientNpcLoadSurfaceCore.IndexForms[1]);
        Assert.Equal(("m_nBodyOffset + 4 + m_nCurrentFrame", 18), ClientNpcLoadSurfaceCore.IndexForms[2]);
        Assert.Equal(("m_nKeepFrame", 3), ClientNpcLoadSurfaceCore.IndexForms[12]);
    }

    [Fact]
    public void FrameFieldSplit()
    {
        Assert.True(ClientNpcLoadSurfaceCore.BodyUsesCurrentFrameEffUsesEffectFrame());
    }

    [Fact]
    public void ImageIndexes()
    {
        Assert.True(ClientNpcLoadSurfaceCore.HardcodedImageIndexes());
        Assert.True(ClientNpcLoadSurfaceCore.SevenDistinctIndexes());
        Assert.True(ClientNpcLoadSurfaceCore.ImageIndexesDistinct());
        Assert.True(ClientNpcLoadSurfaceCore.ImageIndexesInRange());
        Assert.Equal(new[] { 0, 1, 2, 3, 9, 10, 11 }, ClientNpcLoadSurfaceCore.ImageIndexes);
    }

    // ===================== 四、"小于二千"路径 =====================

    [Fact]
    public void AbsolutePath()
    {
        Assert.True(ClientNpcLoadSurfaceCore.Below200UsesIndex0());
        Assert.True(ClientNpcLoadSurfaceCore.Range226To245UsesIndex0());
        Assert.True(ClientNpcLoadSurfaceCore.ElseUsesIndex1());
        Assert.True(ClientNpcLoadSurfaceCore.AbsoluteIndexValues());

        Assert.Equal(0, ClientNpcLoadSurfaceCore.AbsoluteImageIndex(100));
        Assert.Equal(0, ClientNpcLoadSurfaceCore.AbsoluteImageIndex(230));
        Assert.Equal(1, ClientNpcLoadSurfaceCore.AbsoluteImageIndex(1000));
    }

    [Fact]
    public void AbsoluteRangeBoundary()
    {
        Assert.Equal(0, ClientNpcLoadSurfaceCore.AbsoluteImageIndex(226));
        Assert.Equal(0, ClientNpcLoadSurfaceCore.AbsoluteImageIndex(245));
        Assert.Equal(1, ClientNpcLoadSurfaceCore.AbsoluteImageIndex(225));
        Assert.Equal(1, ClientNpcLoadSurfaceCore.AbsoluteImageIndex(246));
    }

    [Fact]
    public void SameNameDifferentMeaning()
    {
        Assert.True(ClientNpcLoadSurfaceCore.AbsoluteVsRelative());
        Assert.True(ClientNpcLoadSurfaceCore.SameNameDifferentMeaning());
        Assert.True(ClientNpcLoadSurfaceCore.OffBy2000());
        Assert.True(ClientNpcLoadSurfaceCore.SameNumberDifferentMeaning());

        // **2100 的相对值是 100；而绝对路径里 100 是另一回事**
        Assert.Equal(100, ClientNpcLoadSurfaceCore.Relative(2100));
    }

    [Fact]
    public void LongChain()
    {
        Assert.True(ClientNpcLoadSurfaceCore.LongElseIfChain());
    }

    // ===================== 五、跨批次衔接 =====================

    [Fact]
    public void CrossBatch()
    {
        Assert.True(ClientNpcLoadSurfaceCore.DirCompressionFourOccurrences());
        Assert.True(ClientNpcLoadSurfaceCore.Quadruplicated());
        Assert.True(ClientNpcLoadSurfaceCore.ThresholdFourthOccurrence());
        Assert.True(ClientNpcLoadSurfaceCore.LargestDriftInstance());
    }

    // ===================== 行数 =====================

    [Fact]
    public void LineCounts()
    {
        Assert.True(ClientNpcLoadSurfaceCore.TotalLinesValues());
        Assert.True(ClientNpcLoadSurfaceCore.LongestMethodSoFar());
        Assert.True(ClientNpcLoadSurfaceCore.ColorCaseDensityHigh());
        Assert.Equal(578, ClientNpcLoadSurfaceCore.TotalLines());
        Assert.Equal(5, ClientNpcLoadSurfaceCore.TotalLines() / ClientNpcLoadSurfaceCore.TotalColorCases);
    }
}
