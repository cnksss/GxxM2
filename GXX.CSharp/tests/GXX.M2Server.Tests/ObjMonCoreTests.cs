using System;
using GXX.M2Server;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 批次J197：`ObjMon.pas` 中 `TMonster` 核心行为 1:1 测试（三方法合计 91 行）。
/// **两处关键陷阱**：① `TMonster.Run` 在单元里出现两次，
/// 第一次（934-1118）整段处于 `(* ... *)` 块注释中；
/// ② `tick_diff(旧, 新)` 返回 `新 - 旧` 且是无符号、带回绕补偿。
/// </summary>
public sealed class ObjMonCoreTests
{
    // ===================== 常量 =====================

    [Fact]
    public void Constants()
    {
        Assert.Equal(840, ObjMonCore.OperateStart);
        Assert.Equal(4, ObjMonCore.OperateLines);
        Assert.Equal(845, ObjMonCore.ThinkStart);
        Assert.Equal(42, ObjMonCore.ThinkLines);
        Assert.Equal(888, ObjMonCore.AttackStart);
        Assert.Equal(45, ObjMonCore.AttackLines);
        Assert.Equal(91, ObjMonCore.TotalLines);
        Assert.Equal(3000, ObjMonCore.ThinkThrottleMs);
        Assert.Equal(3, ObjMonCore.ThinkThrottleFactor);
        Assert.Equal(2, ObjMonCore.ObjCountThreshold);
        Assert.Equal(1, ObjMonCore.NpcCountThreshold);
        Assert.Equal(8, ObjMonCore.RandomDirectionBound);
        Assert.Equal(622, ObjMonCore.SpecialAppr);
        Assert.Equal(10, ObjMonCore.PoisonChanceDenominator);
        Assert.Equal(3, ObjMonCore.PoisonDurationBound);
        Assert.Equal(2, ObjMonCore.PoisonDurationBase);
        Assert.Equal(2, ObjMonCore.PoisonDurationMin);
        Assert.Equal(4, ObjMonCore.PoisonDurationMax);
        Assert.Equal(5, ObjMonCore.POISON_STONE);
        Assert.Equal(0, ObjMonCore.RC_PLAYOBJECT);
        Assert.Equal(1, ObjMonCore.RC_HEROOBJECT);
        Assert.Equal(80, ObjMonCore.RC_MONSTER);
        Assert.Equal(10, ObjMonCore.RC_NPC);
        Assert.Equal(933, ObjMonCore.CommentOpen);
        Assert.Equal(1119, ObjMonCore.CommentClose);
        Assert.Equal(1121, ObjMonCore.LiveRunStart);
        Assert.Equal(934, ObjMonCore.OldRunStart);
        Assert.Equal(9502, ObjMonCore.UnitLines);
        Assert.Equal(54, ObjMonCore.DerivedClassCount);
        Assert.Equal(7, ObjMonCore.TMonsterMethodCount);
    }

    [Fact]
    public void SpanAndOrder()
    {
        Assert.True(ObjMonCore.SpanMatches());
        Assert.True(ObjMonCore.TotalLinesAddUp());
        Assert.True(ObjMonCore.StartsAscending());
        Assert.True(ObjMonCore.SevenMethods());
        Assert.True(ObjMonCore.MethodsOrdered());
        Assert.True(ObjMonCore.BatchMethodsInList());
        Assert.True(ObjMonCore.WithinUnit());
        Assert.True(ObjMonCore.NoInstrumentation());
    }

    [Fact]
    public void MethodList()
    {
        Assert.Equal(7, ObjMonCore.Methods.Length);
        Assert.Equal("Create", ObjMonCore.Methods[0].Name);
        Assert.Equal(788, ObjMonCore.Methods[0].Line);
        Assert.Equal("AttackTarget", ObjMonCore.Methods[5].Name);
        Assert.Equal("Run", ObjMonCore.Methods[6].Name);
        Assert.Equal(1121, ObjMonCore.Methods[6].Line);
    }

    // ===================== 一、Run 双份结构 =====================

    [Fact]
    public void RunAppearsTwice()
    {
        Assert.True(ObjMonCore.RunAppearsTwice());
        Assert.True(ObjMonCore.OldRunCommentedOut());
        Assert.True(ObjMonCore.CommentOpens933());
        Assert.True(ObjMonCore.CommentCloses1119());
        Assert.True(ObjMonCore.LiveRunAt1121());
        Assert.True(ObjMonCore.LiveRunIsSecond());
        Assert.True(ObjMonCore.OldRunInsideComment());
        Assert.True(ObjMonCore.LiveRunOutsideComment());
        Assert.True(ObjMonCore.NaiveSearchHitsComment());
    }

    [Fact]
    public void CommentRegion()
    {
        // **注释区 933..1119、旧 Run 在 934、生效 Run 在 1121**
        Assert.True(ObjMonCore.CommentOpen < ObjMonCore.OldRunStart);
        Assert.True(ObjMonCore.OldRunStart < ObjMonCore.CommentClose);
        Assert.True(ObjMonCore.LiveRunStart > ObjMonCore.CommentClose);
    }

    [Fact]
    public void NewRunAdditions()
    {
        Assert.True(ObjMonCore.SharedPrefix());
        Assert.True(ObjMonCore.SameGuardOrder());
        Assert.True(ObjMonCore.WalkWaitLockIsNew());
        Assert.True(ObjMonCore.IsCanMoveIsNew());
        Assert.True(ObjMonCore.PetPickupIsNew());
        Assert.True(ObjMonCore.SmartObjectIsNew());
    }

    // ===================== 二、Think =====================

    [Fact]
    public void ThinkFacts()
    {
        Assert.True(ObjMonCore.ThinkThrottleThreeSeconds());
        Assert.True(ObjMonCore.ThrottleWrittenAsProduct());
        Assert.True(ObjMonCore.ThrottleRefreshImmediate());
        Assert.True(ObjMonCore.HasCommentedCandidate());
        Assert.True(ObjMonCore.ElseInsideComment());
        Assert.True(ObjMonCore.LiveIsProperTargetCheck());
        Assert.True(ObjMonCore.ThreePartCondition());
        Assert.True(ObjMonCore.ElseIfSecondBranch());
        Assert.True(ObjMonCore.TwoApisTwoThresholds());
        Assert.True(ObjMonCore.ObjCountThreshold2());
        Assert.True(ObjMonCore.NpcCountThreshold1());
        Assert.True(ObjMonCore.SetOnlyInThink());
        Assert.True(ObjMonCore.ClearedOnMoveSuccess());
        Assert.True(ObjMonCore.RandomDirection0To7());
        Assert.True(ObjMonCore.NotRun());
        Assert.True(ObjMonCore.RequiresPositionChange());
        Assert.True(ObjMonCore.CrossCallStateMachine());
    }

    [Fact]
    public void ThinkConditions()
    {
        Assert.Equal(3, ObjMonCore.DupConditions.Length);
        Assert.Equal("MasterNilOrNotSafeZone", ObjMonCore.DupConditions[0]);
        Assert.Equal("IsElseIf", ObjMonCore.DupConditions[1]);
        Assert.Equal("NpcCountOverOne", ObjMonCore.DupConditions[2]);
    }

    [Fact]
    public void ThinkThrottleBoundaries()
    {
        // **刚思考过：不再思考**
        Assert.False(ObjMonCore.ShouldThink(1000, 1000));
        Assert.True(ObjMonCore.ThrottledRightAfter());

        // **差一秒：仍节流**
        Assert.True(ObjMonCore.ThrottledAtOneSecond());

        // **恰好三秒：仍节流（严格大于）**
        Assert.False(ObjMonCore.ShouldThink(4000, 1000));
        Assert.True(ObjMonCore.ThrottledExactlyAtThreeSeconds());

        // **超过三秒：通过**
        Assert.True(ObjMonCore.ShouldThink(4001, 1000));
        Assert.True(ObjMonCore.PassesAfterThreeSeconds());
    }

    [Fact]
    public void DupModeDecision()
    {
        Assert.True(ObjMonCore.NoMasterUsesObjCount());
        Assert.True(ObjMonCore.NotSafeZoneUsesObjCount());
        Assert.True(ObjMonCore.MasterNotPlayerUsesObjCount());
        Assert.True(ObjMonCore.SafeZonePlayerMasterUsesNpcCount());
        Assert.True(ObjMonCore.NpcOneIsEnough());
        Assert.True(ObjMonCore.ObjOneNotEnough());

        // **阈值确实不同：对象数要 2、NPC 数只要 1**
        Assert.True(ObjMonCore.NeedsDupMode(false, true, ObjMonCore.RC_PLAYOBJECT, 2, 0));
        Assert.False(ObjMonCore.NeedsDupMode(false, true, ObjMonCore.RC_PLAYOBJECT, 1, 0));

        // **安全区且主人是玩家：走 NPC 分支**
        Assert.True(ObjMonCore.NeedsDupMode(true, true, ObjMonCore.RC_PLAYOBJECT, 0, 1));
        Assert.False(ObjMonCore.NeedsDupMode(true, true, ObjMonCore.RC_PLAYOBJECT, 0, 0));
    }

    [Fact]
    public void RandomDirectionValidity()
    {
        for (int d = 0; d < ObjMonCore.RandomDirectionBound; d++)
        {
            Assert.True(ObjMonCore.ValidRandomDirection(d));
        }

        Assert.False(ObjMonCore.ValidRandomDirection(8));
        Assert.False(ObjMonCore.ValidRandomDirection(-1));
    }

    // ===================== 三、AttackTarget =====================

    [Fact]
    public void AttackFacts()
    {
        Assert.True(ObjMonCore.TripleGuard());
        Assert.True(ObjMonCore.TargetNotNil());
        Assert.True(ObjMonCore.TargetNotDead());
        Assert.True(ObjMonCore.TargetNotGhost());
        Assert.True(ObjMonCore.ZeroPowerGuard());
        Assert.True(ObjMonCore.RequiresMaster());
        Assert.True(ObjMonCore.OnlyPlayerObject());
        Assert.True(ObjMonCore.DeletesTargetNotJustSkips());
        Assert.True(ObjMonCore.HeroNotCovered());
        Assert.True(ObjMonCore.HitThrottleSum());
        Assert.True(ObjMonCore.DelayResetOnHit());
        Assert.True(ObjMonCore.TwoTicksRefreshed());
        Assert.True(ObjMonCore.DirOutParam());
        Assert.True(ObjMonCore.DirReusedForAttack());
        Assert.True(ObjMonCore.FalseSkipsAttack());
        Assert.True(ObjMonCore.BreakHolySeizeUnconditional());
        Assert.True(ObjMonCore.TrueEvenIfThrottled());
        Assert.True(ObjMonCore.OutsideThrottleGuard());
        Assert.True(ObjMonCore.MeansInRangeNotActuallyHit());
        Assert.True(ObjMonCore.ElseSetsTargetXY());
        Assert.True(ObjMonCore.SameMapWalkToward());
        Assert.True(ObjMonCore.DifferentMapDropTarget());
        Assert.True(ObjMonCore.DifferentGuardHelpers());
        Assert.True(ObjMonCore.NotInterchangeable());
    }

    [Fact]
    public void RaceConstants()
    {
        Assert.True(ObjMonCore.PlayerObjectIsZero());
        Assert.True(ObjMonCore.HeroObjectIsOne());
        Assert.True(ObjMonCore.AdjacentButDistinct());
        Assert.Equal(0, ObjMonCore.RC_PLAYOBJECT);
        Assert.Equal(1, ObjMonCore.RC_HEROOBJECT);
    }

    [Fact]
    public void GuardTruthTable()
    {
        Assert.True(ObjMonCore.AllThreePass());
        Assert.True(ObjMonCore.NilTargetRejected());
        Assert.True(ObjMonCore.DeadTargetRejected());
        Assert.True(ObjMonCore.GhostTargetRejected());

        Assert.True(ObjMonCore.CanAttackTarget(false, false, false));
        Assert.False(ObjMonCore.CanAttackTarget(true, false, false));
        Assert.False(ObjMonCore.CanAttackTarget(false, true, false));
        Assert.False(ObjMonCore.CanAttackTarget(false, false, true));
    }

    [Fact]
    public void ZeroPowerInterception()
    {
        Assert.True(ObjMonCore.ZeroPowerPlayerDropped());
        Assert.True(ObjMonCore.NonZeroPowerKept());
        Assert.True(ObjMonCore.HeroTargetKept());
        Assert.True(ObjMonCore.NoMasterKept());

        // **只对玩家（0）生效、对英雄（1）不生效**
        Assert.True(ObjMonCore.ShouldDropTarget(0, true, ObjMonCore.RC_PLAYOBJECT));
        Assert.False(ObjMonCore.ShouldDropTarget(0, true, ObjMonCore.RC_HEROOBJECT));
    }

    [Fact]
    public void PoisonFacts()
    {
        Assert.True(ObjMonCore.Appr622Special());
        Assert.True(ObjMonCore.OneInTenChance());
        Assert.True(ObjMonCore.PoisonDurationTwoToFour());
        Assert.True(ObjMonCore.PoisonStoneType());
        Assert.True(ObjMonCore.RandomThreePlusTwo());
        Assert.True(ObjMonCore.DurationBoundYieldsFour());
        Assert.True(ObjMonCore.PoisonDurationsInRange());
        Assert.True(ObjMonCore.PoisonDurationBoundaries());
    }

    [Fact]
    public void PoisonDurations()
    {
        // **Random(3) 取 0/1/2 → 时长 2/3/4**
        Assert.Equal(2, ObjMonCore.PoisonDuration(0));
        Assert.Equal(3, ObjMonCore.PoisonDuration(1));
        Assert.Equal(4, ObjMonCore.PoisonDuration(2));

        // **类型是 POISON_STONE = 5**
        Assert.Equal(5, ObjMonCore.POISON_STONE);
    }

    // ===================== tick_diff =====================

    [Fact]
    public void TickDiffSemantics()
    {
        Assert.True(ObjMonCore.TickDiffReturnsNowMinusLast());
        Assert.True(ObjMonCore.TickDiffUnsigned());
        Assert.True(ObjMonCore.TickDiffHandlesWraparound());
        Assert.True(ObjMonCore.SignedMisreadingInverts());

        // **`新 - 旧`、不是 `旧 - 新`**
        Assert.Equal(1000u, ObjMonCore.TickDiff(1000, 2000));
        Assert.Equal(0u, ObjMonCore.TickDiff(2000, 2000));

        // **回绕补偿**
        Assert.Equal(10u, ObjMonCore.TickDiff(uint.MaxValue - 5, 5));
    }

    [Fact]
    public void HitThrottleBoundaries()
    {
        Assert.True(ObjMonCore.HitThrottledWhenCooling());
        Assert.True(ObjMonCore.HitAllowedAfterCooldown());
        Assert.True(ObjMonCore.HitThrottledExactlyAtThreshold());
        Assert.True(ObjMonCore.ExtraDelayPostpones());

        // **同一时刻：差 0、不能打**
        Assert.False(ObjMonCore.CanHit(1000, 1000, 500, 0));

        // **冷却已过：可以打**
        Assert.True(ObjMonCore.CanHit(1000, 2000, 500, 0));

        // **恰好 500：严格大于、仍不能打**
        Assert.False(ObjMonCore.CanHit(1000, 1500, 500, 0));

        // **501：可以打**
        Assert.True(ObjMonCore.CanHit(1000, 1501, 500, 0));
    }

    // ===================== 四、整体 =====================

    [Fact]
    public void OverallFacts()
    {
        Assert.True(ObjMonCore.OperateIsPureForward());
        Assert.True(ObjMonCore.ExplicitInherited());
        Assert.True(ObjMonCore.RedundantButHarmless());
        Assert.True(ObjMonCore.BaseClassHome());
        Assert.True(ObjMonCore.ManyDerivedClasses());
        Assert.True(ObjMonCore.HumMonCopyMonElsewhere());
    }
}
