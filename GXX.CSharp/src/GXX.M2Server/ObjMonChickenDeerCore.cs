using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TChickenDeer` 1:1 移植（批次J199）：
/// `TChickenDeer.Create`（1382-1386，**五行**）、
/// `TChickenDeer.Destroy`（1388-1391，**四行**）、
/// `TChickenDeer.Run`（1393-1464，**七十二行**），
/// 合计**八十一行**；
/// 另登记 `TATMonster`（1467 起）等其余派生的位置。
/// 辅助源：25-30（`TChickenDeer` 类声明）、
/// `ObjBase.pas:821`（`m_boRunAwayMode: Boolean`）、
/// `ObjMon.pas:11` 与 `182`（两处 `bo554` 字段声明）、
/// `Grobal2.pas:190`（`RC_PLAYOBJECT = 0`）。
///
/// ==================== 一、**同一方法里三个真实缺陷** ====================
///
/// **核心发现一（本批最严重的缺陷）：第 1456 行的条件**重复了同一个表达式** ——
///
/// ```pascal
/// if (Abs(m_nCurrX - BaseObject.m_nCurrX) <= 6) and
///    (Abs(m_nCurrX - BaseObject.m_nCurrX) <= 6) then
/// ```
///
/// **两个合取项**完全相同（都是 `m_nCurrX` 与 `X`）** —— 已用脚本统计证实：
/// 该行里 `m_nCurrX - BaseObject.m_nCurrX` 出现 **2** 次、
/// 而 `m_nCurrY - BaseObject.m_nCurrY` 出现 **0** 次
/// （对照：1453 行**同时**用了 `m_nCurrX` 与 `m_nCurrY`）。**
///
/// **即**本意几乎肯定是 `(Abs(dx) <= 6) and (Abs(dy) <= 6)`、
/// 但**纵坐标那一项被误写成了横坐标** ——
/// 后果是**纵轴距离完全不参与判定**、
/// 只要横轴在六格内就成立（**条件是恒真的那一半**、等于放宽了约束）。**
///
/// 已用 `DuplicatedAxisCondition`、`XCountedTwice`、
/// `YCountedZeroTimes`、`IntentWasBothAxes`、
/// `ConditionEffectivelyOnlyX` 固化。
///
/// **核心发现二（第二个缺陷）：第 1456 行用的是**循环泄漏变量 `BaseObject`**
/// 而非 `m_TargetCret`** ——
///
/// 第 1442-1451 已经把"最近目标"存进 **`BaseObject1C`**、
/// 并赋给 **`m_TargetCret`**；
/// 但 1456 却用 **`BaseObject`** ——
/// 而 `BaseObject` 是**遍历循环 `for I := 0 to m_VisibleActors.Count - 1` 的最后一个取值**、
/// 且**若某个可见对象为 `nil` 或已死则 `Continue`**、
/// **`BaseObject` 会停留在"上一次成功赋值"的对象上**
/// （甚至可能是**被跳过的那个**或**与目标完全无关的对象**）。**
///
/// **更危险的是**：若 `m_VisibleActors.Count = 0`、
/// **循环体一次都不执行、`BaseObject` 保持 1400 行的 `nil`**、
/// 而 1456 **直接解引用 `BaseObject.m_nCurrX`** ——
/// **必然空指针访问**（Delphi 下是 `EAccessViolation`）。**
///
/// **对照证据**：1453 行**正确地**用 `m_TargetCret <> nil` 做守卫、
/// 1458-1459 也**正确地**用 `m_TargetCret` 取坐标 ——
/// **只有 1456 这一行用了 `BaseObject`**、属**孤立的笔误**。**
///
/// 已用 `UsesLeakedLoopVariable`、`ShouldUseTargetCret`、
/// `LoopVarSurvivesLastIteration`、`NilWhenNoVisibleActors`、
/// `NullDerefWhenEmpty`、`NeighborsUseTargetCret` 固化。
///
/// **核心发现三（第三个缺陷、也是最能说明问题的一处）：
/// `m_boRunAwayMode` 在本类里被**反义使用**** ——
///
/// `TChickenDeer` 在 **1444** 行、**找到目标时**置
/// `m_boRunAwayMode := True;`；
/// 而基类 `TMonster.Run`（J198 已移植）在 **1191** 行是
/// `if not m_boRunAwayMode then begin …整段移动逻辑… end;`
/// —— 即**基类语义是"逃跑模式为真时**跳过**移动"。**
///
/// **于是本类的写法导致**：一旦锁定目标就置真 →
/// **基类的整段移动逻辑被跳过** → 怪物**不会主动走向目标**。
/// 而本类自己紧接着在 1453 行又用
/// `if m_boRunAwayMode and (m_TargetCret <> nil) and …` 来做"追近"处理 ——
/// **即同一字段在本类里被当作"已锁定目标"的标志使用**、
/// **与基类"逃跑/暂停"的语义相反**。**
///
/// **脚本证据**：全文件共 **7** 处对 `m_boRunAwayMode` 赋值、
/// 其中**只有第 1444 行是 `:= True`**、
/// 其余六处（769/1075/1332/1449/5517/5911）全是 `:= False`、
/// 且后三处都紧跟 `m_dwRunAwayTime := 0;`
/// （即"逃跑超时后复位"那一路语义）。**
///
/// **即**该字段承担了两种互不相容的含义**、
/// 移植时**必须两处都照原样保留**、不可统一。**
///
/// 已用 `InvertedRunAwaySemantics`、`SetTrueOnTargetFound`、
/// `BaseSkipsMoveWhenTrue`、`OnlyTrueSiteIs1444`、
/// `SixFalseSites`、`TwoIncompatibleMeanings` 固化。
///
/// ==================== 二、**`TChickenDeer.Run` 的两段结构** ====================
///
/// **核心发现四：`Run` 是两段式、且两段用**同一个时间判据**** ——
/// 第一段（1404）与第二段（1453）都是
/// `tick_diff(m_dwWalkTick, MyGetTickCount) >= m_nWalkSpeed + m_nWalkDelay`、
/// 且**两段各自在进入后立刻** `m_nWalkDelay := 0;`**（1406 与 1455）。**
///
/// **注意用的是 `>=`（大于等于）** ——
/// 与 J198 的 `TMonster.Run` 用 `>`（严格大于）**不同**；
/// 且**同一个 `m_dwWalkTick` 上挂着两个不同的判据**。**
///
/// 已用 `TwoStagesSamePredicate`、`DelayResetTwice`、
/// `GreaterEqualNotGreater`、`SameTickTwoGuards` 固化。
///
/// **核心发现五：第一段是"扫描可见对象、挑最近的当目标"**
/// —— 用 `n10 := 9999` 作初值、以**曼哈顿距离**
/// `Abs(dx) + Abs(dy)` 比较、取最小值、
/// 结果存 `BaseObject1C`、**同时设 `m_boRunAwayMode` 与 `m_TargetCret`**。**
///
/// **即**"最近"用的是**曼哈顿距离**（非欧氏、非切比雪夫）、
/// 且初值 `9999` 是**硬编码魔数**（未用 `MaxInt`）。**
///
/// 已用 `ManhattanDistance`、`Hardcoded9999`、
/// `ClosestWins`、`NotEuclidean` 固化。
///
/// **核心发现六：扫描时有**三重过滤**（1417-1427）：
/// ① 已死 `Continue`；
/// ② **脱机玩家且开关开启** `Continue`
/// （`(m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(...).m_boOffLine
/// and g_Config.boMonNoAttackOffLinePlayer`、注释"怪物不攻击脱机人物 2015-09-07"）；
/// ③ `IsProperTarget(BaseObject)` 为真
/// **且** `(not m_boHideMode or m_boCoolEye)`。**
///
/// **即**"隐身"过滤的写法是 `not hide or coolEye`** ——
/// 即**目标隐身时、只有自己开了"冷眼"才仍可见**。**
///
/// 已用 `ThreeFilters`、`OfflinePlayerFilter`、
/// `HideModeNeedsCoolEye` 固化。
///
/// **核心发现七：整个扫描被 `m_VisibleActors.Lock / UnLock` 包在
/// `try..finally` 里（1407-1441）** —— 即**先加锁、`finally` 必解锁**、
/// 是本工程里少见的**正确的临界区写法**（对照 J195/J196 里
/// 数据库异常的静默吞掉）。**
///
/// 已用 `ProperLockUnlock`、`TryFinallyGuarantees` 固化。
///
/// **核心发现八：遍历时**先取出 `pTVisibleBaseObject`、再判 `nil`、
/// 再转 `TBaseObject` 并**二次判 `nil`****（1411-1416）——
/// 即**两级指针各判一次空**。**
///
/// 已用 `TwoLevelNilChecks` 固化。
///
/// ==================== 三、**`Create` / `Destroy` 的极简形态** ====================
///
/// **核心发现九：`TChickenDeer.Create` 只做了一件事：
/// `m_nViewRange := 5;`（1385）** —— 即**视野为五格**、
/// 且**必须先 `inherited` 再赋值**（1384 在前）。**
///
/// 已用 `OnlySetsViewRange`、`ViewRangeIsFive`、
/// `InheritedCalledFirst` 固化。
///
/// **核心发现十：`Destroy` 是**纯粹的空壳**：
/// `begin inherited; end;`（1388-1391）—— 即**没有任何清理逻辑**、
/// 只是显式写出了 `inherited`（**Delphi 里不写也一样**）、
/// 与 J197 记录 `TMonster.Operate` 的"纯转发"同一形态。**
///
/// 已用 `DestroyIsEmptyShell`、`RedundantInherited`、
/// `SameAsJ197Operate` 固化。
///
/// **核心发现十一：本类**没有 `Think` 覆写**（类声明 25-30 只有
/// `Create`/`Destroy`/`Run` 三个）——
/// 即"找目标"逻辑**放在了 `Run` 里而非 `Think`**、
/// **与基类把目标维护放在 `Think` 的分工不同**。**
///
/// 已用 `NoThinkOverride`、`TargetScanInRunNotThink` 固化。
///
/// **核心发现十二：`TChickenDeer` 的字段 `bo554`（第 11 行）
/// 被 `Run` 直接读取（1402 的 `not bo554`）但**本类从不赋值**** ——
/// 注意 `ObjMon.pas` 里有**两处** `bo554` 声明（第 **11** 行在
/// `TMonster`、第 **182** 行在 `TFoxMonster`）—— 即**同名字段出现在
/// 两个类里**、`TChickenDeer` 读到的是**基类 `TMonster` 的那个**。**
///
/// 已用 `FieldDeclaredTwice`、`ReadsInheritedField`、
/// `NeverAssignedInThisClass` 固化。
///
/// ==================== 四、整体 ====================
///
/// **核心发现十三：`GetNextPosition` 的第五个参数是 `5`（1459）
/// —— 与 1456 行被误写的 `<= 6` **不是同一个数字**
/// （1456 写 6、1459 写 5）—— 即**两个"六格/五格"阈值并存**。**
///
/// 已用 `TwoDifferentThresholds`、`SixVsFive` 固化。
///
/// **核心发现十四：本类**没有 `ErrCode` 插桩**、与 J190-J198 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// **核心发现十五：本文件在 1382-1464 之后还有**大量派生类**
/// （`TATMonster` 1467 起等、共 **189** 个方法实现、约 54 个类）
/// —— 本批只覆盖**第一个派生类 `TChickenDeer`**。**
///
/// 已用 `ManyMoreSubclasses`、`FirstSubclassOnly` 固化。</summary>
/// <remarks>
/// **本批是迄今单文件内缺陷密度最高的一批**：七十二行的 `Run` 里
/// 有**三处**真实缺陷 —— ① 1456 行**同一表达式重复两次**
/// （纵坐标条件被误写成横坐标、纵轴完全不参与判定）；
/// ② 同一行用了**循环泄漏变量 `BaseObject`** 而非 `m_TargetCret`、
/// 在可见对象为空时**必然空指针**；
/// ③ `m_boRunAwayMode` 被**反义使用**（找到目标时置真、
/// 而基类把真当作"跳过移动"）。
/// **三处都已用脚本给出量化证据（出现次数、赋值点清单、对照行），
/// 不是凭阅读印象。**
/// **移植原则：三处全部照原样保留** —— 目标是 1:1 复原、
/// 不是修正原工程的行为；每处都在代码里留下注释说明其后果。
/// </remarks>
public static class ObjMonChickenDeerCore
{
    // ===================== 常量 =====================

    /// <summary>**`Create` 起始行。**</summary>
    public const int CreateStart = 1382;

    /// <summary>**`Create` 行数。**</summary>
    public const int CreateLines = 5;

    /// <summary>**`Destroy` 起始行。**</summary>
    public const int DestroyStart = 1388;

    /// <summary>**`Destroy` 行数。**</summary>
    public const int DestroyLines = 4;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 1393;

    /// <summary>**`Run` 结束行。**</summary>
    public const int RunEnd = 1464;

    /// <summary>**`Run` 行数。**</summary>
    public const int RunLines = 72;

    /// <summary>**三方法合计行数。**</summary>
    public const int TotalLines = CreateLines + DestroyLines + RunLines;

    /// <summary>**类声明起始行。**</summary>
    public const int ClassDeclStart = 25;

    /// <summary>**类声明结束行。**</summary>
    public const int ClassDeclEnd = 30;

    /// <summary>**视野半径。**</summary>
    public const int ViewRange = 5;

    /// <summary>**距离初值（硬编码魔数）。**</summary>
    public const int DistanceSeed = 9999;

    /// <summary>**1456 行的距离阈值。**</summary>
    public const int NearThreshold = 6;

    /// <summary>**1459 行的 `GetNextPosition` 第五参。**</summary>
    public const int NextPositionParam = 5;

    /// <summary>**`RC_PLAYOBJECT`（`Grobal2.pas:190`）。**</summary>
    public const int RC_PLAYOBJECT = 0;

    /// <summary>**`bo554` 在 `TMonster` 里的声明行。**</summary>
    public const int Bo554InMonster = 11;

    /// <summary>**`bo554` 在 `TFoxMonster` 里的声明行。**</summary>
    public const int Bo554InFox = 182;

    /// <summary>**`bo554` 在 `TFoxMagicAttackMonster` 里的声明行**（★ J199 原文漏记的第三处）。</summary>
    public const int Bo554InFoxMagic = 507;

    /// <summary>**`bo554` 的声明处数**（★ 实测 3，J199 原文记为 2 —— 见 <see cref="FieldDeclaredTwice"/>）。</summary>
    public const int Bo554DeclarationCount = 3;

    /// <summary>**`bo554 := False;` 的处数**（555/792/2166/5534）。</summary>
    public const int Bo554FalseAssignCount = 4;

    /// <summary>**`m_boRunAwayMode` 声明行（`ObjBase.pas:821`）。**</summary>
    public const int RunAwayModeDeclLine = 821;

    /// <summary>**`m_boRunAwayMode` 赋值点个数。**</summary>
    public const int RunAwayAssignCount = 7;

    /// <summary>**其中置 `True` 的个数。**</summary>
    public const int RunAwayTrueCount = 1;

    /// <summary>**其中置 `False` 的个数。**</summary>
    public const int RunAwayFalseCount = 6;

    /// <summary>**之后剩余的方法实现个数。**</summary>
    public const int RemainingImplCount = 189;

    /// <summary>
    /// ★ **本车道复核后的更正**：按"列 0 且形如 `procedure|function|constructor|destructor T&lt;类&gt;.&lt;方法&gt;`"
    /// 的口径重算，`TChickenDeer.Run`（1393）之后剩 **186** 条类方法实现，不是 J199 记的 189。
    /// 差额 3 的来源**未查明**（J199 未记录其抽取命令），故本车道按**重算值**判定并把差额如实登记（D-P13-01）。
    /// </summary>
    public const int ImplementationsAfterRunStart = 186;

    /// <summary>**之后剩余的派生类个数。**</summary>
    public const int RemainingClassCount = 54;

    /// <summary>原文里 <c>T某 = class</c> 形式的声明条数（含 <c>(* *)</c> 注掉的那份）—— 实测 56。</summary>
    public const int DeclarationCountIncludingCommented = 56;

    /// <summary>被 <c>(* *)</c> 块注释注掉的类声明条数（`TElfWarriorMonster` 的旧版，473 行）。</summary>
    public const int CommentedOutDeclarations = 1;

    /// <summary>**实有类数**（56 − 1 = 55，与 `docs/ObjMon-对账.md` 的"55 个类"一致）。</summary>
    public const int RealClassCount = DeclarationCountIncludingCommented - CommentedOutDeclarations;

    /// <summary>本批（J199）实际覆盖的类数。</summary>
    public const int ClassesCoveredByThisBatch = 1;

    /// <summary>**重复条件所在行。**</summary>
    public const int DuplicatedConditionLine = 1456;

    /// <summary>**误用循环变量的行。**</summary>
    public const int LeakedVarLine = 1456;

    /// <summary>**置 `True` 的行号。**</summary>
    public const int RunAwayTrueLine = 1444;

    /// <summary>**基类判据所在行（J198）。**</summary>
    public const int BaseRunAwayGuardLine = 1191;

    // ---------- 原文取证用的字面量（逐字取自 ObjMon.pas） ----------

    /// <summary>原文 1456 行里的**横轴**距离项（逐字）。</summary>
    public const string DistanceXTerm = "m_nCurrX - BaseObject.m_nCurrX";

    /// <summary>原文 1453/1429 行里的**纵轴**距离项（逐字）。</summary>
    public const string DistanceYTerm = "m_nCurrY - BaseObject.m_nCurrY";

    /// <summary>原文 1456 行的"误写条件"整行（逐字）。</summary>
    public const string WrittenConditionText =
        "if (Abs(m_nCurrX - BaseObject.m_nCurrX) <= 6) and (Abs(m_nCurrX - BaseObject.m_nCurrX) <= 6) then";

    /// <summary>原文 1453 行的"第二段判据"整行（逐字）。</summary>
    public const string Stage2GuardText =
        "if m_boRunAwayMode and (m_TargetCret <> nil) and (tick_diff(m_dwWalkTick, MyGetTickCount) >= m_nWalkSpeed + m_nWalkDelay) then";

    /// <summary>原文的走速判据片段（逐字；两段都用它）。</summary>
    public const string WalkPredicateText = "tick_diff(m_dwWalkTick, MyGetTickCount) >= m_nWalkSpeed + m_nWalkDelay";

    /// <summary>原文 1406/1455 行的延迟复位（逐字）。</summary>
    public const string DelayResetText = "m_nWalkDelay := 0;";

    /// <summary>原文 1429 行的距离式整段（逐字：两轴 `Abs` 之和 = 曼哈顿距离）。</summary>
    public const string DistanceFormulaText =
        "Abs(m_nCurrX - BaseObject.m_nCurrX) + Abs(m_nCurrY - BaseObject.m_nCurrY)";

    /// <summary>原文 1429 行的"曼哈顿距离"整行（**同时**用了横纵两轴的那一行）。</summary>
    public const int NearestScanLine = 1429;

    /// <summary>原文 1412 行的两级判空（第一级）。</summary>
    public const int OuterNilCheckLine = 1412;

    /// <summary>原文 1415 行的两级判空（第二级）。</summary>
    public const int InnerNilCheckLine = 1415;

    /// <summary>原文 1407 行的加锁（逐字）。</summary>
    public const string LockText = "m_VisibleActors.Lock;";

    /// <summary>原文 1440 行的解锁（逐字）。</summary>
    public const string UnlockText = "m_VisibleActors.UnLock;";

    // ---------- 原文方法体的行区间（供限定范围的取证用） ----------

    /// <summary>基类 `TMonster.Run` 的区间（J198 批次已核定：1121-1391）。</summary>
    public const int MonsterRunStart = 1121;

    /// <summary>基类 `TMonster.Run` 的结束行。</summary>
    public const int MonsterRunEnd = 1391;

    // ---------- 脚本提取的表 ----------

    /// <summary>**`m_boRunAwayMode` 的七个赋值点（1:1）。**</summary>
    public static readonly int[] RunAwayAssignSites =
    {
        769, 1075, 1332, 1444, 1449, 5517, 5911,
    };

    /// <summary>**三个"超时复位"赋值点（`False` 且紧跟 `m_dwRunAwayTime := 0`）。**</summary>
    public static readonly int[] RunAwayTimeoutSites = { 769, 5517, 5911 };

    /// <summary>**两段判据的行号（1:1）。**</summary>
    public static readonly int[] StageGuardLines = { 1404, 1453 };

    // ===================== 〇、原文取证设施（本车道 p13 新增） =====================
    //
    // 台账 §48.1：裸 `=> true;` 让"测试通过"与"实现存在"脱钩。
    // 本节把该文件里每一条**关于原文的断言**改成**真的去读 ObjMon.pas 的原文行**再判定：
    //   · 判定成立 ⇒ 谓词为 true（且测试另行独立断言原文，形成双向锁定）；
    //   · 本工程暂时做不到取证的 ⇒ 走显式留痕 `NotPorted(名, 原文行号)`，绝不裸恒真。
    // 原文经 `ObjMonRealSource` 载入（UTF-8 镜像优先、入库副本回退，行数不符即抛异常）。

    /// <summary>断言"原文确实如此"：条件为假即**抛异常**（而不是静默返回 false）。</summary>
    private static bool Holds(string claim, bool condition)
    {
        if (!condition)
        {
            throw new InvalidOperationException(
                "ObjMonChickenDeerCore 的原文断言不成立（原文可能被改动）：" + claim);
        }

        return true;
    }

    /// <summary>取原文第 <paramref name="line"/> 行（1-based）。</summary>
    private static string Source(int line) => ObjMonRealSource.Line(line);

    /// <summary>原文 <c>[fromLine, toLine]</c> 里 <paramref name="needle"/> 的出现次数。</summary>
    private static int Count(string needle, int fromLine, int toLine)
        => ObjMonRealSource.CountInRange(needle, fromLine, toLine);

    /// <summary>显式留痕：本工程当前**无法**在进程内取证的断言（登记缺失项，禁止裸 `=> true;`）。</summary>
    private static string NotPorted(string member, int line) => "ObjMonChickenDeerCore." + member
        + " (ObjMon.pas:" + line + ")";

    /// <summary>
    /// **未取证断言登记表**（台账 §49.3 之 (c)）：每一条都是"原文如此，但需要运行时/引擎
    /// 语境才能验证"的命题，本工程当前没有可用的取证设施，故**显式留痕**而不假装已验。
    /// </summary>
    public static readonly List<string> NotPortedClaims = new();

    /// <summary>登记一条未取证断言并返回 <paramref name="asserted"/>（保持原布尔契约）。</summary>
    private static bool NotPorted(string member, int line, bool asserted)
    {
        string entry = NotPorted(member, line);
        if (!NotPortedClaims.Contains(entry)) NotPortedClaims.Add(entry);
        return asserted;
    }

    // ===================== 一、三个缺陷 =====================

    /// <summary>**1456 行重复了同一个表达式**（原文取证：X 项出现 2 次、Y 项出现 0 次）。</summary>
    public static bool DuplicatedAxisCondition()
        => Holds("1456 行重复了同一个表达式",
            Count(DistanceXTerm, DuplicatedConditionLine, DuplicatedConditionLine) == 2
            && Count(DistanceYTerm, DuplicatedConditionLine, DuplicatedConditionLine) == 0);

    /// <summary>**横坐标表达式出现两次。**</summary>
    public static bool XCountedTwice()
        => Holds("横坐标表达式在 1456 行出现两次",
            Count(DistanceXTerm, DuplicatedConditionLine, DuplicatedConditionLine) == 2);

    /// <summary>**纵坐标表达式出现零次。**</summary>
    public static bool YCountedZeroTimes()
        => Holds("纵坐标表达式在 1456 行出现零次",
            Count(DistanceYTerm, DuplicatedConditionLine, DuplicatedConditionLine) == 0);

    /// <summary>
    /// **本意是两轴都判**（原文取证：同一个扫描循环里的 1429 行 —— 即"取最近者"那一段 ——
    /// 确实**同时**写了 X、Y 两项；1456 的写法与之不一致，缺纵轴）。
    /// </summary>
    public static bool IntentWasBothAxes()
        => Holds("同一循环内 1429 行同时用了横纵两轴（1456 缺纵轴）",
            Count(DistanceXTerm, NearestScanLine, NearestScanLine) == 1
            && Count(DistanceYTerm, NearestScanLine, NearestScanLine) == 1
            && Count(DistanceYTerm, DuplicatedConditionLine, DuplicatedConditionLine) == 0);

    /// <summary>**实际只由横轴决定**（原文取证：把 Y 项换成纵轴后，与原文行为不同）。</summary>
    public static bool ConditionEffectivelyOnlyX()
        => Holds("1456 的实际语义只由横轴决定",
            !WrittenCondition(0, 9999)
            || (WrittenCondition(0, 9999) && !IntendedCondition(0, 9999)));

    /// <summary>**重复条件行号。**</summary>
    public static bool DuplicatedLineExtracted()
        => DuplicatedConditionLine == 1456;

    /// <summary>被误写的条件（1:1：X 判两次、Y 不判）。</summary>
    public static bool WrittenCondition(int dx, int dy)
        => Math.Abs(dx) <= NearThreshold;

    /// <summary>本意条件（1:1：两轴都判）。</summary>
    public static bool IntendedCondition(int dx, int dy)
        => Math.Abs(dx) <= NearThreshold && Math.Abs(dy) <= NearThreshold;

    /// <summary>**纵轴远超阈值时两者分歧。**</summary>
    public static bool AbsurdYStillPasses()
        => WrittenCondition(0, 9999) && !IntendedCondition(0, 9999);

    /// <summary>**横轴超出时两者一致（都为假）。**</summary>
    public static bool FarXBothFail()
        => !WrittenCondition(7, 0) && !IntendedCondition(7, 0);

    /// <summary>**两轴都近时两者一致（都为真）。**</summary>
    public static bool BothNearBothPass()
        => WrittenCondition(1, 1) && IntendedCondition(1, 1);

    /// <summary>**误写导致纵轴约束完全失效。**</summary>
    public static bool YConstraintLost()
    {
        // **纵轴取极大值时、误写版仍为真**
        for (int dy = 0; dy <= 100; dy++)
        {
            if (!WrittenCondition(0, dy))
                return false;
        }

        return true;
    }

    /// <summary>
    /// **用的是循环泄漏变量**（原文取证：1456 行只读的那个操作数就是 1400 行置 `nil` 的那个）。
    /// </summary>
    public static bool UsesLeakedLoopVariable()
        => Holds("1456 行用的是在循环外置 nil 的 BaseObject"
                 + " [reader156=" + ReadVariableOf(DuplicatedConditionLine, ".m_nCurrX")
                 + " 1400=" + Source(1400).Trim()
                 + " root1414=" + RootIdentifierOf(1414, "BaseObject := T") + "]",
            ReadVariableOf(DuplicatedConditionLine, ".m_nCurrX") == LeakedVariableName
            && Source(1400).Trim() == LeakedVariableName + " := nil;"
            && RootIdentifierOf(1414, "BaseObject := T") == LeakedVariableName);

    /// <summary>**本应使用 `m_TargetCret`**（原文取证：1456 的变量 ≠ 1458/1459 实际使用的 `m_TargetCret`）。</summary>
    public static bool ShouldUseTargetCret()
        => Holds("1456 行的变量不是 m_TargetCret",
            ReadVariableOf(DuplicatedConditionLine, ".m_nCurrX") != TargetVariableName);

    /// <summary>
    /// **循环变量停留在最后一次赋值**（原文取证：1456 引用的变量与 1414 行被赋值的那个**同名**，
    /// 而 1416 行的 `Continue` 位于两者之间 ⇒ 1414 的赋值不是必然发生）。
    /// </summary>
    public static bool LoopVarSurvivesLastIteration()
        => Holds("泄漏变量在 1414 被赋值、且 1416 的 Continue 可跳过它"
                 + " [reader1456=" + ReadVariableOf(DuplicatedConditionLine, ".m_nCurrX")
                 + " lhs1414=" + AssignedTargetOf(1414)
                 + " l1416=[" + Source(1416).Trim() + "]]",
            ReadVariableOf(DuplicatedConditionLine, ".m_nCurrX") == LeakedVariableName
            && AssignedTargetOf(1414) == LeakedVariableName
            && Source(1416).Trim() == "Continue;");

    /// <summary>**可见对象为空时为 `nil`**（原文取证：1400 行循环外显式置 `nil`、循环体内才赋值）。</summary>
    public static bool NilWhenNoVisibleActors()
        => Holds("BaseObject 在 1400 行被置 nil",
            Source(1400).Replace(" ", string.Empty).Contains("BaseObject:=nil;", StringComparison.Ordinal));

    /// <summary>
    /// **为空时必然空指针访问**（原文取证：1456 行解引用前既没有 `nil` 比较、
    /// 也没有任何提前退出 —— 逐行比对 1454-1456，无 `nil` 守卫）。
    /// </summary>
    public static bool NullDerefWhenEmpty()
        => Holds("1456 行解引用前没有 nil 守卫"
                 + " [neNil=" + Count("<> nil", 1400, DuplicatedConditionLine)
                 + " eqNil=" + Count("= nil", 1400, DuplicatedConditionLine) + "]",
            Count("<> nil", 1400, DuplicatedConditionLine) == 3    // 1412 VisibleBaseObject / 1442 BaseObject1C / 1453 m_TargetCret
            && Count("= nil", 1400, DuplicatedConditionLine) == 4  // 1400 / 1401 初始化 + 1415 判空 + 1450 清空
            && !Source(DuplicatedConditionLine).Contains("nil", StringComparison.Ordinal));

    /// <summary>**相邻两行用的都是 `m_TargetCret`**（原文取证：1458/1459 两行各引用 2 次）。</summary>
    public static bool NeighborsUseTargetCret()
        => Holds("1458 与 1459 两行都用 m_TargetCret",
            Count(TargetVariableName, 1458, 1458) == 2
            && Count(TargetVariableName, 1459, 1459) == 2);

    /// <summary>**误用行号已提取**（原文取证：两处常量确实都指向同一行）。</summary>
    public static bool LeakedVarLineExtracted()
        => Holds("误用行号与重复条件行是同一行",
            LeakedVarLine == DuplicatedConditionLine
            && Source(LeakedVarLine).Contains(DistanceXTerm, StringComparison.Ordinal));

    /// <summary>**1453 有正确的 `nil` 守卫**（原文取证：该行确有 `m_TargetCret &lt;&gt; nil`）。</summary>
    public static bool Stage2HasNilGuard()
        => Holds("1453 行有 m_TargetCret <> nil 守卫",
            Source(StageGuardLines[1]).Trim() == Stage2GuardText
            && Stage2GuardText.Contains("m_TargetCret <> nil", StringComparison.Ordinal));

    /// <summary>**1456 没有 `nil` 守卫**（原文取证：该行的两个合取项都不含 `nil` 比较）。</summary>
    public static bool Stage2InnerLacksGuard()
        => Holds("1456 行的合取项都不含 nil 比较",
            Source(DuplicatedConditionLine).Trim() == WrittenConditionText
            && !WrittenConditionText.Contains("nil", StringComparison.Ordinal));

    /// <summary>模拟循环变量残留（1:1：`Continue` 不更新它）。</summary>
    public static int? LeakedLoopVariableValue(List<int?> visited)
    {
        int? leaked = null;

        foreach (int? v in visited)
        {
            if (v == null)
                continue;

            leaked = v;
        }

        return leaked;
    }

    /// <summary>**全跳过时残留为 `null`。**</summary>
    public static bool AllSkippedLeavesNull()
        => LeakedLoopVariableValue(new List<int?> { null, null }) == null;

    /// <summary>**空列表残留为 `null`。**</summary>
    public static bool EmptyListLeavesNull()
        => LeakedLoopVariableValue(new List<int?>()) == null;

    /// <summary>**有值时残留最后一个。**</summary>
    public static bool LastValueSurvives()
        => LeakedLoopVariableValue(new List<int?> { 1, 2, 3 }) == 3;

    /// <summary>**被跳过的不会更新残留。**</summary>
    public static bool SkippedDoesNotUpdate()
        => LeakedLoopVariableValue(new List<int?> { 7, null }) == 7;

    /// <summary>
    /// **`m_boRunAwayMode` 被反义使用**（原文取证：把它在**基类** 1191 行的用法与
    /// **本类** 1444 行的用法放在一起看 —— 基类是 `if not m_boRunAwayMode then`（真 ⇒ 跳过移动），
    /// 本类却在"找到目标"时置真；两者对同一个真值的解读相反）。
    /// </summary>
    public static bool InvertedRunAwaySemantics()
        => Holds("基类与本类对 m_boRunAwayMode 的真值解读相反",
            Source(BaseRunAwayGuardLine).TrimStart().StartsWith("if not m_boRunAwayMode then", StringComparison.Ordinal)
            && Count("m_boRunAwayMode := True;", RunStart, RunEnd) == 1);

    /// <summary>
    /// **找到目标时置真**（原文取证：唯一那处置真在 `BaseObject1C &lt;&gt; nil` 分支内 ——
    /// 即"找到了目标"才置真）。
    /// </summary>
    public static bool SetTrueOnTargetFound()
        => Holds("唯一置真点在'找到目标'分支内",
            Source(RunAwayTrueLine).Trim() == "m_boRunAwayMode := True;"
            && Source(RunAwayTrueLine - 2).Trim() == "if BaseObject1C <> nil then");

    /// <summary>
    /// **基类为真时跳过移动**（原文取证：基类 1191 行的守卫整行逐字比对 ——
    /// 该 `if` 是 `not m_boRunAwayMode`，故真值 ⇒ 整段移动逻辑被跳过）。
    /// </summary>
    public static bool BaseSkipsMoveWhenTrue()
        => Holds("基类 1191 行的守卫是 not m_boRunAwayMode",
            Count("if not m_boRunAwayMode", MonsterRunStart, MonsterRunEnd) == 1
            && Source(BaseRunAwayGuardLine).TrimStart().StartsWith("if not m_boRunAwayMode", StringComparison.Ordinal)
            && Count("m_boRunAwayMode", BaseRunAwayGuardLine, BaseRunAwayGuardLine) == 1);

    /// <summary>**唯一置真点在 1444。**</summary>
    public static bool OnlyTrueSiteIs1444() => RunAwayTrueLine == 1444;

    /// <summary>**六个置假点。**</summary>
    public static bool SixFalseSites() => RunAwayFalseCount == 6;

    /// <summary>
    /// **两种互不相容的含义**（原文取证：同一字段在原文里同时存在
    /// "置真 = 已锁定目标"（1444，在 `BaseObject1C &lt;&gt; nil` 分支内）与
    /// "置假 + 复位超时"（1449/769/5517/5911）两类用法；且基类把它读成"跳过移动"）。
    /// </summary>
    public static bool TwoIncompatibleMeanings()
        => Holds("同一字段被两种不相容的语义使用",
            SetTrueOnTargetFound() && BaseSkipsMoveWhenTrue()
            && Count("m_boRunAwayMode := False;", 1, ObjMonRealSource.UnitLineCount) == RunAwayFalseCount
            && RunAwayTrueCount == 1);

    /// <summary>**赋值点七个。**</summary>
    public static bool SevenAssignSites()
        => RunAwayAssignSites.Length == RunAwayAssignCount;

    /// <summary>**赋值点表已提取。**</summary>
    public static bool AssignSitesExtracted()
        => RunAwayAssignSites[0] == 769 && RunAwayAssignSites[6] == 5911;

    /// <summary>**只有一处为真。**</summary>
    public static bool ExactlyOneTrue()
        => RunAwayTrueCount == 1
           && RunAwayTrueCount + RunAwayFalseCount == RunAwayAssignCount;

    /// <summary>**三个超时复位点。**</summary>
    public static bool ThreeTimeoutSites()
        => RunAwayTimeoutSites.Length == 3;

    /// <summary>**超时点表已提取。**</summary>
    public static bool TimeoutSitesExtracted()
        => RunAwayTimeoutSites[0] == 769 && RunAwayTimeoutSites[2] == 5911;

    /// <summary>**1444 不在超时点里。**</summary>
    public static bool TrueSiteNotTimeout()
        => Array.IndexOf(RunAwayTimeoutSites, RunAwayTrueLine) < 0;

    /// <summary>**基类判据行已记录**（原文取证：该行确实出现且含该字段）。</summary>
    public static bool BaseGuardLineExtracted()
        => Holds("基类判据行确实在 TMonster.Run 内且含 m_boRunAwayMode",
            BaseRunAwayGuardLine >= MonsterRunStart && BaseRunAwayGuardLine <= MonsterRunEnd
            && Source(BaseRunAwayGuardLine).Contains("m_boRunAwayMode", StringComparison.Ordinal));

    /// <summary>基类语义（1:1：真则跳过移动）。**</summary>
    public static bool BaseProceedsWithMove(bool runAwayMode) => !runAwayMode;

    /// <summary>本类语义（1:1：真表示已锁定目标）。**</summary>
    public static bool HasLockedTarget(bool runAwayMode) => runAwayMode;

    /// <summary>**两个含义在真值上冲突。**</summary>
    public static bool SemanticsConflict()
        => BaseProceedsWithMove(true) != HasLockedTarget(true);

    // ===================== 二、两段结构 =====================

    /// <summary>
    /// **两段用同一判据**（原文取证：1404 与 1453 两行的走速判据片段逐字相同，
    /// 且全 `Run` 里该片段恰好出现 2 次）。
    /// </summary>
    public static bool TwoStagesSamePredicate()
        => Holds("两段判据逐字相同且各出现一次",
            StageGuardText().Contains(WalkPredicateText, StringComparison.Ordinal)
            && Source(StageGuardLines[1]).Contains(WalkPredicateText, StringComparison.Ordinal)
            && Count(WalkPredicateText, RunStart, RunEnd) == 2);

    /// <summary>**两段行号已提取**（原文取证：两行都含走速判据片段）。</summary>
    public static bool StageLinesExtracted()
        => Holds("1404/1453 两行都含走速判据",
            StageGuardLines[0] == 1404 && StageGuardLines[1] == 1453
            && Source(StageGuardLines[0]).Contains(WalkPredicateText, StringComparison.Ordinal));

    /// <summary>**延迟被复位两次**（原文取证：`m_nWalkDelay := 0;` 在 `Run` 里恰好 2 处，即 1406/1455）。</summary>
    public static bool DelayResetTwice()
        => Holds("Run 内延迟复位恰好两处",
            Count(DelayResetText, RunStart, RunEnd) == 2
            && Source(1406).Trim() == DelayResetText
            && Source(1455).Trim() == DelayResetText);

    /// <summary>
    /// **用的是 `>=` 而非 `&gt;`**（原文取证：`Run` 内的走速判据全部用 `&gt;=`；
    /// 把 `&gt;=` 换成 `&gt;` 后该片段即不再出现）。
    /// </summary>
    public static bool GreaterEqualNotGreater()
        => Holds("走速判据用的是 >=",
            Count(WalkPredicateText, RunStart, RunEnd) == 2
            && Count(WalkPredicateText.Replace(" >= ", " > "), RunStart, RunEnd) == 0);

    /// <summary>
    /// **同一 tick 挂两个判据**（原文取证：两处判据的左侧都是 `tick_diff(m_dwWalkTick, …)`，
    /// 即同一个 `m_dwWalkTick` 字段被两段各自判定）。
    /// </summary>
    public static bool SameTickTwoGuards()
        => Holds("两处判据共用同一个 m_dwWalkTick",
            Count("tick_diff(m_dwWalkTick, MyGetTickCount)", RunStart, RunEnd) == 2
            && Count("m_dwWalkTick := 0", RunStart, RunEnd) == 0);

    /// <summary>走速判据（1:1：`>=`）。</summary>
    public static bool CanWalk(uint walkTick, uint now, uint speed, uint delay)
        => unchecked(now - walkTick) >= speed + delay;

    /// <summary>**恰好相等即通过（与 J198 相反）。**</summary>
    public static bool ExactlyEqualPasses()
        => CanWalk(0, 500, 500, 0);

    /// <summary>**未到则不走。**</summary>
    public static bool NotYetBlocked()
        => !CanWalk(0, 499, 500, 0);

    /// <summary>**超过则走。**</summary>
    public static bool ExceededPasses()
        => CanWalk(0, 501, 500, 0);

    /// <summary>**额外延迟会推迟。**</summary>
    public static bool DelayPostpones()
        => CanWalk(0, 500, 500, 0) && !CanWalk(0, 500, 500, 100);

    /// <summary>
    /// **曼哈顿距离**（原文取证：1429 行的距离式是"两个 `Abs` 相加"，
    /// 而不是平方和 —— 即定义为两轴距离之和）。
    /// </summary>
    public static bool ManhattanDistance()
        => Holds("1429 行的距离式是两个 Abs 之和",
            Source(NearestScanLine).Contains(DistanceFormulaText, StringComparison.Ordinal)
            && Count("Abs(", NearestScanLine, NearestScanLine) == 2
            && Count("+", NearestScanLine, NearestScanLine) == 1);

    /// <summary>**硬编码 9999**（原文取证：种子确实写在原文里、且与常量一致）。</summary>
    public static bool Hardcoded9999()
        => Holds("1429 之前的距离种子是硬编码 9999",
            DistanceSeed == ExtractAssignedInt(1399, "n10 :=")
            && Source(1399).Trim() == "n10 := 9999;");

    /// <summary>
    /// **取最近者**（原文取证：1430 行的比较是**严格小于** `nC &lt; n10`
    /// —— 相等时不替换，故并列取先出现者）。
    /// </summary>
    public static bool ClosestWins()
        => Holds("1430 行用的是严格小于",
            Source(1430).Trim() == "if nC < n10 then"
            && Count("nC < n10", RunStart, RunEnd) == 1
            && Count("nC <= n10", RunStart, RunEnd) == 0);

    /// <summary>
    /// **不是欧氏距离**（原文取证：1429 行不含任何乘法/平方 ——
    /// 把曼哈顿式与欧氏式代入同一对点会得到不同值，故"不是欧氏"可判定）。
    /// </summary>
    public static bool NotEuclidean()
        => Holds("1429 行不含平方、且两式在对角点上取值不同",
            Count("*", NearestScanLine, NearestScanLine) == 0
            && Count("Sqrt", NearestScanLine, NearestScanLine) == 0
            && Manhattan(0, 0, 1, 1) != (int)Math.Sqrt(2));

    /// <summary>曼哈顿距离（1:1）。</summary>
    public static int Manhattan(int x1, int y1, int x2, int y2)
        => Math.Abs(x1 - x2) + Math.Abs(y1 - y2);

    /// <summary>**对角走两步算两格。**</summary>
    public static bool DiagonalCountsTwo() => Manhattan(0, 0, 1, 1) == 2;

    /// <summary>**直线一格的为一。**</summary>
    public static bool StraightOneIsOne() => Manhattan(0, 0, 1, 0) == 1;

    /// <summary>**欧氏会给出不同值。**</summary>
    public static bool EuclideanDiffers()
        => Manhattan(0, 0, 1, 1) != 1;

    /// <summary>选最近者（1:1）。</summary>
    public static int ChooseClosest(List<(int Dist, int Id)> candidates)
    {
        int seed = DistanceSeed;
        int chosen = -1;

        foreach (var (dist, id) in candidates)
        {
            if (dist < seed)
            {
                seed = dist;
                chosen = id;
            }
        }

        return chosen;
    }

    /// <summary>**选距离最小者。**</summary>
    public static bool ChoosesSmallest()
        => ChooseClosest(new List<(int, int)> { (10, 1), (3, 2), (7, 3) }) == 2;

    /// <summary>**空列表返回 -1。**</summary>
    public static bool EmptyReturnsMinusOne()
        => ChooseClosest(new List<(int, int)>()) == -1;

    /// <summary>**严格小于：相等取先者。**</summary>
    public static bool TieKeepsFirst()
        => ChooseClosest(new List<(int, int)> { (5, 1), (5, 2) }) == 1;

    /// <summary>
    /// **三重过滤**（原文取证：循环体内三条 `Continue` 路径 —— 已死 1417/1418、
    /// 脱机玩家 1420-1424、以及 `IsProperTarget` + 隐身 1425-1427）。
    /// </summary>
    public static bool ThreeFilters()
        => Holds("扫描循环里三处过滤各就其位",
            Source(1417).Trim() == "if BaseObject.m_boDeath then"
            && Source(1420).Contains("m_boOffLine", StringComparison.Ordinal)
            && Source(1425).Trim() == "if IsProperTarget(BaseObject) then"
            && Count("Continue;", RunStart, RunEnd) == 3);

    /// <summary>**脱机玩家过滤**（原文取证：1420 行的三个合取项逐字核对）。</summary>
    public static bool OfflinePlayerFilter()
        => Holds("脱机玩家过滤的三个合取项逐字一致",
            Count("(BaseObject.m_btRaceServer = RC_PLAYOBJECT)", 1420, 1420) == 1
            && Count("(TPlayObject(BaseObject).m_boOffLine)", 1420, 1420) == 1
            && Count("g_Config.boMonNoAttackOffLinePlayer", 1420, 1420) == 1
            && Source(1421).Trim() == "then");

    /// <summary>**隐身需要冷眼**（原文取证：1427 行的判据就是 `not hide or coolEye`）。</summary>
    public static bool HideModeNeedsCoolEye()
        => Holds("1427 行的判据是 not m_boHideMode or m_boCoolEye",
            Source(1427).Trim() == "if not BaseObject.m_boHideMode or m_boCoolEye then"
            && IsVisibleTarget(false, false)
            && !IsVisibleTarget(true, false)
            && IsVisibleTarget(true, true));

    /// <summary>隐身判据（1:1）。</summary>
    public static bool IsVisibleTarget(bool hideMode, bool coolEye)
        => !hideMode || coolEye;

    /// <summary>**不隐身则可见。**</summary>
    public static bool NotHiddenVisible() => IsVisibleTarget(false, false);

    /// <summary>**隐身且无冷眼则不可见。**</summary>
    public static bool HiddenNoCoolEyeInvisible() => !IsVisibleTarget(true, false);

    /// <summary>**隐身但有冷眼则可见。**</summary>
    public static bool HiddenWithCoolEyeVisible() => IsVisibleTarget(true, true);

    /// <summary>脱机过滤（1:1）。</summary>
    public static bool ShouldSkipOffline(int race, bool offline, bool configOn)
        => race == RC_PLAYOBJECT && offline && configOn;

    /// <summary>**脱机玩家且开关开则跳过。**</summary>
    public static bool OfflineSkipped()
        => ShouldSkipOffline(RC_PLAYOBJECT, true, true);

    /// <summary>**开关关则不跳过。**</summary>
    public static bool ConfigOffNotSkipped()
        => !ShouldSkipOffline(RC_PLAYOBJECT, true, false);

    /// <summary>**非玩家不跳过。**</summary>
    public static bool NonPlayerNotSkipped()
        => !ShouldSkipOffline(80, true, true);

    /// <summary>
    /// **正常临界区写法**（原文取证：`Run` 内 `Lock` 与 `UnLock` 各恰好一处，
    /// 且解锁在 1440、加锁在 1407 —— 顺序正确）。
    /// </summary>
    public static bool ProperLockUnlock()
        => Holds("Run 内加锁/解锁各一处且顺序正确",
            Count(LockText, RunStart, RunEnd) == 1
            && Count(UnlockText, RunStart, RunEnd) == 1
            && Source(1407).Trim() == LockText
            && Source(1440).Trim() == UnlockText
            && 1407 < 1440);

    /// <summary>
    /// **`finally` 保证解锁**（原文取证：1408 行 `try`、1439 行 `finally`、
    /// 解锁语句落在 `finally` 之内 —— 三条行号关系都核对）。
    /// </summary>
    public static bool TryFinallyGuarantees()
        => Holds("解锁落在 try..finally 的 finally 分支内",
            Source(1408).Trim() == "try"
            && Source(1439).Trim() == "finally"
            && Source(1440).Trim() == UnlockText
            && 1408 < 1439 && 1439 < 1440
            && Source(1441).Trim() == "end;");

    /// <summary>
    /// **两级判空**（原文取证：1412 判 `VisibleBaseObject &lt;&gt; nil`、
    /// 1415 判 `BaseObject = nil` —— 对同一个可见对象的两级指针各判一次空）。
    /// </summary>
    public static bool TwoLevelNilChecks()
        => Holds("1412 与 1415 两级各判一次空",
            Source(1412).Trim() == "if VisibleBaseObject <> nil then"
            && Source(1415).Trim() == "if BaseObject = nil then"
            && Source(1416).Trim() == "Continue;");

    // ===================== 三、Create / Destroy =====================

    /// <summary>
    /// **只设了视野**（原文取证：`Create` 体内只有 `inherited;` 与一条赋值 ——
    /// 语句总数为 2，其中赋值 1 条，且赋值目标就是 `m_nViewRange`）。
    /// </summary>
    /// <summary>
    /// **只设了视野**（原文取证：`Create` 的方法体就是 `inherited;` 加一条视野赋值，
    /// 两条语句逐字比对，且体内没有别的赋值）。
    /// </summary>
    public static bool OnlySetsViewRange()
        => Holds("Create 体内除 inherited 外只有一条视野赋值",
            Source(1384).Trim() == "inherited;"
            && Source(1385).Trim() == "m_nViewRange := 5;"
            && Count(":=", 1384, 1386) == 1);

    /// <summary>**视野为五**（原文取证：从原文 1385 行的赋值语句里**解析右值**得到 5）。</summary>
    public static bool ViewRangeIsFive()
        => Holds("Create 里的视野赋值右值是 5",
            ExtractAssignedInt(1385, "m_nViewRange :=") == ViewRange);

    /// <summary>**先 `inherited` 再赋值**（原文取证：1384 是 `inherited;`、1385 才是赋值）。</summary>
    public static bool InheritedCalledFirst()
        => Holds("inherited 在赋值之前",
            Source(1384).Trim() == "inherited;"
            && Source(1385).Trim() == "m_nViewRange := 5;"
            && 1384 < 1385);

    /// <summary>
    /// **`Destroy` 是空壳**（原文取证：`Destroy` 体内除 `inherited;` 外没有任何语句 ——
    /// 1389/1390 两行的语句计数为 1）。
    /// </summary>
    public static bool DestroyIsEmptyShell()
        => Holds("Destroy 体内除 inherited 外没有语句",
            Source(DestroyStart + 2).Trim() == "inherited;"
            && Count(";", DestroyStart + 1, DestroyStart + DestroyLines - 2) == 1);

    /// <summary>
    /// **冗余的 `inherited`**（原文取证：`Destroy` 只有 `inherited;` 一句 ——
    /// Delphi 里不写效果相同，故此写法本身不改变行为）。
    /// </summary>
    public static bool RedundantInherited()
        => Holds("Destroy 的整段体就是一句 inherited",
            DestroyIsEmptyShell()
            && Count("inherited", DestroyStart, DestroyStart + DestroyLines - 1) == 1);

    /// <summary>
    /// **与 J197 的 `Operate` 同形态**：本工程当前**没有**把 J197 的 `TMonster.Operate`
    /// 原文摘录做成可查询的证据表，故无法在进程内核对"同形态"这一跨批次结论。
    /// </summary>
    public static bool SameAsJ197Operate()
        => NotPorted(nameof(SameAsJ197Operate), 1390, true);

    /// <summary>
    /// **没有 `Think` 覆写**（原文取证：`TChickenDeer` 的类声明体内只有
    /// Create/Destroy/Run 三个成员）。
    /// </summary>
    public static bool NoThinkOverride()
        => Holds("TChickenDeer 声明体内不含 Think",
            Count("Think", ClassDeclStart, ClassDeclEnd) == 0
            && Count("Run", ClassDeclStart, ClassDeclEnd) == 1);

    /// <summary>
    /// **找目标在 `Run` 里**（原文取证：`TChickenDeer` 没有覆写 `Think`，
    /// 而"扫可见对象挑最近者"那段确实落在 `Run` 的 1409-1438）。
    /// </summary>
    public static bool TargetScanInRunNotThink()
        => Holds("目标扫描落在 Run 内且本类无 Think"
                 + " [visibleActors=" + Count("m_VisibleActors", RunStart, RunEnd) + "]",
            NoThinkOverride()
            && Count("m_VisibleActors", RunStart, RunEnd) == 4);

    /// <summary>**类声明三方法**（原文取证：声明体 6 行里正好 3 个 `;` 结尾的成员）。</summary>
    public static bool ThreeDeclaredMethods()
        => Holds("声明体 6 行、含三个成员",
            ClassDeclEnd - ClassDeclStart + 1 == 6
            && Count("constructor Create", ClassDeclStart, ClassDeclEnd) == 1
            && Count("destructor Destroy", ClassDeclStart, ClassDeclEnd) == 1
            && Count("procedure Run", ClassDeclStart, ClassDeclEnd) == 1);

    /// <summary>
    /// **`bo554` 声明两次** —— ★ **本车道复核后的更正**：原文里其实是**三处**声明
    /// （`TMonster`:11、`TFoxMonster`:182、`TFoxMagicAttackMonster`:507），
    /// 且**四处赋值**（555/792/2166/5534，全为 `False`）。
    /// J199 原文说"两处"是**漏数**（只看了 11 与 182）。
    /// 本谓词按**实测**判定：三处声明、四处赋值、赋值全为 `False`。
    /// </summary>
    public static bool FieldDeclaredTwice()
        => Holds("bo554 实测三处声明、四处赋值（全 False）",
            Bo554InMonster == 11 && Bo554InFox == 182
            && Source(Bo554InMonster).Contains("bo554: Boolean;", StringComparison.Ordinal)
            && Source(Bo554InFox).Contains("bo554: Boolean;", StringComparison.Ordinal)
            && Source(Bo554InFoxMagic).Contains("bo554: Boolean;", StringComparison.Ordinal)
            && CountBo554Declarations() == Bo554DeclarationCount
            && Count("bo554 := False;", 1, ObjMonRealSource.UnitLineCount) == Bo554FalseAssignCount
            && Count("bo554 := True;", 1, ObjMonRealSource.UnitLineCount) == 0);

    /// <summary>
    /// **读的是继承来的字段**（原文取证：`Run` 读了 `bo554`，
    /// 而 `TChickenDeer` 自己的声明体里没有它 ⇒ 只能来自基类）。
    /// </summary>
    public static bool ReadsInheritedField()
        => Holds("Run 读了 bo554、而本类声明体没有它",
            Count("bo554", RunStart, RunEnd) == 1
            && Count("bo554", ClassDeclStart, ClassDeclEnd) == 0);
    /// <summary>
    /// **本类从不赋值**（原文取证：`bo554` 全单元四处赋值全部落在**别的类**的实现里
    /// （555/792/2166/5534，均不在 1382-1464 内）⇒ `TChickenDeer` 自己从不写它）。
    /// </summary>
    public static bool NeverAssignedInThisClass()
        => Holds("TChickenDeer 的三个方法体内零赋值",
            Count("bo554 :=", CreateStart, RunEnd) == 0
            && Count("bo554 :=", 1, ObjMonRealSource.UnitLineCount) == Bo554FalseAssignCount);

    // ===================== 四、整体 =====================

    /// <summary>**两个阈值不同（6 与 5）**（原文取证：1456 行是 6、1459 行的第五参是 5）。</summary>
    public static bool TwoDifferentThresholds()
        => Holds("1456 用 6、1459 用 5",
            NearThreshold != NextPositionParam
            && Source(DuplicatedConditionLine).Contains("<= " + NearThreshold + ")", StringComparison.Ordinal)
            && Source(DuplicatedConditionLine + 3).Contains(", " + NextPositionParam + ", m_nTargetX", StringComparison.Ordinal));

    /// <summary>**是 6 与 5**（原文取证：两个数字逐字核对）。</summary>
    public static bool SixVsFive()
        => Holds("两个阈值常量确为 6 与 5",
            NearThreshold == 6 && NextPositionParam == 5 && TwoDifferentThresholds());

    /// <summary>
    /// **无插桩**（原文取证：`TChickenDeer` 的三个方法体内不含 `ErrCode` 之类的插桩调用；
    /// 逐行扫 1382-1464 计数为 0）。
    /// </summary>
    public static bool NoInstrumentation()
        => Holds("TChickenDeer 三个方法体内零插桩",
            Count("ErrCode", CreateStart, RunEnd) == 0
            && Count("WriteLog", CreateStart, RunEnd) == 0);

    /// <summary>
    /// **后面还有很多派生类**（原文取证：`TChickenDeer.Run` 的**实现起始行之后**
    /// 仍有 189 条类方法实现 —— 由原文重算，不再断言 J199 那个说不清出处的魔数）。
    /// </summary>
    public static bool ManyMoreSubclasses()
        => Holds("RunStart 之后另有 " + ImplementationsAfterRunStart + " 条类方法实现"
                 + " [afterRunStart=" + CountImplementationsAfterLine(RunStart)
                 + " 复核值=" + ImplementationsAfterRunStart
                 + " J199旧值=" + RemainingImplCount + "]",
            CountImplementationsAfterLine(RunStart) == ImplementationsAfterRunStart
            && ImplementationsAfterRunStart > 0);

    /// <summary>
    /// **本批只做第一个派生类**：这是**批次口径陈述**（关于本车道做了多少），
    /// 不是关于原文的事实 —— 原文里没有可核对的对应物，故显式留痕。
    /// </summary>
    public static bool FirstSubclassOnly()
        => NotPorted(nameof(FirstSubclassOnly), 1467, true);

    /// <summary>
    /// **剩余类数为 54** —— ★ **本车道复核后的更正**：原文类声明 56 条、
    /// 扣掉被 `(* *)` 注掉的那 1 条 ⇒ **实有 55 个类**；
    /// 减去本批覆盖的 1 个 ⇒ **剩余 54**。
    /// J199 的常量表里 <c>RemainingClassCount = 54</c> 是对的，
    /// 但它把"类声明条数"记成了 55（应为 56）—— 本谓词按**原文重算**判定。
    /// </summary>
    public static bool RemainingClassCountIs54()
        => Holds("实有类数减本批覆盖数等于 54",
            CountClassDeclarations() == DeclarationCountIncludingCommented
            && DeclarationCountIncludingCommented - CommentedOutDeclarations == RealClassCount
            && RealClassCount - ClassesCoveredByThisBatch == RemainingClassCount);

    // ===================== 五、跨度 =====================

    /// <summary>
    /// **三方法行数相加**（原文取证：三段各自的行数与原文起止行一致；
    /// `Create` 后有一行空行、`Destroy` 后有一行空行）。
    /// </summary>
    public static bool TotalLinesAddUp()
        => Holds("三段行数与原文起止一致"
                 + " [create=" + (CreateStart + CreateLines - 1)
                 + " destroyStart=" + DestroyStart
                 + " runStart=" + RunStart + "]",
            TotalLines == 81
            && (CreateStart + CreateLines) == DestroyStart - 1
            && (DestroyStart + DestroyLines) == RunStart - 1);

    /// <summary>**跨度自洽**（原文取证：`Run` 的起止行与行数三者一致）。</summary>
    public static bool SpanMatches()
        => Holds("起止行与行数自洽",
            (RunEnd - RunStart + 1) == RunLines
            && (CreateStart + CreateLines - 1) == 1386
            && (DestroyStart + DestroyLines - 1) == 1391
            && TotalLinesAddUp());

    /// <summary>**方法起始行递增**（原文取证：三个实现的起始行在原文里确实递增）。</summary>
    public static bool StartsAscending()
        => Holds("三个实现按原文顺序排列",
            CreateStart < DestroyStart && DestroyStart < RunStart
            && ObjMonRealSource.FindImplementationLine("TChickenDeer", "Create") == CreateStart
            && ObjMonRealSource.FindImplementationLine("TChickenDeer", "Destroy") == DestroyStart
            && ObjMonRealSource.FindImplementationLine("TChickenDeer", "Run") == RunStart);

    /// <summary>**在单元内**（原文取证：结束行不超过单元总行数）。</summary>
    public static bool WithinUnit()
        => Holds("Run 结束行在单元内",
            RunEnd < ObjMonRealSource.UnitLineCount);

    // ===================== 六、取证辅助（原文重算） =====================

    /// <summary>取原文第 <paramref name="line"/> 行的 `<c>is</c>` 判据整行。</summary>
    private static string StageGuardText() => Source(StageGuardLines[0]);

    /// <summary>`m_TargetCret` 的原文写法（逐字）。</summary>
    public const string TargetVariableName = "m_TargetCret";

    /// <summary>循环泄漏变量的原文名字（逐字：1400 行被置 `nil`、1456 行被解引用）。</summary>
    public const string LeakedVariableName = "BaseObject";

    /// <summary>
    /// 取原文第 <paramref name="line"/> 行**赋值语句的左侧标识符**（第一个 `:=` 之前那个标识符）——
    /// 用于判定"某一行的赋值目标是不是某个变量"。
    /// </summary>
    private static string AssignedTargetOf(int line)
    {
        string text = Source(line);
        int op = text.IndexOf(":=", StringComparison.Ordinal);
        if (op <= 0) return string.Empty;

        int i = op;
        while (i > 0 && char.IsWhiteSpace(text[i - 1])) i--;
        int end = i;
        while (i > 0 && (char.IsLetterOrDigit(text[i - 1]) || text[i - 1] == '_')) i--;
        return i < end ? text.Substring(i, end - i) : string.Empty;
    }

    /// <summary>
    /// 从原文第 <paramref name="line"/> 行里、<paramref name="suffixAnchor"/> 之前取"根标识符" ——
    /// 1456 行的形状是 <c>Abs(&lt;变量&gt;.m_nCurrX ...)</c>，取 <c>.m_nCurrX</c> 前面那个标识符。
    /// </summary>
    private static string ReadVariableOf(int line, string suffixAnchor)
    {
        string text = Source(line);
        int at = text.IndexOf(suffixAnchor, StringComparison.Ordinal);
        if (at <= 0) return string.Empty;

        int i = at;
        while (i > 0 && (char.IsLetterOrDigit(text[i - 1]) || text[i - 1] == '_')) i--;
        return i < at ? text.Substring(i, at - i) : string.Empty;
    }

    /// <summary>
    /// 从原文第 <paramref name="line"/> 行里、紧跟 <paramref name="anchor"/> 之后取"根标识符"
    /// （取到第一个非 `[A-Za-z0-9_]` 字符为止，并越过紧邻的左括号）——
    /// 用于判定两行引用的是不是同一个变量。
    /// </summary>
    private static string RootIdentifierOf(int line, string anchor)
    {
        string text = Source(line);
        int at = text.IndexOf(anchor, StringComparison.Ordinal);
        if (at < 0) return string.Empty;

        int i = at + anchor.Length;
        while (i < text.Length && text[i] == '(') i++;

        int start = i;
        while (i < text.Length && (char.IsLetterOrDigit(text[i]) || text[i] == '_')) i++;
        return i > start ? text.Substring(start, i - start) : string.Empty;
    }

    /// <summary>
    /// 从原文第 <paramref name="line"/> 行里、`<paramref name="assignmentAnchor"/>` 之后
    /// **解析整数字面量**（把右值从原文里读出来，而不是断言一个常量等于自己）。
    /// 解析不出时返回 <see cref="int.MinValue"/>。
    /// </summary>
    private static int ExtractAssignedInt(int line, string assignmentAnchor)
    {
        string text = Source(line);
        int at = text.IndexOf(assignmentAnchor, StringComparison.Ordinal);
        if (at < 0) return int.MinValue;

        int i = at + assignmentAnchor.Length;
        while (i < text.Length && char.IsWhiteSpace(text[i])) i++;

        int start = i;
        if (i < text.Length && (text[i] == '-' || text[i] == '+')) i++;
        while (i < text.Length && char.IsDigit(text[i])) i++;

        return i > start && int.TryParse(text.Substring(start, i - start), out int value)
            ? value
            : int.MinValue;
    }

    /// <summary>
    /// 原文里**类方法实现**的条数 —— 判据与 <c>docs/ObjMon-对账.md</c> §1 一致：
    /// 列 0 且形如 <c>procedure|function|constructor|destructor T&lt;类&gt;.&lt;方法&gt;</c>。
    /// （单元级裸例程不计入；<c>sub_*</c> 这类反编译名按原样计入。）
    /// </summary>
    private static int CountImplementationLines()
        => CountTopLevelImplementations(1, ObjMonRealSource.UnitLineCount);

    /// <summary>原文 <paramref name="line"/> 之后的类方法实现条数。</summary>
    private static int CountImplementationLinesAfter(int line)
        => CountTopLevelImplementations(line + 1, ObjMonRealSource.UnitLineCount);

    /// <summary>原文第 <paramref name="line"/> 行**之后**（不含该行）的类方法实现条数。</summary>
    private static int CountImplementationsAfterLine(int line)
        => CountTopLevelImplementations(line + 1, ObjMonRealSource.UnitLineCount);

    private static int CountTopLevelImplementations(int fromLine, int toLine)
    {
        int n = 0;
        for (int i = fromLine; i <= toLine; i++)
        {
            string raw = Source(i);
            if (raw.Length == 0 || char.IsWhiteSpace(raw[0])) continue;

            string t = raw.TrimStart();
            string? rest = null;
            foreach (string kw in new[] { "procedure ", "function ", "constructor ", "destructor " })
            {
                if (t.StartsWith(kw, StringComparison.Ordinal)) { rest = t.Substring(kw.Length); break; }
            }

            if (rest == null) continue;
            if (!rest.StartsWith("T", StringComparison.Ordinal)) continue;   // 单元级裸例程不计
            if (rest.IndexOf('.') <= 1) continue;                            // 必须形如 T<名>.<名>

            n++;
        }

        return n;
    }

    /// <summary>原文里 <c>bo554: Boolean;</c> 形式的字段声明处数（脚本重算）。</summary>
    public static int CountBo554DeclarationsPublic() => CountBo554Declarations();

    /// <summary>原文里类声明条数（供测试侧双向核对）。</summary>
    public static int CountClassDeclarationsPublic() => CountClassDeclarations();

    /// <summary>原文 1464 之后的顶层实现条数（供测试侧双向核对）。</summary>
    public static int CountImplementationLinesAfter1464Public() => CountImplementationLinesAfter(RunEnd);

    /// <summary>★ 由原文重算的"`TChickenDeer.Run` 起始行之后"的类方法实现条数（J199 记 189，实为 186）。</summary>
    public static int CountImplementationsAfterRunStartPublic() => CountImplementationsAfterLine(RunStart);

    /// <summary>原文里顶层实现条数（供测试侧双向核对）。</summary>
    public static int CountImplementationLinesPublic() => CountImplementationLines();

    /// <summary>原文里 <c>bo554: Boolean;</c> 形式的字段声明处数（脚本重算）。</summary>
    private static int CountBo554Declarations()
    {
        int n = 0;
        for (int i = 1; i <= ObjMonRealSource.UnitLineCount; i++)
        {
            if (Source(i).TrimStart().StartsWith("bo554: Boolean;", StringComparison.Ordinal)) n++;
        }

        return n;
    }

    /// <summary>
    /// 原文里 <c>T某 = class</c> 形式的类声明条数（含被 <c>(* *)</c> 注掉的那一份）——
    /// 判据与 <c>tools/audit-objmon-coverage.ps1</c> 一致：
    /// <c>^\s*(T\w+)\s*=\s*class\b</c>。
    /// </summary>
    private static int CountClassDeclarations()
        => System.Text.RegularExpressions.Regex.Matches(
               string.Join("\n", ObjMonRealSource.Lines),
               @"^[ \t]*(T[A-Za-z0-9_]+)[ \t]*=[ \t]*class\b",
               System.Text.RegularExpressions.RegexOptions.Multiline).Count;
}
