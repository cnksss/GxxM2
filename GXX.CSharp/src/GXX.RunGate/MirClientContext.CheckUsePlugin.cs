// =====================================================================================
// 源单元：Source/RunGate/MirClientContext.pas（GBK，实测 11,125 LF）
//
// 本文件对应原文 **CheckUsePlugin 3465-9688**（约 6,224 行，全工程最大的单个函数）。
//
// ★ 覆盖状态：**未覆盖（接缝）**。
//   本车道只落地了该函数的**忠实前导段 3505-3519**（原文逐行）与"异常兜底 9682-9686"的形状，
//   其 `case DefMsg.Ident of` 分派体（3521-9681）**尚未 1:1 移植**。
//
//   原样保留的语义（可验证）：
//     :3505  {$I VMProtectBegin.inc}   ← VMProtect 标记，按 §2.3 不移植
//     :3507  Result := False
//     :3508  nDelayTime := 0
//     :3509  sSendMsg := ''
//     :3511  ErrorCode := 1
//     :3513  nCompensationValue := 0
//     :3515  DefMsg := @Msg.DefMessage
//     :3516  dwCurTick := Msg.dwTimeTick      // 取收包时间
//     :3518  IsDropConcurrent := False
//     :3519  AntiPlugAction := nil
//     :9681  except → :9684 AddMainLogMsg('TMirClientContext.CheckUsePlugin Error, Code = ' + ...)
//
//   ★ 跨语言易错点（已核实，重要）：调用点 :1820 写的是
//       `if CheckUsePlugin(@ProcessMsg.DefMessage) then`
//     而形参声明是 `Msg: PProcessMsg`（:80 / :3465）—— **类型不匹配的指针双关**。
//     之所以"能工作"，是因为 `TProcessMsg` 的前两个字段恰好是
//       `DefMessage: TDefaultMessage`（偏移 0，16 字节）与 `dwTimeTick: LongWord`（偏移 16），
//     所以把 `@ProcessMsg.DefMessage` 当成 `PProcessMsg` 解引用时：
//       `Msg.DefMessage` → 正确（偏移 0）
//       `Msg.dwTimeTick` → 正确（偏移 16）
//     C# 侧 TProcessMsg 是引用类型，直接传整个 `ProcessMsg` 与原文**语义完全等价**，
//     故 `MirClientContext.Run.cs` 的调用点写成 `CheckUsePlugin(ProcessMsg)`。
//
//   ★ 接管者需要的结构（侦察结论，供后续车道 1:1 落地）：
//     - 常量 :3467-3472：DEBUG_LEVEL = 0 / MAX_REPAIR_TIME = 200 / TIME_INACCURACY = 0 /
//       DELAY_TIME_ADD = TIME_INACCURACY + 10 / DROP_CONCURRENT_RATE = 3
//     - 分派键：`DefMsg.Ident`（`{$IF NEED_REGISTER = 0}` 为 case，NEED_REGISTER = 1 的活分支
//       走 `if DefMsg.Ident = CM_WALK then ...` 链）
//     - 使用的本类状态：dwCollectIntervalArr / nCollectIntervalIndexArr / RecordActionArr /
//       nRecordActionIndex / SumSpeedProcessArr / nCompensationArr / LastLockAntiPlugActionMode /
//       FLastAction / GameSpeed / nMoveSpeed / nAttackSpeed / nSpellSpeed / MagicUseTickList
//     - 使用的全局量：g_Config.ActionList[*] / g_wActionSpeedIntervals / g_MagicCDList /
//       g_sMagicCDMsgText / LastEatingItemTick / LastHeroEatingItemTick / HumBagItems / HeroBagItems
//     - 调用的本类方法：GetConcurrentPacketCount / ClearConcurrentPacket / ContinuousSpeed /
//       ProcessAssasinate / SendMessaggeToClient / DelayClose / SendActionRet / AddServerMsg
//     - 纯判定内核（已由同项目既有接缝覆盖，可直接复用，不必重写）：
//         `RunGateUtilsClientStat`（多数表决）、`RunGateForwardClassifier`、
//         `RunGateTiming.TickDiff`、`uFrmGameSpeedLogic.cs` 的 `TAntiPlugActionMode` /
//         `TAntiPlugAction` / `TAntiPlugConfig` / `RunGateConst.AntiPlugActionModeNames*` /
//         `FormGlobals.g_wActionSpeedIntervals` / `ActionModeUseSpeedIntervals`
// =====================================================================================

using System;
using GXX.Core.Protocol;

using static GXX.RunGate.GateShareSeam;
using static GXX.RunGate.FormGlobals;
using static GXX.RunGate.RunGateConst;

namespace GXX.RunGate;

public partial class TMirClientContext
{
    // ---------------------------------------------------------------------------------
    // 原文 :3465-3472 局部常量
    // ---------------------------------------------------------------------------------
    private const int CHECKUSEPLUGIN_DEBUG_LEVEL = 0;                       // :3467
    private const int CHECKUSEPLUGIN_MAX_REPAIR_TIME = 200;                 // :3468
    private const int CHECKUSEPLUGIN_TIME_INACCURACY = 0;                   // :3469
    private const int CHECKUSEPLUGIN_DELAY_TIME_ADD = CHECKUSEPLUGIN_TIME_INACCURACY + 10;   // :3470
    private const int CHECKUSEPLUGIN_DROP_CONCURRENT_RATE = 3;              // :3472

    /// <summary>
    /// 原文 :3465-9688 <c>function TMirClientContext.CheckUsePlugin(Msg: PProcessMsg): Boolean;</c>
    /// <para>
    /// <b>本车道未覆盖分派体（3521-9681）</b>；只落地忠实前导段与异常兜底。
    /// 未覆盖时返回 false = "未检测到外挂"，即放行（与原文"无匹配 Ident 时的 Result := False"一致）。
    /// </para>
    /// </summary>
    public bool CheckUsePlugin(TProcessMsg Msg)
    {
        // {$I VMProtectBegin.inc}   :3506 —— VMProtect 标记，§2.3 不移植
        const bool Result = false;                                        // :3507  Result := False
        int nDelayTime = 0;                                               // :3508
        string sSendMsg = "";                                             // :3509

        int ErrorCode = 1;                                                // :3511

        int nCompensationValue = 0;                                       // :3513

        TDefaultMessage DefMsg = Msg.DefMessage;                          // :3515  DefMsg := @Msg.DefMessage
        uint dwCurTick = Msg.dwTimeTick;                                  // :3516  取收包时间

        bool IsDropConcurrent = false;                                    // :3518
        TAntiPlugAction AntiPlugAction = null;                            // :3519

        try
        {
            // =========================================================================
            // ★未覆盖：原文 3521-9681 的 `case DefMsg.Ident of` 分派体（约 6,160 行）。
            //   本车道未移植。若在此静默返回，等于对**所有** Ident 都判定"无外挂"。
            //   已登记在 docs/并行报告-p4-rungate-mirclient.md 的「未覆盖」与
            //   「需要调度方改白名单外文件的精确签名要求」两节。
            // =========================================================================
            _ = DefMsg;
            _ = dwCurTick;
            _ = nDelayTime;
            _ = sSendMsg;
            _ = nCompensationValue;
            _ = IsDropConcurrent;
            _ = AntiPlugAction;
            _ = ErrorCode;
        }
        catch (Exception E)
        {
            AddMainLogMsg("TMirClientContext.CheckUsePlugin Error, Code = " + ErrorCode + ". " + E.Message, 0);   // :9684
        }

        return Result;                                                    // :3507 的 Result（恒 false）
    }
}
