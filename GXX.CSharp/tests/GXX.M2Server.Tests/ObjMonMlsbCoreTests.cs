using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J209：`ObjMon.pas` 中 `TMLSBAttackMonster`（魔龙石碑怪物）
/// 两个方法 1:1 测试（合计 83 行）。
/// **本批最有价值的发现**：J207 的"群攻段无 `try..finally`"缺陷找到了
/// **内部对照** —— 本类（4960-5025）与 J207（4771-4843）结构几乎相同、
/// 相隔不到三百行，**一段有保护一段没有**，
/// 使该缺陷的定性从"缺少保护"细化为"保护策略不一致"。
/// 另：`m_boHideMode` 过滤在本文件并存两种等价写法、共 29 处。
/// </summary>
public sealed class ObjMonMlsbCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(4943, ObjMonMlsbCore.CreateStart);
        Assert.Equal(4948, ObjMonMlsbCore.CreateEnd);
        Assert.Equal(6, ObjMonMlsbCore.CreateLines);
        Assert.Equal(4950, ObjMonMlsbCore.AttackTargetStart);
        Assert.Equal(5026, ObjMonMlsbCore.AttackTargetEnd);
        Assert.Equal(77, ObjMonMlsbCore.AttackTargetLines);
        Assert.Equal(83, ObjMonMlsbCore.TotalLines);

        Assert.Equal(4947, ObjMonMlsbCore.MagicFlagFalseLine);
        Assert.Equal(4946, ObjMonMlsbCore.ViewRangeLine);
        Assert.Equal(2, ObjMonMlsbCore.ViewRange);
        Assert.Equal(1, ObjMonMlsbCore.FalseAssignSites);
        Assert.Equal(5, ObjMonMlsbCore.FlagSitesTotal);
        Assert.Equal(4667, ObjMonMlsbCore.PhysicalStart);
        Assert.Equal(4690, ObjMonMlsbCore.PhysicalEnd);
        Assert.Equal(24, ObjMonMlsbCore.PhysicalLines);

        Assert.Equal(4959, ObjMonMlsbCore.BaseCallLine);
        Assert.Equal(4960, ObjMonMlsbCore.ListCreateLine);
        Assert.Equal(4961, ObjMonMlsbCore.TryLine);
        Assert.Equal(4962, ObjMonMlsbCore.GetMapLine);
        Assert.Equal(2, ObjMonMlsbCore.GroupRadius);
        Assert.Equal(4966, ObjMonMlsbCore.LoopStart);
        Assert.Equal(5021, ObjMonMlsbCore.LoopEnd);
        Assert.Equal(4969, ObjMonMlsbCore.ExcludeTargetLine);
        Assert.Equal(4970, ObjMonMlsbCore.HideFilterLine);
        Assert.Equal(5023, ObjMonMlsbCore.FinallyLine);
        Assert.Equal(5024, ObjMonMlsbCore.FreeLine);
        Assert.Equal(5025, ObjMonMlsbCore.TryEndLine);

        Assert.Equal(4771, ObjMonMlsbCore.J207ListCreateLine);
        Assert.Equal(4843, ObjMonMlsbCore.J207FreeLine);
        Assert.Equal(12, ObjMonMlsbCore.AcceptFormSites);
        Assert.Equal(17, ObjMonMlsbCore.RejectFormSites);
        Assert.Equal(29, ObjMonMlsbCore.HideFilterTotal);
        Assert.Equal(3642, ObjMonMlsbCore.J204ParenBugLine);
        Assert.Equal(14, ObjMonMlsbCore.ClassesCovered);
        Assert.Equal(40, ObjMonMlsbCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonMlsbCore.SpanMatches());
        Assert.True(ObjMonMlsbCore.TotalLinesAddUp());
        Assert.True(ObjMonMlsbCore.CreateBeforeAttackTarget());
        Assert.True(ObjMonMlsbCore.WithinUnit());
        Assert.True(ObjMonMlsbCore.NoInstrumentation());
        Assert.True(ObjMonMlsbCore.TryImmediatelyAfterCreate());
        Assert.True(ObjMonMlsbCore.LoopInsideTry());
        Assert.True(ObjMonMlsbCore.PhysicalSpanMatches());
        Assert.True(ObjMonMlsbCore.FlagSitesConsistent());
        Assert.True(ObjMonMlsbCore.DiscountAfterPower());
        Assert.True(ObjMonMlsbCore.FlagFollowsViewRange());
    }

    // ===================== 一、物理分支的唯一受益者 =====================

    [Fact]
    public void PhysicalBranchFacts()
    {
        Assert.True(ObjMonMlsbCore.OnlyFalseAssign());
        Assert.True(ObjMonMlsbCore.EnablesPhysicalBranch());
        Assert.True(ObjMonMlsbCore.J206PredictionRealised());
        Assert.True(ObjMonMlsbCore.ViewRangeTwo());
        Assert.True(ObjMonMlsbCore.ViewRangeInsideCreate());
        Assert.True(ObjMonMlsbCore.FlagInsideCreate());
        Assert.True(ObjMonMlsbCore.GoesPhysical());
        Assert.True(ObjMonMlsbCore.TwoMeansTwoRoles());
        Assert.True(ObjMonMlsbCore.SmallestViewRange());
        Assert.True(ObjMonMlsbCore.FitsStationaryMonster());

        // **承接 J206 的分派：设假 => 物理**
        Assert.Equal("physical", ObjMonMlsbCore.Branch(false));
        Assert.Equal("magic", ObjMonMlsbCore.Branch(true));
    }

    // ===================== 二、先基类后群攻 =====================

    [Fact]
    public void BaseThenGroupFacts()
    {
        Assert.True(ObjMonMlsbCore.BaseFirstThenGroup());
        Assert.True(ObjMonMlsbCore.GroupNeverTouchesResult());
        Assert.True(ObjMonMlsbCore.ResultSemanticsMismatch());
        Assert.True(ObjMonMlsbCore.FalseDespiteGroupDamage());
        Assert.True(ObjMonMlsbCore.ExcludesPrimaryTarget());
        Assert.True(ObjMonMlsbCore.AvoidsDoubleDamage());
        Assert.True(ObjMonMlsbCore.UniqueAmongSiblings());
    }

    [Fact]
    public void TargetSelectionBoundaries()
    {
        // **普通合法目标入选**
        Assert.True(ObjMonMlsbCore.NormalTargetPasses());

        // **主目标被排除（避免与基类重复伤害）**
        Assert.True(ObjMonMlsbCore.PrimaryExcluded());

        // **隐藏者：无冷眼排除、有冷眼入选**
        Assert.True(ObjMonMlsbCore.HiddenExcluded());
        Assert.True(ObjMonMlsbCore.HiddenPassesWithCoolEye());

        // **其余排除项**
        Assert.True(ObjMonMlsbCore.DeadExcluded());
        Assert.True(ObjMonMlsbCore.GhostExcluded());
        Assert.True(ObjMonMlsbCore.OfflineExcluded());
        Assert.True(ObjMonMlsbCore.ImproperExcluded());
        Assert.True(ObjMonMlsbCore.NullExcluded());

    }

    // ===================== 三、try..finally 对照 =====================

    [Fact]
    public void TryFinallyContrast()
    {
        Assert.True(ObjMonMlsbCore.HasTryFinally());
        Assert.True(ObjMonMlsbCore.J207DoesNot());
        Assert.True(ObjMonMlsbCore.DirectInternalContrast());
        Assert.True(ObjMonMlsbCore.ThreeHundredLinesApart());
        Assert.True(ObjMonMlsbCore.FreeInFinally());
        Assert.True(ObjMonMlsbCore.FreeIsLastStatement());
        Assert.True(ObjMonMlsbCore.DifferentTeardownOrder());
        Assert.True(ObjMonMlsbCore.J207FreeIsBare());
        Assert.True(ObjMonMlsbCore.CitesJ207Defect());
        Assert.True(ObjMonMlsbCore.RefinesJ207Classification());
        Assert.True(ObjMonMlsbCore.FromOmissionToInconsistency());

        // **相隔 189 行，同一文件**
        Assert.Equal(189, ObjMonMlsbCore.DistanceBetweenGroups);
        Assert.True(ObjMonMlsbCore.DistanceBetweenGroups < 300);
    }

    // ===================== 四、半径与圆心 =====================

    [Fact]
    public void RadiusAndCenterFacts()
    {
        Assert.True(ObjMonMlsbCore.HardcodedRadius());
        Assert.True(ObjMonMlsbCore.J207UsesConfig());
        Assert.True(ObjMonMlsbCore.MixedStrategies());
        Assert.True(ObjMonMlsbCore.SelfCentered());
        Assert.True(ObjMonMlsbCore.J207TargetCentered());
        Assert.True(ObjMonMlsbCore.OppositeChoices());
        Assert.True(ObjMonMlsbCore.NoSecondFilterNeeded());
        Assert.True(ObjMonMlsbCore.RadiusMatchesViewRange());
        Assert.True(ObjMonMlsbCore.GetMapLineExtracted());

        Assert.Equal("g_Config.nSnowWindRange", ObjMonMlsbCore.J207RadiusSource);
        Assert.Equal("self", ObjMonMlsbCore.SelfCenter);
        Assert.Equal("target", ObjMonMlsbCore.J207Center);
    }

    // ===================== 五、两种隐藏过滤写法 =====================

    [Fact]
    public void HideFilterCensus()
    {
        Assert.True(ObjMonMlsbCore.AcceptFormUsed());
        Assert.True(ObjMonMlsbCore.TwoIdiomsCoexist());
        Assert.True(ObjMonMlsbCore.ComplementsProven());
        Assert.True(ObjMonMlsbCore.TwentyNineSites());
        Assert.True(ObjMonMlsbCore.LargestSplitSoFar());
        Assert.True(ObjMonMlsbCore.AcceptFormExtracted());
        Assert.True(ObjMonMlsbCore.RejectFormExtracted());
        Assert.True(ObjMonMlsbCore.TablesAscending());
        Assert.True(ObjMonMlsbCore.TablesDisjoint());
        Assert.True(ObjMonMlsbCore.CountsAddUp());
        Assert.True(ObjMonMlsbCore.LongerFilterChain());
        Assert.True(ObjMonMlsbCore.AddsLifeGhostHide());
        Assert.True(ObjMonMlsbCore.DropsDistanceTerm());

        Assert.Equal(12, ObjMonMlsbCore.AcceptFormLines.Length);
        Assert.Equal(17, ObjMonMlsbCore.RejectFormLines.Length);
        Assert.Equal(1427, ObjMonMlsbCore.AcceptFormLines[0]);
        Assert.Equal(4970, ObjMonMlsbCore.AcceptFormLines[5]);
        Assert.Equal(8357, ObjMonMlsbCore.AcceptFormLines[11]);
        Assert.Equal(3586, ObjMonMlsbCore.RejectFormLines[0]);
        Assert.Equal(3642, ObjMonMlsbCore.RejectFormLines[1]);
        Assert.Equal(8931, ObjMonMlsbCore.RejectFormLines[16]);
    }

    [Fact]
    public void TwoIdiomsAreComplements()
    {
        Assert.True(ObjMonMlsbCore.ComplementsInAllStates());
        Assert.True(ObjMonMlsbCore.VisiblePassesBoth());
        Assert.True(ObjMonMlsbCore.HiddenWithCoolEyePassesBoth());
        Assert.True(ObjMonMlsbCore.HiddenNoCoolEyeBlocksBoth());

        // **四态穷举：两式恒相反**
        Assert.True(ObjMonMlsbCore.AcceptForm(false, false));
        Assert.False(ObjMonMlsbCore.RejectForm(false, false));

        Assert.True(ObjMonMlsbCore.AcceptForm(false, true));
        Assert.False(ObjMonMlsbCore.RejectForm(false, true));

        Assert.True(ObjMonMlsbCore.AcceptForm(true, true));
        Assert.False(ObjMonMlsbCore.RejectForm(true, true));

        Assert.False(ObjMonMlsbCore.AcceptForm(true, false));
        Assert.True(ObjMonMlsbCore.RejectForm(true, false));
    }

    [Fact]
    public void J204ParenBugReproduced()
    {
        Assert.True(ObjMonMlsbCore.ReconfirmsJ204());
        Assert.True(ObjMonMlsbCore.NilGuardBypassed());
        Assert.True(ObjMonMlsbCore.IndependentReproduction());
        Assert.True(ObjMonMlsbCore.DifferWhenImproper());
        Assert.True(ObjMonMlsbCore.DefectObservable());
        Assert.True(ObjMonMlsbCore.AgreeWhenProper());

        // **`and` 优先于 `or`：非法目标时 `nil` 守卫被绕过**
        Assert.True(ObjMonMlsbCore.BuggyGuard(false, false, false, false));
        Assert.False(ObjMonMlsbCore.CorrectGuard(false, false, false, false));

        // **合法目标时两版一致**
        Assert.Equal(
            ObjMonMlsbCore.CorrectGuard(true, true, false, true),
            ObjMonMlsbCore.BuggyGuard(true, true, false, true));
    }

    // ===================== 六、伤害管线 =====================

    [Fact]
    public void PipelineFacts()
    {
        Assert.True(ObjMonMlsbCore.NoHealIdiom());
        Assert.True(ObjMonMlsbCore.NoPoisonNoParalysis());
        Assert.True(ObjMonMlsbCore.ShorterPipeline());
        Assert.True(ObjMonMlsbCore.FirstCounterexampleToHealIdiom());
        Assert.True(ObjMonMlsbCore.PipelineExtracted());
        Assert.True(ObjMonMlsbCore.MissingExtracted());
        Assert.True(ObjMonMlsbCore.MissingNotInPipeline());
        Assert.True(ObjMonMlsbCore.CapBeforeAbsorb());
        Assert.True(ObjMonMlsbCore.SameAsJ205J207());
        Assert.True(ObjMonMlsbCore.StillInconsistent());

        Assert.Equal(9, ObjMonMlsbCore.Pipeline.Length);
        Assert.Equal(2, ObjMonMlsbCore.MissingFromPipeline.Length);

        // **封顶在第 5 位、吸收在第 6 位 => 封顶在前**
        Assert.Equal(5, Array.IndexOf(ObjMonMlsbCore.Pipeline, "GetAttackPowerMax"));
        Assert.Equal(6, Array.IndexOf(ObjMonMlsbCore.Pipeline, "absorb"));
    }

    [Fact]
    public void MasterDiscountFacts()
    {
        Assert.True(ObjMonMlsbCore.MasterDiscount());
        Assert.True(ObjMonMlsbCore.SameAsJ207());
        Assert.True(ObjMonMlsbCore.AlsoInJ206CommentedStub());
        Assert.True(ObjMonMlsbCore.TripleAppearance());
        Assert.True(ObjMonMlsbCore.NoMasterNoDiscount());
        Assert.True(ObjMonMlsbCore.FullRateKeepsValue());
        Assert.True(ObjMonMlsbCore.HalfRateHalves());

        Assert.Equal(100, ObjMonMlsbCore.SlavePower(100, 100));
        Assert.Equal(50, ObjMonMlsbCore.SlavePower(100, 50));
        Assert.Equal(25, ObjMonMlsbCore.SlavePower(100, 25));
    }

    // ===================== 七、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonMlsbCore.NoMagicAttackTargetOverride());
        Assert.True(ObjMonMlsbCore.ReliesOnPhysical());
        Assert.True(ObjMonMlsbCore.TwoFactsCompose());
        Assert.True(ObjMonMlsbCore.NoRunOverride());
        Assert.True(ObjMonMlsbCore.InheritsJ206Run());
        Assert.True(ObjMonMlsbCore.ExplainsSearchBehaviour());
        Assert.True(ObjMonMlsbCore.FourteenClassesCovered());
        Assert.True(ObjMonMlsbCore.RemainingApprox());
    }
}
