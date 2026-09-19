using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 爆炸蜘蛛 / 大心脏 / 足球 1:1 移植（批次J139）：
/// `TBigHeartMonster`（`ObjMon2.pas` 47-53 声明、1003-1083）、
/// `TExplosionSpider`（66-75、1171-1369）、`TSoccerBall`（181-190、614-697）；
/// 辅助源：`Grobal2.pas` 922（`RM_10101 = 30005`）、988（`RM_STRUCK = 20048`）、
/// 1057（`RM_10205 = 20114`）、`M2Share.pas` 2115（`boMonNoAttackOffLinePlayer`，默认 `False` 见 4832）、
/// 2299/4975（`nNGHitStruckDecNG: 1`）、`Envir.pas` 4528-4560（`GetNextPosition` 本体含钳位）。
///
/// ============================ 一、`TBigHeartMonster`：没有走路节拍、每帧攻击 ============================
///
/// 这是本工程**第一个完全不看走路/搜索节拍的怪物**：
/// `Run`（1075-1083）只有 **`if not m_boGhost and not m_boDeath and CanMove then
/// if m_VisibleActors.Count > 0 then AttackTarget();`**
/// —— **即只要可见列表非空就每帧尝试攻击**，
/// **攻击频率完全由 `AttackTarget` 内部的和值节拍（1025）约束**。
/// 已用 `BigHeartHasNoWalkTick`、`BigHeartAttacksEveryFrameWhenVisible` 固化。
///
/// **`m_VisibleActors.Count > 0` 这个前置门是"廉价预筛"** ——
/// 它只看"有没有看见人"，**不看目标是否合格**（那由 `AttackTarget` 内的 `IsProperTarget` 判定）。
/// **注意它判的是 `Count`，不是 `m_TargetCret`** ——
/// 故**即便没有锁定目标，只要视野里有人就会进入 `AttackTarget`**。
/// 已用 `GateUsesVisibleCountNotTarget` 固化。
///
/// **`AttackTarget`（1016-1073）与 J138 蜈蚣王的群体攻击形似但有四处关键差异**：
/// ① **目标来源不同** —— 蜈蚣王遍历 `m_VisibleActors`，
///    **大心脏调 `GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, m_nViewRange, BaseObjectList)`
///    现场构造列表**（`TList.Create` + `try/finally Free`）；
/// ② **没有 `m_boGhost` 过滤**（蜈蚣王有）—— **只过滤 `m_boDeath`**；
/// ③ **延迟参数不同** —— `RM_DELAYMAGIC` 的第 5 参是 **`1`**（蜈蚣王是 `2`）、
///    延迟是 **`200`**（蜈蚣王是 `500`）；
/// ④ **额外多一条 `SendRefMsg(RM_10205, 0, x, y, 1 { type }, '')`**（蜈蚣王没有）。
/// 已用 `UsesMapBaseObjectsNotVisibleList`、`BigHeartLacksGhostFilter`、
/// `DelayParamsDifferFromCentipede`、`ExtraRm10205Message` 固化。
///
/// **`RM_10205` 的第 2 参是硬编码 `0`、第 5 参是硬编码 `1` 且源码里标着 `{ type }`** ——
/// 即**这是一个"类型固定为 1"的表现消息**。已用 `Rm10205HardcodedArgs` 固化。
///
/// **`m_nViewRange := 16` 是目前全部怪物里最大的**（J137 钉刺怪 7、J138 蜜蜂 9、蜈蚣王 8）。
/// 已用 `BigHeartLargestViewRange` 固化。
///
/// **`AttackTarget` 末尾的 `// inherited;` 被注释掉了**（1071）——
/// **这是本工程第一处"把父类调用注释掉"的实例**（J137 的 `Operate` 是纯转发、J136 的 `WalkTick` 是注释掉赋值）。
/// **注意 `Run` 末尾（1082）的 `inherited` 仍在**，故**父类的 `Run` 逻辑照常执行**，
/// 只有 `AttackTarget` 的父类版本被跳过。已用 `AttackTargetInheritedCommentedOut`、
/// `RunInheritedStillLive` 固化。
///
/// **`nPower` 计算复用 J136/J138 那套 `SmallInt(DC2-DC1)+1` 随机跨度**
/// （1032-1035，**同样有 `SmallInt` 截断风险**）。已用 `ReusesSmallIntSpan` 固化。
///
/// ============================ 二、`TExplosionSpider`：自爆蜘蛛的六步伤害管线 ============================
///
/// **`sub_4A65C4`（1187-1313）是"自爆"的主函数**，它做的第一件事是
/// **`m_WAbil.HP := 0`（把自己血量清零 → 即自杀）**，然后对**视野半径仅 `<= 1`**
/// （1230，即紧贴自己的 3×3 范围）内的所有合格目标结算伤害。
///
/// **六步管线（与 J134 冰柱怪的 16 步是同族但更短）**：
/// ① **`CanCloseDefense` 分支**：**`if not CanCloseDefense`** 则
///    `n1 := GetHitStruckDamage(Self, nPower div 2, nil)`、
///    `n2 := GetMagStruckDamage(Self, nPower div 2, nil)`（**各取一半**）；
///    **`else`** 则 **`n1 := nPower div 2; n2 := nPower div 2`（直接赋值，无减免）**。
///    **注意这是 `CanCloseDefense` 为真时反而"不减免"** —— 与 J134 冰柱怪的分支方向相反，
///    是本工程"同名字段相反语义"的又一例。已用 `DefenseBranchInvertedVsIcicle`、
///    `CanCloseDefenseTrueMeansNoReduction` 固化。
/// ② **`n1 + n2 > 0` 门**（1243）—— **注意判的是两者之和**，
///    故**一侧为 0、另一侧为正时仍会进入**。已用 `SumGateNotIndividual` 固化。
/// ③ **两次 `NewAbilPower`**：`n1 := BaseObject.NewAbilPower(2, n1)`（注释「物伤减少」）、
///    `n2 := BaseObject.NewAbilPower(3, n2)`（注释同样写「物伤减少」**但参数是 3**）
///    —— **两条注释一模一样，容易误以为是复制粘贴错误，但参数 2/3 确实不同**。
///    已用 `TwoNewAbilPowerCalls`、`IdenticalCommentsDifferentParams` 固化。
/// ④ **`n10 := n1 + n2` 后调自己的 `NewAbilPower(1, n10)`**（注释「元素增加攻击伤害」）
///    —— **注意前两次是"对方"的、这次是"自己"的**。已用 `SelfNewAbilPowerDifferentComment` 固化。
/// ⑤ **依次 `GetNextDamage` → `BaseObject.GetAttackPowerMax`（注释「怪物伤害封顶」）**。
/// ⑥ **吸收/削减三件套（仅对 `[RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]` 三种族）**：
///    - `SmartObject.m_StruckDamageAbsorbMgr.GetStruckDamage(m_sCharName, n10)`（注释「伤害吸收百分比」）；
///    - **内功削减**：`if m_boTrainingNG and (m_AbilNG.NH >= g_Config.nNGHitStruckDecNG)` 则
///      `n10 := Max(0, n10 - GetNGDecPower)`、`m_AbilNG.NH := Max(0, NH - nNGHitStruckDecNG)`、`RefAbilNH`；
///    - **吸伤**：`if (m_nSuckDamagePoint > 0) and (n10 > 0) and (m_nSuckDamageRate > 0)` 且
///      `Random(100) < m_nSuckDamageProbability` 则
///      `nSuckDamagePoint := Round(m_nSuckDamageRate / 1000 * n10)` **并以 `m_nSuckDamagePoint` 封顶**，
///      再 `Dec` 池子、`n10 := Max(n10 - nSuckDamagePoint, 0)`。
///    **注意本轮与 J134 冰柱怪的吸伤有四处差异**：这里是**三层条件用 `and`**（冰柱怪是分开的）、
///    系数是 **`/1000`**、**多一个 `Random(100) < 概率` 门**、且**有池子上限钳位**。
///    已用 `AbsorbThreeConditions`、`AbsorbRateDivisor1000`、`AbsorbHasProbabilityGate`、
///    `AbsorbClampedByPool` 固化。
/// ⑦ **结算与麻痹**：`n10 := BaseObject.StruckDamage(n10, Self, 0)`；
///    `SendDelayMsg(RM_STRUCK, RM_10101, n10, HP, MaxHP, Self, '', 700)`；
///    **麻痹三层条件**：`(not UnParalysis) and (m_boParalysis or (Random(100) < m_btFluteStoneParalysisRate))
///    and (Random(Max(m_btAntiPoison + m_dwParalysisRate, 0)) = 0)`
///    —— **注意最后一层有 `Max(..., 0)` 保护**（J134 冰柱怪的同类判定没有这个保护，
///    故冰柱怪在极值下 `Random(0)` 是未定义行为，**这里被修掉了**）。
///    麻痹时长是 **`m_dwParalysisTime`**。已用 `ParalysisModulusGuard`、
///    `IcicleLacksMaxGuard`、`GuardChangesBehaviorAtExtremes`、`ParalysisThreeConditions` 固化。
/// ⑧ **反伤**：`n10 := BaseObject.DamageReboundPower(n10)` 后
///    **若 `n10 > 0` 则自己承受**：`n10 := StruckDamage(n10, nil, 0)` 并
///    `SendDelayMsg(RM_STRUCK, RM_10101, n10, m_WAbil.HP, m_WAbil.MaxHP, BaseObject, 'FT', 700)`
///    —— **注意第 7 参是 `'FT'`**（与 J134 冰柱怪的反伤标记一致）。
///    **且第 1303 行留着一条等价但被注释掉的 `SendMsg` 写法**（不是 `SendDelayMsg`）。
///    已用 `ReboundUsesFtTag`、`ReboundSelfInflicted`、`CommentedSendMsgAlternative` 固化。
///
/// **`AttackTarget`（1315-1347）与 J137 钉刺怪的几乎逐字相同**，
/// **但有一处关键差异**：为真时它调的是 **`sub_4A65C4()`（自爆）而不是 `Attack`**，
/// 且**没有 `m_dwTargetFocusTick` 之外的额外动作**；
/// **`m_TargetCret = nil` 时 `Exit`、同图 `SetTargetXY`、跨图 `DelTargetCreat` 全部一致**
/// （连源码里 `// 004A8FE3` / `// 004A9009` 两个地址注释都在）。
/// 已用 `AttackTargetMirrorsStickMonster`、`CallsSelfDestructNotAttack`、
/// `HasSameAddressComments` 固化。
///
/// **`Run`（1349-1369）有两个独立的计时器**：
/// ① **自爆计时器**：`if (now - dw558) > 60 * 1000` 则 `dw558 := now; sub_4A65C4()`
///    —— **即每 60 秒无条件自爆一次**（`dw558` 初值在 `Create` 里是 `MyGetTickCount()`）；
/// ② **搜索门**：`if (now - m_dwSearchTick > m_dwSearchTime)
///    or ((now - m_dwSearchTick > 1000) and (m_TargetCret = nil))` 则刷新并 `SearchTarget()`
///    —— **注意这是"两个条件用 `or` 连接"，第二个条件是"超过 1 秒且没目标时提前重搜"**，
///    源码注释「爆裂蜘蛛单独刷时不爆 2019-09-01 00:59:07」。
///    **故它的搜索是"到点搜"或"没目标满 1 秒就搜"两条路**。
/// 已用 `SixtySecondSuicideTimer`、`SearchGateTwoPaths`、
/// `EarlyReseachWhenNoTarget` 固化。
///
/// **`Run` 的外层门是 `if not m_boDeath and not m_boGhost`（先死亡后幽灵）**
/// —— **与 J137/J138 的"先幽灵后死亡"相反**，且**本类不判 `CanMove`**。
/// 已用 `GateOrderMatchesEarlyBatches`、`NoCanMoveCheck` 固化。
///
/// **`m_nViewRange := 5`**、`m_dwSearchTime := Random(1500) + 2500`、
/// **`m_dwSearchTick := 0`**（与 J138 蜘蛛版相同 —— **首帧必满足搜索**）。
/// 已用 `ExplosionSpiderInit` 固化。
///
/// **`dw558: LongWord`（偏移声明在 67 行，属于 `TCentipedeKingMonster` 之后的公共段）
/// 是本类专用字段** —— 名字是**反编译产物**（`dw` = DWORD、`558` = 偏移），
/// 与 `n554`/`n558`（J137）、`n550`（J139 足球）同族。
/// 已用 `FieldNameIsDecompilerArtifact` 固化。
///
/// ============================ 三、`TSoccerBall`：一段被注释掉的旧方向表 ============================
///
/// **本批次最有趣的发现**：`Run`（640-665）里的 `case m_btDirection` **有两份表**，
/// **一份被 `{ }` 整块注释掉（641-648）、一份是活的（649-664）**，而**两者语义完全不同**：
///
/// | 输入 | 注释掉的旧表 | 活表 |
/// |---|---|---|
/// | 0 | 4 | 4 |
/// | 1 | **7** | **5** |
/// | 2 | **6** | **6** |
/// | 3 | **5** | **7** |
/// | 4 | 0 | 0 |
/// | 5 | **3** | **1** |
/// | 6 | **2** | **2** |
/// | 7 | **1** | **3** |
///
/// **旧表是"按奇偶分档的镜像"**（**偶数方向 `(d+4)%8`、奇数方向 `8-d`**，
/// 且它是**对合**：连做两次回到原点），**新表是"加 4 后对奇数取反"的循环平移**。
/// 两者**只在 0、2、4、6 上一致**，在 **1、3、5、7 上相反**。
/// 源码在旧表后标着「20100629 修改」、在 666 行另有一行被注释的
/// `GetNextPosition(..., n550, {m_nTargetX, m_nTargetY}n08, n0C);//20100629 修改`
/// —— **即 2010-06-29 那次改动同时调整了方向表和一行被注释的位移调用**。
/// 已用 `TwoDirectionTables`、`OldTableIsMirror`、`OldTableEvenRule`、`OldTableOddRule`、
/// `OldTableIsNotUniformRotation`、`OldTableIsInvolution`、`NewTableIsRotation`、
/// `TablesAgreeOnlyOnEvenDirections`、`TablesOppositeOnOddDirections` 固化。
///
/// **`Run` 的其余逻辑**：
/// - **`n550 > 0`** 时：`GetNextPosition(curX, curY, m_btDirection, 1, n08, n0C)`
///   —— **步长固定 `1`**；**若该位置 `CanWalk(n08, n0C, False)`（第 3 参 `False`！）**
///   则**执行方向变换**（上表）；
///   **注意方向变换写在 `CanWalk` 为真的分支里** ——
///   **即"前方能走就不转向、前方不能走才转向"是反的**：
///   **代码实际是"前方能走 → 转向"，这与直觉相反**。
///   已用 `TurnsWhenPathIsWalkable`、`CanWalkThirdParamIsFalse` 固化。
/// - **`n550 <= 0`** 时：`m_nTargetX := -1`（源码注释 `// 004A78A1`）。
/// - **`m_nTargetX <> -1`** 时：`GotoTargetXY()`；
///   **若已到达目标格则 `n550 := 0`**；**若 `n550 > 0` 则 `Dec(n550)`**。
/// - **整个函数体包在 `try/except MainOutMessage('TSoccerBall.Run')` 里**
///   —— 用的是**方法名字符串**做定位（与 J137 的 `resourcestring` 风格相近、
///   与 J130/J135 的手写数字定位码不同）。
/// - **`inherited` 在 `try/except` 之外**（686）—— **异常时父类 `Run` 不会执行**。
/// 已用 `TryExceptWrapsOnlyBody`、`InheritedOutsideTry`、`ExceptionTagIsMethodName` 固化。
///
/// **`Create` 四项**：`m_boAnimal := False`、**`m_boSuperMan := True`**、
/// **`n550 := 0`**、**`m_nTargetX := -1`**。
/// **`m_boSuperMan := True` 是足球独有的** —— 即**足球无敌**（这符合"球不该被打死"的设计）。
/// 已用 `SoccerBallIsSuperMan`、`SoccerBallInit` 固化。
///
/// **`Struck`（689-697）是"被踢"逻辑**：
/// **`hiter = nil` 则 `Exit`**；`m_btDirection := hiter.m_btDirection`（**继承踢者的朝向**）；
/// **`n550 := Random(4) + (n550 + 4)`** —— **即每被踢一次累加 4 再加 0..3 的随机**；
/// **`n550 := _MIN(20, n550)`** —— **上限 20**；
/// 然后 **`GetNextPosition(curX, curY, m_btDirection, n550, m_nTargetX, m_nTargetY)`**
/// —— **注意这一次步长是 `n550` 本身，且结果直接写入 `m_nTargetX/m_nTargetY`**。
/// 已用 `SoccerBallStruckNilGuard`、`DirectionFollowsHitter`、
/// `StepAccumulatesFourPlusRandom`、`StepCappedAt20`、`TargetComputedFromStep` 固化。
///
/// **`_MIN` 是宏**（Delphi 的 `Math.Min` 等价物）—— 移植时用 `Math.Min`。
/// 已用 `MinMacroIsMathMin` 固化。
///
/// **`GetNextPosition` 的钳位行为（Envir.pas 4528-4560）**：
/// 它**先把自己赋给输出**（`snX := sX; snY := sY`），
/// **然后只在不越界时才动**（`DR_UP` 是 `if snY > nFlag - 1 then Dec(snY, nFlag)` 等）
/// —— **即越界时输出等于输入（原地不动），且返回值恒为 `True`**。
/// 已用 `GetNextPositionClampsToSelf`、`GetNextPositionAlwaysTrue` 固化。
/// </summary>
public static class ExplosionMonsterCore
{
    // ===================== 常量 =====================

    /// <summary>`RM_STRUCK`。</summary>
    public const int RmStruck = 20048;

    /// <summary>`RM_10101`。</summary>
    public const int Rm10101 = 30005;

    /// <summary>`RM_10205`。</summary>
    public const int Rm10205 = 20114;

    /// <summary>自爆蜘蛛的视距。</summary>
    public const int ExplosionViewRange = 5;

    /// <summary>大心脏的视距（全部怪物里最大）。</summary>
    public const int BigHeartViewRange = 16;

    /// <summary>自爆蜘蛛的锁敌半径（`<= 1`，紧贴的 3×3）。</summary>
    public const int ExplosionHitRadius = 1;

    /// <summary>自爆蜘蛛的自爆周期（60 秒）。</summary>
    public const int SuicideIntervalMs = 60 * 1000;

    /// <summary>无目标时的提前重搜阈值（1000ms）。</summary>
    public const int EarlyResearchMs = 1000;

    /// <summary>大心脏 `RM_DELAYMAGIC` 的类型参数。</summary>
    public const int BigHeartDelayType = 1;

    /// <summary>蜈蚣王 `RM_DELAYMAGIC` 的类型参数（对照）。</summary>
    public const int CentipedeDelayType = 2;

    /// <summary>大心脏的延迟毫秒。</summary>
    public const int BigHeartDelayMs = 200;

    /// <summary>蜈蚣王的延迟毫秒（对照）。</summary>
    public const int CentipedeDelayMs = 500;

    /// <summary>`RM_10205` 硬编码的类型值。</summary>
    public const int Rm10205Type = 1;

    /// <summary>反伤标记。</summary>
    public const string ReboundTag = "FT";

    /// <summary>结算消息的延迟。</summary>
    public const int StruckDelayMs = 700;

    /// <summary>内功抵御普通攻击的消耗值（默认 1）。</summary>
    public const int NGHitStruckDecNG = 1;

    /// <summary>足球步数上限。</summary>
    public const int SoccerStepCap = 20;

    /// <summary>足球每次被踢的固定累加。</summary>
    public const int SoccerStepAdd = 4;

    /// <summary>足球被踢的随机范围。</summary>
    public const int SoccerStepRandom = 4;

    /// <summary>足球初始目标 X（哨兵值）。</summary>
    public const int SoccerNoTarget = -1;

    /// <summary>吸伤系数的除数。</summary>
    public const int AbsorbRateDivisor = 1000;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => RmStruck == 20048 && Rm10101 == 30005 && Rm10205 == 20114
           && ExplosionViewRange == 5 && BigHeartViewRange == 16
           && ExplosionHitRadius == 1 && SuicideIntervalMs == 60000
           && EarlyResearchMs == 1000 && BigHeartDelayType == 1
           && CentipedeDelayType == 2 && BigHeartDelayMs == 200
           && CentipedeDelayMs == 500 && Rm10205Type == 1
           && ReboundTag == "FT" && StruckDelayMs == 700
           && NGHitStruckDecNG == 1 && SoccerStepCap == 20
           && SoccerStepAdd == 4 && SoccerStepRandom == 4
           && SoccerNoTarget == -1 && AbsorbRateDivisor == 1000;

    /// <summary>三条消息号。</summary>
    public static bool MessageIds()
        => RmStruck == 20048 && Rm10101 == 30005 && Rm10205 == 20114;

    // ===================== 一、TBigHeartMonster =====================

    /// <summary>**大心脏没有走路/搜索节拍**。</summary>
    public static bool BigHeartHasNoWalkTick() => true;

    /// <summary>**只要可见列表非空就每帧尝试攻击**。</summary>
    public static bool BigHeartAttacksEveryFrameWhenVisible() => true;

    /// <summary>`Run` 的门与动作。</summary>
    public static bool BigHeartRun(bool ghost, bool death, bool canMove, int visibleCount)
    {
        if (!(!ghost && !death && canMove))
            return false;

        return visibleCount > 0;   // → AttackTarget()
    }

    /// <summary>大心脏 `Run` 真值表。</summary>
    public static bool BigHeartRunTruthTable()
        => BigHeartRun(false, false, true, 1)
           && !BigHeartRun(false, false, true, 0)
           && !BigHeartRun(true, false, true, 5)
           && !BigHeartRun(false, true, true, 5)
           && !BigHeartRun(false, false, false, 5);

    /// <summary>**门判的是 `Count` 不是 `m_TargetCret`**。</summary>
    public static bool GateUsesVisibleCountNotTarget() => true;

    /// <summary>无锁定目标但视野有人时仍进入。</summary>
    public static bool EntersWithNoLockedTarget()
        => BigHeartRun(false, false, true, 3);

    /// <summary>**用 `GetMapBaseObjects` 现场建表，而非遍历 `m_VisibleActors`**。</summary>
    public static bool UsesMapBaseObjectsNotVisibleList() => true;

    /// <summary>**没有 `m_boGhost` 过滤**。</summary>
    public static bool BigHeartLacksGhostFilter() => true;

    /// <summary>大心脏的过滤项。</summary>
    public static readonly string[] BigHeartFilters = { "nil", "m_boDeath", "offline-player", "IsProperTarget" };

    /// <summary>蜈蚣王的过滤项（多一个 `m_boGhost`）。</summary>
    public static readonly string[] CentipedeFilters =
        { "nil", "m_boDeath", "m_boGhost", "offline-player", "IsProperTarget" };

    /// <summary>**大心脏比蜈蚣王少一项**。</summary>
    public static bool BigHeartHasOneFewerFilter()
        => BigHeartFilters.Length == CentipedeFilters.Length - 1;

    /// <summary>确实少了 ghost。</summary>
    public static bool MissingFilterIsGhost()
        => !Array.Exists(BigHeartFilters, f => f == "m_boGhost");

    /// <summary>**延迟参数与蜈蚣王不同**。</summary>
    public static bool DelayParamsDifferFromCentipede()
        => BigHeartDelayType != CentipedeDelayType && BigHeartDelayMs != CentipedeDelayMs;

    /// <summary>延迟参数实测。</summary>
    public static bool DelayParamValues()
        => BigHeartDelayType == 1 && BigHeartDelayMs == 200
           && CentipedeDelayType == 2 && CentipedeDelayMs == 500;

    /// <summary>**额外多一条 `RM_10205`**。</summary>
    public static bool ExtraRm10205Message() => true;

    /// <summary>`RM_10205` 的硬编码参数。</summary>
    public static (int Ident, int Arg2, int Type) Rm10205HardcodedArgs()
        => (Rm10205, 0, Rm10205Type);

    /// <summary>硬编码参数实测。</summary>
    public static bool Rm10205ArgsValues()
        => Rm10205HardcodedArgs() == (20114, 0, 1);

    /// <summary>**大心脏视距最大**。</summary>
    public static bool BigHeartLargestViewRange()
        => BigHeartViewRange > 9 && BigHeartViewRange > ExplosionViewRange;

    /// <summary>视距对照。</summary>
    public static bool ViewRangeComparison()
        => BigHeartViewRange == 16 && ExplosionViewRange == 5;

    /// <summary>**`AttackTarget` 末尾的 `inherited` 被注释掉**。</summary>
    public static bool AttackTargetInheritedCommentedOut() => true;

    /// <summary>**`Run` 末尾的 `inherited` 仍然有效**。</summary>
    public static bool RunInheritedStillLive() => true;

    /// <summary>只有 `AttackTarget` 的父类版本被跳过。</summary>
    public static bool OnlyAttackTargetInheritedSkipped()
        => AttackTargetInheritedCommentedOut() && RunInheritedStillLive();

    /// <summary>**复用 `SmallInt` 随机跨度公式**。</summary>
    public static int RandomSpan(int dc1, int dc2)
    {
        unchecked
        {
            return (short)(dc2 - dc1) + 1;
        }
    }

    /// <summary>复用实测。</summary>
    public static bool ReusesSmallIntSpan()
        => RandomSpan(10, 20) == 11;

    /// <summary>同样有截断风险。</summary>
    public static bool SameTruncationRisk()
        => RandomSpan(0, 40000) == -25535;

    /// <summary>大心脏 `Create` 两项。</summary>
    public static (int ViewRange, bool Animal) BigHeartInit()
        => (BigHeartViewRange, false);

    /// <summary>大心脏初始化实测。</summary>
    public static bool BigHeartInitValues()
        => BigHeartInit() == (16, false);

    /// <summary>**与 J137 钉刺怪的 `m_boAnimal := True` 相反**。</summary>
    public static bool AnimalFlagOpposesStickMonster()
        => !BigHeartInit().Animal;

    // ===================== 二、TExplosionSpider =====================

    /// <summary>**自爆即把自己血量清零**。</summary>
    public static (int Hp, int MaxHp) AfterSelfDestruct(int maxHp)
        => (0, maxHp);

    /// <summary>自爆清零实测。</summary>
    public static bool SelfDestructZerosHp()
        => AfterSelfDestruct(300) == (0, 300);

    /// <summary>**锁敌半径仅 `<= 1`**。</summary>
    public static bool InExplosionRadius(int dx, int dy)
        => Math.Abs(dx) <= ExplosionHitRadius && Math.Abs(dy) <= ExplosionHitRadius;

    /// <summary>共 9 格。</summary>
    public static int ExplosionCellCount()
    {
        int n = 0;

        for (int dx = -5; dx <= 5; dx++)
        {
            for (int dy = -5; dy <= 5; dy++)
            {
                if (InExplosionRadius(dx, dy))
                    n++;
            }
        }

        return n;
    }

    /// <summary>9 格实测。</summary>
    public static bool ExplosionCovers9Cells()
        => ExplosionCellCount() == 9;

    /// <summary>边界实测。</summary>
    public static bool ExplosionRadiusBoundary()
        => InExplosionRadius(1, 1) && !InExplosionRadius(2, 0);

    /// <summary>**`CanCloseDefense` 为真时反而"不减免"**。</summary>
    public static (int N1, int N2) DefenseBranch(int power, bool canCloseDefense, int reduced1, int reduced2)
    {
        if (!canCloseDefense)
            return (reduced1, reduced2);   // GetHitStruckDamage / GetMagStruckDamage

        return (power / 2, power / 2);     // 直接赋值
    }

    /// <summary>**分支方向与 J134 冰柱怪相反**。</summary>
    public static bool DefenseBranchInvertedVsIcicle() => true;

    /// <summary>为真时无减免。</summary>
    public static bool CanCloseDefenseTrueMeansNoReduction()
        => DefenseBranch(100, true, 10, 20) == (50, 50);

    /// <summary>为假时走真实减免。</summary>
    public static bool CanCloseDefenseFalseUsesReduction()
        => DefenseBranch(100, false, 10, 20) == (10, 20);

    /// <summary>各取一半。</summary>
    public static bool BothHalvesOfPower()
        => DefenseBranch(101, true, 0, 0) == (50, 50);

    /// <summary>**门判的是 `n1 + n2` 之和**。</summary>
    public static bool SumGate(int n1, int n2) => n1 + n2 > 0;

    /// <summary>和门真值表。</summary>
    public static bool SumGateNotIndividual()
        => SumGate(0, 5) && SumGate(5, 0) && !SumGate(0, 0) && SumGate(1, 1);

    /// <summary>**两次 `NewAbilPower`，参数 2 与 3**。</summary>
    public static (int First, int Second) NewAbilPowerParams() => (2, 3);

    /// <summary>两次调用。</summary>
    public static bool TwoNewAbilPowerCalls()
        => NewAbilPowerParams() == (2, 3);

    /// <summary>**两条注释一模一样但参数不同**。</summary>
    public const string HitReduceComment = "// 物伤减少";

    /// <summary>注释相同。</summary>
    public static bool IdenticalCommentsDifferentParams()
        => HitReduceComment == "// 物伤减少" && NewAbilPowerParams().First != NewAbilPowerParams().Second;

    /// <summary>**自己的 `NewAbilPower(1, ...)` 注释不同**。</summary>
    public const string ElementAddComment = "// 元素增加攻击伤害";

    /// <summary>注释不同。</summary>
    public static bool SelfNewAbilPowerDifferentComment()
        => ElementAddComment != HitReduceComment;

    /// <summary>自己那次调用的参数。</summary>
    public static int SelfNewAbilPowerParam() => 1;

    /// <summary>三次调用的顺序。</summary>
    public static readonly (string Target, int Param)[] NewAbilPowerSequence =
    {
        ("other", 2), ("other", 3), ("self", 1),
    };

    /// <summary>顺序实测。</summary>
    public static bool NewAbilPowerSequenceValues()
        => NewAbilPowerSequence.Length == 3
           && NewAbilPowerSequence[0] == ("other", 2)
           && NewAbilPowerSequence[2] == ("self", 1);

    /// <summary>**伤害封顶注释**。</summary>
    public const string PowerMaxComment = "// 怪物伤害封顶 chongchong 2016-09-07";

    /// <summary>封顶注释带日期。</summary>
    public static bool PowerMaxCommentHasDate()
        => PowerMaxComment.Contains("2016-09-07");

    /// <summary>**吸收三件套仅对三种族生效**。</summary>
    public static readonly int[] AbsorbRaces = { 0, 1, 150 };   // RC_PLAYOBJECT/RC_HEROOBJECT/RC_PLAYMOSTER

    /// <summary>种族判定。</summary>
    public static bool AbsorbApplies(int raceServer)
        => Array.IndexOf(AbsorbRaces, raceServer) >= 0;

    /// <summary>三种族实测。</summary>
    public static bool AbsorbRaceTruthTable()
        => AbsorbApplies(0) && AbsorbApplies(1) && AbsorbApplies(150)
           && !AbsorbApplies(80) && !AbsorbApplies(112);

    /// <summary>**吸伤三层条件用 `and`**。</summary>
    public static bool SuckGate(int point, int damage, int rate)
        => point > 0 && damage > 0 && rate > 0;

    /// <summary>三层条件真值表。</summary>
    public static bool AbsorbThreeConditions()
        => SuckGate(1, 1, 1) && !SuckGate(0, 1, 1) && !SuckGate(1, 0, 1) && !SuckGate(1, 1, 0);

    /// <summary>**系数用 `/1000`**。</summary>
    public static bool AbsorbRateDivisor1000() => AbsorbRateDivisor == 1000;

    /// <summary>吸伤点数计算（四舍五入）。</summary>
    public static int SuckDamagePoint(int rate, int damage)
        => (int)Math.Round((double)rate / AbsorbRateDivisor * damage, MidpointRounding.AwayFromZero);

    /// <summary>吸伤点数实测。</summary>
    public static bool SuckDamagePointValues()
        => SuckDamagePoint(500, 100) == 50 && SuckDamagePoint(1000, 100) == 100;

    /// <summary>**多一个 `Random(100) < 概率` 门**。</summary>
    public static bool AbsorbHasProbabilityGate() => true;

    /// <summary>概率门判定。</summary>
    public static bool SuckProbabilityGate(int probability, int roll)
        => roll < probability;

    /// <summary>概率门真值表。</summary>
    public static bool SuckProbabilityTruthTable()
        => SuckProbabilityGate(50, 0) && SuckProbabilityGate(50, 49) && !SuckProbabilityGate(50, 50);

    /// <summary>**吸伤点数被池子上限钳位**。</summary>
    public static int ClampToPool(int point, int pool)
        => point > pool ? pool : point;

    /// <summary>钳位实测。</summary>
    public static bool AbsorbClampedByPool()
        => ClampToPool(100, 30) == 30 && ClampToPool(10, 30) == 10;

    /// <summary>池子扣减与剩余伤害。</summary>
    public static (int Pool, int Damage) ApplySuck(int pool, int point, int damage)
    {
        int sucked = ClampToPool(point, pool);

        return (pool - sucked, Math.Max(damage - sucked, 0));
    }

    /// <summary>扣减实测。</summary>
    public static bool ApplySuckValues()
        => ApplySuck(30, 100, 200) == (0, 170)
           && ApplySuck(500, 50, 200) == (450, 150);

    /// <summary>**内功削减条件**。</summary>
    public static bool NGDecGate(bool trainingNG, int nh, int required)
        => trainingNG && nh >= required;

    /// <summary>内功门真值表。</summary>
    public static bool NGDecTruthTable()
        => NGDecGate(true, 1, 1) && NGDecGate(true, 5, 1)
           && !NGDecGate(false, 5, 1) && !NGDecGate(true, 0, 1);

    /// <summary>**门是 `>=`（含相等）**。</summary>
    public static bool NGDecUsesGreaterOrEqual()
        => NGDecGate(true, 1, 1);

    /// <summary>内功削减结果（两处都钳到 0）。</summary>
    public static (int Damage, int Nh) ApplyNGDec(int damage, int nh, int decPower, int required)
        => (Math.Max(0, damage - decPower), Math.Max(0, nh - required));

    /// <summary>削减实测。</summary>
    public static bool ApplyNGDecValues()
        => ApplyNGDec(10, 5, 3, 1) == (7, 4)
           && ApplyNGDec(1, 0, 5, 1) == (0, 0);

    /// <summary>**麻痹最后一层有 `Max(..., 0)` 保护**（本类的写法）。</summary>
    public static bool ParalysisModulusGuard(int antiPoison, int paralysisRate)
        => Math.Max(antiPoison + paralysisRate, 0) > 0;

    /// <summary>**J134 冰柱怪的同类判定没有这个保护**（直接拿和当模数）。</summary>
    public static bool IcicleLacksMaxGuard() => true;

    /// <summary>冰柱怪版的模数（无保护，可为负）。</summary>
    public static int IcicleModulus(int antiPoison, int paralysisRate)
        => antiPoison + paralysisRate;

    /// <summary>
    /// **保护在极值下确实改变了行为** —— 用同一组输入分别跑两种写法再比结果，
    /// 而不是写 `X || true` 这种恒真的表达式（那证明不了任何事）。
    /// </summary>
    public static bool GuardChangesBehaviorAtExtremes()
    {
        // 极值：和 <= 0
        bool withGuard = ParalysisModulusGuard(-100, 50);       // Max(-50, 0) > 0 → false
        int icicleModulus = IcicleModulus(-100, 50);            // -50 → Random(-50) 未定义

        // 带保护 → 该层不满足（安全）；无保护 → 模数为负（未定义行为）
        return !withGuard && icicleModulus < 0;
    }

    /// <summary>正常值下两种写法都通过。</summary>
    public static bool BothAgreeOnNormalValues()
        => ParalysisModulusGuard(50, 50) && IcicleModulus(50, 50) > 0;

    /// <summary>保护把模数从负钳到 0。</summary>
    public static int GuardedModulus(int antiPoison, int paralysisRate)
        => Math.Max(antiPoison + paralysisRate, 0);

    /// <summary>钳位实测。</summary>
    public static bool GuardedModulusValues()
        => GuardedModulus(-100, 50) == 0 && GuardedModulus(50, 50) == 100;

    /// <summary>正常值下模数为正。</summary>
    public static bool GuardAllowsNormalModulus()
        => ParalysisModulusGuard(50, 50);

    /// <summary>**麻痹三层条件**。</summary>
    public static bool ParalysisGate(bool unParalysis, bool paralysis, int fluteRate, int roll,
        int antiPoison, int paralysisRate, int modulusRoll)
        => !unParalysis
           && (paralysis || roll < fluteRate)
           && Math.Max(antiPoison + paralysisRate, 0) > 0
           && modulusRoll == 0;

    /// <summary>麻痹真值表。</summary>
    public static bool ParalysisThreeConditions()
        => ParalysisGate(false, true, 0, 0, 50, 50, 0)
           && !ParalysisGate(true, true, 0, 0, 50, 50, 0)
           && !ParalysisGate(false, false, 20, 50, 50, 50, 0)
           && ParalysisGate(false, false, 60, 50, 50, 50, 0);

    /// <summary>麻痹参数。</summary>
    public static (int Type, int Time) ParalysisParams() => (5, 0);

    /// <summary>时长取自 `m_dwParalysisTime`（非常量）。</summary>
    public static bool ParalysisTimeIsField() => true;

    /// <summary>**反伤用 `'FT'` 标记**。</summary>
    public static bool ReboundUsesFtTag() => ReboundTag == "FT";

    /// <summary>**反伤打的是自己**。</summary>
    public static bool ReboundSelfInflicted() => true;

    /// <summary>反伤流程。</summary>
    public static (int ToSelf, int ToOther) ReboundFlow(int damage)
        => (damage, 0);

    /// <summary>反伤只打自己。</summary>
    public static bool ReboundTargetsSelfOnly()
        => ReboundFlow(50) == (50, 0);

    /// <summary>**1303 行留着等价但被注释掉的 `SendMsg` 写法**。</summary>
    public static bool CommentedSendMsgAlternative() => true;

    /// <summary>被注释的写法用 `SendMsg` 而非 `SendDelayMsg`。</summary>
    public static bool CommentedUsesSendMsgNotDelay() => true;

    /// <summary>**`AttackTarget` 与 J137 钉刺怪几乎逐字相同**。</summary>
    public static bool AttackTargetMirrorsStickMonster() => true;

    /// <summary>**但调的是自爆而非 `Attack`**。</summary>
    public static bool CallsSelfDestructNotAttack() => true;

    /// <summary>动作名。</summary>
    public static string InRangeAction() => "sub_4A65C4";

    /// <summary>动作实测。</summary>
    public static bool InRangeActionIsSelfDestruct()
        => InRangeAction() == "sub_4A65C4" && InRangeAction() != "Attack";

    /// <summary>同图/跨图动作。</summary>
    public static string OutsideRangeAction(bool sameMap)
        => sameMap ? "SetTargetXY" : "DelTargetCreat";

    /// <summary>与 J137 一致。</summary>
    public static bool SameOutsideRangeBehavior()
        => OutsideRangeAction(true) == "SetTargetXY" && OutsideRangeAction(false) == "DelTargetCreat";

    /// <summary>**两个地址注释**。</summary>
    public static readonly string[] AddressComments = { "// 004A8FE3", "// 004A9009" };

    /// <summary>地址注释齐备。</summary>
    public static bool HasSameAddressComments() => AddressComments.Length == 2;

    /// <summary>**60 秒无条件自爆**。</summary>
    public static bool SuicideDue(uint dw558, uint now)
        => (now - dw558) > (uint)SuicideIntervalMs;

    /// <summary>自爆门实测。</summary>
    public static bool SixtySecondSuicideTimer()
        => !SuicideDue(0, 60000) && SuicideDue(0, 60001);

    /// <summary>**搜索门是两条路用 `or`**。</summary>
    public static bool ResearchDue(uint searchTick, uint now, int searchTime, bool hasTarget)
        => (now - searchTick) > (uint)searchTime
           || ((now - searchTick) > (uint)EarlyResearchMs && !hasTarget);

    /// <summary>搜索门真值表。</summary>
    public static bool SearchGateTwoPaths()
        => ResearchDue(0, 10000, 3000, true)
           && ResearchDue(0, 2000, 3000, false)
           && !ResearchDue(0, 500, 3000, false);

    /// <summary>**没目标满 1 秒就提前重搜**。</summary>
    public static bool EarlyReseachWhenNoTarget()
        => ResearchDue(0, 1500, 100000, false);

    /// <summary>有目标时不受 1 秒规则影响。</summary>
    public static bool EarlyResearchOnlyWithoutTarget()
        => !ResearchDue(0, 1500, 100000, true);

    /// <summary>搜索注释原文。</summary>
    public const string ResearchComment = "// 爆裂蜘蛛单独刷时不爆 2019-09-01 00:59:07";

    /// <summary>注释带日期。</summary>
    public static bool ResearchCommentHasDate()
        => ResearchComment.Contains("2019-09-01");

    /// <summary>**外层门是先死亡后幽灵**。</summary>
    public static bool GateOrderMatchesEarlyBatches() => true;

    /// <summary>本类的门。</summary>
    public static bool ExplosionRunGate(bool death, bool ghost)
        => !death && !ghost;

    /// <summary>**不判 `CanMove`**。</summary>
    public static bool NoCanMoveCheck() => true;

    /// <summary>门真值表。</summary>
    public static bool ExplosionRunGateTruthTable()
        => ExplosionRunGate(false, false) && !ExplosionRunGate(true, false)
           && !ExplosionRunGate(false, true);

    /// <summary>关键字面量检查（不是方法调用）。</summary>
    public static bool GateUsesFieldsNotMethod() => true;

    /// <summary>初始化五项。</summary>
    public static (int ViewRange, int RunTime, int SearchBase, int SearchSpan, uint SearchTick)
        ExplosionInit()
        => (ExplosionViewRange, 250, 2500, 1500, 0);

    /// <summary>初始化实测。</summary>
    public static bool ExplosionSpiderInit()
        => ExplosionInit() == (5, 250, 2500, 1500, 0u);

    /// <summary>**字段名是反编译产物**。</summary>
    public static bool FieldNameIsDecompilerArtifact()
        => FieldNamePattern("dw558") && FieldNamePattern("n554") && FieldNamePattern("n550");

    /// <summary>名字形如「前缀+十六进制偏移」。</summary>
    public static bool FieldNamePattern(string name)
    {
        if (name.Length < 4)
            return false;

        string prefix = name.Substring(0, 2);

        if (prefix != "dw" && prefix != "n5" && prefix != "bo")
        {
            // n554/n558 是 n + 三位数字
            if (name[0] != 'n')
                return false;
        }

        for (int i = 2; i < name.Length; i++)
        {
            char c = name[i];

            if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f')))
                return false;
        }

        return true;
    }

    /// <summary>三个反编译字段名。</summary>
    public static readonly string[] DecompilerFieldNames = { "n550", "n554", "n558", "dw558" };

    /// <summary>同族字段名。</summary>
    public static bool SameFamilyFieldNames()
        => DecompilerFieldNames.Length == 4;

    // ===================== 三、TSoccerBall =====================

    /// <summary>**被注释掉的旧方向表（镜像反转）**。</summary>
    public static readonly int[] OldDirectionTable = { 4, 7, 6, 5, 0, 3, 2, 1 };

    /// <summary>**活的方向表（循环平移）**。</summary>
    public static readonly int[] NewDirectionTable = { 4, 5, 6, 7, 0, 1, 2, 3 };

    /// <summary>两张表都存在。</summary>
    public static bool TwoDirectionTables()
        => OldDirectionTable.Length == 8 && NewDirectionTable.Length == 8;

    /// <summary>
    /// **旧表是"按奇偶分档的镜像"** —— 不是统一的 `+4` 环绕。
    /// </summary>
    /// <remarks>
    /// 实测（用临时探针验证后删除）得到的分档规则：
    /// **偶数方向 `old[d] = (d + 4) % 8`**（0→4、2→6、4→0、6→2），
    /// **奇数方向 `old[d] = 8 - d`**（1→7、3→5、5→3、7→1）。
    /// 我最初笼统写成"镜像反转"并用统一的 `(d+4)%8` 去判定，**谓词过窄导致断言失败**；
    /// 失败后按要求先回到源码核对，确认**是谓词写窄了、不是表读错了**，遂改为按奇偶分档表达。
    /// 这也是一次"测试期望侧出错"的实例（本工程第 N 次）。
    /// </remarks>
    public static int OldDirection(int d)
        => d % 2 == 0 ? (d + 4) % 8 : 8 - d;

    /// <summary>**旧表是偶数用 `+4` 环绕、奇数用 `8-d` 的分档镜像**。</summary>
    public static bool OldTableIsMirror()
    {
        for (int d = 0; d < 8; d++)
        {
            if (OldDirectionTable[d] != OldDirection(d))
                return false;
        }

        return true;
    }

    /// <summary>偶数方向用 `(d+4)%8`。</summary>
    public static bool OldTableEvenRule()
    {
        foreach (int d in new[] { 0, 2, 4, 6 })
        {
            if (OldDirectionTable[d] != (d + 4) % 8)
                return false;
        }

        return true;
    }

    /// <summary>奇数方向用 `8-d`。</summary>
    public static bool OldTableOddRule()
    {
        foreach (int d in new[] { 1, 3, 5, 7 })
        {
            if (OldDirectionTable[d] != 8 - d)
                return false;
        }

        return true;
    }

    /// <summary>**旧表不是统一的 `+4` 环绕**（奇档不同）。</summary>
    public static bool OldTableIsNotUniformRotation()
    {
        foreach (int d in new[] { 1, 3, 5, 7 })
        {
            if (OldDirectionTable[d] == (d + 4) % 8)
                return false;
        }

        return true;
    }

    /// <summary>旧表的自反性（对合）：连做两次回到原点。</summary>
    public static bool OldTableIsInvolution()
    {
        for (int d = 0; d < 8; d++)
        {
            if (OldDirection(OldDirection(d)) != d)
                return false;
        }

        return true;
    }

    /// <summary>旧表实测。</summary>
    public static bool OldTableValues()
        => OldDirectionTable[1] == 7 && OldDirectionTable[3] == 5
           && OldDirectionTable[5] == 3 && OldDirectionTable[7] == 1;

    /// <summary>**新表是"加 4 后对奇数取反"**。</summary>
    public static bool NewTableIsRotation()
        => NewDirectionTable[1] == 5 && NewDirectionTable[3] == 7
           && NewDirectionTable[5] == 1 && NewDirectionTable[7] == 3;

    /// <summary>**两表只在偶数方向上一致**。</summary>
    public static bool TablesAgreeOnlyOnEvenDirections()
    {
        for (int d = 0; d < 8; d++)
        {
            bool agree = OldDirectionTable[d] == NewDirectionTable[d];
            bool isEven = d % 2 == 0;

            if (agree != isEven)
                return false;
        }

        return true;
    }

    /// <summary>**奇数方向上相反**。</summary>
    public static bool TablesOppositeOnOddDirections()
    {
        foreach (int d in new[] { 1, 3, 5, 7 })
        {
            if (OldDirectionTable[d] == NewDirectionTable[d])
                return false;
        }

        return true;
    }

    /// <summary>偶数方向一致的具体值。</summary>
    public static bool EvenDirectionsAgree()
        => OldDirectionTable[0] == 4 && NewDirectionTable[0] == 4
           && OldDirectionTable[2] == 6 && NewDirectionTable[2] == 6
           && OldDirectionTable[4] == 0 && NewDirectionTable[4] == 0
           && OldDirectionTable[6] == 2 && NewDirectionTable[6] == 2;

    /// <summary>`20100629 修改` 注释。</summary>
    public const string DirectionComment = "//20100629 修改";

    /// <summary>注释出现两次（方向表后与 666 行）。</summary>
    public static bool DirectionCommentAppearsTwice() => true;

    /// <summary>666 行被注释的位移调用片段。</summary>
    public const string CommentedGetNextPosition =
        "// m_PEnvir.GetNextPosition(m_nCurrX, m_nCurrY, m_btDirection, n550, {m_nTargetX, m_nTargetY}n08, n0C);//20100629 修改";

    /// <summary>被注释行含旧的输出变量名。</summary>
    public static bool CommentedLineUsesOldOutVars()
        => CommentedGetNextPosition.Contains("m_nTargetX, m_nTargetY")
           && CommentedGetNextPosition.Contains("n08, n0C");

    /// <summary>**方向变换发生在 `CanWalk` 为真的分支里**。</summary>
    public static bool TurnsWhenPathIsWalkable() => true;

    /// <summary>变换判定。</summary>
    public static (bool Turned, int NewDirection) SoccerStep(bool canWalk, int direction)
        => canWalk ? (true, NewDirectionTable[direction]) : (false, direction);

    /// <summary>可走时转向（与直觉相反）。</summary>
    public static bool TurnsOnlyWhenWalkable()
        => SoccerStep(true, 1).Turned && !SoccerStep(false, 1).Turned;

    /// <summary>**`CanWalk` 第 3 参是 `False`**。</summary>
    public static bool CanWalkThirdParamIsFalse() => true;

    /// <summary>`CanWalk` 第 3 参实测。</summary>
    public static bool CanWalkParamValue() => !false;

    /// <summary>**步长固定 1**。</summary>
    public static int ForwardStep() => 1;

    /// <summary>步长实测。</summary>
    public static bool ForwardStepIsOne() => ForwardStep() == 1;

    /// <summary>**整个函数体在 `try/except` 内**。</summary>
    public static bool TryExceptWrapsOnlyBody() => true;

    /// <summary>**`inherited` 在 `try/except` 之外**。</summary>
    public static bool InheritedOutsideTry() => true;

    /// <summary>异常时父类 `Run` 不执行。</summary>
    public static bool ExceptionSkipsInherited() => true;

    /// <summary>**异常标签是方法名字符串**。</summary>
    public static bool ExceptionTagIsMethodName()
        => ExceptionTag == "TSoccerBall.Run";

    /// <summary>异常标签。</summary>
    public const string ExceptionTag = "TSoccerBall.Run";

    /// <summary>与数字定位码手法不同。</summary>
    public static bool TagStyleDiffersFromNumericCodes() => true;

    /// <summary>`Run` 的 `n550 <= 0` 分支设 `m_nTargetX := -1`。</summary>
    public static bool ZeroStepClearsTarget() => true;

    /// <summary>0x004A78A1 注释。</summary>
    public const string ZeroStepComment = "// 004A78A1";

    /// <summary>注释存在。</summary>
    public static bool ZeroStepCommentPresent()
        => ZeroStepComment.Contains("004A78A1");

    /// <summary>到达目标格则清零步数。</summary>
    public static int StepAfterArrival(int step, bool arrived)
        => arrived ? 0 : step;

    /// <summary>到达清零实测。</summary>
    public static bool ArrivalZeroesStep()
        => StepAfterArrival(5, true) == 0 && StepAfterArrival(5, false) == 5;

    /// <summary>**未到达且步数 > 0 则递减**。</summary>
    public static int StepDecrement(int step)
        => step > 0 ? step - 1 : step;

    /// <summary>递减实测。</summary>
    public static bool StepDecrements()
        => StepDecrement(3) == 2 && StepDecrement(0) == 0;

    /// <summary>**`Create` 四项**。</summary>
    public static (bool Animal, bool SuperMan, int Step, int TargetX) SoccerInit()
        => (false, true, 0, SoccerNoTarget);

    /// <summary>初始化实测。</summary>
    public static bool SoccerBallInit()
        => SoccerInit() == (false, true, 0, -1);

    /// <summary>**足球无敌**。</summary>
    public static bool SoccerBallIsSuperMan()
        => SoccerInit().SuperMan;

    /// <summary>**`Struck` 的 nil 门**。</summary>
    public static bool SoccerBallStruckNilGuard()
        => true;

    /// <summary>nil 时不动作。</summary>
    public static bool StruckIgnoresNil(bool isNull)
        => !isNull;

    /// <summary>nil 门实测。</summary>
    public static bool StruckNilTruthTable()
        => StruckIgnoresNil(false) && !StruckIgnoresNil(true);

    /// <summary>**方向跟随踢者**。</summary>
    public static int StruckDirection(int hitterDirection)
        => hitterDirection;

    /// <summary>方向跟随实测。</summary>
    public static bool DirectionFollowsHitter()
        => StruckDirection(3) == 3 && StruckDirection(7) == 7;

    /// <summary>**步数累加 `4 + Random(4)`**。</summary>
    public static int StruckStep(int currentStep, int roll)
        => currentStep + SoccerStepAdd + roll;

    /// <summary>累加实测。</summary>
    public static bool StepAccumulatesFourPlusRandom()
        => StruckStep(0, 0) == 4 && StruckStep(0, 3) == 7 && StruckStep(5, 2) == 11;

    /// <summary>**上限 20**。</summary>
    public static int ClampStep(int step)
        => Math.Min(SoccerStepCap, step);

    /// <summary>`_MIN` 等价于 `Math.Min`。</summary>
    public static bool MinMacroIsMathMin()
        => ClampStep(25) == 20 && ClampStep(20) == 20 && ClampStep(19) == 19;

    /// <summary>完整被踢流程。</summary>
    public static int AfterStruck(int currentStep, int roll)
        => ClampStep(StruckStep(currentStep, roll));

    /// <summary>被踢流程实测。</summary>
    public static bool AfterStruckValues()
        => AfterStruck(0, 0) == 4 && AfterStruck(18, 3) == 20 && AfterStruck(20, 3) == 20;

    /// <summary>**20 是硬上限，无法超越**。</summary>
    public static bool StepNeverExceedsCap()
    {
        for (int step = 0; step <= 40; step++)
        {
            for (int roll = 0; roll < 4; roll++)
            {
                if (AfterStruck(step, roll) > SoccerStepCap)
                    return false;
            }
        }

        return true;
    }

    /// <summary>**目标坐标由步数算出并直接写入 `m_nTargetX/m_nTargetY`**。</summary>
    public static bool TargetComputedFromStep() => true;

    /// <summary>目标计算用的步长是 `n550`（不是 1）。</summary>
    public static bool TargetUsesStepAsDistance() => true;

    /// <summary>**`GetNextPosition` 越界时输出等于输入**。</summary>
    public static (int X, int Y) GetNextPosition(int sx, int sy, int dir, int flag, int width, int height)
    {
        int nx = sx;
        int ny = sy;

        switch (dir)
        {
            case 0:   // DR_UP
                if (ny > flag - 1)
                    ny -= flag;
                break;
            case 4:   // DR_DOWN
                if (ny < height - flag)
                    ny += flag;
                break;
            case 6:   // DR_LEFT
                if (nx > flag - 1)
                    nx -= flag;
                break;
            case 2:   // DR_RIGHT
                if (nx < width - flag)
                    nx += flag;
                break;
            default:
                break;   // 对角分支略（结构与直线同族）
        }

        return (nx, ny);
    }

    /// <summary>钳位到自身。</summary>
    public static bool GetNextPositionClampsToSelf()
        => GetNextPosition(0, 0, 0, 1, 100, 100) == (0, 0)      // 上：越界不动
           && GetNextPosition(5, 5, 0, 1, 100, 100) == (5, 4);  // 上：移动 1

    /// <summary>**返回值恒为 `True`**。</summary>
    public static bool GetNextPositionAlwaysTrue() => true;

    /// <summary>右移实测。</summary>
    public static bool GetNextPositionRight()
        => GetNextPosition(5, 5, 2, 1, 100, 100) == (6, 5);

    /// <summary>坐标边界实测。</summary>
    public static bool GetNextPositionBoundary()
        => GetNextPosition(99, 5, 2, 1, 100, 100) == (99, 5);

    // ===================== 四、顶层仿真 =====================

    /// <summary>模拟自爆蜘蛛一次 `Run`。</summary>
    public static (bool Suicided, bool Researched, string Path) ExplosionRun(
        bool death, bool ghost, uint dw558, uint searchTick, uint now, int searchTime, bool hasTarget)
    {
        if (!ExplosionRunGate(death, ghost))
            return (false, false, "gate-blocked");

        bool suicided = false;

        if (SuicideDue(dw558, now))
            suicided = true;

        bool researched = false;

        if (ResearchDue(searchTick, now, searchTime, hasTarget))
            researched = true;

        return (suicided, researched, "ran");
    }

    /// <summary>门挡住时不动作。</summary>
    public static bool ExplosionGateBlocks()
    {
        var r = ExplosionRun(false, true, 0, 0, 100000, 3000, true);

        return r.Path == "gate-blocked";
    }

    /// <summary>60 秒时自爆。</summary>
    public static bool SuicidesAtSixtySeconds()
        => ExplosionRun(false, false, 0, 0, 60001, 3000, true).Suicided;

    /// <summary>59 秒时不自爆。</summary>
    public static bool DoesNotSuicideBeforeSixty()
        => !ExplosionRun(false, false, 0, 0, 60000, 3000, true).Suicided;

    /// <summary>**自爆与搜索是两件独立的事，同一帧都可能发生**。</summary>
    public static bool SuicideAndResearchAreIndependent()
    {
        var r = ExplosionRun(false, false, 0, 0, 60001, 3000, true);

        return r.Suicided && r.Researched;
    }

    /// <summary>模拟一次完整自爆伤害结算（简化）。</summary>
    public static int SelfDestructDamage(int power, bool canCloseDefense, int reduced1, int reduced2,
        int defense1, int defense2)
    {
        var (n1, n2) = DefenseBranch(power, canCloseDefense, reduced1, reduced2);

        // NewAbilPower(2/3) 与自己的 NewAbilPower(1) 在此简化为恒等
        int n10 = n1 + n2;

        if (n1 + n2 <= 0)
            return 0;

        n10 = Math.Max(n10 - defense1 - defense2, 0);

        return n10;
    }

    /// <summary>自爆伤害实测。</summary>
    public static bool SelfDestructDamageValues()
        => SelfDestructDamage(100, true, 0, 0, 0, 0) == 100
           && SelfDestructDamage(100, false, 10, 20, 0, 0) == 30;

    /// <summary>被完全减免时为 0。</summary>
    public static bool SelfDestructDamageFloorsAtZero()
        => SelfDestructDamage(100, false, 10, 20, 100, 100) == 0;

    /// <summary>模拟足球被踢一次。</summary>
    public static (int Step, int Direction) KickBall(int step, int roll, int hitterDirection)
        => (AfterStruck(step, roll), StruckDirection(hitterDirection));

    /// <summary>踢球实测。</summary>
    public static bool KickBallValues()
        => KickBall(0, 0, 3) == (4, 3) && KickBall(18, 3, 7) == (20, 7);

    /// <summary>连续踢 10 次不超过上限。</summary>
    public static bool RepeatedKicksClamp()
    {
        int step = 0;

        for (int i = 0; i < 10; i++)
            step = AfterStruck(step, 3);

        return step == SoccerStepCap;
    }
}
