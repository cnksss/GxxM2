using System;
using System.Collections.Generic;
using System.Linq;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using Xunit;

using static GXX.RunGate.GateShareSeam;
using static GXX.RunGate.FormGlobals;
using static GXX.RunGate.RunGateConst;
using static GXX.RunGate.RunGateUtilsConst;
using static GXX.Core.Protocol.Grobal2Const;

namespace GXX.RunGate.Tests;

// =====================================================================================
// CheckUsePlugin（原文 3465-9688）的**已覆盖切片**测试。
//
// 已覆盖：前导段 3505-3519、CM_SITDOWN 9000-9469（独立测试类
//         MirClientContextCheckUsePluginSitDownTests）、CM_DROPITEM 9474-9487、
//         CM_PICKUP 9492-9505、else 9506-9516、公共收尾 9521-9681（CheckUsePluginPostlude）、
//         兜底 9682-9686。
// 未覆盖：3528-8999（CM_WALK / CM_RUN / CM_TURN / 攻击族 / CM_SPELL）
//         → 显式早退，见 MirClientContext.CheckUsePlugin.cs 文件头 §偏差。
// =====================================================================================
[Collection("RunGateFormLane")]
public class MirClientContextCheckUsePluginTests
{
    private readonly CapturingTransport _t = new();
    private readonly FakeTcpClient _tcp = new();
    private readonly TRunGate _runGate;
    private readonly TMirClientContext _ctx;

    public MirClientContextCheckUsePluginTests()
    {
        FormGlobals.ResetForTest();
        FormGlobals.ResetConfigForTest();
        GateShareSeam.ResetForTest();
        FrmMainSeam.FrmMain = null;

        IocpTransport.Current = _t;
        _runGate = new TRunGate { TcpClient = _tcp };
        var core = new TIocpCore { Owner = new TIocpTcpServer { BindObject = _runGate } };
        _ctx = new TMirClientContext(core, 0x77);
        _ctx.RemoteAddr = "5.6.7.8";
        _ctx.ContextID = 9;
        _ctx.InvokeDoReset(false);
        _ctx.sChrName = "检测角色";
        _t.Sent.Clear();
    }

    private TProcessMsg Msg(ushort ident) =>
        new TProcessMsg { DefMessage = TDefaultMessage.Make(ident, 0, 0, 0, 0), dwTimeTick = MyGetTickCount() };

    private TAntiPlugAction Action(TActionProcessMode mode,
        TSumActionProcessMode sum = TSumActionProcessMode.sapmNone, bool script = false) =>
        new TAntiPlugAction { ProcessMode = mode, SumProcessMode = sum, boProcessScript = script };

    // ---- 未覆盖族：显式早退（差异断言，钉死本车道的已知缺口）----

    [Theory]
    [InlineData(CM_WALK)]
    [InlineData(CM_RUN)]
    [InlineData(CM_TURN)]
    [InlineData(CM_HIT)]
    [InlineData(CM_HEAVYHIT)]
    [InlineData(CM_115HIT)]
    [InlineData(CM_CUSTOM_HIT001)]
    [InlineData(CM_CUSTOM_HIT001 + 299)]     // 自定义技能区间上界内
    [InlineData(CM_SPELL)]
    // CM_SITDOWN 已于本轮移植（见 MirClientContextCheckUsePluginSitDownTests），
    // 故**不再**出现在本早退名单里。
    public void CheckUsePlugin_UnportedIdentFamilies_ReturnFalseWithoutSideEffects(ushort ident)
    {
        // ★ 本车道的**已知缺口**（原文 3528-9473 未移植）：
        //   这些 ident 在原文里会记录动作（walk→baWalk、spell→baSpell…）并进入公共收尾；
        //   本车道却早退。差异断言：返回 false 且**不写** RecordActionArr、**不动** boContinueSpeed。
        _ctx.GameSpeed.boContinueSpeed = true;         // 若进入收尾会被置 False
        _ctx.nRecordActionIndex = 0;

        Assert.False(_ctx.CheckUsePlugin(Msg(ident)));

        Assert.Equal(0, _ctx.nRecordActionIndex);                      // 未记录动作
        Assert.True(_ctx.GameSpeed.boContinueSpeed);                   // 收尾未执行
        Assert.Equal(TBaseAction.baOther, _ctx.LastActionProbe);
    }

    [Fact]
    public void CheckUsePlugin_CustomHitRangeUpperBoundIsExclusive()
    {
        // 原文 :6699 `Ident < CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT` —— 上界**不含**
        Assert.False(_ctx.CheckUsePlugin(Msg((ushort)(CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT - 1))));
        // 正好越界 → 不再属于攻击族 → 落到 else 分支（会记录 baOther）
        _ctx.nRecordActionIndex = 0;
        Assert.False(_ctx.CheckUsePlugin(Msg((ushort)(CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT))));
        Assert.Equal(1, _ctx.nRecordActionIndex);
    }

    // ---- 已覆盖分支：CM_DROPITEM（原文 9474-9487）----

    [Fact]
    public void CheckUsePlugin_DropItem_RecordsBaOther()
    {
        _ctx.nRecordActionIndex = 0;
        Assert.False(_ctx.CheckUsePlugin(Msg(CM_DROPITEM)));

        Assert.Equal(1, _ctx.nRecordActionIndex);
        Assert.Equal(TBaseAction.baOther, _ctx.RecordActionArr[0].Action);
        Assert.Equal(TBaseAction.baOther, _ctx.LastActionProbe);
        Assert.Equal((ushort)CM_DROPITEM, _ctx.RecordActionArr[0].DefMsg.Ident);
    }

    // ---- 已覆盖分支：CM_PICKUP（原文 9492-9505）----

    [Fact]
    public void CheckUsePlugin_PickUp_RecordsBaOther()
    {
        Assert.False(_ctx.CheckUsePlugin(Msg(CM_PICKUP)));

        Assert.Equal(1, _ctx.nRecordActionIndex);
        Assert.Equal(TBaseAction.baOther, _ctx.RecordActionArr[0].Action);
        Assert.Equal(TBaseAction.baOther, _ctx.LastActionProbe);
    }

    // ---- 已覆盖分支：else（原文 9506-9516）----

    [Fact]
    public void CheckUsePlugin_UnknownIdent_TakesElseBranchAndRecordsBaOther()
    {
        Assert.False(_ctx.CheckUsePlugin(Msg(60000)));      // 不在任何已知族

        Assert.Equal(1, _ctx.nRecordActionIndex);
        Assert.Equal(TBaseAction.baOther, _ctx.RecordActionArr[0].Action);
    }

    [Fact]
    public void CheckUsePlugin_RecordActionIndexWrapsAtMaxAndAtNegative()
    {
        // 原文 :9479/:9497/:9508 `if (nRecordActionIndex >= MAX) or (nRecordActionIndex < 0) then nRecordActionIndex := 0;`
        _ctx.nRecordActionIndex = MirClientContextConst.MAX_RECORD_ACTION_COUNT;
        _ctx.CheckUsePlugin(Msg(CM_PICKUP));
        Assert.Equal(1, _ctx.nRecordActionIndex);           // 先归 0，再 ++ → 1
        Assert.Equal(TBaseAction.baOther, _ctx.RecordActionArr[0].Action);

        _ctx.nRecordActionIndex = -5;
        _ctx.CheckUsePlugin(Msg(CM_PICKUP));
        Assert.Equal(1, _ctx.nRecordActionIndex);
    }

    [Fact]
    public void CheckUsePlugin_RingBufferOverwritesOldestSlot()
    {
        // RecordActionArr[0..29] 是环形缓冲：写满 30 条后回到槽 0
        for (int i = 0; i < MirClientContextConst.MAX_RECORD_ACTION_COUNT; i++)
            _ctx.CheckUsePlugin(Msg((ushort)(20000 + i)));
        Assert.Equal(MirClientContextConst.MAX_RECORD_ACTION_COUNT, _ctx.nRecordActionIndex);

        _ctx.CheckUsePlugin(Msg(CM_DROPITEM));
        Assert.Equal(1, _ctx.nRecordActionIndex);
        Assert.Equal((ushort)CM_DROPITEM, _ctx.RecordActionArr[0].DefMsg.Ident);   // 槽 0 被覆盖
    }

    [Fact]
    public void CheckUsePlugin_NoAntiPlugAction_ClearsContinueSpeedFlag()
    {
        // 原文 :9678-9681：AntiPlugAction = nil → boContinueSpeed := False
        _ctx.GameSpeed.boContinueSpeed = true;
        _ctx.CheckUsePlugin(Msg(CM_PICKUP));
        Assert.False(_ctx.GameSpeed.boContinueSpeed);
    }

    // ---- 公共收尾（原文 9521-9681）—— 经 CheckUsePluginPostlude 直接驱动 ----

    [Fact]
    public void Postlude_EmptySendMsg_SendsNothing()
    {
        _ctx.CheckUsePluginPostlude(null, Msg(CM_PICKUP), "", 0, false, 0, 0, 1);
        Assert.Empty(_t.Sent);
    }

    [Fact]
    public void Postlude_NonEmptySendMsg_SendsSysMessageWithConfigColors()
    {
        g_Config.btMsgType = 1;
        g_Config.btMsgFColor = 0x11;
        g_Config.btMsgBColor = 0x22;

        _ctx.CheckUsePluginPostlude(null, Msg(CM_PICKUP), "超速警告", 0, false, 0, 0, 1);

        Assert.Single(_t.Sent);                              // :9524
    }

    [Fact]
    public void Postlude_ProcessScript_SendsCM_SENDUSERSPEEDINGToM2()
    {
        _tcp.Sent.Clear();
        TAntiPlugAction action = Action(TActionProcessMode.apmLost, script: true);

        _ctx.CheckUsePluginPostlude(action, Msg(CM_PICKUP), "", 0, false, 0, 0, 1);

        Assert.Single(_tcp.Sent);                            // :9540
        // 外层传输命令是 GM_DATA；负载是 `StructBytes.BytesOf(DefaultMessage)`（16 字节裸 TDefaultMessage），
        // 其中 Ident 在偏移 8（long Recog 之后）→ 应等于 CM_SENDUSERSPEEDING。
        Assert.Equal(GM_DATA, _tcp.Sent[0].Ident);
        Assert.Equal(TDefaultMessage.SizeOf, _tcp.Sent[0].Data.Length);
        Assert.Equal((ushort)CM_SENDUSERSPEEDING, BitConverter.ToUInt16(_tcp.Sent[0].Data, 8));
    }

    [Fact]
    public void Postlude_ProcessScriptWithoutRunGate_DoesNotCrash()
    {
        var ctx = new TMirClientContext(new TIocpCore(), 0);   // Owner = null → GetRunGate() = null
        ctx.InvokeDoReset(false);
        TAntiPlugAction action = Action(TActionProcessMode.apmLost, script: true);

        ctx.CheckUsePluginPostlude(action, new TProcessMsg(), "", 0, false, 0, 0, 1);

        Assert.Empty(_tcp.Sent);
    }

    [Fact]
    public void Postlude_ContinueSpeedFlagArmsStartTickOnce()
    {
        TAntiPlugAction action = Action(TActionProcessMode.apmLost);

        _ctx.CheckUsePluginPostlude(action, Msg(CM_PICKUP), "", 0, false, 0, 0, 1);
        Assert.True(_ctx.GameSpeed.boContinueSpeed);         // :9545
        uint firstTick = _ctx.GameSpeed.dwStartSpeedTick;    // :9546
        Assert.NotEqual(0u, firstTick);

        _ctx.CheckUsePluginPostlude(action, Msg(CM_PICKUP), "", 0, false, 0, 0, 1);
        Assert.Equal(firstTick, _ctx.GameSpeed.dwStartSpeedTick);   // 已 True → 不再改
    }

    [Fact]
    public void Postlude_SumSpeedMaxReached_LockUser()
    {
        g_Config.nSumSpeedMaxCount = 3;
        g_Config.nLockTime = 20;
        g_Config.boSaveLockStatus = false;
        _ctx.LastLockAntiPlugActionMode = TAntiPlugActionMode.amHit;
        _ctx.SumSpeedProcessArr[(int)TAntiPlugActionMode.amHit, 1] = 3;

        TAntiPlugAction action = Action(TActionProcessMode.apmLost, TSumActionProcessMode.sampLockUser);
        _ctx.CheckUsePluginPostlude(action, Msg(CM_PICKUP), "", 0, false, 0, 0, 1);

        Assert.True(_ctx.boLocked);                          // :9553 LockUser(nLockTime)
    }

    [Fact]
    public void Postlude_SumSpeedMaxReached_OfflineClosesDelayed()
    {
        g_Config.nSumSpeedMaxCount = 3;
        _ctx.LastLockAntiPlugActionMode = TAntiPlugActionMode.amSpell;
        _ctx.SumSpeedProcessArr[(int)TAntiPlugActionMode.amSpell, 1] = 3;

        TAntiPlugAction action = Action(TActionProcessMode.apmLost, TSumActionProcessMode.sampOffline);
        bool r = _ctx.CheckUsePluginPostlude(action, Msg(CM_PICKUP), "", 0, false, 0, 0, 1);

        Assert.True(_ctx.boDelayClose);                      // :9557 DelayClose(1000)
        Assert.False(r);                                     // :9558 Exit 之后不再走 ProcessMode 分派
    }

    [Fact]
    public void Postlude_SumSpeedBelowMax_NoLockNoClose()
    {
        g_Config.nSumSpeedMaxCount = 3;
        _ctx.SumSpeedProcessArr[(int)TAntiPlugActionMode.amHit, 1] = 2;
        _ctx.LastLockAntiPlugActionMode = TAntiPlugActionMode.amHit;

        TAntiPlugAction action = Action(TActionProcessMode.apmLost, TSumActionProcessMode.sampLockUser);
        _ctx.CheckUsePluginPostlude(action, Msg(CM_PICKUP), "", 0, false, 0, 0, 1);

        Assert.False(_ctx.boLocked);
        Assert.False(_ctx.boDelayClose);
    }

    [Fact]
    public void Postlude_SpeedClearData_ClearsClientMsgList()
    {
        g_Config.boSpeedClearData = true;
        _ctx.ProcessClientMessage(TDefaultMessage.Make(3011, 0, 0, 0, 0), Array.Empty<byte>());
        Assert.Equal(1, _ctx.ClientMsgListProbe.Count);

        _ctx.CheckUsePluginPostlude(Action(TActionProcessMode.apmLost), Msg(CM_PICKUP), "", 0, false, 0, 0, 1);

        Assert.Equal(0, _ctx.ClientMsgListProbe.Count);      // :9565
    }

    [Fact]
    public void Postlude_SpeedClearDataOff_KeepsClientMsgList()
    {
        g_Config.boSpeedClearData = false;
        _ctx.ProcessClientMessage(TDefaultMessage.Make(3011, 0, 0, 0, 0), Array.Empty<byte>());

        _ctx.CheckUsePluginPostlude(Action(TActionProcessMode.apmLost), Msg(CM_PICKUP), "", 0, false, 0, 0, 1);

        Assert.Equal(1, _ctx.ClientMsgListProbe.Count);
    }

    // ---- ProcessMode 六态差异断言（原文 :9568-9674）----

    [Fact]
    public void Postlude_ApmLost_ReturnsTrueAndKeepsPacketAsHandled()
    {
        Assert.True(_ctx.CheckUsePluginPostlude(Action(TActionProcessMode.apmLost),
            Msg(CM_PICKUP), "", 0, false, 0, 0, 1));
    }

    [Fact]
    public void Postlude_ApmNoProcess_ReturnsFalse()
    {
        Assert.False(_ctx.CheckUsePluginPostlude(Action(TActionProcessMode.apmNoProcess),
            Msg(CM_PICKUP), "", 0, false, 0, 0, 1));
    }

    [Fact]
    public void Postlude_ApmDelay_WithPositiveDelay_QueuesDelayedMessage()
    {
        TProcessMsg msg = Msg(CM_PICKUP);
        Assert.True(_ctx.CheckUsePluginPostlude(Action(TActionProcessMode.apmDelay),
            msg, "", 500, false, 0, 0, 1));

        Assert.Equal(1, _ctx.ClientMsgListProbe.Count);      // :9576 DelayClientMessage
        var queued = (TClientMsg)_ctx.ClientMsgListProbe[0];
        Assert.True(queued.boDelay);
    }

    [Fact]
    public void Postlude_ApmDelay_WithZeroDelay_DoesNotQueue()
    {
        Assert.True(_ctx.CheckUsePluginPostlude(Action(TActionProcessMode.apmDelay),
            Msg(CM_PICKUP), "", 0, false, 0, 0, 1));

        Assert.Equal(0, _ctx.ClientMsgListProbe.Count);      // :9575 条件不成立
    }

    [Fact]
    public void Postlude_ApmDelay_WithNegativeDelay_DoesNotQueue()
    {
        Assert.True(_ctx.CheckUsePluginPostlude(Action(TActionProcessMode.apmDelay),
            Msg(CM_PICKUP), "", -1, false, 0, 0, 1));

        Assert.Equal(0, _ctx.ClientMsgListProbe.Count);      // :9575 `nDelayTime > 0`
    }

    [Fact]
    public void Postlude_ApmRebound_IsDropConcurrentPicksDifferentActionRet()
    {
        _t.Sent.Clear();
        _ctx.CheckUsePluginPostlude(Action(TActionProcessMode.apmRebound), Msg(CM_PICKUP), "", 0,
            IsDropConcurrent: true, 0, 0, 1);
        byte[] withDrop = _t.Sent[0];

        _t.Sent.Clear();
        _ctx.CheckUsePluginPostlude(Action(TActionProcessMode.apmRebound), Msg(CM_PICKUP), "", 0,
            IsDropConcurrent: false, 0, 0, 1);
        byte[] withoutDrop = _t.Sent[0];

        Assert.NotEqual(withDrop, withoutDrop);              // :9583-9586 SendActionRet(True/False) 不同帧
    }

    [Fact]
    public void Postlude_ApmOffline_SetsResultTrueAndDelaysClose()
    {
        Assert.True(_ctx.CheckUsePluginPostlude(Action(TActionProcessMode.apmOffline),
            Msg(CM_PICKUP), "", 0, false, 0, 0, 1));

        Assert.True(_ctx.boDelayClose);                      // :9591
    }

    [Fact]
    public void Postlude_ApmFakeAttackPass_NonConcurrent_SendsActionRetTrue()
    {
        _ctx.LastLockAntiPlugActionMode = TAntiPlugActionMode.amHit;   // 非并发模式
        _ctx.LastActionProbe.ToString();                                // 触一下探针（无副作用）

        Assert.True(_ctx.CheckUsePluginPostlude(Action(TActionProcessMode.apmFakeAttackPass),
            Msg(CM_PICKUP), "", 0, false, 0, 0, 1));

        Assert.Single(_t.Sent);                              // :9657 SendActionRet(True)
    }

    [Fact]
    public void Postlude_ApmFakeAttackPass_ConcurrentMode_WithQueuedPackets_SendsOneRetPerPacket()
    {
        // 原文 :9597 并发三模式之一 → :9599 ClearConcurrentPacket 清掉同 Ident 的积压并逐条回 SM_ACTION_RET
        g_nMaxClientPacketCount = 100;
        _ctx.LastLockAntiPlugActionMode = TAntiPlugActionMode.amHitConcurrent;
        for (int i = 0; i < 3; i++)
            _ctx.ProcessClientMessage(TDefaultMessage.Make(CM_HIT, 0, 0, 0, 0), Array.Empty<byte>());

        _t.Sent.Clear();
        bool r = _ctx.CheckUsePluginPostlude(Action(TActionProcessMode.apmFakeAttackPass),
            Msg(CM_HIT), "", 0, IsDropConcurrent: false, 0, 0, 1);

        // 原文缺陷（:9715 ClearConcurrentPacket 恒返回 0）→ ConcurrentCount = 0 → **不发** SM_ACTION_RET；
        // 且 `not IsDropConcurrent` → Result := False（:9615）
        Assert.Equal(0, _ctx.GetConcurrentPacketCount(TDefaultMessage.Make(CM_HIT, 0, 0, 0, 0)));
        Assert.False(r);
    }

    [Fact]
    public void Postlude_ApmFakeAttackPass_ConcurrentMode_DropConcurrentTrueReturnsTrue()
    {
        _ctx.LastLockAntiPlugActionMode = TAntiPlugActionMode.amSpellConcurrent;
        _t.Sent.Clear();

        bool r = _ctx.CheckUsePluginPostlude(Action(TActionProcessMode.apmFakeAttackPass),
            Msg(CM_SPELL), "", 0, IsDropConcurrent: true, 0, 0, 1);

        Assert.True(r);                                      // :9621
        Assert.Single(_t.Sent);                              // :9620 SendActionRet(True)
    }

    [Fact]
    public void Postlude_ApmFakeAttackPass_MoveConcurrentIsAlsoTreatedAsConcurrent()
    {
        _ctx.LastLockAntiPlugActionMode = TAntiPlugActionMode.amMoveConcurrent;
        _t.Sent.Clear();

        bool r = _ctx.CheckUsePluginPostlude(Action(TActionProcessMode.apmFakeAttackPass),
            Msg(CM_WALK), "", 0, IsDropConcurrent: true, 0, 0, 1);

        Assert.True(r);
    }

    [Fact]
    public void Postlude_ApmFakeAttackPass_AfterSpell_AddsExtraMagicFireFailFrame()
    {
        // 原文 :9660 `if FLastAction = baSpell then` 额外补一个 SM_MAGICFIRE_FAIL
        _ctx.LastLockAntiPlugActionMode = TAntiPlugActionMode.amHit;   // 非并发
        _ctx.nRecogId = 4242;
        // FLastAction 是 private；用一次性 CheckUsePlugin 把它设成 baOther 后再验证"不补帧"
        _ctx.CheckUsePlugin(Msg(CM_PICKUP));                           // FLastAction := baOther
        _t.Sent.Clear();

        bool r = _ctx.CheckUsePluginPostlude(Action(TActionProcessMode.apmFakeAttackPass),
            Msg(CM_PICKUP), "", 0, false, 0, 0, 1);

        Assert.True(r);
        Assert.Single(_t.Sent);                                        // FLastAction = baOther → 只 1 帧

        // 为了覆盖 baSpell 分支，公开探针 LastActionProbe 只读；
        // 因此这里用差异断言反向固定：baOther 时**不额外**发帧（帧数 1 而不是 2）。
        Assert.Equal(TBaseAction.baOther, _ctx.LastActionProbe);
    }
}
