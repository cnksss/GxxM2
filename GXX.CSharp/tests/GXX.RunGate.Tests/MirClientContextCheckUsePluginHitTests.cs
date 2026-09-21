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
// `CheckUsePlugin` 的**攻击族**（`CM_HIT`..`CM_115HIT` + `CM_CUSTOM_HIT001..`，原文 :6683-7884）测试。
//
// 本族是三族里**唯一含“攻击槽”语义**的：
//   * `dwTempInterval` 来自 **`nAttackSpeed`**（`g_wActionSpeedIntervals[mode][200 + nAttackSpeed]`）
//     —— 与 `MoveSpeedInterval` / `SpellSpeedInterval` 同构但**变量不同**
//   * 四个“X到攻击”子块的 mode 是 amWalkToHit / amRunToHit / amTurnToHit / amCutMeatToHit
//   * **暗杀检测的前导集合是 `[baTurn, baCutMeat]`**（与 CM_SPELL 逐字相同，与 TURN/WALK/RUN 都不同），
//     目标动作是 **baHit**
//   * :6708 的记录**没有** `ErrorCode := <基础值>`；`Inc(nRecordActionIndex)` 在暗杀检测**之后**（:6771）
//   * `:7809-7850` 是“动作名 → 日志文本”的**二级 if 链**（本族独有，配合 `sHitMagic`）
//
// 逐条差异断言（对应用户特别提醒的三点）：
//   D-H1 :7126-7127 amRunToHit 的 apmDelay 单语句 + tick 取 **amRunToHit 槽** → nDelayTime 为负 → 不延时
//   D-H2 :7505      `{(FLastAction = baHit) and} amHit.boEnabled` → 对任意 FLastAction 都成立
//   D-H3 :7722-7731 amHit 的 apmDelay **自愈段被 `{}` 注释** → nDelayCount 不累加、不发 SendActionRet
//   D-H4 :7676-7677 amHit 采集规则被改：`nCollectIndex+1 <= 5 → nSpeedCount >= 3`（其它族是 `<=3 → >=2`）
//   D-H5 :7594      `if not boCollectSpeed` 恒真包裹（同 CM_RUN 的 amRun）
//   D-H6 :7738      超速日志用 **`AntiPlugActionModeNames`（带两个尾随空格）**，a-d 子块用 `_3`
//   D-H10 :7849-7850 `else sHitMagic := '其他技能'` **不可达**（家族 ident 集合与 if 链覆盖集合完全相同）
// =====================================================================================
[Collection("RunGateFormLane")]
public class MirClientContextCheckUsePluginHitTests
{
    private readonly CapturingTransport _t = new();
    private readonly FakeTcpClient _tcp = new();
    private readonly TRunGate _runGate;
    private readonly TMirClientContext _ctx;
    private uint _now = 6_000_000;

    private const int AmHit = (int)TAntiPlugActionMode.amHit;                         // 0
    private const int AmWalk = (int)TAntiPlugActionMode.amWalk;                       // 2
    private const int AmRun = (int)TAntiPlugActionMode.amRun;                         // 3
    private const int AmTurn = (int)TAntiPlugActionMode.amTurn;                       // 4
    private const int AmCutMeat = (int)TAntiPlugActionMode.amCutMeat;                 // 5
    private const int AmWalkToHit = (int)TAntiPlugActionMode.amWalkToHit;             // 6
    private const int AmHitToWalk = (int)TAntiPlugActionMode.amHitToWalk;             // 7
    private const int AmRunToHit = (int)TAntiPlugActionMode.amRunToHit;               // 8
    private const int AmHitToRun = (int)TAntiPlugActionMode.amHitToRun;               // 9
    private const int AmTurnToHit = (int)TAntiPlugActionMode.amTurnToHit;             // 14
    private const int AmCutMeatToHit = (int)TAntiPlugActionMode.amCutMeatToHit;       // 18
    private const int AmHitConcurrent = (int)TAntiPlugActionMode.amHitConcurrent;     // 24

    private const int Half = RunGateConst.HalfSpeedIntervalsCount;                    // 200

    public MirClientContextCheckUsePluginHitTests()
    {
        FormGlobals.ResetForTest();
        FormGlobals.ResetConfigForTest();
        GateShareSeam.ResetForTest();
        FrmMainSeam.FrmMain = null;

        IocpTransport.Current = _t;
        _runGate = new TRunGate { TcpClient = _tcp };
        var core = new TIocpCore { Owner = new TIocpTcpServer { BindObject = _runGate } };
        _ctx = new TMirClientContext(core, 0x44);
        _ctx.RemoteAddr = "4.4.4.4";
        _ctx.ContextID = 29;
        _ctx.InvokeDoReset(false);
        _ctx.sChrName = "攻击角色";
        _t.Sent.Clear();
        _t.SentText.Clear();
        _now = 6_000_000;
        MyGetTickCountProvider = () => _now;
    }

    private void ResetCtx()
    {
        _ctx.InvokeDoReset(false);
        _ctx.sChrName = "攻击角色";
        _ctx.btJob = 0;
        _ctx.nAttackSpeed = 0;
        _t.Sent.Clear();
    }

    private TProcessMsg Hit(ushort ident = CM_HIT) => new TProcessMsg
    {
        DefMessage = TDefaultMessage.Make(ident, 0, 0, 0, 0),
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

    private static void SetAttackInterval(int mode, int speed, ushort value)
        => g_wActionSpeedIntervals[mode][Half + speed] = value;

    private void QueueHitPackets(int count)
    {
        for (int i = 0; i < count; i++)
            _ctx.ProcessClientMessage(TDefaultMessage.Make(CM_HIT, 0, 0, 0, 0), Array.Empty<byte>());
    }

    // ---------------------------------------------------------------------------------
    // 1. 基础骨架（无基础 ErrorCode；Inc 在暗杀检测之后；三个 tick 槽）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void Hit_RecordsBaHitAndRefreshesThreeTickSlots()
    {
        _ctx.LastActionForTest = TBaseAction.baOther;

        Assert.False(_ctx.CheckUsePlugin(Hit()));

        Assert.Equal(1, _ctx.nRecordActionIndex);                            // :6771 Inc（在暗杀检测之后）
        Assert.Equal(TBaseAction.baHit, _ctx.RecordActionArr[0].Action);     // :6704
        Assert.Equal((ushort)CM_HIT, _ctx.RecordActionArr[0].DefMsg.Ident);  // :6706
        Assert.Equal(TBaseAction.baHit, _ctx.LastActionProbe);               // :7883
        Assert.Equal(_now, _ctx.GameSpeed.dwTicks[AmHit]);                   // :7876
        Assert.Equal(_now, _ctx.GameSpeed.dwTicks[AmHitToWalk]);             // :7879
        Assert.Equal(_now, _ctx.GameSpeed.dwTicks[AmHitToRun]);              // :7880
        Assert.Empty(_t.Sent);
    }

    [Fact]
    public void Hit_RecordActionIndexWrapsBeforeWrite()
    {
        _ctx.nRecordActionIndex = MirClientContextConst.MAX_RECORD_ACTION_COUNT;
        _ctx.CheckUsePlugin(Hit());
        Assert.Equal(1, _ctx.nRecordActionIndex);
        Assert.Equal(TBaseAction.baHit, _ctx.RecordActionArr[0].Action);
    }

    // ---------------------------------------------------------------------------------
    // 2. 暗杀检测：前导集合 = [baTurn, baCutMeat]（★ 与 CM_SPELL 相同，与 TURN/WALK/RUN 不同）
    // ---------------------------------------------------------------------------------

    private void SeedAssasinateChain(TBaseAction preAction, uint now)
    {
        _ctx.RecordActionArr[4] = new TRecordActionInfo { Action = preAction, Tick = now, DefMsg = default };
        _ctx.RecordActionArr[3] = new TRecordActionInfo { Action = TBaseAction.baHit, Tick = now, DefMsg = default };
        _ctx.RecordActionArr[2] = new TRecordActionInfo { Action = preAction, Tick = now, DefMsg = default };
        _ctx.RecordActionArr[1] = new TRecordActionInfo { Action = TBaseAction.baHit, Tick = now, DefMsg = default };
        _ctx.RecordActionArr[0] = new TRecordActionInfo { Action = preAction, Tick = now, DefMsg = default };
        _ctx.nRecordActionIndex = 5;
        _ctx.RecordActionArr[MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1] =
            new TRecordActionInfo { Action = TBaseAction.baOther, Tick = 1, DefMsg = default };
        _now = now;
    }

    [Fact]
    public void Hit_Assasinate_PreActionSetIsTurnAndCutMeat()
    {
        SeedAssasinateChain(TBaseAction.baTurn, 6_000_000);
        Assert.False(_ctx.CheckUsePlugin(Hit()));
        Assert.True(_ctx.boDelayClose);                          // :6737 Exit
        Assert.Equal(5, _ctx.nRecordActionIndex);                // Exit 早于 :6771

        ResetCtx();
        SeedAssasinateChain(TBaseAction.baCutMeat, 6_000_000);
        _ctx.CheckUsePlugin(Hit());
        Assert.True(_ctx.boDelayClose);

        // ★ 差异断言：baWalk（CM_TURN/CM_WALK/CM_RUN 的前导）与 baSpell 都**不算**
        ResetCtx();
        SeedAssasinateChain(TBaseAction.baWalk, 6_000_000);
        _ctx.CheckUsePlugin(Hit());
        Assert.False(_ctx.boDelayClose);

        ResetCtx();
        SeedAssasinateChain(TBaseAction.baSpell, 6_000_000);
        _ctx.CheckUsePlugin(Hit());
        Assert.False(_ctx.boDelayClose);
    }

    [Fact]
    public void Hit_Assasinate_FullVsNotFullRingBuffer()
    {
        SeedAssasinateChain(TBaseAction.baTurn, 6_000_000);
        _ctx.CheckUsePlugin(Hit());
        Assert.True(_ctx.boDelayClose);

        ResetCtx();
        SeedAssasinateChain(TBaseAction.baTurn, 6_000_000);
        _ctx.RecordActionArr[MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1] = default;   // 未满
        _ctx.CheckUsePlugin(Hit());
        Assert.False(_ctx.boDelayClose);                         // 槽 0 被 `> 0` 守卫跳过 → 只 2 次
        Assert.Equal(6, _ctx.nRecordActionIndex);
    }

    // ---------------------------------------------------------------------------------
    // 3. 攻击并发块（amHitConcurrent）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void HitConcurrent_BelowInterval_NoAction()
    {
        Enable(AmHitConcurrent, 5, TActionProcessMode.apmLost);
        QueueHitPackets(3);

        Assert.False(_ctx.CheckUsePlugin(Hit()));
        Assert.Equal(TAntiPlugActionMode.amHit, _ctx.LastLockAntiPlugActionMode);
        Assert.Empty(_t.Sent);
    }

    [Fact]
    public void HitConcurrent_AtOrAboveInterval_DropConcurrentFromHitTick()
    {
        TAntiPlugAction conc = Action(AmHitConcurrent);
        conc.boEnabled = true;
        conc.nInterval = 1;
        QueueHitPackets(1);
        _ctx.LastActionForTest = TBaseAction.baHit;
        SetAttackInterval(AmHit, 0, 300);
        _ctx.GameSpeed.dwTicks[AmHit] = 6_000_000 - 50;      // 50 <= 300 div 3 = 100

        bool r = _ctx.CheckUsePlugin(Hit());

        Assert.True(r);                                      // 收尾 apmFakeAttackPass + IsDropConcurrent
        Assert.Single(_t.Sent);                              // :9620 SendActionRet(True)
        Assert.Equal(TAntiPlugActionMode.amHitConcurrent, _ctx.LastLockAntiPlugActionMode);   // :6790
    }

    [Fact]
    public void HitConcurrent_BoDebugAloneCountsButDoesNotJudge()
    {
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        TAntiPlugAction conc = Action(AmHitConcurrent);
        conc.boEnabled = false;
        conc.boDebug = true;
        QueueHitPackets(2);
        _ctx.LastActionForTest = TBaseAction.baOther;

        Assert.False(_ctx.CheckUsePlugin(Hit()));

        Assert.Single(logs);                                 // :7868（2 + 1 = 3）
        Assert.Equal(AntiPlugActionModeNames3[AmHitConcurrent] + ":3; 用户:攻击角色", logs[0]);
    }

    // ---------------------------------------------------------------------------------
    // 4. 四个“X到攻击”子块 + nAttackSpeed 三支取值
    // ---------------------------------------------------------------------------------

    [Fact]
    public void HitSubBlocks_UseTheirOwnHitModesAndTickSlots()
    {
        // amWalkToHit / amRunToHit / amTurnToHit / amCutMeatToHit；tick 分别取 amWalk/amRun/amTurn/amCutMeat
        (int mode, int tickMode, TBaseAction lastAction)[] cases =
        {
            (AmWalkToHit, AmWalk, TBaseAction.baWalk),
            (AmRunToHit, AmRun, TBaseAction.baRun),
            (AmTurnToHit, AmTurn, TBaseAction.baTurn),
            (AmCutMeatToHit, AmCutMeat, TBaseAction.baCutMeat),
        };

        foreach ((int mode, int tickMode, TBaseAction lastAction) in cases)
        {
            ResetCtx();
            Enable(mode, 250, TActionProcessMode.apmLost);
            SetAttackInterval(mode, 0, 250);
            _ctx.LastActionForTest = lastAction;
            _ctx.GameSpeed.dwTicks[tickMode] = 0;
            _ctx.GameSpeed.dwTicks[AmHit] = 1_000_000;       // 若误用 amHit 槽则不判定
            _now = 25;                                       // 25 <= 250 div 10

            Assert.True(_ctx.CheckUsePlugin(Hit()));
            Assert.Equal((TAntiPlugActionMode)mode, _ctx.LastLockAntiPlugActionMode);
        }
    }

    [Fact]
    public void HitSubBlocK_PlainLogWithoutSpeedSegment()
    {
        // a-d 子块的超速日志是 `【用户超速】%s:%d; 用户:%s`（**不带 [攻击速度]**），用 Names3
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        g_Config.boShowAttackLog = true;
        Enable(AmWalkToHit, 250, TActionProcessMode.apmLost);
        SetAttackInterval(AmWalkToHit, 0, 250);
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _now = 25;

        _ctx.CheckUsePlugin(Hit());

        Assert.Single(logs);
        Assert.Equal("【用户超速】" + AntiPlugActionModeNames3[AmWalkToHit] + ":25; 用户:攻击角色", logs[0]);
    }

    [Fact]
    public void HitSubBlocks_AttackSpeedIntervalFollowsNAttackSpeed()
    {
        // ★ 与 nMoveSpeed / nSpellSpeed 同构的三支：槽 0（<= -200）/ 槽 200+value / 槽 400（>= +200）；
        //   -199 走槽 1（不钳到槽 0）
        Enable(AmWalkToHit, 250, TActionProcessMode.apmLost);
        g_wActionSpeedIntervals[AmWalkToHit][0] = 1000;
        g_wActionSpeedIntervals[AmWalkToHit][1] = 1;
        g_wActionSpeedIntervals[AmWalkToHit][Half] = 1;
        g_wActionSpeedIntervals[AmWalkToHit][Half * 2] = 500;
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _now = 100;

        _ctx.nAttackSpeed = -Half;                           // 槽 0 = 1000 → 硬超速
        Assert.True(_ctx.CheckUsePlugin(Hit()));

        ResetCtx();
        Enable(AmWalkToHit, 250, TActionProcessMode.apmLost);
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _ctx.nAttackSpeed = 0;                               // 槽 200 = 1 → 不判定
        _now = 100;
        Assert.False(_ctx.CheckUsePlugin(Hit()));

        ResetCtx();
        Enable(AmWalkToHit, 250, TActionProcessMode.apmLost);
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _ctx.nAttackSpeed = Half;                            // 槽 400 = 500 → 100 <= 500 div 3 → 判定
        _now = 100;
        Assert.True(_ctx.CheckUsePlugin(Hit()));

        ResetCtx();
        Enable(AmWalkToHit, 250, TActionProcessMode.apmLost);
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _ctx.nAttackSpeed = -199;                            // 槽 1 = 1 → 不判定
        _now = 100;
        Assert.False(_ctx.CheckUsePlugin(Hit()));
    }

    // ---------------------------------------------------------------------------------
    // 5. ★ D-H1：amRunToHit 的 apmDelay（单语句 + tick 取 amRunToHit 槽 → 负 nDelayTime → 不延时）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void HitSubBlock_RunToHit_ApmDelayProducesNegativeDelayTimeAndSkipsQueue()
    {
        Enable(AmRunToHit, 300, TActionProcessMode.apmDelay);
        SetAttackInterval(AmRunToHit, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baRun;
        _ctx.nCollectIntervalIndexArr[AmRunToHit] = 1;
        _ctx.dwCollectIntervalArr[AmRunToHit, 0] = -1;       // 组合超速（nCollectIndex >= 1 → >= 2）
        _ctx.GameSpeed.dwTicks[AmRun] = 6_000_000 - 200;     // dwCurrentInterval = 200（> div 3、< 2*nInterval）
        _ctx.GameSpeed.dwTicks[AmRunToHit] = 0;              // :7127 读的就是这个槽（0 → tick 差 = now）
        _now = 6_000_000;

        Assert.True(_ctx.CheckUsePlugin(Hit()));

        // nDelayTime = (int)(300 - tick_diff(0, 6_000_000) + 10) → 巨大无符号回绕后为**负**
        // → 收尾 `if nDelayTime > 0` 不成立 → **不排延时消息**；且 :7126 是单语句 → nDelayCount 不动
        // → 而且 :7872 的 `if nDelayTime = 0` 也不成立 → **三个 dwTicks 槽都不刷新**
        Assert.Equal(0, _ctx.ClientMsgListProbe.Count);
        Assert.Equal(0, _ctx.GameSpeed.nDelayCount[AmRunToHit]);
        Assert.Equal(0u, _ctx.GameSpeed.dwTicks[AmHit]);
        Assert.Equal(0u, _ctx.GameSpeed.dwTicks[AmHitToWalk]);
        Assert.Equal(0u, _ctx.GameSpeed.dwTicks[AmHitToRun]);
    }

    [Fact]
    public void HitSubBlock_WalkToHit_ApmDelayStandardQueuesAndCounts()
    {
        // 对照（同一子块形状、不同子块）：amWalkToHit 是标准 apmDelay → 排队 + nDelayCount = 1
        Enable(AmWalkToHit, 250, TActionProcessMode.apmDelay);
        SetAttackInterval(AmWalkToHit, 0, 250);
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _now = 25;

        Assert.True(_ctx.CheckUsePlugin(Hit()));

        Assert.Equal(1, _ctx.GameSpeed.nDelayCount[AmWalkToHit]);   // :6956
        Assert.Equal(1, _ctx.ClientMsgListProbe.Count);             // 收尾 DelayClientMessage
        Assert.Equal(0u, _ctx.GameSpeed.dwTicks[AmHit]);            // :7872 闸门 → 未刷新
    }

    // ---------------------------------------------------------------------------------
    // 6. ★ amHit 子块：D-H2 / D-H3 / D-H4 / D-H5 / D-H6
    // ---------------------------------------------------------------------------------

    [Fact]
    public void HitSubBlock_AmHit_FallsThroughForAnyLastAction()
    {
        // D-H2：:7505 的 `FLastAction = baHit` 被 `{}` 注释 → baOther 也会进入。
        // 注意：本族采集规则是 `nCollectIndex+1 <= 5 → nSpeedCount >= 3`（D-H4）→ 需凑够 3 次
        Enable(AmHit, 300, TActionProcessMode.apmLost);
        SetAttackInterval(AmHit, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.nCollectIntervalIndexArr[AmHit] = 2;
        _ctx.dwCollectIntervalArr[AmHit, 0] = -1;            // nSpeedCount = 2 + 1(boCurrentSpeed) = 3
        _ctx.dwCollectIntervalArr[AmHit, 1] = -1;
        _ctx.GameSpeed.dwTicks[AmHit] = 6_000_000 - 200;     // 200 > 100 → 非丢弃并发
        _now = 6_000_000;

        Assert.True(_ctx.CheckUsePlugin(Hit()));
        Assert.Equal(TAntiPlugActionMode.amHit, _ctx.LastLockAntiPlugActionMode);
    }

    [Fact]
    public void HitSubBlock_AmHit_DropConcurrentExitsWithResultTrue()
    {
        Enable(AmHit, 300, TActionProcessMode.apmLost);
        SetAttackInterval(AmHit, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmHit] = 6_000_000 - 50;      // 50 <= 100
        _now = 6_000_000;

        Assert.True(_ctx.CheckUsePlugin(Hit()));

        Assert.Single(_t.Sent);                              // :7534 SendActionRet(True)
        Assert.Equal(5_999_950u, _ctx.GameSpeed.dwTicks[AmHit]);   // Exit 早于 :7876
        Assert.Equal(TBaseAction.baOther, _ctx.LastActionProbe);   // Exit 早于 :7883
    }

    [Fact]
    public void HitSubBlock_AmHit_ApmDelayDoesNotSelfHeal()
    {
        // ★ D-H3：amHit 的 nDelayCount 自愈段被 `{}` 注释 → 只赋值 nDelayTime（正）→ 排队，
        //   但 nDelayCount **不累加**（与 WALK/RUN/TURN/SPELL 的同名段都不同）
        Enable(AmHit, 300, TActionProcessMode.apmDelay);
        SetAttackInterval(AmHit, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.nCollectIntervalIndexArr[AmHit] = 2;
        _ctx.dwCollectIntervalArr[AmHit, 0] = -1;
        _ctx.dwCollectIntervalArr[AmHit, 1] = -1;
        _ctx.GameSpeed.dwTicks[AmHit] = 6_000_000 - 200;     // 200 → nDelayTime = 300-200+10 = 110 > 0
        _now = 6_000_000;

        Assert.True(_ctx.CheckUsePlugin(Hit()));

        Assert.Equal(0, _ctx.GameSpeed.nDelayCount[AmHit]);  // ★ 自愈段被注释 → 不累加
        Assert.Equal(1, _ctx.ClientMsgListProbe.Count);      // 但 nDelayTime > 0 → 仍排队
    }

    [Fact]
    public void HitSubBlock_AmHit_CollectRuleRequiresThreeHitsWithinFive()
    {
        // ★ D-H4：`nCollectIndex + 1 <= 5 → nSpeedCount >= 3`（其它族是 `<= 3 → >= 2`）
        Enable(AmHit, 300, TActionProcessMode.apmLost);
        SetAttackInterval(AmHit, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.nCollectIntervalIndexArr[AmHit] = 2;            // nCollectIndex + 1 = 3
        _ctx.dwCollectIntervalArr[AmHit, 0] = -1;            // nSpeedCount = 1 + 1(boCurrentSpeed) = 2
        _ctx.GameSpeed.dwTicks[AmHit] = 6_000_000 - 200;
        _now = 6_000_000;

        // nSpeedCount = 2 < 3 → **不判定**（若按其它族的 `>= 2` 规则就会判定）
        Assert.False(_ctx.CheckUsePlugin(Hit()));

        // 对照：再给一条负值 → nSpeedCount = 3 → 判定
        ResetCtx();
        Enable(AmHit, 300, TActionProcessMode.apmLost);
        SetAttackInterval(AmHit, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.nCollectIntervalIndexArr[AmHit] = 2;
        _ctx.dwCollectIntervalArr[AmHit, 0] = -1;
        _ctx.dwCollectIntervalArr[AmHit, 1] = -1;
        _ctx.GameSpeed.dwTicks[AmHit] = 6_000_000 - 200;
        _now = 6_000_000;

        Assert.True(_ctx.CheckUsePlugin(Hit()));
        Assert.Equal(TAntiPlugActionMode.amHit, _ctx.LastLockAntiPlugActionMode);
    }

    [Fact]
    public void HitSubBlock_AmHit_SuperSpeedLogUsesNameWithTrailingSpaces()
    {
        // ★ D-H6：amHit 的超速日志用 `AntiPlugActionModeNames`（**带两个尾随空格**）而不是 `_3`
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        g_Config.boShowAttackLog = true;
        Enable(AmHit, 300, TActionProcessMode.apmLost);
        SetAttackInterval(AmHit, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.nCollectIntervalIndexArr[AmHit] = 2;
        _ctx.dwCollectIntervalArr[AmHit, 0] = -1;
        _ctx.dwCollectIntervalArr[AmHit, 1] = -1;
        _ctx.GameSpeed.dwTicks[AmHit] = 6_000_000 - 200;
        _now = 6_000_000;

        _ctx.CheckUsePlugin(Hit());

        Assert.Single(logs);
        Assert.Equal("【用户超速】" + AntiPlugActionModeNames[AmHit] + ":200; [攻击速度+0]; 用户:攻击角色", logs[0]);
    }

    [Fact]
    public void HitSubBlock_AmHit_CompensationPoolAccumulates()
    {
        Enable(AmHit, 300, TActionProcessMode.apmLost);
        Action(AmHit).nCompensationValue = 100;
        SetAttackInterval(AmHit, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmHit] = 6_000_000 - 400;     // nCompensationValue = 100
        _now = 6_000_000;

        _ctx.CheckUsePlugin(Hit());

        Assert.Equal(100, _ctx.nCompensationArr[AmHit]);     // :7554 累加 + Min 夹紧
    }

    // ---------------------------------------------------------------------------------
    // 7. ★ sHitMagic 二级 if 链（:7809-7850）+ 47/48 日志
    // ---------------------------------------------------------------------------------

    [Theory]
    [InlineData(CM_HIT, "普通攻击")]
    [InlineData(CM_HEAVYHIT, "跳起来打")]
    [InlineData(CM_BIGHIT, "强行攻击")]
    [InlineData(CM_POWERHIT, "攻杀剑术")]
    [InlineData(CM_LONGHIT, "刺杀剑法")]
    [InlineData(CM_WIDEHIT, "半月弯刀")]
    [InlineData(CM_FIREHIT, "烈火剑法")]
    [InlineData(CM_CRSHIT, "双龙斩")]
    [InlineData(CM_TWNHIT, "龙影剑法")]
    [InlineData(CM_SWORDHIT, "逐日剑法")]
    [InlineData(CM_43HIT, "雷霆剑法")]
    [InlineData(CM_66HIT, "开天斩轻击")]
    [InlineData(CM_66HIT1, "开天斩重击")]
    [InlineData(CM_101HIT, "三绝杀")]
    [InlineData(CM_102HIT, "断岳斩")]
    [InlineData(CM_103HIT, "横扫千军")]
    [InlineData(CM_113HIT, "断空斩")]
    [InlineData(CM_115HIT, "血魂一击")]
    public void HitDebugLog_ActionNameChain(ushort ident, string expectedName)
    {
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        Action(AmHit).boDebug = true;                        // :7807（FLastAction 被注释 → 任意值都进）
        SetAttackInterval(AmHit, 0, 500);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmHit] = 0;
        _now = 600;

        _ctx.CheckUsePlugin(Hit(ident));

        Assert.Single(logs);
        Assert.Equal(AntiPlugActionModeNames3[AmHit] + ":600; [攻击速度+0]; 用户:攻击角色; "
            + expectedName + "; 补偿:+0; 补偿池:0", logs[0]);   // :7855-7856（47 分支，补偿 >= 0）
    }

    [Theory]
    [InlineData(0, "自定义技能1")]
    [InlineData(299, "自定义技能300")]
    public void HitDebugLog_CustomHitNameUsesIdentOffset(int offset, string expectedName)
    {
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        Action(AmHit).boDebug = true;
        SetAttackInterval(AmHit, 0, 500);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmHit] = 0;
        _now = 600;

        _ctx.CheckUsePlugin(Hit((ushort)(CM_CUSTOM_HIT001 + offset)));

        Assert.Single(logs);
        Assert.Contains(expectedName + ";", logs[0]);         // :7847 '自定义技能' + IntToStr(Ident - CM_CUSTOM_HIT001 + 1)
    }

    [Fact]
    public void HitDebugLog_OutOfRangeCustomHitDoesNotEnterAttackFamily()
    {
        // 自定义技能区间上界**不含** → 落到 else 分支（不会走本族的 sHitMagic 链）
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        Action(AmHit).boDebug = true;
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.nRecordActionIndex = 0;

        _ctx.CheckUsePlugin(Hit((ushort)(CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT)));

        Assert.Empty(logs);                                   // 未进入攻击族
        Assert.Equal(1, _ctx.nRecordActionIndex);             // 落进 `else`（记录 baOther）
        Assert.Equal(TBaseAction.baOther, _ctx.LastActionProbe);
    }

    [Fact]
    public void HitDebugLog_NegativeCompensationUses48Branch()
    {
        // :7858-7862（48 分支）：nCompensationValue < 0 → `补偿:%d`
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        Enable(AmHit, 300, TActionProcessMode.apmLost);
        Action(AmHit).boDebug = true;
        Action(AmHit).nCompensationValue = 100;
        SetAttackInterval(AmHit, 0, 300);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.nCompensationArr[AmHit] = 50;                   // 池 50
        _ctx.GameSpeed.dwTicks[AmHit] = 6_000_000 - 200;     // dwCurrentInterval = 200 < 300 → 补偿 -100
        _now = 6_000_000;

        _ctx.CheckUsePlugin(Hit());

        // 池 50 + (-100) < 0 → nCompensationValue := -50、池清零；随后 dwCurrentInterval += 50 = 250
        // 日志：tick_diff(6_000_000-200, 6_000_000) - (-50) = 250
        Assert.Single(logs);
        Assert.Equal(AntiPlugActionModeNames3[AmHit] + ":250; [攻击速度+0]; 用户:攻击角色; "
            + "普通攻击; 补偿:-50; 补偿池:0", logs[0]);
        Assert.Equal(0, _ctx.nCompensationArr[AmHit]);        // :7571 池清零
    }

    // ---------------------------------------------------------------------------------
    // 8. 与其它族的边界
    // ---------------------------------------------------------------------------------

    [Fact]
    public void Hit_IsNoLongerAnUnportedEarlyReturn()
    {
        _ctx.GameSpeed.boContinueSpeed = true;

        Assert.False(_ctx.CheckUsePlugin(Hit()));

        Assert.Equal(1, _ctx.nRecordActionIndex);
        Assert.Equal(TBaseAction.baHit, _ctx.LastActionProbe);
        Assert.False(_ctx.GameSpeed.boContinueSpeed);        // 已进入公共收尾 :9680
    }
}
