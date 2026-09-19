namespace GXX.M2Server.Engine;

/// <summary>
/// 批次J31：FunctionConfig.pas HeroOption 英雄选项页 / Other 其他页 / Other3 页依赖 g_Config 字段
/// （M2Share.pas typed-constant 初值 1:1）。BonusAbilof 页复用 ServerValue.cs 已有 BonusAbilofWarr/
/// Wizard/Taos（TBonusAbil）结构。
/// </summary>
public static partial class M2Config
{
    // ---- HeroOption 英雄选项页 ----
    public static bool boHeroGetAllExp = true;
    public static bool boHumanGetAllExp = false; // 人物获取全部经验
    public static int nHeroKillMonExpRate = 50;
    public static int nHeroNotKillMonExpRate = 50;
    public static bool boAllowCopySelf = false;
    public static int nLimitExpLevel = 1000;
    public static int nLimitExpValue = 1;
    public static uint dwHeroWarrorAttackTime = 700;
    public static uint dwHeroWizardAttackTime = 1200;
    public static uint dwHeroTaoistAttackTime = 1200;
    public static uint dwHeroWarrorWalkTime = 500;
    public static uint dwHeroWizardWalkTime = 600;
    public static uint dwHeroTaoistWalkTime = 600;
    public static uint dwHeroAvoidTime = 1300;
    public static bool boHeroHitCmp = false;
    public static int nWarrCmpInvTime = 100;
    public static int nRecallHeroTime = 60;
    public static int nRecallDeputyHeroTime = 60 * 5;
    public static int nClearHeroGhostTick = 3000;
    public static byte btHeroNameColor = 147;
    public static bool boHeroShowMasterName = true;
    public static int nHeroNeedMagicItem = 2; // 英雄需要毒符
    public static bool boHeroPickUpItem = false;
    public static bool boWarrorAttack = false;

    // ---- Other 其他页 ----
    public static bool boDeleteItemDuraZero = false;
    public static string sMysteriousManName = "神秘人";
    public static int nDamageItemDuraRate = 100;
    public static bool boDuraChangeLight = false;
    public static bool boPoisonWeaponCanMagicAttack = false;
    public static bool boPoisonWeaponCanHitAllTarget = false;
    public static int nQueryBagItemsTime = 3;
    public static bool boShowRefreshBagMsg = false;

    public static bool boLuckUseNewAlgorism = false;
    public static bool boHongMoSuiteWithPower = false;
    public static bool boGroupReCallNotInSafeZone = false;
    public static bool boNewHumanAttatckMode_HAM_PEACE = false;
    public static bool boAutoGroupMaster = false;
    public static bool boGuardNotAttackPlayMoster = false;
    public static bool boWarNoDropUseItem = false;
    public static int nHongMoSuiteRateChange = 50;
    public static bool boShowMysteriousMan = true;
    public static int nLimitScriptGotoCount = 20;
    public static bool boM2CacheRankData = false;
    public static bool boGroupUseOldMode = false;

    // ---- Other3 页 ----
    public static uint dwRevivalTime = 60 * 1000; // 复活间隔时间
    public static bool boRevivalTouch = false;
    public static bool boSaveRevivalTime = true;
    public static bool boFBExitCreaterOffline = false;
    public static bool boFBDisableDelay30s = false;
    public static bool boJewelryCalcBasicAbilitys = false;
    public static bool boJewelryCalcGroupAbilitys = false;
    public static bool boJewelryDecDura = false;
    public static string sJewelryBoxHint = "首饰盒";
    public static bool boOpenNewGuildTemp = false;
    public static bool boNoShowNewGuildHumanCount = false;
    public static bool boCloseNPCNoItemMsg = false;
    public static bool boChangeUseItemNameByPlayName = true;
    public static string sChangeUseItemName = "";
    public static int nStarBaseNum = 10;
    public static int nStarLineMaxCount = 30;
    public static bool boDisableDuFuTakeArmRingL = false;
    public static bool boRecordBeadExp = false;
    public static bool boDisableMoveParalysisHuman = false; // 人物麻痹状态禁止传送

    /// <summary>typed-constant 初值复位（英雄选项/其他/Other3 页字段，测试隔离用）。</summary>
    public static void ResetFunctionFinalDefaults()
    {
        boHeroGetAllExp = true;
        boHumanGetAllExp = false;
        nHeroKillMonExpRate = 50;
        nHeroNotKillMonExpRate = 50;
        boAllowCopySelf = false;
        nLimitExpLevel = 1000;
        nLimitExpValue = 1;
        dwHeroWarrorAttackTime = 700;
        dwHeroWizardAttackTime = 1200;
        dwHeroTaoistAttackTime = 1200;
        dwHeroWarrorWalkTime = 500;
        dwHeroWizardWalkTime = 600;
        dwHeroTaoistWalkTime = 600;
        dwHeroAvoidTime = 1300;
        boHeroHitCmp = false;
        nWarrCmpInvTime = 100;
        nRecallHeroTime = 60;
        nRecallDeputyHeroTime = 60 * 5;
        nClearHeroGhostTick = 3000;
        btHeroNameColor = 147;
        boHeroShowMasterName = true;
        nHeroNeedMagicItem = 2;
        boHeroPickUpItem = false;
        boWarrorAttack = false;
        boDeleteItemDuraZero = false;
        sMysteriousManName = "神秘人";
        nDamageItemDuraRate = 100;
        boDuraChangeLight = false;
        boPoisonWeaponCanMagicAttack = false;
        boPoisonWeaponCanHitAllTarget = false;
        nQueryBagItemsTime = 3;
        boShowRefreshBagMsg = false;

        boLuckUseNewAlgorism = false;
        boHongMoSuiteWithPower = false;
        boGroupReCallNotInSafeZone = false;
        boNewHumanAttatckMode_HAM_PEACE = false;
        boAutoGroupMaster = false;
        boGuardNotAttackPlayMoster = false;
        boWarNoDropUseItem = false;
        nHongMoSuiteRateChange = 50;
        boShowMysteriousMan = true;
        nLimitScriptGotoCount = 20;
        boM2CacheRankData = false;
        boGroupUseOldMode = false;
        dwRevivalTime = 60 * 1000;
        boRevivalTouch = false;
        boSaveRevivalTime = true;
        boFBExitCreaterOffline = false;
        boFBDisableDelay30s = false;
        boJewelryCalcBasicAbilitys = false;
        boJewelryCalcGroupAbilitys = false;
        boJewelryDecDura = false;
        sJewelryBoxHint = "首饰盒";
        boOpenNewGuildTemp = false;
        boNoShowNewGuildHumanCount = false;
        boCloseNPCNoItemMsg = false;
        boChangeUseItemNameByPlayName = true;
        sChangeUseItemName = "";
        nStarBaseNum = 10;
        nStarLineMaxCount = 30;
        boDisableDuFuTakeArmRingL = false;
        boRecordBeadExp = false;
        boDisableMoveParalysisHuman = false;
    }
}
