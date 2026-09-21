// ============================================================================
// 测试：车道 p13-m2-objplayer —— **核心片 6**（ObjPlayer.pas 12689-14898）。
// 被测托管源：GXX.M2Server/Engine/PlayerSurface/TPlayObject.PlayerSurface.Core6.cs
// 原文出处：Source/M2Engine/ObjPlayer.pas:12710-14379（真实体 31 条）
//
// 覆盖要求（任务书）：
//   · 每个已移植方法族 ≥1 个 [Fact]，含正常路径 + 边界；
//   · 每个「原文如此」缺陷各有一条**锁定缺陷**的断言（方法名带 _OriginalDefect）；
//   · 自足、确定：`new TPlayObject()` + 公共成员 + 接缝注入（随机数/时钟一律注入）。
//
// 「原文如此」四条（K=4）：
//   ① SendUpdateItemHeroM2Light 与 SendUpdateItemInsuranceCount 的 ident 相同（均 10330）
//   ② SendDelDealItem 把 SM_DEALREMOTEDELITEM 发给**自己**（SendAddDealItem 却发给对方）
//   ③ SendUpdateItemPropertyText **不**做 EncodeString（SendUpdateItemName 做）
//   ④ SysMsg/SysMsgEx 的 boAddPrefix 参数从未被使用（前缀只受 g_Config.boShowPreFixMsg 支配）
// ============================================================================

using System;
using System.Collections.Generic;
using GXX.Core.Protocol;
using GXX.M2Server.Engine;
using Xunit;

namespace GXX.M2Server.Tests;

/// <summary>
/// 本片测试的**串行集合**：本文件的用例会读写若干**进程级静态量**
/// （<see cref="PlayerSurfacePortLedger"/> 留痕台账、<c>M2Config</c> 的前缀/颜色/开关、
/// 以及 <c>PlayerSurface*Seams</c> 一族静态接缝）。
/// 同车道的其它片测试（Core1/Core2/Core3/Core4…）也读写同一批静态量，
/// 故本集合声明 <c>DisableParallelization = true</c> —— 保证本文件的用例**不与任何其它集合并发**，
/// 使「留痕条数 == 16」这类精确断言保持确定性（否则会被并行片的重置/累加打乱）。
/// </summary>
[CollectionDefinition("PlayerSurfacePortLedgerSerial", DisableParallelization = true)]
public class PlayerSurfacePortLedgerSerialCollection
{
}

[Collection("PlayerSurfacePortLedgerSerial")]
public class ObjPlayerCore6Tests : IDisposable
{
    // ---- 采集面 -----------------------------------------------------
    private readonly List<(TDefaultMessage msg, string text)> _socket = new();
    private readonly List<(TDefaultMessage msg, byte[] buf)> _socketEx = new();
    private readonly List<(TPlayObject target, ushort ident, long recog, ushort p, ushort tag, ushort series, string text)> _defMsg = new();
    private readonly List<(TPlayObject target, TDefaultMessage msg, object? buf)> _socketExTo = new();
    private readonly List<string> _spaceMove = new();
    private readonly List<string> _mapRandomMove = new();
    private readonly List<string> _customMagicQuery = new();

    // ---- 需要保存/还原的全局配置（M2Config 为静态） -----------------
    private readonly bool _svShowPrefix;
    private readonly string _svMonPrefix, _svHintPrefix, _svGmPrefix, _svSysPrefix, _svLinePrefix, _svCustPrefix, _svCastlePrefix;
    private readonly byte _svGreenF, _svGreenB, _svBlueF, _svBlueB, _svRedF, _svRedB, _svCustF, _svCustB, _svChallengeGold;
    private readonly bool _svCheckActionCount;

    private readonly Queue<uint> _tickQueue = new();

    public ObjPlayerCore6Tests()
    {
        // 保存
        _svShowPrefix = M2Config.boShowPreFixMsg;
        _svMonPrefix = M2Config.sMonSayMsgpreFix;
        _svHintPrefix = M2Config.sHintMsgPreFix;
        _svGmPrefix = M2Config.sGMRedMsgpreFix;
        _svSysPrefix = M2Config.sSysMsgPreFix;
        _svLinePrefix = M2Config.sLineNoticePreFix;
        _svCustPrefix = M2Config.sCustMsgpreFix;
        _svCastlePrefix = M2Config.sCastleMsgpreFix;
        _svGreenF = M2Config.btGreenMsgFColor;
        _svGreenB = M2Config.btGreenMsgBColor;
        _svBlueF = M2Config.btBlueMsgFColor;
        _svBlueB = M2Config.btBlueMsgBColor;
        _svRedF = M2Config.btRedMsgFColor;
        _svRedB = M2Config.btRedMsgBColor;
        _svCustF = M2Config.btCustMsgFColor;
        _svCustB = M2Config.btCustMsgBColor;
        _svChallengeGold = M2Config.btChallengeGoldIndex;
        _svCheckActionCount = M2Config.boCheckActionCount;

        HookSeams();
    }

    public void Dispose()
    {
        PlayerSurfaceSocketSeams.ResetDefaults();
        PlayerSurfaceMsgSeams.ResetDefaults();
        PlayerSurfaceItemSeams.ResetDefaults();
        PlayerSurfaceCore6Seams.ResetDefaults();
        PlayerSurfaceNpcSeams.ResetDefaults();
        PlayerSurfacePortLedger.ResetNotPorted();

        M2Config.boShowPreFixMsg = _svShowPrefix;
        M2Config.sMonSayMsgpreFix = _svMonPrefix;
        M2Config.sHintMsgPreFix = _svHintPrefix;
        M2Config.sGMRedMsgpreFix = _svGmPrefix;
        M2Config.sSysMsgPreFix = _svSysPrefix;
        M2Config.sLineNoticePreFix = _svLinePrefix;
        M2Config.sCustMsgpreFix = _svCustPrefix;
        M2Config.sCastleMsgpreFix = _svCastlePrefix;
        M2Config.btGreenMsgFColor = _svGreenF;
        M2Config.btGreenMsgBColor = _svGreenB;
        M2Config.btBlueMsgFColor = _svBlueF;
        M2Config.btBlueMsgBColor = _svBlueB;
        M2Config.btRedMsgFColor = _svRedF;
        M2Config.btRedMsgBColor = _svRedB;
        M2Config.btCustMsgFColor = _svCustF;
        M2Config.btCustMsgBColor = _svCustB;
        M2Config.btChallengeGoldIndex = _svChallengeGold;
        M2Config.boCheckActionCount = _svCheckActionCount;
    }

    // ---- 共用装配 ---------------------------------------------------

    private void HookSeams()
    {
        PlayerSurfaceSocketSeams.ResetDefaults();
        PlayerSurfaceMsgSeams.ResetDefaults();
        PlayerSurfaceItemSeams.ResetDefaults();
        PlayerSurfaceCore6Seams.ResetDefaults();
        PlayerSurfaceNpcSeams.ResetDefaults();
        PlayerSurfacePortLedger.ResetNotPorted();

        PlayerSurfaceSocketSeams.SendSocket = (_, msg, text) => _socket.Add((msg, text));
        PlayerSurfaceSocketSeams.SendSocketEx = (_, msg, buf) => _socketEx.Add((msg, buf));
        PlayerSurfaceMsgSeams.SendDefMessage =
            (target, ident, recog, p, tag, series, text) => _defMsg.Add((target, ident, recog, p, tag, series, text));
        PlayerSurfaceCore6Seams.SendSocketExTo = (target, msg, buf) => _socketExTo.Add((target, msg, buf));
        PlayerSurfaceCore6Seams.SpaceMove = (_, map, x, y, n) => _spaceMove.Add($"{map}|{x}|{y}|{n}");
        PlayerSurfaceCore6Seams.MapRandomMove = (_, map, n) => _mapRandomMove.Add($"{map}|{n}");
        PlayerSurfaceCore6Seams.CustomMagicHasFailMsg = id =>
        {
            _customMagicQuery.Add(id.ToString());
            return false;
        };
        PlayerSurfaceNpcSeams.MyGetTickCount = () => _tickQueue.Count > 0 ? _tickQueue.Dequeue() : 4242u;
    }

    private static TPlayObject NewPlayer() => new TPlayObject();

    /// <summary>注入固定 stdItem（原文 `UserEngine.GetStdItem` 的接缝）。</summary>
    private static void SetStdItem(TStdItem? item) => PlayerSurfaceItemSeams.GetStdItem = _ => item;

    /// <summary>构造一个带标准名的 stdItem。</summary>
    private static TStdItem StdItemNamed(string name, byte weight = 0, byte stdMode = 0, int dc1 = 0, int dc2 = 0,
        int ac1 = 0, int ac2 = 0)
    {
        var s = new TStdItem { Weight = weight, StdMode = stdMode, DC1 = dc1, DC2 = dc2, AC1 = ac1, AC2 = ac2 };
        s.NameStr = name;
        return s;
    }

    private static TUserItem UserItemNamed(string name, ushort wIndex = 0, int makeIndex = 0)
    {
        var it = new TUserItem { wIndex = wIndex, MakeIndex = makeIndex };
        it.NameStr = name;
        return it;
    }

    // =================================================================
    // 一、SendUpdateItem* 族（原文 12710-12866；17 条）
    // =================================================================

    [Fact]
    public void SendUpdateItemName_NormalPath_IdentRecogParamTagSeries_AndEncodesName()
    {
        // 原文 12715：MakeDefaultMsg(SM_UPDATEITEM_NAME, MakeIndex, nWhere, Integer(IsHero) shl 1 or Integer(IsBoxItem), 0)
        var p = NewPlayer();
        p.SendUpdateItemName(2, 777, false, "屠龙");

        var (msg, text) = Assert.Single(_socket);
        Assert.Equal((ushort)Grobal2Const.SM_UPDATEITEM_NAME, msg.Ident);
        Assert.Equal(777L, msg.Recog);   // MakeIndex → nRecog
        Assert.Equal((ushort)2, msg.Param);  // nWhere → wParam
        Assert.Equal((ushort)0, msg.Tag);
        Assert.Equal((ushort)0, msg.Series);
        // 原文 12716：SendSocket(@m_DefMsg, EncodeString(NewItemName)) —— **编码后**的字符串
        Assert.Equal(EDcode.EncodeStringText("屠龙"), text);
        Assert.NotEqual("屠龙", text);
    }

    [Fact]
    public void SendUpdateItemName_IsBoxItemBoundary_SetsOnlyTagBit0_HeroBitNeverSet()
    {
        // 原文 12715 的 `Integer(IsHero) shl 1 or Integer(IsBoxItem)`，而 IsHero 在 12714 被硬置 False ⇒ Tag 只能取 0/1
        var p = NewPlayer();
        p.SendUpdateItemName(0, 1, true, "x");
        p.SendUpdateItemName(0, 1, false, "x");
        Assert.Equal((ushort)1, _socket[0].msg.Tag);   // IsBoxItem=True
        Assert.Equal((ushort)0, _socket[1].msg.Tag);   // IsBoxItem=False
    }

    [Fact]
    public void SendUpdateItemColor_NormalPath_ColorGoesToSeries()
    {
        // 原文 12724
        var p = NewPlayer();
        p.SendUpdateItemColor(3, 55, true, 0xFC);

        var (msg, text) = Assert.Single(_socket);
        Assert.Equal((ushort)Grobal2Const.SM_UPDATEITEM_COLOR, msg.Ident);
        Assert.Equal(55L, msg.Recog);
        Assert.Equal((ushort)3, msg.Param);
        Assert.Equal((ushort)1, msg.Tag);
        Assert.Equal((ushort)0xFC, msg.Series);
        Assert.Equal("", text);   // 原文 12725：SendSocket(@m_DefMsg, '')
    }

    [Fact]
    public void SendUpdateItemDura_NormalPath()
    {
        var p = NewPlayer();
        p.SendUpdateItemDura(1, 9, false, 65000);
        var (msg, _) = Assert.Single(_socket);
        Assert.Equal((ushort)Grobal2Const.SM_UPDATEITEM_DURA, msg.Ident);
        Assert.Equal(9L, msg.Recog);
        Assert.Equal((ushort)1, msg.Param);
        Assert.Equal((ushort)65000, msg.Series);
    }

    [Fact]
    public void SendUpdateItemDuraMax_NormalPath()
    {
        var p = NewPlayer();
        p.SendUpdateItemDuraMax(1, 9, true, 65535);
        var (msg, _) = Assert.Single(_socket);
        Assert.Equal((ushort)Grobal2Const.SM_UPDATEITEM_DURAMAX, msg.Ident);
        Assert.Equal((ushort)1, msg.Tag);
        Assert.Equal((ushort)65535, msg.Series);
    }

    [Fact]
    public void SendUpdateItemUpgradeCount_NormalPath_ByteToSeries()
    {
        var p = NewPlayer();
        p.SendUpdateItemUpgradeCount(4, 12, false, 255);
        var (msg, _) = Assert.Single(_socket);
        Assert.Equal((ushort)Grobal2Const.SM_UPDATEITEM_UPGRADECOUNT, msg.Ident);
        Assert.Equal((ushort)255, msg.Series);
    }

    [Fact]
    public void SendUpdateItemNewLook_NormalPath()
    {
        var p = NewPlayer();
        p.SendUpdateItemNewLook(5, 13, false, 1234);
        var (msg, _) = Assert.Single(_socket);
        Assert.Equal((ushort)Grobal2Const.SM_UPDATEITEM_NEWLOOK, msg.Ident);
        Assert.Equal((ushort)1234, msg.Series);
    }

    [Fact]
    public void SendUpdateItemNewShape_NormalPath()
    {
        var p = NewPlayer();
        p.SendUpdateItemNewShape(6, 14, true, 4321);
        var (msg, _) = Assert.Single(_socket);
        Assert.Equal((ushort)Grobal2Const.SM_UPDATEITEM_NEWSHAPE, msg.Ident);
        Assert.Equal((ushort)1, msg.Tag);
        Assert.Equal((ushort)4321, msg.Series);
    }

    [Fact]
    public void SendUpdateItemHeroM2Light_NormalPath_AndByteBoundary()
    {
        var p = NewPlayer();
        p.SendUpdateItemHeroM2Light(1, 5, false, 0);
        p.SendUpdateItemHeroM2Light(1, 5, false, 255);
        Assert.Equal((ushort)Grobal2Const.SM_UPDATEITEM_HEROM2LIGHT, _socket[0].msg.Ident);
        Assert.Equal((ushort)0, _socket[0].msg.Series);
        Assert.Equal((ushort)255, _socket[1].msg.Series);
    }

    [Fact]
    public void SendUpdateItemInsuranceCount_DuplicateIdentWithHeroM2Light_OriginalDefect()
    {
        // ★ 原文如此 ①（锁定缺陷）：Grobal2.pas 里两条 ident 取值相同（均 10330）——
        //   `SM_UPDATEITEM_HEROM2LIGHT`（原文 1805）与 `SM_UPDATEITEM_INSURANCECOUNT`（原文 1806）。
        //   于是「英雄 M2 光效」与「投保次数」在客户端**无法区分**。照抄原文，不修。
        Assert.Equal(Grobal2Const.SM_UPDATEITEM_HEROM2LIGHT, Grobal2Const.SM_UPDATEITEM_INSURANCECOUNT);
        Assert.Equal(10330, Grobal2Const.SM_UPDATEITEM_INSURANCECOUNT);

        var p = NewPlayer();
        p.SendUpdateItemHeroM2Light(1, 5, false, 3);
        p.SendUpdateItemInsuranceCount(1, 5, false, 3);
        Assert.Equal(_socket[0].msg.Ident, _socket[1].msg.Ident);   // 报文无法区分
    }

    [Fact]
    public void SendUpdateItemBind_NormalAndBoundary_TrueIsOne()
    {
        // 原文 12800：Word(IsBind) —— Delphi Boolean→Word 是 1/0（**不是** $FFFF）
        var p = NewPlayer();
        p.SendUpdateItemBind(1, 5, false, true);
        p.SendUpdateItemBind(1, 5, false, false);
        Assert.Equal((ushort)Grobal2Const.SM_UPDATEITEM_BIND, _socket[0].msg.Ident);
        Assert.Equal((ushort)1, _socket[0].msg.Series);
        Assert.Equal((ushort)0, _socket[1].msg.Series);
    }

    [Fact]
    public void SendUpdateItemLimitTime_SplitsInt32IntoLoHiWords()
    {
        // 原文 12806：wTag := LoWord(LimitTime); wSeries := HiWord(LimitTime)
        var p = NewPlayer();
        p.SendUpdateItemLimitTime(2, 8, 0x12345678);
        var (msg, _) = Assert.Single(_socket);
        Assert.Equal((ushort)Grobal2Const.SM_UPDATEITEM_LIMITTIME, msg.Ident);
        Assert.Equal((ushort)0x5678, msg.Tag);
        Assert.Equal((ushort)0x1234, msg.Series);
    }

    [Fact]
    public void SendUpdateItemLimitTime_NegativeValue_UsesComplementBits()
    {
        // 边界：负数按 32 位补码 —— LoWord(-1) = $FFFF、HiWord(-1) = $FFFF
        var p = NewPlayer();
        p.SendUpdateItemLimitTime(0, 1, -1);
        var (msg, _) = Assert.Single(_socket);
        Assert.Equal((ushort)0xFFFF, msg.Tag);
        Assert.Equal((ushort)0xFFFF, msg.Series);
    }

    [Fact]
    public void SendUpdateItemNewValue_TagLayoutIsLowByteBoxItem_HighByteValueIndex()
    {
        // 原文 12813：MakeWord(Byte(IsBoxItem), ValueIndex) —— 与同族其它方法的 Tag 布局**不同**
        var p = NewPlayer();
        p.SendUpdateItemNewValue(3, 77, true, 0x0A, 999);
        var (msg, _) = Assert.Single(_socket);
        Assert.Equal((ushort)Grobal2Const.SM_UPDATEITEM_NEWVALUE, msg.Ident);
        Assert.Equal((ushort)0x0A01, msg.Tag);   // 低字节 1（IsBoxItem）、高字节 0x0A（ValueIndex）
        Assert.Equal((ushort)999, msg.Series);
    }

    [Fact]
    public void SendUpdateItemNewValue_ValueIndexBoundary_255()
    {
        var p = NewPlayer();
        p.SendUpdateItemNewValue(3, 77, false, 255, 0);
        Assert.Equal((ushort)0xFF00, _socket[0].msg.Tag);
    }

    [Fact]
    public void SendUpdateItemFlute_TailIs32BytesOfEightFlutes()
    {
        // 原文 12824：SendSocketEx(@m_DefMsg, @UserItem.Flutes[0], SizeOf(UserItem.Flutes)) = 8×4 = 32 字节
        var p = NewPlayer();
        var it = UserItemNamed("武器", makeIndex: 4242);
        it.btFluteCount = 3;
        it.SetFlute(0, new TFluteInfo { GemIndex = 0x1122, GemCount = 0x3344 });

        p.SendUpdateItemFlute(1, false, it);

        var (msg, buf) = Assert.Single(_socketEx);
        Assert.Equal((ushort)Grobal2Const.SM_UPDATEITEM_FLUTE, msg.Ident);
        Assert.Equal(4242L, msg.Recog);
        Assert.Equal((ushort)3, msg.Series);       // btFluteCount
        Assert.Equal(32, buf.Length);
        Assert.Equal(0x22, buf[0]);                 // GemIndex 低字节（小端）
        Assert.Equal(0x11, buf[1]);
        Assert.Equal(0x44, buf[2]);                 // GemCount 低字节
        Assert.Equal(0x33, buf[3]);
        Assert.Equal(0, buf[4]);                    // 第 2 个凹槽未写
    }

    [Fact]
    public void SendUpdateItemProgress_TailIs42Bytes_Index0AndIndex1Boundary()
    {
        var p = NewPlayer();
        var it = UserItemNamed("物品", makeIndex: 9);
        it.Progress0 = new TUserItemProgress { boOpen = 1, btNameColor = 2, btCount = 3, btShowType = 1, wMax = 100, wValue = 50 };
        it.Progress1 = new TUserItemProgress { wMax = 200, wValue = 20 };

        p.SendUpdateItemProgress(1, false, it, 0);
        p.SendUpdateItemProgress(1, false, it, 1);

        var (msg0, buf0) = _socketEx[0];
        var (_, buf1) = _socketEx[1];
        Assert.Equal((ushort)Grobal2Const.SM_UPDATEITEM_PROGRESS, msg0.Ident);
        Assert.Equal((ushort)0, msg0.Series);        // ProgressIndex
        Assert.Equal(42, buf0.Length);
        Assert.Equal(1, buf0[0]);                    // boOpen
        Assert.Equal(100, buf0[4]);                  // wMax 低字节
        Assert.Equal(50, buf0[6]);                   // wValue 低字节
        Assert.Equal(42, buf1.Length);
        Assert.Equal(200, buf1[4]);
        Assert.Equal(20, buf1[6]);
    }

    [Fact]
    public void SendUpdateItemProgress_OutOfRangeIndex_SendsNoTail_OriginalWasUb()
    {
        // ★ 差异断言（原文缺陷 UB，**不可复刻**）：原文 `Progress[ProgressIndex]` 数组只有 [0..1]，
        //   `ProgressIndex >= 2` 时原文读到数组之外的内存（越界读）。托管侧不复刻越界，
        //   返回空尾数据（报文头仍然发出，只是不带尾数据）。
        var p = NewPlayer();
        var it = UserItemNamed("物品", makeIndex: 9);
        p.SendUpdateItemProgress(1, false, it, 2);

        var (msg, buf) = Assert.Single(_socketEx);
        Assert.Equal((ushort)Grobal2Const.SM_UPDATEITEM_PROGRESS, msg.Ident);
        Assert.Equal((ushort)2, msg.Series);
        Assert.Empty(buf);
    }

    [Fact]
    public void SendUpdateItemPropertyText_NotEncoded_OriginalDefect()
    {
        // ★ 原文如此 ③（锁定缺陷）：原文 12843 是 `SendSocket(@m_DefMsg, sText)`，
        //   **没有** EncodeString（对照 SendUpdateItemName 原文 12716 有）—— 照抄，不修。
        var p = NewPlayer();
        p.SendUpdateItemPropertyText(1, 5, false, "自定义属性文本");

        var (msg, text) = Assert.Single(_socket);
        Assert.Equal((ushort)Grobal2Const.SM_UPDATEITEM_PROPERTYTEXT, msg.Ident);
        Assert.Equal((ushort)0, msg.Series);
        Assert.Equal("自定义属性文本", text);                                  // 原样明文
        Assert.NotEqual(EDcode.EncodeStringText("自定义属性文本"), text);      // 与编码版不同
    }

    [Fact]
    public void SendUpdateItemPropertyColor_NormalPath()
    {
        var p = NewPlayer();
        p.SendUpdateItemPropertyColor(2, 3, true, 0xFB);
        var (msg, text) = Assert.Single(_socket);
        Assert.Equal((ushort)Grobal2Const.SM_UPDATEITEM_PROPERTYCOLOR, msg.Ident);
        Assert.Equal((ushort)1, msg.Tag);
        Assert.Equal((ushort)0xFB, msg.Series);
        Assert.Equal("", text);
    }

    [Fact]
    public void SendUpdateItemPropertyValue_TailIs17Bytes_PackedCustomProperty()
    {
        // 原文 12863-12864：@UserItem.CustomProperty.Properties[i] + SizeOf(TCustomProperty) = 17（packed）
        var p = NewPlayer();
        var it = UserItemNamed("物品", makeIndex: 31);
        var custom = new TUserItemProperty();
        var prop = new TCustomProperty { btColor = 7, btBindType = 1, btShowFlag = 2, btPercent = 1, btHintModule = 3 };
        prop.SetValues(new[] { 11, 22, 33 });
        custom.SetProp(3, prop);
        it.CustomProperty = custom;

        p.SendUpdateItemPropertyValue(1, false, it, 3);

        var (msg, buf) = Assert.Single(_socketEx);
        Assert.Equal((ushort)Grobal2Const.SM_UPDATEITEM_PROPERTYVALUES, msg.Ident);
        Assert.Equal(31L, msg.Recog);
        Assert.Equal((ushort)3, msg.Series);
        Assert.Equal(17, buf.Length);
        Assert.Equal(7, buf[0]);      // btColor
        Assert.Equal(1, buf[1]);      // btBindType
        Assert.Equal(11, buf[5]);     // nValues[0] 低字节
        Assert.Equal(22, buf[9]);     // nValues[1] 低字节
        Assert.Equal(33, buf[13]);    // nValues[2] 低字节
    }

    [Fact]
    public void SendUpdateItemPropertyValue_IndexBoundary_19Valid_20SendsNoTail()
    {
        var p = NewPlayer();
        var it = UserItemNamed("物品", makeIndex: 1);
        p.SendUpdateItemPropertyValue(1, false, it, 19);
        p.SendUpdateItemPropertyValue(1, false, it, 20);

        Assert.Equal(17, _socketEx[0].buf.Length);
        Assert.Empty(_socketEx[1].buf);   // ≥20 = 原文越界 UB，托管不发送
    }

    // =================================================================
    // 二、SysMsg / SysMsgEx（原文 12867-12990；4 条重载）
    // =================================================================

    private static void SetGreenColor(byte f, byte b)
    {
        M2Config.btGreenMsgFColor = f;
        M2Config.btGreenMsgBColor = b;
    }

    [Fact]
    public void SysMsg_NormalPath_IdentRecogColorAndPrefix()
    {
        // 原文 12880（前缀）/ 12898（c_Green → MakeWord(btGreenMsgFColor, btGreenMsgBColor)）/ 12908-12909
        M2Config.boShowPreFixMsg = true;
        M2Config.sMonSayMsgpreFix = "[MON]";
        SetGreenColor(0x11, 0x22);

        var p = NewPlayer();
        p.m_IsSendUserLogon = true;
        p.SysMsg("hello", TMsgColor.c_Green, TMsgType.t_Mon, true);

        var (msg, text) = Assert.Single(_socket);
        Assert.Equal((ushort)Grobal2Const.SM_SYSMESSAGE, msg.Ident);
        Assert.Equal(p.m_nRecogId, msg.Recog);     // NativeInt(Self)
        Assert.Equal((ushort)0x2211, msg.Param);   // MakeWord(F=0x11, B=0x22)
        Assert.Equal((ushort)0, msg.Tag);
        Assert.Equal((ushort)1, msg.Series);
        Assert.Equal("[MON]hello", text);          // 前缀 + 原文（**不编码**）
    }

    [Fact]
    public void SysMsg_CustType_FallsBackToCustColor_OthersUseRed()
    {
        // 原文 12901-12905：非 Green/Blue 时，`MsgType = t_Cust` 用祝福色，其余用红字色
        M2Config.boShowPreFixMsg = false;
        M2Config.btCustMsgFColor = 0xFC;
        M2Config.btCustMsgBColor = 0xFF;
        M2Config.btRedMsgFColor = 0xFF;
        M2Config.btRedMsgBColor = 0x38;

        var p = NewPlayer();
        p.m_IsSendUserLogon = true;
        p.SysMsg("a", TMsgColor.c_Red, TMsgType.t_Cust, true);
        p.SysMsg("b", TMsgColor.c_Red, TMsgType.t_Hint, true);

        Assert.Equal((ushort)0xFFFC, _socket[0].msg.Param);
        Assert.Equal((ushort)0x38FF, _socket[1].msg.Param);
    }

    [Fact]
    public void SysMsg_BlueColor_UsesBlueConfig()
    {
        M2Config.boShowPreFixMsg = false;
        M2Config.btBlueMsgFColor = 0xFF;
        M2Config.btBlueMsgBColor = 0xFC;
        M2Config.btBlueMsgFColor = 0x01;
        M2Config.btBlueMsgBColor = 0x02;

        var p = NewPlayer();
        p.m_IsSendUserLogon = true;
        p.SysMsg("x", TMsgColor.c_Blue, TMsgType.t_Hint, true);
        Assert.Equal((ushort)0x0201, _socket[0].msg.Param);
    }

    [Fact]
    public void SysMsg_OfflineOrDummy_EarlyExit_NoSend()
    {
        // 原文 12871-12872：`m_boOffLine or m_boDummyObject → Exit`（注释「修改离线人物不发送」）
        var p = NewPlayer();
        p.m_IsSendUserLogon = true;
        p.m_boOffLine = true;
        p.SysMsg("x", TMsgColor.c_Red, TMsgType.t_Hint, true);
        Assert.Empty(_socket);

        p.m_boOffLine = false;
        p.m_boDummyObject = true;
        p.SysMsg("x", TMsgColor.c_Red, TMsgType.t_Hint, true);
        Assert.Empty(_socket);
    }

    [Fact]
    public void SysMsg_BoAddPrefixFalse_StillPrefixed_OriginalDefect()
    {
        // ★ 原文如此 ④（锁定缺陷）：`boAddPrefix` 参数在方法体内**从未被使用** ——
        //   前缀只由 `g_Config.boShowPreFixMsg` 决定，传 False 也照样加前缀（原文 12876-12894）。
        M2Config.boShowPreFixMsg = true;
        M2Config.sHintMsgPreFix = "[H]";

        var p = NewPlayer();
        p.m_IsSendUserLogon = true;
        p.SysMsg("body", TMsgColor.c_Red, TMsgType.t_Hint, false);   // boAddPrefix = False

        var (_, text) = Assert.Single(_socket);
        Assert.Equal("[H]body", text);   // 仍然加了前缀 —— 缺陷保留
    }

    [Fact]
    public void SysMsgEx_BoAddPrefixFalse_StillPrefixed_OriginalDefect()
    {
        // ★ 原文如此 ④（SysMsgEx 侧）：SysMsgEx **完全没有前缀块**，故 boAddPrefix 也是死参数
        //   —— 无论传 True/False 都不加前缀（原文 12949-12975）。
        M2Config.boShowPreFixMsg = true;
        M2Config.sHintMsgPreFix = "[H]";

        var p = NewPlayer();
        p.m_IsSendUserLogon = true;
        p.SysMsgEx("body", TMsgColor.c_Red, TMsgType.t_Hint, true);
        p.SysMsgEx("body", TMsgColor.c_Red, TMsgType.t_Hint, false);

        Assert.Equal("body", _socket[0].text);
        Assert.Equal("body", _socket[1].text);
    }

    [Fact]
    public void SysMsg_NotLogon_FallsBackToBaseSysMsg()
    {
        // 原文 12912：`inherited SysMsg(sMsg, MsgColor, MsgType, boAddPrefix)`
        //   托管基类落点 = TCreature.SysMsgs（ObjBase.OnlineMsg.cs:22/25）
        var p = NewPlayer();
        p.m_IsSendUserLogon = false;
        p.SysMsg("fallback", TMsgColor.c_Red, TMsgType.t_Hint, true);

        Assert.Empty(_socket);
        Assert.Contains("fallback", p.SysMsgs);
    }

    [Fact]
    public void SysMsg_FColorBColorOverload_PutsMakeWordIntoParam()
    {
        // 原文 12942：MakeDefaultMsg(SM_SYSMESSAGE, NativeInt(Self), MakeWord(FColor, BColor), 0, 1)
        M2Config.boShowPreFixMsg = false;
        var p = NewPlayer();
        p.m_IsSendUserLogon = true;
        p.SysMsg("m", 255, 249, TMsgType.t_Hint, true);

        var (msg, text) = Assert.Single(_socket);
        Assert.Equal((ushort)0xF9FF, msg.Param);
        Assert.Equal("m", text);
    }

    [Fact]
    public void SysMsgEx_NormalPath_NoPrefixAndColorFromConfig()
    {
        // 原文 12958-12971：SysMsgEx **没有前缀块**，直接算颜色后发 SM_SYSMESSAGE
        M2Config.boShowPreFixMsg = true;         // 故意打开：SysMsgEx 也不该加前缀
        M2Config.sHintMsgPreFix = "[H]";
        M2Config.btRedMsgFColor = 0xFF;
        M2Config.btRedMsgBColor = 0x38;

        var p = NewPlayer();
        p.m_IsSendUserLogon = true;
        p.SysMsgEx("plain", TMsgColor.c_Red, TMsgType.t_Hint, true);

        var (msg, text) = Assert.Single(_socket);
        Assert.Equal((ushort)Grobal2Const.SM_SYSMESSAGE, msg.Ident);
        Assert.Equal((ushort)0x38FF, msg.Param);
        Assert.Equal("plain", text);
    }

    [Fact]
    public void SysMsgEx_FColorBColorOverload_AndFallbackSeam()
    {
        var p = NewPlayer();
        p.m_IsSendUserLogon = true;
        p.SysMsgEx("k", 1, 2, TMsgType.t_Hint, true);
        Assert.Equal((ushort)0x0201, _socket[0].msg.Param);

        // 未登录 → 走 `inherited SysMsgEx` 接缝（原文 12988）
        var fallback = new List<string>();
        PlayerSurfaceCore6Seams.BaseSysMsgEx = (_, msg, _, _, _) => fallback.Add(msg);
        var p2 = NewPlayer();
        p2.m_IsSendUserLogon = false;
        p2.SysMsgEx("fb", TMsgColor.c_Green, TMsgType.t_Hint, true);
        Assert.Equal(new[] { "fb" }, fallback);
    }

    // =================================================================
    // 三、GetUserItemHandWeight（原文 13416-13434）
    // =================================================================

    [Fact]
    public void GetUserItemHandWeight_SumsOnlyHandSlots_ExcludingWhereSlot()
    {
        // 原文 13426：只累加 `I in [U_WEAPON, U_FASHIONWEAPON, U_SHIELD]` 且 `I <> nWhere`
        var p = NewPlayer();
        var weights = new Dictionary<int, byte>
        {
            [Grobal2Const.U_WEAPON] = 5,          // 1
            [Grobal2Const.U_FASHIONWEAPON] = 7,   // 19
            [Grobal2Const.U_SHIELD] = 3,          // 16
            [0] = 9,                              // 非腕力槽（衣服）—— 不计
        };
        PlayerSurfaceItemSeams.GetStdItem = idx => weights.TryGetValue(idx, out var w)
            ? StdItemNamed("item", weight: w)
            : (TStdItem?)null;

        p.m_UseItems[Grobal2Const.U_WEAPON] = UserItemNamed("w", wIndex: Grobal2Const.U_WEAPON);
        p.m_UseItems[Grobal2Const.U_FASHIONWEAPON] = UserItemNamed("fw", wIndex: Grobal2Const.U_FASHIONWEAPON);
        p.m_UseItems[Grobal2Const.U_SHIELD] = UserItemNamed("sh", wIndex: Grobal2Const.U_SHIELD);
        p.m_UseItems[0] = UserItemNamed("dress", wIndex: 0);

        // nWhere = U_WEAPON → 只算 时装武器(7) + 盾牌(3)
        Assert.Equal(10, p.GetUserItemHandWeight(Grobal2Const.U_WEAPON));
    }

    [Fact]
    public void GetUserItemHandWeight_WhereMinusOne_SumsAllHandSlotsIncludingWeapon()
    {
        // 边界：nWhere = -1 → `(nWhere = -1)` 短路，**全部**腕力槽都算
        var p = NewPlayer();
        var weights = new Dictionary<int, byte>
        {
            [Grobal2Const.U_WEAPON] = 5,
            [Grobal2Const.U_FASHIONWEAPON] = 7,
            [Grobal2Const.U_SHIELD] = 3,
        };
        PlayerSurfaceItemSeams.GetStdItem = idx => weights.TryGetValue(idx, out var w)
            ? StdItemNamed("item", weight: w)
            : (TStdItem?)null;

        p.m_UseItems[Grobal2Const.U_WEAPON] = UserItemNamed("w", wIndex: Grobal2Const.U_WEAPON);
        p.m_UseItems[Grobal2Const.U_FASHIONWEAPON] = UserItemNamed("fw", wIndex: Grobal2Const.U_FASHIONWEAPON);
        p.m_UseItems[Grobal2Const.U_SHIELD] = UserItemNamed("sh", wIndex: Grobal2Const.U_SHIELD);

        Assert.Equal(15, p.GetUserItemHandWeight(-1));
    }

    [Fact]
    public void GetUserItemHandWeight_EmptySlotsAndNullStdItem_ContributeZero()
    {
        // 边界：空槽（托管 null）按「全零记录」→ wIndex = 0；stdItem 为 null 时跳过
        var p = NewPlayer();
        SetStdItem(null);
        Assert.Equal(0, p.GetUserItemHandWeight(-1));

        p.m_UseItems[Grobal2Const.U_WEAPON] = UserItemNamed("w", wIndex: 1);
        SetStdItem(null);
        Assert.Equal(0, p.GetUserItemHandWeight(-1));
    }

    // =================================================================
    // 四、SendAddMagic（原文 13653-13675）
    // =================================================================

    private static int ClientMagicSize => CustomNpcUtils.StructToBytes(default(TClientMagic)).Length;

    [Fact]
    public void SendAddMagic_NormalPath_IdentSeriesAndTailSize()
    {
        // 原文 13673-13674：MakeDefaultMsg(SM_ADDMAGIC, 0, 0, 0, 1) + SendSocketEx(@ClientMagic, SizeOf(TClientMagic))
        var p = NewPlayer();
        PlayerSurfaceCore6Seams.UserMagicToClientMagic = _ => new TClientMagic { Key = 3, Level = 4 };
        p.SendAddMagic(new THumMagic { wMagIdx = 7, btKey = 3 });

        var (msg, buf) = Assert.Single(_socketEx);
        Assert.Equal((ushort)Grobal2Const.SM_ADDMAGIC, msg.Ident);
        Assert.Equal(0L, msg.Recog);
        Assert.Equal((ushort)0, msg.Param);
        Assert.Equal((ushort)0, msg.Tag);
        Assert.Equal((ushort)1, msg.Series);
        Assert.Equal(ClientMagicSize, buf.Length);
    }

    [Fact]
    public void SendAddMagic_Offline_EarlyExit()
    {
        // 原文 13658-13659
        var p = NewPlayer();
        p.m_boOffLine = true;
        p.SendAddMagic(new THumMagic { wMagIdx = 7 });
        Assert.Empty(_socketEx);
    }

    [Fact]
    public void SendAddMagic_CustomMagicFailMsgSeam_ConsultedOnlyWhenConfigRegistered()
    {
        // 原文 13666-13672：只有 `GetCustomMagicConfig(wMagIdx) <> nil` 时才去看 FailMsg
        const ushort magicId = 4321;
        var p = NewPlayer();
        PlayerSurfaceCore6Seams.UserMagicToClientMagic = _ => new TClientMagic();

        // 未注册配置 → 接缝**不被咨询**
        p.SendAddMagic(new THumMagic { wMagIdx = magicId });
        Assert.Empty(_customMagicQuery);

        // 注册配置 → 接缝被咨询一次
        var cfg = new TCustomMagicConfig(magicId);
        M2Config.RegisterCustomMagic(cfg);
        try
        {
            p.SendAddMagic(new THumMagic { wMagIdx = magicId });
            Assert.Equal(new[] { magicId.ToString() }, _customMagicQuery);
        }
        finally
        {
            M2Config.CustomMagicConfigs.Remove(magicId);
        }
    }

    // =================================================================
    // 五、InitSpeed（原文 13978-14001）
    // =================================================================

    [Fact]
    public void InitSpeed_CheckActionCountTrue_StampsThreeTicksAndClearsCounts()
    {
        // 原文 13982/13985/13988 是**三次独立** MyGetTickCount 调用
        M2Config.boCheckActionCount = true;
        _tickQueue.Enqueue(111u);
        _tickQueue.Enqueue(222u);
        _tickQueue.Enqueue(333u);

        var p = NewPlayer();
        p.m_nMoveCount = 9;
        p.m_nAttackCount = 8;
        p.m_nMagicAttackCount = 7;
        p.m_nCheckMoveCount = 6;
        p.m_nCheckAttackCount = 5;
        p.m_nCheckMagicAttackCount = 4;

        p.InitSpeed();

        Assert.Equal(111u, p.m_dwCheckMoveTick);
        Assert.Equal(222u, p.m_dwCheckAttackTick);
        Assert.Equal(333u, p.m_dwCheckMagicAttackTick);
        Assert.Equal(0, p.m_nMoveCount);
        Assert.Equal(0, p.m_nAttackCount);
        Assert.Equal(0, p.m_nMagicAttackCount);
        Assert.Equal(0, p.m_nCheckMoveCount);
        Assert.Equal(0, p.m_nCheckAttackCount);
        Assert.Equal(0, p.m_nCheckMagicAttackCount);
    }

    [Fact]
    public void InitSpeed_CheckActionCountFalse_ClearsCountsButLeavesTicks()
    {
        // 边界：else 分支**不碰** tick（原文 13994-13999）
        M2Config.boCheckActionCount = false;
        _tickQueue.Clear();

        var p = NewPlayer();
        p.m_dwCheckMoveTick = 11;
        p.m_dwCheckAttackTick = 22;
        p.m_dwCheckMagicAttackTick = 33;
        p.m_nMoveCount = 9;

        p.InitSpeed();

        Assert.Equal(11u, p.m_dwCheckMoveTick);
        Assert.Equal(22u, p.m_dwCheckAttackTick);
        Assert.Equal(33u, p.m_dwCheckMagicAttackTick);
        Assert.Equal(0, p.m_nMoveCount);
    }

    // =================================================================
    // 六、BaseObjectMove（原文 14003-14029）
    // =================================================================

    [Fact]
    public void BaseObjectMove_CoordinatesGiven_CallsSpaceMoveWithParsedInts()
    {
        // 原文 14018-14022：StrToIntDef + SpaceMove(sMAP, nX, nY, 0)
        var p = NewPlayer();
        p.BaseObjectMove("0", "100", "200");
        Assert.Equal(new[] { "0|100|200|0" }, _spaceMove);
        Assert.Empty(_mapRandomMove);
    }

    [Fact]
    public void BaseObjectMove_EmptyCoordinates_CallsMapRandomMove()
    {
        // 原文 14025：坐标不全（任一为空）→ MapRandomMove(sMAP, 0)
        var p = NewPlayer();
        p.BaseObjectMove("0", "", "200");
        Assert.Equal(new[] { "0|0" }, _mapRandomMove);
        Assert.Empty(_spaceMove);
    }

    [Fact]
    public void BaseObjectMove_EmptyMap_UsesCurrentMapName()
    {
        // 原文 14015-14016：sMAP = '' → sMAP := m_sMapName
        var p = NewPlayer();
        p.m_sMapName = "3";
        p.BaseObjectMove("", "1", "2");
        Assert.Equal(new[] { "3|1|2|0" }, _spaceMove);
    }

    [Fact]
    public void BaseObjectMove_BadCoordinates_StrToIntDefGivesZero()
    {
        // 边界：Delphi `StrToIntDef(sX, 0)` —— 非数字 → 0
        var p = NewPlayer();
        p.BaseObjectMove("0", "abc", "xyz");
        Assert.Equal(new[] { "0|0|0|0" }, _spaceMove);
    }

    [Fact]
    public void BaseObjectMove_Imprison_SendsForbiddenMsgAndDoesNotMove()
    {
        // 原文 14008-14012：m_boImprison → SysMsg('禁止使用此命令！', c_Red, t_Hint) + Exit
        var p = NewPlayer();
        p.m_boImprison = true;
        p.BaseObjectMove("0", "1", "2");

        Assert.Empty(_spaceMove);
        Assert.Empty(_mapRandomMove);
        // m_IsSendUserLogon 默认 False → 走基类落点（原文 12912 的 inherited）
        Assert.Contains("禁止使用此命令！", p.SysMsgs);
    }

    [Fact]
    public void BaseObjectMove_MapChangedAndPlayer_ClearsTimeRecall_AndBoundaryForNonPlayer()
    {
        // 原文 14027-14028：换了地图（指针比较）且 m_btRaceServer = RC_PLAYOBJECT → m_boTimeRecall := False
        var p = NewPlayer();
        p.m_boTimeRecall = true;
        var oldEnvir = new TEnvirnoment();
        var newEnvir = new TEnvirnoment();
        p.m_PEnvir = oldEnvir;
        PlayerSurfaceCore6Seams.SpaceMove = (self, _, _, _, _) => self.m_PEnvir = newEnvir;

        p.BaseObjectMove("0", "1", "2");
        Assert.False(p.m_boTimeRecall);

        // 边界：非玩家种族 → 保持 True
        var mon = new TPlayObject { m_btRaceServer = Grobal2Const.RC_MONSTER };
        mon.m_boTimeRecall = true;
        mon.m_PEnvir = oldEnvir;
        PlayerSurfaceCore6Seams.SpaceMove = (self, _, _, _, _) => self.m_PEnvir = newEnvir;
        mon.BaseObjectMove("0", "1", "2");
        Assert.True(mon.m_boTimeRecall);
    }

    // =================================================================
    // 七、交易 / 挑战报文族（原文 14262-14371）
    // =================================================================

    [Fact]
    public void SendDelDealItem_RemoteDelGoesToSelf_OriginalDefect()
    {
        // ★ 原文如此 ②（锁定缺陷）：原文 14267 先发 `SM_DEALDELITEM_OK` 给自己，
        //   14281 的 `SM_DEALREMOTEDELITEM` **也发给自己**（`SendDefMessage` 是 Self 的方法调用），
        //   而对称的 SendAddDealItem（14302）却发给 `m_DealCreat` —— 删除通知对方看不到。照抄，不修。
        var p = NewPlayer();
        var partner = new TPlayObject();
        p.m_DealCreat = partner;
        SetStdItem(StdItemNamed("屠龙"));
        _tickQueue.Enqueue(100u);
        _tickQueue.Enqueue(200u);

        p.SendDelDealItem(UserItemNamed("", wIndex: 5, makeIndex: 66));

        Assert.Equal(2, _defMsg.Count);
        Assert.Same(p, _defMsg[0].target);
        Assert.Equal((ushort)Grobal2Const.SM_DEALDELITEM_OK, _defMsg[0].ident);
        // ★ 缺陷：第二条也发给**自己**，而不是 partner
        Assert.Same(p, _defMsg[1].target);
        Assert.NotSame(partner, _defMsg[1].target);
        Assert.Equal((ushort)Grobal2Const.SM_DEALREMOTEDELITEM, _defMsg[1].ident);
        Assert.Equal(66L, _defMsg[1].recog);
        Assert.Equal("屠龙", _defMsg[1].text);
        // 14284-14285：两个 LastTick 都刷新
        Assert.Equal(100u, partner.m_DealLastTick);
        Assert.Equal(200u, p.m_DealLastTick);
    }

    [Fact]
    public void SendDelDealItem_NoPartner_OnlySendsOk()
    {
        // 边界：m_DealCreat = nil → 只发第一条（原文 14268 的守卫）
        var p = NewPlayer();
        p.SendDelDealItem(UserItemNamed("x", wIndex: 5, makeIndex: 1));
        var only = Assert.Single(_defMsg);
        Assert.Equal((ushort)Grobal2Const.SM_DEALDELITEM_OK, only.ident);
    }

    [Fact]
    public void SendDelDealItem_CustomNameWhenBtValue13IsOne_Boundary()
    {
        // 原文 14273：`(UserItem.btValue[13] = 1) and (UserItem.Name <> '')` 时用**自定义名**
        SetStdItem(StdItemNamed("标准名"));

        var p1 = NewPlayer();
        p1.m_DealCreat = new TPlayObject();
        var it1 = UserItemNamed("自定义名", wIndex: 5, makeIndex: 1);
        it1.SetBtValue(13, 1);
        p1.SendDelDealItem(it1);
        Assert.Equal("自定义名", _defMsg[1].text);

        _defMsg.Clear();
        var p2 = NewPlayer();
        p2.m_DealCreat = new TPlayObject();
        var it2 = UserItemNamed("自定义名", wIndex: 5, makeIndex: 1);   // btValue[13] 保持 0
        p2.SendDelDealItem(it2);
        Assert.Equal("标准名", _defMsg[1].text);
    }

    [Fact]
    public void SendAddDealItem_SendsToPartner_WithSelfAsRecog()
    {
        // 原文 14301-14302：报文装配在自己身上（nRecog = NativeInt(Self)），由**对方**投递
        var p = NewPlayer();
        var partner = new TPlayObject();
        p.m_DealCreat = partner;
        SetStdItem(StdItemNamed("物品"));
        PlayerSurfaceItemSeams.UserItemToClientItem = (_, _) => "CLIENT-ITEM";
        _tickQueue.Enqueue(11u);
        _tickQueue.Enqueue(22u);

        p.SendAddDealItem(UserItemNamed("", wIndex: 5, makeIndex: 9));

        Assert.Equal((ushort)Grobal2Const.SM_DEALADDITEM_OK, Assert.Single(_defMsg).ident);
        var send = Assert.Single(_socketExTo);
        Assert.Same(partner, send.target);                    // 对方投递
        Assert.Equal((ushort)Grobal2Const.SM_DEALREMOTEADDITEM, send.msg.Ident);
        Assert.Equal(p.m_nRecogId, send.msg.Recog);           // NativeInt(Self)
        Assert.Equal((ushort)1, send.msg.Series);
        Assert.Equal("CLIENT-ITEM", send.buf);
        Assert.Equal(11u, partner.m_DealLastTick);
        Assert.Equal(22u, p.m_DealLastTick);
    }

    [Fact]
    public void OpenDealDlg_SetsState_CallsSeam_SendsMenuWithPartnerName()
    {
        // 原文 14311-14315
        _tickQueue.Enqueue(777u);
        var p = NewPlayer();
        var partner = new TPlayObject { m_sCharName = "对方" };
        var seamCalls = new List<TPlayObject>();
        PlayerSurfaceCore6Seams.GetBackDealItems = self => seamCalls.Add(self);

        p.OpenDealDlg(partner);

        Assert.True(p.m_boDealing);
        Assert.Same(partner, p.m_DealCreat);
        Assert.Same(p, Assert.Single(seamCalls));
        var sent = Assert.Single(_defMsg);
        Assert.Equal((ushort)Grobal2Const.SM_DEALMENU, sent.ident);
        Assert.Equal("对方", sent.text);
        Assert.Equal(777u, p.m_DealLastTick);
    }

    [Fact]
    public void OpenChallengeDlg_SetsStateAndPutsGoldIndexInParam()
    {
        // 原文 14321-14327：wParam = g_Config.btChallengeGoldIndex（挑战附加币控制）
        M2Config.btChallengeGoldIndex = 7;
        _tickQueue.Enqueue(888u);
        var p = NewPlayer();
        var partner = new TPlayObject { m_sCharName = "挑战者" };
        var seamCalls = new List<TPlayObject>();
        PlayerSurfaceCore6Seams.GetBackChallengeItems = self => seamCalls.Add(self);

        p.OpenChallengeDlg(partner);

        Assert.True(p.m_boChallengeing);
        Assert.Same(partner, p.m_ChallengeCreat);
        Assert.Same(p, Assert.Single(seamCalls));
        var sent = Assert.Single(_defMsg);
        Assert.Equal((ushort)Grobal2Const.SM_CHALLENGEMENU, sent.ident);
        Assert.Equal((ushort)7, sent.p);
        Assert.Equal("挑战者", sent.text);
        Assert.Equal(888u, p.m_ChallengeLastTick);
    }

    [Fact]
    public void SendDelChallengeItem_RemoteDelGoesToPartner_ContrastWithDealDefect()
    {
        // 对照 ②：挑战版原文 14345 **确实**把 SM_CHALLENGEREMOTEDELITEM 发给对方
        var p = NewPlayer();
        var partner = new TPlayObject();
        p.m_ChallengeCreat = partner;
        SetStdItem(StdItemNamed("标准名"));
        _tickQueue.Enqueue(31u);
        _tickQueue.Enqueue(32u);

        p.SendDelChallengeItem(UserItemNamed("", wIndex: 5, makeIndex: 42));

        Assert.Equal(2, _defMsg.Count);
        Assert.Same(p, _defMsg[0].target);
        Assert.Equal((ushort)Grobal2Const.SM_CHALLENGEDELITEM_OK, _defMsg[0].ident);
        Assert.Same(partner, _defMsg[1].target);      // ← 发给对方（与交易版不同）
        Assert.Equal((ushort)Grobal2Const.SM_CHALLENGEREMOTEDELITEM, _defMsg[1].ident);
        Assert.Equal(42L, _defMsg[1].recog);
        Assert.Equal(31u, partner.m_ChallengeLastTick);
        Assert.Equal(32u, p.m_ChallengeLastTick);
    }

    [Fact]
    public void SendAddChallengeItem_NormalPath_SendsToPartner()
    {
        var p = NewPlayer();
        var partner = new TPlayObject();
        p.m_ChallengeCreat = partner;
        SetStdItem(StdItemNamed("物品"));
        PlayerSurfaceItemSeams.UserItemToClientItem = (_, _) => "CH-CLIENT";
        _tickQueue.Enqueue(41u);
        _tickQueue.Enqueue(42u);

        p.SendAddChallengeItem(UserItemNamed("", wIndex: 5, makeIndex: 8));

        Assert.Equal((ushort)Grobal2Const.SM_CHALLENGEADDITEM_OK, Assert.Single(_defMsg).ident);
        var send = Assert.Single(_socketExTo);
        Assert.Same(partner, send.target);
        Assert.Equal((ushort)Grobal2Const.SM_CHALLENGEREMOTEADDITEM, send.msg.Ident);
        Assert.Equal(p.m_nRecogId, send.msg.Recog);
        Assert.Equal("CH-CLIENT", send.buf);
    }

    [Fact]
    public void SendAddDealItem_NoPartnerOrNoStdItem_DoesNotSendRemote()
    {
        // 边界：m_DealCreat = nil（原文 14295）或 StdItem = nil（14298）→ 不发远程包
        var p = NewPlayer();
        SetStdItem(StdItemNamed("物品"));
        p.SendAddDealItem(UserItemNamed("", wIndex: 5, makeIndex: 1));
        Assert.Empty(_socketExTo);

        var p2 = NewPlayer();
        p2.m_DealCreat = new TPlayObject();
        SetStdItem(null);
        p2.SendAddDealItem(UserItemNamed("", wIndex: 5, makeIndex: 1));
        Assert.Empty(_socketExTo);
    }

    // =================================================================
    // 八、未移植留痕台账（PortNotPorted）
    // =================================================================

    [Fact]
    public void NotPortedMethods_SixteenAreRecordedWithOriginalLines()
    {
        var p = NewPlayer();
        var stdItem = new TStdItem();

        Assert.False(p.CheckTakeOnItems(0, stdItem));
        Assert.Equal(0, p.GetUserItemWeitht(-1));
        Assert.False(p.EatItems(stdItem));
        Assert.False(p.ReadBook(stdItem));
        p.SendDelMagic(new THumMagic());
        Assert.False(p.EatUseItems(1));
        p.MoveToHome();
        p.MoveRandomToHome();
        Assert.False(p.WeaptonMakeLuck());
        Assert.False(p.RepairWeapon());
        Assert.False(p.SuperRepairWeapon());
        Assert.False(p.WinLottery());
        p.JoinGroup(new TPlayObject());
        p.MakeMine();
        Assert.False(p.QuestTakeCheckItem(new TUserItem()));
        p.MakeSaveRcd(default);

        Assert.Equal(16, PlayerSurfacePortLedger.NotPortedCount);
        var expected = new[]
        {
            "CheckTakeOnItems (ObjPlayer.pas:12991)",
            "GetUserItemWeitht (ObjPlayer.pas:13375)",
            "EatItems (ObjPlayer.pas:13437)",
            "ReadBook (ObjPlayer.pas:13600)",
            "SendDelMagic (ObjPlayer.pas:13684)",
            "EatUseItems (ObjPlayer.pas:13777)",
            "MoveToHome (ObjPlayer.pas:13963)",
            "MoveRandomToHome (ObjPlayer.pas:13974)",
            "WeaptonMakeLuck (ObjPlayer.pas:14038)",
            "RepairWeapon (ObjPlayer.pas:14102)",
            "SuperRepairWeapon (ObjPlayer.pas:14141)",
            "WinLottery (ObjPlayer.pas:14173)",
            "JoinGroup (ObjPlayer.pas:14374)",
            "MakeMine (ObjPlayer.pas:14392)",
            "QuestTakeCheckItem (ObjPlayer.pas:14503)",
            "MakeSaveRcd (ObjPlayer.pas:14544)",
        };
        foreach (var e in expected)
            Assert.Contains(e, PlayerSurfacePortLedger.NotPortedMethods);
    }

    [Fact]
    public void NotPorted_DoesNotSendAnythingOrMutateState()
    {
        // 留痕方法一律「无副作用」：不发明行为、不发报文
        var p = NewPlayer();
        p.m_IsSendUserLogon = true;
        p.MoveToHome();
        p.MoveRandomToHome();
        p.MakeMine();
        p.MakeSaveRcd(default);
        Assert.Empty(_socket);
        Assert.Empty(_socketEx);
        Assert.Empty(_defMsg);
        Assert.Empty(_spaceMove);
        Assert.Empty(_mapRandomMove);
    }
}
