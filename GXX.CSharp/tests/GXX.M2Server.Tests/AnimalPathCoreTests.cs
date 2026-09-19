using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J149：`TAnimalObject.RunToTargetXY`（15739-15795）与目标增删四层覆写链 1:1 测试。
/// **方向表、方向调整全表与重试循环调用次数均由临时探针实测后写入。**
/// </summary>
public sealed class AnimalPathCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(AnimalPathCore.ConstantsMatchSource());
        Assert.True(AnimalPathCore.DirectionsAreContiguous());
        Assert.True(AnimalPathCore.EightDirectionNames());
    }

    [Fact]
    public void DirectionConstants()
    {
        Assert.Equal(0, AnimalPathCore.DrUp);
        Assert.Equal(1, AnimalPathCore.DrUpRight);
        Assert.Equal(2, AnimalPathCore.DrRight);
        Assert.Equal(3, AnimalPathCore.DrDownRight);
        Assert.Equal(4, AnimalPathCore.DrDown);
        Assert.Equal(5, AnimalPathCore.DrDownLeft);
        Assert.Equal(6, AnimalPathCore.DrLeft);
        Assert.Equal(7, AnimalPathCore.DrUpLeft);
        Assert.Equal(8, AnimalPathCore.DirectionCount);
    }

    // ===================== 一、方向推导 =====================

    [Fact]
    public void DirectionIsDefaultDown()
    {
        Assert.True(AnimalPathCore.DirectionIsDefaultDown());
        Assert.True(AnimalPathCore.SamePositionKeepsDefaultDown());
        Assert.Equal(4, AnimalPathCore.DirectionFromDelta(5, 5, 5, 5));
    }

    [Fact]
    public void RightSideThreeCases()
    {
        Assert.True(AnimalPathCore.RightSideThreeCases());

        // 探针实测
        Assert.Equal(2, AnimalPathCore.DirectionFromDelta(9, 5, 5, 5));
        Assert.Equal(3, AnimalPathCore.DirectionFromDelta(9, 5, 9, 5));
        Assert.Equal(1, AnimalPathCore.DirectionFromDelta(9, 5, 1, 5));
    }

    [Fact]
    public void LeftSideThreeCases()
    {
        Assert.True(AnimalPathCore.LeftSideThreeCases());

        // 探针实测
        Assert.Equal(6, AnimalPathCore.DirectionFromDelta(1, 5, 5, 5));
        Assert.Equal(5, AnimalPathCore.DirectionFromDelta(1, 5, 9, 5));
        Assert.Equal(7, AnimalPathCore.DirectionFromDelta(1, 5, 1, 5));
    }

    [Fact]
    public void SameColumnThreeCases()
    {
        Assert.True(AnimalPathCore.SameColumnUsesElseIf());

        // 探针实测
        Assert.Equal(0, AnimalPathCore.DirectionFromDelta(5, 5, 1, 5));
        Assert.Equal(4, AnimalPathCore.DirectionFromDelta(5, 5, 9, 5));
        Assert.Equal(4, AnimalPathCore.DirectionFromDelta(5, 5, 5, 5));
    }

    [Fact]
    public void DirectionTableAllNine()
    {
        // **九种组合完整表，含同点**
        Assert.True(AnimalPathCore.DirectionTableAllNine());
    }

    [Fact]
    public void ParallelIfsNotElseIf()
    {
        Assert.True(AnimalPathCore.ParallelIfsNotElseIf());
        Assert.True(AnimalPathCore.AtTargetIsNoOpTruthTable());
    }

    // ===================== 二、方向调整与重试循环 =====================

    [Fact]
    public void AdjustDirectionCases()
    {
        Assert.True(AnimalPathCore.RandomOneIsPlusOne());
        Assert.True(AnimalPathCore.RandomZeroIsMinusOne());
        Assert.True(AnimalPathCore.ZeroDirectionWrapsToSeven());
        Assert.True(AnimalPathCore.OverSevenWrapsToZero());
    }

    [Fact]
    public void AdjustDirectionFullTable()
    {
        // **八个方向 × 两种随机值，逐一实测**
        Assert.True(AnimalPathCore.AdjustDirectionFullTable());

        Assert.Equal(1, AnimalPathCore.AdjustDirection(0, 1));
        Assert.Equal(7, AnimalPathCore.AdjustDirection(0, 0));
        Assert.Equal(0, AnimalPathCore.AdjustDirection(7, 1));
        Assert.Equal(6, AnimalPathCore.AdjustDirection(7, 0));
    }

    [Fact]
    public void DirectionRandomThreeValues()
    {
        Assert.True(AnimalPathCore.DirectionRandomModIsThree());
        Assert.True(AnimalPathCore.DirectionRandomThreeValues());
        Assert.Equal(3, AnimalPathCore.DirectionRandomMod);
    }

    [Fact]
    public void RetryLoopShape()
    {
        Assert.True(AnimalPathCore.RetryLoopIsEightTurns());
        Assert.True(AnimalPathCore.SameRandomAllEightTurns());
        Assert.True(AnimalPathCore.OldCoordinatesNeverUpdated());
        Assert.True(AnimalPathCore.RunToValueDiscarded());
    }

    [Fact]
    public void EffectiveAtMostOnce()
    {
        // 探针实测：tries=8、calls=1、moved=true
        Assert.True(AnimalPathCore.EffectiveAtMostOnce());

        int calls = 0;
        var r = AnimalPathCore.RunRetryLoop(4, 1, _ => { calls++; return true; });

        Assert.Equal(8, r.Tries);
        Assert.Equal(1, calls);
        Assert.True(r.Moved);
    }

    [Fact]
    public void AllTurnsAttemptWhenStuck()
    {
        // 探针实测：calls=16（每轮两次），**我最初按 8 写是错的**
        Assert.True(AnimalPathCore.AllTurnsAttemptWhenStuck());

        int calls = 0;
        var r = AnimalPathCore.RunRetryLoop(4, 1, _ => { calls++; return false; });

        Assert.Equal(8, r.Tries);
        Assert.Equal(16, calls);
        Assert.False(r.Moved);
    }

    [Fact]
    public void SecondAttemptSucceeds()
    {
        // 探针实测：calls=3、moved=true、FinalDir=5
        Assert.True(AnimalPathCore.SecondAttemptSucceeds());

        int calls = 0;
        var r = AnimalPathCore.RunRetryLoop(4, 1, _ => { calls++; return calls >= 2; });

        Assert.Equal(3, calls);
        Assert.Equal(5, r.FinalDir);
    }

    [Fact]
    public void FirstSuccessKeepsDirection()
    {
        // 探针实测：首步成功时 FinalDir 保持 4
        Assert.True(AnimalPathCore.FirstSuccessKeepsDirection());

        var r = AnimalPathCore.RunRetryLoop(4, 2, _ => true);

        Assert.Equal(4, r.FinalDir);
    }

    // ===================== 三、SetTargetCreat / DelTargetCreat =====================

    [Fact]
    public void OuterGate()
    {
        Assert.True(AnimalPathCore.OuterGateRequiresChange());
        Assert.True(AnimalPathCore.SetTargetOuterGate(false, false));
        Assert.False(AnimalPathCore.SetTargetOuterGate(true, false));
        Assert.False(AnimalPathCore.SetTargetOuterGate(false, true));
    }

    [Fact]
    public void NameGate()
    {
        Assert.True(AnimalPathCore.NameMismatchRejected());
        Assert.True(AnimalPathCore.EmptyNameSkipsNameCheck());
        Assert.True(AnimalPathCore.SameNameAccepted());
    }

    [Fact]
    public void DeadOrGhostGate()
    {
        Assert.True(AnimalPathCore.DeadOrGhostRejectedTruthTable());
        Assert.True(AnimalPathCore.DeadOrGhostRejected(true, false));
        Assert.False(AnimalPathCore.DeadOrGhostRejected(false, false));
    }

    [Fact]
    public void NilTargetAlwaysAllowed()
    {
        // **两道内层门只在目标非空时检查，清目标永远放行**
        Assert.True(AnimalPathCore.NilTargetAlwaysAllowed());
        Assert.True(AnimalPathCore.NilTargetSkipsInnerGates(true));
        Assert.False(AnimalPathCore.NilTargetSkipsInnerGates(false));
    }

    [Fact]
    public void SetTargetWrites()
    {
        Assert.True(AnimalPathCore.FourFieldsWritten());
        Assert.True(AnimalPathCore.DoTauntTargetCleared());
        Assert.Equal(4, AnimalPathCore.SetTargetWrites.Length);
    }

    [Fact]
    public void MissingParenthesesOnTick()
    {
        // **第二处 tick 赋值源码少了括号，写的是函数地址**
        Assert.True(AnimalPathCore.MissingParenthesesOnTick());
    }

    [Fact]
    public void DelTargetLayers()
    {
        Assert.True(AnimalPathCore.BaseDelClearsOneField());
        Assert.True(AnimalPathCore.BaseDelDoesNotTouchTicks());
        Assert.True(AnimalPathCore.SmartDelClearsFourMore());
        Assert.True(AnimalPathCore.AnimalDelClearsTwoMore());
        Assert.True(AnimalPathCore.LayerChainIsMonotonic());
        Assert.True(AnimalPathCore.TargetXyUseMinusOne());
        Assert.True(AnimalPathCore.DelTargetChainIsThreeLayers());

        Assert.Single(AnimalPathCore.BaseDelFields);
        Assert.Equal(4, AnimalPathCore.SmartDelExtraFields.Length);
        Assert.Equal(2, AnimalPathCore.AnimalDelExtraFields.Length);
        Assert.Equal(-1, AnimalPathCore.NoTargetCoordinate);
    }

    [Fact]
    public void AnimalSetTargetIsBareBlock()
    {
        // **整行注释掉的门 + 裸块无条件调用基类**
        Assert.True(AnimalPathCore.AnimalSetTargetIsBareBlock());
        Assert.True(AnimalPathCore.UnconditionalInherited());
        Assert.True(AnimalPathCore.CommentedOutAttackStateGatePresent());
    }

    [Fact]
    public void CommentedGateSemantics()
    {
        // **被注释掉的门原本是"排除攻击状态在 1..9999 之间"**
        Assert.True(AnimalPathCore.CommentedGateExclusionTruthTable());
        Assert.True(AnimalPathCore.CommentedGateWasExclusion(0));
        Assert.False(AnimalPathCore.CommentedGateWasExclusion(1));
        Assert.False(AnimalPathCore.CommentedGateWasExclusion(9999));
        Assert.True(AnimalPathCore.CommentedGateWasExclusion(10000));
    }
}
