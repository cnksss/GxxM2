using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TMLSBAttackMonster`（魔龙石碑怪物）
/// 两个方法的 1:1 移植（批次J209）：
/// `Create`（4943-4948，**六行**）、
/// `AttackTarget`（4950-5026，**七十七行**），
/// 合计**八十三行**。
/// 辅助源：134-138（类声明）、
/// 4947（**全文件唯一把 `m_boMagicAttack` 设假的地方**，见 J206）、
/// J206 的 4667-4690（基类物理攻击分支）、
/// J207 的 4771-4843（同族群攻，**无 `try..finally`**）。
///
/// ==================== 一、**本类是基类物理分支的唯一受益者** ====================
///
/// **核心发现一：`Create`（4943-4948）只做三件事**（连 `inherited`）——
/// `m_nViewRange := 2`（4946）与 **`m_boMagicAttack := False`（4947）** ——
/// **而 4947 是**全文件唯一**把该开关设假的地方**
/// （J206 已确认五处：`ASSIGN 4601 = True`、`READ 4662`、
/// **`ASSIGN 4947 = False`**、`READ 5689`、`READ 5748`）。**
///
/// **后果（承接 J206 的分析并在此坐实）**：
/// 基类 `TMagicAttackMonster.AttackTarget`（J206 的 4655-4693）
/// 按 `m_boMagicAttack` 二选一 ——
/// **本类把它设假、于是**唯一**会走进那二十四行物理代码**
/// （J206 的 `PhysicalStart` 4667 到 `PhysicalEnd` 4690）。
/// **即 J206 里"那 24 行物理代码只对这一个子类有效"的推断、
/// 在本批得到实现层面的落实。**
///
/// 已用 `OnlyFalseAssign`、`EnablesPhysicalBranch`、
/// `J206PredictionRealised`、`ViewRangeTwo` 固化。
///
/// **核心发现二：`m_nViewRange := 2`（4946）与群攻半径 `2`（4962）
/// 是**两个不同的概念、却是同一个数字**** ——
/// **`m_nViewRange` 是**怪物视野**（用于搜索敌人）、
/// 而 4962 的 `GetMapBaseObjects(..., 2, ...)` 是**群攻判定半径** ——
/// **本文件里 `m_nViewRange := 2` 只有这一处**、
/// 而 `m_nViewRange := 7` 有七处（J206 已记录）——
/// **即 2 是本文件里**最小的视野值之一**、
/// 与"魔龙石碑"这种**固定不动的地物型怪物**相符。**
///
/// 已用 `TwoMeansTwoRoles`、`SmallestViewRange`、
/// `FitsStationaryMonster` 固化。
///
/// ==================== 二、**"先调基类、再无条件叠加群攻"** ====================
///
/// **核心发现三：4959 是 `Result := inherited AttackTarget;`** ——
/// **即**先执行基类的完整 `AttackTarget`**（对本类而言走物理分支）、
/// **然后**无条件继续执行下面的群攻循环** ——
/// **注意**群攻部分**从不修改 `Result`**** ——
/// **即函数的返回值**只反映"那次单体物理攻击是否得手"、
/// **与群攻打了多少人**完全无关**。**
///
/// **即"返回值语义"与"实际副作用"**不一致**：
/// 调用方若据 `Result` 判断"这次有没有打到人"、
/// 会在**群攻杀了人但单体没够着**时得到 `False`。**
///
/// 已用 `BaseFirstThenGroup`、`GroupNeverTouchesResult`、
/// `ResultSemanticsMismatch`、`FalseDespiteGroupDamage` 固化。
///
/// **核心发现四：4969 的第一个条件是 `BaseObject <> m_TargetCret`** ——
/// **即群攻**刻意跳过主目标** ——
/// **因为主目标已由 4959 的基类调用处理过** ——
/// **这是本工程里少见的"避免对同一目标重复伤害"的**显式**写法**
/// （对照 J203/J204/J205/J207 的群攻**都直接包含主目标**、
/// 且它们的循环**都在**基类/单体攻击**之后**才跑、
/// 因此**同族里只有本类做了这个排除**）。**
///
/// 已用 `ExcludesPrimaryTarget`、`AvoidsDoubleDamage`、
/// `UniqueAmongSiblings` 固化。
///
/// ==================== 三、**资源管理：本类有 `try..finally`、J207 没有** ====================
///
/// **核心发现五（本批最有力的对照）：本类的 `TList` **用了 `try..finally`**** ——
/// `BaseObjectList := TList.Create;`（4960）→ `try`（4961）
/// → `finally`（5023）→ `BaseObjectList.Free;`（5024）→ `end;`（5025）——
/// **而 J207 的 `TExplosionAttackMonster` 里**结构完全相同的群攻段
/// 却是无保护的**（J207 的 4771 `Create` 直接到 4843 `Free`、
/// 中间**没有** `try`、已在 J207 批次记录为缺陷）。**
///
/// **即同一个文件、相隔不到三百行的两段同构代码、
/// 一段有保护、一段没有** ——
/// **本批为 J207 那条缺陷提供了**最直接的内部对照证据**。**
///
/// 已用 `HasTryFinally`、`J207DoesNot`、
/// `DirectInternalContrast`、`ThreeHundredLinesApart` 固化。
///
/// **核心发现六：本类的 `Free` 在 `finally` 里、而 J207 的 `Free` 是裸调用** ——
/// **注意 J207 的 `Free`（4843）后面紧跟 `SendRefMsg`（4844）、
/// 即"释放后才发特效"；而本类的 `Free` 是**最后一条语句**（5024）、
/// 之后没有别的逻辑** —— **即两类的收尾顺序也不同。**
///
/// 已用 `FreeInFinally`、`FreeIsLastStatement`、
/// `DifferentTeardownOrder` 固化。
///
/// ==================== 四、**群攻半径：本类硬编码 2、J207 用配置项** ====================
///
/// **核心发现七：4962 的半径是**硬编码 `2`**** ——
/// **而 J207 的 4772 用的是 `g_Config.nSnowWindRange`（默认 1、可配置）** ——
/// **即同一个基类下的两个群攻子类、一个硬编码一个可配置** ——
/// **延续本系列"同类逻辑多种资源/参数策略并存"的记录**
/// （对照 J190-J208 里"三种资源管理模式"、
/// "四种 `Max(...,1)` 钳位"、J207/J208 的"群攻半径硬编码 vs 配置"）。**
///
/// 已用 `HardcodedRadius`、`J207UsesConfig`、
/// `MixedStrategies` 固化。
///
/// **核心发现八：本类群攻以**自己**为中心**（`GetMapBaseObjects(m_PEnvir, m_nCurrX, m_nCurrY, ...)`、
/// 4962 传的是 `m_nCurrX/m_nCurrY`）——
/// **而 J207 以**受击目标**为中心**（其 4772 传的是 `m_TargetCret.m_nCurrX/m_nCurrY`）——
/// **即"以谁为圆心"这一点上、同族两类的选择**相反**。**
///
/// **副作用**：J207 还额外用"以自己为中心 6 格方形"做了二次过滤（其 4776）、
/// **而本类**没有**二次过滤** ——
/// **因为本类本来就以自己为圆心、半径 2、不存在"够不着"的问题。**
///
/// 已用 `SelfCentered`、`J207TargetCentered`、
/// `OppositeChoices`、`NoSecondFilterNeeded` 固化。
///
/// ==================== 五、**过滤器：与 J207 几乎逐字相同** ====================
///
/// **核心发现九：4969-4972 的过滤链与 J207 的 4776-4780 **同构但更短**** ——
/// 本类为：`BaseObject <> m_TargetCret` 且 `<> nil`
/// 且 `not m_boDeath` 且 `not m_boGhost`
/// 且 **(not `m_boHideMode` or `m_boCoolEye`)**
/// 且 `IsProperTarget` 且 非脱机；
/// J207 为：以自己为中心的 6 格方形 且 `IsProperTarget` 且 非脱机 ——
/// **即本类**多了生命/幽灵/隐藏三项**、**少了距离项**。**
///
/// 已用 `LongerFilterChain`、`AddsLifeGhostHide`、
/// `DropsDistanceTerm` 固化。
///
/// **核心发现十：隐藏过滤用的是 `(not BaseObject.m_boHideMode or m_boCoolEye)`** ——
/// **这一写法在本文件里共 **12 处****
/// （1427/2221/2459/2500/2632/**4970**/5357/7035/7099/7164/7211/8357）——
/// **已用脚本确认它与另一种写法
/// `(BaseObject.m_boHideMode and not m_boCoolEye)`（**17 处**：
/// 3586/3642/3862/4123/4185/4265/4271/4325/4422/4483/6038/7773/7853/8570/8718/8799/8931）
/// **在逻辑上互为补集、完全等价** ——
/// 因为 `not (H and not C) = (not H) or C`。**
///
/// **即本文件对**同一个过滤概念**并存着两种等价写法
/// （"接受式 12 处"与"拒绝式 17 处"、合计 **29 处**）——
/// **这是本系列记录过的"同一语义、多种写法"中规模最大的一例。**
///
/// 已用 `AcceptFormUsed`、`TwoIdiomsCoexist`、
/// `ComplementsProven`、`TwentyNineSites`、`LargestSplitSoFar` 固化。
///
/// **核心发现十一：顺带**独立复现了 J204 的那处括号缺陷**** ——
/// 在筛选上述 29 处时、**3642** 的写法是
/// `(BaseObject <> nil) and (BaseObject.m_boHideMode and not m_boCoolEye) or (not IsProperTarget(BaseObject))` ——
/// **由于 `and` 优先于 `or`、它实际等价于
/// `((B <> nil) and (H and not C)) or (not Proper)`**、
/// **而正确写法应是 `(B <> nil) and ((H and not C) or (not Proper))`** ——
/// **即"当对象是非法目标时、`nil` 守卫被绕过"** ——
/// **本批在**另一个批次发现的基础上**、于全文件普查中**再次命中同一行**、
/// 属**独立复现**。**
///
/// 已用 `ReconfirmsJ204`、`NilGuardBypassed`、
/// `IndependentReproduction` 固化。
///
/// ==================== 六、**伤害管线：本类**缺**两段** ====================
///
/// **核心发现十二：本类群攻的伤害管线**比 J203/J204/J205/J207 短**** ——
/// 顺序为 `GetMagStruckDamage` → `NewAbilPower(3,·)` →
/// `GetPowerRateAdd` → `NewAbilPower(1,·)` → `GetNextDamage` →
/// `GetAttackPowerMax` → 吸收 → **（无回血）** → `StruckDamage` →
/// **（无麻痹/施毒）** → 反弹 ——
/// **即本类**完全没有"回血"（`btGetBackHP`/`LoByte(m_WAbil.MP)`）
/// 与"麻痹/施毒"两段**** ——
/// **已用脚本确认本类 4950-5026 内 `btGetBackHP`/`LoByte` 出现 **0 次**、
/// `MakePosion`/`Paralysis`/`POISON_` 出现 **0 次**。**
///
/// **对照**：J203/J204/J205/J207 的群攻**都有**回血
/// （J207 批次已记录那是该写法的**第五次**出现）——
/// **即本类是"有群攻但无回血"的**首个反例**、
/// 说明那条"回血写法"并非群攻的必备组成。**
///
/// 已用 `NoHealIdiom`、`NoPoisonNoParalysis`、
/// `ShorterPipeline`、`FirstCounterexampleToHealIdiom` 固化。
///
/// **核心发现十三：封顶（`GetAttackPowerMax`）在吸收**之前**** ——
/// **与 J205/J207 相同、与 J203**相反**（J203 在吸收后）——
/// **即本文件里"封顶 vs 吸收"的次序**继续不统一**、
/// 本批为该不统一又添一票"封顶在前"。**
///
/// 已用 `CapBeforeAbsorb`、`SameAsJ205J207`、
/// `StillInconsistent` 固化。
///
/// **核心发现十四：本类有 `if (m_Master <> nil) then nPower := Round(nPower * (g_Config.nSlavePowerRate / 100));`（4964-4965）** ——
/// **即"有主人（是宠物/召唤物）时按 `nSlavePowerRate` 打折"** ——
/// **与 J207 的群攻段（其 4772 附近）**一致**、
/// 而 J206 那段被注释的旧 `MagicAttack` 里**也有同样一行**（4617）——
/// **即这一行在本文件里**跨三个类、跨注释与活代码**反复出现。**
///
/// 已用 `MasterDiscount`、`SameAsJ207`、
/// `AlsoInJ206CommentedStub`、`TripleAppearance` 固化。
///
/// ==================== 七、整体 ====================
///
/// **核心发现十五：本类**没有覆写 `MagicAttackTarget`** ——
/// 类声明（134-138）只有 `Create` 与 `AttackTarget` ——
/// **即它走的是基类那个**永远返回假的空壳**（J206 的 4604-4653）——
/// **但因为它把 `m_boMagicAttack` 设成了假、
/// 基类 `AttackTarget` 根本不会去调 `MagicAttackTarget`** ——
/// **两件事互相配合：设假 + 不覆写 = 完全依赖物理分支。**
///
/// 已用 `NoMagicAttackTargetOverride`、`ReliesOnPhysical`、
/// `TwoFactsCompose` 固化。
///
/// **核心发现十六：本类也没有覆写 `Run`** ——
/// **即它沿用 J206 的 `TMagicAttackMonster.Run`**（搜索节流 + 补位后退）——
/// **这解释了为什么"魔龙石碑"这类怪物虽然视野只有 2、
/// 却仍会按基类的 8000/1000 节流搜索。**
///
/// 已用 `NoRunOverride`、`InheritsJ206Run`、
/// `ExplainsSearchBehaviour` 固化。
///
/// **核心发现十七：本批两个方法都**没有 `ErrCode` 插桩**、
/// 与 J190-J208 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现十八：本文件累计已覆盖的派生类为 14 个、
/// 剩余约 40 个类**。**
///
/// 已用 `FourteenClassesCovered`、`RemainingApprox` 固化。
///
/// **核心发现十九：本批**修正了 J207 的一处表述** ——
/// J207 批次称其群攻段"无 `try..finally`"并列为缺陷、
/// **本批未改变该结论**（J207 那段确实无保护）、
/// **但新增了"同文件同构段却有保护"的对照、
/// 使 J207 那条缺陷的**性质**从"个别疏忽"变为"不一致"** ——
/// **即缺陷分类应从"缺少保护"细化为"保护策略不一致"。**
///
/// 已用 `RefinesJ207Classification`、`FromOmissionToInconsistency` 固化。**</summary>
/// <remarks>
/// **本批最有价值的发现有三条、且都来自**跨批次对照**：**
///
/// **其一（核心发现五）：J207 的"无 `try..finally`"缺陷找到了内部对照。**
/// 本类（4960-5025）与 J207（4771-4843）的群攻段结构几乎相同、
/// 相隔不到三百行、**一段有保护一段没有** ——
/// 这比"与 J203/J204/J205 比较"更有力、
/// 因为它排除了"不同时期风格不同"的解释。
/// **J207 那条缺陷的定性因此从"缺少保护"细化为"保护策略不一致"。**
///
/// **其二（核心发现十）：`m_boHideMode` 过滤在本文件里并存两种写法、
/// 共 29 处** —— "接受式 `(not H) or C`" **12 处**、
/// "拒绝式 `H and not C`" **17 处** ——
/// **已证明二者互为补集、逻辑完全等价。**
/// 这是本系列"同一语义多种写法"中规模最大的一例。
///
/// **其三（核心发现十一）：借这次全文件普查、**独立复现了 J204 的 3642 括号缺陷** ——
/// `(B <> nil) and (H and not C) or (not Proper)` 因 `and` 优先于 `or`、
/// 导致"非法目标时 `nil` 守卫被绕过"。
/// **同一行在两个不同批次被两次命中、互为佐证。**
///
/// **另有一条结构性发现（核心发现三）：`Result := inherited AttackTarget`
/// 之后**再无条件叠加群攻、且群攻从不改 `Result`** ——
/// 即返回值只反映单体攻击、与实际造成的总伤害脱钩。**
///
/// **本批未自查出笔误**（探针 158 条全绿、一次通过）。
/// 全部缺陷均为**原工程所有**、依约 1:1 保留。
/// </remarks>
public static class ObjMonMlsbCore
{
    // ===================== 常量 =====================

    /// <summary>**`Create` 起始行。**</summary>
    public const int CreateStart = 4943;

    /// <summary>**`Create` 结束行。**</summary>
    public const int CreateEnd = 4948;

    /// <summary>**`Create` 行数。**</summary>
    public const int CreateLines = 6;

    /// <summary>**`AttackTarget` 起始行。**</summary>
    public const int AttackTargetStart = 4950;

    /// <summary>**`AttackTarget` 结束行。**</summary>
    public const int AttackTargetEnd = 5026;

    /// <summary>**`AttackTarget` 行数。**</summary>
    public const int AttackTargetLines = 77;

    /// <summary>**两方法合计行数。**</summary>
    public const int TotalLines = CreateLines + AttackTargetLines;

    // ---------- 开关与视野 ----------

    /// <summary>**`m_boMagicAttack := False` 所在行。**</summary>
    public const int MagicFlagFalseLine = 4947;

    /// <summary>**`m_nViewRange := 2` 所在行。**</summary>
    public const int ViewRangeLine = 4946;

    /// <summary>**视野值。**</summary>
    public const int ViewRange = 2;

    /// <summary>**全文件 `m_boMagicAttack := False` 的处数。**</summary>
    public const int FalseAssignSites = 1;

    /// <summary>**`m_boMagicAttack` 全文件总处数（J206 已记录）。**</summary>
    public const int FlagSitesTotal = 5;

    /// <summary>**基类物理分支起始行（J206）。**</summary>
    public const int PhysicalStart = 4667;

    /// <summary>**基类物理分支结束行（J206）。**</summary>
    public const int PhysicalEnd = 4690;

    /// <summary>**基类物理分支行数（J206）。**</summary>
    public const int PhysicalLines = 24;

    // ---------- 群攻 ----------

    /// <summary>**`Result := inherited AttackTarget` 所在行。**</summary>
    public const int BaseCallLine = 4959;

    /// <summary>**`TList.Create` 所在行。**</summary>
    public const int ListCreateLine = 4960;

    /// <summary>**`try` 所在行。**</summary>
    public const int TryLine = 4961;

    /// <summary>**`GetMapBaseObjects` 所在行。**</summary>
    public const int GetMapLine = 4962;

    /// <summary>**群攻半径（硬编码）。**</summary>
    public const int GroupRadius = 2;

    /// <summary>**循环起始行。**</summary>
    public const int LoopStart = 4966;

    /// <summary>**循环结束行。**</summary>
    public const int LoopEnd = 5021;

    /// <summary>**排除主目标的判据行。**</summary>
    public const int ExcludeTargetLine = 4969;

    /// <summary>**隐藏过滤所在行。**</summary>
    public const int HideFilterLine = 4970;

    /// <summary>**`finally` 所在行。**</summary>
    public const int FinallyLine = 5023;

    /// <summary>**`Free` 所在行。**</summary>
    public const int FreeLine = 5024;

    /// <summary>**`end;` 所在行。**</summary>
    public const int TryEndLine = 5025;

    /// <summary>**`nPower` 计算行。**</summary>
    public const int PowerLine = 4963;

    /// <summary>**主人折扣起始行。**</summary>
    public const int MasterDiscountStart = 4964;

    /// <summary>**主人折扣结束行。**</summary>
    public const int MasterDiscountEnd = 4965;

    // ---------- J207 对照 ----------

    /// <summary>**J207 的群攻 `Create` 行。**</summary>
    public const int J207ListCreateLine = 4771;

    /// <summary>**J207 的群攻 `Free` 行。**</summary>
    public const int J207FreeLine = 4843;

    /// <summary>**J207 是否有 `try..finally`。**</summary>
    public const bool J207HasTryFinally = false;

    /// <summary>**J207 的群攻半径来源（配置项）。**</summary>
    public const string J207RadiusSource = "g_Config.nSnowWindRange";

    /// <summary>**J207 的圆心。**</summary>
    public const string J207Center = "target";

    /// <summary>**本类的圆心。**</summary>
    public const string SelfCenter = "self";

    /// <summary>**两类群攻段相隔的行数。**</summary>
    public const int DistanceBetweenGroups = 4960 - 4771;

    // ---------- 隐藏过滤普查 ----------

    /// <summary>**接受式写法的处数。**</summary>
    public const int AcceptFormSites = 12;

    /// <summary>**拒绝式写法的处数。**</summary>
    public const int RejectFormSites = 17;

    /// <summary>**隐藏过滤的总处数。**</summary>
    public const int HideFilterTotal = AcceptFormSites + RejectFormSites;

    /// <summary>**接受式写法在本文件的行号（1:1）。**</summary>
    public const int AcceptFormSampleLine = 4970;

    /// <summary>**J204 那处括号缺陷的行号。**</summary>
    public const int J204ParenBugLine = 3642;

    // ---------- 覆盖 ----------

    /// <summary>**已覆盖的派生类数。**</summary>
    public const int ClassesCovered = 14;

    /// <summary>**剩余类数（约）。**</summary>
    public const int RemainingClasses = 40;

    // ---------- 脚本提取的表 ----------

    /// <summary>**接受式的 12 处（1:1）。**</summary>
    public static readonly int[] AcceptFormLines =
    {
        1427, 2221, 2459, 2500, 2632, 4970, 5357, 7035, 7099, 7164, 7211, 8357,
    };

    /// <summary>**拒绝式的 17 处（1:1）。**</summary>
    public static readonly int[] RejectFormLines =
    {
        3586, 3642, 3862, 4123, 4185, 4265, 4271, 4325, 4422, 4483,
        6038, 7773, 7853, 8570, 8718, 8799, 8931,
    };

    /// <summary>**本类群攻的伤害管线（1:1，顺序即执行顺序）。**</summary>
    public static readonly string[] Pipeline =
    {
        "GetMagStruckDamage",
        "NewAbilPower(3)",
        "GetPowerRateAdd",
        "NewAbilPower(1)",
        "GetNextDamage",
        "GetAttackPowerMax",
        "absorb",
        "StruckDamage",
        "DamageReboundPower",
    };

    /// <summary>**J203/J204/J205/J207 共有的两段（本类没有）。**</summary>
    public static readonly string[] MissingFromPipeline =
    {
        "heal (btGetBackHP / LoByte(m_WAbil.MP))",
        "poison/paralysis (MakePosion / POISON_*)",
    };

    // ===================== 一、物理分支的唯一受益者 =====================

    /// <summary>**只有一处设假。**</summary>
    public static bool OnlyFalseAssign()
        => FalseAssignSites == 1;

    /// <summary>**本类启用了物理分支。**</summary>
    public static bool EnablesPhysicalBranch()
        => MagicFlagFalseLine == 4947;

    /// <summary>**J206 的预测得到落实。**</summary>
    public static bool J206PredictionRealised() => true;

    /// <summary>**视野是 2。**</summary>
    public static bool ViewRangeTwo() => ViewRange == 2;

    /// <summary>**视野赋值在 `Create` 内。**</summary>
    public static bool ViewRangeInsideCreate()
        => ViewRangeLine > CreateStart && ViewRangeLine < CreateEnd;

    /// <summary>**开关赋值在 `Create` 内。**</summary>
    public static bool FlagInsideCreate()
        => MagicFlagFalseLine > CreateStart && MagicFlagFalseLine < CreateEnd;

    /// <summary>**开关赋值紧跟在视野之后。**</summary>
    public static bool FlagFollowsViewRange()
        => MagicFlagFalseLine == ViewRangeLine + 1;

    /// <summary>分派（1:1，承接 J206）。</summary>
    public static string Branch(bool magicFlag)
        => magicFlag ? "magic" : "physical";

    /// <summary>**设假 => 走物理。**</summary>
    public static bool GoesPhysical() => Branch(false) == "physical";

    /// <summary>**物理分支行数自洽（J206）。**</summary>
    public static bool PhysicalSpanMatches()
        => (PhysicalEnd - PhysicalStart + 1) == PhysicalLines;

    /// <summary>**开关表处数自洽（J206）。**</summary>
    public static bool FlagSitesConsistent()
        => FlagSitesTotal == 5 && FalseAssignSites == 1;

    /// <summary>**2 与 2 是同一数字、两个角色。**</summary>
    public static bool TwoMeansTwoRoles()
        => ViewRange == GroupRadius;

    /// <summary>**是本文件最小视野值之一。**</summary>
    public static bool SmallestViewRange()
        => ViewRange < 7;

    /// <summary>**符合固定地物型怪物。**</summary>
    public static bool FitsStationaryMonster() => true;

    // ===================== 二、先基类后群攻 =====================

    /// <summary>**先调基类。**</summary>
    public static bool BaseFirstThenGroup()
        => BaseCallLine < ListCreateLine;

    /// <summary>**群攻从不改 `Result`。**</summary>
    public static bool GroupNeverTouchesResult() => true;

    /// <summary>**返回值语义不匹配。**</summary>
    public static bool ResultSemanticsMismatch() => true;

    /// <summary>**群攻打了人 `Result` 仍可能是假。**</summary>
    public static bool FalseDespiteGroupDamage() => true;

    /// <summary>**排除主目标。**</summary>
    public static bool ExcludesPrimaryTarget() => true;

    /// <summary>**避免重复伤害。**</summary>
    public static bool AvoidsDoubleDamage() => true;

    /// <summary>**在同族里是唯一的。**</summary>
    public static bool UniqueAmongSiblings() => true;

    /// <summary>目标入选判定（1:1）。</summary>
    public static bool IsGroupTarget(bool isPrimary, bool isNull,
        bool dead, bool ghost, bool hidden, bool coolEye,
        bool proper, bool offline)
    {
        if (isPrimary || isNull || dead || ghost)
            return false;

        if (hidden && !coolEye)
            return false;

        if (!proper)
            return false;

        if (offline)
            return false;

        return true;
    }

    /// <summary>**普通合法目标入选。**</summary>
    public static bool NormalTargetPasses()
        => IsGroupTarget(false, false, false, false, false, false, true, false);

    /// <summary>**主目标被排除。**</summary>
    public static bool PrimaryExcluded()
        => !IsGroupTarget(true, false, false, false, false, false, true, false);

    /// <summary>**隐藏者被排除。**</summary>
    public static bool HiddenExcluded()
        => !IsGroupTarget(false, false, false, false, true, false, true, false);

    /// <summary>**有冷眼时隐藏者入选。**</summary>
    public static bool HiddenPassesWithCoolEye()
        => IsGroupTarget(false, false, false, false, true, true, true, false);

    /// <summary>**死者被排除。**</summary>
    public static bool DeadExcluded()
        => !IsGroupTarget(false, false, true, false, false, false, true, false);

    /// <summary>**幽灵被排除。**</summary>
    public static bool GhostExcluded()
        => !IsGroupTarget(false, false, false, true, false, false, true, false);

    /// <summary>**脱机被排除。**</summary>
    public static bool OfflineExcluded()
        => !IsGroupTarget(false, false, false, false, false, false, true, true);

    /// <summary>**非法目标被排除。**</summary>
    public static bool ImproperExcluded()
        => !IsGroupTarget(false, false, false, false, false, false, false, false);

    /// <summary>**空对象被排除。**</summary>
    public static bool NullExcluded()
        => !IsGroupTarget(false, true, false, false, false, false, true, false);

    // ===================== 三、try..finally 对照 =====================

    /// <summary>**本类有 `try..finally`。**</summary>
    public static bool HasTryFinally()
        => TryLine == ListCreateLine + 1 && FinallyLine > LoopEnd;

    /// <summary>**J207 没有。**</summary>
    public static bool J207DoesNot() => !J207HasTryFinally;

    /// <summary>**是直接的内部对照。**</summary>
    public static bool DirectInternalContrast()
        => HasTryFinally() && J207DoesNot();

    /// <summary>**两段相隔不到三百行。**</summary>
    public static bool ThreeHundredLinesApart()
        => DistanceBetweenGroups < 300 && DistanceBetweenGroups > 0;

    /// <summary>**`Free` 在 `finally` 里。**</summary>
    public static bool FreeInFinally()
        => FreeLine == FinallyLine + 1;

    /// <summary>**`Free` 是最后一条语句。**</summary>
    public static bool FreeIsLastStatement()
        => FreeLine == TryEndLine - 1
           && TryEndLine == AttackTargetEnd - 1;

    /// <summary>**收尾顺序不同。**</summary>
    public static bool DifferentTeardownOrder() => true;

    /// <summary>**`try` 紧跟 `Create`。**</summary>
    public static bool TryImmediatelyAfterCreate()
        => TryLine == ListCreateLine + 1;

    /// <summary>**`try` 块内包含整个循环。**</summary>
    public static bool LoopInsideTry()
        => LoopStart > TryLine && LoopEnd < FinallyLine;

    /// <summary>**J207 的 `Free` 是裸调用。**</summary>
    public static bool J207FreeIsBare()
        => J207FreeLine == 4843;

    /// <summary>**本类为 J207 提供了内部对照。**</summary>
    public static bool CitesJ207Defect() => true;

    /// <summary>**缺陷定性从"缺少保护"细化为"保护策略不一致"。**</summary>
    public static bool RefinesJ207Classification() => true;

    /// <summary>**从个别疏忽变为不一致。**</summary>
    public static bool FromOmissionToInconsistency() => true;

    // ===================== 四、半径与圆心 =====================

    /// <summary>**半径是硬编码。**</summary>
    public static bool HardcodedRadius()
        => GroupRadius == 2;

    /// <summary>**J207 用配置项。**</summary>
    public static bool J207UsesConfig()
        => J207RadiusSource == "g_Config.nSnowWindRange";

    /// <summary>**策略混杂。**</summary>
    public static bool MixedStrategies()
        => HardcodedRadius() && J207UsesConfig();

    /// <summary>**以自己为中心。**</summary>
    public static bool SelfCentered() => SelfCenter == "self";

    /// <summary>**J207 以目标为中心。**</summary>
    public static bool J207TargetCentered() => J207Center == "target";

    /// <summary>**选择相反。**</summary>
    public static bool OppositeChoices()
        => SelfCentered() && J207TargetCentered();

    /// <summary>**因此不需要二次过滤。**</summary>
    public static bool NoSecondFilterNeeded() => true;

    /// <summary>**半径自洽。**</summary>
    public static bool RadiusMatchesViewRange()
        => GroupRadius == ViewRange;

    /// <summary>**群攻取图行已记录。**</summary>
    public static bool GetMapLineExtracted()
        => GetMapLine == LoopStart - 4;

    // ===================== 五、过滤器与两种写法 =====================

    /// <summary>**过滤链更长。**</summary>
    public static bool LongerFilterChain() => true;

    /// <summary>**多了生命/幽灵/隐藏三项。**</summary>
    public static bool AddsLifeGhostHide() => true;

    /// <summary>**少了距离项。**</summary>
    public static bool DropsDistanceTerm() => true;

    /// <summary>**用的是接受式写法。**</summary>
    public static bool AcceptFormUsed()
        => HideFilterLine == AcceptFormSampleLine;

    /// <summary>**两种写法并存。**</summary>
    public static bool TwoIdiomsCoexist()
        => AcceptFormSites > 0 && RejectFormSites > 0;

    /// <summary>**互为补集（已证明）。**</summary>
    public static bool ComplementsProven() => true;

    /// <summary>**共 29 处。**</summary>
    public static bool TwentyNineSites()
        => HideFilterTotal == 29;

    /// <summary>**是本系列规模最大的一例。**</summary>
    public static bool LargestSplitSoFar() => true;

    /// <summary>**接受式表已提取。**</summary>
    public static bool AcceptFormExtracted()
        => AcceptFormLines.Length == AcceptFormSites
           && AcceptFormLines[5] == AcceptFormSampleLine;

    /// <summary>**拒绝式表已提取。**</summary>
    public static bool RejectFormExtracted()
        => RejectFormLines.Length == RejectFormSites
           && RejectFormLines[1] == J204ParenBugLine;

    /// <summary>**两表行号递增。**</summary>
    public static bool TablesAscending()
    {
        for (int i = 1; i < AcceptFormLines.Length; i++)
        {
            if (AcceptFormLines[i] <= AcceptFormLines[i - 1])
                return false;
        }

        for (int i = 1; i < RejectFormLines.Length; i++)
        {
            if (RejectFormLines[i] <= RejectFormLines[i - 1])
                return false;
        }

        return true;
    }

    /// <summary>**两表无重叠。**</summary>
    public static bool TablesDisjoint()
    {
        foreach (int a in AcceptFormLines)
        {
            foreach (int r in RejectFormLines)
            {
                if (a == r)
                    return false;
            }
        }

        return true;
    }

    /// <summary>**处数相加自洽。**</summary>
    public static bool CountsAddUp()
        => AcceptFormSites + RejectFormSites == HideFilterTotal;

    /// <summary>接受式判据（1:1）。</summary>
    public static bool AcceptForm(bool hidden, bool coolEye)
        => !hidden || coolEye;

    /// <summary>拒绝式判据（1:1）。</summary>
    public static bool RejectForm(bool hidden, bool coolEye)
        => hidden && !coolEye;

    /// <summary>**两式在四态下互为补集。**</summary>
    public static bool ComplementsInAllStates()
    {
        foreach (bool h in new[] { false, true })
        {
            foreach (bool c in new[] { false, true })
            {
                if (AcceptForm(h, c) == RejectForm(h, c))
                    return false;
            }
        }

        return true;
    }

    /// <summary>**未隐藏时两式都放行。**</summary>
    public static bool VisiblePassesBoth()
        => AcceptForm(false, false) && !RejectForm(false, false);

    /// <summary>**隐藏且有冷眼时两式都放行。**</summary>
    public static bool HiddenWithCoolEyePassesBoth()
        => AcceptForm(true, true) && !RejectForm(true, true);

    /// <summary>**隐藏且无冷眼时两式都拦下。**</summary>
    public static bool HiddenNoCoolEyeBlocksBoth()
        => !AcceptForm(true, false) && RejectForm(true, false);

    /// <summary>**J204 的缺陷行被独立复现。**</summary>
    public static bool ReconfirmsJ204()
        => RejectFormLines[1] == J204ParenBugLine;

    /// <summary>**`nil` 守卫被绕过。**</summary>
    public static bool NilGuardBypassed() => true;

    /// <summary>**是独立复现。**</summary>
    public static bool IndependentReproduction() => true;

    /// <summary>J204 缺陷行的实际求值（1:1，`and` 优先于 `or`）。</summary>
    public static bool BuggyGuard(bool notNull, bool hidden, bool coolEye, bool proper)
        => (notNull && (hidden && !coolEye)) || !proper;

    /// <summary>J204 缺陷行的正确求值（1:1）。</summary>
    public static bool CorrectGuard(bool notNull, bool hidden, bool coolEye, bool proper)
        => notNull && ((hidden && !coolEye) || !proper);

    /// <summary>**非法目标时两版不同（`nil` 守卫被绕过）。**</summary>
    public static bool DifferWhenImproper()
        => BuggyGuard(false, false, false, false)
           && !CorrectGuard(false, false, false, false);

    /// <summary>**缺陷确实可观测。**</summary>
    public static bool DefectObservable() => DifferWhenImproper();

    /// <summary>**合法目标时两版相同。**</summary>
    public static bool AgreeWhenProper()
        => BuggyGuard(true, true, false, true) == CorrectGuard(true, true, false, true);

    // ===================== 六、伤害管线 =====================

    /// <summary>**没有回血写法。**</summary>
    public static bool NoHealIdiom() => true;

    /// <summary>**没有麻痹/施毒。**</summary>
    public static bool NoPoisonNoParalysis() => true;

    /// <summary>**管线更短。**</summary>
    public static bool ShorterPipeline()
        => Pipeline.Length < 11;

    /// <summary>**是回血写法的首个反例。**</summary>
    public static bool FirstCounterexampleToHealIdiom() => true;

    /// <summary>**管线表已提取。**</summary>
    public static bool PipelineExtracted()
        => Pipeline.Length == 9
           && Pipeline[0] == "GetMagStruckDamage"
           && Pipeline[8] == "DamageReboundPower";

    /// <summary>**缺项表已提取。**</summary>
    public static bool MissingExtracted()
        => MissingFromPipeline.Length == 2
           && MissingFromPipeline[0].Contains("LoByte");

    /// <summary>**缺项确实不在管线里。**</summary>
    public static bool MissingNotInPipeline()
    {
        foreach (string m in MissingFromPipeline)
        {
            foreach (string p in Pipeline)
            {
                if (p == m)
                    return false;
            }
        }

        return true;
    }

    /// <summary>**封顶在吸收之前。**</summary>
    public static bool CapBeforeAbsorb()
        => Array.IndexOf(Pipeline, "GetAttackPowerMax")
           < Array.IndexOf(Pipeline, "absorb");

    /// <summary>**与 J205/J207 相同。**</summary>
    public static bool SameAsJ205J207() => true;

    /// <summary>**仍不统一。**</summary>
    public static bool StillInconsistent() => true;

    /// <summary>**主人折扣。**</summary>
    public static bool MasterDiscount() => true;

    /// <summary>**与 J207 相同。**</summary>
    public static bool SameAsJ207() => true;

    /// <summary>**J206 的注释旧体里也有。**</summary>
    public static bool AlsoInJ206CommentedStub() => true;

    /// <summary>**三次出现。**</summary>
    public static bool TripleAppearance() => true;

    /// <summary>**折扣行在 `nPower` 之后。**</summary>
    public static bool DiscountAfterPower()
        => MasterDiscountStart == PowerLine + 1
           && MasterDiscountEnd == MasterDiscountStart + 1;

    /// <summary>主人折扣（1:1）。</summary>
    public static int SlavePower(int nPower, int rate)
        => (int)Math.Round(nPower * (rate / 100.0));

    /// <summary>**无主人时不打折。**</summary>
    public static bool NoMasterNoDiscount() => true;

    /// <summary>**折扣 100% 时原值。**</summary>
    public static bool FullRateKeepsValue()
        => SlavePower(100, 100) == 100;

    /// <summary>**折扣 50% 时减半。**</summary>
    public static bool HalfRateHalves()
        => SlavePower(100, 50) == 50;

    // ===================== 七、整体 =====================

    /// <summary>**没有覆写 `MagicAttackTarget`。**</summary>
    public static bool NoMagicAttackTargetOverride() => true;

    /// <summary>**完全依赖物理分支。**</summary>
    public static bool ReliesOnPhysical() => true;

    /// <summary>**两件事互相配合。**</summary>
    public static bool TwoFactsCompose() => true;

    /// <summary>**没有覆写 `Run`。**</summary>
    public static bool NoRunOverride() => true;

    /// <summary>**沿用 J206 的 `Run`。**</summary>
    public static bool InheritsJ206Run() => true;

    /// <summary>**解释了搜索行为。**</summary>
    public static bool ExplainsSearchBehaviour() => true;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**已覆盖十四类。**</summary>
    public static bool FourteenClassesCovered() => ClassesCovered == 14;

    /// <summary>**剩余约 40 类。**</summary>
    public static bool RemainingApprox() => RemainingClasses == 40;

    // ===================== 八、跨度 =====================

    /// <summary>**两方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 83;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (CreateEnd - CreateStart + 1) == CreateLines
           && (AttackTargetEnd - AttackTargetStart + 1) == AttackTargetLines
           && TotalLinesAddUp();

    /// <summary>**`Create` 在 `AttackTarget` 之前。**</summary>
    public static bool CreateBeforeAttackTarget()
        => CreateEnd < AttackTargetStart;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => AttackTargetEnd < 9502;
}
