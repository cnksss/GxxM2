namespace GXX.M2Server.Engine;

/// <summary>
/// M2Share.pas g_Config 字段子集（批次J67：ConfigClient.pas 窗体依赖）。
/// typed constant 默认值 1:1（M2Share.pas g_Config 常量块 4119-6610）。
/// </summary>
public static partial class M2Config
{
    // ---- 界面类型与内挂开关 ----
    public static byte btConfigDlgType = 0;
    // boStartGameAuxiliary 已在 M2Config.GameDie.cs（批次沿用）
    public static bool boCanOpenGameConfigDlg = true;
    public static bool boNotCanUseClientConfig = false;
    public static bool boGreenHintNewStyle = false;

    // ---- 动作帧与间隔 ----
    public static uint dwMoveFrameTime = 600;
    public static uint dwHitFrameTime = 700;
    public static uint dwMagicHitFrameTime = 500;
    public static uint dwPluginPickupTime = 300;
    public static uint dwPluginMinEatItemTime = 500;
    public static uint dwIncSpeedDecInterval = 30;
    public static uint dwIncMoveSpeedDecInterval = 30;
    public static uint dwIncSpellSpeedDecInterval = 30;

    // ---- 界面按钮显隐 ----
    public static bool boActionLogButton = true;
    public static bool boMissionButton = true;
    public static bool boFriendButton = true;
    public static bool boControlHelpButton = true;
    public static bool boRankButton = true;
    public static bool boWhisperButton = true;
    public static bool boOpenShopButton = true;
    public static bool boUserShopButton = true;
    public static bool boWebButton = true;
    public static bool boOpenHeroButton = true;
    public static bool boChallengeButton = true;
    public static bool boShowGlory = true;
    public static bool boShowHorseButton = true;
    public static bool boShowDeputyHeroButton = true;
    public static bool boShowBagArrange = true;
    public static bool boKeyTabGetActor = true;
    public static bool boHideIconWithHideTitle = false;
    public static bool boShowExSkillIcon = false;
    /// <summary>g_ArrButtonConfigCRC（btnArrBtnSettingClick 落盘标记，J69）。</summary>
    public static uint g_ArrButtonConfigCRC;
    public static bool boShowMerchantDlgHelp = true;

    // ---- 血条/名字偏移 ----
    public static bool boShowMagicShieldHP = false;
    public static int nHumHPBarOffsetX, nHumHPBarOffsetY;
    public static int nNpcHPBarOffsetX, nNpcHPBarOffsetY;
    public static int nMonHPBarOffsetX, nMonHPBarOffsetY;
    public static int nHumNameOffsetX, nHumNameOffsetY;
    public static int nNpcNameOffsetX, nNpcNameOffsetY;
    public static int nMonNameOffsetX, nMonNameOffsetY;

    // ---- 血数字 ----
    public static bool boHealthNumberText = false;
    public static bool boBlastHitShowHealthNum = false;
    public static int nHealthNumberOffsetX, nHealthNumberOffsetY;
    public static int nHealthNumberMoveSpeed = 50;
    public static int nNewLeftGroupInfoOffsetX, nNewLeftGroupInfoOffsetY;
    public static bool boPoisoningHideHealthNum = false;
    public static bool boHPStoneHideHealthNum = false;
    public static bool boMPStoneHideHealthNum = false;
    public static bool boMagicSetDir = false;

    // ---- 悬浮提示窗 ----
    public static byte btHintWindowbackgroundColor = 0;
    public static byte btHintWindowbackgroundAlpha = 120;
    public static (byte Left, byte Top, byte Right, byte Bottom) HintWindowBorderWidth = (8, 8, 8, 8);
    public static bool boShowHintWindowFrame = true;
    public static bool boShowHintLines = true;
    public static string sShowHintFontName = "宋体";
    public static byte btShowHintNameFontSize = 10;
    public static byte btShowHintNameFontBold;
    public static byte btShowHintNameFontStroke;
    public static byte btShowHintOtherFontSize = 9;
    public static byte btShowHintOtherFontBold;
    public static byte btShowHintOtherFontStroke;

    // ---- 速度三轴（-10..+10） ----
    public static int nMoveSpeed;
    public static int nAttackSpeed;
    public static int nSpellSpeed;

    // ---- 内挂选项组 / 页签可见 / 悬浮样式 ----
    public static readonly bool[] ClientConfigs = new bool[90]
    {
        true, true, true, true, true, true, true, true, true, true,
        true, true, true, true, true, true, true, true, true, true,
        true, true, true, true, true, true, true, true, true, true,
        true, true, true, true, true, true, true, true, true, true,
        true, true, true, true, true, true, true, true, true, true,
        true, true, true, true, true, true, true, true, true, true,
        true, true, true, true, true, true, true, true, true, true,
        false, false, false, false, false, false, false, false, false, false,
        false, false, false, false, false, false, false, false, false, false,
    };
    public static readonly bool[] ClientConfigTabSheetVisibles = new bool[12]
    {
        true, true, true, true, true, true, true, true, true, true, true, true,
    };
    public static byte btSuspensionShowItem = 1;
    public static byte btBagFastItemCompareMode;

    // ---- 物品名/来源 ----
    public static bool boEscCloseNPC = false;
    public static bool boHintWithMouse = true;
    public static bool boNpcDlgHintWithMouse = false;
    public static bool boHelmetShowInBox = false;
    public static bool boShowItemForm = false;
    public static bool boShowItemSellPrice = false;
    public static bool boShowInsuranceInfo = false;
    public static byte btShowItemFormColor = 251;
    public static byte btShowItemSellPriceColor = 243;
    public static byte btShowInsuranceInfoColor = 255;
    public static readonly bool[] boShowItemFromFields = new bool[7] { true, true, true, true, true, true, true };

    // ---- 时装 ----
    public static bool boShowNormalFashion = false;
    public static bool boShowFashionHideShield = false;
    public static bool boShowFashionHideHats = false;
    public static bool boFashionJewelryOpen = false;

    // ---- 主页 / 内挂功能组 / 雾 ----
    public static string sHomePage = "http://www.gxxm2.com";
    public static readonly bool[] DBotFuncs = new bool[6] { true, true, true, true, true, true };
    public static bool boViewFog = false;

    // ---- 亮度（0 日出/1 白天/2 傍晚/3 黑夜 × 24 段） ----
    public static readonly int[] BrightConfig = new int[24]
    {
        3, 3, 3, 3, 0, 1, 1, 1, 1, 1, 1, 2, 3, 3, 3, 0, 1, 1, 1, 1, 1, 1, 1, 2,
    };

    // ---- 受击显数/保护 ----
    public static bool boMonStruckShowNumber = true;
    public static bool boHumStruckShowNumber = true;
    public static bool boCloseBookProtect = false;
    public static bool boCloseLogoutProtect = false;
    public static bool boBagRightkey = true;
    public static bool boGetExpMsgAddChatBoardMsg = true;

    // ---- 提示坐标/方向（本体+英雄 六组） ----
    public static bool boAddItemMsgXRightToLeft, boAddItemMsgYBottomToTop;
    public static bool boGetExpMsgXRightToLeft, boGetExpMsgYBottomToTop;
    public static bool boUpLevelMsgXRightToLeft, boUpLevelMsgYBottomToTop;
    public static bool boHeroAddItemMsgXRightToLeft, boHeroAddItemMsgYBottomToTop;
    public static bool boHeroGetExpMsgXRightToLeft, boHeroGetExpMsgYBottomToTop;
    public static bool boHeroUpLevelMsgXRightToLeft, boHeroUpLevelMsgYBottomToTop;

    // ---- 页签隐藏 ----
    public static bool boHideTabSheet2, boHeroHideTabSheet2;
    public static bool boHideTabSheet5, boHeroHideTabSheet5;
    public static bool boHideTabSheet7;

    // ---- 新属性显示（24） ----
    public static readonly bool[] NewAbilShowStateDlg = new bool[24];

    // ---- 提示颜色（本体+英雄 六组） ----
    public static byte btAddItemMsgFColor = 250, btAddItemMsgBColor = 2;
    public static byte btGetExpMsgFColor = 255, btGetExpMsgBColor = 249;
    public static byte btUpLevelMsgFColor = 2, btUpLevelMsgBColor = 0;
    public static int nAddItemMsgX = 30, nAddItemMsgY = 40;
    public static int nGetExpMsgX = 30, nGetExpMsgY = 40;
    public static int nUpLevelMsgX = 30, nUpLevelMsgY = 40;
    public static byte btHeroAddItemMsgFColor = 2, btHeroAddItemMsgBColor;
    public static byte btHeroGetExpMsgFColor = 249, btHeroGetExpMsgBColor = 255;
    public static byte btHeroUpLevelMsgFColor = 2, btHeroUpLevelMsgBColor;
    public static int nHeroAddItemMsgX = 30, nHeroAddItemMsgY = 40;
    public static int nHeroGetExpMsgX = 9, nHeroGetExpMsgY = 380;
    public static int nHeroUpLevelMsgX = 30, nHeroUpLevelMsgY = 40;

    // ---- 背包/英雄快捷键 ----
    public static bool boShowBagGameGoldSeparator = false;
    public static bool boShowBagGameInfo = true;
    public static bool boShowHeroShortKey = true;
    public static int nShowHeroShortKeyX, nShowHeroShortKeyY;

    // ---- 寻路/串窗 ----
    public static bool boUseFindPath = true;
    public static bool boUseOldSerialWindows = false;
    public static bool boMoveItemShowID = false;
    public static byte boStateWindowsType = 1;   // 0：1.85 1：合击
    public static int nTitleFileIndex = -1;

    // ---- 恢复节奏 ----
    public static int nPerHealth = 10;
    public static int nPerSpell = 10;
    public static int nIncHealthSpellTime = 700;
    public static int nHealthFillTime = 450;
    public static int nHealthFillTime_Human_Warrior = 350;
    public static int nHealthFillTime_Human_TaoistAndWizard = 350;
    public static int nHealthFillTime_Hero_Warrior = 350;
    public static int nHealthFillTime_Hero_TaoistAndWizard = 350;
    public static int nSpellFillTime = 800;
    public static int nSpellFillTime_Human_Warrior = 800;
    public static int nSpellFillTime_Human_TaoistAndWizard = 800;
    public static int nSpellFillTime_Hero_Warrior = 800;
    public static int nSpellFillTime_Hero_TaoistAndWizard = 800;
    public static int nHealthBaseNum = 75;
    public static int nSpellBaseNum = 18;
    public static int nHealthBaseNum_Human_Warrior = 75;
    public static int nHealthBaseNum_Human_TaoistAndWizard = 75;
    public static int nHealthBaseNum_Hero_Warrior = 75;
    public static int nHealthBaseNum_Hero_TaoistAndWizard = 75;
    public static int nSpellBaseNum_Human_Warrior = 18;
    public static int nSpellBaseNum_Human_TaoistAndWizard = 18;
    public static int nSpellBaseNum_Hero_Warrior = 18;
    public static int nSpellBaseNum_Hero_TaoistAndWizard = 18;
    public static uint dwUseItemIntervalTime = 500;
    public static uint dwUseAttackItemIntervalTime = 500;
    public static uint dwUseOrdinaryTime_Human_Warrior = 300;
    public static uint dwUseSpecialTime_Human_Warrior = 1000;
    public static uint dwUseOrdinaryTime_Human_TaoistAndWizard = 300;
    public static uint dwUseSpecialTime_Human_TaoistAndWizard = 1000;
    public static uint dwUseOrdinaryTime_Hero_Warrior = 300;
    public static uint dwUseSpecialTime_Hero_Warrior = 1000;
    public static uint dwUseOrdinaryTime_Hero_TaoistAndWizard = 300;
    public static uint dwUseSpecialTime_Hero_TaoistAndWizard = 1000;

    // ---- 小地图 ----
    public static bool boMinMapCloseRadar = false;
    public static byte btMinMapType;
    public static bool boMinMapUseFindPath = false;
    public static bool boLoginShowMinMap = false;
    public static uint dwMinMapFlagFlash = 300;
    public static byte btMinMapColorSelf = 255;
    public static byte btMinMapColorOther = 251;
    public static byte btMinMapColorNPC = 222;
    public static byte btMinMapColorGuard = 254;
    public static byte btMinMapColorMonster = 249;
    public static byte btMinMapColorHero = 252;
    public static byte btMinMapColorBoss;

    // ---- 其它显隐 ----
    public static bool boHideItemNameNum = false;
    public static bool boGemUpgrade = true;
    public static bool boShowGuildName = true;
    public static bool boShopGuiCanMove = false;
    public static bool boNPCGuiCanMove = false;

    // ---- 治愈 ----
    public static int nIncHealingLimite = 300;
    public static int nPerHealing = 5;
    public static int nPerHealingTime = 700;
    public static int nBigPerHealing = 5;
    public static int nBigPerHealingTime = 700;

    // ---- 掉落颜色 ----
    public static byte btThrowAwayItemColor = 254;

    // ---- 界面坐标/缩放 ----
    public static int nBetterItemX = 550, nBetterItemY = 350;
    public static int nSmallInfoX = 10, nSmallInfoY = 280;
    public static int nJoyStickX = 90, nJoyStickY = 100;
    public static int nJoyStickMaxX = 320, nJoyStickMaxY = 280;
    public static int nSkillCtrX = 35, nSkillCtrY = 60;
    public static byte btMapScale = 13;
    public static byte btGuiScale = 11;
    public static bool boShowMulitDlg = false;
    public static bool boShowBetterItem = false;
    public static byte btMultiViewRange = 5;

    /// <summary>g_ArrButtonConfig[7]（M2Share 7786：七组排列按钮配置，typed constant 全 0 缺省）。</summary>
    public static readonly ArrButtonGroupConfig[] g_ArrButtonConfig = new ArrButtonGroupConfig[7]
    {
        new(), new(), new(), new(), new(), new(), new(),
    };
}

/// <summary>TArrButtonGroupConfig（Grobal2.pas）：HorzAligment/VertAligment 取 TAlignment/TVerticalAlignment 序（0 左上）。</summary>
public struct ArrButtonGroupConfig
{
    public int HorzAligment;   // taLeftJustify=0 / taCenter=1 / taRightJustify=2
    public int VertAligment;   // taAlignTop=0 / taAlignCenter=1 / taAlignBottom=2
    public int OffsetX, OffsetY;
    public int NextOffsetX, NextOffsetY;
}
