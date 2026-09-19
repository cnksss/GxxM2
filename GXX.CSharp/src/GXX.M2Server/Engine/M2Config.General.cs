namespace GXX.M2Server.Engine;

/// <summary>
/// M2Share.pas g_Config: TConfig 常量初始化块中通用/目录字段子集（批次J1：GeneralConfig.pas 所需，1:1 默认值）。
/// </summary>
public static partial class M2Config
{
    // ---- Server（服务器信息） ----
    public static string sServerName = "GxxM2";
    public static int nServerNumber;
    public static bool boVentureServer;
    public static bool boTestServer = true;
    public static bool boServiceMode;
    public static bool boNonPKServer;
    public static int nTestLevel = 1;
    public static int nTestGold;
    public static int nTestUserLimit = 1000;
    public static int nUserFull = 1000;

    // ---- Server（网络地址） ----
    public static string sGateAddr = "127.0.0.1";
    public static int nGatePort = 5000;
    public static string sDBAddr = "127.0.0.1";
    public static int nDBPort = 6000;
    public static string sIDSAddr = "127.0.0.1";
    public static int nIDSPort = 5600;
    public static string sMsgSrvAddr = "127.0.0.1";
    public static int nMsgSrvPort = 4900;
    public static string sLogServerAddr = "127.0.0.1";
    public static int nLogServerPort = 10000;

    // ---- 目录（typed constant 初始化顺序 1:1） ----
    public static string sBaseDir = ".\\BaseDir\\";
    public static string sGuildDir = ".\\GuildDir\\List\\";
    public static string sGuildFile = ".\\GuildDir\\List.txt";
    public static string sVentureDir = ".\\VentureDir\\";
    public static string sConLogDir = ".\\ConLogDir\\";
    public static string sCastleDir = ".\\CastleDir\\";
    public static string sCastleFile = ".\\CastleDir\\List.txt";
    public static string sEnvirDir = ".\\Envir\\";
    public static string sMapDir = ".\\Map\\";
    public static string sNoticeDir = ".\\Notice\\";
    public static string sLogDir = ".\\Log\\";
    public static string sPlugDir = ".\\";
    public static string sBoxsDir = ".\\Envir\\Boxs\\";                 // 宝箱目录 piaoyun 2013-08-22
    public static string sBoxsFile = ".\\Envir\\Boxs\\BoxsList.txt";   // 宝箱文件(BoxsList.txt)
    public static string sSmartMonsterDir = ".\\Envir\\SmartMonster\\"; // 自定义怪物目录 chongchong 2014-07-19
    public static string sCustomMonsterClientConfigFileName = "";      // 自定义怪物生成的登录器配置文件 chongchong 2014-09-20
    public static string sCustomMagicDir = ".\\Envir\\CustomMagic\\";   // 自定义技能目录 chongchong 2014-07-19
    public static string sSmartNpcDir = ".\\Envir\\CustomNPC\\";
    public static string sItemDropLimit = ".\\Envir\\ItemDropLimit\\";
    public static string sItemDropLogDir = ".\\Envir\\ItemDropLimit\\DropLog\\";

    public static void ResetGeneralDefaults()
    {
        sServerName = "GxxM2";
        nServerNumber = 0;
        boVentureServer = false;
        boTestServer = true;
        boServiceMode = false;
        boNonPKServer = false;
        nTestLevel = 1;
        nTestGold = 0;
        nTestUserLimit = 1000;
        nUserFull = 1000;

        sGateAddr = "127.0.0.1";
        nGatePort = 5000;
        sDBAddr = "127.0.0.1";
        nDBPort = 6000;
        sIDSAddr = "127.0.0.1";
        nIDSPort = 5600;
        sMsgSrvAddr = "127.0.0.1";
        nMsgSrvPort = 4900;
        sLogServerAddr = "127.0.0.1";
        nLogServerPort = 10000;

        sBaseDir = ".\\BaseDir\\";
        sGuildDir = ".\\GuildDir\\List\\";
        sGuildFile = ".\\GuildDir\\List.txt";
        sVentureDir = ".\\VentureDir\\";
        sConLogDir = ".\\ConLogDir\\";
        sCastleDir = ".\\CastleDir\\";
        sCastleFile = ".\\CastleDir\\List.txt";
        sEnvirDir = ".\\Envir\\";
        sMapDir = ".\\Map\\";
        sNoticeDir = ".\\Notice\\";
        sLogDir = ".\\Log\\";
        sPlugDir = ".\\";
        sBoxsDir = ".\\Envir\\Boxs\\";
        sBoxsFile = ".\\Envir\\Boxs\\BoxsList.txt";
        sSmartMonsterDir = ".\\Envir\\SmartMonster\\";
        sCustomMagicDir = ".\\Envir\\CustomMagic\\";
        sSmartNpcDir = ".\\Envir\\CustomNPC\\";
        sItemDropLimit = ".\\Envir\\ItemDropLimit\\";
        sItemDropLogDir = ".\\Envir\\ItemDropLimit\\DropLog\\";
    }
}
