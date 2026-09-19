using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J139：爆炸蜘蛛 / 大心脏 / 足球 —— `TExplosionSpider` / `TBigHeartMonster` / `TSoccerBall`
/// （ObjMon2.pas 47-53、66-75、181-190、614-697、1003-1083、1171-1369）1:1 测试。
/// </summary>
public sealed class ExplosionMonsterCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(ExplosionMonsterCore.ConstantsMatchSource());
        Assert.True(ExplosionMonsterCore.MessageIds());
        Assert.Equal(20048, ExplosionMonsterCore.RmStruck);
        Assert.Equal(30005, ExplosionMonsterCore.Rm10101);
        Assert.Equal(20114, ExplosionMonsterCore.Rm10205);
    }

    [Fact]
    public void RangeAndTimingConstants()
    {
        Assert.Equal(5, ExplosionMonsterCore.ExplosionViewRange);
        Assert.Equal(16, ExplosionMonsterCore.BigHeartViewRange);
        Assert.Equal(1, ExplosionMonsterCore.ExplosionHitRadius);
        Assert.Equal(60000, ExplosionMonsterCore.SuicideIntervalMs);
        Assert.Equal(1000, ExplosionMonsterCore.EarlyResearchMs);
        Assert.Equal(20, ExplosionMonsterCore.SoccerStepCap);
    }

    // ===================== 一、TBigHeartMonster =====================

    [Fact]
    public void BigHeartHasNoWalkTick()
    {
        // **本工程第一个完全不看走路/搜索节拍的怪物**
        Assert.True(ExplosionMonsterCore.BigHeartHasNoWalkTick());
        Assert.True(ExplosionMonsterCore.BigHeartAttacksEveryFrameWhenVisible());
        Assert.True(ExplosionMonsterCore.BigHeartRunTruthTable());
    }

    [Fact]
    public void BigHeartRunValues()
    {
        Assert.True(ExplosionMonsterCore.BigHeartRun(false, false, true, 1));
        Assert.False(ExplosionMonsterCore.BigHeartRun(false, false, true, 0));
        Assert.False(ExplosionMonsterCore.BigHeartRun(true, false, true, 5));
    }

    [Fact]
    public void GateUsesVisibleCountNotTarget()
    {
        // **判的是 Count 不是 m_TargetCret —— 无锁定目标也会进入**
        Assert.True(ExplosionMonsterCore.GateUsesVisibleCountNotTarget());
        Assert.True(ExplosionMonsterCore.EntersWithNoLockedTarget());
    }

    [Fact]
    public void UsesMapBaseObjectsNotVisibleList()
    {
        Assert.True(ExplosionMonsterCore.UsesMapBaseObjectsNotVisibleList());
        Assert.True(ExplosionMonsterCore.BigHeartLacksGhostFilter());
        Assert.True(ExplosionMonsterCore.BigHeartHasOneFewerFilter());
        Assert.True(ExplosionMonsterCore.MissingFilterIsGhost());
    }

    [Fact]
    public void FilterLists()
    {
        Assert.Equal(4, ExplosionMonsterCore.BigHeartFilters.Length);
        Assert.Equal(5, ExplosionMonsterCore.CentipedeFilters.Length);
        Assert.Contains("m_boDeath", ExplosionMonsterCore.BigHeartFilters);
        Assert.DoesNotContain("m_boGhost", ExplosionMonsterCore.BigHeartFilters);
        Assert.Contains("m_boGhost", ExplosionMonsterCore.CentipedeFilters);
    }

    [Fact]
    public void DelayParamsDifferFromCentipede()
    {
        // **类型 1/延迟 200 vs 蜈蚣王的 2/500**
        Assert.True(ExplosionMonsterCore.DelayParamsDifferFromCentipede());
        Assert.True(ExplosionMonsterCore.DelayParamValues());
    }

    [Fact]
    public void ExtraRm10205Message()
    {
        Assert.True(ExplosionMonsterCore.ExtraRm10205Message());
        Assert.True(ExplosionMonsterCore.Rm10205ArgsValues());
        Assert.Equal((20114, 0, 1), ExplosionMonsterCore.Rm10205HardcodedArgs());
    }

    [Fact]
    public void BigHeartLargestViewRange()
    {
        // **16 是目前全部怪物里最大的**
        Assert.True(ExplosionMonsterCore.BigHeartLargestViewRange());
        Assert.True(ExplosionMonsterCore.ViewRangeComparison());
    }

    [Fact]
    public void AttackTargetInheritedCommentedOut()
    {
        // **本项目第一处把父类调用注释掉**
        Assert.True(ExplosionMonsterCore.AttackTargetInheritedCommentedOut());
        Assert.True(ExplosionMonsterCore.RunInheritedStillLive());
        Assert.True(ExplosionMonsterCore.OnlyAttackTargetInheritedSkipped());
    }

    [Fact]
    public void ReusesSmallIntSpan()
    {
        Assert.True(ExplosionMonsterCore.ReusesSmallIntSpan());
        Assert.True(ExplosionMonsterCore.SameTruncationRisk());
        Assert.Equal(11, ExplosionMonsterCore.RandomSpan(10, 20));
        Assert.Equal(-25535, ExplosionMonsterCore.RandomSpan(0, 40000));
    }

    [Fact]
    public void BigHeartInitValues()
    {
        Assert.True(ExplosionMonsterCore.BigHeartInitValues());
        Assert.True(ExplosionMonsterCore.AnimalFlagOpposesStickMonster());
        Assert.Equal((16, false), ExplosionMonsterCore.BigHeartInit());
    }

    // ===================== 二、TExplosionSpider =====================

    [Fact]
    public void SelfDestructZerosHp()
    {
        // **自爆即把自己血量清零**
        Assert.True(ExplosionMonsterCore.SelfDestructZerosHp());
        Assert.Equal((0, 300), ExplosionMonsterCore.AfterSelfDestruct(300));
    }

    [Fact]
    public void ExplosionCovers9Cells()
    {
        // **锁敌半径 <= 1，共 9 格**
        Assert.True(ExplosionMonsterCore.ExplosionCovers9Cells());
        Assert.Equal(9, ExplosionMonsterCore.ExplosionCellCount());
        Assert.True(ExplosionMonsterCore.ExplosionRadiusBoundary());
    }

    [Fact]
    public void DefenseBranchInvertedVsIcicle()
    {
        // **为真时反而不减免 —— 与 J134 冰柱怪方向相反**
        Assert.True(ExplosionMonsterCore.DefenseBranchInvertedVsIcicle());
        Assert.True(ExplosionMonsterCore.CanCloseDefenseTrueMeansNoReduction());
        Assert.True(ExplosionMonsterCore.CanCloseDefenseFalseUsesReduction());
        Assert.True(ExplosionMonsterCore.BothHalvesOfPower());
    }

    [Fact]
    public void DefenseBranchValues()
    {
        Assert.Equal((50, 50), ExplosionMonsterCore.DefenseBranch(100, true, 10, 20));
        Assert.Equal((10, 20), ExplosionMonsterCore.DefenseBranch(100, false, 10, 20));
        Assert.Equal((50, 50), ExplosionMonsterCore.DefenseBranch(101, true, 0, 0));
    }

    [Fact]
    public void SumGateNotIndividual()
    {
        // **判的是 n1 + n2 之和**
        Assert.True(ExplosionMonsterCore.SumGateNotIndividual());
        Assert.True(ExplosionMonsterCore.SumGate(0, 5));
        Assert.False(ExplosionMonsterCore.SumGate(0, 0));
    }

    [Fact]
    public void NewAbilPowerSequence()
    {
        // **两次"物伤减少"（参数 2/3）后一次"元素增加"（参数 1，且是自己）**
        Assert.True(ExplosionMonsterCore.TwoNewAbilPowerCalls());
        Assert.True(ExplosionMonsterCore.IdenticalCommentsDifferentParams());
        Assert.True(ExplosionMonsterCore.SelfNewAbilPowerDifferentComment());
        Assert.True(ExplosionMonsterCore.NewAbilPowerSequenceValues());
        Assert.Equal(1, ExplosionMonsterCore.SelfNewAbilPowerParam());
    }

    [Fact]
    public void PowerMaxCommentHasDate()
    {
        Assert.True(ExplosionMonsterCore.PowerMaxCommentHasDate());
        Assert.Contains("2016-09-07", ExplosionMonsterCore.PowerMaxComment);
    }

    [Fact]
    public void AbsorbRaceTruthTable()
    {
        // **吸收三件套仅对玩家/英雄/人形怪三种族**
        Assert.True(ExplosionMonsterCore.AbsorbRaceTruthTable());
        Assert.True(ExplosionMonsterCore.AbsorbApplies(0));
        Assert.True(ExplosionMonsterCore.AbsorbApplies(150));
        Assert.False(ExplosionMonsterCore.AbsorbApplies(80));
    }

    [Fact]
    public void AbsorbThreeConditions()
    {
        Assert.True(ExplosionMonsterCore.AbsorbThreeConditions());
        Assert.True(ExplosionMonsterCore.SuckGate(1, 1, 1));
        Assert.False(ExplosionMonsterCore.SuckGate(0, 1, 1));
    }

    [Fact]
    public void AbsorbHasProbabilityGate()
    {
        Assert.True(ExplosionMonsterCore.AbsorbHasProbabilityGate());
        Assert.True(ExplosionMonsterCore.SuckProbabilityTruthTable());
    }

    [Fact]
    public void AbsorbClampedByPool()
    {
        // **吸伤点数被池子上限钳位**
        Assert.True(ExplosionMonsterCore.AbsorbRateDivisor1000());
        Assert.True(ExplosionMonsterCore.AbsorbClampedByPool());
        Assert.True(ExplosionMonsterCore.SuckDamagePointValues());
        Assert.True(ExplosionMonsterCore.ApplySuckValues());
        Assert.Equal(50, ExplosionMonsterCore.SuckDamagePoint(500, 100));
        Assert.Equal((0, 170), ExplosionMonsterCore.ApplySuck(30, 100, 200));
    }

    [Fact]
    public void NGDecTruthTable()
    {
        // **门是 >=（含相等）**
        Assert.True(ExplosionMonsterCore.NGDecTruthTable());
        Assert.True(ExplosionMonsterCore.NGDecUsesGreaterOrEqual());
        Assert.True(ExplosionMonsterCore.ApplyNGDecValues());
        Assert.Equal((7, 4), ExplosionMonsterCore.ApplyNGDec(10, 5, 3, 1));
    }

    [Fact]
    public void ParalysisModulusGuard()
    {
        // **麻痹最后一层有 Max(...,0) 保护；J134 冰柱怪没有**
        Assert.True(ExplosionMonsterCore.ParalysisModulusGuard(50, 50));
        Assert.True(ExplosionMonsterCore.IcicleLacksMaxGuard());
        Assert.True(ExplosionMonsterCore.GuardAllowsNormalModulus());
        Assert.True(ExplosionMonsterCore.GuardChangesBehaviorAtExtremes());
        Assert.True(ExplosionMonsterCore.BothAgreeOnNormalValues());
        Assert.True(ExplosionMonsterCore.GuardedModulusValues());
        Assert.True(ExplosionMonsterCore.ParalysisThreeConditions());
    }

    [Fact]
    public void ParalysisGuardSemantics()
    {
        // 极值下（antiPoison + rate <= 0）保护生效，模数为 0 → 该层不满足
        Assert.False(ExplosionMonsterCore.ParalysisModulusGuard(-100, 50));

        // 无保护的写法会得到负数模数（Random 未定义行为）
        Assert.Equal(-50, ExplosionMonsterCore.IcicleModulus(-100, 50));
        Assert.Equal(0, ExplosionMonsterCore.GuardedModulus(-100, 50));

        // 正常值下模数为正
        Assert.True(ExplosionMonsterCore.ParalysisModulusGuard(50, 50));
        Assert.Equal(100, ExplosionMonsterCore.GuardedModulus(50, 50));
    }

    [Fact]
    public void ParalysisTimeIsField()
    {
        Assert.True(ExplosionMonsterCore.ParalysisTimeIsField());
        Assert.Equal((5, 0), ExplosionMonsterCore.ParalysisParams());
    }

    [Fact]
    public void ReboundUsesFtTag()
    {
        // **反伤打自己，标记 'FT'**
        Assert.True(ExplosionMonsterCore.ReboundUsesFtTag());
        Assert.True(ExplosionMonsterCore.ReboundSelfInflicted());
        Assert.True(ExplosionMonsterCore.ReboundTargetsSelfOnly());
        Assert.Equal((50, 0), ExplosionMonsterCore.ReboundFlow(50));
    }

    [Fact]
    public void CommentedSendMsgAlternative()
    {
        Assert.True(ExplosionMonsterCore.CommentedSendMsgAlternative());
        Assert.True(ExplosionMonsterCore.CommentedUsesSendMsgNotDelay());
    }

    [Fact]
    public void AttackTargetMirrorsStickMonster()
    {
        // **与 J137 钉刺怪几乎逐字相同，但调自爆而非 Attack**
        Assert.True(ExplosionMonsterCore.AttackTargetMirrorsStickMonster());
        Assert.True(ExplosionMonsterCore.CallsSelfDestructNotAttack());
        Assert.True(ExplosionMonsterCore.InRangeActionIsSelfDestruct());
        Assert.True(ExplosionMonsterCore.SameOutsideRangeBehavior());
        Assert.True(ExplosionMonsterCore.HasSameAddressComments());
        Assert.Equal("sub_4A65C4", ExplosionMonsterCore.InRangeAction());
    }

    [Fact]
    public void SixtySecondSuicideTimer()
    {
        Assert.True(ExplosionMonsterCore.SixtySecondSuicideTimer());
        Assert.False(ExplosionMonsterCore.SuicideDue(0, 60000));
        Assert.True(ExplosionMonsterCore.SuicideDue(0, 60001));
    }

    [Fact]
    public void SearchGateTwoPaths()
    {
        // **"到点搜" 或 "没目标满 1 秒就搜"**
        Assert.True(ExplosionMonsterCore.SearchGateTwoPaths());
        Assert.True(ExplosionMonsterCore.EarlyReseachWhenNoTarget());
        Assert.True(ExplosionMonsterCore.EarlyResearchOnlyWithoutTarget());
        Assert.True(ExplosionMonsterCore.ResearchCommentHasDate());
    }

    [Fact]
    public void ExplosionRunGateTruthTable()
    {
        // **先死亡后幽灵，且不判 CanMove**
        Assert.True(ExplosionMonsterCore.GateOrderMatchesEarlyBatches());
        Assert.True(ExplosionMonsterCore.NoCanMoveCheck());
        Assert.True(ExplosionMonsterCore.ExplosionRunGateTruthTable());
        Assert.True(ExplosionMonsterCore.GateUsesFieldsNotMethod());
    }

    [Fact]
    public void ExplosionSpiderInit()
    {
        Assert.True(ExplosionMonsterCore.ExplosionSpiderInit());
        Assert.Equal((5, 250, 2500, 1500, 0u), ExplosionMonsterCore.ExplosionInit());
    }

    [Fact]
    public void FieldNameIsDecompilerArtifact()
    {
        // **n550/n554/n558/dw558 都是反编译产物**
        Assert.True(ExplosionMonsterCore.FieldNameIsDecompilerArtifact());
        Assert.True(ExplosionMonsterCore.SameFamilyFieldNames());
        Assert.True(ExplosionMonsterCore.FieldNamePattern("dw558"));
        Assert.True(ExplosionMonsterCore.FieldNamePattern("n554"));
        Assert.False(ExplosionMonsterCore.FieldNamePattern("GetPower"));
    }

    // ===================== 三、TSoccerBall =====================

    [Fact]
    public void TwoDirectionTables()
    {
        // **一份注释掉、一份是活的**
        Assert.True(ExplosionMonsterCore.TwoDirectionTables());
        Assert.Equal(8, ExplosionMonsterCore.OldDirectionTable.Length);
        Assert.Equal(8, ExplosionMonsterCore.NewDirectionTable.Length);
    }

    [Fact]
    public void OldTableIsMirror()
    {
        // **按奇偶分档：偶数 (d+4)%8、奇数 8-d（不是统一 +4 环绕）**
        Assert.True(ExplosionMonsterCore.OldTableIsMirror());
        Assert.True(ExplosionMonsterCore.OldTableEvenRule());
        Assert.True(ExplosionMonsterCore.OldTableOddRule());
        Assert.True(ExplosionMonsterCore.OldTableIsNotUniformRotation());
        Assert.True(ExplosionMonsterCore.OldTableIsInvolution());
        Assert.True(ExplosionMonsterCore.OldTableValues());
    }

    [Fact]
    public void OldDirectionFormulaValues()
    {
        // 偶数方向：(d+4)%8
        Assert.Equal(4, ExplosionMonsterCore.OldDirection(0));
        Assert.Equal(6, ExplosionMonsterCore.OldDirection(2));
        Assert.Equal(0, ExplosionMonsterCore.OldDirection(4));
        Assert.Equal(2, ExplosionMonsterCore.OldDirection(6));

        // 奇数方向：8-d
        Assert.Equal(7, ExplosionMonsterCore.OldDirection(1));
        Assert.Equal(5, ExplosionMonsterCore.OldDirection(3));
        Assert.Equal(3, ExplosionMonsterCore.OldDirection(5));
        Assert.Equal(1, ExplosionMonsterCore.OldDirection(7));

        // 奇数档确实不等于 (d+4)%8
        Assert.NotEqual((1 + 4) % 8, ExplosionMonsterCore.OldDirection(1));
    }

    [Fact]
    public void NewTableIsRotation()
    {
        Assert.True(ExplosionMonsterCore.NewTableIsRotation());
    }

    [Fact]
    public void TablesAgreeOnlyOnEvenDirections()
    {
        // **只在 0/2/4/6 上一致，在 1/3/5/7 上相反**
        Assert.True(ExplosionMonsterCore.TablesAgreeOnlyOnEvenDirections());
        Assert.True(ExplosionMonsterCore.TablesOppositeOnOddDirections());
        Assert.True(ExplosionMonsterCore.EvenDirectionsAgree());
    }

    [Fact]
    public void DirectionTableValues()
    {
        Assert.Equal(4, ExplosionMonsterCore.OldDirectionTable[0]);
        Assert.Equal(7, ExplosionMonsterCore.OldDirectionTable[1]);
        Assert.Equal(6, ExplosionMonsterCore.OldDirectionTable[2]);
        Assert.Equal(5, ExplosionMonsterCore.OldDirectionTable[3]);

        Assert.Equal(4, ExplosionMonsterCore.NewDirectionTable[0]);
        Assert.Equal(5, ExplosionMonsterCore.NewDirectionTable[1]);
        Assert.Equal(6, ExplosionMonsterCore.NewDirectionTable[2]);
        Assert.Equal(7, ExplosionMonsterCore.NewDirectionTable[3]);
    }

    [Fact]
    public void DirectionCommentAppearsTwice()
    {
        Assert.True(ExplosionMonsterCore.DirectionCommentAppearsTwice());
        Assert.Contains("20100629", ExplosionMonsterCore.DirectionComment);
        Assert.True(ExplosionMonsterCore.CommentedLineUsesOldOutVars());
    }

    [Fact]
    public void TurnsWhenPathIsWalkable()
    {
        // **"前方能走就转向"—— 与直觉相反**
        Assert.True(ExplosionMonsterCore.TurnsWhenPathIsWalkable());
        Assert.True(ExplosionMonsterCore.TurnsOnlyWhenWalkable());
        Assert.True(ExplosionMonsterCore.CanWalkThirdParamIsFalse());
        Assert.True(ExplosionMonsterCore.ForwardStepIsOne());
    }

    [Fact]
    public void SoccerStepValues()
    {
        Assert.Equal((true, 5), ExplosionMonsterCore.SoccerStep(true, 1));
        Assert.Equal((true, 7), ExplosionMonsterCore.SoccerStep(true, 3));
        Assert.Equal((false, 1), ExplosionMonsterCore.SoccerStep(false, 1));
    }

    [Fact]
    public void TryExceptStructure()
    {
        // **整个函数体在 try 内，inherited 在外面**
        Assert.True(ExplosionMonsterCore.TryExceptWrapsOnlyBody());
        Assert.True(ExplosionMonsterCore.InheritedOutsideTry());
        Assert.True(ExplosionMonsterCore.ExceptionSkipsInherited());
        Assert.True(ExplosionMonsterCore.ExceptionTagIsMethodName());
        Assert.True(ExplosionMonsterCore.TagStyleDiffersFromNumericCodes());
        Assert.Equal("TSoccerBall.Run", ExplosionMonsterCore.ExceptionTag);
    }

    [Fact]
    public void ZeroStepClearsTarget()
    {
        Assert.True(ExplosionMonsterCore.ZeroStepClearsTarget());
        Assert.True(ExplosionMonsterCore.ZeroStepCommentPresent());
    }

    [Fact]
    public void ArrivalAndDecrement()
    {
        // **到达清零；未到达且 > 0 则递减**
        Assert.True(ExplosionMonsterCore.ArrivalZeroesStep());
        Assert.True(ExplosionMonsterCore.StepDecrements());
        Assert.Equal(0, ExplosionMonsterCore.StepAfterArrival(5, true));
        Assert.Equal(2, ExplosionMonsterCore.StepDecrement(3));
    }

    [Fact]
    public void SoccerBallInit()
    {
        // **足球无敌**
        Assert.True(ExplosionMonsterCore.SoccerBallInit());
        Assert.True(ExplosionMonsterCore.SoccerBallIsSuperMan());
        Assert.Equal((false, true, 0, -1), ExplosionMonsterCore.SoccerInit());
    }

    [Fact]
    public void SoccerBallStruckNilGuard()
    {
        Assert.True(ExplosionMonsterCore.SoccerBallStruckNilGuard());
        Assert.True(ExplosionMonsterCore.StruckNilTruthTable());
    }

    [Fact]
    public void DirectionFollowsHitter()
    {
        Assert.True(ExplosionMonsterCore.DirectionFollowsHitter());
        Assert.Equal(3, ExplosionMonsterCore.StruckDirection(3));
        Assert.Equal(7, ExplosionMonsterCore.StruckDirection(7));
    }

    [Fact]
    public void StepAccumulatesFourPlusRandom()
    {
        // **每被踢一次累加 4 + Random(4)**
        Assert.True(ExplosionMonsterCore.StepAccumulatesFourPlusRandom());
        Assert.Equal(4, ExplosionMonsterCore.StruckStep(0, 0));
        Assert.Equal(7, ExplosionMonsterCore.StruckStep(0, 3));
        Assert.Equal(11, ExplosionMonsterCore.StruckStep(5, 2));
    }

    [Fact]
    public void StepCappedAt20()
    {
        Assert.True(ExplosionMonsterCore.MinMacroIsMathMin());
        Assert.True(ExplosionMonsterCore.AfterStruckValues());
        Assert.True(ExplosionMonsterCore.StepNeverExceedsCap());
        Assert.Equal(20, ExplosionMonsterCore.ClampStep(25));
        Assert.Equal(19, ExplosionMonsterCore.ClampStep(19));
    }

    [Fact]
    public void TargetComputedFromStep()
    {
        Assert.True(ExplosionMonsterCore.TargetComputedFromStep());
        Assert.True(ExplosionMonsterCore.TargetUsesStepAsDistance());
    }

    [Fact]
    public void GetNextPositionClamps()
    {
        // **越界时输出等于输入，且返回值恒为 True**
        Assert.True(ExplosionMonsterCore.GetNextPositionClampsToSelf());
        Assert.True(ExplosionMonsterCore.GetNextPositionAlwaysTrue());
        Assert.True(ExplosionMonsterCore.GetNextPositionRight());
        Assert.True(ExplosionMonsterCore.GetNextPositionBoundary());
        Assert.Equal((5, 4), ExplosionMonsterCore.GetNextPosition(5, 5, 0, 1, 100, 100));
        Assert.Equal((0, 0), ExplosionMonsterCore.GetNextPosition(0, 0, 0, 1, 100, 100));
    }

    // ===================== 四、仿真 =====================

    [Fact]
    public void ExplosionGateBlocks()
    {
        Assert.True(ExplosionMonsterCore.ExplosionGateBlocks());
    }

    [Fact]
    public void SuicidesAtSixtySeconds()
    {
        Assert.True(ExplosionMonsterCore.SuicidesAtSixtySeconds());
        Assert.True(ExplosionMonsterCore.DoesNotSuicideBeforeSixty());
    }

    [Fact]
    public void SuicideAndResearchAreIndependent()
    {
        // **自爆与搜索是两件独立的事**
        Assert.True(ExplosionMonsterCore.SuicideAndResearchAreIndependent());
    }

    [Fact]
    public void SelfDestructDamageValues()
    {
        Assert.True(ExplosionMonsterCore.SelfDestructDamageValues());
        Assert.True(ExplosionMonsterCore.SelfDestructDamageFloorsAtZero());
        Assert.Equal(100, ExplosionMonsterCore.SelfDestructDamage(100, true, 0, 0, 0, 0));
        Assert.Equal(30, ExplosionMonsterCore.SelfDestructDamage(100, false, 10, 20, 0, 0));
    }

    [Fact]
    public void KickBallValues()
    {
        Assert.True(ExplosionMonsterCore.KickBallValues());
        Assert.True(ExplosionMonsterCore.RepeatedKicksClamp());
        Assert.Equal((4, 3), ExplosionMonsterCore.KickBall(0, 0, 3));
        Assert.Equal((20, 7), ExplosionMonsterCore.KickBall(18, 3, 7));
    }

    [Fact]
    public void ExplosionRunValues()
    {
        var r = ExplosionMonsterCore.ExplosionRun(false, false, 0, 0, 60001, 3000, true);

        Assert.True(r.Suicided);
        Assert.True(r.Researched);
        Assert.Equal("ran", r.Path);
    }
}
