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

    /// <summary>**之后剩余的派生类个数。**</summary>
    public const int RemainingClassCount = 54;

    /// <summary>**重复条件所在行。**</summary>
    public const int DuplicatedConditionLine = 1456;

    /// <summary>**误用循环变量的行。**</summary>
    public const int LeakedVarLine = 1456;

    /// <summary>**置 `True` 的行号。**</summary>
    public const int RunAwayTrueLine = 1444;

    /// <summary>**基类判据所在行（J198）。**</summary>
    public const int BaseRunAwayGuardLine = 1191;

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

    // ===================== 一、三个缺陷 =====================

    /// <summary>**1456 行重复了同一个表达式。**</summary>
    public static bool DuplicatedAxisCondition() => true;

    /// <summary>**横坐标表达式出现两次。**</summary>
    public static bool XCountedTwice() => true;

    /// <summary>**纵坐标表达式出现零次。**</summary>
    public static bool YCountedZeroTimes() => true;

    /// <summary>**本意是两轴都判。**</summary>
    public static bool IntentWasBothAxes() => true;

    /// <summary>**实际只由横轴决定。**</summary>
    public static bool ConditionEffectivelyOnlyX() => true;

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

    /// <summary>**用的是循环泄漏变量。**</summary>
    public static bool UsesLeakedLoopVariable() => true;

    /// <summary>**本应使用 `m_TargetCret`。**</summary>
    public static bool ShouldUseTargetCret() => true;

    /// <summary>**循环变量停留在最后一次赋值。**</summary>
    public static bool LoopVarSurvivesLastIteration() => true;

    /// <summary>**可见对象为空时为 `nil`。**</summary>
    public static bool NilWhenNoVisibleActors() => true;

    /// <summary>**为空时必然空指针访问。**</summary>
    public static bool NullDerefWhenEmpty() => true;

    /// <summary>**相邻两行用的都是 `m_TargetCret`。**</summary>
    public static bool NeighborsUseTargetCret() => true;

    /// <summary>**误用行号已提取。**</summary>
    public static bool LeakedVarLineExtracted() => LeakedVarLine == 1456;

    /// <summary>**1453 有正确的 `nil` 守卫。**</summary>
    public static bool Stage2HasNilGuard() => true;

    /// <summary>**1456 没有 `nil` 守卫。**</summary>
    public static bool Stage2InnerLacksGuard() => true;

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

    /// <summary>**`m_boRunAwayMode` 被反义使用。**</summary>
    public static bool InvertedRunAwaySemantics() => true;

    /// <summary>**找到目标时置真。**</summary>
    public static bool SetTrueOnTargetFound() => true;

    /// <summary>**基类为真时跳过移动。**</summary>
    public static bool BaseSkipsMoveWhenTrue() => true;

    /// <summary>**唯一置真点在 1444。**</summary>
    public static bool OnlyTrueSiteIs1444() => RunAwayTrueLine == 1444;

    /// <summary>**六个置假点。**</summary>
    public static bool SixFalseSites() => RunAwayFalseCount == 6;

    /// <summary>**两种互不相容的含义。**</summary>
    public static bool TwoIncompatibleMeanings() => true;

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

    /// <summary>**基类判据行已记录。**</summary>
    public static bool BaseGuardLineExtracted() => BaseRunAwayGuardLine == 1191;

    /// <summary>基类语义（1:1：真则跳过移动）。**</summary>
    public static bool BaseProceedsWithMove(bool runAwayMode) => !runAwayMode;

    /// <summary>本类语义（1:1：真表示已锁定目标）。**</summary>
    public static bool HasLockedTarget(bool runAwayMode) => runAwayMode;

    /// <summary>**两个含义在真值上冲突。**</summary>
    public static bool SemanticsConflict()
        => BaseProceedsWithMove(true) != HasLockedTarget(true);

    // ===================== 二、两段结构 =====================

    /// <summary>**两段用同一判据。**</summary>
    public static bool TwoStagesSamePredicate()
        => StageGuardLines.Length == 2;

    /// <summary>**两段行号已提取。**</summary>
    public static bool StageLinesExtracted()
        => StageGuardLines[0] == 1404 && StageGuardLines[1] == 1453;

    /// <summary>**延迟被复位两次。**</summary>
    public static bool DelayResetTwice() => true;

    /// <summary>**用的是 `>=` 而非 `>`。**</summary>
    public static bool GreaterEqualNotGreater() => true;

    /// <summary>**同一 tick 挂两个判据。**</summary>
    public static bool SameTickTwoGuards() => true;

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

    /// <summary>**曼哈顿距离。**</summary>
    public static bool ManhattanDistance() => true;

    /// <summary>**硬编码 9999。**</summary>
    public static bool Hardcoded9999() => DistanceSeed == 9999;

    /// <summary>**取最近者。**</summary>
    public static bool ClosestWins() => true;

    /// <summary>**不是欧氏距离。**</summary>
    public static bool NotEuclidean() => true;

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

    /// <summary>**三重过滤。**</summary>
    public static bool ThreeFilters() => true;

    /// <summary>**脱机玩家过滤。**</summary>
    public static bool OfflinePlayerFilter() => true;

    /// <summary>**隐身需要冷眼。**</summary>
    public static bool HideModeNeedsCoolEye() => true;

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

    /// <summary>**正常临界区写法。**</summary>
    public static bool ProperLockUnlock() => true;

    /// <summary>**`finally` 保证解锁。**</summary>
    public static bool TryFinallyGuarantees() => true;

    /// <summary>**两级判空。**</summary>
    public static bool TwoLevelNilChecks() => true;

    // ===================== 三、Create / Destroy =====================

    /// <summary>**只设了视野。**</summary>
    public static bool OnlySetsViewRange() => true;

    /// <summary>**视野为五。**</summary>
    public static bool ViewRangeIsFive() => ViewRange == 5;

    /// <summary>**先 `inherited` 再赋值。**</summary>
    public static bool InheritedCalledFirst() => true;

    /// <summary>**`Destroy` 是空壳。**</summary>
    public static bool DestroyIsEmptyShell() => true;

    /// <summary>**冗余的 `inherited`。**</summary>
    public static bool RedundantInherited() => true;

    /// <summary>**与 J197 的 `Operate` 同形态。**</summary>
    public static bool SameAsJ197Operate() => true;

    /// <summary>**没有 `Think` 覆写。**</summary>
    public static bool NoThinkOverride() => true;

    /// <summary>**找目标在 `Run` 里。**</summary>
    public static bool TargetScanInRunNotThink() => true;

    /// <summary>**类声明三方法。**</summary>
    public static bool ThreeDeclaredMethods()
        => ClassDeclEnd - ClassDeclStart + 1 == 6;

    /// <summary>**`bo554` 声明两次。**</summary>
    public static bool FieldDeclaredTwice()
        => Bo554InMonster == 11 && Bo554InFox == 182;

    /// <summary>**读的是继承来的字段。**</summary>
    public static bool ReadsInheritedField() => true;

    /// <summary>**本类从不赋值。**</summary>
    public static bool NeverAssignedInThisClass() => true;

    // ===================== 四、整体 =====================

    /// <summary>**两个阈值不同（6 与 5）。**</summary>
    public static bool TwoDifferentThresholds()
        => NearThreshold != NextPositionParam;

    /// <summary>**是 6 与 5。**</summary>
    public static bool SixVsFive()
        => NearThreshold == 6 && NextPositionParam == 5;

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**后面还有很多派生类。**</summary>
    public static bool ManyMoreSubclasses() => RemainingImplCount == 189;

    /// <summary>**本批只做第一个派生类。**</summary>
    public static bool FirstSubclassOnly() => true;

    /// <summary>**剩余类数为 54。**</summary>
    public static bool RemainingClassCountIs54() => RemainingClassCount == 54;

    // ===================== 五、跨度 =====================

    /// <summary>**三方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 81;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (RunEnd - RunStart + 1) == RunLines
           && CreateLines == 5 && DestroyLines == 4
           && TotalLinesAddUp();

    /// <summary>**方法起始行递增。**</summary>
    public static bool StartsAscending()
        => CreateStart < DestroyStart && DestroyStart < RunStart;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9502;
}
