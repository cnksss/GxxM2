using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J238：`ObjMon.pas` 中 `TWealthAnimalMon`（富贵兽）七个方法的 1:1 测试（130 行）。
/// **本批最有价值的发现**：
/// ① 整个"灵符赏金"系统是**死的且可证明** —— `m_nGameGird` 唯一的活读取是 `> 0`、
///    而能把它变正数的两处写入（初值 9376、累加 9406）**都被注释掉了**；
/// ② `POISON_LOCKSPELL { 7 }` 的花括号注解与常量（**2**）**不符** —— 本系列第一次；
/// ③ 花括号的**第六种**用法："保留配置名、把值换成字面量"
///    （`Random(100 { g_Config.nMon79CrazyRate })`）；
/// ④ **本批到达 `ObjMon.pas` 末尾**（第 9501 行是 `end.`）。
/// </summary>
public sealed class ObjMonWealthAnimalMonCoreTests
{
    [Fact]
    public void Constants()
    {
        Assert.Equal(9502, ObjMonWealthAnimalMonCore.UnitLines);
        Assert.Equal(9501, ObjMonWealthAnimalMonCore.EndDotLine);
        Assert.Equal(9499, ObjMonWealthAnimalMonCore.LastMethodEnd);

        Assert.Equal(9361, ObjMonWealthAnimalMonCore.CreateStart);
        Assert.Equal(9377, ObjMonWealthAnimalMonCore.CreateEnd);
        Assert.Equal(17, ObjMonWealthAnimalMonCore.CreateLines);
        Assert.Equal(9379, ObjMonWealthAnimalMonCore.DestroyStart);
        Assert.Equal(9382, ObjMonWealthAnimalMonCore.DestroyEnd);
        Assert.Equal(4, ObjMonWealthAnimalMonCore.DestroyLines);
        Assert.Equal(9385, ObjMonWealthAnimalMonCore.StruckStart);
        Assert.Equal(9389, ObjMonWealthAnimalMonCore.StruckEnd);
        Assert.Equal(5, ObjMonWealthAnimalMonCore.StruckLines);
        Assert.Equal(9392, ObjMonWealthAnimalMonCore.AttackStart);
        Assert.Equal(9395, ObjMonWealthAnimalMonCore.AttackEnd);
        Assert.Equal(4, ObjMonWealthAnimalMonCore.AttackLines);
        Assert.Equal(9398, ObjMonWealthAnimalMonCore.Struck1Start);
        Assert.Equal(9419, ObjMonWealthAnimalMonCore.Struck1End);
        Assert.Equal(22, ObjMonWealthAnimalMonCore.Struck1Lines);
        Assert.Equal(9421, ObjMonWealthAnimalMonCore.DieStart);
        Assert.Equal(9472, ObjMonWealthAnimalMonCore.DieEnd);
        Assert.Equal(52, ObjMonWealthAnimalMonCore.DieLines);
        Assert.Equal(9474, ObjMonWealthAnimalMonCore.RunStart);
        Assert.Equal(9499, ObjMonWealthAnimalMonCore.RunEnd);
        Assert.Equal(26, ObjMonWealthAnimalMonCore.RunLines);
        Assert.Equal(130, ObjMonWealthAnimalMonCore.TotalLines);
        Assert.Equal(7, ObjMonWealthAnimalMonCore.MethodCount);

        Assert.Equal("m_nGameGird", ObjMonWealthAnimalMonCore.RewardFieldName);
        Assert.Equal(532, ObjMonWealthAnimalMonCore.RewardDeclLine);
        Assert.Equal(9376, ObjMonWealthAnimalMonCore.RewardInitLine);
        Assert.Equal(9406, ObjMonWealthAnimalMonCore.RewardIncLine);
        Assert.Equal(9408, ObjMonWealthAnimalMonCore.RewardBroadcastLine);
        Assert.Equal(9424, ObjMonWealthAnimalMonCore.RewardReadLine);
        Assert.Equal(13, ObjMonWealthAnimalMonCore.RewardOccurrences);
        Assert.Equal(0, ObjMonWealthAnimalMonCore.LivePositiveWrites);
        Assert.Equal(1, ObjMonWealthAnimalMonCore.LiveReads);

        Assert.Equal(9476, ObjMonWealthAnimalMonCore.WrongBraceLine);
        Assert.Equal(7, ObjMonWealthAnimalMonCore.BraceValue);
        Assert.Equal(2, ObjMonWealthAnimalMonCore.POISON_LOCKSPELL);
        Assert.Equal(11, ObjMonWealthAnimalMonCore.LockspellDeclLine);
        Assert.Equal(5, ObjMonWealthAnimalMonCore.POISON_STONE);
        Assert.Equal(13, ObjMonWealthAnimalMonCore.StoneDeclLine);
        Assert.Equal(20006, ObjMonWealthAnimalMonCore.RM_HIT);
        Assert.Equal(944, ObjMonWealthAnimalMonCore.HitDeclLine);
        Assert.Equal(306, ObjMonWealthAnimalMonCore.HitOldValue);

        Assert.Equal("g_Config.nMon79CrazyRate", ObjMonWealthAnimalMonCore.ArchivedRateName);
        Assert.Equal("g_Config.nMon79CrazyTime", ObjMonWealthAnimalMonCore.ArchivedTimeName);
        Assert.Equal(100, ObjMonWealthAnimalMonCore.LiteralHundred);
        Assert.Equal(9412, ObjMonWealthAnimalMonCore.CrazyRollLine);
        Assert.Equal(9414, ObjMonWealthAnimalMonCore.OpenCrazyLine);

        Assert.Equal(9423, ObjMonWealthAnimalMonCore.TryLine);
        Assert.Equal(9468, ObjMonWealthAnimalMonCore.ExceptLine);
        Assert.Equal(9469, ObjMonWealthAnimalMonCore.LogLine);
        Assert.Equal("TWealthAnimalMon.Die", ObjMonWealthAnimalMonCore.LogText);
        Assert.Equal(9471, ObjMonWealthAnimalMonCore.DieInheritedLine);

        Assert.Equal(15, ObjMonWealthAnimalMonCore.CommentedImmunitySlot);
        Assert.Equal(100, ObjMonWealthAnimalMonCore.ImmunityValue);
        Assert.Equal(9371, ObjMonWealthAnimalMonCore.CommentedImmunityLine);
        Assert.Equal(9375, ObjMonWealthAnimalMonCore.ViewRangeLine);
        Assert.Equal(9479, ObjMonWealthAnimalMonCore.WalkThrottleLine);
        Assert.Equal(9498, ObjMonWealthAnimalMonCore.RunInheritedLine);

        Assert.Equal(531, ObjMonWealthAnimalMonCore.ClassDeclLine);
        Assert.Equal(530, ObjMonWealthAnimalMonCore.SeparatorLine);
        Assert.Equal(532, ObjMonWealthAnimalMonCore.GameGirdFieldLine);
        Assert.Equal(42, ObjMonWealthAnimalMonCore.ClassesCovered);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonWealthAnimalMonCore.SpanMatches());
        Assert.True(ObjMonWealthAnimalMonCore.TotalLinesAddUp());
        Assert.True(ObjMonWealthAnimalMonCore.MethodsAscending());
        Assert.True(ObjMonWealthAnimalMonCore.MethodsContiguous());
        Assert.True(ObjMonWealthAnimalMonCore.WithinUnit());
        Assert.True(ObjMonWealthAnimalMonCore.NoInstrumentation());
    }

    // ===================== 〇、到达单元末尾 =====================

    [Fact]
    public void EndOfUnitFacts()
    {
        Assert.True(ObjMonWealthAnimalMonCore.ReachesEndOfUnit());
        Assert.True(ObjMonWealthAnimalMonCore.EndDotAt9501());
        Assert.True(ObjMonWealthAnimalMonCore.LastMethodInFile());
        Assert.True(ObjMonWealthAnimalMonCore.LinearSweepFinished());
        Assert.True(ObjMonWealthAnimalMonCore.NotEqualToAllClassesPorted());
        Assert.True(ObjMonWealthAnimalMonCore.OnlyBlankBetween());
    }

    // ===================== 一、赏金系统是死的 =====================

    [Fact]
    public void RewardSystemIsDeadFacts()
    {
        Assert.True(ObjMonWealthAnimalMonCore.RewardSystemIsDead());
        Assert.True(ObjMonWealthAnimalMonCore.NoLiveWriteEver());
        Assert.True(ObjMonWealthAnimalMonCore.OnlyLiveWritesAreZero());
        Assert.True(ObjMonWealthAnimalMonCore.OnlyLiveReadIsGreaterThanZero());
        Assert.True(ObjMonWealthAnimalMonCore.TwoIndependentKills());
        Assert.True(ObjMonWealthAnimalMonCore.MostCompleteDanglingId());
        Assert.True(ObjMonWealthAnimalMonCore.ExactlyOneLiveRead());
        Assert.True(ObjMonWealthAnimalMonCore.NoLivePositiveWriteInCensus());
    }

    [Fact]
    public void RewardCensusTable()
    {
        Assert.True(ObjMonWealthAnimalMonCore.RewardCensusExtracted());
        Assert.Equal(9, ObjMonWealthAnimalMonCore.RewardCensus.Length);

        // **两处被注掉的写入**
        Assert.False(ObjMonWealthAnimalMonCore.RewardCensus[1].Live);
        Assert.Equal("init", ObjMonWealthAnimalMonCore.RewardCensus[1].Kind);
        Assert.False(ObjMonWealthAnimalMonCore.RewardCensus[2].Live);
        Assert.Equal("increment", ObjMonWealthAnimalMonCore.RewardCensus[2].Kind);

        // **唯一的活读取**
        Assert.True(ObjMonWealthAnimalMonCore.RewardCensus[4].Live);
        Assert.Equal("read>0", ObjMonWealthAnimalMonCore.RewardCensus[4].Kind);

        Assert.Equal(new[] { 9443, 9464 }, ObjMonWealthAnimalMonCore.RewardZeroLines);
        Assert.Equal(new[] { 9430, 9450 }, ObjMonWealthAnimalMonCore.RewardPayLines);
    }

    [Fact]
    public void PayoutBoundaries()
    {
        Assert.True(ObjMonWealthAnimalMonCore.ZeroNeverPays());
        Assert.True(ObjMonWealthAnimalMonCore.AlwaysZeroSoNeverPays());
        Assert.False(ObjMonWealthAnimalMonCore.PayoutRuns(0));
        Assert.True(ObjMonWealthAnimalMonCore.PayoutRuns(1));
        Assert.True(ObjMonWealthAnimalMonCore.PayoutRuns(100));
    }

    [Fact]
    public void ParallelPayoutFacts()
    {
        Assert.True(ObjMonWealthAnimalMonCore.TwoParallelPayouts());
        Assert.True(ObjMonWealthAnimalMonCore.LastHiterFirst());
        Assert.True(ObjMonWealthAnimalMonCore.ExpHitterElse());
        Assert.True(ObjMonWealthAnimalMonCore.FormatVersusFormatToStr());
        Assert.True(ObjMonWealthAnimalMonCore.SlightlyDifferentParallels());
        Assert.True(ObjMonWealthAnimalMonCore.BothCheckRaceServer());
        Assert.True(ObjMonWealthAnimalMonCore.NoDedup());
    }

    // ===================== 二、花括号注解与常量不符 =====================

    [Fact]
    public void WrongBraceValueFacts()
    {
        Assert.True(ObjMonWealthAnimalMonCore.BraceValueContradictsConstant());
        Assert.True(ObjMonWealthAnimalMonCore.SevenVersusTwo());
        Assert.True(ObjMonWealthAnimalMonCore.FirstWrongBraceValue());
        Assert.True(ObjMonWealthAnimalMonCore.CorrectOnesElsewhere());
        Assert.True(ObjMonWealthAnimalMonCore.AnnotationIsNotAKey());
        Assert.True(ObjMonWealthAnimalMonCore.StoneBraceWasCorrect());

        Assert.NotEqual(ObjMonWealthAnimalMonCore.BraceValue,
            ObjMonWealthAnimalMonCore.POISON_LOCKSPELL);
    }

    [Fact]
    public void HitOldValueFacts()
    {
        Assert.True(ObjMonWealthAnimalMonCore.HitOldValueResidue());
        Assert.True(ObjMonWealthAnimalMonCore.ThreeOhSixVersus20006());
        Assert.True(ObjMonWealthAnimalMonCore.ThirdOccurrenceOfOldValue());
        Assert.NotEqual(ObjMonWealthAnimalMonCore.HitOldValue,
            ObjMonWealthAnimalMonCore.RM_HIT);
    }

    // ===================== 三、无条件覆写 =====================

    [Fact]
    public void UnconditionalOverrideFacts()
    {
        Assert.True(ObjMonWealthAnimalMonCore.UnconditionalZero());
        Assert.True(ObjMonWealthAnimalMonCore.UnconditionalFalse());
        Assert.True(ObjMonWealthAnimalMonCore.IgnoresAllParameters());
        Assert.True(ObjMonWealthAnimalMonCore.DoesNotCallInherited());
        Assert.True(ObjMonWealthAnimalMonCore.StrongerThanShapeFour());
        Assert.True(ObjMonWealthAnimalMonCore.AnyDamageBecomesZero());
        Assert.True(ObjMonWealthAnimalMonCore.NeverAttacks());
        Assert.True(ObjMonWealthAnimalMonCore.TwoEntryPoints());
        Assert.True(ObjMonWealthAnimalMonCore.SignatureOneIsImmune());
        Assert.True(ObjMonWealthAnimalMonCore.CustomOneTakesDamage());

        Assert.Equal(0, ObjMonWealthAnimalMonCore.StruckDamageResult(99999));
        Assert.False(ObjMonWealthAnimalMonCore.AttackTargetResultValue());
    }

    // ===================== 四、花括号"存档" =====================

    [Fact]
    public void InlineBraceArchiveFacts()
    {
        Assert.True(ObjMonWealthAnimalMonCore.InlineBraceArchivesConfig());
        Assert.True(ObjMonWealthAnimalMonCore.LiteralHundredUsed());
        Assert.True(ObjMonWealthAnimalMonCore.ConfigNamePreservedAsComment());
        Assert.True(ObjMonWealthAnimalMonCore.SixthBraceUsage());
        Assert.True(ObjMonWealthAnimalMonCore.NotADeletionButAnArchive());
        Assert.True(ObjMonWealthAnimalMonCore.BothConfigsArchived());
    }

    [Fact]
    public void CrazyRollBoundaries()
    {
        Assert.True(ObjMonWealthAnimalMonCore.ZeroEntersCrazy());
        Assert.True(ObjMonWealthAnimalMonCore.OthersDoNot());
        Assert.True(ObjMonWealthAnimalMonCore.CrazyRollFires(0));
        Assert.False(ObjMonWealthAnimalMonCore.CrazyRollFires(1));
        Assert.False(ObjMonWealthAnimalMonCore.CrazyRollFires(99));
    }

    [Fact]
    public void CrazyBranchFacts()
    {
        Assert.True(ObjMonWealthAnimalMonCore.CrazyBranchIsCommentOnly());
        Assert.True(ObjMonWealthAnimalMonCore.ElseBranchDoesTheRoll());
        Assert.True(ObjMonWealthAnimalMonCore.CommentedLineIsTheAccumulator());
        Assert.True(ObjMonWealthAnimalMonCore.EchoesTheDeadReward());
        Assert.True(ObjMonWealthAnimalMonCore.PositiveAndAlive());
        Assert.True(ObjMonWealthAnimalMonCore.DeadSkips());
        Assert.True(ObjMonWealthAnimalMonCore.ZeroSkips());
    }

    [Fact]
    public void DamageEntryBoundaries()
    {
        Assert.True(ObjMonWealthAnimalMonCore.HandlesDamage(10, false));
        Assert.False(ObjMonWealthAnimalMonCore.HandlesDamage(10, true));
        Assert.False(ObjMonWealthAnimalMonCore.HandlesDamage(0, false));
    }

    // ---------- Die 的 try..except ----------

    [Fact]
    public void DieTryExceptFacts()
    {
        Assert.True(ObjMonWealthAnimalMonCore.TryExceptAroundDie());
        Assert.True(ObjMonWealthAnimalMonCore.SwallowsAgain());
        Assert.True(ObjMonWealthAnimalMonCore.ThirdExceptSecondPorted());
        Assert.True(ObjMonWealthAnimalMonCore.InheritedOutsideTry());
        Assert.True(ObjMonWealthAnimalMonCore.OppositeOfJ231());
        Assert.True(ObjMonWealthAnimalMonCore.ExceptLinesChecked());
        Assert.True(ObjMonWealthAnimalMonCore.J231InheritedWasInside());

        Assert.Equal(new[] { 2887, 8058, 9468 }, ObjMonWealthAnimalMonCore.ExceptLines);
    }

    // ===================== 五、Create / Run =====================

    [Fact]
    public void ImmunityFacts()
    {
        Assert.True(ObjMonWealthAnimalMonCore.EightSlotsOneCommented());
        Assert.True(ObjMonWealthAnimalMonCore.Slot15IsAntiRevive());
        Assert.True(ObjMonWealthAnimalMonCore.SevenLiveImmunities());
        Assert.True(ObjMonWealthAnimalMonCore.SlotsDistinct());
        Assert.True(ObjMonWealthAnimalMonCore.CommentedSlotNotLive());
        Assert.True(ObjMonWealthAnimalMonCore.AllImmunitiesAreHundred());

        Assert.Equal(new[] { 13, 16, 18, 17, 14, 19, 20 },
            ObjMonWealthAnimalMonCore.LiveImmunitySlots);
    }

    [Fact]
    public void CreateAndCommentFacts()
    {
        Assert.True(ObjMonWealthAnimalMonCore.ViewRangeZero());
        Assert.True(ObjMonWealthAnimalMonCore.StickCommentTwoVersions());
        Assert.True(ObjMonWealthAnimalMonCore.ShortVersionHasFullWidthComma());
        Assert.True(ObjMonWealthAnimalMonCore.LongVersionHasParenthetical());
        Assert.True(ObjMonWealthAnimalMonCore.TwoCommentOutsInCreate());
        Assert.True(ObjMonWealthAnimalMonCore.DeclLinesChecked());

        Assert.Equal(new[] { 9371, 9376 }, ObjMonWealthAnimalMonCore.CreateCommentLines);
    }

    [Fact]
    public void GuardFacts()
    {
        Assert.True(ObjMonWealthAnimalMonCore.FourItemGuard());
        Assert.True(ObjMonWealthAnimalMonCore.DeathBeforeGhost());
        Assert.True(ObjMonWealthAnimalMonCore.OppositeOrderOfJ231());
        Assert.True(ObjMonWealthAnimalMonCore.AllOkRuns());
        Assert.True(ObjMonWealthAnimalMonCore.AnyBlocks());
        Assert.True(ObjMonWealthAnimalMonCore.OneAnnotatedOneNot());
    }

    [Fact]
    public void GuardBoundaries()
    {
        Assert.True(ObjMonWealthAnimalMonCore.CanRun(false, false, false, false));
        Assert.False(ObjMonWealthAnimalMonCore.CanRun(true, false, false, false));
        Assert.False(ObjMonWealthAnimalMonCore.CanRun(false, true, false, false));
        Assert.False(ObjMonWealthAnimalMonCore.CanRun(false, false, true, false));
        Assert.False(ObjMonWealthAnimalMonCore.CanRun(false, false, false, true));
    }

    [Fact]
    public void AnimationFacts()
    {
        Assert.True(ObjMonWealthAnimalMonCore.ThreeLevelNestedAnimation());
        Assert.True(ObjMonWealthAnimalMonCore.InsideWalkThrottle());
        Assert.True(ObjMonWealthAnimalMonCore.NeverActuallyMoves());
        Assert.True(ObjMonWealthAnimalMonCore.RandomDirection0To7());
        Assert.True(ObjMonWealthAnimalMonCore.FirstRM_HIT());
        Assert.True(ObjMonWealthAnimalMonCore.RunGuardLineIsWalkThrottle());
        Assert.True(ObjMonWealthAnimalMonCore.TurnAnimation());
        Assert.True(ObjMonWealthAnimalMonCore.TurnMissIsNone());
        Assert.True(ObjMonWealthAnimalMonCore.JumpAnimation());
        Assert.True(ObjMonWealthAnimalMonCore.HitAnimation());
        Assert.True(ObjMonWealthAnimalMonCore.JumpBeatsHit());
        Assert.True(ObjMonWealthAnimalMonCore.AllMissIsNone());
    }

    [Fact]
    public void AnimationBoundaries()
    {
        // **掷中 20 且 4 中 => 转向**
        Assert.Equal("turn", ObjMonWealthAnimalMonCore.PickAnimation(0, 1, 0, 0, 0));
        Assert.Equal("none", ObjMonWealthAnimalMonCore.PickAnimation(0, 0, 0, 0, 0));

        // **掷中 6 且 6 中 => 跳；否则 3 中 => 攻击**
        Assert.Equal("jump", ObjMonWealthAnimalMonCore.PickAnimation(1, 0, 0, 1, 0));
        Assert.Equal("hit", ObjMonWealthAnimalMonCore.PickAnimation(1, 0, 0, 0, 1));

        // **跳优先于攻击**
        Assert.Equal("jump", ObjMonWealthAnimalMonCore.PickAnimation(1, 0, 0, 1, 1));

        Assert.Equal("none", ObjMonWealthAnimalMonCore.PickAnimation(5, 5, 5, 5, 5));
    }

    [Fact]
    public void RunTailFacts()
    {
        Assert.True(ObjMonWealthAnimalMonCore.InheritedUnconditional());
        Assert.True(ObjMonWealthAnimalMonCore.PureShellDestroy());
        Assert.True(ObjMonWealthAnimalMonCore.TwentyFirstOccurrence());
    }

    // ===================== 六、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonWealthAnimalMonCore.BaseIsTATMonster());
        Assert.True(ObjMonWealthAnimalMonCore.FourOverrides());
        Assert.True(ObjMonWealthAnimalMonCore.FirstDieOverride());
        Assert.True(ObjMonWealthAnimalMonCore.WidestOverrideSurface());
        Assert.True(ObjMonWealthAnimalMonCore.FortyTwoClassesCovered());
        Assert.True(ObjMonWealthAnimalMonCore.RemainingUnknownPendingAudit());
    }
}
