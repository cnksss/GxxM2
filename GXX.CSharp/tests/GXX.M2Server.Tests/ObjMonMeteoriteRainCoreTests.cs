using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J219：`ObjMon.pas` 中 `TMeteoriteRainAttackMonster`（流星火雨怪物）
/// 三个方法 1:1 测试（合计 173 行）。
/// **本批最有价值的发现**：
/// ① 共享外层模板第一次被**按类能力裁剪**（少三行概率门、靠近改成放弃）、
///    而两处裁剪的理由都写在类注释"怪物不能移动"里；
/// ② `Create` 是全文件 **38 个构造里唯一**不先调 `inherited` 的（37:1）。
/// </summary>
public sealed class ObjMonMeteoriteRainCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(6377, ObjMonMeteoriteRainCore.CreateStart);
        Assert.Equal(6381, ObjMonMeteoriteRainCore.CreateEnd);
        Assert.Equal(5, ObjMonMeteoriteRainCore.CreateLines);
        Assert.Equal(6383, ObjMonMeteoriteRainCore.AttackStart);
        Assert.Equal(6535, ObjMonMeteoriteRainCore.AttackEnd);
        Assert.Equal(153, ObjMonMeteoriteRainCore.AttackLines);
        Assert.Equal(6537, ObjMonMeteoriteRainCore.RunStart);
        Assert.Equal(6551, ObjMonMeteoriteRainCore.RunEnd);
        Assert.Equal(15, ObjMonMeteoriteRainCore.RunLines);
        Assert.Equal(173, ObjMonMeteoriteRainCore.TotalLines);

        Assert.Equal(6385, ObjMonMeteoriteRainCore.NestedStart);
        Assert.Equal(6507, ObjMonMeteoriteRainCore.NestedEnd);
        Assert.Equal(123, ObjMonMeteoriteRainCore.NestedLines);
        Assert.Equal(6509, ObjMonMeteoriteRainCore.OuterStart);
        Assert.Equal(6535, ObjMonMeteoriteRainCore.OuterEnd);
        Assert.Equal(27, ObjMonMeteoriteRainCore.OuterLines);
        Assert.Equal(30, ObjMonMeteoriteRainCore.TemplateLines);
        Assert.Equal(3, ObjMonMeteoriteRainCore.MissingLines);
        Assert.Equal(5651, ObjMonMeteoriteRainCore.TemplateStart);
        Assert.Equal(5680, ObjMonMeteoriteRainCore.TemplateEnd);
        Assert.Equal(5661, ObjMonMeteoriteRainCore.TemplateGateLine);
        Assert.Equal(3, ObjMonMeteoriteRainCore.TemplateGateLines);
        Assert.Equal(5672, ObjMonMeteoriteRainCore.TemplateApproachLine);
        Assert.Equal(6527, ObjMonMeteoriteRainCore.DiscardLine);
        Assert.Equal(6532, ObjMonMeteoriteRainCore.DiscardOtherMapLine);
        Assert.Equal(2, ObjMonMeteoriteRainCore.SubstantiveChanges);

        Assert.Equal(6379, ObjMonMeteoriteRainCore.FireTickInitLine);
        Assert.Equal(6380, ObjMonMeteoriteRainCore.InheritedLine);
        Assert.Equal(38, ObjMonMeteoriteRainCore.ConstructorCount);
        Assert.Equal(37, ObjMonMeteoriteRainCore.InheritedFirstCount);
        Assert.Equal(1, ObjMonMeteoriteRainCore.InheritedNotFirstCount);
        Assert.Equal(228, ObjMonMeteoriteRainCore.FireTickDeclLine);
        Assert.Equal(1, ObjMonMeteoriteRainCore.PrivateFieldCount);
        Assert.Equal(3, ObjMonMeteoriteRainCore.FireTickSites);

        Assert.Equal(6403, ObjMonMeteoriteRainCore.ListCreateLine);
        Assert.Equal(6404, ObjMonMeteoriteRainCore.GetMapLine);
        Assert.Equal(2, ObjMonMeteoriteRainCore.RadiusDefault);
        Assert.Equal(2013, ObjMonMeteoriteRainCore.RadiusDeclLine);
        Assert.Equal(4712, ObjMonMeteoriteRainCore.RadiusDefaultLine);
        Assert.Equal(6409, ObjMonMeteoriteRainCore.SelfFilterLine);
        Assert.Equal(6412, ObjMonMeteoriteRainCore.ProperTargetLine);
        Assert.Equal(6414, ObjMonMeteoriteRainCore.ResistLine);
        Assert.Equal(10, ObjMonMeteoriteRainCore.ResistBound);
        Assert.Equal(6477, ObjMonMeteoriteRainCore.FreeLine);

        Assert.Equal(6478, ObjMonMeteoriteRainCore.FireStart);
        Assert.Equal(6506, ObjMonMeteoriteRainCore.FireEnd);
        Assert.Equal(20000, ObjMonMeteoriteRainCore.FireCooldownMs);
        Assert.Equal(6478, ObjMonMeteoriteRainCore.CooldownLine);
        Assert.Equal(6480, ObjMonMeteoriteRainCore.RandomizeLine);
        Assert.Equal(6481, ObjMonMeteoriteRainCore.FireRangeLine);
        Assert.Equal(3, ObjMonMeteoriteRainCore.FireRangeBase);
        Assert.Equal(5, ObjMonMeteoriteRainCore.FireRangeBound);
        Assert.Equal(3, ObjMonMeteoriteRainCore.FireRangeMin);
        Assert.Equal(7, ObjMonMeteoriteRainCore.FireRangeMax);
        Assert.Equal(6483, ObjMonMeteoriteRainCore.PowerOverwriteLine);
        Assert.Equal(4, ObjMonMeteoriteRainCore.FireTiles);
        Assert.Equal(18, ObjMonMeteoriteRainCore.ET_FIREMON33_7);
        Assert.Equal(3130, ObjMonMeteoriteRainCore.FireEffectTypeLine);
        Assert.Equal(10000, ObjMonMeteoriteRainCore.FireDurationMs);
        Assert.Equal(58, ObjMonMeteoriteRainCore.EffectId);
        Assert.Equal(6505, ObjMonMeteoriteRainCore.EffectLine);
        Assert.Equal(20198, ObjMonMeteoriteRainCore.RM_LIGHTINGEX);
        Assert.Equal(5, ObjMonMeteoriteRainCore.RandomizeSites);
        Assert.Equal(5, ObjMonMeteoriteRainCore.J212FireTiles);

        Assert.Equal(6539, ObjMonMeteoriteRainCore.GuardLine);
        Assert.Equal(6541, ObjMonMeteoriteRainCore.ThrottleLine);
        Assert.Equal(8000, ObjMonMeteoriteRainCore.SearchWithTargetMs);
        Assert.Equal(1000, ObjMonMeteoriteRainCore.SearchWithoutTargetMs);
        Assert.Equal(6545, ObjMonMeteoriteRainCore.SearchTargetLine);
        Assert.Equal(6548, ObjMonMeteoriteRainCore.AttackCallLine);
        Assert.Equal(6550, ObjMonMeteoriteRainCore.FinalInheritedLine);

        Assert.Equal(226, ObjMonMeteoriteRainCore.ClassDeclLine);
        Assert.Equal(225, ObjMonMeteoriteRainCore.FactionCommentLine);
        Assert.Equal(231, ObjMonMeteoriteRainCore.AttackDeclLine);
        Assert.Equal(173, ObjMonMeteoriteRainCore.AntiMagicDeclLine);
        Assert.Equal(817, ObjMonMeteoriteRainCore.AnimalObjectDeclLine);
        Assert.Equal(236, ObjMonMeteoriteRainCore.NextCannotMoveClassLine);
        Assert.Equal(25, ObjMonMeteoriteRainCore.ClassesCovered);
        Assert.Equal(29, ObjMonMeteoriteRainCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonMeteoriteRainCore.SpanMatches());
        Assert.True(ObjMonMeteoriteRainCore.TotalLinesAddUp());
        Assert.True(ObjMonMeteoriteRainCore.AttackDecompositionAddsUp());
        Assert.True(ObjMonMeteoriteRainCore.MethodsAscending());
        Assert.True(ObjMonMeteoriteRainCore.MethodsContiguous());
        Assert.True(ObjMonMeteoriteRainCore.NestedBeforeOuter());
        Assert.True(ObjMonMeteoriteRainCore.WithinUnit());
        Assert.True(ObjMonMeteoriteRainCore.NoInstrumentation());
    }

    // ===================== 一、Create 的孤例 =====================

    [Fact]
    public void InheritedOrderingFacts()
    {
        Assert.True(ObjMonMeteoriteRainCore.InheritedNotFirst());
        Assert.True(ObjMonMeteoriteRainCore.OnlyOneOfThirtyEight());
        Assert.True(ObjMonMeteoriteRainCore.QuantifiedOutlier());
        Assert.True(ObjMonMeteoriteRainCore.HarmlessHereButOrderDependent());
        Assert.True(ObjMonMeteoriteRainCore.RatioIsThirtySevenToOne());
        Assert.True(ObjMonMeteoriteRainCore.CountsAddUp());
        Assert.True(ObjMonMeteoriteRainCore.ThisClassIsFieldFirst());
        Assert.True(ObjMonMeteoriteRainCore.OthersAreInheritedFirst());
    }

    [Fact]
    public void InheritedOrderingArithmetic()
    {
        // **37 : 1**
        Assert.Equal(37, ObjMonMeteoriteRainCore.InheritedFirstCount
            / ObjMonMeteoriteRainCore.InheritedNotFirstCount);
        Assert.Equal(38, ObjMonMeteoriteRainCore.InheritedFirstCount
            + ObjMonMeteoriteRainCore.InheritedNotFirstCount);

        // **字段初始化行在 inherited 之前**
        Assert.True(ObjMonMeteoriteRainCore.FireTickInitLine
            < ObjMonMeteoriteRainCore.InheritedLine);

        Assert.Equal("field-first", ObjMonMeteoriteRainCore.InitOrder(false));
        Assert.Equal("inherited-first", ObjMonMeteoriteRainCore.InitOrder(true));
    }

    [Fact]
    public void FireTickFieldFacts()
    {
        Assert.True(ObjMonMeteoriteRainCore.SinglePrivateField());
        Assert.True(ObjMonMeteoriteRainCore.FireCooldownField());
        Assert.True(ObjMonMeteoriteRainCore.ThreeSites());
        Assert.True(ObjMonMeteoriteRainCore.InitThenReadWrite());
    }

    // ===================== 二、外层体的两处裁剪 =====================

    [Fact]
    public void TemplateTrimFacts()
    {
        Assert.True(ObjMonMeteoriteRainCore.OuterIsTwentySeven());
        Assert.True(ObjMonMeteoriteRainCore.TemplateMinusThree());
        Assert.True(ObjMonMeteoriteRainCore.ThreeLinesAreTheGate());
        Assert.True(ObjMonMeteoriteRainCore.TwoSubstantiveChanges());
        Assert.True(ObjMonMeteoriteRainCore.GateDeleted());
        Assert.True(ObjMonMeteoriteRainCore.ApproachBecomesDiscard());
        Assert.True(ObjMonMeteoriteRainCore.BothJustifiedByCannotMove());
        Assert.True(ObjMonMeteoriteRainCore.FirstPurposefulTrim());
        Assert.True(ObjMonMeteoriteRainCore.AlignedHeadIdentical());
        Assert.True(ObjMonMeteoriteRainCore.OuterSpanMatches());
        Assert.True(ObjMonMeteoriteRainCore.TemplateSpanMatches());
    }

    [Fact]
    public void DegenerateBranchFacts()
    {
        Assert.True(ObjMonMeteoriteRainCore.BothBranchesDiscard());
        Assert.True(ObjMonMeteoriteRainCore.SameMapAlsoDiscards());
        Assert.True(ObjMonMeteoriteRainCore.OtherMapDiscards());
        Assert.True(ObjMonMeteoriteRainCore.DegenerateToSameBody());
        Assert.True(ObjMonMeteoriteRainCore.StructureRetained());
    }

    [Fact]
    public void TemplateTrimArithmetic()
    {
        // **30 - 3 = 27**
        Assert.Equal(ObjMonMeteoriteRainCore.OuterLines,
            ObjMonMeteoriteRainCore.TemplateLines
            - ObjMonMeteoriteRainCore.MissingLines);
        Assert.Equal(3, ObjMonMeteoriteRainCore.MissingLines);

        // **两处实质改动**
        Assert.Equal(2, ObjMonMeteoriteRainCore.SubstantiveChanges);
    }

    [Fact]
    public void DeclarationFormFacts()
    {
        Assert.True(ObjMonMeteoriteRainCore.NoOverride());
        Assert.True(ObjMonMeteoriteRainCore.CorrectAsChainRoot());
        Assert.True(ObjMonMeteoriteRainCore.NotEvenVirtual());
        Assert.True(ObjMonMeteoriteRainCore.StaticButSafe());
        Assert.True(ObjMonMeteoriteRainCore.AttacksWithoutGateRoll());
        Assert.True(ObjMonMeteoriteRainCore.TemplateNeedsGateRoll());
    }

    // ===================== 三、Run =====================

    [Fact]
    public void RunFacts()
    {
        Assert.True(ObjMonMeteoriteRainCore.RealRunImplementation());
        Assert.True(ObjMonMeteoriteRainCore.ShellChainBroken());
        Assert.True(ObjMonMeteoriteRainCore.SameGuardAsJ214J216());
        Assert.True(ObjMonMeteoriteRainCore.SameThrottle());
        Assert.True(ObjMonMeteoriteRainCore.CallsAttackTargetWithoutParens());
        Assert.True(ObjMonMeteoriteRainCore.NoThinkCall());
        Assert.True(ObjMonMeteoriteRainCore.NoThinkMethodEither());
        Assert.True(ObjMonMeteoriteRainCore.ContrastWithJ214J216());
        Assert.True(ObjMonMeteoriteRainCore.NoWalkThrottleBlock());
        Assert.True(ObjMonMeteoriteRainCore.ThreePlacesReflectIt());
        Assert.True(ObjMonMeteoriteRainCore.RunDecompositionAddsUp());
        Assert.True(ObjMonMeteoriteRainCore.FinalInheritedUnconditional());
        Assert.True(ObjMonMeteoriteRainCore.OrderIsGuardSearchAttackBase());
    }

    [Fact]
    public void RunGuardBoundaries()
    {
        Assert.True(ObjMonMeteoriteRainCore.AllTrueRuns());
        Assert.True(ObjMonMeteoriteRainCore.AnyBlocks());

        Assert.True(ObjMonMeteoriteRainCore.CanRun(false, false, false, false, true));
        Assert.False(ObjMonMeteoriteRainCore.CanRun(true, false, false, false, true));
        Assert.False(ObjMonMeteoriteRainCore.CanRun(false, true, false, false, true));
        Assert.False(ObjMonMeteoriteRainCore.CanRun(false, false, true, false, true));
        Assert.False(ObjMonMeteoriteRainCore.CanRun(false, false, false, true, true));
        Assert.False(ObjMonMeteoriteRainCore.CanRun(false, false, false, false, false));
    }

    [Fact]
    public void RunSearchBoundaries()
    {
        Assert.True(ObjMonMeteoriteRainCore.SearchAfterEightWithTarget());
        Assert.True(ObjMonMeteoriteRainCore.ExactlyEightBlocks());
        Assert.True(ObjMonMeteoriteRainCore.SearchAfterOneWithoutTarget());

        Assert.True(ObjMonMeteoriteRainCore.ShouldSearch(8001, true));
        Assert.False(ObjMonMeteoriteRainCore.ShouldSearch(8000, true));
        Assert.True(ObjMonMeteoriteRainCore.ShouldSearch(1001, false));
        Assert.False(ObjMonMeteoriteRainCore.ShouldSearch(1000, false));
    }

    // ===================== 四、群攻段 =====================

    [Fact]
    public void RadiusFacts()
    {
        Assert.True(ObjMonMeteoriteRainCore.RadiusFromConfig());
        Assert.True(ObjMonMeteoriteRainCore.DifferentConfigKey());
        Assert.True(ObjMonMeteoriteRainCore.DefaultIsTwo());
        Assert.True(ObjMonMeteoriteRainCore.NeverUnified());
        Assert.True(ObjMonMeteoriteRainCore.GroupCombosExtracted());
        Assert.True(ObjMonMeteoriteRainCore.TwoConfigsThreeHardcoded());
        Assert.True(ObjMonMeteoriteRainCore.AllCombosDiffer());
        Assert.True(ObjMonMeteoriteRainCore.ConfigKeysDiffer());

        Assert.Equal(5, ObjMonMeteoriteRainCore.GroupCombos.Length);
        Assert.Equal("J219", ObjMonMeteoriteRainCore.GroupCombos[4].Batch);
        Assert.Equal("config nSkill58AttackRange",
            ObjMonMeteoriteRainCore.GroupCombos[4].Radius);
        Assert.Equal("target", ObjMonMeteoriteRainCore.GroupCombos[4].Center);
    }

    [Fact]
    public void SelfFilterFacts()
    {
        Assert.True(ObjMonMeteoriteRainCore.SelfCenteredSixFilter());
        Assert.True(ObjMonMeteoriteRainCore.SameAsJ207DoubleFilter());
        Assert.True(ObjMonMeteoriteRainCore.ExactlySixInside());
        Assert.True(ObjMonMeteoriteRainCore.SevenOutside());

        Assert.True(ObjMonMeteoriteRainCore.WithinSelfSix(6, 6));
        Assert.True(ObjMonMeteoriteRainCore.WithinSelfSix(0, 0));
        Assert.False(ObjMonMeteoriteRainCore.WithinSelfSix(7, 0));
        Assert.False(ObjMonMeteoriteRainCore.WithinSelfSix(0, 7));
    }

    [Fact]
    public void ResistFormFacts()
    {
        Assert.True(ObjMonMeteoriteRainCore.NewResistForm());
        Assert.True(ObjMonMeteoriteRainCore.FifthForm());
        Assert.True(ObjMonMeteoriteRainCore.UsesAntiMagicNotAntiPoison());
        Assert.True(ObjMonMeteoriteRainCore.DirectionIsReversed());
        Assert.True(ObjMonMeteoriteRainCore.ResistFormsExtracted());
        Assert.True(ObjMonMeteoriteRainCore.FirstFourSameDirection());

        Assert.Equal(5, ObjMonMeteoriteRainCore.ResistForms.Length);
        Assert.Equal("m_nAntiMagic", ObjMonMeteoriteRainCore.ResistForms[4].Target);
    }

    [Fact]
    public void ResistBoundaries()
    {
        Assert.True(ObjMonMeteoriteRainCore.ZeroAntiMagicAlwaysHits());
        Assert.True(ObjMonMeteoriteRainCore.FullAntiMagicNeverHits());
        Assert.True(ObjMonMeteoriteRainCore.HigherAntiMagicHarder());
        Assert.True(ObjMonMeteoriteRainCore.RollMaxIsNine());

        Assert.True(ObjMonMeteoriteRainCore.PassesResist(0, 0));
        Assert.True(ObjMonMeteoriteRainCore.PassesResist(2, 5));
        Assert.False(ObjMonMeteoriteRainCore.PassesResist(8, 5));
        Assert.False(ObjMonMeteoriteRainCore.PassesResist(10, 9));

        // **恰好等于躲避值时命中（`>=`）**
        Assert.True(ObjMonMeteoriteRainCore.PassesResist(5, 5));
    }

    [Fact]
    public void ResourceFacts()
    {
        Assert.True(ObjMonMeteoriteRainCore.NoTryFinally());
        Assert.True(ObjMonMeteoriteRainCore.FourToTwoAgainst());
    }

    // ===================== 五、火圈段 =====================

    [Fact]
    public void FireCooldownFacts()
    {
        Assert.True(ObjMonMeteoriteRainCore.TwentySecondCooldown());
        Assert.True(ObjMonMeteoriteRainCore.RawSubtraction());
        Assert.True(ObjMonMeteoriteRainCore.JustFiredNotReady());
        Assert.True(ObjMonMeteoriteRainCore.ExactlyTwentyBlocks());
        Assert.True(ObjMonMeteoriteRainCore.OneOverReady());

        Assert.False(ObjMonMeteoriteRainCore.FireReady(1000, 1000));
        Assert.False(ObjMonMeteoriteRainCore.FireReady(0, 20000));
        Assert.True(ObjMonMeteoriteRainCore.FireReady(0, 20001));
    }

    [Fact]
    public void RandomizeFacts()
    {
        Assert.True(ObjMonMeteoriteRainCore.CallsRandomize());
        Assert.True(ObjMonMeteoriteRainCore.FiveRandomizeSites());
        Assert.True(ObjMonMeteoriteRainCore.RandomizeTableExtracted());
        Assert.True(ObjMonMeteoriteRainCore.SecondRandomizeSite());

        Assert.Equal(5, ObjMonMeteoriteRainCore.RandomizeLines.Length);
        Assert.Equal(new[] { 1621, 6480, 6788, 7207, 9126 },
            ObjMonMeteoriteRainCore.RandomizeLines);
    }

    [Fact]
    public void FireRangeBoundaries()
    {
        Assert.True(ObjMonMeteoriteRainCore.FireRangeThreeToSeven());
        Assert.True(ObjMonMeteoriteRainCore.MinFireRange());
        Assert.True(ObjMonMeteoriteRainCore.MaxFireRange());

        Assert.Equal(3, ObjMonMeteoriteRainCore.FireRange(0));
        Assert.Equal(7, ObjMonMeteoriteRainCore.FireRange(4));
    }

    [Fact]
    public void FireTileFacts()
    {
        Assert.True(ObjMonMeteoriteRainCore.RingIsHollow());
        Assert.True(ObjMonMeteoriteRainCore.FourCardinalTiles());
        Assert.True(ObjMonMeteoriteRainCore.ContrastWithJ212Plus());
        Assert.True(ObjMonMeteoriteRainCore.DifferenceIsTheCenter());
        Assert.True(ObjMonMeteoriteRainCore.FireTilesExtracted());
        Assert.True(ObjMonMeteoriteRainCore.FireTilesDisjoint());
        Assert.True(ObjMonMeteoriteRainCore.NoCenterTile());
        Assert.True(ObjMonMeteoriteRainCore.AllOnAxes());
        Assert.True(ObjMonMeteoriteRainCore.PlusCenterEqualsJ212());
    }

    [Fact]
    public void FireTileTable()
    {
        Assert.Equal(4, ObjMonMeteoriteRainCore.FireTileOffsets.Length);
        Assert.Equal((0, -1), (ObjMonMeteoriteRainCore.FireTileOffsets[0].Dx,
            ObjMonMeteoriteRainCore.FireTileOffsets[0].Dy));
        Assert.Equal("up", ObjMonMeteoriteRainCore.FireTileOffsets[0].Name);
        Assert.Equal("left", ObjMonMeteoriteRainCore.FireTileOffsets[1].Name);
        Assert.Equal("right", ObjMonMeteoriteRainCore.FireTileOffsets[2].Name);
        Assert.Equal("down", ObjMonMeteoriteRainCore.FireTileOffsets[3].Name);

        Assert.Equal(new[] { 6485, 6490, 6495, 6500 },
            ObjMonMeteoriteRainCore.FireTileLines);
    }

    [Fact]
    public void FireTypeFacts()
    {
        Assert.True(ObjMonMeteoriteRainCore.FireEffectTypeIsEighteen());
        Assert.True(ObjMonMeteoriteRainCore.FireTypeDeclChecked());
        Assert.True(ObjMonMeteoriteRainCore.DurationIsTenSeconds());
        Assert.True(ObjMonMeteoriteRainCore.SeventhParamTrue());
        Assert.True(ObjMonMeteoriteRainCore.J212SeventhParamFalse());
        Assert.True(ObjMonMeteoriteRainCore.SameCtorTwoForms());
        Assert.True(ObjMonMeteoriteRainCore.EffectFiftyEight());
        Assert.True(ObjMonMeteoriteRainCore.EffectLineChecked());
    }

    [Fact]
    public void PowerReuseAndCenterFacts()
    {
        Assert.True(ObjMonMeteoriteRainCore.NPowerReusedAsDamage());
        Assert.True(ObjMonMeteoriteRainCore.OneVariableTwoRoles());
        Assert.True(ObjMonMeteoriteRainCore.TwoCentersInOneMethod());
        Assert.True(ObjMonMeteoriteRainCore.FirstOfItsKind());
        Assert.True(ObjMonMeteoriteRainCore.DamageUsesTargetCenter());
        Assert.True(ObjMonMeteoriteRainCore.FireUsesSelfCenter());
        Assert.True(ObjMonMeteoriteRainCore.CentersDiffer());

        Assert.Equal("target", ObjMonMeteoriteRainCore.PickCenter(true));
        Assert.Equal("self", ObjMonMeteoriteRainCore.PickCenter(false));
    }

    [Fact]
    public void PerTileGuardFacts()
    {
        Assert.True(ObjMonMeteoriteRainCore.PerTileGuardAgain());
        Assert.True(ObjMonMeteoriteRainCore.DynamicCoordinates());
        Assert.True(ObjMonMeteoriteRainCore.ContrastWithJ212Fixed());
    }

    // ===================== 六、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonMeteoriteRainCore.TwentyFiveClassesCovered());
        Assert.True(ObjMonMeteoriteRainCore.RemainingApprox());
        Assert.True(ObjMonMeteoriteRainCore.TwoClassComments());
        Assert.True(ObjMonMeteoriteRainCore.CannotMoveExplainsAll());
        Assert.True(ObjMonMeteoriteRainCore.SeveralCannotMoveClasses());
        Assert.True(ObjMonMeteoriteRainCore.NextClassTenLinesLater());
    }
}
