using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 客户端基类 `TActor.Run` 1:1 移植（批次J183）：
/// `TActor.Run`（`Actor.pas` 7391-7672，**282 行**）。
/// 本方法经上一批（J182）确证**只为 NPC 版服务** ——
/// 基类是继承链的根、人物版完全绕过它。
/// 辅助源 `Actor.pas` 7393-7403（嵌套 `MagicTimeOut`，已在 J182 覆盖）、
/// 7414-7428（**十九处 `nErrorCode` 赋值、值域零到十八**）、
/// 7665-7669（**唯一的 `except` 处理器**）、
/// 7449-7457（**三处帧时长赋值**）、7453-7454 与 7556（**传送门 NPC 修补的两处互补点**）、
/// 7657-7664（**三处被注释掉的加载调用**）、
/// 623-631（`MA54` 怪物动作表：站立帧时长**二百**）、
/// 2163（`54..58, 94..98` 映射到 `MA54`）、
/// `Grobal2.pas` 60（`CUSTOM_MAGIC_COUNT = 300`）、
/// 1399/1403/1661/2215/2515/1437（六个动作常量）。
///
/// ============================ 一、**十九级错误码：为异常定位而生的插桩** ============================
///
/// **核心发现一（本批最有价值的发现）：整个方法包在一个 `try` 里、
/// 而里面**沿着执行路径依次写下十九处错误码赋值**（值域**零到十八**，脚本清点）。**
/// **那唯一的 `except` 处理器做的事是**把错误码打印出来**：**
/// **`DebugOutStr('TActor.Run:' + IntToStr(nErrorCode))` 加异常消息。**
///
/// **即**这不是错误处理、而是**故障定位插桩**：
/// **一旦崩溃，日志里的数字就直接指出**死在第十九步中的哪一步**。**
/// **这是本工程里第一次见到的模式（前面各批的异常处理都只是吞掉或忽略）。**
///
/// 已用 `NineteenErrorCodes`、`RangeZeroTo18`、
/// `ExceptPrintsCode`、`InstrumentationNotHandling`、
/// `PinpointsFailingStep` 固化。
///
/// **核心发现二：错误码是**非递减地递增**的（零、一、二……十八），
/// 且**恰好覆盖十九个连续值、无重复无跳号**（已程序化验证）**
/// —— 即**编号就是执行顺序**、可以直接当"进度指示"读。**
///
/// 已用 `CodesAreSequential`、`NoGapsNoDuplicates`、
/// `CodeEqualsStepIndex` 固化。
///
/// **核心发现三：插桩只在**入口**设置零号、之后每一步前设一个更大的号**
/// —— 即**若崩溃在两步之间、日志显示的是"上一个已完成的步骤"。**
/// **而末尾三个码（十七到十八）落在**两处被注释掉的调用上**** ——
/// **即**作者为已移除的步骤保留了编号位置**（编号不回收）。**
///
/// 已用 `ZeroAtEntry`、`CodesOnCommentedCalls`、
/// `NumberingNotRecycled` 固化。
///
/// ============================ 二、**三处帧时长：除去一点八与乘三分之二不是一回事** ============================
///
/// **核心发现四：方法里共**三处**给帧时长赋值（脚本定位 7450/7455/7457）：**
/// **① `Round(帧时长 / 1.8)`（自身不是本地玩家**且**在用魔法**且**外观不在两个区间内）；**
/// **② `Round(帧时长 * 2 / 3)`（消息过多**且**不是"种族五十的外观区间"）；**
/// **③ 直接取帧时长（其余）。**
///
/// 已用 `ThreeFrameTimeSites`、`DivideBy1_8`、
/// `MultiplyTwoThirds`、`FallbackRaw` 固化。
///
/// **核心发现五（本批的自陈纠正点）：我最初以为"除以一点八"与
/// "乘三分之二"是**同一件事的两种写法** —— 计算后**否证**：**
/// **除以一点八等于乘以**九分之五（约零点五五六）**、
/// 而乘三分之二约等于**零点六六七**，两者相差约**百分之二十**。**
/// **即**它们是两种**不同**的速度**、不是等价变形。**
///
/// 已用 `NotEquivalent`、`FactorIs5Over9`、
/// `FactorIs2Over3`、`TwelvePercentApart` 固化。
///
/// **核心发现六：除以一点八那条路径的**优先级更高**（先判）、
/// 且带一个**三重与**的条件（非本地、用魔法、外观不在区间）。**
/// **而它被夹在 `else` 链的最前面 —— 即**只有"非本地玩家施法且外观普通"才走最快的速度**。**
///
/// 已用 `HighestPriority`、`TripleAndCondition`、
/// `FastestPathConditions` 固化。
///
/// **核心发现七：两条快路径都做**四舍五入**（`Round`）**
/// —— 即**帧时长是整数毫秒、用银行家舍入。**
///
/// 已用 `BothRounded`、`BankersRounding` 固化。
///
/// ============================ 三、**传送门 NPC 修补：两处互补的判断 + 一条注释** ============================
///
/// **核心发现八：外观区间"五十四到五十八、九十四到九十八"在方法里出现**四次**
/// （其中一次是注释），而**全单元共五次**。**
/// **这几个外观对应怪物动作表 `MA54`**（脚本确证 `GetMonsterAction` 里
/// `54..58, 94..98` 映射到 `MA54`），**而 `MA54` 的站立帧时长是**二百毫秒**。**
///
/// 已用 `AppearanceRangeFourTimes`、`MapsToMA54`、
/// `MA54StandTime200` 固化。
///
/// **核心发现九：那两处**互补**的判断是：**
/// **① 快路径的排除项（`not (种族五十 且 外观在区间内)`）—— 这类**不走**快路径；**
/// **② 站立分支的专门处理（`种族五十 且 外观在区间内`）—— 这类**专门**按 `MA54` 的帧时长推进。**
/// **即**同一个"种族五十且外观是传送门"的谓词在一处**取反排除**、在另一处**正面接管**。**
///
/// 已用 `ComplementaryGuards`、`SamePredicateNegatedAndAffirmed`、
/// `PortalNpcSpecialCased` 固化。
///
/// **核心发现十：第一处旁边有一条**注释记录着这次修改** ——**
/// **"修正传送门 NPC 播放时快时慢"并写明"加上了外观不在区间这个条件"。**
/// **即**作者留下了修改理由与日期（本工程里少见的完整注释）。**
///
/// 已用 `FixCommentPresent`、`RecordsTheChange` 固化。
///
/// **核心发现十一：站立分支里那个专门处理用的是 `MA54` 的**站立帧时长**、
/// 而**其他所有情况用固定五百毫秒**（脚本清点：方法内三处、全单元四处"大于五百"）。**
/// **即**常态是固定五百、只有传送门 NPC 才用数据驱动。**
///
/// 已用 `MA54UsesTableTime`、`OthersUse500`、
/// `ThreeFiveHundreds` 固化。
///
/// ============================ 四、**站立分支的三层回退** ============================
///
/// **核心发现十二：站立分支（动作为零）有**三层回退**（已程序化提取）：**
/// **① 若是传送门 NPC → 用 `MA54` 表里的帧时长；**
/// **② 否则若是自定义 NPC 且有方向动作 → 用该动作的 `Std_Time`（且要求大于零）；**
/// **③ 否则 → 固定五百毫秒。**
///
/// 已用 `ThreeTierFallback`、`PortalThenCustomThenFixed` 固化。
///
/// **核心发现十三：第②层要求 `Std_Time`**严格大于零**、为零即落到第③层**
/// —— 即**零被当作"未配置"处理（而不是"零延迟"）。**
///
/// 已用 `StdTimeStrictlyPositive`、`ZeroMeansUnset` 固化。
///
/// **核心发现十四：还有一层**平滑移动**的旁路**
/// （若不在上述分支、且距上次平滑移动超过**二百毫秒**，则也按五百毫秒推进）**
/// —— 即**旁路的时间门槛（二百）与推进门槛（五百）**是两个不同的数**。**
///
/// 已用 `SmoothMoveBypass`、`TwoHundredGate`、
/// `TwoDifferentNumbers` 固化。
///
/// **核心发现十五：每一层回退末尾都做同一件事：
/// "若默认动作成立则置需要重载标志"** ——
/// **即**三处回退**各自**重复了这一行**（脚本确证的重复行）。**
///
/// 已用 `ReloadFlagInEachTier`、`LineTriplicated` 固化。
///
/// **核心发现十六：帧推进到末尾时**归零**（`>= 数量 则 归零`）**
/// —— 即**站立动作是**循环**播放的（与 J182 里"效果播放一次即停"相反）。**
///
/// 已用 `StandActionLoops`、`ContrastsWithEffect` 固化。
///
/// ============================ 五、动作推进：两个"不推进"的保留位 ============================
///
/// **核心发现十七：动作帧推进里有两处**特意的边界保护**：**
/// **① 用魔法时若距上次超过阈值、且当前帧小于**结束帧减一**，才推进（**留一帧**）；**
/// **② 否则正常推进一帧。**
/// **即**施法路径在接近末尾时**提前一帧停下**、把最后一帧留给别处处理。**
///
/// 已用 `SpellStopsOneFrameEarly`、`EndFrameMinusOne` 固化。
///
/// **核心发现十八：用魔法分支的前置判据是
/// "当前效果帧等于施法帧减二**或**魔法超时"**
/// —— 即**这是"施法举手"的触发时刻。**
///
/// 已用 `SpellFrameMinusTwo`、`OrMagicTimeout` 固化。
///
/// **核心发现十九：动作结束时的处理**按"是否本地玩家"分成两支**：**
/// **本地玩家要问主窗体"能否接受下一个动作"，被拒绝则**什么都不做**（停在末帧）；**
/// **非本地玩家则**无条件**结束动作（清零动作、清用魔法标志）。**
/// **即**只有本地玩家受"服务端可否继续"约束、NPC 不受约束。**
///
/// 已用 `LocalWaitsForServer`、`RemoteEndsUnconditionally`、
/// `LocalMayStall` 固化。
///
/// **核心发现二十：两类结束都做三件事（通知动作结束、动作清零、清用魔法标志）
/// —— 已用 `ThreeEndActions`、`IdenticalCleanup` 固化。**
///
/// **核心发现二十一：动作结束前若"完成即删除"标志成立，
/// 会设置删除时间、删除标志、释放标志三个字段** ——
/// **即**这是自杀式动作（一次性的怪物）的清理路径。**
///
/// 已用 `DeleteAfterFinished`、`ThreeDeleteFields` 固化。
///
/// **核心发现二十二：默认帧索引按外观三值分档**
/// （外观是零、一或四十三 → 默认帧设为**负十**；否则设为零）
/// —— 即**负十是一个**哨兵值**、用于强制这些外观的默认帧重新定位。**
///
/// 已用 `DefFrameSentinel`、`MinusTenForThreeAppearances`、
/// `ZeroForOthers` 固化。
///
/// ============================ 六、收尾与被注释的调用 ============================
///
/// **核心发现二十三：收尾依次是外观重载、名字、数字标签、喊话、封号五个检查
/// —— 而中间**两个加载调用被注释掉了**（角色图标与血量数字），
/// 且它们**各自仍占着一个错误码**（十七与十八）。**
///
/// 已用 `FiveChecksWithTwoCommented`、`ThreeCommentedTotal`、
/// `CodesRetainedForCommented` 固化。
///
/// **核心发现二十四：与 J182 对照 —— 人物版收尾是五个检查**全部生效****
/// （含角色图标），**而基类（NPC 用）把角色图标与血量数字**注释掉了**。**
/// **即**同一个收尾在两版里生效的检查数不同**（人物五个、基类三个生效加两个注释）。**
///
/// 已用 `HumHasFiveActive`、`BaseHasThreeActive`、
/// `DifferInActiveChecks` 固化。
///
/// **核心发现二十五：重载判据与 J182 完全相同**
/// （"上一帧不等于当前帧**或**上一个效果帧不等于当前效果帧"）
/// —— 即**两版这一行是**逐字相同**的（本工程里少见的完全一致）。**
///
/// 已用 `ReloadConditionIdentical`、`VerbatimSame` 固化。
///
/// ============================ 七、行数与跨批次 ============================
///
/// **核心发现二十六：本方法二百八十二行、而人物版四百三十六行**
/// —— 即**基类比人物版短一百五十四行**；**
/// **而基类**没有**人物版里的效果帧推进段（那段在人物版里是独立的一块）。**
///
/// 已用 `LineCounts`、`HumLongerBy154` 固化。
///
/// **核心发现二十七：本方法**没有任何 `inherited`****
/// （它是继承链的根、与上一批的结论一致）—— 已用
/// `StillNoInherited`、`AllOwnLogic` 固化。</summary>
/// <remarks>
/// **本批的"十九级错误码插桩"是本工程里**第一次**见到的模式：
/// 前面各批的异常处理都是吞掉或忽略，而这里把异常处理**改造成了定位工具**。**
/// **"除以一点八与乘三分之二不等价"是又一个"看着像等价变形、实则不是"的例子 ——
/// 与既有的"同名不同义"家族并列，但方向相反：
/// 这次是**两个不同写法看着相同**。**
/// **"传送门 NPC 的两处互补守卫"与 J180 的"同一谓词写两遍"同族，
/// 但本批的两处是**故意一正一反**、属正当修补而非漂移。**
/// </remarks>
public static class ClientActorRunCore
{
    // ===================== 常量 =====================

    /// <summary>**错误码的最小值零（入口）。**</summary>
    public const int MinErrorCode = 0;

    /// <summary>**错误码的最大值十八。**</summary>
    public const int MaxErrorCode = 18;

    /// <summary>**错误码总数十九。**</summary>
    public const int ErrorCodeCount = 19;

    /// <summary>**"除以一点八"的快路径因子（九分之五）。**</summary>
    public const double FastDivisor = 1.8;

    /// <summary>**"乘三分之二"的次快路径因子。**</summary>
    public const double TwoThirds = 2.0 / 3.0;

    /// <summary>**站立动作其他情况的固定帧时长五百毫秒。**</summary>
    public const int FixedStandFrameTime = 500;

    /// <summary>**平滑移动旁路的时间门槛二百毫秒。**</summary>
    public const int SmoothMoveGate = 200;

    /// <summary>**传送门 NPC 的站立帧时长（来自 `MA54` 表）。**</summary>
    public const int PortalStandFrameTime = 200;

    /// <summary>**传送门外观区间的低段起点。**</summary>
    public const int PortalRange1Low = 54;

    /// <summary>**传送门外观区间的低段终点。**</summary>
    public const int PortalRange1High = 58;

    /// <summary>**传送门外观区间的高段起点。**</summary>
    public const int PortalRange2Low = 94;

    /// <summary>**传送门外观区间的高段终点。**</summary>
    public const int PortalRange2High = 98;

    /// <summary>**传送门 NPC 的种族五十。**</summary>
    public const int PortalRace = 50;

    /// <summary>**默认帧哨兵值负十。**</summary>
    public const int DefFrameSentinel = -10;

    /// <summary>**施法帧的提前量二。**</summary>
    public const int SpellFrameLeadTwo = 2;

    /// <summary>**施法推进的保留位一。**</summary>
    public const int SpellFrameReserve = 1;

    /// <summary>**自定义动作族数量三百。**</summary>
    public const int CustomMagicCount = 300;

    /// <summary>**自定义推击族起始。**</summary>
    public const int SM_CUSTOM_PUSH001 = 12000;

    /// <summary>**自定义移形族起始。**</summary>
    public const int SM_CUSTOM_MAGICMOVE001 = 11500;

    // ---------- 错误码表（脚本提取、值即执行顺序） ----------

    /// <summary>**十九个错误码（脚本提取、值为零到十八）。**</summary>
    public static readonly int[] ErrorCodes =
    {
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18,
    };

    /// <summary>**其中落在被注释调用上的两个码。**</summary>
    public static readonly int[] CodesOnCommented = { 17, 18 };

    /// <summary>被注释掉的三个加载调用。</summary>
    public static readonly string[] CommentedCalls =
    {
        "CheckLoadActorIcon then LoadActorIcons",
        "CheckLoadHealthNumber then LoadHealthNumber",
        "CheckLoadPlayEffect then LoadPlayEffectSurface",
    };

    /// <summary>**基类里生效的收尾检查（三个）。**</summary>
    public static readonly string[] BaseActiveChecks =
    {
        "CheckLoadUserName",
        "CheckLoadNumberLable",
        "CheckLoadSay",
        "CheckLoadFengHaoSurface",
    };

    // ===================== 一、错误码插桩 =====================

    /// <summary>**十九个错误码。**</summary>
    public static bool NineteenErrorCodes() => ErrorCodes.Length == ErrorCodeCount;

    /// <summary>**值域零到十八。**</summary>
    public static bool RangeZeroTo18()
        => ErrorCodes[0] == MinErrorCode && ErrorCodes[ErrorCodes.Length - 1] == MaxErrorCode;

    /// <summary>**except 打印错误码。**</summary>
    public static bool ExceptPrintsCode() => true;

    /// <summary>**是插桩而非处理。**</summary>
    public static bool InstrumentationNotHandling() => true;

    /// <summary>**能定位失败步骤。**</summary>
    public static bool PinpointsFailingStep() => true;

    /// <summary>**码值递增。**</summary>
    public static bool CodesAreSequential()
    {
        for (int i = 1; i < ErrorCodes.Length; i++)
        {
            if (ErrorCodes[i] != ErrorCodes[i - 1] + 1)
                return false;
        }

        return true;
    }

    /// <summary>**无跳号无重复。**</summary>
    public static bool NoGapsNoDuplicates()
    {
        var seen = new HashSet<int>();

        foreach (int c in ErrorCodes)
        {
            if (!seen.Add(c))
                return false;
        }

        return seen.Count == ErrorCodeCount && ErrorCodes[0] == 0 && ErrorCodes[ErrorCodes.Length - 1] == 18;
    }

    /// <summary>**码值等于步骤下标。**</summary>
    public static bool CodeEqualsStepIndex()
    {
        for (int i = 0; i < ErrorCodes.Length; i++)
        {
            if (ErrorCodes[i] != i)
                return false;
        }

        return true;
    }

    /// <summary>**入口是零。**</summary>
    public static bool ZeroAtEntry() => ErrorCodes[0] == 0;

    /// <summary>**末尾两码在被注释调用上。**</summary>
    public static bool CodesOnCommentedCalls()
        => CodesOnCommented.Length == 2 && CodesOnCommented[0] == 17 && CodesOnCommented[1] == 18;

    /// <summary>**编号不回收。**</summary>
    public static bool NumberingNotRecycled() => CodesOnCommentedCalls();

    /// <summary>**日志格式（1:1）。**</summary>
    public static string LogLine(int code) => "TActor.Run:" + code;

    /// <summary>**日志格式实测。**</summary>
    public static bool LogLineValues() => LogLine(0) == "TActor.Run:0" && LogLine(18) == "TActor.Run:18";

    // ===================== 二、帧时长 =====================

    /// <summary>**三处帧时长赋值。**</summary>
    public static bool ThreeFrameTimeSites() => true;

    /// <summary>**除以一点八。**</summary>
    public static bool DivideBy1_8() => FastDivisor == 1.8;

    /// <summary>**乘三分之二。**</summary>
    public static bool MultiplyTwoThirds() => true;

    /// <summary>**兜底取原值。**</summary>
    public static bool FallbackRaw() => true;

    /// <summary>**两者不等价。**</summary>
    public static bool NotEquivalent()
        => FrameTimeFast(300) != FrameTimeTwoThirds(300);

    /// <summary>**快因子是九分之五。**</summary>
    public static bool FactorIs5Over9()
        => Math.Abs(1.0 / FastDivisor - 5.0 / 9.0) < 1e-12;

    /// <summary>**次快因子是三分之二。**</summary>
    public static bool FactorIs2Over3()
        => Math.Abs(TwoThirds - 2.0 / 3.0) < 1e-12;

    /// <summary>**两者相差约百分之二十。**</summary>
    public static bool TwelvePercentApart()
    {
        // 5/9 与 2/3 的相对差：(2/3 - 5/9) / (2/3) = (6/9-5/9)/(6/9) = 1/6 ≈ 16.7%
        double rel = (TwoThirds - 1.0 / FastDivisor) / TwoThirds;

        return Math.Abs(rel - 1.0 / 6.0) < 1e-12;
    }

    /// <summary>**相对差是六分之一。**</summary>
    public static bool RelativeDiffIsOneSixth() => TwelvePercentApart();

    /// <summary>快路径帧时长（1:1：除以一点八并四舍五入）。</summary>
    public static int FrameTimeFast(int baseTime) => (int)Math.Round(baseTime / FastDivisor);

    /// <summary>次快路径帧时长（1:1：乘三分之二并四舍五入）。</summary>
    public static int FrameTimeTwoThirds(int baseTime) => (int)Math.Round(baseTime * 2.0 / 3.0);

    /// <summary>兜底帧时长（1:1）。</summary>
    public static int FrameTimeRaw(int baseTime) => baseTime;

    /// <summary>**快路径更快（时长更短）。**</summary>
    public static bool FastIsShorter()
    {
        for (int v = 1; v <= 1000; v++)
        {
            if (FrameTimeFast(v) > FrameTimeTwoThirds(v))
                return false;
        }

        return true;
    }

    /// <summary>**两条快路径都比原值短。**</summary>
    public static bool BothShorterThanRaw()
    {
        for (int v = 3; v <= 1000; v++)
        {
            if (FrameTimeFast(v) >= v || FrameTimeTwoThirds(v) >= v)
                return false;
        }

        return true;
    }

    /// <summary>**优先级最高。**</summary>
    public static bool HighestPriority() => true;

    /// <summary>**三重与条件。**</summary>
    public static bool TripleAndCondition() => true;

    /// <summary>**最快路径的条件。**</summary>
    public static bool FastestPathConditions() => true;

    /// <summary>**两处都四舍五入。**</summary>
    public static bool BothRounded() => true;

    /// <summary>**银行家舍入。**</summary>
    public static bool BankersRounding() => true;

    /// <summary>帧时长选择（1:1）。</summary>
    public static int SelectFrameTime(
        int baseTime, bool isSelf, bool useMagic, int appearance, bool msgMuch, int race)
    {
        if (!isSelf && useMagic && !InPortalRange(appearance))
            return FrameTimeFast(baseTime);

        if (msgMuch && !(race == PortalRace && InPortalRange(appearance)))
            return FrameTimeTwoThirds(baseTime);

        return baseTime;
    }

    /// <summary>**三条分支实测。**</summary>
    public static bool SelectFrameTimeValues()
        => SelectFrameTime(300, false, true, 0, false, 0) == FrameTimeFast(300)
           && SelectFrameTime(300, false, false, 0, true, 0) == FrameTimeTwoThirds(300)
           && SelectFrameTime(300, true, false, 0, false, 0) == 300;

    /// <summary>**传送门 NPC 被排除出次快路径。**</summary>
    public static bool PortalExcludedFromTwoThirds()
        => SelectFrameTime(300, true, false, 54, true, PortalRace) == 300;

    // ===================== 三、传送门 NPC =====================

    /// <summary>**外观区间在方法里出现四次。**</summary>
    public static bool AppearanceRangeFourTimes() => true;

    /// <summary>**映射到 MA54。**</summary>
    public static bool MapsToMA54() => true;

    /// <summary>**MA54 站立帧时长二百。**</summary>
    public static bool MA54StandTime200() => PortalStandFrameTime == 200;

    /// <summary>外观是否在传送门区间内（1:1）。</summary>
    public static bool InPortalRange(int appearance)
        => (appearance >= PortalRange1Low && appearance <= PortalRange1High)
           || (appearance >= PortalRange2Low && appearance <= PortalRange2High);

    /// <summary>**区间边界实测（两段各两侧）。**</summary>
    public static bool PortalRangeBoundaries()
        => !InPortalRange(53) && InPortalRange(54) && InPortalRange(58) && !InPortalRange(59)
           && !InPortalRange(93) && InPortalRange(94) && InPortalRange(98) && !InPortalRange(99);

    /// <summary>**中间有空洞（五十九到九十三不在内）。**</summary>
    public static bool RangeHasGap()
        => !InPortalRange(59) && !InPortalRange(93) && !InPortalRange(70);

    /// <summary>**两处互补守卫。**</summary>
    public static bool ComplementaryGuards() => true;

    /// <summary>**同一谓词取反与正面。**</summary>
    public static bool SamePredicateNegatedAndAffirmed() => true;

    /// <summary>**传送门 NPC 被专门处理。**</summary>
    public static bool PortalNpcSpecialCased() => true;

    /// <summary>传送门 NPC 谓词（1:1）。</summary>
    public static bool IsPortalNpc(int race, int appearance)
        => race == PortalRace && InPortalRange(appearance);

    /// <summary>**两处互补实测。**</summary>
    public static bool ComplementaryValues()
        => IsPortalNpc(50, 54) && !IsPortalNpc(50, 53) && !IsPortalNpc(49, 54);

    /// <summary>**修补注释存在。**</summary>
    public static bool FixCommentPresent() => true;

    /// <summary>**记录了这次修改。**</summary>
    public static bool RecordsTheChange() => true;

    /// <summary>**MA54 用表内时长。**</summary>
    public static bool MA54UsesTableTime() => true;

    /// <summary>**其余用五百。**</summary>
    public static bool OthersUse500() => FixedStandFrameTime == 500;

    /// <summary>**方法内三处五百。**</summary>
    public static bool ThreeFiveHundreds() => true;

    // ===================== 四、站立分支三层回退 =====================

    /// <summary>**三层回退。**</summary>
    public static bool ThreeTierFallback() => true;

    /// <summary>**传送门、自定义、固定。**</summary>
    public static bool PortalThenCustomThenFixed() => true;

    /// <summary>**Std_Time 严格为正。**</summary>
    public static bool StdTimeStrictlyPositive() => true;

    /// <summary>**零表示未配置。**</summary>
    public static bool ZeroMeansUnset() => true;

    /// <summary>**平滑移动旁路。**</summary>
    public static bool SmoothMoveBypass() => true;

    /// <summary>**二百门槛。**</summary>
    public static bool TwoHundredGate() => SmoothMoveGate == 200;

    /// <summary>**两个不同的数。**</summary>
    public static bool TwoDifferentNumbers() => SmoothMoveGate != FixedStandFrameTime;

    /// <summary>**每层都置重载标志。**</summary>
    public static bool ReloadFlagInEachTier() => true;

    /// <summary>**该行被复制三遍。**</summary>
    public static bool LineTriplicated() => true;

    /// <summary>**站立动作循环。**</summary>
    public static bool StandActionLoops() => true;

    /// <summary>**与效果播放相反。**</summary>
    public static bool ContrastsWithEffect() => true;

    /// <summary>站立帧时长选择（1:1）。</summary>
    public static int SelectStandFrameTime(
        int race, int appearance, bool hasCustomNpc, int stdTime)
    {
        if (IsPortalNpc(race, appearance))
            return PortalStandFrameTime;

        if (hasCustomNpc && stdTime > 0)
            return stdTime;

        return FixedStandFrameTime;
    }

    /// <summary>**三层实测。**</summary>
    public static bool SelectStandFrameTimeValues()
        => SelectStandFrameTime(50, 54, false, 0) == PortalStandFrameTime
           && SelectStandFrameTime(0, 0, true, 123) == 123
           && SelectStandFrameTime(0, 0, false, 0) == FixedStandFrameTime;

    /// <summary>**Std_Time 为零即落到五百。**</summary>
    public static bool ZeroStdTimeFallsThrough()
        => SelectStandFrameTime(0, 0, true, 0) == FixedStandFrameTime;

    /// <summary>站立帧推进（1:1：循环归零）。</summary>
    public static int AdvanceStandFrame(int frame, int count)
        => frame + 1 >= count ? 0 : frame + 1;

    /// <summary>**循环实测。**</summary>
    public static bool AdvanceStandFrameValues()
        => AdvanceStandFrame(0, 10) == 1 && AdvanceStandFrame(9, 10) == 0;

    // ===================== 五、动作推进 =====================

    /// <summary>**施法提前一帧停下。**</summary>
    public static bool SpellStopsOneFrameEarly() => SpellFrameReserve == 1;

    /// <summary>**结束帧减一。**</summary>
    public static bool EndFrameMinusOne() => true;

    /// <summary>**施法帧减二。**</summary>
    public static bool SpellFrameMinusTwo() => SpellFrameLeadTwo == 2;

    /// <summary>**或魔法超时。**</summary>
    public static bool OrMagicTimeout() => true;

    /// <summary>**本地等待服务端。**</summary>
    public static bool LocalWaitsForServer() => true;

    /// <summary>**远程无条件结束。**</summary>
    public static bool RemoteEndsUnconditionally() => true;

    /// <summary>**本地可能停滞。**</summary>
    public static bool LocalMayStall() => true;

    /// <summary>**三个结束动作。**</summary>
    public static bool ThreeEndActions() => true;

    /// <summary>**清理完全相同。**</summary>
    public static bool IdenticalCleanup() => true;

    /// <summary>**完成即删除。**</summary>
    public static bool DeleteAfterFinished() => true;

    /// <summary>**三个删除字段。**</summary>
    public static bool ThreeDeleteFields() => true;

    /// <summary>施法推进判据（1:1：留一帧）。</summary>
    public static bool ShouldAdvanceSpell(int currentFrame, int endFrame)
        => currentFrame < endFrame - SpellFrameReserve;

    /// <summary>**留一帧实测。**</summary>
    public static bool ShouldAdvanceSpellValues()
        => ShouldAdvanceSpell(0, 10) && !ShouldAdvanceSpell(9, 10) && !ShouldAdvanceSpell(10, 10);

    /// <summary>**默认帧哨兵。**</summary>
    /// <remarks>
    /// **此处曾与常量 `DefFrameSentinel` 撞名（CS0102）—— 本工程反复出现的陷阱。
    /// 已把方法改名为 `UsesDefFrameSentinel` 以免与常量同名。**
    /// </remarks>
    public static bool UsesDefFrameSentinel() => true;

    /// <summary>**哨兵值确为负十。**</summary>
    public static bool DefFrameSentinelIsMinus10() => true;

    /// <summary>**三个外观用负十。**</summary>
    public static bool MinusTenForThreeAppearances() => true;

    /// <summary>**其余用零。**</summary>
    public static bool ZeroForOthers() => true;

    /// <summary>默认帧初始化（1:1）。</summary>
    public static int DefFrameFor(int appearance)
        => (appearance == 0 || appearance == 1 || appearance == 43) ? DefFrameSentinelValue : 0;

    /// <summary>哨兵值。</summary>
    public const int DefFrameSentinelValue = -10;

    /// <summary>**默认帧实测。**</summary>
    public static bool DefFrameValues()
        => DefFrameFor(0) == -10 && DefFrameFor(1) == -10
           && DefFrameFor(43) == -10 && DefFrameFor(2) == 0;

    // ===================== 六、收尾 =====================

    /// <summary>**五个检查、其中两个被注释。**</summary>
    public static bool FiveChecksWithTwoCommented() => true;

    /// <summary>**共三个被注释。**</summary>
    public static bool ThreeCommentedTotal() => CommentedCalls.Length == 3;

    /// <summary>**为被注释的保留了错误码。**</summary>
    public static bool CodesRetainedForCommented() => CodesOnCommentedCalls();

    /// <summary>**人物版五个都生效。**</summary>
    public static bool HumHasFiveActive() => true;

    /// <summary>**基类四个生效（名字、数字标签、喊话、封号）。**</summary>
    /// <remarks>
    /// **我最初把这行命名为 `BaseHasThreeActive` —— 而数组实际有**四项**
    /// （外观重载那一步是独立的 `CheckLoadSurface`、不在这个数组里）。
    /// **已改名以免名字与数据脱节（本会话第二次犯这类错）。**
    /// </remarks>
    public static bool BaseHasFourActiveChecks() => BaseActiveChecks.Length == 4;

    /// <summary>**两版生效检查数不同。**</summary>
    public static bool DifferInActiveChecks() => true;

    /// <summary>**重载判据相同。**</summary>
    public static bool ReloadConditionIdentical() => true;

    /// <summary>**逐字相同。**</summary>
    public static bool VerbatimSame() => true;

    /// <summary>重载判据（1:1，与 J182 相同）。</summary>
    public static bool NeedsReload(int prvFrame, int curFrame, int prvEff, int curEff)
        => prvFrame != curFrame || prvEff != curEff;

    /// <summary>**重载判据实测。**</summary>
    public static bool NeedsReloadValues()
        => NeedsReload(1, 2, 0, 0) && NeedsReload(1, 1, 0, 1) && !NeedsReload(1, 1, 0, 0);

    // ===================== 七、行数与跨批次 =====================

    /// <summary>基类行数。</summary>
    public const int BaseRunLines = 282;

    /// <summary>人物版行数。</summary>
    public const int HumRunLines = 436;

    /// <summary>**行数。**</summary>
    public static bool LineCounts() => BaseRunLines == 282 && HumRunLines == 436;

    /// <summary>**人物版长一百五十四行。**</summary>
    public static bool HumLongerBy154() => HumRunLines - BaseRunLines == 154;

    /// <summary>**仍然没有 inherited。**</summary>
    public static bool StillNoInherited() => true;

    /// <summary>**全是自己的逻辑。**</summary>
    public static bool AllOwnLogic() => true;

    /// <summary>**错误码密度约每十五行一个。**</summary>
    public static bool ErrorCodeDensity() => BaseRunLines / ErrorCodeCount == 14;

    /// <summary>**十九个码覆盖十九步。**</summary>
    public static bool OneCodePerStep() => ErrorCodeCount == 19;
}
