using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using GXX.Client.GUI.Mir;
using GXX.Core.Protocol;
using Xunit;

namespace GXX.Client.Tests;

// ============================================================================================
// 【P17 切片3 / p17-client-mshare】TWarrContinueHitManager + 平台族
//
// 行号一律指 `_analysis/utf8_mirror/Client-HGE/MShare.pas` 的 UTF-8 镜像行号。
//
// ⚠ 配置播种顺序约定（本轮踩过的坑）：`SetWarrIds` **会先重置 seam**，
//   所以调用点必须「先 SetWarrIds、再设开关/间隔/开关量」，否则会被清零。
//   一律走 `ConfigureWarr(...)` 以免写反。
// ============================================================================================

public class MShareWarrContinueHitManagerP17Tests
{
    public MShareWarrContinueHitManagerP17Tests()
    {
        MShareGlobalsReset.ResetForTests();
    }

    /// <summary>
    /// 清场并把受管技能表设成 `ids`（原文 `g_ClientConfig.ArrDisableWarrContinueHitIDs`，0 = 表尾）。
    /// ⚠ 内含重置 ⇒ **必须在设置开关/间隔之前**调用。
    /// </summary>
    private static void SetWarrIds(params ushort[] ids)
    {
        MShareGlobalsReset.ResetForTests();
        for (int i = 0; i < ids.Length && i < 10; i++)
            MShareGlobals.g_ConfigClient.ArrDisableWarrContinueHitIDs[i] = ids[i];
    }

    /// <summary>正确顺序的封装：先清场播种，再设开关与间隔。</summary>
    private static void ConfigureWarr(ushort[] ids, bool disableSwitch, uint minInterval)
    {
        SetWarrIds(ids);
        MShareGlobals.g_ConfigClient.boDisableWarrContinueHit = disableSwitch ? (byte)1 : (byte)0;
        MShareGlobals.g_ConfigClient.nWarrContinueHitMinInterval = minInterval;
    }

    // ------------------------------------------------------------------------------------
    // Create / 初值（原文 11987-11991）
    // ------------------------------------------------------------------------------------

    [Fact]
    public void Create_ZeroesLastUseMagicTick()
    {
        // 原文 11989：FLastUseMagicTick := 0;（FLastUseMagicID 靠零初始化，未显式赋值）
        var m = new TWarrContinueHitManager();
        Assert.Equal(0u, m.LastUseMagicTickForTest);
        Assert.Equal(0, m.LastUseMagicIDForTest);
    }

    // ------------------------------------------------------------------------------------
    // 配置播种自检
    // ------------------------------------------------------------------------------------

    [Fact]
    public void ConfigureWarr_ActuallyLandsOnTheConfigClient()
    {
        ConfigureWarr(new ushort[] { 12, 25 }, disableSwitch: true, minInterval: 500);

        Assert.Equal(1, MShareGlobals.g_ConfigClient.boDisableWarrContinueHit);
        Assert.Equal(500u, MShareGlobals.g_ConfigClient.nWarrContinueHitMinInterval);
        Assert.Equal(12, MShareGlobals.g_ConfigClient.ArrDisableWarrContinueHitIDs[0]);
        Assert.Equal(25, MShareGlobals.g_ConfigClient.ArrDisableWarrContinueHitIDs[1]);
        Assert.Equal(0, MShareGlobals.g_ConfigClient.ArrDisableWarrContinueHitIDs[2]);
    }

    [Fact]
    public void SetWarrIds_ResetsTheSwitchAndInterval_SoOrderMatters()
    {
        // 反面锁死：先设开关、再 SetWarrIds ⇒ 开关被抹回 0（这就是本轮两个用例假红的根因）
        MShareGlobals.g_ConfigClient.boDisableWarrContinueHit = 1;
        MShareGlobals.g_ConfigClient.nWarrContinueHitMinInterval = 777;
        SetWarrIds(12);

        Assert.Equal(0, MShareGlobals.g_ConfigClient.boDisableWarrContinueHit);
        Assert.Equal(0u, MShareGlobals.g_ConfigClient.nWarrContinueHitMinInterval);
    }

    // ------------------------------------------------------------------------------------
    // CanOpenMagic —— ★ 原文如此：方法体被整段注释，恒 True（原文 11993-12069）
    // ------------------------------------------------------------------------------------

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(12)]
    [InlineData(65535)]
    public void CanOpenMagic_AlwaysTrue_BodyIsEntirelyCommentedOutInTheOriginal(ushort magicId)
    {
        // 原文 12003-12068 整段被 `{ }` 注释，只剩 12001 的 `Result := True;`
        // ⇒ **无论入参是什么、无论全局开关如何**都返回 True。
        ConfigureWarr(new ushort[] { 12, 25, 26 }, disableSwitch: true, minInterval: 9999);
        var m = new TWarrContinueHitManager();
        m.UseMagic(12);
        m.LastUseMagicTickForTest = MShareGlobals.MyGetTickCount; // 间隔 0

        Assert.True(m.CanOpenMagic(magicId));
    }

    [Fact]
    public void CanOpenMagic_IgnoresTheDisableSwitchAndTheManagedTable()
    {
        // 再确认一次"被注释的逻辑确实不生效"：
        // 把原文注释块（12003-12068）里会走到的**所有前置条件**都摆成"会导致 False"的形态，
        // 函数仍必须返回 True —— 因为那段逻辑根本没有编译进原文的二进制。
        ConfigureWarr(new ushort[] { 12, 25, 26, 40, 42, 43, 56, 66, 113, 115 },
                      disableSwitch: true, minInterval: 9999);   // 装满 10 个受管 ID（无 0 表尾）
        MShareGlobals.g_boContinuous = true;

        var m = new TWarrContinueHitManager();
        m.UseMagic(12);
        m.LastUseMagicTickForTest = MShareGlobals.MyGetTickCount;

        Assert.True(m.CanOpenMagic(12));
        Assert.True(m.CanOpenMagic(25));
        Assert.True(m.CanOpenMagic(999));
        Assert.True(m.CanOpenMagic(0));
    }

    // ------------------------------------------------------------------------------------
    // CanUseMagic —— 三道早退门（原文 12071-12103）
    // ------------------------------------------------------------------------------------

    [Fact]
    public void CanUseMagic_Gate1_DisabledFeaturePassesThrough()
    {
        // 原文 12079-12080：if not g_ClientConfig.boDisableWarrContinueHit then Exit;
        ConfigureWarr(new ushort[] { 12 }, disableSwitch: false, minInterval: 9999);
        var m = new TWarrContinueHitManager();
        m.UseMagic(12);                                       // 让 FLastUseMagicID = 12
        m.LastUseMagicTickForTest = MShareGlobals.MyGetTickCount;

        // 门 1 先短路：即使刚刚用过、即使 tick 间隔为 0、即使间隔要求 9999，也返回 True
        Assert.True(m.CanUseMagic(12 + 1));
    }

    [Fact]
    public void CanUseMagic_Gate2_NoPreviousMagicPassesThrough()
    {
        // 原文 12082-12083：if FLastUseMagicID = 0 then Exit;
        ConfigureWarr(new ushort[] { 12, 25 }, disableSwitch: true, minInterval: 9999);
        var m = new TWarrContinueHitManager();                // FLastUseMagicID = 0

        Assert.True(m.CanUseMagic(25));
    }

    [Fact]
    public void CanUseMagic_Gate3_SameMagicPassesThrough()
    {
        // 原文 12084-12085：if FLastUseMagicID = MagicID then Exit;
        ConfigureWarr(new ushort[] { 12, 25 }, disableSwitch: true, minInterval: 9999);
        var m = new TWarrContinueHitManager();
        m.UseMagic(12);
        m.LastUseMagicTickForTest = MShareGlobals.MyGetTickCount; // 间隔 0

        Assert.True(m.CanUseMagic(12));                       // 同一个技能 ⇒ 门 3 放行
    }

    [Fact]
    public void CanUseMagic_NotInManagedTable_PassesThrough()
    {
        // 原文 12100：IsFound 为 False 时**不改** Result（保持 12077 的 True）
        ConfigureWarr(new ushort[] { 12, 25 }, disableSwitch: true, minInterval: 9999);
        var m = new TWarrContinueHitManager();
        m.UseMagic(12);
        m.LastUseMagicTickForTest = MShareGlobals.MyGetTickCount;

        Assert.True(m.CanUseMagic(99));                       // 99 不在表里
    }

    [Fact]
    public void CanUseMagic_InManagedTable_TooSoonIsFalse()
    {
        // 原文 12101：tick_diff(...) >= nWarrContinueHitMinInterval + 100
        ConfigureWarr(new ushort[] { 12, 25 }, disableSwitch: true, minInterval: 500); // 阈值 600
        var m = new TWarrContinueHitManager();
        m.UseMagic(12);

        // 间隔 300 < 600 ⇒ False（留余量，避免调度抖动把边界推过去）
        m.LastUseMagicTickForTest = MShareGlobals.MyGetTickCount - 300;
        Assert.False(m.CanUseMagic(25));
    }

    [Fact]
    public void CanUseMagic_InManagedTable_EnoughIntervalIsTrue()
    {
        ConfigureWarr(new ushort[] { 12, 25 }, disableSwitch: true, minInterval: 500);
        var m = new TWarrContinueHitManager();
        m.UseMagic(12);

        // 间隔 2000 >= 600 ⇒ True
        m.LastUseMagicTickForTest = MShareGlobals.MyGetTickCount - 2000;
        Assert.True(m.CanUseMagic(25));
    }

    [Fact]
    public void CanUseMagic_IntervalZero_StillRequiresTheHardcoded100ms()
    {
        // nWarrContinueHitMinInterval = 0 ⇒ 阈值 = 100（原文那个 `+ 100` 是硬编码下限）
        ConfigureWarr(new ushort[] { 12, 25 }, disableSwitch: true, minInterval: 0);
        var m = new TWarrContinueHitManager();
        m.UseMagic(12);

        m.LastUseMagicTickForTest = MShareGlobals.MyGetTickCount - 10;   // 10 < 100
        Assert.False(m.CanUseMagic(25));

        m.LastUseMagicTickForTest = MShareGlobals.MyGetTickCount - 5000; // 5000 >= 100
        Assert.True(m.CanUseMagic(25));
    }

    [Fact]
    public void CanUseMagic_ZeroIntervalAtSameTickIsFalse_ProvingThePlus100Floor()
    {
        // 若原文没有 `+ 100` 而是直接 `>= 0`，则"同一 tick"会返回 True。
        // 这里把间隔设为 0 并把 last tick 设为"现在"，差值为 0..(抖动) —— 必然 < 100 ⇒ False。
        ConfigureWarr(new ushort[] { 12, 25 }, disableSwitch: true, minInterval: 0);
        var m = new TWarrContinueHitManager();
        m.UseMagic(12);
        m.LastUseMagicTickForTest = MShareGlobals.MyGetTickCount;

        Assert.False(m.CanUseMagic(25));
    }

    [Fact]
    public void CanUseMagic_TickDiffWraparound_IsHandledByCardinalSemantics()
    {
        // 原文 12101 用的是 `tick_diff`（Cardinal）：last 大于 now 时按回绕算。
        // 把 last 设成 now + 1（回绕侧）⇒ tick_diff 约等于 uint.MaxValue ⇒ 必然 >= 阈值 ⇒ True。
        ConfigureWarr(new ushort[] { 12, 25 }, disableSwitch: true, minInterval: 500);
        var m = new TWarrContinueHitManager();
        m.UseMagic(12);
        m.LastUseMagicTickForTest = MShareGlobals.MyGetTickCount + 1;

        Assert.True(m.CanUseMagic(25));
    }

    // ------------------------------------------------------------------------------------
    // UseMagic（原文 12105-12127）
    // ------------------------------------------------------------------------------------

    [Fact]
    public void UseMagic_RecordsIdAndTick_OnlyWhenIdIsInManagedTable()
    {
        ConfigureWarr(new ushort[] { 12, 25 }, disableSwitch: true, minInterval: 0);
        var m = new TWarrContinueHitManager();

        m.UseMagic(99);                                       // 不在表里 ⇒ 不记录
        Assert.Equal(0, m.LastUseMagicIDForTest);
        Assert.Equal(0u, m.LastUseMagicTickForTest);

        m.UseMagic(25);                                       // 在表里 ⇒ 记录
        Assert.Equal(25, m.LastUseMagicIDForTest);
        Assert.NotEqual(0u, m.LastUseMagicTickForTest);
    }

    [Fact]
    public void UseMagic_ZeroTerminatedScan_StopsAtFirstZero()
    {
        // 原文 12114-12115：if TempID = 0 then Break;（0 之后的元素**永不**被看到）
        ConfigureWarr(new ushort[] { 0, 25 }, disableSwitch: true, minInterval: 0);
        var m = new TWarrContinueHitManager();

        m.UseMagic(25);
        Assert.Equal(0, m.LastUseMagicIDForTest);             // 未记录（正面锁死"遇 0 即停"）
    }

    [Fact]
    public void UseMagic_FirstElementZero_TableIsEmpty()
    {
        ConfigureWarr(new ushort[0], disableSwitch: true, minInterval: 0);  // 全 0
        var m = new TWarrContinueHitManager();

        m.UseMagic(12);
        Assert.Equal(0, m.LastUseMagicIDForTest);
    }

    [Fact]
    public void UseMagic_ScansAllTenSlots_WhenNoneIsZero()
    {
        // 原文 `for I := 0 to Length(...) - 1`（0..9）：最后一格也要能被命中
        ushort[] full = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        ConfigureWarr(full, disableSwitch: true, minInterval: 0);
        var m = new TWarrContinueHitManager();

        m.UseMagic(10);                                       // 最后一格
        Assert.Equal(10, m.LastUseMagicIDForTest);
    }

    // ------------------------------------------------------------------------------------
    // 与 M2Server 同名类的隔离（p12-e2only-review:839-840 登记的风险）
    // ------------------------------------------------------------------------------------

    [Fact]
    public void ClientTypeIsInTheClientNamespace()
    {
        Assert.Equal("GXX.Client.GUI.Mir.TWarrContinueHitManager", typeof(TWarrContinueHitManager).FullName);
    }

    [Fact]
    public void TwoSameNamedClasses_HaveDifferentCanOpenMagicSignatures()
    {
        // M2 版（GXX.M2Server.Engine）的 `CanOpenMagic(ushort, ref string)` 有 2 个参数；
        // 客户端版只有 1 个 ⇒ 两者不可互相改指。若 GXX.M2Server 未加载则跳过该半段断言。
        var clientOpen = typeof(TWarrContinueHitManager).GetMethod("CanOpenMagic");
        Assert.NotNull(clientOpen);
        Assert.Single(clientOpen!.GetParameters());

        var m2 = Type.GetType("GXX.M2Server.Engine.TWarrContinueHitManager, GXX.M2Server");
        if (m2 != null)
        {
            Assert.NotEqual(typeof(TWarrContinueHitManager), m2);
            var m2Open = m2.GetMethod("CanOpenMagic");
            Assert.NotNull(m2Open);
            Assert.Equal(2, m2Open!.GetParameters().Length);
        }
    }
}

// ============================================================================================
// 平台族（优先③）
// ============================================================================================

public class MSharePlatformFamilyP17Tests
{
    public MSharePlatformFamilyP17Tests() => MShareGlobalsReset.ResetForTests();

    // ------------------------------------------------------------------------------------
    // IsInContinuous（原文 3267-3270）
    // ------------------------------------------------------------------------------------

    [Fact]
    public void IsInContinuous_ReadsTheGlobal()
    {
        // 原文 3269：Result := g_boContinuous;（后面的 InterlockedCompareExchange 是注释）
        Assert.False(MShareFunctions.IsInContinuous());
        MShareGlobals.g_boContinuous = true;
        Assert.True(MShareFunctions.IsInContinuous());
    }

    // ------------------------------------------------------------------------------------
    // ProcessFileNameSpecialChar（原文 11565-11597）
    // ------------------------------------------------------------------------------------

    [Theory]
    [InlineData("/", "{")]
    [InlineData("\\", "}")]
    [InlineData(":", ";")]
    [InlineData("*", "@")]
    [InlineData("?", "!")]
    [InlineData("\"", "~")]
    [InlineData("<", "(")]
    [InlineData(">", ")")]
    [InlineData("|", "-")]
    public void ProcessFileNameSpecialChar_ReplacesEachIllegalChar(string input, string expected)
        => Assert.Equal(expected, MShareFunctions.ProcessFileNameSpecialChar(input));

    [Fact]
    public void ProcessFileNameSpecialChar_CombinedAndPassthrough()
    {
        Assert.Equal("a{b}c;d@e!f~g(h)i-j", MShareFunctions.ProcessFileNameSpecialChar("a/b\\c:d*e?f\"g<h>i|j"));
        Assert.Equal("正常文件名.txt", MShareFunctions.ProcessFileNameSpecialChar("正常文件名.txt"));
        Assert.Equal("", MShareFunctions.ProcessFileNameSpecialChar(""));
        Assert.Null(MShareFunctions.ProcessFileNameSpecialChar(null));
    }

    // ------------------------------------------------------------------------------------
    // GetTempDir / MakeTempFileName（原文 11682-11701）
    // ------------------------------------------------------------------------------------

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern uint GetTempPathW(uint nBufferLength, StringBuilder lpBuffer);

    [Fact]
    public void GetTempDir_MatchesWin32GetTempPath_IncludingTrailingSeparator()
    {
        // 原文 11686：GetTempPath(SizeOf(Buf) div SizeOf(Buf[0]), Buf); Result := StrPas(Buf);
        var buf = new StringBuilder(260);
        GetTempPathW(260, buf);
        string win32 = buf.ToString();

        Assert.Equal(win32, MShareFunctions.GetTempDir());
        Assert.EndsWith("\\", MShareFunctions.GetTempDir());   // 关键语义：**带**结尾反斜杠
    }

    [Fact]
    public void MakeTempFileName_QueryPerformanceCounterBranch_HasNoLeadingZeros()
    {
        // 原文 11694-11695：if QueryPerformanceCounter(N) then Result := Format('%x', [N]);
        MShareFunctions.PerformanceCounterProvider = () => 0xABC;
        Assert.Equal("abc", MShareFunctions.MakeTempFileName(""));         // 小写、无前导零
        Assert.Equal("abc.tmp", MShareFunctions.MakeTempFileName("tmp"));  // Length(FileExt) > 0 才加点
    }

    [Fact]
    public void MakeTempFileName_FallbackBranch_UsesEightPlusFourWithLeadingZeros()
    {
        // 原文 11697：Result := Format('%.8x%.4x', [MyGetTickCount, Random(MAXINT)]);
        MShareFunctions.QueryPerformanceCounterFailsForTests = true;
        MShareFunctions.RandomProvider = () => 0x1F;

        string s = MShareFunctions.MakeTempFileName("");
        Assert.Equal(12, s.Length);                       // 8 + 4 位十六进制
        Assert.EndsWith("001f", s);                       // %.4x 带前导零
        Assert.Matches("^[0-9a-f]{12}$", s);
    }

    [Fact]
    public void MakeTempFileName_RealBranch_ProducesHexAndOptionalExtension()
    {
        // 默认走 Stopwatch.GetTimestamp()（= QueryPerformanceCounter）
        string noExt = MShareFunctions.MakeTempFileName("");
        Assert.Matches("^[0-9a-f]+$", noExt);
        Assert.DoesNotContain(".", noExt);

        string withExt = MShareFunctions.MakeTempFileName("dat");
        Assert.EndsWith(".dat", withExt);
        Assert.True(withExt.Length > 4);
    }

    // ------------------------------------------------------------------------------------
    // _FileSize / GetAbsolutePathEx（原文 8984-8992 / 6542-6550）
    // ------------------------------------------------------------------------------------

    [Fact]
    public void FileSize_ReturnsZeroWhenNotFound_ElseTheLength()
    {
        // 原文 8987-8991：FindFirst 失败 ⇒ 0
        string missing = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
            "mshare_p17_missing_" + Guid.NewGuid().ToString("N") + ".bin");
        Assert.Equal(0u, MShareFunctions.FileSize(missing));

        string tmp = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
            "mshare_p17_" + Guid.NewGuid().ToString("N") + ".bin");
        try
        {
            System.IO.File.WriteAllBytes(tmp, new byte[1234]);
            Assert.Equal(1234u, MShareFunctions.FileSize(tmp));
        }
        finally
        {
            if (System.IO.File.Exists(tmp)) System.IO.File.Delete(tmp);
        }
    }

    [Fact]
    public void FileSize_DirectoryIsTreatedAsNotFound()
    {
        // 托管侧把目录按"找不到"处理 ⇒ 0（已在报告登记为 D-P17-04 的实现口径）
        Assert.Equal(0u, MShareFunctions.FileSize(System.IO.Path.GetTempPath()));
    }

    [Fact]
    public void FileSize_EmptyNameReturnsZeroInsteadOfThrowing()
    {
        Assert.Equal(0u, MShareFunctions.FileSize(""));
    }

    [Fact]
    public void GetAbsolutePathEx_CombinesWithoutResolvingDotDot()
    {
        // 原文 6547：PathCombine(Dest, BasePath, RelativePath)
        Assert.Equal(@"C:\base\sub", MShareFunctions.GetAbsolutePathEx(@"C:\base", "sub"));
        Assert.Equal(@"C:\base\x\..\y", MShareFunctions.GetAbsolutePathEx(@"C:\base", @"x\..\y"));
        Assert.Equal(@"C:\base", MShareFunctions.GetAbsolutePathEx(@"C:\base", ""));
    }

    // ------------------------------------------------------------------------------------
    // ShiftStateToPlugShiftState（原文 11786-11809）
    // ------------------------------------------------------------------------------------

    [Fact]
    public void ShiftStateToPlugShiftState_ConstantsMatchOriginal()
    {
        // 原文 2582-2588
        Assert.Equal(1, MShareFunctions.ShiftState_Shift);
        Assert.Equal(2, MShareFunctions.ShiftState_Alt);
        Assert.Equal(4, MShareFunctions.ShiftState_Ctrl);
        Assert.Equal(8, MShareFunctions.ShiftState_Left);
        Assert.Equal(16, MShareFunctions.ShiftState_Right);
        Assert.Equal(32, MShareFunctions.ShiftState_Middle);
        Assert.Equal(64, MShareFunctions.ShiftState_Double);
    }

    [Theory]
    [InlineData(GXX.Client.GUI.Share.TShiftState.ssShift, 1)]
    [InlineData(GXX.Client.GUI.Share.TShiftState.ssAlt, 2)]
    [InlineData(GXX.Client.GUI.Share.TShiftState.ssCtrl, 4)]
    [InlineData(GXX.Client.GUI.Share.TShiftState.ssLeft, 8)]
    [InlineData(GXX.Client.GUI.Share.TShiftState.ssRight, 16)]
    [InlineData(GXX.Client.GUI.Share.TShiftState.ssMiddle, 32)]
    [InlineData(GXX.Client.GUI.Share.TShiftState.ssDouble, 64)]
    public void ShiftStateToPlugShiftState_SingleBitsMapOneToOne(
        GXX.Client.GUI.Share.TShiftState shift, int expected)
        => Assert.Equal(expected, MShareFunctions.ShiftStateToPlugShiftState(shift));

    [Fact]
    public void ShiftStateToPlugShiftState_EmptyIsZero_AndBitsAccumulate()
    {
        Assert.Equal(0, MShareFunctions.ShiftStateToPlugShiftState(GXX.Client.GUI.Share.TShiftState.ssNone));

        // 七个独立 if ⇒ 累加（不是 else-if 链）
        var all = GXX.Client.GUI.Share.TShiftState.ssShift | GXX.Client.GUI.Share.TShiftState.ssAlt
                | GXX.Client.GUI.Share.TShiftState.ssCtrl | GXX.Client.GUI.Share.TShiftState.ssLeft
                | GXX.Client.GUI.Share.TShiftState.ssRight | GXX.Client.GUI.Share.TShiftState.ssMiddle
                | GXX.Client.GUI.Share.TShiftState.ssDouble;
        Assert.Equal(1 + 2 + 4 + 8 + 16 + 32 + 64, MShareFunctions.ShiftStateToPlugShiftState(all));

        Assert.Equal(1 + 4, MShareFunctions.ShiftStateToPlugShiftState(
            GXX.Client.GUI.Share.TShiftState.ssShift | GXX.Client.GUI.Share.TShiftState.ssCtrl));
    }

    // ------------------------------------------------------------------------------------
    // CheckBlockListSys（原文 11454-11482）
    // ------------------------------------------------------------------------------------

    [Fact]
    public void CheckBlockListSys_NoUsernameMeansNotBlocked()
    {
        // 原文 11472：sUserName <> '' 才查黑名单；否则 Result 保持 True
        MShareGlobals.g_MyBlacklist = new List<string> { "坏人" };
        // 非四个已知 Ident ⇒ case 不取用户名 ⇒ 用户名保持 ''
        Assert.True(MShareFunctions.CheckBlockListSys(0, "坏人:你好"));
    }

    [Fact]
    public void CheckBlockListSys_Hear_BlockedWhenNameInBlacklist()
    {
        MShareGlobals.g_MyBlacklist = new List<string> { "坏人" };
        Assert.False(MShareFunctions.CheckBlockListSys(Grobal2Const.SM_HEAR, "坏人:你好呀"));
        Assert.True(MShareFunctions.CheckBlockListSys(Grobal2Const.SM_HEAR, "好人:你好呀"));
    }

    [Fact]
    public void CheckBlockListSys_BlacklistLookupIsCaseInsensitive()
    {
        // 原文 11475 走 THashedStringList/TStringList.IndexOf ⇒ AnsiCompareText（大小写不敏感）
        MShareGlobals.g_MyBlacklist = new List<string> { "BadBoy" };
        Assert.False(MShareFunctions.CheckBlockListSys(Grobal2Const.SM_HEAR, "badboy:hi"));
        Assert.False(MShareFunctions.CheckBlockListSys(Grobal2Const.SM_HEAR, "BADBOY:hi"));
    }

    [Fact]
    public void CheckBlockListSys_WhisperSplitsOnEquals()
    {
        // 原文 11469-11470：SM_WHISPER 用 '=' 切
        MShareGlobals.g_MyBlacklist = new List<string> { "坏人" };
        Assert.False(MShareFunctions.CheckBlockListSys(Grobal2Const.SM_WHISPER, "坏人=悄悄话"));
        Assert.True(MShareFunctions.CheckBlockListSys(Grobal2Const.SM_WHISPER, "好人=悄悄话"));
    }

    [Fact]
    public void CheckBlockListSys_Cry_StripsFirstThreeCharacters()
    {
        // 原文 11465-11468：SM_CRY 先按 ':' 切，再 RightStr(名, Length-3)
        // 例："+20坏人:呜" ⇒ 切出 "+20坏人" ⇒ 去掉前 3 字符 ⇒ "坏人"
        MShareGlobals.g_MyBlacklist = new List<string> { "坏人" };
        Assert.False(MShareFunctions.CheckBlockListSys(Grobal2Const.SM_CRY, "+20坏人:呜"));
    }

    [Fact]
    public void CheckBlockListSys_GroupAndGuildMessagesUseColon()
    {
        // 原文 11462-11464：SM_HEAR / SM_GROUPMESSAGE / SM_GUILDMESSAGE 同一分支
        MShareGlobals.g_MyBlacklist = new List<string> { "坏人" };
        Assert.False(MShareFunctions.CheckBlockListSys(Grobal2Const.SM_GROUPMESSAGE, "坏人:组队消息"));
        Assert.False(MShareFunctions.CheckBlockListSys(Grobal2Const.SM_GUILDMESSAGE, "坏人:行会消息"));
    }

    [Fact]
    public void CheckBlockListSys_PrivateChatShowsLevel_BlacklistStillFilters()
    {
        // 原文 11473-11474 注释"私聊显示等级时，黑名单不过滤" —— 注意它**先**把 `名 等级` 以空格切成 `名`，
        // 所以带等级前缀的形态仍会被黑名单命中。
        MShareGlobals.g_MyBlacklist = new List<string> { "坏人" };
        Assert.False(MShareFunctions.CheckBlockListSys(Grobal2Const.SM_HEAR, "坏人 42级:你好"));
    }

    [Fact]
    public void CheckBlockListSys_NullBlacklistDoesNotThrow()
    {
        // 原文 2462 初值 nil（Assigned 未判，原文会炸）；托管侧返回"不在黑名单"
        MShareGlobals.g_MyBlacklist = null;
        Assert.True(MShareFunctions.CheckBlockListSys(Grobal2Const.SM_HEAR, "坏人:你好"));
    }

    [Fact]
    public void CheckBlockListSys_ExceptionPathReturnsFalseAndLeavesATrace()
    {
        // 原文 11478-11481：except ⇒ Result := False; DebugOutStr(...)
        int traces = 0;
        MShareFunctions.CheckBlockListSysExceptionHandler = () => traces++;

        // 构造会抛异常的输入：sMsg = null 时 GetValidStr3_Ex 内部 str.Length 会 NRE
        bool result = MShareFunctions.CheckBlockListSys(Grobal2Const.SM_HEAR, null);

        Assert.False(result);
        Assert.Equal(1, traces);
    }

    [Fact]
    public void CheckBlockListSys_EmptyMessageIsNotBlocked()
    {
        MShareGlobals.g_MyBlacklist = new List<string> { "坏人" };
        Assert.True(MShareFunctions.CheckBlockListSys(Grobal2Const.SM_HEAR, ""));
    }
}
