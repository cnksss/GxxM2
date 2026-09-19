namespace GXX.M2Server.Engine;

/// <summary>
/// 批次J22：GameConfig.pas 依赖 g_Config 游戏速度页字段（M2Share.pas typed-constant 初值 1:1）。
/// GameSpeed 页覆盖：速度控制开关、六间隔、六消息数、超速踢下线、弯腰控制、
/// 动作更新下发、检测动作次数组。
/// </summary>
public static partial class M2Config
{
    public static bool boSpeedControl = true;

    public static uint dwHitIntervalTime = 700;       // 攻击间隔
    public static uint dwMagicHitIntervalTime = 450;  // 魔法间隔
    public static uint dwRunIntervalTime = 400;       // 跑步间隔
    public static uint dwWalkIntervalTime = 400;      // 走路间隔
    public static uint dwTurnIntervalTime = 100;      // 换方向间隔
    public static uint dwDigUpIntervalTime = 100;     // Delphi 原文 dwDigUPIntervalTime

    public static int nMaxHitMsgCount = 1;
    public static int nMaxSpellMsgCount = 1;
    public static int nMaxRunMsgCount = 1;
    public static int nMaxWalkMsgCount = 1;
    public static int nMaxTurnMsgCount = 1;
    public static int nMaxSitDonwMsgCount = 1;
    public static int nMaxDigUpMsgCount = 1;

    public static bool boKickOverSpeed = false;
    public static byte btSpeedControlMode = 0;
    public static int nOverSpeedKickCount = 4;
    public static uint dwDropOverSpeed = 10;

    public static bool boDisableStruck = false;       // 不显示人物弯腰动作
    public static bool boDisableSelfStruck = true;    // 自己不显示人物弯腰动作
    public static bool boMagicshieldStruck = false;
    public static uint dwStruckTime = 100;            // 人物弯腰停留时间

    public static bool boSpellSendUpdateMsg = false;
    public static bool boActionSendActionMsg = false;

    public static bool boSendUpdateMsg = false;
    public static int nMaxHitDeliveryTime = 100;
    public static int nMaxMagicHitDeliveryTime = 100;
    public static int nMaxRunDeliveryTime = 100;
    public static int nMaxWalkDeliveryTime = 100;
    public static int nMaxTurnDeliveryTime = 100;
    public static int nMaxDigUpDeliveryTime = 100;

    public static bool boHorseRun3Grid = false;       // 骑马一步三格

    // 检测动作次数组
    public static bool boCheckActionCount = false;
    public static uint dwHitCountIntervalTime = 800;        // 攻击次数检测时间
    public static uint dwMagicHitCountIntervalTime = 1500;  // 魔法次数检测时间
    public static uint dwMoveCountIntervalTime = 800;       // 移动次数检测时间
    public static uint nCanHitCount = 3;
    public static uint nCanMagicHitCount = 3;
    public static uint nCanMoveCount = 3;
    public static uint nCheckHitCount = 4;
    public static uint nCheckMagicHitCount = 3;
    public static uint nCheckMoveCount = 4;

    /// <summary>typed-constant 初值复位（GameSpeed 页字段，测试隔离用）。</summary>
    public static void ResetGameSpeedDefaults()
    {
        boSpeedControl = true;
        dwHitIntervalTime = 700;
        dwMagicHitIntervalTime = 450;
        dwRunIntervalTime = 400;
        dwWalkIntervalTime = 400;
        dwTurnIntervalTime = 100;
        dwDigUpIntervalTime = 100;
        nMaxHitMsgCount = 1;
        nMaxSpellMsgCount = 1;
        nMaxRunMsgCount = 1;
        nMaxWalkMsgCount = 1;
        nMaxTurnMsgCount = 1;
        nMaxSitDonwMsgCount = 1;
        nMaxDigUpMsgCount = 1;
        boKickOverSpeed = false;
        btSpeedControlMode = 0;
        nOverSpeedKickCount = 4;
        dwDropOverSpeed = 10;
        boDisableStruck = false;
        boDisableSelfStruck = true;
        boMagicshieldStruck = false;
        dwStruckTime = 100;
        boSpellSendUpdateMsg = false;
        boActionSendActionMsg = false;
        boSendUpdateMsg = false;
        nMaxHitDeliveryTime = 100;
        nMaxMagicHitDeliveryTime = 100;
        nMaxRunDeliveryTime = 100;
        nMaxWalkDeliveryTime = 100;
        nMaxTurnDeliveryTime = 100;
        nMaxDigUpDeliveryTime = 100;
        boHorseRun3Grid = false;
        boCheckActionCount = false;
        dwHitCountIntervalTime = 800;
        dwMagicHitCountIntervalTime = 1500;
        dwMoveCountIntervalTime = 800;
        nCanHitCount = 3;
        nCanMagicHitCount = 3;
        nCanMoveCount = 3;
        nCheckHitCount = 4;
        nCheckMagicHitCount = 3;
        nCheckMoveCount = 4;
    }
}

/// <summary>
/// UserEngine 接缝（GameConfig 保存路径 1:1：SendServerConfig 按 boSendServerConfig 门控、
/// OptionSave3 固定调用 SendMapCanRun；真实网关下发随 UserEngine 批次接入，当前计数供测试）。
/// </summary>
public static class GameConfigState
{
    public static int SendServerConfigCalls;
    public static int SendMapCanRunCalls;

    public static void SendServerConfig() => SendServerConfigCalls++;
    public static void SendMapCanRun() => SendMapCanRunCalls++;
}
