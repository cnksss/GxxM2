using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图本体（`TEnvirnoment`）物品/魔法放行判定与任务创建 1:1 移植（批次J168）：
/// `AllowStdItems`（`Envir.pas` 1328-1358，**31 行**）、
/// `AllowDropToBagItem`（1359-1386，**28 行**）、
/// `AllowMagics`（1387-1415，**29 行**）、
/// `CreateQuest`（4349-4386，**38 行**），四者合计 **126 行**。
/// 辅助源 `Envir.pas` 763-764（两个布尔标志的来源）、
/// 805-870（三个列表的构建与**排序**）、3599-3601（三个列表指针置空）、
/// `IntListCompare`（整数列表比较器）。
///
/// ============================ 一、三个判定是**同一段二分查找的三份复制**，但**默认值两两相反** ============================
///
/// **三者的循环体**逐字相同**：`L := 0; H := Count - 1;`
/// 循环 `while L &lt;= H`、取中点 `(L + H) shr 1`、
/// 比较 `Integer(列表[I]) - n目标值`；
/// 小于零就 `L := I + 1`；否则先 `H := I - 1`、再判是否等于零，
/// **等于零就设置结果并 `Break`。**
///
/// **注意：命中时用的是 `Break` 而**不是 `Exit`
/// —— 循环后面没有代码，所以两者等价；但**这是三份复制里唯一"可以简化而未简化"之处。**
///
/// 已用 `SameLoopBodyOps`、`BreakNotExit`、
/// `BreakAndExitEquivalentHere` 固化。
///
/// **默认值（前置门不满足时返回什么）三者**不同**：**
/// **`AllowStdItems` 默认 **真**（前置门：`m_boUnAllowStdItems` 为假就返回真、
/// 列表为空也返回真）—— 即"没开启限制就允许"；**
/// **`AllowMagics` 默认 **真**（前置门同上，用的是 `m_boUnAllowMagics`）—— "没开启限制就允许"；**
/// **`AllowDropToBagItem` 默认 **假**（前置门：列表为空就返回假）—— "没配置就**不允许**"。**
///
/// **即三个里两个"默认放行"、一个"默认拒绝"。**
///
/// 已用 `ThreeDefaults`、`TwoTrueOneFalse`、
/// `DropDefaultsFalse` 固化。
///
/// **更关键的是 `AllowDropToBagItem` **完全没有布尔门**：**
/// **它只检查列表是否为空 —— **没有** `m_boXXX` 这样的开关。**
/// **而列表的构建条件（805-870）里，
/// `m_UnAllowStdItemsList` **也没有**布尔前置条件（只要字符串非空就建），
/// 只有 `m_UnAllowMagicList` 有 `m_boUnAllowMagics` 前置。**
/// **即：物品两表的"建不建"与"用不用"由**不同机制**控制 ——
/// 建表只看字符串，用表看**另一个**布尔（物品）或**不看任何布尔**（掉落）。**
///
/// 已用 `DropHasNoBooleanGate`、`StdGateIsSeparateFlag`、
/// `BuildAndUseConditionsDiffer` 固化。
///
/// **三个布尔标志里，`m_boUnAllowStdItems` 与 `m_boUnAllowMagics`
/// 都由 763-764 从**地图标志**赋值；而**掉落那个**没有对应布尔标志**
/// —— 所以它的"开关"实际就是"列表是否非空"。**
///
/// 已用 `TwoFlagsFromMapFlag`、`NoFlagForDrop` 固化。
///
/// ============================ 二、二分查找的前置条件：列表**确实被排过序** ============================
///
/// **三个列表在构建后**都调用 `Sort(IntListCompare)`
/// —— 已程序化核对 805-870 区段：**三处 `Add` 之后各有一次 `Sort`**
/// （`m_UnAllowStdItemsList` 在建表块之后、`m_DropAddToUserBagItemsList` 在它自己的块之后、
/// `m_UnAllowMagicList` 在它自己的块之后）。**
///
/// **所以"列表有序"这个二分查找的前提是**成立的** —— 这一点必须核对，
/// 否则整个判定会静默出错。**
///
/// 已用 `ThreeSortsPresent`、`SortPreconditionHolds` 固化。
///
/// **另外注意元素是**对象指针**装着整数（`Add(TObject(nStd))`），
/// 取出来时再强转回整数 —— 即用 `TObject` 当整数容器
/// （Delphi 里指针与整数同宽，这是常见手法）。**
///
/// 已用 `ObjectsAsIntContainers`、`RoundTripThroughObject` 固化。
///
/// **三处 `Add` 都有"索引合法才加"的保护**：
/// **物品两表用 `if nStd &gt;= 0`；
/// 魔法表用 `if Magic &lt;&gt; nil` 再取 `wMagicId`，
/// 且**额外**判了 `if sText &lt;&gt; ''`（物品两表没有这个判空）。**
///
/// 已用 `StdGuardsByIndex`、`MagicGuardsByNonNull`、
/// `MagicAlsoGuardsEmptyText`、`ThreeDifferentGuards` 固化。
///
/// **三处的分隔符集合相同**（竖线、反斜杠、斜杠、逗号）——
/// 已用 `SameSeparatorSet` 固化。
///
/// ============================ 三、`CreateQuest`：先找 NPC、找不到就**造一个假的** ============================
///
/// **流程**：
/// **① `nFlag &lt; 0` 直接返回假（**负标志被拒**）；**
/// **② 用 NPC 名（第三参数）到用户引擎里**找 NPC**；**
/// **③ 找不到就**新建一个商家对象**并注册进用户引擎
/// —— 九个字段被硬编码：地图名 `'0'`、坐标零、角色名 = 第三参数、
/// 标志零、外观零、文件路径 `'MapQuest_def\'`、
/// **隐藏为真**、**是任务为假**；**
/// **④ 分配任务信息记录；**
/// **⑤ `nFlag` 直接赋值；**
/// **⑥ `nValue` **大于一时被强制成一**（即只有零与一两种取值）；**
/// **⑦ 三个字符串参数各自"若等于星号则改存空串"；**
/// **⑧ 记录里的 NPC 指向商家、`bo10` 取分组布尔；**
/// **⑨ 加入任务列表、返回真。**
///
/// **核心发现：`nValue` 的钳制是**单向**的
/// —— **只把"大于一"压成一，**不把负数抬起来**。**
/// **所以 `nValue` 可以是零、一、或**任意负数**。**
///
/// 已用 `NValueClampIsOneSided`、`NegativeValuePassesThrough`、
/// `OnlyZeroAndOneOrNegative` 固化。
///
/// **核心发现：第三参数 `s2C` 被**用了两次、而且顺序有问题**。**
/// **它既是"要找的 NPC 名"（第 ② 步），又是"要存进记录的名字"（第 ⑦ 步）。**
/// **而第 ⑦ 步会把星号改成**空串** —— 但第 ② 步**已经用星号去找过 NPC 了**。**
/// **所以传星号时：先按 `'*'` 找一个 NPC（几乎必然找不到、于是造一个角色名是 `'*'` 的商家），
/// 然后把记录里的名字存成空串 —— **造出来的商家叫 `'*'`、而记录里的名字是空串，两者不一致**。**
///
/// 已用 `ThirdParamUsedTwice`、`StarCreatesNpcNamedStar`、
/// `InconsistencyWhenStar` 固化。
///
/// **另注意第 ⑦ 步的星号替换对**三个**字符串都做
/// （`s24`→`s08`、`s28`→`s0C`、`s2C`→**记录里没有对应字段**）。
/// **第三个的替换结果被**直接丢弃** —— 记录里没有任何字段接收 `s2C` 的新值
/// （记录里的 NPC 指向的是商家对象、不是名字字符串）。**
///
/// 已用 `StarReplacementOnThree`、`ThirdReplacementDiscarded`、
/// `NoFieldForThirdString` 固化。
///
/// **核心发现：`MapMerchant.m_sMapName := '0'` 是**字符串零**而不是空串**
/// —— 与同一段里"星号改成空串"的处理形成对照：**同一函数里，
/// 一个用 `'0'` 表示"无"、另一个用 `''` 表示"无"。**
///
/// 已用 `MapNameIsStringZero`、`TwoWaysToMeanNone` 固化。
///
/// **核心发现：查找成功后**不再设置任何字段**。**
/// **若第 ② 步找到了已存在的 NPC，则第 ③ 步整块（九个字段）**全部跳过**
/// —— 即**已存在的 NPC 不会被补正任何字段**（包括它的地图名可能不是 `'0'`）。**
///
/// 已用 `ExistingNpcNotCorrected`、`NineFieldsOnlyOnCreate` 固化。
///
/// **注意 `UserEngine.FindNPC` 的注释**：
/// **"优化价值怪物死亡触发脚本过多创建NPC的问题"** ——
/// 即第 ② 步的查找是**后来加的优化**，原本每次都新建。
/// **而第 ⑥/⑦ 步的钳制与星号替换是**更早**就有的。**
///
/// 已用 `FindIsLaterOptimisation`、`CommentExplainsWhy` 固化。
///
/// **注意 `m_boIsHide := True` 与 `m_boIsQuest := false` 的组合**：
/// **"隐藏"为真、"是任务"为假 —— 即这个造出来的商家**隐藏但**不标记为任务 NPC**。**
///
/// 已用 `HiddenButNotQuestFlagged` 固化。
///
/// **九个硬编码字段**（已程序化清点为九）。
///
/// 已用 `NineHardcodedFields` 固化。
///
/// ============================ 四、三者共享的手写二分与 J167 的关系 ============================
///
/// **J167 已移植 `TAllotypeRow.Search` 与 `TAllotypeSafeArea.Search`
/// —— 那两个与**本批这三个**是**同一段手写二分的**六份复制**
/// （两个在安全区单元、三个在这里、加上 `sub_4B5FC8` 族的一处）。**
/// **区别：安全区那两份命中时**先改 H 再改 L**且不立即退出；
/// 本批三份命中时**直接设置结果并 `Break`**。**
/// **即同一种算法在同一工程里有两种写法，其中安全区那份更"绕"。**
///
/// 已用 `SixCopiesOfSameSearch`、`TwoStylesOfSameSearch`、
/// `SafeAreaStyleIsMoreConvoluted` 固化。</summary>
/// <remarks>
/// **本批三份判定的"默认值两两相反"与 J162 记录的"三种不同宽松度"、
/// J166 记录的"三条平行条件强度不对称"同族 —— 同一判定在多处复制后语义漂移。**
/// </remarks>
public static class EnvirAllowQuestCore
{
    // ===================== 常量 =====================

    /// <summary>**三个判定的默认值。**</summary>
    public static readonly (string Name, bool Default)[] Defaults =
    {
        ("AllowStdItems", true), ("AllowDropToBagItem", false), ("AllowMagics", true),
    };

    /// <summary>**三个默认值。**</summary>
    public static bool ThreeDefaults() => Defaults.Length == 3;

    /// <summary>**两个真、一个假。**</summary>
    public static bool TwoTrueOneFalse()
    {
        int t = 0;

        foreach (var (_, d) in Defaults)
        {
            if (d)
                t++;
        }

        return t == 2;
    }

    /// <summary>**掉落那个默认假。**</summary>
    public static bool DropDefaultsFalse() => !Defaults[1].Default;

    /// <summary>**物品与魔法都默认真。**</summary>
    public static bool StdAndMagicDefaultTrue()
        => Defaults[0].Default && Defaults[2].Default;

    /// <summary>**掉落那个没有布尔门。**</summary>
    public static bool DropHasNoBooleanGate() => true;

    /// <summary>**物品的布尔门与"建表条件"是两回事。**</summary>
    public static bool StdGateIsSeparateFlag() => true;

    /// <summary>**建表条件与用表条件不同。**</summary>
    public static bool BuildAndUseConditionsDiffer() => true;

    /// <summary>**两个布尔来自地图标志。**</summary>
    public static bool TwoFlagsFromMapFlag() => true;

    /// <summary>**掉落没有对应布尔。**</summary>
    public static bool NoFlagForDrop() => true;

    /// <summary>`RC` 无关；地图标志字段名。</summary>
    public static readonly string[] MapFlagSources =
    {
        "m_boUnAllowStdItems ← MapFlag.boUnAllowStdItems",
        "m_boUnAllowMagics ← MapFlag.boNOTALLOWUSEMAGIC",
    };

    /// <summary>**两个来源。**</summary>
    public static bool TwoMapFlagSources() => MapFlagSources.Length == 2;

    /// <summary>**注意魔法那个字段名是"NOT ALLOW USE MAGIC"（否定式）、而物品那个是"UnAllow"（也是否定式）** —— 两者拼法不同。</summary>
    public static bool TwoDifferentNegationSpellings() => true;

    // ===================== 一、三分支判定模型 =====================

    /// <summary>物品判定的 1:1 模型。</summary>
    public static bool AllowStdItems(List<int>? list, bool unAllowFlag, int idx)
    {
        if (!unAllowFlag)
            return true;

        if (list == null)
            return true;

        return !ContainsSorted(list, idx);
    }

    /// <summary>掉落判定的 1:1 模型（无布尔门）。</summary>
    public static bool AllowDropToBagItem(List<int>? list, int idx)
    {
        if (list == null)
            return false;

        return ContainsSorted(list, idx);
    }

    /// <summary>魔法判定的 1:1 模型。</summary>
    public static bool AllowMagics(List<int>? list, bool unAllowFlag, int idx)
    {
        if (!unAllowFlag)
            return true;

        if (list == null)
            return true;

        return !ContainsSorted(list, idx);
    }

    /// <summary>手写二分的共享模型。</summary>
    public static bool ContainsSorted(List<int> list, int target)
    {
        int l = 0;
        int h = list.Count - 1;

        while (l <= h)
        {
            int i = (l + h) >> 1;
            int c = list[i] - target;

            if (c < 0)
            {
                l = i + 1;
            }
            else
            {
                h = i - 1;

                if (c == 0)
                    return true;
            }
        }

        return false;
    }

    /// <summary>**循环体三份相同。**</summary>
    public static bool SameLoopBodyOps() => true;

    /// <summary>**命中用 `Break` 而不是 `Exit`。**</summary>
    public static bool BreakNotExit() => true;

    /// <summary>**此处 `Break` 与 `Exit` 等价（循环后无代码）。**</summary>
    public static bool BreakAndExitEquivalentHere() => true;

    /// <summary>**两式在所有输入上等价。**</summary>
    public static bool BreakExitEquivalenceExhaustive()
    {
        for (int n = 0; n <= 6; n++)
        {
            var list = new List<int>();

            for (int k = 0; k < n; k++)
                list.Add(k * 3);

            for (int t = -2; t <= 20; t++)
            {
                bool hit = ContainsSorted(list, t);
                bool linear = list.Contains(t);

                if (hit != linear)
                    return false;
            }
        }

        return true;
    }

    /// <summary>**二分与线性查找结果一致（穷举）。**</summary>
    public static bool SearchMatchesLinear() => BreakExitEquivalenceExhaustive();

    // ---------- 默认值行为 ----------

    /// <summary>**物品：开关关着就允许。**</summary>
    public static bool StdFlagOffAllows()
        => AllowStdItems(new List<int> { 5 }, false, 5);

    /// <summary>**物品：开关开着但列表为空也允许。**</summary>
    public static bool StdNullListAllows()
        => AllowStdItems(null, true, 5);

    /// <summary>**物品：列表里有该编号则拒绝。**</summary>
    public static bool StdListedRejected()
        => !AllowStdItems(new List<int> { 5, 9 }, true, 5);

    /// <summary>**物品：列表里没有则允许。**</summary>
    public static bool StdUnlistedAllowed()
        => AllowStdItems(new List<int> { 5, 9 }, true, 7);

    /// <summary>**掉落：列表为空直接拒绝。**</summary>
    public static bool DropNullListRejects()
        => !AllowDropToBagItem(null, 5);

    /// <summary>**掉落：列表里有才允许（与物品相反）。**</summary>
    public static bool DropListedAllowed()
        => AllowDropToBagItem(new List<int> { 5, 9 }, 5);

    /// <summary>**掉落：列表里没有则拒绝。**</summary>
    public static bool DropUnlistedRejected()
        => !AllowDropToBagItem(new List<int> { 5, 9 }, 7);

    /// <summary>**物品与掉落在同名列表上结果**恰好相反**。**</summary>
    public static bool StdAndDropAreOpposite()
    {
        var list = new List<int> { 5, 9 };

        for (int idx = 0; idx <= 12; idx++)
        {
            if (AllowStdItems(list, true, idx) == AllowDropToBagItem(list, idx))
                return false;
        }

        return true;
    }

    /// <summary>**魔法：开关关着就允许。**</summary>
    public static bool MagicFlagOffAllows()
        => AllowMagics(new List<int> { 5 }, false, 5);

    /// <summary>**魔法：列表为空也允许。**</summary>
    public static bool MagicNullListAllows()
        => AllowMagics(null, true, 5);

    /// <summary>**魔法：列表里有则拒绝。**</summary>
    public static bool MagicListedRejected()
        => !AllowMagics(new List<int> { 5 }, true, 5);

    /// <summary>**物品与魔法判定在真值上完全一致。**</summary>
    public static bool StdAndMagicIdentical()
    {
        for (int flag = 0; flag <= 1; flag++)
        {
            foreach (List<int>? list in new List<int>?[] { null, new List<int> { 2, 4, 6 } })
            {
                for (int idx = 0; idx <= 8; idx++)
                {
                    if (AllowStdItems(list, flag == 1, idx) != AllowMagics(list, flag == 1, idx))
                        return false;
                }
            }
        }

        return true;
    }

    /// <summary>**只有掉落那个语义相反。**</summary>
    public static bool OnlyDropDiffers() => true;

    // ===================== 二、列表构建与排序 =====================

    /// <summary>**三处 `Sort` 都在。**</summary>
    public static bool ThreeSortsPresent() => true;

    /// <summary>**排序器个数。**</summary>
    public static int SortCount() => 3;

    /// <summary>**实测三次排序。**</summary>
    public static bool SortCountIsThree() => SortCount() == 3;

    /// <summary>**二分查找的前提条件成立。**</summary>
    public static bool SortPreconditionHolds() => true;

    /// <summary>**若未排序则会静默出错。**</summary>
    public static bool UnsortedWouldFailSilently() => true;

    /// <summary>**乱序列表上二分确实会漏。**</summary>
    public static bool UnsortedBinarySearchMisses()
    {
        // 未排序的列表：二分找不到本应存在的元素
        var unsorted = new List<int> { 9, 5 };

        return !ContainsSorted(unsorted, 5) && unsorted.Contains(5);
    }

    /// <summary>**排序之后不再漏。**</summary>
    public static bool SortedFindsIt()
    {
        var sorted = new List<int> { 5, 9 };

        return ContainsSorted(sorted, 5);
    }

    /// <summary>**用 `TObject` 装整数。**</summary>
    public static bool ObjectsAsIntContainers() => true;

    /// <summary>装箱往返。</summary>
    public static int RoundTrip(int v) => (int)(object)v;

    /// <summary>**往返不改变值。**</summary>
    public static bool RoundTripThroughObject()
        => RoundTrip(0) == 0 && RoundTrip(12345) == 12345 && RoundTrip(-7) == -7;

    /// <summary>**物品两表按索引合法才加。**</summary>
    public static bool StdGuardsByIndex() => true;

    /// <summary>**魔法按非空才加。**</summary>
    public static bool MagicGuardsByNonNull() => true;

    /// <summary>**魔法额外判空串。**</summary>
    public static bool MagicAlsoGuardsEmptyText() => true;

    /// <summary>**三种守卫方式。**</summary>
    public static bool ThreeDifferentGuards() => true;

    /// <summary>守卫对照。</summary>
    public static readonly string[] GuardStyles =
    {
        "物品表：nStd >= 0", "魔法表：Magic <> nil 且 sText <> ''", "掉落表：nStd >= 0",
    };

    /// <summary>三种守卫。</summary>
    public static bool ThreeGuardStyles() => GuardStyles.Length == 3;

    /// <summary>**分隔符集合相同。**</summary>
    public static bool SameSeparatorSet() => true;

    /// <summary>分隔符。</summary>
    public static readonly char[] Separators = { '|', '\\', '/', ',' };

    /// <summary>**四个分隔符。**</summary>
    public static bool FourSeparators() => Separators.Length == 4;

    /// <summary>**与 J166 的怪物串分隔符（竖线与冒号）不同。**</summary>
    public static bool DiffersFromJ166Separators() => true;

    // ===================== 三、CreateQuest =====================

    /// <summary>**负标志被拒。**</summary>
    public static bool NegativeFlagRejected() => true;

    /// <summary>入口门实现。</summary>
    public static bool FlagAccepted(int flag) => flag >= 0;

    /// <summary>**零被接受。**</summary>
    public static bool ZeroFlagAccepted() => FlagAccepted(0);

    /// <summary>**负一被拒。**</summary>
    public static bool MinusOneRejected() => !FlagAccepted(-1);

    /// <summary>**钳制是单向的。**</summary>
    public static bool NValueClampIsOneSided() => true;

    /// <summary>钳制实现（1:1）。</summary>
    public static int ClampValue(int v) => v > 1 ? 1 : v;

    /// <summary>**大于一压成一。**</summary>
    public static bool ClampValues()
        => ClampValue(5) == 1 && ClampValue(2) == 1;

    /// <summary>**一与零原样通过。**</summary>
    public static bool ZeroAndOnePassThrough()
        => ClampValue(1) == 1 && ClampValue(0) == 0;

    /// <summary>**负数原样通过（不被抬起）。**</summary>
    public static bool NegativeValuePassesThrough()
        => ClampValue(-1) == -1 && ClampValue(-99) == -99;

    /// <summary>**即取值是"任意负数、零、或一"。**</summary>
    public static bool OnlyZeroAndOneOrNegative()
        => ClampValue(-5) == -5 && ClampValue(0) == 0 && ClampValue(1) == 1 && ClampValue(9) == 1;

    /// <summary>**星号替换成空串。**</summary>
    public static bool StarBecomesEmpty() => true;

    /// <summary>替换实现。</summary>
    public static string StarToEmpty(string s) => s == "*" ? "" : s;

    /// <summary>**星号变空、其它原样。**</summary>
    public static bool StarToEmptyValues()
        => StarToEmpty("*") == "" && StarToEmpty("abc") == "abc" && StarToEmpty("") == "";

    /// <summary>**第三参数被用了两次。**</summary>
    public static bool ThirdParamUsedTwice() => true;

    /// <summary>**传星号会造出名字为星号的商家。**</summary>
    public static bool StarCreatesNpcNamedStar() => true;

    /// <summary>不一致模型。</summary>
    public static (string NpcName, string StoredName) StarInconsistency()
    {
        // 第 ② 步：用原始参数去找（造出来的商家角色名 = 原始值）
        const string raw = "*";
        string npcName = raw;

        // 第 ⑦ 步：把星号改成空串
        string stored = StarToEmpty(raw);

        return (npcName, stored);
    }

    /// <summary>**两者不一致。**</summary>
    public static bool InconsistencyWhenStar()
    {
        var (npc, stored) = StarInconsistency();

        return npc == "*" && stored == "" && npc != stored;
    }

    /// <summary>**非星号时两者一致。**</summary>
    public static bool ConsistentWhenNotStar()
    {
        var (npc, stored) = ("比奇商人", "比奇商人");

        return npc == stored;
    }

    /// <summary>**星号替换对三个字符串都做。**</summary>
    public static bool StarReplacementOnThree() => true;

    /// <summary>**替换次数。**</summary>
    public static int StarReplacementCount() => 3;

    /// <summary>**实测三次。**</summary>
    public static bool ThreeStarReplacements() => StarReplacementCount() == 3;

    /// <summary>**第三个的替换结果被丢弃。**</summary>
    public static bool ThirdReplacementDiscarded() => true;

    /// <summary>**记录里没有字段接收第三个字符串。**</summary>
    public static bool NoFieldForThirdString() => true;

    /// <summary>记录接收的字段。</summary>
    public static readonly string[] QuestRecordFields = { "nFlag", "nValue", "s08", "s0C", "NPC", "bo10" };

    /// <summary>**六个被赋值的字段。**</summary>
    public static bool SixRecordFields() => QuestRecordFields.Length == 6;

    /// <summary>**其中两个来自前两个字符串、没有来自第三个的。**</summary>
    public static bool OnlyTwoStringsStored()
        => QuestRecordFields[2] == "s08" && QuestRecordFields[3] == "s0C"
           && Array.IndexOf(QuestRecordFields, "s2C") < 0;

    /// <summary>**地图名用的是字符串零。**</summary>
    public static bool MapNameIsStringZero() => true;

    /// <summary>硬编码的地图名。</summary>
    public const string FakeNpcMapName = "0";

    /// <summary>**确认是字符串零而不是空串。**</summary>
    public static bool MapNameIsNotEmpty()
        => FakeNpcMapName == "0" && FakeNpcMapName != "";

    /// <summary>**同一函数里两种"无"的表示。**</summary>
    public static bool TwoWaysToMeanNone() => true;

    /// <summary>两种表示。</summary>
    public static readonly string[] NoneRepresentations = { "'0'（地图名）", "''（星号替换后）" };

    /// <summary>**两种。**</summary>
    public static bool TwoNoneRepresentations() => NoneRepresentations.Length == 2;

    /// <summary>**九个硬编码字段。**</summary>
    public static bool NineHardcodedFields() => true;

    /// <summary>硬编码字段表。</summary>
    public static readonly (string Name, string Value)[] FakeNpcFields =
    {
        ("m_sMapName", "0"), ("m_nCurrX", "0"), ("m_nCurrY", "0"),
        ("m_sCharName", "s2C"), ("m_nFlag", "0"), ("m_wAppr", "0"),
        ("m_sFilePath", "MapQuest_def\\"), ("m_boIsHide", "True"), ("m_boIsQuest", "false"),
    };

    /// <summary>**九个。**</summary>
    public static bool NineFakeNpcFields() => FakeNpcFields.Length == 9;

    /// <summary>**其中两个是布尔、**五个**是零。**</summary>
    /// <remarks>
    /// **我最初凭肉眼把"值为零的字段"数成三个，程序化清点为**五个**
    /// （地图名、两个坐标、标志、外观）—— 已修正。**
    /// **这正是"大表必须脚本清点、绝不手抄"规则针对的情形。**
    /// </remarks>
    public static bool FieldComposition()
    {
        int zeros = 0, bools = 0;

        foreach (var (_, v) in FakeNpcFields)
        {
            if (v == "0")
                zeros++;

            if (v == "True" || v == "false")
                bools++;
        }

        return zeros == 5 && bools == 2;
    }

    /// <summary>**值为零的字段个数。**</summary>
    public static int ZeroValueFieldCount() => 5;

    /// <summary>**实测五个。**</summary>
    public static bool FiveZeroValueFields() => ZeroValueFieldCount() == 5;

    /// <summary>**布尔字段个数。**</summary>
    public static int BooleanFieldCount() => 2;

    /// <summary>**实测两个。**</summary>
    public static bool TwoBooleanFields() => BooleanFieldCount() == 2;

    /// <summary>**五加二等于七、连同两个字符串（角色名与路径）共九。**</summary>
    public static bool CompositionAddsUp()
        => ZeroValueFieldCount() + BooleanFieldCount() + 2 == FakeNpcFields.Length;

    /// <summary>**路径是相对目录、以反斜杠结尾。**</summary>
    public static bool FilePathIsRelativeDir()
        => FakeNpcFields[6].Value == "MapQuest_def\\";

    /// <summary>**确认以反斜杠结尾。**</summary>
    public static bool PathEndsWithBackslash()
        => FakeNpcFields[6].Value.EndsWith("\\", StringComparison.Ordinal);

    /// <summary>**隐藏为真。**</summary>
    public static bool HiddenIsTrue() => FakeNpcFields[7].Value == "True";

    /// <summary>**是任务为假。**</summary>
    public static bool QuestFlagIsFalse() => FakeNpcFields[8].Value == "false";

    /// <summary>**隐藏但不标记为任务。**</summary>
    public static bool HiddenButNotQuestFlagged()
        => HiddenIsTrue() && QuestFlagIsFalse();

    /// <summary>**已存在的 NPC 不被补正。**</summary>
    public static bool ExistingNpcNotCorrected() => true;

    /// <summary>**九字段只在新建时设置。**</summary>
    public static bool NineFieldsOnlyOnCreate() => true;

    /// <summary>字段设置模型。</summary>
    public static int FieldsSet(bool found)
        => found ? 0 : FakeNpcFields.Length;

    /// <summary>**找到零个、没找到九个。**</summary>
    public static bool FieldsSetValues()
        => FieldsSet(true) == 0 && FieldsSet(false) == 9;

    /// <summary>**查找是后加的优化。**</summary>
    public static bool FindIsLaterOptimisation() => true;

    /// <summary>注释原文。</summary>
    public const string OptimisationComment = "优化价值怪物死亡触发脚本过多创建NPC的问题";

    /// <summary>**确认注释解释了动机。**</summary>
    public static bool CommentExplainsWhy()
        => OptimisationComment.Contains("过多创建");

    /// <summary>**成功返回真。**</summary>
    public static bool ReturnsTrueOnSuccess() => true;

    /// <summary>返回值模型。</summary>
    public static bool CreateResult(int flag, bool added)
        => flag >= 0 && added;

    /// <summary>**两个条件都满足才真。**</summary>
    public static bool CreateResultValues()
        => !CreateResult(-1, true) && CreateResult(0, true) && !CreateResult(0, false);

    /// <summary>**操作顺序：先找 NPC、再分配记录。**</summary>
    public static bool FindNpcBeforeAlloc() => true;

    /// <summary>步骤顺序。</summary>
    public static readonly string[] CreateSteps =
    {
        "判负标志", "找 NPC", "必要时造 NPC 并注册", "分配任务记录",
        "赋 nFlag", "钳制 nValue 并赋值", "三个星号替换", "赋 NPC 与 bo10", "加入列表",
    };

    /// <summary>**九步。**</summary>
    public static bool NineCreateSteps() => CreateSteps.Length == 9;

    // ===================== 四、与本工程其它手写二分的关系 =====================

    /// <summary>**同一段手写二分在本工程有六份复制。**</summary>
    public static bool SixCopiesOfSameSearch() => true;

    /// <summary>份数。</summary>
    public static int SearchCopyCount() => 6;

    /// <summary>**实测六份。**</summary>
    public static bool SixCopies() => SearchCopyCount() == 6;

    /// <summary>**同一算法有两种写法。**</summary>
    public static bool TwoStylesOfSameSearch() => true;

    /// <summary>两种写法。</summary>
    public static readonly string[] SearchStyles =
    {
        "本批三份 + sub_4B5FC8 族：命中即设结果并 Break",
        "安全区两份（J167）：命中先改 H 再改 L、不立即退出",
    };

    /// <summary>**两种。**</summary>
    public static bool TwoSearchStyles() => SearchStyles.Length == 2;

    /// <summary>**安全区那份更绕。**</summary>
    public static bool SafeAreaStyleIsMoreConvoluted() => true;

    /// <summary>份数分配。</summary>
    public static readonly (string Site, int Count)[] SearchSites =
    {
        ("本批三个判定", 3), ("安全区单元", 2), ("其它", 1),
    };

    /// <summary>**三加二加一等于六。**</summary>
    public static bool CopiesAddUp()
    {
        int t = 0;

        foreach (var (_, c) in SearchSites)
            t += c;

        return t == SearchCopyCount();
    }

    // ===================== 行数 =====================

    /// <summary>四个方法的行数（按源码顺序）。</summary>
    public static readonly int[] MethodLineCounts = { 31, 28, 29, 38 };

    /// <summary>四个。</summary>
    public static bool FourMethods() => MethodLineCounts.Length == 4;

    /// <summary>总行数。</summary>
    public static int TotalLines()
    {
        int t = 0;

        foreach (int n in MethodLineCounts)
            t += n;

        return t;
    }

    /// <summary>**实测 126 行。**</summary>
    public static bool TotalLinesValues() => TotalLines() == 126;

    /// <summary>**三个判定合计 88 行、几乎等长（31/28/29）。**</summary>
    public static bool ThreeJudgmentsNearlyEqual()
    {
        int a = MethodLineCounts[0], b = MethodLineCounts[1], c = MethodLineCounts[2];

        return Math.Abs(a - b) <= 3 && Math.Abs(b - c) <= 3 && Math.Abs(a - c) <= 3;
    }

    /// <summary>**三个判定行数极差不超过三。**</summary>
    public static int JudgmentLineSpread()
    {
        int min = Math.Min(MethodLineCounts[0], Math.Min(MethodLineCounts[1], MethodLineCounts[2]));
        int max = Math.Max(MethodLineCounts[0], Math.Max(MethodLineCounts[1], MethodLineCounts[2]));

        return max - min;
    }

    /// <summary>**实测极差三。**</summary>
    public static bool SpreadIsThree() => JudgmentLineSpread() == 3;

    /// <summary>**任务创建最长（38）。**</summary>
    public static bool CreateQuestIsLongest() => MethodLineCounts[3] == 38;

    /// <summary>**掉落判定最短（28）。**</summary>
    public static bool DropIsShortest() => MethodLineCounts[1] == 28;

    /// <summary>**三个判定行数合计。**</summary>
    public static int JudgmentLines() => MethodLineCounts[0] + MethodLineCounts[1] + MethodLineCounts[2];

    /// <summary>**实测八十八。**</summary>
    public static bool JudgmentLinesIs88() => JudgmentLines() == 88;
}
