using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J223：`ObjMon.pas` 中 `TMagicAttackNotMoveMonster2.AttackTarget`
/// 的 1:1 测试（338 行，含四个嵌套函数）。
/// **本批最有价值的发现**：
/// ① `SingleAttack` 的麻痹判据**极性反了**（`UnParalysis` 少了 `not`）——
///    抗性越高越容易被麻痹、抗性 0 的反而永远不会；
/// ② `MoveTargetAttack`（68 行）是**死代码**，且其唯一调用点**实参个数写错**（0 参函数传了 1 个）；
/// ③ `m_dwHitTick` **只在攻击成功时**刷新 ⇒ 失败时每帧重试（J221 恰相反）；
/// ④ 同一句末尾判据在本类**不是恒真的**（因 `Exit` 在 `if Result then` 之内）。
/// </summary>
public sealed class ObjMonMagicNotMove2AttackCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(6984, ObjMonMagicNotMove2AttackCore.Start);
        Assert.Equal(7321, ObjMonMagicNotMove2AttackCore.End);
        Assert.Equal(338, ObjMonMagicNotMove2AttackCore.Lines);
        Assert.Equal(6986, ObjMonMagicNotMove2AttackCore.SingleStart);
        Assert.Equal(7049, ObjMonMagicNotMove2AttackCore.SingleEnd);
        Assert.Equal(64, ObjMonMagicNotMove2AttackCore.SingleLines);
        Assert.Equal(7050, ObjMonMagicNotMove2AttackCore.ThuderStart);
        Assert.Equal(7113, ObjMonMagicNotMove2AttackCore.ThuderEnd);
        Assert.Equal(64, ObjMonMagicNotMove2AttackCore.ThuderLines);
        Assert.Equal(7114, ObjMonMagicNotMove2AttackCore.MoveStart);
        Assert.Equal(7181, ObjMonMagicNotMove2AttackCore.MoveEnd);
        Assert.Equal(68, ObjMonMagicNotMove2AttackCore.MoveLines);
        Assert.Equal(7182, ObjMonMagicNotMove2AttackCore.GroupStart);
        Assert.Equal(7263, ObjMonMagicNotMove2AttackCore.GroupEnd);
        Assert.Equal(82, ObjMonMagicNotMove2AttackCore.GroupLines);
        Assert.Equal(7265, ObjMonMagicNotMove2AttackCore.OuterStart);
        Assert.Equal(7321, ObjMonMagicNotMove2AttackCore.OuterEnd);
        Assert.Equal(57, ObjMonMagicNotMove2AttackCore.OuterLines);
        Assert.Equal(4, ObjMonMagicNotMove2AttackCore.NestedCount);
        Assert.Equal(441, ObjMonMagicNotMove2AttackCore.ClassTotalLines);
        Assert.Equal(103, ObjMonMagicNotMove2AttackCore.AlreadyDoneLines);

        Assert.Equal("ThuderAttack", ObjMonMagicNotMove2AttackCore.WrongThunder);
        Assert.Equal("ThunderAttack", ObjMonMagicNotMove2AttackCore.RightThunder);
        Assert.Equal(7050, ObjMonMagicNotMove2AttackCore.ThuderDeclLine);
        Assert.Equal(7286, ObjMonMagicNotMove2AttackCore.ThuderCallLine);
        Assert.Equal(3, ObjMonMagicNotMove2AttackCore.SpellingErrorOrdinal);
        Assert.Equal(7266, ObjMonMagicNotMove2AttackCore.EffectTypeDeclLine);
        Assert.Equal(6, ObjMonMagicNotMove2AttackCore.EffectTypeSites);

        Assert.Equal(7277, ObjMonMagicNotMove2AttackCore.CaseLine);
        Assert.Equal(4, ObjMonMagicNotMove2AttackCore.CaseBound);
        Assert.Equal(7278, ObjMonMagicNotMove2AttackCore.Arm0Line);
        Assert.Equal(7280, ObjMonMagicNotMove2AttackCore.Arm0EffectLine);
        Assert.Equal(7281, ObjMonMagicNotMove2AttackCore.Arm0CallLine);
        Assert.Equal(7283, ObjMonMagicNotMove2AttackCore.Arm1Line);
        Assert.Equal(7285, ObjMonMagicNotMove2AttackCore.Arm1EffectLine);
        Assert.Equal(7286, ObjMonMagicNotMove2AttackCore.Arm1CallLine);
        Assert.Equal(7288, ObjMonMagicNotMove2AttackCore.DeadArmCommentStart);
        Assert.Equal(7294, ObjMonMagicNotMove2AttackCore.DeadArmCommentEnd);
        Assert.Equal(7291, ObjMonMagicNotMove2AttackCore.DeadArmEffectLine);
        Assert.Equal(7292, ObjMonMagicNotMove2AttackCore.DeadArmCallLine);
        Assert.Equal(7295, ObjMonMagicNotMove2AttackCore.ElseLine);
        Assert.Equal(7297, ObjMonMagicNotMove2AttackCore.ElseEffectLine);
        Assert.Equal(7298, ObjMonMagicNotMove2AttackCore.ElseCallLine);
        Assert.Equal(7301, ObjMonMagicNotMove2AttackCore.ResultGuardLine);
        Assert.Equal(7303, ObjMonMagicNotMove2AttackCore.HitTickSetLine);
        Assert.Equal(7304, ObjMonMagicNotMove2AttackCore.SendLine);
        Assert.Equal(7305, ObjMonMagicNotMove2AttackCore.OuterExitLine);
        Assert.Equal(7311, ObjMonMagicNotMove2AttackCore.TailCheckLine);
        Assert.Equal(6, ObjMonMagicNotMove2AttackCore.TailThreshold);
        Assert.Equal(7275, ObjMonMagicNotMove2AttackCore.RangeGateLine);
        Assert.Equal(7, ObjMonMagicNotMove2AttackCore.RangeGateThreshold);
        Assert.Equal(7313, ObjMonMagicNotMove2AttackCore.DiscardSameMapLine);
        Assert.Equal(7318, ObjMonMagicNotMove2AttackCore.DiscardOtherMapLine);
        Assert.Equal(7274, ObjMonMagicNotMove2AttackCore.CallSlaveLine);
        Assert.Equal(7271, ObjMonMagicNotMove2AttackCore.CooldownLine);
        Assert.Equal(7273, ObjMonMagicNotMove2AttackCore.HitDelayLine);
        Assert.Equal(6847, ObjMonMagicNotMove2AttackCore.J221HitTickLine);
        Assert.Equal(6834, ObjMonMagicNotMove2AttackCore.J221CallSlaveLine);
        Assert.Equal(6863, ObjMonMagicNotMove2AttackCore.J221SendLine);
        Assert.Equal(6869, ObjMonMagicNotMove2AttackCore.J221TailCheckLine);

        Assert.Equal(3, ObjMonMagicNotMove2AttackCore.TwinDiffs);
        Assert.Equal(60, ObjMonMagicNotMove2AttackCore.TwinIdenticalLines);
        Assert.Equal(7045, ObjMonMagicNotMove2AttackCore.ParalysisCondLine);
        Assert.Equal(7046, ObjMonMagicNotMove2AttackCore.ParalysisActLine);
        Assert.Equal(7109, ObjMonMagicNotMove2AttackCore.FrozenCondLine);
        Assert.Equal(7110, ObjMonMagicNotMove2AttackCore.FrozenActLine);
        Assert.Equal(3, ObjMonMagicNotMove2AttackCore.EffectRollBound);
        Assert.Equal(3, ObjMonMagicNotMove2AttackCore.DurationBase);
        Assert.Equal(3, ObjMonMagicNotMove2AttackCore.DurationMin);
        Assert.Equal(5, ObjMonMagicNotMove2AttackCore.DurationMax);
        Assert.Equal(6, ObjMonMagicNotMove2AttackCore.DurationFormOrdinal);
        Assert.Equal(5, ObjMonMagicNotMove2AttackCore.POISON_STONE);
        Assert.Equal(807, ObjMonMagicNotMove2AttackCore.UnParalysisDeclLine);
        Assert.Equal(24795, ObjMonMagicNotMove2AttackCore.GetUnParalysisImpl);
        Assert.Equal(7035, ObjMonMagicNotMove2AttackCore.GuardStartLine);
        Assert.Equal(7039, ObjMonMagicNotMove2AttackCore.GuardEndLine);
        Assert.Equal(6660, ObjMonMagicNotMove2AttackCore.J221ObjSendLine);

        Assert.Equal(7174, ObjMonMagicNotMove2AttackCore.LevelCheckLine);
        Assert.Equal(7176, ObjMonMagicNotMove2AttackCore.FrontPosLine);
        Assert.Equal(7177, ObjMonMagicNotMove2AttackCore.SpaceMoveLine);
        Assert.Equal(0, ObjMonMagicNotMove2AttackCore.SpaceMoveFourthArg);
        Assert.Equal(1, ObjMonMagicNotMove2AttackCore.FoxFourthArg);

        Assert.Equal(7191, ObjMonMagicNotMove2AttackCore.GroupResultLine);
        Assert.Equal(7193, ObjMonMagicNotMove2AttackCore.GroupMasterLine);
        Assert.Equal(7196, ObjMonMagicNotMove2AttackCore.GroupBaseDamageLine);
        Assert.Equal(7204, ObjMonMagicNotMove2AttackCore.GroupListLine);
        Assert.Equal(7205, ObjMonMagicNotMove2AttackCore.GroupTryLine);
        Assert.Equal(7206, ObjMonMagicNotMove2AttackCore.GroupGetMapLine);
        Assert.Equal(5, ObjMonMagicNotMove2AttackCore.GroupRadius);
        Assert.Equal(7207, ObjMonMagicNotMove2AttackCore.RandomizeLine);
        Assert.Equal(7217, ObjMonMagicNotMove2AttackCore.LoopPowerRateAddLine);
        Assert.Equal(7218, ObjMonMagicNotMove2AttackCore.LoopNextDamageLine);
        Assert.Equal(7220, ObjMonMagicNotMove2AttackCore.LoopPowerMaxLine);
        Assert.Equal(7005, ObjMonMagicNotMove2AttackCore.SinglePowerRateAddLine);
        Assert.Equal(7006, ObjMonMagicNotMove2AttackCore.SingleNextDamageLine);
        Assert.Equal(7008, ObjMonMagicNotMove2AttackCore.SinglePowerMaxLine);
        Assert.Equal(7248, ObjMonMagicNotMove2AttackCore.DebuffCaseLine);
        Assert.Equal(5, ObjMonMagicNotMove2AttackCore.DebuffBound);
        Assert.Equal("5, 30, True", ObjMonMagicNotMove2AttackCore.DebuffCommonArgs);
        Assert.Equal(7260, ObjMonMagicNotMove2AttackCore.GroupFinallyLine);
        Assert.Equal(7261, ObjMonMagicNotMove2AttackCore.GroupFreeLine);
        Assert.Equal(5, ObjMonMagicNotMove2AttackCore.RandomizeSites);
        Assert.Equal(29, ObjMonMagicNotMove2AttackCore.ClassesCovered);
        Assert.Equal(25, ObjMonMagicNotMove2AttackCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.SpanMatches());
        Assert.True(ObjMonMagicNotMove2AttackCore.DecompositionAddsUp());
        Assert.True(ObjMonMagicNotMove2AttackCore.NestedLinesAddUp());
        Assert.True(ObjMonMagicNotMove2AttackCore.NestedSpansMatch());
        Assert.True(ObjMonMagicNotMove2AttackCore.ClassTotalAddsUp());
        Assert.True(ObjMonMagicNotMove2AttackCore.NestedBeforeOuter());
        Assert.True(ObjMonMagicNotMove2AttackCore.WithinUnit());
        Assert.True(ObjMonMagicNotMove2AttackCore.NoInstrumentation());
    }

    // ===================== 一、四个嵌套函数 =====================

    [Fact]
    public void NestedFunctionFacts()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.FourNestedFunctions());
        Assert.True(ObjMonMagicNotMove2AttackCore.MostSoFar());
        Assert.True(ObjMonMagicNotMove2AttackCore.AllReturnBoolean());
        Assert.True(ObjMonMagicNotMove2AttackCore.NestedTableExtracted());
        Assert.True(ObjMonMagicNotMove2AttackCore.OnlyOneIsDead());
        Assert.True(ObjMonMagicNotMove2AttackCore.DeadOneIsMoveTarget());

        Assert.Equal(4, ObjMonMagicNotMove2AttackCore.NestedTable.Length);
        Assert.Equal("SingleAttack", ObjMonMagicNotMove2AttackCore.NestedTable[0].Func);
        Assert.True(ObjMonMagicNotMove2AttackCore.NestedTable[0].Called);
        Assert.Equal("ThuderAttack", ObjMonMagicNotMove2AttackCore.NestedTable[1].Func);
        Assert.True(ObjMonMagicNotMove2AttackCore.NestedTable[1].Called);
        Assert.Equal("MoveTargetAttack", ObjMonMagicNotMove2AttackCore.NestedTable[2].Func);
        Assert.False(ObjMonMagicNotMove2AttackCore.NestedTable[2].Called);
        Assert.Equal("GroupAttack", ObjMonMagicNotMove2AttackCore.NestedTable[3].Func);
        Assert.True(ObjMonMagicNotMove2AttackCore.NestedTable[3].Called);
    }

    [Fact]
    public void DeadCodeFacts()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.MoveTargetIsDeadCode());
        Assert.True(ObjMonMagicNotMove2AttackCore.OnlyCallSiteCommented());
        Assert.True(ObjMonMagicNotMove2AttackCore.CallPassesOneArgToZeroArgFunction());
        Assert.True(ObjMonMagicNotMove2AttackCore.WouldNotCompileIfUncommented());
        Assert.True(ObjMonMagicNotMove2AttackCore.TwoReasonsForRetirement());
        Assert.Equal(7292, ObjMonMagicNotMove2AttackCore.DeadArmCallLine);
    }

    [Fact]
    public void SpellingFacts()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.ThuderMisspelling());
        Assert.True(ObjMonMagicNotMove2AttackCore.ThirdSpellingError());
        Assert.True(ObjMonMagicNotMove2AttackCore.ReusedInSibling());
        Assert.True(ObjMonMagicNotMove2AttackCore.SameFamilyAsJ202AndJ221());
        Assert.True(ObjMonMagicNotMove2AttackCore.SpellingTableExtracted());
        Assert.True(ObjMonMagicNotMove2AttackCore.ThreeDistinctCorrections());
        Assert.True(ObjMonMagicNotMove2AttackCore.MisspellingAppearsTwice());

        Assert.Equal(3, ObjMonMagicNotMove2AttackCore.SpellingErrors.Length);
        Assert.Equal("Poision", ObjMonMagicNotMove2AttackCore.SpellingErrors[0].Wrong);
        Assert.Equal("Poison", ObjMonMagicNotMove2AttackCore.SpellingErrors[0].Right);
        Assert.Equal("nEfftctType", ObjMonMagicNotMove2AttackCore.SpellingErrors[1].Wrong);
        Assert.Equal("ThuderAttack", ObjMonMagicNotMove2AttackCore.SpellingErrors[2].Wrong);
        Assert.Equal("ThunderAttack", ObjMonMagicNotMove2AttackCore.SpellingErrors[2].Right);
    }

    // ===================== 二、nEfftctType =====================

    [Fact]
    public void EffectTypeFormFacts()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.NotABitmaskHere());
        Assert.True(ObjMonMagicNotMove2AttackCore.PlainAssignment());
        Assert.True(ObjMonMagicNotMove2AttackCore.ValuesOneTwoThree());
        Assert.True(ObjMonMagicNotMove2AttackCore.DeadFour());
        Assert.True(ObjMonMagicNotMove2AttackCore.SameNameDifferentSemantics());
        Assert.True(ObjMonMagicNotMove2AttackCore.SharedTypoToo());
        Assert.True(ObjMonMagicNotMove2AttackCore.SixSites());
        Assert.True(ObjMonMagicNotMove2AttackCore.FourOnlyInDeadCode());
        Assert.True(ObjMonMagicNotMove2AttackCore.LiveValuesAreOneTwoThree());
        Assert.True(ObjMonMagicNotMove2AttackCore.FourReservedForDeadArm());
        Assert.True(ObjMonMagicNotMove2AttackCore.EffectTypeFormsExtracted());
        Assert.True(ObjMonMagicNotMove2AttackCore.EffectTypeLinesChecked());

        Assert.Equal(2, ObjMonMagicNotMove2AttackCore.EffectTypeForms.Length);
        Assert.Equal(new[] { 1, 2, 3 }, ObjMonMagicNotMove2AttackCore.LiveValues());
        Assert.Equal(new[] { 7266, 7280, 7285, 7291, 7297, 7304 },
            ObjMonMagicNotMove2AttackCore.EffectTypeLines);
    }

    [Fact]
    public void CaseArmFacts()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.CaseWithDeletedArm());
        Assert.True(ObjMonMagicNotMove2AttackCore.ElseTakesHalf());
        Assert.True(ObjMonMagicNotMove2AttackCore.GroupProbabilityDoubled());
        Assert.True(ObjMonMagicNotMove2AttackCore.EffectiveRates());
        Assert.True(ObjMonMagicNotMove2AttackCore.RollZeroSingle());
        Assert.True(ObjMonMagicNotMove2AttackCore.RollOneThuder());
        Assert.True(ObjMonMagicNotMove2AttackCore.RollsTwoAndThreeGroup());
        Assert.True(ObjMonMagicNotMove2AttackCore.GroupShareIsHalf());
        Assert.True(ObjMonMagicNotMove2AttackCore.ThreeDistinctOutcomes());
    }

    [Fact]
    public void CaseArmBoundaries()
    {
        Assert.Equal("SingleAttack", ObjMonMagicNotMove2AttackCore.PickArm(0));
        Assert.Equal("ThuderAttack", ObjMonMagicNotMove2AttackCore.PickArm(1));
        Assert.Equal("GroupAttack", ObjMonMagicNotMove2AttackCore.PickArm(2));
        Assert.Equal("GroupAttack", ObjMonMagicNotMove2AttackCore.PickArm(3));

        // **群攻占 2/4（因 arm 2 被删）**
        int group = 0;

        for (int r = 0; r < ObjMonMagicNotMove2AttackCore.CaseBound; r++)
        {
            if (ObjMonMagicNotMove2AttackCore.PickArm(r) == "GroupAttack")
                group++;
        }

        Assert.Equal(2, group);
    }

    // ===================== 三、两胞胎与麻痹极性 =====================

    [Fact]
    public void TwinFacts()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.TwinsSixtyOfSixtyThree());
        Assert.True(ObjMonMagicNotMove2AttackCore.OnlyThreeDiffs());
        Assert.True(ObjMonMagicNotMove2AttackCore.SharedDamagePipeline());
        Assert.True(ObjMonMagicNotMove2AttackCore.StrongestCopyPasteEvidence());
        Assert.True(ObjMonMagicNotMove2AttackCore.TwinArithmetic());
        Assert.True(ObjMonMagicNotMove2AttackCore.TwinsSameLength());
    }

    [Fact]
    public void ParalysisPolarityFacts()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.UnParalysisWithoutNot());
        Assert.True(ObjMonMagicNotMove2AttackCore.InvertedPolarity());
        Assert.True(ObjMonMagicNotMove2AttackCore.OnlyReversedSiteInSeries());
        Assert.True(ObjMonMagicNotMove2AttackCore.HighResistGetsParalysed());
        Assert.True(ObjMonMagicNotMove2AttackCore.ZeroResistNeverParalysed());
        Assert.True(ObjMonMagicNotMove2AttackCore.TheTwoAreComplements());
        Assert.True(ObjMonMagicNotMove2AttackCore.OppositeAtZeroResist());
        Assert.True(ObjMonMagicNotMove2AttackCore.GetterLinesChecked());
    }

    [Fact]
    public void ParalysisBoundaries()
    {
        // **反写版：抗性 100 时任何掷骰都能上毒、抗性 0 时永远不能**
        Assert.True(ObjMonMagicNotMove2AttackCore.ParalysisFiresInverted(100, 0));
        Assert.True(ObjMonMagicNotMove2AttackCore.ParalysisFiresInverted(100, 99));
        Assert.False(ObjMonMagicNotMove2AttackCore.ParalysisFiresInverted(0, 0));
        Assert.False(ObjMonMagicNotMove2AttackCore.ParalysisFiresInverted(50, 50));

        // **正确版恰好相反**
        Assert.False(ObjMonMagicNotMove2AttackCore.ParalysisFiresCorrect(100, 0));
        Assert.True(ObjMonMagicNotMove2AttackCore.ParalysisFiresCorrect(0, 0));
    }

    [Fact]
    public void DurationFacts()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.SixthDurationForm());
        Assert.True(ObjMonMagicNotMove2AttackCore.ThreeToFive());
        Assert.True(ObjMonMagicNotMove2AttackCore.SameFormulaForBothEffects());
        Assert.True(ObjMonMagicNotMove2AttackCore.UniqueAmongClasses());
        Assert.True(ObjMonMagicNotMove2AttackCore.MinDuration());
        Assert.True(ObjMonMagicNotMove2AttackCore.MaxDuration());
        Assert.True(ObjMonMagicNotMove2AttackCore.OneInThree());
        Assert.True(ObjMonMagicNotMove2AttackCore.RollZeroFires());
        Assert.True(ObjMonMagicNotMove2AttackCore.OthersDoNot());

        Assert.Equal(3, ObjMonMagicNotMove2AttackCore.Duration(0));
        Assert.Equal(4, ObjMonMagicNotMove2AttackCore.Duration(1));
        Assert.Equal(5, ObjMonMagicNotMove2AttackCore.Duration(2));
        Assert.True(ObjMonMagicNotMove2AttackCore.EffectFires(0));
        Assert.False(ObjMonMagicNotMove2AttackCore.EffectFires(1));
        Assert.False(ObjMonMagicNotMove2AttackCore.EffectFires(2));
    }

    [Fact]
    public void GuardAndSendFacts()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.FivePartGuard());
        Assert.True(ObjMonMagicNotMove2AttackCore.CompoundNegation());
        Assert.True(ObjMonMagicNotMove2AttackCore.AcceptFormHideFilter());
        Assert.True(ObjMonMagicNotMove2AttackCore.CombinationOfTwoCensusedForms());
        Assert.True(ObjMonMagicNotMove2AttackCore.ZeroCheckAfterAbsorb());
        Assert.True(ObjMonMagicNotMove2AttackCore.GuardedElementStep());
        Assert.True(ObjMonMagicNotMove2AttackCore.SingleZeroCheck());
        Assert.True(ObjMonMagicNotMove2AttackCore.RecipientNotCastMessage());
        Assert.True(ObjMonMagicNotMove2AttackCore.MessageIdDiffersFromSibling());
        Assert.True(ObjMonMagicNotMove2AttackCore.SameCallDifferentMeaning());

        Assert.Equal(new[] { 7033, 7097 }, ObjMonMagicNotMove2AttackCore.ZeroCheckLines);
        Assert.Equal(new[] { 7043, 7107 }, ObjMonMagicNotMove2AttackCore.SendLines);
    }

    [Fact]
    public void FilterBoundaries()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.ConfigOffAttacks());
        Assert.True(ObjMonMagicNotMove2AttackCore.OfflineExcluded());
        Assert.True(ObjMonMagicNotMove2AttackCore.ShouldAttack(false, true, true));
        Assert.False(ObjMonMagicNotMove2AttackCore.ShouldAttack(true, true, true));
        Assert.True(ObjMonMagicNotMove2AttackCore.ShouldAttack(true, true, false));

        Assert.True(ObjMonMagicNotMove2AttackCore.NotHiddenPasses());
        Assert.True(ObjMonMagicNotMove2AttackCore.HiddenWithCoolEyePasses());
        Assert.True(ObjMonMagicNotMove2AttackCore.HiddenNoCoolEyeBlocked());
    }

    // ===================== 四、MoveTargetAttack =====================

    [Fact]
    public void MoveTargetFacts()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.MovesTheTargetNotSelf());
        Assert.True(ObjMonMagicNotMove2AttackCore.OnlyIfTargetHigherLevel());
        Assert.True(ObjMonMagicNotMove2AttackCore.ComplementsCannotMove());
        Assert.True(ObjMonMagicNotMove2AttackCore.RetiredAnyway());
        Assert.True(ObjMonMagicNotMove2AttackCore.SpaceMoveFourthArgZero());
        Assert.True(ObjMonMagicNotMove2AttackCore.FoxPassedOne());
        Assert.True(ObjMonMagicNotMove2AttackCore.OpaqueParamDifferentValues());
        Assert.True(ObjMonMagicNotMove2AttackCore.MapFromTargetCoordsFromSelf());
        Assert.True(ObjMonMagicNotMove2AttackCore.CrossMapConfusionIfLive());
        Assert.True(ObjMonMagicNotMove2AttackCore.MootBecauseDead());
        Assert.True(ObjMonMagicNotMove2AttackCore.FrontPosChecked());
    }

    [Fact]
    public void MoveTargetBoundaries()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.StrongerTargetMoved());
        Assert.True(ObjMonMagicNotMove2AttackCore.WeakerTargetKept());
        Assert.True(ObjMonMagicNotMove2AttackCore.EqualLevelKept());

        Assert.True(ObjMonMagicNotMove2AttackCore.ShouldMoveTarget(10, 20));
        Assert.False(ObjMonMagicNotMove2AttackCore.ShouldMoveTarget(20, 10));
        Assert.False(ObjMonMagicNotMove2AttackCore.ShouldMoveTarget(10, 10));
    }

    // ===================== 五、GroupAttack =====================

    [Fact]
    public void GroupAttackFacts()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.StartsTrue());
        Assert.True(ObjMonMagicNotMove2AttackCore.OthersStartFalse());
        Assert.True(ObjMonMagicNotMove2AttackCore.AlwaysReportsSuccess());
        Assert.True(ObjMonMagicNotMove2AttackCore.SkipsTargetGuard());
        Assert.True(ObjMonMagicNotMove2AttackCore.ThreeStepsMovedIntoLoop());
        Assert.True(ObjMonMagicNotMove2AttackCore.CapPerTargetVsOnce());
        Assert.True(ObjMonMagicNotMove2AttackCore.BaseFromPrimaryTarget());
        Assert.True(ObjMonMagicNotMove2AttackCore.TryFinallyPresent());
        Assert.True(ObjMonMagicNotMove2AttackCore.OnlyGroupBuildsList());
        Assert.True(ObjMonMagicNotMove2AttackCore.ThirdConfirmationOfTheRule());
        Assert.True(ObjMonMagicNotMove2AttackCore.FreeInFinally());
    }

    [Fact]
    public void RadiusAndCombinationFacts()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.HardcodedFive());
        Assert.True(ObjMonMagicNotMove2AttackCore.TargetCentered());
        Assert.True(ObjMonMagicNotMove2AttackCore.EighthCombination());
        Assert.True(ObjMonMagicNotMove2AttackCore.FiveRadiusValues());
        Assert.True(ObjMonMagicNotMove2AttackCore.GroupCombosExtracted());
        Assert.True(ObjMonMagicNotMove2AttackCore.NewCombination());

        Assert.Equal(8, ObjMonMagicNotMove2AttackCore.GroupCombos.Length);
        Assert.Equal("J223", ObjMonMagicNotMove2AttackCore.GroupCombos[7].Batch);
        Assert.Equal("hardcoded 5", ObjMonMagicNotMove2AttackCore.GroupCombos[7].Radius);
        Assert.Equal("target", ObjMonMagicNotMove2AttackCore.GroupCombos[7].Center);
    }

    [Fact]
    public void RandomizeFacts()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.FourthRandomizeSite());
        Assert.True(ObjMonMagicNotMove2AttackCore.ThreeInThisFeatureCluster());
        Assert.True(ObjMonMagicNotMove2AttackCore.AuthorsHabitConfirmed());
        Assert.True(ObjMonMagicNotMove2AttackCore.RandomizeTableExtracted());

        Assert.Equal(new[] { 1621, 6480, 6788, 7207, 9126 },
            ObjMonMagicNotMove2AttackCore.RandomizeLines);
        Assert.Equal(new[] { 6480, 6788, 7207 },
            ObjMonMagicNotMove2AttackCore.FeatureClusterRandomize);
    }

    [Fact]
    public void DebuffFacts()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.CaseRandomFiveNoElse());
        Assert.True(ObjMonMagicNotMove2AttackCore.TwoOfFiveDebuff());
        Assert.True(ObjMonMagicNotMove2AttackCore.SilentThreeOutOfFive());
        Assert.True(ObjMonMagicNotMove2AttackCore.ArmsDifferOnlyInFirstArg());
        Assert.True(ObjMonMagicNotMove2AttackCore.RollZeroDebuffsZero());
        Assert.True(ObjMonMagicNotMove2AttackCore.RollOneDebuffsThree());
        Assert.True(ObjMonMagicNotMove2AttackCore.RollsTwoToFourNothing());

        Assert.Equal(new[] { 0, 3 }, ObjMonMagicNotMove2AttackCore.DebuffFirstArgs);
        Assert.Equal(new[] { 7251, 7255 },
            ObjMonMagicNotMove2AttackCore.DebuffArmLines);
    }

    [Fact]
    public void DebuffBoundaries()
    {
        Assert.Equal(0, ObjMonMagicNotMove2AttackCore.DebuffIndex(0));
        Assert.Equal(3, ObjMonMagicNotMove2AttackCore.DebuffIndex(1));
        Assert.Null(ObjMonMagicNotMove2AttackCore.DebuffIndex(2));
        Assert.Null(ObjMonMagicNotMove2AttackCore.DebuffIndex(3));
        Assert.Null(ObjMonMagicNotMove2AttackCore.DebuffIndex(4));
    }

    [Fact]
    public void ReboundFacts()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.NoReboundAnywhere());
        Assert.True(ObjMonMagicNotMove2AttackCore.SiblingHasRebound());
        Assert.True(ObjMonMagicNotMove2AttackCore.OppositeChoices());
    }

    // ===================== 六、外层体 =====================

    [Fact]
    public void OuterBodyFacts()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.CallSlaveUnconditional());
        Assert.True(ObjMonMagicNotMove2AttackCore.SiblingUsesNarrowBand());
        Assert.True(ObjMonMagicNotMove2AttackCore.ReliesOnInnerGuard());
        Assert.True(ObjMonMagicNotMove2AttackCore.TriesOnFirstTick());
        Assert.True(ObjMonMagicNotMove2AttackCore.HitTickOnlyOnSuccess());
        Assert.True(ObjMonMagicNotMove2AttackCore.InsideResultGuard());
        Assert.True(ObjMonMagicNotMove2AttackCore.RetryEveryTickOnFailure());
        Assert.True(ObjMonMagicNotMove2AttackCore.OppositeOfJ221());
    }

    [Fact]
    public void LivenessFacts()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.NotTautologicalHere());
        Assert.True(ObjMonMagicNotMove2AttackCore.BecauseExitIsConditional());
        Assert.True(ObjMonMagicNotMove2AttackCore.TwoFallthroughCases());
        Assert.True(ObjMonMagicNotMove2AttackCore.DirectContrastWithJ221());
        Assert.True(ObjMonMagicNotMove2AttackCore.SameCodeDifferentLiveness());
        Assert.True(ObjMonMagicNotMove2AttackCore.J221IsTautological());
        Assert.True(ObjMonMagicNotMove2AttackCore.ThisIsNotTautological());
    }

    [Fact]
    public void LivenessTable()
    {
        // **`Exit` 无条件 + 门 >= 判据 => 恒真（J221/J219）**
        Assert.True(ObjMonMagicNotMove2AttackCore.CheckIsTautological(7, 6, true));
        Assert.True(ObjMonMagicNotMove2AttackCore.CheckIsTautological(6, 6, true));

        // **`Exit` 有条件 => 不恒真（本类）**
        Assert.False(ObjMonMagicNotMove2AttackCore.CheckIsTautological(7, 6, false));

        // **门 < 判据时即使是无条件也不恒真（J215 模板）**
        Assert.False(ObjMonMagicNotMove2AttackCore.CheckIsTautological(5, 6, true));
    }

    [Fact]
    public void DegenerateBranchFacts()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.BothDiscardAgain());
        Assert.True(ObjMonMagicNotMove2AttackCore.ThirdOccurrence());
        Assert.True(ObjMonMagicNotMove2AttackCore.ShallowerDegeneracy());
        Assert.True(ObjMonMagicNotMove2AttackCore.ConditionMeaningfulButBodiesSame());
    }

    [Fact]
    public void SendGuardFacts()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.SendInsideResultGuard());
        Assert.True(ObjMonMagicNotMove2AttackCore.SiblingSendsUnconditionally());
        Assert.True(ObjMonMagicNotMove2AttackCore.NoZeroEffectHere());
        Assert.True(ObjMonMagicNotMove2AttackCore.J221CanSendZero());
        Assert.True(ObjMonMagicNotMove2AttackCore.RangeGateIsSeven());
        Assert.True(ObjMonMagicNotMove2AttackCore.TailIsSix());
        Assert.True(ObjMonMagicNotMove2AttackCore.ThresholdsDiffer());
    }

    [Fact]
    public void CooldownBoundaries()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.FreshTickBlocks());
        Assert.True(ObjMonMagicNotMove2AttackCore.StaleTickAllows());

        Assert.False(ObjMonMagicNotMove2AttackCore.ShouldAttempt(1000, 1000, 100, 0));
        Assert.True(ObjMonMagicNotMove2AttackCore.ShouldAttempt(1000, 2000, 100, 0));
        Assert.False(ObjMonMagicNotMove2AttackCore.ShouldAttempt(1000, 1100, 100, 0));
    }

    // ===================== 七、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonMagicNotMove2AttackCore.TwentyNineClassesCovered());
        Assert.True(ObjMonMagicNotMove2AttackCore.RemainingApprox());
    }
}
