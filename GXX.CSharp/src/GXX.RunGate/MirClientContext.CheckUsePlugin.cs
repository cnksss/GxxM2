// =====================================================================================
// 源单元：Source/RunGate/MirClientContext.pas（GBK，实测 11,125 LF）
// 本文件对应原文 **CheckUsePlugin 3465-9688**（6,224 行，全工程最大的单个函数）。
//
// -------------------------------------------------------------------------------------
// ★ 分派结构（本车道实测，与 docs/并行报告-p4-rungate-mirclient.md §8 的表格一致）
// -------------------------------------------------------------------------------------
// 原文 :3521-3529 是 `{$IF NEED_REGISTER = 0} case DefMsg.Ident of {$ELSE} if DefMsg.Ident = CM_WALK then`。
// 活分支 NEED_REGISTER = 1（Grobal2_Ex.pas:18）→ **顶层是 if / else-if 链，不是 case**。
// 顶层只有 **9 个分支**：
//
//   | # | 分派键                        | 起始行 | 结束行 | 行数  |
//   |---|-------------------------------|--------|--------|-------|
//   | 1 | CM_WALK  (走路)               | 3528   | 4696   | 1,169 |
//   | 2 | CM_RUN   (跑步)               | 4697   | 5857   | 1,161 |
//   | 3 | CM_TURN  (转向)               | 5858   | 6689   |   832 |
//   | 4 | 攻击族（CM_HIT..CM_115HIT + 自定义技能）| 6690 | 7888 | 1,199 |
//   | 5 | CM_SPELL (魔法)               | 7889   | 8999   | 1,111 |
//   | 6 | CM_SITDOWN (坐下)             | 9000   | 9473   |   474 |
//   | 7 | CM_DROPITEM (丢物品)          | 9474   | 9487   |    14 |
//   | 8 | CM_PICKUP (捡物品)            | 9492   | 9505   |    14 |
//   | 9 | else（其它全部 ident）        | 9506   | 9516   |    11 |
//   公共收尾（所有分支共用）            | 9521   | 9681   |   161 |
//   异常兜底                            | 9682   | 9686   |     5 |
//
// 注：:9471-9475 / :9489-9493 / :9517-9519 是 `{$IF NEED_REGISTER = 0}` 的 case 标签/`end; // end case`
//     包裹行（死分支），本车道按活分支等价展开。
//
// -------------------------------------------------------------------------------------
// ★ 本车道覆盖状态
// -------------------------------------------------------------------------------------
// **已完成**：
//   * 局部常量 3467-3472（5 个）
//   * 忠实前导段 3505-3519
//   * CM_DROPITEM 分支 9474-9487
//   * CM_PICKUP  分支 9492-9505
//   * else       分支 9506-9516
//   * **公共收尾 9521-9681**（提取为 `CheckUsePluginPostlude`，语义逐行等价；见下方偏差说明）
//   * 异常兜底 9682-9686
//
// **未覆盖（显式早退，见 `UnportedIdentFamilies`）**：
//   * CM_WALK 3528-4696 / CM_RUN 4697-5857 / CM_TURN 5858-6689 /
//     攻击族 6690-7888 / CM_SPELL 7889-8999 / CM_SITDOWN 9000-9473
//     → 合计 **5,472 行**
//
// -------------------------------------------------------------------------------------
// ★ 偏差（本车道唯一一处结构性偏差，必须登记）
// -------------------------------------------------------------------------------------
// 未覆盖的 6 个 ident 族在原文里会走各自的分支、**然后进入公共收尾 9521-9681**。
// 本车道对它们**显式早退**（`return false`，不进入收尾），而不是让它们落进 `else` 分支。
// 原因：落进 `else` 会把 `RecordActionArr[nRecordActionIndex].Action` 错写成 `baOther`
//       （原文走路写 `baWalk`、魔法写 `baSpell`），并在收尾里对**每个**走路/攻击包执行
//       `GameSpeed.boContinueSpeed := False`，直接破坏连续超速状态机 —— 比"整族不处理"更糟。
// 影响：未覆盖族**只是不被反外挂判定**（等同"放行"），与上一提交（整函数返回 false）行为一致，
//       不会引入新的错误副作用。等后续车道移植完对应族后，把 `UnportedIdentFamilies` 里的判断逐条删除即可。
//
// -------------------------------------------------------------------------------------
// ★ 跨语言易错点（已核实，重要）
// -------------------------------------------------------------------------------------
// 调用点 :1820 写的是 `if CheckUsePlugin(@ProcessMsg.DefMessage) then`，而形参声明是
// `Msg: PProcessMsg`（:80 / :3465）—— **类型不匹配的指针双关**。它能工作是因为 `TProcessMsg`
// 的前两个字段恰好是 `DefMessage: TDefaultMessage`（偏移 0，16 字节）与
// `dwTimeTick: LongWord`（偏移 16）：
//     Msg.DefMessage → 正确（偏移 0）
//     Msg.dwTimeTick → 正确（偏移 16）
// C# 侧 `TProcessMsg` 是引用类型，直接传整个 `ProcessMsg` 与原文**语义完全等价**，
// 故 `MirClientContext.Run.cs` 的调用点写成 `CheckUsePlugin(ProcessMsg)`。
// =====================================================================================

using System;
using GXX.Core.Protocol;

using static GXX.RunGate.GateShareSeam;
using static GXX.RunGate.FormGlobals;
using static GXX.RunGate.RunGateConst;
using static GXX.RunGate.RunGateUtilsConst;
using static GXX.Core.Protocol.Grobal2Const;

namespace GXX.RunGate;

public partial class TMirClientContext
{
    // ---------------------------------------------------------------------------------
    // 原文 :3467-3472 局部常量
    // ---------------------------------------------------------------------------------
    private const int DEBUG_LEVEL = 0;                                  // :3467
    private const int MAX_REPAIR_TIME = 200;                            // :3468
    private const int TIME_INACCURACY = 0;                              // :3469
    private const int DELAY_TIME_ADD = TIME_INACCURACY + 10;            // :3470
    private const int DROP_CONCURRENT_RATE = 3;                         // :3472

    /// <summary>
    /// 未覆盖的 6 个 ident 族（原文 :3528-9473）—— 见本文件头 §偏差。
    /// 这些 ident 直接早退 `return false`（= 不判定 = 放行），**不**进入公共收尾。
    /// 后续车道移植完某一族后，把对应行删掉即可。
    /// </summary>
    private static bool UnportedIdentFamilies(ushort ident)
    {
        // :3528 CM_WALK
        if (ident == CM_WALK) return true;
        // :4697 CM_RUN
        if (ident == CM_RUN) return true;
        // :5858 CM_TURN
        if (ident == CM_TURN) return true;
        // :6690-6699 攻击族（CM_HIT..CM_115HIT + 自定义技能区间）
        if (ident == CM_HIT || ident == CM_HEAVYHIT || ident == CM_BIGHIT ||
            ident == CM_POWERHIT || ident == CM_LONGHIT || ident == CM_WIDEHIT ||
            ident == CM_FIREHIT || ident == CM_CRSHIT || ident == CM_TWNHIT ||
            ident == CM_SWORDHIT || ident == CM_43HIT || ident == CM_66HIT ||
            ident == CM_66HIT1 || ident == CM_101HIT || ident == CM_102HIT ||
            ident == CM_103HIT || ident == CM_113HIT || ident == CM_115HIT ||
            (ident >= CM_CUSTOM_HIT001 && ident < CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT)) return true;
        // :7889 CM_SPELL
        if (ident == CM_SPELL) return true;
        // :9000 CM_SITDOWN
        if (ident == CM_SITDOWN) return true;
        return false;
    }

    /// <summary>
    /// 原文 :3465-9688 <c>function TMirClientContext.CheckUsePlugin(Msg: PProcessMsg): Boolean;</c>
    /// <para>
    /// **已覆盖**：前导段 3505-3519、`CM_DROPITEM` 9474-9487、`CM_PICKUP` 9492-9505、
    /// `else` 9506-9516、公共收尾 9521-9681（提取为 <see cref="CheckUsePluginPostlude"/>）、兜底 9682-9686。
    /// </para>
    /// <para>
    /// **未覆盖**：3528-9473（CM_WALK / CM_RUN / CM_TURN / 攻击族 / CM_SPELL / CM_SITDOWN，5,472 行）
    /// —— 显式早退，见 <see cref="UnportedIdentFamilies"/>。
    /// </para>
    /// </summary>
    public bool CheckUsePlugin(TProcessMsg Msg)
    {
        // ---- 局部变量（原文 :3474-3503，逐条保留）----
        string sSendMsg = "";                                    // :3474
        int nDelayTime = 0, ConcurrentCount, Len;                // :3475
        uint dwCurTick, dwTempInterval, dwCurrentInterval = 0;   // :3476

        TAntiPlugAction AntiPlugAction;                          // :3478
        TDefaultMessage DefMsg;                                  // :3479

        int nSpeedCount;                                         // :3481

        int I, nCollectIndex, nCollectCount, PreIndex, PrePreIndex;   // :3483
        TDefaultMessage DefaultMessage = default;                // :3484

        bool boCurrentSpeed, boContinueSpeed, boCollectSpeed;    // :3486
        bool boContinueSpeedPass;                                // :3487

        int nAssasinate;                                         // :3489 暗杀次数

        bool IsDropConcurrent = false;                           // :3491

        int nDropConcurrentCount = 0;                            // :3493

        TDefaultMessage SendDefMsg = default;                    // :3495

        string sHitMagic = "";                                   // :3497

        object RunGateObj;                                       // :3499
        TRunGate RunGate;                                        // :3500

        int nCompensationValue = 0;                              // :3502

        int ErrorCode = 1;                                       // :3504

        // {$I VMProtectBegin.inc}                               // :3506
        bool Result = false;                                     // :3507
        nDelayTime = 0;                                          // :3508
        sSendMsg = "";                                           // :3509

        ErrorCode = 1;                                           // :3511

        nCompensationValue = 0;                                  // :3513

        DefMsg = Msg.DefMessage;                                 // :3515  DefMsg := @Msg.DefMessage
        dwCurTick = Msg.dwTimeTick;                              // :3516  取收包时间

        IsDropConcurrent = false;                                // :3518
        AntiPlugAction = null;                                   // :3519
        try
        {
            // =========================================================================
            // ★未覆盖族早退（原文 :3528 / :4697 / :5858 / :6690 / :7889 / :9000 各自的分支）
            //   见本文件头 §偏差。
            // =========================================================================
            if (UnportedIdentFamilies(DefMsg.Ident))
            {
                return false;
            }

            // 原文 :3522/3528 的 `{$IF NEED_REGISTER = 0} case DefMsg.Ident of {$ELSE} if DefMsg.Ident = CM_WALK`
            // 结构 → 活分支是 if/else-if 链；下面从 CM_DROPITEM (9474) 起逐字对应。
            if (false)
            {
                // 占位：保持 if/else-if 链的形状（原文 CM_WALK..CM_SITDOWN 六族的锚点）
            }

            // {$IF NEED_REGISTER = 0} CM_DROPITEM: {$ELSE}   :9471-9474
            else if (DefMsg.Ident == CM_DROPITEM)
            {
                // :9476-9487
                ErrorCode = 7;                                   // :9477

                if (nRecordActionIndex >= MirClientContextConst.MAX_RECORD_ACTION_COUNT || nRecordActionIndex < 0)
                    nRecordActionIndex = 0;                      // :9480
                RecordActionArr[nRecordActionIndex].Action = TBaseAction.baOther;    // :9481
                RecordActionArr[nRecordActionIndex].Tick = MyGetTickCount();         // :9482
                RecordActionArr[nRecordActionIndex].DefMsg = DefMsg;                 // :9483
                nRecordActionIndex++;                                                // :9484 Inc

                FLastAction = TBaseAction.baOther;                                   // :9486
            }

            // {$IF NEED_REGISTER = 0} CM_PICKUP: {$ELSE}     :9489-9492
            else if (DefMsg.Ident == CM_PICKUP)
            {
                // :9494-9505
                ErrorCode = 8;                                   // :9495

                if (nRecordActionIndex >= MirClientContextConst.MAX_RECORD_ACTION_COUNT || nRecordActionIndex < 0)
                    nRecordActionIndex = 0;                      // :9498
                RecordActionArr[nRecordActionIndex].Action = TBaseAction.baOther;    // :9499
                RecordActionArr[nRecordActionIndex].Tick = MyGetTickCount();         // :9500
                RecordActionArr[nRecordActionIndex].DefMsg = DefMsg;                 // :9501
                nRecordActionIndex++;                                                // :9502 Inc

                FLastAction = TBaseAction.baOther;                                   // :9504
            }

            else
            {
                // :9506-9516
                if (nRecordActionIndex >= MirClientContextConst.MAX_RECORD_ACTION_COUNT || nRecordActionIndex < 0)
                    nRecordActionIndex = 0;                      // :9509
                RecordActionArr[nRecordActionIndex].Action = TBaseAction.baOther;    // :9510
                RecordActionArr[nRecordActionIndex].Tick = MyGetTickCount();         // :9511
                RecordActionArr[nRecordActionIndex].DefMsg = DefMsg;                 // :9512
                nRecordActionIndex++;                                                // :9513 Inc

                FLastAction = TBaseAction.baOther;                                   // :9515
            }

            // {$IF NEED_REGISTER = 0} end; // end case {$IFEND}   :9517-9519

            // ---- 公共收尾 9521-9681 ----
            Result = CheckUsePluginPostlude(AntiPlugAction, Msg, sSendMsg, nDelayTime,
                IsDropConcurrent, ConcurrentCount: 0, dwCurrentInterval: 0, ErrorCode);
        }
        catch (Exception E)
        {
            // :9682-9686
            ErrorCode = ErrorCode;   // 原文如此（ErrorCode 在 except 里只被读取）
            AddMainLogMsg("TMirClientContext.CheckUsePlugin Error, Code = " + ErrorCode + ". " + E.Message, 0);   // :9684
        }
        // {$I VMProtectEnd.inc}                                  // :9686
        return Result;                                           // :3507 的 Result
    }

    /// <summary>
    /// 原文 **9521-9681** —— 所有 ident 分支共用的公共收尾（逐行等价）。
    /// <para>
    /// 提取为方法只是把原文的局部变量（`AntiPlugAction` / `sSendMsg` / `nDelayTime` /
    /// `IsDropConcurrent` / `ConcurrentCount` / `dwCurrentInterval` / `ErrorCode` / `Result`）
    /// 变成形参/返回值；**语句顺序、判据、分支顺序完全照抄**。
    /// 原文 :9676 的 `Exit`（跳出整个 CheckUsePlugin）在本方法里落成 `return`。
    /// </para>
    /// <para>
    /// <b>public 的理由</b>：原文里 `AntiPlugAction` 由未覆盖的 6 个 ident 分支赋值，
    /// 因此在当前覆盖状态下从公开入口 <see cref="CheckUsePlugin"/> **无法**触达本方法；
    /// 测试需要一个入口。按 uBuffer*.cs 的探针约定公开（生产语义不变）。
    /// </para>
    /// </summary>
    public bool CheckUsePluginPostlude(TAntiPlugAction AntiPlugAction, TProcessMsg Msg,
        string sSendMsg, int nDelayTime, bool IsDropConcurrent, int ConcurrentCount,
        uint dwCurrentInterval, int ErrorCode)
    {
        bool Result = false;                                     // 承接原文 :3507 的 Result

        // 发送超速显示信息 chongchong 2014-12-15
        if (AnsiLen(sSendMsg) != 0)                              // :9522 Length(sSendMsg) <> 0
        {
            SendMessaggeToClient(sSendMsg, g_Config.btMsgType, g_Config.btMsgFColor, g_Config.btMsgBColor);   // :9524
        }

        // 检查到多次超速处理
        if (AntiPlugAction != null)                              // :9528
        {
            TRunGate RunGate = null;                             // :9530
            object RunGateObj = GetRunGate();                    // :9531
            if (RunGateObj != null && RunGateObj is TRunGate)    // :9532
            {
                RunGate = (TRunGate)RunGateObj;                  // :9534
            }

            if (AntiPlugAction.boProcessScript && RunGate != null)   // :9537
            {
                TDefaultMessage DefaultMessage = MakeDefaultMsg(CM_SENDUSERSPEEDING, 0, 0, 0, 0);   // :9539
                RunGate.SendServerMsg(GM_DATA, (ushort)ContextID, unchecked((int)Socket), nUserListIndex,
                    StructBytes.BytesOf(DefaultMessage), TDefaultMessage.SizeOf);                    // :9540
            }

            if (!GameSpeed.boContinueSpeed)                      // :9543
            {
                GameSpeed.boContinueSpeed = true;                // :9545
                GameSpeed.dwStartSpeedTick = MyGetTickCount();   // :9546
            }

            if (SumSpeedProcessArr[(int)LastLockAntiPlugActionMode, 1] >= (uint)g_Config.nSumSpeedMaxCount)   // :9549
            {
                if (AntiPlugAction.SumProcessMode == TSumActionProcessMode.sampLockUser)    // :9551
                {
                    LockUser(g_Config.nLockTime);            // :9553
                }
                else if (AntiPlugAction.SumProcessMode == TSumActionProcessMode.sampOffline)   // :9555
                {
                    DelayClose(1000);                        // :9557
                    return Result;                           // :9558 Exit
                }
            }

            // 超速后清空所有未处理数据 chongchong 2014-12-15
            if (g_Config.boSpeedClearData)                       // :9563
            {
                ClearClientMsgList();                            // :9565
            }

            switch (AntiPlugAction.ProcessMode)                  // :9568
            {
                case TActionProcessMode.apmLost:                 // :9569 丢弃封包（封包无效）
                    Result = true;                               // :9571
                    break;
                case TActionProcessMode.apmDelay:                // :9573 停顿操作
                    if (nDelayTime > 0)                          // :9575
                        DelayClientMessage(Msg, (uint)nDelayTime);   // :9576 add chongchong 2015-06-01
                    Result = true;                               // :9577
                    break;
                case TActionProcessMode.apmRebound:              // :9579 反弹卡刀
                    Result = true;                               // :9581

                    if (IsDropConcurrent)                        // :9583
                        SendActionRet(true);                     // :9584
                    else
                        SendActionRet(false);                    // :9586
                    break;
                case TActionProcessMode.apmOffline:              // :9588 掉线处理
                    Result = true;                               // :9590
                    DelayClose(1000);                            // :9591
                    //Close;                                     // :9592 原文如此（已注释）
                    break;
                case TActionProcessMode.apmFakeAttackPass:       // :9594 假刀放行 / 丢弃封包
                    // 并发处理，对应的是丢弃封包
                    if (LastLockAntiPlugActionMode == TAntiPlugActionMode.amHitConcurrent ||
                        LastLockAntiPlugActionMode == TAntiPlugActionMode.amSpellConcurrent ||
                        LastLockAntiPlugActionMode == TAntiPlugActionMode.amMoveConcurrent)   // :9597
                    {
                        ConcurrentCount = ClearConcurrentPacket(Msg.DefMessage);   // :9599
                        int nDropConcurrentCount = ConcurrentCount;                // :9600
                        if (ConcurrentCount > 0)                                   // :9601
                        {
                            // 原文 :9608 把 AnsiString 当字节缓冲做 `sSendMsg := sSendMsg + EncodeRunGateMsg(...)`；
                            // C# 侧用 byte[] 累加，避免 GBK 往返（偏差 D3）。
                            byte[] sendMsgBytes = Array.Empty<byte>();
                            for (int I = 0; I <= ConcurrentCount - 1; I++)         // :9604
                            {
                                TDefaultMessage SendDefMsg = MakeDefaultMsg(SM_ACTION_RET, 0, 1, 0, 0);   // :9606

                                sendMsgBytes = Concat(sendMsgBytes, EncodeRunGateMsg(SendDefMsg, null, 0));   // :9608
                            }
                            PostSendTextBytes(sendMsgBytes);                       // :9610 PostSendText(sSendMsg)
                        }

                        // 后面的包丢了，这个包算作有效
                        if (!IsDropConcurrent)                                   // :9614
                            Result = false;                                      // :9615
                        else
                        {
                            nDropConcurrentCount = nDropConcurrentCount + 1;     // :9618

                            SendActionRet(true);                                 // :9620
                            Result = true;                                       // :9621
                        }

                        // { :9624-9652 原文整段被注释：boShowDropConcurrentLog 的三条日志 }
                    }
                    // 假刀放行
                    else
                    {
                        SendActionRet(true);                                     // :9657

                        // 魔法假刀要特殊对待 (差一个SM_MAGICFIRE包，手举起来半天不放下去) chongchong
                        if (FLastAction == TBaseAction.baSpell)                  // :9660
                        {
                            TDefaultMessage SendDefMsg = MakeDefaultMsg(SM_MAGICFIRE_FAIL, nRecogId, 0, 0, 0);   // :9662
                            PostSendTextBytes(EncodeRunGateMsg(SendDefMsg, null, 0));   // :9663-9664
                        }

                        Result = true;                                           // :9667
                    }
                    break;
                case TActionProcessMode.apmNoProcess:            // :9670 不做处理
                    Result = false;                              // :9672
                    break;
            }

            _ = dwCurrentInterval;   // 原文只在被注释的 :9624-9652 日志块里用
            return Result;                                   // :9676 Exit
        }
        else
        {
            GameSpeed.boContinueSpeed = false;               // :9680
        }

        return Result;
    }
}
