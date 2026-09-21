// ============================================================================
// 源单元：Source\M2Engine\ObjPlayer.pas（GBK / UTF-8 镜像 49,232 LF；implementation 起于 :1411）
// 本文件：**经验结算 / 金额零头 / 聊天与下发面（切片 Core1）** —— 本车道 p13-m2-objplayer 自有切片。
//
// 覆盖原文行号范围（实现段；行号由 `_analysis/utf8_mirror/M2Engine/ObjPlayer.pas` 的
// 顶层 `TPlayObject.` 方法头 + begin/end 配平扫描实测得到 —— 任务书给的
// `.p13scratch/_p13_joined.txt` 在本工作树中**不存在**（`Test-Path` 为 False），
// 故改用镜像文件直接测量；每条方法体首行的 `// 原文 NNNN-NNNN` 即实测区间）：
//
//   1485-2505  Create                    （1021 行，★ 未移植 —— 见下）
//   2507-2521  DealCancel                2523-2527  DealCancelA
//   （2529-2547 GoldChanged / GameGoldChanged / NewGamePointChanged / GameGloryChanged
//     **已由 TPlayObject.PlayerSurface.Gold.cs 移植**，本文件不重复声明）
//   2549-2631  RunNotice                 （★ 未移植）
//   2634-2672  WinExp                    2674-2835  GetExp
//   2837-2922  IncExp                    2924-2963  WinExpNG
//   2965-3073  GetExpNG                  3075-3135  IncExpNG
//   3137-3229  IncBeadExp
//   （3231-3249 IncGold / 3284-3293 DecGold / 3295-3303 DecGameGold / 3251-3260 IncGameGold
//     **已由 Gold.cs 移植**，本文件不重复声明）
//   3262-3271  IncGamePoint               3273-3282  IncGameGird
//   3305-3311  DecGameGird               3313-3322  IncGameDiamond
//   3324-3330  DecGameDiamond            3332-3338  IncGameGlory
//   3340-3346  DecGameGlory              3348-3351  SetSoftVersionDateEx
//   3353-3358  SendAcupointLevels
//   （3360-3394 SendAddItem **已由 TCreature.PlayerSurface.Items.cs 移植**，不重复声明）
//   3396-3424  IsGroupMember              3428-3509  Whisper
//   3511-3524  IsBlockWhisper             3526-3553  SendSocket（原文 virtual）
//   3556-3584  SendSocketEx               3587-3597  SendOpenMagic
//   3599-3612  SendDefMessage             3614-3651  ClientQueryAssessHero
//   3653-3665  RefUserState               （接缝：地图两个标志）
//   3667-3673  GetHearMsgFColor           3675-3678  RefHearMsgColor
//   3680-3684  RefMyStatus                3687-3690  CanSaveToStorage
//   3692-3696  CanSaveToBigStorage
//   （3698-3771 Operate **不在本切片**，由 TPlayObject.PlayerSurface.Core2.cs 负责）
// 末条方法末端：3696（下一条 `Operate` 的 header 在 3698）。
//
// 方法总数：**41**（与顶层扫描在本区间命中的 `TPlayObject.` 方法数一致）。
//
// 三数对账（本切片）：
//   真实体 33   WinExp / GetExp / IncExp / WinExpNG / GetExpNG / IncExpNG / IncBeadExp /
//               IncGamePoint / IncGameGird / DecGameGird / IncGameDiamond / DecGameDiamond /
//               IncGameGlory / DecGameGlory / SetSoftVersionDateEx / SendAcupointLevels /
//               IsGroupMember / Whisper / IsBlockWhisper / SendSocket / SendSocketEx /
//               SendOpenMagic / SendDefMessage / ClientQueryAssessHero / GetHearMsgFColor /
//               RefHearMsgColor / RefMyStatus / CanSaveToStorage / CanSaveToBigStorage /
//               DealCancel / DealCancelA / RefUserState /
//               **GetLevelExpRate**（★ ObjBase.pas:41976-41985 —— `WinExp` 的唯一调用点，
//               托管侧全仓 grep 只有本文件命中，即尚未被任何切片移植；按任务书第 4/5 条就地落地）
//   NotPorted 2 Create（1485-2505，1021 行）/ RunNotice（2549-2631）
//   原文如此 9  逐条见各方法 XML doc 的「★ 原文缺陷 / 原文如此」段：
//               ① WinExp:2639-2641 两步 32 位乘法裸回绕（无溢出保护）
//               ② GetExp:2716/2736 `boHumanGetAllExp` 分支把 lwExp 重新赋成 dwExp
//               ③ GetExp:2757 与 IncExp:2856 的 `RefExp:` 标签夹在 `if..end`/`else` 之间，
//                  `goto RefExp; Exit;` 使 else 分支在触顶前不可达
//               ④ GetExp:2665-2668 注释写"英雄1000以后"、代码判的却是人物 m_Abil.Level
//               ⑤ GetExpNG:2751 用 Min、GetExpNG:3015 用 Max —— 同类封顶方向相反
//               ⑥ IncExp:2915 的 m_dwGetExp 被写在 `if g_FunctionNPC <> nil` 里面
//               ⑦ IncBeadExp:3184-3185 用 `Exit`（中断整轮）而非 `Continue`
//               ⑧ IncBeadExp:3198 `Round(dwUseExp * 100 / StdItem.Shape)` 无除零保护
//               ⑨ SendSocket:3532 / SendSocketEx:3562 `DefMsg = nil` 只打日志不早退；
//                  另 Whisper:3451-3468 的"自动回复"发给了发送者自己
//
// 39 条**逐条对齐**（32 条区间内真实体 + 2 NotPorted + 5 已由更早切片移植）：
//   · 32 = 上列真实体里**落在 1486-3687 区间之内**的那些（GetLevelExpRate 另有出处，见下）
//   · 2  = Create（1485-2505）/ RunNotice（2549-2631）—— 本文件的 NotPorted
//   · 5  = GoldChanged / GameGoldChanged / NewGamePointChanged / GameGloryChanged
//          （`PlayerSurface.Gold.cs`）+ SendAddItem（`TCreature.PlayerSurface.Items.cs`）
//   （任务书给的 41 条是把 `Operate`（3698，**Core2**）与 `Run`（3772，**Core2**）
//     一起算进来的口径：39 + 2 = 41。）
//   ★ 额外的第 33 条真实体 `GetLevelExpRate` **不属于**这 39 条 —— 它的原文出处是
//     `ObjBase.pas:41976-41985`，是被任务书第 4/5 条允许的"补齐本切片所需成员"。
//
// ★★ Create（1485-2505，1021 行）**整条留痕未移植**，**没有**做半移植：
//   它是纯字段初始化例程，逐段赋值约 700 个 `TPlayObject` 字段
//   （`m_ItemBoxItems`/`m_VisibleEvents`/`m_ChallengeItemList`/`m_ClientBufList`/
//    `m_ArrBufList`/`m_CanJmpScriptLableList`/… 的 `TList.Create`/`TStringList.Create`），
//   其中绝大多数成员托管侧**不存在**且**不是标量**（是 `TList`/`THashList`/`TSafeList`/
//   `TBoxServerConfig`/`TStorageHeroInfo` 等容器与记录），声明它们等于把本切片变成
//   "半个 ObjPlayer 字段面"，与任务书第 3 条（只补本切片需要的成员）冲突。
//   故按任务书「若某个赋值块需要其它类/单元的成员，就对整个方法用 PortNotPorted」处理。
//
// ⚠ 本文件**不重复声明**任何既有成员（逐条 grep 取证）：
//   · m_ItemList（背包）→ Engine/ObjBase.cs:62
//   · m_nGold / m_nGameGold / m_nGamePoint / m_nGameDiamond / m_nGameGird / m_nGameGlory /
//     m_boOffLine / m_boDummyObject / m_btPermission / m_sAutoSendMsg / m_boEmergencyClose
//     → Engine/ObjBase.OnlineMsg.cs:10/13/35/36/41/44/46/47/48/49/52
//   · m_sUserID / m_sIPaddr / m_nSocket / m_nGSocketIdx / m_dwLogonTick / m_boReadyRun /
//     m_nSessionId / m_MagicList → Engine/ObjBase.cs:188-197
//   · m_wAbil / m_wScreenWidth 之外的 TAbility 面 → Engine/ObjBase.cs:65（m_wAbil）
//   · m_MyHero → Engine/PlayerSurface/TPlayObject.PlayerSurface.Hero.cs:31
//   · m_nViewRange **属于 `TMonster`**（Engine/ObjBase.cs:220），**不是** TPlayObject 的，
//     故本文件既不复用也不重复声明（RunNotice 因此留痕，见下）
//   · m_btRaceServer → Engine/MagicModel.cs:52；m_btRace → Engine/ObjBase.cs:20
//   · m_nRecogId / m_boGhost / m_sCharName / m_PEnvir / m_btDirection / m_nCurrX / m_nCurrY
//     → Engine/ObjBase.cs:16-24
//   · m_StorageItemList / m_BigStorageItemList / m_nInfinityStorageExtCount /
//     m_AssessHeroInfos / m_boInFreePKArea / m_dwHearMsgColorTick / m_btHearMsgColor /
//     m_nOffOnlineTick / m_boDealing / m_DealCreat / m_DealLastTick / m_boKickFlag /
//     m_boSoftClose / m_boSendNotice / m_dwWaitLoginNoticeOKTick / m_BlockWhisperList /
//     m_boHearWhisper / m_GetWhisperHuman / m_GroupOwner / m_GroupMembers /
//     m_Abil / m_AbilNG / m_boTrainingNG / m_dwGetExp / m_dwBeadExp / m_nBeadSource /
//     m_nKillMonExpMultiple / m_nKillMonExpRate / m_SuiteExpMultiple / m_rExpItem /
//     m_boExpFromFromHero / FSoftVersionDateEx → **全文 grep 无命中**，本文件按原文声明
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.Core.Rtl;
using GXX.Core.Util;

namespace GXX.M2Server.Engine;

/// <summary>
/// 切片 Core1 的**跨单元依赖接缝**。
///
/// 为什么需要：本切片的方法体属于 `ObjPlayer.pas`，但其中若干调用落在
/// **其它单元/尚未切出的层**上（`ObjHero.pas` 的 `THeroObject.GetExp`、
/// `ObjBase.pas` 的 `HasLevelUp`/`IncHealthSpell`/`AddBodyLuck`/`TPlayObject.SysMsg`、
/// `M2Share.pas` 的 `AddGameDataLog`/`MainOutMessage`/`RunSocket.GetSocket`、
/// `TPlayObject.SendNotice`/`SendClientDataFile`）。
/// 按任务书第 4 条与台账 §48.1，这些**不臆造替身**，而是落最小接缝，默认「无宿主」。
/// </summary>
/// <remarks>
/// 逐条与既有接缝的关系：
/// 1. `SendDefMessage` **不在此处** —— 复用既有
///    <see cref="PlayerSurfaceMsgSeams.SendDefMessage"/>（`Gold.cs:52`），避免造第三份；
/// 2. `SendSocket` / `SendSocketEx` 是**原文 `TPlayObject` 自己的方法**
///    （ObjPlayer.pas:3526/3556），本文件已 1:1 移植，主体逻辑走
///    <see cref="PlayerSurfaceSocketSeams"/> 的投递落点与
///    <see cref="PlayerSurfaceCore1Seams.RunSocketGetSocket"/> 的网关查表接缝；
/// 3. `SendMsg(BaseObject, wIdent, wParam, nParam1, nParam2, nParam3, sMsg)`
///    （`TBaseObject.SendMsg`，7 参）托管侧**不存在**（`Engine/ObjBase.cs:79` 只有 6 参入队版，
///    名字相同但语义不同）—— 故按 §18.8「不把虚调用降级」的反面，用接缝**如实表达**该调用；
/// 4. `AddGameDataLog`（`M2Share.pas:3121` 的 9 参重载）与 `Npc/ObjNpcSeams.cs:602` 的
///    同签名接缝**分属不同命名空间/车道**，本文件不跨车道引用，另立一份。
/// </remarks>
public static class PlayerSurfaceCore1Seams
{
    // ------------------------------------------------------------------
    // ObjBase.pas 侧
    // ------------------------------------------------------------------

    /// <summary>`TBaseObject.AddBodyLuck(dLuck: Double)`（ObjBase.pas:649 声明，:13916 实现）。
    /// 原文调用点：ObjPlayer.pas:2767 / 2789。默认：无宿主，丢弃。</summary>
    public static Action<TCreature, double> AddBodyLuck { get; set; } = (_, _) => { };

    /// <summary>`TBaseObject.HasLevelUp(nLevel: Integer; IsTriggerFunc: Boolean; SendHealthSpellChanged: Boolean)`
    /// （ObjBase.pas:13517）。原文调用点：ObjPlayer.pas:2866 / 2894 —— **两处都只传 1 个实参**，
    /// 靠两个默认值补齐。默认：无宿主，丢弃。</summary>
    public static Action<TCreature, int> HasLevelUp { get; set; } = (_, _) => { };

    /// <summary>`TSmartObject.HasLevelUpNG(nLevel: Integer)`（ObjBase.pas:1326 声明，:3595 实现）。
    /// 原文调用点：:3033 / :3059 / :3099 / :3122。默认：无宿主，丢弃。</summary>
    public static Action<TCreature, int> HasLevelUpNG { get; set; } = (_, _) => { };

    /// <summary>`TBaseObject.IncHealthSpell(nHP, nMP: LongWord; SendHealthSpellChanged: Boolean)`
    /// （ObjBase.pas:20500）。原文调用点（**三参全传**）：:2771 / :2799 / :2867 / :2895。默认：无宿主，丢弃。</summary>
    public static Action<TCreature, uint, uint, bool> IncHealthSpell { get; set; } = (_, _, _, _) => { };

    /// <summary>`TBaseObject.SendMsg(BaseObject; wIdent, wParam; nParam1..nParam3: NativeInt; sMsg)`
    /// （ObjBase.pas，**7 参**）。原文调用点：:2813 / :2912 / :3069 / :3131 / :3456 / :3460 /
    /// :3465 / :3474 / :3477 / :3482 / :3486 / :3492 / :3495 / :3499 / :3503 / :3683。
    /// 默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject, int, long, long, long, long, string> SendMsg { get; set; }
        = (_, _, _, _, _, _, _) => { };

    /// <summary>`TBaseObject.SysMsg(sMsg: AnsiString; FColor, BColor: Integer; MsgType: TMsgType)`
    /// （ObjBase.pas）—— 原文 :3217 用的是**四参**重载 `SysMsg(sMsg, 251, 249, t_Hint)`
    /// （与托管 `TCreature.SysMsg(string, TMsgColor, TMsgType)` **不同签名**，不能直调）。
    /// 默认：无宿主，丢弃。</summary>
    public static Action<TCreature, string, int, int, TMsgType> SysMsgFB { get; set; }
        = (_, _, _, _, _) => { };

    /// <summary>`TPlayObject.SendUpdateItem(UserItem: pTUserItem)`（原文 :1198 一带声明）。
    /// 原文调用点：:3215。默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject, TUserItem> SendUpdateItem { get; set; } = (_, _) => { };

    /// <summary>`TPlayObject.LevelUpFunc(): Boolean`（声明 ObjPlayer.pas:1304，
    /// 实现体在 ObjPlayer.pas 内，依赖 `g_Config.boLevelUpFunc` 与脚本事件）。
    /// 原文调用点：:2832（`GetExp` 升级后处理）。**返回值被原文丢弃**（作为语句调用）。
    /// 默认：无宿主，返回 false。</summary>
    public static Func<TPlayObject, bool> LevelUpFunc { get; set; } = _ => false;

    /// <summary>
    /// `TEnvirnoment.m_boEXPRATE` / `m_nEXPRATE`（原文 :2658-2659 与 :2949-2950）——
    /// 地图经验倍率。原文是**两次字段读**（先判 `m_boEXPRATE` 为真，再取 `m_nEXPRATE`）。
    /// 托管 `TEnvirnoment`（`Engine/Envir.cs:80`）**无这两个字段**且不是 partial ⇒ 接缝。
    /// 参数：玩家（取其 `m_PEnvir`）/ **out** 倍率。返回：原文 `m_PEnvir.m_boEXPRATE`。
    /// ⚠ 原文**不判** `m_PEnvir = nil`（会 AV）；接缝默认实现**同样不判**，
    /// 由宿主自行决定（默认实现直接返回 false，不触碰 `m_PEnvir`）。
    /// 默认：false（= 原文 `m_boEXPRATE = False` 分支，倍率不生效）。
    /// </summary>
    public static MapExpRateFn MapExpRate { get; set; } = (TPlayObject _, out uint rate) =>
    {
        rate = 0;
        return false;
    };

    /// <summary>
    /// `g_Config.AcupointLevels: array [0 .. 4, 0 .. 4] of Integer`
    /// （`M2Share.pas:2326` 声明，:5001 typed constant）—— 打通穴道需要的内功等级表。
    /// 原文 :3356 用 `SendSocketEx(@m_DefMsg, @g_Config.AcupointLevels, SizeOf(g_Config.AcupointLevels))`
    /// **按内存原样**送出（`5 * 5 * 4 = 100` 字节）。
    /// 托管 `M2Config` **无该字段**（全文 grep 0 命中）⇒ 接缝返回**同样 100 字节**的块，
    /// 默认内容为全零（尺寸与原文 `SizeOf` 一致，内容由宿主注入）。
    /// </summary>
    public static Func<byte[]> AcupointLevels { get; set; } = () => new byte[5 * 5 * 4];

    // ------------------------------------------------------------------
    // ObjHero.pas 侧
    // ------------------------------------------------------------------

    /// <summary>`THeroObject.GetExp(dwExp: LongWord; IsAdjustHero, IsIncBead: Boolean)`
    /// （ObjHero.pas:6194 一带）—— 原文三处**硬强转** `THeroObject(m_MyHero).GetExp(...)`。
    /// 原文调用点：:2700（`GetExp(lwExp, False, IsIncBead)`）、
    /// :2713（`GetExp(lwExp, FromNPC, IsIncBead)`）、:2728 / :2733（`GetExp(lwExp, False, IsIncBead)`）。
    /// ★ 托管侧 `THeroObject` 尚未切出（`m_MyHero` 是 `TCreature?`，Hero.cs:31）——
    /// 故用接缝如实表达"转调英雄对象的同名方法"，参数逐个对齐。
    /// 默认：无宿主，丢弃。</summary>
    public static Action<TCreature, uint, bool, bool> HeroGetExp { get; set; } = (_, _, _, _) => { };

    /// <summary>`THeroObject.GetExpNG(dwExp: LongWord)`（ObjHero.pas）。
    /// 原文调用点：:2983（`GetExpNG(lwExp)`）、:2994 / :2999。默认：无宿主，丢弃。</summary>
    public static Action<TCreature, uint> HeroGetExpNG { get; set; } = (_, _) => { };

    // ------------------------------------------------------------------
    // M2Share.pas / 全局侧
    // ------------------------------------------------------------------

    /// <summary>`AddGameDataLog(LogAction1, LogAction2: Byte; LogActor: TBaseObject; ItemName: string;
    /// ItemMakeIndex: Integer; TargetName: string; Data1, Data2: Integer; LogDesc: string)`
    /// （M2Share.pas:3121 声明，:11686-11745 实现）。原文调用点（**9 参全传**）：
    /// :2828 / :2921 / :3072 / :3133。默认：无宿主，丢弃。</summary>
    public static Action<byte, byte, TCreature, string, int, string, int, int, string> AddGameDataLog { get; set; }
        = (_, _, _, _, _, _, _, _, _) => { };

    /// <summary>
    /// **同族 8 参重载**：原文 `AddGameDataLog(LogAction1, LogAction2: Byte; LogActor: TBaseObject;
    /// ItemName: string; ItemMakeIndex: Integer; TargetName: string; Data1, Data2: Integer)` ——
    /// `ObjPlayer.pas:2828 / 2921 / 3072 / 3133` 的四处调用**只传 8 个实参**（省掉 `LogDesc`）。
    /// ⚠ C# 的 `Action` 委托**不能有可选参数**，故此处必须**另立一个 8 参委托**才能表达原文的重载
    /// （集成时曾误把 8 参调用接到 9 参委托上 ⇒ CS7036）。
    /// 本片调用点：`IncExp` 2828 / `IncExp` 2921 / `IncExpNG` 3072 / `IncBeadExp` 3133。
    /// 默认：无宿主，丢弃。
    /// </summary>
    public static Action<byte, byte, TCreature, string, int, string, int, int> AddGameDataLog8 { get; set; }
        = (_, _, _, _, _, _, _, _) => { };

    /// <summary>`MainOutMessage(sMsg: string)`（M2Share.pas）—— 原文 :3449 / :3533 / :3550 /
    /// :3551 / :3563 / :3581 / :3582 / :2629。默认：无宿主，丢弃。</summary>
    public static Action<string> MainOutMessage { get; set; } = _ => { };

    /// <summary>`RunSocket.GetSocket(nGateIdx: Integer): TSocketThread`（RunSock.pas）。
    /// ★ **返回 `object?`**：`TSocketThread` 是原文类型（`RunSock.pas`），托管侧
    /// `Engine/RunSock.cs` 只有别的面；此处按"不造第二份"的原则让宿主注入任意宿主对象，
    /// 由本接缝的 <see cref="RunSocketAdd"/> 自行解释。默认：无宿主，返回 null（= 原文
    /// `SocketThread = nil` 早退分支）。</summary>
    public static Func<int, object?> RunSocketGetSocket { get; set; } = _ => null;

    /// <summary>`SocketThread.Add(GM_DATA, m_nSocket, m_nGSocketIdx, 0, DefMsg, Buffer, Len)`
    /// （RunSock.pas）—— 参数逐个对齐原文 3526-3584 两处调用。
    /// 参数：宿主对象 / m_nSocket / m_nGSocketIdx / DefMsg / 文本或二进制尾数据。
    /// 默认：无宿主，丢弃。</summary>
    public static Action<object, int, int, TDefaultMessage, PlayerSurfaceSocketPayload> RunSocketAdd { get; set; }
        = (_, _, _, _, _) => { };

    /// <summary>`TPlayObject.SendNotice(IsRealSendNotice: Boolean)`（ObjPlayer.pas:8203，
    /// 实现 **105 行**）—— 原文调用点：:2566 / :2577（**都在 `RunNotice` 里**）。
    /// 托管侧该方法是**留痕未移植**（`TPlayObject.PlayerSurface.Core4.cs:532`）。
    /// ★ 本文件**已把 `RunNotice` 整条留痕**，故当前**没有**任何调用点；保留此接缝是为了
    /// 让将来接通 `RunNotice` 时不必再改公共约定面（声明与 `ResetDefaults` 成对存在）。
    /// 默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject, bool> SendNotice { get; set; } = (_, _) => { };

    /// <summary>`TPlayObject.SendClientDataFile(sFileName: string)`（ObjPlayer.pas）。
    /// 原文调用点：:2584（同样在 `RunNotice` 里）。默认：无宿主，丢弃。</summary>
    public static Action<TPlayObject, string> SendClientDataFile { get; set; } = (_, _) => { };

    /// <summary>`UserEngine.GetPlayObject(sName: string): TPlayObject`（UsrEngn.pas）。
    /// 原文调用点：:3432（`Whisper`）。默认：无宿主，返回 null。</summary>
    public static Func<string, TPlayObject?> GetPlayObject { get; set; } = _ => null;

    /// <summary>
    /// `MyGetTickCount(): LongWord`（`M2Share.pas`）—— 本切片四处早退判定
    /// （:3538 / :3568 / :3592 / :3604）与 `GetHearMsgFColor`（:3669）都要读它。
    ///
    /// **为什么需要接缝**：本车道另有两个 tick 面 ——
    /// `PlayerSurfaceNpcSeams.MyGetTickCount`（`NpcSession.cs:47`，**该切片的字段初始化器**用它）
    /// 与 `PlayerSurfaceTick.MyGetTickCount()`（`TWarrContinueHitManager.cs:301` 的静态方法）。
    /// 二者都指向 `DelphiRTL.GetTickCount()`，**不可注入**。
    /// 而 `SendSocket` 的 tick 回绕缺陷无法用"设一个绝对值"的方式稳定复现
    /// （`Environment.TickCount` 会变化），故本切片另立一个**可注入**的 tick 接缝：
    /// 它在**方法体内**（不是字段初始化器）被调用，因此不影响本类字段的构造初值。
    /// 默认实现 = 既有的 `PlayerSurfaceTick.MyGetTickCount()`（**行为不变**）。
    /// </summary>
    public static Func<uint> MyGetTickCount { get; set; } = PlayerSurfaceTick.MyGetTickCount;

    // ------------------------------------------------------------------
    // M2Share.pas 的全局串（原文在 `Whisper` 里引用；托管侧未收录）
    // ------------------------------------------------------------------

    /// <summary>`g_sCanotSendmsg`（`M2Share.pas` 全局串；原文 :3437 拼在对方名字后面）。
    /// 原文值 `'无法发送信息.'`（含**前导空格**：`whostr + '无法发送信息.'`）。</summary>
    public static string g_sCanotSendmsg { get; set; } = "无法发送信息.";

    /// <summary>`g_sUserDenyWhisperMsg`（`M2Share.pas` 全局串；原文 :3443）。
    /// 原文值 `' 拒绝私聊！'`（**含前导空格**）。</summary>
    public static string g_sUserDenyWhisperMsg { get; set; } = " 拒绝私聊！";

    /// <summary>`g_sUserNotOnLine`（`M2Share.pas` 全局串；原文 :3508）。
    /// 与 `Npc/ObjNpcInputSeams.cs:34` 的同名字符串**值相同**（`'  没有在线！'`，两个前导空格），
    /// 但分属不同命名空间/车道，本文件不跨车道引用。</summary>
    public static string g_sUserNotOnLine { get; set; } = "  没有在线！";

    /// <summary>`g_sDealActionCancelMsg`（`M2Share.pas` 全局串；原文 :2519 ——
    /// `SysMsg(g_sDealActionCancelMsg { '交易取消' } , c_Green, t_Hint)`）。
    /// 原文注释里给出了字面值，故默认值即 `'交易取消'`（与原文 typed constant 一致）。</summary>
    public static string g_sDealActionCancelMsg { get; set; } = "交易取消";

    /// <summary>
    /// `TEnvirnoment.m_boFightZone` / `m_boSAFE`（原文 :3658 / :3660，`RefUserState` 的两个条件）——
    /// 托管 `TEnvirnoment`（`Engine/Envir.cs:80`）**无这两个字段**且不是 partial ⇒ 接缝。
    /// 参数：玩家（取其 `m_PEnvir`）/ **out** 两个标志。默认：两者皆 false
    /// （= 原文两处 `if` 都为假，状态字节只剩 `m_boInFreePKArea` 那一位）。
    /// ⚠ 原文**不判** `m_PEnvir = nil`；默认实现不触碰 `m_PEnvir`。
    /// </summary>
    public static MapStateFlagsFn MapStateFlags { get; set; } = (TPlayObject _, out bool fightZone, out bool safe) =>
    {
        fightZone = false;
        safe = false;
    };

    /// <summary>恢复全部默认实现（单测隔离用）。</summary>
    public static void ResetDefaults()
    {
        AddBodyLuck = (_, _) => { };
        HasLevelUp = (_, _) => { };
        HasLevelUpNG = (_, _) => { };
        IncHealthSpell = (_, _, _, _) => { };
        SendMsg = (_, _, _, _, _, _, _) => { };
        SysMsgFB = (_, _, _, _, _) => { };
        SendUpdateItem = (_, _) => { };
        HeroGetExp = (_, _, _, _) => { };
        HeroGetExpNG = (_, _) => { };
        AddGameDataLog = (_, _, _, _, _, _, _, _, _) => { };
        MainOutMessage = _ => { };
        RunSocketGetSocket = _ => null;
        RunSocketAdd = (_, _, _, _, _) => { };
        SendNotice = (_, _) => { };
        SendClientDataFile = (_, _) => { };
        GetPlayObject = _ => null;
        MyGetTickCount = PlayerSurfaceTick.MyGetTickCount;
        LevelUpFunc = _ => false;
        MapExpRate = (TPlayObject _, out uint rate) =>
        {
            rate = 0;
            return false;
        };
        AcupointLevels = () => new byte[5 * 5 * 4];
        g_sCanotSendmsg = "无法发送信息.";
        g_sUserDenyWhisperMsg = " 拒绝私聊！";
        g_sUserNotOnLine = "  没有在线！";
        g_sDealActionCancelMsg = "交易取消";
        MapStateFlags = (TPlayObject _, out bool fightZone, out bool safe) =>
        {
            fightZone = false;
            safe = false;
        };
    }
}

/// <summary>
/// `TEnvirnoment.m_boFightZone` / `m_boSAFE` 读取的托管签名
/// （见 <see cref="PlayerSurfaceCore1Seams.MapStateFlags"/>）。
/// </summary>
/// <param name="self">取 `m_PEnvir` 的玩家对象。</param>
/// <param name="fightZone">原文 `m_PEnvir.m_boFightZone`。</param>
/// <param name="safe">原文 `m_PEnvir.m_boSAFE`。</param>
public delegate void MapStateFlagsFn(TPlayObject self, out bool fightZone, out bool safe);

/// <summary>
/// `TEnvirnoment.m_boEXPRATE` / `m_nEXPRATE` 读取的托管签名
/// （见 <see cref="PlayerSurfaceCore1Seams.MapExpRate"/>）。
/// </summary>
/// <param name="self">取 `m_PEnvir` 的玩家对象。</param>
/// <param name="rate">原文 `m_PEnvir.m_nEXPRATE`（仅在返回 true 时有意义）。</param>
/// <returns>原文 `m_PEnvir.m_boEXPRATE`。</returns>
public delegate bool MapExpRateFn(TPlayObject self, out uint rate);

/// <summary>
/// 原文 `SocketThread.Add(..., Buffer, BufferLen)` 里 `Buffer: PAnsiChar` 的托管载体。
/// 原文两处调用形态不同（:3546 传 `PAnsiChar(sMsg)` + `Length(sMsg)`；:3577 传
/// `Buffer` + `BufferLen`），一律归一为"字节序列 + 长度"。
/// </summary>
public readonly struct PlayerSurfaceSocketPayload
{
    /// <summary>字节载荷（原文 `PAnsiChar` 指向的那段内存）。</summary>
    public readonly byte[] Buffer;

    /// <summary>原文第三/第六参 `BufferLen`（**显式长度**，不是 `Buffer.Length`）。</summary>
    public readonly int BufferLen;

    /// <summary>构造。</summary>
    public PlayerSurfaceSocketPayload(byte[] buffer, int bufferLen)
    {
        Buffer = buffer;
        BufferLen = bufferLen;
    }

    /// <summary>`GM_DATA`（原文 `RunSock.pas`：消息类型常量）。</summary>
    public const int GM_DATA = 1;
}

/// <summary>
/// `ObjPlayer.pas:1485-3696` 的 `TPlayObject` 方法（切片 Core1）。
/// </summary>
public partial class TPlayObject
{
    // ==================================================================
    // 本切片需要的字段
    // （原文均属 `TPlayObject`，逐条对象文件 grep 确认托管侧无同名成员）
    // ==================================================================

    /// <summary>
    /// ⚠ 本文件**不声明** `m_Abil` / `m_AbilNG`（避免与并行车道重复声明 → CS0102）：
    /// 前者由 <c>PlayerSurface.Core3.cs:392</c> 以 `public ref TAbility m_Abil =&gt; ref m_wAbil;`
    /// 提供（引用别名，本文件一律按原文写 `m_Abil`，落到与 `m_wAbil` 同一块存储）；
    /// 后者由 <c>PlayerSurface.Core3.cs:351</c> 声明（`public TAbilityNG m_AbilNG;`）。
    /// </summary>
    /// <summary>原文 `m_boTrainingNG: Boolean;`（ObjPlayer.pas:8864 等使用）—— 是否修炼内功。</summary>
    public bool m_boTrainingNG;

    /// <summary>原文 `m_dwGetExp: LongWord;`（ObjPlayer.pas:344）—— 最近一次获得的经验（供脚本读）。</summary>
    public uint m_dwGetExp;

    /// <summary>原文 `m_dwBeadExp: LongWord;`（`@GetBeadExp` 脚本事件期间的暂存经验）。</summary>
    public uint m_dwBeadExp;

    /// <summary>原文 `m_nBeadSource: Integer;`（同上的来源标记）。</summary>
    public int m_nBeadSource;

    /// <summary>原文 `m_nKillMonExpMultiple: Integer;`（人物指定的杀怪经验倍数）。
    /// 原文 :2641 用 `LongWord(m_nKillMonExpMultiple)` 先做**有符号→无符号**转换再相乘。</summary>
    public int m_nKillMonExpMultiple;

    /// <summary>原文 `m_nKillMonExpRate: Integer;`（NPC 指定的杀怪经验百分比倍数）。</summary>
    public int m_nKillMonExpRate;

    /// <summary>原文 `m_SuiteExpMultiple: Integer;`（套装经验百分比倍数）。</summary>
    public int m_SuiteExpMultiple;

    /// <summary>原文 `m_rExpItem: Double;`（物品经验倍率，原文 :2662 `Round(m_rExpItem * dwExp)`）。</summary>
    public double m_rExpItem;

    /// <summary>原文 `m_boExpFromFromHero: Boolean; // 经验来源是英雄`（ObjPlayer.pas:408）。</summary>
    public bool m_boExpFromFromHero;

    /// <summary>原文 `FSoftVersionDateEx: Integer;`（ObjPlayer.pas:522 声明；
    /// 注释「版本号成员变量从 TPlayObject 修改至父类 TBaseObject piaoyun 2013-08-06」
    /// —— 原文如此，字段声明仍在 TPlayObject 的 published 段里）。</summary>
    public int FSoftVersionDateEx;

    // ---- 聊天 / 交易会话 ----

    /// <summary>原文 `m_BlockWhisperList: TStringList; // 0x280 禁止私聊人员列表`（ObjPlayer.pas:68）。</summary>
    public readonly List<string> m_BlockWhisperList = new();

    /// <summary>原文 `m_boHearWhisper: Boolean; // 0x27C 允许私聊`（ObjPlayer.pas:63）。</summary>
    public bool m_boHearWhisper;

    /// <summary>原文 `m_GetWhisperHuman: TPlayObject;`（ObjPlayer.pas:220）—— 私聊监听者。</summary>
    public TPlayObject? m_GetWhisperHuman;

    // ------------------------------------------------------------------
    // ⚠ 本文件**不声明**交易三字段（`m_boDealing` :31 / `m_DealCreat` :33 / `m_DealLastTick` :32）
    //   —— 它们由并行车道 `PlayerSurface.Core3.cs` 声明（`m_boDealing`/`m_DealLastTick` :289-296 一带
    //   与 `m_DealItemList` :297 / `m_nDealGolds` :300 / `m_boDealOK` :303）。
    //   本切片只在 `Whisper` 与 `IsGroupMember` 面读写自己声明的那些字段。
    // ------------------------------------------------------------------

    // ---- 组队 ----

    /// <summary>原文 `m_GroupOwner: TPlayObject; // 0x274`（ObjPlayer.pas:61）—— 队长。</summary>
    public TPlayObject? m_GroupOwner;

    /// <summary>
    /// 原文 `m_GroupMembers: TSafeStringList; // 0x278 组成员`（ObjPlayer.pas:62）。
    /// 原文按 `m_GroupMembers.Objects[I]`（**对象指针**）判成员身份（:3414），
    /// 字符串内容不参与 `IsGroupMember`，故托管侧只保留对象面：`List&lt;TPlayObject&gt;`。
    /// </summary>
    public readonly List<TPlayObject> m_GroupMembers = new();

    // ---- 仓库 ----

    /// <summary>原文 `m_StorageItemList: array [0 .. 3] of TList; // 仓库1-4`（ObjPlayer.pas:45）。</summary>
    public readonly List<TUserItem?>[] m_StorageItemList =
    {
        new(), new(), new(), new(),
    };

    /// <summary>原文 `m_BigStorageItemList: TList; // 无限仓库`（ObjPlayer.pas:47）。</summary>
    public readonly List<TUserItem?> m_BigStorageItemList = new();

    /// <summary>原文 `m_nInfinityStorageExtCount: Integer; // 个人无限仓库扩展数量`（ObjPlayer.pas:441）。</summary>
    public int m_nInfinityStorageExtCount;

    // ---- 客户端信息 / 状态 ----

    /// <summary>原文 `m_AssessHeroInfos: array [0 .. 1] of TStorageHeroInfo;`（ObjPlayer.pas:389）。
    /// 元素类型复用 `GXX.Core.Protocol.TStorageHeroInfo`（`Grobal2.Types5.cs:1005`，1:1）。</summary>
    public TStorageHeroInfo[] m_AssessHeroInfos = new TStorageHeroInfo[2];

    /// <summary>原文 `m_boInFreePKArea: Boolean;`（自由 PK 区标志）。</summary>
    public bool m_boInFreePKArea;

    /// <summary>原文 `m_dwHearMsgColorTick: LongWord;`（ObjPlayer.pas:364）—— 公聊颜色覆盖的有效期。</summary>
    public uint m_dwHearMsgColorTick;

    /// <summary>原文 `m_btHearMsgColor: Byte;`（ObjPlayer.pas:363）—— 公聊字体颜色。</summary>
    public byte m_btHearMsgColor;

    /// <summary>原文 `m_nOffOnlineTick: LongWord;`（ObjPlayer.pas:238）—— 离线挂机起点（原文 :1615
    /// 初始化为 `MyGetTickCount`）。</summary>
    public uint m_nOffOnlineTick = PlayerSurfaceTick.MyGetTickCount();

    /// <summary>原文 `m_boSendNotice: Boolean; // 0x6A9`（ObjPlayer.pas:138）。</summary>
    public bool m_boSendNotice;

    /// <summary>原文 `m_dwWaitLoginNoticeOKTick: LongWord;`（ObjPlayer.pas:139）。
    /// ⚠ 本切片 `RunNotice` **整条留痕**（见其 XML doc），该字段与 `m_boSendNotice`
    /// 一起按原文声明，供将来接通 `RunNotice` 时直接使用。</summary>
    public uint m_dwWaitLoginNoticeOKTick;

    // ==================================================================
    // WinExp（原文 2634-2672）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.WinExp(dwExp: LongWord);`（ObjPlayer.pas:2634-2672）。
    ///
    /// 逐行语义（全部照抄）：
    /// <list type="number">
    ///   <item><description>:2636 `if dwExp > 0 then` —— 唯一门控，0 直接整条跳过。</description></item>
    ///   <item><description>:2639 `dwExp := g_Config.dwKillMonExpMultiple * dwExp;`
    ///     —— **系统**杀怪倍数（`LongWord` × `LongWord`，32 位回绕）。</description></item>
    ///   <item><description>:2641 `dwExp := LongWord(m_nKillMonExpMultiple) * dwExp;`
    ///     —— **人物**倍数（原文先把**有符号** `Integer` 转 `LongWord`）。</description></item>
    ///   <item><description>:2644-2656 套装/NPC 百分比：`m_nKillMonExpRate > 0` 时
    ///     取 `Max(m_SuiteExpMultiple, m_nKillMonExpRate)` 一路；否则仅当
    ///     `m_SuiteExpMultiple > 0` 才乘。⚠ 原文 :2648/:2650/:2655 的 `Round(...)`
    ///     是 **Delphi 银行家舍入**（`.5 → 偶数`）→ `M2ShareFuncs.DelphiRound`。</description></item>
    ///   <item><description>:2658-2659 地图倍率：`m_PEnvir.m_boEXPRATE` 为真时乘
    ///     `m_PEnvir.m_nEXPRATE / 100`（**浮点 `/`，不是 `div`**）。</description></item>
    ///   <item><description>:2661-2662 物品倍率：`m_boExpItem` 为真时乘 `m_rExpItem`（Double）。</description></item>
    ///   <item><description>:2664-2669 **人物杀的怪**才做高等级封顶：
    ///     `not m_boExpFromFromHero` 且 `m_Abil.Level >= g_Config.nHighLevel` 时
    ///     `dwExp := Max(g_Config.nHighLevelGetExp, 0)`。</description></item>
    ///   <item><description>:2670 `GetExp(GetLevelExpRate(dwExp))` —— 走
    ///     `TBaseObject.GetLevelExpRate`（ObjBase.pas:41976-41985）后再进 `GetExp`
    ///     （后三参用默认值 `FromNPC = False` / `IsIncBead = True` / `IsFromChangeExp = False`）。</description></item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// ★ 原文缺陷（原文 2639-2641，逐字保留）：两步乘法都是 **32 位无符号乘法**，
    /// 原文**不做任何溢出检查**（既不提升到 Int64 也不夹 Upper bound）——
    /// 只要 `dwExp * 倍数` 超过 `2^32`，结果就会**静默回绕**成一个小值，
    /// 玩家反而**少拿**经验。托管侧用 `unchecked` 复刻该回绕
    /// （锁定用例 `ObjPlayerCore1Tests.WinExp_KillMonMultipleWrapsAround_OriginalDefect`）。
    /// 同族的 `GetExpNG`（:2955）却对同类乘积用了 `abs(Round(...))` —— 两处**不一致**，
    /// 逐字保留差异，不做统一。
    /// </remarks>
    public void WinExp(uint dwExp)
    {
        // 原文 2636：if dwExp > 0 then
        if (dwExp > 0)
        {
            // 原文 2639：dwExp := g_Config.dwKillMonExpMultiple * dwExp;
            //   ★ 原文缺陷：LongWord × LongWord 的 32 位回绕，无溢出检查。
            dwExp = unchecked(M2Config.dwKillMonExpMultiple * dwExp);

            // 原文 2641：dwExp := LongWord(m_nKillMonExpMultiple) * dwExp;
            dwExp = unchecked((uint)m_nKillMonExpMultiple * dwExp);

            // 原文 2644-2656：套装 / NPC 百分比倍数
            if (m_nKillMonExpRate > 0)
            {
                // 原文 2647-2650
                if (m_SuiteExpMultiple > 0 && m_SuiteExpMultiple > m_nKillMonExpRate)
                    dwExp = unchecked((uint)M2ShareFuncs.DelphiRound(m_SuiteExpMultiple / 100.0 * dwExp));
                else
                    dwExp = unchecked((uint)M2ShareFuncs.DelphiRound(m_nKillMonExpRate / 100.0 * dwExp));
            }
            else
            {
                // 原文 2654-2655
                if (m_SuiteExpMultiple > 0)
                    dwExp = unchecked((uint)M2ShareFuncs.DelphiRound(m_SuiteExpMultiple / 100.0 * dwExp));
            }

            // 原文 2658-2659：if m_PEnvir.m_boEXPRATE then dwExp := Round((m_PEnvir.m_nEXPRATE / 100) * dwExp);
            //   接缝：托管 `TEnvirnoment`（Engine/Envir.cs）无 m_boEXPRATE / m_nEXPRATE。
            uint mapRate = 0;
            if (PlayerSurfaceCore1Seams.MapExpRate(this, out mapRate))
                dwExp = unchecked((uint)M2ShareFuncs.DelphiRound(mapRate / 100.0 * dwExp));

            // 原文 2661-2662：if m_boExpItem then dwExp := Round(m_rExpItem * dwExp);
            if (m_boExpItem)
                dwExp = unchecked((uint)M2ShareFuncs.DelphiRound(m_rExpItem * dwExp));

            // 原文 2664-2669：人物杀的怪，先计算最高经验限制
            if (!m_boExpFromFromHero)
            {
                // 原文 2667-2668：if m_Abil.Level >= g_Config.nHighLevel then dwExp := Max(g_Config.nHighLevelGetExp, 0);
                if (m_Abil.Level >= (uint)M2Config.nHighLevel)
                    dwExp = (uint)Math.Max(M2Config.nHighLevelGetExp, 0);
            }

            // 原文 2670：GetExp(GetLevelExpRate(dwExp));
            GetExp(GetLevelExpRate(dwExp));
        }
    }

    // ==================================================================
    // GetLevelExpRate（原文 ObjBase.pas:41976-41985）—— WinExp 的唯一调用点
    // ==================================================================

    /// <summary>
    /// 原文 `function TBaseObject.GetLevelExpRate(dwExp: LongWord): LongWord;`
    /// （**声明 ObjBase.pas:556，实现 ObjBase.pas:41976-41985**）。
    ///
    /// <para><b>为什么落在本文件</b>：这是 `WinExp`（ObjPlayer.pas:2670）的**唯一**调用点，
    /// 而托管侧全仓（`grep -rn "GetLevelExpRate" src/`）**只有本文件命中** ——
    /// 即该方法**尚未被任何切片移植**。按任务书第 4/5 条（缺的常量/成员可在本文件声明并注明原文名），
    /// 在此按原文 1:1 落地；`m_Abil` 走 Core3 的 `ref` 别名，故与 `m_WAbil.Level` 同源。
    /// ⚠ 托管侧本方法落在 `TPlayObject` 而原文在 `TBaseObject`（`TCreature` 那层）——
    /// 与 `GetLevelExp`（落在 `AbilRecalc` 静态类）一样属于**已登记的层级偏差**；
    /// 若日后 `TCreature` 上出现同名成员，需删除此处并改为继承（否则 `TPlayObject` 会**隐藏**基类版本）。</para>
    ///
    /// <code>
    ///   Result := dwExp;
    ///   if (m_Abil.Level > 0)                                     // 41980
    ///     and (m_Abil.Level &lt; MAXCHANGELEVEL)                     // 41981
    ///     and (dwExp &gt; 0)                                        // 41982
    ///     and (g_Config.LevelExpRates[m_Abil.Level] &gt; 0) then     // 41983
    ///     Result := Round(dwExp / 100 * g_Config.LevelExpRates[m_Abil.Level]);  // 41984
    /// </code>
    ///
    /// ★ 四个条件**全部**要成立才生效（`and` 短路）：等级 0、等级 ≥ 1000（`MAXCHANGELEVEL`）、
    /// 经验 0、该等级倍率为 0 —— 任一不成立都**原样返回 `dwExp`**。
    /// ⚠ :41984 的 `dwExp / 100` 是 Delphi 的**浮点除法**（`/`，不是 `div`），
    /// 且 `Round` 是**银行家舍入** → `M2ShareFuncs.DelphiRound`。
    /// </summary>
    /// <param name="dwExp">原文 `dwExp: LongWord`。</param>
    /// <returns>原文 `Result`。</returns>
    public uint GetLevelExpRate(uint dwExp)
    {
        // 原文 41978：Result := dwExp;
        uint result = dwExp;

        // 原文 41980-41984
        if (m_Abil.Level > 0
            && m_Abil.Level < (uint)AbilRecalc.MAXCHANGELEVEL
            && dwExp > 0
            && M2Config.LevelExpRates[m_Abil.Level] > 0)
        {
            // 原文 41984：Result := Round(dwExp / 100 * g_Config.LevelExpRates[m_Abil.Level]);
            result = unchecked((uint)M2ShareFuncs.DelphiRound(dwExp / 100.0 * M2Config.LevelExpRates[m_Abil.Level]));
        }

        // 原文 41985：end;（隐式返回 Result）
        return result;
    }

    // ==================================================================
    // GetExp（原文 2674-2835）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.GetExp(dwExp: LongWord; IsAdjustHero: Boolean = True;
    /// FromNPC: Boolean = False; IsIncBead: Boolean = True; IsFromChangeExp: Boolean = False);`
    /// （声明 ObjPlayer.pas:1203-1204，实现 :2674-2835）——**人物经验结算主入口**。
    ///
    /// 逐段（全部照抄）：
    /// <list type="number">
    ///   <item><description>:2686-2691 等级上限三档：`g_Config.btMaxLevel = 0 → MAXUPLEVEL`；
    ///     `= 1 → High(Integer)`；否则 `High(LongWord)`。</description></item>
    ///   <item><description>:2695-2702 **英雄异地**：`m_MyHero &lt;&gt; nil` 且经验来源是英雄
    ///     且 `not g_Config.boShareExpHeroSameMap` 且两 `m_PEnvir` 不等 → 经验**全给英雄**并 `Exit`。</description></item>
    ///   <item><description>:2704-2745 `IsAdjustHero` 分经验：英雄未死且同图（或开了同图开关）
    ///     时按 `nHeroNotKillMonExpRate`（NPC 来源）/ `nHeroKillMonExpRate` 分给英雄，
    ///     再按 `boHumanGetAllExp` 决定人物是否**拿全部**（拿全部时 `lwExp := dwExp` —— ★ 见缺陷）。</description></item>
    ///   <item><description>:2748-2752 **人物**高等级封顶：`not IsFromChangeExp` 且
    ///     `m_Abil.Level >= g_Config.nHighLevel` → `dwExp := Max(nHighLevelGetExp, 0)`、
    ///     `lwExp := Min(lwExp, nHighLevelGetExp)`。</description></item>
    ///   <item><description>:2757-2806 **`RefExp` 升级循环**：见下方「★ 控制流逐字保留」。</description></item>
    ///   <item><description>:2808 `m_dwGetExp := dwExp`；:2810-2811 `IsIncBead` 时 `IncBeadExp(dwExp, FromNPC)`。</description></item>
    ///   <item><description>:2813 `SendMsg(Self, RM_WINEXP, 0, dwExp, 0, 0, '')`；
    ///     :2814-2823 `g_FunctionNPC &lt;&gt; nil` 时两次 `GotoLable`（`@GetExp`；非 NPC 来源再来一次
    ///     `@KillMonGetExp`），两次前都把 `m_nScriptGotoCount` 归零。</description></item>
    ///   <item><description>:2826-2834 升过级则：`AddGameDataLog(LOG_LevelChange, LOG_ActionNone, Self, '等级', 0, '0', 新等级, 旧等级)`
    ///     → `RecalcLevelAbilitys(False)` → `RecalcAbilitys()` → `SendRefMsg(RM_LEVELUP, ...)`
    ///     → `LevelUpFunc()` → `SendRefMsg(RM_HEALTHSPELLCHANGED, ...)`。</description></item>
    /// </list>
    ///
    /// ★★ **控制流逐字保留（原文 2760-2806，本方法最关键的一处）**：
    /// 原文把 `RefExp:` 标签**放在 `if ... end` 与 `else` 之间**：
    /// <code>
    ///   if (lwExp &gt;= dwAddExp) and (m_Abil.MaxExp &gt; m_Abil.Exp) then
    ///   begin
    ///     if m_Abil.Level &lt; nMaxLevel then
    ///     begin
    ///       ...
    ///       if lwExp &gt; 0 then
    ///       begin
    ///         if nMaxUpLevelCount &lt; g_Config.nMaxUpLevelCount then
    ///         begin goto RefExp; Exit; end
    ///         else Inc(m_Abil.Exp, lwExp);
    ///       end;
    ///     end;
    ///   end
    ///   else
    ///   begin ... Inc(m_Abil.Level); ... if nMaxUpLevelCount &lt; ... then begin goto RefExp; Exit; end; end;
    /// </code>
    /// 其中 **`Exit` 紧跟在 `goto` 之后** —— `goto` 一旦执行，`Exit` **永远不会被执行**，
    /// 于是**跳过整个 `else` 分支**、直接落到 :2808 的收尾段（发经验、脚本事件、升级后处理）。
    /// 也就是说：**升到 `nMaxUpLevelCount` 上限的那一次，收尾段照常执行**；
    /// 而在**不**触顶的常规升级路径上（`goto` 执行），`else` 分支里的
    /// "`Inc(m_Abil.Level); Dec(m_Abil.Exp, m_Abil.MaxExp);`" 是**死代码**（永远走不到）。
    /// 托管侧用 `while(true)` + `continue` 复刻"跳回 2757"，并把 `else` 分支写成
    /// **循环退出之后的顺序代码**（仅 `while` 因 `lwExp &lt; dwAddExp` 或
    /// `MaxExp &lt;= Exp` 自然结束、且 `goto` 未执行时才可达）—— 两条路径的**可达性完全一致**。
    ///
    /// ★ 原文缺陷（原文 2716 / 2736，逐字保留）：`boHumanGetAllExp` 为真时
    /// `lwExp := dwExp` —— 而按 :2712/:2732 的语义，`lwExp` 此刻**已经是被扣掉的那部分**，
    /// 重新赋成 `dwExp` 之后**没有**任何后续代码把它写回 `m_Abil.Exp` 的增量来源
    /// （:2759 的 `dwAddExp` 只依赖 `m_Abil`），所以"人物拿全部"这条分支的
    /// `dwExp` 归位实际上只影响 `m_dwGetExp`/`IncBeadExp`/`RM_WINEXP`，**不影响实际加的经验**
    /// —— 与 `IncExp` 的写法（没有 `boHumanGetAllExp` 分支）不一致。逐字保留。
    ///
    /// ★ 原文缺陷（原文 2665-2668 的 `TODO` 注释与其实现）：注释写
    /// 「英雄1000以后使用固定经验」，但该 `if` 判的是**人物** `m_Abil.Level`
    /// （英雄等级在 `m_MyHero` 上），注释与代码不符 —— 原文如此，保留注释。
    /// </remarks>
    /// <param name="dwExp">原文 `dwExp: LongWord`。</param>
    /// <param name="isAdjustHero">原文 `IsAdjustHero: Boolean = True`（注意原文与实现文件的默认值都是 True）。</param>
    /// <param name="fromNPC">原文 `FromNPC: Boolean = False`。</param>
    /// <param name="isIncBead">原文 `IsIncBead: Boolean = True`。</param>
    /// <param name="isFromChangeExp">原文 `IsFromChangeExp: Boolean = False`。</param>
    public void GetExp(uint dwExp, bool isAdjustHero = true, bool fromNPC = false,
        bool isIncBead = true, bool isFromChangeExp = false)
    {
        // 原文 2677-2682：过程级局部变量
        uint lwExp;
        uint nMaxLevel;

        // 原文 2686-2691
        if (M2Config.btMaxLevel == 0)
            nMaxLevel = (uint)AbilRecalc.MAXUPLEVEL;
        else if (M2Config.btMaxLevel == 1)
            nMaxLevel = int.MaxValue;
        else
            nMaxLevel = uint.MaxValue;

        // 原文 2693：lwExp := dwExp;
        lwExp = dwExp;

        // 原文 2695-2702：英雄异地 → 经验全分给英雄
        if (m_MyHero != null
            && m_boExpFromFromHero
            && !M2Config.boShareExpHeroSameMap
            && m_MyHero.m_PEnvir != m_PEnvir)
        {
            // 原文 2700：THeroObject(m_MyHero).GetExp(lwExp, False, IsIncBead);
            PlayerSurfaceCore1Seams.HeroGetExp(m_MyHero, lwExp, false, isIncBead);
            // 原文 2701：Exit;
            return;
        }

        // 原文 2704-2745：IsAdjustHero 分经验
        if (isAdjustHero)
        {
            if (m_MyHero != null
                && !m_MyHero.m_boDeath
                && (M2Config.boShareExpHeroSameMap || m_MyHero.m_PEnvir == m_PEnvir))
            {
                if (fromNPC)
                {
                    // 原文 2712：lwExp := Round(dwExp * (g_Config.nHeroNotKillMonExpRate / 100));
                    lwExp = unchecked((uint)M2ShareFuncs.DelphiRound(dwExp * (M2Config.nHeroNotKillMonExpRate / 100.0)));
                    // 原文 2713：THeroObject(m_MyHero).GetExp(lwExp, FromNPC, IsIncBead);
                    PlayerSurfaceCore1Seams.HeroGetExp(m_MyHero, lwExp, fromNPC, isIncBead);
                    // 原文 2715-2721
                    if (M2Config.boHumanGetAllExp)
                        lwExp = dwExp;                       // 原文 2716
                    else
                    {
                        lwExp = dwExp - lwExp;               // 原文 2719
                        dwExp = lwExp;                       // 原文 2720
                    }
                }
                else
                {
                    // 原文 2725-2742
                    if (M2Config.boHeroGetAllExp)
                    {
                        lwExp = dwExp;                       // 原文 2727
                        PlayerSurfaceCore1Seams.HeroGetExp(m_MyHero, lwExp, false, isIncBead);  // 原文 2728
                    }
                    else
                    {
                        lwExp = unchecked((uint)M2ShareFuncs.DelphiRound(dwExp * (M2Config.nHeroKillMonExpRate / 100.0))); // 原文 2732
                        PlayerSurfaceCore1Seams.HeroGetExp(m_MyHero, lwExp, false, isIncBead);  // 原文 2733
                        if (M2Config.boHumanGetAllExp)
                            lwExp = dwExp;                   // 原文 2736
                        else
                        {
                            lwExp = dwExp - lwExp;           // 原文 2739
                            dwExp = lwExp;                   // 原文 2740
                        }
                    }
                }
            }
        }

        // 原文 2748-2752：人物经验受最高经验限制
        if (!isFromChangeExp && m_Abil.Level >= (uint)M2Config.nHighLevel)
        {
            dwExp = (uint)Math.Max(M2Config.nHighLevelGetExp, 0);          // 原文 2750
            lwExp = Math.Min(lwExp, (uint)Math.Max(M2Config.nHighLevelGetExp, 0)); // 原文 2751
        }

        // 原文 2754-2756：nMaxUpLevelCount := 0; IsLeveUped := False; OldLevel := m_Abil.Level;
        int nMaxUpLevelCount = 0;
        bool isLeveUped = false;
        uint oldLevel = m_Abil.Level;

        // 原文 2757：RefExp:   ← goto 目标
        //   ★ 用 while(true) + continue 复刻 `goto RefExp`；`continue` 与 `goto` 一样
        //     **跳过紧随其后的 `Exit`**（原文 2777/2803），故收尾段只在
        //     "循环自然结束"或"else 分支走到末尾"时执行 —— 与原文字节级可达性一致。
        while (true)
        {
            // 原文 2758：Inc(nMaxUpLevelCount);
            nMaxUpLevelCount++;
            // 原文 2759：dwAddExp := m_Abil.MaxExp - m_Abil.Exp;
            uint dwAddExp = unchecked(m_Abil.MaxExp - m_Abil.Exp);

            // 原文 2760：if (lwExp >= dwAddExp) and (m_Abil.MaxExp > m_Abil.Exp) then
            if (lwExp >= dwAddExp && m_Abil.MaxExp > m_Abil.Exp)
            {
                // 原文 2762：if m_Abil.Level < nMaxLevel then
                if (m_Abil.Level < nMaxLevel)
                {
                    m_Abil.Level++;                     // 原文 2764：Inc(m_Abil.Level);
                    lwExp = unchecked(lwExp - dwAddExp); // 原文 2765：lwExp := lwExp - dwAddExp;
                    m_Abil.Exp = 0;                     // 原文 2766
                    // 原文 2767：AddBodyLuck(dwAddExp * 0.002);
                    PlayerSurfaceCore1Seams.AddBodyLuck(this, dwAddExp * 0.002);
                    isLeveUped = true;                  // 原文 2768
                    // 原文 2769：// HasLevelUp(m_Abil.Level - 1, True, False);   ← 原文注释掉
                    m_Abil.MaxExp = AbilRecalc.GetLevelExp(this, m_Abil.Level);  // 原文 2770
                    // 原文 2771：IncHealthSpell(2000, 2000, False);
                    PlayerSurfaceCore1Seams.IncHealthSpell(this, 2000, 2000, false);
                    // 原文 2772-2781
                    if (lwExp > 0)
                    {
                        if (nMaxUpLevelCount < M2Config.nMaxUpLevelCount)
                        {
                            // 原文 2776-2777：goto RefExp; Exit;
                            continue;
                        }
                        else
                            m_Abil.Exp = unchecked(m_Abil.Exp + lwExp);   // 原文 2780
                    }
                }
            }
            else
            {
                // 原文 2784-2806：else 分支
                if (m_Abil.MaxExp > m_Abil.Exp)
                {
                    // 原文 2788-2789
                    m_Abil.Exp = unchecked(m_Abil.Exp + lwExp);
                    PlayerSurfaceCore1Seams.AddBodyLuck(this, lwExp * 0.002);
                }
                else
                {
                    // 原文 2793-2804
                    m_Abil.Level++;                                       // 原文 2793
                    m_Abil.Exp = unchecked(m_Abil.Exp - m_Abil.MaxExp);   // 原文 2794：Dec(m_Abil.Exp, m_Abil.MaxExp);
                    isLeveUped = true;                                    // 原文 2795
                    // 原文 2797：// HasLevelUp(m_Abil.Level - 1, True, False);   ← 原文注释掉
                    m_Abil.MaxExp = AbilRecalc.GetLevelExp(this, m_Abil.Level);  // 原文 2798
                    // 原文 2799：IncHealthSpell(2000, 2000, False);
                    PlayerSurfaceCore1Seams.IncHealthSpell(this, 2000, 2000, false);
                    if (nMaxUpLevelCount < M2Config.nMaxUpLevelCount)
                    {
                        // 原文 2802-2803：goto RefExp; Exit;
                        continue;
                    }
                }
            }

            // 走到这里 = 原文 `goto` 未执行（else 分支末尾）或提前退出循环体后落回
            //   —— 与原文 :2806 的 `end;` 之后完全一致。
            break;
        }

        // 原文 2808：m_dwGetExp := dwExp;
        m_dwGetExp = dwExp;
        // 原文 2809-2811：聚灵珠加经验
        if (isIncBead)
            IncBeadExp(dwExp, fromNPC);   // 原文 2811

        // 原文 2813：SendMsg(Self, RM_WINEXP, 0, dwExp, 0, 0, '');
        PlayerSurfaceCore1Seams.SendMsg(this, Grobal2Const.RM_WINEXP, 0, dwExp, 0, 0, "");

        // 原文 2814-2823：功能 NPC 事件
        if (PlayerSurfaceItemSeams.FunctionNPC != null)
        {
            // 原文 2816-2817：m_nScriptGotoCount := 0; g_FunctionNPC.GotoLable(Self, '@GetExp', False);
            m_nScriptGotoCount = 0;
            PlayerSurfaceItemSeams.FunctionNpcGotoLable(
                PlayerSurfaceItemSeams.FunctionNPC, this, "@GetExp", false);
            // 原文 2818-2822
            if (!fromNPC)
            {
                m_nScriptGotoCount = 0;
                PlayerSurfaceItemSeams.FunctionNpcGotoLable(
                    PlayerSurfaceItemSeams.FunctionNPC, this, "@KillMonGetExp", false);
            }
        }

        // 原文 2826-2834：升级后处理
        if (isLeveUped)
        {
            // 原文 2828：AddGameDataLog(LOG_LevelChange, LOG_ActionNone, Self, '等级', 0, '0', m_Abil.Level, OldLevel);
            PlayerSurfaceCore1Seams.AddGameDataLog8(PlayerSurfaceLogActionConst.LOG_LevelChange, GXX.M2Server.Npc.ObjNpcConst.LOG_ActionNone,
                this, "等级", 0, "0", (int)m_Abil.Level, (int)oldLevel);
            // 原文 2829：RecalcLevelAbilitys(False);
            this.RecalcLevelAbilitys(false);
            // 原文 2830：RecalcAbilitys();
            RecalcAbilitys();
            // 原文 2831：SendRefMsg(RM_LEVELUP, m_btDirection, m_nCurrX, m_nCurrY, 0, '');
            SendRefMsg(Grobal2Const.RM_LEVELUP, m_btDirection, m_nCurrX, m_nCurrY, 0, "", 0);
            // 原文 2832：LevelUpFunc();
            PlayerSurfaceCore1Seams.LevelUpFunc(this);
            // 原文 2833：SendRefMsg(RM_HEALTHSPELLCHANGED, 0, 0, 0, 0, ''); // 刷新HP
            SendRefMsg(Grobal2Const.RM_HEALTHSPELLCHANGED, 0, 0, 0, 0, "", 0);
        }
    }

    // ==================================================================
    // IncExp（原文 2837-2922）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.IncExp(dwExp: LongWord);`（ObjPlayer.pas:2837-2922）。
    ///
    /// 与 <see cref="GetExp"/> 的差异（逐条照抄，**不做统一**）：
    /// <list type="bullet">
    ///   <item><description>**没有**英雄分经验段、**没有** `boHumanGetAllExp`/`boHeroGetAllExp` 分支。</description></item>
    ///   <item><description>:2866 / :2894 调 `HasLevelUp(m_Abil.Level - 1)`（**只传 1 个实参**，
    ///     另两个默认值补齐）—— 与 `GetExp` 里**被注释掉的**同调用形成对照。</description></item>
    ///   <item><description>:2771/:2799 的 `IncHealthSpell(2000, 2000, False)` 在这里写的是
    ///     `IncHealthSpell(2000, 2000)`（**两参**，第三参用默认值 True）—— 逐字保留。</description></item>
    ///   <item><description>加经验一律走 `Int64Value := Int64(m_Abil.Exp) + lwExp;
    ///     m_Abil.Exp := Min(Int64Value, High(LongWord));`（:2877-2878 / :2887-2888 / :2906-2907），
    ///     即**先提升到 Int64 再夹上限**，与 `GetExp` 的 `Inc(m_Abil.Exp, lwExp)`（32 位回绕）**不同**。</description></item>
    ///   <item><description>:2913-2918 `g_FunctionNPC` 只发一次 `@GetExp`（`GetExp` 还发 `@KillMonGetExp`），
    ///     且 `m_dwGetExp := dwExp` **写在 `if g_FunctionNPC &lt;&gt; nil` 里面**（:2915），
    ///     与 `GetExp` 的 :2808（无条件）不同 —— 逐字保留。</description></item>
    ///   <item><description>:2920-2921 尾部判定写的是 `if OldLevel &lt;&gt; m_Abil.Level`
    ///     （**依赖等级变化**），而 `GetExp` 用的是 `IsLeveUped` 标志 —— 逐字保留。</description></item>
    /// </list>
    ///
    /// ★ 与 `GetExp` 同样的 `RefExp:` 标签位置缺陷（原文 2856-2900，
    /// `goto RefExp; Exit;` 使 `else` 分支在触顶前不可达）—— 托管侧用同样的
    /// `while(true)` + `continue` 复刻，见 <see cref="GetExp"/> 的详细说明。
    /// </remarks>
    public void IncExp(uint dwExp)
    {
        uint lwExp;
        uint nMaxLevel;
        uint oldLevel;

        // 原文 2847-2852
        if (M2Config.btMaxLevel == 0)
            nMaxLevel = (uint)AbilRecalc.MAXUPLEVEL;
        else if (M2Config.btMaxLevel == 1)
            nMaxLevel = int.MaxValue;
        else
            nMaxLevel = uint.MaxValue;

        // 原文 2853-2855：lwExp := dwExp; nMaxUpLevelCount := 0; OldLevel := m_Abil.Level;
        lwExp = dwExp;
        int nMaxUpLevelCount = 0;
        oldLevel = m_Abil.Level;

        // 原文 2856：RefExp:
        while (true)
        {
            // 原文 2857：Inc(nMaxUpLevelCount);
            nMaxUpLevelCount++;
            // 原文 2858：dwAddExp := m_Abil.MaxExp - m_Abil.Exp;
            uint dwAddExp = unchecked(m_Abil.MaxExp - m_Abil.Exp);

            // 原文 2859
            if (lwExp >= dwAddExp && m_Abil.MaxExp > m_Abil.Exp)
            {
                // 原文 2861
                if (m_Abil.Level < nMaxLevel)
                {
                    m_Abil.Level++;                       // 原文 2863
                    lwExp = unchecked(lwExp - dwAddExp);  // 原文 2864
                    m_Abil.Exp = 0;                       // 原文 2865
                    // 原文 2866：HasLevelUp(m_Abil.Level - 1);
                    PlayerSurfaceCore1Seams.HasLevelUp(this, (int)(m_Abil.Level - 1));
                    // 原文 2867：IncHealthSpell(2000, 2000);
                    //   ⚠ 原文只传两参（第三参默认 True）—— 与 :2771 的三参写法不同，逐字保留。
                    PlayerSurfaceCore1Seams.IncHealthSpell(this, 2000, 2000, true);
                    // 原文 2868-2880
                    if (lwExp > 0)
                    {
                        if (nMaxUpLevelCount < M2Config.nMaxUpLevelCount)
                        {
                            // 原文 2873-2874：goto RefExp; Exit;
                            continue;
                        }
                        else
                        {
                            // 原文 2877-2878：Int64Value := Int64(m_Abil.Exp) + lwExp; m_Abil.Exp := Min(Int64Value, High(LongWord));
                            long int64Value = (long)m_Abil.Exp + lwExp;
                            m_Abil.Exp = (uint)Math.Min(int64Value, uint.MaxValue);
                        }
                    }
                }
            }
            else
            {
                // 原文 2883-2911
                if (m_Abil.MaxExp > m_Abil.Exp)
                {
                    // 原文 2887-2888
                    long int64Value = (long)m_Abil.Exp + lwExp;
                    m_Abil.Exp = (uint)Math.Min(int64Value, uint.MaxValue);
                }
                else
                {
                    // 原文 2892-2895
                    m_Abil.Level++;
                    m_Abil.Exp = unchecked(m_Abil.Exp - m_Abil.MaxExp);   // 原文 2893：Dec(m_Abil.Exp, m_Abil.MaxExp);
                    PlayerSurfaceCore1Seams.HasLevelUp(this, (int)(m_Abil.Level - 1));  // 原文 2894
                    PlayerSurfaceCore1Seams.IncHealthSpell(this, 2000, 2000, true);     // 原文 2895
                    if (nMaxUpLevelCount < M2Config.nMaxUpLevelCount)
                    {
                        // 原文 2898-2899：goto RefExp; Exit;
                        continue;
                    }
                    else
                    {
                        // 原文 2903-2908：升级完后，还有经验，加到人物身上
                        if (lwExp > 0)
                        {
                            // 原文 2906-2907
                            long int64Value = (long)m_Abil.Exp + lwExp;
                            m_Abil.Exp = (uint)Math.Min(int64Value, uint.MaxValue);
                        }
                    }
                }
            }

            break;
        }

        // 原文 2912：SendMsg(Self, RM_WINEXP, 0, dwExp, 0, 0, '');
        PlayerSurfaceCore1Seams.SendMsg(this, Grobal2Const.RM_WINEXP, 0, dwExp, 0, 0, "");

        // 原文 2913-2918
        if (PlayerSurfaceItemSeams.FunctionNPC != null)
        {
            // 原文 2915：m_dwGetExp := dwExp;   ← 注意：在 if 里面（与 GetExp:2808 不同）
            m_dwGetExp = dwExp;
            // 原文 2916-2917
            m_nScriptGotoCount = 0;
            PlayerSurfaceItemSeams.FunctionNpcGotoLable(
                PlayerSurfaceItemSeams.FunctionNPC, this, "@GetExp", false);
        }

        // 原文 2920-2921：if OldLevel <> m_Abil.Level then AddGameDataLog(...);
        if (oldLevel != m_Abil.Level)
        {
            PlayerSurfaceCore1Seams.AddGameDataLog8(PlayerSurfaceLogActionConst.LOG_LevelChange, GXX.M2Server.Npc.ObjNpcConst.LOG_ActionNone,
                this, "等级", 0, "0", (int)m_Abil.Level, (int)oldLevel);
        }
    }

    // ==================================================================
    // WinExpNG（原文 2924-2963）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.WinExpNG(dwExp: LongWord);`（ObjPlayer.pas:2924-2963）—— 内功版。
    ///
    /// 与 <see cref="WinExp"/> 的差异（逐条照抄）：
    /// <list type="bullet">
    ///   <item><description>:2926 门控是 **两个条件**：`(dwExp &gt; 0) and (m_boTrainingNG)`
    ///     —— 没学内功则**整条跳过**。</description></item>
    ///   <item><description>:2934 那句 `dwExp := Round((m_nKillMonExpRate / 100) * dwExp);` **被注释掉**（原文如此，保留为注释）。</description></item>
    ///   <item><description>:2955 多一步内功倍率：`dwExp := abs(Round(dwExp * g_Config.nNGKillMonExpMultiple / 100));`
    ///     —— **`abs` 是原文唯一的"防溢出"手段**，与 `WinExp` 的裸回绕不一致（见 <see cref="WinExp"/> 的缺陷说明）。</description></item>
    ///   <item><description>:2958 封顶判的是 **`m_AbilNG.Level &gt;= MAXNG_LEVEL`**（内功满级常量，
    ///     `M2Share.pas:203` = `High(Word)` = 65535），而不是 `g_Config.nHighLevel`。</description></item>
    ///   <item><description>:2961 `GetExpNG(dwExp)` —— **只传一个实参**，`IsAdjustHero` 用默认 True。</description></item>
    /// </list>
    /// </summary>
    public void WinExpNG(uint dwExp)
    {
        // 原文 2926：if (dwExp > 0) and (m_boTrainingNG) then
        if (dwExp > 0 && m_boTrainingNG)
        {
            // 原文 2929：dwExp := g_Config.dwKillMonExpMultiple * dwExp;
            dwExp = unchecked(M2Config.dwKillMonExpMultiple * dwExp);
            // 原文 2931：dwExp := LongWord(m_nKillMonExpMultiple) * dwExp;
            dwExp = unchecked((uint)m_nKillMonExpMultiple * dwExp);

            // 原文 2934：// dwExp := Round((m_nKillMonExpRate / 100) * dwExp);   ← 原文注释掉，保留
            if (m_nKillMonExpRate > 0)
            {
                // 原文 2938-2941
                if (m_SuiteExpMultiple > 0 && m_SuiteExpMultiple > m_nKillMonExpRate)
                    dwExp = unchecked((uint)M2ShareFuncs.DelphiRound(m_SuiteExpMultiple / 100.0 * dwExp));
                else
                    dwExp = unchecked((uint)M2ShareFuncs.DelphiRound(m_nKillMonExpRate / 100.0 * dwExp));
            }
            else
            {
                // 原文 2945-2946
                if (m_SuiteExpMultiple > 0)
                    dwExp = unchecked((uint)M2ShareFuncs.DelphiRound(m_SuiteExpMultiple / 100.0 * dwExp));
            }

            // 原文 2949-2950：地图倍率
            uint mapRate = 0;
            if (PlayerSurfaceCore1Seams.MapExpRate(this, out mapRate))
                dwExp = unchecked((uint)M2ShareFuncs.DelphiRound(mapRate / 100.0 * dwExp));

            // 原文 2952-2953：物品经验倍数
            if (m_boExpItem)
                dwExp = unchecked((uint)M2ShareFuncs.DelphiRound(m_rExpItem * dwExp));

            // 原文 2955：dwExp := abs(Round(dwExp * g_Config.nNGKillMonExpMultiple / 100));
            dwExp = unchecked((uint)Math.Abs(M2ShareFuncs.DelphiRound(dwExp * PlayerSurfaceCore1Const.nNGKillMonExpMultiple / 100.0)));

            // 原文 2956-2960
            if (!m_boExpFromFromHero)
            {
                // 原文 2958-2959
                if (m_AbilNG.Level >= PlayerSurfaceCore1Const.MAXNG_LEVEL)
                    dwExp = (uint)Math.Max(M2Config.nHighLevelGetExp, 0);
            }

            // 原文 2961：GetExpNG(dwExp);
            GetExpNG(dwExp);
        }
    }

    // ==================================================================
    // GetExpNG（原文 2965-3073）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.GetExpNG(dwExp: LongWord; IsAdjustHero: Boolean = True);`
    /// （声明 ObjPlayer.pas:1208，实现 :2965-3073）—— 内功经验结算。
    ///
    /// 逐段（全部照抄）：
    /// <list type="number">
    ///   <item><description>:2973-2976 **两处早退**：`not m_boTrainingNG` → `Exit`；`dwExp = 0` → `Exit`。</description></item>
    ///   <item><description>:2979-2985 英雄异地 → `THeroObject(m_MyHero).GetExpNG(lwExp)` 后 `Exit`。</description></item>
    ///   <item><description>:2986-3010 `IsAdjustHero` 分经验：**与 `GetExp` 有三处不同** ——
    ///     ① 英雄分支**没有** `FromNPC` 子分支；② `boHumanGetAllExp` 为假时才扣减
    ///     （`if not g_Config.boHumanGetAllExp then`，注意 `GetExp` 写的是 `if ... then lwExp := dwExp else ...`，
    ///     布尔极性相反但结果同为"人物不全拿时扣减"）；
    ///     ③ 英雄不在同图/已死时走 `else lwExp := dwExp;`（:3009）—— `GetExp` **没有**这个 else。</description></item>
    ///   <item><description>:3012-3016 内功满级封顶：`m_AbilNG.Level &gt;= MAXNG_LEVEL` 时
    ///     `dwExp := Max(nHighLevelGetExp, 0)` 且 **`lwExp := Max(lwExp, nHighLevelGetExp)`**
    ///     （注意：人物侧 :2751 用的是 `Min`，此处是 **`Max`** —— 原文如此，**两处相反**）。</description></item>
    ///   <item><description>:3018-3019 内功等级上限门：`m_AbilNG.Level &gt;= g_Config.nNGMaxLevelLimte` → `Exit`
    ///     （**直接退出，不加经验**）。</description></item>
    ///   <item><description>:3023-3066 升级循环（同样的 `RefExp:` 位置缺陷，见 <see cref="GetExp"/>）。</description></item>
    ///   <item><description>:3067 `m_dwGetExp := dwExp`；:3069 `SendMsg(Self, RM_WINEXP, 1, dwExp, 0, 0, '')`
    ///     —— **第三参是 1**（内功标识），与人物版的 0 不同。</description></item>
    ///   <item><description>:3071-3072 尾部 `if OldLevel &lt;&gt; m_AbilNG.Level then AddGameDataLog(..., '内功等级', ...)`。</description></item>
    /// </list>
    ///
    /// ★ 原文缺陷（原文 3012-3015，逐字保留）：`m_AbilNG.Level &gt;= MAXNG_LEVEL` 时把
    /// `lwExp` 抬到 `Max(..., nHighLevelGetExp)` —— 与人物侧 :2751 的 `Min(...)` **方向相反**；
    /// 内功满级后每次获得经验都会被"抬高"，本条**照抄不修**。
    /// </remarks>
    public void GetExpNG(uint dwExp, bool isAdjustHero = true)
    {
        // 原文 2966-2969：过程级局部
        uint lwExp;

        // 原文 2973-2974：if not m_boTrainingNG then Exit;
        if (!m_boTrainingNG) return;
        // 原文 2975-2976：if dwExp = 0 then Exit;
        if (dwExp == 0) return;

        // 原文 2977：lwExp := dwExp;
        lwExp = dwExp;

        // 原文 2979-2985：英雄异地 → 全给英雄
        if (m_MyHero != null && m_boExpFromFromHero
            && !M2Config.boShareExpHeroSameMap
            && m_MyHero.m_PEnvir != m_PEnvir)
        {
            // 原文 2983：THeroObject(m_MyHero).GetExpNG(lwExp);
            PlayerSurfaceCore1Seams.HeroGetExpNG(m_MyHero, lwExp);
            // 原文 2984：Exit;
            return;
        }

        // 原文 2986-3010：IsAdjustHero
        if (isAdjustHero)
        {
            if (m_MyHero != null && !m_MyHero.m_boDeath
                && (M2Config.boShareExpHeroSameMap || m_MyHero.m_PEnvir == m_PEnvir))
            {
                // 原文 2991-3006
                if (M2Config.boHeroGetAllExp)
                {
                    lwExp = dwExp;                                       // 原文 2993
                    PlayerSurfaceCore1Seams.HeroGetExpNG(m_MyHero, lwExp); // 原文 2994
                }
                else
                {
                    // 原文 2998-2999
                    lwExp = unchecked((uint)M2ShareFuncs.DelphiRound(dwExp * (M2Config.nHeroKillMonExpRate / 100.0)));
                    PlayerSurfaceCore1Seams.HeroGetExpNG(m_MyHero, lwExp);
                    // 原文 3001-3005：if not g_Config.boHumanGetAllExp then begin lwExp := dwExp - lwExp; dwExp := lwExp; end;
                    if (!M2Config.boHumanGetAllExp)
                    {
                        lwExp = dwExp - lwExp;
                        dwExp = lwExp;
                    }
                }
            }
            else
                lwExp = dwExp;   // 原文 3009
        }

        // 原文 3012-3016
        if (m_AbilNG.Level >= PlayerSurfaceCore1Const.MAXNG_LEVEL)
        {
            // ★ 原文缺陷：人物侧 :2751 是 Min，此处是 Max —— 逐字保留
            dwExp = (uint)Math.Max(M2Config.nHighLevelGetExp, 0);                        // 原文 3014
            lwExp = Math.Max(lwExp, (uint)Math.Max(M2Config.nHighLevelGetExp, 0));       // 原文 3015
        }

        // 原文 3018-3019：if m_AbilNG.Level >= g_Config.nNGMaxLevelLimte then Exit;
        if (m_AbilNG.Level >= (uint)PlayerSurfaceCore1Const.nNGMaxLevelLimte) return;

        // 原文 3021-3022：nMaxUpLevelCount := 0; OldLevel := m_AbilNG.Level;
        int nMaxUpLevelCount = 0;
        uint oldLevel = m_AbilNG.Level;

        // 原文 3023：RefExp:
        while (true)
        {
            // 原文 3024：Inc(nMaxUpLevelCount);
            nMaxUpLevelCount++;
            // 原文 3025：dwAddExp := m_AbilNG.MaxExp - m_AbilNG.Exp;
            uint dwAddExp = unchecked(m_AbilNG.MaxExp - m_AbilNG.Exp);

            // 原文 3026
            if (lwExp >= dwAddExp && m_AbilNG.MaxExp > m_AbilNG.Exp)
            {
                // 原文 3028
                if (m_AbilNG.Level < PlayerSurfaceCore1Const.MAXNG_LEVEL)
                {
                    m_AbilNG.Level++;                      // 原文 3030：Inc 到 ushort 字段
                    lwExp = unchecked(lwExp - dwAddExp);   // 原文 3031
                    m_AbilNG.Exp = 0;                      // 原文 3032
                    // 原文 3033：HasLevelUpNG(m_AbilNG.Level - 1);
                    PlayerSurfaceCore1Seams.HasLevelUpNG(this, (int)m_AbilNG.Level - 1);
                    // 原文 3034-3044
                    if (lwExp > 0)
                    {
                        if (nMaxUpLevelCount < M2Config.nMaxUpLevelCount)
                        {
                            // 原文 3038-3039：goto RefExp; Exit;
                            continue;
                        }
                        else
                            m_AbilNG.Exp = unchecked(m_AbilNG.Exp + lwExp);   // 原文 3043
                    }
                }
            }
            else
            {
                // 原文 3048-3066
                if (m_AbilNG.MaxExp > m_AbilNG.Exp)
                {
                    // 原文 3052：Inc(m_AbilNG.Exp, lwExp);
                    m_AbilNG.Exp = unchecked(m_AbilNG.Exp + lwExp);
                    // 原文 3053：// AddBodyLuck(lwExp * 0.002);   ← 原文注释掉
                }
                else
                {
                    // 原文 3057-3064
                    m_AbilNG.Level++;
                    m_AbilNG.Exp = unchecked(m_AbilNG.Exp - m_AbilNG.MaxExp);   // 原文 3058
                    PlayerSurfaceCore1Seams.HasLevelUpNG(this, (int)m_AbilNG.Level - 1);  // 原文 3059
                    if (nMaxUpLevelCount < M2Config.nMaxUpLevelCount)
                    {
                        // 原文 3062-3063：goto RefExp; Exit;
                        continue;
                    }
                }
            }

            break;
        }

        // 原文 3067：m_dwGetExp := dwExp;
        m_dwGetExp = dwExp;
        // 原文 3068：// IncBeadExp(dwExp);   ← 原文注释掉
        // 原文 3069：SendMsg(Self, RM_WINEXP, 1, dwExp, 0, 0, '');   ← 第三参 = 1（内功）
        PlayerSurfaceCore1Seams.SendMsg(this, Grobal2Const.RM_WINEXP, 1, dwExp, 0, 0, "");

        // 原文 3071-3072
        if (oldLevel != m_AbilNG.Level)
        {
            PlayerSurfaceCore1Seams.AddGameDataLog8(PlayerSurfaceLogActionConst.LOG_LevelChange, GXX.M2Server.Npc.ObjNpcConst.LOG_ActionNone,
                this, "内功等级", 0, "0", m_AbilNG.Level, (int)oldLevel);
        }
    }

    // ==================================================================
    // IncExpNG（原文 3075-3135）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.IncExpNG(dwExp: LongWord);`（ObjPlayer.pas:3075-3135）。
    ///
    /// 与 <see cref="GetExpNG"/> 的差异（逐条照抄）：
    /// <list type="bullet">
    ///   <item><description>:3083 门控是 **`if m_boTrainingNG then begin ... end;`**（整条包住），
    ///     **没有** `dwExp = 0` 早退、**没有**英雄分段、**没有** `nNGMaxLevelLimte` 早退、
    ///     **没有**满级封顶段。</description></item>
    ///   <item><description>:3131 `SendMsg(Self, RM_WINEXP, 1, dwExp, 0, 0, '')` 的第三参同样是 1；
    ///     但 `m_dwGetExp` **完全没被赋值**（`GetExpNG` 有 :3067）—— 逐字保留。</description></item>
    ///   <item><description>:3108/:3116 的 `Inc(m_AbilNG.Exp, lwExp)` 是 **16 位无符号字段的加法**
    ///     （`TAbilityNG.Exp` 是 `LongWord`，此处不失真；`Level` 是 `Word`）。</description></item>
    /// </list>
    /// </summary>
    public void IncExpNG(uint dwExp)
    {
        // 原文 3083：if m_boTrainingNG then
        if (m_boTrainingNG)
        {
            // 原文 3085-3087
            uint lwExp = dwExp;
            int nMaxUpLevelCount = 0;
            uint oldLevel = m_AbilNG.Level;

            // 原文 3088：RefExp:
            while (true)
            {
                // 原文 3089：Inc(nMaxUpLevelCount);
                nMaxUpLevelCount++;
                // 原文 3090：dwAddExp := m_AbilNG.MaxExp - m_AbilNG.Exp;
                uint dwAddExp = unchecked(m_AbilNG.MaxExp - m_AbilNG.Exp);

                // 原文 3092
                if (lwExp >= dwAddExp && m_AbilNG.MaxExp > m_AbilNG.Exp)
                {
                    // 原文 3094
                    if (m_AbilNG.Level < PlayerSurfaceCore1Const.MAXNG_LEVEL)
                    {
                        m_AbilNG.Level++;                      // 原文 3096
                        lwExp = unchecked(lwExp - dwAddExp);   // 原文 3097
                        m_AbilNG.Exp = 0;                      // 原文 3098
                        PlayerSurfaceCore1Seams.HasLevelUpNG(this, (int)m_AbilNG.Level - 1);  // 原文 3099
                        // 原文 3100-3109
                        if (lwExp > 0)
                        {
                            if (nMaxUpLevelCount < M2Config.nMaxUpLevelCount)
                            {
                                // 原文 3104-3105：goto RefExp; Exit;
                                continue;
                            }
                            else
                                m_AbilNG.Exp = unchecked(m_AbilNG.Exp + lwExp);   // 原文 3108
                        }
                    }
                }
                else
                {
                    // 原文 3112-3129
                    if (m_AbilNG.MaxExp > m_AbilNG.Exp)
                    {
                        m_AbilNG.Exp = unchecked(m_AbilNG.Exp + lwExp);   // 原文 3116
                    }
                    else
                    {
                        m_AbilNG.Level++;                                            // 原文 3120
                        m_AbilNG.Exp = unchecked(m_AbilNG.Exp - m_AbilNG.MaxExp);    // 原文 3121
                        PlayerSurfaceCore1Seams.HasLevelUpNG(this, (int)m_AbilNG.Level - 1);  // 原文 3122
                        if (nMaxUpLevelCount < M2Config.nMaxUpLevelCount)
                        {
                            // 原文 3125-3126：goto RefExp; Exit;
                            continue;
                        }
                    }
                }

                break;
            }

            // 原文 3131：SendMsg(Self, RM_WINEXP, 1, dwExp, 0, 0, '');
            PlayerSurfaceCore1Seams.SendMsg(this, Grobal2Const.RM_WINEXP, 1, dwExp, 0, 0, "");

            // 原文 3132-3133
            if (oldLevel != m_AbilNG.Level)
            {
                PlayerSurfaceCore1Seams.AddGameDataLog8(PlayerSurfaceLogActionConst.LOG_LevelChange, GXX.M2Server.Npc.ObjNpcConst.LOG_ActionNone,
                    this, "内功等级", 0, "0", m_AbilNG.Level, (int)oldLevel);
            }
        }
    }

    // ==================================================================
    // IncBeadExp（原文 3137-3229）—— 聚灵珠
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.IncBeadExp(dwExp: LongWord; IsFromNPC: Boolean = False); // 聚灵珠`
    /// （ObjPlayer.pas:3137-3229）。
    ///
    /// 逐段（全部照抄）：
    /// <list type="number">
    ///   <item><description>:3148-3149 `dwExp &lt;= 0` 早退（**注意是 `&lt;=`，即 0 也退**）。</description></item>
    ///   <item><description>:3151-3156 遍历背包，跳过三类：`StdItem = nil`、`StdItem.StdMode &lt;&gt; 49`、
    ///     `UserItem.Dura &gt;= UserItem.DuraMax`。</description></item>
    ///   <item><description>:3158 条件 `(StdItem.AC1 &lt;= 0) or (m_WAbil.Level &lt;= StdItem.AC1)`
    ///     —— ⚠ 读的是 **`m_WAbil.Level`**（不是 `m_Abil.Level`），逐字保留。</description></item>
    ///   <item><description>:3160-3165 `StdItem.Reserved1 &gt; 0` 时把 `UserItem.btValue[4]` 当
    ///     **`PDouble`**（8 字节日期）解引用，`Date &gt; dLastDate^` 则跳过（**过期珠不再聚经验**）。</description></item>
    ///   <item><description>:3168-3171 收集比例：`IsFromNPC and (StdItem.Shape = 0)` → 全额；
    ///     否则 `Round(StdItem.Shape / 100 * dwExp)`。</description></item>
    ///   <item><description>:3173-3182 功能 NPC 的 `@GetBeadExp` 事件（期间 `m_dwBeadExp`/`m_nBeadSource`
    ///     可被脚本改写，事件后取回 `dwAddExp` 并把两个字段归零）。</description></item>
    ///   <item><description>:3184-3185 `dwAddExp &lt; 1` → **`Exit`（退出整个过程，不是 `Continue`）**。</description></item>
    ///   <item><description>:3187-3212 耐久换算：`nInt64 := dwItemExp + dwAddExp`；`nDura := nInt64 div 10000`；
    ///     满则 `dwUseExp := (DuraMax - Dura) * 10000 - dwItemExp`（NPC 全额时不再按 Shape 折算），
    ///     `nInt64 := 0`、`Dura := DuraMax`；否则 `Dura := Min(Dura + nDura, DuraMax)`、
    ///     `nInt64 := nInt64 mod 10000`、`dwExp := 0`。</description></item>
    ///   <item><description>:3214-3217 `btValue[0] := Min(nInt64, High(LongWord))`、`SendUpdateItem`，
    ///     聚满时发 `StdItem.Name + '的经验已聚满！'`（**四参 SysMsg**：颜色 251/249，`t_Hint`）。</description></item>
    ///   <item><description>:3219-3220 `if dwExp = 0 then Break;`（只在上面 `dwExp := 0` 的分支才会成立）。</description></item>
    ///   <item><description>:3224-3228 尾部：`g_Config.boRecordBeadExp and (dwExp &gt; 0)` 时把余量累进
    ///     `m_dwRecordBeadExp`（夹 `High(LongWord)`）。</description></item>
    /// </list>
    ///
    /// ★ 原文缺陷（原文 3184-3185，逐字保留）：`if dwAddExp &lt; 1 then Exit;` 用的是 **`Exit`**，
    /// 于是**第一颗不达标的珠子就会中断整轮遍历** —— 后面还有能吃的珠子也不会被处理。
    /// 从上下文看意图应是 `Continue`（与 :3156 的 `Continue` 同族）。本条**照抄不修**，
    /// 锁定用例 `ObjPlayerCore1Tests.IncBeadExp_FirstNonQualifyingBeadExitsLoop_OriginalDefect`。
    ///
    /// ★ 原文缺陷（原文 3198，逐字保留）：`dwUseExp := Round(dwUseExp * 100 / StdItem.Shape);`
    /// —— 该分支的进入条件是 `IsFromNPC and (StdItem.Shape = 0)` 为**假**，其中一种情形是
    /// **`StdItem.Shape = 0` 且 `IsFromNPC = False`**，此时 `100 / 0` 在 Delphi 浮点上下文
    /// 产生 `Inf`（`Round(Inf)` 抛 `EInvalidOp`）。原文没有除零保护，照抄。
    ///
    /// ★ 原文如此（原文 3162-3163，逐字保留）：`dLastDate := @UserItem.btValue[4]` 把
    /// **`Integer` 数组的第 4 个元素地址**当 `PDouble` 解引用（跨 2 个 int 读 8 字节）。
    /// 托管侧按等价语义读 `btValue[4]`/`btValue[5]` 两个 int 拼出的 8 字节（小端 `double`）。
    /// </remarks>
    /// <param name="dwExp">原文 `dwExp: LongWord`。</param>
    /// <param name="isFromNPC">原文 `IsFromNPC: Boolean = False`。</param>
    public void IncBeadExp(uint dwExp, bool isFromNPC = false)
    {
        // 原文 3148-3149：if dwExp <= 0 then Exit;
        if (dwExp <= 0) return;

        // 原文 3151：for I := 0 to m_ItemList.Count - 1 do
        for (int i = 0; i <= m_ItemList.Count - 1; i++)
        {
            // 原文 3153：UserItem := m_ItemList.Items[I];
            //   ⚠ 托管 `m_ItemList` 的元素是**可空值类型**（`TUserItem?`，见 ObjBase.cs:62 的 D35 登记），
            //     原文是 `pTUserItem`（可为 nil）。用模式匹配一次覆盖两种形态。
            if (m_ItemList[i] is not TUserItem userItem) continue;

            // 原文 3154：StdItem := UserEngine.GetStdItem(UserItem.wIndex);
            TStdItem? stdItemN = PlayerSurfaceItemSeams.GetStdItem(userItem.wIndex);
            // 原文 3155：if (StdItem = nil) or (StdItem.StdMode <> 49) or (UserItem.Dura >= UserItem.DuraMax) then Continue;
            if (stdItemN == null) continue;
            TStdItem stdItem = stdItemN.Value;
            if (stdItem.StdMode != 49 || userItem.Dura >= userItem.DuraMax) continue;

            // 原文 3158：if (StdItem.AC1 <= 0) or (m_WAbil.Level <= StdItem.AC1) then
            if (stdItem.AC1 <= 0 || m_wAbil.Level <= (uint)stdItem.AC1)
            {
                // 原文 3160-3165：StdItem.Reserved1 > 0 时用 btValue[4] 当 PDouble 比日期
                if (stdItem.Reserved1 > 0)
                {
                    // 原文 3162：dLastDate := @UserItem.btValue[4];
                    double lastDate = BitConverter.Int64BitsToDouble(
                        ((long)userItem.GetBtValue(5) << 32) | (uint)userItem.GetBtValue(4));
                    // 原文 3163-3164：if Date > dLastDate^ then Continue;
                    if (DateTime.Now.ToOADate() > lastDate) continue;
                }

                // 原文 3167-3171：收集比例
                uint dwAddExp;
                if (isFromNPC && stdItem.Shape == 0)
                    dwAddExp = dwExp;                                        // 原文 3169
                else
                    dwAddExp = unchecked((uint)M2ShareFuncs.DelphiRound(stdItem.Shape / 100.0 * dwExp)); // 原文 3171

                // 原文 3173-3182：功能 NPC 的 @GetBeadExp 事件
                if (PlayerSurfaceItemSeams.FunctionNPC != null)
                {
                    m_nScriptGotoCount = 0;          // 原文 3175
                    m_dwBeadExp = dwAddExp;          // 原文 3176
                    m_nBeadSource = stdItem.Source;  // 原文 3177：⚠ Source 是 int，原文直接赋给 Integer 字段
                    PlayerSurfaceItemSeams.FunctionNpcGotoLable(
                        PlayerSurfaceItemSeams.FunctionNPC, this, "@GetBeadExp", false);  // 原文 3178
                    dwAddExp = m_dwBeadExp;          // 原文 3179
                    m_dwBeadExp = 0;                 // 原文 3180
                    m_nBeadSource = 0;               // 原文 3181
                }

                // 原文 3184-3185：if dwAddExp < 1 then Exit;
                //   ★ 原文缺陷：**Exit（退出整轮遍历）**，而不是 Continue —— 见 XML doc。
                if (dwAddExp < 1) return;

                // 原文 3187-3188：dwItemExp := UserItem.btValue[0]; nInt64 := dwItemExp + dwAddExp;
                uint dwItemExp = unchecked((uint)userItem.GetBtValue(0));
                long nInt64 = (long)dwItemExp + dwAddExp;

                // 原文 3189：nDura := nInt64 div 10000;
                //   原文 `nInt64` 是 **Int64**（:3188 的 `nInt64 := dwItemExp + dwAddExp` 提升，
                //   且 :3194 的 `(DuraMax - Dura) * 10000` 需要 64 位），`nDura` 是 `Integer` ——
                //   托管保留同样的 64 位中间量，仅把商收回 `int`（与原文的赋值窄化一致）。
                int nDura = unchecked((int)(nInt64 / 10000));

                // 原文 3190：if nDura > 0 then
                if (nDura > 0)
                {
                    // 原文 3192：if UserItem.Dura + nDura >= UserItem.DuraMax then
                    //   ⚠ 原文此处是 `Word + Integer` 的提升（不做 UShort 截断）—— 与 :3206 的
                    //     `Min(UserItem.Dura + nDura, UserItem.DuraMax)` **写法不同**，逐字保留差异。
                    if (userItem.Dura + nDura >= userItem.DuraMax)
                    {
                        // 原文 3194：dwUseExp := (UserItem.DuraMax - UserItem.Dura) * 10000 - dwItemExp;
                        long dwUseExp = ((long)userItem.DuraMax - userItem.Dura) * 10000 - dwItemExp;
                        if (isFromNPC && stdItem.Shape == 0)
                        {
                            // 原文 3196：dwUseExp := dwUseExp;   ← 原文如此（自赋值，无操作）
                            dwUseExp = dwUseExp;
                        }
                        else
                        {
                            // 原文 3198：dwUseExp := Round(dwUseExp * 100 / StdItem.Shape);
                            //   ★ 原文缺陷：Shape = 0 时除零 —— 见 XML doc。
                            dwUseExp = M2ShareFuncs.DelphiRound(dwUseExp * 100.0 / stdItem.Shape);
                        }

                        // 原文 3200-3202
                        dwExp = unchecked(dwExp - (uint)dwUseExp);
                        nInt64 = 0;
                        userItem.Dura = userItem.DuraMax;
                    }
                    else
                    {
                        // 原文 3206-3208
                        userItem.Dura = (ushort)Math.Min((long)userItem.Dura + nDura, (long)userItem.DuraMax);
                        nInt64 = nInt64 % 10000;
                        dwExp = 0;
                    }
                }
                else
                    dwExp = 0;                       // 原文 3212

                // 原文 3214：UserItem.btValue[0] := Min(nInt64, High(LongWord));
                //   ⚠ `btValue` 是 `array[0..13] of Integer`（**有符号**）——
                //     托管 `TUserItem.btValue` 同为 `fixed int`，故此处 `unchecked((int)...)` 与原文一致。
                userItem.SetBtValue(0, unchecked((int)Math.Min(nInt64, uint.MaxValue)));

                // 写回槽位（托管 D35 契约：`TUserItem` 是值类型，改完必须写回）
                m_ItemList[i] = userItem;

                // 原文 3215：SendUpdateItem(UserItem);
                PlayerSurfaceCore1Seams.SendUpdateItem(this, userItem);

                // 原文 3216-3217：if UserItem.Dura >= UserItem.DuraMax then SysMsg(StdItem.Name + '的经验已聚满！', 251, 249, t_Hint);
                if (userItem.Dura >= userItem.DuraMax)
                {
                    PlayerSurfaceCore1Seams.SysMsgFB(this, stdItem.NameStr + "的经验已聚满！",
                        251, 249, TMsgType.t_Hint);
                }

                // 原文 3219-3220：if dwExp = 0 then Break;
                if (dwExp == 0) break;
            }
        }

        // 原文 3224-3228
        if (M2Config.boRecordBeadExp && dwExp > 0)
        {
            // 原文 3226-3227：nInt64 := m_dwRecordBeadExp + dwExp; m_dwRecordBeadExp := Min(nInt64, High(LongWord));
            long nInt64 = (long)m_dwRecordBeadExp + dwExp;
            m_dwRecordBeadExp = (uint)Math.Min(nInt64, uint.MaxValue);
        }
    }

    // ==================================================================
    // 金额零头（原文 3262-3346）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.IncGamePoint(nGamePoint: LongWord);`（ObjPlayer.pas:3262-3271）。
    /// `Int64(m_nGamePoint) + nGamePoint` 超 `High(LongWord)` 才夹上限。
    /// ⚠ 字段类型偏差（既有）：托管 `m_nGamePoint` 是 `int`（`ObjBase.OnlineMsg.cs:37`）、
    /// 原文是 `Integer`（**有符号**）—— 原文 `Int64(m_nGamePoint)` 按有符号提升，托管一致。
    /// </summary>
    public void IncGamePoint(uint nGamePoint)
    {
        // 原文 3266：Int64Value := Int64(m_nGamePoint) + nGamePoint;
        long int64Value = (long)m_nGamePoint + nGamePoint;
        // 原文 3267-3270
        if (int64Value > uint.MaxValue)
            m_nGamePoint = unchecked((int)uint.MaxValue);
        else
            m_nGamePoint = (int)int64Value;
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.IncGameGird(nPoint: LongWord);`（ObjPlayer.pas:3273-3282）。
    /// 与 <see cref="IncGamePoint"/> 同构；字段托管 `m_nGameGird`（`ObjBase.OnlineMsg.cs:40`，`int`）。
    /// </summary>
    public void IncGameGird(uint nPoint)
    {
        // 原文 3277：Int64Value := Int64(m_nGameGird) + nPoint;
        long int64Value = (long)m_nGameGird + nPoint;
        // 原文 3278-3281
        if (int64Value > uint.MaxValue)
            m_nGameGird = unchecked((int)uint.MaxValue);
        else
            m_nGameGird = (int)int64Value;
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.DecGameGird(nPoint: LongWord);`（ObjPlayer.pas:3305-3311）。
    /// 不够时**夹到 0**（与 `DecGold` 的"原地不动"不同）。
    /// </summary>
    public void DecGameGird(uint nPoint)
    {
        // 原文 3307-3310：if m_nGameGird >= nPoint then Dec(...) else m_nGameGird := 0;
        if ((uint)m_nGameGird >= nPoint)
            m_nGameGird = unchecked((int)((uint)m_nGameGird - nPoint));
        else
            m_nGameGird = 0;
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.IncGameDiamond(nPoint: LongWord);`（ObjPlayer.pas:3313-3322）。
    /// </summary>
    public void IncGameDiamond(uint nPoint)
    {
        // 原文 3317：Int64Value := Int64(m_nGameDiamond) + nPoint;
        long int64Value = (long)m_nGameDiamond + nPoint;
        // 原文 3318-3321
        if (int64Value > uint.MaxValue)
            m_nGameDiamond = unchecked((int)uint.MaxValue);
        else
            m_nGameDiamond = (int)int64Value;
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.DecGameDiamond(nPoint: LongWord);`（ObjPlayer.pas:3324-3330）。
    /// 不够时夹到 0。
    /// </summary>
    public void DecGameDiamond(uint nPoint)
    {
        // 原文 3326-3329
        if ((uint)m_nGameDiamond >= nPoint)
            m_nGameDiamond = unchecked((int)((uint)m_nGameDiamond - nPoint));
        else
            m_nGameDiamond = 0;
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.IncGameGlory(nPoint: Integer);`（ObjPlayer.pas:3332-3338）。
    /// ⚠ 形参是**有符号** `Integer`；溢出判定是 `Int64(m_nGameGlory) + nPoint > High(Integer)`
    /// —— 即**只判上溢、不判下溢**（负数把荣誉减到 `Low(Integer)` 以下会静默回绕），原文如此。
    /// </summary>
    public void IncGameGlory(int nPoint)
    {
        // 原文 3334：if Int64(m_nGameGlory) + nPoint > High(Integer) then
        if ((long)m_nGameGlory + nPoint > int.MaxValue)
            m_nGameGlory = int.MaxValue;      // 原文 3335
        else
            m_nGameGlory = unchecked(m_nGameGlory + nPoint);   // 原文 3337：Inc(m_nGameGlory, nPoint);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.DecGameGlory(nPoint: Integer);`（ObjPlayer.pas:3340-3346）。
    /// 不够时夹到 0（原文 `m_nGameGlory := 0`，不是 `m_nGameGlory - nPoint`）。
    /// </summary>
    public void DecGameGlory(int nPoint)
    {
        // 原文 3342-3345
        if (m_nGameGlory >= nPoint)
            m_nGameGlory = unchecked(m_nGameGlory - nPoint);
        else
            m_nGameGlory = 0;
    }

    // ==================================================================
    // SetSoftVersionDateEx（原文 3348-3351）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.SetSoftVersionDateEx(Value: Integer);`（ObjPlayer.pas:3348-3351）
    /// —— `SoftVersionDateEx` 属性的写方法（属性声明 :1360，`read FSoftVersionDateEx`）。
    /// </summary>
    public void SetSoftVersionDateEx(int value)
    {
        // 原文 3350：FSoftVersionDateEx := Value;
        FSoftVersionDateEx = value;
    }

    // ==================================================================
    // SendAcupointLevels（原文 3353-3358）
    // ==================================================================

    /// <summary>
    /// 原文 `function TPlayObject.SendAcupointLevels(): Boolean;`（ObjPlayer.pas:3353-3358）
    /// —— 下发「打通穴道需要的内功等级」表。
    /// <code>
    ///   m_DefMsg := MakeDefaultMsg(SM_ACUPOINTLEVELS, 0, 0, 0, 0);                      // 3355
    ///   SendSocketEx(@m_DefMsg, @g_Config.AcupointLevels, SizeOf(g_Config.AcupointLevels)); // 3356
    ///   Result := True;                                                                // 3357
    /// </code>
    /// ⚠ `g_Config.AcupointLevels` 是 `array [0 .. 4, 0 .. 4] of Integer`（`M2Share.pas:2326`
    /// 与 :5001 typed constant）—— 托管 `M2Config` **无该字段**（全文 grep 0 命中），
    /// 故表格内容经 <see cref="PlayerSurfaceCore1Seams.AcupointLevels"/> 接缝取字节块，
    /// 默认返回 `5×5×4 = 100` 字节的全零块（**尺寸与原文 `SizeOf` 一致**，内容由宿主注入）。
    /// 返回值恒为 `True`（原文无失败分支）。
    /// </summary>
    public bool SendAcupointLevels()
    {
        // 原文 3355：m_DefMsg := MakeDefaultMsg(SM_ACUPOINTLEVELS, 0, 0, 0, 0);
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_ACUPOINTLEVELS, 0, 0, 0, 0);
        // 原文 3356：SendSocketEx(@m_DefMsg, @g_Config.AcupointLevels, SizeOf(g_Config.AcupointLevels));
        SendSocketEx(m_DefMsg, PlayerSurfaceCore1Seams.AcupointLevels());
        // 原文 3357：Result := True;
        return true;
    }

    // ==================================================================
    // IsGroupMember（原文 3396-3424）
    // ==================================================================

    /// <summary>
    /// 原文 `function TPlayObject.IsGroupMember(Target: TPlayObject): Boolean; // 004C3908`
    /// （ObjPlayer.pas:3396-3424）。
    ///
    /// 逐行：
    /// <list type="number">
    ///   <item><description>:3400-3402 `Result := False; if m_GroupOwner = nil then Exit;`</description></item>
    ///   <item><description>:3404-3405 `if m_GroupOwner.m_GroupMembers = nil then Exit;`</description></item>
    ///   <item><description>:3407-3411 `{$IF MULTI_THREAD = 1}` 下 `LockR(26)`/`UnLockR` ——
    ///     托管侧**无该编译开关所需的多线程容器**（`TSafeStringList` 未切出），
    ///     且判等只在**对象指针**上（`:3414 m_GroupMembers.Objects[I] = Target`），
    ///     托管 `List&lt;TPlayObject&gt;` 的索引访问本身即原子读，故此处**不伪造锁**。</description></item>
    ///   <item><description>:3412-3419 正序遍历，命中即 `Result := True; Break`。</description></item>
    /// </list>
    /// ⚠ 原文按 `Objects[I]`（对象指针）判等，**不是**按名字 —— 托管侧 `List&lt;TPlayObject&gt;`
    /// 用 `ReferenceEquals` 逐字对应（`==` 在托管 `TPlayObject` 上是引用比较，但显式
    /// `ReferenceEquals` 更能表达原文的指针语义）。
    /// </summary>
    public bool IsGroupMember(TPlayObject? target)
    {
        // 原文 3400：Result := False;
        bool result = false;
        // 原文 3401-3402：if m_GroupOwner = nil then Exit;
        if (m_GroupOwner == null) return result;
        // 原文 3404-3405：if m_GroupOwner.m_GroupMembers = nil then Exit;
        if (m_GroupOwner.m_GroupMembers == null) return result;

        // 原文 3412-3419
        for (int i = 0; i <= m_GroupOwner.m_GroupMembers.Count - 1; i++)
        {
            // 原文 3414：if m_GroupOwner.m_GroupMembers.Objects[I] = Target then
            if (ReferenceEquals(m_GroupOwner.m_GroupMembers[i], target))
            {
                result = true;   // 原文 3416
                break;           // 原文 3417
            }
        }

        return result;
    }

    // ==================================================================
    // Whisper（原文 3428-3509）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.Whisper(whostr, saystr: string);`（ObjPlayer.pas:3428-3509）
    /// —— 私聊。
    ///
    /// 逐段（全部照抄）：
    /// <list type="number">
    ///   <item><description>:3432 `PlayObject := UserEngine.GetPlayObject(whostr);`
    ///     → 接缝 <see cref="PlayerSurfaceCore1Seams.GetPlayObject"/>。</description></item>
    ///   <item><description>:3433-3508 目标存在/否则 `SysMsg(whostr + g_sUserNotOnLine, c_Red, t_Hint)`。</description></item>
    ///   <item><description>:3435-3439 目标 `not m_boReadyRun` → `whostr + g_sCanotSendmsg` 后 Exit。</description></item>
    ///   <item><description>:3441-3445 目标 `not m_boHearWhisper` **或** `IsBlockWhisper(m_sCharName)`
    ///     → `whostr + g_sUserDenyWhisperMsg` 后 Exit。</description></item>
    ///   <item><description>:3448-3449 `g_Config.boRecordPrivateMsg and (Length(saystr) &gt; 0)` 时
    ///     打一行 `[私聊] 甲=&gt;乙:内容` 到 `MainOutMessage`。</description></item>
    ///   <item><description>:3451-3469 目标离线挂机且 `m_sAutoSendMsg &lt;&gt; ''` → **自动回复**：
    ///     `m_btPermission &gt;= 10`（GM）走 GM 颜色，否则走普通私聊颜色；随后 `Exit`。</description></item>
    ///   <item><description>:3471-3505 两条大分支（`m_btPermission &gt; 0` / 否则）：
    ///     都把消息发给目标，并**额外**发给双方各自的 `m_GetWhisperHuman`（若存在且未 Ghost）。</description></item>
    /// </list>
    ///
    /// ★ 原文缺陷（原文 3451-3468，逐字保留）：自动回复分支把消息发给
    /// **`SendMsg(Self, RM_WHISPER, ...)`** —— 接收者是**发送方自己**（`Self`），
    /// 内容却是 `PlayObject.m_sAutoSendMsg`（**目标的**自动回复语）。
    /// 于是"自动回复"实际显示在**发私聊的人自己**的聊天框里，而不是目标侧。
    /// 与紧随其后的 :3474（`PlayObject.SendMsg(PlayObject, ...)`，接收者=目标）**不一致**。
    /// 本条**照抄不修**，锁定用例
    /// `ObjPlayerCore1Tests.Whisper_OfflineAutoReplyIsSentToSelf_OriginalDefect`。
    ///
    /// ★ 原文如此（原文 3449，逐字保留）：日志行的中文前缀 `'[私聊] '` 是**硬编码**，
    /// 而其它提示串都取 `g_s*` 全局（可配置）。
    ///
    /// ⚠ 托管签名偏差（已登记）：原文 `whostr`/`saystr` 是 `string`（Delphi 的 **AnsiString**），
    /// 托管用 `string`；`m_sAutoSendMsg` 同理（`ObjBase.OnlineMsg.cs:41`）。
    /// </remarks>
    public void Whisper(string whostr, string saystr)
    {
        // 原文 3430：var PlayObject: TPlayObject;
        // 原文 3432：PlayObject := UserEngine.GetPlayObject(whostr);
        TPlayObject? playObject = PlayerSurfaceCore1Seams.GetPlayObject(whostr);

        // 原文 3433：if PlayObject <> nil then
        if (playObject != null)
        {
            // 原文 3435-3439
            if (!playObject.m_boReadyRun)
            {
                // 原文 3437：SysMsg(whostr + g_sCanotSendmsg, c_Red, t_Hint);
                SysMsg(whostr + PlayerSurfaceCore1Seams.g_sCanotSendmsg, TMsgColor.c_Red, TMsgType.t_Hint);
                return;
            }

            // 原文 3441-3445
            if (!playObject.m_boHearWhisper || playObject.IsBlockWhisper(m_sCharName))
            {
                // 原文 3443：SysMsg(whostr + g_sUserDenyWhisperMsg, c_Red, t_Hint);
                SysMsg(whostr + PlayerSurfaceCore1Seams.g_sUserDenyWhisperMsg, TMsgColor.c_Red, TMsgType.t_Hint);
                return;
            }

            // 原文 3448-3449：记录私聊信息
            if (M2Config.boRecordPrivateMsg && saystr.Length > 0)
            {
                PlayerSurfaceCore1Seams.MainOutMessage(
                    "[私聊] " + m_sCharName + "=>" + playObject.m_sCharName + ":" + saystr);
            }

            // 原文 3451-3469：离线挂机人物自动回复
            if (playObject.m_boOffLine && playObject.m_sAutoSendMsg != "")
            {
                if (m_btPermission >= 10)
                {
                    // 原文 3455-3461
                    if (M2Config.boShowWhisperLevelMsg)
                    {
                        PlayerSurfaceCore1Seams.SendMsg(this, Grobal2Const.RM_WHISPER, 0,
                            M2Config.btGMWhisperMsgFColor, M2Config.btGMWhisperMsgBColor, 0,
                            string.Format("{0} " + M2ShareState.g_sShowWhisperLevelMsg + "=> {1}",
                                playObject.m_sCharName, playObject.m_wAbil.Level, playObject.m_sAutoSendMsg));
                    }
                    else
                    {
                        PlayerSurfaceCore1Seams.SendMsg(this, Grobal2Const.RM_WHISPER, 0,
                            M2Config.btGMWhisperMsgFColor, M2Config.btGMWhisperMsgBColor, 0,
                            string.Format("{0} => {1}", playObject.m_sCharName, playObject.m_sAutoSendMsg));
                    }
                }
                else
                {
                    // 原文 3465-3466
                    PlayerSurfaceCore1Seams.SendMsg(this, Grobal2Const.RM_WHISPER, 0,
                        M2Config.btWhisperMsgFColor, M2Config.btWhisperMsgBColor, 0,
                        playObject.m_sCharName + "=>" + " " + playObject.m_sAutoSendMsg);
                }
                // 原文 3468：Exit;
                return;
            }

            // 原文 3471：if m_btPermission > 0 then
            if (m_btPermission > 0)
            {
                // 原文 3473-3478
                if (M2Config.boShowWhisperLevelMsg)
                {
                    PlayerSurfaceCore1Seams.SendMsg(playObject, Grobal2Const.RM_WHISPER, 0,
                        M2Config.btGMWhisperMsgFColor, M2Config.btGMWhisperMsgBColor, 0,
                        string.Format("{0} " + M2ShareState.g_sShowWhisperLevelMsg + "=> {1}",
                            m_sCharName, m_wAbil.Level, saystr));
                }
                else
                {
                    PlayerSurfaceCore1Seams.SendMsg(playObject, Grobal2Const.RM_WHISPER, 0,
                        M2Config.btGMWhisperMsgFColor, M2Config.btGMWhisperMsgBColor, 0,
                        string.Format("{0} => {1}", m_sCharName, saystr));
                }

                // 原文 3481-3483：取得私聊信息（发给自己的监听者）
                if (m_GetWhisperHuman != null && !m_GetWhisperHuman.m_boGhost)
                {
                    PlayerSurfaceCore1Seams.SendMsg(m_GetWhisperHuman, Grobal2Const.RM_WHISPER, 0,
                        M2Config.btGMWhisperMsgFColor, M2Config.btGMWhisperMsgBColor, 0,
                        m_sCharName + "=>" + playObject.m_sCharName + " " + saystr);
                }

                // 原文 3485-3487：发给目标的监听者
                if (playObject.m_GetWhisperHuman != null && !playObject.m_GetWhisperHuman.m_boGhost)
                {
                    PlayerSurfaceCore1Seams.SendMsg(playObject.m_GetWhisperHuman, Grobal2Const.RM_WHISPER, 0,
                        M2Config.btGMWhisperMsgFColor, M2Config.btGMWhisperMsgBColor, 0,
                        m_sCharName + "=>" + playObject.m_sCharName + " " + saystr);
                }
            }
            else
            {
                // 原文 3491-3496
                if (M2Config.boShowWhisperLevelMsg)
                {
                    PlayerSurfaceCore1Seams.SendMsg(playObject, Grobal2Const.RM_WHISPER, 0,
                        M2Config.btWhisperMsgFColor, M2Config.btWhisperMsgBColor, 0,
                        string.Format("{0} " + M2ShareState.g_sShowWhisperLevelMsg + "=> {1}",
                            m_sCharName, m_wAbil.Level, saystr));
                }
                else
                {
                    PlayerSurfaceCore1Seams.SendMsg(playObject, Grobal2Const.RM_WHISPER, 0,
                        M2Config.btWhisperMsgFColor, M2Config.btWhisperMsgBColor, 0,
                        string.Format("{0} => {1}", m_sCharName, saystr));
                }

                // 原文 3498-3500
                if (m_GetWhisperHuman != null && !m_GetWhisperHuman.m_boGhost)
                {
                    PlayerSurfaceCore1Seams.SendMsg(m_GetWhisperHuman, Grobal2Const.RM_WHISPER, 0,
                        M2Config.btWhisperMsgFColor, M2Config.btWhisperMsgBColor, 0,
                        m_sCharName + "=>" + playObject.m_sCharName + " " + saystr);
                }

                // 原文 3502-3504
                if (playObject.m_GetWhisperHuman != null && !playObject.m_GetWhisperHuman.m_boGhost)
                {
                    PlayerSurfaceCore1Seams.SendMsg(playObject.m_GetWhisperHuman, Grobal2Const.RM_WHISPER, 0,
                        M2Config.btWhisperMsgFColor, M2Config.btWhisperMsgBColor, 0,
                        m_sCharName + "=>" + playObject.m_sCharName + " " + saystr);
                }
            }
        }
        else
        {
            // 原文 3508：SysMsg(whostr + g_sUserNotOnLine, c_Red, t_Hint);
            SysMsg(whostr + PlayerSurfaceCore1Seams.g_sUserNotOnLine, TMsgColor.c_Red, TMsgType.t_Hint);
        }
    }

    // ==================================================================
    // IsBlockWhisper（原文 3511-3524）
    // ==================================================================

    /// <summary>
    /// 原文 `function TPlayObject.IsBlockWhisper(sName: string): Boolean;`（ObjPlayer.pas:3511-3524）。
    /// <code>
    ///   Result := False;
    ///   for I := 0 to m_BlockWhisperList.Count - 1 do
    ///     if CompareText(sName, m_BlockWhisperList.Strings[I]) = 0 then begin Result := True; Break; end;
    /// </code>
    /// ⚠ 原文用 **`CompareText`**（**大小写不敏感**、按当前 locale 的 `AnsiCompareText` 语义）；
    /// 托管侧用 `string.Equals(..., StringComparison.OrdinalIgnoreCase)` —— 对 ASCII 名字等价，
    /// 对非 ASCII 名字的 locale 折叠差异**未复刻**（原文在中文环境下也只折叠 ASCII 大小写）。
    /// </summary>
    public bool IsBlockWhisper(string sName)
    {
        // 原文 3515：Result := False;
        bool result = false;
        // 原文 3516-3523
        for (int i = 0; i <= m_BlockWhisperList.Count - 1; i++)
        {
            // 原文 3518：if CompareText(sName, m_BlockWhisperList.Strings[I]) = 0 then
            if (string.Equals(sName, m_BlockWhisperList[i], StringComparison.OrdinalIgnoreCase))
            {
                result = true;   // 原文 3520
                break;           // 原文 3521
            }
        }
        return result;
    }

    // ==================================================================
    // SendSocket（原文 3526-3553）—— 原文 virtual
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.SendSocket(DefMsg: pTDefaultMessage; sMsg: AnsiString); virtual;`
    /// （声明 ObjPlayer.pas:1191，实现 :3526-3553）—— **下发总闸门**。
    ///
    /// 逐行：
    /// <list type="number">
    ///   <item><description>:3532-3533 `if DefMsg = nil then MainOutMessage('SendSocket DefMsg is nil');`
    ///     —— ⚠ **只打日志、不早退**（下面照样继续用 `DefMsg`），见缺陷说明。</description></item>
    ///   <item><description>:3535-3536 `if m_boDummyObject then Exit;`</description></item>
    ///   <item><description>:3538-3539 `if m_boOffLine and (MyGetTickCount - m_nOffOnlineTick &gt; 1000 * 30) then Exit;`
    ///     —— 挂机超 30 秒**不再下发**。</description></item>
    ///   <item><description>:3541-3543 `SocketThread := RunSocket.GetSocket(m_nGateIdx); if SocketThread = nil then Exit;`</description></item>
    ///   <item><description>:3545-3553 `try SocketThread.Add(GM_DATA, m_nSocket, m_nGSocketIdx, 0, DefMsg, PAnsiChar(sMsg), Length(sMsg));
    ///     except on E: Exception do begin MainOutMessage(sExceptionMsg); MainOutMessage(E.Message); end; end;`
    ///     —— **异常被吞**（无 `raise`），只打两行日志。</description></item>
    /// </list>
    ///
    /// ★ 原文缺陷（原文 3532-3533，逐字保留）：`DefMsg = nil` 只 `MainOutMessage` 而**不 `Exit`**
    /// —— 随后 :3546 仍把 `nil` 交给 `SocketThread.Add`（Delphi 里会写野指针/后续 AV）。
    /// 托管侧 `TDefaultMessage` 是**值类型**（`struct`），不存在 `nil`，
    /// 故该分支在托管侧**不可达**；本实现**保留同样的日志点与"不早退"结构**
    /// （用一个恒 `false` 的判定占位），并在注释中留痕。
    ///
    /// ★ 原文如此（原文 3538 / 3568 / 3592 / 3604，四处**同一写法**）：
    /// `MyGetTickCount - m_nOffOnlineTick` 是 **LongWord（32 位无符号）减法**，
    /// 回绕时差值会变成接近 `2^32` 的数，于是"挂机刚上线"的瞬间也可能被判成"超 30 秒"。
    /// 托管侧用 `unchecked` 复刻回绕，锁定用例
    /// `ObjPlayerCore1Tests.SendSocket_TickWrapCanFalselySuppressSend_OriginalDefect`。
    /// </summary>
    /// <param name="defMsg">原文 `DefMsg: pTDefaultMessage`（托管为值类型，按值传）。</param>
    /// <param name="sMsg">原文 `sMsg: AnsiString`（托管 `string`，按 GBK 落地为字节）。</param>
    public virtual void SendSocket(TDefaultMessage defMsg, string sMsg)
    {
        // 原文 3532-3533：if DefMsg = nil then MainOutMessage('SendSocket DefMsg is nil');
        //   托管 `TDefaultMessage` 是值类型 ⇒ 该分支不可达；为**保留日志点与"不早退"结构**，
        //   用一个恒假判定占位（原文如此：不早退）。
        if (false)   // ★ 原文 3532 的 `DefMsg = nil` 在托管侧不可表达（值类型）
            PlayerSurfaceCore1Seams.MainOutMessage("SendSocket DefMsg is nil");

        // 原文 3535-3536：if m_boDummyObject then Exit;
        if (m_boDummyObject) return;

        // 原文 3538-3539：if m_boOffLine and (MyGetTickCount - m_nOffOnlineTick > 1000 * 30) then Exit;
        if (m_boOffLine && unchecked(PlayerSurfaceCore1Seams.MyGetTickCount() - m_nOffOnlineTick) > 1000 * 30)
            return;

        // 原文 3541-3543：SocketThread := RunSocket.GetSocket(m_nGateIdx); if SocketThread = nil then Exit;
        object? socketThread = PlayerSurfaceCore1Seams.RunSocketGetSocket(m_nGateIdx);
        if (socketThread == null) return;

        // 原文 3545-3553：try ... except on E: Exception do begin MainOutMessage(...); MainOutMessage(E.Message); end; end;
        try
        {
            // 原文 3546：SocketThread.Add(GM_DATA, m_nSocket, m_nGSocketIdx, 0, DefMsg, PAnsiChar(sMsg), Length(sMsg));
            byte[] bytes = DelphiRTL.AnsiBytes(sMsg);
            PlayerSurfaceCore1Seams.RunSocketAdd(socketThread, m_nSocket, m_nGSocketIdx, defMsg,
                new PlayerSurfaceSocketPayload(bytes, bytes.Length));
        }
        catch (Exception e)
        {
            // 原文 3550-3551：MainOutMessage(sExceptionMsg); MainOutMessage(E.Message);
            PlayerSurfaceCore1Seams.MainOutMessage("[Exception] TPlayObject.SendSocket..");
            PlayerSurfaceCore1Seams.MainOutMessage(e.Message);
        }
    }

    // ==================================================================
    // SendSocketEx（原文 3556-3584）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.SendSocketEx(DefMsg: pTDefaultMessage; Buffer: PAnsiChar; BufferLen: Integer);`
    /// （声明 ObjPlayer.pas:1192 —— **原文没有 `virtual`**，故托管侧同样不标虚，实现 :3556-3584）。
    ///
    /// 与 <see cref="SendSocket"/> **逐行同构**，唯一差别是 :3577
    /// 传的是 `Buffer` + `BufferLen`（二进制块）而不是 `PAnsiChar(sMsg)` + `Length(sMsg)`；
    /// :3576 那句 `// DefMsg.DataLen := Length(sMsg);` 是**注释掉的**（原文如此，保留）。
    /// 异常消息串也不同：`'[Exception] TPlayObject.SendSocketEx..'`（:3560）。
    /// </summary>
    public void SendSocketEx(TDefaultMessage defMsg, byte[] buffer)
    {
        // 原文 3562-3563：if DefMsg = nil then MainOutMessage('SendSocketEx DefMsg is nil');
        if (false)   // ★ 原文 3562 的 `DefMsg = nil` 在托管侧不可表达（值类型），结构保留
            PlayerSurfaceCore1Seams.MainOutMessage("SendSocketEx DefMsg is nil");

        // 原文 3565-3566：if m_boDummyObject then Exit;
        if (m_boDummyObject) return;

        // 原文 3568-3569：if m_boOffLine and (MyGetTickCount - m_nOffOnlineTick > 1000 * 30) then Exit;
        if (m_boOffLine && unchecked(PlayerSurfaceCore1Seams.MyGetTickCount() - m_nOffOnlineTick) > 1000 * 30)
            return;

        // 原文 3571-3573
        object? socketThread = PlayerSurfaceCore1Seams.RunSocketGetSocket(m_nGateIdx);
        if (socketThread == null) return;

        // 原文 3575-3584
        try
        {
            // 原文 3576：// DefMsg.DataLen := Length(sMsg);   ← 原文注释掉
            // 原文 3577：SocketThread.Add(GM_DATA, m_nSocket, m_nGSocketIdx, 0, DefMsg, Buffer, BufferLen);
            PlayerSurfaceCore1Seams.RunSocketAdd(socketThread, m_nSocket, m_nGSocketIdx, defMsg,
                new PlayerSurfaceSocketPayload(buffer, buffer.Length));
        }
        catch (Exception e)
        {
            // 原文 3581-3582
            PlayerSurfaceCore1Seams.MainOutMessage("[Exception] TPlayObject.SendSocketEx..");
            PlayerSurfaceCore1Seams.MainOutMessage(e.Message);
        }
    }

    // ==================================================================
    // SendOpenMagic（原文 3587-3597）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.SendOpenMagic(MagicID: Word; IsOpen: Boolean; AddData: Word);`
    /// （ObjPlayer.pas:3587-3597）。
    /// <code>
    ///   if m_boDummyObject then Exit;                                          // 3589-3590
    ///   if m_boOffLine and (MyGetTickCount - m_nOffOnlineTick > 1000 * 30) then Exit;  // 3592-3593
    ///   m_DefMsg := MakeDefaultMsg(SM_MAGIC_OPEN, 0, MagicID, Integer(IsOpen), AddData); // 3595
    ///   SendSocket(@m_DefMsg, '');                                             // 3596
    /// </code>
    /// ⚠ :3595 的 `Integer(IsOpen)` 是 Delphi 的**布尔→整数**转换（`True → 1`、`False → 0`）——
    /// 托管侧用 `isOpen ? 1 : 0` 表达，再经 `MakeDefaultMsg` 静默窄化到 `Word`
    /// （原文 `MakeDefaultMsg` 的 `wTag: Word` 形参同样会窄化）。
    /// </summary>
    public void SendOpenMagic(ushort magicId, bool isOpen, ushort addData)
    {
        // 原文 3589-3590
        if (m_boDummyObject) return;
        // 原文 3592-3593
        if (m_boOffLine && unchecked(PlayerSurfaceCore1Seams.MyGetTickCount() - m_nOffOnlineTick) > 1000 * 30)
            return;
        // 原文 3595：m_DefMsg := MakeDefaultMsg(SM_MAGIC_OPEN, 0, MagicID, Integer(IsOpen), AddData);
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_MAGIC_OPEN, 0, magicId, isOpen ? 1 : 0, addData);
        // 原文 3596：SendSocket(@m_DefMsg, '');
        SendSocket(m_DefMsg, "");
    }

    // ==================================================================
    // SendDefMessage（原文 3599-3612）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.SendDefMessage(wIdent: Word; nRecog: Int64; nParam, nTag, nSeries: Word;
    /// sMsg: AnsiString); // 004CAD6C`（ObjPlayer.pas:3599-3612）。
    /// <code>
    ///   if m_boDummyObject then Exit;                                     // 3601-3602
    ///   if m_boOffLine and (MyGetTickCount - m_nOffOnlineTick > 1000 * 30) then Exit;  // 3604-3605
    ///   m_DefMsg := MakeDefaultMsg(wIdent, nRecog, nParam, nTag, nSeries); // 3607
    ///   if sMsg <> '' then SendSocket(@m_DefMsg, EncodeString(sMsg))       // 3609
    ///   else               SendSocket(@m_DefMsg, '');                      // 3611
    /// </code>
    /// ★ 关键：非空消息走 **`EncodeString`（6-bit 编码）**，空消息走**空串**（不编码）。
    /// 托管 `EDcode.EncodeString(string)` 返回 `byte[]`（`EDcode.cs:327`），而
    /// <see cref="SendSocket"/> 的第二参是 `string`（= 原文 `AnsiString` 的字节承载口径）——
    /// 按 Core4 已确立的 1:1 映射（`PlayerSurfaceCore4Const.AnsiStringFromBytes`，**不复用**该类
    /// 以免跨切片耦合，此处用同一 Latin-1 口径就地表达）。
    /// </summary>
    /// <param name="wIdent">原文 `wIdent: Word`。</param>
    /// <param name="nRecog">原文 `nRecog: Int64`（托管 `long`）。</param>
    public void SendDefMessage(ushort wIdent, long nRecog, ushort nParam, ushort nTag, ushort nSeries, string sMsg)
    {
        // 原文 3601-3602
        if (m_boDummyObject) return;
        // 原文 3604-3605
        if (m_boOffLine && unchecked(PlayerSurfaceCore1Seams.MyGetTickCount() - m_nOffOnlineTick) > 1000 * 30)
            return;

        // 原文 3607：m_DefMsg := MakeDefaultMsg(wIdent, nRecog, nParam, nTag, nSeries);
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(wIdent, nRecog, nParam, nTag, nSeries);

        // 原文 3608-3611
        if (sMsg != "")
        {
            // 原文 3609：SendSocket(@m_DefMsg, EncodeString(sMsg));
            //   托管口径：AnsiString ↔ 字节的 1:1 映射（Latin-1），与 Core4 同一约定。
            SendSocket(m_DefMsg, System.Text.Encoding.Latin1.GetString(EDcode.EncodeString(sMsg)));
        }
        else
        {
            // 原文 3611：SendSocket(@m_DefMsg, '');
            SendSocket(m_DefMsg, "");
        }
    }

    // ==================================================================
    // ClientQueryAssessHero（原文 3614-3651）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.ClientQueryAssessHero(sData: string);`（ObjPlayer.pas:3614-3651）
    /// —— 「评估主副将英雄」查询。
    ///
    /// 逐行：
    /// <list type="number">
    ///   <item><description>:3619-3620 两个槽位的 `sHeroName` 先清空。</description></item>
    ///   <item><description>:3621 `if sData &lt;&gt; '' then` 才开始解析。</description></item>
    ///   <item><description>:3623 `sData := GetValidStr3_Ex(sData, sHeroData, '/')` —— 取第一段。</description></item>
    ///   <item><description>:3624-3625 非空则 `DecodeString(sHeroData, @m_AssessHeroInfos[0], SizeOf(TStorageHeroInfo))`
    ///     —— **原地把所有字节解到记录上**（覆盖整 64 字节，不是只写名字）。</description></item>
    ///   <item><description>:3627-3631 第二段同理写 `[1]`。</description></item>
    ///   <item><description>:3633-3641 **两个都非空**且 `[0].nLevel &lt; [1].nLevel` 时**整体交换**
    ///     （注释「调换一下把主将放在数据第一个位置」）。</description></item>
    ///   <item><description>:3643-3647 两个都非空 → `sData := EncodeBuffer([0]) + '/' + EncodeBuffer([1])`，
    ///     否则 `sData := ''`。</description></item>
    ///   <item><description>:3649-3650 `m_DefMsg := MakeDefaultMsg(SM_SENDSTORAGEHEROINFOEX, 0, 0, 0, 0); SendSocket(@m_DefMsg, sData);`</description></item>
    /// </list>
    ///
    /// ★ 原文如此（原文 3623-3630，逐字保留）：两次解析都用**同一个**临时变量 `sHeroData`，
    /// 且第二次解析（:3629）**先**调 `GetValidStr3_Ex` 再**无条件** `DecodeString`（:3630）
    /// —— 与第一次（:3624 有 `if sHeroData &lt;&gt; ''` 保护）**不对称**。
    /// 于是第二段为空串时仍会 `DecodeString('' , ...)`（清掉 `[1]` 的字节）。
    /// 逐字保留，不做对称化。
    ///
    /// ⚠ 托管侧 `TStorageHeroInfo` 是 `unsafe struct`（`Grobal2.Types5.cs:1005`），
    /// `DecodeString(sHeroData, @m_AssessHeroInfos[0], SizeOf(...))` 的"就地解码到记录"
    /// 用 `EDcode.DecodeString(byte[], byte[], int)` + `StructBytes.FromBytes` 表达
    /// （字节级等价；`SizeOf(TStorageHeroInfo)` = `ACTOR_NAME_LEN + 1 + 4 + 1 + 1`）。
    /// </remarks>
    public void ClientQueryAssessHero(string sData)
    {
        // 原文 3616-3617：var sHeroData: string; StorageHeroInfo: TStorageHeroInfo;
        //   ⚠ 托管侧 `sHeroData` 必须**显式初始化**：原文是过程级 `var`，
        //   Delphi 对 `string` 有零初始化 ⇒ 初值 `''`；C# 的局部变量无零初始化，
        //   且首次使用点是 `GetValidStr3_Ex(sData, ref sHeroData, '/')`（该 out/ref 形参
        //   在托管实现里可能只读不写）⇒ CS0165。置 `""` 与 Delphi 语义一致。
        string sHeroData = "";

        // 原文 3619-3620：两个槽位的英雄名先清空
        SetAssessHeroName(0, "");
        SetAssessHeroName(1, "");

        // 原文 3621：if sData <> '' then
        if (sData != "")
        {
            // 原文 3623：sData := GetValidStr3_Ex(sData, sHeroData, '/');
            sData = HUtil32.GetValidStr3_Ex(sData, ref sHeroData, '/');
            // 原文 3624-3625
            if (sHeroData != "")
                DecodeStorageHeroInfo(sHeroData, 0);

            // 原文 3627：if sData <> '' then
            if (sData != "")
            {
                // 原文 3629-3630（★ 原文如此：此处**无**非空保护）
                sData = HUtil32.GetValidStr3_Ex(sData, ref sHeroData, '/');
                DecodeStorageHeroInfo(sHeroData, 1);
            }

            // 原文 3633-3641：两个都有名字时，等级低的排到后面
            if (GetAssessHeroName(0) != "" && GetAssessHeroName(1) != "")
            {
                if (m_AssessHeroInfos[0].nLevel < m_AssessHeroInfos[1].nLevel)
                {
                    // 原文 3637-3639：StorageHeroInfo := [0]; [0] := [1]; [1] := StorageHeroInfo;
                    TStorageHeroInfo storageHeroInfo = m_AssessHeroInfos[0];
                    m_AssessHeroInfos[0] = m_AssessHeroInfos[1];
                    m_AssessHeroInfos[1] = storageHeroInfo;
                }
            }

            // 原文 3643-3647
            if (GetAssessHeroName(0) != "" && GetAssessHeroName(1) != "")
            {
                // 原文 3644-3645：sData := EncodeBuffer(@m_AssessHeroInfos[0], SizeOf(TStorageHeroInfo))
                //                     + '/' + EncodeBuffer(@m_AssessHeroInfos[1], SizeOf(TStorageHeroInfo));
                byte[] b0 = StructBytes.BytesOf(m_AssessHeroInfos[0]);
                byte[] b1 = StructBytes.BytesOf(m_AssessHeroInfos[1]);
                sData = System.Text.Encoding.Latin1.GetString(EDcode.EncodeBuffer(b0, b0.Length))
                      + "/"
                      + System.Text.Encoding.Latin1.GetString(EDcode.EncodeBuffer(b1, b1.Length));
            }
            else
                sData = "";   // 原文 3647
        }

        // 原文 3649-3650
        m_DefMsg = PlayerSurfacePack.MakeDefaultMsg(Grobal2Const.SM_SENDSTORAGEHEROINFOEX, 0, 0, 0, 0);
        SendSocket(m_DefMsg, sData);
    }

    /// <summary>原文 `m_AssessHeroInfos[i].sHeroName` 的读取（`TStorageHeroInfo.HeroName` 属性）。</summary>
    private string GetAssessHeroName(int index) => m_AssessHeroInfos[index].HeroName;

    /// <summary>原文 `m_AssessHeroInfos[i].sHeroName := value` 的写入。</summary>
    private void SetAssessHeroName(int index, string value)
    {
        TStorageHeroInfo info = m_AssessHeroInfos[index];
        info.HeroName = value;
        m_AssessHeroInfos[index] = info;
    }

    /// <summary>
    /// 原文 `DecodeString(sHeroData, @m_AssessHeroInfos[index], SizeOf(TStorageHeroInfo));`
    /// （ObjPlayer.pas:3625 / 3630）—— 6-bit 解码**原地**覆盖整条记录。
    /// </summary>
    private void DecodeStorageHeroInfo(string encoded, int index)
    {
        byte[] src = DelphiRTL.AnsiBytes(encoded);
        byte[] raw = new byte[StructBytes.SizeOf<TStorageHeroInfo>()];
        EDcode.DecodeString(src, raw, raw.Length);
        m_AssessHeroInfos[index] = StructBytes.FromBytes<TStorageHeroInfo>(raw);
    }

    // ==================================================================
    // GetHearMsgFColor / RefHearMsgColor（原文 3667-3678）
    // ==================================================================

    /// <summary>
    /// 原文 `function TPlayObject.GetHearMsgFColor: Byte; // 公聊字体颜色`（ObjPlayer.pas:3667-3673）。
    /// <code>
    ///   if MyGetTickCount &lt; m_dwHearMsgColorTick then Result := m_btHearMsgColor
    ///   else Result := g_Config.btHearMsgFColor;
    /// </code>
    /// ⚠ **判决方向容易读反**：`MyGetTickCount &lt; m_dwHearMsgColorTick` 为真 =
    /// 「当前时刻**还没到**覆盖截止点」→ 用**玩家自己的**颜色；否则用**全局配置**色。
    /// （原文没有任何地方把 `m_dwHearMsgColorTick` 设成"未来时刻"以外的用法，故时间回绕时
    /// 会整段翻转 —— 原文如此，不做保护。）
    /// </summary>
    public byte GetHearMsgFColor()
    {
        // 原文 3669-3672
        if (PlayerSurfaceCore1Seams.MyGetTickCount() < m_dwHearMsgColorTick)
            return m_btHearMsgColor;              // 原文 3670
        return M2Config.btHearMsgFColor;          // 原文 3672
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.RefHearMsgColor();`（ObjPlayer.pas:3675-3678）
    /// —— `SendRefMsg(RM_HEARCOLOR, m_btHearMsgColor, NativeInt(Self), 0, 0, '')`。
    /// ⚠ `m_btHearMsgColor`（`Byte`）作为 `wParam: NativeInt` 实参 —— 原文靠隐式拓宽；
    /// `NativeInt(Self)` 是**对象指针**，托管侧取 `m_nRecogId`（对象身份在托管侧的等价载体，
    /// 与 `UserItemToClientItem`/`SendAddItem` 的既有口径一致）。
    /// </summary>
    public void RefHearMsgColor()
    {
        // 原文 3677：SendRefMsg(RM_HEARCOLOR, m_btHearMsgColor, NativeInt(Self), 0, 0, '');
        SendRefMsg(Grobal2Const.RM_HEARCOLOR, m_btHearMsgColor, m_nRecogId, 0, 0, "", 0);
    }

    // ==================================================================
    // RefMyStatus（原文 3680-3684）
    // ==================================================================

    /// <summary>
    /// 原文 `procedure TPlayObject.RefMyStatus();`（ObjPlayer.pas:3680-3684）。
    /// <code>
    ///   RecalcAbilitys();                          // 3682
    ///   SendMsg(Self, RM_MYSTATUS, 0, 0, 0, 0, ''); // 3683
    /// </code>
    /// ★ **顺序不可换**：先重算能力、再发状态 —— 客户端拿到的 `RM_MYSTATUS`
    /// 一定对应重算后的 `m_WAbil`。
    /// ⚠ 原文调的是 **`RecalcAbilitys()`**（`TPlayObject` 的 `override`，声明 :1225）——
    /// 托管侧落在 `TCreature.RecalcAbilitys()`（`PlayerSurface/TCreature.PlayerSurface.Base.cs:341`，
    /// **virtual**），故此处直接调该虚方法（保留虚分派；本切片**不**重复声明 `RecalcAbilitys`）。
    /// </summary>
    public void RefMyStatus()
    {
        // 原文 3682：RecalcAbilitys();
        RecalcAbilitys();
        // 原文 3683：SendMsg(Self, RM_MYSTATUS, 0, 0, 0, 0, '');
        PlayerSurfaceCore1Seams.SendMsg(this, Grobal2Const.RM_MYSTATUS, 0, 0, 0, 0, "");
    }

    // ==================================================================
    // CanSaveToStorage / CanSaveToBigStorage（原文 3687-3696）
    // ==================================================================

    /// <summary>
    /// 原文 `function TPlayObject.CanSaveToStorage(Index: Integer): Boolean;`
    /// （声明 ObjPlayer.pas:1117，实现 :3687-3690）——「检查是否允许存仓库 -- piaoyun 2013-06-22」。
    /// <code>
    ///   Result := m_StorageItemList[Index].Count &lt; MAX_STORE_ITEM;
    /// </code>
    /// ⚠ `MAX_STORE_ITEM = 49`（`Grobal2.pas`，托管 `Grobal2Const.MAX_STORE_ITEM`，
    /// `Grobal2.Const.g.cs:24`）。
    /// ★ 原文**不判** `Index` 越界（`m_StorageItemList` 是 `array[0..3]`，传 4 会 AV）；
    /// 托管 `List&lt;TUserItem?&gt;[]` 同样会抛 `IndexOutOfRangeException` —— 行为等价，
    /// 锁定用例 `ObjPlayerCore1Tests.CanSaveToStorage_IndexOutOfRangeThrows_OriginalDefect`。
    /// </summary>
    public bool CanSaveToStorage(int index)
    {
        // 原文 3689：Result := m_StorageItemList[Index].Count < MAX_STORE_ITEM;
        return m_StorageItemList[index].Count < Grobal2Const.MAX_STORE_ITEM;
    }

    /// <summary>
    /// 原文 `function TPlayObject.CanSaveToBigStorage(): Boolean;`（声明 ObjPlayer.pas:1118，
    /// 实现 :3692-3696）。
    /// <code>
    ///   Result := (g_Config.boInfinityStorage or (m_nInfinityStorageExtCount &gt; 0)) and
    ///     (m_BigStorageItemList.Count &lt; g_Config.nInfinityStorageCount + m_nInfinityStorageExtCount);
    /// </code>
    /// ⚠ 两个条件**都要**成立：① 无限仓库总开关开 **或** 个人扩展数 &gt; 0；
    /// ② 当前件数 &lt; 配置容量 + 个人扩展数（原文**不做**下限保护，
    /// `nInfinityStorageCount + m_nInfinityStorageExtCount` 为负时比较结果随之为假）。
    /// </summary>
    public bool CanSaveToBigStorage()
    {
        // 原文 3694-3695
        return (M2Config.boInfinityStorage || m_nInfinityStorageExtCount > 0)
            && m_BigStorageItemList.Count < M2Config.nInfinityStorageCount + m_nInfinityStorageExtCount;
    }

    // ==================================================================
    // 以下 5 条：依赖尚未移植的类型/大段成员 → 显式留痕（台账 §48.1，禁止裸桩）
    // ==================================================================

    /// <summary>
    /// 原文 `constructor TPlayObject.Create;`（ObjPlayer.pas:1485-2505，**1021 行**）。
    ///
    /// <para><b>未移植（显式留痕）</b> —— 按任务书「Create 是巨型字段初始化例程：
    /// 若某个赋值块需要其它类/单元的成员，就对整个方法用 PortNotPorted」处理。
    /// 本切片**没有**做半移植。</para>
    ///
    /// <para><b>阻塞面（逐段，带原文行号）</b>：
    /// <list type="bullet">
    ///   <item><description>:1490 `PlayObjectMessageArray`（**单元级全局数组**，托管侧已由
    ///     `PlayerSurfaceMessageTable` 承载，但它是**静态表**、不需要逐实例 `FillChar`）。</description></item>
    ///   <item><description>:1491-1492 `m_ItemBoxItems`（`THumanItemBoxItems`，**类型未切出**）、
    ///     `m_ItemBoxAddIndex`。</description></item>
    ///   <item><description>:1494-1502 副本地图族 `m_FBEnvir`/`m_FBMapSendTimeEnvir`/`m_boFBSendTime`/
    ///     `m_boFBSendExitTime`/`m_boFBSendFailTime`/`m_dwFBExitTick`/`m_boMirrorMapSendTime`/
    ///     `m_MirrorMapSendTimeEnvir`/`m_boCollecting`（`TEnvirnoment` 引用族 + 若干标志）。</description></item>
    ///   <item><description>:1504-1515 状态标志族（部分托管已有：`m_boEmergencyClose`），
    ///     其余 `m_boSwitchData`/`m_boReconnection`/`m_boGotoSoftClose`/`m_boButch` 等未声明。</description></item>
    ///   <item><description>:1516-1518 `FillChar(m_QuestFlag, ...)`（**已有**，见
    ///     `PlayerSurfaceNpcSession.cs:139`）、`m_BonusAbil`/`m_CurBonusAbil`（`TNakedAbility`，
    ///     字段未声明）。</description></item>
    ///   <item><description>:1519-1900 主体：`m_nKey`、`m_VisibleEvents: THashList`、
    ///     `m_DealItemList`、`m_ChallengeItemList`、`m_StorageItemList[*] := TList.Create`、
    ///     `m_BigStorageItemList := TList.Create`、`m_BlockWhisperList := TStringList.Create`、
    ///     `m_TimeLabelList`、`m_ClientBufList`/`m_ArrBufList: TSafeList`、
    ///     `m_VisibleEvents`、`m_ArrayList`、`m_StringList`/`m_IntegerList`、
    ///     `m_CheckDenyLogonList` 等**容器构造**与约 700 个标量初值，
    ///     其中容器类型（`THashList`/`TSafeList`/`THashedStringList`/`TQuickList`）
    ///     托管侧**多数不存在**，逐块补齐等于把本切片扩成"半个 ObjPlayer 字段面"，
    ///     与任务书第 3 条（只补本切片需要的成员）冲突。</description></item>
    /// </list>
    /// 结论：**Create 的移植必须等上述容器类型落地**，否则只能造替身（违反 §14.2）。</para>
    /// </summary>
    public void Create()
    {
        PortNotPorted(nameof(Create), 1485);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.RunNotice;`（ObjPlayer.pas:2549-2631，**83 行**）。
    /// <para><b>未移植</b>。缺失成员（原文行号）：
    /// <list type="bullet">
    ///   <item><description><c>SendNotice(False/True)</c>（:2566 / :2577）—— 托管侧该方法
    ///     **留痕未移植**（`TPlayObject.PlayerSurface.Core4.cs:532`；其阻塞面：
    ///     `NoticeManager`、`WideReplaceText`、`FRecbNoticeCode`、`FIsSendRunNotice`）。</description></item>
    ///   <item><description><c>SendClientDataFile(Msg.sMsg)</c>（:2584）—— 未移植。</description></item>
    ///   <item><description><c>GetMessage(@Msg)</c>（:2579）—— 托管侧是 `MsgQueueConsumeCore.GetMessage`
    ///     的**静态纯函数**（`MsgQueueConsumeCore.cs:144`），签名与"逐条取本对象队列"不同。</description></item>
    ///   <item><description><c>m_ClientUIType: TClientUIType</c>（:2598）—— **类型未切出**
    ///     （`Grobal2` 无此枚举，`Client` 侧的同名物属 UI 层，不可引用）。</description></item>
    ///   <item><description><c>m_nViewRange</c>（:2595-2618）—— ⚠ 托管侧**同名成员属于 `TMonster`**
    ///     （`Engine/ObjBase.cs:220`），在 `TPlayObject` 上**不存在且不可重复声明**。</description></item>
    ///   <item><description><c>g_nSendRefMsgRange</c>（:2619）、<c>m_wScreenWidth</c>/<c>m_wScreenHeight</c>/
    ///     <c>m_wClientViewRange</c>/<c>m_nViewRangeExtY</c>/<c>m_dwClientTick</c>/
    ///     <c>m_boLoginNoticeOK</c>/<c>m_boIsMobile</c>（:2594-2623）、
    ///     <c>C_VIEWRANGEEXTY</c>（:2621，`M2Share.pas:3646` = 0）。</description></item>
    /// </list>
    /// 其中 `m_ClientUIType` 是**类型级**阻塞（不是"多声明几个标量就能过"），
    /// 故按任务书对 `Create` 的同一口径**整条留痕**，不做半移植。</para>
    /// </summary>
    public void RunNotice()
    {
        PortNotPorted(nameof(RunNotice), 2549);
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.DealCancel;`（ObjPlayer.pas:2507-2521）。
    /// <code>
    ///   if not m_boDealing then Exit;                                    // 2509-2510
    ///   m_boDealing := False;                                            // 2512
    ///   SendDefMessage(SM_DEALCANCEL, 0, 0, 0, 0, '');                   // 2513
    ///   if m_DealCreat &lt;&gt; nil then TPlayObject(m_DealCreat).DealCancel;  // 2514-2515
    ///   m_DealCreat := nil;                                              // 2517
    ///   GetBackDealItems();                                              // 2518
    ///   SysMsg(g_sDealActionCancelMsg { '交易取消' } , c_Green, t_Hint);   // 2519
    ///   m_DealLastTick := MyGetTickCount();                              // 2520
    /// </code>
    /// ★ **顺序不可换**：先清 `m_boDealing`、再发 `SM_DEALCANCEL`、再递归通知对手、
    /// 再置空 `m_DealCreat`、**然后才** `GetBackDealItems`（把交易栏物品收回背包）——
    /// 若先置空 `m_DealCreat` 就会丢掉"通知对手"这一步。
    ///
    /// ★ **递归**（:2514-2515）是原文的真实结构：对手若也在交易中，会被递归调用一次
    /// 来取消它那一边；对手的 `m_boDealing` 此时尚未被本方置空，故**不会无限递归**
    /// （第二层里对手的 `m_DealCreat` 指向本方，但本方 `m_boDealing` 已在 :2512 清零 ⇒ 第二层立即 Exit）。
    ///
    /// 接缝：`SendDefMessage` 复用本车道既有 <see cref="PlayerSurfaceMsgSeams.SendDefMessage"/>；
    /// `GetBackDealItems` 复用并行切片 `PlayerSurface.Core3.cs:1640` 的实现（**不重复移植**）；
    /// `g_sDealActionCancelMsg` 见 <see cref="PlayerSurfaceCore1Seams.g_sDealActionCancelMsg"/>。
    /// ⚠ `m_boDealing` / `m_DealCreat` / `m_DealLastTick` 由 Core3 声明（本文件不重复声明）。
    /// </summary>
    public void DealCancel()
    {
        // 原文 2509-2510：if not m_boDealing then Exit;
        if (!m_boDealing) return;

        // 原文 2512：m_boDealing := False;
        m_boDealing = false;

        // 原文 2513：SendDefMessage(SM_DEALCANCEL, 0, 0, 0, 0, '');
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_DEALCANCEL, 0, 0, 0, 0, "");

        // 原文 2514-2515：if m_DealCreat <> nil then TPlayObject(m_DealCreat).DealCancel;
        if (m_DealCreat != null)
            m_DealCreat.DealCancel();

        // 原文 2517：m_DealCreat := nil;
        m_DealCreat = null;

        // 原文 2518：GetBackDealItems();
        GetBackDealItems();

        // 原文 2519：SysMsg(g_sDealActionCancelMsg { '交易取消' } , c_Green, t_Hint);
        SysMsg(PlayerSurfaceCore1Seams.g_sDealActionCancelMsg, TMsgColor.c_Green, TMsgType.t_Hint);

        // 原文 2520：m_DealLastTick := MyGetTickCount();
        m_DealLastTick = PlayerSurfaceCore1Seams.MyGetTickCount();
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.DealCancelA;`（ObjPlayer.pas:2523-2527）。
    /// <code>
    ///   m_Abil.HP := m_WAbil.HP;   // 2525
    ///   DealCancel();              // 2526
    /// </code>
    /// ★ 语义：把「已扣但未成交」的血量恢复成主属性的 HP，再走取消交易。
    /// ⚠ 托管侧 `m_Abil` 是 Core3 提供的 **`ref` 别名**（`=> ref m_wAbil`），
    /// 故本行的**效果**退化为 `m_wAbil.HP := m_wAbil.HP`（自赋值、无副作用）——
    /// 这是 Core3 已登记的偏差（原文两字段独立）。逐字保留原文的书写形状，不做"优化"。
    /// </summary>
    public void DealCancelA()
    {
        // 原文 2525：m_Abil.HP := m_WAbil.HP;
        m_Abil.HP = m_WAbil.HP;
        // 原文 2526：DealCancel();
        DealCancel();
    }

    /// <summary>
    /// 原文 `procedure TPlayObject.RefUserState;`（ObjPlayer.pas:3653-3665）。
    /// <code>
    ///   n8 := 0;
    ///   if m_PEnvir.m_boFightZone then n8 := n8 or 1;   // 3658-3659
    ///   if m_PEnvir.m_boSAFE      then n8 := n8 or 2;   // 3660-3661
    ///   if m_boInFreePKArea       then n8 := n8 or 4;   // 3662-3663
    ///   SendDefMessage(SM_AREASTATE, n8, 0, 0, 0, '');  // 3664
    /// </code>
    /// ★ 三个位是 **or 叠加**（不是覆盖）：`1` 战斗区 / `2` 安全区 / `4` 自由 PK 区；
    /// 三个都成立时为 `7`。条件全假时下发 `n8 = 0`（**照样下发**，原文没有"无变化就不发"的短路）。
    /// 接缝：地图侧两个标志见 <see cref="PlayerSurfaceCore1Seams.MapStateFlags"/>。
    /// </summary>
    public void RefUserState()
    {
        // 原文 3657：n8 := 0;
        int n8 = 0;

        // 原文 3658-3661：两个地图标志（接缝）
        PlayerSurfaceCore1Seams.MapStateFlags(this, out bool fightZone, out bool safe);
        if (fightZone) n8 |= 1;      // 原文 3659
        if (safe) n8 |= 2;           // 原文 3661

        // 原文 3662-3663：if m_boInFreePKArea then n8 := n8 or 4;
        if (m_boInFreePKArea) n8 |= 4;

        // 原文 3664：SendDefMessage(SM_AREASTATE, n8, 0, 0, 0, '');
        PlayerSurfaceMsgSeams.SendDefMessage(this, (ushort)Grobal2Const.SM_AREASTATE, n8, 0, 0, 0, "");
    }
}

/// <summary>
/// 切片 Core1 需要的**原文常量**（托管 `Grobal2Const` / `M2Config` 未收录者）。
/// </summary>
public static class PlayerSurfaceCore1Const
{
    /// <summary>
    /// 原文 `MAXNG_LEVEL = High(Word);`（`M2Share.pas:203`）—— 内功等级上限。
    /// `High(Word)` = **65535**（`Word` 是 16 位无符号）。
    /// </summary>
    public const uint MAXNG_LEVEL = ushort.MaxValue;

    /// <summary>
    /// 原文 `g_Config.nNGMaxLevelLimte: Integer;`（`M2Share.pas:2287` 声明，
    /// :4961 typed constant 默认值 **65535**）—— 内功「等级上限门」。
    /// ⚠ 托管 `M2Config` **无该字段**（`grep -rn "nNGMaxLevelLimte" src/GXX.M2Server/Engine`
    /// 只命中本文件），故按任务书「常量不存在就在本文件声明并注释原文名」处理。
    /// </summary>
    public const int nNGMaxLevelLimte = 65535;

    /// <summary>
    /// 原文 `g_Config.nNGKillMonExpMultiple: Integer;`（`M2Share.pas:2300` 声明，
    /// :4976 typed constant 默认值 **40**，注释「杀怪内功经验倍数」）。
    /// ⚠ 托管 `M2Config` **无该字段**，同 <see cref="nNGMaxLevelLimte"/> 的处理口径。
    /// </summary>
    public const int nNGKillMonExpMultiple = 40;
}
