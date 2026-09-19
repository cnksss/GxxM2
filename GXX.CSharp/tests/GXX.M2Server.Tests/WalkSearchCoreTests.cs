using System;
using System.Collections.Generic;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J142：`TBaseObject.WalkTo`（13562-13689）、`TAnimalObject.SearchTarget`（39969-40113）、
/// `TAnimalObject.sub_4C959C`（40115-40152）1:1 测试。
/// </summary>
public sealed class WalkSearchCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void ConstantsMatchSource()
    {
        Assert.True(WalkSearchCore.ConstantsMatchSource());
        Assert.True(WalkSearchCore.MessageValues());
        Assert.True(WalkSearchCore.NpcRangeIsTenToFifty());
    }

    [Fact]
    public void ConstantValues()
    {
        Assert.Equal(20002, WalkSearchCore.RmWalk);
        Assert.Equal(20001, WalkSearchCore.RmTurn);
        Assert.Equal(11, WalkSearchCore.RcGuard);
        Assert.Equal(10, WalkSearchCore.RcNpc);
        Assert.Equal(50, WalkSearchCore.RcAnimal);
        Assert.Equal(112, WalkSearchCore.RcArcherGuard);
        Assert.Equal(142, WalkSearchCore.RcMoveArcherGuard);
        Assert.Equal(55, WalkSearchCore.PracticeMonsterRace);
        Assert.Equal(8, WalkSearchCore.StateTransparent);
        Assert.Equal(999, WalkSearchCore.DistanceInit);
        Assert.Equal(20, WalkSearchCore.TauntDistanceTolerance);
    }

    // ===================== 一、WalkTo =====================

    [Fact]
    public void DeltaTableCorrect()
    {
        Assert.True(WalkSearchCore.DeltaTableCorrect());
        Assert.True(WalkSearchCore.DiagonalVsCardinal());
        Assert.Equal(8, WalkSearchCore.Deltas.Length);
    }

    [Fact]
    public void DeltaValues()
    {
        Assert.Equal((0, -1), WalkSearchCore.Deltas[0]);
        Assert.Equal((1, -1), WalkSearchCore.Deltas[1]);
        Assert.Equal((1, 0), WalkSearchCore.Deltas[2]);
        Assert.Equal((1, 1), WalkSearchCore.Deltas[3]);
        Assert.Equal((0, 1), WalkSearchCore.Deltas[4]);
        Assert.Equal((-1, 1), WalkSearchCore.Deltas[5]);
        Assert.Equal((-1, 0), WalkSearchCore.Deltas[6]);
        Assert.Equal((-1, -1), WalkSearchCore.Deltas[7]);
    }

    [Fact]
    public void IllegalDirectionToOrigin()
    {
        // **case 无 else：非法方向映射到 (0,0)**
        Assert.True(WalkSearchCore.IllegalDirectionToOrigin());
        Assert.Equal((0, 0), WalkSearchCore.TargetCell(50, 60, 99));
    }

    [Fact]
    public void LegalDirectionValues()
    {
        Assert.True(WalkSearchCore.LegalDirectionValues());
    }

    [Fact]
    public void BoundsTruthTable()
    {
        Assert.True(WalkSearchCore.BoundsTruthTable());
        Assert.True(WalkSearchCore.BoundsAreInclusive());
        Assert.True(WalkSearchCore.InBounds(0, 0, 10, 10));
        Assert.False(WalkSearchCore.InBounds(10, 10, 10, 10));
    }

    [Fact]
    public void ImprisonTruthTable()
    {
        // **禁锢区是以位置为中心、范围为半径的方形，四边均含端点**
        // pos=(5,5) range=2 → 区域 [3,7]x[3,7]（探针逐格打印确认）
        Assert.True(WalkSearchCore.ImprisonTruthTable());
        Assert.True(WalkSearchCore.ImprisonAreaIsSquare());
        Assert.True(WalkSearchCore.ImprisonBoundariesInclusive());

        Assert.False(WalkSearchCore.Imprisoned(7, 7, 5, 5, 2));   // 上边界 7，在区内
        Assert.True(WalkSearchCore.Imprisoned(8, 5, 5, 5, 2));    // 8 > 7，越界
        Assert.False(WalkSearchCore.Imprisoned(3, 5, 5, 5, 2));   // 下边界 3，在区内
        Assert.True(WalkSearchCore.Imprisoned(2, 5, 5, 5, 2));    // 2 < 3，越界
    }

    [Fact]
    public void FireWallTruthTable()
    {
        // **怪物不走火墙格**
        Assert.True(WalkSearchCore.FireWallTruthTable());
        Assert.True(WalkSearchCore.MonsterAvoidsFireWall(false, false));
        Assert.False(WalkSearchCore.MonsterAvoidsFireWall(true, false));
    }

    [Fact]
    public void DingShenTruthTable()
    {
        Assert.True(WalkSearchCore.DingShenTruthTable());
        Assert.True(WalkSearchCore.DingShenGateOutsideTry());
        Assert.False(WalkSearchCore.DingShenGate(false, false));
        Assert.True(WalkSearchCore.DingShenGate(true, false));
        Assert.True(WalkSearchCore.DingShenGate(false, true));
    }

    [Fact]
    public void RollbackSemantics()
    {
        // **走失败要把自己从地图摘下再挪回去**
        Assert.True(WalkSearchCore.RollbackOnWalkFailure());
        Assert.True(WalkSearchCore.RollbackThreeSteps());
        Assert.True(WalkSearchCore.DeleteResultIgnored());
        Assert.Equal(3, WalkSearchCore.RollbackSteps.Length);
    }

    [Fact]
    public void DirectionAssignsBeforeSuccess()
    {
        // **方向在成功之前写入、失败不回滚**
        Assert.True(WalkSearchCore.DirectionAssignsBeforeSuccess());
        Assert.True(WalkSearchCore.DirectionAssignIsInsideTry());
        Assert.True(WalkSearchCore.CoordsUpdateOnlyOnMoveSuccess());
        Assert.True(WalkSearchCore.WalkMsgOnlyWhenMoved());
        Assert.True(WalkSearchCore.StationTickOnSuccessOnly());
    }

    [Fact]
    public void CommentedDebugLine()
    {
        Assert.True(WalkSearchCore.HasCommentedDebugOutput());
        Assert.True(WalkSearchCore.CommentedDebugLine());
        Assert.Contains("OutputDebugString", WalkSearchCore.CommentedDebug);
    }

    [Fact]
    public void ExceptionMsgFormat()
    {
        Assert.True(WalkSearchCore.ExceptionMsgFormat());
        Assert.Equal("[Exception] TBaseObject.WalkTo", WalkSearchCore.ExceptionMsg);
    }

    [Fact]
    public void WalkToAddressComment()
    {
        Assert.True(WalkSearchCore.WalkToAddressComment());
        Assert.Contains("004C3F64", WalkSearchCore.WalkToAddress);
    }

    [Fact]
    public void MasterFrontBlockCommented()
    {
        // **被注释掉的"不能走到主人面前"，作者自称不知为何要加**
        Assert.True(WalkSearchCore.MasterFrontBlockCommented());
        Assert.True(WalkSearchCore.MasterFrontCommentHasMarks());
        Assert.True(WalkSearchCore.AuthorUnsure());
        Assert.True(WalkSearchCore.SameFamilyAsGuardFix());
        Assert.Contains("??????", WalkSearchCore.MasterFrontComment);
        Assert.Contains("2017-11-02", WalkSearchCore.MasterFrontComment);
    }

    [Fact]
    public void MasterFrontWouldBlock()
    {
        Assert.True(WalkSearchCore.MasterFrontWouldBlock(5, 5, 5, 5));
        Assert.False(WalkSearchCore.MasterFrontWouldBlock(5, 5, 6, 5));
    }

    // ---- 隐身破除 ----

    [Fact]
    public void HideBreakTruthTable()
    {
        Assert.True(WalkSearchCore.HideBreakTruthTable());
        Assert.True(WalkSearchCore.HideBreakGate(true, true));
        Assert.False(WalkSearchCore.HideBreakGate(true, false));
    }

    [Fact]
    public void HideModeExTruthTable()
    {
        // **只有玩家/英雄才看 m_nHideModeEx，怪物恒为真**
        Assert.True(WalkSearchCore.HideModeExTruthTable());
        Assert.True(WalkSearchCore.MonsterIgnoresHideModeEx());
        Assert.True(WalkSearchCore.HideModeExGate(80, 999));
    }

    [Fact]
    public void HideClearFourSteps()
    {
        Assert.True(WalkSearchCore.HideClearFourSteps());
        Assert.Equal(4, WalkSearchCore.HideClearSteps.Length);
        Assert.True(WalkSearchCore.HideClearBoundary());
    }

    [Fact]
    public void HideBreakCommentHasDate()
    {
        Assert.True(WalkSearchCore.HideBreakCommentHasDate());
        Assert.Contains("2020-03-23", WalkSearchCore.HideBreakComment);
    }

    [Fact]
    public void CommentedAlternative()
    {
        // **被 (* *) 注释掉的"把时长置 1"替代实现**
        Assert.True(WalkSearchCore.HasCommentedAlternative());
        Assert.True(WalkSearchCore.CommentedAlternativeSetsOne());
        Assert.True(WalkSearchCore.CommentedAltTwoLines());
        Assert.True(WalkSearchCore.CommentedAltHasDate());
        Assert.Equal(2, WalkSearchCore.CommentedHiddenAlt.Length);
    }

    [Fact]
    public void CanWalkToCases()
    {
        Assert.True(WalkSearchCore.CanWalkToCases());

        // 正常可走
        Assert.True(WalkSearchCore.CanWalkTo(5, 5, 2, 10, 10, false, false, false, 0, 0, 0, false, (x, y) => true));

        // 定身拦下
        Assert.False(WalkSearchCore.CanWalkTo(5, 5, 2, 10, 10, true, false, false, 0, 0, 0, false, (x, y) => true));

        // 越界拦下
        Assert.False(WalkSearchCore.CanWalkTo(9, 5, 2, 10, 10, false, false, false, 0, 0, 0, false, (x, y) => true));

        // 禁锢拦下（nx=7 才越界，故从 x=6 出发）
        Assert.False(WalkSearchCore.CanWalkTo(6, 5, 2, 10, 10, false, false, true, 5, 5, 1, false, (x, y) => true));
        Assert.True(WalkSearchCore.ImprisonBoundaryNotBlocking());
        Assert.True(WalkSearchCore.BoundsEdgeNotBlocking());

        // 怪物遇火墙拦下
        Assert.False(WalkSearchCore.CanWalkTo(5, 5, 2, 10, 10, false, false, false, 0, 0, 0, true, (x, y) => false));

        // 玩家遇火墙不拦
        Assert.True(WalkSearchCore.CanWalkTo(5, 5, 2, 10, 10, false, false, false, 0, 0, 0, false, (x, y) => false));
    }

    // ===================== 二、SearchTarget =====================

    [Fact]
    public void PetGateTruthTable()
    {
        // **三态 m_boEnabledPetAttack**
        Assert.True(WalkSearchCore.PetGateTruthTable());
        Assert.True(WalkSearchCore.FlagOneForcesAllow());
        Assert.True(WalkSearchCore.FlagTwoForcesDeny());
        Assert.True(WalkSearchCore.FlagZeroFollowsConfig());
    }

    [Fact]
    public void PetGateValues()
    {
        Assert.False(WalkSearchCore.PetGate(false, 0, false));
        Assert.False(WalkSearchCore.PetGate(true, 1, false));
        Assert.True(WalkSearchCore.PetGate(true, 0, false));
        Assert.False(WalkSearchCore.PetGate(true, 0, true));
        Assert.True(WalkSearchCore.PetGate(true, 2, true));
    }

    [Fact]
    public void PetGateClearsTarget()
    {
        Assert.True(WalkSearchCore.PetGateClearsTarget());
        Assert.True(WalkSearchCore.PetGateDelThenExit());
        Assert.True(WalkSearchCore.HasCommentedAttackStateBlock());
    }

    [Fact]
    public void ExemptTruthTable()
    {
        Assert.True(WalkSearchCore.ExemptTruthTable());
        Assert.True(WalkSearchCore.NpcIntervalIsClosed());
        Assert.True(WalkSearchCore.FourExemptionComments());
        Assert.Equal(4, WalkSearchCore.ExemptionComments.Length);
    }

    [Fact]
    public void ArcherGuardNoLongerExempt()
    {
        // **普通弓箭手那行豁免被注释掉了**
        Assert.True(WalkSearchCore.ArcherGuardNoLongerExempt());
        Assert.True(WalkSearchCore.MoveArcherGuardStillExempt());
        Assert.True(WalkSearchCore.CommentedExemptionLine());
    }

    [Fact]
    public void RetaliateTruthTable()
    {
        // **两段逐字相同的四分支判定**
        Assert.True(WalkSearchCore.RetaliateTruthTable());
        Assert.True(WalkSearchCore.RetaliateDuplicatedTwice());
        Assert.True(WalkSearchCore.RetaliateCopyCountIsTwo());
        Assert.True(WalkSearchCore.TwoMasterLevels());
        Assert.True(WalkSearchCore.GrandMasterIsSeparateField());
        Assert.True(WalkSearchCore.TwoTriggersDiffer());
        Assert.Equal(2, WalkSearchCore.RetaliateCopyCount());
    }

    [Fact]
    public void RetaliateValues()
    {
        // 打我 → 还手
        Assert.True(WalkSearchCore.ShouldRetaliate(true, false, false, false, false, false, true, false, 0));

        // 打主人 → 还手
        Assert.True(WalkSearchCore.ShouldRetaliate(false, true, true, false, false, false, true, false, 0));

        // 打主人的主人 → 还手
        Assert.True(WalkSearchCore.ShouldRetaliate(false, false, false, false, true, true, true, false, 0));

        // 他主人是玩家且我没主人 → 还手
        Assert.True(WalkSearchCore.ShouldRetaliate(false, false, false, false, false, false, true, true, 0));

        // 都没打 → 不还手
        Assert.False(WalkSearchCore.ShouldRetaliate(false, false, false, false, false, false, true, false, 0));

        // 他主人不是玩家 → 不还手
        Assert.False(WalkSearchCore.ShouldRetaliate(false, false, false, false, false, false, true, true, 80));
    }

    [Fact]
    public void PetHoldTruthTable()
    {
        // **"主人不攻击，别人只攻击宝宝时，宝宝不动"**
        Assert.True(WalkSearchCore.PetHoldTruthTable());
        Assert.True(WalkSearchCore.PetHoldCommentHasDate());
        Assert.Contains("2017-06-30", WalkSearchCore.PetHoldComment);
    }

    [Fact]
    public void ProperTargetTruthTable()
    {
        Assert.True(WalkSearchCore.ProperTargetTruthTable());
        Assert.True(WalkSearchCore.DummyExemptionPlayerOnly());
        Assert.True(WalkSearchCore.DummyHideCommentHasDate());
        Assert.Contains("2014-09-01", WalkSearchCore.DummyHideComment);
    }

    [Fact]
    public void OffLineTruthTable()
    {
        // **怪物不攻击脱机人物**
        Assert.True(WalkSearchCore.OffLineTruthTable());
        Assert.True(WalkSearchCore.OffLineCommentHasDate());
        Assert.Contains("2015-09-07", WalkSearchCore.OffLineComment);
    }

    [Fact]
    public void ManhattanValues()
    {
        Assert.True(WalkSearchCore.ManhattanValues());
        Assert.True(WalkSearchCore.NotChebyshevNotEuclid());
        Assert.Equal(7, WalkSearchCore.Distance(0, 0, 3, 4));
        Assert.Equal(0, WalkSearchCore.Distance(5, 5, 5, 5));
    }

    [Fact]
    public void CloserBoundary()
    {
        // **严格小于（取最近）**
        Assert.True(WalkSearchCore.CloserBoundary());
        Assert.False(WalkSearchCore.IsCloser(5, 5));
        Assert.True(WalkSearchCore.IsCloser(4, 5));
    }

    [Fact]
    public void DistanceInitMagic()
    {
        Assert.True(WalkSearchCore.DistanceInitIs999());
        Assert.True(WalkSearchCore.SharedDistanceInit());
        Assert.True(WalkSearchCore.DistanceAtLeastInitNeverSelected());
    }

    // ---- 选主之后的处理 ----

    [Fact]
    public void TauntTruthTable()
    {
        // **嘲讽目标三重清理：幽灵/死亡、换地图、超距**
        Assert.True(WalkSearchCore.TauntTruthTable());
        Assert.True(WalkSearchCore.TauntToleranceStrict());
        Assert.True(WalkSearchCore.TauntAxesUseOr());
        Assert.True(WalkSearchCore.TauntClearsOnMapChange());
    }

    [Fact]
    public void TauntValues()
    {
        Assert.True(WalkSearchCore.TauntCleared(true, false, true, 0, 0));
        Assert.True(WalkSearchCore.TauntCleared(false, false, false, 0, 0));
        Assert.True(WalkSearchCore.TauntCleared(false, false, true, 21, 0));
        Assert.False(WalkSearchCore.TauntCleared(false, false, true, 20, 20));
    }

    [Fact]
    public void SafeAreaTruthTable()
    {
        // **人物回安全区了，宝宝不攻击该人物**
        Assert.True(WalkSearchCore.SafeAreaTruthTable());
        Assert.True(WalkSearchCore.SafeAreaCommentHasDate());
        Assert.Contains("2017-06-26", WalkSearchCore.SafeAreaComment);
    }

    [Fact]
    public void PracticeFallbackTruthTable()
    {
        // **修复宝宝不攻击练功师**
        Assert.True(WalkSearchCore.PracticeFallbackTruthTable());
        Assert.True(WalkSearchCore.PracticeCommentHasDate());
        Assert.Contains("2014-10-25", WalkSearchCore.PracticeComment);
    }

    [Fact]
    public void DecisionTruthTable()
    {
        Assert.True(WalkSearchCore.DecisionTruthTable());
        Assert.True(WalkSearchCore.SafeAreaBeatsDirectSet());
        Assert.Equal("set", WalkSearchCore.TargetDecision(true, false, false, false));
        Assert.Equal("del", WalkSearchCore.TargetDecision(true, true, false, false));
        Assert.Equal("masterTarget", WalkSearchCore.TargetDecision(false, false, true, false));
        Assert.Equal("taunt", WalkSearchCore.TargetDecision(false, false, false, true));
        Assert.Equal("none", WalkSearchCore.TargetDecision(false, false, false, false));
    }

    [Fact]
    public void TauntFallbackWritesFieldDirectly()
    {
        // **嘲讽兜底直接写 m_TargetCret，不走 SetTargetCreat**
        Assert.True(WalkSearchCore.TauntFallbackWritesFieldDirectly());
    }

    [Fact]
    public void CommentedFixes()
    {
        Assert.True(WalkSearchCore.CommentedPlayMosterFixHasDate());
        Assert.True(WalkSearchCore.HasCommentedSetTargetCreat());
        Assert.Contains("RC_PLAYMOSTER", WalkSearchCore.CommentedPlayMosterFix);
    }

    [Fact]
    public void LockingSemantics()
    {
        Assert.True(WalkSearchCore.LocksActorList());
        Assert.True(WalkSearchCore.UsesTryFinallyUnlock());
        Assert.True(WalkSearchCore.IteratesVisibleActors());
    }

    // ===================== 三、sub_4C959C =====================

    [Fact]
    public void SubFilterTruthTable()
    {
        Assert.True(WalkSearchCore.SubFilterTruthTable());
        Assert.True(WalkSearchCore.SubSkipsAllExemptions());
        Assert.True(WalkSearchCore.SubHasNoRetaliateCheck());
        Assert.True(WalkSearchCore.SubHasNoTaunt());
        Assert.True(WalkSearchCore.SubHasNoPetGate());
    }

    [Fact]
    public void SubIsSimplifiedVariant()
    {
        Assert.True(WalkSearchCore.SubIsSimplifiedVariant());
        Assert.True(WalkSearchCore.SameDistanceMetricAndInit());
        Assert.True(WalkSearchCore.SubLocksActorList());
        Assert.True(WalkSearchCore.BothUseSetTargetCreat());
        Assert.True(WalkSearchCore.FiveSharedTraits());
        Assert.Equal(5, WalkSearchCore.SharedTraits.Length);
    }

    [Fact]
    public void SubNameIsAddress()
    {
        // **方法名是反汇编地址**
        Assert.True(WalkSearchCore.SubNameIsAddress());
        Assert.True(WalkSearchCore.SubMethodNameFormat());
        Assert.Equal("sub_4C959C", WalkSearchCore.SubMethodName);
    }

    [Fact]
    public void SubRangeValues()
    {
        Assert.True(WalkSearchCore.SubRangeValues());
        Assert.Equal((40115, 40152), WalkSearchCore.SubRange());
    }

    // ===================== 顶层仿真 =====================

    [Fact]
    public void SelectNearestPicksClosest()
    {
        Assert.True(WalkSearchCore.SelectNearestPicksClosest());
    }

    [Fact]
    public void SelectNearestNoneAccepted()
    {
        Assert.True(WalkSearchCore.SelectNearestNoneAccepted());
    }

    [Fact]
    public void SelectNearestTieKeepsFirst()
    {
        // **并列时保留先遇到的（严格小于）**
        Assert.True(WalkSearchCore.SelectNearestTieKeepsFirst());
    }

    [Fact]
    public void SimulateWalkSuccess()
    {
        Assert.True(WalkSearchCore.SimulateWalkSuccess());
    }

    [Fact]
    public void SimulateWalkDingShen()
    {
        // **定身时坐标不动，但方向仍被写入**
        Assert.True(WalkSearchCore.SimulateWalkDingShen());
        Assert.True(WalkSearchCore.SimulateWalkDirectionPersists());
    }

    [Fact]
    public void SimulateWalkBlocked()
    {
        Assert.True(WalkSearchCore.SimulateWalkBlocked());
    }
}
