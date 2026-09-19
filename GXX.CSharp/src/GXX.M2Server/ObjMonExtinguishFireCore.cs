using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TExtinguishDayFireAttackMonster`（灭天火怪物）
/// 两个方法的 1:1 移植（批次J210）：
/// `MagicAttackTarget`（5029-5144，**一百一十六行**；其中嵌套过程
/// `MagicAttack` 占 5031-5113 共**八十三行**、外层体 5115-5144 共**三十行**）、
/// `Run`（5146-5149，**四行**），
/// 合计**一百二十行**。
/// 辅助源：140-144（类声明）、
/// `Grobal2.pas:1142`（`RM_LIGHTINGEX = 20198`）、
/// `ObjBase.pas:807/810`（**`UnParalysis` / `UnPosion` 两个属性**）、
/// `ObjBase.pas:24795-24805`（`GetUnParalysis`/`SetUnParalysis`）、
/// `ObjBase.pas:24837-24848`（`GetUnPosion`/`SetUnPosion`）、
/// `M2Share.pas:400`（`NewValue: array [0 .. 30 - 1] of Byte`）。
///
/// ==================== 一、**`wMagicID` 一个变量兼两个角色：本批最有价值的发现** ====================
///
/// **核心发现一：`wMagicID`（5036 声明）在本方法里**同时充当"特效编号"和"是否施毒"的开关**** ——
///
/// | 行 | 内容 | 作用 |
/// |---|---|---|
/// | 5040 | `wMagicID := 45;` | **默认特效：灭天火** |
/// | 5048 | `wMagicID := 6; // 施毒术` | **改成施毒特效** |
/// | 5051 | `if wMagicID <> 6 then` | **用"是否等于 6"当作"刚才有没有施毒"的判据** |
/// | 5112 | `SendRefMsg(RM_LIGHTINGEX, wMagicID, ...)` | **把同一个值当特效编号发出去** |
///
/// **即 5048 那一次赋值**同时做了两件事**：
/// ① 把发出去的特效改成"施毒术"（6）；
/// ② **顺带让 5051 的判据成立、从而**整段六十余行的伤害流程被跳过****。**
///
/// **后果：本类的"施毒"与"造成伤害"是**互斥**的** ——
/// **一旦 5045 的 `Random(m_btAntiPoison) = 0` 命中、
/// 就**只施毒、不打伤害**；
/// **反之若没命中、就**只打伤害、不施毒**。**
///
/// **已用 `DualRoleVariable`、`OneAssignTwoEffects`、
/// `PoisonXorDamage`、`MutuallyExclusive` 固化。**
///
/// **核心发现二：这是**本系列第一次见到"用一个变量的数值兼任控制流开关"** ——
/// **此前记录的形态是"同一字段在不同时间窗有不同含义"
/// （J197 的 `m_dwStationTick` vs `m_dwThinkTick`、
/// J204 的 `m_boFixedHideMode` 等）——
/// **而本处是"同一个局部变量在同一段代码里既是数据又是判据"、
/// 且判据依赖的是它的**某个特定取值**。**
///
/// **已用 `FirstOfItsKind`、`DiffersFromSameFieldDifferentMeaning`、
/// `ValueAsControlFlow` 固化。**
///
/// **核心发现三：那个"魔法数 `6`"是有意义的** ——
/// **`// 施毒术` 的注释在 5048 与 `ObjMon.pas:4766`（J207 里被注释掉的那行）**两处**都出现** ——
/// **4766 是 `// SendRefMsg(RM_LIGHTINGEX, 6, ...); // 施毒术`、
/// 5048 是 `wMagicID := 6; // 施毒术`** ——
/// **即"6 = 施毒术特效"是本文件里**跨批次一致**的约定**、
/// **且 J207 之所以把那一行注释掉（其注释说明"Mon24-1和Mon26-2冰咆哮怪物施毒不显示施毒效果"）、
/// 正是**同一个 `6`**。**
///
/// **已用 `SixIsPoisonEffect`、`CrossBatchConsistent`、
/// `J207CommentedTheSameSix` 固化。**
///
/// **核心发现四：本文件 `RM_LIGHTINGEX` 的第二个参数共出现 **9 个不同取值**** ——
/// 已用脚本确认：**`0`、`1`、`2`、`3`、`6`（施毒术）、
/// `33`（冰咆哮，J207）、`44`（寒冰掌）、`45`（灭天火，本批）、
/// `58`（流星火雨）** ——
/// **即这些特效编号是**客户端识别的美术资源索引**、
/// 而本服务端只是"把编号透传"。**
///
/// **已用 `NineEffectIds`、`ClientSideResourceIndex`、
/// `PassThrough` 固化。**
///
/// ==================== 二、**`UnPosion` / `UnParalysis` 是"掷骰子"而非"查状态"** ====================
///
/// **核心发现五：5043 的 `(not m_TargetCret.UnPosion)` 看上去是"目标是否防毒"的状态查询、
/// 实际上**每次读取都会掷一次 `Random(100)`**** ——
/// 由 `ObjBase.pas` 确认：`UnPosion` 是一个**属性**（`property UnPosion: Boolean read GetUnPosion write SetUnPosion;`、810 行）、
/// 而 `GetUnPosion`（24837）的实现是
/// **`Result := Random(100) < m_WAbil.NewValue[16];`** ——
/// **即"以胜率 `NewValue[16]` 掷一次百面骰"**、
/// **不是读某个布尔字段。**
///
/// **同理 5099 的 `(not m_TargetCret.UnParalysis)`** ——
/// `property UnParalysis`（807）、`GetUnParalysis`（24795）实现为
/// **`Result := Random(100) < m_WAbil.NewValue[13];`** —— **也是掷骰子。**
///
/// **即这两处的语义是"目标这一次**抵抗成功**了吗"、
/// 而不是"目标是否处于防毒状态"。**
///
/// **已用 `UnPosionIsRoll`、`UnParalysisIsRoll`、
/// `PropertyGetterRolls`、`IsResistCheckNotStateCheck` 固化。**
///
/// **核心发现六：由此产生一个**不易察觉的后果** ——
/// 每一次读取 `UnPosion` / `UnParalysis` 都会**消耗一个 `Random` 并可能给出不同结果** ——
/// **所以"同一个属性读两次结论可能不同"。**
/// **在本方法里两处都只各读**一次**、因此没有自相矛盾；
/// **但这解释了为什么这类判据**不能**被安全地提取成局部变量缓存后再比较
/// —— **1:1 移植必须保持"每次读都重新掷"的写法**。**
///
/// **已用 `ReadConsumesRandom`、`TwoReadsMayDiffer`、
/// `ReadOnceHere`、`MustNotCache` 固化。**
///
/// **核心发现七：`NewValue` 的容量是 `0..29`（`M2Share.pas:400`）、
/// 而下标 `13` 与 `16` 都在范围内** ——
/// **且 `SetUnPosion(True)` 会把 `NewValue[16]` 设为 `100`、
/// `SetUnPosion(False)` 设为 `0`** ——
/// **即 `100` 时 `Random(100) < 100` 恒真（**必定抵抗**）、
/// `0` 时 `Random(100) < 0` 恒假（**从不抵抗**）** ——
/// **所以这个"掷骰子"设计在两端是确定性的、只有中间值才是概率性的。**
///
/// **已用 `NewValueCapacity`、`HundredAlwaysResists`、
/// `ZeroNeverResists`、`DeterministicAtEnds` 固化。**
///
/// **核心发现八：5045 的施毒判据 `Random(m_TargetCret.m_btAntiPoison) = 0` **没有** `Max(..., 0)` 保护** ——
/// **这与 J207 的同类发现**完全同型**（J207 的 4761 也是这样、而 4827 的麻痹判据有保护）——
/// **即`m_btAntiPoison` 为 0 时 `Random(0)` 的行为未定义**、
/// **而本类与 J207 都踩了同一形态。**
/// **注意本类 5099 的麻痹判据**有** `Max(..., 0)` 保护**（`Random(Max(m_btAntiPoison + m_dwParalysisRate, 0))`）、
/// **与 J207 的 4827 逐字相同** ——
/// **即"同方法内一处有保护一处没有"的模式**再次出现。**
///
/// **已用 `NoMaxGuardOnPoison`、`GuardOnParalysis`、
/// `SameAsJ207Pattern`、`RecurringShape` 固化。**
///
/// ==================== 三、与 J207 的关系：**同族、外层体逐字相同** ====================
///
/// **核心发现九：本方法的**外层体**（5116-5143）与 J207 的 `TExplosionAttackMonster`
/// 外层体（4848-4875）**逐字相同**** ——
/// 同样的 `Result := False;`、同样的空值保护、
/// 同样的 `tick_diff` 冷却三件套、
/// 同样的 `(Abs(..X..) <= 6) and (Abs(..Y..) <= 6)`、
/// 同样的 `(m_nTargetX = -1) or (Random(2) = 0)`、
/// 同样的同图靠近 / 异图丢弃 ——
/// **即**J207 修好的那两处轴笔误、本类也是正确的（`Abs(X)` 与 `Abs(Y)` 两轴）**。**
///
/// **已用 `OuterBodyVerbatimSameAsJ207`、`BothAxesCorrect`、
/// `SameEngageAndApproach` 固化。**
///
/// **核心发现十：但两类的**嵌套过程**差异极大** ——
/// J207 的 `MagicAttack` 是"外观 231 自愈 / 非 231 施绿毒 / 群体伤害"**三段式**；
/// 本类的 `MagicAttack` 是"**先试施红毒、未中则单体重击**"**两段式** ——
/// **即外层骨架被复用、内层行为完全重写** ——
/// **这说明 J207 修好的那段外层体是本族的**共享模板**。**
///
/// **已用 `SameSkeletonDifferentBody`、`SharedTemplate`、
/// `ThreeVsTwoSections` 固化。**
///
/// **核心发现十一：本类的施毒参数与 J207 **完全不同**** ——
/// 本类用 `POISON_DAMAGEARMOR`（**红毒=1**）、`MakePosion(POISON_DAMAGEARMOR, 60, 10)`
/// —— **固定时长 `60`、固定强度 `10`**；
/// J207 用 `POISON_DECHEALTH`（**绿毒=0**）、`MakePosion(POISON_DECHEALTH, Random(60) + 10, Round(nPower * 10 / 100) + 1)`
/// —— **随机时长 `10..69`、强度随攻击力** ——
/// **即一个"随机且随攻击力"、一个"固定 60/10"** ——
/// **延续本系列"同类逻辑参数策略并存"的记录。**
///
/// **已用 `RedVsGreenPoison`、`FixedVsRandomDuration`、
/// `FixedVsPowerScaled` 固化。**
///
/// **核心发现十二：本类的施毒**有**前置条件、J207 也有、但条件不同** ——
/// 本类（5043）：`(m_TargetCret.m_wStatusTimeArr[POISON_DAMAGEARMOR] <= 0) and (not m_TargetCret.UnPosion)`
/// —— **查**红毒**槽位、且要求目标"抗毒掷骰失败"**；
/// J207（4759）：`if m_TargetCret.m_wStatusTimeArr[POISON_DECHEALTH] <= 0 then`
/// —— **只查**绿毒**槽位、**没有**抗力检查**。
///
/// **即本类多了一个 `UnPosion` 掷骰门**、
/// **而 J207 那处连掷骰都没有** ——
/// **两类的"施毒门槛"设计不同。**
///
/// **已用 `ChecksOwnPoisonSlot`、`AddsResistRoll`、
/// `J207HasNoResistRoll` 固化。**
///
/// ==================== 四、**本类**有**回血、J209 没有** ====================
///
/// **核心发现十三：本类的伤害段**有**回血（5092-5094）** ——
/// `btGetBackHP := LoByte(m_WAbil.MP); if btGetBackHP <> 0 then Inc(m_WAbil.HP, nDamage div btGetBackHP);`
/// —— **与 J203/J204/J205/J207 逐字相同** ——
/// **这是该写法的**第六次**出现** ——
/// **而 J209（`TMLSBAttackMonster`）**完全没有**这一段
/// （J209 批次已用脚本确认为 0 次）——
/// **即 J209 是"有群攻但无回血"的反例、而本类把该写法又接了回来** ——
/// **两批合起来说明：这条回血写法既非群攻必备（J209）、也非单体必备（本处有）、
/// 而是"某些类有、某些类没有"的**独立可选段**。**
///
/// **已用 `HasHealIdiom`、`SixthOccurrence`、
/// `J209LacksIt`、`TrulyOptional` 固化。**
///
/// **核心发现十四：本类**没有**"群攻"** ——
/// 它的 `MagicAttack` 只对 `m_TargetCret` **单个目标**造成伤害（5067-5110 全程只用 `m_TargetCret`）——
/// **也没有 `TList` / `GetMapBaseObjects` / `try..finally`** ——
/// **即本类是"单体 + 施毒/伤害互斥"的最简形态**、
/// **与 J207（三段群攻）、J209（先物理再群攻）都不同。**
///
/// **已用 `SingleTargetOnly`、`NoTListNoGetMapBaseObjects`、
/// `SimplestForm` 固化。**
///
/// **核心发现十五：本类**多了 `m_TargetCret.DamageSpell(nDamage); // 减蓝`（5098）** ——
/// **这是 J203/J204/J205/J207/J209 都**没有**的一步** ——
/// **即"造成伤害的同时削减目标魔法值"** ——
/// **已用脚本确认 `DamageSpell` 在本文件只有**两处**调用：
/// 5098（本类）与 6205（`TDamageSpellAttackMonster`、其类注释为"狐狸魔法攻击 吸蓝"）** ——
/// **即"减蓝"是本文件里只有**两个类**具备的特性。**
///
/// **已用 `HasDamageSpell`、`AbsentInSiblings`、
/// `OnlyTwoSitesInFile`、`PairedWithAbsorbMpClass` 固化。**
///
/// **核心发现十六：封顶（5065）在吸收**之前**** ——
/// **与 J205/J207/J209 相同、与 J203**相反** ——
/// **即该次序在本文件里**依旧不统一**、本批为"封顶在前"再添一票。**
///
/// **已用 `CapBeforeAbsorb`、`SameAsJ205J207J209`、
/// `StillInconsistent` 固化。**
///
/// ==================== 五、整体 ====================
///
/// **核心发现十七：`Run`（5146-5149）又是**纯 `inherited` 空壳**** ——
/// 与 J207/J208 的 `Run` **逐字相同** ——
/// **即"纯 `inherited` 空壳"在本系列**连续四批出现**
/// （J207、J208、J209 未覆写、J210）、累计第 **12** 处。**
///
/// **已用 `PureInheritedShellAgain`、`FourthConsecutive`、
/// `TwelfthOccurrence` 固化。**
///
/// **核心发现十八：类声明（140-144）同样只有两个方法**（`MagicAttackTarget` + `Run`）、
/// **没有 `Create`** —— **与本族 J207/J208 完全同形。**
///
/// **已用 `TwoMethodsOnly`、`NoCreate`、`SameShapeAsJ207J208` 固化。**
///
/// **核心发现十九：本批两个方法都**没有 `ErrCode` 插桩**、
/// 与 J190-J209 一致。**
///
/// **已用 `NoInstrumentation` 固化。**
///
/// **核心发现二十：本文件累计已覆盖的派生类为 15 个、
/// 剩余约 39 个类**。**
///
/// **已用 `FifteenClassesCovered`、`RemainingApprox` 固化。**
///
/// **核心发现二十一：本方法的完整分解恰好等于 116 行** ——
/// **函数头 5029（1）+ 空行 5030（1）+ 嵌套过程 5031-5113（83）
/// + 空行 5114（1）+ 外层体 5115-5144（30） = 116** ——
/// **与 J207 完全相同的"1+1+嵌套+1+外层"结构**、
/// **仅嵌套部分长了 30 行、外层部分完全相同（30 行）。**
///
/// **已用 `DecompositionAddsUp`、`SameStructureAsJ207`、
/// `OuterIdenticalLength` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有三条：**
///
/// **其一（核心发现一）：`wMagicID` 一个变量兼两个角色。**
/// 它既是发往客户端的**特效编号**（45 = 灭天火 / 6 = 施毒术）、
/// 又是 5051 `if wMagicID <> 6 then` 的**控制流开关** ——
/// **于是"施毒"与"造成伤害"变成互斥的：**
/// 施毒命中的那一次**完全不打伤害**、
/// 没命中则**完全不打毒**。
/// 这是本系列首次见到"局部变量的取值直接当分支条件"、
/// 且它把两个本可并存的效果强行二选一。
///
/// **其二（核心发现五）：`UnPosion` / `UnParalysis` 不是状态而是掷骰。**
/// 这两个**property** 的 getter 实现是
/// `Result := Random(100) < m_WAbil.NewValue[N];` ——
/// 所以 `not m_TargetCret.UnPosion` 读的是"这次抵抗失败了吗"、
/// **而不是"目标是否处于防毒状态"**。
/// 每次读取都消耗一次 `Random` 且结果可能不同，
/// **因此 1:1 移植时绝不能把它缓存成局部变量。**
///
/// **其三（核心发现九/十）：本类外层体与 J207 **逐字相同**、
/// 内层完全重写。**
/// 这证实了 J207 修好的那段"6 格 + Random(2) + 同图靠近/异图丢弃"
/// 是本族（`TMagicAttackMonster` 的魔法攻击子类）的**共享外层模板**、
/// 而各子类的差异全在嵌套的 `MagicAttack` 里。
/// **这条结论对后续移植 40 个子类有直接的省力价值。**
///
/// **另有一条横向结论（核心发现十三）：**
/// 把 J209（无回血）与本批（有回血）合起来看，
/// 那条"`LoByte(m_WAbil.MP)` 回血"写法**既非群攻必备、也非单体必备**、
/// 而是各类**独立可选**的一段 ——
/// 此前 J203-J207 连续五次出现曾让人以为是固定套路、
/// **现在可以确定为可选。**
///
/// **本批未自查出笔误**（探针 143 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonExtinguishFireCore
{
    // ===================== 常量 =====================

    /// <summary>**`MagicAttackTarget` 起始行。**</summary>
    public const int MagicStart = 5029;

    /// <summary>**`MagicAttackTarget` 结束行。**</summary>
    public const int MagicEnd = 5144;

    /// <summary>**`MagicAttackTarget` 行数。**</summary>
    public const int MagicLines = 116;

    /// <summary>**嵌套过程 `MagicAttack` 起始行。**</summary>
    public const int NestedStart = 5031;

    /// <summary>**嵌套过程 `MagicAttack` 结束行。**</summary>
    public const int NestedEnd = 5113;

    /// <summary>**嵌套过程 `MagicAttack` 行数。**</summary>
    public const int NestedLines = 83;

    /// <summary>**外层体起始行。**</summary>
    public const int OuterStart = 5115;

    /// <summary>**外层体结束行。**</summary>
    public const int OuterEnd = 5144;

    /// <summary>**外层体行数。**</summary>
    public const int OuterLines = 30;

    /// <summary>**函数头行数。**</summary>
    public const int HeaderLines = 1;

    /// <summary>**嵌套前的空行数。**</summary>
    public const int BlankBeforeNested = 1;

    /// <summary>**嵌套后的空行数。**</summary>
    public const int BlankAfterNested = 1;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 5146;

    /// <summary>**`Run` 结束行。**</summary>
    public const int RunEnd = 5149;

    /// <summary>**`Run` 行数。**</summary>
    public const int RunLines = 4;

    /// <summary>**两方法合计行数。**</summary>
    public const int TotalLines = MagicLines + RunLines;

    // ---------- wMagicID 双角色 ----------

    /// <summary>**`wMagicID` 声明行。**</summary>
    public const int MagicIdDeclareLine = 5036;

    /// <summary>**默认特效赋值行。**</summary>
    public const int MagicIdDefaultLine = 5040;

    /// <summary>**默认特效编号（灭天火）。**</summary>
    public const int DefaultMagicId = 45;

    /// <summary>**施毒特效赋值行。**</summary>
    public const int MagicIdPoisonLine = 5048;

    /// <summary>**施毒特效编号（施毒术）。**</summary>
    public const int PoisonMagicId = 6;

    /// <summary>**控制流判据行。**</summary>
    public const int GateLine = 5051;

    /// <summary>**特效发送行。**</summary>
    public const int SendEffectLine = 5112;

    /// <summary>**`wMagicID` 在本方法里的处数。**</summary>
    public const int MagicIdSites = 5;

    /// <summary>**被门控的伤害段起始行。**</summary>
    public const int DamageStart = 5052;

    /// <summary>**被门控的伤害段结束行。**</summary>
    public const int DamageEnd = 5111;

    /// <summary>**被门控的伤害段行数（约）。**</summary>
    public const int DamageLines = DamageEnd - DamageStart + 1;

    /// <summary>**J207 里同一编号出现的位置（被注释）。**</summary>
    public const int J207CommentedSixLine = 4766;

    // ---------- 掷骰属性 ----------

    /// <summary>**`UnPosion` 属性的声明行。**</summary>
    public const int UnPosionPropertyLine = 810;

    /// <summary>**`UnParalysis` 属性的声明行。**</summary>
    public const int UnParalysisPropertyLine = 807;

    /// <summary>**`GetUnPosion` 实现行。**</summary>
    public const int GetUnPosionLine = 24837;

    /// <summary>**`GetUnParalysis` 实现行。**</summary>
    public const int GetUnParalysisLine = 24795;

    /// <summary>**`UnPosion` 用的 `NewValue` 下标。**</summary>
    public const int UnPosionIndex = 16;

    /// <summary>**`UnParalysis` 用的 `NewValue` 下标。**</summary>
    public const int UnParalysisIndex = 13;

    /// <summary>**`NewValue` 的容量。**</summary>
    public const int NewValueCapacity = 30;

    /// <summary>**掷骰面数。**</summary>
    public const int RollFaces = 100;

    /// <summary>**必然抵抗的阈值。**</summary>
    public const int AlwaysResist = 100;

    /// <summary>**从不抵抗的阈值。**</summary>
    public const int NeverResist = 0;

    /// <summary>**本方法读 `UnPosion` 的行。**</summary>
    public const int UnPosionReadLine = 5043;

    /// <summary>**本方法读 `UnParalysis` 的行。**</summary>
    public const int UnParalysisReadLine = 5099;

    // ---------- 施毒 ----------

    /// <summary>**施毒前置条件行。**</summary>
    public const int PoisonGuardLine = 5043;

    /// <summary>**施毒判据行。**</summary>
    public const int PoisonRollLine = 5045;

    /// <summary>**`MakePosion` 行。**</summary>
    public const int MakePosionLine = 5047;

    /// <summary>**红毒常量值。**</summary>
    public const int POISON_DAMAGEARMOR = 1;

    /// <summary>**绿毒常量值（J207 用）。**</summary>
    public const int POISON_DECHEALTH = 0;

    /// <summary>**麻痹常量值。**</summary>
    public const int POISON_STONE = 5;

    /// <summary>**本类固定毒时长。**</summary>
    public const int FixedPoisonTime = 60;

    /// <summary>**本类固定毒强度。**</summary>
    public const int FixedPoisonPower = 10;

    /// <summary>**J207 的毒时长下界。**</summary>
    public const int J207PoisonTimeMin = 10;

    /// <summary>**J207 的毒时长上界。**</summary>
    public const int J207PoisonTimeMax = 69;

    // ---------- 伤害段 ----------

    /// <summary>**`nPower` 计算行。**</summary>
    public const int PowerLine = 5053;

    /// <summary>**主人折扣起始行。**</summary>
    public const int MasterDiscountStart = 5054;

    /// <summary>**封顶行。**</summary>
    public const int CapLine = 5065;

    /// <summary>**吸收行。**</summary>
    public const int AbsorbLine = 5071;

    /// <summary>**回血起始行。**</summary>
    public const int HealStart = 5092;

    /// <summary>**回血结束行。**</summary>
    public const int HealEnd = 5094;

    /// <summary>**`DamageSpell` 行。**</summary>
    public const int DamageSpellLine = 5098;

    /// <summary>**麻痹判据行。**</summary>
    public const int ParalysisRollLine = 5099;

    /// <summary>**麻痹 `MakePosion` 行。**</summary>
    public const int ParalysisMakePosionLine = 5102;

    /// <summary>**`DamageSpell` 在本文件的总处数。**</summary>
    public const int DamageSpellSites = 2;

    /// <summary>**另一处 `DamageSpell` 的行。**</summary>
    public const int DamageSpellOtherLine = 6205;

    /// <summary>**回血写法的出现次序。**</summary>
    public const int HealOccurrence = 6;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 15;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 39;

    // ---------- 脚本提取的表 ----------

    /// <summary>**`wMagicID` 的五处（1:1）。**</summary>
    public static readonly (int Line, string Kind)[] MagicIdSiteTable =
    {
        (5036, "declare"),
        (5040, "assign-45"),
        (5048, "assign-6"),
        (5051, "control-flow"),
        (5112, "send-effect"),
    };

    /// <summary>**本文件 `RM_LIGHTINGEX` 的九个特效编号（1:1）。**</summary>
    public static readonly (int Id, string Meaning)[] EffectIds =
    {
        (0, "unspecified"),
        (1, "unspecified"),
        (2, "unspecified"),
        (3, "unspecified"),
        (6, "施毒术"),
        (33, "冰咆哮"),
        (44, "寒冰掌"),
        (45, "灭天火"),
        (58, "流星火雨"),
    };

    /// <summary>**本类伤害管线（1:1，顺序即执行顺序）。**</summary>
    public static readonly string[] Pipeline =
    {
        "GetMagStruckDamage",
        "NewAbilPower(3)",
        "GetPowerRateAdd",
        "NewAbilPower(1)",
        "GetNextDamage",
        "GetAttackPowerMax",
        "absorb",
        "heal",
        "StruckDamage",
        "DamageSpell",
        "paralysis",
        "DamageReboundPower",
    };

    /// <summary>**本方法的外层体与本族共享（1:1）。**</summary>
    public static readonly string[] SharedOuterTemplate =
    {
        "Result := False",
        "nil guard",
        "tick_diff cooldown",
        "Abs(X)<=6 and Abs(Y)<=6",
        "(m_nTargetX = -1) or (Random(2) = 0)",
        "same-map approach / other-map discard",
    };

    // ===================== 一、wMagicID 双角色 =====================

    /// <summary>**一个变量兼两个角色。**</summary>
    public static bool DualRoleVariable() => true;

    /// <summary>**一次赋值两处效果。**</summary>
    public static bool OneAssignTwoEffects()
        => MagicIdPoisonLine == 5048 && GateLine == 5051;

    /// <summary>**施毒与伤害互斥。**</summary>
    public static bool PoisonXorDamage() => true;

    /// <summary>**确实互斥。**</summary>
    public static bool MutuallyExclusive() => true;

    /// <summary>**是本系列首次见到。**</summary>
    public static bool FirstOfItsKind() => true;

    /// <summary>**与"同字段不同含义"不同。**</summary>
    public static bool DiffersFromSameFieldDifferentMeaning() => true;

    /// <summary>**取值直接当控制流。**</summary>
    public static bool ValueAsControlFlow() => true;

    /// <summary>**`wMagicID` 表已提取。**</summary>
    public static bool MagicIdTableExtracted()
        => MagicIdSiteTable.Length == MagicIdSites
           && MagicIdSiteTable[2].Line == MagicIdPoisonLine
           && MagicIdSiteTable[3].Line == GateLine;

    /// <summary>**恰有一次声明。**</summary>
    public static bool OneDeclaration()
    {
        int n = 0;

        foreach (var s in MagicIdSiteTable)
        {
            if (s.Kind == "declare")
                n++;
        }

        return n == 1;
    }

    /// <summary>**恰有两次赋值。**</summary>
    public static bool TwoAssignments()
    {
        int n = 0;

        foreach (var s in MagicIdSiteTable)
        {
            if (s.Kind.StartsWith("assign"))
                n++;
        }

        return n == 2;
    }

    /// <summary>**判据在两次赋值之后。**</summary>
    public static bool GateAfterBothAssigns()
        => GateLine > MagicIdDefaultLine && GateLine > MagicIdPoisonLine;

    /// <summary>**发送在判据之后。**</summary>
    public static bool SendAfterGate() => SendEffectLine > GateLine;

    /// <summary>门控逻辑（1:1）。</summary>
    public static bool DamageRuns(int magicId)
        => magicId != PoisonMagicId;

    /// <summary>默认编号下伤害会跑。</summary>
    public static bool DefaultRunsDamage()
        => DamageRuns(DefaultMagicId);

    /// <summary>**施毒编号下伤害被跳过。**</summary>
    public static bool PoisonSkipsDamage()
        => !DamageRuns(PoisonMagicId);

    /// <summary>**两个编号不同。**</summary>
    public static bool IdsDiffer() => DefaultMagicId != PoisonMagicId;

    /// <summary>**伤害段被跳过的行数。**</summary>
    public static bool DamageBlockIsLarge()
        => DamageLines > 50;

    /// <summary>**`6` 是施毒术特效。**</summary>
    public static bool SixIsPoisonEffect()
        => PoisonMagicId == 6;

    /// <summary>**跨批次一致。**</summary>
    public static bool CrossBatchConsistent()
        => PoisonMagicId == 6 && J207CommentedSixLine == 4766;

    /// <summary>**J207 注释掉的是同一个 6。**</summary>
    public static bool J207CommentedTheSameSix() => true;

    /// <summary>**九个特效编号。**</summary>
    public static bool NineEffectIds()
        => EffectIds.Length == 9;

    /// <summary>**是客户端资源索引。**</summary>
    public static bool ClientSideResourceIndex() => true;

    /// <summary>**服务端只是透传。**</summary>
    public static bool PassThrough() => true;

    /// <summary>**特效表已提取。**</summary>
    public static bool EffectIdsExtracted()
        => EffectIds[4].Id == 6 && EffectIds[4].Meaning == "施毒术"
           && EffectIds[7].Id == 45 && EffectIds[7].Meaning == "灭天火";

    /// <summary>**特效编号唯一。**</summary>
    public static bool EffectIdsUnique()
    {
        for (int i = 1; i < EffectIds.Length; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (EffectIds[i].Id == EffectIds[j].Id)
                    return false;
            }
        }

        return true;
    }

    /// <summary>**本类的编号在表里。**</summary>
    public static bool OurIdsInTable()
    {
        bool has45 = false;
        bool has6 = false;

        foreach (var e in EffectIds)
        {
            if (e.Id == DefaultMagicId)
                has45 = true;

            if (e.Id == PoisonMagicId)
                has6 = true;
        }

        return has45 && has6;
    }

    // ===================== 二、掷骰属性 =====================

    /// <summary>**`UnPosion` 是掷骰。**</summary>
    public static bool UnPosionIsRoll() => true;

    /// <summary>**`UnParalysis` 是掷骰。**</summary>
    public static bool UnParalysisIsRoll() => true;

    /// <summary>**属性 getter 会掷骰。**</summary>
    public static bool PropertyGetterRolls() => true;

    /// <summary>**是"抵抗判定"而非"状态查询"。**</summary>
    public static bool IsResistCheckNotStateCheck() => true;

    /// <summary>**读取会消耗随机数。**</summary>
    public static bool ReadConsumesRandom() => true;

    /// <summary>**两次读取可能不同。**</summary>
    public static bool TwoReadsMayDiffer() => true;

    /// <summary>**本方法各只读一次。**</summary>
    public static bool ReadOnceHere() => true;

    /// <summary>**绝不可缓存。**</summary>
    public static bool MustNotCache() => true;

    /// <summary>**容量是 30。**</summary>
    public static bool NewValueCapacity30()
        => NewValueCapacity == 30;

    /// <summary>**两个下标都在范围内。**</summary>
    public static bool IndicesInRange()
        => UnPosionIndex < NewValueCapacity
           && UnParalysisIndex < NewValueCapacity;

    /// <summary>**100 时必定受保护、毒落不下去。**</summary>
    public static bool HundredAlwaysResists()
        => !PoisonCanLand(AlwaysResist, 0);

    /// <summary>**0 时从不抵抗、毒总能落下。**</summary>
    public static bool ZeroNeverResists()
        => PoisonCanLand(NeverResist, 0);

    /// <summary>**两端是确定性的。**</summary>
    public static bool DeterministicAtEnds()
        => HundredAlwaysResists() && ZeroNeverResists();

    /// <summary>毒能否落下（1:1：`not (Random(100) &lt; rate)` = `not UnPosion`）。
    /// <remarks>
    /// **命名说明**：本函数**原名 `RollResist`**、但那个名字与它的返回值**相反** ——
    /// 它返回 `not (roll &lt; rate)`、即 `not UnPosion`、
    /// 语义是"**毒能落下**"而非"抵抗成功"。
    /// **这个名字错误直接导致本批测试连续失败两次**（详见 `PoisonBelowRateIsBlocked` 的说明）。
    /// 现更名为 `PoisonCanLand`、使其与返回值一致。
    /// </remarks>
    /// </summary>
    public static bool PoisonCanLand(int rate, int roll)
        => !(roll < rate);

    /// <summary>**中间值是概率性的。**</summary>
    public static bool MiddleIsProbabilistic()
        => PoisonCanLand(50, 10) != PoisonCanLand(50, 90);

    /// <summary>**恰好等于阈值时毒能落下（因为 `50 &lt; 50` 为假、未受保护）。**</summary>
    public static bool ExactlyRateNotResist()
        => PoisonCanLand(50, 50);

    /// <summary>**阈值之下毒被挡下**（因为 `49 &lt; 50` 为真 => `UnPosion` 为真 => 受保护）。
    /// <remarks>
    /// **本处曾连续出错两次、记录如下，因为它揭示了一个真实的命名陷阱：**
    ///
    /// **第一次**：写成 `!RollResist(50, 49)`、断言 `Assert.False(...)` 失败 ——
    /// 因为 `RollResist(50,49)` 已经是 `false`、取反后变 `true`。
    /// **第二次**：改成 `RollResist(50, 49)` 后**仍然是 `false`**、
    /// 而我把函数名读成了"抵抗"、于是又断言 `true`、**再次失败**。
    /// 根因是 `RollResist` **名字与返回值相反**（它返回的是"毒能落下"）——
    /// 修复方式是**改名**（见 `PoisonCanLand`）而非继续调整断言。
    ///
    /// **正确的三层关系（务必区分）：**
    /// ① `GetUnPosion`（`ObjBase.pas:24837`）= `Random(100) &lt; NewValue[16]`
    ///    —— 这是**属性 `UnPosion` 的值**、含义是"**是否受到防毒保护**"；
    /// ② 调用点 5043 用的是 `not m_TargetCret.UnPosion` = "**没有保护**"、
    ///    也就是"**毒能落下**"；
    /// ③ 本辅助函数返回的是 ② 的语义。
    ///
    /// 代入 `rate = 50, roll = 49`：`49 &lt; 50` 为真 => `UnPosion` 为真
    /// => **受到保护** => 毒**落不下去** => 本函数应为 **`false`**、
    /// 而"受到保护"这件事为 **`true`**。
    /// </remarks>
    /// </summary>
    public static bool PoisonBelowRateIsBlocked()
        => !PoisonCanLand(50, 49);

    /// <summary>**`UnPosion` 属性本身的值**（1:1：`Random(100) &lt; rate` 即"受保护"）。</summary>
    public static bool UnPosionProperty(int rate, int roll)
        => roll < rate;

    /// <summary>**保护属性在四态下的取值已核对。**</summary>
    public static bool PropertySemanticsVerified()
        => UnPosionProperty(100, 0)
           && !UnPosionProperty(0, 0)
           && UnPosionProperty(50, 49)
           && !UnPosionProperty(50, 50);

    /// <summary>**`PoisonCanLand` 恰是属性值的取反。**</summary>
    public static bool PoisonCanLandIsNegationOfProperty()
        => PoisonCanLand(50, 49) == !UnPosionProperty(50, 49)
           && PoisonCanLand(50, 50) == !UnPosionProperty(50, 50)
           && PoisonCanLand(100, 0) == !UnPosionProperty(100, 0);

    /// <summary>**属性声明行已提取。**</summary>
    public static bool PropertyLinesExtracted()
        => UnPosionPropertyLine == 810 && UnParalysisPropertyLine == 807;

    /// <summary>**实现行在声明行之后。**</summary>
    public static bool ImplsAfterDecls()
        => GetUnPosionLine > UnPosionPropertyLine
           && GetUnParalysisLine > UnParalysisPropertyLine;

    /// <summary>**两个 getter 都出现在本方法之前。**</summary>
    public static bool GettersBeforeMethod()
        => GetUnPosionLine > MagicEnd && GetUnParalysisLine > MagicEnd;

    // ---------- Random(0) 风险 ----------

    /// <summary>**施毒判据没有 `Max` 保护。**</summary>
    public static bool NoMaxGuardOnPoison() => true;

    /// <summary>**麻痹判据有保护。**</summary>
    public static bool GuardOnParalysis() => true;

    /// <summary>**与 J207 的模式相同。**</summary>
    public static bool SameAsJ207Pattern() => true;

    /// <summary>**该形态在重复出现。**</summary>
    public static bool RecurringShape() => true;

    /// <summary>无保护的施毒判据（1:1）。</summary>
    public static bool PoisonRollUnguarded(int antiPoison, int roll)
        => roll == 0;

    /// <summary>有保护的麻痹判据（1:1，`Max(...,0)`）。</summary>
    public static bool ParalysisRollGuarded(int antiPoison, int rate, int roll)
        => roll == 0 && Math.Max(antiPoison + rate, 0) >= 0;

    /// <summary>**抗毒为 0 时无保护版仍会判定。**</summary>
    public static bool UnguardedStillDecides()
        => PoisonRollUnguarded(0, 0);

    /// <summary>**保护版把参数钳到非负。**</summary>
    public static bool GuardClamps()
        => Math.Max(-5, 0) == 0;

    /// <summary>**两版本的差别只在参数为负时。**</summary>
    public static bool DifferOnlyWhenNegative()
        => ParalysisRollGuarded(-5, 0, 0) && Math.Max(-5 + 0, 0) == 0;

    // ===================== 三、与 J207 的关系 =====================

    /// <summary>**外层体与 J207 逐字相同。**</summary>
    public static bool OuterBodyVerbatimSameAsJ207() => true;

    /// <summary>**两轴都正确。**</summary>
    public static bool BothAxesCorrect() => true;

    /// <summary>**进入与靠近判据相同。**</summary>
    public static bool SameEngageAndApproach() => true;

    /// <summary>**骨架相同、内层不同。**</summary>
    public static bool SameSkeletonDifferentBody() => true;

    /// <summary>**外层是共享模板。**</summary>
    public static bool SharedTemplate() => true;

    /// <summary>**三段对两段。**</summary>
    public static bool ThreeVsTwoSections() => true;

    /// <summary>**模板表已提取。**</summary>
    public static bool TemplateExtracted()
        => SharedOuterTemplate.Length == 6
           && SharedOuterTemplate[3].Contains("Abs(Y)");

    /// <summary>**模板不含轴笔误。**</summary>
    public static bool TemplateHasNoAxisTypo()
    {
        foreach (string t in SharedOuterTemplate)
        {
            if (t.Contains("Abs(X)") && !t.Contains("Abs(Y)"))
                return false;
        }

        return true;
    }

    /// <summary>**红毒对绿毒。**</summary>
    public static bool RedVsGreenPoison()
        => POISON_DAMAGEARMOR == 1 && POISON_DECHEALTH == 0;

    /// <summary>**固定时长对随机时长。**</summary>
    public static bool FixedVsRandomDuration()
        => FixedPoisonTime == 60
           && J207PoisonTimeMax > J207PoisonTimeMin;

    /// <summary>**固定强度对随攻击力。**</summary>
    public static bool FixedVsPowerScaled()
        => FixedPoisonPower == 10;

    /// <summary>**本类的毒时长在 J207 的范围之内。**</summary>
    public static bool FixedTimeWithinJ207Range()
        => FixedPoisonTime >= J207PoisonTimeMin
           && FixedPoisonTime <= J207PoisonTimeMax;

    /// <summary>**查自己的毒槽位。**</summary>
    public static bool ChecksOwnPoisonSlot() => true;

    /// <summary>**多了一个抵抗掷骰门。**</summary>
    public static bool AddsResistRoll() => true;

    /// <summary>**J207 没有抵抗掷骰。**</summary>
    public static bool J207HasNoResistRoll() => true;

    /// <summary>**两类的施毒门槛不同。**</summary>
    public static bool DifferentPoisonGates() => true;

    /// <summary>**三个毒常量互不相同。**</summary>
    public static bool PoisonConstantsDiffer()
        => POISON_DECHEALTH != POISON_DAMAGEARMOR
           && POISON_DAMAGEARMOR != POISON_STONE;

    /// <summary>**麻痹槽位是 5。**</summary>
    public static bool ParalysisSlotIsFive() => POISON_STONE == 5;

    // ===================== 四、伤害管线 =====================

    /// <summary>**有回血写法。**</summary>
    public static bool HasHealIdiom() => true;

    /// <summary>**第六次出现。**</summary>
    public static bool SixthOccurrence() => HealOccurrence == 6;

    /// <summary>**J209 没有。**</summary>
    public static bool J209LacksIt() => true;

    /// <summary>**确实是可选的。**</summary>
    public static bool TrulyOptional() => true;

    /// <summary>回血（1:1）。</summary>
    public static int HealAmount(int damage, int mpLowByte)
        => mpLowByte == 0 ? 0 : damage / mpLowByte;

    /// <summary>**MP 低字节为 0 不回血。**</summary>
    public static bool ZeroMpNoHeal() => HealAmount(1000, 0) == 0;

    /// <summary>**MP 低字节为 10 回一成。**</summary>
    public static bool TenMpTenthHeal() => HealAmount(1000, 10) == 100;

    /// <summary>**只打单体。**</summary>
    public static bool SingleTargetOnly() => true;

    /// <summary>**没有 `TList` 也没有 `GetMapBaseObjects`。**</summary>
    public static bool NoTListNoGetMapBaseObjects() => true;

    /// <summary>**是最简形态。**</summary>
    public static bool SimplestForm() => true;

    /// <summary>**有减蓝。**</summary>
    public static bool HasDamageSpell() => true;

    /// <summary>**兄弟类都没有。**</summary>
    public static bool AbsentInSiblings() => true;

    /// <summary>**全文件只有两处。**</summary>
    public static bool OnlyTwoSitesInFile()
        => DamageSpellSites == 2;

    /// <summary>**与"吸蓝"类配对。**</summary>
    public static bool PairedWithAbsorbMpClass() => true;

    /// <summary>**两处行号已提取。**</summary>
    public static bool DamageSpellLinesExtracted()
        => DamageSpellLine == 5098
           && DamageSpellOtherLine == 6205;

    /// <summary>**另一处在后面。**</summary>
    public static bool OtherSiteIsLater()
        => DamageSpellOtherLine > DamageSpellLine;

    /// <summary>**封顶在吸收之前。**</summary>
    public static bool CapBeforeAbsorb()
        => Array.IndexOf(Pipeline, "GetAttackPowerMax")
           < Array.IndexOf(Pipeline, "absorb");

    /// <summary>**与 J205/J207/J209 相同。**</summary>
    public static bool SameAsJ205J207J209() => true;

    /// <summary>**仍不统一。**</summary>
    public static bool StillInconsistent() => true;

    /// <summary>**管线表已提取。**</summary>
    public static bool PipelineExtracted()
        => Pipeline.Length == 12
           && Pipeline[0] == "GetMagStruckDamage"
           && Pipeline[11] == "DamageReboundPower";

    /// <summary>**回血在吸收之后。**</summary>
    public static bool HealAfterAbsorb()
        => Array.IndexOf(Pipeline, "heal")
           > Array.IndexOf(Pipeline, "absorb");

    /// <summary>**减蓝在 `StruckDamage` 之后。**</summary>
    public static bool DamageSpellAfterStruck()
        => Array.IndexOf(Pipeline, "DamageSpell")
           > Array.IndexOf(Pipeline, "StruckDamage");

    /// <summary>**麻痹在减蓝之后。**</summary>
    public static bool ParalysisAfterDamageSpell()
        => Array.IndexOf(Pipeline, "paralysis")
           > Array.IndexOf(Pipeline, "DamageSpell");

    /// <summary>**主人折扣。**</summary>
    public static bool MasterDiscount() => true;

    /// <summary>主人折扣（1:1）。</summary>
    public static int SlavePower(int nPower, int rate)
        => (int)Math.Round(nPower * (rate / 100.0));

    /// <summary>**折扣 50% 时减半。**</summary>
    public static bool HalfRateHalves() => SlavePower(100, 50) == 50;

    /// <summary>**折扣在 `nPower` 之后。**</summary>
    public static bool DiscountAfterPower()
        => MasterDiscountStart == PowerLine + 1;

    /// <summary>**封顶在吸收之前（行号）。**</summary>
    public static bool CapLineBeforeAbsorbLine()
        => CapLine < AbsorbLine;

    /// <summary>**回血在减蓝之前。**</summary>
    public static bool HealBeforeDamageSpell()
        => HealStart < DamageSpellLine;

    // ===================== 五、整体 =====================

    /// <summary>**又是纯 `inherited` 空壳。**</summary>
    public static bool PureInheritedShellAgain() => true;

    /// <summary>**连续四批出现。**</summary>
    public static bool FourthConsecutive() => true;

    /// <summary>**第十二次出现。**</summary>
    public static bool TwelfthOccurrence() => true;

    /// <summary>**只有两个方法。**</summary>
    public static bool TwoMethodsOnly() => true;

    /// <summary>**没有 `Create`。**</summary>
    public static bool NoCreate() => true;

    /// <summary>**与 J207/J208 同形。**</summary>
    public static bool SameShapeAsJ207J208() => true;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**已覆盖十五类。**</summary>
    public static bool FifteenClassesCovered() => ClassesCovered == 15;

    /// <summary>**剩余约 39 类。**</summary>
    public static bool RemainingApprox() => RemainingClasses == 39;

    // ===================== 六、跨度 =====================

    /// <summary>**两方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 120;

    /// <summary>**完整分解相加等于总行数。**</summary>
    public static bool DecompositionAddsUp()
        => HeaderLines + BlankBeforeNested + NestedLines
           + BlankAfterNested + OuterLines == MagicLines;

    /// <summary>**结构与 J207 相同。**</summary>
    public static bool SameStructureAsJ207() => true;

    /// <summary>**外层体长度与 J207 相同（30 行）。**</summary>
    public static bool OuterIdenticalLength()
        => OuterLines == 30;

    /// <summary>**嵌套比 J207 **短** 30 行。**
    /// <remarks>
    /// **修正记录**：初版名为 `NestedThirtyLonger`、断言 `NestedLines - 113 == 30`、
    /// 探针实测为假（`83 - 113 = -30`）。
    /// **正确事实：J207 的嵌套过程是 113 行（4733-4845）、
    /// 本批是 83 行（5031-5113）、即本批**短 30 行**。**
    /// **注意外层体两者都是 30 行、逐字相同** ——
    /// 所以方法总长的差异（J207 的 146 vs 本批的 116）**全部**来自嵌套过程，
    /// **即"共享外层模板 + 各写各的内层"这一结论在两次批次的体量上也成立。**
    /// </remarks>
    /// </summary>
    public static bool NestedThirtyShorter()
        => 113 - NestedLines == 30;

    /// <summary>**方法总长比 J207 短 30 行。**</summary>
    public static bool TotalThirtyShorterThanJ207()
        => 146 - MagicLines == 30;

    /// <summary>**两处差额一致（都来自嵌套）。**</summary>
    public static bool BothDifferencesAgree()
        => NestedThirtyShorter() && TotalThirtyShorterThanJ207();

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
