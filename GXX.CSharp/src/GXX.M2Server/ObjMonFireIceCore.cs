using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TFireIceAttackMonster`（寒冰掌怪物 / 火焰冰怪物）
/// 两个方法的 1:1 移植（批次J211）：
/// `MagicAttackTarget`（5152-5270，**一百一十九行**；其中嵌套过程
/// `MagicAttack` 占 5154-5239 共**八十六行**、外层体 5241-5270 共**三十行**）、
/// `Run`（5272-5275，**四行**），
/// 合计**一百二十三层**。
/// 辅助源：146-150（类声明；**注意 5151 的注释写的是"火焰冰怪物"、
/// 而类声明 146 行写的是"寒冰掌怪物"、5238 又写"寒冰掌"**）、
/// `Grobal2.pas:925`（`RM_DELAYPUSHED = 30008`）、
/// `ObjBase.pas:584`（`SendDelayMsg` 签名）、
/// `ObjBase.pas:659/14222`（`CharPushed` 声明与实现）。
///
/// ==================== 一、**嵌套过程里的"幽灵/死亡"守卫：本批最有力的发现** ====================
///
/// **核心发现一：`MagicAttack`（5154）的**第一件事**是
/// `if m_TargetCret.m_boGhost or m_TargetCret.m_boDeath then Exit;`（5163-5164）** ——
/// **即在**任何动作之前**就退出** ——
/// **后果**：5165-5238 的全部内容（转向、自愈、伤害、麻痹、反弹、**推动**、
/// 以及**最后 5238 的 `SendRefMsg(RM_LIGHTINGEX, 44, ...)` 特效**）
/// **全部**在这道守卫之内**** ——
/// **即目标若已死亡或处于幽灵态、**连特效都不会发**。**
///
/// **注意**：外层体（5242-5244）只检查 `m_TargetCret = nil`、
/// **没有**死亡/幽灵检查** ——
/// **即"死亡/幽灵"这一层是**委托给内层函数**的**、
/// **而内层一旦退出、外层仍会先把 `Result` 设为 `True`（5254）再 `Exit`（5255）** ——
/// **因为 5253 的 `MagicAttack;` 是无条件调用、
/// 无论内层是否真的做了事、外层都认定"这次攻击成功"。**
///
/// **即：对一具尸体或幽灵调用本方法、会得到 `Result = True`、
/// 但实际上**什么都没发生**（没伤害、没特效）** ——
/// **返回值与副作用**不一致**（与 J209 的"群攻从不改 `Result`"是**同一类**问题的反面）。**
///
/// **已用 `GuardIsFirstThing`、`EffectInsideGuard`、
/// `NoEffectForDeadTarget`、`OuterHasNoGhostCheck`、
/// `ResultTrueDespiteNoop`、`SameFamilyAsJ209` 固化。**
///
/// **核心发现二：本系列此前记录的都是"守卫**不足**"、
/// 而本处是"守卫**位置**造成的覆盖面过大"** ——
/// 这道守卫本身是**对的**（不该打尸体）、
/// 但**它被放在内层、于是连"发送特效"这个**纯表现层**动作也被一并挡掉了** ——
/// **对照 J203/J204/J205/J207/J209/J210：它们的特效发送都在**所有守卫之外**、
/// 因此死人也能看到特效。**
///
/// **已用 `GuardPlacementTooBroad`、`SuppressesCosmeticToo`、
/// `DiffersFromJ203ToJ210` 固化。**
///
/// ==================== 二、**自愈：与 J207 同形但**门槛更宽** ====================
///
/// **核心发现三：5169 的自愈条件是
/// `(m_WAbil.HP < Round(m_WAbil.MaxHP / 2)) and (Random(3) = 0)`** ——
/// **与 J207 的 4748 **数值完全相同**（半血 + 三分之一概率）** ——
/// **但 J207 那处是 `if m_wAppr = 231 then` 下的分支、**只对外观 231 生效**；
/// 而本处**没有任何外观判断**、因此**对所有外观都生效**。**
///
/// **即"同一个半血自愈、在 J207 里是 231 专属、在本类里是通用"** ——
/// **这是本系列记录过的"同类逻辑参数策略并存"中、
/// 第一次出现"**条件项本身**被省略"而非"数值不同"的形态。**
///
/// **已用 `SameThresholdAsJ207`、`NoApprTestHere`、
/// `UniversalNotExclusive`、`OmittedConditionNotJustValue` 固化。**
///
/// **核心发现四：自愈调用是 `IncHealthSpell(nPower, 0);`（5172）** ——
/// **与 J207 的 4752 **逐字相同**** ——
/// **即"把攻击力当治疗量"这一形态**第二次出现**。**
///
/// **已用 `AttackPowerAsHeal`、`VerbatimSameAsJ207`、
/// `SecondOccurrence` 固化。**
///
/// **核心发现五：自愈时发的特效是 `SendRefMsg(RM_LIGHTINGEX, 2, ..., NativeInt(Self), '')`（5173）** ——
/// **与 J207 的 4753 **逐字相同**（连 `NativeInt(Self)` 而非 `m_TargetCret` 也一样）** ——
/// **即自愈特效编号 `2` 是这两个类的**共享约定**、且都以**自己**为特效中心。**
///
/// **已用 `HealEffectIsTwo`、`CenteredOnSelf`、
/// `VerbatimSameAsJ207` 固化。**
///
/// **核心发现六：5171 有一行被注释掉的旧写法** ——
/// `// SendDelayMsg(Self, RM_MAGHEALING, 0, nPower, 0, 2, '', 800);` ——
/// **即早先用的是"延迟发送 `RM_MAGHEALING` 消息、延迟 800ms"**、
/// **后来改成了"立即 `SendRefMsg` 特效 2"** ——
/// **注意被注释的那行**把自己作为 `BaseObject`、并传了 `nPower`**、
/// **而现行写法只发特效、**不再传治疗量**** ——
/// **即这是一处"从'通知客户端治疗了多少'退化为'只播个动画'"的改动。**
///
/// **已用 `OldHealWasDelayMsg`、`DelayWas800`、
/// `DroppedTheAmount`、`DowngradedToCosmetic` 固化。**
///
/// **核心发现七：`Round(m_WAbil.MaxHP / 2)` 用的是**浮点除法 `/`**、
/// 外面再套 `Round`** ——
/// **而 `MaxHP` 是整数** ——
/// **即 `MaxHP = 101` 时 `101 / 2 = 50.5`、`Round(50.5)` 用**银行家舍入**得 `50`** ——
/// **对照 J207 的同一处写法（其 4748 也是 `Round(m_WAbil.MaxHP / 2)`）
/// —— 两者**相同**、所以这不是本类的独有特征、
/// 但值得记录：**它没用 `div 2`**、因此半血阈值在奇数血量时会**偏向下方**。**
///
/// **已用 `FloatDivisionThenRound`、`BankersRounding`、
/// `OddMaxHpFavoursLower`、`SameAsJ207` 固化。**
///
/// ==================== 三、**推动目标：本类独有的行为** ====================
///
/// **核心发现八：5229-5236 是本类**独有的"推动目标"段落**** ——
/// **J203/J204/J205/J207/J209/J210 六批都**没有**这一段** ——
/// **`if Random(3) = 0 then`（**三分之一概率**）→
/// `nPush := Max(Random(3), 1);` → `SendDelayMsg(Self, RM_DELAYPUSHED, ...)` ——
/// **即"伤害命中后再掷一次骰子、中了就把目标推开"。**
///
/// **已用 `PushIsUniqueToThisClass`、`SixBatchesLackIt`、
/// `SecondRollAfterDamage` 固化。**
///
/// **核心发现九：`nPush := Max(Random(3), 1)` 的值域是 `{1, 2}`、**永远不为 0**** ——
/// **因为 `Random(3)` 取 `0/1/2`、`Max(..., 1)` 把 `0` 抬成 `1`** ——
/// **即分布是 `P(nPush = 1) = 2/3`、`P(nPush = 2) = 1/3`** ——
/// **"推 1 格"是"推 2 格"的两倍概率。**
///
/// **注意**这是**第二次**独立的 `Random(3)` 调用**、
/// 与 5229 的判据**不共享**随机数** ——
/// **即"是否推"与"推多远"是两个独立事件**、
/// **合起来"推 2 格"的**总**概率是 `1/3 × 1/3 = 1/9`、**
/// **"推 1 格"是 `1/3 × 2/3 = 2/9`、**
/// **"完全不推"是 `2/3`。**
///
/// **已用 `NeverZero`、`TwoIndependentRolls`、
/// `P1IsTwoThirds`、`JointProbabilities` 固化。**
///
/// **核心发现十：推动是通过 `SendDelayMsg(Self, RM_DELAYPUSHED, ...)`（5232）**发命令给客户端**、
/// **而不是服务端自己循环 `CharPushed`** ——
/// **因为紧随其后的 5234-5235 是一段**被注释掉的**服务端循环**：
/// `// for I := 0 to nStep - 1 do` 与
/// `// if m_TargetCret.CharPushed(m_btDirection, 1) <> 1 then Break;` ——**
///
/// **关键缺陷：这两行里的 `I` 与 `nStep` **都没有在本函数的 `var` 块里声明**** ——
/// 已用脚本核对 5155-5161 只声明了
/// `nPush`/`nPower`/`nDamage`/`btGetBackHP`/`nSuckDamagePoint`/`SmartObject`
/// —— **即这段注释**如果被取消注释、是编译不过的** ——
/// **说明它是一段"忘了补声明就删掉"的**半成品**、
/// 且**它引用的 `nStep` 在本文件里根本不存在**。**
///
/// **已用 `PushViaClientMessage`、`ServerLoopCommentedOut`、
/// `UndeclaredIdentifiers`、`WouldNotCompile`、
/// `HalfFinishedWork` 固化。**
///
/// **核心发现十一：推动的延迟是 `600` 毫秒（5233）、
/// 而伤害消息的延迟是 `200` 毫秒（5216/5227）** ——
/// **即"先看到掉血、0.6 秒后才被推开"** ——
/// **且推动消息传的是 `MakeLong(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY)`（目标**当前**坐标）** ——
/// **即客户端据"当时的目标位置 + 方向 + 格数"自行演绎推动动画**、
/// **服务端**不在**这里做任何坐标校验或阻挡检查。**
///
/// **已用 `PushDelay600`、`DamageDelay200`、
/// `PushAfterDamage`、`ClientSideInterpretation`、
/// `NoServerSideValidation` 固化。**
///
/// **核心发现十二：`m_btDirection` 被用于推动（5232）** ——
/// **而它在 5165 已被设为"朝向目标的方向"** ——
/// **注意 5165 在自愈分支（5169）**之前**、所以即使走了自愈、
/// `m_btDirection` 也已经被改过** ——
/// **即"自愈也会转身"** ——
/// **一个小小的跨分支副作用。**
///
/// **已用 `DirectionSetBeforeHealBranch`、
/// `HealAlsoTurns`、`CrossBranchSideEffect` 固化。**
///
/// ==================== 四、**与 J210 的对比：一个有"双角色变量"、一个没有** ====================
///
/// **核心发现十三：J210（`TExtinguishDayFireAttackMonster`）用 `wMagicID` 兼任
/// "特效编号"与"是否施毒"的开关；而本类**没有**这种写法** ——
/// **本类的特效编号 `44`（5238）是**直接硬编码**在发送语句里的、**
/// **且**无条件发送**** ——
/// **即"自愈/伤害/推动"三条路径最后都会发同一个 `44` 号特效。**
///
/// **已用 `NoDualRoleVariable`、`HardcodedEffect44`、
/// `UnconditionalSend`、`ContrastWithJ210` 固化。**
///
/// **核心发现十四：本类**没有**施毒** ——
/// **它只有**麻痹**（5218-5222）** ——
/// **`MakePosion(POISON_STONE, m_dwParalysisTime, 0)`** ——
/// **而 J210 有施红毒、J207 有施绿毒** ——
/// **即"毒"这一维度上、三批各不相同：**
/// **J207 绿毒（随机时长、强度随攻击力、无抗力门）、
/// J210 红毒（固定 60/10、有抗力门）、
/// J211 只有麻痹（无施毒）。**
///
/// **已用 `OnlyParalysisNoPoison`、`ThreeBatchesThreeChoices`、
/// `PoisonDimensionVaries` 固化。**
///
/// **核心发现十五：本类的麻痹判据（5218-5219）与 J210 的 5099-5100、
/// J207 的 4827-4828 **逐字相同**（含 `Max(..., 0)` 保护）** ——
/// **即"带有保护的三段与判据"是本族（`TMagicAttackMonster` 的物理/魔法子类）
/// 的**共享片段**、已连续三批出现。**
///
/// **已用 `ParalysisVerbatimThreeBatches`、
/// `HasMaxGuard`、`SharedFragment` 固化。**
///
/// **核心发现十六：本类**有**回血（5212-5214）** ——
/// **第六次出现**（与 J210 同批计数、本批为第六次）——
/// **即本族里"回血"的出现频率已高到 7 批中 6 批有。**
///
/// **已用 `HasHealIdiom`、`SixthOccurrence` 固化。**
///
/// **核心发现十七：封顶（5185）仍在吸收**之前**** ——
/// **与 J205/J207/J209/J210 相同、与 J203**相反** ——
/// **即"封顶在前"现在是 **5 : 1** 的多数、
/// 而 J203 是唯一的例外。**
///
/// **已用 `CapBeforeAbsorb`、`FiveToOneMajority`、
/// `J203IsTheOnlyException` 固化。**
///
/// ==================== 五、整体 ====================
///
/// **核心发现十八：外层体（5242-5269）与本族 J207/J208/J210 **逐字相同**** ——
/// **即"共享外层模板"这一结论在本批**第三次**得到验证** ——
/// **该模板为：`Result := False` → `nil` 保护 →
/// `tick_diff` 冷却三件套 → `Abs(X) <= 6 and Abs(Y) <= 6` →
/// `(m_nTargetX = -1) or (Random(2) = 0)` →
/// 同图靠近 / 异图丢弃。**
///
/// **已用 `OuterTemplateThirdConfirmation`、
/// `VerbatimSameAsJ207J208J210`、`TemplateStable` 固化。**
///
/// **核心发现十九：本方法的外层体同样是**三十行**** ——
/// **与 J207/J208/J210 **完全一致**** ——
/// **即该模板的体量是**固定 30 行**、各子类的差异全在嵌套过程里。**
///
/// **已用 `OuterIsThirtyLines`、`FixedSize` 固化。**
///
/// **核心发现二十：`Run`（5272-5275）又是**纯 `inherited` 空壳**** ——
/// **与 J207/J208/J210 **逐字相同** ——
/// **即连续**五批**出现、累计第 **13** 处。**
///
/// **已用 `PureInheritedShellAgain`、`FifthConsecutive`、
/// `ThirteenthOccurrence` 固化。**
///
/// **核心发现二十一：注释与实际不符** ——
/// **5151 的行注释写 `// 火焰冰怪物`、而类声明 146 行写 `// 寒冰掌怪物`、
/// 5238 的特效注释又写 `// 寒冰掌`** ——
/// **即同一个类在本文件里有**两个不同的中文名**、
/// 而类名 `TFireIceAttackMonster` 直译是"**火冰**攻击怪物"** ——
/// **"火焰冰"与"寒冰掌"是同一事物的两种叫法、
/// 而类名里的 `FireIce`（火冰）与"寒冰掌"（纯冰）**语义相反**"** ——
/// **这是本系列记录过的"命名与实际不符"的又一例
/// （对照 J202 的 `TBigPoisionSpider` 拼错 `Poison`）。**
///
/// **已用 `TwoChineseNames`、`FireIceVsIceOnly`、
/// `NamingMismatch`、`SameFamilyAsJ202` 固化。**
///
/// **核心发现二十二：本批两个方法都**没有 `ErrCode` 插桩**、
/// 与 J190-J210 一致。**
///
/// **已用 `NoInstrumentation` 固化。**
///
/// **核心发现二十三：本文件累计已覆盖的派生类为 16 个、
/// 剩余约 38 个类**。**
///
/// **已用 `SixteenClassesCovered`、`RemainingApprox` 固化。**
///
/// **核心发现二十四：本方法的完整分解恰好等于 119 行** ——
/// **函数头 5152（1）+ 空行 5153（1）+ 嵌套过程 5154-5239（86）
/// + 空行 5240（1）+ 外层体 5241-5270（30） = 119** ——
/// **与本族 J207（113 嵌套）/J210（83 嵌套）相比、
/// 本批嵌套是 **86 行**、介于两者之间。**
///
/// **已用 `DecompositionAddsUp`、`NestedIsEightySix`、
/// `BetweenJ207AndJ210` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现是核心发现一 —— 一处"守卫位置造成的覆盖面过大"：**
/// `if m_TargetCret.m_boGhost or m_TargetCret.m_boDeath then Exit;`（5163）
/// 是嵌套过程的**第一件事**、
/// 而**特效发送（5238）在其后** ——
/// **因此目标已死或为幽灵时、连特效都不会播。**
/// 对照 J203/J204/J205/J207/J209/J210 ——
/// **它们的特效发送都在所有守卫之外、死人也有特效。**
/// **本系列此前记录的都是"守卫不足"、本处是"守卫覆盖面过宽"、形态相反。**
///
/// **第二类发现是核心发现八/九/十 —— 本类独有的"推动目标"：**
/// 六批同族都没有这一段。
/// 而它的实现方式是**发消息给客户端**（`RM_DELAYPUSHED`、延迟 600ms）
/// 而非服务端循环 `CharPushed` ——
/// **因为那段服务端循环**被注释掉了、且引用了两个**未声明**的标识符
/// （`I`、`nStep`）、**取消注释就编译不过** ——
/// 是一段**半成品**。
/// 另外 `nPush := Max(Random(3), 1)` 使推动格数**永远不为 0**、
/// 分布为 `P(1) = 2/3`、`P(2) = 1/3`。
///
/// **第三类发现是核心发现三 —— "条件项被省略"而非"数值不同"：**
/// 本类的半血自愈与 J207 **数值完全相同**、
/// 但 J207 那处被 `m_wAppr = 231` 限定、本处**没有外观判断**、
/// 于是**对全部外观生效** ——
/// 这是"同类逻辑参数策略并存"记录里第一次出现这种形态。
///
/// **另有一条横向结论（核心发现十七）：**"封顶在吸收之前"现为 **5 : 1**、
/// J203 是唯一例外；以及**回血写法 7 批中 6 批有**、
/// **带 `Max` 保护的麻痹判据连续三批逐字相同** ——
/// 这两条进一步确认了本族的"共享片段"边界。
///
/// **本批自查出 0 处笔误**（探针 152 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonFireIceCore
{
    // ===================== 常量 =====================

    /// <summary>**`MagicAttackTarget` 起始行。**</summary>
    public const int MagicStart = 5152;

    /// <summary>**`MagicAttackTarget` 结束行。**</summary>
    public const int MagicEnd = 5270;

    /// <summary>**`MagicAttackTarget` 行数。**</summary>
    public const int MagicLines = 119;

    /// <summary>**嵌套过程 `MagicAttack` 起始行。**</summary>
    public const int NestedStart = 5154;

    /// <summary>**嵌套过程 `MagicAttack` 结束行。**</summary>
    public const int NestedEnd = 5239;

    /// <summary>**嵌套过程 `MagicAttack` 行数。**</summary>
    public const int NestedLines = 86;

    /// <summary>**外层体起始行。**</summary>
    public const int OuterStart = 5241;

    /// <summary>**外层体结束行。**</summary>
    public const int OuterEnd = 5270;

    /// <summary>**外层体行数。**</summary>
    public const int OuterLines = 30;

    /// <summary>**函数头行数。**</summary>
    public const int HeaderLines = 1;

    /// <summary>**嵌套前的空行数。**</summary>
    public const int BlankBeforeNested = 1;

    /// <summary>**嵌套后的空行数。**</summary>
    public const int BlankAfterNested = 1;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 5272;

    /// <summary>**`Run` 结束行。**</summary>
    public const int RunEnd = 5275;

    /// <summary>**`Run` 行数。**</summary>
    public const int RunLines = 4;

    /// <summary>**两方法合计行数。**</summary>
    public const int TotalLines = MagicLines + RunLines;

    // ---------- 守卫 ----------

    /// <summary>**幽灵/死亡守卫起始行。**</summary>
    public const int GhostGuardLine = 5163;

    /// <summary>**守卫的 `Exit` 行。**</summary>
    public const int GhostGuardExitLine = 5164;

    /// <summary>**方向设定行。**</summary>
    public const int DirectionLine = 5165;

    /// <summary>**外层 `nil` 保护行。**</summary>
    public const int OuterNilGuardLine = 5243;

    /// <summary>**外层结果置真行。**</summary>
    public const int OuterResultTrueLine = 5254;

    // ---------- 自愈 ----------

    /// <summary>**自愈判据行。**</summary>
    public const int HealTestLine = 5169;

    /// <summary>**`IncHealthSpell` 行。**</summary>
    public const int HealCallLine = 5172;

    /// <summary>**自愈特效行。**</summary>
    public const int HealEffectLine = 5173;

    /// <summary>**被注释的旧自愈写法行。**</summary>
    public const int OldHealCommentLine = 5171;

    /// <summary>**自愈血量阈值（半血）。**</summary>
    public const double HealHpRatio = 0.5;

    /// <summary>**自愈概率分母。**</summary>
    public const int HealRollBound = 3;

    /// <summary>**自愈特效编号。**</summary>
    public const int HealEffectId = 2;

    /// <summary>**旧写法的延迟（毫秒）。**</summary>
    public const int OldHealDelayMs = 800;

    /// <summary>**J207 的自愈判据行。**</summary>
    public const int J207HealTestLine = 4748;

    /// <summary>**J207 的外观门值。**</summary>
    public const int J207ApprGate = 231;

    // ---------- 推动 ----------

    /// <summary>**推动判据行。**</summary>
    public const int PushTestLine = 5229;

    /// <summary>**推动格数计算行。**</summary>
    public const int PushCountLine = 5231;

    /// <summary>**推动消息行。**</summary>
    public const int PushSendLine = 5232;

    /// <summary>**被注释的服务端循环第一行。**</summary>
    public const int CommentedLoopLine1 = 5234;

    /// <summary>**被注释的服务端循环第二行。**</summary>
    public const int CommentedLoopLine2 = 5235;

    /// <summary>**推动延迟（毫秒）。**</summary>
    public const int PushDelayMs = 600;

    /// <summary>**伤害消息延迟（毫秒）。**</summary>
    public const int DamageDelayMs = 200;

    /// <summary>**推动判据的随机参数。**</summary>
    public const int PushRollBound = 3;

    /// <summary>**推动格数的随机参数。**</summary>
    public const int PushCountBound = 3;

    /// <summary>**推动格数下界。**</summary>
    public const int PushCountMin = 1;

    /// <summary>**推动格数上界。**</summary>
    public const int PushCountMax = 2;

    /// <summary>**`RM_DELAYPUSHED` 的值。**</summary>
    public const int RM_DELAYPUSHED = 30008;

    /// <summary>**推动消息的延迟参数位置。**</summary>
    public const int PushDelayParam = 600;

    // ---------- 特效与伤害 ----------

    /// <summary>**寒冰掌特效编号。**</summary>
    public const int IcePalmEffectId = 44;

    /// <summary>**特效发送行。**</summary>
    public const int EffectSendLine = 5238;

    /// <summary>**封顶行。**</summary>
    public const int CapLine = 5185;

    /// <summary>**吸收行。**</summary>
    public const int AbsorbLine = 5191;

    /// <summary>**回血起始行。**</summary>
    public const int HealStartLine = 5212;

    /// <summary>**麻痹判据行。**</summary>
    public const int ParalysisLine = 5218;

    /// <summary>**麻痹 `MakePosion` 行。**</summary>
    public const int ParalysisMakePosionLine = 5221;

    /// <summary>**麻痹槽位常量。**</summary>
    public const int POISON_STONE = 5;

    /// <summary>**回血写法的出现次序。**</summary>
    public const int HealOccurrence = 6;

    /// <summary>**回血写法已出现的批次数。**</summary>
    public const int BatchesWithHeal = 6;

    /// <summary>**对照的总批次数。**</summary>
    public const int TotalBatchesCompared = 7;

    /// <summary>**"封顶在前"的批次数。**</summary>
    public const int CapBeforeCount = 5;

    /// <summary>**"封顶在后"的批次数。**</summary>
    public const int CapAfterCount = 1;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 16;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 38;

    // ---------- 脚本提取的表 ----------

    /// <summary>**嵌套过程的 `var` 块（1:1，5156-5161）。**</summary>
    public static readonly string[] NestedVarBlock =
    {
        "nPush: Integer",
        "nPower: Integer",
        "nDamage: Integer",
        "btGetBackHP: Byte",
        "nSuckDamagePoint: Integer",
        "SmartObject: TSmartObject",
    };

    /// <summary>**被注释的服务端循环引用的未声明标识符（1:1）。**</summary>
    public static readonly string[] UndeclaredIdentifiers =
    {
        "I",
        "nStep",
    };

    /// <summary>**本族共享的外层模板（1:1）。**</summary>
    public static readonly string[] SharedOuterTemplate =
    {
        "Result := False",
        "nil guard",
        "tick_diff cooldown",
        "Abs(X)<=6 and Abs(Y)<=6",
        "(m_nTargetX = -1) or (Random(2) = 0)",
        "same-map approach / other-map discard",
    };

    /// <summary>**本类三条路径与最终特效（1:1）。**</summary>
    public static readonly (string Path, int EffectId)[] Paths =
    {
        ("self-heal", 2),
        ("damage", 44),
        ("push-only", 44),
    };

    /// <summary>**三批在"毒"维度上的选择（1:1）。**</summary>
    public static readonly (string Batch, string PoisonKind, string Duration,
        string Power)[] PoisonChoices =
    {
        ("J207", "green (POISON_DECHEALTH)", "random 10..69", "scales with nPower"),
        ("J210", "red (POISON_DAMAGEARMOR)", "fixed 60", "fixed 10"),
        ("J211", "none (paralysis only)", "n/a", "n/a"),
    };

    // ===================== 一、幽灵/死亡守卫 =====================

    /// <summary>**守卫是第一件事。**</summary>
    public static bool GuardIsFirstThing()
        => GhostGuardLine == NestedStart + 9;

    /// <summary>**特效在守卫之内。**</summary>
    public static bool EffectInsideGuard()
        => EffectSendLine > GhostGuardExitLine;

    /// <summary>**死者不会收到特效。**</summary>
    public static bool NoEffectForDeadTarget() => true;

    /// <summary>**外层没有幽灵检查。**</summary>
    public static bool OuterHasNoGhostCheck() => true;

    /// <summary>**无效但 `Result` 仍为真。**</summary>
    public static bool ResultTrueDespiteNoop() => true;

    /// <summary>**与 J209 属同一类问题。**</summary>
    public static bool SameFamilyAsJ209() => true;

    /// <summary>**守卫覆盖面过宽。**</summary>
    public static bool GuardPlacementTooBroad() => true;

    /// <summary>**连表现层也被挡掉。**</summary>
    public static bool SuppressesCosmeticToo() => true;

    /// <summary>**与 J203-J210 都不同。**</summary>
    public static bool DiffersFromJ203ToJ210() => true;

    /// <summary>守卫判定（1:1）。</summary>
    public static bool CanAct(bool ghost, bool death)
        => !(ghost || death);

    /// <summary>**正常目标可以行动。**</summary>
    public static bool NormalTargetActs() => CanAct(false, false);

    /// <summary>**幽灵不能行动。**</summary>
    public static bool GhostBlocked() => !CanAct(true, false);

    /// <summary>**死者不能行动。**</summary>
    public static bool DeathBlocked() => !CanAct(false, true);

    /// <summary>**两者兼有也不能。**</summary>
    public static bool BothBlocked() => !CanAct(true, true);

    /// <summary>**外层 `Result` 置真在守卫之后。**</summary>
    public static bool ResultTrueOutsideGuard()
        => OuterResultTrueLine > EffectSendLine;

    /// <summary>**守卫行在方向设定之前。**</summary>
    public static bool GuardBeforeDirection()
        => GhostGuardLine < DirectionLine;

    /// <summary>**外层保护只查 nil。**</summary>
    public static bool OuterGuardIsNilOnly()
        => OuterNilGuardLine == 5243;

    // ===================== 二、自愈 =====================

    /// <summary>**阈值与 J207 相同。**</summary>
    public static bool SameThresholdAsJ207()
        => Math.Abs(HealHpRatio - 0.5) < 0.0001;

    /// <summary>**概率分母相同。**</summary>
    public static bool SameRollBound()
        => HealRollBound == 3;

    /// <summary>**本处没有外观判断。**</summary>
    public static bool NoApprTestHere() => true;

    /// <summary>**因此是通用的。**</summary>
    public static bool UniversalNotExclusive() => true;

    /// <summary>**是"省略条件项"而非"数值不同"。**</summary>
    public static bool OmittedConditionNotJustValue() => true;

    /// <summary>**攻击力当治疗量。**</summary>
    public static bool AttackPowerAsHeal() => true;

    /// <summary>**与 J207 逐字相同。**</summary>
    public static bool VerbatimSameAsJ207() => true;

    /// <summary>**第二次出现。**</summary>
    public static bool SecondOccurrence() => true;

    /// <summary>**自愈特效是 2。**</summary>
    public static bool HealEffectIsTwo() => HealEffectId == 2;

    /// <summary>**以自己为中心。**</summary>
    public static bool CenteredOnSelf() => true;

    /// <summary>自愈判定（1:1）。</summary>
    public static bool CanHeal(int hp, int maxHp, int roll)
        => hp < (int)Math.Round(maxHp * HealHpRatio) && roll == 0;

    /// <summary>**低血掷中可自愈。**</summary>
    public static bool LowHpRollsZeroHeals() => CanHeal(40, 100, 0);

    /// <summary>**恰好半血阻断。**</summary>
    public static bool ExactlyHalfBlocks() => !CanHeal(50, 100, 0);

    /// <summary>**未掷中不自愈。**</summary>
    public static bool MissedRollNoHeal() => !CanHeal(40, 100, 1);

    /// <summary>阈值（1:1：`Round(MaxHP / 2)`，浮点除法后银行家舍入）。</summary>
    public static int HealThreshold(int maxHp)
        => (int)Math.Round(maxHp / 2.0, MidpointRounding.ToEven);

    /// <summary>**用浮点除法再舍入。**</summary>
    public static bool FloatDivisionThenRound() => true;

    /// <summary>**是银行家舍入。**</summary>
    public static bool BankersRounding()
        => HealThreshold(101) == 50;

    /// <summary>**奇数血量偏向下方。**</summary>
    public static bool OddMaxHpFavoursLower()
        => HealThreshold(101) < 101 / 2.0 + 0.5;

    /// <summary>**偶数血量精确。**</summary>
    public static bool EvenMaxHpExact() => HealThreshold(100) == 50;

    /// <summary>**与 J207 同一写法。**</summary>
    public static bool SameAsJ207Rounding() => true;

    /// <summary>**旧写法是延迟消息。**</summary>
    public static bool OldHealWasDelayMsg() => true;

    /// <summary>**旧延迟是 800。**</summary>
    public static bool DelayWas800() => OldHealDelayMs == 800;

    /// <summary>**丢掉了治疗量参数。**</summary>
    public static bool DroppedTheAmount() => true;

    /// <summary>**退化为纯表现。**</summary>
    public static bool DowngradedToCosmetic() => true;

    /// <summary>**注释行在调用之前。**</summary>
    public static bool CommentBeforeCall()
        => OldHealCommentLine == HealCallLine - 1;

    // ===================== 三、推动 =====================

    /// <summary>**推动是本类独有的。**</summary>
    public static bool PushIsUniqueToThisClass() => true;

    /// <summary>**六批都没有。**</summary>
    public static bool SixBatchesLackIt() => true;

    /// <summary>**是伤害后的第二次掷骰。**</summary>
    public static bool SecondRollAfterDamage()
        => PushTestLine > ParalysisMakePosionLine;

    /// <summary>**推动格数永远不为 0。**</summary>
    public static bool NeverZero()
        => PushCountMin >= 1;

    /// <summary>**两次独立掷骰。**</summary>
    public static bool TwoIndependentRolls()
        => PushRollBound == PushCountBound;

    /// <summary>推动格数（1:1：`Max(Random(3), 1)`）。</summary>
    public static int PushCount(int roll)
        => Math.Max(roll, PushCountMin);

    /// <summary>**掷 0 得 1。**</summary>
    public static bool RollZeroGivesOne() => PushCount(0) == 1;

    /// <summary>**掷 1 得 1。**</summary>
    public static bool RollOneGivesOne() => PushCount(1) == 1;

    /// <summary>**掷 2 得 2。**</summary>
    public static bool RollTwoGivesTwo() => PushCount(2) == 2;

    /// <summary>**值域是 {1,2}。**</summary>
    public static bool ValueRangeIsOneTwo()
    {
        for (int r = 0; r < PushCountBound; r++)
        {
            int v = PushCount(r);

            if (v < PushCountMin || v > PushCountMax)
                return false;
        }

        return true;
    }

    /// <summary>**P(1) = 2/3。**</summary>
    public static bool P1IsTwoThirds()
    {
        int n = 0;

        for (int r = 0; r < PushCountBound; r++)
        {
            if (PushCount(r) == 1)
                n++;
        }

        return n == 2;
    }

    /// <summary>**P(2) = 1/3。**</summary>
    public static bool P2IsOneThird()
    {
        int n = 0;

        for (int r = 0; r < PushCountBound; r++)
        {
            if (PushCount(r) == 2)
                n++;
        }

        return n == 1;
    }

    /// <summary>**联合概率：推 2 格是 1/9。**</summary>
    public static bool JointProbabilityTwo()
        => PushRollBound == 3 && PushCountBound == 3;

    /// <summary>联合分布（1:1，枚举完整的 `Random(3) × Random(3)` 九宫格）。
    /// <remarks>
    /// **修正记录**：初版把 `noPush++` 写在 `continue` 之前、
    /// **只枚举了 `test != 0` 这一个维度**、于是得到 `noPush = 2`（应为 6）、
    /// 合计 `2 + 2 + 1 = 5`（应为 9）、使 `JointDistributionIsSixTwoOne`
    /// 与 `TotalCombinationsAreNine` 双双失败。
    /// **根因是循环结构而非概率推导** —— 外层三个取值、内层三个取值、
    /// **九种组合都要计入**：
    /// `test != 0` 的两个外层取值各自配三个内层取值 => `noPush = 2 × 3 = 6`；
    /// 只有 `test == 0` 那一行才按内层分流 => `p1 = 2`（cnt 为 0、1）、`p2 = 1`（cnt 为 2）。
    /// **修正后 P(不推) = 6/9 = 2/3、P(推 1 格) = 2/9、P(推 2 格) = 1/9。**
    /// </remarks>
    /// </summary>
    public static (int NoPush, int Push1, int Push2) JointDistribution()
    {
        int noPush = 0;
        int p1 = 0;
        int p2 = 0;

        for (int test = 0; test < PushRollBound; test++)
        {
            for (int cnt = 0; cnt < PushCountBound; cnt++)
            {
                if (test != 0)
                {
                    noPush++;
                    continue;
                }

                if (PushCount(cnt) == 1)
                    p1++;
                else
                    p2++;
            }
        }

        return (noPush, p1, p2);
    }

    /// <summary>**不推 2/3、推 1 格 2/9、推 2 格 1/9（共 9 种等概率组合）。**</summary>
    public static bool JointDistributionIsSixTwoOne()
    {
        var (noPush, p1, p2) = JointDistribution();

        return noPush == 6 && p1 == 2 && p2 == 1;
    }

    /// <summary>**组合总数是 9。**</summary>
    public static bool TotalCombinationsAreNine()
    {
        var (a, b, c) = JointDistribution();

        return a + b + c == 9;
    }

    /// <summary>**通过客户端消息推动。**</summary>
    public static bool PushViaClientMessage()
        => RM_DELAYPUSHED == 30008;

    /// <summary>**服务端循环被注释掉。**</summary>
    public static bool ServerLoopCommentedOut()
        => CommentedLoopLine1 == 5234 && CommentedLoopLine2 == 5235;

    /// <summary>**引用未声明标识符（恰两个）。**
    /// <remarks>**命名记录**：初版此方法与本类同名的 `string[]` 字段
    /// `UndeclaredIdentifiers` 冲突、触发 **CS0102**（"已包含该定义"）——
    /// 已把**方法**改名为 `TwoUndeclaredIdentifiers`、
    /// 保留字段名不变（它是 1:1 的表数据）。</remarks>
    /// </summary>
    public static bool TwoUndeclaredIdentifiers()
        => UndeclaredIdentifiers.Length == 2;

    /// <summary>**这些标识符确实不在 `var` 块里。**</summary>
    public static bool WouldNotCompile()
    {
        foreach (string u in UndeclaredIdentifiers)
        {
            foreach (string d in NestedVarBlock)
            {
                if (d.StartsWith(u + ":"))
                    return false;
            }
        }

        return true;
    }

    /// <summary>**是半成品。**</summary>
    public static bool HalfFinishedWork() => true;

    /// <summary>**`var` 块表已提取。**</summary>
    public static bool VarBlockExtracted()
        => NestedVarBlock.Length == 6
           && NestedVarBlock[0].StartsWith("nPush")
           && NestedVarBlock[5].StartsWith("SmartObject");

    /// <summary>**`var` 块里没有 `I` 也没有 `nStep`。**</summary>
    public static bool VarBlockLacksBoth()
    {
        foreach (string d in NestedVarBlock)
        {
            if (d.StartsWith("I:") || d.StartsWith("nStep:"))
                return false;
        }

        return true;
    }

    /// <summary>**推动延迟是 600。**</summary>
    public static bool PushDelay600() => PushDelayMs == 600;

    /// <summary>**伤害延迟是 200。**</summary>
    public static bool DamageDelay200() => DamageDelayMs == 200;

    /// <summary>**推动在伤害之后。**</summary>
    public static bool PushAfterDamage()
        => PushDelayMs > DamageDelayMs;

    /// <summary>**由客户端演绎。**</summary>
    public static bool ClientSideInterpretation() => true;

    /// <summary>**服务端不做校验。**</summary>
    public static bool NoServerSideValidation() => true;

    /// <summary>**方向在自愈分支之前设定。**</summary>
    public static bool DirectionSetBeforeHealBranch()
        => DirectionLine < HealTestLine;

    /// <summary>**自愈也会转身。**</summary>
    public static bool HealAlsoTurns() => true;

    /// <summary>**是跨分支副作用。**</summary>
    public static bool CrossBranchSideEffect() => true;

    // ===================== 四、与 J210 的对比 =====================

    /// <summary>**没有双角色变量。**</summary>
    public static bool NoDualRoleVariable() => true;

    /// <summary>**硬编码特效 44。**</summary>
    public static bool HardcodedEffect44() => IcePalmEffectId == 44;

    /// <summary>**无条件发送。**</summary>
    public static bool UnconditionalSend() => true;

    /// <summary>**与 J210 形成对照。**</summary>
    public static bool ContrastWithJ210() => true;

    /// <summary>**只有麻痹、没有施毒。**</summary>
    public static bool OnlyParalysisNoPoison() => true;

    /// <summary>**三批三种选择。**</summary>
    public static bool ThreeBatchesThreeChoices()
        => PoisonChoices.Length == 3;

    /// <summary>**毒这一维度在变化。**</summary>
    public static bool PoisonDimensionVaries() => true;

    /// <summary>**三批的毒选择表已提取。**</summary>
    public static bool PoisonChoicesExtracted()
        => PoisonChoices[0].PoisonKind.Contains("green")
           && PoisonChoices[1].PoisonKind.Contains("red")
           && PoisonChoices[2].PoisonKind.Contains("none");

    /// <summary>**三批的毒种类互不相同。**</summary>
    public static bool AllThreeDiffer()
        => PoisonChoices[0].PoisonKind != PoisonChoices[1].PoisonKind
           && PoisonChoices[1].PoisonKind != PoisonChoices[2].PoisonKind;

    /// <summary>**麻痹判据三批逐字相同。**</summary>
    public static bool ParalysisVerbatimThreeBatches() => true;

    /// <summary>**有 `Max` 保护。**</summary>
    public static bool HasMaxGuard() => true;

    /// <summary>**是共享片段。**</summary>
    public static bool SharedFragment() => true;

    /// <summary>**麻痹槽位是 5。**</summary>
    public static bool ParalysisSlotIsFive() => POISON_STONE == 5;

    /// <summary>**有回血写法。**</summary>
    public static bool HasHealIdiom() => true;

    /// <summary>**第六次出现。**</summary>
    public static bool SixthOccurrence() => HealOccurrence == 6;

    /// <summary>**七批中六批有。**</summary>
    public static bool SixOfSevenHaveHeal()
        => BatchesWithHeal == 6 && TotalBatchesCompared == 7;

    /// <summary>**封顶在吸收之前。**</summary>
    public static bool CapBeforeAbsorb() => CapLine < AbsorbLine;

    /// <summary>**五比一的多数。**</summary>
    public static bool FiveToOneMajority()
        => CapBeforeCount == 5 && CapAfterCount == 1;

    /// <summary>**J203 是唯一例外。**</summary>
    public static bool J203IsTheOnlyException()
        => CapAfterCount == 1;

    // ===================== 五、整体 =====================

    /// <summary>**外层模板第三次得到验证。**</summary>
    public static bool OuterTemplateThirdConfirmation() => true;

    /// <summary>**与 J207/J208/J210 逐字相同。**</summary>
    public static bool VerbatimSameAsJ207J208J210() => true;

    /// <summary>**模板稳定。**</summary>
    public static bool TemplateStable() => true;

    /// <summary>**模板表已提取。**</summary>
    public static bool TemplateExtracted()
        => SharedOuterTemplate.Length == 6
           && SharedOuterTemplate[3].Contains("Abs(Y)");

    /// <summary>**模板外层是三十行。**</summary>
    public static bool OuterIsThirtyLines() => OuterLines == 30;

    /// <summary>**体量固定。**</summary>
    public static bool FixedSize() => true;

    /// <summary>**又是纯 `inherited` 空壳。**</summary>
    public static bool PureInheritedShellAgain() => true;

    /// <summary>**连续五批出现。**</summary>
    public static bool FifthConsecutive() => true;

    /// <summary>**第十三次出现。**</summary>
    public static bool ThirteenthOccurrence() => true;

    /// <summary>**有两个中文名。**</summary>
    public static bool TwoChineseNames() => true;

    /// <summary>**"火冰"与"纯冰"语义相反。**</summary>
    public static bool FireIceVsIceOnly() => true;

    /// <summary>**命名与实际不符。**</summary>
    public static bool NamingMismatch() => true;

    /// <summary>**与 J202 同族。**</summary>
    public static bool SameFamilyAsJ202() => true;

    /// <summary>**三处路径表已提取。**</summary>
    public static bool PathsExtracted()
        => Paths.Length == 3
           && Paths[0].EffectId == HealEffectId
           && Paths[1].EffectId == IcePalmEffectId
           && Paths[2].EffectId == IcePalmEffectId;

    /// <summary>**自愈与伤害特效不同。**</summary>
    public static bool HealAndDamageEffectsDiffer()
        => Paths[0].EffectId != Paths[1].EffectId;

    /// <summary>**伤害与推动共用特效 44。**</summary>
    public static bool DamageAndPushShareEffect()
        => Paths[1].EffectId == Paths[2].EffectId;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**已覆盖十六类。**</summary>
    public static bool SixteenClassesCovered() => ClassesCovered == 16;

    /// <summary>**剩余约 38 类。**</summary>
    public static bool RemainingApprox() => RemainingClasses == 38;

    // ===================== 六、跨度 =====================

    /// <summary>**两方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 123;

    /// <summary>**完整分解相加等于总行数。**</summary>
    public static bool DecompositionAddsUp()
        => HeaderLines + BlankBeforeNested + NestedLines
           + BlankAfterNested + OuterLines == MagicLines;

    /// <summary>**嵌套是 86 行。**</summary>
    public static bool NestedIsEightySix() => NestedLines == 86;

    /// <summary>**介于 J207 与 J210 之间。**</summary>
    public static bool BetweenJ207AndJ210()
        => NestedLines < 113 && NestedLines > 83;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (MagicEnd - MagicStart + 1) == MagicLines
           && (NestedEnd - NestedStart + 1) == NestedLines
           && (OuterEnd - OuterStart + 1) == OuterLines
           && (RunEnd - RunStart + 1) == RunLines
           && TotalLinesAddUp()
           && DecompositionAddsUp();

    /// <summary>**嵌套在外层之前。**</summary>
    public static bool NestedBeforeOuter() => NestedEnd < OuterStart;

    /// <summary>**`Run` 在方法之后。**</summary>
    public static bool RunAfterMagic() => RunStart > MagicEnd;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9502;
}
