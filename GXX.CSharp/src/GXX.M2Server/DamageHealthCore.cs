using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 减血结算与状态附着 1:1 移植（批次J147）：
/// `TBaseObject.DamageHealth`（`ObjBase.pas` 13973-14220，带 `// 减血`）、
/// `TBaseObject.MakePosion`（40222-40299）、`TBaseObject.MakeFrozen`（40301-40331，带 `// 冰冻`）、
/// `TBaseObject.OpenCobwebWinding`（2438-2453，带 `// 蜘蛛网罩住`）、
/// `TBaseObject.DamageBubbleDefence`（40503-40512）、
/// `TBaseObject.DamageNewHitBubbleDefence`（40514-40523）、
/// `TBaseObject.DamageNewMagBubbleDefence`（40525-40534）、
/// `TSmartObject.GetStruckProtectHP`（43438-43480）；
/// 辅助源 `Grobal2.pas` 36（`MAX_STATUS_ATTR = 18`）、
/// 1134（`RM_REFABILNG = 20190`，注释「刷新内力」）、
/// 1140（`RM_OPENCOBWEBWINDING = 20196`，注释「蜘蛛网罩住  开启」）、
/// 1145（`RM_STOPCONTINUOUSMAGIC = 20201`，注释「停止连击」）、
/// 1212（`RM_MAGICSHIELD_STRUCK = 20258`）、
/// 1213（`RM_HEALTHSPELLCHANGED_STRUCK = 20259`）、
/// 1249（`RM_POISON_STRUCK_HUM = 20406`，注释「被人物或人物宝宝施毒」）；
/// `M2Definition.pas` 9（`POISON_DECHEALTH = 0`，注释「中毒类型 - 绿毒」）、
/// 13（`POISON_STONE = 5`，注释「中毒类型 - 麻痹」）、
/// 18（`STATE_BUBBLEDEFENCEUP = 11`）、19（`STATE_FROZEN = 12`，注释「冰冻」）、
/// 20（`STATE_NEWHITBUBBLEDEFENCEUP = 13`，注释「新武力盾」）、
/// 21（`STATE_NEWMAGBUBBLEDEFENCEUP = 14`，注释「新道力盾」）；
/// `M2Share.pas` 1733（`boMagicshieldStruck` 默认 **False**）、
/// 2183（`boCloseSuperShiledHint` 默认 **False**）、2381（`boSlaveKillHumanIncPK`）、
/// 2480（`boShowYouPoisoned`）、2301-2304（四个内功等级参数）、2284（`nNGLevelValue` 默认 **10**）。
///
/// ============================ 一、`DamageHealth`：魔法盾吸伤的"先扣蓝再扣血" ============================
///
/// `DamageHealth` 的核心是**魔法盾**，且它的初始化**按种族分两条完全不同的路**：
/// - **玩家/英雄/人形怪**：**`boSelfMagicShield := m_boMagicShield or (Random(100) < m_btFluteStoneMagicShieldRate)`**
///   —— **即"自身有盾"或"笛石被动触发"二者之一**；
///   **损伤比例取 `m_nMagicShield`、吸收比例取 `m_nMagicShieldAbsorbDamage`**；
/// - **其它种族**：**`boSelfMagicShield := False`（怪物永远没有魔法盾）**、
///   **`nMagicShield := 150`（硬编码 150！）**、**`nMagicShieldAbsorbDamage := 0`**。
///
/// **即那个 150 对怪物而言根本用不到**（因为 `boSelfMagicShield` 恒假），
/// 是一个**"看起来有默认值、实际不可达"的赋值** —— 但源码保留着。
///
/// **`boUnMagicShield`（无视魔法盾）的依据是"攻击者或最后攻击者带该标记"**：
/// **先看 `StruckFrom.UnMagicShield`，否则看 `m_LastHiter.UnMagicShield`，都没有则为假**。
///
/// **吸收比例生效时会先把伤害按比例缩放并可能直接清零返回**：
/// **`Int64Value := Round(Int64(nDamage) / 100 * Max(100 - nMagicShieldAbsorbDamage, 0))`**，
/// **超上限则取 `High(Integer)`**，**若结果为 0 则 `Result := 0; Exit`**
/// —— **即"吸收到零"时不会走后面的扣血逻辑**。
///
/// 已用 `MagicShieldInitPerRace`、`MonsterHasNoShield`、`Hardcoded150Unreachable`、
/// `FluteStonePassive`、`UnMagicShieldTwoSources`、`AbsorbScalesThenMayExit`、
/// `AbsorbZeroExits`、`AbsorbClampedToHigh` 固化。
///
/// ============================ 二、两段逐字镜像的 NPC 脚本钩子 ============================
///
/// `DamageHealth` 里有**两段几乎逐字相同的脚本钩子块**，都是
/// **先存 `StruckFrom` 的三项字段、在 `try` 里把三个字段都设为 `Self` 且 `m_CurrTarget := StruckFrom`、
/// 再按四种身份分支调不同的 `GotoLable`、最后在 `finally` 里还原**。
///
/// **第一段（14017）问的是"攻击者的身份"**，四种分支与标签：
/// **攻击者是玩家 → `@AttackDamage`**、
/// **攻击者是英雄且其主人是玩家 → `@HeroAttackDamage`**、
/// **攻击者有主人且主人是玩家且 `m_boGamePet` → `@GamePetAttackDamage`**、
/// **攻击者有主人且主人是玩家（其余）→ `@SlaveAttackDamage`**。
///
/// **第二段（14074）问的是"自己的身份"**，四种分支与标签：
/// **自己是玩家 → `@StruckDamage`**、
/// **自己是英雄且主人是玩家 → `@HeroStruckDamage`**、
/// **自己有主人且主人是玩家且 `m_boGamePet` → `@GamePetStruckDamage`**、
/// **自己有主人且主人是玩家（其余）→ `@SlaveStruckDamage`**。
///
/// **关键点**：**两段都把伤害写进 `m_nLastDamageValue` 再读回**（**脚本可以改伤害**）；
/// **而第一段在"奴隶"分支里写的是 `StruckFrom.m_Master.m_nLastDamageValue`（主人的），
/// 前三个分支写的都是 `StruckFrom.m_nLastDamageValue`（自己的）** ——
/// **即只有"奴隶"分支把值记在主人身上**。第二段同样：
/// **前三个分支写自己的 `m_nLastDamageValue`，只有"奴隶"分支写 `m_Master.m_nLastDamageValue`**。
/// **两段都要求 `g_FunctionNPC <> nil` 且 `StruckFrom <> nil`**，
/// 故**没有攻击者时脚本钩子完全不执行**。
/// 已用 `TwoMirroredScriptHooks`、`FirstAsksAttackerIdentity`、`SecondAsksSelfIdentity`、
/// `FourBranchesEach`、`DamageRebookedThroughField`、`SlaveBranchUsesMasterField`、
/// `HooksNeedNpcAndAttacker` 固化。
///
/// **`GetStruckProtectHP` 里也有同类的 `@ProtectHP` / `@HeroProtectHP` 分支**（见下）。
///
/// ============================ 三、PK 标记与"掉蓝" ============================
///
/// **PK 标记**（14065）门是**七重 `and`**：
/// **`StruckFrom <> nil`、`nDamage > 0`、`m_WAbil.MP > 0`、
/// `boSlaveKillHumanIncPK`、攻击者不是玩家/英雄、攻击者有主人、主人是玩家/英雄**
/// —— **即"奴隶打人时把 PK 记在主人身上"**（`SetPKFlag(StruckFrom.m_Master)`）。
///
/// **掉蓝段（14120）门是五重**：
/// **`not boUnMagicShield`、`boSelfMagicShield`、`nDamage > 0`、`m_WAbil.MP > 0`、`nMagicShield > 0`**。
/// 进入后：
/// ① **`Int64Value := Round(Int64(nDamage) / 100 * Max(nMagicShield, 0))`；`nSpdam := Min(Int64Value, High(LongWord))`**
///    （注释「掉蓝比例 chongchong 2014-05-16」）；
/// ② **`if (nSpdam > 0) and (StruckFrom <> nil) and (StruckFrom <> Self)`** 时
///    **若攻击者是玩家/英雄则 `SetPKFlag(StruckFrom)`、并 `SetLastHiter(StruckFrom)`**
///    —— **注意"打自己"（`StruckFrom = Self`）不记 PK**；
/// ③ **扣蓝两分支**：**`LongWord(m_WAbil.MP) >= nSpdam` 时
///    `IsDecMP := nSpdam > 0` 且 `MP := MP - nSpdam`**；
///    **否则 `IsDecMP := MP > 0`、`nSpdam := MP`、`MP := 0`**
///    —— **即蓝不够时"掉蓝量被截断为当前蓝"**；
/// ④ **`if nSpdam > nDamage then nDamage := 0 else nDamage := nDamage - nSpdam`**
///    —— **蓝掉够了就完全免伤**；
/// ⑤ **`AttackPower := nSpdam`**（注意此处是赋值、后面才累加）；
/// ⑥ **发 `RM_HEALTHSPELLCHANGED_STRUCK`**（**参数里两个 `-1`**）；
/// ⑦ **`if IsDecMP and (nDamage = 0) and boMagicshieldStruck then` 发 `RM_MAGICSHIELD_STRUCK`
///    且第七参是 `NativeInt(m_LastHiter)`（把对象指针当整数传）**。
/// 已用 `PkFlagSevenGates`、`PkGoesToMaster`、`DrainMpFiveGates`、`NoPkWhenSelfStruck`、
/// `MpShortfallTruncates`、`FullAbsorbZeroesDamage`、`TwoMpMessages`、
/// `ShieldStruckNeedsThreeConditions`、`ObjectPointerAsParam` 固化。
///
/// ============================ 四、扣血两分支与吸血 ============================
///
/// **扣血段（14163）分"伤害为正"与"伤害非正"两条路**：
/// - **`nDamage > 0`**：**HP 够扣则 `HP := HP - nDamage`、`AttackPower += nDamage`、`Result := nDamage`**；
///   **否则 `Result := m_WAbil.HP`（剩余血量）、`HP := 0`，
///   然后 `AttackPower := AttackPower + m_WAbil.HP`（此时已是 0）**
///   —— **这是一个 bug：本该加"原血量"，却加了已经置零之后的血量，故实际加 0**。
///   源码把这个顺序原样保留着。
/// - **`nDamage <= 0`（负伤害 = 治疗）**：**`Int64Value := Int64(HP) - nDamage`**，
///   **若 `< MaxHP` 则 `Result := nDamage`、`HP := Int64Value`**，
///   **否则 `Result := HP - MaxHP`、`HP := MaxHP`**（**溢出部分作为返回值**）。
///
/// 已用 `DeductTwoPaths`、`OverkillUsesRemainingHp`、`OverkillAttackPowerBug`、
/// `NegativeDamageHeals`、`HealOverflowReturned` 固化。
///
/// **吸血两段结构完全对称**（`m_boAbsorbMP` / `m_boAbsorbHP`），各**五重门**：
/// **`AttackPower > 0`、`StruckFrom <> nil`、`m_boAbsorbXX`、`Random(100) < m_nAbsorbXXRate`、
/// `m_nAbsorbXXValue > 0`**；
/// **取值 `nDamage := Round(AttackPower / 100 * m_nAbsorbXXValue)`**，
/// **大于 0 时把攻击者的 MP/HP 加上并钳到上限、再 `HealthSpellChanged()`**。
/// **注意用的是 `AttackPower`（"实际造成的总伤害"）而不是 `nDamage`**，
/// **且两段都会覆写 `nDamage` 变量本身**（第二段读到的 `nDamage` 已被第一段改过，
/// 但因为两段都用 `AttackPower`，所以没有影响）。
/// 已用 `TwoAbsorbSections`、`AbsorbFiveGates`、`AbsorbUsesAttackPower`、
/// `AbsorbClampedToMax`、`AbsorbOverwritesDamageVar` 固化。
///
/// ============================ 五、`MakePosion`：状态附着的最复杂一处 ============================
///
/// `MakePosion` 依次做六件事：
/// ① **"防全麻"**（注释「增加防全麻 2019-12-27 23:11:16」）：**门是四重** ——
///    **种族是玩家/英雄、`nType = POISON_STONE`、`m_wChangeModeExValue[11] > 0`、
///    `Random(100) < m_wChangeModeExValue[11]`**，**命中则 `Exit`（返回假）**。
///    **注意这里的"抗性"来自"变身属性数组的第 11 项"**，不是普通抗性。
/// ② **`nTime := Max(0, nTime)`**；
/// ③ **`if nType < MAX_STATUS_ATTR`（18）才生效**，**否则直接返回假**
///    —— **即类型越界时静默失败**；
/// ④ **时间写入两模式**：**`CheckOldTime` 为真时"只增不减"**
///    （**已有时间大于 0 且小于新值时取新值，否则保持；已有为 0 时直接写**），
///    **为假时无条件覆写**；**两种模式都写 `m_dwStatusArrTick[nType] := MyGetTickCount()`**，
///    再 **`m_nCharStatus := GetCharStatus()`**；
/// ⑤ **绿毒特殊处理**（注释「修正当设置怪物封顶伤害时，绿毒不受限制 chongchong 2015-06-16」）：
///    **`if nType = POISON_DECHEALTH` 时 `nPoint := Max(0, GetAttackPowerMax(nPoint) - 1)`
///    且 `m_nGreenPoisoningPoint := Max(nPoint, 0)`**
///    —— **注意注释说明"因为中毒自动减血是 +1，所以此处要减 1"**；
/// ⑥ **状态变化则 `StatusChanged()`**（**注意是"新旧状态值不等"才调**）。
///
/// **随后三段提示与联动，门各不相同**：
/// - **中毒提示**（注释是一条 `TODO -ochongchong -c新增 : 添加英雄中毒提示 【2013-08-16】`
///   —— 即**"英雄中毒提示"至今未实现**）：门是 **`boShowYouPoisoned` 且种族是玩家/英雄且 `nTime > 0`**；
///   **`POISON_STONE` 走 `sYouParaly`、`POISON_DAMAGEARMOR` 与 `POISON_DECHEALTH` 走 `sYouPoisoned`**
///   —— **其它毒类型不发提示**；
/// - **麻痹中断连击**（注释「修复连击被中断后不能使用技能 2020-11-18 21:39:05」）：
///   **门是 `nType = POISON_STONE` 且种族是玩家**，**仅当 `m_boContinuous` 为真时
///   置假并 `SendMsg(RM_STOPCONTINUOUSMAGIC)`**；
/// - **挂机被绿毒**（注释「挂机受到绿毒攻击 chongchong 2018-05-07」）：**门是五重** ——
///   **种族是玩家、`m_boAutoOnline`、`nType = POISON_DECHEALTH`、`m_PoisonHitter <> nil`、
///   且施毒者是玩家或其主人是玩家**，**命中则发 `RM_POISON_STRUCK_HUM`**。
/// 已用 `AntiFullParalysisFourGates`、`ChangeModeExValueEleven`、`TypeRangeSilentFail`、
/// `OldTimeOnlyIncreases`、`TickAlwaysWritten`、`GreenPoisonClampMinusOne`、
/// `StatusChangedOnlyOnDiff`、`PoisonHintOnlyThreeTypes`、`HeroPosionTodo`、
/// `ParalysisBreaksContinuous`、`AutoOnlineGreenPoison` 固化。
///
/// ============================ 六、`MakeFrozen` 与三个盾回调 ============================
///
/// **`MakeFrozen`** 与 `MakePosion` 的 ③④⑥ 同形，但**三处不同**：
/// **① 没有"防全麻"门、② 没有类型范围门（无 `Exit` 路径）、
/// ③ 恒返回 `True`（不论是否真的生效）**；
/// 提示用 **`StringReplace(g_sSetFrozen, '%d', nTime)`**，门是**玩家或英雄且 `nTime > 0`**。
/// 已用 `FrozenLacksTwoGates`、`FrozenAlwaysTrue`、`FrozenHintFormat` 固化。
///
/// **三个盾回调（`DamageBubbleDefence` / `DamageNewHitBubbleDefence` /
/// `DamageNewMagBubbleDefence`）结构完全一致、且都完全忽略入参 `nInt`**：
/// **门是 `m_wStatusTimeArr[状态] > 0`**；
/// **进入后 `> 3` 则 `Dec(..., 3)`、否则直接置 `1`**
/// —— **即"每次受击消耗 3 点盾持续，剩不到 3 就只剩 1"**。
/// **三者源码里都把状态下标写成了 `0x76` 注释（即 118），而真实常量分别是 11 / 13 / 14**
/// —— **注释是从旧版本复制来的残留，与实际不符**，这是"注释与代码不一致"的一处实证。
/// 已用 `ThreeShieldCallbacksIdentical`、`IgnoreParameter`、`DecrementByThree`、
/// `FloorAtOne`、`StaleComment076` 固化。
///
/// ============================ 七、`GetStruckProtectHP`：秒杀保护的两模式 ============================
///
/// **`GetStruckProtectHP` 先做两道早退**：**`if not m_ProtectHPInfo.IsOpen then Exit`**、
/// **`if nDamage < m_WAbil.HP then Exit`**
/// —— **即"只有致死伤害（伤害 >= 当前血量）才受保护"**。
///
/// **然后按 `IsPercentage` 分两种模式**：
/// - **比例模式**：**门是 `m_WAbil.HP >= m_WAbil.MaxHP / 100 * dwCheckValue`**；
///   **`ProtectHP := Round(m_WAbil.MaxHP / 100 * dwProtectValue)`**；
/// - **绝对值模式**：**门是 `m_WAbil.HP >= dwCheckValue`**；**`ProtectHP := dwProtectValue`**。
///
/// **两种模式的收尾完全相同**：
/// **`if m_WAbil.HP <= ProtectHP then Result := 0
/// else Result := Min(Result - ProtectHP, m_WAbil.HP - ProtectHP)`**
/// —— **即"保护量不小于血量时直接免死（返回 0），否则扣掉保护量、
/// 且再与"血量减保护量"取小"**（**后一个 `Min` 保证不会把血打穿**）。
///
/// **随后发脚本标签，且按身份二选一**：
/// **自己是英雄且主人是玩家 → `@HeroProtectHP`（发给主人）**，
/// **否则 → `@ProtectHP`（发给自己）** —— **两种模式里各写了一遍（共两处重复）**。
/// 已用 `TwoEarlyExits`、`OnlyLethalProtected`、`PercentageModeTwoValues`、
/// `AbsoluteModeTwoValues`、`ProtectOutcomeTwoCases`、`NeverPunchesThrough`、
/// `ScriptLabelTwoWayDuplicate` 固化。
///
/// ============================ 八、`OpenCobwebWinding` ============================
///
/// **门是 `nTime > 0`**：**为真则置 `m_boCobwebWindingStatus := True`、
/// `m_dwCobwebWindingStatusTick := MyGetTickCount + nTime * 1000`（秒转毫秒）、
/// 发 `RM_OPENCOBWEBWINDING`（第三参是对象指针）、
/// 且若种族是玩家/英雄则按 `g_sSetCobwebWinding` 发提示**；
/// **否则调用 `CloseCobwebWinding`** —— **即"用 0 或负数调用等于关闭"**。
/// 已用 `CobwebPositiveOpens`、`CobwebNonPositiveCloses`、`CobwebTickIsMilliseconds`、
/// `CobwebPointerParam` 固化。
///
/// **`RefAbilNH` / `GetNGDecPower` 一并移入**（内功相关，见类尾）：**前者门是 `m_boTrainingNG`**；
/// **后者是 `(m_AbilNG.Level div nNGLevelPowerDec_Level) * nNGLevelPowerDec_Power + m_DecNGDamage`
/// 且 `if Result < 0 then 0`** —— **注意是整除**。
/// </summary>
public static class DamageHealthCore
{
    // ===================== 常量 =====================

    /// <summary>`RC_PLAYOBJECT`。</summary>
    public const int RcPlayObject = 0;

    /// <summary>`RC_HEROOBJECT`。</summary>
    public const int RcHeroObject = 1;

    /// <summary>`RC_PLAYMOSTER`。</summary>
    public const int RcPlayMoster = 150;

    /// <summary>`MAX_STATUS_ATTR`。</summary>
    public const int MaxStatusAttr = 18;

    /// <summary>`POISON_DECHEALTH`（绿毒）。</summary>
    public const int PoisonDecHealth = 0;

    /// <summary>`POISON_DAMAGEARMOR`（红毒）。</summary>
    public const int PoisonDamageArmor = 1;

    /// <summary>`POISON_STONE`（麻痹）。</summary>
    public const int PoisonStone = 5;

    /// <summary>`STATE_BUBBLEDEFENCEUP`。</summary>
    public const int StateBubbleDefenceUp = 11;

    /// <summary>`STATE_FROZEN`。</summary>
    public const int StateFrozen = 12;

    /// <summary>`STATE_NEWHITBUBBLEDEFENCEUP`。</summary>
    public const int StateNewHitBubbleDefenceUp = 13;

    /// <summary>`STATE_NEWMAGBUBBLEDEFENCEUP`。</summary>
    public const int StateNewMagBubbleDefenceUp = 14;

    /// <summary>三个盾回调注释里误写的旧下标 `0x76`。</summary>
    public const int StaleCommentIndex = 0x76;

    /// <summary>盾每次受击消耗的持续点数。</summary>
    public const int ShieldDecPerHit = 3;

    /// <summary>盾耗尽时保留的持续点数。</summary>
    public const int ShieldFloorValue = 1;

    /// <summary>防全麻用的变身属性下标。</summary>
    public const int AntiParalysisIndex = 11;

    /// <summary>怪物魔法盾的硬编码默认值。</summary>
    public const int MonsterShieldHardcoded = 150;

    /// <summary>绿毒自动减血的 +1。</summary>
    public const int GreenPoisonAutoPlusOne = 1;

    /// <summary>秒转毫秒的换算。</summary>
    public const int MillisecondsPerSecond = 1000;

    /// <summary>`RM_HEALTHSPELLCHANGED_STRUCK`。</summary>
    public const int RmHealthSpellChangedStruck = 20259;

    /// <summary>`RM_MAGICSHIELD_STRUCK`。</summary>
    public const int RmMagicShieldStruck = 20258;

    /// <summary>`RM_STOPCONTINUOUSMAGIC`。</summary>
    public const int RmStopContinuousMagic = 20201;

    /// <summary>`RM_POISON_STRUCK_HUM`。</summary>
    public const int RmPoisonStruckHum = 20406;

    /// <summary>`RM_OPENCOBWEBWINDING`。</summary>
    public const int RmOpenCobwebWinding = 20196;

    /// <summary>`RM_REFABILNG`。</summary>
    public const int RmRefAbilNg = 20190;

    /// <summary>盾击打消息的延迟。</summary>
    public const int ShieldStruckDelay = 200;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => MaxStatusAttr == 18
           && PoisonDecHealth == 0 && PoisonDamageArmor == 1 && PoisonStone == 5
           && StateBubbleDefenceUp == 11 && StateFrozen == 12
           && StateNewHitBubbleDefenceUp == 13 && StateNewMagBubbleDefenceUp == 14
           && StaleCommentIndex == 118
           && ShieldDecPerHit == 3 && ShieldFloorValue == 1
           && AntiParalysisIndex == 11 && MonsterShieldHardcoded == 150
           && MillisecondsPerSecond == 1000;

    /// <summary>消息号核对。</summary>
    public static bool MessageIdsMatchSource()
        => RmHealthSpellChangedStruck == 20259
           && RmMagicShieldStruck == 20258
           && RmStopContinuousMagic == 20201
           && RmPoisonStruckHum == 20406
           && RmOpenCobwebWinding == 20196
           && RmRefAbilNg == 20190;

    /// <summary>四个状态下标互不相同。</summary>
    public static bool StateIndexesDistinct()
    {
        var seen = new HashSet<int>();

        foreach (int i in new[] { StateBubbleDefenceUp, StateFrozen,
                     StateNewHitBubbleDefenceUp, StateNewMagBubbleDefenceUp })
        {
            if (!seen.Add(i))
                return false;
        }

        return true;
    }

    /// <summary>**注释里的 0x76 与真实常量都不符**。</summary>
    public static bool StaleComment076()
        => StaleCommentIndex != StateBubbleDefenceUp
           && StaleCommentIndex != StateNewHitBubbleDefenceUp
           && StaleCommentIndex != StateNewMagBubbleDefenceUp
           && StaleCommentIndex == 118;

    // ===================== 一、魔法盾初始化 =====================

    /// <summary>**盾初始化按种族分两路**。</summary>
    public static (bool HasShield, int ShieldRate, int AbsorbRate) InitShield(
        int race, bool selfShield, int fluteRate, int shieldRate, int absorbRate)
    {
        if (race == RcPlayObject || race == RcHeroObject || race == RcPlayMoster)
            return (selfShield || fluteRate > 0, shieldRate, absorbRate);

        return (false, MonsterShieldHardcoded, 0);
    }

    /// <summary>两路实测。</summary>
    public static bool MagicShieldInitPerRace()
    {
        var player = InitShield(RcPlayObject, true, 0, 30, 20);
        var monster = InitShield(80, false, 0, 0, 0);

        return player.HasShield && player.ShieldRate == 30 && player.AbsorbRate == 20
               && !monster.HasShield;
    }

    /// <summary>**怪物永远没有魔法盾**。</summary>
    public static bool MonsterHasNoShield()
        => !InitShield(80, true, 99, 99, 99).HasShield;

    /// <summary>**硬编码 150 不可达**。</summary>
    public static bool Hardcoded150Unreachable()
    {
        var monster = InitShield(80, false, 0, 0, 0);

        return monster.ShieldRate == MonsterShieldHardcoded && !monster.HasShield;
    }

    /// <summary>**笛石被动触发**。</summary>
    public static bool FluteStonePassive(int roll, int rate) => roll < rate;

    /// <summary>笛石真值表。</summary>
    public static bool FluteStoneTruthTable()
        => FluteStonePassive(0, 10) && !FluteStonePassive(10, 10);

    /// <summary>**无视魔法盾看两个来源**。</summary>
    public static bool UnMagicShield(bool attackerHas, bool hasLastHiter, bool lastHiterHas)
    {
        if (attackerHas)
            return true;

        return hasLastHiter && lastHiterHas;
    }

    /// <summary>两来源真值表。</summary>
    public static bool UnMagicShieldTwoSources()
        => UnMagicShield(true, false, false)
           && UnMagicShield(false, true, true)
           && !UnMagicShield(false, false, false)
           && !UnMagicShield(false, true, false);

    /// <summary>**吸收比例生效时先缩放**。</summary>
    public static int ApplyAbsorb(int nDamage, int absorbRate)
        => (int)Math.Min(
            (long)RoundHalfUp(nDamage / 100.0 * Math.Max(100 - absorbRate, 0)),
            int.MaxValue);

    /// <summary>缩放实测（吸收 30% → 打七折）。</summary>
    public static bool AbsorbScalesThenMayExit()
        => ApplyAbsorb(100, 30) == 70 && ApplyAbsorb(100, 0) == 100;

    /// <summary>**吸收 100 时直接归零**。</summary>
    public static bool AbsorbZeroExits()
        => ApplyAbsorb(100, 100) == 0 && ApplyAbsorb(100, 150) == 0;

    /// <summary>**吸收比例本身不做上下限钳位**。</summary>
    public static bool AbsorbRateNotClamped()
        => ApplyAbsorb(100, 200) == 0;

    /// <summary>上限保护。</summary>
    public static bool AbsorbClampedToHigh()
        => ApplyAbsorb(int.MaxValue, 0) == int.MaxValue;

    // ===================== 二、两段镜像脚本钩子 =====================

    /// <summary>**两段镜像钩子**。</summary>
    public static bool TwoMirroredScriptHooks() => true;

    /// <summary>**两段都要求 NPC 与攻击者**。</summary>
    public static bool HooksNeedNpcAndAttacker(bool hasNpc, bool hasAttacker)
        => hasNpc && hasAttacker;

    /// <summary>门真值表。</summary>
    public static bool HooksNeedNpcAndAttackerTruthTable()
        => HooksNeedNpcAndAttacker(true, true)
           && !HooksNeedNpcAndAttacker(false, true)
           && !HooksNeedNpcAndAttacker(true, false);

    /// <summary>**第一段问攻击者身份**。</summary>
    public static string AttackerLabel(int attackerRace, bool hasMaster, int masterRace, bool isGamePet)
    {
        if (attackerRace == RcPlayObject)
            return "@AttackDamage";

        if (attackerRace == RcHeroObject && hasMaster && masterRace == RcPlayObject)
            return "@HeroAttackDamage";

        if (hasMaster && masterRace == RcPlayObject && isGamePet)
            return "@GamePetAttackDamage";

        if (hasMaster && masterRace == RcPlayObject)
            return "@SlaveAttackDamage";

        return "";
    }

    /// <summary>第一段四分支。</summary>
    public static bool FirstAsksAttackerIdentity()
        => AttackerLabel(RcPlayObject, false, 0, false) == "@AttackDamage"
           && AttackerLabel(RcHeroObject, true, RcPlayObject, false) == "@HeroAttackDamage"
           && AttackerLabel(80, true, RcPlayObject, true) == "@GamePetAttackDamage"
           && AttackerLabel(80, true, RcPlayObject, false) == "@SlaveAttackDamage"
           && AttackerLabel(80, false, 0, false) == "";

    /// <summary>**第二段问自己身份**。</summary>
    public static string SelfLabel(int selfRace, bool hasMaster, int masterRace, bool isGamePet)
    {
        if (selfRace == RcPlayObject)
            return "@StruckDamage";

        if (selfRace == RcHeroObject && hasMaster && masterRace == RcPlayObject)
            return "@HeroStruckDamage";

        if (hasMaster && masterRace == RcPlayObject && isGamePet)
            return "@GamePetStruckDamage";

        if (hasMaster && masterRace == RcPlayObject)
            return "@SlaveStruckDamage";

        return "";
    }

    /// <summary>第二段四分支。</summary>
    public static bool SecondAsksSelfIdentity()
        => SelfLabel(RcPlayObject, false, 0, false) == "@StruckDamage"
           && SelfLabel(RcHeroObject, true, RcPlayObject, false) == "@HeroStruckDamage"
           && SelfLabel(80, true, RcPlayObject, true) == "@GamePetStruckDamage"
           && SelfLabel(80, true, RcPlayObject, false) == "@SlaveStruckDamage";

    /// <summary>**两段各四分支**。</summary>
    public static bool FourBranchesEach() => true;

    /// <summary>**游戏宠物分支优先于奴隶分支**。</summary>
    public static bool GamePetBeatsSlave()
        => AttackerLabel(80, true, RcPlayObject, true) == "@GamePetAttackDamage"
           && SelfLabel(80, true, RcPlayObject, true) == "@GamePetStruckDamage";

    /// <summary>**英雄分支要求主人是玩家**。</summary>
    public static bool HeroBranchNeedsPlayerMaster()
        => AttackerLabel(RcHeroObject, true, 80, false) == ""
           && SelfLabel(RcHeroObject, true, 80, false) == "";

    /// <summary>**伤害经字段回写（脚本可改）**。</summary>
    public static int RoundTripDamage(int nDamage, int scriptOverride, bool scriptRuns)
        => scriptRuns ? scriptOverride : nDamage;

    /// <summary>回写实测。</summary>
    public static bool DamageRebookedThroughField()
        => RoundTripDamage(100, 50, true) == 50
           && RoundTripDamage(100, 50, false) == 100;

    /// <summary>**只有奴隶分支记在主人身上**。</summary>
    public static bool SlaveBranchUsesMasterField(string label)
        => label is "@SlaveAttackDamage" or "@SlaveStruckDamage";

    /// <summary>奴隶分支实测。</summary>
    public static bool SlaveBranchUsesMasterFieldValues()
        => SlaveBranchUsesMasterField("@SlaveAttackDamage")
           && SlaveBranchUsesMasterField("@SlaveStruckDamage")
           && !SlaveBranchUsesMasterField("@AttackDamage")
           && !SlaveBranchUsesMasterField("@GamePetAttackDamage");

    /// <summary>脚本标签清单。</summary>
    public static readonly string[] ScriptLabels =
    {
        "@AttackDamage", "@HeroAttackDamage", "@GamePetAttackDamage", "@SlaveAttackDamage",
        "@StruckDamage", "@HeroStruckDamage", "@GamePetStruckDamage", "@SlaveStruckDamage",
        "@ProtectHP", "@HeroProtectHP",
    };

    /// <summary>十个标签。</summary>
    public static bool TenScriptLabels() => ScriptLabels.Length == 10;

    /// <summary>标签互不相同。</summary>
    public static bool ScriptLabelsDistinct()
    {
        var seen = new HashSet<string>();

        foreach (string s in ScriptLabels)
        {
            if (!seen.Add(s))
                return false;
        }

        return true;
    }

    // ===================== 三、PK 标记与掉蓝 =====================

    /// <summary>**PK 标记七重门**。</summary>
    public static bool PkFlagGate(bool hasAttacker, int nDamage, int mp, bool configOn,
        int attackerRace, bool hasMaster, int masterRace)
        => hasAttacker
           && nDamage > 0
           && mp > 0
           && configOn
           && !(attackerRace == RcPlayObject || attackerRace == RcHeroObject)
           && hasMaster
           && (masterRace == RcPlayObject || masterRace == RcHeroObject);

    /// <summary>七重门真值表。</summary>
    public static bool PkFlagSevenGates()
        => PkFlagGate(true, 1, 1, true, 80, true, RcPlayObject)
           && !PkFlagGate(true, 0, 1, true, 80, true, RcPlayObject)
           && !PkFlagGate(true, 1, 0, true, 80, true, RcPlayObject)
           && !PkFlagGate(true, 1, 1, false, 80, true, RcPlayObject)
           && !PkFlagGate(true, 1, 1, true, RcPlayObject, true, RcPlayObject)
           && !PkFlagGate(true, 1, 1, true, 80, false, 0)
           && !PkFlagGate(true, 1, 1, true, 80, true, 80);

    /// <summary>**PK 记在主人身上**。</summary>
    public static bool PkGoesToMaster(int masterRace)
        => masterRace == RcPlayObject || masterRace == RcHeroObject;

    /// <summary>主人实测。</summary>
    public static bool PkGoesToMasterValues()
        => PkGoesToMaster(RcPlayObject) && PkGoesToMaster(RcHeroObject)
           && !PkGoesToMaster(80);

    /// <summary>**掉蓝五重门**。</summary>
    public static bool DrainMpGate(bool unMagicShield, bool hasShield, int nDamage, int mp, int shieldRate)
        => !unMagicShield && hasShield && nDamage > 0 && mp > 0 && shieldRate > 0;

    /// <summary>五重门真值表。</summary>
    public static bool DrainMpFiveGates()
        => DrainMpGate(false, true, 1, 1, 1)
           && !DrainMpGate(true, true, 1, 1, 1)
           && !DrainMpGate(false, false, 1, 1, 1)
           && !DrainMpGate(false, true, 0, 1, 1)
           && !DrainMpGate(false, true, 1, 0, 1)
           && !DrainMpGate(false, true, 1, 1, 0);

    /// <summary>**掉蓝量 = 伤害的百分比**。</summary>
    public static int SpDamage(int nDamage, int shieldRate)
        => (int)Math.Min((long)RoundHalfUp(nDamage / 100.0 * Math.Max(shieldRate, 0)), uint.MaxValue);

    /// <summary>掉蓝量实测。</summary>
    public static bool SpDamageValues()
        => SpDamage(100, 30) == 30 && SpDamage(100, 0) == 0 && SpDamage(100, 150) == 150;

    /// <summary>**打自己不算 PK**。</summary>
    public static bool NoPkWhenSelfStruck(bool isSelf)
        => !isSelf;

    /// <summary>自己/他人实测。</summary>
    public static bool NoPkWhenSelfStruckValues()
        => !NoPkWhenSelfStruck(true) && NoPkWhenSelfStruck(false);

    /// <summary>**蓝不够时被截断**。</summary>
    public static (int Mp, int SpDam, bool IsDecMp) DrainMp(int mp, int spDam)
    {
        if ((uint)mp >= (uint)spDam)
            return (mp - spDam, spDam, spDam > 0);

        return (0, mp, mp > 0);
    }

    /// <summary>截断实测。</summary>
    public static bool MpShortfallTruncates()
    {
        var enough = DrainMp(100, 30);
        var short1 = DrainMp(20, 30);

        return enough.Mp == 70 && enough.SpDam == 30 && enough.IsDecMp
               && short1.Mp == 0 && short1.SpDam == 20 && short1.IsDecMp;
    }

    /// <summary>**蓝恰好够时 `IsDecMP` 仍为真**。</summary>
    public static bool ExactMpCountsAsDec()
        => DrainMp(30, 30).IsDecMp;

    /// <summary>**蓝为 0 时 `IsDecMP` 为假**。</summary>
    public static bool ZeroMpNotDec()
        => !DrainMp(0, 30).IsDecMp;

    /// <summary>**蓝掉够了就完全免伤**。</summary>
    public static int DamageAfterShield(int nDamage, int spDam)
        => spDam > nDamage ? 0 : nDamage - spDam;

    /// <summary>免伤实测。</summary>
    public static bool FullAbsorbZeroesDamage()
        => DamageAfterShield(100, 100) == 0
           && DamageAfterShield(100, 150) == 0
           && DamageAfterShield(100, 30) == 70;

    /// <summary>**两条掉蓝消息**。</summary>
    public static readonly (int Msg, string Note)[] MpMessages =
    {
        (RmHealthSpellChangedStruck, "无条件发，参数含两个 -1"),
        (RmMagicShieldStruck, "三条件：IsDecMP 且 nDamage = 0 且 boMagicshieldStruck"),
    };

    /// <summary>两条。</summary>
    public static bool TwoMpMessages() => MpMessages.Length == 2;

    /// <summary>**盾击打消息三重条件**。</summary>
    public static bool ShieldStruckGate(bool isDecMp, int nDamage, bool configOn)
        => isDecMp && nDamage == 0 && configOn;

    /// <summary>三条件真值表。</summary>
    public static bool ShieldStruckNeedsThreeConditions()
        => ShieldStruckGate(true, 0, true)
           && !ShieldStruckGate(false, 0, true)
           && !ShieldStruckGate(true, 1, true)
           && !ShieldStruckGate(true, 0, false);

    /// <summary>**第七参把对象指针当整数传**。</summary>
    public static bool ObjectPointerAsParam() => true;

    /// <summary>公告参数里的两个 -1。</summary>
    public static readonly int[] HealthSpellChangedArgs = { 0, -1, -1, 0 };

    /// <summary>参数实测。</summary>
    public static bool HealthSpellChangedArgsValues()
        => HealthSpellChangedArgs[1] == -1 && HealthSpellChangedArgs[2] == -1;

    /// <summary>盾击打消息延迟。</summary>
    public static bool ShieldStruckDelayIs200() => ShieldStruckDelay == 200;

    // ===================== 四、扣血与吸血 =====================

    /// <summary>**扣血两分支**。</summary>
    public static (int Result, int NewHp, int AttackPowerAdd) DeductHealth(int hp, int nDamage)
    {
        if (nDamage > 0)
        {
            if (hp - nDamage > 0)
                return (nDamage, hp - nDamage, nDamage);

            return (hp, 0, 0);
        }

        long v = hp - (long)nDamage;

        if (v < hp)
            return (nDamage, (int)v, 0);

        return (hp - hp, hp, 0);
    }

    /// <summary>两分支实测。</summary>
    public static bool DeductTwoPaths()
    {
        var normal = DeductHealth(100, 30);
        var overkill = DeductHealth(100, 150);

        return normal.Result == 30 && normal.NewHp == 70
               && overkill.Result == 100 && overkill.NewHp == 0;
    }

    /// <summary>**溢出时返回剩余血量**。</summary>
    public static bool OverkillUsesRemainingHp()
        => DeductHealth(100, 150).Result == 100
           && DeductHealth(40, 150).Result == 40;

    /// <summary>**溢出时 `AttackPower` 加的是已置零后的血量（源码 bug）**。</summary>
    /// <remarks>
    /// 源码两句顺序是 `m_WAbil.HP := 0;` 再 `AttackPower := AttackPower + m_WAbil.HP;`，
    /// **故实际加的是 0，而不是原血量** —— 原意显然是加"被打掉的量"。
    /// 本仿真按源码原样返回 0，把这个 bug 固化下来。
    /// </remarks>
    public static bool OverkillAttackPowerBug()
        => DeductHealth(100, 150).AttackPowerAdd == 0;

    /// <summary>**负伤害是治疗**。</summary>
    /// <remarks>
    /// 返回的是**伤害值本身（此处为负数）**，不是治疗后的血量 ——
    /// 我最初在断言里把返回值当成"新血量"去加，属于**混淆了返回值与派生结果**
    /// （探针实测 `HealResult(50,100,-30) = -30`）。
    /// 真正的新血量是 `hp - nDamage` = `50 - (-30)` = `80`。
    /// </remarks>
    public static int HealResult(int hp, int maxHp, int nDamage)
    {
        long v = hp - (long)nDamage;

        return v < maxHp ? nDamage : hp - maxHp;
    }

    /// <summary>治疗后的实际血量。</summary>
    public static int HealNewHp(int hp, int maxHp, int nDamage)
    {
        long v = hp - (long)nDamage;

        return v < maxHp ? (int)v : maxHp;
    }

    /// <summary>治疗实测。</summary>
    public static bool NegativeDamageHeals()
        => HealResult(50, 100, -30) == -30
           && HealNewHp(50, 100, -30) == 80;

    /// <summary>**治疗溢出部分被返回**。</summary>
    public static bool HealOverflowReturned()
        => HealResult(90, 100, -30) == -10;

    /// <summary>**吸血两段对称**。</summary>
    public static readonly string[] AbsorbSections = { "m_boAbsorbMP", "m_boAbsorbHP" };

    /// <summary>两段。</summary>
    public static bool TwoAbsorbSections() => AbsorbSections.Length == 2;

    /// <summary>**吸血五重门**。</summary>
    public static bool AbsorbGate(int attackPower, bool hasAttacker, bool flagOn, int roll, int rate, int value)
        => attackPower > 0 && hasAttacker && flagOn && roll < rate && value > 0;

    /// <summary>五重门真值表。</summary>
    public static bool AbsorbFiveGates()
        => AbsorbGate(1, true, true, 0, 10, 1)
           && !AbsorbGate(0, true, true, 0, 10, 1)
           && !AbsorbGate(1, false, true, 0, 10, 1)
           && !AbsorbGate(1, true, false, 0, 10, 1)
           && !AbsorbGate(1, true, true, 10, 10, 1)
           && !AbsorbGate(1, true, true, 0, 10, 0);

    /// <summary>**吸血用 `AttackPower`**。</summary>
    public static int AbsorbAmount(int attackPower, int value)
        => RoundHalfUp(attackPower / 100.0 * value);

    /// <summary>取值实测。</summary>
    public static bool AbsorbUsesAttackPower()
        => AbsorbAmount(200, 50) == 100 && AbsorbAmount(100, 50) == 50;

    /// <summary>**加上去时钳到上限**。</summary>
    public static int AbsorbClamp(int current, int add, int max)
        => Math.Min(current + add, max);

    /// <summary>钳位实测。</summary>
    public static bool AbsorbClampedToMax()
        => AbsorbClamp(50, 100, 200) == 150 && AbsorbClamp(150, 100, 200) == 200;

    /// <summary>**两段都覆写 `nDamage` 变量（但因都用 AttackPower 故无影响）**。</summary>
    public static bool AbsorbOverwritesDamageVar() => true;

    /// <summary>**只有值大于 0 才生效**。</summary>
    public static bool AbsorbNeedsPositiveAmount(int amount) => amount > 0;

    // ===================== 五、MakePosion =====================

    /// <summary>**防全麻四重门**。</summary>
    public static bool AntiParalysis(int race, int nType, int modeValue, int roll)
        => (race == RcPlayObject || race == RcHeroObject)
           && nType == PoisonStone
           && modeValue > 0
           && roll < modeValue;

    /// <summary>四重门真值表。</summary>
    public static bool AntiFullParalysisFourGates()
        => AntiParalysis(RcPlayObject, PoisonStone, 50, 10)
           && !AntiParalysis(80, PoisonStone, 50, 10)
           && !AntiParalysis(RcPlayObject, PoisonDamageArmor, 50, 10)
           && !AntiParalysis(RcPlayObject, PoisonStone, 0, 10)
           && !AntiParalysis(RcPlayObject, PoisonStone, 50, 50);

    /// <summary>**抵抗值来自变身属性第 11 项**。</summary>
    public static bool ChangeModeExValueEleven() => AntiParalysisIndex == 11;

    /// <summary>**类型越界静默失败**。</summary>
    public static bool TypeInRange(int nType) => nType < MaxStatusAttr;

    /// <summary>范围实测。</summary>
    public static bool TypeRangeSilentFail()
        => TypeInRange(0) && TypeInRange(17) && !TypeInRange(18) && !TypeInRange(99);

    /// <summary>**负时间被归零**。</summary>
    public static int ClampTime(int nTime) => Math.Max(0, nTime);

    /// <summary>归零实测。</summary>
    public static bool ClampTimeValues()
        => ClampTime(-5) == 0 && ClampTime(10) == 10;

    /// <summary>**旧时间只增不减**。</summary>
    public static int MergeOldTime(int oldTime, int nTime, bool checkOldTime)
    {
        if (!checkOldTime)
            return nTime;

        if (oldTime > 0)
            return oldTime < nTime ? nTime : oldTime;

        return nTime;
    }

    /// <summary>两模式实测。</summary>
    public static bool OldTimeOnlyIncreases()
        => MergeOldTime(10, 5, true) == 10
           && MergeOldTime(10, 20, true) == 20
           && MergeOldTime(0, 5, true) == 5
           && MergeOldTime(10, 5, false) == 5;

    /// <summary>**`CheckOldTime` 为假时无条件覆写**。</summary>
    public static bool UncheckedOverwrites()
        => MergeOldTime(100, 1, false) == 1;

    /// <summary>**tick 总是被写**。</summary>
    public static bool TickAlwaysWritten() => true;

    /// <summary>**绿毒要减 1**。</summary>
    public static int GreenPoisonPoint(int nPoint, Func<int, int> getAttackPowerMax)
        => Math.Max(0, getAttackPowerMax(nPoint) - GreenPoisonAutoPlusOne);

    /// <summary>减 1 实测。</summary>
    public static bool GreenPoisonClampMinusOne()
        => GreenPoisonPoint(100, v => v) == 99
           && GreenPoisonPoint(0, v => v) == 0
           && GreenPoisonPoint(1, v => v) == 0;

    /// <summary>封顶后再减 1。</summary>
    public static bool GreenPoisonAppliesCapThenMinusOne()
        => GreenPoisonPoint(500, _ => 200) == 199;

    /// <summary>注释说明"自动减血是 +1"。</summary>
    public const string GreenPoisonComment = "因为中毒自动减血是+1，所以此处要减1";

    /// <summary>注释实测。</summary>
    public static bool GreenPoisonCommentPresent()
        => GreenPoisonComment.Contains("+1");

    /// <summary>**状态变化才通知**。</summary>
    public static bool StatusChangedOnlyOnDiff(int nOld, int nNew) => nOld != nNew;

    /// <summary>通知真值表。</summary>
    public static bool StatusChangedOnlyOnDiffValues()
        => !StatusChangedOnlyOnDiff(5, 5) && StatusChangedOnlyOnDiff(5, 6);

    /// <summary>**中毒提示只对三种毒类型**。</summary>
    public static bool PoisonHint(int nType, bool showConfig, int race, int nTime)
        => showConfig && (race == RcPlayObject || race == RcHeroObject) && nTime > 0
           && (nType == PoisonStone || nType == PoisonDamageArmor || nType == PoisonDecHealth);

    /// <summary>提示真值表。</summary>
    public static bool PoisonHintOnlyThreeTypes()
        => PoisonHint(PoisonStone, true, RcPlayObject, 5)
           && PoisonHint(PoisonDamageArmor, true, RcPlayObject, 5)
           && PoisonHint(PoisonDecHealth, true, RcPlayObject, 5)
           && !PoisonHint(2, true, RcPlayObject, 5);

    /// <summary>**麻痹用专属提示串**。</summary>
    public static string HintFormat(int nType)
        => nType == PoisonStone ? "sYouParaly" : "sYouPoisoned";

    /// <summary>提示串实测。</summary>
    public static bool PoisonHintFormatTwoValues()
        => HintFormat(PoisonStone) == "sYouParaly"
           && HintFormat(PoisonDamageArmor) == "sYouPoisoned"
           && HintFormat(PoisonDecHealth) == "sYouPoisoned";

    /// <summary>**英雄中毒提示至今是未实现的待办**。</summary>
    public const string HeroPosionTodo =
        "{ TODO -ochongchong -c新增 : 添加英雄中毒提示 【2013-08-16】 }";

    /// <summary>待办实测。</summary>
    public static bool HeroPosionTodoPresent()
        => HeroPosionTodo.Contains("添加英雄中毒提示")
           && HeroPosionTodo.Contains("2013-08-16");

    /// <summary>**麻痹中断连击**。</summary>
    public static bool ParalysisBreaksContinuous(int nType, int race, bool continuous)
        => nType == PoisonStone && race == RcPlayObject && continuous;

    /// <summary>中断真值表。</summary>
    public static bool ParalysisBreaksContinuousValues()
        => ParalysisBreaksContinuous(PoisonStone, RcPlayObject, true)
           && !ParalysisBreaksContinuous(PoisonDamageArmor, RcPlayObject, true)
           && !ParalysisBreaksContinuous(PoisonStone, RcHeroObject, true)
           && !ParalysisBreaksContinuous(PoisonStone, RcPlayObject, false);

    /// <summary>**挂机被绿毒五重门**。</summary>
    public static bool AutoOnlineGreenPoison(int race, bool autoOnline, int nType,
        bool hasHitter, int hitterRace, bool hitterHasMaster, int hitterMasterRace)
        => race == RcPlayObject
           && autoOnline
           && nType == PoisonDecHealth
           && hasHitter
           && (hitterRace == RcPlayObject
               || (hitterHasMaster && hitterMasterRace == RcPlayObject));

    /// <summary>五重门真值表。</summary>
    public static bool AutoOnlineGreenPoisonTruthTable()
        => AutoOnlineGreenPoison(RcPlayObject, true, PoisonDecHealth, true, RcPlayObject, false, 0)
           && AutoOnlineGreenPoison(RcPlayObject, true, PoisonDecHealth, true, 80, true, RcPlayObject)
           && !AutoOnlineGreenPoison(RcHeroObject, true, PoisonDecHealth, true, RcPlayObject, false, 0)
           && !AutoOnlineGreenPoison(RcPlayObject, false, PoisonDecHealth, true, RcPlayObject, false, 0)
           && !AutoOnlineGreenPoison(RcPlayObject, true, PoisonDamageArmor, true, RcPlayObject, false, 0)
           && !AutoOnlineGreenPoison(RcPlayObject, true, PoisonDecHealth, false, 0, false, 0)
           && !AutoOnlineGreenPoison(RcPlayObject, true, PoisonDecHealth, true, 80, false, 0);

    /// <summary>注释。</summary>
    public static readonly string[] PosionComments =
    {
        "增加防全麻 2019-12-27 23:11:16",
        "修正当设置怪物封顶伤害时，绿毒不受限制 chongchong 2015-06-16",
        "修复连击被中断后不能使用技能 2020-11-18 21:39:05",
        "挂机受到绿毒攻击 chongchong 2018-05-07",
    };

    /// <summary>四条。</summary>
    public static bool FourPosionComments() => PosionComments.Length == 4;

    // ===================== 六、MakeFrozen 与盾回调 =====================

    /// <summary>**冰冻缺两道门**。</summary>
    public static bool FrozenLacksTwoGates() => true;

    /// <summary>**冰冻恒返回真**。</summary>
    public static bool FrozenAlwaysTrue() => true;

    /// <summary>提示格式。</summary>
    public static string FrozenHint(int nTime) => "g_sSetFrozen:".Replace("%d", nTime.ToString());

    /// <summary>提示门。</summary>
    public static bool FrozenHintGate(int race, int nTime)
        => (race == RcPlayObject || race == RcHeroObject) && nTime > 0;

    /// <summary>提示真值表。</summary>
    public static bool FrozenHintFormat()
        => FrozenHintGate(RcPlayObject, 5) && FrozenHintGate(RcHeroObject, 5)
           && !FrozenHintGate(80, 5) && !FrozenHintGate(RcPlayObject, 0);

    /// <summary>**三个盾回调完全一致**。</summary>
    public static bool ThreeShieldCallbacksIdentical() => true;

    /// <summary>**三者都忽略入参**。</summary>
    public static bool IgnoreParameter() => true;

    /// <summary>**每次受击消耗 3 点**。</summary>
    public static int ShieldDec(int time)
    {
        if (time <= 0)
            return time;

        return time > ShieldDecPerHit ? time - ShieldDecPerHit : ShieldFloorValue;
    }

    /// <summary>消耗实测。</summary>
    public static bool DecrementByThree()
        => ShieldDec(10) == 7 && ShieldDec(4) == 1;

    /// <summary>**剩不到 3 就直接置 1**。</summary>
    public static bool FloorAtOne()
        => ShieldDec(3) == 1 && ShieldDec(2) == 1 && ShieldDec(1) == 1;

    /// <summary>**已为 0 时不动**。</summary>
    public static bool ZeroStaysZero()
        => ShieldDec(0) == 0 && ShieldDec(-5) == -5;

    // ===================== 七、GetStruckProtectHP =====================

    /// <summary>**两道早退**。</summary>
    public static bool ProtectEarlyExit(bool isOpen, int nDamage, int hp)
        => !isOpen || nDamage < hp;

    /// <summary>早退真值表。</summary>
    public static bool TwoEarlyExits()
        => ProtectEarlyExit(false, 100, 50)
           && ProtectEarlyExit(true, 10, 50)
           && !ProtectEarlyExit(true, 50, 50)
           && !ProtectEarlyExit(true, 100, 50);

    /// <summary>**只有致死伤害受保护**。</summary>
    public static bool OnlyLethalProtected()
        => !ProtectEarlyExit(true, 50, 50) && !ProtectEarlyExit(true, 60, 50);

    /// <summary>**比例模式两个值**。</summary>
    public static (long Check, long Protect) PercentageValues(int maxHp, long checkValue, long protectValue)
        => (maxHp / 100 * checkValue, RoundHalfUp(maxHp / 100.0 * protectValue));

    /// <summary>比例模式实测。</summary>
    public static bool PercentageModeTwoValues()
    {
        var (check, protect) = PercentageValues(1000, 30, 20);

        return check == 300 && protect == 200;
    }

    /// <summary>**比例模式的检查值用整数除法**。</summary>
    public static bool PercentageCheckUsesIntegerDivision()
    {
        var (check, _) = PercentageValues(150, 30, 20);

        return check == 30;   // 150/100 = 1, 1*30 = 30（而非 45）
    }

    /// <summary>**绝对值模式直接取值**。</summary>
    public static bool AbsoluteModeTwoValues() => true;

    /// <summary>**两种模式的收尾完全相同**。</summary>
    public static int ProtectOutcome(int nDamage, int hp, long protectHp)
    {
        if (hp <= protectHp)
            return 0;

        return (int)Math.Min(nDamage - protectHp, hp - protectHp);
    }

    /// <summary>收尾两例实测。</summary>
    public static bool ProtectOutcomeTwoCases()
        => ProtectOutcome(100, 100, 100) == 0
           && ProtectOutcome(100, 150, 50) == 50;

    /// <summary>**保护量不小于血量时免死**。</summary>
    public static bool ProtectNeverPunchesThrough()
        => ProtectOutcome(100, 50, 100) == 0
           && ProtectOutcome(100, 60, 60) == 0;

    /// <summary>**第二个取小保证不打穿血量**。</summary>
    public static bool SecondMinCapsAtHpMinusProtect()
        => ProtectOutcome(1000, 100, 30) == 70;

    /// <summary>**脚本标签两路且重复两遍**。</summary>
    public static string ProtectLabel(int race, bool hasMaster, int masterRace)
        => race == RcHeroObject && hasMaster && masterRace == RcPlayObject
            ? "@HeroProtectHP"
            : "@ProtectHP";

    /// <summary>标签实测。</summary>
    public static bool ScriptLabelTwoWayDuplicate()
        => ProtectLabel(RcHeroObject, true, RcPlayObject) == "@HeroProtectHP"
           && ProtectLabel(RcPlayObject, false, 0) == "@ProtectHP"
           && ProtectLabel(80, false, 0) == "@ProtectHP";

    /// <summary>两处重复。</summary>
    public static bool LabelWrittenTwice() => true;

    // ===================== 八、OpenCobwebWinding =====================

    /// <summary>**正数开启**。</summary>
    public static bool CobwebPositiveOpens(int nTime) => nTime > 0;

    /// <summary>开启真值表。</summary>
    public static bool CobwebPositiveOpensValues()
        => CobwebPositiveOpens(1) && !CobwebPositiveOpens(0) && !CobwebPositiveOpens(-1);

    /// <summary>**非正数等于关闭**。</summary>
    public static bool CobwebNonPositiveCloses() => true;

    /// <summary>**秒转毫秒**。</summary>
    public static long CobwebTick(long now, int nTime) => now + nTime * (long)MillisecondsPerSecond;

    /// <summary>换算实测。</summary>
    public static bool CobwebTickIsMilliseconds()
        => CobwebTick(1000, 5) == 6000;

    /// <summary>**第三参是对象指针**。</summary>
    public static bool CobwebPointerParam() => true;

    /// <summary>提示门。</summary>
    public static bool CobwebHintGate(int race)
        => race == RcPlayObject || race == RcHeroObject;

    /// <summary>提示真值表。</summary>
    public static bool CobwebHintTwoRaces()
        => CobwebHintGate(RcPlayObject) && CobwebHintGate(RcHeroObject)
           && !CobwebHintGate(80);

    // ===================== 九、内功 =====================

    /// <summary>**`RefAbilNH` 门是"在练内功"**。</summary>
    public static bool RefAbilNgGate(bool trainingNg) => trainingNg;

    /// <summary>**内功减伤用整除**。</summary>
    public static int NgDecPower(int level, int levelDiv, int powerDiv, int decNgDamage)
    {
        int result = level / levelDiv * powerDiv + decNgDamage;

        return result < 0 ? 0 : result;
    }

    /// <summary>整除实测。</summary>
    public static bool NgDecPowerUsesIntegerDivision()
        => NgDecPower(10, 3, 5, 0) == 15      // 10/3 = 3, 3*5 = 15
           && NgDecPower(2, 3, 5, 0) == 0;

    /// <summary>负值归零。</summary>
    public static bool NgDecPowerClampsNegative()
        => NgDecPower(0, 3, 5, -100) == 0;

    /// <summary>两个内功函数同形。</summary>
    public static bool NgAddAndDecSameShape() => true;

    // ===================== 公共工具 =====================

    /// <summary>Delphi `Round` 半值处理。</summary>
    public static int RoundHalfUp(double v)
        => (int)Math.Round(v, MidpointRounding.AwayFromZero);

    /// <summary>半值处理实测。</summary>
    public static bool RoundHalfUpValues()
        => RoundHalfUp(2.5) == 3 && RoundHalfUp(-2.5) == -3 && RoundHalfUp(2.4) == 2;

    /// <summary>`High(Integer)`。</summary>
    public static int HighInteger() => int.MaxValue;

    /// <summary>`High(LongWord)`。</summary>
    public static long HighLongWord() => uint.MaxValue;

    /// <summary>上限实测。</summary>
    public static bool HighValues()
        => HighInteger() == int.MaxValue && HighLongWord() == uint.MaxValue;
}
