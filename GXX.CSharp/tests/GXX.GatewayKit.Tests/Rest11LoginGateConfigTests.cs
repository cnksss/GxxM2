using System;
using System.IO;
using GXX.Core.Rtl;
using GXX.GatewayKit.Rest11;
using Xunit;

namespace GXX.GatewayKit.Tests;

public class Rest11LoginGateConfigTests : IDisposable
{
    private readonly string _dir;
    private readonly string _file;

    public Rest11LoginGateConfigTests()
    {
        _dir = Path.Combine(Path.GetTempPath(), "rest11cfg-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dir);
        _file = Path.Combine(_dir, "LoginGate.ini");
    }

    public void Dispose()
    {
        try { Directory.Delete(_dir, true); } catch { }
    }

    // =====================================================================================
    // ConfigManager.pas:56-88 构造默认值（LoginGate 副本：5500 / 7000+i-1）
    // =====================================================================================
    [Fact]
    public void Constructor_SetsLoginGateDefaults()
    {
        var c = new Rest11LoginGateConfig(_file);
        Assert.Equal("", c.m_szTitle);                       // :62 原文如此（注释里的 '登录网关' 被注释掉）
        Assert.Equal(3, c.m_nShowLogLevel);                  // :63
        Assert.False(c.m_boCheckVersion);                    // :65
        Assert.Equal("", c.m_sClientSoftVer);                 // :66
        Assert.Equal(1, c.m_nGateCount);                     // :68

        Assert.Equal("127.0.0.1", c.m_xGameGateList[1].sServerAdress); // :71
        Assert.Equal(5500, c.m_xGameGateList[1].nServerPort);          // :72 ★ LoginGate 副本
        Assert.Equal(7000, c.m_xGameGateList[1].nGatePort);            // :73 ★ LoginGate 副本
        Assert.Equal(5500, c.m_xGameGateList[32].nServerPort);
        Assert.Equal(7000 + 31, c.m_xGameGateList[32].nGatePort);

        Assert.True(c.m_fCheckNewIDOfIP);       // :76
        Assert.True(c.m_fCheckNullSession);     // :77
        Assert.False(c.m_fOverSpeedSendBack);   // :78
        Assert.False(c.m_fDefenceCCPacket);     // :79
        Assert.False(c.m_fKickOverSpeed);       // :80
        Assert.True(c.m_fKickOverPacketSize);   // :81

        Assert.Equal(700, c.m_nNomClientPacketSize);            // :83
        Assert.Equal(20, c.m_nMaxConnectOfIP);                  // :84
        Assert.Equal(5, c.m_nCheckNewIDOfIP);                   // :85
        Assert.Equal(180 * 1000, c.m_nClientTimeOutTime);       // :86
        Assert.Equal(20, c.m_nMaxClientPacketCount);            // :87

        Assert.Equal((int)Rest11TBlockIPMethod.mDisconnect, c.m_tBlockIPMethod); // :39 未赋值 ⇒ 首项
    }

    /// <summary>19 个字段全部有投影（对照复核报告 §4.1 的 "19 字段 vs 已接 5 键"）。</summary>
    [Fact]
    public void AllNineteenFieldsArePresent()
    {
        var t = typeof(Rest11LoginGateConfig);
        foreach (string name in new[]
        {
            "m_szTitle", "m_nShowLogLevel", "m_nGateCount", "m_boCheckVersion", "m_sClientSoftVer",
            "m_xGameGateList",
            "m_fCheckNewIDOfIP", "m_fCheckNullSession", "m_fOverSpeedSendBack", "m_fDefenceCCPacket",
            "m_fKickOverSpeed", "m_fKickOverPacketSize",
            "m_nCheckNewIDOfIP", "m_nMaxConnectOfIP", "m_nClientTimeOutTime", "m_nNomClientPacketSize",
            "m_nMaxClientPacketCount", "m_tBlockIPMethod"
        })
        {
            Assert.NotNull(t.GetField(name));
        }
    }

    // =====================================================================================
    // ConfigManager.pas:96-141 四个 Read* 的"缺失即回写"
    // =====================================================================================
    [Fact]
    public void ReadString_WritesBackDefault_WhenMissing()
    {
        var c = new Rest11LoginGateConfig(_file);
        Assert.Equal("DEF", c.ReadString("LoginGate", "Title", "DEF"));
        Assert.Contains("DEF", c.m_xIni.ReadString("LoginGate", "Title", ""));  // :103 已回写

        File.WriteAllText(_file, "[LoginGate]\r\nTitle=ABC\r\n", GXX.Core.EncodingInit.GBK);
        var c2 = new Rest11LoginGateConfig(_file);
        Assert.Equal("ABC", c2.ReadString("LoginGate", "Title", "DEF"));        // 有值则读回
    }

    [Fact]
    public void ReadInteger_And_ReadBool_TreatNegativeAsMissing()
    {
        var c = new Rest11LoginGateConfig(_file);
        Assert.Equal(7, c.ReadInteger("Integer", "MaxConnectOfIP", 7));
        Assert.Equal(7, c.m_xIni.ReadInteger("Integer", "MaxConnectOfIP", -1)); // :115 回写默认

        // 显式写 0 不算缺失（:114 只在 < 0 时回写）
        c.m_xIni.WriteInteger("Integer", "MaxConnectOfIP", 0);
        Assert.Equal(0, c.ReadInteger("Integer", "MaxConnectOfIP", 7));

        // ★ 原文缺陷照抄：Delphi BoolToStr(True) = -1，而 ReadBool 用 `< 0` 探测"缺失"
        //   ⇒ **显式写成 -1 的 True 会被当成缺失并回写成 Default(False)**。
        //   原文如此（ConfigManager.pas:114/:126），登记见报告 §6 D-P11-03。
        c.m_xIni.WriteInteger("Switch", "CheckNullSession", -1);
        Assert.False(c.ReadBool("Switch", "CheckNullSession", false));
        // 显式写 1 ⇒ 非负 ⇒ Result = (1 != 0) = True
        c.m_xIni.WriteInteger("Switch", "CheckNullSession", 1);
        Assert.True(c.ReadBool("Switch", "CheckNullSession", false));
        // 显式写 0 ⇒ 非负 ⇒ Result = (0 != 0) = False，且**不**回写
        c.m_xIni.WriteInteger("Switch", "CheckNullSession", 0);
        Assert.False(c.ReadBool("Switch", "CheckNullSession", true));
    }

    [Fact]
    public void ReadFloat_WritesBackWhenBelowThreshold()
    {
        var c = new Rest11LoginGateConfig(_file);
        Assert.Equal(1.5, c.ReadFloat("Float", "X", 1.5), 6);                  // :137 <0.10 ⇒ 回写并返回 Default
        Assert.Equal(1.5, c.m_xIni.ReadFloat("Float", "X", 0), 6);

        c.m_xIni.WriteFloat("Float", "Y", 2.25);
        Assert.Equal(2.25, c.ReadFloat("Float", "Y", 1.5), 6);                 // :140 二次读取返回值
    }

    // =====================================================================================
    // ConfigManager.pas:143-210 LoadConfig —— 段名 [LoginGate]/[Integer]/[Switch]/[Method]
    // =====================================================================================
    [Fact]
    public void LoadConfig_ReadsAllOriginalSections()
    {
        File.WriteAllText(_file, string.Join("\r\n", new[]
        {
            "[LoginGate]",
            "Title=测试登录网关",
            "ShowLogLevel=5",
            "CheckClientSoft=-1",
            "ClientSoftVer=1.2.3",
            "Count=0",
            "ServerAddr=10.9.9.9",
            "ServerPort=6600",
            "GatePort=7700",
            "[Integer]",
            "ClientTimeOutTime3=60000",
            "MaxConnectOfIP=33",
            "CheckNewIDOfIP=9",
            "NomClientPacketSize2=800",
            "MaxClientPacketCount2=30",
            "[Switch]",
            "CheckNewIDOfIP=0",
            "CheckNullSession=0",
            "OverSpeedSendBack=1",
            "DefenceCCPacket=1",
            "KickOverSpeed=1",
            "KickOverPacketSize=1",
            "[Method]",
            "BlockIPMethod=2",
            ""
        }), GXX.Core.EncodingInit.GBK);

        var c = new Rest11LoginGateConfig(_file);
        c.LoadConfig();

        Assert.Equal("测试登录网关", c.m_szTitle);          // :148
        Assert.Equal(5, c.m_nShowLogLevel);                  // :151
        // ★ `CheckClientSoft=-1` 被 ReadBool 当成"缺失"（`< 0`）⇒ 回写成默认 False（原文如此）
        Assert.False(c.m_boCheckVersion);                    // :153
        Assert.Equal("1.2.3", c.m_sClientSoftVer);           // :154
        Assert.Equal(60000, c.m_nClientTimeOutTime);         // :156
        Assert.Equal(33, c.m_nMaxConnectOfIP);               // :163
        Assert.Equal(9, c.m_nCheckNewIDOfIP);                // :164
        Assert.Equal(800, c.m_nNomClientPacketSize);         // :165
        Assert.Equal(30, c.m_nMaxClientPacketCount);         // :166
        Assert.False(c.m_fCheckNewIDOfIP);                   // :169
        Assert.False(c.m_fCheckNullSession);                 // :170
        Assert.True(c.m_fOverSpeedSendBack);                 // :171
        Assert.True(c.m_fDefenceCCPacket);                   // :172
        Assert.True(c.m_fKickOverSpeed);                     // :173
        Assert.True(c.m_fKickOverPacketSize);                // :174
        Assert.Equal(2, c.m_tBlockIPMethod);                 // :177
        Assert.Equal(1, c.m_nGateCount);                     // :182 Count<=0 分支
        Assert.Equal("10.9.9.9", c.m_xGameGateList[1].sServerAdress);  // :183
        Assert.Equal(6600, c.m_xGameGateList[1].nServerPort);          // :184
        Assert.Equal(7700, c.m_xGameGateList[1].nGatePort);            // :185
        Assert.Equal(7700 + 31, c.m_xGameGateList[32].nGatePort);      // :191 GatePort + I - 1
    }

    [Fact]
    public void LoadConfig_ClampsClientTimeOutToTenSeconds()
    {
        File.WriteAllText(_file, "[Integer]\r\nClientTimeOutTime3=500\r\n", GXX.Core.EncodingInit.GBK);
        var c = new Rest11LoginGateConfig(_file);
        c.LoadConfig();                                                      // :157-161
        Assert.Equal(10 * 1000, c.m_nClientTimeOutTime);
        Assert.Equal(10 * 1000, c.m_xIni.ReadInteger("Integer", "ClientTimeOutTime3", -1)); // :160 已回写
    }

    [Fact]
    public void LoadConfig_MultiGateBranch_ReadsIndexedKeys()
    {
        File.WriteAllText(_file, string.Join("\r\n", new[]
        {
            "[LoginGate]",
            "Count=2",
            "ServerAddr1=1.1.1.1",
            "ServerPort1=5501",
            "GatePort1=7001",
            "ServerAddr2=2.2.2.2",
            "ServerPort2=5502",
            "GatePort2=7002",
            ""
        }), GXX.Core.EncodingInit.GBK);

        var c = new Rest11LoginGateConfig(_file);
        c.LoadConfig();

        Assert.Equal(2, c.m_nGateCount);                              // :179
        Assert.Equal("1.1.1.1", c.m_xGameGateList[1].sServerAdress);   // :198
        Assert.Equal(5501, c.m_xGameGateList[1].nServerPort);          // :199
        Assert.Equal(7001, c.m_xGameGateList[1].nGatePort);            // :200
        Assert.Equal("2.2.2.2", c.m_xGameGateList[2].sServerAdress);
        Assert.Equal(7002, c.m_xGameGateList[2].nGatePort);
        Assert.Equal(7001 + 31, c.m_xGameGateList[32].nGatePort);      // :207 从 idx1 顺延
    }

    // =====================================================================================
    // ConfigManager.pas:212-267 SaveConfig
    // =====================================================================================
    [Fact]
    public void SaveConfig_Type0_WritesLoginGateSectionAndMirrorsIndex1()
    {
        var c = new Rest11LoginGateConfig(_file);
        c.m_szTitle = "T";
        c.m_nShowLogLevel = 4;
        c.m_boCheckVersion = true;
        c.m_sClientSoftVer = "9.9";
        c.m_nGateCount = 1;
        c.SaveConfig(0);

        string text = c.m_xIni.ToText();
        Assert.Contains("[LoginGate]", text);
        Assert.Contains("Title=T", text);
        Assert.Contains("ShowLogLevel=4", text);
        Assert.Contains("Count=1", text);
        Assert.Contains("ServerAddr1=", text);
        Assert.Contains("ServerAddr=", text);           // :233 idx1 额外镜像
        Assert.Contains("CheckClientSoft=-1", text);    // :239 BoolToStr(True) = -1
        Assert.Contains("ClientSoftVer=9.9", text);
    }

    [Fact]
    public void SaveConfig_Type1_WritesIntegerSwitchMethodSections()
    {
        var c = new Rest11LoginGateConfig(_file)
        {
            m_nMaxConnectOfIP = 21,
            m_nCheckNewIDOfIP = 6,
            m_nClientTimeOutTime = 90000,
            m_nNomClientPacketSize = 701,
            m_nMaxClientPacketCount = 22,
            m_tBlockIPMethod = 1
        };
        c.m_fOverSpeedSendBack = true;
        c.SaveConfig(1);

        string text = c.m_xIni.ToText();
        Assert.Contains("[Integer]", text);
        Assert.Contains("MaxConnectOfIP=21", text);         // :245
        Assert.Contains("ClientTimeOutTime3=90000", text);  // :247
        Assert.Contains("NomClientPacketSize2=701", text);  // :248
        Assert.Contains("MaxClientPacketCount2=22", text);  // :249
        Assert.Contains("[Switch]", text);
        Assert.Contains("CheckNullSession=-1", text);       // :253 默认 True
        Assert.Contains("[Method]", text);
        Assert.Contains("BlockIPMethod=1", text);           // :259
    }

    [Fact]
    public void SaveConfig_Type2_IsEmptyBranch()
    {
        var c = new Rest11LoginGateConfig(_file);
        c.SaveConfig(2);                                    // :261-264 原文如此：空分支
        Assert.Equal("", c.m_xIni.ToText());
    }

    /// <summary>`TIniFile.WriteString` 对空串**不落盘**（`ConfigManager` `m_szTitle` 默认就是空串）。</summary>
    [Fact]
    public void WriteString_SkipsEmptyValue()
    {
        var ini = new Rest11LoginGateIniFile(Path.Combine(_dir, "empty.ini"));
        ini.WriteString("S", "K", "");
        Assert.False(ini.ValueExists("S", "K"));
        Assert.Equal("", ini.ToText());
    }

    [Fact]
    public void IniFile_IsCaseInsensitiveAndKeepsKeyOrder()
    {
        var ini = new Rest11LoginGateIniFile(Path.Combine(_dir, "order.ini"));
        ini.WriteInteger("Integer", "B", 2);
        ini.WriteInteger("Integer", "A", 1);
        ini.WriteInteger("Integer", "B", 3);   // 同键改写不改变顺序
        string text = ini.ToText();
        Assert.True(text.IndexOf("B=3", StringComparison.Ordinal) < text.IndexOf("A=1", StringComparison.Ordinal));
        Assert.Equal(3, ini.ReadInteger("INTEGER", "b", -1)); // 节/键名大小写不敏感
    }

    // =====================================================================================
    // FuncForComm.pas:507-561 / :563-587 ShowThreadInfo / OnTimerProc / 常量
    // =====================================================================================
    [Fact]
    public void ProtocolConstants_MatchLoginGateCopy()
    {
        Assert.Equal("正在启动登陆网关...", Rest11LoginGateConstants._STR_NOW_START);  // :16（SelGate 副本是"角色网关"）
        Assert.Equal("登陆网关启动完成...", Rest11LoginGateConstants._STR_STARTED);    // :17
        Assert.Equal(@".\Config.ini", Rest11LoginGateConstants._STR_CONFIG_FILE);     // :20
        Assert.Equal(@".\BlockIPList.txt", Rest11LoginGateConstants._STR_BLOCK_FILE); // :21
        Assert.Equal(@".\BlockIPAreaList.txt", Rest11LoginGateConstants._STR_BLOCK_AREA_FILE); // :22
        Assert.Equal(2024, Rest11LoginGateConstants._IDM_SERVERSOCK_MSG);            // :25
        Assert.Equal(2025, Rest11LoginGateConstants._IDM_TIMER_STARTSERVICE);
        Assert.Equal(2026, Rest11LoginGateConstants._IDM_TIMER_STOPSERVICE);
        Assert.Equal(2027, Rest11LoginGateConstants._IDM_TIMER_KEEP_ALIVE);
        Assert.Equal(2028, Rest11LoginGateConstants._IDM_TIMER_THREAD_INFO);
        Assert.Equal(64, Rest11LoginGateConstants.FIRST_PAKCET_MAX_LEN);             // :32 八进制 0080
    }

    [Fact]
    public void MiscConstants_MatchLoginGateCopy()
    {
        Assert.Equal(1000, Rest11LoginGateMisc.SG_FORMHANDLE); // Misc.pas:121
        Assert.Equal(1001, Rest11LoginGateMisc.SG_STARTNOW);
        Assert.Equal(1002, Rest11LoginGateMisc.SG_STARTOK);
        Assert.Equal(1003, Rest11LoginGateMisc.SG_ACTIVE);
        Assert.Equal(2000, Rest11LoginGateMisc.GS_QUIT);        // :126
        Assert.Equal(0, Rest11LoginGateMisc.VER_TYPE);          // :9
        Assert.Equal("登陆网关", Rest11LoginGateMisc.PROGRAM_NAME); // :13
        Assert.Equal(4, (int)Rest11LoginGateMisc.TProgamType.tLoginGate); // :129-131
    }

    // =====================================================================================
    // ClientSession.pas:47 / :791-800 FillUserList —— USER_ARRAY_COUNT = 1000 + 48
    // =====================================================================================
    [Fact]
    public void FillUserList_FillsSameInstanceAcrossAllSlots()
    {
        var list = Rest11LoginGateSession.FillUserList();
        Assert.Equal(1048, list.Length);                        // AcceptExWorkedThread.pas:12-13
        Assert.NotNull(list[0]);
        Assert.Same(list[0], list[1047]);                       // 原文把同一个 g_pFillUserObj 填满全表
    }

    [Fact]
    public void UserArrayCountDefault_IsOneThousandFortyEight()
    {
        Assert.Equal(1048, Rest11LoginGateOptions.Rest11DefaultUserArrayCount);
    }
}
