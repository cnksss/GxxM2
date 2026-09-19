namespace GXX.M2Server.Engine;

/// <summary>
/// 批次J27：FunctionConfig.pas Master 拜师/MakeMine 挖矿/WinLottery 赌博页依赖 g_Config 字段
/// （M2Share.pas typed-constant 初值 1:1）。
/// </summary>
public static partial class M2Config
{
    // ---- Master 拜师页 ----
    public static int nMasterOKLevel = 500;
    public static int nMasterOKCreditPoint = 0;
    public static int nMasterOKBonusPoint = 0;
    public static int nMasterCount = 5; // 徒弟数量

    // ---- MakeMine 挖矿页 ----
    public static int nMakeMineHitRate = 4; // 挖矿命中率
    public static int nMakeMineRate = 12;   // 挖矿率
    public static int nStoneTypeRate = 120;
    public static int nStoneTypeRateMin = 56;
    public static int nGoldStoneMin = 1;
    public static int nGoldStoneMax = 2;
    public static int nSilverStoneMin = 3;
    public static int nSilverStoneMax = 20;
    public static int nSteelStoneMin = 21;
    public static int nSteelStoneMax = 45;
    public static int nBlackStoneMin = 46;
    public static int nBlackStoneMax = 56;
    public static int nStoneMinDura = 3000;
    public static int nStoneGeneralDuraRate = 13000;
    public static int nStoneAddDuraRate = 20;
    public static int nStoneAddDuraMax = 10000;

    // ---- WinLottery 赌博页 ----
    public static int nWinLottery1Gold = 1000000;
    public static int nWinLottery2Gold = 200000;
    public static int nWinLottery3Gold = 100000;
    public static int nWinLottery4Gold = 10000;
    public static int nWinLottery5Gold = 1000;
    public static int nWinLottery6Gold = 500;
    public static int nWinLottery1Min = 16180;
    public static int nWinLottery1Max = 16185;
    public static int nWinLottery2Min = 16170;
    public static int nWinLottery2Max = 16179;
    public static int nWinLottery3Min = 16150;
    public static int nWinLottery3Max = 16169;
    public static int nWinLottery4Min = 16000;
    public static int nWinLottery4Max = 16149;
    public static int nWinLottery5Min = 14000;
    public static int nWinLottery5Max = 15999;
    public static int nWinLottery6Min = 1;
    public static int nWinLottery6Max = 4999;
    public static int nWinLotteryRate = 30000;

    /// <summary>typed-constant 初值复位（Master/MakeMine/WinLottery 页字段，测试隔离用）。</summary>
    public static void ResetFunctionMineDefaults()
    {
        nMasterOKLevel = 500;
        nMasterOKCreditPoint = 0;
        nMasterOKBonusPoint = 0;
        nMasterCount = 5;
        nMakeMineHitRate = 4;
        nMakeMineRate = 12;
        nStoneTypeRate = 120;
        nStoneTypeRateMin = 56;
        nGoldStoneMin = 1;
        nGoldStoneMax = 2;
        nSilverStoneMin = 3;
        nSilverStoneMax = 20;
        nSteelStoneMin = 21;
        nSteelStoneMax = 45;
        nBlackStoneMin = 46;
        nBlackStoneMax = 56;
        nStoneMinDura = 3000;
        nStoneGeneralDuraRate = 13000;
        nStoneAddDuraRate = 20;
        nStoneAddDuraMax = 10000;
        nWinLottery1Gold = 1000000;
        nWinLottery2Gold = 200000;
        nWinLottery3Gold = 100000;
        nWinLottery4Gold = 10000;
        nWinLottery5Gold = 1000;
        nWinLottery6Gold = 500;
        nWinLottery1Min = 16180;
        nWinLottery1Max = 16185;
        nWinLottery2Min = 16170;
        nWinLottery2Max = 16179;
        nWinLottery3Min = 16150;
        nWinLottery3Max = 16169;
        nWinLottery4Min = 16000;
        nWinLottery4Max = 16149;
        nWinLottery5Min = 14000;
        nWinLottery5Max = 15999;
        nWinLottery6Min = 1;
        nWinLottery6Max = 4999;
        nWinLotteryRate = 30000;
    }
}
