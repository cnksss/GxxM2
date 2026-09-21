using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using Xunit;

using static GXX.RunGate.GateShareSeam;
using static GXX.RunGate.FormGlobals;
using static GXX.RunGate.RunGateConst;
using static GXX.RunGate.RunGateUtilsConst;
using static GXX.Core.Protocol.Grobal2Const;

namespace GXX.RunGate.Tests;

// =====================================================================================
// `CheckUsePlugin` 的 **CM_WALK（走路）族**（原文 :3525-4692）移植测试。
//
// **与 CM_RUN 是"同构但不同"的一对**，本文件对每一处差异都单独断言：
//   * 四个子块的 mode 是 **amHitToWalk / amSpellToWalk / amTurnToMove / amCutMeatToMove**
//     （后两个与 CM_RUN **共用** amTurnToMove / amCutMeatToMove —— 没有 amTurnToWalk / amCutMeatToWalk）
//   * 子块守卫：只有 baSpell 带 `btJob <> 0`（与 CM_RUN 相同）；baHit 在本族**有**前置
//     `ErrorCode := 204`（D-W2，CM_RUN 的 baHit 没有）
//   * 第五个子块是 **amWalk**，**没有** CM_RUN 的 `if not boCollectSpeed` 包裹（D-R3 的差异面）
//   * tick 刷新是 OldLastWalkTick + dwTicks[amWalk] + amWalkToHit + amWalkToSpell（:4684-4688）
//   * 基础 ErrorCode 2 / 201 / 202 / 203；尾部调试码 8/9/10/11/228/12/13/14
//   * 相同点：**没有暗杀检测**；并发块都用 amMoveConcurrent；丢弃并发 `Result := True` + Exit
// =====================================================================================
[Collection("RunGateFormLane")]
public class MirClientContextCheckUsePluginWalkTests
{
    private readonly CapturingTransport _t = new();
    private readonly FakeTcpClient _tcp = new();
    private readonly TRunGate _runGate;
    private readonly TMirClientContext _ctx;
    private uint _now = 5_000_000;

    private const int AmHit = (int)TAntiPlugActionMode.amHit;                       // 0
    private const int AmSpell = (int)TAntiPlugActionMode.amSpell;                   // 1
    private const int AmWalk = (int)TAntiPlugActionMode.amWalk;                     // 2
    private const int AmTurn = (int)TAntiPlugActionMode.amTurn;                     // 4
    private const int AmCutMeat = (int)TAntiPlugActionMode.amCutMeat;               // 5
    private const int AmHitToWalk = (int)TAntiPlugActionMode.amHitToWalk;           // 7
    private const int AmSpellToWalk = (int)TAntiPlugActionMode.amSpellToWalk;       // 11
    private const int AmTurnToMove = (int)TAntiPlugActionMode.amTurnToMove;         // 21
    private const int AmCutMeatToMove = (int)TAntiPlugActionMode.amCutMeatToMove;   // 23
    private const int AmMoveConcurrent = (int)TAntiPlugActionMode.amMoveConcurrent; // 26
    private const int AmWalkToHit = (int)TAntiPlugActionMode.amWalkToHit;           // 6
    private const int AmWalkToSpell = (int)TAntiPlugActionMode.amWalkToSpell;       // 10

    private const int Half = RunGateConst.HalfSpeedIntervalsCount;                   // 200

    public MirClientContextCheckUsePluginWalkTests()
    {
        FormGlobals.ResetForTest();
        FormGlobals.ResetConfigForTest();
        GateShareSeam.ResetForTest();
        FrmMainSeam.FrmMain = null;

        IocpTransport.Current = _t;
        _runGate = new TRunGate { TcpClient = _tcp };
        var core = new TIocpCore { Owner = new TIocpTcpServer { BindObject = _runGate } };
        _ctx = new TMirClientContext(core, 0x11);
        _ctx.RemoteAddr = "1.1.1.1";
        _ctx.ContextID = 23;
        _ctx.InvokeDoReset(false);
        _ctx.sChrName = "走路角色";
        _t.Sent.Clear();
        _t.SentText.Clear();
        _now = 5_000_000;
        MyGetTickCountProvider = () => _now;
    }

    private void ResetCtx()
    {
        _ctx.InvokeDoReset(false);
        _ctx.sChrName = "走路角色";
        _ctx.btJob = 0;
        _ctx.nMoveSpeed = 0;
        _t.Sent.Clear();
    }

    private TProcessMsg Walk() => new TProcessMsg
    {
        DefMessage = TDefaultMessage.Make(CM_WALK, 0, 0, 0, 0),
        dwTimeTick = _now,
    };

    private static TAntiPlugAction Action(int mode) => g_Config.ActionList[mode];

    private void Enable(int mode, uint interval, TActionProcessMode processMode)
    {
        TAntiPlugAction a = Action(mode);
        a.boEnabled = true;
        a.nInterval = interval;
        a.ProcessMode = processMode;
    }

    private static void SetMoveInterval(int mode, int speed, ushort value)
        => g_wActionSpeedIntervals[mode][Half + speed] = value;

    private void QueueWalkPackets(int count)
    {
        for (int i = 0; i < count; i++)
            _ctx.ProcessClientMessage(TDefaultMessage.Make(CM_WALK, 0, 0, 0, 0), Array.Empty<byte>());
    }

    // ---------------------------------------------------------------------------------
    // 1. 基础骨架（无暗杀检测；四个 tick 槽）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void Walk_RecordsBaWalkAndRefreshesFourTickSlots()
    {
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmWalk] = 222;

        Assert.False(_ctx.CheckUsePlugin(Walk()));

        Assert.Equal(1, _ctx.nRecordActionIndex);                             // :3538 Inc
        Assert.Equal(TBaseAction.baWalk, _ctx.RecordActionArr[0].Action);     // :3535
        Assert.Equal((ushort)CM_WALK, _ctx.RecordActionArr[0].DefMsg.Ident);  // :3537
        Assert.Equal(TBaseAction.baWalk, _ctx.LastActionProbe);               // :4691
        Assert.Equal(222u, _ctx.GameSpeed.OldLastWalkTick);                   // :4684（先存旧值）
        Assert.Equal(_now, _ctx.GameSpeed.dwTicks[AmWalk]);                   // :4685
        Assert.Equal(_now, _ctx.GameSpeed.dwTicks[AmWalkToHit]);              // :4687
        Assert.Equal(_now, _ctx.GameSpeed.dwTicks[AmWalkToSpell]);            // :4688
        Assert.Empty(_t.Sent);
    }

    [Fact]
    public void Walk_HasNoAssasinateDetectionEvenWithFullTurnChain()
    {
        // ★ 与 CM_TURN/CM_SPELL/CM_SITDOWN 不同：CM_WALK 完全没有暗杀检测段
        _ctx.RecordActionArr[MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1] =
            new TRecordActionInfo { Action = TBaseAction.baOther, Tick = 1, DefMsg = default };
        for (int i = 0; i < 5; i++)
            _ctx.RecordActionArr[i] = new TRecordActionInfo
            {
                Action = i % 2 == 0 ? TBaseAction.baWalk : TBaseAction.baTurn,
                Tick = _now,
                DefMsg = default,
            };
        _ctx.nRecordActionIndex = 5;

        _ctx.CheckUsePlugin(Walk());

        Assert.False(_ctx.boDelayClose);                     // 未调 ProcessAssasinate
        Assert.Equal(6, _ctx.nRecordActionIndex);
    }

    // ---------------------------------------------------------------------------------
    // 2. 移动并发块（:3540-3590）—— 与 CM_RUN 同块但用 baWalk 判 IsDropConcurrent
    // ---------------------------------------------------------------------------------

    [Fact]
    public void WalkMoveConcurrent_BelowInterval_NoAction()
    {
        Enable(AmMoveConcurrent, 5, TActionProcessMode.apmLost);
        QueueWalkPackets(3);

        Assert.False(_ctx.CheckUsePlugin(Walk()));
        Assert.Equal(TAntiPlugActionMode.amHit, _ctx.LastLockAntiPlugActionMode);
        Assert.Empty(_t.Sent);
    }

    [Fact]
    public void WalkMoveConcurrent_AtOrAboveInterval_DropConcurrentFromWalkTick()
    {
        TAntiPlugAction conc = Action(AmMoveConcurrent);
        conc.boEnabled = true;
        conc.nInterval = 1;
        QueueWalkPackets(1);
        _ctx.LastActionForTest = TBaseAction.baWalk;
        SetMoveInterval(AmWalk, 0, 300);
        _ctx.GameSpeed.dwTicks[AmWalk] = 5_000_000 - 50;     // 50 <= 300 div 3 = 100

        bool r = _ctx.CheckUsePlugin(Walk());

        Assert.True(r);                                      // 收尾 apmFakeAttackPass + IsDropConcurrent
        Assert.Single(_t.Sent);                              // 收尾 :9620 SendActionRet(True)
        Assert.Equal(TAntiPlugActionMode.amMoveConcurrent, _ctx.LastLockAntiPlugActionMode);   // :3562
    }

    [Fact]
    public void WalkMoveConcurrent_BoDebugAloneCountsButDoesNotJudge()
    {
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        TAntiPlugAction conc = Action(AmMoveConcurrent);
        conc.boEnabled = false;
        conc.boDebug = true;
        QueueWalkPackets(2);
        _ctx.LastActionForTest = TBaseAction.baOther;

        Assert.False(_ctx.CheckUsePlugin(Walk()));

        Assert.Single(logs);                                 // :4676（2 + 1 = 3）
        Assert.Equal(AntiPlugActionModeNames3[AmMoveConcurrent] + ":3; 用户:走路角色", logs[0]);
    }

    // ---------------------------------------------------------------------------------
    // 3. 四个“X到走路”子块的 mode / tick / 守卫（★ 与 CM_RUN 逐条对照）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void WalkSubBlock_HitToWalk_UsesHitTickNeedsNoBtJobAndUsesPlainLog()
    {
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        g_Config.boShowAttackLog = true;
        Enable(AmHitToWalk, 250, TActionProcessMode.apmLost);
        SetMoveInterval(AmHitToWalk, 0, 250);
        _ctx.LastActionForTest = TBaseAction.baHit;
        _ctx.btJob = 0;                                      // ★ 本子块无需 btJob
        _ctx.GameSpeed.dwTicks[AmHit] = 0;
        _ctx.GameSpeed.dwTicks[AmWalk] = 1_000_000;          // 若误用 amWalk 槽则不判定
        _now = 25;                                           // 25 <= 250 div 10

        Assert.True(_ctx.CheckUsePlugin(Walk()));
        Assert.Equal(TAntiPlugActionMode.amHitToWalk, _ctx.LastLockAntiPlugActionMode);
        // ★ 本族四个子块的超速日志**不带** `[移动速度]`（只有第五个 amWalk 子块带）
        Assert.Single(logs);
        Assert.Equal("【用户超速】" + AntiPlugActionModeNames3[AmHitToWalk] + ":25; 用户:走路角色", logs[0]);
    }

    [Fact]
    public void WalkSubBlock_SpellToWalk_RequiresBtJob()
    {
        Enable(AmSpellToWalk, 250, TActionProcessMode.apmLost);
        SetMoveInterval(AmSpellToWalk, 0, 250);
        _ctx.LastActionForTest = TBaseAction.baSpell;
        _ctx.GameSpeed.dwTicks[AmSpell] = 0;
        _now = 25;

        _ctx.btJob = 0;
        Assert.False(_ctx.CheckUsePlugin(Walk()));
        Assert.Equal(TAntiPlugActionMode.amHit, _ctx.LastLockAntiPlugActionMode);

        ResetCtx();
        Enable(AmSpellToWalk, 250, TActionProcessMode.apmLost);
        _ctx.btJob = 4;
        _ctx.LastActionForTest = TBaseAction.baSpell;
        _ctx.GameSpeed.dwTicks[AmSpell] = 0;
        _now = 25;
        Assert.True(_ctx.CheckUsePlugin(Walk()));
        Assert.Equal(TAntiPlugActionMode.amSpellToWalk, _ctx.LastLockAntiPlugActionMode);
    }

    [Fact]
    public void WalkSubBlock_TurnAndCutMeatUseTheSharedMoveModes()
    {
        // ★ amTurnToMove / amCutMeatToMove 是 WALK 与 RUN **共用**的两个 mode（没有 ToWalk 版本）：
        //   同一时刻只有一个子块能命中（互斥），但两者都映射到 am*ToMove
        Enable(AmTurnToMove, 250, TActionProcessMode.apmLost);
        SetMoveInterval(AmTurnToMove, 0, 250);
        _ctx.LastActionForTest = TBaseAction.baTurn;
        _ctx.GameSpeed.dwTicks[AmTurn] = 0;
        _ctx.GameSpeed.dwTicks[AmWalk] = 1_000_000;
        _now = 25;
        Assert.True(_ctx.CheckUsePlugin(Walk()));
        Assert.Equal(TAntiPlugActionMode.amTurnToMove, _ctx.LastLockAntiPlugActionMode);

        ResetCtx();
        Enable(AmCutMeatToMove, 250, TActionProcessMode.apmLost);
        SetMoveInterval(AmCutMeatToMove, 0, 250);
        _ctx.LastActionForTest = TBaseAction.baCutMeat;
        _ctx.GameSpeed.dwTicks[AmCutMeat] = 0;
        _ctx.GameSpeed.dwTicks[AmWalk] = 1_000_000;
        _now = 25;
        Assert.True(_ctx.CheckUsePlugin(Walk()));
        Assert.Equal(TAntiPlugActionMode.amCutMeatToMove, _ctx.LastLockAntiPlugActionMode);
    }

    [Fact]
    public void WalkSubBlock_MoveIntervalFollowsNMoveSpeed()
    {
        Enable(AmHitToWalk, 250, TActionProcessMode.apmLost);
        g_wActionSpeedIntervals[AmHitToWalk][0] = 1000;      // -200 槽
        g_wActionSpeedIntervals[AmHitToWalk][Half] = 1;      // 0 槽
        _ctx.LastActionForTest = TBaseAction.baHit;
        _ctx.GameSpeed.dwTicks[AmHit] = 0;
        _now = 100;

        _ctx.nMoveSpeed = -Half;                             // interval 1000 → 100 <= 100 → 硬超速
        Assert.True(_ctx.CheckUsePlugin(Walk()));

        ResetCtx();
        Enable(AmHitToWalk, 250, TActionProcessMode.apmLost);
        _ctx.LastActionForTest = TBaseAction.baHit;
        _ctx.GameSpeed.dwTicks[AmHit] = 0;
        _ctx.nMoveSpeed = 0;                                 // interval 1 → 不判定
        _now = 100;
        Assert.False(_ctx.CheckUsePlugin(Walk()));
    }

    // ---------------------------------------------------------------------------------
    // 4. 第五个子块：amWalk（丢弃并发 + 补偿池）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void WalkSubBlock_AmWalk_FallsThroughForAnyLastAction()
    {
        // ★ D-W3/D-R2 的同一族缺陷：:4329 的 `FLastAction = baWalk` 被 `{}` 注释
        Enable(AmWalk, 300, TActionProcessMode.apmLost);
        SetMoveInterval(AmWalk, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.nCollectIntervalIndexArr[AmWalk] = 1;
        _ctx.dwCollectIntervalArr[AmWalk, 0] = -1;           // 组合超速（nCollectIndex >= 1）
        _ctx.GameSpeed.dwTicks[AmWalk] = 5_000_000 - 200;    // 200 > 300 div 3 → 非丢弃并发

        Assert.True(_ctx.CheckUsePlugin(Walk()));
        Assert.Equal(TAntiPlugActionMode.amWalk, _ctx.LastLockAntiPlugActionMode);
    }

    [Fact]
    public void WalkSubBlock_AmWalk_DropConcurrentExitsWithResultTrue()
    {
        Enable(AmWalk, 300, TActionProcessMode.apmLost);
        SetMoveInterval(AmWalk, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmWalk] = 5_000_000 - 50;     // 50 <= 100
        _now = 5_000_000;

        Assert.True(_ctx.CheckUsePlugin(Walk()));

        Assert.Single(_t.Sent);                              // :4366 SendActionRet(True)
        Assert.Equal(4_999_950u, _ctx.GameSpeed.dwTicks[AmWalk]);   // Exit 早于 :4685
        Assert.Equal(TBaseAction.baOther, _ctx.LastActionProbe);    // Exit 早于 :4691
        Assert.Equal(0u, _ctx.GameSpeed.OldLastWalkTick);           // 未执行 :4684
    }

    [Fact]
    public void WalkSubBlock_AmWalk_BoChangeMapClampsInterval()
    {
        Enable(AmWalk, 300, TActionProcessMode.apmLost);
        SetMoveInterval(AmWalk, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.boChangeMap = true;
        _ctx.GameSpeed.dwTicks[AmWalk] = 5_000_000 - 50;     // 抬到 310 → 不再丢弃并发
        _now = 5_000_000;

        Assert.False(_ctx.CheckUsePlugin(Walk()));
        Assert.Equal(TAntiPlugActionMode.amHit, _ctx.LastLockAntiPlugActionMode);
    }

    [Fact]
    public void WalkSubBlock_AmWalk_CompensationPoolAccumulates()
    {
        // :4371-4418 补偿池（与 CM_RUN 的 :5531-5578 同构）
        Enable(AmWalk, 300, TActionProcessMode.apmLost);
        Action(AmWalk).nCompensationValue = 100;
        SetMoveInterval(AmWalk, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmWalk] = 5_000_000 - 400;    // nCompensationValue = 100 ∈ [4, …)
        _now = 5_000_000;

        Assert.False(_ctx.CheckUsePlugin(Walk()));
        Assert.Equal(100, _ctx.nCompensationArr[AmWalk]);    // :4386 累加 + Min 夹紧
    }

    [Fact]
    public void WalkSubBlock_AmWalk_ApmDelaySetsDelayTimeAndSkipsTickRefresh()
    {
        Enable(AmWalk, 300, TActionProcessMode.apmDelay);
        SetMoveInterval(AmWalk, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.nCollectIntervalIndexArr[AmWalk] = 1;
        _ctx.dwCollectIntervalArr[AmWalk, 0] = -1;
        _ctx.GameSpeed.dwTicks[AmWalk] = 5_000_000 - 200;
        _now = 5_000_000;

        Assert.True(_ctx.CheckUsePlugin(Walk()));

        Assert.Equal(1, _ctx.GameSpeed.nDelayCount[AmWalk]);   // :4557
        Assert.Equal(1, _ctx.ClientMsgListProbe.Count);       // 收尾 DelayClientMessage
        Assert.Equal(4_999_800u, _ctx.GameSpeed.dwTicks[AmWalk]);   // :4680 闸门 → 未刷新
    }

    // ---------------------------------------------------------------------------------
    // 5. 调试日志（:4598-4677）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void WalkTailDebugLog_AmWalkBranchIgnoresLastActionAndLogsCompensation()
    {
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        Action(AmWalk).boDebug = true;
        SetMoveInterval(AmWalk, 0, 500);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _now = 600;

        _ctx.CheckUsePlugin(Walk());

        Assert.Single(logs);
        Assert.Equal(AntiPlugActionModeNames3[AmWalk] + ":600; [移动速度+0]; 用户:走路角色; 补偿:+0; 补偿池:0", logs[0]);
    }

    [Fact]
    public void WalkTailDebugLog_HitBranchUsesHitToWalkAndHitTick()
    {
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        Action(AmHitToWalk).boDebug = true;
        _ctx.LastActionForTest = TBaseAction.baHit;
        _ctx.GameSpeed.dwTicks[AmHit] = 100;
        _now = 700;

        _ctx.CheckUsePlugin(Walk());

        Assert.Single(logs);
        Assert.Equal(AntiPlugActionModeNames3[AmHitToWalk] + ":600; 用户:走路角色", logs[0]);
    }

    // ---------------------------------------------------------------------------------
    // 6. 边界
    // ---------------------------------------------------------------------------------

    [Fact]
    public void Walk_IsNoLongerAnUnportedEarlyReturn()
    {
        _ctx.GameSpeed.boContinueSpeed = true;

        Assert.False(_ctx.CheckUsePlugin(Walk()));

        Assert.Equal(1, _ctx.nRecordActionIndex);
        Assert.Equal(TBaseAction.baWalk, _ctx.LastActionProbe);
        Assert.False(_ctx.GameSpeed.boContinueSpeed);        // 已进入公共收尾 :9680
    }
}
