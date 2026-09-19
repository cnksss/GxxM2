using System;
using System.Collections.Generic;
using System.IO;
using GXX.Core.Rtl;
using GXX.Core.Util;
using static GXX.GameCenter.GShareGlobals;

namespace GXX.GameCenter;

/// <summary>
/// GMain.pas 配置生成段 1:1 移植（行号范围 1619..2606 + 6604..6727）：
/// <list type="bullet">
/// <item>GenGameConfig（1619-1630）</item>
/// <item>GenDBServerConfig（1632-1974，含嵌套 GetRunGatePort/GetRunGateDBPort）</item>
/// <item>GenLoginServerConfig（1976-2089）</item>
/// <item>GenLogServerConfig（2091-2113）</item>
/// <item>GenM2ServerConfig（2115-2245）</item>
/// <item>GenLoginGateConfig（2247-2271）</item>
/// <item>GenSelGateConfig（2273-2295）</item>
/// <item>GenMutLoginGateConfig（2297-2350）</item>
/// <item>GetMutLoginGateZero（2353-2367）</item>
/// <item>GenMutSelGateConfigEx（2369-2404）</item>
/// <item>GetMutSelGateZero（2406-2420）</item>
/// <item>GenMutSelGateConfig（2422-2458）</item>
/// <item>GetMutRunGateConfing（2460-2481）</item>
/// <item>GetMutRunGateConfingEx（2483-2519）</item>
/// <item>GetMutRunGateZero（2521-2535）</item>
/// <item>GenRunGateConfig（2537-2606）</item>
/// <item>GenBackupConfig（6604-6727）</item>
/// </list>
/// 落盘的 INI 节序/键序、伴生 txt 行序、目录创建顺序逐字保留；原文笔误见各处 "原文如此" 注释。
/// </summary>
public static class GMainConfig
{
    /// <summary>
    /// GMain.pas:1972-1973 <c>dtpDate.Date := Now; dtpTime.Time := Now;</c>
    /// 由宿主窗体接线（GenDBServerConfig 末尾）；未接线时相当于原窗体无该控件。
    /// </summary>
    public static Action<DateTime>? DtpDateTimeSetter;

    /// <summary>GMain.pas:1619 <c>procedure TfrmMain.GenGameConfig;</c></summary>
    public static void GenGameConfig()
    {
        GenDBServerConfig();
        GenLoginServerConfig();
        GenLogServerConfig();
        GenM2ServerConfig();
        GenLoginGateConfig();
        GenSelGateConfig();
        GenRunGateConfig();

        GenBackupConfig();
    }

    // ===================== GMain.pas:1632-1974 GenDBServerConfig =====================

    /// <summary>GMain.pas:1634-1652 嵌套函数 <c>GetRunGatePort(nIndex)</c>：取第 nIndex 个"已启动"网关的 GatePort。</summary>
    public static int GetRunGatePort(int nIndex)
    {
        int Result = 0;
        int nC = 0;
        for (int I = 0; I <= g_RunGateInfo.Length - 1; I++)
        {
            if (g_RunGateInfo[I].boGetStart)
            {
                if (nC == nIndex)
                {
                    Result = g_RunGateInfo[I].nGatePort;
                    break;
                }
                nC++;
            }
        }
        return Result;
    }

    /// <summary>GMain.pas:1654-1672 嵌套函数 <c>GetRunGateDBPort(nIndex)</c>：取第 nIndex 个"已启动"网关的 DBPort。</summary>
    public static int GetRunGateDBPort(int nIndex)
    {
        int Result = 0;
        int nC = 0;
        for (int I = 0; I <= g_RunGateInfo.Length - 1; I++)
        {
            if (g_RunGateInfo[I].boGetStart)
            {
                if (nC == nIndex)
                {
                    Result = g_RunGateInfo[I].nDBPort;
                    break;
                }
                nC++;
            }
        }
        return Result;
    }

    /// <summary>GMain.pas:1632 <c>procedure TfrmMain.GenDBServerConfig;</c></summary>
    public static void GenDBServerConfig()
    {
        using var cleanup = new CleanupScope();

        string sIniFile = g_sGameDirectory + g_sDBServer_Directory;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }

        using (var IniGameConf = new GameCenterIniFile(sIniFile + g_sDBServer_ConfigFile))
        {
            IniGameConf.WriteString("Setup", "ServerName", g_sGameName);
            IniGameConf.WriteString("Setup", "ServerAddr", g_sDBServer_Config_ServerAddr);
            IniGameConf.WriteInteger("Setup", "ServerPort", g_nDBServer_Config_ServerPort);
            IniGameConf.WriteString("Setup", "MapFile", g_sGameDirectory + g_sDBServer_Config_MapFile);
            IniGameConf.WriteBool("Setup", "ViewHackMsg", g_boDBServer_Config_ViewHackMsg);
            IniGameConf.WriteBool("Setup", "DoubleLineMode", g_boDoubleLineMode);
            // 增加动态IP支持 piaoyun 2013-08-30
            IniGameConf.WriteBool("Setup", "DynamicIPMode", g_boDynamicIPMode);

            IniGameConf.WriteBool("Setup", "DisableAutoGame", g_boDBServer_DisableAutoGame);
            IniGameConf.WriteString("Setup", "GateAddr", g_sDBServer_Config_GateAddr);
            IniGameConf.WriteInteger("Setup", "GatePort", g_nDBServer_Config_GatePort);

            IniGameConf.WriteString("Server", "IDSAddr", g_sLoginServer_ServerAddr);                          //登录服务器IP
            IniGameConf.WriteInteger("Server", "IDSPort", g_nLoginServer_ServerPort);                         //登录服务器端口

            IniGameConf.WriteBool("Setup", "UseSqliteDB", g_boUseSqliteDB);
            IniGameConf.WriteString("Setup", "DBName", g_sHeroDBName);
            IniGameConf.WriteString("Setup", "SqliteDBName", g_sSqliteDBName);

            IniGameConf.WriteInteger("DataSaveDB", "DataSaveDBType", g_nDataSaveDBType);
            IniGameConf.WriteString("DataSaveDB", "DataSaveDBServer", g_sDataSaveDBServer);
            IniGameConf.WriteInteger("DataSaveDB", "DataSaveDBPort", g_wDataSaveDBPort);
            IniGameConf.WriteString("DataSaveDB", "DataSaveDBUser", g_sDataSaveDBUser);
            IniGameConf.WriteString("DataSaveDB", "DataSaveDBPassword", g_sDataSaveDBPassword);
            IniGameConf.WriteString("DataSaveDB", "DataSaveDataBase", g_sDataSaveDataBase);

            IniGameConf.WriteInteger("DBClear", "Interval", g_nDBServer_Config_Interval);
            IniGameConf.WriteInteger("DBClear", "Level1", g_nDBServer_Config_Level1);
            IniGameConf.WriteInteger("DBClear", "Level2", g_nDBServer_Config_Level2);
            IniGameConf.WriteInteger("DBClear", "Level3", g_nDBServer_Config_Level3);
            IniGameConf.WriteInteger("DBClear", "Day1", g_nDBServer_Config_Day1);
            IniGameConf.WriteInteger("DBClear", "Day2", g_nDBServer_Config_Day2);
            IniGameConf.WriteInteger("DBClear", "Day3", g_nDBServer_Config_Day3);
            IniGameConf.WriteInteger("DBClear", "Month1", g_nDBServer_Config_Month1);
            IniGameConf.WriteInteger("DBClear", "Month2", g_nDBServer_Config_Month2);
            IniGameConf.WriteInteger("DBClear", "Month3", g_nDBServer_Config_Month3);
            IniGameConf.WriteString("DB", "Dir", sIniFile + g_sDBServer_Config_Dir);
            IniGameConf.WriteString("DB", "IdDir", sIniFile + g_sDBServer_Config_IdDir);
            IniGameConf.WriteString("DB", "HumDir", sIniFile + g_sDBServer_Config_HumDir);
            IniGameConf.WriteString("DB", "FeeDir", sIniFile + g_sDBServer_Config_FeeDir);
            IniGameConf.WriteString("DB", "BackupDir", sIniFile + g_sDBServer_Config_BackupDir);
            IniGameConf.WriteString("DB", "ConnectDir", sIniFile + g_sDBServer_Config_ConnectDir);
            IniGameConf.WriteString("DB", "LogDir", sIniFile + g_sDBServer_Config_LogDir);

            IniGameConf.UpdateFile();
        }

        var SaveList = new TStringList();
        SaveList.Add(g_sLocalIPaddr);

        if (g_boDoubleLineMode)
        {
            SaveList.Add(g_sLocalIPaddr1);
            SaveList.Add(g_sExtIPaddr);
            SaveList.Add(g_sExtNetComIPaddr);
        }
        else
        {
            SaveList.Add(g_sExtIPaddr);
        }

        SaveList.SaveToFile(sIniFile + g_sDBServer_AddrTableFile);

        SaveList.Clear();

        // 原文如此（GMain.pas:1750-1754）：SaveList.Clear 之后直接 SaveToFile(GateList)，
        // 生成的 !GateList.ini 此刻只有空文件；地址行在 IniGameConf.Free（GMain.pas:1873）之后才追加。
        // 下文所有地址行都累积在 addrLineList（等价于原文的 SaveList），最后统一追加。
        var addrLineList = new TStringList();
        string sFileName = sIniFile + g_sDBServer_GateListFile;
        if (!File.Exists(sFileName) || g_nRunGate_Count == 0)
        {
            addrLineList.SaveToFile(sFileName);
            addrLineList.Clear();
        }

        cleanup.Register(SaveList);
        cleanup.Register(addrLineList);

        // 原文（GMain.pas:1756-1873）先用 TIniFile.Create 打开 !GateList.ini 并把 GateDBPort0 键
        // 写进内存缓冲，最后 IniGameConf.Free 才落盘 —— 磁盘顺序是 [GateDBPort0] 键在前、地址行在后。
        // TIniFile 落盘是"缓存全量重写"，会丢弃文件里原有的其它键，这里先清空文件以对齐。
        File.WriteAllText(sFileName, "", GXX.Core.EncodingInit.GBK);
        using (var IniGameConf = new GameCenterIniFile(sFileName))
        {
            IniGameConf.EraseSection("GateDBPort0");

            switch (g_nRunGate_Count)
            {
                case 1:
                    {
                        addrLineList.Add(GameCenterFormat.Format("%s %s %d", g_sLocalIPaddr, g_sExtIPaddr, GetRunGatePort(0)));
                        IniGameConf.WriteInteger("GateDBPort0", "1", GetRunGateDBPort(0));
                    }
                    break;

                case 2:
                    {
                        addrLineList.Add(GameCenterFormat.Format("%s %s %d %s %d", g_sLocalIPaddr,
                          g_sExtIPaddr, GetRunGatePort(0), g_sExtIPaddr, GetRunGatePort(1)));
                        IniGameConf.WriteInteger("GateDBPort0", "1", GetRunGateDBPort(0));
                        IniGameConf.WriteInteger("GateDBPort0", "2", GetRunGateDBPort(1));
                    }
                    break;

                case 3:
                    {
                        addrLineList.Add(GameCenterFormat.Format("%s %s %d %s %d %s %d", g_sLocalIPaddr,
                          g_sExtIPaddr, GetRunGatePort(0),
                            g_sExtIPaddr, GetRunGatePort(1),
                            g_sExtIPaddr, GetRunGatePort(2)));
                        IniGameConf.WriteInteger("GateDBPort0", "1", GetRunGateDBPort(0));
                        IniGameConf.WriteInteger("GateDBPort0", "2", GetRunGateDBPort(1));
                        IniGameConf.WriteInteger("GateDBPort0", "3", GetRunGateDBPort(2));
                    }
                    break;

                case 4:
                    {
                        addrLineList.Add(GameCenterFormat.Format("%s %s %d %s %d %s %d %s %d", g_sLocalIPaddr,
                          g_sExtIPaddr, GetRunGatePort(0),
                            g_sExtIPaddr, GetRunGatePort(1),
                            g_sExtIPaddr, GetRunGatePort(2),
                            g_sExtIPaddr, GetRunGatePort(3)));
                        IniGameConf.WriteInteger("GateDBPort0", "1", GetRunGateDBPort(0));
                        IniGameConf.WriteInteger("GateDBPort0", "2", GetRunGateDBPort(1));
                        IniGameConf.WriteInteger("GateDBPort0", "3", GetRunGateDBPort(2));
                        IniGameConf.WriteInteger("GateDBPort0", "4", GetRunGateDBPort(3));
                    }
                    break;
                case 5:
                    {
                        addrLineList.Add(GameCenterFormat.Format("%s %s %d %s %d %s %d %s %d %s %d", g_sLocalIPaddr,
                          g_sExtIPaddr, GetRunGatePort(0),
                            g_sExtIPaddr, GetRunGatePort(1),
                            g_sExtIPaddr, GetRunGatePort(2),
                            g_sExtIPaddr, GetRunGatePort(3),
                            g_sExtIPaddr, GetRunGatePort(4)));
                        IniGameConf.WriteInteger("GateDBPort0", "1", GetRunGateDBPort(0));
                        IniGameConf.WriteInteger("GateDBPort0", "2", GetRunGateDBPort(1));
                        IniGameConf.WriteInteger("GateDBPort0", "3", GetRunGateDBPort(2));
                        IniGameConf.WriteInteger("GateDBPort0", "4", GetRunGateDBPort(3));
                        IniGameConf.WriteInteger("GateDBPort0", "5", GetRunGateDBPort(4));
                    }
                    break;

                case 6:
                    {
                        addrLineList.Add(GameCenterFormat.Format("%s %s %d %s %d %s %d %s %d %s %d %s %d", g_sLocalIPaddr,
                          g_sExtIPaddr, GetRunGatePort(0),
                            g_sExtIPaddr, GetRunGatePort(1),
                            g_sExtIPaddr, GetRunGatePort(2),
                            g_sExtIPaddr, GetRunGatePort(3),
                            g_sExtIPaddr, GetRunGatePort(4),
                            g_sExtIPaddr, GetRunGatePort(5)));

                        IniGameConf.WriteInteger("GateDBPort0", "1", GetRunGateDBPort(0));
                        IniGameConf.WriteInteger("GateDBPort0", "2", GetRunGateDBPort(1));
                        IniGameConf.WriteInteger("GateDBPort0", "3", GetRunGateDBPort(2));
                        IniGameConf.WriteInteger("GateDBPort0", "4", GetRunGateDBPort(3));
                        IniGameConf.WriteInteger("GateDBPort0", "5", GetRunGateDBPort(4));
                        IniGameConf.WriteInteger("GateDBPort0", "6", GetRunGateDBPort(5));
                    }
                    break;

                case 7:
                    {
                        addrLineList.Add(GameCenterFormat.Format("%s %s %d %s %d %s %d %s %d %s %d %s %d %s %d", g_sLocalIPaddr,
                          g_sExtIPaddr, GetRunGatePort(0),
                            g_sExtIPaddr, GetRunGatePort(1),
                            g_sExtIPaddr, GetRunGatePort(2),
                            g_sExtIPaddr, GetRunGatePort(3),
                            g_sExtIPaddr, GetRunGatePort(4),
                            g_sExtIPaddr, GetRunGatePort(5),
                            g_sExtIPaddr, GetRunGatePort(6)));

                        IniGameConf.WriteInteger("GateDBPort0", "1", GetRunGateDBPort(0));
                        IniGameConf.WriteInteger("GateDBPort0", "2", GetRunGateDBPort(1));
                        IniGameConf.WriteInteger("GateDBPort0", "3", GetRunGateDBPort(2));
                        IniGameConf.WriteInteger("GateDBPort0", "4", GetRunGateDBPort(3));
                        IniGameConf.WriteInteger("GateDBPort0", "5", GetRunGateDBPort(4));
                        IniGameConf.WriteInteger("GateDBPort0", "6", GetRunGateDBPort(5));
                        IniGameConf.WriteInteger("GateDBPort0", "7", GetRunGateDBPort(6));
                    }
                    break;

                case 8:
                    {
                        addrLineList.Add(GameCenterFormat.Format("%s %s %d %s %d %s %d %s %d %s %d %s %d %s %d %s %d", g_sLocalIPaddr,
                          g_sExtIPaddr, GetRunGatePort(0),
                            g_sExtIPaddr, GetRunGatePort(1),
                            g_sExtIPaddr, GetRunGatePort(2),
                            g_sExtIPaddr, GetRunGatePort(3),
                            g_sExtIPaddr, GetRunGatePort(4),
                            g_sExtIPaddr, GetRunGatePort(5),
                            g_sExtIPaddr, GetRunGatePort(6),
                            g_sExtIPaddr, GetRunGatePort(7)));

                        IniGameConf.WriteInteger("GateDBPort0", "1", GetRunGateDBPort(0));
                        IniGameConf.WriteInteger("GateDBPort0", "2", GetRunGateDBPort(1));
                        IniGameConf.WriteInteger("GateDBPort0", "3", GetRunGateDBPort(2));
                        IniGameConf.WriteInteger("GateDBPort0", "4", GetRunGateDBPort(3));
                        IniGameConf.WriteInteger("GateDBPort0", "5", GetRunGateDBPort(4));
                        IniGameConf.WriteInteger("GateDBPort0", "6", GetRunGateDBPort(5));
                        IniGameConf.WriteInteger("GateDBPort0", "7", GetRunGateDBPort(6));
                        IniGameConf.WriteInteger("GateDBPort0", "8", GetRunGateDBPort(7));
                    }
                    break;

                    // 原文如此：g_nRunGate_Count 为 0 或 >8 时 case 不命中，
                    // GateDBPort0 节已被 EraseSection 清空，且 !GateList.ini 行不追加。
            }

            IniGameConf.UpdateFile();
        }

        if (g_boDoubleLineMode)
        {
            switch (g_nRunGate_Count)
            {
                case 1:
                    addrLineList.Add(GameCenterFormat.Format("%s %s %d", g_sLocalIPaddr1,
                      g_sExtNetComIPaddr, GetRunGatePort(0)));
                    break;

                case 2:
                    addrLineList.Add(GameCenterFormat.Format("%s %s %d %s %d", g_sLocalIPaddr1,
                      g_sExtNetComIPaddr, GetRunGatePort(0),
                        g_sExtNetComIPaddr, GetRunGatePort(1)));
                    break;

                case 3:
                    addrLineList.Add(GameCenterFormat.Format("%s %s %d %s %d %s %d", g_sLocalIPaddr1,
                      g_sExtNetComIPaddr, GetRunGatePort(0),
                        g_sExtNetComIPaddr, GetRunGatePort(1),
                        g_sExtNetComIPaddr, GetRunGatePort(2)));
                    break;

                case 4:
                    addrLineList.Add(GameCenterFormat.Format("%s %s %d %s %d %s %d %s %d", g_sLocalIPaddr1,
                      g_sExtNetComIPaddr, GetRunGatePort(0),
                        g_sExtNetComIPaddr, GetRunGatePort(1),
                        g_sExtNetComIPaddr, GetRunGatePort(2),
                        g_sExtNetComIPaddr, GetRunGatePort(3)));
                    break;

                case 5:
                    addrLineList.Add(GameCenterFormat.Format("%s %s %d %s %d %s %d %s %d %s %d", g_sLocalIPaddr1,
                      g_sExtNetComIPaddr, GetRunGatePort(0),
                        g_sExtNetComIPaddr, GetRunGatePort(1),
                        g_sExtNetComIPaddr, GetRunGatePort(2),
                        g_sExtNetComIPaddr, GetRunGatePort(3),
                        g_sExtNetComIPaddr, GetRunGatePort(4)));
                    break;

                case 6:
                    addrLineList.Add(GameCenterFormat.Format("%s %s %d %s %d %s %d %s %d %s %d %s %d", g_sLocalIPaddr1,
                      g_sExtNetComIPaddr, GetRunGatePort(0),
                        g_sExtNetComIPaddr, GetRunGatePort(1),
                        g_sExtNetComIPaddr, GetRunGatePort(2),
                        g_sExtNetComIPaddr, GetRunGatePort(3),
                        g_sExtNetComIPaddr, GetRunGatePort(4),
                        g_sExtNetComIPaddr, GetRunGatePort(5)));
                    break;

                case 7:
                    addrLineList.Add(GameCenterFormat.Format("%s %s %d %s %d %s %d %s %d %s %d %s %d %s %d", g_sLocalIPaddr1,
                      g_sExtNetComIPaddr, GetRunGatePort(0),
                        g_sExtNetComIPaddr, GetRunGatePort(1),
                        g_sExtNetComIPaddr, GetRunGatePort(2),
                        g_sExtNetComIPaddr, GetRunGatePort(3),
                        g_sExtNetComIPaddr, GetRunGatePort(4),
                        g_sExtNetComIPaddr, GetRunGatePort(5),
                        g_sExtNetComIPaddr, GetRunGatePort(6)));
                    break;

                case 8:
                    addrLineList.Add(GameCenterFormat.Format("%s %s %d %s %d %s %d %s %d %s %d %s %d %s %d %s %d", g_sLocalIPaddr1,
                      g_sExtNetComIPaddr, GetRunGatePort(0),
                        g_sExtNetComIPaddr, GetRunGatePort(1),
                        g_sExtNetComIPaddr, GetRunGatePort(2),
                        g_sExtNetComIPaddr, GetRunGatePort(3),
                        g_sExtNetComIPaddr, GetRunGatePort(4),
                        g_sExtNetComIPaddr, GetRunGatePort(5),
                        g_sExtNetComIPaddr, GetRunGatePort(6),
                        g_sExtNetComIPaddr, GetRunGatePort(7)));
                    break;
            }
        }

        // 原文：地址行在 IniGameConf.Free 之后写入 —— 即追加到 [GateDBPort0] 节尾部。
        // （原文此处 SaveList 已 Clear 过，HostType 的 !GateList.ini 地址行因此写在节内；
        //   逐字保留该行为，见批次报告"原文缺陷/易错点"。）
        foreach (string line in addrLineList.AsEnumerable())
            AppendLineToFile(sFileName, line);

        // 原文如此（GMain.pas:1933）：SaveList 在这里已经再次累积过地址列表
        // （见 GMain.pas:1748-1749 SaveToFile(AddrTableFile) / 1750 SaveList.Clear / 1933 SaveToFile(ServerinfoFile)）。
        SaveList.Add(g_sLocalIPaddr);

        if (g_boDoubleLineMode)
        {
            SaveList.Add(g_sLocalIPaddr1);
            SaveList.Add(g_sExtIPaddr);
            SaveList.Add(g_sExtNetComIPaddr);
        }
        else
        {
            SaveList.Add(g_sExtIPaddr);
        }

        SaveList.SaveToFile(sIniFile + g_sDBServer_ServerinfoFile);

        sIniFile = g_sGameDirectory + g_sDBServer_Directory + g_sDBServer_Config_Dir;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }
        sIniFile = g_sGameDirectory + g_sDBServer_Directory + g_sDBServer_Config_IdDir;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }
        sIniFile = g_sGameDirectory + g_sDBServer_Directory + g_sDBServer_Config_HumDir;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }
        sIniFile = g_sGameDirectory + g_sDBServer_Directory + g_sDBServer_Config_FeeDir;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }
        sIniFile = g_sGameDirectory + g_sDBServer_Directory + g_sDBServer_Config_BackupDir;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }
        sIniFile = g_sGameDirectory + g_sDBServer_Directory + g_sDBServer_Config_ConnectDir;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }
        sIniFile = g_sGameDirectory + g_sDBServer_Directory + g_sDBServer_Config_LogDir;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }

        DtpDateTimeSetter?.Invoke(DelphiRTL.Now());
    }

    // ===================== GMain.pas:1976-2089 GenLoginServerConfig =====================

    /// <summary>GMain.pas:1976 <c>procedure TfrmMain.GenLoginServerConfig;</c></summary>
    public static void GenLoginServerConfig()
    {
        string sIniFile = g_sGameDirectory + g_sLoginServer_Directory;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }

        using (var IniGameConf = new GameCenterIniFile(sIniFile + g_sLoginServer_ConfigFile))
        {
            IniGameConf.WriteInteger("Server", "ReadyServers", g_sLoginServer_ReadyServers);

            IniGameConf.WriteString("Server", "EnableMakingID", DelphiSystem.BoolToStr(g_sLoginServer_EnableMakingID));
            IniGameConf.WriteString("Server", "EnableTrial", DelphiSystem.BoolToStr(g_sLoginServer_EnableTrial));
            IniGameConf.WriteString("Server", "TestServer", DelphiSystem.BoolToStr(g_sLoginServer_TestServer));
            // 增加动态IP支持 piaoyun 2013-08-30
            IniGameConf.WriteBool("Server", "DynamicIPMode", g_boDynamicIPMode);
            IniGameConf.WriteString("Server", "GateAddr", g_sAllIPaddr);                                      //g_sLoginServer_GateAddr
            IniGameConf.WriteInteger("Server", "GatePort", g_nLoginServer_GatePort);
            IniGameConf.WriteString("Server", "ServerAddr", g_sAllIPaddr);                                    //g_sLoginServer_ServerAddr
            IniGameConf.WriteString("Server", "ServerName", g_sGameName);
            IniGameConf.WriteInteger("Server", "ServerPort", g_nLoginServer_ServerPort);
            IniGameConf.WriteInteger("Server", "ControlPort", g_nLoginServer_ControlPort);

            IniGameConf.WriteString("DB", "IdDir", sIniFile + g_sLoginServer_IdDir);
            IniGameConf.WriteString("DB", "FeedIDList", sIniFile + g_sLoginServer_FeedIDList);
            IniGameConf.WriteString("DB", "FeedIPList", sIniFile + g_sLoginServer_FeedIPList);
            IniGameConf.WriteString("DB", "CountLogDir", sIniFile + g_sLoginServer_CountLogDir);
            IniGameConf.WriteString("DB", "WebLogDir", sIniFile + g_sLoginServer_WebLogDir);

            IniGameConf.WriteString("DB", "ChrLogDir", sIniFile + g_sLoginServer_ChrLogDir);
            IniGameConf.WriteString("DB", "IDLogDir", sIniFile + g_sLoginServer_IDLogDir);

            IniGameConf.WriteInteger("DataSaveDB", "DataSaveDBType", g_nDataSaveDBType);
            IniGameConf.WriteString("DataSaveDB", "DataSaveDBServer", g_sDataSaveDBServer);
            IniGameConf.WriteInteger("DataSaveDB", "DataSaveDBPort", g_wDataSaveDBPort);
            IniGameConf.WriteString("DataSaveDB", "DataSaveDBUser", g_sDataSaveDBUser);
            IniGameConf.WriteString("DataSaveDB", "DataSaveDBPassword", g_sDataSaveDBPassword);
            IniGameConf.WriteString("DataSaveDB", "DataSaveDataBase", g_sDataSaveDataBase);

            IniGameConf.UpdateFile();
        }

        var SaveList = new TStringList();

        if ((!SelGate.boGetStart) && (!SelGate1.boGetStart))
        {
            SaveList.Add(DelphiFormat.Format("%s %s %s %s %s:%d", g_sGameName, "Title1", g_sLocalIPaddr, g_sLocalIPaddr, g_sExtIPaddr, g_nSeLGate_GatePort));
            if (g_boDoubleLineMode)
                SaveList.Add(DelphiFormat.Format("%s %s %s %s %s:%d", g_sGameName, "Title2", g_sLocalIPaddr1, g_sLocalIPaddr1, g_sExtNetComIPaddr, g_nSeLGate_GatePort));
        }
        else if (SelGate.boGetStart && SelGate1.boGetStart)
        {
            SaveList.Add(DelphiFormat.Format("%s %s %s %s %s:%d %s:%d", g_sGameName, "Title1", g_sLocalIPaddr, g_sLocalIPaddr, g_sExtIPaddr, g_nSeLGate_GatePort, g_sExtIPaddr, g_nSeLGate_GatePort1));
            if (g_boDoubleLineMode)
                SaveList.Add(DelphiFormat.Format("%s %s %s %s %s:%d %s:%d", g_sGameName, "Title2", g_sLocalIPaddr1, g_sLocalIPaddr1, g_sExtNetComIPaddr, g_nSeLGate_GatePort, g_sExtIPaddr, g_nSeLGate_GatePort1));
        }
        else if (SelGate.boGetStart)
        {
            SaveList.Add(DelphiFormat.Format("%s %s %s %s %s:%d", g_sGameName, "Title1", g_sLocalIPaddr, g_sLocalIPaddr, g_sExtIPaddr, g_nSeLGate_GatePort));
            if (g_boDoubleLineMode)
                SaveList.Add(DelphiFormat.Format("%s %s %s %s %s:%d", g_sGameName, "Title2", g_sLocalIPaddr1, g_sLocalIPaddr1, g_sExtNetComIPaddr, g_nSeLGate_GatePort));
        }
        else
        {
            SaveList.Add(DelphiFormat.Format("%s %s %s %s %s:%d", g_sGameName, "Title1", g_sLocalIPaddr, g_sLocalIPaddr, g_sExtIPaddr, g_nSeLGate_GatePort1));
            if (g_boDoubleLineMode)
                SaveList.Add(DelphiFormat.Format("%s %s %s %s %s:%d", g_sGameName, "Title2", g_sLocalIPaddr1, g_sLocalIPaddr1, g_sExtNetComIPaddr, g_nSeLGate_GatePort1));
        }

        SaveList.SaveToFile(sIniFile + g_sLoginServer_AddrTableFile);

        SaveList.Clear();

        SaveList.Add(g_sLocalIPaddr);

        if (g_boDoubleLineMode)
        {
            SaveList.Add(g_sLocalIPaddr1);
            SaveList.Add(g_sExtIPaddr);
            SaveList.Add(g_sExtNetComIPaddr);
        }
        else
        {
            SaveList.Add(g_sExtIPaddr);
        }

        SaveList.SaveToFile(sIniFile + g_sLoginServer_ServeraddrFile);

        SaveList.Clear();
        SaveList.Add(DelphiFormat.Format("%s %s %d", g_sGameName, g_sGameName, g_nLimitOnlineUser));
        SaveList.SaveToFile(sIniFile + g_sLoginServerUserLimitFile);

        sIniFile = g_sGameDirectory + g_sLoginServer_Directory + g_sLoginServer_IdDir;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }

        sIniFile = g_sGameDirectory + g_sLoginServer_Directory + g_sLoginServer_CountLogDir;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }

        sIniFile = g_sGameDirectory + g_sLoginServer_Directory + g_sLoginServer_WebLogDir;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }
    }

    // ===================== GMain.pas:2091-2113 GenLogServerConfig =====================

    /// <summary>GMain.pas:2091 <c>procedure TfrmMain.GenLogServerConfig;</c></summary>
    public static void GenLogServerConfig()
    {
        string sIniFile = g_sGameDirectory + g_sLogServer_Directory;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }

        using (var IniGameConf = new GameCenterIniFile(sIniFile + g_sLogServer_ConfigFile))
        {
            IniGameConf.WriteString("Setup", "ServerName", g_sGameName);
            IniGameConf.WriteInteger("Setup", "Port", g_nLogServer_Port);
            IniGameConf.WriteString("Setup", "BaseDir", sIniFile + g_sLogServer_BaseDir);

            string sBaseDir = sIniFile + g_sLogServer_BaseDir;
            if (!Directory.Exists(sBaseDir))
            {
                Directory.CreateDirectory(sBaseDir);
            }

            IniGameConf.UpdateFile();
        }
    }

    // ===================== GMain.pas:2115-2245 GenM2ServerConfig =====================

    /// <summary>GMain.pas:2115 <c>procedure TfrmMain.GenM2ServerConfig;</c></summary>
    public static void GenM2ServerConfig()
    {
        string sIniFile = g_sGameDirectory + g_sM2Server_Directory;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }

        using (var IniGameConf = new GameCenterIniFile(sIniFile + g_sM2Server_ConfigFile))
        {
            IniGameConf.WriteString("Server", "ServerName", g_sGameName);
            IniGameConf.WriteInteger("Server", "ServerNumber", g_nM2Server_ServerNumber);
            IniGameConf.WriteInteger("Server", "ServerIndex", g_nM2Server_ServerIndex);

            IniGameConf.WriteInteger("Server", "EditionId", g_nM2Server_EditionId);
            IniGameConf.WriteInteger("Server", "AreaId", g_nM2Server_AreaId);

            IniGameConf.WriteString("Server", "VentureServer", DelphiSystem.BoolToStr(g_boM2Server_VentureServer));
            IniGameConf.WriteString("Server", "TestServer", DelphiSystem.BoolToStr(g_boM2Server_TestServer));
            IniGameConf.WriteInteger("Server", "TestLevel", g_nM2Server_TestLevel);
            IniGameConf.WriteInteger("Server", "TestGold", g_nM2Server_TestGold);
            IniGameConf.WriteInteger("Server", "TestServerUserLimit", g_nLimitOnlineUser);
            IniGameConf.WriteString("Server", "ServiceMode", DelphiSystem.BoolToStr(g_boM2Server_ServiceMode));
            IniGameConf.WriteString("Server", "NonPKServer", DelphiSystem.BoolToStr(g_boM2Server_NonPKServer));

            IniGameConf.WriteString("Server", "DBAddr", g_sDBServer_Config_ServerAddr);
            IniGameConf.WriteInteger("Server", "DBPort", g_nDBServer_Config_ServerPort);
            IniGameConf.WriteString("Server", "IDSAddr", g_sLoginServer_ServerAddr);
            IniGameConf.WriteInteger("Server", "IDSPort", g_nLoginServer_ServerPort);
            IniGameConf.WriteString("Server", "MsgSrvAddr", g_sAllIPaddr);                                    //g_sM2Server_MsgSrvAddr
            IniGameConf.WriteInteger("Server", "MsgSrvPort", g_nM2Server_MsgSrvPort);
            IniGameConf.WriteString("Server", "LogServerAddr", g_sLogServer_ServerAddr);
            IniGameConf.WriteInteger("Server", "LogServerPort", g_nLogServer_Port);
            IniGameConf.WriteString("Server", "GateAddr", g_sAllIPaddr);                                      //g_sM2Server_GateAddr
            IniGameConf.WriteInteger("Server", "GatePort", g_nM2Server_GatePort);

            IniGameConf.WriteBool("Server", "UseSqliteDB", g_boUseSqliteDB);
            IniGameConf.WriteString("Server", "DBName", g_sHeroDBName);
            IniGameConf.WriteString("Server", "SqliteDBName", g_sSqliteDBName);

            IniGameConf.WriteInteger("Server", "UserFull", g_nLimitOnlineUser);

            IniGameConf.WriteString("Share", "BaseDir", sIniFile + g_sM2Server_BaseDir);
            IniGameConf.WriteString("Share", "GuildDir", sIniFile + g_sM2Server_GuildDir);
            IniGameConf.WriteString("Share", "GuildFile", sIniFile + g_sM2Server_GuildFile);
            IniGameConf.WriteString("Share", "VentureDir", sIniFile + g_sM2Server_VentureDir);
            IniGameConf.WriteString("Share", "ConLogDir", sIniFile + g_sM2Server_ConLogDir);
            IniGameConf.WriteString("Share", "LogDir", sIniFile + g_sM2Server_LogDir);
            IniGameConf.WriteString("Share", "PlugDir", sIniFile);
            IniGameConf.WriteString("Share", "BoxsDir", sIniFile + g_sM2Server_BoxsDir);

            IniGameConf.WriteString("Share", "CastleDir", sIniFile + g_sM2Server_CastleDir);
            IniGameConf.WriteString("Share", "EnvirDir", sIniFile + g_sM2Server_EnvirDir);
            IniGameConf.WriteString("Share", "MapDir", sIniFile + g_sM2Server_MapDir);
            IniGameConf.WriteString("Share", "NoticeDir", sIniFile + g_sM2Server_NoticeDir);
            IniGameConf.WriteString("Share", "CastleFile", sIniFile + g_sM2Server_CastleFile);

            IniGameConf.WriteInteger("DataSaveDB", "DataSaveDBType", g_nDataSaveDBType);
            IniGameConf.WriteString("DataSaveDB", "DataSaveDBServer", g_sDataSaveDBServer);
            IniGameConf.WriteInteger("DataSaveDB", "DataSaveDBPort", g_wDataSaveDBPort);
            IniGameConf.WriteString("DataSaveDB", "DataSaveDBUser", g_sDataSaveDBUser);
            // 原文如此（GMain.pas:2180）：键名 'DataSaveDBPassword ' 尾部多一个空格，与其余六处不一致。
            IniGameConf.WriteString("DataSaveDB", "DataSaveDBPassword ", g_sDataSaveDBPassword);
            IniGameConf.WriteString("DataSaveDB", "DataSaveDataBase", g_sDataSaveDataBase);

            IniGameConf.UpdateFile();
        }

        sIniFile = g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_BaseDir;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }
        sIniFile = g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_GuildDir;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }
        sIniFile = g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_VentureDir;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }
        sIniFile = g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_ConLogDir;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }
        sIniFile = g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_LogDir;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }
        sIniFile = g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_CastleDir;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }
        sIniFile = g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_EnvirDir;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }
        sIniFile = g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_MapDir;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }
        sIniFile = g_sGameDirectory + g_sM2Server_Directory + g_sM2Server_NoticeDir;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }

        sIniFile = g_sGameDirectory + g_sM2Server_Directory;
        var SaveList = new TStringList();
        SaveList.Add("GM");
        SaveList.SaveToFile(sIniFile + g_sM2Server_AbuseFile);

        SaveList.Clear();
        SaveList.Add(g_sLocalIPaddr);
        SaveList.SaveToFile(sIniFile + g_sM2Server_RunAddrFile);

        SaveList.Clear();
        SaveList.Add(g_sLocalIPaddr);
        SaveList.SaveToFile(sIniFile + g_sM2Server_ServerTableFile);
    }

    // ===================== GMain.pas:2247-2271 GenLoginGateConfig =====================

    /// <summary>GMain.pas:2247 <c>procedure TfrmMain.GenLoginGateConfig;</c></summary>
    public static void GenLoginGateConfig()
    {
        string sIniFile = g_sGameDirectory + g_sLoginGate_Directory;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }
        using var IniGameConf = new GameCenterIniFile(sIniFile + g_sLoginGate_ConfigFile);
        IniGameConf.WriteString("LoginGate", "Title", g_sGameName);
        IniGameConf.WriteString("LoginGate", "GateAddr", g_sLoginGate_GateAddr);

        IniGameConf.WriteString("LoginGate", "ServerAddr", g_sLoginGate_ServerAddr);
        // 原文如此（GMain.pas:2262）：ServerPort 取 g_nLoginServer_GatePort，
        // 被注释掉的 g_nLoginGate_ServerPort 不生效。
        IniGameConf.WriteInteger("LoginGate", "ServerPort", g_nLoginServer_GatePort);
        IniGameConf.WriteInteger("LoginGate", "GatePort", g_nLoginGate_GatePort);

        IniGameConf.WriteInteger("LoginGate", "Count", 1);
        IniGameConf.WriteString("LoginGate", "ServerAddr1", g_sLoginGate_ServerAddr);
        IniGameConf.WriteInteger("LoginGate", "ServerPort1", g_nLoginServer_GatePort);
        IniGameConf.WriteInteger("LoginGate", "GatePort1", g_nLoginGate_GatePort);

        IniGameConf.UpdateFile();
    }

    // ===================== GMain.pas:2273-2295 GenSelGateConfig =====================

    /// <summary>GMain.pas:2273 <c>procedure TfrmMain.GenSelGateConfig();</c></summary>
    public static void GenSelGateConfig()
    {
        string sIniFile = g_sGameDirectory + g_sSelGate_Directory;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }
        using var IniGameConf = new GameCenterIniFile(sIniFile + g_sSelGate_ConfigFile);
        IniGameConf.WriteString("SelGate", "Title", g_sGameName);
        IniGameConf.WriteString("SelGate", "ServerAddr", g_sSelGate_ServerAddr);
        // 原文如此（GMain.pas:2286）：ServerPort 取 g_nDBServer_Config_GatePort。
        IniGameConf.WriteInteger("SelGate", "ServerPort", g_nDBServer_Config_GatePort);
        IniGameConf.WriteString("SelGate", "GateAddr", g_sSelGate_GateAddr);
        IniGameConf.WriteInteger("SelGate", "GatePort", g_nSeLGate_GatePort);
        // 原文注释（GMain.pas:2289）：//IniGameConf.WriteBool('SelGate', 'DynamicIPDisMode', g_boDynamicIPMode);
        /* 原文注释块（GMain.pas:2290-2293）：
        IniGameConf.WriteInteger('SelGate', 'ShowLogLevel', g_nSelGate_ShowLogLevel);
        //IniGameConf.WriteInteger('SelGate', 'MaxConnOfIPaddr', g_nSelGate_MaxConnOfIPaddr);
        IniGameConf.WriteInteger('SelGate', 'BlockMethod', g_nSelGate_BlockMethod);
        IniGameConf.WriteInteger('SelGate', 'KeepConnectTimeOut', g_nSelGate_KeepConnectTimeOut); */
        IniGameConf.UpdateFile();
    }

    // ===================== GMain.pas:2297-2350 GenMutLoginGateConfig =====================

    /// <summary>
    /// GMain.pas:2297 <c>procedure TfrmMain.GenMutLoginGateConfig(nIndex: Integer);</c>
    /// 差异说明：原文 case 只有 0/1 两支，其余 nIndex 下 sGateAddr/nGatePort/sServerAddr
    /// 保持上一次调用的栈值（Delphi 未初始化局部变量语义）；托管侧为类型默认值（""/0/""）。
    /// </summary>
    public static void GenMutLoginGateConfig(int nIndex)
    {
        string sIniFile = g_sGameDirectory + g_sLoginGate_Directory;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }

        string sGateAddr = "";
        int nGatePort = 0;
        string sServerAddr = "";

        if (g_boDoubleLineMode)
        {
            switch (nIndex)
            {
                case 0:
                    {
                        sGateAddr = g_sExtIPaddr;
                        nGatePort = g_nLoginGate_GatePort;
                        sServerAddr = g_sLoginGate_ServerAddr;
                    }
                    break;
                case 1:
                    {
                        sGateAddr = g_sExtNetComIPaddr;
                        nGatePort = g_nLoginGate_GatePort;
                        sServerAddr = g_sLoginGate_ServerAddr1;
                    }
                    break;
            }
        }
        else
        {
            sGateAddr = g_sAllIPaddr;
            nGatePort = g_nLoginGate_GatePort;
            sServerAddr = g_sLoginGate_ServerAddr;
        }

        using var IniGameConf = new GameCenterIniFile(sIniFile + g_sLoginGate_ConfigFile);
        IniGameConf.WriteString("LoginGate", "Title", g_sGameName);

        IniGameConf.WriteString("LoginGate", "ServerAddr", sServerAddr);
        IniGameConf.WriteInteger("LoginGate", "ServerPort", g_nLoginServer_GatePort);
        IniGameConf.WriteString("LoginGate", "GateAddr", sGateAddr);
        IniGameConf.WriteInteger("LoginGate", "GatePort", nGatePort);
        /* 原文注释块（GMain.pas:2342-2347）：
        IniGameConf.WriteInteger('LoginGate', 'Count', 1);
        IniGameConf.WriteString('LoginGate', 'ServerAddr1', g_sLoginGate_ServerAddr);
        IniGameConf.WriteInteger('LoginGate', 'ServerPort1', g_nLoginServer_GatePort);
        IniGameConf.WriteInteger('LoginGate', 'GatePort1', g_nLoginGate_GatePort);
        */

        IniGameConf.UpdateFile();
    }

    // ===================== GMain.pas:2353-2367 GetMutLoginGateZero =====================

    /// <summary>GMain.pas:2353 <c>procedure TfrmMain.GetMutLoginGateZero;</c>（2019-09-28 16:18:10）</summary>
    public static void GetMutLoginGateZero()
    {
        // 原文如此（GMain.pas:2358 + 2364）：目录用 g_sSelGate_Directory，文件名用 g_sLoginGate_ConfigFile。
        string sIniFile = g_sGameDirectory + g_sSelGate_Directory;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }

        using var IniGameConf = new GameCenterIniFile(sIniFile + g_sLoginGate_ConfigFile);
        IniGameConf.WriteInteger("LoginGate", "Count", 0);
        IniGameConf.UpdateFile();
    }

    // ===================== GMain.pas:2369-2404 GenMutSelGateConfigEx =====================

    /// <summary>GMain.pas:2369 <c>procedure TfrmMain.GenMutSelGateConfigEx;</c>（2019-09-28 16:18:04）</summary>
    public static void GenMutSelGateConfigEx()
    {
        string sIniFile = g_sGameDirectory + g_sSelGate_Directory;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }

        using var IniGameConf = new GameCenterIniFile(sIniFile + g_sSelGate_ConfigFile);
        int SelGateIndex = 0;
        if (SelGate.boGetStart)
        {
            SelGateIndex++;
            IniGameConf.WriteString("SelGates_iocp", "ServerAddr" + DelphiSystem.IntToStr(SelGateIndex), g_sSelGate_ServerAddr);
            IniGameConf.WriteInteger("SelGates_iocp", "ServerPort" + DelphiSystem.IntToStr(SelGateIndex), g_nDBServer_Config_GatePort);
            IniGameConf.WriteInteger("SelGates_iocp", "GatePort" + DelphiSystem.IntToStr(SelGateIndex), g_nSeLGate_GatePort);
        }

        if (SelGate1.boGetStart)
        {
            SelGateIndex++;
            IniGameConf.WriteString("SelGates_iocp", "ServerAddr" + DelphiSystem.IntToStr(SelGateIndex), g_sSelGate_ServerAddr);
            IniGameConf.WriteInteger("SelGates_iocp", "ServerPort" + DelphiSystem.IntToStr(SelGateIndex), g_nDBServer_Config_GatePort);
            IniGameConf.WriteInteger("SelGates_iocp", "GatePort" + DelphiSystem.IntToStr(SelGateIndex), g_nSeLGate_GatePort1);
        }

        IniGameConf.WriteInteger("SelGates_iocp", "Count", SelGateIndex);

        IniGameConf.UpdateFile();
    }

    // ===================== GMain.pas:2406-2420 GetMutSelGateZero =====================

    /// <summary>GMain.pas:2406 <c>procedure TfrmMain.GetMutSelGateZero;</c>（2019-09-28 16:18:10）</summary>
    public static void GetMutSelGateZero()
    {
        string sIniFile = g_sGameDirectory + g_sSelGate_Directory;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }

        using var IniGameConf = new GameCenterIniFile(sIniFile + g_sSelGate_ConfigFile);
        IniGameConf.WriteInteger("SelGates_iocp", "Count", 0);
        IniGameConf.UpdateFile();
    }

    // ===================== GMain.pas:2422-2458 GenMutSelGateConfig =====================

    /// <summary>
    /// GMain.pas:2422 <c>procedure TfrmMain.GenMutSelGateConfig(nIndex: Integer);</c>
    /// 差异说明同 <see cref="GenMutLoginGateConfig"/>：case 缺省支的未初始化局部变量按托管默认值处理。
    /// </summary>
    public static void GenMutSelGateConfig(int nIndex)
    {
        string sIniFile = g_sGameDirectory + g_sSelGate_Directory;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }

        string sGateAddr = "";
        int nGatePort = 0;
        string sServerAddr = "";

        switch (nIndex)
        {
            case 0:
                {
                    sGateAddr = g_sSelGate_GateAddr;
                    nGatePort = g_nSeLGate_GatePort;
                    sServerAddr = g_sSelGate_ServerAddr;
                }
                break;
            case 1:
                {
                    sGateAddr = g_sSelGate_GateAddr1;
                    nGatePort = g_nSeLGate_GatePort1;
                    sServerAddr = g_sSelGate_ServerAddr;
                }
                break;
        }
        using var IniGameConf = new GameCenterIniFile(sIniFile + g_sSelGate_ConfigFile);
        IniGameConf.WriteString("SelGate", "Title", g_sGameName);

        IniGameConf.WriteString("SelGate", "ServerAddr", sServerAddr);
        // 原文如此（GMain.pas:2454）：ServerPort 取 g_nDBServer_Config_GatePort。
        IniGameConf.WriteInteger("SelGate", "ServerPort", g_nDBServer_Config_GatePort);
        IniGameConf.WriteString("SelGate", "GateAddr", sGateAddr);
        IniGameConf.WriteInteger("SelGate", "GatePort", nGatePort);
        IniGameConf.UpdateFile();
    }

    // ===================== GMain.pas:2460-2481 GetMutRunGateConfing =====================

    /// <summary>GMain.pas:2460 <c>procedure TfrmMain.GetMutRunGateConfing(nGateIndex: Integer);</c>
    /// （原文如此：方法名 Confing 为笔误）。</summary>
    public static void GetMutRunGateConfing(int nGateIndex)
    {
        if ((nGateIndex >= 0) && (nGateIndex < GShareConst.MAXRUNGATECOUNT))
        {
            string sIniFile = g_sGameDirectory + g_sRunGate_Directory;
            if (!Directory.Exists(sIniFile))
            {
                Directory.CreateDirectory(sIniFile);
            }
            using var IniGameConf = new GameCenterIniFile(sIniFile + g_sRunGate_ConfigFile);
            IniGameConf.WriteString("GameGate", "Title", g_sGameName + "(" + DelphiSystem.IntToStr(g_RunGateInfo[nGateIndex].nGatePort) + ")");
            IniGameConf.WriteString("GameGate", "ServerAddr", g_sRunGate_ServerAddr);
            IniGameConf.WriteInteger("GameGate", "ServerPort", g_nM2Server_GatePort);
            IniGameConf.WriteString("GameGate", "GateAddr", g_sAllIPaddr);                                   //g_RunGateInfo[nGateIndex].sGateAddr
            IniGameConf.WriteInteger("GameGate", "GatePort", g_RunGateInfo[nGateIndex].nGatePort);
            IniGameConf.WriteInteger("GameGate", "DBPort", g_RunGateInfo[nGateIndex].nDBPort);
            IniGameConf.UpdateFile();
        }
    }

    // ===================== GMain.pas:2483-2519 GetMutRunGateConfingEx =====================

    /// <summary>GMain.pas:2483 <c>procedure TfrmMain.GetMutRunGateConfingEx;</c>
    /// （add chongchong 多线程网关配置生成 2014-06-22）。</summary>
    public static void GetMutRunGateConfingEx()
    {
        string sIniFile = g_sGameDirectory + g_sRunGate_Directory;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }

        using var IniGameConf = new GameCenterIniFile(sIniFile + g_sRunGate_ConfigFile);
        int RunGateIndex = 0;
        string PortStr = "";
        for (int I = 0; I <= g_RunGateInfo.Length - 1; I++)
        {
            if (g_RunGateInfo[I].boGetStart)
            {
                RunGateIndex++;
                IniGameConf.WriteInteger("GameGates", "Port" + DelphiSystem.IntToStr(RunGateIndex), g_RunGateInfo[I].nGatePort);
                PortStr = PortStr + DelphiSystem.IntToStr(g_RunGateInfo[I].nGatePort) + ",";
            }
        }
        if (PortStr.Length > 0)
            PortStr = "(" + DelphiRTL.Copy(PortStr, 1, PortStr.Length - 1) + ")";

        IniGameConf.WriteString("GameGate", "Title", PortStr);
        IniGameConf.WriteInteger("GameGates", "Count", RunGateIndex);

        IniGameConf.WriteString("GameGate", "ServerAddr", g_sRunGate_ServerAddr);
        IniGameConf.WriteInteger("GameGate", "ServerPort", g_nM2Server_GatePort);
        IniGameConf.WriteString("GameGate", "GateAddr", g_sAllIPaddr);
        // 原文如此（GMain.pas:2516）：注释块 {'Server'RunGate} 被剥离后键为 'DBPort'，写在 [GameGate] 节。
        IniGameConf.WriteInteger("GameGate", "DBPort", g_nRunGateDBPort_MulThread);

        IniGameConf.UpdateFile();
    }

    // ===================== GMain.pas:2521-2535 GetMutRunGateZero =====================

    /// <summary>GMain.pas:2521 <c>procedure TfrmMain.GetMutRunGateZero;</c></summary>
    public static void GetMutRunGateZero()
    {
        string sIniFile = g_sGameDirectory + g_sRunGate_Directory;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }

        using var IniGameConf = new GameCenterIniFile(sIniFile + g_sRunGate_ConfigFile);
        IniGameConf.WriteInteger("GameGates", "Count", 0);
        IniGameConf.UpdateFile();
    }

    // ===================== GMain.pas:2537-2606 GenRunGateConfig =====================

    /// <summary>GMain.pas:2537 <c>procedure TfrmMain.GenRunGateConfig;</c></summary>
    public static void GenRunGateConfig()
    {
        string sIniFile = g_sGameDirectory + g_sRunGate_Directory;
        if (!Directory.Exists(sIniFile))
        {
            Directory.CreateDirectory(sIniFile);
        }
        using (var IniGameConf = new GameCenterIniFile(sIniFile + g_sRunGate_ConfigFile))
        {
            IniGameConf.WriteString("GameGate", "Title", g_sGameName);
            g_nRunGate_Count = 0;
            //nIndex := 0;

            /* TODO -opiaoyun -c修改 : 运行网关参数配置，先配置一个~~~ 【2013-09-07】 */
            IniGameConf.WriteString("GameGate", "ServerAddr", g_sRunGate_ServerAddr);
            IniGameConf.WriteInteger("GameGate", "ServerPort", g_nM2Server_GatePort);
            IniGameConf.WriteString("GameGate", "GateAddr", g_sAllIPaddr);           //g_RunGateInfo[I].sGateAddr
            IniGameConf.WriteInteger("GameGate", "GatePort", g_RunGateInfo[0].nGatePort);

            if (g_boRunGate_GetMultiThread)
                IniGameConf.WriteInteger("GameGate", "DBPort", g_nRunGateDBPort_MulThread);
            else
                IniGameConf.WriteInteger("GameGate", "DBPort", g_RunGateInfo[0].nDBPort);

            IniGameConf.UpdateFile();
        }

        var SaveList = new TStringList();
        try
        {
            SaveList.Add(g_sDBServer_Config_ServerAddr);
            SaveList.SaveToFile(sIniFile + g_sRunGate_AddrTableFile);
        }
        finally
        {
            // 原文 finally SaveList.Free（托管侧无等价动作）。
        }

        g_nRunGate_Count++;

        // 配置其他网关
        for (int I = 1; I <= g_RunGateInfo.Length - 1; I++)
        {
            if (g_RunGateInfo[I].boGetStart)
            {
                g_nRunGate_Count++;

                sIniFile = g_sGameDirectory + DelphiFormat.Format(g_sRunGate_DirectoryEx, I);
                // 没有目录则跳过
                if (Directory.Exists(sIniFile))
                {
                    using var IniGameConf = new GameCenterIniFile(sIniFile + g_sRunGate_ConfigFile);
                    IniGameConf.WriteString("GameGate", "Title", g_sGameName);
                    IniGameConf.WriteString("GameGate", "ServerAddr", g_sRunGate_ServerAddr);
                    IniGameConf.WriteInteger("GameGate", "ServerPort", g_nM2Server_GatePort);
                    IniGameConf.WriteString("GameGate", "GateAddr", g_sAllIPaddr);           //g_RunGateInfo[I].sGateAddr
                    IniGameConf.WriteInteger("GameGate", "GatePort", g_RunGateInfo[I].nGatePort);

                    if (g_boRunGate_GetMultiThread)
                        // 原文如此（GMain.pas:2597）：多线程分支的 DBPort 写在 'GameGates' 节，
                        // 与单线程分支写入的 'GameGate' 节不一致。
                        IniGameConf.WriteInteger("GameGates", "DBPort", g_nRunGateDBPort_MulThread);
                    else
                        IniGameConf.WriteInteger("GameGate", "DBPort", g_RunGateInfo[I].nDBPort);

                    IniGameConf.UpdateFile();
                }
            }
        }
    }

    // ===================== GMain.pas:6604-6727 GenBackupConfig =====================

    /// <summary>
    /// GMain.pas:6604 <c>procedure TfrmMain.GenBackupConfig;</c>
    /// 原文使用 <c>Label DoExit</c> + <c>goto DoExit</c> 跳到 <c>RefBackListToView</c>；
    /// 托管侧在三个失败分支显式调用 <see cref="RefBackListToView"/> 后 return（分支顺序与原文一致）。
    /// </summary>
    public static void GenBackupConfig()
    {
        DeleteBackListFile();
        // 原文 TIniFile.Create 在构造时即建立空文件；失败分支（goto DoExit）会让文件保持为空。
        // 托管侧以"延迟创建"表达同一磁盘结果：只有真正写入过才落盘。
        GameCenterIniFile? Conini = null;
        GameCenterIniFile ConiniRef() => Conini ??= new GameCenterIniFile(BackListFileName());
        try
        {
            for (int I = 0; I <= g_BackUpManager.m_BackUpList.Count - 1; I++)
            {
                IBackUpTask BackUpTask = g_BackUpManager.m_BackUpList[I];

                if (g_sOldGameDirectory.Length > 0)
                {
                    if (SameText(DelphiRTL.Copy(BackUpTask.SourceDirectory, 1, g_sOldGameDirectory.Length), g_sOldGameDirectory))
                    {
                        int Len = g_sOldGameDirectory.Length;
                        BackUpTask.SourceDirectory = g_sGameDirectory + DelphiRTL.Copy(BackUpTask.SourceDirectory, Len + 1, DelphiRTL.MaxInt);
                    }
                    else
                    {
                        GameCenterDialogs.ShowMessage("请检查备份路径配置是否正确");
                        RefBackListToView?.Invoke();
                        return;
                    }

                    if (SameText(DelphiRTL.Copy(BackUpTask.DestDirectory, 1, g_sOldGameDirectory.Length), g_sOldGameDirectory))
                    {
                        int Len = g_sOldGameDirectory.Length;
                        BackUpTask.DestDirectory = g_sGameDirectory + DelphiRTL.Copy(BackUpTask.DestDirectory, Len + 1, DelphiRTL.MaxInt);
                    }
                    else
                    {
                        GameCenterDialogs.ShowMessage("请检查备份路径配置是否正确");
                        RefBackListToView?.Invoke();
                        return;
                    }
                }
                else
                {
                    GameCenterDialogs.ShowMessage("请检查备份路径配置是否正确");
                    RefBackListToView?.Invoke();
                    return;
                }

                // 原文如此（GMain.pas:6653）：节名直接用 IntToStr(I)（"0"/"1"/…），与 SaveBackList 一致。
                var ConiniW = ConiniRef();
                ConiniW.WriteString(DelphiSystem.IntToStr(I), "Source", BackUpTask.SourceDirectory);
                ConiniW.WriteString(DelphiSystem.IntToStr(I), "Save", BackUpTask.DestDirectory);
                ConiniW.WriteInteger(DelphiSystem.IntToStr(I), "Hour", BackUpTask.Hour);
                ConiniW.WriteInteger(DelphiSystem.IntToStr(I), "Min", BackUpTask.Min);
                ConiniW.WriteInteger(DelphiSystem.IntToStr(I), "BackMode", BackUpTask.Mode);
                ConiniW.WriteBool(DelphiSystem.IntToStr(I), "GetBack", BackUpTask.Start);
                // 是否压缩 piaoyun 2013-08-30
                ConiniW.WriteBool(DelphiSystem.IntToStr(I), "IsCompress", BackUpTask.IsCompress);
                ConiniW.UpdateFile();
            }

            int nMyGetTxtNum = g_IniConf.ReadInteger("ClearServer", "MyGetTxtNum", 0);
            int nMyGetFileNum = g_IniConf.ReadInteger("ClearServer", "MyGetFileNum", 0);
            int nMyGetDirNum = g_IniConf.ReadInteger("ClearServer", "MyGetDirNum", 0);
            string S;
            if (nMyGetTxtNum != 0)
            {
                ClearListCall(GMainHelpers.ListSource.MyGetTxt);
                for (int I = 0; I <= nMyGetTxtNum - 1; I++)
                {
                    S = g_IniConf.ReadString("ClearServer", "MyGetTxt" + DelphiSystem.IntToStr(I), "读取配置文件错误");
                    if (SameText(DelphiRTL.Copy(S, 1, g_sOldGameDirectory.Length), g_sOldGameDirectory))
                    {
                        int Len = g_sOldGameDirectory.Length;
                        S = g_sGameDirectory + DelphiRTL.Copy(S, Len + 1, DelphiRTL.MaxInt);
                    }
                    g_IniConf.WriteString("ClearServer", "MyGetTxt" + DelphiSystem.IntToStr(I), S);

                    AddToListCall(GMainHelpers.ListSource.MyGetTxt, S);
                }
            }

            if (nMyGetFileNum != 0)
            {
                ClearListCall(GMainHelpers.ListSource.MyGetFile);
                for (int I = 0; I <= nMyGetFileNum - 1; I++)
                {
                    S = g_IniConf.ReadString("ClearServer", "MyGetFile" + DelphiSystem.IntToStr(I), "读取配置文件错误");

                    if (SameText(DelphiRTL.Copy(S, 1, g_sOldGameDirectory.Length), g_sOldGameDirectory))
                    {
                        int Len = g_sOldGameDirectory.Length;
                        S = g_sGameDirectory + DelphiRTL.Copy(S, Len + 1, DelphiRTL.MaxInt);
                    }
                    g_IniConf.WriteString("ClearServer", "MyGetFile" + DelphiSystem.IntToStr(I), S);

                    AddToListCall(GMainHelpers.ListSource.MyGetFile, S);
                }
            }
            if (nMyGetDirNum != 0)
            {
                ClearListCall(GMainHelpers.ListSource.MyGetDir);
                for (int I = 0; I <= nMyGetDirNum - 1; I++)
                {
                    S = g_IniConf.ReadString("ClearServer", "MyGetDir" + DelphiSystem.IntToStr(I), "读取配置文件错误");

                    if (SameText(DelphiRTL.Copy(S, 1, g_sOldGameDirectory.Length), g_sOldGameDirectory))
                    {
                        int Len = g_sOldGameDirectory.Length;
                        S = g_sGameDirectory + DelphiRTL.Copy(S, Len + 1, DelphiRTL.MaxInt);
                    }
                    g_IniConf.WriteString("ClearServer", "MyGetDir" + DelphiSystem.IntToStr(I), S);

                    AddToListCall(GMainHelpers.ListSource.MyGetDir, S);
                }
            }
        }
        finally
        {
            // 原文 finally: if Conini <> nil then Conini.Free（Free 触发 UpdateFile）。
            if (Conini != null)
            {
                Conini.UpdateFile();
                Conini.Dispose();
            }
        }

        // DoExit:
        RefBackListToView?.Invoke();
    }

    // ---------------- GenBackupConfig 的宿主接缝（ListBox 访问复用 GMainHelpers 的同一组接缝） ----------------

    /// <summary>GMain.pas:6614 <c>ExtractFilePath(ParamStr(0)) + 'BackList.txt'</c> 的文件名提供者（测试注入临时目录）。</summary>
    public static Func<string> BackListFileNameProvider = () => Path.Combine(AppContext.BaseDirectory, "BackList.txt");

    /// <summary>GMain.pas:6614 <c>DeleteFile(ExtractFilePath(ParamStr(0)) + 'BackList.txt')</c>。</summary>
    public static void DeleteBackListFile()
    {
        string f = BackListFileName();
        if (File.Exists(f)) File.Delete(f);
    }

    /// <summary>当前 BackList.txt 全路径。</summary>
    public static string BackListFileName() => BackListFileNameProvider();

    /// <summary>GMain.pas:6722 <c>RefBackListToView</c> 接缝。</summary>
    public static Action? RefBackListToView;

    /// <summary>GMain.pas:6670/6688/6705 <c>frmMain.lst*.Items.Clear</c>（复用 GMainHelpers 接缝）。</summary>
    private static void ClearListCall(GMainHelpers.ListSource src) => GMainHelpers.ClearList(src);

    /// <summary>GMain.pas:6682/6700/6717 <c>frmMain.lst*.Items.Add(S)</c>（复用 GMainHelpers 接缝）。</summary>
    private static void AddToListCall(GMainHelpers.ListSource src, string s) => GMainHelpers.AddToList(src, s);

    /// <summary>System.SysUtils <c>SameText</c>（忽略大小写比较）。</summary>
    public static bool SameText(string a, string b)
        => string.Equals(a ?? "", b ?? "", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// 等价于原文 <c>SaveList.SaveToFile(已有的 !GateList.ini)</c>：
    /// Delphi <c>TStrings.SaveToFile</c> 会**截断并覆盖**目标文件；这里第一步已写入 INI 缓存内容，
    /// 因此把"要追加的行"追加到文件末尾（GBK + CRLF）。
    /// </summary>
    private static void AppendLineToFile(string path, string line)
    {
        File.AppendAllText(path, line + "\r\n", GXX.Core.EncodingInit.GBK);
    }
}
