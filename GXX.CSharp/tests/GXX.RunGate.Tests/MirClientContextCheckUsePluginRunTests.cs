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
// `CheckUsePlugin` 的 **CM_RUN（跑行）族**（原文 :4694-5853）移植测试。
//
// 覆盖重点：
//   * 环形缓冲写 baRun + Inc（**本族没有暗杀检测** —— 与 TURN/SPELL/SITDOWN 都不同）
//   * **移动并发块**（:4712-4759，amMoveConcurrent）：`boEnabled or boDebug` 才计数，
//     只有 `boEnabled` 才判定；命中后 `FLastAction = baRun` 才算 IsDropConcurrent
//   * 四个“X到跑步”子块：baHit→**amHitToRun**、baSpell→**amSpellToRun**（**唯一带 `btJob ≠ 0`**）、
//     baTurn→**amTurnToMove**（不是 amTurnToRun）、baCutMeat→**amCutMeatToMove**（不是 amCutMeatToRun）
//     —— 后两个 mode 与 CM_WALK 的同名子块**共用**，与命名直觉不符（原文如此）
//   * 限速间隔来自 `g_wActionSpeedIntervals[mode][200 + nMoveSpeed]`（±200 边界）
//   * 第五个子块（amRun，:5488）的 `FLastAction = baRun` 被 `{}` 注释 → 对任意 FLastAction 成立；
//     含 **丢弃并发**（Result := True + Exit）与 **补偿池**（nCompensationValue / nCompensationArr）
//   * D-R3：:5589 `if not boCollectSpeed then` 包裹整段采集（此时刚置 False，恒真）
//   * D-R4：:5746 的 `ErrorCode := 323` 与 :5694 重复
//   * :5841-5850 刷新 dwTicks[amRun]（先存 OldLastRunTick）+ amRunToHit + amRunToSpell
// =====================================================================================
[Collection("RunGateFormLane")]
public class MirClientContextCheckUsePluginRunTests
{
    private readonly CapturingTransport _t = new();
    private readonly FakeTcpClient _tcp = new();
    private readonly TRunGate _runGate;
    private readonly TMirClientContext _ctx;
    private uint _now = 4_000_000;

    private const int AmHit = (int)TAntiPlugActionMode.amHit;                       // 0
    private const int AmSpell = (int)TAntiPlugActionMode.amSpell;                   // 1
    private const int AmRun = (int)TAntiPlugActionMode.amRun;                       // 3
    private const int AmCutMeat = (int)TAntiPlugActionMode.amCutMeat;               // 5
    private const int AmHitToRun = (int)TAntiPlugActionMode.amHitToRun;             // 9
    private const int AmSpellToRun = (int)TAntiPlugActionMode.amSpellToRun;         // 13
    private const int AmTurnToMove = (int)TAntiPlugActionMode.amTurnToMove;         // 21
    private const int AmCutMeatToMove = (int)TAntiPlugActionMode.amCutMeatToMove;   // 23
    private const int AmMoveConcurrent = (int)TAntiPlugActionMode.amMoveConcurrent; // 26
    private const int AmRunToHit = (int)TAntiPlugActionMode.amRunToHit;             // 8
    private const int AmRunToSpell = (int)TAntiPlugActionMode.amRunToSpell;         // 12

    private const int Half = RunGateConst.HalfSpeedIntervalsCount;                   // 200

    public MirClientContextCheckUsePluginRunTests()
    {
        FormGlobals.ResetForTest();
        FormGlobals.ResetConfigForTest();
        GateShareSeam.ResetForTest();
        FrmMainSeam.FrmMain = null;

        IocpTransport.Current = _t;
        _runGate = new TRunGate { TcpClient = _tcp };
        var core = new TIocpCore { Owner = new TIocpTcpServer { BindObject = _runGate } };
        _ctx = new TMirClientContext(core, 0x22);
        _ctx.RemoteAddr = "2.2.2.2";
        _ctx.ContextID = 19;
        _ctx.InvokeDoReset(false);
        _ctx.sChrName = "跑行角色";
        _t.Sent.Clear();
        _t.SentText.Clear();
        _now = 4_000_000;
        MyGetTickCountProvider = () => _now;
    }

    private void ResetCtx()
    {
        _ctx.InvokeDoReset(false);
        _ctx.sChrName = "跑行角色";
        _ctx.btJob = 0;
        _ctx.nMoveSpeed = 0;
        _t.Sent.Clear();
    }

    private TProcessMsg Run() => new TProcessMsg
    {
        DefMessage = TDefaultMessage.Make(CM_RUN, 0, 0, 0, 0),
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

    private void QueueRunPackets(int count)
    {
        for (int i = 0; i < count; i++)
            _ctx.ProcessClientMessage(TDefaultMessage.Make(CM_RUN, 0, 0, 0, 0), Array.Empty<byte>());
    }

    // ---------------------------------------------------------------------------------
    // 1. 基础骨架（无暗杀检测）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void Run_RecordsBaRunWithoutAssasinateDetectionAndRefreshesThreeTicks()
    {
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmRun] = 111;

        Assert.False(_ctx.CheckUsePlugin(Run()));

        Assert.Equal(1, _ctx.nRecordActionIndex);                            // :4706 Inc
        Assert.Equal(TBaseAction.baRun, _ctx.RecordActionArr[0].Action);     // :4703
        Assert.Equal((ushort)CM_RUN, _ctx.RecordActionArr[0].DefMsg.Ident);  // :4705
        Assert.Equal(TBaseAction.baRun, _ctx.LastActionProbe);               // :5852
        Assert.Equal(_now, _ctx.GameSpeed.dwTicks[AmRun]);                   // :5846
        Assert.Equal(111u, _ctx.GameSpeed.OldLastRunTick);                   // :5845（先存旧值）
        Assert.Equal(_now, _ctx.GameSpeed.dwTicks[AmRunToHit]);              // :5848
        Assert.Equal(_now, _ctx.GameSpeed.dwTicks[AmRunToSpell]);            // :5849
        Assert.Empty(_t.Sent);
    }

    [Fact]
    public void Run_HasNoAssasinateDetectionEvenWithFullTurnChain()
    {
        // ★ 差异断言：CM_RUN 完全没有 :5868-5930 那样的暗杀检测 ——
        //   即使把 RecordActionArr 铺成"跑步被转向包夹"（CM_TURN 会判定），也**不**触发 ProcessAssasinate
        _ctx.RecordActionArr[MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1] =
            new TRecordActionInfo { Action = TBaseAction.baOther, Tick = 1, DefMsg = default };
        for (int i = 0; i < 5; i++)
            _ctx.RecordActionArr[i] = new TRecordActionInfo
            {
                Action = i % 2 == 0 ? TBaseAction.baRun : TBaseAction.baTurn,
                Tick = _now,
                DefMsg = default,
            };
        _ctx.nRecordActionIndex = 5;

        _ctx.CheckUsePlugin(Run());

        Assert.False(_ctx.boDelayClose);                     // 未调 ProcessAssasinate（:3462 DelayClose(100)）
        Assert.Equal(6, _ctx.nRecordActionIndex);            // 直接 Inc
    }

    // ---------------------------------------------------------------------------------
    // 2. 移动并发块（:4712-4759）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void RunMoveConcurrent_BelowInterval_NoAction()
    {
        Enable(AmMoveConcurrent, 5, TActionProcessMode.apmLost);
        QueueRunPackets(3);                                  // ConcurrentCount = 3 < 5

        Assert.False(_ctx.CheckUsePlugin(Run()));

        Assert.Equal(TAntiPlugActionMode.amHit, _ctx.LastLockAntiPlugActionMode);
        Assert.Empty(_t.Sent);
    }

    [Fact]
    public void RunMoveConcurrent_AtOrAboveInterval_SetsActionAndDropConcurrentFromRunTick()
    {
        // 并发命中 + FLastAction = baRun → :4752 用 dwTicks[amRun] 算 IsDropConcurrent
        TAntiPlugAction conc = Action(AmMoveConcurrent);
        conc.boEnabled = true;
        conc.nInterval = 1;
        QueueRunPackets(1);
        _ctx.LastActionForTest = TBaseAction.baRun;
        SetMoveInterval(AmRun, 0, 300);                      // dwTempInterval = 300
        _ctx.GameSpeed.dwTicks[AmRun] = 4_000_000 - 50;      // 50 <= 300 div 3 = 100 → IsDropConcurrent

        bool r = _ctx.CheckUsePlugin(Run());

        // AntiPlugAction = amMoveConcurrent（默认 apmFakeAttackPass）→ 收尾并发分支
        // → ClearConcurrentPacket 恒 0 → IsDropConcurrent = True → SendActionRet(True) + Result = True
        Assert.True(r);
        Assert.Single(_t.Sent);
        Assert.Equal(TAntiPlugActionMode.amMoveConcurrent, _ctx.LastLockAntiPlugActionMode);   // :4732
        Assert.True(_ctx.GameSpeed.boContinueSpeed);                                           // 收尾 :9545
    }

    [Fact]
    public void RunMoveConcurrent_AboveDropThreshold_LeavesIsDropConcurrentFalse()
    {
        TAntiPlugAction conc = Action(AmMoveConcurrent);
        conc.boEnabled = true;
        conc.nInterval = 1;
        QueueRunPackets(1);
        _ctx.LastActionForTest = TBaseAction.baRun;
        SetMoveInterval(AmRun, 0, 300);
        _ctx.GameSpeed.dwTicks[AmRun] = 4_000_000 - 101;     // 101 > 100

        Assert.False(_ctx.CheckUsePlugin(Run()));            // 收尾 → Result = False（不丢并发）
        Assert.Empty(_t.Sent);
    }

    [Fact]
    public void RunMoveConcurrent_BoDebugAloneCountsButDoesNotJudge()
    {
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        TAntiPlugAction conc = Action(AmMoveConcurrent);
        conc.boEnabled = false;
        conc.boDebug = true;
        QueueRunPackets(2);
        _ctx.LastActionForTest = TBaseAction.baOther;

        Assert.False(_ctx.CheckUsePlugin(Run()));

        Assert.Single(logs);                                 // :5836（ConcurrentCount + 1 = 3）
        Assert.Equal(AntiPlugActionModeNames3[AmMoveConcurrent] + ":3; 用户:跑行角色", logs[0]);
    }

    // ---------------------------------------------------------------------------------
    // 3. 四个“X到跑步”子块的 mode / tick / 守卫
    // ---------------------------------------------------------------------------------

    [Fact]
    public void RunSubBlock_HitToRun_UsesHitTickAndNeedsNoBtJob()
    {
        Enable(AmHitToRun, 250, TActionProcessMode.apmLost);
        SetMoveInterval(AmHitToRun, 0, 250);
        _ctx.LastActionForTest = TBaseAction.baHit;
        _ctx.btJob = 0;                                      // ★ 本子块无需 btJob
        _ctx.GameSpeed.dwTicks[AmHit] = 0;
        _ctx.GameSpeed.dwTicks[AmRun] = 1_000_000;           // 若误用 amRun 槽则不判定
        _now = 25;                                           // 25 <= 250 div 10 = 25

        Assert.True(_ctx.CheckUsePlugin(Run()));
        Assert.Equal(TAntiPlugActionMode.amHitToRun, _ctx.LastLockAntiPlugActionMode);
    }

    [Fact]
    public void RunSubBlock_SpellToRun_RequiresBtJob()
    {
        Enable(AmSpellToRun, 250, TActionProcessMode.apmLost);
        SetMoveInterval(AmSpellToRun, 0, 250);
        _ctx.LastActionForTest = TBaseAction.baSpell;
        _ctx.GameSpeed.dwTicks[AmSpell] = 0;
        _now = 25;

        // (a) btJob = 0 → 守卫 `(btJob <> 0) and …` 不成立
        _ctx.btJob = 0;
        Assert.False(_ctx.CheckUsePlugin(Run()));
        Assert.Equal(TAntiPlugActionMode.amHit, _ctx.LastLockAntiPlugActionMode);

        // (b) btJob <> 0 → 判定命中
        ResetCtx();
        Enable(AmSpellToRun, 250, TActionProcessMode.apmLost);
        _ctx.btJob = 4;
        _ctx.LastActionForTest = TBaseAction.baSpell;
        _ctx.GameSpeed.dwTicks[AmSpell] = 0;
        _now = 25;
        Assert.True(_ctx.CheckUsePlugin(Run()));
        Assert.Equal(TAntiPlugActionMode.amSpellToRun, _ctx.LastLockAntiPlugActionMode);
    }

    [Fact]
    public void RunSubBlock_TurnToRun_UsesAmTurnToMoveModeNotAmTurnToRun()
    {
        // ★ 原文 :5126/:5130-5134 用的是 `amTurnToMove`（不是 amTurnToRun —— 那个枚举成员不存在）
        Enable(AmTurnToMove, 250, TActionProcessMode.apmLost);
        SetMoveInterval(AmTurnToMove, 0, 250);
        _ctx.LastActionForTest = TBaseAction.baTurn;
        _ctx.GameSpeed.dwTicks[(int)TAntiPlugActionMode.amTurn] = 0;
        _ctx.GameSpeed.dwTicks[AmRun] = 1_000_000;
        _now = 25;

        Assert.True(_ctx.CheckUsePlugin(Run()));
        Assert.Equal(TAntiPlugActionMode.amTurnToMove, _ctx.LastLockAntiPlugActionMode);
    }

    [Fact]
    public void RunSubBlock_CutMeatToRun_UsesAmCutMeatToMoveMode()
    {
        Enable(AmCutMeatToMove, 250, TActionProcessMode.apmLost);
        SetMoveInterval(AmCutMeatToMove, 0, 250);
        _ctx.LastActionForTest = TBaseAction.baCutMeat;
        _ctx.GameSpeed.dwTicks[AmCutMeat] = 0;
        _ctx.GameSpeed.dwTicks[AmRun] = 1_000_000;
        _now = 25;

        Assert.True(_ctx.CheckUsePlugin(Run()));
        Assert.Equal(TAntiPlugActionMode.amCutMeatToMove, _ctx.LastLockAntiPlugActionMode);
    }

    [Fact]
    public void RunSubBlock_MoveIntervalFollowsNMoveSpeed()
    {
        // 间隔 = g_wActionSpeedIntervals[mode][200 + nMoveSpeed]，±200 是 `<=`/`>=` 边界
        Enable(AmHitToRun, 250, TActionProcessMode.apmLost);
        g_wActionSpeedIntervals[AmHitToRun][0] = 1000;       // -200
        g_wActionSpeedIntervals[AmHitToRun][1] = 1;          // -199（不钳到槽 0）
        g_wActionSpeedIntervals[AmHitToRun][Half] = 1;       // 0
        _ctx.LastActionForTest = TBaseAction.baHit;
        _ctx.GameSpeed.dwTicks[AmHit] = 0;
        _now = 100;

        // (a) nMoveSpeed = -200 → interval 1000 → 硬超速
        _ctx.nMoveSpeed = -Half;
        Assert.True(_ctx.CheckUsePlugin(Run()));

        // (b) nMoveSpeed = 0 → interval 1 → 不判定
        ResetCtx();
        Enable(AmHitToRun, 250, TActionProcessMode.apmLost);
        _ctx.LastActionForTest = TBaseAction.baHit;
        _ctx.GameSpeed.dwTicks[AmHit] = 0;
        _ctx.nMoveSpeed = 0;
        _now = 100;
        Assert.False(_ctx.CheckUsePlugin(Run()));

        // (c) nMoveSpeed = -199 → 槽 1 = 1 → 不判定
        ResetCtx();
        Enable(AmHitToRun, 250, TActionProcessMode.apmLost);
        _ctx.LastActionForTest = TBaseAction.baHit;
        _ctx.GameSpeed.dwTicks[AmHit] = 0;
        _ctx.nMoveSpeed = -199;
        _now = 100;
        Assert.False(_ctx.CheckUsePlugin(Run()));
    }

    // ---------------------------------------------------------------------------------
    // 4. 第五个子块：amRun（丢弃并发 + 补偿池）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void RunSubBlock_AmRun_FallsThroughForAnyLastAction()
    {
        // ★ D-R2：:5488 的 `FLastAction = baRun` 被 `{}` 注释 → baOther 也会进入本子块。
        //   用"非丢弃并发"的组合超速（nCollectIndex >= 1 的采集路径）证明普通判定命中
        Enable(AmRun, 300, TActionProcessMode.apmLost);
        SetMoveInterval(AmRun, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.nCollectIntervalIndexArr[AmRun] = 1;
        _ctx.dwCollectIntervalArr[AmRun, 0] = -1;
        _ctx.GameSpeed.dwTicks[AmRun] = 4_000_000 - 200;     // 200 > 300 div 3 = 100 → 非丢弃并发
        _now = 4_000_000;

        Assert.True(_ctx.CheckUsePlugin(Run()));
        Assert.Equal(TAntiPlugActionMode.amRun, _ctx.LastLockAntiPlugActionMode);
    }

    [Fact]
    public void RunSubBlock_AmRun_DropConcurrentExitsWithResultTrue()
    {
        // :5504 赋值 IsDropConcurrent；:5513 命中 → SendActionRet(True) + Result := True + Exit
        Enable(AmRun, 300, TActionProcessMode.apmLost);
        SetMoveInterval(AmRun, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmRun] = 4_000_000 - 50;      // 50 <= 100 → 丢弃并发
        _now = 4_000_000;

        Assert.True(_ctx.CheckUsePlugin(Run()));             // D-R2 的另一面：Exit 带 Result = True

        Assert.Single(_t.Sent);                              // :5524 SendActionRet(True)
        Assert.Equal(3_999_950u, _ctx.GameSpeed.dwTicks[AmRun]);   // Exit 早于 :5846（保持调用前的值）
        Assert.Equal(TBaseAction.baOther, _ctx.LastActionProbe);   // Exit 早于 :5852
    }

    [Fact]
    public void RunSubBlock_AmRun_BoChangeMapClampsInterval()
    {
        // :5501 `if boChangeMap and (dwCurrentInterval <= dwTempInterval + DELAY_TIME_ADD) then
        //        dwCurrentInterval := dwTempInterval + DELAY_TIME_ADD;`
        Enable(AmRun, 300, TActionProcessMode.apmLost);
        SetMoveInterval(AmRun, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.boChangeMap = true;
        _ctx.GameSpeed.dwTicks[AmRun] = 4_000_000 - 50;      // 被抬到 300 + 10 = 310
        _now = 4_000_000;

        // 抬到 310 后：IsDropConcurrent = 310 <= 100 → False；boCurrentSpeed = 310 < 300 → False
        // → 组合路径不成立、硬超速 310 <= 30 不成立 → 不判定（对照下面未换图的用例）
        Assert.False(_ctx.CheckUsePlugin(Run()));
        Assert.Equal(TAntiPlugActionMode.amHit, _ctx.LastLockAntiPlugActionMode);

        // 对照：boChangeMap = False → 50 直接命中丢弃并发
        ResetCtx();
        Enable(AmRun, 300, TActionProcessMode.apmLost);
        SetMoveInterval(AmRun, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.boChangeMap = false;
        _ctx.GameSpeed.dwTicks[AmRun] = 4_000_000 - 50;
        _now = 4_000_000;
        Assert.True(_ctx.CheckUsePlugin(Run()));
    }

    [Fact]
    public void RunSubBlock_AmRun_CompensationPoolAccumulatesAndOffsetsInterval()
    {
        // :5531-5578 补偿池：nCompensationValue >= 4 → 累加进池；负值 → 从池里扣并回退 dwCurrentInterval
        Enable(AmRun, 300, TActionProcessMode.apmLost);
        Action(AmRun).nCompensationValue = 100;
        SetMoveInterval(AmRun, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmRun] = 4_000_000 - 400;     // dwCurrentInterval = 400
        _now = 4_000_000;
        // 400 > 300 div 3 = 100 且 400 < 300 * 2 = 600 → 进补偿分支
        // nCompensationValue = 400 - 300 = 100 >= 4 → 池 += 100（池初值 0 → 100，夹到 nCompensationValue=100）
        // 池变 100 后：boCurrentSpeed = 400 < 300 → False；硬超速 400 <= 30 → False；
        // 组合需 boCurrentSpeed → 不判定
        Assert.False(_ctx.CheckUsePlugin(Run()));
        Assert.Equal(100, _ctx.nCompensationArr[AmRun]);     // :5546 累加（Min 夹到 100）

        // 对照：把池塞满 → :5533 先把池夹到 nCompensationValue
        ResetCtx();
        Enable(AmRun, 300, TActionProcessMode.apmLost);
        Action(AmRun).nCompensationValue = 100;
        _ctx.nCompensationArr[AmRun] = 999;
        SetMoveInterval(AmRun, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmRun] = 4_000_000 - 400;
        _now = 4_000_000;
        _ctx.CheckUsePlugin(Run());
        Assert.True(_ctx.nCompensationArr[AmRun] <= 100);    // :5535 夹紧
    }

    [Fact]
    public void RunSubBlock_AmRun_CompensationValueBelowFourZeroesAndClearsPool()
    {
        // :5544-5556：nCompensationValue 在 [0,4) → 置 0；boZeroCompensationValueClearPool 时清池
        Enable(AmRun, 300, TActionProcessMode.apmLost);
        Action(AmRun).nCompensationValue = 100;
        g_Config.boZeroCompensationValueClearPool = true;
        _ctx.nCompensationArr[AmRun] = 50;
        SetMoveInterval(AmRun, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmRun] = 4_000_000 - 302;      // nCompensationValue = 2 ∈ [0,4)
        _now = 4_000_000;

        _ctx.CheckUsePlugin(Run());

        Assert.Equal(0, _ctx.nCompensationArr[AmRun]);       // :5554 清池
    }

    [Fact]
    public void RunSubBlock_AmRun_NotCollectSpeedWrapperIsAlwaysTrue()
    {
        // ★ D-R3：:5589 `if not boCollectSpeed then` —— :5581 刚把 boCollectSpeed 置 False，故恒真；
        //   本用例用"走不到 div 3 旁路"的速度证明采集池仍然被写入（等价于没有这层包裹）
        Enable(AmRun, 300, TActionProcessMode.apmLost);
        SetMoveInterval(AmRun, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.nCollectIntervalIndexArr[AmRun] = 0;
        _ctx.GameSpeed.dwTicks[AmRun] = 4_000_000 - 400;      // 400：> div 3、< 2*nInterval
        _now = 4_000_000;

        _ctx.CheckUsePlugin(Run());

        // 400 >= 300 → :5752 的 `>=` 分支写 1（不是负值）；位置与索引都证明采集段确实执行了
        Assert.Equal(1, _ctx.dwCollectIntervalArr[AmRun, 0]);
        Assert.Equal(1, _ctx.nCollectIntervalIndexArr[AmRun]);          // :5756 环形前进
    }

    [Fact]
    public void RunSubBlock_AmRun_ApmDelaySetsDelayTimeAndSkipsTickRefresh()
    {
        Enable(AmRun, 300, TActionProcessMode.apmDelay);
        SetMoveInterval(AmRun, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.nCollectIntervalIndexArr[AmRun] = 1;
        _ctx.dwCollectIntervalArr[AmRun, 0] = -1;            // 组合超速（nCollectIndex >= 1）
        _ctx.GameSpeed.dwTicks[AmRun] = 4_000_000 - 200;     // 200 > div 3 → 非丢弃并发
        _now = 4_000_000;

        Assert.True(_ctx.CheckUsePlugin(Run()));

        Assert.Equal(1, _ctx.GameSpeed.nDelayCount[AmRun]);   // :5719
        Assert.Equal(1, _ctx.ClientMsgListProbe.Count);      // 收尾 DelayClientMessage
        Assert.Equal(3_999_800u, _ctx.GameSpeed.dwTicks[AmRun]);   // :5841 闸门 → 未刷新
    }

    // ---------------------------------------------------------------------------------
    // 5. 调试日志与 ErrorCode（:5760-5837）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void RunTailDebugLog_AmRunBranchIgnoresLastActionAndLogsCompensation()
    {
        // :5802 `else if {(FLastAction = baRun) and} amRun.boDebug`（同 D-T2）
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        Action(AmRun).boDebug = true;
        SetMoveInterval(AmRun, 0, 500);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmRun] = 0;
        _now = 600;                                          // tick_diff = 600

        _ctx.CheckUsePlugin(Run());

        Assert.Single(logs);
        Assert.Equal(AntiPlugActionModeNames3[AmRun] + ":600; [移动速度+0]; 用户:跑行角色; 补偿:+0; 补偿池:0", logs[0]);
    }

    [Fact]
    public void RunTailDebugLog_HitBranchUsesHitToRunAndHitTick()
    {
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        Action(AmHitToRun).boDebug = true;
        _ctx.LastActionForTest = TBaseAction.baHit;
        _ctx.GameSpeed.dwTicks[AmHit] = 100;
        _now = 700;

        _ctx.CheckUsePlugin(Run());

        Assert.Single(logs);
        Assert.Equal(AntiPlugActionModeNames3[AmHitToRun] + ":600; 用户:跑行角色", logs[0]);
    }

    // ---------------------------------------------------------------------------------
    // 6. 与其它族的边界
    // ---------------------------------------------------------------------------------

    [Fact]
    public void Run_IsNoLongerAnUnportedEarlyReturn()
    {
        _ctx.GameSpeed.boContinueSpeed = true;

        Assert.False(_ctx.CheckUsePlugin(Run()));

        Assert.Equal(1, _ctx.nRecordActionIndex);
        Assert.Equal(TBaseAction.baRun, _ctx.LastActionProbe);
        Assert.False(_ctx.GameSpeed.boContinueSpeed);        // 已进入公共收尾 :9680
    }
}
