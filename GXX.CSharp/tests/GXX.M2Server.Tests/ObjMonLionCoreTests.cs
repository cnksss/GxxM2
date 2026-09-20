using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J229：`ObjMon.pas` 中 `TLionMonster`（狮子）三个方法的 1:1 测试（216 行）。
/// **本批最有价值的发现**：
/// ① 同一个 `AttackTarget` 里有**两条独立的攻击路径**（贯穿光束 + 近身 `Attack`），
///    且外层门序与阈值都与共享模板不同（范围→概率→冷却 vs 模板的冷却→范围→概率）；
/// ② `{ nDir := }` 五个字符的注释造就一个**纯死调用**、并让光束**永远走直线**；
/// ③ 光束的伤害是**发消息**而不是本地结算的（八参、伤害在第四格、延迟 600、无 `StruckDamage`）；
/// ④ `GotoTargetXY` 里嵌套的 `function Run` **遮蔽了类自己的 `Run` 方法**（形态㉑ 新变体）。
/// </summary>
public sealed class ObjMonLionCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(7496, ObjMonLionCore.AttackStart);
        Assert.Equal(7625, ObjMonLionCore.AttackEnd);
        Assert.Equal(130, ObjMonLionCore.AttackLines);
        Assert.Equal(7627, ObjMonLionCore.GotoStart);
        Assert.Equal(7708, ObjMonLionCore.GotoEnd);
        Assert.Equal(82, ObjMonLionCore.GotoLines);
        Assert.Equal(7710, ObjMonLionCore.RunStart);
        Assert.Equal(7713, ObjMonLionCore.RunEnd);
        Assert.Equal(4, ObjMonLionCore.RunLines);
        Assert.Equal(216, ObjMonLionCore.TotalLines);
        Assert.Equal(7500, ObjMonLionCore.BeamStart);
        Assert.Equal(7582, ObjMonLionCore.BeamEnd);
        Assert.Equal(83, ObjMonLionCore.BeamLines);
        Assert.Equal(7584, ObjMonLionCore.AttackOuterStart);
        Assert.Equal(7625, ObjMonLionCore.AttackOuterEnd);
        Assert.Equal(42, ObjMonLionCore.AttackOuterLines);
        Assert.Equal(7629, ObjMonLionCore.WalkStart);
        Assert.Equal(7688, ObjMonLionCore.WalkEnd);
        Assert.Equal(60, ObjMonLionCore.WalkLines);
        Assert.Equal(7690, ObjMonLionCore.NestedRunStart);
        Assert.Equal(7693, ObjMonLionCore.NestedRunEnd);
        Assert.Equal(7695, ObjMonLionCore.GotoOuterStart);
        Assert.Equal(7708, ObjMonLionCore.GotoOuterEnd);

        Assert.Equal(7588, ObjMonLionCore.BeamRangeLine);
        Assert.Equal(3, ObjMonLionCore.BeamRangeThreshold);
        Assert.Equal(7590, ObjMonLionCore.BeamGateLine);
        Assert.Equal(3, ObjMonLionCore.BeamGateBound);
        Assert.Equal(7592, ObjMonLionCore.BeamCooldownLine);
        Assert.Equal(7596, ObjMonLionCore.BeamCallLine);
        Assert.Equal(7597, ObjMonLionCore.BeamResultLine);
        Assert.Equal(7598, ObjMonLionCore.BeamExitLine);
        Assert.Equal(7602, ObjMonLionCore.MeleeCheckLine);
        Assert.Equal(7604, ObjMonLionCore.MeleeCooldownLine);
        Assert.Equal(7608, ObjMonLionCore.FocusTickLine);
        Assert.Equal(7609, ObjMonLionCore.BaseAttackLine);
        Assert.Equal(7610, ObjMonLionCore.BreakSeizeLine);
        Assert.Equal(7612, ObjMonLionCore.MeleeResultLine);
        Assert.Equal(5655, ObjMonLionCore.TemplateCooldownLine);
        Assert.Equal(6, ObjMonLionCore.TemplateRangeThreshold);
        Assert.Equal(2, ObjMonLionCore.TemplateGateBound);

        Assert.Equal(7571, ObjMonLionCore.DeadCallCommentLine);
        Assert.Equal(7572, ObjMonLionCore.DeadCallLine);
        Assert.Equal(7573, ObjMonLionCore.StaleDirectionLine);
        Assert.Equal(7511, ObjMonLionCore.DirectionSetLine);
        Assert.Equal(7569, ObjMonLionCore.DoubleNegationLine);
        Assert.Equal(7525, ObjMonLionCore.LoopLine);
        Assert.Equal(2, ObjMonLionCore.LoopMax);
        Assert.Equal(3, ObjMonLionCore.BeamMaxTiles);
        Assert.Equal(4, ObjMonLionCore.ProbeDistance);
        Assert.Equal(7522, ObjMonLionCore.FirstProbeLine);
        Assert.Equal(7524, ObjMonLionCore.TargetProbeLine);
        Assert.Equal(7527, ObjMonLionCore.GetMovingObjectLine);
        Assert.Equal(7563, ObjMonLionCore.ResistLine);
        Assert.Equal(10, ObjMonLionCore.ResistBound);
        Assert.Equal(6414, ObjMonLionCore.J219ResistLine);
        Assert.Equal(7565, ObjMonLionCore.SendLine);
        Assert.Equal(0, ObjMonLionCore.SendFifthArg);
        Assert.Equal(0, ObjMonLionCore.SendSixthArg);
        Assert.Equal(600, ObjMonLionCore.SendDelay);
        Assert.Equal(200, ObjMonLionCore.OtherDelay);
        Assert.Equal(7581, ObjMonLionCore.EffectLine);
        Assert.Equal(7520, ObjMonLionCore.PositiveGuardLine);
        Assert.Equal(7580, ObjMonLionCore.PositiveGuardEndLine);
        Assert.Equal(7498, ObjMonLionCore.Bt06DeclLine);

        Assert.Equal(7633, ObjMonLionCore.N10DeclLine);
        Assert.Equal(7634, ObjMonLionCore.N14DeclLine);
        Assert.Equal(7635, ObjMonLionCore.N20DeclLine);
        Assert.Equal(7639, ObjMonLionCore.N10AssignLine);
        Assert.Equal(7640, ObjMonLionCore.N14AssignLine);
        Assert.Equal(7641, ObjMonLionCore.DwTickCommentLine);
        Assert.Equal(7642, ObjMonLionCore.CascadeStart);
        Assert.Equal(7668, ObjMonLionCore.CascadeEnd);
        Assert.Equal(7642, ObjMonLionCore.DirDefaultLine);
        Assert.Equal(7671, ObjMonLionCore.WalkToLine);
        Assert.Equal(7669, ObjMonLionCore.OldPosLine);
        Assert.Equal(7672, ObjMonLionCore.N20RollLine);
        Assert.Equal(7673, ObjMonLionCore.RetryLoopLine);
        Assert.Equal(7631, ObjMonLionCore.LoopVarDeclLine);
        Assert.Equal(7675, ObjMonLionCore.StillBlockedLine);
        Assert.Equal(7677, ObjMonLionCore.RotateLine);
        Assert.Equal(7683, ObjMonLionCore.WrapHighLine);
        Assert.Equal(7685, ObjMonLionCore.RotateWalkLine);
        Assert.Equal(0, ObjMonLionCore.DR_UP);
        Assert.Equal(7, ObjMonLionCore.DR_UPLEFT);
        Assert.Equal(7692, ObjMonLionCore.RunToLine);
        Assert.Equal(7700, ObjMonLionCore.NestedRunCallLine);
        Assert.Equal(7698, ObjMonLionCore.GotoRangeLine);
        Assert.Equal(3, ObjMonLionCore.GotoRangeThreshold);
        Assert.Equal(7705, ObjMonLionCore.NearWalkLine);
        Assert.Equal(7616, ObjMonLionCore.SameMapLine);
        Assert.Equal(7622, ObjMonLionCore.DiscardOtherMapLine);

        Assert.Equal(286, ObjMonLionCore.ClassDeclLine);
        Assert.Equal(288, ObjMonLionCore.AttackDeclLine);
        Assert.Equal(289, ObjMonLionCore.RunDeclLine);
        Assert.Equal(290, ObjMonLionCore.GotoDeclLine);
        Assert.Equal(32, ObjMonLionCore.TATMonsterLine);
        Assert.Equal(286, ObjMonLionCore.CommentLine);
        Assert.Equal(7717, ObjMonLionCore.NextImplLine);
        Assert.Equal(713, ObjMonLionCore.GetAttackDirDeclLine);
        Assert.Equal(27062, ObjMonLionCore.GetAttackDirImplLine);
        Assert.Equal(31, ObjMonLionCore.ClassesCovered);
        Assert.Equal(23, ObjMonLionCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonLionCore.SpanMatches());
        Assert.True(ObjMonLionCore.TotalLinesAddUp());
        Assert.True(ObjMonLionCore.AttackDecompositionAddsUp());
        Assert.True(ObjMonLionCore.GotoDecompositionAddsUp());
        Assert.True(ObjMonLionCore.MethodsAscending());
        Assert.True(ObjMonLionCore.MethodsContiguous());
        Assert.True(ObjMonLionCore.WithinUnit());
        Assert.True(ObjMonLionCore.NoInstrumentation());
    }

    // ===================== 一、两条攻击路径 =====================

    [Fact]
    public void TwoPathsFacts()
    {
        Assert.True(ObjMonLionCore.TwoIndependentAttackPaths());
        Assert.True(ObjMonLionCore.BeamThenMelee());
        Assert.True(ObjMonLionCore.BeamHasExitMeleeHasNot());
        Assert.True(ObjMonLionCore.MeleeUsesBaseAttack());
        Assert.True(ObjMonLionCore.AttackPathsExtracted());
        Assert.True(ObjMonLionCore.BeamHasGateMeleeHasNot());

        Assert.Equal(2, ObjMonLionCore.AttackPaths.Length);
        Assert.Equal("beam", ObjMonLionCore.AttackPaths[0].Path);
        Assert.True(ObjMonLionCore.AttackPaths[0].Exits);
        Assert.Equal("melee", ObjMonLionCore.AttackPaths[1].Path);
        Assert.False(ObjMonLionCore.AttackPaths[1].Exits);
    }

    [Fact]
    public void GateOrderFacts()
    {
        Assert.True(ObjMonLionCore.GateOrderReversed());
        Assert.True(ObjMonLionCore.ThresholdsBothChanged());
        Assert.True(ObjMonLionCore.NestedShapeDifferent());
        Assert.True(ObjMonLionCore.NotTheSharedTemplate());
        Assert.True(ObjMonLionCore.GateOrderExtracted());
        Assert.True(ObjMonLionCore.AllThreeLayersDiffer());
        Assert.True(ObjMonLionCore.RangeHalved());
        Assert.True(ObjMonLionCore.ProbabilityLowered());

        Assert.Equal(3, ObjMonLionCore.GateOrder.Length);
        Assert.Contains("range", ObjMonLionCore.GateOrder[0].Lion);
        Assert.Contains("cooldown", ObjMonLionCore.GateOrder[0].Template);
    }

    [Fact]
    public void BeamGateBoundaries()
    {
        Assert.True(ObjMonLionCore.AllConditionsFire());
        Assert.True(ObjMonLionCore.OutOfRangeBlocks());
        Assert.True(ObjMonLionCore.MissedRollBlocks());
        Assert.True(ObjMonLionCore.CooldownBlocks());

        Assert.True(ObjMonLionCore.BeamFires(3, 3, 0, true));
        Assert.False(ObjMonLionCore.BeamFires(4, 0, 0, true));
        Assert.False(ObjMonLionCore.BeamFires(0, 4, 0, true));
        Assert.False(ObjMonLionCore.BeamFires(1, 1, 1, true));
        Assert.False(ObjMonLionCore.BeamFires(1, 1, 2, true));
        Assert.False(ObjMonLionCore.BeamFires(1, 1, 0, false));
    }

    // ===================== 二、死调用与双重否定 =====================

    [Fact]
    public void DeadCallFacts()
    {
        Assert.True(ObjMonLionCore.BraceCommentKillsAssignment());
        Assert.True(ObjMonLionCore.PureDeadCall());
        Assert.True(ObjMonLionCore.StaleDirectionUsedBelow());
        Assert.True(ObjMonLionCore.BeamAlwaysStraight());
        Assert.True(ObjMonLionCore.ThirdKindOfBraceDisable());
        Assert.True(ObjMonLionCore.GetNextDirectionHasNoSideEffect());

        Assert.True(ObjMonLionCore.DeadCallCommentLine
            < ObjMonLionCore.DeadCallLine);
        Assert.True(ObjMonLionCore.DeadCallLine
            < ObjMonLionCore.StaleDirectionLine);
        Assert.True(ObjMonLionCore.DirectionSetLine
            < ObjMonLionCore.DeadCallLine);
    }

    [Fact]
    public void DoubleNegationFacts()
    {
        Assert.True(ObjMonLionCore.AbsLeZeroIsEqZero());
        Assert.True(ObjMonLionCore.SameAsEqZero());
        Assert.True(ObjMonLionCore.DoubleNegation());
        Assert.True(ObjMonLionCore.EquivalentToNotBothZero());

        Assert.False(ObjMonLionCore.NotBothZero(0, 0));
        Assert.True(ObjMonLionCore.NotBothZero(0, 1));
        Assert.True(ObjMonLionCore.NotBothZero(1, 0));
        Assert.True(ObjMonLionCore.NotBothZero(1, 1));

        Assert.True(ObjMonLionCore.AbsIsZero(0));
        Assert.False(ObjMonLionCore.AbsIsZero(-3));
        Assert.False(ObjMonLionCore.AbsIsZero(1));
    }

    [Fact]
    public void BeamGeometryFacts()
    {
        Assert.True(ObjMonLionCore.ThreeTilesMax());
        Assert.True(ObjMonLionCore.ProbeDistanceFour());
        Assert.True(ObjMonLionCore.OffByOneBetweenCountAndDistance());
        Assert.True(ObjMonLionCore.LoopAlwaysBreaks());
        Assert.True(ObjMonLionCore.LoopLinesChecked());

        Assert.Equal(1, ObjMonLionCore.ProbeDistance
            - ObjMonLionCore.BeamMaxTiles);
    }

    [Fact]
    public void ResistFacts()
    {
        Assert.True(ObjMonLionCore.FixedTenFaceForm());
        Assert.True(ObjMonLionCore.SameAsJ219());
        Assert.True(ObjMonLionCore.TwoFamiliesCoexist());
        Assert.True(ObjMonLionCore.ZeroAlwaysHits());
        Assert.True(ObjMonLionCore.FullNeverHits());

        Assert.True(ObjMonLionCore.PassesResist(0, 0));
        Assert.False(ObjMonLionCore.PassesResist(10, 9));
        Assert.True(ObjMonLionCore.PassesResist(5, 5));
    }

    [Fact]
    public void SendLayoutFacts()
    {
        Assert.True(ObjMonLionCore.EightArgsDifferentLayout());
        Assert.True(ObjMonLionCore.DamageInFourthSlot());
        Assert.True(ObjMonLionCore.DelaySixHundred());
        Assert.True(ObjMonLionCore.NoStruckDamageCall());
        Assert.True(ObjMonLionCore.ClientSideDamageSettlement());
        Assert.True(ObjMonLionCore.SendLayoutExtracted());
        Assert.True(ObjMonLionCore.FourSlotsDiffer());
    }

    [Fact]
    public void SendLayoutTable()
    {
        Assert.Equal(6, ObjMonLionCore.SendLayout.Length);
        Assert.Equal("Self", ObjMonLionCore.SendLayout[0].Lion);
        Assert.Equal("RM_MAGSTRUCK", ObjMonLionCore.SendLayout[1].Lion);
        Assert.Equal("0", ObjMonLionCore.SendLayout[2].Lion);
        Assert.Equal("nPower", ObjMonLionCore.SendLayout[3].Lion);
        Assert.Equal("0 / 0", ObjMonLionCore.SendLayout[4].Lion);
        Assert.Equal("600", ObjMonLionCore.SendLayout[5].Lion);
    }

    [Fact]
    public void EffectAndFilterFacts()
    {
        Assert.True(ObjMonLionCore.EffectOutsideDamageGuard());
        Assert.True(ObjMonLionCore.ZeroDamageStillSends());
        Assert.True(ObjMonLionCore.SameAsJ228OppositeOfJ223());
        Assert.True(ObjMonLionCore.WidestFilterInSeries());
        Assert.True(ObjMonLionCore.CompoundNegationFilter());
        Assert.True(ObjMonLionCore.NoDeathGhostHideCheck());
        Assert.True(ObjMonLionCore.ConfigOffAttacks());
        Assert.True(ObjMonLionCore.OfflineExcluded());
    }

    // ===================== 三、GotoTargetXY =====================

    [Fact]
    public void ShadowingFacts()
    {
        Assert.True(ObjMonLionCore.NestedRunShadowsMethod());
        Assert.True(ObjMonLionCore.ShadowingIsLegal());
        Assert.True(ObjMonLionCore.ResolvesToNested());
        Assert.True(ObjMonLionCore.Shape21NewVariant());
        Assert.True(ObjMonLionCore.TwoRunsCoexistHarmlessly());

        // **嵌套 Run 在 7690、类方法 Run 在 7710**
        Assert.True(ObjMonLionCore.NestedRunStart
            < ObjMonLionCore.RunStart);
    }

    [Fact]
    public void HandRolledCascadeFacts()
    {
        Assert.True(ObjMonLionCore.HandRolledEightWay());
        Assert.True(ObjMonLionCore.GetNextDirectionAlsoUsed());
        Assert.True(ObjMonLionCore.TwoApproachesInOneMethod());
        Assert.True(ObjMonLionCore.CascadeCoversEight());

        Assert.Equal(8, ObjMonLionCore.DR_UPLEFT
            - ObjMonLionCore.DR_UP + 1);
    }

    [Fact]
    public void OffsetNameFacts()
    {
        Assert.True(ObjMonLionCore.OffsetStyleNames());
        Assert.True(ObjMonLionCore.N10N14N20());
        Assert.True(ObjMonLionCore.CommentedDwTick3F4());
        Assert.True(ObjMonLionCore.SameFamilyAsJ159Etc());
    }

    [Fact]
    public void RetryLoopFacts()
    {
        Assert.True(ObjMonLionCore.LoopVarUnused());
        Assert.True(ObjMonLionCore.CounterOnly());
        Assert.True(ObjMonLionCore.NoBreakOnSuccess());
        Assert.True(ObjMonLionCore.GuardIsStillBlocked());
        Assert.True(ObjMonLionCore.OppositeOfShape24());
    }

    [Fact]
    public void RotationFacts()
    {
        Assert.True(ObjMonLionCore.AsymmetricTwoThirdsOneThird());
        Assert.True(ObjMonLionCore.NonZeroIncrements());
        Assert.True(ObjMonLionCore.ZeroDecrements());
        Assert.True(ObjMonLionCore.ZeroWrapsToSeven());
        Assert.True(ObjMonLionCore.SevenWrapsToZero());
        Assert.True(ObjMonLionCore.WrapHandledInTwoPlaces());
        Assert.True(ObjMonLionCore.RandomThreeAgain());
    }

    [Fact]
    public void RotationBoundaries()
    {
        Assert.Equal(4, ObjMonLionCore.RotateDir(3, 1));
        Assert.Equal(4, ObjMonLionCore.RotateDir(3, 2));
        Assert.Equal(2, ObjMonLionCore.RotateDir(3, 0));
        Assert.Equal(7, ObjMonLionCore.RotateDir(0, 0));
        Assert.Equal(0, ObjMonLionCore.RotateDir(7, 1));
        Assert.Equal(6, ObjMonLionCore.RotateDir(7, 0));
    }

    [Fact]
    public void FarNearFacts()
    {
        Assert.True(ObjMonLionCore.FarVersusNear());
        Assert.True(ObjMonLionCore.RunThenWalkFallback());
        Assert.True(ObjMonLionCore.NearWalkOnly());
        Assert.True(ObjMonLionCore.ThresholdsComplementary());
        Assert.True(ObjMonLionCore.ThreeIsFar());
        Assert.True(ObjMonLionCore.TwoIsNear());

        Assert.True(ObjMonLionCore.IsFar(3, 0));
        Assert.True(ObjMonLionCore.IsFar(0, 3));
        Assert.False(ObjMonLionCore.IsFar(2, 2));
    }

    [Fact]
    public void RunShellFacts()
    {
        Assert.True(ObjMonLionCore.RunIsPureShell());
        Assert.True(ObjMonLionCore.EighteenthOccurrence());
    }

    // ===================== 四、其他 =====================

    [Fact]
    public void Bt06Facts()
    {
        Assert.True(ObjMonLionCore.Bt06IsOutParam());
        Assert.True(ObjMonLionCore.SameNameDifferentRoleAsJ216());
        Assert.True(ObjMonLionCore.Bt06LinesChecked());

        Assert.Equal(new[] { 7498, 7602, 7609 }, ObjMonLionCore.Bt06Lines);
    }

    [Fact]
    public void MeleeExtrasFacts()
    {
        Assert.True(ObjMonLionCore.FocusTickOnMeleeOnly());
        Assert.True(ObjMonLionCore.BreakHolySeizeOnMeleeOnly());
        Assert.True(ObjMonLionCore.MeleeMeansEngaged());
        Assert.True(ObjMonLionCore.MeleeResultOutsideCooldown());
        Assert.True(ObjMonLionCore.BeamResultInnermost());
        Assert.True(ObjMonLionCore.DifferentDepthsInOneMethod());
    }

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonLionCore.ThirtyOneClassesCovered());
        Assert.True(ObjMonLionCore.RemainingApprox());
        Assert.True(ObjMonLionCore.BaseIsTATMonster());
        Assert.True(ObjMonLionCore.FirstBatchWithThisBase());
        Assert.True(ObjMonLionCore.OverridesGotoTargetXY());
        Assert.True(ObjMonLionCore.FirstGotoTargetXYOverride());
        Assert.True(ObjMonLionCore.DeclLinesChecked());
        Assert.True(ObjMonLionCore.GetAttackDirLinesChecked());
    }
}
