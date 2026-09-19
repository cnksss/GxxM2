using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GXX.GameCenter;
using Xunit;
using static GXX.GameCenter.GShareGlobals;

namespace GXX.GameCenter.Tests;

/// <summary>
/// GShare.pas 声明段（第 13..46 行 const/type + 第 56..283 行 var）覆盖率与语义测试。
/// </summary>
[Collection("GameCenterSequential")]
public sealed class GShareDeclTests : GameCenterTestBase
{
    // ---------- 声明完整性（防漏项/改名） ----------

    [Fact]
    public void Declarations_AllGlobalsPresentWithExpectedTypes()
    {
        var problems = GShareGlobals.VerifyDeclarations();
        Assert.Empty(problems);
    }

    [Fact]
    public void Declarations_GlobalCountMatchesPasVarSegment()
    {
        // GShare.pas:56..283 的 var 段共 235 个全局变量（含数组/记录变量）。
        Assert.Equal(235, GShareGlobals.ExpectedGlobals.Count);
    }

    [Fact]
    public void Constants_MaxRunGateCountIsEight()
    {
        Assert.Equal(8, GShareConst.MAXRUNGATECOUNT);
    }

    // ---------- 默认值 ----------

    [Fact]
    public void ResetForTests_RestoresPasInitialValues()
    {
        // 先验证 ResetForTests 的原始默认值，再恢复到测试临时目录。
        GShareGlobals.ResetForTests();
        Assert.Equal(@"D:\MirServer\", g_sGameDirectory);
        Reset();
        Assert.Equal("BmM2", g_sGameName);   // 原文如此：g_sGameName 无初值，实际由 LoadConfig 读 Config.ini 的 GameName（缺省 m_sGameName='BmM2'）
        Assert.Equal(0, g_nFormIdx);
        Assert.Equal(@".\Config.ini", g_sConfFile);
        Assert.Equal("", g_sOldGameDirectory);
        Assert.Equal(false, g_boUseSqliteDB);
        Assert.Equal("HeroDB", g_sHeroDBName);
        Assert.Equal(@"D:\MirServer\Mud2\DB\BmM2.db", g_sSqliteDBName);
        Assert.Equal(0, g_nDataSaveDBType);
        Assert.Equal((ushort)3306, g_wDataSaveDBPort);
        Assert.Equal("0.0.0.0", g_sAllIPaddr);
        Assert.Equal("127.0.0.1", g_sLocalIPaddr);
        Assert.Equal("127.0.0.2", g_sLocalIPaddr1);
        Assert.Equal("192.168.0.1", g_sExtIPaddr);
        Assert.Equal("192.168.100.1", g_sExtNetComIPaddr);
        Assert.Equal(false, g_boDoubleLineMode);
        Assert.Equal(10000, g_nLimitOnlineUser);
        Assert.Equal(false, g_boDynamicIPMode);
    }

    [Fact]
    public void ResetForTests_ProgramRecordsAreAllZeroed()
    {
        g_boDBServer_GetStart = false;
        DBServer = TProgram.Create(false, false, false, 9, 9, "x.exe", "x\\");
        RunGate[3].sProgramFile = "RunGate.exe";
        RunGate[3].nMainFormX = 12345;
        g_RunGateInfo[5].nGatePort = 6543;

        GShareGlobals.ResetForTests();

        Assert.False(DBServer.boGetStart);
        Assert.Equal("", DBServer.sProgramFile);
        Assert.Equal(0, DBServer.nMainFormX);
        Assert.Equal("", RunGate[3].sProgramFile);
        Assert.Equal(0, RunGate[3].nMainFormX);
        Assert.Equal(0, g_RunGateInfo[5].nGatePort);
        Assert.Equal(GShareConst.MAXRUNGATECOUNT, RunGate.Length);
        Assert.Equal(GShareConst.MAXRUNGATECOUNT, g_RunGateInfo.Length);
        Assert.Equal(27201, g_nRunGateDBPort_MulThread);
        Assert.Equal(27201, g_nRunGate0_DBPort);
        Assert.Equal(27901, g_nRunGate7_DBPort);
    }

    [Fact]
    public void ResetForTests_RestoresRunGatePortDefaults()
    {
        GShareGlobals.ResetForTests();
        Assert.Equal(7200, g_nRunGate_GatePort);
        Assert.Equal(7300, g_nRunGate1_GatePort);
        Assert.Equal(7400, g_nRunGate2_GatePort);
        Assert.Equal(7500, g_nRunGate3_GatePort);
        Assert.Equal(7600, g_nRunGate4_GatePort);
        Assert.Equal(7700, g_nRunGate5_GatePort);
        Assert.Equal(7800, g_nRunGate6_GatePort);
        Assert.Equal(7900, g_nRunGate7_GatePort);
        Assert.Equal(3, g_nRunGate_Count);
    }

    [Fact]
    public void ResetForTests_RestoresDirectoryConfigs()
    {
        GShareGlobals.ResetForTests();
        Assert.Equal(@"DBServer\", g_sDBServer_Directory);
        Assert.Equal("dbsrc.ini", g_sDBServer_ConfigFile);
        Assert.Equal(@"LoginSrv\", g_sLoginServer_Directory);
        Assert.Equal("Logsrv.ini", g_sLoginServer_ConfigFile);
        Assert.Equal(@"LogServer\", g_sLogServer_Directory);
        Assert.Equal("LogData.ini", g_sLogServer_ConfigFile);
        Assert.Equal(@"Mir200\", g_sM2Server_Directory);
        Assert.Equal("!setup.txt", g_sM2Server_ConfigFile);
        Assert.Equal(@"LoginGate\", g_sLoginGate_Directory);
        Assert.Equal(@"SelGate\", g_sSelGate_Directory);
        Assert.Equal(@"RunGate\", g_sRunGate_Directory);
        Assert.Equal(@"RunGate%d\", g_sRunGate_DirectoryEx);
    }

    [Fact]
    public void ResetForTests_ResetsAutoStartAndStopTimeouts()
    {
        g_dwStopTimeOut = 1;
        g_dwStopTick = 99;
        g_boEmbeddedWindow = true;
        g_nAutoStartDelayTime = 1;
        g_boAutoStartServer = true;
        g_nAutoStartTimeCount = 5;

        GShareGlobals.ResetForTests();

        Assert.Equal((uint)10000, g_dwStopTimeOut);
        Assert.Equal((uint)0, g_dwStopTick);
        Assert.False(g_boEmbeddedWindow);
        Assert.Equal(60, g_nAutoStartDelayTime);
        Assert.False(g_boAutoStartServer);
        Assert.Equal(0, g_nAutoStartTimeCount);
    }

    [Fact]
    public void BootButtonCaptions_MatchPas()
    {
        Assert.Equal("启动游戏服务器(&S)", g_sButtonStartGame);
        Assert.Equal("停止游戏服务器(&T)", g_sButtonStopGame);
        Assert.Equal("中止启动游戏服务器(&T)", g_sButtonStopStartGame);
        Assert.Equal("中止停止游戏服务器(&T)", g_sButtonStopStopGame);
    }

    [Fact]
    public void RunGateDirectoryEx_RetainsFormatPlaceholder()
    {
        // GMain.pas:2584 用 Format(g_sRunGate_DirectoryEx, [i]) 展开。
        Assert.Equal("RunGate1\\", GXX.Core.Rtl.DelphiFormat.Format(g_sRunGate_DirectoryEx, 1));
        Assert.Equal("RunGate7\\", GXX.Core.Rtl.DelphiFormat.Format(g_sRunGate_DirectoryEx, 7));
    }

    // ---------- GShare.pas 记录类型 ----------

    [Fact]
    public void TProgram_ShortStringFieldsClampToGbkCapacity()
    {
        // string[50]：60 个 ASCII 字符截断为 50；string[100]：120 → 100。
        var p = default(TProgram);
        p.sProgramFile = new string('A', 60);
        p.sDirectory = new string('B', 120);
        Assert.Equal(50, p.sProgramFile.Length);
        Assert.Equal(100, p.sDirectory.Length);
    }

    [Fact]
    public void TProgram_ShortStringClampCountsGbkBytesNotChars()
    {
        // 中文每字 2 字节：string[50] 只能容纳 25 个汉字。
        var p = default(TProgram);
        p.sProgramFile = new string('中', 30);
        Assert.Equal(25, p.sProgramFile.Length);
    }

    [Fact]
    public void TProgram_ShortStringClampKeepsValueWhenWithinCapacity()
    {
        var p = default(TProgram);
        p.sProgramFile = "DBServer.exe";
        p.sDirectory = @"DBServer\";
        Assert.Equal("DBServer.exe", p.sProgramFile);
        Assert.Equal(@"DBServer\", p.sDirectory);
    }

    [Fact]
    public void TProgram_CreateSetsFieldsInDeclarationOrder()
    {
        var p = TProgram.Create(true, true, false, 0, 326, "DBServer.exe", @"DBServer\");
        Assert.True(p.boGetStart);
        Assert.True(p.boMinimize);
        Assert.False(p.boReStart);
        Assert.Equal((byte)0, p.btStartStatus);
        Assert.Equal("DBServer.exe", p.sProgramFile);
        Assert.Equal(@"DBServer\", p.sDirectory);
        Assert.Equal(IntPtr.Zero, p.ProcessHandle);
        Assert.Equal(0UL, p.MainFormHandle);
        Assert.Equal(0, p.nMainFormX);
        Assert.Equal(326, p.nMainFormY);
    }

    [Fact]
    public void TRunGateInfo_ShortStringGateAddrClampsToFifteenBytes()
    {
        var g = default(TRunGateInfo);
        g.sGateAddr = "255.255.255.255.255";
        Assert.Equal(15, g.sGateAddr.Length);
        Assert.Equal("255.255.255.255", g.sGateAddr);
    }

    [Fact]
    public void TInstanceInfo_IsEightBytesPackedRecord()
    {
        // CheckPrevious.pas:12-15 packed record：THandle(4) + Integer(4)。
        Assert.Equal(8, Marshal.SizeOf<TInstanceInfo>());
        Assert.Equal(0, (int)Marshal.OffsetOf<TInstanceInfo>(nameof(TInstanceInfo.PreviousHandle)));
        Assert.Equal(4, (int)Marshal.OffsetOf<TInstanceInfo>(nameof(TInstanceInfo.RunCounter)));
    }

    [Fact]
    public void TCheckCode_DefaultsAreZero()
    {
        var c = default(TCheckCode);
        Assert.Equal((uint)0, c.dwThread0);
        Assert.Null(c.sThread0);
    }

    // ---------- SysUtils.BoolToStr（Delphi 7 默认重载） ----------

    [Fact]
    public void DelphiSystem_BoolToStr_ReturnsMinusOneAndZero()
    {
        Assert.Equal("-1", DelphiSystem.BoolToStr(true));
        Assert.Equal("0", DelphiSystem.BoolToStr(false));
    }

    [Fact]
    public void DelphiSystem_BoolToStr_UseBoolStrsOverloadReturnsText()
    {
        Assert.Equal("True", DelphiSystem.BoolToStr(true, true));
        Assert.Equal("False", DelphiSystem.BoolToStr(false, true));
        Assert.Equal("-1", DelphiSystem.BoolToStr(true, false));
    }

    [Fact]
    public void DelphiSystem_BoolToStr_DiffersFromHUtil32Version()
    {
        // 差异断言：HUtil32.BoolToStr 返回 "True"/"False"，与 SysUtils 版本不同；
        // GMain.pas 的 INI 落盘必须用 SysUtils 版本（否则 LoginSrv.ini 文本不一致）。
        Assert.Equal("True", GXX.Core.Util.HUtil32.BoolToStr(true));
        Assert.Equal("-1", DelphiSystem.BoolToStr(true));
        Assert.NotEqual(GXX.Core.Util.HUtil32.BoolToStr(true), DelphiSystem.BoolToStr(true));
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("  a  ", "a")]
    [InlineData("\t\r\nb", "b")]
    [InlineData("a b", "a b")]
    public void DelphiSystem_TrimMatchesDelphiWhitespaceSet(string input, string expected)
    {
        Assert.Equal(expected, DelphiSystem.Trim(input));
    }

    [Fact]
    public void DelphiStringReplace_RemoveBackslashMatchesCheckPreviousMappingName()
    {
        // CheckPrevious.pas:37 StringReplace(ParamStr(0), '\', '', [rfReplaceAll, rfIgnoreCase])
        Assert.Equal(@"D:MirServerGameCenter.exe",
            DelphiStringReplace.RemoveBackslash(@"D:\MirServer\GameCenter.exe"));
        Assert.Equal("", DelphiStringReplace.RemoveBackslash(""));
    }

    [Fact]
    public void DelphiSystem_IntToStrUsesInvariantCulture()
    {
        Assert.Equal("27201", DelphiSystem.IntToStr(27201));
        Assert.Equal("-1", DelphiSystem.IntToStr(-1));
    }
}
