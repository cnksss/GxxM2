using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J231：`ObjMon.pas` 中**两个类**的 1:1 测试（112 行）：
/// ① `TFireCrossMonster.AttackTarget`（7 行，闭合该类）；
/// ② `TDevilBat`（恶魔蝙蝠）的 `Create`/`Destroy`/`AttackTarget`/`Run`。
/// **本批最有价值的发现**：
/// ① `TDevilBat.AttackTarget` 里两个 Abs 判据**都用了 X 轴**（Y 从未被检查）；
/// ② 类注释"攻击方式靠近人物**自爆攻击**"预告了那行 `m_WAbil.HP := 0; // 死亡`；
/// ③ `Run` 被 `try..except` 包着、`except` 只记一行日志就吞掉
///    （全文件 31 个 `try` 只有 3 个 `except`）；
/// ④ 任务点巡逻段在本文件里出现**五次**（J216 是第 4 次、本批第 5 次）。
/// </summary>
public sealed class ObjMonDevilBatCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(7946, ObjMonDevilBatCore.DispatcherStart);
        Assert.Equal(7952, ObjMonDevilBatCore.DispatcherEnd);
        Assert.Equal(7, ObjMonDevilBatCore.DispatcherLines);
        Assert.Equal(7955, ObjMonDevilBatCore.CreateStart);
        Assert.Equal(7962, ObjMonDevilBatCore.CreateEnd);
        Assert.Equal(8, ObjMonDevilBatCore.CreateLines);
        Assert.Equal(7964, ObjMonDevilBatCore.DestroyStart);
        Assert.Equal(7967, ObjMonDevilBatCore.DestroyEnd);
        Assert.Equal(4, ObjMonDevilBatCore.DestroyLines);
        Assert.Equal(7969, ObjMonDevilBatCore.AttackStart);
        Assert.Equal(8000, ObjMonDevilBatCore.AttackEnd);
        Assert.Equal(32, ObjMonDevilBatCore.AttackLines);
        Assert.Equal(8002, ObjMonDevilBatCore.RunStart);
        Assert.Equal(8062, ObjMonDevilBatCore.RunEnd);
        Assert.Equal(61, ObjMonDevilBatCore.RunLines);
        Assert.Equal(112, ObjMonDevilBatCore.TotalLines);
        Assert.Equal(5, ObjMonDevilBatCore.MethodCount);

        Assert.Equal(7948, ObjMonDevilBatCore.DispatchRollLine);
        Assert.Equal(4, ObjMonDevilBatCore.DispatchBound);
        Assert.Equal(7949, ObjMonDevilBatCore.TwoCallLine);
        Assert.Equal(7951, ObjMonDevilBatCore.OneCallLine);
        Assert.Equal(7758, ObjMonDevilBatCore.TwoDeclLine);
        Assert.Equal(7717, ObjMonDevilBatCore.OneDeclLine);
        Assert.Equal(187, ObjMonDevilBatCore.TwoAttackLines);
        Assert.Equal(40, ObjMonDevilBatCore.OneAttackLines);

        Assert.Equal(7957, ObjMonDevilBatCore.CreateInheritedLine);
        Assert.Equal(7958, ObjMonDevilBatCore.AnimalLine);
        Assert.Equal(7959, ObjMonDevilBatCore.StickLine);
        Assert.Equal(7960, ObjMonDevilBatCore.AntiPoisonLine);
        Assert.Equal(200, ObjMonDevilBatCore.AntiPoisonValue);
        Assert.Equal(7961, ObjMonDevilBatCore.ViewRangeLine);
        Assert.Equal(11, ObjMonDevilBatCore.ViewRangeValue);
        Assert.Equal(4, ObjMonDevilBatCore.CreateFieldCount);
        Assert.Equal(209, ObjMonDevilBatCore.StickDeclLine);
        Assert.Equal(132, ObjMonDevilBatCore.ViewRangeDeclLine);
        Assert.Equal(7966, ObjMonDevilBatCore.DestroyInheritedLine);

        Assert.Equal(7974, ObjMonDevilBatCore.DirCheckLine);
        Assert.Equal(7976, ObjMonDevilBatCore.CooldownLine);
        Assert.Equal(7978, ObjMonDevilBatCore.HitTickLine);
        Assert.Equal(7979, ObjMonDevilBatCore.HitDelayLine);
        Assert.Equal(7980, ObjMonDevilBatCore.FocusTickLine);
        Assert.Equal(7981, ObjMonDevilBatCore.BaseAttackLine);
        Assert.Equal(7982, ObjMonDevilBatCore.SuicideLine);
        Assert.Equal(7984, ObjMonDevilBatCore.ResultTrueLine);
        Assert.Equal(7988, ObjMonDevilBatCore.SameMapLine);
        Assert.Equal(7990, ObjMonDevilBatCore.BothAxesLine);
        Assert.Equal(7992, ObjMonDevilBatCore.SetTargetXYLine);
        Assert.Equal(7997, ObjMonDevilBatCore.DiscardLine);
        Assert.Equal(6, ObjMonDevilBatCore.SiblingFarThreshold);
        Assert.Equal(7971, ObjMonDevilBatCore.Bt06DeclLine);

        Assert.Equal(8004, ObjMonDevilBatCore.TryLine);
        Assert.Equal(8058, ObjMonDevilBatCore.ExceptLine);
        Assert.Equal(8059, ObjMonDevilBatCore.LogLine);
        Assert.Equal("{异常} TDevilBat.Run", ObjMonDevilBatCore.LogText);
        Assert.Equal(3081, ObjMonDevilBatCore.MainOutMessageDeclLine);
        Assert.Equal(31, ObjMonDevilBatCore.FileTryCount);
        Assert.Equal(3, ObjMonDevilBatCore.FileExceptCount);
        Assert.Equal(8005, ObjMonDevilBatCore.GuardLine);
        Assert.Equal(8005, ObjMonDevilBatCore.BraceValueLine);
        Assert.Equal(5, ObjMonDevilBatCore.POISON_STONE);
        Assert.Equal(8008, ObjMonDevilBatCore.SearchThrottleLine);
        Assert.Equal(1000, ObjMonDevilBatCore.SearchThresholdMs);
        Assert.Equal(8000, ObjMonDevilBatCore.UsualFirstTierMs);
        Assert.Equal(8011, ObjMonDevilBatCore.SearchTargetLine);
        Assert.Equal(8013, ObjMonDevilBatCore.WalkThrottleLine);
        Assert.Equal(8017, ObjMonDevilBatCore.NoAttackModeLine);
        Assert.Equal(8019, ObjMonDevilBatCore.TargetCheckLine);
        Assert.Equal(8021, ObjMonDevilBatCore.AttackCallLine);
        Assert.Equal(8023, ObjMonDevilBatCore.AttackSuccessInheritedLine);
        Assert.Equal(8024, ObjMonDevilBatCore.AttackSuccessExitLine);
        Assert.Equal(8029, ObjMonDevilBatCore.TargetXResetLine);
        Assert.Equal(8030, ObjMonDevilBatCore.MissionLine);
        Assert.Equal(8032, ObjMonDevilBatCore.NegIndexLine);
        Assert.Equal(8034, ObjMonDevilBatCore.ReachLine);
        Assert.Equal(8037, ObjMonDevilBatCore.IncIndexLine);
        Assert.Equal(8039, ObjMonDevilBatCore.ClampLine);
        Assert.Equal(8041, ObjMonDevilBatCore.SetTargetXLine);
        Assert.Equal(8042, ObjMonDevilBatCore.SetTargetYLine);
        Assert.Equal(8046, ObjMonDevilBatCore.TailCheckLine);
        Assert.Equal(8048, ObjMonDevilBatCore.GotoLine);
        Assert.Equal(8053, ObjMonDevilBatCore.WonderingLine);
        Assert.Equal(8057, ObjMonDevilBatCore.FinalInheritedLine);
        Assert.Equal(3, ObjMonDevilBatCore.ReachThreshold);
        Assert.Equal(5865, ObjMonDevilBatCore.J216MissionLine);
        Assert.Equal(8030, ObjMonDevilBatCore.ThisMissionLine);

        Assert.Equal(521, ObjMonDevilBatCore.ClassDeclLine);
        Assert.Equal(522, ObjMonDevilBatCore.AbilityCommentLine);
        Assert.Equal(520, ObjMonDevilBatCore.SectionCommentLine);
        Assert.Equal(524, ObjMonDevilBatCore.CreateDeclLine);
        Assert.Equal(526, ObjMonDevilBatCore.AttackDeclLine);
        Assert.Equal(527, ObjMonDevilBatCore.RunDeclLine);
        Assert.Equal(531, ObjMonDevilBatCore.NextClassLine);
        Assert.Equal(8064, ObjMonDevilBatCore.NextImplLine);
        Assert.Equal(34, ObjMonDevilBatCore.ClassesCovered);
        Assert.Equal(20, ObjMonDevilBatCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonDevilBatCore.SpanMatches());
        Assert.True(ObjMonDevilBatCore.TotalLinesAddUp());
        Assert.True(ObjMonDevilBatCore.MethodsAscending());
        Assert.True(ObjMonDevilBatCore.MethodsContiguous());
        Assert.True(ObjMonDevilBatCore.RunDecompositionAddsUp());
        Assert.True(ObjMonDevilBatCore.WithinUnit());
        Assert.True(ObjMonDevilBatCore.NoInstrumentation());
    }

    // ===================== 一、分派器 =====================

    [Fact]
    public void DispatcherFacts()
    {
        Assert.True(ObjMonDevilBatCore.SevenLineDispatcher());
        Assert.True(ObjMonDevilBatCore.RandomFourGates());
        Assert.True(ObjMonDevilBatCore.HeavyPathIsTheMinority());
        Assert.True(ObjMonDevilBatCore.OneQuarterVersusThreeQuarters());
        Assert.True(ObjMonDevilBatCore.RollZeroTwoAttack());
        Assert.True(ObjMonDevilBatCore.OthersOneAttack());
        Assert.True(ObjMonDevilBatCore.LightPathShare());
        Assert.True(ObjMonDevilBatCore.NoNilGuard());
        Assert.True(ObjMonDevilBatCore.DelegatesGuardToCallees());
        Assert.True(ObjMonDevilBatCore.BothCalleesHaveTheirOwn());
        Assert.True(ObjMonDevilBatCore.MixedDeclarationParens());
        Assert.True(ObjMonDevilBatCore.CallSitesMatchOwnDeclsOnlyPartly());
        Assert.True(ObjMonDevilBatCore.ParenStyleInconsistent());
    }

    [Fact]
    public void DispatcherBoundaries()
    {
        Assert.Equal("TwoAttack", ObjMonDevilBatCore.Dispatch(0));
        Assert.Equal("OneAttack", ObjMonDevilBatCore.Dispatch(1));
        Assert.Equal("OneAttack", ObjMonDevilBatCore.Dispatch(2));
        Assert.Equal("OneAttack", ObjMonDevilBatCore.Dispatch(3));

        // **轻路径占 3/4**
        int light = 0;

        for (int r = 0; r < ObjMonDevilBatCore.DispatchBound; r++)
        {
            if (ObjMonDevilBatCore.Dispatch(r) == "OneAttack")
                light++;
        }

        Assert.Equal(3, light);
    }

    // ===================== 二、TDevilBat 基类与 Create =====================

    [Fact]
    public void ClassAndCreateFacts()
    {
        Assert.True(ObjMonDevilBatCore.BaseIsTMonster());
        Assert.True(ObjMonDevilBatCore.ThirdDistinctBase());
        Assert.True(ObjMonDevilBatCore.NoCommonBaseAmongMonsters());
        Assert.True(ObjMonDevilBatCore.CommentDocumentsTheSuicide());
        Assert.True(ObjMonDevilBatCore.SelfDestructIsByDesign());
        Assert.True(ObjMonDevilBatCore.MixedPunctuationInComment());
        Assert.True(ObjMonDevilBatCore.AntiPoisonIsTwoHundred());
        Assert.True(ObjMonDevilBatCore.FourFieldsSetInCreate());
        Assert.True(ObjMonDevilBatCore.FieldLinesAscending());
        Assert.True(ObjMonDevilBatCore.InheritedFirstInCreate());
        Assert.True(ObjMonDevilBatCore.HasSectionComment());
        Assert.True(ObjMonDevilBatCore.DeclLinesChecked());
        Assert.True(ObjMonDevilBatCore.NextClassTenLinesLater());
    }

    [Fact]
    public void CommentCopyFacts()
    {
        Assert.True(ObjMonDevilBatCore.ElevenIsViewRange());
        Assert.True(ObjMonDevilBatCore.ConstantHasAReason());
        Assert.True(ObjMonDevilBatCore.UsedOnWrongAxis());
        Assert.True(ObjMonDevilBatCore.CommentCopiedFromDeclaration());
        Assert.True(ObjMonDevilBatCore.IncludesFullWidthComma());
        Assert.True(ObjMonDevilBatCore.SuffixOfTheOriginalComment());
    }

    [Fact]
    public void ShellDestructorFacts()
    {
        Assert.True(ObjMonDevilBatCore.PureShellDestructor());
        Assert.True(ObjMonDevilBatCore.InheritedOnly());
        Assert.True(ObjMonDevilBatCore.NoContainerToFree());
        Assert.True(ObjMonDevilBatCore.SiblingOfTheRunShell());
        Assert.True(ObjMonDevilBatCore.NotOneOfTheThreeMinority());
    }

    // ---------- 两轴同抄 ----------

    [Fact]
    public void CrossAxisBugFacts()
    {
        Assert.True(ObjMonDevilBatCore.BothAxesUseX());
        Assert.True(ObjMonDevilBatCore.SecondShouldBeY());
        Assert.True(ObjMonDevilBatCore.YRangeNeverChecked());
        Assert.True(ObjMonDevilBatCore.CrossAxisCopyPaste());
        Assert.True(ObjMonDevilBatCore.DifferWhenYLarge());
        Assert.True(ObjMonDevilBatCore.WronglyAcceptsFarY());
        Assert.True(ObjMonDevilBatCore.CorrectRejectsFarY());
        Assert.True(ObjMonDevilBatCore.BothRejectFarX());
    }

    [Fact]
    public void CrossAxisBoundaries()
    {
        // **Y 很远但 X 很近时：错误版接受、正确版拒绝**
        Assert.True(ObjMonDevilBatCore.RangeCheckAsWritten(0, 999));
        Assert.False(ObjMonDevilBatCore.RangeCheckCorrect(0, 999));

        // **X 很远时两者都拒绝**
        Assert.False(ObjMonDevilBatCore.RangeCheckAsWritten(999, 0));
        Assert.False(ObjMonDevilBatCore.RangeCheckCorrect(999, 0));

        // **两轴都近时两者都接受**
        Assert.True(ObjMonDevilBatCore.RangeCheckAsWritten(3, 3));
        Assert.True(ObjMonDevilBatCore.RangeCheckCorrect(3, 3));
    }

    [Fact]
    public void SuicideFacts()
    {
        Assert.True(ObjMonDevilBatCore.SuicideAfterAttack());
        Assert.True(ObjMonDevilBatCore.AttackZeroesHp());
        Assert.True(ObjMonDevilBatCore.NoAttackKeepsHp());
        Assert.True(ObjMonDevilBatCore.ResultOutsideCooldown());
        Assert.True(ObjMonDevilBatCore.SameCallOrder());

        Assert.Equal(0, ObjMonDevilBatCore.HpAfterAttack(100, true));
        Assert.Equal(100, ObjMonDevilBatCore.HpAfterAttack(100, false));
    }

    [Fact]
    public void TailFacts()
    {
        Assert.True(ObjMonDevilBatCore.NoDiscardBranch());
        Assert.True(ObjMonDevilBatCore.FarMeansDoNothing());
        Assert.True(ObjMonDevilBatCore.ContrastWithJ229J230());
        Assert.True(ObjMonDevilBatCore.Bt06FifthAppearance());
        Assert.True(ObjMonDevilBatCore.SameAsJ229AndJ230());
        Assert.True(ObjMonDevilBatCore.OnlyJ216Differs());
        Assert.True(ObjMonDevilBatCore.Bt06LinesChecked());

        Assert.Equal(new[] { 7971, 7974, 7981 }, ObjMonDevilBatCore.Bt06Lines);
    }

    // ===================== 三、Run =====================

    [Fact]
    public void TryExceptFacts()
    {
        Assert.True(ObjMonDevilBatCore.TryExceptAroundRun());
        Assert.True(ObjMonDevilBatCore.SwallowsTheException());
        Assert.True(ObjMonDevilBatCore.ThirdExceptInFile());
        Assert.True(ObjMonDevilBatCore.ThirtyOneTrysThreeExcepts());
        Assert.True(ObjMonDevilBatCore.LogsThenSwallows());
        Assert.True(ObjMonDevilBatCore.ExceptLinesChecked());
        Assert.True(ObjMonDevilBatCore.MainOutMessageHasDefaults());
        Assert.True(ObjMonDevilBatCore.InheritedInsideTry());
        Assert.True(ObjMonDevilBatCore.CatchesBaseExceptionsToo());
        Assert.True(ObjMonDevilBatCore.OverBroadCatch());

        Assert.Equal(new[] { 2887, 8058, 9468 }, ObjMonDevilBatCore.ExceptLines);
    }

    [Fact]
    public void GuardAndThrottleFacts()
    {
        Assert.True(ObjMonDevilBatCore.TwoItemGuard());
        Assert.True(ObjMonDevilBatCore.NarrowerThanFiveFold());
        Assert.True(ObjMonDevilBatCore.BraceValueCommentInSubscript());
        Assert.True(ObjMonDevilBatCore.Shape36FourthSite());
        Assert.True(ObjMonDevilBatCore.PoisonStoneIsFive());
        Assert.True(ObjMonDevilBatCore.AllFalseRuns());
        Assert.True(ObjMonDevilBatCore.AnyBlocks());
        Assert.True(ObjMonDevilBatCore.SingleThresholdThrottle());
        Assert.True(ObjMonDevilBatCore.RequiresNoTarget());
        Assert.True(ObjMonDevilBatCore.ThirdThrottleShape());
        Assert.True(ObjMonDevilBatCore.HalfTheUsualThrottle());
        Assert.True(ObjMonDevilBatCore.SameWalkThrottleAsJ214J216());
    }

    [Fact]
    public void GuardAndThrottleBoundaries()
    {
        Assert.True(ObjMonDevilBatCore.CanRun(false, false, false, false));
        Assert.False(ObjMonDevilBatCore.CanRun(true, false, false, false));
        Assert.False(ObjMonDevilBatCore.CanRun(false, true, false, false));
        Assert.False(ObjMonDevilBatCore.CanRun(false, false, true, false));
        Assert.False(ObjMonDevilBatCore.CanRun(false, false, false, true));

        // **有目标时永不重搜**
        Assert.True(ObjMonDevilBatCore.NoReseachWithTarget());
        Assert.False(ObjMonDevilBatCore.ShouldSearch(99999, true));

        // **无目标超 1 秒即搜**
        Assert.True(ObjMonDevilBatCore.SearchAfterOneWithoutTarget());
        Assert.True(ObjMonDevilBatCore.ExactlyOneBlocks());
        Assert.True(ObjMonDevilBatCore.ShouldSearch(1001, false));
        Assert.False(ObjMonDevilBatCore.ShouldSearch(1000, false));
    }

    [Fact]
    public void RunStructureFacts()
    {
        Assert.True(ObjMonDevilBatCore.AttackThenPatrol());
        Assert.True(ObjMonDevilBatCore.AttackSuccessExits());
        Assert.True(ObjMonDevilBatCore.SameTailShapeAsJ216());
        Assert.True(ObjMonDevilBatCore.GotoTargetXYWithParens());
        Assert.True(ObjMonDevilBatCore.UsesBaseWondering());
    }

    // ---------- 任务点段 ----------

    [Fact]
    public void MissionBlockFacts()
    {
        Assert.True(ObjMonDevilBatCore.MissionBlockAppearsFiveTimes());
        Assert.True(ObjMonDevilBatCore.FiveLineNumbers());
        Assert.True(ObjMonDevilBatCore.J216WasTheFourth());
        Assert.True(ObjMonDevilBatCore.ThisIsTheFifth());
        Assert.True(ObjMonDevilBatCore.NotJustAFoxTruckPair());
        Assert.True(ObjMonDevilBatCore.SetXAlsoFiveLines());
        Assert.True(ObjMonDevilBatCore.TablesCorrespond());
        Assert.True(ObjMonDevilBatCore.MissionBlockVerbatim());
        Assert.True(ObjMonDevilBatCore.SameGuardOrder());
        Assert.True(ObjMonDevilBatCore.ConfirmsJ216OrderNote());
        Assert.True(ObjMonDevilBatCore.NegIndexAfterUse());
        Assert.True(ObjMonDevilBatCore.ReachThresholdIsThree());
        Assert.True(ObjMonDevilBatCore.IndexOrderCorrect());

        Assert.Equal(new[] { 748, 1001, 1207, 5865, 8030 },
            ObjMonDevilBatCore.MissionBlockLines);
        Assert.Equal(new[] { 759, 1012, 1218, 5876, 8041 },
            ObjMonDevilBatCore.MissionSetXLines);
    }

    [Fact]
    public void ReachBoundaries()
    {
        Assert.True(ObjMonDevilBatCore.ThreeIsReached());
        Assert.True(ObjMonDevilBatCore.FourNotReached());

        Assert.True(ObjMonDevilBatCore.Reached(3, 3));
        Assert.True(ObjMonDevilBatCore.Reached(0, 0));
        Assert.False(ObjMonDevilBatCore.Reached(4, 0));
        Assert.False(ObjMonDevilBatCore.Reached(0, 4));
    }

    // ===================== 四、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonDevilBatCore.TwoClassesClosedInOneBatch());
        Assert.True(ObjMonDevilBatCore.FireCrossClosed());
        Assert.True(ObjMonDevilBatCore.DevilBatClosed());
        Assert.True(ObjMonDevilBatCore.NextClassIsTortoise());
        Assert.True(ObjMonDevilBatCore.ThirtyFourClassesCovered());
        Assert.True(ObjMonDevilBatCore.RemainingApprox());
    }
}
