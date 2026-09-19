namespace GXX.M2Server.Engine;

/// <summary>
/// M2Share.pas g_Config 字段子集（批次J3）：动作速度控制 + 系统消息前缀。
/// </summary>
public static partial class M2Config
{
    // ---- 动作速度控制（ActionSpeedConfig.pas 使用；typed constant 默认值 1:1） ----
    public static bool boControlActionInterval = true;
    public static bool boControlWalkHit = true;
    public static bool boControlRunLongHit = true;
    public static bool boControlRunHit = true;
    public static bool boControlRunMagic = true;
    public static uint dwActionIntervalTime = 200;      // 组合操作间隔
    public static uint dwRunLongHitIntervalTime = 200;  // 跑位刺杀间隔
    public static uint dwRunHitIntervalTime = 200;      // 跑位攻击间隔
    public static uint dwWalkHitIntervalTime = 200;     // 走位攻击间隔
    public static uint dwRunMagicIntervalTime = 300;    // 跑位魔法间隔

    // ---- 系统消息前缀（typed constant 默认值 1:1） ----
    public static string sLineNoticePreFix = "〖公告〗";
    public static string sSysMsgPreFix = "〖系统〗";
    public static string sGuildMsgPreFix = "〖行会〗";
    public static string sGroupMsgPreFix = "〖组队〗";
    public static string sHintMsgPreFix = "〖提示〗";
    public static string sGMRedMsgpreFix = "〖ＧＭ〗";
    public static string sMonSayMsgpreFix = "〖怪物〗";
    public static string sCustMsgpreFix = "〖祝福〗";
    public static string sCastleMsgpreFix = "〖城主〗";
    public static string sNationMsgPreFix = "〖国家〗";

    public static void ResetSpeedDefaults()
    {
        boControlActionInterval = true;
        boControlWalkHit = true;
        boControlRunLongHit = true;
        boControlRunHit = true;
        boControlRunMagic = true;
        dwActionIntervalTime = 200;
        dwRunLongHitIntervalTime = 200;
        dwRunHitIntervalTime = 200;
        dwWalkHitIntervalTime = 200;
        dwRunMagicIntervalTime = 300;
    }

    // ---- 元宝族名称（typed constant 默认值 1:1，ViewOnlineHuman 表头使用） ----
    public static string sGameGoldName = "元宝";
    public static string sGamePointName = "游戏点";
    public static string sGameDiamondName = "金刚石";
    public static string sGameGirdName = "灵符";
    public static string sCreditPointName = "声望";
    public static string sPayMentPointName = "秒卡点";

    public static void ResetMsgPrefixDefaults()
    {
        sLineNoticePreFix = "〖公告〗";
        sSysMsgPreFix = "〖系统〗";
        sGuildMsgPreFix = "〖行会〗";
        sGroupMsgPreFix = "〖组队〗";
        sHintMsgPreFix = "〖提示〗";
        sGMRedMsgpreFix = "〖ＧＭ〗";
        sMonSayMsgpreFix = "〖怪物〗";
        sCustMsgpreFix = "〖祝福〗";
        sCastleMsgpreFix = "〖城主〗";
        sNationMsgPreFix = "〖国家〗";
    }
}

/// <summary>
/// M2Share.pas TOnlineMsgControl + g_OnlineMsgControl（boSaveXxx 持久组 / boXxx 活动组，默认全 False）。
/// </summary>
public class OnlineMsgControl
{
    public bool boSaveDisableTrading;
    public bool boSaveDisableRepair;
    public bool boSaveDisableBuy;
    public bool boSaveDisableSell;
    public bool boSaveDisableSaveToStorage;
    public bool boSaveDisableGetFromStorage;
    public bool boSaveDisableDropItem;
    public bool boSaveDisableUseNpc;
    public bool boSaveDisableChallenge;
    public bool boSaveDisableShop;
    public bool boDisableTrading;
    public bool boDisableRepair;
    public bool boDisableBuy;
    public bool boDisableSell;
    public bool boDisableSaveToStorage;
    public bool boDisableGetFromStorage;
    public bool boDisableDropItem;
    public bool boDisableUseNpc;
    public bool boDisableChallenge;
    public bool boDisableShop;

    /// <summary>M2Share 全局实例（g_OnlineMsgControl）。</summary>
    public static readonly OnlineMsgControl g_OnlineMsgControl = new();

    public static void ResetDefaults()
    {
        var o = g_OnlineMsgControl;
        o.boSaveDisableTrading = false; o.boSaveDisableRepair = false; o.boSaveDisableBuy = false;
        o.boSaveDisableSell = false; o.boSaveDisableSaveToStorage = false; o.boSaveDisableGetFromStorage = false;
        o.boSaveDisableDropItem = false; o.boSaveDisableUseNpc = false; o.boSaveDisableChallenge = false;
        o.boSaveDisableShop = false;
        o.boDisableTrading = false; o.boDisableRepair = false; o.boDisableBuy = false;
        o.boDisableSell = false; o.boDisableSaveToStorage = false; o.boDisableGetFromStorage = false;
        o.boDisableDropItem = false; o.boDisableUseNpc = false; o.boDisableChallenge = false;
        o.boDisableShop = false;
    }
}
