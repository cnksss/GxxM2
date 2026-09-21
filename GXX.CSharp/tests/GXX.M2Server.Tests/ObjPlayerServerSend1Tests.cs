// ============================================================================
// 测试：本车道 **ServerSend* 广播处理器片（切片 ServerSend1）**。
// 被测托管源：GXX.M2Server/Engine/PlayerSurface/TPlayObject.PlayerSurface.ServerSend1.cs
// 原文出处：Source/M2Engine/ObjPlayer.pas:36854-38859
//
// 覆盖策略（117 条方法，不写 117 个手写用例）：
//   ① **表驱动一致性测试** —— 对每条「有 `<> Self` 守卫」的处理器：
//        · BaseObject == Self  ⇒ **一个字节都不发**（守卫生效）；
//        · BaseObject <> Self  ⇒ 发出的 `m_DefMsg` 的
//          Ident / Recog / Param / Tag / Series **逐字段等于**预期。
//      对每条「无守卫」的处理器：BaseObject == Self 时**照样发**。
//   ② **手写分支测试** —— `ServerSendRush` 的 `case wIdent of`
//        （RM_PUSH / RM_RUSH / RM_RUSHKUNG / RM_100HIT / RM_MAGICMOVE /
//          RM_CUSTOM_MAGICMOVE / RM_CUSTOM_PUSH），
//        含 `RM_CUSTOM_MAGICMOVE` 的范围门（内 / 上界外 / 起点外）与
//        `RM_CUSTOM_PUSH` 对 `ProcessMsg.wParam` 的**原地改写**副作用。
//   ③ 装配细节断言（尾数据字节、多对象读取、Int64 不窄化、原文缺陷固化）。
// 全部确定性；无 `Thread.Sleep`。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

public class ObjPlayerServerSend1Tests : IDisposable
{
    private const nint SelfHandleValue = 0x00A1B2C3;
    private const nint OtherHandleValue = 0x00D4E5F6;

    public ObjPlayerServerSend1Tests() => ResetAll();
    public void Dispose() => ResetAll();

    private static void ResetAll()
    {
        PlayerSurfaceServerSendSeams.ResetDefaults();
        PlayerSurfaceSocketSeams.ResetDefaults();
        PlayerSurfaceMsgSeams.ResetDefaults();
        PlayerSurfacePortLedger.ResetNotPorted();
    }

    // ==================================================================
    // 夹具
    // ==================================================================

    /// <summary>捕获框架：记录 `SendSocketRef` / `SendSocketExRef` 两个出口。</summary>
    private sealed class SendFrame
    {
        public readonly List<TDefaultMessage> SocketMsgs = new();
        public readonly List<string> SocketTexts = new();
        public readonly List<TDefaultMessage> SocketExMsgs = new();
        public readonly List<byte[]> SocketExBufs = new();

        public int SocketCount => SocketMsgs.Count;
        public int SocketExCount => SocketExMsgs.Count;
        public int TotalCount => SocketCount + SocketExCount;
        public TDefaultMessage LastSocketMsg => SocketMsgs[^1];
    }

    /// <summary>表驱动行里的方法接收者（`Row` 是静态表，故把实例与处理器一起绑定）。</summary>
    private delegate void Receiver(TPlayObject player, TProcessMessage msg, ref bool boResult);

    private static (TPlayObject Player, TProcessMessage Msg, SendFrame Frame) NewCase(
        nint baseObject, string sMsg = "")
    {
        var player = new TPlayObject { SelfHandle = SelfHandleValue };
        var frame = new SendFrame();

        PlayerSurfaceSocketSeams.SendSocket =
            (_, msg, text) => { frame.SocketMsgs.Add(msg); frame.SocketTexts.Add(text); };
        PlayerSurfaceSocketSeams.SendSocketEx =
            (_, msg, buf) => { frame.SocketExMsgs.Add(msg); frame.SocketExBufs.Add(buf); };

        var msg2 = new TProcessMessage
        {
            BaseObject = baseObject,
            sMsg = sMsg,
            wIdent = 0,
            wParam = 0,
            nParam1 = 0,
            nParam2 = 0,
            nParam3 = 0
        };
        return (player, msg2, frame);
    }

    /// <summary>夹具里写死的输入：wParam=0x0E07 / nParam1=0x0D01 / nParam2=0x090A / nParam3=0x0B0C。</summary>
    private static void SetStdInput(TProcessMessage msg)
    {
        msg.wIdent = 0x0102;
        msg.wParam = 0x0E07;
        msg.nParam1 = 0x0D01;
        msg.nParam2 = 0x090A;
        msg.nParam3 = 0x0B0C;
    }

    // ==================================================================
    // ① 表驱动
    // ==================================================================

    /// <summary>一条表驱动行：`HasSelfGuard` 决定 `BaseObject == Self` 时是否**必须不发**。</summary>
    private sealed record Row(string Name, Receiver Handler, bool HasSelfGuard, TDefaultMessage ExpectedMsg);

    /// <summary>预期报文（`Recog` 默认即 `ProcessMsg.BaseObject` = 非 Self 句柄）。</summary>
    private static TDefaultMessage Exp(int ident, ushort param, ushort tag, ushort series, nint recog = OtherHandleValue)
        => TDefaultMessage.Make((ushort)ident, recog, param, tag, series);

    private static readonly Row[] Rows =
    {
        // ---- 带 `<> Self` 守卫（原文如此）----
        new("ServerSendWalk", (p, m, ref b) => p.ServerSendWalk(m, ref b), true,
            Exp(Grobal2Const.SM_WALK, 0x0E07, 0, 0)),
        new("ServerSendRun", (p, m, ref b) => p.ServerSendRun(m, ref b), true,
            Exp(Grobal2Const.SM_RUN, 0x0E07, 0, 0)),
        new("ServerSendHorseRun", (p, m, ref b) => p.ServerSendHorseRun(m, ref b), true,
            Exp(Grobal2Const.SM_HORSERUN, 0x0E07, 0, 0)),
        new("ServerSendHit", (p, m, ref b) => p.ServerSendHit(m, ref b), true,
            Exp(Grobal2Const.SM_HIT, 0x0E07, 0, 0)),
        new("ServerSendHeavyHit", (p, m, ref b) => p.ServerSendHeavyHit(m, ref b), true,
            Exp(Grobal2Const.SM_HEAVYHIT, 0x0E07, 0, 0)),
        new("ServerSendBigHit", (p, m, ref b) => p.ServerSendBigHit(m, ref b), true,
            Exp(Grobal2Const.SM_BIGHIT, 0x0E07, 0, 0)),
        new("ServerSendPowerHit", (p, m, ref b) => p.ServerSendPowerHit(m, ref b), true,
            Exp(Grobal2Const.SM_POWERHIT, 0x0E07, 0, 0)),
        new("ServerSendLongHit", (p, m, ref b) => p.ServerSendLongHit(m, ref b), true,
            Exp(Grobal2Const.SM_LONGHIT, 0x0E07, 0, 0)),
        new("ServerSendWideHit", (p, m, ref b) => p.ServerSendWideHit(m, ref b), true,
            Exp(Grobal2Const.SM_WIDEHIT, 0x0E07, 0, 0)),
        new("ServerSendFireHit", (p, m, ref b) => p.ServerSendFireHit(m, ref b), true,
            Exp(Grobal2Const.SM_FIREHIT, 0x0E07, 0, 0)),
        new("ServerSendCrsHit", (p, m, ref b) => p.ServerSendCrsHit(m, ref b), true,
            Exp(Grobal2Const.SM_CRSHIT, 0x0E07, 0, 0)),
        new("ServerSendSWordHit", (p, m, ref b) => p.ServerSendSWordHit(m, ref b), true,
            Exp(Grobal2Const.SM_SWORDHIT, 0x0E07, 0, 0)),
        new("ServerSendTwnHit", (p, m, ref b) => p.ServerSendTwnHit(m, ref b), true,
            Exp(Grobal2Const.SM_TWNHIT, 0x0E07, 0, 0)),
        new("ServerSend43Hit", (p, m, ref b) => p.ServerSend43Hit(m, ref b), true,
            Exp(Grobal2Const.SM_43HIT, 0x0E07, 0, 0)),
        new("ServerSend60Hit", (p, m, ref b) => p.ServerSend60Hit(m, ref b), true,
            Exp(Grobal2Const.SM_60HIT, 0x0E07, 0, 0)),
        new("ServerSend61Hit", (p, m, ref b) => p.ServerSend61Hit(m, ref b), true,
            Exp(Grobal2Const.SM_61HIT, 0x0E07, 0, 0)),
        new("ServerSend62Hit", (p, m, ref b) => p.ServerSend62Hit(m, ref b), true,
            Exp(Grobal2Const.SM_62HIT, 0x0E07, 0, 0)),
        new("ServerSend66Hit", (p, m, ref b) => p.ServerSend66Hit(m, ref b), true,
            Exp(Grobal2Const.SM_66HIT, 0x0E07, 0, 0)),
        new("ServerSend66Hit1", (p, m, ref b) => p.ServerSend66Hit1(m, ref b), true,
            Exp(Grobal2Const.SM_66HIT1, 0x0E07, 0, 0)),
        new("ServerSend101Hit", (p, m, ref b) => p.ServerSend101Hit(m, ref b), true,
            Exp(Grobal2Const.SM_101HIT, 0x0E07, 0, 0)),
        new("ServerSend102Hit", (p, m, ref b) => p.ServerSend102Hit(m, ref b), true,
            Exp(Grobal2Const.SM_102HIT, 0x0E07, 0, 0)),
        new("ServerSend103Hit", (p, m, ref b) => p.ServerSend103Hit(m, ref b), true,
            Exp(Grobal2Const.SM_103HIT, 0x0E07, 0, 0)),
        new("ServerSend113Hit", (p, m, ref b) => p.ServerSend113Hit(m, ref b), true,
            Exp(Grobal2Const.SM_113HIT, 0x0E07, 0, 0)),
        new("ServerSend115Hit", (p, m, ref b) => p.ServerSend115Hit(m, ref b), true,
            Exp(Grobal2Const.SM_115HIT, 0x0E07, 0, 0)),
        new("ServerSendButch", (p, m, ref b) => p.ServerSendButch(m, ref b), true,
            Exp(Grobal2Const.SM_BUTCH, 0x0E07, 0, 0)),
        new("ServerSendMonMove", (p, m, ref b) => p.ServerSendMonMove(m, ref b), true,
            // 原文如此：ident 用的是 SM_SITDOWN（不是 SM_MONMOVE）
            Exp(Grobal2Const.SM_SITDOWN, 0x0E07, 0, 0)),
        new("ServerSendSpell", (p, m, ref b) => p.ServerSendSpell(m, ref b), true,
            Exp(Grobal2Const.SM_SPELL, 0x0E07, 0, 0)),
        new("ServerSendSpell2", (p, m, ref b) => p.ServerSendSpell2(m, ref b), true,
            // 原文如此：ident 用的是 SM_POWERHIT（不是 SM_SPELL）
            Exp(Grobal2Const.SM_POWERHIT, 0x0E07, 0, 0)),
        new("ServerSendCustomHit", (p, m, ref b) => p.ServerSendCustomHit(m, ref b), true,
            Exp(Grobal2Const.SM_CUSTOM_HIT001 + 0x0B0C, 0x0E07, 0, 0)),

        // ---- 无 `<> Self` 守卫（原文如此）：Self 时**照样发** ----
        new("ServerSendTurnEx", (p, m, ref b) => p.ServerSendTurnEx(m, ref b), false,
            Exp(Grobal2Const.SM_TURN, 0x0E07, 0, 0)),
        new("ServerSend115HitTargetEffect", (p, m, ref b) => p.ServerSend115HitTargetEffect(m, ref b), false,
            Exp(Grobal2Const.SM_115HIT_TARGET_EFFECT, 0x0E0F, 0, 0x0E07)),
        new("ServerSendCustomHitTargetEff", (p, m, ref b) => p.ServerSendCustomHitTargetEff(m, ref b), false,
            Exp(Grobal2Const.SM_CUSTOM_HIT_TARGET_EFF, 0x0E07, 0, 0x0D01)),
        new("ServerSendCustomMagicSelfKeepPlay", (p, m, ref b) => p.ServerSendCustomMagicSelfKeepPlay(m, ref b), false,
            Exp(Grobal2Const.SM_CUSTOM_MAGIC_SELFKEEP_PLAY, 0x0E07, 0x0D01, 0)),
        new("ServerSendHear", (p, m, ref b) => p.ServerSendHear(m, ref b), false,
            Exp(Grobal2Const.SM_HEAR, 0x0D01, 0, 1)),
        new("ServerSendWhisper", (p, m, ref b) => p.ServerSendWhisper(m, ref b), false,
            Exp(Grobal2Const.SM_WHISPER, 0x0D01, 0, 1)),
        new("ServerSendCry", (p, m, ref b) => p.ServerSendCry(m, ref b), false,
            Exp(Grobal2Const.SM_CRY, 0x0D01, 0, 1)),
        new("ServerSendSysMessage", (p, m, ref b) => p.ServerSendSysMessage(m, ref b), false,
            Exp(Grobal2Const.SM_SYSMESSAGE, 0x0D01, 0x0E07, 1)),
        new("ServerSendSysMessageEx", (p, m, ref b) => p.ServerSendSysMessageEx(m, ref b), false,
            // 原文如此：与 ServerSendSysMessage 逐字相同
            Exp(Grobal2Const.SM_SYSMESSAGE, 0x0D01, 0x0E07, 1)),
        new("ServerSendGroupMessage", (p, m, ref b) => p.ServerSendGroupMessage(m, ref b), false,
            // 原文如此：ident 用的是 SM_SYSMESSAGE（不是 SM_GROUPMESSAGE）
            Exp(Grobal2Const.SM_SYSMESSAGE, 0x0D01, 0, 1)),
        new("ServerSendGuildMessage", (p, m, ref b) => p.ServerSendGuildMessage(m, ref b), false,
            Exp(Grobal2Const.SM_GUILDMESSAGE, 0x0D01, 0, 1)),
        new("ServerSendMerchantSay", (p, m, ref b) => p.ServerSendMerchantSay(m, ref b), false,
            Exp(Grobal2Const.SM_MERCHANTSAY, 0x0D01, 0, 1)),
        new("ServerSendNationMessage", (p, m, ref b) => p.ServerSendNationMessage(m, ref b), false,
            Exp(Grobal2Const.SM_NATIONMESSAGE, 0x0D01, 0, 1)),
        new("ServerSendMoveMessage", (p, m, ref b) => p.ServerSendMoveMessage(m, ref b), false,
            // 原文如此：nRecog 取的是 nParam3
            Exp(Grobal2Const.SM_MOVEMESSAGE, 0x0E07, 0x0D01, 0x090A, recog: 0x0B0C)),
        new("ServerSendMoveMessageEx", (p, m, ref b) => p.ServerSendMoveMessageEx(m, ref b), false,
            Exp(Grobal2Const.SM_MOVEMESSAGE, 0x0E07, 0x0D01, 0x090A, recog: 0x0B0C)),
        new("ServerSendNewMoveMessage", (p, m, ref b) => p.ServerSendNewMoveMessage(m, ref b), false,
            Exp(Grobal2Const.SM_MOVEMESSAGE_NEW, 0x0E07, 0x0D01, 0x090A, recog: 0x0B0C)),
        new("ServerSendDelayMessage", (p, m, ref b) => p.ServerSendDelayMessage(m, ref b), false,
            Exp(Grobal2Const.SM_DELAYMESSAGE, 0x0E07, 0x090A, 0x0B0C, recog: 0x0D01)),
        new("ServerSendMoveHintMsg", (p, m, ref b) => p.ServerSendMoveHintMsg(m, ref b), false,
            Exp(Grobal2Const.SM_MOVEHINTMSG, 0x0E07, 0x090A, 0x0B0C, recog: 0x0D01)),
        new("ServerSendCenterMessage", (p, m, ref b) => p.ServerSendCenterMessage(m, ref b), false,
            Exp(Grobal2Const.SM_CENTERMESSAGE, 0x0E07, 0x0D01, 0x090A, recog: 0x0B0C)),
        new("ServerSendCenterMessageEx", (p, m, ref b) => p.ServerSendCenterMessageEx(m, ref b), false,
            Exp(Grobal2Const.SM_CENTERMESSAGE, 0x0E07, 0x0D01, 0x090A, recog: 0x0B0C)),
        new("ServerSendTopChatBoardMessage", (p, m, ref b) => p.ServerSendTopChatBoardMessage(m, ref b), false,
            Exp(Grobal2Const.SM_TOPCHATBOARDMESSAGE, 0x0E07, 0x0D01, 0x090A, recog: 0x0B0C)),
        new("ServerSendTopChatBoardMessageEx", (p, m, ref b) => p.ServerSendTopChatBoardMessageEx(m, ref b), false,
            Exp(Grobal2Const.SM_TOPCHATBOARDMESSAGE, 0x0E07, 0x0D01, 0x090A, recog: 0x0B0C)),
        new("ServerSendAuctionBroadcastMsg", (p, m, ref b) => p.ServerSendAuctionBroadcastMsg(m, ref b), false,
            Exp(Grobal2Const.SM_AuctionBroadcastMsg, 0x0E07, 0x090A, 0x0B0C, recog: 0x0D01)),
        new("ServerSendPlayDrinkSay", (p, m, ref b) => p.ServerSendPlayDrinkSay(m, ref b), false,
            Exp(Grobal2Const.SM_PLAYDRINKSAY, 0x0E07, 0x0D01, 0x090A, recog: 0x0B0C)),
        new("ServerSendFeatureChanged", (p, m, ref b) => p.ServerSendFeatureChanged(m, ref b), false,
            Exp(Grobal2Const.SM_FEATURECHANGED_NEW, 0x0E07, 0x0D01, 0x090A)),
        new("ServerSendSetClientBuff", (p, m, ref b) => p.ServerSendSetClientBuff(m, ref b), false,
            Exp(Grobal2Const.SM_SETCLIENTBUFF, 0x0E07, 0x0D01, 0x0B0C, recog: 0x090A)),
        new("ServerSendSetArrBuff", (p, m, ref b) => p.ServerSendSetArrBuff(m, ref b), false,
            Exp(Grobal2Const.SM_SETARRBUFF, 0x0E07, 0x0D01, 0x0B0C, recog: 0x090A)),
        new("ServerSendCloseClientBuff", (p, m, ref b) => p.ServerSendCloseClientBuff(m, ref b), false,
            Exp(Grobal2Const.SM_CLOSECLIENTBUFF, 0, 0x0D01, 0, recog: 0)),
        new("ServerSendShowClientBuff", (p, m, ref b) => p.ServerSendShowClientBuff(m, ref b), false,
            Exp(Grobal2Const.SM_SHOWCLIENTBUFF, 0x0E07, 0x0D01, 0, recog: 0)),
        new("ServerSendCloseArrBuff", (p, m, ref b) => p.ServerSendCloseArrBuff(m, ref b), false,
            Exp(Grobal2Const.SM_CLOSEARRBUFF, 0, 0x0D01, 0, recog: 0)),
        new("ServerSendShowArrBuff", (p, m, ref b) => p.ServerSendShowArrBuff(m, ref b), false,
            Exp(Grobal2Const.SM_SHOWARRBUFF, 0x0E07, 0x0D01, 0, recog: 0)),
        new("ServerSendOpenBooks", (p, m, ref b) => p.ServerSendOpenBooks(m, ref b), false,
            Exp(Grobal2Const.SM_OPENBOOKS, 0x0D01, 0, 0)),
        new("ServerSendSceneShake", (p, m, ref b) => p.ServerSendSceneShake(m, ref b), false,
            Exp(Grobal2Const.SM_SCENESHAKE, 0x0D01, 0, 0, recog: 0x0E07)),
        new("ServerSendEffectStep", (p, m, ref b) => p.ServerSendEffectStep(m, ref b), false,
            Exp(Grobal2Const.SM_EFFECTSTEP, 0x0E07, 0x0D01, 0x090A)),
        new("ServerSendDelButton", (p, m, ref b) => p.ServerSendDelButton(m, ref b), false,
            Exp(Grobal2Const.SM_DELBUTTON, 0x0D01, 0, 0)),
        new("ServerSendDelArrButton", (p, m, ref b) => p.ServerSendDelArrButton(m, ref b), false,
            Exp(Grobal2Const.SM_DELARRBUTTON, 0x0D01, 0, 0)),
        new("ServerSendDelNumberButton", (p, m, ref b) => p.ServerSendDelNumberButton(m, ref b), false,
            Exp(Grobal2Const.SM_DELNUMBERBUTTON, 0x0D01, 0, 0)),
        new("ServerSendShowPhantom", (p, m, ref b) => p.ServerSendShowPhantom(m, ref b), false,
            Exp(Grobal2Const.SM_SHOWPHANTOM, 0x0E07, 0, 0)),
        new("ServerSendClosePhantom", (p, m, ref b) => p.ServerSendClosePhantom(m, ref b), false,
            Exp(Grobal2Const.SM_CLOSEPHANTOM, 0, 0, 0)),
        new("ServerSendMagicFireEx", (p, m, ref b) => p.ServerSendMagicFireEx(m, ref b), false,
            Exp(Grobal2Const.SM_MAGICFIRE_EX, 0x0D01, 0x090A, 0x0B0C, recog: 0x0E07)),
        new("ServerSendMagicFireEx2", (p, m, ref b) => p.ServerSendMagicFireEx2(m, ref b), false,
            Exp(Grobal2Const.SM_MAGICFIRE_EX_2, 0x0D01, 0x090A, 0x0B0C, recog: 0x0E07)),
        new("ServerSendDigDown", (p, m, ref b) => p.ServerSendDigDown(m, ref b), false,
            Exp(Grobal2Const.SM_DIGDOWN, 0x0E07, 0, 0)),
    };

    private static void RunRow(Row row, nint baseObject)
    {
        ResetAll();
        (TPlayObject player, TProcessMessage msg, SendFrame frame) = NewCase(baseObject);
        SetStdInput(msg);

        bool boResult = false;
        row.Handler(player, msg, ref boResult);

        if (row.HasSelfGuard && baseObject == SelfHandleValue)
        {
            // ★ 守卫：BaseObject == Self ⇒ 一个字节都不发
            Assert.Equal(0, frame.TotalCount);
            return;
        }

        Assert.Equal(1, frame.SocketCount);
        Assert.Equal(0, frame.SocketExCount);
        TDefaultMessage actual = frame.LastSocketMsg;
        Assert.Equal(row.ExpectedMsg.Ident, actual.Ident);
        Assert.Equal(row.ExpectedMsg.Recog, actual.Recog);
        Assert.Equal(row.ExpectedMsg.Param, actual.Param);
        Assert.Equal(row.ExpectedMsg.Tag, actual.Tag);
        Assert.Equal(row.ExpectedMsg.Series, actual.Series);
    }

    [Fact]
    public void TableDriven_NonSelfMessages_EmitExpectedIdentAndParams()
    {
        int guarded = 0;
        foreach (Row row in Rows)
        {
            RunRow(row, OtherHandleValue);
            if (row.HasSelfGuard) guarded++;
        }
        Assert.True(Rows.Length >= 60, "表驱动行数应 >= 60，实际 " + Rows.Length);
        Assert.True(guarded >= 25, "带 <> Self 守卫的行数应 >= 25，实际 " + guarded);
    }

    [Fact]
    public void TableDriven_SelfMessages_GuardedHandlersEmitNothing()
    {
        // 断言全部发生在 RunRow 内部
        foreach (Row row in Rows)
            RunRow(row, SelfHandleValue);
    }

    [Fact]
    public void TableDriven_GuardIsDrivenBySameObjectSeam_NotByRawEquality()
    {
        // ★ 证明守卫真的经由接缝：接成"永不相等"⇒ 带守卫者在 Self 时也会发
        ResetAll();
        (TPlayObject player, TProcessMessage msg, SendFrame frame) = NewCase(SelfHandleValue);
        PlayerSurfaceServerSendSeams.SameObject = (_, _) => false;
        bool b = false;
        player.ServerSendWalk(msg, ref b);
        Assert.Equal(1, frame.SocketCount);
        Assert.Equal(Grobal2Const.SM_WALK, frame.LastSocketMsg.Ident);

        // 反向：接成"永远相等"⇒ 非 Self 也不发
        ResetAll();
        (TPlayObject player2, TProcessMessage msg2, SendFrame frame2) = NewCase(OtherHandleValue);
        PlayerSurfaceServerSendSeams.SameObject = (_, _) => true;
        player2.ServerSendWalk(msg2, ref b);
        Assert.Equal(0, frame2.TotalCount);
    }

    // ==================================================================
    // ② ServerSendRush 的分支（原文 36894-36964）
    // ==================================================================

    private static (TPlayObject Player, TProcessMessage Msg, SendFrame Frame) RushCase(
        ushort wIdent, nint wParam = 0x0E07, nint nParam1 = 0x0D01, nint nParam2 = 0x090A,
        nint nParam3 = 0x0B0C, string sMsg = "")
    {
        (TPlayObject player, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue, sMsg);
        msg.wIdent = wIdent;
        msg.wParam = wParam;
        msg.nParam1 = nParam1;
        msg.nParam2 = nParam2;
        msg.nParam3 = nParam3;
        return (player, msg, frame);
    }

    [Fact]
    public void ServerSendRush_RmPush_UsesSmBackstepAndLiteralStepOne()
    {
        // 原文 36903-36904：SM_BACKSTEP，wSeries := MakeWord(wParam, **1**)（不是 m_nLight）
        ResetAll();
        PlayerSurfaceServerSendSeams.GetLight = _ => 0x55;
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = RushCase(Grobal2Const.RM_PUSH);
        bool b = false;
        p.ServerSendRush(msg, ref b);

        Assert.Equal(1, frame.SocketCount);
        Assert.Equal(Grobal2Const.SM_BACKSTEP, frame.LastSocketMsg.Ident);
        Assert.Equal(0x0D01, frame.LastSocketMsg.Param);
        Assert.Equal(0x090A, frame.LastSocketMsg.Tag);
        Assert.Equal(0x0E07 | (1 << 8), frame.LastSocketMsg.Series);
        Assert.NotEqual(0x0E07 | (0x55 << 8), frame.LastSocketMsg.Series);
    }

    [Fact]
    public void ServerSendRush_RmRush_UsesSmRushAndLightHighByte()
    {
        ResetAll();
        PlayerSurfaceServerSendSeams.GetLight = _ => 0x77;
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = RushCase(Grobal2Const.RM_RUSH);
        bool b = false;
        p.ServerSendRush(msg, ref b);

        Assert.Equal(Grobal2Const.SM_RUSH, frame.LastSocketMsg.Ident);
        Assert.Equal((ushort)(0x07 | (0x77 << 8)), frame.LastSocketMsg.Series);
    }

    [Fact]
    public void ServerSendRush_RmRushkung_UsesSmRushkung()
    {
        ResetAll();
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = RushCase(Grobal2Const.RM_RUSHKUNG);
        bool b = false;
        p.ServerSendRush(msg, ref b);
        Assert.Equal(Grobal2Const.SM_RUSHKUNG, frame.LastSocketMsg.Ident);
    }

    [Fact]
    public void ServerSendRush_Rm100Hit_UsesSm100HitAndParam3AsHighByte()
    {
        ResetAll();
        PlayerSurfaceServerSendSeams.GetLight = _ => 0x77;   // 本分支**不读** m_nLight
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = RushCase(Grobal2Const.RM_100HIT);
        bool b = false;
        p.ServerSendRush(msg, ref b);

        Assert.Equal(Grobal2Const.SM_100HIT, frame.LastSocketMsg.Ident);
        // 原文 36913：MakeWord(wParam, nParam3) ⇒ 高位取 nParam3 的低字节 0x0C
        Assert.Equal((ushort)(0x07 | (0x0C << 8)), frame.LastSocketMsg.Series);
        Assert.NotEqual((ushort)(0x07 | (0x77 << 8)), frame.LastSocketMsg.Series);
    }

    [Fact]
    public void ServerSendRush_RmMagicMove_UsesSmMagicMove()
    {
        ResetAll();
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = RushCase(Grobal2Const.RM_MAGICMOVE);
        bool b = false;
        p.ServerSendRush(msg, ref b);
        Assert.Equal(Grobal2Const.SM_MAGICMOVE, frame.LastSocketMsg.Ident);
    }

    [Fact]
    public void ServerSendRush_RmCustomMagicMove_InRange_EmitsCustomIdent()
    {
        // 原文 36917-36929：nIndex := nParam3 - CUSTOM_MAGIC_START_ID；命中范围后 ident := SM_CUSTOM_MAGICMOVE001 + nIndex
        ResetAll();
        const int offset = 7;
        (TPlayObject p, TProcessMessage msg, SendFrame frame) =
            RushCase(Grobal2Const.RM_CUSTOM_MAGICMOVE, nParam3: Grobal2Const.CUSTOM_MAGIC_START_ID + offset);
        bool b = false;
        p.ServerSendRush(msg, ref b);

        Assert.Equal(Grobal2Const.SM_CUSTOM_MAGICMOVE001 + offset, frame.LastSocketMsg.Ident);
    }

    [Fact]
    public void ServerSendRush_RmCustomMagicMove_AtUpperBoundary_IsOutOfRange()
    {
        // 范围门是**半开**的：nIndex < SM_CUSTOM_MAGICMOVE001 + CUSTOM_MAGIC_COUNT
        //   ⇒ nParam3 = START_ID + CUSTOM_MAGIC_COUNT 时越上界，**不写** m_DefMsg
        ResetAll();
        (TPlayObject p, TProcessMessage msg, SendFrame frame) =
            RushCase(Grobal2Const.RM_CUSTOM_MAGICMOVE,
                nParam3: Grobal2Const.CUSTOM_MAGIC_START_ID + Grobal2Const.CUSTOM_MAGIC_COUNT);
        bool b = false;
        p.ServerSendRush(msg, ref b);

        // 原文如此：case 无 else ⇒ m_DefMsg 保持上一次的值（此处是默认 0）
        Assert.Equal(1, frame.SocketCount);
        Assert.Equal(0, frame.LastSocketMsg.Ident);
    }

    [Fact]
    public void ServerSendRush_RmCustomMagicMove_BelowStartId_IsOutOfRange()
    {
        // nParam3 < CUSTOM_MAGIC_START_ID ⇒ nIndex < 0 ⇒ 整段跳过
        ResetAll();
        (TPlayObject p, TProcessMessage msg, SendFrame frame) =
            RushCase(Grobal2Const.RM_CUSTOM_MAGICMOVE, nParam3: Grobal2Const.CUSTOM_MAGIC_START_ID - 1);
        bool b = false;
        p.ServerSendRush(msg, ref b);

        Assert.Equal(1, frame.SocketCount);
        Assert.Equal(0, frame.LastSocketMsg.Ident);
    }

    [Fact]
    public void ServerSendRush_RmCustomPush_RewritesCallerWParamToLoWord()
    {
        // ★ 原文 36933：`ProcessMsg.wParam := LoWord(ProcessMsg.wParam);`
        //   这是对**调用方消息对象**的原地改写（真实副作用）。
        ResetAll();
        const int offset = 3;
        nint encoded = (nint)(((long)(Grobal2Const.CUSTOM_MAGIC_START_ID + offset) << 16) | 0x0E07);
        (TPlayObject p, TProcessMessage msg, SendFrame frame) =
            RushCase(Grobal2Const.RM_CUSTOM_PUSH, wParam: encoded, nParam3: 0x0B0C);

        Assert.Equal(encoded, msg.wParam);            // 调用前是编码值
        bool b = false;
        p.ServerSendRush(msg, ref b);

        Assert.Equal(0x0E07, msg.wParam);             // 调用后已被改写成 LoWord
        Assert.Equal(Grobal2Const.SM_CUSTOM_PUSH001 + offset, frame.LastSocketMsg.Ident);
        Assert.Equal(0x0D01, frame.LastSocketMsg.Param);
        Assert.Equal(0x090A, frame.LastSocketMsg.Tag);
        // 原文 36940：MakeWord(**wParam**, nParam3) —— 用的是**已改写后**的 wParam
        Assert.Equal((ushort)(0x07 | (0x0C << 8)), frame.LastSocketMsg.Series);
    }

    [Fact]
    public void ServerSendRush_RmCustomPush_OutOfRange_StillRewritesWParam()
    {
        // 原文顺序是**先改写 wParam、后判范围门** ⇒ 即使越界，改写仍然发生
        ResetAll();
        const int offset = Grobal2Const.CUSTOM_MAGIC_COUNT;   // 越上界
        nint encoded = (nint)(((long)(Grobal2Const.CUSTOM_MAGIC_START_ID + offset) << 16) | 0x0E07);
        (TPlayObject p, TProcessMessage msg, SendFrame frame) =
            RushCase(Grobal2Const.RM_CUSTOM_PUSH, wParam: encoded);
        bool b = false;
        p.ServerSendRush(msg, ref b);

        Assert.Equal(0x0E07, msg.wParam);
        Assert.Equal(0, frame.LastSocketMsg.Ident);
    }

    [Fact]
    public void ServerSendRush_UnknownIdent_StillSendsPreviousDefMsg()
    {
        // 原文如此：`case` 无 `else` ⇒ 未匹配的 ident 让 m_DefMsg 保持**上一次**的值后照样下发
        ResetAll();
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = RushCase(0x7FFF);
        p.m_DefMsg = TDefaultMessage.Make(0x1234, 0x99, 0x11, 0x22, 0x33);
        bool b = false;
        p.ServerSendRush(msg, ref b);

        Assert.Equal(1, frame.SocketCount);
        Assert.Equal(0x1234, frame.LastSocketMsg.Ident);
    }

    [Fact]
    public void ServerSendRush_SelfWithPushOrRush_CallsSendMapCanRun()
    {
        // 原文 36961-36963：BaseObject = Self 且 race = RC_PLAYOBJECT 且 ident ∈ {RM_PUSH, RM_RUSH}
        ResetAll();
        M2Config.SendMapCanRunCalls = 0;
        (TPlayObject player, TProcessMessage msg, SendFrame frame) = NewCase(SelfHandleValue);
        player.m_btRaceServer = (byte)Grobal2Const.RC_PLAYOBJECT;
        msg.wIdent = Grobal2Const.RM_PUSH;
        msg.wParam = 0x03;
        bool b = false;
        player.ServerSendRush(msg, ref b);

        Assert.Equal(1, M2Config.SendMapCanRunCalls);
        Assert.Equal(1, frame.SocketCount);   // ServerSendRush 本身**没有** <> Self 守卫

        // 反向：ident 换成 RM_MAGICMOVE ⇒ 不调用
        ResetAll();
        M2Config.SendMapCanRunCalls = 0;
        (TPlayObject player2, TProcessMessage msg2, SendFrame frame2) = NewCase(SelfHandleValue);
        player2.m_btRaceServer = (byte)Grobal2Const.RC_PLAYOBJECT;
        msg2.wIdent = Grobal2Const.RM_MAGICMOVE;
        player2.ServerSendRush(msg2, ref b);
        Assert.Equal(0, M2Config.SendMapCanRunCalls);
    }

    [Fact]
    public void ServerSendTurn_AlsoFiresWhenIdentIsRmTurn2()
    {
        // 原文 36862：`(BaseObject <> Self) or (wIdent = RM_TURN2)`
        ResetAll();
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(SelfHandleValue);
        msg.wIdent = Grobal2Const.RM_TURN2;
        bool b = false;
        p.ServerSendTurn(msg, ref b);

        Assert.Equal(1, frame.SocketCount);
        Assert.Equal(Grobal2Const.SM_TURN, frame.LastSocketMsg.Ident);

        ResetAll();
        (TPlayObject p2, TProcessMessage msg2, SendFrame frame2) = NewCase(SelfHandleValue);
        msg2.wIdent = 0;
        p2.ServerSendTurn(msg2, ref b);
        Assert.Equal(0, frame2.TotalCount);
    }

    // ==================================================================
    // ③ 装配细节
    // ==================================================================

    [Fact]
    public void ServerSendWalk_CharStatusTailIsRawLittleEndianInt32()
    {
        ResetAll();
        PlayerSurfaceServerSendSeams.GetCharStatus = _ => 0x12345678;
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue);
        bool b = false;
        p.ServerSendWalk(msg, ref b);

        Assert.Equal(1, frame.SocketExCount);
        Assert.Equal(new byte[] { 0x78, 0x56, 0x34, 0x12 }, frame.SocketExBufs[0]);
    }

    [Fact]
    public void ServerSendLongHit_TextTailIsFullInt64NotNarrowed()
    {
        // ★ 差异断言：原文 `IntToStr` 有 **Int64 重载** ⇒ nParam3 不被窄化成 32 位。
        ResetAll();
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue);
        msg.nParam3 = 0x1_0000_0000L + 7;
        bool b = false;
        p.ServerSendLongHit(msg, ref b);

        Assert.Equal(1, frame.SocketCount);
        Assert.Equal((0x1_0000_0000L + 7).ToString(), frame.SocketTexts[0]);
        Assert.NotEqual("7", frame.SocketTexts[0]);   // 若被 (int) 窄化就会得到 "7"
    }

    [Fact]
    public void ServerSendSpell_TextTailIsParam3PipeMsg()
    {
        ResetAll();
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue, "hello");
        msg.nParam3 = 5;
        bool b = false;
        p.ServerSendSpell(msg, ref b);
        Assert.Equal("5|hello", frame.SocketTexts[0]);
    }

    [Fact]
    public void ServerSendHear_EncodesParam1Param2IntoLowBytesOnly()
    {
        // 原文 37695：MakeWord(nParam1, nParam2) —— **只取低字节**
        ResetAll();
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue);
        msg.nParam1 = 0x1122;
        msg.nParam2 = 0x3344;
        bool b = false;
        p.ServerSendHear(msg, ref b);

        Assert.Equal(0x4422, frame.LastSocketMsg.Param);
    }

    [Fact]
    public void ServerSendDeath_Param3One_UsesNoDeathAndIgnoresGetFeature()
    {
        ResetAll();
        bool featureAsked = false;
        PlayerSurfaceServerSendSeams.GetFeature = (_, _) => { featureAsked = true; return (5, new byte[5]); };
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue);
        msg.nParam3 = 1;
        bool b = false;
        p.ServerSendDeath(msg, ref b);

        Assert.False(featureAsked);        // 原文 37962 那行被注释掉了 ⇒ Feature 恒 0
        Assert.Equal(Grobal2Const.SM_NOWDEATH, frame.LastSocketMsg.Ident);
        Assert.Equal(1, frame.SocketExCount);                                  // Feature = 0 ⇒ 走 else
        Assert.Equal(StructBytes.SizeOf<TCharDesc>(), frame.SocketExBufs[0].Length);
    }

    [Fact]
    public void ServerSendDeath_Param3NotOne_UsesDeathAndFeatureTail()
    {
        ResetAll();
        byte[] feature = { 0xAA, 0xBB, 0xCC };
        PlayerSurfaceServerSendSeams.GetFeature = (_, _) => (feature.Length, feature);
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue);
        msg.nParam3 = 0;
        bool b = false;
        p.ServerSendDeath(msg, ref b);

        Assert.Equal(Grobal2Const.SM_DEATH, frame.LastSocketMsg.Ident);
        Assert.Equal(1, frame.SocketExCount);
        int off = StructBytes.SizeOf<TCharDesc>();
        Assert.Equal(off + feature.Length, frame.SocketExBufs[0].Length);
        Assert.Equal(0xAA, frame.SocketExBufs[0][off]);
        Assert.Equal(0xBB, frame.SocketExBufs[0][off + 1]);
        Assert.Equal(0xCC, frame.SocketExBufs[0][off + 2]);
    }

    [Fact]
    public void ServerSendSkeleton_AlwaysSendsCharDescEvenWithZeroFeature()
    {
        // ⚠ 原文如此：与 ServerSendDeath 不同，本方法**无条件** SetLength + 发送
        ResetAll();
        PlayerSurfaceServerSendSeams.GetFeature = (_, _) => (0, Array.Empty<byte>());
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue);
        bool b = false;
        p.ServerSendSkeleton(msg, ref b);

        Assert.Equal(Grobal2Const.SM_SKELETON, frame.LastSocketMsg.Ident);
        Assert.Equal(1, frame.SocketExCount);
        Assert.Equal(StructBytes.SizeOf<TCharDesc>(), frame.SocketExBufs[0].Length);
    }

    [Fact]
    public void ServerSendDisppear_Param1Zero_SendsEmptyText_ElseBinaryInt64()
    {
        ResetAll();
        bool b = false;

        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue);
        msg.nParam1 = 0;
        p.ServerSendDisppear(msg, ref b);
        Assert.Equal(1, frame.SocketCount);
        Assert.Equal(0, frame.SocketExCount);

        ResetAll();
        (TPlayObject p2, TProcessMessage msg2, SendFrame frame2) = NewCase(OtherHandleValue);
        msg2.nParam1 = 0x0102030405060708L;
        p2.ServerSendDisppear(msg2, ref b);
        Assert.Equal(0, frame2.SocketCount);
        Assert.Equal(1, frame2.SocketExCount);
        Assert.Equal(new byte[] { 8, 7, 6, 5, 4, 3, 2, 1 }, frame2.SocketExBufs[0]);
    }

    [Fact]
    public void ServerSendMoveFail_RecogIsSelfHandle_ButCharStatusTailReadsBaseObject()
    {
        // ★ 差异断言：同一句里 nRecog = SelfHandle，尾数据却读 ProcessMsg.BaseObject 的 m_nCharStatus
        ResetAll();
        bool asked = false;
        nint askedFor = -1;
        PlayerSurfaceServerSendSeams.GetCharStatus = h => { asked = true; askedFor = h; return 0; };
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue);
        p.m_nCurrX = 11; p.m_nCurrY = 22; p.m_btDirection = 5;
        bool b = false;
        p.ServerSendMoveFail(msg, ref b);

        Assert.Equal(1, frame.SocketCount);
        Assert.Equal(SelfHandleValue, frame.LastSocketMsg.Recog);   // 用 Self
        Assert.True(asked);
        Assert.Equal(OtherHandleValue, askedFor);                  // 尾数据却读 BaseObject
        Assert.Equal(11, frame.LastSocketMsg.Param);
        Assert.Equal(22, frame.LastSocketMsg.Tag);
        Assert.Equal(5, frame.LastSocketMsg.Series);
    }

    [Fact]
    public void ServerSendTurn_TextTail_DependsOnRm100Hit()
    {
        ResetAll();
        bool b = false;

        PlayerSurfaceServerSendSeams.GetCharColor = _ => 42;
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue, "hi");
        msg.wIdent = Grobal2Const.RM_100HIT;
        p.ServerSendTurn(msg, ref b);
        string text100 = frame.SocketTexts[0];
        Assert.EndsWith("|hi", text100);              // 原文 36878：'|' + sMsg（**未**编码）
        Assert.DoesNotContain("/42", text100);

        ResetAll();
        PlayerSurfaceServerSendSeams.GetCharColor = _ => 42;
        (TPlayObject p2, TProcessMessage msg2, SendFrame frame2) = NewCase(OtherHandleValue, "hi");
        msg2.wIdent = 0x1234;
        p2.ServerSendTurn(msg2, ref b);
        string textOther = frame2.SocketTexts[0];
        Assert.Contains("|", textOther);
        Assert.DoesNotContain("|hi", textOther);      // 原文 36881：EncodeString(sMsg + '/' + nObjCount)
    }

    [Fact]
    public void ServerSendWinExp_GuardDirectionIsInverted()
    {
        // 原文 37820 用 `= Self`（等号）—— 与非自发送的其余族**相反**
        ResetAll();
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(SelfHandleValue);
        p.m_Abil.Exp = 777;
        msg.wParam = 0;
        bool b = false;
        p.ServerSendWinExp(msg, ref b);

        Assert.Equal(1, frame.SocketCount);
        Assert.Equal(Grobal2Const.SM_WINEXP, frame.LastSocketMsg.Ident);
        Assert.Equal(777L, frame.LastSocketMsg.Recog);
        Assert.Equal(0, frame.LastSocketMsg.Series);

        ResetAll();
        (TPlayObject p2, TProcessMessage msg2, SendFrame frame2) = NewCase(SelfHandleValue);
        p2.m_AbilNG.Exp = 888;
        msg2.wParam = 1;
        p2.ServerSendWinExp(msg2, ref b);
        Assert.Equal(888L, frame2.LastSocketMsg.Recog);
        Assert.Equal(1, frame2.LastSocketMsg.Series);
    }

    [Fact]
    public void ServerSendWinExp_NonSelfWithoutHero_SendsNothing()
    {
        ResetAll();
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue);
        bool b = false;
        p.ServerSendWinExp(msg, ref b);
        Assert.Equal(0, frame.TotalCount);   // m_MyHero = nil ⇒ 两个分支都不进
    }

    [Fact]
    public void ServerSendUserName_MysteriousManBranch_NeedsPermissionTen()
    {
        ResetAll();
        PlayerSurfaceServerSendSeams.GetRaceServer = _ => Grobal2Const.RC_PLAYOBJECT;
        PlayerSurfaceServerSendSeams.GetMysteriousMan = _ => true;
        PlayerSurfaceServerSendSeams.GetShowName = (_, _) => "SECRET-NAME";
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue, "RAW-NAME");
        p.m_btPermission = 9;
        bool b = false;
        p.ServerSendUserName(msg, ref b);
        Assert.Equal("RAW-NAME", frame.SocketTexts[0]);

        ResetAll();
        PlayerSurfaceServerSendSeams.GetRaceServer = _ => Grobal2Const.RC_PLAYOBJECT;
        PlayerSurfaceServerSendSeams.GetMysteriousMan = _ => true;
        PlayerSurfaceServerSendSeams.GetShowName = (_, _) => "SECRET-NAME";
        (TPlayObject p2, TProcessMessage msg2, SendFrame frame2) = NewCase(OtherHandleValue, "RAW-NAME");
        p2.m_btPermission = 10;
        p2.ServerSendUserName(msg2, ref b);
        Assert.Equal("SECRET-NAME", frame2.SocketTexts[0]);
    }

    [Fact]
    public void ServerSendUserName_NonPlayableRace_AlwaysUsesRawMsg()
    {
        ResetAll();
        PlayerSurfaceServerSendSeams.GetRaceServer = _ => 12345;   // 不属于三个允许的 race
        PlayerSurfaceServerSendSeams.GetMysteriousMan = _ => true;
        PlayerSurfaceServerSendSeams.GetShowName = (_, _) => "SECRET-NAME";
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue, "RAW-NAME");
        p.m_btPermission = 99;
        bool b = false;
        p.ServerSendUserName(msg, ref b);
        Assert.Equal("RAW-NAME", frame.SocketTexts[0]);
    }

    [Fact]
    public void ServerSendLevelUp_NoNilCheck_IsPreserved()
    {
        // ★ 原文缺陷固化：原文第一行**不判空**就解引用 BaseObject。
        //   托管侧同样不判空 —— 接缝在 BaseObject = 0 时照样被调用（收到 0），而不是提前 return。
        ResetAll();
        bool called = false;
        nint seen = -1;
        PlayerSurfaceServerSendSeams.GetRaceServer = h => { called = true; seen = h; return 0; };
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(0);
        bool b = false;
        p.ServerSendLevelUp(msg, ref b);

        Assert.True(called, "原文不判空 ⇒ 接缝必须被调用");
        Assert.Equal(0, seen);
    }

    [Fact]
    public void ServerSendLevelUp_PlayerBranch_Self_SendsLevelUpThenAbility()
    {
        ResetAll();
        PlayerSurfaceServerSendSeams.GetRaceServer = _ => Grobal2Const.RC_PLAYOBJECT;
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(SelfHandleValue);
        msg.wParam = 0x0042;
        p.m_nGold = 12345;
        p.m_nGameGold = 0x000A000B;
        p.m_btJob = 3;
        bool b = false;
        p.ServerSendLevelUp(msg, ref b);

        Assert.Equal(2, frame.SocketCount);
        Assert.Equal(Grobal2Const.SM_LEVELUP, frame.SocketMsgs[0].Ident);
        Assert.Equal((ushort)0x0042, frame.SocketMsgs[0].Series);
        Assert.Equal(Grobal2Const.SM_ABILITY, frame.SocketMsgs[1].Ident);
        Assert.Equal(12345L, frame.SocketMsgs[1].Recog);
        Assert.Equal((ushort)(3 | (99 << 8)), frame.SocketMsgs[1].Param);   // MakeWord(m_btJob, 99)
        Assert.Equal((ushort)0x000B, frame.SocketMsgs[1].Tag);             // LoWord(m_nGameGold)
        Assert.Equal((ushort)0x000A, frame.SocketMsgs[1].Series);          // HiWord(m_nGameGold)
    }

    [Fact]
    public void ServerSendLevelUp_PlayerBranch_NotSelf_SendsOnlyLevelUp()
    {
        ResetAll();
        PlayerSurfaceServerSendSeams.GetRaceServer = _ => Grobal2Const.RC_PLAYOBJECT;
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue);
        bool b = false;
        p.ServerSendLevelUp(msg, ref b);

        Assert.Equal(1, frame.SocketCount);
        Assert.Equal(Grobal2Const.SM_LEVELUP, frame.SocketMsgs[0].Ident);
    }

    [Fact]
    public void ServerSendAbility_CopiesAbilIntoWAbilAndSendsCompressed()
    {
        ResetAll();
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue);
        p.m_Abil.Exp = 11; p.m_Abil.MaxExp = 22; p.m_Abil.Level = 33;
        p.m_nGold = 44; p.m_nGameGold = 0x00010002; p.m_btJob = 7;
        bool b = false;
        p.ServerSendAbility(msg, ref b);

        Assert.Equal(1, frame.SocketCount);
        Assert.Equal(Grobal2Const.SM_ABILITY, frame.LastSocketMsg.Ident);
        Assert.Equal(44L, frame.LastSocketMsg.Recog);
        Assert.Equal((ushort)(7 | (99 << 8)), frame.LastSocketMsg.Param);
        // 原文 38210-38212：三字段从 m_Abil 拷进 m_WAbil
        Assert.Equal(11u, p.m_wAbil.Exp);
        Assert.Equal(22u, p.m_wAbil.MaxExp);
        Assert.Equal(33u, p.m_wAbil.Level);
    }

    [Fact]
    public void ServerSendHealthSpellChanged_NilBaseObject_ExitsWithoutSending()
    {
        // 原文 38224-38225：`if BaseObject = nil then Exit;`
        ResetAll();
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(0);
        bool b = false;
        p.ServerSendHealthSpellChanged(msg, ref b);
        Assert.Equal(0, frame.TotalCount);

        // 非 nil 且 wParam > 0 ⇒ 走 TMessageHealthSpellChangedInfo
        ResetAll();
        PlayerSurfaceServerSendSeams.GetMP = _ => 0x000A000B;
        PlayerSurfaceServerSendSeams.GetJob = _ => 2;
        (TPlayObject p2, TProcessMessage msg2, SendFrame frame2) = NewCase(OtherHandleValue);
        msg2.wParam = 5;
        p2.ServerSendHealthSpellChanged(msg2, ref b);
        Assert.Equal(1, frame2.SocketExCount);
        Assert.Equal(Grobal2Const.SM_HEALTHSPELLCHANGED, frame2.SocketExMsgs[0].Ident);
        Assert.Equal((ushort)0x000B, frame2.SocketExMsgs[0].Param);   // LoWord(MP)
        Assert.Equal((ushort)0x000A, frame2.SocketExMsgs[0].Tag);     // HiWord(MP)
        Assert.Equal((ushort)(2 | (0 << 8)), frame2.SocketExMsgs[0].Series);   // MakeWord(m_btJob, 0)
        Assert.Equal(StructBytes.SizeOf<TMessageHealthSpellChangedInfo>(), frame2.SocketExBufs[0].Length);
    }

    [Fact]
    public void ServerSendHealthSpellChanged_AttackFromPlayerMaster_SetsIsAttackFromHum()
    {
        // 原文 38237-38239：race = RC_PLAYOBJECT 或 (Master <> nil 且 Master.race = RC_PLAYOBJECT)
        ResetAll();
        PlayerSurfaceServerSendSeams.GetAttackerRaceServer = _ => 999;
        PlayerSurfaceServerSendSeams.AttackerHasMaster = _ => true;
        PlayerSurfaceServerSendSeams.GetAttackerMasterRaceServer = _ => Grobal2Const.RC_PLAYOBJECT;
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue);
        msg.wParam = 5;
        msg.nParam3 = 0xDEAD;
        bool b = false;
        p.ServerSendHealthSpellChanged(msg, ref b);

        TMessageHealthSpellChangedInfo info =
            StructBytes.FromBytes<TMessageHealthSpellChangedInfo>(frame.SocketExBufs[0]);
        Assert.Equal(1, info.IsAttackFromHum);

        // 反向：Master 也不是玩家 ⇒ 0
        ResetAll();
        PlayerSurfaceServerSendSeams.GetAttackerRaceServer = _ => 999;
        PlayerSurfaceServerSendSeams.AttackerHasMaster = _ => true;
        PlayerSurfaceServerSendSeams.GetAttackerMasterRaceServer = _ => 999;
        (TPlayObject p2, TProcessMessage msg2, SendFrame frame2) = NewCase(OtherHandleValue);
        msg2.wParam = 5;
        msg2.nParam3 = 0xDEAD;
        p2.ServerSendHealthSpellChanged(msg2, ref b);
        info = StructBytes.FromBytes<TMessageHealthSpellChangedInfo>(frame2.SocketExBufs[0]);
        Assert.Equal(0, info.IsAttackFromHum);
    }

    [Fact]
    public void ServerSendHPMPChangedFormStone_NonPositiveWParam_SendsNothingButDefMsgIsAssembled()
    {
        // ★ 原文如此：m_DefMsg 已装配好，但 wParam <= 0 时被丢弃
        ResetAll();
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue);
        msg.wParam = 0;
        bool b = false;
        p.ServerSendHPMPChangedFormStone(msg, ref b);
        Assert.Equal(0, frame.TotalCount);
        Assert.Equal(Grobal2Const.SM_HPMPCHANGED_FORM_STONE, p.m_DefMsg.Ident);   // 已装配
    }

    [Fact]
    public void ServerSendHPMPChangedFormStone_PositiveWParam_PacksStoneFlagsIntoTag()
    {
        // 原文 38288：lTag1 := MakeWord(Integer(boHPStoneHideHealthNum), Integer(boMPStoneHideHealthNum))
        ResetAll();
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue);
        msg.wParam = 1;
        bool b = false;
        p.ServerSendHPMPChangedFormStone(msg, ref b);

        Assert.Equal(1, frame.SocketExCount);
        TNewMessageBodyWL body = StructBytes.FromBytes<TNewMessageBodyWL>(frame.SocketExBufs[0]);
        Assert.Equal(0, body.lTag1);   // 两个配置默认都是 false ⇒ MakeWord(0,0) = 0
    }

    [Fact]
    public void ServerSendDayChangeing_BrightnessTable()
    {
        // 原文 38314-38333：boDARK ⇒ 1；否则 case m_nBright: 1→0, 0/2→2, 3→1, 其它→1；最后 boDAY ⇒ 0
        bool dark = true, day = false;
        PlayerSurfaceServerSendSeams.GetMapDayFlag = (_, isDay) => isDay ? day : dark;
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue);
        bool b = false;

        p.m_nBright = 1;
        p.ServerSendDayChangeing(msg, ref b);
        Assert.Equal(Grobal2Const.SM_DAYCHANGING, frame.LastSocketMsg.Ident);
        Assert.Equal(1, frame.LastSocketMsg.Param);      // m_nBright
        Assert.Equal(1, frame.LastSocketMsg.Tag);        // dark ⇒ 1

        dark = false;
        var rows = new (int Bright, int Expect)[]
        {
            (1, 0), (0, 2), (2, 2), (3, 1), (99, 1),
        };
        foreach ((int bright, int expect) in rows)
        {
            p.m_nBright = bright;
            p.ServerSendDayChangeing(msg, ref b);
            Assert.Equal(expect, frame.LastSocketMsg.Tag);
        }

        // boDAY ⇒ 无论亮度都归 0
        day = true;
        p.m_nBright = 0;
        p.ServerSendDayChangeing(msg, ref b);
        Assert.Equal(0, frame.LastSocketMsg.Tag);
    }

    [Fact]
    public void ServerSendSpaceMoveShow_IdentAndStateReset()
    {
        // 原文 38762-38769：wIdent = RM_SPACEMOVE_SHOW ⇒ SM_SPACEMOVE_SHOW，否则 SM_SPACEMOVE_SHOW2
        ResetAll();
        PlayerSurfaceServerSendSeams.MyGetTickCount = () => 0xDEADBEEF;
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue);
        bool b = false;

        msg.wIdent = Grobal2Const.RM_SPACEMOVE_SHOW;
        p.ServerSendSpaceMoveShow(msg, ref b);
        Assert.Equal(Grobal2Const.SM_SPACEMOVE_SHOW, frame.LastSocketMsg.Ident);

        // 原文 38782-38796 的 6 个 tick
        Assert.Equal(0xDEADBEEFu, p.m_dwIncGoldTick);
        Assert.Equal(0xDEADBEEFu, p.m_dwDecGoldTick);
        Assert.Equal(0xDEADBEEFu, p.m_dwIncGameGoldTick);
        Assert.Equal(0xDEADBEEFu, p.m_dwDecGameGoldTick);
        Assert.Equal(0xDEADBEEFu, p.m_dwIncGamePointTick);
        Assert.Equal(0xDEADBEEFu, p.m_dwDecGamePointTick);

        // 6 个布尔初值（注意 IncGameGold / DecGameGold 是 **False**）
        Assert.True(p.m_boIncGold);
        Assert.True(p.m_boDecGold);
        Assert.False(p.m_boIncGameGold);
        Assert.False(p.m_boDecGameGold);
        Assert.True(p.m_boIncGamePoint);
        Assert.True(p.m_boDecGamePoint);

        msg.wIdent = 0x1234;
        p.ServerSendSpaceMoveShow(msg, ref b);
        Assert.Equal(Grobal2Const.SM_SPACEMOVE_SHOW2, frame.LastSocketMsg.Ident);
    }

    [Fact]
    public void ServerSendReconnection_SetsFlag()
    {
        ResetAll();
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue, "payload");
        Assert.False(p.m_boReconnection);
        bool b = false;
        p.ServerSendReconnection(msg, ref b);
        Assert.True(p.m_boReconnection);
        Assert.Equal(0, frame.SocketCount);   // 走 SendDefMessage 接缝，不走 SendSocketRef
    }

    [Fact]
    public void ServerSendUseItems_And_ServerSendMyMagic_DelegateToTheExistingSenders()
    {
        // 原文 38361 / 38641 是两句转调（本身无守卫、无报文装配）
        ResetAll();
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(SelfHandleValue);
        bool b = false;
        p.ServerSendUseItems(msg, ref b);
        p.ServerSendMyMagic(msg, ref b);
        Assert.Equal(0, frame.TotalCount);
    }

    [Fact]
    public void ServerSendSitDown_IsAnIntentionallyEmptyBody_AndSendsNothing()
    {
        // 原文 36997-36999 就是 `begin end;` —— 既非 NotPorted 也非裸桩
        ResetAll();
        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue);
        bool b = false;
        p.ServerSendSitDown(msg, ref b);
        Assert.Equal(0, frame.TotalCount);
        Assert.Equal(0, PlayerSurfacePortLedger.NotPortedCount);
    }

    [Fact]
    public void ServerSendClearObjects_GoesThroughTheSharedSendDefMessageSeam()
    {
        // 原文 38500：无条件 SendDefMessage(SM_CLEAROBJECTS, 0,0,0,0,'')
        ResetAll();
        var seen = new List<(ushort Ident, long Recog, ushort Param, ushort Tag, ushort Series, string Msg)>();
        PlayerSurfaceMsgSeams.SendDefMessage =
            (_, ident, recog, param, tag, series, sMsg) => seen.Add((ident, recog, param, tag, series, sMsg));

        (TPlayObject p, TProcessMessage msg, SendFrame frame) = NewCase(OtherHandleValue);
        bool b = false;
        p.ServerSendClearObjects(msg, ref b);

        Assert.Single(seen);
        Assert.Equal((ushort)Grobal2Const.SM_CLEAROBJECTS, seen[0].Ident);
        Assert.Equal(0, frame.TotalCount);
    }

    // ==================================================================
    // ④ 账本（NotPorted）
    // ==================================================================

    [Fact]
    public void NotPorted_LedgerHasExactlyTheFiveBlockedHandlers()
    {
        ResetAll();
        var p = new TPlayObject();
        bool b = false;
        var msg = new TProcessMessage();

        p.ServerSendStruck(msg, ref b);
        p.ServerSendMagicshieldStruck(msg, ref b);
        p.ServerSendLogon(msg, ref b);
        p.ServerSendChangeMap(msg, ref b);
        p.ServerSendShowEvent(msg, ref b);

        Assert.Equal(5, PlayerSurfacePortLedger.NotPortedCount);
        Assert.Contains("ServerSendStruck (ObjPlayer.pas:37393)", PlayerSurfacePortLedger.NotPortedMethods);
        Assert.Contains("ServerSendMagicshieldStruck (ObjPlayer.pas:37577)", PlayerSurfacePortLedger.NotPortedMethods);
        Assert.Contains("ServerSendLogon (ObjPlayer.pas:38001)", PlayerSurfacePortLedger.NotPortedMethods);
        Assert.Contains("ServerSendChangeMap (ObjPlayer.pas:38503)", PlayerSurfacePortLedger.NotPortedMethods);
        Assert.Contains("ServerSendShowEvent (ObjPlayer.pas:38812)", PlayerSurfacePortLedger.NotPortedMethods);
    }

    [Fact]
    public void NotPorted_LedgerIsIdempotent()
    {
        ResetAll();
        var p = new TPlayObject();
        bool b = false;
        var msg = new TProcessMessage();
        p.ServerSendStruck(msg, ref b);
        p.ServerSendStruck(msg, ref b);
        p.ServerSendStruck(msg, ref b);
        Assert.Equal(1, PlayerSurfacePortLedger.NotPortedCount);
    }

    // ==================================================================
    // ⑤ 接缝默认值 + 117 条签名的结构性固化
    // ==================================================================

    [Fact]
    public void Seams_DefaultsAreDocumentedAndSafe()
    {
        ResetAll();
        Assert.True(PlayerSurfaceServerSendSeams.SameObject(7, 7));
        Assert.False(PlayerSurfaceServerSendSeams.SameObject(7, 8));
        Assert.False(PlayerSurfaceServerSendSeams.ObjectPresent(0));
        Assert.True(PlayerSurfaceServerSendSeams.ObjectPresent(1));

        Assert.Equal(0, PlayerSurfaceServerSendSeams.GetLight(123));
        Assert.Equal(0, PlayerSurfaceServerSendSeams.GetCharStatus(123));
        Assert.Equal(0, PlayerSurfaceServerSendSeams.GetDirection(123));
        Assert.Equal(0, PlayerSurfaceServerSendSeams.GetCurrX(123));
        Assert.Equal(0, PlayerSurfaceServerSendSeams.GetCurrY(123));
        Assert.Equal(0, PlayerSurfaceServerSendSeams.GetJob(123));
        Assert.Equal(0, PlayerSurfaceServerSendSeams.GetRaceServer(123));
        Assert.Equal(0, PlayerSurfaceServerSendSeams.GetAttackerRaceServer(123));
        Assert.False(PlayerSurfaceServerSendSeams.AttackerHasMaster(123));
        Assert.Equal(0, PlayerSurfaceServerSendSeams.GetAttackerMasterRaceServer(123));
        Assert.False(PlayerSurfaceServerSendSeams.GetMysteriousMan(123));
        Assert.Equal("", PlayerSurfaceServerSendSeams.GetShowName(123, true));
        Assert.Equal(0, PlayerSurfaceServerSendSeams.GetHP(123));
        Assert.Equal(0, PlayerSurfaceServerSendSeams.GetMaxHP(123));
        Assert.Equal(0, PlayerSurfaceServerSendSeams.GetMP(123));
        Assert.Equal(0, PlayerSurfaceServerSendSeams.GetMaxMP(123));
        Assert.Equal(0, PlayerSurfaceServerSendSeams.GetLevel(123));
        Assert.False(PlayerSurfaceServerSendSeams.HasEnvir(123));
        Assert.Equal(0, PlayerSurfaceServerSendSeams.GetSecretFlag(123));
        Assert.Equal(0, PlayerSurfaceServerSendSeams.GetSecretFlag2(123));
        Assert.False(PlayerSurfaceServerSendSeams.GetMapDayFlag(new TPlayObject(), false));
        Assert.Equal(0, PlayerSurfaceServerSendSeams.GetCharColor(123));

        (int f, byte[] bytes) = PlayerSurfaceServerSendSeams.GetFeature(123, new TPlayObject());
        Assert.Equal(0, f);
        Assert.Empty(bytes);
    }

    [Fact]
    public void AllPortedHandlers_BindToTheUnifiedReceiverSignature()
    {
        // 结构性固化：117 条 ServerSend* 全部能以同一个委托签名绑定
        //（编译通过即证明"参数是 TProcessMessage + ref bool、返回 void"三者一致）
        var handlers = new List<Receiver>
        {
            (p, m, ref b) => p.ServerSendTurn(m, ref b),
            (p, m, ref b) => p.ServerSendTurnEx(m, ref b),
            (p, m, ref b) => p.ServerSendRush(m, ref b),
            (p, m, ref b) => p.ServerSendWalk(m, ref b),
            (p, m, ref b) => p.ServerSendRun(m, ref b),
            (p, m, ref b) => p.ServerSendHorseRun(m, ref b),
            (p, m, ref b) => p.ServerSendSitDown(m, ref b),
            (p, m, ref b) => p.ServerSendHit(m, ref b),
            (p, m, ref b) => p.ServerSendHeavyHit(m, ref b),
            (p, m, ref b) => p.ServerSendBigHit(m, ref b),
            (p, m, ref b) => p.ServerSendPowerHit(m, ref b),
            (p, m, ref b) => p.ServerSendLongHit(m, ref b),
            (p, m, ref b) => p.ServerSendWideHit(m, ref b),
            (p, m, ref b) => p.ServerSendFireHit(m, ref b),
            (p, m, ref b) => p.ServerSendCrsHit(m, ref b),
            (p, m, ref b) => p.ServerSendSWordHit(m, ref b),
            (p, m, ref b) => p.ServerSendTwnHit(m, ref b),
            (p, m, ref b) => p.ServerSend43Hit(m, ref b),
            (p, m, ref b) => p.ServerSend60Hit(m, ref b),
            (p, m, ref b) => p.ServerSend61Hit(m, ref b),
            (p, m, ref b) => p.ServerSend62Hit(m, ref b),
            (p, m, ref b) => p.ServerSend66Hit(m, ref b),
            (p, m, ref b) => p.ServerSend66Hit1(m, ref b),
            (p, m, ref b) => p.ServerSend101Hit(m, ref b),
            (p, m, ref b) => p.ServerSend102Hit(m, ref b),
            (p, m, ref b) => p.ServerSend103Hit(m, ref b),
            (p, m, ref b) => p.ServerSend113Hit(m, ref b),
            (p, m, ref b) => p.ServerSend115Hit(m, ref b),
            (p, m, ref b) => p.ServerSend115HitTargetEffect(m, ref b),
            (p, m, ref b) => p.ServerSendCustomHit(m, ref b),
            (p, m, ref b) => p.ServerSendCustomHitTargetEff(m, ref b),
            (p, m, ref b) => p.ServerSendCustomMagicSelfKeepPlay(m, ref b),
            (p, m, ref b) => p.ServerSendMonMove(m, ref b),
            (p, m, ref b) => p.ServerSendHealthSpellChangedStruck(m, ref b),
            (p, m, ref b) => p.ServerSendStruck(m, ref b),
            (p, m, ref b) => p.ServerSendMagicshieldStruck(m, ref b),
            (p, m, ref b) => p.ServerSendHear(m, ref b),
            (p, m, ref b) => p.ServerSendWhisper(m, ref b),
            (p, m, ref b) => p.ServerSendCry(m, ref b),
            (p, m, ref b) => p.ServerSendSysMessage(m, ref b),
            (p, m, ref b) => p.ServerSendSysMessageEx(m, ref b),
            (p, m, ref b) => p.ServerSendGroupMessage(m, ref b),
            (p, m, ref b) => p.ServerSendGuildMessage(m, ref b),
            (p, m, ref b) => p.ServerSendMerchantSay(m, ref b),
            (p, m, ref b) => p.ServerSendMoveMessage(m, ref b),
            (p, m, ref b) => p.ServerSendMoveMessageEx(m, ref b),
            (p, m, ref b) => p.ServerSendNewMoveMessage(m, ref b),
            (p, m, ref b) => p.ServerSendDelayMessage(m, ref b),
            (p, m, ref b) => p.ServerSendMoveHintMsg(m, ref b),
            (p, m, ref b) => p.ServerSendCenterMessage(m, ref b),
            (p, m, ref b) => p.ServerSendCenterMessageEx(m, ref b),
            (p, m, ref b) => p.ServerSendTopChatBoardMessage(m, ref b),
            (p, m, ref b) => p.ServerSendTopChatBoardMessageEx(m, ref b),
            (p, m, ref b) => p.ServerSendAuctionBroadcastMsg(m, ref b),
            (p, m, ref b) => p.ServerSendPlayDrinkSay(m, ref b),
            (p, m, ref b) => p.ServerSendWinExp(m, ref b),
            (p, m, ref b) => p.ServerSendUserName(m, ref b),
            (p, m, ref b) => p.ServerSendLevelUp(m, ref b),
            (p, m, ref b) => p.ServerSendChangeNameColor(m, ref b),
            (p, m, ref b) => p.ServerSendSpell(m, ref b),
            (p, m, ref b) => p.ServerSendSpell2(m, ref b),
            (p, m, ref b) => p.ServerSendSpell3(m, ref b),
            (p, m, ref b) => p.ServerSendMoveFail(m, ref b),
            (p, m, ref b) => p.ServerSendDeath(m, ref b),
            (p, m, ref b) => p.ServerSendDisppear(m, ref b),
            (p, m, ref b) => p.ServerSendLogon(m, ref b),
            (p, m, ref b) => p.ServerSendAbility(m, ref b),
            (p, m, ref b) => p.ServerSendHealthSpellChanged(m, ref b),
            (p, m, ref b) => p.ServerSendHPMPChangedFormStone(m, ref b),
            (p, m, ref b) => p.ServerSendDayChangeing(m, ref b),
            (p, m, ref b) => p.ServerSendItemShow(m, ref b),
            (p, m, ref b) => p.ServerSendItemHide(m, ref b),
            (p, m, ref b) => p.ServerSendDoorOpen(m, ref b),
            (p, m, ref b) => p.ServerSendDoorClose(m, ref b),
            (p, m, ref b) => p.ServerSendUseItems(m, ref b),
            (p, m, ref b) => p.ServerSendWeightChanged(m, ref b),
            (p, m, ref b) => p.ServerSendFeatureChanged(m, ref b),
            (p, m, ref b) => p.ServerSendNationMessage(m, ref b),
            (p, m, ref b) => p.ServerSendSetClientBuff(m, ref b),
            (p, m, ref b) => p.ServerSendCloseClientBuff(m, ref b),
            (p, m, ref b) => p.ServerSendShowClientBuff(m, ref b),
            (p, m, ref b) => p.ServerSendSetArrBuff(m, ref b),
            (p, m, ref b) => p.ServerSendCloseArrBuff(m, ref b),
            (p, m, ref b) => p.ServerSendShowArrBuff(m, ref b),
            (p, m, ref b) => p.ServerSendOpenBooks(m, ref b),
            (p, m, ref b) => p.ServerSendSceneShake(m, ref b),
            (p, m, ref b) => p.ServerSendEffectStep(m, ref b),
            (p, m, ref b) => p.ServerSendAddButton(m, ref b),
            (p, m, ref b) => p.ServerSendDelButton(m, ref b),
            (p, m, ref b) => p.ServerSendAddArrButton(m, ref b),
            (p, m, ref b) => p.ServerSendDelArrButton(m, ref b),
            (p, m, ref b) => p.ServerSendAddNumberButton(m, ref b),
            (p, m, ref b) => p.ServerSendDelNumberButton(m, ref b),
            (p, m, ref b) => p.ServerSendShowPhantom(m, ref b),
            (p, m, ref b) => p.ServerSendClosePhantom(m, ref b),
            (p, m, ref b) => p.ServerSendClearObjects(m, ref b),
            (p, m, ref b) => p.ServerSendChangeMap(m, ref b),
            (p, m, ref b) => p.ServerSendButch(m, ref b),
            (p, m, ref b) => p.ServerSendMagicFire(m, ref b),
            (p, m, ref b) => p.ServerSendMagicFireEx(m, ref b),
            (p, m, ref b) => p.ServerSendMagicFireEx2(m, ref b),
            (p, m, ref b) => p.ServerSendMyMagic(m, ref b),
            (p, m, ref b) => p.ServerSendMagicLVEXP(m, ref b),
            (p, m, ref b) => p.ServerSendSkeleton(m, ref b),
            (p, m, ref b) => p.ServerSendDuraChange(m, ref b),
            (p, m, ref b) => p.ServerSendGoldChanged(m, ref b),
            (p, m, ref b) => p.ServerSendChangeLight(m, ref b),
            (p, m, ref b) => p.ServerSendCharStatusChanged(m, ref b),
            (p, m, ref b) => p.ServerSendDigUp(m, ref b),
            (p, m, ref b) => p.ServerSendDigDown(m, ref b),
            (p, m, ref b) => p.ServerSendFlyAxe(m, ref b),
            (p, m, ref b) => p.ServerSendLighting(m, ref b),
            (p, m, ref b) => p.ServerSendSubAbility(m, ref b),
            (p, m, ref b) => p.ServerSendSpaceMoveShow(m, ref b),
            (p, m, ref b) => p.ServerSendReconnection(m, ref b),
            (p, m, ref b) => p.ServerSendHideEvent(m, ref b),
            (p, m, ref b) => p.ServerSendShowEvent(m, ref b),
        };

        Assert.Equal(117, handlers.Count);   // 与原文区间 36854-38859 的条数一致

        // 全部可调用且不抛（未接线时各处理器走"无宿主"路径）
        ResetAll();
        var probe = new TPlayObject { SelfHandle = SelfHandleValue };
        bool probeResult = false;
        var probeMsg = new TProcessMessage { BaseObject = OtherHandleValue };
        foreach (Receiver h in handlers)
            h(probe, probeMsg, ref probeResult);
    }
}
