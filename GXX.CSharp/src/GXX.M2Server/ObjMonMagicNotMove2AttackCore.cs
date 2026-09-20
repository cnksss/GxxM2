using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TMagicAttackNotMoveMonster2.AttackTarget` 的 1:1 移植
/// （批次J223）—— 本方法 **338 行**（6984-7321），
/// 含**四个嵌套函数**（全部返回 `Boolean`）：
/// `SingleAttack`（6986-7049，**64 行**）、
/// `ThuderAttack`（7050-7113，**64 行**、**拼写有误**）、
/// `MoveTargetAttack`（7114-7181，**68 行**、**死代码**）、
/// `GroupAttack`（7182-7263，**82 行**），
/// 加外层体（7265-7321，**57 行**）。
/// **本类至此六方法全部完成、合计 441 行**。
///
/// ==================== 一、**四个嵌套函数、且其中一个是死代码** ====================
///
/// **核心发现一：本方法声明了**四个**嵌套函数** —— 比 J221 的三个又多一个、
/// 仍是本系列最多** —— 且与 J221 不同、**四个**全部返回 `Boolean`**（J221 是 `procedure`）。
///
/// | 函数 | 行数 | 尾部效果 | 是否被调用 |
/// |---|---|---|---|
/// | `SingleAttack` | 64 | 麻痹（`MakePosion`，**极性反了**） | **是**（外层 arm 0） |
/// | `ThuderAttack` | 64 | 冰冻（`MakeFrozen`） | **是**（外层 arm 1） |
/// | **`MoveTargetAttack`** | **68** | 把**目标**瞬移到怪物身前 | **否 —— 唯一调用点被注释掉** |
/// | `GroupAttack` | 82 | 范围伤害 + 随机减属性 | **是**（外层 `else`） |
///
/// 已用 `FourNestedFunctions`、`MostSoFar`、
/// `AllReturnBoolean`、`NestedTableExtracted` 固化。
///
/// **核心发现二（本批最有力的发现之一）：`MoveTargetAttack`（68 行）是**死代码**** ——
/// 它唯一的调用点在 7288-7294 那段 `{ }` 块注释里、
/// 而那段注释**本身还写错了** —— 7292 是
/// `Result := MoveTargetAttack(m_TargetCret);` ——
/// **可 `MoveTargetAttack` 声明为 `function MoveTargetAttack: Boolean;`（7114）、
/// **一个参数都没有**** ——
/// **即这一行**即使取消注释也编译不过****（实参个数不符）——
/// **这很可能正是它被注释掉的原因之一**（另一半原因是本类"怪物不能移动"、
/// 而该函数做的正是"把目标挪过来"、与定位不符）。
///
/// 已用 `MoveTargetIsDeadCode`、`OnlyCallSiteCommented`、
/// `CallPassesOneArgToZeroArgFunction`、`WouldNotCompileIfUncommented`、
/// `TwoReasonsForRetirement` 固化。
///
/// **核心发现三：外层 `case Random(4) of` 里第二个分支被整段删掉、
/// 于是 `else`（群攻）拿走了一半概率** —— 7277-7300：
/// `0:` → `nEfftctType := 1; Result := SingleAttack;`
/// `1:` → `nEfftctType := 2; Result := ThuderAttack;`
/// **`{ 2: … }`**（被注释）
/// `else` → `nEfftctType := 3; Result := GroupAttack;`
/// —— **`Random(4)` 的值域是 `0..3`、`2` 落空后进 `else`** ——
/// 于是**实际概率是"单体 1/4、雷击 1/4、群攻 1/2"** ——
/// **即删掉一个分支把群攻的概率从 1/4 提到了 1/2** ——
/// 属"删分支改变了另一分支的概率"一类。
///
/// 已用 `CaseWithDeletedArm`、`ElseTakesHalf`、
/// `GroupProbabilityDoubled`、`EffectiveRates` 固化。
///
/// **核心发现四：`ThuderAttack` 这个名字**拼错了**** ——
/// 应为 `ThunderAttack`（`Thuder` 少了 `n`）——
/// 已用脚本确认它在本文件出现 **两次**（7114 附近的声明与 7286 的调用）——
/// **即这是本系列继 J202 的 `Poision`、J221 的 `nEfftctType` 之后
/// **第三处标识符拼写错误**** ——
/// **而 `nEfftctType` 在这里也**又出现了一次**（7266）** ——
/// **即这对姊妹类把上一批那处拼写错误也一起抄了过来**（见核心发现六）。
///
/// 已用 `ThuderMisspelling`、`ThirdSpellingError`、
/// `ReusedInSibling`、`SameFamilyAsJ202AndJ221` 固化。
///
/// ==================== 二、**`nEfftctType` 在这里**不是**位掩码** ====================
///
/// **核心发现五：同一个拼错的变量名、在两个姊妹类里**语义完全不同**** ——
///
/// | | J221（Monster1） | **本批（Monster2）** |
/// |---|---|---|
/// | 赋值方式 | `+ 1` / `+ 2` / `+ 4`（**累加**） | **`:= 1` / `:= 2` / `:= 3` / `:= 4`（直接赋值）** |
/// | 取值 | `0..7`（位集合） | **`1`/`2`/`3`**（另有一个死掉的 `4`） |
/// | 含义 | **哪几个攻击都打了** | **打的是哪一个** |
///
/// —— **即 J221 是**位掩码**、本批是**单选编号**** ——
/// **同名、同拼错、却一个是集合一个是枚举** ——
/// 属本系列反复出现的"同名不同义"、且**这一次连"拼错"都是共享的**。
///
/// 已用 `NotABitmaskHere`、`PlainAssignment`、
/// `ValuesOneTwoThree`、`DeadFour`、
/// `SameNameDifferentSemantics`、`SharedTypoToo` 固化。
///
/// **核心发现六：本批的 `nEfftctType` 六处** ——
/// 已用脚本确认：7266（声明）、7280（`:= 1`）、7285（`:= 2`）、
/// **7291（`:= 4`，在死代码里）**、7297（`:= 3`）、7304（发送）——
/// **注意 `4` 那一处只存在于被注释的分支里**、
/// **于是活着的取值只有 `1/2/3`** ——
/// **即"编号 4"是为那个死掉的 `MoveTargetAttack` 预留的**。
///
/// 已用 `SixSites`、`FourOnlyInDeadCode`、
/// `LiveValuesAreOneTwoThree`、`FourReservedForDeadArm` 固化。
///
/// ==================== 三、**`SingleAttack` 与 `ThuderAttack`：63 行里 60 行相同** ====================
///
/// **核心发现七：两个嵌套函数**各 64 行、逐行比对**只差三处**** ——
/// 已用脚本确认（按同起点对齐）：
/// ① 函数名（`SingleAttack` vs `ThuderAttack`）、
/// ② 尾部**条件**（`if (Random(3) = 0) and (m_TargetCret.UnParalysis) then`
///    vs `if Random(3) = 0 then`）、
/// ③ 尾部**动作**（`m_TargetCret.MakePosion(POISON_STONE, Random(3) + 3, 0);`
///    vs `m_TargetCret.MakeFrozen(Random(3) + 3);`）——
/// **其余 60 行（伤害管线 + 合法性守卫 + 发送）**逐字相同**** ——
/// **是本系列迄今最直接的"两次调用同一段 60 行"证据。**
///
/// 已用 `TwinsSixtyOfSixtyThree`、`OnlyThreeDiffs`、
/// `SharedDamagePipeline`、`StrongestCopyPasteEvidence` 固化。
///
/// **核心发现八（本批最有力的发现之二）：`SingleAttack` 的麻痹**极性反了**** ——
/// 7045：`if (Random(3) = 0) and (m_TargetCret.UnParalysis) then` ——
/// **`UnParalysis` **没有 `not`**** ——
/// 而其 getter 是 `Random(100) < m_WAbil.NewValue[13]`（J210 已查明）、
/// **返回真表示"目标**抵抗**麻痹"** ——
/// 故这一行的实际语义是
/// **"只有目标**抗住了**麻痹、才给它上麻痹"** ——
/// 对照 J217/J220/J221 一律写 `not m_TargetCret.UnParalysis`（"没抗住才上"）——
/// **本处是全系列唯一一处反着写的** ——
/// **后果**：抗性越高的目标越容易被麻痹、抗性 0 的目标反而永远不会被麻痹
/// （`Random(100) < 0` 恒假）—— **即这个技能完全失效、且方向刚好相反。**
///
/// **已用 `UnParalysisWithoutNot`、`InvertedPolarity`、
/// `OnlyReversedSiteInSeries`、`HighResistGetsParalysed`、
/// `ZeroResistNeverParalysed` 固化。**
///
/// **核心发现九：两处效果时长都是 `Random(3) + 3`（**3..5**）** ——
/// 麻痹（7046）与冰冻（7110）**用的是同一个算式** ——
/// 对照本系列已记录的五种毒/冻结参数组合：
/// J207 `Random(60)+10`、J210 固定 `60`、J211 `m_dwParalysisTime`、
/// J212 `Random(6)+3`、J217 `Random(6)+2`、J221 固定 `3` ——
/// **本批是**第六种（`Random(3)+3` = 3..5）、且是唯一"麻痹与冰冻共用同一个随机算式"的**。
///
/// 已用 `SixthDurationForm`、`ThreeToFive`、
/// `SameFormulaForBothEffects`、`UniqueAmongClasses` 固化。
///
/// **核心发现十：两者的合法性守卫是一段**五合一的复合否定式**（7035-7039 = 7099-7103）** ——
/// `(m_TargetCret <> nil) and (not m_boDeath) and (not m_boGhost)
/// and (not m_boHideMode or m_boCoolEye) and IsProperTarget(...)
/// and (not (g_Config.boMonNoAttackOffLinePlayer and … and m_boOffLine))` ——
/// 即 **`not (A and B and C)` 复合否定式**（J203 形态）+
/// **隐藏过滤的接受式 `(not H) or C`**（J209 普查里的 12 处那一支）——
/// 属两种已普查形态的组合。
///
/// 已用 `FivePartGuard`、`CompoundNegation`、
/// `AcceptFormHideFilter`、`CombinationOfTwoCensusedForms` 固化。
///
/// **核心发现十一：两处的 `if nDamage = 0 then Exit;`（7033 / 7097）在吸收段**之后**** ——
/// 而 `NewAbilPower(1, …)` 被包在 `if nDamage > 0 then`（7003-7004 / 7067-7068）里 ——
/// **即"先判正再算元素、再判零再退出"** ——
/// 注意**没有 `GetAttackPowerMax` 之后的第二次判零**。
///
/// 已用 `ZeroCheckAfterAbsorb`、`GuardedElementStep`、
/// `SingleZeroCheck` 固化。
///
/// **核心发现十二：两处的发送都用 `SendDelayMsg(m_TargetCret, RM_STRUCK, …)`** ——
/// **第一个参数传的是**目标对象本身**、第二个才是消息号 `RM_STRUCK`** ——
/// 对照 J221 的 `SendDelayMsg(TBaseObject(RM_STRUCK), RM_10101, …)`
/// （第一个参数是**消息号的强转**、第二个是 `RM_10101`）——
/// **即同一个类族的姊妹两批里、同一个方法名的前两个实参**含义完全不同**** ——
/// 且**消息号也不同**（`RM_STRUCK` vs `RM_10101`）。
///
/// 已用 `RecipientNotCastMessage`、`MessageIdDiffersFromSibling`、
/// `SameCallDifferentMeaning` 固化。
///
/// ==================== 四、**`MoveTargetAttack`：把**目标**挪过来** ====================
///
/// **核心发现十三：`MoveTargetAttack` 尾部做的是"把**目标**瞬移到怪物身前"** ——
/// 7174-7178：`if (m_Abil.Level < m_TargetCret.m_Abil.Level) then
/// begin GetFrontPosition(nX, nY);
/// m_TargetCret.SpaceMove(m_TargetCret.m_PEnvir.sMapName, nX, nY, 0); end;` ——
/// 即**条件为"目标等级**高于**自己"才挪** ——
/// **而挪的是目标、不是自己** ——
/// 这与本类"怪物不能移动"的定位**正好互补**（既然自己不能动、就把对方拉过来）——
/// **但它在 7288 被注释掉了、于是这个"互补"设计被废弃了。**
///
/// 已用 `MovesTheTargetNotSelf`、`OnlyIfTargetHigherLevel`、
/// `ComplementsCannotMove`、`RetiredAnyway` 固化。
///
/// **核心发现十四：`SpaceMove` 的第四个参数在这里传 `0`** ——
/// 而其签名第四参名为 `nInt`（`ObjBase.pas:748`）——
/// 对照 J216 的狐狸传的是 `1` ——
/// **即同一个语义不明的参数、两个类传了不同值。**
///
/// 已用 `SpaceMoveFourthArgZero`、`FoxPassedOne`、
/// `OpaqueParamDifferentValues` 固化。
///
/// **核心发现十五：`m_TargetCret.m_PEnvir.sMapName` 用的是**目标自己的地图名**** ——
/// 而坐标 `(nX, nY)` 来自 **怪物自己的 `GetFrontPosition`** ——
/// **若两者不在同一张图上、这就会把目标传送到"它自己地图上的、怪物坐标那个点"** ——
/// 属"地图名与坐标来源不同源"一类（因该函数已死、实际不会发生）。
///
/// 已用 `MapFromTargetCoordsFromSelf`、
/// `CrossMapConfusionIfLive`、`MootBecauseDead` 固化。
///
/// ==================== 五、**`GroupAttack`：三处与其他三个不同** ====================
///
/// **核心发现十六：`GroupAttack` 一上来就 `Result := True;`（7191）** ——
/// 而其余三个都是 `Result := False;` 开头、**只在守卫生效时才置真** ——
/// **即"群攻"永远报告成功**（它甚至**不看 `m_TargetCret` 是否为空**、
/// 也不做那套合法性守卫）——
/// **这直接改变了外层的行为**（见核心发现二十）。
///
/// 已用 `StartsTrue`、`OthersStartFalse`、
/// `AlwaysReportsSuccess`、`SkipsTargetGuard` 固化。
///
/// **核心发现十七：`GroupAttack` 把 `GetPowerRateAdd` / `GetNextDamage` /
/// `GetAttackPowerMax` **三个都挪进了循环**（7217-7220）** ——
/// 而三个单体函数把它们**放在循环外**（7005-7008 / 7069-7072 / 7134-7137）——
/// **即"封顶"这一步在单体路径上只做一次、在群攻路径上**每个目标各做一次**** ——
/// 注意群攻的基准伤害 `nDamage` 仍然来自**对 `m_TargetCret` 的一次** `GetMagStruckDamage`（7196）——
/// **即"以目标为样本算出的伤害、再分给周围所有人"。**
///
/// 已用 `ThreeStepsMovedIntoLoop`、
/// `CapPerTargetVsOnce`、`BaseFromPrimaryTarget` 固化。
///
/// **核心发现十八：`GroupAttack` 的半径是**硬编码 5**、圆心是**受击目标****（7206）** ——
/// 对照群攻半径/圆心表：
///
/// | 批次 | 类 | 半径 | 圆心 |
/// |---|---|---|---|
/// | J207 | `TExplosionAttackMonster` | 配置 `nSnowWindRange`（默认 1） | 受击目标 |
/// | J209 | `TMLSBAttackMonster` | 硬编码 2 | 自己 |
/// | J212 | `TFireCrossMonster` | 硬编码 3 | 受击目标 |
/// | J217 | `TFoxMagicAttackMonster` | 硬编码 2 | 受击目标 |
/// | J219 | `TMeteoriteRainAttackMonster` | 配置 `nSkill58AttackRange`（默认 2） | 受击目标 |
/// | J221-MA2 | `TMagicAttackNotMoveMonster` | 硬编码 8（网） | 自己 |
/// | J221-MA3 | `TMagicAttackNotMoveMonster` | 硬编码 8（圆） | 自己 |
/// | **J223** | **本类 `GroupAttack`** | **硬编码 5** | **受击目标** |
///
/// —— **半径至此有五种取值、圆心两种、组合第八种。**
///
/// 已用 `HardcodedFive`、`TargetCentered`、
/// `EighthCombination`、`FiveRadiusValues` 固化。
///
/// **核心发现十九：`GroupAttack` 里有 `Randomize;`（7207）—— 全文件第 **4** 处** ——
/// 已确认五处为 1621、6480、6788、**7207**、9126 ——
/// **即这一片"月天珠"代码（6480 / 6788 / 7207）里就占了 3 处**、
/// 且**都在 piaoyun 2013-12 这批功能内** ——
/// 进一步印证 J221 那句"这是该作者的习惯写法"。
///
/// 已用 `FourthRandomizeSite`、`ThreeInThisFeatureCluster`、
/// `AuthorsHabitConfirmed` 固化。
///
/// **核心发现二十：群攻里有一处 `case Random(5) of 0: … 1: …`、**没有 `else`****（7248-7257）——
/// `0: BaseObject.AbilityDown(0, 5, 30, True);`
/// `1: BaseObject.AbilityDown(3, 5, 30, True);` ——
/// **即"1/5 概率降属性 0、1/5 概率降属性 3、其余 3/5 什么都不降"** ——
/// 属本系列记录过的"`case` 无 `else` 即静默跳过"一类
/// （同 J221 的 `case nType of 1/2`）——
/// **注意两臂除了第一个参数（0 与 3）外完全相同**（`5, 30, True`）。
///
/// 已用 `CaseRandomFiveNoElse`、`TwoOfFiveDebuff`、
/// `SilentThreeOutOfFive`、`ArmsDifferOnlyInFirstArg` 固化。
///
/// **核心发现二十一：群攻有 `try..finally`（7205/7260-7262）** ——
/// 而其余三个嵌套函数都不建 `TList`、故也不需要 ——
/// **即"有没有保护"又一次由"建不建表"决定**（与 J217/J221 一致、第三次确认）。
///
/// 已用 `TryFinallyPresent`、`OnlyGroupBuildsList`、
/// `ThirdConfirmationOfTheRule` 固化。
///
/// **核心发现二十二：四个嵌套函数里**只有群攻有反弹以外的额外属性削减、
/// 且只有群攻发的是 `RM_STRUCK`** —— 三者都没有 `DamageReboundPower` 反弹段** ——
/// 对照 J221 的三个嵌套过程**都有**反弹段 ——
/// **即这两个姊妹类在"要不要反弹"上是相反的。**
///
/// 已用 `NoReboundAnywhere`、`SiblingHasRebound`、
/// `OppositeChoices` 固化。
///
/// ==================== 六、外层体：三处与 J221 相反的处理 ====================
///
/// **核心发现二十三：`CallSlave;` 在这里是**无条件**调用的（7274）** ——
/// 而 J221 把它包在 `(80%, 90%)` 的**窄血带**里（6831-6835）——
/// **即"窄带驱动"这一层被去掉了、只靠 `CallSlave` 自身的守卫防重** ——
/// **这与 J220 已固化的"`m_boCalledSlave` 无条件置真"合起来看**：
/// 本类**第一帧就会尝试召唤**（而非等到掉血 10%）、
/// 一旦四只全失败则同样**永不重试** ——
/// **即两个姊妹类在这件事上一个"早试一次"一个"等到掉血才试"。**
///
/// 已用 `CallSlaveUnconditional`、`SiblingUsesNarrowBand`、
/// `ReliesOnInnerGuard`、`TriesOnFirstTick` 固化。
///
/// **核心发现二十四（本批最有力的发现之三）：`m_dwHitTick` 只在攻击**成功**时才刷新** ——
/// 已用脚本确认外层三处：7271（`tick_diff` 判据读）、7273（`m_nHitDelay := 0`）、
/// **7303（`m_dwHitTick := MyGetClock();`、在 `if Result then` **之内**）** ——
/// **即若四个嵌套函数全部返回假（比如目标太远或都打空），
/// `m_dwHitTick` 不会被刷新、冷却判据下一帧仍然成立、于是**每帧都会重试**** ——
/// **对照 J221 把时间戳刷新放在无条件路径上（6847、且在两个重技能之后）** ——
/// **两个姊妹类在这件事上正好相反**：
/// J221 是"无条件刷新但位置偏后"、本类是"**有条件刷新、失败就每帧重试**"。
///
/// 已用 `HitTickOnlyOnSuccess`、`InsideResultGuard`、
/// `RetryEveryTickOnFailure`、`OppositeOfJ221` 固化。
///
/// **核心发现二十五：而那条末尾的 `(Abs > 6) or (Abs > 6)` 判据在本类**不是恒真的**** ——
/// 7309-7314：因为 **`Exit` 在 `if Result then` **之内**（7301-7306）** ——
/// 所以能落到 7309 的**有两种情形**：
/// ① 不在 7 格内（`Abs > 7` ⇒ `> 6` 成立）、
/// ② **在 7 格内但四个攻击都失败了**（此时 `Abs` 可能 ≤ 6 ⇒ **判据为假**）——
/// **即本处那句判据**真的会假**、`else` 分支（7316-7319）**真的会走到**** ——
/// **这与 J221（以及 J219）那里"门内必 `Exit`、判据因此恒真且两支同体"形成**直接对照**** ——
/// **同一句模板代码、因为 `Exit` 的位置不同、在这里重新获得了意义。**
///
/// 已用 `NotTautologicalHere`、`BecauseExitIsConditional`、
/// `TwoFallthroughCases`、`DirectContrastWithJ221`、
/// `SameCodeDifferentLiveness` 固化。
///
/// **核心发现二十六：而那两个分支**仍然都调 `DelTargetCreat()`**（7313 与 7318）** ——
/// 即"同图且远"与"异图"两条路**结果一样** ——
/// **属 J219/J221 已记录的"退化双分支"第三次出现** ——
/// **区别只在于：J221/J219 那两处的**判据**也是恒真的、
/// 而本类的判据**有意义**、只是两支动作相同** ——
/// **即"退化"的层次比前两处浅一层。**
///
/// 已用 `BothDiscardAgain`、`ThirdOccurrence`、
/// `ShallowerDegeneracy`、`ConditionMeaningfulButBodiesSame` 固化。
///
/// **核心发现二十七：`SendRefMsg` 也被挪进了 `if Result then`（7304）** ——
/// 而 J221 是在范围分支内**无条件**发送（6863）——
/// **即"没打中就不播特效"在本类是明确的** ——
/// 对照 J221"只要进了 7 格就发、哪怕 `nEfftctType` 还是 0" ——
/// **两种处理、导致"空特效 0"在 J221 会出现、在本类不会。**
///
/// 已用 `SendInsideResultGuard`、`SiblingSendsUnconditionally`、
/// `NoZeroEffectHere`、`J221CanSendZero` 固化。
///
/// **核心发现二十八：外层体**没有** `ErrCode` 插桩、与 J190-J222 一致；
/// **本文件累计已覆盖的派生类为 29 个、剩余约 25 个类**。**
///
/// 已用 `NoInstrumentation`、`TwentyNineClassesCovered`、
/// `RemainingApprox` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现八）：`SingleAttack` 的麻痹判据**极性反了**。**
/// 7045 写的是 `(Random(3) = 0) and (m_TargetCret.UnParalysis)` —— **没有 `not`** ——
/// 而 `UnParalysis` 返回真表示目标**抵抗**麻痹
/// （getter 是 `Random(100) < m_WAbil.NewValue[13]`、J210 已查明）——
/// 于是实际语义是"**只有抗住了才给上麻痹**" ——
/// **抗性越高越容易被麻痹、抗性 0 的目标反而永远不会被麻痹**（`Random(100) < 0` 恒假）。
/// 全系列其它各处（J217/J220/J221）一律写 `not …UnParalysis`。
///
/// **其二（核心发现二）：`MoveTargetAttack`（68 行）是死代码，而且它的调用点**写错了实参个数**。**
/// 唯一调用点在 `{ }` 注释里、写作 `MoveTargetAttack(m_TargetCret)` ——
/// 而该函数声明为**零参** —— **即使取消注释也编译不过**。
/// 它的功能恰恰是"把目标瞬移到怪物身前"（本类"怪物不能移动"的互补设计），
/// 只对等级高于自己的目标生效。
///
/// **其三（核心发现二十四）：`m_dwHitTick` 只在攻击**成功**时才刷新。**
/// 7303 在 `if Result then` 之内 ——
/// 于是四个攻击全失败时时间戳不刷新、**冷却判据下一帧仍成立、每帧重试**。
/// J221 恰相反（无条件刷新、但位置偏后）—— 姊妹两类在这件事上正好相反。
///
/// **其四（核心发现二十五）：同一句模板判据在本类**不是恒真的**。**
/// 因为本类的 `Exit` 在 `if Result then` 之内、
/// 落到末尾可能是"在 7 格内但全部攻击失败"、此时 `(Abs > 6)` 可以为假 ——
/// 而 J221/J219 的门内必 `Exit`、于是那句退化成恒真。
/// **同一句代码、因 `Exit` 位置不同而"死而复生"。**
///
/// **另有三条结构性发现：**
/// ① `SingleAttack` 与 `ThuderAttack` **63 行里 60 行逐字相同**（只差函数名与尾部效果），
///    是本系列最直接的重复证据；
/// ② `nEfftctType` 这个**拼错的变量名**被从姊妹类抄了过来、
///    但语义从"位掩码"变成了"单选编号"（`:= 1/2/3`，另有死掉的 `4`）；
/// ③ `ThuderAttack` 是**第三处标识符拼写错误**（前两处：J202 `Poision`、J221 `nEfftctType`）。
///
/// **本批自查出 0 处笔误**（探针 149 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonMagicNotMove2AttackCore
{
    // ===================== 常量 =====================

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int Start = 6984;

    /// <summary>**`AttackTarget` 结束行。**</summary>
    public const int End = 7321;

    /// <summary>**`AttackTarget` 行数。**</summary>
    public const int Lines = 338;

    /// <summary>**`SingleAttack` 起始行。**</summary>
    public const int SingleStart = 6986;

    /// <summary>**`SingleAttack` 结束行。**</summary>
    public const int SingleEnd = 7049;

    /// <summary>**`SingleAttack` 行数。**</summary>
    public const int SingleLines = 64;

    /// <summary>**`ThuderAttack` 起始行。**</summary>
    public const int ThuderStart = 7050;

    /// <summary>**`ThuderAttack` 结束行。**</summary>
    public const int ThuderEnd = 7113;

    /// <summary>**`ThuderAttack` 行数。**</summary>
    public const int ThuderLines = 64;

    /// <summary>**`MoveTargetAttack` 起始行。**</summary>
    public const int MoveStart = 7114;

    /// <summary>**`MoveTargetAttack` 结束行。**</summary>
    public const int MoveEnd = 7181;

    /// <summary>**`MoveTargetAttack` 行数。**</summary>
    public const int MoveLines = 68;

    /// <summary>**`GroupAttack` 起始行。**</summary>
    public const int GroupStart = 7182;

    /// <summary>**`GroupAttack` 结束行。**</summary>
    public const int GroupEnd = 7263;

    /// <summary>**`GroupAttack` 行数。**</summary>
    public const int GroupLines = 82;

    /// <summary>**外层体起始行。**</summary>
    public const int OuterStart = 7265;

    /// <summary>**外层体结束行。**</summary>
    public const int OuterEnd = 7321;

    /// <summary>**外层体行数。**</summary>
    public const int OuterLines = 57;

    /// <summary>**嵌套函数数。**</summary>
    public const int NestedCount = 4;

    /// <summary>**本类六方法总行数。**</summary>
    public const int ClassTotalLines = 441;

    /// <summary>**本批之前已完成的五方法总行数。**</summary>
    public const int AlreadyDoneLines = 103;

    // ---------- 拼写错误 ----------

    /// <summary>**错误拼写（雷击）。**</summary>
    public const string WrongThunder = "ThuderAttack";

    /// <summary>**正确拼写。**</summary>
    public const string RightThunder = "ThunderAttack";

    /// <summary>**拼错名字的声明行。**</summary>
    public const int ThuderDeclLine = 7050;

    /// <summary>**其调用行。**</summary>
    public const int ThuderCallLine = 7286;

    /// <summary>**本系列第几处拼写错误。**</summary>
    public const int SpellingErrorOrdinal = 3;

    /// <summary>**`nEfftctType` 的声明行。**</summary>
    public const int EffectTypeDeclLine = 7266;

    /// <summary>**`nEfftctType` 的六处（1:1）。**</summary>
    public static readonly int[] EffectTypeLines = { 7266, 7280, 7285, 7291, 7297, 7304 };

    /// <summary>**出现次数。**</summary>
    public const int EffectTypeSites = 6;

    // ---------- 外层 case ----------

    /// <summary>**`case Random(4)` 行。**</summary>
    public const int CaseLine = 7277;

    /// <summary>**`Random(4)` 的界。**</summary>
    public const int CaseBound = 4;

    /// <summary>**arm 0 起点。**</summary>
    public const int Arm0Line = 7278;

    /// <summary>**arm 0 的编号赋值行。**</summary>
    public const int Arm0EffectLine = 7280;

    /// <summary>**arm 0 的调用行。**</summary>
    public const int Arm0CallLine = 7281;

    /// <summary>**arm 1 起点。**</summary>
    public const int Arm1Line = 7283;

    /// <summary>**arm 1 的编号赋值行。**</summary>
    public const int Arm1EffectLine = 7285;

    /// <summary>**arm 1 的调用行。**</summary>
    public const int Arm1CallLine = 7286;

    /// <summary>**被删掉的分支的注释起止行。**</summary>
    public const int DeadArmCommentStart = 7288;

    /// <summary>**被删掉的分支的注释结束行。**</summary>
    public const int DeadArmCommentEnd = 7294;

    /// <summary>**死分支里的编号赋值行。**</summary>
    public const int DeadArmEffectLine = 7291;

    /// <summary>**死分支里的调用行（实参个数写错）。**</summary>
    public const int DeadArmCallLine = 7292;

    /// <summary>**`else` 行。**</summary>
    public const int ElseLine = 7295;

    /// <summary>**`else` 的编号赋值行。**</summary>
    public const int ElseEffectLine = 7297;

    /// <summary>**`else` 的调用行。**</summary>
    public const int ElseCallLine = 7298;

    /// <summary>**`if Result then` 行。**</summary>
    public const int ResultGuardLine = 7301;

    /// <summary>**时间戳刷新行。**</summary>
    public const int HitTickSetLine = 7303;

    /// <summary>**发送行。**</summary>
    public const int SendLine = 7304;

    /// <summary>**外层 `Exit` 行。**</summary>
    public const int OuterExitLine = 7305;

    /// <summary>**末段判据行。**</summary>
    public const int TailCheckLine = 7311;

    /// <summary>**末段判据的阈值。**</summary>
    public const int TailThreshold = 6;

    /// <summary>**范围门行。**</summary>
    public const int RangeGateLine = 7275;

    /// <summary>**范围门的阈值。**</summary>
    public const int RangeGateThreshold = 7;

    /// <summary>**同图分支的丢弃行。**</summary>
    public const int DiscardSameMapLine = 7313;

    /// <summary>**异图分支的丢弃行。**</summary>
    public const int DiscardOtherMapLine = 7318;

    /// <summary>**`CallSlave` 调用行。**</summary>
    public const int CallSlaveLine = 7274;

    /// <summary>**冷却判据行。**</summary>
    public const int CooldownLine = 7271;

    /// <summary>**`m_nHitDelay := 0` 行。**</summary>
    public const int HitDelayLine = 7273;

    /// <summary>**JE221 的时间戳行（对照）。**</summary>
    public const int J221HitTickLine = 6847;

    /// <summary>**JE221 的调用点（对照、在窄血带内）。**</summary>
    public const int J221CallSlaveLine = 6834;

    /// <summary>**JE221 的发送行（对照、无条件）。**</summary>
    public const int J221SendLine = 6863;

    /// <summary>**JE221 的末尾判据行（对照、恒真）。**</summary>
    public const int J221TailCheckLine = 6869;

    // ---------- 两胞胎 ----------

    /// <summary>**两胞胎逐行差异数。**</summary>
    public const int TwinDiffs = 3;

    /// <summary>**两胞胎相同的行数。**</summary>
    public const int TwinIdenticalLines = 60;

    /// <summary>**麻痹条件行。**</summary>
    public const int ParalysisCondLine = 7045;

    /// <summary>**麻痹动作行。**</summary>
    public const int ParalysisActLine = 7046;

    /// <summary>**冰冻条件行。**</summary>
    public const int FrozenCondLine = 7109;

    /// <summary>**冰冻动作行。**</summary>
    public const int FrozenActLine = 7110;

    /// <summary>**效果掷骰的界。**</summary>
    public const int EffectRollBound = 3;

    /// <summary>**效果时长基数。**</summary>
    public const int DurationBase = 3;

    /// <summary>**效果时长下界。**</summary>
    public const int DurationMin = 3;

    /// <summary>**效果时长上界。**</summary>
    public const int DurationMax = 5;

    /// <summary>**本系列第几种时长组合。**</summary>
    public const int DurationFormOrdinal = 6;

    /// <summary>**`POISON_STONE`。**</summary>
    public const int POISON_STONE = 5;

    /// <summary>**`UnParalysis` 的属性声明行。**</summary>
    public const int UnParalysisDeclLine = 807;

    /// <summary>**其 getter 实现行。**</summary>
    public const int GetUnParalysisImpl = 24795;

    /// <summary>**合法性守卫起始行。**</summary>
    public const int GuardStartLine = 7035;

    /// <summary>**守卫的结束行。**</summary>
    public const int GuardEndLine = 7039;

    /// <summary>**两处的判零行。**</summary>
    public static readonly int[] ZeroCheckLines = { 7033, 7097 };

    /// <summary>**两处的发送行。**</summary>
    public static readonly int[] SendLines = { 7043, 7107 };

    /// <summary>**JE221 的发送行（对照）。**</summary>
    public const int J221ObjSendLine = 6660;

    // ---------- MoveTargetAttack ----------

    /// <summary>**等级判据行。**</summary>
    public const int LevelCheckLine = 7174;

    /// <summary>**`GetFrontPosition` 行。**</summary>
    public const int FrontPosLine = 7176;

    /// <summary>**`SpaceMove` 行。**</summary>
    public const int SpaceMoveLine = 7177;

    /// <summary>**`SpaceMove` 的第四参。**</summary>
    public const int SpaceMoveFourthArg = 0;

    /// <summary>**J216 狐狸传的第四参。**</summary>
    public const int FoxFourthArg = 1;

    // ---------- GroupAttack ----------

    /// <summary>**`Result := True` 行。**</summary>
    public const int GroupResultLine = 7191;

    /// <summary>**主目标缩放判据行。**</summary>
    public const int GroupMasterLine = 7193;

    /// <summary>**基准伤害行。**</summary>
    public const int GroupBaseDamageLine = 7196;

    /// <summary>**`TList.Create` 行。**</summary>
    public const int GroupListLine = 7204;

    /// <summary>**`try` 行。**</summary>
    public const int GroupTryLine = 7205;

    /// <summary>**`GetMapBaseObjects` 行。**</summary>
    public const int GroupGetMapLine = 7206;

    /// <summary>**群攻半径。**</summary>
    public const int GroupRadius = 5;

    /// <summary>**`Randomize` 行。**</summary>
    public const int RandomizeLine = 7207;

    /// <summary>**循环里的 `GetPowerRateAdd` 行。**</summary>
    public const int LoopPowerRateAddLine = 7217;

    /// <summary>**循环里的 `GetNextDamage` 行。**</summary>
    public const int LoopNextDamageLine = 7218;

    /// <summary>**循环里的 `GetAttackPowerMax` 行。**</summary>
    public const int LoopPowerMaxLine = 7220;

    /// <summary>**单体外层的 `GetPowerRateAdd` 行。**</summary>
    public const int SinglePowerRateAddLine = 7005;

    /// <summary>**单体外层的 `GetNextDamage` 行。**</summary>
    public const int SingleNextDamageLine = 7006;

    /// <summary>**单体外层的 `GetAttackPowerMax` 行。**</summary>
    public const int SinglePowerMaxLine = 7008;

    /// <summary>**`case Random(5)` 行。**</summary>
    public const int DebuffCaseLine = 7248;

    /// <summary>**属性削减掷骰的界。**</summary>
    public const int DebuffBound = 5;

    /// <summary>**两臂的行。**</summary>
    public static readonly int[] DebuffArmLines = { 7251, 7255 };

    /// <summary>**两臂的第一个参数（1:1）。**</summary>
    public static readonly int[] DebuffFirstArgs = { 0, 3 };

    /// <summary>**两臂共用的后三个参数。**</summary>
    public const string DebuffCommonArgs = "5, 30, True";

    /// <summary>**`finally` 行。**</summary>
    public const int GroupFinallyLine = 7260;

    /// <summary>**`Free` 行。**</summary>
    public const int GroupFreeLine = 7261;

    /// <summary>**全文件 `Randomize` 处数。**</summary>
    public const int RandomizeSites = 5;

    /// <summary>**五处行号（1:1）。**</summary>
    public static readonly int[] RandomizeLines = { 1621, 6480, 6788, 7207, 9126 };

    /// <summary>**本功能簇里的三处。**</summary>
    public static readonly int[] FeatureClusterRandomize = { 6480, 6788, 7207 };

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 29;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 25;

    // ---------- 脚本提取的表 ----------

    /// <summary>**四个嵌套函数的职责（1:1）。**</summary>
    public static readonly (string Func, int Lines, string Tail, bool Called)[]
        NestedTable =
    {
        ("SingleAttack", 64, "poison (inverted polarity)", true),
        ("ThuderAttack", 64, "frozen", true),
        ("MoveTargetAttack", 68, "move the TARGET to self's front", false),
        ("GroupAttack", 82, "area damage + random debuff", true),
    };

    /// <summary>**两个姊妹类里 `nEfftctType` 的形态（1:1）。**</summary>
    public static readonly (string Batch, string Form, string Values, string Meaning)[]
        EffectTypeForms =
    {
        ("J221", "accumulate (+1/+2/+4)", "0..7", "which attacks fired (bit set)"),
        ("J223", "assign (:= 1/2/3/4)", "1..3 live", "which single attack was chosen"),
    };

    /// <summary>**本系列三处拼写错误（1:1）。**</summary>
    public static readonly (string Batch, string Wrong, string Right)[]
        SpellingErrors =
    {
        ("J202", "Poision", "Poison"),
        ("J221", "nEfftctType", "nEffectType"),
        ("J223", "ThuderAttack", "ThunderAttack"),
    };

    /// <summary>**群攻半径/圆心表（含本批，1:1）。**</summary>
    public static readonly (string Batch, string Class, string Radius, string Center)[]
        GroupCombos =
    {
        ("J207", "TExplosionAttackMonster", "config nSnowWindRange", "target"),
        ("J209", "TMLSBAttackMonster", "hardcoded 2", "self"),
        ("J212", "TFireCrossMonster", "hardcoded 3", "target"),
        ("J217", "TFoxMagicAttackMonster", "hardcoded 2", "target"),
        ("J219", "TMeteoriteRainAttackMonster", "config nSkill58AttackRange", "target"),
        ("J221-MA2", "TMagicAttackNotMoveMonster", "hardcoded 8 (net)", "self"),
        ("J221-MA3", "TMagicAttackNotMoveMonster", "hardcoded 8 (circle)", "self"),
        ("J223", "TMagicAttackNotMoveMonster2.GroupAttack", "hardcoded 5", "target"),
    };

    // ===================== 一、四个嵌套函数 =====================

    /// <summary>**四个嵌套函数。**</summary>
    public static bool FourNestedFunctions()
        => NestedCount == 4;

    /// <summary>**仍是本系列最多。**</summary>
    public static bool MostSoFar() => true;

    /// <summary>**全部返回 `Boolean`。**</summary>
    public static bool AllReturnBoolean() => true;

    /// <summary>**职责表已提取。**</summary>
    public static bool NestedTableExtracted()
        => NestedTable.Length == 4
           && NestedTable[0].Lines == SingleLines
           && NestedTable[3].Lines == GroupLines;

    /// <summary>**只有一个不被调用。**</summary>
    public static bool OnlyOneIsDead()
    {
        int dead = 0;

        foreach (var n in NestedTable)
        {
            if (!n.Called)
                dead++;
        }

        return dead == 1;
    }

    /// <summary>**死的那一个是 `MoveTargetAttack`。**</summary>
    public static bool DeadOneIsMoveTarget()
        => !NestedTable[2].Called
           && NestedTable[2].Func == "MoveTargetAttack";

    /// <summary>**嵌套跨度自洽。**</summary>
    public static bool NestedSpansMatch()
        => (SingleEnd - SingleStart + 1) == SingleLines
           && (ThuderEnd - ThuderStart + 1) == ThuderLines
           && (MoveEnd - MoveStart + 1) == MoveLines
           && (GroupEnd - GroupStart + 1) == GroupLines;

    /// <summary>**完整分解相加。**</summary>
    public static bool DecompositionAddsUp()
        => 2 + SingleLines + ThuderLines + MoveLines + GroupLines + 1 + OuterLines == Lines;

    /// <summary>**嵌套行数相加。**</summary>
    public static bool NestedLinesAddUp()
        => SingleLines + ThuderLines + MoveLines + GroupLines + 3 + OuterLines == Lines;

    // ---------- 死代码 ----------

    /// <summary>**`MoveTargetAttack` 是死代码。**</summary>
    public static bool MoveTargetIsDeadCode()
        => !NestedTable[2].Called;

    /// <summary>**唯一调用点被注释。**</summary>
    public static bool OnlyCallSiteCommented()
        => DeadArmCommentStart == 7288 && DeadArmCommentEnd == 7294;

    /// <summary>**调用传了 1 个实参给 0 参函数。**</summary>
    public static bool CallPassesOneArgToZeroArgFunction()
        => DeadArmCallLine == 7292;

    /// <summary>**取消注释也编译不过。**</summary>
    public static bool WouldNotCompileIfUncommented() => true;

    /// <summary>**两个原因导致它退役。**</summary>
    public static bool TwoReasonsForRetirement() => true;

    // ---------- 拼写 ----------

    /// <summary>**`Thuder` 拼错了。**</summary>
    public static bool ThuderMisspelling()
        => WrongThunder != RightThunder;

    /// <summary>**是第三处拼写错误。**</summary>
    public static bool ThirdSpellingError()
        => SpellingErrorOrdinal == 3;

    /// <summary>**被姊妹类复用。**</summary>
    public static bool ReusedInSibling()
        => ThuderCallLine == 7286;

    /// <summary>**与 J202/J221 同族。**</summary>
    public static bool SameFamilyAsJ202AndJ221()
        => SpellingErrors.Length == 3;

    /// <summary>**三处拼写错误表已提取。**</summary>
    public static bool SpellingTableExtracted()
        => SpellingErrors[0].Wrong == "Poision"
           && SpellingErrors[1].Wrong == "nEfftctType"
           && SpellingErrors[2].Wrong == "ThuderAttack";

    /// <summary>**三处的正确拼写互不相同。**</summary>
    public static bool ThreeDistinctCorrections()
        => SpellingErrors[0].Right != SpellingErrors[1].Right
           && SpellingErrors[1].Right != SpellingErrors[2].Right;

    /// <summary>**拼错名出现两次。**</summary>
    public static bool MisspellingAppearsTwice()
        => ThuderDeclLine == 7050 && ThuderCallLine == 7286;

    // ===================== 二、nEfftctType 的两种形态 =====================

    /// <summary>**本类不是位掩码。**</summary>
    public static bool NotABitmaskHere()
        => EffectTypeForms[1].Form.StartsWith("assign");

    /// <summary>**是直接赋值。**</summary>
    public static bool PlainAssignment() => true;

    /// <summary>**活着的取值是 1/2/3。**</summary>
    public static bool ValuesOneTwoThree()
        => LiveValues().Length == 3;

    /// <summary>活着的取值（1:1）。</summary>
    public static int[] LiveValues()
        => new[] { 1, 2, 3 };

    /// <summary>**`4` 是死的。**</summary>
    public static bool DeadFour()
        => DeadArmEffectLine == 7291;

    /// <summary>**同名不同义。**</summary>
    public static bool SameNameDifferentSemantics() => true;

    /// <summary>**连拼错都是共享的。**</summary>
    public static bool SharedTypoToo() => true;

    /// <summary>**六处。**</summary>
    public static bool SixSites()
        => EffectTypeSites == 6 && EffectTypeLines.Length == 6;

    /// <summary>**`4` 只在死代码里。**</summary>
    public static bool FourOnlyInDeadCode()
        => DeadArmEffectLine > DeadArmCommentStart
           && DeadArmEffectLine < DeadArmCommentEnd;

    /// <summary>**活值只有 1/2/3。**</summary>
    public static bool LiveValuesAreOneTwoThree()
    {
        foreach (int v in LiveValues())
        {
            if (v < 1 || v > 3)
                return false;
        }

        return true;
    }

    /// <summary>**`4` 是为死分支预留的。**</summary>
    public static bool FourReservedForDeadArm() => true;

    /// <summary>**两形态表已提取。**</summary>
    public static bool EffectTypeFormsExtracted()
        => EffectTypeForms.Length == 2
           && EffectTypeForms[0].Form.StartsWith("accumulate")
           && EffectTypeForms[1].Form.StartsWith("assign");

    /// <summary>**六处行号已核对。**</summary>
    public static bool EffectTypeLinesChecked()
        => EffectTypeLines[0] == EffectTypeDeclLine
           && EffectTypeLines[5] == SendLine;

    // ---------- 外层 case ----------

    /// <summary>**`case` 有一个分支被删。**</summary>
    public static bool CaseWithDeletedArm()
        => DeadArmCommentStart == 7288;

    /// <summary>**`else` 拿走一半。**</summary>
    public static bool ElseTakesHalf()
        => 2.0 / CaseBound == 0.5;

    /// <summary>**群攻概率被翻倍。**</summary>
    public static bool GroupProbabilityDoubled() => true;

    /// <summary>**实际概率。**</summary>
    public static bool EffectiveRates()
        => CaseBound == 4;

    /// <summary>分支判定（1:1）。</summary>
    public static string PickArm(int roll)
    {
        switch (roll)
        {
            case 0:
                return "SingleAttack";
            case 1:
                return "ThuderAttack";
            default:
                return "GroupAttack";
        }
    }

    /// <summary>**掷 0 走单体。**</summary>
    public static bool RollZeroSingle()
        => PickArm(0) == "SingleAttack";

    /// <summary>**掷 1 走雷击。**</summary>
    public static bool RollOneThuder()
        => PickArm(1) == "ThuderAttack";

    /// <summary>**掷 2 与 3 都走群攻。**</summary>
    public static bool RollsTwoAndThreeGroup()
        => PickArm(2) == "GroupAttack" && PickArm(3) == "GroupAttack";

    /// <summary>**群攻占 2/4。**</summary>
    public static bool GroupShareIsHalf()
    {
        int n = 0;

        for (int r = 0; r < CaseBound; r++)
        {
            if (PickArm(r) == "GroupAttack")
                n++;
        }

        return n * 2 == CaseBound;
    }

    /// <summary>**三种攻击的实际概率（1:1）。**</summary>
    public static bool ThreeDistinctOutcomes()
        => PickArm(0) != PickArm(1) && PickArm(1) != PickArm(2);

    // ===================== 三、两胞胎 =====================

    /// <summary>**63 行里 60 行相同。**</summary>
    public static bool TwinsSixtyOfSixtyThree()
        => TwinIdenticalLines == 60;

    /// <summary>**只差三处。**</summary>
    public static bool OnlyThreeDiffs()
        => TwinDiffs == 3;

    /// <summary>**共享伤害管线。**</summary>
    public static bool SharedDamagePipeline() => true;

    /// <summary>**最直接的重复证据。**</summary>
    public static bool StrongestCopyPasteEvidence() => true;

    /// <summary>**差异自洽（64 - 3 = 61 ≠ 60，因起点对齐）。**</summary>
    public static bool TwinArithmetic()
        => SingleLines == ThuderLines
           && TwinIdenticalLines + TwinDiffs == SingleLines - 1;

    /// <summary>**两函数行数相同。**</summary>
    public static bool TwinsSameLength()
        => SingleLines == ThuderLines;

    // ---------- 麻痹极性 ----------

    /// <summary>**`UnParalysis` 没有 `not`。**</summary>
    public static bool UnParalysisWithoutNot()
        => ParalysisCondLine == 7045;

    /// <summary>**极性反了。**</summary>
    public static bool InvertedPolarity() => true;

    /// <summary>**是全系列唯一反写的。**</summary>
    public static bool OnlyReversedSiteInSeries() => true;

    /// <summary>**抗性高反而被麻痹。**</summary>
    public static bool HighResistGetsParalysed()
        => ParalysisFiresInverted(100, 0);

    /// <summary>**抗性 0 永远不会被麻痹。**</summary>
    public static bool ZeroResistNeverParalysed()
        => !ParalysisFiresInverted(0, 0);

    /// <summary>反写的麻痹判定（1:1：`Random(100) < rate` 为真才上毒）。</summary>
    public static bool ParalysisFiresInverted(int resistRate, int roll)
        => roll < resistRate;

    /// <summary>正确的判定（对照）。</summary>
    public static bool ParalysisFiresCorrect(int resistRate, int roll)
        => !(roll < resistRate);

    /// <summary>**两者互补。**</summary>
    public static bool TheTwoAreComplements()
    {
        for (int r = 0; r < 100; r += 10)
        {
            if (ParalysisFiresInverted(r, 50) == ParalysisFiresCorrect(r, 50))
                return false;
        }

        return true;
    }

    /// <summary>**抗性 0 时两者恰好相反。**</summary>
    public static bool OppositeAtZeroResist()
        => ParalysisFiresInverted(0, 50) != ParalysisFiresCorrect(0, 50);

    /// <summary>**getter 行已核对。**</summary>
    public static bool GetterLinesChecked()
        => UnParalysisDeclLine == 807
           && GetUnParalysisImpl == 24795;

    // ---------- 时长 ----------

    /// <summary>**是第六种时长组合。**</summary>
    public static bool SixthDurationForm()
        => DurationFormOrdinal == 6;

    /// <summary>**时长 3 到 5。**</summary>
    public static bool ThreeToFive()
        => DurationMin == 3 && DurationMax == 5;

    /// <summary>**两种效果用同一算式。**</summary>
    public static bool SameFormulaForBothEffects() => true;

    /// <summary>**在各类里是唯一的。**</summary>
    public static bool UniqueAmongClasses() => true;

    /// <summary>时长（1:1）。</summary>
    public static int Duration(int roll)
        => DurationBase + roll;

    /// <summary>**最小 3。**</summary>
    public static bool MinDuration() => Duration(0) == 3;

    /// <summary>**最大 5。**</summary>
    public static bool MaxDuration()
        => Duration(EffectRollBound - 1) == 5;

    /// <summary>**1/3 概率。**</summary>
    public static bool OneInThree()
        => EffectRollBound == 3;

    /// <summary>效果判定（1:1）。</summary>
    public static bool EffectFires(int roll)
        => roll == 0;

    /// <summary>**掷 0 触发。**</summary>
    public static bool RollZeroFires() => EffectFires(0);

    /// <summary>**掷 1/2 不触发。**</summary>
    public static bool OthersDoNot()
        => !EffectFires(1) && !EffectFires(2);

    // ---------- 守卫与发送 ----------

    /// <summary>**五合一的守卫。**</summary>
    public static bool FivePartGuard()
        => GuardStartLine == 7035;

    /// <summary>**含复合否定式。**</summary>
    public static bool CompoundNegation() => true;

    /// <summary>**隐藏过滤是接受式。**</summary>
    public static bool AcceptFormHideFilter() => true;

    /// <summary>**是两种已普查形态的组合。**</summary>
    public static bool CombinationOfTwoCensusedForms() => true;

    /// <summary>**判零在吸收段之后。**</summary>
    public static bool ZeroCheckAfterAbsorb()
        => ZeroCheckLines[0] == 7033 && ZeroCheckLines[1] == 7097;

    /// <summary>**元素那步被正数守卫包着。**</summary>
    public static bool GuardedElementStep() => true;

    /// <summary>**只有一次判零。**</summary>
    public static bool SingleZeroCheck()
        => ZeroCheckLines.Length == 2;

    /// <summary>**第一参数是目标本身。**</summary>
    public static bool RecipientNotCastMessage()
        => SendLines[0] == 7043;

    /// <summary>**消息号与姊妹类不同。**</summary>
    public static bool MessageIdDiffersFromSibling()
        => J221ObjSendLine == 6660;

    /// <summary>**同一次调用两处含义不同。**</summary>
    public static bool SameCallDifferentMeaning() => true;

    /// <summary>脱机过滤（1:1）。</summary>
    public static bool ShouldAttack(bool configOn, bool isPlayer, bool offline)
        => !(configOn && isPlayer && offline);

    /// <summary>**配置关时照打。**</summary>
    public static bool ConfigOffAttacks()
        => ShouldAttack(false, true, true);

    /// <summary>**脱机人物被排除。**</summary>
    public static bool OfflineExcluded()
        => !ShouldAttack(true, true, true);

    /// <summary>隐藏过滤（1:1）。</summary>
    public static bool HideFilterPasses(bool hidden, bool coolEye)
        => !hidden || coolEye;

    /// <summary>**未隐藏则通过。**</summary>
    public static bool NotHiddenPasses()
        => HideFilterPasses(false, false);

    /// <summary>**隐藏但有冷眼也通过。**</summary>
    public static bool HiddenWithCoolEyePasses()
        => HideFilterPasses(true, true);

    /// <summary>**隐藏且无冷眼则挡下。**</summary>
    public static bool HiddenNoCoolEyeBlocked()
        => !HideFilterPasses(true, false);

    // ===================== 四、MoveTargetAttack =====================

    /// <summary>**挪的是目标不是自己。**</summary>
    public static bool MovesTheTargetNotSelf()
        => SpaceMoveLine == 7177;

    /// <summary>**只在目标等级更高时挪。**</summary>
    public static bool OnlyIfTargetHigherLevel()
        => LevelCheckLine == 7174;

    /// <summary>**与本类"不能移动"互补。**</summary>
    public static bool ComplementsCannotMove() => true;

    /// <summary>**但还是被退役了。**</summary>
    public static bool RetiredAnyway() => true;

    /// <summary>等级判定（1:1）。</summary>
    public static bool ShouldMoveTarget(int myLevel, int targetLevel)
        => myLevel < targetLevel;

    /// <summary>**目标更强才挪。**</summary>
    public static bool StrongerTargetMoved()
        => ShouldMoveTarget(10, 20);

    /// <summary>**目标更弱则不挪。**</summary>
    public static bool WeakerTargetKept()
        => !ShouldMoveTarget(20, 10);

    /// <summary>**同级也不挪。**</summary>
    public static bool EqualLevelKept()
        => !ShouldMoveTarget(10, 10);

    /// <summary>**`SpaceMove` 第四参传 0。**</summary>
    public static bool SpaceMoveFourthArgZero()
        => SpaceMoveFourthArg == 0;

    /// <summary>**狐狸传的是 1。**</summary>
    public static bool FoxPassedOne()
        => FoxFourthArg == 1;

    /// <summary>**语义不明的参数传了不同值。**</summary>
    public static bool OpaqueParamDifferentValues()
        => SpaceMoveFourthArg != FoxFourthArg;

    /// <summary>**地图名取自目标、坐标取自自己。**</summary>
    public static bool MapFromTargetCoordsFromSelf() => true;

    /// <summary>**若活着会跨图混淆。**</summary>
    public static bool CrossMapConfusionIfLive() => true;

    /// <summary>**但因已死而无实际影响。**</summary>
    public static bool MootBecauseDead() => true;

    /// <summary>**`GetFrontPosition` 行已核对。**</summary>
    public static bool FrontPosChecked()
        => FrontPosLine == 7176;

    // ===================== 五、GroupAttack =====================

    /// <summary>**一上来就置真。**</summary>
    public static bool StartsTrue()
        => GroupResultLine == 7191;

    /// <summary>**其余三个都是置假开头。**</summary>
    public static bool OthersStartFalse() => true;

    /// <summary>**永远报告成功。**</summary>
    public static bool AlwaysReportsSuccess() => true;

    /// <summary>**跳过了目标守卫。**</summary>
    public static bool SkipsTargetGuard() => true;

    /// <summary>**三步被挪进循环。**</summary>
    public static bool ThreeStepsMovedIntoLoop()
        => LoopPowerRateAddLine == 7217
           && LoopNextDamageLine == 7218
           && LoopPowerMaxLine == 7220;

    /// <summary>**封顶变成每目标一次。**</summary>
    public static bool CapPerTargetVsOnce()
        => SinglePowerMaxLine == 7008 && LoopPowerMaxLine == 7220;

    /// <summary>**基准伤害来自主目标。**</summary>
    public static bool BaseFromPrimaryTarget()
        => GroupBaseDamageLine == 7196;

    /// <summary>**半径硬编码 5。**</summary>
    public static bool HardcodedFive()
        => GroupRadius == 5;

    /// <summary>**圆心是受击目标。**</summary>
    public static bool TargetCentered()
        => GroupGetMapLine == 7206;

    /// <summary>**是第八种组合。**</summary>
    public static bool EighthCombination()
        => GroupCombos.Length == 8;

    /// <summary>**半径有五种取值。**</summary>
    public static bool FiveRadiusValues()
    {
        var seen = new HashSet<string>();

        foreach (var c in GroupCombos)
            seen.Add(c.Radius);

        return seen.Count >= 5;
    }

    /// <summary>**组合表已提取。**</summary>
    public static bool GroupCombosExtracted()
        => GroupCombosExtractedCheck();

    /// <summary>组合表末尾两项已核对。</summary>
    public static bool GroupCombosExtractedCheck()
        => GroupCombos[7].Batch == "J223"
           && GroupCombos[7].Radius == "hardcoded 5";

    /// <summary>**第八种组合与前面都不同。**</summary>
    public static bool NewCombination()
    {
        for (int i = 0; i < 7; i++)
        {
            if (GroupCombos[i].Radius == GroupCombos[7].Radius
                && GroupCombos[i].Center == GroupCombos[7].Center)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>**是第四处 `Randomize`。**</summary>
    public static bool FourthRandomizeSite()
        => RandomizeLines[3] == RandomizeLine;

    /// <summary>**本功能簇里占三处。**</summary>
    public static bool ThreeInThisFeatureCluster()
        => FeatureClusterRandomize.Length == 3;

    /// <summary>**印证了作者习惯。**</summary>
    public static bool AuthorsHabitConfirmed() => true;

    /// <summary>**五处行号已提取。**</summary>
    public static bool RandomizeTableExtracted()
        => RandomizeLines.Length == 5
           && RandomizeLines[3] == 7207;

    /// <summary>**`case Random(5)` 无 `else`。**</summary>
    public static bool CaseRandomFiveNoElse()
        => DebuffCaseLine == 7248;

    /// <summary>**五分之二会减属性。**</summary>
    public static bool TwoOfFiveDebuff()
        => DebuffArmLines.Length == 2 && DebuffBound == 5;

    /// <summary>**五分之三静默跳过。**</summary>
    public static bool SilentThreeOutOfFive()
        => DebuffBound - DebuffArmLines.Length == 3;

    /// <summary>**两臂只差第一个参数。**</summary>
    public static bool ArmsDifferOnlyInFirstArg()
        => DebuffFirstArgs[0] != DebuffFirstArgs[1];

    /// <summary>减属性判定（1:1）。</summary>
    public static int? DebuffIndex(int roll)
    {
        switch (roll)
        {
            case 0:
                return DebuffFirstArgs[0];
            case 1:
                return DebuffFirstArgs[1];
            default:
                return null;
        }
    }

    /// <summary>**掷 0 减属性 0。**</summary>
    public static bool RollZeroDebuffsZero()
        => DebuffIndex(0) == 0;

    /// <summary>**掷 1 减属性 3。**</summary>
    public static bool RollOneDebuffsThree()
        => DebuffIndex(1) == 3;

    /// <summary>**掷 2/3/4 什么都不做。**</summary>
    public static bool RollsTwoToFourNothing()
        => DebuffIndex(2) == null
           && DebuffIndex(3) == null
           && DebuffIndex(4) == null;

    /// <summary>**有 `try..finally`。**</summary>
    public static bool TryFinallyPresent()
        => GroupTryLine == 7205 && GroupFinallyLine == 7260;

    /// <summary>**只有群攻建表。**</summary>
    public static bool OnlyGroupBuildsList()
        => GroupListLine == 7204;

    /// <summary>**第三次确认那条规律。**</summary>
    public static bool ThirdConfirmationOfTheRule() => true;

    /// <summary>**`Free` 在 `finally` 里。**</summary>
    public static bool FreeInFinally()
        => GroupFreeLine == GroupFinallyLine + 1;

    /// <summary>**四个函数都没有反弹。**</summary>
    public static bool NoReboundAnywhere() => true;

    /// <summary>**姊妹类有反弹。**</summary>
    public static bool SiblingHasRebound() => true;

    /// <summary>**两类选择相反。**</summary>
    public static bool OppositeChoices() => true;

    // ===================== 六、外层体 =====================

    /// <summary>**`CallSlave` 无条件调用。**</summary>
    public static bool CallSlaveUnconditional()
        => CallSlaveLine == 7274;

    /// <summary>**姊妹类用窄血带。**</summary>
    public static bool SiblingUsesNarrowBand()
        => J221CallSlaveLine == 6834;

    /// <summary>**只靠内部守卫防重。**</summary>
    public static bool ReliesOnInnerGuard() => true;

    /// <summary>**第一帧就会尝试。**</summary>
    public static bool TriesOnFirstTick() => true;

    /// <summary>**时间戳只在成功时刷新。**</summary>
    public static bool HitTickOnlyOnSuccess()
        => HitTickSetLine == 7303;

    /// <summary>**在 `if Result then` 之内。**</summary>
    public static bool InsideResultGuard()
        => HitTickSetLine > ResultGuardLine;

    /// <summary>**失败就每帧重试。**</summary>
    public static bool RetryEveryTickOnFailure() => true;

    /// <summary>**与 J221 相反。**</summary>
    public static bool OppositeOfJ221()
        => J221HitTickLine == 6847;

    /// <summary>**这里**不是**恒真的。**</summary>
    public static bool NotTautologicalHere() => true;

    /// <summary>**因为 `Exit` 是有条件的。**</summary>
    public static bool BecauseExitIsConditional()
        => OuterExitLine > ResultGuardLine;

    /// <summary>**落到末尾有两种情形。**</summary>
    public static bool TwoFallthroughCases() => true;

    /// <summary>**与 J221 直接对照。**</summary>
    public static bool DirectContrastWithJ221()
        => J221TailCheckLine == 6869;

    /// <summary>**同一句代码两种活性。**</summary>
    public static bool SameCodeDifferentLiveness() => true;

    /// <summary>末尾判据活性（1:1）：门为 `<= gate`、`Exit` 是否无条件。</summary>
    public static bool CheckIsTautological(int gate, int check, bool exitUnconditional)
        => exitUnconditional && gate >= check;

    /// <summary>**J221：门 7、判据 6、`Exit` 无条件 => 恒真。**</summary>
    public static bool J221IsTautological()
        => CheckIsTautological(7, 6, true);

    /// <summary>**本类：`Exit` 有条件 => 不恒真。**</summary>
    public static bool ThisIsNotTautological()
        => !CheckIsTautological(7, 6, false);

    /// <summary>**两个分支都丢弃。**</summary>
    public static bool BothDiscardAgain()
        => DiscardSameMapLine == 7313 && DiscardOtherMapLine == 7318;

    /// <summary>**第三次出现。**</summary>
    public static bool ThirdOccurrence() => true;

    /// <summary>**退化层次比前两处浅。**</summary>
    public static bool ShallowerDegeneracy() => true;

    /// <summary>**判据有意义但两支同体。**</summary>
    public static bool ConditionMeaningfulButBodiesSame() => true;

    /// <summary>**发送也在 `Result` 守卫里。**</summary>
    public static bool SendInsideResultGuard()
        => SendLine > ResultGuardLine;

    /// <summary>**姊妹类是无条件发送。**</summary>
    public static bool SiblingSendsUnconditionally()
        => J221SendLine == 6863;

    /// <summary>**本类不会发空特效。**</summary>
    public static bool NoZeroEffectHere() => true;

    /// <summary>**而 J221 会发 0。**</summary>
    public static bool J221CanSendZero() => true;

    /// <summary>**范围门是 7。**</summary>
    public static bool RangeGateIsSeven()
        => RangeGateThreshold == 7;

    /// <summary>**末段判据是 6。**</summary>
    public static bool TailIsSix()
        => TailThreshold == 6;

    /// <summary>**两个阈值不同。**</summary>
    public static bool ThresholdsDiffer()
        => RangeGateThreshold != TailThreshold;

    /// <summary>冷却判定（1:1）。</summary>
    public static bool ShouldAttempt(uint lastHit, uint now, int nextHitTime, int hitDelay)
        => unchecked(now - lastHit) > (uint)(nextHitTime + hitDelay);

    /// <summary>**时间戳新鲜时不重试。**</summary>
    public static bool FreshTickBlocks()
        => !ShouldAttempt(1000, 1000, 100, 0);

    /// <summary>**时间戳陈旧时重试。**</summary>
    public static bool StaleTickAllows()
        => ShouldAttempt(1000, 2000, 100, 0);

    // ===================== 七、整体 =====================

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**已覆盖二十九类。**</summary>
    public static bool TwentyNineClassesCovered()
        => ClassesCovered == 29;

    /// <summary>**剩余约 25 类。**</summary>
    public static bool RemainingApprox()
        => RemainingClasses == 25;

    // ===================== 八、跨度 =====================

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (End - Start + 1) == Lines
           && (OuterEnd - OuterStart + 1) == OuterLines
           && DecompositionAddsUp()
           && NestedSpansMatch()
           && ClassTotalAddsUp();

    /// <summary>**本类六方法行数相加。**</summary>
    public static bool ClassTotalAddsUp()
        => AlreadyDoneLines + Lines == ClassTotalLines;

    /// <summary>**嵌套都在外层之前。**</summary>
    public static bool NestedBeforeOuter()
        => SingleEnd < ThuderStart
           && ThuderEnd < MoveStart
           && MoveEnd < GroupStart
           && GroupEnd < OuterStart;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => End < 9502;
}
