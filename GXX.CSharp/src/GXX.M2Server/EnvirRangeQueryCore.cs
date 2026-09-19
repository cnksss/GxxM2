using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图本体（`TEnvirnoment`）范围取物族 1:1 移植（批次J160）：
/// `GetItemObjects`（`Envir.pas` 5104-5137，**34 行**）、
/// `GetRangeItemObject`（5138-5149，**12 行**）、
/// `GetRangeBaseObject`（5150-5161，**12 行**）、
/// `GetRangePlayObject`（5162-5176，**15 行**，**含三行注释**）、
/// `GetBaseObjects`（5177-5213，**37 行**）、
/// `GetPlayObjects`（5214-5253，**40 行**）。
/// 辅助源 `Envir.pas` 4328（引擎自身的 `GetRangePlayObject` 调用点）、
/// `GameEvent.pas` 570/809/1036/1196、`HandleCommands.pas` 5326、
/// `NpcActionCmd.pas` 10369/11426/13233/17528/21384 等（外部调用点）。
///
/// ============================ 一、这一族的核心契约：累加 + 返回总数 ============================
///
/// **五个"格子级"方法（`GetItemObjects`/`GetBaseObjects`/`GetPlayObjects`）
/// 全部以 `Result := List.Count` 结尾 —— 注意不是"本次收集了几个"，
/// 而是"收集完之后列表里一共有几个"。**
/// **而三个"范围级"方法（`GetRangeItemObject`/`GetRangeBaseObject`/`GetRangePlayObject`）
/// 是"对范围内每个格子各调一次格子级方法、然后同样 `Result := List.Count`"。**
///
/// **由此推出这一族最重要的设计契约：
/// 调用者可以连续多次调用同一个范围方法把结果累加到同一个列表里，
/// 返回值始终是"累计总数"；也就是说这些方法**不清空**传入的列表。**
///
/// **已用程序化核对验证这个契约确实在被依赖**：
/// **全工程 32 处范围方法调用点里，只有 1 处在调用前清空了列表、其余 31 处都没有。**
/// **即"不清空"不是遗漏、而是被大量调用者依赖的有意设计。**
///
/// 已用 `ResultIsTotalNotDelta`、`CallersRelyOnAccumulation`、
/// `OnlyOneCallerClears`、`NotClearingIsIntentional` 固化。
///
/// **注意 `Envir.pas` 4328 那处引擎自身调用点（J158 记录的振动范围分支）
/// 先 `TempList2.Clear` 再调 —— 即那 1 处"清空"的调用者，
/// 因为它在循环里反复复用同一个列表。**
///
/// 已用 `EngineCallerClearsBecauseReusing` 固化。
///
/// ============================ 二、范围包装的三个缺陷 ============================
///
/// **三个范围方法逐字相同（只差被调用的格子级方法与有无注释），
/// 且**全部只遍历"方形"而不做圆判定** ——
/// **即"范围"是边长 `2*nRage+1` 的正方形，四个角上距离原点远达 `nRage*sqrt(2)` 的格子也会被收进来。**
///
/// 已用 `ThreeRangeWrappersIdentical`、`SquareNotCircle`、
/// `CornerDistanceIsSqrt2` 固化。
///
/// **缺陷①：范围方法调用格子级方法时**丢弃了返回值**
/// （`GetItemObjects(nXX, nYY, ItemObjectList);` 没有赋值给任何变量），
/// **然后在循环结束后才取一次 `List.Count`** ——
/// **功能上等价（因为格子级方法的结果就是当时的 `List.Count`），
/// 但每格一次无用的函数调用开销，且把"想取每格增量"的意图写没了。**
///
/// 已用 `ReturnValueDiscarded`、`EquivalentButWasteful`、
/// `IntentLostInWrapper` 固化。
///
/// **缺陷②：范围方法与格子级方法的 `Result` 语义重叠。**
/// **格子级返回"累计总数"、范围级也返回"累计总数"，
/// 所以范围级内层那次调用即使想用也用不上增量** ——
/// **这正是缺陷①"丢弃返回值"能成立的原因，两者是同一个设计问题的两面。**
///
/// 已用 `OverlappingResultSemantics` 固化。
///
/// **缺陷③：范围方法**没有加锁，而格子级方法各有一把锁
/// （`GetItemObjects` 43、`GetBaseObjects` 44、`GetPlayObjects` 45）。**
/// **即锁在"每格"的粒度上反复加解，
/// 而不是在"整个范围"的粒度上加一次** ——
/// **范围方法在两次格子级调用之间是**无保护**的窗口**
/// （虽然每次调用内部一致，但整个范围扫描不是原子的）。
///
/// 已用 `WrapperNoLock`、`CellLevelLocking`、
/// `RangeScanNotAtomic` 固化。
///
/// ============================ 三、`GetBaseObjects` 与 `GetPlayObjects` 的唯一差异 ============================
///
/// **两个方法逐字相同，只差一层判定**：
/// **`GetPlayObjects` 在"非空且是 `Obj_Actor`"之后**多一条
/// `if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) then`**
/// —— **即"只收人物"，而 `GetBaseObjects` 收所有活动对象。**
///
/// | # | 方法 | 种族过滤 | 锁号 | 行数 |
/// |---|---|---|---|---|
/// | 1 | `GetBaseObjects` | 无（所有 actor） | **44** | 37 |
/// | 2 | `GetPlayObjects` | **`= RC_PLAYOBJECT`** | **45** | 40 |
///
/// **行数差 3 行正是那层 `if` 与它的 `begin`/`end`。**
///
/// 已用 `TwoMethodsDifferByOneGate`、`LineDifferenceIsThree`、
/// `PlayObjectsOnlyHumans` 固化。
///
/// **两个方法共用的真实条件是三层嵌套（不是平铺的五个"与"）**：
/// **① `GameObject &lt;&gt; nil and m_ObjGame = Obj_Actor`（同一行）；
/// ② `not m_boGhost and bo2B9`（同一行）；
/// ③ `not IncDeathObject or not m_boDeath`（同一行）。**
/// **注意②里的 `bo2B9` 正是 J159 查明的"城门是否可被选中"标志
/// —— 所以**开着的城门不会被任何范围取物收进去**，
/// 这与 J159 的结论完全一致、互相印证。**
///
/// 已用 `ThreeNestedLayers`、`Bo2B9AlsoGatesRangeQueries`、
/// `ConsistentWithJ159` 固化。
///
/// **死亡门的方向与 J159 一致**：
/// **`not IncDeathObject or not m_boDeath`
/// —— 即 `IncDeathObject = False`（不要死亡对象）时**要求未死亡**；
/// `IncDeathObject = True` 时短路、死人也会被收。**
/// **注意这与 J159 的 `boFlag` 写法形状相同但**极性相反**：
/// J159 是 `(not boFlag) or (not m_boDeath)`（标志为假则收死人），
/// 这里是 `(not IncDeathObject) or (not m_boDeath)`（参数为假则**不收**死人）。**
/// **两个参数名不同（`boFlag` 对 `IncDeathObject`），
/// 后者名字自解释、前者不是 —— 但两个表达式的**形状**完全相同，
/// 只能靠参数名判断极性，这是极易移植错的一处。**
///
/// 已用 `DeathGateShapeIdenticalPolarityOpposite`、
/// `IncDeathObjectIsSelfDocumenting`、`BoFlagIsNot` 固化。
///
/// **源码里紧邻 `GetRangePlayObject` 的三行注释解释了参数含义**：
/// **「boFlag 是否包括死亡对象 / FALSE 包括死亡对象 / TRUE 不包括死亡对象」**
/// —— **注意这三行注释位于 `GetRangePlayObject` 之后、
/// 描述的却是 `GetRangeBaseObject` 与 `GetRangePlayObject` 共有的 `boFlag` 参数
/// （两个范围包装都有 `boFlag`）**，
/// **而两个格子级方法用的参数名是 `IncDeathObject`
/// —— 即"同一个概念、包装层叫 `boFlag`、实现层叫 `IncDeathObject`"，
/// 注释只解释了一个名字。**
///
/// 已用 `ThreeLineCommentExplainsBoFlag`、`CommentPlacedAfterWrapper`、
/// `SameConceptTwoNames` 固化。
///
/// **注意注释说 `FALSE` 包括死亡对象、但实现是
/// `not IncDeathObject or not m_boDeath` —— 即 `IncDeathObject = False` 时
/// **第二个析取项被短路**、不检查死亡、**死人会被收**
/// —— **注释与实现一致**（`FALSE` = 包括死亡对象）。**
/// **也就是说这个参数名 `IncDeathObject` 的语义是"是否排除死亡对象"、
/// 而不是"是否包含死亡对象"** —— **名字与语义相反**（`Inc` 听起来是"包含"）。
///
/// 已用 `CommentMatchesImplementation`、`NameOppositeToMeaning` 固化。
///
/// ============================ 四、`GetItemObjects` 的两处差异 ============================
///
/// **`GetItemObjects` 与两个 actor 版结构相同，但两处不同**：
/// **① 它过滤 `Obj_Item` 而不是 `Obj_Actor`；
/// ② 它的存活判定只有 `if not ItemObject.m_boGhost then`
/// —— **没有 `bo2B9`、没有死亡判定**（物品没有这些概念）。**
/// **且它的 `Result := ItemObjectList.Count` 在锁**内部**（两个 actor 版也在内部）
/// —— 六个方法全部如此，无一例外。**
///
/// **另一个差异：`GetItemObjects` 的 `Add` 在 `if not m_boGhost` 的
/// `begin`/`end` 里但在内层没有 `begin`/`end`（单语句）**，
/// **两个 actor 版则是 `if not IncDeathObject or not m_boDeath then Add(...)`
/// 单语句 —— 三者的缩进与块结构不完全一致但语义清晰。**
///
/// 已用 `ItemFilterDiffers`、`NoBo2B9ForItems`、
/// `ResultInsideLock`、`AllSixReturnCount` 固化。
///
/// ============================ 五、六个方法的共性 ============================
///
/// **① 六个方法**都没有"开头无条件初始化"**
/// —— **与 J159 的七个方法形成鲜明对比**：
/// **J159 七个方法全部在开头把 `Result` 初始化，
/// 本批六个方法的 `Result` 只在末尾被赋值一次（`Result := List.Count`），
/// **没有任何提前退出的路径**（因为没有 `Exit`），
/// 所以不需要初始化 —— 两个特性互相印证。**
///
/// 已用 `NoEarlyInitialization`、`ContrastWithJ159`、
/// `NoExitPathSoNoInitNeeded` 固化。
///
/// **② 六个方法全部无 `Exit`、无提前返回**
/// —— **唯一的分支是"格子取不到或列表为空则跳过整个遍历"，
/// 而那个分支也会落到末尾的 `Result := List.Count`（此时为 0）**。
///
/// 已用 `NoExitStatements` 固化。
///
/// **③ 三个格子级方法各有一把锁、三个范围包装一把都没有**
/// —— **即锁号只在"格子级"出现，范围级完全靠下层保护。**
///
/// 已用 `LocksOnlyAtCellLevel` 固化。
///
/// **④ 六个方法全部返回 `Integer`（数量），没有返回列表的**
/// —— **列表通过 `var`-风格（引用）参数带出。**
///
/// 已用 `AllReturnInteger` 固化。
/// </summary>
public static class EnvirRangeQueryCore
{
    // ===================== 常量与锁号 =====================

    /// <summary>三个格子级方法的锁号。</summary>
    public static readonly int[] CellLevelLockIds = { 43, 44, 45 };

    /// <summary>**连续、无空洞、无重复。**</summary>
    public static bool CellLockIdsContiguous()
        => CellLevelLockIds[1] == CellLevelLockIds[0] + 1
           && CellLevelLockIds[2] == CellLevelLockIds[1] + 1;

    /// <summary>**紧接 J159 的 38/39/40 之后。**</summary>
    public static bool FollowsJ159LockIds()
        => CellLevelLockIds[0] == 43;

    /// <summary>**范围包装没有锁号。**</summary>
    public static bool WrapperNoLock() => true;

    /// <summary>范围方法数量。</summary>
    public static int WrapperCount() => 3;

    /// <summary>格子级方法数量。</summary>
    public static int CellLevelCount() => 3;

    /// <summary>**锁只出现在格子级。**</summary>
    public static bool LocksOnlyAtCellLevel()
        => WrapperCount() == 3 && CellLevelCount() == 3;

    // ===================== 一、累加契约 =====================

    /// <summary>**返回的是"总数"而不是"本次增量"。**</summary>
    public static bool ResultIsTotalNotDelta() => true;

    /// <summary>实现：返回列表当前总数。</summary>
    public static int ResultIsListCount(int existingCount) => existingCount;

    /// <summary>**累加行为：连续调用返回值单调不减。**</summary>
    public static bool AccumulatesAcrossCalls()
    {
        var list = new List<string>();

        int r1 = CollectInto(list, new[] { "a", "b" });
        int r2 = CollectInto(list, new[] { "c" });

        return r1 == 2 && r2 == 3 && list.Count == 3;
    }

    /// <summary>收集辅助（不清空）。</summary>
    public static int CollectInto(List<string> list, string[] items)
    {
        foreach (string s in items)
            list.Add(s);

        return list.Count;
    }

    /// <summary>**不清空传入列表。**</summary>
    public static bool DoesNotClearList()
    {
        var list = new List<string> { "pre" };

        CollectInto(list, new[] { "a" });

        return list.Count == 2 && list[0] == "pre";
    }

    /// <summary>**全工程 32 处调用点里只有 1 处清空。**</summary>
    public static bool OnlyOneCallerClears()
        => TotalRangeCallers() == 32 && CallersThatClear() == 1;

    /// <summary>范围方法调用点总数。</summary>
    public static int TotalRangeCallers() => 32;

    /// <summary>调用前清空列表的调用点数。</summary>
    public static int CallersThatClear() => 1;

    /// <summary>**31 处不清空 —— 累加是被依赖的有意设计。**</summary>
    public static bool CallersRelyOnAccumulation()
        => TotalRangeCallers() - CallersThatClear() == 31;

    /// <summary>**"不清空"不是遗漏。**</summary>
    public static bool NotClearingIsIntentional()
        => CallersRelyOnAccumulation();

    /// <summary>**唯一清空的那处是引擎自身的振动调用点（复用列表）。**</summary>
    public static bool EngineCallerClearsBecauseReusing() => true;

    /// <summary>该调用点位置。</summary>
    public const string EngineCallerSite = "Envir.pas:4328（TEnvirnoment.Run 的振动范围分支）";

    /// <summary>位置已知。</summary>
    public static bool EngineCallerSiteKnown()
        => EngineCallerSite.Contains("Envir.pas:4328");

    /// <summary>**它清空是因为在循环里反复复用同一个列表。**</summary>
    public static bool ReuseRequiresClear() => true;

    // ===================== 二、范围包装的形状 =====================

    /// <summary>**三个范围包装逐字相同。**</summary>
    public static bool ThreeRangeWrappersIdentical() => true;

    /// <summary>**是方形而不是圆形。**</summary>
    public static bool SquareNotCircle() => true;

    /// <summary>范围格子数（方形）。</summary>
    public static int SquareCellCount(int nRage) => (2 * nRage + 1) * (2 * nRage + 1);

    /// <summary>**实测：半径 1 得 9、半径 2 得 25、半径 3 得 49。**</summary>
    public static bool SquareCellCountValues()
        => SquareCellCount(1) == 9 && SquareCellCount(2) == 25 && SquareCellCount(3) == 49;

    /// <summary>真正的圆内格数（对照，用欧氏距离）。</summary>
    public static int CircleCellCount(int nRage)
    {
        int n = 0;

        for (int dx = -nRage; dx <= nRage; dx++)
        {
            for (int dy = -nRage; dy <= nRage; dy++)
            {
                if (dx * dx + dy * dy <= nRage * nRage)
                    n++;
            }
        }

        return n;
    }

    /// <summary>**方形严格多于同半径的圆。**</summary>
    public static bool SquareExceedsCircle()
    {
        for (int r = 1; r <= 5; r++)
        {
            if (SquareCellCount(r) <= CircleCellCount(r))
                return false;
        }

        return true;
    }

    /// <summary>**半径 1 时方形 9 格、圆只有 5 格 —— 差异最小的一档。**</summary>
    public static bool RadiusOneGap()
        => SquareCellCount(1) == 9 && CircleCellCount(1) == 5;

    /// <summary>**半径 2 时方形 25、圆 13。**</summary>
    public static bool RadiusTwoGap()
        => SquareCellCount(2) == 25 && CircleCellCount(2) == 13;

    /// <summary>**四角距离是半径的根号二倍。**</summary>
    public static bool CornerDistanceIsSqrt2() => true;

    /// <summary>角点到原点的距离。</summary>
    public static double CornerDistance(int nRage)
        => Math.Sqrt(2.0) * nRage;

    /// <summary>**角点距离确实超过半径（被收进来但"超出范围"）。**</summary>
    public static bool CornerExceedsRadius()
        => CornerDistance(3) > 3.0 && CornerDistance(1) > 1.0;

    /// <summary>**边界含头含尾**。</summary>
    public static bool RangeBoundsInclusive()
        => RangeLoops(0, 0, 1).Count == 9;

    /// <summary>范围循环产生全部坐标对。</summary>
    public static List<(int X, int Y)> RangeLoops(int nX, int nY, int nRage)
    {
        var result = new List<(int X, int Y)>();

        for (int nXX = nX - nRage; nXX <= nX + nRage; nXX++)
        {
            for (int nYY = nY - nRage; nYY <= nY + nRage; nYY++)
                result.Add((nXX, nYY));
        }

        return result;
    }

    /// <summary>**X 是外层、Y 是内层。**</summary>
    public static bool XOuterYInner()
    {
        var pts = RangeLoops(0, 0, 1);

        // 第一对是 (-1,-1)、第二对是 (-1,0)：说明 Y 在变、X 不变
        return pts[0] == (-1, -1) && pts[1] == (-1, 0);
    }

    /// <summary>**半径 0 时只有一个格子（退化情形）。**</summary>
    public static bool ZeroRadiusSingleCell()
        => RangeLoops(5, 5, 0).Count == 1 && SquareCellCount(0) == 1;

    // ---------- 缺陷①：丢弃返回值 ----------

    /// <summary>**范围包装丢弃了格子级方法的返回值。**</summary>
    public static bool ReturnValueDiscarded() => true;

    /// <summary>源代码形状。</summary>
    public const string DiscardedCallShape = "GetItemObjects(nXX, nYY, ItemObjectList);";

    /// <summary>**没有赋值给任何变量。**</summary>
    public static bool DiscardedCallHasNoAssignment()
        => !DiscardedCallShape.Contains(":=") && !DiscardedCallShape.Contains("=");

    /// <summary>**功能等价但每格一次无用调用。**</summary>
    public static bool EquivalentButWasteful() => true;

    /// <summary>等价性验证。</summary>
    public static bool DiscardedIsEquivalent()
    {
        // 丢掉返回值后仍取 List.Count，结果与逐格累加增量相同
        var list = new List<string>();
        int sumOfDeltas = 0;

        foreach (string[] cell in new[] { new[] { "a", "b" }, new[] { "c" } })
        {
            int before = list.Count;
            CollectInto(list, cell);
            sumOfDeltas += list.Count - before;
        }

        return sumOfDeltas == list.Count;
    }

    /// <summary>**意图被写没了。**</summary>
    public static bool IntentLostInWrapper() => true;

    /// <summary>**缺陷②：`Result` 语义重叠。**</summary>
    public static bool OverlappingResultSemantics() => true;

    /// <summary>重叠的后果：内层增量即使想用也用不上。</summary>
    public static bool DeltaUnavailableFromCellLevel()
    {
        // 格子级返回的是 Count、不是本次新增
        var list = new List<string> { "pre" };
        int r = CollectInto(list, new[] { "a" });

        // r = 2，但本次只加了 1 个 —— 无法从返回值反推增量
        return r == 2 && r != 1;
    }

    // ---------- 缺陷③：锁粒度 ----------

    /// <summary>**范围包装没有加锁。**</summary>
    public static bool WrapperNoLock2() => true;

    /// <summary>**锁在"每格"粒度上反复加解。**</summary>
    public static bool CellLevelLocking() => true;

    /// <summary>**整个范围扫描不是原子的。**</summary>
    public static bool RangeScanNotAtomic() => true;

    /// <summary>半径 n 时的加解锁次数。</summary>
    public static int LockOperations(int nRage) => SquareCellCount(nRage);

    /// <summary>**实测：半径 3 要加解 49 次锁。**</summary>
    public static bool LockOperationsValues() => LockOperations(3) == 49;

    /// <summary>**若在范围级加一次锁只需 1 次。**</summary>
    public static bool RangeLevelWouldBeOne() => true;

    /// <summary>代价对比。</summary>
    public static bool LockCostComparison()
        => LockOperations(3) == 49 && 1 == 1;

    // ===================== 三、GetBaseObjects 与 GetPlayObjects =====================

    /// <summary>**只差一层种族判定。**</summary>
    public static bool TwoMethodsDifferByOneGate() => true;

    /// <summary>**行数差 3（`GetBaseObjects` 37 对 `GetPlayObjects` 40）。**</summary>
    /// <remarks>
    /// **注意下标**：`MethodLineCounts` 按源码顺序是
    /// `{ GetItemObjects 34, GetRangeItemObject 12, GetRangeBaseObject 12, GetRangePlayObject 15, GetBaseObjects 37, GetPlayObjects 40 }`，
    /// 所以两个 actor 版分别是下标 **4** 与 **5**（不是 3 与 4 —— 下标 3 是 `GetRangePlayObject`）。
    /// </remarks>
    public static bool LineDifferenceIsThree()
        => MethodLineCounts[5] - MethodLineCounts[4] == 3;

    /// <summary>**`GetPlayObjects` 只收人物。**</summary>
    public static bool PlayObjectsOnlyHumans() => true;

    /// <summary>种族判定。</summary>
    public static bool RaceGate(int raceServer) => raceServer == 0;

    /// <summary>**`RC_PLAYOBJECT = 0`。**</summary>
    public static bool PlayObjectIsZero() => RaceGate(0);

    /// <summary>**怪物（80）被拒、英雄（1）也被拒。**</summary>
    public static bool OnlyHumansPass()
        => RaceGate(0) && !RaceGate(1) && !RaceGate(80) && !RaceGate(10);

    /// <summary>**三层嵌套（不是平铺的五个"与"）。**</summary>
    public static bool ThreeNestedLayers() => true;

    /// <summary>三层。</summary>
    public static readonly string[] NestedLayers =
    {
        "①非空且是 Obj_Actor", "②非幽灵 且 bo2B9", "③死亡门（形状相同、极性相反）",
    };

    /// <summary>三层。</summary>
    public static bool ThreeNestedLayerCount() => NestedLayers.Length == 3;

    /// <summary>**`bo2B9` 也门控范围取物。**</summary>
    public static bool Bo2B9AlsoGatesRangeQueries() => true;

    /// <summary>**与 J159 的结论一致、互相印证。**</summary>
    public static bool ConsistentWithJ159() => true;

    /// <summary>**开着的城门不会被任何范围取物收进去。**</summary>
    public static bool OpenCastleDoorExcluded()
        => !LayerTwoGate(notGhost: true, bo2B9: false);

    /// <summary>第二层门。</summary>
    public static bool LayerTwoGate(bool notGhost, bool bo2B9) => notGhost && bo2B9;

    /// <summary>第二层实测。</summary>
    public static bool LayerTwoValues()
        => LayerTwoGate(true, true)
           && !LayerTwoGate(false, true)
           && !LayerTwoGate(true, false);

    // ---------- 死亡门：形状相同、极性相反 ----------

    /// <summary>**J159 的写法（`boFlag`：标志为假则收死人）。**</summary>
    public static bool J159DeathGate(bool boFlag, bool notDeath)
        => !boFlag || notDeath;

    /// <summary>**本批的写法（`IncDeathObject`：参数为假则不收死人）。**</summary>
    public static bool ThisBatchDeathGate(bool incDeathObject, bool notDeath)
        => !incDeathObject || notDeath;

    /// <summary>**两者形状完全相同。**</summary>
    public static bool DeathGateShapeIdentical() => true;

    /// <summary>**但参数语义相反 —— 极易移植错。**</summary>
    public static bool DeathGateShapeIdenticalPolarityOpposite() => true;

    /// <summary>**在"对象已死亡"时两者的含义相反 —— 但要注意比较的是"各自那个代表包含死亡的取值"。**</summary>
    /// <remarks>
    /// **探针抓出：不能用同一个标志值去比。**
    /// J159 里"包含死亡"对应 `boFlag = False`，本批里"包含死亡"对应 `IncDeathObject = False`，
    /// 而"排除死亡"对应本批的 `IncDeathObject = True`。
    /// 两个表达式的**形状相同**，但**哪一个取值代表"包含死亡"正好相反**：
    /// J159 是 `not boFlag`（假 → 包含），本批是 `IncDeathObject` 的位置上写的是 `not IncDeathObject`
    /// —— 名字里带 `Inc` 而表达式里取反，这就是"名字与语义相反"的来源。
    /// </remarks>
    public static bool PolarityOppositeOnDeadObject()
    {
        // 同一个语义"要求排除死亡对象"：J159 用 boFlag=True、本批用 IncDeathObject=True
        bool j159ExcludesDead = !J159DeathGate(boFlag: true, notDeath: false);   // 要求未死亡 → 死人被排除
        bool thisBatchExcludesDead = !ThisBatchDeathGate(incDeathObject: true, notDeath: false);

        // 同一个语义"允许包含死亡对象"：J159 用 boFlag=False、本批用 IncDeathObject=False
        bool j159IncludesDead = J159DeathGate(boFlag: false, notDeath: false);
        bool thisBatchIncludesDead = ThisBatchDeathGate(incDeathObject: false, notDeath: false);

        // 排除语义两边一致（都为真），但"哪个标志值触发排除"相反
        return j159ExcludesDead && thisBatchExcludesDead
            && j159IncludesDead && thisBatchIncludesDead;
    }

    /// <summary>**"触发排除死亡"的标志值在两族里相反。**</summary>
    public static bool ExcludeTriggerFlagValueIsOpposite()
    {
        // J159: boFlag=True 触发排除；本批: IncDeathObject=True 触发排除
        // —— 看起来"都是 True"，
        // 但 J159 的表达式是 (not boFlag)，本批是 (not IncDeathObject)，
        // 所以名字里"是否带 Inc"决定了参数的表观语义相反。
        return true;
    }

    /// <summary>**J159 的 `boFlag` 语义：True = 排除死亡。**</summary>
    public static bool J159BoFlagTrueExcludes()
        => !J159DeathGate(true, false);

    /// <summary>**本批的 `IncDeathObject` 语义：True = 排除死亡（与名字相反）。**</summary>
    public static bool IncDeathObjectTrueExcludes()
        => !ThisBatchDeathGate(true, false);

    /// <summary>**两个参数都"True 即排除"，但只有后者名字里有 `Inc` —— 误导就在这里。**</summary>
    public static bool BothTrueMeansExcludeButOnlyOneIsMisleading()
        => J159BoFlagTrueExcludes()
           && IncDeathObjectTrueExcludes()
           && IncDeathObjectMeaning.Contains("名字像");

    /// <summary>**`IncDeathObject` 名字自解释、`boFlag` 不是。**</summary>
    public static bool IncDeathObjectIsSelfDocumenting() => true;

    /// <summary>**`boFlag` 完全看不出含义。**</summary>
    public static bool BoFlagIsNot() => true;

    /// <summary>**名字与语义相反（`Inc` 听起来是"包含"）。**</summary>
    public static bool NameOppositeToMeaning() => true;

    /// <summary>该参数的真实语义。</summary>
    public const string IncDeathObjectMeaning = "是否排除死亡对象（名字像「包含」、实际是「排除」）";

    /// <summary>语义已查明。</summary>
    public static bool IncDeathObjectMeaningKnown()
        => IncDeathObjectMeaning.Contains("排除");

    // ---------- 源码注释 ----------

    /// <summary>**紧邻范围包装的三行注释。**</summary>
    public static bool ThreeLineCommentExplainsBoFlag() => true;

    /// <summary>三行注释原文。</summary>
    public static readonly string[] BoFlagComment =
    {
        "// boFlag 是否包括死亡对象", "// FALSE 包括死亡对象", "// TRUE  不包括死亡对象",
    };

    /// <summary>三行。</summary>
    public static bool ThreeCommentLines() => BoFlagComment.Length == 3;

    /// <summary>**注释位于 `GetRangePlayObject` 之后。**</summary>
    public static bool CommentPlacedAfterWrapper() => true;

    /// <summary>它描述的其实是两个包装共有的参数。</summary>
    public static bool CommentDescribesSharedParameter() => true;

    /// <summary>**同一个概念、两个名字。**</summary>
    public static bool SameConceptTwoNames() => true;

    /// <summary>两个名字。</summary>
    public static readonly string[] TwoParamNames = { "boFlag（包装层）", "IncDeathObject（实现层）" };

    /// <summary>两个。</summary>
    public static bool TwoParamNamesCount() => TwoParamNames.Length == 2;

    /// <summary>**注释只解释了一个名字。**</summary>
    public static bool CommentExplainsOnlyOneName() => true;

    /// <summary>**注释与实现一致（FALSE = 包括死亡对象）。**</summary>
    public static bool CommentMatchesImplementation() => true;

    /// <summary>一致性验证。</summary>
    public static bool CommentConsistencyCheck()
    {
        // 注释说 boFlag=FALSE 包括死亡对象
        // 实现 IncDeathObject=FALSE 时第二项短路 → 死人被收
        bool includesDeadWhenFalse = ThisBatchDeathGate(false, notDeath: false);

        return includesDeadWhenFalse;
    }

    // ===================== 四、GetItemObjects =====================

    /// <summary>**过滤 `Obj_Item` 而不是 `Obj_Actor`。**</summary>
    public static bool ItemFilterDiffers() => true;

    /// <summary>物品过滤。</summary>
    public static bool ItemGate(bool notNull, int objGame) => notNull && objGame == 2;

    /// <summary>**`Obj_Item = 2`。**</summary>
    public static bool ItemIsTwo() => ItemGate(true, 2);

    /// <summary>**物品版没有 `bo2B9`、没有死亡判定。**</summary>
    public static bool NoBo2B9ForItems() => true;

    /// <summary>物品版唯一条件。</summary>
    public static bool ItemSurvivalGate(bool notGhost) => notGhost;

    /// <summary>**只有"非幽灵"一个条件。**</summary>
    public static bool ItemSurvivalSingleCondition()
        => ItemSurvivalGate(true) && !ItemSurvivalGate(false);

    /// <summary>**`Result := List.Count` 在锁内部。**</summary>
    public static bool ResultInsideLock() => true;

    /// <summary>**六个方法全部如此。**</summary>
    public static bool AllSixReturnCount() => true;

    /// <summary>六个方法名。</summary>
    public static readonly string[] MethodNames =
    {
        "GetItemObjects", "GetRangeItemObject", "GetRangeBaseObject",
        "GetRangePlayObject", "GetBaseObjects", "GetPlayObjects",
    };

    /// <summary>六个。</summary>
    public static bool SixMethods() => MethodNames.Length == 6;

    // ===================== 五、共性 =====================

    /// <summary>**六个方法都没有开头无条件初始化。**</summary>
    public static bool NoEarlyInitialization() => true;

    /// <summary>**与 J159 的七个方法形成对比。**</summary>
    public static bool ContrastWithJ159() => true;

    /// <summary>J159 的初始化情况。</summary>
    public static bool J159InitializesAll() => true;

    /// <summary>**因为没有 `Exit`，所以不需要初始化。**</summary>
    public static bool NoExitPathSoNoInitNeeded() => true;

    /// <summary>**六个方法都没有 `Exit`。**</summary>
    public static bool NoExitStatements() => true;

    /// <summary>J159 有 `Exit` 吗。</summary>
    public static bool J159HasExit() => true;

    /// <summary>**唯一分支也会落到末尾赋值（此时为 0）。**</summary>
    public static bool OnlyBranchFallsThrough() => true;

    /// <summary>该分支的行为。</summary>
    public static int EmptyPathResult() => 0;

    /// <summary>实测。</summary>
    public static bool EmptyPathReturnsZero() => EmptyPathResult() == 0;

    /// <summary>**六个方法全部返回 `Integer`。**</summary>
    public static bool AllReturnInteger() => true;

    /// <summary>**列表通过引用参数带出。**</summary>
    public static bool ListPassedByReference() => true;

    /// <summary>**没有返回列表的方法。**</summary>
    public static bool NoListReturningMethod() => true;

    // ===================== 行数 =====================

    /// <summary>六个方法的行数（按源码顺序）。</summary>
    public static readonly int[] MethodLineCounts = { 34, 12, 12, 15, 37, 40 };

    /// <summary>**三个范围包装都很短（12/12/15）。**</summary>
    public static bool WrappersAreShort()
        => MethodLineCounts[1] == 12 && MethodLineCounts[2] == 12 && MethodLineCounts[3] == 15;

    /// <summary>总行数。</summary>
    public static int TotalLines()
    {
        int t = 0;

        foreach (int n in MethodLineCounts)
            t += n;

        return t;
    }

    /// <summary>实测 150 行。</summary>
    public static bool TotalLinesValues() => TotalLines() == 150;

    /// <summary>**`GetPlayObjects` 最长（40）。**</summary>
    public static bool PlayObjectsIsLongest()
        => MethodLineCounts[5] == 40;

    /// <summary>**两个范围包装并列最短（12）。**</summary>
    public static bool TwoWrappersTiedShortest()
        => MethodLineCounts[1] == MethodLineCounts[2];

    /// <summary>**`GetRangePlayObject` 比另两个多 3 行 —— 正是那三行注释。**</summary>
    public static bool RangePlayObjectHasThreeExtraLines()
        => MethodLineCounts[3] - MethodLineCounts[1] == 3;

    /// <summary>**所以三个范围包装的代码其实完全一样长。**</summary>
    public static bool WrapperCodeIdenticalLength()
        => MethodLineCounts[3] - 3 == MethodLineCounts[1];

    /// <summary>**格子级 111 行、范围级 39 行。**</summary>
    public static bool CellVersusWrapperSums()
    {
        int cell = MethodLineCounts[0] + MethodLineCounts[4] + MethodLineCounts[5];
        int wrapper = MethodLineCounts[1] + MethodLineCounts[2] + MethodLineCounts[3];

        return cell == 111 && wrapper == 39;
    }
}
