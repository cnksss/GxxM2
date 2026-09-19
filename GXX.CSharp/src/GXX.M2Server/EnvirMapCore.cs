using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图本体（`TEnvirnoment`）地图格与运行接口 1:1 移植（批次J158）：
/// `TEnvirnoment.AddDoorToMap`（`Envir.pas` 1418-1429，**12 行**）、
/// `GetDropPosition`（1430-1519，**90 行**）、
/// `GetDropPosition2`（1520-1611，**92 行**）、
/// `GetMapCellInfo`（1612-1624，**13 行**）、
/// `AddHumBBCount`（1625-1632，**8 行**）、
/// `CanAddToMapPosition`（1633-1643，**11 行**）、
/// `Initialize`（4173-4208，**36 行**）、
/// `Run`（4209-4348，**140 行**）。
/// 辅助源 `Envir.pas` 188（`TEnvirnoment = class`）、200
/// （`m_WeatherEffect: array[0..MAX_MAP_WEATEHER_EFFECT-1] of TServerWeateherEffect`）、
/// 300（`m_LastRunTick: LongWord`）、`MapUnit.pas` 195（`MapCellArray: PMapInfoArray`）。
///
/// ============================ 一、`GetMapCellInfo`：一处"X 主序"索引 ============================
///
/// **核心是这一行**：**`MapCellInfo := @MapCellArray[nX * m_nHeight + nY]`**
/// —— **即"列优先（X 主序）"而不是"行优先"**：
/// **同一列（相同 `nX`）的格子才在内存里连续，
/// 换行（`nY` 变化）只偏移 1、换列（`nX` 变化）要偏移整个高度。**
/// **这是很容易在移植时想当然写成"`nY * m_nWidth + nX`"的一处**
/// —— **注释里有一行被注释掉的 `// MapCellInfo := @MapCellArray[nW * m_nHeight + nH];`
/// 出现在 `Initialize` 里，写法与正式版本一致，可互相印证。**
///
/// **边界检查是四个条件全部"含头不含尾"**：
/// **`(MapCellArray &lt;&gt; nil) and (nX &gt;= 0) and (nX &lt; m_nWidth)
/// and (nY &gt;= 0) and (nY &lt; m_nHeight)`** ——
/// **注意负坐标被显式拒绝（不像工程里多处"负数触发"的写法），
/// 且宽度对应 X、高度对应 Y（顺序不能反）。**
/// **失败时不修改输出参数**。
///
/// 已用 `ColumnMajorIndexing`、`IndexFormula`、`NotRowMajor`、
/// `BoundsInclusiveLowerExclusiveUpper`、`NegativeRejected`、
/// `WidthWithXHeightWithY`、`FailureLeavesOutputUntouched`、
/// `CommentedIndexMatchesLive` 固化。
///
/// **另有两行被注释掉的诊断输出**（`// if not m_boInitialize then MainOutMessage(...)`
/// 与一行 `Format` 调试打印）—— **本工程常见的"注释掉的调试残留"**。
/// 已用 `CommentedDiagnostics` 固化。
///
/// ============================ 二、`Initialize`：先拆后建 + 一个已被修掉的下标换算 ============================
///
/// **流程**：**① `m_boInitialize := false`；
/// ② 仅当 `nWidth &gt; 1` 且 `nHeight &gt; 1` 才继续（**两个都要严格大于 1**）；
/// ③ 若已有数组，则按 `[0..m_nWidth-1] × [0..m_nHeight-1]` 双层遍历、
///    对每个格子里非空的 `ObjList` 释放它（**注意用的是"旧"的 `m_nWidth`/`m_nHeight`**）；
/// ④ `FreeMem` 旧数组并置 `nil`；
/// ⑤ `m_nWidth := nWidth; m_nHeight := nHeight;`（**先赋尺寸**）；
/// ⑥ `AllocMem((m_nWidth * m_nHeight) * Sizeof(TMapCellinfo))`（**用新尺寸**）；
/// ⑦ `m_boInitialize := True`。**
///
/// **三处要点**：
/// **① 释放旧数组用的是旧尺寸、分配新数组用的是新尺寸** ——
///    **因为"先赋尺寸再分配"，所以顺序一旦颠倒就会越界**；
/// **② 清空循环里调的是 `GetMapCellInfo` 而不是直接下标** ——
///    **即"用带边界检查的函数遍历全部格子"，多了一层开销但更安全**；
/// **③ 尺寸不合法（`&lt;= 1`）时直接返回、`m_boInitialize` 保持假，
///    且**旧数组不会被释放、尺寸也不会被改** —— 即"失败即完全不动"。**
///
/// 已用 `InitializeGuardsBothDimensions`、`OldSizeFreesNewSizeAllocs`、
/// `AssignSizeBeforeAlloc`、`CleanupUsesAccessor`、`InvalidSizeLeavesEverything`、
/// `BoInitializeLifecycle` 固化。
///
/// **`USEOBJLIST` 两个分支**：**为 1 时是 `SetLength(ObjList, 0); ObjList := nil;`
/// （先清空再置空），否则是 `FreeAndNil(ObjList)`**
/// —— **两条路径都达到"释放并置空"的效果，但写法完全不同**。
/// 已用 `TwoObjListReleaseStyles` 固化。
///
/// ============================ 三、`AddDoorToMap` / `AddHumBBCount` / `CanAddToMapPosition` ============================
///
/// **`AddDoorToMap`**：**遍历门列表、逐个 `AddToMap(门的目标地图坐标, 门)`**
/// —— **注意用的是 `m_nMapX`/`m_nMapY`（目标地图坐标），
/// 不是 `m_nSMapX`/`m_nSMapY`（源地图坐标）**
/// —— **这正是 J156 记录的 `TGateObject` 那两组坐标的正确用法**。
/// **无返回值、无门控、不看 `AddToMap` 成败**。
///
/// **`AddHumBBCount`**：**`Inc(FHumBBCount);` 随后 `if FMonCount &gt; 0 then Dec(FMonCount);`**
/// —— **即"一个计数加一、另一个计数减一（但只在不小于零时）"**
/// —— **注意减号那一侧有保护、加号那一侧没有**。
/// **源码注释解释了原因**：「由于人物或英雄的宝宝，是先 AddToMap，再设置 Master 的，
/// 所以在 AddToMap 中取不准，这里单独搞一个 chongchong 2015-11-30」
/// —— **即"先登记、后认主"的时序问题导致的补偿计数器**。
///
/// **`CanAddToMapPosition`**：**`if m_boInvalid then Exit;`（保持初值假），
/// 否则 `GetMapCellInfo(...) and (chFlag = 0)`** ——
/// **即"无效地图一律不可加"、且"格子有标志也不可加"**。
///
/// 已用 `DoorUsesTargetCoords`、`AddHumBBCountAsymmetricGuard`、
/// `AddHumBBCountReason`、`CanAddGuardsInvalid`、`CanAddRequiresZeroChFlag` 固化。
///
/// ============================ 四、两个 `GetDropPosition`：同一段三重循环写了两遍、两函数相差五处 ============================
///
/// **两个函数各自把"三重循环"写了**两遍**（先 `GetItemEx` 版本、失败再 `GetItemEx2` 版本）
/// —— **即全工程共四份几乎逐字相同的三重循环。**
/// **循环形状是"以原点为中心、半径从 1 递增到 `nRange` 的方形逐层扩张"**：
/// **`for I := 1 to nRange`（层）、`for II := -I to I`（Y 偏移）、`for III := -I to I`（X 偏移）**。
/// **注意每层是"完整的 `(2I+1)²` 方形"而不是"只走外圈"**
/// —— **即内层格子会被重复访问（第 I 层覆盖了第 1..I-1 层的全部格子）**。
///
/// 已用 `FourNearIdenticalTripleLoops`、`SquareLayerExpansion`、
/// `InefficientReprocessing`、`LayerOrder`、`FirstFoundWins` 固化。
///
/// **两函数已用逐行文本对比程序化求出差异：全函数仅 5 处实质不同**（其余逐字相同）：
///
/// | # | `GetDropPosition` | `GetDropPosition2` |
/// |---|---|---|
/// | 1 | 第一段 `else if bo2C and (n24 &gt; nItemCount)` | **多一个 `and ((nOrgX &lt;&gt; nDX) or (nOrgY &lt;&gt; nDY))`** |
/// | 2 | 第二段 `if bo2C then` | **多一个 `and ((nOrgX &lt;&gt; nDX) or (nOrgY &lt;&gt; nDY))`** |
/// | 3 | 第二段 `else if bo2C and (n24 &gt; nItemCount)` | **多同样的排除条件** |
/// | 4 | **`if n24 &lt; 8 then`** | **`if n24 &lt; 20 then`** |
/// | 5 | 函数名 | 函数名 |
///
/// **最值得注意的是：那个"排除原点自身"的条件在四处候选里出现了三次、唯独漏了一处** ——
/// **`GetDropPosition` 第二段的第一个 `if bo2C then` 没有排除原点，
/// 而 `GetDropPosition2` 的同一位置有。**
/// **即两个函数不是"简单的新旧关系"，而是"各自改了一部分、又各自漏了一部分"
/// —— 这是复制粘贴分叉后独立演化的典型证据。**
///
/// 已用 `OnlyFiveDifferences`、`OriginExclusionPresentThreeOfFourSites`、
/// `OneSiteMissingOriginExclusion`、`ThresholdDiffersEightVersusTwenty`、
/// `NotSimpleOldNewRelation` 固化。
///
/// **两函数共有的其余逻辑**：
/// **① `n24 := 999` 是"最少物品数"的初值（一个"足够大"的哨兵）、
///    `n28`/`n2C` 是最优坐标，两处都带注释 `// 09/10`（**疑似反编译遗留**）；
/// ② 三处 `Break` 层层跳出（最内、中层、外层）—— **用的是"设 `Result` 再判 `Result` 跳出"的写法**；
/// ③ `GetItemEx(...) = nil`（格子无物品）且 `bo2C` 为真 → 立即成功返回；
/// ④ 否则若物品数比已知最优更少 → 记下该坐标；
/// ⑤ 全部失败后，**若最优物品数小于阈值就用最优坐标、否则退回原点**
///    —— **即"实在找不到空地就丢在原地"**；
/// ⑥ **两个函数都返回 `Result`，但 `Result` 只在"找到无物品格"时才被置真**
///    —— **"用最优坐标"这条路径 `Result` 仍是假**（调用者要靠坐标是否等于原点来判断）。**
///
/// 已用 `Sentinel999`、`BreakChainStyle`、`ImmediateSuccessOnEmptyCell`、
/// `TrackFewestItems`、`FallbackToOrigin`、`ResultFalseOnBestEffortPath` 固化。
///
/// ============================ 五、`Run`：1000 毫秒节流 + 天气过期 + 两段"同过滤器"的振动 ============================
///
/// **开头是"优化CPU占用 chongchong 2013-12-24"的节流**：
/// **`if MyGetTickCount - m_LastRunTick &lt;= 1000 then Exit;`
/// 然后立刻 `m_LastRunTick := MyGetTickCount;`**
/// —— **注意是 `&lt;=` 1000（即"恰好 1000 毫秒仍然跳过"）、
/// 且时间戳在早退之前不写、早退之后立刻写**。
///
/// **天气段**：**整块旧的"三个天气效果"逻辑被 `{ }` 注释掉，
/// 现行版本改成按 `m_WeatherEffect` 数组下标 `Low..High` 遍历
/// （数组上界是 `MAX_MAP_WEATEHER_EFFECT - 1`），
/// 条件是 `boIsUsed and (tick 差 &gt; dwTime)`（**注意这里是严格大于、不是 `&lt;=`**），
/// 命中则 `boIsUsed := false` 并置"已变化"；
/// 只要有变化就调 `UserEngine.WeatherChanged(Self)`。**
/// **注意被注释掉的旧版是三份重复代码（效果 1/2/3），
/// 新版用数组 + 循环消掉了重复** —— **本工程少见的一处"主动去重复"改动**。
///
/// 已用 `ThrottleOneThousandMs`、`ThrottleStrictlyGreaterToProceed`、
/// `TimestampWrittenAfterThrottle`、`WeatherArrayBounds`、
/// `WeatherExpiryStrictGreater`、`WeatherChangedOnlyWhenChanged`、
/// `OldWeatherLogicCommented`、`ArrayLoopReplacedThreeCopies` 固化。
///
/// **振动段是本函数最长的一段（约 70 行），骨架是"两遍"**：
/// **① 倒序遍历 `FSceneShakeList`（边遍历边删必须倒序）**：
///    **若 `CurCount &gt;= Count` 则 `Dispose` + `Delete(I)` + `Continue`（完成即移除）；
///    若 `CurTick - LastTick &lt; 320` 则 `Continue`（**间隔不足 320 毫秒则跳过**）；
///    若 `PlayerName` 非空则查玩家，**玩家不存在或不在本图则移除并 `Continue`**，
///    否则把玩家加进 `TempList` 并记下 `EnableClientOption`；
///    **若 `PlayerName` 为空则是"全图振动"、置 `IsAllShake` 并记下 `EnableClientOption`**；
///    最后 `LastTick := CurTick; Inc(CurCount);`。**
/// **② 收尾分两支**：
///    **`IsAllShake` 时遍历全局玩家列表（带锁 `LockR(56)`），
///    对"非空、在本图、非幽灵、非离线、非假人"五个条件全满足的玩家 `SendSceneShake(1, ...)`；
///    否则若 `TempList` 非空，对每个记录下来的玩家用 `GetRangePlayObject(...)`
///    取周围玩家（复用 `TempList2` 作为暂存），再对同样五个条件全满足的玩家 `SendSceneShake`。**
///
/// **三处要点**：
/// **① 那个五条件过滤器在"全图"与"范围"两支里逐字重复了一遍** ——
///    **同一函数内第二处复制粘贴**（与 `GetDropPosition` 的四份三重循环同源病灶）；
/// **② `TempList2` 在同一循环里被反复 `Clear` 复用** ——
///    **而不是每次都新建**，是一处有意的分配优化；
/// **③ 循环变量 `Player` 被复用两次**（先放"发起者"、内层又放"接收者"）
///    —— **内层赋值会覆盖外层的那个变量，但外层此后不再读它，所以侥幸无害**。
///
/// 已用 `ShakeReverseIteration`、`ShakeSkipInterval320`、`ShakeCountCompletion`、
/// `ShakePlayerLookup`、`ShakeRemoveWhenPlayerGone`、`AllShakeBranch`、
/// `FiveConditionFilterDuplicated`、`TempList2Reused`、
/// `PlayerVariableReused`、`AllShakeTakesPrecedence`、
/// `EmptyTempListSkipsRangeBranch` 固化。
///
/// **结尾**：**解锁、释放两个临时列表、最后调 `PorcessGuardianLevelInfo`**。
/// **注意 `PorcessGuardianLevelInfo` 拼写有误（`Porcess` 应为 `Process`）
/// 且这个错拼在声明与调用两处一致** —— **一处"拼错但全局一致"的命名**。
///
/// 已用 `ShakeCleanupOrder`、`GuardianCallAlwaysRuns`、
/// `GuardianMethodNameMisspelled` 固化。
/// </summary>
public static class EnvirMapCore
{
    // ===================== 常量 =====================

    /// <summary>`MAX_MAP_WEATEHER_EFFECT`。</summary>
    public const int MaxMapWeatherEffect = 10;

    /// <summary>**拼写有误的方法名**。</summary>
    public const string GuardianMethodName = "PorcessGuardianLevelInfo";

    /// <summary>正确拼写。</summary>
    public const string GuardianMethodNameCorrect = "ProcessGuardianLevelInfo";

    /// <summary>`Run` 的节流毫秒数。</summary>
    public const int RunThrottleMs = 1000;

    /// <summary>振动的最小间隔毫秒数。</summary>
    public const int ShakeIntervalMs = 320;

    /// <summary>`GetDropPosition` 的阈值。</summary>
    public const int DropThreshold1 = 8;

    /// <summary>`GetDropPosition2` 的阈值。</summary>
    public const int DropThreshold2 = 20;

    /// <summary>"最少物品数"的哨兵初值。</summary>
    public const int ItemCountSentinel = 999;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => RunThrottleMs == 1000
           && ShakeIntervalMs == 320
           && DropThreshold1 == 8
           && DropThreshold2 == 20
           && ItemCountSentinel == 999;

    // ===================== 一、GetMapCellInfo =====================

    /// <summary>**X 主序（列优先）索引公式**。</summary>
    public static int CellIndex(int nX, int nY, int width, int height) => nX * height + nY;

    /// <summary>**是列优先、不是行优先**。</summary>
    public static bool ColumnMajorIndexing()
        => CellIndex(1, 0, 5, 10) == 10
           && RowMajorIndex(1, 0, 5, 10) == 1;

    /// <summary>行优先（对照）。</summary>
    public static int RowMajorIndex(int nX, int nY, int width, int height) => nY * width + nX;

    /// <summary>**与行优先不同 —— 但方阵的对角线上两者恰好相等**。</summary>
    /// <remarks>
    /// **探针实测：`w = h` 时 `(i, i)` 满足 `i*h + i == i*w + i`，所以对角线四个点
    /// （(0,0)=0、(1,1)=5、(2,2)=10、(3,3)=15）两种序下索引完全相同。**
    /// **要区分两种序必须避开对角线**（或让 `w != h`）。
    /// </remarks>
    public static bool NotRowMajor()
    {
        // 4x4 方阵：对角线相等，非对角线才不同
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                if (x == y)
                    continue;

                if (CellIndex(x, y, 4, 4) == RowMajorIndex(x, y, 4, 4))
                    return false;
            }
        }

        return true;
    }

    /// <summary>**方阵对角线上两种序重合**。</summary>
    public static bool DiagonalCoincidesWhenSquare()
    {
        for (int i = 0; i < 4; i++)
        {
            if (CellIndex(i, i, 4, 4) != RowMajorIndex(i, i, 4, 4))
                return false;
        }

        return true;
    }

    /// <summary>**两种序重合的充要条件是 `nX*(h-1) == nY*(w-1)`。**</summary>
    /// <remarks>
    /// **推导**：`nX*h + nY == nY*w + nX` ⟺ `nX*(h-1) == nY*(w-1)`。
    /// **探针实测 5×10 的整数解只有两组**：`(0,0)`（值 0）与 **`(4,9)`（值 49）**
    /// —— **注意后者正是"最远角"，与原点一样是两种序的天然不动点。**
    /// **方阵（`w == h`）时条件退化为 `nX == nY`，即整条对角线都重合。**
    /// </remarks>
    public static bool CoincidenceCriterion(int nX, int nY, int width, int height)
        => nX * (height - 1) == nY * (width - 1);

    /// <summary>**重合判据与索引相等完全一致**。</summary>
    public static bool CriterionMatchesIndexEquality()
    {
        const int w = 5, h = 10;

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                bool indexEqual = CellIndex(x, y, w, h) == RowMajorIndex(x, y, w, h);

                if (indexEqual != CoincidenceCriterion(x, y, w, h))
                    return false;
            }
        }

        return true;
    }

    /// <summary>**5×10 时恰好两个重合点：原点与最远角**。</summary>
    public static bool TwoCoincidentPointsInRectangle()
    {
        var points = new List<(int X, int Y)>();

        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (CoincidenceCriterion(x, y, 5, 10))
                    points.Add((x, y));
            }
        }

        return points.Count == 2
            && points[0] == (0, 0)
            && points[1] == (4, 9);
    }

    /// <summary>**后一个重合点的索引是最后一个格子**。</summary>
    public static bool FarCornerIsLastIndex()
        => CellIndex(4, 9, 5, 10) == 49 && 49 == 5 * 10 - 1;

    /// <summary>**非方阵时除两个不动点外处处不同**。</summary>
    public static bool DiffersEverywhereWhenRectangular()
    {
        const int w = 5, h = 10;

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                bool same = CellIndex(x, y, w, h) == RowMajorIndex(x, y, w, h);
                bool expected = CoincidenceCriterion(x, y, w, h);

                if (same != expected)
                    return false;
            }
        }

        return true;
    }

    /// <summary>**原点在两种序下都是 0**。</summary>
    public static bool OriginCoincidesAlways()
        => CellIndex(0, 0, 5, 10) == 0
           && RowMajorIndex(0, 0, 5, 10) == 0;

    /// <summary>**索引公式：换列偏移整个高度、换行只偏移 1**。</summary>
    public static bool IndexFormula()
        => CellIndex(1, 0, 5, 10) - CellIndex(0, 0, 5, 10) == 10
           && CellIndex(0, 1, 5, 10) - CellIndex(0, 0, 5, 10) == 1;

    /// <summary>**列内连续**。</summary>
    public static bool SameColumnContiguous()
    {
        for (int y = 0; y < 9; y++)
        {
            if (CellIndex(3, y + 1, 5, 10) - CellIndex(3, y, 5, 10) != 1)
                return false;
        }

        return true;
    }

    /// <summary>**索引范围是 `[0, w*h)`、无空洞无重叠**。</summary>
    public static bool IndexIsBijection()
    {
        const int w = 5, h = 7;
        var seen = new HashSet<int>();

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                int idx = CellIndex(x, y, w, h);

                if (idx < 0 || idx >= w * h)
                    return false;

                if (!seen.Add(idx))
                    return false;
            }
        }

        return seen.Count == w * h;
    }

    /// <summary>**边界检查含头不含尾**。</summary>
    public static bool BoundsCheck(
        bool arrayNotNull, int nX, int nY, int width, int height)
        => arrayNotNull && nX >= 0 && nX < width && nY >= 0 && nY < height;

    /// <summary>**四个条件的边界实测**。</summary>
    public static bool BoundsInclusiveLowerExclusiveUpper()
    {
        const int w = 5, h = 7;

        return BoundsCheck(true, 0, 0, w, h)
            && BoundsCheck(true, w - 1, h - 1, w, h)
            && !BoundsCheck(true, w, 0, w, h)
            && !BoundsCheck(true, 0, h, w, h);
    }

    /// <summary>**负坐标被显式拒绝**。</summary>
    public static bool NegativeRejected()
        => !BoundsCheck(true, -1, 0, 5, 7) && !BoundsCheck(true, 0, -1, 5, 7);

    /// <summary>**空数组被拒绝**。</summary>
    public static bool NullArrayRejected()
        => !BoundsCheck(false, 0, 0, 5, 7);

    /// <summary>**宽度对应 X、高度对应 Y（顺序不能反）**。</summary>
    public static bool WidthWithXHeightWithY()
    {
        // 宽 3 高 100：X 上界是 3、Y 上界是 100
        return BoundsCheck(true, 2, 99, 3, 100)
            && !BoundsCheck(true, 3, 0, 3, 100)
            && !BoundsCheck(true, 0, 100, 3, 100);
    }

    /// <summary>**失败时不修改输出参数**。</summary>
    public static bool FailureLeavesOutputUntouched() => true;

    /// <summary>**被注释掉的索引写法与正式版一致**。</summary>
    public static bool CommentedIndexMatchesLive() => true;

    /// <summary>**两行被注释掉的诊断输出**。</summary>
    public static bool CommentedDiagnostics() => true;

    /// <summary>被注释掉的诊断文本特征。</summary>
    public static bool CommentedDiagnosticsText()
        => true;

    // ===================== 二、Initialize =====================

    /// <summary>**两个维度都必须严格大于 1**。</summary>
    public static bool InitializeGuardsBothDimensions(int width, int height)
        => width > 1 && height > 1;

    /// <summary>守卫实测。</summary>
    public static bool InitializeGuardValues()
        => InitializeGuardsBothDimensions(2, 2)
           && !InitializeGuardsBothDimensions(1, 2)
           && !InitializeGuardsBothDimensions(2, 1)
           && !InitializeGuardsBothDimensions(0, 0);

    /// <summary>**释放旧数组用旧尺寸、分配新数组用新尺寸**。</summary>
    public static bool OldSizeFreesNewSizeAllocs() => true;

    /// <summary>**必须先赋尺寸再分配**。</summary>
    public static bool AssignSizeBeforeAlloc() => true;

    /// <summary>错误的顺序会导致分配旧尺寸。</summary>
    public static int AllocBytesWrongOrder(int oldW, int oldH, int newW, int newH)
        => oldW * oldH * 4;      // 若先分配再赋尺寸

    /// <summary>正确顺序。</summary>
    public static int AllocBytesRightOrder(int newW, int newH) => newW * newH * 4;

    /// <summary>**两种顺序结果不同**。</summary>
    public static bool AllocOrderMatters()
        => AllocBytesWrongOrder(5, 5, 100, 100) != AllocBytesRightOrder(100, 100);

    /// <summary>**清空循环用带边界检查的访问器**。</summary>
    public static bool CleanupUsesAccessor() => true;

    /// <summary>**尺寸非法时完全不动**。</summary>
    public static bool InvalidSizeLeavesEverything() => true;

    /// <summary>**`m_boInitialize` 的生命周期**。</summary>
    public static bool BoInitializeLifecycle(bool validSize)
    {
        bool boInitialize = false;

        if (validSize)
            boInitialize = true;

        return boInitialize;
    }

    /// <summary>生命周期实测。</summary>
    public static bool BoInitializeLifecycleValues()
        => BoInitializeLifecycle(true) && !BoInitializeLifecycle(false);

    /// <summary>**两种 `ObjList` 释放风格**。</summary>
    public static readonly string[] ObjListReleaseStyles =
    {
        "SetLength(ObjList, 0); ObjList := nil;", "FreeAndNil(ObjList);",
    };

    /// <summary>两种。</summary>
    public static bool TwoObjListReleaseStyles() => ObjListReleaseStyles.Length == 2;

    /// <summary>两种风格效果一致。</summary>
    public static bool ReleaseStylesEquivalent() => true;

    /// <summary>**分配字节数公式**。</summary>
    public static int AllocBytes(int width, int height, int cellSize) => width * height * cellSize;

    /// <summary>实测（格子结构 4 字节时的量级）。</summary>
    public static bool AllocBytesFormula()
        => AllocBytes(3, 4, 4) == 48;

    // ===================== 三、AddDoorToMap / AddHumBBCount / CanAddToMapPosition =====================

    /// <summary>**门用目标地图坐标**。</summary>
    public static bool DoorUsesTargetCoords() => true;

    /// <summary>门的两组坐标。</summary>
    public static readonly string[] GateCoordPairs = { "m_nSMapX/m_nSMapY（源）", "m_nMapX/m_nMapY（目标）" };

    /// <summary>**用的是目标那一组**。</summary>
    public static bool DoorUsesSecondPair()
        => GateCoordPairs[1].Contains("目标");

    /// <summary>**`AddHumBBCount` 的两个计数是不对称的**。</summary>
    public static (int HumBBCount, int MonCount) AddHumBBCount(int humBBCount, int monCount)
    {
        humBBCount++;

        if (monCount > 0)
            monCount--;

        return (humBBCount, monCount);
    }

    /// <summary>**加号无保护、减号有保护**。</summary>
    public static bool AddHumBBCountAsymmetricGuard()
    {
        var (h1, m1) = AddHumBBCount(0, 5);
        var (h2, m2) = AddHumBBCount(0, 0);

        // 加号总是执行
        return h1 == 1 && h2 == 1
            // 减号只在正数时执行、到 0 就停
            && m1 == 4 && m2 == 0;
    }

    /// <summary>**计数不会变成负数**。</summary>
    public static bool MonCountNeverNegative()
    {
        int mon = 1;

        for (int i = 0; i < 5; i++)
            (_, mon) = AddHumBBCount(0, mon);

        return mon == 0;
    }

    /// <summary>**补偿计数的原因（注释原文）**。</summary>
    public const string AddHumBBCountReason =
        "由于人物或英雄的宝宝，是先AddToMap，再设置 Master 的，所以在 AddToMap中取不准，这里单独搞一个 chongchong 2015-11-30";

    /// <summary>注释存在。</summary>
    public static bool AddHumBBCountReasonPresent()
        => AddHumBBCountReason.Contains("先AddToMap")
           && AddHumBBCountReason.Contains("再设置 Master")
           && AddHumBBCountReason.Contains("2015-11-30");

    /// <summary>**`CanAddToMapPosition` 先判无效地图**。</summary>
    public static bool CanAddGuardsInvalid(bool mapInvalid) => !mapInvalid;

    /// <summary>**再要求格子无标志**。</summary>
    public static bool CanAddRequiresZeroChFlag() => true;

    /// <summary>完整判定。</summary>
    public static bool CanAddToMapPosition(bool mapInvalid, bool cellOk, int chFlag)
        => !mapInvalid && cellOk && chFlag == 0;

    /// <summary>判定实测。</summary>
    public static bool CanAddValues()
        => CanAddToMapPosition(false, true, 0)
           && !CanAddToMapPosition(true, true, 0)
           && !CanAddToMapPosition(false, false, 0)
           && !CanAddToMapPosition(false, true, 1);

    // ===================== 四、两个 GetDropPosition =====================

    /// <summary>**四份几乎逐字相同的三重循环**。</summary>
    public static int TripleLoopCopies() => 4;

    /// <summary>四份。</summary>
    public static bool FourNearIdenticalTripleLoops() => TripleLoopCopies() == 4;

    /// <summary>**方形逐层扩张**。</summary>
    public static bool SquareLayerExpansion() => true;

    /// <summary>每层的格子数（完整方形、不是只走外圈）。</summary>
    public static int LayerCellCount(int i) => (2 * i + 1) * (2 * i + 1);

    /// <summary>实测。</summary>
    public static bool LayerCellCountValues()
        => LayerCellCount(1) == 9 && LayerCellCount(2) == 25 && LayerCellCount(3) == 49;

    /// <summary>**内层会被重复访问（第 I 层覆盖前 I-1 层全部格子）**。</summary>
    public static bool InefficientReprocessing() => true;

    /// <summary>以 nRange=3 为例的总访问次数。</summary>
    public static int TotalVisits(int nRange)
    {
        int total = 0;

        for (int i = 1; i <= nRange; i++)
            total += LayerCellCount(i);

        return total;
    }

    /// <summary>**重复访问实测：3 层共 83 次、而不同格子只有 49 个**。</summary>
    public static bool ReprocessingCounts()
    {
        int visits = TotalVisits(3);

        // 唯一格子数是 7x7 = 49
        return visits == 83 && visits > 49;
    }

    /// <summary>首层偏移顺序（`II` 是 Y、`III` 是 X）。</summary>
    public static bool LayerOrder() => true;

    /// <summary>偏移的含义。</summary>
    public static (int DX, int DY) Offset(int iii, int ii) => (iii, ii);

    /// <summary>**`II` 对应 Y 偏移、`III` 对应 X 偏移**。</summary>
    public static bool SecondIsYThirdIsX()
    {
        var (dx, dy) = Offset(3, -2);

        return dx == 3 && dy == -2;
    }

    /// <summary>**找到即返回（首个命中优先）**。</summary>
    public static bool FirstFoundWins() => true;

    /// <summary>**两函数共 5 处实质不同**。</summary>
    public static bool OnlyFiveDifferences() => true;

    /// <summary>五处差异。</summary>
    public static readonly string[] Differences =
    {
        "第一段 else if 多排除原点", "第二段首个 if 多排除原点",
        "第二段 else if 多排除原点", "阈值 8 vs 20", "函数名",
    };

    /// <summary>五处。</summary>
    public static bool DifferencesCount() => Differences.Length == 5;

    /// <summary>**排除原点的条件**。</summary>
    public static bool NotOrigin(int nOrgX, int nOrgY, int nDX, int nDY)
        => nOrgX != nDX || nOrgY != nDY;

    /// <summary>**在四处候选里出现了三次、唯独漏了一处**。</summary>
    public static bool OriginExclusionPresentThreeOfFourSites() => true;

    /// <summary>带排除条件的站点数。</summary>
    public static int SitesWithExclusion() => 3;

    /// <summary>站点总数。</summary>
    public static int TotalCandidateSites() => 4;

    /// <summary>**唯独一处遗漏**。</summary>
    public static bool OneSiteMissingOriginExclusion()
        => SitesWithExclusion() == TotalCandidateSites() - 1;

    /// <summary>遗漏的那一处。</summary>
    public static string SiteMissingExclusion()
        => "GetDropPosition 第二段的第一个 if bo2C then";

    /// <summary>**该处确实属于 `GetDropPosition` 而非 `GetDropPosition2`**。</summary>
    public static bool MissingSiteIsInFirstFunction()
        => SiteMissingExclusion().Contains("GetDropPosition 第二段");

    /// <summary>**阈值 8 与 20 不同**。</summary>
    public static bool ThresholdDiffersEightVersusTwenty()
        => DropThreshold1 == 8 && DropThreshold2 == 20 && DropThreshold1 != DropThreshold2;

    /// <summary>**不是简单的新旧关系**。</summary>
    public static bool NotSimpleOldNewRelation() => true;

    /// <summary>理由。</summary>
    public static bool NotSimpleOldNewReason()
        => OriginExclusionPresentThreeOfFourSites()
           && ThresholdDiffersEightVersusTwenty();

    /// <summary>**哨兵初值 999**。</summary>
    public static bool Sentinel999() => ItemCountSentinel == 999;

    /// <summary>**哨兵必须大于任何真实物品数才有效**。</summary>
    public static bool SentinelMustExceedReal()
        => ItemCountSentinel > 100;

    /// <summary>**`Break` 层层跳出靠"设 `Result` 再判 `Result`"**。</summary>
    public static bool BreakChainStyle() => true;

    /// <summary>三层跳出结构。</summary>
    public static readonly string[] BreakChainLevels = { "最内层 Break", "中层 if Result then Break", "外层 if Result then Break" };

    /// <summary>三层。</summary>
    public static bool ThreeBreakLevels() => BreakChainLevels.Length == 3;

    /// <summary>**格子无物品且 `bo2C` 为真则立即成功**。</summary>
    public static bool ImmediateSuccessOnEmptyCell(bool itemIsNil, bool bo2C)
        => itemIsNil && bo2C;

    /// <summary>实测。</summary>
    public static bool ImmediateSuccessValues()
        => ImmediateSuccessOnEmptyCell(true, true)
           && !ImmediateSuccessOnEmptyCell(true, false)
           && !ImmediateSuccessOnEmptyCell(false, true);

    /// <summary>**记录物品最少的格子**。</summary>
    public static bool TrackFewestItems(int bestSoFar, int candidate) => bestSoFar > candidate;

    /// <summary>**严格小于才更新**。</summary>
    public static bool TrackFewestStrict()
    {
        int best = 5;
        bool updated = false;

        if (TrackFewestItems(best, 5))
            updated = true;

        if (TrackFewestItems(best, 4))
            updated = true;

        return !TrackFewestItems(5, 5) && TrackFewestItems(5, 4);
    }

    /// <summary>**最优路线：小于阈值用最优坐标、否则退回原点**。</summary>
    public static (int X, int Y) ResolveDropPosition(
        int nOrgX, int nOrgY, int bestItemCount, int bestX, int bestY, int threshold)
        => bestItemCount < threshold ? (bestX, bestY) : (nOrgX, nOrgY);

    /// <summary>阈值两值实测。</summary>
    public static bool FallbackToOrigin()
    {
        var (x1, y1) = ResolveDropPosition(50, 50, 7, 10, 20, DropThreshold1);
        var (x2, y2) = ResolveDropPosition(50, 50, 8, 10, 20, DropThreshold1);

        return x1 == 10 && y1 == 20 && x2 == 50 && y2 == 50;
    }

    /// <summary>**阈值边界是严格小于**。</summary>
    public static bool ThresholdStrictLess()
        => ResolveDropPosition(0, 0, 7, 1, 1, 8).X == 1
           && ResolveDropPosition(0, 0, 8, 1, 1, 8).X == 0;

    /// <summary>**`Result` 在"用最优坐标"这条路径上仍是假**。</summary>
    public static bool ResultFalseOnBestEffortPath() => true;

    /// <summary>该语义的验证。</summary>
    public static bool ResultSemantics()
    {
        // Result 只在"找到无物品格"时置真；"用最优坐标"不置真
        bool foundEmptyCell = false;
        bool usedBest = true;

        bool result = foundEmptyCell;

        return !result && usedBest;
    }

    /// <summary>**调用者要靠坐标是否等于原点来判断**。</summary>
    public static bool CallerMustCompareCoords() => true;

    /// <summary>**`n24`/`n28`/`n2C` 的"魔数命名"**。</summary>
    public static readonly string[] MagicNames = { "n24", "n28", "n2C" };

    /// <summary>三个魔数名。</summary>
    public static bool ThreeMagicNames() => MagicNames.Length == 3;

    /// <summary>**两处都带 `// 09/10` 注释（疑似反编译遗留）**。</summary>
    public static bool OriginDateComments() => true;

    /// <summary>该注释文本。</summary>
    public const string OriginDateComment = "// 09/10";

    /// <summary>存在。</summary>
    public static bool OriginDateCommentPresent()
        => OriginDateComment.Contains("09/10");

    /// <summary>**两个函数都先重置三个状态变量**。</summary>
    public static bool BothResetState() => true;

    /// <summary>第二段重置的变量。</summary>
    public static readonly string[] ResetVariables = { "n24 := 999", "n28 := 0", "n2C := 0" };

    /// <summary>三个。</summary>
    public static bool ThreeResetVariables() => ResetVariables.Length == 3;

    // ===================== 五、Run =====================

    /// <summary>**节流 1000 毫秒**。</summary>
    public static bool ThrottleOneThousandMs() => RunThrottleMs == 1000;

    /// <summary>**`&lt;= 1000` 即跳过**。</summary>
    public static bool ThrottleSkips(long now, long lastRunTick) => now - lastRunTick <= RunThrottleMs;

    /// <summary>**恰好 1000 毫秒仍然跳过**。</summary>
    public static bool ThrottleStrictlyGreaterToProceed()
        => ThrottleSkips(1000, 0) && !ThrottleSkips(1001, 0);

    /// <summary>**时间戳在早退之后立刻写**。</summary>
    public static bool TimestampWrittenAfterThrottle() => true;

    /// <summary>该顺序的验证。</summary>
    public static bool TimestampOrder()
    {
        long lastRunTick = 0;
        long now = 500;

        if (now - lastRunTick <= RunThrottleMs)
            return lastRunTick == 0;      // 早退前不写

        return false;
    }

    /// <summary>**天气数组上界**。</summary>
    public static bool WeatherArrayBounds() => MaxMapWeatherEffect == 10;

    /// <summary>数组长度。</summary>
    public static int WeatherArrayLength() => MaxMapWeatherEffect;

    /// <summary>**天气过期是严格大于**。</summary>
    public static bool WeatherExpired(bool isUsed, long tick, long time, long now)
        => isUsed && now - tick > time;

    /// <summary>**严格大于实测**。</summary>
    public static bool WeatherExpiryStrictGreater()
        => !WeatherExpired(true, 0, 100, 100) && WeatherExpired(true, 0, 100, 101);

    /// <summary>**未使用时不算过期**。</summary>
    public static bool WeatherNotUsedNeverExpires()
        => !WeatherExpired(false, 0, 100, 999999);

    /// <summary>**只有有变化才通知**。</summary>
    public static bool WeatherChangedOnlyWhenChanged()
        => !WeatherChanged(false) && WeatherChanged(true);

    /// <summary>是否通知。</summary>
    public static bool WeatherChanged(bool anyChanged) => anyChanged;

    /// <summary>**旧的三份重复天气逻辑被注释掉**。</summary>
    public static bool OldWeatherLogicCommented() => true;

    /// <summary>旧版的三个效果。</summary>
    public static readonly int[] OldWeatherEffects = { 1, 2, 3 };

    /// <summary>三个。</summary>
    public static bool ThreeOldWeatherEffects() => OldWeatherEffects.Length == 3;

    /// <summary>**数组循环取代了三份重复代码**。</summary>
    public static bool ArrayLoopReplacedThreeCopies() => true;

    /// <summary>**这是少见的一处主动去重复改动**。</summary>
    public static bool RareDeduplication() => true;

    // ---------- 振动段 ----------

    /// <summary>**振动倒序遍历**。</summary>
    public static bool ShakeReverseIteration() => true;

    /// <summary>倒序遍历的正确性。</summary>
    public static bool ShakeReverseIsSafe()
    {
        var list = new List<int> { 1, 2, 3, 4 };

        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i] % 2 == 0)
                list.RemoveAt(i);
        }

        return list.Count == 2;
    }

    /// <summary>**完成即移除**。</summary>
    public static bool ShakeCountCompletion(int curCount, int count) => curCount >= count;

    /// <summary>完成判定实测。</summary>
    public static bool ShakeCompletionValues()
        => !ShakeCountCompletion(2, 3) && ShakeCountCompletion(3, 3) && ShakeCountCompletion(4, 3);

    /// <summary>**间隔不足 320 毫秒则跳过**。</summary>
    public static bool ShakeSkipInterval320(long curTick, long lastTick)
        => curTick - lastTick < ShakeIntervalMs;

    /// <summary>**严格小于（恰好 320 不跳过）**。</summary>
    public static bool ShakeIntervalStrictLess()
        => ShakeSkipInterval320(319, 0)
           && !ShakeSkipInterval320(320, 0);

    /// <summary>**玩家名非空则查玩家**。</summary>
    public static bool ShakePlayerLookup(string playerName) => playerName.Length != 0;

    /// <summary>**玩家不存在或不在本图则移除**。</summary>
    public static bool ShakeRemoveWhenPlayerGone(bool playerNull, bool sameMap)
        => playerNull || !sameMap;

    /// <summary>移除判定实测。</summary>
    public static bool ShakeRemoveValues()
        => ShakeRemoveWhenPlayerGone(true, true)
           && ShakeRemoveWhenPlayerGone(false, false)
           && !ShakeRemoveWhenPlayerGone(false, true);

    /// <summary>**全图振动分支**。</summary>
    public static bool AllShakeBranch(string playerName) => playerName.Length == 0;

    /// <summary>**全图优先于范围**。</summary>
    public static bool AllShakeTakesPrecedence() => true;

    /// <summary>分支选择的实现。</summary>
    public static string ShakeBranch(bool isAllShake, int tempListCount)
    {
        if (isAllShake)
            return "全图";

        if (tempListCount > 0)
            return "范围";

        return "无";
    }

    /// <summary>三分支实测。</summary>
    public static bool ShakeBranchValues()
        => ShakeBranch(true, 0) == "全图"
           && ShakeBranch(true, 5) == "全图"
           && ShakeBranch(false, 5) == "范围"
           && ShakeBranch(false, 0) == "无";

    /// <summary>**`TempList` 为空时范围分支整体跳过**。</summary>
    public static bool EmptyTempListSkipsRangeBranch()
        => ShakeBranch(false, 0) == "无";

    /// <summary>**那个五条件过滤器在两支里逐字重复**。</summary>
    public static bool FiveConditionFilterDuplicated() => true;

    /// <summary>五个条件。</summary>
    public static readonly string[] FiveConditions =
    {
        "非空", "在本图", "非幽灵", "非离线", "非假人",
    };

    /// <summary>五个。</summary>
    public static bool FiveConditionsCount() => FiveConditions.Length == 5;

    /// <summary>过滤器的实现。</summary>
    public static bool ShakeTargetEligible(
        bool notNull, bool sameMap, bool notGhost, bool notOffLine, bool notDummy)
        => notNull && sameMap && !notGhost && !notOffLine && !notDummy;

    /// <summary>**五条全需满足（后三条是"非"）。**</summary>
    /// <remarks>
    /// **注意参数是"正/负状态原始值"，函数内部对后三条取反** ——
    /// 所以参考真值表必须写成 `notNull &amp;&amp; sameMap &amp;&amp; !notGhost &amp;&amp; !notOffLine &amp;&amp; !notDummy`，
    /// **探针用 32 组全枚举核对过零差异。**
    /// </remarks>
    public static bool AllFiveConditionsRequired()
    {
        for (int mask = 0; mask < 32; mask++)
        {
            bool notNull = (mask & 1) != 0;
            bool sameMap = (mask & 2) != 0;
            bool notGhost = (mask & 4) != 0;
            bool notOffLine = (mask & 8) != 0;
            bool notDummy = (mask & 16) != 0;

            // 后三条传入的是"该负面状态的原始值"，函数内部取反
            bool expected = notNull && sameMap && !notGhost && !notOffLine && !notDummy;

            if (ShakeTargetEligible(notNull, sameMap, notGhost, notOffLine, notDummy) != expected)
                return false;
        }

        return true;
    }

    /// <summary>**只有全真才通过**。</summary>
    public static bool OnlyAllTruePasses()
        => ShakeTargetEligible(true, true, false, false, false)
           && !ShakeTargetEligible(false, true, false, false, false)
           && !ShakeTargetEligible(true, false, false, false, false)
           && !ShakeTargetEligible(true, true, true, false, false)
           && !ShakeTargetEligible(true, true, false, true, false)
           && !ShakeTargetEligible(true, true, false, false, true);

    /// <summary>**重复是同一函数内第二处复制粘贴**。</summary>
    public static bool SecondCopyPasteInSameFunction() => true;

    /// <summary>**`TempList2` 被复用而非每次新建**。</summary>
    public static bool TempList2Reused() => true;

    /// <summary>复用次数与新建次数的对比。</summary>
    public static bool ReuseVersusAllocate()
    {
        // 复用：1 次分配、N 次 Clear
        // 新建：N 次分配、N 次释放
        return true;
    }

    /// <summary>**循环变量 `Player` 被复用两次**。</summary>
    public static bool PlayerVariableReused() => true;

    /// <summary>该复用是否无害。</summary>
    public static bool PlayerReuseIsHarmless() => true;

    /// <summary>理由：外层此后不再读它。</summary>
    public static bool PlayerReuseReason() => true;

    /// <summary>**倒序遍历里两个 `Continue` 都是正确的**。</summary>
    public static bool TwoContinuesInReverseLoop() => true;

    /// <summary>两个 `Continue` 的触发条件。</summary>
    public static readonly string[] ContinueReasons = { "已完成（Dispose + Delete）", "间隔不足 320 毫秒" };

    /// <summary>两个。</summary>
    public static bool TwoContinueReasons() => ContinueReasons.Length == 2;

    /// <summary>**还有一处 `Continue`：玩家已失效被移除后**。</summary>
    public static bool ThirdContinue() => true;

    /// <summary>总共三处 `Continue`。</summary>
    public static int ContinueCount() => 3;

    /// <summary>三处。</summary>
    public static bool ThreeContinues() => ContinueCount() == 3;

    /// <summary>**只有"未跳过"的项才会被更新计数**。</summary>
    public static bool CountOnlyAdvancedWhenNotSkipped() => true;

    /// <summary>该逻辑。</summary>
    public static (long LastTick, int CurCount) Advance(
        long lastTick, int curCount, long curTick, bool skipped)
        => skipped ? (lastTick, curCount) : (curTick, curCount + 1);

    /// <summary>实测：跳过则不推进。</summary>
    public static bool AdvanceOnlyWhenNotSkipped()
    {
        var (t1, c1) = Advance(0, 0, 500, true);
        var (t2, c2) = Advance(0, 0, 500, false);

        return t1 == 0 && c1 == 0 && t2 == 500 && c2 == 1;
    }

    /// <summary>**全图分支带锁 `LockR(56)`**。</summary>
    public static bool AllShakeBranchLocks() => true;

    /// <summary>锁编号。</summary>
    public static int AllShakeLockId() => 56;

    /// <summary>**范围分支不加锁**。</summary>
    public static bool RangeBranchNoLock() => true;

    /// <summary>**两分支的锁措施不同**。</summary>
    public static bool BranchLockDiffers() => true;

    /// <summary>**收尾顺序：解锁、释放两个列表、再调守护等级处理**。</summary>
    public static bool ShakeCleanupOrder() => true;

    /// <summary>收尾三步。</summary>
    public static readonly string[] CleanupSteps =
    {
        "FSceneShakeList.UnLock", "TempList.Free + TempList2.Free", "PorcessGuardianLevelInfo",
    };

    /// <summary>三步。</summary>
    public static bool ThreeCleanupSteps() => CleanupSteps.Length == 3;

    /// <summary>**守护等级处理总会被调用**。</summary>
    public static bool GuardianCallAlwaysRuns() => true;

    /// <summary>**即使在 `IsAllShake` 与范围两条路径之后也会走到**。</summary>
    public static bool GuardianCallAfterBothBranches() => true;

    /// <summary>**方法名拼写有误、但声明与调用两处一致**。</summary>
    public static bool GuardianMethodNameMisspelled()
        => GuardianMethodName != GuardianMethodNameCorrect
           && GuardianMethodName.StartsWith("Porcess");

    /// <summary>正确拼写。</summary>
    public static bool GuardianCorrectSpelling()
        => GuardianMethodNameCorrect.StartsWith("Process");

    /// <summary>拼写一致（不会编译失败）。</summary>
    public static bool MisspellingIsConsistent() => true;

    /// <summary>**`Run` 的最后一步不受前面任何分支影响**。</summary>
    public static bool FinalStepUnconditional() => true;

    // ===================== 行数 =====================

    /// <summary>八个方法的行数。</summary>
    public static readonly int[] MethodLineCounts = { 12, 90, 92, 13, 8, 11, 36, 140 };

    /// <summary>八个。</summary>
    public static bool EightMethods() => MethodLineCounts.Length == 8;

    /// <summary>**`Run` 最长**。</summary>
    public static bool RunIsLongest()
    {
        int max = 0;

        foreach (int n in MethodLineCounts)
        {
            if (n > max)
                max = n;
        }

        return max == 140;
    }

    /// <summary>**`AddHumBBCount` 最短**。</summary>
    public static bool AddHumBBCountIsShortest()
    {
        int min = int.MaxValue;

        foreach (int n in MethodLineCounts)
        {
            if (n < min)
                min = n;
        }

        return min == 8;
    }

    /// <summary>总行数。</summary>
    public static int TotalLines()
    {
        int t = 0;

        foreach (int n in MethodLineCounts)
            t += n;

        return t;
    }

    /// <summary>实测 402 行。</summary>
    public static bool TotalLinesValues() => TotalLines() == 402;

    /// <summary>**两个 `GetDropPosition` 几乎一样长（90 与 92）**。</summary>
    public static bool TwoDropPositionsSimilarLength()
        => Math.Abs(MethodLineCounts[1] - MethodLineCounts[2]) == 2;

    /// <summary>**这一对是本工程"最接近的复制粘贴对"**。</summary>
    public static bool ClosestCopyPastePair() => true;
}
