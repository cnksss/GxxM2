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
//   * **CM_SITDOWN 分支 8997-9469**（本轮，提取为 `CheckUsePluginSitDown`；474 行）
//   * CM_DROPITEM 分支 9474-9487
//   * CM_PICKUP  分支 9492-9505
//   * else       分支 9506-9516
//   * **公共收尾 9521-9681**（提取为 `CheckUsePluginPostlude`，语义逐行等价；见下方偏差说明）
//   * 异常兜底 9682-9686
//
// **未覆盖（显式早退，见 `UnportedIdentFamilies`）**：
//   * CM_WALK 3528-4696 / CM_RUN 4697-5857 / CM_TURN 5858-6689 /
//     攻击族 6690-7888 / CM_SPELL 7889-8999
//     → 合计 **4,998 行**（本轮从 5,472 行减掉 CM_SITDOWN 的 474 行）
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
using static GXX.Core.Rtl.DelphiRTL;
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
    /// **尚未移植**的 ident 族（原文 :3528-8999）—— 见本文件头 §偏差。
    /// 这些 ident 直接早退 `return false`（= 不判定 = 放行），**不**进入公共收尾。
    /// 后续车道移植完某一族后，把对应行删掉即可（各族互不影响）。
    /// <para>
    /// ✅ 已从本表移除：`CM_SITDOWN`（:9000-9469，本轮移植为 <see cref="CheckUsePluginSitDown"/>）。
    /// </para>
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
        // :9000 CM_SITDOWN —— **已移植**（见 CheckUsePluginSitDown），故不在此早退。
        return false;
    }

    /// <summary>
    /// 原文 :3465-9688 <c>function TMirClientContext.CheckUsePlugin(Msg: PProcessMsg): Boolean;</c>
    /// <para>
    /// **已覆盖**：前导段 3505-3519、`CM_SITDOWN` 8997-9469（<see cref="CheckUsePluginSitDown"/>）、
    /// `CM_DROPITEM` 9474-9487、`CM_PICKUP` 9492-9505、`else` 9506-9516、
    /// 公共收尾 9521-9681（提取为 <see cref="CheckUsePluginPostlude"/>）、兜底 9682-9686。
    /// </para>
    /// <para>
    /// **未覆盖**：3528-8999（CM_WALK / CM_RUN / CM_TURN / 攻击族 / CM_SPELL，4,998 行）
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
            // ★**未移植**族早退（原文 :3528 CM_WALK / :4697 CM_RUN / :5858 CM_TURN /
            //   :6690 攻击族 / :7889 CM_SPELL —— CM_SITDOWN 已于本轮移植，不再早退）
            //   见本文件头 §偏差。
            // =========================================================================
            if (UnportedIdentFamilies(DefMsg.Ident))
            {
                return false;
            }

            // 原文 :3522/3528 的 `{$IF NEED_REGISTER = 0} case DefMsg.Ident of {$ELSE} if DefMsg.Ident = CM_WALK`
            // 结构 → 活分支是 if/else-if 链；**已移植的分支按原文顺序排列在本链首部**：
            //   * CM_SITDOWN :9000-9469 ✅（本轮）
            //   * CM_WALK :3528 / CM_RUN :4697 / CM_TURN :5858 / 攻击族 :6690 / CM_SPELL :7889
            //     → 仍由上面的 `UnportedIdentFamilies` 早退拦下，后续车道按序插入本链首部。
            // 下面依次是原文 :9474 CM_DROPITEM / :9492 CM_PICKUP / :9506 else。

            // {$IF NEED_REGISTER = 0} CM_SITDOWN: {$ELSE}     :8997-9000
            if (DefMsg.Ident == CM_SITDOWN)
            {
                // ---- 分支体 :9002-9469（提取为 CheckUsePluginSitDown）----
                if (CheckUsePluginSitDown(Msg, DefMsg, dwCurTick,
                        ref sSendMsg, ref nDelayTime, ref AntiPlugAction, ref ErrorCode, ref dwCurrentInterval))
                {
                    // 原文 :9039 / :9069 / :9286 / :9322 的 `Exit` —— 跳出整个 CheckUsePlugin，
                    // **跳过公共收尾 9521-9681**；此时 Result 仍是 :3507 的 False。
                    return Result;
                }
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

    // =================================================================================
    // 原文 :8997-9469  `else if DefMsg.Ident = CM_SITDOWN then`（**挖肉**）分支体
    //
    // ★ 分支骨架（与原文 begin/end 配对逐行对应）
    //   :9003      ErrorCode := 6;
    //   :9005-9009 环形缓冲写入 baCutMeat
    //   :9012-9072 暗杀检测（**两条互斥路径**，见下）
    //   :9073      Inc(nRecordActionIndex)          —— 在暗杀检测之后、限速之前
    //   :9076-9246 FLastAction ∈ {baWalk,baRun} → **移动到挖肉** amMoveToCutMeat 限速
    //   :9249-9427 否则若 amCutMeat.boEnabled   → **挖肉** amCutMeat 限速
    //   :9429-9459 按 FLastAction 三态打调试日志（ErrorCode 64/65/66）
    //   :9461-9466 nDelayTime = 0 时刷新 dwTicks[amCutMeat] / [amCutMeatToHit]
    //   :9468      FLastAction := baCutMeat;
    //
    // ★ 两条暗杀检测路径的**真实差异**（不可合并，已用测试钉死）
    //   (a) “采集满”（:9012 `RecordActionArr[MAX-1].Tick <> 0` 为真）：
    //       索引用 `mod MAX_RECORD_ACTION_COUNT`，**没有**正数守卫 → 槽 0 也参与判定。
    //   (b) 未满（:9043 else）：索引是裸减法 `nRecordActionIndex - I`，靠
    //       `PreIndex > 0` / `PrePreIndex > 0` 守卫 → **槽 0 与负下标都被跳过**；
    //       而 :9047 首判用的是 `PreIndex >= 0`（**与 :9055/:9058 的 `> 0` 不对称**，原文如此）。
    //   两路径都依赖 `{$B-}`（默认短路求值）：MirClientContext.pas 内**没有** `{$B+}`（已核实）
    //   → Delphi 的 `and` 等价 C# 的 `&&`，越界读不会发生（托管侧的 `&&` 同理短路）。
    //
    // ★ 原文缺陷 / 易错点（全部照抄，不做“顺手修正”）
    //   D-S1 :9012 的判据是**固定槽 MAX-1**，不是 nRecordActionIndex —— 写满一圈后恒为真
    //        （只要该槽 Tick 非 0），于是之后所有 CM_SITDOWN 都走 (a) 路径。
    //   D-S2 :9249 的 `else if` 只在 FLastAction ∉ {baWalk,baRun} 时才被评估：
    //        若 FLastAction 是走路/跑步而 amMoveToCutMeat.boEnabled = False，
    //        **amCutMeat 的整套限速被跳过**（不是“落到 else 再判一次”）。
    //   D-S3 :9461 的条件里 `and (not Msg.boDelay)` 被 `{ }` 注释掉 → 只判 `nDelayTime = 0`。
    //   D-S4 :9463 与 :9465 对 `dwTicks[amCutMeat]` **赋同一个值两次**（中间 :9464 是 amCutMeatToHit）。
    //   D-S5 :9039/:9069/:9286/:9322 的 `Exit` 都是**跳出整个 CheckUsePlugin**（跳过公共收尾）；
    //        本方法用 `return true` 表达，调用点 `return Result;`（此时 Result 仍为 False）。
    //   D-S6 :9275/:9311 的 `(nCollectIndex - I + nCollectCount) mod nCollectCount`：Delphi 的 `mod`
    //        结果符号随被除数（与 C# `%` 一致）；当 `nContinueSpeedCount > nCollectCount + nCollectIndex`
    //        时下标为负 —— Delphi 越界读内存、托管侧抛 IndexOutOfRangeException（照抄，不修正）。
    // =================================================================================

    /// <summary>
    /// 原文 :9002-9469 —— <c>CM_SITDOWN</c>（挖肉）分支体，1:1 移植。
    /// <para>
    /// 返回 <c>true</c> 表示原文在该分支内执行了 <c>Exit</c>（:9039 / :9069 / :9286 / :9322），
    /// 调用点据此 **跳过公共收尾 9521-9681**。
    /// </para>
    /// <para>
    /// 形参是原文 <c>CheckUsePlugin</c> 的函数级局部量中被本分支改写的部分
    /// （`sSendMsg` :3474 / `nDelayTime` :3475 / `dwCurrentInterval` :3476 /
    /// `AntiPlugAction` :3478 / `ErrorCode` :3504）；
    /// 其余函数级局部量只被本分支使用，故在本方法内以同名局部量重新声明（`dwTempInterval` /
    /// `boCurrentSpeed` / `boContinueSpeed` / `boCollectSpeed` / `boContinueSpeedPass` /
    /// `nSpeedCount` / `I` / `nCollectIndex` / `nCollectCount` / `PreIndex` / `PrePreIndex` /
    /// `nAssasinate`）。
    /// </para>
    /// </summary>
    private bool CheckUsePluginSitDown(TProcessMsg Msg, in TDefaultMessage DefMsg, uint dwCurTick,
        ref string sSendMsg, ref int nDelayTime, ref TAntiPlugAction AntiPlugAction, ref int ErrorCode,
        ref uint dwCurrentInterval)
    {
        // ---- 原文 :3474-3502 中被本分支独占的局部量（同名同类型）----
        uint dwTempInterval;                                        // :3476
        bool boCurrentSpeed, boContinueSpeed, boCollectSpeed;       // :3486
        bool boContinueSpeedPass;                                   // :3487
        int nSpeedCount;                                            // :3481
        int I, nCollectIndex, nCollectCount, PreIndex, PrePreIndex;  // :3483
        int nAssasinate;                                            // :3489

        // 原文在本分支反复取 `g_Config.ActionList[amMoveToCutMeat]` / `[amCutMeat]`（并取址赋给
        // AntiPlugAction）。托管侧 TAntiPlugAction 是引用类型 → 取局部别名与 `@g_Config.ActionList[…]`
        // **对象同一**，语义等价。
        int modeMoveToCutMeat = (int)TAntiPlugActionMode.amMoveToCutMeat;   // 22
        int modeCutMeat = (int)TAntiPlugActionMode.amCutMeat;               // 5
        TAntiPlugAction MoveToCutMeatAction = g_Config.ActionList[modeMoveToCutMeat];
        TAntiPlugAction CutMeatAction = g_Config.ActionList[modeCutMeat];

        ErrorCode = 6;                                              // :9003
        // :9004 原文此处是一行只有空格的空行

        if (nRecordActionIndex >= MirClientContextConst.MAX_RECORD_ACTION_COUNT || nRecordActionIndex < 0)
            nRecordActionIndex = 0;                                 // :9006
        RecordActionArr[nRecordActionIndex].Action = TBaseAction.baCutMeat;   // :9007
        RecordActionArr[nRecordActionIndex].Tick = MyGetTickCount();          // :9008
        RecordActionArr[nRecordActionIndex].DefMsg = DefMsg;                  // :9009

        // 所有采集满了
        if (RecordActionArr[MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1].Tick != 0)   // :9012
        {
            nAssasinate = 0;                                        // :9014
            PreIndex = (nRecordActionIndex - 1 + MirClientContextConst.MAX_RECORD_ACTION_COUNT)
                % MirClientContextConst.MAX_RECORD_ACTION_COUNT;     // :9015
            // 挖肉前面是其他包
            if (IsAssasinatePreAction(RecordActionArr[PreIndex].Action) &&              // :9017
                RunGateTiming.TickDiff(RecordActionArr[PreIndex].Tick, MyGetTickCount()) <= 250)   // :9018
            {
                nAssasinate++;                                      // :9020 Inc

                for (I = 2; I <= MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1; I++)   // :9022
                {
                    PreIndex = (nRecordActionIndex - I + MirClientContextConst.MAX_RECORD_ACTION_COUNT)
                        % MirClientContextConst.MAX_RECORD_ACTION_COUNT;    // :9024
                    if (RecordActionArr[PreIndex].Action == TBaseAction.baCutMeat)          // :9025
                    {
                        PrePreIndex = (nRecordActionIndex - I - 1 + MirClientContextConst.MAX_RECORD_ACTION_COUNT)
                            % MirClientContextConst.MAX_RECORD_ACTION_COUNT;                // :9027
                        if (IsAssasinatePreAction(RecordActionArr[PrePreIndex].Action) &&   // :9028
                            RunGateTiming.TickDiff(RecordActionArr[PrePreIndex].Tick,
                                RecordActionArr[PreIndex].Tick) <= 250)                     // :9029
                        {
                            nAssasinate++;                              // :9031 Inc
                        }
                    }
                }

                if (nAssasinate >= 3)                               // :9036
                {
                    ProcessAssasinate();                            // :9038
                    return true;                                    // :9039 Exit
                }
            }
        }
        else
        {
            nAssasinate = 0;                                        // :9045
            PreIndex = nRecordActionIndex - 1;                      // :9046
            if (PreIndex >= 0 && IsAssasinatePreAction(RecordActionArr[PreIndex].Action) &&   // :9047
                RunGateTiming.TickDiff(RecordActionArr[PreIndex].Tick, MyGetTickCount()) <= 250)   // :9048
            {
                nAssasinate++;                                      // :9050 Inc

                for (I = 2; I <= MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1; I++)   // :9052
                {
                    PreIndex = nRecordActionIndex - I;              // :9054
                    if (PreIndex > 0 && RecordActionArr[PreIndex].Action == TBaseAction.baCutMeat)   // :9055
                    {
                        PrePreIndex = nRecordActionIndex - I - 1;   // :9057
                        if (PrePreIndex > 0 && IsAssasinatePreAction(RecordActionArr[PrePreIndex].Action) &&   // :9058
                            RunGateTiming.TickDiff(RecordActionArr[PrePreIndex].Tick,
                                RecordActionArr[PreIndex].Tick) <= 250)                             // :9059
                        {
                            nAssasinate++;                          // :9061 Inc
                        }
                    }
                }

                if (nAssasinate >= 3)                               // :9066
                {
                    ProcessAssasinate();                            // :9068
                    return true;                                    // :9069 Exit
                }
            }
        }
        nRecordActionIndex++;                                       // :9073 Inc

        // 移动到挖肉
        if (FLastAction == TBaseAction.baWalk || FLastAction == TBaseAction.baRun)   // :9076
        {
            if (MoveToCutMeatAction.boEnabled)                      // :9078
            {
                dwTempInterval = MoveToCutMeatAction.nInterval;      // :9080

                if (FLastAction == TBaseAction.baWalk)              // :9082
                    dwCurrentInterval = RunGateTiming.TickDiff(
                        GameSpeed.dwTicks[(int)TAntiPlugActionMode.amWalk], dwCurTick);          // :9083
                else
                    dwCurrentInterval = RunGateTiming.TickDiff(
                        GameSpeed.dwTicks[(int)TAntiPlugActionMode.amRun], dwCurTick);           // :9085

                boCurrentSpeed = dwCurrentInterval < dwTempInterval;    // :9087
                // :9088 原文此处是一行只有空格的空行

                nSpeedCount = 0;                                    // :9089
                boCollectSpeed = false;                             // :9090

                if (SumSpeedProcessArr[modeMoveToCutMeat, 0] == 0)  // :9092
                    SumSpeedProcessArr[modeMoveToCutMeat, 0] = MyGetTickCount();    // :9093

                if (g_Config.dwCollectCount /*{g_Config.ActionList[amMoveToCutMeat].nCollectCount}*/ >= 2)   // :9095
                {
                    nCollectIndex = nCollectIntervalIndexArr[modeMoveToCutMeat];        // :9097
                    nCollectCount = g_Config.dwCollectCount /*{g_Config.ActionList[amMoveToCutMeat].nCollectCount}*/;   // :9098

                    // 倒数第2条数据采集到，加本次就是最一条搞定
                    if (dwCollectIntervalArr[modeMoveToCutMeat, nCollectCount - 2] != 0)    // :9101
                    {
                        // { :9103-9116 原文整段被注释：'本次和上次都超速就算超速' 的
                        //   boContinueSpeed := … < 0 判据 + 连续三次超速直接 ContinuousSpeed + Exit }

                        for (I = 0; I <= nCollectCount - 1; I++)    // :9118
                        {
                            if (I != nCollectIndex &&
                                dwCollectIntervalArr[modeMoveToCutMeat, I] < 0)             // :9120
                                nSpeedCount++;                      // :9121 Inc
                        }
                        if (boCurrentSpeed) nSpeedCount++;          // :9123 Inc
                        boCollectSpeed = nSpeedCount >= g_Config.dwSpeedValue;  // :9124  // g_Config.ActionList[amMoveToCutMeat].nCollectSpeedCount;
                    }
                    else
                    {
                        // 至少采集了1条
                        if (nCollectIndex >= 1)                     // :9129
                        {
                            // { :9131-9143 原文整段被注释：同上，`nCollectIndex - 1` / `- 2` 版本 }

                            for (I = 0; I <= nCollectIndex - 1; I++)    // :9145
                            {
                                if (dwCollectIntervalArr[modeMoveToCutMeat, I] < 0)     // :9147
                                    nSpeedCount++;                  // :9148 Inc
                            }
                            if (boCurrentSpeed) nSpeedCount++;      // :9150 Inc

                            if (dwCurrentInterval <= dwTempInterval / 3)    // :9152 dwTempInterval div 3
                            {
                                boCollectSpeed = true;              // :9154
                            }
                            else
                            {
                                if (nCollectIndex + 1 <= 3)         // :9158
                                    boCollectSpeed = nSpeedCount >= 2;                      // :9159
                                else if (nCollectIndex + 1 <= 7)    // :9160
                                {
                                    boCollectSpeed = nSpeedCount >= (nCollectIndex + 1) / 2;    // :9162
                                }
                                else
                                {
                                    boCollectSpeed = nSpeedCount >= (nCollectIndex + 1) / 2 - 1;   // :9166
                                }
                            }
                        }
                        // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                        else if (dwCurrentInterval <= dwTempInterval / 3)   // :9171
                        {
                            boCollectSpeed = true;                  // :9173
                        }
                    }
                }
                else if (dwCurrentInterval <= dwTempInterval / 3)   // :9177
                {
                    boCollectSpeed = true;                          // :9179
                }

                // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
                boContinueSpeedPass = GameSpeed.boContinueSpeed &&  // :9183
                    (RunGateTiming.TickDiff(GameSpeed.dwStartSpeedTick, MyGetTickCount()) >=
                        dwTempInterval + g_Config.dwContinueSpeedPassIncTime);   // :9184

                if ((dwCurrentInterval <= dwTempInterval / 10) ||   // :9186
                    ((!boContinueSpeedPass) && boCurrentSpeed && boCollectSpeed))   // :9187
                {
                    if (MoveToCutMeatAction.boShowHint)             // :9189
                        sSendMsg = MoveToCutMeatAction.sHintText;   // :9190

                    AntiPlugAction = MoveToCutMeatAction;           // :9192 AntiPlugAction := @g_Config.ActionList[amMoveToCutMeat]
                    LastLockAntiPlugActionMode = TAntiPlugActionMode.amMoveToCutMeat;   // :9193

                    if (RunGateTiming.TickDiff(SumSpeedProcessArr[modeMoveToCutMeat, 0],
                            MyGetTickCount()) <= g_Config.nSumSpeedCheckTime * 1000)    // :9195
                        SumSpeedProcessArr[modeMoveToCutMeat, 1]++;                 // :9196 Inc
                    else
                    {
                        SumSpeedProcessArr[modeMoveToCutMeat, 1] = 1;               // :9199
                        SumSpeedProcessArr[modeMoveToCutMeat, 0] = MyGetTickCount();   // :9200
                    }

                    if (AntiPlugAction.ProcessMode == TActionProcessMode.apmDelay &&
                        dwCurrentInterval < dwTempInterval)         // :9203
                    {
                        nDelayTime = (int)(dwTempInterval - dwCurrentInterval) + DELAY_TIME_ADD;   // :9205

                        // 修正加速一段时间后恢复到正常状态，一直提示加速
                        GameSpeed.nDelayCount[modeMoveToCutMeat] =
                            GameSpeed.nDelayCount[modeMoveToCutMeat] + 1;           // :9208
                        if (GameSpeed.nDelayCount[modeMoveToCutMeat] > 8)           // :9209
                        {
                            GameSpeed.nDelayCount[modeMoveToCutMeat] = 0;           // :9211
                            nDelayTime = 0;                                     // :9212
                            SendActionRet(true);                                // :9213
                        }
                    }

                    if (g_Config.boShowAttackLog)                   // :9217
                    {
                        ErrorCode = 62;                             // :9219
                        AddMainLogMsg(Format("【用户超速】%s:%d; 用户:%s",      // :9220-9223
                            AntiPlugActionModeNames3[modeMoveToCutMeat], dwCurrentInterval, sChrName), 0);
                    }
                }
                else
                {
                    if (!Msg.boDelay && !boContinueSpeedPass)       // :9228
                    {
                        GameSpeed.nDelayCount[modeMoveToCutMeat] = 0;   // :9230
                    }
                }

                if (nDelayTime == 0 && !Msg.boDelay)                // :9234
                {
                    nCollectIndex = nCollectIntervalIndexArr[modeMoveToCutMeat];    // :9236

                    if (dwCurrentInterval >= dwTempInterval)        // :9238
                        dwCollectIntervalArr[modeMoveToCutMeat, nCollectIndex] = 1;         // :9239
                    else
                        dwCollectIntervalArr[modeMoveToCutMeat, nCollectIndex] =
                            unchecked((int)(dwCurrentInterval - dwTempInterval));           // :9241

                    nCollectIntervalIndexArr[modeMoveToCutMeat] = (nCollectIndex + 1)
                        % g_Config.dwCollectCount /*{g_Config.ActionList[amMoveToCutMeat].nCollectCount}*/;   // :9243
                }
            }
        }

        // 去掉 (FLastAction = baCutMeat) and 是因为边挖肉边吃药的时候，加速检测不到 chongchong 2016-10-06
        else if (/*{(FLastAction = baCutMeat) and}*/ CutMeatAction.boEnabled)   // :9249
        {
            dwTempInterval = CutMeatAction.nInterval;               // :9251
            dwCurrentInterval = RunGateTiming.TickDiff(GameSpeed.dwTicks[modeCutMeat], dwCurTick);   // :9252
            boCurrentSpeed = dwCurrentInterval < dwTempInterval;    // :9253
            // :9254 原文此处是一行只有空格的空行

            nSpeedCount = 0;                                        // :9255
            boCollectSpeed = false;                                 // :9256

            if (SumSpeedProcessArr[modeCutMeat, 0] == 0)            // :9258
                SumSpeedProcessArr[modeCutMeat, 0] = MyGetTickCount();   // :9259

            if (g_Config.dwCollectCount /*{g_Config.ActionList[amCutMeat].nCollectCount}*/ >= 2)   // :9261
            {
                nCollectIndex = nCollectIntervalIndexArr[modeCutMeat];      // :9263
                nCollectCount = g_Config.dwCollectCount /*{g_Config.ActionList[amCutMeat].nCollectCount}*/;   // :9264

                // 倒数第2条数据采集到，加本次就是最一条搞定
                if (dwCollectIntervalArr[modeCutMeat, nCollectCount - 2] != 0)   // :9267
                {
                    // 本次和上次都超速就算超速 chongchong 2016-10-07
                    if (g_Config.boContinueSpeedCloseSocket && boCurrentSpeed)   // :9270
                    {
                        boContinueSpeed = true;                         // :9272
                        for (I = 1; I <= g_Config.nContinueSpeedCount; I++)   // :9273
                        {
                            if (dwCollectIntervalArr[modeCutMeat,
                                    (nCollectIndex - I + nCollectCount) % nCollectCount] >= 0)   // :9275
                            {
                                boContinueSpeed = false;                // :9277
                                break;                                  // :9278 Break
                            }
                        }

                        // 连续三次超速直接断开
                        if (boContinueSpeed)                        // :9283
                        {
                            ContinuousSpeed(TAntiPlugActionMode.amCutMeat, dwCurrentInterval);   // :9285
                            return true;                            // :9286 Exit
                        }
                    }

                    for (I = 0; I <= nCollectCount - 1; I++)        // :9290
                    {
                        if (I != nCollectIndex && dwCollectIntervalArr[modeCutMeat, I] < 0)   // :9292
                            nSpeedCount++;                          // :9293 Inc
                    }
                    if (boCurrentSpeed) nSpeedCount++;              // :9295 Inc
                    boCollectSpeed = nSpeedCount >= g_Config.dwSpeedValue;   // :9296  // g_Config.ActionList[amCutMeat].nCollectSpeedCount;
                }
                else
                {
                    // 至少采集了1条
                    if (nCollectIndex >= 1)                         // :9301
                    {
                        // 本次和上次都超速就算超速 chongchong 2016-10-07
                        if (g_Config.boContinueSpeedCloseSocket && boCurrentSpeed)   // :9304
                        {
                            if (nCollectIndex >= g_Config.nContinueSpeedCount)      // :9306
                            {
                                boContinueSpeed = true;             // :9308
                                for (I = 1; I <= g_Config.nContinueSpeedCount; I++)   // :9309
                                {
                                    if (dwCollectIntervalArr[modeCutMeat, nCollectIndex - I] >= 0)   // :9311
                                    {
                                        boContinueSpeed = false;        // :9313
                                        break;                          // :9314 Break
                                    }
                                }

                                // 连续三次超速直接断开
                                if (boContinueSpeed)            // :9319
                                {
                                    ContinuousSpeed(TAntiPlugActionMode.amCutMeat, dwCurrentInterval);   // :9321
                                    return true;                    // :9322 Exit
                                }
                            }
                        }

                        for (I = 0; I <= nCollectIndex - 1; I++)    // :9327
                        {
                            if (dwCollectIntervalArr[modeCutMeat, I] < 0)   // :9329
                                nSpeedCount++;                      // :9330 Inc
                        }
                        if (boCurrentSpeed) nSpeedCount++;          // :9332 Inc

                        if (dwCurrentInterval <= dwTempInterval / 3)    // :9334
                        {
                            boCollectSpeed = true;                  // :9336
                        }
                        else
                        {
                            if (nCollectIndex + 1 <= 3)             // :9340
                                boCollectSpeed = nSpeedCount >= 2;                  // :9341
                            else if (nCollectIndex + 1 <= 7)        // :9342
                            {
                                boCollectSpeed = nSpeedCount >= (nCollectIndex + 1) / 2;    // :9344
                            }
                            else
                            {
                                boCollectSpeed = nSpeedCount >= (nCollectIndex + 1) / 2 - 1;   // :9348
                            }
                        }
                    }
                    // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                    else if (dwCurrentInterval <= dwTempInterval / 3)   // :9353
                    {
                        boCollectSpeed = true;                      // :9355
                    }
                }
            }
            else if (dwCurrentInterval <= dwTempInterval / 3)       // :9359
            {
                boCollectSpeed = true;                              // :9361
            }

            // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
            boContinueSpeedPass = GameSpeed.boContinueSpeed &&      // :9365
                (RunGateTiming.TickDiff(GameSpeed.dwStartSpeedTick, MyGetTickCount()) >=
                    dwTempInterval + g_Config.dwContinueSpeedPassIncTime);   // :9366

            if ((dwCurrentInterval <= dwTempInterval / 10) ||       // :9368
                ((!boContinueSpeedPass) && boCurrentSpeed && boCollectSpeed))   // :9369
            {
                if (CutMeatAction.boShowHint)                       // :9371
                    sSendMsg = CutMeatAction.sHintText;             // :9372

                AntiPlugAction = CutMeatAction;                     // :9374 AntiPlugAction := @g_Config.ActionList[amCutMeat]
                LastLockAntiPlugActionMode = TAntiPlugActionMode.amCutMeat;   // :9375

                if (RunGateTiming.TickDiff(SumSpeedProcessArr[modeCutMeat, 0],
                        MyGetTickCount()) <= g_Config.nSumSpeedCheckTime * 1000)    // :9377
                    SumSpeedProcessArr[modeCutMeat, 1]++;                       // :9378 Inc
                else
                {
                    SumSpeedProcessArr[modeCutMeat, 1] = 1;                     // :9381
                    SumSpeedProcessArr[modeCutMeat, 0] = MyGetTickCount();      // :9382
                }

                if (AntiPlugAction.ProcessMode == TActionProcessMode.apmDelay &&
                    dwCurrentInterval < dwTempInterval)         // :9385
                {
                    nDelayTime = (int)(dwTempInterval - dwCurrentInterval) + DELAY_TIME_ADD;   // :9387

                    // 修正加速一段时间后恢复到正常状态，一直提示加速
                    GameSpeed.nDelayCount[modeCutMeat] = GameSpeed.nDelayCount[modeCutMeat] + 1;   // :9390
                    if (GameSpeed.nDelayCount[modeCutMeat] > 8)     // :9391
                    {
                        GameSpeed.nDelayCount[modeCutMeat] = 0;     // :9393
                        nDelayTime = 0;                             // :9394
                        SendActionRet(true);                        // :9395
                    }
                }

                if (g_Config.boShowAttackLog)                       // :9399
                {
                    ErrorCode = 63;                                 // :9401
                    AddMainLogMsg(Format("【用户超速】%s:%d; 用户:%s",          // :9402-9405
                        AntiPlugActionModeNames3[modeCutMeat], dwCurrentInterval, sChrName), 0);
                }
            }
            else
            {
                if (!Msg.boDelay && !boContinueSpeedPass)           // :9410
                {
                    GameSpeed.nDelayCount[modeCutMeat] = 0;         // :9412
                }
            }

            if (nDelayTime == 0 && !Msg.boDelay)                    // :9416
            {
                nCollectIndex = nCollectIntervalIndexArr[modeCutMeat];   // :9418

                if (dwCurrentInterval >= dwTempInterval)            // :9420
                    dwCollectIntervalArr[modeCutMeat, nCollectIndex] = 1;       // :9421
                else
                    dwCollectIntervalArr[modeCutMeat, nCollectIndex] =
                        unchecked((int)(dwCurrentInterval - dwTempInterval));   // :9423

                nCollectIntervalIndexArr[modeCutMeat] = (nCollectIndex + 1)
                    % g_Config.dwCollectCount /*{g_Config.ActionList[amCutMeat].nCollectCount}*/;   // :9425
            }
        }

        if (!Msg.boDelay)                                           // :9429
        {
            if (FLastAction == TBaseAction.baCutMeat)               // :9431
            {
                if (CutMeatAction.boDebug)                          // :9433
                {
                    ErrorCode = 64;                                 // :9435
                    AddMainLogMsg(Format("%s:%d; 用户:%s",          // :9436-9437
                        AntiPlugActionModeNames[modeCutMeat],
                        RunGateTiming.TickDiff(GameSpeed.dwTicks[modeCutMeat], dwCurTick), sChrName), 0);
                }
            }
            else if (FLastAction == TBaseAction.baWalk)             // :9440
            {
                if (MoveToCutMeatAction.boDebug)                    // :9442
                {
                    ErrorCode = 65;                                 // :9444
                    AddMainLogMsg(Format("%s:%d; 用户:%s",          // :9445-9446
                        AntiPlugActionModeNames3[modeMoveToCutMeat],
                        RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)TAntiPlugActionMode.amWalk], dwCurTick),
                        sChrName), 0);
                }
            }
            else if (FLastAction == TBaseAction.baRun)              // :9450
            {
                if (MoveToCutMeatAction.boDebug)                    // :9452
                {
                    ErrorCode = 66;                                 // :9454
                    AddMainLogMsg(Format("%s:%d; 用户:%s",          // :9455-9456
                        AntiPlugActionModeNames3[modeMoveToCutMeat],
                        RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)TAntiPlugActionMode.amRun], dwCurTick),
                        sChrName), 0);
                }
            }
        }

        if (nDelayTime == 0 /*{and (not Msg.boDelay)}*/)            // :9461
        {
            GameSpeed.dwTicks[modeCutMeat] = dwCurTick;                                          // :9463
            GameSpeed.dwTicks[(int)TAntiPlugActionMode.amCutMeatToHit] = dwCurTick;              // :9464
            GameSpeed.dwTicks[modeCutMeat] = dwCurTick;   // :9465（与 :9463 同值重复赋值 —— 原文缺陷 D-S4）
        }

        FLastAction = TBaseAction.baCutMeat;                        // :9468

        return false;   // 原文 :9469 分支体结束 → 继续进入公共收尾 9521-9681
    }

    /// <summary>
    /// 原文 `RecordActionArr[…].Action in [baHit, baSpell, baWalk, baRun, baTurn]`
    /// （:9017 / :9028 / :9047 / :9058 四处，集合内容一致）。
    /// </summary>
    private static bool IsAssasinatePreAction(TBaseAction action) =>
        action == TBaseAction.baHit || action == TBaseAction.baSpell || action == TBaseAction.baWalk ||
        action == TBaseAction.baRun || action == TBaseAction.baTurn;

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
