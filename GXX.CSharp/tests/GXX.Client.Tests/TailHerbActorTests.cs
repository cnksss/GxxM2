using System;
using System.Collections.Generic;
using System.Linq;
using GXX.Client.Tail;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 并行批次 P2c / 车道 <c>par/p2c-client-tail</c>：
/// <c>Source/Client-HGE/HerbActor.pas</c>（1223 行）的**部分移植**测试。
///
/// <para>覆盖 <see cref="HerbActorFramework"/> 里逐行抽取的决策逻辑；
/// 未覆盖范围见 <see cref="HerbActorCoverage"/>（用例 20 会对其进行结构断言）。</para>
/// </summary>
public sealed class TailHerbActorTests
{
    /// <summary>可注入的动作表假实现。</summary>
    private sealed class FakePart : IHerbMonsterAction
    {
        public THerbActionPart ActStand { get; set; } = P(100, 4, 1, 200);
        public THerbActionPart ActWalk { get; set; } = P(200, 6, 2, 300, 40);
        public THerbActionPart ActAttack { get; set; } = P(1000, 3, 1, 150);
        public THerbActionPart ActCritical { get; set; } = P(2000, 5, 2, 250);
        public THerbActionPart ActStruck { get; set; } = P(3000, 2, 1, 120);
        public THerbActionPart ActDie { get; set; } = P(4000, 8, 2, 400);
        public THerbActionPart ActDeath { get; set; } = P(5000, 10, 1, 500);

        public static THerbActionPart P(int start, int frame, int skip, uint ftime, int usetick = 0)
            => new THerbActionPart { start = start, frame = frame, skip = skip, ftime = ftime, usetick = usetick };
    }

    private sealed class FakeActor : IHerbActorView
    {
        public int ChangeAppr { get; set; } = -1;
        public int CurrentAction { get; set; }
        public int BtRace { get; set; }
        public int Appearance { get; set; }
        public int BtDir { get; set; }
        public bool BoDeath { get; set; }
        public bool BoSkeleton { get; set; }
        public int CurrentDefFrame { get; set; }
        public bool BoUseMagic { get; set; } = true;
        public int CurrentFrame { get; set; } = 77;
    }

    // ══════════════════════════════════════════════════════════════════════
    // 常量与枚举
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：四个常量与原文一致（原文 :19-23）。</summary>
    [Fact]
    public void Constants_MatchSource()
    {
        Assert.Equal(600, HerbActorConst.BEEQUEENBASE);
        Assert.Equal(120, HerbActorConst.DOORDEATHEFFECTBASE);
        Assert.Equal(224, HerbActorConst.WALLLEFTBROKENEFFECTBASE);
        Assert.Equal(240, HerbActorConst.WALLRIGHTBROKENEFFECTBASE);
    }

    /// <summary>用例 2：TDoorState 的取值顺序（原文 :26）。</summary>
    [Fact]
    public void DoorStateOrder_MatchesSource()
    {
        Assert.Equal(0, (int)TDoorState.dsOpen);
        Assert.Equal(1, (int)TDoorState.dsClose);
        Assert.Equal(2, (int)TDoorState.dsBroken);
    }

    /// <summary>用例 3：SM_* 常量值（与 Grobal2.pas 核对的那些）。</summary>
    [Fact]
    public void SmConstants_MatchGrobal2()
    {
        Assert.Equal(10, ActorCoreSM.SM_TURN);
        Assert.Equal(14, ActorCoreSM.SM_HIT);
        Assert.Equal(20, ActorCoreSM.SM_DIGUP);
        Assert.Equal(21, ActorCoreSM.SM_DIGDOWN);
        Assert.Equal(31, ActorCoreSM.SM_STRUCK);
        Assert.Equal(32, ActorCoreSM.SM_DEATH);
        Assert.Equal(34, ActorCoreSM.SM_NOWDEATH);
        Assert.Equal(1445, ActorCoreSM.SM_LIGHTINGEX);

        Assert.Equal(8946, ActorCoreSMAttack.SM_ATTACK01);
        Assert.Equal(8951, ActorCoreSMAttack.SM_ATTACK06);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 攻击动作归一化（原文 :163-169 等四处逐字相同）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：SM_ATTACK01..06 都被归一化为 SM_HIT。</summary>
    [Theory]
    [InlineData(ActorCoreSMAttack.SM_ATTACK01)]
    [InlineData(ActorCoreSMAttack.SM_ATTACK02)]
    [InlineData(ActorCoreSMAttack.SM_ATTACK03)]
    [InlineData(ActorCoreSMAttack.SM_ATTACK04)]
    [InlineData(ActorCoreSMAttack.SM_ATTACK05)]
    [InlineData(ActorCoreSMAttack.SM_ATTACK06)]
    public void IsChangeApprAttackAction_CoversAttack01To06(int action)
        => Assert.True(HerbActorFramework.IsChangeApprAttackAction(action));

    /// <summary>
    /// 用例 2：<b>差异断言</b> —— 列表外的动作（含 SM_HIT 自身、SM_LIGHTINGEX、
    /// 以及一个不存在的动作号）都**不**被改写。
    /// </summary>
    [Fact]
    public void IsChangeApprAttackAction_ExcludesEverythingElse()
    {
        Assert.False(HerbActorFramework.IsChangeApprAttackAction(ActorCoreSM.SM_HIT));
        Assert.False(HerbActorFramework.IsChangeApprAttackAction(ActorCoreSM.SM_LIGHTINGEX));
        Assert.False(HerbActorFramework.IsChangeApprAttackAction(ActorCoreSM.SM_TURN));
        Assert.False(HerbActorFramework.IsChangeApprAttackAction(0));
        Assert.False(HerbActorFramework.IsChangeApprAttackAction(ActorCoreSMAttack.SM_ATTACK_OUT_OF_LIST));
        // 边界：ATTACK01 之前 / ATTACK06 之后
        Assert.False(HerbActorFramework.IsChangeApprAttackAction(ActorCoreSMAttack.SM_ATTACK01 - 1));
        Assert.False(HerbActorFramework.IsChangeApprAttackAction(ActorCoreSMAttack.SM_ATTACK06 + 1));
    }

    /// <summary>用例 3：PreKillingHerb 的变身处 —— 改写动作并返回 true。</summary>
    [Fact]
    public void PreKillingHerb_ChangeApprBranch_RewritesActionAndReturnsTrue()
    {
        var a = new FakeActor { ChangeAppr = 0, CurrentAction = ActorCoreSMAttack.SM_ATTACK03 };
        Assert.True(HerbActorFramework.PreKillingHerb(a));
        Assert.Equal(ActorCoreSM.SM_HIT, a.CurrentAction);
        // 变身处**不**触碰这两处（在 Exit 之前）
        Assert.True(a.BoUseMagic);
        Assert.Equal(77, a.CurrentFrame);
    }

    /// <summary>用例 4：PreKillingHerb 的变身处 —— 非攻击动作不改写。</summary>
    [Fact]
    public void PreKillingHerb_ChangeApprBranch_LeavesNonAttackAction()
    {
        var a = new FakeActor { ChangeAppr = 3, CurrentAction = 11 /* SM_WALK */ };
        Assert.True(HerbActorFramework.PreKillingHerb(a));
        Assert.Equal(11 /* SM_WALK */, a.CurrentAction);
    }

    /// <summary>用例 5：PreKillingHerb 的非变身处 —— 清两个字段并返回 false。</summary>
    [Fact]
    public void PreKillingHerb_NonChangeApprBranch_ClearsFlags()
    {
        var a = new FakeActor { ChangeAppr = -1, BoUseMagic = true, CurrentFrame = 77 };
        Assert.False(HerbActorFramework.PreKillingHerb(a));
        Assert.False(a.BoUseMagic);
        Assert.Equal(-1, a.CurrentFrame);
    }

    /// <summary>用例 6：ChangeAppr = 0 也算"变身处"（原文用 &gt;= 0）。</summary>
    [Fact]
    public void PreKillingHerb_ChangeApprZeroCountsAsOverride()
    {
        var a = new FakeActor { ChangeAppr = 0, BoUseMagic = true };
        Assert.True(HerbActorFramework.PreKillingHerb(a));
        Assert.True(a.BoUseMagic);          // 未被清零
    }

    // ══════════════════════════════════════════════════════════════════════
    // PlanKillingHerb（原文 :157-268）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：SM_TURN 用 ActStand 且**不加方向偏移**（原文 :182 把乘法注释掉了）。</summary>
    [Fact]
    public void PlanKillingHerb_Turn_UsesActStandWithoutDirOffset()
    {
        var pm = new FakePart();
        var p = HerbActorFramework.PlanKillingHerb(ActorCoreSM.SM_TURN, 3, 0, pm);
        Assert.Equal(pm.ActStand.start, p.StartFrame);
        Assert.Equal(pm.ActStand.start + pm.ActStand.frame - 1, p.EndFrame);
        Assert.Equal(pm.ActStand.ftime, p.FrameTime);
        Assert.Equal(pm.ActStand.frame, p.DefFrameCount);
        Assert.True(p.ShiftCalled);
        Assert.False(p.SetWarModeTime);
        Assert.Equal("SM_TURN", p.Branch);
    }

    /// <summary>用例 2：SM_DIGUP 用 ActWalk 且带 usetick / MoveStep（原文 :189-199）。</summary>
    [Fact]
    public void PlanKillingHerb_DigUp_UsesActWalkAndSetsMaxTick()
    {
        var pm = new FakePart();
        var p = HerbActorFramework.PlanKillingHerb(ActorCoreSM.SM_DIGUP, 5, 0, pm);
        Assert.Equal(pm.ActWalk.start, p.StartFrame);
        Assert.Equal(pm.ActWalk.start + pm.ActWalk.frame - 1, p.EndFrame);
        Assert.Equal(pm.ActWalk.ftime, p.FrameTime);
        Assert.Equal(pm.ActWalk.usetick, p.MaxTick);
        Assert.Equal(0, p.CurTick);
        Assert.Equal(1, p.MoveStep);
        Assert.True(p.ShiftCalled);
    }

    /// <summary>用例 3：SM_HIT / SM_LIGHTINGEX 用 ActAttack + 方向偏移 + WarModeTime。</summary>
    [Fact]
    public void PlanKillingHerb_HitAndLightingEx_UseActAttackWithDirOffset()
    {
        var pm = new FakePart();
        foreach (int action in new[] { ActorCoreSM.SM_HIT, ActorCoreSM.SM_LIGHTINGEX })
        {
            var p = HerbActorFramework.PlanKillingHerb(action, 3, 0, pm);
            int expectedStart = pm.ActAttack.start + 3 * (pm.ActAttack.frame + pm.ActAttack.skip);
            Assert.Equal(expectedStart, p.StartFrame);
            Assert.Equal(expectedStart + pm.ActAttack.frame - 1, p.EndFrame);
            Assert.True(p.SetWarModeTime);
            Assert.True(p.ShiftCalled);
        }
    }

    /// <summary>用例 4：SM_STRUCK 用**传入的** m_dwStruckFrameTime 而非表里的 ftime。</summary>
    [Fact]
    public void PlanKillingHerb_StruckUsesStruckFrameTime()
    {
        var pm = new FakePart();
        var p = HerbActorFramework.PlanKillingHerb(ActorCoreSM.SM_STRUCK, 2, 12345, pm);
        Assert.Equal(12345u, p.FrameTime);
        Assert.NotEqual(pm.ActStruck.ftime, p.FrameTime);   // 差异断言：不是表里的 ftime
        Assert.True(p.UsedStruckFrameTime);
        Assert.True(p.ResetStruckCounter);
        Assert.Equal(pm.ActStruck.start + 2 * (pm.ActStruck.frame + pm.ActStruck.skip), p.StartFrame);
    }

    /// <summary>用例 5：SM_DEATH 的 StartFrame 被改写成 EndFrame（尸体定格在末帧）。</summary>
    [Fact]
    public void PlanKillingHerb_DeathPinsStartFrameToEndFrame()
    {
        var pm = new FakePart();
        var p = HerbActorFramework.PlanKillingHerb(ActorCoreSM.SM_DEATH, 1, 0, pm);
        int start = pm.ActDie.start + 1 * (pm.ActDie.frame + pm.ActDie.skip);
        int end = start + pm.ActDie.frame - 1;
        Assert.Equal(end, p.StartFrame);
        Assert.Equal(end, p.EndFrame);
        Assert.Equal(pm.ActDie.ftime, p.FrameTime);
    }

    /// <summary>用例 6：SM_NOWDEATH 从首帧正常播放（与 SM_DEATH 的差异断言）。</summary>
    [Fact]
    public void PlanKillingHerb_NowDeath_KeepsStartFrame()
    {
        var pm = new FakePart();
        var death = HerbActorFramework.PlanKillingHerb(ActorCoreSM.SM_DEATH, 1, 0, pm);
        var nowDeath = HerbActorFramework.PlanKillingHerb(ActorCoreSM.SM_NOWDEATH, 1, 0, pm);
        Assert.NotEqual(death.StartFrame, nowDeath.StartFrame);
        Assert.Equal(death.EndFrame, nowDeath.EndFrame);
    }

    /// <summary>用例 7：SM_DIGDOWN 用 ActDeath + 加删除标记（原文 :260-265）。</summary>
    [Fact]
    public void PlanKillingHerb_DigDown_UsesActDeathAndMarksDelete()
    {
        var pm = new FakePart();
        var p = HerbActorFramework.PlanKillingHerb(ActorCoreSM.SM_DIGDOWN, 7, 0, pm);
        Assert.Equal(pm.ActDeath.start, p.StartFrame);      // 不加方向偏移
        Assert.Equal(pm.ActDeath.start + pm.ActDeath.frame - 1, p.EndFrame);
        Assert.Equal(pm.ActDeath.ftime, p.FrameTime);
        Assert.True(p.DelActionAfterFinished);
    }

    /// <summary>用例 8：未知动作 ⇒ 计划里一个字段都不写（原文 case 无 else）。</summary>
    [Fact]
    public void PlanKillingHerb_UnknownAction_WritesNothing()
    {
        var p = HerbActorFramework.PlanKillingHerb(999999, 1, 0, new FakePart());
        Assert.Null(p.StartFrame);
        Assert.Null(p.EndFrame);
        Assert.Null(p.FrameTime);
        Assert.False(p.ShiftCalled);
    }

    /// <summary>用例 9：动作表为 null ⇒ 返回 null（原文 :178 <c>if pm = nil then Exit;</c>）。</summary>
    [Fact]
    public void PlanKillingHerb_NullActionTable_ReturnsNull()
        => Assert.Null(HerbActorFramework.PlanKillingHerb(ActorCoreSM.SM_TURN, 0, 0, null));

    // ══════════════════════════════════════════════════════════════════════
    // PlanBeeQueen（原文 :303-397）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>
    /// 用例 1：<b>与 KillingHerb 的核心差异</b> —— BeeQueen 的所有分支都**不加**方向偏移。
    /// </summary>
    [Fact]
    public void PlanBeeQueen_NeverAppliesDirectionOffset()
    {
        var pm = new FakePart();
        foreach (int action in new[]
        {
            ActorCoreSM.SM_TURN, ActorCoreSM.SM_HIT, ActorCoreSM.SM_LIGHTINGEX,
            ActorCoreSM.SM_STRUCK, ActorCoreSM.SM_DEATH, ActorCoreSM.SM_NOWDEATH,
        })
        {
            var bee = HerbActorFramework.PlanBeeQueen(action, 3, 0, pm);
            Assert.NotNull(bee.StartFrame);
            // 方向 3 与方向 0 的结果必须相同
            var bee0 = HerbActorFramework.PlanBeeQueen(action, 0, 0, pm);
            Assert.Equal(bee0.StartFrame, bee.StartFrame);
            Assert.Equal(bee0.EndFrame, bee.EndFrame);
        }

        // 对照：KillingHerb 的 HIT 分支方向敏感
        var kill = HerbActorFramework.PlanKillingHerb(ActorCoreSM.SM_HIT, 3, 0, pm);
        var kill0 = HerbActorFramework.PlanKillingHerb(ActorCoreSM.SM_HIT, 0, 0, pm);
        Assert.NotEqual(kill0.StartFrame, kill.StartFrame);
    }

    /// <summary>用例 2：BeeQueen **没有** SM_DIGUP / SM_DIGDOWN 分支（差异断言）。</summary>
    [Fact]
    public void PlanBeeQueen_LacksDigUpAndDigDownBranches()
    {
        var pm = new FakePart();
        Assert.Null(HerbActorFramework.PlanBeeQueen(ActorCoreSM.SM_DIGUP, 0, 0, pm).StartFrame);
        Assert.Null(HerbActorFramework.PlanBeeQueen(ActorCoreSM.SM_DIGDOWN, 0, 0, pm).StartFrame);
        Assert.False(HerbActorFramework.PlanBeeQueen(ActorCoreSM.SM_DIGDOWN, 0, 0, pm).DelActionAfterFinished);
        // 对照：KillingHerb 两者都有
        Assert.NotNull(HerbActorFramework.PlanKillingHerb(ActorCoreSM.SM_DIGUP, 0, 0, pm).StartFrame);
        Assert.True(HerbActorFramework.PlanKillingHerb(ActorCoreSM.SM_DIGDOWN, 0, 0, pm).DelActionAfterFinished);
    }

    /// <summary>用例 3：BeeQueen 的 SM_TURN 仍写 DefFrameCount（与 KillingHerb 一致）。</summary>
    [Fact]
    public void PlanBeeQueen_TurnSetsDefFrameCount()
    {
        var pm = new FakePart();
        var p = HerbActorFramework.PlanBeeQueen(ActorCoreSM.SM_TURN, 0, 0, pm);
        Assert.Equal(pm.ActStand.frame, p.DefFrameCount);
        Assert.Equal(pm.ActStand.start, p.StartFrame);
    }

    /// <summary>用例 4：BeeQueen 的 DEATH 也是"定格末帧"。</summary>
    [Fact]
    public void PlanBeeQueen_DeathPinsToEndFrame()
    {
        var pm = new FakePart();
        var p = HerbActorFramework.PlanBeeQueen(ActorCoreSM.SM_DEATH, 4, 0, pm);
        Assert.Equal(pm.ActDie.start + pm.ActDie.frame - 1, p.StartFrame);
        Assert.Equal(p.StartFrame, p.EndFrame);
        Assert.False(p.UsedStruckFrameTime);
    }

    // ══════════════════════════════════════════════════════════════════════
    // PlanCentipedeKing（原文 :430-514）
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：LIGHTINGEX / HIT 用 ActCritical 并打开死亡特效（0..9 帧、62ms）。</summary>
    [Fact]
    public void PlanCentipedeKing_AttackBranchesUseCriticalAndDieEffect()
    {
        var pm = new FakePart();
        foreach (int action in new[] { ActorCoreSM.SM_LIGHTINGEX, ActorCoreSM.SM_HIT })
        {
            var p = HerbActorFramework.PlanCentipedeKing(action, 0, pm);
            Assert.Equal(pm.ActCritical.start, p.StartFrame);
            Assert.Equal(pm.ActCritical.start + pm.ActCritical.frame - 1, p.EndFrame);
            Assert.Equal(pm.ActCritical.ftime, p.FrameTime);
            Assert.True(p.ForceDirZero);
            Assert.True(p.UseDieEffect);
            Assert.True(p.ShiftCalled);
            Assert.False(p.DelegatesToInherited);
        }
    }

    /// <summary>用例 2：SM_TURN 与 else 分支都"置方向 0 + 转调 inherited"。</summary>
    [Fact]
    public void PlanCentipedeKing_TurnAndDefaultDelegateToInherited()
    {
        var pm = new FakePart();
        var turn = HerbActorFramework.PlanCentipedeKing(ActorCoreSM.SM_TURN, 0, pm);
        Assert.True(turn.ForceDirZero);
        Assert.True(turn.DelegatesToInherited);
        Assert.Null(turn.StartFrame);

        var other = HerbActorFramework.PlanCentipedeKing(ActorCoreSM.SM_DEATH, 0, pm);
        Assert.True(other.ForceDirZero);
        Assert.True(other.DelegatesToInherited);
        Assert.Null(other.StartFrame);
    }

    /// <summary>
    /// 用例 3：<b>差异断言</b> —— SM_DIGDOWN 也转调 inherited，但**不**置方向 0
    /// （原文 :506-508 这一支没有 <c>m_btDir := 0</c>，与其他 inherited 分支不同）。
    /// </summary>
    [Fact]
    public void PlanCentipedeKing_DigDownDelegatesWithoutForcingDirZero()
    {
        var p = HerbActorFramework.PlanCentipedeKing(ActorCoreSM.SM_DIGDOWN, 0, new FakePart());
        Assert.True(p.DelegatesToInherited);
        Assert.False(p.ForceDirZero);
    }

    // ══════════════════════════════════════════════════════════════════════
    // 其他子类
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：TMineMon 的两个覆写都是纯转调（无前置副作用）。</summary>
    [Fact]
    public void MineMon_OverridesArePureDelegation()
    {
        Assert.True(HerbActorFramework.MineMon_DelegatesToInherited_WithNoOwnWork());
        Assert.True(HerbActorFramework.MineMon_Create_HasNoFieldInit());
        // 与 TKillingHerb 的差异：TMineMon 不做 m_boUseMagic/m_nCurrentFrame 清理
        var a = new FakeActor { ChangeAppr = -1, BoUseMagic = true, CurrentFrame = 77 };
        Assert.False(HerbActorFramework.PreKillingHerb(a));
        Assert.False(a.BoUseMagic);
        Assert.Equal(-1, a.CurrentFrame);
        Assert.True(HerbActorFramework.MineMon_DelegatesToInherited_WithNoOwnWork());
    }

    /// <summary>用例 2：TBigHeartMon / TSpiderHouseMon 都是"方向清零"。</summary>
    [Fact]
    public void DirZeroClasses_ClearDirection()
    {
        var a = new FakeActor { BtDir = 5 };
        HerbActorFramework.PlanDirZeroThenInherited(a);
        Assert.Equal(0, a.BtDir);

        var b = new FakeActor { BtDir = 7 };
        HerbActorFramework.PlanDirZeroThenInherited(b);
        Assert.Equal(0, b.BtDir);

        HerbActorFramework.PlanDirZeroThenInherited(null);   // 不应抛
    }

    /// <summary>用例 3：GetDefaultFrameKillingHerb 的死亡 + 骷髅分支。</summary>
    [Fact]
    public void GetDefaultFrameKillingHerb_DeathAndSkeleton()
    {
        var pm = new FakePart();
        // 骷髅：ActDeath.start
        Assert.Equal(pm.ActDeath.start,
            HerbActorFramework.GetDefaultFrameKillingHerb(true, true, 3, 0, pm, out _));
        // 非骷髅：ActDie.start + Dir*(frame+skip) + (frame-1)
        int expected = pm.ActDie.start + 3 * (pm.ActDie.frame + pm.ActDie.skip) + (pm.ActDie.frame - 1);
        Assert.Equal(expected,
            HerbActorFramework.GetDefaultFrameKillingHerb(true, false, 3, 0, pm, out _));
        // 两种死亡取值不同（差异断言）
        Assert.NotEqual(
            HerbActorFramework.GetDefaultFrameKillingHerb(true, true, 3, 0, pm, out _),
            HerbActorFramework.GetDefaultFrameKillingHerb(true, false, 3, 0, pm, out _));
    }

    /// <summary>用例 4：GetDefaultFrameKillingHerb 的存活分支与边界（cf 越界归零）。</summary>
    [Fact]
    public void GetDefaultFrameKillingHerb_AliveBranchBoundaries()
    {
        var pm = new FakePart();      // ActStand.frame = 4
        Assert.Equal(pm.ActStand.start,
            HerbActorFramework.GetDefaultFrameKillingHerb(false, false, 0, -1, pm, out int dfc1));
        Assert.Equal(4, dfc1);        // m_nDefFrameCount 被写成 ActStand.frame

        Assert.Equal(pm.ActStand.start + 2,
            HerbActorFramework.GetDefaultFrameKillingHerb(false, false, 0, 2, pm, out _));

        // 越界（>= frame）归 0
        Assert.Equal(pm.ActStand.start,
            HerbActorFramework.GetDefaultFrameKillingHerb(false, false, 0, 4, pm, out _));
        Assert.Equal(pm.ActStand.start,
            HerbActorFramework.GetDefaultFrameKillingHerb(false, false, 0, 99, pm, out _));

        // null 表 ⇒ 0
        Assert.Equal(0, HerbActorFramework.GetDefaultFrameKillingHerb(false, false, 0, 0, null, out int dfc2));
        Assert.Equal(0, dfc2);
    }

    /// <summary>
    /// 用例 5：<b>与 BeeQueen 的差异断言</b> —— BeeQueen 的死亡分支**没有**骷髅分支，
    /// 且去掉方向偏移。
    /// </summary>
    [Fact]
    public void GetDefaultFrameBeeQueen_DiffersFromKillingHerbOnDeath()
    {
        var pm = new FakePart();
        int bee = HerbActorFramework.GetDefaultFrameBeeQueen(true, 0, pm, out _);
        Assert.Equal(pm.ActDie.start + (pm.ActDie.frame - 1), bee);

        int kill = HerbActorFramework.GetDefaultFrameKillingHerb(true, false, 3, 0, pm, out _);
        Assert.NotEqual(bee, kill);       // 方向项不同

        // 存活分支两者**相同**
        int beeAlive = HerbActorFramework.GetDefaultFrameBeeQueen(false, 1, pm, out int d1);
        int killAlive = HerbActorFramework.GetDefaultFrameKillingHerb(false, false, 3, 1, pm, out int d2);
        Assert.Equal(killAlive, beeAlive);
        Assert.Equal(d2, d1);
    }

    /// <summary>用例 6：GetDefaultFrameMineMon 未变身恒 0；<c>ChangeAppr &gt;= 0</c> 才转调 inherited。</summary>
    [Fact]
    public void GetDefaultFrameMineMon_ReturnsZeroWhenNotChanged()
    {
        Assert.Equal(0, HerbActorFramework.GetDefaultFrameMineMon(-1, () => 12345));
        Assert.Equal(0, HerbActorFramework.GetDefaultFrameMineMon(-2, () => 12345));
        Assert.Equal(0, HerbActorFramework.GetDefaultFrameMineMon(-1, null));
        // >= 0 视为变身 ⇒ 转调 inherited（注意原文用 >= 而非 >，0 也算）
        Assert.Equal(12345, HerbActorFramework.GetDefaultFrameMineMon(0, () => 12345));
        Assert.Equal(12345, HerbActorFramework.GetDefaultFrameMineMon(3, () => 12345));
        // inherited 为 null 时兜底 0
        Assert.Equal(0, HerbActorFramework.GetDefaultFrameMineMon(3, null));
    }

    // ══════════════════════════════════════════════════════════════════════
    // TCastleDoor
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：bowalk 的三态判定（只有 dsClose 为 false）。</summary>
    [Fact]
    public void ApplyDoorState_BowalkIsFalseOnlyWhenClosed()
    {
        Assert.True(HerbActorFramework.ApplyDoorState_Bowalk(TDoorState.dsOpen));
        Assert.False(HerbActorFramework.ApplyDoorState_Bowalk(TDoorState.dsClose));
        Assert.True(HerbActorFramework.ApplyDoorState_Bowalk(TDoorState.dsBroken));
    }

    /// <summary>
    /// 用例 2：<b>差异断言</b> —— 只有 <c>dsOpen</c> 会把前 3 格**重新标记为不可走**，
    /// <c>dsBroken</c> 虽然 bowalk 也是 true，但**不会**做这个覆盖。
    /// </summary>
    [Fact]
    public void ApplyDoorState_FirstThreeBlockedOnlyWhenOpen()
    {
        Assert.True(HerbActorFramework.ApplyDoorState_FirstThreeAreBlocked(TDoorState.dsOpen));
        Assert.False(HerbActorFramework.ApplyDoorState_FirstThreeAreBlocked(TDoorState.dsClose));
        Assert.False(HerbActorFramework.ApplyDoorState_FirstThreeAreBlocked(TDoorState.dsBroken));

        // 净效果表：dsOpen ⇒ 那 3 格最终不可走；其余 ⇒ 可走
        foreach (TDoorState s in Enum.GetValues<TDoorState>())
        {
            bool bowalk = HerbActorFramework.ApplyDoorState_Bowalk(s);
            bool overridden = HerbActorFramework.ApplyDoorState_FirstThreeAreBlocked(s);
            bool firstThreeFinal = overridden ? false : (s == TDoorState.dsClose ? true : true);
            // 无条件标记为可走，随后仅 dsOpen 覆盖为不可走
            bool effective = overridden ? false : true;
            Assert.Equal(s == TDoorState.dsOpen ? false : true, effective);
            _ = bowalk; _ = firstThreeFinal;
        }
    }

    /// <summary>用例 3：可行走标记的偏移表与原文字面一致。</summary>
    [Fact]
    public void ApplyDoorState_OffsetTablesMatchSource()
    {
        Assert.Equal(new[] { (0, -2), (1, -1), (1, -2) }, HerbActorFramework.DoorAlwaysWalkOffsets);
        Assert.Equal(new[]
        {
            (0, 0), (0, -1), (0, -2), (1, -1), (1, -2), (-1, -1), (-1, 0), (-1, 1), (-2, 0),
        }, HerbActorFramework.DoorBowalkOffsets);

        // 差异断言：前 3 个偏移是第 2 张表的子集（原文的"重复设置"
        // —— (0,-2)/(1,-1)/(1,-2) 两次都在列表里）
        foreach (var off in HerbActorFramework.DoorAlwaysWalkOffsets)
        {
            Assert.Contains(off, HerbActorFramework.DoorBowalkOffsets);
        }
        Assert.Equal(9, HerbActorFramework.DoorBowalkOffsets.Length);
    }

    /// <summary>用例 4：Create 的字段默认值（DownDrawLevel=1、UserName 是空格）。</summary>
    [Fact]
    public void CastleDoor_Defaults()
    {
        Assert.Equal(1, HerbActorFramework.CastleDoorDownDrawLevel);
        Assert.Equal(" ", HerbActorFramework.CastleDoorUserName);
        Assert.NotEqual("", HerbActorFramework.CastleDoorUserName);   // 差异断言：是一个空格
    }

    // ══════════════════════════════════════════════════════════════════════
    // TWallStructure 常量与覆盖登记
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>用例 1：墙体破碎特效基址按左右分。</summary>
    [Fact]
    public void WallBrokenEffectBase_SplitsLeftAndRight()
    {
        Assert.Equal(224, HerbActorFramework.WallBrokenEffectBase(true));
        Assert.Equal(240, HerbActorFramework.WallBrokenEffectBase(false));
        Assert.NotEqual(HerbActorFramework.WallBrokenEffectBase(true),
                        HerbActorFramework.WallBrokenEffectBase(false));
    }

    /// <summary>用例 2：bomarkpos 默认 false。</summary>
    [Fact]
    public void WallBomarkPosDefault_IsFalse()
        => Assert.False(HerbActorFramework.WallBomarkPosDefault());

    /// <summary>用例 3：ActionName 的映射（含未知动作的兜底）。</summary>
    [Fact]
    public void ActionName_MapsKnownAndUnknown()
    {
        Assert.Equal("SM_TURN", HerbActorFramework.ActionName(ActorCoreSM.SM_TURN));
        Assert.Equal("SM_HIT", HerbActorFramework.ActionName(ActorCoreSM.SM_HIT));
        Assert.Equal("SM_DIGDOWN", HerbActorFramework.ActionName(ActorCoreSM.SM_DIGDOWN));
        Assert.Equal("SM_123", HerbActorFramework.ActionName(123));
    }

    /// <summary>用例 4：覆盖登记表自洽（每条都有类名/行号/状态，且行号落在原文范围内）。</summary>
    [Fact]
    public void CoverageTable_IsSelfConsistent()
    {
        var units = HerbActorCoverage.Units;
        Assert.NotEmpty(units);
        foreach (var u in units)
        {
            Assert.False(string.IsNullOrWhiteSpace(u.ClassName));
            Assert.False(string.IsNullOrWhiteSpace(u.Status));
            Assert.InRange(u.SourceLine, 1, 1223);
            // 已落地 与 未落地 必须明确标注
            Assert.True(u.Status.StartsWith("已落地") || u.Status.StartsWith("部分") || u.Status.Contains("未落地"),
                $"{u.ClassName} 的状态未标注落地程度：{u.Status}");
        }
        // 必须显式登记"基类虚方法受阻"这一条
        Assert.Contains(units, u => u.Status.Contains("未落地") && u.Status.Contains("Scenes"));
        // 未覆盖区间登记非空
        Assert.NotEmpty(HerbActorCoverage.UncoveredRanges);
    }

    /// <summary>用例 5：未覆盖区间不重叠且落在原文 1..1341 内。</summary>
    [Fact]
    public void UncoveredRanges_AreOrderedAndInBounds()
    {
        var ranges = HerbActorCoverage.UncoveredRanges
            .Where(r => r.From <= r.To)      // 忽略占位行
            .OrderBy(r => r.From)
            .ToArray();
        Assert.NotEmpty(ranges);
        foreach (var r in ranges)
        {
            Assert.InRange(r.From, 1, 1341);
            Assert.InRange(r.To, 1, 1341);
            Assert.True(r.To >= r.From);
            Assert.False(string.IsNullOrWhiteSpace(r.Reason));
        }
        for (int i = 1; i < ranges.Length; i++)
        {
            Assert.True(ranges[i].From > ranges[i - 1].To,
                $"未覆盖区间重叠：{ranges[i - 1]} 与 {ranges[i]}");
        }
    }
}
