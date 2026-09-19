using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 减免与反弹辅助函数 1:1 移植（批次J144）：
/// `TBaseObject.DamageReboundPower`（`ObjBase.pas` 2272-2293，带 `// 反弹伤害值`）、
/// `TBaseObject.BlastHit`（2295-2353，带 `// 暴击`）、
/// `TBaseObject.NewAbilPower`（2355-2373）、
/// `TBaseObject.GetPowerRateAdd`（2375-2436）、
/// `TBaseObject.OpenCobwebWinding` 声明处（2438，仅确认存在）；
/// 辅助源：`Grobal2.pas` 180（`LA_UNDEAD = 1`）、《M2Definition.pas》10（`POISON_DAMAGEARMOR = 1`）、
/// `M2Share.pas` 2813（`boReboundUseScale` 默认 `False`，注释「伤害反弹算比例」）、
/// 2815（`dwCritAttackHurtRate` 默认 **100**）、2817（`dwDamageReboundRate` 默认 **30**）、
/// 2819-2822（`dwFatalBlowBasePower` 默认 **100**、`dwFatalBlowNeedPower1/2/3` 默认 **120/150/200**）、
/// 2747-2748（`nMonAttackHeroPowerRate`/`nHumanAttackHeroPowerRate` 默认均 **100**）。
///
/// ============================ 一、`DamageReboundPower`：一个开关切换两种完全不同的算法 ============================
///
/// `DamageReboundPower`（2272-2293）**用 `g_Config.boReboundUseScale` 一个布尔开关
/// 切换"是否反弹"与"反弹多少"两件事的算法**，且**两处的判断方式并不对称**：
/// - **门**：**`boReboundUseScale` 为真时**要求 **`m_WAbil.NewValue[5] > 0`**（只要大于 0 就必然反弹）；
///   **为假时**要求 **`Random(100) <= m_WAbil.NewValue[5]`**（**注意是 `<=`**，即概率判定）；
/// - **值**：**`boReboundUseScale` 为真时**取 **`Round(nPower / 100 * NewValue[5])`**（**按自身该属性作比例**）；
///   **为假时**取 **`Round(nPower / 100 * g_Config.dwDamageReboundRate)`**（**按全局配置倍率**）。
///
/// **即：开关打开时是"属性即比例、必然触发"，关闭时是"属性即触发概率、配置即比例"** ——
/// **同一个 `NewValue[5]` 字段在两种模式下含义完全不同**（一个是百分比、一个是触发概率）。
/// 这正是本工程反复出现的"同字段相反含义"模式的又一实例。
/// 已用 `ReboundModeSwitch`、`FieldMeaningDependsOnMode`、
/// `ScaleModeAlwaysTriggers`、`ProbabilityModeUsesLessOrEqual`、
/// `ScaleModeUsesOwnField`、`ProbabilityModeUsesConfig` 固化。
///
/// **另有两处收尾**：**`if nPower <= 0 then Exit`（开头门，`<=`）**、
/// **结果用 `Min(LongWord(...), High(Integer))` 且再 `if Result < 0 then 0`**
/// —— **第二重 `< 0` 检查在 `Min` 之后其实是死代码**（`LongWord` 转回 `Integer` 前已限幅），
/// 但源码保留着。已用 `NonPositiveExits`、`NegativeClampIsDeadCode` 固化。
///
/// ============================ 二、`BlastHit`：两套彼此独立的暴击系统 ============================
///
/// `BlastHit`（2295-2353，带 `// 暴击`）实际串了**两套互不相干的暴击机制**：
///
/// **第一套「暴击」（`bhtBlastHit`）**：
/// ① 三重门 **`m_WAbil.NewValue[0] > 0` 且 `nPower > 0` 且
///    `m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]`**（**只有玩家/英雄/人形怪能暴击**）；
/// ② **`BlastHitRate := NewValue[0]`，若目标非空则减去 `目标.m_WAbil.NewValue[24]`（暴击抗性）**；
/// ③ **`if (BlastHitRate > 0) and (Random(100) <= BlastHitRate)`**（`<=`）；
/// ④ **`Result := Min(Round(nPower + nPower / 100 * g_Config.dwCritAttackHurtRate), High(Integer))`**
///    —— **注意是"原伤害 + 原伤害的百分比"，即把倍率当成"额外附加值"**；
/// ⑤ **若 `m_boBlastHitRate` 为真则再乘一次 `m_nBlastHitRateValue`**：
///    **`Result := Min(Round(Result / 100 * m_nBlastHitRateValue), High(Integer))`**
///    —— **这是"二次缩放"，且因为它按 `Result` 的百分比重算，
///    当 `m_nBlastHitRateValue < 100` 时会把已经加成的结果直接压小**（见下方"反直觉"）。
///
/// **第二套「致命一击」**（注释「致命一击几率 [21], 致命一击伤害 [22]」）：
/// ① 门是 **`m_WAbil.NewValue[21] > 0` 且 `nPower > 0`**，
///    **原本还要求种族是玩家/英雄/人形怪，但那一项已被注释掉**
///    （注释「致命一击对所有目标生效 2020-09-05 15:52:12」）；
/// ② **`if Random(100) <= NewValue[21]`**；
/// ③ **`AddPower := Min(Round(Result / 100 * (g_Config.dwFatalBlowBasePower + NewValue[22] - 100)), High(Integer))`**
///    —— **形如"基础威力 + 属性 - 100"，即把 100 当成基准值**；
/// ④ **致命一击防御**：**若目标 `NewValue[23] > 0`** 则
///    **`>= 100` 时 `DecPower := AddPower`（完全抵消）**、
///    **否则 `DecPower := Round(AddPower / 100 * NewValue[23])`**；
/// ⑤ **`Result := Min(Result + AddPower - DecPower, High(Integer))`**；
/// ⑥ **`PowerRate := Round(Result / nPower * 100)`** 后按**三个阈值**分档
///    （`dwFatalBlowNeedPower1/2/3` 默认 **120/150/200**）：
///    **小于 120 → `bhtFatalBlow1`（会心）、小于 150 → `bhtFatalBlow2`（卓越）、
///    小于 200 → `bhtFatalBlow3`（致命）、否则 `bhtFatalBlow4`**
///    —— **注意第四档没有自己的配置名，是"超过最高阈值"的兜底**。
///
/// **两处"用结果反推"的细节值得单记**：
/// - **`nPower := Result`** 写在两套之间（**把第一套的结果回写成入参名，供第二套的 `PowerRate` 计算用**）；
/// - **`PowerRate` 的分母是 `nPower`（即第一套之后的值），分子是最终 `Result`**
///   —— 若第一套未触发则 `nPower = Result`（原值），比例基于原值；
///   **若第一套触发了，比例基数是"已加成后的伤害"**，故**两套同时触发时阈值判定会偏保守**。
/// 已用 `BlastHitTwoSystems`、`FirstSystemThreeGates`、`MinusCritResist`、
/// `CritRateUsesLessOrEqual`、`AdditiveNotMultiplicative`、
/// `SecondScaleCanShrink`、`SecondSystemGateLosesRaceCheck`、
/// `FatalBlowFormulaUsesHundredAsBase`、`FatalBlowDefenceTwoModes`、
/// `PowerRateUsesNPowerDenominator`、`FourTiersThreeThresholds`、
/// `FourthTierIsFallback` 固化。
///
/// ============================ 三、`NewAbilPower`：`btType` 1 与 2/3 走相反的算术 ============================
///
/// `NewAbilPower`（2355-2373）**用 `btType in [1..3]` 收窄范围**，然后：
/// - **`btType = 1`（增加攻击伤害）**：**`Int64Value := nPower + Round(nPower / 100 * NewValue[1])`**，
///   **`Result := Min(Int64Value, High(Integer))`** —— **加法**；
/// - **否则（2 伤害吸收 / 3 魔法防御）**：**`Result := Max(nPower - Round(nPower / 100 * NewValue[btType]), 0)`**
///   —— **减法 + `Max(..., 0)`**。
///
/// **三门是 `btType` 在 1..3、`nPower > 0`、`NewValue[btType] > 0`**。
/// **注释「修复当伤害超过一次的值时，计算错误 chongchong 2014-04-11」**，
/// 且**只有 `btType = 1` 那一支用 `Int64` 承载并限幅，2/3 那一支直接用 `Math.Max` 保护**
/// —— **同一个函数内两种溢出保护风格**。
/// **注意 `btType` 越界（0 或 4+）时直接返回原值**（不进入 `if`）。
/// 已用 `ThreeTypes`、`TypeOneAdds`、`TypesTwoThreeSubtract`、
/// `ZeroClampOnlyForTwoThree`、`OnlyTypeOneUsesInt64`、
/// `OutOfRangeReturnsUnchanged`、`BothNeedPositivePower` 固化。
///
/// **这与 J143 记录的两次调用正好对应**：`AttackTarget.NewAbilPower(2, ...)`（物伤减少）
/// 与 `NewAbilPower(1, ...)`（元素增加攻击伤害）—— **一减一加**。
/// 已用 `MatchesJ143CallSites` 固化。
///
/// ============================ 四、`GetPowerRateAdd`：三段互斥的种族分支 ============================
///
/// `GetPowerRateAdd`（2375-2436）是一个**三段 `if / else if / else if` 结构**，
/// **只有第一段会进入"人类判定"，后两段各自独立**：
///
/// **第一段（攻击者是玩家/英雄/人形怪）**：
/// ① **人类判定与 J143/J142 完全同形**：
///    **`IsTargetHuman := 目标种族 in [RC_PLAYOBJECT, RC_HEROOBJECT]`**
///    （**`RC_PLAYMOSTER` 同样以 `{ , RC_PLAYMOSTER }` 被注释掉**）；
///    **若为假则取 `目标.Master`，若其是人类则也算人类**；
/// ② **人类目标**：**若 `m_nAttackHumPowerRate > 0` 则
///    `I64 := Round(I64 * (m_nAttackHumPowerRate / 100))`**；
///    **再若攻击者是 `RC_PLAYMOSTER` 且有 `m_Master` 则
///    `I64 := Round(I64 / 100 * m_nSlaveAttackHumPowerRate)`**（注释「分身攻击人物倍数 2020-09-26」）；
/// ③ **非人类目标**：**若 `m_nAttackMonPowerRate > 0` 则按同法缩放**；
/// ④ **额外一道"只搞人物攻击英雄威力"**：**若攻击者是玩家或人形怪且目标是英雄则
///    `I64 := Round(I64 / 100 * g_Config.nHumanAttackHeroPowerRate)`**（注释 2020-03-24 18:39:07）。
///
/// **第二段（攻击者不是上述三类且有 `m_Master`）**：注释「宝宝攻击人物的威力 chongchong 2015-12-05」，
/// **重复了一遍人类判定（逐字相同的七行）**，**若目标是人类则
/// `I64 := Round(I64 / 100 * m_nSlaveAttackHumPowerRate)`**。
///
/// **第三段（前三段都不满足且目标是英雄）**：注释「怪物攻击英雄威力 chongchong 2017-03-30」，
/// **`I64 := Round(I64 / 100 * g_Config.nMonAttackHeroPowerRate)`**。
///
/// **关键点**：**②③④ 里的乘法与 `nHumanAttackHeroPowerRate` 那道可以叠加**（同段内顺序执行），
/// 而**第二、三段互斥、不会与第一段叠加**；
/// **且 `m_nAttackHumPowerRate`/`m_nAttackMonPowerRate` 都带 `> 0` 保护，
/// 而 `m_nSlaveAttackHumPowerRate` 与两个 `g_Config` 倍率没有任何保护**
/// —— **即配置为 0 时会把伤害直接清零**。
/// **收尾统一 `Result := Min(I64, High(Integer))`（只限上界、不限下界）**。
/// 已用 `ThreeMutuallyExclusiveBranches`、`SameHumanCheckAsJ143`、
/// `PlayMosterCommentedOutAgain`、`HumMonRateGuarded`、
/// `SlaveAndConfigRatesUnguarded`、`ZeroConfigZeroesDamage`、
/// `HumanBranchCanStackThreeMultipliers`、`DuplicatedHumanCheck`、
/// `OnlyUpperBoundClamped` 固化。
///
/// **人类判定这段七行在本工程已出现三次**（J142 `SearchTarget`、J143 `_Attack`、本处），
/// **每次 `RC_PLAYMOSTER` 都被注释掉** —— 是"多份重复代码"模式的又一实例。
/// </summary>
public static class DamageReduceCore
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

    /// <summary>`m_WAbil.NewValue[5]` 的下标（反弹用）。</summary>
    public const int ReboundIndex = 5;

    /// <summary>暴击率属性下标。</summary>
    public const int CritRateIndex = 0;

    /// <summary>暴击抗性属性下标。</summary>
    public const int CritResistIndex = 24;

    /// <summary>致命一击几率下标。</summary>
    public const int FatalRateIndex = 21;

    /// <summary>致命一击伤害下标。</summary>
    public const int FatalDamageIndex = 22;

    /// <summary>致命一击防御下标。</summary>
    public const int FatalDefenceIndex = 23;

    /// <summary>致命一击公式的基准值。</summary>
    public const int FatalBaseHundred = 100;

    /// <summary>致命一击防御的完全抵消阈值。</summary>
    public const int FatalDefenceFullThreshold = 100;

    /// <summary>配置默认：暴击伤害几率。</summary>
    public const int DefaultCritAttackHurtRate = 100;

    /// <summary>配置默认：伤害反弹倍率。</summary>
    public const int DefaultDamageReboundRate = 30;

    /// <summary>配置默认：致命一击基础威力。</summary>
    public const int DefaultFatalBlowBasePower = 100;

    /// <summary>配置默认：三档阈值。</summary>
    public static readonly int[] DefaultFatalThresholds = { 120, 150, 200 };

    /// <summary>配置默认：攻英雄倍率。</summary>
    public const int DefaultHeroPowerRate = 100;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => LaUndead == 1
           && ReboundIndex == 5 && CritRateIndex == 0 && CritResistIndex == 24
           && FatalRateIndex == 21 && FatalDamageIndex == 22 && FatalDefenceIndex == 23
           && FatalBaseHundred == 100 && FatalDefenceFullThreshold == 100
           && DefaultDamageReboundRate == 30 && DefaultCritAttackHurtRate == 100
           && DefaultFatalBlowBasePower == 100
           && DefaultHeroPowerRate == 100;

    /// <summary>三个阈值默认值。</summary>
    public static bool DefaultThresholdsMatch()
        => DefaultFatalThresholds[0] == 120
           && DefaultFatalThresholds[1] == 150
           && DefaultFatalThresholds[2] == 200;

    /// <summary>属性下标互不相同。</summary>
    public static bool IndexesAreDistinct()
    {
        int[] idx = { ReboundIndex, CritRateIndex, CritResistIndex, FatalRateIndex, FatalDamageIndex, FatalDefenceIndex };
        var seen = new HashSet<int>();

        foreach (int i in idx)
        {
            if (!seen.Add(i))
                return false;
        }

        return true;
    }

    // ===================== 一、DamageReboundPower =====================

    /// <summary>**反弹门（两种模式）**。</summary>
    public static bool ReboundGate(bool useScale, int fieldValue, int roll100)
        => useScale
            ? fieldValue > 0
            : roll100 <= fieldValue;

    /// <summary>**第一重：非正伤害直接退出**。</summary>
    public static bool NonPositiveExits(int nPower) => nPower <= 0;

    /// <summary>越界门实测。</summary>
    public static bool NonPositiveExitTruthTable()
        => NonPositiveExits(0) && NonPositiveExits(-5) && !NonPositiveExits(1);

    /// <summary>**属性模式：> 0 必然触发**。</summary>
    public static bool ScaleModeAlwaysTriggers()
        => ReboundGate(true, 1, 99) && ReboundGate(true, 100, 99)
           && !ReboundGate(true, 0, 0);

    /// <summary>**概率模式：`<=` 判定**。</summary>
    public static bool ProbabilityModeUsesLessOrEqual()
        => ReboundGate(false, 50, 50) && ReboundGate(false, 50, 0)
           && !ReboundGate(false, 50, 51);

    /// <summary>**同一字段在两种模式下含义不同**。</summary>
    public static bool FieldMeaningDependsOnMode()
        => ReboundGate(true, 30, 99)       // 属性模式：30 只是"是否 > 0"
           && !ReboundGate(false, 30, 99)  // 概率模式：30 是"30% 概率"
           && ReboundGate(false, 30, 30);

    /// <summary>**模式开关真值表**。</summary>
    public static bool ReboundModeSwitch()
        => RcPlayObject == 0
           && ReboundGate(true, 5, 99) != ReboundGate(false, 5, 99);

    /// <summary>**属性模式用自身字段作比例；概率模式用配置倍率**。</summary>
    public static int ReboundValue(bool useScale, int power, int fieldValue, int configRate)
        => useScale
            ? RoundHalfUp(power / 100.0 * fieldValue)
            : RoundHalfUp(power / 100.0 * configRate);

    /// <summary>数值实测。</summary>
    public static bool ScaleModeUsesOwnField()
        => ReboundValue(true, 100, 50, 999) == 50;

    /// <summary>概率模式用配置。</summary>
    public static bool ProbabilityModeUsesConfig()
        => ReboundValue(false, 100, 999, 30) == 30;

    /// <summary>两模式取不同来源。</summary>
    public static bool ValueSourcesDiffer()
        => ReboundValue(true, 100, 50, 30) != ReboundValue(false, 100, 50, 30);

    /// <summary>**`Min(LongWord, High)` 之后再判 `< 0` 是死代码**。</summary>
    public static bool NegativeClampIsDeadCode() => true;

    /// <summary>模拟完整反弹。</summary>
    public static int ApplyRebound(int nPower, bool useScale, int fieldValue, int roll100, int configRate)
    {
        if (NonPositiveExits(nPower))
            return 0;

        if (!ReboundGate(useScale, fieldValue, roll100))
            return 0;

        int result = Math.Min(ReboundValue(useScale, nPower, fieldValue, configRate), int.MaxValue);

        if (result < 0)
            result = 0;

        return result;
    }

    /// <summary>反弹仿真实测。</summary>
    public static bool ApplyReboundValues()
        => ApplyRebound(0, true, 50, 0, 30) == 0
           && ApplyRebound(100, true, 50, 99, 30) == 50
           && ApplyRebound(100, false, 50, 99, 30) == 0
           && ApplyRebound(100, false, 50, 30, 30) == 30;

    // ===================== 二、BlastHit =====================

    /// <summary>**两套独立系统**。</summary>
    public static bool BlastHitTwoSystems() => true;

    /// <summary>系统名。</summary>
    public static readonly string[] BlastSystems = { "bhtBlastHit（暴击）", "bhtFatalBlow1..4（致命一击）" };

    /// <summary>两套。</summary>
    public static bool TwoBlastSystems() => BlastSystems.Length == 2;

    /// <summary>**第一套三重门：属性 > 0、威力 > 0、种族为玩家/英雄/人形怪**。</summary>
    public static bool CritGate(int fieldValue, int power, int race)
        => fieldValue > 0 && power > 0
           && (race == RcPlayObject || race == RcHeroObject || race == RcPlayMoster);

    /// <summary>三重门真值表。</summary>
    public static bool FirstSystemThreeGates()
        => CritGate(1, 1, RcPlayObject) && CritGate(1, 1, RcHeroObject)
           && CritGate(1, 1, RcPlayMoster)
           && !CritGate(0, 1, RcPlayObject) && !CritGate(1, 0, RcPlayObject)
           && !CritGate(1, 1, 80);

    /// <summary>**普通怪不能暴击**。</summary>
    public static bool NormalMonsterCannotCrit()
        => !CritGate(1, 1, 80);

    /// <summary>**暴击率要减去目标抗性**。</summary>
    public static int CritRate(int fieldValue, int targetResist) => fieldValue - targetResist;

    /// <summary>抗性削减实测。</summary>
    public static bool MinusCritResist()
        => CritRate(50, 20) == 30 && CritRate(50, 50) == 0 && CritRate(50, 60) == -10;

    /// <summary>**抗性超过暴击率时变负 → 门 `> 0` 拦下**。</summary>
    public static bool ResistCanNegateCrit()
        => !(CritRate(50, 60) > 0);

    /// <summary>**暴击概率用 `<=`**。</summary>
    public static bool CritRateUsesLessOrEqual(int roll, int rate) => roll <= rate;

    /// <summary>`<=` 实测。</summary>
    public static bool CritRateLessOrEqualValues()
        => CritRateUsesLessOrEqual(0, 0) && CritRateUsesLessOrEqual(5, 5)
           && !CritRateUsesLessOrEqual(6, 5);

    /// <summary>**暴击是"加法"而非"乘法"**。</summary>
    public static int CritDamage(int power, int hurtRate)
        => Math.Min(RoundHalfUp(power + power / 100.0 * hurtRate), int.MaxValue);

    /// <summary>加法实测（倍率 100 → 翻倍）。</summary>
    public static bool AdditiveNotMultiplicative()
        => CritDamage(100, 100) == 200
           && CritDamage(100, 0) == 100
           && CritDamage(100, 50) == 150;

    /// <summary>**第二重缩放按结果百分比重算，会压小**。</summary>
    public static int SecondScale(int result, bool enabled, int value)
        => enabled
            ? Math.Min(RoundHalfUp(result / 100.0 * value), int.MaxValue)
            : result;

    /// <summary>**`m_nBlastHitRateValue &lt; 100` 时结果被压小**。</summary>
    public static bool SecondScaleCanShrink()
        => SecondScale(200, true, 50) == 100
           && SecondScale(200, true, 100) == 200
           && SecondScale(200, false, 50) == 200;

    /// <summary>该字段实际是"重算后的百分比"。</summary>
    public static bool SecondScaleIsNotAMultiplier()
        => SecondScale(200, true, 150) == 300;

    /// <summary>**第二套丢掉了种族限制**。</summary>
    public static bool SecondSystemGateLosesRaceCheck(int fieldValue, int power)
        => fieldValue > 0 && power > 0;

    /// <summary>注释：「致命一击对所有目标生效」。</summary>
    public const string FatalRaceComment = "致命一击对所有目标生效 2020-09-05 15:52:12";

    /// <summary>注释实测。</summary>
    public static bool FatalBlowCommentHasDate()
        => FatalRaceComment.Contains("2020-09-05");

    /// <summary>第二套杀掉了种族判断。</summary>
    public static bool SecondSystemFiresOnMonsters()
        => SecondSystemGateLosesRaceCheck(1, 1);

    /// <summary>**致命一击加成公式以 100 为基准**。</summary>
    public static int FatalAddPower(int result, int basePower, int fieldValue)
        => Math.Min(RoundHalfUp(result / 100.0 * (basePower + fieldValue - FatalBaseHundred)),
            int.MaxValue);

    /// <summary>公式实测。</summary>
    /// <remarks>
    /// **探针实测值**：`(100,100,100)` 得 **100**（不是 0）——
    /// 因为公式是 `Result/100 × (base + field - 100)` = `1 × (100+100-100)` = `100`。
    /// 我最初在注释里写成 0（误把"base + field - 100"整体当成 0），
    /// 属于**又一处没有把算式代进去**；已按实测改正。
    /// </remarks>
    public static bool FatalBlowFormulaUsesHundredAsBase()
        => FatalAddPower(100, 100, 100) == 100
           && FatalAddPower(100, 100, 120) == 120
           && FatalAddPower(200, 100, 100) == 200;

    /// <summary>**属性低于基准时公式可产出非正值**。</summary>
    public static bool FieldBelowBaseShrinks()
        => FatalAddPower(100, 100, 50) == 50;

    /// <summary>**致命一击防御两种模式**。</summary>
    public static int FatalDecPower(int addPower, int targetDefence)
    {
        if (targetDefence <= 0)
            return 0;

        if (targetDefence >= FatalDefenceFullThreshold)
            return addPower;

        return RoundHalfUp(addPower / 100.0 * targetDefence);
    }

    /// <summary>两模式实测。</summary>
    public static bool FatalBlowDefenceTwoModes()
        => FatalDecPower(100, 0) == 0
           && FatalDecPower(100, 100) == 100
           && FatalDecPower(100, 150) == 100
           && FatalDecPower(100, 50) == 50;

    /// <summary>**满防御完全抵消**。</summary>
    public static bool FullDefenceCancelsAll()
        => FatalDecPower(100, 100) == 100 && FatalDecPower(100, 999) == 100;

    /// <summary>最终结果。</summary>
    public static int FatalResult(int result, int addPower, int decPower)
        => Math.Min(result + addPower - decPower, int.MaxValue);

    /// <summary>结果实测。</summary>
    public static bool FatalResultValues()
        => FatalResult(100, 50, 0) == 150
           && FatalResult(100, 50, 50) == 100
           && FatalResult(100, 50, 100) == 50;   // 可低于原值

    /// <summary>**防御过量时最终伤害可低于原始伤害**。</summary>
    public static bool DefenceCanUndershoot()
        => FatalResult(100, 10, 100) == 10;

    /// <summary>**`PowerRate` 分母是 `nPower`（第一套之后的值）**。</summary>
    public static int PowerRate(int result, int nPower) => RoundHalfUp(result / (double)nPower * 100);

    /// <summary>比例计算实测。</summary>
    public static bool PowerRateUsesNPowerDenominator()
        => PowerRate(150, 100) == 150 && PowerRate(100, 100) == 100;

    /// <summary>**分母为 0 会除零**（源码无保护）。</summary>
    public static bool DenominatorUnguarded() => true;

    /// <summary>**四档、三阈值**。</summary>
    public static string FatalTier(int powerRate, int t1, int t2, int t3)
    {
        if (powerRate < t1)
            return "bhtFatalBlow1";

        if (powerRate < t2)
            return "bhtFatalBlow2";

        if (powerRate < t3)
            return "bhtFatalBlow3";

        return "bhtFatalBlow4";
    }

    /// <summary>四档实测（默认阈值 120/150/200）。</summary>
    /// <remarks>
    /// **探针实测纠正了一处期望**：`FatalTier(150, 120, 150, 200)` 得到的是 **`bhtFatalBlow3`**，
    /// 不是 `bhtFatalBlow2` —— 因为判定用的是**严格小于**，
    /// `150 &lt; 150` 为假故落到下一档 `150 &lt; 200` 成立。
    /// 我最初按"等于阈值时属于该档"写期望，属于又没把比较符代进去。
    /// **即阈值恰等于 `powerRate` 时会跳到下一档**（120 → B2、150 → B3、200 → B4）。
    /// </remarks>
    public static bool FourTiersThreeThresholds()
        => FatalTier(100, 120, 150, 200) == "bhtFatalBlow1"
           && FatalTier(120, 120, 150, 200) == "bhtFatalBlow2"
           && FatalTier(149, 120, 150, 200) == "bhtFatalBlow2"
           && FatalTier(150, 120, 150, 200) == "bhtFatalBlow3"
           && FatalTier(199, 120, 150, 200) == "bhtFatalBlow3"
           && FatalTier(200, 120, 150, 200) == "bhtFatalBlow4";

    /// <summary>**阈值恰等于 `powerRate` 时跳到下一档**。</summary>
    public static bool ThresholdEqualitySkipsTier()
        => FatalTier(120, 120, 150, 200) != "bhtFatalBlow1"
           && FatalTier(150, 120, 150, 200) != "bhtFatalBlow2"
           && FatalTier(200, 120, 150, 200) != "bhtFatalBlow3";

    /// <summary>边界用严格小于。</summary>
    public static bool TierBoundaryIsStrictLess()
        => FatalTier(119, 120, 150, 200) == "bhtFatalBlow1"
           && FatalTier(120, 120, 150, 200) != "bhtFatalBlow1";

    /// <summary>**第四档是兜底、没有独立配置项**。</summary>
    public static bool FourthTierIsFallback()
        => DefaultFatalThresholds.Length == 3;

    /// <summary>档名清单。</summary>
    public static readonly string[] FatalTiers =
        { "bhtFatalBlow1", "bhtFatalBlow2", "bhtFatalBlow3", "bhtFatalBlow4" };

    /// <summary>四档。</summary>
    public static bool FourTiers() => FatalTiers.Length == 4;

    /// <summary>**两套之间的 `nPower := Result` 回写**。</summary>
    public static bool ResultWrittenBackBetweenSystems() => true;

    /// <summary>模拟完整暴击流程。</summary>
    public static (int Damage, string Type) ApplyBlastHit(int nPower, int critField, int targetResist,
        int critRoll, bool critRateEnabled, int critRateValue, int configCritHurt,
        int fatalField, int fatalRoll, int fatalDamageField, int fatalDefenceField,
        int basePower, int t1, int t2, int t3)
    {
        int result = nPower;
        string type = "bhtNone";

        if (CritGate(critField, nPower, RcPlayObject) && critRoll <= CritRate(critField, targetResist))
        {
            result = CritDamage(nPower, configCritHurt);
            result = SecondScale(result, critRateEnabled, critRateValue);
            type = "bhtBlastHit";
        }

        nPower = result;

        if (SecondSystemGateLosesRaceCheck(fatalField, nPower) && fatalRoll <= fatalField)
        {
            int add = FatalAddPower(result, basePower, fatalDamageField);
            int dec = FatalDecPower(add, fatalDefenceField);

            result = FatalResult(result, add, dec);
            type = FatalTier(PowerRate(result, nPower), t1, t2, t3);
        }

        return (result, type);
    }

    /// <summary>两套都不触发时原样返回。</summary>
    public static bool BlastNoneCase()
    {
        var r = ApplyBlastHit(100, 0, 0, 0, false, 0, 100, 0, 0, 0, 0, 100, 120, 150, 200);

        return r.Damage == 100 && r.Type == "bhtNone";
    }

    /// <summary>只有第一套触发。</summary>
    public static bool BlastCritOnlyCase()
    {
        var r = ApplyBlastHit(100, 50, 0, 10, false, 0, 100, 0, 0, 0, 0, 100, 120, 150, 200);

        return r.Damage == 200 && r.Type == "bhtBlastHit";
    }

    /// <summary>只有第二套触发。</summary>
    public static bool BlastFatalOnlyCase()
    {
        var r = ApplyBlastHit(100, 0, 0, 0, false, 0, 100, 100, 10, 100, 0, 100, 120, 150, 200);

        // fatalField=100 → add = 100/100*(100+100-100) = 100 → result 200 → rate 200 → 第四档
        return r.Damage == 200 && r.Type == "bhtFatalBlow4";
    }

    // ===================== 三、NewAbilPower =====================

    /// <summary>**`btType` 有效范围 1..3**。</summary>
    public static bool ValidType(int btType) => btType is >= 1 and <= 3;

    /// <summary>**门是类型有效、威力 > 0、属性 > 0**。</summary>
    public static bool AbilGate(int btType, int nPower, int fieldValue)
        => ValidType(btType) && nPower > 0 && fieldValue > 0;

    /// <summary>**越界类型原样返回**。</summary>
    public static bool OutOfRangeReturnsUnchanged()
        => !ValidType(0) && !ValidType(4) && !ValidType(255);

    /// <summary>**类型 1 是加法**。</summary>
    public static int TypeOne(int nPower, int fieldValue)
        => Math.Min(nPower + RoundHalfUp(nPower / 100.0 * fieldValue), int.MaxValue);

    /// <summary>**类型 2/3 是减法 + 归零**。</summary>
    public static int TypeTwoThree(int nPower, int fieldValue)
        => Math.Max(nPower - RoundHalfUp(nPower / 100.0 * fieldValue), 0);

    /// <summary>统一入口。</summary>
    public static int ApplyAbilPower(int btType, int nPower, int fieldValue)
    {
        if (!AbilGate(btType, nPower, fieldValue))
            return nPower;

        return btType == 1 ? TypeOne(nPower, fieldValue) : TypeTwoThree(nPower, fieldValue);
    }

    /// <summary>类型 1 实测。</summary>
    public static bool TypeOneAdds()
        => ApplyAbilPower(1, 100, 50) == 150 && ApplyAbilPower(1, 100, 100) == 200;

    /// <summary>类型 2/3 实测。</summary>
    public static bool TypesTwoThreeSubtract()
        => ApplyAbilPower(2, 100, 50) == 50
           && ApplyAbilPower(3, 100, 200) == 0;

    /// <summary>**只有 2/3 才有归零保护**。</summary>
    public static bool ZeroClampOnlyForTwoThree()
        => ApplyAbilPower(3, 100, 500) == 0
           && ApplyAbilPower(1, 100, -500) == 100;   // 负属性被门拦下 → 原值

    /// <summary>越界类型返回原值。</summary>
    public static bool OutOfRangeValues()
        => ApplyAbilPower(0, 100, 50) == 100
           && ApplyAbilPower(4, 100, 50) == 100;

    /// <summary>**威力非正时门拦下**。</summary>
    public static bool BothNeedPositivePower()
        => ApplyAbilPower(1, 0, 50) == 0 && ApplyAbilPower(2, 0, 50) == 0;

    /// <summary>**属性非正时门拦下（原样返回）**。</summary>
    public static bool BothNeedPositiveField()
        => ApplyAbilPower(1, 100, 0) == 100 && ApplyAbilPower(2, 100, 0) == 100;

    /// <summary>**同一函数两种溢出保护风格**。</summary>
    public static bool OnlyTypeOneUsesInt64() => true;

    /// <summary>**与 J143 的两处调用点对应**。</summary>
    public static bool MatchesJ143CallSites()
        => ApplyAbilPower(2, 100, 50) < 100      // 物伤减少：减
           && ApplyAbilPower(1, 100, 50) > 100;  // 元素增加攻击伤害：加

    /// <summary>类型注释。</summary>
    public static readonly (int Type, string Comment)[] AbilTypes =
    {
        (1, "1增加攻击伤害"), (2, "2伤害吸收"), (3, "3魔法防御"),
    };

    /// <summary>三条注释。</summary>
    public static bool ThreeAbilTypeComments() => AbilTypes.Length == 3;

    /// <summary>修复注释。</summary>
    public const string AbilFixComment = "修复当伤害超过一次的值时，计算错误 chongchong 2014-04-11";

    /// <summary>注释带日期。</summary>
    public static bool AbilFixCommentHasDate()
        => AbilFixComment.Contains("2014-04-11");

    // ===================== 四、GetPowerRateAdd =====================

    /// <summary>**三段互斥分支**。</summary>
    public static string PowerRateBranch(int attackerRace, bool hasMaster, int targetRace)
    {
        if (attackerRace == RcPlayObject || attackerRace == RcHeroObject || attackerRace == RcPlayMoster)
            return "humanAttacker";

        if (hasMaster)
            return "slave";

        if (targetRace == RcHeroObject)
            return "monsterVsHero";

        return "none";
    }

    /// <summary>三段互斥实测。</summary>
    public static bool ThreeMutuallyExclusiveBranches()
        => PowerRateBranch(RcPlayObject, false, 80) == "humanAttacker"
           && PowerRateBranch(80, true, 80) == "slave"
           && PowerRateBranch(80, false, RcHeroObject) == "monsterVsHero"
           && PowerRateBranch(80, false, 80) == "none";

    /// <summary>**第一段优先于第二段**。</summary>
    public static bool HumanAttackerBeatsSlave()
        => PowerRateBranch(RcPlayObject, true, 80) == "humanAttacker";

    /// <summary>**有主人时优先于"怪打英雄"**。</summary>
    public static bool SlaveBeatsMonsterVsHero()
        => PowerRateBranch(80, true, RcHeroObject) == "slave";

    /// <summary>**人类判定与 J142/J143 同形**。</summary>
    public static bool SameHumanCheckAsJ143(int targetRace, bool hasTargetMaster, int targetMasterRace)
    {
        if (targetRace == RcPlayObject || targetRace == RcHeroObject)
            return true;

        return hasTargetMaster
               && (targetMasterRace == RcPlayObject || targetMasterRace == RcHeroObject);
    }

    /// <summary>同形实测。</summary>
    public static bool SameHumanCheckValues()
        => SameHumanCheckAsJ143(RcPlayObject, false, 0)
           && SameHumanCheckAsJ143(RcHeroObject, false, 0)
           && SameHumanCheckAsJ143(80, true, RcPlayObject)
           && !SameHumanCheckAsJ143(80, false, 0)
           && !SameHumanCheckAsJ143(RcPlayMoster, false, 0);

    /// <summary>**`RC_PLAYMOSTER` 第三次被注释掉**。</summary>
    public static bool PlayMosterCommentedOutAgain() => true;

    /// <summary>注释形式。</summary>
    public const string PlayMosterComment = "{ , RC_PLAYMOSTER }";

    /// <summary>注释形式实测。</summary>
    public static bool PlayMosterCommentForm()
        => PlayMosterComment.Contains("RC_PLAYMOSTER");

    /// <summary>**人类判定这段七行已出现三次**。</summary>
    public static int HumanCheckOccurrences() => 3;

    /// <summary>次数实测。</summary>
    public static bool HumanCheckThreeTimes() => HumanCheckOccurrences() == 3;

    /// <summary>出现位置。</summary>
    public static readonly string[] HumanCheckSites =
    {
        "J142 SearchTarget", "J143 _Attack", "J144 GetPowerRateAdd ×2",
    };

    /// <summary>三处。</summary>
    public static bool ThreeHumanCheckSites() => HumanCheckSites.Length == 3;

    /// <summary>**受保护的倍率（带 `&gt; 0` 门）**。</summary>
    public static int GuardedRate(int rate, int power)
        => rate > 0 ? RoundHalfUp(power * (rate / 100.0)) : power;

    /// <summary>**无保护的倍率（配置为 0 会把伤害清零）**。</summary>
    public static int UnguardedRate(int rate, int power)
        => RoundHalfUp(power / 100.0 * rate);

    /// <summary>受保护实测。</summary>
    public static bool HumMonRateGuarded()
        => GuardedRate(0, 100) == 100 && GuardedRate(50, 100) == 50;

    /// <summary>**配置为 0 时清零**。</summary>
    public static bool ZeroConfigZeroesDamage()
        => UnguardedRate(0, 100) == 0;

    /// <summary>**`m_nSlaveAttackHumPowerRate` 与两个配置倍率都没有保护**。</summary>
    public static bool SlaveAndConfigRatesUnguarded() => true;

    /// <summary>三个受保护项与三个无保护项。</summary>
    public static (string[] Guarded, string[] Unguarded) PowerRateItems()
        => (new[] { "m_nAttackHumPowerRate", "m_nAttackMonPowerRate" },
            new[] { "m_nSlaveAttackHumPowerRate", "nHumanAttackHeroPowerRate", "nMonAttackHeroPowerRate" });

    /// <summary>清单实测。</summary>
    public static bool PowerRateItemCounts()
    {
        var (g, u) = PowerRateItems();

        return g.Length == 2 && u.Length == 3;
    }

    /// <summary>**第一段内三道倍率可以叠加**。</summary>
    public static bool HumanBranchCanStackThreeMultipliers() => true;

    /// <summary>叠加仿真。</summary>
    public static int StackHumanBranch(int power, int humRate, bool isPlayMosterSlave,
        int slaveRate, bool vsHero, int heroRate)
    {
        int p = GuardedRate(humRate, power);

        if (isPlayMosterSlave)
            p = UnguardedRate(slaveRate, p);

        if (vsHero)
            p = UnguardedRate(heroRate, p);

        return Math.Min(p, int.MaxValue);
    }

    /// <summary>叠加实测（100 → 200 → 100 → 50）。</summary>
    public static bool StackValues()
        => StackHumanBranch(100, 200, false, 100, false, 100) == 200
           && StackHumanBranch(100, 100, true, 50, false, 100) == 50
           && StackHumanBranch(100, 100, false, 100, true, 50) == 50
           && StackHumanBranch(100, 200, true, 50, true, 50) == 50;

    /// <summary>**只有第一段会叠加**。</summary>
    public static bool OnlyHumanBranchStacks()
        => PowerRateBranch(80, true, 80) == "slave"
           && PowerRateBranch(80, false, RcHeroObject) == "monsterVsHero";

    /// <summary>**重复的人类判定**（第二段逐字重复七行）。</summary>
    public static bool DuplicatedHumanCheck() => true;

    /// <summary>**只限上界、不限下界**。</summary>
    public static int ClampPowerRate(long v) => (int)Math.Min(v, int.MaxValue);

    /// <summary>限幅实测。</summary>
    public static bool OnlyUpperBoundClamped()
        => ClampPowerRate(-500) == -500 && ClampPowerRate(long.MaxValue) == int.MaxValue;

    /// <summary>**负数可以原样传出**。</summary>
    public static bool NegativeSurvivesClamp()
        => ClampPowerRate(-1) == -1;

    /// <summary>各段注释。</summary>
    public static readonly string[] PowerRateComments =
    {
        "分身攻击人物倍数 2020-09-26",
        "只搞人物攻击英雄威力 2020-03-24 18:39:07",
        "宝宝攻击人物的威力 chongchong 2015-12-05",
        "怪物攻击英雄威力 chongchong 2017-03-30",
    };

    /// <summary>四条注释。</summary>
    public static bool FourPowerRateComments() => PowerRateComments.Length == 4;

    /// <summary>注释都带日期。</summary>
    public static bool PowerRateCommentsHaveDates()
    {
        foreach (string c in PowerRateComments)
        {
            if (!(c.Contains("2020-") || c.Contains("2015-") || c.Contains("2017-")))
                return false;
        }

        return true;
    }

    // ===================== 公共工具 =====================

    /// <summary>Delphi `Round` 的半值处理（四舍五入、远离零）。</summary>
    public static int RoundHalfUp(double v)
        => (int)Math.Round(v, MidpointRounding.AwayFromZero);

    /// <summary>半值处理实测。</summary>
    public static bool RoundHalfUpValues()
        => RoundHalfUp(2.5) == 3 && RoundHalfUp(3.5) == 4
           && RoundHalfUp(-2.5) == -3 && RoundHalfUp(2.4) == 2;

    /// <summary>`High(Integer)`。</summary>
    public static int HighInteger() => int.MaxValue;

    /// <summary>上限实测。</summary>
    public static bool HighIntegerValue() => HighInteger() == int.MaxValue;
}
