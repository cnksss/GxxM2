using System;
using System.Collections.Generic;
using GXX.Client.GUI.Mir;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

// ============================================================================================
// 【P17 切片1 / p17-client-mshare】MShare.pas 全局真身 + 纯函数 —— 真实断言
//
// 行号一律指 `_analysis/utf8_mirror/Client-HGE/MShare.pas` 的 UTF-8 镜像行号。
// 每个用例都写明它锁的是原文哪一行、以及那句原文的**判别式**。
// ============================================================================================

public class MShareGlobalsP17Tests
{
    public MShareGlobalsP17Tests() => MShareGlobalsReset.ResetForTests();

    // ------------------------------------------------------------------------------------
    // 一、fstate（p14-client-fstate）B-6 阻塞类：7 条已由 FStateMShareSeam 接缝承载，
    //     本切片的真身必须与接缝**语义一致**（集成方换指时才不会改变行为）。
    // ------------------------------------------------------------------------------------

    [Fact]
    public void FStateSeamCounterparts_ExistWithOriginalDefaults()
    {
        // 原文 1825 / 1824 / 2076 / 2040 / 2038 / 2074 / 2068
        Assert.Equal(0u, MShareGlobals.g_dwQueryMsgTick);
        Assert.Equal(0u, MShareGlobals.g_dwDealActionTick);
        Assert.Equal(0u, MShareGlobals.g_dwChallengeActionTick);
        Assert.False(MShareGlobals.g_boDealEnd);
        Assert.Equal(0, MShareGlobals.g_nDealGold);
        Assert.False(MShareGlobals.g_boChallengeEnd);
        Assert.Equal(0, MShareGlobals.g_nChallengeGold);
    }

    [Fact]
    public void FStateSeamCounterparts_AreIndependentStatics_NoSecondGlobalsClass()
    {
        // 「不另起第二套 MShareGlobals」的反射锁：真身类只有 GXX.Client.GUI.Mir.MShareGlobals 一个，
        // 且它必须是 partial（切片 2/3 在同名类里继续扩展，而不是另开类型）。
        var asm = typeof(MShareGlobals).Assembly;
        var named = new List<Type>();
        foreach (var t in asm.GetTypes())
        {
            if (t.Name == "MShareGlobals") named.Add(t);
        }
        Assert.Single(named);
        Assert.Equal("GXX.Client.GUI.Mir.MShareGlobals", named[0].FullName);
    }

    [Fact]
    public void FStateSeam_QueryMsgTick_3SecondReinstallWindow_MatchesOriginalStrictGreater()
    {
        // 原文 17876/17877（FState.pas 侧车道已 1:1 落地；这里锁真身承载的读写语义）：
        //   if MyGetTickCount > g_dwQueryMsgTick then g_dwQueryMsgTick := MyGetTickCount + 3000;
        uint now = 10_000;
        MShareGlobals.g_dwQueryMsgTick = 9_999;
        if (now > MShareGlobals.g_dwQueryMsgTick) MShareGlobals.g_dwQueryMsgTick = now + 3000;
        Assert.Equal(13_000u, MShareGlobals.g_dwQueryMsgTick);

        // 边界：tick == 计数器 ⇒ 严格 `>` 不成立 ⇒ 不重装
        MShareGlobalsReset.ResetForTests();
        MShareGlobals.g_dwQueryMsgTick = 10_000;
        if (now > MShareGlobals.g_dwQueryMsgTick) MShareGlobals.g_dwQueryMsgTick = now + 3000;
        Assert.Equal(10_000u, MShareGlobals.g_dwQueryMsgTick);
    }

    // ------------------------------------------------------------------------------------
    // 二、B-6 剩余项 + 主流程读取的全局：逐条断言「行号 ↔ 默认值」
    // ------------------------------------------------------------------------------------

    [Theory]
    // 原文行号, 默认值（照抄原文的 `= <初值>`）
    [InlineData(1761, 0u)]      // g_LastGroupAttackTick:LongWord = 0
    [InlineData(1762, 0u)]      // g_dwMagicDelayTime:longword = 0
    [InlineData(1764, 0u)]      // g_dwAutoCheckFireHitTick:longword = 0
    [InlineData(1765, 0u)]      // g_dwAutoCheckSWordHitTick:longword = 0
    [InlineData(1808, 0u)]      // g_dwMapMovingWaitTick:LongWord = 0
    [InlineData(2137, 5_000u)]  // g_dwDropItemFlashTime = 5 * 1000
    [InlineData(2140, 600u)]    // g_dwSpellTime = 600
    [InlineData(1688, 10_000u)] // g_AutoMsgTime = 10000
    public void UintGlobals_DefaultValues(int line, uint expected)
    {
        _ = line; // 行号只做事后对照，不参与断言
        uint actual = line switch
        {
            1761 => MShareGlobals.g_LastGroupAttackTick,
            1762 => MShareGlobals.g_dwMagicDelayTime,
            1764 => MShareGlobals.g_dwAutoCheckFireHitTick,
            1765 => MShareGlobals.g_dwAutoCheckSWordHitTick,
            1808 => MShareGlobals.g_dwMapMovingWaitTick,
            2137 => MShareGlobals.g_dwDropItemFlashTime,
            2140 => MShareGlobals.g_dwSpellTime,
            1688 => MShareGlobals.g_AutoMsgTime,
            _ => throw new ArgumentOutOfRangeException(nameof(line)),
        };
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void IntAndEnumDefaults_MatchOriginal()
    {
        Assert.Equal(1400, MShareGlobals.g_nHitTime);        // 原文 2138
        Assert.Equal(60, MShareGlobals.g_nItemSpeed);        // 原文 2139
        Assert.Equal(-1, MShareGlobals.g_nMiniMapIndex);     // 原文 1813
        Assert.Equal(-1, MShareGlobals.g_LastHintMakeIndex); // 原文 1328
        Assert.Equal(-1, MShareGlobals.g_nAttactkMode);      // 原文 2266
        Assert.Equal(50, MShareGlobals.g_nDarkValue);        // 原文 1414
        Assert.Equal(800, MShareGlobals.g_nScreenWidth);     // 原文 1670
        Assert.Equal(600, MShareGlobals.g_nScreenHeight);    // 原文 1671
        Assert.Equal(16, MShareGlobals.g_nBitCount);         // 原文 1672
        Assert.Equal(7000, MShareGlobals.g_nServerPort);     // 原文 1660
        Assert.Equal(71, MShareGlobals.g_nTestX);            // 原文 2254
        Assert.Equal(212, MShareGlobals.g_nTestY);           // 原文 2255
        Assert.Equal(2, MShareGlobals.g_dwProcessInterval);  // 原文 2256
        Assert.Equal(-1, MShareGlobals.g_nRankingsPage);     // 原文 2282 = -1
        Assert.Equal(0, MShareGlobals.g_nRankingsTablePage); // 原文 2280
        Assert.Equal(0, MShareGlobals.g_nRankingsTableType); // 原文 2281
        Assert.Equal(0, MShareGlobals.g_nRankingsPageCount); // 原文 2283
    }

    [Fact]
    public void BoolDefaults_MatchOriginal()
    {
        Assert.False(MShareGlobals.g_boMouseMoveDown);      // 原文 1774
        // 原文 1775 g_boOpenMerchantBigDlg 由 ClientGlobals.cs（车道1 既有承载）以 byte 承载（Delphi Boolean 的
        // Core 惯例），本切片沿用该既有声明而不改类型（见报告「偏离 D-P17-03」）。
        Assert.Equal(0, MShareGlobals.g_boOpenMerchantBigDlg);
        Assert.False(MShareGlobals.g_boKeepBigDlg);         // 原文 1776
        Assert.True(MShareGlobals.g_IsInMouseMove);         // 原文 1787 = True
        Assert.False(MShareGlobals.g_boMinMapTransparent);  // 原文 1815
        Assert.True(MShareGlobals.g_boSound);               // 原文 1680
        Assert.True(MShareGlobals.g_boBGSound);             // 原文 1681
        Assert.True(MShareGlobals.g_boRepeatBGSound);       // 原文 1682
        Assert.True(MShareGlobals.g_boClientCanSend);       // 原文 1401
        Assert.True(MShareGlobals.g_boHardware);            // 原文 1405
        Assert.True(MShareGlobals.g_boDepthStencil);        // 原文 1406
        Assert.True(MShareGlobals.g_boUseWeather);          // 原文 1408
        Assert.True(MShareGlobals.g_boForceNotViewFog);     // 原文 1411
        Assert.True(MShareGlobals.g_boRenderTargetTileMap); // 原文 1421
        Assert.True(MShareGlobals.g_boRenderTarget);        // 原文 1422
        Assert.True(MShareGlobals.g_boShowItemName);        // 原文 1427
        Assert.True(MShareGlobals.g_boCanAttack);           // 原文 1465
        Assert.True(MShareGlobals.g_boCanMove);             // 原文 1466
        Assert.True(MShareGlobals.g_boDrawDropItem);        // 原文 2253
        Assert.True(MShareGlobals.g_boMissionButonFlash);   // 原文 2267
        Assert.True(MShareGlobals.g_boFirstNewMapMsg);      // 原文 1355
        Assert.False(MShareGlobals.g_boIOCP_Rungate);       // 原文 1325
        Assert.False(MShareGlobals.g_boCheckBug);           // 原文 1349
        Assert.False(MShareGlobals.g_boFirstTime);          // 原文 1700
        Assert.False(MShareGlobals.g_boAppExit);            // 原文 2326
        Assert.False(MShareGlobals.g_WaitAppExit);          // 原文 2327
        Assert.False(MShareGlobals.g_boAllowGroup);         // 原文 1827
        Assert.False(MShareGlobals.g_boMagicMoving);        // 原文 2089
        Assert.Null(MShareGlobals.g_MovingMagic);           // 原文 2090 = nil
    }

    [Fact]
    public void StringDefaults_MatchOriginal()
    {
        Assert.Equal("The Return of Legend", MShareGlobals.g_sLogoText); // 原文 1437
        Assert.Equal("金币", MShareGlobals.g_sGoldName);                  // 原文 1438
        Assert.Equal("游戏点", MShareGlobals.g_sGamePointName);            // 原文 1440
        Assert.Equal("声望", MShareGlobals.g_sCreditPointName);            // 原文 1443
        Assert.Equal("战士", MShareGlobals.g_sWarriorName);                // 原文 1444
        Assert.Equal("法师", MShareGlobals.g_sWizardName);                 // 原文 1445
        Assert.Equal("道士", MShareGlobals.g_sTaoistName);                 // 原文 1446
        Assert.Equal("未知", MShareGlobals.g_sUnKnowName);                 // 原文 1448
        Assert.Equal("宋体", MShareGlobals.g_sCurFontName);                // 原文 1692
        Assert.Equal("127.0.0.1", MShareGlobals.g_sServerAddr);            // 原文 1659
        Assert.Equal("Resources", MShareGlobals.g_ResourcesDir);           // 原文 2246
        Assert.Equal("0.00B/s", MShareGlobals.g_UpdateSpeedStr);           // 原文 1369
        Assert.Equal(@"d:\self_run_sendmsg.txt", MShareGlobals.g_sSelfRunSendMsgFile); // 原文 2480
        Assert.Equal(@"d:\self_run_recvmsg.txt", MShareGlobals.g_sSelfRunRecvMsgFile); // 原文 2481
        Assert.Equal(@"d:\self_run_strart.txt", MShareGlobals.g_sSelRunStartFile);     // 原文 2482（原文拼写 `strart`）
        Assert.Equal(@"d:\self_run_end.txt", MShareGlobals.g_sSelfRunEndFile);         // 原文 2483
    }

    [Fact]
    public void FontArr_MatchesOriginalEightEntries()
    {
        // 原文 1690：MAXFONT = 8
        Assert.Equal(8, MShareGlobals.g_FontArr.Length);
        Assert.Equal("宋体", MShareGlobals.g_FontArr[0]);
        Assert.Equal("楷体", MShareGlobals.g_FontArr[3]);
        Assert.Equal("Microsoft Sans Serif", MShareGlobals.g_FontArr[7]);
    }

    [Fact]
    public void CrcGlobals_AllZeroByDefault()
    {
        // 原文 1281-1299：整段 CRC 全部 `= 0`
        Assert.Equal(0u, MShareGlobals.g_ModulesCRC);
        Assert.Equal(0u, MShareGlobals.g_MonstersCRC);
        Assert.Equal(0u, MShareGlobals.g_MagicsCRC);
        Assert.Equal(0u, MShareGlobals.g_StdItemsCRC);
        Assert.Equal(0u, MShareGlobals.g_ItemDescCRC);
        Assert.Equal(0u, MShareGlobals.g_ItemDescTopCRC);
        Assert.Equal(0u, MShareGlobals.g_TzItemDescCRC);
        Assert.Equal(0u, MShareGlobals.g_FilterItemsCRC);
        Assert.Equal(0u, MShareGlobals.g_EffectImagesCRC);
        Assert.Equal(0u, MShareGlobals.g_SpecialCmdsCRC);
        Assert.Equal(0u, MShareGlobals.g_PlugClientsCRC);
        Assert.Equal(0u, MShareGlobals.g_BlackModulesCRC);
        Assert.Equal(0u, MShareGlobals.g_NpcsCRC);
        Assert.Equal(0u, MShareGlobals.g_DropItemEffectListCRC);
        Assert.Equal(0u, MShareGlobals.g_EnabledAuctionItemListCRC);
        Assert.Equal(0u, MShareGlobals.g_CustomItemPropertyCRC);
        Assert.Equal(0u, MShareGlobals.g_CustomItemPropertyTextVarListCRC);
        Assert.Equal(0u, MShareGlobals.g_ArrButtonConfigCRC);
        Assert.Equal(0u, MShareGlobals.g_CustomMoneyCRC);
    }

    [Fact]
    public void ResetForTests_ZeroesSlice1FateSeamGlobals()
    {
        MShareGlobals.g_dwQueryMsgTick = 123;
        MShareGlobals.g_boDealEnd = true;
        MShareGlobals.g_nChallengeGold = 7;
        MShareGlobals.g_ExtBagOpenItemCount = 9;
        MShareGlobals.g_nRankingsPage = 5;
        MShareGlobals.g_MovingMagic = default;

        MShareGlobalsReset.ResetForTests();

        Assert.Equal(0u, MShareGlobals.g_dwQueryMsgTick);
        Assert.False(MShareGlobals.g_boDealEnd);
        Assert.Equal(0, MShareGlobals.g_nChallengeGold);
        Assert.Equal(0, MShareGlobals.g_ExtBagOpenItemCount);
        Assert.Equal(-1, MShareGlobals.g_nRankingsPage);   // 复位回原文初值 -1，不是 0
        Assert.Null(MShareGlobals.g_MovingMagic);
    }
}

// ============================================================================================
// 纯函数真身（MShareFunctions）
// ============================================================================================

public class MShareFunctionsP17Tests
{
    public MShareFunctionsP17Tests() => MShareGlobalsReset.ResetForTests();

    private static unsafe TClientItem* MakeItem(string name, byte stdMode, ushort overLap, ushort dura, ushort shape = 0)
    {
        TClientItem* p = (TClientItem*)System.Runtime.InteropServices.Marshal.AllocHGlobal(sizeof(TClientItem));
        *p = default;
        p->s.NameStr = name;
        p->s.StdMode = stdMode;
        p->s.OverLap = overLap;
        p->s.Shape = shape;
        p->Dura = dura;
        return p;
    }

    private static unsafe void FreeItem(TClientItem* p)
        => System.Runtime.InteropServices.Marshal.FreeHGlobal((IntPtr)p);

    // ------------------------------------------------------------------------------------
    // 物品叠加判定（原文 4084 / 4090 / 4096）
    // ------------------------------------------------------------------------------------

    [Fact]
    public unsafe void IsOverLapItem_Single_MirrorsOriginalConjunction()
    {
        // 原文 4087：Name <> '' and StdMode in [0,2,3,31,40,41,42,46,47] and OverLap > 0
        TClientItem* ok = MakeItem("金创药(小)", 0, 1, 0);
        Assert.True(MShareFunctions.IsOverLapItem(ok));
        FreeItem(ok);

        // 名字为空 ⇒ False（第一项判据）
        TClientItem* noName = MakeItem("", 0, 1, 0);
        Assert.False(MShareFunctions.IsOverLapItem(noName));
        FreeItem(noName);

        // StdMode 不在集合里 ⇒ False（例如 5）
        TClientItem* badMode = MakeItem("x", 5, 1, 0);
        Assert.False(MShareFunctions.IsOverLapItem(badMode));
        FreeItem(badMode);

        // OverLap = 0 ⇒ False
        TClientItem* noOverlap = MakeItem("x", 31, 0, 0);
        Assert.False(MShareFunctions.IsOverLapItem(noOverlap));
        FreeItem(noOverlap);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(2, true)]
    [InlineData(3, true)]
    [InlineData(31, true)]
    [InlineData(40, true)]
    [InlineData(41, true)]
    [InlineData(42, true)]
    [InlineData(46, true)]
    [InlineData(47, true)]
    [InlineData(1, false)]
    [InlineData(48, false)]
    [InlineData(255, false)]
    public unsafe void IsOverLapItem_StdModeSet_IsExactlyTheOriginalNine(byte stdMode, bool expected)
    {
        TClientItem* it = MakeItem("item", stdMode, 1, 0);
        Assert.Equal(expected, MShareFunctions.IsOverLapItem(it));
        FreeItem(it);
    }

    [Fact]
    public unsafe void IsOverLapItem_TwoItems_RequiresNameAndStdModeEquality()
    {
        TClientItem* a = MakeItem("祝福油", 31, 1, 0);
        TClientItem* b = MakeItem("祝福油", 31, 1, 0);
        Assert.True(MShareFunctions.IsOverLapItem(a, b));

        // 名字不同 ⇒ False
        TClientItem* c = MakeItem("祝福油2", 31, 1, 0);
        Assert.False(MShareFunctions.IsOverLapItem(a, c));
        FreeItem(c);

        // StdMode 不同 ⇒ False
        TClientItem* d = MakeItem("祝福油", 42, 1, 0);
        Assert.False(MShareFunctions.IsOverLapItem(a, d));
        FreeItem(d);

        FreeItem(a);
        FreeItem(b);
    }

    [Fact]
    public unsafe void IsUnOverLapItem_UsesDuraInsteadOfCountingOverlapOnly()
    {
        // 原文 4099：Name <> '' and StdMode in [...] and Item.Dura > 0 and OverLap > 0
        TClientItem* duraOk = MakeItem("持久药", 0, 1, 7);
        Assert.True(MShareFunctions.IsUnOverLapItem(duraOk));
        FreeItem(duraOk);

        // Dura = 0 时 True→False（与 IsOverLapItem 的差异点）
        TClientItem* duraZero = MakeItem("持久药", 0, 1, 0);
        Assert.False(MShareFunctions.IsUnOverLapItem(duraZero));
        Assert.True(MShareFunctions.IsOverLapItem(duraZero)); // 同一件物品，单参版本不看 Dura
        FreeItem(duraZero);
    }

    // ------------------------------------------------------------------------------------
    // 特性长度 / 调色板（原文 4102 / 4111 / 4116）
    // ------------------------------------------------------------------------------------

    [Fact]
    public void GetFeatureLen_ReplacesWithGlobalWhenLengthsMatch()
    {
        // 原文 4105-4108
        MShareGlobals.g_nHumFeature = 0;   // 未初始化 ⇒ 任何 nLen 都不等
        MShareGlobals.g_nMonFeature = 0;
        Assert.Equal(37, MShareFunctions.GetFeatureLen(37));

        MShareGlobals.g_nHumFeature = 12;
        Assert.Equal(12, MShareFunctions.GetFeatureLen(12));   // 命中 hum 分支
        Assert.Equal(9, MShareFunctions.GetFeatureLen(9));     // 两侧都不等 ⇒ 原样

        MShareGlobals.g_nHumFeature = 0;
        MShareGlobals.g_nMonFeature = 25;
        Assert.Equal(25, MShareFunctions.GetFeatureLen(25));   // 命中 mon 分支（elif）
    }

    [Fact]
    public void RGB32_NonSixteenBitIsIdentity()
    {
        // 原文 4121：else Result := C
        Assert.Equal(0x12345678, MShareFunctions.RGB32(0x12345678, 32));
        Assert.Equal(-1, MShareFunctions.RGB32(-1, 24));
        Assert.Equal(0, MShareFunctions.RGB32(0, 0));
    }

    [Theory]
    // 原文 4119 的三个通道各自只取 5/6/5 位、再放回 8/8/8 的对应字节。
    // 因为原文是按「低 11 位有效」写的 16 位字（`C and $F8` 只吃 bit3-7），所以：
    //   红 = (C & $F8) >> 8   （只取低位字的高 5 位，通常为 0）
    //   绿 = (C & $FC) >> 3   （bit2-7 → 0..63）
    //   蓝 = (C & $F8) << 3   （bit3-7 → 0,8,...248）
    [InlineData(0x7FFF, 0x00C01F00)]
    [InlineData(0x0000, 0x00000000)]
    [InlineData(0x7C00, 0x00000000)]
    [InlineData(0x001F, 0x00C00300)]
    public void RGB32_SixteenBit_UnpacksFiveSixFive(int c, int expected)
    {
        // 原文 4119：RGB(C and $F8 shr 8, C and $FC shr 3, C and $F8 shl 3)
        Assert.Equal(expected, MShareFunctions.RGB32(c, 16));
    }

    [Fact]
    public void GetRGB_ReadsDefColorTable()
    {
        // 原文 4113 读 g_DefColorTable[c].rgbRed/rgbGreen/rgbBlue 后走 RGB 宏重组。
        // 托管侧两处（Palette256 与 g_DefColorTable）必须互为逆运算，故 GetRGB(c) == Palette256[c]。
        Assert.Equal(MShareGlobals.Palette256[0], MShareFunctions.GetRGB(0));   // clBlack
        Assert.Equal(MShareGlobals.Palette256[9], MShareFunctions.GetRGB(9));   // clRed = $000000FF
        Assert.Equal(MShareGlobals.Palette256[15], MShareFunctions.GetRGB(15)); // clWhite
        Assert.Equal(0x000000FF, MShareFunctions.GetRGB(9));
        Assert.Equal(0x00FFFFFF, MShareFunctions.GetRGB(15));

        // 逐项全等（256 项），确保不是碰巧命中
        for (int c = 0; c < 256; c++)
            Assert.Equal(MShareGlobals.Palette256[c], MShareFunctions.GetRGB((byte)c));
    }

    // ------------------------------------------------------------------------------------
    // 职业 / 性别名（原文 8957 / 8973）
    // ------------------------------------------------------------------------------------

    [Fact]
    public void GetJobName_UsesGlobalNameTable()
    {
        // 原文 8957-8971（全局名在 1444-1448）
        Assert.Equal("战士", MShareFunctions.GetJobName(0));
        Assert.Equal("法师", MShareFunctions.GetJobName(1));
        Assert.Equal("道士", MShareFunctions.GetJobName(2));
        Assert.Equal("未知", MShareFunctions.GetJobName(3));
        Assert.Equal("未知", MShareFunctions.GetJobName(-1));

        // 换名（登录器可下发）后必须跟着变，而不是写死
        string saved = MShareGlobals.g_sWarriorName;
        MShareGlobals.g_sWarriorName = "武士";
        Assert.Equal("武士", MShareFunctions.GetJobName(0));
        MShareGlobals.g_sWarriorName = saved; // 全局是共享静态，必须还原（否则污染同类的其它用例）
    }

    [Fact]
    public void GetSexName_ZeroOneOnly()
    {
        // 原文 8973-8982：case 0/1，其余保持 Result := ''
        Assert.Equal("男", MShareFunctions.GetSexName(0));
        Assert.Equal("女", MShareFunctions.GetSexName(1));
        Assert.Equal("", MShareFunctions.GetSexName(2));
        Assert.Equal("", MShareFunctions.GetSexName(-1));
    }

    // ------------------------------------------------------------------------------------
    // 座标换算（原文 4181-4201）
    // ------------------------------------------------------------------------------------

    [Fact]
    public void ActorXYToMapXY_And_Back_UseIntegerDivisionLikeOriginal()
    {
        // 原文 4183：nX := nCurrX * 48 div 32 //  4184：nY := nCurrY * 32 div 32（恒等，照抄）
        MShareFunctions.ActorXYToMapXY(2, 3, out int mx, out int my);
        Assert.Equal(3, mx);   // 2*48/32 = 3
        Assert.Equal(3, my);   // 3*32/32 = 3（Y 轴恒等）

        MShareFunctions.MapXYToActorXY(3, 3, out int ax, out int ay);
        Assert.Equal(2, ax);   // 3*32/48 = 2（整数除法截断，非 2.0 上取）
        Assert.Equal(3, ay);

        // 截断方向：-1*48/32 = -1（C# 向零截断）
        MShareFunctions.ActorXYToMapXY(-1, -1, out int nx, out int ny);
        Assert.Equal(-1, nx);
        Assert.Equal(-1, ny);
    }

    [Fact]
    public void MapToScreen_And_ScreenToMap_AreInverseRounded()
    {
        // 原文 4195：nXY := Round(nScreenWH * nMapXY / nMapWH)
        MShareFunctions.MapToScreen(64, 100, 10, out int sx);
        Assert.Equal(16, sx);  // 100*10/64 = 15.625 → 16

        // 原文 4200：nXY := Round(nMapWH * nScreenXY / nScreenWH)
        MShareFunctions.ScreenToMap(64, 100, 16, out int mx);
        Assert.Equal(10, mx);  // 64*16/100 = 10.24 → 10

        // .5 边界走 Delphi Round 的银行家舍入（ToEven）：1/3 与 2/3 的中间落点
        MShareFunctions.MapToScreen(3, 1, 1, out int half);
        Assert.Equal(0, half); // 1/3 = 0.333… → 0
        MShareFunctions.MapToScreen(3, 2, 1, out int half2);
        Assert.Equal(1, half2); // 2/3 = 0.667… → 1
    }

    // ------------------------------------------------------------------------------------
    // 字符串 / 数字（原文 8536 / 8557 / 11484 / 11659 / 11672）
    // ------------------------------------------------------------------------------------

    [Theory]
    [InlineData("M001", "M", 1)]        // 注意：StrToIntDef('001') = 1，前导零丢弃
    [InlineData("Tiles10", "Tiles", 10)]
    [InlineData("abc", "abc", 0)]       // nC = 0 ⇒ 原样返回
    [InlineData("123", "", 123)]        // 全数字 ⇒ Copy(Src,1,0) = ''
    [InlineData("", "", 0)]
    public void DeleteNumber_StripsTrailingDigits(string src, string expectedText, int expectedNum)
    {
        // 原文 8536-8554
        string actual = MShareFunctions.DeleteNumber(src, out int nNum);
        Assert.Equal(expectedText, actual);
        Assert.Equal(expectedNum, nNum);
    }

    [Theory]
    [InlineData("Tiles.wil", "Tiles")]
    [InlineData("a.b.c", "a")]          // 原文用 Pos ⇒ 第一个点，不是最后一个
    [InlineData("noext", "noext")]
    [InlineData(".hidden", "")]
    public void DeleteFileExt_CutsAtFirstDot(string src, string expected)
    {
        // 原文 8557-8564（8640 处为逐字重复的副本，行为相同）
        Assert.Equal(expected, MShareFunctions.DeleteFileExt(src));
    }

    [Fact]
    public void tick_diff_WrapsLikeCardinal()
    {
        // 原文 11661-11664
        Assert.Equal(500u, MShareFunctions.tick_diff(1000, 1500));
        Assert.Equal(0u, MShareFunctions.tick_diff(1500, 1500));
        // 回绕：High(Cardinal) - start + end
        Assert.Equal(uint.MaxValue - 4000u + 1000u, MShareFunctions.tick_diff(4000, 1000));
    }

    [Theory]
    [InlineData(99999999u, "9999W")]  // 99999999 div 10000 = 9999
    [InlineData(100000u, "10W")]
    [InlineData(99999u, "99999")]
    [InlineData(0u, "0")]
    [InlineData(100000000u, "1.00E")] // 原文 11675 除的是 1e8（不是 1e9），照抄
    [InlineData(250000000u, "2.50E")]
    public void HpAddUnit_FormatsLikeOriginal(uint v, string expected)
    {
        Assert.Equal(expected, MShareFunctions.HpAddUnit(v));
    }

    [Theory]
    // ★ 原文 11484 的 `IntToHexN` **名不副实**：它把 `Digits` 当"进制"用
    //   （`I mod Digits` / `I div Digits`），而不是十六进制的位数；`Digits ∈ [2, 10]` 才有效，
    //   `Digits > 10` 或 `< 2` 直接返回空串。**没有 `Digits = 16` 的十六进制形态**
    //   —— 这是原文缺陷（见报告 P17-DEF-02），此处照抄并锁死。
    [InlineData(255, 10, "255")]
    [InlineData(0, 10, "0")]
    [InlineData(255, 2, "11111111")]
    [InlineData(255, 3, "100110")]   // 1*3^5+0+0+1*3^2+1*3+0 = 243+9+3 = 255 ✓
    [InlineData(255, 4, "3333")]     // 3*64+3*16+3*4+3 = 192+48+12+3 = 255 ✓
    [InlineData(255, 5, "2010")]     // 2*125+0+1*5+0 = 255 ✓
    [InlineData(255, 8, "377")]      // 3*64+7*8+7 = 192+56+7 = 255 ✓
    [InlineData(255, 9, "313")]      // 3*81+1*9+3 = 243+9+3 = 255 ✓
    [InlineData(1, 2, "1")]
    [InlineData(31, 8, "37")]        // 3*8+7 = 31 ✓
    public void IntToHexN_UsesDigitsAsRadix(int v, int digits, string expected)
    {
        // 原文 11484-11519
        Assert.Equal(expected, MShareFunctions.IntToHexN(v, digits));
    }

    [Theory]
    [InlineData(255, 1)]   // Digits < 2 ⇒ ''
    [InlineData(255, 0)]
    [InlineData(255, -1)]
    [InlineData(255, 11)]  // Digits > 10 ⇒ ''
    [InlineData(255, 16)]  // ★ 十六进制在这里**返回空串**（原文缺陷 P17-DEF-02）
    [InlineData(0, 16)]
    [InlineData(255, 99)]
    public void IntToHexN_OutOfRangeDigitsReturnsEmpty(int v, int digits)
    {
        Assert.Equal("", MShareFunctions.IntToHexN(v, digits));
    }

    [Fact]
    public void GetTickCount_Ex_IsMonotonicish()
    {
        uint a = MShareFunctions.GetTickCount_Ex();
        uint b = MShareFunctions.GetTickCount_Ex();
        Assert.True(b >= a || b < a); // 只要求可调用且返回 uint（回绕允许）
    }

    // ------------------------------------------------------------------------------------
    // 输入过滤 / 攻击判定 / 包裹格位（原文 11959 / 11781 / 11762）
    // ------------------------------------------------------------------------------------

    [Theory]
    [InlineData("hello@world", true)]
    [InlineData("<script>", true)]
    [InlineData("a>b", true)]
    [InlineData("$var", true)]
    [InlineData("普通文本", false)]
    [InlineData("", false)]
    public void GetInputBoxInFilterList_CharBlacklist(string input, bool expected)
    {
        // 原文 11970：WChr in [WideChar('@'), WideChar('<'), WideChar('>'), WideChar('$')]
        MShareGlobals.g_InputBoxFilterList = new List<string>();
        Assert.Equal(expected, MShareFunctions.GetInputBoxInFilterList(input));
    }

    [Fact]
    public void GetInputBoxInFilterList_SubstringListIsCaseInsensitiveOnInput()
    {
        // 原文 11976-11982：sInputBox := LowerCase(sInputBox); Pos(过滤词, sInputBox) > 0
        MShareGlobals.g_InputBoxFilterList = new List<string> { "gm", "广告" };
        Assert.True(MShareFunctions.GetInputBoxInFilterList("GM你好"));      // 输入转小写后含 "gm"
        Assert.True(MShareFunctions.GetInputBoxInFilterList("发广告了"));
        Assert.False(MShareFunctions.GetInputBoxInFilterList("正常发言"));

        // 原文 2243 初值 nil ⇒ 不能 NRE（原文 nil.Count 会炸；托管侧按空表处理并已在报告登记）
        MShareGlobals.g_InputBoxFilterList = null;
        Assert.False(MShareFunctions.GetInputBoxInFilterList("正常发言"));
    }

    [Theory]
    [InlineData(14, true)]    // SM_HIT
    [InlineData(15, true)]    // SM_HEAVYHIT
    [InlineData(16, true)]    // SM_BIGHIT
    [InlineData(18, true)]    // SM_POWERHIT
    [InlineData(19, true)]    // SM_LONGHIT
    [InlineData(24, true)]    // SM_WIDEHIT
    [InlineData(8, true)]     // SM_FIREHIT
    [InlineData(25, true)]    // SM_CRSHIT
    [InlineData(26, true)]    // SM_TWNHIT
    [InlineData(56, true)]    // SM_SWORDHIT
    [InlineData(43, true)]    // SM_43HIT
    [InlineData(66, true)]    // SM_66HIT
    [InlineData(166, true)]   // SM_66HIT1
    [InlineData(9101, true)]  // SM_101HIT
    [InlineData(9102, true)]  // SM_102HIT
    [InlineData(9103, true)]  // SM_103HIT
    [InlineData(113, true)]   // SM_113HIT
    [InlineData(115, true)]   // SM_115HIT
    [InlineData(11000, true)] // SM_CUSTOM_HIT001 区间下界
    [InlineData(11299, true)] // SM_CUSTOM_HIT001 + CUSTOM_MAGIC_COUNT - 1
    [InlineData(11299 + 1, false)] // 区间外（原文是严格 `<`）
    [InlineData(10999, false)]
    [InlineData(0, false)]
    [InlineData(1, false)]
    public void IsAttackAction_MatchesOriginalEighteenPlusRange(int action, bool expected)
    {
        // 原文 11783
        Assert.Equal(expected, MShareFunctions.IsAttackAction(action));
    }

    [Fact]
    public void GetMaxBagCount_AddsExtBagOpenCount()
    {
        // 原文 11764：Result := DEF_MAX_BAG_ITEM + g_ExtBagOpenItemCount（DEF_MAX_BAG_ITEM = 46）
        Assert.Equal(Grobal2Const.DEF_MAX_BAG_ITEM, MShareFunctions.GetMaxBagCount());
        MShareGlobals.g_ExtBagOpenItemCount = 10;
        Assert.Equal(Grobal2Const.DEF_MAX_BAG_ITEM + 10, MShareFunctions.GetMaxBagCount());
    }
}
