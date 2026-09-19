using System;
using System.Collections.Generic;
using System.IO;
using GXX.GameCenter;
using Xunit;
using static GXX.GameCenter.GShareGlobals;

namespace GXX.GameCenter.Tests;

/// <summary>
/// GMain.pas 配置生成段（1619..2606 + 6604..6727）1:1 移植测试。
/// <para>
/// 期望文本与 _recon 诊断转储逐行一致（原文 TIniFile 语义：每批写入落盘后带一个空行，
/// 因此同一个节被分两批写入时中间会出现空行；文件末尾的换行由末尾的 "" 元素表示）。
/// </para>
/// </summary>
[Collection("GameCenterSequential")]
public sealed class GMainConfigTests : GameCenterTestBase
{
    // ==================================================================
    // GenDBServerConfig（GMain.pas:1632-1974）
    // ==================================================================

    [Fact]
    public void GenDBServerConfig_WritesExactDbSrcIniText()
    {
        SetupRunGates(3);
        GMainConfig.GenDBServerConfig();

        var expected = new List<string?>
        {
            "[Setup]",
            "ServerName=BmM2",
            "ServerAddr=127.0.0.1",
            "ServerPort=6000",
            "MapFile=" + g_sGameDirectory + @"Mir200\Envir\MapInfo.txt",
            "ViewHackMsg=0",
            "DoubleLineMode=0",
            "DynamicIPMode=0",
            "DisableAutoGame=0",
            "GateAddr=127.0.0.1",
            "GatePort=5100",
            // 原文如此（GMain.pas:1686-1705）：[Setup] 只写一次（同一批次连续写 14 个键），
            // 因此节内不出现中间空行。
            "UseSqliteDB=0",
            "DBName=HeroDB",
            "SqliteDBName=" + @"D:\MirServer\Mud2\DB\BmM2.db",
            "",
            "[Server]",
            "IDSAddr=127.0.0.1",
            "IDSPort=5600",
            "",
            "[DataSaveDB]",
            "DataSaveDBType=0",
            "DataSaveDBServer=",
            "DataSaveDBPort=3306",
            "DataSaveDBUser=",
            "DataSaveDBPassword=",
            "DataSaveDataBase=",
            "",
            "[DBClear]",
            "Interval=1000",
            "Level1=1",
            "Level2=7",
            "Level3=14",
            "Day1=7",
            "Day2=62",
            "Day3=124",
            "Month1=0",
            "Month2=0",
            "Month3=0",
            "",
            "[DB]",
            "Dir=" + g_sGameDirectory + @"DBServer\FDB\",
            "IdDir=" + g_sGameDirectory + @"DBServer\FDB\",
            "HumDir=" + g_sGameDirectory + @"DBServer\FDB\",
            "FeeDir=" + g_sGameDirectory + @"DBServer\FDB\",
            "BackupDir=" + g_sGameDirectory + @"DBServer\Backup\",
            "ConnectDir=" + g_sGameDirectory + @"DBServer\Connection\",
            "LogDir=" + g_sGameDirectory + @"DBServer\Log\",
        };
        AssertLines(Under("DBServer", "dbsrc.ini"), expected.ToArray());
    }

    [Fact]
    public void GenDBServerConfig_WritesAddrTableWithSingleLineWhenNotDoubleLine()
    {
        SetupRunGates(3);
        GMainConfig.GenDBServerConfig();

        AssertLines(Under("DBServer", "!addrtable.txt"), "127.0.0.1", "192.168.0.1");
    }

    [Fact]
    public void GenDBServerConfig_WritesAddrTableFourLinesWhenDoubleLine()
    {
        SetupRunGates(3);
        g_boDoubleLineMode = true;
        GMainConfig.GenDBServerConfig();

        AssertLines(Under("DBServer", "!addrtable.txt"),
            "127.0.0.1", "127.0.0.2", "192.168.0.1", "192.168.100.1");
    }

    [Fact]
    public void GenDBServerConfig_GateListPortsUseOnlyStartedGates()
    {
        // 只启动第 0、2、3 个网关（boGetStart），GetRunGatePort 按"已启动序号"取端口。
        g_nRunGate_Count = 4;
        g_RunGateInfo = new TRunGateInfo[GShareConst.MAXRUNGATECOUNT];
        g_RunGateInfo[0].boGetStart = true; g_RunGateInfo[0].nGatePort = 7200; g_RunGateInfo[0].nDBPort = 27201;
        g_RunGateInfo[2].boGetStart = true; g_RunGateInfo[2].nGatePort = 7400; g_RunGateInfo[2].nDBPort = 27401;
        g_RunGateInfo[3].boGetStart = true; g_RunGateInfo[3].nGatePort = 7500; g_RunGateInfo[3].nDBPort = 27501;

        GMainConfig.GenDBServerConfig();

        // 第 0 个已启动 = 索引 0；第 1 个已启动 = 索引 2；第 2 个已启动 = 索引 3。
        Assert.Equal(7200, GMainConfig.GetRunGatePort(0));
        Assert.Equal(7400, GMainConfig.GetRunGatePort(1));
        Assert.Equal(7500, GMainConfig.GetRunGatePort(2));
        Assert.Equal(0, GMainConfig.GetRunGatePort(3));
        Assert.Equal(27201, GMainConfig.GetRunGateDBPort(0));
        Assert.Equal(27401, GMainConfig.GetRunGateDBPort(1));
        Assert.Equal(27501, GMainConfig.GetRunGateDBPort(2));
        Assert.Equal(0, GMainConfig.GetRunGateDBPort(3));
    }

    [Theory]
    [InlineData(1, "127.0.0.1 192.168.0.1 7200")]
    [InlineData(2, "127.0.0.1 192.168.0.1 7200 192.168.0.1 7300")]
    [InlineData(3, "127.0.0.1 192.168.0.1 7200 192.168.0.1 7300 192.168.0.1 7400")]
    [InlineData(4, "127.0.0.1 192.168.0.1 7200 192.168.0.1 7300 192.168.0.1 7400 192.168.0.1 7500")]
    [InlineData(5, "127.0.0.1 192.168.0.1 7200 192.168.0.1 7300 192.168.0.1 7400 192.168.0.1 7500 192.168.0.1 7600")]
    [InlineData(6, "127.0.0.1 192.168.0.1 7200 192.168.0.1 7300 192.168.0.1 7400 192.168.0.1 7500 192.168.0.1 7600 192.168.0.1 7700")]
    [InlineData(7, "127.0.0.1 192.168.0.1 7200 192.168.0.1 7300 192.168.0.1 7400 192.168.0.1 7500 192.168.0.1 7600 192.168.0.1 7700 192.168.0.1 7800")]
    [InlineData(8, "127.0.0.1 192.168.0.1 7200 192.168.0.1 7300 192.168.0.1 7400 192.168.0.1 7500 192.168.0.1 7600 192.168.0.1 7700 192.168.0.1 7800 192.168.0.1 7900")]
    public void GenDBServerConfig_GateListLineMatchesRunGateCountCase(int count, string expected)
    {
        SetupRunGates(count);
        GMainConfig.GenDBServerConfig();

        // 原文顺序：TIniFile.Create 先把 "[GateDBPort0]" + 节内键（1..count=DBPort）写入文件，
        // IniGameConf.Free 之后再写地址行 —— 因此地址行前有一个空行（节尾空行）。
        var lines = new List<string?> { "[GateDBPort0]" };
        for (int i = 0; i < count; i++) lines.Add(null);   // 1=27201 … 仅校验行数
        lines.Add("");
        lines.Add(expected);
        AssertLines(Under("DBServer", "!GateList.ini"), lines.ToArray());
    }

    [Fact]
    public void GenDBServerConfig_GateListCountZero_LeavesSectionEmpty()
    {
        // 原文 case 无 0 分支：EraseSection 把节清空且不加任何行。
        SetupRunGates(0);
        File.WriteAllText(Under("DBServer", "!GateList.ini"), "[GateDBPort0]\r\n1=9\r\n", GXX.Core.EncodingInit.GBK);

        GMainConfig.GenDBServerConfig();

        AssertLines(Under("DBServer", "!GateList.ini"), Array.Empty<string?>());
    }

    [Fact]
    public void GenDBServerConfig_GateListDbPortsWrittenPerStartedGate()
    {
        SetupRunGates(3);
        GMainConfig.GenDBServerConfig();

        using var ini = new GameCenterIniFile(Under("DBServer", "!GateList.ini"));
        Assert.Equal(27201, ini.ReadInteger("GateDBPort0", "1", -1));
        Assert.Equal(27301, ini.ReadInteger("GateDBPort0", "2", -1));
        Assert.Equal(27401, ini.ReadInteger("GateDBPort0", "3", -1));
        Assert.Equal(-1, ini.ReadInteger("GateDBPort0", "4", -1));
    }

    [Fact]
    public void GenDBServerConfig_ServerInfoEmptyWhenNotDoubleLine()
    {
        SetupRunGates(3);
        GMainConfig.GenDBServerConfig();

        // 原文如此（GMain.pas:1933）：!serverinfo.txt 与 !addrtable.txt 内容完全相同
        // （SaveList 在 AddrTableFile 之后重新累积了同一组地址）。
        AssertLines(Under("DBServer", "!serverinfo.txt"), "127.0.0.1", "192.168.0.1");
    }

    [Fact]
    public void GenDBServerConfig_ServerInfoAddsNetComLineWhenDoubleLine()
    {
        SetupRunGates(3);
        g_boDoubleLineMode = true;
        GMainConfig.GenDBServerConfig();

        AssertLines(Under("DBServer", "!serverinfo.txt"),
            "127.0.0.1", "127.0.0.2", "192.168.0.1", "192.168.100.1");
    }

    [Fact]
    public void GenDBServerConfig_ServerInfoUsesExtIpAddrAsSecondToken()
    {
        // GetRunGatePort 拼串用的是 g_sExtIPaddr（非 g_sExtNetComIPaddr）。
        SetupRunGates(1);
        g_sExtIPaddr = "10.0.0.1";
        g_sExtNetComIPaddr = "10.0.0.2";
        g_boDoubleLineMode = true;

        GMainConfig.GenDBServerConfig();

        Assert.Equal(new[] { "127.0.0.1", "127.0.0.2", "10.0.0.1", "10.0.0.2" },
            ReadLinesGbk(Under("DBServer", "!serverinfo.txt")));
        // 双线模式追加第二行（g_sLocalIPaddr1 + g_sExtNetComIPaddr + 网关端口）。
        Assert.Equal(new[]
        {
            "[GateDBPort0]", "1=27201", "",
            "127.0.0.1 10.0.0.1 7200",
            "127.0.0.2 10.0.0.2 7200",
        }, ReadLinesGbk(Under("DBServer", "!GateList.ini")));
    }

    [Fact]
    public void GenDBServerConfig_CreatesDbSubDirectories()
    {
        SetupRunGates(3);
        GMainConfig.GenDBServerConfig();

        foreach (string rel in new[]
        {
            @"DBServer\FDB\", @"DBServer\Backup\", @"DBServer\Connection\", @"DBServer\Log\",
        })
            Assert.True(Directory.Exists(Under(rel)), rel);
    }

    [Fact]
    public void GenDBServerConfig_ExistingGateListIsRewrittenWithHeaderAndLine()
    {
        SetupRunGates(1);
        File.WriteAllText(Under("DBServer", "!GateList.ini"), "[GateDBPort0]\r\n1=9\r\n", GXX.Core.EncodingInit.GBK);

        GMainConfig.GenDBServerConfig();

        AssertLines(Under("DBServer", "!GateList.ini"),
            "[GateDBPort0]", "1=27201", "", "127.0.0.1 192.168.0.1 7200");
    }

    [Fact]
    public void GenDBServerConfig_SetsDateAndTimePickerViaSeam()
    {
        SetupRunGates(3);
        DateTime? stamp = null;
        GMainConfig.DtpDateTimeSetter = dt => stamp = dt;

        GMainConfig.GenDBServerConfig();

        Assert.NotNull(stamp);
        Assert.True((DateTime.Now - stamp!.Value).TotalMinutes < 1);
    }

    // ==================================================================
    // GenLoginServerConfig（GMain.pas:1976-2089）
    // ==================================================================

    [Fact]
    public void GenLoginServerConfig_WritesExactLogSrvIniText()
    {
        GMainConfig.GenLoginServerConfig();

        AssertLines(Under("LoginSrv", "Logsrv.ini"),
            "[Server]",
            "ReadyServers=0",
            // 原文如此：EnableMakingID/EnableTrial/TestServer 以 BoolToStr 写 '-1'/'0'
            "EnableMakingID=-1",
            "EnableTrial=0",
            "TestServer=-1",
            "DynamicIPMode=0",
            "GateAddr=0.0.0.0",
            "GatePort=5500",
            "ServerAddr=0.0.0.0",
            "ServerName=BmM2",
            "ServerPort=5600",
            "ControlPort=0",
            "",
            "[DB]",
            "IdDir=" + g_sGameDirectory + @"LoginSrv\IDDB\",
            "FeedIDList=" + g_sGameDirectory + @"LoginSrv\FeedIDList.txt",
            "FeedIPList=" + g_sGameDirectory + @"LoginSrv\FeedIPList.txt",
            "CountLogDir=" + g_sGameDirectory + @"LoginSrv\CountLog\",
            "WebLogDir=" + g_sGameDirectory + @"LoginSrv\GameWFolder\",
            "ChrLogDir=" + g_sGameDirectory + @"LoginSrv\ChrLog\",
            "IDLogDir=" + g_sGameDirectory + @"LoginSrv\IDLogDir\",
            "",
            "[DataSaveDB]",
            "DataSaveDBType=0",
            "DataSaveDBServer=",
            "DataSaveDBPort=3306",
            "DataSaveDBUser=",
            "DataSaveDBPassword=",
            "DataSaveDataBase=",
            "");
    }

    [Theory]
    // 原文四分支（GMain.pas:2024-2047），全部在 g_boDoubleLineMode=False 下
    [InlineData(false, false, "BmM2 Title1 127.0.0.1 127.0.0.1 192.168.0.1:7100")]
    [InlineData(true, true, "BmM2 Title1 127.0.0.1 127.0.0.1 192.168.0.1:7100 192.168.0.1:7101")]
    [InlineData(true, false, "BmM2 Title1 127.0.0.1 127.0.0.1 192.168.0.1:7100")]
    [InlineData(false, true, "BmM2 Title1 127.0.0.1 127.0.0.1 192.168.0.1:7101")]
    public void GenLoginServerConfig_AddrTableTitleLineVariants(bool selGate, bool selGate1, string expected)
    {
        SelGate.boGetStart = selGate;
        SelGate1.boGetStart = selGate1;

        GMainConfig.GenLoginServerConfig();

        AssertLines(Under("LoginSrv", "!addrtable.txt"), expected);
    }

    [Fact]
    public void GenLoginServerConfig_AddrTableTitle2LineForDoubleLineMode()
    {
        SelGate.boGetStart = true;
        SelGate1.boGetStart = true;
        g_boDoubleLineMode = true;

        GMainConfig.GenLoginServerConfig();

        AssertLines(Under("LoginSrv", "!addrtable.txt"),
            "BmM2 Title1 127.0.0.1 127.0.0.1 192.168.0.1:7100 192.168.0.1:7101",
            "BmM2 Title2 127.0.0.2 127.0.0.2 192.168.100.1:7100 192.168.0.1:7101");
    }

    [Fact]
    public void GenLoginServerConfig_ServerAddrFileSingleLineWhenNotDoubleLine()
    {
        GMainConfig.GenLoginServerConfig();
        AssertLines(Under("LoginSrv", "!serveraddr.txt"), "127.0.0.1", "192.168.0.1");
    }

    [Fact]
    public void GenLoginServerConfig_ServerAddrFileFourLinesWhenDoubleLine()
    {
        g_boDoubleLineMode = true;
        GMainConfig.GenLoginServerConfig();
        AssertLines(Under("LoginSrv", "!serveraddr.txt"),
            "127.0.0.1", "127.0.0.2", "192.168.0.1", "192.168.100.1");
    }

    [Fact]
    public void GenLoginServerConfig_UserLimitFileUsesLimitOnlineUser()
    {
        g_nLimitOnlineUser = 4321;
        GMainConfig.GenLoginServerConfig();
        AssertLines(Under("LoginSrv", "!UserLimit.txt"), "BmM2 BmM2 4321");
    }

    [Fact]
    public void GenLoginServerConfig_CreatesThreeLogDirectories()
    {
        GMainConfig.GenLoginServerConfig();
        Assert.True(Directory.Exists(Under("LoginSrv", "IDDB")));
        Assert.True(Directory.Exists(Under("LoginSrv", "CountLog")));
        Assert.True(Directory.Exists(Under("LoginSrv", "GameWFolder")));
    }

    // ==================================================================
    // GenLogServerConfig（GMain.pas:2091-2113）
    // ==================================================================

    [Fact]
    public void GenLogServerConfig_WritesExactTextAndCreatesBaseDir()
    {
        GMainConfig.GenLogServerConfig();

        AssertLines(Under("LogServer", "LogData.ini"),
            "[Setup]",
            "ServerName=BmM2",
            "Port=10000",
            "BaseDir=" + g_sGameDirectory + @"LogServer\BaseDir\",
            "");
        Assert.True(Directory.Exists(Under("LogServer", "BaseDir")));
    }

    // ==================================================================
    // GenM2ServerConfig（GMain.pas:2115-2245）
    // ==================================================================

    [Fact]
    public void GenM2ServerConfig_WritesExactSetupTxtText()
    {
        GMainConfig.GenM2ServerConfig();

        string S = g_sGameDirectory + @"Mir200\";
        AssertLines(Under("Mir200", "!setup.txt"),
            "[Server]",
            "ServerName=BmM2",
            "ServerNumber=0",
            "ServerIndex=0",
            "EditionId=0",
            "AreaId=0",
            "VentureServer=0",
            "TestServer=-1",
            "TestLevel=1",
            "TestGold=0",
            "TestServerUserLimit=10000",
            "ServiceMode=0",
            "NonPKServer=0",
            "DBAddr=127.0.0.1",
            "DBPort=6000",
            "IDSAddr=127.0.0.1",
            "IDSPort=5600",
            "MsgSrvAddr=0.0.0.0",
            "MsgSrvPort=4900",
            "LogServerAddr=127.0.0.1",
            "LogServerPort=10000",
            "GateAddr=0.0.0.0",
            "GatePort=5000",
            "UseSqliteDB=0",
            "DBName=HeroDB",
            "SqliteDBName=" + @"D:\MirServer\Mud2\DB\BmM2.db",
            "UserFull=10000",
            "",
            "[Share]",
            "BaseDir=" + S + @"Share\",
            "GuildDir=" + S + @"GuildBase\Guilds\",
            "GuildFile=" + S + @"GuildBase\Guildlist.txt",
            "VentureDir=" + S + @"ShareV\",
            "ConLogDir=" + S + @"ConLog\",
            "LogDir=" + S + @"Log\",
            "PlugDir=" + S,
            "BoxsDir=" + S + @"Envir\Boxs\",
            "CastleDir=" + S + @"Castle\",
            "EnvirDir=" + S + @"Envir\",
            "MapDir=" + S + @"Map\",
            "NoticeDir=" + S + @"Notice\",
            "CastleFile=" + S + @"Castle\List.txt",
            "",
            "[DataSaveDB]",
            "DataSaveDBType=0",
            "DataSaveDBServer=",
            "DataSaveDBPort=3306",
            "DataSaveDBUser=",
            // 原文如此（GMain.pas:2180）：键名尾部多一个空格，与 DBServer/LoginSrv 侧不一致。
            "DataSaveDBPassword =",
            "DataSaveDataBase=",
            "",
            "");
    }

    [Fact]
    public void GenM2ServerConfig_PasswordKeyCarriesTrailingSpace()
    {
        g_sDataSaveDBPassword = "pw123";
        GMainConfig.GenM2ServerConfig();

        string text = ReadGbk(Under("Mir200", "!setup.txt"));
        Assert.Contains("DataSaveDBPassword =pw123\r\n", text);
        Assert.DoesNotContain("DataSaveDBPassword=pw123", text);

        using var ini = new GameCenterIniFile(Under("Mir200", "!setup.txt"));
        // 原文如此（GMain.pas:2180）：键名尾部空格只影响**写盘文本**；TIniFile 读回时 Ident 会被 Trim，
        // 因此必须用 'DataSaveDBPassword' 读取，带尾空格的查询键反而不命中。
        Assert.Equal("pw123", ini.ReadString("DataSaveDB", "DataSaveDBPassword", ""));
        Assert.Equal("", ini.ReadString("DataSaveDB", "DataSaveDBPassword ", ""));
    }

    [Fact]
    public void GenM2ServerConfig_CompanionFilesContainGmAndLocalIp()
    {
        GMainConfig.GenM2ServerConfig();

        AssertLines(Under("Mir200", "!abuse.txt"), "GM");
        AssertLines(Under("Mir200", "!runaddr.txt"), "127.0.0.1");
        AssertLines(Under("Mir200", "!servertable.txt"), "127.0.0.1");
    }

    [Fact]
    public void GenM2ServerConfig_CreatesNineShareDirectories()
    {
        GMainConfig.GenM2ServerConfig();

        foreach (string rel in new[]
        {
            @"Mir200\Share\", @"Mir200\GuildBase\Guilds\", @"Mir200\ShareV\", @"Mir200\ConLog\",
            @"Mir200\Log\", @"Mir200\Castle\", @"Mir200\Envir\", @"Mir200\Map\", @"Mir200\Notice\",
        })
            Assert.True(Directory.Exists(Under(rel)), rel);
    }

    // ==================================================================
    // GenLoginGateConfig（GMain.pas:2247-2271）
    // ==================================================================

    [Fact]
    public void GenLoginGateConfig_WritesExactText()
    {
        GMainConfig.GenLoginGateConfig();

        AssertLines(Under("LoginGate", "Config.ini"),
            "[LoginGate]",
            "Title=BmM2",
            "GateAddr=0.0.0.0",
            "ServerAddr=127.0.0.1",
            "ServerPort=5500",
            "GatePort=7000",
            "Count=1",
            "ServerAddr1=127.0.0.1",
            "ServerPort1=5500",
            "GatePort1=7000",
            "");
    }

    [Fact]
    public void GenLoginGateConfig_ServerPortFollowsLoginServerGatePortNotGateServerPort()
    {
        // 差异断言：把两个候选值改成不同，验证取的是 g_nLoginServer_GatePort。
        g_nLoginServer_GatePort = 5501;
        g_nLoginGate_ServerPort = 5599;

        GMainConfig.GenLoginGateConfig();

        using var ini = new GameCenterIniFile(Under("LoginGate", "Config.ini"));
        Assert.Equal(5501, ini.ReadInteger("LoginGate", "ServerPort", -1));
        Assert.Equal(5501, ini.ReadInteger("LoginGate", "ServerPort1", -1));
    }

    // ==================================================================
    // GenSelGateConfig（GMain.pas:2273-2295）
    // ==================================================================

    [Fact]
    public void GenSelGateConfig_WritesExactText()
    {
        GMainConfig.GenSelGateConfig();

        AssertLines(Under("SelGate", "Config.ini"),
            "[SelGate]",
            "Title=BmM2",
            "ServerAddr=127.0.0.1",
            "ServerPort=5100",
            "GateAddr=0.0.0.0",
            "GatePort=7100",
            "");
    }

    [Fact]
    public void GenSelGateConfig_ServerPortFollowsDbServerGatePort()
    {
        g_nDBServer_Config_GatePort = 5199;
        GMainConfig.GenSelGateConfig();

        using var ini = new GameCenterIniFile(Under("SelGate", "Config.ini"));
        Assert.Equal(5199, ini.ReadInteger("SelGate", "ServerPort", -1));
    }

    // ==================================================================
    // GenMutLoginGateConfig（GMain.pas:2297-2350）
    // ==================================================================

    [Fact]
    public void GenMutLoginGateConfig_NotDoubleLine_UsesAllIpAddr()
    {
        GMainConfig.GenMutLoginGateConfig(0);

        using var ini = new GameCenterIniFile(Under("LoginGate", "Config.ini"));
        Assert.Equal("0.0.0.0", ini.ReadString("LoginGate", "GateAddr", ""));
        Assert.Equal(7000, ini.ReadInteger("LoginGate", "GatePort", -1));
        Assert.Equal("127.0.0.1", ini.ReadString("LoginGate", "ServerAddr", ""));
        // 原文注释块屏蔽了 Count/ServerAddr1/ServerPort1/GatePort1
        Assert.Equal(-1, ini.ReadInteger("LoginGate", "Count", -1));
    }

    [Fact]
    public void GenMutLoginGateConfig_DoubleLineIndex0And1UseDifferentAddresses()
    {
        g_boDoubleLineMode = true;

        GMainConfig.GenMutLoginGateConfig(0);
        using (var ini = new GameCenterIniFile(Under("LoginGate", "Config.ini")))
        {
            Assert.Equal("192.168.0.1", ini.ReadString("LoginGate", "GateAddr", ""));
            Assert.Equal("127.0.0.1", ini.ReadString("LoginGate", "ServerAddr", ""));
        }

        GMainConfig.GenMutLoginGateConfig(1);
        using (var ini = new GameCenterIniFile(Under("LoginGate", "Config.ini")))
        {
            Assert.Equal("192.168.100.1", ini.ReadString("LoginGate", "GateAddr", ""));
            Assert.Equal("127.0.0.2", ini.ReadString("LoginGate", "ServerAddr", ""));
        }
    }

    [Fact]
    public void GenMutLoginGateConfig_DoubleLineOutOfRangeIndex_LeavesLocalsEmpty()
    {
        // 差异说明：原文 case 无 2+ 分支，Delphi 局部变量保持上一次调用的栈值（未初始化）；
        // 托管侧为类型默认值（""/0），本测试锁定该差异。
        g_boDoubleLineMode = true;
        GMainConfig.GenMutLoginGateConfig(5);

        using var ini = new GameCenterIniFile(Under("LoginGate", "Config.ini"));
        Assert.Equal("", ini.ReadString("LoginGate", "GateAddr", "X"));
        Assert.Equal(0, ini.ReadInteger("LoginGate", "GatePort", -1));
    }

    // ==================================================================
    // GetMutLoginGateZero（GMain.pas:2353-2367）
    // ==================================================================

    [Fact]
    public void GetMutLoginGateZero_WritesCountZeroIntoLoginGateConfigUnderSelGateDir()
    {
        // 原文如此（GMain.pas:2358+2364）：目录用 g_sSelGate_Directory，文件名用 g_sLoginGate_ConfigFile。
        GMainConfig.GetMutLoginGateZero();

        string path = Path.Combine(Dir, "SelGate", "Config.ini");
        Assert.True(File.Exists(path));
        AssertLines(path, "[LoginGate]", "Count=0", "");
    }

    // ==================================================================
    // GenMutSelGateConfigEx（GMain.pas:2369-2404）
    // ==================================================================

    [Fact]
    public void GenMutSelGateConfigEx_BothGatesOff_WritesCountZeroOnly()
    {
        SelGate.boGetStart = false;
        SelGate1.boGetStart = false;

        GMainConfig.GenMutSelGateConfigEx();

        AssertLines(Under("SelGate", "Config.ini"), "[SelGates_iocp]", "Count=0", "");
    }

    [Fact]
    public void GenMutSelGateConfigEx_BothGatesOn_WritesTwoEntriesThenCount()
    {
        SelGate.boGetStart = true;
        SelGate1.boGetStart = true;

        GMainConfig.GenMutSelGateConfigEx();

        AssertLines(Under("SelGate", "Config.ini"),
            "[SelGates_iocp]",
            "ServerAddr1=127.0.0.1",
            "ServerPort1=5100",
            "GatePort1=7100",
            "ServerAddr2=127.0.0.1",
            "ServerPort2=5100",
            "GatePort2=7101",
            "Count=2",
            "");
    }

    [Fact]
    public void GenMutSelGateConfigEx_OnlySecondGateOn_WritesSingleEntryIndexedOne()
    {
        SelGate.boGetStart = false;
        SelGate1.boGetStart = true;

        GMainConfig.GenMutSelGateConfigEx();

        AssertLines(Under("SelGate", "Config.ini"),
            "[SelGates_iocp]",
            "ServerAddr1=127.0.0.1",
            "ServerPort1=5100",
            "GatePort1=7101",
            "Count=1",
            "");
    }

    // ==================================================================
    // GetMutSelGateZero（GMain.pas:2406-2420）
    // ==================================================================

    [Fact]
    public void GetMutSelGateZero_WritesCountZeroAndKeepsOtherSections()
    {
        Directory.CreateDirectory(Under("SelGate"));
        File.WriteAllText(Under("SelGate", "Config.ini"), "[Other]\r\nK=V\r\n", GXX.Core.EncodingInit.GBK);

        GMainConfig.GetMutSelGateZero();

        using var ini = new GameCenterIniFile(Under("SelGate", "Config.ini"));
        Assert.Equal(0, ini.ReadInteger("SelGates_iocp", "Count", -1));
        Assert.Equal("V", ini.ReadString("Other", "K", ""));
    }

    // ==================================================================
    // GenMutSelGateConfig（GMain.pas:2422-2458）
    // ==================================================================

    [Fact]
    public void GenMutSelGateConfig_Index0UsesFirstGateAddrAndPort()
    {
        GMainConfig.GenMutSelGateConfig(0);

        AssertLines(Under("SelGate", "Config.ini"),
            "[SelGate]",
            "Title=BmM2",
            "ServerAddr=127.0.0.1",
            "ServerPort=5100",
            "GateAddr=0.0.0.0",
            "GatePort=7100",
            "");
    }

    [Fact]
    public void GenMutSelGateConfig_Index1UsesSecondGateAddrAndPort()
    {
        g_sSelGate_GateAddr1 = "10.0.0.9";
        GMainConfig.GenMutSelGateConfig(1);

        using var ini = new GameCenterIniFile(Under("SelGate", "Config.ini"));
        Assert.Equal("10.0.0.9", ini.ReadString("SelGate", "GateAddr", ""));
        Assert.Equal(7101, ini.ReadInteger("SelGate", "GatePort", -1));
    }

    [Fact]
    public void GenMutSelGateConfig_OutOfRangeIndex_LeavesLocalsEmpty()
    {
        GMainConfig.GenMutSelGateConfig(9);

        using var ini = new GameCenterIniFile(Under("SelGate", "Config.ini"));
        Assert.Equal("", ini.ReadString("SelGate", "GateAddr", "X"));
        Assert.Equal(0, ini.ReadInteger("SelGate", "GatePort", -1));
    }

    // ==================================================================
    // GetMutRunGateConfing / Ex / Zero（GMain.pas:2460-2535）
    // ==================================================================

    [Fact]
    public void GetMutRunGateConfing_WritesPerGateConfig()
    {
        SetupRunGates(3);

        GMainConfig.GetMutRunGateConfing(2);

        AssertLines(Under("RunGate", "Config.ini"),
            "[GameGate]",
            "Title=BmM2(7400)",
            "ServerAddr=127.0.0.1",
            "ServerPort=5000",
            "GateAddr=0.0.0.0",
            "GatePort=7400",
            "DBPort=27401",
            "");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(8)]
    public void GetMutRunGateConfing_OutOfRangeIndex_WritesNothing(int index)
    {
        SetupRunGates(3);

        GMainConfig.GetMutRunGateConfing(index);

        Assert.False(File.Exists(Under("RunGate", "Config.ini")));
    }

    [Fact]
    public void GetMutRunGateConfingEx_WritesPortListAndCount()
    {
        g_RunGateInfo = new TRunGateInfo[GShareConst.MAXRUNGATECOUNT];
        for (int i = 0; i < GShareConst.MAXRUNGATECOUNT; i++)
        {
            g_RunGateInfo[i].boGetStart = i < 3;
            g_RunGateInfo[i].nGatePort = 7200 + i * 100;
        }

        GMainConfig.GetMutRunGateConfingEx();

        AssertLines(Under("RunGate", "Config.ini"),
            "[GameGates]",
            "Port1=7200",
            "Port2=7300",
            "Port3=7400",
            "Count=3",
            "",
            "[GameGate]",
            "Title=(7200,7300,7400)",
            "ServerAddr=127.0.0.1",
            "ServerPort=5000",
            "GateAddr=0.0.0.0",
            // 原文如此（GMain.pas:2516）：DBPort 写在 [GameGate] 节
            "DBPort=27201",
            "");
    }

    [Fact]
    public void GetMutRunGateConfingEx_NoStartedGate_TitleEmpty()
    {
        SetupRunGates(0);

        GMainConfig.GetMutRunGateConfingEx();

        using var ini = new GameCenterIniFile(Under("RunGate", "Config.ini"));
        Assert.Equal("", ini.ReadString("GameGate", "Title", "X"));
        Assert.Equal(0, ini.ReadInteger("GameGates", "Count", -1));
        Assert.Equal(-1, ini.ReadInteger("GameGates", "Port1", -1));
    }

    [Fact]
    public void GetMutRunGateZero_WritesCountZero()
    {
        GMainConfig.GetMutRunGateZero();
        AssertLines(Under("RunGate", "Config.ini"), "[GameGates]", "Count=0", "");
    }

    // ==================================================================
    // GenRunGateConfig（GMain.pas:2537-2606）
    // ==================================================================

    [Fact]
    public void GenRunGateConfig_NotMultiThread_WritesSingleThreadDbPort()
    {
        SetupRunGates(3);
        g_boRunGate_GetMultiThread = false;

        GMainConfig.GenRunGateConfig();

        AssertLines(Under("RunGate", "Config.ini"),
            "[GameGate]",
            "Title=BmM2",
            "ServerAddr=127.0.0.1",
            "ServerPort=5000",
            "GateAddr=0.0.0.0",
            "GatePort=7200",
            "DBPort=27201",
            "");
        AssertLines(Under("RunGate", "!addrtable.txt"), "127.0.0.1");
        Assert.Equal(3, g_nRunGate_Count);
    }

    [Fact]
    public void GenRunGateConfig_MultiThread_UsesMultiThreadDbPort()
    {
        SetupRunGates(3);
        g_boRunGate_GetMultiThread = true;
        g_nRunGateDBPort_MulThread = 28201;

        GMainConfig.GenRunGateConfig();

        using var ini = new GameCenterIniFile(Under("RunGate", "Config.ini"));
        Assert.Equal(28201, ini.ReadInteger("GameGate", "DBPort", -1));
    }

    [Fact]
    public void GenRunGateConfig_CountIsIncrementedEvenWhenDirectoryMissing()
    {
        // 只有 RunGate1\ 目录存在（RunGate2\ 缺失）→ 计数仍加，但不写文件。
        SetupRunGates(3);
        Directory.CreateDirectory(Under("RunGate1"));
        GMainConfig.GenRunGateConfig();

        Assert.Equal(3, g_nRunGate_Count);
        Assert.False(File.Exists(Path.Combine(Dir, "RunGate2", "Config.ini")));
        Assert.False(File.Exists(Path.Combine(Dir, "RunGate3", "Config.ini")));

        using var ini = new GameCenterIniFile(Path.Combine(Dir, "RunGate1", "Config.ini"));
        Assert.Equal(7300, ini.ReadInteger("GameGate", "GatePort", -1));
        Assert.Equal(27301, ini.ReadInteger("GameGate", "DBPort", -1));
    }

    [Fact]
    public void GenRunGateConfig_MultiThreadSecondaryGatesWriteDbPortIntoGameGatesSection()
    {
        // 原文如此（GMain.pas:2597）：多线程分支的 DBPort 写在 'GameGates' 节，与单线程分支不一致。
        SetupRunGates(3);
        g_boRunGate_GetMultiThread = true;
        g_nRunGateDBPort_MulThread = 28201;
        Directory.CreateDirectory(Under("RunGate1"));

        GMainConfig.GenRunGateConfig();

        using var ini = new GameCenterIniFile(Path.Combine(Dir, "RunGate1", "Config.ini"));
        Assert.Equal(28201, ini.ReadInteger("GameGates", "DBPort", -1));
        Assert.Equal(-1, ini.ReadInteger("GameGate", "DBPort", -1));
    }

    [Fact]
    public void GenRunGateConfig_OnlyFirstGateConfiguredWhenCountOne()
    {
        SetupRunGates(1);

        GMainConfig.GenRunGateConfig();

        Assert.Equal(1, g_nRunGate_Count);
        Assert.False(File.Exists(Path.Combine(Dir, "RunGate1", "Config.ini")));
    }

    // ==================================================================
    // GenGameConfig（GMain.pas:1619-1630）编排
    // ==================================================================

    [Fact]
    public void GenGameConfig_InvokesAllSevenGeneratorsPlusBackup()
    {
        SetupRunGates(3);
        var manager = new MemoryBackUpManager();
        g_BackUpManager = manager;
        g_sOldGameDirectory = @"D:\Old\";
        manager.Add(new MemoryBackUpTask
        {
            SourceDirectory = @"D:\Old\DBServer\FDB\",
            DestDirectory = @"D:\Old\Backup\",
        });
        int refViewCalled = 0;
        GMainConfig.RefBackListToView = () => refViewCalled++;

        GMainConfig.GenGameConfig();

        Assert.True(File.Exists(Under("DBServer", "dbsrc.ini")));
        Assert.True(File.Exists(Under("LoginSrv", "Logsrv.ini")));
        Assert.True(File.Exists(Under("LogServer", "LogData.ini")));
        Assert.True(File.Exists(Under("Mir200", "!setup.txt")));
        Assert.True(File.Exists(Under("LoginGate", "Config.ini")));
        Assert.True(File.Exists(Under("SelGate", "Config.ini")));
        Assert.True(File.Exists(Under("RunGate", "Config.ini")));
        // GenBackupConfig 是第八个生成器：写了 BackList.txt 且回调了 RefBackListToView
        Assert.True(File.Exists(GMainConfig.BackListFileName()));
        Assert.True(refViewCalled >= 1);
        AssertLines(GMainConfig.BackListFileName(),
            "[0]",
            "Source=" + Dir + @"\DBServer\FDB\",
            "Save=" + Dir + @"\Backup\",
            "Hour=0",
            "Min=0",
            "BackMode=0",
            "GetBack=0",
            "IsCompress=0",
            "");
    }

    [Fact]
    public void GenGameConfig_BackupBranchWithNoTasks_ReportsConfigError()
    {
        // 空备份清单 + g_sOldGameDirectory 为空 → 原文 else 分支：提示 + RefBackListToView。
        SetupRunGates(3);
        g_BackUpManager = new MemoryBackUpManager();
        g_sOldGameDirectory = "";
        int refView = 0;
        GMainConfig.RefBackListToView = () => refView++;

        GMainConfig.GenGameConfig();

        Assert.Equal(1, refView);
        Assert.False(File.Exists(GMainConfig.BackListFileName()));
    }

    // ==================================================================
    // GenBackupConfig（GMain.pas:6604-6727）
    // ==================================================================

    [Fact]
    public void GenBackupConfig_ReprefixesOldGameDirectoryAndWritesBackList()
    {
        var manager = new MemoryBackUpManager();
        g_BackUpManager = manager;
        g_sOldGameDirectory = @"D:\Old\";
        manager.Add(new MemoryBackUpTask
        {
            SourceDirectory = @"D:\Old\DBServer\FDB\",
            DestDirectory = @"D:\Old\Backup\",
            Hour = 3,
            Min = 30,
            Mode = 1,
            Start = true,
            IsCompress = false,
        });
        GMainConfig.RefBackListToView = () => { };

        GMainConfig.GenBackupConfig();

        AssertLines(GMainConfig.BackListFileName(),
            "[0]",
            "Source=" + Dir + @"\DBServer\FDB\",
            "Save=" + Dir + @"\Backup\",
            "Hour=3",
            "Min=30",
            "BackMode=1",
            "GetBack=1",
            "IsCompress=0",
            "");
    }

    [Fact]
    public void GenBackupConfig_OldDirectoryEmpty_ShowsMessageAndSkipsWrite()
    {
        g_BackUpManager = new MemoryBackUpManager();
        g_BackUpManager.Add(new MemoryBackUpTask { SourceDirectory = "x", DestDirectory = "y" });
        g_sOldGameDirectory = "";
        int refView = 0;
        GMainConfig.RefBackListToView = () => refView++;

        GMainConfig.GenBackupConfig();

        Assert.Equal("请检查备份路径配置是否正确", GameCenterDialogs.LastMessage);
        Assert.Equal(1, refView);
        Assert.False(File.Exists(GMainConfig.BackListFileName()));
    }

    [Fact]
    public void GenBackupConfig_SourceNotUnderOldDirectory_ShowsMessage()
    {
        g_BackUpManager = new MemoryBackUpManager();
        g_BackUpManager.Add(new MemoryBackUpTask
        {
            SourceDirectory = @"E:\Other\FDB\",
            DestDirectory = @"E:\Other\Backup\",
        });
        g_sOldGameDirectory = @"D:\Old\";
        GMainConfig.RefBackListToView = () => { };

        GMainConfig.GenBackupConfig();

        Assert.Equal("请检查备份路径配置是否正确", GameCenterDialogs.LastMessage);
        Assert.False(File.Exists(GMainConfig.BackListFileName()));
    }

    [Fact]
    public void GenBackupConfig_DestNotUnderOldDirectory_ShowsMessage()
    {
        g_BackUpManager = new MemoryBackUpManager();
        g_BackUpManager.Add(new MemoryBackUpTask
        {
            SourceDirectory = @"D:\Old\FDB\",
            DestDirectory = @"E:\Other\Backup\",
        });
        g_sOldGameDirectory = @"D:\Old\";
        GMainConfig.RefBackListToView = () => { };

        GMainConfig.GenBackupConfig();

        Assert.Equal("请检查备份路径配置是否正确", GameCenterDialogs.LastMessage);
    }

    [Fact]
    public void GenBackupConfig_ReprefixesClearServerListsAndAddsToControls()
    {
        g_BackUpManager = new MemoryBackUpManager();
        g_sOldGameDirectory = @"D:\Old\";
        g_IniConf.WriteInteger("ClearServer", "MyGetTxtNum", 2);
        g_IniConf.WriteString("ClearServer", "MyGetTxt0", @"D:\Old\a.txt");
        g_IniConf.WriteString("ClearServer", "MyGetTxt1", @"E:\untouched.txt");
        g_IniConf.WriteInteger("ClearServer", "MyGetFileNum", 0);
        g_IniConf.WriteInteger("ClearServer", "MyGetDirNum", 0);

        var added = new List<string>();
        GMainHelpers.ListAddHandler = (src, s) => added.Add(src + ":" + s);
        int clears = 0;
        GMainHelpers.ListClearHandler = _ => clears++;
        GMainConfig.RefBackListToView = () => { };

        GMainConfig.GenBackupConfig();

        Assert.Equal(1, clears);
        Assert.Equal(2, added.Count);
        Assert.Equal("MyGetTxt:" + Dir + @"\a.txt", added[0]);
        // 不以旧目录开头则原样保留
        Assert.Equal(@"MyGetTxt:E:\untouched.txt", added[1]);
    }

    [Fact]
    public void GenBackupConfig_ZeroCounts_DoNotTouchLists()
    {
        g_BackUpManager = new MemoryBackUpManager();
        g_sOldGameDirectory = @"D:\Old\";
        g_IniConf.WriteInteger("ClearServer", "MyGetTxtNum", 0);
        g_IniConf.WriteInteger("ClearServer", "MyGetFileNum", 0);
        g_IniConf.WriteInteger("ClearServer", "MyGetDirNum", 0);

        int calls = 0;
        GMainHelpers.ListClearHandler = _ => calls++;
        GMainHelpers.ListAddHandler = (_, _) => calls++;
        GMainConfig.RefBackListToView = () => { };

        GMainConfig.GenBackupConfig();

        Assert.Equal(0, calls);
    }

    [Fact]
    public void GenBackupConfig_MultipleTasksPreserveSectionOrder()
    {
        var manager = new MemoryBackUpManager();
        g_BackUpManager = manager;
        g_sOldGameDirectory = @"D:\Old\";
        manager.Add(new MemoryBackUpTask { SourceDirectory = @"D:\Old\A\", DestDirectory = @"D:\Old\B\" });
        manager.Add(new MemoryBackUpTask { SourceDirectory = @"D:\Old\C\", DestDirectory = @"D:\Old\D\" });
        GMainConfig.RefBackListToView = () => { };

        GMainConfig.GenBackupConfig();

        AssertLines(GMainConfig.BackListFileName(),
            "[0]",
            "Source=" + Dir + @"\A\",
            "Save=" + Dir + @"\B\",
            "Hour=0",
            "Min=0",
            "BackMode=0",
            "GetBack=0",
            "IsCompress=0",
            "",
            "[1]",
            "Source=" + Dir + @"\C\",
            "Save=" + Dir + @"\D\",
            "Hour=0",
            "Min=0",
            "BackMode=0",
            "GetBack=0",
            "IsCompress=0",
            "");
    }
}
