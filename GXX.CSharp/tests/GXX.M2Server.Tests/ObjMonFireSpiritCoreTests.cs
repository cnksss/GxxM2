using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J228：`ObjMon.pas` 中 `TFireSpiritMonster`（火灵）两个方法的 1:1 测试（115 行）。
/// **本批最有价值的发现**：
/// ① 外层体是共享模板第 8 次**逐字**确认（`0/30`）、且是保留两个可变点的"纯净版"
///    （J219 删了概率门、J222 把"去追"换成"放弃"、本类两处都没动）；
/// ② `wMagicID` 又回到**双角色**（`199`/`200`：特效编号 + `if wMagicID = 200` 加伤门），
///    取值是已见最大的一对；
/// ③ `CanFly` 把整个伤害段包住、而外层**不看它**照样置 `Result := True`
///    ⇒"被墙挡住"与"打中了"对外层完全一样（没掉血、没特效、却报成功）。
/// </summary>
public sealed class ObjMonFireSpiritCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(7377, ObjMonFireSpiritCore.Start);
        Assert.Equal(7487, ObjMonFireSpiritCore.End);
        Assert.Equal(111, ObjMonFireSpiritCore.Lines);
        Assert.Equal(7379, ObjMonFireSpiritCore.NestedStart);
        Assert.Equal(7456, ObjMonFireSpiritCore.NestedEnd);
        Assert.Equal(78, ObjMonFireSpiritCore.NestedLines);
        Assert.Equal(7458, ObjMonFireSpiritCore.OuterStart);
        Assert.Equal(7487, ObjMonFireSpiritCore.OuterEnd);
        Assert.Equal(30, ObjMonFireSpiritCore.OuterLines);
        Assert.Equal(7489, ObjMonFireSpiritCore.RunStart);
        Assert.Equal(7492, ObjMonFireSpiritCore.RunEnd);
        Assert.Equal(4, ObjMonFireSpiritCore.RunLines);
        Assert.Equal(115, ObjMonFireSpiritCore.TotalLines);
        Assert.Equal(1, ObjMonFireSpiritCore.NestedCount);

        Assert.Equal(5651, ObjMonFireSpiritCore.TemplateStart);
        Assert.Equal(5680, ObjMonFireSpiritCore.TemplateEnd);
        Assert.Equal(30, ObjMonFireSpiritCore.TemplateLines);
        Assert.Equal(0, ObjMonFireSpiritCore.TemplateDiffLines);
        Assert.Equal(8, ObjMonFireSpiritCore.TemplateConfirmations);
        Assert.Equal(7479, ObjMonFireSpiritCore.ApproachLine);
        Assert.Equal(7468, ObjMonFireSpiritCore.GateLine);
        Assert.Equal(27, ObjMonFireSpiritCore.J219OuterLines);
        Assert.Equal(30, ObjMonFireSpiritCore.J222OuterLines);
        Assert.Equal(6869, ObjMonFireSpiritCore.J222DiscardLine == 7313 ? 6869 : 0);
        Assert.Equal(6527, ObjMonFireSpiritCore.J219DiscardLine);

        Assert.Equal(7459, ObjMonFireSpiritCore.ResultFalseLine);
        Assert.Equal(7460, ObjMonFireSpiritCore.NilGuardLine);
        Assert.Equal(7462, ObjMonFireSpiritCore.CooldownLine);
        Assert.Equal(7464, ObjMonFireSpiritCore.HitTickLine);
        Assert.Equal(7465, ObjMonFireSpiritCore.HitDelayLine);
        Assert.Equal(7466, ObjMonFireSpiritCore.RangeGateLine);
        Assert.Equal(7470, ObjMonFireSpiritCore.AttackCallLine);
        Assert.Equal(7471, ObjMonFireSpiritCore.ResultTrueLine);
        Assert.Equal(7475, ObjMonFireSpiritCore.SameMapLine);
        Assert.Equal(7484, ObjMonFireSpiritCore.DiscardOtherMapLine);

        Assert.Equal(7384, ObjMonFireSpiritCore.MagicIdDeclLine);
        Assert.Equal(3, ObjMonFireSpiritCore.MagicIdSites);
        Assert.Equal(7388, ObjMonFireSpiritCore.MagicIdRollLine);
        Assert.Equal(3, ObjMonFireSpiritCore.MagicIdRollBound);
        Assert.Equal(200, ObjMonFireSpiritCore.MagicIdBonus);
        Assert.Equal(199, ObjMonFireSpiritCore.MagicIdNormal);
        Assert.Equal(7434, ObjMonFireSpiritCore.BonusGateLine);
        Assert.Equal(7435, ObjMonFireSpiritCore.BonusLine);
        Assert.Equal(1, ObjMonFireSpiritCore.BonusClampMin);
        Assert.Equal(4, ObjMonFireSpiritCore.ClampFamilySites);
        Assert.Equal(6, ObjMonFireSpiritCore.J210MagicId);
        Assert.Equal(1, ObjMonFireSpiritCore.J218MagicIdDefault);

        Assert.Equal(7392, ObjMonFireSpiritCore.DirectionLine);
        Assert.Equal(7393, ObjMonFireSpiritCore.CanFlyLine);
        Assert.Equal(7455, ObjMonFireSpiritCore.CanFlyEndLine);
        Assert.Equal(401, ObjMonFireSpiritCore.CanFlyDeclLine);
        Assert.Equal(5297, ObjMonFireSpiritCore.CanFlyImplLine);
        Assert.Equal(7396, ObjMonFireSpiritCore.MasterLine);
        Assert.Equal(7399, ObjMonFireSpiritCore.BaseDamageLine);
        Assert.Equal(7403, ObjMonFireSpiritCore.PowerRateAddLine);
        Assert.Equal(7405, ObjMonFireSpiritCore.NextDamageLine);
        Assert.Equal(7407, ObjMonFireSpiritCore.PowerMaxLine);
        Assert.Equal(7432, ObjMonFireSpiritCore.PositiveGuardLine);
        Assert.Equal(7453, ObjMonFireSpiritCore.PositiveGuardEndLine);
        Assert.Equal(7454, ObjMonFireSpiritCore.SendLine);
        Assert.Equal(20102, ObjMonFireSpiritCore.RM_LIGHTING);

        Assert.Equal(7442, ObjMonFireSpiritCore.ParalysisLine);
        Assert.Equal(7445, ObjMonFireSpiritCore.MakePosionLine);
        Assert.Equal(5, ObjMonFireSpiritCore.POISON_STONE);
        Assert.Equal(7045, ObjMonFireSpiritCore.J223InvertedLine);
        Assert.Equal(7436, ObjMonFireSpiritCore.RegenStart);
        Assert.Equal(7447, ObjMonFireSpiritCore.ReboundStart);
        Assert.Equal(7448, ObjMonFireSpiritCore.ReboundZeroLine);
        Assert.Equal(7440, ObjMonFireSpiritCore.StruckSendLine);
        Assert.Equal(7043, ObjMonFireSpiritCore.J223SendLine);

        Assert.Equal(280, ObjMonFireSpiritCore.ClassDeclLine);
        Assert.Equal(282, ObjMonFireSpiritCore.AttackDeclLine);
        Assert.Equal(283, ObjMonFireSpiritCore.RunDeclLine);
        Assert.Equal(280, ObjMonFireSpiritCore.CommentLine);
        Assert.Equal(286, ObjMonFireSpiritCore.NextClassLine);
        Assert.Equal(30, ObjMonFireSpiritCore.ClassesCovered);
        Assert.Equal(24, ObjMonFireSpiritCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonFireSpiritCore.SpanMatches());
        Assert.True(ObjMonFireSpiritCore.TotalLinesAddUp());
        Assert.True(ObjMonFireSpiritCore.DecompositionAddsUp());
        Assert.True(ObjMonFireSpiritCore.OuterSpanMatches());
        Assert.True(ObjMonFireSpiritCore.TemplateSpanMatches());
        Assert.True(ObjMonFireSpiritCore.RunSpanMatches());
        Assert.True(ObjMonFireSpiritCore.NestedBeforeOuter());
        Assert.True(ObjMonFireSpiritCore.MethodsAscending());
        Assert.True(ObjMonFireSpiritCore.WithinUnit());
        Assert.True(ObjMonFireSpiritCore.NoInstrumentation());
    }

    // ===================== 一、外层模板 =====================

    [Fact]
    public void TemplateFacts()
    {
        Assert.True(ObjMonFireSpiritCore.OuterTemplateVerbatim());
        Assert.True(ObjMonFireSpiritCore.ThirtyLinesZeroDiff());
        Assert.True(ObjMonFireSpiritCore.EighthConfirmation());
        Assert.True(ObjMonFireSpiritCore.KeepsTheApproachBranch());
        Assert.True(ObjMonFireSpiritCore.KeepsTheProbabilityGate());
        Assert.True(ObjMonFireSpiritCore.PureFormNotTrimmed());
        Assert.True(ObjMonFireSpiritCore.ThreeClassComparison());
        Assert.True(ObjMonFireSpiritCore.ThisClassChangedNeither());
        Assert.True(ObjMonFireSpiritCore.J219DeletedTheGate());
        Assert.True(ObjMonFireSpiritCore.J222ReplacedTheAction());
        Assert.True(ObjMonFireSpiritCore.TwoKeptThirtyLines());
        Assert.True(ObjMonFireSpiritCore.J219IsThreeShorter());
    }

    [Fact]
    public void TemplateInstanceTable()
    {
        Assert.Equal(3, ObjMonFireSpiritCore.TemplateInstances.Length);
        Assert.Equal("J228", ObjMonFireSpiritCore.TemplateInstances[0].Batch);
        Assert.Equal(30, ObjMonFireSpiritCore.TemplateInstances[0].OuterLines);
        Assert.Equal("present", ObjMonFireSpiritCore.TemplateInstances[0].Gate);
        Assert.Equal("SetTargetXY", ObjMonFireSpiritCore.TemplateInstances[0].Approach);

        Assert.Equal("J219", ObjMonFireSpiritCore.TemplateInstances[1].Batch);
        Assert.Equal(27, ObjMonFireSpiritCore.TemplateInstances[1].OuterLines);
        Assert.Equal("deleted", ObjMonFireSpiritCore.TemplateInstances[1].Gate);

        Assert.Equal("J222", ObjMonFireSpiritCore.TemplateInstances[2].Batch);
        Assert.Equal("DelTargetCreat", ObjMonFireSpiritCore.TemplateInstances[2].Approach);
    }

    [Fact]
    public void RunShellFacts()
    {
        Assert.True(ObjMonFireSpiritCore.RunIsPureShell());
        Assert.True(ObjMonFireSpiritCore.SeventeenthOccurrence());
    }

    // ===================== 二、wMagicID 的双角色 =====================

    [Fact]
    public void MagicIdFacts()
    {
        Assert.True(ObjMonFireSpiritCore.DualRoleAgain());
        Assert.True(ObjMonFireSpiritCore.ThreeSitesOnly());
        Assert.True(ObjMonFireSpiritCore.AssignedThenGated());
        Assert.True(ObjMonFireSpiritCore.LargestValuesSeen());
        Assert.True(ObjMonFireSpiritCore.SameNameFluctuatingRoleCount());
        Assert.True(ObjMonFireSpiritCore.MagicIdHistoryExtracted());
        Assert.True(ObjMonFireSpiritCore.MagicIdLinesChecked());
        Assert.True(ObjMonFireSpiritCore.OneInThree());
        Assert.True(ObjMonFireSpiritCore.DifferentUseOfSameBound());

        Assert.Equal(new[] { 7389, 7391, 7434 }, ObjMonFireSpiritCore.MagicIdLines);
    }

    [Fact]
    public void MagicIdHistoryTable()
    {
        Assert.Equal(3, ObjMonFireSpiritCore.MagicIdHistory.Length);
        Assert.Equal("J210", ObjMonFireSpiritCore.MagicIdHistory[0].Batch);
        Assert.Equal(2, ObjMonFireSpiritCore.MagicIdHistory[0].Roles);
        Assert.Equal("J218", ObjMonFireSpiritCore.MagicIdHistory[1].Batch);
        Assert.Equal(1, ObjMonFireSpiritCore.MagicIdHistory[1].Roles);
        Assert.Equal("J228", ObjMonFireSpiritCore.MagicIdHistory[2].Batch);
        Assert.Equal("199 / 200", ObjMonFireSpiritCore.MagicIdHistory[2].Values);
    }

    [Fact]
    public void MagicIdBoundaries()
    {
        Assert.True(ObjMonFireSpiritCore.RollZeroGivesBonus());
        Assert.True(ObjMonFireSpiritCore.OthersGiveNormal());

        Assert.Equal(200, ObjMonFireSpiritCore.MagicId(0));
        Assert.Equal(199, ObjMonFireSpiritCore.MagicId(1));
        Assert.Equal(199, ObjMonFireSpiritCore.MagicId(2));
    }

    // ---------- 加伤 ----------

    [Fact]
    public void BonusFacts()
    {
        Assert.True(ObjMonFireSpiritCore.FiftyPercentBonus());
        Assert.True(ObjMonFireSpiritCore.MaxOneClamp());
        Assert.True(ObjMonFireSpiritCore.ClampFamilyFourthSite());
        Assert.True(ObjMonFireSpiritCore.MinBonusIncrementIsOne());
        Assert.True(ObjMonFireSpiritCore.ZeroDamageGetsOne());
        Assert.True(ObjMonFireSpiritCore.OneDamageGetsOne());
        Assert.True(ObjMonFireSpiritCore.HundredGetsFifty());
        Assert.True(ObjMonFireSpiritCore.HalfRoundsToEvenZero());
        Assert.True(ObjMonFireSpiritCore.TinyDamageDoubles());
        Assert.True(ObjMonFireSpiritCore.ThreeGivesTwo());
        Assert.True(ObjMonFireSpiritCore.TwoGivesOne());
    }

    [Fact]
    public void BonusArithmetic()
    {
        // **增量 = Max(Round(n/2), 1)**
        Assert.Equal(1, ObjMonFireSpiritCore.BonusIncrement(1));
        Assert.Equal(1, ObjMonFireSpiritCore.BonusIncrement(2));
        Assert.Equal(2, ObjMonFireSpiritCore.BonusIncrement(3));
        Assert.Equal(50, ObjMonFireSpiritCore.BonusIncrement(100));

        // **总伤害 = n + 增量**
        Assert.Equal(2, ObjMonFireSpiritCore.Bonus(1));
        Assert.Equal(3, ObjMonFireSpiritCore.Bonus(2));
        Assert.Equal(150, ObjMonFireSpiritCore.Bonus(100));
    }

    [Fact]
    public void FinalDamageBoundaries()
    {
        Assert.True(ObjMonFireSpiritCore.BonusRaisesDamage());
        Assert.True(ObjMonFireSpiritCore.NormalUnchanged());
        Assert.True(ObjMonFireSpiritCore.BonusIsOneAndHalf());

        Assert.Equal(150, ObjMonFireSpiritCore.FinalDamage(100, true));
        Assert.Equal(100, ObjMonFireSpiritCore.FinalDamage(100, false));
        Assert.Equal(2, ObjMonFireSpiritCore.FinalDamage(1, true));
    }

    // ===================== 三、CanFly 的门 =====================

    [Fact]
    public void CanFlyFacts()
    {
        Assert.True(ObjMonFireSpiritCore.CanFlyWrapsWholeBody());
        Assert.True(ObjMonFireSpiritCore.DirectionComputedOutside());
        Assert.True(ObjMonFireSpiritCore.SuccessFlagSetRegardless());
        Assert.True(ObjMonFireSpiritCore.BlockedLooksLikeHit());
        Assert.True(ObjMonFireSpiritCore.NoDamageNoEffectButReportsTrue());
        Assert.True(ObjMonFireSpiritCore.SendInsideCanFly());
        Assert.True(ObjMonFireSpiritCore.SendInsideCanFlyOutsideDamageGuard());
        Assert.True(ObjMonFireSpiritCore.ZeroDamageStillSends());
        Assert.True(ObjMonFireSpiritCore.BlockedSendsNothing());
        Assert.True(ObjMonFireSpiritCore.FourArgSourceDest());
        Assert.True(ObjMonFireSpiritCore.NoMapArgument());
        Assert.True(ObjMonFireSpiritCore.SameMapCheckComesLater());
        Assert.True(ObjMonFireSpiritCore.CanFlyImplChecked());
    }

    [Fact]
    public void CanFlyGateBoundaries()
    {
        // **两条路都报成功 —— 外层无法区分**
        Assert.True(ObjMonFireSpiritCore.PassableReportsTrue());
        Assert.True(ObjMonFireSpiritCore.BlockedAlsoReportsTrue());
        Assert.True(ObjMonFireSpiritCore.IndistinguishableToCaller());

        // **方向在门外先算**
        Assert.True(ObjMonFireSpiritCore.DirectionLine
            < ObjMonFireSpiritCore.CanFlyLine);

        // **特效在门内、判零之外**
        Assert.True(ObjMonFireSpiritCore.SendLine
            < ObjMonFireSpiritCore.CanFlyEndLine);
        Assert.True(ObjMonFireSpiritCore.SendLine
            > ObjMonFireSpiritCore.PositiveGuardEndLine);
    }

    // ===================== 四、与 J223 的对照 =====================

    [Fact]
    public void NeighbourContrastFacts()
    {
        Assert.True(ObjMonFireSpiritCore.ParalysisCorrectHere());
        Assert.True(ObjMonFireSpiritCore.ReversedInAdjacentClass());
        Assert.True(ObjMonFireSpiritCore.NeighbourContrast());
        Assert.True(ObjMonFireSpiritCore.DoublesTheJ223Finding());
        Assert.True(ObjMonFireSpiritCore.HasMaxGuard());
        Assert.True(ObjMonFireSpiritCore.ParalysisSlotIsFive());
        Assert.True(ObjMonFireSpiritCore.CastThenMessageId());
        Assert.True(ObjMonFireSpiritCore.SameAsJ217ToJ221());
        Assert.True(ObjMonFireSpiritCore.DiffersFromJ223());
        Assert.True(ObjMonFireSpiritCore.TwoFormsCoexist());
    }

    [Fact]
    public void ParalysisBoundaries()
    {
        Assert.True(ObjMonFireSpiritCore.CorrectFormFires());
        Assert.True(ObjMonFireSpiritCore.ResistBlocks());

        Assert.True(ObjMonFireSpiritCore.ParalysisFires(false, true, 0, 0, 0));
        Assert.False(ObjMonFireSpiritCore.ParalysisFires(true, true, 0, 0, 0));
        Assert.False(ObjMonFireSpiritCore.ParalysisFires(false, true, 0, 0, 1));
        Assert.False(ObjMonFireSpiritCore.ParalysisFires(false, false, 0, 1, 0));
    }

    // ---------- 伤害管线 ----------

    [Fact]
    public void PipelineFacts()
    {
        Assert.True(ObjMonFireSpiritCore.FiveStepPipeline());
        Assert.True(ObjMonFireSpiritCore.CompleteForm());
        Assert.True(ObjMonFireSpiritCore.ThreeSubclassesThreeFormulas());
        Assert.True(ObjMonFireSpiritCore.PipelineOrdered());
        Assert.True(ObjMonFireSpiritCore.PositiveGuardForm());
        Assert.True(ObjMonFireSpiritCore.J223UsesEarlyExit());
        Assert.True(ObjMonFireSpiritCore.TwoEquivalentFormsCoexist());
    }

    // ---------- 回血与反弹 ----------

    [Fact]
    public void RegenAndReboundFacts()
    {
        Assert.True(ObjMonFireSpiritCore.ManaBasedRegen());
        Assert.True(ObjMonFireSpiritCore.ReboundAfterParalysis());
        Assert.True(ObjMonFireSpiritCore.SecondZeroCheckForRebound());
        Assert.True(ObjMonFireSpiritCore.ZeroMpNoRegen());
        Assert.True(ObjMonFireSpiritCore.RegenIsDivByMp());
        Assert.True(ObjMonFireSpiritCore.RegenInsideGuard());

        Assert.Equal(0, ObjMonFireSpiritCore.Regen(100, 0));
        Assert.Equal(10, ObjMonFireSpiritCore.Regen(100, 10));
        Assert.Equal(50, ObjMonFireSpiritCore.Regen(100, 2));
    }

    // ===================== 五、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonFireSpiritCore.ThirtyClassesCovered());
        Assert.True(ObjMonFireSpiritCore.RemainingApprox());
        Assert.True(ObjMonFireSpiritCore.SameBaseAsJ217J218());
        Assert.True(ObjMonFireSpiritCore.FourthSubclass());
        Assert.True(ObjMonFireSpiritCore.CommentBreaksThePattern());
        Assert.True(ObjMonFireSpiritCore.OnlyTwoOverrides());
        Assert.True(ObjMonFireSpiritCore.NoNewMethods());
        Assert.True(ObjMonFireSpiritCore.FamilyPattern());
        Assert.True(ObjMonFireSpiritCore.NextClassSixLinesLater());

        Assert.Equal(new[] { 207, 213, 219 }, ObjMonFireSpiritCore.SiblingDeclLines);
    }
}
