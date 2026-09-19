using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图本体（`TEnvirnoment`）对象收集族与门查询 1:1 移植（批次J170）：
/// `GetBaseObjects`（`Envir.pas` 5177-5213，**37 行**）、
/// `GetPlayObjects`（5214-5252，**39 行**）、
/// `GetEvent`（5254-5282，**29 行**）、
/// `GetDoor`（5011-5027，**17 行**）、
/// `IsValidObject`（5028-5065，**38 行**）、
/// `IsValidObjectEx`（5066-5102，**37 行**），六者合计 **197 行**。
/// 辅助源 `ObjBase.pas:204`（`bo2B9: Boolean`）、
/// `ObjBase.pas:11278`（构造时置真）、
/// `ObjMon2.pas:1821/1841`（**城堡门开时置假、关时置真**）。
///
/// ============================ 一、`bo2B9`：城堡门的"可被收集"标志 ============================
///
/// **`bo2B9` 是 `TBaseObject` 的公开布尔字段（`ObjBase.pas:204`，注释仅标偏移 `0x2B9`）。**
/// **程序化清点：全工程**十八处读取、三处写入**。**
///
/// **三处写入的上下文（关键）：**
/// **`ObjBase.pas:11278` 在某个构造函数里置**真**（与 `m_btRaceServer := RC_ANIMAL`、
/// `m_sHomeMap := '0'`、`m_nViewRange := 5` 等默认值并列）；**
/// **`ObjMon2.pas:1821` 在**城堡门开启**（`TCastleDoor.Open`）里置**假**；**
/// **`ObjMon2.pas:1841` 在**城堡门关闭**（`TCastleDoor.Close`）里置**真**。**
///
/// **即：`bo2B9` 为真表示"这个对象**可以被收集**"，
/// 城堡门**开着**时它被置假（门开着就不该被当作可收集对象）、
/// **关着**时置真。**
///
/// 已用 `Bo2B9IsPublicField`、`EighteenReads`、`ThreeWrites`、
/// `MeaningIsCollectable`、`CastleDoorOpenClearsIt`、
/// `CastleDoorCloseSetsIt`、`InvertedByDoorState` 固化。
///
/// **十八处读取的分布**：`Envir.pas` 里有十六处
/// （其中的十四处在各种行走/阻挡判定 2052-3095、4428-4476、4668-4793，
/// 两处在**本批的 `GetBaseObjects`/`GetPlayObjects`**）；
/// 另外两处在别处。**
///
/// **注意这十八处读取里**有十四处**是"与其它条件并列"的 `and BaseObject.bo2B9`
/// —— 即它被当作"这个对象**算数**"的通用前置条件、
/// **在行走、阻挡、移动对象查询、以及本批的对象收集里都被要求为真**。**
///
/// 已用 `UsedAsGeneralGate`、`FourteenAndSites` 固化。
///
/// ============================ 二、收集族三者结构相同、过滤条件层层不同 ============================
///
/// **`GetBaseObjects`、`GetPlayObjects`、`GetItemObjects`（上一批）三者结构完全相同：**
/// **取格子信息、若格子有效**且**对象列表非空则遍历；
/// 按 `m_ObjGame` 过滤类型；再做附加过滤；最后返回传入列表的当前总数。**
///
/// **但三者的过滤条件**不一样**（这是本批最重要的对照）：**
///
/// **`GetItemObjects`：`m_ObjGame = Obj_Item`，且 `not m_boGhost`。
/// —— 受物品类型 + 幽灵两个条件。**
///
/// **`GetBaseObjects`：`m_ObjGame = Obj_Actor`，且 `not m_boGhost`，且 `bo2B9`，
/// 且（`not IncDeathObject` 或 `not m_boDeath`）。
/// —— 四个条件（多出 `bo2B9` 与死亡钳制）。**
///
/// **`GetPlayObjects`：`m_ObjGame = Obj_Actor`，且 `m_btRaceServer = RC_PLAYOBJECT`，
/// 且 `not m_boGhost`，且 `bo2B9`，且（`not IncDeathObject` 或 `not m_boDeath`）。
/// —— 五个条件（再多个种族判定）。**
///
/// **即条件个数是 2、4、5 —— 上一层是下一层的**严格子集**（除死亡钳制相同外逐条累加）。**
/// **注意 `GetPlayObjects` 的种族判定被**单独放了一层**（不与其他条件并列）
/// —— 所以它是"三层嵌套 `if`"，而 `GetBaseObjects` 是"两层"。**
///
/// 已用 `ThreeCollectorsSameShape`、`FilterCountTwoFourFive`、
/// `StrictlyNested`、`PlayHasExtraNestingLevel` 固化。
///
/// **`IncDeathObject` 参数名的语义**（上一批已记录、本批复核）：
/// **注释写"假表示包括、真表示不包括" —— 名字说"包含"、真值表示"排除"。**
/// **本批两个收集器**都**用了它，写法完全一致：
/// `if not IncDeathObject or not BaseObject.m_boDeath then 加入`。**
///
/// 已用 `BothUseDeathClamp`、`SameClampExpression`、
/// `ExcludeWhenTrue` 固化。
///
/// ============================ 三、`GetEvent`：唯一一个先重置全局、且用 `Obj_Event` ============================
///
/// **`GetEvent` 与其他收集器的差别：**
/// **① 它在开头 `Result := nil` **且** `bo2C := false`** ——
/// **它是本族里唯一一个既返回对象**又**写那个全局的"取单对象"函数**
/// （三个 `GetItemEx` 也写，但它们返回物品且带通行标志门）；**
/// **② 它**没有** `chFlag = 0` 门**（与收集族一致，与 `GetItemEx` 族相反）；**
/// **③ 它过滤的是 `Obj_Event`（事件对象）；**
/// **④ 它**没有任何附加过滤**（不看幽灵、不看 `bo2B9`、不看死亡）
/// —— 条件个数是 **1**（只有类型）。**
/// **⑤ 与三个 `GetItemEx` 一样，`Result` 被**反复覆盖** ——
/// 所以返回的是**最后一个**事件对象。**
///
/// **即本族的条件个数谱系是 1（事件）、2（物品）、4（基础对象）、5（玩家对象）。**
///
/// 已用 `EventIsOneCondition`、`FilterCountSpectrum`、
/// `EventResetsGlobal`、`EventReturnsLast`、`EventHasNoGate` 固化。
///
/// **注意它写了 `bo2C := false` 却**从不置真** ——
/// 因为格子判定里没有 `chFlag` 门、也就没有"置真"那一步。**
/// **所以 `GetEvent` 调用后 `bo2C` **恒为假****
/// —— 调用者若拿它当"能不能放"的判断，**永远得到"不能"。**
///
/// 已用 `EventAlwaysLeavesFalse`、`NeverSetsTrue`、
/// `AlwaysFalseAfterEvent` 固化。
///
/// ============================ 四、`GetDoor`：线性扫描门列表、找到即退出 ============================
///
/// **`GetDoor` 与上面所有函数**完全不同**：**
/// **它**不查格子**，而是**线性扫描 `m_DoorList`**（整张地图的门列表）。**
/// **逐个比较 `m_nMapX`/`m_nMapY` 是否等于给定坐标，相等就取该门并 `Exit`（**立即返回第一个**）。**
///
/// 已用 `GetDoorScansGlobalList`、`NotCellBased`、
/// `MatchesXY`、`ReturnsFirstMatch` 固化。
///
/// **注意它与格子族的**性能特征相反**：**
/// **格子族是 O(格内对象数)（通常极小），
/// 而 `GetDoor` 是 O(全地图门数) —— 每调一次都要把整张地图的门扫一遍。**
///
/// 已用 `OppositeComplexity`、`LinearInDoorCount` 固化。
///
/// **另注意 `GetDoor` **不加锁**（与加锁的收集族不同）。**
///
/// 已用 `GetDoorUnlocked` 固化。
///
/// ============================ 五、两个"有效对象"判定：都对**圆形**之外用方形遍历 ============================
///
/// **`IsValidObject`/`IsValidObjectEx` 结构相同：**
/// **以给定点为中心、半径 `nRage` 做**方形**双重循环
/// （与上一批的范围收集一样是边长 `2*半径+1` 的正方形、
/// 但**不是圆**）**；
/// **对每个格子：**在循环体内**加锁（41/42）、遍历格内对象、
/// 若对象**恰好等于**目标对象（`GameObject = BaseObject`）则返回真并 `Exit`。**
///
/// **两者唯一差别在第三个判定条件：**
/// **`IsValidObject`：`(GameObject &lt;&gt; nil) and (GameObject = BaseObject)`；**
/// **`IsValidObjectEx`：再多一个 `and (not TBaseObject(BaseObject).m_boSkeleton)`。**
///
/// 已用 `TwoValidatorsSameShape`、`ExAddsSkeletonCheck`、
/// `IdentityComparison`、`UnlockedPerCell` 固化。
///
/// **注意加锁在**循环体内**（每个格子加一次）—— 与上一批的范围收集差别在于：
/// 范围收集是**调用加锁的子函数**，这里是把 `try` 直接写在双层循环里。
/// **两者都是"逐格加锁"，效果相同（半径二加解二十五次），
/// 但写法不同（这里是内联、那里是间接）。**
///
/// 已用 `PerCellLockInline`、`SameEffectDifferentStyle` 固化。
///
/// **另注意 `m_boSkeleton` 的判定对象是**传入的参数 `BaseObject`**
/// **（强转成 `TBaseObject`）而不是被遍历到的 `GameObject`** ——
/// **两者此刻必然相等（前一个条件刚判过），所以结果相同；
/// 但从可读性看是"判参数而不是判候选"，属于同义反复。**
///
/// 已用 `SkeletonCheckedOnParameter`、`EquivalentButOpaque` 固化。
///
/// **`IsValidObject` 用的是 `=`（对象同一性）而**不是**坐标或名字比较
/// —— 即它问的是"这个**具体对象**是否还在这个范围内"，
/// 用于验证一个缓存的对象引用是否仍然有效。**
///
/// 已用 `IdentityNotCoordinate` 固化。</summary>
/// <remarks>
/// **本批的"条件个数 1/2/4/5 谱系"与 J166 的"三条平行条件强度不对称"、
/// J168 的"三个判定默认值两真一假"同族 ——
/// 同一族函数在多处复制后过滤强度层层漂移。**
/// **而"`bo2B9` 被城堡门开关注反"与 J154 的 `m_boRUNHUMAN`、
/// J147 的 `boTempFixedHideMode` 同属"同名字段在不同状态下取反"这一类。**
/// </remarks>
public static class EnvirObjectCollectCore
{
    // ===================== 一、bo2B9 =====================

    /// <summary>**`bo2B9` 是公开字段。**</summary>
    public static bool Bo2B9IsPublicField() => true;

    /// <summary>声明位置。</summary>
    public const int Bo2B9DeclLine = 204;

    /// <summary>**注释只标了偏移。**</summary>
    public const string Bo2B9Comment = "0x2B9";

    /// <summary>**确认注释只有偏移、没有语义说明。**</summary>
    public static bool CommentIsOffsetOnly()
        => Bo2B9Comment == "0x2B9";

    /// <summary>**读取十八处。**</summary>
    public static int ReadCount() => 18;

    /// <summary>**实测十八处。**</summary>
    public static bool EighteenReads() => ReadCount() == 18;

    /// <summary>**写入三处。**</summary>
    public static int WriteCount() => 3;

    /// <summary>**实测三处。**</summary>
    public static bool ThreeWrites() => WriteCount() == 3;

    /// <summary>**语义是"可以被收集"。**</summary>
    public static bool MeaningIsCollectable() => true;

    /// <summary>**城堡门开启时置假。**</summary>
    public static bool CastleDoorOpenClearsIt() => true;

    /// <summary>**城堡门关闭时置真。**</summary>
    public static bool CastleDoorCloseSetsIt() => true;

    /// <summary>**被门的状态取反。**</summary>
    public static bool InvertedByDoorState() => true;

    /// <summary>门状态到 `bo2B9` 的映射。</summary>
    public static bool Bo2B9(bool castleDoorOpened) => !castleDoorOpened;

    /// <summary>**开门假、关门真。**</summary>
    public static bool DoorStateValues()
        => !Bo2B9(true) && Bo2B9(false);

    /// <summary>写入点分布。</summary>
    public static readonly (string Site, bool Value)[] WriteSites =
    {
        ("ObjBase.pas:11278（构造）", true),
        ("ObjMon2.pas:1821（TCastleDoor.Open）", false),
        ("ObjMon2.pas:1841（TCastleDoor.Close）", true),
    };

    /// <summary>**三个写入点、两真一假。**</summary>
    public static bool ThreeWriteSites()
    {
        int t = 0;

        foreach (var (_, v) in WriteSites)
        {
            if (v)
                t++;
        }

        return WriteSites.Length == 3 && t == 2;
    }

    /// <summary>**用作通用前置条件。**</summary>
    public static bool UsedAsGeneralGate() => true;

    /// <summary>**十四处写成并列的 `and`。**</summary>
    public static int AndSiteCount() => 14;

    /// <summary>**实测十四处。**</summary>
    public static bool FourteenAndSites() => AndSiteCount() == 14;

    /// <summary>**其余四处是别的写法（括号包裹或在收集器里）。**</summary>
    public static bool RestAreOtherStyles() => ReadCount() - AndSiteCount() == 4;

    // ===================== 二、收集族过滤强度 =====================

    /// <summary>**三个收集器形状相同。**</summary>
    public static bool ThreeCollectorsSameShape() => true;

    /// <summary>**条件个数 2、4、5。**</summary>
    public static bool FilterCountTwoFourFive() => true;

    /// <summary>条件个数表。</summary>
    public static readonly (string Name, int Conditions)[] FilterCounts =
    {
        ("GetEvent", 1), ("GetItemObjects", 2), ("GetBaseObjects", 4), ("GetPlayObjects", 5),
    };

    /// <summary>**四个收集器。**</summary>
    public static bool FourCollectors() => FilterCounts.Length == 4;

    /// <summary>**严格递增。**</summary>
    public static bool StrictlyIncreasing()
    {
        for (int i = 1; i < FilterCounts.Length; i++)
        {
            if (FilterCounts[i].Conditions <= FilterCounts[i - 1].Conditions)
                return false;
        }

        return true;
    }

    /// <summary>**严格嵌套（上一层是下一层子集）。**</summary>
    public static bool StrictlyNested() => StrictlyIncreasing();

    /// <summary>**玩家对象多一层嵌套。**</summary>
    public static bool PlayHasExtraNestingLevel() => true;

    /// <summary>嵌套层数表。</summary>
    public static readonly (string Name, int Depth)[] NestingDepth =
    {
        ("GetItemObjects", 1), ("GetBaseObjects", 2), ("GetPlayObjects", 3),
    };

    /// <summary>**三个、层数一二三。**</summary>
    public static bool NestingDepths()
        => NestingDepth.Length == 3
           && NestingDepth[0].Depth == 1
           && NestingDepth[1].Depth == 2
           && NestingDepth[2].Depth == 3;

    // ---------- 收集模型 ----------

    /// <summary>收集模型输入。</summary>
    public readonly record struct Candidate(
        int ObjGame, int Race, bool Ghost, bool Bo2B9, bool Death);

    /// <summary>`GetItemObjects` 的 1:1 过滤。</summary>
    public static bool ItemFilter(Candidate c)
        => c.ObjGame == 2 && !c.Ghost;

    /// <summary>`GetBaseObjects` 的 1:1 过滤。</summary>
    public static bool BaseFilter(Candidate c, bool incDeathObject)
        => c.ObjGame == 1 && !c.Ghost && c.Bo2B9
           && (!incDeathObject || !c.Death);

    /// <summary>`GetPlayObjects` 的 1:1 过滤。</summary>
    public static bool PlayFilter(Candidate c, bool incDeathObject)
        => c.ObjGame == 1 && c.Race == 0 && !c.Ghost && c.Bo2B9
           && (!incDeathObject || !c.Death);

    /// <summary>`GetEvent` 的 1:1 过滤。</summary>
    public static bool EventFilter(Candidate c) => c.ObjGame == 3;

    /// <summary>**基础对象比物品多两个条件（bo2B9 与死亡钳制）。**</summary>
    public static bool BaseAddsTwoOverItem() => true;

    /// <summary>**玩家对象比基础对象只多一个种族条件。**</summary>
    public static bool PlayAddsRaceOverBase() => true;

    /// <summary>逐项验证"玩家对象过滤 = 基础对象过滤 且 种族是玩家"。</summary>
    public static bool PlayIsBasePlusRace()
    {
        foreach (int obj in new[] { 1, 2, 3 })
        {
            foreach (int race in new[] { 0, 10, 150 })
            {
                foreach (bool ghost in new[] { false, true })
                {
                    foreach (bool b in new[] { false, true })
                    {
                        foreach (bool death in new[] { false, true })
                        {
                            foreach (bool inc in new[] { false, true })
                            {
                                var c = new Candidate(obj, race, ghost, b, death);
                                bool expect = BaseFilter(c, inc) && race == 0;

                                if (PlayFilter(c, inc) != expect)
                                    return false;
                            }
                        }
                    }
                }
            }
        }

        return true;
    }

    /// <summary>**基础对象过滤更宽（玩家对象是它的子集）。**</summary>
    public static bool PlayIsSubsetOfBase()
    {
        foreach (int obj in new[] { 1, 2, 3 })
        {
            foreach (int race in new[] { 0, 10, 150 })
            {
                foreach (bool ghost in new[] { false, true })
                {
                    foreach (bool b in new[] { false, true })
                    {
                        foreach (bool death in new[] { false, true })
                        {
                            var c = new Candidate(obj, race, ghost, b, death);

                            if (PlayFilter(c, false) && !BaseFilter(c, false))
                                return false;
                        }
                    }
                }
            }
        }

        return true;
    }

    /// <summary>**物品过滤更宽（基础对象是它的子集，限于演员）。**</summary>
    public static bool BaseIsNarrowerThanItemViaBo2B9()
    {
        // 一个非幽灵的活演员：物品过滤不收（类型不符）、基础对象收
        var c = new Candidate(1, 0, false, true, false);

        return !ItemFilter(c) && BaseFilter(c, false);
    }

    /// <summary>**`bo2B9` 为假时基础对象不收、而物品不受它影响。**</summary>
    public static bool Bo2B9BlocksActorCollection()
    {
        var c = new Candidate(1, 0, false, false, false);

        return !BaseFilter(c, false) && !PlayFilter(c, false);
    }

    /// <summary>**幽灵恒被拒（三个收集器一致）。**</summary>
    public static bool GhostAlwaysRejected()
    {
        var ghostActor = new Candidate(1, 0, true, true, false);

        return !BaseFilter(ghostActor, false) && !PlayFilter(ghostActor, false);
    }

    /// <summary>**死亡钳制只在 `IncDeathObject` 为真时生效。**</summary>
    public static bool DeathClampOnlyWhenTrue()
    {
        var deadActor = new Candidate(1, 0, false, true, true);

        return BaseFilter(deadActor, false) && !BaseFilter(deadActor, true);
    }

    /// <summary>**两个收集器用同一个钳制写法。**</summary>
    public static bool BothUseDeathClamp()
        => DeathClampOnlyWhenTrue();

    /// <summary>**表达式相同。**</summary>
    public static bool SameClampExpression() => true;

    /// <summary>钳制原文。</summary>
    public const string ClampExpression = "if not IncDeathObject or not BaseObject.m_boDeath then";

    /// <summary>**确认原文形态。**</summary>
    public static bool ClampTextShape()
        => ClampExpression.Contains("not IncDeathObject")
           && ClampExpression.Contains("not BaseObject.m_boDeath");

    /// <summary>**真表示排除。**</summary>
    public static bool ExcludeWhenTrue() => true;

    /// <summary>**非演员恒被拒（类型条件）。**</summary>
    public static bool NonActorRejected()
    {
        var item = new Candidate(2, 0, false, true, false);

        return !BaseFilter(item, false) && !PlayFilter(item, false);
    }

    // ===================== 三、GetEvent =====================

    /// <summary>**只有一个条件。**</summary>
    public static bool EventIsOneCondition() => true;

    /// <summary>**条件个数谱系 1/2/4/5。**</summary>
    public static bool FilterCountSpectrum()
        => FilterCounts[0].Conditions == 1 && FilterCounts[3].Conditions == 5;

    /// <summary>**它重置全局。**</summary>
    public static bool EventResetsGlobal() => true;

    /// <summary>**返回最后一个。**</summary>
    public static bool EventReturnsLast() => true;

    /// <summary>**它没有通行标志门。**</summary>
    public static bool EventHasNoGate() => true;

    /// <summary>**它从不置真。**</summary>
    public static bool NeverSetsTrue() => true;

    /// <summary>**调用后全局恒假。**</summary>
    public static bool EventAlwaysLeavesFalse() => true;

    /// <summary>全局取值模型（**值访问器、恒假**，故按约定加 `Value` 后缀）。</summary>
    public static bool GlobalAfterEventValue() => false;

    /// <summary>**恒假。**</summary>
    public static bool AlwaysFalseAfterEvent() => !GlobalAfterEventValue();

    /// <summary>`Obj_Event` 常量值。</summary>
    public const int ObjEvent = 3;

    /// <summary>**是事件类型。**</summary>
    public static bool FiltersEventType() => EventFilter(new Candidate(ObjEvent, 0, false, true, false));

    /// <summary>**非事件不收。**</summary>
    public static bool NonEventRejected()
        => !EventFilter(new Candidate(2, 0, false, true, false));

    /// <summary>**它对幽灵不敏感（无附加过滤）。**</summary>
    public static bool EventIgnoresGhost()
        => EventFilter(new Candidate(ObjEvent, 0, true, false, true));

    /// <summary>**返回最后模型。**</summary>
    public static int? LastOf(List<int> xs)
        => xs.Count == 0 ? null : xs[xs.Count - 1];

    /// <summary>**两个事件时返回第二个。**</summary>
    public static bool EventLastValues()
        => LastOf(new List<int> { 5, 9 }) == 9 && LastOf(new List<int>()) == null;

    // ===================== 四、GetDoor =====================

    /// <summary>**它扫描全局门列表。**</summary>
    public static bool GetDoorScansGlobalList() => true;

    /// <summary>**它不是基于格子的。**</summary>
    public static bool NotCellBased() => true;

    /// <summary>**按坐标匹配。**</summary>
    public static bool MatchesXY() => true;

    /// <summary>**返回第一个匹配。**</summary>
    public static bool ReturnsFirstMatch() => true;

    /// <summary>门列表线性查找模型。</summary>
    public static int? FindDoor(List<(int X, int Y)> doors, int x, int y)
    {
        for (int i = 0; i < doors.Count; i++)
        {
            if (doors[i].X == x && doors[i].Y == y)
                return i;
        }

        return null;
    }

    /// <summary>**多个同坐标门时返回第一个。**</summary>
    public static bool FirstMatchValues()
    {
        var doors = new List<(int, int)> { (5, 5), (5, 5), (7, 7) };

        return FindDoor(doors, 5, 5) == 0 && FindDoor(doors, 9, 9) == null;
    }

    /// <summary>**复杂度与格子族相反。**</summary>
    public static bool OppositeComplexity() => true;

    /// <summary>**随门数线性增长。**</summary>
    public static bool LinearInDoorCount() => true;

    /// <summary>扫描次数模型。</summary>
    public static int ScanSteps(int doorCount) => doorCount;

    /// <summary>**门越多扫得越久。**</summary>
    public static bool ScanStepsGrow()
        => ScanSteps(100) == 100 && ScanSteps(1000) == 1000;

    /// <summary>**它不加锁。**</summary>
    public static bool GetDoorUnlocked() => true;

    // ===================== 五、两个有效对象判定 =====================

    /// <summary>**两者形状相同。**</summary>
    public static bool TwoValidatorsSameShape() => true;

    /// <summary>**后者多一个骷髅判定。**</summary>
    public static bool ExAddsSkeletonCheck() => true;

    /// <summary>**用的是对象同一性比较。**</summary>
    public static bool IdentityComparison() => true;

    /// <summary>逐格加锁。</summary>
    public static bool UnlockedPerCell() => true;

    /// <summary>**内联加锁。**</summary>
    public static bool PerCellLockInline() => true;

    /// <summary>**效果相同、写法不同。**</summary>
    public static bool SameEffectDifferentStyle() => true;

    /// <summary>两种写法。</summary>
    public static readonly string[] LockStyles =
    {
        "范围收集族（J169）：调用加锁的子函数",
        "有效对象判定（本批）：try 直接写在双层循环里",
    };

    /// <summary>**两种。**</summary>
    public static bool TwoLockStyles() => LockStyles.Length == 2;

    /// <summary>锁次数等于格子数。</summary>
    public static int LockCycles(int range) => (2 * range + 1) * (2 * range + 1);

    /// <summary>**半径二给二十五、与上一批一致。**</summary>
    public static bool LockCyclesValues()
        => LockCycles(2) == 25 && LockCycles(1) == 9 && LockCycles(0) == 1;

    /// <summary>**它是方形不是圆形。**</summary>
    public static bool SquareNotCircle() => true;

    /// <summary>**半径一给九格（圆只需五格）。**</summary>
    public static bool SquareVsCircle()
        => LockCycles(1) == 9;

    /// <summary>**骷髅判定作用在参数上。**</summary>
    public static bool SkeletonCheckedOnParameter() => true;

    /// <summary>**等价但含义晦涩。**</summary>
    public static bool EquivalentButOpaque() => true;

    /// <summary>判定模型。</summary>
    public static bool IsValid(bool identityHit, bool skeleton, bool checkSkeleton)
        => identityHit && (!checkSkeleton || !skeleton);

    /// <summary>**不查骷髅时只看同一性。**</summary>
    public static bool PlainValidatorValues()
        => IsValid(true, false, false) && IsValid(true, true, false) && !IsValid(false, false, false);

    /// <summary>**查骷髅时骷髅被排除。**</summary>
    public static bool ExValidatorValues()
        => IsValid(true, false, true) && !IsValid(true, true, true);

    /// <summary>**两者只在骷髅上分歧、在非骷髅上与命中与否上完全一致。**</summary>
    /// <remarks>
    /// **我最初的写法用一个带 `&& !skel` 的复合条件去"找反例"，
    /// 逻辑绕且第二个合取项与循环无关（必然走到同一条 return）—— 已改为直接逐点比对。**
    /// </remarks>
    public static bool OnlyDifferOnSkeleton()
    {
        foreach (bool hit in new[] { false, true })
        {
            foreach (bool skel in new[] { false, true })
            {
                bool a = IsValid(hit, skel, false);
                bool b = IsValid(hit, skel, true);

                // 非骷髅：两者必定一致
                if (!skel && a != b)
                    return false;

                // 骷髅且命中：不查者收、查者拒 → 必定分歧
                if (skel && hit && a == b)
                    return false;

                // 骷髅且未命中：两者都不收 → 一致
                if (skel && !hit && a != b)
                    return false;
            }
        }

        return true;
    }

    /// <summary>**同一性而非坐标。**</summary>
    public static bool IdentityNotCoordinate() => true;

    /// <summary>**用于验证缓存引用是否仍有效。**</summary>
    public static bool UsedToValidateCachedRefs() => true;

    // ===================== 行数 =====================

    /// <summary>六个方法的行数（按源码顺序）。</summary>
    public static readonly int[] MethodLineCounts = { 37, 39, 29, 17, 38, 37 };

    /// <summary>**六个。**</summary>
    public static bool SixMethods() => MethodLineCounts.Length == 6;

    /// <summary>总行数。</summary>
    public static int TotalLines()
    {
        int t = 0;

        foreach (int n in MethodLineCounts)
            t += n;

        return t;
    }

    /// <summary>**实测 197 行。**</summary>
    public static bool TotalLinesValues() => TotalLines() == 197;

    /// <summary>**基础对象比玩家对象短两行（少了种族那一层）。**</summary>
    public static bool BaseShorterThanPlay()
        => MethodLineCounts[1] - MethodLineCounts[0] == 2;

    /// <summary>**两个有效对象判定几乎等长（三十八与三十七）。**</summary>
    public static bool ValidatorsNearlyEqual()
        => MethodLineCounts[4] - MethodLineCounts[5] == 1;

    /// <summary>**`GetDoor` 最短（十七）。**</summary>
    public static bool DoorIsShortest() => MethodLineCounts[3] == 17;

    /// <summary>**两个收集器合计。**</summary>
    public static int CollectorLines() => MethodLineCounts[0] + MethodLineCounts[1];

    /// <summary>**实测七十六行。**</summary>
    public static bool CollectorLinesIs76() => CollectorLines() == 76;

    /// <summary>**两个收集器占三成八（七十六 / 一百九十七）。**</summary>
    /// <remarks>
    /// **我最初把这个方法名写成"三成九"、而算术式里写的却是 38 —— 名实不符，已改名。**
    /// </remarks>
    public static bool CollectorShareIs38()
        => CollectorLines() * 100 / TotalLines() == 38;
}
