using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

/// <summary>
/// 批次 p6-client-actor：Source\Client-HGE\CustomActor.pas（1,130 行）1:1 移植的
/// 纯决策层行为锁定。
///
/// 覆盖：CustomActorGate（全单元 6 处前置门）、CustomActorLogic.CalcActorFrame（72-401，
/// 含 10 个动作分支 + SM_ATTACK01..06 六槽 + 默认落空）、CustomActorDefaultFrame（585-639）、
/// CustomActorSurface（402-584 的偏移与取图分派）、CustomActorDraw（640-787 的绘制顺序与
/// DrawSelfMagicEffect 门）、CustomActorRun（788-1038 的推进与两段特效生成门）、
/// CustomActorSound（1039-1130）。
///
/// 差异断言（"看起来一样实则不同"）：
///   CalcActorFrame_SkeletonBranchIsEmptyButLightStillWritten（空实现仍写 m_nChrLight）
///   CalcActorFrame_DeathUsesLastFrameWhileNowDeathUsesFullRange（SM_DEATH vs SM_NOWDEATH）
///   SurfaceFetchKind_Effect2DoesNotHonourGrayScale2（三段取图分派的不对称）
///   DrawSequence_Eff1Eff2SelfAndUnknownShareTheElseBranch（第三个枚举值与未知值同落 else）
///   SelfEffectImageIndex_NoneIgnoresDirWhileNormalHonoursIt
///   Run_NoFlyConditionIsNotTheComplementOfFlyCondition（844 与 941 的或关系不互补）
/// </summary>
public sealed class CustomActorTests : IDisposable
{
    private readonly List<Action> _restore = new();

    public CustomActorTests()
    {
        CustomActorEnv.Reset();
        _restore.Add(CustomActorEnv.Reset);

        // 画布默认就绪，便于 LoadSurface 走到主体
        CustomActorEnv.GameCanvasActive = true;
        CustomActorEnv.GameCanvasInitialized = true;

        _restore.Add(() =>
        {
            CustomActorEnv.GameCanvasActive = false;
            CustomActorEnv.GameCanvasInitialized = false;
        });
    }

    public void Dispose()
    {
        foreach (var r in _restore)
            r();
    }

    // ===================== 测试构造工具 =====================

    private static TMonsterClientAction Act(TMonsterClientActionType t, int start, int play, int empty,
        bool calcDir, int playTime = 100, int effectIndex = -1, int effectIndex2 = -1,
        int effectFile = -1, int effectFile2 = -1, int actionFile = -1)
        => new()
        {
            ActionType = t,
            StartIndex = (short)start,
            PlayCount = (ushort)play,
            EmptyCount = (ushort)empty,
            PlayTime = (ushort)playTime,
            CalcDir = (byte)(calcDir ? 1 : 0),
            EffectIndex = (short)effectIndex,
            EffectIndex2 = (short)effectIndex2,
            EffectFile = (short)effectFile,
            EffectFile2 = (short)effectFile2,
            ActionFile = (short)actionFile,
        };

    /// <summary>
    /// 标准自定义怪配置：
    /// stand(100,4,2) / walk(200,6,1) / defAttack(300,3,0) / struck(400,2,0) /
    /// die(500,8,1) / stoneRevive(600,5,1)，全部 CalcDir。
    /// </summary>
    private static TClientCustomMonsterConfig MakeCfg(
        bool calcDir = true, int diePlay = 8, int dieEmpty = 1,
        int attack1Start = 700, int attack1Play = 5)
    {
        var cfg = new TClientCustomMonsterConfig();
        cfg.Actions[(int)TMonsterClientActionType.matStand] =
            Act(TMonsterClientActionType.matStand, 100, 4, 2, calcDir);
        cfg.Actions[(int)TMonsterClientActionType.matWalk] =
            Act(TMonsterClientActionType.matWalk, 200, 6, 1, calcDir);
        cfg.Actions[(int)TMonsterClientActionType.matDefAttack] =
            Act(TMonsterClientActionType.matDefAttack, 300, 3, 0, calcDir);
        cfg.Actions[(int)TMonsterClientActionType.matStruck] =
            Act(TMonsterClientActionType.matStruck, 400, 2, 0, calcDir);
        cfg.Actions[(int)TMonsterClientActionType.matDie] =
            Act(TMonsterClientActionType.matDie, 500, diePlay, dieEmpty, calcDir);
        cfg.Actions[(int)TMonsterClientActionType.matStoneRevive] =
            Act(TMonsterClientActionType.matStoneRevive, 600, 5, 1, calcDir);
        cfg.Actions[(int)TMonsterClientActionType.matAttack1] =
            Act(TMonsterClientActionType.matAttack1, attack1Start, attack1Play, 0, calcDir);
        return cfg;
    }

    private static CustomActorCalcInput In(int race, int changeAppr, int action, in TClientCustomMonsterConfig cfg,
        int dir = 0, int state = 0, int oldChrLight = 0, int struckFrameTime = 150, int btStep = 0)
        => new(changeAppr, race, action, dir, btStep, state, oldChrLight, struckFrameTime, in cfg);

    // ===================== 前置门（全单元 6 处，逐字一致）=====================

    [Theory]
    [InlineData(0, 156, false)]    // ChangeAppr >= 0 但 race = 156 → 走自定义怪
    [InlineData(-1, 999, false)]   // ChangeAppr < 0 → 走自定义怪
    [InlineData(0, 999, true)]     // ChangeAppr >= 0 且 race <> 156 → 退回基类
    [InlineData(5, 0, true)]
    public void Gate_BypassToInherited(int changeAppr, int race, bool expected)
        => Assert.Equal(expected, CustomActorGate.BypassToInherited(changeAppr, race));

    // ===================== CalcActorFrame（72-401）=====================

    [Fact]
    public void CalcActorFrame_BypassGateReturnsNull()
    {
        var cfg = MakeCfg();
        var plan = CustomActorLogic.CalcActorFrame(
            In(999, 0, TActorCore.SM_TURN, in cfg, dir: 3), out int newAction);

        Assert.Null(plan);
        Assert.Equal(TActorCore.SM_TURN, newAction);   // 非攻击动作不改写
    }

    [Fact]
    public void CalcActorFrame_BypassGateRewritesAttackToHit()
    {
        var cfg = MakeCfg();

        // 80-87：六个攻击码在此门内被改写为 SM_HIT
        foreach (int atk in CustomActorAttackSlot.AttackActions)
        {
            var plan = CustomActorLogic.CalcActorFrame(
                In(999, 0, atk, in cfg), out int newAction);
            Assert.Null(plan);
            Assert.Equal(TActorCore.SM_HIT, newAction);
        }
    }

    [Fact]
    public void CalcActorFrame_AttackRewritesOnlyWhenBypassGateHolds()
    {
        // 自定义怪身份下（门为假），攻击码**不被**改写
        var cfg = MakeCfg();
        var plan = CustomActorLogic.CalcActorFrame(
            In(156, -1, CustomActorAttackActions.SM_ATTACK01, in cfg), out int newAction);

        Assert.NotNull(plan);
        Assert.Equal(CustomActorAttackActions.SM_ATTACK01, newAction);
    }

    [Fact]
    public void CalcActorFrame_StandBranch()
    {
        var cfg = MakeCfg();
        var plan = CustomActorLogic.CalcActorFrame(
            In(156, -1, TActorCore.SM_TURN, in cfg, dir: 2, oldChrLight: 7), out _);

        Assert.NotNull(plan);
        var p = plan!;
        Assert.False(p.UseMagic);
        Assert.Equal(0, p.BodyOffset);                 // 96
        Assert.Equal(7, p.ChrLight);                   // 98
        Assert.Equal(100 + 2 * (4 + 2), p.StartFrame); // 126
        Assert.Equal(100 + 2 * 6 + 4 - 1, p.EndFrame); // 127
        Assert.Equal(4, p.DefFrameCount);
        Assert.True(p.WritesDefFrameCount);
        Assert.Equal(TMonsterClientActionType.matStand, p.ClientActionType);
        Assert.True(p.DoShift);
        Assert.Equal(0, p.ShiftStep);
        Assert.Equal(1, p.ShiftMax);                   // 132: Shift(dir,0,0,1)
    }

    [Fact]
    public void CalcActorFrame_StoneRevivePinsSingleFrame()
    {
        var cfg = MakeCfg();
        var plan = CustomActorLogic.CalcActorFrame(
            In(156, -1, TActorCore.SM_TURN, in cfg, dir: 3,
               state: (int)ActorStates.STATE_STONE_MODE), out _);

        var p = plan!;
        // 112-113：StartFrame := EndFrame（单帧定格）——与站立分支的关键差异
        Assert.Equal(600 + 3 * (5 + 1), p.StartFrame);
        Assert.Equal(p.StartFrame, p.EndFrame);
        Assert.Equal(TMonsterClientActionType.matStoneRevive, p.ClientActionType);
    }

    [Fact]
    public void CalcActorFrame_StandIsNotSingleFrameUnlikeStoneRevive()
    {
        // 差异断言：两条 0/SM_TURN 路径的 EndFrame 语义不同
        var cfg = MakeCfg();
        var stand = CustomActorLogic.CalcActorFrame(
            In(156, -1, TActorCore.SM_TURN, in cfg), out _)!;
        var stone = CustomActorLogic.CalcActorFrame(
            In(156, -1, TActorCore.SM_TURN, in cfg,
               state: (int)ActorStates.STATE_STONE_MODE), out _)!;

        Assert.NotEqual(stand.EndFrame - stand.StartFrame, stone.EndFrame - stone.StartFrame);
        Assert.Equal(3, stand.EndFrame - stand.StartFrame);   // PlayCount 4
        Assert.Equal(0, stone.EndFrame - stone.StartFrame);
    }

    [Theory]
    [InlineData(TActorCore.SM_WALK)]
    [InlineData(TActorCore.SM_RUSH)]
    [InlineData(TActorCore.SM_RUSHKUNG)]
    public void CalcActorFrame_WalkFamilyForward(int action)
    {
        var cfg = MakeCfg();
        var plan = CustomActorLogic.CalcActorFrame(
            In(156, -1, action, in cfg, dir: 1), out _);

        var p = plan!;
        Assert.Equal(200 + 1 * (6 + 1), p.StartFrame);
        Assert.Equal(p.StartFrame + 6 - 1, p.EndFrame);
        Assert.Equal(1, p.MoveStep);                                  // 147
        Assert.Equal(p.ShiftDir, 1);                                  // 153: Shift(m_btDir,...)
        Assert.Equal(p.ShiftMax, p.EndFrame - p.StartFrame + 1);
        Assert.Equal(TMonsterClientActionType.matWalk, p.ClientActionType);
        Assert.False(p.WritesDefFrameCount);                          // 行走族不写
    }

    [Fact]
    public void CalcActorFrame_BackstepUsesReversedDirAndStep()
    {
        var cfg = MakeCfg();
        var plan = CustomActorLogic.CalcActorFrame(
            In(156, -1, TActorCore.SM_BACKSTEP, in cfg, dir: 2, btStep: 3), out _);

        var p = plan!;
        Assert.Equal(6, p.ShiftDir);            // GetBack(2) = (2+4)%8 = 6
        Assert.Equal(3, p.ShiftStep);           // 149: m_nMoveStep := m_btStep
        Assert.Equal(3, p.MoveStep);
    }

    [Fact]
    public void CalcActorFrame_WalkAndBackstepShiftDifferOnSameDir()
    {
        // 差异断言：同方向下前进用 m_btDir、后退用 GetBack
        var cfg = MakeCfg();
        var walk = CustomActorLogic.CalcActorFrame(
            In(156, -1, TActorCore.SM_WALK, in cfg, dir: 2, btStep: 3), out _)!;
        var back = CustomActorLogic.CalcActorFrame(
            In(156, -1, TActorCore.SM_BACKSTEP, in cfg, dir: 2, btStep: 3), out _)!;

        Assert.NotEqual(walk.ShiftDir, back.ShiftDir);
        Assert.Equal(2, walk.ShiftDir);
        Assert.Equal(6, back.ShiftDir);
    }

    [Fact]
    public void CalcActorFrame_DigUpResetsState()
    {
        var cfg = MakeCfg();
        var plan = CustomActorLogic.CalcActorFrame(
            In(156, -1, TActorCore.SM_DIGUP, in cfg, dir: 1, state: 0x7F), out _);

        var p = plan!;
        Assert.True(p.WritesState);                 // 170: m_nState := 0
        Assert.Equal(0, p.State);
        Assert.Equal(600 + 1 * (5 + 1), p.StartFrame);
        Assert.Equal(TMonsterClientActionType.matStoneRevive, p.ClientActionType);
        Assert.True(p.WritesDefFrameCount);         // 168
    }

    [Fact]
    public void CalcActorFrame_LightingExIsEmptyImplementation()
    {
        var cfg = MakeCfg();
        var p = CustomActorLogic.CalcActorFrame(
            In(156, -1, TActorCore.SM_LIGHTINGEX, in cfg, oldChrLight: 9), out _)!;

        // 174-205 整段被原文注释掉：只有 93-98 的无条件重置生效
        Assert.Null(p.ClientActionType);     // ClientAction 保持 nil
        Assert.False(p.DoShift);             // 不 Shift
        Assert.Equal(9, p.ChrLight);         // 但 98 仍写 m_nChrLight
        Assert.Equal(0, p.BodyOffset);
    }

    [Fact]
    public void CalcActorFrame_SkeletonBranchIsEmptyButLightStillWritten()
    {
        var cfg = MakeCfg();
        var p = CustomActorLogic.CalcActorFrame(
            In(156, -1, TActorCore.SM_SKELETON, in cfg, oldChrLight: 4), out _)!;

        // 276-278 是**完全空**的 begin end；但 98 行的 m_nChrLight 仍已生效
        Assert.Null(p.ClientActionType);
        Assert.False(p.DoShift);
        Assert.Equal(4, p.ChrLight);
    }

    [Fact]
    public void CalcActorFrame_LightingAndSkeletonBothLeaveClientActionNil()
    {
        // 差异断言：两个"空实现"动作的差异仅在 SM_LIGHTINGEX 有整段注释代码，
        // 但**输出上二者完全一致**（都只留 93-98 的重置）——固化这一点以防日后误"补全"。
        var cfg = MakeCfg();
        var a = CustomActorLogic.CalcActorFrame(
            In(156, -1, TActorCore.SM_LIGHTINGEX, in cfg, oldChrLight: 4), out _)!;
        var b = CustomActorLogic.CalcActorFrame(
            In(156, -1, TActorCore.SM_SKELETON, in cfg, oldChrLight: 4), out _)!;

        Assert.Equal(a.ClientActionType, b.ClientActionType);
        Assert.Equal(a.DoShift, b.DoShift);
        Assert.Equal(a.ChrLight, b.ChrLight);
        Assert.Equal(a.FrameTime, b.FrameTime);
    }

    [Fact]
    public void CalcActorFrame_HitWritesWarModeTime()
    {
        var cfg = MakeCfg();
        var p = CustomActorLogic.CalcActorFrame(
            In(156, -1, TActorCore.SM_HIT, in cfg, dir: 2), out _)!;

        Assert.True(p.WritesWarModeTime);      // 217
        Assert.Equal(300 + 2 * 3, p.StartFrame);
        Assert.Equal(TMonsterClientActionType.matDefAttack, p.ClientActionType);
    }

    [Fact]
    public void CalcActorFrame_StruckUsesStruckFrameTimeAndResetsStruckCounter()
    {
        var cfg = MakeCfg();
        var p = CustomActorLogic.CalcActorFrame(
            In(156, -1, TActorCore.SM_STRUCK, in cfg, struckFrameTime: 275), out _)!;

        Assert.Equal(275u, p.FrameTime);            // 239：不用 ClientAction.PlayTime
        Assert.True(p.ResetsCustomMagicStruck);     // 243
        Assert.Equal(TMonsterClientActionType.matStruck, p.ClientActionType);
    }

    [Fact]
    public void CalcActorFrame_StruckFrameTimeDiffersFromActionPlayTime()
    {
        // 差异断言：其余分支用 ClientAction.PlayTime，SM_STRUCK 用 m_dwStruckFrameTime
        var cfg = MakeCfg();
        var struck = CustomActorLogic.CalcActorFrame(
            In(156, -1, TActorCore.SM_STRUCK, in cfg, struckFrameTime: 275), out _)!;
        var hit = CustomActorLogic.CalcActorFrame(
            In(156, -1, TActorCore.SM_HIT, in cfg, struckFrameTime: 275), out _)!;

        Assert.Equal(275u, struck.FrameTime);
        Assert.Equal(100u, hit.FrameTime);          // MakeCfg 的 PlayTime
        Assert.NotEqual(struck.FrameTime, hit.FrameTime);
    }

    [Fact]
    public void CalcActorFrame_DeathUsesLastFrameWhileNowDeathUsesFullRange()
    {
        var cfg = MakeCfg(diePlay: 8, dieEmpty: 1);
        var death = CustomActorLogic.CalcActorFrame(
            In(156, -1, TActorCore.SM_DEATH, in cfg, dir: 2), out _)!;
        var nowDeath = CustomActorLogic.CalcActorFrame(
            In(156, -1, TActorCore.SM_NOWDEATH, in cfg, dir: 2), out _)!;

        // 259-260：StartFrame 直接取末帧且 EndFrame := StartFrame
        Assert.Equal(500 + 2 * 9 + 8 - 1, death.StartFrame);
        Assert.Equal(death.StartFrame, death.EndFrame);
        Assert.False(death.DoShift);                 // 死亡分支无 Shift

        // 271-272：整段动画
        Assert.Equal(500 + 2 * 9, nowDeath.StartFrame);
        Assert.Equal(nowDeath.StartFrame + 8 - 1, nowDeath.EndFrame);
        Assert.False(nowDeath.DoShift);              // 现死也无 Shift

        Assert.NotEqual(death.StartFrame, nowDeath.StartFrame);
    }

    [Fact]
    public void CalcActorFrame_AttackBranchUsesSlotAndSetsMagic()
    {
        var cfg = MakeCfg(attack1Start: 700, attack1Play: 5);
        cfg.AttackConfigs[0] = new TClientAttackConfig { Self_LightRange = 12 };

        var p = CustomActorLogic.CalcActorFrame(
            In(156, -1, CustomActorAttackActions.SM_ATTACK01, in cfg, dir: 1, oldChrLight: 3), out _)!;

        Assert.True(p.UseMagic);                     // 342
        Assert.Equal(12, p.ChrLight);                // 286-288：Self_LightRange > 0 覆盖
        Assert.Equal(5, p.SpellFrame);               // 343：ClientAction.PlayCount
        Assert.Equal(0, p.CurEffFrame);              // 393
        Assert.Equal(0, p.CurSelfEffFrame);          // 394
        Assert.Equal(0, p.AttackConfigIndex);
        Assert.Equal(TMonsterClientActionType.matAttack1, p.ClientActionType);
        Assert.True(p.DoShift);
    }

    [Fact]
    public void CalcActorFrame_AttackLightRangeZeroDoesNotOverride()
    {
        var cfg = MakeCfg();
        cfg.AttackConfigs[0] = new TClientAttackConfig { Self_LightRange = 0 };

        var p = CustomActorLogic.CalcActorFrame(
            In(156, -1, CustomActorAttackActions.SM_ATTACK01, in cfg, oldChrLight: 3), out _)!;

        Assert.Equal(3, p.ChrLight);                 // 286 的 > 0 门未过 → 保留 m_nOldChrLight
    }

    [Theory]
    [InlineData(0, TMonsterClientActionType.matAttack1)]
    [InlineData(1, TMonsterClientActionType.matAttack2)]
    [InlineData(2, TMonsterClientActionType.matAttack3)]
    [InlineData(3, TMonsterClientActionType.matAttack4)]
    [InlineData(4, TMonsterClientActionType.matAttack5)]
    [InlineData(5, TMonsterClientActionType.matAttack6)]
    public void CalcActorFrame_AttackSlotsMapOneToOne(int slot, TMonsterClientActionType expected)
    {
        var cfg = MakeCfg();
        int action = CustomActorAttackSlot.AttackActions[slot];
        var p = CustomActorLogic.CalcActorFrame(In(156, -1, action, in cfg), out _)!;

        Assert.Equal(expected, p.ClientActionType);
        Assert.Equal(slot, p.AttackConfigIndex);
    }

    [Fact]
    public void CalcActorFrame_UnknownActionFallsThroughToDefault()
    {
        var cfg = MakeCfg();
        var p = CustomActorLogic.CalcActorFrame(
            In(156, -1, 12345, in cfg, oldChrLight: 6), out _)!;

        // 原文 case 无 else：ClientAction 保持 nil，仅 93-98 生效
        Assert.Null(p.ClientActionType);
        Assert.False(p.DoShift);
        Assert.Equal(6, p.ChrLight);
    }

    [Fact]
    public void CalcActorFrame_CalcDirFalseUsesZeroDir()
    {
        var cfg = MakeCfg(calcDir: false);
        var p = CustomActorLogic.CalcActorFrame(
            In(156, -1, TActorCore.SM_TURN, in cfg, dir: 5), out _)!;

        Assert.Equal(100, p.StartFrame);            // TempDir = 0
    }

    [Fact]
    public void CalcActorFrame_ZeroAndTurnAreEquivalent()
    {
        var cfg = MakeCfg();
        var zero = CustomActorLogic.CalcActorFrame(In(156, -1, 0, in cfg, dir: 4), out _)!;
        var turn = CustomActorLogic.CalcActorFrame(
            In(156, -1, TActorCore.SM_TURN, in cfg, dir: 4), out _)!;

        Assert.Equal(zero.StartFrame, turn.StartFrame);
        Assert.Equal(zero.EndFrame, turn.EndFrame);
        Assert.Equal(zero.ClientActionType, turn.ClientActionType);
    }

    // ===================== SelfCenterEffect（350-377）=====================

    [Fact]
    public void SelfCenterEffect_RequiresMdctCenter()
    {
        var cfg = MakeCfg();
        cfg.AttackConfigs[0] = new TClientAttackConfig
        {
            Self_StartIndex = 10, Self_PlayCount = 4, Self_EmptyCount = 1,
            Self_DirCalcType = TCustomDirCalcType.mdctNormal,
        };
        var p = CustomActorLogic.SelfCenterEffect(CustomActorAttackActions.SM_ATTACK01, in cfg);
        Assert.False(p.Applies);                    // 350：仅 mdctCenter
    }

    [Fact]
    public void SelfCenterEffect_DirCountEightAndSixteen()
    {
        var cfg8 = MakeCfg();
        cfg8.AttackConfigs[0] = new TClientAttackConfig
        {
            Self_StartIndex = 10, Self_PlayCount = 4, Self_EmptyCount = 1,
            Self_DirCalcType = TCustomDirCalcType.mdctCenter,
            Self_DirCount = TCustomDirCount.mdcDir8,
        };
        Assert.Equal(8, CustomActorLogic.SelfCenterEffect(CustomActorAttackActions.SM_ATTACK01, in cfg8).DirCount);

        var cfg16 = MakeCfg();
        cfg16.AttackConfigs[0] = new TClientAttackConfig
        {
            Self_StartIndex = 10, Self_PlayCount = 4, Self_EmptyCount = 1,
            Self_DirCalcType = TCustomDirCalcType.mdctCenter,
            Self_DirCount = TCustomDirCount.mdcDir16,
        };
        Assert.Equal(16, CustomActorLogic.SelfCenterEffect(CustomActorAttackActions.SM_ATTACK01, in cfg16).DirCount);
    }

    [Fact]
    public void SelfCenterEffect_FrameFormula()
    {
        var cfg = MakeCfg();
        cfg.AttackConfigs[0] = new TClientAttackConfig
        {
            Self_StartIndex = 10, Self_PlayCount = 4, Self_EmptyCount = 2,
            Self_DirCalcType = TCustomDirCalcType.mdctCenter,
            Self_DirCount = TCustomDirCount.mdcDir8,
        };
        var p = CustomActorLogic.SelfCenterEffect(CustomActorAttackActions.SM_ATTACK01, in cfg);

        Assert.Equal(10, CustomActorLogic.SelfCenterEffectFrame(in p, 0));
        Assert.Equal(10 + 6, CustomActorLogic.SelfCenterEffectFrame(in p, 1));
        Assert.Equal(10 + 5 * 6, CustomActorLogic.SelfCenterEffectFrame(in p, 5));
    }

    [Fact]
    public void SelfCenterEffect_NonAttackActionDoesNotApply()
    {
        var cfg = MakeCfg();
        var p = CustomActorLogic.SelfCenterEffect(TActorCore.SM_HIT, in cfg);
        Assert.False(p.Applies);
    }

    // ===================== GetDefaultFrame（585-639）=====================

    private static CustomActorDefaultFrameInput Df(int changeAppr, int race, in TClientCustomMonsterConfig cfg,
        int dir = 0, bool death = false, bool skeleton = false, int state = 0, int curDefFrame = -1,
        int oldChrLight = 0)
        => new(changeAppr, race, dir, death, skeleton, state, curDefFrame, oldChrLight, in cfg);

    [Fact]
    public void GetDefaultFrame_BypassGateReturnsNull()
    {
        var cfg = MakeCfg();
        Assert.Null(CustomActorDefaultFrame.Compute(Df(0, 999, in cfg)));
    }

    [Fact]
    public void GetDefaultFrame_StandHonoursDefFrame()
    {
        var cfg = MakeCfg();
        var p = CustomActorDefaultFrame.Compute(Df(-1, 156, in cfg, dir: 3, curDefFrame: 2, oldChrLight: 8))!;

        Assert.Equal(100 + 3 * 6 + 2, p.Frame);
        Assert.Equal(8, p.ChrLight);                       // 596
        Assert.Equal((int)TMonsterClientActionType.matStand, p.ClientActionIndex);
    }

    [Theory]
    [InlineData(-1, 0)]     // 621: < 0 → 0
    [InlineData(0, 0)]
    [InlineData(3, 3)]      // PlayCount = 4：3 仍在范围内 → 原值
    [InlineData(4, 0)]      // 623: >= PlayCount(4) → 0
    [InlineData(2, 2)]      // 否则原值
    public void GetDefaultFrame_DefFrameThreeWayClamp(int curDefFrame, int expectedCf)
    {
        var cfg = MakeCfg();
        var p = CustomActorDefaultFrame.Compute(Df(-1, 156, in cfg, curDefFrame: curDefFrame))!;
        Assert.Equal(100 + expectedCf, p.Frame);
    }

    [Fact]
    public void GetDefaultFrame_DeathSkeletonUsesDieStartWithoutDir()
    {
        var cfg = MakeCfg();
        var p = CustomActorDefaultFrame.Compute(
            Df(-1, 156, in cfg, dir: 5, death: true, skeleton: true))!;

        Assert.Equal(500, p.Frame);                        // 600：不加方向
        Assert.Equal((int)TMonsterClientActionType.matDie, p.ClientActionIndex);
    }

    [Fact]
    public void GetDefaultFrame_DeathNonSkeletonUsesLastFrameWithDir()
    {
        var cfg = MakeCfg(diePlay: 8, dieEmpty: 1);
        var p = CustomActorDefaultFrame.Compute(
            Df(-1, 156, in cfg, dir: 2, death: true))!;

        Assert.Equal(500 + 2 * 9 + 7, p.Frame);
    }

    [Fact]
    public void GetDefaultFrame_DeathSkeletonAndNonSkeletonDiffer()
    {
        var cfg = MakeCfg(diePlay: 8, dieEmpty: 1);
        var skel = CustomActorDefaultFrame.Compute(
            Df(-1, 156, in cfg, dir: 2, death: true, skeleton: true))!;
        var full = CustomActorDefaultFrame.Compute(
            Df(-1, 156, in cfg, dir: 2, death: true))!;

        Assert.NotEqual(skel.Frame, full.Frame);
        Assert.Equal(500, skel.Frame);
        Assert.Equal(500 + 2 * 9 + 7, full.Frame);
    }

    [Fact]
    public void GetDefaultFrame_StoneModeUsesReviveStart()
    {
        var cfg = MakeCfg();
        var p = CustomActorDefaultFrame.Compute(
            Df(-1, 156, in cfg, dir: 1, state: (int)ActorStates.STATE_STONE_MODE))!;

        Assert.Equal(600 + 1 * 6, p.Frame);
        Assert.Equal((int)TMonsterClientActionType.matStoneRevive, p.ClientActionIndex);
    }

    [Fact]
    public void GetDefaultFrame_DeathTakesPrecedenceOverStoneMode()
    {
        // 差异断言：598 的 m_boDeath 分支在外层，先于 611 的石化判定
        var cfg = MakeCfg();
        var p = CustomActorDefaultFrame.Compute(
            Df(-1, 156, in cfg, dir: 1, death: true,
               state: (int)ActorStates.STATE_STONE_MODE))!;

        Assert.Equal((int)TMonsterClientActionType.matDie, p.ClientActionIndex);
        Assert.NotEqual((int)TMonsterClientActionType.matStoneRevive, p.ClientActionIndex);
    }

    // ===================== LoadSurface（402-584）=====================

    private static CustomActorSurfaceInput Si(int changeAppr, int race, in TClientCustomMonsterConfig cfg,
        bool death = false, bool reverse = false, int currentFrame = 10, int dir = 0, int appr = 0)
        => new(changeAppr, race, death, reverse, currentFrame, dir, appr, in cfg);

    [Fact]
    public void LoadSurface_BypassGateReturnsNull()
    {
        var cfg = MakeCfg();
        Assert.Null(CustomActorSurface.Compute(Si(0, 999, in cfg)));
    }

    [Fact]
    public void LoadSurface_CanvasNotReadyReturnsNull()
    {
        var cfg = MakeCfg();
        CustomActorEnv.GameCanvasActive = false;
        Assert.Null(CustomActorSurface.Compute(Si(-1, 156, in cfg)));
    }

    [Fact]
    public void LoadSurface_HiddenWhenGhostGateHolds()
    {
        var cfg = MakeCfg();
        CustomActorEnv.PlugInEnabled = true;
        CustomActorEnv.ClientConfigBoHideGhost = true;
        CustomActorEnv.ConfigDlgCkHideGhost = true;

        var p = CustomActorSurface.Compute(Si(-1, 156, in cfg, death: true, appr: 0))!;
        Assert.True(p.Hidden);
        Assert.False(p.LoadBody);
    }

    [Theory]
    [InlineData(900, true)]    // 边界：900 与 906 均在豁免区间内
    [InlineData(906, true)]
    [InlineData(899, false)]
    [InlineData(907, false)]
    public void HideGhost_AppearanceRangeIsInclusive(int appr, bool exempt)
    {
        CustomActorEnv.PlugInEnabled = true;
        CustomActorEnv.ClientConfigBoHideGhost = true;
        CustomActorEnv.ConfigDlgCkHideGhost = true;

        bool hidden = CustomActorSurface.HideGhost(true, appr);
        Assert.Equal(!exempt, hidden);     // 豁免 → 不隐藏
    }

    [Fact]
    public void HideGhost_RequiresAllThreeSwitches()
    {
        CustomActorEnv.PlugInEnabled = false;   // 只关一个
        CustomActorEnv.ClientConfigBoHideGhost = true;
        CustomActorEnv.ConfigDlgCkHideGhost = true;
        Assert.False(CustomActorSurface.HideGhost(true, 0));

        CustomActorEnv.PlugInEnabled = true;
        CustomActorEnv.ClientConfigBoHideGhost = false;
        Assert.False(CustomActorSurface.HideGhost(true, 0));

        CustomActorEnv.ClientConfigBoHideGhost = true;
        CustomActorEnv.ConfigDlgCkHideGhost = false;
        Assert.False(CustomActorSurface.HideGhost(true, 0));

        CustomActorEnv.ConfigDlgCkHideGhost = true;
        Assert.False(CustomActorSurface.HideGhost(false, 0));   // m_boDeath 也要
        Assert.True(CustomActorSurface.HideGhost(true, 0));
    }

    [Fact]
    public void LoadSurface_BodyOffsetIsCurrentFrame()
    {
        var cfg = MakeCfg();                     // 全部动作 StartIndex>=0 且 PlayCount>0
        var p = CustomActorSurface.Compute(Si(-1, 156, in cfg, currentFrame: 42))!;

        Assert.True(p.LoadBody);
        Assert.Equal(42, p.BodyOffset);          // 502：nBodyOffset := m_nCurrentFrame
    }

    [Fact]
    public void LoadSurface_SkipsActionWithZeroPlayCount()
    {
        var cfg = new TClientCustomMonsterConfig();   // 全 0：StartIndex 0 但 PlayCount 0
        var p = CustomActorSurface.Compute(Si(-1, 156, in cfg))!;

        Assert.False(p.LoadBody);                     // 496 的三门未过
        Assert.False(p.LoadEffect);
        Assert.False(p.LoadEffect2);
    }

    [Fact]
    public void LoadSurface_ActionFileOutOfRangeFallsBackToWMon()
    {
        var cfg = MakeCfg();
        cfg.Actions[(int)TMonsterClientActionType.matStand] =
            Act(TMonsterClientActionType.matStand, 100, 4, 2, true, actionFile: 7);
        CustomActorEnv.EffectImageListCountFn = () => 3;      // 7 >= 3 → 越界

        var p = CustomActorSurface.Compute(Si(-1, 156, in cfg, appr: 55))!;
        Assert.True(p.BodyFallsBackToWMon);
        Assert.Equal(55, p.BodyFileIndex);                    // g_WMonImages.Images[m_wAppearance]
    }

    [Fact]
    public void LoadSurface_ActionFileInRangeUsesEffectImageList()
    {
        var cfg = MakeCfg();
        cfg.Actions[(int)TMonsterClientActionType.matStand] =
            Act(TMonsterClientActionType.matStand, 100, 4, 2, true, actionFile: 2);
        CustomActorEnv.EffectImageListCountFn = () => 3;      // 2 < 3 → 命中

        var p = CustomActorSurface.Compute(Si(-1, 156, in cfg, appr: 55))!;
        Assert.False(p.BodyFallsBackToWMon);
        Assert.Equal(2, p.BodyFileIndex);
    }

    [Fact]
    public void EffectOffset_CalcDirUsesModuloOfStride()
    {
        // 514：TempOffset = (currentFrame - StartIndex) mod (PlayCount + EmptyCount)
        var a = Act(TMonsterClientActionType.matStand, 100, 4, 2, true, effectIndex: 900);
        // (110 - 100) % 6 = 4；dir=2 → 900 + 2*6 + 4
        Assert.Equal(900 + 2 * 6 + 4, CustomActorSurface.EffectOffsetOf(in a, 2, 110, 0));
    }

    [Fact]
    public void EffectOffset_DieNoCalcDirDropsDirectionStride()
    {
        // 516-517：matDie 且 DieNoCalcDir → EffectIndex + TempOffset（不加方向步长）
        var a = Act(TMonsterClientActionType.matDie, 100, 4, 2, true, effectIndex: 900);
        Assert.Equal(900 + 4, CustomActorSurface.EffectOffsetOf(in a, 2, 110, 1));
    }

    [Fact]
    public void EffectOffset_DieWithCalcDirKeepsDirectionStride()
    {
        var a = Act(TMonsterClientActionType.matDie, 100, 4, 2, true, effectIndex: 900);
        // DieNoCalcDir = 0 → 仍走 519 的完整式
        Assert.Equal(900 + 2 * 6 + 4, CustomActorSurface.EffectOffsetOf(in a, 2, 110, 0));
    }

    [Fact]
    public void EffectOffset_NoCalcDirIgnoresModuloAndDir()
    {
        // 521-522：TempOffset = m_nCurrentFrame - StartIndex（**不取模**）
        var a = Act(TMonsterClientActionType.matStand, 100, 4, 2, false, effectIndex: 900);
        Assert.Equal(900 + 10, CustomActorSurface.EffectOffsetOf(in a, 2, 110, 0));
    }

    [Fact]
    public void EffectOffset2_ReadsEffectIndex2NotEffectIndex()
    {
        // 差异断言：Effect2 段读 EffectIndex2，与 Effect1 段的 EffectIndex 不同源
        var a = Act(TMonsterClientActionType.matStand, 100, 4, 2, true,
            effectIndex: 900, effectIndex2: 950);

        Assert.Equal(900 + 2 * 6 + 4, CustomActorSurface.EffectOffsetOf(in a, 2, 110, 0));
        Assert.Equal(950 + 2 * 6 + 4, CustomActorSurface.EffectOffset2Of(in a, 2, 110, 0));
    }

    [Fact]
    public void SurfaceFetchKind_ReverseFrameSuppressesAllThree()
    {
        Assert.Equal("none", CustomActorSurface.SurfaceFetchKind(TColorEffect.ceGrayScale, true, false));
        Assert.Equal("none", CustomActorSurface.SurfaceFetchKind(TColorEffect.ceBright, true, false));
        Assert.Equal("none", CustomActorSurface.SurfaceFetchKind(TColorEffect.ceNone, true, true));
    }

    [Theory]
    [InlineData(TColorEffect.ceGrayScale, "gray")]
    [InlineData(TColorEffect.ceBright, "bright")]
    [InlineData(TColorEffect.ceNone, "image")]
    [InlineData(TColorEffect.ceRed, "image")]
    public void SurfaceFetchKind_BodyMapping(TColorEffect ce, string expected)
        => Assert.Equal(expected, CustomActorSurface.SurfaceFetchKind(ce, false, false));

    [Fact]
    public void SurfaceFetchKind_Effect2DoesNotHonourGrayScale2()
    {
        // 差异断言：原文 573（Effect2 段）只列 ceGrayScale / ceBright，
        // **没有 ceGrayScale2**；而 551（Body）与 562（Effect1）都有。
        Assert.Equal("gray", CustomActorSurface.SurfaceFetchKind(TColorEffect.ceGrayScale2, false, false));
        Assert.Equal("gray", CustomActorSurface.SurfaceFetchKind(TColorEffect.ceGrayScale2, false, false));
        Assert.Equal("image", CustomActorSurface.SurfaceFetchKind(TColorEffect.ceGrayScale2, false, true));

        var (body, eff1, eff2) = CustomActorSurface.SurfaceFetches(TColorEffect.ceGrayScale2, false);
        Assert.Equal("gray", body);
        Assert.Equal("gray", eff1);
        Assert.Equal("image", eff2);
    }

    // ===================== DrawChr（640-787）=====================

    private static CustomActorDrawInput Di(int changeAppr, int race, int order,
        bool hasEff = true, bool hasEff2 = true)
        => new(changeAppr, race, order, 0, 0, hasEff, hasEff2);

    [Fact]
    public void DrawSequence_BypassGateReturnsNull()
        => Assert.Null(CustomActorDraw.Sequence(Di(0, 999, 0)));

    [Fact]
    public void DrawSequence_SelfEff1Eff2Order()
    {
        var seq = CustomActorDraw.Sequence(Di(-1, 156, CustomActorDraw.MdoSelfEff1Eff2))!;
        Assert.Equal(new[] { "self", "body", "eff1", "eff2", "-self" }, seq);
    }

    [Fact]
    public void DrawSequence_Eff1SelfEff2Order()
    {
        var seq = CustomActorDraw.Sequence(Di(-1, 156, CustomActorDraw.MdoEff1SelfEff2))!;
        Assert.Equal(new[] { "self", "eff1", "body", "eff2", "-self" }, seq);
    }

    [Fact]
    public void DrawSequence_Eff1Eff2SelfOrder()
    {
        var seq = CustomActorDraw.Sequence(Di(-1, 156, CustomActorDraw.MdoEff1Eff2Self))!;
        Assert.Equal(new[] { "self", "eff1", "eff2", "body", "-self" }, seq);
    }

    [Fact]
    public void DrawSequence_Eff1Eff2SelfAndUnknownShareTheElseBranch()
    {
        // 差异断言（反向）：mdoEff1_Eff2_Self 与任何未知 DrawOrder **落同一 else**，
        // 故二者产出完全一致 —— 固化"未知值不比枚举值更特殊"这一点。
        var known = CustomActorDraw.Sequence(Di(-1, 156, CustomActorDraw.MdoEff1Eff2Self))!;
        var unknown = CustomActorDraw.Sequence(Di(-1, 156, 99))!;
        Assert.Equal(known, unknown);
    }

    [Fact]
    public void DrawSequence_MissingSurfacesOmitTheirLayers()
    {
        var seq = CustomActorDraw.Sequence(Di(-1, 156, CustomActorDraw.MdoSelfEff1Eff2,
            hasEff: false, hasEff2: false))!;
        Assert.Equal(new[] { "self", "body", "-self" }, seq);
    }

    [Fact]
    public void DrawSequence_SelfCallsAlwaysBracketTheSequence()
    {
        // 724 恒在最前、785 恒在最后（对全部三种 DrawOrder 成立）
        foreach (int order in new[] { 0, 1, 2, 7 })
        {
            var seq = CustomActorDraw.Sequence(Di(-1, 156, order))!;
            Assert.Equal("self", seq[0]);
            Assert.Equal("-self", seq[seq.Count - 1]);
        }
    }

    [Fact]
    public void SelfMagicEffectGate_RequiresUseMagicAndNonZeroEffect()
    {
        Assert.False(CustomActorDraw.SelfMagicEffectGate(false, 8946));
        Assert.False(CustomActorDraw.SelfMagicEffectGate(true, 0));
        Assert.True(CustomActorDraw.SelfMagicEffectGate(true, 8946));
    }

    [Theory]
    [InlineData(-8946, 0)]      // -1 * SM_ATTACK01
    [InlineData(-8947, 1)]
    [InlineData(-8951, 5)]
    [InlineData(8946, -1)]      // 正值不匹配（原文是 -1 * SM_ATTACKxx）
    [InlineData(0, -1)]
    public void SelfMagicEffectAttackSlot_MatchesNegatedAttackCodes(int effectNumber, int expected)
        => Assert.Equal(expected, CustomActorDraw.SelfMagicEffectAttackSlot(effectNumber));

    [Fact]
    public void AdvanceSelfFrame_ResetsOnFrameZero()
    {
        var (f, t, ticked) = CustomActorDraw.AdvanceSelfFrameEx(
            curEffFrame: 0, curSelfEffFrame: 9, tick: 111,
            selfPlayTime: 50, selfPlayCount: 4, myGetTickCount: 999, timeGetTime: 777);

        Assert.Equal(0, f);            // 665：m_nCurSelfEffFrame := 0
        Assert.Equal(777u, t);         // 666：tick 刷新为 TimeGetTime
        Assert.True(ticked);
    }

    [Fact]
    public void AdvanceSelfFrame_IncrementsOnlyWhenTickElapsed()
    {
        // 未到点：不变
        var (f1, _, ticked1) = CustomActorDraw.AdvanceSelfFrameEx(
            1, 2, 100, selfPlayTime: 50, selfPlayCount: 4,
            myGetTickCount: 140, timeGetTime: 0);
        Assert.Equal(2, f1);
        Assert.False(ticked1);

        // 恰好到点（>=）：推进
        var (f2, t2, ticked2) = CustomActorDraw.AdvanceSelfFrameEx(
            1, 2, 100, selfPlayTime: 50, selfPlayCount: 4,
            myGetTickCount: 150, timeGetTime: 900);
        Assert.Equal(3, f2);
        Assert.Equal(900u, t2);
        Assert.True(ticked2);
    }

    [Fact]
    public void AdvanceSelfFrame_IncrementBoundaryIsSelfPlayCountInclusive()
    {
        // 668：`m_nCurSelfEffFrame <= Self_PlayCount` — 等于 PlayCount 时**仍**再加 1
        var (atCount, _, _) = CustomActorDraw.AdvanceSelfFrameEx(
            1, 4, 100, selfPlayTime: 50, selfPlayCount: 4,
            myGetTickCount: 200, timeGetTime: 0);
        Assert.Equal(5, atCount);      // 4 <= 4 → Inc

        var (aboveCount, _, _) = CustomActorDraw.AdvanceSelfFrameEx(
            1, 5, 100, selfPlayTime: 50, selfPlayCount: 4,
            myGetTickCount: 200, timeGetTime: 0);
        Assert.Equal(5, aboveCount);   // 5 <= 4 为假 → 不 Inc
    }

    [Fact]
    public void SelfEffectFrameInRange_IsInclusiveUpperBound()
    {
        // 681：[0..Self_PlayCount - 1]
        Assert.True(CustomActorDraw.SelfEffectFrameInRange(0, 4, 100));
        Assert.True(CustomActorDraw.SelfEffectFrameInRange(3, 4, 100));
        Assert.False(CustomActorDraw.SelfEffectFrameInRange(4, 4, 100));
        Assert.False(CustomActorDraw.SelfEffectFrameInRange(-1, 4, 100));
    }

    [Fact]
    public void SelfEffectFrameInRange_RequiresStartIndexAndPlayCount()
    {
        Assert.False(CustomActorDraw.SelfEffectFrameInRange(0, 4, -1));   // Self_StartIndex < 0
        Assert.False(CustomActorDraw.SelfEffectFrameInRange(0, 0, 100));  // Self_PlayCount = 0
    }

    [Fact]
    public void SelfEffectImageIndex_NoneIgnoresDirWhileNormalHonoursIt()
    {
        // 差异断言：mdctNone 图号与方向无关；mdctNormal 含 nDir 步长
        int none = CustomActorDraw.SelfEffectImageIndex(
            0, 10, 4, 2, curSelfEffFrame: 1, btDir: 5,
            magicTargetX: 3, magicTargetY: 3, dirCount: 0, myX: 0, myY: 0)!.Value;
        Assert.Equal(10 + 1, none);

        CustomActorEnv.GetNextDirectionFn = (_, _, _, _) => 3;
        int normal = CustomActorDraw.SelfEffectImageIndex(
            1, 10, 4, 2, curSelfEffFrame: 1, btDir: 5,
            magicTargetX: 3, magicTargetY: 3, dirCount: 0, myX: 0, myY: 0)!.Value;
        Assert.Equal(10 + 3 * 6 + 1, normal);

        Assert.NotEqual(none, normal);
    }

    [Fact]
    public void SelfEffectImageIndex_NormalUsesBtDirWhenTargetIsMinusOne()
    {
        // 692-693：目标 (-1,-1) 时**不看** GetNextDirection，直接用 m_btDir
        bool called = false;
        CustomActorEnv.GetNextDirectionFn = (_, _, _, _) => { called = true; return 7; };

        int idx = CustomActorDraw.SelfEffectImageIndex(
            1, 10, 4, 2, 1, btDir: 5, magicTargetX: -1, magicTargetY: -1,
            dirCount: 0, myX: 0, myY: 0)!.Value;

        Assert.Equal(10 + 5 * 6 + 1, idx);
        Assert.False(called);
    }

    [Fact]
    public void SelfEffectImageIndex_NonNoneNonNormalReturnsNull()
    {
        // 689/691 两分支均未命中 → SelfEffectSurface 保持 nil
        Assert.Null(CustomActorDraw.SelfEffectImageIndex(
            2, 10, 4, 2, 1, 0, 0, 0, 0, 0, 0));
    }

    [Fact]
    public void SelfEffectImageIndex_DirCountSixteenUsesFlyDirection16()
    {
        bool used16 = false;
        CustomActorEnv.GetFlyDirection16Fn = (_, _, _, _) => { used16 = true; return 9; };
        CustomActorEnv.GetNextDirectionFn = (_, _, _, _) => -1;

        int idx = CustomActorDraw.SelfEffectImageIndex(
            1, 10, 4, 2, 1, btDir: 0, magicTargetX: 3, magicTargetY: 3,
            dirCount: 1 /*mdcDir16*/, myX: 0, myY: 0)!.Value;

        Assert.True(used16);
        Assert.Equal(10 + 9 * 6 + 1, idx);
    }

    [Fact]
    public void SelfEffectDrawKind_BlendOnlyForMdmBlend()
    {
        Assert.Equal("DrawBlend", CustomActorDraw.SelfEffectDrawKind(0));
        Assert.Equal("Draw", CustomActorDraw.SelfEffectDrawKind(1));
        Assert.Equal("Draw", CustomActorDraw.SelfEffectDrawKind(9));
    }

    [Fact]
    public void SelfEffectDrawGate_DrawOrderPairsWithBeforeDraw()
    {
        // 675：mdoPriorMagic(1) 且 BeforeDraw
        Assert.True(CustomActorDraw.SelfEffectDrawGate(1, true, 1));
        Assert.False(CustomActorDraw.SelfEffectDrawGate(1, false, 1));
        // 676：mdoPriorSelf(0) 且 not BeforeDraw
        Assert.True(CustomActorDraw.SelfEffectDrawGate(0, false, 1));
        Assert.False(CustomActorDraw.SelfEffectDrawGate(0, true, 1));
    }

    [Fact]
    public void SelfEffectDrawGate_MdctCenterSuppressesBothPhases()
    {
        // 677：Self_DirCalcType = mdctCenter(2) → 无论哪一相都不绘
        Assert.False(CustomActorDraw.SelfEffectDrawGate(1, true, 2));
        Assert.False(CustomActorDraw.SelfEffectDrawGate(0, false, 2));
    }

    // ===================== Run（788-1038）=====================

    private static CustomActorRunInput Ri(int changeAppr, int race, in TClientCustomMonsterConfig cfg,
        bool useEffect = false, bool useMagic = false, int curEffFrame = 0, int spellFrame = 1,
        bool createEffect = false, int effectNumber = 0, uint now = 0,
        uint effectStartTime = 0, uint effectFrameTime = 0, int effectFrame = 0, int effectEnd = 0)
        => new(changeAppr, race, useEffect, effectFrameTime, effectStartTime,
               effectFrame, effectEnd, useMagic, curEffFrame, spellFrame, createEffect,
               effectNumber, 0, now, in cfg);

    [Fact]
    public void Run_BypassGateReturnsNull()
    {
        var cfg = MakeCfg();
        Assert.Null(CustomActorRun.Compute(Ri(0, 999, in cfg)));
    }

    [Fact]
    public void Run_NotUsingMagicLeavesEffectStateOnly()
    {
        var cfg = MakeCfg();
        var p = CustomActorRun.Compute(Ri(-1, 156, in cfg))!;

        Assert.False(p.UseMagicAfter);
        Assert.False(p.MagicBranchTaken);
        Assert.False(p.EffectSpawned);
        Assert.False(p.EarlyExit);
    }

    [Fact]
    public void Run_EffectAdvancesUntilEndThenStops()
    {
        var cfg = MakeCfg();

        // 811：TimeGetTime - start > frameTime（严格大于）→ 推进
        var adv = CustomActorRun.Compute(Ri(-1, 156, in cfg,
            useEffect: true, effectFrameTime: 100, effectStartTime: 50, now: 200,
            effectFrame: 2, effectEnd: 5))!;
        Assert.True(adv.UseEffect);
        Assert.True(adv.EffectAdvanced);
        Assert.Equal(3, adv.EffectFrame);

        // 813-817：已到 end → 关掉 m_boUseEffect（帧号不再变）
        var end = CustomActorRun.Compute(Ri(-1, 156, in cfg,
            useEffect: true, effectFrameTime: 100, effectStartTime: 50, now: 200,
            effectFrame: 5, effectEnd: 5))!;
        Assert.False(end.UseEffect);
        Assert.Equal(5, end.EffectFrame);
    }

    [Fact]
    public void Run_EffectTimeoutIsStrictlyGreater()
    {
        var cfg = MakeCfg();
        // now - start == frameTime 恰好相等 → **不**推进
        var eq = CustomActorRun.Compute(Ri(-1, 156, in cfg,
            useEffect: true, effectFrameTime: 100, effectStartTime: 50, now: 150,
            effectFrame: 2, effectEnd: 5))!;
        Assert.False(eq.EffectAdvanced);

        var gt = CustomActorRun.Compute(Ri(-1, 156, in cfg,
            useEffect: true, effectFrameTime: 100, effectStartTime: 50, now: 151,
            effectFrame: 2, effectEnd: 5))!;
        Assert.True(gt.EffectAdvanced);
    }

    [Fact]
    public void Run_WrongFrameClearsCreateEffect()
    {
        var cfg = MakeCfg();
        // 827 为假 → 1033-1035 的 else → m_boCreateEffect := False
        var p = CustomActorRun.Compute(Ri(-1, 156, in cfg,
            useMagic: true, curEffFrame: 0, spellFrame: 5, createEffect: true))!;

        Assert.True(p.MagicBranchTaken == false);
        Assert.True(p.ResetCreateEffect);
        Assert.False(p.CreateEffect);
    }

    [Fact]
    public void Run_AlreadyCreatedEffectDoesNothing()
    {
        var cfg = MakeCfg();
        // 828 为假（m_boCreateEffect 已 True）→ 整体什么都不做，CreateEffect 保持 True
        var p = CustomActorRun.Compute(Ri(-1, 156, in cfg,
            useMagic: true, curEffFrame: 4, spellFrame: 5, createEffect: true,
            effectNumber: -CustomActorAttackActions.SM_ATTACK01))!;

        Assert.True(p.MagicBranchTaken);
        Assert.True(p.CreateEffect);
        Assert.False(p.EffectSpawned);
    }

    [Fact]
    public void Run_UnknownEffectNumberExitsEarly()
    {
        var cfg = MakeCfg();
        // 830-839 未命中任何攻击号 → ClientConfig 仍 nil → 842 Exit
        var p = CustomActorRun.Compute(Ri(-1, 156, in cfg,
            useMagic: true, curEffFrame: 4, spellFrame: 5, createEffect: false,
            effectNumber: 12345))!;

        Assert.True(p.EarlyExit);
        Assert.True(p.CreateEffect);       // 829 已先置 True
        Assert.False(p.EffectSpawned);
    }

    [Fact]
    public void Run_NoFlyBranchBuildsTargetPlan()
    {
        var cfg = MakeCfg();
        cfg.AttackConfigs[0] = new TClientAttackConfig
        {
            Fly_StartIndex = -1,            // 844 前半为真 → 目标分支
            Fly_PlayCount = 0,
            Target_StartIndex = 10, Target_StartIndex2 = 20, Target_PlayCount = 4,
            Target_PlayTime = 80, Target_LightRange = 3,
            Target_DrawMode = TCustomDrawMode.mdmBlend, Target_DrawMode2 = TCustomDrawMode.mdmNormal,
            Target_KeepPlay = 0,
        };

        var p = CustomActorRun.Compute(Ri(-1, 156, in cfg,
            useMagic: true, curEffFrame: 4, spellFrame: 5, createEffect: false,
            effectNumber: -CustomActorAttackActions.SM_ATTACK01))!;

        Assert.False(p.EarlyExit);
        Assert.True(p.EffectSpawned);
        Assert.NotNull(p.Target);
        Assert.Null(p.Fly);
        Assert.True(p.Target!.Applies);
        Assert.Equal(10, p.Target.StartIndex);
        Assert.Equal(20, p.Target.StartIndex2);
        Assert.Equal(4, p.Target.PlayCount);
        Assert.Equal(80, p.Target.PlayTime);
        Assert.Equal(3, p.Target.LightRange);
    }

    [Fact]
    public void Run_TargetKeepPlayWithNonZeroKeepTimeSuppressesTarget()
    {
        var cfg = MakeCfg();
        cfg.AttackConfigs[0] = new TClientAttackConfig
        {
            Fly_StartIndex = -1, Fly_PlayCount = 0,
            Target_StartIndex = 10, Target_PlayCount = 4,
            Target_KeepPlay = 1, Target_KeepTime = 500,   // 847：持续播放且时间非 0 → 三门为假
        };

        var p = CustomActorRun.Compute(Ri(-1, 156, in cfg,
            useMagic: true, curEffFrame: 4, spellFrame: 5,
            effectNumber: -CustomActorAttackActions.SM_ATTACK01))!;

        Assert.False(p.EffectSpawned);
        Assert.False(p.Target!.Applies);
    }

    [Fact]
    public void Run_TargetKeepPlayWithZeroKeepTimeStillApplies()
    {
        var cfg = MakeCfg();
        cfg.AttackConfigs[0] = new TClientAttackConfig
        {
            Fly_StartIndex = -1, Fly_PlayCount = 0,
            Target_StartIndex = 10, Target_PlayCount = 4,
            Target_KeepPlay = 1, Target_KeepTime = 0,     // 847：KeepTime = 0 时仍允许
        };

        var p = CustomActorRun.Compute(Ri(-1, 156, in cfg,
            useMagic: true, curEffFrame: 4, spellFrame: 5,
            effectNumber: -CustomActorAttackActions.SM_ATTACK01))!;

        Assert.True(p.Target!.Applies);
    }

    [Fact]
    public void Run_FlyBranchBuildsFlyPlan()
    {
        var cfg = MakeCfg();
        cfg.AttackConfigs[0] = new TClientAttackConfig
        {
            Fly_StartIndex = 30, Fly_PlayCount = 6, Fly_EmptyCount = 1,
            Fly_PlayTime = 60, Fly_LightRange = 2, Fly_CalcDir = 1,
            Fly_DirCount = TCustomDirCount.mdcDir8,
            Explosion_StartIndex = 40, Explosion_StartIndex2 = 41, Explosion_PlayCount = 5,
            Explosion_PlayTime = 70, Explosion_LightRange = 4,
            Explosion_KeepPlay = 0, Explosion_KeepTime = 0,
            FlyEff_StartIndex = 50, FlyEff_DrawMode = TCustomDrawMode.mdmNormal,
        };

        var p = CustomActorRun.Compute(Ri(-1, 156, in cfg,
            useMagic: true, curEffFrame: 4, spellFrame: 5,
            effectNumber: -CustomActorAttackActions.SM_ATTACK01))!;

        Assert.False(p.EarlyExit);
        Assert.True(p.EffectSpawned);
        Assert.NotNull(p.Fly);
        Assert.Null(p.Target);
        Assert.Equal(30, p.Fly!.FlyStartIndex);
        Assert.Equal(6, p.Fly.FlyPlayCount);
        Assert.True(p.Fly.FlyCalcDir);
    }

    [Fact]
    public void Run_NoFlyBranchWinsWhenStartIndexIsNegative()
    {
        // 844：`(Fly_StartIndex < 0) or (Fly_PlayCount <= 0)` —— **或**关系。
        // Fly_StartIndex = -1 → 前半即真 → 走目标分支（844 优先于 941），
        // 即便 Fly_PlayCount 为正也不改变。
        var cfg = MakeCfg();
        cfg.AttackConfigs[0] = new TClientAttackConfig
        {
            Fly_StartIndex = -1, Fly_PlayCount = 3,
            Target_StartIndex = 10, Target_PlayCount = 4,
        };

        var p = CustomActorRun.Compute(Ri(-1, 156, in cfg,
            useMagic: true, curEffFrame: 4, spellFrame: 5,
            effectNumber: -CustomActorAttackActions.SM_ATTACK01))!;

        Assert.Null(p.Fly);
        Assert.NotNull(p.Target);
    }

    [Fact]
    public void Run_NoFlyConditionIsNotTheComplementOfFlyCondition()
    {
        // 差异断言：844 是 `(Start < 0) or (Play <= 0)`；941 是 `(Start >= 0) or (Play > 0)`。
        // 当 (Start >= 0 且 Play <= 0) 时：844 为假（False or True = True → 其实是真）。
        // 精确取 (Start = -1, Play = -5 视作 0)：844 = (-1<0)=True → 目标分支；
        // 故要构造 844 为假必须 Start >= 0 且 Play > 0，此时 941 也必为真。
        // 真正的缝隙在 Start >= 0 且 Play > 0 → 两分支都"可入"，由 844 优先。
        var cfg = MakeCfg();
        cfg.AttackConfigs[0] = new TClientAttackConfig
        {
            Fly_StartIndex = 5, Fly_PlayCount = 4,       // 844: False or False = False → 不取目标分支
            Target_StartIndex = 10, Target_PlayCount = 4,
        };

        var p = CustomActorRun.Compute(Ri(-1, 156, in cfg,
            useMagic: true, curEffFrame: 4, spellFrame: 5,
            effectNumber: -CustomActorAttackActions.SM_ATTACK01))!;

        // 941 为真 → 走飞行；即"两分支同时可入时 844 优先"
        Assert.NotNull(p.Fly);
        Assert.Null(p.Target);
    }

    [Fact]
    public void FlyDir_ZeroWhenCalcDirFalse()
    {
        Assert.Equal(0, CustomActorRun.FlyDir(false, 0, 0, 0, 9, 9));
    }

    [Fact]
    public void FlyDir_EightVersusSixteen()
    {
        bool used8 = false, used16 = false;
        CustomActorEnv.GetFlyDirectionFn = (_, _, _, _) => { used8 = true; return 2; };
        CustomActorEnv.GetFlyDirection16Fn = (_, _, _, _) => { used16 = true; return 11; };

        Assert.Equal(2, CustomActorRun.FlyDir(true, 0, 0, 0, 1, 1));
        Assert.True(used8);
        Assert.False(used16);

        Assert.Equal(11, CustomActorRun.FlyDir(true, 1, 0, 0, 1, 1));
        Assert.True(used16);
    }

    [Fact]
    public void FlyBaseFrame_And_FlyEffStartFrameShareStride()
    {
        var p = new CustomActorFlyEffectPlan(true, 30, 6, 1, 60, 0, 2, 0, false, 0,
            40, 41, 5, 70, 0, 0, false, 4, false, 0, 0, 50, 3, 1);

        Assert.Equal(30 + 3 * 7, CustomActorRun.FlyBaseFrame(in p, 3));
        // 1008：FlyEff_StartIndex 也按 Fly_PlayCount + Fly_EmptyCount 步进
        Assert.Equal(50 + 3 * 7, CustomActorRun.FlyEffStartFrame(in p, 3));
    }

    [Fact]
    public void ExplosionFrames_KeepPlayOverridesToMinusOne()
    {
        var keep = new CustomActorFlyEffectPlan(true, 30, 6, 1, 60, 0, 2, 0, false, 0,
            40, 41, 5, 70, 0, 0, false, 4, true, 500, 0, 50, 3, 1);
        Assert.Equal((-1, -1, 0), CustomActorRun.ExplosionFrames(in keep));

        var once = new CustomActorFlyEffectPlan(true, 30, 6, 1, 60, 0, 2, 0, false, 0,
            40, 41, 5, 70, 0, 0, false, 4, true, 0, 0, 50, 3, 1);
        Assert.Equal((40, 41, 5), CustomActorRun.ExplosionFrames(in once));
    }

    [Fact]
    public void ExplosionBlends_BlendOnlyForMdmBlend()
    {
        var p = new CustomActorFlyEffectPlan(true, 30, 6, 1, 60, 0, 2, 0, false, 0,
            40, 41, 5, 70, 0 /*mdmBlend*/, 1 /*mdmNormal*/, false, 4, false, 0, 0, 50, 3, 1);
        Assert.Equal((true, false), CustomActorRun.ExplosionBlends(in p));
    }

    // ===================== m_Saying 多目标（912-938）=====================

    [Fact]
    public void SayingTargets_EmptySayingYieldsNothing()
        => Assert.Empty(CustomActorRunContext.SayingTargets(""));

    [Fact]
    public void SayingTargets_SplitsTrimsAndDropsZero()
    {
        var t = CustomActorRunContext.SayingTargets(" 11 , 0 ,22,abc,33 ");
        Assert.Equal(new List<long> { 11, 22, 33 }, t);
    }

    [Fact]
    public void SayingTargets_NegativeIdsAreKept()
    {
        // 919：判据是 `<> 0`，故负数**保留**
        var t = CustomActorRunContext.SayingTargets("-5,0,7");
        Assert.Equal(new List<long> { -5, 7 }, t);
    }

    [Fact]
    public void StrToInt64Def_ReturnsDefaultOnGarbage()
    {
        Assert.Equal(0L, CustomActorRunContext.StrToInt64Def("xyz", 0));
        Assert.Equal(99L, CustomActorRunContext.StrToInt64Def("", 99));
        Assert.Equal(42L, CustomActorRunContext.StrToInt64Def(" 42 ", 0));
    }

    // ===================== RunSound / RunActSound（1039-1130）=====================

    private static TClientCustomMonsterConfig MakeSoundCfg()
    {
        var cfg = MakeCfg();
        cfg.BaseConfig.Sounds[(int)TMonsterSoundType.mstNormal] = new ShortStr30 { Value = "s_normal" };
        cfg.BaseConfig.Sounds[(int)TMonsterSoundType.mstAttack] = new ShortStr30 { Value = "s_attack" };
        cfg.BaseConfig.Sounds[(int)TMonsterSoundType.mstAttack1] = new ShortStr30 { Value = "s_atk1" };
        cfg.BaseConfig.Sounds[(int)TMonsterSoundType.mstAttack6] = new ShortStr30 { Value = "s_atk6" };
        cfg.BaseConfig.Sounds[(int)TMonsterSoundType.mstStruck] = new ShortStr30 { Value = "s_struck" };
        cfg.BaseConfig.Sounds[(int)TMonsterSoundType.mstDigUP] = new ShortStr30 { Value = "s_digup" };
        cfg.BaseConfig.Sounds[(int)TMonsterSoundType.mstDie] = new ShortStr30 { Value = "s_die" };
        return cfg;
    }

    [Fact]
    public void RunSound_BypassGateReturnsNull()
    {
        var cfg = MakeSoundCfg();
        Assert.Null(CustomActorSound.RunSound(In(999, 0, TActorCore.SM_STRUCK, in cfg), 5));
    }

    [Fact]
    public void RunSound_StruckAddsWeaponSoundOnlyWhenNonNegative()
    {
        var cfg = MakeSoundCfg();

        var withWeapon = CustomActorSound.RunSound(
            In(156, -1, TActorCore.SM_STRUCK, in cfg), 123)!.Value;
        Assert.Equal(2, withWeapon.Sounds.Count);
        Assert.Equal("s_struck", withWeapon.Sounds[0]);
        Assert.Contains("123", withWeapon.Sounds[1]);

        var noWeapon = CustomActorSound.RunSound(
            In(156, -1, TActorCore.SM_STRUCK, in cfg), -1)!.Value;
        Assert.Single(noWeapon.Sounds);
        Assert.Equal("s_struck", noWeapon.Sounds[0]);
    }

    [Fact]
    public void RunSound_DigUpPlaysAppear()
    {
        var cfg = MakeSoundCfg();
        var r = CustomActorSound.RunSound(In(156, -1, TActorCore.SM_DIGUP, in cfg), -1)!.Value;
        Assert.Single(r.Sounds);
        Assert.Equal("s_digup", r.Sounds[0]);
    }

    [Fact]
    public void RunSound_OtherActionsProduceNoSound()
    {
        var cfg = MakeSoundCfg();
        foreach (int a in new[] { TActorCore.SM_TURN, TActorCore.SM_WALK, TActorCore.SM_DEATH, 0 })
        {
            var r = CustomActorSound.RunSound(In(156, -1, a, in cfg), -1)!.Value;
            Assert.Empty(r.Sounds);
        }
    }

    [Fact]
    public void RunSound_ReturnsNullOnlyForBypassNotEmptyResult()
    {
        // 差异断言：前置门返回 null；命中门但无声音时返回**空列表**（非 null）
        var cfg = MakeSoundCfg();
        Assert.Null(CustomActorSound.RunSound(In(5, 0, TActorCore.SM_STRUCK, in cfg), -1));
        Assert.NotNull(CustomActorSound.RunSound(In(156, -1, TActorCore.SM_STRUCK, in cfg), -1));
    }

    [Fact]
    public void RunActSound_ExitsWhenRunSoundFlagFalse()
    {
        var cfg = MakeSoundCfg();
        // 1043 先于前置门：m_boRunSound 为假 → null
        Assert.Null(CustomActorSound.RunActSound(
            In(156, -1, TActorCore.SM_HIT, in cfg), boRunSound: false, frame: 3));
    }

    [Fact]
    public void RunActSound_BypassGateReturnsNull()
    {
        var cfg = MakeSoundCfg();
        Assert.Null(CustomActorSound.RunActSound(
            In(999, 0, TActorCore.SM_HIT, in cfg), boRunSound: true, frame: 3));
    }

    [Fact]
    public void RunActSound_HitOnlyOnFrameThree()
    {
        var cfg = MakeSoundCfg();

        var at3 = CustomActorSound.RunActSound(
            In(156, -1, TActorCore.SM_HIT, in cfg), true, 3)!.Value;
        Assert.Equal("s_attack", at3.Sound);
        Assert.True(at3.CloseRunSound);           // 1060

        var at2 = CustomActorSound.RunActSound(
            In(156, -1, TActorCore.SM_HIT, in cfg), true, 2)!.Value;
        Assert.Equal("", at2.Sound);
        Assert.False(at2.CloseRunSound);
    }

    [Theory]
    [InlineData(CustomActorAttackActions.SM_ATTACK01, "s_atk1")]
    [InlineData(CustomActorAttackActions.SM_ATTACK06, "s_atk6")]
    public void RunActSound_AttackSlotsUseOwnSoundOnFrameThree(int action, string expected)
    {
        var cfg = MakeSoundCfg();
        var r = CustomActorSound.RunActSound(In(156, -1, action, in cfg), true, 3)!.Value;
        Assert.Equal(expected, r.Sound);
        Assert.True(r.CloseRunSound);
    }

    [Fact]
    public void RunActSound_TurnRequiresFrameOneAndRandomHit()
    {
        var cfg = MakeSoundCfg();

        // Random(8) = 1 → 播
        CustomActorEnv.RandomFn = _ => 1;
        var hit = CustomActorSound.RunActSound(
            In(156, -1, TActorCore.SM_TURN, in cfg), true, 1)!.Value;
        Assert.Equal("s_normal", hit.Sound);
        Assert.True(hit.CloseRunSound);

        // Random(8) <> 1 → 不播
        CustomActorEnv.RandomFn = _ => 2;
        var miss = CustomActorSound.RunActSound(
            In(156, -1, TActorCore.SM_TURN, in cfg), true, 1)!.Value;
        Assert.Equal("", miss.Sound);

        // 帧不为 1 → 即便随机命中也不播
        CustomActorEnv.RandomFn = _ => 1;
        var wrongFrame = CustomActorSound.RunActSound(
            In(156, -1, TActorCore.SM_TURN, in cfg), true, 2)!.Value;
        Assert.Equal("", wrongFrame.Sound);
    }

    [Fact]
    public void RunActSound_NowDeathOnlyOnFrameTwo()
    {
        var cfg = MakeSoundCfg();

        var at2 = CustomActorSound.RunActSound(
            In(156, -1, TActorCore.SM_NOWDEATH, in cfg), true, 2)!.Value;
        Assert.Equal("s_die", at2.Sound);

        var at3 = CustomActorSound.RunActSound(
            In(156, -1, TActorCore.SM_NOWDEATH, in cfg), true, 3)!.Value;
        Assert.Equal("", at3.Sound);
    }

    [Fact]
    public void RunActSound_UnlistedActionYieldsEmptyButNotNull()
    {
        var cfg = MakeSoundCfg();
        var r = CustomActorSound.RunActSound(
            In(156, -1, TActorCore.SM_WALK, in cfg), true, 3)!.Value;
        Assert.Equal("", r.Sound);
        Assert.False(r.CloseRunSound);
    }

    [Fact]
    public void HitAndAttackSoundFramesDifferFromNowDeath()
    {
        // 差异断言：受击/攻击用第 3 帧，现死用第 2 帧，转身用第 1 帧
        var cfg = MakeSoundCfg();

        Assert.Equal("s_attack", CustomActorSound.RunActSound(
            In(156, -1, TActorCore.SM_HIT, in cfg), true, 3)!.Value.Sound);
        Assert.Equal("", CustomActorSound.RunActSound(
            In(156, -1, TActorCore.SM_HIT, in cfg), true, 2)!.Value.Sound);

        Assert.Equal("s_die", CustomActorSound.RunActSound(
            In(156, -1, TActorCore.SM_NOWDEATH, in cfg), true, 2)!.Value.Sound);
        Assert.Equal("", CustomActorSound.RunActSound(
            In(156, -1, TActorCore.SM_NOWDEATH, in cfg), true, 3)!.Value.Sound);
    }

    // ===================== TCustomActor 类本体 =====================

    [Fact]
    public void ActorCtor_MirrorsSourceInitialisation()
    {
        var cfg = MakeCfg();
        var a = new TCustomActor(cfg);

        // 64-69
        Assert.Equal(cfg.Actions[(int)TMonsterClientActionType.matStand].StartIndex,
            a.Config.Actions[(int)TMonsterClientActionType.matStand].StartIndex);
        Assert.Null(a.m_BodySurface);
        Assert.Equal(1, a.m_nChrLight);          // 68：不是 m_nOldChrLight
        Assert.False(a.m_boCreateEffect);        // 69
        Assert.Equal("TCustomActor", a.ActorClass);
    }

    [Fact]
    public void ActorCtor_ChrLightIsOneNotOldChrLight()
    {
        // 差异断言：68 写 m_nChrLight，m_nOldChrLight 保持默认 0
        var a = new TCustomActor(MakeCfg());
        Assert.Equal(1, a.m_nChrLight);
        Assert.Equal(0, a.m_nOldChrLight);
    }

    [Fact]
    public void ActorCalcActorFrame_StandWritesState()
    {
        var cfg = MakeCfg();
        var a = new TCustomActor(cfg)
        {
            m_nChangeAppr = -1,
            m_btRace = 156,
            m_nCurrentAction = TActorCore.SM_TURN,
            m_btDir = 2,
            m_nOldChrLight = 5,
        };
        CustomActorEnv.TimeGetTimeFn = () => 4242;   // TCustomActor 经 CustomActorEnv 取时钟

        a.CalcActorFrame();

        Assert.False(a.m_boUseMagic);
        Assert.Equal(-1, a.m_nCurrentFrame);
        Assert.Equal(0, a.m_nBodyOffset);
        Assert.Equal(5, a.m_nChrLight);
        Assert.Equal(100 + 2 * 6, a.m_nStartFrame);
        Assert.Equal(100 + 2 * 6 + 3, a.m_nEndFrame);
        Assert.Equal(4, a.m_nDefFrameCount);
        Assert.Equal(4242u, a.m_dwStartTime);
        Assert.Equal(TMonsterClientActionType.matStand, a.ClientAction);
    }

    [Fact]
    public void ActorCalcActorFrame_StruckResetsStruckCounterAndUsesStruckFrameTime()
    {
        var cfg = MakeCfg();
        var a = new TCustomActor(cfg)
        {
            m_nChangeAppr = -1, m_btRace = 156,
            m_nCurrentAction = TActorCore.SM_STRUCK,
            m_dwStruckFrameTime = 275,
            m_CustomMagicStatusEffect_Struck = 7,
        };

        a.CalcActorFrame();

        Assert.Equal(275u, a.m_dwFrameTime);
        Assert.Equal(0, a.m_CustomMagicStatusEffect_Struck);
    }

    [Fact]
    public void ActorGetDefaultFrame_UsesConfig()
    {
        var cfg = MakeCfg();
        var a = new TCustomActor(cfg)
        {
            m_nChangeAppr = -1, m_btRace = 156, m_btDir = 3,
            m_nCurrentDefFrame = 2, m_nOldChrLight = 6,
        };

        Assert.Equal(100 + 3 * 6 + 2, a.GetDefaultFrame(false));
        Assert.Equal(6, a.m_nChrLight);
        Assert.Equal(TMonsterClientActionType.matStand, a.ClientAction);
    }

    [Fact]
    public void ActorLoadSurface_CanvasNotReadyLeavesSurfacesUntouched()
    {
        var cfg = MakeCfg();
        var a = new TCustomActor(cfg) { m_nChangeAppr = -1, m_btRace = 156 };
        a.m_BodySurface = "sentinel";

        CustomActorEnv.GameCanvasActive = false;
        a.LoadSurface(null);

        Assert.Equal("sentinel", a.m_BodySurface);   // 414 提前 Exit，不清表面
    }

    [Fact]
    public void ActorLoadSurface_NullablePlanActuallyComesBackForReadyCanvas()
    {
        var cfg = MakeCfg();
        var a = new TCustomActor(cfg) { m_nChangeAppr = -1, m_btRace = 156 };

        int fetches = 0;
        CustomActorEnv.FetchSurfaceFn = (_, _, _, _, kind) => { fetches++; return "s"; };

        a.LoadSurface(null);

        Assert.True(fetches > 0);
        Assert.False(a.m_boLoadSurface);
    }

    [Fact]
    public void ActorDrawChr_BypassGateDoesNotInvokeSeam()
    {
        var cfg = MakeCfg();
        var a = new TCustomActor(cfg) { m_nChangeAppr = 5, m_btRace = 99 };

        int draws = 0;
        CustomActorEnv.DrawSurfaceFn = (_, _, _, _) => draws++;

        a.DrawChr(0, 0, false, false);
        Assert.Equal(0, draws);
    }

    [Fact]
    public void ActorRunSound_PlaysConfiguredSounds()
    {
        var cfg = MakeSoundCfg();
        var a = new TCustomActor(cfg)
        {
            m_nChangeAppr = -1, m_btRace = 156,
            m_nCurrentAction = TActorCore.SM_DIGUP,
        };

        var played = new List<string>();
        CustomActorEnv.PlaySoundFn = s => played.Add(s);

        a.RunSound();

        Assert.True(a.m_boRunSound);            // 1117
        Assert.Equal(new[] { "s_digup" }, played);
    }

    [Fact]
    public void ActorRunActSound_ClosesFlagAfterPlay()
    {
        var cfg = MakeSoundCfg();
        var a = new TCustomActor(cfg)
        {
            m_nChangeAppr = -1, m_btRace = 156,
            m_nCurrentAction = TActorCore.SM_HIT,
            m_boRunSound = true,
        };

        var played = new List<string>();
        CustomActorEnv.PlaySoundFn = s => played.Add(s);

        a.RunActSound(3);

        Assert.False(a.m_boRunSound);           // 1060
        Assert.Equal(new[] { "s_attack" }, played);
    }

    [Fact]
    public void ActorRunActSound_DoesNothingWhenFlagAlreadyFalse()
    {
        var cfg = MakeSoundCfg();
        var a = new TCustomActor(cfg)
        {
            m_nChangeAppr = -1, m_btRace = 156,
            m_nCurrentAction = TActorCore.SM_HIT,
            m_boRunSound = false,
        };

        var played = new List<string>();
        CustomActorEnv.PlaySoundFn = s => played.Add(s);

        a.RunActSound(3);
        Assert.Empty(played);
    }

    // ===================== 常量核对 =====================

    [Fact]
    public void AttackActionConstantsMatchGrobal2()
    {
        Assert.Equal(8946, CustomActorAttackActions.SM_ATTACK01);
        Assert.Equal(8947, CustomActorAttackActions.SM_ATTACK02);
        Assert.Equal(8948, CustomActorAttackActions.SM_ATTACK03);
        Assert.Equal(8949, CustomActorAttackActions.SM_ATTACK04);
        Assert.Equal(8950, CustomActorAttackActions.SM_ATTACK05);
        Assert.Equal(8951, CustomActorAttackActions.SM_ATTACK06);
    }

    [Fact]
    public void AttackSlotTableIsOrderedAndMatchesConstants()
    {
        Assert.Equal(6, CustomActorAttackSlot.AttackActions.Length);
        for (int i = 0; i < 6; i++)
        {
            Assert.Equal(8946 + i, CustomActorAttackSlot.AttackActions[i]);
            Assert.Equal(i, CustomActorAttackSlot.SlotOf(CustomActorAttackSlot.AttackActions[i]));
        }
        Assert.Equal(-1, CustomActorAttackSlot.SlotOf(TActorCore.SM_HIT));
    }

    [Fact]
    public void CustomMonsterRaceIs156()
        => Assert.Equal(156, CustomActorGate.CustomMonsterRace);

}
