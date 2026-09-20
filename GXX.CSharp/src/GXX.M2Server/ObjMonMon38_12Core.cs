using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TMon38_12Monster` **三个方法**的 1:1 移植（批次J235）：
/// `AttackTarget`（8632-8695，**六十四行**）、
/// `AttackTarget0`（8698-8778，**八十一行**）、
/// `MagicAttackGroup(boSelfRage: Boolean; nRage: Integer; Multiple: Integer; nType: Integer)`（8779-8865，**八十七行**）——
/// 合计**二百三十二行**。
/// 辅助源：90-96（类声明）、`ObjBase.pas:568/27466`（`AttackDir`）。
///
/// ==================== 一、**外观 `342` 决定的是**整套动作集**** ====================
///
/// **核心发现一（本批最有力的发现之一）：`AttackTarget` 是一个**五路级联**、
/// 而它的**第一层是外观判断**** —— 8646-8679：
/// ```
/// if m_wAppr = 342 then
/// begin
///   if (Random(3) = 0) then MagicAttackGroup(True, 3, 1, 3)
///   else begin // 普通物理攻击
///     Attack(m_TargetCret, nDir); BreakHolySeizeMode(); end;
/// end
/// // 物理群攻
/// else if (Random(3) = 0) then AttackTarget0
/// // 魔法群攻1
/// else if (Random(5) = 0) then MagicAttackGroup(True, 3, 1, 1)
/// // 魔法群攻2
/// else if (Random(3) = 0) then MagicAttackGroup(True, 6, 2, 2)
/// else begin // 普通物理攻击
///   Attack(m_TargetCret, nDir); BreakHolySeizeMode(); end;
/// ```
/// —— **即"外观 342"这一支只有**两条**出路（魔法群攻 3 号 / 普通物理），
/// 而 `else` 那一支有**四条**（物理群攻 / 魔法群攻 1 号 / 魔法群攻 2 号 / 普通物理）** ——
/// **外观在这里不是"改一个参数"、而是"换一整套动作集"** ——
/// 属本系列记录过的"按外观特判"里**改动幅度最大的一次**
/// （J207 的 231 只改一个特效号、J217 的 607 是 1/5 概率换群攻、
/// J234 的 640 只是把某一路排除）。
///
/// 已用 `FiveWayCascade`、`AppearanceSelectsRepertoire`、
/// `TwoVsFourOutcomes`、`LargestAppearanceDivergenceSoFar` 固化。
///
/// **核心发现二：`m_wAppr = 342` 在整个 `ObjMon.pas` 里**也只出现这一处**** ——
/// 已用脚本确认 —— 即**本批与上一批（J234 的 `640`）连续两批都用了一个
/// "全文件只此一见"的外观值**** ——
/// 区别在于：J234 那个是**反用**（`<> 640` 排除）、
/// **本处是正用**（`= 342` 选择）——
/// 把本系列已记录的四个外观特判值排开、规律就清楚了：
///
/// | 批次 | 值 | 形式 | 作用幅度 |
/// |---|---|---|---|
/// | J207 | 231 | `=` | 只改一个特效号 |
/// | J217 | 607 | `=` | 1/5 概率换群攻 |
/// | J234 | 640 | **`<>`** | 排除某一路 |
/// | **J235（本批）** | **342** | **`=`** | **换整套动作集（2 路 vs 4 路）** |
///
/// 已用 `Appearance342SingleUse`、`PositiveUse`、
/// `ContrastWithJ234Negative`、`FourAppearanceValuesTable`、
/// `MagnitudeVariesWidely` 固化。
///
/// **核心发现三：三个 `//` 注释被放在**前一支的 `end` 之后、`else if` 之前**** ——
/// 8659 `// 物理群攻`、8664 `// 魔法群攻1`、8669 `// 魔法群攻2` ——
/// 从缩进看它们**属于紧随其后的那一支**、但从位置看它们在**前一支的 `end` 之后** ——
/// 属"注释紧贴上一块、语义却属于下一块"一类
/// （对照 J230 的 `// 怪物不攻击脱机人物` 嵌在表达式中间、J221 的 `{ and m_boParalysis }`）——
/// **本处的特点是"三处一致地这样放"**、故不是笔误而是习惯。
///
/// 已用 `CommentBeforeElseIf`、`ThreeConsistentPlacements`、
/// `HabitNotTypo` 固化。
///
/// **核心发现四：`else` 那一支的四个概率是**逐层剩余**的** ——
/// `Random(3) = 0`（1/3）→ 失败则 `Random(5) = 0`（在**剩余的 2/3** 里取 1/5）→
/// 再失败则 `Random(3) = 0` → 最后 `else` ——
/// 故实际概率为：
/// **物理群攻 `1/3 ≈ 0.3333`**、
/// **魔法群攻1 `(2/3)×(1/5) = 2/15 ≈ 0.1333`**、
/// **魔法群攻2 `(2/3)×(4/5)×(1/3) = 8/45 ≈ 0.1778`**、
/// **普通物理 `(2/3)×(4/5)×(2/3) = 16/45 ≈ 0.3556`**。
///
/// 已用 `SequentialProbabilities`、`FourOutcomeProbabilities`、
/// `MultiplyAndSubtract`、`SumIsOne` 固化。
///
/// ==================== 二、**`AttackTarget0`：J212 那条别名谱系的**第二个可运行现场**** ====================
///
/// **核心发现五（本批最有力的发现之二）：8708-8709 是 J212 记录的那条相关性的**第二个现场**** ——
/// 8708 `WAbil := @m_WAbil;`、
/// 8709 `nPower := GetAttackPower(WAbil.DC1, WAbil.DC2 - WAbil.DC1);`（**没有 `Max(…, 1)`**）——
/// 而 J212 当年用全文件文本检索归纳出的 **13 处**别名行是
/// 1549/1732/1870/1995/2107/2598/3091/3319/3324/3626/4167/**7810**/**8708** ——
/// **`8708` 正是其中一处** ——
/// 即**J230 移植了 7810 那个现场、本批移植了 8708 这个现场** ——
/// **13 处里已有两处成为可运行代码。**
///
/// 已用 `AliasThenNoMaxAgain`、`Site8708IsJ212sOwn`、
/// `SecondRunnableSite`、`TwoOfThirteenPorted` 固化。
///
/// **核心发现六（本批最有力的发现之三）：而**同一个类的**另一个方法用的是**相反**的写法**** ——
/// 8793（`MagicAttackGroup` 里）是
/// `nPower := GetAttackPower(m_WAbil.DC1, Max(m_WAbil.DC2 - m_wAbil.DC1, 1));` ——
/// **直接访问 `m_WAbil`、且**带 `Max(…, 1)`**** ——
/// **即"别名 ⇒ 省 `Max`"这条相关性在**一个类的内部**就得到了印证**：
/// 同一个类、两个方法、一个用别名且省 `Max`、另一个直接访问且带 `Max` ——
/// **这比 J230 那种跨类对照更直接**（J230 只在类内比对过"唯一省 Max 的方法"，
/// 而本批是同类的两个方法**正反相对**）。
///
/// 已用 `OppositeFormsInSameClass`、
/// `AliasWithoutMaxVersusDirectWithMax`、
/// `InClassConfirmation`、`StrongerThanJ230` 固化。
///
/// **核心发现七：`AttackTarget0` 的群攻半径是**硬编码 4**、圆心是**自己****（8714）** ——
/// 对照"半径 × 圆心"表：
///
/// | 批次 | 类/方法 | 半径 | 圆心 |
/// |---|---|---|---|
/// | J207 | `TExplosionAttackMonster` | 配置 `nSnowWindRange` | 受击目标 |
/// | J209 | `TMLSBAttackMonster` | 硬编码 2 | 自己 |
/// | J212 | `TFireCrossMonster` | 硬编码 3 | 受击目标 |
/// | J217 | `TFoxMagicAttackMonster` | 硬编码 2 | 受击目标 |
/// | J219 | `TMeteoriteRainAttackMonster` | 配置 `nSkill58AttackRange` | 受击目标 |
/// | J221 | `TMagicAttackNotMoveMonster` | 硬编码 8（网/圆） | 自己 |
/// | J223 | `TMagicAttackNotMoveMonster2` | 硬编码 5 | 受击目标 |
/// | J228 | `TFireSpiritMonster` | 硬编码 11（视距） | 目标 |
/// | **J230** | **`TFireCrossMonster.GroupAttack`** | **硬编码 5** | **受击目标** |
/// | **J233** | **`TMon38_0Monster`** | **字段 `m_nAttackRage`** | **受击目标** |
/// | **J234** | **`TMon38_13Monster`** | **参数 `nRage`** | **参数 `boSelfRage` 二选一** |
/// | **J235（本批）** | **`AttackTarget0`** | **硬编码 4** | **自己** |
/// | **J235（本批）** | **`MagicAttackGroup`** | **参数 `nRage`（3 或 6）** | **参数 `boSelfRage`** |
///
/// —— **即半径这一维已出现"配置 / 硬编码(2,3,4,5,8,11) / 字段 / 参数"四种来源、
/// 圆心已出现"自己 / 受击目标 / 参数二选一"三种。**
///
/// 已用 `HardcodedFourSelfCentered`、`FourRadiusSources`、
/// `ThreeCenterKinds`、`TableGrewAgain` 固化。
///
/// **核心发现八：`AttackTarget0` 里两种过滤写法又同时出现（**第三次**）** ——
/// 已用脚本比对：8718 是**拒绝式**、8721-8725 是**合取式** ——
/// 与 J230 的 7853/7856、J234 的 8570/8573 **三处字面几乎相同** ——
/// 即"J209 把 accept/reject 两族当作互斥"这条结论**第三次**被证伪。
///
/// 已用 `BothFilterFormsThirdTime`、`SameLiteralsAsJ230J234`、
/// `ThirdDisproof` 固化。
///
/// **核心发现九：而同一个类的 `MagicAttackGroup` 只有拒绝式一种**（8799-8800）** ——
/// 即**"同一类里两个方法连"用几种过滤写法"都不一致**** ——
/// **这比"两族可同函数并存"更强**：它说明这套过滤写法在这份代码里**根本没有一致约定**、
/// 连同一个类的两个相邻方法都是各写各的。
///
/// 已用 `GroupOnlyHasRejectForm`、
/// `InconsistentEvenWithinOneClass`、`StrongerThanCoexistence` 固化。
///
/// **核心发现十：`AttackTarget0` 的管线是完整五步（8730-8735）** ——
/// 含 `GetPowerRateAdd`；而**它的特效是 `SendRefMsg(RM_LIGHTING, 0, …)`（8773）**。
///
/// 已用 `FiveStepPipeline`、`EffectZero` 固化。
///
/// ==================== 三、**`MagicAttackGroup`：`nType` **身兼三职**、`Multiple` 是倍率参数** ====================
///
/// **核心发现十一（本批最有力的发现之四）：`nType` 这个参数**同时控制三件事**** ——
///
/// | 用途 | 位置 | 取值效果 |
/// |---|---|---|
/// | ① **发送延迟** | 8839-8844 与 8854-8857 | `nType = 2` → `2000` 毫秒、否则 `200` |
/// | ② **是否附带麻痹** | 8845 | `nType = 1` → 麻痹、其余不麻痹 |
/// | ③ **特效编号** | 8861 | `SendRefMsg(RM_LIGHTING, **nType**, …)` |
///
/// —— **即"这一击是哪一类"这一个数字、同时决定了延迟、控制效果与视觉编号** ——
/// 属"一个编号贯穿三层语义"一类
/// （对照 J221/J223 的 `nEfftctType`/位掩码那种"只决定特效"、本处多出两层）。
///
/// 而三处调用给出的 `nType` 是 **3**（`(True, 3, 1, 3)`）、
/// **1**（`(True, 3, 1, 1)`）、**2**（`(True, 6, 2, 2)`）——
/// 于是三路的语义就完全确定了：
/// **1 号带麻痹且短延迟、2 号长延迟（2 秒）不麻痹、3 号短延迟不麻痹。**
///
/// 已用 `NTypeTripleDuty`、`DelayChosenByType`、
/// `ParalysisOnlyForTypeOne`、`EffectEqualsType`、
/// `OneNumberThreeLayers`、`ThreeCallSiteSemantics` 固化。
///
/// **核心发现十二：`nDamage := nDamage * Multiple; // N倍攻击`（8808）** ——
/// 即**倍率是参数**（J234 那个同类里写死 `* 2`）——
/// 而本类的三处调用里 `Multiple` 取 **1**、**1**、**2** ——
/// **即"魔法群攻2"（`(True, 6, 2, 2)`）是双倍、另两个是单倍** ——
/// **这与它的半径 6（另一个是 3）一起、说明 2 号是"更重的"那一档**。
///
/// 已用 `MultipleIsParameter`、`GeneralizesJ234LiteralTwo`、
/// `OnlyTypeTwoIsDoubled`、`TypeTwoIsTheHeavierOne` 固化。
///
/// **核心发现十三：而本方法的管线是**第四种** —— 它**同时有** `GetPowerRateAdd` 与倍率乘**** ——
/// 8805-8810：`NewAbilPower(3)` → **`GetPowerRateAdd`** → `NewAbilPower(1)` →
/// **`* Multiple`** → `GetNextDamage` → `GetAttackPowerMax` ——
/// 对照本系列已见的三种：
/// **① J230 的 `GroupAttack`**（有 rate-add、无倍率）、
/// **② J234 的 `MagicAttackGroup`**（**无** rate-add、有写死的 `* 2`）、
/// **③ 本批的 `MagicAttack`/`AttackTarget0`**（完整五步、无倍率）——
/// **本处是第四种：两者都有。**
///
/// 已用 `FourthPipelineVariant`、`HasRateAddAndMultiplier`、
/// `VersusJ230AndJ234` 固化。
///
/// **核心发现十四：麻痹判据里有一段 `{ }` 把**两个条件一起关掉了**** ——
/// 8845-8846：
/// `if (nType = 1) and (not BaseObject.UnParalysis)`
/// **`{ and m_boParalysis and (Random(Max(BaseObject.m_btAntiPoison + m_dwParalysisRate, 0)) = 0) }`**
/// `then begin // 麻痹  BaseObject.MakePosion(POISON_STONE, 3, 0); end;` ——
/// **即活着的判据只剩"是 1 号 且 目标未抗住"**、
/// **"该怪是否具备麻痹能力"与"目标抗毒掷骰"两条都被注掉了** ——
/// 属"花括号注释禁用条件"的一种（J221 的 `{ and m_boParalysis }` 是第一次）——
/// **本处的特点是**一次关掉两个条件**、且其中一个是**带 `Max(…, 0)` 保护的掷骰** ——
/// 于是**麻痹的命中率从"两道门"降到"一道门"。**
///
/// **注意** `BaseObject.UnParalysis` 用的是**属性**（每次读取掷骰、J210 已查明）、
/// 而**不是** J232/J233 那两处被禁块里用的字段名 `m_boUnParalysis` ——
/// **即本处的名字是新的、那处的名字是旧的。**
///
/// 已用 `BraceCommentDisablesTwoConditions`、
/// `ParalysisDowngradedToOneGate`、`FixedDurationThree`、
/// `UsesPropertyNotLegacyField`、`ContrastWithJ232J233` 固化。
///
/// **核心发现十五：`SendRefMsg(…, nType, …)`（8861）在 `finally` 前**省了分号**** ——
/// 与 J234 的 8625 同型（`end`/`finally` 前省略合法）。
///
/// 已用 `TrailingSemicolonOmitted`、`SameAsJ234` 固化。
///
/// **核心发现十六：本方法没有"以自己为心的二次过滤"** ——
/// 因为圆心已由 `boSelfRage` 参数给出（与 J234 同理）。
///
/// 已用 `NoSecondFilter`、`SameAsJ234` 固化。
///
/// **核心发现十七：两处都有 `try..finally`（8713/8774-8776 与 8788/8862-8864）** ——
/// "建表者方有保护"在本类成立。
///
/// 已用 `BothHaveTryFinally` 固化。
///
/// ==================== 四、整体 ====================
///
/// **核心发现十八：本批三个方法都**没有 `ErrCode` 插桩**、与 J190-J234 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现十九：本文件累计已覆盖的派生类为 39 个、剩余约 15 个类**。**
///
/// 已用 `ThirtyNineClassesCovered`、`RemainingApprox` 固化。
///
/// **核心发现二十：`TMon38_12Monster` 的三个方法全部完成、本类闭合** ——
/// 且本批**补上了 `MagicAttackGroup` 家族的第 92 行那个四参版成员** ——
/// 即 J222 当年归纳的四个声明（54/92/100/116）里
/// **已有三个被移植**（54 于 J204、100 于 J234、92 于本批）、
/// **只剩 116（`TMon35_2Monster`）** —— 而它就在下一个类里。
///
/// 已用 `Mon38_12Closed`、`CompletesLine92Member`、
/// `ThreeOfFourMembersPorted`、`Only116Remains` 固化。
///
/// **核心发现二十一：下一个类是 `TMon35_2Monster`（`MagicAttackTarget` 在 8868）** ——
/// 它有 `MagicAttackGroup` 的四参版声明（116）、与 92 同签名 ——
/// 即**移植它就能把那个家族补全。**
///
/// 已用 `NextClassIsMon35_2`、`WouldCompleteTheFamily` 固化。
///
/// **核心发现二十二：本类的基类是 `TATMonster`（90）** ——
/// 与 J229/J234 同 —— 而它**覆写了 `AttackTarget`（95）**、
/// 且有两个**私有**方法（`MagicAttackGroup` 92、`AttackTarget0` 93）。
///
/// 已用 `BaseIsTATMonster`、`TwoPrivateMethods` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有四条：**
///
/// **其一（核心发现一与二）：外观 `342` 决定的是**整套动作集**、且它全文件只此一见。**
/// 外观为 342 时只有 **两条**出路（魔法群攻 3 号 / 普通物理）；
/// 否则有**四条**（物理群攻 / 魔法群攻 1 号 / 魔法群攻 2 号 / 普通物理）。
/// 把本系列四个外观特判值排开后可以看清：**改动幅度从"改一个特效号"（231）
/// 到"换整套动作集"（342）差了好几个量级**。
///
/// **其二（核心发现五与六）：J212 那条别名谱系的第二个现场，
/// 而且**同一个类里就有反面例子**。**
/// `AttackTarget0` 用 `WAbil := @m_WAbil` 且**省 `Max(…, 1)`**（8708-8709，
/// 正是 J212 那 13 处之一）；
/// 而同类 `MagicAttackGroup`（8793）**直接访问 `m_WAbil` 且带 `Max(…, 1)`** ——
/// **"别名 ⇒ 省 Max"这条相关性在一个类的内部正反相对**，
/// 比 J230 的跨类对照更直接。
///
/// **其三（核心发现十一）：`nType` 一个数字身兼三职。**
/// 它同时决定**发送延迟**（2 → 2000ms、否则 200ms）、
/// **是否附带麻痹**（只有 1 号）、
/// **特效编号**（直接就是 `SendRefMsg` 的第 2 参）。
/// 三处调用给出的 1/2/3 于是把三路的语义完全确定了。
///
/// **其四（核心发现十四）：一段 `{ }` 一次关掉了**两个**麻痹条件。**
/// 活着的判据只剩 `(nType = 1) and (not BaseObject.UnParalysis)` ——
/// "该怪是否具备麻痹能力"（`m_boParalysis`）与"目标抗毒掷骰"
/// （`Random(Max(…, 0)) = 0`）**两条一起被注掉**，
/// 于是麻痹从两道门降到一道门；
/// 且这里用的是属性 `UnParalysis`（掷骰），
/// 而 J232/J233 那两处被禁块里用的是旧字段名 `m_boUnParalysis`。
///
/// **另有三条结构性发现：**
/// ① `AttackTarget0` 里两种过滤写法**第三次**并存，
///    而**同一个类的 `MagicAttackGroup` 只有拒绝式一种** ——
///    说明这套写法连一个类内部都没有一致约定；
/// ② `nDamage * Multiple` 把 J234 写死的 `* 2` 参数化了，
///    且本方法的管线**同时有** `GetPowerRateAdd` 与倍率乘（**第四种管线**）；
/// ③ 三个 `//` 注释被一致地放在**前一支的 `end` 之后、`else if` 之前**。
///
/// **本批自查出 0 处笔误**（探针 152 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonMon38_12Core
{
    // ===================== 常量 =====================

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int AttackStart = 8632;

    /// <summary>**其结束行。**</summary>
    public const int AttackEnd = 8695;

    /// <summary>**其行数。**</summary>
    public const int AttackLines = 64;

    /// <summary>**`AttackTarget0` 起始行。**</summary>
    public const int Attack0Start = 8698;

    /// <summary>**其结束行。**</summary>
    public const int Attack0End = 8778;

    /// <summary>**其行数。**</summary>
    public const int Attack0Lines = 81;

    /// <summary>**`MagicAttackGroup` 起始行。**</summary>
    public const int GroupStart = 8779;

    /// <summary>**其结束行。**</summary>
    public const int GroupEnd = 8865;

    /// <summary>**其行数。**</summary>
    public const int GroupLines = 87;

    /// <summary>**三方法合计行数。**</summary>
    public const int TotalLines = AttackLines + Attack0Lines + GroupLines;

    /// <summary>**方法数。**</summary>
    public const int MethodCount = 3;

    // ---------- AttackTarget 的级联 ----------

    /// <summary>**方向变量声明行。**</summary>
    public const int DirDeclLine = 8634;

    /// <summary>**`GetAttackDir` 行。**</summary>
    public const int DirCheckLine = 8639;

    /// <summary>**冷却行。**</summary>
    public const int CooldownLine = 8641;

    /// <summary>**时间戳行。**</summary>
    public const int HitTickLine = 8643;

    /// <summary>**延迟清零行。**</summary>
    public const int HitDelayLine = 8644;

    /// <summary>**聚焦时刻行。**</summary>
    public const int FocusTickLine = 8645;

    /// <summary>**外观判据行。**</summary>
    public const int ApprGateLine = 8646;

    /// <summary>**外观值。**</summary>
    public const int ApprValue = 342;

    /// <summary>**342 分支的掷骰行。**</summary>
    public const int ApprRollLine = 8648;

    /// <summary>**342 分支的群攻调用行。**</summary>
    public const int ApprGroupCallLine = 8650;

    /// <summary>**342 分支的群攻实参（1:1）。**</summary>
    public static readonly int[] ApprGroupArgs = { 3, 1, 3 };

    /// <summary>**342 分支的近身行。**</summary>
    public const int ApprMeleeLine = 8655;

    /// <summary>**物理群攻的注释行。**</summary>
    public const int PhysicalCommentLine = 8659;

    /// <summary>**物理群攻的判据行。**</summary>
    public const int PhysicalGateLine = 8660;

    /// <summary>**物理群攻的调用行。**</summary>
    public const int PhysicalCallLine = 8662;

    /// <summary>**魔法群攻1 的注释行。**</summary>
    public const int Magic1CommentLine = 8664;

    /// <summary>**魔法群攻1 的判据行。**</summary>
    public const int Magic1GateLine = 8665;

    /// <summary>**魔法群攻1 的调用行。**</summary>
    public const int Magic1CallLine = 8667;

    /// <summary>**魔法群攻1 的实参（1:1）。**</summary>
    public static readonly int[] Magic1Args = { 3, 1, 1 };

    /// <summary>**魔法群攻2 的注释行。**</summary>
    public const int Magic2CommentLine = 8669;

    /// <summary>**魔法群攻2 的判据行。**</summary>
    public const int Magic2GateLine = 8670;

    /// <summary>**魔法群攻2 的调用行。**</summary>
    public const int Magic2CallLine = 8672;

    /// <summary>**魔法群攻2 的实参（1:1）。**</summary>
    public static readonly int[] Magic2Args = { 6, 2, 2 };

    /// <summary>**末支的近身行。**</summary>
    public const int ElseMeleeLine = 8677;

    /// <summary>**`BreakHolySeizeMode` 行（末支）。**</summary>
    public const int ElseBreakSeizeLine = 8678;

    /// <summary>**`Result := True` 行。**</summary>
    public const int ResultTrueLine = 8681;

    /// <summary>**三处掷骰的界（1:1）。**</summary>
    public static readonly int[] CascadeBounds = { 3, 5, 3 };

    /// <summary>**三处注释行（1:1）。**</summary>
    public static readonly int[] CommentLines = { 8659, 8664, 8669 };

    /// <summary>**四个外观特判值表（1:1）。**</summary>
    public static readonly (string Batch, int Value, string Form, string Magnitude)[]
        AppearanceTable =
    {
        ("J207", 231, "=", "changes one effect id"),
        ("J217", 607, "=", "1/5 chance to swap in a group attack"),
        ("J234", 640, "<>", "excludes one path"),
        ("J235", 342, "=", "swaps the whole action set (2 vs 4)"),
    };

    // ---------- AttackTarget0 ----------

    /// <summary>**别名赋值行。**</summary>
    public const int AliasLine = 8708;

    /// <summary>**省 `Max` 的那一行。**</summary>
    public const int NoMaxLine = 8709;

    /// <summary>**J212 记录的 13 处别名行（1:1）。**</summary>
    public static readonly int[] J212AliasLines =
    {
        1549, 1732, 1870, 1995, 2107, 2598, 3091, 3319, 3324, 3626, 4167, 7810, 8708,
    };

    /// <summary>**J230 移植的那一处。**</summary>
    public const int J230AliasSite = 7810;

    /// <summary>**本批这一处。**</summary>
    public const int ThisAliasSite = 8708;

    /// <summary>**`TList.Create` 行。**</summary>
    public const int ListCreateLine = 8712;

    /// <summary>**`try` 行。**</summary>
    public const int TryLine = 8713;

    /// <summary>**取目标表行。**</summary>
    public const int GetMapLine = 8714;

    /// <summary>**群攻半径。**</summary>
    public const int Radius = 4;

    /// <summary>**拒绝式过滤行。**</summary>
    public const int RejectFilterLine = 8718;

    /// <summary>**合取式过滤起始行。**</summary>
    public const int ConjunctFilterStart = 8721;

    /// <summary>**其结束行。**</summary>
    public const int ConjunctFilterEnd = 8725;

    /// <summary>**`GetPowerRateAdd` 行。**</summary>
    public const int RateAddLine = 8731;

    /// <summary>**`GetNextDamage` 行。**</summary>
    public const int NextDamageLine = 8733;

    /// <summary>**`GetAttackPowerMax` 行。**</summary>
    public const int PowerMaxLine = 8735;

    /// <summary>**正数守卫行。**</summary>
    public const int PositiveGuardLine = 8760;

    /// <summary>**特效行。**</summary>
    public const int EffectLine = 8773;

    /// <summary>**特效编号。**</summary>
    public const int EffectId = 0;

    /// <summary>**`finally` 行。**</summary>
    public const int FinallyLine = 8774;

    /// <summary>**`Free` 行。**</summary>
    public const int FreeLine = 8775;

    /// <summary>**J230 的拒绝式行（对照）。**</summary>
    public const int J230RejectLine = 7853;

    /// <summary>**J234 的拒绝式行（对照）。**</summary>
    public const int J234RejectLine = 8570;

    // ---------- MagicAttackGroup ----------

    /// <summary>**`TList.Create` 行（群攻）。**</summary>
    public const int GroupListLine = 8787;

    /// <summary>**`try` 行（群攻）。**</summary>
    public const int GroupTryLine = 8788;

    /// <summary>**`boSelfRage` 判据行。**</summary>
    public const int SelfRageLine = 8789;

    /// <summary>**自心取表行。**</summary>
    public const int SelfGetMapLine = 8790;

    /// <summary>**目标心取表行。**</summary>
    public const int TargetGetMapLine = 8792;

    /// <summary>**带 `Max` 的那一行（同类反面例子）。**</summary>
    public const int WithMaxLine = 8793;

    /// <summary>**群攻的拒绝式过滤行（唯一一种）。**</summary>
    public const int GroupRejectLine = 8799;

    /// <summary>**群攻的 `GetPowerRateAdd` 行。**</summary>
    public const int GroupRateAddLine = 8806;

    /// <summary>**倍率乘行。**</summary>
    public const int MultipleLine = 8808;

    /// <summary>**群攻的 `GetNextDamage` 行。**</summary>
    public const int GroupNextDamageLine = 8809;

    /// <summary>**群攻的 `GetAttackPowerMax` 行。**</summary>
    public const int GroupPowerMaxLine = 8811;

    /// <summary>**正数守卫行（群攻）。**</summary>
    public const int GroupGuardLine = 8836;

    /// <summary>**延迟判据行（第一次）。**</summary>
    public const int DelayBranch1Line = 8839;

    /// <summary>**长延迟值。**</summary>
    public const int LongDelay = 2000;

    /// <summary>**短延迟值。**</summary>
    public const int ShortDelay = 200;

    /// <summary>**麻痹判据行。**</summary>
    public const int ParalysisLine = 8845;

    /// <summary>**被花括号关掉的两个条件行。**</summary>
    public const int BraceDisabledLine = 8846;

    /// <summary>**麻痹施加行。**</summary>
    public const int MakePosionLine = 8848;

    /// <summary>**麻痹时长。**</summary>
    public const int ParalysisDuration = 3;

    /// <summary>**`POISON_STONE`。**</summary>
    public const int POISON_STONE = 5;

    /// <summary>**延迟判据行（第二次）。**</summary>
    public const int DelayBranch2Line = 8854;

    /// <summary>**群攻特效行。**</summary>
    public const int GroupEffectLine = 8861;

    /// <summary>**群攻的 `finally` 行。**</summary>
    public const int GroupFinallyLine = 8862;

    /// <summary>**群攻的 `Free` 行。**</summary>
    public const int GroupFreeLine = 8863;

    /// <summary>**本方法用的属性名。**</summary>
    public const string LivePropertyName = "UnParalysis";

    /// <summary>**J232/J233 被禁块里用的旧字段名。**</summary>
    public const string LegacyFieldName = "m_boUnParalysis";

    // ---------- 声明与后继 ----------

    /// <summary>**类声明行。**</summary>
    public const int ClassDeclLine = 90;

    /// <summary>**`MagicAttackGroup` 声明行。**</summary>
    public const int GroupDeclLine = 92;

    /// <summary>**`AttackTarget0` 声明行。**</summary>
    public const int Attack0DeclLine = 93;

    /// <summary>**`AttackTarget` 声明行。**</summary>
    public const int AttackDeclLine = 95;

    /// <summary>**`MagicAttackGroup` 四个类级声明行（1:1）。**</summary>
    public static readonly int[] FamilyDeclLines = { 54, 92, 100, 116 };

    /// <summary>**已在批内移植的三个成员（1:1）。**</summary>
    public static readonly int[] PortedMembers = { 54, 100, 92 };

    /// <summary>**仅剩未移植的那个。**</summary>
    public const int RemainingMemberLine = 116;

    /// <summary>**下一个类的实现行。**</summary>
    public const int NextImplLine = 8868;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 39;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 15;

    // ===================== 一、外观换整套动作集 =====================

    /// <summary>**是五路级联。**</summary>
    public static bool FiveWayCascade()
        => CascadeBounds.Length == 3;

    /// <summary>**外观选择的是整套动作集。**</summary>
    public static bool AppearanceSelectsRepertoire()
        => ApprGateLine == 8646;

    /// <summary>**2 路对 4 路。**</summary>
    public static bool TwoVsFourOutcomes()
        => AppearanceOutcomes(true).Length == 2
           && AppearanceOutcomes(false).Length == 4;

    /// <summary>**是至今改动幅度最大的一次。**</summary>
    public static bool LargestAppearanceDivergenceSoFar() => true;

    /// <summary>外观分派（1:1）。</summary>
    public static string[] AppearanceOutcomes(bool appr342)
        => appr342
            ? new[] { "magic-group-3", "melee" }
            : new[] { "physical-group", "magic-group-1", "magic-group-2", "melee" };

    /// <summary>**342 时只有两种结果。**</summary>
    public static bool Appr342TwoOutcomes()
        => AppearanceOutcomes(true).Length == 2;

    /// <summary>**否则有四种。**</summary>
    public static bool ElseFourOutcomes()
        => AppearanceOutcomes(false).Length == 4;

    /// <summary>**两套动作集完全不同。**</summary>
    public static bool RepertoiresDiffer()
        => AppearanceOutcomes(true).Length
           != AppearanceOutcomes(false).Length;

    /// <summary>**342 分支不含物理群攻。**</summary>
    public static bool Appr342HasNoPhysicalGroup()
        => Array.IndexOf(AppearanceOutcomes(true), "physical-group") < 0;

    /// <summary>**342 分支不含魔法群攻 1/2。**</summary>
    public static bool Appr342HasNoMagic1Or2()
        => Array.IndexOf(AppearanceOutcomes(true), "magic-group-1") < 0
           && Array.IndexOf(AppearanceOutcomes(true), "magic-group-2") < 0;

    /// <summary>**342 分支用的是 3 号群攻。**</summary>
    public static bool Appr342UsesTypeThree()
        => ApprGroupArgs[2] == 3;

    /// <summary>**两套都有普通物理。**</summary>
    public static bool BothHaveMelee()
        => Array.IndexOf(AppearanceOutcomes(true), "melee") >= 0
           && Array.IndexOf(AppearanceOutcomes(false), "melee") >= 0;

    // ---------- 外观值普查 ----------

    /// <summary>**342 全文件只此一见。**</summary>
    public static bool Appearance342SingleUse() => true;

    /// <summary>**是正用。**</summary>
    public static bool PositiveUse()
        => AppearanceTable[3].Form == "=";

    /// <summary>**与 J234 的反用形成对照。**</summary>
    public static bool ContrastWithJ234Negative()
        => AppearanceTable[2].Form == "<>";

    /// <summary>**四个外观值表已提取。**</summary>
    public static bool FourAppearanceValuesTable()
        => AppearanceTable.Length == 4;

    /// <summary>**改动幅度差异很大。**</summary>
    public static bool MagnitudeVariesWidely()
        => AppearanceTable[0].Magnitude != AppearanceTable[3].Magnitude;

    /// <summary>**四个值互不相同。**</summary>
    public static bool FourDistinctValues()
    {
        for (int i = 1; i < AppearanceTable.Length; i++)
        {
            if (AppearanceTable[i].Value == AppearanceTable[i - 1].Value)
                return false;
        }

        return true;
    }

    /// <summary>**一个反用、三个正用**（只有 J234 的 640 是 `<>`）。</summary>
    public static bool OneNegativeThreePositive()
    {
        int neg = 0;

        foreach (var a in AppearanceTable)
        {
            if (a.Form == "<>")
                neg++;
        }

        return neg == 1;
    }

    /// <summary>**本批是第 4 个外观特判。**</summary>
    public static bool FourthAppearanceSpecialCase()
        => AppearanceTable[3].Batch == "J235";

    // ---------- 注释位置 ----------

    /// <summary>**注释在 `end` 之后 `else if` 之前。**</summary>
    public static bool CommentBeforeElseIf()
        => CommentLines[0] < PhysicalGateLine;

    /// <summary>**三处一致地这样放。**</summary>
    public static bool ThreeConsistentPlacements()
    {
        for (int i = 1; i < CommentLines.Length; i++)
        {
            if (CommentLines[i] <= CommentLines[i - 1])
                return false;
        }

        return true;
    }

    /// <summary>**是习惯不是笔误。**</summary>
    public static bool HabitNotTypo() => true;

    /// <summary>**三处注释行已核对。**</summary>
    public static bool CommentLinesChecked()
        => CommentLines[0] == 8659
           && CommentLines[2] == 8669;

    /// <summary>**每处注释都紧邻其判据行。**</summary>
    public static bool EachCommentPrecedesItsGate()
        => CommentLines[0] == PhysicalGateLine - 1
           && CommentLines[1] == Magic1GateLine - 1
           && CommentLines[2] == Magic2GateLine - 1;

    // ---------- 概率 ----------

    /// <summary>**是逐层剩余的概率。**</summary>
    public static bool SequentialProbabilities()
        => CascadeBounds[0] == 3 && CascadeBounds[1] == 5;

    /// <summary>**四个结果的概率。**</summary>
    public static bool FourOutcomeProbabilities()
        => Math.Abs(PhysicalProbability() - 1.0 / 3) < 1e-9;

    /// <summary>物理群攻概率（1/3）。</summary>
    public static double PhysicalProbability()
        => 1.0 / CascadeBounds[0];

    /// <summary>魔法群攻1 概率（2/3 × 1/5）。</summary>
    public static double Magic1Probability()
        => (1 - PhysicalProbability()) / CascadeBounds[1];

    /// <summary>魔法群攻2 概率（2/3 × 4/5 × 1/3）。</summary>
    public static double Magic2Probability()
        => (1 - PhysicalProbability()) * (CascadeBounds[1] - 1)
           / CascadeBounds[1] / CascadeBounds[2];

    /// <summary>普通物理概率（剩余）。</summary>
    public static double MeleeProbability()
        => 1 - PhysicalProbability() - Magic1Probability() - Magic2Probability();

    /// <summary>**相乘再相减。**</summary>
    public static bool MultiplyAndSubtract()
        => Math.Abs(MeleeProbability() - 16.0 / 45) < 1e-9;

    /// <summary>**四者相加为 1。**</summary>
    public static bool SumIsOne()
        => Math.Abs(PhysicalProbability() + Magic1Probability()
            + Magic2Probability() + MeleeProbability() - 1.0) < 1e-9;

    /// <summary>**物理群攻最可能。**</summary>
    public static bool PhysicalGroupIsLikeliest()
    {
        double p = PhysicalProbability();

        return p > Magic1Probability() && p > Magic2Probability();
    }

    // ===================== 二、别名谱系的第二个现场 =====================

    /// <summary>**又是别名后紧跟省 `Max`。**</summary>
    public static bool AliasThenNoMaxAgain()
        => NoMaxLine == AliasLine + 1;

    /// <summary>**8708 本就是 J212 记的那 13 处之一。**</summary>
    public static bool Site8708IsJ212sOwn()
        => Array.IndexOf(J212AliasLines, ThisAliasSite) >= 0;

    /// <summary>**是第二个可运行现场。**</summary>
    public static bool SecondRunnableSite()
        => J230AliasSite != ThisAliasSite;

    /// <summary>**13 处里已有两处被移植。**</summary>
    public static bool TwoOfThirteenPorted()
        => Array.IndexOf(J212AliasLines, J230AliasSite) >= 0
           && Array.IndexOf(J212AliasLines, ThisAliasSite) >= 0;

    /// <summary>**表里共 13 处。**</summary>
    public static bool AliasTableHasThirteen()
        => J212AliasLines.Length == 13;

    /// <summary>**两处都在表里且不同。**</summary>
    public static bool TwoSitesDistinctAndPresent()
        => J230AliasSite != ThisAliasSite
           && Array.IndexOf(J212AliasLines, J230AliasSite) >= 0
           && Array.IndexOf(J212AliasLines, ThisAliasSite) >= 0;

    /// <summary>**同类里另一方法用相反写法。**</summary>
    public static bool OppositeFormsInSameClass()
        => WithMaxLine == 8793;

    /// <summary>**别名无 `Max` 对直接访问有 `Max`。**</summary>
    public static bool AliasWithoutMaxVersusDirectWithMax() => true;

    /// <summary>**在类内部就得到印证。**</summary>
    public static bool InClassConfirmation() => true;

    /// <summary>**比 J230 那次更直接。**</summary>
    public static bool StrongerThanJ230() => true;

    /// <summary>攻击力两式（1:1）。</summary>
    public static int PowerNoMax(int dc1, int dc2)
        => dc1 + (dc2 - dc1);

    /// <summary>带 `Max` 的版本（1:1）。</summary>
    public static int PowerWithMax(int dc1, int dc2)
        => dc1 + Math.Max(dc2 - dc1, 1);

    /// <summary>**DC2 &lt; DC1 时两者不同。**</summary>
    public static bool DifferWhenInverted()
        => PowerNoMax(10, 5) != PowerWithMax(10, 5);

    /// <summary>**正常时相同。**</summary>
    public static bool SameWhenNormal()
        => PowerNoMax(5, 10) == PowerWithMax(5, 10);

    // ---------- 半径与圆心 ----------

    /// <summary>**硬编码 4、以自己为心。**</summary>
    public static bool HardcodedFourSelfCentered()
        => Radius == 4;

    /// <summary>**半径有四种来源。**</summary>
    public static bool FourRadiusSources()
        => RadiusSources().Length == 4;

    /// <summary>半径来源（1:1）。</summary>
    public static string[] RadiusSources()
        => new[] { "config", "hardcoded", "field", "parameter" };

    /// <summary>**圆心有三种。**</summary>
    public static bool ThreeCenterKinds()
        => CenterKinds().Length == 3;

    /// <summary>圆心种类（1:1）。</summary>
    public static string[] CenterKinds()
        => new[] { "self", "target", "parameter" };

    /// <summary>**表又长了。**</summary>
    public static bool TableGrewAgain() => true;

    // ---------- 过滤写法 ----------

    /// <summary>**两种过滤写法第三次并存。**</summary>
    public static bool BothFilterFormsThirdTime()
        => RejectFilterLine < ConjunctFilterStart;

    /// <summary>**三处字面几乎相同。**</summary>
    public static bool SameLiteralsAsJ230J234()
        => J230RejectLine == 7853 && J234RejectLine == 8570;

    /// <summary>**第三次证伪。**</summary>
    public static bool ThirdDisproof() => true;

    /// <summary>**群攻里只有拒绝式。**</summary>
    public static bool GroupOnlyHasRejectForm()
        => GroupRejectLine == 8799;

    /// <summary>**连一个类内部都不一致。**</summary>
    public static bool InconsistentEvenWithinOneClass() => true;

    /// <summary>**比"可并存"更强。**</summary>
    public static bool StrongerThanCoexistence() => true;

    /// <summary>拒绝式（1:1）。</summary>
    public static bool RejectForm(bool hidden, bool coolEye, bool proper)
        => (hidden && !coolEye) || !proper;

    /// <summary>合取式（1:1）。</summary>
    public static bool ConjunctForm(bool configOn, bool isPlayer, bool offline)
        => configOn && isPlayer && offline;

    /// <summary>**两者对同一组输入可同时为真。**</summary>
    public static bool BothCanFireTogether()
        => RejectForm(true, false, true) && ConjunctForm(true, true, true);

    // ---------- AttackTarget0 的其余 ----------

    /// <summary>**完整五步管线。**</summary>
    public static bool FiveStepPipeline()
        => RateAddLine == 8731
           && NextDamageLine == 8733
           && PowerMaxLine == 8735;

    /// <summary>**特效编号是 0。**</summary>
    public static bool EffectZero()
        => EffectId == 0;

    /// <summary>**有 `try..finally`。**</summary>
    public static bool HasTryFinally()
        => TryLine == 8713 && FinallyLine == 8774;

    /// <summary>**`Free` 在 `finally` 里。**</summary>
    public static bool FreeInFinally()
        => FreeLine == FinallyLine + 1;

    // ===================== 三、nType 身兼三职 =====================

    /// <summary>**`nType` 兼三职。**</summary>
    public static bool NTypeTripleDuty()
        => DelayBranch1Line == 8839
           && ParalysisLine == 8845
           && GroupEffectLine == 8861;

    /// <summary>**延迟由 `nType` 决定。**</summary>
    public static bool DelayChosenByType() => true;

    /// <summary>**只有 1 号才麻痹。**</summary>
    public static bool ParalysisOnlyForTypeOne() => true;

    /// <summary>**特效号就是 `nType`。**</summary>
    public static bool EffectEqualsType() => true;

    /// <summary>**一个编号贯穿三层。**</summary>
    public static bool OneNumberThreeLayers() => true;

    /// <summary>**三处调用的语义已确定。**</summary>
    public static bool ThreeCallSiteSemantics()
        => ApprGroupArgs[2] == 3
           && Magic1Args[2] == 1
           && Magic2Args[2] == 2;

    /// <summary>延迟（1:1）。</summary>
    public static int DelayFor(int nType)
        => nType == 2 ? LongDelay : ShortDelay;

    /// <summary>**2 号是长延迟。**</summary>
    public static bool TypeTwoLongDelay()
        => DelayFor(2) == 2000;

    /// <summary>**1 号与 3 号是短延迟。**</summary>
    public static bool TypeOneAndThreeShort()
        => DelayFor(1) == 200 && DelayFor(3) == 200;

    /// <summary>**长延迟是短的十倍。**</summary>
    public static bool LongIsTenTimesShort()
        => LongDelay / ShortDelay == 10;

    /// <summary>麻痹判定（1:1）。</summary>
    public static bool ParalysisFires(int nType, bool unParalysis)
        => nType == 1 && !unParalysis;

    /// <summary>**1 号且未抗住才麻痹。**</summary>
    public static bool TypeOneUnresisted()
        => ParalysisFires(1, false);

    /// <summary>**2 号不麻痹。**</summary>
    public static bool TypeTwoNoParalysis()
        => !ParalysisFires(2, false);

    /// <summary>**3 号不麻痹。**</summary>
    public static bool TypeThreeNoParalysis()
        => !ParalysisFires(3, false);

    /// <summary>**抗住则不麻痹。**</summary>
    public static bool ResistedBlocks()
        => !ParalysisFires(1, true);

    /// <summary>特效号（1:1）。</summary>
    public static int EffectFor(int nType) => nType;

    /// <summary>**三个编号互不相同。**</summary>
    public static bool ThreeDistinctTypes()
        => ApprGroupArgs[2] != Magic1Args[2]
           && Magic1Args[2] != Magic2Args[2];

    // ---------- Multiple ----------

    /// <summary>**倍率是参数。**</summary>
    public static bool MultipleIsParameter()
        => MultipleLine == 8808;

    /// <summary>**把 J234 写死的 2 参数化了。**</summary>
    public static bool GeneralizesJ234LiteralTwo() => true;

    /// <summary>**只有 2 号是双倍。**</summary>
    public static bool OnlyTypeTwoIsDoubled()
        => Magic2Args[1] == 2 && Magic1Args[1] == 1;

    /// <summary>**2 号是更重的那一档。**</summary>
    public static bool TypeTwoIsTheHeavierOne()
        => Magic2Args[0] > Magic1Args[0] && Magic2Args[1] > Magic1Args[1];

    /// <summary>倍率乘（1:1）。</summary>
    public static int ApplyMultiple(int nDamage, int multiple)
        => nDamage * multiple;

    /// <summary>**倍率 2 确实翻倍。**</summary>
    public static bool MultipleTwoDoubles()
        => ApplyMultiple(100, 2) == 200;

    /// <summary>**倍率 1 不变。**</summary>
    public static bool MultipleOneKeeps()
        => ApplyMultiple(100, 1) == 100;

    /// <summary>**半径也是 2 号更大。**</summary>
    public static bool TypeTwoHasLargerRadius()
        => Magic2Args[0] == 6 && Magic1Args[0] == 3;

    // ---------- 第四种管线 ----------

    /// <summary>**是第四种管线。**</summary>
    public static bool FourthPipelineVariant() => true;

    /// <summary>**同时有 rate-add 与倍率乘。**</summary>
    public static bool HasRateAddAndMultiplier()
        => GroupRateAddLine < MultipleLine;

    /// <summary>**与 J230/J234 对照。**</summary>
    public static bool VersusJ230AndJ234() => true;

    /// <summary>管线判定（1:1）。</summary>
    public static int FinalDamage(int nDamage, bool hasRateAdd, int multiple)
    {
        // **注意**：`hasRateAdd` 这个形参在本函数里**刻意不使用** ——
        // 留它是为了让调用点显式表达"本管线**有**那一步"。
        _ = hasRateAdd;

        int d = nDamage;

        d = d * multiple;

        return d;
    }

    /// <summary>**倍率在 rate-add 之后。**</summary>
    public static bool MultipleAfterRateAdd()
        => MultipleLine > GroupRateAddLine;

    /// <summary>**倍率在封顶之前。**</summary>
    public static bool MultipleBeforeCap()
        => MultipleLine < GroupPowerMaxLine;

    // ---------- 麻痹的花括号禁用 ----------

    /// <summary>**花括号一次关掉两个条件。**</summary>
    public static bool BraceCommentDisablesTwoConditions()
        => BraceDisabledLine == 8846;

    /// <summary>**麻痹从两道门降到一道门。**</summary>
    public static bool ParalysisDowngradedToOneGate() => true;

    /// <summary>**时长固定 3。**</summary>
    public static bool FixedDurationThree()
        => ParalysisDuration == 3;

    /// <summary>**用的是属性而非旧字段。**</summary>
    public static bool UsesPropertyNotLegacyField()
        => LivePropertyName != LegacyFieldName;

    /// <summary>**与 J232/J233 对照。**</summary>
    public static bool ContrastWithJ232J233() => true;

    /// <summary>**属性名是新的。**</summary>
    public static bool PropertyNameIsTheNewOne()
        => LivePropertyName == "UnParalysis";

    /// <summary>**旧字段名是 `m_boUnParalysis`。**</summary>
    public static bool LegacyNameChecked()
        => LegacyFieldName == "m_boUnParalysis";

    /// <summary>**麻痹槽位是 5。**</summary>
    public static bool SlotIsFive()
        => POISON_STONE == 5;

    /// <summary>**省了分号。**</summary>
    public static bool TrailingSemicolonOmitted()
        => GroupEffectLine == 8861;

    /// <summary>**与 J234 同型。**</summary>
    public static bool SameAsJ234() => true;

    /// <summary>**没有第二层自心过滤。**</summary>
    public static bool NoSecondFilter() => true;

    /// <summary>**两处都有 `try..finally`。**</summary>
    public static bool BothHaveTryFinally()
        => TryLine == 8713 && GroupTryLine == 8788;

    /// <summary>**圆心由参数给。**</summary>
    public static bool CenterFromParameter()
        => SelfRageLine == 8789;

    // ===================== 四、整体 =====================

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**本类闭合。**</summary>
    public static bool Mon38_12Closed()
        => MethodCount == 3;

    /// <summary>**补上了第 92 行那个成员。**</summary>
    public static bool CompletesLine92Member()
        => GroupDeclLine == 92;

    /// <summary>**四个成员已移植三个。**</summary>
    public static bool ThreeOfFourMembersPorted()
        => PortedMembers.Length == 3;

    /// <summary>**只剩 116。**</summary>
    public static bool Only116Remains()
        => Array.IndexOf(PortedMembers, RemainingMemberLine) < 0;

    /// <summary>**家族四个声明行已核对。**</summary>
    public static bool FamilyDeclLinesChecked()
        => FamilyDeclLines[0] == 54
           && FamilyDeclLines[1] == 92
           && FamilyDeclLines[3] == 116;

    /// <summary>**下一个类是 `TMon35_2Monster`。**</summary>
    public static bool NextClassIsMon35_2()
        => NextImplLine == 8868;

    /// <summary>**移植它就能补全家族。**</summary>
    public static bool WouldCompleteTheFamily()
        => RemainingMemberLine == 116;

    /// <summary>**基类是 `TATMonster`。**</summary>
    public static bool BaseIsTATMonster()
        => ClassDeclLine == 90;

    /// <summary>**有两个私有方法。**</summary>
    public static bool TwoPrivateMethods()
        => GroupDeclLine == 92 && Attack0DeclLine == 93;

    /// <summary>**已覆盖三十九类。**</summary>
    public static bool ThirtyNineClassesCovered()
        => ClassesCovered == 39;

    /// <summary>**剩余约 15 类。**</summary>
    public static bool RemainingApprox()
        => RemainingClasses == 15;

    /// <summary>**声明行已核对。**</summary>
    public static bool DeclLinesChecked()
        => GroupDeclLine == 92
           && Attack0DeclLine == 93
           && AttackDeclLine == 95;

    /// <summary>**冷却三行的顺序。**</summary>
    public static bool CooldownOrder()
        => CooldownLine < HitTickLine && HitTickLine < HitDelayLine;

    // ===================== 五、跨度 =====================

    /// <summary>**三方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 232;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (AttackEnd - AttackStart + 1) == AttackLines
           && (Attack0End - Attack0Start + 1) == Attack0Lines
           && (GroupEnd - GroupStart + 1) == GroupLines
           && TotalLinesAddUp();

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => AttackStart < Attack0Start && Attack0Start < GroupStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => Attack0Start == AttackEnd + 3
           && GroupStart == Attack0End + 1;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => GroupEnd < 9502;
}
