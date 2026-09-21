// ============================================================================
// 测试：本车道 p13-m2-objplayer **切片 Core4（玩家会话配置下发与状态清理片）**。
// 被测托管源：GXX.M2Server/Engine/PlayerSurface/TPlayObject.PlayerSurface.Core4.cs
// 原文出处：Source/M2Engine/ObjPlayer.pas:8094-10476（42 条方法）
//   · 8094-8101  ClearStatusTime          （真实体）
//   · 8101-8114  ClearTimeLabel           （真实体）
//   · 8114-8128  ClearAllDelayLabel       （真实体 / ★ 原文缺陷 1）
//   · 9214-9228  SendGoldInfo             （真实体）
//   · 9229-9240  SendNewGamePointInfo     （真实体 / ★ 原文缺陷 1）
//   · 9241-9245  SendGameGlory            （真实体）
//   · 9914-9938  SendClientBlackModules   （真实体 / ★ 原文缺陷 1）
//   · 9975-9991  SendArrButtonConfig      （真实体 / ★ 原文缺陷 1）
//   另有 34 条 PortNotPorted —— 末两条用例做**否定性计数取证**（台账 §37.3）。
// 辅助源：Grobal2.pas:36（MAX_STATUS_ATTR=18）/ :4118（TStatusTime）；
//         M2Share.pas:210（sSTRING_GOLDNAME='金币'）。
// 用例分布：每族「正常 / 边界 / 早退顺序」，每条原文缺陷各一条专属断言。
// ============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using GXX.M2Server.Npc;
using Xunit;

namespace GXX.M2Server.Tests;

[Collection(PlayerSurfacePortLedgerSerialCollection.Name)]
public class ObjPlayerCore4Tests : IDisposable
{
    // SendArrButtonConfig 的字节布局断言会写 M2Config 全局，构造/析构时保存并还原。
    private readonly int _savedOffsetX = M2Config.g_ArrButtonConfig[0].OffsetX;

    public ObjPlayerCore4Tests()
    {
        PlayerSurfaceSocketSeams.ResetDefaults();
        PlayerSurfaceMsgSeams.ResetDefaults();
        PlayerSurfaceBaseSeams.ResetDefaults();
        PlayerSurfacePortLedger.ResetNotPorted();
        ClientModuleState.g_BlackModuleList.Clear();
        M2Config.g_ArrButtonConfigCRC = 0;
    }

    public void Dispose()
    {
        PlayerSurfaceSocketSeams.ResetDefaults();
        PlayerSurfaceMsgSeams.ResetDefaults();
        PlayerSurfaceBaseSeams.ResetDefaults();
        PlayerSurfacePortLedger.ResetNotPorted();
        ClientModuleState.g_BlackModuleList.Clear();
        M2Config.g_ArrButtonConfigCRC = 0;
        M2Config.g_ArrButtonConfig[0].OffsetX = _savedOffsetX;
    }

    // ------------------------------------------------------------------
    // 采集器
    // ------------------------------------------------------------------

    private sealed record DefCall(ushort Ident, long Recog, ushort Param, ushort Tag, ushort Series, string Msg);
    private sealed record SockCall(TDefaultMessage Msg, string Payload);
    private sealed record SockExCall(TDefaultMessage Msg, byte[] Buf);

    private static List<DefCall> CaptureDef()
    {
        var list = new List<DefCall>();
        PlayerSurfaceMsgSeams.SendDefMessage =
            (_, ident, recog, p, t, s, msg) => list.Add(new DefCall(ident, recog, p, t, s, msg));
        return list;
    }

    private static List<SockCall> CaptureSocket()
    {
        var list = new List<SockCall>();
        PlayerSurfaceSocketSeams.SendSocket = (_, m, s) => list.Add(new SockCall(m, s));
        return list;
    }

    private static List<SockExCall> CaptureSocketEx()
    {
        var list = new List<SockExCall>();
        PlayerSurfaceSocketSeams.SendSocketEx = (_, m, b) => list.Add(new SockExCall(m, b));
        return list;
    }

    /// <summary>原文 AnsiString 的 1:1 字节映射（Latin-1），与生产代码同一口径。</summary>
    private static string Latin1(byte[] b) => Encoding.Latin1.GetString(b);

    // ==================================================================
    // ClearStatusTime（原文 8094-8101）
    // ==================================================================

    /// <summary>原文 8095-8098：三个数组整块清零，`m_nCharStatus` 取自 `GetCharStatus`。</summary>
    [Fact]
    public void ClearStatusTime_ZeroesAllThreeArrays_AndTakesCharStatusFromSeam()
    {
        PlayerSurfaceBaseSeams.GetCharStatus = _ => 12345;

        var p = new TPlayObject();
        for (int i = 0; i < p.m_wStatusTimeArr.Length; i++)
        {
            p.m_wStatusTimeArr[i] = (ushort)(i + 1);
            p.m_nStatusPowerTime[i] = (ushort)(i + 100);
            p.m_nStatusPower[i] = i - 9;           // 含负值（原文 m_nStatusPower 是**有符号** Integer 数组）
        }

        p.ClearStatusTime();

        Assert.All(p.m_wStatusTimeArr, v => Assert.Equal((ushort)0, v));
        Assert.All(p.m_nStatusPowerTime, v => Assert.Equal((ushort)0, v));
        Assert.All(p.m_nStatusPower, v => Assert.Equal(0, v));
        Assert.Equal(12345, p.m_nCharStatus);
    }

    /// <summary>边界：接缝未接线时 `GetCharStatus` 返回 0（默认实现）。</summary>
    [Fact]
    public void ClearStatusTime_UnwiredSeam_CharStatusBecomesZero()
    {
        var p = new TPlayObject();
        p.m_nCharStatus = 999;

        p.ClearStatusTime();

        Assert.Equal(0, p.m_nCharStatus);
    }

    /// <summary>
    /// 原文 8095-8097 的长度口径：三个数组都必须是 `MAX_STATUS_ATTR`（Grobal2.pas:36 = 18）。
    /// 原文第 1、2 行用 `SizeOf(TStatusTime)`（= 18×2 字节），第 3 行用 `SizeOf(m_nStatusPower)`
    /// （= 18×4 字节）—— 两者都恰好覆盖各自数组，长度必须一致。
    /// </summary>
    [Fact]
    public void ClearStatusTime_ArrayLengths_MatchMaxStatusAttr()
    {
        var p = new TPlayObject();

        Assert.Equal(18, PlayerSurfaceCore4Const.MAX_STATUS_ATTR);
        Assert.Equal(PlayerSurfaceCore4Const.MAX_STATUS_ATTR, p.m_wStatusTimeArr.Length);
        Assert.Equal(PlayerSurfaceCore4Const.MAX_STATUS_ATTR, p.m_nStatusPowerTime.Length);
        Assert.Equal(PlayerSurfaceCore4Const.MAX_STATUS_ATTR, p.m_nStatusPower.Length);
    }

    // ==================================================================
    // ClearTimeLabel（原文 8101-8114）
    // ==================================================================

    /// <summary>原文 8106-8110：倒序遍历，`nType` 相同者置 `boDelete`，不同者**原样不动**。</summary>
    [Fact]
    public void ClearTimeLabel_MarksMatchingType_AndLeavesOthersUntouched()
    {
        var p = new TPlayObject();
        p.m_TimeLabelList.Add(new TTimeLabel { nType = 1 });
        p.m_TimeLabelList.Add(new TTimeLabel { nType = 2 });
        p.m_TimeLabelList.Add(new TTimeLabel { nType = 1 });
        p.m_TimeLabelList.Add(new TTimeLabel { nType = 3 });

        p.ClearTimeLabel(1);

        Assert.True(p.m_TimeLabelList[0].boDelete);
        Assert.False(p.m_TimeLabelList[1].boDelete);
        Assert.True(p.m_TimeLabelList[2].boDelete);
        Assert.False(p.m_TimeLabelList[3].boDelete);
    }

    /// <summary>边界：空列表 —— `for I := -1 downto 0` 零次执行，不抛异常。</summary>
    [Fact]
    public void ClearTimeLabel_EmptyList_DoesNotThrow()
    {
        var p = new TPlayObject();

        p.ClearTimeLabel(7);

        Assert.Empty(p.m_TimeLabelList);
    }

    /// <summary>
    /// ★ 语义锁定（原文 8109-8110 只写 `boDelete := True`）：本方法**不删除、不摘链**，
    /// 列表长度不变 —— 真正摘除发生在心跳（原文 :4155/:4167/:4194/:4244）。
    /// 若有人把这里"顺手改成 RemoveAll"，本用例立刻失败。
    /// </summary>
    [Fact]
    public void ClearTimeLabel_OnlyMarksDelete_DoesNotRemoveFromList()
    {
        var p = new TPlayObject();
        p.m_TimeLabelList.Add(new TTimeLabel { nType = 5 });
        p.m_TimeLabelList.Add(new TTimeLabel { nType = 5 });

        p.ClearTimeLabel(5);

        Assert.Equal(2, p.m_TimeLabelList.Count);
    }

    // ==================================================================
    // ClearAllDelayLabel（原文 8114-8128）
    // ==================================================================

    /// <summary>原文 8119-8124：遍历（逐条 Dispose）后整表 `Clear` —— 最终列表为空。</summary>
    [Fact]
    public void ClearAllDelayLabel_EmptiesTheList()
    {
        var p = new TPlayObject();
        p.m_TimeLabelList.Add(new TTimeLabel { nType = 1 });
        p.m_TimeLabelList.Add(new TTimeLabel { nType = 2 });
        p.m_TimeLabelList.Add(new TTimeLabel { nType = 3 });

        p.ClearAllDelayLabel();

        Assert.Empty(p.m_TimeLabelList);
    }

    /// <summary>边界：空列表不抛异常。</summary>
    [Fact]
    public void ClearAllDelayLabel_EmptyList_DoesNotThrow()
    {
        var p = new TPlayObject();

        p.ClearAllDelayLabel();

        Assert.Empty(p.m_TimeLabelList);
    }

    /// <summary>
    /// ★ 原文缺陷锁定（原文 8119-8122）：与同族其它删除点（:4155/:4167/:4194/:4244/:6528）
    /// 的"倒序 + 先摘后放"不同，本方法**正向遍历、只 Dispose 不摘链**，
    /// 直到 :8124 才 `Clear`。可观测后果：它**不按 `boDelete` 过滤**，
    /// 哪怕没有任何一条被标记删除，也会把**全表**清空
    /// （对比 `ClearTimeLabel` 的"只标记、不清表"）。
    /// </summary>
    [Fact]
    public void ClearAllDelayLabel_ForwardDisposeWithoutUnlink_OriginalDefect()
    {
        var p = new TPlayObject();
        p.m_TimeLabelList.Add(new TTimeLabel { nType = 1, boDelete = false });
        p.m_TimeLabelList.Add(new TTimeLabel { nType = 2, boDelete = false });

        p.ClearAllDelayLabel();

        // ★ 原文如此：boDelete 全为 False 也一律清空（本方法根本不读 boDelete）
        Assert.Empty(p.m_TimeLabelList);
    }

    // ==================================================================
    // SendGoldInfo（原文 9214-9228）
    // ==================================================================

    /// <summary>原文 9220-9221：五个名称用 `#13` 连接（**不是** sLineBreak）。</summary>
    [Fact]
    public void SendGoldInfo_WithName_BuildsFiveNamesJoinedByCr()
    {
        var def = CaptureDef();

        new TPlayObject().SendGoldInfo(true);

        Assert.Single(def);
        Assert.Equal(
            M2Config.sGameGoldName + '\r' + M2Config.sGamePointName + '\r' + ObjNpcConst.sSTRING_GOLDNAME
            + '\r' + M2Config.sGameDiamondName + '\r' + M2Config.sGameGirdName,
            def[0].Msg);
        Assert.DoesNotContain("\r\n", def[0].Msg);   // 确认是单字节 CR
    }

    /// <summary>原文 9224：`boSendName = False` → 载荷为空串，但**仍然下发**（不早退）。</summary>
    [Fact]
    public void SendGoldInfo_WithoutName_SendsEmptyPayload()
    {
        var def = CaptureDef();

        new TPlayObject().SendGoldInfo(false);

        Assert.Single(def);
        Assert.Equal("", def[0].Msg);
    }

    /// <summary>
    /// 原文 9225 的报文形状：`SM_GAMEGOLDNAME`(nRecog = m_nGameGold)
    /// + `LoWord(m_nGamePoint)` / `HiWord(m_nGamePoint)`，wSeries = 0。
    /// </summary>
    [Fact]
    public void SendGoldInfo_MessageShape_MatchesOriginal9225()
    {
        var def = CaptureDef();
        var p = new TPlayObject();
        p.m_nGameGold = 4321;
        p.m_nGamePoint = 0x0007_0009;   // Lo=9, Hi=7

        p.SendGoldInfo(false);

        Assert.Single(def);
        Assert.Equal((ushort)Grobal2Const.SM_GAMEGOLDNAME, def[0].Ident);
        Assert.Equal(4321L, def[0].Recog);
        Assert.Equal((ushort)9, def[0].Param);
        Assert.Equal((ushort)7, def[0].Tag);
        Assert.Equal((ushort)0, def[0].Series);
    }

    /// <summary>★ 差异锁定：`LoWord/HisWord` 对**负值**按 LongWord 重解释（Delphi 语义），不抛异常。</summary>
    [Fact]
    public void SendGoldInfo_NegativeGamePoint_LoHiWordAreBitReinterpreted()
    {
        var def = CaptureDef();
        var p = new TPlayObject();
        p.m_nGamePoint = -1;

        p.SendGoldInfo(false);

        Assert.Equal((ushort)0xFFFF, def[0].Param);
        Assert.Equal((ushort)0xFFFF, def[0].Tag);
    }

    // ==================================================================
    // SendNewGamePointInfo（原文 9229-9240）
    // ==================================================================

    /// <summary>原文 9234：三项（金刚石 / 灵符 / 声望）用 `#13` 连接。</summary>
    [Fact]
    public void SendNewGamePointInfo_WithName_BuildsThreeNamesJoinedByCr()
    {
        var def = CaptureDef();

        new TPlayObject().SendNewGamePointInfo(true);

        Assert.Single(def);
        Assert.Equal(
            M2Config.sGameDiamondName + '\r' + M2Config.sGameGirdName + '\r' + M2Config.sCreditPointName,
            def[0].Msg);
    }

    /// <summary>原文 9236：`boSendName = False` → 空载荷，仍下发。</summary>
    [Fact]
    public void SendNewGamePointInfo_WithoutName_SendsEmptyPayload()
    {
        var def = CaptureDef();

        new TPlayObject().SendNewGamePointInfo(false);

        Assert.Single(def);
        Assert.Equal("", def[0].Msg);
        Assert.Equal((ushort)Grobal2Const.SM_GAMEPOINTNAME, def[0].Ident);
    }

    /// <summary>
    /// ★★ 原文缺陷锁定（原文 9237）：本条报文标识是 `SM_GAMEPOINTNAME`（**游戏点**），
    /// 但 ① nRecog 传 `m_nGameDiamond`、② Param/Tag 拆 `m_nGameGird`、
    /// ③ 名称串只有三项 —— **`m_nGamePoint` 自始至终没有被读取**。
    /// </summary>
    [Fact]
    public void SendNewGamePointInfo_GamePointValueIsNeverSent_OriginalDefect()
    {
        var def = CaptureDef();
        var p = new TPlayObject();
        p.m_nGamePoint = 777;          // ★ 这个值在本方法里**不存在任何出口**
        p.m_nGameDiamond = 11;
        p.m_nGameGird = 0x0005_0003;   // Lo=3, Hi=5

        p.SendNewGamePointInfo(true);

        Assert.Single(def);
        Assert.Equal((ushort)Grobal2Const.SM_GAMEPOINTNAME, def[0].Ident);
        Assert.Equal(11L, def[0].Recog);              // ← 金刚石，不是游戏点
        Assert.Equal((ushort)3, def[0].Param);        // ← 灵符 Lo
        Assert.Equal((ushort)5, def[0].Tag);          // ← 灵符 Hi
        // 游戏点 777 在任何字段里都找不到
        Assert.NotEqual(777L, def[0].Recog);
        Assert.DoesNotContain("777", def[0].Msg);
    }

    // ==================================================================
    // SendGameGlory（原文 9241-9245）
    // ==================================================================

    /// <summary>原文 9242：单条 `SM_GAMEGLORY`，nRecog = `m_nGameGlory`，其余全 0。</summary>
    [Fact]
    public void SendGameGlory_SendsGloryValue()
    {
        var def = CaptureDef();
        var p = new TPlayObject();
        p.m_nGameGlory = 2468;

        p.SendGameGlory();

        Assert.Single(def);
        Assert.Equal((ushort)Grobal2Const.SM_GAMEGLORY, def[0].Ident);
        Assert.Equal(2468L, def[0].Recog);
        Assert.Equal((ushort)0, def[0].Param);
        Assert.Equal((ushort)0, def[0].Tag);
        Assert.Equal((ushort)0, def[0].Series);
        Assert.Equal("", def[0].Msg);
    }

    /// <summary>
    /// ★ 原文如此锁定：`SendGameGlory` **没有** `m_boOffLine or m_boDummyObject` 早退
    /// （对比同族 :9920 / :9976 / :10015 都有）—— 离线/假人时**照发**。
    /// </summary>
    [Fact]
    public void SendGameGlory_HasNoOfflineEarlyExit_OriginalAsIs()
    {
        var def = CaptureDef();
        var p = new TPlayObject();
        p.m_boOffLine = true;
        p.m_boDummyObject = true;

        p.SendGameGlory();

        Assert.Single(def);
    }

    // ==================================================================
    // SendClientBlackModules（原文 9914-9938）
    // ==================================================================

    /// <summary>原文 9920-9921：`m_boOffLine` → 直接 Exit（不发任何报文）。</summary>
    [Fact]
    public void SendClientBlackModules_OffLine_DoesNotSend()
    {
        ClientModuleState.g_BlackModuleList.Add(new TModuleInfo { sMD5 = "AA" });
        var sock = CaptureSocket();
        var p = new TPlayObject();
        p.m_boOffLine = true;

        p.SendClientBlackModules();

        Assert.Empty(sock);
    }

    /// <summary>原文 9920-9921：`m_boDummyObject` → 同样 Exit。</summary>
    [Fact]
    public void SendClientBlackModules_DummyObject_DoesNotSend()
    {
        ClientModuleState.g_BlackModuleList.Add(new TModuleInfo { sMD5 = "AA" });
        var sock = CaptureSocket();
        var p = new TPlayObject();
        p.m_boDummyObject = true;

        p.SendClientBlackModules();

        Assert.Empty(sock);
    }

    /// <summary>
    /// 原文 9923-9935 的正常路径：黑名单 MD5 逐条成行（`TStringList.Text`，CRLF 结尾）
    /// → `zEncodeString` → 以 AnsiString 字节面送出，报文标识 `SM_BLACKMODULEMD5`。
    /// </summary>
    [Fact]
    public void SendClientBlackModules_PayloadIsEncodedMd5Lines()
    {
        ClientModuleState.g_BlackModuleList.Add(new TModuleInfo { sMD5 = "AA11" });
        ClientModuleState.g_BlackModuleList.Add(new TModuleInfo { sMD5 = "BB22" });
        var sock = CaptureSocket();

        new TPlayObject().SendClientBlackModules(9);

        Assert.Single(sock);
        Assert.Equal((ushort)Grobal2Const.SM_BLACKMODULEMD5, sock[0].Msg.Ident);
        Assert.Equal((ushort)9, sock[0].Msg.Series);       // ShowProgress → wSeries

        byte[] encoded = EDcode.zEncodeString("AA11\r\nBB22\r\n");
        Assert.Equal(encoded.Length, sock[0].Payload.Length);   // ★ 字节数必须一一对应
        for (int i = 0; i < encoded.Length; i++)
            Assert.Equal(encoded[i], (byte)sock[0].Payload[i]); // ★ AnsiString 的字节保持映射
        Assert.Equal(Latin1(encoded), sock[0].Payload);
    }

    /// <summary>边界：黑名单为空 → 仍下发一条 `SM_BLACKMODULEMD5`（载荷为"空串的编码"）。</summary>
    [Fact]
    public void SendClientBlackModules_EmptyBlackList_StillSends()
    {
        var sock = CaptureSocket();

        new TPlayObject().SendClientBlackModules();

        Assert.Single(sock);
        Assert.Equal((ushort)Grobal2Const.SM_BLACKMODULEMD5, sock[0].Msg.Ident);
        Assert.Equal(Latin1(EDcode.zEncodeString("")), sock[0].Payload);
    }

    /// <summary>
    /// ★★ 原文缺陷锁定（原文 9913 的形参 `ClientCRC`）：全方法体**从不读取** `ClientCRC`
    /// —— 同族的 `SendSpecialCmdList`(:9269)/`SendEffectImageList`(:9315) 都有
    /// `g_XxxCRC &lt;&gt; ClientCRC` 短路，**只有本方法没有**。
    /// 可观测后果：无论客户端报什么 CRC，都拿到**同一份全量载荷**（不存在"CRC 命中就发空"的分支）。
    /// </summary>
    [Fact]
    public void SendClientBlackModules_ClientCrcIsIgnored_OriginalDefect()
    {
        ClientModuleState.g_BlackModuleList.Add(new TModuleInfo { sMD5 = "CC33" });

        var sockA = CaptureSocket();
        new TPlayObject().SendClientBlackModules(0, 0);
        var sockB = CaptureSocket();
        new TPlayObject().SendClientBlackModules(0, 0xDEADBEEF);

        Assert.Single(sockA);
        Assert.Single(sockB);
        // ★ 两个完全不同的 ClientCRC 得到**逐字节相同**的载荷 ⇒ 参数被忽略、无短路
        Assert.Equal(sockA[0].Payload, sockB[0].Payload);
        Assert.NotEqual("", sockB[0].Payload);
        Assert.Equal(sockA[0].Msg.Ident, sockB[0].Msg.Ident);
    }

    // ==================================================================
    // SendArrButtonConfig（原文 9975-9991）
    // ==================================================================

    /// <summary>
    /// 原文 9976-9982：CRC 不等 → 走 `SendSocketEx`，尾数据是
    /// `SizeOf(g_ArrButtonConfig)`（7 组 × 6 个 Integer × 4 = 168 字节）的**内存原样**块。
    /// </summary>
    [Fact]
    public void SendArrButtonConfig_CrcMismatch_GoesThroughSendSocketEx_With168Bytes()
    {
        M2Config.g_ArrButtonConfigCRC = 0x1111_2222;
        M2Config.g_ArrButtonConfig[0].OffsetX = 0x0102_0304;

        var ex = CaptureSocketEx();
        var sock = CaptureSocket();

        new TPlayObject().SendArrButtonConfig(5, 0x9999_8888);

        Assert.Empty(sock);
        Assert.Single(ex);
        Assert.Equal((ushort)Grobal2Const.SM_ARR_BUTTON_CONFIG, ex[0].Msg.Ident);
        Assert.Equal((ushort)5, ex[0].Msg.Series);
        Assert.Equal(7 * 6 * sizeof(int), ex[0].Buf.Length);
        // 结构体字段序：HorzAligment(0) / VertAligment(4) / OffsetX(8) / OffsetY(12) / ...
        Assert.Equal(0x0102_0304, BitConverter.ToInt32(ex[0].Buf, 8));
    }

    /// <summary>原文 9986-9987：CRC 相等 → 走 `SendSocket`，载荷为**空串**（不带配置块）。</summary>
    [Fact]
    public void SendArrButtonConfig_CrcMatch_GoesThroughSendSocket_WithEmptyPayload()
    {
        M2Config.g_ArrButtonConfigCRC = 0xABCD_1234;

        var ex = CaptureSocketEx();
        var sock = CaptureSocket();

        new TPlayObject().SendArrButtonConfig(3, 0xABCD_1234);

        Assert.Empty(ex);
        Assert.Single(sock);
        Assert.Equal((ushort)Grobal2Const.SM_ARR_BUTTON_CONFIG, sock[0].Msg.Ident);
        Assert.Equal((ushort)3, sock[0].Msg.Series);
        Assert.Equal("", sock[0].Payload);
    }

    /// <summary>原文 9976-9977：离线 / 假人 → 两个分支都不走。</summary>
    [Fact]
    public void SendArrButtonConfig_OffLineOrDummy_DoesNotSend()
    {
        M2Config.g_ArrButtonConfigCRC = 0x0000_0001;

        var ex = CaptureSocketEx();
        var sock = CaptureSocket();

        var p = new TPlayObject();
        p.m_boOffLine = true;
        p.SendArrButtonConfig(0, 0);
        p.m_boOffLine = false;
        p.m_boDummyObject = true;
        p.SendArrButtonConfig(0, 0);

        Assert.Empty(ex);
        Assert.Empty(sock);
    }

    /// <summary>
    /// ★★ 原文缺陷锁定（原文 9981-9987）：两个分支都用**同一个**标识
    /// `SM_ARR_BUTTON_CONFIG`（没有 `*_CACHE` 变体），且**报文面不对称** ——
    /// 命中分支走 `SendSocket(..., '')`、未命中分支走 `SendSocketEx(..., 168 字节)`。
    /// </summary>
    [Fact]
    public void SendArrButtonConfig_BranchesAreAsymmetric_OriginalDefect()
    {
        M2Config.g_ArrButtonConfigCRC = 0x0000_0007;

        // 未命中
        var ex1 = CaptureSocketEx();
        var sock1 = CaptureSocket();
        new TPlayObject().SendArrButtonConfig(1, 0x1234);
        var missIdent = ex1[0].Msg.Ident;

        // 命中
        var ex2 = CaptureSocketEx();
        var sock2 = CaptureSocket();
        new TPlayObject().SendArrButtonConfig(1, 0x0000_0007);
        var hitIdent = sock2[0].Msg.Ident;

        // ★ 两个分支标识**完全相同**（不存在 *_CACHE 区分）
        Assert.Equal(missIdent, hitIdent);
        Assert.Equal((ushort)Grobal2Const.SM_ARR_BUTTON_CONFIG, hitIdent);
        // ★ 发送面不对称
        Assert.Empty(sock1);        // 未命中：不走 SendSocket
        Assert.Empty(ex2);          // 命中：不走 SendSocketEx
        Assert.Equal("", sock2[0].Payload);
        Assert.NotEqual(0, ex1[0].Buf.Length);
    }

    // ==================================================================
    // 留痕计数取证（台账 §37.3）—— 否定性断言
    // ==================================================================

    /// <summary>
    /// 8 条真实体方法**不得**产生任何 `PortNotPorted` 留痕（防"半移植后还挂着留痕"）。
    /// </summary>
    [Fact]
    public void Core4_PortedMethods_DoNotRegisterNotPortedBookkeeping()
    {
        Assert.Equal(0, PlayerSurfacePortLedger.NotPortedCount);

        var p = new TPlayObject();
        p.ClearStatusTime();
        p.ClearTimeLabel(0);
        p.ClearAllDelayLabel();
        p.SendGoldInfo(false);
        p.SendNewGamePointInfo(false);
        p.SendGameGlory();
        p.SendClientBlackModules();
        p.SendArrButtonConfig();

        Assert.Equal(0, PlayerSurfacePortLedger.NotPortedCount);
    }

    /// <summary>
    /// 34 条未移植方法**逐条**留痕，总数恰为 34（8 + 34 = 42 = 本切片方法总数）。
    /// 若有人把某条写成裸 `=> true;` 或漏掉留痕，本用例立刻失败。
    /// </summary>
    [Fact]
    public void Core4_NotPortedTally_IsExactlyThirtyFour()
    {
        var p = new TPlayObject();
        bool b1 = false, b2 = false, b3 = false, b4 = false;

        p.SendMapDescription();
        p.GetMapCanRun(ref b1, ref b2, ref b3, ref b4);
        p.SendMapCanRun();
        p.SendNotice(false);
        p.UserLogon();
        p.GetBoxItems();
        p.SendSpecialCmdList();
        p.SendEffectImageList();
        p.SendMissionNPC();
        p.SendClientModules();
        p.SendFilterItemList();
        p.SendItemDescList();
        p.SendItemDescTopList();
        p.SendTzItemDescList();
        p.SendCustomMonsterConfig();
        p.SendCustomMagicConfig();
        p.SendCustomNpcConfig();
        p.SendDropItemEffectList();
        p.SendEnabledAuctionItemList();
        p.SendUnbindList();
        p.SendInputBoxFilterList();
        p.SendStdItemList();
        p.SendPlugClientList();
        p.SendCustomMoney();
        p.SendCustomItemPropertyConfig();
        p.SendCustomItemPropertyTextVarList();
        p.SendLogon();
        p.SendServerConfig();
        p.SendEnableClientUploadPickItems();
        p.SendUseItems();
        p.SendUseIcons(p);
        p.SendUseEffects(p);
        p.SendUseMagic();
        p.ClientTakeOnItemsEx(0, 0, "");

        Assert.Equal(34, PlayerSurfacePortLedger.NotPortedCount);
    }

    /// <summary>留痕条目的**形状**必须带原文行号（`方法名 (ObjPlayer.pas:行号)`），且行号落在本切片内。</summary>
    [Fact]
    public void Core4_NotPortedLedgerEntries_CarryOriginalLineNumbersInsideSlice()
    {
        var p = new TPlayObject();
        p.UserLogon();
        p.ClientTakeOnItemsEx(0, 0, "");
        p.SendMapDescription();

        Assert.Equal(new[]
        {
            "UserLogon (ObjPlayer.pas:8308)",
            "ClientTakeOnItemsEx (ObjPlayer.pas:10240)",
            "SendMapDescription (ObjPlayer.pas:8128)",
        }, PlayerSurfacePortLedger.NotPortedMethods);
    }

    /// <summary>留痕幂等（同一方法重复调用只记一条）—— 与 PortKit 的既有契约一致。</summary>
    [Fact]
    public void Core4_NotPortedBookkeeping_IsIdempotent()
    {
        var p = new TPlayObject();
        p.SendUseMagic();
        p.SendUseMagic();
        p.SendUseMagic();

        Assert.Equal(1, PlayerSurfacePortLedger.NotPortedCount);
    }
}
