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
// `CheckUsePlugin` 的 **CM_SITDOWN（挖肉）族**（原文 :8997-9469）移植测试。
//
// 覆盖重点（每一条都对应原文行号）：
//   * 环形缓冲写入 baCutMeat + `Inc(nRecordActionIndex)` 的**位置**（:9007-9009 / :9073）
//   * 暗杀检测的两条互斥路径（:9012 采集满 `mod` 版 vs :9043 未满裸减法版），
//     以及两者**唯一的真实差异**：槽 0 在未满路径被 `> 0` 守卫排除（差异断言）
//   * 250ms 边界的**闭区间**（`<= 250`）
//   * “移动到挖肉”（amMoveToCutMeat，:9076 走路/跑步分支）与“挖肉”（amCutMeat，:9249）
//     的互斥关系（D-S2：走路/跑步且 amMoveToCutMeat 未启用时 **amCutMeat 整段被跳过**）
//   * 限速判定：`<= nInterval div 10` 硬超速 / `boCurrentSpeed && boCollectSpeed` 组合 / `div 3` 旁路
//   * 采集池编码：`dwCurrentInterval - dwTempInterval` 的**无符号回绕成负 int**（:9241/:9423）
//   * `nDelayTime` 对 :9461「刷新 dwTicks」的闸门，以及 `nDelayCount > 8` 的自愈（:9209-9213）
//   * `boContinueSpeedCloseSocket` 的连续超速提前断开（:9270-9287，含 Break 分支）
//   * `Msg.boDelay` 的两个闸门（:9429 生效 / :9461 被 `{}` 注释掉 → 照抄）
//   * `Exit`（:9039/:9069/:9286/:9322）**跳过公共收尾**：Result=false 且不刷新 dwTicks/FLastAction
// =====================================================================================
[Collection("RunGateFormLane")]
public class MirClientContextCheckUsePluginSitDownTests
{
    private readonly CapturingTransport _t = new();
    private readonly FakeTcpClient _tcp = new();
    private readonly TRunGate _runGate;
    private readonly TMirClientContext _ctx;
    private uint _now = 1_000_000;

    private const int AmCutMeat = (int)TAntiPlugActionMode.amCutMeat;                 // 5
    private const int AmMoveToCutMeat = (int)TAntiPlugActionMode.amMoveToCutMeat;     // 22
    private const int AmWalk = (int)TAntiPlugActionMode.amWalk;                       // 2
    private const int AmRun = (int)TAntiPlugActionMode.amRun;                         // 3
    private const int AmCutMeatToHit = (int)TAntiPlugActionMode.amCutMeatToHit;       // 18

    public MirClientContextCheckUsePluginSitDownTests()
    {
        FormGlobals.ResetForTest();
        FormGlobals.ResetConfigForTest();
        GateShareSeam.ResetForTest();
        FrmMainSeam.FrmMain = null;

        IocpTransport.Current = _t;
        _runGate = new TRunGate { TcpClient = _tcp };
        var core = new TIocpCore { Owner = new TIocpTcpServer { BindObject = _runGate } };
        _ctx = new TMirClientContext(core, 0x99);
        _ctx.RemoteAddr = "9.9.9.9";
        _ctx.ContextID = 11;
        _ctx.InvokeDoReset(false);
        _ctx.sChrName = "挖肉角色";
        _t.Sent.Clear();
        _t.SentText.Clear();
        _now = 1_000_000;
        MyGetTickCountProvider = () => _now;
    }

    /// <summary>DoReset 会把 sChrName 清空（原文 :491）→ 复位后必须重设。</summary>
    private void ResetCtx()
    {
        _ctx.InvokeDoReset(false);
        _ctx.sChrName = "挖肉角色";
    }

    private TProcessMsg Msg(ushort ident, uint tick)
        => new TProcessMsg { DefMessage = TDefaultMessage.Make(ident, 0, 0, 0, 0), dwTimeTick = tick };

    private TProcessMsg SitDown() => Msg(CM_SITDOWN, _now);

    private static TRecordActionInfo Rec(TBaseAction action, uint tick)
        => new TRecordActionInfo { Action = action, Tick = tick, DefMsg = default };

    private static TAntiPlugAction Action(int mode) => g_Config.ActionList[mode];

    // ---------------------------------------------------------------------------------
    // 1. 基础：环形缓冲写入 + 刷新 dwTicks（两个限速块都未启用）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void SitDown_RecordsBaCutMeatAndRefreshesCutMeatTicks()
    {
        // 默认配置：amCutMeat.boEnabled = False、amMoveToCutMeat.boEnabled = False（GateShare.pas:704/:942）
        _ctx.nRecordActionIndex = 0;
        _ctx.LastActionForTest = TBaseAction.baOther;

        Assert.False(_ctx.CheckUsePlugin(SitDown()));      // AntiPlugAction = nil → 收尾 Result = False

        Assert.Equal(1, _ctx.nRecordActionIndex);                                  // :9073 Inc
        Assert.Equal(TBaseAction.baCutMeat, _ctx.RecordActionArr[0].Action);       // :9007
        Assert.Equal(_now, _ctx.RecordActionArr[0].Tick);                          // :9008
        Assert.Equal((ushort)CM_SITDOWN, _ctx.RecordActionArr[0].DefMsg.Ident);    // :9009
        Assert.Equal(TBaseAction.baCutMeat, _ctx.LastActionProbe);                 // :9468
        Assert.Equal(_now, _ctx.GameSpeed.dwTicks[AmCutMeat]);                     // :9463
        Assert.Equal(_now, _ctx.GameSpeed.dwTicks[AmCutMeatToHit]);                // :9464
        Assert.Empty(_t.Sent);
    }

    [Fact]
    public void SitDown_RecordActionIndexWrapsBeforeWrite()
    {
        // :9005 `>= MAX or < 0 → 0`（与 CM_DROPITEM/CM_PICKUP/else 同构）
        _ctx.nRecordActionIndex = MirClientContextConst.MAX_RECORD_ACTION_COUNT;
        _ctx.CheckUsePlugin(SitDown());
        Assert.Equal(1, _ctx.nRecordActionIndex);
        Assert.Equal(TBaseAction.baCutMeat, _ctx.RecordActionArr[0].Action);

        _ctx.nRecordActionIndex = -3;
        _ctx.CheckUsePlugin(SitDown());
        Assert.Equal(1, _ctx.nRecordActionIndex);
    }

    // ---------------------------------------------------------------------------------
    // 2. 暗杀检测（原文 :9012-9072）—— 两条路径 + 槽 0 差异断言
    // ---------------------------------------------------------------------------------

    /// <summary>
    /// 铺一条“挖肉 / 走路”交替链：槽 4/2/0 = baWalk，槽 3/1 = baCutMeat。
    /// nRecordActionIndex = 5 → 本次 CM_SITDOWN 写槽 5。
    /// </summary>
    private void SeedAssasinateChain(uint walkTick, uint cutTick, uint now)
    {
        _ctx.RecordActionArr[4] = Rec(TBaseAction.baWalk, walkTick);
        _ctx.RecordActionArr[3] = Rec(TBaseAction.baCutMeat, cutTick);
        _ctx.RecordActionArr[2] = Rec(TBaseAction.baWalk, walkTick);
        _ctx.RecordActionArr[1] = Rec(TBaseAction.baCutMeat, cutTick);
        _ctx.RecordActionArr[0] = Rec(TBaseAction.baWalk, walkTick);
        _ctx.nRecordActionIndex = 5;
        _now = now;
    }

    [Fact]
    public void SitDown_Assasinate_FullRingBufferDetectsThreeHitsAndExits()
    {
        // :9012 `RecordActionArr[MAX-1].Tick <> 0` 为真 → 走“采集满”路径（索引取模，无正数守卫）
        SeedAssasinateChain(_now, _now, 1_000_000);
        _ctx.RecordActionArr[MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1] =
            Rec(TBaseAction.baOther, 1);                       // Tick <> 0 → 采集满
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _t.Sent.Clear();

        Assert.False(_ctx.CheckUsePlugin(SitDown()));          // 原文 :9039 Exit → Result 仍是 False

        Assert.True(_ctx.boDelayClose);                        // ProcessAssasinate :3462 DelayClose(100)
        Assert.Single(_t.Sent);                                // :3461 SendMessaggeToClient
        Assert.Empty(_t.SentText);
        // ★ Exit 发生在 :9073 / :9463 / :9468 **之前**
        Assert.Equal(5, _ctx.nRecordActionIndex);              // 未 Inc
        Assert.Equal(0u, _ctx.GameSpeed.dwTicks[AmCutMeat]);   // 未刷新 dwTicks
        Assert.Equal(TBaseAction.baWalk, _ctx.LastActionProbe); // FLastAction 未改成 baCutMeat
        Assert.Equal(TAntiPlugActionMode.amHit, _ctx.LastLockAntiPlugActionMode);   // 收尾未执行
    }

    [Fact]
    public void SitDown_Assasinate_NotFullRingBufferSkipsSlotZero()
    {
        // ★ 差异断言（本族最易错处）：同一份链，仅 `[MAX-1].Tick` 不同 →
        //   (a) 采集满：取模索引 → 槽 0 参与 → nAssasinate = 3 → 判定 + Exit
        //   (b) 未满：裸减法 + `> 0` 守卫 → 槽 0 被跳过 → nAssasinate = 2 → 不判定
        SeedAssasinateChain(_now, _now, 1_000_000);
        _ctx.RecordActionArr[MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1] = default;   // Tick = 0
        _ctx.LastActionForTest = TBaseAction.baWalk;

        Assert.False(_ctx.CheckUsePlugin(SitDown()));

        Assert.False(_ctx.boDelayClose);                       // 无暗杀判定
        Assert.Empty(_t.Sent);
        Assert.Equal(6, _ctx.nRecordActionIndex);              // 本次 Inc 生效（未 Exit）
        Assert.Equal(TBaseAction.baCutMeat, _ctx.LastActionProbe);   // :9468 生效
        Assert.Equal(_now, _ctx.GameSpeed.dwTicks[AmCutMeat]);       // :9463 生效
    }

    [Fact]
    public void SitDown_Assasinate_250msBoundaryIsInclusive()
    {
        // :9018 / :9029 `tick_diff(...) <= 250` —— **闭区间**
        SeedAssasinateChain(1_000_000 - 250, 1_000_000, 1_000_000);
        _ctx.RecordActionArr[MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1] = Rec(TBaseAction.baOther, 1);
        _ctx.CheckUsePlugin(SitDown());
        Assert.True(_ctx.boDelayClose);                        // 恰好 250 → 三处全成立 → 判定

        // 复位后把走路时间再推早 1ms（251）→ 首判就超出边界 → 一个都不计
        ResetCtx();
        SeedAssasinateChain(1_000_000 - 251, 1_000_000, 1_000_000);
        _ctx.RecordActionArr[MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1] = Rec(TBaseAction.baOther, 1);
        _ctx.CheckUsePlugin(SitDown());
        Assert.False(_ctx.boDelayClose);
    }

    [Fact]
    public void SitDown_Assasinate_WrongPreActionKindDoesNotCount()
    {
        // :9017 `Action in [baHit, baSpell, baWalk, baRun, baTurn]` —— baOther / baCutMeat 都不算；
        // 且 :9019 的 begin 同时包住 for 与最终判据 → 首判不成立就**整段不执行**
        SeedAssasinateChain(_now, _now, 1_000_000);
        _ctx.RecordActionArr[4] = Rec(TBaseAction.baOther, _now);
        _ctx.RecordActionArr[MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1] = Rec(TBaseAction.baOther, 1);

        _ctx.CheckUsePlugin(SitDown());

        Assert.False(_ctx.boDelayClose);
    }

    // ---------------------------------------------------------------------------------
    // 3. amCutMeat（挖肉）限速 —— 硬超速 / 组合超速 / 采集池 / 延时
    // ---------------------------------------------------------------------------------

    [Fact]
    public void SitDown_CutMeat_HardSpeedTriggersReboundAndPostlude()
    {
        TAntiPlugAction cut = Action(AmCutMeat);
        cut.boEnabled = true;                                   // 默认 nInterval = 620 / apmRebound
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmCutMeat] = 0;
        _now = 62;                                              // tick_diff(0,62) = 62 <= 620 div 10 = 62

        Assert.True(_ctx.CheckUsePlugin(SitDown()));             // 收尾 apmRebound → Result = True

        Assert.Equal(TAntiPlugActionMode.amCutMeat, _ctx.LastLockAntiPlugActionMode);   // :9375
        Assert.True(_ctx.GameSpeed.boContinueSpeed);                                    // 收尾 :9545
        Assert.Single(_t.Sent);                                                         // 收尾 :9586 SendActionRet(False)
        Assert.Equal(1u, _ctx.SumSpeedProcessArr[AmCutMeat, 1]);                        // :9378 Inc

        // 采集池：62 < 620 → `dwCurrentInterval - dwTempInterval` 无符号回绕成负 int（:9423）
        Assert.Equal(62 - 620, _ctx.dwCollectIntervalArr[AmCutMeat, 0]);
        Assert.Equal(1, _ctx.nCollectIntervalIndexArr[AmCutMeat]);                      // :9425 环形前进
    }

    [Fact]
    public void SitDown_CutMeat_HardSpeedAndThirdDivisionBoundariesAreSeparate()
    {
        // 原文有两套阈值：:9368 `div 10`（硬超速）与 :9334/:9353 `div 3`（“网速双倍”旁路）
        TAntiPlugAction cut = Action(AmCutMeat);
        cut.boEnabled = true;
        cut.nInterval = 100;                                    // div 10 = 10、div 3 = 33
        cut.ProcessMode = TActionProcessMode.apmLost;

        // (a) dwCurrentInterval = 34 → 34 > 10 且 34 > 33，且组合路径（dwCollectCount ≥ 2 /
        //     nCollectIndex = 0 / nSpeedCount = 0）不成立 → **不判定**
        _ctx.GameSpeed.dwTicks[AmCutMeat] = 0;
        _now = 34;
        Assert.False(_ctx.CheckUsePlugin(SitDown()));
        Assert.Empty(_t.Sent);
        Assert.Equal(TAntiPlugActionMode.amHit, _ctx.LastLockAntiPlugActionMode);

        // (b) dwCurrentInterval = 33 → 命中 `div 3` 旁路（:9353）→ 判定
        ResetCtx();
        Action(AmCutMeat).boEnabled = true;
        Action(AmCutMeat).nInterval = 100;
        Action(AmCutMeat).ProcessMode = TActionProcessMode.apmLost;
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmCutMeat] = 0;
        _now = 33;
        Assert.True(_ctx.CheckUsePlugin(SitDown()));
        Assert.Equal(TAntiPlugActionMode.amCutMeat, _ctx.LastLockAntiPlugActionMode);

        // (c) dwCurrentInterval = 10 → 恰好命中硬超速 `<= nInterval div 10`（:9368）
        ResetCtx();
        Action(AmCutMeat).boEnabled = true;
        Action(AmCutMeat).nInterval = 100;
        Action(AmCutMeat).ProcessMode = TActionProcessMode.apmLost;
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmCutMeat] = 0;
        _now = 10;
        Assert.True(_ctx.CheckUsePlugin(SitDown()));
        Assert.Equal(TAntiPlugActionMode.amCutMeat, _ctx.LastLockAntiPlugActionMode);
    }

    [Fact]
    public void SitDown_CutMeat_CollectSpeedCombinationNeedsBothFlags()
    {
        // :9368 组合路径 `(not boContinueSpeedPass) and boCurrentSpeed and boCollectSpeed`
        TAntiPlugAction cut = Action(AmCutMeat);
        cut.boEnabled = true;                                   // nInterval = 620
        g_Config.dwCollectCount = 15;
        g_Config.dwSpeedValue = 1;
        for (int i = 0; i < 15; i++) _ctx.dwCollectIntervalArr[AmCutMeat, i] = -1;   // 含倒数第2条（:9267）
        _ctx.nCollectIntervalIndexArr[AmCutMeat] = 0;
        _ctx.GameSpeed.dwTicks[AmCutMeat] = 0;
        _now = 500;                                             // 500 < 620 → boCurrentSpeed；> 62 → 只走组合路径

        Assert.True(_ctx.CheckUsePlugin(SitDown()));
        Assert.Equal(TAntiPlugActionMode.amCutMeat, _ctx.LastLockAntiPlugActionMode);

        // 对照：dwCurrentInterval >= nInterval → boCurrentSpeed = False → 组合路径不成立
        ResetCtx();
        Action(AmCutMeat).boEnabled = true;
        g_Config.dwSpeedValue = 1;
        for (int i = 0; i < 15; i++) _ctx.dwCollectIntervalArr[AmCutMeat, i] = -1;
        _ctx.GameSpeed.dwTicks[AmCutMeat] = 0;
        _now = 700;
        Assert.False(_ctx.CheckUsePlugin(SitDown()));
    }

    [Fact]
    public void SitDown_CutMeat_CollectCountBelowTwoUsesThirdOfInterval()
    {
        // :9359 `else if dwCurrentInterval <= dwTempInterval div 3` —— dwCollectCount < 2 的旁路
        TAntiPlugAction cut = Action(AmCutMeat);
        cut.boEnabled = true;                                   // nInterval = 620
        g_Config.dwCollectCount = 1;
        _ctx.GameSpeed.dwTicks[AmCutMeat] = 0;
        _now = 200;                                             // 200 <= 206 → boCollectSpeed = True

        Assert.True(_ctx.CheckUsePlugin(SitDown()));
        Assert.Equal(TAntiPlugActionMode.amCutMeat, _ctx.LastLockAntiPlugActionMode);
        // dwCollectCount = 1 → `mod 1` 恒 0（:9425 不会除零）
        Assert.Equal(0, _ctx.nCollectIntervalIndexArr[AmCutMeat]);
        Assert.Equal(200 - 620, _ctx.dwCollectIntervalArr[AmCutMeat, 0]);
    }

    [Fact]
    public void SitDown_CutMeat_SlowerThanIntervalRecordsOne()
    {
        // :9420 `dwCurrentInterval >= dwTempInterval` → 采集值写 1（而不是负数）
        TAntiPlugAction cut = Action(AmCutMeat);
        cut.boEnabled = true;
        _ctx.GameSpeed.dwTicks[AmCutMeat] = 0;
        _now = 700;                                             // 700 >= 620

        Assert.False(_ctx.CheckUsePlugin(SitDown()));
        Assert.Equal(1, _ctx.dwCollectIntervalArr[AmCutMeat, 0]);
        Assert.Equal(1, _ctx.nCollectIntervalIndexArr[AmCutMeat]);
        Assert.Equal(700u, _ctx.GameSpeed.dwTicks[AmCutMeat]);
    }

    [Fact]
    public void SitDown_CutMeat_ApmDelaySetsDelayTimeAndSkipsTickRefresh()
    {
        // :9387 nDelayTime := dwTempInterval - dwCurrentInterval + DELAY_TIME_ADD(=10)
        // :9461 `if nDelayTime = 0` 为假 → **不刷新** dwTicks（与不延时路径的差异断言）
        TAntiPlugAction cut = Action(AmCutMeat);
        cut.boEnabled = true;
        cut.ProcessMode = TActionProcessMode.apmDelay;
        _ctx.GameSpeed.dwTicks[AmCutMeat] = 0;
        _now = 62;                                              // 62 < 620 → 硬超速 + 走延时分支

        Assert.True(_ctx.CheckUsePlugin(SitDown()));             // 收尾 apmDelay → Result = True

        Assert.Equal(1, _ctx.GameSpeed.nDelayCount[AmCutMeat]);                 // :9390
        Assert.Equal(1, _ctx.ClientMsgListProbe.Count);                         // 收尾 :9576 DelayClientMessage
        Assert.True(((TClientMsg)_ctx.ClientMsgListProbe[0]).boDelay);
        Assert.Equal(0u, _ctx.GameSpeed.dwTicks[AmCutMeat]);                    // ★ :9461 闸门 → 未刷新
        Assert.Equal(0u, _ctx.GameSpeed.dwTicks[AmCutMeatToHit]);
        Assert.Equal(0, _ctx.dwCollectIntervalArr[AmCutMeat, 0]);               // :9416 闸门 → 未采集
    }

    [Fact]
    public void SitDown_CutMeat_DelayCountOverEightSelfHealsAndSendsActionRet()
    {
        // :9391 `if nDelayCount > 8` → 归零 + nDelayTime := 0 + SendActionRet(True)
        TAntiPlugAction cut = Action(AmCutMeat);
        cut.boEnabled = true;
        cut.ProcessMode = TActionProcessMode.apmDelay;
        _ctx.GameSpeed.nDelayCount[AmCutMeat] = 8;
        _ctx.GameSpeed.dwTicks[AmCutMeat] = 0;
        _now = 62;

        Assert.True(_ctx.CheckUsePlugin(SitDown()));

        Assert.Equal(0, _ctx.GameSpeed.nDelayCount[AmCutMeat]);                 // :9393
        Assert.Single(_t.Sent);                                                 // :9395 SendActionRet(True)
        Assert.Equal(0, _ctx.ClientMsgListProbe.Count);                         // nDelayTime 已被清 0
        Assert.Equal(62u, _ctx.GameSpeed.dwTicks[AmCutMeat]);                   // nDelayTime = 0 → :9463 生效
    }

    [Fact]
    public void SitDown_CutMeat_ContinueSpeedCloseSocket_ClosesOnContinuousSpeed()
    {
        // :9270-9287：boContinueSpeedCloseSocket + 最近 nContinueSpeedCount 条全负 → ContinuousSpeed + Exit
        g_Config.boContinueSpeedCloseSocket = true;
        g_Config.nContinueSpeedCount = 4;
        TAntiPlugAction cut = Action(AmCutMeat);
        cut.boEnabled = true;                                   // nInterval = 620
        for (int i = 0; i < 15; i++) _ctx.dwCollectIntervalArr[AmCutMeat, i] = -1;   // 全负
        _ctx.nCollectIntervalIndexArr[AmCutMeat] = 0;
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmCutMeat] = 0;
        _now = 500;                                             // boCurrentSpeed = True

        Assert.False(_ctx.CheckUsePlugin(SitDown()));            // 原文 :9286 Exit

        Assert.True(_ctx.boDelayClose);                         // ContinuousSpeed :3454 DelayClose(1000)
        Assert.Single(_t.Sent);                                 // ContinuousSpeed :3453 SendMessaggeToClient
        Assert.Equal(0u, _ctx.GameSpeed.dwTicks[AmCutMeat]);    // Exit 早于 :9463
        Assert.Equal(TBaseAction.baOther, _ctx.LastActionProbe); // Exit 早于 :9468
    }

    [Fact]
    public void SitDown_CutMeat_ContinueSpeed_LatestSlotNonNegativeBreaksTheChain()
    {
        // ★ 差异断言：同一个“连续超速”配置，只要最近一条采集值 >= 0 → `Break` → boContinueSpeed = False
        //   → 不走 ContinuousSpeed，而是走普通判定（apmRebound → Result = True 且不 DelayClose）
        g_Config.boContinueSpeedCloseSocket = true;
        g_Config.nContinueSpeedCount = 4;
        TAntiPlugAction cut = Action(AmCutMeat);
        cut.boEnabled = true;
        for (int i = 0; i < 15; i++) _ctx.dwCollectIntervalArr[AmCutMeat, i] = -1;
        _ctx.dwCollectIntervalArr[AmCutMeat, (0 - 1 + 15) % 15] = 0;   // 最近一条 = 0 → :9311 `>= 0` → Break
        _ctx.nCollectIntervalIndexArr[AmCutMeat] = 0;
        _ctx.GameSpeed.dwTicks[AmCutMeat] = 0;
        _now = 500;

        Assert.True(_ctx.CheckUsePlugin(SitDown()));             // 普通超速判定（apmRebound）

        Assert.False(_ctx.boDelayClose);                        // 未走 ContinuousSpeed
        Assert.Equal(TAntiPlugActionMode.amCutMeat, _ctx.LastLockAntiPlugActionMode);
    }

    // ---------------------------------------------------------------------------------
    // 4. amMoveToCutMeat（移动到挖肉）—— 走路/跑步分支的 tick 选择（:9082-9085）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void SitDown_MoveToCutMeat_WalkUsesWalkTick_RunUsesRunTick()
    {
        // (a) FLastAction = baWalk → 取 dwTicks[amWalk]
        TAntiPlugAction mv = Action(AmMoveToCutMeat);
        mv.boEnabled = true;
        mv.nInterval = 500;
        mv.ProcessMode = TActionProcessMode.apmLost;
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _ctx.GameSpeed.dwTicks[AmRun] = 1_000;                  // 若误用 amRun 则 tick_diff 巨大 → 不判定
        _now = 50;                                              // 硬超速阈值 = 500 div 10 = 50

        Assert.True(_ctx.CheckUsePlugin(SitDown()));
        Assert.Equal(TAntiPlugActionMode.amMoveToCutMeat, _ctx.LastLockAntiPlugActionMode);   // :9193

        // (b) FLastAction = baRun → 取 dwTicks[amRun]
        ResetCtx();
        TAntiPlugAction mv2 = Action(AmMoveToCutMeat);
        mv2.boEnabled = true;
        mv2.nInterval = 500;
        mv2.ProcessMode = TActionProcessMode.apmLost;
        _ctx.LastActionForTest = TBaseAction.baRun;
        _ctx.GameSpeed.dwTicks[AmWalk] = 1_000;                 // 若误用 amWalk 则不判定
        _ctx.GameSpeed.dwTicks[AmRun] = 0;
        _now = 50;

        Assert.True(_ctx.CheckUsePlugin(SitDown()));
        Assert.Equal(TAntiPlugActionMode.amMoveToCutMeat, _ctx.LastLockAntiPlugActionMode);
    }

    [Fact]
    public void SitDown_WalkOrRunAction_SkipsCutMeatBranchWhenMoveToCutMeatDisabled()
    {
        // ★ 差异断言（D-S2，原文 :9249 的 `else if`）：FLastAction 是走路/跑步时，
        //   若 amMoveToCutMeat.boEnabled = False，则 **amCutMeat 那一整段限速被跳过**
        //   （不是“再落到 else 判一次”）—— 即使 amCutMeat 已启用且严重超速也不判定。
        TAntiPlugAction cut = Action(AmCutMeat);
        cut.boEnabled = true;                                   // nInterval = 620
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmCutMeat] = 0;
        _now = 10;                                              // 极硬超速（<= 62）

        Assert.False(_ctx.CheckUsePlugin(SitDown()));
        Assert.Empty(_t.Sent);                                  // 未触发任何判定
        Assert.Equal(TAntiPlugActionMode.amHit, _ctx.LastLockAntiPlugActionMode);
        // 但 :9461 仍然执行（nDelayTime = 0）
        Assert.Equal(10u, _ctx.GameSpeed.dwTicks[AmCutMeat]);

        // 对照：仅把 FLastAction 换成 baOther → 同一个 amCutMeat 配置立刻判定
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmCutMeat] = 0;
        _now = 10;
        Assert.True(_ctx.CheckUsePlugin(SitDown()));
        Assert.Equal(TAntiPlugActionMode.amCutMeat, _ctx.LastLockAntiPlugActionMode);
    }

    [Fact]
    public void SitDown_MoveToCutMeat_ShowHintFillsSendMsg()
    {
        // :9189-9190 `if boShowHint then sSendMsg := sHintText` → 收尾 :9524 SendMessaggeToClient
        TAntiPlugAction mv = Action(AmMoveToCutMeat);
        mv.boEnabled = true;
        mv.nInterval = 500;
        mv.ProcessMode = TActionProcessMode.apmLost;
        mv.boShowHint = true;
        mv.sHintText = "[提示]: 您的【移动挖肉】速度出现异常";
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _now = 50;

        Assert.True(_ctx.CheckUsePlugin(SitDown()));
        Assert.Single(_t.Sent);                                 // 只有提示包（apmLost 不发 SendActionRet）

        // 对照：boShowHint = False → 无提示包
        ResetCtx();
        _t.Sent.Clear();
        TAntiPlugAction mv2 = Action(AmMoveToCutMeat);
        mv2.boEnabled = true;
        mv2.nInterval = 500;
        mv2.ProcessMode = TActionProcessMode.apmLost;
        mv2.boShowHint = false;
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _now = 50;
        Assert.True(_ctx.CheckUsePlugin(SitDown()));
        Assert.Empty(_t.Sent);
    }

    [Fact]
    public void SitDown_MoveToCutMeat_ApmDelaySetsDelayTimeAndSkipsTickRefresh()
    {
        // :9205 与 :9387 同构（移动到挖肉版本）
        TAntiPlugAction mv = Action(AmMoveToCutMeat);
        mv.boEnabled = true;
        mv.nInterval = 500;
        mv.ProcessMode = TActionProcessMode.apmDelay;
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _now = 50;

        Assert.True(_ctx.CheckUsePlugin(SitDown()));

        Assert.Equal(1, _ctx.GameSpeed.nDelayCount[AmMoveToCutMeat]);           // :9208
        Assert.Equal(1, _ctx.ClientMsgListProbe.Count);                         // 收尾 DelayClientMessage
        Assert.Equal(0, _ctx.dwCollectIntervalArr[AmMoveToCutMeat, 0]);         // :9234 闸门 → 未采集
        Assert.Equal(0u, _ctx.GameSpeed.dwTicks[AmCutMeat]);                    // :9461 闸门 → 未刷新
    }

    [Fact]
    public void SitDown_MoveToCutMeat_NoSpeed_ZeroesDelayCountOnlyWhenNotContinueSpeedPass()
    {
        // :9228 `if (not Msg.boDelay) and (not boContinueSpeedPass) then nDelayCount := 0`
        TAntiPlugAction mv = Action(AmMoveToCutMeat);
        mv.boEnabled = true;
        mv.nInterval = 500;
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _now = 900;                                             // 900 >= 500 → 不快
        _ctx.GameSpeed.nDelayCount[AmMoveToCutMeat] = 5;

        // (a) boContinueSpeed = False → boContinueSpeedPass = False → 归零
        Assert.False(_ctx.CheckUsePlugin(SitDown()));
        Assert.Equal(0, _ctx.GameSpeed.nDelayCount[AmMoveToCutMeat]);

        // (b) boContinueSpeed = True 且已过 `nInterval + dwContinueSpeedPassIncTime`(500+30) → 不归零
        _ctx.GameSpeed.nDelayCount[AmMoveToCutMeat] = 5;
        _ctx.GameSpeed.boContinueSpeed = true;
        _ctx.GameSpeed.dwStartSpeedTick = 0;
        _now = 900;                                             // tick_diff(0,900) = 900 >= 530
        Assert.False(_ctx.CheckUsePlugin(SitDown()));
        Assert.Equal(5, _ctx.GameSpeed.nDelayCount[AmMoveToCutMeat]);
    }

    // ---------------------------------------------------------------------------------
    // 5. Msg.boDelay 的两个闸门（:9429 生效 / :9461 被注释掉）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void SitDown_BoDelayPacket_SkipsCollectWriteButStillRefreshesTicks()
    {
        // :9429 `if (not Msg.boDelay)` 包住调试日志；:9416 的采集写入也判 `not Msg.boDelay`；
        // 而 :9461 只有 `if nDelayTime = 0` —— `and (not Msg.boDelay)` 被 `{}` 注释掉（原文缺陷 D-S3）
        TAntiPlugAction cut = Action(AmCutMeat);
        cut.boEnabled = true;
        _ctx.GameSpeed.dwTicks[AmCutMeat] = 0;
        _now = 500;
        TProcessMsg msg = SitDown();
        msg.boDelay = true;

        Assert.False(_ctx.CheckUsePlugin(msg));

        Assert.Equal(0, _ctx.dwCollectIntervalArr[AmCutMeat, 0]);      // :9416 未执行
        Assert.Equal(0, _ctx.nCollectIntervalIndexArr[AmCutMeat]);
        Assert.Equal(500u, _ctx.GameSpeed.dwTicks[AmCutMeat]);         // ★ :9463 仍然执行
        Assert.Equal(500u, _ctx.GameSpeed.dwTicks[AmCutMeatToHit]);
    }

    // ---------------------------------------------------------------------------------
    // 6. 调试日志（:9431-9458，ErrorCode 64/65/66）与超速日志（:9217/:9399）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void SitDown_DebugLog_CutMeatWalkRunUseTheirOwnModeNamesAndTicks()
    {
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);

        // (a) FLastAction = baCutMeat → AntiPlugActionModeNames[amCutMeat]（**带尾随空格的版本**）+ dwTicks[amCutMeat]
        Action(AmCutMeat).boDebug = true;
        _ctx.LastActionForTest = TBaseAction.baCutMeat;
        _ctx.GameSpeed.dwTicks[AmCutMeat] = 0;
        _now = 500;
        _ctx.CheckUsePlugin(SitDown());
        Assert.Single(logs);
        Assert.Equal(AntiPlugActionModeNames[AmCutMeat] + ":500; 用户:挖肉角色", logs[0]);   // :9436-9437

        // (b) FLastAction = baWalk → AntiPlugActionModeNames_3[amMoveToCutMeat] + dwTicks[amWalk]
        logs.Clear();
        Action(AmMoveToCutMeat).boDebug = true;
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 100;
        _now = 700;
        _ctx.CheckUsePlugin(SitDown());
        Assert.Single(logs);
        Assert.Equal(AntiPlugActionModeNames3[AmMoveToCutMeat] + ":600; 用户:挖肉角色", logs[0]);  // :9445-9446

        // (c) FLastAction = baRun → dwTicks[amRun]
        logs.Clear();
        _ctx.LastActionForTest = TBaseAction.baRun;
        _ctx.GameSpeed.dwTicks[AmRun] = 400;
        _now = 900;
        _ctx.CheckUsePlugin(SitDown());
        Assert.Single(logs);
        Assert.Equal(AntiPlugActionModeNames3[AmMoveToCutMeat] + ":500; 用户:挖肉角色", logs[0]);  // :9455-9456
    }

    [Fact]
    public void SitDown_DebugLog_SuppressedWhenBoDelay()
    {
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        Action(AmCutMeat).boDebug = true;
        _ctx.LastActionForTest = TBaseAction.baCutMeat;
        TProcessMsg msg = SitDown();
        msg.boDelay = true;

        _ctx.CheckUsePlugin(msg);

        Assert.Empty(logs);                                     // :9429 `if (not Msg.boDelay)`
    }

    [Fact]
    public void SitDown_AttackLogFlag_LogsUserSpeedingWithModeName3()
    {
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        g_Config.boShowAttackLog = true;
        Action(AmCutMeat).boEnabled = true;
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmCutMeat] = 0;
        _now = 62;

        _ctx.CheckUsePlugin(SitDown());

        Assert.Single(logs);
        Assert.Equal("【用户超速】" + AntiPlugActionModeNames3[AmCutMeat] + ":62; 用户:挖肉角色", logs[0]);   // :9402-9405

        // 移动到挖肉的日志用 mode 22 的名字（:9220-9223）
        logs.Clear();
        ResetCtx();
        g_Config.boShowAttackLog = true;
        TAntiPlugAction mv = Action(AmMoveToCutMeat);
        mv.boEnabled = true;
        mv.nInterval = 500;
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _now = 50;
        _ctx.CheckUsePlugin(SitDown());
        Assert.Single(logs);
        Assert.Equal("【用户超速】" + AntiPlugActionModeNames3[AmMoveToCutMeat] + ":50; 用户:挖肉角色", logs[0]);
    }

    // ---------------------------------------------------------------------------------
    // 7. 与其它 ident 族的边界（CM_SITDOWN 不再走早退）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void SitDown_IsNoLongerAnUnportedEarlyReturn()
    {
        // 原 §8.2 的早退表里 CM_SITDOWN 已删除 → 现在会写 RecordActionArr / 改 FLastAction
        _ctx.GameSpeed.boContinueSpeed = true;
        _ctx.nRecordActionIndex = 0;

        Assert.False(_ctx.CheckUsePlugin(SitDown()));

        Assert.Equal(1, _ctx.nRecordActionIndex);                              // 已进入分支体
        Assert.Equal(TBaseAction.baCutMeat, _ctx.LastActionProbe);
        Assert.False(_ctx.GameSpeed.boContinueSpeed);                           // 已进入公共收尾 :9680
    }
}
