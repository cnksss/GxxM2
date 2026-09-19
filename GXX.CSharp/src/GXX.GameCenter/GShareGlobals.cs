using System;
using System.Collections.Generic;
using System.Reflection;
using GXX.Core.Util;

namespace GXX.GameCenter;

/// <summary>
/// GShare.pas 第 56..283 行 <c>var</c> 段 1:1 移植（GameCenter 全局配置状态）。
/// 命名保留 Delphi 原名（g_ 前缀），类型按转换文档 §3.1 映射：
/// String→string、Integer→int、Word→ushort、LongWord→uint、Boolean→bool、TList→List&lt;T&gt;。
/// </summary>
/// <remarks>
/// 逐行来源：GShare.pas 中的每个字段都在下方以 <c>// GShare.pas:NN</c> 标注其原始行号。
/// <c>g_BackUpManager: TBackUpManager</c>（DataBackUp.pas）与 <c>g_IniConf: Tinifile</c> 见 §接缝说明。
/// </remarks>
public static class GShareGlobals
{
    // ---------------- 备份管理器 / 单实例（GShare.pas:57-58） ----------------

    /// <summary>
    /// GShare.pas:57 <c>g_BackUpManager: TBackUpManager;</c>
    /// 接缝：待 DataBackUp.pas（TBackUpManager/TBackUpTask）移植后接入；
    /// 当前由 <see cref="IBackUpManager"/> 最小接口承载，仅暴露 SaveBackList/LoadBackList 所需的成员。
    /// </summary>
    public static IBackUpManager? g_BackUpManager;

    /// <summary>GShare.pas:58 <c>g_boHeroDBOK: Boolean = False;</c></summary>
    public static bool g_boHeroDBOK = false;

    // ---------------- 游戏名称 / 窗体序号 ----------------

    /// <summary>GShare.pas:60 <c>g_sGameName: string;</c>（默认空串，实际由 LoadConfig 读 Config.ini 的 GameName，缺省 m_sGameName='BmM2'）。</summary>
    public static string g_sGameName = "";

    /// <summary>GShare.pas:61 <c>g_nFormIdx: Integer;</c></summary>
    public static int g_nFormIdx = 0;

    // ---------------- 主配置文件 g_IniConf（GShare.pas:62） ----------------

    /// <summary>
    /// GShare.pas:62 <c>g_IniConf: Tinifile;</c>，在 initialization 段以
    /// <c>Tinifile.Create(ExtractFilePath(ParamStr(0)) + g_sConfFile)</c> 建立（GShare.pas:532）。
    /// 接缝：INI 读写由 <see cref="IGameCenterIniFile"/> 抽象，默认实现为
    /// <see cref="GameCenterIniFile"/>（保序 + 追加式落盘，见 §GMain.cs）。
    /// </summary>
    public static IGameCenterIniFile? g_IniConf;

    // ---------------- 按钮标题（GShare.pas:63-66，对应 GMain.pas:1573 起） ----------------

    /// <summary>GShare.pas:63 <c>g_sButtonStartGame = '启动游戏服务器(&amp;S)'</c></summary>
    public static string g_sButtonStartGame = "启动游戏服务器(&S)";

    /// <summary>GShare.pas:64 <c>g_sButtonStopGame = '停止游戏服务器(&amp;T)'</c></summary>
    public static string g_sButtonStopGame = "停止游戏服务器(&T)";

    /// <summary>GShare.pas:65 <c>g_sButtonStopStartGame = '中止启动游戏服务器(&amp;T)'</c></summary>
    public static string g_sButtonStopStartGame = "中止启动游戏服务器(&T)";

    /// <summary>GShare.pas:66 <c>g_sButtonStopStopGame = '中止停止游戏服务器(&amp;T)'</c></summary>
    public static string g_sButtonStopStopGame = "中止停止游戏服务器(&T)";

    // ---------------- 目录与配置文件（GShare.pas:68-70） ----------------

    /// <summary>GShare.pas:68 <c>g_sConfFile: string = '.\Config.ini';</c>（原文如此：值里已含 "<c>.\</c>"，拼接时再加 ExtractFilePath）。</summary>
    public static string g_sConfFile = @".\Config.ini";

    /// <summary>GShare.pas:69 <c>g_sGameDirectory: string = 'D:\MirServer\';</c></summary>
    public static string g_sGameDirectory = @"D:\MirServer\";

    /// <summary>GShare.pas:70 <c>g_sOldGameDirectory: string = '';</c></summary>
    public static string g_sOldGameDirectory = "";

    // ---------------- 数据源（英雄库 BDE/Sqlite）（GShare.pas:72-74） ----------------

    /// <summary>GShare.pas:72 <c>g_boUseSqliteDB: Boolean = False;</c></summary>
    public static bool g_boUseSqliteDB = false;

    /// <summary>GShare.pas:73 <c>g_sHeroDBName: string = 'HeroDB';</c></summary>
    public static string g_sHeroDBName = "HeroDB";

    /// <summary>GShare.pas:74 <c>g_sSqliteDBName: string = 'D:\MirServer\Mud2\DB\BmM2.db';</c></summary>
    public static string g_sSqliteDBName = @"D:\MirServer\Mud2\DB\BmM2.db";

    // ---------------- DataSaveDB（GShare.pas:76-81） ----------------

    /// <summary>GShare.pas:76 <c>g_nDataSaveDBType: Integer = 0;</c></summary>
    public static int g_nDataSaveDBType = 0;

    /// <summary>GShare.pas:77 <c>g_sDataSaveDBServer: string;</c></summary>
    public static string g_sDataSaveDBServer = "";

    /// <summary>GShare.pas:78 <c>g_wDataSaveDBPort: Word = 3306;</c></summary>
    public static ushort g_wDataSaveDBPort = 3306;

    /// <summary>GShare.pas:79 <c>g_sDataSaveDBUser: string;</c></summary>
    public static string g_sDataSaveDBUser = "";

    /// <summary>GShare.pas:80 <c>g_sDataSaveDBPassword: string;</c></summary>
    public static string g_sDataSaveDBPassword = "";

    /// <summary>GShare.pas:81 <c>g_sDataSaveDataBase: string;</c></summary>
    public static string g_sDataSaveDataBase = "";

    // ---------------- IP 地址（GShare.pas:83-91） ----------------

    /// <summary>GShare.pas:83 <c>g_sAllIPaddr: string = '0.0.0.0';</c></summary>
    public static string g_sAllIPaddr = "0.0.0.0";

    /// <summary>GShare.pas:84 <c>g_sLocalIPaddr: string = '127.0.0.1';</c></summary>
    public static string g_sLocalIPaddr = "127.0.0.1";

    /// <summary>GShare.pas:85 <c>g_sLocalIPaddr1: string = '127.0.0.2';</c></summary>
    public static string g_sLocalIPaddr1 = "127.0.0.2";

    /// <summary>GShare.pas:87 <c>g_sExtIPaddr: string = '192.168.0.1';</c></summary>
    public static string g_sExtIPaddr = "192.168.0.1";

    /// <summary>GShare.pas:88 <c>g_sExtNetComIPaddr: string = '192.168.100.1';</c></summary>
    public static string g_sExtNetComIPaddr = "192.168.100.1";

    /// <summary>GShare.pas:89 <c>g_boDoubleLineMode: Boolean = False;</c>（双线一区模式）</summary>
    public static bool g_boDoubleLineMode = false;

    /// <summary>GShare.pas:90 <c>g_nLimitOnlineUser: Integer = 10000;</c>（服务器最高上线人数）</summary>
    public static int g_nLimitOnlineUser = 10000;

    /// <summary>GShare.pas:91 <c>g_boDynamicIPMode: Boolean = False;</c>（增加动态IP支持 piaoyun 2013-08-30）</summary>
    public static bool g_boDynamicIPMode = false;

    // ---------------- DBServer（GShare.pas:93-131） ----------------

    /// <summary>GShare.pas:93 <c>g_sDBServer_ProgramFile = 'DBServer.exe';</c></summary>
    public static string g_sDBServer_ProgramFile = "DBServer.exe";

    /// <summary>GShare.pas:94 <c>g_sDBServer_Directory = 'DBServer\';</c></summary>
    public static string g_sDBServer_Directory = @"DBServer\";

    /// <summary>GShare.pas:95 <c>g_boDBServer_GetStart: Boolean = True;</c></summary>
    public static bool g_boDBServer_GetStart = true;

    /// <summary>GShare.pas:96 <c>g_boDBServer_GetMinimize: Boolean = True;</c></summary>
    public static bool g_boDBServer_GetMinimize = true;

    /// <summary>GShare.pas:97 <c>g_sDBServer_ConfigFile = 'dbsrc.ini';</c></summary>
    public static string g_sDBServer_ConfigFile = "dbsrc.ini";

    /// <summary>GShare.pas:98 <c>g_sDBServer_Config_ServerAddr = '127.0.0.1';</c></summary>
    public static string g_sDBServer_Config_ServerAddr = "127.0.0.1";

    /// <summary>GShare.pas:99 <c>g_nDBServer_Config_ServerPort: Integer = 6000;</c></summary>
    public static int g_nDBServer_Config_ServerPort = 6000;

    /// <summary>GShare.pas:100 <c>g_sDBServer_Config_GateAddr = '127.0.0.1';</c></summary>
    public static string g_sDBServer_Config_GateAddr = "127.0.0.1";

    /// <summary>GShare.pas:101 <c>g_nDBServer_Config_GatePort: Integer = 5100;</c></summary>
    public static int g_nDBServer_Config_GatePort = 5100;

    /// <summary>GShare.pas:102 <c>g_sDBServer_Config_IDSAddr = '127.0.0.1';</c></summary>
    public static string g_sDBServer_Config_IDSAddr = "127.0.0.1";

    /// <summary>GShare.pas:103 <c>g_nDBServer_Config_IDSPort: Integer = 5600;</c></summary>
    public static int g_nDBServer_Config_IDSPort = 5600;

    /// <summary>GShare.pas:105 <c>g_nDBServer_Config_Interval: Integer = 1000;</c></summary>
    public static int g_nDBServer_Config_Interval = 1000;

    /// <summary>GShare.pas:106 <c>g_nDBServer_Config_Level1: Integer = 1;</c></summary>
    public static int g_nDBServer_Config_Level1 = 1;

    /// <summary>GShare.pas:107 <c>g_nDBServer_Config_Level2: Integer = 7;</c></summary>
    public static int g_nDBServer_Config_Level2 = 7;

    /// <summary>GShare.pas:108 <c>g_nDBServer_Config_Level3: Integer = 14;</c></summary>
    public static int g_nDBServer_Config_Level3 = 14;

    /// <summary>GShare.pas:109 <c>g_nDBServer_Config_Day1: Integer = 7;</c></summary>
    public static int g_nDBServer_Config_Day1 = 7;

    /// <summary>GShare.pas:110 <c>g_nDBServer_Config_Day2: Integer = 62;</c></summary>
    public static int g_nDBServer_Config_Day2 = 62;

    /// <summary>GShare.pas:111 <c>g_nDBServer_Config_Day3: Integer = 124;</c></summary>
    public static int g_nDBServer_Config_Day3 = 124;

    /// <summary>GShare.pas:112 <c>g_nDBServer_Config_Month1: Integer = 0;</c></summary>
    public static int g_nDBServer_Config_Month1 = 0;

    /// <summary>GShare.pas:113 <c>g_nDBServer_Config_Month2: Integer = 0;</c></summary>
    public static int g_nDBServer_Config_Month2 = 0;

    /// <summary>GShare.pas:114 <c>g_nDBServer_Config_Month3: Integer = 0;</c></summary>
    public static int g_nDBServer_Config_Month3 = 0;

    /// <summary>GShare.pas:116 <c>g_sDBServer_Config_Dir = 'FDB\';</c></summary>
    public static string g_sDBServer_Config_Dir = @"FDB\";

    /// <summary>GShare.pas:117 <c>g_sDBServer_Config_IdDir = 'FDB\';</c>（原文如此：与 Dir 同值）</summary>
    public static string g_sDBServer_Config_IdDir = @"FDB\";

    /// <summary>GShare.pas:118 <c>g_sDBServer_Config_HumDir = 'FDB\';</c></summary>
    public static string g_sDBServer_Config_HumDir = @"FDB\";

    /// <summary>GShare.pas:119 <c>g_sDBServer_Config_FeeDir = 'FDB\';</c></summary>
    public static string g_sDBServer_Config_FeeDir = @"FDB\";

    /// <summary>GShare.pas:120 <c>g_sDBServer_Config_BackupDir = 'Backup\';</c></summary>
    public static string g_sDBServer_Config_BackupDir = @"Backup\";

    /// <summary>GShare.pas:121 <c>g_sDBServer_Config_ConnectDir = 'Connection\';</c></summary>
    public static string g_sDBServer_Config_ConnectDir = @"Connection\";

    /// <summary>GShare.pas:122 <c>g_sDBServer_Config_LogDir = 'Log\';</c></summary>
    public static string g_sDBServer_Config_LogDir = @"Log\";

    /// <summary>GShare.pas:124 <c>g_sDBServer_Config_MapFile = 'Mir200\Envir\MapInfo.txt';</c></summary>
    public static string g_sDBServer_Config_MapFile = @"Mir200\Envir\MapInfo.txt";

    /// <summary>GShare.pas:125 <c>g_boDBServer_Config_ViewHackMsg: Boolean = False;</c></summary>
    public static bool g_boDBServer_Config_ViewHackMsg = false;

    /// <summary>GShare.pas:126 <c>g_sDBServer_AddrTableFile = '!addrtable.txt';</c></summary>
    public static string g_sDBServer_AddrTableFile = "!addrtable.txt";

    /// <summary>GShare.pas:127 <c>g_sDBServer_GateListFile = '!GateList.ini';</c></summary>
    public static string g_sDBServer_GateListFile = "!GateList.ini";

    /// <summary>GShare.pas:128 <c>g_sDBServer_ServerinfoFile = '!serverinfo.txt';</c></summary>
    public static string g_sDBServer_ServerinfoFile = "!serverinfo.txt";

    /// <summary>GShare.pas:129 <c>g_nDBServer_MainFormX: Integer = 0;</c></summary>
    public static int g_nDBServer_MainFormX = 0;

    /// <summary>GShare.pas:130 <c>g_nDBServer_MainFormY: Integer = 326;</c></summary>
    public static int g_nDBServer_MainFormY = 326;

    /// <summary>GShare.pas:131 <c>g_boDBServer_DisableAutoGame: Boolean = False;</c></summary>
    public static bool g_boDBServer_DisableAutoGame = false;

    // ---------------- LoginServer（GShare.pas:133-164） ----------------

    /// <summary>GShare.pas:133 <c>g_sLoginServer_ProgramFile = 'LoginSrv.exe';</c></summary>
    public static string g_sLoginServer_ProgramFile = "LoginSrv.exe";

    /// <summary>GShare.pas:134 <c>g_sLoginServer_Directory = 'LoginSrv\';</c></summary>
    public static string g_sLoginServer_Directory = @"LoginSrv\";

    /// <summary>GShare.pas:135 <c>g_sLoginServer_ConfigFile = 'Logsrv.ini';</c></summary>
    public static string g_sLoginServer_ConfigFile = "Logsrv.ini";

    /// <summary>GShare.pas:136 <c>g_boLoginServer_GetStart: Boolean = True;</c></summary>
    public static bool g_boLoginServer_GetStart = true;

    /// <summary>GShare.pas:137 <c>g_boLoginServer_GetMinimize: Boolean = True;</c></summary>
    public static bool g_boLoginServer_GetMinimize = true;

    /// <summary>GShare.pas:138 <c>g_sLoginServer_GateAddr = '127.0.0.1';</c></summary>
    public static string g_sLoginServer_GateAddr = "127.0.0.1";

    /// <summary>GShare.pas:139 <c>g_nLoginServer_GatePort: Integer = 5500;</c></summary>
    public static int g_nLoginServer_GatePort = 5500;

    /// <summary>GShare.pas:140 <c>g_sLoginServer_ServerAddr = '127.0.0.1';</c></summary>
    public static string g_sLoginServer_ServerAddr = "127.0.0.1";

    /// <summary>GShare.pas:141 <c>g_nLoginServer_ServerPort: Integer = 5600;</c></summary>
    public static int g_nLoginServer_ServerPort = 5600;

    /// <summary>GShare.pas:142 <c>g_nLoginServer_ControlPort: Integer = 0;</c></summary>
    public static int g_nLoginServer_ControlPort = 0;

    /// <summary>GShare.pas:144 <c>g_sLoginServer_ReadyServers: Integer = 0;</c>（原文如此：名字带 _s 实为 Integer）</summary>
    public static int g_sLoginServer_ReadyServers = 0;

    /// <summary>GShare.pas:145 <c>g_sLoginServer_EnableMakingID: Boolean = True;</c>（原文如此：名字带 _s 实为 Boolean）</summary>
    public static bool g_sLoginServer_EnableMakingID = true;

    /// <summary>GShare.pas:146 <c>g_sLoginServer_EnableTrial: Boolean = False;</c></summary>
    public static bool g_sLoginServer_EnableTrial = false;

    /// <summary>GShare.pas:147 <c>g_sLoginServer_TestServer: Boolean = True;</c></summary>
    public static bool g_sLoginServer_TestServer = true;

    /// <summary>GShare.pas:149 <c>g_sLoginServer_IdDir = 'IDDB\';</c></summary>
    public static string g_sLoginServer_IdDir = @"IDDB\";

    /// <summary>GShare.pas:150 <c>g_sLoginServer_FeedIDList = 'FeedIDList.txt';</c></summary>
    public static string g_sLoginServer_FeedIDList = "FeedIDList.txt";

    /// <summary>GShare.pas:151 <c>g_sLoginServer_FeedIPList = 'FeedIPList.txt';</c></summary>
    public static string g_sLoginServer_FeedIPList = "FeedIPList.txt";

    /// <summary>GShare.pas:152 <c>g_sLoginServer_CountLogDir = 'CountLog\';</c></summary>
    public static string g_sLoginServer_CountLogDir = @"CountLog\";

    /// <summary>GShare.pas:153 <c>g_sLoginServer_WebLogDir = 'GameWFolder\';</c></summary>
    public static string g_sLoginServer_WebLogDir = @"GameWFolder\";

    /// <summary>GShare.pas:154 <c>g_sLoginServer_ChrLogDir = 'ChrLog\';</c></summary>
    public static string g_sLoginServer_ChrLogDir = @"ChrLog\";

    /// <summary>GShare.pas:155 <c>g_sLoginServer_IDLogDir = 'IDLogDir\';</c></summary>
    public static string g_sLoginServer_IDLogDir = @"IDLogDir\";

    /// <summary>GShare.pas:157 <c>g_sLoginServer_AddrTableFile = '!addrtable.txt';</c></summary>
    public static string g_sLoginServer_AddrTableFile = "!addrtable.txt";

    /// <summary>GShare.pas:158 <c>g_sLoginServer_ServeraddrFile = '!serveraddr.txt';</c></summary>
    public static string g_sLoginServer_ServeraddrFile = "!serveraddr.txt";

    /// <summary>GShare.pas:159 <c>g_sLoginServerUserLimitFile = '!UserLimit.txt';</c>（原文如此：无下划线分隔）</summary>
    public static string g_sLoginServerUserLimitFile = "!UserLimit.txt";

    /// <summary>GShare.pas:160 <c>g_sLoginServerFeedIDListFile = 'FeedIDList.txt';</c></summary>
    public static string g_sLoginServerFeedIDListFile = "FeedIDList.txt";

    /// <summary>GShare.pas:161 <c>g_sLoginServerFeedIPListFile = 'FeedIPList.txt';</c></summary>
    public static string g_sLoginServerFeedIPListFile = "FeedIPList.txt";

    /// <summary>GShare.pas:162 <c>g_nLoginServer_MainFormX: Integer = 251;</c></summary>
    public static int g_nLoginServer_MainFormX = 251;

    /// <summary>GShare.pas:163 <c>g_nLoginServer_MainFormY: Integer = 0;</c></summary>
    public static int g_nLoginServer_MainFormY = 0;

    /// <summary>GShare.pas:164 <c>g_nLoginServer_RouteList: TList;</c></summary>
    public static List<object> g_nLoginServer_RouteList = new();

    // ---------------- LogServer（GShare.pas:166-175） ----------------

    /// <summary>GShare.pas:166 <c>g_sLogServer_ProgramFile = 'LogDataServer.exe';</c></summary>
    public static string g_sLogServer_ProgramFile = "LogDataServer.exe";

    /// <summary>GShare.pas:167 <c>g_sLogServer_Directory = 'LogServer\';</c></summary>
    public static string g_sLogServer_Directory = @"LogServer\";

    /// <summary>GShare.pas:168 <c>g_boLogServer_GetStart: Boolean = True;</c></summary>
    public static bool g_boLogServer_GetStart = true;

    /// <summary>GShare.pas:169 <c>g_boLogServer_GetMinimize: Boolean = True;</c></summary>
    public static bool g_boLogServer_GetMinimize = true;

    /// <summary>GShare.pas:170 <c>g_sLogServer_ConfigFile = 'LogData.ini';</c></summary>
    public static string g_sLogServer_ConfigFile = "LogData.ini";

    /// <summary>GShare.pas:171 <c>g_sLogServer_BaseDir = 'BaseDir\';</c></summary>
    public static string g_sLogServer_BaseDir = @"BaseDir\";

    /// <summary>GShare.pas:172 <c>g_sLogServer_ServerAddr = '127.0.0.1';</c></summary>
    public static string g_sLogServer_ServerAddr = "127.0.0.1";

    /// <summary>GShare.pas:173 <c>g_nLogServer_Port: Integer = 10000;</c></summary>
    public static int g_nLogServer_Port = 10000;

    /// <summary>GShare.pas:174 <c>g_nLogServer_MainFormX: Integer = 251;</c></summary>
    public static int g_nLogServer_MainFormX = 251;

    /// <summary>GShare.pas:175 <c>g_nLogServer_MainFormY: Integer = 239;</c></summary>
    public static int g_nLogServer_MainFormY = 239;

    // ---------------- M2Server（GShare.pas:177-224） ----------------

    /// <summary>GShare.pas:177 <c>g_sM2Server_ProgramFile = 'M2Server.exe';</c></summary>
    public static string g_sM2Server_ProgramFile = "M2Server.exe";

    /// <summary>GShare.pas:178 <c>g_sM2Server_Directory = 'Mir200\';</c></summary>
    public static string g_sM2Server_Directory = @"Mir200\";

    /// <summary>GShare.pas:179 <c>g_boM2Server_GetStart: Boolean = True;</c></summary>
    public static bool g_boM2Server_GetStart = true;

    /// <summary>GShare.pas:180 <c>g_boM2Server_GetMinimize: Boolean = True;</c></summary>
    public static bool g_boM2Server_GetMinimize = true;

    /// <summary>GShare.pas:181 <c>g_sM2Server_ConfigFile = '!setup.txt';</c></summary>
    public static string g_sM2Server_ConfigFile = "!setup.txt";

    /// <summary>GShare.pas:182 <c>g_sM2Server_AbuseFile = '!abuse.txt';</c></summary>
    public static string g_sM2Server_AbuseFile = "!abuse.txt";

    /// <summary>GShare.pas:183 <c>g_sM2Server_RunAddrFile = '!runaddr.txt';</c></summary>
    public static string g_sM2Server_RunAddrFile = "!runaddr.txt";

    /// <summary>GShare.pas:184 <c>g_sM2Server_ServerTableFile = '!servertable.txt';</c></summary>
    public static string g_sM2Server_ServerTableFile = "!servertable.txt";

    /// <summary>GShare.pas:186 <c>g_nM2Server_ServerNumber: Integer = 0;</c></summary>
    public static int g_nM2Server_ServerNumber = 0;

    /// <summary>GShare.pas:187 <c>g_nM2Server_ServerIndex: Integer = 0;</c></summary>
    public static int g_nM2Server_ServerIndex = 0;

    /// <summary>GShare.pas:188 <c>g_nM2Server_EditionId: Integer = 0;</c>（版本ID）</summary>
    public static int g_nM2Server_EditionId = 0;

    /// <summary>GShare.pas:189 <c>g_nM2Server_AreaId: Integer = 0;</c>（区服ID）</summary>
    public static int g_nM2Server_AreaId = 0;

    /// <summary>GShare.pas:190 <c>g_boM2Server_VentureServer: Boolean = False;</c></summary>
    public static bool g_boM2Server_VentureServer = false;

    /// <summary>GShare.pas:191 <c>g_boM2Server_TestServer: Boolean = True;</c></summary>
    public static bool g_boM2Server_TestServer = true;

    /// <summary>GShare.pas:192 <c>g_nM2Server_TestLevel: Integer = 1;</c></summary>
    public static int g_nM2Server_TestLevel = 1;

    /// <summary>GShare.pas:193 <c>g_nM2Server_TestGold: Integer = 0;</c></summary>
    public static int g_nM2Server_TestGold = 0;

    /// <summary>GShare.pas:194 <c>g_boM2Server_ServiceMode: Boolean = False;</c></summary>
    public static bool g_boM2Server_ServiceMode = false;

    /// <summary>GShare.pas:195 <c>g_boM2Server_NonPKServer: Boolean = False;</c></summary>
    public static bool g_boM2Server_NonPKServer = false;

    /// <summary>GShare.pas:196 <c>g_sM2Server_MsgSrvAddr = '127.0.0.1';</c></summary>
    public static string g_sM2Server_MsgSrvAddr = "127.0.0.1";

    /// <summary>GShare.pas:197 <c>g_nM2Server_MsgSrvPort: Integer = 4900;</c></summary>
    public static int g_nM2Server_MsgSrvPort = 4900;

    /// <summary>GShare.pas:198 <c>g_sM2Server_GateAddr = '127.0.0.1';</c></summary>
    public static string g_sM2Server_GateAddr = "127.0.0.1";

    /// <summary>GShare.pas:199 <c>g_nM2Server_GatePort: Integer = 5000;</c></summary>
    public static int g_nM2Server_GatePort = 5000;

    /// <summary>GShare.pas:201 <c>g_sM2Server_BaseDir = 'Share\';</c></summary>
    public static string g_sM2Server_BaseDir = @"Share\";

    /// <summary>GShare.pas:202 <c>g_sM2Server_GuildDir = 'GuildBase\Guilds\';</c></summary>
    public static string g_sM2Server_GuildDir = @"GuildBase\Guilds\";

    /// <summary>GShare.pas:203 <c>g_sM2Server_GuildFile = 'GuildBase\Guildlist.txt';</c></summary>
    public static string g_sM2Server_GuildFile = @"GuildBase\Guildlist.txt";

    /// <summary>GShare.pas:204 <c>g_sM2Server_VentureDir = 'ShareV\';</c></summary>
    public static string g_sM2Server_VentureDir = @"ShareV\";

    /// <summary>GShare.pas:205 <c>g_sM2Server_ConLogDir = 'ConLog\';</c></summary>
    public static string g_sM2Server_ConLogDir = @"ConLog\";

    /// <summary>GShare.pas:206 <c>g_sM2Server_LogDir = 'Log\';</c></summary>
    public static string g_sM2Server_LogDir = @"Log\";

    /// <summary>GShare.pas:207 <c>g_sM2Server_CastleDir = 'Castle\';</c></summary>
    public static string g_sM2Server_CastleDir = @"Castle\";

    /// <summary>GShare.pas:208 <c>g_sM2Server_EnvirDir = 'Envir\';</c></summary>
    public static string g_sM2Server_EnvirDir = @"Envir\";

    /// <summary>GShare.pas:209 <c>g_sM2Server_MapDir = 'Map\';</c></summary>
    public static string g_sM2Server_MapDir = @"Map\";

    /// <summary>GShare.pas:210 <c>g_sM2Server_NoticeDir = 'Notice\';</c></summary>
    public static string g_sM2Server_NoticeDir = @"Notice\";

    /// <summary>GShare.pas:212 <c>g_sM2Server_BoxsDir = 'Envir\Boxs\';</c>（宝箱目录 piaoyun 2013-08-22）</summary>
    public static string g_sM2Server_BoxsDir = @"Envir\Boxs\";

    /// <summary>GShare.pas:214 <c>g_nM2Server_MainFormX: Integer = 560;</c></summary>
    public static int g_nM2Server_MainFormX = 560;

    /// <summary>GShare.pas:215 <c>g_nM2Server_MainFormY: Integer = 0;</c></summary>
    public static int g_nM2Server_MainFormY = 0;

    /// <summary>GShare.pas:216 <c>g_sM2Server_CastleFile = 'Castle\List.txt';</c></summary>
    public static string g_sM2Server_CastleFile = @"Castle\List.txt";

    // ---------------- LoginGate（GShare.pas:218-241） ----------------

    /// <summary>GShare.pas:218 <c>g_sLoginGate_ProgramFile = 'LoginGate.exe';</c></summary>
    public static string g_sLoginGate_ProgramFile = "LoginGate.exe";

    /// <summary>GShare.pas:219 <c>g_sLoginGate_Directory = 'LoginGate\';</c></summary>
    public static string g_sLoginGate_Directory = @"LoginGate\";

    /// <summary>GShare.pas:220 <c>g_boLoginGate_GetStart: Boolean = True;</c></summary>
    public static bool g_boLoginGate_GetStart = true;

    /// <summary>GShare.pas:221 <c>g_boLoginGate_GetMinimize: Boolean = True;</c></summary>
    public static bool g_boLoginGate_GetMinimize = true;

    /// <summary>GShare.pas:222 <c>g_sLoginGate_ConfigFile = 'Config.ini';</c></summary>
    public static string g_sLoginGate_ConfigFile = "Config.ini";

    /// <summary>GShare.pas:224 <c>g_sLoginGate_ServerAddr = '127.0.0.1';</c></summary>
    public static string g_sLoginGate_ServerAddr = "127.0.0.1";

    /// <summary>GShare.pas:225 <c>g_sLoginGate_ServerAddr1 = '127.0.0.2';</c></summary>
    public static string g_sLoginGate_ServerAddr1 = "127.0.0.2";

    /// <summary>GShare.pas:226 <c>g_nLoginGate_ServerPort: Integer = 5500;</c></summary>
    public static int g_nLoginGate_ServerPort = 5500;

    /// <summary>GShare.pas:228 <c>g_sLoginGate_GateAddr = '0.0.0.0';</c></summary>
    public static string g_sLoginGate_GateAddr = "0.0.0.0";

    /// <summary>GShare.pas:229 <c>g_nLoginGate_GatePort: Integer = 7000;</c></summary>
    public static int g_nLoginGate_GatePort = 7000;

    /// <summary>GShare.pas:231 <c>g_nLoginGate_ShowLogLevel: Integer = 3;</c></summary>
    public static int g_nLoginGate_ShowLogLevel = 3;

    /// <summary>GShare.pas:232 <c>g_nLoginGate_MaxConnOfIPaddr: Integer = 20;</c></summary>
    public static int g_nLoginGate_MaxConnOfIPaddr = 20;

    /// <summary>GShare.pas:233 <c>g_nLoginGate_BlockMethod: Integer = 0;</c></summary>
    public static int g_nLoginGate_BlockMethod = 0;

    /// <summary>GShare.pas:234 <c>g_nLoginGate_KeepConnectTimeOut: Integer = 60000;</c></summary>
    public static int g_nLoginGate_KeepConnectTimeOut = 60000;

    /// <summary>GShare.pas:235 <c>g_nLoginGate_MainFormX: Integer = 0;</c></summary>
    public static int g_nLoginGate_MainFormX = 0;

    /// <summary>GShare.pas:236 <c>g_nLoginGate_MainFormY: Integer = 0;</c></summary>
    public static int g_nLoginGate_MainFormY = 0;

    // ---------------- SelGate（GShare.pas:238-267） ----------------

    /// <summary>GShare.pas:238 <c>g_sSelGate_ProgramFile = 'SelGate.exe';</c></summary>
    public static string g_sSelGate_ProgramFile = "SelGate.exe";

    /// <summary>GShare.pas:239 <c>g_sSelGate_Directory = 'SelGate\';</c></summary>
    public static string g_sSelGate_Directory = @"SelGate\";

    /// <summary>GShare.pas:240 <c>g_boSelGate_GetStart: Boolean = True;</c></summary>
    public static bool g_boSelGate_GetStart = true;

    /// <summary>GShare.pas:241 <c>g_boSelGate_GetStart1: Boolean = False;</c></summary>
    public static bool g_boSelGate_GetStart1 = false;

    /// <summary>GShare.pas:242 <c>g_boSelGate_GetMinimize: Boolean = True;</c></summary>
    public static bool g_boSelGate_GetMinimize = true;

    /// <summary>GShare.pas:243 <c>g_sSelGate_ConfigFile = 'Config.ini';</c></summary>
    public static string g_sSelGate_ConfigFile = "Config.ini";

    /// <summary>GShare.pas:245 <c>g_sSelGate_ServerAddr = '127.0.0.1';</c></summary>
    public static string g_sSelGate_ServerAddr = "127.0.0.1";

    /// <summary>GShare.pas:246 <c>g_nSelGate_ServerPort: Integer = 5100;</c></summary>
    public static int g_nSelGate_ServerPort = 5100;

    /// <summary>GShare.pas:247 <c>g_sSelGate_GateAddr = '0.0.0.0';</c></summary>
    public static string g_sSelGate_GateAddr = "0.0.0.0";

    /// <summary>GShare.pas:248 <c>g_nSeLGate_GatePort: Integer = 7100;</c>（原文如此：nSeL 大小写）</summary>
    public static int g_nSeLGate_GatePort = 7100;

    /// <summary>GShare.pas:249 <c>g_sSelGate_GateAddr1 = '0.0.0.0';</c></summary>
    public static string g_sSelGate_GateAddr1 = "0.0.0.0";

    /// <summary>GShare.pas:250 <c>g_nSeLGate_GatePort1: Integer = 7101;</c>（原文如此：nSeL 大小写）</summary>
    public static int g_nSeLGate_GatePort1 = 7101;

    /// <summary>GShare.pas:253 <c>g_nSelGate_ShowLogLevel: Integer = 3;</c></summary>
    public static int g_nSelGate_ShowLogLevel = 3;

    /// <summary>GShare.pas:254 <c>g_nSelGate_MaxConnOfIPaddr: Integer = 20;</c></summary>
    public static int g_nSelGate_MaxConnOfIPaddr = 20;

    /// <summary>GShare.pas:255 <c>g_nSelGate_BlockMethod: Integer = 0;</c></summary>
    public static int g_nSelGate_BlockMethod = 0;

    /// <summary>GShare.pas:256 <c>g_nSelGate_KeepConnectTimeOut: Integer = 60000;</c></summary>
    public static int g_nSelGate_KeepConnectTimeOut = 60000;

    /// <summary>GShare.pas:257 <c>g_nSelGate_MainFormX: Integer = 0;</c></summary>
    public static int g_nSelGate_MainFormX = 0;

    /// <summary>GShare.pas:258 <c>g_nSelGate_MainFormY: Integer = 163;</c></summary>
    public static int g_nSelGate_MainFormY = 163;

    /// <summary>GShare.pas:259 <c>g_boSelGate_GetMultiThread: Boolean = True;</c>（2019-09-28 15:45:08）</summary>
    public static bool g_boSelGate_GetMultiThread = true;

    // ---------------- RunGate（GShare.pas:261-302） ----------------

    /// <summary>GShare.pas:261 <c>g_sRunGate_ProgramFile = 'RunGate.exe';</c></summary>
    public static string g_sRunGate_ProgramFile = "RunGate.exe";

    /// <summary>GShare.pas:262 <c>g_sRunGate_RegKey = 'ABCDEFGHIJKL';</c></summary>
    public static string g_sRunGate_RegKey = "ABCDEFGHIJKL";

    /// <summary>GShare.pas:264 <c>g_sRunGate_Directory = 'RunGate\';</c></summary>
    public static string g_sRunGate_Directory = @"RunGate\";

    /// <summary>GShare.pas:265 <c>g_sRunGate_DirectoryEx = 'RunGate%d\';</c></summary>
    public static string g_sRunGate_DirectoryEx = @"RunGate%d\";

    /// <summary>GShare.pas:266 <c>g_sRunGate_AddrTableFile = '!addrtable.txt';</c></summary>
    public static string g_sRunGate_AddrTableFile = "!addrtable.txt";

    /// <summary>GShare.pas:268 <c>g_boRunGate1_GetStart: Boolean = True;</c></summary>
    public static bool g_boRunGate1_GetStart = true;

    /// <summary>GShare.pas:269 <c>g_boRunGate2_GetStart: Boolean = True;</c></summary>
    public static bool g_boRunGate2_GetStart = true;

    /// <summary>GShare.pas:270 <c>g_boRunGate3_GetStart: Boolean = True;</c></summary>
    public static bool g_boRunGate3_GetStart = true;

    /// <summary>GShare.pas:271 <c>g_boRunGate4_GetStart: Boolean = True;</c></summary>
    public static bool g_boRunGate4_GetStart = true;

    /// <summary>GShare.pas:272 <c>g_boRunGate5_GetStart: Boolean = True;</c></summary>
    public static bool g_boRunGate5_GetStart = true;

    /// <summary>GShare.pas:273 <c>g_boRunGate6_GetStart: Boolean = True;</c></summary>
    public static bool g_boRunGate6_GetStart = true;

    /// <summary>GShare.pas:274 <c>g_boRunGate7_GetStart: Boolean = True;</c></summary>
    public static bool g_boRunGate7_GetStart = true;

    /// <summary>GShare.pas:276 <c>g_boRunGate_GetStart: Boolean = True;</c></summary>
    public static bool g_boRunGate_GetStart = true;

    /// <summary>GShare.pas:277 <c>g_boRunGate_GetMinimize: Boolean = True;</c></summary>
    public static bool g_boRunGate_GetMinimize = true;

    /// <summary>GShare.pas:278 <c>g_boRunGate_GetMultiThread: Boolean = False;</c></summary>
    public static bool g_boRunGate_GetMultiThread = false;

    /// <summary>GShare.pas:280 <c>g_sRunGate_ConfigFile = 'Config.ini';</c>（RunGate.ini）</summary>
    public static string g_sRunGate_ConfigFile = "Config.ini";

    /// <summary>GShare.pas:281 <c>g_nRunGate_Count: Integer = 3;</c></summary>
    public static int g_nRunGate_Count = 3;

    /// <summary>GShare.pas:282 <c>g_sRunGate_ServerAddr = '127.0.0.1';</c></summary>
    public static string g_sRunGate_ServerAddr = "127.0.0.1";

    /// <summary>GShare.pas:283 <c>g_nRunGate_ServerPort: Integer = 5000;</c></summary>
    public static int g_nRunGate_ServerPort = 5000;

    /// <summary>GShare.pas:284 <c>g_sRunGate_GateAddr = '0.0.0.0';</c></summary>
    public static string g_sRunGate_GateAddr = "0.0.0.0";

    /// <summary>GShare.pas:285 <c>g_nRunGate_GatePort: Integer = 7200;</c></summary>
    public static int g_nRunGate_GatePort = 7200;

    /// <summary>GShare.pas:286 <c>g_sRunGate1_GateAddr = '0.0.0.0';</c></summary>
    public static string g_sRunGate1_GateAddr = "0.0.0.0";

    /// <summary>GShare.pas:287 <c>g_nRunGate1_GatePort: Integer = 7300;</c></summary>
    public static int g_nRunGate1_GatePort = 7300;

    /// <summary>GShare.pas:288 <c>g_sRunGate2_GateAddr = '0.0.0.0';</c></summary>
    public static string g_sRunGate2_GateAddr = "0.0.0.0";

    /// <summary>GShare.pas:289 <c>g_nRunGate2_GatePort: Integer = 7400;</c></summary>
    public static int g_nRunGate2_GatePort = 7400;

    /// <summary>GShare.pas:290 <c>g_sRunGate3_GateAddr = '0.0.0.0';</c></summary>
    public static string g_sRunGate3_GateAddr = "0.0.0.0";

    /// <summary>GShare.pas:291 <c>g_nRunGate3_GatePort: Integer = 7500;</c></summary>
    public static int g_nRunGate3_GatePort = 7500;

    /// <summary>GShare.pas:292 <c>g_sRunGate4_GateAddr = '0.0.0.0';</c></summary>
    public static string g_sRunGate4_GateAddr = "0.0.0.0";

    /// <summary>GShare.pas:293 <c>g_nRunGate4_GatePort: Integer = 7600;</c></summary>
    public static int g_nRunGate4_GatePort = 7600;

    /// <summary>GShare.pas:294 <c>g_sRunGate5_GateAddr = '0.0.0.0';</c></summary>
    public static string g_sRunGate5_GateAddr = "0.0.0.0";

    /// <summary>GShare.pas:295 <c>g_nRunGate5_GatePort: Integer = 7700;</c></summary>
    public static int g_nRunGate5_GatePort = 7700;

    /// <summary>GShare.pas:296 <c>g_sRunGate6_GateAddr = '0.0.0.0';</c></summary>
    public static string g_sRunGate6_GateAddr = "0.0.0.0";

    /// <summary>GShare.pas:297 <c>g_nRunGate6_GatePort: Integer = 7800;</c></summary>
    public static int g_nRunGate6_GatePort = 7800;

    /// <summary>GShare.pas:298 <c>g_sRunGate7_GateAddr = '0.0.0.0';</c></summary>
    public static string g_sRunGate7_GateAddr = "0.0.0.0";

    /// <summary>GShare.pas:299 <c>g_nRunGate7_GatePort: Integer = 7900;</c></summary>
    public static int g_nRunGate7_GatePort = 7900;

    /// <summary>GShare.pas:301 <c>g_nRunGate0_DBPort: Integer = 27201;</c></summary>
    public static int g_nRunGate0_DBPort = 27201;

    /// <summary>GShare.pas:302 <c>g_nRunGate1_DBPort: Integer = 27301;</c></summary>
    public static int g_nRunGate1_DBPort = 27301;

    /// <summary>GShare.pas:303 <c>g_nRunGate2_DBPort: Integer = 27401;</c></summary>
    public static int g_nRunGate2_DBPort = 27401;

    /// <summary>GShare.pas:304 <c>g_nRunGate3_DBPort: Integer = 27501;</c></summary>
    public static int g_nRunGate3_DBPort = 27501;

    /// <summary>GShare.pas:305 <c>g_nRunGate4_DBPort: Integer = 27601;</c></summary>
    public static int g_nRunGate4_DBPort = 27601;

    /// <summary>GShare.pas:306 <c>g_nRunGate5_DBPort: Integer = 27701;</c></summary>
    public static int g_nRunGate5_DBPort = 27701;

    /// <summary>GShare.pas:307 <c>g_nRunGate6_DBPort: Integer = 27801;</c></summary>
    public static int g_nRunGate6_DBPort = 27801;

    /// <summary>GShare.pas:308 <c>g_nRunGate7_DBPort: Integer = 27901;</c></summary>
    public static int g_nRunGate7_DBPort = 27901;

    /// <summary>GShare.pas:310 <c>g_nRunGateDBPort_MulThread: Integer = 27201;</c></summary>
    public static int g_nRunGateDBPort_MulThread = 27201;

    // ---------------- 运行网关信息数组（GShare.pas:312） ----------------

    /// <summary>GShare.pas:312 <c>g_RunGateInfo: array[0..MAXRUNGATECOUNT - 1] of TRunGateInfo;</c>（2006-11-12 增加）</summary>
    public static TRunGateInfo[] g_RunGateInfo = new TRunGateInfo[GShareConst.MAXRUNGATECOUNT];

    // ---------------- 受管程序记录（GShare.pas:314-322） ----------------

    /// <summary>GShare.pas:314 <c>DBServer: TProgram;</c></summary>
    public static TProgram DBServer;

    /// <summary>GShare.pas:315 <c>LoginServer: TProgram;</c></summary>
    public static TProgram LoginServer;

    /// <summary>GShare.pas:316 <c>LogServer: TProgram;</c></summary>
    public static TProgram LogServer;

    /// <summary>GShare.pas:317 <c>M2Server: TProgram;</c></summary>
    public static TProgram M2Server;

    /// <summary>GShare.pas:318 <c>RunGate: array[0..MAXRUNGATECOUNT - 1] of TProgram;</c></summary>
    public static TProgram[] RunGate = new TProgram[GShareConst.MAXRUNGATECOUNT];

    /// <summary>GShare.pas:319 <c>SelGate: TProgram;</c></summary>
    public static TProgram SelGate;

    /// <summary>GShare.pas:320 <c>SelGate1: TProgram;</c></summary>
    public static TProgram SelGate1;

    /// <summary>GShare.pas:321 <c>LoginGate: TProgram;</c></summary>
    public static TProgram LoginGate;

    /// <summary>GShare.pas:322 <c>LoginGate1: TProgram;</c></summary>
    public static TProgram LoginGate1;

    // ---------------- 停止/启动计时（GShare.pas:324-326） ----------------

    /// <summary>GShare.pas:324 <c>g_dwStopTick: LongWord;</c></summary>
    public static uint g_dwStopTick = 0;

    /// <summary>GShare.pas:325 <c>g_dwStopTimeOut: LongWord = 10000;</c></summary>
    public static uint g_dwStopTimeOut = 10000;

    /// <summary>GShare.pas:326 <c>g_boEmbeddedWindow: Boolean = False;</c></summary>
    public static bool g_boEmbeddedWindow = false;

    // ---------------- 自动启动（GShare.pas:328-330，原文如此：命名 n/s/bo 前缀混用） ----------------

    /// <summary>GShare.pas:328 <c>g_nAutoStartDelayTime: Integer = 60;</c></summary>
    public static int g_nAutoStartDelayTime = 60;

    /// <summary>GShare.pas:329 <c>g_boAutoStartServer: Boolean = False;</c></summary>
    public static bool g_boAutoStartServer = false;

    /// <summary>GShare.pas:330 <c>g_nAutoStartTimeCount: Integer = 0;</c></summary>
    public static int g_nAutoStartTimeCount = 0;

    // ---------------- 测试隔离 ----------------

    /// <summary>
    /// 单测用：把全部全局状态复位为 GShare.pas 声明的初始值。
    /// （Delphi 进程每次启动都会执行 unit initialization，测试进程需显式模拟。）
    /// </summary>
    public static void ResetForTests()
    {
        g_BackUpManager = null;
        g_boHeroDBOK = false;
        g_sGameName = "";
        g_nFormIdx = 0;
        g_IniConf = null;

        g_sButtonStartGame = "启动游戏服务器(&S)";
        g_sButtonStopGame = "停止游戏服务器(&T)";
        g_sButtonStopStartGame = "中止启动游戏服务器(&T)";
        g_sButtonStopStopGame = "中止停止游戏服务器(&T)";

        g_sConfFile = @".\Config.ini";
        g_sGameDirectory = @"D:\MirServer\";
        g_sOldGameDirectory = "";

        g_boUseSqliteDB = false;
        g_sHeroDBName = "HeroDB";
        g_sSqliteDBName = @"D:\MirServer\Mud2\DB\BmM2.db";

        g_nDataSaveDBType = 0;
        g_sDataSaveDBServer = "";
        g_wDataSaveDBPort = 3306;
        g_sDataSaveDBUser = "";
        g_sDataSaveDBPassword = "";
        g_sDataSaveDataBase = "";

        g_sAllIPaddr = "0.0.0.0";
        g_sLocalIPaddr = "127.0.0.1";
        g_sLocalIPaddr1 = "127.0.0.2";
        g_sExtIPaddr = "192.168.0.1";
        g_sExtNetComIPaddr = "192.168.100.1";
        g_boDoubleLineMode = false;
        g_nLimitOnlineUser = 10000;
        g_boDynamicIPMode = false;

        g_sDBServer_ProgramFile = "DBServer.exe";
        g_sDBServer_Directory = @"DBServer\";
        g_boDBServer_GetStart = true;
        g_boDBServer_GetMinimize = true;
        g_sDBServer_ConfigFile = "dbsrc.ini";
        g_sDBServer_Config_ServerAddr = "127.0.0.1";
        g_nDBServer_Config_ServerPort = 6000;
        g_sDBServer_Config_GateAddr = "127.0.0.1";
        g_nDBServer_Config_GatePort = 5100;
        g_sDBServer_Config_IDSAddr = "127.0.0.1";
        g_nDBServer_Config_IDSPort = 5600;
        g_nDBServer_Config_Interval = 1000;
        g_nDBServer_Config_Level1 = 1;
        g_nDBServer_Config_Level2 = 7;
        g_nDBServer_Config_Level3 = 14;
        g_nDBServer_Config_Day1 = 7;
        g_nDBServer_Config_Day2 = 62;
        g_nDBServer_Config_Day3 = 124;
        g_nDBServer_Config_Month1 = 0;
        g_nDBServer_Config_Month2 = 0;
        g_nDBServer_Config_Month3 = 0;
        g_sDBServer_Config_Dir = @"FDB\";
        g_sDBServer_Config_IdDir = @"FDB\";
        g_sDBServer_Config_HumDir = @"FDB\";
        g_sDBServer_Config_FeeDir = @"FDB\";
        g_sDBServer_Config_BackupDir = @"Backup\";
        g_sDBServer_Config_ConnectDir = @"Connection\";
        g_sDBServer_Config_LogDir = @"Log\";
        g_sDBServer_Config_MapFile = @"Mir200\Envir\MapInfo.txt";
        g_boDBServer_Config_ViewHackMsg = false;
        g_sDBServer_AddrTableFile = "!addrtable.txt";
        g_sDBServer_GateListFile = "!GateList.ini";
        g_sDBServer_ServerinfoFile = "!serverinfo.txt";
        g_nDBServer_MainFormX = 0;
        g_nDBServer_MainFormY = 326;
        g_boDBServer_DisableAutoGame = false;

        g_sLoginServer_ProgramFile = "LoginSrv.exe";
        g_sLoginServer_Directory = @"LoginSrv\";
        g_sLoginServer_ConfigFile = "Logsrv.ini";
        g_boLoginServer_GetStart = true;
        g_boLoginServer_GetMinimize = true;
        g_sLoginServer_GateAddr = "127.0.0.1";
        g_nLoginServer_GatePort = 5500;
        g_sLoginServer_ServerAddr = "127.0.0.1";
        g_nLoginServer_ServerPort = 5600;
        g_nLoginServer_ControlPort = 0;
        g_sLoginServer_ReadyServers = 0;
        g_sLoginServer_EnableMakingID = true;
        g_sLoginServer_EnableTrial = false;
        g_sLoginServer_TestServer = true;
        g_sLoginServer_IdDir = @"IDDB\";
        g_sLoginServer_FeedIDList = "FeedIDList.txt";
        g_sLoginServer_FeedIPList = "FeedIPList.txt";
        g_sLoginServer_CountLogDir = @"CountLog\";
        g_sLoginServer_WebLogDir = @"GameWFolder\";
        g_sLoginServer_ChrLogDir = @"ChrLog\";
        g_sLoginServer_IDLogDir = @"IDLogDir\";
        g_sLoginServer_AddrTableFile = "!addrtable.txt";
        g_sLoginServer_ServeraddrFile = "!serveraddr.txt";
        g_sLoginServerUserLimitFile = "!UserLimit.txt";
        g_sLoginServerFeedIDListFile = "FeedIDList.txt";
        g_sLoginServerFeedIPListFile = "FeedIPList.txt";
        g_nLoginServer_MainFormX = 251;
        g_nLoginServer_MainFormY = 0;
        g_nLoginServer_RouteList = new List<object>();

        g_sLogServer_ProgramFile = "LogDataServer.exe";
        g_sLogServer_Directory = @"LogServer\";
        g_boLogServer_GetStart = true;
        g_boLogServer_GetMinimize = true;
        g_sLogServer_ConfigFile = "LogData.ini";
        g_sLogServer_BaseDir = @"BaseDir\";
        g_sLogServer_ServerAddr = "127.0.0.1";
        g_nLogServer_Port = 10000;
        g_nLogServer_MainFormX = 251;
        g_nLogServer_MainFormY = 239;

        g_sM2Server_ProgramFile = "M2Server.exe";
        g_sM2Server_Directory = @"Mir200\";
        g_boM2Server_GetStart = true;
        g_boM2Server_GetMinimize = true;
        g_sM2Server_ConfigFile = "!setup.txt";
        g_sM2Server_AbuseFile = "!abuse.txt";
        g_sM2Server_RunAddrFile = "!runaddr.txt";
        g_sM2Server_ServerTableFile = "!servertable.txt";
        g_nM2Server_ServerNumber = 0;
        g_nM2Server_ServerIndex = 0;
        g_nM2Server_EditionId = 0;
        g_nM2Server_AreaId = 0;
        g_boM2Server_VentureServer = false;
        g_boM2Server_TestServer = true;
        g_nM2Server_TestLevel = 1;
        g_nM2Server_TestGold = 0;
        g_boM2Server_ServiceMode = false;
        g_boM2Server_NonPKServer = false;
        g_sM2Server_MsgSrvAddr = "127.0.0.1";
        g_nM2Server_MsgSrvPort = 4900;
        g_sM2Server_GateAddr = "127.0.0.1";
        g_nM2Server_GatePort = 5000;
        g_sM2Server_BaseDir = @"Share\";
        g_sM2Server_GuildDir = @"GuildBase\Guilds\";
        g_sM2Server_GuildFile = @"GuildBase\Guildlist.txt";
        g_sM2Server_VentureDir = @"ShareV\";
        g_sM2Server_ConLogDir = @"ConLog\";
        g_sM2Server_LogDir = @"Log\";
        g_sM2Server_CastleDir = @"Castle\";
        g_sM2Server_EnvirDir = @"Envir\";
        g_sM2Server_MapDir = @"Map\";
        g_sM2Server_NoticeDir = @"Notice\";
        g_sM2Server_BoxsDir = @"Envir\Boxs\";
        g_nM2Server_MainFormX = 560;
        g_nM2Server_MainFormY = 0;
        g_sM2Server_CastleFile = @"Castle\List.txt";

        g_sLoginGate_ProgramFile = "LoginGate.exe";
        g_sLoginGate_Directory = @"LoginGate\";
        g_boLoginGate_GetStart = true;
        g_boLoginGate_GetMinimize = true;
        g_sLoginGate_ConfigFile = "Config.ini";
        g_sLoginGate_ServerAddr = "127.0.0.1";
        g_sLoginGate_ServerAddr1 = "127.0.0.2";
        g_nLoginGate_ServerPort = 5500;
        g_sLoginGate_GateAddr = "0.0.0.0";
        g_nLoginGate_GatePort = 7000;
        g_nLoginGate_ShowLogLevel = 3;
        g_nLoginGate_MaxConnOfIPaddr = 20;
        g_nLoginGate_BlockMethod = 0;
        g_nLoginGate_KeepConnectTimeOut = 60000;
        g_nLoginGate_MainFormX = 0;
        g_nLoginGate_MainFormY = 0;

        g_sSelGate_ProgramFile = "SelGate.exe";
        g_sSelGate_Directory = @"SelGate\";
        g_boSelGate_GetStart = true;
        g_boSelGate_GetStart1 = false;
        g_boSelGate_GetMinimize = true;
        g_sSelGate_ConfigFile = "Config.ini";
        g_sSelGate_ServerAddr = "127.0.0.1";
        g_nSelGate_ServerPort = 5100;
        g_sSelGate_GateAddr = "0.0.0.0";
        g_nSeLGate_GatePort = 7100;
        g_sSelGate_GateAddr1 = "0.0.0.0";
        g_nSeLGate_GatePort1 = 7101;
        g_nSelGate_ShowLogLevel = 3;
        g_nSelGate_MaxConnOfIPaddr = 20;
        g_nSelGate_BlockMethod = 0;
        g_nSelGate_KeepConnectTimeOut = 60000;
        g_nSelGate_MainFormX = 0;
        g_nSelGate_MainFormY = 163;
        g_boSelGate_GetMultiThread = true;

        g_sRunGate_ProgramFile = "RunGate.exe";
        g_sRunGate_RegKey = "ABCDEFGHIJKL";
        g_sRunGate_Directory = @"RunGate\";
        g_sRunGate_DirectoryEx = @"RunGate%d\";
        g_sRunGate_AddrTableFile = "!addrtable.txt";
        g_boRunGate1_GetStart = true;
        g_boRunGate2_GetStart = true;
        g_boRunGate3_GetStart = true;
        g_boRunGate4_GetStart = true;
        g_boRunGate5_GetStart = true;
        g_boRunGate6_GetStart = true;
        g_boRunGate7_GetStart = true;
        g_boRunGate_GetStart = true;
        g_boRunGate_GetMinimize = true;
        g_boRunGate_GetMultiThread = false;
        g_sRunGate_ConfigFile = "Config.ini";
        g_nRunGate_Count = 3;
        g_sRunGate_ServerAddr = "127.0.0.1";
        g_nRunGate_ServerPort = 5000;
        g_sRunGate_GateAddr = "0.0.0.0";
        g_nRunGate_GatePort = 7200;
        g_sRunGate1_GateAddr = "0.0.0.0";
        g_nRunGate1_GatePort = 7300;
        g_sRunGate2_GateAddr = "0.0.0.0";
        g_nRunGate2_GatePort = 7400;
        g_sRunGate3_GateAddr = "0.0.0.0";
        g_nRunGate3_GatePort = 7500;
        g_sRunGate4_GateAddr = "0.0.0.0";
        g_nRunGate4_GatePort = 7600;
        g_sRunGate5_GateAddr = "0.0.0.0";
        g_nRunGate5_GatePort = 7700;
        g_sRunGate6_GateAddr = "0.0.0.0";
        g_nRunGate6_GatePort = 7800;
        g_sRunGate7_GateAddr = "0.0.0.0";
        g_nRunGate7_GatePort = 7900;
        g_nRunGate0_DBPort = 27201;
        g_nRunGate1_DBPort = 27301;
        g_nRunGate2_DBPort = 27401;
        g_nRunGate3_DBPort = 27501;
        g_nRunGate4_DBPort = 27601;
        g_nRunGate5_DBPort = 27701;
        g_nRunGate6_DBPort = 27801;
        g_nRunGate7_DBPort = 27901;
        g_nRunGateDBPort_MulThread = 27201;
        g_RunGateInfo = new TRunGateInfo[GShareConst.MAXRUNGATECOUNT];

        DBServer = default;
        LoginServer = default;
        LogServer = default;
        M2Server = default;
        RunGate = new TProgram[GShareConst.MAXRUNGATECOUNT];
        SelGate = default;
        SelGate1 = default;
        LoginGate = default;
        LoginGate1 = default;

        g_dwStopTick = 0;
        g_dwStopTimeOut = 10000;
        g_boEmbeddedWindow = false;
        g_nAutoStartDelayTime = 60;
        g_boAutoStartServer = false;
        g_nAutoStartTimeCount = 0;
    }

    /// <summary>
    /// 声明完整性核对表：GShare.pas 的每一个全局变量名 → 期望的托管类型名。
    /// 测试以此断言"声明段 100% 覆盖"，任何漏项/改名都会被测出来。
    /// </summary>
    public static readonly Dictionary<string, string> ExpectedGlobals = new()
    {
        { "g_BackUpManager", "IBackUpManager" },
        { "g_boHeroDBOK", "Boolean" },
        { "g_sGameName", "String" },
        { "g_nFormIdx", "Int32" },
        { "g_IniConf", "IGameCenterIniFile" },
        { "g_sButtonStartGame", "String" },
        { "g_sButtonStopGame", "String" },
        { "g_sButtonStopStartGame", "String" },
        { "g_sButtonStopStopGame", "String" },
        { "g_sConfFile", "String" },
        { "g_sGameDirectory", "String" },
        { "g_sOldGameDirectory", "String" },
        { "g_boUseSqliteDB", "Boolean" },
        { "g_sHeroDBName", "String" },
        { "g_sSqliteDBName", "String" },
        { "g_nDataSaveDBType", "Int32" },
        { "g_sDataSaveDBServer", "String" },
        { "g_wDataSaveDBPort", "UInt16" },
        { "g_sDataSaveDBUser", "String" },
        { "g_sDataSaveDBPassword", "String" },
        { "g_sDataSaveDataBase", "String" },
        { "g_sAllIPaddr", "String" },
        { "g_sLocalIPaddr", "String" },
        { "g_sLocalIPaddr1", "String" },
        { "g_sExtIPaddr", "String" },
        { "g_sExtNetComIPaddr", "String" },
        { "g_boDoubleLineMode", "Boolean" },
        { "g_nLimitOnlineUser", "Int32" },
        { "g_boDynamicIPMode", "Boolean" },
        { "g_sDBServer_ProgramFile", "String" },
        { "g_sDBServer_Directory", "String" },
        { "g_boDBServer_GetStart", "Boolean" },
        { "g_boDBServer_GetMinimize", "Boolean" },
        { "g_sDBServer_ConfigFile", "String" },
        { "g_sDBServer_Config_ServerAddr", "String" },
        { "g_nDBServer_Config_ServerPort", "Int32" },
        { "g_sDBServer_Config_GateAddr", "String" },
        { "g_nDBServer_Config_GatePort", "Int32" },
        { "g_sDBServer_Config_IDSAddr", "String" },
        { "g_nDBServer_Config_IDSPort", "Int32" },
        { "g_nDBServer_Config_Interval", "Int32" },
        { "g_nDBServer_Config_Level1", "Int32" },
        { "g_nDBServer_Config_Level2", "Int32" },
        { "g_nDBServer_Config_Level3", "Int32" },
        { "g_nDBServer_Config_Day1", "Int32" },
        { "g_nDBServer_Config_Day2", "Int32" },
        { "g_nDBServer_Config_Day3", "Int32" },
        { "g_nDBServer_Config_Month1", "Int32" },
        { "g_nDBServer_Config_Month2", "Int32" },
        { "g_nDBServer_Config_Month3", "Int32" },
        { "g_sDBServer_Config_Dir", "String" },
        { "g_sDBServer_Config_IdDir", "String" },
        { "g_sDBServer_Config_HumDir", "String" },
        { "g_sDBServer_Config_FeeDir", "String" },
        { "g_sDBServer_Config_BackupDir", "String" },
        { "g_sDBServer_Config_ConnectDir", "String" },
        { "g_sDBServer_Config_LogDir", "String" },
        { "g_sDBServer_Config_MapFile", "String" },
        { "g_boDBServer_Config_ViewHackMsg", "Boolean" },
        { "g_sDBServer_AddrTableFile", "String" },
        { "g_sDBServer_GateListFile", "String" },
        { "g_sDBServer_ServerinfoFile", "String" },
        { "g_nDBServer_MainFormX", "Int32" },
        { "g_nDBServer_MainFormY", "Int32" },
        { "g_boDBServer_DisableAutoGame", "Boolean" },
        { "g_sLoginServer_ProgramFile", "String" },
        { "g_sLoginServer_Directory", "String" },
        { "g_sLoginServer_ConfigFile", "String" },
        { "g_boLoginServer_GetStart", "Boolean" },
        { "g_boLoginServer_GetMinimize", "Boolean" },
        { "g_sLoginServer_GateAddr", "String" },
        { "g_nLoginServer_GatePort", "Int32" },
        { "g_sLoginServer_ServerAddr", "String" },
        { "g_nLoginServer_ServerPort", "Int32" },
        { "g_nLoginServer_ControlPort", "Int32" },
        { "g_sLoginServer_ReadyServers", "Int32" },
        { "g_sLoginServer_EnableMakingID", "Boolean" },
        { "g_sLoginServer_EnableTrial", "Boolean" },
        { "g_sLoginServer_TestServer", "Boolean" },
        { "g_sLoginServer_IdDir", "String" },
        { "g_sLoginServer_FeedIDList", "String" },
        { "g_sLoginServer_FeedIPList", "String" },
        { "g_sLoginServer_CountLogDir", "String" },
        { "g_sLoginServer_WebLogDir", "String" },
        { "g_sLoginServer_ChrLogDir", "String" },
        { "g_sLoginServer_IDLogDir", "String" },
        { "g_sLoginServer_AddrTableFile", "String" },
        { "g_sLoginServer_ServeraddrFile", "String" },
        { "g_sLoginServerUserLimitFile", "String" },
        { "g_sLoginServerFeedIDListFile", "String" },
        { "g_sLoginServerFeedIPListFile", "String" },
        { "g_nLoginServer_MainFormX", "Int32" },
        { "g_nLoginServer_MainFormY", "Int32" },
        { "g_nLoginServer_RouteList", "List`1" },
        { "g_sLogServer_ProgramFile", "String" },
        { "g_sLogServer_Directory", "String" },
        { "g_boLogServer_GetStart", "Boolean" },
        { "g_boLogServer_GetMinimize", "Boolean" },
        { "g_sLogServer_ConfigFile", "String" },
        { "g_sLogServer_BaseDir", "String" },
        { "g_sLogServer_ServerAddr", "String" },
        { "g_nLogServer_Port", "Int32" },
        { "g_nLogServer_MainFormX", "Int32" },
        { "g_nLogServer_MainFormY", "Int32" },
        { "g_sM2Server_ProgramFile", "String" },
        { "g_sM2Server_Directory", "String" },
        { "g_boM2Server_GetStart", "Boolean" },
        { "g_boM2Server_GetMinimize", "Boolean" },
        { "g_sM2Server_ConfigFile", "String" },
        { "g_sM2Server_AbuseFile", "String" },
        { "g_sM2Server_RunAddrFile", "String" },
        { "g_sM2Server_ServerTableFile", "String" },
        { "g_nM2Server_ServerNumber", "Int32" },
        { "g_nM2Server_ServerIndex", "Int32" },
        { "g_nM2Server_EditionId", "Int32" },
        { "g_nM2Server_AreaId", "Int32" },
        { "g_boM2Server_VentureServer", "Boolean" },
        { "g_boM2Server_TestServer", "Boolean" },
        { "g_nM2Server_TestLevel", "Int32" },
        { "g_nM2Server_TestGold", "Int32" },
        { "g_boM2Server_ServiceMode", "Boolean" },
        { "g_boM2Server_NonPKServer", "Boolean" },
        { "g_sM2Server_MsgSrvAddr", "String" },
        { "g_nM2Server_MsgSrvPort", "Int32" },
        { "g_sM2Server_GateAddr", "String" },
        { "g_nM2Server_GatePort", "Int32" },
        { "g_sM2Server_BaseDir", "String" },
        { "g_sM2Server_GuildDir", "String" },
        { "g_sM2Server_GuildFile", "String" },
        { "g_sM2Server_VentureDir", "String" },
        { "g_sM2Server_ConLogDir", "String" },
        { "g_sM2Server_LogDir", "String" },
        { "g_sM2Server_CastleDir", "String" },
        { "g_sM2Server_EnvirDir", "String" },
        { "g_sM2Server_MapDir", "String" },
        { "g_sM2Server_NoticeDir", "String" },
        { "g_sM2Server_BoxsDir", "String" },
        { "g_nM2Server_MainFormX", "Int32" },
        { "g_nM2Server_MainFormY", "Int32" },
        { "g_sM2Server_CastleFile", "String" },
        { "g_sLoginGate_ProgramFile", "String" },
        { "g_sLoginGate_Directory", "String" },
        { "g_boLoginGate_GetStart", "Boolean" },
        { "g_boLoginGate_GetMinimize", "Boolean" },
        { "g_sLoginGate_ConfigFile", "String" },
        { "g_sLoginGate_ServerAddr", "String" },
        { "g_sLoginGate_ServerAddr1", "String" },
        { "g_nLoginGate_ServerPort", "Int32" },
        { "g_sLoginGate_GateAddr", "String" },
        { "g_nLoginGate_GatePort", "Int32" },
        { "g_nLoginGate_ShowLogLevel", "Int32" },
        { "g_nLoginGate_MaxConnOfIPaddr", "Int32" },
        { "g_nLoginGate_BlockMethod", "Int32" },
        { "g_nLoginGate_KeepConnectTimeOut", "Int32" },
        { "g_nLoginGate_MainFormX", "Int32" },
        { "g_nLoginGate_MainFormY", "Int32" },
        { "g_sSelGate_ProgramFile", "String" },
        { "g_sSelGate_Directory", "String" },
        { "g_boSelGate_GetStart", "Boolean" },
        { "g_boSelGate_GetStart1", "Boolean" },
        { "g_boSelGate_GetMinimize", "Boolean" },
        { "g_sSelGate_ConfigFile", "String" },
        { "g_sSelGate_ServerAddr", "String" },
        { "g_nSelGate_ServerPort", "Int32" },
        { "g_sSelGate_GateAddr", "String" },
        { "g_nSeLGate_GatePort", "Int32" },
        { "g_sSelGate_GateAddr1", "String" },
        { "g_nSeLGate_GatePort1", "Int32" },
        { "g_nSelGate_ShowLogLevel", "Int32" },
        { "g_nSelGate_MaxConnOfIPaddr", "Int32" },
        { "g_nSelGate_BlockMethod", "Int32" },
        { "g_nSelGate_KeepConnectTimeOut", "Int32" },
        { "g_nSelGate_MainFormX", "Int32" },
        { "g_nSelGate_MainFormY", "Int32" },
        { "g_boSelGate_GetMultiThread", "Boolean" },
        { "g_sRunGate_ProgramFile", "String" },
        { "g_sRunGate_RegKey", "String" },
        { "g_sRunGate_Directory", "String" },
        { "g_sRunGate_DirectoryEx", "String" },
        { "g_sRunGate_AddrTableFile", "String" },
        { "g_boRunGate1_GetStart", "Boolean" },
        { "g_boRunGate2_GetStart", "Boolean" },
        { "g_boRunGate3_GetStart", "Boolean" },
        { "g_boRunGate4_GetStart", "Boolean" },
        { "g_boRunGate5_GetStart", "Boolean" },
        { "g_boRunGate6_GetStart", "Boolean" },
        { "g_boRunGate7_GetStart", "Boolean" },
        { "g_boRunGate_GetStart", "Boolean" },
        { "g_boRunGate_GetMinimize", "Boolean" },
        { "g_boRunGate_GetMultiThread", "Boolean" },
        { "g_sRunGate_ConfigFile", "String" },
        { "g_nRunGate_Count", "Int32" },
        { "g_sRunGate_ServerAddr", "String" },
        { "g_nRunGate_ServerPort", "Int32" },
        { "g_sRunGate_GateAddr", "String" },
        { "g_nRunGate_GatePort", "Int32" },
        { "g_sRunGate1_GateAddr", "String" },
        { "g_nRunGate1_GatePort", "Int32" },
        { "g_sRunGate2_GateAddr", "String" },
        { "g_nRunGate2_GatePort", "Int32" },
        { "g_sRunGate3_GateAddr", "String" },
        { "g_nRunGate3_GatePort", "Int32" },
        { "g_sRunGate4_GateAddr", "String" },
        { "g_nRunGate4_GatePort", "Int32" },
        { "g_sRunGate5_GateAddr", "String" },
        { "g_nRunGate5_GatePort", "Int32" },
        { "g_sRunGate6_GateAddr", "String" },
        { "g_nRunGate6_GatePort", "Int32" },
        { "g_sRunGate7_GateAddr", "String" },
        { "g_nRunGate7_GatePort", "Int32" },
        { "g_nRunGate0_DBPort", "Int32" },
        { "g_nRunGate1_DBPort", "Int32" },
        { "g_nRunGate2_DBPort", "Int32" },
        { "g_nRunGate3_DBPort", "Int32" },
        { "g_nRunGate4_DBPort", "Int32" },
        { "g_nRunGate5_DBPort", "Int32" },
        { "g_nRunGate6_DBPort", "Int32" },
        { "g_nRunGate7_DBPort", "Int32" },
        { "g_nRunGateDBPort_MulThread", "Int32" },
        { "g_RunGateInfo", "TRunGateInfo[]" },
        { "DBServer", "TProgram" },
        { "LoginServer", "TProgram" },
        { "LogServer", "TProgram" },
        { "M2Server", "TProgram" },
        { "RunGate", "TProgram[]" },
        { "SelGate", "TProgram" },
        { "SelGate1", "TProgram" },
        { "LoginGate", "TProgram" },
        { "LoginGate1", "TProgram" },
        { "g_dwStopTick", "UInt32" },
        { "g_dwStopTimeOut", "UInt32" },
        { "g_boEmbeddedWindow", "Boolean" },
        { "g_nAutoStartDelayTime", "Int32" },
        { "g_boAutoStartServer", "Boolean" },
        { "g_nAutoStartTimeCount", "Int32" },
    };

    /// <summary>测试辅助：按 <see cref="ExpectedGlobals"/> 断言字段存在且类型匹配，返回缺失/类型不符清单。</summary>
    public static List<string> VerifyDeclarations()
    {
        var problems = new List<string>();
        var t = typeof(GShareGlobals);
        foreach (var kv in ExpectedGlobals)
        {
            FieldInfo? f = t.GetField(kv.Key, BindingFlags.Public | BindingFlags.Static);
            if (f == null) { problems.Add($"missing field: {kv.Key}"); continue; }
            string actual = f.FieldType.Name;
            if (actual != kv.Value) problems.Add($"type mismatch: {kv.Key} expected {kv.Value} actual {actual}");
        }
        return problems;
    }
}

/// <summary>
/// GShare.pas:49 <c>procedure LoadConfig()</c> / GMain.pas:1590 调用的接缝说明：
/// <c>g_BackUpManager: TBackUpManager</c> 属 DataBackUp.pas（本车道未移植）。
/// 这里只给出 SaveBackList/LoadBackList（GMain.pas:1446-1517）实际触碰的最小成员集合。
/// </summary>
public interface IBackUpManager
{
    /// <summary>DataBackUp.pas <c>TBackUpManager.m_BackUpList</c>（TList）。</summary>
    IList<IBackUpTask> m_BackUpList { get; }

    /// <summary>DataBackUp.pas <c>TBackUpManager.Add</c>。</summary>
    void Add(IBackUpTask task);
}

/// <summary>
/// DataBackUp.pas <c>TBackUpTask</c> 的 SaveBackList/LoadBackList 所需字段子集。
/// 接缝：待 <c>DataBackUp.pas</c> 移植后接入完整实现。
/// </summary>
public interface IBackUpTask
{
    /// <summary>源目录。</summary>
    string SourceDirectory { get; set; }

    /// <summary>目标目录。</summary>
    string DestDirectory { get; set; }

    /// <summary>备份模式（0/1，对应 RadioButtonBackMode1/2）。</summary>
    byte Mode { get; set; }

    /// <summary>小时。</summary>
    ushort Hour { get; set; }

    /// <summary>分钟。</summary>
    ushort Min { get; set; }

    /// <summary>是否启动自动备份。</summary>
    bool Start { get; set; }

    /// <summary>是否压缩（piaoyun 2013-08-30）。</summary>
    bool IsCompress { get; set; }

    /// <summary>已备份次数（RefBackListToView 列 3）。</summary>
    int BackUpCount { get; }

    /// <summary>失败次数（RefBackListToView 列 4）。</summary>
    int FailCount { get; }
}
