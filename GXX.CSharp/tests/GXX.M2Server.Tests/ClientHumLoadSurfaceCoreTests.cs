using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J181：`THumActor.LoadSurface` 1:1 测试（968 行、全工程最长方法）。
/// **十九个表面清空、骑马六档阶梯乘三条着色路径、以及"九对多数"偏差在两批间稳定。**
/// </summary>
public sealed class ClientHumLoadSurfaceCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(100000, ClientHumLoadSurfaceCore.CustomMonsterBase);
        Assert.Equal(10000, ClientHumLoadSurfaceCore.CustomThreshold);
        Assert.Equal(17, ClientHumLoadSurfaceCore.SM_SPELL);
        Assert.Equal(113, ClientHumLoadSurfaceCore.SM_113HIT);
        Assert.Equal(115, ClientHumLoadSurfaceCore.SM_115HIT);
        Assert.Equal(9100, ClientHumLoadSurfaceCore.SM_100HIT);
        Assert.Equal(9103, ClientHumLoadSurfaceCore.SM_103HIT);
        Assert.Equal(11000, ClientHumLoadSurfaceCore.SM_CUSTOM_HIT001);
        Assert.Equal(12000, ClientHumLoadSurfaceCore.SM_CUSTOM_PUSH001);
        Assert.Equal(300, ClientHumLoadSurfaceCore.CUSTOM_MAGIC_COUNT);
        Assert.Equal(1000, ClientHumLoadSurfaceCore.CUSTOM_MAGIC_START_ID);
    }

    // ===================== 一、清理字段 =====================

    [Fact]
    public void ClearedFields()
    {
        Assert.True(ClientHumLoadSurfaceCore.NineteenSurfacesCleared());
        Assert.True(ClientHumLoadSurfaceCore.FourFlagsCleared());
        Assert.True(ClientHumLoadSurfaceCore.SurfacesDistinct());
        Assert.Equal(19, ClientHumLoadSurfaceCore.ClearedSurfaces.Length);
        Assert.Equal(4, ClientHumLoadSurfaceCore.ClearedFlags.Length);
    }

    [Fact]
    public void HorseSurfaces()
    {
        Assert.True(ClientHumLoadSurfaceCore.HorseRelatedFive());
        Assert.True(ClientHumLoadSurfaceCore.FiveHorseSurfaces());
        Assert.True(ClientHumLoadSurfaceCore.HorseSurfacesAllCleared());
        Assert.Equal(5, ClientHumLoadSurfaceCore.HorseSurfaces.Length);
    }

    [Fact]
    public void EffectLayers()
    {
        Assert.True(ClientHumLoadSurfaceCore.SixEffectLayers());
        Assert.True(ClientHumLoadSurfaceCore.ThreeNoBlendFlags());
        Assert.True(ClientHumLoadSurfaceCore.OneToOneWithEffects());
        Assert.True(ClientHumLoadSurfaceCore.ThreeNoBlend());
        Assert.True(ClientHumLoadSurfaceCore.NoBlendAllCleared());
        Assert.Equal(3, ClientHumLoadSurfaceCore.NoBlendFlags.Length);
    }

    [Fact]
    public void ClearOrdering()
    {
        Assert.True(ClientHumLoadSurfaceCore.AllClearedBeforeBranches());
        Assert.True(ClientHumLoadSurfaceCore.HalfRelateToHorseOrShape());
        Assert.True(ClientHumLoadSurfaceCore.ClassificationCounts());
    }

    [Fact]
    public void CommentedLeftover()
    {
        Assert.True(ClientHumLoadSurfaceCore.CommentedStaticVar());
        Assert.True(ClientHumLoadSurfaceCore.TestModeLeftover());
    }

    // ===================== 二、自定义怪物块 =====================

    [Fact]
    public void CustomMonster()
    {
        Assert.True(ClientHumLoadSurfaceCore.CustomMonsterBlockPresent());
        Assert.True(ClientHumLoadSurfaceCore.NearDuplicateOfJ180());
        Assert.True(ClientHumLoadSurfaceCore.DifferentGuardForm());
        Assert.True(ClientHumLoadSurfaceCore.CustomMonsterBoundary());
    }

    [Fact]
    public void CustomMonsterGuard()
    {
        Assert.False(ClientHumLoadSurfaceCore.IsCustomMonster(-1));
        Assert.False(ClientHumLoadSurfaceCore.IsCustomMonster(99999));
        Assert.True(ClientHumLoadSurfaceCore.IsCustomMonster(100000));
    }

    [Fact]
    public void Minus100000()
    {
        Assert.True(ClientHumLoadSurfaceCore.Minus100000Again());
        Assert.True(ClientHumLoadSurfaceCore.SameBaseAsJ179());
        Assert.True(ClientHumLoadSurfaceCore.MonsterApprIndexValues());
        Assert.Equal(42, ClientHumLoadSurfaceCore.MonsterApprIndex(100042));
    }

    // ===================== 三、骑马阶梯 =====================

    [Fact]
    public void HorseReferences()
    {
        Assert.True(ClientHumLoadSurfaceCore.ThirtyHorseReferences());
        Assert.True(ClientHumLoadSurfaceCore.HorseRefGrouping());
        Assert.True(ClientHumLoadSurfaceCore.LadderThreeInstances());
    }

    [Fact]
    public void LadderShape()
    {
        Assert.True(ClientHumLoadSurfaceCore.SixThresholdsTimesThreePaths());
        Assert.True(ClientHumLoadSurfaceCore.SevenImageVarsEach3x());
        Assert.True(ClientHumLoadSurfaceCore.UniformLadderStructure());
        Assert.True(ClientHumLoadSurfaceCore.UniformSixHundredFormula());
        Assert.Equal(18, ClientHumLoadSurfaceCore.HumLadderThresholds.Length * ClientHumLoadSurfaceCore.HumLadderColorPaths);
    }

    [Fact]
    public void LadderData()
    {
        Assert.True(ClientHumLoadSurfaceCore.SixThresholds());
        Assert.True(ClientHumLoadSurfaceCore.ThresholdValues());
        Assert.True(ClientHumLoadSurfaceCore.ThresholdsDescending());
        Assert.True(ClientHumLoadSurfaceCore.ThresholdsAreArithmetic());
        Assert.True(ClientHumLoadSurfaceCore.ThresholdsStep50());
        Assert.True(ClientHumLoadSurfaceCore.AllThresholdsPositive());
        Assert.True(ClientHumLoadSurfaceCore.LowestThresholdIs50());

        Assert.Equal(new[] { 300, 250, 200, 150, 100, 50 }, ClientHumLoadSurfaceCore.HumLadderThresholds);
    }

    [Fact]
    public void ImageNumberingMirrorsThreshold()
    {
        // **编号与阈值同向递减（编号是镜像、不是反向）**
        Assert.True(ClientHumLoadSurfaceCore.ImageNumbersDescending());
        Assert.True(ClientHumLoadSurfaceCore.NumberingMirrorsThreshold());
        Assert.True(ClientHumLoadSurfaceCore.NumberingTracksThreshold());
        Assert.True(ClientHumLoadSurfaceCore.NumberingRisesWithThreshold());
        Assert.True(ClientHumLoadSurfaceCore.SixIsHighest());

        Assert.Equal(new[] { 6, 5, 4, 3, 2, 1 }, ClientHumLoadSurfaceCore.HumLadderImageNumbers);
    }

    [Fact]
    public void ImageVars()
    {
        Assert.True(ClientHumLoadSurfaceCore.ImageVarsDistinct());
        Assert.Equal(7, ClientHumLoadSurfaceCore.HumLadderImageVars.Length);
        Assert.Equal("g_WLHorseHumImg", ClientHumLoadSurfaceCore.HumLadderImageVars[0]);
        Assert.Equal("g_WLHorseHumImg6", ClientHumLoadSurfaceCore.HumLadderImageVars[6]);
    }

    [Fact]
    public void LadderFormula()
    {
        Assert.True(ClientHumLoadSurfaceCore.ThresholdAsBase());
        Assert.True(ClientHumLoadSurfaceCore.FallbackUsesRawValue());
        Assert.True(ClientHumLoadSurfaceCore.LadderIndexValues());
        Assert.True(ClientHumLoadSurfaceCore.FallbackIndexValues());
        Assert.True(ClientHumLoadSurfaceCore.SameValueDifferentIndex());
    }

    [Fact]
    public void LadderIndexModel()
    {
        // **兜底不减阈值**
        Assert.Equal(6003, ClientHumLoadSurfaceCore.LadderFallbackIndex(0, 10, 1, 3));
        // **有阈值时减去该档阈值**
        Assert.Equal(5, ClientHumLoadSurfaceCore.LadderIndex(0, 300, 1, 5, 300));
        Assert.Equal(600 * 3 + 7, ClientHumLoadSurfaceCore.LadderIndex(1, 301, 2, 7, 300));
    }

    [Fact]
    public void LadderTriplication()
    {
        Assert.True(ClientHumLoadSurfaceCore.Triplicated());
        Assert.True(ClientHumLoadSurfaceCore.OnlyGetterDiffers());
        Assert.True(ClientHumLoadSurfaceCore.GrayPathOnlyOne());
        Assert.True(ClientHumLoadSurfaceCore.NoGray2InLadder());
    }

    // ===================== 四、着色统计 =====================

    [Fact]
    public void ColorTotals()
    {
        Assert.True(ClientHumLoadSurfaceCore.ColorTotal134());
        Assert.True(ClientHumLoadSurfaceCore.NineAgain());
        Assert.True(ClientHumLoadSurfaceCore.FiftyEightGrayOnly());
        Assert.True(ClientHumLoadSurfaceCore.SixtySevenBright());
        Assert.True(ClientHumLoadSurfaceCore.PartsSumToTotal());
        Assert.Equal(134, ClientHumLoadSurfaceCore.TotalColorCases);
    }

    [Fact]
    public void NineIsStableAcrossBatches()
    {
        // **两批的"含全数"都是九**
        Assert.True(ClientHumLoadSurfaceCore.NineIsStableAcrossBatches());
        Assert.True(ClientHumLoadSurfaceCore.SameNineInBothBatches());
        Assert.True(ClientHumLoadSurfaceCore.EvidenceOfCommonOrigin());
        Assert.Equal(ClientHumLoadSurfaceCore.PrevBatchGrayWithTwo, ClientHumLoadSurfaceCore.GrayWithTwo);
    }

    [Fact]
    public void DriftComparison()
    {
        // **本批四成三、上批四成一、差两个百分点**
        Assert.True(ClientHumLoadSurfaceCore.DriftWorseHere());
        Assert.True(ClientHumLoadSurfaceCore.PrevDriftRatioIs41());
        Assert.True(ClientHumLoadSurfaceCore.DriftGapIs2());
        Assert.True(ClientHumLoadSurfaceCore.DriftRatioIs43());
        Assert.True(ClientHumLoadSurfaceCore.GrayTwoRatioIs6());

        Assert.Equal(43, ClientHumLoadSurfaceCore.GrayOnlyOne * 100 / ClientHumLoadSurfaceCore.TotalColorCases);
        Assert.Equal(41, ClientHumLoadSurfaceCore.NpcGrayOnlyOne * 100 / ClientHumLoadSurfaceCore.NpcColorCases);
    }

    // ===================== 五、动作分支 =====================

    [Fact]
    public void CustomActionBranches()
    {
        Assert.True(ClientHumLoadSurfaceCore.ThreeCustomActionBranches());
        Assert.True(ClientHumLoadSurfaceCore.HalfOpenRanges());
        Assert.True(ClientHumLoadSurfaceCore.SpellThenHitThenPush());
        Assert.True(ClientHumLoadSurfaceCore.AbsThenMinusThenPlus1000());
        Assert.True(ClientHumLoadSurfaceCore.SameFormulaDifferentBase());
    }

    [Fact]
    public void HalfOpenRangeBoundary()
    {
        Assert.True(ClientHumLoadSurfaceCore.HalfOpenBoundary());

        Assert.False(ClientHumLoadSurfaceCore.IsCustomHit(10999));
        Assert.True(ClientHumLoadSurfaceCore.IsCustomHit(11000));
        Assert.True(ClientHumLoadSurfaceCore.IsCustomHit(11299));
        Assert.False(ClientHumLoadSurfaceCore.IsCustomHit(11300));
    }

    [Fact]
    public void ConfigIndex()
    {
        Assert.True(ClientHumLoadSurfaceCore.ConfigIndexValues());

        Assert.Equal(1000, ClientHumLoadSurfaceCore.ConfigIndex(11000, 11000));
        Assert.Equal(1000, ClientHumLoadSurfaceCore.ConfigIndex(12000, 12000));
    }

    [Fact]
    public void HitSet()
    {
        Assert.True(ClientHumLoadSurfaceCore.FiveDisjuncts());
        Assert.True(ClientHumLoadSurfaceCore.ClosedRangePlusSingles());
        Assert.True(ClientHumLoadSurfaceCore.FourHitActionsInSet());
        Assert.True(ClientHumLoadSurfaceCore.ClosedRangeBoundary());
        Assert.True(ClientHumLoadSurfaceCore.ThreeSinglesInSet());
        Assert.True(ClientHumLoadSurfaceCore.FourConsecutive());
    }

    [Fact]
    public void HitSetModel()
    {
        Assert.True(ClientHumLoadSurfaceCore.InHitSet(9100));
        Assert.True(ClientHumLoadSurfaceCore.InHitSet(9101));
        Assert.True(ClientHumLoadSurfaceCore.InHitSet(9102));
        Assert.True(ClientHumLoadSurfaceCore.InHitSet(9103));
        Assert.True(ClientHumLoadSurfaceCore.InHitSet(113));
        Assert.True(ClientHumLoadSurfaceCore.InHitSet(115));
        Assert.True(ClientHumLoadSurfaceCore.InHitSet(17));
        // **闭区间两端都算、两侧差一都不算**
        Assert.False(ClientHumLoadSurfaceCore.InHitSet(9099));
        Assert.False(ClientHumLoadSurfaceCore.InHitSet(9104));
    }

    [Fact]
    public void ActionRewrite()
    {
        Assert.True(ClientHumLoadSurfaceCore.ActionRewrittenToZero());
        Assert.True(ClientHumLoadSurfaceCore.SideEffectInLoad());
        Assert.True(ClientHumLoadSurfaceCore.HitOrPushCondition());
        Assert.True(ClientHumLoadSurfaceCore.OnlyTwoRewritten());
        Assert.True(ClientHumLoadSurfaceCore.RewriteValues());
    }

    [Fact]
    public void RewriteModel()
    {
        Assert.Equal(0, ClientHumLoadSurfaceCore.RewriteAction(9100));
        Assert.Equal(0, ClientHumLoadSurfaceCore.RewriteAction(12000));
        Assert.Equal(999, ClientHumLoadSurfaceCore.RewriteAction(999));
        Assert.Equal(5, ClientHumLoadSurfaceCore.RewriteAction(ClientHumLoadSurfaceCore.SM_WHATEVER));
    }

    // ---------- 调试与收尾 ----------

    [Fact]
    public void DebugCheck()
    {
        Assert.True(ClientHumLoadSurfaceCore.DebugFrameCheck());
        Assert.True(ClientHumLoadSurfaceCore.LocalPlayerOnly());
        Assert.True(ClientHumLoadSurfaceCore.DependsOnCommentedField());
        Assert.True(ClientHumLoadSurfaceCore.DebugConditionValues());
    }

    [Fact]
    public void DebugConditionModel()
    {
        Assert.True(ClientHumLoadSurfaceCore.ShouldRecordFrame(1, true, 1, 2));
        Assert.False(ClientHumLoadSurfaceCore.ShouldRecordFrame(0, true, 1, 2));
        Assert.False(ClientHumLoadSurfaceCore.ShouldRecordFrame(1, false, 1, 2));
        Assert.False(ClientHumLoadSurfaceCore.ShouldRecordFrame(1, true, 2, 2));
    }

    [Fact]
    public void Ending()
    {
        Assert.True(ClientHumLoadSurfaceCore.EndsWithIconsAndActionChanged());
        Assert.True(ClientHumLoadSurfaceCore.SameAsJ179AndJ180());
    }

    // ===================== 六、衔接与行数 =====================

    [Fact]
    public void CrossBatch()
    {
        Assert.True(ClientHumLoadSurfaceCore.SharedThreeConcepts());
        Assert.True(ClientHumLoadSurfaceCore.AddsHorseAndShape());
    }

    [Fact]
    public void Ratios()
    {
        Assert.True(ClientHumLoadSurfaceCore.LineRatio());
        Assert.True(ClientHumLoadSurfaceCore.BranchRatio());
        Assert.True(ClientHumLoadSurfaceCore.LinesGrowFasterThanBranches());

        Assert.Equal(167, ClientHumLoadSurfaceCore.TotalLines() * 100 / ClientHumLoadSurfaceCore.NpcLoadSurfaceLines);
        Assert.Equal(124, ClientHumLoadSurfaceCore.TotalColorCases * 100 / ClientHumLoadSurfaceCore.NpcColorCases);
    }

    [Fact]
    public void LineCounts()
    {
        Assert.True(ClientHumLoadSurfaceCore.TotalLinesValues());
        Assert.True(ClientHumLoadSurfaceCore.LongestMethodInSuite());
        Assert.True(ClientHumLoadSurfaceCore.ExceedsNpcBy390());
        Assert.True(ClientHumLoadSurfaceCore.ColorDensityIs7());
        Assert.Equal(968, ClientHumLoadSurfaceCore.TotalLines());
        Assert.Equal(390, ClientHumLoadSurfaceCore.TotalLines() - ClientHumLoadSurfaceCore.NpcLoadSurfaceLines);
    }
}
