using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J236：`ObjMon.pas` 中 `TMon35_2Monster` 三个方法的 1:1 测试（134 行）。
/// **本批最有价值的发现**：
/// ① 本类的 `MagicAttackGroup` 与 J235（`TMon38_12Monster`）那份**对齐后 `0 / 80` 完全相同**，
///    差别只是插入了**六行**"脱机人物"过滤 —— 于是 J230 那条"两种过滤写法可同循环并存"
///    有了最清晰的形态；
/// ② 类注释写着**另一个类的名字**（`// Mon38_12 …`，正是 J235 那个类）；
/// ③ 8887 行是一个**孤立的空语句 `;`** —— 本系列新记录的一种缺陷形态；
/// ④ 四个 `MagicAttackGroup` 声明里**三个有默认值、只有 100 号没有**，
///    且两组默认值不同，而**所有调用点都显式传满四个实参**。
/// </summary>
public sealed class ObjMonMon35_2CoreTests
{
    [Fact]
    public void Constants()
    {
        Assert.Equal(8868, ObjMonMon35_2Core.TargetStart);
        Assert.Equal(8904, ObjMonMon35_2Core.TargetEnd);
        Assert.Equal(37, ObjMonMon35_2Core.TargetLines);
        Assert.Equal(8906, ObjMonMon35_2Core.RunStart);
        Assert.Equal(8909, ObjMonMon35_2Core.RunEnd);
        Assert.Equal(4, ObjMonMon35_2Core.RunLines);
        Assert.Equal(8911, ObjMonMon35_2Core.GroupStart);
        Assert.Equal(9003, ObjMonMon35_2Core.GroupEnd);
        Assert.Equal(93, ObjMonMon35_2Core.GroupLines);
        Assert.Equal(134, ObjMonMon35_2Core.TotalLines);
        Assert.Equal(3, ObjMonMon35_2Core.MethodCount);

        Assert.Equal(8918, ObjMonMon35_2Core.BodyStart);
        Assert.Equal(9003, ObjMonMon35_2Core.BodyEnd);
        Assert.Equal(86, ObjMonMon35_2Core.BodyLines);
        Assert.Equal(8786, ObjMonMon35_2Core.J235BodyStart);
        Assert.Equal(8865, ObjMonMon35_2Core.J235BodyEnd);
        Assert.Equal(80, ObjMonMon35_2Core.J235BodyLines);
        Assert.Equal(0, ObjMonMon35_2Core.AlignedDiffLines);
        Assert.Equal(6, ObjMonMon35_2Core.InsertedLines);
        Assert.Equal(8933, ObjMonMon35_2Core.InsertedStart);
        Assert.Equal(8938, ObjMonMon35_2Core.InsertedEnd);
        Assert.Equal(6, ObjMonMon35_2Core.BodyLineDelta);

        Assert.Equal(100, ObjMonMon35_2Core.DeclWithoutDefaults);
        Assert.Equal(116, ObjMonMon35_2Core.ThisDeclLine);
        Assert.Equal(8911, ObjMonMon35_2Core.ThisImplLine);

        Assert.Equal(8870, ObjMonMon35_2Core.DirDeclLine);
        Assert.Equal("Dir", ObjMonMon35_2Core.DirVarName);
        Assert.Equal(8873, ObjMonMon35_2Core.NilGuardLine);
        Assert.Equal(8875, ObjMonMon35_2Core.CooldownLine);
        Assert.Equal(8877, ObjMonMon35_2Core.HitTickLine);
        Assert.Equal(8878, ObjMonMon35_2Core.HitDelayLine);
        Assert.Equal(8879, ObjMonMon35_2Core.RangeGateLine);
        Assert.Equal(6, ObjMonMon35_2Core.RangeThreshold);
        Assert.Equal(8881, ObjMonMon35_2Core.GateLine);
        Assert.Equal(2, ObjMonMon35_2Core.GateBound);
        Assert.Equal(8883, ObjMonMon35_2Core.FaceCommentLine);
        Assert.Equal(8884, ObjMonMon35_2Core.NextDirLine);
        Assert.Equal(8885, ObjMonMon35_2Core.TurnToLine);
        Assert.Equal(8886, ObjMonMon35_2Core.GroupCallLine);
        Assert.Equal(8887, ObjMonMon35_2Core.StraySemicolonLine);
        Assert.Equal(8888, ObjMonMon35_2Core.ResultTrueLine);
        Assert.Equal(8889, ObjMonMon35_2Core.GateExitLine);
        Assert.Equal(8892, ObjMonMon35_2Core.SameMapLine);
        Assert.Equal(8894, ObjMonMon35_2Core.TailCheckLine);
        Assert.Equal(8896, ObjMonMon35_2Core.SetTargetXYLine);
        Assert.Equal(8901, ObjMonMon35_2Core.DiscardLine);
        Assert.Equal(5651, ObjMonMon35_2Core.TemplateStart);
        Assert.Equal(30, ObjMonMon35_2Core.TemplateLines);
        Assert.Equal(10, ObjMonMon35_2Core.TemplateConfirmations);

        Assert.Equal(8919, ObjMonMon35_2Core.ListCreateLine);
        Assert.Equal(8920, ObjMonMon35_2Core.TryLine);
        Assert.Equal(8921, ObjMonMon35_2Core.SelfRageLine);
        Assert.Equal(8922, ObjMonMon35_2Core.SelfGetMapLine);
        Assert.Equal(8924, ObjMonMon35_2Core.TargetGetMapLine);
        Assert.Equal(8925, ObjMonMon35_2Core.WithMaxLine);
        Assert.Equal(8931, ObjMonMon35_2Core.RejectFilterLine);
        Assert.Equal(8933, ObjMonMon35_2Core.ConjunctFilterStart);
        Assert.Equal(8938, ObjMonMon35_2Core.ConjunctFilterEnd);
        Assert.Equal(8944, ObjMonMon35_2Core.RateAddLine);
        Assert.Equal(8946, ObjMonMon35_2Core.MultipleLine);
        Assert.Equal(8947, ObjMonMon35_2Core.NextDamageLine);
        Assert.Equal(8949, ObjMonMon35_2Core.PowerMaxLine);
        Assert.Equal(8974, ObjMonMon35_2Core.GuardLine);
        Assert.Equal(8977, ObjMonMon35_2Core.DelayBranch1Line);
        Assert.Equal(2000, ObjMonMon35_2Core.LongDelay);
        Assert.Equal(200, ObjMonMon35_2Core.ShortDelay);
        Assert.Equal(8983, ObjMonMon35_2Core.ParalysisLine);
        Assert.Equal(8984, ObjMonMon35_2Core.BraceDisabledLine);
        Assert.Equal(8986, ObjMonMon35_2Core.MakePosionLine);
        Assert.Equal(3, ObjMonMon35_2Core.ParalysisDuration);
        Assert.Equal(5, ObjMonMon35_2Core.POISON_STONE);
        Assert.Equal(8992, ObjMonMon35_2Core.DelayBranch2Line);
        Assert.Equal(8999, ObjMonMon35_2Core.GroupEffectLine);
        Assert.Equal(9000, ObjMonMon35_2Core.FinallyLine);
        Assert.Equal(9001, ObjMonMon35_2Core.FreeLine);
        Assert.Equal(8779, ObjMonMon35_2Core.J235GroupStart);
        Assert.Equal(8799, ObjMonMon35_2Core.J235RejectLine);
        Assert.Equal(8808, ObjMonMon35_2Core.J235MultipleLine);
        Assert.Equal(8861, ObjMonMon35_2Core.J235EffectLine);

        Assert.Equal(114, ObjMonMon35_2Core.ClassDeclLine);
        Assert.Equal("Mon38_12", ObjMonMon35_2Core.CommentedClassName);
        Assert.Equal("Mon35_2", ObjMonMon35_2Core.ActualClassName);
        Assert.Equal("Mon38_12", ObjMonMon35_2Core.J235ClassName);
        Assert.Equal(118, ObjMonMon35_2Core.TargetDeclLine);
        Assert.Equal(119, ObjMonMon35_2Core.RunDeclLine);
        Assert.Equal(9005, ObjMonMon35_2Core.NextSectionLine);
        Assert.Equal(9006, ObjMonMon35_2Core.NextCreateLine);
        Assert.Equal(40, ObjMonMon35_2Core.ClassesCovered);
        Assert.Equal(14, ObjMonMon35_2Core.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonMon35_2Core.SpanMatches());
        Assert.True(ObjMonMon35_2Core.TotalLinesAddUp());
        Assert.True(ObjMonMon35_2Core.MethodsAscending());
        Assert.True(ObjMonMon35_2Core.MethodsContiguous());
        Assert.True(ObjMonMon35_2Core.WithinUnit());
        Assert.True(ObjMonMon35_2Core.NoInstrumentation());
    }

    // ===================== 一、两份实现只差六行 =====================

    [Fact]
    public void AlignedDiffFacts()
    {
        Assert.True(ObjMonMon35_2Core.AlignedDiffIsZero());
        Assert.True(ObjMonMon35_2Core.SixInsertedLines());
        Assert.True(ObjMonMon35_2Core.OnlyTheOfflineFilterDiffers());
        Assert.True(ObjMonMon35_2Core.ClearestFormOfTheJ209Story());
        Assert.True(ObjMonMon35_2Core.VerbatimCopyExceptOneBlock());
        Assert.True(ObjMonMon35_2Core.BodyDeltaEqualsInserted());
        Assert.True(ObjMonMon35_2Core.AlignedBodiesEqualLength());
        Assert.True(ObjMonMon35_2Core.InsertedBlockMatchesMissing());
        Assert.True(ObjMonMon35_2Core.InsertedIsConjunctForm());
        Assert.True(ObjMonMon35_2Core.InsertedLinesChecked());
        Assert.True(ObjMonMon35_2Core.InsertedFollowsReject());
        Assert.True(ObjMonMon35_2Core.BothHaveRejectForm());
        Assert.True(ObjMonMon35_2Core.OnlyThisOneHasConjunct());
    }

    [Fact]
    public void InsertedBlockVerbatim()
    {
        Assert.Equal(6, ObjMonMon35_2Core.InsertedBlock.Length);
        Assert.Contains("脱机", ObjMonMon35_2Core.InsertedBlock[0]);
        Assert.Equal("Continue;", ObjMonMon35_2Core.InsertedBlock[4]);
        Assert.Equal("end;", ObjMonMon35_2Core.InsertedBlock[5]);
    }

    [Fact]
    public void BodyArithmetic()
    {
        // **对齐后两份体等长**
        Assert.Equal(ObjMonMon35_2Core.J235BodyLines,
            ObjMonMon35_2Core.BodyLines - ObjMonMon35_2Core.InsertedLines);
        Assert.Equal(ObjMonMon35_2Core.InsertedLines,
            ObjMonMon35_2Core.BodyLineDelta);
    }

    // ---------- 四个声明 ----------

    [Fact]
    public void DeclarationFacts()
    {
        Assert.True(ObjMonMon35_2Core.FourDeclarationsRead());
        Assert.True(ObjMonMon35_2Core.ThreeHaveDefaultsOneDoesNot());
        Assert.True(ObjMonMon35_2Core.TwoDifferentDefaultSets());
        Assert.True(ObjMonMon35_2Core.DefaultsNeverExercised());
        Assert.True(ObjMonMon35_2Core.AllCallSitesExplicit());
        Assert.True(ObjMonMon35_2Core.TwoAndTwoParamSplit());
        Assert.True(ObjMonMon35_2Core.FamilyDeclLinesChecked());
        Assert.True(ObjMonMon35_2Core.DefaultsDistributionTable());
    }

    [Fact]
    public void DeclarationTables()
    {
        Assert.Equal(new[] { 54, 92, 100, 116 }, ObjMonMon35_2Core.FamilyDeclLines);
        Assert.Equal(new[] { 54, 92, 116 }, ObjMonMon35_2Core.DeclsWithDefaults);
        Assert.Equal(new[] { "True", "5" }, ObjMonMon35_2Core.Defaults54);
        Assert.Equal(new[] { "1", "1" }, ObjMonMon35_2Core.Defaults92And116);
    }

    [Fact]
    public void DeclImplMismatchFacts()
    {
        Assert.True(ObjMonMon35_2Core.ImplDropsDefaults());
        Assert.True(ObjMonMon35_2Core.ImplMergesTypes());
        Assert.True(ObjMonMon35_2Core.DeclAndImplDiffer());
    }

    // ===================== 二、模板与空语句 =====================

    [Fact]
    public void TemplateFacts()
    {
        Assert.True(ObjMonMon35_2Core.TenthTemplateConfirmation());
        Assert.True(ObjMonMon35_2Core.StandardRangeSix());
        Assert.True(ObjMonMon35_2Core.GateIsStandardSix());
        Assert.True(ObjMonMon35_2Core.SixIsInRange());
        Assert.True(ObjMonMon35_2Core.SevenIsOut());
        Assert.True(ObjMonMon35_2Core.NilGuardFirst());
        Assert.True(ObjMonMon35_2Core.CooldownOrder());
    }

    [Fact]
    public void RangeBoundaries()
    {
        Assert.True(ObjMonMon35_2Core.InRange(6, 6));
        Assert.True(ObjMonMon35_2Core.InRange(0, 6));
        Assert.False(ObjMonMon35_2Core.InRange(7, 0));
        Assert.False(ObjMonMon35_2Core.InRange(0, 7));
    }

    [Fact]
    public void StraySemicolonFacts()
    {
        Assert.True(ObjMonMon35_2Core.StraySemicolon());
        Assert.True(ObjMonMon35_2Core.EmptyStatement());
        Assert.True(ObjMonMon35_2Core.NewShapeEmptyStatement());
        Assert.True(ObjMonMon35_2Core.DiffersFromEmptyBeginEnd());
        Assert.True(ObjMonMon35_2Core.SemicolonAfterCall());
        Assert.True(ObjMonMon35_2Core.SemicolonBeforeResult());
        Assert.True(ObjMonMon35_2Core.EmptyStatementIsNoOp());
    }

    [Fact]
    public void TurnToFacts()
    {
        Assert.True(ObjMonMon35_2Core.TurnsBeforeAttacking());
        Assert.True(ObjMonMon35_2Core.TurnToCall());
        Assert.True(ObjMonMon35_2Core.FieldWriteVersusMethodCall());
        Assert.True(ObjMonMon35_2Core.FaceCommentExplainsIt());
    }

    [Fact]
    public void DirectionVarNamingFacts()
    {
        Assert.True(ObjMonMon35_2Core.DirectionVarNamingCensus());
        Assert.True(ObjMonMon35_2Core.ThirdName());
        Assert.True(ObjMonMon35_2Core.ThreeDistinctNames());
        Assert.True(ObjMonMon35_2Core.ThisIsTheThird());

        Assert.Equal(3, ObjMonMon35_2Core.DirectionVarNames.Length);
        Assert.Equal("bt06", ObjMonMon35_2Core.DirectionVarNames[0].Name);
        Assert.Equal("nDir", ObjMonMon35_2Core.DirectionVarNames[1].Name);
        Assert.Equal("Dir", ObjMonMon35_2Core.DirectionVarNames[2].Name);
    }

    [Fact]
    public void TailLivenessFacts()
    {
        Assert.True(ObjMonMon35_2Core.ExitInsideProbabilityGate());
        Assert.True(ObjMonMon35_2Core.TailCheckStillMeaningful());
        Assert.True(ObjMonMon35_2Core.SameAsJ222J228J232J235());
        Assert.True(ObjMonMon35_2Core.SixNoApproach());
        Assert.True(ObjMonMon35_2Core.SevenApproaches());
    }

    [Fact]
    public void TailBoundaries()
    {
        Assert.False(ObjMonMon35_2Core.TailFires(6, 6));
        Assert.True(ObjMonMon35_2Core.TailFires(7, 0));
        Assert.True(ObjMonMon35_2Core.TailFires(0, 7));
        Assert.True(ObjMonMon35_2Core.TailFires(10, 10));
    }

    [Fact]
    public void RunShellFacts()
    {
        Assert.True(ObjMonMon35_2Core.RunIsPureShell());
        Assert.True(ObjMonMon35_2Core.TwentiethOccurrence());
    }

    // ===================== 三、抄错的类注释 =====================

    [Fact]
    public void StaleClassCommentFacts()
    {
        Assert.True(ObjMonMon35_2Core.ClassCommentNamesAnotherClass());
        Assert.True(ObjMonMon35_2Core.NamesTheJ235Class());
        Assert.True(ObjMonMon35_2Core.CopiedCommentNeverUpdated());
        Assert.True(ObjMonMon35_2Core.LooksPlausibleUntilCrossChecked());
        Assert.True(ObjMonMon35_2Core.ClassDeclChecked());
    }

    [Fact]
    public void SameProfileFacts()
    {
        Assert.True(ObjMonMon35_2Core.SameArgsAsJ235ApprBranch());
        Assert.True(ObjMonMon35_2Core.SameAttackProfile());
        Assert.True(ObjMonMon35_2Core.BaseIsTMagicAttackMonster());
        Assert.True(ObjMonMon35_2Core.SixthSubclass());
        Assert.True(ObjMonMon35_2Core.SameOverrideSet());
        Assert.True(ObjMonMon35_2Core.AttackTargetInherited());
    }

    [Fact]
    public void GroupCallArgsTable()
    {
        Assert.Equal(new[] { 3, 1, 3 }, ObjMonMon35_2Core.GroupCallArgs);
    }

    // ===================== 四、群攻内部 =====================

    [Fact]
    public void NTypeFacts()
    {
        Assert.True(ObjMonMon35_2Core.NTypeTripleDutyAgain());
        Assert.True(ObjMonMon35_2Core.BraceDisableAgain());
        Assert.True(ObjMonMon35_2Core.VerbatimFromJ235());
        Assert.True(ObjMonMon35_2Core.TypeTwoLongDelay());
        Assert.True(ObjMonMon35_2Core.OthersShort());
        Assert.True(ObjMonMon35_2Core.TypeOneUnresisted());
        Assert.True(ObjMonMon35_2Core.OthersNoParalysis());
        Assert.True(ObjMonMon35_2Core.ResistedBlocks());
        Assert.True(ObjMonMon35_2Core.EffectEqualsType());
        Assert.True(ObjMonMon35_2Core.FixedDurationThree());
        Assert.True(ObjMonMon35_2Core.SlotIsFive());
    }

    [Fact]
    public void DelayAndParalysisBoundaries()
    {
        Assert.Equal(2000, ObjMonMon35_2Core.DelayFor(2));
        Assert.Equal(200, ObjMonMon35_2Core.DelayFor(1));
        Assert.Equal(200, ObjMonMon35_2Core.DelayFor(3));

        Assert.True(ObjMonMon35_2Core.ParalysisFires(1, false));
        Assert.False(ObjMonMon35_2Core.ParalysisFires(1, true));
        Assert.False(ObjMonMon35_2Core.ParalysisFires(2, false));
        Assert.False(ObjMonMon35_2Core.ParalysisFires(3, false));

        Assert.Equal(2, ObjMonMon35_2Core.EffectFor(2));
    }

    [Fact]
    public void PipelineFacts()
    {
        Assert.True(ObjMonMon35_2Core.FourthPipelineAgain());
        Assert.True(ObjMonMon35_2Core.HasRateAddAndMultiplier());
        Assert.True(ObjMonMon35_2Core.PipelineMatchesJ235());
        Assert.True(ObjMonMon35_2Core.WithMaxPresent());
    }

    [Fact]
    public void CenterFacts()
    {
        Assert.True(ObjMonMon35_2Core.CenterFromParameter());
        Assert.True(ObjMonMon35_2Core.TrueMeansSelf());
        Assert.True(ObjMonMon35_2Core.FalseMeansTarget());

        Assert.Equal("self", ObjMonMon35_2Core.PickCenter(true));
        Assert.Equal("target", ObjMonMon35_2Core.PickCenter(false));
    }

    [Fact]
    public void ResourceAndMiscFacts()
    {
        Assert.True(ObjMonMon35_2Core.TrailingSemicolonOmitted());
        Assert.True(ObjMonMon35_2Core.ThirdOccurrence());
        Assert.True(ObjMonMon35_2Core.HasTryFinally());
        Assert.True(ObjMonMon35_2Core.FreeInFinally());
    }

    // ===================== 五、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonMon35_2Core.FamilyNowComplete());
        Assert.True(ObjMonMon35_2Core.FourOfFourPorted());
        Assert.True(ObjMonMon35_2Core.FamilyClosed());
        Assert.True(ObjMonMon35_2Core.OnePerBatch());
        Assert.True(ObjMonMon35_2Core.Mon35_2Closed());
        Assert.True(ObjMonMon35_2Core.NextClassIsXueLingLeader());
        Assert.True(ObjMonMon35_2Core.NextSectionChecked());
        Assert.True(ObjMonMon35_2Core.FortyClassesCovered());
        Assert.True(ObjMonMon35_2Core.RemainingApprox());

        Assert.Equal(4, ObjMonMon35_2Core.PortedMembers.Length);
        Assert.Equal("J204", ObjMonMon35_2Core.PortedMembers[0].Batch);
        Assert.Equal(116, ObjMonMon35_2Core.PortedMembers[3].Line);
    }
}
