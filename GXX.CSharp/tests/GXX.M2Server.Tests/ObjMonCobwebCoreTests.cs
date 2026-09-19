using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J203：`ObjMon.pas` 中 `TCobwebMonster` 三个方法 1:1 测试
/// （合计 184 行）。
/// **本批最严重的发现**：`MonAttackTarget` 的"直线范围攻击"两段里
/// `Obj` 被取出并校验、随后**被丢弃** ——
/// 两条 `Attack(...)` 的实参都是 `m_TargetCret`、
/// **"2 格范围攻击"从未发生、主目标反而被多打两次**。
/// </summary>
public sealed class ObjMonCobwebCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(3250, ObjMonCobwebCore.MonAttackStart);
        Assert.Equal(3293, ObjMonCobwebCore.MonAttackEnd);
        Assert.Equal(44, ObjMonCobwebCore.MonAttackLines);
        Assert.Equal(3295, ObjMonCobwebCore.CobwebStart);
        Assert.Equal(3427, ObjMonCobwebCore.CobwebEnd);
        Assert.Equal(133, ObjMonCobwebCore.CobwebLines);
        Assert.Equal(3429, ObjMonCobwebCore.AttackTargetStart);
        Assert.Equal(3435, ObjMonCobwebCore.AttackTargetEnd);
        Assert.Equal(7, ObjMonCobwebCore.AttackTargetLines);
        Assert.Equal(184, ObjMonCobwebCore.TotalLines);
        Assert.Equal(5, ObjMonCobwebCore.CobwebChanceDenominator);
        Assert.Equal(0, ObjMonCobwebCore.CobwebTriggerValue);
        Assert.Equal(3, ObjMonCobwebCore.GroupRadius);
        Assert.Equal(5, ObjMonCobwebCore.CobwebTimeBound);
        Assert.Equal(2, ObjMonCobwebCore.CobwebTimeBase);
        Assert.Equal(2, ObjMonCobwebCore.CobwebTimeMin);
        Assert.Equal(6, ObjMonCobwebCore.CobwebTimeMax);
        Assert.Equal(200, ObjMonCobwebCore.DelayMs);
        Assert.Equal(300, ObjMonCobwebCore.J201DelayMs);
        Assert.Equal(2, ObjMonCobwebCore.AttackDirRange);
        Assert.Equal(1, ObjMonCobwebCore.SplashStepNear);
        Assert.Equal(2, ObjMonCobwebCore.SplashStepFar);
        Assert.Equal(0, ObjMonCobwebCore.RC_PLAYOBJECT);
        Assert.Equal(1, ObjMonCobwebCore.RC_HEROOBJECT);
        Assert.Equal(16, ObjMonCobwebCore.AttackTargetCretSites);
        Assert.Equal(2, ObjMonCobwebCore.SuspectSites);
        Assert.Equal(0, ObjMonCobwebCore.AttackObjArgSites);
        Assert.Equal(8, ObjMonCobwebCore.ClassesCovered);
        Assert.Equal(46, ObjMonCobwebCore.RemainingClasses);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonCobwebCore.SpanMatches());
        Assert.True(ObjMonCobwebCore.TotalLinesAddUp());
        Assert.True(ObjMonCobwebCore.StartsAscending());
        Assert.True(ObjMonCobwebCore.MethodsAreContiguous());
        Assert.True(ObjMonCobwebCore.WithinUnit());
        Assert.True(ObjMonCobwebCore.NoInstrumentation());
    }

    // ===================== 一、Obj 被丢弃 =====================

    [Fact]
    public void ObjResolvedButDiscarded()
    {
        Assert.True(ObjMonCobwebCore.ObjResolvedButDiscarded());
        Assert.True(ObjMonCobwebCore.AttackPassesTargetCret());
        Assert.True(ObjMonCobwebCore.ObjNeverUsedAsArgument());
        Assert.True(ObjMonCobwebCore.MainTargetHitThreeTimes());
        Assert.True(ObjMonCobwebCore.SplashNeverOccurs());
        Assert.True(ObjMonCobwebCore.SplashSitesExtracted());
        Assert.True(ObjMonCobwebCore.ObjSitesExtracted());
        Assert.True(ObjMonCobwebCore.SuspectLinesExtracted());
        Assert.True(ObjMonCobwebCore.LocalTypoNotGlobal());
        Assert.True(ObjMonCobwebCore.SixteenSitesInFile());
        Assert.True(ObjMonCobwebCore.OthersAreCorrect());
        Assert.True(ObjMonCobwebCore.OnlyTwoAreSuspect());
        Assert.True(ObjMonCobwebCore.TwoDifferentSteps());
        Assert.True(ObjMonCobwebCore.StepsAreOneAndTwo());

        Assert.Equal(new[] { 3273, 3277 }, ObjMonCobwebCore.SplashCallSites);
        Assert.Equal(new[] { 3271, 3275 }, ObjMonCobwebCore.ObjResolveSites);
        Assert.Equal(new[] { 3273, 3277 }, ObjMonCobwebCore.SuspectLineNumbers);
    }

    [Fact]
    public void SplashSimulation()
    {
        Assert.True(ObjMonCobwebCore.ThreeMainHits());
        Assert.True(ObjMonCobwebCore.OneHitWhenBothEmpty());
        Assert.True(ObjMonCobwebCore.TwoHitsWhenOnlyNear());
        Assert.True(ObjMonCobwebCore.NeverHitsOthers());

        // **两格都有合法目标：主目标被打三次**
        Assert.Equal(new[] { "main@3267", "main@3273", "main@3277" },
            ObjMonCobwebCore.SimulateSplash(false, true, false, true));

        // **都为空：只打一次**
        Assert.Equal(new[] { "main@3267" },
            ObjMonCobwebCore.SimulateSplash(true, false, true, false));

        // ****没有任何一次打的是"别人"** —— 范围攻击确实没发生**
        foreach (string h in ObjMonCobwebCore.SimulateSplash(false, true, false, true))
        {
            Assert.StartsWith("main@", h);
        }
    }

    // ===================== 二、GetAttackDir 双重重载 =====================

    [Fact]
    public void GetAttackDirOverloads()
    {
        Assert.True(ObjMonCobwebCore.TwoOverloads());
        Assert.True(ObjMonCobwebCore.OrShortCircuits());
        Assert.True(ObjMonCobwebCore.RangeVersionWritesDir());
        Assert.True(ObjMonCobwebCore.SameInBothMethods());
        Assert.True(ObjMonCobwebCore.RangeIsTwo());
        Assert.True(ObjMonCobwebCore.VerbatimDuplication());
        Assert.True(ObjMonCobwebCore.SameComment());
        Assert.True(ObjMonCobwebCore.OnlyVariableNameDiffers());
        Assert.True(ObjMonCobwebCore.SharedProloguesExtracted());
        Assert.True(ObjMonCobwebCore.ProloguesSameLength());
        Assert.True(ObjMonCobwebCore.RangeAloneSuffices());
        Assert.True(ObjMonCobwebCore.AdjacentAloneSuffices());
        Assert.True(ObjMonCobwebCore.NeitherFails());

        Assert.Equal(2, ObjMonCobwebCore.SharedPrologues.Length);
        Assert.Equal(3260, ObjMonCobwebCore.SharedPrologues[0].Start);
        Assert.Equal(3311, ObjMonCobwebCore.SharedPrologues[1].Start);
    }

    [Fact]
    public void EitherDirBoundaries()
    {
        Assert.True(ObjMonCobwebCore.EitherDirMatches(true, false));
        Assert.True(ObjMonCobwebCore.EitherDirMatches(false, true));
        Assert.True(ObjMonCobwebCore.EitherDirMatches(true, true));
        Assert.False(ObjMonCobwebCore.EitherDirMatches(false, false));
    }

    // ===================== 三、AttackTarget 分派 =====================

    [Fact]
    public void DispatchFacts()
    {
        Assert.True(ObjMonCobwebCore.OneInFiveChance());
        Assert.True(ObjMonCobwebCore.TwentyPercentCobweb());
        Assert.True(ObjMonCobwebCore.EightyPercentNormal());
        Assert.True(ObjMonCobwebCore.BothCallsParenthesized());
        Assert.True(ObjMonCobwebCore.CobwebIsSuperset());
        Assert.True(ObjMonCobwebCore.SharedPrologue());
        Assert.True(ObjMonCobwebCore.ExtraGroupAttack());
        Assert.True(ObjMonCobwebCore.RollZeroGoesCobweb());
        Assert.True(ObjMonCobwebCore.RollOneGoesMon());
        Assert.True(ObjMonCobwebCore.RollFourGoesMon());
        Assert.True(ObjMonCobwebCore.ExactlyOneCobwebValue());
    }

    [Fact]
    public void DispatchBoundaries()
    {
        Assert.Equal("cobweb", ObjMonCobwebCore.Dispatch(0));
        Assert.Equal("mon", ObjMonCobwebCore.Dispatch(1));
        Assert.Equal("mon", ObjMonCobwebCore.Dispatch(4));

        // **五种取值里恰一种走蛛网**
        int cobweb = 0;

        for (int r = 0; r < 5; r++)
        {
            if (ObjMonCobwebCore.Dispatch(r) == "cobweb")
                cobweb++;
        }

        Assert.Equal(1, cobweb);
    }

    // ===================== 四、群体攻击 =====================

    [Fact]
    public void GroupAttackFacts()
    {
        Assert.True(ObjMonCobwebCore.RadiusThree());
        Assert.True(ObjMonCobwebCore.CenteredOnTarget());
        Assert.True(ObjMonCobwebCore.DifferentFromSpitRadius());
        Assert.True(ObjMonCobwebCore.TryFinallyFree());
        Assert.True(ObjMonCobwebCore.ProperResourceRelease());
        Assert.True(ObjMonCobwebCore.TwoFoldFilter());
        Assert.True(ObjMonCobwebCore.NegatedForm());
        Assert.True(ObjMonCobwebCore.OppositeStructureToJ199());
    }

    [Fact]
    public void OfflineFilterBoundaries()
    {
        Assert.True(ObjMonCobwebCore.OfflinePlayerSkipped());
        Assert.True(ObjMonCobwebCore.ConfigOffNotSkipped());
        Assert.True(ObjMonCobwebCore.NonPlayerNotSkipped());
        Assert.True(ObjMonCobwebCore.OnlineNotSkipped());

        Assert.False(ObjMonCobwebCore.ShouldSkip(true, true, true));
        Assert.True(ObjMonCobwebCore.ShouldSkip(false, true, true));
        Assert.True(ObjMonCobwebCore.ShouldSkip(true, false, true));
        Assert.True(ObjMonCobwebCore.ShouldSkip(true, true, false));
    }

    [Fact]
    public void CommentDriftFacts()
    {
        Assert.True(ObjMonCobwebCore.CommentAfterTargetLine());
        Assert.True(ObjMonCobwebCore.CommentBelongsTo3348());
        Assert.True(ObjMonCobwebCore.DriftedFromJ201());
    }

    // ===================== 五、伤害管线次序 =====================

    [Fact]
    public void PipelineOrderDiffers()
    {
        Assert.True(ObjMonCobwebCore.PipelineOrderDiffersFromJ201());
        Assert.True(ObjMonCobwebCore.CapAndAbsorbSwapped());
        Assert.True(ObjMonCobwebCore.AbilPowerAndRateSwapped());
        Assert.True(ObjMonCobwebCore.NoUnifiedOrder());
        Assert.True(ObjMonCobwebCore.SameElementsDifferentOrder());
        Assert.True(ObjMonCobwebCore.BothHaveSevenStages());
    }

    [Fact]
    public void PipelineIsNotEqual()
    {
        // **两条管线不相同（`Assert.False`、非探针可自动判定的肯定事实）**
        Assert.False(ObjMonCobwebCore.PipelinesEqual());

        // **元素集合相同、只是次序不同**
        Assert.True(ObjMonCobwebCore.SameElementsDifferentOrder());

        // **本处：吸收在封顶之前**
        Assert.True(
            Array.IndexOf(ObjMonCobwebCore.ThisPipeline, "Absorb")
            < Array.IndexOf(ObjMonCobwebCore.ThisPipeline, "GetAttackPowerMax"));

        // **J201：封顶在吸收之前（相反）**
        Assert.True(
            Array.IndexOf(ObjMonCobwebCore.J201Pipeline, "GetAttackPowerMax")
            < Array.IndexOf(ObjMonCobwebCore.J201Pipeline, "Absorb"));

        Assert.Equal(7, ObjMonCobwebCore.ThisPipeline.Length);
        Assert.Equal(7, ObjMonCobwebCore.J201Pipeline.Length);
    }

    // ===================== 六、回血、蛛网、延迟 =====================

    [Fact]
    public void SelfHealFacts()
    {
        Assert.True(ObjMonCobwebCore.SelfHealByLowByteOfMp());
        Assert.True(ObjMonCobwebCore.DividesByMpLowByte());
        Assert.True(ObjMonCobwebCore.ZeroSkipsHeal());
        Assert.True(ObjMonCobwebCore.IntegerHoldsByte());
        Assert.True(ObjMonCobwebCore.ZeroMpNoHeal());
        Assert.True(ObjMonCobwebCore.OneMpFullHeal());
        Assert.True(ObjMonCobwebCore.TenMpTenthHeal());
        Assert.True(ObjMonCobwebCore.MaxLowByteIs255());

        Assert.Equal(0, ObjMonCobwebCore.HealAmount(1000, 0));
        Assert.Equal(1000, ObjMonCobwebCore.HealAmount(1000, 1));
        Assert.Equal(100, ObjMonCobwebCore.HealAmount(1000, 10));
        Assert.Equal(1, ObjMonCobwebCore.HealAmount(255, 255));
    }

    [Fact]
    public void CobwebFacts()
    {
        Assert.True(ObjMonCobwebCore.CobwebOnlyForPlayers());
        Assert.True(ObjMonCobwebCore.NotSelf());
        Assert.True(ObjMonCobwebCore.Duration2To6());
        Assert.True(ObjMonCobwebCore.UpperBoundIsSix());
        Assert.True(ObjMonCobwebCore.MinTimeIsTwo());
        Assert.True(ObjMonCobwebCore.MaxTimeIsSix());
        Assert.True(ObjMonCobwebCore.AllTimesInRange());
        Assert.True(ObjMonCobwebCore.PlayerWebbable());
        Assert.True(ObjMonCobwebCore.HeroWebbable());
        Assert.True(ObjMonCobwebCore.MonsterNotWebbable());
        Assert.True(ObjMonCobwebCore.LightingEffectAfterLoop());
        Assert.True(ObjMonCobwebCore.TargetWebbedAgain());
        Assert.True(ObjMonCobwebCore.PossibleDoubleWeb());
    }

    [Fact]
    public void CobwebTimeBoundaries()
    {
        // **时长 2..6（上界不是 7）**
        Assert.Equal(2, ObjMonCobwebCore.CobwebTime(0));
        Assert.Equal(6, ObjMonCobwebCore.CobwebTime(4));
        Assert.NotEqual(7, ObjMonCobwebCore.CobwebTime(4));

        // **种族：只有玩家与英雄**
        Assert.True(ObjMonCobwebCore.CobwebRaceAllowed(0));
        Assert.True(ObjMonCobwebCore.CobwebRaceAllowed(1));
        Assert.False(ObjMonCobwebCore.CobwebRaceAllowed(80));
        Assert.False(ObjMonCobwebCore.CobwebRaceAllowed(10));
    }

    [Fact]
    public void DelayAndStyleFacts()
    {
        Assert.True(ObjMonCobwebCore.DelayTwoHundred());
        Assert.True(ObjMonCobwebCore.DiffersFromJ201());
        Assert.True(ObjMonCobwebCore.ReboundTagFTAgain());
        Assert.True(ObjMonCobwebCore.NoPoisonOnlyParalysis());
        Assert.True(ObjMonCobwebCore.ParalysisVerbatimWithJ201());
        Assert.True(ObjMonCobwebCore.FiveLineBraceComment());
        Assert.True(ObjMonCobwebCore.OldPackedDcFormAgain());
        Assert.True(ObjMonCobwebCore.DifferentCommentStyleThanJ201());
        Assert.True(ObjMonCobwebCore.UsesGetAttackPower());
        Assert.True(ObjMonCobwebCore.ManuallyRolledInJ201());
        Assert.True(ObjMonCobwebCore.TwoApproachesCoexist());
    }

    [Fact]
    public void VariableScopingFacts()
    {
        Assert.True(ObjMonCobwebCore.PowerReusedAsRebound());
        Assert.True(ObjMonCobwebCore.OverwrittenInsideLoop());
        Assert.True(ObjMonCobwebCore.SameFamilyAsJ201());
        Assert.True(ObjMonCobwebCore.DamagePerTarget());
        Assert.True(ObjMonCobwebCore.PowerSharedAcrossTargets());
        Assert.True(ObjMonCobwebCore.InconsistentScoping());
    }

    // ===================== 七、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonCobwebCore.ThreeDeclaredMethods());
        Assert.True(ObjMonCobwebCore.NoRunOrCreateOverride());
        Assert.True(ObjMonCobwebCore.ElseBranchDuplicated());
        Assert.True(ObjMonCobwebCore.MarkerZeroFFF0());
        Assert.True(ObjMonCobwebCore.MarkerZeroFFF1());
        Assert.True(ObjMonCobwebCore.MarkersDiffer());
        Assert.True(ObjMonCobwebCore.EightClassesCovered());
        Assert.True(ObjMonCobwebCore.RemainingApprox());

        Assert.Equal(new[] { "0FFF0h", "0FFF1h" }, ObjMonCobwebCore.ElseMarkers);
    }
}
