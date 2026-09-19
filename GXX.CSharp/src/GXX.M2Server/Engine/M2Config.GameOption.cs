namespace GXX.M2Server.Engine;

/// <summary>
/// 批次J23：GameConfig.pas 城堡/Option（跑动·交易·掉落·安全区·红名回城·国家·挑战）/PK/测试服页
/// 依赖 g_Config 字段（M2Share.pas typed-constant 初值 1:1）。
/// </summary>
public static partial class M2Config
{
    // ---- 城堡 ----
    public static int nRepairDoorPrice = 2000000;
    public static int nRepairWallPrice = 500000;
    public static int nHireArcherPrice = 300000;
    public static int nHireGuardPrice = 300000;
    public static int nCastleGoldMax = 10000000;
    public static int nCastleOneDayGold = 2000000;
    public static string sCASTLENAME = "沙巴克";
    public static string sCastleHomeMap = "3";
    public static int nCastleHomeX = 644;
    public static int nCastleHomeY = 290;
    public static int nCastleWarRangeX = 100;
    public static int nCastleWarRangeY = 100;
    public static int nCastleTaxRate = 5;
    public static bool boGetAllNpcTax = false;
    public static int nCastleMemberPriceRate = 80;

    // ---- 跑动 ----
    public static bool boDiableHumanRun = true;
    public static bool boRUNHUMAN = false;
    public static bool boRUNMON = false;
    public static bool boRunNpc = false;
    public static bool boRunGuard = false;
    public static bool boWarDisHumRun = false;
    public static bool boWarHreoRun = false;      // 攻城区域允许穿英雄
    public static bool boGMRunAll = false;
    public static bool boSafeAreaLimited = false;
    public static bool boSafeAreaDisNpcRun = false;
    public static bool boSafeAreaDisShopStallHumRun = false;
    public static bool boSafeAreaDisOffLineHumRun = false;
    public static bool boWarDisTeleport = false;  // 攻城区域禁止传送戒指

    // ---- 交易/掉落 ----
    public static uint dwTryDealTime = 3000;
    public static uint dwDealOKTime = 1000;
    public static bool boCanNotGetBackDeal = true;
    public static bool boDisableDeal = false;
    public static bool boControlDropItem = false;
    public static bool boInSafeDisableDrop = false;
    public static int nCanDropGold = 1000;
    public static int nCanDropPrice = 500;

    // ---- 安全区/起点/组队 ----
    public static int nSafeZoneSize = 10;
    public static bool boHintSafeZone = false;    // 显示进入、离开安全区提示
    public static uint dwHintSafeZoneY = 40;
    public static byte btHintSafeZoneFColor = 250;
    public static byte btHintSafeZoneBColor = 0;
    public static byte btHintSafeZoneFSize = 9;
    public static int nStartPointSize = 3;
    public static int nGroupMembersMax = 10;

    // ---- 红名/回城 ----
    public static string sRedHomeMap = "3";
    public static int nRedHomeX = 845;
    public static int nRedHomeY = 674;
    public static string sRedDieHomeMap = "3";
    public static int nRedDieHomeX = 839;
    public static int nRedDieHomeY = 668;
    public static string sHomeMap = "0";
    public static int nHomeX = 289;
    public static int nHomeY = 618;

    // ---- PK ----
    public static uint dwDecPkPointTime = 2 * 60 * 1000;
    public static int nDecPkPointCount = 1;
    public static uint dwPKFlagTime = 60 * 1000;
    public static int nKillHumanAddPKPoint = 100;
    public static int nKillHumanDecLuckPoint = 500;
    public static int nDummyAddPKPoint = 100;     // 假人杀人增加PK点
    public static uint dwKillHeroAddPKPoint = 100;
    public static bool boKillHumanWinLevel = false;
    public static bool boKilledLostLevel = false;
    public static bool boKillHumanWinExp = false;
    public static bool boKilledLostExp = false;
    public static int nKillHumanWinLevel = 1;
    public static int nKilledLostLevel = 1;
    public static int nKillHumanWinExp = 100000;
    public static int nKillHumanLostExp = 100000;
    public static int nHumanLevelDiffer = 10;
    public static bool boPKLevelProtect = false;
    public static int nPKProtectLevel = 10;
    public static int nRedPKProtectLevel = 10;
    public static uint dwKillHumanWeaponUnlockRate = 5;
    public static bool boHeroKillHumanNotWeaponUnlock = false;

    // ---- 测试服/试玩/行会/离线（OptionSave0 页） ----
    public static int nStartPermission = 0;
    public static int nHumanMaxGold = 100000000;
    public static int nHumanTryModeMaxGold = 100000;
    public static int nTryModeLevel = 7;
    public static bool boTryModeUseStorage = false;
    public static int nGuildMemberMaxLimit = 260;
    public static int nGuildNameLen = 30;
    public static int nGuildRankNameLen = 18;
    public static uint dwHumChgMapOrLoginProtectTime = 1000;
    public static bool boOffLineShop = false;
    public static bool boOffLineHero = false;
    public static bool boOffLineSlave = false;
    public static bool boSellItemToNpcShopNoCalcAddProperty = false;
    public static bool boShowNewValueFromBuyNpcItem = false;

    // ---- 挑战/国家/杂项（OptionSave3/Save 写键依赖） ----
    public static uint dwTryChallengeTime = 3000;
    public static uint dwChallengeOKTime = 1000;
    public static bool boCanNotGetBackChallenge = false;
    public static bool boDisableChallenge = false;
    public static uint dwChallengeTime = 1 * 1000 * 60;
    public static byte btChallengeGoldIndex = 0;
    public static bool boNationGroupCheck = false;
    public static bool boNationGuildCheck = false;
    public static int nNationSayLevel = 1;
    public static uint dwDecLightItemDrugTime = 500;

    /// <summary>typed-constant 初值复位（城堡/Option/PK/测试服页字段，测试隔离用）。</summary>
    public static void ResetGameOptionDefaults()
    {
        nRepairDoorPrice = 2000000;
        nRepairWallPrice = 500000;
        nHireArcherPrice = 300000;
        nHireGuardPrice = 300000;
        nCastleGoldMax = 10000000;
        nCastleOneDayGold = 2000000;
        sCASTLENAME = "沙巴克";
        sCastleHomeMap = "3";
        nCastleHomeX = 644;
        nCastleHomeY = 290;
        nCastleWarRangeX = 100;
        nCastleWarRangeY = 100;
        nCastleTaxRate = 5;
        boGetAllNpcTax = false;
        nCastleMemberPriceRate = 80;
        boDiableHumanRun = true;
        boRUNHUMAN = false;
        boRUNMON = false;
        boRunNpc = false;
        boRunGuard = false;
        boWarDisHumRun = false;
        boWarHreoRun = false;
        boGMRunAll = false;
        boSafeAreaLimited = false;
        boSafeAreaDisNpcRun = false;
        boSafeAreaDisShopStallHumRun = false;
        boSafeAreaDisOffLineHumRun = false;
        boWarDisTeleport = false;
        dwTryDealTime = 3000;
        dwDealOKTime = 1000;
        boCanNotGetBackDeal = true;
        boDisableDeal = false;
        boControlDropItem = false;
        boInSafeDisableDrop = false;
        nCanDropGold = 1000;
        nCanDropPrice = 500;
        nSafeZoneSize = 10;
        boHintSafeZone = false;
        dwHintSafeZoneY = 40;
        btHintSafeZoneFColor = 250;
        btHintSafeZoneBColor = 0;
        btHintSafeZoneFSize = 9;
        nStartPointSize = 3;
        nGroupMembersMax = 10;
        sRedHomeMap = "3";
        nRedHomeX = 845;
        nRedHomeY = 674;
        sRedDieHomeMap = "3";
        nRedDieHomeX = 839;
        nRedDieHomeY = 668;
        sHomeMap = "0";
        nHomeX = 289;
        nHomeY = 618;
        dwDecPkPointTime = 2 * 60 * 1000;
        nDecPkPointCount = 1;
        dwPKFlagTime = 60 * 1000;
        nKillHumanAddPKPoint = 100;
        nKillHumanDecLuckPoint = 500;
        nDummyAddPKPoint = 100;
        dwKillHeroAddPKPoint = 100;
        boKillHumanWinLevel = false;
        boKilledLostLevel = false;
        boKillHumanWinExp = false;
        boKilledLostExp = false;
        nKillHumanWinLevel = 1;
        nKilledLostLevel = 1;
        nKillHumanWinExp = 100000;
        nKillHumanLostExp = 100000;
        nHumanLevelDiffer = 10;
        boPKLevelProtect = false;
        nPKProtectLevel = 10;
        nRedPKProtectLevel = 10;
        dwKillHumanWeaponUnlockRate = 5;
        boHeroKillHumanNotWeaponUnlock = false;
        nStartPermission = 0;
        nHumanMaxGold = 100000000;
        nHumanTryModeMaxGold = 100000;
        nTryModeLevel = 7;
        boTryModeUseStorage = false;
        nGuildMemberMaxLimit = 260;
        nGuildNameLen = 30;
        nGuildRankNameLen = 18;
        dwHumChgMapOrLoginProtectTime = 1000;
        boOffLineShop = false;
        boOffLineHero = false;
        boOffLineSlave = false;
        boSellItemToNpcShopNoCalcAddProperty = false;
        boShowNewValueFromBuyNpcItem = false;
        dwTryChallengeTime = 3000;
        dwChallengeOKTime = 1000;
        boCanNotGetBackChallenge = false;
        boDisableChallenge = false;
        dwChallengeTime = 1 * 1000 * 60;
        btChallengeGoldIndex = 0;
        boNationGroupCheck = false;
        boNationGuildCheck = false;
        nNationSayLevel = 1;
        dwDecLightItemDrugTime = 500;
    }
}
