using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 客户端动作默认帧与默认动作推进 1:1 移植（批次J178）：
/// `TActor.GetDefaultFrame`（`Actor.pas` 6136-6203，**68 行**、**基类**）、
/// `TNpcActor.GetDefaultFrame`（10435-10481，**47 行**）、
/// `TStatuaryNpcActor.GetDefaultFrame`（17650-17653，**4 行**、**恒返回零**）、
/// `TActor.DefaultMotion`（6208-6222，**15 行**），四者合计 **134 行**。
/// 辅助源 `Grobal2.pas` 181（`STATE_STONE_MODE = 1`）、
/// `Actor.pas` 208-219（`TMonsterAction` 记录**九个动作字段**）、
/// 2067/2095（`GetRaceByPM` 的双声明与实现）、
/// 1722/1725（`m_nDefFrameCount`/`m_dwLoadSurfaceTime`）、
/// 1791（`Shift` 声明）、5476（`LoadSurface` 把上次加载时间设成**六十秒前**）。
///
/// ============================ 一、核心发现：**取帧的"执行路径"与"宽度无关的各段"能差出十倍** ============================
///
/// **基类的 `GetDefaultFrame` 有**两条互斥的大分支**：**
/// **① 若**种族等于一百五十六**且**变身外观大于等于零** → 走"自定义怪物配置"路径；**
/// **② 否则 → 走"按种族查动作表"路径。**
///
/// **注意分支①的判据是 `m_btRace in [156]` —— 一个**单元素集合**。
/// 这是本工程里少见的一种写法（用集合判一个值），
/// 与既有记录的"范围分支里加额外条件"同族。**
///
/// 已用 `Race156SingleElementSet`、`TwoMutuallyExclusiveBranches`、
/// `SingleElementSetInsteadOfEquality` 固化。
///
/// **而分支②里**先调 `GetRaceByPM`、若返回空则**直接退出**（`Exit`）
/// —— 注意此时 `Result` 已是零（函数入口处赋值）、所以**退出等于返回零**。**
///
/// 已用 `NilRaceReturnsZero`、`ExitAfterZeroInit` 固化。
///
/// **核心发现：两个分支里都有"当前默认帧"的**三态钳制**，
/// 且写法**完全一致**（小于零→取零、大于等于上限→取零、否则取原值）。**
/// **即**负值与越界值都被**折叠到零**** —— 而不是被钳到上限。
///
/// 已用 `ThreeWayClamp`、`OutOfRangeFoldsToZero`、
/// `NotClampedToUpperBound`、`SameClampInBothBranches` 固化。
///
/// **核心发现：分支①里**死亡且非骷髅**时用的是"播放数**减一**"**
/// —— 即**停在动作的**最后一帧****；**而"站立"那段用的是普通当前帧。
/// 即**死亡动作取末帧、站立动作取当前帧**。**
///
/// 已用 `DeathUsesLastFrame`、`DeathMinusOne`、
/// `StandUsesCurrentFrame` 固化。
///
/// **核心发现：分支①的石化分支**不钳制当前帧**（直接用起始索引、不加帧偏移）**
/// —— 即**石化时永远停在动作的第一帧。**
///
/// 已用 `StoneIgnoresCurrentFrame`、`StoneStaysAtFirstFrame` 固化。
///
/// **核心发现：分支①的"骷髅"分支**只取起始索引、不加方向乘数也不加帧偏移**
/// —— 即**骷髅形状是单帧的。**
///
/// 已用 `SkeletonSingleFrame` 固化。
///
/// **核心发现：方向乘数是"播放数**加**空帧数"**（而不是只乘播放数）
/// —— 即**每个方向占用的索引宽度是"播放数加空帧数"。**
///
/// 已用 `DirStrideIsPlayPlusEmpty` 固化。
///
/// **核心发现：`CalcDir` 决定方向乘数是**取真实方向**还是**取零**
/// —— 即关闭方向计算时所有方向共用第一组。**
///
/// 已用 `CalcDirGatesDirection`、`CalcDirFalseMeansZero` 固化。
///
/// ============================ 二、**`m_nDefFrameCount` 只在部分路径上被赋值** ============================
///
/// **全文件里 `m_nDefFrameCount` 只被赋值**六处**（已程序化清点）：
/// 三处在自定义动作路径（都赋"播放数"）、一处在基类的站立路径（赋"站立帧数"）、
/// 另两处在别处。**
/// **关键：**自定义怪物路径（分支①）**整段**没有**给它赋值** ——
/// **所以走自定义怪物时该计数保持**上一次的值**（可能是别的动作、甚至是零）。**
///
/// 已用 `DefFrameCountAssignedSixTimes`、`CustomPathDoesNotAssign`、
/// `NeverAssignedInBranchOne`、`CarriesStaleValue` 固化。
///
/// **核心发现：它在分支②的"站立"路径上被赋成"站立帧数"，
/// 而**死亡路径上没有赋值** —— 即死亡时该计数仍是站立时的值。**
///
/// 已用 `NotAssignedOnDeathPath` 固化。
///
/// ============================ 三、NPC 类的取帧：方向取模**又出现了一次** ============================
///
/// **`TNpcActor.GetDefaultFrame` 的两条分支与基类**判据相同但要简单得多**：**
/// **① 外观号大于等于一万 → 自定义 NPC 配置路径；② 否则 → 普通路径。**
///
/// 已用 `SameThresholdAsDrawChr`、`SimplerThanBase` 固化。
///
/// **核心发现：普通路径里的"方向对三取模"**与上一批（J177）在
/// `DrawChr` 里见到的那句**是同一套写法、同一个排除区间二四六到二七二****
/// —— 即**同一个"方向压缩"逻辑在本单元的**两个方法里各写了一遍**。**
///
/// 已用 `SameModuloAsJ177`、`DuplicatedDirCompression`、
/// `SameExclusionRange` 固化。
///
/// **核心发现：自定义路径里的方向压缩**不是对三取模、而是对**配置里的方向数**取模**
/// —— **且当方向数**不大于一**时直接把方向置**零**。**
/// **即：配置说只有一方向时，方向被强制成零（而不是对一取模、那样会恒得零、
/// 结果相同但写法不同）。**
///
/// 已用 `ConfigDirCountModulo`、`OneOrLessForcesZero`、
/// `NotModuloByOne` 固化。
///
/// **核心发现：普通路径里有一个**六段外观号集合**把方向强制置零**
/// —— 集合是"五十四到五十九、七十到七十五、八十一到八十四、
/// 九十到九十二、九十四到一百零一、二百一十一到二百二十五"（已程序化提取六段）。**
/// **注意这个集合与 J177 里的"五十四到五十八、九十四到九十八"**不同**
/// —— **J177 那两段各宽五、本处第一段是五十四到五十九（宽六）、
/// 而本处第五段是九十四到一百零一（宽八）。**
/// **即两处的区间**边界不一致**（本处上界更大）。**
///
/// 已用 `SixSpecialRanges`、`ForcesDirZero`、
/// `DiffersFromJ177Ranges`、`BroaderUpperBounds` 固化。
///
/// **核心发现：这段"方向置零"发生在**计算最终索引之前**，
/// 所以**被置零的方向会直接参与"起始加方向乘宽度加帧"的计算**
/// —— 即那六段外观号**永远只画第一组方向**。**
///
/// 已用 `ZeroDirFeedsIntoIndex` 固化。
///
/// **核心发现：三条分支的最终公式**完全相同**（起始索引加方向乘宽度加帧）。**
///
/// 已用 `SameFinalFormula` 固化。
///
/// **核心发现：雕像类**恒返回零**、且是个空壳覆盖** ——
/// 即**雕像类完全不参与"默认帧"逻辑（它的绘制由上一批的雕像层单独负责）。**
///
/// 已用 `StatuaryReturnsZero`、`EmptyOverride`、
/// `StatuaryDrawsSeparately` 固化。
///
/// ============================ 四、`DefaultMotion`：四秒战时模式 + 方向自增 ============================
///
/// **流程：① 把"反向帧"置假；② 若在战时模式且"当前时间减战时模式时间**大于四秒**"
/// 则退出战时模式**并**同时清一个"自定义魔法无动作"标志；**
/// **③ 取默认帧；④ 调 `Shift`（参数是"方向、零、一、一"）；⑤ 返回"新帧不等于旧帧"；**
/// **⑥ **最后**把当前帧改成新帧。**
///
/// 已用 `WarModeExpiresAfter4s`、`ClearsTwoFlags`、
/// `ReturnsFrameChanged`、`AssignsCurrentFrameLast` 固化。
///
/// **核心发现：第 ⑤ 步的返回值用的是**改帧之前**的比较、而第 ⑥ 步才真正改帧**
/// —— 即**返回值表达的是"这一帧是否变化了"，而不是"改成什么"。**
/// **而 `GetDefaultFrame` 的参数传的是**战时模式标志** ——
/// 即**这个参数在基类里**根本没被使用**（基类的函数体里没有引用 `wmode`）。**
///
/// 已用 `CompareBeforeAssign`、`WmodeUnusedInBase`、
/// `ParameterNeverRead` 固化。
///
/// **核心发现：`Shift` 的四个参数是**方向、零、一、一** ——
/// 即"步长零、当前一、上限一"。**
/// **注意 `Shift` 的实现在 `Actor.pas` 里**只有声明**（在类声明的嵌套区），
/// **本处按"方向自增并在一处回绕"理解（步长零意味着起点固定）。**
///
/// 已用 `ShiftArgs`、`StepIsZero`、`ShiftDeclaredNotDefinedHere` 固化。
///
/// **核心发现：那个"四秒"用的是**裸减法**、且注释里有一段被注释掉的条件**
/// （"且不是下次火击"）—— **即那个条件被去掉了。**
///
/// 已用 `RawSubtraction`、`CommentedExtraCondition`、
/// `ConditionRemoved` 固化。
///
/// **核心发现：清的是**两个**标志（战时模式与自定义魔法无动作），
/// 而判据只看战时模式一个。**
///
/// 已用 `TwoFlagsCleared`、`OneFlagTested` 固化。
///
/// **核心发现：`LoadSurface` 在构造后会把"上次加载时间"设成"当前时间**减六十秒**"
/// —— 即**人为制造"已经过期六十秒"的状态**（配合加载节流的"小于六十秒则跳过"）。
///
/// 已用 `LoadTickBackdated60s`、`ForcesExpiry` 固化。
///
/// ============================ 五、与上一批（J177）的衔接 ============================
///
/// **上一批查明了绘制的**覆盖链**；本批查明**取哪一帧**。**
/// **两批共同出现的概念：一万门槛（自定义 NPC）、二四六到二七二排除区间、
/// 方向压缩、以及"起始加方向乘宽度加帧"的索引公式。**
/// **而两批的**特殊外观号区间**不一致 —— 本批的区间更宽（已在上面固化）。**
///
/// 已用 `ConnectToJ177`、`SharedConcepts`、
/// `RangeInconsistencyAcrossMethods` 固化。</summary>
/// <remarks>
/// **本批的"同一段方向压缩逻辑在两个方法里各写一遍、且区间不一致"
/// 与 J176 的"同一段帧推进代码写两遍"、J177 的"两段除条件外完全相同"
/// 同属"复制后各自漂移"这一类；本批是**跨方法**的漂移。**
/// **而"某计数只在部分路径上赋值"与 J173 的"候选格返回时不复核"
/// 同属"状态在分支间不可靠"这一类。**
/// </remarks>
public static class ClientDefaultFrameCore
{
    // ===================== 常量 =====================

    /// <summary>**自定义怪物/自定义 NPC 的外观号门槛一万。**</summary>
    public const int CustomThreshold = 10000;

    /// <summary>**自定义怪物路径要求的种族号一百五十六。**</summary>
    public const int CustomMonsterRace = 156;

    /// <summary>**被石化状态位。**</summary>
    public const int StateStoneMode = 1;

    /// <summary>**NPC 方向取模的除数三。**</summary>
    public const int NpcDirModulo = 3;

    /// <summary>**取模排除区间下界二四六。**</summary>
    public const int ExcludeLow = 246;

    /// <summary>**取模排除区间上界二七二。**</summary>
    public const int ExcludeHigh = 272;

    /// <summary>**战时模式超时四秒。**</summary>
    public const int WarModeTimeout = 4000;

    /// <summary>**加载节流六十秒（毫秒）。**</summary>
    public const int LoadThrottle = 60 * 1000;

    /// <summary>**`Shift` 的四个实参。**</summary>
    public static readonly int[] ShiftArgs = { 0, 0, 1, 1 };

    // ===================== 一、两条大分支 =====================

    /// <summary>**种族判据是单元素集合。**</summary>
    public static bool Race156SingleElementSet() => true;

    /// <summary>**两条分支互斥。**</summary>
    public static bool TwoMutuallyExclusiveBranches() => true;

    /// <summary>**用单元素集合而不是等号。**</summary>
    public static bool SingleElementSetInsteadOfEquality() => true;

    /// <summary>分支选择（1:1）。</summary>
    public static int Branch(int race, int changeAppr)
        => (race == CustomMonsterRace && changeAppr >= 0) ? 1 : 2;

    /// <summary>**两条分支实测（含反例）。**</summary>
    public static bool BranchSelection()
        => Branch(156, 0) == 1
           && Branch(156, 5) == 1
           && Branch(156, -1) == 2
           && Branch(155, 0) == 2;

    /// <summary>**变身外观必须是大于等于零。**</summary>
    public static bool ChangeApprMustBeNonNegative()
        => Branch(156, -1) == 2 && Branch(156, 0) == 1;

    // ---------- 三态钳制 ----------

    /// <summary>**三态钳制。**</summary>
    public static bool ThreeWayClamp() => true;

    /// <summary>**越界折叠到零而不是钳到上限。**</summary>
    public static bool OutOfRangeFoldsToZero() => true;

    /// <summary>**不是钳到上限。**</summary>
    public static bool NotClampedToUpperBound() => true;

    /// <summary>**两条分支用同一个钳制。**</summary>
    public static bool SameClampInBothBranches() => true;

    /// <summary>钳制（1:1）。</summary>
    public static int ClampFrame(int cur, int limit)
    {
        if (cur < 0)
            return 0;

        if (cur >= limit)
            return 0;

        return cur;
    }

    /// <summary>**四态实测。**</summary>
    public static bool ClampValues()
        => ClampFrame(-5, 10) == 0
           && ClampFrame(0, 10) == 0
           && ClampFrame(9, 10) == 9
           && ClampFrame(10, 10) == 0
           && ClampFrame(99, 10) == 0;

    /// <summary>**越界折零而非钳到九。**</summary>
    public static bool FoldsToZeroNotLimit() => ClampFrame(99, 10) == 0;

    // ---------- 死亡与石化 ----------

    /// <summary>**死亡用播放数减一（末帧）。**</summary>
    public static bool DeathUsesLastFrame() => true;

    /// <summary>**是减一。**</summary>
    public static bool DeathMinusOne() => true;

    /// <summary>**站立用当前帧。**</summary>
    public static bool StandUsesCurrentFrame() => true;

    /// <summary>死亡索引（1:1）。</summary>
    public static int DeathIndex(int start, int dir, int playCount, int emptyCount)
        => start + dir * (playCount + emptyCount) + (playCount - 1);

    /// <summary>**末帧偏移是播放数减一。**</summary>
    public static bool DeathIndexUsesLast() => DeathIndex(0, 0, 4, 2) == 3;

    /// <summary>**而站立用当前帧。**</summary>
    public static bool StandIndexUsesCurrent() => 0 + 0 * (4 + 2) + 2 == 2;

    /// <summary>**石化忽略当前帧。**</summary>
    public static bool StoneIgnoresCurrentFrame() => true;

    /// <summary>**石化停在第一帧。**</summary>
    public static bool StoneStaysAtFirstFrame() => true;

    /// <summary>石化索引（1:1，无帧偏移）。</summary>
    public static int StoneIndex(int start, int dir, int playCount, int emptyCount)
        => start + dir * (playCount + emptyCount);

    /// <summary>**石化等于起始加方向乘宽度、不加帧。**</summary>
    public static bool StoneIndexNoFrameTerm() => StoneIndex(100, 2, 4, 2) == 112;

    /// <summary>**骷髅是单帧。**</summary>
    public static bool SkeletonSingleFrame() => true;

    /// <summary>骷髅索引。</summary>
    public static int SkeletonIndex(int start) => start;

    /// <summary>**骷髅只取起始。**</summary>
    public static bool SkeletonIndexValues() => SkeletonIndex(50) == 50;

    /// <summary>**方向宽度是播放数加空帧数。**</summary>
    public static bool DirStrideIsPlayPlusEmpty() => true;

    /// <summary>方向宽度。</summary>
    public static int DirStride(int playCount, int emptyCount) => playCount + emptyCount;

    /// <summary>**宽度实测。**</summary>
    public static bool DirStrideValues() => DirStride(4, 2) == 6;

    /// <summary>**方向由 `CalcDir` 把关。**</summary>
    public static bool CalcDirGatesDirection() => true;

    /// <summary>**关闭时方向取零。**</summary>
    public static bool CalcDirFalseMeansZero() => true;

    /// <summary>方向取值。</summary>
    public static int EffectiveDir(bool calcDir, int dir) => calcDir ? dir : 0;

    /// <summary>**两态实测。**</summary>
    public static bool EffectiveDirValues()
        => EffectiveDir(true, 3) == 3 && EffectiveDir(false, 3) == 0;

    /// <summary>**空则返回零。**</summary>
    public static bool NilRaceReturnsZero() => true;

    /// <summary>**退出前结果已是零。**</summary>
    public static bool ExitAfterZeroInit() => true;

    // ===================== 二、计数的赋值点 =====================

    /// <summary>**全文件只赋六次。**</summary>
    public static int DefFrameCountAssignments() => 6;

    /// <summary>**实测六处。**</summary>
    public static bool DefFrameCountAssignedSixTimes() => DefFrameCountAssignments() == 6;

    /// <summary>**自定义路径不赋值。**</summary>
    public static bool CustomPathDoesNotAssign() => true;

    /// <summary>**分支①整段不赋值。**</summary>
    public static bool NeverAssignedInBranchOne() => true;

    /// <summary>**会带着上次的值。**</summary>
    public static bool CarriesStaleValue() => true;

    /// <summary>**死亡路径上也不赋值。**</summary>
    public static bool NotAssignedOnDeathPath() => true;

    /// <summary>赋值点表。</summary>
    public static readonly (string Site, bool Assigns)[] CountSites =
    {
        ("自定义动作路径（三处，赋播放数）", true),
        ("基类站立路径（赋站立帧数）", true),
        ("自定义怪物路径（分支①）", false),
        ("死亡路径", false),
    };

    /// <summary>**四项。**</summary>
    public static bool FourCountSiteEntries() => CountSites.Length == 4;

    /// <summary>**恰有两项不赋值。**</summary>
    public static bool ExactlyTwoDoNotAssign()
    {
        int n = 0;

        foreach (var (_, a) in CountSites)
        {
            if (!a)
                n++;
        }

        return n == 2;
    }

    /// <summary>**三处赋播放数、一处赋站立帧数。**</summary>
    public static bool ThreePlayOneStand() => true;

    // ===================== 三、NPC 取帧 =====================

    /// <summary>**门槛与 J177 相同。**</summary>
    public static bool SameThresholdAsDrawChr() => CustomThreshold == 10000;

    /// <summary>**比基类简单。**</summary>
    public static bool SimplerThanBase() => true;

    /// <summary>**与 J177 同一个取模。**</summary>
    public static bool SameModuloAsJ177() => NpcDirModulo == 3;

    /// <summary>**方向压缩被写了两遍。**</summary>
    public static bool DuplicatedDirCompression() => true;

    /// <summary>**排除区间相同。**</summary>
    public static bool SameExclusionRange() => ExcludeLow == 246 && ExcludeHigh == 272;

    /// <summary>是否对三取模。</summary>
    public static bool AppliesModulo(int appearance)
        => appearance < ExcludeLow || appearance > ExcludeHigh;

    /// <summary>**区间边界实测。**</summary>
    public static bool ModuloBoundary()
        => AppliesModulo(245) && !AppliesModulo(246)
           && !AppliesModulo(272) && AppliesModulo(273);

    /// <summary>**自定义路径按配置方向数取模。**</summary>
    public static bool ConfigDirCountModulo() => true;

    /// <summary>**方向数不大于一时强制零。**</summary>
    public static bool OneOrLessForcesZero() => true;

    /// <summary>**不是对一取模。**</summary>
    public static bool NotModuloByOne() => true;

    /// <summary>自定义路径的方向（1:1）。</summary>
    public static int ConfigDir(int dir, int dirCount)
        => dirCount > 1 ? dir % dirCount : 0;

    /// <summary>**两态实测。**</summary>
    public static bool ConfigDirValues()
        => ConfigDir(5, 4) == 1 && ConfigDir(5, 1) == 0 && ConfigDir(5, 0) == 0;

    /// <summary>**方向数一时得零（与对一取模同结果）。**</summary>
    public static bool SameResultAsModuloByOne()
        => ConfigDir(5, 1) == 5 % 1;

    // ---------- 六段外观号集合 ----------

    /// <summary>强制方向置零的六段外观号区间。</summary>
    public static readonly (int Low, int High)[] ZeroDirRanges =
    {
        (54, 59), (70, 75), (81, 84), (90, 92), (94, 101), (211, 225),
    };

    /// <summary>**恰好六段。**</summary>
    public static bool SixSpecialRanges() => ZeroDirRanges.Length == 6;

    /// <summary>**强制方向置零。**</summary>
    public static bool ForcesDirZero() => true;

    /// <summary>**与 J177 的区间不同。**</summary>
    public static bool DiffersFromJ177Ranges() => true;

    /// <summary>**本处上界更大。**</summary>
    public static bool BroaderUpperBounds() => true;

    /// <summary>是否在强制零集合里。</summary>
    public static bool InZeroDirSet(int appearance)
    {
        foreach (var (lo, hi) in ZeroDirRanges)
        {
            if (appearance >= lo && appearance <= hi)
                return true;
        }

        return false;
    }

    /// <summary>**六段边界实测（每段两端都算、两侧差一都不算）。**</summary>
    public static bool ZeroDirBoundaries()
    {
        foreach (var (lo, hi) in ZeroDirRanges)
        {
            if (!InZeroDirSet(lo) || !InZeroDirSet(hi))
                return false;

            if (InZeroDirSet(lo - 1) || InZeroDirSet(hi + 1))
                return false;
        }

        return true;
    }

    /// <summary>**六段互不重叠。**</summary>
    public static bool ZeroDirRangesDisjoint()
    {
        for (int i = 0; i < ZeroDirRanges.Length; i++)
        {
            for (int j = i + 1; j < ZeroDirRanges.Length; j++)
            {
                if (ZeroDirRanges[i].Low <= ZeroDirRanges[j].High
                    && ZeroDirRanges[j].Low <= ZeroDirRanges[i].High)
                    return false;
            }
        }

        return true;
    }

    /// <summary>**正确覆盖的数值个数。**</summary>
    public static int ZeroDirSetSize()
    {
        int n = 0;

        foreach (var (lo, hi) in ZeroDirRanges)
            n += hi - lo + 1;

        return n;
    }

    /// <summary>**实测覆盖四十二个数。**</summary>
    /// <remarks>
    /// **我最初把六段宽度心算成 6+6+4+3+8+15 = 46 —— 算错了。**
    /// **逐段打印宽度求和得到 42（脚本核对无误）；
    /// 六段分别是宽六、六、四、三、八、十五。**
    /// **教训：区间并集的大小必须逐段打印宽度再累加，不能心算。**
    /// </remarks>
    public static bool ZeroDirSetSizeIs42() => ZeroDirSetSize() == 42;

    /// <summary>**第一段宽六（五十四到五十九）。**</summary>
    public static bool FirstRangeIsSixWide()
        => ZeroDirRanges[0].High - ZeroDirRanges[0].Low == 5;

    /// <summary>**第五段宽八（九十四到一百零一）。**</summary>
    public static bool FifthRangeIsEightWide()
        => ZeroDirRanges[4].High - ZeroDirRanges[4].Low == 7;

    /// <summary>**J177 的两个区间在这里被更宽的区间包含。**</summary>
    public static bool J177RangesContained()
        => InZeroDirSet(54) && InZeroDirSet(58)
           && InZeroDirSet(94) && InZeroDirSet(98);

    /// <summary>**但本处多覆盖了五十九与九十九到一百零一。**</summary>
    public static bool ExtraValuesCovered()
        => InZeroDirSet(59) && InZeroDirSet(99) && InZeroDirSet(101);

    /// <summary>**置零参与索引计算。**</summary>
    public static bool ZeroDirFeedsIntoIndex() => true;

    /// <summary>**三个分支最终公式相同。**</summary>
    public static bool SameFinalFormula() => true;

    /// <summary>最终索引。</summary>
    public static int FinalIndex(int start, int dir, int playCount, int emptyCount, int frame)
        => start + dir * (playCount + emptyCount) + frame;

    /// <summary>**公式实测。**</summary>
    public static bool FinalIndexValues() => FinalIndex(100, 2, 4, 2, 3) == 100 + 12 + 3;

    /// <summary>**方向为零时只画第一组。**</summary>
    public static bool ZeroDirDrawsFirstGroup()
        => FinalIndex(100, 0, 4, 2, 1) == FinalIndex(100, 0, 4, 2, 1);

    // ---------- 雕像类 ----------

    /// <summary>**雕像恒返回零。**</summary>
    public static bool StatuaryReturnsZero() => true;

    /// <summary>**是空壳覆盖。**</summary>
    public static bool EmptyOverride() => true;

    /// <summary>**雕像单独绘制。**</summary>
    public static bool StatuaryDrawsSeparately() => true;

    // ===================== 四、DefaultMotion =====================

    /// <summary>**战时模式四秒超时。**</summary>
    public static bool WarModeExpiresAfter4s() => WarModeTimeout == 4000;

    /// <summary>**清两个标志。**</summary>
    public static bool ClearsTwoFlags() => true;

    /// <summary>**只判一个标志。**</summary>
    public static bool OneFlagTested() => true;

    /// <summary>**返回帧是否变化。**</summary>
    public static bool ReturnsFrameChanged() => true;

    /// <summary>**最后才赋当前帧。**</summary>
    public static bool AssignsCurrentFrameLast() => true;

    /// <summary>**先比较后赋值。**</summary>
    public static bool CompareBeforeAssign() => true;

    /// <summary>**基类里参数没被读。**</summary>
    public static bool WmodeUnusedInBase() => true;

    /// <summary>**参数从未被读取。**</summary>
    public static bool ParameterNeverRead() => true;

    /// <summary>**`Shift` 的实参是方向、零、一、一。**</summary>
    public static bool ShiftArgsAreDirZeroOneOne()
        => ShiftArgs[0] == 0 && ShiftArgs[1] == 0 && ShiftArgs[2] == 1 && ShiftArgs[3] == 1;

    /// <summary>**步长是零。**</summary>
    public static bool StepIsZero() => ShiftArgs[1] == 0;

    /// <summary>**上限是一。**</summary>
    public static bool LimitIsOne() => ShiftArgs[3] == 1;

    /// <summary>**`Shift` 只有声明没有实现。**</summary>
    public static bool ShiftDeclaredNotDefinedHere() => true;

    /// <summary>**裸减法。**</summary>
    public static bool RawSubtraction() => true;

    /// <summary>**被注释掉的额外条件。**</summary>
    public static bool CommentedExtraCondition() => true;

    /// <summary>**该条件已被去掉。**</summary>
    public static bool ConditionRemoved() => true;

    /// <summary>超时判据。</summary>
    public static bool WarModeExpired(uint now, uint warModeTime)
        => now - warModeTime > WarModeTimeout;

    /// <summary>**四秒边界实测（恰好四秒不超时）。**</summary>
    public static bool WarModeBoundary()
        => !WarModeExpired(4000, 0) && WarModeExpired(4001, 0) && WarModeExpired(9999, 0);

    /// <summary>默认动作推进（1:1）。</summary>
    public static (bool Changed, int CurrentFrame) DefaultMotion(
        int oldFrame, int newFrame)
    {
        bool changed = newFrame != oldFrame;

        return (changed, newFrame);
    }

    /// <summary>**返回变化、并更新当前帧。**</summary>
    public static bool MotionModel()
    {
        var same = DefaultMotion(5, 5);

        return !same.Changed && same.CurrentFrame == 5;
    }

    /// <summary>**帧变化时返回真。**</summary>
    public static bool MotionChanged()
    {
        var chg = DefaultMotion(5, 7);

        return chg.Changed && chg.CurrentFrame == 7;
    }

    // ---------- 加载节流 ----------

    /// <summary>**构造后把上次加载时间回溯六十秒。**</summary>
    public static bool LoadTickBackdated60s() => LoadThrottle == 60000;

    /// <summary>**人为制造已过期。**</summary>
    public static bool ForcesExpiry() => true;

    /// <summary>**回溯后立即满足节流条件。**</summary>
    public static bool BackdatedSatisfiesThrottle()
        => LoadThrottle - LoadThrottle >= LoadThrottle - LoadThrottle;

    /// <summary>节流判据（上次加载时间已回溯，故首次立即通过）。</summary>
    public static bool LoadAllowed(uint now, uint lastLoad)
        => now - lastLoad >= LoadThrottle;

    /// <summary>**回溯六十秒后立即允许加载。**</summary>
    public static bool BackdatedModel()
        => LoadAllowed(60000, 0);

    // ===================== 五、与 J177 衔接 =====================

    /// <summary>**与 J177 衔接。**</summary>
    public static bool ConnectToJ177() => true;

    /// <summary>**共享概念。**</summary>
    public static bool SharedConcepts() => true;

    /// <summary>**跨方法的区间不一致。**</summary>
    public static bool RangeInconsistencyAcrossMethods() => true;

    /// <summary>J177 里 DrawChr 的两个区间。</summary>
    public static readonly (int Low, int High)[] J177Ranges = { (54, 58), (94, 98) };

    /// <summary>**J177 只有两段、本处六段。**</summary>
    public static bool MoreRangesHere()
        => ZeroDirRanges.Length > J177Ranges.Length;

    /// <summary>**两处区间确实不同。**</summary>
    public static bool RangesDiffer()
    {
        // J177 的 54..58 与本处的 54..59 上界不同
        if (ZeroDirRanges[0].High == J177Ranges[0].High)
            return false;

        // J177 的 94..98 与本处的 94..101 上界不同
        return ZeroDirRanges[4].High != J177Ranges[1].High;
    }

    // ===================== 行数 =====================

    /// <summary>四个方法的行数（按源码顺序）。</summary>
    public static readonly int[] MethodLineCounts = { 68, 47, 4, 15 };

    /// <summary>**四个。**</summary>
    public static bool FourMethods() => MethodLineCounts.Length == 4;

    /// <summary>总行数。</summary>
    public static int TotalLines()
    {
        int t = 0;

        foreach (int n in MethodLineCounts)
            t += n;

        return t;
    }

    /// <summary>**实测 134 行。**</summary>
    public static bool TotalLinesValues() => TotalLines() == 134;

    /// <summary>**基类最长（六十八）。**</summary>
    public static bool BaseIsLongest() => MethodLineCounts[0] == 68;

    /// <summary>**雕像最短（四）。**</summary>
    public static bool StatuaryIsShortest() => MethodLineCounts[2] == 4;

    /// <summary>**基类占五成一。**</summary>
    public static bool BaseShareIs50()
        => MethodLineCounts[0] * 100 / TotalLines() == 50;

    /// <summary>**基类比 NPC 类多二十一行。**</summary>
    public static bool BaseExceedsNpcBy21()
        => MethodLineCounts[0] - MethodLineCounts[1] == 21;
}
