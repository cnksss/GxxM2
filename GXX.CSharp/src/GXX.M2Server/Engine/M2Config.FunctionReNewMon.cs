namespace GXX.M2Server.Engine;

/// <summary>
/// 批次J28：FunctionConfig.pas ReNewLevel 转生页与 MonUpgrade 宝宝升级页依赖 g_Config 字段
/// （M2Share.pas typed-constant 初值 1:1）。
/// </summary>
public static partial class M2Config
{
    // ---- ReNewLevel 转生页 ----
    /// <summary>转生名字颜色表（Delphi array[0..9] of Byte）。</summary>
    public static byte[] ReNewNameColor = { 0xFF, 0xFE, 0x93, 0x9A, 0xE5, 0xA8, 0xB4, 0xFC, 0xB4, 0xFC };
    public static uint dwReNewNameColorTime = 2000;
    public static bool boReNewChangeColor = true;
    public static bool boReNewLevelClearExp = true;

    // ---- MonUpgrade 宝宝升级页 ----
    public static int nMonUpLvNeedKillBase = 100;
    public static int nMonUpLvRate = 16;
    /// <summary>宝宝升级所需杀怪数（Delphi array[0..SLAVEMAXLEVEL-2]，SLAVEMAXLEVEL=9）。</summary>
    public static int[] MonUpLvNeedKillCount = { 0, 0, 50, 100, 200, 300, 600, 1200 };
    /// <summary>宝宝各等级名字颜色（Delphi array[0..SLAVEMAXLEVEL] of Byte）。</summary>
    public static byte[] SlaveColor = { 0xFF, 0xFF, 0xFE, 0x93, 0x9A, 0xE5, 0xA8, 0xB4, 0xFC, 249 };
    public static bool boMasterDieMutiny = false;
    public static int nMasterDieMutinyRate = 5;
    public static int nMasterDieMutinyPower = 10;
    public static int nMasterDieMutinySpeed = 5;
    // boBBMonAutoChangeColor/dwBBMonAutoChangeColorTime 同名字段见下（dwBBMonAutoChangeColorTime 为本批新增）
    public static uint dwBBMonAutoChangeColorTime = 3000;
    public static int nSlavePowerRate = 100;
    public static bool boMasterRoyaltyDie = false;
    public static bool boMasterRoyaltyFullHP = false;
    public static bool boSlaveRelaxCanStruck = false;
    public static bool boSlaveNotAttackHuman = false;
    public static bool boSlaveNotAttackHero = false;
    public static bool boSlaveLockTarget = false;
    public static bool boSlaveNoLockHuman = false;
    public static int nSlave9HP = 60;
    public static int nSlave9AC = 10;
    public static int nSlave9MAC = 10;
    public static int nSlave9DC = 10;
    public static int nSlave9MoveSpeed = 0;

    /// <summary>typed-constant 初值复位（转生/宝宝升级页字段，测试隔离用）。</summary>
    public static void ResetFunctionReNewMonDefaults()
    {
        ReNewNameColor = new byte[] { 0xFF, 0xFE, 0x93, 0x9A, 0xE5, 0xA8, 0xB4, 0xFC, 0xB4, 0xFC };
        dwReNewNameColorTime = 2000;
        boReNewChangeColor = true;
        boReNewLevelClearExp = true;
        nMonUpLvNeedKillBase = 100;
        nMonUpLvRate = 16;
        MonUpLvNeedKillCount = new int[] { 0, 0, 50, 100, 200, 300, 600, 1200 };
        SlaveColor = new byte[] { 0xFF, 0xFF, 0xFE, 0x93, 0x9A, 0xE5, 0xA8, 0xB4, 0xFC, 249 };
        boMasterDieMutiny = false;
        nMasterDieMutinyRate = 5;
        nMasterDieMutinyPower = 10;
        nMasterDieMutinySpeed = 5;
        dwBBMonAutoChangeColorTime = 3000;
        nSlavePowerRate = 100;
        boMasterRoyaltyDie = false;
        boMasterRoyaltyFullHP = false;
        boSlaveRelaxCanStruck = false;
        boSlaveNotAttackHuman = false;
        boSlaveNotAttackHero = false;
        boSlaveLockTarget = false;
        boSlaveNoLockHuman = false;
        nSlave9HP = 60;
        nSlave9AC = 10;
        nSlave9MAC = 10;
        nSlave9DC = 10;
        nSlave9MoveSpeed = 0;
    }
}
