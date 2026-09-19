using System.IO;
using System.Text;
using GXX.Core;
using GXX.SelGate;
using Xunit;

namespace GXX.SelGate.Tests;

/// <summary>
/// ConfigManager.pas（SelGate 特有 INI 配置）→ CConfigMgr 的 1:1 断言。
/// 覆盖：构造默认值（:54-83）、Read*/写回语义（:91-134）、LoadConfig 两分支（:136-200）、
/// SaveConfig nType=0/1/2 的**完整落盘文本与键序**（:202-247）。
/// </summary>
public class SelGateConfigTests
{
    private static string ReadIni(string path) => File.ReadAllText(path, EncodingInit.GBK);

    // =====================================================================================
    // 1. 构造默认值
    // =====================================================================================

    [Fact]
    public void Ctor_DefaultValues_MatchDelphiInitializer()
    {
        using var dir = new TempDir();
        var cfg = new CConfigMgr(dir.File("Config.ini"));

        Assert.Equal("", cfg.m_szTitle);        // :60 原文如此：被注释掉的 '角色网关' 不再赋值
        Assert.Equal(3, cfg.m_nShowLogLevel);   // :61
        Assert.Equal(1, cfg.m_nGateCount);      // :63

        // :66-68 全部 32 个槽位（MAX_SERVER_COUNT = 32，AcceptExWorkedThread.pas:26）
        for (int i = 1; i <= CConfigMgr.MAX_SERVER_COUNT; i++)
        {
            Assert.Equal("127.0.0.1", cfg.m_xGameGateList[i].sServerAdress);
            Assert.Equal(5100, cfg.m_xGameGateList[i].nServerPort);
            Assert.Equal(7100 + i - 1, cfg.m_xGameGateList[i].nGatePort);
        }

        Assert.True(cfg.m_fCheckNewIDOfIP);      // :71
        Assert.True(cfg.m_fCheckNullSession);    // :72
        Assert.False(cfg.m_fOverSpeedSendBack);  // :73
        Assert.False(cfg.m_fDefenceCCPacket);    // :74
        Assert.False(cfg.m_fKickOverSpeed);      // :75
        Assert.True(cfg.m_fKickOverPacketSize);  // :76

        Assert.Equal(700, cfg.m_nNomClientPacketSize);   // :78
        Assert.Equal(20, cfg.m_nMaxConnectOfIP);         // :79
        Assert.Equal(5, cfg.m_nCheckNewIDOfIP);          // :80
        Assert.Equal(180 * 1000, cfg.m_nClientTimeOutTime); // :81
        Assert.Equal(20, cfg.m_nMaxClientPacketCount);   // :82

        // :37 m_tBlockIPMethod 未赋值 ⇒ 序型首项 mDisconnect = 0
        Assert.Equal((int)TBlockIPMethod.mDisconnect, cfg.m_tBlockIPMethod);
    }

    // =====================================================================================
    // 2. ReadString/ReadInteger/ReadBool 的"缺键即回写默认值"语义
    // =====================================================================================

    [Fact]
    public void ReadString_MissingKey_WritesDefaultBackToIni()
    {
        using var dir = new TempDir();
        var cfg = new CConfigMgr(dir.File("Config.ini"));

        Assert.Equal("DEF", cfg.ReadString("Strings", "Title", "DEF"));         // :95-98
        Assert.Equal("DEF", cfg.m_xIni.ReadString("Strings", "Title", ""));     // 已回写
        // 再以空 Default 读取：键已存在 ⇒ 返回文件值 DEF（:99-100 的 else 分支）
        Assert.Equal("DEF", cfg.ReadString("Strings", "Title", ""));
    }

    [Fact]
    public void ReadString_EmptyDefaultOnAbsentKey_WritesNothing_SoResultIsEmpty()
    {
        using var dir = new TempDir();
        var cfg = new CConfigMgr(dir.File("Config.ini"));
        Assert.Equal("", cfg.ReadString("Strings", "Title", ""));               // :95 Result := Default
        Assert.False(cfg.m_xIni.ValueExists("Strings", "Title"));               // :98 WriteString('') 被 TIniFile 跳过
    }

    [Fact]
    public void ReadInteger_NegativeStoredValue_TreatedAsMissing()
    {
        using var dir = new TempDir();
        string path = dir.File("Config.ini");
        File.WriteAllText(path, "[Integer]\r\nMaxConnectOfIP=-1\r\n", EncodingInit.GBK);

        var cfg = new CConfigMgr(path);
        Assert.Equal(20, cfg.ReadInteger("Integer", "MaxConnectOfIP", 20)); // :109 szLoadInt < 0 ⇒ 写回 Default
        Assert.Equal("20", cfg.m_xIni.ReadString("Integer", "MaxConnectOfIP", ""));
    }

    [Fact]
    public void ReadInteger_ZeroStoredValue_IsHonoured()
    {
        using var dir = new TempDir();
        string path = dir.File("Config.ini");
        File.WriteAllText(path, "[Integer]\r\nMaxConnectOfIP=0\r\n", EncodingInit.GBK);

        var cfg = new CConfigMgr(path);
        Assert.Equal(0, cfg.ReadInteger("Integer", "MaxConnectOfIP", 20)); // :109 0 不小于 0 ⇒ 采用原值
    }

    [Fact]
    public void ReadBool_UsesIntegerParsing_NegativeMeansMissing()
    {
        using var dir = new TempDir();
        string path = dir.File("Config.ini");
        File.WriteAllText(path, "[Switch]\r\nA=-1\r\nB=0\r\nC=1\r\nD=2\r\n", EncodingInit.GBK);

        var cfg = new CConfigMgr(path);
        Assert.False(cfg.ReadBool("Switch", "A", false)); // :121-122 -1 < 0 ⇒ 回写 Default(false)
        Assert.False(cfg.ReadBool("Switch", "B", true));  // :124 0 ⇒ false
        Assert.True(cfg.ReadBool("Switch", "C", false));  // :124 1 ⇒ true
        Assert.True(cfg.ReadBool("Switch", "D", false));  // :124 != 0 ⇒ true

        // :122 WriteBool(true) 落盘为 "-1"（Delphi BoolToStr 语义）；:122 WriteBool(false) 落盘为 "0"
        var cfg2 = new CConfigMgr(path);
        Assert.Equal("0", cfg2.ReadString("Switch", "A", "?"));
        Assert.Equal("0", cfg2.ReadString("Switch", "B", "?"));

        File.WriteAllText(path, "[Switch]\r\nE=abc\r\n", EncodingInit.GBK); // 非整数 ⇒ TIniFile 以 Default(-1) 处理
        var cfg3 = new CConfigMgr(path);
        Assert.True(cfg3.ReadBool("Switch", "E", true));
        Assert.Equal("-1", cfg3.ReadString("Switch", "E", "?"));          // 回写为 -1
    }

    [Fact]
    public void ReadFloat_UnparsableValue_ReturnsDefault_AndBelowThresholdRewrites()
    {
        using var dir = new TempDir();
        string path = dir.File("Config.ini");
        File.WriteAllText(path, "[F]\r\nX=abc\r\nY=0.5\r\n", EncodingInit.GBK);

        var cfg = new CConfigMgr(path);
        Assert.Equal(1.25, cfg.ReadFloat("F", "X", 1.25)); // :130 StrToFloatDef('abc',0)=0 < 0.10 ⇒ 取 Default 并回写
        Assert.Equal(0.5, cfg.ReadFloat("F", "Y", 1.25));  // 0.5 >= 0.10 ⇒ 采用文件值
    }

    // =====================================================================================
    // 3. LoadConfig：SelGates_iocp 段（:168-184）
    // =====================================================================================

    [Fact]
    public void LoadConfig_GateCountGreaterThanZero_ReadsIocpSection_AndFillsRemainingFromIndex1()
    {
        using var dir = new TempDir();
        string path = dir.File("Config.ini");
        File.WriteAllText(path,
            "[SelGates_iocp]\r\nCount=3\r\nServerAddr1=10.0.0.1\r\nServerPort1=6001\r\nGatePort1=7001\r\n" +
            "ServerAddr2=10.0.0.2\r\nServerPort2=6002\r\nGatePort2=7002\r\n", EncodingInit.GBK);

        var cfg = new CConfigMgr(path);
        cfg.LoadConfig();

        Assert.Equal(3, cfg.m_nGateCount);                                    // :168
        Assert.Equal("10.0.0.1", cfg.m_xGameGateList[1].sServerAdress);       // :173
        Assert.Equal(6001, cfg.m_xGameGateList[1].nServerPort);               // :174
        Assert.Equal(7001, cfg.m_xGameGateList[1].nGatePort);                 // :175
        Assert.Equal("10.0.0.2", cfg.m_xGameGateList[2].sServerAdress);
        // 第 3 项缺 ServerAddr3/ServerPort3 ⇒ 回写槽位默认值（构造器 :66-68），GatePort3 同
        Assert.Equal("127.0.0.1", cfg.m_xGameGateList[3].sServerAdress);
        Assert.Equal(5100, cfg.m_xGameGateList[3].nServerPort);
        Assert.Equal(7102, cfg.m_xGameGateList[3].nGatePort);

        // :178-183 其余槽位从 [1] 派生，GatePort = [1].GatePort + I - 1
        Assert.Equal("10.0.0.1", cfg.m_xGameGateList[4].sServerAdress);
        Assert.Equal(6001, cfg.m_xGameGateList[4].nServerPort);
        Assert.Equal(7004, cfg.m_xGameGateList[4].nGatePort);
        Assert.Equal(7001 + CConfigMgr.MAX_SERVER_COUNT - 1, cfg.m_xGameGateList[CConfigMgr.MAX_SERVER_COUNT].nGatePort);
    }

    [Fact]
    public void LoadConfig_GateCountZero_FallsBackToLegacySelGateSection()
    {
        using var dir = new TempDir();
        string path = dir.File("Config.ini");
        File.WriteAllText(path,
            "[SelGate]\r\nServerAddr=192.168.1.9\r\nServerPort=7100\r\nGatePort=7200\r\n", EncodingInit.GBK);

        var cfg = new CConfigMgr(path);
        cfg.LoadConfig();

        Assert.Equal(1, cfg.m_nGateCount);                                   // :187
        Assert.Equal("192.168.1.9", cfg.m_xGameGateList[1].sServerAdress);   // :189
        Assert.Equal(7100, cfg.m_xGameGateList[1].nServerPort);              // :190
        Assert.Equal(7200, cfg.m_xGameGateList[1].nGatePort);                // :191
        Assert.Equal("192.168.1.9", cfg.m_xGameGateList[2].sServerAdress);   // :195
        Assert.Equal(7100, cfg.m_xGameGateList[2].nServerPort);              // :196
        Assert.Equal(7201, cfg.m_xGameGateList[2].nGatePort);                // :197
        Assert.Equal(7200 + CConfigMgr.MAX_SERVER_COUNT - 1, cfg.m_xGameGateList[CConfigMgr.MAX_SERVER_COUNT].nGatePort);
    }

    [Fact]
    public void LoadConfig_ClientTimeOutBelowTenSeconds_ClampedAndPersisted()
    {
        using var dir = new TempDir();
        string path = dir.File("Config.ini");
        File.WriteAllText(path, "[Integer]\r\nClientTimeOutTime2=5000\r\n", EncodingInit.GBK);

        var cfg = new CConfigMgr(path);
        cfg.LoadConfig();

        Assert.Equal(10 * 1000, cfg.m_nClientTimeOutTime);                          // :148
        Assert.Equal("10000", cfg.m_xIni.ReadString("Integer", "ClientTimeOutTime2", "")); // :149 立即落盘
    }

    [Fact]
    public void LoadConfig_KeyNamesUseThe2Suffix_VersusLoginGateNames()
    {
        using var dir = new TempDir();
        string path = dir.File("Config.ini");
        // 不带 "2" 后缀的旧键名必须**不被 ReadInteger 读取**（ReadInteger 只认 NomClientPacketSize2）
        File.WriteAllText(path, "[Integer]\r\nNomClientPacketSize=123\r\nMaxClientPacketCount=9\r\n", EncodingInit.GBK);

        var cfg = new CConfigMgr(path);
        cfg.LoadConfig();

        Assert.Equal(700, cfg.m_nNomClientPacketSize);   // :154 读 NomClientPacketSize2（缺 ⇒ 默认 700）
        Assert.Equal(20, cfg.m_nMaxClientPacketCount);   // :155 读 MaxClientPacketCount2（缺 ⇒ 默认 20）
        Assert.Equal("700", cfg.m_xIni.ReadString("Integer", "NomClientPacketSize2", ""));
    }

    [Fact]
    public void ReadString_WriteSkipsEmptyValue_SoTitleKeyIsAbsentUntilNonEmpty()
    {
        using var dir = new TempDir();
        string path = dir.File("Config.ini");
        var cfg = new CConfigMgr(path);
        cfg.LoadConfig();                                  // m_szTitle 为 '' ⇒ WriteString('') 被跳过
        Assert.Equal("", cfg.m_xIni.ReadString("Strings", "Title", ""));
        Assert.False(cfg.m_xIni.ValueExists("Strings", "Title"));

        cfg.m_szTitle = "S1";
        cfg.SaveConfig(0);                                 // :211 此时才写入 Title
        Assert.Equal("S1", cfg.m_xIni.ReadString("Strings", "Title", ""));
    }

    // =====================================================================================
    // 4. SaveConfig(0)：Strings/Title、Integer/ShowLogLevel、SelGates_iocp —— 完整文本 + 键序
    // =====================================================================================

    [Fact]
    public void SaveConfig_Type0_WritesExactTextAndKeyOrder()
    {
        using var dir = new TempDir();
        string path = dir.File("Config.ini");
        var cfg = new CConfigMgr(path);
        cfg.m_szTitle = "测试网关";
        cfg.m_nShowLogLevel = 5;
        cfg.m_nGateCount = 2;
        cfg.m_xGameGateList[1].sServerAdress = "10.0.0.1";
        cfg.m_xGameGateList[1].nServerPort = 6001;
        cfg.m_xGameGateList[1].nGatePort = 7001;
        cfg.m_xGameGateList[2].sServerAdress = "10.0.0.2";
        cfg.m_xGameGateList[2].nServerPort = 6002;
        cfg.m_xGameGateList[2].nGatePort = 7002;

        cfg.SaveConfig(0);                                                       // :209-221

        string expected =
            "[Strings]\r\n" +
            "Title=测试网关\r\n" +
            "\r\n" +
            "[Integer]\r\n" +
            "ShowLogLevel=5\r\n" +
            "\r\n" +
            "[SelGates_iocp]\r\n" +
            "Count=2\r\n" +
            "ServerAddr1=10.0.0.1\r\n" +
            "ServerPort1=6001\r\n" +
            "GatePort1=7001\r\n" +
            "ServerAddr2=10.0.0.2\r\n" +
            "ServerPort2=6002\r\n" +
            "GatePort2=7002\r\n" +
            "\r\n";
        Assert.Equal(expected, ReadIni(path));
    }

    // =====================================================================================
    // 5. SaveConfig(1)：Integer 五键 → Switch 六键 → Method —— 完整文本 + 键序 + BoolToStr(-1/0)
    // =====================================================================================

    [Fact]
    public void SaveConfig_Type1_WritesExactTextAndKeyOrder()
    {
        using var dir = new TempDir();
        string path = dir.File("Config.ini");
        var cfg = new CConfigMgr(path);
        cfg.m_nMaxConnectOfIP = 25;             // :225
        cfg.m_nCheckNewIDOfIP = 6;              // :226
        cfg.m_nClientTimeOutTime = 30000;       // :227
        cfg.m_nNomClientPacketSize = 800;       // :228
        cfg.m_nMaxClientPacketCount = 30;       // :229
        cfg.m_fCheckNewIDOfIP = true;           // :232
        cfg.m_fCheckNullSession = false;        // :233
        cfg.m_fOverSpeedSendBack = true;        // :234
        cfg.m_fDefenceCCPacket = false;         // :235
        cfg.m_fKickOverSpeed = true;            // :236
        cfg.m_fKickOverPacketSize = false;      // :237
        cfg.m_tBlockIPMethod = (int)TBlockIPMethod.mBlockList; // :239

        cfg.SaveConfig(1);

        string expected =
            "[Integer]\r\n" +
            "MaxConnectOfIP=25\r\n" +
            "CheckNewIDOfIP=6\r\n" +
            "ClientTimeOutTime2=30000\r\n" +
            "NomClientPacketSize2=800\r\n" +
            "MaxClientPacketCount2=30\r\n" +
            "\r\n" +
            "[Switch]\r\n" +
            "CheckNewIDOfIP=-1\r\n" +
            "CheckNullSession=0\r\n" +
            "OverSpeedSendBack=-1\r\n" +
            "DefenceCCPacket=0\r\n" +
            "KickOverSpeed=-1\r\n" +
            "KickOverPacketSize=0\r\n" +
            "\r\n" +
            "[Method]\r\n" +
            "BlockIPMethod=2\r\n" +
            "\r\n";
        Assert.Equal(expected, ReadIni(path));

        // 回读断言：-1 必须被 ReadBool 判为 True，0 判为 False
        var cfg2 = new CConfigMgr(path);
        cfg2.LoadConfig();
        // 直接比对 xIni 的原始文本（只走公开读取 API，不引入额外局部串）
        Assert.Equal("-1", cfg2.m_xIni.ReadString("Switch", "CheckNewIDOfIP", "?"));
        Assert.Equal("0", cfg2.m_xIni.ReadString("Switch", "CheckNullSession", "?"));
        Assert.Equal("0", cfg2.m_xIni.ReadString("Switch", "OverSpeedSendBack", "?"));
        Assert.Equal("0", cfg2.m_xIni.ReadString("Switch", "DefenceCCPacket", "?"));
        Assert.Equal("0", cfg2.m_xIni.ReadString("Switch", "KickOverSpeed", "?"));
        Assert.Equal("0", cfg2.m_xIni.ReadString("Switch", "KickOverPacketSize", "?"));
        Assert.Equal((int)TBlockIPMethod.mBlockList, cfg2.m_tBlockIPMethod);
        Assert.True(cfg2.m_fCheckNewIDOfIP);
        Assert.False(cfg2.m_fCheckNullSession);
    }

    [Fact]
    public void SaveConfig_Type2_IsAnEmptyBranch()
    {
        using var dir = new TempDir();
        string path = dir.File("Config.ini");
        var cfg = new CConfigMgr(path);
        cfg.SaveConfig(2);                                   // :241-244 原文如此：该分支什么都不写
        Assert.False(File.Exists(path));
        Assert.Equal("", cfg.m_xIni.ToText());
    }

    [Fact]
    public void SaveConfig_Type1_IsStableAcrossRepeatedCalls()
    {
        using var dir = new TempDir();
        string path = dir.File("Config.ini");
        var cfg = new CConfigMgr(path);
        cfg.SaveConfig(1);
        string first = ReadIni(path);
        cfg.SaveConfig(1);                                   // 重复写不得重复追加键
        Assert.Equal(first, ReadIni(path));
    }

    [Fact]
    public void LoadConfig_ThenSaveConfig_RoundTripsThroughDisk()
    {
        using var dir = new TempDir();
        string path = dir.File("Config.ini");
        var cfg = new CConfigMgr(path);
        cfg.LoadConfig();          // 首次：全部缺键 ⇒ 全部回写默认值（并产生 5 个节）
        cfg.SaveConfig(0);
        cfg.SaveConfig(1);

        var cfg2 = new CConfigMgr(path);
        cfg2.LoadConfig();

        Assert.Equal(cfg.m_szTitle, cfg2.m_szTitle);
        Assert.Equal(cfg.m_nShowLogLevel, cfg2.m_nShowLogLevel);
        Assert.Equal(cfg.m_nGateCount, cfg2.m_nGateCount);
        Assert.Equal(cfg.m_nClientTimeOutTime, cfg2.m_nClientTimeOutTime);
        Assert.Equal(cfg.m_nMaxConnectOfIP, cfg2.m_nMaxConnectOfIP);
        Assert.Equal(cfg.m_nNomClientPacketSize, cfg2.m_nNomClientPacketSize);
        Assert.Equal(cfg.m_nMaxClientPacketCount, cfg2.m_nMaxClientPacketCount);
        Assert.Equal(cfg.m_fKickOverPacketSize, cfg2.m_fKickOverPacketSize);
        for (int i = 1; i <= CConfigMgr.MAX_SERVER_COUNT; i++)
        {
            Assert.Equal(cfg.m_xGameGateList[i].sServerAdress, cfg2.m_xGameGateList[i].sServerAdress);
            Assert.Equal(cfg.m_xGameGateList[i].nServerPort, cfg2.m_xGameGateList[i].nServerPort);
            Assert.Equal(cfg.m_xGameGateList[i].nGatePort, cfg2.m_xGameGateList[i].nGatePort);
        }
    }
}
