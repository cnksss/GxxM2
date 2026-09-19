namespace GXX.M2Server.Engine;

/// <summary>
/// 批次J34：ItemSet.pas 巨片第一片（主保存组：凹槽/体验率/行会传送/攻沙毒/传送移动/异常状态）
/// 依赖 g_Config 字段（M2Share.pas typed-constant 初值 1:1）。
/// </summary>
public static partial class M2Config
{
    // ---- 凹槽（物品笛子） ----
    public static bool boOpenItemFlute = true;              // 开启凹槽功能
    public static bool boDisableRightClickFluteStone = false;
    public static int nItemFluteStoneCount = 3;             // 相同属性镶嵌限制数量
    public static int nItemFluteStoneIdxCount = 3;
    public static int nItemFluteStoneOverlapCount = 0;      // 单孔允许叠加宝石数量

    // ---- 物品体验率 ----
    public static int nItemExpRate = 10000;
    public static int nItemPowerRate = 10000;

    // ---- 行会传送/攻沙毒/传送移动 ----
    public static int nGuildRecallTime = 180;
    public static int nAttackPosionRate = 5;
    public static int nAttackPosionTime = 6;
    public static bool boUserMoveCanDupObj = false;
    public static bool boUserMoveCanOnItem = true;
    public static uint dwUserMoveTime = 10;

    // ---- 异常状态（魔道麻痹/冰冻/蛛网） ----
    public static uint dwMDParalysisRate = 5;   // 魔道麻痹戒指机率
    public static uint dwMDParalysisTime = 6;   // 魔道麻痹戒指时间
    public static uint dwFrozenRate = 5;        // 冰冻戒指机率
    public static uint dwFrozenTime = 6;        // 冰冻戒指时间
    public static bool boFrozenUseMagicStruck = false; // 冰冻对魔法攻击有效
    public static uint dwCobwebWindingRate = 5; // 蜘蛛网戒指机率
    public static uint dwCobwebWindingTime = 6; // 蜘蛛网戒指时间
    public static bool boCobwebWindingUseMagicStruck = false; // 蜘蛛网对魔法攻击有效

    // ---- 批次J35：AddValue 极品属性 / UnKnow 未鉴定 / NewAbil 新属性 ----

    /// <summary>极品属性全键（键名 = INI 键，初值 = typed-constant；ItemSetAddValueTables.Defaults）。</summary>
    public static readonly System.Collections.Generic.Dictionary<string, int> ItemSetAddValues =
        new(ItemSetAddValueTables.Defaults);

    /// <summary>未鉴定物品键（UnknowRing/Necklace/HelMet × AC/DC/MC/SC/MAC × AddRate/AddValueMaxLimit）。</summary>
    public static readonly System.Collections.Generic.Dictionary<string, int> ItemSetUnknowValues = new()
    {
        ["UnknowRingACAddRate"] = 20,
        ["UnknowRingACAddValueMaxLimit"] = 4,
        ["UnknowRingDCAddRate"] = 20,
        ["UnknowRingDCAddValueMaxLimit"] = 6,
        ["UnknowRingMCAddRate"] = 20,
        ["UnknowRingMCAddValueMaxLimit"] = 6,
        ["UnknowRingSCAddRate"] = 20,
        ["UnknowRingSCAddValueMaxLimit"] = 6,
        ["UnknowRingMACAddRate"] = 20,
        ["UnknowRingMACAddValueMaxLimit"] = 4,
        ["UnknowNecklaceACAddRate"] = 20,
        ["UnknowNecklaceACAddValueMaxLimit"] = 5,
        ["UnknowNecklaceDCAddRate"] = 30,
        ["UnknowNecklaceDCAddValueMaxLimit"] = 5,
        ["UnknowNecklaceMCAddRate"] = 30,
        ["UnknowNecklaceMCAddValueMaxLimit"] = 5,
        ["UnknowNecklaceSCAddRate"] = 30,
        ["UnknowNecklaceSCAddValueMaxLimit"] = 5,
        ["UnknowNecklaceMACAddRate"] = 20,
        ["UnknowNecklaceMACAddValueMaxLimit"] = 5,
        ["UnknowHelMetACAddRate"] = 20,
        ["UnknowHelMetACAddValueMaxLimit"] = 4,
        ["UnknowHelMetDCAddRate"] = 30,
        ["UnknowHelMetDCAddValueMaxLimit"] = 3,
        ["UnknowHelMetMCAddRate"] = 30,
        ["UnknowHelMetMCAddValueMaxLimit"] = 3,
        ["UnknowHelMetSCAddRate"] = 30,
        ["UnknowHelMetSCAddValueMaxLimit"] = 3,
        ["UnknowHelMetMACAddRate"] = 20,
        ["UnknowHelMetMACAddValueMaxLimit"] = 4,
    };

    // ---- NewAbil 新属性（标量；数组随巨片余部） ----
    public static bool boItemNewAbilAllowUse = false;
    public static int nItemNewAbilMonRandomAddValue = 10;
    public static int nItemNewAbilMakeRandomAddValue = 10;
    public static int nItemNewAbilScriptRandomAddValue = 10;
    public static bool boCloseDefenseUseScale = false;
    public static bool boReboundUseScale = false;
    public static uint dwCritAttackHurtRate = 100;
    public static uint dwDamageReboundRate = 30;
    public static uint dwFatalBlowBasePower = 100;
    public static uint dwFatalBlowNeedPower1 = 120;
    public static uint dwFatalBlowNeedPower2 = 150;
    public static uint dwFatalBlowNeedPower3 = 200;

    /// <summary>typed-constant 初值复位（ItemSet 第一片字段，测试隔离用）。</summary>
    public static void ResetItemSetSliceDefaults()
    {
        boOpenItemFlute = true;
        boDisableRightClickFluteStone = false;
        nItemFluteStoneCount = 3;
        nItemFluteStoneIdxCount = 3;
        nItemFluteStoneOverlapCount = 0;
        nItemExpRate = 10000;
        nItemPowerRate = 10000;
        nGuildRecallTime = 180;
        nAttackPosionRate = 5;
        nAttackPosionTime = 6;
        boUserMoveCanDupObj = false;
        boUserMoveCanOnItem = true;
        dwUserMoveTime = 10;
        dwMDParalysisRate = 5;
        dwMDParalysisTime = 6;
        dwFrozenRate = 5;
        dwFrozenTime = 6;
        boFrozenUseMagicStruck = false;
        dwCobwebWindingRate = 5;
        dwCobwebWindingTime = 6;
        boCobwebWindingUseMagicStruck = false;
    }

    /// <summary>typed-constant 初值复位（AddValue/UnKnow/NewAbil 字段，测试隔离用）。</summary>
    public static void ResetItemSetSlice2Defaults()
    {
        ItemSetAddValues.Clear();
        foreach (var (key, def) in ItemSetAddValueTables.Defaults)
            ItemSetAddValues[key] = def;
        ItemSetUnknowValues["UnknowRingACAddRate"] = 20;
        ItemSetUnknowValues["UnknowRingACAddValueMaxLimit"] = 4;
        ItemSetUnknowValues["UnknowRingDCAddRate"] = 20;
        ItemSetUnknowValues["UnknowRingDCAddValueMaxLimit"] = 6;
        ItemSetUnknowValues["UnknowRingMCAddRate"] = 20;
        ItemSetUnknowValues["UnknowRingMCAddValueMaxLimit"] = 6;
        ItemSetUnknowValues["UnknowRingSCAddRate"] = 20;
        ItemSetUnknowValues["UnknowRingSCAddValueMaxLimit"] = 6;
        ItemSetUnknowValues["UnknowRingMACAddRate"] = 20;
        ItemSetUnknowValues["UnknowRingMACAddValueMaxLimit"] = 4;
        ItemSetUnknowValues["UnknowNecklaceACAddRate"] = 20;
        ItemSetUnknowValues["UnknowNecklaceACAddValueMaxLimit"] = 5;
        ItemSetUnknowValues["UnknowNecklaceDCAddRate"] = 30;
        ItemSetUnknowValues["UnknowNecklaceDCAddValueMaxLimit"] = 5;
        ItemSetUnknowValues["UnknowNecklaceMCAddRate"] = 30;
        ItemSetUnknowValues["UnknowNecklaceMCAddValueMaxLimit"] = 5;
        ItemSetUnknowValues["UnknowNecklaceSCAddRate"] = 30;
        ItemSetUnknowValues["UnknowNecklaceSCAddValueMaxLimit"] = 5;
        ItemSetUnknowValues["UnknowNecklaceMACAddRate"] = 20;
        ItemSetUnknowValues["UnknowNecklaceMACAddValueMaxLimit"] = 5;
        ItemSetUnknowValues["UnknowHelMetACAddRate"] = 20;
        ItemSetUnknowValues["UnknowHelMetACAddValueMaxLimit"] = 4;
        ItemSetUnknowValues["UnknowHelMetDCAddRate"] = 30;
        ItemSetUnknowValues["UnknowHelMetDCAddValueMaxLimit"] = 3;
        ItemSetUnknowValues["UnknowHelMetMCAddRate"] = 30;
        ItemSetUnknowValues["UnknowHelMetMCAddValueMaxLimit"] = 3;
        ItemSetUnknowValues["UnknowHelMetSCAddRate"] = 30;
        ItemSetUnknowValues["UnknowHelMetSCAddValueMaxLimit"] = 3;
        ItemSetUnknowValues["UnknowHelMetMACAddRate"] = 20;
        ItemSetUnknowValues["UnknowHelMetMACAddValueMaxLimit"] = 4;
        boItemNewAbilAllowUse = false;
        nItemNewAbilMonRandomAddValue = 10;
        nItemNewAbilMakeRandomAddValue = 10;
        nItemNewAbilScriptRandomAddValue = 10;
        boCloseDefenseUseScale = false;
        boReboundUseScale = false;
        dwCritAttackHurtRate = 100;
        dwDamageReboundRate = 30;
        dwFatalBlowBasePower = 100;
        dwFatalBlowNeedPower1 = 120;
        dwFatalBlowNeedPower2 = 150;
        dwFatalBlowNeedPower3 = 200;
    }
}
