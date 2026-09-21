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
// `CheckUsePlugin` 的 **CM_TURN（转身）族**（原文 :5855-6681）移植测试。
//
// 覆盖重点：
//   * 暗杀检测（:5868-5930）：前导动作集合是 `[baWalk,baRun,baHit,baSpell]`
//     —— **与 CM_SITDOWN 的 `[baHit,baSpell,baWalk,baRun,baTurn]` 不同**（差异断言）
//   * 四个按 FLastAction 互斥的限速子块（:5933 / :6117 / :6285 / :6453）：
//     mode（amTurn / amHitToTurn / amSpellToTurn / amMoveToTurn）与**取 tick 的模式**
//     （amTurn / amHit / amSpell / amWalk|amRun）各不相同（逐个差异断言）
//   * “连续超速直接断开”段（boContinueSpeedCloseSocket）**只有 amTurn 子块是活代码**，
//     b/c/d 子块同一段被 `{}` 注释 —— 这是本族最关键的一处“看起来一样实则不同”
//   * 日志格式：只有 amTurn 子块的超速日志带 `; [移动速度%s]`（:6087-6091）
//   * D-T2：尾部调试日志的 `else if {(FLastAction = baTurn) and} amTurn.boDebug` 对任何
//     FLastAction 都成立
//   * D-T4：:6677 只刷新 dwTicks[amTurn] 一个槽（与 CM_SITDOWN 的 3 个槽不同）
//   * D-T5：FLastAction = baOther 时四个限速子块全不执行（链无 else）
//   * Exit（:5897 / :5927 / :5972 / :6007）跳过公共收尾
// =====================================================================================
[Collection("RunGateFormLane")]
public class MirClientContextCheckUsePluginTurnTests
{
    private readonly CapturingTransport _t = new();
    private readonly FakeTcpClient _tcp = new();
    private readonly TRunGate _runGate;
    private readonly TMirClientContext _ctx;
    private uint _now = 2_000_000;

    private const int AmHit = (int)TAntiPlugActionMode.amHit;                 // 0
    private const int AmSpell = (int)TAntiPlugActionMode.amSpell;             // 1
    private const int AmWalk = (int)TAntiPlugActionMode.amWalk;               // 2
    private const int AmRun = (int)TAntiPlugActionMode.amRun;                 // 3
    private const int AmTurn = (int)TAntiPlugActionMode.amTurn;               // 4
    private const int AmHitToTurn = (int)TAntiPlugActionMode.amHitToTurn;     // 15
    private const int AmSpellToTurn = (int)TAntiPlugActionMode.amSpellToTurn; // 17
    private const int AmMoveToTurn = (int)TAntiPlugActionMode.amMoveToTurn;   // 20
    private const int AmCutMeatToHit = (int)TAntiPlugActionMode.amCutMeatToHit;   // 18

    public MirClientContextCheckUsePluginTurnTests()
    {
        FormGlobals.ResetForTest();
        FormGlobals.ResetConfigForTest();
        GateShareSeam.ResetForTest();
        FrmMainSeam.FrmMain = null;

        IocpTransport.Current = _t;
        _runGate = new TRunGate { TcpClient = _tcp };
        var core = new TIocpCore { Owner = new TIocpTcpServer { BindObject = _runGate } };
        _ctx = new TMirClientContext(core, 0x55);
        _ctx.RemoteAddr = "5.5.5.5";
        _ctx.ContextID = 13;
        _ctx.InvokeDoReset(false);
        _ctx.sChrName = "转身角色";
        _t.Sent.Clear();
        _t.SentText.Clear();
        _now = 2_000_000;
        MyGetTickCountProvider = () => _now;
    }

    private void ResetCtx()
    {
        _ctx.InvokeDoReset(false);
        _ctx.sChrName = "转身角色";
        _t.Sent.Clear();
    }

    private TProcessMsg Msg(ushort ident, uint tick)
        => new TProcessMsg { DefMessage = TDefaultMessage.Make(ident, 0, 0, 0, 0), dwTimeTick = tick };

    private TProcessMsg Turn() => Msg(CM_TURN, _now);

    private static TRecordActionInfo Rec(TBaseAction action, uint tick)
        => new TRecordActionInfo { Action = action, Tick = tick, DefMsg = default };

    private static TAntiPlugAction Action(int mode) => g_Config.ActionList[mode];

    private void Enable(int mode, uint interval, TActionProcessMode processMode)
    {
        TAntiPlugAction a = Action(mode);
        a.boEnabled = true;
        a.nInterval = interval;
        a.ProcessMode = processMode;
    }

    // ---------------------------------------------------------------------------------
    // 1. 基础骨架
    // ---------------------------------------------------------------------------------

    [Fact]
    public void Turn_RecordsBaTurnAndRefreshesOnlyTurnTick()
    {
        _ctx.nRecordActionIndex = 0;
        _ctx.LastActionForTest = TBaseAction.baOther;

        Assert.False(_ctx.CheckUsePlugin(Turn()));

        Assert.Equal(1, _ctx.nRecordActionIndex);                              // :5931 Inc
        Assert.Equal(TBaseAction.baTurn, _ctx.RecordActionArr[0].Action);      // :5864
        Assert.Equal(_now, _ctx.RecordActionArr[0].Tick);                      // :5865
        Assert.Equal((ushort)CM_TURN, _ctx.RecordActionArr[0].DefMsg.Ident);   // :5866
        Assert.Equal(TBaseAction.baTurn, _ctx.LastActionProbe);                // :6680
        Assert.Equal(_now, _ctx.GameSpeed.dwTicks[AmTurn]);                    // :6677
        // ★ 差异断言（D-T4）：CM_TURN 只刷 amTurn 一个槽，CM_SITDOWN 刷三个（amCutMeat/amCutMeatToHit）
        Assert.Equal(0u, _ctx.GameSpeed.dwTicks[AmCutMeatToHit]);
        Assert.Empty(_t.Sent);
    }

    [Fact]
    public void Turn_RecordActionIndexWrapsBeforeWrite()
    {
        _ctx.nRecordActionIndex = MirClientContextConst.MAX_RECORD_ACTION_COUNT;
        _ctx.CheckUsePlugin(Turn());
        Assert.Equal(1, _ctx.nRecordActionIndex);
        Assert.Equal(TBaseAction.baTurn, _ctx.RecordActionArr[0].Action);
    }

    // ---------------------------------------------------------------------------------
    // 2. 暗杀检测 + 与 CM_SITDOWN 的前导动作集合差异
    // ---------------------------------------------------------------------------------

    private void SeedTurnAssasinateChain(TBaseAction preAction, uint now)
    {
        // 槽 4/2/0 = preAction，槽 3/1 = baTurn（都在 250ms 内）
        _ctx.RecordActionArr[4] = Rec(preAction, now);
        _ctx.RecordActionArr[3] = Rec(TBaseAction.baTurn, now);
        _ctx.RecordActionArr[2] = Rec(preAction, now);
        _ctx.RecordActionArr[1] = Rec(TBaseAction.baTurn, now);
        _ctx.RecordActionArr[0] = Rec(preAction, now);
        _ctx.nRecordActionIndex = 5;
        _ctx.RecordActionArr[MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1] = Rec(TBaseAction.baOther, 1);
        _now = now;
    }

    [Fact]
    public void Turn_Assasinate_FullRingBufferDetectsAndExits()
    {
        SeedTurnAssasinateChain(TBaseAction.baWalk, 2_000_000);
        _ctx.LastActionForTest = TBaseAction.baWalk;

        Assert.False(_ctx.CheckUsePlugin(Turn()));               // :5897 Exit → Result 仍 False

        Assert.True(_ctx.boDelayClose);                          // ProcessAssasinate :3462
        Assert.Single(_t.Sent);
        Assert.Equal(5, _ctx.nRecordActionIndex);                // Exit 早于 :5931
        Assert.Equal(0u, _ctx.GameSpeed.dwTicks[AmTurn]);        // Exit 早于 :6677
        Assert.Equal(TBaseAction.baWalk, _ctx.LastActionProbe);  // Exit 早于 :6680
    }

    [Fact]
    public void Turn_Assasinate_NotFullRingBufferSkipsSlotZero()
    {
        SeedTurnAssasinateChain(TBaseAction.baWalk, 2_000_000);
        _ctx.RecordActionArr[MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1] = default;   // Tick = 0 → 未满
        _ctx.LastActionForTest = TBaseAction.baWalk;

        Assert.False(_ctx.CheckUsePlugin(Turn()));

        Assert.False(_ctx.boDelayClose);                         // 槽 0 被 `> 0` 守卫跳过 → 只计 2 次
        Assert.Equal(6, _ctx.nRecordActionIndex);
        Assert.Equal(TBaseAction.baTurn, _ctx.LastActionProbe);
        Assert.Equal(_now, _ctx.GameSpeed.dwTicks[AmTurn]);
    }

    [Fact]
    public void Turn_Assasinate_PreActionSetExcludesTurnAndCutMeat()
    {
        // ★ 差异断言：CM_TURN 的前导集合是 [baWalk,baRun,baHit,baSpell]
        //   → baTurn / baCutMeat 都不算（而 CM_SITDOWN 把 baTurn 算作前导、baCutMeat 不算）
        SeedTurnAssasinateChain(TBaseAction.baTurn, 2_000_000);
        _ctx.CheckUsePlugin(Turn());
        Assert.False(_ctx.boDelayClose);

        ResetCtx();
        SeedTurnAssasinateChain(TBaseAction.baCutMeat, 2_000_000);
        _ctx.CheckUsePlugin(Turn());
        Assert.False(_ctx.boDelayClose);

        ResetCtx();
        SeedTurnAssasinateChain(TBaseAction.baSpell, 2_000_000);   // 对照：baSpell 算
        _ctx.CheckUsePlugin(Turn());
        Assert.True(_ctx.boDelayClose);
    }

    // ---------------------------------------------------------------------------------
    // 3. 四个限速子块：mode 与取 tick 的模式
    // ---------------------------------------------------------------------------------

    [Fact]
    public void TurnSubBlock_TurnAction_HardSpeedDetectsOnTurnTick()
    {
        // 子块 a :5933-6114：FLastAction = baTurn → amTurn，取 dwTicks[amTurn]
        Enable(AmTurn, 100, TActionProcessMode.apmRebound);
        _ctx.LastActionForTest = TBaseAction.baTurn;
        _ctx.GameSpeed.dwTicks[AmTurn] = 0;
        _now = 10;                                              // 10 <= 100 div 10 = 10 → 硬超速

        Assert.True(_ctx.CheckUsePlugin(Turn()));                // apmRebound → 收尾 Result = True

        Assert.Equal(TAntiPlugActionMode.amTurn, _ctx.LastLockAntiPlugActionMode);   // :6060
        Assert.Equal(1u, _ctx.SumSpeedProcessArr[AmTurn, 1]);                        // :6063
        Assert.Single(_t.Sent);                                                      // SendActionRet(False)
        Assert.Equal(10 - 100, _ctx.dwCollectIntervalArr[AmTurn, 0]);                // :6109 负值编码
        Assert.Equal(1, _ctx.nCollectIntervalIndexArr[AmTurn]);                      // :6111
    }

    [Fact]
    public void TurnSubBlock_HitAction_UsesHitTickAndHitToTurnMode()
    {
        // 子块 b :6117-6282：FLastAction = baHit → amHitToTurn，取 dwTicks[amHit]
        Enable(AmHitToTurn, 250, TActionProcessMode.apmLost);
        _ctx.LastActionForTest = TBaseAction.baHit;
        _ctx.GameSpeed.dwTicks[AmHit] = 0;
        _ctx.GameSpeed.dwTicks[AmTurn] = 1_000_000;             // 若误用 amTurn 槽 → tick_diff 巨大 → 不判定
        _now = 25;                                              // 25 <= 250 div 10 = 25

        Assert.True(_ctx.CheckUsePlugin(Turn()));

        Assert.Equal(TAntiPlugActionMode.amHitToTurn, _ctx.LastLockAntiPlugActionMode);
        Assert.True(_ctx.GameSpeed.boContinueSpeed);
    }

    [Fact]
    public void TurnSubBlock_SpellAction_UsesSpellTickAndSpellToTurnMode()
    {
        // 子块 c :6285-6450：FLastAction = baSpell → amSpellToTurn，取 dwTicks[amSpell]
        Enable(AmSpellToTurn, 250, TActionProcessMode.apmLost);
        _ctx.LastActionForTest = TBaseAction.baSpell;
        _ctx.GameSpeed.dwTicks[AmSpell] = 0;
        _ctx.GameSpeed.dwTicks[AmTurn] = 1_000_000;
        _now = 25;

        Assert.True(_ctx.CheckUsePlugin(Turn()));
        Assert.Equal(TAntiPlugActionMode.amSpellToTurn, _ctx.LastLockAntiPlugActionMode);
    }

    [Fact]
    public void TurnSubBlock_WalkRunAction_ChoosesWalkOrRunTick()
    {
        // 子块 d :6453-6623：FLastAction ∈ {baWalk,baRun} → amMoveToTurn，取对应的移动 tick
        Enable(AmMoveToTurn, 250, TActionProcessMode.apmLost);

        // (a) baWalk → dwTicks[amWalk]
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _ctx.GameSpeed.dwTicks[AmRun] = 1_000_000;
        _now = 25;
        Assert.True(_ctx.CheckUsePlugin(Turn()));
        Assert.Equal(TAntiPlugActionMode.amMoveToTurn, _ctx.LastLockAntiPlugActionMode);

        // (b) baRun → dwTicks[amRun]
        ResetCtx();
        Enable(AmMoveToTurn, 250, TActionProcessMode.apmLost);
        _ctx.LastActionForTest = TBaseAction.baRun;
        _ctx.GameSpeed.dwTicks[AmWalk] = 1_000_000;
        _ctx.GameSpeed.dwTicks[AmRun] = 0;
        _now = 25;
        Assert.True(_ctx.CheckUsePlugin(Turn()));
        Assert.Equal(TAntiPlugActionMode.amMoveToTurn, _ctx.LastLockAntiPlugActionMode);
    }

    [Fact]
    public void TurnSubBlock_OtherLastAction_RunsNoRateBlockAtAll()
    {
        // ★ 差异断言（D-T5）：:5933-6623 是 if/else-if 链且无 else →
        //   FLastAction = baOther 时四个子块全不执行（即使 amTurn 已启用且硬超速）
        Enable(AmTurn, 100, TActionProcessMode.apmRebound);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmTurn] = 0;
        _now = 10;

        Assert.False(_ctx.CheckUsePlugin(Turn()));
        Assert.Equal(TAntiPlugActionMode.amHit, _ctx.LastLockAntiPlugActionMode);
        Assert.Empty(_t.Sent);
        Assert.Equal(10u, _ctx.GameSpeed.dwTicks[AmTurn]);       // :6677 仍执行
    }

    // ---------------------------------------------------------------------------------
    // 4. “连续超速直接断开”：只有 amTurn 子块是活代码（D-T1 的核心差异）
    // ---------------------------------------------------------------------------------

    private void SetupContinueSpeed(int mode)
    {
        g_Config.boContinueSpeedCloseSocket = true;
        g_Config.nContinueSpeedCount = 4;
        for (int i = 0; i < 15; i++) _ctx.dwCollectIntervalArr[mode, i] = -1;   // 全负 → 连续超速
        _ctx.nCollectIntervalIndexArr[mode] = 0;
    }

    [Fact]
    public void TurnSubBlock_TurnAction_ContinueSpeedBlockIsLiveAndCloses()
    {
        Enable(AmTurn, 100, TActionProcessMode.apmRebound);      // nInterval = 100
        SetupContinueSpeed(AmTurn);
        _ctx.LastActionForTest = TBaseAction.baTurn;
        _ctx.GameSpeed.dwTicks[AmTurn] = 0;
        _now = 50;                                              // 50 > 10（非硬超速）但 < 100 → boCurrentSpeed

        Assert.False(_ctx.CheckUsePlugin(Turn()));                // :5972 Exit

        Assert.True(_ctx.boDelayClose);                          // ContinuousSpeed :3454 DelayClose(1000)
        Assert.Single(_t.Sent);                                  // ContinuousSpeed :3453
        Assert.Equal(0u, _ctx.GameSpeed.dwTicks[AmTurn]);        // Exit 早于 :6677
        Assert.Equal(1, _ctx.nRecordActionIndex);                // :5931 已执行（Exit 在其之后）
    }

    [Fact]
    public void TurnSubBlock_HitAction_ContinueSpeedBlockIsCommentedOut()
    {
        // ★ 差异断言（D-T1）：同一个“连续超速”配置，子块 b 的该段被 `{}` 注释掉（:6139-6152 / :6167-6179）
        //   → 不走 ContinuousSpeed，而是走普通超速判定（apmLost → Result True，且不 DelayClose）
        Enable(AmHitToTurn, 250, TActionProcessMode.apmLost);
        SetupContinueSpeed(AmHitToTurn);
        _ctx.LastActionForTest = TBaseAction.baHit;
        _ctx.GameSpeed.dwTicks[AmHit] = 0;
        _now = 50;                                              // 50 > 25（非硬超速）但 < 250 → boCurrentSpeed

        Assert.True(_ctx.CheckUsePlugin(Turn()));                 // 普通判定 + apmLost

        Assert.False(_ctx.boDelayClose);                         // ★ 未走 ContinuousSpeed
        Assert.Equal(TAntiPlugActionMode.amHitToTurn, _ctx.LastLockAntiPlugActionMode);
    }

    [Fact]
    public void TurnSubBlock_TurnAction_ContinueSpeed_LatestSlotNonNegativeBreaks()
    {
        // :5961 `>= 0 → Break`：最近一条采集值非负即打断“连续超速”判定
        Enable(AmTurn, 100, TActionProcessMode.apmLost);
        SetupContinueSpeed(AmTurn);
        _ctx.dwCollectIntervalArr[AmTurn, (0 - 1 + 15) % 15] = 0;
        _ctx.LastActionForTest = TBaseAction.baTurn;
        _ctx.GameSpeed.dwTicks[AmTurn] = 0;
        _now = 50;

        Assert.True(_ctx.CheckUsePlugin(Turn()));

        Assert.False(_ctx.boDelayClose);
        Assert.Equal(TAntiPlugActionMode.amTurn, _ctx.LastLockAntiPlugActionMode);
    }

    // ---------------------------------------------------------------------------------
    // 5. 日志格式 / 调试日志
    // ---------------------------------------------------------------------------------

    [Fact]
    public void TurnSubBlock_TurnAction_LogHasMoveSpeedOnlyInThisSubBlock()
    {
        // ★ 差异断言：只有 amTurn 子块的超速日志带 `; [移动速度%s]`（:6087-6091）
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        g_Config.boShowAttackLog = true;
        Enable(AmTurn, 100, TActionProcessMode.apmLost);
        _ctx.LastActionForTest = TBaseAction.baTurn;
        _ctx.GameSpeed.dwTicks[AmTurn] = 0;
        _now = 10;

        _ctx.CheckUsePlugin(Turn());

        Assert.Single(logs);
        Assert.Equal("【用户超速】" + AntiPlugActionModeNames3[AmTurn] + ":10; [移动速度+0]; 用户:转身角色", logs[0]);

        // 对照：子块 b（amHitToTurn）的日志**没有**移动速度段（:6256-6259）
        logs.Clear();
        ResetCtx();
        g_Config.boShowAttackLog = true;
        Enable(AmHitToTurn, 250, TActionProcessMode.apmLost);
        _ctx.LastActionForTest = TBaseAction.baHit;
        _ctx.GameSpeed.dwTicks[AmHit] = 0;
        _now = 25;
        _ctx.CheckUsePlugin(Turn());

        Assert.Single(logs);
        Assert.Equal("【用户超速】" + AntiPlugActionModeNames3[AmHitToTurn] + ":25; 用户:转身角色", logs[0]);
    }

    [Fact]
    public void Turn_TailDebugLog_LastBranchIgnoresLastAction()
    {
        // ★ 差异断言（D-T2）：:6667 `else if {(FLastAction = baTurn) and} amTurn.boDebug then`
        //   —— 前半被 `{}` 注释掉 → 对任何非 {baHit,baSpell,baWalk,baRun} 的 FLastAction 都成立
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        Action(AmTurn).boDebug = true;
        _ctx.GameSpeed.dwTicks[AmTurn] = 0;
        _now = 500;

        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.CheckUsePlugin(Turn());
        Assert.Single(logs);
        Assert.Equal(AntiPlugActionModeNames3[AmTurn] + ":500; 用户:转身角色", logs[0]);

        logs.Clear();
        _ctx.LastActionForTest = TBaseAction.baCutMeat;          // 同样命中（条件里没有 FLastAction 判据）
        _ctx.GameSpeed.dwTicks[AmTurn] = 0;
        _now = 500;
        _ctx.CheckUsePlugin(Turn());
        Assert.Single(logs);
        Assert.Equal(AntiPlugActionModeNames3[AmTurn] + ":500; 用户:转身角色", logs[0]);

        // 对照：FLastAction = baWalk 时命中的是 amMoveToTurn 的那条（不是 amTurn 的）
        logs.Clear();
        Action(AmTurn).boDebug = false;
        Action(AmMoveToTurn).boDebug = true;
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 100;
        _now = 700;
        _ctx.CheckUsePlugin(Turn());
        Assert.Single(logs);
        Assert.Equal(AntiPlugActionModeNames3[AmMoveToTurn] + ":600; 用户:转身角色", logs[0]);
    }

    [Fact]
    public void Turn_TailDebugLog_SuppressedWhenBoDelay()
    {
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        Action(AmTurn).boDebug = true;
        _ctx.LastActionForTest = TBaseAction.baOther;
        TProcessMsg msg = Turn();
        msg.boDelay = true;

        _ctx.CheckUsePlugin(msg);

        Assert.Empty(logs);                                     // :6625 `if (not Msg.boDelay)`
    }

    // ---------------------------------------------------------------------------------
    // 6. 延时路径与 dwTicks 闸门
    // ---------------------------------------------------------------------------------

    [Fact]
    public void TurnSubBlock_TurnAction_ApmDelaySkipsTurnTickRefresh()
    {
        // :6072 nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD
        // :6675 `if nDelayTime = 0` → 为假 → **不刷新** dwTicks[amTurn]
        Enable(AmTurn, 100, TActionProcessMode.apmDelay);
        _ctx.LastActionForTest = TBaseAction.baTurn;
        _ctx.GameSpeed.dwTicks[AmTurn] = 0;
        _now = 10;

        Assert.True(_ctx.CheckUsePlugin(Turn()));

        Assert.Equal(1, _ctx.GameSpeed.nDelayCount[AmTurn]);      // :6075
        Assert.Equal(1, _ctx.ClientMsgListProbe.Count);           // 收尾 DelayClientMessage
        Assert.Equal(0u, _ctx.GameSpeed.dwTicks[AmTurn]);         // ★ :6675 闸门
        Assert.Equal(0, _ctx.dwCollectIntervalArr[AmTurn, 0]);    // :6102 闸门
    }

    [Fact]
    public void TurnSubBlock_TurnAction_DelayCountOverEightSelfHeals()
    {
        Enable(AmTurn, 100, TActionProcessMode.apmDelay);
        _ctx.GameSpeed.nDelayCount[AmTurn] = 8;
        _ctx.LastActionForTest = TBaseAction.baTurn;
        _ctx.GameSpeed.dwTicks[AmTurn] = 0;
        _now = 10;

        Assert.True(_ctx.CheckUsePlugin(Turn()));

        Assert.Equal(0, _ctx.GameSpeed.nDelayCount[AmTurn]);      // :6078
        Assert.Single(_t.Sent);                                   // :6080 SendActionRet(True)
        Assert.Equal(0, _ctx.ClientMsgListProbe.Count);
        Assert.Equal(10u, _ctx.GameSpeed.dwTicks[AmTurn]);        // nDelayTime 已清 0 → :6677
    }

    [Fact]
    public void TurnSubBlock_NoSpeed_ZeroesDelayCountOnlyWhenNotContinueSpeedPass()
    {
        // :6096 与 CM_SITDOWN 的 :9228 同构（else 里的 nDelayCount 归零）
        Enable(AmTurn, 100, TActionProcessMode.apmLost);
        _ctx.LastActionForTest = TBaseAction.baTurn;
        _ctx.GameSpeed.dwTicks[AmTurn] = 0;
        _ctx.GameSpeed.nDelayCount[AmTurn] = 5;
        _now = 500;                                              // 500 >= 100 → 不快

        Assert.False(_ctx.CheckUsePlugin(Turn()));
        Assert.Equal(0, _ctx.GameSpeed.nDelayCount[AmTurn]);

        _ctx.GameSpeed.nDelayCount[AmTurn] = 5;
        _ctx.GameSpeed.boContinueSpeed = true;
        _ctx.GameSpeed.dwStartSpeedTick = 0;
        _ctx.GameSpeed.dwTicks[AmTurn] = 0;                       // 上一次调用在 :6677 刷过该槽，此处复位
        _now = 500;                                              // tick_diff(0,500) = 500 >= 100+30
        Assert.False(_ctx.CheckUsePlugin(Turn()));
        Assert.Equal(5, _ctx.GameSpeed.nDelayCount[AmTurn]);      // boContinueSpeedPass → 不归零
    }

    // ---------------------------------------------------------------------------------
    // 7. 边界
    // ---------------------------------------------------------------------------------

    [Fact]
    public void Turn_ThirdDivisionBoundaryIsInclusiveAndHardPathIsSubset()
    {
        // :6044 / :6019 / :6038 的 `dwCurrentInterval <= dwTempInterval div 3` 是闭区间；
        // 且 `div 10` 的硬超速集合**包含于** `div 3` 集合（nInterval/10 <= nInterval/3 恒成立）
        // → 硬超速无法只靠 tick 与 div 3 旁路区分开，这里钉死 div 3 的边界。
        Enable(AmTurn, 100, TActionProcessMode.apmLost);          // div 3 = 33、div 10 = 10
        _ctx.LastActionForTest = TBaseAction.baTurn;

        _ctx.GameSpeed.dwTicks[AmTurn] = 0;
        _now = 34;                                              // 34 > 33 → 无任何旁路 → 不判定
        Assert.False(_ctx.CheckUsePlugin(Turn()));
        Assert.Equal(TAntiPlugActionMode.amHit, _ctx.LastLockAntiPlugActionMode);

        ResetCtx();
        Enable(AmTurn, 100, TActionProcessMode.apmLost);
        _ctx.LastActionForTest = TBaseAction.baTurn;
        _ctx.GameSpeed.dwTicks[AmTurn] = 0;
        _now = 33;                                              // 恰好 33 → 命中 :6044 旁路
        Assert.True(_ctx.CheckUsePlugin(Turn()));
        Assert.Equal(TAntiPlugActionMode.amTurn, _ctx.LastLockAntiPlugActionMode);

        ResetCtx();
        Enable(AmTurn, 100, TActionProcessMode.apmLost);
        _ctx.LastActionForTest = TBaseAction.baTurn;
        _ctx.GameSpeed.dwTicks[AmTurn] = 0;
        _now = 10;                                              // 恰好 10 → 命中硬超速
        Assert.True(_ctx.CheckUsePlugin(Turn()));
    }
}
