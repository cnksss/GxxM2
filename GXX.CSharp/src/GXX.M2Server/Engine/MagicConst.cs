using GXX.Core.Protocol;

namespace GXX.M2Server.Engine;

/// <summary>SKILL_* 技能 ID 常量（M2Share.pas 1:1，全部 90+ 项）。</summary>
public static class MagicConst
{
    public const ushort SKILL_ONESWORD = 3;      // 基本剑术
    public const ushort SKILL_ILKWANG = 4;       // 攻杀剑术
    public const ushort SKILL_YEDO = 7;
    public const ushort SKILL_ERGUM = 12;        // 刺杀剑术
    public const ushort SKILL_BANWOL = 25;       // 半月刀法
    public const ushort SKILL_FIRESWORD = 26;    // 烈火剑法
    public const ushort SKILL_MOOTEBO = 27;      // 野蛮冲撞
    public const ushort SKILL_FIREBALL = 1;      // 火球术
    public const ushort SKILL_FIREBALL2 = 5;
    public const ushort SKILL_HEALLING = 2;      // 治愈术
    public const ushort SKILL_AMYOUNSUL = 6;     // 施毒术
    public const ushort SKILL_FIREWIND = 8;      // 抗拒火环
    public const ushort SKILL_FIRE = 9;          // 地狱火
    public const ushort SKILL_SHOOTLIGHTEN = 10; // 疾光电影
    public const ushort SKILL_LIGHTENING = 11;   // 雷电术
    public const ushort SKILL_FIRECHARM = 13;    // 灵魂火符
    public const ushort SKILL_HANGMAJINBUB = 14;
    public const ushort SKILL_DEJIWONHO = 15;
    public const ushort SKILL_HOLYSHIELD = 16;
    public const ushort SKILL_SKELLETON = 17;    // 召唤骷髅
    public const ushort SKILL_CLOAK = 18;        // 隐身术
    public const ushort SKILL_BIGCLOAK = 19;
    public const ushort SKILL_TAMMING = 20;      // 诱惑之光
    public const ushort SKILL_SPACEMOVE = 21;
    public const ushort SKILL_EARTHFIRE = 22;    // 火墙
    public const ushort SKILL_FIREBOOM = 23;     // 爆裂火焰
    public const ushort SKILL_LIGHTFLOWER = 24;  // 地狱雷光
    public const ushort SKILL_SHOWHP = 28;
    public const ushort SKILL_BIGHEALLING = 29;  // 群体治疗
    public const ushort SKILL_SINSU = 30;        // 召唤神兽
    public const ushort SKILL_SHIELD = 31;       // 魔法盾
    public const ushort SKILL_KILLUNDEAD = 32;   // 圣言术
    public const ushort SKILL_SNOWWIND = 33;     // 冰咆哮
    public const ushort SKILL_UNAMYOUNSUL = 34;  // 解毒术
    public const ushort SKILL_WINDTEBO = 35;     // 火焰冰
    public const ushort SKILL_MABE = 36;         // 怒之/静之火焰冰
    public const ushort SKILL_GROUPLIGHTENING = 37; // 群体雷电术

    public const ushort SKILL_38 = 38;           // 诅咒术
    public const ushort SKILL_GROUPAMYOUNSUL = 51; // 群体施毒术
    public const ushort SKILL_GROUPDEDING = 39;  // 彻地钉
    public const ushort SKILL_40 = 40;           // 双龙斩
    public const ushort SKILL_41 = 41;           // 狮子吼
    public const ushort SKILL_42 = 42;           // 龙影剑法
    public const ushort SKILL_43 = 43;           // 雷霆剑法
    public const ushort SKILL_44 = 44;           // 寒冰掌
    public const ushort SKILL_45 = 45;           // 灭天火
    public const ushort SKILL_46 = 46;           // 新诅咒术
    public const ushort SKILL_47 = 47;           // 火龙烈焰
    public const ushort SKILL_48 = 48;           // 气功波
    public const ushort SKILL_49 = 49;           // 净化术
    public const ushort SKILL_50 = 50;           // 无极真气
    public const ushort SKILL_52 = 52;           // 飓风破
    public const ushort SKILL_53 = 53;
    public const ushort SKILL_54 = 54;           // 骷髅咒
    public const ushort SKILL_55 = 55;           // 召唤月灵
    public const ushort SKILL_56 = 56;           // 逐日剑法
    public const ushort SKILL_57 = 57;           // 噬血术
    public const ushort SKILL_58 = 58;           // 静之流星火雨
    public const ushort SKILL_59 = 59;
    public const ushort SKILL_60 = 60;           // 静之破魂斩
    public const ushort SKILL_61 = 61;           // 静之劈星斩
    public const ushort SKILL_62 = 62;           // 静之雷霆一击
    public const ushort SKILL_63 = 63;           // 静之噬魂沼泽
    public const ushort SKILL_64 = 64;           // 静之末日审判
    public const ushort SKILL_65 = 65;           // 静之火龙气焰
    public const ushort SKILL_66 = 66;           // 开天斩
    public const ushort SKILL_67 = 67;           // 先天元力
    public const ushort SKILL_68 = 68;           // 酒气护体
    public const ushort SKILL_69 = 69;           // 禁锢
    public const ushort SKILL_70 = 70;           // 心灵召唤
    public const ushort SKILL_71 = 71;
    public const ushort SKILL_72 = 72;
    public const ushort SKILL_73 = 73;           // 道力盾
    public const ushort SKILL_74 = 74;           // 分身术
    public const ushort SKILL_75 = 75;           // 护体神盾
    public const ushort SKILL_76 = 76;           // 召唤圣兽
    public const ushort SKILL_77 = 77;           // 唯我独尊
    public const ushort SKILL_78 = 78;           // 召唤火灵
    public const ushort SKILL_79 = 79;           // 神龙附体
    public const ushort SKILL_80 = 80;           // 召唤巨魔
    public const ushort SKILL_81 = 81;
    public const ushort SKILL_82 = 82;
    public const ushort SKILL_83 = 83;
    public const ushort SKILL_85 = 84;
    public const ushort SKILL_86 = 86;
    public const ushort SKILL_87 = 87;           // 武力盾
    public const ushort SKILL_88 = 88;
    public const ushort SKILL_89 = 89;
    public const ushort SKILL_90 = 90;           // 宠物捕捉
    public const ushort SKILL_91 = 91;           // 招魂术

    public const ushort SKILL_100 = 100;
    public const ushort SKILL_101 = 101;
    public const ushort SKILL_102 = 102;
    public const ushort SKILL_103 = 103;
    public const ushort SKILL_104 = 104;         // 凤舞祭
    public const ushort SKILL_105 = 105;         // 惊雷爆
    public const ushort SKILL_106 = 106;         // 冰天雪地
    public const ushort SKILL_107 = 107;         // 双龙破
    public const ushort SKILL_108 = 108;         // 虎啸诀
    public const ushort SKILL_109 = 109;         // 八卦掌
    public const ushort SKILL_110 = 110;         // 三焰咒
    public const ushort SKILL_111 = 111;         // 万剑归宗
    public const ushort SKILL_112 = 112;
    public const ushort SKILL_113 = 113;         // 断空斩
    public const ushort SKILL_114 = 114;         // 倚天辟地
    public const ushort SKILL_115 = 115;         // 血魄一击
    public const ushort SKILL_116 = 116;
    public const ushort SKILL_117 = 117;

    public const ushort SKILL_NEW_TEST = 201;
    public const ushort SKILL_202 = 202;         // 裂神符
    public const ushort SKILL_203 = 203;         // 死亡之眼
    public const ushort SKILL_204 = 204;         // 十步一杀
    public const ushort SKILL_205 = 205;         // 冰霜雪雨
    public const ushort SKILL_206 = 206;         // 冰霜群雨
    public const ushort SKILL_207 = 207;         // 金刚护体
    public const ushort SKILL_208 = 208;         // 旋风斩
    public const ushort SKILL_209 = 209;         // 五雷轰
    public const ushort SKILL_210 = 210;         // 幽冥火符
}

/// <summary>引擎全局配置中技能体系所用字段（g_Config 对应子集，可由配置加载覆盖）。</summary>
public static partial class M2Config
{
    /// <summary>幸运达到该值时攻击恒取上限（nMaxLuckMaxPower）。</summary>
    public static int nMaxLuckMaxPower = 10;

    /// <summary>强化技能 1..9 重威力倍率表（百分比，下标 0=1重）。</summary>
    public static readonly int[] NewLevelMagicPowerRates = { 100, 110, 120, 130, 140, 150, 160, 170, 180 };

    /// <summary>强化超过 9 重后每重追加倍率（百分比）。</summary>
    public static int NewLevelMagicPowerRatesAfter9 = 10;

    /// <summary>特定技能强化倍率表（Index → 技能组，见 GetNewLevelPower 映射）。</summary>
    public static readonly int[][] NewLevelMagicPowerRatesSpecific =
    {
        new[] { 100, 110, 120, 130, 140, 150, 160, 170, 180 }, // Index 0 基本剑术组
        new[] { 100, 110, 120, 130, 140, 150, 160, 170, 180 },
        new[] { 100, 110, 120, 130, 140, 150, 160, 170, 180 },
        new[] { 100, 110, 120, 130, 140, 150, 160, 170, 180 },
        new[] { 100, 110, 120, 130, 140, 150, 160, 170, 180 },
        new[] { 100, 110, 120, 130, 140, 150, 160, 170, 180 }, // Index 5 逐日
        new int[9], new int[9], new int[9], new int[9], new int[9], new int[9], new int[9], new int[9],
        new int[9], new int[9], new int[9], new int[9], new int[9], new int[9], new int[9], new int[9],
        new int[9], // Index 20 流星火雨
        new int[9], new int[9],
        new int[9], new int[9], new int[9],
        new int[9], new int[9], new int[9], new int[9], new int[9], new int[9], new int[9], new int[9],
        new int[9], new int[9], new int[9], new int[9], new int[9], new int[9], new int[9], new int[9],
        new int[9], new int[9], new int[9], new int[9], new int[9], new int[9], new int[9]
    };

    public static bool boFireWindPushSameLevel;  // 抗拒火环是否推同级
    public static bool boQigongPushSameLevel;    // 气功波是否推同级
    public static int nMagicAttackRage = 12;     // 魔法攻击默认视距
    public static bool boViewRangeCanMagicAttack;
    public static int nSkill204Distance = 10;
    public static int nSkillLighteningPowerRate = 100;
    public static int nSkill57PowerRate = 100;
    public static int nSkill57AddHPRate = 50;
    public static int nNGHitStruckDecNG = 10;

    // ---- 物品随机升级（g_Config.n*Add* 系列）----
    public static int nWeaponDCAddRate = 20;
    public static int nWeaponDCAddValueRate = 10;
    public static int nWeaponDCAddValueMaxLimit = 20;
    public static int nWeaponHitSpeedAddRate;
    public static int nWeaponHitSpeedAddValueRate = 15;
    public static int nWeaponHitSpeedAddValueMaxLimit = 12;
    public static int nWeaponMCAddRate;
    public static int nWeaponMCAddValueRate = 10;
    public static int nWeaponMCAddValueMaxLimit = 20;
    public static int nWeaponSCAddRate;
    public static int nWeaponSCAddValueRate = 10;
    public static int nWeaponSCAddValueMaxLimit = 20;

    public static int nDressACAddRate = 20;
    public static int nDressACAddValueRate = 10;
    public static int nDressACAddValueMaxLimit = 15;
    public static int nDressMACAddRate;
    public static int nDressMACAddValueRate = 10;
    public static int nDressMACAddValueMaxLimit = 15;
    public static int nDressDCAddRate;
    public static int nDressDCAddValueRate = 10;
    public static int nDressDCAddValueMaxLimit = 15;
    public static int nDressMCAddRate;
    public static int nDressMCAddValueRate = 10;
    public static int nDressMCAddValueMaxLimit = 15;
    public static int nDressSCAddRate;
    public static int nDressSCAddValueRate = 10;
    public static int nDressSCAddValueMaxLimit = 15;

    /// <summary>脚本 CHECKSERVERNAME 用服务器名。</summary>
    public static string ServerNameForScript = "GXX";
}
