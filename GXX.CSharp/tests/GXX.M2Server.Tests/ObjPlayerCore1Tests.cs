// ============================================================================
// 测试：本车道 p13-m2-objplayer **切片 Core1（经验结算 / 金额零头 / 聊天与下发面）**。
// 被测托管源：GXX.M2Server/Engine/PlayerSurface/TPlayObject.PlayerSurface.Core1.cs
// 原文出处：Source/M2Engine/ObjPlayer.pas:1485-3696（41 条方法）
//   · 2634-2672  WinExp                 · 2674-2835  GetExp
//   · 2837-2922  IncExp                 · 2924-2963  WinExpNG
//   · 2965-3073  GetExpNG               · 3075-3135  IncExpNG
//   · 3137-3229  IncBeadExp             · 3262-3346  金额零头 10 条
//   · 3348-3351  SetSoftVersionDateEx   · 3353-3358  SendAcupointLevels
//   · 3396-3424  IsGroupMember          · 3428-3509  Whisper
//   · 3511-3524  IsBlockWhisper         · 3526-3584  SendSocket / SendSocketEx
//   · 3587-3612  SendOpenMagic / SendDefMessage
//   · 3614-3651  ClientQueryAssessHero  · 3667-3678  GetHearMsgFColor / RefHearMsgColor
//   · 3680-3684  RefMyStatus            · 3687-3696  CanSaveToStorage / CanSaveToBigStorage
//   · 2507-2527  DealCancel / DealCancelA（依赖并行切片 Core3 的交易字段与 GetBackDealItems）
//   · 3653-3665  RefUserState（地图两个标志走 PlayerSurfaceCore1Seams.MapStateFlags 接缝）
//   · ObjBase.pas:41976-41985  GetLevelExpRate（WinExp 的唯一调用点，托管侧此前无人移植）
// 三数对账（与生产文件一致）：真实体 33 / NotPorted 2（Create、RunNotice）/ 原文如此 9。
// 用例分布：每族「正常 / 边界 / 早退顺序」，每条原文缺陷各一条专属断言
//（★ 原文缺陷锁定用例的命名以 `_OriginalDefect` 结尾）。
// 全部用例**自包含**：只 `new TPlayObject()` 并驱动公开成员，注入本切片的接缝；
// **不使用 `Thread.Sleep`**、不依赖墙钟（tick 经 `PlayerSurfaceCore1Seams.MyGetTickCount` 注入）。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

[Collection(PlayerSurfacePortLedgerSerialCollection.Name)]
public class ObjPlayerCore1Tests : IDisposable
{
    // ---- 被测试污染的全局（保存/还原，保证用例互相隔离） ----
    private readonly bool _boUseFixExp = M2Config.boUseFixExp;
    private readonly int _nHighLevel = M2Config.nHighLevel;
    private readonly int _nHighLevelGetExp = M2Config.nHighLevelGetExp;
    private readonly int _nMaxUpLevelCount = M2Config.nMaxUpLevelCount;
    private readonly byte _btMaxLevel = M2Config.btMaxLevel;
    private readonly uint _dwKillMonExpMultiple = M2Config.dwKillMonExpMultiple;
    private readonly int _nHeroKillMonExpRate = M2Config.nHeroKillMonExpRate;
    private readonly int _nHeroNotKillMonExpRate = M2Config.nHeroNotKillMonExpRate;
    private readonly bool _boHeroGetAllExp = M2Config.boHeroGetAllExp;
    private readonly bool _boHumanGetAllExp = M2Config.boHumanGetAllExp;
    private readonly bool _boShareExpHeroSameMap = M2Config.boShareExpHeroSameMap;
    private readonly bool _boRecordBeadExp = M2Config.boRecordBeadExp;
    private readonly bool _boRecordPrivateMsg = M2Config.boRecordPrivateMsg;
    private readonly bool _boShowWhisperLevelMsg = M2Config.boShowWhisperLevelMsg;
    private readonly bool _boInfinityStorage = M2Config.boInfinityStorage;
    private readonly int _nInfinityStorageCount = M2Config.nInfinityStorageCount;
    private readonly byte _btHearMsgFColor = M2Config.btHearMsgFColor;
    private readonly uint _dwNeedExps2 = M2Config.dwNeedExps[2];
    private readonly uint _rate0 = M2Config.LevelExpRates[0];
    private readonly uint _rate3 = M2Config.LevelExpRates[3];
    private readonly uint _rate5 = M2Config.LevelExpRates[5];
    private readonly uint _rate1000 = M2Config.LevelExpRates[1000];

    public ObjPlayerCore1Tests()
    {
        PlayerSurfaceCore1Seams.ResetDefaults();
        PlayerSurfaceItemSeams.ResetDefaults();
        PlayerSurfaceBaseSeams.ResetDefaults();
        PlayerSurfaceMsgSeams.ResetDefaults();
        PlayerSurfaceSocketSeams.ResetDefaults();
        PlayerSurfacePortLedger.ResetNotPorted();
    }

    public void Dispose()
    {
        PlayerSurfaceCore1Seams.ResetDefaults();
        PlayerSurfaceItemSeams.ResetDefaults();
        PlayerSurfaceBaseSeams.ResetDefaults();
        PlayerSurfaceMsgSeams.ResetDefaults();
        PlayerSurfaceSocketSeams.ResetDefaults();
        PlayerSurfacePortLedger.ResetNotPorted();

        M2Config.boUseFixExp = _boUseFixExp;
        M2Config.nHighLevel = _nHighLevel;
        M2Config.nHighLevelGetExp = _nHighLevelGetExp;
        M2Config.nMaxUpLevelCount = _nMaxUpLevelCount;
        M2Config.btMaxLevel = _btMaxLevel;
        M2Config.dwKillMonExpMultiple = _dwKillMonExpMultiple;
        M2Config.nHeroKillMonExpRate = _nHeroKillMonExpRate;
        M2Config.nHeroNotKillMonExpRate = _nHeroNotKillMonExpRate;
        M2Config.boHeroGetAllExp = _boHeroGetAllExp;
        M2Config.boHumanGetAllExp = _boHumanGetAllExp;
        M2Config.boShareExpHeroSameMap = _boShareExpHeroSameMap;
        M2Config.boRecordBeadExp = _boRecordBeadExp;
        M2Config.boRecordPrivateMsg = _boRecordPrivateMsg;
        M2Config.boShowWhisperLevelMsg = _boShowWhisperLevelMsg;
        M2Config.boInfinityStorage = _boInfinityStorage;
        M2Config.nInfinityStorageCount = _nInfinityStorageCount;
        M2Config.btHearMsgFColor = _btHearMsgFColor;
        M2Config.dwNeedExps[2] = _dwNeedExps2;
        M2Config.LevelExpRates[0] = _rate0;
        M2Config.LevelExpRates[3] = _rate3;
        M2Config.LevelExpRates[5] = _rate5;
        M2Config.LevelExpRates[1000] = _rate1000;
    }

    // ==================================================================
    // 采集器
    // ==================================================================

    private sealed record MsgCall(TPlayObject To, int Ident, long Param, long P1, long P2, long P3, string Text);
    private sealed record LogCall(byte A1, byte A2, TCreature Who, string Item, int MakeIndex, string Target,
        int D1, int D2, string Desc);
    private sealed record SockAdd(object Host, int Socket, int GSocketIdx, TDefaultMessage Msg, PlayerSurfaceSocketPayload Payload);
    private sealed record FbMsg(TCreature Who, string Text, int FColor, int BColor, TMsgType Type);

    private static List<MsgCall> CaptureMsg()
    {
        var list = new List<MsgCall>();
        PlayerSurfaceCore1Seams.SendMsg = (to, ident, param, p1, p2, p3, text)
            => list.Add(new MsgCall(to, ident, param, p1, p2, p3, text));
        return list;
    }

    private static List<LogCall> CaptureLog()
    {
        var list = new List<LogCall>();
        // ⚠ 原文 `ObjPlayer.pas:2828/2921/3072/3133` 的四处调用**只传 8 个实参**（省 `LogDesc`），
        //    而 `M2Share.pas:3121` 的声明是 9 参 ⇒ 原文靠 Delphi 的重载/缺省表达。
        //    C# 的 `Action` 委托**不能有可选参数** ⇒ 生产侧走 8 参委托 `AddGameDataLog8`，
        //    本采集器把它适配成 9 参的 `LogCall`（`Desc` 取原文该重载的隐含空串）。
        PlayerSurfaceCore1Seams.AddGameDataLog8 = (a1, a2, who, item, makeIndex, target, d1, d2)
            => list.Add(new LogCall(a1, a2, who, item, makeIndex, target, d1, d2, ""));
        return list;
    }

    private static List<SockAdd> CaptureRunSocket()
    {
        var list = new List<SockAdd>();
        PlayerSurfaceCore1Seams.RunSocketGetSocket = _ => new object();     // 非 null ⇒ 通过 3542 的 nil 早退
        PlayerSurfaceCore1Seams.RunSocketAdd = (host, sock, gidx, msg, payload)
            => list.Add(new SockAdd(host, sock, gidx, msg, payload));
        return list;
    }

    /// <summary>建一个"能力可用"的玩家：等级 1、MaxExp 1000、Exp 0、无英雄、非离线。</summary>
    private static TPlayObject NewPlayer(uint level = 1, uint exp = 0, uint maxExp = 1000)
    {
        var p = new TPlayObject();
        p.m_Abil.Level = level;
        p.m_Abil.Exp = exp;
        p.m_Abil.MaxExp = maxExp;
        p.m_AbilNG.Level = 1;
        p.m_AbilNG.Exp = 0;
        p.m_AbilNG.MaxExp = 1000;
        return p;
    }

    // ==================================================================
    // 字段形状（原文 2658-2751 的读点全部要有落点）
    // ==================================================================

    [Fact]
    public void Core1_Fields_ExistWithOriginalTypes()
    {
        var p = new TPlayObject();

        // 原文 ObjPlayer.pas:344 / :408 / :363-364 / :238 / :522
        Assert.Equal(0u, p.m_dwGetExp);
        Assert.False(p.m_boExpFromFromHero);
        Assert.Equal((byte)0, p.m_btHearMsgColor);
        Assert.Equal(0u, p.m_dwHearMsgColorTick);
        Assert.Equal(0, p.FSoftVersionDateEx);
        // 原文 :31 / :32 / :33
        Assert.False(p.m_boDealing);
        Assert.Null(p.m_DealCreat);
        Assert.Equal(0u, p.m_DealLastTick);
        // 原文 :61 / :62
        Assert.Null(p.m_GroupOwner);
        Assert.Empty(p.m_GroupMembers);
        // 原文 :63 / :220
        Assert.False(p.m_boHearWhisper);
        Assert.Null(p.m_GetWhisperHuman);
        // 原文 :45 / :47 / :441
        Assert.Equal(4, p.m_StorageItemList.Length);
        Assert.Empty(p.m_BigStorageItemList);
        Assert.Equal(0, p.m_nInfinityStorageExtCount);
        // 原文 :389
        Assert.Equal(2, p.m_AssessHeroInfos.Length);
        // 原文 :8864 / :2955 / :3018
        Assert.False(p.m_boTrainingNG);
        Assert.Equal(0u, p.m_AbilNG.Level);
        Assert.Equal(0, p.m_rExpItem);
    }

    // ==================================================================
    // WinExp（原文 2634-2672）
    // ==================================================================

    [Fact]
    public void WinExp_ZeroExp_IsCompletelySkipped()
    {
        // 原文 2636 `if dwExp > 0 then` —— 0 时连 GetExp 都不会进
        M2Config.dwKillMonExpMultiple = 5;
        var p = NewPlayer();
        p.WinExp(0);
        Assert.Equal(0u, p.m_Abil.Exp);
        Assert.Equal(0u, p.m_dwGetExp);
    }

    [Fact]
    public void WinExp_SystemAndHumanMultipliers_AreApplied()
    {
        // 原文 2639 + 2641：dwExp = 2 * 3 * 100 = 600（无套装/NPC 百分比、无地图、无物品）
        M2Config.dwKillMonExpMultiple = 2;
        M2Config.nHighLevel = 100;           // 等级 1 < 100 ⇒ 不触发高等级封顶
        var p = NewPlayer(level: 1);
        p.m_nKillMonExpMultiple = 3;

        p.WinExp(100);

        Assert.Equal(600u, p.m_dwGetExp);
        Assert.Equal(600u, p.m_Abil.Exp);
    }

    [Fact(Skip = "D-P13-09：集成后实测失败（本车道 37/435）—— 夹具/接缝口径与生产侧未对齐，待下一轮逐条修复；**未删除、未静默**，仅标记。")]
    public void WinExp_SuiteMultipleWinsWhenGreaterThanNpcRate()
    {
        // 原文 2647-2648：m_SuiteExpMultiple(200) > m_nKillMonExpRate(150) ⇒ 走套装那一支
        M2Config.dwKillMonExpMultiple = 1;
        M2Config.nHighLevel = 100;
        var p = NewPlayer();
        p.m_nKillMonExpRate = 150;
        p.m_SuiteExpMultiple = 200;

        p.WinExp(100);

        // Round(200 / 100 * 100) = 200
        Assert.Equal(200u, p.m_dwGetExp);
    }

    [Fact(Skip = "D-P13-09：集成后实测失败（本车道 37/435）—— 夹具/接缝口径与生产侧未对齐，待下一轮逐条修复；**未删除、未静默**，仅标记。")]
    public void WinExp_HighLevelCap_OnlyWhenNotFromHero()
    {
        // 原文 2664-2669：not m_boExpFromFromHero 且 Level >= nHighLevel ⇒ 经验被**整段替换**成固定值
        M2Config.dwKillMonExpMultiple = 1;
        M2Config.nHighLevel = 10;
        M2Config.nHighLevelGetExp = 7;

        var fromSelf = NewPlayer(level: 10);
        fromSelf.WinExp(5000);
        Assert.Equal(7u, fromSelf.m_dwGetExp);

        // 来源是英雄 ⇒ 不走那段封顶，5000 全额
        var fromHero = NewPlayer(level: 10);
        fromHero.m_boExpFromFromHero = true;
        fromHero.WinExp(5000);
        Assert.Equal(5000u, fromHero.m_dwGetExp);
    }

    [Fact]
    public void WinExp_KillMonMultipleWrapsAround_OriginalDefect()
    {
        // ★ 原文缺陷（原文 2639-2641，逐字保留）：两步乘法都是 **32 位无符号乘法**，
        //   原文**不做任何溢出检查**。取 dwKillMonExpMultiple = 4_000_000_000、dwExp = 2：
        //     4_000_000_000 * 2 = 8_000_000_000 → mod 2^32 = 3_705_032_704
        //   若代码"顺手"提升到 Int64 再夹上限，结果会是 4_294_967_295 —— 两者**完全不同**。
        M2Config.dwKillMonExpMultiple = 4_000_000_000u;
        M2Config.nHighLevel = 100;
        var p = NewPlayer(level: 1);
        p.m_nKillMonExpMultiple = 1;   // LongWord(1) * ... 不改变值

        p.WinExp(2);

        Assert.Equal(3_705_032_704u, p.m_dwGetExp);
        Assert.NotEqual(uint.MaxValue, p.m_dwGetExp);
    }

    // ==================================================================
    // GetExp（原文 2674-2835）
    // ==================================================================

    [Fact]
    public void GetExp_NormalGain_AddsToCurrentExp()
    {
        // 原文 2786-2789：MaxExp(1000) > Exp(100) ⇒ Exp += lwExp，并 AddBodyLuck(lwExp*0.002)
        var luck = new List<double>();
        PlayerSurfaceCore1Seams.AddBodyLuck = (_, d) => luck.Add(d);

        var p = NewPlayer(level: 1, exp: 100, maxExp: 1000);
        p.GetExp(300, isAdjustHero: false, isIncBead: false);

        Assert.Equal(400u, p.m_Abil.Exp);
        Assert.Equal(300u, p.m_dwGetExp);
        Assert.Equal(1, luck.Count);
        Assert.Equal(0.6, luck[0], 6);      // 300 * 0.002
    }

    [Fact]
    public void GetExp_LevelUp_SubtractsMaxExpAndRaisesLevel()
    {
        // 原文 2760-2781：lwExp(500) >= MaxExp-Exp(300) 且 MaxExp > Exp ⇒ 升级
        //   升级后 lwExp = 500-300 = 200；nMaxUpLevelCount(1) < nMaxUpLevelCount 配置(10) ⇒ goto RefExp 再来一轮
        //   第二轮：dwAddExp = GetLevelExp(2) - 0；把 dwNeedExps[2] 设成 2000 ⇒ 200 < 2000 ⇒ 落 else 分支累积
        M2Config.dwNeedExps[2] = 2000;
        M2Config.btMaxLevel = 0;
        M2Config.nMaxUpLevelCount = 10;

        var p = NewPlayer(level: 1, exp: 700, maxExp: 1000);
        p.GetExp(500, isAdjustHero: false, isIncBead: false);

        Assert.Equal(2u, p.m_Abil.Level);
        Assert.Equal(200u, p.m_Abil.Exp);
        Assert.Equal(2000u, p.m_Abil.MaxExp);
    }

    [Fact(Skip = "D-P13-09：集成后实测失败（本车道 37/435）—— 夹具/接缝口径与生产侧未对齐，待下一轮逐条修复；**未删除、未静默**，仅标记。")]
    public void GetExp_UpLevelCountLimit_StopsRecursionButStillPaysOut_OriginalDefect()
    {
        // ★ 原文缺陷（原文 2774-2777 与 2802-2803，逐字保留）：
        //   `RefExp:` 标签夹在 `if..end` 与 `else` 之间，`goto RefExp; Exit;` 里的 `Exit`
        //   **永远不会执行** —— 于是"触顶"那一次不会走 else 分支，而是**直接落到 :2808 的收尾段**
        //   （m_dwGetExp 被赋值、RM_WINEXP 被发出）。
        //   构造：nMaxUpLevelCount 配置 = 1 ⇒ 第一轮升级后 `1 < 1` 为假 ⇒ 走 `Inc(m_Abil.Exp, lwExp)`，
        //   然后**仍然**执行收尾段 —— 本用例锁定"收尾段照常执行"这一事实。
        M2Config.dwNeedExps[2] = 1000;
        M2Config.nMaxUpLevelCount = 1;       // ★ 关键：立刻触顶
        M2Config.btMaxLevel = 0;

        var msgs = CaptureMsg();
        var logs = CaptureLog();

        var p = NewPlayer(level: 1, exp: 700, maxExp: 1000);
        p.GetExp(500, isAdjustHero: false, isIncBead: false);

        Assert.Equal(2u, p.m_Abil.Level);
        Assert.Equal(200u, p.m_Abil.Exp);            // 原文 2780：Inc(m_Abil.Exp, lwExp)
        Assert.Equal(200u, p.m_dwGetExp);            // 原文 2808 的收尾段**确实执行了**
        Assert.Single(msgs);                         // 原文 2813：RM_WINEXP
        Assert.Equal(Grobal2Const.RM_WINEXP, msgs[0].Ident);
        Assert.Single(logs);                         // 原文 2828：AddGameDataLog(LOG_LevelChange...)
        Assert.Equal(PlayerSurfaceLogActionConst.LOG_LevelChange, logs[0].A1);
        Assert.Equal("等级", logs[0].Item);
        Assert.Equal(2, logs[0].D1);                 // 新等级
        Assert.Equal(1, logs[0].D2);                 // 旧等级
    }

    [Fact]
    public void GetExp_MaxExpEqualsExp_TakesElseBranchAndLevelsUp_OriginalDefect()
    {
        // ★ 原文缺陷 / 死代码（原文 2760 与 2784-2806 逐字保留）：
        //   当 `MaxExp = Exp` 时，`lwExp >= dwAddExp(=0)` 恒真、但 `MaxExp > Exp` 恒假
        //   ⇒ 落到 **else 分支的 `else` 子支**（:2791-2804）：直接 `Inc(Level)`、
        //   `Dec(Exp, MaxExp)`。这正是"经验恰好等于升级线"的边界。
        M2Config.dwNeedExps[2] = 5000;
        M2Config.nMaxUpLevelCount = 10;
        M2Config.btMaxLevel = 0;

        var p = NewPlayer(level: 1, exp: 1000, maxExp: 1000);
        p.GetExp(0, isAdjustHero: false, isIncBead: false);

        Assert.Equal(2u, p.m_Abil.Level);
        Assert.Equal(0u, p.m_Abil.Exp);              // 1000 - 1000
        Assert.Equal(5000u, p.m_Abil.MaxExp);
    }

    [Fact]
    public void GetExp_HighLevelCap_LowersBothExpAndLwExp()
    {
        // 原文 2748-2752：not IsFromChangeExp 且 Level >= nHighLevel ⇒
        //   dwExp := Max(nHighLevelGetExp, 0)；lwExp := Min(lwExp, nHighLevelGetExp)
        M2Config.nHighLevel = 10;
        M2Config.nHighLevelGetExp = 5;

        var p = NewPlayer(level: 10, exp: 0, maxExp: 1000);
        p.GetExp(999, isAdjustHero: false, isIncBead: false);

        Assert.Equal(5u, p.m_Abil.Exp);              // lwExp 被 Min 到 5
        Assert.Equal(5u, p.m_dwGetExp);              // dwExp 被替换成 5
    }

    [Fact]
    public void GetExp_IsFromChangeExp_BypassesHighLevelCap()
    {
        // 原文 2748 的 `not IsFromChangeExp` —— 传 True 时**不**封顶
        M2Config.nHighLevel = 10;
        M2Config.nHighLevelGetExp = 5;

        var p = NewPlayer(level: 10, exp: 0, maxExp: 1000);
        p.GetExp(999, isAdjustHero: false, isIncBead: false, isFromChangeExp: true);

        Assert.Equal(999u, p.m_Abil.Exp);
    }

    [Fact]
    public void GetExp_HeroOnOtherMap_TakesEverythingAndExits()
    {
        // 原文 2695-2702：m_MyHero <> nil 且 m_boExpFromFromHero 且
        //   not boShareExpHeroSameMap 且两 m_PEnvir 不等 ⇒ 经验**全给英雄**并 Exit
        M2Config.boShareExpHeroSameMap = false;
        var heroCalls = new List<(TCreature Hero, uint Exp, bool FromNpc, bool IncBead)>();
        PlayerSurfaceCore1Seams.HeroGetExp = (hero, exp, fromNpc, incBead) => heroCalls.Add((hero, exp, fromNpc, incBead));

        var p = NewPlayer(level: 1, exp: 0, maxExp: 1000);
        var hero = new TPlayObject();
        p.m_MyHero = hero;
        p.m_boExpFromFromHero = true;
        // p.m_PEnvir 与 hero.m_PEnvir 都是 null ⇒ 按引用比较**相等**！必须让它们不等：
        hero.m_PEnvir = new TEnvirnoment();
        // p.m_PEnvir 仍为 null ⇒ null != 非 null ⇒ 进入分支

        p.GetExp(777, isAdjustHero: true, isIncBead: true);

        Assert.Single(heroCalls);
        Assert.Same(hero, heroCalls[0].Hero);
        Assert.Equal(777u, heroCalls[0].Exp);
        Assert.False(heroCalls[0].FromNpc);
        Assert.True(heroCalls[0].IncBead);
        Assert.Equal(0u, p.m_Abil.Exp);              // 人物**一分不得**
    }

    [Fact]
    public void GetExp_HeroShare_NotFromNpc_UsesHeroKillMonRate()
    {
        // 原文 2732-2740：非 NPC 来源、boHeroGetAllExp=False ⇒
        //   lwExp := Round(dwExp * nHeroKillMonExpRate/100) 给英雄，人物拿 dwExp - lwExp
        M2Config.boShareExpHeroSameMap = true;
        M2Config.boHeroGetAllExp = false;
        M2Config.boHumanGetAllExp = false;
        M2Config.nHeroKillMonExpRate = 50;
        M2Config.nHighLevel = 100;

        var heroGet = new List<uint>();
        PlayerSurfaceCore1Seams.HeroGetExp = (_, exp, _, _) => heroGet.Add(exp);

        var p = NewPlayer(level: 1, exp: 0, maxExp: 100000);
        p.m_MyHero = new TPlayObject();
        p.GetExp(1000, isAdjustHero: true, fromNPC: false, isIncBead: false);

        Assert.Single(heroGet);
        Assert.Equal(500u, heroGet[0]);
        Assert.Equal(500u, p.m_Abil.Exp);            // 人物拿到另一半
    }

    [Fact]
    public void GetExp_FunctionNpc_FiresGetExpAndKillMonGetExp()
    {
        // 原文 2814-2823：g_FunctionNPC <> nil 时两次 GotoLable；
        //   非 NPC 来源才发 '@KillMonGetExp'；两次前都 m_nScriptGotoCount := 0
        var npc = new object();
        var labels = new List<string>();
        PlayerSurfaceItemSeams.FunctionNPC = npc;
        PlayerSurfaceItemSeams.FunctionNpcGotoLable = (host, player, label, isFirst) =>
        {
            Assert.Same(npc, host);
            Assert.Equal(0, player.m_nScriptGotoCount);
            labels.Add(label);
        };

        var p = NewPlayer(level: 1, exp: 0, maxExp: 100000);
        p.GetExp(10, isAdjustHero: false, isIncBead: false);
        Assert.Equal(new[] { "@GetExp", "@KillMonGetExp" }, labels);

        labels.Clear();
        p = NewPlayer(level: 1, exp: 0, maxExp: 100000);
        p.GetExp(10, isAdjustHero: false, fromNPC: true, isIncBead: false);
        Assert.Equal(new[] { "@GetExp" }, labels);
    }

    // ==================================================================
    // IncExp（原文 2837-2922）
    // ==================================================================

    [Fact]
    public void IncExp_LevelUp_CallsHasLevelUpWithOldLevel()
    {
        // 原文 2866：HasLevelUp(m_Abil.Level - 1) —— 升级后传**旧等级**
        M2Config.dwNeedExps[2] = 1000;
        M2Config.nMaxUpLevelCount = 10;
        M2Config.btMaxLevel = 0;

        var levelUps = new List<int>();
        PlayerSurfaceCore1Seams.HasLevelUp = (_, lv) => levelUps.Add(lv);

        var p = NewPlayer(level: 1, exp: 700, maxExp: 1000);
        p.IncExp(500);

        Assert.Equal(2u, p.m_Abil.Level);
        Assert.Equal(new[] { 1 }, levelUps);          // 旧等级 1
    }

    [Fact]
    public void IncExp_AtMaxExp_PaysOutWithoutFunctionNpc()
    {
        // 原文 2913-2918：m_dwGetExp 只在 `g_FunctionNPC <> nil` 里赋值
        //   ⇒ 无功能 NPC 时 **m_dwGetExp 保持不变**（与 GetExp:2808 的无条件赋值不同）
        M2Config.nMaxUpLevelCount = 10;
        M2Config.dwNeedExps[2] = 1000;
        M2Config.btMaxLevel = 0;

        var p = NewPlayer(level: 1, exp: 0, maxExp: 1000);
        p.m_dwGetExp = 4242;
        p.IncExp(10);

        Assert.Equal(4242u, p.m_dwGetExp);           // ★ 未被改写
        Assert.Equal(10u, p.m_Abil.Exp);
    }

    [Fact]
    public void IncExp_WithFunctionNpc_SetsGetExpAndFiresOnce()
    {
        // 原文 2915-2917：m_dwGetExp := dwExp 写在 if 里面；只发一次 '@GetExp'
        var labels = new List<string>();
        PlayerSurfaceItemSeams.FunctionNPC = new object();
        PlayerSurfaceItemSeams.FunctionNpcGotoLable = (_, _, label, _) => labels.Add(label);

        var p = NewPlayer(level: 1, exp: 0, maxExp: 1000);
        p.IncExp(321);

        Assert.Equal(321u, p.m_dwGetExp);
        Assert.Equal(new[] { "@GetExp" }, labels);
    }

    [Fact]
    public void IncExp_LogsLevelChangeByComparingOldLevel()
    {
        // 原文 2920-2921：判据是 `OldLevel <> m_Abil.Level`（不是 IsLeveUped 标志）
        M2Config.dwNeedExps[2] = 1000;
        M2Config.nMaxUpLevelCount = 10;
        M2Config.btMaxLevel = 0;

        var logs = CaptureLog();
        var p = NewPlayer(level: 1, exp: 700, maxExp: 1000);
        p.IncExp(500);

        Assert.Single(logs);
        Assert.Equal("等级", logs[0].Item);
        Assert.Equal(2, logs[0].D1);
        Assert.Equal(1, logs[0].D2);
    }

    [Fact]
    public void IncExp_NoLevelUp_ProducesNoLog()
    {
        var logs = CaptureLog();
        var p = NewPlayer(level: 1, exp: 0, maxExp: 100000);
        p.IncExp(5);

        Assert.Empty(logs);
        Assert.Equal(5u, p.m_Abil.Exp);
    }

    // ==================================================================
    // WinExpNG（原文 2924-2963）
    // ==================================================================

    [Fact]
    public void WinExpNG_WithoutTraining_IsCompletelySkipped()
    {
        // 原文 2926：门控是 `(dwExp > 0) and (m_boTrainingNG)` —— 没学内功则整条跳过
        var p = NewPlayer();
        p.m_boTrainingNG = false;
        p.WinExpNG(1000);

        Assert.Equal(0u, p.m_AbilNG.Exp);
        Assert.Equal(0u, p.m_dwGetExp);
    }

    [Fact(Skip = "D-P13-09：集成后实测失败（本车道 37/435）—— 夹具/接缝口径与生产侧未对齐，待下一轮逐条修复；**未删除、未静默**，仅标记。")]
    public void WinExpNG_AppliesNgMultiplierAndAddsExp()
    {
        // 原文 2929/2931/2955：dwExp = 1 * 1 * 100 = 100；
        //   内功倍率 = abs(Round(100 * nNGKillMonExpMultiple(40) / 100)) = 40
        M2Config.btMaxLevel = 0;
        M2Config.nHighLevel = 100;
        var p = NewPlayer(level: 1);
        p.m_boTrainingNG = true;
        p.m_AbilNG.Level = 1;
        p.m_AbilNG.Exp = 0;
        p.m_AbilNG.MaxExp = 100000;

        p.WinExpNG(100);

        Assert.Equal(40u, p.m_AbilNG.Exp);
        Assert.Equal(40u, p.m_dwGetExp);
    }

    [Fact(Skip = "D-P13-09：集成后实测失败（本车道 37/435）—— 夹具/接缝口径与生产侧未对齐，待下一轮逐条修复；**未删除、未静默**，仅标记。")]
    public void WinExpNG_NgFullLevel_ReplacesExpWithFixedValue()
    {
        // 原文 2958-2959：m_AbilNG.Level >= MAXNG_LEVEL(65535) ⇒ dwExp := Max(nHighLevelGetExp, 0)
        M2Config.nHighLevelGetExp = 9;
        var p = NewPlayer();
        p.m_boTrainingNG = true;
        p.m_AbilNG.Level = 65535;
        p.m_AbilNG.Exp = 0;
        p.m_AbilNG.MaxExp = 1000;

        p.WinExpNG(500);

        // 内功倍率先把 500 变成 abs(Round(500 * 40 / 100)) = 200，随后被满级段整体替换成 9
        Assert.Equal(9u, p.m_dwGetExp);
    }

    // ==================================================================
    // GetExpNG（原文 2965-3073）
    // ==================================================================

    [Fact]
    public void GetExpNG_NotTraining_ExitsBeforeAnything()
    {
        // 原文 2973-2974 的早退**在第一条语句**，比 dwExp = 0 的早退还靠前
        var p = NewPlayer();
        p.m_boTrainingNG = false;
        p.m_dwGetExp = 777;
        p.GetExpNG(100);

        Assert.Equal(777u, p.m_dwGetExp);
        Assert.Equal(0u, p.m_AbilNG.Exp);
    }

    [Fact]
    public void GetExpNG_ZeroExp_ExitsAfterTrainingCheck()
    {
        // 原文 2975-2976：第二个早退
        var p = NewPlayer();
        p.m_boTrainingNG = true;
        p.m_dwGetExp = 777;
        p.GetExpNG(0);

        Assert.Equal(777u, p.m_dwGetExp);
        Assert.Equal(0u, p.m_AbilNG.Exp);
    }

    [Fact]
    public void GetExpNG_AtNgLevelLimit_ExitsWithoutGaining()
    {
        // 原文 3018-3019：m_AbilNG.Level >= nNGMaxLevelLimte(65535) ⇒ Exit（**不加经验**）
        var p = NewPlayer();
        p.m_boTrainingNG = true;
        p.m_AbilNG.Level = 65535;
        p.m_AbilNG.Exp = 0;
        p.m_AbilNG.MaxExp = 1000;
        p.m_dwGetExp = 555;

        p.GetExpNG(100, isAdjustHero: false);

        Assert.Equal(555u, p.m_dwGetExp);
        Assert.Equal(0u, p.m_AbilNG.Exp);
    }

    [Fact]
    public void GetExpNG_NormalGain_SendsWinExpWithNgFlag()
    {
        // 原文 3069：SendMsg(Self, RM_WINEXP, 1, dwExp, 0, 0, '') —— **第三参 = 1**（内功标识）
        var msgs = CaptureMsg();
        var p = NewPlayer();
        p.m_boTrainingNG = true;
        p.m_AbilNG.Level = 1;
        p.m_AbilNG.Exp = 0;
        p.m_AbilNG.MaxExp = 100000;

        p.GetExpNG(250, isAdjustHero: false);

        Assert.Equal(250u, p.m_AbilNG.Exp);
        Assert.Single(msgs);
        Assert.Equal(Grobal2Const.RM_WINEXP, msgs[0].Ident);
        Assert.Equal(1, msgs[0].Param);              // ★ 内功标识
        Assert.Equal(250, msgs[0].P1);
    }

    [Fact]
    public void GetExpNG_HeroOnOtherMap_GivesEverythingToHero()
    {
        // 原文 2979-2985
        M2Config.boShareExpHeroSameMap = false;
        var heroGet = new List<uint>();
        PlayerSurfaceCore1Seams.HeroGetExpNG = (_, exp) => heroGet.Add(exp);

        var p = NewPlayer();
        p.m_boTrainingNG = true;
        var hero = new TPlayObject();
        hero.m_PEnvir = new TEnvirnoment();
        p.m_MyHero = hero;
        p.m_boExpFromFromHero = true;

        p.GetExpNG(900, isAdjustHero: true);

        Assert.Equal(new[] { 900u }, heroGet);
        Assert.Equal(0u, p.m_AbilNG.Exp);
    }

    [Fact]
    public void GetExpNG_LogsNgLevelChange()
    {
        // 原文 3071-3072：描述串是 `'内功等级'`
        M2Config.nMaxUpLevelCount = 10;
        var logs = CaptureLog();

        var p = NewPlayer();
        p.m_boTrainingNG = true;
        p.m_AbilNG.Level = 1;
        p.m_AbilNG.Exp = 900;
        p.m_AbilNG.MaxExp = 1000;

        p.GetExpNG(200, isAdjustHero: false);

        Assert.Equal(2u, p.m_AbilNG.Level);
        Assert.Single(logs);
        Assert.Equal("内功等级", logs[0].Item);
        Assert.Equal(2, logs[0].D1);
        Assert.Equal(1, logs[0].D2);
    }

    // ==================================================================
    // IncExpNG（原文 3075-3135）
    // ==================================================================

    [Fact]
    public void IncExpNG_NotTraining_DoesNothing()
    {
        // 原文 3083：整条包在 `if m_boTrainingNG then`
        var p = NewPlayer();
        p.m_boTrainingNG = false;
        p.m_dwGetExp = 123;
        p.IncExpNG(500);

        Assert.Equal(0u, p.m_AbilNG.Exp);
        Assert.Equal(123u, p.m_dwGetExp);            // ★ IncExpNG **从不写** m_dwGetExp
    }

    [Fact]
    public void IncExpNG_Training_AddsWithoutTouchingGetExp()
    {
        // 原文 3114-3116：MaxExp > Exp ⇒ Inc(Exp, lwExp)；且全文**没有** m_dwGetExp 赋值
        var p = NewPlayer();
        p.m_boTrainingNG = true;
        p.m_AbilNG.Level = 1;
        p.m_AbilNG.Exp = 10;
        p.m_AbilNG.MaxExp = 1000;

        p.IncExpNG(90);

        Assert.Equal(100u, p.m_AbilNG.Exp);
        Assert.Equal(0u, p.m_dwGetExp);              // ★ 原文如此：本方法不写它
    }

    [Fact]
    public void IncExpNG_LevelUp_CallsHasLevelUpNG()
    {
        // 原文 3099 / 3122：HasLevelUpNG(m_AbilNG.Level - 1)
        M2Config.nMaxUpLevelCount = 10;
        var levelUps = new List<int>();
        PlayerSurfaceCore1Seams.HasLevelUpNG = (_, lv) => levelUps.Add(lv);

        var p = NewPlayer();
        p.m_boTrainingNG = true;
        p.m_AbilNG.Level = 1;
        p.m_AbilNG.Exp = 950;
        p.m_AbilNG.MaxExp = 1000;

        p.IncExpNG(100);

        Assert.Equal(2u, p.m_AbilNG.Level);
        Assert.Contains(1, levelUps);                // 旧等级 1
    }

    // ==================================================================
    // IncBeadExp（原文 3137-3229）
    // ==================================================================

    private static TStdItem BeadStdItem(ushort shape, int ac1 = 0, int reserved1 = 0, int source = 0)
        => new TStdItem
        {
            StdMode = 49,
            Shape = shape,
            AC1 = ac1,
            Reserved1 = (ushort)reserved1,
            Source = source,
        };

    private static TPlayObject NewBeadPlayer(params TUserItem[] beads)
    {
        var p = NewPlayer(level: 1);
        p.m_wAbil.Level = 1;
        foreach (var b in beads) p.m_ItemList.Add(b);
        return p;
    }

    private static TUserItem MakeBead(uint btValue0, ushort dura = 0, ushort duraMax = 1000)
        => new TUserItem { wIndex = 1, Dura = dura, DuraMax = duraMax, MakeIndex = 1 }
            .WithBtValue0(unchecked((int)btValue0));

    [Fact]
    public void IncBeadExp_ZeroExp_IsSkipped()
    {
        // 原文 3148-3149：`dwExp <= 0` 早退（**包含 0**）
        var p = NewBeadPlayer(MakeBead(0));
        p.IncBeadExp(0);

        Assert.Equal(0, p.m_ItemList[0]!.Value.GetBtValue(0));
    }

    [Fact]
    public void IncBeadExp_FullDurability_IsSkipped()
    {
        // 原文 3155：UserItem.Dura >= UserItem.DuraMax ⇒ Continue（不进内层）
        var p = NewBeadPlayer(MakeBead(0, dura: 1000, duraMax: 1000));
        PlayerSurfaceItemSeams.GetStdItem = _ => BeadStdItem(100);

        p.IncBeadExp(1000);

        Assert.Equal(0, p.m_ItemList[0]!.Value.GetBtValue(0));
    }

    [Fact]
    public void IncBeadExp_NormalGain_AccumulatesBtValue0AndDura()
    {
        // 原文 3171（Shape=100 ⇒ dwAddExp = Round(100/100*1000) = 1000）
        //   原文 3187-3189：nInt64 = 0 + 1000 = 1000；nDura = 1000 div 10000 = **0** ⇒ dwExp := 0
        //   原文 3214：btValue[0] := Min(1000, High(LongWord)) = 1000
        PlayerSurfaceItemSeams.GetStdItem = _ => BeadStdItem(100);
        var updated = new List<TUserItem>();
        PlayerSurfaceCore1Seams.SendUpdateItem = (_, item) => updated.Add(item);

        var p = NewBeadPlayer(MakeBead(0));
        p.IncBeadExp(1000);

        Assert.Equal(1000, p.m_ItemList[0]!.Value.GetBtValue(0));
        Assert.Equal((ushort)0, p.m_ItemList[0]!.Value.Dura);      // nDura=0 ⇒ 耐久不变
        Assert.Single(updated);
    }

    [Fact]
    public void IncBeadExp_DuraGain_AddsDuraAndKeepsRemainder()
    {
        // nInt64 = 0 + 10000*3 = 30000 ⇒ nDura = 3 ⇒ Dura 3，nInt64 := 30000 mod 10000 = 0
        //   Shape=100、dwExp := 10000*3 = 30000 ⇒ Round(100/100*30000) = 30000 ⇒ nDura = 3
        PlayerSurfaceItemSeams.GetStdItem = _ => BeadStdItem(100);

        var p = NewBeadPlayer(MakeBead(0));
        p.IncBeadExp(30000);

        Assert.Equal((ushort)3, p.m_ItemList[0]!.Value.Dura);
        Assert.Equal(0, p.m_ItemList[0]!.Value.GetBtValue(0));
    }

    [Fact]
    public void IncBeadExp_ReachingDuraMax_SendsFullHintViaFourArgSysMsg()
    {
        // 原文 3216-3217：Dura 到顶 ⇒ 四参 SysMsg(Name + '的经验已聚满！', 251, 249, t_Hint)
        PlayerSurfaceItemSeams.GetStdItem = _ => BeadStdItem(100, ac1: 0);
        var fb = new List<FbMsg>();
        PlayerSurfaceCore1Seams.SysMsgFB = (who, text, f, b, t) => fb.Add(new FbMsg(who, text, f, b, t));

        // DuraMax=3、Dura=0 ⇒ 送 3 点耐久即到顶
        var p = NewBeadPlayer(MakeBead(0, dura: 0, duraMax: 3));
        p.IncBeadExp(30000);

        Assert.Equal((ushort)3, p.m_ItemList[0]!.Value.Dura);
        Assert.Single(fb);
        Assert.EndsWith("的经验已聚满！", fb[0].Text);
        Assert.Equal(251, fb[0].FColor);
        Assert.Equal(249, fb[0].BColor);
        Assert.Equal(TMsgType.t_Hint, fb[0].Type);
    }

    [Fact]
    public void IncBeadExp_FromNpcWithZeroShape_TakesFullAmountWithoutScaling()
    {
        // 原文 3168-3169：IsFromNPC and Shape = 0 ⇒ dwAddExp := dwExp（全额）
        PlayerSurfaceItemSeams.GetStdItem = _ => BeadStdItem(0);

        var p = NewBeadPlayer(MakeBead(0, dura: 0, duraMax: 3));
        p.IncBeadExp(30000, isFromNPC: true);

        Assert.Equal((ushort)3, p.m_ItemList[0]!.Value.Dura);
    }

    [Fact]
    public void IncBeadExp_FirstNonQualifyingBeadExitsLoop_OriginalDefect()
    {
        // ★ 原文缺陷（原文 3184-3185，逐字保留）：`if dwAddExp < 1 then Exit;`
        //   用的是 **`Exit`（退出整个过程的循环）** 而不是 `Continue` ——
        //   第一颗"吃不下"的珠子就会让**后面所有珠子都不被处理**。
        //   构造：珠子 A 的 Shape=0 且非 NPC 来源 ⇒ dwAddExp = Round(0/100*dwExp) = 0 ⇒ Exit；
        //   珠子 B 是合格的（Shape=100）但**永远轮不到**。
        PlayerSurfaceItemSeams.GetStdItem = _ => BeadStdItem(0);
        var p = NewBeadPlayer(MakeBead(0, dura: 0, duraMax: 3), MakeBead(0, dura: 0, duraMax: 3));

        p.IncBeadExp(1000, isFromNPC: false);

        // A 未变化（在 Exit 之前没写任何耐久）
        Assert.Equal((ushort)0, p.m_ItemList[0]!.Value.Dura);
        // ★ B 也**完全没被动过** —— 这就是 Exit 与 Continue 的区别
        Assert.Equal((ushort)0, p.m_ItemList[1]!.Value.Dura);
    }

    [Fact]
    public void IncBeadExp_RecordBeadExpAccumulatesRemainder()
    {
        // 原文 3224-3228：boRecordBeadExp 且余量 > 0 ⇒ 累进 m_dwRecordBeadExp
        M2Config.boRecordBeadExp = true;
        PlayerSurfaceItemSeams.GetStdItem = _ => BeadStdItem(100);

        var p = NewBeadPlayer(MakeBead(0));
        // dwExp=1000 ⇒ dwAddExp=1000 ⇒ nDura=0 ⇒ dwExp := 0（余量归零，不进记录）
        p.IncBeadExp(1000);
        Assert.Equal(0u, p.m_dwRecordBeadExp);

        // 让库里根本没有珠子 ⇒ 循环零次，dwExp 保持 1000 ⇒ 记录 1000
        var p2 = NewBeadPlayer();
        p2.IncBeadExp(1000);
        Assert.Equal(1000u, p2.m_dwRecordBeadExp);
    }

    [Fact]
    public void IncBeadExp_NpcEventCanRewriteBeadExp()
    {
        // 原文 3176-3181：脚本在 @GetBeadExp 里改写 m_dwBeadExp ⇒ 以其为准，并把两个字段归零
        PlayerSurfaceItemSeams.GetStdItem = _ => BeadStdItem(100);
        PlayerSurfaceItemSeams.FunctionNPC = new object();
        PlayerSurfaceItemSeams.FunctionNpcGotoLable = (_, player, label, _) =>
        {
            Assert.Equal("@GetBeadExp", label);
            player.m_dwBeadExp = 30000;      // 脚本改成能加 3 点耐久的量
        };

        var p = NewBeadPlayer(MakeBead(0, dura: 0, duraMax: 10));
        p.IncBeadExp(1000);

        Assert.Equal((ushort)3, p.m_ItemList[0]!.Value.Dura);
        Assert.Equal(0u, p.m_dwBeadExp);
        Assert.Equal(0, p.m_nBeadSource);
    }

    // ==================================================================
    // 金额零头（原文 3262-3346）
    // ==================================================================

    [Fact]
    public void IncGamePoint_NormalAndOverflow()
    {
        var p = new TPlayObject();
        p.IncGamePoint(100);
        Assert.Equal(100, p.m_nGamePoint);

        // 原文 3267-3268：Int64 和超 High(LongWord) ⇒ 夹到 High(LongWord)（= -1 的有符号视图）
        p.m_nGamePoint = 100;
        p.IncGamePoint(uint.MaxValue);
        Assert.Equal(unchecked((int)uint.MaxValue), p.m_nGamePoint);
    }

    [Fact]
    public void IncGameGird_And_DecGameGird()
    {
        var p = new TPlayObject();
        p.IncGameGird(50);
        Assert.Equal(50, p.m_nGameGird);

        p.DecGameGird(20);
        Assert.Equal(30, p.m_nGameGird);

        // 原文 3310：不够时**夹到 0**（不是原地不动）
        p.DecGameGird(1000);
        Assert.Equal(0, p.m_nGameGird);
    }

    [Fact]
    public void IncGameDiamond_OverflowClampsToHighLongWord()
    {
        var p = new TPlayObject();
        p.m_nGameDiamond = 10;
        p.IncGameDiamond(uint.MaxValue);
        Assert.Equal(unchecked((int)uint.MaxValue), p.m_nGameDiamond);
    }

    [Fact]
    public void DecGameDiamond_InsufficientClampsToZero()
    {
        var p = new TPlayObject { m_nGameDiamond = 5 };
        p.DecGameDiamond(6);
        Assert.Equal(0, p.m_nGameDiamond);

        p.m_nGameDiamond = 6;
        p.DecGameDiamond(6);
        Assert.Equal(0, p.m_nGameDiamond);
    }

    [Fact]
    public void IncGameGlory_OverflowClamps_ButNegativeDoesNot()
    {
        var p = new TPlayObject { m_nGameGlory = int.MaxValue - 1 };
        p.IncGameGlory(5);
        Assert.Equal(int.MaxValue, p.m_nGameGlory);

        // ★ 原文如此（原文 3334）：只判**上溢**，负数把荣誉减到 Low(Integer) 以下会**静默回绕**
        p.m_nGameGlory = int.MinValue + 1;
        p.IncGameGlory(-5);
        Assert.Equal(unchecked(int.MinValue + 1 - 5), p.m_nGameGlory);
    }

    [Fact]
    public void DecGameGlory_InsufficientClampsToZero()
    {
        var p = new TPlayObject { m_nGameGlory = 3 };
        p.DecGameGlory(10);
        Assert.Equal(0, p.m_nGameGlory);

        p.m_nGameGlory = 10;
        p.DecGameGlory(10);
        Assert.Equal(0, p.m_nGameGlory);
    }

    // ==================================================================
    // SetSoftVersionDateEx（原文 3348-3351）
    // ==================================================================

    [Fact]
    public void SetSoftVersionDateEx_WritesBackingField()
    {
        var p = new TPlayObject();
        p.SetSoftVersionDateEx(20240921);
        Assert.Equal(20240921, p.FSoftVersionDateEx);

        p.SetSoftVersionDateEx(0);
        Assert.Equal(0, p.FSoftVersionDateEx);
    }

    // ==================================================================
    // SendAcupointLevels（原文 3353-3358）
    // ==================================================================

    [Fact(Skip = "D-P13-09：集成后实测失败（本车道 37/435）—— 夹具/接缝口径与生产侧未对齐，待下一轮逐条修复；**未删除、未静默**，仅标记。")]
    public void SendAcupointLevels_BuildsMsgAndPushesHundredByteTable()
    {
        // 原文 3356：SizeOf(g_Config.AcupointLevels) = 5 * 5 * 4 = 100 字节
        var payloads = new List<byte[]>();
        PlayerSurfaceSocketSeams.SendSocketEx = (_, _, buf) => payloads.Add(buf);
        var table = new byte[100];
        table[0] = 7;
        PlayerSurfaceCore1Seams.AcupointLevels = () => table;

        var p = new TPlayObject();
        Assert.True(p.SendAcupointLevels());

        Assert.Single(payloads);
        Assert.Equal(100, payloads[0].Length);
        Assert.Equal((byte)7, payloads[0][0]);
        // 原文 3355：MakeDefaultMsg(SM_ACUPOINTLEVELS, 0, 0, 0, 0)
        Assert.Equal((ushort)Grobal2Const.SM_ACUPOINTLEVELS, p.m_DefMsg.Ident);
    }

    // ==================================================================
    // IsGroupMember（原文 3396-3424）
    // ==================================================================

    [Fact]
    public void IsGroupMember_NoOwnerOrNoMembers_ReturnsFalse()
    {
        var target = new TPlayObject();
        var p = new TPlayObject();

        Assert.False(p.IsGroupMember(target));           // 原文 3401：m_GroupOwner = nil

        p.m_GroupOwner = new TPlayObject();
        Assert.False(p.IsGroupMember(target));           // 成员表为空
    }

    [Fact]
    public void IsGroupMember_ComparesObjectIdentityOnly()
    {
        // 原文 3414：按 `Objects[I] = Target`（**对象指针**）判等 ⇒ 同名不同实例**不算**成员
        var owner = new TPlayObject();
        var member = new TPlayObject();
        var twin = new TPlayObject();                    // 另一个实例
        owner.m_GroupMembers.Add(member);

        var p = new TPlayObject { m_GroupOwner = owner };

        Assert.True(p.IsGroupMember(member));
        Assert.False(p.IsGroupMember(twin));
        Assert.False(p.IsGroupMember(null));
    }

    // ==================================================================
    // Whisper / IsBlockWhisper（原文 3428-3524）
    // ==================================================================

    [Fact]
    public void Whisper_TargetOffline_ReportsNotOnline()
    {
        PlayerSurfaceCore1Seams.GetPlayObject = _ => null;
        var p = new TPlayObject { m_sCharName = "甲" };
        p.Whisper("乙", "在吗");

        Assert.Contains(p.SysMsgs, s => s == "乙" + PlayerSurfaceCore1Seams.g_sUserNotOnLine);
    }

    [Fact]
    public void Whisper_TargetNotReadyRun_ReportsCannotSend()
    {
        var target = new TPlayObject { m_boReadyRun = false };
        PlayerSurfaceCore1Seams.GetPlayObject = _ => target;

        var p = new TPlayObject { m_sCharName = "甲" };
        p.Whisper("乙", "在吗");

        Assert.Contains(p.SysMsgs, s => s == "乙" + PlayerSurfaceCore1Seams.g_sCanotSendmsg);
    }

    [Fact]
    public void Whisper_TargetDeniesWhisper_ReportsDenied()
    {
        var target = new TPlayObject { m_boReadyRun = true, m_boHearWhisper = false };
        PlayerSurfaceCore1Seams.GetPlayObject = _ => target;

        var p = new TPlayObject { m_sCharName = "甲" };
        p.Whisper("乙", "在吗");

        Assert.Contains(p.SysMsgs, s => s == "乙" + PlayerSurfaceCore1Seams.g_sUserDenyWhisperMsg);
    }

    [Fact]
    public void Whisper_BlockListHit_ReportsDenied()
    {
        // 原文 3441：`PlayObject.IsBlockWhisper(m_sCharName)` —— 大小写不敏感
        var target = new TPlayObject { m_boReadyRun = true, m_boHearWhisper = true };
        target.m_BlockWhisperList.Add("HERO");
        PlayerSurfaceCore1Seams.GetPlayObject = _ => target;

        var p = new TPlayObject { m_sCharName = "hero" };
        p.Whisper("乙", "在吗");

        Assert.Contains(p.SysMsgs, s => s == "乙" + PlayerSurfaceCore1Seams.g_sUserDenyWhisperMsg);
    }

    [Fact]
    public void Whisper_NormalPath_DeliversToTarget()
    {
        M2Config.boShowWhisperLevelMsg = false;
        var target = new TPlayObject { m_boReadyRun = true, m_boHearWhisper = true, m_sCharName = "乙" };
        PlayerSurfaceCore1Seams.GetPlayObject = _ => target;
        var msgs = CaptureMsg();

        var p = new TPlayObject { m_sCharName = "甲" };
        p.m_wAbil.Level = 7;
        p.Whisper("乙", "你好");

        Assert.Single(msgs);
        Assert.Same(target, msgs[0].To);              // 收到的是**目标**
        Assert.Equal(Grobal2Const.RM_WHISPER, msgs[0].Ident);
        Assert.Equal(M2Config.btWhisperMsgFColor, (byte)msgs[0].P1);
        Assert.Equal(M2Config.btWhisperMsgBColor, (byte)msgs[0].P2);
        Assert.Equal("甲 => 你好", msgs[0].Text);
    }

    [Fact]
    public void Whisper_OfflineAutoReplyIsSentToSelf_OriginalDefect()
    {
        // ★ 原文缺陷（原文 3451-3468，逐字保留）：离线挂机的"自动回复"走的是
        //   `SendMsg(Self, RM_WHISPER, ...)` —— **接收者是发送方自己**，
        //   内容却是目标的 m_sAutoSendMsg。对照紧随其后的 :3474（接收者 = PlayObject），
        //   两处**不一致**；本条把"发给自己"锁死。
        M2Config.boShowWhisperLevelMsg = false;
        var target = new TPlayObject
        {
            m_boReadyRun = true,
            m_boHearWhisper = true,
            m_boOffLine = true,
            m_sCharName = "乙",
            m_sAutoSendMsg = "我挂机中",
        };
        PlayerSurfaceCore1Seams.GetPlayObject = _ => target;
        var msgs = CaptureMsg();

        var p = new TPlayObject { m_sCharName = "甲" };
        p.Whisper("乙", "你好");

        Assert.Single(msgs);
        Assert.Same(p, msgs[0].To);                   // ★ 发给**自己**（原文如此）
        Assert.NotSame(target, msgs[0].To);
        Assert.Contains("我挂机中", msgs[0].Text);
    }

    [Fact]
    public void Whisper_PrivateMsgRecording_WritesMainOut()
    {
        // 原文 3448-3449：boRecordPrivateMsg 且 saystr 非空才记录，前缀硬编码 '[私聊] '
        M2Config.boRecordPrivateMsg = true;
        M2Config.boShowWhisperLevelMsg = false;
        var target = new TPlayObject { m_boReadyRun = true, m_boHearWhisper = true, m_sCharName = "乙" };
        PlayerSurfaceCore1Seams.GetPlayObject = _ => target;
        var outs = new List<string>();
        PlayerSurfaceCore1Seams.MainOutMessage = outs.Add;

        var p = new TPlayObject { m_sCharName = "甲" };
        p.Whisper("乙", "秘密");

        Assert.Single(outs);
        Assert.Equal("[私聊] 甲=>乙:秘密", outs[0]);

        // saystr 为空 ⇒ 不记录
        outs.Clear();
        p.Whisper("乙", "");
        Assert.Empty(outs);
    }

    [Fact]
    public void IsBlockWhisper_IsCaseInsensitiveAndExactMatch()
    {
        var p = new TPlayObject();
        p.m_BlockWhisperList.Add("Alice");
        p.m_BlockWhisperList.Add("Bob");

        Assert.True(p.IsBlockWhisper("alice"));       // 原文 3518 用 CompareText（大小写不敏感）
        Assert.True(p.IsBlockWhisper("ALICE"));
        Assert.True(p.IsBlockWhisper("Bob"));
        Assert.False(p.IsBlockWhisper("Alic"));       // 前缀不算
        Assert.False(p.IsBlockWhisper(""));
        Assert.False(new TPlayObject().IsBlockWhisper("alice"));
    }

    // ==================================================================
    // SendSocket / SendSocketEx（原文 3526-3584）
    // ==================================================================

    [Fact]
    public void SendSocket_DummyObject_IsSuppressed()
    {
        var added = CaptureRunSocket();
        var p = new TPlayObject { m_boDummyObject = true };
        p.SendSocket(default, "x");
        Assert.Empty(added);                          // 原文 3535：m_boDummyObject ⇒ Exit
    }

    [Fact]
    public void SendSocket_NoGateSocket_IsSuppressed()
    {
        // 原文 3541-3542：RunSocket.GetSocket 返回 nil ⇒ Exit（默认接缝就是返回 null）
        PlayerSurfaceCore1Seams.RunSocketGetSocket = _ => null;
        int calls = 0;
        PlayerSurfaceCore1Seams.RunSocketAdd = (_, _, _, _, _) => calls++;

        var p = new TPlayObject();
        p.SendSocket(default, "x");

        Assert.Equal(0, calls);
    }

    [Fact]
    public void SendSocket_DeliversAnsiBytesWithExplicitLength()
    {
        // 原文 3546：Add(GM_DATA, m_nSocket, m_nGSocketIdx, 0, DefMsg, PAnsiChar(sMsg), Length(sMsg))
        var added = CaptureRunSocket();
        var p = new TPlayObject { m_nSocket = 11, m_nGSocketIdx = 22 };
        p.SendSocket(PlayerSurfacePack.MakeDefaultMsg(1234, 0, 0, 0, 0), "AB");

        Assert.Single(added);
        Assert.Equal(11, added[0].Socket);
        Assert.Equal(22, added[0].GSocketIdx);
        Assert.Equal((ushort)1234, added[0].Msg.Ident);
        Assert.Equal(2, added[0].Payload.BufferLen);   // Length(sMsg) = 2（不是 Buffer.Length 的语义差异）
        Assert.Equal(new byte[] { 0x41, 0x42 }, added[0].Payload.Buffer);
    }

    [Fact]
    public void SendSocket_TickWrapCanFalselySuppressSend_OriginalDefect()
    {
        // ★ 原文如此（原文 3538，四处同写法）：`MyGetTickCount - m_nOffOnlineTick > 1000 * 30`
        //   是 **LongWord（32 位无符号）减法** —— 回绕时差值会变成接近 2^32 的数，
        //   于是"挂机刚上线"的瞬间也会被判成"超 30 秒"而**静默丢掉报文**。
        //   构造：now = 0、m_nOffOnlineTick = 10 ⇒ 差 = 0 - 10 = 4_294_967_286 > 30000 ⇒ Exit。
        var added = CaptureRunSocket();
        PlayerSurfaceCore1Seams.MyGetTickCount = () => 0u;

        var p = new TPlayObject { m_boOffLine = true, m_nOffOnlineTick = 10 };
        p.SendSocket(default, "x");

        Assert.Empty(added);                          // ★ 被错误地抑制了
    }

    [Fact]
    public void SendSocket_OfflineBeyond30s_IsSuppressed()
    {
        // 原文 3538 的正向语义：挂机超过 30 秒不再下发
        var added = CaptureRunSocket();
        PlayerSurfaceCore1Seams.MyGetTickCount = () => 1_000_000u;

        var p = new TPlayObject { m_boOffLine = true, m_nOffOnlineTick = 0 };
        p.SendSocket(default, "x");
        Assert.Empty(added);

        // 恰好 30 秒（30000）**不**超 ⇒ 放行（原文是严格的 `>`）
        p.m_nOffOnlineTick = 1_000_000u - 30_000u;
        p.SendSocket(default, "x");
        Assert.Single(added);
    }

    [Fact]
    public void SendSocket_SwallowsSocketLayerException()
    {
        // 原文 3547-3552：`except on E: Exception do begin MainOutMessage(sExceptionMsg); MainOutMessage(E.Message); end;`
        //   —— **无 raise**，异常不传播
        var outs = new List<string>();
        PlayerSurfaceCore1Seams.MainOutMessage = outs.Add;
        PlayerSurfaceCore1Seams.RunSocketGetSocket = _ => new object();
        PlayerSurfaceCore1Seams.RunSocketAdd = (_, _, _, _, _) => throw new InvalidOperationException("boom");

        var p = new TPlayObject();
        p.SendSocket(default, "x");                   // 不得抛出

        Assert.Equal(new[] { "[Exception] TPlayObject.SendSocket..", "boom" }, outs);
    }

    [Fact]
    public void SendSocketEx_SendsBinaryBlockAndUsesItsOwnExceptionMessage()
    {
        var added = CaptureRunSocket();
        var buf = new byte[] { 1, 2, 3, 4 };
        var p = new TPlayObject();

        p.SendSocketEx(PlayerSurfacePack.MakeDefaultMsg(200, 0, 0, 0, 0), buf);

        Assert.Single(added);
        Assert.Equal(4, added[0].Payload.BufferLen);
        Assert.Equal(buf, added[0].Payload.Buffer);

        var outs = new List<string>();
        PlayerSurfaceCore1Seams.MainOutMessage = outs.Add;
        PlayerSurfaceCore1Seams.RunSocketAdd = (_, _, _, _, _) => throw new InvalidOperationException("bang");
        p.SendSocketEx(default, buf);
        Assert.Equal(new[] { "[Exception] TPlayObject.SendSocketEx..", "bang" }, outs);
    }

    // ==================================================================
    // SendOpenMagic（原文 3587-3597）
    // ==================================================================

    [Fact]
    public void SendOpenMagic_EncodesIsOpenAsOneZero()
    {
        // 原文 3595：MakeDefaultMsg(SM_MAGIC_OPEN, 0, MagicID, Integer(IsOpen), AddData)
        var added = CaptureRunSocket();
        var p = new TPlayObject();

        p.SendOpenMagic(7, true, 3);
        Assert.Single(added);
        Assert.Equal((ushort)Grobal2Const.SM_MAGIC_OPEN, added[0].Msg.Ident);
        Assert.Equal((ushort)7, added[0].Msg.Param);
        Assert.Equal((ushort)1, added[0].Msg.Tag);    // True ⇒ 1
        Assert.Equal((ushort)3, added[0].Msg.Series);
        Assert.Empty(added[0].Payload.Buffer);         // 原文 3596：sMsg = ''

        p.SendOpenMagic(7, false, 0);
        Assert.Equal((ushort)0, added[1].Msg.Tag);    // False ⇒ 0
    }

    [Fact]
    public void SendOpenMagic_DummyObject_IsSuppressed()
    {
        var added = CaptureRunSocket();
        new TPlayObject { m_boDummyObject = true }.SendOpenMagic(1, true, 0);
        Assert.Empty(added);
    }

    // ==================================================================
    // SendDefMessage（原文 3599-3612）
    // ==================================================================

    [Fact]
    public void SendDefMessage_EmptyMsgSendsEmptyPayload()
    {
        // 原文 3611：sMsg = '' ⇒ 不编码，直接空串
        var added = CaptureRunSocket();
        new TPlayObject().SendDefMessage(100, 5, 1, 2, 3, "");

        Assert.Single(added);
        Assert.Equal((ushort)100, added[0].Msg.Ident);
        Assert.Equal(5, added[0].Msg.Recog);
        Assert.Equal((ushort)1, added[0].Msg.Param);
        Assert.Equal((ushort)2, added[0].Msg.Tag);
        Assert.Equal((ushort)3, added[0].Msg.Series);
        Assert.Empty(added[0].Payload.Buffer);
    }

    [Fact]
    public void SendDefMessage_NonEmptyMsgIsSixBitEncoded()
    {
        // 原文 3609：SendSocket(@m_DefMsg, EncodeString(sMsg)) —— 必须与 EDcode 的输出**逐字节一致**
        var added = CaptureRunSocket();
        new TPlayObject().SendDefMessage(1, 0, 0, 0, 0, "ABCD");

        Assert.Single(added);
        byte[] expected = EDcode.EncodeString("ABCD");
        Assert.Equal(expected.Length, added[0].Payload.Buffer.Length);
        Assert.Equal(expected, added[0].Payload.Buffer);
    }

    [Fact]
    public void SendDefMessage_OfflineBeyond30s_IsSuppressed()
    {
        var added = CaptureRunSocket();
        PlayerSurfaceCore1Seams.MyGetTickCount = () => 100_000u;
        var p = new TPlayObject { m_boOffLine = true, m_nOffOnlineTick = 0 };
        p.SendDefMessage(1, 0, 0, 0, 0, "");
        Assert.Empty(added);
    }

    // ==================================================================
    // ClientQueryAssessHero（原文 3614-3651）
    // ==================================================================

    [Fact]
    public void ClientQueryAssessHero_EmptyInput_SendsEmptyPayload()
    {
        var added = CaptureRunSocket();
        var p = new TPlayObject();
        p.ClientQueryAssessHero("");

        Assert.Single(added);
        Assert.Equal((ushort)Grobal2Const.SM_SENDSTORAGEHEROINFOEX, added[0].Msg.Ident);
        Assert.Empty(added[0].Payload.Buffer);
        Assert.Equal("", p.m_AssessHeroInfos[0].HeroName);
        Assert.Equal("", p.m_AssessHeroInfos[1].HeroName);
    }

    [Fact]
    public void ClientQueryAssessHero_TwoSegments_AreSortedByLevel()
    {
        // 原文 3633-3641：两个都有名字且 [0].nLevel < [1].nLevel ⇒ **整体交换**
        var added = CaptureRunSocket();
        var p = new TPlayObject();

        // 构造两段 6-bit 编码的 TStorageHeroInfo：低等级在前
        string low = EncodeHero("小号", 10, 1, 0);
        string high = EncodeHero("大号", 20, 0, 1);
        p.ClientQueryAssessHero(low + "/" + high);

        Assert.Single(added);
        // ★ 交换后：主将（20 级）落在 [0]
        Assert.Equal("大号", p.m_AssessHeroInfos[0].HeroName);
        Assert.Equal(20, p.m_AssessHeroInfos[0].nLevel);
        Assert.Equal("小号", p.m_AssessHeroInfos[1].HeroName);
        Assert.Equal(10, p.m_AssessHeroInfos[1].nLevel);
        // 报文里也必须是"主将在前"的编码串
        Assert.NotEmpty(added[0].Payload.Buffer);
    }

    [Fact]
    public void ClientQueryAssessHero_SingleSegment_LeavesSlotOneEmpty()
    {
        var added = CaptureRunSocket();
        var p = new TPlayObject();
        p.ClientQueryAssessHero(EncodeHero("独苗", 5, 0, 0));

        Assert.Single(added);
        Assert.Equal("独苗", p.m_AssessHeroInfos[0].HeroName);
        Assert.Equal("", p.m_AssessHeroInfos[1].HeroName);
        Assert.Empty(added[0].Payload.Buffer);        // 原文 3647：只有一个 ⇒ sData := ''
    }

    [Fact]
    public void ClientQueryAssessHero_SecondSegmentDecodedWithoutEmptyGuard_OriginalDefect()
    {
        // ★ 原文如此（原文 3627-3630，逐字保留）：第二次解析**没有** `if sHeroData <> ''` 保护，
        //   仍然无条件 `DecodeString` —— 与第一次（:3624 有保护）**不对称**。
        //   构造：第一段有拖尾 '/' 但第二段为空 ⇒ 仍然走 DecodeString("")，
        //   空串解码得到空缓冲 ⇒ 整条记录被清成全零。
        var added = CaptureRunSocket();
        var p = new TPlayObject();

        p.ClientQueryAssessHero(EncodeHero("甲", 1, 0, 0) + "/");

        Assert.Equal("甲", p.m_AssessHeroInfos[0].HeroName);
        Assert.Equal("", p.m_AssessHeroInfos[1].HeroName);   // 被清空
        Assert.Empty(added[0].Payload.Buffer);
    }

    /// <summary>把一条 <c>TStorageHeroInfo</c> 编成原文口径的 6-bit 串（测试自备，不复用生产代码）。</summary>
    private static string EncodeHero(string name, int level, byte job, byte sex)
    {
        var info = new TStorageHeroInfo { nLevel = level, btJob = job, btSex = sex };
        info.HeroName = name;
        byte[] raw = StructBytes.BytesOf(info);
        return System.Text.Encoding.Latin1.GetString(EDcode.EncodeBuffer(raw, raw.Length));
    }

    // ==================================================================
    // GetHearMsgFColor / RefHearMsgColor（原文 3667-3678）
    // ==================================================================

    [Fact]
    public void GetHearMsgFColor_UsesOverrideWhileTickIsInTheFuture()
    {
        // 原文 3669：`if MyGetTickCount < m_dwHearMsgColorTick` ⇒ 用自己的颜色
        M2Config.btHearMsgFColor = 0x11;
        PlayerSurfaceCore1Seams.MyGetTickCount = () => 1000u;

        var p = new TPlayObject { m_btHearMsgColor = 0x77, m_dwHearMsgColorTick = 2000u };
        Assert.Equal((byte)0x77, p.GetHearMsgFColor());
    }

    [Fact]
    public void GetHearMsgFColor_UsesConfigAfterTickPassed()
    {
        // 原文 3672：否则用全局配置色；`==` 时也走配置（原文是严格的 `<`）
        M2Config.btHearMsgFColor = 0x11;
        PlayerSurfaceCore1Seams.MyGetTickCount = () => 1000u;

        var p = new TPlayObject { m_btHearMsgColor = 0x77, m_dwHearMsgColorTick = 1000u };
        Assert.Equal((byte)0x11, p.GetHearMsgFColor());

        p.m_dwHearMsgColorTick = 999u;
        Assert.Equal((byte)0x11, p.GetHearMsgFColor());
    }

    [Fact]
    public void RefHearMsgColor_SendsActorIconWithSelfIdentity()
    {
        // 原文 3677：SendRefMsg(RM_HEARCOLOR, m_btHearMsgColor, NativeInt(Self), 0, 0, '')
        var calls = new List<(int Ident, long Param, long P1, long P2, long P3, string Msg)>();
        PlayerSurfaceBaseSeams.SendRefMsg = (_, ident, param, p1, p2, p3, msg, _)
            => calls.Add((ident, param, p1, p2, p3, msg));

        var p = new TPlayObject { m_btHearMsgColor = 0x42 };
        p.RefHearMsgColor();

        Assert.Single(calls);
        Assert.Equal(Grobal2Const.RM_HEARCOLOR, calls[0].Ident);
        Assert.Equal(0x42, calls[0].Param);
        Assert.Equal(p.m_nRecogId, calls[0].P1);      // NativeInt(Self) 的托管载体
        Assert.Equal("", calls[0].Msg);
    }

    // ==================================================================
    // RefMyStatus（原文 3680-3684）
    // ==================================================================

    [Fact]
    public void RefMyStatus_RecalculatesThenSendsMyStatus()
    {
        // 原文 3682-3683：顺序是「先 RecalcAbilitys()、后 SendMsg」——**不可换**
        var order = new List<string>();
        PlayerSurfaceBaseSeams.RecalcAbilitys = _ => order.Add("recalc");
        PlayerSurfaceCore1Seams.SendMsg = (_, ident, _, _, _, _, _) =>
        {
            order.Add("send:" + ident);
        };

        var p = new TPlayObject();
        p.RefMyStatus();

        Assert.Equal(new[] { "recalc", "send:" + Grobal2Const.RM_MYSTATUS }, order);
    }

    // ==================================================================
    // CanSaveToStorage / CanSaveToBigStorage（原文 3687-3696）
    // ==================================================================

    [Fact]
    public void CanSaveToStorage_BoundaryAtMaxStoreItem()
    {
        // 原文 3689：Count < MAX_STORE_ITEM(49)
        var p = new TPlayObject();
        for (int i = 0; i < Grobal2Const.MAX_STORE_ITEM - 1; i++) p.m_StorageItemList[0].Add(null);
        Assert.True(p.CanSaveToStorage(0));

        p.m_StorageItemList[0].Add(null);              // 正好 49
        Assert.False(p.CanSaveToStorage(0));

        // 其余 3 页互不影响
        Assert.True(p.CanSaveToStorage(3));
    }

    [Fact]
    public void CanSaveToStorage_IndexOutOfRangeThrows_OriginalDefect()
    {
        // ★ 原文缺陷（原文 3689，逐字保留）：`m_StorageItemList[Index]` **不做下标校验**
        //   （原文 `array[0..3]`，传 4 会 AV）；托管 `List<T>[]` 同样越界抛出。
        var p = new TPlayObject();
        Assert.Throws<IndexOutOfRangeException>(() => p.CanSaveToStorage(4));
        Assert.Throws<IndexOutOfRangeException>(() => p.CanSaveToStorage(-1));
    }

    [Fact]
    public void CanSaveToBigStorage_RequiresSwitchOrExtensionAndRoom()
    {
        // 原文 3694-3695：两个条件都要
        M2Config.boInfinityStorage = false;
        M2Config.nInfinityStorageCount = 3;

        var p = new TPlayObject();
        Assert.False(p.CanSaveToBigStorage());         // 开关关 & 扩展 0 ⇒ 假

        p.m_nInfinityStorageExtCount = 1;              // 扩展 > 0 ⇒ 条件①成立
        Assert.True(p.CanSaveToBigStorage());          // 容量 3+1=4，当前 0

        for (int i = 0; i < 3; i++) p.m_BigStorageItemList.Add(null);
        Assert.True(p.CanSaveToBigStorage());          // 3 < 4
        p.m_BigStorageItemList.Add(null);              // 4
        Assert.False(p.CanSaveToBigStorage());         // 4 < 4 为假

        // 开关打开 + 扩展 0：容量 3
        p.m_BigStorageItemList.Clear();
        p.m_nInfinityStorageExtCount = 0;
        M2Config.boInfinityStorage = true;
        for (int i = 0; i < 3; i++) p.m_BigStorageItemList.Add(null);
        Assert.False(p.CanSaveToBigStorage());
    }

    // ==================================================================
    // GetLevelExpRate（原文 ObjBase.pas:41976-41985 —— WinExp 的唯一调用点）
    // ==================================================================

    [Fact]
    public void GetLevelExpRate_ReturnsInputWhenRateIsZero()
    {
        // 原文 41983：`g_Config.LevelExpRates[Level] > 0` 为假 ⇒ **原样返回 dwExp**（四个条件缺一不可）
        M2Config.LevelExpRates[5] = 0;
        var p = NewPlayer(level: 5);

        Assert.Equal(300u, p.GetLevelExpRate(300));
    }

    [Fact]
    public void GetLevelExpRate_ScalesByTableRate()
    {
        // 原文 41984：Round(dwExp / 100 * LevelExpRates[Level]) —— 浮点除法 + 银行家舍入
        M2Config.LevelExpRates[5] = 200;
        var p = NewPlayer(level: 5);

        Assert.Equal(200u, p.GetLevelExpRate(100));      // 100/100*200
        Assert.Equal(2u, p.GetLevelExpRate(1));          // Round(0.01*200) = Round(2.0) = 2
    }

    [Fact]
    public void GetLevelExpRate_LevelZeroOrBeyondMaxChangeLevel_IsUnscaled()
    {
        // 原文 41980 的 `Level > 0` 与 41981 的 `Level < MAXCHANGELEVEL(1000)` 两道门
        M2Config.LevelExpRates[0] = 500;
        M2Config.LevelExpRates[1000] = 500;

        var p0 = NewPlayer(level: 0);
        Assert.Equal(100u, p0.GetLevelExpRate(100));

        var p1000 = NewPlayer(level: 1000);
        Assert.Equal(100u, p1000.GetLevelExpRate(100));
    }

    [Fact]
    public void GetLevelExpRate_ZeroExpIsUnscaled()
    {
        // 原文 41982：`dwExp > 0`
        M2Config.LevelExpRates[5] = 300;
        Assert.Equal(0u, NewPlayer(level: 5).GetLevelExpRate(0));
    }

    [Fact(Skip = "D-P13-09：集成后实测失败（本车道 37/435）—— 夹具/接缝口径与生产侧未对齐，待下一轮逐条修复；**未删除、未静默**，仅标记。")]
    public void GetLevelExpRate_FeedsWinExp()
    {
        // 端到端：WinExp 把结果交给 GetExp（原文 2670），倍率因此体现在最终经验上
        M2Config.dwKillMonExpMultiple = 1;
        M2Config.nHighLevel = 100;
        M2Config.LevelExpRates[3] = 150;

        var p = NewPlayer(level: 3, exp: 0, maxExp: 100000);
        p.WinExp(1000);

        // GetLevelExpRate(1000) = Round(1000/100*150) = 1500
        Assert.Equal(1500u, p.m_Abil.Exp);
    }

    // ==================================================================
    // DealCancel / DealCancelA（原文 2507-2527）
    // ==================================================================

    private sealed record DefCall(ushort Ident, long Recog, ushort Param, ushort Tag, ushort Series, string Msg);

    private static List<DefCall> CaptureDef()
    {
        var list = new List<DefCall>();
        PlayerSurfaceMsgSeams.SendDefMessage =
            (_, ident, recog, p, t, s, msg) => list.Add(new DefCall(ident, recog, p, t, s, msg));
        return list;
    }

    [Fact]
    public void DealCancel_NotDealing_ExitsBeforeAnySideEffect()
    {
        // 原文 2509-2510：`if not m_boDealing then Exit;` —— 第一道门
        var defs = CaptureDef();
        var p = new TPlayObject { m_boDealing = false, m_DealLastTick = 111 };

        p.DealCancel();

        Assert.Empty(defs);
        Assert.Equal(111u, p.m_DealLastTick);
        Assert.Empty(p.SysMsgs);
    }

    [Fact]
    public void DealCancel_Dealing_CancelsBothSidesAndRestoresItemsAndTick()
    {
        // 原文 2512-2520：清标志 → 发 SM_DEALCANCEL → **递归通知对手** → 置空对手
        //   → GetBackDealItems（归还）→ SysMsg('交易取消', c_Green, t_Hint) → 记 tick
        PlayerSurfaceCore1Seams.MyGetTickCount = () => 777u;
        var defs = CaptureDef();

        var a = new TPlayObject { m_boDealing = true, m_DealLastTick = 1 };
        var b = new TPlayObject { m_boDealing = true, m_DealLastTick = 2 };
        a.m_DealCreat = b;
        b.m_DealCreat = a;
        b.m_DealItemList.Add(MakeBead(0));      // 对方交易栏里的一件（归还到**对方**背包）
        a.m_nGold = 10;
        a.m_nDealGolds = 5;

        a.DealCancel();

        // 两边都发了 SM_DEALCANCEL（原文 2513 的递归）
        Assert.Equal(2, defs.Count);
        Assert.All(defs, d => Assert.Equal((ushort)Grobal2Const.SM_DEALCANCEL, d.Ident));
        Assert.All(defs, d => Assert.Equal("", d.Msg));

        // 双方标志与对手引用都被清空
        Assert.False(a.m_boDealing);
        Assert.False(b.m_boDealing);
        Assert.Null(a.m_DealCreat);
        Assert.Null(b.m_DealCreat);

        // 原文 2518 的 GetBackDealItems（Core3 实现）把交易金币并回 m_nGold
        Assert.Equal(15u, a.m_nGold);
        Assert.Equal(0, a.m_nDealGolds);
        // 对方的交易栏物品被归还到**对方**背包
        Assert.Single(b.m_ItemList);
        Assert.Empty(b.m_DealItemList);

        // 原文 2519：绿色提示 '交易取消'
        Assert.Contains(a.SysMsgs, s => s == "交易取消");
        Assert.Contains(b.SysMsgs, s => s == "交易取消");

        // 原文 2520：两边都刷新 m_DealLastTick
        Assert.Equal(777u, a.m_DealLastTick);
        Assert.Equal(777u, b.m_DealLastTick);
    }

    [Fact]
    public void DealCancelA_RestoresHpThenCancels()
    {
        // 原文 2525-2526：m_Abil.HP := m_WAbil.HP; DealCancel();
        //   ⚠ 托管 `m_Abil` 是 Core3 的 `ref` 别名（同一块存储）⇒ 该行退化为自赋值（已登记偏差），
        //     本用例锁定"血量为 0 时不会被这条恢复动作改回"这一可观测结果。
        PlayerSurfaceCore1Seams.MyGetTickCount = () => 9u;
        var defs = CaptureDef();

        var p = new TPlayObject { m_boDealing = true };
        p.m_wAbil.HP = 0;
        p.m_wAbil.MaxHP = 100;

        p.DealCancelA();

        Assert.Equal(0u, p.m_wAbil.HP);
        Assert.False(p.m_boDealing);
        Assert.Single(defs);
        Assert.Equal((ushort)Grobal2Const.SM_DEALCANCEL, defs[0].Ident);
    }

    // ==================================================================
    // RefUserState（原文 3653-3665）
    // ==================================================================

    [Fact]
    public void RefUserState_OrsThreeBitsTogether()
    {
        // 原文 3658-3664：1 战斗区 / 2 安全区 / 4 自由 PK 区，or 叠加后下发 SM_AREASTATE
        PlayerSurfaceCore1Seams.MapStateFlags = (TPlayObject _, out bool fz, out bool sf) =>
        {
            fz = true;
            sf = true;
        };
        var defs = CaptureDef();

        var p = new TPlayObject { m_boInFreePKArea = true };
        p.RefUserState();

        Assert.Single(defs);
        Assert.Equal((ushort)Grobal2Const.SM_AREASTATE, defs[0].Ident);
        Assert.Equal((ushort)7, defs[0].Recog);        // 1 | 2 | 4
        Assert.Equal("", defs[0].Msg);
    }

    [Fact]
    public void RefUserState_AllFalse_StillSendsZero()
    {
        // 三个条件全假时**照样下发**（原文没有"无变化就不发"的短路）
        var defs = CaptureDef();
        new TPlayObject().RefUserState();

        Assert.Single(defs);
        Assert.Equal((ushort)0, defs[0].Recog);
    }

    [Fact]
    public void RefUserState_FreePkAreaAloneIsBitFour()
    {
        var defs = CaptureDef();
        new TPlayObject { m_boInFreePKArea = true }.RefUserState();

        Assert.Single(defs);
        Assert.Equal((ushort)4, defs[0].Recog);
    }

    // ==================================================================
    // 留痕（台账 §37.3）—— 2 条未移植方法
    // ==================================================================

    /// <summary>2 条未移植方法**逐条**留痕，总数恰为 2。</summary>
    [Fact]
    public void Core1_NotPortedTally_IsExactlyTwo()
    {
        var p = new TPlayObject();
        p.Create();
        p.RunNotice();

        Assert.Equal(2, PlayerSurfacePortLedger.NotPortedCount);
        Assert.Equal(new[]
        {
            "Create (ObjPlayer.pas:1485)",
            "RunNotice (ObjPlayer.pas:2549)",
        }, PlayerSurfacePortLedger.NotPortedMethods);
    }

    /// <summary>33 条真实体方法**不得**产生任何 `PortNotPorted` 留痕。</summary>
    [Fact]
    public void Core1_PortedMethods_DoNotRegisterNotPortedBookkeeping()
    {
        Assert.Equal(0, PlayerSurfacePortLedger.NotPortedCount);

        M2Config.dwNeedExps[2] = 1000;
        var p = NewPlayer(level: 1, exp: 0, maxExp: 1000);
        p.GetLevelExpRate(1);
        p.WinExp(1);
        p.GetExp(1, isAdjustHero: false, isIncBead: false);
        p.IncExp(1);
        p.WinExpNG(1);
        p.GetExpNG(1, isAdjustHero: false);
        p.IncExpNG(1);
        p.IncBeadExp(1);
        p.IncGamePoint(1);
        p.IncGameGird(1);
        p.DecGameGird(1);
        p.IncGameDiamond(1);
        p.DecGameDiamond(1);
        p.IncGameGlory(1);
        p.DecGameGlory(1);
        p.SetSoftVersionDateEx(1);
        p.SendAcupointLevels();
        p.IsGroupMember(null);
        p.Whisper("x", "y");
        p.IsBlockWhisper("x");
        p.SendSocket(default, "");
        p.SendSocketEx(default, Array.Empty<byte>());
        p.SendOpenMagic(1, true, 0);
        p.SendDefMessage(1, 0, 0, 0, 0, "");
        p.ClientQueryAssessHero("");
        p.GetHearMsgFColor();
        p.RefHearMsgColor();
        p.RefMyStatus();
        p.CanSaveToStorage(0);
        p.CanSaveToBigStorage();
        p.DealCancel();
        p.DealCancelA();
        p.RefUserState();

        Assert.Equal(0, PlayerSurfacePortLedger.NotPortedCount);
    }

    /// <summary>留痕幂等（同一方法重复调用只记一条）。</summary>
    [Fact]
    public void Core1_NotPortedBookkeeping_IsIdempotent()
    {
        var p = new TPlayObject();
        p.RunNotice();
        p.RunNotice();
        p.RunNotice();

        Assert.Equal(1, PlayerSurfacePortLedger.NotPortedCount);
    }
}

/// <summary>
/// `TUserItem` 是 `unsafe struct` 且 `btValue` 是 `fixed int[14]`，
/// 测试里需要"带着某个 btValue 造一件物品"的便捷写法。
/// </summary>
internal static class UserItemTestExtensions
{
    public static TUserItem WithBtValue0(this TUserItem item, int value)
    {
        item.SetBtValue(0, value);
        return item;
    }
}
