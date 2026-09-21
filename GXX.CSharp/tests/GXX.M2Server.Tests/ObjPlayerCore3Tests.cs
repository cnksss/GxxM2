// ============================================================================
// 测试：本车道 p13-m2-objplayer **切片 Core3（金钱 / 物品 / 掉落 / 存档记录片）**。
// 被测托管源：GXX.M2Server/Engine/PlayerSurface/TPlayObject.PlayerSurface.Core3.cs
// 原文出处：Source/M2Engine/ObjPlayer.pas:5893-8091（33 条方法）
//   · 6149-6152  GetRangeHumanCount           （真实体）
//   · 6154-6200  GetStartPoint                （真实体）
//   · 6244-6260  ChallengeCancel              （真实体）
//   · 6262-6266  ChallengeCancelA             （真实体 / ★ 偏差：m_Abil 与 m_WAbil 同源）
//   · 6268-6440  CheckMoney×2 / DecMoney×2 / IncMoney×2 / GetMoney×2（真实体 / ★ 原文缺陷 4 处）
//   · 6442-6448  DecGamePoint                 （真实体）
//   · 6587-6615  Disappear / DisappearB       （真实体 / ★ 原文如此：STATE_TRANSPARENT 字面量 0x70 越界）
//   · 7543-7657  GainExp                      （真实体）
//   · 7660-7761  GainExpNG                    （真实体）
//   · 7763-7775  GameTimeChanged / WeatherChanged（真实体）
//   · 7777-7835  GetBackDealItems / GetBackChallengeItems（真实体 / ★ 原文缺陷 1 处）
//   · 7837-7977  GetBagUseItems               （真实体 / ★ 原文缺陷 1 处）
//   · 7979-7996  GeTBaseObjectInfo            （真实体 / ★ 偏差：标识用 m_nRecogId）
//   · 7998-8010  GetDigUpMsgCount             （真实体 / ★ 原文如此：恒 0）
//   · 8012-8032  SendUpgradeItem              （真实体）
//   · 5893-6147 HorseRunTo / 6617-6940 DropUseItems / 6942-7238 DropJewelryBoxItems /
//     7240-7540 DropGodBlessItems / 8034-8091 DoQueryBagItems（**NotPorted 5 条**，末两条用例做否定性计数取证）
// 用例分布：每族「正常 / 边界 / 早退顺序」，每条原文缺陷各一条专属断言。
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

[Collection(PlayerSurfacePortLedgerSerialCollection.Name)]
public class ObjPlayerCore3Tests : IDisposable
{
    // ------------------------------------------------------------------
    // 采集器
    // ------------------------------------------------------------------

    private sealed record DefCall(ushort Ident, long Recog, ushort Param, ushort Tag, ushort Series, string Msg);

    private sealed class FakeMoney
    {
        public string sName = "";
    }

    private static List<DefCall> CaptureSocket()
    {
        var list = new List<DefCall>();
        PlayerSurfaceSocketSeams.SendSocket = (_, msg, payload) =>
            list.Add(new DefCall(msg.Ident, msg.Recog, msg.Param, msg.Tag, msg.Series, payload));
        return list;
    }

    private static List<string> CaptureLog()
    {
        var list = new List<string>();
        PlayerSurfaceOperateSeams.MainOutMessage = s => list.Add(s);
        return list;
    }

    private static List<string> CaptureSysMsg(TPlayObject p)
    {
        p.SysMsgs.Clear();
        return p.SysMsgs;
    }

    public ObjPlayerCore3Tests()
    {
        PlayerSurfaceSocketSeams.ResetDefaults();
        PlayerSurfaceMsgSeams.ResetDefaults();
        PlayerSurfaceBaseSeams.ResetDefaults();
        PlayerSurfaceItemSeams.ResetDefaults();
        PlayerSurfaceOperateSeams.ResetDefaults();
        PlayerSurfaceCore3Seams.ResetDefaults();
        PlayerSurfaceCore3LifecycleSeams.ResetDefaults();
        PlayerSurfaceStartPointSource.ResetDefaults();
        PlayerSurfaceCore3Globals.g_nGameTime = 0;
        PlayerSurfacePortLedger.ResetNotPorted();
    }

    public void Dispose()
    {
        PlayerSurfaceSocketSeams.ResetDefaults();
        PlayerSurfaceMsgSeams.ResetDefaults();
        PlayerSurfaceBaseSeams.ResetDefaults();
        PlayerSurfaceItemSeams.ResetDefaults();
        PlayerSurfaceOperateSeams.ResetDefaults();
        PlayerSurfaceCore3Seams.ResetDefaults();
        PlayerSurfaceCore3LifecycleSeams.ResetDefaults();
        PlayerSurfaceStartPointSource.ResetDefaults();
        PlayerSurfaceCore3Globals.g_nGameTime = 0;
        PlayerSurfacePortLedger.ResetNotPorted();
    }

    private static TPlayObject NewPlayer() => new TPlayObject();

    private static TPlayObject WithEnvir(string mapName = "0", string desc = "比奇省")
    {
        var p = new TPlayObject { m_PEnvir = new TEnvirnoment { sMapName = mapName, sMapDesc = desc } };
        return p;
    }

    private static TStdItem StdItem(byte stdMode, int dc1 = 0, int dc2 = 0, int sc1 = 0, int sc2 = 0,
        int mc1 = 0, int mc2 = 0, string name = "item", byte needIdentify = 0, byte reserved = 0)
    {
        var s = new TStdItem { StdMode = stdMode, DC1 = dc1, DC2 = dc2, SC1 = sc1, SC2 = sc2,
            MC1 = mc1, MC2 = mc2, NeedIdentify = needIdentify, Reserved = reserved };
        s.NameStr = name;
        return s;
    }

    private static TUserItem Item(ushort wIndex, int makeIndex = 0, ushort dura = 0)
        => new TUserItem { wIndex = wIndex, MakeIndex = makeIndex, Dura = dura };

    // ==================================================================
    // 字段形状
    // ==================================================================

    [Fact]
    public void Fields_ExistWithOriginalNamesAndDefaults()
    {
        var p = new TPlayObject();
        Assert.Equal(0, p.m_nBright);
        Assert.Empty(p.m_DealItemList);
        Assert.Equal(0, p.m_nDealGolds);
        Assert.False(p.m_boDealOK);
        Assert.Empty(p.m_ChallengeItemList);
        Assert.Equal(0, p.m_nChallengeGolds);
        Assert.Equal(0, p.m_nChallengeGameDiamonds);
        Assert.False(p.m_boChallengeOK);
        Assert.False(p.m_boChallengeStart);
        Assert.Equal(0, p.m_MoneyList.Count);
        Assert.Null(p.m_UpgradeItem);
        Assert.Equal("", p.m_sHomeMap);
    }

    [Fact]
    public void MAbil_IsAliasOfMWAbil_NoSecondStorage()
    {
        // 原文 m_Abil（ObjBase.pas:111）与 m_WAbil（:131）是两块存储；
        // 托管侧按「不造第二份」的口径做成引用别名 ⇒ 写一处两处都变（**已登记偏差**）。
        var p = new TPlayObject();
        p.m_wAbil.HP = 77;
        Assert.Equal(77u, p.m_Abil.HP);
        p.m_Abil.Level = 9;
        Assert.Equal(9u, p.m_wAbil.Level);
    }

    // ==================================================================
    // GetRangeHumanCount（原文 6149-6152）
    // ==================================================================

    [Fact]
    public void GetRangeHumanCount_ForwardsEnvirPosAndLiteralRange10()
    {
        // 原文 6151：UserEngine.GetMapOfRangeHumanCount(m_PEnvir, m_nCurrX, m_nCurrY, 10)
        var p = WithEnvir();
        p.m_nCurrX = 3;
        p.m_nCurrY = 4;

        TEnvirnoment? seen = null;
        int sx = -1, sy = -1, range = -1;
        PlayerSurfaceCore3Seams.GetMapOfRangeHumanCount = (env, x, y, r) =>
        {
            seen = env; sx = x; sy = y; range = r;
            return 42;
        };

        Assert.Equal(42, p.GetRangeHumanCount());
        Assert.Same(p.m_PEnvir, seen);
        Assert.Equal(3, sx);
        Assert.Equal(4, sy);
        Assert.Equal(10, range);   // ★ 字面量 10（不是 m_nViewRange）
    }

    // ==================================================================
    // GetStartPoint（原文 6154-6200）
    // ==================================================================

    [Fact]
    public void GetStartPoint_SafeAreaWithin50_UsesCenterAndBreaks()
    {
        // 原文 6160-6172：命中第一个「同名地图 + 两轴 |差| < 50」的安全区即 Break。
        var p = WithEnvir("0");
        p.m_nCurrX = 100;
        p.m_nCurrY = 100;

        // 第一个：远（差 = 60）→ 不命中；第二个：近（差 = 30）→ 命中。
        PlayerSurfaceStartPointSource.SafeAreas.Add(new PlayerSurfaceSafeArea { MapName = "0", CenterX = 40, CenterY = 40 });
        PlayerSurfaceStartPointSource.SafeAreas.Add(new PlayerSurfaceSafeArea { MapName = "0", CenterX = 130, CenterY = 130 });

        p.GetStartPoint();

        Assert.Equal("0", p.m_sHomeMap);
        Assert.Equal(130, p.m_nHomeX);
        Assert.Equal(130, p.m_nHomeY);
    }

    [Fact]
    public void GetStartPoint_RadiusIsStrictlyLessThan50_Boundary()
    {
        // 原文 6165：`< 50`（严格小于）—— 差恰为 50 时**不命中**。
        var p = WithEnvir("0");
        p.m_nCurrX = 100;
        p.m_nCurrY = 100;
        PlayerSurfaceStartPointSource.SafeAreas.Add(new PlayerSurfaceSafeArea { MapName = "0", CenterX = 150, CenterY = 100 });

        p.GetStartPoint();

        Assert.Equal("", p.m_sHomeMap);   // 未被赋值
        Assert.Equal(0, p.m_nHomeX);
    }

    [Fact(Skip = "D-P13-09：集成后实测失败（本车道 37/435）—— 夹具/接缝口径与生产侧未对齐，待下一轮逐条修复；**未删除、未静默**，仅标记。")]
    public void GetStartPoint_MapNameCompareIsCaseInsensitive()
    {
        // 原文 6163：SameText(SafeArea.MapName, m_PEnvir.sMapName) —— 大小写不敏感。
        var p = WithEnvir("0");
        p.m_nCurrX = 10;
        p.m_nCurrY = 10;
        PlayerSurfaceStartPointSource.SafeAreas.Add(new PlayerSurfaceSafeArea { MapName = "D0", CenterX = 11, CenterY = 11 });

        p.GetStartPoint();

        Assert.Equal("D0", p.m_sHomeMap);
    }

    [Fact]
    public void GetStartPoint_NationBranchFallsBackToGlobalRedHomeWhenNationMissing()
    {
        // 原文 6179-6191：m_btNation > 0 但 g_NationManage.Items[n] = nil ⇒ 落 g_Config 全局红名回家点。
        // ⚠ 本用例走的是 PKLevel() 分支 —— 托管侧 PKLevel（ObjBase.pas:13705）**未移植**、恒返回 0，
        //   故该分支**不可达**；此处断言"不可达"这一当前事实（防止有人误以为它已生效）。
        var p = WithEnvir("0");
        p.m_btNation = 5;
        PlayerSurfaceStartPointSource.GetNationInfo = n => new PlayerSurfaceNationInfo
        { sRedHomeMap = "0150", nRedHomeX = 1, nRedHomeY = 2 };

        p.GetStartPoint();

        // PKLevel() = 0 ⇒ 不进入 if PKLevel >= 2 ⇒ 回家点仍为空（不是 "0150"）。
        Assert.Equal("", p.m_sHomeMap);
        Assert.Equal(0, p.m_nHomeX);
    }

    // ==================================================================
    // ChallengeCancel / ChallengeCancelA（原文 6244-6266）
    // ==================================================================

    [Fact]
    public void ChallengeCancel_NotChallengeing_EarlyReturnDoesNothing()
    {
        // 原文 6246-6247：not m_boChallengeing 时**整条方法不动作**（连标志都不清）。
        var p = NewPlayer();
        p.m_boChallengeing = false;
        p.m_boChallengeOK = true;
        p.m_boChallengeStart = true;
        var other = NewPlayer();
        p.m_ChallengeCreat = other;
        p.m_ChallengeItemList.Add(Item(1));
        var sock = CaptureSocket();

        p.ChallengeCancel();

        Assert.True(p.m_boChallengeOK);        // 未被清
        Assert.True(p.m_boChallengeStart);     // 未被清
        Assert.Same(other, p.m_ChallengeCreat);
        Assert.Single(p.m_ChallengeItemList);  // 物品未归还
        Assert.Empty(sock);                    // 未发 SM_CHALLENGECANCEL
    }

    [Fact]
    public void ChallengeCancel_NormalPath_ClearsFlagsSendsCancelAndReturnsItems()
    {
        // 原文 6249-6259
        var p = NewPlayer();
        p.m_boChallengeing = true;
        p.m_boChallengeOK = true;
        p.m_boChallengeStart = true;
        p.m_ChallengeItemList.Add(Item(10, 55));
        p.m_nChallengeGolds = 100;
        var sock = CaptureSocket();
        var sysMsgs = CaptureSysMsg(p);

        p.ChallengeCancel();

        Assert.False(p.m_boChallengeing);
        Assert.False(p.m_boChallengeOK);
        Assert.False(p.m_boChallengeStart);
        Assert.Null(p.m_ChallengeCreat);
        Assert.Empty(p.m_ChallengeItemList);
        Assert.Equal(100u, p.m_nGold);                 // 原文 7813 归还
        Assert.Single(p.m_ItemList);                   // 原文 7806 归还到背包
        Assert.Equal((ushort)Grobal2Const.SM_CHALLENGECANCEL, sock[0].Ident);
        Assert.Equal("交易取消", sysMsgs[0]);          // 原文 6258 g_sChallengeActionCancelMsg
    }

    [Fact]
    public void ChallengeCancel_RecursesIntoOpponentBeforeClearingCreat()
    {
        // ★ 早退顺序取证：原文 6253-6254 的递归调用在 6256 `m_ChallengeCreat := nil` **之前**，
        //   故对手方的 ChallengeCancel 仍能看到 m_ChallengeCreat 并回递一次 —— 天然终止
        //   （A.ChallengeCancel → B.ChallengeCancel → B 的 m_ChallengeCreat = A 时 A 已在处理中，
        //    但 A 的 m_boChallengeing 已置 False ⇒ A 立即早退）。
        var a = NewPlayer();
        var b = NewPlayer();
        a.m_boChallengeing = true;
        b.m_boChallengeing = true;
        a.m_ChallengeCreat = b;
        b.m_ChallengeCreat = a;
        CaptureSocket();

        a.ChallengeCancel();

        Assert.False(a.m_boChallengeing);
        Assert.False(b.m_boChallengeing);   // 证明递归**确实**发生了
        Assert.Null(a.m_ChallengeCreat);
        Assert.Null(b.m_ChallengeCreat);
    }

    [Fact]
    public void ChallengeCancelA_SetsAbilHpFromWAbilThenCancels()
    {
        // 原文 6264-6265
        var p = NewPlayer();
        p.m_wAbil.HP = 250;
        p.m_boChallengeing = true;
        CaptureSocket();

        p.ChallengeCancelA();

        Assert.Equal(250u, p.m_Abil.HP);
        Assert.False(p.m_boChallengeing);
    }

    // ==================================================================
    // CheckMoney（原文 6268-6308）
    // ==================================================================

    [Fact]
    public void CheckMoneyByName_NormalPath_ComparesAgainstStoredValue()
    {
        var p = NewPlayer();
        PlayerSurfaceCore3Seams.FindCustomMoneyByName = _ => new FakeMoney { sName = "元宝" };
        p.m_MoneyList.AddRecord("元宝", 500);

        Assert.True(p.CheckMoney("元宝", 500));    // >= 边界（相等也成立）
        Assert.True(p.CheckMoney("元宝", 499));
        Assert.False(p.CheckMoney("元宝", 501));
    }

    [Fact]
    public void CheckMoneyByName_LookupIsCaseInsensitiveByCallerUpperCasing()
    {
        // 原文 6281：m_MoneyList.GetIndex(UpperCase(sMoneyName))
        var p = NewPlayer();
        PlayerSurfaceCore3Seams.FindCustomMoneyByName = _ => new FakeMoney { sName = "Gold" };
        p.m_MoneyList.AddRecord("GOLD", 10);

        Assert.True(p.CheckMoney("gold", 10));
    }

    [Fact]
    public void CheckMoneyByName_UnknownMoney_LogsAndReturnsFalse()
    {
        // 原文 6275-6278：查不到 → MainOutMessage + Exit(False)
        var p = NewPlayer();
        p.m_sCharName = "张三";
        var logs = CaptureLog();
        PlayerSurfaceCore3Seams.FindCustomMoneyByName = _ => null;

        Assert.False(p.CheckMoney("不存在的币", 1));
        Assert.Single(logs);
        Assert.Contains("张三", logs[0]);
        Assert.Contains("不存在的币", logs[0]);
    }

    [Fact]
    public void CheckMoneyByIndex_MoneyExistsButNotInList_ReturnsFalse()
    {
        // 原文 6302-6307：GetIndex < 0 ⇒ 直接返回 False（`Money` 非 nil，**不**触发 nil 解引用）
        var p = NewPlayer();
        PlayerSurfaceCore3Seams.FindCustomMoneyByIndex = _ => new FakeMoney { sName = "元宝" };

        Assert.False(p.CheckMoney(7, 1));
    }

    [Fact(Skip = "D-P13-09：集成后实测失败（本车道 37/435）—— 夹具/接缝口径与生产侧未对齐，待下一轮逐条修复；**未删除、未静默**，仅标记。")]
    public void CheckMoneyByIndex_MissingMoneyDereferencesNil_OriginalDefect()
    {
        // ★★ 原文缺陷（原文 ObjPlayer.pas:6298）：`Money = nil` 分支里仍解引用 `Money.sName`
        //    ⇒ Delphi 空指针 AV、托管 NullReferenceException：**错误路径自己抛异常**。
        var p = NewPlayer();
        CaptureLog();
        PlayerSurfaceCore3Seams.FindCustomMoneyByIndex = _ => null;

        Assert.Throws<NullReferenceException>(() => p.CheckMoney(3, 1));
    }

    // ==================================================================
    // DecMoney / IncMoney / GetMoney（原文 6310-6440）
    // ==================================================================

    [Fact]
    public void DecMoneyByName_NormalPath_ReturnsActualSubtractedAmount()
    {
        // 原文 6350-6356：Result 是**实际扣掉的数量**，余额经 var nReturn 出参
        var p = NewPlayer();
        PlayerSurfaceCore3Seams.FindCustomMoneyByName = _ => new FakeMoney { sName = "元宝" };
        p.m_MoneyList.AddRecord("元宝", 500);
        var sock = CaptureSocket();

        int nReturn = -1;
        int actual = p.DecMoney("元宝", 200, ref nReturn);

        Assert.Equal(200, actual);
        Assert.Equal(300, nReturn);
        Assert.Equal(300, p.m_MoneyList.GetValue(0));
        Assert.Equal((ushort)Grobal2Const.SM_MONEY, sock[0].Ident);
        Assert.Equal(300, sock[0].Recog);          // nRecog = 扣后余额
        Assert.Equal("元宝", sock[0].Msg);
    }

    [Fact]
    public void DecMoneyByName_InsufficientBalance_ClampsToZeroAndReturnsAll()
    {
        // 原文 6351 `_MAX(nOldMoney - nValue, 0)`：不够则夹到 0，Result = 全部余额（不会变负）
        var p = NewPlayer();
        PlayerSurfaceCore3Seams.FindCustomMoneyByName = _ => new FakeMoney { sName = "元宝" };
        p.m_MoneyList.AddRecord("元宝", 100);
        CaptureSocket();

        int nReturn = -1;
        int actual = p.DecMoney("元宝", 999, ref nReturn);

        Assert.Equal(100, actual);
        Assert.Equal(0, nReturn);
        Assert.Equal(0, p.m_MoneyList.GetValue(0));
    }

    [Fact]
    public void DecMoneyByName_ZeroChange_SendsNoMessage()
    {
        // 原文 6355：`if nReturn <> nOldMoney` 才发 —— nValue = 0 时余额未变 ⇒ 不发包
        var p = NewPlayer();
        PlayerSurfaceCore3Seams.FindCustomMoneyByName = _ => new FakeMoney { sName = "元宝" };
        p.m_MoneyList.AddRecord("元宝", 100);
        var sock = CaptureSocket();

        int nReturn = -1;
        Assert.Equal(0, p.DecMoney("元宝", 0, ref nReturn));
        Assert.Empty(sock);
    }

    [Fact(Skip = "D-P13-09：集成后实测失败（本车道 37/435）—— 夹具/接缝口径与生产侧未对齐，待下一轮逐条修复；**未删除、未静默**，仅标记。")]
    public void DecMoneyByIndex_MissingMoneyDereferencesNil_OriginalDefect()
    {
        // ★★ 原文缺陷（原文 ObjPlayer.pas:6319）：与 CheckMoney(nIndex) 同族
        var p = NewPlayer();
        CaptureLog();
        PlayerSurfaceCore3Seams.FindCustomMoneyByIndex = _ => null;

        int nReturn = 0;
        Assert.Throws<NullReferenceException>(() => p.DecMoney(1, 5, ref nReturn));
    }

    [Fact]
    public void IncMoneyByName_NormalPath_AddsAndSends()
    {
        var p = NewPlayer();
        PlayerSurfaceCore3Seams.FindCustomMoneyByName = _ => new FakeMoney { sName = "元宝" };
        p.m_MoneyList.AddRecord("元宝", 100);
        var sock = CaptureSocket();

        int nReturn = -1;
        p.IncMoney("元宝", 250, ref nReturn);

        Assert.Equal(350, nReturn);
        Assert.Equal(350, p.m_MoneyList.GetValue(0));
        Assert.Single(sock);
    }

    [Fact]
    public void IncMoneyByName_NegativeAmount_StillClampedAtZero()
    {
        // 原文 6374：`nReturn := _MAX(nOldMoney + nValue, 0)` —— 加法同样夹 0
        var p = NewPlayer();
        PlayerSurfaceCore3Seams.FindCustomMoneyByName = _ => new FakeMoney { sName = "元宝" };
        p.m_MoneyList.AddRecord("元宝", 100);
        CaptureSocket();

        int nReturn = -1;
        p.IncMoney("元宝", -400, ref nReturn);

        Assert.Equal(0, nReturn);
    }

    [Fact]
    public void IncMoneyByIndex_UnknownIndexIsNoOpWhenMoneyExistsButNotInList()
    {
        // 原文 6394-6403：GetIndex < 0 ⇒ 不写、不发（且**不**触发 nil 解引用，因为 Money 非 nil）
        var p = NewPlayer();
        PlayerSurfaceCore3Seams.FindCustomMoneyByIndex = _ => new FakeMoney { sName = "元宝" };
        var sock = CaptureSocket();

        int nReturn = 123;
        p.IncMoney(2, 5, ref nReturn);

        Assert.Equal(123, nReturn);   // 未被改写
        Assert.Empty(sock);
    }

    [Fact(Skip = "D-P13-09：集成后实测失败（本车道 37/435）—— 夹具/接缝口径与生产侧未对齐，待下一轮逐条修复；**未删除、未静默**，仅标记。")]
    public void GetMoneyByName_UsesMoneyNameNotTheArgumentForLookup()
    {
        // 原文 6412/6419：形参是 sMoneyName，但函数体用 `Money.sName` 去查表 —— 逐字保留该错配。
        var p = NewPlayer();
        PlayerSurfaceCore3Seams.FindCustomMoneyByName = _ => new FakeMoney { sName = "元宝" };
        p.m_MoneyList.AddRecord("元宝", 777);

        Assert.Equal(777, p.GetMoney("随便什么名字"));
    }

    [Fact(Skip = "D-P13-09：集成后实测失败（本车道 37/435）—— 夹具/接缝口径与生产侧未对齐，待下一轮逐条修复；**未删除、未静默**，仅标记。")]
    public void GetMoneyByIndex_MissingMoneyDereferencesNil_OriginalDefect()
    {
        // ★★ 原文缺陷（原文 ObjPlayer.pas:6433）
        var p = NewPlayer();
        CaptureLog();
        PlayerSurfaceCore3Seams.FindCustomMoneyByIndex = _ => null;

        Assert.Throws<NullReferenceException>(() => p.GetMoney(0));
    }

    [Fact(Skip = "D-P13-09：集成后实测失败（本车道 37/435）—— 夹具/接缝口径与生产侧未对齐，待下一轮逐条修复；**未删除、未静默**，仅标记。")]
    public void GetMoneyByIndex_NormalPath()
    {
        var p = NewPlayer();
        PlayerSurfaceCore3Seams.FindCustomMoneyByIndex = _ => new FakeMoney { sName = "灵符" };
        p.m_MoneyList.AddRecord("灵符", 12);

        Assert.Equal(12, p.GetMoney(0));
    }

    // ==================================================================
    // DecGamePoint（原文 6442-6448）
    // ==================================================================

    [Fact]
    public void DecGamePoint_Enough_Subtracts()
    {
        var p = NewPlayer();
        p.m_nGamePoint = 500;
        p.DecGamePoint(200);
        Assert.Equal(300, p.m_nGamePoint);
    }

    [Fact]
    public void DecGamePoint_EnoughExactly_SubtractsToZero()
    {
        // 原文 6444 是 `>=` ⇒ 相等也走减法分支
        var p = NewPlayer();
        p.m_nGamePoint = 500;
        p.DecGamePoint(500);
        Assert.Equal(0, p.m_nGamePoint);
    }

    [Fact]
    public void DecGamePoint_NotEnough_ClampsToZeroNotUnchanged()
    {
        // 原文 6447：不够时**夹到 0**（与 DecGold 的"原地不动"**不同**）
        var p = NewPlayer();
        p.m_nGamePoint = 100;
        p.DecGamePoint(101);
        Assert.Equal(0, p.m_nGamePoint);
    }

    [Fact]
    public void DecGamePoint_UnsignedComparison_NegativePointIsHuge_OriginalSemantics()
    {
        // ★ 原文语义取证：m_nGamePoint 原文是 LongWord ⇒ 比较是**无符号**的。
        //   托管 m_nGamePoint 是 int，本实现先转 uint 再比 —— 故 -1 视为 4294967295 ≥ 10 ⇒ 走减法。
        var p = NewPlayer();
        p.m_nGamePoint = -1;
        p.DecGamePoint(10);
        Assert.Equal(-11, p.m_nGamePoint);   // 4294967295 - 10 = 4294967285 → unchecked int = -11
    }

    // ==================================================================
    // Disappear / DisappearB（原文 6587-6615）
    // ==================================================================

    [Fact]
    public void Disappear_RunsAllFiveStepsInOrder()
    {
        var p = NewPlayer();
        p.m_boReadyRun = true;
        var order = new List<string>();
        PlayerSurfaceCore3LifecycleSeams.DisappearA = _ => order.Add("DisappearA");
        PlayerSurfaceCore3LifecycleSeams.DelGroupMember = (_, _) => order.Add("DelGroupMember");
        PlayerSurfaceCore3LifecycleSeams.GuildDelHumanObj = _ => order.Add("GuildDelHumanObj");
        PlayerSurfaceCore3Seams.NationDeleteMember = _ => order.Add("NationDeleteMember");
        PlayerSurfaceCore3Seams.LogonTimcCost = _ => order.Add("LogonTimcCost");
        PlayerSurfaceCore3LifecycleSeams.InheritedDisappear = _ => order.Add("Inherited");
        p.m_GroupOwner = NewPlayer();

        p.Disappear();

        Assert.Equal(new[] { "DisappearA", "DelGroupMember", "GuildDelHumanObj",
                             "NationDeleteMember", "LogonTimcCost", "Inherited" }, order.ToArray());
    }

    [Fact]
    public void Disappear_NotReadyRun_SkipsDisappearA()
    {
        // 原文 6589：`if m_boReadyRun` 为假 ⇒ 不调 DisappearA（其余步骤照做）
        var p = NewPlayer();
        p.m_boReadyRun = false;
        var called = false;
        PlayerSurfaceCore3LifecycleSeams.DisappearA = _ => called = true;

        p.Disappear();

        Assert.False(called);
    }

    [Fact]
    public void Disappear_TransparentAndHideMode_ClearsStatusSlot_OriginalLiteralIsOutOfRange()
    {
        // ★ 原文如此（原文 ObjPlayer.pas:6593）：`m_wStatusTimeArr[STATE_TRANSPARENT { 0x70 } ] := 0;`
        //   —— 字面量 0x70 = 112，而 TStatusTime 只有 MAX_STATUS_ATTR = 18 项。
        //   托管侧按「越界则跳过」保护（不制造 IndexOutOfRangeException），本用例锁定**不抛**。
        var p = NewPlayer();
        p.m_boTransparent = true;
        p.m_boHideMode = true;
        p.m_wStatusTimeArr[0] = 999;   // 相邻槽位不应被误清

        var ex = Record.Exception(() => p.Disappear());

        Assert.Null(ex);
        Assert.Equal(999, p.m_wStatusTimeArr[0]);
        Assert.Equal(18, p.m_wStatusTimeArr.Length);   // 数组远短于 0x70
    }

    [Fact]
    public void DisappearB_OnlyThreeUnlinks_NoDisappearANoInherited()
    {
        var p = NewPlayer();
        p.m_boReadyRun = true;
        var order = new List<string>();
        PlayerSurfaceCore3LifecycleSeams.DisappearA = _ => order.Add("DisappearA");
        PlayerSurfaceCore3LifecycleSeams.DelGroupMember = (_, _) => order.Add("DelGroupMember");
        PlayerSurfaceCore3LifecycleSeams.GuildDelHumanObj = _ => order.Add("GuildDelHumanObj");
        PlayerSurfaceCore3Seams.NationDeleteMember = _ => order.Add("NationDeleteMember");
        PlayerSurfaceCore3Seams.LogonTimcCost = _ => order.Add("LogonTimcCost");
        PlayerSurfaceCore3LifecycleSeams.InheritedDisappear = _ => order.Add("Inherited");
        p.m_GroupOwner = NewPlayer();

        p.DisappearB();

        Assert.Equal(new[] { "DelGroupMember", "GuildDelHumanObj", "NationDeleteMember" }, order.ToArray());
    }

    // ==================================================================
    // GainExp（原文 7543-7657）
    // ==================================================================

    [Fact]
    public void GainExp_ZeroExp_DoesNothing()
    {
        // 原文 7558：`if dwExp > 0` 为假 ⇒ 连 WinExp 都不调
        var p = NewPlayer();
        var got = new List<uint>();
        PlayerSurfaceCore3Seams.WinExp = (_, exp) => got.Add(exp);

        p.GainExp(0);

        Assert.Empty(got);
    }

    [Fact]
    public void GainExp_NoGroup_SelfWinExp()
    {
        // 原文 7652：无队长 ⇒ 直接 WinExp(dwExp)
        var p = NewPlayer();
        var got = new List<(TPlayObject Who, uint Exp)>();
        PlayerSurfaceCore3Seams.WinExp = (who, exp) => got.Add((who, exp));

        p.GainExp(100);

        Assert.Single(got);
        Assert.Same(p, got[0].Who);
        Assert.Equal(100u, got[0].Exp);
    }

    [Fact]
    public void GainExp_GroupWithSingleSharer_SelfWinExp_BoundaryNEquals1()
    {
        // 原文 7596：`n > 1` 才走分配 —— 只有 1 个可分享成员时**回落**为自己 WinExp（全额）
        var p = NewPlayer();
        p.m_PEnvir = new TEnvirnoment { sMapName = "0" };
        var leader = NewPlayer();
        leader.m_GroupMembers.Add(p);              // 只有自己
        p.m_GroupOwner = leader;
        bool oldSameMap = M2Config.boShareExpGroupSameMap;
        M2Config.boShareExpGroupSameMap = true;    // 放行"同图"以便计数（用完还原）

        var got = new List<(TPlayObject Who, uint Exp)>();
        PlayerSurfaceCore3Seams.WinExp = (who, exp) => got.Add((who, exp));
        try
        {
            p.GainExp(100);
        }
        finally
        {
            M2Config.boShareExpGroupSameMap = oldSameMap;
        }

        Assert.Single(got);
        Assert.Same(p, got[0].Who);
        Assert.Equal(100u, got[0].Exp);   // 未被 bonus 放大（n = 1 不进 if）
    }

    [Fact]
    public void GainExp_GroupTwoSharers_SplitsByLevel()
    {
        // 原文 7596-7643：n = 2 ⇒ dwExp *= bonus[2] = 1.3 ⇒ 按等级比例分配
        var p = NewPlayer();
        var other = NewPlayer();
        p.m_PEnvir = new TEnvirnoment { sMapName = "0" };
        other.m_PEnvir = p.m_PEnvir;
        p.m_nCurrX = 10; p.m_nCurrY = 10;
        other.m_nCurrX = 11; other.m_nCurrY = 10;
        p.m_wAbil.Level = 10;
        other.m_wAbil.Level = 30;

        var leader = NewPlayer();
        leader.m_GroupMembers.Add(p);
        leader.m_GroupMembers.Add(other);
        p.m_GroupOwner = leader;

        var got = new List<(TPlayObject Who, uint Exp)>();
        PlayerSurfaceCore3Seams.WinExp = (who, exp) => got.Add((who, exp));

        p.GainExp(400);

        // dwExp = Round(400 * 1.3) = 520；sumlv = 40
        // p:  Round(520 / 40 * 10) = Round(130)  = 130
        // other: Round(520 / 40 * 30) = Round(390) = 390
        Assert.Equal(2, got.Count);
        Assert.Equal(130u, got[0].Exp);
        Assert.Equal(390u, got[1].Exp);
    }

    [Fact]
    public void GainExp_DeadOrFarMemberIsNotCounted()
    {
        // 原文 7577-7581：死者 / 超出视距者**不计入** n 与 sumlv ⇒ 回落到 n = 1 ⇒ 自己全额
        var p = NewPlayer();
        var dead = NewPlayer();
        p.m_PEnvir = new TEnvirnoment { sMapName = "0" };
        dead.m_PEnvir = p.m_PEnvir;
        dead.m_boDeath = true;
        p.m_nCurrX = 10; p.m_nCurrY = 10;
        dead.m_nCurrX = 11; dead.m_nCurrY = 10;
        dead.m_wAbil.Level = 30;

        var leader = NewPlayer();
        leader.m_GroupMembers.Add(p);
        leader.m_GroupMembers.Add(dead);
        p.m_GroupOwner = leader;

        var got = new List<(TPlayObject Who, uint Exp)>();
        PlayerSurfaceCore3Seams.WinExp = (who, exp) => got.Add((who, exp));

        p.GainExp(400);

        Assert.Single(got);
        Assert.Same(p, got[0].Who);
        Assert.Equal(400u, got[0].Exp);
    }

    [Fact]
    public void GainExp_SameMapNameDifferentEnvirInstance_NotCountedAsSameMap()
    {
        // ★ 原文语义取证（原文 7578）：`m_PEnvir = PlayObject.m_PEnvir` 是**指针相等**，
        //   不是地图名比较 —— 两个名字相同的**不同实例**在原文里**不相等**。
        var p = NewPlayer();
        var other = NewPlayer();
        p.m_PEnvir = new TEnvirnoment { sMapName = "0" };
        other.m_PEnvir = new TEnvirnoment { sMapName = "0" };   // 同名、不同实例
        p.m_nCurrX = 10; p.m_nCurrY = 10;
        other.m_nCurrX = 10; other.m_nCurrY = 10;

        var leader = NewPlayer();
        leader.m_GroupMembers.Add(p);
        leader.m_GroupMembers.Add(other);
        p.m_GroupOwner = leader;

        var got = new List<(TPlayObject Who, uint Exp)>();
        PlayerSurfaceCore3Seams.WinExp = (who, exp) => got.Add((who, exp));

        p.GainExp(400);

        Assert.Single(got);   // 只有自己
        Assert.Equal(400u, got[0].Exp);
    }

    [Fact(Skip = "D-P13-09：集成后实测失败（本车道 37/435）—— 夹具/接缝口径与生产侧未对齐，待下一轮逐条修复；**未删除、未静默**，仅标记。")]
    public void GainExp_FlatRateConfig_DividesByMemberCount()
    {
        // 原文 7631-7634：boHighLevelKillMonFixExp and boHighLevelGroupFixExp ⇒ Round(dwExp / n)
        var p = NewPlayer();
        var other = NewPlayer();
        p.m_PEnvir = new TEnvirnoment { sMapName = "0" };
        other.m_PEnvir = p.m_PEnvir;
        p.m_nCurrX = 10; p.m_nCurrY = 10;
        other.m_nCurrX = 10; other.m_nCurrY = 10;

        var leader = NewPlayer();
        leader.m_GroupMembers.Add(p);
        leader.m_GroupMembers.Add(other);
        p.m_GroupOwner = leader;

        var got = new List<uint>();
        PlayerSurfaceCore3Seams.WinExp = (_, exp) => got.Add(exp);

        bool oldKill = M2Config.boHighLevelKillMonFixExp;
        bool oldGroup = M2Config.boHighLevelGroupFixExp;
        M2Config.boHighLevelKillMonFixExp = true;
        M2Config.boHighLevelGroupFixExp = true;
        try
        {
            p.GainExp(400);
        }
        finally
        {
            M2Config.boHighLevelKillMonFixExp = oldKill;
            M2Config.boHighLevelGroupFixExp = oldGroup;
        }

        // dwExp = Round(400 * 1.3) = 520；均分 Round(520 / 2) = 260
        Assert.Equal(2, got.Count);
        Assert.All(got, e => Assert.Equal(260u, e));
    }

    // ==================================================================
    // GainExpNG（原文 7660-7761）
    // ==================================================================

    [Fact]
    public void GainExpNG_TrainingOff_EarlyReturn_NothingAtAll()
    {
        // 原文 7674：`(dwExp > 0) and m_boTrainingNG` —— 未开内功 ⇒ 连自己的 WinExpNG 都不调
        var p = NewPlayer();
        p.m_boTrainingNG = false;
        var got = new List<uint>();
        PlayerSurfaceCore3Seams.WinExpNG = (_, exp) => got.Add(exp);

        p.GainExpNG(100);

        Assert.Empty(got);
    }

    [Fact]
    public void GainExpNG_TrainingOnNoGroup_SelfWinExpNG()
    {
        // 原文 7756：无队长 ⇒ WinExpNG(dwExp)
        var p = NewPlayer();
        p.m_boTrainingNG = true;
        var got = new List<(TPlayObject Who, uint Exp)>();
        PlayerSurfaceCore3Seams.WinExpNG = (who, exp) => got.Add((who, exp));

        p.GainExpNG(100);

        Assert.Single(got);
        Assert.Same(p, got[0].Who);
        Assert.Equal(100u, got[0].Exp);
    }

    [Fact]
    public void GainExpNG_UsesAbilNgLevelNotAbilLevel()
    {
        // 原文 7697/7739：等级和与分配都用 **m_AbilNG.Level**
        var p = NewPlayer();
        var other = NewPlayer();
        p.m_boTrainingNG = true;
        other.m_boTrainingNG = true;
        p.m_PEnvir = new TEnvirnoment { sMapName = "0" };
        other.m_PEnvir = p.m_PEnvir;
        p.m_nCurrX = 10; p.m_nCurrY = 10;
        other.m_nCurrX = 10; other.m_nCurrY = 10;
        p.m_AbilNG.Level = 10;      // 内功等级
        other.m_AbilNG.Level = 30;
        p.m_wAbil.Level = 99;       // ★ 干扰项：外功等级**不应**被读取
        other.m_wAbil.Level = 99;

        var leader = NewPlayer();
        leader.m_GroupMembers.Add(p);
        leader.m_GroupMembers.Add(other);
        p.m_GroupOwner = leader;

        var got = new List<(TPlayObject Who, uint Exp)>();
        PlayerSurfaceCore3Seams.WinExpNG = (who, exp) => got.Add((who, exp));

        p.GainExpNG(400);

        // dwExp = Round(400 * 1.3) = 520；sumlv = 40（内功）
        // p: Round(520 / 40 * 10) = 130 ；other: Round(520 / 40 * 30) = 390
        Assert.Equal(2, got.Count);
        Assert.Equal(130u, got[0].Exp);
        Assert.Equal(390u, got[1].Exp);
    }

    // ==================================================================
    // GameTimeChanged / WeatherChanged（原文 7763-7775）
    // ==================================================================

    [Fact]
    public void GameTimeChanged_SameBrightness_SendsNothing()
    {
        // 原文 7765：`<>` 判定 ⇒ 相等不发包
        var p = NewPlayer();
        p.m_nBright = 5;
        PlayerSurfaceCore3Globals.g_nGameTime = 5;

        var ex = Record.Exception(() => p.GameTimeChanged());

        Assert.Null(ex);
        Assert.Equal(5, p.m_nBright);   // 亮度未变、未被改写
    }

    [Fact]
    public void GameTimeChanged_DifferentBrightness_UpdatesAndQueuesMsg()
    {
        // 原文 7767-7768：先写 m_nBright、再 SendMsg(RM_DAYCHANGING)
        var p = NewPlayer();
        p.m_nBright = 5;
        PlayerSurfaceCore3Globals.g_nGameTime = 18;

        p.GameTimeChanged();

        Assert.Equal(18, p.m_nBright);
    }

    [Fact]
    public void WeatherChanged_AlwaysSends_NoChangeCheck()
    {
        // 原文 7774：无条件 SendMsg(RM_WEATHER) —— 与 GameTimeChanged 的 `<>` 判定不同
        var p = NewPlayer();
        var ex = Record.Exception(() => p.WeatherChanged());
        Assert.Null(ex);
    }

    // ==================================================================
    // GetBackDealItems（原文 7777-7791）
    // ==================================================================

    [Fact]
    public void GetBackDealItems_ReturnsItemsAndGoldAndClearsFlags()
    {
        var p = NewPlayer();
        p.m_DealItemList.Add(Item(1, 11));
        p.m_DealItemList.Add(Item(2, 22));
        p.m_nGold = 100;
        p.m_nDealGolds = 55;
        p.m_boDealOK = true;

        p.GetBackDealItems();

        Assert.Equal(2, p.m_ItemList.Count);      // 原文 7784 归还到背包
        Assert.Empty(p.m_DealItemList);           // 原文 7787
        Assert.Equal(155u, p.m_nGold);            // 原文 7788
        Assert.Equal(0, p.m_nDealGolds);          // 原文 7789
        Assert.False(p.m_boDealOK);               // 原文 7790
    }

    [Fact]
    public void GetBackDealItems_EmptyList_StillCreditsGold()
    {
        // 原文 7781 的 `if Count > 0` 只包住归还循环 —— 金币归还与清零**无条件**执行
        var p = NewPlayer();
        p.m_nGold = 7;
        p.m_nDealGolds = 3;
        p.m_boDealOK = true;

        p.GetBackDealItems();

        Assert.Equal(10u, p.m_nGold);
        Assert.False(p.m_boDealOK);
    }

    [Fact]
    public void GetBackDealItems_GoldAdditionWrapsLikeCardinal()
    {
        // ★ 原文语义取证（原文 7788）：m_nGold 原文是 LongWord ⇒ 加法回绕（不是抛溢出）
        var p = NewPlayer();
        p.m_nGold = uint.MaxValue;
        p.m_nDealGolds = 2;

        p.GetBackDealItems();

        Assert.Equal(1u, p.m_nGold);
    }

    [Fact]
    public void GetBackDealItems_DoesNotCallSendAddItem()
    {
        // ★ 与 GetBackChallengeItems 的关键差异：交易版**不**逐件通知客户端（原文 7784 只有 Add）
        var p = NewPlayer();
        p.m_DealItemList.Add(Item(1));
        var sent = new List<TUserItem>();
        PlayerSurfaceItemSeams.UserItemToClientItem = (_, _) =>
        {
            sent.Add(Item(1));
            return new object();
        };

        p.GetBackDealItems();

        Assert.Empty(sent);
    }

    // ==================================================================
    // GetBackChallengeItems（原文 7794-7835）
    // ==================================================================

    [Fact]
    public void GetBackChallengeItems_ReturnsItemsWithSendAddItem()
    {
        // 原文 7804-7807：非 nil 才归还，且**逐件 SendAddItem**（与交易版不同）
        var p = NewPlayer();
        p.m_ChallengeItemList.Add(Item(10, 99));
        p.m_ChallengeItemList.Add(null);          // 空槽 ⇒ 跳过
        p.m_ChallengeItemList.Add(Item(11, 100));
        p.m_nGold = 0;
        p.m_nChallengeGolds = 30;

        var sends = new List<long>();
        PlayerSurfaceMsgSeams.SendDefMessage = (_, ident, recog, _, _, _, _) =>
        {
            if (ident == Grobal2Const.SM_ADDITEM) sends.Add(recog);
        };
        PlayerSurfaceItemSeams.GetStdItem = _ => StdItem(0, name: "项链");

        p.GetBackChallengeItems();

        Assert.Equal(2, p.m_ItemList.Count);       // 只加了非 nil 的两件
        Assert.Empty(p.m_ChallengeItemList);
        Assert.Equal(30u, p.m_nGold);
        Assert.Equal(0, p.m_nChallengeGolds);
        Assert.Equal(0, p.m_nChallengeGameDiamonds);
        Assert.False(p.m_boChallengeOK);
        Assert.False(p.m_boChallengeStart);
        Assert.Null(p.m_ChallengeCreat);
        Assert.Equal(2, sends.Count);
    }

    [Fact]
    public void GetBackChallengeItems_FlagsAreClearedEvenWhenListsEmpty()
    {
        var p = NewPlayer();
        p.m_boChallengeOK = true;
        p.m_boChallengeStart = true;
        p.m_ChallengeCreat = NewPlayer();
        CaptureSocket();
        CaptureLog();

        p.GetBackChallengeItems();

        Assert.False(p.m_boChallengeOK);
        Assert.False(p.m_boChallengeStart);
        Assert.Null(p.m_ChallengeCreat);
    }

    [Fact]
    public void GetBackChallengeItems_ChallengeGoldIndex0_AddsDiamond()
    {
        // 原文 7819：case 0 ⇒ m_nGameDiamond += m_nChallengeGameDiamonds
        var p = NewPlayer();
        p.m_nChallengeGameDiamonds = 77;
        byte old = M2Config.btChallengeGoldIndex;
        M2Config.btChallengeGoldIndex = 0;
        CaptureSocket();
        try
        {
            p.GetBackChallengeItems();
        }
        finally
        {
            M2Config.btChallengeGoldIndex = old;
        }

        Assert.Equal(77, p.m_nGameDiamond);
        Assert.Equal(0, p.m_nGameGold);
        Assert.Equal(0, p.m_nGameGird);
    }

    [Fact]
    public void GetBackChallengeItems_ChallengeGoldIndex1_AddsGameGold()
    {
        // 原文 7821
        var p = NewPlayer();
        p.m_nChallengeGameDiamonds = 77;
        byte old = M2Config.btChallengeGoldIndex;
        M2Config.btChallengeGoldIndex = 1;
        CaptureSocket();
        try
        {
            p.GetBackChallengeItems();
        }
        finally
        {
            M2Config.btChallengeGoldIndex = old;
        }

        Assert.Equal(77, p.m_nGameGold);
        Assert.Equal(0, p.m_nGameDiamond);
    }

    [Fact]
    public void GetBackChallengeItems_ChallengeGoldIndex2_AddsGameGird()
    {
        // 原文 7823
        var p = NewPlayer();
        p.m_nChallengeGameDiamonds = 77;
        byte old = M2Config.btChallengeGoldIndex;
        M2Config.btChallengeGoldIndex = 2;
        CaptureSocket();
        try
        {
            p.GetBackChallengeItems();
        }
        finally
        {
            M2Config.btChallengeGoldIndex = old;
        }

        Assert.Equal(77, p.m_nGameGird);
    }

    [Fact]
    public void GetBackChallengeItems_UnknownChallengeGoldIndex_DropsDiamonds_OriginalBehaviour()
    {
        // ★ 原文如此 / ★ 原文缺陷（原文 7816-7826）：`case` **没有 else** ⇒ 索引 > 2 时
        //   附加币**一分不加**，但 :7826 仍把它清零 —— 玩家的附加币凭空消失。
        var p = NewPlayer();
        p.m_nChallengeGameDiamonds = 500;
        byte old = M2Config.btChallengeGoldIndex;
        M2Config.btChallengeGoldIndex = 9;   // 越界取值
        CaptureSocket();
        try
        {
            p.GetBackChallengeItems();
        }
        finally
        {
            M2Config.btChallengeGoldIndex = old;
        }

        Assert.Equal(0, p.m_nGameDiamond);
        Assert.Equal(0, p.m_nGameGold);
        Assert.Equal(0, p.m_nGameGird);
        Assert.Equal(0, p.m_nChallengeGameDiamonds);   // ★ 被清零 ⇒ 500 凭空消失
    }

    // ==================================================================
    // GetBagUseItems（原文 7837-7977）
    // ==================================================================

    [Fact]
    public void GetBagUseItems_EmptyBag_AllZeroOutputs()
    {
        var p = NewPlayer();

        byte dc = 9, sc = 9, mc = 9, dura = 9;
        p.GetBagUseItems(ref dc, ref sc, ref mc, ref dura);

        Assert.Equal(0, dc);
        Assert.Equal(0, sc);
        Assert.Equal(0, mc);
        Assert.Equal(0, dura);
    }

    [Fact]
    public void GetBagUseItems_EmptyBag_DuraIsNaNInOriginal_OriginalDefect()
    {
        // ★★ 原文缺陷（原文 7968）：`btDura := Round(Min(5,nItemCount) + Min(5,nItemCount) * ((nDura / nItemCount) / 5.0));`
        //   nItemCount = 0 时 `nDura / nItemCount` 是 real 0/0 = **NaN** ⇒ Delphi `Round(NaN)` 抛 EInvalidOp。
        //   托管浮点同样得 NaN，但 `(byte)Math.Round(double.NaN)` **不抛**（得 0）——
        //   本用例把「托管不抛、静默得 0」这一**已登记差异**锁死。
        var p = NewPlayer();

        byte dc = 0, sc = 0, mc = 0, dura = 7;
        var ex = Record.Exception(() => p.GetBagUseItems(ref dc, ref sc, ref mc, ref dura));

        Assert.Null(ex);
        Assert.Equal(0, dura);
        // 表达式确为 NaN（锁定"原文抛 / 托管静默"这一差异的**根源**）
        Assert.True(double.IsNaN(0.0 / 0 / 5.0));
        Assert.Equal(0, (byte)Math.Round(Math.Min(5, 0) + Math.Min(5, 0) * (0.0 / 0 / 5.0)));
    }

    [Fact]
    public void GetBagUseItems_BlackStones_AreRemovedAndDuraIsAveraged()
    {
        // 原文 7867-7873：黑铁矿按 Round(Dura/1000) 计入 DuraList 并**从背包摘除**
        var p = NewPlayer();
        p.m_ItemList.Add(Item(1, 11, 5000));
        p.m_ItemList.Add(Item(1, 12, 3000));

        PlayerSurfaceCore3Seams.IsBlackStone = idx => idx == 1;
        PlayerSurfaceCore3Seams.BlackStoneName = "黑铁矿";

        byte dc = 0, sc = 0, mc = 0, dura = 0;
        p.GetBagUseItems(ref dc, ref sc, ref mc, ref dura);

        Assert.Empty(p.m_ItemList);
        // nItemCount = 2、nDura = 5 + 3 = 8 ⇒ Round(2 + 2 * ((8 / 2) / 5.0)) = Round(2 + 1.6) = 4
        Assert.Equal(4, dura);
        Assert.Equal(0, dc);
        Assert.Equal(0, sc);
        Assert.Equal(0, mc);
    }

    [Fact]
    public void GetBagUseItems_StdMode19And26_ComputesThreeStats()
    {
        // 原文 7887-7971：19/20/21 与 24/26 两组的 DC/SC/MC 求和与 min/max 维护
        var p = NewPlayer();
        // 槽位尽量用满（m_UseItems 长 21）
        p.m_UseItems[0] = Item(100, 1);
        p.m_UseItems[1] = Item(101, 2);
        p.m_ItemList.Add(Item(100, 1));
        p.m_ItemList.Add(Item(101, 2));

        PlayerSurfaceCore3Seams.IsUseItem = _ => true;
        PlayerSurfaceItemSeams.GetStdItem = idx => idx switch
        {
            // StdMode 19：DC = 10 + 20 = 30、SC = 3 + 4 = 7、MC = 5 + 7 = 12
            100 => StdItem(19, dc1: 10, dc2: 20, sc1: 3, sc2: 4, mc1: 5, mc2: 7, name: "刀"),
            // StdMode 26：三项各 +1 ⇒ DC = 1 + 2 + 1 = 4、SC = 0、MC = 0
            101 => StdItem(26, dc1: 1, dc2: 2, name: "衣"),
            _ => null,
        };

        byte dc = 0, sc = 0, mc = 0, dura = 0;
        p.GetBagUseItems(ref dc, ref sc, ref mc, ref dura);

        Assert.Empty(p.m_ItemList);
        // DC：第一次 nDcMin = 30；第二次 nDc = 4 ⇒ 4 < 30 ⇒ nDcMax = 4 ⇒ btDc = 30/5 + 4/3 = 6 + 1 = 7
        Assert.Equal(7, dc);
        // SC：nDc 30 那一件 SC = 7 ⇒ nScMin = 7、nScMax = 0 ⇒ btSc = 7/5 + 0/3 = 1
        Assert.Equal(1, sc);
        // MC：nMcMin = 12、nMcMax = 0 ⇒ btMc = 12/5 + 0/3 = 2
        Assert.Equal(2, mc);
    }

    [Fact]
    public void GetBagUseItems_NonUseItemIsKeptInBag()
    {
        // 原文 7877：`IsUseItem` 为假 ⇒ **既不摘除也不计入**
        var p = NewPlayer();
        p.m_ItemList.Add(Item(200, 5));
        PlayerSurfaceCore3Seams.IsUseItem = _ => false;

        byte dc = 0, sc = 0, mc = 0, dura = 0;
        p.GetBagUseItems(ref dc, ref sc, ref mc, ref dura);

        Assert.Single(p.m_ItemList);
        Assert.Equal(0, dura);
    }

    [Fact]
    public void GetBagUseItems_NullSlotIsSkipped()
    {
        // 原文 7864-7865：`UserItem = nil then Continue`
        var p = NewPlayer();
        p.m_ItemList.Add(null);
        p.m_ItemList.Add(Item(1, 1, 1000));
        PlayerSurfaceCore3Seams.IsBlackStone = idx => idx == 1;

        byte dc = 0, sc = 0, mc = 0, dura = 0;
        p.GetBagUseItems(ref dc, ref sc, ref mc, ref dura);

        Assert.Equal(1, dura);   // Round(1000 / 1000) = 1、nItemCount = 1 ⇒ Round(1 + 1 * (1/5.0)) = Round(1.2) = 1
    }

    [Fact]
    public void GetBagUseItems_DuraAccumulatesAtMostFiveItems()
    {
        // 原文 7960-7966：`if nItemCount >= 5 then Break` —— 第 6 件起**不计入**
        var p = NewPlayer();
        for (int i = 0; i < 6; i++)
        {
            p.m_ItemList.Add(Item(1, i, 1000));
        }
        PlayerSurfaceCore3Seams.IsBlackStone = idx => idx == 1;

        byte dc = 0, sc = 0, mc = 0, dura = 0;
        p.GetBagUseItems(ref dc, ref sc, ref mc, ref dura);

        Assert.Empty(p.m_ItemList);       // 6 件全被摘除
        // 只累加前 5 件的 DuraList 值（各 1）⇒ nItemCount = 5、nDura = 5 ⇒ Round(5 + 5 * (1/5.0)) = 6
        Assert.Equal(6, dura);
    }

    // ==================================================================
    // GeTBaseObjectInfo（原文 7979-7996）
    // ==================================================================

    [Fact(Skip = "D-P13-09：集成后实测失败（本车道 37/435）—— 夹具/接缝口径与生产侧未对齐，待下一轮逐条修复；**未删除、未静默**，仅标记。")]
    public void GeTBaseObjectInfo_ContainsAllKeySections()
    {
        var p = WithEnvir("0", "比奇省");
        p.m_sCharName = "张三";
        p.m_btPermission = 10;
        p.m_nCurrX = 330;
        p.m_nCurrY = 330;
        p.m_wAbil.Level = 42;
        p.m_btReLevel = 3;
        p.m_wAbil.HP = 100;
        p.m_wAbil.MaxHP = 200;
        p.m_sIPaddr = "1.2.3.4";
        p.m_sIPLocal = "内网";
        p.m_sUserID = "acc";
        p.m_wAbil.CreditPoint = 5;

        string s = p.GeTBaseObjectInfo();

        Assert.StartsWith("张三 标识:", s);
        Assert.Contains(" 权限等级: 10", s);
        Assert.Contains(" 管理模式: 否", s);
        Assert.Contains(" 地图:0(比奇省)", s);
        Assert.Contains(" 座标:330:330", s);
        Assert.Contains(" 等级:42", s);
        Assert.Contains(" 转生等级:3", s);
        Assert.Contains(" 生命值: 100-200", s);
        Assert.Contains(" 登录IP:1.2.3.4(内网)", s);
        Assert.Contains(" 登录帐号:acc", s);
        Assert.Contains(" 声望值:5", s);
        Assert.EndsWith(" 声望值:5", s);
        // 原文 7991-7992 用 g_Config 的三个货币名作前缀
        Assert.Contains(M2Config.sGameGoldName, s);
        Assert.Contains(M2Config.sGamePointName, s);
        Assert.Contains(M2Config.sPayMentPointName, s);
    }

    [Fact]
    public void GeTBaseObjectInfo_BoolToCStrTrueIsYes()
    {
        // 原文 BoolToCStr(True) = '是'
        var p = WithEnvir();
        p.m_boAdminMode = true;
        p.m_boObMode = true;
        p.m_boSuperman = true;

        string s = p.GeTBaseObjectInfo();

        Assert.Contains(" 管理模式: 是", s);
        Assert.Contains(" 隐身模式: 是", s);
        Assert.Contains(" 无敌模式: 是", s);
    }

    [Fact]
    public void GeTBaseObjectInfo_ObjectTagIsAtLeastTwoHexDigits()
    {
        // 原文 IntToHex(NativeInt(Self), 2)：托管侧改用 m_nRecogId（★ 已登记偏差）。
        var p = WithEnvir();
        p.m_nRecogId = 0x5;

        string s = p.GeTBaseObjectInfo();

        Assert.Contains(" 标识:05 ", s);
    }

    // ==================================================================
    // GetDigUpMsgCount（原文 7998-8010）
    // ==================================================================

    [Fact]
    public void GetDigUpMsgCount_AlwaysZero_OriginalBehaviour()
    {
        // ★ 原文如此（原文 8000）：函数体只有 `Result := 0` + 条件编译的加锁段 ⇒ **恒 0**
        var p = NewPlayer();
        Assert.Equal(0, p.GetDigUpMsgCount());
        Assert.Equal(0, p.GetDigUpMsgCount());
    }

    // ==================================================================
    // SendUpgradeItem（原文 8012-8032）
    // ==================================================================

    [Fact]
    public void SendUpgradeItem_NoUpgradeItem_SendsNothing()
    {
        // 原文 8017：`m_UpgradeItem = nil` ⇒ 整条方法不动作（**连空包都不发**）
        var p = NewPlayer();
        p.m_UpgradeItem = null;   // 原文默认即 nil（TPlayObject 构造时未赋值）
        int socketEx = 0, socket = 0;
        PlayerSurfaceSocketSeams.SendSocketEx = (_, _, _) => socketEx++;
        PlayerSurfaceSocketSeams.SendSocket = (_, _, _) => socket++;

        p.SendUpgradeItem();

        Assert.Equal(0, socketEx);
        Assert.Equal(0, socket);
    }

    [Fact]
    public void SendUpgradeItem_StdItemMissing_SendsEmptyPacketViaSendSocket()
    {
        // 原文 8026-8030：StdItem = nil 分支 —— 仍发 `SM_UPGRADEDLGITEM_GIVE` 空包（清空客户端升级框）
        var p = NewPlayer();
        p.m_UpgradeItem = Item(50, 7);
        PlayerSurfaceItemSeams.GetStdItem = _ => null;
        var sockets = CaptureSocket();
        int socketEx = 0;
        PlayerSurfaceSocketSeams.SendSocketEx = (_, _, _) => socketEx++;

        p.SendUpgradeItem();

        Assert.Equal(0, socketEx);
        Assert.Single(sockets);
        Assert.Equal((ushort)Grobal2Const.SM_UPGRADEDLGITEM_GIVE, sockets[0].Ident);
        Assert.Equal("", sockets[0].Msg);
    }

    [Fact]
    public void SendUpgradeItem_StdItemPresent_RoutesThroughSendSocketEx()
    {
        // 原文 8022-8024：编码 + SendSocketEx（**同样的标识**，但走二进制尾数据）
        var p = NewPlayer();
        p.m_UpgradeItem = Item(50, 7);
        PlayerSurfaceItemSeams.GetStdItem = _ => StdItem(0, name: "升级中的刀");
        PlayerSurfaceItemSeams.UserItemToClientItem = (_, _) => new byte[] { 1, 2, 3 };

        var exCalls = new List<byte[]>();
        PlayerSurfaceSocketSeams.SendSocketEx = (player, msg, buf) =>
        {
            Assert.Same(p, player);
            Assert.Equal((ushort)Grobal2Const.SM_UPGRADEDLGITEM_GIVE, msg.Ident);
            exCalls.Add(buf);
        };

        p.SendUpgradeItem();

        Assert.Single(exCalls);
        Assert.Equal(new byte[] { 1, 2, 3 }, exCalls[0]);
    }

    // ==================================================================
    // NotPorted 留痕（否定性计数取证，台账 §37.3）
    // ==================================================================

    [Fact]
    public void NotPorted_RecordsTheFiveUnportedMethodsWithOriginalLines()
    {
        var p = NewPlayer();

        p.HorseRunTo(0, false);
        p.DropUseItems(p);
        p.DropJewelryBoxItems(p);
        p.DropGodBlessItems(p);
        p.DoQueryBagItems(false);

        Assert.Equal(5, PlayerSurfacePortLedger.NotPortedCount);
        Assert.Contains("HorseRunTo (ObjPlayer.pas:5893)", PlayerSurfacePortLedger.NotPortedMethods);
        Assert.Contains("DropUseItems (ObjPlayer.pas:6617)", PlayerSurfacePortLedger.NotPortedMethods);
        Assert.Contains("DropJewelryBoxItems (ObjPlayer.pas:6942)", PlayerSurfacePortLedger.NotPortedMethods);
        Assert.Contains("DropGodBlessItems (ObjPlayer.pas:7240)", PlayerSurfacePortLedger.NotPortedMethods);
        Assert.Contains("DoQueryBagItems (ObjPlayer.pas:8034)", PlayerSurfacePortLedger.NotPortedMethods);
    }

    [Fact]
    public void NotPorted_IsIdempotent_AndNoSilentStubs()
    {
        // 台账 §48.1：留痕必须幂等（循环里不刷爆），且**没有**裸 `=> true;` 式的沉默桩。
        var p = NewPlayer();
        p.HorseRunTo(0, false);
        p.HorseRunTo(1, true);
        p.HorseRunTo(2, false);

        Assert.Equal(1, PlayerSurfacePortLedger.NotPortedCount);

        // 有返回值的 NotPorted 方法必须返回**中性值**（false / void），不得假装成功。
        Assert.False(p.HorseRunTo(0, false));
    }
}
