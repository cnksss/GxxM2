namespace GXX.M2Server.Engine;

/// <summary>
/// 批次J24：GameConfig.pas General 经验页/Msg/Time/Price 页依赖 g_Config 字段
/// （M2Share.pas typed-constant 初值 1:1）。
/// </summary>
public static partial class M2Config
{
    // ---- General 页（公告/在线人数/掉率/聊天记录） ----
    public static int nSoftVersionDate = 20020522;
    public static uint dwConsoleShowUserCountTime = 10 * 60 * 1000;
    public static uint dwShowLineNoticeTime = 5 * 60 * 1000;
    public static int nLineNoticeColor = 2;
    public static bool boShowMakeItemMsg = false;
    public static bool boShowExceptionMsg = false;
    public static bool boCanOldClientLogon = true;
    public static bool boSendOnlineCount = true;
    public static int nSendOnlineCountRate = 10;
    public static uint dwSendOnlineTime = 5 * 60 * 1000;
    public static int nMonsterPowerRate = 10;
    public static bool boRecordPublicMsg = false;
    public static bool boRecordPrivateMsg = false;
    public static bool boRecordGuildMsg = false;
    public static bool boRecordCryCryMsg = false;
    public static bool boRecordGroupMsg = false;
    public static bool boRecordNationMsg = false;
    public static bool boPermissionChangeLog = true;

    // ---- 经验（ExpSave 写 Exps.ini） ----
    public static uint dwKillMonExpMultiple = 1;
    /// <summary>升级经验倍率表（Delphi LevelExpRates: TLevelNeedExp，索引 = 等级，0 位未用）。</summary>
    public static uint[] LevelExpRates = new uint[1001];
    public static bool boHighLevelKillMonFixExp = false;
    public static bool boHighLevelGroupFixExp = true;
    public static int nHighLevel = 100;
    public static int nHighLevelGetExp = 1;
    public static bool boLimitChangeExp = true;
    public static int nMaxUpLevelCount = 10;
    public static bool boShareExpGroupSameScreen = false;
    public static bool boShareExpGroupSameMap = false;   // 分经验 - 组队不需要在同一地图
    public static bool boShareExpHeroSameMap = true;     // 分经验 - 英雄不需要在同一地图
    public static int nItemsPowerRate = 10;
    public static int nItemsACPowerRate = 10;

    // ---- 攻城时间（原 TUserCastle 内静态字段随本批迁回 g_Config 等效层） ----
    public static uint dwCastleWarTime = 3u * 60 * 60 * 1000;
    public static uint dwGetCastleTime = 10u * 60 * 1000;

    // ---- Msg 页 ----
    public static uint dwSayMsgTime = 3 * 1000;
    public static int nSayMsgCount = 2;
    public static uint dwDisableSayMsgTime = 60 * 1000;
    public static int nSayMsgMaxLen = 80;
    public static int nSayRedMsgMaxLen = 255;
    public static int nCanShoutMsgLevel = 7;
    public static bool boShutRedMsgShowGMName = false;
    public static bool boShowPreFixMsg = true;
    public static bool boShowWhisperLevelMsg = true;
    public static int nUserItemSayMsgTime = 3;
    public static int nMaxInputStringLen = 90;

    // ---- Time 页 ----
    public static int nStartCastleWarDays = 4;
    public static int nStartCastlewarTime = 20;
    public static uint dwShowCastleWarEndMsgTime = 10 * 60 * 1000;
    public static uint dwGuildWarTime = 3u * 60 * 60 * 1000;
    public static uint dwSaveHumanRcdTime = 10 * 60 * 1000;
    public static uint dwHumanFreeDelayTime = 5 * 60 * 1000;
    public static uint dwGetDBSockMsgTime = 5 * 1000;
    public static uint dwMakeGhostTime = 3u * 60 * 1000;
    public static uint dwMakeMonGhostTime = 3u * 60 * 1000;
    public static uint dwMakeDummyGhostTime = 3 * 1000;
    public static uint dwClearDropOnFloorItemTime = 180 * 1000;
    public static uint dwFloorItemCanPickUpTime = 2 * 60 * 1000;
    public static uint dwHorseTakeTime = 3;
    public static uint dwTakeOnHorseUseTime = 2;
    public static bool boReadyOnHorseDisableAction = false;
    public static uint dwNpcButtonClickTime = 300;
    public static uint dwNpcActorClickTime = 300;
    public static byte btPlayerVarJClearTime = 0;
    public static uint dwDearRecallTime = 10;
    public static uint dwMasterRecallTime = 10;
    public static uint dwGroupRecallTime = 10;

    // ---- Price 页 ----
    public static int nBuildGuildPrice = 1000000;
    public static int nGuildWarPrice = 30000;
    public static int nMakeDurgPrice = 100;
    public static int nSuperRepairPriceRate = 3;
    public static int nRepairItemDecDura = 30;

    /// <summary>typed-constant 初值复位（General/Msg/Time/Price 页字段，测试隔离用）。</summary>
    public static void ResetGameMsgTimeDefaults()
    {
        nSoftVersionDate = 20020522;
        dwConsoleShowUserCountTime = 10 * 60 * 1000;
        dwShowLineNoticeTime = 5 * 60 * 1000;
        nLineNoticeColor = 2;
        boShowMakeItemMsg = false;
        boShowExceptionMsg = false;
        boCanOldClientLogon = true;
        boSendOnlineCount = true;
        nSendOnlineCountRate = 10;
        dwSendOnlineTime = 5 * 60 * 1000;
        nMonsterPowerRate = 10;
        boRecordPublicMsg = false;
        boRecordPrivateMsg = false;
        boRecordGuildMsg = false;
        boRecordCryCryMsg = false;
        boRecordGroupMsg = false;
        boRecordNationMsg = false;
        boPermissionChangeLog = true;
        dwKillMonExpMultiple = 1;
        boHighLevelKillMonFixExp = false;
        boHighLevelGroupFixExp = true;
        nHighLevel = 100;
        nHighLevelGetExp = 1;
        boLimitChangeExp = true;
        nMaxUpLevelCount = 10;
        boShareExpGroupSameScreen = false;
        boShareExpGroupSameMap = false;
        boShareExpHeroSameMap = true;
        nItemsPowerRate = 10;
        nItemsACPowerRate = 10;
        dwCastleWarTime = 3u * 60 * 60 * 1000;
        dwGetCastleTime = 10u * 60 * 1000;
        dwSayMsgTime = 3 * 1000;
        nSayMsgCount = 2;
        dwDisableSayMsgTime = 60 * 1000;
        nSayMsgMaxLen = 80;
        nSayRedMsgMaxLen = 255;
        nCanShoutMsgLevel = 7;
        boShutRedMsgShowGMName = false;
        boShowPreFixMsg = true;
        boShowWhisperLevelMsg = true;
        nUserItemSayMsgTime = 3;
        nMaxInputStringLen = 90;
        nStartCastleWarDays = 4;
        nStartCastlewarTime = 20;
        dwShowCastleWarEndMsgTime = 10 * 60 * 1000;
        dwGuildWarTime = 3u * 60 * 60 * 1000;
        dwSaveHumanRcdTime = 10 * 60 * 1000;
        dwHumanFreeDelayTime = 5 * 60 * 1000;
        dwGetDBSockMsgTime = 5 * 1000;
        dwMakeGhostTime = 3u * 60 * 1000;
        dwMakeMonGhostTime = 3u * 60 * 1000;
        dwMakeDummyGhostTime = 3 * 1000;
        dwClearDropOnFloorItemTime = 180 * 1000;
        dwFloorItemCanPickUpTime = 2 * 60 * 1000;
        dwHorseTakeTime = 3;
        dwTakeOnHorseUseTime = 2;
        boReadyOnHorseDisableAction = false;
        dwNpcButtonClickTime = 300;
        dwNpcActorClickTime = 300;
        btPlayerVarJClearTime = 0;
        dwDearRecallTime = 10;
        dwMasterRecallTime = 10;
        dwGroupRecallTime = 10;
        nBuildGuildPrice = 1000000;
        nGuildWarPrice = 30000;
        nMakeDurgPrice = 100;
        nSuperRepairPriceRate = 3;
        nRepairItemDecDura = 30;
    }
}

/// <summary>ComboBoxLevelExp 经验计划（Delphi TLevelExpScheme 1:1，FormCreate AddItem 顺序）。</summary>
public enum TLevelExpScheme
{
    s_OldLevelExp,
    s_StdLevelExp,
    s_2Mult,
    s_5Mult,
    s_8Mult,
    s_10Mult,
    s_20Mult,
    s_30Mult,
    s_40Mult,
    s_50Mult,
    s_60Mult,
    s_70Mult,
    s_80Mult,
    s_90Mult,
    s_100Mult,
    s_150Mult,
    s_200Mult,
    s_250Mult,
    s_300Mult,
}
