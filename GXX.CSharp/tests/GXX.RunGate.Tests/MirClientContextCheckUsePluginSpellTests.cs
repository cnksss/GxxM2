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
// `CheckUsePlugin` 的 **CM_SPELL（魔法）族**（原文 :7886-8995）移植测试。
//
// 覆盖重点：
//   * 暗杀检测的前导动作集合是 **[baTurn, baCutMeat]** —— 三族互不相同（差异断言）
//   * **魔法并发块**（:7964-8008）：`ConcurrentCount := GetConcurrentPacketCount` 的触发条件是
//     `boEnabled or boDebug`，而判定只用 `boEnabled`；命中后 `FLastAction = baSpell` 才按
//     `nSpellSpeed` 判 `IsDropConcurrent`
//   * `dwTempInterval` 来自 `g_wActionSpeedIntervals[mode][200 + nSpellSpeed]` 的三支取值
//     （±200 边界是 `<=`/`>=`；-199 不钳到槽 0）★ 限速/模式判定的核心差异
//   * 五个子块：走路/跑步/转向/挖肉到魔法（都要求 `btJob <> 0`）+ amSpell（不要求 FLastAction，
//     但同样要求 `btJob <> 0`）
//   * amSpell 的“丢弃并发”路径：`Result := True` + Exit（**唯一一处 Exit 返回 True**，D-P1），
//     并发出 `SendActionRet(True)` + `SM_MAGICFIRE_FAIL` 两帧
//   * `IsDropConcurrent` 在 :8728 是**赋值**（会把并行块已置的 True 改回 False，D-P2）
//   * 日志：只有 amSpell 子块带 `; [魔法速度%s]`；:8977 无条件 `ErrorCode := 61`（D-P3，覆盖 56-60）
//   * :8983-8992 刷新 **3 个** dwTicks 槽（amSpell / amSpellToWalk / amSpellToRun）
// =====================================================================================
[Collection("RunGateFormLane")]
public class MirClientContextCheckUsePluginSpellTests
{
    private readonly CapturingTransport _t = new();
    private readonly FakeTcpClient _tcp = new();
    private readonly TRunGate _runGate;
    private readonly TMirClientContext _ctx;
    private uint _now = 3_000_000;

    private const int AmSpell = (int)TAntiPlugActionMode.amSpell;                       // 1
    private const int AmWalk = (int)TAntiPlugActionMode.amWalk;                         // 2
    private const int AmTurn = (int)TAntiPlugActionMode.amTurn;                         // 4
    private const int AmCutMeat = (int)TAntiPlugActionMode.amCutMeat;                   // 5
    private const int AmWalkToSpell = (int)TAntiPlugActionMode.amWalkToSpell;           // 10
    private const int AmSpellToWalk = (int)TAntiPlugActionMode.amSpellToWalk;           // 11
    private const int AmSpellToRun = (int)TAntiPlugActionMode.amSpellToRun;             // 13
    private const int AmSpellConcurrent = (int)TAntiPlugActionMode.amSpellConcurrent;   // 25

    private const int Half = RunGateConst.HalfSpeedIntervalsCount;                      // 200
    private const int Last = RunGateConst.SpeedIntervalsCount - 1;                      // 400

    public MirClientContextCheckUsePluginSpellTests()
    {
        FormGlobals.ResetForTest();
        FormGlobals.ResetConfigForTest();
        GateShareSeam.ResetForTest();
        FrmMainSeam.FrmMain = null;

        IocpTransport.Current = _t;
        _runGate = new TRunGate { TcpClient = _tcp };
        var core = new TIocpCore { Owner = new TIocpTcpServer { BindObject = _runGate } };
        _ctx = new TMirClientContext(core, 0x33);
        _ctx.RemoteAddr = "3.3.3.3";
        _ctx.ContextID = 17;
        _ctx.InvokeDoReset(false);
        _ctx.sChrName = "魔法角色";
        _t.Sent.Clear();
        _t.SentText.Clear();
        _now = 3_000_000;
        MyGetTickCountProvider = () => _now;
    }

    private void ResetCtx()
    {
        _ctx.InvokeDoReset(false);
        _ctx.sChrName = "魔法角色";
        _ctx.btJob = 0;
        _ctx.nSpellSpeed = 0;
        _t.Sent.Clear();
    }

    private TProcessMsg Msg(ushort ident, uint tick)
        => new TProcessMsg { DefMessage = TDefaultMessage.Make(ident, 0, 0, 0, 0), dwTimeTick = tick };

    private TProcessMsg Spell() => Msg(CM_SPELL, _now);

    private static TRecordActionInfo Rec(TBaseAction action, uint tick)
        => new TRecordActionInfo { Action = action, Tick = tick, DefMsg = default };

    private static TAntiPlugAction Action(int mode) => g_Config.ActionList[mode];

    /// <summary>`g_wActionSpeedIntervals[mode][200 + nSpellSpeed]`（nSpellSpeed = 0 → 槽 200）。</summary>
    private static void SetInterval(int mode, int index, ushort value)
        => g_wActionSpeedIntervals[mode][index] = value;

    private void QueueSpellPackets(int count)
    {
        for (int i = 0; i < count; i++)
            _ctx.ProcessClientMessage(TDefaultMessage.Make(CM_SPELL, 0, 0, 0, 0), Array.Empty<byte>());
    }

    // ---------------------------------------------------------------------------------
    // 1. 基础骨架
    // ---------------------------------------------------------------------------------

    [Fact]
    public void Spell_RecordsBaSpellAndRefreshesThreeTicks()
    {
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.btJob = 0;
        _ctx.GameSpeed.dwTicks[AmWalk] = 777;                    // 不应被本分支刷新

        Assert.False(_ctx.CheckUsePlugin(Spell()));

        Assert.Equal(1, _ctx.nRecordActionIndex);                              // :7962 Inc
        Assert.Equal(TBaseAction.baSpell, _ctx.RecordActionArr[0].Action);     // :7895
        Assert.Equal((ushort)CM_SPELL, _ctx.RecordActionArr[0].DefMsg.Ident);  // :7897
        Assert.Equal(TBaseAction.baSpell, _ctx.LastActionProbe);               // :8994
        // :8983-8992 是**三个**槽（与 CM_SITDOWN 的三个、CM_TURN 的一个都不同）
        Assert.Equal(_now, _ctx.GameSpeed.dwTicks[AmSpell]);
        Assert.Equal(_now, _ctx.GameSpeed.dwTicks[AmSpellToWalk]);
        Assert.Equal(_now, _ctx.GameSpeed.dwTicks[AmSpellToRun]);
        Assert.Equal(777u, _ctx.GameSpeed.dwTicks[AmWalk]);                    // 未动
        Assert.Empty(_t.Sent);
    }

    [Fact]
    public void Spell_RecordActionIndexWrapsBeforeWrite()
    {
        _ctx.nRecordActionIndex = MirClientContextConst.MAX_RECORD_ACTION_COUNT;
        _ctx.CheckUsePlugin(Spell());
        Assert.Equal(1, _ctx.nRecordActionIndex);
        Assert.Equal(TBaseAction.baSpell, _ctx.RecordActionArr[0].Action);
    }

    // ---------------------------------------------------------------------------------
    // 2. 暗杀检测：前导集合 = [baTurn, baCutMeat]（★ 三族互不相同）
    // ---------------------------------------------------------------------------------

    private void SeedSpellAssasinateChain(TBaseAction preAction, uint now)
    {
        _ctx.RecordActionArr[4] = Rec(preAction, now);
        _ctx.RecordActionArr[3] = Rec(TBaseAction.baSpell, now);
        _ctx.RecordActionArr[2] = Rec(preAction, now);
        _ctx.RecordActionArr[1] = Rec(TBaseAction.baSpell, now);
        _ctx.RecordActionArr[0] = Rec(preAction, now);
        _ctx.nRecordActionIndex = 5;
        _ctx.RecordActionArr[MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1] = Rec(TBaseAction.baOther, 1);
        _now = now;
    }

    [Fact]
    public void Spell_Assasinate_PreActionSetIsTurnAndCutMeatOnly()
    {
        // baTurn / baCutMeat 算；baWalk（CM_TURN 的前导）与 baHit（CM_SITDOWN 的前导）**都不算**
        SeedSpellAssasinateChain(TBaseAction.baTurn, 3_000_000);
        _ctx.LastActionForTest = TBaseAction.baOther;
        Assert.False(_ctx.CheckUsePlugin(Spell()));
        Assert.True(_ctx.boDelayClose);                          // :7928 Exit

        ResetCtx();
        SeedSpellAssasinateChain(TBaseAction.baCutMeat, 3_000_000);
        _ctx.CheckUsePlugin(Spell());
        Assert.True(_ctx.boDelayClose);

        ResetCtx();
        SeedSpellAssasinateChain(TBaseAction.baWalk, 3_000_000);
        _ctx.CheckUsePlugin(Spell());
        Assert.False(_ctx.boDelayClose);

        ResetCtx();
        SeedSpellAssasinateChain(TBaseAction.baHit, 3_000_000);
        _ctx.CheckUsePlugin(Spell());
        Assert.False(_ctx.boDelayClose);
    }

    [Fact]
    public void Spell_Assasinate_FullVsNotFullRingBuffer()
    {
        // 采集满（取模索引，槽 0 参与）→ 3 次 → Exit；未满（`> 0` 守卫）→ 只 2 次 → 不 Exit
        SeedSpellAssasinateChain(TBaseAction.baTurn, 3_000_000);
        _ctx.CheckUsePlugin(Spell());
        Assert.True(_ctx.boDelayClose);
        Assert.Equal(5, _ctx.nRecordActionIndex);                // Exit 早于 :7962
        Assert.Equal(0u, _ctx.GameSpeed.dwTicks[AmSpell]);       // Exit 早于 :8987

        ResetCtx();
        SeedSpellAssasinateChain(TBaseAction.baTurn, 3_000_000);
        _ctx.RecordActionArr[MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1] = default;
        _ctx.CheckUsePlugin(Spell());
        Assert.False(_ctx.boDelayClose);
        Assert.Equal(6, _ctx.nRecordActionIndex);
        Assert.Equal(3_000_000u, _ctx.GameSpeed.dwTicks[AmSpell]);
    }

    // ---------------------------------------------------------------------------------
    // 3. 魔法并发块（:7964-8008）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void SpellConcurrent_BelowInterval_NoAntiPlugAction()
    {
        Action(AmSpellConcurrent).boEnabled = true;
        Action(AmSpellConcurrent).nInterval = 5;                 // 默认 1
        QueueSpellPackets(3);                                    // ConcurrentCount = 3 < 5

        Assert.False(_ctx.CheckUsePlugin(Spell()));

        Assert.Equal(TAntiPlugActionMode.amHit, _ctx.LastLockAntiPlugActionMode);
        Assert.False(_ctx.GameSpeed.boContinueSpeed);
        Assert.Empty(_t.Sent);
    }

    [Fact]
    public void SpellConcurrent_AtOrAboveInterval_SetsActionAndHint()
    {
        TAntiPlugAction conc = Action(AmSpellConcurrent);
        conc.boEnabled = true;
        conc.nInterval = 3;
        conc.boShowHint = true;
        conc.sHintText = "[提示]: 魔法并发数异常";
        QueueSpellPackets(3);
        _ctx.LastActionForTest = TBaseAction.baOther;            // → 不进入 :7991 的 baSpell 分支

        bool r = _ctx.CheckUsePlugin(Spell());

        // 默认 ProcessMode = apmFakeAttackPass + LastLock = 并发模式 → 收尾走并发分支；
        // ClearConcurrentPacket 恒返回 0（原文缺陷），IsDropConcurrent 仍为 False → Result = False
        Assert.False(r);
        Assert.Equal(TAntiPlugActionMode.amSpellConcurrent, _ctx.LastLockAntiPlugActionMode);   // :7981
        Assert.True(_ctx.GameSpeed.boContinueSpeed);                                            // 收尾 :9545
        Assert.Single(_t.Sent);                                                                 // 只有提示包
        Assert.Equal(1u, _ctx.SumSpeedProcessArr[AmSpellConcurrent, 1]);                        // :7984
    }

    [Fact]
    public void SpellConcurrent_DropConcurrentFlowsIntoPostludeAndReturnsTrue()
    {
        // 并发块命中 + FLastAction = baSpell → :8002 判 IsDropConcurrent
        // → AntiPlugAction ≠ nil → :8010 的整块被跳过 → 收尾按 IsDropConcurrent 决定 Result
        TAntiPlugAction conc = Action(AmSpellConcurrent);
        conc.boEnabled = true;
        conc.nInterval = 1;
        QueueSpellPackets(1);
        _ctx.LastActionForTest = TBaseAction.baSpell;
        SetInterval(AmSpell, Half, 30);                          // dwTempInterval = 30
        _ctx.GameSpeed.dwTicks[AmSpell] = 3_000_000 - 10;        // dwCurrentInterval = 10 <= 30 div 3 = 10
        _now = 3_000_000;

        bool r = _ctx.CheckUsePlugin(Spell());

        Assert.True(r);                                          // 收尾 apmFakeAttackPass + IsDropConcurrent → True
        Assert.Single(_t.Sent);                                  // 收尾 :9620 SendActionRet(True)
        Assert.Equal(TAntiPlugActionMode.amSpellConcurrent, _ctx.LastLockAntiPlugActionMode);
    }

    [Fact]
    public void SpellConcurrent_AboveDropThreshold_IsDropConcurrentStaysFalse()
    {
        // 同一配置，只把 dwCurrentInterval 抬高到 div 3 之上 → :8004 不执行 → 收尾 Result = False
        TAntiPlugAction conc = Action(AmSpellConcurrent);
        conc.boEnabled = true;
        conc.nInterval = 1;
        QueueSpellPackets(1);
        _ctx.LastActionForTest = TBaseAction.baSpell;
        SetInterval(AmSpell, Half, 30);
        _ctx.GameSpeed.dwTicks[AmSpell] = 3_000_000 - 11;        // 11 > 10
        _now = 3_000_000;

        bool r = _ctx.CheckUsePlugin(Spell());

        Assert.False(r);
        Assert.Empty(_t.Sent);
    }

    [Fact]
    public void SpellConcurrent_BoDebugAloneStillCountsPackets()
    {
        // :7966 的判据是 `boEnabled or boDebug`，而 :7969 的判定只用 `boEnabled`
        // → boDebug 单独打开时 ConcurrentCount 仍被计算（可由 :8979 的调试日志观测）
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        TAntiPlugAction conc = Action(AmSpellConcurrent);
        conc.boEnabled = false;
        conc.boDebug = true;
        QueueSpellPackets(4);
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.btJob = 0;

        Assert.False(_ctx.CheckUsePlugin(Spell()));

        Assert.Single(logs);                                     // :8979
        Assert.Equal(AntiPlugActionModeNames3[AmSpellConcurrent] + ":5; 用户:魔法角色", logs[0]);   // 4 + 1
        Assert.Equal(0u, _ctx.SumSpeedProcessArr[AmSpellConcurrent, 1]);   // 未判定
    }

    // ---------------------------------------------------------------------------------
    // 4. 四个“X到魔法”子块：间隔来自 g_wActionSpeedIntervals + nSpellSpeed 三支
    // ---------------------------------------------------------------------------------

    [Fact]
    public void SpellSubBlock_WalkToSpell_RequiresBtJobNonZero()
    {
        Action(AmWalkToSpell).boEnabled = true;
        Action(AmWalkToSpell).ProcessMode = TActionProcessMode.apmLost;
        SetInterval(AmWalkToSpell, Half, 100);
        _ctx.nSpellSpeed = 0;
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _now = 10;                                               // 10 <= 100 div 10 = 10 → 硬超速

        // (a) btJob = 0 → 子块被跳过（:8015 `btJob <> 0`）
        _ctx.btJob = 0;
        Assert.False(_ctx.CheckUsePlugin(Spell()));
        Assert.Equal(TAntiPlugActionMode.amHit, _ctx.LastLockAntiPlugActionMode);

        // (b) btJob <> 0 → 判定命中，mode = amWalkToSpell
        ResetCtx();
        Action(AmWalkToSpell).boEnabled = true;
        Action(AmWalkToSpell).ProcessMode = TActionProcessMode.apmLost;
        SetInterval(AmWalkToSpell, Half, 100);
        _ctx.btJob = 3;
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _now = 10;
        Assert.True(_ctx.CheckUsePlugin(Spell()));
        Assert.Equal(TAntiPlugActionMode.amWalkToSpell, _ctx.LastLockAntiPlugActionMode);
    }

    [Fact]
    public void SpellSubBlock_SpeedIntervalFollowsNSpellSpeedThreeBranches()
    {
        // ★ 限速判定的核心：`dwTempInterval` 由 nSpellSpeed 决定取哪一支
        //   槽 0（<= -200）/ 槽 200+value（-199..199）/ 槽 400（>= +200）
        Action(AmWalkToSpell).boEnabled = true;
        Action(AmWalkToSpell).ProcessMode = TActionProcessMode.apmLost;
        SetInterval(AmWalkToSpell, 0, 1000);                     // -200 分支
        SetInterval(AmWalkToSpell, 1, 1);                        // -199 → 槽 1（**不钳到槽 0**）
        SetInterval(AmWalkToSpell, Half, 1);                     // 0 → 槽 200
        SetInterval(AmWalkToSpell, Last, 500);                   // +200 分支
        _ctx.btJob = 3;
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _now = 100;

        // (a) nSpellSpeed = -200 → dwTempInterval = 1000 → 硬超速（100 <= 100）
        _ctx.nSpellSpeed = -Half;
        Assert.True(_ctx.CheckUsePlugin(Spell()));
        Assert.Equal(TAntiPlugActionMode.amWalkToSpell, _ctx.LastLockAntiPlugActionMode);

        // (b) nSpellSpeed = 0 → dwTempInterval = 1 → 完全不判定
        ResetCtx();
        Action(AmWalkToSpell).boEnabled = true;
        Action(AmWalkToSpell).ProcessMode = TActionProcessMode.apmLost;
        _ctx.btJob = 3;
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _ctx.nSpellSpeed = 0;
        _now = 100;
        Assert.False(_ctx.CheckUsePlugin(Spell()));

        // (c) nSpellSpeed = +200 → dwTempInterval = 500 → 100 <= 500 div 3 → 判定
        ResetCtx();
        Action(AmWalkToSpell).boEnabled = true;
        Action(AmWalkToSpell).ProcessMode = TActionProcessMode.apmLost;
        _ctx.btJob = 3;
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _ctx.nSpellSpeed = Half;
        _now = 100;
        Assert.True(_ctx.CheckUsePlugin(Spell()));

        // (d) nSpellSpeed = -199 → 槽 1 = 1 → 不判定（证明边界是 `<= -200` 而不是 `< 0` 之类）
        ResetCtx();
        Action(AmWalkToSpell).boEnabled = true;
        Action(AmWalkToSpell).ProcessMode = TActionProcessMode.apmLost;
        _ctx.btJob = 3;
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _ctx.nSpellSpeed = -199;
        _now = 100;
        Assert.False(_ctx.CheckUsePlugin(Spell()));
    }

    [Fact]
    public void SpellSubBlock_HitLastAction_FallsThroughToAmSpellBlock()
    {
        // 前四个子块都不匹配 baHit → 落到 amSpell 子块（:8717，条件里没有 FLastAction）
        // 注意要绕开 :8735 的“丢弃并发”：把 dwCurrentInterval 抬到 div 3 之上，
        // 并用 `nCollectIndex >= 1` 的采集路径凑出组合超速
        Action(AmSpell).boEnabled = true;
        Action(AmSpell).ProcessMode = TActionProcessMode.apmLost;
        SetInterval(AmSpell, Half, 300);
        _ctx.btJob = 3;
        _ctx.LastActionForTest = TBaseAction.baHit;
        _ctx.nCollectIntervalIndexArr[AmSpell] = 1;
        _ctx.dwCollectIntervalArr[AmSpell, 0] = -1;
        _ctx.GameSpeed.dwTicks[AmSpell] = 3_000_000 - 200;       // 200 > 300 div 3 = 100 → 非丢弃并发
        _now = 3_000_000;

        Assert.True(_ctx.CheckUsePlugin(Spell()));
        Assert.Equal(TAntiPlugActionMode.amSpell, _ctx.LastLockAntiPlugActionMode);
    }

    // ---------------------------------------------------------------------------------
    // 5. amSpell 的“丢弃并发”路径（D-P1 / D-P2）
    // ---------------------------------------------------------------------------------

    [Fact]
    public void SpellSubBlock_SpellAction_DropConcurrentExitsWithResultTrue()
    {
        // :8728 赋值 → True；:8735 命中 → SendActionRet + SM_MAGICFIRE_FAIL + Result := True + Exit
        Action(AmSpell).boEnabled = true;
        SetInterval(AmSpell, Half, 30);
        _ctx.btJob = 3;
        _ctx.LastActionForTest = TBaseAction.baOther;            // Exit 后应保持不变成 baSpell
        _ctx.GameSpeed.dwTicks[AmSpell] = 3_000_000 - 5;         // 5 <= 30 div 3 = 10
        _ctx.nRecogId = 4242;
        _now = 3_000_000;

        Assert.True(_ctx.CheckUsePlugin(Spell()));                // ★ D-P1：Exit 但 Result = True

        Assert.Equal(2, _t.Sent.Count);                          // SendActionRet(True) + SM_MAGICFIRE_FAIL
        Assert.Equal(TBaseAction.baOther, _ctx.LastActionProbe); // Exit 早于 :8994
        Assert.Equal(2_999_995u, _ctx.GameSpeed.dwTicks[AmSpell]);   // Exit 早于 :8987
        Assert.Equal(TAntiPlugActionMode.amHit, _ctx.LastLockAntiPlugActionMode);
    }

    [Fact]
    public void SpellSubBlock_SpellAction_ContinueSpeedPassSkipsDropPath()
    {
        // ★ 差异断言：同样“极快”的 tick，只要 boContinueSpeedPass 为真 → :8735 不成立 → 跳过丢弃并发；
        //   此时只有 `div 10` 硬超速那条 OR 分支能让判定成立（组合路径被 `not boContinueSpeedPass` 关掉）
        Action(AmSpell).boEnabled = true;
        Action(AmSpell).ProcessMode = TActionProcessMode.apmLost;
        SetInterval(AmSpell, Half, 100);
        _ctx.btJob = 3;
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.GameSpeed.dwTicks[AmSpell] = 3_000_000 - 5;         // 5 <= 100 div 10 = 10 → 硬超速
        _ctx.GameSpeed.boContinueSpeed = true;
        _ctx.GameSpeed.dwStartSpeedTick = 0;                     // tick_diff(0, now) 远大于 100+30
        _now = 3_000_000;

        Assert.True(_ctx.CheckUsePlugin(Spell()));

        Assert.False(_ctx.boDelayClose);
        Assert.Equal(TAntiPlugActionMode.amSpell, _ctx.LastLockAntiPlugActionMode);   // 普通判定命中
        Assert.Equal(3_000_000u, _ctx.GameSpeed.dwTicks[AmSpell]);                    // :8987 已刷新
        Assert.Equal(TBaseAction.baSpell, _ctx.LastActionProbe);
        Assert.Empty(_t.Sent);                                                        // apmLost 不发帧
    }

    [Fact]
    public void SpellSubBlock_SpellAction_DropConcurrentMakesNormalPathUnreachable()
    {
        // ★ 由 :7980/:8004 与 :8010 的组合可推出：`IsDropConcurrent = True` ⟹ `AntiPlugAction ≠ nil`
        //   ⟹ `if AntiPlugAction = nil` 整块（含 :8728 的赋值）**不可达** ——
        //   即 D-P2 的“把 True 改回 False”实际**不可观测**（:3518 每次调用都会先置 False）。
        //   本用例把这个不可达性钉死：并发块命中后，普通路径的 :8876 不会执行。
        TAntiPlugAction conc = Action(AmSpellConcurrent);
        conc.boEnabled = true;
        conc.nInterval = 1;
        QueueSpellPackets(1);
        Action(AmSpell).boEnabled = true;
        Action(AmSpell).ProcessMode = TActionProcessMode.apmLost;
        SetInterval(AmSpell, Half, 300);
        _ctx.btJob = 3;
        _ctx.LastActionForTest = TBaseAction.baSpell;
        _ctx.GameSpeed.dwTicks[AmSpell] = 3_000_000 - 50;        // 50 <= 300 div 3 = 100 → :8004 置 True
        _now = 3_000_000;

        bool r = _ctx.CheckUsePlugin(Spell());

        Assert.True(r);                                          // 收尾按 IsDropConcurrent = True
        Assert.Single(_t.Sent);                                  // :9620 SendActionRet(True)
        Assert.Equal(TAntiPlugActionMode.amSpellConcurrent, _ctx.LastLockAntiPlugActionMode);
        Assert.Equal(0u, _ctx.SumSpeedProcessArr[AmSpell, 1]);   // ★ 普通路径的 :8876 未执行
    }

    // ---------------------------------------------------------------------------------
    // 6. 日志：只有 amSpell 子块带 [魔法速度]；:8977 的无条件 ErrorCode := 61
    // ---------------------------------------------------------------------------------

    [Fact]
    public void SpellSubBlock_LogHasSpellSpeedOnlyForAmSpell()
    {
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        g_Config.boShowAttackLog = true;

        // (a) amWalkToSpell 子块：日志无速度段
        Action(AmWalkToSpell).boEnabled = true;
        Action(AmWalkToSpell).ProcessMode = TActionProcessMode.apmLost;
        SetInterval(AmWalkToSpell, Half, 100);
        _ctx.btJob = 3;
        _ctx.LastActionForTest = TBaseAction.baWalk;
        _ctx.GameSpeed.dwTicks[AmWalk] = 0;
        _now = 10;
        _ctx.CheckUsePlugin(Spell());
        Assert.Single(logs);
        Assert.Equal("【用户超速】" + AntiPlugActionModeNames3[AmWalkToSpell] + ":10; 用户:魔法角色", logs[0]);

        // (b) amSpell 子块：日志带 `[魔法速度%s]`
        logs.Clear();
        ResetCtx();
        g_Config.boShowAttackLog = true;
        Action(AmSpell).boEnabled = true;
        Action(AmSpell).ProcessMode = TActionProcessMode.apmLost;
        SetInterval(AmSpell, Half - 7, 300);                     // nSpellSpeed = -7 → 槽 193
        _ctx.btJob = 3;
        _ctx.nSpellSpeed = -7;                                   // GetSpeedText(-7) = "-7"
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.nCollectIntervalIndexArr[AmSpell] = 1;
        _ctx.dwCollectIntervalArr[AmSpell, 0] = -1;
        _ctx.GameSpeed.dwTicks[AmSpell] = 3_000_000 - 200;       // 200 > 300 div 3 → 非丢弃并发
        _now = 3_000_000;
        _ctx.CheckUsePlugin(Spell());
        Assert.Single(logs);
        Assert.Equal("【用户超速】" + AntiPlugActionModeNames3[AmSpell] + ":200; [魔法速度-7]; 用户:魔法角色", logs[0]);
    }

    [Fact]
    public void Spell_TailDebugLog_LastBranchIgnoresLastAction()
    {
        // :8970 `else if {(FLastAction = baSpell) and} amSpell.boDebug then`（D-P5，同 D-T2）
        var logs = new List<string>();
        AddMainLogMsgSink = (m, l) => logs.Add(m);
        Action(AmSpell).boDebug = true;
        _ctx.btJob = 0;
        _ctx.GameSpeed.dwTicks[AmSpell] = 0;
        _ctx.nSpellSpeed = 0;
        _ctx.LastActionForTest = TBaseAction.baOther;            // 非 walk/run/turn/cutmeat
        _now = 600;

        _ctx.CheckUsePlugin(Spell());

        Assert.Single(logs);
        Assert.Equal(AntiPlugActionModeNames3[AmSpell] + ":600; [魔法速度+0]; 用户:魔法角色", logs[0]);
    }

    // ---------------------------------------------------------------------------------
    // 7. 延时闸门：nDelayTime ≠ 0 → 三个 dwTicks 槽都不刷新
    // ---------------------------------------------------------------------------------

    [Fact]
    public void Spell_ApmDelaySkipsAllThreeTickRefreshes()
    {
        // 需要绕开“丢弃并发”（:8735）：用 nCollectIndex >= 1 的采集路径而不是 div 3 旁路，
        // 使 dwCurrentInterval(200) > dwTempInterval div 3(100) 但组合超速仍成立
        Action(AmSpell).boEnabled = true;
        Action(AmSpell).ProcessMode = TActionProcessMode.apmDelay;
        SetInterval(AmSpell, Half, 300);
        _ctx.btJob = 3;
        _ctx.LastActionForTest = TBaseAction.baOther;
        _ctx.nCollectIntervalIndexArr[AmSpell] = 1;
        _ctx.dwCollectIntervalArr[AmSpell, 0] = -1;              // nSpeedCount = 1 + 1(boCurrentSpeed) = 2 >= 2
        _ctx.GameSpeed.dwTicks[AmSpell] = 3_000_000 - 200;       // 200 < 300 → boCurrentSpeed
        _now = 3_000_000;

        Assert.True(_ctx.CheckUsePlugin(Spell()));

        Assert.Equal(1, _ctx.GameSpeed.nDelayCount[AmSpell]);     // :8888
        Assert.Equal(1, _ctx.ClientMsgListProbe.Count);          // 收尾 DelayClientMessage
        Assert.Equal(2_999_800u, _ctx.GameSpeed.dwTicks[AmSpell]);   // ★ :8983 闸门 → 保持调用前的值
        Assert.Equal(0u, _ctx.GameSpeed.dwTicks[AmSpellToWalk]);
        Assert.Equal(0u, _ctx.GameSpeed.dwTicks[AmSpellToRun]);
    }
}
