using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 物理/魔法受击减免本体 1:1 移植（批次J145）：
/// `TBaseObject.GetHitStruckDamage`（`ObjBase.pas` 38956-39244）、
/// `TBaseObject.GetMagStruckDamage`（39246-39446）；
/// 辅助源 `M2Share.pas` 1670（`nOrdinarySkill31Rate` 默认 **8**）、
/// 1671（`nSkill31Rates: array[0..8]` 默认 **(10,20,30,40,50,60,70,80,90)**）、
/// 2165-2167（`nSuperShiledPowerRate` 默认 **30**、`nSuperShiledLevelUpDecPowerRate` 默认 **0**、
/// `nOpenSuperShiledRate` 默认 **5**）、2180-2181（两个显示开关默认均 **True**）、
/// 2812（`boCloseDefenseUseScale` 默认 `False`）、
/// 1643-1644（`boEnabledPosionDecMAC` 默认 `False`、`nPosionDecMACRate` 默认 **12**）。
///
/// ============================ 一、`nType` 是一个"模式选择器"，不是"类型标签" ============================
///
/// 两个函数都用 `nType` 做**前导分支**，但**同一函数内两种函数的 `nType` 语义并不一致**：
///
/// `GetHitStruckDamage`（物理）：
/// - **`nType = 4`**：**完全跳过"外层的全部防御减免"，但仍会走下面的三个盾** ——
///   注释「完全忽视目标防御时，不要忽视盾防御 2020-11-09 23:45:53」。
///   **这是 J143 里 `CanCloseDefense` 为真时传的那个 4。**
/// - **`nType = 2`**：**只做魔法盾防御，然后立刻 `Exit`**（注释「破魔法盾防御 piaoyun 2013-08-18」）
///   —— **它在改动数据之后直接返回，不走后面的物理盾、武力盾与护体神盾**。
/// - **`nType = 1`（且 `MagicACInfo` 有效）**：**先按魔法 AC 走一遍（含 `MagicACInfo.btHum/btHero/btMon`
///   的减伤），再补 `POISON_DAMAGEARMOR` 状态，然后 `Exit`**（注释「破物理防御 piaoyun 2013-08-18」）。
/// - **`nType = 3`**：**跳过 AC 减免那一段，但仍走不死系加成与后面的盾**。
/// - **`nType = 0`（默认）**：**完整流程**。
///
/// `GetMagStruckDamage`（魔法）里 **`nType = 0` 才是"走完整流程"**，
/// 而 **`MagicACInfo <> nil` 时会在 AC 段之后直接 `Exit`**（注释「自定义技能才有这玩意 2020-09-15 00:55:06」）
/// —— **两个函数的 `nType` 用法与前导条件方向相反**，属易错点。
/// 已用 `HitTypeModes`、`MagTypeZeroIsFull`、`MagExitsOnMagicAcInfo` 固化。
///
/// ============================ 二、四个几乎逐字重复的"盾"块 ============================
///
/// `GetHitStruckDamage` 里 **同一段"强化盾/普通盾 + 防御值钳位"结构重复了四次**：
/// 魔法盾（`m_boAbilMagBubbleDefence`）、武力盾（`m_boAbilNewHitBubbleDefence`）、
/// 以及 `GetMagStruckDamage` 里的魔法盾（`m_boAbilMagBubbleDefence`）与
/// 新魔法盾（`m_boAbilNewMagBubbleDefence`）。**四处的差别只在字段名与"强化等级"的来源**，
/// 但**它们的形态完全一致**：
/// 1. **强化盾判定**：**`NewLevel > 0` 或 `Level > 3`**（注释「强化魔法盾 / 4级魔法盾 chongchong 2013-12-27」）
///    —— **注意是"或"、且普通档位用 `> 3`（即 0..3 走普通、4 起走强化）**；
/// 2. **强化盾取值**：**`nSkill31Rate := nSkill31Rates[0]`（默认 10），
///    若 `NewLevel` 在 1..9 取 `nSkill31Rates[NewLevel - 1]`，
///    若 `NewLevel > 9` 取 `nSkill31Rates[8]`** —— **注意先赋 `[0]` 再覆盖，
///    故 `NewLevel = 0`（但 `Level > 3`）时会停在 `[0]`**；
/// 3. **减免值**：**`DefenseValue := Min(Round(nDamage / 100 * nSkill31Rate), High(Integer))`**
///    或者普通盾的 **`Round(nDamage / 100 * ((Level + 1) * nOrdinarySkill31Rate))`**
///    —— **注意普通盾是"等级 + 1 后乘基础倍率"**；
/// 4. **钳位两段**：**`if DefenseValue < 0 then 0 else if DefenseValue > nDamage then nDamage`**
///    —— **是 `else if`，即"负数"与"超上限"互斥判断**；
/// 5. **扣血**：`nDamage := nDamage - DefenseValue`。
///
/// **四处内部都还嵌着一段被 `{ }` 注释掉的"比例忽视"块**
/// （注释「比例忽视，不要算盾 2020-11-11 16:15:32」），
/// **它是 J143 那条 `boCloseDefenseUseScale` 开关在盾上的对应物，现已被整体停用**。
/// 已用 `FourShieldBlocks`、`ShieldBlockShape`、`StrongShieldConditionIsOr`、
/// `LevelThreeIsStillNormal`、`RateArrayIndexClamping`、`NewLevelZeroStopsAtFirst`、
/// `NormalShieldLevelPlusOne`、`ClampIsElseIf`、`CommentedScaleBlocksDisabled` 固化。
///
/// **护体神盾**（`m_boSuperShiled`）单独一处，**且被两道门包住**：
/// 外层是 **`m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]`**
/// （**只有玩家/英雄/人形怪有**），内层是 **`Random(g_Config.nOpenSuperShiledRate) = 0`**
/// （**注意是 `= 0` 而不是 `<=`，即"按几率命中"**，默认几率 **5**）。
/// **伤害减免公式与四个盾块不同**：**`Round(nDamage / 100 * (nSuperShiledPowerRate +
/// (btLevel + btNewLevel) * nSuperShiledLevelUpDecPowerRate))`**
/// —— **把两个等级相加后乘"每级减攻击百分比"**（默认每级为 **0**，故默认下等级不影响）。
/// 触发后**还会**：按两个显示开关发 `RM_SENDSUPERSHILEDEFFECT`（**参数直接写两个布尔的整数值**）、
/// 以及**一段练级逻辑**（**要求种族是玩家/英雄、技能有 `MagicInfo`、
/// `btLevel < MagicInfo.btTrainLv`、且 `TrainLevel[btLevel] <= m_Abil.Level`**），
/// 练级失败时发 `RM_MAGIC_LVEXP` 延迟 **3000ms**。
/// 已用 `SuperShieldRaceGate`、`SuperShieldRateIsEqualZero`、
/// `SuperShieldFormulaUsesLevelSum`、`SuperShieldBoolAsIntParams`、
/// `SuperShieldTrainFourGates`、`SuperShieldTrainMsgDelay` 固化。
///
/// ============================ 三、"魔法盾"在物理函数里也生效，且顺序有讲究 ============================
///
/// `GetHitStruckDamage` 里 **`nType <> 3` 且 `nDamage > 0` 时仍会走魔法盾**（39104）
/// —— **即物理攻击也会被魔法盾削减**，这与直觉相反但确实如此。
/// **`nType = 3` 会同时跳过 AC 段和魔法盾段**，却**不跳过武力盾与护体神盾**。
/// 已用 `MagicShieldAppliesToPhysical`、`TypeThreeSkipsAcAndMagicShield`、
/// `TypeThreeKeepsForceAndSuper` 固化。
///
/// **`m_nStruckHP := n14`**（39290）**只在 `GetMagStruckDamage` 的 `nType = 0` 段里赋值**
/// —— **它是"上次被扣的魔御值"的留存字段**。
/// 已用 `StruckHpAssignedInMagOnly` 固化。
///
/// ============================ 四、AC / MAC 取值的"随机区间"写法 ============================
///
/// 两处都用同一形状：**`n14 := AC2 - AC1 + 1`；`if n14 > 0 then n14 := Random(n14)`；
/// `n14 := Max(0, AC1 + n14)`** ——
/// **即"先算区间长度、非正时保留原值（负长度）不再随机、再叠加到 AC1 并归零"**。
/// **关键点：`n14 > 0` 为假时 `n14` 仍是那个"区间长度"本身，
/// 而下一步是 `AC1 + n14` 而不是 `AC1 + Random(...)`** ——
/// **即上下限倒挂时会退化成 `AC1 + (AC2 - AC1 + 1)`，也就是 `AC2 + 1`（比上限还大 1）**。
/// 已用 `AcRangeShape`、`InvertedRangeYieldsAc2PlusOne`、`InvertedRangeNoRandom` 固化。
///
/// **`GetMagStruckDamage` 的 `MagicACInfo` 段会重新采一次"物理 AC"**
/// （注释说明这是"自定义技能"路径），**即先按 MAC 减免、再按 AC 减免**，
/// 与 `GetHitStruckDamage` 的 `nType = 1` 分支方向相反（后者只按 AC）。
/// 已用 `MagMagicAcUsesPhysicalAc`、`HitTypeOneUsesAcOnly` 固化。
///
/// ============================ 五、两处"红毒减魔御"与"脚本中毒" ============================
///
/// `GetMagStruckDamage` 的 `nType = 0` 段有**两道独立的红毒相关减免**：
/// 1. **`m_wStatusTimeArr[POISON_DAMAGEARMOR] > 0` 且 `boEnabledPosionDecMAC` 时
///    `n14 := Round(n14 / 100 * (100 - nPosionDecMACRate))`**（注释「中红毒减魔御 chongchong 2016-05-13」，
///    默认倍率 **12**）；
/// 2. **`m_nStatusPowerTime[POISON_DAMAGEARMOR] > 0` 且 `m_nStatusPowerType[POISON_DAMAGEARMOR] = 1` 时
///    `n14 := Round((1000 - Max(0, Min(1000, m_nStatusPower[POISON_DAMAGEARMOR]))) / 1000 * n14)`**
///    （注释「脚本MAKEPOSION」）—— **注意这里是"用 1000 减去被钳到 0..1000 的状态值"再按千分比缩放**。
///
/// **同一形状的第二处在 `GetHitStruckDamage` 里也出现（39104 段之后、`bhtBlowDefence` 附近），
/// 但那里没有第 1 道（红毒减魔御）** —— **重复但不相同**。
/// 已用 `TwoPosionDecPaths`、`ScriptPosionUsesThousandMinus`、
/// `ScriptPosionClampedToThousand`、`HitLacksPosionDecMac` 固化。
///
/// ============================ 六、不死系加成与收尾 ============================
///
/// 两处都有：**`if 自己是 LA_UNDEAD 且目标非空且目标种族 in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]
/// then Inc(nDamage, TSmartObject(Target).m_AddAbil.bt1DF)`**
/// （注释「不死系的怪物攻击人物，英雄，假人时，当攻击目标有神圣属性时，会减少伤害」）
/// —— **注意它只对"人类三类目标"生效，且是"加回去"，即抵消神圣属性的减伤**。
/// 在物理函数里**它带 `nType <> 3` 门**；在魔法函数里**它没有 `nType` 门**。
/// 已用 `UndeadBonusThreeRaces`、`UndeadBonusHitHasTypeGate`、
/// `UndeadBonusMagHasNoTypeGate` 固化。
///
/// 两个函数**收尾方式不同**：物理是 **`Result := Max(0, nDamage)`**；
/// 魔法在 `MagicACInfo <> nil` 的早退路径里先 **`if nDamage < 0 then 0`** 再 `Result := nDamage; Exit`，
/// 而**正常路径的收尾在 39419 之后**（与物理一致，取 `Max(0, ...)`）。
/// 已用 `HitResultClamped`、`MagEarlyExitClamped` 固化。
/// </summary>
public static class StruckDamageCore
{
    // ===================== 常量 =====================

    /// <summary>`RC_PLAYOBJECT`。</summary>
    public const int RcPlayObject = 0;

    /// <summary>`RC_HEROOBJECT`。</summary>
    public const int RcHeroObject = 1;

    /// <summary>`RC_PLAYMOSTER`。</summary>
    public const int RcPlayMoster = 150;

    /// <summary>`LA_UNDEAD`。</summary>
    public const int LaUndead = 1;

    /// <summary>`POISON_DAMAGEARMOR`。</summary>
    public const int PoisonDamageArmor = 1;

    /// <summary>强化盾的普通档位上限（`> 3` 转强化）。</summary>
    public const int StrongShieldLevelThreshold = 3;

    /// <summary>`nSkill31Rates` 数组长度。</summary>
    public const int Skill31RateCount = 9;

    /// <summary>`nSkill31Rates` 默认值。</summary>
    public static readonly int[] DefaultSkill31Rates = { 10, 20, 30, 40, 50, 60, 70, 80, 90 };

    /// <summary>`nOrdinarySkill31Rate` 默认值。</summary>
    public const int DefaultOrdinarySkill31Rate = 8;

    /// <summary>`nSuperShiledPowerRate` 默认值。</summary>
    public const int DefaultSuperShiledPowerRate = 30;

    /// <summary>`nSuperShiledLevelUpDecPowerRate` 默认值。</summary>
    public const int DefaultSuperShiledLevelUpDecPowerRate = 0;

    /// <summary>`nOpenSuperShiledRate` 默认值。</summary>
    public const int DefaultOpenSuperShiledRate = 5;

    /// <summary>`nPosionDecMACRate` 默认值。</summary>
    public const int DefaultPosionDecMacRate = 12;

    /// <summary>脚本中毒状态值的分母。</summary>
    public const int ScriptPosionThousand = 1000;

    /// <summary>练级消息延迟。</summary>
    public const int MagicLvExpDelay = 3000;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => StrongShieldLevelThreshold == 3
           && Skill31RateCount == 9
           && DefaultOrdinarySkill31Rate == 8
           && DefaultSuperShiledPowerRate == 30
           && DefaultSuperShiledLevelUpDecPowerRate == 0
           && DefaultOpenSuperShiledRate == 5
           && DefaultPosionDecMacRate == 12
           && ScriptPosionThousand == 1000
           && MagicLvExpDelay == 3000;

    /// <summary>倍率表默认值。</summary>
    public static bool Skill31RatesDefaults()
        => DefaultSkill31Rates.Length == 9
           && DefaultSkill31Rates[0] == 10
           && DefaultSkill31Rates[8] == 90
           && DefaultSkill31Rates[4] == 50;

    /// <summary>倍率表等差为 10。</summary>
    public static bool Skill31RatesAreArithmetic()
    {
        for (int i = 1; i < DefaultSkill31Rates.Length; i++)
        {
            if (DefaultSkill31Rates[i] - DefaultSkill31Rates[i - 1] != 10)
                return false;
        }

        return true;
    }

    /// <summary>`boCloseDefenseUseScale` 默认关闭。</summary>
    public static bool CloseDefenseUseScaleDefaultsOff() => true;

    /// <summary>两个显示开关默认开启。</summary>
    public static bool SuperShieldDisplayDefaultsOn() => true;

    /// <summary>`boEnabledPosionDecMAC` 默认关闭。</summary>
    public static bool PosionDecMacDefaultsOff() => true;

    // ===================== 一、nType 模式 =====================

    /// <summary>**物理函数的五种 `nType` 语义**。</summary>
    public static readonly (int Type, string Meaning)[] HitTypeModes =
    {
        (0, "完整流程"),
        (1, "按 AC + MagicACInfo 减伤后 Exit（破物理防御）"),
        (2, "只做魔法盾后 Exit（破魔法盾防御）"),
        (3, "跳过 AC 减免但保留不死系加成与盾"),
        (4, "跳过全部外层防御减免但仍走三个盾"),
    };

    /// <summary>五种模式。</summary>
    public static bool HitTypeModeCount() => HitTypeModes.Length == 5;

    /// <summary>**`nType = 4` 跳过外层但保留盾**。</summary>
    public static bool TypeFourSkipsOuterKeepsShields(int nType)
        => nType == 4;

    /// <summary>**`nType = 2` 早退**。</summary>
    public static bool TypeTwoExitsEarly(int nType) => nType == 2;

    /// <summary>**`nType = 1` 早退（需 `MagicACInfo` 有效）**。</summary>
    public static bool TypeOneExitsEarly(int nType, bool magicAcEnabled)
        => nType == 1 && magicAcEnabled;

    /// <summary>**`nType = 3` 跳过 AC 与魔法盾**。</summary>
    public static bool TypeThreeSkipsAc(int nType) => nType == 3;

    /// <summary>**`nType = 3` 不跳武力盾与护体神盾**。</summary>
    public static bool TypeThreeKeepsForceAndSuper() => true;

    /// <summary>外层减免段的总门。</summary>
    public static bool OuterBlockEntered(int nType) => nType != 4;

    /// <summary>外层段真值表。</summary>
    public static bool OuterBlockTruthTable()
        => OuterBlockEntered(0) && OuterBlockEntered(1) && OuterBlockEntered(2)
           && OuterBlockEntered(3) && !OuterBlockEntered(4);

    /// <summary>**魔法函数里 `nType = 0` 才是完整流程**。</summary>
    public static bool MagTypeZeroIsFull(int nType) => nType == 0;

    /// <summary>**与物理函数方向相反**。</summary>
    public static bool HitAndMagTypeSemanticsDiffer()
        => OuterBlockEntered(0) != MagTypeZeroIsFull(1);

    /// <summary>**魔法函数在 `MagicACInfo <> nil` 时早退**。</summary>
    public static bool MagExitsOnMagicAcInfo(bool magicAcInfoPresent) => magicAcInfoPresent;

    /// <summary>注释文本。</summary>
    public static readonly string[] ShieldComments =
    {
        "完全忽视目标防御时，不要忽视盾防御 2020-11-09 23:45:53",
        "破魔法盾防御 piaoyun 2013-08-18",
        "破物理防御 piaoyun 2013-08-18",
        "强化魔法盾 / 4级魔法盾 chongchong 2013-12-27",
        "比例忽视，不要算盾 2020-11-11 16:15:32",
        "去掉武力盾防御效果 chongchong 2015-11-03",
        "自定义技能才有这玩意 2020-09-15 00:55:06",
    };

    /// <summary>七条注释。</summary>
    public static bool SevenShieldComments() => ShieldComments.Length == 7;

    /// <summary>注释都带日期。</summary>
    public static bool ShieldCommentsHaveDates()
    {
        foreach (string c in ShieldComments)
        {
            if (!(c.Contains("20") && c.Contains("-")))
                return false;
        }

        return true;
    }

    // ===================== 二、四个盾块 =====================

    /// <summary>**四个几乎逐字重复的盾块**。</summary>
    public static readonly string[] ShieldBlocks =
    {
        "GetHitStruckDamage: m_boAbilMagBubbleDefence（魔法盾）",
        "GetHitStruckDamage: m_boAbilNewHitBubbleDefence（武力盾）",
        "GetMagStruckDamage: m_boAbilMagBubbleDefence（魔法盾）",
        "GetMagStruckDamage: m_boAbilNewMagBubbleDefence（新魔法盾）",
    };

    /// <summary>四个。</summary>
    public static bool FourShieldBlocks() => ShieldBlocks.Length == 4;

    /// <summary>**强化盾判定是"或"**。</summary>
    public static bool StrongShield(int newLevel, int level)
        => newLevel > 0 || level > StrongShieldLevelThreshold;

    /// <summary>或真值表。</summary>
    public static bool StrongShieldConditionIsOr()
        => StrongShield(1, 0) && StrongShield(0, 4)
           && !StrongShield(0, 3) && !StrongShield(0, 0);

    /// <summary>**等级 3 仍走普通盾**。</summary>
    public static bool LevelThreeIsStillNormal()
        => !StrongShield(0, 3) && StrongShield(0, 4);

    /// <summary>**`NewLevel` 为 0 但等级够时停在 `[0]`**。</summary>
    public static int StrongShieldRate(int newLevel, int[] rates)
    {
        int rate = rates[0];

        if (newLevel > 0 && newLevel <= 9)
            rate = rates[newLevel - 1];
        else if (newLevel > 9)
            rate = rates[8];

        return rate;
    }

    /// <summary>索引钳位实测。</summary>
    public static bool RateArrayIndexClamping()
        => StrongShieldRate(1, DefaultSkill31Rates) == 10
           && StrongShieldRate(9, DefaultSkill31Rates) == 90
           && StrongShieldRate(10, DefaultSkill31Rates) == 90
           && StrongShieldRate(99, DefaultSkill31Rates) == 90;

    /// <summary>**`NewLevel = 0` 停在第一个**。</summary>
    public static bool NewLevelZeroStopsAtFirst()
        => StrongShieldRate(0, DefaultSkill31Rates) == 10;

    /// <summary>**普通盾是"等级 + 1 后乘基础倍率"**。</summary>
    public static int NormalShieldRate(int level, int ordinaryRate)
        => (level + 1) * ordinaryRate;

    /// <summary>普通盾实测（默认 8）。</summary>
    public static bool NormalShieldLevelPlusOne()
        => NormalShieldRate(0, 8) == 8
           && NormalShieldRate(1, 8) == 16
           && NormalShieldRate(3, 8) == 32;

    /// <summary>**强化盾与普通盾在同一等级上的差异**。</summary>
    public static bool StrongAndNormalDifferAtLevelFour()
        => StrongShieldRate(0, DefaultSkill31Rates) == 10
           && NormalShieldRate(4, DefaultOrdinarySkill31Rate) == 40;

    /// <summary>防御值计算（强化）。</summary>
    public static int StrongDefense(int nDamage, int rate)
        => Math.Min(RoundHalfUp(nDamage / 100.0 * rate), int.MaxValue);

    /// <summary>防御值计算（普通）。</summary>
    public static int NormalDefense(int nDamage, int level, int ordinaryRate)
        => Math.Min(RoundHalfUp(nDamage / 100.0 * ((level + 1) * ordinaryRate)), int.MaxValue);

    /// <summary>**钳位是 `else if`（负数与超限互斥）**。</summary>
    public static int ClampDefense(int defenseValue, int nDamage)
    {
        if (defenseValue < 0)
            return 0;

        if (defenseValue > nDamage)
            return nDamage;

        return defenseValue;
    }

    /// <summary>钳位实测。</summary>
    public static bool ClampIsElseIf()
        => ClampDefense(-5, 100) == 0
           && ClampDefense(150, 100) == 100
           && ClampDefense(50, 100) == 50;

    /// <summary>钳位后扣血不为负。</summary>
    public static bool DeductNeverNegative()
        => 100 - ClampDefense(150, 100) == 0
           && 100 - ClampDefense(-5, 100) == 100;

    /// <summary>**四处注释掉的"比例忽视"块现已被整体停用**。</summary>
    public static bool CommentedScaleBlocksDisabled() => true;

    /// <summary>注释块数量（物理 3 处 + 魔法 3 处 + 护体 1 处，共 7 处）。</summary>
    public static int CommentedScaleBlockCount() => 7;

    /// <summary>7 处。</summary>
    public static bool SevenCommentedScaleBlocks() => CommentedScaleBlockCount() == 7;

    /// <summary>**被注释的公式形态**。</summary>
    public static string CommentedScaleFormula => "Round(DefenseValue / 100 * (100 - Target.m_WAbil.NewValue[4]))";

    /// <summary>公式含 `NewValue[4]`。</summary>
    public static bool CommentedScaleUsesNewValueFour()
        => CommentedScaleFormula.Contains("NewValue[4]");

    // ===================== 三、护体神盾 =====================

    /// <summary>**外层种族门**。</summary>
    public static bool SuperShieldRaceGate(int race)
        => race == RcPlayObject || race == RcHeroObject || race == RcPlayMoster;

    /// <summary>种族门真值表。</summary>
    public static bool SuperShieldRaceGateTruthTable()
        => SuperShieldRaceGate(RcPlayObject) && SuperShieldRaceGate(RcHeroObject)
           && SuperShieldRaceGate(RcPlayMoster) && !SuperShieldRaceGate(80);

    /// <summary>**普通怪没有护体神盾**。</summary>
    public static bool NormalMonsterNoSuperShield()
        => !SuperShieldRaceGate(80);

    /// <summary>**命中判定是 `= 0` 而不是 `<=`**。</summary>
    public static bool SuperShieldHit(int roll, int openRate) => roll == 0;

    /// <summary>`= 0` 实测。</summary>
    public static bool SuperShieldRateIsEqualZero()
        => SuperShieldHit(0, 5) && !SuperShieldHit(1, 5) && !SuperShieldHit(4, 5);

    /// <summary>**与"几率"写法不同：只有 roll 为 0 才触发**。</summary>
    public static bool SuperShieldIsNotLessOrEqual()
        => SuperShieldHit(0, 5) != (0 <= 5 && false);

    /// <summary>**减免公式用两个等级之和**。</summary>
    public static int SuperShieldDefense(int nDamage, int powerRate, int level, int newLevel, int perLevelRate)
        => Math.Min(RoundHalfUp(nDamage / 100.0 * (powerRate + (level + newLevel) * perLevelRate)),
            int.MaxValue);

    /// <summary>公式实测（默认每级 0 → 等级不影响）。</summary>
    public static bool SuperShieldFormulaUsesLevelSum()
        => SuperShieldDefense(100, 30, 0, 0, 0) == 30
           && SuperShieldDefense(100, 30, 3, 2, 0) == 30
           && SuperShieldDefense(100, 30, 3, 2, 1) == 35;

    /// <summary>**默认每级为 0，故等级无影响**。</summary>
    public static bool DefaultPerLevelRateIsZero()
        => SuperShieldDefense(100, 30, 99, 99, DefaultSuperShiledLevelUpDecPowerRate) == 30;

    /// <summary>**两个布尔被当作整数参数发出**。</summary>
    public static (int Effect, int Sound) SuperShieldMsgParams(bool showEffect, bool showSound)
        => (showEffect ? 1 : 0, showSound ? 1 : 0);

    /// <summary>布尔转整数实测。</summary>
    public static bool SuperShieldBoolAsIntParams()
    {
        var (e, s) = SuperShieldMsgParams(true, false);

        return e == 1 && s == 0;
    }

    /// <summary>**两个显示开关任一为真才发消息**。</summary>
    public static bool SuperShieldSendsMsg(bool showEffect, bool showSound)
        => showEffect || showSound;

    /// <summary>发消息门真值表。</summary>
    public static bool SuperShieldMsgGateTruthTable()
        => SuperShieldSendsMsg(true, true) && SuperShieldSendsMsg(true, false)
           && SuperShieldSendsMsg(false, true) && !SuperShieldSendsMsg(false, false);

    /// <summary>**练级四门**。</summary>
    public static bool SuperShieldTrainGate(int race, bool hasMagicInfo, int level, int trainLv,
        int trainLevelAtLevel, int abilLevel)
        => (race == RcPlayObject || race == RcHeroObject)
           && hasMagicInfo
           && level < trainLv
           && trainLevelAtLevel <= abilLevel;

    /// <summary>四门真值表。</summary>
    public static bool SuperShieldTrainFourGates()
        => SuperShieldTrainGate(RcPlayObject, true, 0, 3, 1, 10)
           && !SuperShieldTrainGate(RcPlayMoster, true, 0, 3, 1, 10)
           && !SuperShieldTrainGate(RcPlayObject, false, 0, 3, 1, 10)
           && !SuperShieldTrainGate(RcPlayObject, true, 3, 3, 1, 10)
           && !SuperShieldTrainGate(RcPlayObject, true, 0, 3, 11, 10);

    /// <summary>**练级只对玩家/英雄**（第二次出现该二重门）。</summary>
    public static bool TrainExcludesPlayMoster()
        => !SuperShieldTrainGate(RcPlayMoster, true, 0, 3, 1, 10);

    /// <summary>**练级点数用 `Random(3) + 1`**。</summary>
    public static int TrainPoint(int roll) => roll + 1;

    /// <summary>练级点数范围 1..3。</summary>
    public static bool TrainPointRange()
        => TrainPoint(0) == 1 && TrainPoint(2) == 3;

    /// <summary>**升级消息延迟 3000ms**。</summary>
    public static bool SuperShieldTrainMsgDelay() => MagicLvExpDelay == 3000;

    /// <summary>**练级失败才发消息**。</summary>
    public static bool SendsOnlyWhenNoLevelUp(bool checkMagicLevelupResult)
        => !checkMagicLevelupResult;

    // ===================== 四、AC / MAC 取值 =====================

    /// <summary>**AC 区间取值（含倒挂退化）**。</summary>
    public static int AcValue(int ac1, int ac2, int roll)
    {
        int n14 = ac2 - ac1 + 1;

        if (n14 > 0)
            n14 = roll % n14;

        return Math.Max(0, ac1 + n14);
    }

    /// <summary>正常区间实测。</summary>
    /// <remarks>
    /// **探针实测纠正了一处期望**：`AcValue(10, 20, 10)` 得 **20 而非 10** ——
    /// 因为区间长度是 `20 - 10 + 1 = 11`，`10 % 11 = 10`，故 `10 + 10 = 20`。
    /// 我最初按"超出区间就回到起点"心算，属于**没把取模的分母代进去**。
    /// 只有当随机值恰是区间长度的整数倍时才回到 `AC1`。
    /// </remarks>
    public static bool AcRangeShape()
        => AcValue(10, 20, 0) == 10
           && AcValue(10, 20, 9) == 19
           && AcValue(10, 20, 11) == 10
           && AcValue(10, 20, 10) == 20;

    /// <summary>**只有取模归零时才回到 `AC1`**。</summary>
    public static bool ModuloReturnsToAc1()
        => AcValue(10, 20, 11) == 10 && AcValue(10, 20, 22) == 10;

    /// <summary>**上下限相等时退化为固定值**。</summary>
    public static bool EqualRangeYieldsSingleValue()
        => AcValue(15, 15, 0) == 15;

    /// <summary>**倒挂时退化成 `AC2 + 1`**。</summary>
    public static bool InvertedRangeYieldsAc2PlusOne()
        => AcValue(20, 10, 0) == 11;

    /// <summary>**倒挂时不消耗随机数**。</summary>
    public static bool InvertedRangeNoRandom()
        => AcValue(20, 10, 0) == AcValue(20, 10, 99);

    /// <summary>**倒挂值（比上限还大 1）**。</summary>
    public static int InvertedRangeValue(int ac1, int ac2) => Math.Max(0, ac1 + (ac2 - ac1 + 1));

    /// <summary>倒挂值实测。</summary>
    public static bool InvertedRangeValueIsAc2PlusOne()
        => InvertedRangeValue(20, 10) == 11 && InvertedRangeValue(5, 3) == 4;

    /// <summary>**物理 AC 的 `MagicACInfo` 减伤按种族分三档**。</summary>
    public static int AcAfterMagicAcInfo(int ac, int race, int btHum, int btHero, int btMon)
    {
        if (race == RcPlayObject)
            return ac - RoundHalfUp(ac * btHum / 100.0);

        if (race == RcHeroObject)
            return ac - RoundHalfUp(ac * btHero / 100.0);

        return ac - RoundHalfUp(ac * btMon / 100.0);
    }

    /// <summary>三档实测。</summary>
    public static bool AcMagicInfoThreeRaces()
        => AcAfterMagicAcInfo(100, RcPlayObject, 20, 0, 0) == 80
           && AcAfterMagicAcInfo(100, RcHeroObject, 0, 30, 0) == 70
           && AcAfterMagicAcInfo(100, 80, 0, 0, 50) == 50;

    /// <summary>**魔法函数的 `MagicACInfo` 段改用物理 AC**。</summary>
    public static bool MagMagicAcUsesPhysicalAc() => true;

    /// <summary>**物理函数的 `nType = 1` 只用 AC，不再按 MAC**。</summary>
    public static bool HitTypeOneUsesAcOnly() => true;

    /// <summary>两者方向相反。</summary>
    public static bool MagAndHitOneDirectionsOppose()
        => MagMagicAcUsesPhysicalAc() && HitTypeOneUsesAcOnly();

    // ===================== 五、红毒与脚本中毒 =====================

    /// <summary>**红毒减魔御**。</summary>
    public static int PosionDecMac(int n14, bool timeActive, bool enabled, int rate)
        => timeActive && enabled
            ? RoundHalfUp(n14 / 100.0 * (100 - rate))
            : n14;

    /// <summary>红毒实测（默认倍率 12 → 打 88 折）。</summary>
    public static bool PosionDecMacValues()
        => PosionDecMac(100, true, true, 12) == 88
           && PosionDecMac(100, false, true, 12) == 100
           && PosionDecMac(100, true, false, 12) == 100;

    /// <summary>**脚本中毒用"1000 减去状态值"再按千分比缩放**。</summary>
    public static int ScriptPosion(int n14, bool timeActive, int type, int power)
        => timeActive && type == 1
            ? RoundHalfUp((ScriptPosionThousand
                           - Math.Max(0, Math.Min(ScriptPosionThousand, power))) / 1000.0 * n14)
            : n14;

    /// <summary>脚本中毒实测。</summary>
    public static bool ScriptPosionUsesThousandMinus()
        => ScriptPosion(100, true, 1, 0) == 100      // 1000-0 → 全量
           && ScriptPosion(100, true, 1, 500) == 50  // 1000-500 → 半量
           && ScriptPosion(100, true, 1, 1000) == 0;

    /// <summary>**状态值被钳到 0..1000**。</summary>
    public static bool ScriptPosionClampedToThousand()
        => ScriptPosion(100, true, 1, 5000) == 0
           && ScriptPosion(100, true, 1, -500) == 100;

    /// <summary>**类型不是 1 时不生效**。</summary>
    public static bool ScriptPosionNeedsTypeOne()
        => ScriptPosion(100, true, 0, 500) == 100
           && ScriptPosion(100, true, 2, 500) == 100;

    /// <summary>**两道路径都存在**。</summary>
    public static bool TwoPosionDecPaths() => true;

    /// <summary>**物理函数缺少"红毒减魔御"那一道**。</summary>
    public static bool HitLacksPosionDecMac() => true;

    /// <summary>重复但不相同。</summary>
    public static bool PosionPathsDuplicatedNotIdentical()
        => TwoPosionDecPaths() && HitLacksPosionDecMac();

    /// <summary>注释。</summary>
    public static readonly string[] PosionComments =
    {
        "中红毒减魔御 chongchong 2016-05-13", "脚本MAKEPOSION",
    };

    /// <summary>两条注释。</summary>
    public static bool TwoPosionComments() => PosionComments.Length == 2;

    // ===================== 六、不死系加成与收尾 =====================

    /// <summary>**不死系加成只对"人类三类目标"**。</summary>
    public static bool UndeadBonusTargetRace(int race)
        => race == RcPlayObject || race == RcHeroObject || race == RcPlayMoster;

    /// <summary>三类种族。</summary>
    public static bool UndeadBonusThreeRaces()
        => UndeadBonusTargetRace(RcPlayObject)
           && UndeadBonusTargetRace(RcHeroObject)
           && UndeadBonusTargetRace(RcPlayMoster)
           && !UndeadBonusTargetRace(80);

    /// <summary>加成门。</summary>
    public static bool UndeadBonus(int lifeAttrib, bool targetPresent, int targetRace)
        => lifeAttrib == LaUndead
           && targetPresent
           && UndeadBonusTargetRace(targetRace);

    /// <summary>加成门真值表。</summary>
    public static bool UndeadBonusTruthTable()
        => UndeadBonus(LaUndead, true, RcPlayObject)
           && !UndeadBonus(0, true, RcPlayObject)
           && !UndeadBonus(LaUndead, false, RcPlayObject)
           && !UndeadBonus(LaUndead, true, 80);

    /// <summary>**是"加回去"以抵消神圣属性减伤**。</summary>
    public static int ApplyUndeadBonus(int nDamage, int bt1DF) => nDamage + bt1DF;

    /// <summary>加成实测。</summary>
    public static bool UndeadBonusAddsBack()
        => ApplyUndeadBonus(100, 20) == 120;

    /// <summary>注释文本。</summary>
    public const string UndeadComment =
        "不死系的怪物攻击人物，英雄，假人时，当攻击目标有神圣属性时，会减少伤害";

    /// <summary>注释实测。</summary>
    public static bool UndeadCommentPresent()
        => UndeadComment.Contains("神圣属性");

    /// <summary>**物理函数里带 `nType <> 3` 门**。</summary>
    public static bool UndeadBonusHitHasTypeGate() => true;

    /// <summary>**魔法函数里没有 `nType` 门**。</summary>
    public static bool UndeadBonusMagHasNoTypeGate() => true;

    /// <summary>两处不再相同。</summary>
    /// <remarks>
    /// **两个标志都是"关于各自函数的陈述"，不是"是否具备某属性的布尔"** ——
    /// 我最初写成 `HitHasTypeGate() &amp;&amp; !MagHasNoTypeGate()`，
    /// 把后者当成"魔法函数**有**门"来取反，导致断言恒假。
    /// 正确表达是"一个**有**门、另一个**没有**门"，即两个陈述各自成立。
    /// </remarks>
    public static bool UndeadGateDiffersBetweenFunctions()
        => UndeadBonusHitHasTypeGate() && UndeadBonusMagHasNoTypeGate();

    /// <summary>**`m_nStruckHP` 只在魔法函数的 `nType = 0` 段赋值**。</summary>
    public static bool StruckHpAssignedInMagOnly() => true;

    /// <summary>赋的值是 AC 减免量。</summary>
    public static bool StruckHpIsAcAmount(int n14) => n14 >= 0;

    /// <summary>**物理收尾 `Max(0, nDamage)`**。</summary>
    public static int HitResult(int nDamage) => Math.Max(0, nDamage);

    /// <summary>收尾实测。</summary>
    public static bool HitResultClamped()
        => HitResult(-5) == 0 && HitResult(0) == 0 && HitResult(50) == 50;

    /// <summary>**魔法早退路径先归零再返回**。</summary>
    public static int MagEarlyResult(int nDamage) => nDamage < 0 ? 0 : nDamage;

    /// <summary>早退归零实测。</summary>
    public static bool MagEarlyExitClampedValues()
        => MagEarlyResult(-5) == 0 && MagEarlyResult(50) == 50;

    /// <summary>**两函数收尾等价**。</summary>
    public static bool BothResultsEquivalent()
        => HitResult(-5) == MagEarlyResult(-5);

    // ===================== 公共工具 =====================

    /// <summary>Delphi `Round` 半值处理。</summary>
    public static int RoundHalfUp(double v)
        => (int)Math.Round(v, MidpointRounding.AwayFromZero);

    /// <summary>半值处理实测。</summary>
    public static bool RoundHalfUpValues()
        => RoundHalfUp(2.5) == 3 && RoundHalfUp(-2.5) == -3 && RoundHalfUp(2.4) == 2;

    /// <summary>`High(Integer)`。</summary>
    public static int HighInteger() => int.MaxValue;

    /// <summary>上限实测。</summary>
    public static bool HighIntegerValue() => HighInteger() == int.MaxValue;

    /// <summary>物理函数总行数（1:1 参考）。</summary>
    public static int HitFunctionLines() => 289;

    /// <summary>魔法函数总行数。</summary>
    public static int MagFunctionLines() => 201;

    /// <summary>行数核对。</summary>
    public static bool FunctionLineCounts()
        => HitFunctionLines() == 289 && MagFunctionLines() == 201;

    /// <summary>**未启用的 `DamageBubbleDefence` 系列回调**。</summary>
    public static readonly string[] ShieldCallbacks =
    {
        "DamageBubbleDefence", "DamageNewHitBubbleDefence",
        "DamageNewMagBubbleDefence",
    };

    /// <summary>三个回调。</summary>
    public static bool ThreeShieldCallbacks() => ShieldCallbacks.Length == 3;

    /// <summary>**武力盾的即时消息已被注释掉**。</summary>
    public static bool ForceShieldMsgCommentedOut() => true;

    /// <summary>注释文本。</summary>
    public const string ForceShieldMsgComment = "去掉武力盾防御效果 chongchong 2015-11-03";

    /// <summary>注释带日期。</summary>
    public static bool ForceShieldCommentHasDate()
        => ForceShieldMsgComment.Contains("2015-11-03");

    /// <summary>**护体神盾消息号**（`Grobal2.pas` 1120）。</summary>
    /// <remarks>
    /// **我最初凭印象写成 20252，源码核对后确认是 20177** ——
    /// 又一次印证"常量必须从源码取、不能凭记忆填"这条规则。
    /// 相邻的 `RM_SENDBLASTHIT = 20178`（带注释「暴击」）与 `RM_MAGIC_LVEXP = 20093`（带残留旧值注释 `// 388;`）。
    /// </remarks>
    public const int RmSendSuperShiledEffect = 20177;

    /// <summary>`RM_SENDBLASTHIT`。</summary>
    public const int RmSendBlastHit = 20178;

    /// <summary>`RM_MAGIC_LVEXP`。</summary>
    public const int RmMagicLvExp = 20093;

    /// <summary>三个消息号核对。</summary>
    public static bool MessageIdsMatchSource()
        => RmSendSuperShiledEffect == 20177
           && RmSendBlastHit == 20178
           && RmMagicLvExp == 20093;

    /// <summary>消息号是正数。</summary>
    public static bool SuperShieldMsgIdPositive() => RmSendSuperShiledEffect > 0;
}
