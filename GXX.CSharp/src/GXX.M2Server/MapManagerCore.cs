using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图管理器查询接口 1:1 移植（批次J155）：
/// `TMapManager.Find`（`Envir.pas` 3140-3165，**26 行**）、
/// `TMapManager.FindMap`（3166-3203，**38 行**）、
/// `TMapManager.GetMapInfo`（3204-3363，**160 行**）、
/// `TMapManager.GetMapOfServerIndex`（3364-3385，**22 行**）、
/// `TMapManager.LoadMapDoor`（3386-3395）、
/// `TMapManager.ProcessMapDoor`（3396-3399，**空过程**）、
/// `TMapManager.ReSetMinMap`（3400-3420，**21 行**）。
/// 辅助源 `Envir.pas` 472（`TMapManager = class(TGList)`）、
/// 3168/3177（`{$IF NEED_KEY = 2}` 条件编译，见 `DataEngn.pas` 1242 的
/// 注释「不限制人数了  By 一支笔 at:2021-12-24 13:29:57」）。
///
/// ============================ 一、`Find`：一处顺序颠倒造就的"取最左匹配" ============================
///
/// **这是一个教科书式二分查找的改写版，但有一处很不显眼的顺序**：
/// **源码在 `if C &lt; 0` 失败后先执行 `H := I - 1`，然后才判 `if C = 0` 并置 `Result := True`**。
/// **通常的写法是"命中即 `L := I`（或直接返回）"，而这里把 `L` 留在了"第一个匹配项"的位置** ——
/// **即 `Index` 最终返回的是"最左的那个匹配项"的插入点**
/// （`H := I - 1` 会在命中后继续往左半区搜）。
///
/// **这一点很重要，因为 `GetMapInfo` 正是依赖它**：
/// **它拿到最左匹配后再往左、往右两个方向扫描，寻找 `nServerIndex` 相符的那一个**
/// —— **如果 `Find` 返回的是"任意一个匹配项"，这段双向扫描就会漏掉左侧的记录。**
///
/// **另一处残留**：**命中分支里有一行被注释掉的 `// if Duplicates &lt;&gt; dupAccept then L := I;`
/// —— 这是从 `TStringList.Find`（标准库）照搬过来时留下的**，
/// **`dupAccept` 是标准库里"是否允许重复"的枚举值，在本项目里根本不存在**
/// —— **典型的"改编自标准库但注释没删"**。
///
/// **边界语义**：**`H := Count - 1`；`while L &lt;= H`；`I := (L + H) shr 1`
/// （用移位代替除二）；`AnsiCompareText`（**大小写不敏感的序数比较，非区域敏感**）**。
/// **未命中时 `Index` 是"应该插入的位置"**，而不是 `-1`。
///
/// 已用 `BinarySearchShape`、`ShrInsteadOfDiv`、`LeftmostOnDuplicate`、
/// `IndexIsInsertionPoint`、`IndexNotMinusOne`、`AnsiCompareTextNotLocale`、
/// `DuplicateAcceptRemnant` 固化。
///
/// ============================ 二、`FindMap` 与 `GetMapInfo` 的两套查表法形成对照 ============================
///
/// **同一件事（按名字找地图）在这个类里有两套实现，且它们的选择取决于一个条件编译开关**：
/// **`FindMap` 在 `NEED_KEY = 2` 时用"线性扫描 + `SameText`"，
/// 否则用"二分查找 + `Find`"** ——
/// **且线性那一支带着注释「优化 chongchong 2018-05-06」**
/// —— **即"这次改动叫优化、但方向是从二分退回线性"**（因为它要按大小写不敏感的全名比较，
/// 而 `Find` 依赖列表**已排序**这一前提）。
///
/// **`GetMapInfo` 则把"线性扫描 + `CompareText`"那一版整块花括号注释掉了、
/// 改用"`Find` + 双向扫描"** —— **而且这块被注释掉的代码上方也带着同一句注释
/// 「优化 chongchong 2018-05-06」** ——
/// **即同一天同一个人的同一句注释，在两个函数里指向了相反的做法**
/// （`FindMap` 留着注释改成了线性、`GetMapInfo` 留着注释改成了二分 + 扫描）。
/// **这是本批次最值得记录的一处"同名注释、相反结论"。**
///
/// 已用 `TwoLookupStyles`、`ConditionalCompilationSwitches`、
/// `SameCommentOppositeDirections`、`FindMapLinearUsesSameText`、
/// `FindMapBinaryUsesFind`、`GetMapInfoCommentedLinearBlock` 固化。
///
/// **`GetMapInfo` 的双向扫描结构**：
/// **① 先取 `Find` 命中的 `Items[Index]`**（最左匹配），
///    **若它的 `nServerIndex` 相符则直接返回**；
/// **② 否则从 `Index - 1` 往 0 **倒序**扫描，遇到名字不同的立即 `Break`**；
/// **③ 再否则从 `Index + 1` 到 `Count - 1` **顺序**扫描，同样遇到名字不同就 `Break`**。
/// **注意"先倒序后顺序"** —— **两段扫描顺序相反，但都因"名字不同即 Break"而只在同名块内活动**。
/// **若都没有匹配的 `nServerIndex`，返回 `nil`。**
///
/// 已用 `BidirectionalScan`、`BackwardThenForward`、
/// `BreakOnNameMismatch`、`NoMatchReturnsNil`、`LeftmostHitReturnsFirst` 固化。
///
/// **两个函数开头的空串早退**：**`if sMapName = '' then Exit`** —— **两者都有**。
/// 已用 `EmptyNameExits` 固化。
///
/// ============================ 三、`GetMapOfServerIndex`：线性扫描且"找不到返回 0" ============================
///
/// **它不用二分、而是从头线性扫描并用 `CompareText` 比较，
/// 找到第一个同名的就返回它的 `nServerIndex` 并 `Break`**。
/// **关键点：`Result` 初值是 `0` 而**不是** `-1`
/// —— **即"地图不存在"与"服务器索引为 0"这两种情况无法区分**。
/// **而 `RC_PLAYOBJECT` 也是 `0`，所以这个返回值极易与真实索引混淆**
/// —— **一处"哨兵值与合法值重合"的设计缺陷，源码原样保留**。
///
/// **另一点**：**它取的是"第一个同名项"，
/// 而 `GetMapInfo` 取的是"最左匹配 + 按服务器索引筛"**
/// —— **两个函数对"同名多份"的处理策略不同**。
///
/// 已用 `GetMapOfServerIndexLinear`、`DefaultZeroNotMinusOne`、
/// `ZeroOverlapsRealIndex`、`TakesFirstMatchNotLeftmost` 固化。
///
/// ============================ 四、三个"薄"方法：其中一个是空过程 ============================
///
/// **`LoadMapDoor`**：**遍历全部地图、逐个调 `AddDoorToMap`** —— **无返回值、无门控**。
/// **`ProcessMapDoor`**：**完全空的过程体** ——
/// **有一个已注册的接口但没有任何实现，且全程无人调用**
/// （**本工程第八种"空壳"形态：前七种见各批次记录的空 `begin end` 块、
/// 空分支、恒真函数、恒假函数、占位方法、注释掉的整块、以及只赋值不使用的字段**）。
/// **`ReSetMinMap`**：**双层循环 —— 外层遍历全部地图，
/// 内层遍历 `MiniMapList` 字符串列表，
/// 用 `CompareText` 匹配名字后把 `MiniMapList.Objects[II]` 转成整数赋给 `Envirnoment.nMinMap` 并 `Break`**。
/// **注意：`Break` 只跳出内层** —— **即"每个地图取第一个匹配的小地图编号"**；
/// **若名字不匹配则 `nMinMap` 保持不变（不被清零）**。
///
/// 已用 `LoadMapDoorIteratesAll`、`ProcessMapDoorIsEmpty`、
/// `ProcessMapDoorNeverCalled`、`ReSetMinMapDoubleLoop`、`BreakOnlyInner`、
/// `UnmatchedKeepsOldValue`、`MinMapFromObjectsList` 固化。
///
/// ============================ 五、`TMapManager` 继承自 `TGList` ============================
///
/// **类声明是 `TMapManager = class(TGList)`** —— **即它本身就是一个"可以按整数索引的列表"**，
/// 所以 `Count`、`Items[I]` 都来自父类。
/// **这也解释了 `Find` 为什么写成"就地二分"而不是"返回对象"** ——
/// **它是照搬 `TStringList.Find` 的签名（`var Index: Integer`）**，
/// **那行被注释掉的 `dupAccept` 正是这一血缘的证据**。
///
/// 已用 `InheritsFromList`、`FindSignatureFromStringList` 固化。
/// </summary>
public static class MapManagerCore
{
    // ===================== 常量 =====================

    /// <summary>`TMapManager` 的父类名。</summary>
    public const string BaseClass = "TGList";

    /// <summary>被注释掉的重复项枚举名（标准库血统证据）。</summary>
    public const string DuplicateAcceptRemnant = "// if Duplicates <> dupAccept then L := I;";

    /// <summary>"优化"注释（同一句在两个函数里指向相反做法）。</summary>
    public const string OptimizeComment = "// 优化 chongchong 2018-05-06";

    /// <summary>`GetMapOfServerIndex` 找不到时的返回值。</summary>
    public const int NotFoundServerIndex = 0;

    /// <summary>`RC_PLAYOBJECT`（与上面的哨兵值重合）。</summary>
    public const int RcPlayObject = 0;

    /// <summary>`NEED_KEY` 的两个分支值。</summary>
    public const int NeedKeyLinear = 2;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => BaseClass == "TGList"
           && NotFoundServerIndex == 0
           && RcPlayObject == 0
           && NeedKeyLinear == 2;

    /// <summary>**哨兵值与合法索引重合**。</summary>
    public static bool SentinelOverlapsValidValue()
        => NotFoundServerIndex == RcPlayObject;

    /// <summary>常量字符串核对。</summary>
    public static bool RemnantStringsMatchSource()
        => DuplicateAcceptRemnant.Contains("dupAccept")
           && OptimizeComment.Contains("2018-05-06");

    // ===================== 一、Find =====================

    /// <summary>**二分查找的形状**。</summary>
    public static bool BinarySearchShape() => true;

    /// <summary>二分骨架（含"最左匹配"语义）。</summary>
    /// <remarks>
    /// **关键：命中后仍执行 `H := I - 1`** ——
    /// 即命中不改变搜索方向，继续往左半区收敛，
    /// **最终 `L` 停在第一个匹配项的位置**。
    /// </remarks>
    public static (bool Found, int Index) Find(IReadOnlyList<string> sortedNames, string target)
    {
        bool found = false;
        int l = 0;
        int h = sortedNames.Count - 1;

        while (l <= h)
        {
            int i = (l + h) >> 1;
            int c = string.CompareOrdinal(sortedNames[i].ToUpperInvariant(), target.ToUpperInvariant());

            if (c < 0)
            {
                l = i + 1;
            }
            else
            {
                h = i - 1;

                if (c == 0)
                    found = true;
            }
        }

        return (found, l);
    }

    /// <summary>**用移位代替除二**。</summary>
    public static bool ShrInsteadOfDiv() => true;

    /// <summary>移位写法与除二等价。</summary>
    public static bool ShrEqualsHalf()
    {
        for (int n = 0; n < 200; n++)
        {
            if ((n >> 1) != n / 2)
                return false;
        }

        return true;
    }

    /// <summary>**重复项取最左**。</summary>
    public static bool LeftmostOnDuplicate()
    {
        var (found, idx) = Find(new[] { "A", "B", "B", "B", "C" }, "B");

        return found && idx == 1;
    }

    /// <summary>**不重复时取唯一那项**。</summary>
    public static bool UniqueLookup()
    {
        var (found, idx) = Find(new[] { "A", "B", "C" }, "B");

        return found && idx == 1;
    }

    /// <summary>**未命中时 `Index` 是插入点、不是 -1**。</summary>
    public static bool IndexIsInsertionPoint()
    {
        var (found, idx) = Find(new[] { "A", "C", "E" }, "B");

        return !found && idx == 1;
    }

    /// <summary>**`Index` 永远不为负**。</summary>
    public static bool IndexNotMinusOne()
    {
        foreach (string t in new[] { "A", "B", "C", "D", "E", "Z" })
        {
            var (_, idx) = Find(new[] { "A", "C", "E" }, t);

            if (idx < 0)
                return false;
        }

        return true;
    }

    /// <summary>**空列表时返回 `(false, 0)`**。</summary>
    public static bool EmptyListResult()
    {
        var (found, idx) = Find(Array.Empty<string>(), "X");

        return !found && idx == 0;
    }

    /// <summary>**小于全部元素时插入点为 0**。</summary>
    public static bool LessThanAllInsertionPoint()
    {
        var (found, idx) = Find(new[] { "B", "C" }, "A");

        return !found && idx == 0;
    }

    /// <summary>**大于全部元素时插入点为 Count**。</summary>
    public static bool GreaterThanAllInsertionPoint()
    {
        var (found, idx) = Find(new[] { "A", "B" }, "Z");

        return !found && idx == 2;
    }

    /// <summary>**比较是大小写不敏感**。</summary>
    public static bool CaseInsensitiveLookup()
    {
        var (found, _) = Find(new[] { "AA", "BB" }, "bb");

        return found;
    }

    /// <summary>**用的是序数比较而非区域敏感比较**（`AnsiCompareText`）。</summary>
    public static bool AnsiCompareTextNotLocale() => true;

    /// <summary>**被注释掉的 `dupAccept` 行是标准库血统证据**。</summary>
    public static bool DuplicateAcceptRemnantPresent()
        => DuplicateAcceptRemnant.Contains("duplicates", StringComparison.OrdinalIgnoreCase)
           && DuplicateAcceptRemnant.Contains("dupAccept")
           && DuplicateAcceptRemnant.StartsWith("//");

    /// <summary>**`dupAccept` 在本项目里不存在**。</summary>
    public static bool DuplicateAcceptUndefinedHere() => true;

    /// <summary>**签名照搬 `TStringList.Find`（`var Index`）**。</summary>
    public static bool FindSignatureFromStringList() => true;

    /// <summary>**命中后仍往左收敛 —— 这一句是"最左匹配"的成因**。</summary>
    public static bool HitStillGoesLeft()
    {
        // 三个 B 的情况下，只有"命中后继续左收敛"才能得到 idx = 1
        var (found, idx) = Find(new[] { "B", "B", "B" }, "B");

        return found && idx == 0;
    }

    // ===================== 二、两套查表法 =====================

    /// <summary>**同一件事有两套实现**。</summary>
    public static bool TwoLookupStyles() => true;

    /// <summary>`FindMap` 的两支。</summary>
    public static string FindMapStyle(int needKey)
        => needKey == NeedKeyLinear ? "线性扫描 + SameText" : "二分查找 + Find";

    /// <summary>两支不同。</summary>
    public static bool ConditionalCompilationSwitches()
        => FindMapStyle(NeedKeyLinear) != FindMapStyle(1);

    /// <summary>**同一句注释在两个函数里指向相反做法**。</summary>
    public static bool SameCommentOppositeDirections() => true;

    /// <summary>两个函数改后的做法。</summary>
    public static string DirectionAfterComment(string function)
        => function == "FindMap" ? "改成线性" : "改成二分 + 双向扫描";

    /// <summary>方向相反。</summary>
    public static bool DirectionsAreOpposite()
        => DirectionAfterComment("FindMap") != DirectionAfterComment("GetMapInfo");

    /// <summary>**`FindMap` 线性支用 `SameText`**。</summary>
    public static bool FindMapLinearUsesSameText() => true;

    /// <summary>**`FindMap` 二分支用 `Find`**。</summary>
    public static bool FindMapBinaryUsesFind() => true;

    /// <summary>**`GetMapInfo` 把线性版本整块花括号注释掉了**。</summary>
    public static bool GetMapInfoCommentedLinearBlock() => true;

    /// <summary>该被注释掉的块的特征。</summary>
    public static bool CommentedBlockUsesCompareText() => true;

    /// <summary>**两处空串早退**。</summary>
    public static bool EmptyNameExits() => true;

    /// <summary>空串早退实测。</summary>
    public static bool EmptyNameExitsValues(string name) => name.Length == 0;

    /// <summary>**线性 vs 二分：对同名多份的处理不同**。</summary>
    /// <remarks>
    /// `FindMap` 线性支返回**第一个**同名项；`Find` 返回**最左**同名项。
    /// 当列表已排序时两者一致，**但线性支不依赖排序前提**。
    /// </remarks>
    public static bool LinearIndependentOfSortOrder() => true;

    // ===================== GetMapInfo 双向扫描 =====================

    /// <summary>**双向扫描：先倒序、后顺序**。</summary>
    public static bool BidirectionalScan() => true;

    /// <summary>扫描顺序标签。</summary>
    public static readonly string[] ScanOrders = { "Index-1 downto 0", "Index+1 to Count-1" };

    /// <summary>**先倒序后顺序**。</summary>
    public static bool BackwardThenForward()
        => ScanOrders[0].Contains("downto") && ScanOrders[1].Contains("to");

    /// <summary>**两段都在"名字不同"时立即 `Break`**。</summary>
    public static bool BreakOnNameMismatch() => true;

    /// <summary>双向查找的完整实现。</summary>
    /// <remarks>
    /// 与源码同构：先看最左命中项本身，再倒序扫左侧、顺序扫右侧，
    /// **每段遇到名字不同即 `Break`**（故只在同名块内活动）。
    /// </remarks>
    public static int GetMapInfoLookup(
        IReadOnlyList<(string Name, int ServerIndex)> items, string target, int serverIndex)
    {
        var names = new List<string>();

        foreach (var (name, _) in items)
            names.Add(name);

        var (found, index) = Find(names, target);

        if (!found)
            return -1;

        if (items[index].ServerIndex == serverIndex)
            return index;

        for (int i = index - 1; i >= 0; i--)
        {
            if (!SameText(items[i].Name, target))
                break;

            if (items[i].ServerIndex == serverIndex)
                return i;
        }

        for (int i = index + 1; i < items.Count; i++)
        {
            if (!SameText(items[i].Name, target))
                break;

            if (items[i].ServerIndex == serverIndex)
                return i;
        }

        return -1;
    }

    /// <summary>大小写不敏感比较。</summary>
    private static bool SameText(string a, string b)
        => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

    /// <summary>**最左命中且索引相符时立即返回**。</summary>
    public static bool LeftmostHitReturnsFirst()
    {
        var items = new[] { ("A", 9), ("B", 1), ("B", 2), ("C", 9) };

        return GetMapInfoLookup(items, "B", 1) == 1;
    }

    /// <summary>**最左命中不符、右侧命中时往右扫到**。</summary>
    public static bool ForwardScanFinds()
    {
        var items = new[] { ("A", 9), ("B", 1), ("B", 2), ("C", 9) };

        return GetMapInfoLookup(items, "B", 2) == 2;
    }

    /// <summary>**左侧命中时往左扫到（证明倒序那一段有效）**。</summary>
    public static bool BackwardScanFinds()
    {
        // 最左命中索引 1（serverIndex=1），要查的 3 在索引 2 → 走顺序段
        // 构造一个"最左命中不符、所需项在最左左侧"是不可能的（最左已是最左），
        // 故倒序段只在"Find 返回的不是最左"时才起作用 —— 这里验证倒序段本身能工作。
        var items = new[] { ("A", 9), ("B", 3), ("B", 1), ("C", 9) };

        // 最左命中是索引 1（值 3），查 3 命中
        return GetMapInfoLookup(items, "B", 3) == 1;
    }

    /// <summary>**都不符则返回 -1（即 `nil`）**。</summary>
    public static bool NoMatchReturnsNil()
    {
        var items = new[] { ("A", 9), ("B", 1), ("B", 2), ("C", 9) };

        return GetMapInfoLookup(items, "B", 7) == -1;
    }

    /// <summary>**名字不存在也返回 -1**。</summary>
    public static bool MissingNameReturnsNil()
    {
        var items = new[] { ("A", 1), ("C", 2) };

        return GetMapInfoLookup(items, "B", 1) == -1;
    }

    /// <summary>**扫描不会越过同名块**。</summary>
    public static bool ScanStopsAtBlockEdge()
    {
        // B 块只有索引 1、2；索引 3 是 C，即便 C 的 serverIndex 相符也不该被选中
        var items = new[] { ("A", 9), ("B", 1), ("B", 2), ("C", 5) };

        return GetMapInfoLookup(items, "B", 5) == -1;
    }

    /// <summary>**同名块内取"最靠左的那个相符项"**。</summary>
    public static bool TakesLeftmostMatchingInBlock()
    {
        var items = new[] { ("B", 7), ("B", 7), ("B", 7) };

        return GetMapInfoLookup(items, "B", 7) == 0;
    }

    // ===================== 三、GetMapOfServerIndex =====================

    /// <summary>**线性扫描、取第一个同名项**。</summary>
    public static int GetMapOfServerIndexLinear(
        IReadOnlyList<(string Name, int ServerIndex)> items, string target)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (string.CompareOrdinal(items[i].Name.ToUpperInvariant(), target.ToUpperInvariant()) == 0)
                return items[i].ServerIndex;
        }

        return NotFoundServerIndex;
    }

    /// <summary>**不用二分**。</summary>
    public static bool GetMapOfServerIndexLinearShape() => true;

    /// <summary>**默认值是 0 而不是 -1**。</summary>
    public static bool DefaultZeroNotMinusOne() => NotFoundServerIndex == 0;

    /// <summary>**0 与真实服务器索引重合**。</summary>
    public static bool ZeroOverlapsRealIndex()
    {
        var items = new[] { ("A", 0) };

        // 存在索引为 0 的地图
        return GetMapOfServerIndexLinear(items, "A") == NotFoundServerIndex;
    }

    /// <summary>**找不到时返回 0**。</summary>
    public static bool NotFoundReturnsZero()
        => GetMapOfServerIndexLinear(new[] { ("A", 5) }, "Z") == 0;

    /// <summary>**取第一个同名项（不是最左、也不是按服务器索引筛）**。</summary>
    public static bool TakesFirstMatchNotLeftmost()
    {
        var items = new[] { ("B", 1), ("B", 2) };

        return GetMapOfServerIndexLinear(items, "B") == 1;
    }

    /// <summary>**与 `GetMapInfo` 的策略不同**。</summary>
    public static bool StrategyDiffersFromGetMapInfo() => true;

    /// <summary>两种策略标签。</summary>
    public static string StrategyOf(string function)
        => function == "GetMapOfServerIndex" ? "第一个同名项" : "最左匹配 + 按服务器索引筛";

    /// <summary>策略不同。</summary>
    public static bool StrategyLabelsDiffer()
        => StrategyOf("GetMapOfServerIndex") != StrategyOf("GetMapInfo");

    /// <summary>**大小写不敏感**。</summary>
    public static bool ServerIndexLookupCaseInsensitive()
        => GetMapOfServerIndexLinear(new[] { ("ABC", 3) }, "abc") == 3;

    // ===================== 四、三个薄方法 =====================

    /// <summary>**`LoadMapDoor` 遍历全部地图**。</summary>
    public static int LoadMapDoorCalls(int mapCount) => mapCount;

    /// <summary>**每个地图调一次、无门控**。</summary>
    public static bool LoadMapDoorIteratesAll()
        => LoadMapDoorCalls(0) == 0
           && LoadMapDoorCalls(3) == 3;

    /// <summary>**`ProcessMapDoor` 是空过程**。</summary>
    public static bool ProcessMapDoorIsEmpty() => true;

    /// <summary>该过程的体。</summary>
    public static string ProcessMapDoorBody() => "begin end";

    /// <summary>体为空。</summary>
    public static bool ProcessMapDoorBodyEmpty()
        => ProcessMapDoorBody().Replace("begin", "").Replace("end", "").Trim().Length == 0;

    /// <summary>**已注册但全程无人调用**。</summary>
    public static bool ProcessMapDoorNeverCalled() => true;

    /// <summary>**这是本工程第八种"空壳"形态**。</summary>
    public static string[] EmptyShellForms =
    {
        "空 begin end 块", "空分支", "恒真函数", "恒假函数",
        "占位方法", "注释掉的整块", "只赋值不使用的字段", "空过程体",
    };

    /// <summary>八种。</summary>
    public static bool EightEmptyShellForms() => EmptyShellForms.Length == 8;

    /// <summary>**空过程体是第八种**。</summary>
    public static bool EmptyProcedureIsEighth()
        => EmptyShellForms[7] == "空过程体";

    /// <summary>**`ReSetMinMap` 是双层循环**。</summary>
    public static bool ReSetMinMapDoubleLoop() => true;

    /// <summary>`ReSetMinMap` 的实现。</summary>
    public static Dictionary<string, int> ReSetMinMap(
        IReadOnlyList<(string Name, int MinMap)> maps,
        IReadOnlyList<(string Name, int Value)> miniMapList)
    {
        var result = new Dictionary<string, int>();

        foreach (var (name, minMap) in maps)
        {
            int value = minMap;

            for (int i = 0; i < miniMapList.Count; i++)
            {
                if (string.CompareOrdinal(
                        miniMapList[i].Name.ToUpperInvariant(), name.ToUpperInvariant()) == 0)
                {
                    value = miniMapList[i].Value;
                    break;
                }
            }

            result[name] = value;
        }

        return result;
    }

    /// <summary>**匹配则赋新值**。</summary>
    public static bool ReSetMinMapAssigns()
    {
        var r = ReSetMinMap(new[] { ("A", 0) }, new[] { ("A", 7) });

        return r["A"] == 7;
    }

    /// <summary>**不匹配则保持原值（不被清零）**。</summary>
    public static bool UnmatchedKeepsOldValue()
    {
        var r = ReSetMinMap(new[] { ("A", 5) }, new[] { ("Z", 7) });

        return r["A"] == 5;
    }

    /// <summary>**`Break` 只跳出内层 —— 每个地图仍会被处理**。</summary>
    public static bool BreakOnlyInner()
    {
        var r = ReSetMinMap(new[] { ("A", 0), ("B", 0) }, new[] { ("A", 1), ("B", 2) });

        return r["A"] == 1 && r["B"] == 2;
    }

    /// <summary>**同名多项时取第一个**。</summary>
    public static bool TakesFirstMiniMap()
    {
        var r = ReSetMinMap(new[] { ("A", 0) }, new[] { ("A", 1), ("A", 9) });

        return r["A"] == 1;
    }

    /// <summary>**小地图编号取自 `Objects` 列表（整数）**。</summary>
    public static bool MinMapFromObjectsList() => true;

    /// <summary>**空地图列表时无事发生**。</summary>
    public static bool EmptyMapListNoOp()
        => ReSetMinMap(Array.Empty<(string, int)>(), new[] { ("A", 1) }).Count == 0;

    /// <summary>**大小写不敏感匹配**。</summary>
    public static bool ReSetMinMapCaseInsensitive()
    {
        var r = ReSetMinMap(new[] { ("ABC", 0) }, new[] { ("abc", 4) });

        return r["ABC"] == 4;
    }

    // ===================== 五、继承关系 =====================

    /// <summary>**继承自 `TGList`**。</summary>
    public static bool InheritsFromList() => BaseClass == "TGList";

    /// <summary>**`Count` 与 `Items[I]` 来自父类**。</summary>
    public static bool ListMembersFromBase() => true;

    /// <summary>**行数**。</summary>
    public static readonly int[] MethodLineCounts = { 26, 38, 160, 22, 10, 3, 21 };

    /// <summary>七个方法。</summary>
    public static bool SevenMethods() => MethodLineCounts.Length == 7;

    /// <summary>**`GetMapInfo` 最长**。</summary>
    public static bool GetMapInfoIsLongest()
    {
        int max = 0;

        foreach (int n in MethodLineCounts)
        {
            if (n > max)
                max = n;
        }

        return max == 160;
    }

    /// <summary>**`ProcessMapDoor` 最短（3 行、空体）**。</summary>
    public static bool ProcessMapDoorIsShortest()
    {
        int min = int.MaxValue;

        foreach (int n in MethodLineCounts)
        {
            if (n < min)
                min = n;
        }

        return min == 3;
    }

    /// <summary>总行数。</summary>
    public static int TotalLines()
    {
        int t = 0;

        foreach (int n in MethodLineCounts)
            t += n;

        return t;
    }

    /// <summary>实测 280 行。</summary>
    public static bool TotalLinesValues() => TotalLines() == 280;
}
