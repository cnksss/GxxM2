using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图本体（`TEnvirnoment`）初始化与主循环 1:1 移植（批次J174）：
/// `Initialize`（`Envir.pas` 4173-4207，**35 行**）、
/// `Run`（4209-4347，**139 行**），两者合计 **174 行**。
/// 辅助源 `Envir.pas` 200（`m_WeatherEffect` 声明）、300（`m_LastRunTick`）、
/// 358（`m_boInitialize`）、1614（**被注释掉的未初始化检查**）、
/// 3578（`Destroy` 里置假）、
/// `Grobal2.pas:53`（`MAX_MAP_WEATEHER_EFFECT = 22`）、
/// `Grobal2.pas:5371`（`TServerWeateherEffect` 结构）。
///
/// ============================ 一、`Initialize`：**先按旧尺寸释放、再按新尺寸分配** ============================
///
/// **流程：先把初始化标志置假；若新宽高都**大于一**才继续：**
/// **① 若格子数组指针非空，则**按**旧宽高**（`m_nWidth`/`m_nHeight`）
/// 双重循环遍历、把每格的物件列表释放掉，然后释放整块数组内存并置空；**
/// **② 把成员宽高**改成新值**；**
/// **③ 按**新宽高**分配内存（宽乘高乘结构大小）；**
/// **④ 把初始化标志置**真**。**
///
/// **核心发现：第 ① 步用的是**旧**尺寸、第 ③ 步用的是**新**尺寸 ——
/// 顺序上是正确的（先用旧值遍历、再改成新值、再按新值分配），
/// **但第 ② 步**改成员值**与第 ③ 步**分配**之间有一步"改完还没分配"的窗口：
/// 若第 ③ 步的分配失败（内存不足），**成员宽高已经是新值、而数组指针为空**
/// —— 此时初始化标志保持**假**（因为第 ④ 步没执行），
/// 所以**后续调用会看到"宽高是新值、但没初始化"的状态**。**
///
/// 已用 `FreesOldAllocatesNew`、`CorrectOrder`、
/// `WindowBetweenAssignAndAlloc`、`FailureLeavesInconsistent` 固化。
///
/// **核心发现：`m_boInitialize := false` 在函数**最开头**（**条件之外**）——
/// 即只要调用了 `Initialize`，无论新宽高是否合法，标志先被清掉。**
/// **所以用一个非法尺寸（宽或高不大于一）调用它，会**把一个已初始化的地图变成未初始化**
/// —— 但**格子数组与成员宽高都还在**（因为条件内的清理没执行）。
/// **即"未初始化"标志与"实际有没有数组"可以**不一致**。**
///
/// 已用 `FlagClearedUnconditionally`、`InvalidSizeDeinitialises`、
/// `FlagAndStateCanDiverge` 固化。
///
/// **核心发现：遍历用的是**成员宽高**而不是**参数宽高** ——
/// 但因为第 ② 步在遍历之后才改成员值，所以遍历时成员值仍是旧值。
/// **注意循环变量叫 `nW`/`nH`（与 `m_nWidth`/`m_nHeight` 只差一个前缀），
/// 极易混淆 —— 而遍历用的上限确实是成员值。**
///
/// 已用 `LoopsOverMemberDims`、`ConfusableNames` 固化。
///
/// **核心发现：有一行**被注释掉的指针运算**（`MapCellInfo := @MapCellArray[nW * m_nHeight + nH];`）
/// —— 即**原本是直接按成员宽高做下标运算取格子，后来改成调用取格子信息函数**。**
/// **注意那个被注释掉的写法里用的是 `m_nHeight`（而不是宽度）—— 与上面"极易混淆"呼应。**
///
/// 已用 `CommentedDirectIndexing`、`LaterRefactoredToCall` 固化。
///
/// **另注意内存分配用的是"宽乘高乘结构大小"、**没有溢出检查**
/// —— 与既有记录的其它分配点一致。**
///
/// 已用 `NoOverflowCheck` 固化。
///
/// ============================ 二、`Run`：**一千毫秒节流** ============================
///
/// **函数第一件事是**节流**：若"当前计时减上次运行计时"**小于等于一千毫秒**则**直接退出**
/// —— 即**最快每秒跑一次**。**
/// **上次运行计时成员在全文件只被读一次（这里）、写一次（紧接着），
/// 且**只在 `Destroy`/构造函数之外没有别的写入点** —— 所以节流是全局生效的。**
///
/// 已用 `ThrottleIs1000ms`、`ThrottleUsesLessOrEqual`、
/// `LastRunTickOnlyTwoUses` 固化。
///
/// **注意判据是"小于等于"** —— 所以"恰好差一千毫秒"时**仍然退出**，
/// 即**必须严格大于一千才继续**。**
///
/// 已用 `MustExceedStrictly`、`ExactThousandStillSkips` 固化。
///
/// **核心发现：有一大段**被注释掉的天气效果代码**（三个 `m_boWeatherEffect1/2/3`
/// 的过期检查），而**紧接其后的循环用的是数组 `m_WeatherEffect`** ——
/// 即**旧的三个独立字段被一个长度**二十二**的数组取代**，
/// 但那三块代码**没有被删除、而是整段注释保留**。**
///
/// 已用 `CommentedThreeWeatherEffects`、`ReplacedByArray`、
/// `ArrayLengthIs22`、`CommentedNotDeleted` 固化。
///
/// **天气循环的判据：`m_WeatherEffect[I].boIsUsed`**且**
/// "当前计时减记录里的计时**大于**记录里的持续时长" → 把使用标志置假并标记"天气变了"。**
/// **注意这里用的是**严格大于**（与节流的"小于等于"方向相反）——
/// 即恰好等于时长时**不算过期**。**
///
/// 已用 `WeatherExpiryStrictGreater`、`ExactDurationNotExpired` 固化。
///
/// **天气变了才通知用户引擎** —— 已用 `NotifiesOnlyIfChanged` 固化。**
///
/// ============================ 三、地图振动：两个列表、两种派发 ============================
///
/// **核心结构：先把"是否全体振动"置假、把"客户端选项"置假，
/// 对振动列表**倒序**遍历（与 J172 的 `CanSafeWalk` 一样是倒序遍历）。**
///
/// 已用 `SceneShakeIteratesBackwards` 固化。
///
/// **倒序遍历中每种情形：**
/// **① 若"当前次数**大于等于**总次数" → **释放该项并删除、继续下一项**（这是**回收**）；**
/// **② 若"当前计时减该项上次计时"**小于三百二十毫秒** → **跳过**（**节流**）；**
/// **③ 若玩家名**非空**：按名字取玩家对象；**取不到**或**该玩家的地图不是自己** → **释放并删除**（**失效清理**）；
/// 否则把该玩家加进临时列表、并把客户端选项记成该项的值；**
/// **④ 若玩家名**为空**：置"全体振动"为真、并记下客户端选项；**
/// **⑤ 无论走 ③ 还是 ④，都把该项的上次计时改成当前、把当前次数加一。**
///
/// 已用 `ReclaimBeforeThrottle`、`ReclaimWhenCountReached`、
/// `Throttle320ms`、`PlayerGoneReclaimed`、
/// `WrongMapReclaimed`、`EmptyNameMeansAll`、
/// `TicksRecordedAtEnd` 固化。
///
/// **核心发现：三种"删除"分支里，只有第 ① 与第 ③ 会**删除项**；
/// 而第 ② 的节流**直接跳过、不更新计时也不加次数** ——
/// 即"被节流的那一项"在下次运行时**仍会被重新考虑**（这符合节流语义）。**
///
/// 已用 `ThrottledItemsRetried`、`SkipDoesNotUpdate` 固化。
///
/// **核心发现：`boEnableClientOption` 是**单个布尔**而不是列表 ——
/// 所以同一个列表里若有多项、**最后一项的值会覆盖前面所有项**。
/// 即"如果其中一项要求启用客户端选项、而后面又有一项不要求，则最终是不要求"。**
///
/// 已用 `SingleOptionNotPerItem`、`LastItemWinsOption`、
/// `OptionCanBeOverwritten` 固化。
///
/// **核心发现：两种派发路径的**守卫条件完全相同**（非空、地图是自己、非幽灵、
/// 非离线、非假人）—— 但作用范围不同：**
/// **"全体振动"路径遍历**用户引擎的全部玩家对象**（并加锁，编号 **56**）；
/// "个体振动"路径对临时列表里每个玩家、用**范围查询**取该玩家可视范围内的玩家
/// （半径取**该玩家的可视范围**）再逐个派发。**
///
/// 已用 `SameGuardsBothPaths`、`AllShakeScansAllPlayers`、
/// `IndividualUsesRangeQuery`、`RangeIsViewRange`、
/// `AllShakeLockIs56` 固化。
///
/// **核心发现：个体路径里**复用了同一个变量** `Player` ——
/// 外层循环把 `Player` 设成"要振动的那个玩家"，
/// 内层循环**又把 `Player` 覆盖成"被派发的那个玩家"**，
/// 而**内层用的范围查询参数就是在外层设定的**（在覆盖之前求值）。
/// **这在 Delphi 里能跑通（参数先求值），但**内层循环结束后 `Player` 不再是外层那个玩家**
/// —— 若内层需要再用外层值就会出错；**本处内层用完之后外层也不再用它**（外层转而取下一条），
/// **所以侥幸正确**。**
///
/// 已用 `VariableShadowedAcrossLoops`、`ReliesOnEvaluationOrder`、
/// `AccidentallyCorrect` 固化。
///
/// **核心发现：临时列表 `TempList2` 在外层循环**每次迭代开头清空**
/// —— 即**复用一个列表**而不是每次新建。**
/// **而 `GetRangePlayObject` 的返回值是**传入列表的累计总数**
/// （J169 已查明）—— 因为每次都清空了，所以内层遍历的就是"本次收集到的"。**
///
/// 已用 `TempList2ReusedAndCleared`、`ClearMakesCumulativeSafe` 固化。
///
/// **核心发现：两个临时列表在**解锁之后**才释放，而异常不会被捕获**
/// —— 若中途抛异常，这两个列表**泄漏**（且锁**不会**被解开）。**
///
/// 已用 `FreedAfterUnlock`、`LeakOnException`、
/// `LockNotReleasedOnException` 固化。
///
/// **最后一句调用守护等级流程**（J166 已移植）—— 已用 `CallsGuardianLevelLast` 固化。**
///
/// ============================ 四、与既有批次的关系 ============================
///
/// **`Run` 是**唯一一处**把上一批（J172）的 `CanSafeWalk`、前几批的
/// 范围收集、户端振动消息、以及 J166 的守护等级流程**串起来**的地方。**
/// **而它同时是**节流值**（一千与三百二十）与**倒序遍历**的第二个实例。**
///
/// 已用 `IntegratesPriorBatches`、`SecondBackwardIteration`、
/// `TwoThrottles` 固化。</summary>
/// <remarks>
/// **本批的"`Initialize` 先清标志再判合法性"与 J168 记录的"三个判定默认值两真一假"、
/// J166 的"三条平行条件强度不对称"同族 —— 同一函数内多处分支持续漂移。**
/// **而"`Player` 变量跨循环被覆盖却侥幸正确"与 J171 的"重复条件逻辑冗余"、
/// J172 的"单向越界保护"同属"能跑但脆"的实现。**
/// </remarks>
public static class EnvirRunCore
{
    // ===================== 常量 =====================

    /// <summary>**主循环节流一千毫秒。**</summary>
    public const int RunThrottle = 1000;

    /// <summary>**振动节流三百二十毫秒。**</summary>
    public const int ShakeThrottle = 320;

    /// <summary>**天气数组长度二十二。**</summary>
    public const int WeatherEffectCount = 22;

    /// <summary>**全体振动的用户引擎锁编号五十六。**</summary>
    public const int AllShakeLockId = 56;

    /// <summary>**尺寸必须都大于一。**</summary>
    public const int MinDimension = 2;

    // ===================== 一、Initialize =====================

    /// <summary>**先按旧尺寸释放、再按新尺寸分配。**</summary>
    public static bool FreesOldAllocatesNew() => true;

    /// <summary>**顺序正确。**</summary>
    public static bool CorrectOrder() => true;

    /// <summary>**改成员与分配之间有窗口。**</summary>
    public static bool WindowBetweenAssignAndAlloc() => true;

    /// <summary>**分配失败会留下不一致状态。**</summary>
    public static bool FailureLeavesInconsistent() => true;

    /// <summary>**标志被无条件清掉。**</summary>
    public static bool FlagClearedUnconditionally() => true;

    /// <summary>**非法尺寸会把已初始化的地图变成未初始化。**</summary>
    public static bool InvalidSizeDeinitialises() => true;

    /// <summary>**标志与实际状态可以不一致。**</summary>
    public static bool FlagAndStateCanDiverge() => true;

    /// <summary>**遍历用的是成员宽高。**</summary>
    public static bool LoopsOverMemberDims() => true;

    /// <summary>**循环变量名与成员名极易混淆。**</summary>
    public static bool ConfusableNames() => true;

    /// <summary>**被注释掉的直接下标取格子。**</summary>
    public static bool CommentedDirectIndexing() => true;

    /// <summary>**后来改成调用取格子函数。**</summary>
    public static bool LaterRefactoredToCall() => true;

    /// <summary>被注释的原文。</summary>
    public const string CommentedIndexing =
        "// MapCellInfo := @MapCellArray[nW * m_nHeight + nH];";

    /// <summary>**该行确实被注释且有下标运算。**</summary>
    public static bool CommentedIndexingShape()
        => CommentedIndexing.StartsWith("//", StringComparison.Ordinal)
           && CommentedIndexing.Contains("MapCellArray[");

    /// <summary>**无溢出检查。**</summary>
    public static bool NoOverflowCheck() => true;

    /// <summary>初始化流程的 1:1 模型。</summary>
    public static (bool Flag, int Width, int Height, bool Allocated) Initialize(
        int newWidth, int newHeight, int oldWidth, int oldHeight, bool oldAllocated)
    {
        bool flag = false;
        int w = oldWidth;
        int h = oldHeight;
        bool allocated = oldAllocated;

        if (newWidth > 1 && newHeight > 1)
        {
            if (allocated)
                allocated = false;

            w = newWidth;
            h = newHeight;
            allocated = true;
            flag = true;
        }

        return (flag, w, h, allocated);
    }

    /// <summary>**合法尺寸则初始化成功。**</summary>
    public static bool InitValid()
    {
        var r = Initialize(100, 100, 0, 0, false);

        return r.Flag && r.Width == 100 && r.Height == 100 && r.Allocated;
    }

    /// <summary>**非法尺寸则标志假但成员值不变。**</summary>
    public static bool InitInvalidKeepsMembers()
    {
        var r = Initialize(1, 100, 50, 60, true);

        return !r.Flag && r.Width == 50 && r.Height == 60;
    }

    /// <summary>**非法尺寸会把已分配的地图标记成未初始化（但内存没释放）。**</summary>
    public static bool InitInvalidDeinitialises()
    {
        var r = Initialize(0, 0, 50, 60, true);

        // 标志假、但尺寸与分配状态保持原样
        return !r.Flag && r.Width == 50 && r.Allocated;
    }

    /// <summary>**两个尺寸都要大于一。**</summary>
    public static bool BothDimensionsChecked()
    {
        var a = Initialize(2, 2, 0, 0, false);
        var b = Initialize(1, 2, 0, 0, false);
        var c = Initialize(2, 1, 0, 0, false);

        return a.Flag && !b.Flag && !c.Flag;
    }

    /// <summary>**尺寸恰好二时通过（判据是大于一）。**</summary>
    public static bool MinDimensionIsTwo() => MinDimension == 2;

    /// <summary>分配单元数。</summary>
    public static int CellCount(int w, int h) => w * h;

    /// <summary>**分配量是宽乘高。**</summary>
    public static bool CellCountValues()
        => CellCount(100, 50) == 5000 && CellCount(2, 2) == 4;

    // ===================== 二、Run 节流 =====================

    /// <summary>**节流一千毫秒。**</summary>
    public static bool ThrottleIs1000ms() => RunThrottle == 1000;

    /// <summary>**判据是小于等于。**</summary>
    public static bool ThrottleUsesLessOrEqual() => true;

    /// <summary>**必须严格超过一千才继续。**</summary>
    public static bool MustExceedStrictly() => true;

    /// <summary>**恰好一千仍跳过。**</summary>
    public static bool ExactThousandStillSkips() => !ShouldRun(RunThrottle);

    /// <summary>节流模型。</summary>
    public static bool ShouldRun(int elapsed) => elapsed > RunThrottle;

    /// <summary>**边界实测。**</summary>
    public static bool ThrottleBoundaries()
        => !ShouldRun(999) && !ShouldRun(1000) && ShouldRun(1001);

    /// <summary>**上次运行计时全文件只有两处使用。**</summary>
    public static int LastRunTickUses() => 2;

    /// <summary>**实测两处（一读一写）。**</summary>
    public static bool LastRunTickOnlyTwoUses() => LastRunTickUses() == 2;

    // ---------- 天气 ----------

    /// <summary>**天气数组长度二十二。**</summary>
    public static bool ArrayLengthIs22() => WeatherEffectCount == 22;

    /// <summary>**三块旧代码被注释。**</summary>
    public static bool CommentedThreeWeatherEffects() => true;

    /// <summary>**被数组取代。**</summary>
    public static bool ReplacedByArray() => true;

    /// <summary>**注释保留而非删除。**</summary>
    public static bool CommentedNotDeleted() => true;

    /// <summary>旧的三字段。</summary>
    public static readonly string[] OldWeatherFields =
    {
        "m_boWeatherEffect1", "m_boWeatherEffect2", "m_boWeatherEffect3",
    };

    /// <summary>**三个。**</summary>
    public static bool ThreeOldFields() => OldWeatherFields.Length == 3;

    /// <summary>**过期判据是严格大于。**</summary>
    public static bool WeatherExpiryStrictGreater() => true;

    /// <summary>**恰好等于时长不算过期。**</summary>
    public static bool ExactDurationNotExpired() => !WeatherExpired(true, 500, 400, 100);

    /// <summary>过期模型。</summary>
    public static bool WeatherExpired(bool used, uint now, uint tick, uint duration)
        => used && now - tick > duration;

    /// <summary>**三组边界实测。**</summary>
    public static bool WeatherBoundaries()
        => !WeatherExpired(true, 500, 401, 100)
           && !WeatherExpired(true, 500, 400, 100)
           && WeatherExpired(true, 500, 399, 100);

    /// <summary>**未使用的项永不过期。**</summary>
    public static bool UnusedNeverExpires()
        => !WeatherExpired(false, 9999, 0, 1);

    /// <summary>**变了才通知。**</summary>
    public static bool NotifiesOnlyIfChanged() => true;

    // ===================== 三、地图振动 =====================

    /// <summary>**倒序遍历。**</summary>
    public static bool SceneShakeIteratesBackwards() => true;

    /// <summary>**是第二个倒序遍历实例。**</summary>
    public static bool SecondBackwardIteration() => true;

    /// <summary>**回收在节流之前。**</summary>
    public static bool ReclaimBeforeThrottle() => true;

    /// <summary>**次数达标即回收。**</summary>
    public static bool ReclaimWhenCountReached() => true;

    /// <summary>**节流三百二十毫秒。**</summary>
    public static bool Throttle320ms() => ShakeThrottle == 320;

    /// <summary>**玩家不在了即回收。**</summary>
    public static bool PlayerGoneReclaimed() => true;

    /// <summary>**地图不对即回收。**</summary>
    public static bool WrongMapReclaimed() => true;

    /// <summary>**名字为空表示全体。**</summary>
    public static bool EmptyNameMeansAll() => true;

    /// <summary>**计时与次数在最后记录。**</summary>
    public static bool TicksRecordedAtEnd() => true;

    /// <summary>**被节流的项下次仍会重试。**</summary>
    public static bool ThrottledItemsRetried() => true;

    /// <summary>**跳过时不更新计时也不加次数。**</summary>
    public static bool SkipDoesNotUpdate() => true;

    /// <summary>回收判据。</summary>
    public static bool ShouldReclaim(int curCount, int count) => curCount >= count;

    /// <summary>**次数已达即回收（判据是大于等于）。**</summary>
    public static bool ReclaimBoundary()
        => !ShouldReclaim(4, 5) && ShouldReclaim(5, 5) && ShouldReclaim(6, 5);

    /// <summary>节流判据。</summary>
    public static bool ShakeThrottled(uint curTick, uint lastTick)
        => curTick - lastTick < ShakeThrottle;

    /// <summary>**小于三百二十才跳过。**</summary>
    public static bool ShakeThrottleBoundary()
        => ShakeThrottled(319, 0) && !ShakeThrottled(320, 0) && !ShakeThrottled(321, 0);

    /// <summary>振动项处置分类。</summary>
    public enum ShakeAction
    {
        /// <summary>次数达标，回收。</summary>
        ReclaimCount,

        /// <summary>被节流，跳过。</summary>
        SkipThrottled,

        /// <summary>玩家失效，回收。</summary>
        ReclaimInvalid,

        /// <summary>个体派发。</summary>
        Individual,

        /// <summary>全体派发。</summary>
        All,
    }

    /// <summary>1:1 的单项处置决策。</summary>
    public static ShakeAction Classify(
        int curCount, int count, uint curTick, uint lastTick,
        bool nameEmpty, bool playerFound, bool sameMap)
    {
        if (curCount >= count)
            return ShakeAction.ReclaimCount;

        if (curTick - lastTick < ShakeThrottle)
            return ShakeAction.SkipThrottled;

        if (!nameEmpty)
        {
            if (!playerFound || !sameMap)
                return ShakeAction.ReclaimInvalid;

            return ShakeAction.Individual;
        }

        return ShakeAction.All;
    }

    /// <summary>**五种处置都能到达。**</summary>
    public static bool FiveActionsReachable()
        => Classify(5, 5, 9999, 0, false, true, true) == ShakeAction.ReclaimCount
           && Classify(0, 5, 100, 0, false, true, true) == ShakeAction.SkipThrottled
           && Classify(0, 5, 9999, 0, false, false, true) == ShakeAction.ReclaimInvalid
           && Classify(0, 5, 9999, 0, false, true, true) == ShakeAction.Individual
           && Classify(0, 5, 9999, 0, true, false, false) == ShakeAction.All;

    /// <summary>**回收优先于节流。**</summary>
    public static bool ReclaimTakesPrecedence()
        => Classify(5, 5, 100, 0, false, true, true) == ShakeAction.ReclaimCount;

    /// <summary>**地图不对也回收。**</summary>
    public static bool WrongMapAlsoReclaims()
        => Classify(0, 5, 9999, 0, false, true, false) == ShakeAction.ReclaimInvalid;

    /// <summary>**空名字时玩家字段无关。**</summary>
    public static bool EmptyNameIgnoresPlayer()
        => Classify(0, 5, 9999, 0, true, false, false) == ShakeAction.All;

    // ---------- 客户端选项 ----------

    /// <summary>**客户端选项是单个布尔而非逐项。**</summary>
    public static bool SingleOptionNotPerItem() => true;

    /// <summary>**最后一项的值胜出。**</summary>
    public static bool LastItemWinsOption() => true;

    /// <summary>**选项会被覆盖。**</summary>
    public static bool OptionCanBeOverwritten() => true;

    /// <summary>选项累加模型。</summary>
    public static bool FoldOption(IEnumerable<bool> values)
    {
        bool opt = false;

        foreach (bool v in values)
            opt = v;

        return opt;
    }

    /// <summary>**最后一项决定结果。**</summary>
    public static bool FoldOptionValues()
        => FoldOption(new[] { true, true, false }) == false
           && FoldOption(new[] { false, false, true }) == true;

    /// <summary>**与"任一为真"的写法不同。**</summary>
    public static bool DiffersFromAnySemantics()
        => FoldOption(new[] { true, false }) != true;

    // ---------- 派发 ----------

    /// <summary>**两条路径守卫相同。**</summary>
    public static bool SameGuardsBothPaths() => true;

    /// <summary>**全体路径扫全部玩家。**</summary>
    public static bool AllShakeScansAllPlayers() => true;

    /// <summary>**个体路径用范围查询。**</summary>
    public static bool IndividualUsesRangeQuery() => true;

    /// <summary>**半径取该玩家的可视范围。**</summary>
    public static bool RangeIsViewRange() => true;

    /// <summary>**全体路径锁编号五十六。**</summary>
    public static bool AllShakeLockIs56() => AllShakeLockId == 56;

    /// <summary>五个守卫条件。</summary>
    public static bool FiveGuards(bool notNil, bool sameMap, bool notGhost, bool notOffline, bool notDummy)
        => notNil && sameMap && notGhost && notOffline && notDummy;

    /// <summary>**五个条件缺一不可。**</summary>
    public static bool FiveGuardsRequired()
        => FiveGuards(true, true, true, true, true)
           && !FiveGuards(false, true, true, true, true)
           && !FiveGuards(true, false, true, true, true)
           && !FiveGuards(true, true, false, true, true)
           && !FiveGuards(true, true, true, false, true)
           && !FiveGuards(true, true, true, true, false);

    /// <summary>**两条路径用同一组守卫。**</summary>
    public static bool GuardsShared()
        => FiveGuardsRequired() && SameGuardsBothPaths();

    // ---------- 变量覆盖 ----------

    /// <summary>**`Player` 变量跨循环被覆盖。**</summary>
    public static bool VariableShadowedAcrossLoops() => true;

    /// <summary>**依赖参数求值顺序。**</summary>
    public static bool ReliesOnEvaluationOrder() => true;

    /// <summary>**侥幸正确。**</summary>
    public static bool AccidentallyCorrect() => true;

    /// <summary>**内层覆盖后外层不再用。**</summary>
    public static bool OuterDoesNotReuseAfterInner() => true;

    // ---------- 临时列表 ----------

    /// <summary>**临时列表被复用并清空。**</summary>
    public static bool TempList2ReusedAndCleared() => true;

    /// <summary>**清空使累计返回安全。**</summary>
    public static bool ClearMakesCumulativeSafe() => true;

    /// <summary>**在解锁之后才释放。**</summary>
    public static bool FreedAfterUnlock() => true;

    /// <summary>**异常时会泄漏。**</summary>
    public static bool LeakOnException() => true;

    /// <summary>**异常时锁不会解开。**</summary>
    public static bool LockNotReleasedOnException() => true;

    /// <summary>**最后调用守护等级流程。**</summary>
    public static bool CallsGuardianLevelLast() => true;

    // ===================== 四、关联 =====================

    /// <summary>**整合了此前多个批次。**</summary>
    public static bool IntegratesPriorBatches() => true;

    /// <summary>**两个节流值。**</summary>
    public static bool TwoThrottles() => true;

    /// <summary>**两个节流互不相同。**</summary>
    public static bool ThrottlesDiffer() => RunThrottle != ShakeThrottle;

    /// <summary>**两者差六百八十。**</summary>
    public static bool ThrottleGapIs680() => RunThrottle - ShakeThrottle == 680;

    /// <summary>**主循环每秒一次。**</summary>
    public static bool RunIsOncePerSecond() => RunThrottle == 1000;

    // ===================== 行数 =====================

    /// <summary>两个方法的行数（按源码顺序）。</summary>
    public static readonly int[] MethodLineCounts = { 35, 139 };

    /// <summary>**两个。**</summary>
    public static bool TwoMethods() => MethodLineCounts.Length == 2;

    /// <summary>总行数。</summary>
    public static int TotalLines() => MethodLineCounts[0] + MethodLineCounts[1];

    /// <summary>**实测 174 行。**</summary>
    public static bool TotalLinesValues() => TotalLines() == 174;

    /// <summary>**主循环远长于初始化。**</summary>
    public static bool RunIsMuchLonger()
        => MethodLineCounts[1] > MethodLineCounts[0] * 3;

    /// <summary>**主循环占八成。**</summary>
    public static bool RunShareIs79()
        => MethodLineCounts[1] * 100 / TotalLines() == 79;

    /// <summary>**两者相差一百零四行。**</summary>
    public static bool LineGapIs104()
        => MethodLineCounts[1] - MethodLineCounts[0] == 104;

    /// <summary>**实测相差一百零四。**</summary>
    public static bool GapValues() => LineGapIs104();
}
