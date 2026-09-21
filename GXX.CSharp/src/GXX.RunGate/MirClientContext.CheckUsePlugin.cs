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
//   * **CM_WALK 分支 3525-4692**（提取为 `CheckUsePluginWalk` + `RunFamilySpeedBlock` + `MoveSpeedInterval`）
//   * **CM_RUN 分支 4694-5853**（提取为 `CheckUsePluginRun` + `RunFamilySpeedBlock` + `MoveSpeedInterval`）
//   * **CM_TURN 分支 5855-6681**（提取为 `CheckUsePluginTurn` + `TurnFamilySpeedBlock`）
//   * **CM_SPELL 分支 7886-8995**（提取为 `CheckUsePluginSpell` + `SpellSpeedInterval` +
//     `SpellFamilySpeedBlock`；公共采集/判定体见 `CollectSpeedDetect`）
//   * **CM_SITDOWN 分支 8997-9469**（提取为 `CheckUsePluginSitDown`）
//   * CM_DROPITEM 分支 9474-9487
//   * CM_PICKUP  分支 9492-9505
//   * else       分支 9506-9516
//   * **公共收尾 9521-9681**（提取为 `CheckUsePluginPostlude`，语义逐行等价；见下方偏差说明）
//   * 异常兜底 9682-9686
//
// **未覆盖（显式早退，见 `UnportedIdentFamilies`）**：
//   * 攻击族 6690-7888 → 按上一版口径（5,472 − 474 − 832 − 1,111 − 1,161(RUN) − 1,169(WALK)）计 **725 行**
//     ⚠ 口径说明：§8 分派表的**区间长度**是 5,946 行（攻击族区间 6690-7888 = 1,199 行），
//       与报告 §3.1 的 5,472 口径不一致（历史遗留）；此处沿用减法口径，两种口径的差额固定在攻击族。
//
// -------------------------------------------------------------------------------------
// ★ 偏差（本车道唯一一处结构性偏差，必须登记）
// -------------------------------------------------------------------------------------
// 未覆盖的 ident 族（**攻击族** 6690-7888）在原文里会走各自的分支、**然后进入公共收尾 9521-9681**。
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
        // :3528 CM_WALK —— **已移植**（见 CheckUsePluginWalk），故不在此早退。
        // :4697 CM_RUN —— **已移植**（见 CheckUsePluginRun），故不在此早退。
        // :5858 CM_TURN —— **已移植**（见 CheckUsePluginTurn），故不在此早退。
        // :6690-6699 攻击族（CM_HIT..CM_115HIT + 自定义技能区间）
        if (ident == CM_HIT || ident == CM_HEAVYHIT || ident == CM_BIGHIT ||
            ident == CM_POWERHIT || ident == CM_LONGHIT || ident == CM_WIDEHIT ||
            ident == CM_FIREHIT || ident == CM_CRSHIT || ident == CM_TWNHIT ||
            ident == CM_SWORDHIT || ident == CM_43HIT || ident == CM_66HIT ||
            ident == CM_66HIT1 || ident == CM_101HIT || ident == CM_102HIT ||
            ident == CM_103HIT || ident == CM_113HIT || ident == CM_115HIT ||
            (ident >= CM_CUSTOM_HIT001 && ident < CM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT)) return true;
        // :7889 CM_SPELL —— **已移植**（见 CheckUsePluginSpell），故不在此早退。
        return false;
    }

    /// <summary>
    /// 原文 :3465-9688 <c>function TMirClientContext.CheckUsePlugin(Msg: PProcessMsg): Boolean;</c>
    /// <para>
    /// **已覆盖**：前导段 3505-3519、`CM_TURN` 5855-6681（<see cref="CheckUsePluginTurn"/>）、
    /// `CM_SPELL` 7886-8995（<see cref="CheckUsePluginSpell"/>）、
    /// `CM_SITDOWN` 8997-9469（<see cref="CheckUsePluginSitDown"/>）、
    /// `CM_DROPITEM` 9474-9487、`CM_PICKUP` 9492-9505、`else` 9506-9516、
    /// 公共收尾 9521-9681（提取为 <see cref="CheckUsePluginPostlude"/>）、兜底 9682-9686。
    /// </para>
    /// <para>
    /// **未覆盖**：攻击族 6690-7888（725 行，口径见文件头）—— 显式早退，见 <see cref="UnportedIdentFamilies"/>。
    /// </para>
    /// </summary>
    public bool CheckUsePlugin(TProcessMsg Msg)
    {
        // ---- 局部变量（原文 :3474-3503，逐条保留）----
        string sSendMsg = "";                                    // :3474
        // C# 要求 ref 实参明确赋值 → ConcurrentCount 置 0（原文是未初始化局部量，唯一写入点是
        // :7965 CM_SPELL 分支的 `ConcurrentCount := 0`；托管侧的 0 初值与该赋值一致，语义无差异）
        int nDelayTime = 0, ConcurrentCount = 0, Len;             // :3475
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
            // ★**未移植**族早退（原文 :6690 攻击族 —— CM_WALK / CM_RUN / CM_TURN / CM_SPELL /
            //   CM_SITDOWN 均已移植，不再早退）。见本文件头 §偏差。
            // =========================================================================
            if (UnportedIdentFamilies(DefMsg.Ident))
            {
                return false;
            }

            // 原文 :3522/3528 的 `{$IF NEED_REGISTER = 0} case DefMsg.Ident of {$ELSE} if DefMsg.Ident = CM_WALK`
            // 结构 → 活分支是 if/else-if 链；**已移植的分支按原文顺序排列在本链首部**：
            //   * CM_WALK :3525-4692 ✅
            //   * CM_RUN :4694-5853 ✅
            //   * CM_TURN :5855-6681 ✅
            //   * CM_SPELL :7886-8995 ✅
            //   * CM_SITDOWN :8997-9469 ✅
            //   * 攻击族 :6690 → 仍由上面的 `UnportedIdentFamilies` 早退拦下，后续车道按序插入本链首部。
            // 下面依次是原文 :9474 CM_DROPITEM / :9492 CM_PICKUP / :9506 else。

            // {$IF NEED_REGISTER = 0} CM_WALK: {$ELSE}         :3525-3528
            if (DefMsg.Ident == CM_WALK)
            {
                // ---- 分支体 :3530-4692（提取为 CheckUsePluginWalk）----
                if (CheckUsePluginWalk(Msg, DefMsg, dwCurTick, ref sSendMsg, ref nDelayTime,
                        ref AntiPlugAction, ref ErrorCode, ref dwCurrentInterval,
                        ref ConcurrentCount, ref IsDropConcurrent, ref nCompensationValue, ref Result))
                {
                    // 原文 :4368（丢弃并发，Result 已置 True）、:4455 / :4491（连续超速）
                    return Result;
                }
            }

            // {$IF NEED_REGISTER = 0} CM_RUN: {$ELSE}          :4694-4697
            else if (DefMsg.Ident == CM_RUN)
            {
                // ---- 分支体 :4699-5853（提取为 CheckUsePluginRun）----
                if (CheckUsePluginRun(Msg, DefMsg, dwCurTick, ref sSendMsg, ref nDelayTime,
                        ref AntiPlugAction, ref ErrorCode, ref dwCurrentInterval,
                        ref ConcurrentCount, ref IsDropConcurrent, ref nCompensationValue, ref Result))
                {
                    // 原文 :5526（丢弃并发，Result 已置 True）、:5616 / :5651（连续超速）
                    return Result;
                }
            }

            // {$IF NEED_REGISTER = 0} CM_TURN: {$ELSE}         :5855-5858
            else if (DefMsg.Ident == CM_TURN)
            {
                // ---- 分支体 :5860-6681（提取为 CheckUsePluginTurn）----
                if (CheckUsePluginTurn(Msg, DefMsg, dwCurTick,
                        ref sSendMsg, ref nDelayTime, ref AntiPlugAction, ref ErrorCode, ref dwCurrentInterval))
                {
                    // 原文 :5897 / :5927 / :5972 / :6007 的 `Exit`
                    return Result;
                }
            }

            // {$IF NEED_REGISTER = 0} CM_SPELL: {$ELSE}        :7886-7889
            else if (DefMsg.Ident == CM_SPELL)
            {
                // ---- 分支体 :7891-8995（提取为 CheckUsePluginSpell）----
                if (CheckUsePluginSpell(Msg, DefMsg, dwCurTick, ref sSendMsg, ref nDelayTime,
                        ref AntiPlugAction, ref ErrorCode, ref dwCurrentInterval,
                        ref ConcurrentCount, ref IsDropConcurrent, ref Result))
                {
                    // 原文 :7928 / :7958（暗杀）、:8788 / :8824（连续超速）、:8754（丢弃并发，Result 已置 True）
                    return Result;
                }
            }

            // {$IF NEED_REGISTER = 0} CM_SITDOWN: {$ELSE}     :8997-9000
            else if (DefMsg.Ident == CM_SITDOWN)
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
                IsDropConcurrent, ConcurrentCount, dwCurrentInterval, ErrorCode);
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

    // =================================================================================
    // 原文 :5855-6681  `else if DefMsg.Ident = CM_TURN then`（**转身**）分支体
    //
    // ★ 骨架
    //   :5861      ErrorCode := 4;
    //   :5862-5866 环形缓冲写入 baTurn
    //   :5868-5930 暗杀检测（前导动作集合 = [baWalk, baRun, baHit, baSpell]，目标动作 = baTurn）
    //   :5931      Inc(nRecordActionIndex)
    //   :5933-6623 四个按 FLastAction 互斥的限速子块（见下表）
    //   :6625-6673 按 FLastAction 打调试日志（ErrorCode 32/33/34/35/36）
    //   :6675-6678 nDelayTime = 0 时**只**刷新 dwTicks[amTurn]
    //   :6680      FLastAction := baTurn;
    //
    // ★ 四个限速子块的**逐字等价性（已用脚本机械化核对）**
    //   把四个子块按“模式名/取 tick 的模式/ErrorCode/日志格式”归一化后做集合比较，
    //   结果只差下表列出的 5 个轴（`div 3  then` 多一个空格是原文笔误，不影响语义）：
    //
    //   | 子块 | 原文行 | FLastAction | mode | 取 tick 的模式 | 连续超速断开块 | ErrorCode | 日志 |
    //   |---|---|---|---|---|---|---|---|
    //   | a | :5933-6114 | baTurn        | amTurn        | amTurn        | **有**（:5956-5974、:5989-6010） | 28 | 带 `; [移动速度%s]` + GetSpeedText(nMoveSpeed) |
    //   | b | :6117-6282 | baHit         | amHitToTurn   | amHit         | 无（同一段被 `{}` 注释，:6139-6152、:6167-6179） | 29 | 无 |
    //   | c | :6285-6450 | baSpell       | amSpellToTurn | amSpell       | 无（:6307-6320、:6335-6347） | 30 | 无 |
    //   | d | :6453-6623 | baWalk/baRun  | amMoveToTurn  | amWalk/amRun（:6459-6462 按 FLastAction 选） | 无（:6480-6493、:6508-6520） | 31 | 无 |
    //
    //   故本车道把这一公共体提取为 `TurnFamilySpeedBlock(mode, tickMode, hasContinueSpeedBlock,
    //   errorCode, logWithMoveSpeed, …)`（**结构性偏差 D-T1，与 `CheckUsePluginPostlude` 同类**）：
    //   每个语句、判据、常量、顺序、注释都只写一遍，四个调用点各自传入上表的 5 个轴值。
    //
    // ★ 原文缺陷 / 易错点（照抄）
    //   D-T2 :6667 `else if {(FLastAction = baTurn) and} g_Config.ActionList[amTurn].boDebug then`
    //        —— `FLastAction = baTurn` 被 `{}` 注释掉 → 该分支对**任何**非
    //        {baHit, baSpell, baWalk, baRun} 的 FLastAction（含 baOther/baCutMeat）都成立。
    //   D-T3 :6675 的 `and (not Msg.boDelay)` 同样被注释 → 只判 `nDelayTime = 0`（与 CM_SITDOWN 的 D-S3 同型）。
    //   D-T4 :6677 只刷新 `dwTicks[amTurn]` 一个槽（CM_SITDOWN 的 :9463-9465 刷三个，两者不同）。
    //   D-T5 :5933-6623 是 if/else-if 链且**没有 else**：FLastAction = baOther 时四个限速子块全不执行；
    //        而 :6667 的调试日志分支却会命中（见 D-T2）。
    //   D-T6 :6445/:6618 `dwCurrentInterval - dwTempInterval` 是 LongWord 减法 → 回绕成负 int（同 D-S）。
    // =================================================================================

    /// <summary>
    /// 原文 :5860-6681 —— <c>CM_TURN</c>（转身）分支体，1:1 移植。
    /// 返回 <c>true</c> 表示原文执行了 <c>Exit</c>（:5897 / :5927 暗杀、:5972 / :6007 连续超速）。
    /// </summary>
    private bool CheckUsePluginTurn(TProcessMsg Msg, in TDefaultMessage DefMsg, uint dwCurTick,
        ref string sSendMsg, ref int nDelayTime, ref TAntiPlugAction AntiPlugAction, ref int ErrorCode,
        ref uint dwCurrentInterval)
    {
        // ---- 原文 :3474-3502 中被本分支独占的局部量（同名同类型）----
        int I, PreIndex, PrePreIndex;                                       // :3483
        int nAssasinate;                                                    // :3489

        ErrorCode = 4;                                                      // :5861
        if (nRecordActionIndex >= MirClientContextConst.MAX_RECORD_ACTION_COUNT || nRecordActionIndex < 0)
            nRecordActionIndex = 0;                                         // :5863
        RecordActionArr[nRecordActionIndex].Action = TBaseAction.baTurn;                    // :5864
        RecordActionArr[nRecordActionIndex].Tick = MyGetTickCount();                        // :5865
        RecordActionArr[nRecordActionIndex].DefMsg = DefMsg;                                // :5866

        // 所有采集满了
        if (RecordActionArr[MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1].Tick != 0)   // :5869
        {
            nAssasinate = 0;                                                // :5871
            PreIndex = (nRecordActionIndex - 1 + MirClientContextConst.MAX_RECORD_ACTION_COUNT)
                % MirClientContextConst.MAX_RECORD_ACTION_COUNT;            // :5872

            // 转向前面是其他包
            if (IsTurnAssasinatePreAction(RecordActionArr[PreIndex].Action) &&      // :5875
                RunGateTiming.TickDiff(RecordActionArr[PreIndex].Tick, MyGetTickCount()) <= 250)   // :5876
            {
                nAssasinate++;                                              // :5878 Inc

                for (I = 2; I <= MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1; I++)   // :5880
                {
                    PreIndex = (nRecordActionIndex - I + MirClientContextConst.MAX_RECORD_ACTION_COUNT)
                        % MirClientContextConst.MAX_RECORD_ACTION_COUNT;    // :5882
                    if (RecordActionArr[PreIndex].Action == TBaseAction.baTurn)     // :5883
                    {
                        PrePreIndex = (nRecordActionIndex - I - 1 + MirClientContextConst.MAX_RECORD_ACTION_COUNT)
                            % MirClientContextConst.MAX_RECORD_ACTION_COUNT;    // :5885
                        if (IsTurnAssasinatePreAction(RecordActionArr[PrePreIndex].Action) &&   // :5886
                            RunGateTiming.TickDiff(RecordActionArr[PrePreIndex].Tick,
                                RecordActionArr[PreIndex].Tick) <= 250)         // :5887
                        {
                            nAssasinate++;                                  // :5889 Inc
                        }
                    }
                }

                if (nAssasinate >= 3)                                       // :5894
                {
                    ProcessAssasinate();                                    // :5896
                    return true;                                            // :5897 Exit
                }
            }
        }
        else
        {
            nAssasinate = 0;                                                // :5903
            PreIndex = nRecordActionIndex - 1;                              // :5904
            if (PreIndex >= 0 && IsTurnAssasinatePreAction(RecordActionArr[PreIndex].Action) &&   // :5905
                RunGateTiming.TickDiff(RecordActionArr[PreIndex].Tick, MyGetTickCount()) <= 250)   // :5906
            {
                nAssasinate++;                                              // :5908 Inc

                for (I = 2; I <= MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1; I++)   // :5910
                {
                    PreIndex = nRecordActionIndex - I;                      // :5912
                    if (PreIndex > 0 && RecordActionArr[PreIndex].Action == TBaseAction.baTurn)   // :5913
                    {
                        PrePreIndex = nRecordActionIndex - I - 1;           // :5915
                        if (PrePreIndex > 0 && IsTurnAssasinatePreAction(RecordActionArr[PrePreIndex].Action) &&   // :5916
                            RunGateTiming.TickDiff(RecordActionArr[PrePreIndex].Tick,
                                RecordActionArr[PreIndex].Tick) <= 250)     // :5917
                        {
                            nAssasinate++;                                  // :5919 Inc
                        }
                    }
                }

                if (nAssasinate >= 3)                                       // :5924
                {
                    ProcessAssasinate();                                    // :5926
                    return true;                                            // :5927 Exit
                }
            }
        }
        nRecordActionIndex++;                                               // :5931 Inc

        if (FLastAction == TBaseAction.baTurn)                              // :5933
        {
            // 子块 a（:5935-6114）：amTurn，**唯一**带“连续超速断开”的活分支
            if (TurnFamilySpeedBlock(TAntiPlugActionMode.amTurn, TAntiPlugActionMode.amTurn,
                    hasContinueSpeedBlock: true, errorCode: 28, logWithMoveSpeed: true,
                    Msg, dwCurTick, ref sSendMsg, ref nDelayTime, ref AntiPlugAction, ref ErrorCode,
                    ref dwCurrentInterval))
            {
                return true;                                                // :5972 / :6007 Exit
            }
        }

        // 攻击到转向
        else if (FLastAction == TBaseAction.baHit)                          // :6117
        {
            // 子块 b（:6119-6281）：amHitToTurn/amHit，连续超速段被注释 → hasContinueSpeedBlock = false
            if (TurnFamilySpeedBlock(TAntiPlugActionMode.amHitToTurn, TAntiPlugActionMode.amHit,
                    hasContinueSpeedBlock: false, errorCode: 29, logWithMoveSpeed: false,
                    Msg, dwCurTick, ref sSendMsg, ref nDelayTime, ref AntiPlugAction, ref ErrorCode,
                    ref dwCurrentInterval))
            {
                return true;                                                // 该子块内的 Exit 均在被注释段内（不可达）
            }
        }

        // 魔法到转向
        else if (FLastAction == TBaseAction.baSpell)                        // :6285
        {
            // 子块 c（:6287-6449）
            if (TurnFamilySpeedBlock(TAntiPlugActionMode.amSpellToTurn, TAntiPlugActionMode.amSpell,
                    hasContinueSpeedBlock: false, errorCode: 30, logWithMoveSpeed: false,
                    Msg, dwCurTick, ref sSendMsg, ref nDelayTime, ref AntiPlugAction, ref ErrorCode,
                    ref dwCurrentInterval))
            {
                return true;                                                // 同上（不可达）
            }
        }

        // 移动到转向
        else if (FLastAction == TBaseAction.baWalk || FLastAction == TBaseAction.baRun)   // :6453
        {
            // :6459-6462 取 tick 的模式按 FLastAction 二选一
            TAntiPlugActionMode tickMode = FLastAction == TBaseAction.baWalk
                ? TAntiPlugActionMode.amWalk : TAntiPlugActionMode.amRun;
            // 子块 d（:6455-6622）
            if (TurnFamilySpeedBlock(TAntiPlugActionMode.amMoveToTurn, tickMode,
                    hasContinueSpeedBlock: false, errorCode: 31, logWithMoveSpeed: false,
                    Msg, dwCurTick, ref sSendMsg, ref nDelayTime, ref AntiPlugAction, ref ErrorCode,
                    ref dwCurrentInterval))
            {
                return true;                                                // 同上（不可达）
            }
        }

        // :6625-6673 调试日志
        if (!Msg.boDelay)                                                   // :6625
        {
            if (FLastAction == TBaseAction.baHit)                           // :6627
            {
                if (g_Config.ActionList[(int)TAntiPlugActionMode.amHitToTurn].boDebug)   // :6629
                {
                    ErrorCode = 32;                                         // :6631
                    AddMainLogMsg(Format("%s:%d; 用户:%s",                   // :6632-6633
                        AntiPlugActionModeNames3[(int)TAntiPlugActionMode.amHitToTurn],
                        RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)TAntiPlugActionMode.amHit], dwCurTick),
                        sChrName), 0);
                }
            }

            else if (FLastAction == TBaseAction.baSpell)                    // :6637
            {
                if (g_Config.ActionList[(int)TAntiPlugActionMode.amSpellToTurn].boDebug)   // :6639
                {
                    ErrorCode = 33;                                         // :6641
                    AddMainLogMsg(Format("%s:%d; 用户:%s",                   // :6642-6643
                        AntiPlugActionModeNames3[(int)TAntiPlugActionMode.amSpellToTurn],
                        RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)TAntiPlugActionMode.amSpell], dwCurTick),
                        sChrName), 0);
                }
            }

            else if (FLastAction == TBaseAction.baWalk)                     // :6647
            {
                if (g_Config.ActionList[(int)TAntiPlugActionMode.amMoveToTurn].boDebug)   // :6649
                {
                    ErrorCode = 34;                                         // :6651
                    AddMainLogMsg(Format("%s:%d; 用户:%s",                   // :6652-6653
                        AntiPlugActionModeNames3[(int)TAntiPlugActionMode.amMoveToTurn],
                        RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)TAntiPlugActionMode.amWalk], dwCurTick),
                        sChrName), 0);
                }
            }

            else if (FLastAction == TBaseAction.baRun)                      // :6657
            {
                if (g_Config.ActionList[(int)TAntiPlugActionMode.amMoveToTurn].boDebug)   // :6659
                {
                    ErrorCode = 35;                                         // :6661
                    AddMainLogMsg(Format("%s:%d; 用户:%s",                   // :6662-6663
                        AntiPlugActionModeNames3[(int)TAntiPlugActionMode.amMoveToTurn],
                        RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)TAntiPlugActionMode.amRun], dwCurTick),
                        sChrName), 0);
                }
            }

            // :6667 原文 `else if {(FLastAction = baTurn) and} g_Config.ActionList[amTurn].boDebug then`
            //       —— 前半被 `{}` 注释掉 → 对**任何**非 {baHit,baSpell,baWalk,baRun} 的 FLastAction 都成立（D-T2）
            else if (/*{(FLastAction = baTurn) and}*/ g_Config.ActionList[(int)TAntiPlugActionMode.amTurn].boDebug)
            {
                ErrorCode = 36;                                             // :6669
                AddMainLogMsg(Format("%s:%d; 用户:%s",                       // :6670-6671
                    AntiPlugActionModeNames3[(int)TAntiPlugActionMode.amTurn],
                    RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)TAntiPlugActionMode.amTurn], dwCurTick),
                    sChrName), 0);
            }
        }

        if (nDelayTime == 0 /*{and (not Msg.boDelay)}*/)                    // :6675
        {
            GameSpeed.dwTicks[(int)TAntiPlugActionMode.amTurn] = dwCurTick; // :6677（**只刷这一个槽**，D-T4）
        }

        FLastAction = TBaseAction.baTurn;                                   // :6680

        return false;   // 原文 :6681 分支体结束 → 继续进入公共收尾 9521-9681
    }

    /// <summary>
    /// 原文 :5935-6114 / :6119-6281 / :6287-6449 / :6455-6622 —— CM_TURN 族四个
    /// <c>FLastAction</c> 子块**逐字相同**的限速体（提取见本文件头 D-T1）。
    /// <para>
    /// 五个差异轴全部由形参给出：<paramref name="mode"/>（`g_Config.ActionList[…]` 的下标与
    /// `LastLockAntiPlugActionMode` 的取值）、<paramref name="tickMode"/>（`GameSpeed.dwTicks[…]` 的取
    /// 值下标）、<paramref name="hasContinueSpeedBlock"/>（`boContinueSpeedCloseSocket` 的
    /// “连续超速直接断开”段：**只有 amTurn 子块是活代码**）、<paramref name="errorCode"/>
    /// （`ErrorCode := 28/29/30/31`）、<paramref name="logWithMoveSpeed"/>（日志是否带
    /// `; [移动速度%s]` + `GetSpeedText(nMoveSpeed)`）。
    /// </para>
    /// <para>
    /// 行号注释以**子块 a** 为准（b/c/d 依次为 :6119-6281 / :6287-6449 / :6455-6622，逐行同构）。
    /// 返回 <c>true</c> 表示原文执行了 <c>Exit</c>（仅子块 a 可达：:5972 / :6007）。
    /// </para>
    /// </summary>
    private bool TurnFamilySpeedBlock(TAntiPlugActionMode mode, TAntiPlugActionMode tickMode,
        bool hasContinueSpeedBlock, int errorCode, bool logWithMoveSpeed,
        TProcessMsg Msg, uint dwCurTick, ref string sSendMsg, ref int nDelayTime,
        ref TAntiPlugAction AntiPlugAction, ref int ErrorCode, ref uint dwCurrentInterval)
    {
        TAntiPlugAction action = g_Config.ActionList[(int)mode];

        // :5935 `if g_Config.ActionList[amTurn].boEnabled then begin … end` —— 提前返回等价
        if (!action.boEnabled) return false;

        uint dwTempInterval = action.nInterval;                             // :5937

        return CollectSpeedDetect(mode, tickMode, dwTempInterval, errorCode,
            logWithMoveSpeed ? SpeedLogKind.MoveSpeed : SpeedLogKind.None, hasContinueSpeedBlock,
            default,   // CM_TURN 的子块没有 ErrorCode 阶段标记
            dwCurTick, Msg, ref sSendMsg, ref nDelayTime, ref AntiPlugAction, ref ErrorCode,
            ref dwCurrentInterval);
    }

    /// <summary>超速日志的三种形态（原文三处不同的 Format 模板）。</summary>
    private enum SpeedLogKind
    {
        /// <summary>`'【用户超速】%s:%d; 用户:%s'`（CM_TURN 子块 b/c/d、CM_SPELL 的四个“X到魔法”子块）。</summary>
        None,
        /// <summary>`'【用户超速】%s:%d; [移动速度%s]; 用户:%s'` + `GetSpeedText(nMoveSpeed)`（:6087-6091）。</summary>
        MoveSpeed,
        /// <summary>`'【用户超速】%s:%d; [魔法速度%s]; 用户:%s'` + `GetSpeedText(nSpellSpeed)`（:8900-8904）。</summary>
        SpellSpeed,
    }

    /// <summary>
    /// 原文在采集/判定体的若干**固定位置**插入的 `ErrorCode` 阶段标记。
    /// <para>
    /// CM_WALK / CM_RUN 的每个子块都在这些位置写 `ErrorCode := 2xx/3xx`（原文用于事后定位崩溃点），
    /// 而具体位置随子块略有不同 —— 每个调用点都注明了它用了哪几个。
    /// **0 = 该位置原文没有赋值**（原文从未写过 `ErrorCode := 0`，故 0 可安全充当“无”）。
    /// </para>
    /// </summary>
    private readonly struct SpeedStageCodes
    {
        /// <summary>interval 取完后、`dwCurrentInterval := tick_diff(…)` 之前（:3976 的 213）。</summary>
        public readonly int BeforeTick;
        /// <summary>守卫 `begin` 之后、interval 之前（:3784 的 208）—— 由包装方法读取。</summary>
        public readonly int AfterGuard;
        /// <summary>`SumSpeedProcessArr[…, 0]` 初始化之后（:3802 的 209 / :3987 的 214 / :4169 的 218）。</summary>
        public readonly int AfterSum;
        /// <summary>采集段之后、`boContinueSpeedPass := …` 之前（:3707 的 205 / :3892 的 210 / :4876 的 305）。</summary>
        public readonly int AfterCollect;
        /// <summary>`boContinueSpeedPass := …` 之后、判定 `if` 之前（:4882 的 306 / :5240 的 312）。</summary>
        public readonly int AfterPass;
        /// <summary>判定 if/else 之后、采集池写入之前（:3761 的 206 / :4931 的 307）。</summary>
        public readonly int BeforeWrite;

        public SpeedStageCodes(int beforeTick = 0, int afterGuard = 0, int afterSum = 0,
            int afterCollect = 0, int afterPass = 0, int beforeWrite = 0)
        {
            BeforeTick = beforeTick;
            AfterGuard = afterGuard;
            AfterSum = afterSum;
            AfterCollect = afterCollect;
            AfterPass = afterPass;
            BeforeWrite = beforeWrite;
        }
    }

    /// <summary>
    /// `CheckUsePlugin` 里所有 ident 族共用的**采集池 + 超速判定**体
    /// （原文 CM_TURN 的 :5941-6112 与 CM_SPELL 的四个“X到魔法”子块，
    /// 两者逐字相同，只有 4 个轴不同：<paramref name="mode"/>、<paramref name="tickMode"/>、
    /// <paramref name="dwTempInterval"/> 的来源（`ActionList[mode].nInterval` vs
    /// `g_wActionSpeedIntervals[mode][…]`）、<paramref name="errorCode"/> 与 <paramref name="logKind"/>）。
    /// <para>
    /// 行号注释以 CM_TURN 子块 a 为准（:5941-6112；CM_SPELL 的同一段见 :8004-8184 等）。
    /// 返回 <c>true</c> 表示原文执行了 <c>Exit</c>（仅 <paramref name="hasContinueSpeedBlock"/> 为真时可达）。
    /// </para>
    /// </summary>
    private bool CollectSpeedDetect(TAntiPlugActionMode mode, TAntiPlugActionMode tickMode,
        uint dwTempInterval, int errorCode, SpeedLogKind logKind, bool hasContinueSpeedBlock,
        SpeedStageCodes stages,
        uint dwCurTick, TProcessMsg Msg, ref string sSendMsg, ref int nDelayTime,
        ref TAntiPlugAction AntiPlugAction, ref int ErrorCode, ref uint dwCurrentInterval)
    {
        int m = (int)mode;
        TAntiPlugAction action = g_Config.ActionList[m];
        bool boCurrentSpeed, boCollectSpeed;                                // :3486
        bool boContinueSpeed = false;                                       // :3486
        bool boContinueSpeedPass;                                           // :3487
        int nSpeedCount;                                                    // :3481
        int I, nCollectIndex, nCollectCount;                                // :3483

        if (stages.BeforeTick != 0) ErrorCode = stages.BeforeTick;          // :3976 等
        dwCurrentInterval = RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)tickMode], dwCurTick);   // :5938
        boCurrentSpeed = dwCurrentInterval < dwTempInterval;                // :5939

        nSpeedCount = 0;                                                    // :5941
        boCollectSpeed = false;                                             // :5942

        if (SumSpeedProcessArr[m, 0] == 0)                                  // :5944
            SumSpeedProcessArr[m, 0] = MyGetTickCount();                    // :5945

        if (stages.AfterSum != 0) ErrorCode = stages.AfterSum;              // :3802 / :3987 / :4169 等

        if (g_Config.dwCollectCount /*{g_Config.ActionList[amTurn].nCollectCount}*/ >= 2)   // :5947
        {
            nCollectIndex = nCollectIntervalIndexArr[m];                    // :5949
            nCollectCount = g_Config.dwCollectCount /*{g_Config.ActionList[amTurn].nCollectCount}*/;   // :5950

            // 倒数第2条数据采集到，加本次就是最一条搞定
            if (dwCollectIntervalArr[m, nCollectCount - 2] != 0)            // :5953
            {
                // 本次和上次都超速就算超速 chongchong 2016-10-07
                // （**只有 amTurn 子块是活代码**；b/c/d 的同一段被 `{}` 注释 → hasContinueSpeedBlock = false）
                if (hasContinueSpeedBlock)
                {
                    if (g_Config.boContinueSpeedCloseSocket && boCurrentSpeed)   // :5956
                    {
                        boContinueSpeed = true;                             // :5958
                        for (I = 1; I <= g_Config.nContinueSpeedCount; I++) // :5959
                        {
                            if (dwCollectIntervalArr[m,
                                    (nCollectIndex - I + nCollectCount) % nCollectCount] >= 0)   // :5961
                            {
                                boContinueSpeed = false;                    // :5963
                                break;                                      // :5964 Break
                            }
                        }

                        // 连续三次超速直接断开
                        if (boContinueSpeed)                                // :5969
                        {
                            ContinuousSpeed(mode, dwCurrentInterval);       // :5971
                            return true;                                    // :5972 Exit
                        }
                    }
                }

                for (I = 0; I <= nCollectCount - 1; I++)                    // :5976
                {
                    if (I != nCollectIndex && dwCollectIntervalArr[m, I] < 0)   // :5978
                        nSpeedCount++;                                      // :5979 Inc
                }
                if (boCurrentSpeed) nSpeedCount++;                          // :5981 Inc
                boCollectSpeed = nSpeedCount >= g_Config.dwSpeedValue;      // :5982  // g_Config.ActionList[amTurn].nCollectSpeedCount;
            }
            else
            {
                // 至少采集了1条
                if (nCollectIndex >= 1)                                     // :5987
                {
                    // 本次和上次都超速就算超速 chongchong 2016-10-07（同上，只有 amTurn 是活代码）
                    if (hasContinueSpeedBlock)
                    {
                        if (g_Config.boContinueSpeedCloseSocket && boCurrentSpeed)   // :5989
                        {
                            if (nCollectIndex >= g_Config.nContinueSpeedCount)   // :5991
                            {
                                boContinueSpeed = true;                     // :5993
                                for (I = 1; I <= g_Config.nContinueSpeedCount; I++)   // :5994
                                {
                                    if (dwCollectIntervalArr[m, nCollectIndex - I] >= 0)   // :5996
                                    {
                                        boContinueSpeed = false;            // :5998
                                        break;                              // :5999 Break
                                    }
                                }

                                // 连续三次超速直接断开
                                if (boContinueSpeed)                        // :6004
                                {
                                    ContinuousSpeed(mode, dwCurrentInterval);   // :6006
                                    return true;                            // :6007 Exit
                                }
                            }
                        }
                    }

                    for (I = 0; I <= nCollectIndex - 1; I++)                // :6012
                    {
                        if (dwCollectIntervalArr[m, I] < 0)                 // :6014
                            nSpeedCount++;                                  // :6015 Inc
                    }
                    if (boCurrentSpeed) nSpeedCount++;                      // :6017 Inc

                    if (dwCurrentInterval <= dwTempInterval / 3)            // :6019
                    {
                        boCollectSpeed = true;                              // :6021
                    }
                    else
                    {
                        if (nCollectIndex + 1 <= 3)                         // :6025
                            boCollectSpeed = nSpeedCount >= 2;              // :6026
                        else if (nCollectIndex + 1 <= 7)                    // :6027
                        {
                            boCollectSpeed = nSpeedCount >= (nCollectIndex + 1) / 2;   // :6029
                        }
                        else
                        {
                            boCollectSpeed = nSpeedCount >= (nCollectIndex + 1) / 2 - 1;   // :6033
                        }
                    }
                }
                // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                else if (dwCurrentInterval <= dwTempInterval / 3)           // :6038
                {
                    boCollectSpeed = true;                                  // :6040
                }
            }
        }
        else if (dwCurrentInterval <= dwTempInterval / 3)                   // :6044
        {
            boCollectSpeed = true;                                          // :6046
        }

        if (stages.AfterCollect != 0) ErrorCode = stages.AfterCollect;      // :3707 的 205 等

        // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
        boContinueSpeedPass = GameSpeed.boContinueSpeed &&                  // :6050
            (RunGateTiming.TickDiff(GameSpeed.dwStartSpeedTick, MyGetTickCount()) >=
                dwTempInterval + g_Config.dwContinueSpeedPassIncTime);      // :6051

        if (stages.AfterPass != 0) ErrorCode = stages.AfterPass;            // :4882 的 306 等

        if ((dwCurrentInterval <= dwTempInterval / 10) ||                   // :6053
            ((!boContinueSpeedPass) && boCurrentSpeed && boCollectSpeed))   // :6054
        {
            if (action.boShowHint)                                          // :6056
                sSendMsg = action.sHintText;                                // :6057

            AntiPlugAction = action;                                        // :6059 AntiPlugAction := @g_Config.ActionList[amTurn]
            LastLockAntiPlugActionMode = mode;                              // :6060

            if (RunGateTiming.TickDiff(SumSpeedProcessArr[m, 0], MyGetTickCount()) <=
                g_Config.nSumSpeedCheckTime * 1000)                         // :6062
                SumSpeedProcessArr[m, 1]++;                                 // :6063 Inc
            else
            {
                SumSpeedProcessArr[m, 1] = 1;                               // :6066
                SumSpeedProcessArr[m, 0] = MyGetTickCount();                // :6067
            }

            if (AntiPlugAction.ProcessMode == TActionProcessMode.apmDelay &&
                dwCurrentInterval < dwTempInterval)                         // :6070
            {
                nDelayTime = (int)(dwTempInterval - dwCurrentInterval) + DELAY_TIME_ADD;   // :6072

                // 修正加速一段时间后恢复到正常状态，一直提示加速
                GameSpeed.nDelayCount[m] = GameSpeed.nDelayCount[m] + 1;    // :6075
                if (GameSpeed.nDelayCount[m] > 8)                           // :6076
                {
                    GameSpeed.nDelayCount[m] = 0;                           // :6078
                    nDelayTime = 0;                                         // :6079
                    SendActionRet(true);                                    // :6080
                }
            }

            if (g_Config.boShowAttackLog)                                   // :6084
            {
                ErrorCode = errorCode;                                      // :6086 / :6255 / :6423 / :6596 / :8159 等
                if (logKind == SpeedLogKind.MoveSpeed)
                {
                    // :6087-6091（**只有 CM_TURN 的 amTurn 子块带 `; [移动速度%s]`**）
                    AddMainLogMsg(Format("【用户超速】%s:%d; [移动速度%s]; 用户:%s",
                        AntiPlugActionModeNames3[m], dwCurrentInterval, GetSpeedText(nMoveSpeed), sChrName), 0);
                }
                else if (logKind == SpeedLogKind.SpellSpeed)
                {
                    // :8900-8904（**CM_SPELL 的 amSpell 子块带 `; [魔法速度%s]`**）
                    AddMainLogMsg(Format("【用户超速】%s:%d; [魔法速度%s]; 用户:%s",
                        AntiPlugActionModeNames3[m], dwCurrentInterval, GetSpeedText(nSpellSpeed), sChrName), 0);
                }
                else
                {
                    AddMainLogMsg(Format("【用户超速】%s:%d; 用户:%s",
                        AntiPlugActionModeNames3[m], dwCurrentInterval, sChrName), 0);
                }
            }
        }
        else
        {
            if (!Msg.boDelay && !boContinueSpeedPass)                       // :6096
            {
                GameSpeed.nDelayCount[m] = 0;                               // :6098
            }
        }

        if (stages.BeforeWrite != 0) ErrorCode = stages.BeforeWrite;        // :3761 的 206 等

        if (nDelayTime == 0 && !Msg.boDelay)                                // :6102
        {
            nCollectIndex = nCollectIntervalIndexArr[m];                    // :6104

            if (dwCurrentInterval >= dwTempInterval)                        // :6106
                dwCollectIntervalArr[m, nCollectIndex] = 1;                 // :6107
            else
                dwCollectIntervalArr[m, nCollectIndex] =
                    unchecked((int)(dwCurrentInterval - dwTempInterval));   // :6109

            nCollectIntervalIndexArr[m] = (nCollectIndex + 1)
                % g_Config.dwCollectCount /*{g_Config.ActionList[amTurn].nCollectCount}*/;   // :6111
        }

        return false;
    }

    /// <summary>
    /// 原文 `RecordActionArr[…].Action in [baWalk, baRun, baHit, baSpell]`
    /// （:5875 / :5886 / :5905 / :5916 四处，CM_TURN 族的暗杀前导动作集合 —— **与 CM_SITDOWN 的不同**：
    /// CM_SITDOWN 是 `[baHit, baSpell, baWalk, baRun, baTurn]`，这里是 `[baWalk, baRun, baHit, baSpell]`，
    /// 即 CM_TURN 把 `baTurn` 换成自身并**不含** `baTurn`、CM_SITDOWN 含 `baTurn` 而不含 `baCutMeat`）。
    /// </summary>
    private static bool IsTurnAssasinatePreAction(TBaseAction action) =>
        action == TBaseAction.baWalk || action == TBaseAction.baRun ||
        action == TBaseAction.baHit || action == TBaseAction.baSpell;

    // =================================================================================
    // 原文 :7886-8995  `else if DefMsg.Ident = CM_SPELL then`（**魔法**）分支体
    //
    // ★ 骨架
    //   :7892      ErrorCode := 5;
    //   :7893-7897 环形缓冲写入 baSpell
    //   :7899-7961 暗杀检测（前导动作集合 = **[baTurn, baCutMeat]**，目标动作 = baSpell）
    //              —— 与 CM_SITDOWN（[baHit,baSpell,baWalk,baRun,baTurn]）和
    //                 CM_TURN（[baWalk,baRun,baHit,baSpell]）**三者互不相同**
    //   :7962      Inc(nRecordActionIndex)
    //   :7964-8008 **魔法并发**块（`amSpellConcurrent` + `GetConcurrentPacketCount`；
    //              命中后若 FLastAction = baSpell 再按 `nSpellSpeed` 判 `IsDropConcurrent`）
    //   :8010-8981 `if AntiPlugAction = nil then` 包住的五个 FLastAction 子块 + 调试日志
    //   :8983-8992 nDelayTime = 0 时刷新 dwTicks[amSpell] / [amSpellToWalk] / [amSpellToRun]（**三个槽**）
    //   :8994      FLastAction := baSpell;
    //
    // ★ 五个子块与 CM_TURN 的两处结构性差异
    //   (1) `dwTempInterval` **不是** `ActionList[mode].nInterval`，而是
    //       `g_wActionSpeedIntervals[mode][…]` 按 `nSpellSpeed` 三支取值（:7993-7998 / :8018-8023 /
    //       :8194-8199 / :8370-8375 / :8546-8551 / :8719-8724），且 `ActionList[mode].nInterval`
    //       那一行被 `//` 注释掉（:8017 / :8193 / :8369 / :8545）。提取为 `SpellSpeedInterval(mode)`；
    //   (2) 前四个子块额外要求 `btJob <> 0`（:8015 / :8191 / :8367 / :8543）；第 5 个（amSpell）**不要求**
    //       （原文 :8717 只有 `btJob <> 0` 而没有 `FLastAction = baSpell`，后者被 `{}` 注释）。
    //   除这 2 点外，前四个子块与 CM_TURN 的子块**逐字相同**（已用脚本机械化核对），
    //   故共用 `CollectSpeedDetect`（见 D-T1 的说明）。
    //
    // ★ 原文缺陷 / 易错点（照抄）
    //   D-P1 :8754 的 `Exit` 之前有 `Result := True`（:8753）—— **整个 CheckUsePlugin 里唯一一处
    //        Exit 返回 True**；其余 Exit 都返回 :3507 的 False。
    //   D-P2 :8728 是**赋值** `IsDropConcurrent := dwCurrentInterval <= dwTempInterval div 3`，
    //        而 :8002 是**条件** `if dwCurrentInterval <= … then IsDropConcurrent := True`
    //        —— 两处写法不同：前者会把已经为 True 的 IsDropConcurrent **改回 False**。
    //   D-P3 :8977 `ErrorCode := 61;` 在调试日志链之后**无条件执行** → :8934/:8944/:8954/:8964/:8972
    //        的 56-60 全被覆盖（ErrorCode 实际只可能是 61）。
    //   D-P4 :8015/:8191/:8367/:8543 的 `btJob <> 0` 在**外层 FLastAction 判据之内**：
    //        FLastAction 是走路/跑步/转向/挖肉时，若 `btJob = 0`，则该 else-if 分支被**消费掉**，
    //        后面的 amSpell 子块（:8717）**不会被评估**。
    //   D-P5 :8970 的 `else if {(FLastAction = baSpell) and} …amSpell.boDebug` 同 D-T2，条件里没有 FLastAction。
    //   D-P6 :7991 只在**并发块命中**（`ConcurrentCount >= nInterval`）时才算 `IsDropConcurrent`；
    //        未命中时 `IsDropConcurrent` 保持 :3518 的 False（但 :8728 会重新赋值）。
    // =================================================================================

    /// <summary>
    /// 原文 :7891-8995 —— <c>CM_SPELL</c>（魔法）分支体，1:1 移植。
    /// 返回 <c>true</c> 表示原文执行了 <c>Exit</c>（:7928 / :7958 暗杀、:8754 丢弃并发、:8788 / :8824 连续超速）。
    /// <paramref name="Result"/> 只有 :8753 一处会被置 True（D-P1）。
    /// </summary>
    private bool CheckUsePluginSpell(TProcessMsg Msg, in TDefaultMessage DefMsg, uint dwCurTick,
        ref string sSendMsg, ref int nDelayTime, ref TAntiPlugAction AntiPlugAction, ref int ErrorCode,
        ref uint dwCurrentInterval, ref int ConcurrentCount, ref bool IsDropConcurrent, ref bool Result)
    {
        // ---- 原文 :3474-3502 中被本分支独占/使用的局部量（同名同类型）----
        uint dwTempInterval;                                                // :3476
        bool boCurrentSpeed, boContinueSpeed = false, boCollectSpeed;       // :3486
        bool boContinueSpeedPass;                                           // :3487
        int nSpeedCount;                                                    // :3481
        int I, nCollectIndex, nCollectCount, PreIndex, PrePreIndex;         // :3483
        int nAssasinate;                                                    // :3489

        // 原文在本分支反复取 `g_Config.ActionList[…]`；托管侧 TAntiPlugAction 是引用类型 → 别名等价。
        const int AmSpell = (int)TAntiPlugActionMode.amSpell;                              // 1
        const int AmSpellToWalk = (int)TAntiPlugActionMode.amSpellToWalk;                  // 11
        const int AmSpellToRun = (int)TAntiPlugActionMode.amSpellToRun;                    // 13
        const int AmSpellConcurrent = (int)TAntiPlugActionMode.amSpellConcurrent;          // 25
        TAntiPlugAction SpellAction = g_Config.ActionList[AmSpell];
        TAntiPlugAction SpellConcurrentAction = g_Config.ActionList[AmSpellConcurrent];

        ErrorCode = 5;                                                      // :7892
        if (nRecordActionIndex >= MirClientContextConst.MAX_RECORD_ACTION_COUNT || nRecordActionIndex < 0)
            nRecordActionIndex = 0;                                         // :7894
        RecordActionArr[nRecordActionIndex].Action = TBaseAction.baSpell;                   // :7895
        RecordActionArr[nRecordActionIndex].Tick = MyGetTickCount();                        // :7896
        RecordActionArr[nRecordActionIndex].DefMsg = DefMsg;                                // :7897

        // 所有采集满了
        if (RecordActionArr[MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1].Tick != 0)   // :7900
        {
            nAssasinate = 0;                                                // :7902
            PreIndex = (nRecordActionIndex - 1 + MirClientContextConst.MAX_RECORD_ACTION_COUNT)
                % MirClientContextConst.MAX_RECORD_ACTION_COUNT;            // :7903

            // 转向前面是其他包
            if (IsSpellAssasinatePreAction(RecordActionArr[PreIndex].Action) &&     // :7906
                RunGateTiming.TickDiff(RecordActionArr[PreIndex].Tick, MyGetTickCount()) <= 250)   // :7907
            {
                nAssasinate++;                                              // :7909 Inc

                for (I = 2; I <= MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1; I++)   // :7911
                {
                    PreIndex = (nRecordActionIndex - I + MirClientContextConst.MAX_RECORD_ACTION_COUNT)
                        % MirClientContextConst.MAX_RECORD_ACTION_COUNT;    // :7913
                    if (RecordActionArr[PreIndex].Action == TBaseAction.baSpell)    // :7914
                    {
                        PrePreIndex = (nRecordActionIndex - I - 1 + MirClientContextConst.MAX_RECORD_ACTION_COUNT)
                            % MirClientContextConst.MAX_RECORD_ACTION_COUNT;    // :7916
                        if (IsSpellAssasinatePreAction(RecordActionArr[PrePreIndex].Action) &&   // :7917
                            RunGateTiming.TickDiff(RecordActionArr[PrePreIndex].Tick,
                                RecordActionArr[PreIndex].Tick) <= 250)         // :7918
                        {
                            nAssasinate++;                                  // :7920 Inc
                        }
                    }
                }

                if (nAssasinate >= 3)                                       // :7925
                {
                    ProcessAssasinate();                                    // :7927
                    return true;                                            // :7928 Exit
                }
            }
        }
        else
        {
            nAssasinate = 0;                                                // :7934
            PreIndex = nRecordActionIndex - 1;                              // :7935
            if (PreIndex >= 0 && IsSpellAssasinatePreAction(RecordActionArr[PreIndex].Action) &&   // :7936
                RunGateTiming.TickDiff(RecordActionArr[PreIndex].Tick, MyGetTickCount()) <= 250)   // :7937
            {
                nAssasinate++;                                              // :7939 Inc

                for (I = 2; I <= MirClientContextConst.MAX_RECORD_ACTION_COUNT - 1; I++)   // :7941
                {
                    PreIndex = nRecordActionIndex - I;                      // :7943
                    if (PreIndex > 0 && RecordActionArr[PreIndex].Action == TBaseAction.baSpell)   // :7944
                    {
                        PrePreIndex = nRecordActionIndex - I - 1;           // :7946
                        if (PrePreIndex > 0 && IsSpellAssasinatePreAction(RecordActionArr[PrePreIndex].Action) &&   // :7947
                            RunGateTiming.TickDiff(RecordActionArr[PrePreIndex].Tick,
                                RecordActionArr[PreIndex].Tick) <= 250)     // :7948
                        {
                            nAssasinate++;                                  // :7950 Inc
                        }
                    }
                }

                if (nAssasinate >= 3)                                       // :7955
                {
                    ProcessAssasinate();                                    // :7957
                    return true;                                            // :7958 Exit
                }
            }
        }
        nRecordActionIndex++;                                               // :7962 Inc

        // 魔法并发 chongchong 2014-12-16
        ConcurrentCount = 0;                                                // :7965
        if (SpellConcurrentAction.boEnabled || SpellConcurrentAction.boDebug)   // :7966
            ConcurrentCount = GetConcurrentPacketCount(DefMsg);             // :7967

        if (SpellConcurrentAction.boEnabled)                                // :7969
        {
            if (SumSpeedProcessArr[AmSpellConcurrent, 0] == 0)              // :7971
                SumSpeedProcessArr[AmSpellConcurrent, 0] = MyGetTickCount();   // :7972

            dwTempInterval = SpellConcurrentAction.nInterval;               // :7974
            if (ConcurrentCount >= dwTempInterval)                          // :7975
            {
                if (SpellConcurrentAction.boShowHint)                       // :7977
                    sSendMsg = SpellConcurrentAction.sHintText;             // :7978

                AntiPlugAction = SpellConcurrentAction;                     // :7980
                LastLockAntiPlugActionMode = TAntiPlugActionMode.amSpellConcurrent;   // :7981

                if (RunGateTiming.TickDiff(SumSpeedProcessArr[AmSpellConcurrent, 0],
                        MyGetTickCount()) <= g_Config.nSumSpeedCheckTime * 1000)   // :7983
                    SumSpeedProcessArr[AmSpellConcurrent, 1]++;             // :7984 Inc
                else
                {
                    SumSpeedProcessArr[AmSpellConcurrent, 1] = 1;           // :7987
                    SumSpeedProcessArr[AmSpellConcurrent, 0] = MyGetTickCount();   // :7988
                }

                if (FLastAction == TBaseAction.baSpell)                     // :7991
                {
                    dwTempInterval = SpellSpeedInterval(TAntiPlugActionMode.amSpell);   // :7993-7998

                    dwCurrentInterval = RunGateTiming.TickDiff(
                        GameSpeed.dwTicks[AmSpell], dwCurTick);             // :8000

                    if (dwCurrentInterval <= dwTempInterval / DROP_CONCURRENT_RATE)   // :8002
                    {
                        IsDropConcurrent = true;                            // :8004
                    }
                }
            }
        }

        if (AntiPlugAction == null)                                         // :8010
        {
            // 走路到魔法
            if (FLastAction == TBaseAction.baWalk)                          // :8013
            {
                if (btJob != 0 && g_Config.ActionList[(int)TAntiPlugActionMode.amWalkToSpell].boEnabled)   // :8015
                {
                    // :8017 `//dwTempInterval := g_Config.ActionList[amWalkToSpell].nInterval;`（原文已注释）
                    dwTempInterval = SpellSpeedInterval(TAntiPlugActionMode.amWalkToSpell);   // :8018-8023

                    if (SpellFamilySpeedBlock(TAntiPlugActionMode.amWalkToSpell,
                            TAntiPlugActionMode.amWalk, dwTempInterval, 50,
                            Msg, dwCurTick, ref sSendMsg, ref nDelayTime, ref AntiPlugAction, ref ErrorCode,
                            ref dwCurrentInterval))
                    {
                        return true;                                    // 该子块的 Exit 均在 `{}` 注释内（不可达）
                    }
                }
            }

            // 跑步到魔法
            else if (FLastAction == TBaseAction.baRun)                      // :8189
            {
                if (btJob != 0 && g_Config.ActionList[(int)TAntiPlugActionMode.amRunToSpell].boEnabled)   // :8191
                {
                    // :8193 原文注释同上
                    dwTempInterval = SpellSpeedInterval(TAntiPlugActionMode.amRunToSpell);   // :8194-8199

                    if (SpellFamilySpeedBlock(TAntiPlugActionMode.amRunToSpell,
                            TAntiPlugActionMode.amRun, dwTempInterval, 51,
                            Msg, dwCurTick, ref sSendMsg, ref nDelayTime, ref AntiPlugAction, ref ErrorCode,
                            ref dwCurrentInterval))
                    {
                        return true;                                    // 不可达
                    }
                }
            }

            // 转向到魔法
            else if (FLastAction == TBaseAction.baTurn)                     // :8365
            {
                if (btJob != 0 && g_Config.ActionList[(int)TAntiPlugActionMode.amTurnToSpell].boEnabled)   // :8367
                {
                    // :8369 原文注释同上
                    dwTempInterval = SpellSpeedInterval(TAntiPlugActionMode.amTurnToSpell);   // :8370-8375

                    if (SpellFamilySpeedBlock(TAntiPlugActionMode.amTurnToSpell,
                            TAntiPlugActionMode.amTurn, dwTempInterval, 52,
                            Msg, dwCurTick, ref sSendMsg, ref nDelayTime, ref AntiPlugAction, ref ErrorCode,
                            ref dwCurrentInterval))
                    {
                        return true;                                    // 不可达
                    }
                }
            }

            // 挖肉到魔法
            else if (FLastAction == TBaseAction.baCutMeat)                   // :8541
            {
                if (btJob != 0 && g_Config.ActionList[(int)TAntiPlugActionMode.amCutMeatToSpell].boEnabled)   // :8543
                {
                    // :8545 原文注释同上
                    dwTempInterval = SpellSpeedInterval(TAntiPlugActionMode.amCutMeatToSpell);   // :8546-8551

                    if (SpellFamilySpeedBlock(TAntiPlugActionMode.amCutMeatToSpell,
                            TAntiPlugActionMode.amCutMeat, dwTempInterval, 53,
                            Msg, dwCurTick, ref sSendMsg, ref nDelayTime, ref AntiPlugAction, ref ErrorCode,
                            ref dwCurrentInterval))
                    {
                        return true;                                    // 不可达
                    }
                }
            }

            // 移到下面并去掉 (FLastAction = baSpell) and 是因为边施魔法边吃药的时候，加速检测不到 chongchong 2016-10-06
            else if (btJob != 0 && /*{(FLastAction = baSpell) and}*/ SpellAction.boEnabled)   // :8717
            {
                // :8719-8724
                dwTempInterval = SpellSpeedInterval(TAntiPlugActionMode.amSpell);

                dwCurrentInterval = RunGateTiming.TickDiff(GameSpeed.dwTicks[AmSpell], dwCurTick);   // :8726

                // 注意 :8728 是**赋值**（不是 `if`）—— 会把已为 True 的 IsDropConcurrent 改回 False（D-P2）
                IsDropConcurrent = dwCurrentInterval <= dwTempInterval / DROP_CONCURRENT_RATE;   // :8728
                boCurrentSpeed = dwCurrentInterval < dwTempInterval;        // :8729

                // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
                boContinueSpeedPass = GameSpeed.boContinueSpeed &&          // :8732
                    (RunGateTiming.TickDiff(GameSpeed.dwStartSpeedTick, MyGetTickCount()) >=
                        dwTempInterval + g_Config.dwContinueSpeedPassIncTime);   // :8733

                if (IsDropConcurrent && !boContinueSpeedPass)               // :8735
                {
                    // { :8737-8744 原文整段被注释：`if g_Config.boShowDropConcurrentLog` 的【丢弃并发】日志 }

                    SendActionRet(true);                                    // :8746

                    // 魔法假刀要特殊对待 (差一个SM_MAGICFIRE包，手举起来半天不放下去) chongchong
                    TDefaultMessage SendDefMsg = MakeDefaultMsg(SM_MAGICFIRE_FAIL, nRecogId, 0, 0, 0);   // :8749
                    // 原文 :8750 `sSendMsg := EncodeRunGateMsg(@SendDefMsg, nil, 0)` + :8751 `PostSendText(sSendMsg)`
                    // —— C# 侧 sSendMsg 是 string、编码结果是 byte[]，故用局部 byte[] 承载（偏差 D3 的同一处置）
                    byte[] sDropConcurrentMsg = EncodeRunGateMsg(SendDefMsg, null, 0);
                    PostSendTextBytes(sDropConcurrentMsg);                  // :8751

                    Result = true;                                          // :8753（D-P1：唯一的 Exit 带 True）
                    return true;                                            // :8754 Exit
                }

                nSpeedCount = 0;                                            // :8757
                boCollectSpeed = false;                                     // :8758

                if (SumSpeedProcessArr[AmSpell, 0] == 0)                    // :8760
                    SumSpeedProcessArr[AmSpell, 0] = MyGetTickCount();      // :8761

                if (g_Config.dwCollectCount /*{g_Config.ActionList[amSpell].nCollectCount}*/ >= 2)   // :8763
                {
                    nCollectIndex = nCollectIntervalIndexArr[AmSpell];      // :8765
                    nCollectCount = g_Config.dwCollectCount /*{g_Config.ActionList[amSpell].nCollectCount}*/;   // :8766

                    // 倒数第2条数据采集到，加本次就是最一条搞定
                    if (dwCollectIntervalArr[AmSpell, nCollectCount - 2] != 0)   // :8769
                    {
                        // 本次和上次都超速就算超速 chongchong 2016-10-07
                        if (g_Config.boContinueSpeedCloseSocket && boCurrentSpeed)   // :8772
                        {
                            boContinueSpeed = true;                         // :8774
                            for (I = 1; I <= g_Config.nContinueSpeedCount; I++)   // :8775
                            {
                                if (dwCollectIntervalArr[AmSpell,
                                        (nCollectIndex - I + nCollectCount) % nCollectCount] >= 0)   // :8777
                                {
                                    boContinueSpeed = false;                // :8779
                                    break;                                  // :8780 Break
                                }
                            }

                            // 连续三次超速直接断开
                            if (boContinueSpeed)                        // :8785
                            {
                                ContinuousSpeed(TAntiPlugActionMode.amSpell, dwCurrentInterval);   // :8787
                                return true;                            // :8788 Exit
                            }
                        }

                        for (I = 0; I <= nCollectCount - 1; I++)        // :8792
                        {
                            if (I != nCollectIndex && dwCollectIntervalArr[AmSpell, I] < 0)   // :8794
                                nSpeedCount++;                          // :8795 Inc
                        }
                        if (boCurrentSpeed) nSpeedCount++;              // :8797 Inc
                        boCollectSpeed = nSpeedCount >= g_Config.dwSpeedValue;   // :8798  // g_Config.ActionList[amSpell].nCollectSpeedCount;
                    }
                    else
                    {
                        // 至少采集了1条
                        if (nCollectIndex >= 1)                         // :8803
                        {
                            // 本次和上次都超速就算超速 chongchong 2016-10-07
                            if (g_Config.boContinueSpeedCloseSocket && boCurrentSpeed)   // :8806
                            {
                                if (nCollectIndex >= g_Config.nContinueSpeedCount)      // :8808
                                {
                                    boContinueSpeed = true;             // :8810
                                    for (I = 1; I <= g_Config.nContinueSpeedCount; I++)   // :8811
                                    {
                                        if (dwCollectIntervalArr[AmSpell, nCollectIndex - I] >= 0)   // :8813
                                        {
                                            boContinueSpeed = false;    // :8815
                                            break;                      // :8816 Break
                                        }
                                    }

                                    // 连续三次超速直接断开
                                    if (boContinueSpeed)            // :8821
                                    {
                                        ContinuousSpeed(TAntiPlugActionMode.amSpell, dwCurrentInterval);   // :8823
                                        return true;                // :8824 Exit
                                    }
                                }
                            }

                            for (I = 0; I <= nCollectIndex - 1; I++)    // :8829
                            {
                                if (dwCollectIntervalArr[AmSpell, I] < 0)   // :8831
                                    nSpeedCount++;                      // :8832 Inc
                            }
                            if (boCurrentSpeed) nSpeedCount++;          // :8834 Inc

                            if (dwCurrentInterval <= dwTempInterval / 3)    // :8836
                            {
                                boCollectSpeed = true;                  // :8838
                            }
                            else
                            {
                                if (nCollectIndex + 1 <= 3)             // :8842
                                    boCollectSpeed = nSpeedCount >= 2;                  // :8843
                                else if (nCollectIndex + 1 <= 7)        // :8844
                                {
                                    boCollectSpeed = nSpeedCount >= (nCollectIndex + 1) / 2;   // :8846
                                }
                                else
                                {
                                    boCollectSpeed = nSpeedCount >= (nCollectIndex + 1) / 2 - 1;   // :8850
                                }
                            }
                        }
                        // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                        else if (dwCurrentInterval <= dwTempInterval / 3)   // :8855
                        {
                            boCollectSpeed = true;                      // :8857
                        }
                    }
                }
                else if (dwCurrentInterval <= dwTempInterval / 3)       // :8861
                {
                    boCollectSpeed = true;                              // :8863
                }

                if ((dwCurrentInterval <= dwTempInterval / 10) ||       // :8866
                    ((!boContinueSpeedPass) && boCurrentSpeed && boCollectSpeed))   // :8867
                {
                    if (SpellAction.boShowHint)                         // :8869
                        sSendMsg = SpellAction.sHintText;               // :8870

                    AntiPlugAction = SpellAction;                       // :8872
                    LastLockAntiPlugActionMode = TAntiPlugActionMode.amSpell;   // :8873

                    if (RunGateTiming.TickDiff(SumSpeedProcessArr[AmSpell, 0],
                            MyGetTickCount()) <= g_Config.nSumSpeedCheckTime * 1000)   // :8875
                        SumSpeedProcessArr[AmSpell, 1]++;               // :8876 Inc
                    else
                    {
                        SumSpeedProcessArr[AmSpell, 1] = 1;             // :8879
                        SumSpeedProcessArr[AmSpell, 0] = MyGetTickCount();   // :8880
                    }

                    if (AntiPlugAction.ProcessMode == TActionProcessMode.apmDelay &&
                        dwCurrentInterval < dwTempInterval)             // :8883
                    {
                        nDelayTime = (int)(dwTempInterval - dwCurrentInterval) + DELAY_TIME_ADD;   // :8885

                        // 修正加速一段时间后恢复到正常状态，一直提示加速
                        GameSpeed.nDelayCount[AmSpell] = GameSpeed.nDelayCount[AmSpell] + 1;   // :8888
                        if (GameSpeed.nDelayCount[AmSpell] > 8)         // :8889
                        {
                            GameSpeed.nDelayCount[AmSpell] = 0;         // :8891
                            nDelayTime = 0;                             // :8892
                            SendActionRet(true);                        // :8893
                        }
                    }

                    if (g_Config.boShowAttackLog)                       // :8897
                    {
                        ErrorCode = 55;                                 // :8899
                        AddMainLogMsg(Format("【用户超速】%s:%d; [魔法速度%s]; 用户:%s",   // :8900-8904
                            AntiPlugActionModeNames3[AmSpell], dwCurrentInterval,
                            GetSpeedText(nSpellSpeed), sChrName), 0);
                    }
                }
                else
                {
                    if (!Msg.boDelay && !boContinueSpeedPass)           // :8909
                    {
                        GameSpeed.nDelayCount[AmSpell] = 0;             // :8911
                    }
                }

                if (nDelayTime == 0 && !Msg.boDelay)                    // :8915
                {
                    nCollectIndex = nCollectIntervalIndexArr[AmSpell];  // :8917

                    if (dwCurrentInterval >= dwTempInterval)            // :8919
                        dwCollectIntervalArr[AmSpell, nCollectIndex] = 1;       // :8920
                    else
                        dwCollectIntervalArr[AmSpell, nCollectIndex] =
                            unchecked((int)(dwCurrentInterval - dwTempInterval));   // :8922

                    nCollectIntervalIndexArr[AmSpell] = (nCollectIndex + 1)
                        % g_Config.dwCollectCount /*{g_Config.ActionList[amSpell].nCollectCount}*/;   // :8924
                }
            }

            // :8928-8980 调试日志（**在 `if AntiPlugAction = nil` 之内**）
            if (!Msg.boDelay)                                           // :8928
            {
                if (FLastAction == TBaseAction.baWalk)                  // :8930
                {
                    if (g_Config.ActionList[(int)TAntiPlugActionMode.amWalkToSpell].boDebug)   // :8932
                    {
                        ErrorCode = 56;                                 // :8934
                        AddMainLogMsg(Format("%s:%d; 用户:%s",           // :8935-8936
                            AntiPlugActionModeNames3[(int)TAntiPlugActionMode.amWalkToSpell],
                            RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)TAntiPlugActionMode.amWalk], dwCurTick),
                            sChrName), 0);
                    }
                }

                else if (FLastAction == TBaseAction.baRun)              // :8940
                {
                    if (g_Config.ActionList[(int)TAntiPlugActionMode.amRunToSpell].boDebug)   // :8942
                    {
                        ErrorCode = 57;                                 // :8944
                        AddMainLogMsg(Format("%s:%d; 用户:%s",           // :8945-8946
                            AntiPlugActionModeNames3[(int)TAntiPlugActionMode.amRunToSpell],
                            RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)TAntiPlugActionMode.amRun], dwCurTick),
                            sChrName), 0);
                    }
                }

                else if (FLastAction == TBaseAction.baTurn)             // :8950
                {
                    if (g_Config.ActionList[(int)TAntiPlugActionMode.amTurnToSpell].boDebug)   // :8952
                    {
                        ErrorCode = 58;                                 // :8954
                        AddMainLogMsg(Format("%s:%d; 用户:%s",           // :8955-8956
                            AntiPlugActionModeNames3[(int)TAntiPlugActionMode.amTurnToSpell],
                            RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)TAntiPlugActionMode.amTurn], dwCurTick),
                            sChrName), 0);
                    }
                }

                else if (FLastAction == TBaseAction.baCutMeat)          // :8960
                {
                    if (g_Config.ActionList[(int)TAntiPlugActionMode.amCutMeatToSpell].boDebug)   // :8962
                    {
                        ErrorCode = 59;                                 // :8964
                        AddMainLogMsg(Format("%s:%d; 用户:%s",           // :8965-8966
                            AntiPlugActionModeNames3[(int)TAntiPlugActionMode.amCutMeatToSpell],
                            RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)TAntiPlugActionMode.amCutMeat], dwCurTick),
                            sChrName), 0);
                    }
                }

                // :8970 原文 `else if {(FLastAction = baSpell) and} …amSpell.boDebug then`（D-P5）
                else if (/*{(FLastAction = baSpell) and}*/ SpellAction.boDebug)
                {
                    ErrorCode = 60;                                     // :8972
                    AddMainLogMsg(Format("%s:%d; [魔法速度%s]; 用户:%s",  // :8973-8974
                        AntiPlugActionModeNames3[AmSpell],
                        RunGateTiming.TickDiff(GameSpeed.dwTicks[AmSpell], dwCurTick),
                        GetSpeedText(nSpellSpeed), sChrName), 0);
                }

                ErrorCode = 61;                                         // :8977（**无条件**，覆盖 56-60，D-P3）
                if (SpellConcurrentAction.boDebug && ConcurrentCount > 0)   // :8978
                    AddMainLogMsg(Format("%s:%d; 用户:%s",               // :8979
                        AntiPlugActionModeNames3[AmSpellConcurrent], ConcurrentCount + 1, sChrName), 0);
            }
        }

        if (nDelayTime == 0 /*{and (not Msg.boDelay)}*/)                // :8983
        {
            //if FLastAction = baSpell then                             // :8985 原文如此（已注释）
            {
                GameSpeed.dwTicks[AmSpell] = dwCurTick;                 // :8987
            }

            GameSpeed.dwTicks[AmSpellToWalk] = dwCurTick;               // :8990
            GameSpeed.dwTicks[AmSpellToRun] = dwCurTick;                // :8991
        }

        FLastAction = TBaseAction.baSpell;                              // :8994

        return false;   // 原文 :8995 分支体结束 → 继续进入公共收尾 9521-9681
    }

    /// <summary>
    /// 原文 :7993-7998 / :8018-8023 / :8194-8199 / :8370-8375 / :8546-8551 / :8719-8724 ——
    /// 按 `nSpellSpeed` 从 `g_wActionSpeedIntervals[mode]` 取加速间隔（三支，闭区间边界见 D-注释）。
    /// <para>
    /// `HALF_SPEED_INTERVALS_COUNT = 200`、`SPEED_INTERVALS_COUNT = 401`（GateShare.pas:25-26）。
    /// `nSpellSpeed &lt;= -200` 取下限槽 0；`&gt;= +200` 取上限槽 400；否则取 `200 + nSpellSpeed`
    /// ∈ [1,399]（**注意 -199..-1 与 1..199 共用同一张表，正负各占一半**）。
    /// </para>
    /// </summary>
    private uint SpellSpeedInterval(TAntiPlugActionMode mode)
    {
        if (nSpellSpeed <= -HalfSpeedIntervalsCount)                        // :7993
            return g_wActionSpeedIntervals[(int)mode][0];                   // :7994
        else if (nSpellSpeed >= HalfSpeedIntervalsCount)                    // :7995
            return g_wActionSpeedIntervals[(int)mode][SpeedIntervalsCount - 1];   // :7996
        else
            return g_wActionSpeedIntervals[(int)mode][HalfSpeedIntervalsCount + nSpellSpeed];   // :7998
    }

    /// <summary>
    /// CM_SPELL 的四个“X到魔法”子块（:8013-8185 走路 / :8189-8362 跑步 / :8365-8538 转向 /
    /// :8541-8714 挖肉）—— 与 CM_TURN 的子块体**逐字相同**（脚本已核对），
    /// 只有 `dwTempInterval` 的来源不同（调用点用 <see cref="SpellSpeedInterval"/> 预先算好传入）：
    /// 四个子块的“连续超速断开”段都被 `{}` 注释掉 → <c>hasContinueSpeedBlock = false</c>、
    /// 日志都不带速度段 → <see cref="SpeedLogKind.None"/>。
    /// </summary>
    private bool SpellFamilySpeedBlock(TAntiPlugActionMode intervalMode, TAntiPlugActionMode tickMode,
        uint dwTempInterval, int errorCode,
        TProcessMsg Msg, uint dwCurTick, ref string sSendMsg, ref int nDelayTime,
        ref TAntiPlugAction AntiPlugAction, ref int ErrorCode, ref uint dwCurrentInterval)
        => CollectSpeedDetect(intervalMode, tickMode, dwTempInterval, errorCode, SpeedLogKind.None,
            hasContinueSpeedBlock: false, default,   // CM_SPELL 的四个子块也没有阶段标记
            dwCurTick, Msg, ref sSendMsg, ref nDelayTime,
            ref AntiPlugAction, ref ErrorCode, ref dwCurrentInterval);

    /// <summary>
    /// 原文 `RecordActionArr[…].Action in [baTurn, baCutMeat]`（:7906 / :7917 / :7936 / :7947）——
    /// CM_SPELL 的暗杀前导动作集合。**三族的集合互不相同**（见各 <c>Is*AssasinatePreAction</c> 的注释）。
    /// </summary>
    private static bool IsSpellAssasinatePreAction(TBaseAction action) =>
        action == TBaseAction.baTurn || action == TBaseAction.baCutMeat;

    // =================================================================================
    // 原文 :4694-5853  `else if DefMsg.Ident = CM_RUN then`（**跑步**）分支体
    //
    // ★ 骨架（与 CM_WALK 是"同构但不同"的一对，差异见 §注释 D-W/D-R）
    //   :4700      ErrorCode := 3;
    //   :4701-4706 环形缓冲写入 baRun + Inc（**没有暗杀检测**）
    //   :4708      ErrorCode := 301;  :4710 nCompensationValue := 0;
    //   :4712-4759 **移动并发**块（amMoveConcurrent；命中后若 FLastAction = baRun 再按
    //              `nMoveSpeed` 判 IsDropConcurrent）
    //   :4761-5837 `if AntiPlugAction = nil then` 包住的五个 FLastAction 子块 + 调试日志
    //   :5841-5850 nDelayTime = 0 → OldLastRunTick := dwTicks[amRun]; dwTicks[amRun] 刷新 +
    //              dwTicks[amRunToHit] / dwTicks[amRunToSpell]（**三个槽 + 一个"旧值"槽**）
    //   :5852      FLastAction := baRun;
    //
    // ★ 五个子块
    //   a :4767  baHit     → amHitToRun      取 dwTicks[amHit]   （**无前置 ErrorCode**）
    //   b :4947  baSpell   → amSpellToRun    取 dwTicks[amSpell]  （**唯一有 `btJob <> 0` 守卫的**）
    //   c :5123  baTurn    → **amTurnToMove**（不是 amTurnToRun；原文如此）取 dwTicks[amTurn]
    //   d :5306  baCutMeat → **amCutMeatToMove**（不是 amCutMeatToRun）取 dwTicks[amCutMeat]
    //   e :5488  `{(FLastAction = baRun) and} amRun.boEnabled` → amRun（**带补偿池**）
    //   前四个（a-d）与 CM_WALK 的对应子块**逐字同构**（仅 mode/tick/ErrorCode 不同），
    //   第五个（e）含"丢弃并发 + 补偿池"，与 CM_WALK 的同名块**有实质差异**（见 D-R3/D-R4）。
    //
    // ★ 原文缺陷 / 易错点（照抄）
    //   D-R1 :4712-4715 并发计数在**重置 nCompensationValue 之后**（WALK 是之前）—— 顺序差异照抄。
    //   D-R2 :4742/:5488 并发块的内部判据是 `FLastAction = baRun`，而 :5488 的 `FLastAction = baRun`
    //        被 `{}` 注释掉 → 该子块对**任何** FLastAction（含 baOther/baCutMeat/baSpell）都成立。
    //   D-R3 :5589 `if not boCollectSpeed then begin … end` —— RUN 的 amRun 子块把整段采集
    //        **包在 `not boCollectSpeed` 里**（此时 boCollectSpeed 刚被 :5581 置 False，故恒真；
    //        原文如此）；CM_WALK 的 amWalk 子块**没有**这层包裹。
    //   D-R4 :5746 的 `ErrorCode := 323;` 与 :5694 的 323 **重复**（WALK 处是 225 → 226）。
    //   D-R5 :4767/:5306/:5126 的子块守卫**没有** `btJob <> 0`（只有 :4950 的 baSpell 有）。
    //   D-R6 :5845 `GameSpeed.OldLastRunTick := GameSpeed.dwTicks[amRun];` —— 先存旧值再覆盖（WALK 同构）。
    //   D-R7 :5841 的 `and (not Msg.boDelay)` 被 `{}` 注释 → 只判 `nDelayTime = 0`（同 D-S3/D-T3）。
    // =================================================================================

    /// <summary>
    /// 原文 :3575-3580 / :4334-4339 / :4745-4750 等 —— 按 `nMoveSpeed` 从
    /// `g_wActionSpeedIntervals[mode]` 取加速间隔（与 <see cref="SpellSpeedInterval"/> 同构）。
    /// </summary>
    private uint MoveSpeedInterval(TAntiPlugActionMode mode)
    {
        if (nMoveSpeed <= -HalfSpeedIntervalsCount)
            return g_wActionSpeedIntervals[(int)mode][0];
        else if (nMoveSpeed >= HalfSpeedIntervalsCount)
            return g_wActionSpeedIntervals[(int)mode][SpeedIntervalsCount - 1];
        else
            return g_wActionSpeedIntervals[(int)mode][HalfSpeedIntervalsCount + nMoveSpeed];
    }

    /// <summary>
    /// CM_RUN 的四个“X到跑步”子块（:4767 攻击 / :4947 魔法 / :5123 转向 / :5306 挖肉）——
    /// 与 CM_TURN/CM_SPELL 的子块同源（共用 <see cref="CollectSpeedDetect"/>），
    /// 差异只有：`dwTempInterval` 由 <see cref="MoveSpeedInterval"/> 预先算好传入、
    /// 阶段 `ErrorCode` 标记（<see cref="SpeedStageCodes"/>）、日志都不带速度段。
    /// </summary>
    private bool RunFamilySpeedBlock(TAntiPlugActionMode intervalMode, TAntiPlugActionMode tickMode,
        uint dwTempInterval, int errorCode, SpeedStageCodes stages,
        TProcessMsg Msg, uint dwCurTick, ref string sSendMsg, ref int nDelayTime,
        ref TAntiPlugAction AntiPlugAction, ref int ErrorCode, ref uint dwCurrentInterval)
        => CollectSpeedDetect(intervalMode, tickMode, dwTempInterval, errorCode, SpeedLogKind.None,
            hasContinueSpeedBlock: false, stages, dwCurTick, Msg, ref sSendMsg, ref nDelayTime,
            ref AntiPlugAction, ref ErrorCode, ref dwCurrentInterval);

    /// <summary>
    /// 原文 :4699-5853 —— <c>CM_RUN</c>（跑步）分支体，1:1 移植。
    /// 返回 <c>true</c> 表示原文执行了 <c>Exit</c>（:5526 丢弃并发、:5616/:5651 连续超速）。
    /// </summary>
    private bool CheckUsePluginRun(TProcessMsg Msg, in TDefaultMessage DefMsg, uint dwCurTick,
        ref string sSendMsg, ref int nDelayTime, ref TAntiPlugAction AntiPlugAction, ref int ErrorCode,
        ref uint dwCurrentInterval, ref int ConcurrentCount, ref bool IsDropConcurrent,
        ref int nCompensationValue, ref bool Result)
    {
        // ---- 原文 :3474-3502 中被本分支独占/使用的局部量（同名同类型）----
        uint dwTempInterval;                                                // :3476
        bool boCurrentSpeed, boContinueSpeed = false, boCollectSpeed;       // :3486
        bool boContinueSpeedPass;                                           // :3487
        int nSpeedCount;                                                    // :3481
        int I, nCollectIndex, nCollectCount;                                // :3483

        const int AmRun = (int)TAntiPlugActionMode.amRun;                              // 3
        const int AmRunToHit = (int)TAntiPlugActionMode.amRunToHit;                    // 8
        const int AmRunToSpell = (int)TAntiPlugActionMode.amRunToSpell;                // 12
        const int AmMoveConcurrent = (int)TAntiPlugActionMode.amMoveConcurrent;        // 26
        TAntiPlugAction MoveConcurrentAction = g_Config.ActionList[AmMoveConcurrent];

        ErrorCode = 3;                                                      // :4700
        if (nRecordActionIndex >= MirClientContextConst.MAX_RECORD_ACTION_COUNT || nRecordActionIndex < 0)
            nRecordActionIndex = 0;                                         // :4702
        RecordActionArr[nRecordActionIndex].Action = TBaseAction.baRun;                     // :4703
        RecordActionArr[nRecordActionIndex].Tick = MyGetTickCount();                        // :4704
        RecordActionArr[nRecordActionIndex].DefMsg = DefMsg;                                // :4705
        nRecordActionIndex++;                                                               // :4706 Inc

        ErrorCode = 301;                                                    // :4708

        nCompensationValue = 0;                                             // :4710

        // 移动并发 chongchong 2014-12-16
        ConcurrentCount = 0;                                                // :4713
        if (MoveConcurrentAction.boEnabled || MoveConcurrentAction.boDebug)  // :4714
            ConcurrentCount = GetConcurrentPacketCount(DefMsg);             // :4715

        if (MoveConcurrentAction.boEnabled)                                 // :4717
        {
            ErrorCode = 302;                                                // :4719

            if (SumSpeedProcessArr[AmMoveConcurrent, 0] == 0)               // :4721
                SumSpeedProcessArr[AmMoveConcurrent, 0] = MyGetTickCount();  // :4722

            dwTempInterval = MoveConcurrentAction.nInterval;                // :4724

            if (ConcurrentCount >= dwTempInterval)                          // :4726
            {
                if (MoveConcurrentAction.boShowHint)                        // :4728
                    sSendMsg = MoveConcurrentAction.sHintText;              // :4729

                AntiPlugAction = MoveConcurrentAction;                      // :4731
                LastLockAntiPlugActionMode = TAntiPlugActionMode.amMoveConcurrent;   // :4732

                if (RunGateTiming.TickDiff(SumSpeedProcessArr[AmMoveConcurrent, 0],
                        MyGetTickCount()) <= g_Config.nSumSpeedCheckTime * 1000)   // :4734
                    SumSpeedProcessArr[AmMoveConcurrent, 1]++;              // :4735 Inc
                else
                {
                    SumSpeedProcessArr[AmMoveConcurrent, 1] = 1;            // :4738
                    SumSpeedProcessArr[AmMoveConcurrent, 0] = MyGetTickCount();   // :4739
                }

                if (FLastAction == TBaseAction.baRun)                       // :4742
                {
                    // 走路间隔随移动速度+而改变 chongchong 2015-06-01
                    dwTempInterval = MoveSpeedInterval(TAntiPlugActionMode.amRun);   // :4745-4750

                    dwCurrentInterval = RunGateTiming.TickDiff(GameSpeed.dwTicks[AmRun], dwCurTick);   // :4752
                    if (dwCurrentInterval <= dwTempInterval / DROP_CONCURRENT_RATE)   // :4753
                    {
                        IsDropConcurrent = true;                            // :4755
                    }
                }
            }
        }

        ErrorCode = 303;                                                    // :4761
        if (AntiPlugAction == null)                                         // :4762
        {
            ErrorCode = 304;                                                // :4764

            // 攻击到跑步
            if (FLastAction == TBaseAction.baHit)                           // :4767
            {
                // 注意：本子块**没有**前置 `ErrorCode`（与 CM_WALK 的 :3600 `ErrorCode := 204` 不同）
                if (g_Config.ActionList[(int)TAntiPlugActionMode.amHitToRun].boEnabled)   // :4769
                {
                    // :4771 `//dwTempInterval := g_Config.ActionList[amHitToRun].nInterval;`（原文已注释）
                    dwTempInterval = MoveSpeedInterval(TAntiPlugActionMode.amHitToRun);   // :4773-4777

                    if (RunFamilySpeedBlock(TAntiPlugActionMode.amHitToRun, TAntiPlugActionMode.amHit,
                            dwTempInterval, 15,
                            new SpeedStageCodes(afterCollect: 305, afterPass: 306, beforeWrite: 307),
                            Msg, dwCurTick, ref sSendMsg, ref nDelayTime, ref AntiPlugAction, ref ErrorCode,
                            ref dwCurrentInterval))
                    {
                        return true;                                        // 该子块的 Exit 均在 `{}` 注释内（不可达）
                    }
                }
            }

            // 魔法到跑步
            else if (FLastAction == TBaseAction.baSpell)                    // :4947
            {
                ErrorCode = 308;                                            // :4949

                if (btJob != 0 && g_Config.ActionList[(int)TAntiPlugActionMode.amSpellToRun].boEnabled)   // :4950
                {
                    // :4952 原文注释同上
                    dwTempInterval = MoveSpeedInterval(TAntiPlugActionMode.amSpellToRun);   // :4954-4958

                    // 子块 b 没有任何阶段标记（:4949 的 308 已在守卫前）
                    if (RunFamilySpeedBlock(TAntiPlugActionMode.amSpellToRun, TAntiPlugActionMode.amSpell,
                            dwTempInterval, 16, default,
                            Msg, dwCurTick, ref sSendMsg, ref nDelayTime, ref AntiPlugAction, ref ErrorCode,
                            ref dwCurrentInterval))
                    {
                        return true;                                        // 不可达
                    }
                }
            }

            // 转向到跑步（**mode 是 amTurnToMove**，与 CM_WALK 的同名子块相同）
            else if (FLastAction == TBaseAction.baTurn)                     // :5123
            {
                ErrorCode = 309;                                            // :5125

                if (g_Config.ActionList[(int)TAntiPlugActionMode.amTurnToMove].boEnabled)   // :5126
                {
                    // :5128 原文注释同上
                    dwTempInterval = MoveSpeedInterval(TAntiPlugActionMode.amTurnToMove);   // :5130-5134

                    if (RunFamilySpeedBlock(TAntiPlugActionMode.amTurnToMove, TAntiPlugActionMode.amTurn,
                            dwTempInterval, 17,
                            new SpeedStageCodes(afterSum: 310, afterCollect: 311, afterPass: 312, beforeWrite: 313),
                            Msg, dwCurTick, ref sSendMsg, ref nDelayTime, ref AntiPlugAction, ref ErrorCode,
                            ref dwCurrentInterval))
                    {
                        return true;                                        // 不可达
                    }
                }
            }

            // 挖肉到跑步（**mode 是 amCutMeatToMove**）
            else if (FLastAction == TBaseAction.baCutMeat)                  // :5306
            {
                ErrorCode = 314;                                            // :5308

                if (g_Config.ActionList[(int)TAntiPlugActionMode.amCutMeatToMove].boEnabled)   // :5309
                {
                    // :5311 原文注释同上
                    dwTempInterval = MoveSpeedInterval(TAntiPlugActionMode.amCutMeatToMove);   // :5313-5317

                    if (RunFamilySpeedBlock(TAntiPlugActionMode.amCutMeatToMove, TAntiPlugActionMode.amCutMeat,
                            dwTempInterval, 18,
                            new SpeedStageCodes(afterCollect: 315, afterPass: 316, beforeWrite: 317),
                            Msg, dwCurTick, ref sSendMsg, ref nDelayTime, ref AntiPlugAction, ref ErrorCode,
                            ref dwCurrentInterval))
                    {
                        return true;                                        // 不可达
                    }
                }
            }

            // 移到下面并去掉 (FLastAction = baRun) and 是因为，边跑边吃药的时候，加速检测不到 chongchong 2016-10-06
            else if (/*{(FLastAction = baRun) and}*/ g_Config.ActionList[AmRun].boEnabled)   // :5488
            {
                ErrorCode = 318;                                            // :5490

                // 跑行间隔随移动速度+而改变 chongchong 2015-06-01
                dwTempInterval = MoveSpeedInterval(TAntiPlugActionMode.amRun);   // :5494-5498

                dwCurrentInterval = RunGateTiming.TickDiff(GameSpeed.dwTicks[AmRun], dwCurTick);   // :5500

                if (boChangeMap && dwCurrentInterval <= dwTempInterval + DELAY_TIME_ADD)   // :5501
                    dwCurrentInterval = dwTempInterval + DELAY_TIME_ADD;    // :5502

                IsDropConcurrent = dwCurrentInterval <= dwTempInterval / DROP_CONCURRENT_RATE;   // :5504
                boCurrentSpeed = dwCurrentInterval < dwTempInterval;        // :5505

                ErrorCode = 319;                                            // :5507

                // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会边续反弹 chongchong 2015-12-20
                boContinueSpeedPass = GameSpeed.boContinueSpeed &&          // :5510
                    (RunGateTiming.TickDiff(GameSpeed.dwStartSpeedTick, MyGetTickCount()) >=
                        dwTempInterval + g_Config.dwContinueSpeedPassIncTime);   // :5511

                if (IsDropConcurrent && !boContinueSpeedPass)               // :5513
                {
                    // { :5515-5522 原文整段被注释：`if g_Config.boShowDropConcurrentLog` 的【丢弃并发】日志（ErrorCode 19） }

                    SendActionRet(true);                                    // :5524
                    Result = true;                                          // :5525
                    return true;                                            // :5526 Exit
                }

                ErrorCode = 320;                                            // :5529

                // ---- 补偿池（:5531-5578）----
                if (g_Config.ActionList[AmRun].nCompensationValue > 0)      // :5531
                {
                    if (nCompensationArr[AmRun] > g_Config.ActionList[AmRun].nCompensationValue)   // :5533
                    {
                        nCompensationArr[AmRun] = g_Config.ActionList[AmRun].nCompensationValue;   // :5535
                    }

                    if (dwCurrentInterval > dwTempInterval / 3 &&
                        dwCurrentInterval < dwTempInterval * 2)             // :5538
                    {
                        nCompensationValue = unchecked((int)(dwCurrentInterval - dwTempInterval));   // :5540

                        if (nCompensationValue >= 0)                        // :5542
                        {
                            if (nCompensationValue >= 4)                    // :5544
                            {
                                nCompensationArr[AmRun] = Math.Min(nCompensationArr[AmRun] + nCompensationValue,
                                    g_Config.ActionList[AmRun].nCompensationValue);   // :5546 Delphi Min
                            }
                            else
                            {
                                nCompensationValue = 0;                     // :5550

                                if (g_Config.boZeroCompensationValueClearPool)   // :5552
                                {
                                    nCompensationArr[AmRun] = 0;            // :5554
                                }
                            }
                        }
                        else
                        {
                            if (nCompensationArr[AmRun] + nCompensationValue < 0)   // :5560
                            {
                                nCompensationValue = -nCompensationArr[AmRun];      // :5562
                                nCompensationArr[AmRun] = 0;                        // :5563
                            }
                            else
                            {
                                nCompensationArr[AmRun] = nCompensationArr[AmRun] + nCompensationValue;   // :5567
                            }

                            dwCurrentInterval = unchecked(dwCurrentInterval - (uint)nCompensationValue);   // :5570
                            boCurrentSpeed = dwCurrentInterval < dwTempInterval;    // :5571
                        }
                    }
                }
                else
                {
                    nCompensationArr[AmRun] = 0;                            // :5577
                }

                nSpeedCount = 0;                                            // :5580
                boCollectSpeed = false;                                     // :5581

                ErrorCode = 321;                                            // :5583

                if (SumSpeedProcessArr[AmRun, 0] == 0)                      // :5585
                    SumSpeedProcessArr[AmRun, 0] = MyGetTickCount();        // :5586

                ErrorCode = 322;                                            // :5588
                if (!boCollectSpeed)                                        // :5589（D-R3：CM_WALK 没有这层包裹）
                {
                    if (g_Config.dwCollectCount /*{g_Config.ActionList[amRun].nCollectCount}*/ >= 2)   // :5591
                    {
                        nCollectIndex = nCollectIntervalIndexArr[AmRun];     // :5593
                        nCollectCount = g_Config.dwCollectCount /*{g_Config.ActionList[amRun].nCollectCount}*/;   // :5594

                        // 倒数第2条数据采集到，加本次就是最一条搞定
                        if (dwCollectIntervalArr[AmRun, nCollectCount - 2] != 0)   // :5597
                        {
                            // 本次和上次都超速就算超速 chongchong 2016-10-07
                            if (g_Config.boContinueSpeedCloseSocket && boCurrentSpeed)   // :5600
                            {
                                boContinueSpeed = true;                      // :5602
                                for (I = 1; I <= g_Config.nContinueSpeedCount; I++)   // :5603
                                {
                                    if (dwCollectIntervalArr[AmRun,
                                            (nCollectIndex - I + nCollectCount) % nCollectCount] >= 0)   // :5605
                                    {
                                        boContinueSpeed = false;             // :5607
                                        break;                               // :5608 Break
                                    }
                                }

                                // 连续三次超速直接断开
                                if (boContinueSpeed)                         // :5613
                                {
                                    ContinuousSpeed(TAntiPlugActionMode.amRun, dwCurrentInterval);   // :5615
                                    return true;                             // :5616 Exit
                                }
                            }

                            for (I = 0; I <= nCollectCount - 1; I++)         // :5620
                            {
                                if (I != nCollectIndex && dwCollectIntervalArr[AmRun, I] < 0)   // :5622
                                    nSpeedCount++;                           // :5623 Inc
                            }
                            if (boCurrentSpeed) nSpeedCount++;               // :5625 Inc
                            boCollectSpeed = nSpeedCount >= g_Config.dwSpeedValue;   // :5626  // g_Config.ActionList[amRun].nCollectSpeedCount;
                        }
                        else
                        {
                            // 至少采集了1条
                            if (nCollectIndex >= 1)                          // :5631
                            {
                                // 本次和上次都超速就算超速 chongchong 2016-10-07
                                if (g_Config.boContinueSpeedCloseSocket && boCurrentSpeed)   // :5633
                                {
                                    if (nCollectIndex >= g_Config.nContinueSpeedCount)   // :5635
                                    {
                                        boContinueSpeed = true;              // :5637
                                        for (I = 1; I <= g_Config.nContinueSpeedCount; I++)   // :5638
                                        {
                                            if (dwCollectIntervalArr[AmRun, nCollectIndex - I] >= 0)   // :5640
                                            {
                                                boContinueSpeed = false;     // :5642
                                                break;                       // :5643 Break
                                            }
                                        }

                                        // 连续三次超速直接断开
                                        if (boContinueSpeed)             // :5648
                                        {
                                            ContinuousSpeed(TAntiPlugActionMode.amRun, dwCurrentInterval);   // :5650
                                            return true;                 // :5651 Exit
                                        }
                                    }
                                }

                                for (I = 0; I <= nCollectIndex - 1; I++)     // :5656
                                {
                                    if (dwCollectIntervalArr[AmRun, I] < 0)  // :5658
                                        nSpeedCount++;                       // :5659 Inc
                                }
                                if (boCurrentSpeed) nSpeedCount++;           // :5661 Inc

                                if (dwCurrentInterval <= dwTempInterval / 3)   // :5663
                                {
                                    boCollectSpeed = true;                   // :5665
                                }
                                else
                                {
                                    if (nCollectIndex + 1 <= 3)              // :5669
                                        boCollectSpeed = nSpeedCount >= 2;                  // :5670
                                    else if (nCollectIndex + 1 <= 7)         // :5671
                                    {
                                        boCollectSpeed = nSpeedCount >= (nCollectIndex + 1) / 2;   // :5673
                                    }
                                    else
                                    {
                                        boCollectSpeed = nSpeedCount >= (nCollectIndex + 1) / 2 - 1;   // :5677
                                    }
                                }
                            }
                            // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                            else if (dwCurrentInterval <= dwTempInterval / 3)   // :5682
                            {
                                boCollectSpeed = true;                       // :5684
                            }
                        }
                    }
                    else if (dwCurrentInterval <= dwTempInterval / 3)        // :5688
                    {
                        boCollectSpeed = true;                               // :5690
                    }
                }

                ErrorCode = 323;                                            // :5694
                if (dwCurrentInterval <= dwTempInterval / 10 ||              // :5695
                    (!boContinueSpeedPass && boCurrentSpeed && boCollectSpeed))   // :5696
                {
                    if (g_Config.ActionList[AmRun].boShowHint)               // :5698
                        sSendMsg = g_Config.ActionList[AmRun].sHintText;     // :5699

                    ErrorCode = 324;                                        // :5701

                    AntiPlugAction = g_Config.ActionList[AmRun];             // :5703
                    LastLockAntiPlugActionMode = TAntiPlugActionMode.amRun;  // :5704

                    if (RunGateTiming.TickDiff(SumSpeedProcessArr[AmRun, 0],
                            MyGetTickCount()) <= g_Config.nSumSpeedCheckTime * 1000)   // :5706
                        SumSpeedProcessArr[AmRun, 1]++;                      // :5707 Inc
                    else
                    {
                        SumSpeedProcessArr[AmRun, 1] = 1;                    // :5710
                        SumSpeedProcessArr[AmRun, 0] = MyGetTickCount();     // :5711
                    }

                    if (AntiPlugAction.ProcessMode == TActionProcessMode.apmDelay &&
                        dwCurrentInterval < dwTempInterval)                  // :5714
                    {
                        nDelayTime = (int)(dwTempInterval - dwCurrentInterval) + DELAY_TIME_ADD;   // :5716

                        // 修正加速一段时间后恢复到正常状态，一直提示加速
                        GameSpeed.nDelayCount[AmRun] = GameSpeed.nDelayCount[AmRun] + 1;   // :5719
                        if (GameSpeed.nDelayCount[AmRun] > 8)                // :5720
                        {
                            GameSpeed.nDelayCount[AmRun] = 0;                // :5722
                            nDelayTime = 0;                                  // :5723
                            SendActionRet(true);                             // :5724
                        }
                    }

                    if (g_Config.boShowAttackLog)                            // :5728
                    {
                        ErrorCode = 20;                                      // :5730
                        AddMainLogMsg(Format("【用户超速】%s:%d; [移动速度%s]; 用户:%s",   // :5731-5735
                            AntiPlugActionModeNames3[AmRun], dwCurrentInterval,
                            GetSpeedText(nMoveSpeed), sChrName), 0);
                    }
                }
                else
                {
                    if (!Msg.boDelay && !boContinueSpeedPass)                // :5740
                    {
                        GameSpeed.nDelayCount[AmRun] = 0;                    // :5742
                    }
                }

                ErrorCode = 323;   // :5746（**与 :5694 重复** —— 原文缺陷 D-R4）
                if (nDelayTime == 0 && !Msg.boDelay)                         // :5747
                {
                    nCollectIndex = nCollectIntervalIndexArr[AmRun];          // :5749

                    if (dwCurrentInterval >= dwTempInterval)                  // :5751
                        dwCollectIntervalArr[AmRun, nCollectIndex] = 1;       // :5752
                    else
                        dwCollectIntervalArr[AmRun, nCollectIndex] =
                            unchecked((int)(dwCurrentInterval - dwTempInterval));   // :5754

                    nCollectIntervalIndexArr[AmRun] = (nCollectIndex + 1)
                        % g_Config.dwCollectCount /*{g_Config.ActionList[amRun].nCollectCount}*/;   // :5756
                }
            }

            // :5760-5837 调试日志
            if (!Msg.boDelay)                                               // :5760
            {
                if (FLastAction == TBaseAction.baHit)                       // :5762
                {
                    if (g_Config.ActionList[(int)TAntiPlugActionMode.amHitToRun].boDebug)   // :5764
                    {
                        ErrorCode = 21;                                     // :5766
                        AddMainLogMsg(Format("%s:%d; 用户:%s",               // :5767-5768
                            AntiPlugActionModeNames3[(int)TAntiPlugActionMode.amHitToRun],
                            RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)TAntiPlugActionMode.amHit], dwCurTick),
                            sChrName), 0);
                    }
                }

                else if (FLastAction == TBaseAction.baSpell)                // :5772
                {
                    if (g_Config.ActionList[(int)TAntiPlugActionMode.amSpellToRun].boDebug)   // :5774
                    {
                        ErrorCode = 22;                                     // :5776
                        AddMainLogMsg(Format("%s:%d; 用户:%s",               // :5777-5778
                            AntiPlugActionModeNames3[(int)TAntiPlugActionMode.amSpellToRun],
                            RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)TAntiPlugActionMode.amSpell], dwCurTick),
                            sChrName), 0);
                    }
                }

                else if (FLastAction == TBaseAction.baTurn)                 // :5782
                {
                    if (g_Config.ActionList[(int)TAntiPlugActionMode.amTurnToMove].boDebug)   // :5784
                    {
                        ErrorCode = 23;                                     // :5786
                        AddMainLogMsg(Format("%s:%d; 用户:%s",               // :5787-5788
                            AntiPlugActionModeNames3[(int)TAntiPlugActionMode.amTurnToMove],
                            RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)TAntiPlugActionMode.amTurn], dwCurTick),
                            sChrName), 0);
                    }
                }

                else if (FLastAction == TBaseAction.baCutMeat)              // :5792
                {
                    if (g_Config.ActionList[(int)TAntiPlugActionMode.amCutMeatToMove].boDebug)   // :5794
                    {
                        ErrorCode = 24;                                     // :5796
                        AddMainLogMsg(Format("%s:%d; 用户:%s",               // :5797-5798
                            AntiPlugActionModeNames3[(int)TAntiPlugActionMode.amCutMeatToMove],
                            RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)TAntiPlugActionMode.amCutMeat], dwCurTick),
                            sChrName), 0);
                    }
                }

                // :5802 原文 `else if {(FLastAction = baRun) and} g_Config.ActionList[amRun].boDebug then`
                //       —— 前半被 `{}` 注释掉（同 D-T2/D-P5）
                else if (/*{(FLastAction = baRun) and}*/ g_Config.ActionList[AmRun].boDebug)
                {
                    // 走路间隔随移动速度+而改变 chongchong 2015-06-01
                    // （原文如此：:5804 这里写的是"走路"，而 :5492 写的是"跑行" —— 复制粘贴遗留，照抄）
                    dwTempInterval = MoveSpeedInterval(TAntiPlugActionMode.amRun);   // :5805-5810

                    if (nCompensationValue >= 0)                            // :5812
                    {
                        ErrorCode = 25;                                     // :5814
                        if (boChangeMap && RunGateTiming.TickDiff(GameSpeed.dwTicks[AmRun], dwCurTick) <= dwTempInterval)   // :5815
                            AddMainLogMsg(Format("%s:%d; [移动速度%s]; 用户:%s; 补偿:+%d; 补偿池:%d",   // :5816-5817
                                AntiPlugActionModeNames3[AmRun], dwTempInterval + 20, GetSpeedText(nMoveSpeed),
                                sChrName, nCompensationValue, nCompensationArr[AmRun]), 0);
                        else
                            AddMainLogMsg(Format("%s:%d; [移动速度%s]; 用户:%s; 补偿:+%d; 补偿池:%d",   // :5819-5820
                                AntiPlugActionModeNames3[AmRun],
                                RunGateTiming.TickDiff(GameSpeed.dwTicks[AmRun], dwCurTick) - nCompensationValue,
                                GetSpeedText(nMoveSpeed), sChrName, nCompensationValue, nCompensationArr[AmRun]), 0);
                    }
                    else
                    {
                        ErrorCode = 26;                                     // :5824
                        if (boChangeMap && RunGateTiming.TickDiff(GameSpeed.dwTicks[AmRun], dwCurTick) <= dwTempInterval)   // :5825
                            AddMainLogMsg(Format("%s:%d; [移动速度%s]; 用户:%s; 补偿:%d; 补偿池:%d",   // :5826-5827
                                AntiPlugActionModeNames3[AmRun], dwTempInterval + 20, GetSpeedText(nMoveSpeed),
                                sChrName, nCompensationValue, nCompensationArr[AmRun]), 0);
                        else
                            AddMainLogMsg(Format("%s:%d; [移动速度%s]; 用户:%s; 补偿:%d; 补偿池:%d",   // :5829-5830
                                AntiPlugActionModeNames3[AmRun],
                                RunGateTiming.TickDiff(GameSpeed.dwTicks[AmRun], dwCurTick) - nCompensationValue,
                                GetSpeedText(nMoveSpeed), sChrName, nCompensationValue, nCompensationArr[AmRun]), 0);
                    }
                }

                ErrorCode = 27;                                         // :5834
                if (MoveConcurrentAction.boDebug && ConcurrentCount > 0)   // :5835
                    AddMainLogMsg(Format("%s:%d; 用户:%s",               // :5836
                        AntiPlugActionModeNames3[AmMoveConcurrent], ConcurrentCount + 1, sChrName), 0);
            }
        }

        if (nDelayTime == 0 /*{and (not Msg.boDelay)}*/)                    // :5841
        {
            //if FLastAction = baRun then                                    // :5843 原文如此（已注释）
            {
                GameSpeed.OldLastRunTick = GameSpeed.dwTicks[AmRun];        // :5845
                GameSpeed.dwTicks[AmRun] = dwCurTick;                       // :5846
            }

            GameSpeed.dwTicks[AmRunToHit] = dwCurTick;                      // :5848
            GameSpeed.dwTicks[AmRunToSpell] = dwCurTick;                    // :5849
        }

        FLastAction = TBaseAction.baRun;                                    // :5852

        return false;   // 原文 :5853 分支体结束 → 继续进入公共收尾 9521-9681
    }

    // =================================================================================
    // 原文 :3525-4692  `if DefMsg.Ident = CM_WALK then`（**走路**）分支体
    //
    // ★ 与 CM_RUN 是"同构但不同"的一对（**逐条差异都要独立断言，不能照抄**）
    //   | 项 | CM_WALK | CM_RUN |
    //   |---|---|---|
    //   | 基础 ErrorCode | 2 / 201 / 202 / 203 | 3 / 301 / 303 / 304 |
    //   | `nCompensationValue := 0` 与 `ConcurrentCount := 0` 的**先后** | 先 ConcurrentCount（:3541）后 nCompensationValue（:3545） | 先 nCompensationValue（:4710）后 ConcurrentCount（:4713）（D-R1） |
    //   | 并发块内 ErrorsCode | 无（只有 202 之后） | :4719 有 `ErrorCode := 302` |
    //   | 前四个子块 | :3598 amHitToWalk(204) / :3778 amSpellToWalk(207,208) / :3963 amTurnToMove(212,213) / :4146 amCutMeatToMove(217) | :4767 amHitToRun(**无前置码**) / :4947 amSpellToRun(308) / :5123 amTurnToMove(309) / :5306 amCutMeatToMove(314) |
    //   | 第五个子块 | amWalk（:4329）：**无** `if not boCollectSpeed` 包裹 | amRun（:5488）：**有**（D-R3） |
    //   | 尾部调试码 | 8/9/10/11/228/12/13/14 | 21/22/23/24/25/26/27 |
    //   | tick 刷新 | OldLastWalkTick + dwTicks[amWalk] + [amWalkToHit] + [amWalkToSpell]（:4684-4688） | OldLastRunTick + dwTicks[amRun] + [amRunToHit] + [amRunToSpell]（:5845-5849） |
    //   **相同点**：都没有暗杀检测；四个子块的 mode 中 baTurn→`amTurnToMove`、baCutMeat→`amCutMeatToMove`
    //   两族**共用**（与"ToWalk / ToRun"的命名直觉不符）；并发块都用 `amMoveConcurrent`。
    //
    // ★ 原文缺陷 / 易错点（照抄）
    //   D-W1 :3541/:3545 的顺序与 CM_RUN 相反（D-R1 的另一面）。
    //   D-W2 :3598 的 baHit 子块有前置 `ErrorCode := 204`，而 CM_RUN 的对应子块没有（不对称）。
    //   D-W3 :4640 的 `else if {(FLastAction = baWalk) and} amWalk.boDebug then` —— 条件里没有 FLastAction。
    //   D-W4 :4684 先存 `OldLastWalkTick` 再覆盖（:4685），两处都读同一个 dwTicks[amWalk]。
    //   D-W5 :3461-3462 / :4360 的三处 `ErrorCode := 6`（丢弃并发日志）都在 `{}` 注释块内（不可达）。
    // =================================================================================

    /// <summary>
    /// 原文 :3525-4692 —— <c>CM_WALK</c>（走路）分支体，1:1 移植。
    /// 返回 <c>true</c> 表示原文执行了 <c>Exit</c>（:4368 丢弃并发、:4455/:4491 连续超速）。
    /// </summary>
    private bool CheckUsePluginWalk(TProcessMsg Msg, in TDefaultMessage DefMsg, uint dwCurTick,
        ref string sSendMsg, ref int nDelayTime, ref TAntiPlugAction AntiPlugAction, ref int ErrorCode,
        ref uint dwCurrentInterval, ref int ConcurrentCount, ref bool IsDropConcurrent,
        ref int nCompensationValue, ref bool Result)
    {
        // ---- 原文 :3474-3502 中被本分支独占/使用的局部量（同名同类型）----
        uint dwTempInterval;                                                // :3476
        bool boCurrentSpeed, boContinueSpeed = false, boCollectSpeed;       // :3486
        bool boContinueSpeedPass;                                           // :3487
        int nSpeedCount;                                                    // :3481
        int I, nCollectIndex, nCollectCount;                                // :3483

        const int AmWalk = (int)TAntiPlugActionMode.amWalk;                            // 2
        const int AmWalkToHit = (int)TAntiPlugActionMode.amWalkToHit;                  // 6
        const int AmWalkToSpell = (int)TAntiPlugActionMode.amWalkToSpell;              // 10
        const int AmMoveConcurrent = (int)TAntiPlugActionMode.amMoveConcurrent;        // 26
        TAntiPlugAction MoveConcurrentAction = g_Config.ActionList[AmMoveConcurrent];

        ErrorCode = 2;                                                      // :3531

        if (nRecordActionIndex >= MirClientContextConst.MAX_RECORD_ACTION_COUNT || nRecordActionIndex < 0)
            nRecordActionIndex = 0;                                         // :3534
        RecordActionArr[nRecordActionIndex].Action = TBaseAction.baWalk;                    // :3535
        RecordActionArr[nRecordActionIndex].Tick = MyGetTickCount();                        // :3536
        RecordActionArr[nRecordActionIndex].DefMsg = DefMsg;                                // :3537
        nRecordActionIndex++;                                                               // :3538 Inc

        // 移动并发 chongchong 2014-12-16
        ConcurrentCount = 0;                                                // :3541
        if (MoveConcurrentAction.boEnabled || MoveConcurrentAction.boDebug)  // :3542
            ConcurrentCount = GetConcurrentPacketCount(DefMsg);             // :3543

        nCompensationValue = 0;                                             // :3545（D-W1：在并发计数**之后**）

        ErrorCode = 201;                                                    // :3547

        if (MoveConcurrentAction.boEnabled)                                 // :3549
        {
            if (SumSpeedProcessArr[AmMoveConcurrent, 0] == 0)               // :3551
                SumSpeedProcessArr[AmMoveConcurrent, 0] = MyGetTickCount();  // :3552

            dwTempInterval = MoveConcurrentAction.nInterval;                // :3554

            if (ConcurrentCount >= dwTempInterval)                          // :3556
            {
                if (MoveConcurrentAction.boShowHint)                        // :3558
                    sSendMsg = MoveConcurrentAction.sHintText;              // :3559

                AntiPlugAction = MoveConcurrentAction;                      // :3561
                LastLockAntiPlugActionMode = TAntiPlugActionMode.amMoveConcurrent;   // :3562

                if (RunGateTiming.TickDiff(SumSpeedProcessArr[AmMoveConcurrent, 0],
                        MyGetTickCount()) <= g_Config.nSumSpeedCheckTime * 1000)   // :3564
                    SumSpeedProcessArr[AmMoveConcurrent, 1]++;              // :3565 Inc
                else
                {
                    SumSpeedProcessArr[AmMoveConcurrent, 1] = 1;            // :3568
                    SumSpeedProcessArr[AmMoveConcurrent, 0] = MyGetTickCount();   // :3569
                }

                if (FLastAction == TBaseAction.baWalk)                      // :3572
                {
                    // 走路间隔随移动速度+而改变 chongchong 2015-06-01
                    dwTempInterval = MoveSpeedInterval(TAntiPlugActionMode.amWalk);   // :3575-3580

                    dwCurrentInterval = RunGateTiming.TickDiff(GameSpeed.dwTicks[AmWalk], dwCurTick);   // :3582

                    if (dwCurrentInterval <= dwTempInterval / DROP_CONCURRENT_RATE)   // :3584
                    {
                        IsDropConcurrent = true;                            // :3586
                    }
                }
            }
        }

        ErrorCode = 202;                                                    // :3592
        if (AntiPlugAction == null)                                         // :3593
        {
            ErrorCode = 203;                                                // :3595

            // 攻击到走路
            if (FLastAction == TBaseAction.baHit)                           // :3598
            {
                ErrorCode = 204;                                            // :3600

                if (g_Config.ActionList[(int)TAntiPlugActionMode.amHitToWalk].boEnabled)   // :3601
                {
                    dwTempInterval = MoveSpeedInterval(TAntiPlugActionMode.amHitToWalk);   // :3603-3608

                    if (RunFamilySpeedBlock(TAntiPlugActionMode.amHitToWalk, TAntiPlugActionMode.amHit,
                            dwTempInterval, 2,
                            new SpeedStageCodes(afterCollect: 205, beforeWrite: 206),
                            Msg, dwCurTick, ref sSendMsg, ref nDelayTime, ref AntiPlugAction, ref ErrorCode,
                            ref dwCurrentInterval))
                    {
                        return true;                                        // 该子块的 Exit 均在 `{}` 注释内（不可达）
                    }
                }
            }

            // 魔法到走路
            else if (FLastAction == TBaseAction.baSpell)                    // :3778
            {
                ErrorCode = 207;                                            // :3780

                if (btJob != 0 && g_Config.ActionList[(int)TAntiPlugActionMode.amSpellToWalk].boEnabled)   // :3782
                {
                    ErrorCode = 208;                                        // :3784

                    dwTempInterval = MoveSpeedInterval(TAntiPlugActionMode.amSpellToWalk);   // :3786-3791

                    if (RunFamilySpeedBlock(TAntiPlugActionMode.amSpellToWalk, TAntiPlugActionMode.amSpell,
                            dwTempInterval, 3,
                            new SpeedStageCodes(afterSum: 209, afterCollect: 210, beforeWrite: 211),
                            Msg, dwCurTick, ref sSendMsg, ref nDelayTime, ref AntiPlugAction, ref ErrorCode,
                            ref dwCurrentInterval))
                    {
                        return true;                                        // 不可达
                    }
                }
            }

            // 转向到走路（**mode 是 amTurnToMove**）
            else if (FLastAction == TBaseAction.baTurn)                     // :3963
            {
                ErrorCode = 212;                                            // :3965

                if (g_Config.ActionList[(int)TAntiPlugActionMode.amTurnToMove].boEnabled)   // :3967
                {
                    dwTempInterval = MoveSpeedInterval(TAntiPlugActionMode.amTurnToMove);   // :3969-3974

                    if (RunFamilySpeedBlock(TAntiPlugActionMode.amTurnToMove, TAntiPlugActionMode.amTurn,
                            dwTempInterval, 4,
                            new SpeedStageCodes(beforeTick: 213, afterSum: 214, afterCollect: 215, beforeWrite: 216),
                            Msg, dwCurTick, ref sSendMsg, ref nDelayTime, ref AntiPlugAction, ref ErrorCode,
                            ref dwCurrentInterval))
                    {
                        return true;                                        // 不可达
                    }
                }
            }

            // 挖肉到走路（**mode 是 amCutMeatToMove**）
            else if (FLastAction == TBaseAction.baCutMeat)                  // :4146
            {
                ErrorCode = 217;                                            // :4148

                if (g_Config.ActionList[(int)TAntiPlugActionMode.amCutMeatToMove].boEnabled)   // :4150
                {
                    // :4152 `//dwTempInterval := g_Config.ActionList[amCutMeatToMove].nInterval;`（原文已注释）
                    dwTempInterval = MoveSpeedInterval(TAntiPlugActionMode.amCutMeatToMove);   // :4154-4158

                    if (RunFamilySpeedBlock(TAntiPlugActionMode.amCutMeatToMove, TAntiPlugActionMode.amCutMeat,
                            dwTempInterval, 5,
                            new SpeedStageCodes(afterSum: 218, afterCollect: 219, beforeWrite: 220),
                            Msg, dwCurTick, ref sSendMsg, ref nDelayTime, ref AntiPlugAction, ref ErrorCode,
                            ref dwCurrentInterval))
                    {
                        return true;                                        // 不可达
                    }
                }
            }

            // 移到下面并去掉 (FLastAction = baWalk) and 是因为边走边吃药的时候，加速检测不到 chongchong 2016-10-06
            else if (/*{(FLastAction = baWalk) and}*/ g_Config.ActionList[AmWalk].boEnabled)   // :4329
            {
                ErrorCode = 221;                                            // :4331

                // 走路间隔随移动速度+而改变 chongchong 2015-06-01
                dwTempInterval = MoveSpeedInterval(TAntiPlugActionMode.amWalk);   // :4334-4339

                dwCurrentInterval = RunGateTiming.TickDiff(GameSpeed.dwTicks[AmWalk], dwCurTick);   // :4341

                if (boChangeMap && dwCurrentInterval <= dwTempInterval + DELAY_TIME_ADD)   // :4343
                    dwCurrentInterval = dwTempInterval + DELAY_TIME_ADD;    // :4344

                IsDropConcurrent = dwCurrentInterval <= dwTempInterval / DROP_CONCURRENT_RATE;   // :4345

                boCurrentSpeed = dwCurrentInterval < dwTempInterval;        // :4347

                ErrorCode = 222;                                            // :4349

                // 连续加速主要用于卡刀反弹，因为卡刀反弹没有延时，所以会连续反弹 chongchong 2015-12-20
                boContinueSpeedPass = GameSpeed.boContinueSpeed &&          // :4352
                    (RunGateTiming.TickDiff(GameSpeed.dwStartSpeedTick, MyGetTickCount()) >=
                        dwTempInterval + g_Config.dwContinueSpeedPassIncTime);   // :4353

                if (IsDropConcurrent && !boContinueSpeedPass)               // :4355
                {
                    // { :4357-4364 原文整段被注释：`if g_Config.boShowDropConcurrentLog` 的【丢弃并发】日志（ErrorCode 6） }

                    SendActionRet(true);                                    // :4366
                    Result = true;                                          // :4367
                    return true;                                            // :4368 Exit
                }

                // ---- 补偿池（:4371-4418）----
                if (g_Config.ActionList[AmWalk].nCompensationValue > 0)     // :4371
                {
                    if (nCompensationArr[AmWalk] > g_Config.ActionList[AmWalk].nCompensationValue)   // :4373
                    {
                        nCompensationArr[AmWalk] = g_Config.ActionList[AmWalk].nCompensationValue;   // :4375
                    }

                    if (dwCurrentInterval > dwTempInterval / 3 &&
                        dwCurrentInterval < dwTempInterval * 2)             // :4378
                    {
                        nCompensationValue = unchecked((int)(dwCurrentInterval - dwTempInterval));   // :4380

                        if (nCompensationValue >= 0)                        // :4382
                        {
                            if (nCompensationValue >= 4)                    // :4384
                            {
                                nCompensationArr[AmWalk] = Math.Min(nCompensationArr[AmWalk] + nCompensationValue,
                                    g_Config.ActionList[AmWalk].nCompensationValue);   // :4386 Delphi Min
                            }
                            else
                            {
                                nCompensationValue = 0;                     // :4390

                                if (g_Config.boZeroCompensationValueClearPool)   // :4392
                                {
                                    nCompensationArr[AmWalk] = 0;           // :4394
                                }
                            }
                        }
                        else
                        {
                            if (nCompensationArr[AmWalk] + nCompensationValue < 0)   // :4400
                            {
                                nCompensationValue = -nCompensationArr[AmWalk];      // :4402
                                nCompensationArr[AmWalk] = 0;                        // :4403
                            }
                            else
                            {
                                nCompensationArr[AmWalk] = nCompensationArr[AmWalk] + nCompensationValue;   // :4407
                            }

                            dwCurrentInterval = unchecked(dwCurrentInterval - (uint)nCompensationValue);   // :4410
                            boCurrentSpeed = dwCurrentInterval < dwTempInterval;    // :4411
                        }
                    }
                }
                else
                {
                    nCompensationArr[AmWalk] = 0;                           // :4417
                }

                ErrorCode = 223;                                            // :4420

                nSpeedCount = 0;                                            // :4422
                boCollectSpeed = false;                                     // :4423

                if (SumSpeedProcessArr[AmWalk, 0] == 0)                     // :4425
                    SumSpeedProcessArr[AmWalk, 0] = MyGetTickCount();       // :4426

                ErrorCode = 224;                                            // :4428

                // 注意：CM_WALK 这里**没有** CM_RUN 的 `if not boCollectSpeed` 包裹（D-R3 的差异面）
                if (g_Config.dwCollectCount /*{g_Config.ActionList[amWalk].nCollectCount}*/ >= 2)   // :4430
                {
                    nCollectIndex = nCollectIntervalIndexArr[AmWalk];        // :4432
                    nCollectCount = g_Config.dwCollectCount /*{g_Config.ActionList[amWalk].nCollectCount}*/;   // :4433

                    // 倒数第2条数据采集到，加本次就是最一条搞定
                    if (dwCollectIntervalArr[AmWalk, nCollectCount - 2] != 0)   // :4436
                    {
                        // 本次和上次都超速就算超速 chongchong 2016-10-07
                        if (g_Config.boContinueSpeedCloseSocket && boCurrentSpeed)   // :4439
                        {
                            boContinueSpeed = true;                         // :4441
                            for (I = 1; I <= g_Config.nContinueSpeedCount; I++)   // :4442
                            {
                                if (dwCollectIntervalArr[AmWalk,
                                        (nCollectIndex - I + nCollectCount) % nCollectCount] >= 0)   // :4444
                                {
                                    boContinueSpeed = false;                // :4446
                                    break;                                  // :4447 Break
                                }
                            }

                            // 连续三次超速直接断开
                            if (boContinueSpeed)                        // :4452
                            {
                                ContinuousSpeed(TAntiPlugActionMode.amWalk, dwCurrentInterval);   // :4454
                                return true;                            // :4455 Exit
                            }
                        }

                        for (I = 0; I <= nCollectCount - 1; I++)        // :4459
                        {
                            if (I != nCollectIndex && dwCollectIntervalArr[AmWalk, I] < 0)   // :4461
                                nSpeedCount++;                          // :4462 Inc
                        }
                        if (boCurrentSpeed) nSpeedCount++;              // :4464 Inc
                        boCollectSpeed = nSpeedCount >= g_Config.dwSpeedValue;   // :4465  // g_Config.ActionList[amWalk].nCollectSpeedCount;
                    }
                    else
                    {
                        // 至少采集了1条
                        if (nCollectIndex >= 1)                         // :4470
                        {
                            // 本次和上次都超速就算超速 chongchong 2016-10-07
                            if (g_Config.boContinueSpeedCloseSocket && boCurrentSpeed)   // :4473
                            {
                                if (nCollectIndex >= g_Config.nContinueSpeedCount)   // :4475
                                {
                                    boContinueSpeed = true;             // :4477
                                    for (I = 1; I <= g_Config.nContinueSpeedCount; I++)   // :4478
                                    {
                                        if (dwCollectIntervalArr[AmWalk, nCollectIndex - I] >= 0)   // :4480
                                        {
                                            boContinueSpeed = false;    // :4482
                                            break;                      // :4483 Break
                                        }
                                    }

                                    // 连续三次超速直接断开
                                    if (boContinueSpeed)            // :4488
                                    {
                                        ContinuousSpeed(TAntiPlugActionMode.amWalk, dwCurrentInterval);   // :4490
                                        return true;                // :4491 Exit
                                    }
                                }
                            }

                            for (I = 0; I <= nCollectIndex - 1; I++)    // :4496
                            {
                                if (dwCollectIntervalArr[AmWalk, I] < 0)   // :4498
                                    nSpeedCount++;                      // :4499 Inc
                            }
                            if (boCurrentSpeed) nSpeedCount++;          // :4501 Inc

                            if (dwCurrentInterval <= dwTempInterval / 3)    // :4503
                            {
                                boCollectSpeed = true;                  // :4505
                            }
                            else
                            {
                                if (nCollectIndex + 1 <= 3)             // :4509
                                    boCollectSpeed = nSpeedCount >= 2;                  // :4510
                                else if (nCollectIndex + 1 <= 7)        // :4511
                                {
                                    boCollectSpeed = nSpeedCount >= (nCollectIndex + 1) / 2;   // :4513
                                }
                                else
                                {
                                    boCollectSpeed = nSpeedCount >= (nCollectIndex + 1) / 2 - 1;   // :4517
                                }
                            }
                        }
                        // 网进入游戏，就双倍（表现为一个表正常，一个包间隔很小），第一个包不会被采集
                        else if (dwCurrentInterval <= dwTempInterval / 3)   // :4522
                        {
                            boCollectSpeed = true;                      // :4524
                        }
                    }
                }
                else if (dwCurrentInterval <= dwTempInterval / 3)       // :4528
                {
                    boCollectSpeed = true;                              // :4530
                }

                ErrorCode = 225;                                        // :4533

                if (dwCurrentInterval <= dwTempInterval / 10 ||         // :4535
                    (!boContinueSpeedPass && boCurrentSpeed && boCollectSpeed))   // :4536
                {
                    if (g_Config.ActionList[AmWalk].boShowHint)         // :4538
                        sSendMsg = g_Config.ActionList[AmWalk].sHintText;   // :4539

                    AntiPlugAction = g_Config.ActionList[AmWalk];       // :4541
                    LastLockAntiPlugActionMode = TAntiPlugActionMode.amWalk;   // :4542

                    if (RunGateTiming.TickDiff(SumSpeedProcessArr[AmWalk, 0],
                            MyGetTickCount()) <= g_Config.nSumSpeedCheckTime * 1000)   // :4544
                        SumSpeedProcessArr[AmWalk, 1]++;                // :4545 Inc
                    else
                    {
                        SumSpeedProcessArr[AmWalk, 1] = 1;              // :4548
                        SumSpeedProcessArr[AmWalk, 0] = MyGetTickCount();   // :4549
                    }

                    if (AntiPlugAction.ProcessMode == TActionProcessMode.apmDelay &&
                        dwCurrentInterval < dwTempInterval)             // :4552
                    {
                        nDelayTime = (int)(dwTempInterval - dwCurrentInterval) + DELAY_TIME_ADD;   // :4554

                        // 修正加速一段时间后恢复到正常状态，一直提示加速
                        GameSpeed.nDelayCount[AmWalk] = GameSpeed.nDelayCount[AmWalk] + 1;   // :4557
                        if (GameSpeed.nDelayCount[AmWalk] > 8)          // :4558
                        {
                            GameSpeed.nDelayCount[AmWalk] = 0;          // :4560
                            nDelayTime = 0;                             // :4561
                            SendActionRet(true);                        // :4562
                        }
                    }

                    if (g_Config.boShowAttackLog)                       // :4566
                    {
                        ErrorCode = 7;                                  // :4568
                        AddMainLogMsg(Format("【用户超速】%s:%d; [移动速度%s]; 用户:%s",   // :4569-4570
                            AntiPlugActionModeNames3[AmWalk], dwCurrentInterval,
                            GetSpeedText(nMoveSpeed), sChrName), 0);
                    }
                }
                else
                {
                    if (!Msg.boDelay && !boContinueSpeedPass)           // :4575
                    {
                        GameSpeed.nDelayCount[AmWalk] = 0;             // :4577
                    }
                }

                ErrorCode = 226;                                        // :4581
                if (nDelayTime == 0 && !Msg.boDelay)                    // :4583
                {
                    nCollectIndex = nCollectIntervalIndexArr[AmWalk];   // :4585

                    if (dwCurrentInterval >= dwTempInterval)            // :4587
                        dwCollectIntervalArr[AmWalk, nCollectIndex] = 1;       // :4588
                    else
                        dwCollectIntervalArr[AmWalk, nCollectIndex] =
                            unchecked((int)(dwCurrentInterval - dwTempInterval));   // :4590

                    nCollectIntervalIndexArr[AmWalk] = (nCollectIndex + 1)
                        % g_Config.dwCollectCount /*{g_Config.ActionList[amWalk].nCollectCount}*/;   // :4592
                }
            }

            ErrorCode = 227;                                            // :4596

            // :4598-4677 调试日志
            if (!Msg.boDelay)                                           // :4598
            {
                if (FLastAction == TBaseAction.baHit)                   // :4600
                {
                    if (g_Config.ActionList[(int)TAntiPlugActionMode.amHitToWalk].boDebug)   // :4602
                    {
                        ErrorCode = 8;                                  // :4604
                        AddMainLogMsg(Format("%s:%d; 用户:%s",           // :4605-4606
                            AntiPlugActionModeNames3[(int)TAntiPlugActionMode.amHitToWalk],
                            RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)TAntiPlugActionMode.amHit], dwCurTick),
                            sChrName), 0);
                    }
                }

                else if (FLastAction == TBaseAction.baSpell)            // :4610
                {
                    if (g_Config.ActionList[(int)TAntiPlugActionMode.amSpellToWalk].boDebug)   // :4612
                    {
                        ErrorCode = 9;                                  // :4614
                        AddMainLogMsg(Format("%s:%d; 用户:%s",           // :4615-4616
                            AntiPlugActionModeNames3[(int)TAntiPlugActionMode.amSpellToWalk],
                            RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)TAntiPlugActionMode.amSpell], dwCurTick),
                            sChrName), 0);
                    }
                }

                else if (FLastAction == TBaseAction.baTurn)             // :4620
                {
                    if (g_Config.ActionList[(int)TAntiPlugActionMode.amTurnToMove].boDebug)   // :4622
                    {
                        ErrorCode = 10;                                 // :4624
                        AddMainLogMsg(Format("%s:%d; 用户:%s",           // :4625-4626
                            AntiPlugActionModeNames3[(int)TAntiPlugActionMode.amTurnToMove],
                            RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)TAntiPlugActionMode.amTurn], dwCurTick),
                            sChrName), 0);
                    }
                }

                else if (FLastAction == TBaseAction.baCutMeat)          // :4630
                {
                    if (g_Config.ActionList[(int)TAntiPlugActionMode.amCutMeatToMove].boDebug)   // :4632
                    {
                        ErrorCode = 11;                                 // :4634
                        AddMainLogMsg(Format("%s:%d; 用户:%s",           // :4635-4636
                            AntiPlugActionModeNames3[(int)TAntiPlugActionMode.amCutMeatToMove],
                            RunGateTiming.TickDiff(GameSpeed.dwTicks[(int)TAntiPlugActionMode.amCutMeat], dwCurTick),
                            sChrName), 0);
                    }
                }

                // :4640 原文 `else if {(FLastAction = baWalk) and} g_Config.ActionList[amWalk].boDebug then`（D-W3）
                else if (/*{(FLastAction = baWalk) and}*/ g_Config.ActionList[AmWalk].boDebug)
                {
                    ErrorCode = 228;                                    // :4642

                    // 走路间隔随移动速度+而改变 chongchong 2015-06-01
                    dwTempInterval = MoveSpeedInterval(TAntiPlugActionMode.amWalk);   // :4645-4650

                    if (nCompensationValue >= 0)                        // :4652
                    {
                        ErrorCode = 12;                                 // :4654
                        if (boChangeMap && RunGateTiming.TickDiff(GameSpeed.dwTicks[AmWalk], dwCurTick) <= dwTempInterval)   // :4655
                            AddMainLogMsg(Format("%s:%d; [移动速度%s]; 用户:%s; 补偿:+%d; 补偿池:%d",   // :4656-4657
                                AntiPlugActionModeNames3[AmWalk], dwTempInterval + 20, GetSpeedText(nMoveSpeed),
                                sChrName, nCompensationValue, nCompensationArr[AmWalk]), 0);
                        else
                            AddMainLogMsg(Format("%s:%d; [移动速度%s]; 用户:%s; 补偿:+%d; 补偿池:%d",   // :4659-4660
                                AntiPlugActionModeNames3[AmWalk],
                                RunGateTiming.TickDiff(GameSpeed.dwTicks[AmWalk], dwCurTick) - nCompensationValue,
                                GetSpeedText(nMoveSpeed), sChrName, nCompensationValue, nCompensationArr[AmWalk]), 0);
                    }
                    else
                    {
                        ErrorCode = 13;                                 // :4664
                        if (boChangeMap && RunGateTiming.TickDiff(GameSpeed.dwTicks[AmWalk], dwCurTick) <= dwTempInterval)   // :4665
                            AddMainLogMsg(Format("%s:%d; [移动速度%s]; 用户:%s; 补偿:%d; 补偿池:%d",   // :4666-4667
                                AntiPlugActionModeNames3[AmWalk], dwTempInterval + 20, GetSpeedText(nMoveSpeed),
                                sChrName, nCompensationValue, nCompensationArr[AmWalk]), 0);
                        else
                            AddMainLogMsg(Format("%s:%d; [移动速度%s]; 用户:%s; 补偿:%d; 补偿池:%d",   // :4669-4670
                                AntiPlugActionModeNames3[AmWalk],
                                RunGateTiming.TickDiff(GameSpeed.dwTicks[AmWalk], dwCurTick) - nCompensationValue,
                                GetSpeedText(nMoveSpeed), sChrName, nCompensationValue, nCompensationArr[AmWalk]), 0);
                    }
                }

                ErrorCode = 14;                                         // :4674
                if (MoveConcurrentAction.boDebug && ConcurrentCount > 0)   // :4675
                    AddMainLogMsg(Format("%s:%d; 用户:%s",               // :4676
                        AntiPlugActionModeNames3[AmMoveConcurrent], ConcurrentCount + 1, sChrName), 0);
            }
        }

        if (nDelayTime == 0 /*{and (not Msg.boDelay)}*/)                    // :4680
        {
            //if FLastAction = baWalk then                                   // :4682 原文如此（已注释）
            {
                GameSpeed.OldLastWalkTick = GameSpeed.dwTicks[AmWalk];      // :4684
                GameSpeed.dwTicks[AmWalk] = dwCurTick;                      // :4685
            }

            GameSpeed.dwTicks[AmWalkToHit] = dwCurTick;                     // :4687
            GameSpeed.dwTicks[AmWalkToSpell] = dwCurTick;                   // :4688
        }

        FLastAction = TBaseAction.baWalk;                                   // :4691

        return false;   // 原文 :4692 分支体结束 → 继续进入公共收尾 9521-9681
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
