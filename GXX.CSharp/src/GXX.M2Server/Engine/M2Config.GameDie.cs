namespace GXX.M2Server.Engine;

/// <summary>
/// 批次J25：GameConfig.pas MsgColor/HumanDie/CharStatus/DieDrop 页依赖 g_Config 字段
/// （M2Share.pas typed-constant 初值 1:1）。
/// </summary>
public static partial class M2Config
{
    // ---- MsgColor 页（21 色对 + NPC 标签四项；Delphi 十六进制初值 1:1） ----
    public static byte btHearMsgFColor = 0x00;
    public static byte btHearMsgBColor = 0xFF;
    public static byte btWhisperMsgFColor = 0xFC;
    public static byte btWhisperMsgBColor = 0xFF;
    public static byte btGMWhisperMsgFColor = 0xFF;
    public static byte btGMWhisperMsgBColor = 0x38;
    public static byte btSendWhisperMsgFColor = 0xB4;
    public static byte btSendWhisperMsgBColor = 0xFF;
    public static byte btRefreshGameGoldFColor = 0xDB;
    public static byte btRefreshGameGoldBColor = 0xFF;
    public static byte btShowWhisperFColor = 219;
    public static byte btShowWhisperBColor = 0xFF;
    public static byte btCloseWhisperFColor = 219;
    public static byte btCloseWhisperBColor = 0x00;
    public static byte btCryMsgFColor = 0x0;
    public static byte btCryMsgBColor = 0x97;
    public static byte btGreenMsgFColor = 0xDB;
    public static byte btGreenMsgBColor = 0xFF;
    public static byte btBlueMsgFColor = 0xFF;
    public static byte btBlueMsgBColor = 0xFC;
    public static byte btRedMsgFColor = 0xFF;
    public static byte btRedMsgBColor = 0x38;
    public static byte btGuildMsgFColor = 0xDB;
    public static byte btGuildMsgBColor = 0xFF;
    public static byte btGroupMsgFColor = 0xC4;
    public static byte btGroupMsgBColor = 0xFF;
    public static byte btCustMsgFColor = 0xFC;
    public static byte btCustMsgBColor = 0xFF;
    public static byte btNationMsgFColor = 0xDB;
    public static byte btNationMsgBColor = 0xFF;
    public static byte btUserSayMsgFColor = 0xFF;
    public static byte btUserSayMsgBColor = 0xFD;
    public static byte btTopUserSayMsgFColor = 0x97;
    public static byte btTopUserSayMsgBColor = 0xEA;
    public static byte btDropItemFColor = 0xFF;
    public static byte btDropItemBColor = 0xFD;
    public static bool boNPCLabelFontStroke = true;
    public static byte btNPCLabelNormalColor = 251;
    public static byte btNPCLabelMouseMoveColor = 253;
    public static byte btNPCLabelMouseDownColor = 58;

    // ---- HumanDie/DieDrop 页 ----
    public static bool boDieScatterBag = true;
    public static int nDieScatterBagRate = 3;
    public static bool boDieRedScatterBagAll = true;
    public static int nDieDropUseItemRate = 30;
    public static int nDieRedDropUseItemRate = 15;
    public static bool boDieDropGold = false;
    public static bool boKillByHumanDropUseItem = false;
    public static bool boKillByMonstDropUseItem = false;
    public static bool boKillByHumanDropJewelryBoxItem = false;
    public static bool boKillByMonstDropJewelryBoxItem = false;
    public static bool boKillByHumanDropGodBlessItem = false;
    public static bool boKillByMonstDropGodBlessItem = false;
    public static int nDropJewelryBoxItemRate = 30;
    public static int nDropGodBlessItemRate = 30;
    public static int nDropUseItemsMaxCount = 15;
    public static int nScatterBagItemsMinLevel = 1;
    public static bool boDropUseItem = false;
    public static int nDieRedDropUseItemOneRate = 20;
    /// <summary>死亡按槽位掉装备几率（Delphi array[0..High(THumanUseItems)]，30 槽全 30）。</summary>
    public static int[] DieDropUseItemRates = new int[30];

    // ---- CharStatus 页 ----
    public static bool boParalyCanRun = false;
    public static bool boParalyCanWalk = false;
    public static bool boParalyCanHit = false;
    public static bool boParalyCanSpell = false;
    /// <summary>攻击模式开关（Delphi array[HAM_ALL..HAM_NATION]，7 开 1 关）。</summary>
    public static bool[] AttatckModes = new bool[8];
    public static bool boStartGameAuxiliary = true; // 是否启动内挂
    /// <summary>ClientConfigs[38]（显示怪名；完整 ClientConfigs 数组建档随客户端配置批次接入）。</summary>
    public static bool ClientConfig38ShowMonName = true;

    /// <summary>typed-constant 初值复位（MsgColor/HumanDie/CharStatus 页字段，测试隔离用）。</summary>
    public static void ResetGameDieDefaults()
    {
        btHearMsgFColor = 0x00;
        btHearMsgBColor = 0xFF;
        btWhisperMsgFColor = 0xFC;
        btWhisperMsgBColor = 0xFF;
        btGMWhisperMsgFColor = 0xFF;
        btGMWhisperMsgBColor = 0x38;
        btSendWhisperMsgFColor = 0xB4;
        btSendWhisperMsgBColor = 0xFF;
        btRefreshGameGoldFColor = 0xDB;
        btRefreshGameGoldBColor = 0xFF;
        btShowWhisperFColor = 219;
        btShowWhisperBColor = 0xFF;
        btCloseWhisperFColor = 219;
        btCloseWhisperBColor = 0x00;
        btCryMsgFColor = 0x0;
        btCryMsgBColor = 0x97;
        btGreenMsgFColor = 0xDB;
        btGreenMsgBColor = 0xFF;
        btBlueMsgFColor = 0xFF;
        btBlueMsgBColor = 0xFC;
        btRedMsgFColor = 0xFF;
        btRedMsgBColor = 0x38;
        btGuildMsgFColor = 0xDB;
        btGuildMsgBColor = 0xFF;
        btGroupMsgFColor = 0xC4;
        btGroupMsgBColor = 0xFF;
        btCustMsgFColor = 0xFC;
        btCustMsgBColor = 0xFF;
        btNationMsgFColor = 0xDB;
        btNationMsgBColor = 0xFF;
        btUserSayMsgFColor = 0xFF;
        btUserSayMsgBColor = 0xFD;
        btTopUserSayMsgFColor = 0x97;
        btTopUserSayMsgBColor = 0xEA;
        btDropItemFColor = 0xFF;
        btDropItemBColor = 0xFD;
        boNPCLabelFontStroke = true;
        btNPCLabelNormalColor = 251;
        btNPCLabelMouseMoveColor = 253;
        btNPCLabelMouseDownColor = 58;
        boDieScatterBag = true;
        nDieScatterBagRate = 3;
        boDieRedScatterBagAll = true;
        nDieDropUseItemRate = 30;
        nDieRedDropUseItemRate = 15;
        boDieDropGold = false;
        boKillByHumanDropUseItem = false;
        boKillByMonstDropUseItem = false;
        boKillByHumanDropJewelryBoxItem = false;
        boKillByMonstDropJewelryBoxItem = false;
        boKillByHumanDropGodBlessItem = false;
        boKillByMonstDropGodBlessItem = false;
        nDropJewelryBoxItemRate = 30;
        nDropGodBlessItemRate = 30;
        nDropUseItemsMaxCount = 15;
        nScatterBagItemsMinLevel = 1;
        boDropUseItem = false;
        nDieRedDropUseItemOneRate = 20;
        Array.Fill(DieDropUseItemRates, 30);
        boParalyCanRun = false;
        boParalyCanWalk = false;
        boParalyCanHit = false;
        boParalyCanSpell = false;
        for (int i = 0; i < 8; i++)
            AttatckModes[i] = i < 7;
        boStartGameAuxiliary = true;
        ClientConfig38ShowMonName = true;
    }
}
