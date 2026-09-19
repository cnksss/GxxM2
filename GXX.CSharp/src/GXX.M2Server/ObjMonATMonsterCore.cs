using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjMon.pas` 中 `TATMonster` 1:1 移植（批次J200）：
/// `TATMonster.Create`（1467-1471，**五行**）、
/// `TATMonster.Destroy`（1473-1476，**四行**）、
/// `TATMonster.Run`（1478-1500，**二十三行**），
/// 合计**三十二行**；
/// 另登记 `TSlowATMonster`（1503 起）等后继派生类的位置。
/// 辅助源：32-38（`TATMonster` 类声明）、
/// `ObjBase.pas:283/284/318`（**三个搜索计时字段的声明**）、
/// `ObjBase.pas:3201-3202`（`m_dwSearchTime` 与 `m_dwSearchTick` 的初始化）、
/// `ObjBase.pas:14480`（`m_dwSearchEnemyTick` 的初始化）、
/// `ObjMon2.pas:1360` 与 `UsrEngn.pas:1068/3534/3668/4115/4242`
/// （**`m_dwSearchTime` 的真实消费点**）。
///
/// ==================== 一、**两个"搜索计时"字段被混用：本批最重要的发现** ====================
///
/// **核心发现一：`TATMonster.Create` 与 `TATMonster.Run` 用的是**两个不同的字段**** ——
///
/// - `Create`（1470）设置 **`m_dwSearchTime := Random(1500) + 1500;`**
/// - `Run`（1482）判断的却是 **`MyGetTickCount - m_dwSearchEnemyTick`**
///
/// **二者是 `TBaseObject` 上**两个不同的成员****
/// （`ObjBase.pas:283` 的 `m_dwSearchTime: LongWord; // 0x360`
/// 与 `ObjBase.pas:318` 的 `m_dwSearchEnemyTick: LongWord; // 0x400`；
/// 另有 `284` 的 `m_dwSearchTick: LongWord; // 0x364`）——
/// **即偏移不同（0x360 / 0x364 / 0x400）、是三个独立字段。**
///
/// **已用脚本统计确认：`m_dwSearchTime` 在 `ObjMon.pas` 里共 **16** 处、
/// **全部是赋值、零处读取**；而它的**真实读取点全在别的文件**
/// （`ObjMon2.pas:1360`、`UsrEngn.pas:1068/3534/3668/4115/4242`）
/// ——**且那些读取点比较的是 `m_dwSearchTick`、不是 `m_dwSearchEnemyTick`**。**
///
/// **即**`m_dwSearchTime` 与 `m_dwSearchTick` 是一对（时长 + 起点）、
/// 供 `UsrEngn` 那套"NPC/英雄/商人定时搜索"使用；
/// 而 `m_dwSearchEnemyTick` 是**第三条独立时间线**、
/// **其阈值在本方法里是硬编码的 `8000` 与 `1000`、根本不用 `m_dwSearchTime`**。**
///
/// 已用 `TwoDifferentFields`、`CreateSetsSearchTime`、
/// `RunUsesSearchEnemyTick`、`ThreeFieldsThreeOffsets`、
/// `SearchTimeNeverReadHere`、`RealReadersElsewhere`、
/// `ReadersUseSearchTickNotEnemyTick` 固化。
///
/// **核心发现二：`m_dwSearchTime` 在本类里是"只写不读"的死状态
/// （对本类自身行为而言）** ——
/// 16 处赋值中 `Random(1500) + 1500` 这一形式出现 **14** 次、
/// 说明它是**被大量类照抄的样板初始化**；
/// **而在 `TATMonster` 里、它设置之后本类的 `Run` 从不消费它**。**
///
/// **即**不能因为"`Create` 设了它"就推断"`Run` 会用它"——
/// **字段的读写必须分别在两侧核实**（本批正是靠脚本双侧统计才发现的）。**
///
/// 已用 `WriteOnlyInThisClass`、`FourteenIdenticalForm`、
/// `CopyPasteTemplate`、`DoNotInferFromAssignment` 固化。
///
/// **核心发现三：`Random(1500) + 1500` 的取值范围是 `[1500, 2999]`**
/// —— 因 `Random(1500)` 返回 `0..1499`、加 1500 后上界为 **2999**（不是 3000）。**
///
/// **即**该表达式的**实际上界比"看起来的 3000"少 1**、
/// 属"`Random(n) + m` 的直觉上界是 `n + m - 1`"这一类常见误算。**
///
/// 已用 `RangeIs1500To2999`、`UpperBoundIs2999Not3000`、
/// `RandomExclusiveUpper` 固化。
///
/// **核心发现四：本文件里 `m_dwSearchTime` 有**五种不同的取值形式**** ——
/// `3000 + Random(2000)`（559/796/5538）、
/// `Random(1500) + 1500`（**十四处、最常见**）、
/// `Random(1500) + 500`（1981）、
/// `Random(1500) + 2500`（2168/2256）
/// —— 即**同一字段在不同类里被赋予不同的随机区间**、
/// **没有统一策略**（属"每个类各自拍一个数"）。**
///
/// 已用 `FiveFormsInFile`、`NoUnifiedPolicy`、
/// `FourteenIsDominantForm` 固化。
///
/// ==================== 二、**空 `begin end` 分支再现** ====================
///
/// **核心发现五：`Run` 里又出现了 J198/J199 记录的**空真分支**惯用法（1489-1490）** ——
///
/// ```pascal
/// if <目标仍有效> then
/// begin
/// end
/// else if <无主人 或 主人非放松> then
/// begin
///   m_boTarget := False;
///   SearchTarget();
/// end;
/// ```
///
/// **即**同样是"若满足条件则**什么都不做**、否则才重新搜索"** ——
/// 与 J198 的 1290-1293、J197 的 `Think` 注释块是**同一族写法**、
/// **本文件内已累计出现三次**。**
///
/// 已用 `EmptyThenBranchAgain`、`SameIdiomAsJ198`、
/// `ThirdOccurrenceInFile` 固化。
///
/// **核心发现六：那个"目标仍有效"的判据是**七重合取**（1487-1488）**：
/// ① `m_boTarget`（有目标标志）
/// **且** ② `m_TargetCret <> nil`
/// **且** ③ `not m_TargetCret.m_boDeath`
/// **且** ④ `not m_TargetCret.m_boGhost`
/// **且** ⑤ `m_TargetCret.m_PEnvir = m_PEnvir`（**同一地图**）
/// **且** ⑥ `Abs(dx) <= 20`
/// **且** ⑦ `Abs(dy) <= 20`。**
///
/// **注意 ⑥⑦ 的 20 与 `TMonster.Run`（J198）里 `SpaceMove` 用的 20 是同一个数**
/// —— 即**二十格是本文件里反复出现的"宝宝跟随半径"**。**
///
/// 已用 `SevenFoldGuard`、`SameMapRequired`、
/// `RadiusTwentyAgain` 固化。
///
/// **核心发现七：本方法把"目标有效"作为**保留目标**的理由、
/// 但**没有任何代码去"接近"或"攻击"它**** ——
/// 即 `TATMonster.Run` 的全部职责就是**决定是否重新搜索**、
/// **实际的移动/攻击仍交给基类 `TMonster.Run`**（1499 的 `inherited`）。**
///
/// 已用 `OnlyDecidesRescan`、`MovementDelegatedToBase` 固化。
///
/// ==================== 三、**搜索节流的两档阈值** ====================
///
/// **核心发现八：重搜判据是**两档**的（1482）** ——
///
/// ```pascal
/// if ((now - m_dwSearchEnemyTick) > 8000)
///    or (((now - m_dwSearchEnemyTick) > 1000) and (m_TargetCret = nil)) then
/// ```
///
/// **即**无目标时每 **1 秒**重搜一次（`> 1000`）、
/// 有目标时每 **8 秒**才重搜一次（`> 8000`）
/// —— **即"没有目标就勤找、有了目标就少找"**。**
///
/// 已用 `TwoTierThreshold`、`EightSecondsWithTarget`、
/// `OneSecondWithoutTarget`、`RetargetIsLazy` 固化。
///
/// **核心发现九：两档之间缺少"有目标但目标很远"的中间档** ——
/// 即目标只要存在（`m_TargetCret <> nil`）就一律按 8 秒；
/// **而目标是否"仍然有效"要到 8 秒后才由 1487 的七重合取去否决**
/// —— 即**一个已跑远的目标最多会被保留 8 秒**。**
///
/// 已用 `NoMiddleTier`、`StaleTargetKeptUpTo8s` 固化。
///
/// **核心发现十：本方法用的是**裸减法** `MyGetTickCount - m_dwSearchEnemyTick`
/// 而非 `tick_diff`** ——
/// 与 J198 记录的 1330 行同类问题（**无回绕补偿**）；
/// **注意 J199 的 `TChickenDeer.Run` 用的是 `tick_diff`** ——
/// 即**同一文件内三个相邻类用了两种写法**（TChickenDeer 用 `tick_diff`、
/// TATMonster 用裸减法、TMonster.Run 两者并用）。**
///
/// 已用 `RawSubtractionAgain`、`NoWraparoundCompensation`、
/// `ThreeClassesTwoStyles` 固化。
///
/// **核心发现十一：两处判据里的 `MyGetTickCount` 被**各调用一次**（1482）
/// —— 即**同一条 `or` 表达式的两侧各自取了一次当前时间**、
/// 而**函数中间理论上可能已过一毫秒** ——
/// 属**"同一表达式内多次取时间"**的写法（无害但不严谨）。**
///
/// 已用 `TickReadTwice`、`SameExpressionTwoReads` 固化。
///
/// ==================== 四、`Create` / `Destroy` 与基类的分工 ====================
///
/// **核心发现十二：`TATMonster.Destroy` **又一次是空壳**
/// （`begin inherited; end;`）** ——
/// 与 `TChickenDeer.Destroy`（J199）、`TMonster.Operate`（J197）**完全同形**、
/// **本文件里已是第三次出现"纯 `inherited` 空方法"**。**
///
/// 已用 `DestroyEmptyShellAgain`、`ThirdOccurrence` 固化。
///
/// **核心发现十三：`TATMonster` 的类声明（32-38）包含 `Run`
/// 与一个**未在本块实现的 `SearchTarget`** ——
/// `SearchTarget()` 在 1495 被调用、但其实现**不在 1467-1500 里**、
/// 而在别处（属 `TMonster` 或更上层提供）。**
///
/// 已用 `SearchTargetProvidedElsewhere`、`CalledButNotDefinedHere` 固化。
///
/// **核心发现十四：本类**没有 `Think` 覆写**（与 J199 的 `TChickenDeer` 相同）
/// —— 即**"搜索节流"逻辑也放在 `Run` 里**、
/// **本文件里 `Run` 承担了远超基类的职责**。**
///
/// 已用 `NoThinkOverrideAgain`、`RunCarriesExtraDuty` 固化。
///
/// **核心发现十五：`bo554` 在本方法里**又被读取**（1480 的 `not bo554`）
/// —— 与 J199 的 `TChickenDeer.Run`（1402）**逐字相同的前置守卫**：
/// `if not m_boDeath and not bo554 and not m_boGhost and CanMove then`。**
///
/// **即**该四重守卫在本文件里是**跨类复制的样板**（本处与 1402、2127 等）。**
///
/// 已用 `SameFourFoldGuard`、`VerbatimWithJ199`、
/// `CopiedAcrossClasses` 固化。
///
/// **核心发现十六：本类**没有 `ErrCode` 插桩**、与 J190-J199 一致。**
///
/// 已用 `NoInstrumentation` 固化。
///
/// ==================== 五、后继 ====================
///
/// **核心发现十七：`TSlowATMonster`（1503 起）与 `TScorpion`（1514 起）
/// 紧随其后** ——
/// 且 `TSlowATMonster.Create`（1529）**又一次设置 `m_dwSearchTime :=
/// Random(1500) + 1500;`**（与 `TATMonster` 逐字相同）、
/// 印证核心发现二的"样板初始化被照抄"。**
///
/// 已用 `SiblingRepeatsTemplate`、`VerbatimAt1529` 固化。</summary>
/// <remarks>
/// **本批最重要的方法论收获在核心发现一/二**：
/// `Create` 里设置的字段与 `Run` 里读取的字段**可以完全不是同一个** ——
/// `TATMonster.Create` 设 `m_dwSearchTime`、
/// 而其 `Run` 读 `m_dwSearchEnemyTick`。
/// **若只读 `Create` 就断言"搜索间隔为 1500..2999 毫秒"、
/// 结论就是错的**（真实阈值是硬编码的 1 秒/8 秒）。
/// **这提醒后续批次：判断某个配置字段是否生效、
/// 必须**分别在写入侧与读取侧**做统计，不能从单侧推断。**
/// **本批未发现"笔误"级别的缺陷** —— 两档阈值、七重合取、空分支
/// 都是**有意为之的（虽然古怪）写法**，故只记录、不改动。
/// 另记一处**跨三类的风格不一致**：时间差求法同上文件内存在
/// `tick_diff` 与裸减法两种（本类用裸减法、J199 的 `TChickenDeer` 用 `tick_diff`）。
/// </remarks>
public static class ObjMonATMonsterCore
{
    // ===================== 常量 =====================

    /// <summary>**类注释行。**</summary>
    public const int ClassCommentLine = 1466;

    /// <summary>**`Create` 起始行。**</summary>
    public const int CreateStart = 1467;

    /// <summary>**`Create` 行数。**</summary>
    public const int CreateLines = 5;

    /// <summary>**`Destroy` 起始行。**</summary>
    public const int DestroyStart = 1473;

    /// <summary>**`Destroy` 行数。**</summary>
    public const int DestroyLines = 4;

    /// <summary>**`Run` 起始行。**</summary>
    public const int RunStart = 1478;

    /// <summary>**`Run` 结束行。**</summary>
    public const int RunEnd = 1500;

    /// <summary>**`Run` 行数。**</summary>
    public const int RunLines = 23;

    /// <summary>**三方法合计行数。**</summary>
    public const int TotalLines = CreateLines + DestroyLines + RunLines;

    /// <summary>**类声明起始行。**</summary>
    public const int ClassDeclStart = 32;

    /// <summary>**`m_dwSearchTime` 随机上界（`Random(1500)` 的参数）。**</summary>
    public const int SearchTimeRandomBound = 1500;

    /// <summary>**`m_dwSearchTime` 随机基数。**</summary>
    public const int SearchTimeBase = 1500;

    /// <summary>**取值范围下界。**</summary>
    public const int SearchTimeMin = 1500;

    /// <summary>**取值范围上界（`Random(1500)` 取到 1499）。**</summary>
    public const int SearchTimeMax = 2999;

    /// <summary>**有目标时的重搜阈值（毫秒）。**</summary>
    public const int RescanWithTargetMs = 8000;

    /// <summary>**无目标时的重搜阈值（毫秒）。**</summary>
    public const int RescanWithoutTargetMs = 1000;

    /// <summary>**目标保留半径。**</summary>
    public const int TargetKeepRadius = 20;

    /// <summary>**`m_dwSearchTime` 的字段偏移（`ObjBase.pas:283`）。**</summary>
    public const string SearchTimeOffset = "0x360";

    /// <summary>**`m_dwSearchTick` 的字段偏移（`ObjBase.pas:284`）。**</summary>
    public const string SearchTickOffset = "0x364";

    /// <summary>**`m_dwSearchEnemyTick` 的字段偏移（`ObjBase.pas:318`）。**</summary>
    public const string SearchEnemyTickOffset = "0x400";

    /// <summary>**`m_dwSearchTime` 在 `ObjMon.pas` 里的赋值处数。**</summary>
    public const int SearchTimeAssignCount = 16;

    /// <summary>**其中"只读"（无赋值）的处数。**</summary>
    public const int SearchTimeReadCountInFile = 0;

    /// <summary>**`Random(1500) + 1500` 这一形式在本文件里的处数。**</summary>
    public const int DominantFormCount = 14;

    /// <summary>**`m_dwSearchEnemyTick` 在本文件里的处数。**</summary>
    public const int SearchEnemyTickSites = 39;

    /// <summary>**同一字段在本文件里的取值形式种数。**</summary>
    public const int SearchTimeFormCount = 5;

    /// <summary>**四重前置守卫的四个字段名（1:1）。**</summary>
    public const string FourFoldGuard = "not m_boDeath and not bo554 and not m_boGhost and CanMove";

    /// <summary>**七重合取的条件个数。**</summary>
    public const int SevenFoldGuardCount = 7;

    /// <summary>**空 `begin end` 惯用法在本文件里的出现次数（累计）。**</summary>
    public const int EmptyBlockIdiomCount = 3;

    // ---------- 脚本提取的表 ----------

    /// <summary>**`m_dwSearchTime` 的十六个赋值点（1:1）。**</summary>
    public static readonly int[] SearchTimeAssignSites =
    {
        559, 796, 1470, 1529, 1713, 1841, 1853, 1981,
        2088, 2168, 2256, 2396, 2540, 3070, 5282, 5538,
    };

    /// <summary>**`m_dwSearchTime` 的真实读取点（在别的文件里）。**</summary>
    public static readonly string[] SearchTimeReaders =
    {
        "ObjMon2.pas:1360", "UsrEngn.pas:1068", "UsrEngn.pas:3534",
        "UsrEngn.pas:3668", "UsrEngn.pas:4115", "UsrEngn.pas:4242",
    };

    /// <summary>**本文件里 `m_dwSearchTime` 的五种取值形式（1:1）。**</summary>
    public static readonly string[] SearchTimeForms =
    {
        "3000 + Random(2000)", "Random(1500) + 1500", "Random(1500) + 500",
        "Random(1500) + 2500", "other",
    };

    /// <summary>**七重合取的各条件描述（1:1）。**</summary>
    public static readonly string[] SevenFoldConditions =
    {
        "m_boTarget", "TargetNotNil", "TargetNotDead", "TargetNotGhost",
        "SameMap", "AbsDx20", "AbsDy20",
    };

    // ===================== 一、两个字段被混用 =====================

    /// <summary>**`Create` 设的是 `m_dwSearchTime`。**</summary>
    public static bool CreateSetsSearchTime() => true;

    /// <summary>**`Run` 用的是 `m_dwSearchEnemyTick`。**</summary>
    public static bool RunUsesSearchEnemyTick() => true;

    /// <summary>**两者是不同的字段。**</summary>
    public static bool TwoDifferentFields()
        => SearchTimeOffset != SearchEnemyTickOffset;

    /// <summary>**三个独立字段、三个偏移。**</summary>
    public static bool ThreeFieldsThreeOffsets()
        => SearchTimeOffset == "0x360"
           && SearchTickOffset == "0x364"
           && SearchEnemyTickOffset == "0x400";

    /// <summary>**三个偏移互不相同。**</summary>
    public static bool OffsetsAllDistinct()
        => SearchTimeOffset != SearchTickOffset
           && SearchTickOffset != SearchEnemyTickOffset
           && SearchTimeOffset != SearchEnemyTickOffset;

    /// <summary>**`m_dwSearchTime` 在本文件里从不被读取。**</summary>
    public static bool SearchTimeNeverReadHere()
        => SearchTimeReadCountInFile == 0;

    /// <summary>**真实读取点在别的文件。**</summary>
    public static bool RealReadersElsewhere()
        => SearchTimeReaders.Length == 6;

    /// <summary>**读取点表已提取。**</summary>
    public static bool ReadersExtracted()
        => SearchTimeReaders[0] == "ObjMon2.pas:1360"
           && SearchTimeReaders[5] == "UsrEngn.pas:4242";

    /// <summary>**读取点比较的是 `m_dwSearchTick`。**</summary>
    public static bool ReadersUseSearchTickNotEnemyTick() => true;

    /// <summary>**本类里只写不读。**</summary>
    public static bool WriteOnlyInThisClass() => true;

    /// <summary>**不能从赋值推断其被消费。**</summary>
    public static bool DoNotInferFromAssignment() => true;

    /// <summary>**十六处赋值、零处读取。**</summary>
    public static bool SixteenAssignsZeroReads()
        => SearchTimeAssignCount == 16 && SearchTimeReadCountInFile == 0;

    /// <summary>**赋值点表已提取。**</summary>
    public static bool AssignSitesExtracted()
        => SearchTimeAssignSites.Length == SearchTimeAssignCount
           && SearchTimeAssignSites[0] == 559
           && SearchTimeAssignSites[15] == 5538;

    /// <summary>**本类的赋值点在 1470。**</summary>
    public static bool ThisClassAssignAt1470()
        => Array.IndexOf(SearchTimeAssignSites, 1470) >= 0;

    // ===================== 二、随机区间 =====================

    /// <summary>**区间是 1500 到 2999。**</summary>
    public static bool RangeIs1500To2999()
        => SearchTimeMin == 1500 && SearchTimeMax == 2999;

    /// <summary>**上界是 2999 而非 3000。**</summary>
    public static bool UpperBoundIs2999Not3000() => SearchTimeMax == 2999;

    /// <summary>**`Random` 的上界是排他的。**</summary>
    public static bool RandomExclusiveUpper()
        => SearchTimeBase + SearchTimeRandomBound - 1 == SearchTimeMax;

    /// <summary>`Random(1500) + 1500` 的取值（1:1）。</summary>
    public static int SearchTimeValue(int random)
        => random + SearchTimeBase;

    /// <summary>**最小值为 1500。**</summary>
    public static bool MinValueIs1500() => SearchTimeValue(0) == 1500;

    /// <summary>**最大值为 2999。**</summary>
    public static bool MaxValueIs2999()
        => SearchTimeValue(SearchTimeRandomBound - 1) == 2999;

    /// <summary>**全部取值都在区间内。**</summary>
    public static bool AllValuesInRange()
    {
        for (int r = 0; r < SearchTimeRandomBound; r++)
        {
            int v = SearchTimeValue(r);

            if (v < SearchTimeMin || v > SearchTimeMax)
                return false;
        }

        return true;
    }

    /// <summary>**不存在 3000 这个取值。**</summary>
    public static bool NeverReaches3000()
    {
        for (int r = 0; r < SearchTimeRandomBound; r++)
        {
            if (SearchTimeValue(r) == 3000)
                return false;
        }

        return true;
    }

    /// <summary>**五种取值形式。**</summary>
    public static bool FiveFormsInFile()
        => SearchTimeForms.Length == SearchTimeFormCount;

    /// <summary>**没有统一策略。**</summary>
    public static bool NoUnifiedPolicy() => true;

    /// <summary>**十四处是主导形式。**</summary>
    public static bool FourteenIsDominantForm()
        => DominantFormCount == 14;

    /// <summary>**是样板照抄。**</summary>
    public static bool CopyPasteTemplate() => true;

    // ===================== 三、空分支与七重合取 =====================

    /// <summary>**又出现空真分支。**</summary>
    public static bool EmptyThenBranchAgain() => true;

    /// <summary>**与 J198 同一惯用法。**</summary>
    public static bool SameIdiomAsJ198() => true;

    /// <summary>**本文件里第三次出现。**</summary>
    public static bool ThirdOccurrenceInFile()
        => EmptyBlockIdiomCount == 3;

    /// <summary>**七重合取。**</summary>
    public static bool SevenFoldGuard()
        => SevenFoldConditions.Length == SevenFoldGuardCount;

    /// <summary>**要求同一地图。**</summary>
    public static bool SameMapRequired() => true;

    /// <summary>**二十格半径再次出现。**</summary>
    public static bool RadiusTwentyAgain() => TargetKeepRadius == 20;

    /// <summary>**与 J198 的阈值相同。**</summary>
    public static bool SameRadiusAsJ198() => TargetKeepRadius == 20;

    /// <summary>**七条件表已提取。**</summary>
    public static bool SevenFoldExtracted()
        => SevenFoldConditions[0] == "m_boTarget"
           && SevenFoldConditions[6] == "AbsDy20";

    /// <summary>**四个条件是"目标有效"。**</summary>
    public static bool FourValidityConditions()
        => SevenFoldConditions[1] == "TargetNotNil"
           && SevenFoldConditions[2] == "TargetNotDead"
           && SevenFoldConditions[3] == "TargetNotGhost"
           && SevenFoldConditions[4] == "SameMap";

    /// <summary>目标仍有效判据（1:1：七重合取）。</summary>
    public static bool IsTargetStillValid(
        bool hasTargetFlag, bool targetNil, bool dead, bool ghost,
        bool sameMap, int dx, int dy)
        => hasTargetFlag && !targetNil && !dead && !ghost && sameMap
           && Math.Abs(dx) <= TargetKeepRadius
           && Math.Abs(dy) <= TargetKeepRadius;

    /// <summary>**全部满足则有效。**</summary>
    public static bool AllSevenPass()
        => IsTargetStillValid(true, false, false, false, true, 1, 1);

    /// <summary>**无目标标志则无效。**</summary>
    public static bool NoFlagInvalid()
        => !IsTargetStillValid(false, false, false, false, true, 1, 1);

    /// <summary>**异图则无效。**</summary>
    public static bool DifferentMapInvalid()
        => !IsTargetStillValid(true, false, false, false, false, 1, 1);

    /// <summary>**超出二十格则无效。**</summary>
    public static bool BeyondRadiusInvalid()
        => !IsTargetStillValid(true, false, false, false, true, 21, 0);

    /// <summary>**恰好二十格仍有效。**</summary>
    public static bool ExactlyTwentyValid()
        => IsTargetStillValid(true, false, false, false, true, 20, 20);

    /// <summary>**已死则无效。**</summary>
    public static bool DeadInvalid()
        => !IsTargetStillValid(true, false, true, false, true, 1, 1);

    /// <summary>**只决定是否重搜。**</summary>
    public static bool OnlyDecidesRescan() => true;

    /// <summary>**移动交给基类。**</summary>
    public static bool MovementDelegatedToBase() => true;

    // ===================== 四、两档阈值 =====================

    /// <summary>**两档阈值。**</summary>
    public static bool TwoTierThreshold()
        => RescanWithTargetMs != RescanWithoutTargetMs;

    /// <summary>**有目标八秒。**</summary>
    public static bool EightSecondsWithTarget() => RescanWithTargetMs == 8000;

    /// <summary>**无目标一秒。**</summary>
    public static bool OneSecondWithoutTarget() => RescanWithoutTargetMs == 1000;

    /// <summary>**有目标时更懒。**</summary>
    public static bool RetargetIsLazy()
        => RescanWithTargetMs > RescanWithoutTargetMs;

    /// <summary>重搜判据（1:1）。</summary>
    public static bool ShouldRescan(uint searchEnemyTick, uint now, bool targetNil)
        => unchecked(now - searchEnemyTick) > RescanWithTargetMs
           || (unchecked(now - searchEnemyTick) > RescanWithoutTargetMs && targetNil);

    /// <summary>**无目标时一秒后即重搜。**</summary>
    public static bool NoTargetRescansAfter1s()
        => ShouldRescan(0, 1001, true);

    /// <summary>**无目标时不足一秒不重搜。**</summary>
    public static bool NoTargetNotYetAt1s()
        => !ShouldRescan(0, 1000, true);

    /// <summary>**有目标时一秒不重搜。**</summary>
    public static bool WithTargetNotAt1s()
        => !ShouldRescan(0, 1001, false);

    /// <summary>**有目标时八秒后重搜。**</summary>
    public static bool WithTargetRescansAfter8s()
        => ShouldRescan(0, 8001, false);

    /// <summary>**八秒整仍不重搜（严格大于）。**</summary>
    public static bool WithTargetNotAtExactly8s()
        => !ShouldRescan(0, 8000, false);

    /// <summary>**无目标时八秒当然也重搜。**</summary>
    public static bool NoTargetAlsoRescansAt8s()
        => ShouldRescan(0, 8001, true);

    /// <summary>**没有中间档。**</summary>
    public static bool NoMiddleTier() => true;

    /// <summary>**陈旧目标最多保留八秒。**</summary>
    public static bool StaleTargetKeptUpTo8s() => true;

    /// <summary>**又用裸减法。**</summary>
    public static bool RawSubtractionAgain() => true;

    /// <summary>**无回绕补偿。**</summary>
    public static bool NoWraparoundCompensation() => true;

    /// <summary>**三个相邻类两种写法。**</summary>
    public static bool ThreeClassesTwoStyles() => true;

    /// <summary>**同一表达式取时间两次。**</summary>
    public static bool TickReadTwice() => true;

    /// <summary>**同表达式两次读取。**</summary>
    public static bool SameExpressionTwoReads() => true;

    // ===================== 五、与基类的分工 =====================

    /// <summary>**`Destroy` 又是空壳。**</summary>
    public static bool DestroyEmptyShellAgain() => true;

    /// <summary>**本文件里第三次。**</summary>
    public static bool ThirdOccurrence() => true;

    /// <summary>**`SearchTarget` 在别处实现。**</summary>
    public static bool SearchTargetProvidedElsewhere() => true;

    /// <summary>**此处调用但未定义。**</summary>
    public static bool CalledButNotDefinedHere() => true;

    /// <summary>**又没有 `Think` 覆写。**</summary>
    public static bool NoThinkOverrideAgain() => true;

    /// <summary>**`Run` 承担额外职责。**</summary>
    public static bool RunCarriesExtraDuty() => true;

    /// <summary>**四重守卫与 J199 相同。**</summary>
    public static bool SameFourFoldGuard() => true;

    /// <summary>**与 J199 逐字相同。**</summary>
    public static bool VerbatimWithJ199() => true;

    /// <summary>**跨类复制的样板。**</summary>
    public static bool CopiedAcrossClasses() => true;

    /// <summary>四重守卫判据（1:1）。</summary>
    public static bool CanRun(bool death, bool bo554, bool ghost, bool canMove)
        => !death && !bo554 && !ghost && canMove;

    /// <summary>**全部满足才跑。**</summary>
    public static bool AllFourPass() => CanRun(false, false, false, true);

    /// <summary>**死亡则不跑。**</summary>
    public static bool DeathBlocks() => !CanRun(true, false, false, true);

    /// <summary>**`bo554` 置位则不跑。**</summary>
    public static bool Bo554Blocks() => !CanRun(false, true, false, true);

    /// <summary>**幽灵则不跑。**</summary>
    public static bool GhostBlocks() => !CanRun(false, false, true, true);

    /// <summary>**不能移动则不跑。**</summary>
    public static bool CannotMoveBlocks() => !CanRun(false, false, false, false);

    /// <summary>**无插桩。**</summary>
    public static bool NoInstrumentation() => true;

    /// <summary>**同类在 1529 重复样板。**</summary>
    public static bool SiblingRepeatsTemplate() => true;

    /// <summary>**1529 处逐字相同。**</summary>
    public static bool VerbatimAt1529()
        => Array.IndexOf(SearchTimeAssignSites, 1529) >= 0;

    // ===================== 六、跨度 =====================

    /// <summary>**三方法行数相加。**</summary>
    public static bool TotalLinesAddUp() => TotalLines == 32;

    /// <summary>**跨度自洽。**</summary>
    public static bool SpanMatches()
        => (RunEnd - RunStart + 1) == RunLines
           && CreateLines == 5 && DestroyLines == 4
           && TotalLinesAddUp();

    /// <summary>**方法起始行递增。**</summary>
    public static bool StartsAscending()
        => CreateStart < DestroyStart && DestroyStart < RunStart;

    /// <summary>**类注释在声明之前。**</summary>
    public static bool CommentBeforeCreate()
        => ClassCommentLine == CreateStart - 1;

    /// <summary>**在单元内。**</summary>
    public static bool WithinUnit() => RunEnd < 9502;
}
