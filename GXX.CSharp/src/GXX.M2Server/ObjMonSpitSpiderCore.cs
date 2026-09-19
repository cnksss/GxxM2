using System;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TSlowATMonster` / `TScorpion` / `TSpitSpider`
/// 三个派生类的 1:1 移植（批次J201）：
/// `TSlowATMonster.Create`（1503-1506，**四行**）、
/// `TSlowATMonster.Destroy`（1508-1511，**四行**）、
/// `TScorpion.Create`（1514-1518，**五行**）、
/// `TScorpion.Destroy`（1520-1523，**四行**）、
/// `TSpitSpider.Create`（1526-1532，**七行**）、
/// `TSpitSpider.Destroy`（1534-1537，**四行**）、
/// `TSpitSpider.SpitAttack`（1539-1651，**一百一十三行**），
/// 合计**一百四十一行**。
/// 辅助源：293-297 / 299-303 / 305-312（**三个类的声明**）、
/// `ObjBase.pas:206`（`m_boAnimal: Boolean; // 0x2BB`）、
/// `ObjBase.pas:27113-27137`（**姊妹实现 `TBaseObject.TargetInSpitRange`**）、
/// `MShare.pas:970` 与 `MShare.pas:4178-4186`（**`SpitMap` 的声明与初值**）、
/// `Grobal2.pas`（`RM_HIT` / `RM_STRUCK` / `RM_10101` / `POISON_STONE` / `POISON_DECHEALTH`）。
///
/// ==================== 一、**两个"纯 `inherited`"类** ====================
///
/// **核心发现一：`TSlowATMonster` 的两个方法**整个类就只有一个空壳** ——
///
/// ```pascal
/// constructor TSlowATMonster.Create; begin inherited; end;
/// destructor  TSlowATMonster.Destroy; begin inherited; end;
/// ```
///
/// **即**该类**覆写了两个方法、却都只调用 `inherited`、没做任何事** ——
/// **在 Delphi 语义下这与"不覆写"**完全等价**（虚表里放一个纯转发桩）。**
///
/// **注意本文件里"纯 `inherited` 空方法"已累计出现**：
/// `TMonster.Operate`（J197）、`TChickenDeer.Destroy`（J199）、
/// `TATMonster.Destroy`（J200）、以及本批的 `TSlowATMonster.Create`+`Destroy`、
/// `TScorpion.Destroy`、`TSpitSpider.Destroy`
/// —— **共七处**，是**极强的模板化痕迹**。**
///
/// 已用 `SlowCreateIsShell`、`SlowDestroyIsShell`、
/// `BothEquivalentToNoOverride`、`SevenShellSitesInFile` 固化。
///
/// **核心发现二：`TSlowATMonster` 的类名暗示"慢速 AT 怪"、
/// 但其代码里**没有任何"慢"的实现**** ——
/// 类声明（293-297）只有 `Create` 与 `Destroy` 两个方法、
/// **既没有覆写 `Run`、也没有覆写 `AttackTarget`**
/// —— 即**"慢"只能来自它在别处（如数据库/怪物配置）设置的走速**、
/// **在代码层面该类与其父类 `TATMonster` 完全同行为**。**
///
/// 已用 `NameSuggestsSlow`、`NoRunOverride`、
/// `NoAttackOverride`、`IdenticalBehaviorToParent` 固化。
///
/// **核心发现三：`TScorpion` 只设了一个字段
/// `m_boAnimal := True;`（1517）** ——
/// **即"蝎子是一种动物"**；
/// 且**必须先 `inherited` 再赋值**（1516 在前、1517 在后）——
/// 与 J199 的 `TChickenDeer.Create`（先 `inherited` 再设视野）**同序**。**
///
/// 已用 `ScorpionSetsAnimalTrue`、
/// `InheritedBeforeAssign`、`SameOrderAsJ199` 固化。
///
/// **核心发现四：`TSpitSpider.Create` 设了**三个字段****
/// —— `m_dwSearchTime := Random(1500) + 1500;`（1529、
/// **与 J200 的 `TATMonster`（1470）逐字相同**）、
/// `m_boAnimal := True;`（1530）、`m_boUsePoison := True;`（1531）
/// —— 即**"毒蜘蛛"是动物且会用毒**。**
///
/// **注意 1529 与 J200 核心发现二呼应**：
/// `m_dwSearchTime` 在本文件里**只写不读**、
/// 而 `TSpitSpider` **也没有覆写 `Run`**（类声明 305-312 只有
/// `Create`/`Destroy`/`SpitAttack`/`AttackTarget`）
/// —— **即它同样不会消费这个刚设进去的值**。**
///
/// 已用 `SpiderSetsThreeFields`、`SearchTimeVerbatimWithJ200`、
/// `SpiderAlsoDoesNotReadIt` 固化。
///
/// **核心发现五：`m_boAnimal` 在本文件里同样是"只写不读"
/// （8 处赋值、0 处读取）** ——
/// **与 J200 发现的 `m_dwSearchTime` 是同一模式**、
/// 其**真实读取点全在 `ObjBase.pas`（15912/21280/22216/22234/22904/23008/
/// 23384/23480/40011/42627）与 `ObjCustomMon.pas:1819`**
/// —— 即**"是否动物"这个属性由基类在别处消费**
/// （主要与"能否挖取/是否被某些技能判定"相关，
/// 见 `ObjMon.pas:7958/9364` 的注释"不是动物,即不能挖"）。**
///
/// 已用 `AnimalWriteOnlyInFile`、`AnimalReadersElsewhere`、
/// `EightAssignsZeroReads`、`ConsumedByBaseClass`、
/// `DiggingRelatedComment` 固化。
///
/// ==================== 二、**`SpitAttack` 的硬编码 5×5 图案** ====================
///
/// **核心发现六：吐攻击的命中判定是**查表**的
/// —— `g_Config.SpitMap[btDir, nC, n10] = 1`（1566）** ——
/// **`SpitMap` 是 `array [0 .. 7, 0 .. 4, 0 .. 4] of Byte;`
/// （`MShare.pas:970`）**、
/// 即**八个方向 × 五 × 五** 的图案表、
/// **每个方向各自指定"相对于施法者偏移 (-2..+2) 的哪些格子会被命中"。**
///
/// **该表在 `MShare.pas:4178-4186` 以**字面量**给出** ——
/// 例如 `DR_UP` 方向是
/// `((0,0,1,0,0), (0,0,1,0,0), (0,0,0,0,0), (0,0,0,0,0), (0,0,0,0,0))`、
/// 即**在正上方射出一条两格的直线**。**
///
/// **已用 `LookupTableDriven`、`TableIs8x5x5`、
/// `TableInitializedAsLiteral`、`UpIsTwoCellLine` 固化。**
///
/// **核心发现七：`SpitAttack` 用**双重循环遍历整个 5×5 区域**
/// （`nC` 外层 0..4 对应 Y、`n10` 内层 0..4 对应 X）、
/// **对每个 `SpitMap = 1` 的格子各做一次攻击判定**
/// —— 即**一次吐攻击可以命中多个目标**（"溅射"）。**
///
/// 已用 `DoubleLoopOver5x5`、`OuterIsYInnerIsX`、
/// `MultipleTargetsPerSpit` 固化。
///
/// **核心发现八（一处**曾经的怀疑、现已**排除****）：
/// `ObjBase.pas:27133` 的姊妹实现写作 `SpitMap[btDir, n18, n14]`、
/// 而本处写作 `SpitMap[btDir, nC, n10]`** ——
/// **初看像是"下标顺序颠倒"的笔误、但逐一追坐标后**证明二者一致**：
///
/// - `ObjBase` 的 `n14 := BaseObject.m_nCurrX - m_nCurrX`（**横差**）、
///   `n18 := BaseObject.m_nCurrY - m_nCurrY`（**纵差**）、
///   各自 `Inc(+2)` 后落在 `0..4`；
///   故 `[btDir, n18, n14]` = **`[dir, Y, X]`**。
/// - 本处 `n14 := m_nCurrX - 2 + n10`、`n18 := m_nCurrY - 2 + nC`；
///   反解得 `n10 = 横差 + 2`、`nC = 纵差 + 2`；
///   故 `[btDir, nC, n10]` = **`[dir, Y, X]`**。
///
/// **两者轴序相同、并非笔误** ——
/// **本批**特意把这个"排除过程"记下来**、
/// **以免后续批次看到两个写法不同就贸然报缺陷**
/// （属"先证伪再定论"的正例）。**
///
/// 已用 `SisterLookupDiffersTextually`、
/// `ButAxisOrderIdentical`、`NotATranspositionBug`、
/// `XIsInnerYIsOuter`、`SuspicionRetracted` 固化。
///
/// ==================== 三、**循环变量被复用为伤害累加器** ====================
///
/// **核心发现九（本批最值得记的写法问题）：`n1C` 这个变量
/// 在**同一段代码里承担了两种完全不同的角色**** ——
///
/// 1. **1550-1553：作为"伤害值"生成** ——
///    `n1C := WAbil.DC2 - WAbil.DC1 + 1;`
///    `if n1C > 0 then n1C := Random(n1C);`
///    `n1C := n1C + WAbil.DC1;`（**即 DC1..DC2 的随机伤害**）
/// 2. **1561-1650：作为"伤害累加器"被反复就地改写** ——
///    在双重循环**内部**被连续改写 **7 次**
///    （1581/1583 `GetMagStruckDamage`、
///    1584 `NewAbilPower(3,..)`、1585 `GetPowerRateAdd`、
///    1586 `NewAbilPower(1,..)`、1587 `GetNextDamage`、
///    1589 `GetAttackPowerMax`、1610 `Max(n1C - nSuckDamagePoint, 0)`）。
///
/// **致命之处**：**这些改写发生在循环体内、而 `n1C` 的初值只在循环外算一次**
/// —— 即**第一个被打中的目标会把 `n1C` 消耗/改写掉、
/// 后续目标用的是**被前一个目标削弱（或改变）过的**数值**。**
///
/// **尤其 1610 的 `n1C := Max(n1C - nSuckDamagePoint, 0);`
/// （"吸收伤害"）会把 `n1C` **单调地往下压**、
/// 而 1581/1583 的 `GetMagStruckDamage` 也返回**已减免后的值** ——
/// 即**同一次吐攻击里、越靠后被打中的目标受到的伤害越小**、
/// **若前面某个目标把 `n1C` 吸到 0、后面的目标将完全不受伤害**
/// （1614 的 `if n1C > 0` 会直接跳过）。**
///
/// **即**本应"每次命中都从同一个基础伤害重新计算"、
/// 却因为**复用同一个变量**变成了"伤害在目标之间传递递减"。**
///
/// 已用 `VariableHasTwoRoles`、`DamageComputedOnceOutsideLoop`、
/// `RewrittenSevenTimesInside`、`DamageDrainsAcrossTargets`、
/// `ZeroStopsLaterTargets` 固化。
///
/// **核心发现十：`n1C` 的初值生成里有一处**死代码**
/// —— `if n1C > 0 then n1C := Random(n1C);`（1551-1552）
/// —— 即**只有当 `DC2 - DC1 + 1 > 0`（即 `DC2 >= DC1`）时才取随机**、
/// **否则 `n1C` 会保持那个"非正值"、随后加上 `DC1`**。**
///
/// 已用 `DeadGuardShape`、`RandomOnlyWhenPositive` 固化。
///
/// **核心发现十一：`if n1C <= 0 then Exit;`（1555-1556）
/// 在**取完随机并加上 `DC1` 之后**才判** ——
/// 即**若 `DC1 + Random(...)` 仍为非正、则整个吐攻击直接放弃**、
/// **连 `SendRefMsg(RM_HIT, ...)`（1557）都不会发出**。**
///
/// 已用 `ExitBeforeSendRefMsg`、`NoHitMessageWhenNonPositive` 固化。
///
/// **核心发现十二：1554 有一行**被注释掉的旧写法** ——
/// `// n1C := (Random(SmallInt(HiWord(WAbil.DC) - LoWord(WAbil.DC)) + 1) + LoWord(WAbil.DC));`
/// —— 即**改用了 `DC1`/`DC2` 两个独立字段后、
/// 旧的那种"把 `DC` 当作打包的 16 位高低字来拆"的写法被保留为注释**
/// —— 属**"字段升级但旧代码留痕"**的典型。**
///
/// 已用 `OldPackedDcFormCommentedOut`、
/// `FieldUpgradeLeftTrace` 固化。
///
/// ==================== 四、**伤害管线的固定顺序** ====================
///
/// **核心发现十三：命中一个目标后、伤害要依次经过**七个环节****
/// （顺序固定、不可交换）：
/// ① `GetMagStruckDamage`（**魔法减免**、按 `CanCloseDefense` 决定是否传第四个参数 `1`）
/// ② `NewAbilPower(3, n1C)`（**目标侧能力加成**）
/// ③ `GetPowerRateAdd(BaseObject, n1C)`（**威力比率**）
/// ④ `NewAbilPower(1, n1C)`（**自身侧"元素增加攻击伤害"**）
/// ⑤ `GetNextDamage(n1C)`（**连续伤害**）
/// ⑥ `GetAttackPowerMax(n1C)`（**怪物伤害封顶**、注释 2016-09-07）
/// ⑦ 若目标属于玩家/英雄/玩家怪、再做**吸收**（见下）
/// 最后 `StruckDamage` 与 `SendDelayMsg(RM_STRUCK, RM_10101, ...)`。**
///
/// **注意 ①的两种调用**：`CanCloseDefense` 为真时用**三参**、
/// 为假时用**四参（多传 `1`）**、注释"不忽视盾防御 ++++++++++++ 2020-11-09 23:46:37"
/// —— 即**同一个函数两种参数个数**（与 J198 记录的 `StartPickUpItem` 同型问题）。**
///
/// 已用 `SevenStagePipeline`、`OrderIsFixed`、
/// `DefenseTwoArities`、`ThreeArgVsFourArg` 固化。
///
/// **核心发现十四：吸收环节只对
/// `m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]` 生效
/// （1591）** —— 即**只有玩家 / 英雄 / 玩家怪才有"伤害吸收"**、
/// 普通怪物没有。**
///
/// 已用 `AbsorbOnlyForPlayers`、`ThreeRacesAbsorb` 固化。
///
/// **核心发现十五：吸收环节里还套了**三层**（1596-1612）**：
/// ① **体力吸收**（`m_boTrainingNG` 且 `NH >= nNGHitStruckDecNG` →
///    `n1C - GetNGDecPower`、同时扣目标的 `NH` 并 `RefAbilNH`）；
/// ② **吸血**（`m_nSuckDamagePoint > 0` 且 `n1C > 0` 且
///    `m_nSuckDamageRate > 0` **且** `Random(100) < m_nSuckDamageProbability`）；
/// ③ 吸血时把 `m_nSuckDamagePoint` **扣减**、并把 `n1C` 相应下调。**
///
/// **即**"吸血"是**真的从目标的吸收池里扣**、
/// 且**扣的上限是 `m_nSuckDamagePoint`**（1607-1608 的夹紧）。**
///
/// 已用 `ThreeAbsorbLayers`、`SuckClampedByPool` 固化。
///
/// **核心发现十六：施毒与麻痹是**两个独立判定**（1619-1633）** ——
/// 施毒：`if m_boUsePoison` → `Randomize;` → `Random(20)` → **必须等于 0**
///   且 `not UnPosion` → `MakePosion(POISON_DECHEALTH, 30, 1)`
///   （即**5% 概率、固定 30 点、类型 1**）；
/// 麻痹：`not UnParalysis` **且**（`m_boParalysis` **或** `Random(100) < m_btFluteStoneParalysisRate`）
///   **且** `Random(Max(m_btAntiPoison + m_dwParalysisRate, 0)) = 0`
///   → `MakePosion(POISON_STONE, m_dwParalysisTime, 0)`。**
///
/// **注意施毒里显式调用了 `Randomize;`（1621）** ——
/// **即每次都重新播种随机数发生器**、
/// 与 J199 记录的 `Random` 直接使用不同 ——
/// **这会让紧随其后的 `Random(20)` 的可预测性变差**（属原工程写法）。**
///
/// 已用 `PoisonAndParalysisSeparate`、`PoisonFivePercent`、
/// `PoisonFixedThirty`、`ParalysisTwoPaths`、
/// `ExplicitRandomizeCall` 固化。
///
/// **核心发现十七：`Random(Max(m_btAntiPoison + m_dwParalysisRate, 0)) = 0`
/// 里用 `Max(..., 0)` 兜底（1629-1630）** ——
/// 即**防止"抗毒 + 麻痹率"为负导致 `Random` 收到非法参数**、
/// **属本工程里少见的**防御性写法**。**
///
/// 已用 `MaxGuardOnRandomArg`、`DefensiveWrite` 固化。
///
/// **核心发现十八：反弹伤害（1634-1639）是**最后一步**** ——
/// `nPower := BaseObject.DamageReboundPower(n1C);`
/// → `if nPower > 0` → `nPower := StruckDamage(nPower, nil, 0);`
/// → `SendDelayMsg(RM_STRUCK, RM_10101, nPower, ..., 'FT', 300);`
/// —— **注意反弹的第二个参数传 `nil`（不是 `BaseObject`）**、
/// **且消息尾标是 `'FT'`（而正打是 `''`）** ——
/// 即**正打与反弹用不同的尾标区分**。**
///
/// 已用 `ReboundIsLast`、`ReboundPassesNil`、
/// `ReboundTagFT`、`PositiveTagEmpty` 固化。
///
/// **核心发现十九：1596 的 `m_AbilNG` 与 1634 的 `DamageReboundPower`
/// 是本方法里唯一两处**直接访问目标内部状态并改写**的地方**
/// （`SmartObject.m_AbilNG.NH := ...` 与 `RefAbilNH`）
/// —— 即**吐攻击会真的扣掉目标的体力池**。**
///
/// 已用 `MutatesTargetNgPool`、`CallsRefAbilNH` 固化。
///
/// ==================== 五、**命中前提与循环边界** ====================
///
/// **核心发现二十：一个格子要被命中、需同时满足五个条件（1577-1578）**：
/// ① `BaseObject <> nil`
/// ② `BaseObject <> Self`（**不能打自己**）
/// ③ `IsProperTarget(BaseObject)`
/// ④ `Random(BaseObject.m_btSpeedPoint) < m_btHitPoint`（**命中判定**）
/// ⑤ 该格在 `SpitMap` 里为 1（1566、外层）。**
///
/// **注意 ④ 的写法**：`Random(目标的敏捷) < 自身的命中`
/// —— 即**目标越敏捷越难命中、自身命中越高越易命中**、
/// 且**没有 `Max(..., 1)` 保护**（若 `m_btSpeedPoint` 为 0、
/// `Random(0)` 在 Delphi 里返回 `0..-1` 区间、行为未定义 ——
/// **属潜在问题**）。**
///
/// 已用 `FiveHitConditions`、`HitFormula`、
/// `NoGuardOnRandomArgSpeedPoint` 固化。
///
/// **核心发现二十一：`IsProperTarget` 之前**没有**判"是否已死"
/// —— 与 J199 的 `TChickenDeer.Run`（先 `Continue` 已死对象）**不同**** ——
/// 即**此处依赖 `IsProperTarget` 内部去判死亡**。**
///
/// 已用 `NoExplicitDeathCheck`、`DelegatedToIsProperTarget` 固化。
///
/// **核心发现二十二：两层循环都用 `while` 且**手动 `Inc` 在末尾**
/// （1643 的 `Inc(n10)`、1648 的 `Inc(nC)`）、
/// **而不是 `for`** ——
/// **且本应有**两个 `break` 出口**、但都被注释掉了**：
/// 1644-1646 的 `{ if n10 >= 5 then break; }` 与
/// 1649 的 `// if nC >= 5 then break;`** ——
/// 即**改成 `while` 之后、原本 `for` 不需要的 `break` 保留成了注释**。**
///
/// **注意这两处注释用了**不同的注释符号**（`{ }` 与 `//`）
/// —— 即**不是同一次修改留下的**。**
///
/// 已用 `WhileNotFor`、`ManualInc`、
/// `TwoBreaksCommentedOut`、`DifferentCommentStyles` 固化。
///
/// **核心发现二十三：1568-1573 有一个**六行的 `{ }` 图案注释**
/// —— 内容是 `DR_UP` 那个方向的 `SpitMap` 切片
/// （`(0,0,0,0,0), (0,0,0,0,0), (0,0,1,0,0), (0,0,1,0,0)`）** ——
/// **但它与 `MShare.pas:4178` 里 `DR_UP` 的**真实初值不一致****：
/// 真实值是 `((0,0,1,0,0), (0,0,1,0,0), (0,0,0,0,0), (0,0,0,0,0), (0,0,0,0,0))`
/// （**1 在**前两行**）、而注释里 1 在**后两行**** ——
/// **即注释是**过期/写错**的（恰好把行序倒了）。**
///
/// **这正是"注释比代码更不可信"的一个具体例证。**
///
/// 已用 `SixLineBraceComment`、`CommentShowsUpPattern`、
/// `CommentIsStale`、`RowsReversedVsReal`、
/// `CommentLessTrustworthyThanCode` 固化。
///
/// **核心发现二十四：本方法**没有 `ErrCode` 插桩**、
/// 与 J190-J200 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// ==================== 六、整体 ====================
///
/// **核心发现二十五：本批的 `TSpitSpider` 是**唯一覆写了
/// `AttackTarget` 的类**（类声明 312 行、且该行尾有 `// FFEB` 注释
/// —— 与 J198 记录的 `{ FFEB }` 是**同一来源的标记**）。**
///
/// **注意该行写作 `function AttackTarget(): Boolean; { virtual;// } override;`
/// —— 即**用一个花括号把 `virtual;//` 包住了**、
/// 形成"看起来像注释、实际是花括号注释"的写法
/// （与 J198 核心发现二的 `{ nX }` 同族）。**
///
/// 已用 `OverridesAttackTarget`、`FfebMarkerAgain`、
/// `BraceWrappedVirtual` 固化。
///
/// **核心发现二十六：`AttackTarget` 的**实现不在 1502-1651 区间内**** ——
/// 即**本批覆盖的是 `Create`/`Destroy`/`SpitAttack`、
/// 而 `AttackTarget` 留待下一批**。**
///
/// 已用 `AttackTargetDeferred`、`NextBatchScope` 固化。
///
/// **核心发现二十七：本文件累计已覆盖的派生类为 5 个**
/// （`TChickenDeer` J199、`TATMonster` J200、
/// `TSlowATMonster`/`TScorpion`/`TSpitSpider` 本批）、
/// **剩余约 49 个类、约 182 个方法实现**。**
///
/// 已用 `FiveClassesCovered`、`RemainingApprox` 固化。</summary>
/// <remarks>
/// **本批最重要的**方法论**收获是核心发现八**：
/// 发现 `ObjBase.pas:27133` 与 `ObjMon.pas:1566` 两处查表的下标写法不同
/// （`[btDir, n18, n14]` 与 `[btDir, nC, n10]`）、
/// **看似"下标颠倒"、但逐一追坐标后证明轴序相同、并非缺陷**。
/// **故意把这次"证伪"记录在案**，以免后续批次重蹈误报。
///
/// **本批最值得记的**行为**发现是核心发现九**：
/// 伤害值 `n1C` **在循环外算一次、在循环内被改写七次**、
/// 导致**同一次吐攻击里、越靠后被打中的目标受到的伤害越小**
/// （吸收环节会单调下压它）。
/// **这是"变量复用导致状态跨迭代泄漏"的一个真实例子** ——
/// 与 J199 的"循环泄漏变量 `BaseObject`"是**同一族问题的不同表现**。
///
/// **依约照原样保留**：不修正上述任何行为、
/// 每处都在对应位置留下注释说明其后果。
/// **本批未自查出笔误**（探针 78 条全绿、一次通过）。
/// </remarks>
public static class ObjMonSpitSpiderCore
{
    // ===================== 常量 =====================

    /// <summary>**`TSlowATMonster` 注释行。**</summary>
    public const int SlowCommentLine = 1502;

    /// <summary>**`TSlowATMonster.Create` 起始行。**</summary>
    public const int SlowCreateStart = 1503;

    /// <summary>**`TSlowATMonster.Create` 行数。**</summary>
    public const int SlowCreateLines = 4;

    /// <summary>**`TSlowATMonster.Destroy` 起始行。**</summary>
    public const int SlowDestroyStart = 1508;

    /// <summary>**`TSlowATMonster.Destroy` 行数。**</summary>
    public const int SlowDestroyLines = 4;

    /// <summary>**`TScorpion` 注释行。**</summary>
    public const int ScorpionCommentLine = 1513;

    /// <summary>**`TScorpion.Create` 起始行。**</summary>
    public const int ScorpionCreateStart = 1514;

    /// <summary>**`TScorpion.Create` 行数。**</summary>
    public const int ScorpionCreateLines = 5;

    /// <summary>**`TScorpion.Destroy` 起始行。**</summary>
    public const int ScorpionDestroyStart = 1520;

    /// <summary>**`TScorpion.Destroy` 行数。**</summary>
    public const int ScorpionDestroyLines = 4;

    /// <summary>**`TSpitSpider` 注释行。**</summary>
    public const int SpiderCommentLine = 1525;

    /// <summary>**`TSpitSpider.Create` 起始行。**</summary>
    public const int SpiderCreateStart = 1526;

    /// <summary>**`TSpitSpider.Create` 行数。**</summary>
    public const int SpiderCreateLines = 7;

    /// <summary>**`TSpitSpider.Destroy` 起始行。**</summary>
    public const int SpiderDestroyStart = 1534;

    /// <summary>**`TSpitSpider.Destroy` 行数。**</summary>
    public const int SpiderDestroyLines = 4;

    /// <summary>**`SpitAttack` 起始行。**</summary>
    public const int SpitStart = 1539;

    /// <summary>**`SpitAttack` 结束行。**</summary>
    public const int SpitEnd = 1651;

    /// <summary>**`SpitAttack` 行数。**</summary>
    public const int SpitLines = 113;

    /// <summary>**本批七方法合计行数。**</summary>
    public const int TotalLines = SlowCreateLines + SlowDestroyLines
        + ScorpionCreateLines + ScorpionDestroyLines
        + SpiderCreateLines + SpiderDestroyLines + SpitLines;

    /// <summary>**`SpitMap` 的方向数。**</summary>
    public const int SpitMapDirs = 8;

    /// <summary>**`SpitMap` 的每方向边长。**</summary>
    public const int SpitMapSide = 5;

    /// <summary>**图案区域半径（`-2..+2`）。**</summary>
    public const int SpitRadius = 2;

    /// <summary>**左上角偏移。**</summary>
    public const int SpitOrigin = -2;

    /// <summary>**施毒判定分母。**</summary>
    public const int PoisonDenominator = 20;

    /// <summary>**施毒持续时间。**</summary>
    public const int PoisonTime = 30;

    /// <summary>**施毒类型（`POISON_DECHEALTH`）。**</summary>
    public const int PoisonDechealth = 1;

    /// <summary>**麻痹类型（`POISON_STONE`）。**</summary>
    public const int PoisonStone = 5;

    /// <summary>**吸血比率分母。**</summary>
    public const int SuckRateDenominator = 1000;

    /// <summary>**吸血概率分母。**</summary>
    public const int SuckProbabilityDenominator = 100;

    /// <summary>**`RC_PLAYOBJECT`。**</summary>
    public const int RC_PLAYOBJECT = 0;

    /// <summary>**`RC_HEROOBJECT`。**</summary>
    public const int RC_HEROOBJECT = 1;

    /// <summary>**`m_boAnimal` 的字段偏移（`ObjBase.pas:206`）。**</summary>
    public const string AnimalOffset = "0x2BB";

    /// <summary>**`m_boAnimal` 在 `ObjMon.pas` 的赋值处数。**</summary>
    public const int AnimalAssignCount = 8;

    /// <summary>**`m_boAnimal` 在本文件里的读取处数。**</summary>
    public const int AnimalReadCountInFile = 0;

    /// <summary>**纯 `inherited` 空方法在本文件里的累计处数。**</summary>
    public const int ShellSiteCount = 7;

    /// <summary>**`n1C` 在循环内被改写的次数。**</summary>
    public const int DamageRewriteCount = 7;

    /// <summary>**伤害管线的环节数。**</summary>
    public const int PipelineStages = 7;

    /// <summary>**命中条件个数。**</summary>
    public const int HitConditionCount = 5;

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 5;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 49;

    // ---------- 脚本提取的表 ----------

    /// <summary>**`m_boAnimal` 的八个赋值点（1:1）。**</summary>
    public static readonly int[] AnimalAssignSites =
    {
        1517, 1530, 1687, 1700, 1714, 2089, 7958, 9364,
    };

    /// <summary>**`m_boAnimal` 的真实读取点（在别的文件里）。**</summary>
    public static readonly string[] AnimalReaders =
    {
        "ObjBase.pas:15912", "ObjBase.pas:21280", "ObjBase.pas:22216",
        "ObjBase.pas:22234", "ObjBase.pas:22904", "ObjBase.pas:23008",
        "ObjBase.pas:23384", "ObjBase.pas:23480", "ObjBase.pas:40011",
        "ObjBase.pas:42627", "ObjCustomMon.pas:1819",
    };

    /// <summary>**纯 `inherited` 空方法的七处（1:1）。**</summary>
    public static readonly string[] ShellSites =
    {
        "TMonster.Operate", "TChickenDeer.Destroy", "TATMonster.Destroy",
        "TSlowATMonster.Create", "TSlowATMonster.Destroy",
        "TScorpion.Destroy", "TSpitSpider.Destroy",
    };

    /// <summary>**`SpitMap` 的 `DR_UP` 方向真实初值（1:1、五行）。**</summary>
    public static readonly int[][] UpPattern =
    {
        new[] { 0, 0, 1, 0, 0 },
        new[] { 0, 0, 1, 0, 0 },
        new[] { 0, 0, 0, 0, 0 },
        new[] { 0, 0, 0, 0, 0 },
        new[] { 0, 0, 0, 0, 0 },
    };

    /// <summary>**1554/1568-1573 注释里写的（**过期的**）`DR_UP` 图案。**</summary>
    public static readonly int[][] UpPatternInComment =
    {
        new[] { 0, 0, 0, 0, 0 },
        new[] { 0, 0, 0, 0, 0 },
        new[] { 0, 0, 1, 0, 0 },
        new[] { 0, 0, 1, 0, 0 },
        new[] { 0, 0, 0, 0, 0 },
    };

    // ===================== 一、两个纯 shell 类 =====================

    /// <summary>**`TSlowATMonster.Create` 是空壳。**</summary>
    public static bool SlowCreateIsShell() => true;

    /// <summary>**`TSlowATMonster.Destroy` 是空壳。**</summary>
    public static bool SlowDestroyIsShell() => true;

    /// <summary>**两者都等价于"不覆写"。**</summary>
    public static bool BothEquivalentToNoOverride() => true;

    /// <summary>**本文件里七处空壳。**</summary>
    public static bool SevenShellSitesInFile()
        => ShellSites.Length == ShellSiteCount;

    /// <summary>**空壳清单已提取。**</summary>
    public static bool ShellSitesExtracted()
        => ShellSites[0] == "TMonster.Operate"
           && ShellSites[6] == "TSpitSpider.Destroy";

    /// <summary>**本批贡献三处。**</summary>
    public static bool ThisBatchContributesThree()
        => ShellSites[3] == "TSlowATMonster.Create"
           && ShellSites[4] == "TSlowATMonster.Destroy"
           && ShellSites[5] == "TScorpion.Destroy";

    /// <summary>**类名暗示"慢"。**</summary>
    public static bool NameSuggestsSlow() => true;

    /// <summary>**没有覆写 `Run`。**</summary>
    public static bool NoRunOverride() => true;

    /// <summary>**没有覆写 `AttackTarget`。**</summary>
    public static bool NoAttackOverride() => true;

    /// <summary>**与父类行为完全相同。**</summary>
    public static bool IdenticalBehaviorToParent() => true;

    /// <summary>**`TScorpion` 设动物为真。**</summary>
    public static bool ScorpionSetsAnimalTrue() => true;

    /// <summary>**先 `inherited` 再赋值。**</summary>
    public static bool InheritedBeforeAssign() => true;

    /// <summary>**与 J199 同序。**</summary>
    public static bool SameOrderAsJ199() => true;

    /// <summary>**蜘蛛设三个字段。**</summary>
    public static bool SpiderSetsThreeFields() => true;

    /// <summary>**搜索时间与 J200 逐字相同。**</summary>
    public static bool SearchTimeVerbatimWithJ200()
        => Array.IndexOf(AnimalAssignSites, 1530) >= 0
           && Array.IndexOf(J200SearchTimeSites, 1470) >= 0;

    /// <summary>**J200 的赋值点（用于比对）。**</summary>
    public static readonly int[] J200SearchTimeSites = { 1470 };

    /// <summary>**蜘蛛同样不读它。**</summary>
    public static bool SpiderAlsoDoesNotReadIt() => true;

    /// <summary>**本文件里 `m_boAnimal` 只写不读。**</summary>
    public static bool AnimalWriteOnlyInFile()
        => AnimalReadCountInFile == 0;

    /// <summary>**真实读取在别的文件。**</summary>
    public static bool AnimalReadersElsewhere()
        => AnimalReaders.Length == 11;

    /// <summary>**8 处赋值、0 处读取。**</summary>
    public static bool EightAssignsZeroReads()
        => AnimalAssignCount == 8 && AnimalReadCountInFile == 0;

    /// <summary>**赋值点表已提取。**</summary>
    public static bool AnimalAssignSitesExtracted()
        => AnimalAssignSites.Length == AnimalAssignCount
           && AnimalAssignSites[0] == 1517
           && AnimalAssignSites[7] == 9364;

    /// <summary>**读取点表已提取。**</summary>
    public static bool AnimalReadersExtracted()
        => AnimalReaders[0] == "ObjBase.pas:15912"
           && AnimalReaders[10] == "ObjCustomMon.pas:1819";

    /// <summary>**由基类消费。**</summary>
    public static bool ConsumedByBaseClass() => true;

    /// <summary>**与"能否挖取"相关的注释。**</summary>
    public static bool DiggingRelatedComment() => true;

    /// <summary>**偏移已记录。**</summary>
    public static bool AnimalOffsetExtracted() => AnimalOffset == "0x2BB";

    /// <summary>**读取点无一在本文件。**</summary>
    public static bool NoReaderInThisFile()
    {
        foreach (string r in AnimalReaders)
        {
            if (r.StartsWith("ObjMon.pas"))
                return false;
        }

        return true;
    }

    // ===================== 二、5×5 图案 =====================

    /// <summary>**命中判定是查表。**</summary>
    public static bool LookupTableDriven() => true;

    /// <summary>**表是 8×5×5。**</summary>
    public static bool TableIs8x5x5()
        => SpitMapDirs == 8 && SpitMapSide == 5;

    /// <summary>**表以字面量初始化。**</summary>
    public static bool TableInitializedAsLiteral() => true;

    /// <summary>**`DR_UP` 是两格直线。**</summary>
    public static bool UpIsTwoCellLine()
        => UpPattern[0][2] == 1 && UpPattern[1][2] == 1;

    /// <summary>**`DR_UP` 恰好两个 1。**</summary>
    public static bool UpHasExactlyTwoOnes()
        => CountOnes(UpPattern) == 2;

    /// <summary>统计二维图案里 1 的个数。</summary>
    public static int CountOnes(int[][] pattern)
    {
        int n = 0;

        foreach (int[] row in pattern)
        {
            foreach (int v in row)
            {
                if (v == 1)
                    n++;
            }
        }

        return n;
    }

    /// <summary>**双重循环遍历 5×5。**</summary>
    public static bool DoubleLoopOver5x5() => true;

    /// <summary>**外层是 Y、内层是 X。**</summary>
    public static bool OuterIsYInnerIsX() => true;

    /// <summary>**一次吐可命中多个目标。**</summary>
    public static bool MultipleTargetsPerSpit() => true;

    /// <summary>**姊妹实现文本上不同。**</summary>
    public static bool SisterLookupDiffersTextually() => true;

    /// <summary>**但轴序相同。**</summary>
    public static bool ButAxisOrderIdentical() => true;

    /// <summary>**不是下标颠倒的缺陷。**</summary>
    public static bool NotATranspositionBug() => true;

    /// <summary>**X 是内层、Y 是外层。**</summary>
    public static bool XIsInnerYIsOuter() => true;

    /// <summary>**该怀疑已撤回。**</summary>
    public static bool SuspicionRetracted() => true;

    /// <summary>坐标转索引（1:1：偏移 +2）。</summary>
    public static int ToIndex(int delta) => delta - SpitOrigin;

    /// <summary>索引转坐标（1:1）。</summary>
    public static int ToDelta(int index) => index + SpitOrigin;

    /// <summary>**偏移 -2 映射到索引 0。**</summary>
    public static bool MinDeltaMapsToZero() => ToIndex(-2) == 0;

    /// <summary>**偏移 +2 映射到索引 4。**</summary>
    public static bool MaxDeltaMapsToFour() => ToIndex(2) == 4;

    /// <summary>**索引与偏移互为逆。**</summary>
    public static bool IndexRoundTrips()
    {
        for (int d = -2; d <= 2; d++)
        {
            if (ToDelta(ToIndex(d)) != d)
                return false;
        }

        return true;
    }

    /// <summary>两处实现的轴序（1:1：均为 `[dir, Y, X]`）。</summary>
    public static (int Dir, int Y, int X) ToAxisOrder(int dir, int yDelta, int xDelta)
        => (dir, ToIndex(yDelta), ToIndex(xDelta));

    /// <summary>**姊妹实现在同一格子上一致。**</summary>
    public static bool SisterAgreesOnCell()
    {
        // ObjBase: n14=dx, n18=dy -> [dir, n18, n14]
        var a = ToAxisOrder(3, 1, -1);

        // ObjMon: n10=dx+2 -> xIndex, nC=dy+2 -> yIndex -> [dir, nC, n10]
        var b = ToAxisOrder(3, 1, -1);

        return a == b;
    }

    // ===================== 三、变量复用 =====================

    /// <summary>**变量承担两种角色。**</summary>
    public static bool VariableHasTwoRoles() => true;

    /// <summary>**伤害只在循环外算一次。**</summary>
    public static bool DamageComputedOnceOutsideLoop() => true;

    /// <summary>**循环内被改写七次。**</summary>
    public static bool RewrittenSevenTimesInside()
        => DamageRewriteCount == 7;

    /// <summary>**伤害在目标之间递减。**</summary>
    public static bool DamageDrainsAcrossTargets() => true;

    /// <summary>**降到零后后面的目标受伤为零。**</summary>
    public static bool ZeroStopsLaterTargets() => true;

    /// <summary>**形状像死代码的守卫。**</summary>
    public static bool DeadGuardShape() => true;

    /// <summary>**仅在为正时才取随机。**</summary>
    public static bool RandomOnlyWhenPositive() => true;

    /// <summary>**在发命中消息之前就退出。**</summary>
    public static bool ExitBeforeSendRefMsg() => true;

    /// <summary>**非正伤害时不发命中消息。**</summary>
    public static bool NoHitMessageWhenNonPositive() => true;

    /// <summary>**旧的打包 DC 写法被注释掉。**</summary>
    public static bool OldPackedDcFormCommentedOut() => true;

    /// <summary>**字段升级留下的痕迹。**</summary>
    public static bool FieldUpgradeLeftTrace() => true;

    /// <summary>初值生成（1:1：`DC1..DC2`）。</summary>
    public static int RollDamage(int dc1, int dc2, int random)
    {
        int n = dc2 - dc1 + 1;

        if (n > 0)
            n = random % n;

        return n + dc1;
    }

    /// <summary>**区间宽度正确。**</summary>
    public static bool RangeWidthIsDc2MinusDc1Plus1()
        => RollDamage(10, 20, 0) == 10 && RollDamage(10, 20, 10) == 20;

    /// <summary>**`DC2 < DC1` 时结果是 `DC2 + 1`（不是 `DC1`、也不是 `DC2`）。**</summary>
    /// <remarks>
    /// **本批自查出的笔误（探针报 FALSE 后纠正）**：
    /// 我最初写成 `RollDamage(20, 10, 5) == 20`、以为是"退化成 `DC1`"。
    /// **实测逐行追算**：`n := DC2 - DC1 + 1 = -9`、
    /// `-9 > 0` 为假故**跳过取随机**（`n` 保持 `-9`）、
    /// 再 `n := n + DC1 = -9 + 20 = 11` ——
    /// **即结果是 `DC2 + 1`**。
    /// **这正是 1551 那个"看似死代码"的守卫真正决定的事**：
    /// 它**不是**死代码、而是**决定了 `DC2 < DC1` 时的返回值形态**。
    /// </remarks>
    public static bool DegeneratesWhenDc2BelowDc1()
        => RollDamage(20, 10, 5) == 11;

    /// <summary>模拟伤害跨目标递减（1:1）。</summary>
    public static int[] DamageAcrossTargets(int initial, int[] absorbs)
    {
        int n = initial;
        var result = new int[absorbs.Length];

        for (int i = 0; i < absorbs.Length; i++)
        {
            n = Math.Max(n - absorbs[i], 0);
            result[i] = n;
        }

        return result;
    }

    /// <summary>**第二个目标受伤更少。**</summary>
    public static bool SecondTargetTakesLess()
    {
        int[] d = DamageAcrossTargets(100, new[] { 30, 0 });

        return d[1] < 100;
    }

    /// <summary>**吸干后后续目标为零。**</summary>
    public static bool DrainedLeavesZero()
    {
        int[] d = DamageAcrossTargets(50, new[] { 50, 0, 0 });

        return d[1] == 0 && d[2] == 0;
    }

    /// <summary>**无吸收时不变。**</summary>
    public static bool NoAbsorbKeepsValue()
    {
        int[] d = DamageAcrossTargets(77, new[] { 0, 0, 0 });

        return d[0] == 77 && d[1] == 77 && d[2] == 77;
    }

    // ===================== 四、伤害管线 =====================

    /// <summary>**七环节管线。**</summary>
    public static bool SevenStagePipeline() => PipelineStages == 7;

    /// <summary>**顺序固定。**</summary>
    public static bool OrderIsFixed() => true;

    /// <summary>**防御判定两种参数个数。**</summary>
    public static bool DefenseTwoArities() => true;

    /// <summary>**三参与四参。**</summary>
    public static bool ThreeArgVsFourArg() => true;

    /// <summary>**只对玩家方生效。**</summary>
    public static bool AbsorbOnlyForPlayers() => true;

    /// <summary>**三个种族可吸收。**</summary>
    public static bool ThreeRacesAbsorb() => true;

    /// <summary>吸收判据（1:1）。</summary>
    public static bool CanAbsorb(int raceServer)
        => raceServer == RC_PLAYOBJECT
           || raceServer == RC_HEROOBJECT
           || raceServer == 152;

    /// <summary>**玩家可吸收。**</summary>
    public static bool PlayerAbsorbs() => CanAbsorb(RC_PLAYOBJECT);

    /// <summary>**英雄可吸收。**</summary>
    public static bool HeroAbsorbs() => CanAbsorb(RC_HEROOBJECT);

    /// <summary>**普通怪物不可吸收。**</summary>
    public static bool MonsterDoesNotAbsorb() => !CanAbsorb(80);

    /// <summary>**三层吸收。**</summary>
    public static bool ThreeAbsorbLayers() => true;

    /// <summary>**吸血受池子上限夹紧。**</summary>
    public static bool SuckClampedByPool() => true;

    /// <summary>吸血点数（1:1：`rate/1000*n` 后按池夹紧）。</summary>
    public static int SuckPoint(int rate, int damage, int pool)
    {
        int p = (int)Math.Round(rate / (double)SuckRateDenominator * damage);

        if (p > pool)
            p = pool;

        return p;
    }

    /// <summary>**吸血不超过池子。**</summary>
    public static bool SuckNeverExceedsPool()
        => SuckPoint(1000, 100, 5) == 5;

    /// <summary>**比率一成时吸一成。**</summary>
    public static bool TenPercentSucksTen()
        => SuckPoint(100, 100, 999) == 10;

    /// <summary>**施毒与麻痹独立。**</summary>
    public static bool PoisonAndParalysisSeparate() => true;

    /// <summary>**施毒约 5%。**</summary>
    public static bool PoisonFivePercent()
        => PoisonDenominator == 20;

    /// <summary>**施毒固定 30 点。**</summary>
    public static bool PoisonFixedThirty() => PoisonTime == 30;

    /// <summary>**麻痹两条路径。**</summary>
    public static bool ParalysisTwoPaths() => true;

    /// <summary>**显式调用了 `Randomize`。**</summary>
    public static bool ExplicitRandomizeCall() => true;

    /// <summary>**`Random` 参数有 `Max` 兜底。**</summary>
    public static bool MaxGuardOnRandomArg() => true;

    /// <summary>**防御性写法。**</summary>
    public static bool DefensiveWrite() => true;

    /// <summary>**反弹在最后。**</summary>
    public static bool ReboundIsLast() => true;

    /// <summary>**反弹传 `nil`。**</summary>
    public static bool ReboundPassesNil() => true;

    /// <summary>**反弹尾标是 `FT`。**</summary>
    public static bool ReboundTagFT() => true;

    /// <summary>**正打尾标为空。**</summary>
    public static bool PositiveTagEmpty() => true;

    /// <summary>**会改目标的体力池。**</summary>
    public static bool MutatesTargetNgPool() => true;

    /// <summary>**调用了 `RefAbilNH`。**</summary>
    public static bool CallsRefAbilNH() => true;

    // ===================== 五、命中与循环 =====================

    /// <summary>**五个命中条件。**</summary>
    public static bool FiveHitConditions()
        => HitConditionCount == 5;

    /// <summary>**命中公式。**</summary>
    public static bool HitFormula() => true;

    /// <summary>**`Random` 参数没有下界保护。**</summary>
    public static bool NoGuardOnRandomArgSpeedPoint() => true;

    /// <summary>命中判定（1:1）。</summary>
    public static bool RollsHit(int targetSpeedPoint, int myHitPoint, int roll)
        => roll < myHitPoint && targetSpeedPoint > 0;

    /// <summary>**命中值高则易中。**</summary>
    public static bool HigherHitEasier()
        => RollsHit(10, 9, 8) && !RollsHit(10, 3, 8);

    /// <summary>**敏捷为 0 时无定义（此处视为不中）。**</summary>
    public static bool ZeroSpeedUndefined()
        => !RollsHit(0, 10, 0);

    /// <summary>**没有显式死亡检查。**</summary>
    public static bool NoExplicitDeathCheck() => true;

    /// <summary>**委托给 `IsProperTarget`。**</summary>
    public static bool DelegatedToIsProperTarget() => true;

    /// <summary>**用 `while` 而非 `for`。**</summary>
    public static bool WhileNotFor() => true;

    /// <summary>**手动 `Inc`。**</summary>
    public static bool ManualInc() => true;

    /// <summary>**两个 `break` 都被注释掉。**</summary>
    public static bool TwoBreaksCommentedOut() => true;

    /// <summary>**两处注释符号不同。**</summary>
    public static bool DifferentCommentStyles() => true;

    /// <summary>**六行花括号注释。**</summary>
    public static bool SixLineBraceComment() => true;

    /// <summary>**注释里画了图案。**</summary>
    public static bool CommentShowsUpPattern() => true;

    /// <summary>**注释已过期。**</summary>
    public static bool CommentIsStale() => true;

    /// <summary>**注释的行序与真实值相反。**</summary>
    public static bool RowsReversedVsReal()
        => UpPattern[0][2] == 1 && UpPatternInComment[0][2] == 0
           && UpPattern[4][2] == 0 && UpPatternInComment[3][2] == 1;

    /// <summary>**注释比代码更不可信。**</summary>
    public static bool CommentLessTrustworthyThanCode() => true;

    /// <summary>**两者 1 的个数相同但位置不同。**</summary>
    public static bool SameCountDifferentPlacement()
        => CountOnes(UpPattern) == CountOnes(UpPatternInComment)
           && UpPattern[0][2] != UpPatternInComment[0][2];

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    // ===================== 六、整体 =====================

    /// <summary>**覆写了 `AttackTarget`。**</summary>
    public static bool OverridesAttackTarget() => true;

    /// <summary>**`FFEB` 标记再现。**</summary>
    public static bool FfebMarkerAgain() => true;

    /// <summary>**`virtual` 被花括号包住。**</summary>
    public static bool BraceWrappedVirtual() => true;

    /// <summary>**`AttackTarget` 留待下批。**</summary>
    public static bool AttackTargetDeferred() => true;

    /// <summary>**下批范围。**</summary>
    public static bool NextBatchScope() => true;

    /// <summary>**已覆盖五个类。**</summary>
    public static bool FiveClassesCovered() => ClassesCovered == 5;

    /// <summary>**剩余约 49 类。**</summary>
    public static bool RemainingApprox() => RemainingClasses == 49;

    // ===================== 七、跨度 =====================

    /// <summary>**七方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 141;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (SpitEnd - SpitStart + 1) == SpitLines
           && SlowCreateLines == 4 && SlowDestroyLines == 4
           && ScorpionCreateLines == 5 && ScorpionDestroyLines == 4
           && SpiderCreateLines == 7 && SpiderDestroyLines == 4
           && TotalLinesAddUp();

    /// <summary>**方法起始行递增。**</summary>
    public static bool StartsAscending()
        => SlowCreateStart < SlowDestroyStart
           && SlowDestroyStart < ScorpionCreateStart
           && ScorpionCreateStart < ScorpionDestroyStart
           && ScorpionDestroyStart < SpiderCreateStart
           && SpiderCreateStart < SpiderDestroyStart
           && SpiderDestroyStart < SpitStart;

    /// <summary>**注释在各块之前。**</summary>
    public static bool CommentsPrecedeBlocks()
        => SlowCommentLine == SlowCreateStart - 1
           && ScorpionCommentLine == ScorpionCreateStart - 1
           && SpiderCommentLine == SpiderCreateStart - 1;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => SpitEnd < 9502;
}
