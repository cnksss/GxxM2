namespace GXX.M2Server.Engine;

/// <summary>
/// 批次J22：FunctionConfig.pas 依赖 g_Config 字段（M2Share.pas typed-constant 初值 1:1）。
/// 第一片：密码保护系统组 + 常规页（饥饿系统/名称颜色/矿石恢复/叠放/地图事件）。
/// </summary>
public static partial class M2Config
{
    // ---- 密码保护系统 ----
    public static bool boPasswordLockSystem = false;      // 是否启用密码保护系统
    public static bool boLockDealAction = false;          // 是否锁定交易操作
    public static bool boLockDropAction = false;          // 是否锁定扔物品操作
    public static bool boLockGetBackItemAction = false;   // 是否锁定取仓库操作
    public static bool boLockHumanLogin = false;          // 登录即锁定
    public static bool boLockWalkAction = false;          // 是否锁定走操作
    public static bool boLockRunAction = false;           // 是否锁定跑操作
    public static bool boLockHitAction = false;           // 是否锁定攻击操作
    public static bool boLockSpellAction = false;         // 是否锁定魔法操作
    public static bool boLockSendMsgAction = false;       // 是否锁定发信息操作
    public static bool boLockUserItemAction = false;      // 是否锁定使用物品操作
    public static bool boLockInObModeAction = false;      // 锁定时进入隐身状态
    public static bool boLockChallenge = false;           // 禁止挑战
    public static bool boLockSummonHero = false;          // 禁止召唤英雄
    public static bool boLockShop = false;                // 禁止商铺
    public static bool boLockStall = false;               // 禁止摆摊
    public static int nPasswordErrorCountLock = 3;        // 输入密码错误超过指定次数则锁定

    // ---- 饥饿系统 ----
    public static bool boHungerSystem = false;
    public static bool boHungerDecHP = false;
    public static bool boHungerDecPower = false;

    // ---- 名称颜色 ----
    public static byte btPKFlagNameColor = 0x2F;
    public static byte btPKLevel1NameColor = 0xFB;
    public static byte btPKLevel2NameColor = 0xF9;
    public static byte btAllyAndGuildNameColor = 0xB4;
    public static byte btWarGuildNameColor = 0x45;
    public static byte btInFreePKAreaNameColor = 0xDD;
    public static byte btMerchantNameColor = 250;
    public static byte btMerchant273NameColor = 244;
    public static byte btGuardNameColor = 250;

    // ---- 矿石/饰品恢复 ----
    public static int nHPRockRate = 10;
    public static int nHPRockType = 1;
    public static int nHPRockTime = 500;
    public static int nHPRockAddType = 1;
    public static int nHPRockAddValue = 10;
    public static int nHPRockDecValue = 1;
    public static int nMPRockRate = 10;
    public static int nMPRockType = 1;
    public static int nMPRockTime = 500;
    public static int nMPRockAddType = 1;
    public static int nMPRockAddValue = 10;
    public static int nMPRockDecValue = 1;
    public static int nHMPRockRate = 10;
    public static int nHMPRockType = 1;
    public static int nHMPRockTime = 500;
    public static int nHMPRockAddType = 1;
    public static int nHMPRockAddValue = 10;
    public static int nHMPRockDecValue = 1;
    public static bool boHMPUse2Times = true;
    public static int nHMPDivDura = 10;

    // ---- 掉落/地图事件 ----
    public static bool boDropOverLapItem = true;
    public static bool boOpenMapEvent = false;

    /// <summary>typed-constant 初值复位（FunctionConfig 第一片字段，测试隔离用）。</summary>
    public static void ResetFunctionDefaults()
    {
        boPasswordLockSystem = false;
        boLockDealAction = false;
        boLockDropAction = false;
        boLockGetBackItemAction = false;
        boLockHumanLogin = false;
        boLockWalkAction = false;
        boLockRunAction = false;
        boLockHitAction = false;
        boLockSpellAction = false;
        boLockSendMsgAction = false;
        boLockUserItemAction = false;
        boLockInObModeAction = false;
        boLockChallenge = false;
        boLockSummonHero = false;
        boLockShop = false;
        boLockStall = false;
        nPasswordErrorCountLock = 3;
        boHungerSystem = false;
        boHungerDecHP = false;
        boHungerDecPower = false;
        btPKFlagNameColor = 0x2F;
        btPKLevel1NameColor = 0xFB;
        btPKLevel2NameColor = 0xF9;
        btAllyAndGuildNameColor = 0xB4;
        btWarGuildNameColor = 0x45;
        btInFreePKAreaNameColor = 0xDD;
        btMerchantNameColor = 250;
        btMerchant273NameColor = 244;
        btGuardNameColor = 250;
        nHPRockRate = 10;
        nHPRockType = 1;
        nHPRockTime = 500;
        nHPRockAddType = 1;
        nHPRockAddValue = 10;
        nHPRockDecValue = 1;
        nMPRockRate = 10;
        nMPRockType = 1;
        nMPRockTime = 500;
        nMPRockAddType = 1;
        nMPRockAddValue = 10;
        nMPRockDecValue = 1;
        nHMPRockRate = 10;
        nHMPRockType = 1;
        nHMPRockTime = 500;
        nHMPRockAddType = 1;
        nHMPRockAddValue = 10;
        nHMPRockDecValue = 1;
        boHMPUse2Times = true;
        nHMPDivDura = 10;
        boDropOverLapItem = true;
        boOpenMapEvent = false;
    }
}
