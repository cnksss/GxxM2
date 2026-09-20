using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J221：`ObjMon.pas` 中 `TMagicAttackNotMoveMonster.AttackTarget`
/// 的 1:1 测试（276 行，含三个嵌套过程）。
/// **本批最有价值的发现**：
/// ① 外层体末尾的"尝试删除攻击目标"判据是**恒真**的 ——
///    而这是"把概率门改成位置门"的副作用（J215 模板里它本来有意义、J219 同形）；
/// ② `nEfftctType` 既拼错了、又是个 `1/2/4` 位掩码、被当作"特效编号"发出；
/// ③ `MagicAttack3` 不给位掩码加位、自己也不发消息 ⇒ 状态改了但视觉层没通知；
/// ④ `if nCur > nMax then Break;` 差一 ⇒ 实际最多冻 5 次而非 4 次。
/// 另更正 J220 的两处常量（277→276、368→367）。
/// </summary>
public sealed class ObjMonMagicNotMoveAttackCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(6604, ObjMonMagicNotMoveAttackCore.Start);
        Assert.Equal(6879, ObjMonMagicNotMoveAttackCore.End);
        Assert.Equal(276, ObjMonMagicNotMoveAttackCore.Lines);
        Assert.Equal(6606, ObjMonMagicNotMoveAttackCore.MA1Start);
        Assert.Equal(6684, ObjMonMagicNotMoveAttackCore.MA1End);
        Assert.Equal(79, ObjMonMagicNotMoveAttackCore.MA1Lines);
        Assert.Equal(6688, ObjMonMagicNotMoveAttackCore.MA2Start);
        Assert.Equal(6756, ObjMonMagicNotMoveAttackCore.MA2End);
        Assert.Equal(69, ObjMonMagicNotMoveAttackCore.MA2Lines);
        Assert.Equal(6760, ObjMonMagicNotMoveAttackCore.MA3Start);
        Assert.Equal(6820, ObjMonMagicNotMoveAttackCore.MA3End);
        Assert.Equal(61, ObjMonMagicNotMoveAttackCore.MA3Lines);
        Assert.Equal(6822, ObjMonMagicNotMoveAttackCore.OuterStart);
        Assert.Equal(6879, ObjMonMagicNotMoveAttackCore.OuterEnd);
        Assert.Equal(58, ObjMonMagicNotMoveAttackCore.OuterLines);
        Assert.Equal(3, ObjMonMagicNotMoveAttackCore.NestedCount);
        Assert.Equal(367, ObjMonMagicNotMoveAttackCore.ClassTotalLines);
        Assert.Equal(277, ObjMonMagicNotMoveAttackCore.J220RecordedLines);
        Assert.Equal(368, ObjMonMagicNotMoveAttackCore.J220RecordedClassTotal);

        Assert.Equal(6823, ObjMonMagicNotMoveAttackCore.EffectTypeDeclLine);
        Assert.Equal(6, ObjMonMagicNotMoveAttackCore.EffectTypeSites);
        Assert.Equal("nEffectType", ObjMonMagicNotMoveAttackCore.CorrectSpelling);
        Assert.Equal("nEfftctType", ObjMonMagicNotMoveAttackCore.ActualSpelling);
        Assert.Equal(7, ObjMonMagicNotMoveAttackCore.EffectTypeMax);
        Assert.Equal(1, ObjMonMagicNotMoveAttackCore.BitNetLightning);
        Assert.Equal(2, ObjMonMagicNotMoveAttackCore.BitParalysis);
        Assert.Equal(4, ObjMonMagicNotMoveAttackCore.BitFrozen);
        Assert.Equal(8, ObjMonMagicNotMoveAttackCore.BitForeverFrozen);

        Assert.Equal(6828, ObjMonMagicNotMoveAttackCore.CooldownLine);
        Assert.Equal(6830, ObjMonMagicNotMoveAttackCore.HitDelayResetLine);
        Assert.Equal(6834, ObjMonMagicNotMoveAttackCore.CallSlaveLine);
        Assert.Equal(6837, ObjMonMagicNotMoveAttackCore.HpBelow80Line);
        Assert.Equal(6840, ObjMonMagicNotMoveAttackCore.ForeverFrozenCooldownLine);
        Assert.Equal(45000, ObjMonMagicNotMoveAttackCore.ForeverFrozenCooldownMs);
        Assert.Equal(6842, ObjMonMagicNotMoveAttackCore.ForeverFrozenTickWriteLine);
        Assert.Equal(6843, ObjMonMagicNotMoveAttackCore.MA3CallLine);
        Assert.Equal(6846, ObjMonMagicNotMoveAttackCore.EffectTypeResetLine);
        Assert.Equal(6847, ObjMonMagicNotMoveAttackCore.HitTickSetLine);
        Assert.Equal(2, ObjMonMagicNotMoveAttackCore.NetLightningRollBound);
        Assert.Equal(6850, ObjMonMagicNotMoveAttackCore.MA2CallLine);
        Assert.Equal(6851, ObjMonMagicNotMoveAttackCore.PlusOneLine);
        Assert.Equal(6853, ObjMonMagicNotMoveAttackCore.RangeGate7Line);
        Assert.Equal(6855, ObjMonMagicNotMoveAttackCore.MA1CallParalysisLine);
        Assert.Equal(6856, ObjMonMagicNotMoveAttackCore.PlusTwoLine);
        Assert.Equal(6857, ObjMonMagicNotMoveAttackCore.RangeGate3Line);
        Assert.Equal(6859, ObjMonMagicNotMoveAttackCore.MA1CallFrozenLine);
        Assert.Equal(6860, ObjMonMagicNotMoveAttackCore.PlusFourLine);
        Assert.Equal(6862, ObjMonMagicNotMoveAttackCore.ResultTrueLine);
        Assert.Equal(6863, ObjMonMagicNotMoveAttackCore.SendLine);
        Assert.Equal(6864, ObjMonMagicNotMoveAttackCore.OuterExitLine);
        Assert.Equal(6867, ObjMonMagicNotMoveAttackCore.SameMapLine);
        Assert.Equal(6869, ObjMonMagicNotMoveAttackCore.TautologicalCheckLine);
        Assert.Equal(6871, ObjMonMagicNotMoveAttackCore.DeleteSameMapLine);
        Assert.Equal(6876, ObjMonMagicNotMoveAttackCore.DeleteOtherMapLine);
        Assert.Equal(6, ObjMonMagicNotMoveAttackCore.TautologicalThreshold);
        Assert.Equal(5670, ObjMonMagicNotMoveAttackCore.TemplateCheckLine);
        Assert.Equal(5661, ObjMonMagicNotMoveAttackCore.TemplateGateLine);

        Assert.Equal(6662, ObjMonMagicNotMoveAttackCore.CaseLine);
        Assert.Equal(6663, ObjMonMagicNotMoveAttackCore.Arm1Line);
        Assert.Equal(6665, ObjMonMagicNotMoveAttackCore.BraceCommentLine);
        Assert.Equal(12, ObjMonMagicNotMoveAttackCore.ParalysisRollBound);
        Assert.Equal(6667, ObjMonMagicNotMoveAttackCore.MakePosionLine);
        Assert.Equal(6670, ObjMonMagicNotMoveAttackCore.Arm2Line);
        Assert.Equal(8, ObjMonMagicNotMoveAttackCore.FrozenRollBound);
        Assert.Equal(6673, ObjMonMagicNotMoveAttackCore.MakeFrozenLine);
        Assert.Equal(3, ObjMonMagicNotMoveAttackCore.ArmDurationSeconds);
        Assert.Equal(5, ObjMonMagicNotMoveAttackCore.POISON_STONE);
        Assert.Equal(6683, ObjMonMagicNotMoveAttackCore.CommentedSendLine);
        Assert.Equal(200, ObjMonMagicNotMoveAttackCore.CommentedSendDelayFactor);
        Assert.Equal(6614, ObjMonMagicNotMoveAttackCore.MA1NilGuardLine);
        Assert.Equal(6697, ObjMonMagicNotMoveAttackCore.MA2NilGuardLine);
        Assert.Equal(6686, ObjMonMagicNotMoveAttackCore.MA2CommentLine);
        Assert.Equal(6758, ObjMonMagicNotMoveAttackCore.MA3CommentLine);

        Assert.Equal(6713, ObjMonMagicNotMoveAttackCore.DirLoopLine);
        Assert.Equal(7, ObjMonMagicNotMoveAttackCore.DirMax);
        Assert.Equal(6715, ObjMonMagicNotMoveAttackCore.DistanceLoopLine);
        Assert.Equal(8, ObjMonMagicNotMoveAttackCore.DistanceMax);
        Assert.Equal(6717, ObjMonMagicNotMoveAttackCore.GetNextPositionLine);
        Assert.Equal(6719, ObjMonMagicNotMoveAttackCore.GetMovingObjectLine);
        Assert.Equal(6720, ObjMonMagicNotMoveAttackCore.MA2FilterLine);
        Assert.Equal(6725, ObjMonMagicNotMoveAttackCore.PowerSwapLine);
        Assert.Equal(6627, ObjMonMagicNotMoveAttackCore.MA1NextDamageLine);
        Assert.Equal(6629, ObjMonMagicNotMoveAttackCore.MA1PowerMaxLine);
        Assert.Equal(6625, ObjMonMagicNotMoveAttackCore.MA1PowerRateAddLine);
        Assert.Equal(6750, ObjMonMagicNotMoveAttackCore.MA2StruckLine);

        Assert.Equal(6767, ObjMonMagicNotMoveAttackCore.MA3ListCreateLine);
        Assert.Equal(6768, ObjMonMagicNotMoveAttackCore.MA3TryLine);
        Assert.Equal(6770, ObjMonMagicNotMoveAttackCore.MA3GetMapLine);
        Assert.Equal(8, ObjMonMagicNotMoveAttackCore.MA3Radius);
        Assert.Equal(6772, ObjMonMagicNotMoveAttackCore.MA3FilterLoopLine);
        Assert.Equal(6775, ObjMonMagicNotMoveAttackCore.MA3FilterLine);
        Assert.Equal(6787, ObjMonMagicNotMoveAttackCore.NMaxLine);
        Assert.Equal(4, ObjMonMagicNotMoveAttackCore.NMaxCap);
        Assert.Equal(6788, ObjMonMagicNotMoveAttackCore.RandomizeLine);
        Assert.Equal(6789, ObjMonMagicNotMoveAttackCore.PickLoopLine);
        Assert.Equal(6791, ObjMonMagicNotMoveAttackCore.RandomPickLine);
        Assert.Equal(6797, ObjMonMagicNotMoveAttackCore.ParenStarStart);
        Assert.Equal(6806, ObjMonMagicNotMoveAttackCore.ParenStarEnd);
        Assert.Equal(6808, ObjMonMagicNotMoveAttackCore.MA3ResistLine);
        Assert.Equal(6810, ObjMonMagicNotMoveAttackCore.OpenForeverFrozenLine);
        Assert.Equal(5, ObjMonMagicNotMoveAttackCore.ForeverFrozenSeconds);
        Assert.Equal(6811, ObjMonMagicNotMoveAttackCore.IncNCurLine);
        Assert.Equal(6813, ObjMonMagicNotMoveAttackCore.BreakLine);
        Assert.Equal(6817, ObjMonMagicNotMoveAttackCore.MA3FinallyLine);
        Assert.Equal(6818, ObjMonMagicNotMoveAttackCore.MA3FreeLine);
        Assert.Equal(5, ObjMonMagicNotMoveAttackCore.RandomizeSites);
        Assert.Equal(27, ObjMonMagicNotMoveAttackCore.ClassesCovered);
        Assert.Equal(27, ObjMonMagicNotMoveAttackCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.SpanMatches());
        Assert.True(ObjMonMagicNotMoveAttackCore.NestedSpansMatch());
        Assert.True(ObjMonMagicNotMoveAttackCore.DecompositionAddsUp());
        Assert.True(ObjMonMagicNotMoveAttackCore.LinesAddUp());
        Assert.True(ObjMonMagicNotMoveAttackCore.ClassTotalAddsUp());
        Assert.True(ObjMonMagicNotMoveAttackCore.NestedBeforeOuter());
        Assert.True(ObjMonMagicNotMoveAttackCore.WithinUnit());
        Assert.True(ObjMonMagicNotMoveAttackCore.NoInstrumentation());
    }

    // ===================== 〇、对 J220 的更正 =====================

    [Fact]
    public void J220CorrectionFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.CorrectsJ220LineCount());
        Assert.True(ObjMonMagicNotMoveAttackCore.CorrectsJ220ClassTotal());
        Assert.True(ObjMonMagicNotMoveAttackCore.WasBlankLineNotEnd());
        Assert.True(ObjMonMagicNotMoveAttackCore.OffByOneInJ220());
        Assert.True(ObjMonMagicNotMoveAttackCore.ClassTotalOffByOne());
        Assert.True(ObjMonMagicNotMoveAttackCore.NoSuchOffByOneAgain());
    }

    [Fact]
    public void J220CorrectionArithmetic()
    {
        // **277 - 276 = 1**
        Assert.Equal(1, ObjMonMagicNotMoveAttackCore.J220RecordedLines
            - ObjMonMagicNotMoveAttackCore.Lines);
        Assert.Equal(1, ObjMonMagicNotMoveAttackCore.J220RecordedClassTotal
            - ObjMonMagicNotMoveAttackCore.ClassTotalLines);

        // **原记的差 1 由上一处连带而来**
        Assert.Equal(ObjMonMagicNotMoveAttackCore.J220RecordedLines
            - ObjMonMagicNotMoveAttackCore.Lines,
            ObjMonMagicNotMoveAttackCore.J220RecordedClassTotal
            - ObjMonMagicNotMoveAttackCore.ClassTotalLines);
    }

    // ===================== 一、三个嵌套过程 =====================

    [Fact]
    public void NestedProcedureFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.ThreeNestedProcedures());
        Assert.True(ObjMonMagicNotMoveAttackCore.MostSoFar());
        Assert.True(ObjMonMagicNotMoveAttackCore.ThreeDistinctRoles());
        Assert.True(ObjMonMagicNotMoveAttackCore.SpanTableExtracted());
        Assert.True(ObjMonMagicNotMoveAttackCore.OnlyMA3HasTryFinally());
        Assert.True(ObjMonMagicNotMoveAttackCore.OnlyMA3BuildsList());
        Assert.True(ObjMonMagicNotMoveAttackCore.StructuralNotStylistic());
        Assert.True(ObjMonMagicNotMoveAttackCore.ConsistentWithJ217());

        Assert.Equal(3, ObjMonMagicNotMoveAttackCore.NestedTable.Length);
        Assert.Equal("MagicAttack(nType)",
            ObjMonMagicNotMoveAttackCore.NestedTable[0].Proc);
        Assert.False(ObjMonMagicNotMoveAttackCore.NestedTable[0].Protected);
        Assert.Equal("MagicAttack2()",
            ObjMonMagicNotMoveAttackCore.NestedTable[1].Proc);
        Assert.Equal("MagicAttack3()",
            ObjMonMagicNotMoveAttackCore.NestedTable[2].Proc);
        Assert.True(ObjMonMagicNotMoveAttackCore.NestedTable[2].Protected);
    }

    // ===================== 二、nEfftctType =====================

    [Fact]
    public void MisspellingFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.MisspelledIdentifier());
        Assert.True(ObjMonMagicNotMoveAttackCore.SixSites());
        Assert.True(ObjMonMagicNotMoveAttackCore.SecondSpellingError());
        Assert.True(ObjMonMagicNotMoveAttackCore.SameFamilyAsJ202Poision());
        Assert.True(ObjMonMagicNotMoveAttackCore.EffectTypeLinesChecked());

        Assert.Equal(6, ObjMonMagicNotMoveAttackCore.EffectTypeLines.Length);
        Assert.Equal(new[] { 6823, 6846, 6851, 6856, 6860, 6863 },
            ObjMonMagicNotMoveAttackCore.EffectTypeLines);
    }

    [Fact]
    public void BitmaskFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.BitmaskAccumulator());
        Assert.True(ObjMonMagicNotMoveAttackCore.ThreeBits());
        Assert.True(ObjMonMagicNotMoveAttackCore.RangeZeroToSeven());
        Assert.True(ObjMonMagicNotMoveAttackCore.BitsAreDisjoint());
        Assert.True(ObjMonMagicNotMoveAttackCore.BitsSumToSeven());
        Assert.True(ObjMonMagicNotMoveAttackCore.SentAsEffectId());
        Assert.True(ObjMonMagicNotMoveAttackCore.ClientMustDecodeBits());
        Assert.True(ObjMonMagicNotMoveAttackCore.ContrastWithSingleIdClasses());
        Assert.True(ObjMonMagicNotMoveAttackCore.BitsAscendWithCallOrder());
        Assert.True(ObjMonMagicNotMoveAttackCore.ConsistentDesign());
    }

    [Fact]
    public void BitmaskBoundaries()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.NoneGivesZero());
        Assert.True(ObjMonMagicNotMoveAttackCore.AllThreeGivesSeven());
        Assert.True(ObjMonMagicNotMoveAttackCore.OnlyNetGivesOne());
        Assert.True(ObjMonMagicNotMoveAttackCore.OnlyParalysisGivesTwo());
        Assert.True(ObjMonMagicNotMoveAttackCore.OnlyFrozenGivesFour());

        Assert.Equal(0, ObjMonMagicNotMoveAttackCore.EffectType(false, false, false));
        Assert.Equal(7, ObjMonMagicNotMoveAttackCore.EffectType(true, true, true));
        Assert.Equal(1, ObjMonMagicNotMoveAttackCore.EffectType(true, false, false));
        Assert.Equal(2, ObjMonMagicNotMoveAttackCore.EffectType(false, true, false));
        Assert.Equal(4, ObjMonMagicNotMoveAttackCore.EffectType(false, false, true));
        Assert.Equal(3, ObjMonMagicNotMoveAttackCore.EffectType(true, true, false));
    }

    [Fact]
    public void MissingBitFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.MA3ContributesNothing());
        Assert.True(ObjMonMagicNotMoveAttackCore.NoPlusEight());
        Assert.True(ObjMonMagicNotMoveAttackCore.NoSendInsideMA3());
        Assert.True(ObjMonMagicNotMoveAttackCore.StateChangedButNotVisualised());
        Assert.True(ObjMonMagicNotMoveAttackCore.OppositeOfJ211());
    }

    // ===================== 三、外层体的恒真判据 =====================

    [Fact]
    public void TautologyFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.TautologicalApproachCheck());
        Assert.True(ObjMonMagicNotMoveAttackCore.ThresholdSevenVsSix());
        Assert.True(ObjMonMagicNotMoveAttackCore.GateExitsSoFallthroughIsFar());
        Assert.True(ObjMonMagicNotMoveAttackCore.BothBranchesSame());
        Assert.True(ObjMonMagicNotMoveAttackCore.SevenVsSixIsTautological());
        Assert.True(ObjMonMagicNotMoveAttackCore.SixVsSixIsTautological());
        Assert.True(ObjMonMagicNotMoveAttackCore.TemplateCheckWasMeaningful());
        Assert.True(ObjMonMagicNotMoveAttackCore.BecauseTemplateGateWasProbabilistic());
        Assert.True(ObjMonMagicNotMoveAttackCore.PositionalGateMakesItDead());
        Assert.True(ObjMonMagicNotMoveAttackCore.SameInJ219());
        Assert.True(ObjMonMagicNotMoveAttackCore.SideEffectOfGateRewrite());
    }

    [Fact]
    public void TautologyTable()
    {
        // **门 >= 判据 => 恒真**
        Assert.True(ObjMonMagicNotMoveAttackCore.CheckIsTautological(7, 6));
        Assert.True(ObjMonMagicNotMoveAttackCore.CheckIsTautological(6, 6));
        Assert.False(ObjMonMagicNotMoveAttackCore.CheckIsTautological(5, 6));
    }

    [Fact]
    public void HitTickReorderFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.HitTickSetLater());
        Assert.True(ObjMonMagicNotMoveAttackCore.TemplateSetsItFirst());
        Assert.True(ObjMonMagicNotMoveAttackCore.ReentrancyWindow());
        Assert.True(ObjMonMagicNotMoveAttackCore.UnexplainedReorder());
        Assert.True(ObjMonMagicNotMoveAttackCore.HitDelayStillFirst());

        Assert.True(ObjMonMagicNotMoveAttackCore.HitTickSetLine
            > ObjMonMagicNotMoveAttackCore.CallSlaveLine);
        Assert.True(ObjMonMagicNotMoveAttackCore.HitTickSetLine
            > ObjMonMagicNotMoveAttackCore.MA3CallLine);
    }

    [Fact]
    public void HpBandFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.NarrowHpBandForCallSlave());
        Assert.True(ObjMonMagicNotMoveAttackCore.StrictlyInside());
        Assert.True(ObjMonMagicNotMoveAttackCore.InconsistentParentheses());
        Assert.True(ObjMonMagicNotMoveAttackCore.AtNinetyNotIn());
        Assert.True(ObjMonMagicNotMoveAttackCore.AtEightyNotIn());
        Assert.True(ObjMonMagicNotMoveAttackCore.AtEightyFiveIn());

        Assert.False(ObjMonMagicNotMoveAttackCore.InCallSlaveBand(90, 100));
        Assert.True(ObjMonMagicNotMoveAttackCore.InCallSlaveBand(85, 100));
        Assert.False(ObjMonMagicNotMoveAttackCore.InCallSlaveBand(80, 100));
        Assert.False(ObjMonMagicNotMoveAttackCore.InCallSlaveBand(95, 100));
    }

    [Fact]
    public void ForeverFrozenCooldownFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.FortyFiveSecondCooldown());
        Assert.True(ObjMonMagicNotMoveAttackCore.ExactlyFortyFiveBlocks());
        Assert.True(ObjMonMagicNotMoveAttackCore.OneOverFortyFive());

        Assert.False(ObjMonMagicNotMoveAttackCore.FrozenReady(0, 45000));
        Assert.True(ObjMonMagicNotMoveAttackCore.FrozenReady(0, 45001));
    }

    [Fact]
    public void OnceOnlyFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.PerFrameAttempt());
        Assert.True(ObjMonMagicNotMoveAttackCore.GuardMakesItOnceOnly());
        Assert.True(ObjMonMagicNotMoveAttackCore.NarrowBandGivesNoSecondChance());
        Assert.True(ObjMonMagicNotMoveAttackCore.FrozenTickReadWriteHere());
        Assert.True(ObjMonMagicNotMoveAttackCore.J220CaveatConfirmed());
        Assert.True(ObjMonMagicNotMoveAttackCore.NotADeadField());
    }

    // ===================== 四、MagicAttack 的两臂 =====================

    [Fact]
    public void CaseArmFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.CaseWithoutElse());
        Assert.True(ObjMonMagicNotMoveAttackCore.TwoArms());
        Assert.True(ObjMonMagicNotMoveAttackCore.CallSitesMatchArms());
        Assert.True(ObjMonMagicNotMoveAttackCore.CompleteByConvention());
        Assert.True(ObjMonMagicNotMoveAttackCore.DescendingDeleteCorrect());
    }

    [Fact]
    public void BraceCommentFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.BraceCommentDisablesGate());
        Assert.True(ObjMonMagicNotMoveAttackCore.ParalysisNotGatedByAbility());
        Assert.True(ObjMonMagicNotMoveAttackCore.SameFamilyAsShape36());
        Assert.True(ObjMonMagicNotMoveAttackCore.OneInTwelve());
    }

    [Fact]
    public void ArmMechanismFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.TwoMechanisms());
        Assert.True(ObjMonMagicNotMoveAttackCore.PoisonVsFrozen());
        Assert.True(ObjMonMagicNotMoveAttackCore.BothFixedThree());
        Assert.True(ObjMonMagicNotMoveAttackCore.UniqueAmongClasses());
        Assert.True(ObjMonMagicNotMoveAttackCore.Arm1ChecksParalysis());
        Assert.True(ObjMonMagicNotMoveAttackCore.Arm2DoesNot());
        Assert.True(ObjMonMagicNotMoveAttackCore.CanFreezeWhileParalysed());
        Assert.True(ObjMonMagicNotMoveAttackCore.ReadsOnce());
        Assert.True(ObjMonMagicNotMoveAttackCore.OneInEight());
    }

    [Fact]
    public void ArmBoundaries()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.ParalysisAllTrue());
        Assert.True(ObjMonMagicNotMoveAttackCore.AlreadyParalysedBlocks());
        Assert.True(ObjMonMagicNotMoveAttackCore.MissedRollBlocks());
        Assert.True(ObjMonMagicNotMoveAttackCore.FrozenIgnoresParalysis());

        Assert.True(ObjMonMagicNotMoveAttackCore.ParalysisFires(false, 0));
        Assert.False(ObjMonMagicNotMoveAttackCore.ParalysisFires(true, 0));
        Assert.False(ObjMonMagicNotMoveAttackCore.ParalysisFires(false, 1));
        Assert.True(ObjMonMagicNotMoveAttackCore.FrozenFires(0));
        Assert.False(ObjMonMagicNotMoveAttackCore.FrozenFires(1));
    }

    [Fact]
    public void SendVariantFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.CommentedOutSend());
        Assert.True(ObjMonMagicNotMoveAttackCore.SevenArgsVsSix());
        Assert.True(ObjMonMagicNotMoveAttackCore.DelayParamDropped());
        Assert.True(ObjMonMagicNotMoveAttackCore.SeparatorsChecked());
        Assert.True(ObjMonMagicNotMoveAttackCore.TripleSlashSeparator());
    }

    [Fact]
    public void NilGuardFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.MA1NilGuard());
        Assert.True(ObjMonMagicNotMoveAttackCore.MA2NilGuard());
        Assert.True(ObjMonMagicNotMoveAttackCore.MA3NoGuardNeeded());
        Assert.True(ObjMonMagicNotMoveAttackCore.StricterThanJ217());
    }

    // ===================== 五、MagicAttack2 =====================

    [Fact]
    public void NetLoopFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.DoubleLoopNet());
        Assert.True(ObjMonMagicNotMoveAttackCore.SixtyFourProbes());
        Assert.True(ObjMonMagicNotMoveAttackCore.DirBoundedZeroToSeven());
        Assert.True(ObjMonMagicNotMoveAttackCore.ContrastWithJ216RandomNine());

        // **8 方向 × 8 格 = 64**
        Assert.Equal(64, (ObjMonMagicNotMoveAttackCore.DirMax + 1)
            * ObjMonMagicNotMoveAttackCore.DistanceMax);

        // **与 J216 的 Random(9) 对照：8 才是合法上界**
        Assert.Equal(8, ObjMonMagicNotMoveAttackCore.DirMax + 1);
    }

    [Fact]
    public void PowerSwapFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.PowerDamageSwap());
        Assert.True(ObjMonMagicNotMoveAttackCore.PowerHoldsDamageInsideLoop());
        Assert.True(ObjMonMagicNotMoveAttackCore.DamageHoldsBase());
        Assert.True(ObjMonMagicNotMoveAttackCore.NamingInverted());
    }

    [Fact]
    public void PipelineDivergenceFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.PipelineDivergence());
        Assert.True(ObjMonMagicNotMoveAttackCore.MA2LacksNextDamage());
        Assert.True(ObjMonMagicNotMoveAttackCore.MA2LacksPowerMax());
        Assert.True(ObjMonMagicNotMoveAttackCore.NoDamageCapOnNetLightning());
        Assert.True(ObjMonMagicNotMoveAttackCore.PowerRateAddMovedIntoLoop());
        Assert.True(ObjMonMagicNotMoveAttackCore.NoRebound());
        Assert.True(ObjMonMagicNotMoveAttackCore.NoControlEffect());
        Assert.True(ObjMonMagicNotMoveAttackCore.NoEffectMessage());
        Assert.True(ObjMonMagicNotMoveAttackCore.OnlyMA1IsComplete());
    }

    [Fact]
    public void OfflineFilterFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.CompoundNegationFilter());
        Assert.True(ObjMonMagicNotMoveAttackCore.NotAAndBAndC());
        Assert.True(ObjMonMagicNotMoveAttackCore.ConsistentWithJ203());
        Assert.True(ObjMonMagicNotMoveAttackCore.ConfigOffAttacks());
        Assert.True(ObjMonMagicNotMoveAttackCore.OfflinePlayerExcluded());
        Assert.True(ObjMonMagicNotMoveAttackCore.OnlinePlayerAttacked());

        Assert.True(ObjMonMagicNotMoveAttackCore.ShouldAttack(false, true, true));
        Assert.False(ObjMonMagicNotMoveAttackCore.ShouldAttack(true, true, true));
        Assert.True(ObjMonMagicNotMoveAttackCore.ShouldAttack(true, true, false));
    }

    // ===================== 六、MagicAttack3 =====================

    [Fact]
    public void RadiusFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.HardcodedEightSelfCentered());
        Assert.True(ObjMonMagicNotMoveAttackCore.TwoRadiusEightForms());
        Assert.True(ObjMonMagicNotMoveAttackCore.FourRadiusValues());
        Assert.True(ObjMonMagicNotMoveAttackCore.GroupCombosExtracted());
        Assert.True(ObjMonMagicNotMoveAttackCore.BothNewOnesSelfCentered());

        Assert.Equal(7, ObjMonMagicNotMoveAttackCore.GroupCombos.Length);
        Assert.Equal("J221-MA2", ObjMonMagicNotMoveAttackCore.GroupCombos[5].Batch);
        Assert.Equal("J221-MA3", ObjMonMagicNotMoveAttackCore.GroupCombos[6].Batch);
    }

    [Fact]
    public void RandomPickFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.RandomPickEachIteration());
        Assert.True(ObjMonMagicNotMoveAttackCore.NeverDeletes());
        Assert.True(ObjMonMagicNotMoveAttackCore.SameTargetMayRepeat());
        Assert.True(ObjMonMagicNotMoveAttackCore.CapIsOnApplicationsNotTargets());
    }

    [Fact]
    public void OffByOneFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.OffByOneInBreak());
        Assert.True(ObjMonMagicNotMoveAttackCore.UsesGreaterNotGreaterOrEqual());
        Assert.True(ObjMonMagicNotMoveAttackCore.ActuallyFive());
        Assert.True(ObjMonMagicNotMoveAttackCore.ShouldBeGreaterOrEqual());
        Assert.True(ObjMonMagicNotMoveAttackCore.FourAllowsFive());
        Assert.True(ObjMonMagicNotMoveAttackCore.CorrectWouldBeFour());
    }

    [Fact]
    public void ApplicationCountArithmetic()
    {
        // **`Inc` 在前、`Break` 在后 => 实际次数 = nMax + 1**
        Assert.Equal(5, ObjMonMagicNotMoveAttackCore.MaxApplications(4));
        Assert.Equal(1, ObjMonMagicNotMoveAttackCore.MaxApplications(0));
        Assert.Equal(2, ObjMonMagicNotMoveAttackCore.MaxApplications(1));

        // **对每个 nMax 都是 n+1（系统性差一）**
        for (int n = 0; n <= 6; n++)
        {
            Assert.Equal(n + 1, ObjMonMagicNotMoveAttackCore.MaxApplications(n));
        }
    }

    [Fact]
    public void CommentSyntaxFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.ParenStarComment());
        Assert.True(ObjMonMagicNotMoveAttackCore.FourthCommentSyntax());
        Assert.True(ObjMonMagicNotMoveAttackCore.DisabledCodeInside());
    }

    [Fact]
    public void RandomizeFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.CallsRandomize());
        Assert.True(ObjMonMagicNotMoveAttackCore.ThirdOfFive());
        Assert.True(ObjMonMagicNotMoveAttackCore.CloseToJ219Site());
        Assert.True(ObjMonMagicNotMoveAttackCore.AuthorsHabit());
        Assert.True(ObjMonMagicNotMoveAttackCore.RandomizeTableExtracted());

        Assert.Equal(new[] { 1621, 6480, 6788, 7207, 9126 },
            ObjMonMagicNotMoveAttackCore.RandomizeLines);
    }

    [Fact]
    public void ResistSemanticsFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.ResistIsOrOfTwo());
        Assert.True(ObjMonMagicNotMoveAttackCore.HigherRateHarder());
        Assert.True(ObjMonMagicNotMoveAttackCore.NameIsAccurate());
        Assert.True(ObjMonMagicNotMoveAttackCore.SameDirectionAsOthers());
        Assert.True(ObjMonMagicNotMoveAttackCore.SameFamilyAsJ219FixedFaces());
        Assert.True(ObjMonMagicNotMoveAttackCore.DifferentFamilyFromVariableFaces());
        Assert.True(ObjMonMagicNotMoveAttackCore.UsesHundredFaces());
        Assert.True(ObjMonMagicNotMoveAttackCore.NoPropertyAlwaysFrozen());
        Assert.True(ObjMonMagicNotMoveAttackCore.HighRollOvercomesProperty());
        Assert.True(ObjMonMagicNotMoveAttackCore.LowRollBlocked());
        Assert.True(ObjMonMagicNotMoveAttackCore.ZeroRateAlwaysFrozen());
        Assert.True(ObjMonMagicNotMoveAttackCore.DurationFiveSeconds());
        Assert.True(ObjMonMagicNotMoveAttackCore.OpenForeverFrozenChecked());
    }

    [Fact]
    public void FrozenResistBoundaries()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.ForeverFrozenFires(false, 100, 0));
        Assert.True(ObjMonMagicNotMoveAttackCore.ForeverFrozenFires(true, 50, 50));
        Assert.False(ObjMonMagicNotMoveAttackCore.ForeverFrozenFires(true, 50, 49));
        Assert.True(ObjMonMagicNotMoveAttackCore.ForeverFrozenFires(true, 0, 0));

        // **rate 越大越**抗**：同样掷 70、rate=60 会被冻、rate=80 不会**
        Assert.True(ObjMonMagicNotMoveAttackCore.ForeverFrozenFires(true, 60, 70));
        Assert.False(ObjMonMagicNotMoveAttackCore.ForeverFrozenFires(true, 80, 70));
    }

    [Fact]
    public void MA3StructureFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.DoubleRaceFilter());
        Assert.True(ObjMonMagicNotMoveAttackCore.EquivalentSoHarmless());
        Assert.True(ObjMonMagicNotMoveAttackCore.SplitFilterAndUse());
        Assert.True(ObjMonMagicNotMoveAttackCore.NoTargetNeeded());
        Assert.True(ObjMonMagicNotMoveAttackCore.SelfCenteredOnly());
        Assert.True(ObjMonMagicNotMoveAttackCore.GuardIsRedundantForIt());
        Assert.True(ObjMonMagicNotMoveAttackCore.FreeInFinally());
    }

    // ===================== 七、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonMagicNotMoveAttackCore.TwentySevenClassesCovered());
        Assert.True(ObjMonMagicNotMoveAttackCore.RemainingApprox());
        Assert.True(ObjMonMagicNotMoveAttackCore.RealCoordinatesHere());
        Assert.True(ObjMonMagicNotMoveAttackCore.ZeroInJ220());
        Assert.True(ObjMonMagicNotMoveAttackCore.InconsistentWithinSameClass());
        Assert.True(ObjMonMagicNotMoveAttackCore.OnlyRangeBranchSetsTrue());
        Assert.True(ObjMonMagicNotMoveAttackCore.MA2AloneGivesFalse());
        Assert.True(ObjMonMagicNotMoveAttackCore.ReturnValueDiscardedByRun());
    }
}
