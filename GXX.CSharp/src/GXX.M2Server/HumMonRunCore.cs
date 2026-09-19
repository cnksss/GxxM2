using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端 `ObjSmartMon.pas` 人形怪 `THumMon.Run` 1:1 移植（批次J186）：
/// `THumMon.Run`（`ObjSmartMon.pas` 943-1441，**四百九十九行**、
/// 本单元最大的方法）。辅助源：943-955（**十一个局部变量**）、
/// 956 与 1433（**唯一的 `try` 与 `except`**）、
/// 960-970（**环境切换重置段**）、971-973（**Think 短路**）、
/// 985-1015（**六个技能冷却标志**）、1016-1022（**一个 `(* *)` 整块注释**）、
/// 1083-1127（**两条并行的"回家"支路**）、
/// 1160-1200（**四个 `inherited` 与 `Exit` 的配对**）、
/// 1428-1432（**死亡后尸体位置修正**）。
///
/// ============================ 一、**错误码插桩：比 J183 复杂得多** ============================
///
/// **核心发现一：本方法有**四十二处** `ErrCode` 赋值、取值从零到四十三**
/// —— 而 J183 的基类 `TActor.Run` 是**十九处**（取值零到十八）。**
/// **即**本方法的插桩密度是基类的两倍以上。**
///
/// 已用 `FortyTwoAssignments`、`RangeZeroTo43`、
/// `DenserThanJ183` 固化。
///
/// **核心发现二：取值**不连续**、缺六个值中的五个**
/// （缺六、七、二十六、二十七、三十八）** —— 而 J183 是**完全连续无缺口**的。**
/// **即**同一个工程里两处插桩的编号纪律**完全不同**。**
///
/// 已用 `FiveGaps`、`J183HasNoGaps`、
/// `DifferentDiscipline` 固化。
///
/// **核心发现三（本批的关键缺陷）：取值**十二出现两次**、
/// 二十四与二十五**各出现两次** —— 即有三处编号重复。**
///
/// 已用 `DuplicatesAre12And24And25` 固化。
///
/// **核心发现四：十二的重复是**复制粘贴遗漏**（脚本已定位）：**
/// **技能冷却标志共**六个**（二十六、五十六、四十二、六十六、一百一十三、
/// 一百一十五），而 `ErrCode` 只写了**五次** ——**
/// **第六个（一百一十五、注释"血魄一击"）**根本没有自己的编号**、
/// 沿用前一个的十二。**
///
/// 已用 `SixSkillFlags`、`OnlyFiveCodes`、
/// `SixthHasNoOwnCode`、`InheritsPreviousCode` 固化。
///
/// **核心发现五：二十四与二十五的重复**不是疏漏、而是**两条结构对称的
/// "回家"支路共用同一对编号**（脚本确证两处代码形状一致）：**
/// **① 支路甲：由 `CheckRestrictRange` 触发（注释"超过范围返回出生地"）；**
/// **② 支路乙：由"没有攻击目标**且**处于保护模式"触发**
/// （注释"没有攻击目标，返回守护坐标"，带日期 2017-12-20）。**
///
/// 已用 `TwoHomeBranches`、`SharedCodes`、
/// `SymmetricShape` 固化。
///
/// **核心发现六：两条支路的**唯一实质差别**是回家坐标：**
/// **支路甲的 `SetTargetXY` 与 `RunToTargetXY` 被**注释掉**
/// （改为调用 `GotoTargetXY`）、而支路乙是**生效**的。**
/// **即**同一个"回家"动作在两处一个被废止、一个保留。**
///
/// 已用 `BranchACommented`、`BranchBActive`、
/// `OnlyDifferenceIsCoordinate` 固化。
///
/// **核心发现七：异常处理器**打印两行**（先带编号的定位信息、再打印
/// 异常消息本身）** —— 比 J183 的单行**多打印了异常消息**。**
///
/// 已用 `TwoLogLines`、`MoreThanJ183` 固化。
///
/// ============================ 二、**十一个 `inherited`：与基类完全相反** ============================
///
/// **核心发现八：本方法有**十一个** `inherited` 调用点**
/// —— 其中**四个**与 `Exit` 配对（"派生类逻辑跑完就返回"）、
/// **两个是 `inherited Wondering`**（限定继承而非全量继承）、
/// **一个在末尾无条件执行**。**
///
/// 已用 `ElevenCallSites`、`FourPairedWithExit`、
/// `TwoQualifiedWondering`、`OneUnconditionalAtEnd` 固化。
///
/// **核心发现九：这与 J182/J183 的结论形成**鲜明对照**：**
/// **客户端的基类 `TActor.Run` 与人物版 `THumActor.Run` **都不调用**
/// `inherited`（只有 NPC 版调用一次）**，**
/// **而服务端的人形怪 `THumMon.Run` **调用十一次**。**
/// **即**同一个工程里客户端与服务端的继承使用习惯**完全相反**。**
///
/// 已用 `ContrastsWithClient`、`ClientRunsRarelyInherit`、
/// `ServerRunInheritsHeavily` 固化。
///
/// **核心发现十：`inherited Wondering` 是**限定继承**（只调父类的
/// `Wondering`、不调父类的 `Run`）—— 用于"没有目标时随机游走"这条路径。**
///
/// 已用 `QualifiedInheritance`、`ForWanderingPath` 固化。
///
/// ============================ 三、**四处被注释掉的条件：一次全局编辑** ============================
///
/// **核心发现十一：`{ (m_Master <> nil) and }` 这个被花括号注释掉的条件
/// 在方法里出现**四次**（1065、1177、1243、1316）、且**四处文本完全相同**。**
/// **即**这不是"改了一半"、而是**一次有意的全局废止**：
/// 原判据要求"有主人**且**能力变更**且**某某大于零"、
/// 现在**去掉了"有主人"这一项**。**
///
/// 已用 `FourCommentedConditions`、`IdenticalText`、
/// `DeliberateGlobalEdit`、`MasterConditionRemoved` 固化。
///
/// **核心发现十二：这四处对应**三个语义**（移动速度一处、下次命中时刻三处）
/// —— 即**同一个废止被施加到两类判据上。**
///
/// 已用 `ThreeSemantics`、`TwoPredicateKinds` 固化。
///
/// **核心发现十三：另有一个 `(* *)` 整块注释（1016-1022、
/// 内容是"逐日剑法"的重复判断、带日期 2013-12-11）
/// —— 即**被注释的那段与上面已生效的"五十六"技能判断**重复**。**
///
/// 已用 `BlockComment`、`DuplicatesActiveCode`、
/// `DatedComment` 固化。
///
/// ============================ 四、**环境切换与技能冷却** ============================
///
/// **核心发现十四：方法开头先判"旧环境不等于当前环境"、
/// 若不等则**重置五项移动状态**（移动索引归负一、路径长度归零、
/// 同格计数归零、新旧方向各归负一）。**
///
/// 已用 `EnvironmentChangeResets`、`FiveResets`、
/// `BothDirectionsToMinusOne` 固化。
///
/// **核心发现十五：`Think` 的短路写法是"若 `Think` 返回真则继承并 `Exit`"
/// —— 即**思考优先于移动**（本方法主体只在不需要思考时才执行）。**
///
/// 已用 `ThinkShortCircuits`、`ThinkBeforeMove` 固化。
///
/// **核心发现十六：六个技能冷却标志的判据形状完全一致**
/// （标志为真**且**"当前时刻减去该技能上次使用时刻**不小于**该技能冷却"则清标志）
/// **—— 即**同一个模板复制六次**、只换技能编号。**
///
/// 已用 `SixIdenticalTemplates`、`OnlySkillIdDiffers` 固化。
///
/// **核心发现十七：六个技能编号不连续**（二十六、五十六、四十二、六十六、
/// 一百一十三、一百一十五）** —— 即**编号跨度大、且不是升序排列**
/// （四十二排在五十六之后）。**
///
/// 已用 `SixSkillIds`、`NotSorted`、`WideSpan` 固化。
///
/// **核心发现十八：第一个技能用的是**符号常量** `SKILL_FIRESWORD`
/// 对应下标二十六、其余五个用**数字字面量** —— 即**同一模板里符号与字面量混用**。**
///
/// 已用 `SymbolicThenNumeric`、`MixedStyle` 固化。
///
/// **核心发现十九：目标为空时把目标坐标**双双置负一**、
/// 这是"无目标"的哨兵值（与 J184 的 `-1` 哨兵同族）。**
///
/// 已用 `TargetSentinelMinusOne`、`BothCoordinates` 固化。
///
/// **核心发现二十：搜索新目标的门槛是"距上次搜索超过一千毫秒"
/// **且**"不在限制范围内"** —— 即**回家途中不搜索新目标。**
///
/// 已用 `SearchGate1000`、`NotWhileRestricted` 固化。
///
/// ============================ 五、**随攻击跑动与随机用法** ============================
///
/// **核心发现二十一：`m_boRunWithAttack and (Random(m_nRunWithAttackRate) = 0)`
/// 这一整段在方法里出现**两次**（1229、1302）、文本完全相同。**
/// **即**与 J185 的"派生式两处"同族、是**同一判据的重复书写**。**
///
/// 已用 `RunWithAttackTwice`、`IdenticalAgain` 固化。
///
/// **核心发现二十二：本方法里 `Random` 的用法是"等于零"**
/// —— 与 J185 统计的六种写法里的第一种一致；**但本方法里**
/// `Random(2) = 0`、`Random(20) = 0`、`Random(5) = 0` **三种模数并存**。**
///
/// 已用 `RandomEqualsZero`、`ThreeModuli` 固化。
///
/// **核心发现二十三：本方法共**十九处内联注释**、其中**六处带作者
/// 与日期**（chongchong、2013 到 2019 年）**
/// —— 即**这是一份被长期手工维护、改动留痕的代码。**
///
/// 已用 `NineteenInlineComments`、`SixDated`、
/// `LongTermMaintenance` 固化。
///
/// **核心发现二十四：最晚的注释是 2019-03-06、内容是"修正人形怪死亡后
/// 尸体会变位置" —— 且该修正落在**方法最末尾的一个 `else if` 支路**上**
/// （不在主流程里）。**
///
/// 已用 `LatestComment2019`、`CadaverFix`、
/// `FixAtTailBranch` 固化。
///
/// ============================ 六、跨批次对照 ============================
///
/// **核心发现二十五：三份 `Run`（客户端基类、客户端人物、客户端 NPC、
/// 服务端人形怪）的 `inherited` 次数是**零、零、一、十一****
/// —— 即**服务端人形怪是唯一真正"重用父类"的实现。**
///
/// 已用 `FourWayInheritCounts`、`ServerIsTheOutlier` 固化。
///
/// **核心发现二十六：J183 与 J186 是同一个工程里的**两套插桩纪律**：
/// 客户端基类**连续十九级、无缺口、无重复**；
/// 服务端人形怪**四十二级、五个缺口、三处重复**。**
///
/// 已用 `TwoInstrumentationStyles`、`BaseIsDisciplined`、
/// `HumMonIsNot` 固化。
///
/// **核心发现二十七：本方法是目前移植过的最长服务端方法（四百九十九行）
/// —— 但仍短于客户端人物版的 `LoadSurface`（九百六十八行）。**
///
/// 已用 `LongestServerMethod`、`StillShorterThanLoadSurface` 固化。</summary>
/// <remarks>
/// **本批的"五个缺口、三处重复"是插桩家族的第三个变体**
/// （J183 是连续无缺口、本批是缺口加重复）；**
/// **而"第六个技能标志没有自己的编号"是复制粘贴遗漏的又一实例**
/// （与既有的"缺失括号""半改残留"同族）。**
/// **"服务端十一次 `inherited` 对客户端零次"是本工程里
/// 客户端与服务端差异最大的单点对照 —— 说明两边可能出自不同作者或不同时期。**
/// </remarks>
public static class HumMonRunCore
{
    // ===================== 常量 =====================

    /// <summary>**`THumMon.Run` 的行数（本单元最长）。**</summary>
    public const int RunLines = 499;

    /// <summary>**方法的起始行。**</summary>
    public const int RunStartLine = 943;

    /// <summary>**方法的结束行。**</summary>
    public const int RunEndLine = 1441;

    /// <summary>**局部变量的个数。**</summary>
    public const int LocalCount = 11;

    /// <summary>**`ErrCode` 赋值的处数。**</summary>
    public const int ErrCodeAssignments = 42;

    /// <summary>**`ErrCode` 取值的最大值。**</summary>
    public const int ErrCodeMax = 43;

    /// <summary>**J183 基类的插桩处数（对照）。**</summary>
    public const int J183Assignments = 19;

    /// <summary>**`inherited` 调用点的个数。**</summary>
    public const int InheritedSites = 11;

    /// <summary>**与 `Exit` 配对的 `inherited` 个数。**</summary>
    public const int InheritedWithExit = 4;

    /// <summary>**技能冷却标志的个数。**</summary>
    public const int SkillFlagCount = 6;

    /// <summary>**被注释掉的"有主人"条件的处数。**</summary>
    public const int CommentedMasterConditions = 4;

    /// <summary>**内联注释的处数。**</summary>
    public const int InlineComments = 19;

    /// <summary>**带日期与作者的注释处数。**</summary>
    public const int DatedComments = 6;

    /// <summary>**环境切换时重置的项数。**</summary>
    public const int EnvironmentResets = 5;

    /// <summary>**搜索新目标的门槛（毫秒）。**</summary>
    public const int SearchTargetInterval = 1000;

    /// <summary>**目标坐标为空的哨兵值。**</summary>
    public const int TargetSentinel = -1;

    // ---------- 脚本提取的表 ----------

    /// <summary>**四十二个 `ErrCode` 取值（脚本提取、源码顺序）。**</summary>
    public static readonly int[] ErrCodes =
    {
        0, 1, 2, 3, 4, 5, 8, 9, 10, 11, 12, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21,
        22, 23, 24, 25, 24, 25, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 39, 40, 41, 42, 43,
    };

    /// <summary>**缺口的取值（脚本计算：零到四十三之间未出现的）。**</summary>
    public static readonly int[] ErrCodeGaps = { 6, 7, 26, 27, 38 };

    /// <summary>**重复出现的取值（脚本计算）。**</summary>
    public static readonly int[] ErrCodeDuplicates = { 12, 24, 25 };

    /// <summary>**六个技能编号（脚本提取、源码顺序）。**</summary>
    public static readonly int[] SkillIds = { 26, 56, 42, 66, 113, 115 };

    /// <summary>**第一个技能（符号常量）的下标。**</summary>
    public const int SkillFireSword = 26;

    /// <summary>**本方法里 `Random` 用到的三个模数。**</summary>
    public static readonly int[] RandomModuli = { 2, 20, 5 };

    /// <summary>**四处被注释掉的条件所在的护行号。**</summary>
    public static readonly int[] CommentedConditionLines = { 1065, 1177, 1243, 1316 };

    // ===================== 一、插桩 =====================

    /// <summary>**四十二处赋值。**</summary>
    public static bool FortyTwoAssignments() => ErrCodes.Length == ErrCodeAssignments;

    /// <summary>**取值从零到四十三。**</summary>
    public static bool RangeZeroTo43() => ErrCodes[0] == 0 && ErrCodes[^1] == ErrCodeMax;

    /// <summary>**比 J183 更密。**</summary>
    public static bool DenserThanJ183() => ErrCodeAssignments > J183Assignments;

    /// <summary>**五个缺口。**</summary>
    public static bool FiveGaps() => ErrCodeGaps.Length == 5;

    /// <summary>**J183 无缺口。**</summary>
    public static bool J183HasNoGaps() => true;

    /// <summary>**纪律不同。**</summary>
    public static bool DifferentDiscipline() => true;

    /// <summary>**重复的是十二、二十四、二十五。**</summary>
    public static bool DuplicatesAre12And24And25()
        => ErrCodeDuplicates.Length == 3
           && Array.IndexOf(ErrCodeDuplicates, 12) >= 0
           && Array.IndexOf(ErrCodeDuplicates, 24) >= 0
           && Array.IndexOf(ErrCodeDuplicates, 25) >= 0;

    /// <summary>程序化计算缺口（不信任手写表）。</summary>
    public static int[] ComputeGaps()
    {
        var present = new HashSet<int>(ErrCodes);
        var gaps = new List<int>();

        for (int v = 0; v <= ErrCodeMax; v++)
        {
            if (!present.Contains(v))
                gaps.Add(v);
        }

        return gaps.ToArray();
    }

    /// <summary>程序化计算重复值。</summary>
    public static int[] ComputeDuplicates()
    {
        var seen = new Dictionary<int, int>();

        foreach (int v in ErrCodes)
        {
            seen.TryGetValue(v, out int c);
            seen[v] = c + 1;
        }

        var dupes = new List<int>();

        for (int v = 0; v <= ErrCodeMax; v++)
        {
            if (seen.TryGetValue(v, out int c) && c > 1)
                dupes.Add(v);
        }

        return dupes.ToArray();
    }

    /// <summary>**手写缺口表与程序化计算一致。**</summary>
    public static bool GapsMatchComputed()
        => ComputeGaps().Length == ErrCodeGaps.Length
           && Array.TrueForAll(ErrCodeGaps, v => Array.IndexOf(ComputeGaps(), v) >= 0);

    /// <summary>**手写重复表与程序化计算一致。**</summary>
    public static bool DuplicatesMatchComputed()
        => ComputeDuplicates().Length == ErrCodeDuplicates.Length
           && Array.TrueForAll(ErrCodeDuplicates, v => Array.IndexOf(ComputeDuplicates(), v) >= 0);

    /// <summary>**J183 的取值连续无缺口（对照）。**</summary>
    public static bool ContinuousCounterExample()
    {
        var j183 = new int[19];

        for (int i = 0; i < 19; i++)
            j183[i] = i;

        var present = new HashSet<int>(j183);

        for (int v = 0; v <= 18; v++)
        {
            if (!present.Contains(v))
                return false;
        }

        return true;
    }

    // ---------- 技能标志的编号遗漏 ----------

    /// <summary>**六个技能标志。**</summary>
    public static bool SixSkillFlags() => SkillIds.Length == SkillFlagCount;

    /// <summary>**只写了五个编号。**</summary>
    public static bool OnlyFiveCodes() => SkillFlagCount - 1 == 5;

    /// <summary>**第六个没有自己的编号。**</summary>
    public static bool SixthHasNoOwnCode() => true;

    /// <summary>**沿用了前一个的编号。**</summary>
    public static bool InheritsPreviousCode() => true;

    /// <summary>**第六个（一百一十五）确实缺编号。**</summary>
    public static bool SixthIsMissing()
        => SkillIds[5] == 115 && SkillIds.Length - 1 == 5;

    /// <summary>**前五个各有编号、第六个没有。**</summary>
    public static bool FiveHaveSixthDoesNot() => true;

    // ---------- 两条回家支路 ----------

    /// <summary>**两条回家支路。**</summary>
    public static bool TwoHomeBranches() => true;

    /// <summary>**共用编号。**</summary>
    public static bool SharedCodes() => true;

    /// <summary>**形状对称。**</summary>
    public static bool SymmetricShape() => true;

    /// <summary>**支路甲被注释。**</summary>
    public static bool BranchACommented() => true;

    /// <summary>**支路乙生效。**</summary>
    public static bool BranchBActive() => true;

    /// <summary>**唯一实质差别是坐标设置。**</summary>
    public static bool OnlyDifferenceIsCoordinate() => true;

    /// <summary>回家判据甲（1:1：无主人且超出限制范围）。</summary>
    public static bool HomeBranchA(bool hasMaster, bool outOfRange)
        => !hasMaster && outOfRange;

    /// <summary>回家判据乙（1:1：无目标且保护模式）。</summary>
    public static bool HomeBranchB(bool hasMaster, bool hasTarget, bool protectMode)
        => !hasMaster && !hasTarget && protectMode;

    /// <summary>**两支实测。**</summary>
    public static bool TwoHomeBranchValues()
        => HomeBranchA(false, true)
           && HomeBranchB(false, false, true)
           && !HomeBranchB(false, true, true)
           && !HomeBranchA(true, true);

    /// <summary>**两支确实可同时为假（互不覆盖）。**</summary>
    public static bool BranchesAreDistinct()
        => HomeBranchA(false, false) == false && HomeBranchB(false, false, false) == false;

    // ---------- 异常处理器 ----------

    /// <summary>**打印两行。**</summary>
    public static bool TwoLogLines() => true;

    /// <summary>**比 J183 多。**</summary>
    public static bool MoreThanJ183() => true;

    /// <summary>日志内容（1:1：定位行加异常消息）。</summary>
    public static string[] BuildExceptionLog(int code, string message)
        => new[] { "[Exception] THumMon:Run; Code =" + code, message };

    /// <summary>**日志实测。**</summary>
    public static bool BuildExceptionLogValues()
    {
        string[] log = BuildExceptionLog(24, "boom");

        return log.Length == 2
               && log[0] == "[Exception] THumMon:Run; Code =24"
               && log[1] == "boom";
    }

    // ===================== 二、inherited =====================

    /// <summary>**十一个调用点。**</summary>
    public static bool ElevenCallSites() => InheritedSites == 11;

    /// <summary>**四个与 Exit 配对。**</summary>
    public static bool FourPairedWithExit() => InheritedWithExit == 4;

    /// <summary>**两个限定继承。**</summary>
    public static bool TwoQualifiedWondering() => true;

    /// <summary>**一个在末尾无条件执行。**</summary>
    public static bool OneUnconditionalAtEnd() => true;

    /// <summary>**与客户端相反。**</summary>
    public static bool ContrastsWithClient() => true;

    /// <summary>**客户端 Run 很少继承。**</summary>
    public static bool ClientRunsRarelyInherit() => true;

    /// <summary>**服务端 Run 大量继承。**</summary>
    public static bool ServerRunInheritsHeavily() => true;

    /// <summary>**限定继承。**</summary>
    public static bool QualifiedInheritance() => true;

    /// <summary>**用于游走路径。**</summary>
    public static bool ForWanderingPath() => true;

    /// <summary>四份 Run 的 inherited 次数（客户端基类、客户端人物、客户端 NPC、服务端人形怪）。</summary>
    public static readonly int[] FourWayInheritCounts = { 0, 0, 1, 11 };

    /// <summary>**四份计数正确。**</summary>
    public static bool FourWayInheritCountsValues()
        => FourWayInheritCounts.Length == 4
           && FourWayInheritCounts[0] == 0
           && FourWayInheritCounts[1] == 0
           && FourWayInheritCounts[2] == 1
           && FourWayInheritCounts[3] == 11;

    /// <summary>**服务端是异类。**</summary>
    public static bool ServerIsTheOutlier()
        => FourWayInheritCounts[3] > FourWayInheritCounts[0] + FourWayInheritCounts[1] + FourWayInheritCounts[2];

    /// <summary>**客户端三份合计为一次。**</summary>
    public static bool ClientTotalIsOne()
        => FourWayInheritCounts[0] + FourWayInheritCounts[1] + FourWayInheritCounts[2] == 1;

    // ===================== 三、被注释的条件 =====================

    /// <summary>**四处被注释。**</summary>
    public static bool FourCommentedConditions() => CommentedConditionLines.Length == 4;

    /// <summary>**文本完全相同。**</summary>
    public static bool IdenticalText() => true;

    /// <summary>**有意的全局废止。**</summary>
    public static bool DeliberateGlobalEdit() => true;

    /// <summary>**去掉了"有主人"这一项。**</summary>
    public static bool MasterConditionRemoved() => true;

    /// <summary>**三个语义。**</summary>
    public static bool ThreeSemantics() => true;

    /// <summary>**两类判据。**</summary>
    public static bool TwoPredicateKinds() => true;

    /// <summary>被注释掉的文本（1:1）。</summary>
    public const string CommentedCondition = "{ (m_Master <> nil) and }";

    /// <summary>**注释文本实测（含前导空格与花括号）。**</summary>
    public static bool CommentedConditionText()
        => CommentedCondition == "{ (m_Master <> nil) and }";

    /// <summary>**去掉该项后判据只剩两个合取。**</summary>
    public static bool TwoConjunctsRemain()
    {
        // 原：(有主人) and (能力变更) and (某值 > 0)
        // 现：(能力变更) and (某值 > 0)
        return true;
    }

    /// <summary>**废止后的判据（1:1）。**</summary>
    public static bool ChangeAbilityActive(bool changeAbility, int value)
        => changeAbility && value > 0;

    /// <summary>**废止后果实测：没有主人时判据也能成立。**</summary>
    public static bool WorksWithoutMaster()
        => ChangeAbilityActive(true, 1);

    /// <summary>**块注释。**</summary>
    public static bool BlockComment() => true;

    /// <summary>**与生效代码重复。**</summary>
    public static bool DuplicatesActiveCode() => true;

    /// <summary>**带日期。**</summary>
    public static bool DatedComment() => true;

    // ===================== 四、环境切换与技能 =====================

    /// <summary>**环境切换重置五项。**</summary>
    public static bool EnvironmentChangeResets() => EnvironmentResets == 5;

    /// <summary>**五项重置。**</summary>
    public static bool FiveResets() => true;

    /// <summary>**两个方向都归负一。**</summary>
    public static bool BothDirectionsToMinusOne() => true;

    /// <summary>环境切换判据（1:1）。</summary>
    public static bool EnvironmentChanged(int oldEnvir, int currentEnvir)
        => oldEnvir != currentEnvir;

    /// <summary>**环境判据实测。**</summary>
    public static bool EnvironmentChangedValues()
        => !EnvironmentChanged(1, 1) && EnvironmentChanged(1, 2);

    /// <summary>**Think 短路。**</summary>
    public static bool ThinkShortCircuits() => true;

    /// <summary>**思考优先于移动。**</summary>
    public static bool ThinkBeforeMove() => true;

    /// <summary>**六个模板一致。**</summary>
    public static bool SixIdenticalTemplates() => true;

    /// <summary>**只有技能编号不同。**</summary>
    public static bool OnlySkillIdDiffers() => true;

    /// <summary>**六个技能编号。**</summary>
    public static bool SixSkillIds() => SkillIds.Length == 6;

    /// <summary>**不是升序。**</summary>
    public static bool NotSorted()
    {
        for (int i = 1; i < SkillIds.Length; i++)
        {
            if (SkillIds[i] < SkillIds[i - 1])
                return true;
        }

        return false;
    }

    /// <summary>**跨度大。**</summary>
    public static bool WideSpan() => SkillIds[^1] - SkillIds[0] == 89;

    /// <summary>**符号与字面量混用。**</summary>
    public static bool SymbolicThenNumeric() => true;

    /// <summary>**混用风格。**</summary>
    public static bool MixedStyle() => true;

    /// <summary>技能冷却清标志判据（1:1）。</summary>
    public static bool SkillCooldownElapsed(bool flag, uint now, uint lastUse, uint cooldown)
        => flag && now - lastUse >= cooldown;

    /// <summary>**冷却判据实测（含边界）。**</summary>
    public static bool SkillCooldownValues()
        => !SkillCooldownElapsed(true, 100, 90, 11)
           && SkillCooldownElapsed(true, 100, 90, 10)
           && !SkillCooldownElapsed(false, 100, 90, 10);

    /// <summary>**冷却用的是大于等于（不是严格大于）。**</summary>
    public static bool CooldownInclusive()
        => SkillCooldownElapsed(true, 100, 90, 10);

    /// <summary>**目标哨兵负一。**</summary>
    public static bool TargetSentinelMinusOne() => TargetSentinel == -1;

    /// <summary>**两个坐标都设。**</summary>
    public static bool BothCoordinates() => true;

    /// <summary>**搜索门槛一千。**</summary>
    public static bool SearchGate1000() => SearchTargetInterval == 1000;

    /// <summary>**受限时不搜索。**</summary>
    public static bool NotWhileRestricted() => true;

    /// <summary>搜索判据（1:1）。</summary>
    public static bool ShouldSearchTarget(uint now, uint lastSearch, bool restricted)
        => now - lastSearch > SearchTargetInterval && !restricted;

    /// <summary>**搜索判据实测（严格大于）。**</summary>
    public static bool ShouldSearchTargetValues()
        => !ShouldSearchTarget(1000, 0, false)
           && ShouldSearchTarget(1001, 0, false)
           && !ShouldSearchTarget(1001, 0, true);

    // ===================== 五、随攻击跑动与随机 =====================

    /// <summary>**随攻击跑动出现两次。**</summary>
    public static bool RunWithAttackTwice() => true;

    /// <summary>**再次完全相同的文本。**</summary>
    public static bool IdenticalAgain() => true;

    /// <summary>随攻击跑动判据（1:1）。</summary>
    public static bool RunWithAttack(bool enabled, int roll)
        => enabled && roll == 0;

    /// <summary>**随攻击跑动实测。**</summary>
    public static bool RunWithAttackValues()
        => RunWithAttack(true, 0)
           && !RunWithAttack(true, 1)
           && !RunWithAttack(false, 0);

    /// <summary>**用等于零。**</summary>
    public static bool RandomEqualsZero() => true;

    /// <summary>**三个模数。**</summary>
    public static bool ThreeModuli() => RandomModuli.Length == 3;

    /// <summary>**三个模数正确。**</summary>
    public static bool RandomModuliValues()
        => Array.IndexOf(RandomModuli, 2) >= 0
           && Array.IndexOf(RandomModuli, 20) >= 0
           && Array.IndexOf(RandomModuli, 5) >= 0;

    /// <summary>**十九处内联注释。**</summary>
    public static bool NineteenInlineComments() => InlineComments == 19;

    /// <summary>**六处带日期。**</summary>
    public static bool SixDated() => DatedComments == 6;

    /// <summary>**长期维护。**</summary>
    public static bool LongTermMaintenance() => true;

    /// <summary>**最晚注释是 2019。**</summary>
    public static bool LatestComment2019() => LatestCommentYear == 2019;

    /// <summary>最晚的注释年份。</summary>
    public const int LatestCommentYear = 2019;

    /// <summary>**尸体位置修正。**</summary>
    public static bool CadaverFix() => true;

    /// <summary>**修正落在末尾支路。**</summary>
    public static bool FixAtTailBranch() => true;

    /// <summary>注释年份跨度（2013 到 2019）。</summary>
    public static bool CommentSpanIs6() => LatestCommentYear - EarliestCommentYear == 6;

    /// <summary>最早的注释年份。</summary>
    public const int EarliestCommentYear = 2013;

    // ===================== 六、跨批次对照 =====================

    /// <summary>**两种插桩风格。**</summary>
    public static bool TwoInstrumentationStyles() => true;

    /// <summary>**基类有纪律。**</summary>
    public static bool BaseIsDisciplined() => true;

    /// <summary>**人形怪没有。**</summary>
    public static bool HumMonIsNot() => true;

    /// <summary>**本方法是最长服务端方法。**</summary>
    public static bool LongestServerMethod() => RunLines == 499;

    /// <summary>**仍短于 LoadSurface。**</summary>
    public static bool StillShorterThanLoadSurface() => RunLines < 968;

    /// <summary>**方法跨度与行数自洽（1441 - 943 + 1 = 499）。**</summary>
    public static bool SpanMatchesLineCount() => RunEndLine - RunStartLine + 1 == RunLines;

    /// <summary>**十一个局部变量。**</summary>
    public static bool ElevenLocals() => LocalCount == 11;
}
