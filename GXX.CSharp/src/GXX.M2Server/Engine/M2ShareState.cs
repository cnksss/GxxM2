using GXX.Core.Util;

namespace GXX.M2Server.Engine;

/// <summary>
/// M2Share.pas 数据库/全局开关与 Config INI 对象（批次J1：GeneralConfig.pas 依赖子集，1:1）。
/// Delphi 侧 Config := TIniFile.Create(g_sSelfFilePath + sConfigFileName)（'!Setup.txt'）。
/// </summary>
public static class M2ShareState
{
    /// <summary>服务器索引（Delphi nServerIndex: Integer = 0）。</summary>
    public static int nServerIndex = 0;

    /// <summary>BDE 数据源名称（g_sDBName = 'HeroDB'）。</summary>
    public static string g_sDBName = "HeroDB";

    /// <summary>Sqlite 数据库文件（g_sSqliteDBName）。</summary>
    public static string g_sSqliteDBName = @"D:\MirServer\Mud2\DB\GxxM2.db";

    /// <summary>g_boUseSqliteDB = False。</summary>
    public static bool g_boUseSqliteDB;

    /// <summary>g_boShowBlockIPLog = False。</summary>
    public static bool g_boShowBlockIPLog;

    /// <summary>g_sSelfFilePath（ExtractFilePath(Application.ExeName)）。</summary>
    public static string g_sSelfFilePath = AppContext.BaseDirectory;

    public const string sConfigFileName = "!Setup.txt";

    /// <summary>Delphi sExpConfigFileName = 'Exps.ini'（ExpConfig 对象）。</summary>
    public const string sExpConfigFileName = "Exps.ini";
    /// <summary>Delphi sCommandFileName = 'Command.ini'（CommandConf 对象）。</summary>
    public const string sCommandFileName = "Command.ini";
    /// <summary>Delphi sStringFileName = 'String.ini'（StringConf 对象）。</summary>
    public const string sStringFileName = "String.ini";

    /// <summary>GM 红色消息命令字符（g_GMRedMsgCmd: Char = '!'）。</summary>
    public static char g_GMRedMsgCmd = '!';
    /// <summary>私聊等级后缀（g_sShowWhisperLevelMsg = '[Lv%u]'）。</summary>
    public static string g_sShowWhisperLevelMsg = "[Lv%u]";

    private static TFastIniFile? _config;
    private static TFastIniFile? _expConfig;
    private static TFastIniFile? _commandConf;
    private static TFastIniFile? _stringConf;

    /// <summary>M2Share 全局 Config（懒加载等效 Delphi initialization 段创建）。</summary>
    public static TFastIniFile ConfigIni =>
        _config ??= new TFastIniFile(Path.Combine(g_sSelfFilePath, sConfigFileName));

    /// <summary>M2Share 全局 ExpConfig（Exps.ini）。</summary>
    public static TFastIniFile ExpConfigIni =>
        _expConfig ??= new TFastIniFile(Path.Combine(g_sSelfFilePath, sExpConfigFileName));

    /// <summary>M2Share 全局 CommandConf（Command.ini）。</summary>
    public static TFastIniFile CommandConfIni =>
        _commandConf ??= new TFastIniFile(Path.Combine(g_sSelfFilePath, sCommandFileName));

    /// <summary>M2Share 全局 StringConf（String.ini）。</summary>
    public static TFastIniFile StringConfIni =>
        _stringConf ??= new TFastIniFile(Path.Combine(g_sSelfFilePath, sStringFileName));

    /// <summary>M2Share initialization 段 Server/Share 节读取（1:1 顺序与键名，布尔串经 CompareText 'TRUE' 判定）。</summary>
    public static void LoadGeneralConfigFromIni()
    {
        var cfg = ConfigIni;
        M2Config.sServerName = cfg.ReadString("Server", "ServerName", M2Config.sServerName);
        M2Config.nServerNumber = cfg.ReadInteger("Server", "ServerNumber", M2Config.nServerNumber);
        M2Config.boTestServer = string.Equals(cfg.ReadString("Server", "TestServer", "FALSE"), "TRUE", StringComparison.OrdinalIgnoreCase);
        M2Config.nTestLevel = cfg.ReadInteger("Server", "TestLevel", M2Config.nTestLevel);
        M2Config.nTestGold = cfg.ReadInteger("Server", "TestGold", M2Config.nTestGold);
        M2Config.nTestUserLimit = cfg.ReadInteger("Server", "TestServerUserLimit", M2Config.nTestUserLimit);
        M2Config.boServiceMode = string.Equals(cfg.ReadString("Server", "ServiceMode", "FALSE"), "TRUE", StringComparison.OrdinalIgnoreCase);
        M2Config.sGateAddr = cfg.ReadString("Server", "GateAddr", M2Config.sGateAddr);
        M2Config.nGatePort = cfg.ReadInteger("Server", "GatePort", M2Config.nGatePort);
        M2Config.sDBAddr = cfg.ReadString("Server", "DBAddr", M2Config.sDBAddr);
        M2Config.nDBPort = cfg.ReadInteger("Server", "DBPort", M2Config.nDBPort);
        M2Config.sIDSAddr = cfg.ReadString("Server", "IDSAddr", M2Config.sIDSAddr);
        M2Config.nIDSPort = cfg.ReadInteger("Server", "IDSPort", M2Config.nIDSPort);
        M2Config.sMsgSrvAddr = cfg.ReadString("Server", "MsgSrvAddr", M2Config.sMsgSrvAddr);
        M2Config.nMsgSrvPort = cfg.ReadInteger("Server", "MsgSrvPort", M2Config.nMsgSrvPort);
        M2Config.sLogServerAddr = cfg.ReadString("Server", "LogServerAddr", M2Config.sLogServerAddr);
        M2Config.nLogServerPort = cfg.ReadInteger("Server", "LogServerPort", M2Config.nLogServerPort);
        M2Config.nUserFull = cfg.ReadInteger("Server", "UserFull", M2Config.nUserFull);

        M2Config.sGuildDir = cfg.ReadString("Share", "GuildDir", M2Config.sGuildDir);
        M2Config.sGuildFile = cfg.ReadString("Share", "GuildFile", M2Config.sGuildFile);
        M2Config.sVentureDir = cfg.ReadString("Share", "VentureDir", M2Config.sVentureDir);
        M2Config.sConLogDir = cfg.ReadString("Share", "ConLogDir", M2Config.sConLogDir);
        M2Config.sCastleDir = cfg.ReadString("Share", "CastleDir", M2Config.sCastleDir);
        M2Config.sEnvirDir = cfg.ReadString("Share", "EnvirDir", M2Config.sEnvirDir);
        M2Config.sMapDir = cfg.ReadString("Share", "MapDir", M2Config.sMapDir);
        M2Config.sNoticeDir = cfg.ReadString("Share", "NoticeDir", M2Config.sNoticeDir);
        M2Config.sPlugDir = cfg.ReadString("Share", "PlugDir", M2Config.sPlugDir);
        M2Config.sBoxsDir = cfg.ReadString("Share", "BoxsDir", M2Config.sBoxsDir);
    }

    /// <summary>测试隔离：重建 Config/Exp/Command/String 各 INI 并复位全局。</summary>
    public static void ResetForTests(string? selfFilePath)
    {
        _config?.Dispose();
        _expConfig?.Dispose();
        _commandConf?.Dispose();
        _stringConf?.Dispose();
        _config = null;
        _expConfig = null;
        _commandConf = null;
        _stringConf = null;
        g_sSelfFilePath = selfFilePath ?? AppContext.BaseDirectory;
        g_GMRedMsgCmd = '!';
        g_sShowWhisperLevelMsg = "[Lv%u]";
    }
}
