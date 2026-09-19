namespace GXX.M2Server.Engine;

/// <summary>
/// 批次J26：FunctionConfig.pas Skill 技能页头部与 UpgradeWeapon 升级武器页依赖 g_Config 字段
/// （M2Share.pas typed-constant 初值 1:1）。
/// 召唤宝宝名字/数量/数组（sBoneFamm/DogzArray 等）已在批次G/H 迁入 M2Config（MagicEx.cs），此处不重复。
/// </summary>
public static partial class M2Config
{
    // ---- 升级武器页 ----
    public static int nUpgradeWeaponDCRate = 100;
    public static int nUpgradeWeaponDCTwoPointRate = 30;
    public static int nUpgradeWeaponDCThreePointRate = 200;
    public static int nUpgradeWeaponMCRate = 100;
    public static int nUpgradeWeaponMCTwoPointRate = 30;
    public static int nUpgradeWeaponMCThreePointRate = 200;
    public static int nUpgradeWeaponSCRate = 100;
    public static int nUpgradeWeaponSCTwoPointRate = 30;
    public static int nUpgradeWeaponSCThreePointRate = 200;
    public static int nUpgradeWeaponMaxPoint = 20;
    public static int nUpgradeWeaponPrice = 10000;
    public static int nClearExpireUpgradeWeaponDays = 8;
    public static uint dwUPgradeWeaponGetBackTime = 60 * 60 * 1000;
    public static bool boWeaponUpgradeFailNotDelete = false;

    // ---- Skill 技能页头部 ----
    public static bool boLimitSwordLong = false;
    public static int nSwordLongPowerRate = 100;
    public static int nSkillYedoPowerRate = 100;
    public static int nSnowWindRange = 1;
    public static int nSnowWindPowerRate = 100;
    public static int nSnowwindWaitTime = 1;
    public static int nFireBoomRage = 1;
    public static int nFireBoomRagePowerRate = 100;
    public static int nElecBlizzardRange = 2;
    public static int nElecBlizzardPowerRate = 100;
    public static bool boShowMsgMagicRangeExceed = true;
    public static int nAmyOunsulPoint = 10;
    public static int nAmyOunsulTimeRate = 100;
    public static int nAmyOunsulMaxTime = 60 * 5;
    public static bool boShowYouPoisoned = true;
    public static uint dwPosionDecHealthTime = 2500;
    public static int nPosionDamagarmor = 12;
    public static bool boEnabledPosionDecMAC = false;
    public static int nPosionDecMACRate = 12;
    public static bool boPosionStopIncHealth = false;
    public static int nMagTurnUndeadLevel = 50;
    public static bool boMagTurnUndeadSameLevel = false;
    public static int nMagTammingLevel = 50;
    public static int nMagTammingTargetLevel = 10;
    public static int nMagTammingHPRate = 100;
    public static int nMagTammingCount = 5;
    public static int nMasterRoyaltyTime = 60;
    public static int nMabMabeHitRandRate = 100;
    public static int nMabMabeHitMinLvLimit = 10;
    public static int nMabMabeHitSucessRate = 21;
    public static int nMabMabeHitMabeTimeRate = 20;
    public static int nMaxMabMabeHitMabeTime = 0;
    public static int nOrdinarySkill31Rate = 8;
    public static bool boDisableInSafeZoneFireCross = false;
    public static bool boSkill41MbAttackPlayObject = false;
    public static int nMagDelayTimeDoubly = 100;
    public static int nNearAttackPowerRate = 100;

    // ---- 召唤宝宝（名字/数量/数组已由批次G/H 迁入 MagicEx.cs；月灵名与数量为此处补齐） ----
    public static string sMonthSpirit = "月灵";
    public static int nMonthSpiritCount = 1;
    public static int nMonthSpiritAttackRange = 1;
    public static (int nHumLevel, string sMonName, int nLevel, int nCount)[] MonthSpiritArray =
        new (int, string, int, int)[10];

    /// <summary>typed-constant 初值复位（Skill 头部/升级武器/召唤宝宝字段，测试隔离用）。</summary>
    public static void ResetFunctionSkillDefaults()
    {
        nUpgradeWeaponDCRate = 100;
        nUpgradeWeaponDCTwoPointRate = 30;
        nUpgradeWeaponDCThreePointRate = 200;
        nUpgradeWeaponMCRate = 100;
        nUpgradeWeaponMCTwoPointRate = 30;
        nUpgradeWeaponMCThreePointRate = 200;
        nUpgradeWeaponSCRate = 100;
        nUpgradeWeaponSCTwoPointRate = 30;
        nUpgradeWeaponSCThreePointRate = 200;
        nUpgradeWeaponMaxPoint = 20;
        nUpgradeWeaponPrice = 10000;
        nClearExpireUpgradeWeaponDays = 8;
        dwUPgradeWeaponGetBackTime = 60 * 60 * 1000;
        boWeaponUpgradeFailNotDelete = false;
        boLimitSwordLong = false;
        nSwordLongPowerRate = 100;
        nSkillYedoPowerRate = 100;
        nSnowWindRange = 1;
        nSnowWindPowerRate = 100;
        nSnowwindWaitTime = 1;
        nFireBoomRage = 1;
        nFireBoomRagePowerRate = 100;
        nElecBlizzardRange = 2;
        nElecBlizzardPowerRate = 100;
        boViewRangeCanMagicAttack = false;
        boShowMsgMagicRangeExceed = true;
        nAmyOunsulPoint = 10;
        nAmyOunsulTimeRate = 100;
        nAmyOunsulMaxTime = 60 * 5;
        boShowYouPoisoned = true;
        dwPosionDecHealthTime = 2500;
        nPosionDamagarmor = 12;
        boEnabledPosionDecMAC = false;
        nPosionDecMACRate = 12;
        boPosionStopIncHealth = false;
        nMagTurnUndeadLevel = 50;
        boMagTurnUndeadSameLevel = false;
        nMagTammingLevel = 50;
        nMagTammingTargetLevel = 10;
        nMagTammingHPRate = 100;
        nMagTammingCount = 5;
        nMasterRoyaltyTime = 60;
        nMabMabeHitRandRate = 100;
        nMabMabeHitMinLvLimit = 10;
        nMabMabeHitSucessRate = 21;
        nMabMabeHitMabeTimeRate = 20;
        nMaxMabMabeHitMabeTime = 0;
        nOrdinarySkill31Rate = 8;
        boDisableInSafeZoneFireCross = false;
        boSkill41MbAttackPlayObject = false;
        nMagDelayTimeDoubly = 100;
        nNearAttackPowerRate = 100;
        sMonthSpirit = "月灵";
        nMonthSpiritCount = 1;
        nMonthSpiritAttackRange = 1;
        MonthSpiritArray = new (int, string, int, int)[10];
    }
}
