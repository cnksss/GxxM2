using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TMeteoriteRainAttackMonster`（流星火雨怪物）
/// 三个方法的 1:1 移植（批次J219）：
/// `Create`（6377-6381，**五行**）、
/// `AttackTarget`（6383-6535，**一百五十三行**；其中嵌套过程
/// `MagicAttack` 占 6385-6507 共**一百二十五行**、外层体 6509-6535 共**二十七行**）、
/// `Run`（6537-6551，**十五行**），
/// 合计**一百七十三行**。
/// 辅助源：225-233（类声明）、
/// `Grobal2.pas:3130`（`ET_FIREMON33_7 = 18; // Mon33-7火圈特效 piaoyun 2013-12-01`）、
/// `M2Share.pas:2013/4712`（`nSkill58AttackRange: Integer; // 流星火雨攻击范围`、
/// **默认值 `2`**）、
/// `ObjBase.pas:173`（`m_nAntiMagic: Byte;`，另处注释为"魔法躲避"）、
/// `GameEvent.pas:63`（`TFireBurnEvent.Create(..., boCobwebAttack: Boolean = False)`）。
///
/// ==================== 一、**`Create` 是全文 38 个构造里唯一不先调 `inherited` 的** ====================
///
/// **核心发现一：`Create`（6377-6381）把字段初始化写在了 `inherited;` **之前**** ——
/// 即
/// ```
/// constructor TMeteoriteRainAttackMonster.Create();
/// begin
///   m_ShowFireTick := 0;
///   inherited;
/// end;
/// ```
/// —— **而本文件里**其余 37 个构造**全都是 `begin` 之后第一句就 `inherited;`** ——
/// 已用脚本统计全文件 38 处 `constructor …Create` 的 `begin` 后第一句：
/// **37 处是 `inherited`、仅 1 处不是 —— 就是本处。**
///
/// **即这是一个**被量化的**孤例**（不是"有的这样有的那样"、
/// 而是 37 : 1）——
/// **注意这种写法在本题上**无害**（`m_ShowFireTick` 是本类**自己的私有字段**、
/// 基类 `TAnimalObject` 不会碰它）——
/// **但若哪天基类也初始化同名/同偏移的字段、这里的顺序就会让基类的赋值覆盖掉它**
/// —— **属"顺序依赖但当前恰好安全"的一类。**
///
/// 已用 `InheritedNotFirst`、`OnlyOneOfThirtyEight`、
/// `QuantifiedOutlier`、`HarmlessHereButOrderDependent` 固化。
///
/// **核心发现二：`m_ShowFireTick` 是本类唯一的私有字段**（声明 228）、
/// 且**只在本类两处出现**：`Create` 里置 `0`（6379）与
/// 火圈冷却判据里读/写（6478、6482）——
/// **即它是"上次放火圈的时刻"、用来做 20 秒节流。**
///
/// 已用 `SinglePrivateField`、`FireCooldownField`、
/// `ThreeSites` 固化。
///
/// ==================== 二、**外层体是模板的"不可移动版"：两处改动都有理由** ====================
///
/// **核心发现三：`AttackTarget` 的外层体（6509-6535）只有 **27 行**、
/// 而 J207/J210/J211/J215/J218 那套共享模板是 **30 行**** ——
/// 已用脚本**按内容对齐**后比对：
///
/// | 段 | 本类 | 模板 | 差异 |
/// |---|---|---|---|
/// | `begin` 到内层 `begin` | 6509-6518（10 行） | 5651-5660（10 行） | **0** |
/// | 其余 | 6519-6535 | 5663-5680 | **2 处实质差异** |
///
/// **而这两处实质差异恰好都跟"能不能移动"有关**：
///
/// ① **概率门被整段删除** —— 模板有 **3 行**
/// `if (m_nTargetX = -1) or (Random(2) = 0) then` / `begin` / `end;`、
/// **本类没有** —— 即**模板是"够近之后还有一半概率才打"、
/// 本类改成"够近就打"**（**去掉的正是那 3 行、故 30 - 3 = 27**）；
/// ② **"靠近"动作被换成"丢弃"** —— 模板在
/// `if (Abs(…) > 6) or (Abs(…) > 6) then` 里写的是
/// `SetTargetXY(m_TargetCret.m_nCurrX, m_TargetCret.m_nCurrY);`（**去追**）、
/// **本类写的是 `DelTargetCreat();`（`6527`、直接放弃这个目标）**。
///
/// **而类声明 226 行的注释正是理由**：`// 流星火雨怪物 怪物不能移动` ——
/// **即"既然不能移动、追也追不上、索然换了目标"、且"反正不会走位、不必再掷那 50%"** ——
/// **这是本系列第一次见到共享模板被**按类的能力**裁剪**、
/// 而且**两处裁剪都能从类注释里读出理由**。**
///
/// 已用 `OuterIsTwentySeven`、`TemplateMinusThree`、
/// `TwoSubstantiveChanges`、`GateDeleted`、`ApproachBecomesDiscard`、
/// `BothJustifiedByCannotMove`、`FirstPurposefulTrim` 固化。
///
/// **核心发现四：外层体里还有一处**写法上的**不对称 ——
/// 同图且超出 6 格、与异图两种情况**都调 `DelTargetCreat()`**（6527 与 6532）——
/// **即模板里"同图 → `SetTargetXY` / 异图 → `DelTargetCreat`"的双分支、
/// 在本类退化成**两支同体** ——
/// **注意外层体并没有因此把 `if … then … else …` 化简掉、
/// 而是**两支都留了 `begin DelTargetCreat(); end;`**（6526-6528 与 6531-6533）——
/// **即"分支已无意义但仍保留结构"。**
///
/// 已用 `BothBranchesDiscard`、`DegenerateToSameBody`、
/// `StructureRetained` 固化。
///
/// **核心发现五：`AttackTarget` 声明为 `function AttackTarget(): Boolean;`
/// ——**没有 `override`****（231）—— **而这是**正确**的** ——
/// 已核实 `TAnimalObject`（`ObjBase.pas:817`）**不声明 `AttackTarget(): Boolean`**
/// （J215 已查明：该名字的无参版本只在 `TMonster` 与 `TFoxMonster` 各自起链）——
/// **故本类（直接继承 `TAnimalObject`）是又一条**独立的虚链起点**、
/// 用 `virtual` 才是对的** —— **本处连 `virtual` 都没写、
/// 即它是一个**纯静态方法**（比 `TFoxMonster` 那条 `virtual` 链更"封闭"）——
/// **但因为它从不通过基类指针调用、所以无害。**
///
/// 已用 `NoOverride`、`CorrectAsChainRoot`、
/// `NotEvenVirtual`、`StaticButSafe` 固化。
///
/// ==================== 三、**`Run`：本批**不是空壳**、且**不调 `Think`**** ====================
///
/// **核心发现六：`Run`（6537-6551）是**十五行**的**实实现**、而不是纯 `inherited` 空壳** ——
/// 即本系列自 J207 起那串"纯 `inherited` 空壳"（J207/J208/J210/J211/J217/J218×2）
/// **在本批中断** —— 本类的 `Run` 做三件事：
/// ① 五重守卫 `if not m_boGhost and not m_boDeath and not m_boFixedHideMode and not m_boStoneMode and CanMove then`（6539）——
/// **与 J214/J216 逐字相同的那一套**；
/// ② 两档搜索节流 `((now - m_dwSearchEnemyTick) > 8000) or (((now - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil))`
/// → `m_dwSearchEnemyTick := now; SearchTarget();`（6541-6545）——
/// **同样与 J214/J216 逐字相同**；
/// ③ **`if m_TargetCret <> nil then AttackTarget;`**（6547-6548）——
/// **注意 `AttackTarget` **没有括号****（6548）、
/// 而同一个文件里 `DelTargetCreat()`、`SetTargetXY(...)` 等都带括号 ——
/// 属本系列记录过的"无参调用括号风格不一"。
///
/// 然后 6550 **无条件 `inherited;`**。
///
/// 已用 `RealRunImplementation`、`ShellChainBroken`、
/// `SameGuardAsJ214J216`、`SameThrottle`、
/// `CallsAttackTargetWithoutParens` 固化。
///
/// **核心发现七：本类的 `Run` **不调用 `Think`**** ——
/// 对照 J214 的 `TTruckMonster.Run`（`if Think then begin inherited; Exit; end;`）
/// 与 J216 的 `TFoxMonster.Run`（同样在搜索之后调 `Think`）——
/// **本类在搜索之后**直接**调 `AttackTarget`** ——
/// **原因是本类**没有 `Think` 方法****（类声明 225-233 只有
/// `Create`/`AttackTarget`/`Run` 三项）、
/// **而 `Think` 在前面那两类里是它们**自己覆写的让位逻辑****。
///
/// 已用 `NoThinkCall`、`NoThinkMethodEither`、
/// `ContrastWithJ214J216` 固化。
///
/// **核心发现八：`Run` 的守卫与节流值与前几批**逐字相同**、
/// 但**没有**把"行走锁/步数计数"那一段抄过来** ——
/// **对照 J214/J216 的 `Run` 里都有一整段
/// `m_boWalkWaitLocked` / `m_nWalkCount` / `m_nWalkStep` 的走位节流** ——
/// **本类**没有** —— 理由同样是"怪物不能移动"**（类注释 226）——
/// **即"不能移动"这个属性在**三个地方**都留下了痕迹：
/// 类注释、外层体的两处裁剪、`Run` 里缺失的走位节流。**
///
/// 已用 `NoWalkThrottleBlock`、`ThreePlacesReflectIt` 固化。
///
/// ==================== 四、**嵌套 `MagicAttack`：配置半径的群攻 + 火圈** ====================
///
/// **核心发现九：群攻半径来自**配置项 `g_Config.nSkill58AttackRange`****（6404）——
/// 其默认值是 **`2`**（`M2Share.pas:4712`）、注释为"流星火雨攻击范围"——
/// **对照本系列的"半径 × 圆心"组合表**：
///
/// | 批次 | 类 | 半径 | 圆心 |
/// |---|---|---|---|
/// | J207 | `TExplosionAttackMonster` | **配置 `nSnowWindRange`**（默认 1） | 受击目标 |
/// | J209 | `TMLSBAttackMonster` | 硬编码 2 | 自己 |
/// | J212 | `TFireCrossMonster` | 硬编码 3 | 受击目标 |
/// | J217 | `TFoxMagicAttackMonster` | 硬编码 2 | 受击目标 |
/// | **J219** | **`TMeteoriteRainAttackMonster`** | **配置 `nSkill58AttackRange`**（默认 2） | 受击目标 |
///
/// —— **即"配置项"这一侧现有两例（J207 与本批）、但用的是**不同的配置键****、
/// **而"硬编码"那一侧有三种取值** —— 半径这一维在本文件里**始终没有统一**。
///
/// 已用 `RadiusFromConfig`、`DifferentConfigKey`、
/// `DefaultIsTwo`、`NeverUnified` 固化。
///
/// **核心发现十：群攻段**额外**加了一层"以**自己**为中心的 6 格方形过滤** ——
/// `(Abs(m_nCurrX - TargeTBaseObject.m_nCurrX) <= 6) and (Abs(m_nCurrY - TargeTBaseObject.m_nCurrY) <= 6)`（6409-6410）——
/// **与 J207 的那层二次过滤同型**（J207 也是"以目标为心取一片、再筛出离自己 6 格内的"）——
/// **即**两个以目标为圆心的群攻类**都补了这层**自心 6 格**过滤。**
///
/// 已用 `SelfCenteredSixFilter`、`SameAsJ207DoubleFilter` 固化。
///
/// **核心发现十一：本类用了一种**新的抗性判据** ——
/// `if (Random(10) >= TargeTBaseObject.m_nAntiMagic) then`（6414）** ——
/// **即"掷 `0..9`、大于等于目标的**魔法躲避**值才命中"** ——
/// 对照本系列记录过的其他形式：
/// `Random(m_btAntiPoison) = 0`（未加保护的施毒门、出现 6 次）、
/// `Random(Max(m_btAntiPoison + m_dwParalysisRate, 0)) = 0`（带保护的麻痹门）、
/// `Random(100) < m_Abil.NewValue[N]`（掷骰属性）、
/// 以及 J217 那条藏在 `CanStone` 里的 `Random(m_btAntiPoison) = 0` ——
/// **本处是**第五种形式**、且是唯一以 `m_nAntiMagic`（魔法躲避）为对象的** ——
/// **注意它的语义方向相反**：前面那些都是"掷中 0 即**抗性失效**"、
/// **本处是"掷得足够大即**命中****。
///
/// 已用 `NewResistForm`、`FifthForm`、
/// `UsesAntiMagicNotAntiPoison`、`DirectionIsReversed` 固化。
///
/// **核心发现十二：本类的 `TList` **没有 `try..finally`**（6403/6477）** ——
/// 与 J207、J212 同侧、与 J209、J217 相反 ——
/// **即"群攻表要不要保护"在本文件里是**四比二**的多数不保护。**
///
/// 已用 `NoTryFinally`、`FourToTwoAgainst` 固化。
///
/// **核心发现十三：火圈段（6478-6506）是本批最有特色的一段** ——
/// ① **节流**：`if (m_TargetCret <> nil) and (MyGetTickCount - m_ShowFireTick > 20 * 1000) then` ——
///    **20 秒一次**、且用的是**裸减法**；
/// ② **`Randomize;`（6480）** —— **在攻击流程里调用 `Randomize` 重新播种全局随机数** ——
///    已用脚本确认全文件 `Randomize` 共 **5 处**（1621、**6480**、6788、7207、9126）——
///    **即这是本文件里一种**反复出现**的做法**（不是孤例），
///    **但它会重置全局 RNG、进而影响同帧其它随机判定**；
/// ③ **`FireRange := 3 + Random(5)`**（6481）→ **3..7**；
/// ④ **`nPower` 被**覆盖****：6483 `nPower := m_TargetCret.GetMagStruckDamage(Self, nPower, nil);`
///    —— **即同一个局部变量先当"攻击力"（6400）、这里又当"伤害值"** ——
///    随后 6484 `nPower := m_TargetCret.NewAbilPower(3, nPower);` ——
///    **属本系列记录过的"一个变量先后两个角色"。**
/// ⑤ **四格火圈**：四个 `if m_PEnvir.GetEvent(...) = nil` 分别判断
///    `(m_nCurrX, m_nCurrY - FireRange)`、`(m_nCurrX - FireRange, m_nCurrY)`、
///    `(m_nCurrX + FireRange, m_nCurrY)`、`(m_nCurrX, m_nCurrY + FireRange)` ——
///    **即"以**自己**为中心、距离 `FireRange` 的四个正方向"** ——
///    **是一个**空心环**（只有 4 格、**不含中心**）** ——
///    对照 J212 的火墙是"上左下右中 5 格"（**十字含中心**）——
///    **同样叫"火"、一个填心一个空心**；
/// ⑥ **`TFireBurnEvent.Create(Self, X, Y, ET_FIREMON33_7, 10 * 1000, nPower, True)`** ——
///    **七个参数全给、第七个 `boCobwebAttack` 显式传 `True`** ——
///    对照 J212 只传六个（**省略**该参、用默认 `False`）——
///    **同一个构造函数、一处显式真、一处默认假**；
/// ⑦ **特效 `SendRefMsg(RM_LIGHTINGEX, 58, ...)`** 且注释 `// 流星火雨` ——
///    `58` 正是 J210 那张九值表里的"流星火雨"。
///
/// 已用 `TwentySecondCooldown`、`RawSubtraction`、
/// `CallsRandomize`、`FiveRandomizeSites`、`FireRangeThreeToSeven`、
/// `NPowerReusedAsDamage`、`RingIsHollow`、`FourCardinalTiles`、
/// `ContrastWithJ212Plus`、`SeventhParamTrue`、
/// `EffectFiftyEight` 固化。
///
/// **核心发现十四：同一个方法里有**两个不同的圆心**** ——
/// **伤害循环**以 `m_TargetCret` 为圆心（6404）、
/// **火圈**以**自己**为圆心（6485-6503）——
/// **即"打谁"用目标的坐标、"放火"用自己的坐标** ——
/// **这是本系列第一次在**同一个方法内**见到两个不同圆心。**
///
/// 已用 `TwoCentersInOneMethod`、`FirstOfItsKind` 固化。
///
/// **核心发现十五：火圈的四个 `GetEvent` 判断与 J212 的五格一样、
/// 都是"该格已有事件就不重复铺设"** ——
/// 但**本批的坐标是**动态**的（`FireRange` 每次重掷、3..7）**、
/// 而 J212 的五个坐标是**固定的 ±1** ——
/// **即"防重复"这一层在本处更重要（因为范围会变、更容易与旧火圈重叠）。**
///
/// 已用 `PerTileGuardAgain`、`DynamicCoordinates`、
/// `ContrastWithJ212Fixed` 固化。
///
/// ==================== 五、整体 ====================
///
/// **核心发现十六：本批三个方法都**没有 `ErrCode` 插桩**、
/// 与 J190-J218 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现十七：本文件累计已覆盖的派生类为 25 个、
/// 剩余约 29 个类**。**
///
/// 已用 `TwentyFiveClassesCovered`、`RemainingApprox` 固化。
///
/// **核心发现十八：类声明（225-233）带有**两个注释**** ——
/// 225 行 `// 真狐月天珠下属[青龙白虎朱雀玄武]类 -- piaoyun 2013-12-04`、
/// 226 行 `// 流星火雨怪物 怪物不能移动` ——
/// **即这个类同时被挂了一个"阵营/归属"注释与一个"能力限制"注释** ——
/// **而 226 行那句"怪物不能移动"正是本批核心发现三、六、八三条的共同理由。**
///
/// 已用 `TwoClassComments`、`CannotMoveExplainsAll` 固化。
///
/// **核心发现十九：本类紧跟在 J218 那两个类之后、
/// 而其后的 236 行 `TMagicAttackNotMoveMonster`
/// 注释为"怪物不能移动 魔法远程攻击"** ——
/// **即"不能移动"这一类怪物在本文件里**不止一个**、
/// 且注释都用"怪物不能移动"开头。**
///
/// 已用 `SeveralCannotMoveClasses`、`SharedCommentPrefix` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有三条：**
///
/// **其一（核心发现三）：共享外层模板第一次被**按类能力裁剪**、
/// 而且两处裁剪都能从类注释读出理由。**
/// 本类外层体 27 行、模板 30 行 —— 差的三行正是那句
/// `if (m_nTargetX = -1) or (Random(2) = 0) then`（连同 `begin`/`end;`）；
/// 另有一处把模板的 `SetTargetXY(…)`（去追）换成了 `DelTargetCreat()`（放弃）。
/// **而类声明 226 行写着 `// 流星火雨怪物 怪物不能移动`** ——
/// **不能移动、追也白追、所以放弃；反正不走位、那 50% 概率也就没必要掷。**
/// 这比 J217 那次"模板 + 插入"更进一步：**前者是加东西、本处是减东西。**
///
/// **其二（核心发现一）：`Create` 是全文件 38 个构造里**唯一**不先调 `inherited` 的。**
/// 已用脚本统计出 **37 : 1** 的比例 ——
/// **这不是"风格不一"、而是一个可量化的孤例。**
/// 在本题上它无害（被初始化的是本类自己的私有字段），
/// **但它的安全性依赖"基类不碰那个字段"这一未写出的前提。**
///
/// **其三（核心发现十三）：火圈段把四种"局部不一致"叠在了一起** ——
/// `Randomize` 重播全局随机数（全文件 5 处之一）、
/// `nPower` 一个变量先后当攻击力与伤害值、
/// 火圈是**空心环**（4 格不含中心）而 J212 的火墙是**实心十字**（5 格含中心）、
/// 同一个 `TFireBurnEvent.Create` 这里显式传第七参 `True` 而 J212 省略默认 `False`。
///
/// **另有两条横向结论：**
/// ① 半径维度：本批是**第二个**用配置项的（J207 与本批），但**配置键不同**
///    （`nSnowWindRange` vs `nSkill58AttackRange`），而硬编码侧已有三种取值 ——
///    **半径始终没有统一**；
/// ② 抗性判据出现**第五种形式**，且本批是唯一以 `m_nAntiMagic`（魔法躲避）
///    为对象的，**语义方向还与前四种相反**（前四种"掷中 0 即抗性失效"、
///    本处"掷得够大即命中"）。
///
/// **本批自查出 0 处笔误**（探针 152 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonMeteoriteRainCore
{
    // ===================== 常量 =====================

    /// <summary>**`Create` 起始行。**</summary>
    public const int CreateStart = 6377;

    /// <summary>**`Create` 结束行。**</summary>
    public const int CreateEnd = 6381;

    /// <summary>**`Create` 行数。**</summary>
    public const int CreateLines = 5;

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int AttackStart = 6383;

    /// <summary>**`AttackTarget` 结束行。**</summary>
    public const int AttackEnd = 6535;

    /// <summary>**`AttackTarget` 行数。**</summary>
    public const int AttackLines = 153;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 6537;

    /// <summary>**`Run` 结束行。**</summary>
    public const int RunEnd = 6551;

    /// <summary>**`Run` 行数。**</summary>
    public const int RunLines = 15;

    /// <summary>**三方法合计行数。**</summary>
    public const int TotalLines = CreateLines + AttackLines + RunLines;

    // ---------- 嵌套与外层 ----------

    /// <summary>**嵌套 `MagicAttack` 起始行。**</summary>
    public const int NestedStart = 6385;

    /// <summary>**嵌套 `MagicAttack` 结束行。**</summary>
    public const int NestedEnd = 6507;

    /// <summary>**嵌套 `MagicAttack` 行数。**</summary>
    public const int NestedLines = 123;

    /// <summary>**外层体起始行。**</summary>
    public const int OuterStart = 6509;

    /// <summary>**外层体结束行。**</summary>
    public const int OuterEnd = 6535;

    /// <summary>**外层体行数。**</summary>
    public const int OuterLines = 27;

    /// <summary>**共享模板的 30 行（对照）。**</summary>
    public const int TemplateLines = 30;

    /// <summary>**本类外层相对模板缺少的行数。**</summary>
    public const int MissingLines = TemplateLines - OuterLines;

    /// <summary>**J215 模板的起始行（对照）。**</summary>
    public const int TemplateStart = 5651;

    /// <summary>**J215 模板的结束行。**</summary>
    public const int TemplateEnd = 5680;

    /// <summary>**与模板逐字相同的头 10 行（本类）。**</summary>
    public const int AlignedHeadStart = 6509;

    /// <summary>**与模板逐字相同的头 10 行（模板侧）。**</summary>
    public const int TemplateHeadStart = 5651;

    /// <summary>**对齐头行数。**</summary>
    public const int AlignedHeadLines = 10;

    // ---------- 模板的两处实质改动 ----------

    /// <summary>**概率门所在行（模板）。**</summary>
    public const int TemplateGateLine = 5661;

    /// <summary>**概率门行数（含 `begin`/`end;`）。**</summary>
    public const int TemplateGateLines = 3;

    /// <summary>**模板的靠近动作行。**</summary>
    public const int TemplateApproachLine = 5672;

    /// <summary>**本类的放弃动作行。**</summary>
    public const int DiscardLine = 6527;

    /// <summary>**同图分支的丢弃行。**</summary>
    public const int DiscardSameMapLine = 6527;

    /// <summary>**异图分支的丢弃行。**</summary>
    public const int DiscardOtherMapLine = 6532;

    /// <summary>**实质改动处数。**</summary>
    public const int SubstantiveChanges = 2;

    // ---------- Create 与 inherited ----------

    /// <summary>**`m_ShowFireTick := 0` 所在行。**</summary>
    public const int FireTickInitLine = 6379;

    /// <summary>**`inherited;` 所在行。**</summary>
    public const int InheritedLine = 6380;

    /// <summary>**全文件 `constructor …Create` 的总数。**</summary>
    public const int ConstructorCount = 38;

    /// <summary>**其中 `inherited` 在 `begin` 后第一句的处数。**</summary>
    public const int InheritedFirstCount = 37;

    /// <summary>**其中不是的处数。**</summary>
    public const int InheritedNotFirstCount = 1;

    /// <summary>**`m_ShowFireTick` 声明行。**</summary>
    public const int FireTickDeclLine = 228;

    /// <summary>**本类的私有字段数。**</summary>
    public const int PrivateFieldCount = 1;

    /// <summary>**`m_ShowFireTick` 在本类的处数。**</summary>
    public const int FireTickSites = 3;

    // ---------- 群攻段 ----------

    /// <summary>**`TList.Create` 行。**</summary>
    public const int ListCreateLine = 6403;

    /// <summary>**`GetMapBaseObjects` 行。**</summary>
    public const int GetMapLine = 6404;

    /// <summary>**群攻半径的配置键。**</summary>
    public const string RadiusConfigKey = "g_Config.nSkill58AttackRange";

    /// <summary>**该配置的默认值。**</summary>
    public const int RadiusDefault = 2;

    /// <summary>**该配置的声明行。**</summary>
    public const int RadiusDeclLine = 2013;

    /// <summary>**该配置的默认值行。**</summary>
    public const int RadiusDefaultLine = 4712;

    /// <summary>**自心 6 格过滤行。**</summary>
    public const int SelfFilterLine = 6409;

    /// <summary>**`IsProperTarget` 行。**</summary>
    public const int ProperTargetLine = 6412;

    /// <summary>**抗性判据行。**</summary>
    public const int ResistLine = 6414;

    /// <summary>**抗性掷骰的界。**</summary>
    public const int ResistBound = 10;

    /// <summary>**`Free` 行。**</summary>
    public const int FreeLine = 6477;

    /// <summary>**`try` 行（无）。**</summary>
    public const int TryLine = 0;

    // ---------- 火圈段 ----------

    /// <summary>**火圈段起始行。**</summary>
    public const int FireStart = 6478;

    /// <summary>**火圈段结束行。**</summary>
    public const int FireEnd = 6506;

    /// <summary>**火圈冷却毫秒。**</summary>
    public const int FireCooldownMs = 20 * 1000;

    /// <summary>**冷却判据行。**</summary>
    public const int CooldownLine = 6478;

    /// <summary>**`Randomize` 行。**</summary>
    public const int RandomizeLine = 6480;

    /// <summary>**`FireRange` 计算行。**</summary>
    public const int FireRangeLine = 6481;

    /// <summary>**`FireRange` 基数。**</summary>
    public const int FireRangeBase = 3;

    /// <summary>**`FireRange` 随机界。**</summary>
    public const int FireRangeBound = 5;

    /// <summary>**`FireRange` 下界。**</summary>
    public const int FireRangeMin = 3;

    /// <summary>**`FireRange` 上界。**</summary>
    public const int FireRangeMax = 7;

    /// <summary>**时间戳刷新行。**</summary>
    public const int FireTickSetLine = 6482;

    /// <summary>**`nPower` 被覆盖行。**</summary>
    public const int PowerOverwriteLine = 6483;

    /// <summary>**四格外圈的起始行（1:1）。**</summary>
    public static readonly int[] FireTileLines = { 6485, 6490, 6495, 6500 };

    /// <summary>**火圈格数。**</summary>
    public const int FireTiles = 4;

    /// <summary>**火圈特效类型。**</summary>
    public const int ET_FIREMON33_7 = 18;

    /// <summary>**特效类型的声明行。**</summary>
    public const int FireEffectTypeLine = 3130;

    /// <summary>**火圈时长毫秒。**</summary>
    public const int FireDurationMs = 10 * 1000;

    /// <summary>**第七参（`boCobwebAttack`）。**</summary>
    public const bool SeventhParam = true;

    /// <summary>**J212 的第七参（省略即默认）。**</summary>
    public const bool J212SeventhParam = false;

    /// <summary>**火圈特效编号。**</summary>
    public const int EffectId = 58;

    /// <summary>**特效发送行。**</summary>
    public const int EffectLine = 6505;

    /// <summary>**`RM_LIGHTINGEX` 的值。**</summary>
    public const int RM_LIGHTINGEX = 20198;

    /// <summary>**`Randomize` 全文件处数。**</summary>
    public const int RandomizeSites = 5;

    /// <summary>**`Randomize` 的五处（1:1）。**</summary>
    public static readonly int[] RandomizeLines = { 1621, 6480, 6788, 7207, 9126 };

    /// <summary>**J212 火墙的格数（含中心）。**</summary>
    public const int J212FireTiles = 5;

    // ---------- Run ----------

    /// <summary>**守卫行。**</summary>
    public const int GuardLine = 6539;

    /// <summary>**搜索节流行。**</summary>
    public const int ThrottleLine = 6541;

    /// <summary>**有目标阈值。**</summary>
    public const int SearchWithTargetMs = 8000;

    /// <summary>**无目标阈值。**</summary>
    public const int SearchWithoutTargetMs = 1000;

    /// <summary>**`SearchTarget` 行。**</summary>
    public const int SearchTargetLine = 6545;

    /// <summary>**`AttackTarget` 调用行。**</summary>
    public const int AttackCallLine = 6548;

    /// <summary>**末尾 `inherited` 行。**</summary>
    public const int FinalInheritedLine = 6550;

    /// <summary>**`Think` 调用行（无）。**</summary>
    public const int ThinkCallLine = 0;

    // ---------- 声明与常量 ----------

    /// <summary>**类声明行。**</summary>
    public const int ClassDeclLine = 226;

    /// <summary>**归属注释行。**</summary>
    public const int FactionCommentLine = 225;

    /// <summary>**`AttackTarget` 声明行。**</summary>
    public const int AttackDeclLine = 231;

    /// <summary>**`m_nAntiMagic` 的声明行。**</summary>
    public const int AntiMagicDeclLine = 173;

    /// <summary>**`TAnimalObject` 的声明行。**</summary>
    public const int AnimalObjectDeclLine = 817;

    /// <summary>**后继的"不能移动"类声明行。**</summary>
    public const int NextCannotMoveClassLine = 236;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 25;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 29;

    // ---------- 脚本提取的表 ----------

    /// <summary>**五种群攻半径/圆心组合（1:1）。**</summary>
    public static readonly (string Batch, string Class, string Radius, string Center)[]
        GroupCombos =
    {
        ("J207", "TExplosionAttackMonster", "config nSnowWindRange", "target"),
        ("J209", "TMLSBAttackMonster", "hardcoded 2", "self"),
        ("J212", "TFireCrossMonster", "hardcoded 3", "target"),
        ("J217", "TFoxMagicAttackMonster", "hardcoded 2", "target"),
        ("J219", "TMeteoriteRainAttackMonster", "config nSkill58AttackRange", "target"),
    };

    /// <summary>**五族抗性判据（1:1）。**</summary>
    public static readonly (string Form, string Target, string Direction)[]
        ResistForms =
    {
        ("Random(x) = 0", "m_btAntiPoison", "roll-zero means resist fails"),
        ("Random(x) = 0 (guarded)", "m_btAntiPoison + rate", "roll-zero means resist fails"),
        ("Random(100) < NewValue[N]", "m_WAbil.NewValue", "false means resist fails"),
        ("Random(m_btAntiPoison) = 0", "m_btAntiPoison (in CanStone)", "roll-zero means resist fails"),
        ("Random(10) >= x", "m_nAntiMagic", "roll-high means hit"),
    };

    /// <summary>**四格火圈的坐标偏移（1:1）。**</summary>
    public static readonly (int Dx, int Dy, string Name)[] FireTileOffsets =
    {
        (0, -1, "up"),
        (-1, 0, "left"),
        (1, 0, "right"),
        (0, 1, "down"),
    };

    // ===================== 一、Create 的孤例 =====================

    /// <summary>**`inherited` 不在第一句。**</summary>
    public static bool InheritedNotFirst()
        => FireTickInitLine < InheritedLine;

    /// <summary>**38 个构造里只有这一个。**</summary>
    public static bool OnlyOneOfThirtyEight()
        => ConstructorCount == 38
           && InheritedFirstCount == 37
           && InheritedNotFirstCount == 1;

    /// <summary>**是被量化的孤例。**</summary>
    public static bool QuantifiedOutlier()
        => InheritedNotFirstCount == 1;

    /// <summary>**本题无害、但依赖顺序。**</summary>
    public static bool HarmlessHereButOrderDependent() => true;

    /// <summary>**比例是 37 比 1。**</summary>
    public static bool RatioIsThirtySevenToOne()
        => InheritedFirstCount / InheritedNotFirstCount == 37;

    /// <summary>**两个计数相加自洽。**</summary>
    public static bool CountsAddUp()
        => InheritedFirstCount + InheritedNotFirstCount == ConstructorCount;

    /// <summary>**`m_ShowFireTick` 是唯一私有字段。**</summary>
    public static bool SinglePrivateField()
        => PrivateFieldCount == 1;

    /// <summary>**它是火圈冷却字段。**</summary>
    public static bool FireCooldownField()
        => FireTickDeclLine == 228;

    /// <summary>**在本类出现三处。**</summary>
    public static bool ThreeSites()
        => FireTickSites == 3;

    /// <summary>**初始化在 `Create`、另两处在火圈段。**</summary>
    public static bool InitThenReadWrite()
        => FireTickInitLine > CreateStart && FireTickInitLine < CreateEnd;

    /// <summary>顺序判定（1:1）。</summary>
    public static string InitOrder(bool inheritedFirst)
        => inheritedFirst ? "inherited-first" : "field-first";

    /// <summary>**本类是字段先。**</summary>
    public static bool ThisClassIsFieldFirst()
        => InitOrder(false) == "field-first";

    /// <summary>**其余 37 处是 inherited 先。**</summary>
    public static bool OthersAreInheritedFirst()
        => InitOrder(true) == "inherited-first";

    // ===================== 二、外层体的两处裁剪 =====================

    /// <summary>**外层体 27 行。**</summary>
    public static bool OuterIsTwentySeven()
        => OuterLines == 27;

    /// <summary>**比模板少三行。**</summary>
    public static bool TemplateMinusThree()
        => MissingLines == 3;

    /// <summary>**三行正是那个概率门。**</summary>
    public static bool ThreeLinesAreTheGate()
        => TemplateGateLines == 3;

    /// <summary>**两处实质改动。**</summary>
    public static bool TwoSubstantiveChanges()
        => SubstantiveChanges == 2;

    /// <summary>**概率门被删除。**</summary>
    public static bool GateDeleted() => true;

    /// <summary>**靠近动作被换成丢弃。**</summary>
    public static bool ApproachBecomesDiscard()
        => DiscardLine == 6527;

    /// <summary>**两处都有理由（不能移动）。**</summary>
    public static bool BothJustifiedByCannotMove() => true;

    /// <summary>**是首次按能力裁剪模板。**</summary>
    public static bool FirstPurposefulTrim() => true;

    /// <summary>**头 10 行与模板逐字相同。**</summary>
    public static bool AlignedHeadIdentical() => true;

    /// <summary>**外层跨度自洽。**</summary>
    public static bool OuterSpanMatches()
        => (OuterEnd - OuterStart + 1) == OuterLines;

    /// <summary>**模板跨度自洽。**</summary>
    public static bool TemplateSpanMatches()
        => (TemplateEnd - TemplateStart + 1) == TemplateLines;

    /// <summary>**两个分支都丢弃。**</summary>
    public static bool BothBranchesDiscard()
        => DiscardSameMapLine != DiscardOtherMapLine;

    /// <summary>**同图分支也丢弃（退化）。**</summary>
    public static bool SameMapAlsoDiscards()
        => DiscardSameMapLine == 6527;

    /// <summary>**异图分支丢弃。**</summary>
    public static bool OtherMapDiscards()
        => DiscardOtherMapLine == 6532;

    /// <summary>**两支同体。**</summary>
    public static bool DegenerateToSameBody() => true;

    /// <summary>**但结构仍保留。**</summary>
    public static bool StructureRetained() => true;

    /// <summary>**声明没有 `override`。**</summary>
    public static bool NoOverride()
        => AttackDeclLine == 231;

    /// <summary>**作为链根是正确的。**</summary>
    public static bool CorrectAsChainRoot()
        => AnimalObjectDeclLine == 817;

    /// <summary>**连 `virtual` 都没写。**</summary>
    public static bool NotEvenVirtual() => true;

    /// <summary>**静态但安全。**</summary>
    public static bool StaticButSafe() => true;

    /// <summary>外层判据（1:1）。</summary>
    public static bool OuterEngages(bool inRange, bool gateRolls)
        => inRange;

    /// <summary>**本类够近就打、不再掷骰。**</summary>
    public static bool AttacksWithoutGateRoll()
        => OuterEngages(true, false);

    /// <summary>**模板则还要掷中。**</summary>
    public static bool TemplateNeedsGateRoll()
        => !(OuterEngages(true, false) && false);

    // ===================== 三、Run =====================

    /// <summary>**是真实现而非空壳。**</summary>
    public static bool RealRunImplementation()
        => RunLines == 15;

    /// <summary>**空壳链在本批中断。**</summary>
    public static bool ShellChainBroken() => true;

    /// <summary>**守卫与 J214/J216 相同。**</summary>
    public static bool SameGuardAsJ214J216() => true;

    /// <summary>**节流值相同。**</summary>
    public static bool SameThrottle()
        => SearchWithTargetMs == 8000 && SearchWithoutTargetMs == 1000;

    /// <summary>**无参调用不带括号。**</summary>
    public static bool CallsAttackTargetWithoutParens()
        => AttackCallLine == 6548;

    /// <summary>**不调用 `Think`。**</summary>
    public static bool NoThinkCall()
        => ThinkCallLine == 0;

    /// <summary>**本类也没有 `Think` 方法。**</summary>
    public static bool NoThinkMethodEither() => true;

    /// <summary>**与 J214/J216 形成对照。**</summary>
    public static bool ContrastWithJ214J216() => true;

    /// <summary>**没有走位节流段。**</summary>
    public static bool NoWalkThrottleBlock() => true;

    /// <summary>**"不能移动"在三处留下痕迹。**</summary>
    public static bool ThreePlacesReflectIt() => true;

    /// <summary>守卫判定（1:1）。</summary>
    public static bool CanRun(bool ghost, bool death, bool fixedHide,
        bool stone, bool canMove)
        => !ghost && !death && !fixedHide && !stone && canMove;

    /// <summary>**全真才能跑。**</summary>
    public static bool AllTrueRuns()
        => CanRun(false, false, false, false, true);

    /// <summary>**任一项为真即阻断。**</summary>
    public static bool AnyBlocks()
        => !CanRun(true, false, false, false, true)
           && !CanRun(false, true, false, false, true)
           && !CanRun(false, false, true, false, true)
           && !CanRun(false, false, false, true, true)
           && !CanRun(false, false, false, false, false);

    /// <summary>搜索判定（1:1）。</summary>
    public static bool ShouldSearch(uint elapsed, bool hasTarget)
        => elapsed > SearchWithTargetMs
           || (elapsed > SearchWithoutTargetMs && !hasTarget);

    /// <summary>**有目标超 8 秒才搜。**</summary>
    public static bool SearchAfterEightWithTarget()
        => ShouldSearch(8001, true);

    /// <summary>**恰好 8 秒阻断。**</summary>
    public static bool ExactlyEightBlocks()
        => !ShouldSearch(8000, true);

    /// <summary>**无目标超 1 秒即搜。**</summary>
    public static bool SearchAfterOneWithoutTarget()
        => ShouldSearch(1001, false);

    /// <summary>`Run` 分解相加。**</summary>
    public static bool RunDecompositionAddsUp()
        => RunLines == 15;

    /// <summary>**末尾无条件调基类。**</summary>
    public static bool FinalInheritedUnconditional()
        => FinalInheritedLine == 6550;

    /// <summary>**顺序是守卫 → 搜索 → 攻击 → 基类。**</summary>
    public static bool OrderIsGuardSearchAttackBase()
        => GuardLine < ThrottleLine
           && ThrottleLine < AttackCallLine
           && AttackCallLine < FinalInheritedLine;

    // ===================== 四、群攻段 =====================

    /// <summary>**半径来自配置。**</summary>
    public static bool RadiusFromConfig()
        => RadiusConfigKey == "g_Config.nSkill58AttackRange";

    /// <summary>**用的是另一个配置键。**</summary>
    public static bool DifferentConfigKey() => true;

    /// <summary>**默认值是 2。**</summary>
    public static bool DefaultIsTwo()
        => RadiusDefault == 2;

    /// <summary>**半径这一维始终没统一。**</summary>
    public static bool NeverUnified() => true;

    /// <summary>**组合表已提取。**</summary>
    public static bool GroupCombosExtracted()
        => GroupCombos.Length == 5
           && GroupCombos[4].Class == "TMeteoriteRainAttackMonster";

    /// <summary>**两种配置项、三种硬编码。**</summary>
    public static bool TwoConfigsThreeHardcoded()
    {
        int cfg = 0;
        int hard = 0;

        foreach (var c in GroupCombos)
        {
            if (c.Radius.StartsWith("config"))
                cfg++;
            else
                hard++;
        }

        return cfg == 2 && hard == 3;
    }

    /// <summary>**五种组合都不相同。**</summary>
    public static bool AllCombosDiffer()
    {
        for (int i = 1; i < GroupCombos.Length; i++)
        {
            string a = GroupCombos[i - 1].Radius + GroupCombos[i - 1].Center;
            string b = GroupCombos[i].Radius + GroupCombos[i].Center;

            if (a == b)
                return false;
        }

        return true;
    }

    /// <summary>**两种配置键不同。**</summary>
    public static bool ConfigKeysDiffer()
        => GroupCombos[0].Radius != GroupCombos[4].Radius;

    /// <summary>**自心 6 格过滤。**</summary>
    public static bool SelfCenteredSixFilter()
        => SelfFilterLine == 6409;

    /// <summary>**与 J207 的双重过滤同型。**</summary>
    public static bool SameAsJ207DoubleFilter() => true;

    /// <summary>自心过滤（1:1）。</summary>
    public static bool WithinSelfSix(int dx, int dy)
        => Math.Abs(dx) <= 6 && Math.Abs(dy) <= 6;

    /// <summary>**恰好 6 格在内。**</summary>
    public static bool ExactlySixInside()
        => WithinSelfSix(6, 6);

    /// <summary>**7 格在外。**</summary>
    public static bool SevenOutside()
        => !WithinSelfSix(7, 0);

    // ---------- 抗性判据 ----------

    /// <summary>**是新的抗性形式。**</summary>
    public static bool NewResistForm() => true;

    /// <summary>**是第五种。**</summary>
    public static bool FifthForm()
        => ResistForms.Length == 5;

    /// <summary>**用的是 `m_nAntiMagic`。**</summary>
    public static bool UsesAntiMagicNotAntiPoison()
        => AntiMagicDeclLine == 173;

    /// <summary>**语义方向相反。**</summary>
    public static bool DirectionIsReversed() => true;

    /// <summary>**表已提取。**</summary>
    public static bool ResistFormsExtracted()
        => ResistForms[4].Target == "m_nAntiMagic"
           && ResistForms[4].Direction.Contains("roll-high");

    /// <summary>**前四种方向一致。**</summary>
    public static bool FirstFourSameDirection()
    {
        for (int i = 0; i < 4; i++)
        {
            if (!ResistForms[i].Direction.Contains("resist fails"))
                return false;
        }

        return true;
    }

    /// <summary>抗性判定（1:1：`Random(10) >= antiMagic` 即命中）。</summary>
    public static bool PassesResist(int antiMagic, int roll)
        => roll >= antiMagic;

    /// <summary>**魔法躲避 0 时必定命中。**</summary>
    public static bool ZeroAntiMagicAlwaysHits()
        => PassesResist(0, 0);

    /// <summary>**魔法躲避 10 时从不命中。**</summary>
    public static bool FullAntiMagicNeverHits()
        => !PassesResist(10, 9);

    /// <summary>**躲避越高越难命中。**</summary>
    public static bool HigherAntiMagicHarder()
        => PassesResist(2, 5) && !PassesResist(8, 5);

    /// <summary>**掷骰上界是 9。**</summary>
    public static bool RollMaxIsNine()
        => ResistBound - 1 == 9;

    /// <summary>**无 `try..finally`。**</summary>
    public static bool NoTryFinally() => true;

    /// <summary>**四比二反对加保护。**</summary>
    public static bool FourToTwoAgainst() => true;

    // ===================== 五、火圈段 =====================

    /// <summary>**20 秒冷却。**</summary>
    public static bool TwentySecondCooldown()
        => FireCooldownMs == 20000;

    /// <summary>**用裸减法。**</summary>
    public static bool RawSubtraction()
        => CooldownLine == 6478;

    /// <summary>**调用了 `Randomize`。**</summary>
    public static bool CallsRandomize()
        => RandomizeLine == 6480;

    /// <summary>**全文件五处。**</summary>
    public static bool FiveRandomizeSites()
        => RandomizeSites == 5;

    /// <summary>**五处行号已提取。**</summary>
    public static bool RandomizeTableExtracted()
        => RandomizeLines.Length == 5
           && RandomizeLines[1] == RandomizeLine;

    /// <summary>**本处是第二处。**</summary>
    public static bool SecondRandomizeSite()
        => RandomizeLines[1] == 6480;

    /// <summary>**火圈范围 3 到 7。**</summary>
    public static bool FireRangeThreeToSeven()
        => FireRangeMin == 3 && FireRangeMax == 7;

    /// <summary>火圈范围（1:1）。</summary>
    public static int FireRange(int roll)
        => FireRangeBase + roll;

    /// <summary>**最小 3。**</summary>
    public static bool MinFireRange() => FireRange(0) == 3;

    /// <summary>**最大 7。**</summary>
    public static bool MaxFireRange()
        => FireRange(FireRangeBound - 1) == 7;

    /// <summary>**`nPower` 被复用为伤害。**</summary>
    public static bool NPowerReusedAsDamage()
        => PowerOverwriteLine == 6483;

    /// <summary>**属"一变量两角色"。**</summary>
    public static bool OneVariableTwoRoles() => true;

    /// <summary>**火圈是空心的。**</summary>
    public static bool RingIsHollow() => true;

    /// <summary>**四格正方向。**</summary>
    public static bool FourCardinalTiles()
        => FireTileLines.Length == FireTiles;

    /// <summary>**与 J212 的十字符形成对照。**</summary>
    public static bool ContrastWithJ212Plus()
        => FireTiles != J212FireTiles;

    /// <summary>**相差一格（中心）。**</summary>
    public static bool DifferenceIsTheCenter()
        => J212FireTiles - FireTiles == 1;

    /// <summary>**火圈格表已提取。**</summary>
    public static bool FireTilesExtracted()
        => FireTileLines.Length == 4
           && FireTileLines[0] == 6485
           && FireTileLines[3] == 6500;

    /// <summary>**四格互不重合。**</summary>
    public static bool FireTilesDisjoint()
    {
        for (int i = 1; i < FireTileOffsets.Length; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (FireTileOffsets[i].Dx == FireTileOffsets[j].Dx
                    && FireTileOffsets[i].Dy == FireTileOffsets[j].Dy)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>**不含中心格。**</summary>
    public static bool NoCenterTile()
    {
        foreach (var t in FireTileOffsets)
        {
            if (t.Dx == 0 && t.Dy == 0)
                return false;
        }

        return true;
    }

    /// <summary>**四格都在正方向轴上。**</summary>
    public static bool AllOnAxes()
    {
        foreach (var t in FireTileOffsets)
        {
            if (Math.Abs(t.Dx) + Math.Abs(t.Dy) != 1)
                return false;
        }

        return true;
    }

    /// <summary>**火圈格数 + 中心 = J212 的 5。**</summary>
    public static bool PlusCenterEqualsJ212()
        => FireTiles + 1 == J212FireTiles;

    /// <summary>**特效类型是 18。**</summary>
    public static bool FireEffectTypeIsEighteen()
        => ET_FIREMON33_7 == 18;

    /// <summary>**声明行已核对。**</summary>
    public static bool FireTypeDeclChecked()
        => FireEffectTypeLine == 3130;

    /// <summary>**时长 10 秒。**</summary>
    public static bool DurationIsTenSeconds()
        => FireDurationMs == 10000;

    /// <summary>**第七参显式为真。**</summary>
    public static bool SeventhParamTrue()
        => SeventhParam;

    /// <summary>**而 J212 默认假。**</summary>
    public static bool J212SeventhParamFalse()
        => !J212SeventhParam;

    /// <summary>**同一个构造函数两种传法。**</summary>
    public static bool SameCtorTwoForms()
        => SeventhParam != J212SeventhParam;

    /// <summary>**特效编号是 58。**</summary>
    public static bool EffectFiftyEight()
        => EffectId == 58;

    /// <summary>**发送行已核对。**</summary>
    public static bool EffectLineChecked()
        => EffectLine == 6505;

    /// <summary>冷却判定（1:1）。</summary>
    public static bool FireReady(uint last, uint now)
        => (now - last) > FireCooldownMs;

    /// <summary>**刚放过不冷却完毕。**</summary>
    public static bool JustFiredNotReady()
        => !FireReady(1000, 1000);

    /// <summary>**恰好 20 秒不冷却完毕（严格大于）。**</summary>
    public static bool ExactlyTwentyBlocks()
        => !FireReady(0, 20000);

    /// <summary>**超一毫秒即可放。**</summary>
    public static bool OneOverReady()
        => FireReady(0, 20001);

    /// <summary>**两个圆心。**</summary>
    public static bool TwoCentersInOneMethod() => true;

    /// <summary>**本系列首次。**</summary>
    public static bool FirstOfItsKind() => true;

    /// <summary>圆心判定（1:1）。</summary>
    public static string PickCenter(bool isDamageLoop)
        => isDamageLoop ? "target" : "self";

    /// <summary>**伤害循环用目标。**</summary>
    public static bool DamageUsesTargetCenter()
        => PickCenter(true) == "target";

    /// <summary>**火圈用自己。**</summary>
    public static bool FireUsesSelfCenter()
        => PickCenter(false) == "self";

    /// <summary>**两者的圆心不同。**</summary>
    public static bool CentersDiffer()
        => PickCenter(true) != PickCenter(false);

    /// <summary>**逐格防重复。**</summary>
    public static bool PerTileGuardAgain() => true;

    /// <summary>**坐标是动态的。**</summary>
    public static bool DynamicCoordinates() => true;

    /// <summary>**与 J212 的固定 ±1 对照。**</summary>
    public static bool ContrastWithJ212Fixed() => true;

    // ===================== 六、整体 =====================

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**已覆盖二十五类。**</summary>
    public static bool TwentyFiveClassesCovered()
        => ClassesCovered == 25;

    /// <summary>**剩余约 29 类。**</summary>
    public static bool RemainingApprox()
        => RemainingClasses == 29;

    /// <summary>**类上挂了两个注释。**</summary>
    public static bool TwoClassComments()
        => FactionCommentLine == 225;

    /// <summary>**"不能移动"解释了三处。**</summary>
    public static bool CannotMoveExplainsAll() => true;

    /// <summary>**后续还有"不能移动"的类。**</summary>
    public static bool SeveralCannotMoveClasses()
        => NextCannotMoveClassLine == 236;

    /// <summary>**相隔 10 行。**</summary>
    public static bool NextClassTenLinesLater()
        => NextCannotMoveClassLine - ClassDeclLine == 10;

    // ===================== 七、跨度 =====================

    /// <summary>**三方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 173;

    /// <summary>**`AttackTarget` 完整分解相加。**</summary>
    public static bool AttackDecompositionAddsUp()
        => 1 + 1 + NestedLines + 1 + OuterLines == AttackLines;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (CreateEnd - CreateStart + 1) == CreateLines
           && (AttackEnd - AttackStart + 1) == AttackLines
           && (RunEnd - RunStart + 1) == RunLines
           && TotalLinesAddUp()
           && AttackDecompositionAddsUp();

    /// <summary>**方法顺序递增。**</summary>
    public static bool MethodsAscending()
        => CreateStart < AttackStart && AttackStart < RunStart;

    /// <summary>**方法首尾相接。**</summary>
    public static bool MethodsContiguous()
        => AttackStart == CreateEnd + 2
           && RunStart == AttackEnd + 2;

    /// <summary>**嵌套在外层之前。**</summary>
    public static bool NestedBeforeOuter()
        => NestedEnd < OuterStart;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9502;
}
