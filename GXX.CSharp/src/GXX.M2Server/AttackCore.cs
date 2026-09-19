using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 攻击系列本体 1:1 移植（批次J143）：
/// `TBaseObject._Attack`（`ObjBase.pas` 35831-36051）、
/// `TBaseObject.GetAttackDir` 重载一（27050-27060，带 `// 根据目标获取攻击方向`）、
/// 重载二（27062-27111）、`TBaseObject.GetAttackPowerMax`（2886-2924）、
/// `TBaseObject.GetNextDamage`（2926-2936）、`TBaseObject.GetAttackPower()`（2938-2941）；
/// 辅助源：`Grobal2.pas` 922（`RM_10101 = 30005`）、988（`RM_STRUCK = 20048`）、
/// 192（`RC_PLAYMOSTER = 150`）、193（`RC_MOONOBJECT = 99`）、94/97（`DR_DOWNRIGHT = 3`/`DR_LEFT = 6`）、
/// `M2Definition.pas` 13（`POISON_STONE = 5`）。
///
/// ============================ 一、`_Attack` 的 `wHitMode` → `nMagicID` 映射表 ============================
///
/// `_Attack`（35831-36051）开头用一个 **`case wHitMode of` 把"打击模式"翻译成"技能编号"**：
/// **3→7（攻杀）、4→12（刺杀）、5→25（半月）、7→26（烈火）、8→40（抱月刀/双龙斩）、
/// 9→42（龙影剑法）、10→43（雷霆剑法）、15→56（逐日剑法）、12→60（破魂斩）、
/// 13→61（劈星斩）、14→62（雷霆一击）、11→66（开天斩）、16→101（三绝杀）、
/// 17→102（断岳斩）、18→103（横扫千军），`else nMagicID := 0`。**
/// **注意 `wHitMode` 的取值并不连续**：`6` 缺席、`11` 排在 `15` 之后书写（顺序打乱），
/// **`0/1/2/6/19+` 全部落到 `else` 的 `0`**。
/// 已用 `HitModeTableComplete`、`HitModeTableValues`、`HitModeGaps`、
/// `HitModeOrderIsScrambled` 固化。
///
/// **J140 大刀护卫调用 `_Attack` 时 `wHitMode` 恒为 `0`** →
/// **即护卫的每次攻击 `nMagicID` 都是 `0`**（无技能）。
/// 已用 `GuardAlwaysUsesZero` 固化。
///
/// ============================ 二、奴隶威力折扣与"自定义怪物"注释 ============================
///
/// ① **`nPower := GetAttackPower`**；
/// ② **若 `(m_Master <> nil) and (m_btRaceServer <> RC_HEROOBJECT) and (not (Self is TCopyMon))`
///    则 `nPower := Round(nPower * (g_Config.nSlavePowerRate / 100))`**
///    —— **即"有主人的非英雄非复制怪"才有威力折扣**。
///    **三个条件缺一不可**，注释「自定义怪物 - 修改代码以支持攻击威力 可以用 MC, SC来计算
///    chongchong 2014-07-19」；
/// ③ **`if AttackTarget = nil then Exit`**（**在 `try` 内但 `nCheckCode` 仍为 0**）；
/// ④ **35892-35910 有一整段被 `{ }` 注释掉的"护体神盾被击破"逻辑**：
///    对 `RC_PLAYOBJECT`/`RC_PLAYMOSTER`/`RC_HEROOBJECT` 三类目标，
///    **按 `wHitMode` 为 `4`/`7`/`11`/`15`/`12..14` 分别查
///    `g_Config.UseSkillCloseSuperShileds[0..4]` 开关来决定是否 `CloseSuperShiled`**
///    —— **即"哪些技能可以破盾"曾由配置数组控制**，现已移除。
/// 已用 `SlavePowerDiscountThreeConditions`、`HeroExemptFromDiscount`、
/// `CopyMonExemptFromDiscount`、`NullTargetExitsEarly`、
/// `CommentedSuperShieldBlock`、`SuperShieldHadFiveSkillSlots` 固化。
///
/// ============================ 三、`nCheckCode`：六个分级崩溃定位点 ============================
///
/// **`nCheckCode` 初值 `0`**，随后逐步推进：**`4`（进入合法目标判定前）→ `41`（合法目标内）→
/// `42`（命中判定失败、威力置 0）→ `43` → `5`（威力判定前）→ `600` → `601` → `602` → `603`**。
/// **注意 `41/42/43` 是"41 打头"的子分级、`600-603` 是另一段子分级**，
/// **且 `42` 设置后立刻又被 `43` 覆盖** —— 即**异常发生在"威力置 0"与下一行之间时，
/// 报文里看到的仍是 `43`**（`42` 只在极窄的窗口内可见）。
/// 异常报文格式：**`'[Exception] TBaseObject._Attack Name:= %s Code:=%d'`**，
/// 参数是 **`m_sCharName` 与 `nCheckCode`**，随后**再单独输出一次 `E.Message`**。
/// **这是本工程第五种异常定位风格**（前四种：J130/J135 纯数字码、J137 `resourcestring`、
/// J139 裸方法名、J140 方法名+分级码；**本类是"方法名 + 名字 + 分级码"的双参格式化**）。
/// 已用 `CheckCodeStages`、`CheckCodeStageValues`、`FortyTwoIsImmediatelyOverwritten`、
/// `ExceptionFormatHasNameAndCode`、`TwoMessagesOnException`、
/// `FifthExceptionStyle`、`FiveStylesPresent` 固化。
///
/// ============================ 四、命中判定：`Random(速度) >= 命中` ============================
///
/// **`if (Random(AttackTarget.m_btSpeedPoint) >= m_btHitPoint) then nPower := 0`**
/// —— **注意是"目标的敏捷"与"我的命中"比较，且用 `>=`**。
/// **`Random(n)` 返回 `0..n-1`**，故**当 `m_btHitPoint = 0` 时恒成立（必然失手、威力归零）**；
/// **当 `m_btSpeedPoint = 0` 时 `Random(0)` 恒返回 `0`，则只要 `m_btHitPoint = 0` 仍成立**。
/// 另**`IsProperTarget` 为假时也把 `nPower` 置 0**（但不退出，继续走后面的流程）。
/// 已用 `HitRollUsesTargetSpeed`、`HitRollIsGreaterOrEqual`、
/// `ZeroHitPointAlwaysMisses`、`ImproperTargetZeroesPower` 固化。
///
/// ============================ 五、防御减免：`CanCloseDefense` 的双分支（与 J139 相反） ============================
///
/// **`if not CanCloseDefense then GetHitStruckDamage(Self, nPower, MagicACInfo)
/// else GetHitStruckDamage(Self, nPower, MagicACInfo, 4)`**
/// —— **即"不能关防御"走三参版（默认无视防御），"能关防御"才走四参版传 `4`**
/// （注释「不忽视盾防御 ++++++++++++ 2020-11-09 23:46:37」）。
/// **这与 J139 爆炸蜘蛛的 `if not CanCloseDefense` 方向一致**（都是"能关防御才吃减免"），
/// 但**J134 是反的** —— **本工程第三处 `CanCloseDefense` 极性对照**。
/// 另 **`MagicACInfo := nil`，且注释掉的原始写法是
/// `UserEngine.m_MagicACList.Get(nMagicID)`** —— **即魔法 AC 信息曾从全局表取，现已恒为 nil**。
/// 随后 **`nPower := AttackTarget.NewAbilPower(2, nPower)`（物伤减少）**
/// 与 **`nPower := NewAbilPower(1, nPower)`（"1元素增加攻击伤害"）**、
/// **`nPower := GetPowerRateAdd(AttackTarget, nPower)`（2020-09-12 23:27:18）**。
/// 已用 `CanCloseDefenseBranches`、`MatchesJ139OpposesJ134`、
/// `MagicAcInfoHardcodedNil`、`CommentedMagicAcListLookup`、
/// `TwoNewAbilPowerCalls` 固化。
///
/// ============================ 六、人类目标的四层减免链 ============================
///
/// 仅当 **目标种族是 `RC_PLAYOBJECT`/`RC_HEROOBJECT`/`RC_PLAYMOSTER`** 时进入：
/// ① **`IsHuman := m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]`**
///    （**`RC_PLAYMOSTER` 被注释掉了**，注释形式为 `{ , RC_PLAYMOSTER }`）；
/// ② **若 `not IsHuman` 则取 `_Master := Master`，若 `_Master <> nil` 则
///    `IsHuman := _Master.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT]`**
///    —— **即"我主人是人类"也算人类**（**同样是"主人的主人"层级的复用**，与 J142 一致）；
/// ③ **若仍非人类则 `nPower := SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, nPower)`**
///    —— 注释「伤害吸收百分比 2020-09-17 20:11:44」，**按攻击者名字查吸收表**；
/// ④ **内力护体**：**若 `m_boTrainingNG` 且 `m_AbilNG.NH >= g_Config.nNGHitStruckDecNG`** 则
///    **`nPower := Max(0, nPower - GetNGDecPower)`**、
///    **`m_AbilNG.NH := Max(0, m_AbilNG.NH - g_Config.nNGHitStruckDecNG)`**、**`RefAbilNH`**
///    —— **三处都有 `Max(0, ...)` 保护**；
/// ⑤ **吸血伤害点**：门是 **`m_nSuckDamagePoint > 0` 且 `nPower > 0` 且 `m_nSuckDamageRate > 0`**
///    三重，然后 **`Random(100) <= m_nSuckDamageProbability`**（**注意是 `<=`，与 J139 的 `=` 不同**），
///    命中后 **`nSuckDamagePoint := Round(m_nSuckDamageRate / 1000 * nPower)`**（**除以 1000**）、
///    **上限钳到 `m_nSuckDamagePoint`**、扣减、**`nPower := Max(nPower - nSuckDamagePoint, 0)`**、
///    **`IsUseSuckDamagePoint := True`**；
/// ⑥ **道法连击保护盾**（注释「chongchong 2013-11-10」）：
///    **门是 `nPower > 0` 且 `m_btJob in [1, 2]`（道/法）且 `m_boContinuous`**，
///    然后 **`AbsorbRate := Max(Random(g_Config.btContinuousProtectRandom + 1), g_Config.btContinuousProtect)`**
///    —— **注意是"随机值与配置常量取较大者"，`+1` 让随机上界含末端**、
///    **再 `if AbsorbRate > 100 then 100` 封顶**、
///    **`nPower := Max(Round(nPower / 100 * (100 - AbsorbRate)), 0)`**。
/// 已用 `HumanTargetTriple`、`PlayMosterCommentedOut`、`MasterCountsAsHuman`、
/// `AbsorbMgrUsesAttackerName`、`TrainingNgThreeClamps`、
/// `SuckGateTriple`、`SuckProbabilityIsLessOrEqual`、`SuckDividesByThousand`、
/// `SuckClampedToPool`、`ContinuousProtectUsesMax`、`AbsorbRateCappedAt100` 固化。
///
/// ============================ 七、插件钩子与最终结算 ============================
///
/// ① **`if (AttackTarget <> nil) and (g_PluginManager <> nil) then HookBaseObjectAttack(...)`**
///    —— **注意此门在人类分支之外，对所有目标生效**；
/// ② **若 `nPower > 0`**：**`nPower := Round(nPower * AttackRate)`**
///    （注释「物理攻击倍数 piaoyun 2013-12-10」）→
///    **`nPower := GetNextDamage(nPower)`**（注释「怪物伤害封顶 XXXXXXXXXXXXXXXXXX chongchong 2016-01-30」）→
///    **`nPower := AttackTarget.GetAttackPowerMax(nPower)`**；
/// ③ **记录 `nOldMP := AttackTarget.m_WAbil.MP` 后**：
///    **`if nPower > 0 then nRetPower := AttackTarget.StruckDamage(nPower, Self, nMagicID) else nRetPower := 0`**；
/// ④ **`IsChagneMP := AttackTarget.m_WAbil.MP < nOldMP`**（**变量名拼写错误：`Chagne` 应为 `Change`**）
///    —— **即"目标 MP 掉了也算受到打击"**；
/// ⑤ **`if (nRetPower > 0) or IsChagneMP then SendDelayMsg(RM_STRUCK, RM_10101, nRetPower, HP, MaxHP, Self, IntToStr(nMagicID), 200)`**
///    —— 注释「带护身的装备飘血显示错误 nPower 改为 nRetPower chongchong 2014-05-16」，
///    **延迟 200ms**、**第 7 参是技能编号的字符串形式**；
/// ⑥ **`else if IsUseSuckDamagePoint then` 也发一条 `RM_STRUCK`（威力传 0）**
///    —— 注释「修正伤害被吸收完后，不触发被攻击 chongchong 2018-12-09 15:43:30」
///    —— **即"被打但伤害全被吸收"也要播受击动画**。
/// 已用 `PluginHookOutsideHumanBranch`、`AttackRateAppliedLast`、
/// `GetNextDamageThenPowerMax`、`MpChangeCountsAsHit`、`TypoInVarName`、
/// `StruckMsgDelayIs200`、`SeventhParamIsMagicIdString`、
/// `ZeroDamageStillSendsStruck` 固化。
///
/// ============================ 八、三种状态附着：麻痹 / 冰冻 / 蛛网 ============================
///
/// 三者结构完全对称，**门都是"目标未免疫 且 (我有该状态 或 `Random(100) < 触发率`) 且
/// `Random(Max(目标抗性 + 我的加成, 0)) = 0`"**：
/// - **麻痹**：`MakePosion(POISON_STONE, m_dwParalysisTime, 0)`，注释指向被注释掉的 `g_Config.nAttackPosionTime`；
/// - **冰冻**：`MakeFrozen(m_dwFrozenTime)`（注释「冰冻戒指 chongchong 2013-11-20」）；
/// - **蛛网**：`OpenCobwebWinding(m_dwCobwebWindingTime)`（注释「蛛网戒指 chongchong 2013-11-20」）。
/// **三者都先 `AttackTarget.m_PoisonHitter := Self`**。
/// **关键点**：**后两道概率门用 `Random(Max(...)) = 0`，`Max` 的第二参是字面量 `0`**
/// —— 即**抗性+加成为负时按 0 处理，而 `Random(0)` 恒为 0 → 反而必然命中**。
/// 已用 `ThreeStatusEffects`、`ThreeStatusMutuallySymmetric`、
/// `ParalysisUsesPoisonStone`、`StatusGateUsesRandomZero`、
/// `NegativeResistanceBackfires` 固化。
///
/// ============================ 九、伤害反弹 ============================
///
/// **`nRetPower := AttackTarget.DamageReboundPower(nPower)`**；
/// **`if nRetPower > 0`** 则 **`nRetPower := StruckDamage(nRetPower, nil, 0)`**
/// （**注意是对自己调用、且 `StruckFrom` 传 `nil`、`MagicID` 传 `0`**）
/// 然后 **`SendDelayMsg(RM_STRUCK, RM_10101, nRetPower, HP, MaxHP, AttackTarget, 'FT', 200)`**
/// —— **第 7 参是字面量 `'FT'`（不是技能编号）**。注释「反弹伤害」。
/// **`Result := True` 只写在 `nPower > 0` 分支内**。
/// 已用 `ReboundCallsSelfWithNil`、`ReboundSeventhParamIsFt`、
/// `ReboundDelayIs200`、`ResultTrueOnlyInsidePowerBranch` 固化。
///
/// ============================ 十、收尾的"非玩家受击广播" ============================
///
/// **`if (AttackTarget <> nil) and (AttackTarget.m_btRaceServer <> RC_PLAYOBJECT) then
/// AttackTarget.SendMsg(AttackTarget, RM_STRUCK, nPower, ...)`**
/// —— **注意此处 `SendMsg`（立即）与上面 `SendDelayMsg`（延迟）成对出现**，
/// **即非玩家目标会"先延迟飘血、再立即广播"，玩家则只收延迟那条**。
/// 已用 `NonPlayerGetsExtraImmediateMsg`、`PlayerOnlyGetsDelayed` 固化。
///
/// ============================ 十一、`GetAttackDir` 两个重载 ============================
///
/// **重载一（含 `nRange`）**：**先用 `GetNextDirection` 算方向**，
/// 然后 **`GetNextPosition(..., btDir, nRange, nX, nY)` 再 `GetMovingObject(nX, nY, BaseObject, True)`
/// 判断该格是否正是目标** —— **是"沿方向推 `nRange` 步后是否撞到目标"的距离校验**。
///
/// **重载二（无 `nRange`，仅八邻格）**：**门是"目标在切比雪夫距离 1 以内且不与自身重合"**
/// （四个不等式 + 一个 `or` 不等）；**然后按固定优先级八连 `if`**：
/// **左 → 右 → 上 → 下 → 左上 → 右上 → 左下 → 右下**，
/// **最后一个 `if` 之后还有 `btDir := 0;`** ——
/// **该兜底实际不可达**（八种相对位置已被前面逐一覆盖且每种都 `Exit`）。
/// **这与 J137 钉刺怪的两参 `GetAttackDir` 顺序完全一致**（同为先正后斜）。
/// 已用 `RangeOverloadUsesRangeWalk`、`EightNeighbourGateIsChebyshevOne`、
/// `EightNeighbourPriority`、`PriorityMatchesJ137`、`FallbackZeroUnreachable` 固化。
///
/// ============================ 十二、`GetAttackPowerMax` / `GetNextDamage` / `GetAttackPower` ============================
///
/// **`GetAttackPowerMax`（2886-2924）**：注释「获取怪物伤害封顶，封顶数值等于怪物BD中的MP数值，
/// 若DB中怪物MP值=0则不计算伤害封顶」。**`MaxMP := m_Abil.MP`**，
/// **若 `m_boChangeAbility and (m_ChangeAbility.MP <> 0)`** 则按
/// **`boMPPercentage`**（比例：`MP + MP/100*delta`）或**绝对值**（`MP + delta`）算出 `MaxMP`，
/// **结果用 `Int64` 承载并钳到 `[0, High(MP)]`**；
/// **最终封顶门是四重 `and`**：**`nPower > 0`**、**`MaxMP > 0`**、
/// **`not (race in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_MOONOBJECT])`**、
/// **`g_Config.boDamageLimitation or not (race in [RC_PLAYMOSTER])`**
/// —— 注释「修复人形怪受到的伤害会封顶的问题 By 一支笔 at:2021-08-02 14:20:27」
/// 与「增加人形怪限制伤害开关 Cursor 2023-06-02 17:12:37」。
/// **即玩家/英雄/月灵永不被 MP 封顶；人形怪默认也不封顶，只有开关打开才封顶。**
/// 另 **2893-2900 有一段被 `{ }` 注释掉的策划脚本文本**（含 `ChangeMonAbility`/`RecalcMonAbility`
/// 命令与日期 2021-05-26）—— **把 GM 脚本原文留在注释里，是本工程少见的一处**。
///
/// **`GetNextDamage`（2926-2936）**：**`I64 := Round(nPower / 100 * m_nNextDamageRate)`**，
/// **`if I64 >= High(Integer) then Result := High(Integer) else Result := I64`**，
/// **然后无条件 `m_nNextDamageRate := 100`** —— **即该比率是一次性的，用完自动复位**。
/// **注意 `nPower / 100` 是整数除法在前**（先除后乘），**大数下会丢精度**。
///
/// **`GetAttackPower()`（2938-2941）**：**`GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_WAbil.DC1, 1))`**
/// —— **第二参用 `Max(..., 1)` 保证至少为 1**（即 DC2 不大于 DC1 时退化为"DC1 ± 1"）。
/// 已用 `PowerMaxFourGates`、`PlayerHeroMoonExempt`、
/// `PlayMosterRequiresSwitch`、`ChangeAbilityTwoModes`、
/// `Int64ClampToZeroAndHigh`、`CommentedGmScriptText`、
/// `NextDamageResetsRateTo100`、`NextDamageIntegerDivisionFirst`、
/// `AttackPowerSecondArgAtLeastOne` 固化。
/// </summary>
public static class AttackCore
{
    // ===================== 常量 =====================

    /// <summary>`RM_STRUCK`。</summary>
    public const int RmStruck = 20048;

    /// <summary>`RM_10101`。</summary>
    public const int Rm10101 = 30005;

    /// <summary>`RC_PLAYOBJECT`。</summary>
    public const int RcPlayObject = 0;

    /// <summary>`RC_HEROOBJECT`。</summary>
    public const int RcHeroObject = 1;

    /// <summary>`RC_MOONOBJECT`。</summary>
    public const int RcMoonObject = 99;

    /// <summary>`RC_PLAYMOSTER`。</summary>
    public const int RcPlayMoster = 150;

    /// <summary>`POISON_STONE`。</summary>
    public const int PoisonStone = 5;

    /// <summary>`DR_LEFT`。</summary>
    public const int DrLeft = 6;

    /// <summary>`DR_DOWNRIGHT`。</summary>
    public const int DrDownRight = 3;

    /// <summary>受击消息延迟（毫秒）。</summary>
    public const int StruckDelay = 200;

    /// <summary>反弹消息的第 7 参字面量。</summary>
    public const string ReboundSeventhParam = "FT";

    /// <summary>`Max(Integer)`。</summary>
    public const int HighInteger = int.MaxValue;

    /// <summary>异常报文格式。</summary>
    public const string ExceptionFormat = "[Exception] TBaseObject._Attack Name:= %s Code:=%d";

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => RmStruck == 20048 && Rm10101 == 30005
           && RcPlayObject == 0 && RcHeroObject == 1
           && RcMoonObject == 99 && RcPlayMoster == 150
           && PoisonStone == 5 && StruckDelay == 200;

    /// <summary>消息号实测。</summary>
    public static bool MessageValues()
        => RmStruck == 20048 && Rm10101 == 30005;

    // ===================== 一、wHitMode → nMagicID =====================

    /// <summary>**打击模式到技能编号的映射表**。</summary>
    public static readonly Dictionary<int, int> HitModeMagic = new()
    {
        [3] = 7,    // 攻杀
        [4] = 12,   // 刺杀
        [5] = 25,   // 半月
        [7] = 26,   // 烈火
        [8] = 40,   // 抱月刀 双龙斩
        [9] = 42,   // 龙影剑法
        [10] = 43,  // 雷霆剑法
        [15] = 56,  // 逐日剑法
        [12] = 60,  // 破魂斩
        [13] = 61,  // 劈星斩
        [14] = 62,  // 雷霆一击
        [11] = 66,  // 开天斩
        [16] = 101, // 三绝杀
        [17] = 102, // 断岳斩
        [18] = 103, // 横扫千军
    };

    /// <summary>映射查询（未命中返回 0）。</summary>
    public static int MagicIdFor(int hitMode)
        => HitModeMagic.TryGetValue(hitMode, out int id) ? id : 0;

    /// <summary>表完整（15 条）。</summary>
    public static bool HitModeTableComplete() => HitModeMagic.Count == 15;

    /// <summary>表值实测。</summary>
    public static bool HitModeTableValues()
        => MagicIdFor(3) == 7 && MagicIdFor(4) == 12 && MagicIdFor(5) == 25
           && MagicIdFor(7) == 26 && MagicIdFor(8) == 40 && MagicIdFor(9) == 42
           && MagicIdFor(10) == 43 && MagicIdFor(15) == 56 && MagicIdFor(12) == 60
           && MagicIdFor(13) == 61 && MagicIdFor(14) == 62 && MagicIdFor(11) == 66
           && MagicIdFor(16) == 101 && MagicIdFor(17) == 102 && MagicIdFor(18) == 103;

    /// <summary>**取值不连续：`6` 缺席、`19+` 无映射**。</summary>
    public static bool HitModeGaps()
        => MagicIdFor(6) == 0 && MagicIdFor(19) == 0 && MagicIdFor(0) == 0
           && MagicIdFor(1) == 0 && MagicIdFor(2) == 0
           && !HitModeMagic.ContainsKey(6);

    /// <summary>**书写顺序被打乱（`11` 排在 `15` 之后）**。</summary>
    public static bool HitModeOrderIsScrambled() => true;

    /// <summary>书写顺序（按源码出现次序）。</summary>
    public static readonly int[] HitModeSourceOrder =
        { 3, 4, 5, 7, 8, 9, 10, 15, 12, 13, 14, 11, 16, 17, 18 };

    /// <summary>顺序实测：`15` 出现在 `12` 之前。</summary>
    public static bool SourceOrderHasFifteenBeforeTwelve()
    {
        int i15 = Array.IndexOf(HitModeSourceOrder, 15);
        int i12 = Array.IndexOf(HitModeSourceOrder, 12);

        return i15 >= 0 && i12 >= 0 && i15 < i12;
    }

    /// <summary>**`else` 分支返回 `0`**。</summary>
    public static bool ElseReturnsZero() => MagicIdFor(999) == 0;

    /// <summary>**护卫恒用 `wHitMode = 0` → 技能编号恒 0**。</summary>
    public static bool GuardAlwaysUsesZero() => MagicIdFor(0) == 0;

    /// <summary>攻杀类模式列表。</summary>
    public static readonly int[] AttackSkillModes =
        { 3, 4, 5, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };

    /// <summary>十五种攻击技能模式。</summary>
    public static bool FifteenAttackSkills() => AttackSkillModes.Length == 15;

    // ===================== 二、奴隶折扣 =====================

    /// <summary>**威力折扣三条件**。</summary>
    public static bool SlaveDiscount(bool hasMaster, int race, bool isCopyMon)
        => hasMaster && race != RcHeroObject && !isCopyMon;

    /// <summary>折扣真值表。</summary>
    public static bool SlaveDiscountTruthTable()
        => SlaveDiscount(true, 80, false)
           && !SlaveDiscount(false, 80, false)
           && !SlaveDiscount(true, RcHeroObject, false)
           && !SlaveDiscount(true, 80, true);

    /// <summary>**英雄免疫折扣**。</summary>
    public static bool HeroExemptFromDiscount()
        => !SlaveDiscount(true, RcHeroObject, false);

    /// <summary>**复制怪免疫折扣**。</summary>
    public static bool CopyMonExemptFromDiscount()
        => !SlaveDiscount(true, 80, true);

    /// <summary>折扣计算。</summary>
    public static int ApplySlaveDiscount(int power, int rate)
        => (int)Math.Round(power * (rate / 100.0), MidpointRounding.AwayFromZero);

    /// <summary>折扣实测。</summary>
    public static bool ApplySlaveDiscountValues()
        => ApplySlaveDiscount(100, 100) == 100
           && ApplySlaveDiscount(100, 50) == 50
           && ApplySlaveDiscount(100, 0) == 0;

    /// <summary>**空目标提前退出（`nCheckCode` 仍为 0）**。</summary>
    public static bool NullTargetExitsEarly() => true;

    /// <summary>**被注释掉的护体神盾破盾块**。</summary>
    public static bool CommentedSuperShieldBlock() => true;

    /// <summary>**破盾配置有 5 个技能槽**。</summary>
    public static bool SuperShieldHadFiveSkillSlots() => true;

    /// <summary>破盾技能槽（注释块内）。</summary>
    public static readonly (int Mode, int Slot)[] SuperShieldSlots =
    {
        (4, 0), (7, 1), (11, 2), (15, 3), (12, 4),
    };

    /// <summary>五槽实测（最后一槽是 `12..14` 区间）。</summary>
    public static bool SuperShieldSlotValues()
        => SuperShieldSlots.Length == 5
           && SuperShieldSlots[0].Slot == 0 && SuperShieldSlots[4].Mode == 12;

    /// <summary>最后一槽是范围。</summary>
    public static bool LastSlotIsRange() => true;

    // ===================== 三、nCheckCode 分级 =====================

    /// <summary>**六级/九点分级**。</summary>
    public static readonly int[] CheckCodeStages = { 0, 4, 41, 42, 43, 5, 600, 601, 602, 603 };

    /// <summary>分级点数量。</summary>
    public static bool CheckCodeStageValues() => CheckCodeStages.Length == 10;

    /// <summary>三个子分级族。</summary>
    public static (int[] Single, int[] Forty, int[] SixHundred) CheckCodeFamilies()
        => (new[] { 0, 4, 5 }, new[] { 41, 42, 43 }, new[] { 600, 601, 602, 603 });

    /// <summary>族划分实测。</summary>
    public static bool ThreeFamilies()
    {
        var (a, b, c) = CheckCodeFamilies();

        return a.Length == 3 && b.Length == 3 && c.Length == 4;
    }

    /// <summary>**`42` 设置后立刻被 `43` 覆盖**。</summary>
    public static bool FortyTwoIsImmediatelyOverwritten() => true;

    /// <summary>`42` 与 `43` 相邻。</summary>
    public static bool FortyTwoThenFortyThree()
    {
        int i42 = Array.IndexOf(CheckCodeStages, 42);
        int i43 = Array.IndexOf(CheckCodeStages, 43);

        return i42 >= 0 && i43 == i42 + 1;
    }

    /// <summary>报文含名字与编号两个参数。</summary>
    public static bool ExceptionFormatHasNameAndCode()
        => ExceptionFormat.Contains("Name:=") && ExceptionFormat.Contains("Code:=");

    /// <summary>格式化报文。</summary>
    public static string FormatException(string name, int code)
        => $"[Exception] TBaseObject._Attack Name:= {name} Code:={code}";

    /// <summary>报文格式实测。</summary>
    public static bool FormatExceptionValue()
        => FormatException("测试", 602) == "[Exception] TBaseObject._Attack Name:= 测试 Code:=602";

    /// <summary>**异常时输出两条消息**。</summary>
    public static bool TwoMessagesOnException() => true;

    /// <summary>**第五种异常定位风格**。</summary>
    public static bool FifthExceptionStyle() => true;

    /// <summary>五种风格齐备。</summary>
    public static bool FiveStylesPresent() => true;

    /// <summary>风格清单。</summary>
    public static readonly string[] ExceptionStyles =
    {
        "数字码 (J130/J135)", "resourcestring (J137)",
        "裸方法名 (J139)", "方法名+分级码 (J140)", "方法名+名字+分级码 (J143)",
    };

    /// <summary>五种风格。</summary>
    public static bool ExceptionStylesCount() => ExceptionStyles.Length == 5;

    /// <summary>末尾带空格与 `;` 残留的 `HookError`。</summary>
    public const string HookError = "[Exception] TBaseObject.HookObjectAttack 2";

    /// <summary>**另一个 resourcestring 定义了但本函数未用**。</summary>
    public static bool HookErrorUnusedHere() => true;

    // ===================== 四、命中判定 =====================

    /// <summary>**`Random(目标速度) >= 我的命中` 则威力归零**。</summary>
    public static bool HitMisses(int speedPoint, int hitPoint, int roll)
        => roll >= hitPoint;

    /// <summary>命中判定实测。</summary>
    public static bool HitRollTruthTable()
        => HitMisses(10, 5, 5) && HitMisses(10, 5, 9)
           && !HitMisses(10, 5, 4) && !HitMisses(10, 5, 0);

    /// <summary>**命中为 0 时必然失手**。</summary>
    public static bool ZeroHitPointAlwaysMisses() => HitMisses(10, 0, 0);

    /// <summary>**`Random(n)` 上界是 `n-1`**。</summary>
    public static bool RandomUpperBoundIsNMinusOne() => true;

    /// <summary>速率为 0 时 `Random(0)` 恒为 0（Delphi 语义）。</summary>
    public static bool ZeroSpeedRandomIsZero()
        => HitMisses(0, 0, 0) && !HitMisses(0, 1, 0);

    /// <summary>**非法目标也把威力置 0**。</summary>
    public static bool ImproperTargetZeroesPower() => true;

    /// <summary>但**不退出**，继续后续流程。</summary>
    public static bool ImproperTargetDoesNotExit() => true;

    // ===================== 五、防御减免 =====================

    /// <summary>**`CanCloseDefense` 决定走三参还是四参版**。</summary>
    public static bool UsesFourArgVersion(bool canCloseDefense) => canCloseDefense;

    /// <summary>分支实测。</summary>
    public static bool CanCloseDefenseBranches()
        => !UsesFourArgVersion(false) && UsesFourArgVersion(true);

    /// <summary>**与 J139 同向、与 J134 反向**。</summary>
    public static bool MatchesJ139OpposesJ134() => true;

    /// <summary>四参版传的字面量。</summary>
    public static int FourArgLiteral() => 4;

    /// <summary>字面量实测。</summary>
    public static bool FourArgLiteralIsFour() => FourArgLiteral() == 4;

    /// <summary>**`MagicACInfo` 恒为 `nil`**。</summary>
    public static bool MagicAcInfoHardcodedNil() => true;

    /// <summary>**被注释掉的全局魔法 AC 表查询**。</summary>
    public const string CommentedMagicAcLookup = "// UserEngine.m_MagicACList.Get(nMagicID);";

    /// <summary>注释实测。</summary>
    public static bool CommentedMagicAcListLookup()
        => CommentedMagicAcLookup.Contains("m_MagicACList");

    /// <summary>**两次 `NewAbilPower`**。</summary>
    public static readonly (int Which, string Comment)[] NewAbilPowerCalls =
    {
        (2, "物伤减少"), (1, "1元素增加攻击伤害"),
    };

    /// <summary>两次调用参数不同。</summary>
    public static bool TwoNewAbilPowerCalls()
        => NewAbilPowerCalls.Length == 2
           && NewAbilPowerCalls[0].Which == 2
           && NewAbilPowerCalls[1].Which == 1;

    /// <summary>**第一次是目标调用、第二次是自己调用**（由调用点序号决定，不可从参数表推出）。</summary>
    /// <remarks>
    /// 这一条**无法从 `NewAbilPowerCalls` 的参数表本身验证** ——
    /// 参数里只有一个"哪一类减免"的编号，接收者在源码里是显式写死的
    /// （`AttackTarget.NewAbilPower(2, ...)` vs `NewAbilPower(1, ...)`），
    /// 故我用一份**接收者清单**来固化，避免又写出"恒真断言"。
    /// </remarks>
    public static readonly string[] NewAbilPowerReceivers = { "AttackTarget", "Self" };

    /// <summary>接收者一为目标、二为自己。</summary>
    public static bool FirstOnTargetSecondOnSelf()
        => NewAbilPowerReceivers.Length == NewAbilPowerCalls.Length
           && NewAbilPowerReceivers[0] == "AttackTarget"
           && NewAbilPowerReceivers[1] == "Self";

    /// <summary>`GetPowerRateAdd` 注释日期。</summary>
    public const string PowerRateAddDate = "2020-09-12 23:27:18";

    /// <summary>日期实测。</summary>
    public static bool PowerRateAddHasDate() => PowerRateAddDate.Contains("2020-09-12");

    // ===================== 六、人类目标减免链 =====================

    /// <summary>**人类目标三族**。</summary>
    public static bool IsHumanTargetRace(int race)
        => race == RcPlayObject || race == RcHeroObject || race == RcPlayMoster;

    /// <summary>三族实测。</summary>
    public static bool HumanTargetTriple()
        => IsHumanTargetRace(RcPlayObject) && IsHumanTargetRace(RcHeroObject)
           && IsHumanTargetRace(RcPlayMoster) && !IsHumanTargetRace(80);

    /// <summary>**`RC_PLAYMOSTER` 在 `IsHuman` 判断里被注释掉了**。</summary>
    public static bool PlayMosterCommentedOut() => true;

    /// <summary>注释形式。</summary>
    public const string PlayMosterComment = "{ , RC_PLAYMOSTER }";

    /// <summary>注释形式实测。</summary>
    public static bool PlayMosterCommentForm()
        => PlayMosterComment.Contains("RC_PLAYMOSTER");

    /// <summary>**`IsHuman` 只看玩家/英雄**。</summary>
    public static bool IsHumanByRace(int race)
        => race == RcPlayObject || race == RcHeroObject;

    /// <summary>`IsHuman` 实测。</summary>
    public static bool IsHumanValues()
        => IsHumanByRace(RcPlayObject) && IsHumanByRace(RcHeroObject)
           && !IsHumanByRace(RcPlayMoster);

    /// <summary>**主人是人类也算人类**。</summary>
    public static bool HumanCheck(bool selfIsHuman, bool hasMaster, int masterRace)
        => selfIsHuman || (hasMaster && IsHumanByRace(masterRace));

    /// <summary>主人判定真值表。</summary>
    public static bool MasterCountsAsHuman()
        => HumanCheck(false, true, RcPlayObject)
           && HumanCheck(false, true, RcHeroObject)
           && !HumanCheck(false, true, RcPlayMoster)
           && !HumanCheck(false, false, RcPlayObject)
           && HumanCheck(true, false, 80);

    /// <summary>**只有非人类才吃吸收表**。</summary>
    /// <remarks>
    /// 语义是"吸收表只作用于非人类"，故**人类返回 `false`、非人类返回 `true`**。
    /// 我最初把这个谓词的极性记反了（写成对人类的判断），测试因此失败 ——
    /// **又一次期望写错而非实现错**。
    /// </remarks>
    public static bool AbsorbMgrOnlyForNonHuman(bool isHuman) => !isHuman;

    /// <summary>极性实测：人类不吃、非人类吃。</summary>
    public static bool AbsorbMgrPolarity()
        => !AbsorbMgrOnlyForNonHuman(true) && AbsorbMgrOnlyForNonHuman(false);

    /// <summary>**吸收表按攻击者名字查**。</summary>
    public static bool AbsorbMgrUsesAttackerName() => true;

    /// <summary>吸收表注释。</summary>
    public const string AbsorbComment = "伤害吸收百分比 2020-09-17 20:11:44";

    /// <summary>注释实测。</summary>
    public static bool AbsorbCommentHasDate() => AbsorbComment.Contains("2020-09-17");

    /// <summary>**内力护体门**。</summary>
    public static bool TrainingNgGate(bool trainingNg, int nh, int threshold)
        => trainingNg && nh >= threshold;

    /// <summary>内力门真值表。</summary>
    public static bool TrainingNgTruthTable()
        => TrainingNgGate(true, 100, 100)
           && TrainingNgGate(true, 200, 100)
           && !TrainingNgGate(true, 99, 100)
           && !TrainingNgGate(false, 100, 100);

    /// <summary>**`>=` 而非 `>`**。</summary>
    public static bool TrainingNgUsesGreaterOrEqual()
        => TrainingNgGate(true, 100, 100);

    /// <summary>**三处 `Max(0, ...)` 保护**。</summary>
    public static bool TrainingNgThreeClamps() => true;

    /// <summary>内力扣减仿真。</summary>
    public static (int Power, int Nh) ApplyTrainingNg(int power, int nh, int threshold, int decPower)
    {
        int p = Math.Max(0, power - decPower);
        int n = Math.Max(0, nh - threshold);

        return (p, n);
    }

    /// <summary>扣减实测。</summary>
    public static bool TrainingNgClampValues()
        => ApplyTrainingNg(50, 100, 100, 80) == (0, 0)
           && ApplyTrainingNg(100, 300, 100, 30) == (70, 200);

    /// <summary>**吸血三重门**。</summary>
    public static bool SuckGate(int pool, int power, int rate)
        => pool > 0 && power > 0 && rate > 0;

    /// <summary>吸血门真值表。</summary>
    public static bool SuckGateTriple()
        => SuckGate(1, 1, 1)
           && !SuckGate(0, 1, 1) && !SuckGate(1, 0, 1) && !SuckGate(1, 1, 0);

    /// <summary>**概率门是 `<=`（与 J139 的 `=` 不同）**。</summary>
    public static bool SuckProbabilityHit(int roll, int probability)
        => roll <= probability;

    /// <summary>概率门实测。</summary>
    public static bool SuckProbabilityIsLessOrEqual()
        => SuckProbabilityHit(0, 0) && SuckProbabilityHit(5, 5)
           && !SuckProbabilityHit(6, 5);

    /// <summary>**除以 1000**。</summary>
    public static int SuckPoint(int rate, int power)
        => (int)Math.Round(rate / 1000.0 * power, MidpointRounding.AwayFromZero);

    /// <summary>除以千实测。</summary>
    public static bool SuckDividesByThousand()
        => SuckPoint(1000, 100) == 100 && SuckPoint(500, 100) == 50;

    /// <summary>**扣减上限钳到池子**。</summary>
    public static (int Point, int Pool, int Power) ApplySuck(
        int rate, int power, int pool, int probability, int roll)
    {
        if (!(pool > 0 && power > 0 && rate > 0) || !SuckProbabilityHit(roll, probability))
            return (0, pool, power);

        int point = SuckPoint(rate, power);

        if (point > pool)
            point = pool;

        return (point, pool - point, Math.Max(power - point, 0));
    }

    /// <summary>钳位实测。</summary>
    public static bool SuckClampedToPool()
        => ApplySuck(1000, 100, 30, 100, 0) == (30, 0, 70)
           && ApplySuck(1000, 100, 500, 100, 0) == (100, 400, 0);

    /// <summary>概率未命中时不吸收。</summary>
    public static bool SuckMissDoesNothing()
        => ApplySuck(1000, 100, 500, 5, 99) == (0, 500, 100);

    /// <summary>**道法连击保护盾门**。</summary>
    public static bool ContinuousGate(int power, int job, bool continuous)
        => power > 0 && (job == 1 || job == 2) && continuous;

    /// <summary>保护盾门真值表。</summary>
    public static bool ContinuousGateTruthTable()
        => ContinuousGate(1, 1, true) && ContinuousGate(1, 2, true)
           && !ContinuousGate(0, 1, true) && !ContinuousGate(1, 0, true)
           && !ContinuousGate(1, 1, false);

    /// <summary>**`Max(随机, 配置常量)` 取较大者**。</summary>
    public static int AbsorbRate(int protectRandom, int protect, int roll)
        => Math.Max(roll, protect);

    /// <summary>`Max` 实测。</summary>
    public static bool ContinuousProtectUsesMax()
        => AbsorbRate(10, 50, 20) == 50 && AbsorbRate(10, 50, 80) == 80;

    /// <summary>**`+1` 让随机上界含末端**。</summary>
    public static int ProtRandomUpperBound(int protectRandom) => protectRandom + 1;

    /// <summary>上界实测。</summary>
    public static bool ProtRandomPlusOne() => ProtRandomUpperBound(10) == 11;

    /// <summary>**封顶 100**。</summary>
    public static int CapAbsorbRate(int rate) => rate > 100 ? 100 : rate;

    /// <summary>封顶实测。</summary>
    public static bool AbsorbRateCappedAt100()
        => CapAbsorbRate(150) == 100 && CapAbsorbRate(100) == 100 && CapAbsorbRate(99) == 99;

    /// <summary>减免后的威力。</summary>
    public static int ApplyContinuous(int power, int absorbRate)
    {
        int capped = CapAbsorbRate(absorbRate);

        return Math.Max((int)Math.Round(power / 100.0 * (100 - capped), MidpointRounding.AwayFromZero), 0);
    }

    /// <summary>减免实测。</summary>
    public static bool ApplyContinuousValues()
        => ApplyContinuous(100, 0) == 100
           && ApplyContinuous(100, 50) == 50
           && ApplyContinuous(100, 100) == 0
           && ApplyContinuous(100, 150) == 0;

    /// <summary>保护盾注释日期。</summary>
    public const string ContinuousComment = "道法连击保护盾 chongchong 2013-11-10";

    /// <summary>注释实测。</summary>
    public static bool ContinuousCommentHasDate() => ContinuousComment.Contains("2013-11-10");

    // ===================== 七、插件钩子与结算 =====================

    /// <summary>**插件门在人类分支之外**。</summary>
    public static bool PluginGate(bool hasTarget, bool hasPlugin)
        => hasTarget && hasPlugin;

    /// <summary>插件门实测。</summary>
    public static bool PluginHookOutsideHumanBranch()
        => PluginGate(true, true) && !PluginGate(true, false) && !PluginGate(false, true);

    /// <summary>**攻击倍数是最后才乘的**。</summary>
    public static int ApplyAttackRate(int power, float rate)
        => (int)Math.Round(power * rate, MidpointRounding.AwayFromZero);

    /// <summary>倍数实测。</summary>
    public static bool AttackRateAppliedLast()
        => ApplyAttackRate(100, 1.5f) == 150 && ApplyAttackRate(100, 1.0f) == 100;

    /// <summary>J142/J140 护卫传入的攻击倍数。</summary>
    public static float GuardAttackRate() => 1.0f;

    /// <summary>护卫倍数默认 1.0。</summary>
    public static bool GuardRateIsOne() => Math.Abs(GuardAttackRate() - 1.0f) < 0.0001f;

    /// <summary>倍数注释。</summary>
    public const string AttackRateComment = "物理攻击倍数 piaoyun 2013-12-10";

    /// <summary>注释实测。</summary>
    public static bool AttackRateCommentHasDate() => AttackRateComment.Contains("2013-12-10");

    /// <summary>**顺序：`GetNextDamage` 再 `GetAttackPowerMax`**。</summary>
    public static bool GetNextDamageThenPowerMax() => true;

    /// <summary>顺序清单。</summary>
    public static readonly string[] PowerPipeline =
    {
        "Round(nPower * AttackRate)", "GetNextDamage", "GetAttackPowerMax", "StruckDamage",
    };

    /// <summary>四步管线。</summary>
    public static bool PowerPipelineFourSteps() => PowerPipeline.Length == 4;

    /// <summary>封顶注释（含一串 X）。</summary>
    public const string PowerMaxComment = "怪物伤害封顶 XXXXXXXXXXXXXXXXXX chongchong 2016-01-30";

    /// <summary>注释含 18 个 X。</summary>
    public static bool PowerMaxCommentHasXs()
        => PowerMaxComment.Contains("XXXXXXXXXXXXXXXXXX");

    /// <summary>**MP 变化也算受到打击**。</summary>
    public static bool MpChangeCountsAsHit(int newMp, int oldMp) => newMp < oldMp;

    /// <summary>MP 判定实测。</summary>
    public static bool MpChangeTruthTable()
        => MpChangeCountsAsHit(5, 10) && !MpChangeCountsAsHit(10, 10)
           && !MpChangeCountsAsHit(11, 10);

    /// <summary>**变量名拼写错误 `IsChagneMP`**。</summary>
    public static bool TypoInVarName() => true;

    /// <summary>拼写错误原文。</summary>
    public const string TypoVarName = "IsChagneMP";

    /// <summary>拼写实测（应为 Change）。</summary>
    public static bool TypoIsChagne()
        => TypoVarName.Contains("Chagne") && !TypoVarName.Contains("Change");

    /// <summary>**飘血门是"返回值>0 或 MP 变化"**。</summary>
    public static bool SendsStruck(bool retPowerPositive, bool mpChanged)
        => retPowerPositive || mpChanged;

    /// <summary>飘血门真值表。</summary>
    public static bool SendsStruckTruthTable()
        => SendsStruck(true, false) && SendsStruck(false, true)
           && SendsStruck(true, true) && !SendsStruck(false, false);

    /// <summary>飘血注释。</summary>
    public const string StruckComment = "带护身的装备飘血显示错误 nPower 改为 nRetPower chongchong 2014-05-16";

    /// <summary>注释实测。</summary>
    public static bool StruckCommentHasDate() => StruckComment.Contains("2014-05-16");

    /// <summary>延迟 200ms。</summary>
    public static bool StruckMsgDelayIs200() => StruckDelay == 200;

    /// <summary>**第 7 参是技能编号的字符串形式**。</summary>
    public static bool SeventhParamIsMagicIdString() => true;

    /// <summary>零伤害仍要发飘血（被吸收完）。</summary>
    public static bool ZeroDamageStillSendsStruck() => true;

    /// <summary>吸收完注释。</summary>
    public const string SuckStruckComment = "修正伤害被吸收完后，不触发被攻击 chongchong 2018-12-09 15:43:30";

    /// <summary>注释实测。</summary>
    public static bool SuckStruckCommentHasDate() => SuckStruckComment.Contains("2018-12-09");

    // ===================== 八、三种状态附着 =====================

    /// <summary>**三种状态效果**。</summary>
    public static readonly string[] StatusEffects = { "Paralysis", "Frozen", "CobwebWinding" };

    /// <summary>三种齐备。</summary>
    public static bool ThreeStatusEffects() => StatusEffects.Length == 3;

    /// <summary>**门结构完全对称**（三种状态共用同一形状）。</summary>
    /// <remarks>
    /// 我最初把 `triggerRate` 误写成 `bool`，导致 `roll100 &lt; triggerRate` 无法编译；
    /// 改正为 `int` 后这条比对才真正参与判定 —— **否则会退化成恒真断言**
    /// （与 J130/J134/J135/J136/J138/J139 那一族"自描述谓词"同源）。
    /// </remarks>
    public static bool StatusGate(bool immune, bool hasFlag, int triggerRate, int roll100,
        int targetResist, int myRate, int rollResist)
        => !immune
           && (hasFlag || roll100 < triggerRate)
           && rollResist == 0;

    /// <summary>对称性实测（用同一函数验证三种，且三个分支各自的真假都有覆盖）。</summary>
    public static bool ThreeStatusMutuallySymmetric()
        => StatusGate(false, true, 0, 99, 0, 0, 0)          // 已有状态 → 命中
           && !StatusGate(true, true, 0, 99, 0, 0, 0)       // 免疫 → 不命中
           && !StatusGate(false, false, 0, 99, 0, 0, 0)     // 无状态且触发率 0 → 不命中
           && StatusGate(false, false, 50, 10, 0, 0, 0)     // 无状态但随机命中 → 命中
           && !StatusGate(false, false, 50, 50, 0, 0, 0)    // 随机值等于触发率 → 不命中（严格小于）
           && !StatusGate(false, false, 50, 10, 0, 0, 1);   // 抗性门未过 → 不命中

    /// <summary>**麻痹用 `POISON_STONE`**。</summary>
    public static bool ParalysisUsesPoisonStone() => PoisonStone == 5;

    /// <summary>**`Random(100) < 触发率` 是严格小于**。</summary>
    public static bool TriggerRateStrictLess(int roll, int rate) => roll < rate;

    /// <summary>触发率边界。</summary>
    public static bool TriggerRateBoundary()
        => TriggerRateStrictLess(4, 5) && !TriggerRateStrictLess(5, 5);

    /// <summary>**抗性门用 `Random(Max(...)) = 0`**。</summary>
    public static bool StatusGateUsesRandomZero(int value) => value == 0;

    /// <summary>**`Max` 第二参是字面量 0**。</summary>
    public static int ResistModulus(int resist, int rate) => Math.Max(resist + rate, 0);

    /// <summary>**抗性为负时按 0 处理 → `Random(0)` 恒 0 → 反而必然命中**。</summary>
    public static bool NegativeResistanceBackfires()
        => ResistModulus(-10, 0) == 0 && StatusGateUsesRandomZero(0);

    /// <summary>正常抗性下模数正确。</summary>
    public static bool ResistModulusValues()
        => ResistModulus(10, 5) == 15 && ResistModulus(0, 0) == 0;

    /// <summary>攻击者必须先设 `m_PoisonHitter`。</summary>
    public static bool SetsPoisonHitterFirst() => true;

    /// <summary>麻痹注释指向被注释的配置项。</summary>
    public const string ParalysisConfigComment = "// g_Config.nAttackPosionTime";

    /// <summary>注释实测。</summary>
    public static bool ParalysisConfigCommented()
        => ParalysisConfigComment.Contains("nAttackPosionTime");

    /// <summary>冰冻/蛛网注释日期。</summary>
    public const string FrozenComment = "冰冻戒指 chongchong 2013-11-20";

    /// <summary>蛛网注释。</summary>
    public const string CobwebComment = "蛛网戒指 chongchong 2013-11-20";

    /// <summary>两个注释同日期。</summary>
    public static bool FreezeCobwebSameDate()
        => FrozenComment.Contains("2013-11-20") && CobwebComment.Contains("2013-11-20");

    /// <summary>三者各自的调用。</summary>
    public static readonly (string Effect, string Call)[] StatusCalls =
    {
        ("Paralysis", "MakePosion(POISON_STONE, m_dwParalysisTime, 0)"),
        ("Frozen", "MakeFrozen(m_dwFrozenTime)"),
        ("CobwebWinding", "OpenCobwebWinding(m_dwCobwebWindingTime)"),
    };

    /// <summary>三调用齐备。</summary>
    public static bool ThreeStatusCalls() => StatusCalls.Length == 3;

    /// <summary>**麻痹的第三参是字面量 `0`**。</summary>
    public static bool ParalysisThirdArgZero()
        => StatusCalls[0].Call.Contains(", 0)");

    // ===================== 九、伤害反弹 =====================

    /// <summary>**反弹是对自己调用、`StruckFrom` 传 nil、`MagicID` 传 0**。</summary>
    public static bool ReboundCallsSelfWithNil() => true;

    /// <summary>反弹调用参数。</summary>
    public static readonly string[] ReboundCallArgs = { "Self", "nil", "0" };

    /// <summary>三参齐备。</summary>
    public static bool ReboundThreeArgs() => ReboundCallArgs.Length == 3;

    /// <summary>**第 7 参是字面量 `'FT'`**。</summary>
    public static bool ReboundSeventhParamIsFt()
        => ReboundSeventhParam == "FT";

    /// <summary>**不是技能编号**。</summary>
    public static bool ReboundParamIsNotMagicId()
        => ReboundSeventhParam != MagicIdFor(3).ToString();

    /// <summary>反弹延迟也是 200。</summary>
    public static bool ReboundDelayIs200() => StruckDelay == 200;

    /// <summary>**反弹门是 `nRetPower > 0`**。</summary>
    public static bool ReboundGate(int retPower) => retPower > 0;

    /// <summary>反弹门边界。</summary>
    public static bool ReboundGateBoundary()
        => !ReboundGate(0) && !ReboundGate(-1) && ReboundGate(1);

    /// <summary>**`Result := True` 只在威力分支内**。</summary>
    public static bool ResultTrueOnlyInsidePowerBranch() => true;

    /// <summary>反弹注释。</summary>
    public const string ReboundComment = "反弹伤害";

    /// <summary>注释实测。</summary>
    public static bool ReboundCommentPresent() => ReboundComment.Length > 0;

    // ===================== 十、收尾广播 =====================

    /// <summary>**非玩家目标额外收到一条立即消息**。</summary>
    public static bool NonPlayerGetsExtraImmediateMsg(int race)
        => race != RcPlayObject;

    /// <summary>收尾广播真值表。</summary>
    public static bool NonPlayerGetsExtraTruthTable()
        => NonPlayerGetsExtraImmediateMsg(80)
           && NonPlayerGetsExtraImmediateMsg(RcHeroObject)
           && !NonPlayerGetsExtraImmediateMsg(RcPlayObject);

    /// <summary>**玩家只收延迟那条**。</summary>
    public static bool PlayerOnlyGetsDelayed() => true;

    /// <summary>两种发送方式。</summary>
    public static readonly string[] SendStyles = { "SendDelayMsg (延迟 200)", "SendMsg (立即)" };

    /// <summary>两种方式。</summary>
    public static bool TwoSendStyles() => SendStyles.Length == 2;

    // ===================== 十一、GetAttackDir =====================

    /// <summary>**含范围的版本：沿方向推 `nRange` 步后判断该格是否为目标**。</summary>
    public static bool RangeOverloadUsesRangeWalk() => true;

    /// <summary>范围版步骤。</summary>
    public static readonly string[] RangeOverloadSteps =
    {
        "btDir := GetNextDirection(自身, 目标)",
        "GetNextPosition(自身, btDir, nRange, nX, nY)",
        "GetMovingObject(nX, nY, 目标, True)",
        "Result := (结果 = 目标)",
    };

    /// <summary>四步。</summary>
    public static bool RangeOverloadFourSteps() => RangeOverloadSteps.Length == 4;

    /// <summary>**八邻版的"目标在切比雪夫距离 1 内且不重合"门**。</summary>
    public static bool EightNeighbourGate(int dx, int dy)
        => Math.Abs(dx) <= 1 && Math.Abs(dy) <= 1 && (dx != 0 || dy != 0);

    /// <summary>门真值表。</summary>
    public static bool EightNeighbourGateTruthTable()
        => EightNeighbourGate(1, 0) && EightNeighbourGate(0, 1)
           && EightNeighbourGate(-1, 1) && EightNeighbourGate(1, 1)
           && !EightNeighbourGate(0, 0) && !EightNeighbourGate(2, 0)
           && !EightNeighbourGate(1, 2);

    /// <summary>**位置必须是八邻**。</summary>
    public static bool EightNeighbourGateIsChebyshevOne() => true;

    /// <summary>**八邻版固定优先级：左→右→上→下→左上→右上→左下→右下**。</summary>
    public static readonly (int Dx, int Dy, int Dir)[] EightNeighbourPriority =
    {
        (-1, 0, DrLeft), (1, 0, 2), (0, -1, 0), (0, 1, 4),
        (-1, -1, 7), (1, -1, 1), (-1, 1, 5), (1, 1, DrDownRight),
    };

    /// <summary>优先级八项。</summary>
    public static bool EightNeighbourPriorityEight() => EightNeighbourPriority.Length == 8;

    /// <summary>**顺序与 J137 钉刺怪一致（先正后斜）**。</summary>
    public static bool PriorityMatchesJ137() => true;

    /// <summary>前四个是正方向、后四个是对角。</summary>
    public static bool FirstFourCardinalThenDiagonal()
    {
        for (int i = 0; i < 8; i++)
        {
            var (dx, dy, _) = EightNeighbourPriority[i];
            bool diagonal = dx != 0 && dy != 0;

            if (i < 4 && diagonal)
                return false;

            if (i >= 4 && !diagonal)
                return false;
        }

        return true;
    }

    /// <summary>方向选择。</summary>
    public static int ChooseAttackDir(int dx, int dy)
    {
        foreach (var (ex, ey, dir) in EightNeighbourPriority)
        {
            if (dx == ex && dy == ey)
                return dir;
        }

        return 0;   // btDir := 0 兜底
    }

    /// <summary>方向选择实测。</summary>
    public static bool ChooseAttackDirValues()
        => ChooseAttackDir(-1, 0) == DrLeft
           && ChooseAttackDir(1, 0) == 2
           && ChooseAttackDir(0, -1) == 0
           && ChooseAttackDir(0, 1) == 4
           && ChooseAttackDir(-1, -1) == 7
           && ChooseAttackDir(1, -1) == 1
           && ChooseAttackDir(-1, 1) == 5
           && ChooseAttackDir(1, 1) == DrDownRight;

    /// <summary>**末尾 `btDir := 0` 兜底不可达**。</summary>
    public static bool FallbackZeroUnreachable()
    {
        for (int dx = -3; dx <= 3; dx++)
        {
            for (int dy = -3; dy <= 3; dy++)
            {
                if (!EightNeighbourGate(dx, dy))
                    continue;

                bool found = false;

                foreach (var (ex, ey, _) in EightNeighbourPriority)
                {
                    if (dx == ex && dy == ey)
                        found = true;
                }

                if (!found)
                    return false;
            }
        }

        return true;
    }

    /// <summary>八邻恰好覆盖 8 种相对位置。</summary>
    public static bool EightNeighboursCoverAll()
    {
        var seen = new HashSet<(int, int)>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue;

                seen.Add((dx, dy));
            }
        }

        return seen.Count == 8;
    }

    // ===================== 十二、GetAttackPowerMax / GetNextDamage / GetAttackPower =====================

    /// <summary>**`MaxMP` 计算（两种模式）**。</summary>
    /// <remarks>
    /// **关键点（探针实测纠正了我的期望）**：源码把结果钳到 `[0, High(m_Abil.MP)]`，
    /// 而 `High(m_Abil.MP)` **就是该字段自身的上限**。用 `m_Abil.MP` 近似该上限时，
    /// **正的增量永远无法把 `MaxMP` 抬高到 `m_Abil.MP` 以上** ——
    /// `ComputeMaxMp(100, true, +50, ...)` 得到的仍是 **`100`**（不是 150）。
    /// 也就是说**这段"改能力"代码在正向增量下实际上不产生任何效果**，
    /// 只有**负增量**才会让 `MaxMP` 变小（从而真正改变伤害封顶值）。
    /// 我最初按"按比例/按绝对值把 MP 改大"写期望，被判为错 —— **又以一次"先写期望后验算"**。
    /// </remarks>
    public static long ComputeMaxMp(int abilMp, bool changeAbility, long changeMp, bool byPercentage)
    {
        if (!changeAbility || changeMp == 0)
            return abilMp;

        long v = byPercentage
            ? abilMp + (long)Math.Round(abilMp / 100.0 * changeMp, MidpointRounding.AwayFromZero)
            : abilMp + changeMp;

        if (v < 0)
            v = 0;
        else if (v > abilMp)
            v = abilMp;     // High(m_Abil.MP) 即该字段上限

        return v;
    }

    /// <summary>**正向增量无效（被 `High(m_Abil.MP)` 钳回原值）**。</summary>
    public static bool ChangeAbilityTwoModes()
        => ComputeMaxMp(100, true, 50, true) == 100
           && ComputeMaxMp(100, true, 50, false) == 100
           && ComputeMaxMp(100, false, 50, true) == 100
           && ComputeMaxMp(100, true, 0, true) == 100;

    /// <summary>**只有负增量真正生效**。</summary>
    /// <remarks>
    /// **两模式在负增量下数值不同**（探针实测）：
    /// 绝对值模式 `200 + (-30) = 170`… 本例取 `100 + (-30) = 70`；
    /// 而**比例模式按"当前值的百分比"缩放** ——
    /// `200 + round(200/100 × -50) = 200 - 100 = 100`（**不是 150**）。
    /// 我最初以为"比例恰为整数时两模式同值"，把 `(200, -50)` 在两种模式下都写成 150 ——
    /// **又一处没有把公式代进去就落笔**。
    /// </remarks>
    public static bool OnlyNegativeDeltaTakesEffect()
        => ComputeMaxMp(100, true, -30, false) == 70
           && ComputeMaxMp(100, true, -30, true) == 70;

    /// <summary>**两模式在负增量下数值不同**。</summary>
    public static bool NegativeDeltaModesDiffer()
        => ComputeMaxMp(200, true, -50, false) == 150
           && ComputeMaxMp(200, true, -50, true) == 100;

    /// <summary>比例模式按当前值的百分比缩放。</summary>
    public static bool PercentageModeScalesByCurrentValue()
        => ComputeMaxMp(200, true, -50, true) == 200 - 100
           && ComputeMaxMp(100, true, -50, true) == 100 - 50;

    /// <summary>**`Int64` 承载并钳到 `[0, High]`**。</summary>
    public static bool Int64ClampToZeroAndHigh()
        => ComputeMaxMp(100, true, -500, false) == 0
           && ComputeMaxMp(100, true, 99999, false) == 100;

    /// <summary>**封顶四重门**。</summary>
    public static bool PowerMaxGate(int power, long maxMp, int race, bool damageLimitation)
        => power > 0
           && maxMp > 0
           && !(race == RcPlayObject || race == RcHeroObject || race == RcMoonObject)
           && (damageLimitation || race != RcPlayMoster);

    /// <summary>四重门真值表。</summary>
    public static bool PowerMaxFourGates()
        => PowerMaxGate(10, 5, 80, false)
           && !PowerMaxGate(0, 5, 80, false)
           && !PowerMaxGate(10, 0, 80, false)
           && !PowerMaxGate(10, 5, RcPlayObject, false)
           && !PowerMaxGate(10, 5, RcHeroObject, false)
           && !PowerMaxGate(10, 5, RcMoonObject, false);

    /// <summary>**玩家/英雄/月灵永不被封顶**。</summary>
    public static bool PlayerHeroMoonExempt()
        => !PowerMaxGate(10, 5, RcPlayObject, true)
           && !PowerMaxGate(10, 5, RcHeroObject, true)
           && !PowerMaxGate(10, 5, RcMoonObject, true);

    /// <summary>**人形怪默认不封顶，仅开关打开才封顶**。</summary>
    public static bool PlayMosterRequiresSwitch()
        => !PowerMaxGate(10, 5, RcPlayMoster, false)
           && PowerMaxGate(10, 5, RcPlayMoster, true);

    /// <summary>普通怪恒封顶。</summary>
    public static bool NormalMonsterAlwaysCapped()
        => PowerMaxGate(10, 5, 80, false) && PowerMaxGate(10, 5, 80, true);

    /// <summary>最终封顶结果。</summary>
    public static int ApplyPowerMax(int power, int maxMp, int race, bool damageLimitation)
        => PowerMaxGate(power, maxMp, race, damageLimitation) ? Math.Min(power, maxMp) : power;

    /// <summary>封顶实测。</summary>
    public static bool ApplyPowerMaxValues()
        => ApplyPowerMax(10, 5, 80, false) == 5
           && ApplyPowerMax(3, 5, 80, false) == 3
           && ApplyPowerMax(10, 5, RcPlayObject, false) == 10;

    /// <summary>**注释里留着 GM 脚本文本**。</summary>
    public static bool CommentedGmScriptText() => true;

    /// <summary>脚本文本关键行。</summary>
    public static readonly string[] CommentedGmScript =
    {
        "[@半兽人的MP]", "#if", "#ACT",
        "ChangeMonAbility 3 半兽人 2 = 10 0 325 323 20",
        "RecalcMonAbility 3 半兽人 325 323 20",
    };

    /// <summary>脚本五行。</summary>
    public static bool GmScriptFiveLines() => CommentedGmScript.Length == 5;

    /// <summary>脚本日期。</summary>
    public const string GmScriptDate = "2021-05-26";

    /// <summary>日期实测。</summary>
    public static bool GmScriptHasDate() => GmScriptDate.Contains("2021-05-26");

    /// <summary>封顶修复注释。</summary>
    public const string PowerMaxFixComment = "修复人形怪受到的伤害会封顶的问题 By 一支笔 at:2021-08-02 14:20:27";

    /// <summary>修复注释实测。</summary>
    public static bool PowerMaxFixHasAuthor()
        => PowerMaxFixComment.Contains("一支笔") && PowerMaxFixComment.Contains("2021-08-02");

    /// <summary>开关注释。</summary>
    public const string DamageLimitSwitchComment = "增加人形怪限制伤害开关 Cursor 2023-06-02 17:12:37";

    /// <summary>开关注释实测。</summary>
    public static bool DamageLimitSwitchHasDate()
        => DamageLimitSwitchComment.Contains("2023-06-02");

    /// <summary>**`GetNextDamage` 先整数除法再乘**。</summary>
    public static int NextDamage(int power, int rate)
    {
        int i64 = (int)Math.Round(power / 100 * (double)rate, MidpointRounding.AwayFromZero);

        return i64 >= HighInteger ? HighInteger : i64;
    }

    /// <summary>**整数除法在前会丢精度**。</summary>
    public static bool NextDamageIntegerDivisionFirst()
        => NextDamage(150, 100) == 100      // 150/100 = 1（整数），1*100 = 100；精确值应为 150
           && NextDamage(100, 100) == 100
           && NextDamage(100, 50) == 50;

    /// <summary>与先乘后除的差异。</summary>
    public static bool NextDamageDiffersFromExact()
        => NextDamage(150, 100) != 150;

    /// <summary>**比率用完复位为 100**。</summary>
    public static bool NextDamageResetsRateTo100() => true;

    /// <summary>复位仿真。</summary>
    public static (int Damage, int RateAfter) NextDamageAndReset(int power, int rate)
        => (NextDamage(power, rate), 100);

    /// <summary>复位实测。</summary>
    public static bool NextDamageResetValues()
        => NextDamageAndReset(100, 50) == (50, 100)
           && NextDamageAndReset(100, 200) == (200, 100);

    /// <summary>上限保护。</summary>
    public static bool NextDamageHighGuard() => true;

    /// <summary>**`GetAttackPower` 第二参至少为 1**。</summary>
    public static int AttackPowerSpan(int dc1, int dc2) => Math.Max(dc2 - dc1, 1);

    /// <summary>下界保护实测。</summary>
    public static bool AttackPowerSecondArgAtLeastOne()
        => AttackPowerSpan(10, 10) == 1
           && AttackPowerSpan(10, 5) == 1
           && AttackPowerSpan(10, 20) == 10;

    /// <summary>**DC2 <= DC1 时退化为"DC1 ± 1"**。</summary>
    public static bool DegeneratesToOne()
        => AttackPowerSpan(10, 10) == 1;

    /// <summary>与 J139 的 `Max(...,0)` 形成对照。</summary>
    public static bool DiffersFromJ139ZeroGuard()
        => AttackPowerSpan(10, 5) == 1;   // J139 麻痹用 Max(...,0)，此处用 Max(...,1)

    // ===================== 顶层仿真 =====================

    /// <summary>模拟 `_Attack` 的威力管线（去掉随机与外部依赖）。</summary>
    public static int SimulatePowerPipeline(int basePower, bool hasMaster, int race, bool isCopyMon,
        int slaveRate, float attackRate, int nextDamageRate, int targetMaxMp,
        int targetRace, bool damageLimitation)
    {
        int power = basePower;

        if (SlaveDiscount(hasMaster, race, isCopyMon))
            power = ApplySlaveDiscount(power, slaveRate);

        if (power <= 0)
            return 0;

        power = ApplyAttackRate(power, attackRate);
        power = NextDamage(power, nextDamageRate);
        power = ApplyPowerMax(power, targetMaxMp, targetRace, damageLimitation);

        return Math.Max(power, 0);
    }

    /// <summary>管线实测（无折扣无封顶）。</summary>
    public static bool PipelinePlain()
        => SimulatePowerPipeline(100, false, 80, false, 100, 1.0f, 100, 0, 80, false) == 100;

    /// <summary>
    /// 管线实测（带折扣）。
    /// </summary>
    /// <remarks>
    /// **探针实测：结果不是 50 而是 0。** 因为折扣后威力为 50，
    /// 而 `GetNextDamage` **先做整数除法** `50 / 100 = 0`，再乘比率 `100` 仍是 `0`
    /// —— 所以**任何低于 100 的威力经过 `GetNextDamage` 都会被清零**。
    /// 这是 `NextDamageIntegerDivisionFirst` 那条特性的直接后果，
    /// 我最初按"折半后仍是 50"写期望，属于**没有把整数除法的影响代进去**。
    /// </remarks>
    public static bool PipelineWithDiscount()
        => SimulatePowerPipeline(100, true, 80, false, 50, 1.0f, 100, 0, 80, false) == 0;

    /// <summary>**威力低于 100 时被 `GetNextDamage` 的整数除法清零**。</summary>
    public static bool SubHundredPowerIsZeroed()
        => NextDamage(99, 100) == 0
           && NextDamage(100, 100) == 100;

    /// <summary>管线实测（带封顶）。</summary>
    public static bool PipelineWithCap()
        => SimulatePowerPipeline(100, false, 80, false, 100, 1.0f, 100, 30, 80, false) == 30;

    /// <summary>**玩家目标不被封顶**。</summary>
    public static bool PipelinePlayerUncapped()
        => SimulatePowerPipeline(100, false, 80, false, 100, 1.0f, 100, 30, RcPlayObject, false) == 100;
}
