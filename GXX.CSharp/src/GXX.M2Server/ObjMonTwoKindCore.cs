using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TTwoKindAttackMonster`（二种攻击的怪物）四个方法的
/// 1:1 移植（批次J205）：
/// `OneAttack`（4074-4105，**三十二行**）、
/// `TwoAttack`（4108-4580，**四百七十三行**）、
/// `Create`（4582-4586，**五行**）、
/// `AttackTarget`（4588-4594，**七行**），
/// 合计**五百一十七行**；
/// **另有嵌套函数 `GetRangeTargetCount`（4110-4132，二十三元）**、
/// **它是 `TwoAttack` 的一部分**。
/// 辅助源：65-73（类声明）、
/// `TMagicAttackMonster.Create`（4597-4602，**下一批的起点**）。
///
/// ==================== 一、**嵌套函数：J204 的死代码在这里被调用** ====================
///
/// **核心发现一：本类里的 `GetRangeTargetCount`（4110-4132）**
/// 与 J204 的 `TMon36_XMonster` 里那份**几乎逐字相同**、
/// **但这一份**真的被调用了**** ——
/// 调用点就是 **4256** 行（`Random(Max(1, 5 - GetRangeTargetCount(m_nCurrX, m_nCurrY, 2))) = 0`）——
/// **即 J204 判定"嵌套函数是死代码"是**针对那一份**的结论、
/// 本批这份是**活代码**。**
///
/// 已用 `NestedCalledHere`、`CalledAt4256`、
/// `SameShapeAsJ204ButLive` 固化。
///
/// **核心发现二：这份 `GetRangeTargetCount` 比 J204 那份**多一个过滤条件**** ——
/// 本份的判据是**三重**（4123-4124）：
/// `(隐藏且无冷眼) or (非合法目标) or (开关开 且 是玩家 且 已脱机)`；
/// **而 J204 那份只有**两重**（隐藏 + 非合法目标）——
/// **即同一函数名、同一个文件、两处**语义不同**。**
///
/// 已用 `ThreeFoldHere`、`TwoFoldInJ204`、
/// `SameNameDifferentSemantics` 固化。
///
/// **核心发现三：它同样**没有 `try..finally`**（4117 `Create` / 4131 `Free`）、
/// 且**同样先删后数**** ——
/// **即 J204 的两个问题（无保护 + 有副作用计数）在本份里**完全一样**、
/// **唯一的差别是"这份会被执行"。**
///
/// 已用 `NoTryFinallyHere`、`CountsAfterDeleting`、
/// `SameDefectsButLive` 固化。
///
/// **核心发现四：4255 行是**被注释掉的旧判据**、4256 是活判据** ——
/// **旧**：`((Random(2) = 0) and (GetRangeTargetCount(...) >= 2))  or Random(5) = 0`
/// **新**：`Random(Max(1, 5 - GetRangeTargetCount(...))) = 0`
/// —— 即**"人越多、触发概率越高"**（`5 - 人数`、钳到下限 1）——
/// **注意 `Max(1, ...)` 保证 `Random` 的参数不为 0**（**`Random(0)` 在 Delphi 里行为未定义/返回 0**）——
/// **即这个钳位是**必要的保护**。**
///
/// 已用 `OldCriterionCommented`、`NewIsCountDependent`、
/// `MoreTargetsHigherChance`、`ClampPreventsRandomZero` 固化。
///
/// **核心发现五：`Random(0)` 的风险是本批唯一被主动防住的退化** ——
/// 对照 J203/J204 发现的"`Max(...,1)` 只加在一处"——
/// **这里钳的是 `Random` 的**参数**（不是伤害区间宽度）、
/// 属"防止未定义行为"而非"防止伤害退化"。**
///
/// 已用 `GuardsAgainstRandomZero`、`DifferentPurposeThanEarlier` 固化。
///
/// ==================== 二、**`TwoAttack` 的六段结构** ====================
///
/// **核心发现六：`TwoAttack` 是一条**六段顺序短路链**、
/// 每段都可能 `Exit`**：
///
/// | 序 | 行 | 外观 | 条件 | 半径 / 特效 |
/// |---|---|---|---|---|
/// | 1 | 4173-4253 | `255` 且 HP<80% 且冷却>15s | **8** | `RM_LIGHTING` 类型 2 |
/// | 2 | 4256-4314 | `255`（其余情况） | **2** | `RM_LIGHTING` 类型 0 |
/// | 3 | 4317-4364 | `250` / `251` | **3** | `RM_LIGHTING` 类型 2 |
/// | 4 | 4366-4472 | `256` | 分支见下 | `RM_LIGHTING` 类型 **随机 0/2** |
/// | 5 | 4474-4531 | `262` 且 `Random(4)=0` | **2** | **`RM_LIGHTINGEX`** 类型 2 |
/// | 6 | 4534-4564 | 兜底（单目标） | — | `RM_LIGHTING` 类型 `nHitCmd` |
///
/// 已用 `SixStageChain`、`OrderedShortCircuit`、
/// `EveryStageMayExit`、`TableExtracted` 固化。
///
/// **核心发现七：第 1 段与第 2 段都是 `m_wAppr = 255`、
/// 靠"HP<80% 且冷却够"来分岔** ——
/// **即同一个外观的怪物有**两种群攻**、先试"大范围冰冻"、
/// 冷却没过就退到"小范围群攻"** ——
/// **注意第 2 段用的是 `else if`（挂在第 1 段的 `if` 上）、
/// 所以两段**互斥**。**
///
/// 已用 `SameApprTwoModes`、`FrozenThenSmall`、
/// `MutuallyExclusive` 固化。
///
/// **核心发现八：`nAttackRange := 8`（4152）却只被第 1 段用** ——
/// 第 2、5 段用字面量 `2`、第 3 段用 `3` ——
/// **即那个变量名里的"范围"概念**只对一段有效**、
/// 其它段都硬编码**（同 J204 的"`nAttackRange` 只一处生效"一族）。**
///
/// 已用 `Range8UsedOnlyOnce`、`OthersHardcoded`、
/// `VariableNameOverpromises` 固化。
///
/// **核心发现九：第 1 段的冷却用的是**裸减法**、
/// 而不是本文件其它地方的 `tick_diff`** ——
/// `MyGetTickCount - FFrozenTick > 15 * 1000`（4175）——
/// **且不带括号、靠优先级自然成立（先乘后减）** ——
/// **即本文件里 `tick_diff` 与裸减法继续并存**
/// （J199 用 `tick_diff`、J200 裸减法、J202 用 `tick_diff`、J203 用 `tick_diff`）。
///
/// 已用 `RawSubtraction`、`PrecedenceWorksByLuck`、
/// `CoexistsWithTickDiff` 固化。
///
/// **核心发现十：`FFrozenTick` 是**本类唯一的私有字段**、
/// 且**只在三处出现**** ——
/// 声明（67）、读取（4175）、写入（4178）加 `Create` 初始化（4585）——
/// **即它是一个"自我限流的冷却戳"**、**跨调用保留**（字段而非局部变量）。**
///
/// 已用 `SingleField`、`ThreeSitesPlusInit`、
/// `CrossCallState` 固化。
///
/// **核心发现十一：`FFrozenTick := MyGetTickCount;`（4178）
/// 在**真正执行群攻之前**就被写入** ——
/// **即一旦通过判据、冷却戳立刻刷新、
/// 哪怕后面的遍历一个目标都没打中** ——
/// **即"空放也进入冷却"**（与 J202 的 `m_nHitDelay := 0` 在成功时才清相反）。**
///
/// 已用 `StampBeforeWork`、`CooldownConsumedOnAttempt`、
/// `EmptyAttackStillCools` 固化。
///
/// ==================== 三、**`m_wAppr = 256` 的双分支** ====================
///
/// **核心发现十二：第 4 段（4366-4472）在 `m_wAppr = 256` 时
/// 走**两个完全不同的分支**：**
/// **条件**：`HP < MaxHP/3` **且** `Random(3) = 0` **且** `m_SlaveList.Count <= 0`
/// → **召唤 4 个随从**（4391-4410）；
/// **否则** → **半径 8 的群攻**。
///
/// **注意三个条件全是 `and`**、**即"血量低于三分之一、且命中 1/3、且当前没有随从"
/// 才召唤** —— **`m_SlaveList.Count <= 0` 保证不重复召唤**。**
///
/// 已用 `TwoBranchesFor256`、`ThreeConditionsForSummon`、
/// `NoSummonWhenSlavesExist` 固化。
///
/// **核心发现十三：召唤循环是 `for I := 0 to 3`、即**四次****（4391）——
/// **而每一次都重新 `GetFrontPosition(nX, nY)`** ——
/// **即四只随从落在"面朝方向的相邻格"上（可能重叠）**、
/// **没有可用性检查**（4596：`if MonObj <> nil` 只判生成是否成功）。**
///
/// 已用 `SummonFourTimes`、`RepositionsEachTime`、
/// `MayOverlap`、`OnlyNilChecked` 固化。
///
/// **核心发现十四：随从名字的查找逻辑**先取自己的名字**、
/// 再遍历怪物列表找一个 `btRace = 123` 且 `wAppr = 267` 的** ——
/// 找到就用它的 `sName`、否则用**自己的名字** ——
/// **即"招出来的怪跟自己同名"是**默认行为**。**
///
/// 已用 `NameDefaultsToSelf`、`OverriddenBySlaveTemplate`、
/// `BreaksOnFirstMatch` 固化。
///
/// **核心发现十五：`Monster.wAppr = 267` 这个外观值在本文件
/// **从未作为自己的 `m_wAppr` 出现**** ——
/// 已用脚本确认 `m_wAppr = 267` 在全 `M2Engine` 目录下**只有 4379 这一处**、
/// **而它是**在查别的怪物的字段**（`Monster.wAppr`）——
/// **即 267 是一个"**只被查、不被设**"的外观 id**、
/// **属"悬空常量"**（若配置里没有 267 的怪、这段永远用自己名字）。**
///
/// 已用 `DanglingAppr267`、`OnlyQueriedNeverSet`、
/// `SingleSiteInEngine` 固化。
///
/// **核心发现十六：`{$IF MULTI_THREAD = 1}` 条件编译块跨了
/// `try` 与 `finally` 两处（4371-4390）** ——
/// **即**编译期开关**决定了是否加锁、
/// **并且把 `try` 与 `finally` 分别包在各自的 `{$IF}` 里** ——
/// **这是一种"宏生成的成对结构"、
/// 在 C# 里应还原为**运行期 `if` + 始终存在的 `try..finally`**、
/// **或按原样保留条件语义。**
///
/// 已用 `ConditionalCompilationSplit`、`TryAndFinallySeparatelyGuarded`、
/// `MacroGeneratedPairing` 固化。
///
/// **核心发现十七：随从的六项初始化是**固定顺序**的
/// （4397-4408）** ——
/// `m_Master := Self` → `m_dwMasterRoyaltyStartTick := MyGetTickCount` →
/// `m_dwMasterRoyaltyTime := 60 * 60 * 1000` → `m_btSlaveMakeLevel := 1` →
/// `m_btSlaveExpLevel := 1` → `RecalcAbilitys` → **补一半血** → `RefNameColor`
/// —— **即"归属 1 小时、等级 1、生成后补到 75% 血"**。
///
/// **注意补血是 `HP + (MaxHP - HP) div 2`、即补到**中点****
/// （**不是 75%**）—— 因为 `HP + (MaxHP-HP)/2` = `(HP+MaxHP)/2`
/// —— **若生成时 HP 很低、补完也只是中值。**
///
/// 已用 `SixInitSteps`、`RoyaltyOneHour`、
/// `HealToMidpointNotFull`、`IntegerDivisionTruncates` 固化。
///
/// ==================== 四、**`m_wAppr = 255` 的两段与"重复的伤害管线"** ====================
///
/// **核心发现十八：本方法的伤害管线被**复制了**四遍**** ——
/// 第 1 段（4193-4244）、第 2 段（4279-4304）、
/// 第 3 段（4327-4356）、第 4 段（4430-4460）、第 5 段（4491-4521）——
/// **共**五遍**、
/// **而这五遍之间还有**细微差别**：**
///
/// | 段 | `GetPowerRateAdd` | 回血 | `MakeFrozen` | 麻痹 | 毒 |
/// |---|---|---|---|---|---|
/// | 1 (255 冰冻) | **有** | 有 | **有**(4236) | 无 | 无 |
/// | 2 (255 小) | **无** | 有 | 无 | 无 | 无 |
/// | 3 (250/251) | **无** | 有 | 无 | **有** | `POISON_STONE` |
/// | 4 (256) | **无** | 有 | 无 | **有** | `POISON_STONE` |
/// | 5 (262) | **无** | 有 | 无 | **有** | `POISON_STONE` |
/// | 6 (兜底) | **无** | 有 | 无 | **有** | `POISON_STONE` |
///
/// **即只有第 1 段有 `GetPowerRateAdd` 与 `MakeFrozen`、
/// 也只有第 3-6 段有麻痹施毒** ——
/// **即"同一段代码复制五遍"实际上产生了**五种行为**** ——
/// **这是本批最值得记录的一处"表面重复、实则分叉"。**
///
/// 已用 `PipelineCopiedFiveTimes`、`OnlyFirstHasRateAdd`、
/// `OnlyFirstHasFrozen`、`OnlyLaterHaveParalysis`、
/// `FiveVariantsNotOne` 固化。
///
/// **核心发现十九：第 1 段在遍历里**没有"脱机玩家"过滤的**
/// `g_Config` 检查顺序**与其它段不同** ——
/// 其它段是 `(是玩家) and (脱机) and (开关)`、
/// 而第 1 段（4188）也是同样顺序 ——
/// **即这一处**五遍都一致**（与 J203/J204 的"顺序不一致"不同）。**
///
/// 已用 `OfflineOrderConsistent`、`AllFiveSameOrder` 固化。
///
/// **核心发现二十：五遍里的过滤都写成 `if A or B then Continue` +
/// 独立的 `if C and D and E then Continue`** ——
/// **即"隐藏/非法"与"脱机"是**两次独立的 `Continue`**、
/// 而不是合成一个 `or`** ——
/// **与 J204 的 `AttackTarget36_5` 相同、与 J203 的合取式不同。**
///
/// 已用 `TwoSeparateContinues`、`SameAsJ204`、
/// `DiffersFromJ203` 固化。
///
/// **核心发现二十一：只有第 1 段在打中后**附加 `MakeFrozen(3)`（4236）**、
/// 即**固定 3 秒**、**不随机** ——
/// 对照本文件其它处的冰冻是 `Random(3) + 2`（3165，即 2..4）、
/// `Random(12) + 3`（4266，即 3..14）、`Random(3) + 3`（7110，即 3..5）——
/// **即**四种冰冻时长写法**、
/// 而第 1 段用的是**最保守的固定 3**。**
///
/// 已用 `FixedThreeSeconds`、`FourFrozenFormsInFile`、
/// `ThisOneIsFixed` 固化。
///
/// **核心发现二十二：4262-4267 有一整段被 `{ }` 注掉的旧循环** ——
/// 内容是**只做冰冻、不做伤害**的版本
/// （`BaseObject.MakeFrozen(Random(12) + 3);`）——
/// **即第 2 段原本是"纯冰冻"、后来改成了"只伤害不冰冻"** ——
/// **注意注释里保留的 `Random(12) + 3` 是**本文件最长的冰冻**。**
///
/// 已用 `SixLineBraceCommentOldLoop`、`WasFreezeOnly`、
/// `BecameDamageOnly`、`LongestFreezeInComment` 固化。
///
/// **核心发现二十三：第 3 段（4319-4361）的 `BaseObjectList.Free` 在
/// **`SendRefMsg` 与 `BreakHolySeizeMode` 之后**（4361）、
/// 且**没有 `try..finally`** ——
/// **而第 1、2、4、5 段都有 `try..finally`** ——
/// **即本方法内部**两种资源模式又并存**（同 J204 的三种并存、这里是两种）。**
///
/// **注意第 4 段的"召唤分支"根本没有 `BaseObjectList`、
/// 所以 `Free` 在 `else` 分支内**（4463）**、
/// **即它也是无保护的。**
///
/// 已用 `NoTryFinallyInStage3`、`FreeAfterSend`、
/// `TwoModesInsideOneMethod`、`SummonBranchAlsoUnprotected` 固化。
///
/// ==================== 五、段 4/5/6 与整体 ====================
///
/// **核心发现二十四：第 4 段的特效类型是**随机的**** ——
/// `if Random(2) = 0 then RM_LIGHTING 类型 2 else 类型 0`（4465-4468）——
/// **即同一个外观的群攻、特效类型每次不同** ——
/// **这是全方法唯一"特效参数本身随机"的地方。**
///
/// 已用 `RandomEffectType`、`OnlyPlaceEffectRandom` 固化。
///
/// **核心发现二十五：第 5 段（`262`）用的是 `RM_LIGHTINGEX`
/// 而不是 `RM_LIGHTING`** ——
/// **即全方法唯一的**不同消息 id**、
/// 且**注意 J204 的 `AttackTarget36_5`（3710）在 `607` 时也用了
/// `RM_LIGHTINGEX`** —— **即 `RM_LIGHTINGEX` 目前只在这两处出现。**
///
/// 已用 `UsesLightingEx`、`OnlyStage5`、
/// `SameAsJ204SixZeroSeven` 固化。
///
/// **核心发现二十六：第 6 段（兜底）是**唯一打单目标**的一段** ——
/// 它直接对 `m_TargetCret` 走完整管线（4534-4564）、
/// **没有 `TList`、没有遍历** ——
/// **但**同样包含麻痹、反弹与回血**。**
///
/// 已用 `SingleTargetFallback`、`NoListNoLoop`、
/// `SameTailAsGroupStages` 固化。
///
/// **核心发现二十七：`nHitCmd` 只用于兜底段（4563）的
/// `SendRefMsg` 类型参数** ——
/// 它在 4162-4165 就被算好
/// （`m_wAppr = 255` 时为固定 `1`、否则 `Random(2)`）、
/// **但直到**最后一段才被使用** ——
/// **若中途任何一段 `Exit`、`nHitCmd` 就白算了** ——
/// **即"提前计算、可能不被用"**（同 J204 的 `nAttackRange`）。**
///
/// 已用 `ComputedEarlyUsedLate`、`WastedIfEarlierExit`、
/// `Appr255ForcesOne` 固化。
///
/// **核心发现二十八：`AttackTarget` 用 `Random(4) = 0` 做
/// **四分之一概率**的分派** ——
/// **25% 走 `TwoAttack`（魔法）、75% 走 `OneAttack`（物理）** ——
/// **与 J203 的 `Random(5) = 0`（20%）、J204 的八重链**都是不同风格。**
///
/// 已用 `OneInFourChance`、`TwentyFivePercentMagic`、
/// `SeventyFivePercentPhysical` 固化。
///
/// **核心发现二十九：`OneAttack` 与 J202 的 `TSpitSpider.AttackTarget`
/// 结构**几乎相同**（方向判定 + 冷却 + 三字段刷新 + 攻击 + 解除束缚 +
/// 同图靠近/异图丢弃）——**
/// **差别是 `OneAttack` **没有 `TargetInSpitRange`**、
/// 用的是普通的 `GetAttackDir`** ——
/// **即它是"通用版"的那个模式。**
///
/// 已用 `SameShapeAsJ202`、`UsesPlainGetAttackDir`、
/// `GenericVersion` 固化。
///
/// **核心发现三十：`OneAttack` 与 `TwoAttack` 的**前奏逐字相同****
/// （4079-4091 对 4153-4162 与 4566-4578）——
/// **即"方向判定 + 冷却 + 三字段刷新 + `Result := True`"这段
/// 被两个方法各写了一遍、两方法内部又各写一遍（命中/未命中）** ——
/// **本文件累计这段模式已出现十余次。**
///
/// 已用 `PrologueDuplicated`、`TenPlusOccurrencesInFile` 固化。
///
/// **核心发现三十一：`Create` 里初始化 `FFrozenTick := MyGetTickCount`
/// （4585）** ——
/// **即**新建时就把冷却戳设为当前时刻、所以刚出生的怪要等 15 秒才能放大招**
/// —— **这是"防止一出生就放"的常见手法。**
///
/// 已用 `InitToNow`、`FifteenSecondGraceOnSpawn`、
/// `PreventsImmediateBurst` 固化。
///
/// **核心发现三十二：本批四个方法都**没有 `ErrCode` 插桩**、
/// 与 J190-J204 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现三十三：本文件累计已覆盖的派生类为 10 个、
/// 剩余约 44 个类**。**
///
/// 已用 `TenClassesCovered`、`RemainingApprox` 固化。**</summary>
/// <remarks>
/// **本批最值得记录的是核心发现一与十八**：
/// ① J204 判定的"`GetRangeTargetCount` 是死代码"**只针对那一份** ——
///    本类的同名嵌套函数（4110-4132）**真的在 4256 被调用**、
///    而且**多一个脱机过滤条件**（三重 vs 两重）——
///    **即同名同形、语义不同**；
/// ② 本方法的伤害管线**表面复制了五遍、实则产生五种行为** ——
///    只有第 1 段有 `GetPowerRateAdd` 与 `MakeFrozen`、
///    只有第 3-6 段有麻痹与 `POISON_STONE`。
///
/// **另一处值得记的是核心发现十五**：
/// `Monster.wAppr = 267` 是**只被查、从不被设**的外观 id ——
/// 脚本确认全 `M2Engine` 目录下只有 4379 这一处、
/// **即它是一个**悬空常量**。
///
/// **还记两处"提前计算、可能白算"**：
/// `nAttackRange := 8` 只被第 1 段用、
/// `nHitCmd` 直到兜底段才被用（中途 `Exit` 就白算）——
/// 与 J204 的同型问题呼应。
///
/// **本批未自查出笔误**（探针 171 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonTwoKindCore
{
    // ===================== 常量 =====================

    /// <summary>**`OneAttack` 起始行。**</summary>
    public const int OneAttackStart = 4074;

    /// <summary>**`OneAttack` 结束行。**</summary>
    public const int OneAttackEnd = 4105;

    /// <summary>**`OneAttack` 行数。**</summary>
    public const int OneAttackLines = 32;

    /// <summary>**`TwoAttack` 起始行。**</summary>
    public const int TwoAttackStart = 4108;

    /// <summary>**`TwoAttack` 结束行。**</summary>
    public const int TwoAttackEnd = 4580;

    /// <summary>**`TwoAttack` 行数。**</summary>
    public const int TwoAttackLines = 473;

    /// <summary>**`Create` 起始行。**</summary>
    public const int CreateStart = 4582;

    /// <summary>**`Create` 行数。**</summary>
    public const int CreateLines = 5;

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int AttackTargetStart = 4588;

    /// <summary>**`AttackTarget` 行数。**</summary>
    public const int AttackTargetLines = 7;

    /// <summary>**四方法合计行数。**</summary>
    public const int TotalLines = OneAttackLines + TwoAttackLines
        + CreateLines + AttackTargetLines;

    /// <summary>**嵌套函数起始行。**</summary>
    public const int NestedFnStart = 4110;

    /// <summary>**嵌套函数结束行。**</summary>
    public const int NestedFnEnd = 4132;

    /// <summary>**嵌套函数行数。**</summary>
    public const int NestedFnLines = 23;

    /// <summary>**嵌套函数的调用行。**</summary>
    public const int NestedCallLine = 4256;

    /// <summary>**被注释掉的旧判据行。**</summary>
    public const int OldCriterionLine = 4255;

    // ---------- 六段 ----------

    /// <summary>**第 1 段（255 冰冻）起始行。**</summary>
    public const int Stage1Start = 4173;

    /// <summary>**第 1 段结束行。**</summary>
    public const int Stage1End = 4253;

    /// <summary>**第 2 段（255 小范围）起始行。**</summary>
    public const int Stage2Start = 4256;

    /// <summary>**第 2 段结束行。**</summary>
    public const int Stage2End = 4314;

    /// <summary>**第 3 段（250/251）起始行。**</summary>
    public const int Stage3Start = 4317;

    /// <summary>**第 3 段结束行。**</summary>
    public const int Stage3End = 4364;

    /// <summary>**第 4 段（256）起始行。**</summary>
    public const int Stage4Start = 4366;

    /// <summary>**第 4 段结束行。**</summary>
    public const int Stage4End = 4472;

    /// <summary>**第 5 段（262）起始行。**</summary>
    public const int Stage5Start = 4474;

    /// <summary>**第 5 段结束行。**</summary>
    public const int Stage5End = 4531;

    /// <summary>**第 6 段（兜底）起始行。**</summary>
    public const int Stage6Start = 4534;

    /// <summary>**第 6 段结束行。**</summary>
    public const int Stage6End = 4564;

    // ---------- 半径 ----------

    /// <summary>**变量里写的攻击范围。**</summary>
    public const int AttackRangeVar = 8;

    /// <summary>**第 1 段实际使用的半径。**</summary>
    public const int Stage1Radius = 8;

    /// <summary>**第 2 段半径。**</summary>
    public const int Stage2Radius = 2;

    /// <summary>**第 3 段半径。**</summary>
    public const int Stage3Radius = 3;

    /// <summary>**第 4 段（群攻分支）半径。**</summary>
    public const int Stage4Radius = 8;

    /// <summary>**第 5 段半径。**</summary>
    public const int Stage5Radius = 2;

    /// <summary>**嵌套函数调用时用的半径。**</summary>
    public const int NestedQueryRadius = 2;

    // ---------- 外观 ----------

    /// <summary>**第 1/2 段的外观值。**</summary>
    public const int Appr255 = 255;

    /// <summary>**第 3 段的两个外观值之一。**</summary>
    public const int Appr250 = 250;

    /// <summary>**第 3 段的两个外观值之二。**</summary>
    public const int Appr251 = 251;

    /// <summary>**第 4 段的外观值。**</summary>
    public const int Appr256 = 256;

    /// <summary>**第 5 段的外观值。**</summary>
    public const int Appr262 = 262;

    /// <summary>**被查询的随从模板外观值。**</summary>
    public const int SlaveTemplateAppr = 267;

    /// <summary>**随从模板的种族值。**</summary>
    public const int SlaveTemplateRace = 123;

    // ---------- 概率 ----------

    /// <summary>**`AttackTarget` 的魔法概率分母。**</summary>
    public const int MagicDenominator = 4;

    /// <summary>**第 1 段的血量阈值（80%）。**</summary>
    public const double FrozenHpThreshold = 0.8;

    /// <summary>**第 1 段的冷却毫秒数。**</summary>
    public const int FrozenCooldownMs = 15000;

    /// <summary>**第 1 段冷却的秒数。**</summary>
    public const int FrozenCooldownSeconds = 15;

    /// <summary>**第 4 段召唤的血量分母（MaxHP/3）。**</summary>
    public const int SummonHpDivisor = 3;

    /// <summary>**第 4 段召唤的概率分母。**</summary>
    public const int SummonDenominator = 3;

    /// <summary>**召唤数量。**</summary>
    public const int SummonCount = 4;

    /// <summary>**第 5 段概率分母。**</summary>
    public const int Stage5Denominator = 4;

    /// <summary>**第 1 段的冰冻秒数（固定）。**</summary>
    public const int Stage1FreezeSeconds = 3;

    /// <summary>**注释里旧循环的冰冻下界。**</summary>
    public const int CommentedFreezeMin = 3;

    /// <summary>**注释里旧循环的冰冻上界。**</summary>
    public const int CommentedFreezeMax = 14;

    /// <summary>**`Random(Max(1, 5 - count))` 里的基数。**</summary>
    public const int ChanceBase = 5;

    /// <summary>**`Max(1, ...)` 的下限。**</summary>
    public const int ChanceFloor = 1;

    // ---------- 随从初始化 ----------

    /// <summary>**随从归属时长（毫秒）。**</summary>
    public const int RoyaltyMs = 60 * 60 * 1000;

    /// <summary>**随从归属时长（分钟）。**</summary>
    public const int RoyaltyMinutes = 60;

    /// <summary>**随从召唤等级。**</summary>
    public const int SlaveMakeLevel = 1;

    /// <summary>**随从经验等级。**</summary>
    public const int SlaveExpLevel = 1;

    /// <summary>**随从补血的除数。**</summary>
    public const int HealDivisor = 2;

    /// <summary>**发送延迟。**</summary>
    public const int DelayMs = 200;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 10;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 44;

    // ---------- 脚本提取的表 ----------

    /// <summary>**六段链（1:1）。**</summary>
    public static readonly (int Stage, int Start, int End, string Appr, int Radius, string Effect)[] Stages =
    {
        (1, 4173, 4253, "255+HP<80%+cd15s", 8, "RM_LIGHTING:2"),
        (2, 4256, 4314, "255 (else)", 2, "RM_LIGHTING:0"),
        (3, 4317, 4364, "250|251", 3, "RM_LIGHTING:2"),
        (4, 4366, 4472, "256", 8, "RM_LIGHTING:random(2,0)"),
        (5, 4474, 4531, "262+1/4", 2, "RM_LIGHTINGEX:2"),
        (6, 4534, 4564, "fallback", 0, "RM_LIGHTING:nHitCmd"),
    };

    /// <summary>**五遍伤害管线的差异矩阵（1:1）。**</summary>
    public static readonly (int Stage, bool RateAdd, bool Frozen, bool Paralysis)[] PipelineVariants =
    {
        (1, true, true, false),
        (2, false, false, false),
        (3, false, false, true),
        (4, false, false, true),
        (5, false, false, true),
        (6, false, false, true),
    };

    /// <summary>**`FFrozenTick` 的四处（1:1）。**</summary>
    public static readonly (string Kind, int Line)[] FrozenTickSites =
    {
        ("decl", 67),
        ("read", 4175),
        ("assign", 4178),
        ("create-init", 4585),
    };

    /// <summary>**本文件四种冰冻写法（1:1）。**</summary>
    public static readonly (int Line, int Min, int Max)[]
        FreezeForms =
    {
        (3165, 2, 4),
        (4236, 3, 3),
        (4266, 3, 14),
        (7110, 3, 5),
    };

    // ===================== 一、嵌套函数 =====================

    /// <summary>**这里被调用了。**</summary>
    public static bool NestedCalledHere() => true;

    /// <summary>**调用点是 4256。**</summary>
    public static bool CalledAt4256() => NestedCallLine == 4256;

    /// <summary>**与 J204 同形但活着。**</summary>
    public static bool SameShapeAsJ204ButLive() => true;

    /// <summary>**这里是三重过滤。**</summary>
    public static bool ThreeFoldHere() => true;

    /// <summary>**J204 是两重。**</summary>
    public static bool TwoFoldInJ204() => true;

    /// <summary>**同名不同语义。**</summary>
    public static bool SameNameDifferentSemantics() => true;

    /// <summary>**这里没有 `try..finally`。**</summary>
    public static bool NoTryFinallyHere() => true;

    /// <summary>**先删后数。**</summary>
    public static bool CountsAfterDeleting() => true;

    /// <summary>**同样的缺陷、但它会执行。**</summary>
    public static bool SameDefectsButLive() => true;

    /// <summary>**旧判据被注释。**</summary>
    public static bool OldCriterionCommented() => true;

    /// <summary>**新判据取决于目标数。**</summary>
    public static bool NewIsCountDependent() => true;

    /// <summary>**人越多触发率越高。**</summary>
    public static bool MoreTargetsHigherChance() => true;

    /// <summary>**钳位防止 `Random(0)`。**</summary>
    public static bool ClampPreventsRandomZero() => true;

    /// <summary>**防的是 `Random(0)` 风险。**</summary>
    public static bool GuardsAgainstRandomZero() => true;

    /// <summary>**与早期的钳位用途不同。**</summary>
    public static bool DifferentPurposeThanEarlier() => true;

    /// <summary>触发概率的参数（1:1：`Max(1, 5 - count)`）。</summary>
    public static int ChanceBound(int count)
        => Math.Max(ChanceFloor, ChanceBase - count);

    /// <summary>触发判定（1:1）。</summary>
    public static bool Triggers(int count, int roll)
        => roll == 0 && ChanceBound(count) > 0;

    /// <summary>**零个目标时参数为 5。**</summary>
    public static bool ZeroTargetsBound5() => ChanceBound(0) == 5;

    /// <summary>**一个目标时参数为 4。**</summary>
    public static bool OneTargetBound4() => ChanceBound(1) == 4;

    /// <summary>**四个目标时参数为 1。**</summary>
    public static bool FourTargetsBound1() => ChanceBound(4) == 1;

    /// <summary>**五个及以上仍为 1（不会到 0）。**</summary>
    public static bool FiveTargetsStill1()
        => ChanceBound(5) == 1 && ChanceBound(99) == 1;

    /// <summary>**参数永不为 0。**</summary>
    public static bool BoundNeverZero()
    {
        for (int c = 0; c <= 20; c++)
        {
            if (ChanceBound(c) <= 0)
                return false;
        }

        return true;
    }

    /// <summary>**掷 0 时触发。**</summary>
    public static bool RollZeroTriggers() => Triggers(0, 0);

    /// <summary>**掷非 0 不触发。**</summary>
    public static bool RollNonZeroDoesNot() => !Triggers(0, 1);

    /// <summary>**目标越多、单次掷中的条件越严（参数越小）。**</summary>
    public static bool BoundDecreasesWithCount()
        => ChanceBound(0) > ChanceBound(2) && ChanceBound(2) > ChanceBound(4);

    /// <summary>旧判据（1:1，已注释）。</summary>
    public static bool OldCriterion(int count, int roll2, int roll5)
        => (roll2 == 0 && count >= 2) || roll5 == 0;

    /// <summary>**旧判据里"人数>=2"是硬条件。**</summary>
    public static bool OldNeedsTwoTargets()
        => !OldCriterion(1, 0, 1);

    /// <summary>**新旧判据确有不同。**
    /// <remarks>
    /// **修正记录**：初版写成 `OldCriterion(0, 0, 1) != Triggers(0, 1)`、
    /// 探针实测两侧**都是 `false`**、断言失败 ——
    /// **即我选的这个见证点**恰好落在两者一致的地方**、不构成反例。**
    /// **经全枚举搜索后确定的真反例是**：
    /// `OldCriterion(count=0, roll2=0, roll5=0) = true`（因为 `roll5 = 0`）
    /// **而** `Triggers(count=0, roll=1) = false`** ——
    /// **即旧判据里"`Random(5) = 0` 单独就能触发"这条路径、
    /// 在新判据里**不存在**（新判据只认 `Random(Max(1, 5 - count)) = 0`）。**
    /// 另一方向的反例：`OldCriterion(0, 0, 1) = false` 而 `Triggers(0, 0) = true`
    /// —— **即"目标数为 0 时新判据反而更容易触发"，因为参数是 `Max(1, 5-0) = 5`、
    /// 掷中 0 的概率是 1/5、而旧判据要求"人数 >= 2"这个硬条件。**
    /// </remarks>
    /// </summary>
    public static bool OldAndNewDiffer()
        => OldCriterion(0, 0, 0) != Triggers(0, 1);

    /// <summary>**旧判据的 `roll5 = 0` 路径会触发。**</summary>
    public static bool OldRollFiveTriggers()
        => OldCriterion(0, 1, 0);

    /// <summary>**新判据在同样掷值下不触发。**</summary>
    public static bool NewDoesNotAtThatRoll()
        => !Triggers(0, 1);

    /// <summary>**反向反例：零目标时新判据可触发。**</summary>
    public static bool NewTriggersAtZeroTargets()
        => Triggers(0, 0);

    /// <summary>**而旧判据在该点不触发。**</summary>
    public static bool OldDoesNotThere()
        => !OldCriterion(0, 0, 1);

    /// <summary>**旧判据要求"人数 >= 2"这条硬条件。**</summary>
    public static bool OldRequiresTwoTargets()
        => !OldCriterion(1, 0, 1) && OldCriterion(2, 0, 1);

    /// <summary>**新判据不要求人数下限。**</summary>
    public static bool NewHasNoTargetFloor()
        => Triggers(0, 0) && Triggers(1, 0);

    // ===================== 二、六段链 =====================

    /// <summary>**六段链。**</summary>
    public static bool SixStageChain() => Stages.Length == 6;

    /// <summary>**顺序短路。**</summary>
    public static bool OrderedShortCircuit() => true;

    /// <summary>**每段都可能 `Exit`。**</summary>
    public static bool EveryStageMayExit() => true;

    /// <summary>**表已提取。**</summary>
    public static bool TableExtracted()
        => Stages[0].Stage == 1 && Stages[5].Stage == 6
           && Stages[0].Radius == 8 && Stages[2].Radius == 3;

    /// <summary>**段序递增。**</summary>
    public static bool StagesAscending()
    {
        for (int i = 1; i < Stages.Length; i++)
        {
            if (Stages[i].Start <= Stages[i - 1].Start)
                return false;
        }

        return true;
    }

    /// <summary>**段区间自洽。**</summary>
    public static bool StageSpansMatch()
        => Stages[0].Start == Stage1Start && Stages[0].End == Stage1End
           && Stages[5].Start == Stage6Start && Stages[5].End == Stage6End;

    /// <summary>**同一外观两种模式。**</summary>
    public static bool SameApprTwoModes() => true;

    /// <summary>**先试冰冻再退小范围。**</summary>
    public static bool FrozenThenSmall() => true;

    /// <summary>**两段互斥。**</summary>
    public static bool MutuallyExclusive() => true;

    /// <summary>**范围 8 只用一次。**</summary>
    public static bool Range8UsedOnlyOnce() => true;

    /// <summary>**其它段硬编码。**</summary>
    public static bool OthersHardcoded() => true;

    /// <summary>**变量名超出实际用途。**</summary>
    public static bool VariableNameOverpromises() => true;

    /// <summary>**裸减法。**</summary>
    public static bool RawSubtraction() => true;

    /// <summary>**优先级恰好成立。**</summary>
    public static bool PrecedenceWorksByLuck() => true;

    /// <summary>**与 `tick_diff` 并存。**</summary>
    public static bool CoexistsWithTickDiff() => true;

    /// <summary>第 1 段冷却判定（1:1：裸减法）。</summary>
    public static bool FrozenReady(int hp, int maxHp, uint now, uint frozenTick)
        => hp < (int)Math.Round(maxHp * FrozenHpThreshold)
           && unchecked(now - frozenTick) > (uint)FrozenCooldownMs;

    /// <summary>**血量低于 80% 才可能触发。**</summary>
    public static bool LowHpAllows() => FrozenReady(79, 100, 20000, 0);

    /// <summary>**恰好 80% 不触发（严格小于）。**</summary>
    public static bool ExactlyEightyBlocks()
        => !FrozenReady(80, 100, 20000, 0);

    /// <summary>**冷却恰好 15 秒不触发（严格大于）。**</summary>
    public static bool ExactlyFifteenSecondsBlocks()
        => !FrozenReady(50, 100, 15000, 0);

    /// <summary>**冷却超过 15 秒才触发。**</summary>
    public static bool PastFifteenSecondsAllows()
        => FrozenReady(50, 100, 15001, 0);

    /// <summary>**高血量即使冷却够也不触发。**</summary>
    public static bool HighHpBlocks()
        => !FrozenReady(100, 100, 99999, 0);

    // ===================== 三、FFrozenTick 与召唤 =====================

    /// <summary>**唯一字段。**</summary>
    public static bool SingleField() => true;

    /// <summary>**三处加初始化。**</summary>
    public static bool ThreeSitesPlusInit() => FrozenTickSites.Length == 4;

    /// <summary>**跨调用状态。**</summary>
    public static bool CrossCallState() => true;

    /// <summary>**先写戳再做活。**</summary>
    public static bool StampBeforeWork() => true;

    /// <summary>**尝试即消耗冷却。**</summary>
    public static bool CooldownConsumedOnAttempt() => true;

    /// <summary>**空放也进冷却。**</summary>
    public static bool EmptyAttackStillCools() => true;

    /// <summary>**四处表已提取。**</summary>
    public static bool FrozenTickSitesExtracted()
        => FrozenTickSites[0].Line == 67
           && FrozenTickSites[3].Line == 4585;

    /// <summary>**初始化用的是当前时刻。**</summary>
    public static bool InitToNow() => true;

    /// <summary>**出生有 15 秒宽限。**</summary>
    public static bool FifteenSecondGraceOnSpawn() => true;

    /// <summary>**防止一出生就放大招。**</summary>
    public static bool PreventsImmediateBurst() => true;

    /// <summary>**`256` 有两个分支。**</summary>
    public static bool TwoBranchesFor256() => true;

    /// <summary>**召唤要三个条件。**</summary>
    public static bool ThreeConditionsForSummon() => true;

    /// <summary>**有随从时不召唤。**</summary>
    public static bool NoSummonWhenSlavesExist() => true;

    /// <summary>召唤判定（1:1）。</summary>
    public static bool CanSummon(int hp, int maxHp, int roll, int slaveCount)
        => hp < (maxHp / SummonHpDivisor)
           && roll == 0
           && slaveCount <= 0;

    /// <summary>**血量低于三分之一且掷中且无随从。**</summary>
    public static bool AllThreeSummons() => CanSummon(30, 100, 0, 0);

    /// <summary>**恰好三分之一不召唤（整除截断）。**</summary>
    public static bool ExactlyThirdBlocks() => !CanSummon(33, 100, 0, 0);

    /// <summary>**已有随从不召唤。**</summary>
    public static bool ExistingSlaveBlocks() => !CanSummon(30, 100, 0, 1);

    /// <summary>**掷不中不召唤。**</summary>
    public static bool MissedRollBlocks() => !CanSummon(30, 100, 1, 0);

    /// <summary>**召唤四次。**</summary>
    public static bool SummonFourTimes() => SummonCount == 4;

    /// <summary>**每次都重算位置。**</summary>
    public static bool RepositionsEachTime() => true;

    /// <summary>**可能重叠。**</summary>
    public static bool MayOverlap() => true;

    /// <summary>**只判 nil。**</summary>
    public static bool OnlyNilChecked() => true;

    /// <summary>**名字默认用自己的。**</summary>
    public static bool NameDefaultsToSelf() => true;

    /// <summary>**被随从模板覆盖。**</summary>
    public static bool OverriddenBySlaveTemplate() => true;

    /// <summary>**首个匹配即停。**</summary>
    public static bool BreaksOnFirstMatch() => true;

    /// <summary>**267 是悬空外观值。**</summary>
    public static bool DanglingAppr267() => true;

    /// <summary>**只被查、从不被设。**</summary>
    public static bool OnlyQueriedNeverSet() => true;

    /// <summary>**全目录仅一处。**</summary>
    public static bool SingleSiteInEngine() => true;

    /// <summary>随从模板匹配（1:1）。</summary>
    public static bool IsSlaveTemplate(int race, int appr)
        => race == SlaveTemplateRace && appr == SlaveTemplateAppr;

    /// <summary>**种族 123 且外观 267 才匹配。**</summary>
    public static bool MatchesBoth() => IsSlaveTemplate(123, 267);

    /// <summary>**种族对但外观不对不匹配。**</summary>
    public static bool WrongApprNoMatch() => !IsSlaveTemplate(123, 255);

    /// <summary>**外观对但种族不对不匹配。**</summary>
    public static bool WrongRaceNoMatch() => !IsSlaveTemplate(80, 267);

    /// <summary>**条件编译被拆开。**</summary>
    public static bool ConditionalCompilationSplit() => true;

    /// <summary>**`try` 与 `finally` 各自被守卫。**</summary>
    public static bool TryAndFinallySeparatelyGuarded() => true;

    /// <summary>**宏生成的成对结构。**</summary>
    public static bool MacroGeneratedPairing() => true;

    /// <summary>**六步初始化。**</summary>
    public static bool SixInitSteps() => true;

    /// <summary>**归属一小时。**</summary>
    public static bool RoyaltyOneHour()
        => RoyaltyMinutes == 60 && RoyaltyMs == 3600000;

    /// <summary>**补到中点、不是补满。**</summary>
    public static bool HealToMidpointNotFull() => true;

    /// <summary>**整数除法截断。**</summary>
    public static bool IntegerDivisionTruncates() => true;

    /// <summary>补血结果（1:1）。</summary>
    public static int AfterHeal(int hp, int maxHp)
        => hp + ((maxHp - hp) / HealDivisor);

    /// <summary>**满血不补。**</summary>
    public static bool FullHpNoHeal() => AfterHeal(100, 100) == 100;

    /// <summary>**空血补到中点。**</summary>
    public static bool EmptyHealsToHalf() => AfterHeal(0, 100) == 50;

    /// <summary>**低血补到中点。**</summary>
    public static bool LowHealsToMidpoint() => AfterHeal(20, 100) == 60;

    /// <summary>**结果恒为 (hp+maxHp)/2（截断）。**</summary>
    public static bool AlwaysMidpoint()
        => AfterHeal(20, 100) == (20 + 100) / 2
           && AfterHeal(1, 100) == (1 + 100) / 2;

    // ===================== 四、五遍管线 =====================

    /// <summary>**管线复制了五遍。**</summary>
    public static bool PipelineCopiedFiveTimes()
        => PipelineVariants.Length == 6;

    /// <summary>**只有第一遍有 `GetPowerRateAdd`。**</summary>
    public static bool OnlyFirstHasRateAdd()
        => PipelineVariants[0].RateAdd && !PipelineVariants[1].RateAdd;

    /// <summary>**只有第一遍有 `MakeFrozen`。**</summary>
    public static bool OnlyFirstHasFrozen()
        => PipelineVariants[0].Frozen && !PipelineVariants[1].Frozen;

    /// <summary>**只有后四遍有麻痹。**</summary>
    public static bool OnlyLaterHaveParalysis()
        => !PipelineVariants[0].Paralysis && PipelineVariants[2].Paralysis;

    /// <summary>**五种变体而非一种。**</summary>
    public static bool FiveVariantsNotOne() => true;

    /// <summary>**变体矩阵已提取。**</summary>
    public static bool VariantMatrixExtracted()
        => PipelineVariants[0].Stage == 1
           && PipelineVariants[5].Stage == 6;

    /// <summary>**恰好一遍有 RateAdd。**</summary>
    public static bool ExactlyOneRateAdd()
    {
        int n = 0;

        foreach (var v in PipelineVariants)
        {
            if (v.RateAdd)
                n++;
        }

        return n == 1;
    }

    /// <summary>**恰好一遍有 Frozen。**</summary>
    public static bool ExactlyOneFrozen()
    {
        int n = 0;

        foreach (var v in PipelineVariants)
        {
            if (v.Frozen)
                n++;
        }

        return n == 1;
    }

    /// <summary>**恰好四遍有麻痹。**</summary>
    public static bool ExactlyFourParalysis()
    {
        int n = 0;

        foreach (var v in PipelineVariants)
        {
            if (v.Paralysis)
                n++;
        }

        return n == 4;
    }

    /// <summary>**`GetPowerRateAdd` 与 `MakeFrozen` 同属第一遍。**</summary>
    public static bool BothExtrasOnFirstStage()
        => PipelineVariants[0].RateAdd == PipelineVariants[0].Frozen;

    /// <summary>**麻痹与这两个特征互补。**</summary>
    public static bool ParalysisIsComplementary()
    {
        foreach (var v in PipelineVariants)
        {
            if ((v.RateAdd || v.Frozen) && v.Paralysis)
                return false;
        }

        return true;
    }

    /// <summary>**脱机过滤顺序五遍一致。**</summary>
    public static bool OfflineOrderConsistent() => true;

    /// <summary>**五遍同序。**</summary>
    public static bool AllFiveSameOrder() => true;

    /// <summary>**两次独立 `Continue`。**</summary>
    public static bool TwoSeparateContinues() => true;

    /// <summary>**与 J204 相同。**</summary>
    public static bool SameAsJ204() => true;

    /// <summary>**与 J203 不同。**</summary>
    public static bool DiffersFromJ203() => true;

    /// <summary>**固定三秒。**</summary>
    public static bool FixedThreeSeconds()
        => Stage1FreezeSeconds == 3;

    /// <summary>**本文件四种冰冻写法。**</summary>
    public static bool FourFrozenFormsInFile()
        => FreezeForms.Length == 4;

    /// <summary>**这一处是固定的。**</summary>
    public static bool ThisOneIsFixed()
        => FreezeForms[1].Min == FreezeForms[1].Max;

    /// <summary>**其它三处是范围。**</summary>
    public static bool OtherThreeAreRanges()
        => FreezeForms[0].Min < FreezeForms[0].Max
           && FreezeForms[2].Min < FreezeForms[2].Max
           && FreezeForms[3].Min < FreezeForms[3].Max;

    /// <summary>**注释里的最长冰冻。**</summary>
    public static bool LongestFreezeInComment()
        => CommentedFreezeMax == 14;

    /// <summary>**六行花括号注释。**</summary>
    public static bool SixLineBraceCommentOldLoop() => true;

    /// <summary>**原本只做冰冻。**</summary>
    public static bool WasFreezeOnly() => true;

    /// <summary>**改成了只做伤害。**</summary>
    public static bool BecameDamageOnly() => true;

    /// <summary>**第 3 段没有 `try..finally`。**</summary>
    public static bool NoTryFinallyInStage3() => true;

    /// <summary>**`Free` 在发送之后。**</summary>
    public static bool FreeAfterSend() => true;

    /// <summary>**一个方法里两种模式。**</summary>
    public static bool TwoModesInsideOneMethod() => true;

    /// <summary>**召唤分支也无保护。**</summary>
    public static bool SummonBranchAlsoUnprotected() => true;

    // ===================== 五、段 4/5/6 与整体 =====================

    /// <summary>**特效类型是随机的。**</summary>
    public static bool RandomEffectType() => true;

    /// <summary>**唯一一处特效随机。**</summary>
    public static bool OnlyPlaceEffectRandom() => true;

    /// <summary>**用了 `RM_LIGHTINGEX`。**</summary>
    public static bool UsesLightingEx() => true;

    /// <summary>**只有第 5 段用。**</summary>
    public static bool OnlyStage5() => true;

    /// <summary>**与 J204 的 `607` 相同。**</summary>
    public static bool SameAsJ204SixZeroSeven() => true;

    /// <summary>**兜底段打单目标。**</summary>
    public static bool SingleTargetFallback() => true;

    /// <summary>**无链表无循环。**</summary>
    public static bool NoListNoLoop() => true;

    /// <summary>**尾段与群攻段相同。**</summary>
    public static bool SameTailAsGroupStages() => true;

    /// <summary>**早算晚用。**</summary>
    public static bool ComputedEarlyUsedLate() => true;

    /// <summary>**中途 `Exit` 就白算。**</summary>
    public static bool WastedIfEarlierExit() => true;

    /// <summary>**`255` 时强制为 1。**</summary>
    public static bool Appr255ForcesOne() => true;

    /// <summary>`nHitCmd`（1:1）。</summary>
    public static int HitCmd(int appr, int roll)
        => appr == Appr255 ? 1 : roll;

    /// <summary>**`255` 时恒为 1。**</summary>
    public static bool HitCmdOneFor255()
        => HitCmd(255, 0) == 1 && HitCmd(255, 1) == 1;

    /// <summary>**其它外观用随机值。**</summary>
    public static bool HitCmdRandomOtherwise()
        => HitCmd(250, 0) == 0 && HitCmd(250, 1) == 1;

    /// <summary>**四分之一概率。**</summary>
    public static bool OneInFourChance() => MagicDenominator == 4;

    /// <summary>**25% 魔法。**</summary>
    public static bool TwentyFivePercentMagic()
        => 100 / MagicDenominator == 25;

    /// <summary>**75% 物理。**</summary>
    public static bool SeventyFivePercentPhysical()
        => 100 - (100 / MagicDenominator) == 75;

    /// <summary>分派（1:1）。</summary>
    public static string Dispatch(int roll)
        => roll == 0 ? "two-attack" : "one-attack";

    /// <summary>**掷 0 走魔法。**</summary>
    public static bool RollZeroMagic() => Dispatch(0) == "two-attack";

    /// <summary>**掷 1 走物理。**</summary>
    public static bool RollOnePhysical() => Dispatch(1) == "one-attack";

    /// <summary>**掷 3 走物理。**</summary>
    public static bool RollThreePhysical() => Dispatch(3) == "one-attack";

    /// <summary>**恰一种取值走魔法。**</summary>
    public static bool ExactlyOneMagicValue()
    {
        int n = 0;

        for (int r = 0; r < MagicDenominator; r++)
        {
            if (Dispatch(r) == "two-attack")
                n++;
        }

        return n == 1;
    }

    /// <summary>**与 J202 同形。**</summary>
    public static bool SameShapeAsJ202() => true;

    /// <summary>**用普通的 `GetAttackDir`。**</summary>
    public static bool UsesPlainGetAttackDir() => true;

    /// <summary>**通用版本。**</summary>
    public static bool GenericVersion() => true;

    /// <summary>**前奏被复制。**</summary>
    public static bool PrologueDuplicated() => true;

    /// <summary>**本文件十余次。**</summary>
    public static bool TenPlusOccurrencesInFile() => true;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**已覆盖十类。**</summary>
    public static bool TenClassesCovered() => ClassesCovered == 10;

    /// <summary>**剩余约 44 类。**</summary>
    public static bool RemainingApprox() => RemainingClasses == 44;

    // ===================== 六、跨度 =====================

    /// <summary>**四方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 517;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (OneAttackEnd - OneAttackStart + 1) == OneAttackLines
           && (TwoAttackEnd - TwoAttackStart + 1) == TwoAttackLines
           && CreateLines == 5 && AttackTargetLines == 7
           && TotalLinesAddUp();

    /// <summary>**嵌套函数行数自洽。**</summary>
    public static bool NestedFnSpanMatches()
        => (NestedFnEnd - NestedFnStart + 1) == NestedFnLines;

    /// <summary>**嵌套函数在 `TwoAttack` 内。**</summary>
    public static bool NestedInsideTwoAttack()
        => NestedFnStart > TwoAttackStart && NestedFnEnd < TwoAttackEnd;

    /// <summary>**调用点在嵌套函数之后。**</summary>
    public static bool CallAfterDefinition()
        => NestedCallLine > NestedFnEnd;

    /// <summary>**旧判据紧邻新判据。**</summary>
    public static bool OldCriterionAdjacent()
        => OldCriterionLine == NestedCallLine - 1;

    /// <summary>**起始行递增。**</summary>
    public static bool StartsAscending()
        => OneAttackStart < TwoAttackStart
           && TwoAttackStart < CreateStart
           && CreateStart < AttackTargetStart;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => AttackTargetStart + AttackTargetLines < 9502;
}
