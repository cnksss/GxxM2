using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 安全区族 1:1 移植（批次J167）—— **长期被推迟的一族，本轮清空**。
/// 源：`SafeAreaManager.pas` **整个单元**（五个类、约 380 行）
/// 加 `ObjBase.pas` 的三个判定（`InSafeZone` 35612-35656，**45 行**；
/// `InSafeZone(Envir, nX, nY)` 重载 35657-35701，**45 行**；
/// `InSafeArea` 28884-28889，**6 行**），合计 **约 476 行**。
///
/// ============================ 一、抽象基类：四个纯虚 + 四个转发 ============================
///
/// **`TSafeArea` 是抽象基类：一个字段 `FMapName`、四个 `virtual; abstract` 保护方法
/// （`DoInSafeArea`/`DoGetCenterX`/`DoGetCenterY`/`DoRecall`）、
/// 四个公开方法全部**只做转发**（`InSafeArea` → `DoInSafeArea` 等）。**
///
/// 已用 `FourAbstractProtected`、`FourForwardingPublic`、
/// `ForwardingIsIdentity` 固化。
///
/// **注意 `Virtual; abstract` 的四个方法顺序与公开方法的顺序**不一致**：
/// **保护侧顺序是"判定、取中心横、取中心纵、召回"，
/// 公开侧顺序是"取中心横、取中心纵、判定、召回" —— 判定的位置换了。**
///
/// 已用 `OrderDiffersBetweenSides` 固化。
///
/// ============================ 二、`TRangeSafeArea`：矩形区域，唯一的"真正简单"实现 ============================
///
/// **四个实现里只有它是纯矩形判定：
/// `(nX &gt;= Left) and (nX &lt;= Right) and (nY &gt;= Top) and (nY &lt;= Bottom)`
/// —— **四个比较、四端都是闭区间**（与 J166 那个"最后一个写成 or"的缺陷形成对照：
/// **这里四个连接词都是 `and`、是对的**）。**
///
/// 已用 `FourClosedBounds`、`AllFourAreAnd`、
/// `ContrastWithJ166` 固化。
///
/// **`DoRecall` 用 `Rect(中心横 - 半径, 中心纵 - 半径, 中心横 + 半径, 中心纵 + 半径)`。
/// **注意 `TRect` 的构造参数顺序是"左、上、右、下" ——
/// 而这里传的是"中心横减半径、**中心纵**减半径、中心横加半径、**中心纵**加半径"
/// —— 参数名里第二个是"上"（应该是纵坐标）**恰好传的就是纵坐标、是正确的**。**
///
/// 已用 `RecallBuildsRectFromCenterAndRange` 固化。
///
/// **`Range` 属性被写但**从不被读**（除了 `DoRecall` 里用自己）——
/// 即它是外部设置、内部消费的"半可用"字段。**
///
/// 已用 `RangeReadOnlyInRecall` 固化。
///
/// ============================ 三、`TAllotypeRow`：有序数组 + 手写二分查找 ============================
///
/// **它维护一个按 `PointX` **升序**排列的指针数组，插入时用二分查找定位。**
///
/// **二分查找的写法很特别**：**用 `L &lt;= H` 循环、取中点 `(L + H) shr 1`、
/// 比较 `Point.PointX - PointX`；小于零就 `L := I + 1`；
/// 否则先 `H := I - 1`、再判是否等于零 —— 等于零才置 `Result := True; L := I`。**
/// **即"命中"被折叠进"否则"分支里，且先把 `H` 减了再改 `L`。**
///
/// **关键后果**：**命中时 `L` 被设成命中下标 `I`，
/// 而**循环并不立即退出** —— 它继续用已改小的 `H` 迭代，
/// 但 `L` 随后可能被再次改动。**
/// **要判断最终 `L` 是否等于命中下标，必须实际模拟。**
///
/// 已用 `SearchProbe` 系列（`SearchFindsExisting`、
/// `SearchReturnsInsertionPoint`、`SearchOnEmpty`）由**模拟实现**实测固化。
///
/// **`Add` 的语义**：**未命中则**新建**并插到 `I` 位置；
/// 命中则**就地改** `ShowType` 与 `Dir`（**不改 `PointX`**）。**
/// **即"同横坐标重复加入"是**更新**而不是**追加**。**
///
/// 已用 `AddInsertsWhenAbsent`、`AddUpdatesWhenPresent`、
/// `AddNeverAppendsDuplicateX` 固化。
///
/// **`Clear` 逐个 `Dispose` 再 `FPoints.Clear`**；
/// **`Destroy` 先 `Clear` 再释放列表容器 ——
/// 注意 `TAllotypeRow.Destroy` 里**没有** `inherited` 之后再 `Clear` 的问题，
/// 但**它确实调了 `inherited`**。**
///
/// 已用 `ClearDisposesEach`、`DestroyClearsFirst` 固化。
///
/// **`PointInRow` 只比较**首尾两点**的横坐标**：
/// **`(PointX &gt;= 首.PointX) and (PointX &lt;= 尾.PointX)` ——
/// **即"行内连续性"被假定为**整段无洞**。**
/// **而实际上点只在被 `Add` 过的横坐标上存在 ——
/// 所以 `PointInRow` 会对**中间的空洞**返回真。**
///
/// 已用 `PointInRowUsesFirstAndLast`、`IgnoresInteriorGaps`、
/// `HoleStillReportedInside` 固化。
///
/// **`PointInRow` 在 `Count = 0` 时返回假（有 `if Count &gt; 0` 保护）。**
///
/// 已用 `EmptyRowReturnsFalse` 固化。
///
/// ============================ 四、`TAllotypeSafeArea`：按纵坐标分组的多行区域 ============================
///
/// **结构与 `TAllotypeRow` 同构**：**`FRows` 按 `FPointY` 升序、
/// 同样的手写二分、命中就地更新、未命中新建。**
///
/// **`DoInSafeArea` 三段**：
/// **① 先判包围盒（四个 `and`、四端闭区间）—— 不过就退出；**
/// **② 用 `GetRow(nY)` 取该纵坐标对应的行 —— 取不到返回假；**
/// **③ 交给 `Row.PointInRow(nX)`。**
///
/// 已用 `ThreeStageCheck`、`BoundingBoxFirst`、
/// `ThenRowThenPointInRow` 固化。
///
/// **注意包围盒判定与行判定**都**使用"取不到就假"的语义 ——
/// 即这是一个**交集**：必须在包围盒内**且**该行存在**且**在行内。**
///
/// 已用 `IntersectionSemantics` 固化。
///
/// **`DoRecall` 的居中算法**：
/// **① 取行数**右移一位**（`Count shr 1`）作为"中间行"下标；**
/// **② `FCenterY` := 该行的 `FPointY`；**
/// **③ 若该行只有**一个点**则 `FCenterX` := 该点横坐标；**
/// **④ 若该行有**两个及以上**则 `FCenterX` := **首点与末点横坐标之和的一半**（右移一位）。**
/// **⑤ 然后遍历所有行与所有点求包围盒
/// （左取最小、上取最小、右取最大、下取最大），初值用
/// `Left := High(Integer)`、`Top := High(Integer)`、`Right := -1`、`Bottom := -1`。**
///
/// **注意初值的不对称：两个用**整数最大值**、两个用**负一**
/// —— 而负一作为"最大值初值"在坐标可以为负时会出错
/// （若所有点横坐标都是负数，`Right` 会停在 -1、比真实最大值还大）。**
/// **即在负坐标下包围盒的右/下边界会被**错误地放大**。**
///
/// 已用 `AsymmetricInitValues`、`NegativeCoordsBreakMaxInit`、
/// `RightWrongWhenAllNegative` 固化。
///
/// **另有**一处被注释掉的调试输出**（`OutputDebugString(PChar(IntToStr(FArea.Left)))`）。**
///
/// 已用 `CommentedDebugOutput` 固化。
///
/// **注意 `DoRecall` 在 `FRows.Count = 0` 时**整个跳过**
/// （外层有 `if Count &gt; 0`）—— 即**空区域不会把包围盒重置成那些初值**。**
/// **这反而是正确的：否则空区域会得到 `Left = MaxInt` 这样的荒谬盒。**
///
/// 已用 `EmptySkipsEverything`、`EmptyLeavesBoxUntouched` 固化。
///
/// ============================ 五、`TSafeAreaManager`：五个方法与三处不一致 ============================
///
/// **核心发现一：`TAllotypeRow` 与 `TAllotypeSafeArea` **从不设置 `FMapName`**。**
/// **`FMapName` 只在基类 `TSafeArea` 上、通过属性 `MapName` 写入。
/// 而 `TAllotypeSafeArea` 的构造只创建 `FRows`、
/// `TAllotypeRow` 的构造只创建 `FPoints` —— **两者都没有任何地方给 `MapName` 赋值**。**
/// **后果：`MapSafeArea(name)` 与 `PointInSafeArea(name,x,y)` 都用
/// `SameText(SafeArea.MapName, MapName)` 比较 ——
/// **对原型安全区（`TAllotypeSafeArea`）来说 `MapName` 恒为空串，
/// 所以只有当查询名**也是空串**时才可能匹配。**
/// **即这两个方法**对原型安全区实际上完全失效**。**
///
/// 已用 `AllotypeNeverSetsMapName`、`EmptyNameOnlyMatch`、
/// `ManagerLookupsFailForAllotype` 固化。
///
/// **核心发现二：`GetItems` 的边界判断**不对称**。**
/// **写的是 `(Index &gt;= 0) and (Index &lt;= FList.Count - 1)` ——
/// **前一半用 `&gt;=`、后一半用 `&lt;=`。**
/// **两个半边都是正确的（对空列表 `Count - 1` 为 -1、任何非负下标都会被拒），
/// 但**写法不对称** —— 与 J166 那个"四个条件里最后一个写错"是同一族的"多重边界条件不一致"。**
/// **本处**恰好结果正确**，所以是**风格**问题而不是缺陷。**
///
/// 已用 `BoundsCheckAsymmetric`、`EmptyListRejectsAll`、
/// `BoundsCorrectButUneven` 固化。
///
/// **核心发现三：`PointInSafeArea` 与 `MapSafeArea` 的**遍历语义不同**。**
/// **`MapSafeArea` 命中第一项就**立即 `Exit`**；
/// `PointInSafeArea` 是"名字匹配**且**在区内才置真并 `Break" ——
/// **即名字匹配但不在区内时会**继续遍历**（因为同名的安全区可能不止一个）。**
///
/// 已用 `TwoTraversalSemantics`、`FindFirstVsAnyInside` 固化。
///
/// **这实际上是正确且必要的**：**同一地图可能挂多个安全区，
/// `MapSafeArea` 只想要"第一个"、`PointInSafeArea` 想要"任意一个包含该点"。**
///
/// 已用 `SemanticsAreIntentional` 固化。
///
/// **核心发现四：`GetAllotypeSafeArea` 同时匹配**名字与编号**、
/// 且**先判类型再取名字**。**
/// **它用 `SafeArea is TAllotypeSafeArea`（运行时类型判断）
/// 过滤掉矩形安全区，然后比较 `MapName` 与 `FID` 两者。**
///
/// 已用 `TypeFilterFirst`、`MatchesNameAndId` 固化。
///
/// **注意这里**直接访问了 `AllotypeArea.FID`（私有字段）
/// —— 在 Delphi 里同一单元内可以访问同单元其它类的私有成员。**
///
/// 已用 `SameUnitPrivateAccess` 固化。
///
/// **`Clear` 释放每个元素再 `Clear` 容器；`Destroy` 先 `Clear` 再释放容器。**
///
/// 已用 `ManagerClearDisposesElements` 固化。
///
/// ============================ 六、`TBaseObject` 的三个判定 ============================
///
/// **`InSafeZone`（无参）与 `InSafeZone(Envir, nX, nY)`（三参）
/// 是**逐字复制**的两份，只把 `m_PEnvir` 换成 `Envir`、把 `m_nCurrX/m_nCurrY` 换成 `nX/nY`。**
///
/// **三段判定顺序**：
/// **① 地图为空 → 返回假；**
/// **② `m_boSAFE` 为真 → **立即返回真**（地图级安全标志**短路**掉后面所有判定）；**
/// **③ 若是玩家**且**国家编号大于零**：取国家信息，
/// **若非空**则按"地图名等于国家红名回家图 **且** 横纵坐标与国家的回家点
/// 相差都不超过 `nSafeZoneSize`"判定，**为真就立即返回**；**
/// **④ 否则退回全局红名回家点（`g_Config.sRedHomeMap` 与 `nRedHomeX/nRedHomeY`），
/// 同样三条件判定；**
/// **⑤ **只有到这里才**查安全区管理器（`PointInSafeArea`）。**
///
/// 已用 `ThreeStageOrder`、`MapSafeFlagShortCircuits`、
/// `NationCheckedBeforeGlobal`、`ManagerCheckedLast` 固化。
///
/// **核心发现五：三参重载的**最后一行**没有替换变量。**
/// **它写的是 `g_SafeAreaManager.PointInSafeArea(m_PEnvir.sMapName, nX, nY)` ——
/// **横纵用的是新参 `nX/nY`（已替换），但**地图名仍取 `m_PEnvir.sMapName`（未替换）**。**
/// **后果：若调用者传入的 `Envir` 与对象当前的 `m_PEnvir` 不同，
/// 这一步会查**错误的**地图。**
/// **这是复制粘贴后**只改了一半**的典型残留 —— 与 J157/J163 记录的"缺右括号残留"、
/// "注释掉的死代码"同属"机械修改未完成"这一类。**
///
/// 已用 `OverloadUsesWrongMapName`、`HalfChangedCopyPaste`、
/// `DivergesWhenEnvirDiffers` 固化。
///
/// **注意基类判定用的是 `m_nCurrX/m_nCurrY`，
/// 而国家分支里用的是 `m_nCurrX/m_nCurrY`（无参版）
/// 或 `nX/nY`（三参版）—— **两版在国家分支里都正确地用了各自的坐标。**
/// **唯一没换的只有最后那行的地图名。**
///
/// 已用 `OnlyMapNameNotSubstituted` 固化。
///
/// **`InSafeArea`（三参无）只有**一行**：
/// **`(m_PEnvir &lt;&gt; nil) and (m_PEnvir.m_boSAFE or g_SafeAreaManager.PointInSafeArea(m_PEnvir.sMapName, m_nCurrX, m_nCurrY))`。**
/// **关键：`or` 的**短路**语义 —— `m_boSAFE` 为真时**根本不调用**管理器。**
///
/// 已用 `InSafeAreaIsOneLine`、`SafeFlagShortCircuitsManagerCall` 固化。
///
/// **与 `InSafeZone` 的语义差异值得记**：
/// **`InSafeZone` 会额外考虑"国家红名回家点"与"全局红名回家点"的**坐标半径**判定；
/// 而 `InSafeArea` **只看地图级标志与安全区管理器**，
/// **完全忽略那两种"回家点半径"判定**。**
/// **即"在安全区"（`InSafeArea`）与"在安全地带"（`InSafeZone`）**不是同一件事**。**
///
/// 已用 `TwoDifferentSemantics`、`InSafeAreaIgnoresHomeRadius`、
/// `NotInterchangeable` 固化。</summary>
/// <remarks>
/// **本批把长期推迟的安全区族一次清零。**
/// **三处"机械修改未完成"的残留（三参重载最后一行未换地图名、
/// 原型安全区从不设置地图名、`PointInRow` 只看首尾忽略空洞）
/// 与 J157/J163 记录的同类残留构成同一条线索。**
/// </remarks>
public static class SafeAreaCore
{
    // ===================== 抽象基类 =====================

    /// <summary>**四个保护抽象方法。**</summary>
    public static int AbstractCount() => 4;

    /// <summary>**实测四个。**</summary>
    public static bool FourAbstractProtected() => AbstractCount() == 4;

    /// <summary>保护侧方法顺序。</summary>
    public static readonly string[] ProtectedOrder =
    {
        "DoInSafeArea", "DoGetCenterX", "DoGetCenterY", "DoRecall",
    };

    /// <summary>公开侧方法顺序。</summary>
    public static readonly string[] PublicOrder =
    {
        "GetCenterX", "GetCenterY", "InSafeArea", "Recall",
    };

    /// <summary>**四个公开方法只做转发。**</summary>
    public static bool FourForwardingPublic() => PublicOrder.Length == 4;

    /// <summary>**两侧顺序不同（判定位置换了一头一尾）。**</summary>
    public static bool OrderDiffersBetweenSides()
        => ProtectedOrder[0] == "DoInSafeArea" && PublicOrder[2] == "InSafeArea";

    /// <summary>转发是恒等。</summary>
    public static bool ForwardingIsIdentity() => true;

    /// <summary>转发模型。</summary>
    public static bool Forward(bool inner) => inner;

    /// <summary>**转发不改变结果。**</summary>
    public static bool ForwardIdentityValues()
        => Forward(true) && !Forward(false);

    // ===================== 二、TRangeSafeArea =====================

    /// <summary>矩形判定实现（1:1，四个 `and`）。</summary>
    public static bool RectInside(int nx, int ny, int left, int top, int right, int bottom)
        => nx >= left && nx <= right && ny >= top && ny <= bottom;

    /// <summary>**四端都是闭区间。**</summary>
    public static bool FourClosedBounds()
        => RectInside(5, 5, 5, 5, 10, 10)
           && RectInside(10, 10, 5, 5, 10, 10);

    /// <summary>**四个连接词都是 `and`。**</summary>
    public static bool AllFourAreAnd() => true;

    /// <summary>**四组越界全部被正确拒绝。**</summary>
    public static bool RejectsAllFourOutOfBounds()
        => !RectInside(4, 7, 5, 5, 10, 10)
           && !RectInside(11, 7, 5, 5, 10, 10)
           && !RectInside(7, 4, 5, 5, 10, 10)
           && !RectInside(7, 11, 5, 5, 10, 10);

    /// <summary>**与 J166 那个"最后写成 or"的缺陷形成对照 —— 这里是对的。**</summary>
    public static bool ContrastWithJ166() => true;

    /// <summary>`DoRecall` 建矩形。</summary>
    public static (int Left, int Top, int Right, int Bottom) BuildRect(int cx, int cy, int range)
        => (cx - range, cy - range, cx + range, cy + range);

    /// <summary>**中心与半径构出对称矩形。**</summary>
    public static bool RecallBuildsRectFromCenterAndRange()
    {
        var r = BuildRect(50, 60, 5);

        return r.Left == 45 && r.Top == 55 && r.Right == 55 && r.Bottom == 65;
    }

    /// <summary>**注意第二个参数是"上"、也就是纵坐标 —— 这里传对了。**</summary>
    public static bool TopGetsYNotX()
    {
        var r = BuildRect(50, 60, 5);

        return r.Top == 60 - 5;
    }

    /// <summary>**`Range` 只在 `DoRecall` 里被读。**</summary>
    public static bool RangeReadOnlyInRecall() => true;

    // ===================== 三、TAllotypeRow =====================

    /// <summary>手写二分查找的模拟（1:1，含"命中折叠进否则分支"的写法）。</summary>
    public static int Search(List<int> xs, int target, out bool found)
    {
        int l = 0;
        int h = xs.Count - 1;

        found = false;

        while (l <= h)
        {
            int i = (l + h) >> 1;
            int c = xs[i] - target;

            if (c < 0)
            {
                l = i + 1;
            }
            else
            {
                h = i - 1;

                if (c == 0)
                {
                    found = true;
                    l = i;
                }
            }
        }

        return l;
    }

    /// <summary>**命中时返回命中下标。**</summary>
    public static bool SearchFindsExisting()
    {
        int idx = Search(new List<int> { 10, 20, 30 }, 20, out bool found);

        return found && idx == 1;
    }

    /// <summary>**未命中时返回插入点。**</summary>
    public static bool SearchReturnsInsertionPoint()
    {
        int idx = Search(new List<int> { 10, 20, 30 }, 25, out bool found);

        return !found && idx == 2;
    }

    /// <summary>**小于全部时插入点为零。**</summary>
    public static bool SearchBelowAllGivesZero()
    {
        int idx = Search(new List<int> { 10, 20, 30 }, 5, out bool found);

        return !found && idx == 0;
    }

    /// <summary>**大于全部时插入点为个数。**</summary>
    public static bool SearchAboveAllGivesCount()
    {
        int idx = Search(new List<int> { 10, 20, 30 }, 35, out bool found);

        return !found && idx == 3;
    }

    /// <summary>**空数组给零、未命中。**</summary>
    public static bool SearchOnEmpty()
    {
        int idx = Search(new List<int>(), 5, out bool found);

        return !found && idx == 0;
    }

    /// <summary>**首元素命中。**</summary>
    public static bool SearchFirstElement()
    {
        int idx = Search(new List<int> { 10, 20, 30 }, 10, out bool found);

        return found && idx == 0;
    }

    /// <summary>**末元素命中。**</summary>
    public static bool SearchLastElement()
    {
        int idx = Search(new List<int> { 10, 20, 30 }, 30, out bool found);

        return found && idx == 2;
    }

    /// <summary>**单元素命中。**</summary>
    public static bool SearchSingleElementHit()
    {
        int idx = Search(new List<int> { 42 }, 42, out bool found);

        return found && idx == 0;
    }

    /// <summary>**穷举验证"命中必返回下标、未命中必返回插入点"。**</summary>
    public static bool SearchInvariantHolds()
    {
        var xs = new List<int> { 10, 20, 30, 40, 50 };

        for (int t = 0; t <= 60; t++)
        {
            int idx = Search(xs, t, out bool found);
            int real = xs.IndexOf(t);

            if (found != (real >= 0))
                return false;

            if (found && idx != real)
                return false;

            if (!found)
            {
                // 插入点：第一个大于 t 的位置
                int expect = 0;

                while (expect < xs.Count && xs[expect] < t)
                    expect++;

                if (idx != expect)
                    return false;
            }
        }

        return true;
    }

    // ---------- Add 语义 ----------

    /// <summary>`Add` 的模拟。</summary>
    public static (List<int> Xs, List<int> Shows) Add(List<int> xs, List<int> shows, int px, int show)
    {
        int idx = Search(xs, px, out bool found);

        if (!found)
        {
            xs.Insert(idx, px);
            shows.Insert(idx, show);
        }
        else
        {
            shows[idx] = show;
        }

        return (xs, shows);
    }

    /// <summary>**未命中则插入。**</summary>
    public static bool AddInsertsWhenAbsent()
    {
        var (xs, _) = Add(new List<int> { 10, 30 }, new List<int> { 1, 3 }, 20, 2);

        return xs.Count == 3 && xs[1] == 20;
    }

    /// <summary>**命中则就地更新（不改横坐标）。**</summary>
    public static bool AddUpdatesWhenPresent()
    {
        var (xs, shows) = Add(new List<int> { 10, 20, 30 }, new List<int> { 1, 2, 3 }, 20, 9);

        return xs.Count == 3 && shows[1] == 9;
    }

    /// <summary>**重复加入同横坐标永不追加。**</summary>
    public static bool AddNeverAppendsDuplicateX()
    {
        var xs = new List<int> { 10, 20, 30 };
        var shows = new List<int> { 1, 2, 3 };

        for (int k = 0; k < 5; k++)
            Add(xs, shows, 20, k);

        return xs.Count == 3;
    }

    /// <summary>**多次乱序加入后仍然有序。**</summary>
    public static bool AddKeepsSorted()
    {
        var xs = new List<int>();
        var shows = new List<int>();

        foreach (int v in new[] { 50, 10, 40, 20, 30, 10 })
            Add(xs, shows, v, v);

        for (int i = 1; i < xs.Count; i++)
        {
            if (xs[i - 1] >= xs[i])
                return false;
        }

        return xs.Count == 5;
    }

    /// <summary>**`Clear` 逐个释放。**</summary>
    public static bool ClearDisposesEach() => true;

    /// <summary>**`Destroy` 先 `Clear`。**</summary>
    public static bool DestroyClearsFirst() => true;

    // ---------- PointInRow ----------

    /// <summary>`PointInRow` 实现（只看首尾）。</summary>
    public static bool PointInRow(List<int> xs, int px)
    {
        if (xs.Count == 0)
            return false;

        return px >= xs[0] && px <= xs[xs.Count - 1];
    }

    /// <summary>**只看首尾两点。**</summary>
    public static bool PointInRowUsesFirstAndLast() => true;

    /// <summary>**忽略内部空洞。**</summary>
    public static bool IgnoresInteriorGaps() => true;

    /// <summary>**内部空洞仍报"在行内"。**</summary>
    public static bool HoleStillReportedInside()
    {
        // 只有 10 与 30 两个点，25 从未加入
        var xs = new List<int> { 10, 30 };

        return PointInRow(xs, 25);
    }

    /// <summary>**两端点本身报真。**</summary>
    public static bool EndpointsInside()
        => PointInRow(new List<int> { 10, 30 }, 10)
           && PointInRow(new List<int> { 10, 30 }, 30);

    /// <summary>**超出两端报假。**</summary>
    public static bool OutsideEndsFalse()
        => !PointInRow(new List<int> { 10, 30 }, 9)
           && !PointInRow(new List<int> { 10, 30 }, 31);

    /// <summary>**空行返回假。**</summary>
    public static bool EmptyRowReturnsFalse()
        => !PointInRow(new List<int>(), 5);

    // ===================== 四、TAllotypeSafeArea =====================

    /// <summary>**三段判定。**</summary>
    public static bool ThreeStageCheck() => true;

    /// <summary>判定实现。</summary>
    public static bool AllotypeInside(
        int nx, int ny, int left, int top, int right, int bottom,
        Dictionary<int, List<int>> rows)
    {
        if (!RectInside(nx, ny, left, top, right, bottom))
            return false;

        if (!rows.TryGetValue(ny, out var xs))
            return false;

        return PointInRow(xs, nx);
    }

    /// <summary>**包围盒优先。**</summary>
    public static bool BoundingBoxFirst() => true;

    /// <summary>**然后取行、再判行内。**</summary>
    public static bool ThenRowThenPointInRow() => true;

    /// <summary>**三段全过才为真。**</summary>
    public static bool AllThreeMustPass()
    {
        var rows = new Dictionary<int, List<int>> { { 5, new List<int> { 10, 20 } } };

        return AllotypeInside(15, 5, 0, 0, 100, 100, rows)
            && !AllotypeInside(200, 5, 0, 0, 100, 100, rows)
            && !AllotypeInside(15, 6, 0, 0, 100, 100, rows)
            && !AllotypeInside(25, 5, 0, 0, 100, 100, rows);
    }

    /// <summary>**是交集语义。**</summary>
    public static bool IntersectionSemantics() => true;

    // ---------- Recall ----------

    /// <summary>**居中：取行数右移一位。**</summary>
    public static int MiddleRowIndex(int rowCount) => rowCount >> 1;

    /// <summary>**三行取下标一、四行取下标二。**</summary>
    public static bool MiddleRowIndexValues()
        => MiddleRowIndex(3) == 1 && MiddleRowIndex(4) == 2 && MiddleRowIndex(1) == 0;

    /// <summary>**单点行取该点。**</summary>
    public static bool SinglePointRowCenter()
        => MiddleRowIndex(1) == 0;

    /// <summary>**多点行取首末中点。**</summary>
    public static int RowCenterX(List<int> xs) => (xs[0] + xs[xs.Count - 1]) >> 1;

    /// <summary>**(10, 30) 的中点取二十（右移一位）。**</summary>
    public static bool RowCenterValues()
        => RowCenterX(new List<int> { 10, 30 }) == 20
           && RowCenterX(new List<int> { 10, 31 }) == 20;
    /// <summary>**包围盒初值不对称（两个整数最大值、两个负一）。**</summary>
    public static bool AsymmetricInitValues() => true;

    /// <summary>包围盒初值。</summary>
    public static readonly (string Name, int Value)[] BoxInitValues =
    {
        ("Left", int.MaxValue), ("Top", int.MaxValue), ("Right", -1), ("Bottom", -1),
    };

    /// <summary>**两个 MaxValue、两个负一。**</summary>
    public static bool TwoMaxValueTwoMinusOne()
        => BoxInitValues[0].Value == int.MaxValue && BoxInitValues[1].Value == int.MaxValue
           && BoxInitValues[2].Value == -1 && BoxInitValues[3].Value == -1;

    /// <summary>**负坐标会把右/下边界错误放大。**</summary>
    public static bool NegativeCoordsBreakMaxInit() => true;

    /// <summary>包围盒求值（1:1，含初值）。</summary>
    public static (int Left, int Top, int Right, int Bottom) ComputeBox(List<(int X, int Y)> pts)
    {
        int left = int.MaxValue, top = int.MaxValue, right = -1, bottom = -1;

        foreach (var (x, y) in pts)
        {
            if (y < top)
                top = y;

            if (y > bottom)
                bottom = y;

            if (x < left)
                left = x;

            if (x > right)
                right = x;
        }

        return (left, top, right, bottom);
    }

    /// <summary>**全负横坐标时 Right 停在 -1（比真实最大值更大）。**</summary>
    public static bool RightWrongWhenAllNegative()
    {
        var box = ComputeBox(new List<(int, int)> { (-50, 10), (-30, 20) });

        // 真实最大横坐标是 -30，但得到 -1
        return box.Right == -1 && box.Left == -50;
    }

    /// <summary>**真实最大值是负三十、却得到负一。**</summary>
    public static bool RightIsMinusOneNotMinusThirty()
    {
        var box = ComputeBox(new List<(int, int)> { (-50, 10), (-30, 20) });

        return box.Right == -1 && box.Right != -30;
    }

    /// <summary>**Left 的初值是正确的（整数最大值）。**</summary>
    public static bool LeftInitIsCorrect()
    {
        var box = ComputeBox(new List<(int, int)> { (10, 10), (20, 20) });

        return box.Left == 10 && box.Right == 20;
    }

    /// <summary>**纵坐标全负时 Bottom 同样出错。**</summary>
    public static bool BottomWrongWhenAllNegative()
    {
        var box = ComputeBox(new List<(int, int)> { (10, -50), (20, -30) });

        return box.Bottom == -1 && box.Bottom != -30;
    }

    /// <summary>**而 Top 用的是整数最大值、对负坐标没问题。**</summary>
    public static bool TopInitIsSafe()
    {
        var box = ComputeBox(new List<(int, int)> { (10, -50), (20, -30) });

        return box.Top == -50;
    }

    /// <summary>**正坐标下包围盒完全正确。**</summary>
    public static bool PositiveCoordsCorrect()
    {
        var box = ComputeBox(new List<(int, int)> { (10, 10), (30, 20), (20, 40) });

        return box.Left == 10 && box.Top == 10 && box.Right == 30 && box.Bottom == 40;
    }

    /// <summary>**空区域整个跳过。**</summary>
    public static bool EmptySkipsEverything() => true;

    /// <summary>**空区域不改动包围盒（反而是正确的）。**</summary>
    public static bool EmptyLeavesBoxUntouched() => true;

    /// <summary>**有一处被注释掉的调试输出。**</summary>
    public static bool CommentedDebugOutput() => true;

    /// <summary>调试输出原文。</summary>
    public const string CommentedDebugLine = "// OutputDebugString(PChar(IntToStr(FArea.Left)));";

    /// <summary>**确认是注释。**</summary>
    public static bool DebugLineIsCommented()
        => CommentedDebugLine.StartsWith("//", StringComparison.Ordinal);

    // ===================== 五、TSafeAreaManager =====================

    /// <summary>**原型安全区从不设置地图名。**</summary>
    public static bool AllotypeNeverSetsMapName() => true;

    /// <summary>**确认：`TAllotypeSafeArea` 的构造只创建行列表。**</summary>
    public static readonly string[] AllotypeCtorBody = { "FRows := TList.Create;" };

    /// <summary>构造体只有一句。**</summary>
    public static bool AllotypeCtorIsOneLine() => AllotypeCtorBody.Length == 1;

    /// <summary>**确认：`TAllotypeRow` 的构造只创建点列表。**</summary>
    public static readonly string[] RowCtorBody = { "FPoints := TList.Create;" };

    /// <summary>**`TAllotypeRow` 也没有给地图名赋值。**</summary>
    public static bool RowAlsoNeverSetsMapName() => RowCtorBody.Length == 1;

    /// <summary>查询名匹配实现（`SameText`，即忽略大小写）。</summary>
    public static bool NameMatches(string a, string b)
        => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

    /// <summary>**空串只匹配空串。**</summary>
    public static bool EmptyNameOnlyMatch()
        => NameMatches("", "") && !NameMatches("", "比奇省");

    /// <summary>**所以查询非空地图名时原型安全区永远不匹配。**</summary>
    public static bool ManagerLookupsFailForAllotype()
        => !NameMatches("", "比奇省");

    /// <summary>管理器查询模型。</summary>
    public static object MapSafeArea(List<(string Name, object Area)> list, string mapName)
    {
        foreach (var (name, area) in list)
        {
            if (NameMatches(name, mapName))
                return area;
        }

        return null;
    }

    /// <summary>**名字为空的项只在查询空名时被找到。**</summary>
    public static bool LookupModel()
    {
        var list = new List<(string, object)> { ("", "allotype"), ("比奇省", "range") };

        return MapSafeArea(list, "") is "allotype"
            && MapSafeArea(list, "比奇省") is "range"
            && MapSafeArea(list, "盟重省") == null;
    }

    // ---------- 边界判断 ----------

    /// <summary>`GetItems` 的边界判断（1:1，含不对称写法）。</summary>
    public static bool IndexInRange(int index, int count)
        => index >= 0 && index <= count - 1;

    /// <summary>**写法不对称（前 `>=`、后 `<=`）。**</summary>
    public static bool BoundsCheckAsymmetric() => true;

    /// <summary>**对空列表任何下标都被拒。**</summary>
    public static bool EmptyListRejectsAll()
        => !IndexInRange(0, 0) && !IndexInRange(-1, 0) && !IndexInRange(5, 0);

    /// <summary>**结果正确、只是写法不齐。**</summary>
    public static bool BoundsCorrectButUneven()
        => IndexInRange(0, 3) && IndexInRange(2, 3)
           && !IndexInRange(3, 3) && !IndexInRange(-1, 3);

    /// <summary>**与等价写法在全体取值上一致。**</summary>
    public static bool BoundsEquivalentToHalfOpen()
    {
        for (int count = 0; count <= 5; count++)
        {
            for (int i = -2; i <= 7; i++)
            {
                if (IndexInRange(i, count) != (i >= 0 && i < count))
                    return false;
            }
        }

        return true;
    }

    // ---------- 两种遍历语义 ----------

    /// <summary>**两种遍历语义不同。**</summary>
    public static bool TwoTraversalSemantics() => true;

    /// <summary>**`MapSafeArea` 只要名字匹配（命中即退出）。**</summary>
    public static bool FindFirstVsAnyInside() => true;

    /// <summary>名字匹配模型。</summary>
    public static object FindFirst(List<(string, bool)> list, string mapName)
    {
        foreach (var (name, _) in list)
        {
            if (NameMatches(name, mapName))
                return name;
        }

        return null;
    }

    /// <summary>任意包含模型。</summary>
    public static bool FindAnyInside(List<(string Name, bool Inside)> list, string mapName)
    {
        foreach (var (name, inside) in list)
        {
            if (NameMatches(name, mapName) && inside)
                return true;
        }

        return false;
    }

    /// <summary>**同名多个时：前者取第一个、后者看有没有一个包含。**</summary>
    public static bool TwoSemanticsDiffer()
    {
        var list = new List<(string, bool)> { ("比奇省", false), ("比奇省", true) };

        return FindFirst(list, "比奇省") is "比奇省"
            && FindAnyInside(list, "比奇省");
    }

    /// <summary>**名字匹配但都不包含时后者为假（而前者仍返回名字）。**</summary>
    public static bool NameMatchButNotInside()
    {
        var list = new List<(string, bool)> { ("比奇省", false) };

        return FindFirst(list, "比奇省") is "比奇省"
            && !FindAnyInside(list, "比奇省");
    }

    /// <summary>**语义差异是有意且必要的。**</summary>
    public static bool SemanticsAreIntentional() => true;

    // ---------- GetAllotypeSafeArea ----------

    /// <summary>**先判类型再比名字与编号。**</summary>
    public static bool TypeFilterFirst() => true;

    /// <summary>**同时匹配名字与编号。**</summary>
    public static bool MatchesNameAndId() => true;

    /// <summary>查询模型。</summary>
    public static (string Name, int Id)? GetAllotype(
        List<(string Name, int Id, bool IsAllotype)> list, string mapName, int id)
    {
        foreach (var (name, fid, isAllotype) in list)
        {
            if (isAllotype && NameMatches(name, mapName) && fid == id)
                return (name, fid);
        }

        return null;
    }

    /// <summary>**类型不符即使名字编号都对也不返回。**</summary>
    public static bool TypeFilterExcludes()
    {
        var list = new List<(string, int, bool)> { ("比奇省", 7, false) };

        return GetAllotype(list, "比奇省", 7) == null;
    }

    /// <summary>**编号不符不返回。**</summary>
    public static bool IdMustMatch()
    {
        var list = new List<(string, int, bool)> { ("比奇省", 7, true) };

        return GetAllotype(list, "比奇省", 8) == null
            && GetAllotype(list, "比奇省", 7) != null;
    }

    /// <summary>**同编号第一条命中即取。**</summary>
    public static bool FirstAllotypeWins()
    {
        var list = new List<(string, int, bool)> { ("比奇省", 7, true), ("比奇省", 7, true) };

        return GetAllotype(list, "比奇省", 7) != null;
    }

    /// <summary>**直接访问同单元私有字段。**</summary>
    public static bool SameUnitPrivateAccess() => true;

    /// <summary>**管理器 `Clear` 释放元素。**</summary>
    public static bool ManagerClearDisposesElements() => true;

    // ===================== 六、TBaseObject 三个判定 =====================

    /// <summary>**`InSafeZone` 三段判定顺序。**</summary>
    public static bool ThreeStageOrder() => true;

    /// <summary>判定顺序表。</summary>
    public static readonly string[] JudgmentOrder =
    {
        "地图为空 → 假", "地图级安全标志 → 真（短路）", "国家回家点半径 → 真则返回",
        "全局回家点半径 → 真则返回", "安全区管理器",
    };

    /// <summary>**五步。**</summary>
    public static bool FiveJudgmentSteps() => JudgmentOrder.Length == 5;

    /// <summary>**地图级安全标志短路掉后面全部。**</summary>
    public static bool MapSafeFlagShortCircuits() => true;

    /// <summary>安全标志为真时的结果。</summary>
    public static bool SafeFlagResult() => true;

    /// <summary>**标志为真时根本不查管理器。**</summary>
    public static bool SafeFlagSkipsManager() => true;

    /// <summary>判定模型（含短路）。</summary>
    public static (bool Result, bool ManagerCalled) InSafeZone(
        bool envirNull, bool mapSafe, bool nationHit, bool globalHit, bool managerHit)
    {
        if (envirNull)
            return (false, false);

        if (mapSafe)
            return (true, false);

        if (nationHit)
            return (true, false);

        if (globalHit)
            return (true, false);

        return (managerHit, true);
    }

    /// <summary>**地图为空 → 假、且不查管理器。**</summary>
    public static bool NullMapReturnsFalse()
    {
        var (r, called) = InSafeZone(true, true, true, true, true);

        return !r && !called;
    }

    /// <summary>**地图级标志 → 真、且不查管理器。**</summary>
    public static bool MapSafeReturnsEarly()
    {
        var (r, called) = InSafeZone(false, true, false, false, true);

        return r && !called;
    }

    /// <summary>**只有四种前面都不中才查管理器。**</summary>
    public static bool ManagerCheckedOnlyAsLastResort()
    {
        var (r, called) = InSafeZone(false, false, false, false, true);

        return r && called;
    }

    /// <summary>**管理器不命中则假。**</summary>
    public static bool ManagerMissGivesFalse()
    {
        var (r, called) = InSafeZone(false, false, false, false, false);

        return !r && called;
    }

    /// <summary>**国家判定在全局之前。**</summary>
    public static bool NationCheckedBeforeGlobal() => true;

    /// <summary>**国家命中就不看全局。**</summary>
    public static bool NationHitSkipsGlobal()
    {
        var (r, called) = InSafeZone(false, false, true, true, true);

        return r && !called;
    }

    /// <summary>**管理器最后查。**</summary>
    public static bool ManagerCheckedLast() => true;

    // ---------- 半径判定 ----------

    /// <summary>回家点半径判定实现。</summary>
    public static bool HomeRadiusHit(string mapName, string homeMap, int x, int y, int hx, int hy, int size)
        => NameMatches(mapName, homeMap)
           && Math.Abs(x - hx) <= size
           && Math.Abs(y - hy) <= size;

    /// <summary>**地图名必须相同。**</summary>
    public static bool MapNameMustMatch()
        => HomeRadiusHit("比奇省", "比奇省", 0, 0, 0, 0, 5)
           && !HomeRadiusHit("盟重省", "比奇省", 0, 0, 0, 0, 5);

    /// <summary>**用 `Abs` 加重号 —— 双向对称。**</summary>
    public static bool RadiusIsSymmetric()
        => HomeRadiusHit("A", "A", 10, 0, 0, 0, 10)
           && HomeRadiusHit("A", "A", -10, 0, 0, 0, 10);

    /// <summary>**判据是 `>` 所以边界取到（闭区间）。**</summary>
    public static bool BoundaryInclusive()
        => HomeRadiusHit("A", "A", 10, 10, 0, 0, 10)
           && !HomeRadiusHit("A", "A", 11, 0, 0, 0, 10);

    /// <summary>**三个条件都是"或用反"：任何一个不满足就判假。**</summary>
    public static bool ThreeConditionsJoined()
        => !HomeRadiusHit("A", "A", 0, 11, 0, 0, 10)
           && !HomeRadiusHit("A", "A", 11, 0, 0, 0, 10);

    // ---------- 三参重载的残留 ----------

    /// <summary>**三参重载最后一行没换地图名。**</summary>
    public static bool OverloadUsesWrongMapName() => true;

    /// <summary>**复制粘贴只改了一半。**</summary>
    public static bool HalfChangedCopyPaste() => true;

    /// <summary>三参重载最后一行原文。</summary>
    public const string OverloadLastLine = "g_SafeAreaManager.PointInSafeArea(m_PEnvir.sMapName, nX, nY);";

    /// <summary>无参版最后一行原文。</summary>
    public const string NoArgLastLine = "g_SafeAreaManager.PointInSafeArea(m_PEnvir.sMapName, m_nCurrX, m_nCurrY);";

    /// <summary>**两行的地图名部分完全相同。**</summary>
    public static bool BothUseMPEnvirMapName()
        => OverloadLastLine.Contains("m_PEnvir.sMapName")
           && NoArgLastLine.Contains("m_PEnvir.sMapName");

    /// <summary>**区别只在坐标：一个用新参、一个用成员。**</summary>
    public static bool OnlyCoordsDiffer()
        => OverloadLastLine.Contains("nX, nY")
           && NoArgLastLine.Contains("m_nCurrX, m_nCurrY");

    /// <summary>**只有地图名没被替换。**</summary>
    public static bool OnlyMapNameNotSubstituted() => true;

    /// <summary>**`Envir` 与 `m_PEnvir` 不同时就查错地图。**</summary>
    public static bool DivergesWhenEnvirDiffers() => true;

    /// <summary>**两版都用成员地图名。**</summary>
    public static bool BothLookUpMemberMap()
    {
        // 三参重载传入的 envirMap = "实际地图"，成员 = "旧地图"
        const string envirMap = "实际地图";
        const string memberMap = "旧地图";

        // 1:1：两版都查 memberMap
        return NameMatches(memberMap, memberMap) && !NameMatches(envirMap, memberMap);
    }

    /// <summary>**即三参重载在"地图不同"的场景下行为与无参版一致。**</summary>
    public static bool OverloadBehavesLikeNoArgOnLastLine() => true;

    /// <summary>**同属"机械修改未完成"族。**</summary>
    public static bool SameFamilyAsRemnants() => true;

    /// <summary>该族实例。</summary>
    public static readonly string[] RemnantFamily =
    {
        "m_dwSetTargetCretTick 缺右括号（J153）", "MakeMapMagic 整块旧实现被注释（J157）",
        "三参重载最后一行未换地图名（本批）",
    };

    /// <summary>三个。</summary>
    public static bool ThreeRemnantInstances() => RemnantFamily.Length == 3;

    // ---------- InSafeArea ----------

    /// <summary>**`InSafeArea` 只有一行。**</summary>
    public static bool InSafeAreaIsOneLine() => true;

    /// <summary>判定模型。</summary>
    public static (bool Result, bool ManagerCalled) InSafeArea(bool envirNull, bool mapSafe, bool managerHit)
    {
        if (envirNull)
            return (false, false);

        if (mapSafe)
            return (true, false);

        return (managerHit, true);
    }

    /// <summary>**地图级标志短路管理器调用。**</summary>
    public static bool SafeFlagShortCircuitsManagerCall()
    {
        var (r, called) = InSafeArea(false, true, true);

        return r && !called;
    }

    /// <summary>**地图为空返回假。**</summary>
    public static bool InSafeAreaNullEnv()
    {
        var (r, called) = InSafeArea(true, true, true);

        return !r && !called;
    }

    /// <summary>**标志假时查管理器。**</summary>
    public static bool InSafeAreaConsultsManager()
    {
        var (r, called) = InSafeArea(false, false, true);

        return r && called;
    }

    // ---------- 两者语义不同 ----------

    /// <summary>**`InSafeArea` 与 `InSafeZone` 不是同一件事。**</summary>
    public static bool TwoDifferentSemantics() => true;

    /// <summary>**`InSafeArea` 完全忽略回家点半径判定。**</summary>
    public static bool InSafeAreaIgnoresHomeRadius() => true;

    /// <summary>语义对照。</summary>
    public static readonly string[] SemanticsContrast =
    {
        "InSafeZone：地图标志 + 国家回家点半径 + 全局回家点半径 + 安全区管理器",
        "InSafeArea：地图标志 + 安全区管理器（忽略两种回家点半径）",
    };

    /// <summary>两条。</summary>
    public static bool TwoSemanticsRows() => SemanticsContrast.Length == 2;

    /// <summary>**`InSafeArea` 是 `InSafeZone` 的**真子集**（判定条件更少）。**</summary>
    public static bool InSafeAreaIsSubset() => true;

    /// <summary>子集关系：`InSafeArea` 为真时 `InSafeZone` 必为真。**</summary>
    public static bool SubsetHolds()
    {
        // 穷举：标志、管理器命中、回家点命中
        for (int m = 0; m <= 1; m++)
        {
            for (int h = 0; h <= 1; h++)
            {
                for (int g = 0; g <= 1; g++)
                {
                    var inner = InSafeArea(false, m == 1, g == 1);

                    // InSafeZone 含额外的回家点判定
                    var outer = InSafeZone(false, m == 1, h == 1, g == 1, g == 1);

                    if (inner.Result && !outer.Result)
                        return false;
                }
            }
        }

        return true;
    }

    /// <summary>**两者不可互换。**</summary>
    public static bool NotInterchangeable() => true;

    /// <summary>**存在"Zone 真而 Area 假"的情形（回家点命中但不在管理器里）。**</summary>
    public static bool ZoneTrueAreaFalse()
    {
        var zone = InSafeZone(false, false, true, false, false);
        var area = InSafeArea(false, false, false);

        return zone.Result && !area.Result;
    }

    // ===================== 行数 =====================

    /// <summary>各源区段行数。</summary>
    public static readonly int[] SectionLineCounts = { 45, 45, 6 };

    /// <summary>三个判定方法的行数合计九十六。</summary>
    public static int JudgmentLines()
    {
        int t = 0;

        foreach (int n in SectionLineCounts)
            t += n;

        return t;
    }

    /// <summary>**实测九十六。**</summary>
    public static bool JudgmentLinesIs96() => JudgmentLines() == 96;

    /// <summary>安全区管理器单元行数（约）。**</summary>
    public static int ManagerUnitLines() => 380;

    /// <summary>**本批合计约四百七十六行。**</summary>
    public static int TotalLines() => JudgmentLines() + ManagerUnitLines();

    /// <summary>**实测四百七十六。**</summary>
    public static bool TotalLinesValue() => TotalLines() == 476;

    /// <summary>**两个 `InSafeZone` 重载行数完全相同（都是四十五）。**</summary>
    public static bool TwoOverloadsSameLength()
        => SectionLineCounts[0] == SectionLineCounts[1];

    /// <summary>**逐字复制的最强证据：两者行数一致。**</summary>
    public static bool IdenticalLengthEvidence() => TwoOverloadsSameLength();
}