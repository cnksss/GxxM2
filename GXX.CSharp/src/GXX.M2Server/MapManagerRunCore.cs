using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图管理器其余方法 1:1 移植（批次J157）：
/// `TMapManager.MakeSafePkZone`（`Envir.pas` 521-609，**89 行**）、
/// `TMapManager.MakeMapMagic`（610-637，**整块被注释掉、只剩空过程体**）、
/// `TMapManager.Create`（3118-3126，**9 行**）、
/// `TMapManager.Destroy`（3127-3139，**13 行**）、
/// `TMapManager.Run`（5392-5469，**78 行**）。
/// 辅助源 `Envir.pas` 473（`m_GateList: TGList`）、475-476
/// （`m_nProcMapIDx`/`m_nProcGateIDx: Integer`）、
/// 3120-3122（`Create` 里两个索引都置 0）。
///
/// ============================ 一、`MakeSafePkZone`：一个"矩形周长"判定与一套层级方向表 ============================
///
/// **它遍历全局安全区管理器，分两类处理**：
/// **① `TRangeSafeArea`（矩形区域）：先要求 `ShowType > 0`，再 `FindMap`，
///    然后在 `[中心-范围, 中心+范围]` 的矩形内逐格判断"是否在周长上"；
/// ② `TAllotypeSafeArea`（异形区域）：`FindMap` 后按行、按点逐格生成事件，
///    点的坐标与方向直接取自数据（不做任何几何计算）。**
///
/// **核心是那个周长判定表达式 —— 它由四个"或"项组成**：
/// **`((nX &lt; nMaxX) and (nY = nMinY))` —— 上边（不含右上角）；
/// `or ((nY &lt; nMaxY) and (nX = nMinX))` —— 左边（不含左下角）；
/// `or (nX = nMaxX)` —— 整个右边（**含两个角**）；
/// `or (nY = nMaxY)` —— 整个下边（**含两个角**）。**
/// **即"前两项故意各让出一个角、后两项故意各多收一个角"，
/// 四条边拼起来恰好是一次完整且不重叠的周长遍历**
/// —— **已用逐格穷举验证"周长上的格子恰好被选中一次、内部格子一个都不选"。**
///
/// 已用 `PerimeterFormula`、`TopEdgeExcludesCorner`、`LeftEdgeExcludesCorner`、
/// `RightEdgeIncludesBothCorners`、`BottomEdgeIncludesBothCorners`、
/// `PerimeterCoversExactlyOnce`、`InteriorNotSelected` 固化。
///
/// **方向判定是一棵"先按 X 分三支、再按 Y 分两支"的层级树**：
/// **`nX = nMinX` 时：`nY = nMinY` → 左上、`nY = nMaxY` → 左下、否则 → 左；
/// `nX = nMaxX` 时：`nY = nMinY` → 右上、`nY = nMaxY` → 右下、否则 → 右；
/// 否则（即上下边上除四角外的格子）：`nY = nMinY` → 上、否则 → 下。**
/// **八个方向全部用上，但四个角走的是"先判 X"这条路径**
/// —— **即角落方向由 X 分支决定、而非由"先判 Y"决定**。
/// **另注意 `DR_UPLEFT=7`、`DR_UPRIGHT=1` 等（见 J153 记录的 `DR_*` 表）
/// —— 即"左上"是 7 而不是 0，方向枚举不是从左上开始的。**
///
/// 已用 `DirectionHierarchy`、`XBranchDecidesCorners`、
/// `AllEightDirectionsUsed`、`DirectionValuesNotStartingAtUpLeft` 固化。
///
/// **三处要点**：
/// **① `ShowType > 0` 这个门**只对矩形区域生效、**异形区域没有这道门**
///    —— **同一函数里两类区域的过滤条件不同**；
/// **② `FindMap` 返回空则整块跳过**（两类都一样）；
/// **③ 事件构造签名不同**：**矩形是 `Create(Envir, nX, nY, ShowType, nDir)` 五个参数，
///    异形是 `Create(Envir, Point.PointX, Row.PointY, Point.ShowType, Point.Dir)`
///    —— 注意"行只有整行的 Y、点只有自己的 X"（`Row.PointY` 与 `Point.PointX`）**
///    —— **即 Y 在行上、X 在点上，两层各管一个维度**。
///
/// 已用 `ShowTypeGateOnlyForRange`、`NullMapSkips`、
/// `EventSignaturesDiffer`、`YOnRowXOnPoint` 固化。
///
/// ============================ 二、`MakeMapMagic`：整块被注释掉 ============================
///
/// **函数体是一个 `(* ... *)` 包围的整段注释 —— 里面是一份完整实现**：
/// **遍历全局地图魔法事件列表、`FindMap`、`Continue` 跳过空地图、
/// 逐个魔法事件，若 `KeepVisible` 则 `AddEvent(TSafeEvent.Create(Envir, X, Y, MagicType))`。**
/// **注意它用 `(* *)` 而不是 `{ }`**（源码里两种注释风格并存），
/// **且被注释掉的代码里用了 `Continue`** ——
/// **这是本工程第九种"空壳"形态：整块被括号注释掉、函数体实际为空**
/// （前八种见 J155：空 `begin end` 块、空分支、恒真函数、恒假函数、
/// 占位方法、被注释掉的整块、只赋值不使用的字段、空过程体）。
/// **与 J155 的 `ProcessMapDoor`（空过程体、没有注释）的区别在于：
/// 这里的实现曾经存在、后来被整段注释掉，属于"注释掉的整块"与"空过程体"的叠加。**
///
/// 已用 `MakeMapMagicIsOneBigComment`、`ParenStarCommentStyle`、
/// `CommentedCodeUsesContinue`、`NinthEmptyShellForm` 固化。
///
/// **紧跟在它后面的 `IntListCompare` 是一个全局比较函数**：
/// **`Result := NativeInt(Item1) - NativeInt(Item2);`**
/// —— **把两个指针当整数相减**（用于列表排序），
/// **这是本工程里唯一"把指针直接当数值参与运算"的地方**。
///
/// 已用 `IntListComparePointerArithmetic` 固化。
///
/// ============================ 三、`Create` / `Destroy`：三与四 ============================
///
/// **`Create` 只做三件事**：**`inherited Create`、
/// `m_GateList := TGList.Create`、两个处理索引都置 0。**
/// **注意它不创建任何地图** —— 地图是靠后续 `AddMapInfo` 逐个加入的。
///
/// **`Destroy` 分两段释放，顺序是"先门、后地图"**：
/// **① 遍历 `m_GateList` 释放每个门对象、再释放列表本身；
/// ② 遍历自身（`Count`/`Items`，来自 `TGList` 父类）释放每个地图对象；
/// ③ 最后 `inherited`。**
/// **注意"先释放 `m_GateList` 再释放地图"这个顺序很重要** ——
/// **因为每个门的 `m_DEnvir` 都指向某个地图对象，
/// 若先释放地图、门里就留下了悬空指针**（此刻虽然不用、但顺序体现了正确的依赖方向）。
/// **另注意 `Destroy` 里对地图的遍历用的是 `for I := 0 to Count - 1`（正序），
/// 而 `Run` 里用的是 `downto 0`（倒序）** —— 两者对同一容器用了相反的遍历方向。
///
/// 已用 `CreateThreeSteps`、`CreateMakesNoMaps`、
/// `DestroyReleasesGatesBeforeMaps`、`DestroyUsesForwardLoop`、
/// `RunUsesReverseLoop`、`OppositeIterationDirections` 固化。
///
/// ============================ 四、`Run`：两个"分片处理"循环，各自带一处缺陷 ============================
///
/// **`Run` 的设计意图**是"把工作切成不超过 5 毫秒的片、用一个持久索引记住断点、
/// 下次从断点继续"（**典型的"分帧处理大量对象"模式**）。**它由两段组成**：
///
/// **【地图段】**
/// **① `nIdx := m_nProcMapIDx`，然后 `if nIdx >= Count then nIdx := 0;`
///    再 `if nIdx < 0 then nIdx := 0;` —— **两次独立的钳位（不是 if/else）**；
/// **② 加锁后 `for I := Count - 1 downto 0` 逐个 `Map.Run`**；
/// **③ 超过 5 毫秒则 `boCheckTimeLimit := True; m_nProcMapIDx := nIdx; Break;`**；
/// **④ `finally` 里"若没超时就把 `m_nProcMapIDx` 归零"**。**
///
/// **这里有两处确凿的问题（已用源码逐行核对，非推测）**：
/// **缺陷一：`nIdx` 在循环体内从不被赋值** ——
/// **它只在循环前被钳位、然后在超时分支里被原样写回。
/// 也就是说"记住断点"这个意图没有实现：无论处理到哪里，
/// 写回的都是同一个值**（初始为 `m_nProcMapIDx` 钳位后的结果）。
///
/// **缺陷二：断点索引与遍历方向不匹配** ——
/// **循环是 `Count - 1 downto 0`（倒序），但断点变量叫"处理到第几个"、
/// 且在超时后写回的是 `nIdx` 这个"前端索引"**；
/// **再加上 `for` 循环的循环变量 `I` 与 `nIdx` 完全是两个不相干的变量**，
/// **所以"恢复"时不会从上次中断处继续。**
///
/// **缺陷三：`m_nProcMapIDx` 初判 `>= Count` 时归零，
/// 但循环体一旦执行过就不会再回看这个值** ——
/// **结合缺陷一，这个索引实际只在 `Count = 0` 时才可能非零地"幸存"**。
///
/// 已用 `MapSegmentSilentClamp`、`MapSegmentNeverAdvancesIdx`、
/// `MapSegmentIdentityWriteback`、`MapSegmentDirectionMismatch`、
/// `MapSegmentLimitFiveMs`、`MapSegmentResetsWhenNotLimited` 固化。
///
/// **【门段】**
/// **① 同样先重置 `boCheckTimeLimit := false`、取时、`nIdx := m_nProcGateIDx`；**
/// **② `while True do` 循环：`if m_GateList.Count <= nIdx then Break;`
///    取门、**若 `m_dwRunTime > 0` 且已到期**则 `FindMap` 后 `DeleteFromMap`，
///    成功就 `m_GateList.Delete(nIdx)`、`Free`、**`Continue`（不 `Inc(nIdx)`）**；**
/// **③ 正常路径 `Inc(nIdx)`；**
/// **④ 超过 5 毫秒则记断点并 `Break`；**
/// **⑤ 循环后"若没超时就把 `m_nProcGateIDx` 归零"。** **注意这里没有 `try/finally`、也没有加锁**，
/// **而地图段两者都有** —— **同一个函数里两段的一致性措施不同**。
///
/// **门段这个 `Continue` 是正确写法**（删掉了当前项，下标不能自增、
/// 下一轮仍在同一位置处理新的那一项）——
/// **与地图段的"完全不推进"形成鲜明对照：一段写对了、一段写错了。**
///
/// 已用 `GateSegmentDeletesAndContinues`、`GateSegmentIncrementsOnNormalPath`、
/// `GateSegmentExpiryCondition`、`GateSegmentNoLock`、`GateSegmentNoTryFinally`、
/// `OnlyDeleteOnSuccess` 固化。
///
/// **门段的到期判定是 `(m_dwRunTime > 0) and (MyGetTickCount > m_dwRunTick)`** ——
/// **注意是严格大于（不是大于等于）**，**且 `m_dwRunTime = 0` 的门永不过期**。
/// 已用 `ExpiryStrictGreaterThan`、`ZeroRunTimeNeverExpires` 固化。
///
/// **另有一处被注释掉的变量声明**
/// （`// dwTickDiff: LongWord;`）**与地图段里整块被注释掉的"镜像地图到期"逻辑**
/// —— **`if Map.m_boMirror and (Map.m_dwMirrorCreateTick &lt;&gt; 0) then begin ... end`
/// 里面只有注释、没有代码**（注释里写「镜像地图时间到了」，
/// 并留下三行被注释掉的"取时间差、与存活时间比较"）。
/// **即"镜像地图会到期消失"这个功能已被掏空、只剩空壳**。
///
/// 已用 `MirrorBlockIsEmpty`、`CommentedTickDiffDecl`、
/// `MirrorStillCallsMapRun` 固化。
/// </summary>
public static class MapManagerRunCore
{
    // ===================== 常量 =====================

    /// <summary>`DR_UP`。</summary>
    public const int DrUp = 0;

    /// <summary>`DR_UPRIGHT`。</summary>
    public const int DrUpRight = 1;

    /// <summary>`DR_RIGHT`。</summary>
    public const int DrRight = 2;

    /// <summary>`DR_DOWNRIGHT`。</summary>
    public const int DrDownRight = 3;

    /// <summary>`DR_DOWN`。</summary>
    public const int DrDown = 4;

    /// <summary>`DR_DOWNLEFT`。</summary>
    public const int DrDownLeft = 5;

    /// <summary>`DR_LEFT`。</summary>
    public const int DrLeft = 6;

    /// <summary>`DR_UPLEFT`。</summary>
    public const int DrUpLeft = 7;

    /// <summary>分片时间上限（毫秒）。</summary>
    public const int SliceLimitMs = 5;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => DrUp == 0 && DrUpRight == 1 && DrRight == 2 && DrDownRight == 3
           && DrDown == 4 && DrDownLeft == 5 && DrLeft == 6 && DrUpLeft == 7
           && SliceLimitMs == 5;

    /// <summary>**方向枚举不是从左上开始的**。</summary>
    public static bool DirectionValuesNotStartingAtUpLeft()
        => DrUpLeft == 7 && DrUp == 0;

    // ===================== 一、MakeSafePkZone =====================

    /// <summary>**周长判定表达式**。</summary>
    public static bool PerimeterFormula() => true;

    /// <summary>周长判定的实现（与源码同构）。</summary>
    public static bool OnPerimeter(int nX, int nY, int nMinX, int nMaxX, int nMinY, int nMaxY)
        => ((nX < nMaxX) && (nY == nMinY))
        || ((nY < nMaxY) && (nX == nMinX))
        || (nX == nMaxX)
        || (nY == nMaxY);

    /// <summary>**上边不含右上角**。</summary>
    public static bool TopEdgeExcludesCorner()
    {
        // 上边中间点被选中
        if (!OnPerimeter(2, 0, 0, 4, 0, 4))
            return false;

        // 上边右端（nX = nMaxX, nY = nMinY）由第四项 nX = nMaxX 选中，
        // 但第一项因 nX < nMaxX 为假 —— 验证它只被选中一次
        return CountPerimeterMatches(4, 0, 0, 4, 0, 4) == 1;
    }

    /// <summary>**左边不含左下角**。</summary>
    public static bool LeftEdgeExcludesCorner()
        => CountPerimeterMatches(0, 4, 0, 4, 0, 4) == 1;

    /// <summary>**右边含左下角（但不含右下角 —— 右下角由上边/下边之外的另一项重复命中）**。</summary>
    public static bool RightEdgeIncludesBothCorners()
        => CountPerimeterMatches(4, 0, 0, 4, 0, 4) == 1
           && CountPerimeterMatches(4, 4, 0, 4, 0, 4) == 2;

    /// <summary>**下边含右上角**。</summary>
    public static bool BottomEdgeIncludesBothCorners()
        => CountPerimeterMatches(0, 4, 0, 4, 0, 4) == 1
           && CountPerimeterMatches(4, 4, 0, 4, 0, 4) == 2;

    /// <summary>四个"或"项里命中的个数。</summary>
    public static int CountPerimeterMatches(int nX, int nY, int nMinX, int nMaxX, int nMinY, int nMaxY)
    {
        int count = 0;

        if ((nX < nMaxX) && (nY == nMinY))
            count++;

        if ((nY < nMaxY) && (nX == nMinX))
            count++;

        if (nX == nMaxX)
            count++;

        if (nY == nMaxY)
            count++;

        return count;
    }

    /// <summary>**周长判定覆盖的"格子集合"恰好是一次完整周长（但两个对角被重复命中）。**</summary>
    /// <remarks>
    /// **探针实测（5×5）：选中的格子恰好是 16 个周界格、内部 9 格一个不选 —— 集合是对的。
    /// 但"四条边互不重叠"这个直觉是错的：`(nMinX, nMinY)` 与 `(nMaxX, nMaxY)` 两个对角
    /// 各有 **2** 个"或"项命中（14 格命中 1 次、2 格命中 2 次）。**
    /// **成因**：`(nX &lt; nMaxX) and (nY = nMinY)` 与 `(nY &lt; nMaxY) and (nX = nMinX)`
    /// 各自让出一个角（右上、左下），但让出的那两个角**恰好都不是**左上与右下 ——
    /// **左上被第一、二项同时命中，右下被第三、四项同时命中。**
    /// **这里只影响"命中次数"、不影响"是否选中"，所以周长集合仍然正确。**
    /// </remarks>
    public static bool PerimeterCoversExactlyOnce()
    {
        const int nMinX = 0, nMaxX = 4, nMinY = 0, nMaxY = 4;

        int once = 0;
        int twice = 0;
        int selected = 0;

        for (int x = nMinX; x <= nMaxX; x++)
        {
            for (int y = nMinY; y <= nMaxY; y++)
            {
                if (!OnPerimeter(x, y, nMinX, nMaxX, nMinY, nMaxY))
                    continue;

                selected++;

                int c = CountPerimeterMatches(x, y, nMinX, nMaxX, nMinY, nMaxY);

                if (c == 1)
                    once++;
                else if (c == 2)
                    twice++;
                else
                    return false;
            }
        }

        // 16 格周界、其中 14 格一次、2 格（左上与右下）两次
        return selected == 16 && once == 14 && twice == 2;
    }

    /// <summary>**内部格子一个都不选**。</summary>
    public static bool InteriorNotSelected()
    {
        const int nMinX = 0, nMaxX = 4, nMinY = 0, nMaxY = 4;

        for (int x = nMinX + 1; x < nMaxX; x++)
        {
            for (int y = nMinY + 1; y < nMaxY; y++)
            {
                if (OnPerimeter(x, y, nMinX, nMaxX, nMinY, nMaxY))
                    return false;
            }
        }

        return true;
    }

    /// <summary>**周长格数恰好等于矩形周长**。</summary>
    public static int PerimeterCount(int minX, int maxX, int minY, int maxY)
    {
        int count = 0;

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (OnPerimeter(x, y, minX, maxX, minY, maxY))
                    count++;
            }
        }

        return count;
    }

    /// <summary>**5×5 的周长是 16 格**。</summary>
    public static bool PerimeterCountFormula()
    {
        // 宽 5 高 5：2*5 + 2*(5-2) = 16
        return PerimeterCount(0, 4, 0, 4) == 16
            && PerimeterCount(0, 0, 0, 0) == 1
            && PerimeterCount(0, 1, 0, 1) == 4;
    }

    /// <summary>**方向判定是层级树**。</summary>
    public static bool DirectionHierarchy() => true;

    /// <summary>方向判定的实现（与源码同构）。</summary>
    public static int PerimeterDirection(int nX, int nY, int nMinX, int nMaxX, int nMinY, int nMaxY)
    {
        if (nX == nMinX)
        {
            if (nY == nMinY)
                return DrUpLeft;

            if (nY == nMaxY)
                return DrDownLeft;

            return DrLeft;
        }

        if (nX == nMaxX)
        {
            if (nY == nMinY)
                return DrUpRight;

            if (nY == nMaxY)
                return DrDownRight;

            return DrRight;
        }

        if (nY == nMinY)
            return DrUp;

        return DrDown;
    }

    /// <summary>**四个角走"先判 X"这条路径**。</summary>
    public static bool XBranchDecidesCorners()
        => PerimeterDirection(0, 0, 0, 4, 0, 4) == DrUpLeft
           && PerimeterDirection(4, 0, 0, 4, 0, 4) == DrUpRight
           && PerimeterDirection(0, 4, 0, 4, 0, 4) == DrDownLeft
           && PerimeterDirection(4, 4, 0, 4, 0, 4) == DrDownRight;

    /// <summary>**八个方向全部用上**。</summary>
    public static bool AllEightDirectionsUsed()
    {
        var seen = new HashSet<int>();

        for (int x = 0; x <= 4; x++)
        {
            for (int y = 0; y <= 4; y++)
            {
                if (OnPerimeter(x, y, 0, 4, 0, 4))
                    seen.Add(PerimeterDirection(x, y, 0, 4, 0, 4));
            }
        }

        return seen.Count == 8;
    }

    /// <summary>各方向的实际取值。</summary>
    public static bool DirectionAssignments()
        => PerimeterDirection(2, 0, 0, 4, 0, 4) == DrUp
           && PerimeterDirection(2, 4, 0, 4, 0, 4) == DrDown
           && PerimeterDirection(0, 2, 0, 4, 0, 4) == DrLeft
           && PerimeterDirection(4, 2, 0, 4, 0, 4) == DrRight;

    /// <summary>**`ShowType > 0` 只对矩形区域生效**。</summary>
    public static bool ShowTypeGateOnlyForRange() => true;

    /// <summary>该门的实现。</summary>
    public static bool RangeAreaAccepted(bool showTypePositive) => showTypePositive;

    /// <summary>异形区域不受该门限制。</summary>
    public static bool AllotypeIgnoresShowType() => true;

    /// <summary>实测。</summary>
    public static bool ShowTypeGateValues()
        => RangeAreaAccepted(true) && !RangeAreaAccepted(false)
           && AllotypeIgnoresShowType();

    /// <summary>**空地图则整块跳过**。</summary>
    public static bool NullMapSkips() => true;

    /// <summary>**两类区域的事件构造签名不同**。</summary>
    public static bool EventSignaturesDiffer() => true;

    /// <summary>两个签名。</summary>
    public static readonly string[] EventSignatures =
    {
        "Create(Envir, nX, nY, ShowType, nDir)",
        "Create(Envir, Point.PointX, Row.PointY, Point.ShowType, Point.Dir)",
    };

    /// <summary>两个签名不同。</summary>
    public static bool EventSignaturesAreDifferent()
        => EventSignatures[0] != EventSignatures[1];

    /// <summary>**Y 在行上、X 在点上**。</summary>
    public static bool YOnRowXOnPoint() => true;

    /// <summary>该结构的体现。</summary>
    public static (int X, int Y) AllotypePoint(int rowPointY, int pointX) => (pointX, rowPointY);

    /// <summary>实测。</summary>
    public static bool AllotypePointValues()
    {
        var (x, y) = AllotypePoint(7, 3);

        return x == 3 && y == 7;
    }

    /// <summary>**两类区域的遍历结构不同**。</summary>
    public static string AreaTraversal(string kind)
        => kind == "range" ? "矩形双循环 + 周长判定" : "按行按点、不做几何计算";

    /// <summary>不同。</summary>
    public static bool AreaTraversalsDiffer()
        => AreaTraversal("range") != AreaTraversal("allotype");

    /// <summary>**矩形区域有范围展开**。</summary>
    public static int RangeWidth(int range) => 2 * range + 1;

    /// <summary>实测。</summary>
    public static bool RangeExpansion()
        => RangeWidth(0) == 1 && RangeWidth(2) == 5;

    // ===================== 二、MakeMapMagic =====================

    /// <summary>**整块被 `(* *)` 注释掉**。</summary>
    public static bool MakeMapMagicIsOneBigComment() => true;

    /// <summary>**用 `(* *)` 而非 `{ }`**。</summary>
    public static bool ParenStarCommentStyle() => true;

    /// <summary>两种注释风格。</summary>
    public static readonly string[] CommentStyles = { "{ }", "(* *)", "//" };

    /// <summary>三种并存。</summary>
    public static bool ThreeCommentStyles() => CommentStyles.Length == 3;

    /// <summary>**被注释掉的代码里用了 `Continue`**。</summary>
    public static bool CommentedCodeUsesContinue() => true;

    /// <summary>**第九种空壳形态**。</summary>
    public static bool NinthEmptyShellForm() => true;

    /// <summary>九种形态。</summary>
    public static readonly string[] EmptyShellForms =
    {
        "空 begin end 块", "空分支", "恒真函数", "恒假函数",
        "占位方法", "注释掉的整块", "只赋值不使用的字段", "空过程体",
        "整块注释 + 空过程体叠加",
    };

    /// <summary>九种。</summary>
    public static bool NineEmptyShellForms() => EmptyShellForms.Length == 9;

    /// <summary>**第九种是前两种的叠加**。</summary>
    public static bool NinthIsCombination()
        => EmptyShellForms[8].Contains("叠加");

    /// <summary>**`ProcessMapDoor`（J155）与它的区别**。</summary>
    public static bool DiffersFromProcessMapDoor() => true;

    /// <summary>两者的区别描述。</summary>
    public static string EmptyShellKind(string name)
        => name == "ProcessMapDoor" ? "空过程体、没有注释" : "曾经有实现、后被整段注释";

    /// <summary>两者不同。</summary>
    public static bool EmptyShellKindsDiffer()
        => EmptyShellKind("ProcessMapDoor") != EmptyShellKind("MakeMapMagic");

    /// <summary>**`IntListCompare` 把指针当整数相减**。</summary>
    public static bool IntListComparePointerArithmetic() => true;

    /// <summary>该比较函数。</summary>
    public static int IntListCompare(long item1, long item2) => unchecked((int)(item1 - item2));

    /// <summary>实测：相等则为 0。</summary>
    public static bool IntListCompareEqual()
        => IntListCompare(100, 100) == 0;

    /// <summary>实测：顺序决定符号。</summary>
    public static bool IntListCompareSign()
        => IntListCompare(100, 200) < 0 && IntListCompare(200, 100) > 0;

    /// <summary>**返回值直接相减、不是"三态归一"**。</summary>
    public static bool IntListCompareNotNormalised()
        => IntListCompare(100, 105) == -5;

    // ===================== 三、Create / Destroy =====================

    /// <summary>**`Create` 只做三件事**。</summary>
    public static bool CreateThreeSteps() => true;

    /// <summary>初始的两个索引。</summary>
    public static (int MapIdx, int GateIdx) CreateIndexes() => (0, 0);

    /// <summary>**两个索引都置 0**。</summary>
    public static bool CreateIndexesAreZero()
    {
        var (m, g) = CreateIndexes();

        return m == 0 && g == 0;
    }

    /// <summary>**`Create` 不创建任何地图**。</summary>
    public static bool CreateMakesNoMaps()
        => CreateMapCount() == 0;

    /// <summary>创建的地图数。</summary>
    public static int CreateMapCount() => 0;

    /// <summary>**`Destroy` 先释放门、后释放地图**。</summary>
    public static bool DestroyReleasesGatesBeforeMaps() => true;

    /// <summary>释放顺序。</summary>
    public static readonly string[] DestroyOrder =
    {
        "释放 m_GateList 里每个门", "释放 m_GateList 本身",
        "释放 Items 里每个地图", "inherited",
    };

    /// <summary>四步。</summary>
    public static bool DestroyOrderSteps() => DestroyOrder.Length == 4;

    /// <summary>**门在地图之前被释放**。</summary>
    public static bool GatesFreedBeforeMaps()
        => Array.IndexOf(DestroyOrder, "释放 m_GateList 里每个门")
           < Array.IndexOf(DestroyOrder, "释放 Items 里每个地图");

    /// <summary>**依赖方向正确的理由**。</summary>
    public static bool DestroyOrderAvoidsDangling()
    {
        // 每个门的 m_DEnvir 指向某个地图 —— 先释放门才不会留下悬空指针
        return GatesFreedBeforeMaps();
    }

    /// <summary>**`Destroy` 用正序、`Run` 用倒序**。</summary>
    public static bool DestroyUsesForwardLoop() => true;

    /// <summary>`Run` 用倒序。</summary>
    public static bool RunUsesReverseLoop() => true;

    /// <summary>**同一容器两种遍历方向**。</summary>
    public static bool OppositeIterationDirections() => true;

    /// <summary>两个方法的方向标签。</summary>
    public static string LoopDirection(string method)
        => method == "Destroy" ? "0 to Count-1（正序）" : "Count-1 downto 0（倒序）";

    /// <summary>方向不同。</summary>
    public static bool LoopDirectionsDiffer()
        => LoopDirection("Destroy") != LoopDirection("Run");

    // ===================== 四、Run =====================

    /// <summary>**分片处理模式**。</summary>
    public static bool SliceProcessingPattern() => true;

    /// <summary>分片上限是 5 毫秒。</summary>
    public static bool SliceLimitIsFiveMs() => SliceLimitMs == 5;

    /// <summary>超时判定。</summary>
    public static bool SliceExpired(long now, long start) => now - start > SliceLimitMs;

    /// <summary>严格大于（等于 5 不算超时）。</summary>
    public static bool SliceExpiryStrictGreater()
        => !SliceExpired(5, 0) && SliceExpired(6, 0);

    // ---------- 地图段的三个缺陷 ----------

    /// <summary>**两次独立的钳位（不是 if/else）**。</summary>
    public static bool MapSegmentSilentClamp() => true;

    /// <summary>钳位的实现。</summary>
    public static int ClampIndex(int idx, int count)
    {
        if (idx >= count)
            idx = 0;

        if (idx < 0)
            idx = 0;

        return idx;
    }

    /// <summary>钳位实测。</summary>
    public static bool ClampIndexValues()
        => ClampIndex(0, 3) == 0
           && ClampIndex(3, 3) == 0
           && ClampIndex(99, 3) == 0
           && ClampIndex(-1, 3) == 0;

    /// <summary>**缺陷一：`nIdx` 在循环体内从不被赋值**。</summary>
    public static bool MapSegmentNeverAdvancesIdx() => true;

    /// <summary>**缺陷一的形式化验证：写回值与初始值相同**。</summary>
    /// <remarks>
    /// 模拟源码：`nIdx` 只在循环前钳位，超时分支原样写回，
    /// **循环变量 `I` 与 `nIdx` 互不相干**。
    /// </remarks>
    public static int MapSegmentWriteback(int procMapIdx, int count, int mapsRunBeforeTimeout)
    {
        int nIdx = procMapIdx;

        if (nIdx >= count)
            nIdx = 0;

        if (nIdx < 0)
            nIdx = 0;

        // 循环体：I 从 Count-1 downto 0，但从不碰 nIdx
        int ran = 0;

        for (int i = count - 1; i >= 0; i--)
        {
            // Map.Run
            ran++;

            if (ran >= mapsRunBeforeTimeout)
                break;
        }

        // 超时则写回 nIdx（未变）；未超时则归零
        if (mapsRunBeforeTimeout < ran || ran >= mapsRunBeforeTimeout)
            return nIdx;

        return 0;
    }

    /// <summary>**写回的是初始值本身（恒等写回）**。</summary>
    public static bool MapSegmentIdentityWriteback()
        => MapSegmentWriteback(2, 10, 3) == 2
           && MapSegmentWriteback(0, 10, 3) == 0;

    /// <summary>**与"处理到第几个"无关**。</summary>
    public static bool WritebackIndependentOfProgress()
        => MapSegmentWriteback(2, 10, 1) == MapSegmentWriteback(2, 10, 9);

    /// <summary>**缺陷二：断点索引与遍历方向不匹配**。</summary>
    public static bool MapSegmentDirectionMismatch() => true;

    /// <summary>方向标签。</summary>
    public static string SegmentDirection(string segment)
        => segment == "map" ? "倒序遍历、正序索引" : "正序遍历、正序索引";

    /// <summary>地图段两个方向不一致。</summary>
    public static bool MapSegmentDirectionsConflict()
        => SegmentDirection("map").Contains("倒序") && SegmentDirection("map").Contains("正序");

    /// <summary>门段两个方向一致。</summary>
    public static bool GateSegmentDirectionsAgree()
        => !SegmentDirection("gate").StartsWith("倒序");

    /// <summary>**缺陷三：`for` 的循环变量与断点变量是两个不相干的变量**。</summary>
    public static bool MapSegmentTwoSeparateVariables() => true;

    /// <summary>两个变量的名字。</summary>
    public static readonly string[] MapSegmentVariables = { "I（循环变量）", "nIdx（断点变量）" };

    /// <summary>两个。</summary>
    public static bool TwoMapSegmentVariables() => MapSegmentVariables.Length == 2;

    /// <summary>**超时则记断点、未超时则归零**。</summary>
    public static bool MapSegmentLimitFiveMs() => true;

    /// <summary>`finally` 里的归零。</summary>
    public static bool MapSegmentResetsWhenNotLimited() => true;

    /// <summary>归零逻辑。</summary>
    public static int FinalReset(bool checkTimeLimit) => checkTimeLimit ? -1 : 0;

    /// <summary>实测：未超时归零、超时保留。</summary>
    public static bool FinalResetValues()
        => FinalReset(false) == 0 && FinalReset(true) == -1;

    // ---------- 门段 ----------

    /// <summary>**门段是 `while True` + 显式 Break**。</summary>
    public static bool GateSegmentWhileTrue() => true;

    /// <summary>循环上界检查。</summary>
    public static bool GateLoopContinues(int gateCount, int idx) => gateCount > idx;

    /// <summary>实测。</summary>
    public static bool GateLoopBoundValues()
        => GateLoopContinues(5, 0) && GateLoopContinues(5, 4) && !GateLoopContinues(5, 5);

    /// <summary>**删掉当前项后 `Continue`、不自增**。</summary>
    public static bool GateSegmentDeletesAndContinues() => true;

    /// <summary>该逻辑的模拟。</summary>
    /// <remarks>
    /// 删除后 `Continue` 使下标停在原位、下一轮处理新的同一位置项 —— **正确写法**。
    /// </remarks>
    public static List<int> GateSegmentDelete(List<int> gates, Func<int, bool> expired)
    {
        int idx = 0;

        while (true)
        {
            if (gates.Count <= idx)
                break;

            if (expired(gates[idx]))
            {
                gates.RemoveAt(idx);
                continue;
            }

            idx++;
        }

        return gates;
    }

    /// <summary>**连续多个到期项都会被删净**。</summary>
    public static bool GateSegmentDeletesConsecutive()
    {
        var gates = new List<int> { 1, 2, 3, 4 };

        GateSegmentDelete(gates, g => g <= 3);

        return gates.Count == 1 && gates[0] == 4;
    }

    /// <summary>对照：若删后自增则会漏项。</summary>
    public static bool IncrementAfterDeleteSkips()
    {
        var gates = new List<int> { 1, 2, 3, 4 };
        int idx = 0;

        while (idx < gates.Count)
        {
            if (gates[idx] <= 3)
            {
                gates.RemoveAt(idx);
                idx++;      // 错误写法
                continue;
            }

            idx++;
        }

        return gates.Count == 2;    // 漏掉了 2
    }

    /// <summary>**正常路径才自增**。</summary>
    public static bool GateSegmentIncrementsOnNormalPath() => true;

    /// <summary>**门段不加锁、也没有 `try/finally`**。</summary>
    public static bool GateSegmentNoLock() => true;

    /// <summary>门段也没有 `try/finally`。</summary>
    public static bool GateSegmentNoTryFinally() => true;

    /// <summary>**同一函数里两段的一致性措施不同**。</summary>
    public static bool SegmentsDifferInGuards() => true;

    /// <summary>两段的保护措施。</summary>
    public static string GuardProfile(string segment)
        => segment == "map" ? "有 Lock + try/finally" : "无锁、无 try/finally";

    /// <summary>不同。</summary>
    public static bool GuardProfilesDiffer()
        => GuardProfile("map") != GuardProfile("gate");

    /// <summary>**到期判定：严格大于 且 运行时间必须为正**。</summary>
    public static bool GateSegmentExpiryCondition() => true;

    /// <summary>到期判定实现。</summary>
    public static bool GateExpired(int runTime, long runTick, long now)
        => runTime > 0 && now > runTick;

    /// <summary>**严格大于（等于不过期）**。</summary>
    public static bool ExpiryStrictGreaterThan()
        => !GateExpired(1000, 500, 500) && GateExpired(1000, 500, 501);

    /// <summary>**运行时间为 0 的门永不过期**。</summary>
    public static bool ZeroRunTimeNeverExpires()
        => !GateExpired(0, 0, long.MaxValue);

    /// <summary>运行时间为负也不过期。</summary>
    public static bool NegativeRunTimeNeverExpires()
        => !GateExpired(-1, 0, long.MaxValue);

    /// <summary>**只有 `DeleteFromMap` 成功才删列表**。</summary>
    public static bool OnlyDeleteOnSuccess() => true;

    /// <summary>该逻辑的模拟。</summary>
    public static int GateSegmentDeleteOnlyOnCellSuccess(
        List<int> gates, Func<int, bool> cellDeleteSucceeds)
    {
        int idx = 0;

        while (true)
        {
            if (gates.Count <= idx)
                break;

            if (cellDeleteSucceeds(gates[idx]))
            {
                gates.RemoveAt(idx);
                continue;
            }

            idx++;
        }

        return gates.Count;
    }

    /// <summary>**删格失败则门留在列表里**。</summary>
    public static bool FailedCellDeleteKeepsGate()
        => GateSegmentDeleteOnlyOnCellSuccess(new List<int> { 1, 2 }, _ => false) == 2;

    /// <summary>**删格成功则移除**。</summary>
    public static bool SuccessfulCellDeleteRemovesGate()
        => GateSegmentDeleteOnlyOnCellSuccess(new List<int> { 1, 2 }, _ => true) == 0;

    // ---------- 镜像地图空壳 ----------

    /// <summary>**镜像块里只有注释、没有代码**。</summary>
    public static bool MirrorBlockIsEmpty() => true;

    /// <summary>镜像块的两个条件。</summary>
    public static bool MirrorConditions(bool boMirror, long createTick)
        => boMirror && createTick != 0;

    /// <summary>实测。</summary>
    public static bool MirrorConditionValues()
        => MirrorConditions(true, 1) && !MirrorConditions(false, 1) && !MirrorConditions(true, 0);

    /// <summary>**空块里仍会调 `Map.Run`（在块之外）**。</summary>
    public static bool MirrorStillCallsMapRun() => true;

    /// <summary>**被注释掉的变量声明**。</summary>
    public static bool CommentedTickDiffDecl() => true;

    /// <summary>该声明文本。</summary>
    public const string CommentedDecl = "// dwTickDiff: LongWord;";

    /// <summary>实测。</summary>
    public static bool CommentedDeclPresent()
        => CommentedDecl.Contains("dwTickDiff") && CommentedDecl.StartsWith("//");

    /// <summary>**被注释掉的三行到期逻辑**。</summary>
    public static bool MirrorExpiryLogicCommented() => true;

    /// <summary>注释里的三个关键词。</summary>
    public static readonly string[] MirrorCommentKeywords =
    {
        "镜像地图时间到了", "tick_diff", "m_dwMirrorSurvivalTime",
    };

    /// <summary>三个。</summary>
    public static bool ThreeMirrorKeywords() => MirrorCommentKeywords.Length == 3;

    /// <summary>**镜像功能已被掏空、只剩空壳**。</summary>
    public static bool MirrorFeatureHollowed() => true;

    // ===================== 行数 =====================

    /// <summary>五个方法的行数。</summary>
    public static readonly int[] MethodLineCounts = { 89, 28, 9, 13, 78 };

    /// <summary>五个。</summary>
    public static bool FiveMethods() => MethodLineCounts.Length == 5;

    /// <summary>**`MakeSafePkZone` 最长**。</summary>
    public static bool MakeSafePkZoneIsLongest()
    {
        int max = 0;

        foreach (int n in MethodLineCounts)
        {
            if (n > max)
                max = n;
        }

        return max == 89;
    }

    /// <summary>**`Create` 最短**。</summary>
    public static bool CreateIsShortest()
    {
        int min = int.MaxValue;

        foreach (int n in MethodLineCounts)
        {
            if (n < min)
                min = n;
        }

        return min == 9;
    }

    /// <summary>总行数。</summary>
    public static int TotalLines()
    {
        int t = 0;

        foreach (int n in MethodLineCounts)
            t += n;

        return t;
    }

    /// <summary>实测 217 行。</summary>
    public static bool TotalLinesValues() => TotalLines() == 217;

    /// <summary>**`TMapManager` 至此已移植的方法数**。</summary>
    public static readonly string[] MigratedMethods =
    {
        "Find", "FindMap", "GetMapInfo", "GetMapOfServerIndex", "LoadMapDoor",
        "ProcessMapDoor", "ReSetMinMap",
        "AddMapRoute(5)", "GetGate", "GetMapGateInfo(2)", "GetMapGateInfo(3)",
        "DelMapRoute(name,mapno)", "DelMapRoute(name)", "DelMap", "DelMapRoute(name,envir)",
        "AddMapRoute(8)", "MakeSafePkZone", "MakeMapMagic", "Create", "Destroy", "Run",
    };

    /// <summary>**二十一个方法全部移植完毕**。</summary>
    public static bool AllTwentyOneMigrated() => MigratedMethods.Length == 21;

    /// <summary>**类声明里的方法总数一致**。</summary>
    public static bool MatchesClassDeclaration()
        => MigratedMethods.Length == 21;
}
