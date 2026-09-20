using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J212：`ObjMon.pas` 中 `TFireCrossMonster`（火墙怪物）
/// 三个方法 1:1 测试（合计 234 行）。
/// **本批最有价值的发现**：
/// ① `AttackTarget`（7946-7952）与 J205 的 `TTwoKindAttackMonster.AttackTarget`
///    （4588-4594）**逐字相同、却分属不同基类**；
/// ② 25 行的嵌套函数 `GetRangeTargetCount` 因唯一调用点被 `{ }` 注释而**完全不可达**；
/// ③ `WAbil := @m_WAbil` 别名与省略的 `Max(...,1)` 钳位**一一对应**（可机械判定的两条血脉）。
/// </summary>
public sealed class ObjMonFireCrossCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(7717, ObjMonFireCrossCore.OneStart);
        Assert.Equal(7756, ObjMonFireCrossCore.OneEnd);
        Assert.Equal(40, ObjMonFireCrossCore.OneLines);
        Assert.Equal(7758, ObjMonFireCrossCore.TwoStart);
        Assert.Equal(7944, ObjMonFireCrossCore.TwoEnd);
        Assert.Equal(187, ObjMonFireCrossCore.TwoLines);
        Assert.Equal(7946, ObjMonFireCrossCore.AttackTargetStart);
        Assert.Equal(7952, ObjMonFireCrossCore.AttackTargetEnd);
        Assert.Equal(7, ObjMonFireCrossCore.AttackTargetLines);
        Assert.Equal(234, ObjMonFireCrossCore.TotalLines);

        Assert.Equal(7760, ObjMonFireCrossCore.NestedStart);
        Assert.Equal(7784, ObjMonFireCrossCore.NestedEnd);
        Assert.Equal(25, ObjMonFireCrossCore.NestedLines);
        Assert.Equal(7786, ObjMonFireCrossCore.VarStart);
        Assert.Equal(7797, ObjMonFireCrossCore.VarEnd);
        Assert.Equal(12, ObjMonFireCrossCore.VarLines);
        Assert.Equal(7798, ObjMonFireCrossCore.BodyStart);
        Assert.Equal(147, ObjMonFireCrossCore.BodyLines);
        Assert.Equal(7814, ObjMonFireCrossCore.CommentedCallLine);
        Assert.Equal(5, ObjMonFireCrossCore.DeadRadius);
        Assert.Equal(2, ObjMonFireCrossCore.DeadThreshold);

        Assert.Equal(7810, ObjMonFireCrossCore.AliasLine);
        Assert.Equal(7811, ObjMonFireCrossCore.PowerLine);
        Assert.Equal(13, ObjMonFireCrossCore.AliasSites);
        Assert.Equal(7792, ObjMonFireCrossCore.BtDeclareLine);
        Assert.Equal(1, ObjMonFireCrossCore.ClampMin);

        Assert.Equal(3, ObjMonFireCrossCore.FireTimeMin);
        Assert.Equal(8, ObjMonFireCrossCore.FireTimeMax);
        Assert.Equal(5, ObjMonFireCrossCore.ET_FIRE);
        Assert.Equal(5, ObjMonFireCrossCore.FireTiles);
        Assert.Equal(2, ObjMonFireCrossCore.FireEffectId);
        Assert.Equal(100, ObjMonFireCrossCore.FireCrossPowerRateDefault);
        Assert.Equal(5, ObjMonFireCrossCore.FireCrossMaxTimeDefault);
        Assert.Equal(7497, ObjMonFireCrossCore.MagicPasConfigLine);
        Assert.Equal(6, ObjMonFireCrossCore.FireCreateArgs);
        Assert.Equal(7, ObjMonFireCrossCore.FireCreateDeclaredArgs);

        Assert.Equal(20102, ObjMonFireCrossCore.RM_LIGHTING);
        Assert.Equal(20198, ObjMonFireCrossCore.RM_LIGHTINGEX);
        Assert.Equal(1, ObjMonFireCrossCore.GroupEffectId);

        Assert.Equal(3, ObjMonFireCrossCore.PoisonTimeBase);
        Assert.Equal(6, ObjMonFireCrossCore.PoisonTimeBound);
        Assert.Equal(20, ObjMonFireCrossCore.PoisonPower);
        Assert.Equal(6, ObjMonFireCrossCore.PoisonBlockLines);
        Assert.Equal(5, ObjMonFireCrossCore.UnguardedOccurrences);
        Assert.Equal(4, ObjMonFireCrossCore.ParalysisVerbatimBatches);
        Assert.Equal(3, ObjMonFireCrossCore.GroupRadius);
        Assert.Equal(7, ObjMonFireCrossCore.HealOccurrence);
        Assert.Equal(17, ObjMonFireCrossCore.ClassesCovered);
        Assert.Equal(37, ObjMonFireCrossCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonFireCrossCore.SpanMatches());
        Assert.True(ObjMonFireCrossCore.TotalLinesAddUp());
        Assert.True(ObjMonFireCrossCore.DecompositionAddsUp());
        Assert.True(ObjMonFireCrossCore.MethodsAscending());
        Assert.True(ObjMonFireCrossCore.MethodsContiguous());
        Assert.True(ObjMonFireCrossCore.NestedThenVar());
        Assert.True(ObjMonFireCrossCore.VarMustPrecedeBegin());
        Assert.True(ObjMonFireCrossCore.FirstOfItsKind());
        Assert.True(ObjMonFireCrossCore.WithinUnit());
        Assert.True(ObjMonFireCrossCore.NoInstrumentation());
    }

    // ===================== 一、与 J205 逐字相同 =====================

    [Fact]
    public void VerbatimSameAsJ205Facts()
    {
        Assert.True(ObjMonFireCrossCore.AttackTargetVerbatimSameAsJ205());
        Assert.True(ObjMonFireCrossCore.SevenLines());
        Assert.True(ObjMonFireCrossCore.Random4EqualsZero());
        Assert.True(ObjMonFireCrossCore.TrueCallsTwo());
        Assert.True(ObjMonFireCrossCore.FalseCallsOne());
        Assert.True(ObjMonFireCrossCore.DifferentBaseClasses());
        Assert.True(ObjMonFireCrossCore.CrossBranchCopyPaste());
        Assert.True(ObjMonFireCrossCore.BodiesMatchIgnoringName());
        Assert.True(ObjMonFireCrossCore.OnlyHeaderDiffers());
        Assert.True(ObjMonFireCrossCore.ExactlyOneLineDiffers());

        // **七行里恰有一行（函数名）不同、其余六行逐字一致**
        Assert.Equal(7, ObjMonFireCrossCore.AttackTargetBody.Length);
        Assert.Equal(7, ObjMonFireCrossCore.J205AttackTargetBody.Length);
        Assert.Equal(1, ObjMonFireCrossCore.DifferingLines());
    }

    [Fact]
    public void DispatchBoundaries()
    {
        Assert.True(ObjMonFireCrossCore.RollZeroGoesTwo());
        Assert.True(ObjMonFireCrossCore.RollOneGoesOne());
        Assert.True(ObjMonFireCrossCore.RollThreeGoesOne());
        Assert.True(ObjMonFireCrossCore.TwoIsOneInFour());

        Assert.Equal("TwoAttack", ObjMonFireCrossCore.Dispatch(0));
        Assert.Equal("OneAttack", ObjMonFireCrossCore.Dispatch(1));
        Assert.Equal("OneAttack", ObjMonFireCrossCore.Dispatch(2));
        Assert.Equal("OneAttack", ObjMonFireCrossCore.Dispatch(3));
    }

    [Fact]
    public void OverrideRelationFacts()
    {
        Assert.True(ObjMonFireCrossCore.OverridesAttackTargetNotMagic());
        Assert.True(ObjMonFireCrossCore.BypassesMagicFlag());
        Assert.True(ObjMonFireCrossCore.DeadFlagForThisClass());
        Assert.True(ObjMonFireCrossCore.OppositeOfJ207ToJ211());
        Assert.True(ObjMonFireCrossCore.NoRunOverride());
        Assert.True(ObjMonFireCrossCore.NoCreate());
        Assert.True(ObjMonFireCrossCore.ConfirmsSiblingRunRedundancy());
        Assert.True(ObjMonFireCrossCore.ThreeMethodsOnly());
    }

    // ===================== 二、死代码 =====================

    [Fact]
    public void DeadCodeFacts()
    {
        Assert.True(ObjMonFireCrossCore.DeadNestedFunction());
        Assert.True(ObjMonFireCrossCore.TwentyFiveLines());
        Assert.True(ObjMonFireCrossCore.OnlyCallSiteCommented());
        Assert.True(ObjMonFireCrossCore.ScopeConfined());
        Assert.True(ObjMonFireCrossCore.SiblingDefinitionIsLive());
        Assert.True(ObjMonFireCrossCore.DefinitionTableExtracted());
        Assert.True(ObjMonFireCrossCore.ReferenceTableExtracted());
        Assert.True(ObjMonFireCrossCore.TwoCommentedReferences());
        Assert.True(ObjMonFireCrossCore.ThirdDefinition());
        Assert.True(ObjMonFireCrossCore.CommentInsideLiveStatement());
        Assert.True(ObjMonFireCrossCore.SwallowsHalfCondition());
        Assert.True(ObjMonFireCrossCore.SixthCommentStyle());
        Assert.True(ObjMonFireCrossCore.SemanticNotJustCosmetic());
        Assert.True(ObjMonFireCrossCore.OriginalIntentWasCrowdGated());
        Assert.True(ObjMonFireCrossCore.BroadenedBehaviour());
        Assert.True(ObjMonFireCrossCore.NotAnUnusedFeature());
        Assert.True(ObjMonFireCrossCore.NestedFilterIsRejectForm());
    }

    [Fact]
    public void DefinitionAndReferenceTables()
    {
        Assert.Equal(3, ObjMonFireCrossCore.DefinitionLines.Length);
        Assert.Equal(3573, ObjMonFireCrossCore.DefinitionLines[0]);
        Assert.Equal(4110, ObjMonFireCrossCore.DefinitionLines[1]);
        Assert.Equal(7760, ObjMonFireCrossCore.DefinitionLines[2]);
        Assert.Equal(3, ObjMonFireCrossCore.ReferenceLines.Length);
        Assert.Equal(4255, ObjMonFireCrossCore.ReferenceLines[0]);
        Assert.Equal(4256, ObjMonFireCrossCore.ReferenceLines[1]);
        Assert.Equal(7814, ObjMonFireCrossCore.ReferenceLines[2]);
        Assert.Equal(2, ObjMonFireCrossCore.CommentedReferenceLines.Length);
        Assert.Equal(4256, ObjMonFireCrossCore.LiveReferenceLine);
    }

    [Fact]
    public void DeadVsLiveCondition()
    {
        // **注释掉的条件："半径 5 内合法目标数 > 2"**
        Assert.True(ObjMonFireCrossCore.DeadCondition(3));
        Assert.False(ObjMonFireCrossCore.DeadCondition(2));
        Assert.False(ObjMonFireCrossCore.DeadCondition(1));

        // **活下来的条件：只需 Random(3) = 0**
        Assert.True(ObjMonFireCrossCore.LiveCondition(0));
        Assert.False(ObjMonFireCrossCore.LiveCondition(1));

        // **于是单人时也可能放火墙（语义被放宽）**
        Assert.True(ObjMonFireCrossCore.SingleTargetCanTrigger());
        Assert.True(ObjMonFireCrossCore.VersionsDiffer());
    }

    [Fact]
    public void LoopDirectionFacts()
    {
        Assert.True(ObjMonFireCrossCore.NestedLoopNeedsDescending());
        Assert.True(ObjMonFireCrossCore.MainLoopDoesNot());
        Assert.True(ObjMonFireCrossCore.CopiedLoopHeader());
        Assert.True(ObjMonFireCrossCore.NestedListUnprotected());
        Assert.True(ObjMonFireCrossCore.SameAsJ207());
        Assert.True(ObjMonFireCrossCore.OppositeOfJ209());
    }

    [Fact]
    public void DeleteSemanticsBoundaries()
    {
        Assert.True(ObjMonFireCrossCore.DescendingDeleteCorrect());
        Assert.True(ObjMonFireCrossCore.DescendingHandlesAdjacent());

        // **相邻两个都命中时，朴素升序会漏删**
        Assert.True(ObjMonFireCrossCore.AdjacentMatchesExposeBug());
        Assert.True(ObjMonFireCrossCore.NaiveAscendingSkipsElements());

        // **不相邻时朴素升序恰好也对（说明初版样本选错了）**
        Assert.True(ObjMonFireCrossCore.NonAdjacentMatchesAccidentallyWork());
    }

    // ===================== 三、两条血脉 =====================

    [Fact]
    public void LineageFacts()
    {
        Assert.True(ObjMonFireCrossCore.PointerAlias());
        Assert.True(ObjMonFireCrossCore.NoMaxClamp());
        Assert.True(ObjMonFireCrossCore.PerfectCorrelation());
        Assert.True(ObjMonFireCrossCore.AliasLineThenUseLine());
        Assert.True(ObjMonFireCrossCore.EveryNoClampHasAlias());
        Assert.True(ObjMonFireCrossCore.AliasTableExtracted());
        Assert.True(ObjMonFireCrossCore.NoClampTableExtracted());
        Assert.True(ObjMonFireCrossCore.ClampedTableExtracted());
        Assert.True(ObjMonFireCrossCore.TablesDisjoint());
        Assert.True(ObjMonFireCrossCore.AliasTableAscending());
        Assert.True(ObjMonFireCrossCore.MechanicallyDecidable());
        Assert.True(ObjMonFireCrossCore.TypeAlsoSplits());
        Assert.True(ObjMonFireCrossCore.IntegerInAliasLineage());
        Assert.True(ObjMonFireCrossCore.BothSafe());
        Assert.True(ObjMonFireCrossCore.FurtherConfirmsLineage());
        Assert.True(ObjMonFireCrossCore.IntegerTableExtracted());

        Assert.Equal(13, ObjMonFireCrossCore.AliasLines.Length);
        Assert.Equal(7, ObjMonFireCrossCore.NoClampPowerLines.Length);
        Assert.Equal(8, ObjMonFireCrossCore.ClampedPowerLines.Length);
        Assert.Equal(4, ObjMonFireCrossCore.IntegerBtLines.Length);
        Assert.Equal(7792, ObjMonFireCrossCore.IntegerBtLines[3]);
    }

    [Fact]
    public void AliasLineAndUseLinePairing()
    {
        // **7 个无钳位调用逐一对应"前一行是别名"**
        Assert.Equal(7810, ObjMonFireCrossCore.AliasLine);
        Assert.Equal(7811, ObjMonFireCrossCore.PowerLine);
        Assert.Equal(ObjMonFireCrossCore.AliasLine + 1, ObjMonFireCrossCore.PowerLine);

        Assert.Equal(1995, ObjMonFireCrossCore.AliasLines[3]);
        Assert.Equal(1996, ObjMonFireCrossCore.NoClampPowerLines[0]);
        Assert.Equal(4167, ObjMonFireCrossCore.AliasLines[10]);
        Assert.Equal(4168, ObjMonFireCrossCore.NoClampPowerLines[5]);
    }

    [Fact]
    public void ClampBoundaries()
    {
        Assert.True(ObjMonFireCrossCore.ObservableDifference());
        Assert.True(ObjMonFireCrossCore.ZeroRangeWhenEqual());
        Assert.True(ObjMonFireCrossCore.ClampWouldForceOne());
        Assert.True(ObjMonFireCrossCore.DifferWhenEqual());
        Assert.True(ObjMonFireCrossCore.SameWhenPositive());

        // **DC1 == DC2：无钳位得 0、有钳位得 1**
        Assert.Equal(0, ObjMonFireCrossCore.PowerRangeNoClamp(50, 50));
        Assert.Equal(1, ObjMonFireCrossCore.PowerRangeClamped(50, 50));

        // **DC1 < DC2：两版一致**
        Assert.Equal(10, ObjMonFireCrossCore.PowerRangeNoClamp(50, 60));
        Assert.Equal(10, ObjMonFireCrossCore.PowerRangeClamped(50, 60));
    }

    // ===================== 四、火墙 =====================

    [Fact]
    public void FireTileFacts()
    {
        Assert.True(ObjMonFireCrossCore.FiveTileCross());
        Assert.True(ObjMonFireCrossCore.SelfCentered());
        Assert.True(ObjMonFireCrossCore.CenterIsThird());
        Assert.True(ObjMonFireCrossCore.OrderIsUpLeftCenterRightDown());
        Assert.True(ObjMonFireCrossCore.FireTilesExtracted());
        Assert.True(ObjMonFireCrossCore.FireTilesDisjoint());
        Assert.True(ObjMonFireCrossCore.FormsPlusShape());
        Assert.True(ObjMonFireCrossCore.CoversFullPlus());
        Assert.True(ObjMonFireCrossCore.PerTileGuard());
        Assert.True(ObjMonFireCrossCore.SequentialIndependentChecks());
        Assert.True(ObjMonFireCrossCore.ReliesOnImplicitDisjointness());
    }

    [Fact]
    public void FireTileTableOrder()
    {
        Assert.Equal(5, ObjMonFireCrossCore.FireTileOffsets.Length);

        // **顺序：上、左、中、右、下（中心第三）**
        Assert.Equal((0, -1), (ObjMonFireCrossCore.FireTileOffsets[0].Dx,
            ObjMonFireCrossCore.FireTileOffsets[0].Dy));
        Assert.Equal("up", ObjMonFireCrossCore.FireTileOffsets[0].Name);
        Assert.Equal("left", ObjMonFireCrossCore.FireTileOffsets[1].Name);
        Assert.Equal((0, 0), (ObjMonFireCrossCore.FireTileOffsets[2].Dx,
            ObjMonFireCrossCore.FireTileOffsets[2].Dy));
        Assert.Equal("center", ObjMonFireCrossCore.FireTileOffsets[2].Name);
        Assert.Equal("right", ObjMonFireCrossCore.FireTileOffsets[3].Name);
        Assert.Equal("down", ObjMonFireCrossCore.FireTileOffsets[4].Name);
    }

    [Fact]
    public void FireDurationBoundaries()
    {
        Assert.True(ObjMonFireCrossCore.OneInThree());
        Assert.True(ObjMonFireCrossCore.Duration3To8());
        Assert.True(ObjMonFireCrossCore.MinFireTime());
        Assert.True(ObjMonFireCrossCore.MaxFireTime());
        Assert.True(ObjMonFireCrossCore.SecondsToMillisAtCallSite());
        Assert.True(ObjMonFireCrossCore.MillisRange());

        Assert.Equal(3, ObjMonFireCrossCore.FireDuration(0));
        Assert.Equal(8, ObjMonFireCrossCore.FireDuration(5));
        Assert.Equal(3000, ObjMonFireCrossCore.FireDurationMs(0));
        Assert.Equal(8000, ObjMonFireCrossCore.FireDurationMs(5));
    }

    [Fact]
    public void FireConfigFacts()
    {
        Assert.True(ObjMonFireCrossCore.IgnoresMaxTimeConfig());
        Assert.True(ObjMonFireCrossCore.HardcodedRandom6Plus3());
        Assert.True(ObjMonFireCrossCore.MagicPasHonoursIt());
        Assert.True(ObjMonFireCrossCore.TwoImplementations());
        Assert.True(ObjMonFireCrossCore.MaxTimeDefaultIsFive());
        Assert.True(ObjMonFireCrossCore.IndependentOfConfig());
        Assert.True(ObjMonFireCrossCore.PowerRateScaled());
        Assert.True(ObjMonFireCrossCore.DefaultIsHundred());
        Assert.True(ObjMonFireCrossCore.IdentityByDefault());
        Assert.True(ObjMonFireCrossCore.ConfigDependentOnly());

        // **默认 100% 时缩放是恒等变换**
        Assert.Equal(137, ObjMonFireCrossCore.ScaleByRate(137, 100));
        Assert.Equal(50, ObjMonFireCrossCore.ScaleByRate(100, 50));
        Assert.Equal(200, ObjMonFireCrossCore.ScaleByRate(100, 200));
    }

    [Fact]
    public void FireFlowFacts()
    {
        Assert.True(ObjMonFireCrossCore.FireWallExitsEarly());
        Assert.True(ObjMonFireCrossCore.MutuallyExclusivePaths());
        Assert.True(ObjMonFireCrossCore.BreakHolySeizeOncePerPath());
        Assert.True(ObjMonFireCrossCore.BreakLinesDiffer());
        Assert.True(ObjMonFireCrossCore.SixArgsOnly());
        Assert.True(ObjMonFireCrossCore.SeventhHasDefaultFalse());
        Assert.True(ObjMonFireCrossCore.ExplicitlyNotCobweb());
        Assert.True(ObjMonFireCrossCore.CrossReferenceToJ203());
        Assert.True(ObjMonFireCrossCore.ArgumentCountDiffersByOne());
    }

    [Fact]
    public void MessageChannelFacts()
    {
        Assert.True(ObjMonFireCrossCore.BothUseRmLighting());
        Assert.True(ObjMonFireCrossCore.FireIsTwo());
        Assert.True(ObjMonFireCrossCore.GroupIsOne());
        Assert.True(ObjMonFireCrossCore.EffectIdsDiffer());
        Assert.True(ObjMonFireCrossCore.UsesRmLighting());
        Assert.True(ObjMonFireCrossCore.NotRmLightingEx());
        Assert.True(ObjMonFireCrossCore.TwoMessageChannels());
        Assert.True(ObjMonFireCrossCore.MessageValuesDiffer());
        Assert.True(ObjMonFireCrossCore.MessageDeltaIs96());

        Assert.Equal(96, ObjMonFireCrossCore.RM_LIGHTINGEX
            - ObjMonFireCrossCore.RM_LIGHTING);
    }

    // ===================== 五、施毒 =====================

    [Fact]
    public void PoisonConfigFacts()
    {
        Assert.True(ObjMonFireCrossCore.FourthPoisonConfig());
        Assert.True(ObjMonFireCrossCore.PoisonDuration3To8());
        Assert.True(ObjMonFireCrossCore.FixedStrength20());
        Assert.True(ObjMonFireCrossCore.PoisonTimeRange());
        Assert.True(ObjMonFireCrossCore.FourBatchesFourConfigs());
        Assert.True(ObjMonFireCrossCore.PoisonConfigsExtracted());
        Assert.True(ObjMonFireCrossCore.J211IsTheNoneOne());
        Assert.True(ObjMonFireCrossCore.SameKindAsJ207());
        Assert.True(ObjMonFireCrossCore.ButParametersDiffer());

        Assert.Equal(4, ObjMonFireCrossCore.PoisonConfigs.Length);
        Assert.Equal("J212", ObjMonFireCrossCore.PoisonConfigs[3].Batch);
        Assert.Equal(3, ObjMonFireCrossCore.PoisonDuration(0));
        Assert.Equal(8, ObjMonFireCrossCore.PoisonDuration(5));
    }

    [Fact]
    public void PoisonDuplicationFacts()
    {
        Assert.True(ObjMonFireCrossCore.DuplicatedVerbatim());
        Assert.True(ObjMonFireCrossCore.OnlyReceiverDiffers());
        Assert.True(ObjMonFireCrossCore.SixLineBlock());
        Assert.True(ObjMonFireCrossCore.PoisonBlockExtracted());
        Assert.True(ObjMonFireCrossCore.PoisonSpansMatch());

        // **两处施毒块各六行、均自洽**
        Assert.Equal(6, ObjMonFireCrossCore.PoisonBlock.Length);
        Assert.Equal(6,
            ObjMonFireCrossCore.OnePoisonEnd - ObjMonFireCrossCore.OnePoisonStart + 1);
        Assert.Equal(6,
            ObjMonFireCrossCore.TwoPoisonEnd - ObjMonFireCrossCore.TwoPoisonStart + 1);
    }

    [Fact]
    public void UnguardedPoisonFacts()
    {
        Assert.True(ObjMonFireCrossCore.UnguardedAgain());
        Assert.True(ObjMonFireCrossCore.FourthAndFifthOccurrence());
        Assert.True(ObjMonFireCrossCore.GuardOnParalysis());
        Assert.True(ObjMonFireCrossCore.RecurringFourthTime());
        Assert.True(ObjMonFireCrossCore.ReusesDiceProperty());
        Assert.True(ObjMonFireCrossCore.SecondClass());
        Assert.True(ObjMonFireCrossCore.ReadOnceEach());
        Assert.True(ObjMonFireCrossCore.StillMustNotCache());
        Assert.True(ObjMonFireCrossCore.SlotsDiffer());
        Assert.True(ObjMonFireCrossCore.ParalysisSlotIsFive());
        Assert.True(ObjMonFireCrossCore.UnguardedStillDecides());

        Assert.True(ObjMonFireCrossCore.PoisonRollUnguarded(0));
        Assert.True(ObjMonFireCrossCore.ParalysisRollGuarded(0, 0, 0));
    }

    // ===================== 六、Result 与冷却 =====================

    [Fact]
    public void ResultDecouplingFacts()
    {
        Assert.True(ObjMonFireCrossCore.ResultOutsideCooldown());
        Assert.True(ObjMonFireCrossCore.MeansInRangeNotAttacked());
        Assert.True(ObjMonFireCrossCore.ThirdOccurrenceOfDecoupling());
        Assert.True(ObjMonFireCrossCore.InRangeCoolingStillTrue());
        Assert.True(ObjMonFireCrossCore.OutOfRangeFalse());

        // **在范围内但冷却没过：Result 仍为真**
        Assert.True(ObjMonFireCrossCore.ResultValue(true));
        Assert.False(ObjMonFireCrossCore.ResultValue(false));
    }

    [Fact]
    public void CooldownAndFocusFacts()
    {
        Assert.True(ObjMonFireCrossCore.UsesTickDiff());
        Assert.True(ObjMonFireCrossCore.CooldownTriple());
        Assert.True(ObjMonFireCrossCore.SharedWithFamily());
        Assert.True(ObjMonFireCrossCore.SetsTargetFocusTick());
        Assert.True(ObjMonFireCrossCore.AbsentInJ207ToJ211());
        Assert.True(ObjMonFireCrossCore.FocusBeforeDirection());
        Assert.True(ObjMonFireCrossCore.DirectionLineIs7809());
        Assert.True(ObjMonFireCrossCore.FocusFollowsCooldownTriple());

        // **冷却边界三态**
        Assert.True(ObjMonFireCrossCore.JustResetIsFalse());
        Assert.True(ObjMonFireCrossCore.AfterThresholdIsTrue());
        Assert.True(ObjMonFireCrossCore.ExactlyAtThresholdIsFalse());
        Assert.False(ObjMonFireCrossCore.CooldownElapsed(1000, 1500, 500, 0));
        Assert.True(ObjMonFireCrossCore.CooldownElapsed(1000, 1501, 500, 0));
    }

    // ===================== 七、群体攻击 =====================

    [Fact]
    public void GroupAttackFacts()
    {
        Assert.True(ObjMonFireCrossCore.HardcodedRadius3());
        Assert.True(ObjMonFireCrossCore.TargetCentered());
        Assert.True(ObjMonFireCrossCore.ThreeClassesThreeChoices());
        Assert.True(ObjMonFireCrossCore.RejectFormIdiom());
        Assert.True(ObjMonFireCrossCore.TwoLayoutsForSameThing());
        Assert.True(ObjMonFireCrossCore.SingleLineVsBeginEnd());
        Assert.True(ObjMonFireCrossCore.SeventhOccurrence());
        Assert.True(ObjMonFireCrossCore.IntegerDeclaredHere());
        Assert.True(ObjMonFireCrossCore.CapBeforeAbsorb());
        Assert.True(ObjMonFireCrossCore.SixToOne());
        Assert.True(ObjMonFireCrossCore.ParalysisVerbatimFourBatches());
        Assert.True(ObjMonFireCrossCore.SharedFragment());
    }

    [Fact]
    public void RejectFormBoundaries()
    {
        Assert.True(ObjMonFireCrossCore.VisibleProperPasses());
        Assert.True(ObjMonFireCrossCore.HiddenNoCoolEyeRejected());
        Assert.True(ObjMonFireCrossCore.HiddenWithCoolEyePasses());
        Assert.True(ObjMonFireCrossCore.ImproperRejected());

        Assert.False(ObjMonFireCrossCore.RejectForm(false, false, true));
        Assert.True(ObjMonFireCrossCore.RejectForm(true, false, true));
        Assert.False(ObjMonFireCrossCore.RejectForm(true, true, true));
        Assert.True(ObjMonFireCrossCore.RejectForm(false, false, false));
    }

    [Fact]
    public void HealIdiomFacts()
    {
        Assert.Equal(0, ObjMonFireCrossCore.HealAmount(1000, 0));
        Assert.Equal(100, ObjMonFireCrossCore.HealAmount(1000, 10));
        Assert.True(ObjMonFireCrossCore.ZeroMpNoHeal());
        Assert.True(ObjMonFireCrossCore.TenMpTenthHeal());
    }

    // ===================== 八、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonFireCrossCore.SeventeenClassesCovered());
        Assert.True(ObjMonFireCrossCore.RemainingApprox());
        Assert.True(ObjMonFireCrossCore.NHTimeNaming());
        Assert.True(ObjMonFireCrossCore.SiblingUsesNHitTime());
        Assert.True(ObjMonFireCrossCore.InconsistentNaming());
    }
}
