using System;
using System.Collections.Generic;
using GXX.Client.Scenes;
using GXX.Core.Protocol;
using Xunit;

using TChrMsg = GXX.Client.Scenes.TChrMsg;

namespace GXX.Client.Tests;

/// <summary>
/// 批次J74：Actor.pas 姿态/动作决策层（ActorMotion.cs）1:1 行为锁定。
/// 覆盖 IsIdle/ActionFinished/GetNextHitTime/CanWalk/CanRun/Strucked/
/// GetDefaultFrame 双分支/DefaultMotion/RunFrameAction/ActionChanged/ActionEnded/
/// CanCancelAction/CancelAction/CleanCharMapSetting/MoveFail/CleanUserMsgs。
/// </summary>
public sealed class ActorMotionTests : IDisposable
{
    private readonly List<Action> _restore = new();

    public ActorMotionTests()
    {
        // 全局接缝快照
        var saved = (TActorCore.ClientConfig_dwHitFrameTime, TActorCore.ClientConfig_dwIncSpeedDecInterval,
            TActorCore.ClientConfig_boAttackSlow, TActorCore.ChrAction, TActorCore.boMapMovingWait,
            TActorCore.LatestSpellTick, TActorCore.MagicPKDelayTime, TActorCore.MyGetTickCountFn,
            TActorCore.StartContinuousMagicAttackFn, TActorCore.ActionCode,
            TActorCore.SetCanDrawTileMapFn, TActorCore.MySelfRef,
            TActorCore.CustomMonsterConfigResolver);

        _restore.Add(() =>
        {
            TActorCore.ClientConfig_dwHitFrameTime = saved.Item1;
            TActorCore.ClientConfig_dwIncSpeedDecInterval = saved.Item2;
            TActorCore.ClientConfig_boAttackSlow = saved.Item3;
            TActorCore.ChrAction = saved.Item4;
            TActorCore.boMapMovingWait = saved.Item5;
            TActorCore.LatestSpellTick = saved.Item6;
            TActorCore.MagicPKDelayTime = saved.Item7;
            TActorCore.MyGetTickCountFn = saved.Item8;
            TActorCore.StartContinuousMagicAttackFn = saved.Item9;
            TActorCore.ActionCode = saved.Item10;
            TActorCore.SetCanDrawTileMapFn = saved.Item11;
            TActorCore.MySelfRef = saved.Item12;
            TActorCore.CustomMonsterConfigResolver = saved.Item13;
        });

        // 默认值复位（各测试自行覆盖）
        TActorCore.ChrAction = 1;
        TActorCore.boMapMovingWait = false;
        TActorCore.LatestSpellTick = 0;
        TActorCore.MagicPKDelayTime = 0;
        TActorCore.ClientConfig_boAttackSlow = false;
        TActorCore.MyGetTickCountFn = () => 1000;
    }

    public void Dispose()
    {
        foreach (var r in _restore)
            r();
    }

    private static TActor MakeActor(long id = 1, int race = 50, int appr = 0)
        => new TActor { m_nRecogId = id, m_btRace = (byte)race, m_wAppearance = (ushort)appr };

    // ===================== IsIdle / ActionFinished =====================

    [Fact]
    public void ComputeIsIdle_RequiresNoActionAndEmptyQueue()
    {
        var a = MakeActor();
        Assert.True(a.ComputeIsIdle());

        a.m_nCurrentAction = TActorCore.SM_TURN;
        Assert.False(a.ComputeIsIdle());

        a.m_nCurrentAction = 0;
        a.SendMsg(new TChrMsg { Ident = TActorCore.SM_WALK });
        Assert.False(a.ComputeIsIdle());
    }

    [Fact]
    public void ActionFinished_NoActionOrNearEndFrame()
    {
        var a = MakeActor();
        Assert.True(a.ActionFinished());              // 无动作

        a.m_nCurrentAction = TActorCore.SM_HIT;
        a.m_nStartFrame = 10;
        a.m_nEndFrame = 20;

        a.m_nCurrentFrame = 10;
        Assert.False(a.ActionFinished());             // 10 >= 19? 否

        a.m_nCurrentFrame = 19;
        Assert.True(a.ActionFinished());              // 19 >= 20-1

        a.m_nCurrentFrame = 25;
        Assert.True(a.ActionFinished());              // 越过末帧也算结束
    }

    // ===================== GetNextHitTime =====================

    [Fact]
    public void GetNextHitTime_ClampsToMinimumHundred()
    {
        TActorCore.ClientConfig_dwHitFrameTime = 0;
        var a = MakeActor();
        Assert.Equal(100, a.GetNextHitTime());

        TActorCore.ClientConfig_dwHitFrameTime = 50;
        Assert.Equal(100, a.GetNextHitTime());

        TActorCore.ClientConfig_dwHitFrameTime = 420;
        Assert.Equal(420, a.GetNextHitTime());
    }

    [Fact]
    public void GetNextHitTime_NegativeConfigFoldsToZero()
    {
        TActorCore.ClientConfig_dwHitFrameTime = -5000;
        var a = MakeActor();
        Assert.Equal(100, a.GetNextHitTime());
    }

    [Fact]
    public void GetNextHitTime_AttackSlowOnlyAppliesToSelf()
    {
        TActorCore.ClientConfig_dwHitFrameTime = 500;
        TActorCore.ClientConfig_boAttackSlow = true;

        var other = MakeActor(2);
        TActorCore.MySelfRef = null;
        Assert.Equal(500, other.GetNextHitTime());     // 非主角不加

        TActorCore.MySelfRef = other;
        Assert.Equal(2000, other.GetNextHitTime());    // 500 + 1500
    }

    [Fact]
    public void GetNextHitTime_SpeedReductionUsesInterval()
    {
        TActorCore.ClientConfig_dwHitFrameTime = 1000;
        TActorCore.ClientConfig_dwIncSpeedDecInterval = 50;

        var a = MakeActor();
        a.m_nAttackSpeed = 4;                          // 1000 - 4*50 = 800
        Assert.Equal(800, a.GetNextHitTime());

        a.m_nAttackSpeed = 30;                         // 1000 - 1500 → Max 0 → 下限 100
        Assert.Equal(100, a.GetNextHitTime());
    }

    [Fact]
    public void GetNextHitTime_ZeroAttackSpeedSkipsReduction()
    {
        TActorCore.ClientConfig_dwHitFrameTime = 900;
        TActorCore.ClientConfig_dwIncSpeedDecInterval = 50;
        var a = MakeActor();
        a.m_nAttackSpeed = 0;
        Assert.Equal(900, a.GetNextHitTime());
    }

    // ===================== CanWalk / CanRun =====================

    [Fact]
    public void CanWalk_ReturnsOneWhenAllGatesOpen()
    {
        TActorCore.MyGetTickCountFn = () => 10000;
        TActorCore.LatestSpellTick = 0;
        TActorCore.MagicPKDelayTime = 1300;
        TActorCore.ChrAction = 1;
        TActorCore.boMapMovingWait = false;

        var a = MakeActor();
        Assert.Equal(1, a.CanWalk());
    }

    [Fact]
    public void CanWalk_BlocksInsideMagicPkDelay()
    {
        TActorCore.MyGetTickCountFn = () => 1000;
        TActorCore.LatestSpellTick = 200;
        TActorCore.MagicPKDelayTime = 1300;             // 1000-200 = 800 < 1300

        var a = MakeActor();
        Assert.Equal(-1, a.CanWalk());
    }

    [Fact]
    public void CanWalk_BlocksOnCaNoneAndMapMovingWait()
    {
        TActorCore.ChrAction = TActorCore.caNone;
        var a = MakeActor();
        Assert.Equal(-1, a.CanWalk());

        TActorCore.ChrAction = 1;
        TActorCore.boMapMovingWait = true;
        Assert.Equal(-1, a.CanWalk());
    }

    [Fact]
    public void CanWalk_SkillLockBitOverridesOpenGates()
    {
        TActorCore.ChrAction = 1;
        TActorCore.boMapMovingWait = false;
        TActorCore.MagicPKDelayTime = 0;

        var a = MakeActor();
        a.m_nState = TActorCore.STATE_SKILL_LOCK;
        Assert.Equal(-1, a.CanWalk());
    }

    [Fact]
    public void CanRun_HpBelowMinimumBlocks()
    {
        var a = MakeActor();
        a.m_nAbilHP = TActorCore.RUN_MINHEALTH - 1;
        Assert.Equal(-1, a.CanRun());

        a.m_nAbilHP = TActorCore.RUN_MINHEALTH;
        Assert.Equal(1, a.CanRun());                    // 等于下限允许
    }

    [Fact]
    public void CanRun_CaNoneAndMapMovingWaitBlock()
    {
        var a = MakeActor();
        a.m_nAbilHP = 500;

        TActorCore.ChrAction = TActorCore.caNone;
        Assert.Equal(-1, a.CanRun());

        TActorCore.ChrAction = 1;
        TActorCore.boMapMovingWait = true;
        Assert.Equal(-1, a.CanRun());
    }

    [Fact]
    public void CanRun_SkillLockBitOverridesHp()
    {
        var a = MakeActor();
        a.m_nAbilHP = 500;
        a.m_nState = TActorCore.STATE_SKILL_LOCK;
        Assert.Equal(-1, a.CanRun());
    }

    // ===================== Strucked =====================

    [Fact]
    public void Strucked_ScansQueueForStruckIdent()
    {
        var a = MakeActor();
        Assert.False(a.Strucked());

        a.SendMsg(new TChrMsg { Ident = TActorCore.SM_WALK });
        a.SendMsg(new TChrMsg { Ident = TActorCore.SM_STRUCK });
        Assert.True(a.Strucked());

        a.MsgList.Clear();
        a.SendMsg(new TChrMsg { Ident = TActorCore.SM_DEATH });
        Assert.False(a.Strucked());
    }

    // ===================== GetDefaultFrame：动作表分支 =====================

    [Fact]
    public void GetDefaultFrame_StandUsesDirOffsetAndZeroesOutOfRangeDefFrame()
    {
        var a = MakeActor(1, race: 50, appr: 0);
        a.m_btDir = 3;
        a.m_nCurrentDefFrame = -1;                      // <0 → cf = 0

        int expected = a.GetDefaultFrame(false);

        // 与直接查表结果对齐
        var pm = ActorActionTables.GetRaceByPM(50, 0)!.Value;
        Assert.Equal(pm.ActStand.start + 3 * (pm.ActStand.frame + pm.ActStand.skip), expected);
        Assert.Equal(pm.ActStand.frame, a.m_nDefFrameCount);
    }

    [Fact]
    public void GetDefaultFrame_DefFrameOutOfRangeFallsBackToZero()
    {
        var a = MakeActor(1, race: 50, appr: 0);
        var pm = ActorActionTables.GetRaceByPM(50, 0)!.Value;

        a.m_btDir = 0;
        a.m_nCurrentDefFrame = pm.ActStand.frame;       // >= frame → cf = 0
        Assert.Equal(pm.ActStand.start, a.GetDefaultFrame(false));

        a.m_nCurrentDefFrame = pm.ActStand.frame + 99;
        Assert.Equal(pm.ActStand.start, a.GetDefaultFrame(false));

        if (pm.ActStand.frame > 1)
        {
            a.m_nCurrentDefFrame = 1;                   // 区间内 → cf = 1
            Assert.Equal(pm.ActStand.start + 1, a.GetDefaultFrame(false));
        }
    }

    [Fact]
    public void GetDefaultFrame_DeathUsesLastDieFrame()
    {
        var a = MakeActor(1, race: 50, appr: 0);
        var pm = ActorActionTables.GetRaceByPM(50, 0)!.Value;

        a.m_boDeath = true;
        a.m_boSkeleton = false;
        a.m_btDir = 2;

        int expected = pm.ActDie.start + 2 * (pm.ActDie.frame + pm.ActDie.skip) + (pm.ActDie.frame - 1);
        Assert.Equal(expected, a.GetDefaultFrame(false));
    }

    [Fact]
    public void GetDefaultFrame_SkeletonUsesDeathTableStart()
    {
        var a = MakeActor(1, race: 50, appr: 0);
        var pm = ActorActionTables.GetRaceByPM(50, 0)!.Value;

        a.m_boDeath = true;
        a.m_boSkeleton = true;
        a.m_btDir = 5;

        Assert.Equal(pm.ActDeath.start, a.GetDefaultFrame(false));
        Assert.Equal(0, a.m_nDefFrameCount);            // 死亡分支不写 DefFrameCount
    }

    [Fact]
    public void GetDefaultFrame_UnmappedRaceFallsBackToMa19()
    {
        // GetRaceByPM 末行 `return MA19` → 未登记 race 不会返回 nil，
        // 因此原文的 `if pm = nil then Exit` 在实机上不可达（保留为防御分支）。
        var a = MakeActor(1, race: 250, appr: 0);
        a.m_btDir = 2;

        var pm19 = ActorActionTables.GetRaceByPM(19, 0)!.Value;
        var pm250 = ActorActionTables.GetRaceByPM(250, 0)!.Value;

        Assert.Equal(pm19.ActStand.start, pm250.ActStand.start);
        Assert.Equal(pm19.ActStand.start + 2 * (pm19.ActStand.frame + pm19.ActStand.skip), a.GetDefaultFrame(false));
        Assert.Equal(pm19.ActStand.frame, a.m_nDefFrameCount);   // 确实走了表分支并写了副作用
    }

    // ===================== GetDefaultFrame：自定义怪分支 =====================

    private static TClientCustomMonsterConfig MakeCustomMon(int standStart, int standPlay, int standEmpty,
        bool standCalcDir, int dieStart, int diePlay, int dieEmpty, bool dieCalcDir,
        int reviveStart, int revivePlay, int reviveEmpty, bool reviveCalcDir)
    {
        var cfg = new TClientCustomMonsterConfig();
        cfg.Actions[(int)TMonsterClientActionType.matStand] = new TMonsterClientAction
        {
            ActionType = TMonsterClientActionType.matStand,
            StartIndex = (short)standStart,
            PlayCount = (ushort)standPlay,
            EmptyCount = (ushort)standEmpty,
            CalcDir = (byte)(standCalcDir ? 1 : 0),
        };
        cfg.Actions[(int)TMonsterClientActionType.matDie] = new TMonsterClientAction
        {
            ActionType = TMonsterClientActionType.matDie,
            StartIndex = (short)dieStart,
            PlayCount = (ushort)diePlay,
            EmptyCount = (ushort)dieEmpty,
            CalcDir = (byte)(dieCalcDir ? 1 : 0),
        };
        cfg.Actions[(int)TMonsterClientActionType.matStoneRevive] = new TMonsterClientAction
        {
            ActionType = TMonsterClientActionType.matStoneRevive,
            StartIndex = (short)reviveStart,
            PlayCount = (ushort)revivePlay,
            EmptyCount = (ushort)reviveEmpty,
            CalcDir = (byte)(reviveCalcDir ? 1 : 0),
        };
        return cfg;
    }

    [Fact]
    public void GetDefaultFrame_CustomMonStandHonoursCalcDir()
    {
        var cfg = MakeCustomMon(500, 4, 2, standCalcDir: true, 600, 6, 1, true, 700, 5, 1, true);
        TActorCore.CustomMonsterConfigResolver = _ => cfg;

        var a = MakeActor(1, race: 156);
        a.m_nChangeAppr = 42;
        a.m_btDir = 3;
        a.m_nCurrentDefFrame = 2;

        // 500 + 3*(4+2) + 2
        Assert.Equal(500 + 3 * 6 + 2, a.GetDefaultFrame(false));
    }

    [Fact]
    public void GetDefaultFrame_CustomMonWithoutCalcDirUsesZeroDir()
    {
        var cfg = MakeCustomMon(500, 4, 2, standCalcDir: false, 600, 6, 1, true, 700, 5, 1, true);
        TActorCore.CustomMonsterConfigResolver = _ => cfg;

        var a = MakeActor(1, race: 156);
        a.m_nChangeAppr = 42;
        a.m_btDir = 3;
        a.m_nCurrentDefFrame = 1;

        Assert.Equal(500 + 1, a.GetDefaultFrame(false));
    }

    [Fact]
    public void GetDefaultFrame_CustomMonDeathUsesLastFrame()
    {
        var cfg = MakeCustomMon(500, 4, 2, true, 600, 6, 1, dieCalcDir: true, 700, 5, 1, true);
        TActorCore.CustomMonsterConfigResolver = _ => cfg;

        var a = MakeActor(1, race: 156);
        a.m_nChangeAppr = 42;
        a.m_btDir = 2;
        a.m_boDeath = true;

        Assert.Equal(600 + 2 * 7 + 5, a.GetDefaultFrame(false));   // dieStart + dir*(6+1) + (6-1)
    }

    [Fact]
    public void GetDefaultFrame_CustomMonSkeletonUsesDieStart()
    {
        var cfg = MakeCustomMon(500, 4, 2, true, 600, 6, 1, true, 700, 5, 1, true);
        TActorCore.CustomMonsterConfigResolver = _ => cfg;

        var a = MakeActor(1, race: 156);
        a.m_nChangeAppr = 42;
        a.m_boDeath = true;
        a.m_boSkeleton = true;

        Assert.Equal(600, a.GetDefaultFrame(false));
    }

    [Fact]
    public void GetDefaultFrame_CustomMonStoneModeUsesReviveStart()
    {
        var cfg = MakeCustomMon(500, 4, 2, true, 600, 6, 1, true, 700, 5, 1, reviveCalcDir: true);
        TActorCore.CustomMonsterConfigResolver = _ => cfg;

        var a = MakeActor(1, race: 156);
        a.m_nChangeAppr = 42;
        a.m_btDir = 1;
        a.m_nState = (int)ActorStates.STATE_STONE_MODE;

        Assert.Equal(700 + 1 * 6, a.GetDefaultFrame(false));      // reviveStart + dir*(5+1)
    }

    [Fact]
    public void GetDefaultFrame_CustomMonMissingConfigYieldsZero()
    {
        TActorCore.CustomMonsterConfigResolver = _ => null;

        var a = MakeActor(1, race: 156);
        a.m_nChangeAppr = 42;
        a.m_btDir = 4;

        Assert.Equal(0, a.GetDefaultFrame(false));
    }

    [Fact]
    public void GetDefaultFrame_CustomMonNegativeChangeApprFallsToTableBranch()
    {
        var cfg = MakeCustomMon(500, 4, 2, true, 600, 6, 1, true, 700, 5, 1, true);
        TActorCore.CustomMonsterConfigResolver = _ => cfg;

        var a = MakeActor(1, race: 156);
        a.m_nChangeAppr = -1;                           // <0 → 走动作表分支
        a.m_nCurrentDefFrame = -1;

        var pm = ActorActionTables.GetRaceByPM(156, 0)!.Value;
        Assert.Equal(pm.ActStand.start + 0 * (pm.ActStand.frame + pm.ActStand.skip), a.GetDefaultFrame(false));
    }

    // ===================== DefaultMotion =====================

    [Fact]
    public void DefaultMotion_ClearsReverseAndReturnsFrameChanged()
    {
        var a = MakeActor(1, race: 50, appr: 0);
        a.m_boReverseFrame = true;
        a.m_nCurrentFrame = -12345;                     // 必与默认帧不同

        bool changed = a.DefaultMotion();

        Assert.False(a.m_boReverseFrame);
        Assert.True(changed);

        // 第二次：帧号已写回 → 不再变化
        bool changed2 = a.DefaultMotion();
        Assert.False(changed2);
    }

    [Fact]
    public void DefaultMotion_WarModeTimesOutAfterFourSeconds()
    {
        TActorCore.MySelfRef = null;
        var a = MakeActor(1, race: 50, appr: 0);
        a.m_boWarMode = true;
        a.m_boCustomMagicNoAction = true;
        a.m_dwWarModeTime = 1000;

        // SceneTime.TickNow 基准 4000 → 4000-1000 = 3000 不大于 4000
        SceneTime.TickNow = () => 4000;
        a.DefaultMotion();
        Assert.True(a.m_boWarMode);

        SceneTime.TickNow = () => 5001;                 // 5001-1000 > 4000
        a.DefaultMotion();
        Assert.False(a.m_boWarMode);
        Assert.False(a.m_boCustomMagicNoAction);
    }

    [Fact]
    public void DefaultMotion_WarModeNotTimedOutStays()
    {
        var a = MakeActor(1, race: 50, appr: 0);
        a.m_boWarMode = true;
        a.m_dwWarModeTime = 10000;
        SceneTime.TickNow = () => 10000;                // 差 0

        a.DefaultMotion();

        Assert.True(a.m_boWarMode);
    }

    // ===================== RunFrameAction / ActionChanged =====================

    [Fact]
    public void RunFrameAction_IsNoOpMaintainedInBase()
    {
        var a = MakeActor();
        a.RunFrameAction(123);
        a.ComputeActionChanged();
        Assert.True(true);                              // 原文空函数体，仅锁定可调用性
    }

    // ===================== ActionEnded =====================

    [Fact]
    public void RunActionEnded_ComboSpellTriggersContinuousAttackForSelf()
    {
        int called = 0;
        TActorCore.StartContinuousMagicAttackFn = () => called++;

        var a = MakeActor();
        TActorCore.MySelfRef = a;
        a.m_nCurrentAction = TActorCore.SM_SPELL;
        a.m_CurMagicEffectNumber = 104;

        SceneTime.TickNow = () => 555;
        a.RunActionEnded();

        Assert.Equal(1, called);
        Assert.Equal(555u, a.m_dwActionEndTime);
    }

    [Theory]
    [InlineData(103)]
    [InlineData(112)]
    public void RunActionEnded_EffectNumberOutsideRangeDoesNotTrigger(int effectNumber)
    {
        int called = 0;
        TActorCore.StartContinuousMagicAttackFn = () => called++;

        var a = MakeActor();
        TActorCore.MySelfRef = a;
        a.m_nCurrentAction = TActorCore.SM_SPELL;
        a.m_CurMagicEffectNumber = effectNumber;

        a.RunActionEnded();

        Assert.Equal(0, called);
    }

    [Fact]
    public void RunActionEnded_NonSelfDoesNotTriggerEvenForComboSpell()
    {
        int called = 0;
        TActorCore.StartContinuousMagicAttackFn = () => called++;

        var a = MakeActor();
        TActorCore.MySelfRef = null;                    // 非主角
        a.m_nCurrentAction = TActorCore.SM_SPELL;
        a.m_CurMagicEffectNumber = 108;

        a.RunActionEnded();

        Assert.Equal(0, called);
    }

    [Fact]
    public void RunActionEnded_NonSpellActionDoesNotTrigger()
    {
        int called = 0;
        TActorCore.StartContinuousMagicAttackFn = () => called++;

        var a = MakeActor();
        TActorCore.MySelfRef = a;
        a.m_nCurrentAction = TActorCore.SM_HIT;
        a.m_CurMagicEffectNumber = 104;

        a.RunActionEnded();

        Assert.Equal(0, called);
    }

    [Fact]
    public void RunActionEnded_AlwaysStampsActionEndTime()
    {
        SceneTime.TickNow = () => 4242;
        var a = MakeActor();
        TActorCore.MySelfRef = null;

        a.RunActionEnded();

        Assert.Equal(4242u, a.m_dwActionEndTime);
    }

    // ===================== CanCancelAction / CancelAction =====================

    [Fact]
    public void CanCancelAction_OnlyHitWithoutEffect()
    {
        var a = MakeActor();
        Assert.False(a.CanCancelAction());              // 无动作

        a.m_nCurrentAction = TActorCore.SM_HIT;
        a.m_boUseEffect = false;
        Assert.True(a.CanCancelAction());

        a.m_boUseEffect = true;
        Assert.False(a.CanCancelAction());              // 特效中不可取消

        a.m_nCurrentAction = TActorCore.SM_WALK;
        a.m_boUseEffect = false;
        Assert.False(a.CanCancelAction());              // 非 SM_HIT
    }

    [Fact]
    public void RunCancelAction_ZeroesActionAndLocksEndFrame()
    {
        var a = MakeActor();
        a.m_nCurrentAction = TActorCore.SM_HIT;
        a.m_boLockEndFrame = false;

        a.RunCancelAction();

        Assert.Equal(0, a.m_nCurrentAction);
        Assert.True(a.m_boLockEndFrame);
    }

    // ===================== CleanCharMapSetting =====================

    [Fact]
    public void CleanCharMapSetting_AlignsSelfAndResetsFrameAndQueue()
    {
        var self = MakeActor(1);
        self.m_nCurrX = 1; self.m_nCurrY = 2;
        self.m_nRx = 3; self.m_nRy = 4;
        TActorCore.MySelfRef = self;

        self.m_nOldx = 9; self.m_nOldy = 8;
        self.m_nCurrentAction = TActorCore.SM_WALK;
        self.m_nCurrentFrame = 33;
        self.SendMsg(new TChrMsg { Ident = TActorCore.CM_WALK });   // 用户消息
        self.SendMsg(new TChrMsg { Ident = TActorCore.SM_STRUCK }); // 服务器消息

        self.CleanCharMapSetting(100, 200);

        Assert.Equal(100, self.m_nCurrX);
        Assert.Equal(200, self.m_nCurrY);
        Assert.Equal(100, self.m_nRx);
        Assert.Equal(200, self.m_nRy);
        Assert.Equal(100, self.m_nOldx);
        Assert.Equal(200, self.m_nOldy);
        Assert.Equal(0, self.m_nCurrentAction);
        Assert.Equal(-1, self.m_nCurrentFrame);
        Assert.Single(self.MsgList);                    // 仅保留服务器消息
        Assert.Equal(TActorCore.SM_STRUCK, self.MsgList[0].Ident);
    }

    // ===================== CleanUserMsgs =====================

    [Fact]
    public void CleanUserMsgs_RemovesOnlyUserInputIdents()
    {
        var a = MakeActor();
        a.SendMsg(new TChrMsg { Ident = TActorCore.CM_WALK });
        a.SendMsg(new TChrMsg { Ident = TActorCore.CM_RUN });
        a.SendMsg(new TChrMsg { Ident = TActorCore.CM_HORSERUN });
        a.SendMsg(new TChrMsg { Ident = TActorCore.CM_TURN });
        a.SendMsg(new TChrMsg { Ident = TActorCore.CM_HIT });
        a.SendMsg(new TChrMsg { Ident = TActorCore.CM_SPELL });
        a.SendMsg(new TChrMsg { Ident = TActorCore.SM_WALK });
        a.SendMsg(new TChrMsg { Ident = TActorCore.SM_STRUCK });
        a.SendMsg(new TChrMsg { Ident = TActorCore.SM_DEATH });

        a.CleanUserMsgs();

        Assert.Equal(3, a.MsgList.Count);
        Assert.All(a.MsgList, m => Assert.True(m.Ident is TActorCore.SM_WALK or TActorCore.SM_STRUCK or TActorCore.SM_DEATH));
    }

    [Fact]
    public void CleanUserMsgs_EmptyQueueIsHarmless()
    {
        var a = MakeActor();
        a.CleanUserMsgs();
        Assert.Empty(a.MsgList);
    }

    [Fact]
    public void CleanUserMsgs_PreservesRelativeOrderOfServerMessages()
    {
        var a = MakeActor();
        a.SendMsg(new TChrMsg { Ident = TActorCore.SM_WALK });
        a.SendMsg(new TChrMsg { Ident = TActorCore.CM_WALK });
        a.SendMsg(new TChrMsg { Ident = TActorCore.SM_RUN });
        a.SendMsg(new TChrMsg { Ident = TActorCore.SM_TURN });

        a.CleanUserMsgs();

        Assert.Equal(new[] { TActorCore.SM_WALK, TActorCore.SM_RUN, TActorCore.SM_TURN },
            new[] { a.MsgList[0].Ident, a.MsgList[1].Ident, a.MsgList[2].Ident });
    }

    // ===================== MoveFail =====================

    [Fact]
    public void MoveFail_WithRestoreArgsWritesSelfCoordinates()
    {
        var self = MakeActor(1);
        TActorCore.MySelfRef = self;
        self.m_nCurrentAction = TActorCore.SM_WALK;
        self.m_boLockEndFrame = false;

        int canDraw = 0;
        TActorCore.SetCanDrawTileMapFn = () => canDraw++;

        self.SendMsg(new TChrMsg { Ident = TActorCore.CM_WALK });

        self.MoveFail(11, 22, 3);

        Assert.Equal(0, self.m_nCurrentAction);
        Assert.True(self.m_boLockEndFrame);
        Assert.Equal(11, self.m_nCurrX);
        Assert.Equal(22, self.m_nCurrY);
        Assert.Equal(3, self.m_btDir);
        Assert.Empty(self.MsgList);                     // CleanUserMsgs 生效
        Assert.Equal(1, canDraw);
    }

    [Fact]
    public void MoveFail_NegativeArgFallsBackToOldCoordsForWalkFamily()
    {
        var self = MakeActor(1);
        TActorCore.MySelfRef = self;
        self.m_nOldx = 7;
        self.m_nOldy = 8;
        self.m_nOldDir = 5;

        TActorCore.SetCanDrawTileMapFn = () => { };

        TActorCore.ActionCode = TActorCore.CM_RUN;
        self.MoveFail(-1, -1, -1);

        Assert.Equal(7, self.m_nCurrX);
        Assert.Equal(8, self.m_nCurrY);
        Assert.Equal(5, self.m_btDir);
    }

    [Fact]
    public void MoveFail_NonWalkActionCodeKeepsCoordinates()
    {
        var self = MakeActor(1);
        TActorCore.MySelfRef = self;
        self.m_nCurrX = 50; self.m_nCurrY = 60;
        self.m_nOldx = 7; self.m_nOldy = 8;

        TActorCore.SetCanDrawTileMapFn = () => { };

        TActorCore.ActionCode = TActorCore.CM_HIT;      // 不在 CM_WALK/RUN/HORSERUN 集合
        self.MoveFail(-1, -1, -1);

        Assert.Equal(50, self.m_nCurrX);
        Assert.Equal(60, self.m_nCurrY);
    }

    [Fact]
    public void MoveFail_NoSelfRefIsSafeAndSkipsCoordinateWrites()
    {
        var a = MakeActor(1);
        TActorCore.MySelfRef = null;
        a.m_nCurrentAction = TActorCore.SM_RUN;

        int canDraw = 0;
        TActorCore.SetCanDrawTileMapFn = () => canDraw++;

        a.MoveFail(1, 2, 3);

        Assert.Equal(0, a.m_nCurrentAction);
        Assert.True(a.m_boLockEndFrame);
        Assert.Equal(1, canDraw);                       // 尾部动作仍执行
    }

    // ===================== 常量校验 =====================

    [Fact]
    public void UserMsgIdents_MatchGrobal2ClientCommandCodes()
    {
        Assert.Equal(3009, TActorCore.CM_HORSERUN);
        Assert.Equal(3010, TActorCore.CM_TURN);
        Assert.Equal(3011, TActorCore.CM_WALK);
        Assert.Equal(3013, TActorCore.CM_RUN);
        Assert.Equal(3014, TActorCore.CM_HIT);
        Assert.Equal(3017, TActorCore.CM_SPELL);
        Assert.Equal(3018, TActorCore.CM_POWERHIT);
        Assert.Equal(3019, TActorCore.CM_LONGHIT);
        Assert.Equal(3024, TActorCore.CM_WIDEHIT);
        Assert.Equal(3025, TActorCore.CM_FIREHIT);
        Assert.Equal(1000, TActorCore.CM_DROPITEM);
        Assert.Equal(1001, TActorCore.CM_PICKUP);
        Assert.Equal(1006, TActorCore.CM_EAT);
    }

    [Fact]
    public void RunMinHealthAndSkillLockBitMatchOriginal()
    {
        Assert.Equal(10, TActorCore.RUN_MINHEALTH);
        Assert.Equal(0x00010000, TActorCore.STATE_SKILL_LOCK);
        Assert.Equal(0, TActorCore.caNone);
    }
}
