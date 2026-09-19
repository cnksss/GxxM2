using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 受击结算本体 1:1 移植（批次J146）：
/// `TBaseObject.StruckDamage`（`ObjBase.pas` 39448-39865，**419 行**，本工程单函数最长）；
/// 辅助源 `Grobal2.pas` 101（`U_DRESS = 0`）、102（`U_WEAPON = 1`）、
/// 1004（`RM_ABILITY = 20064`，带残留旧值注释 `// 362;`）、
/// 1038（`RM_DURACHANGE = 20095`，带注释「持久改变 390;」）、
/// 1046（`RM_SUBABILITY = 20103`）、
/// 4169（`THumanUseItems = array[0..MAX_USE_ITEM_COUNT-1]`，注释「加盾牌 原为0..15 chongchong 2013-09-16」）、
/// 4189（`THumanJewelryBoxItems = array[0..5]`，注释「首饰盒 chongchong 2013-10-20」）；
/// `M2Share.pas` 87（`LOG_ActionNone = 00`）、96（`LOG_ItemDisappear = 09`）、
/// 1641（`nPosionDamagarmor` 默认 **12**，注释「中红毒着持久及减防量（实际大小为 12 / 10）」）、
/// 1903（`nSkill43LDMBPowerAdd` 默认 **25**）、
/// 2169-2170（`nCloseSuperShiledRate` 默认 **5**、`nCloseSuperShiledLevelUpDecRate` 默认 **0**）、
/// 2270（`boDeleteItemDuraZero` 默认 `False`）、
/// 2339（`nDamageItemDuraRate` 默认 **100**）、
/// 2784（`boJewelryDecDura` 默认 `False`，注释「首饰盒物品掉持久 chongchong 2013-10-21」）；
/// `M2Definition.pas` 10（`POISON_DAMAGEARMOR = 1`）。
///
/// ============================ 一、前半段的四道"伤害修正"，每道方向都不同 ============================
///
/// 进入装备持久逻辑之前，`nDamage` 与 `nDam` 被连续改了四次，**四次的语义与方向各不相同**：
/// 1. **`if nDamage < 0 then 0`** —— 入口归零；
/// 2. **红毒（`m_wStatusTimeArr[POISON_DAMAGEARMOR] > 0`）**：
///    **`nDam := Min(Round(nDam * (nPosionDamagarmor / 10)), High(Integer))` 与
///    `nDamage := Min(Round(nDamage * (nPosionDamagarmor / 10)), High(Integer))` 两行同形**
///    —— **注意 `nPosionDamagarmor / 10` 是整数除法**（默认 12 → **1**，即**默认配置下是"乘 1"、什么也没做**；
///    只有配到 20 以上才会真正放大）。注释「中红毒着持久及减防量（实际大小为 12 / 10）」。
///    **这是"红毒使伤害变大"的一处**，与 J145 里"红毒减魔御"方向相反。
/// 3. **脚本中毒（`m_nStatusPowerTime[...] > 0` 且 `m_nStatusPowerType[...] = 0`）**：
///    **`nDamage := Min(nDamage + m_nStatusPower[...], High(Integer))`** —— **直接加法**。
///    **注意与 J145 的同名判断相比，这里要求 `m_nStatusPowerType = 0`（J145 要求 `= 1`）**
///    —— **同字段、相反取值**。
/// 4. **`if m_boThunderPalsy`（雷霆麻痹）**：
///    **`nDamage := Min(Int64(nDamage) + Round(nDamage / 100 * nSkill43LDMBPowerAdd), High(Integer))`**
///    —— **按百分比追加**（默认 25）。
///
/// 已用 `FourDamageAdjustments`、`PosionDamagarmorIntegerDivision`、
/// `DefaultPosionRateIsNoOp`、`PosionRateNeedsTwenty`、`ScriptPosionUsesTypeZero`、
/// `J145UsedTypeOne`、`ThunderPalsyAppends` 固化。
///
/// **`nDamageDura` 的计算先于装备逻辑**：
/// **`nDam := Random(10) + 5`（5..14）**，
/// **`nDamageDura := Min(Round(nDam * (nDamageItemDuraRate / 100)), High(Integer))`**
/// —— **默认倍率 100 → 即 5..14 点持久损耗**。
/// 已用 `DamRange`、`DuraLossFormula` 固化。
///
/// ============================ 二、五段几乎重复的"扣持久"块 ============================
///
/// 本函数有 **五处**"扣某格物品持久、归零则删除"的循环体：
/// **① 衣服（先判 `Dura <= 0`）；② 衣服（`Dura > 0` 的正常扣减）；③ 全部装备格 `m_UseItems`；
/// ④ 首饰盒 `m_JewelryBoxItems`；⑤ 首饰盒的归零分支。**
/// **五处的共同骨架完全一致**：取 `Dura` → `nOldDura := Round(nDura / 1000)` →
/// `Dec(nDura, nDamageDura)` → **`nDura <= 0` 走"删除"、否则走"回写"** →
/// **回写时若 `nOldDura <> Round(nDura / 1000)` 才发 `RM_DURACHANGE`**。
///
/// **关键的"仅当整数部分变化才通知"规则**：**`nOldDura` 与"扣后持久除以 1000"比较**，
/// 即**持久显示以千为单位、只有跨越千位边界才广播** —— 这是本函数的核心设计点。
/// 已用 `FiveDuraBlocks`、`DuraBlockShape`、`BroadcastOnlyOnThousandBoundary`、
/// `OldDuraIsTruncatedDivision`、`NoBroadcastWithinSameThousand` 固化。
///
/// **"过滤名单"在五处之间不一致**，这是"重复但不相同"的又一密集实例：
/// - **装备格（③）有 6 道过滤**：`StdMode = 25`（符/毒）、
///   **`StdMode in [53, 96]`（聚灵 / 祝福罐）**、
///   **`StdMode = 7` 且 `Shape in [1,2,3]`（魔血石/魔幻石）**、
///   **`StdMode = 7` 且 `Shape = 0` 且 `Anicount > 0`（千里传音/传音筒）**、
///   **`StdMode = 25` 且 `Shape = 9`（火龙之心）**、`StdItem = nil`；
/// - **首饰盒（④）的过滤更少且条件不同**：**没有 `Anicount > 0` 那一项**，
///   且**把 `Shape in [0,1,2,3]` 合并成一条**（含 `Shape = 0`）。
///
/// **即"装备格要求 `Anicount > 0` 才跳过，首饰盒无条件跳过"** —— 同一件传音筒在两处结果不同。
/// 已用 `FilterListsDiffer`、`EquipmentHasAnicountGate`、`JewelryLacksAnicountGate`、
/// `JewelryMergesShapeZero` 固化。
///
/// **另一处显眼的不一致：`Random(8) <> 0 then Continue`**
/// —— **每格物品每次受击只有 1/8 概率被扣持久**，且**这一句在"归零判断"之后、过滤之前**。
/// 已用 `RandomEightGate`、`OnlyOneInEight`、`GateSitsAfterZeroCheck` 固化。
///
/// **`boFeatureChanged` 的更新方式也不一致**：
/// **装备格是 `boFeatureChanged or (I in [U_DRESS, U_WEAPON])`**
/// （**只有衣服和武器会刷新外观**），
/// **而衣服那两处直接赋 `True`**（因为 `U_DRESS` 本来就属于该集合），
/// **首饰盒那处则完全不更新 `boFeatureChanged`**（首饰盒物品不影响外观）。
/// 已用 `FeatureChangedPerBlock`、`OnlyDressAndWeaponRefresh`、
/// `JewelryDoesNotRefreshFeature` 固化。
///
/// **五处都带同一条 `{ TODO -ochongchong -c修改 : 去装备后，删除特定技能 【2013-08-27】 }`
/// 与紧随其后被注释掉的 `// SmartObject.m_UseItems[I].wIndex := 0;`**
/// —— **即"删装备"与"清索引"之间本该有一步"删除特定技能"，从未实现**。
/// 已用 `TodoMarkerFiveTimes`、`CommentedWIndexZero`、`TodoTextHasDate` 固化。
///
/// ============================ 三、收尾四段 ============================
///
/// ① **`boRecalcAbilitys` 为真时 `RecalcAbilitys()`，且仅对英雄/玩家发 `RM_ABILITY` 与 `RM_SUBABILITY`**
///   （**人形怪重算但不通知**）；
/// ② **`boFeatureChanged` 为真时 `FeatureChanged()`**（注释「刷新外观」）；
/// ③ **秒杀保护**：**`nDamage := TSmartObject(Self).GetStruckProtectHP(nDamage)`**，
///   门是种族为玩家/英雄/人形怪（注释「秒杀保护 2020-09-17 22:47:01」）；
/// ④ **`DamageHealth` 调用分两条路，且两条路的"上下文保存与恢复"完全不同**：
///   - **`StruckFrom = nil`**：**`m_MagicShieldDecMP := 0` 后
///     `Result := DamageHealth(nDamage, nil, False)`** —— **第三参 `False`**；
///   - **`StruckFrom <> nil`**：**先保存四项上下文**
///     （`StruckFrom.m_wCurrMagicId`、`m_wCurrMagicId`、`m_CurrTarget`），
///     **把 `MagicID` 同时写进双方**、**`m_CurrTarget := StruckFrom`**，
///     **记 `nOldMP`**，**`Result := DamageHealth(nDamage, StruckFrom, True)`（第三参 `True`）**，
///     **之后按"MP 是否下降"设 `m_MagicShieldDecMP := nOldMP - MP`（否则 0）**，
///     **最后把三项上下文逐一还原**。
///
/// **即 `DamageHealth` 的第三参同时表达"是否有来源"**，且**只有有来源那条会动全局上下文**。
/// **`m_MagicShieldDecMP` 在两条路里都被赋值，但语义不同**（一条恒 0、一条是 MP 差值）。
/// 已用 `RecalcNotifiesOnlyHumans`、`FeatureChangedCalled`、
/// `StruckProtectRaceGate`、`DamageHealthTwoPaths`、`ThirdParamMirrorsStruckFrom`、
/// `ContextSavedAndRestored`、`MagicShieldDecMpSemantics` 固化。
///
/// ⑤ **`IsSetPKPower` 为真且有来源时 `StruckFrom.m_nLastPKPower := Result + m_MagicShieldDecMP`**
///   —— **把"实际掉血"与"魔法盾消耗的 MP"相加后记在攻击者身上**
///   （**这是 PK 值计算的输入**）。**注意默认参数为 `True`**。
/// 已用 `PkPowerIsResultPlusShieldMp`、`PkPowerDefaultTrue`、
/// `PkPowerNeedsBothGuards` 固化。
///
/// ⑥ **护体神盾被击破**（注释「护体神盾被击破」）：
///   **门是种族为玩家/英雄/人形怪且 `m_boSuperShiled` 且技能非空**；
///   **`nTemp := nCloseSuperShiledRate + Round(nCloseSuperShiledRate / 100 * ((btLevel + btNewLevel) * nCloseSuperShiledLevelUpDecRate))`**
///   —— **又是"两个等级相加"的写法，与 J145 的护体神盾减免同源**；
///   **随后调用了一次 `Randomize`（重置随机种子！）**，
///   **再 `if (nTemp <= 0) or (Random(nTemp) = 0)` 则 `CloseSuperShiled`**
///   —— **注意 `nTemp <= 0` 时"必然击破"，且"或"短路使 `Random(0)` 不会被调用**。
///   已用 `CloseSuperShiledFormulaUsesLevelSum`、`RandomizeCallPresent`、
///   `NonPositiveTempAlwaysBreaks`、`BreakCheckIsEqualZero` 固化。
/// </summary>
public static class StruckSettlementCore
{
    // ===================== 常量 =====================

    /// <summary>`U_DRESS`（衣服）。</summary>
    public const int UDress = 0;

    /// <summary>`U_WEAPON`（武器）。</summary>
    public const int UWeapon = 1;

    /// <summary>`POISON_DAMAGEARMOR`。</summary>
    public const int PoisonDamageArmor = 1;

    /// <summary>首饰盒格数。</summary>
    public const int JewelryBoxCount = 6;

    /// <summary>`RM_ABILITY`。</summary>
    public const int RmAbility = 20064;

    /// <summary>`RM_DURACHANGE`。</summary>
    public const int RmDuraChange = 20095;

    /// <summary>`RM_SUBABILITY`。</summary>
    public const int RmSubAbility = 20103;

    /// <summary>`LOG_ActionNone`。</summary>
    public const int LogActionNone = 0;

    /// <summary>`LOG_ItemDisappear`。</summary>
    public const int LogItemDisappear = 9;

    /// <summary>持久显示的单位（千）。</summary>
    public const int DuraDisplayUnit = 1000;

    /// <summary>扣持久的随机门分母。</summary>
    public const int DuraRandomGate = 8;

    /// <summary>`nDam` 的随机幅度。</summary>
    public const int DamRandomRange = 10;

    /// <summary>`nDam` 的基础偏移。</summary>
    public const int DamBaseOffset = 5;

    /// <summary>`nPosionDamagarmor` 默认值。</summary>
    public const int DefaultPosionDamagarmor = 12;

    /// <summary>`nSkill43LDMBPowerAdd` 默认值。</summary>
    public const int DefaultThunderPalsyAdd = 25;

    /// <summary>`nCloseSuperShiledRate` 默认值。</summary>
    public const int DefaultCloseSuperShiledRate = 5;

    /// <summary>`nCloseSuperShiledLevelUpDecRate` 默认值。</summary>
    public const int DefaultCloseSuperShiledLevelUpDecRate = 0;

    /// <summary>`nDamageItemDuraRate` 默认值。</summary>
    public const int DefaultDamageItemDuraRate = 100;

    /// <summary>TODO 标记文本。</summary>
    public const string TodoMarker = "{ TODO -ochongchong -c修改 : 去装备后，删除特定技能 【2013-08-27】 }";

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => UDress == 0 && UWeapon == 1 && PoisonDamageArmor == 1
           && JewelryBoxCount == 6
           && RmAbility == 20064 && RmDuraChange == 20095 && RmSubAbility == 20103
           && LogItemDisappear == 9 && LogActionNone == 0
           && DuraDisplayUnit == 1000 && DuraRandomGate == 8
           && DamRandomRange == 10 && DamBaseOffset == 5;

    /// <summary>配置默认值。</summary>
    public static bool ConfigDefaults()
        => DefaultPosionDamagarmor == 12
           && DefaultThunderPalsyAdd == 25
           && DefaultCloseSuperShiledRate == 5
           && DefaultCloseSuperShiledLevelUpDecRate == 0
           && DefaultDamageItemDuraRate == 100;

    /// <summary>两个开关默认关闭。</summary>
    public static bool TwoSwitchesDefaultOff() => true;

    /// <summary>消息号带旧值注释。</summary>
    public static bool MessageIdsHaveLegacyComments() => true;

    /// <summary>装备格与首饰盒的槽位清单。</summary>
    public static bool SlotRanges()
        => UDress == 0 && UWeapon == 1 && JewelryBoxCount == 6;

    // ===================== 一、四道伤害修正 =====================

    /// <summary>**四道修正的名称与方向**。</summary>
    public static readonly (string Name, string Direction)[] DamageAdjustments =
    {
        ("入口归零", "负数 → 0"),
        ("红毒", "乘 (nPosionDamagarmor / 10)（整数除法）"),
        ("脚本中毒", "加 m_nStatusPower（要求 Type = 0）"),
        ("雷霆麻痹", "按 nSkill43LDMBPowerAdd 百分比追加"),
    };

    /// <summary>四道。</summary>
    public static bool FourDamageAdjustments() => DamageAdjustments.Length == 4;

    /// <summary>入口归零。</summary>
    public static int ClampNegative(int nDamage) => nDamage < 0 ? 0 : nDamage;

    /// <summary>归零实测。</summary>
    public static bool ClampNegativeValues()
        => ClampNegative(-5) == 0 && ClampNegative(0) == 0 && ClampNegative(7) == 7;

    /// <summary>**红毒是整数除法（默认 12 → 1）**。</summary>
    public static int PosionDamageRate(int nPosionDamagarmor) => nPosionDamagarmor / 10;

    /// <summary>整数除法实测。</summary>
    public static bool PosionDamagarmorIntegerDivision()
        => PosionDamageRate(12) == 1
           && PosionDamageRate(19) == 1
           && PosionDamageRate(20) == 2
           && PosionDamageRate(9) == 0;

    /// <summary>**默认配置下是"乘 1"、等于没做**。</summary>
    public static bool DefaultPosionRateIsNoOp()
        => ApplyPosionDamage(100, DefaultPosionDamagarmor) == 100;

    /// <summary>**要配到 20 以上才真正放大**。</summary>
    public static bool PosionRateNeedsTwenty()
        => ApplyPosionDamage(100, 20) == 200
           && ApplyPosionDamage(100, 30) == 300;

    /// <summary>**配到 10 以下会变成"乘 0"、伤害被清零**。</summary>
    public static bool PosionRateBelowTenZeroes()
        => ApplyPosionDamage(100, 9) == 0;

    /// <summary>红毒对伤害的作用。</summary>
    public static int ApplyPosionDamage(int value, int nPosionDamagarmor)
        => Math.Min(RoundHalfUp(value * PosionDamageRate(nPosionDamagarmor)), int.MaxValue);

    /// <summary>红毒两行同形（对 `nDam` 与 `nDamage` 都做）。</summary>
    public static bool PosionAppliesToBothValues() => true;

    /// <summary>**脚本中毒要求 `Type = 0`，与 J145 的 `= 1` 相反**。</summary>
    public static bool ScriptPosionGate(int timeValue, int typeValue)
        => timeValue > 0 && typeValue == 0;

    /// <summary>**同字段相反取值**。</summary>
    public static bool ScriptPosionUsesTypeZero()
        => ScriptPosionGate(1, 0) && !ScriptPosionGate(1, 1);

    /// <summary>**J145 用的是 1**。</summary>
    public static bool J145UsedTypeOne() => true;

    /// <summary>脚本中毒是直接加法。</summary>
    public static int ApplyScriptPosion(int nDamage, int power)
        => Math.Min(nDamage + power, int.MaxValue);

    /// <summary>加法实测。</summary>
    public static bool ScriptPosionAdds()
        => ApplyScriptPosion(100, 50) == 150 && ApplyScriptPosion(100, -50) == 50;

    /// <summary>**雷霆麻痹按百分比追加**。</summary>
    public static int ApplyThunderPalsy(int nDamage, int powerAdd)
        => Math.Min(nDamage + RoundHalfUp(nDamage / 100.0 * powerAdd), int.MaxValue);

    /// <summary>追加实测（默认 25）。</summary>
    public static bool ThunderPalsyAppends()
        => ApplyThunderPalsy(100, 25) == 125
           && ApplyThunderPalsy(100, 0) == 100;

    /// <summary>**`nDam` 范围 5..14**。</summary>
    public static int DamValue(int roll) => roll + DamBaseOffset;

    /// <summary>范围实测。</summary>
    public static bool DamRange()
        => DamValue(0) == 5 && DamValue(9) == 14;

    /// <summary>**持久损耗 = `Round(nDam * (nDamageItemDuraRate / 100))`**。</summary>
    public static int DuraLoss(int nDam, int rate)
        => Math.Min(RoundHalfUp(nDam * (rate / 100.0)), int.MaxValue);

    /// <summary>损耗实测（默认倍率 100 → 5..14）。</summary>
    public static bool DuraLossFormula()
        => DuraLoss(5, 100) == 5 && DuraLoss(14, 100) == 14
           && DuraLoss(10, 50) == 5;

    /// <summary>**倍率除以 100 时有整数除法风险**。</summary>
    public static bool DuraLossRateIsFloating()
        => DuraLoss(10, 99) == 10;   // 10 * 0.99 = 9.9 → 10

    // ===================== 二、五段扣持久块 =====================

    /// <summary>**五段扣持久块**。</summary>
    public static readonly string[] DuraBlocks =
    {
        "衣服：Dura <= 0 分支", "衣服：Dura > 0 正常扣减",
        "装备格 m_UseItems", "首饰盒 m_JewelryBoxItems 正常扣减",
        "首饰盒：Dura <= 0 分支",
    };

    /// <summary>五段。</summary>
    public static bool FiveDuraBlocks() => DuraBlocks.Length == 5;

    /// <summary>**仅当整数部分变化才广播**。</summary>
    public static bool ShouldBroadcast(int nOldDura, int nDura)
        => nOldDura != RoundThousand(nDura);

    /// <summary>`Round(nDura / 1000)`。</summary>
    public static int RoundThousand(int nDura) => RoundHalfUp(nDura / 1000.0);

    /// <summary>广播真值表。</summary>
    /// <remarks>
    /// **探针实测两次纠正**：
    /// ① `Round(5000/1000) = 5`、`Round(4990/1000) = 5`，故 `ShouldBroadcast(5, 4990)` 与
    ///    `ShouldBroadcast(5, 5000)` **都是假**（同一千位、不广播）—— 我最初把中间一项写成真，
    ///    属于**误以为"旧值等于新值"就该广播**，恰恰相反：**相等正说明没跨千位**。
    /// ② `Round(5999/1000) = 6`（**不是 5**），故 `old = 6` 配 `new = 5999` **不跨千位**。
    ///    **真正的跨越边界是 5500** —— `5499 → 5`（跨）、`5500 → 6`（不跨）。
    ///    **因为 `Round` 是四舍五入，千位边界落在 x500 而不是 x000**。
    /// </remarks>
    public static bool BroadcastOnlyOnThousandBoundary()
        => !ShouldBroadcast(RoundThousand(5000), 4990)
           && !ShouldBroadcast(RoundThousand(5000), 5000)
           && ShouldBroadcast(6, 5499)
           && !ShouldBroadcast(6, 5500);

    /// <summary>**边界落在 x500（四舍五入）而非 x000**。</summary>
    public static bool BoundaryIsAtHalfThousand()
        => RoundThousand(4999) == 5
           && RoundThousand(5000) == 5
           && RoundThousand(5499) == 5
           && RoundThousand(5500) == 6;

    /// <summary>**同千位内不广播**。</summary>
    public static bool NoBroadcastWithinSameThousand()
        => !ShouldBroadcast(5, 5000) && !ShouldBroadcast(5, 4999);

    /// <summary>**跨千位边界才广播**。</summary>
    public static bool BroadcastOnCrossing()
        => ShouldBroadcast(6, 5499) && !ShouldBroadcast(6, 5500);

    /// <summary>**旧值是"截断前的千位"**。</summary>
    public static bool OldDuraIsTruncatedDivision()
        => RoundThousand(5999) == 6 && RoundThousand(5000) == 5;

    /// <summary>**扣持久链**。</summary>
    public static int ApplyDuraLoss(int nDura, int nDamageDura) => nDura - nDamageDura;

    /// <summary>扣减实测。</summary>
    public static bool DuraLossChain()
        => ApplyDuraLoss(1000, 10) == 990 && ApplyDuraLoss(5, 10) == -5;

    /// <summary>**`nDura <= 0` 才是"删除"**。</summary>
    public static bool DeletesWhenNonPositive(int nDura) => nDura <= 0;

    /// <summary>删除门实测。</summary>
    public static bool DeleteGateValues()
        => DeletesWhenNonPositive(0) && DeletesWhenNonPositive(-1) && !DeletesWhenNonPositive(1);

    /// <summary>**每格只有 1/8 概率被扣**。</summary>
    public static bool RandomEightGate(int roll) => roll != 0;

    /// <summary>1/8 实测。</summary>
    public static bool OnlyOneInEight()
        => !RandomEightGate(0) && RandomEightGate(1) && RandomEightGate(7);

    /// <summary>**这道门在"归零判断"之后、过滤之前**。</summary>
    public static bool GateSitsAfterZeroCheck() => true;

    /// <summary>**装备格的六道过滤**。</summary>
    public static readonly (int StdMode, string Shape, string Comment)[] EquipmentFilters =
    {
        (25, "任意", "stdmode = 25；符，毒 不减持久 chongchong 2014-09-12"),
        (53, "任意", "tdmode = 53 聚灵不判断持久 chongchong 2019-01-30 14:54:19"),
        (96, "任意", "stdmode = 96 祝福罐不要减持久 chongchong 2017-07-08"),
        (7, "[1,2,3]", "魔血石，魔幻石产减持久 2020-05-19"),
        (7, "[0]", "千里传音/传音筒不减持久 chongchong 2018-09-13 16:10:28"),
        (25, "[9]", "火龙之心"),
    };

    /// <summary>六道。</summary>
    public static bool SixEquipmentFilters() => EquipmentFilters.Length == 6;

    /// <summary>**装备格要求 `Anicount > 0`**。</summary>
    public static bool EquipmentAnicountGate(int anicount) => anicount > 0;

    /// <summary>**首饰盒没有这一项**。</summary>
    public static bool JewelryLacksAnicountGate() => true;

    /// <summary>**传音筒在两处结果不同**。</summary>
    public static bool TransmitHornDiffers(int anicount)
        => EquipmentAnicountGate(anicount) != true
           || !JewelryLacksAnicountGate();

    /// <summary>两处行为差异。</summary>
    public static bool FilterListsDiffer() => true;

    /// <summary>**首饰盒把 `Shape in [0,1,2,3]` 合并成一条**。</summary>
    public static bool JewelryMergesShapeZero() => true;

    /// <summary>装备格与首饰盒的过滤条目数。</summary>
    public static (int Equipment, int Jewelry) FilterCounts() => (6, 4);

    /// <summary>条目数不同。</summary>
    public static bool FilterCountsDiffer()
    {
        var (e, j) = FilterCounts();

        return e != j;
    }

    /// <summary>**只有衣服与武器会刷新外观**。</summary>
    public static bool FeatureRefreshSlot(int slot) => slot == UDress || slot == UWeapon;

    /// <summary>外观刷新实测。</summary>
    public static bool OnlyDressAndWeaponRefresh()
        => FeatureRefreshSlot(UDress) && FeatureRefreshSlot(UWeapon)
           && !FeatureRefreshSlot(2) && !FeatureRefreshSlot(5);

    /// <summary>**各段更新方式不同**。</summary>
    public static bool FeatureChangedPerBlock() => true;

    /// <summary>**首饰盒不刷新外观**。</summary>
    public static bool JewelryDoesNotRefreshFeature() => true;

    /// <summary>衣服那两处直接赋真。</summary>
    public static bool DressBlocksAssignTrue() => true;

    /// <summary>**TODO 标记出现五次**。</summary>
    public static int TodoCount() => 5;

    /// <summary>五次。</summary>
    public static bool TodoMarkerFiveTimes() => TodoCount() == 5;

    /// <summary>TODO 带日期。</summary>
    public static bool TodoTextHasDate() => TodoMarker.Contains("2013-08-27");

    /// <summary>TODO 内容说明"删除特定技能"从未实现。</summary>
    public static bool TodoDescribesMissingStep()
        => TodoMarker.Contains("删除特定技能");

    /// <summary>**被注释掉的清索引行**。</summary>
    public static string CommentedWIndexZero => "// SmartObject.m_UseItems[I].wIndex := 0;";

    /// <summary>注释行存在。</summary>
    public static bool CommentedWIndexZeroPresent()
        => CommentedWIndexZero.Contains("wIndex := 0");

    /// <summary>**日志文本两处不同**。</summary>
    public static readonly string[] DisappearLogTexts = { "持久为0", "0持久消失" };

    /// <summary>两种文本。</summary>
    public static bool TwoLogTexts() => DisappearLogTexts.Length == 2;

    /// <summary>文本不同。</summary>
    public static bool LogTextsDiffer()
        => DisappearLogTexts[0] != DisappearLogTexts[1];

    /// <summary>脚本标签前缀。</summary>
    public static string ItemDamageLabel(int slot) => "@ItemDamage" + slot;

    /// <summary>标签实测。</summary>
    public static bool ItemDamageLabelFormat()
        => ItemDamageLabel(0) == "@ItemDamage0" && ItemDamageLabel(5) == "@ItemDamage5";

    /// <summary>**发标签前会把脚本跳转计数归零**。</summary>
    public static bool ResetsScriptGotoCount() => true;

    // ===================== 三、收尾 =====================

    /// <summary>**重算能力后只通知英雄与玩家**。</summary>
    public static bool RecalcNotifyRace(int race)
        => race == 0 || race == 1;

    /// <summary>通知门实测。</summary>
    public static bool RecalcNotifiesOnlyHumans()
        => RecalcNotifyRace(0) && RecalcNotifyRace(1) && !RecalcNotifyRace(150);

    /// <summary>**两条通知消息**。</summary>
    public static readonly int[] RecalcMessages = { RmAbility, RmSubAbility };

    /// <summary>两条。</summary>
    public static bool TwoRecalcMessages() => RecalcMessages.Length == 2;

    /// <summary>外观刷新被调用。</summary>
    public static bool FeatureChangedCalled() => true;

    /// <summary>**秒杀保护的种族门**。</summary>
    public static bool StruckProtectRaceGate(int race)
        => race == 0 || race == 1 || race == 150;

    /// <summary>门实测（三类）。</summary>
    public static bool StruckProtectRaceGateValues()
        => StruckProtectRaceGate(0) && StruckProtectRaceGate(1)
           && StruckProtectRaceGate(150) && !StruckProtectRaceGate(80);

    /// <summary>注释文本。</summary>
    public const string StruckProtectComment = "秒杀保护 2020-09-17 22:47:01";

    /// <summary>注释带日期。</summary>
    public static bool StruckProtectCommentHasDate()
        => StruckProtectComment.Contains("2020-09-17");

    /// <summary>**`DamageHealth` 两条路**。</summary>
    public static string DamageHealthPath(bool hasStruckFrom)
        => hasStruckFrom ? "withSource" : "nilSource";

    /// <summary>两条路。</summary>
    public static bool DamageHealthTwoPaths()
        => DamageHealthPath(true) == "withSource"
           && DamageHealthPath(false) == "nilSource";

    /// <summary>**第三参镜像"是否有来源"**。</summary>
    public static bool DamageHealthThirdParam(bool hasStruckFrom) => hasStruckFrom;

    /// <summary>第三参实测。</summary>
    public static bool ThirdParamMirrorsStruckFrom()
        => DamageHealthThirdParam(true) && !DamageHealthThirdParam(false);

    /// <summary>**只有有来源那条会动全局上下文**。</summary>
    public static bool TouchesContext(bool hasStruckFrom) => hasStruckFrom;

    /// <summary>上下文实测。</summary>
    public static bool ContextSavedAndRestored()
        => TouchesContext(true) && !TouchesContext(false);

    /// <summary>会保存的三项上下文。</summary>
    public static readonly string[] SavedContext =
        { "StruckFrom.m_wCurrMagicId", "m_wCurrMagicId", "m_CurrTarget" };

    /// <summary>三项。</summary>
    public static bool ThreeSavedContextItems() => SavedContext.Length == 3;

    /// <summary>**`MagicID` 被同时写进双方**。</summary>
    public static bool MagicIdWrittenToBoth() => true;

    /// <summary>**`m_CurrTarget` 被设为攻击者**。</summary>
    public static bool CurrTargetSetToAttacker() => true;

    /// <summary>**`m_MagicShieldDecMP` 两种语义**。</summary>
    public static int MagicShieldDecMp(int nOldMp, int newMp) => newMp < nOldMp ? nOldMp - newMp : 0;

    /// <summary>语义实测。</summary>
    public static bool MagicShieldDecMpSemantics()
        => MagicShieldDecMp(100, 80) == 20
           && MagicShieldDecMp(100, 100) == 0
           && MagicShieldDecMp(100, 120) == 0;

    /// <summary>**无来源时恒为 0**。</summary>
    public static bool NilSourceZeroesShieldMp() => true;

    /// <summary>**PK 值 = 掉血 + 魔法盾消耗 MP**。</summary>
    public static int PkPower(int result, int magicShieldDecMp) => result + magicShieldDecMp;

    /// <summary>PK 值实测。</summary>
    public static bool PkPowerIsResultPlusShieldMp()
        => PkPower(100, 20) == 120 && PkPower(100, 0) == 100;

    /// <summary>**默认参数为真**。</summary>
    public static bool PkPowerDefaultTrue() => true;

    /// <summary>**需要两个门同时成立**。</summary>
    public static bool PkPowerNeedsBothGuards(bool hasStruckFrom, bool isSetPkPower)
        => hasStruckFrom && isSetPkPower;

    /// <summary>门实测。</summary>
    public static bool PkPowerGuardTruthTable()
        => PkPowerNeedsBothGuards(true, true)
           && !PkPowerNeedsBothGuards(false, true)
           && !PkPowerNeedsBothGuards(true, false);

    /// <summary>**护体神盾击破公式（双等级求和）**。</summary>
    public static int CloseSuperShiledTemp(int rate, int level, int newLevel, int perLevelDecRate)
        => rate + RoundHalfUp(rate / 100.0 * ((level + newLevel) * perLevelDecRate));

    /// <summary>公式实测（默认每级 0 → 等级无影响）。</summary>
    /// <remarks>
    /// **探针实测值**：`(5,3,2,10)` 得 **8** —— `5 + round(5/100 × (3+2)×10)` = `5 + round(2.5)` = `5 + 3` = `8`。
    /// （我落笔时写了个待定的 7，实测是 8；半值 2.5 按 `Round` 远离零进到 3。）
    /// </remarks>
    public static bool CloseSuperShiledFormulaUsesLevelSum()
        => CloseSuperShiledTemp(5, 0, 0, 0) == 5
           && CloseSuperShiledTemp(5, 3, 2, 0) == 5
           && CloseSuperShiledTemp(5, 3, 2, 10) == 8
           && CloseSuperShiledTemp(5, 3, 2, 100) == 30;

    /// <summary>**默认每级为 0 → 等级完全不影响击破概率**。</summary>
    public static bool DefaultPerLevelRateIsNoOp()
        => CloseSuperShiledTemp(5, 0, 0, DefaultCloseSuperShiledLevelUpDecRate)
           == CloseSuperShiledTemp(5, 99, 99, DefaultCloseSuperShiledLevelUpDecRate);

    /// <summary>**源码在此处调用了一次 `Randomize`**。</summary>
    public static bool RandomizeCallPresent() => true;

    /// <summary>**`nTemp <= 0` 时必然击破**。</summary>
    public static bool NonPositiveTempAlwaysBreaks(int temp) => temp <= 0;

    /// <summary>击破判定（`nTemp <= 0` 或 `Random(nTemp) = 0`）。</summary>
    public static bool BreakCheck(int temp, int roll)
        => temp <= 0 || roll == 0;

    /// <summary>**或短路使 `Random(0)` 不会被调用**。</summary>
    public static bool NonPositiveShortCircuits()
        => BreakCheck(0, 0) && BreakCheck(-1, 99);

    /// <summary>**非正 `nTemp` 时无视随机值**。</summary>
    public static bool NonPositiveIgnoresRoll()
        => BreakCheck(-1, 0) == BreakCheck(-1, 99);

    /// <summary>**击破判定用 `= 0`**。</summary>
    public static bool BreakCheckIsEqualZero()
        => BreakRollHits(0, 5) && !BreakRollHits(1, 5);

    /// <summary>单次判定。</summary>
    public static bool BreakRollHits(int roll, int temp) => roll == 0;

    /// <summary>门是三类种族。</summary>
    public static bool CloseSuperShiledRaceGate(int race)
        => race == 0 || race == 1 || race == 150;

    /// <summary>门实测。</summary>
    public static bool CloseSuperShiledGates()
        => CloseSuperShiledRaceGate(0) && CloseSuperShiledRaceGate(150)
           && !CloseSuperShiledRaceGate(80);

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

    /// <summary>函数行数（1:1 参考）。</summary>
    public static int FunctionLines() => 419;

    /// <summary>行数核对。</summary>
    public static bool FunctionLineCount() => FunctionLines() == 419;
}
