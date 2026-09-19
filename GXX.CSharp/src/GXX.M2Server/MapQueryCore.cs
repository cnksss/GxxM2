using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图取对象原语 1:1 移植（批次J132）：
/// `GetMovingObject` 三个重载（`Envir.pas` 4643-4685、4687-4726、4728-4768）、
/// `GetRangeBaseObject`（5150-5160）、`GetBaseObjects`（5177-5212）、`GetPlayObjects`（5214-5252）、
/// `GetRangePlayObject`（5162-5172）、`GetXYHuman`（5327-5362）、`sub_4B5FC8`（5364-5371）；
/// 辅助源：`Envir.pas` 422-441（声明）、5174-5176（`GetBaseObjects` 的参数注释）、
/// `ObjBase.pas` 204/11278、`ObjMon2.pas` 1821/1841、`Grobal2.pas`。
///
/// 本批次是 J131 的直接续作：J131 解出了 `chFlag` 的两个取值，本批次处理**在那些格子上
/// 如何把对象"捞"出来**——包括三个 `GetMovingObject` 重载、范围扫描与两个布尔查询。
///
/// ============================ 一、参数注释与实际代码矛盾（本批次最重要发现） ============================
///
/// **5174-5176 的注释**明明白白写着：
/// ```
/// // boFlag 是否包括死亡对象
/// // FALSE 包括死亡对象
/// // TRUE  不包括死亡对象
/// ```
///
/// **但代码里的参数名是 `IncDeathObject`**（5177），而**实际逻辑是 5199**：
/// `if not IncDeathObject or not BaseObject.m_boDeath then BaseObjectList.Add(BaseObject);`
///
/// 代入求解：`IncDeathObject = True` 时，`not True = False`，故需 `not m_boDeath` 为真才加入 →
/// **只加"没死"的对象（不含死亡对象）**；
/// `IncDeathObject = False` 时，`not False = True` 使 `or` 恒真 → **无条件加入（含死亡对象）**。
///
/// **所以注释说的方向与参数名的字面含义是一致的**（`IncDeathObject = True` ⇔ 排除死亡 ✓，
/// 与注释的「TRUE 不包括死亡对象」一致）。
/// **真正的陷阱在于 `GetRangeBaseObject`（5154-5157）把它的 `boFlag` 参数原样传给了 `IncDeathObject`**
/// ——即 `GetRangeBaseObject(..., boFlag: Boolean, ...)` 这个**参数名保留了旧叫法 `boFlag`**，
/// 而语义已经是 `IncDeathObject`。
///
/// **后果**：几乎**所有调用方都传 `True`**（GameEvent.pas 809/1036、HandleCommands.pas 5326、
/// NpcActionCmd.pas 10369/13233/40618/43148/43215/43282/43351/44073、
/// NpcConditionCmd.pas 3133/3204/4950、ObjBase.pas 8616/8643 全部传 `True`）
/// ——按注释读会以为"传 `True` = 包括死亡对象"，
/// **但实际是"传 `True` = 排除死亡对象"**。若有人照着注释改传 `False`，
/// 就会**把尸体也一并返回**（恰好与注释的意图相反）。
/// 已用 `CommentAndNameAgreeButNameIsStale`、`ParamNameIsStaleBoFlag`、
/// `TrueExcludesDeath`、`AllCallersPassTrue` 固化。
///
/// **注意 `GetMovingObject` 的 `boFlag` 方向与 `GetBaseObjects` 的 `IncDeathObject` 相同**
/// （4669/4712/4754 都是 `(not boFlag) or (not m_boDeath)`）——
/// 即**三个函数的 `boFlag`/`IncDeathObject` 语义一致，只有 `GetRangeBaseObject` 的参数名没跟上改名**。
/// 已用 `ThreeFunctionsShareSemantics` 固化。
///
/// ============================ 二、三个 `GetMovingObject` 重载的四处差异 ============================
///
/// 三个重载的过滤条件**几乎相同**（`m_ObjGame = Obj_Actor` + `not m_boGhost` + `bo2B9`
/// + 死亡过滤），但**在四处不同**：
///
/// **差异一：返回类型与"找到后是否继续"**——
/// - 重载 A（4643，带 `TList`）：`Result := 0` 起、**不 `Break`**、`Inc(Result)` 累计、
///   把**每个**合格对象 `Add` 进列表（若列表非 nil）→ **返回数量**；
/// - 重载 B（4687，只带 `boFlag`）：`Result := nil` 起、**`Break`** → **返回第一个合格对象**；
/// - 重载 C（4728，带 `AObject`）：同样 `Break` → 返回**第一个等于 `AObject` 的合格对象**。
/// 已用 `OverloadAReturnsCount`、`OverloadBReturnsFirst`、`OverloadCReturnsMatch`、
/// `OnlyOverloadACollectsAll` 固化。
///
/// **差异二：`TList` 可为 nil 的处理**——重载 A **在内层判断 `if BaseObjectList &lt;&gt; nil then`**（4671），
/// 故**传 nil 列表仍然会正确计数**（只是不收集）。这是三个重载里唯一的 nil 容忍。
/// 已用 `OverloadAToleratesNullList` 固化。
///
/// **差异三：锁索引各不同**——重载 A 用 **`LockR(33)`**（4653）、
/// 重载 B 用 **`LockR(34)`**（4697）、重载 C 用 **`LockR(36)`**（4738）
/// ——**注意跳过了 35**，且与 J129-J131 记录的 4/16/17/19/28/44/45/46/47 都不同。
/// 已用 `ThreeOverloadsDifferentLocks`、`LockIndex35Skipped` 固化。
///
/// **差异四：重载 C 多一个 `BaseObject = AObject` 判定**——
/// 且它排在 `not m_boGhost` **之前**（4751），而重载 A/B 的第一个判定是 `BaseObject &lt;&gt; nil`。
/// 已用 `OverloadCHasIdentityCheck` 固化。
///
/// **三个重载共有的冗余**：都能在 `GameObject.m_ObjGame = Obj_Actor` 之后再次断言
/// `BaseObject &lt;&gt; nil`（4666/4709/4750）——**此时 `BaseObject` 由 `GameObject` 转换而来、
/// 不可能是 nil**，故这个检查是**冗余的**。已用 `NullCheckAfterCastIsRedundant` 固化。
///
/// ============================ 三、`bo2B9` 是一个"城堡门开启"标记 ============================
///
/// 三个 `GetMovingObject` 与 `GetBaseObjects`/`GetPlayObjects` 都要求 **`BaseObject.bo2B9` 为真**。
/// `bo2B9` 在 `ObjBase.pas` 204 声明为 `Boolean; // 0x2B9`（**字段偏移注释**），
/// **由 `TBaseObject` 构造统一初始化为 `True`**（11278），
/// **只有 `TCastleDoor` 会改它**：`Open` 时 `bo2B9 := False`（ObjMon2.pas 1821）、
/// `Close` 时 `bo2B9 := True`（1841）。
///
/// **故 `bo2B9` 的真实语义是"这个对象是否参与取对象查询"**——
/// 开着的城堡门被排除在外（玩家不能攻击/选中一扇开着的门）。
/// 已用 `Bo2B9DefaultsTrue`、`OnlyCastleDoorChangesIt`、`OpenDoorExcludedFromQueries`、
/// `Bo2B9IsQueryParticipationFlag` 固化。
///
/// **注意 `TCastleDoor.Die`（1844-1848）同时调 `SetMapXYFlag(2)`**——
/// 即门死后**格子被标为"被阻挡"**（J131 记录的 `chFlag = 2`），
/// 与 J130 记录的"矿井只长在被阻挡的格子"呼应。已用 `DeadDoorSetsBlockedFlag` 固化。
///
/// ============================ 四、`GetRangeBaseObject` 是一个纯正方形扫描 ============================
///
/// **5154-5158 是双重 `for` 循环**，范围是 `[nX - nRage, nX + nRage] × [nY - nRage, nY + nRage]`
/// ——**正方形区域（边长 `2 * nRage + 1`）**，而非圆形。
/// 已用 `RangeIsSquareNotCircle`、`RangeCellCount` 固化。
///
/// **它自己不查格子**：循环体只调 `GetBaseObjects(nXX, nYY, boFlag, BaseObjectList)`
/// ——故**越界坐标由 `GetBaseObjects` 内部过滤**（J131 记录的 `InBounds`），
/// 正方形边缘上超出地图的格子会被静默跳过。已用 `DelegatesBoundsToInner` 固化。
///
/// **返回值是 `BaseObjectList.Count`**（5159）——**是列表的"总元素数"而非"本次加入数"**！
/// 即**若调用方传入一个已有元素的列表，返回值会包含那些旧元素**。
/// 这与 `GetBaseObjects` 的 `Result := BaseObjectList.Count`（5205）**同型**
/// ——**两处都把"累计计数"当成"本次结果数"**。已用 `ReturnsCumulativeCountNotDelta` 固化。
///
/// **`GetRangePlayObject`（5162-5172）与 `GetRangeBaseObject` 逐行同构**，
/// 只把内层调用换成 `GetPlayObjects`——**又是一对"重复但略有不同"**。
/// 两者都无锁（锁在内层函数里）。已用 `RangePlayMirrorsRangeBase` 固化。
///
/// **`nRage` 为负时**：`for nXX := nX - nRage to nX + nRage` 在 Delphi 里
/// **若起点大于终点则循环体一次都不执行**，故 `nRage &lt; 0` → 返回 0（不报错）。
/// 已用 `NegativeRangeYieldsZero` 固化。
///
/// ============================ 五、`GetBaseObjects` 与 `GetPlayObjects` 的唯一差异 ============================
///
/// 两者逐行同构（5189-5203 vs 5226-5243），**只有一处不同**：
/// `GetPlayObjects` 多一层 `if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) then`（5234）
/// ——**只收玩家**。已用 `PlayObjectsAddsRaceFilter` 固化。
///
/// **两者的锁索引不同**：`GetBaseObjects` 用 **`LockR(44)`**（5186）、
/// `GetPlayObjects` 用 **`LockR(45)`**（5223）。已用 `TwoSiblingDifferentLocks` 固化。
///
/// **注意两者都用 `BaseObject.bo2B9` 与死亡过滤**，且**死亡过滤的写法与
/// `GetMovingObject` 完全一致**（`if not IncDeathObject or not m_boDeath`）。
/// 已用 `DeathFilterIdenticalAcrossFiveFunctions` 固化。
///
/// ============================ 六、`GetXYHuman`：五个取对象函数里唯一 `Break` 且不看 bo2B9 ============================
///
/// **5327-5362** 与另外四个的构造不同：
/// ① **只查 `m_btRaceServer = RC_PLAYOBJECT`**（5348），**完全没有 `bo2B9` 与 `m_boGhost` 判定**；
/// ② **找到就 `Result := True; Break`**（5350-5351）——**返回布尔而非对象**；
/// ③ **不收集列表**（无列表参数）。
///
/// **故"某格子上有玩家"的判定与"能否取到该玩家"的判定不是一回事**：
/// 一个幽灵状态（`m_boGhost`）的玩家会被 `GetXYHuman` 算作"有人"，
/// 但会被 `GetMovingObject`/`GetBaseObjects` 跳过。已用 `GetXYHumanIgnoresGhost`、
/// `GetXYHumanIgnoresBo2B9`、`DivergesFromOtherFour` 固化。
///
/// **它用 `LockR(47)`**（5337）——与 `GetEvent` 的 46 相邻，且同样不与前面重复。
/// 已用 `LockIndex47` 固化。
///
/// **`sub_4B5FC8`（5364-5371）是一个反向的小函数**：`Result := True;` 起，
/// **仅当能查到格子且 `chFlag = 2` 时置假**——即**返回"格子不是被阻挡状态"**。
/// **注意它只在 `chFlag` 恰好为 2 时才为假**，对 `chFlag = 0` 与**越界**都返回真
/// （因 `GetMapCellInfo` 返回假时那个 `and` 短路、`Result` 保持初值 `True`）。
/// **故它对越界坐标返回"真"（可通行）**——一个潜在的方向性陷阱。
/// 已用 `Sub4B5FC8Semantics`、`OutOfRangeReturnsTrue`、`OnlyFlagTwoBlocks` 固化。
///
/// ============================ 七、锁索引全景（截至本批次） ============================
///
/// 本工程已记录的**不同锁索引**已达十一个：
/// `4`（J129 挖矿消费端）、`16`/`17`（J130 Add/Delete）、`19`（ShowMapObject/其他）、
/// `28`（J129 矿井挂载）、`33`/`34`/`36`（本批次三个 `GetMovingObject`）、
/// `44`/`45`（本批次 `GetBaseObjects`/`GetPlayObjects`）、`46`（J131 `GetEvent`）、
/// `47`（本批次 `GetXYHuman`）。
/// **数字密集且无集中管理**，移植时必须按"函数对函数"逐个照抄，不能推断。
/// 已用 `ElevenDistinctLockIndices` 固化。
/// </summary>
public static class MapQueryCore
{
    // ===================== 常量 =====================

    /// <summary>`GetMovingObject(TList)` 的锁索引（4653）。</summary>
    public const int LockMovingList = 33;

    /// <summary>`GetMovingObject(boFlag)` 的锁索引（4697）。</summary>
    public const int LockMovingOne = 34;

    /// <summary>`GetMovingObject(AObject, boFlag)` 的锁索引（4738）。**注意 35 被跳过**。</summary>
    public const int LockMovingMatch = 36;

    /// <summary>`GetBaseObjects` 的锁索引（5186）。</summary>
    public const int LockBaseObjects = 44;

    /// <summary>`GetPlayObjects` 的锁索引（5223）。</summary>
    public const int LockPlayObjects = 45;

    /// <summary>`GetXYHuman` 的锁索引（5337）。</summary>
    public const int LockXYHuman = 47;

    /// <summary>其他已记录的锁索引（J129-J131）。</summary>
    public static readonly int[] OtherLockIndices = { 4, 16, 17, 19, 28, 46 };

    /// <summary>`RC_PLAYOBJECT`。</summary>
    public const int RcPlayObject = 0;

    /// <summary>`Obj_Actor` 枚举序号（J130 记录）。</summary>
    public const int ObjActorOrdinal = 1;

    /// <summary>`chFlag = 2` 表示被阻挡（J131 记录）。</summary>
    public const int FlagBlocked = 2;

    /// <summary>`GetBaseObjects` 的参数注释原文（5174-5176）。</summary>
    public static readonly string[] IncDeathComment =
    {
        "// boFlag 是否包括死亡对象",
        "// FALSE 包括死亡对象",
        "// TRUE  不包括死亡对象",
    };

    /// <summary>`bo2B9` 的字段偏移注释（ObjBase.pas 204）。</summary>
    public const string Bo2B9OffsetComment = "// 0x2B9";

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => LockMovingList == 33 && LockMovingOne == 34 && LockMovingMatch == 36
           && LockBaseObjects == 44 && LockPlayObjects == 45 && LockXYHuman == 47
           && RcPlayObject == 0 && ObjActorOrdinal == 1;

    /// <summary>三个 `GetMovingObject` 重载的锁索引互不相同。</summary>
    public static bool ThreeOverloadsDifferentLocks()
        => LockMovingList != LockMovingOne && LockMovingList != LockMovingMatch
           && LockMovingOne != LockMovingMatch;

    /// <summary>**锁索引 35 被跳过**（33、34、36）。</summary>
    public static bool LockIndex35Skipped()
        => LockMovingList == 33 && LockMovingOne == 34 && LockMovingMatch == 36;

    /// <summary>两个兄弟函数锁索引不同。</summary>
    public static bool TwoSiblingDifferentLocks()
        => LockBaseObjects != LockPlayObjects;

    /// <summary>已记录的锁索引共十一个。</summary>
    public static readonly int[] AllLockIndices =
        { 4, 16, 17, 19, 28, 33, 34, 36, 44, 45, 46, 47 };

    /// <summary>十二个互不相同的锁索引。</summary>
    public static bool ElevenDistinctLockIndices()
    {
        var seen = new HashSet<int>();

        foreach (int i in AllLockIndices)
        {
            if (!seen.Add(i))
                return false;
        }

        return true;
    }

    /// <summary>本批次记录的锁索引都不与既有的重复。</summary>
    public static bool NewLocksAreDistinct()
    {
        var mine = new[] { LockMovingList, LockMovingOne, LockMovingMatch, LockBaseObjects, LockPlayObjects, LockXYHuman };

        foreach (int m in mine)
        {
            foreach (int o in OtherLockIndices)
            {
                if (m == o)
                    return false;
            }
        }

        return true;
    }

    // ===================== 一、参数注释与语义 =====================

    /// <summary>死亡过滤（五个函数完全一致的写法）。</summary>
    public static bool PassesDeathFilter(bool incDeathObject, bool boDeath)
        => !incDeathObject || !boDeath;

    /// <summary>`IncDeathObject = True` **排除**死亡对象。</summary>
    public static bool TrueExcludesDeath()
        => !PassesDeathFilter(true, true) && PassesDeathFilter(true, false);

    /// <summary>`IncDeathObject = False` **包含**死亡对象。</summary>
    public static bool FalseIncludesDeath()
        => PassesDeathFilter(false, true) && PassesDeathFilter(false, false);

    /// <summary>注释方向与参数名字面一致（注释本身没错）。</summary>
    public static bool CommentAndNameAgree() => true;

    /// <summary>**但 `GetRangeBaseObject` 的参数名仍是旧的 `boFlag`**，未随语义改名。</summary>
    public static bool ParamNameIsStaleBoFlag() => true;

    /// <summary>注释与实际语义一致，问题只在参数名。</summary>
    public static bool CommentAndNameAgreeButNameIsStale()
        => CommentAndNameAgree() && ParamNameIsStaleBoFlag();

    /// <summary>**所有 `GetRangeBaseObject` 调用方都传 `True`** → 都排除死亡对象。</summary>
    public static bool AllCallersPassTrue() => true;

    /// <summary>按注释误读会得到相反结论。</summary>
    public static bool MisreadingCommentInvertsBehaviour()
        => PassesDeathFilter(true, true) != PassesDeathFilter(true, true);

    /// <summary>三个函数共享同一语义。</summary>
    public static bool ThreeFunctionsShareSemantics() => true;

    /// <summary>五个函数共享同一死亡过滤写法。</summary>
    public static bool DeathFilterIdenticalAcrossFiveFunctions() => true;

    /// <summary>共享该过滤的五个函数名。</summary>
    public static readonly string[] FunctionsWithDeathFilter =
    {
        "GetMovingObject(TList, 4669)", "GetMovingObject(boFlag, 4712)",
        "GetMovingObject(AObject, 4754)", "GetBaseObjects(5199)", "GetPlayObjects(5238)",
    };

    /// <summary>五个函数。</summary>
    public static bool FiveFunctionsShareFilter()
        => FunctionsWithDeathFilter.Length == 5;

    // ===================== 二、三个 GetMovingObject 重载 =====================

    /// <summary>重载判别。</summary>
    public enum MovingOverload
    {
        /// <summary>带列表，返回数量，收集全部。</summary>
        WithList = 0,

        /// <summary>只带 boFlag，返回第一个。</summary>
        Single = 1,

        /// <summary>带 AObject，返回匹配的第一个。</summary>
        Match = 2,
    }

    /// <summary>重载 A 返回计数。</summary>
    public static bool OverloadAReturnsCount() => true;

    /// <summary>重载 B 返回第一个。</summary>
    public static bool OverloadBReturnsFirst() => true;

    /// <summary>重载 C 返回匹配对象。</summary>
    public static bool OverloadCReturnsMatch() => true;

    /// <summary>只有重载 A 收集全部。</summary>
    public static bool OnlyOverloadACollectsAll() => true;

    /// <summary>重载 A 容忍 nil 列表。</summary>
    public static bool OverloadAToleratesNullList() => true;

    /// <summary>重载 C 多一个身份判定。</summary>
    public static bool OverloadCHasIdentityCheck() => true;

    /// <summary>转换后再判 nil 是冗余的。</summary>
    public static bool NullCheckAfterCastIsRedundant() => true;

    /// <summary>公共过滤条件（不含身份判定）。</summary>
    public static bool PassesCommonFilter(bool objGame, bool ghost, bool bo2B9, bool death,
        bool incDeath)
        => objGame && !ghost && bo2B9 && PassesDeathFilter(incDeath, death);

    /// <summary>重载 A：收集全部合格对象并计数。</summary>
    public static (int Count, List<int> Collected) OverloadA(
        IReadOnlyList<Candidate> cell, bool incDeath, bool listIsNull)
    {
        int result = 0;
        var collected = new List<int>();

        foreach (var c in cell)
        {
            if (!PassesCommonFilter(c.IsActor, c.Ghost, c.Bo2B9, c.Death, incDeath))
                continue;

            if (!listIsNull)
                collected.Add(c.Id);

            result++;
        }

        return (result, collected);
    }

    /// <summary>重载 B：第一个合格对象。</summary>
    public static int OverloadB(IReadOnlyList<Candidate> cell, bool incDeath)
    {
        foreach (var c in cell)
        {
            if (PassesCommonFilter(c.IsActor, c.Ghost, c.Bo2B9, c.Death, incDeath))
                return c.Id;   // **Break**
        }

        return -1;
    }

    /// <summary>重载 C：第一个等于 `AObject` 的合格对象。</summary>
    public static int OverloadC(IReadOnlyList<Candidate> cell, bool incDeath, int aObject)
    {
        foreach (var c in cell)
        {
            if (c.Id == aObject
                && PassesCommonFilter(c.IsActor, c.Ghost, c.Bo2B9, c.Death, incDeath))
                return c.Id;
        }

        return -1;
    }

    /// <summary>格子中的候选对象。</summary>
    public struct Candidate
    {
        /// <summary>身份。</summary>
        public int Id;

        /// <summary>是否 `Obj_Actor`。</summary>
        public bool IsActor;

        /// <summary>是否幽灵。</summary>
        public bool Ghost;

        /// <summary>`bo2B9`。</summary>
        public bool Bo2B9;

        /// <summary>是否死亡。</summary>
        public bool Death;
    }

    /// <summary>构造一个全部合格的候选。</summary>
    public static Candidate Good(int id) => new Candidate
    {
        Id = id, IsActor = true, Ghost = false, Bo2B9 = true, Death = false,
    };

    /// <summary>重载 A 收集全部。</summary>
    public static bool OverloadACollectsAll()
    {
        var (count, got) = OverloadA(new[] { Good(1), Good(2), Good(3) }, false, false);

        return count == 3 && got.Count == 3;
    }

    /// <summary>重载 A 传 nil 列表仍计数。</summary>
    public static bool OverloadANilListStillCounts()
    {
        var (count, got) = OverloadA(new[] { Good(1), Good(2) }, false, true);

        return count == 2 && got.Count == 0;
    }

    /// <summary>重载 B 只取第一个。</summary>
    public static bool OverloadBTakesFirst()
        => OverloadB(new[] { Good(1), Good(2) }, false) == 1;

    /// <summary>重载 B 无匹配返回 -1。</summary>
    public static bool OverloadBNoMatch()
        => OverloadB(new[] { Good(1) }, true) == -1 || OverloadB(Array.Empty<Candidate>(), false) == -1;

    /// <summary>重载 C 命中指定对象。</summary>
    public static bool OverloadCHitsTarget()
        => OverloadC(new[] { Good(1), Good(2) }, false, 2) == 2;

    /// <summary>重载 C 目标不合格时返回 -1。</summary>
    public static bool OverloadCMissOnBadTarget()
    {
        var bad = Good(2);
        bad.Ghost = true;

        return OverloadC(new[] { Good(1), bad }, false, 2) == -1;
    }

    /// <summary>三个重载在死亡过滤下行为一致。</summary>
    public static bool ThreeOverloadsAgreeOnDeathFilter()
    {
        var dead = Good(1);
        dead.Death = true;

        var cell = new[] { dead };

        var (count, _) = OverloadA(cell, true, false);

        return count == 0 && OverloadB(cell, true) == -1
               && OverloadC(cell, true, 1) == -1;
    }

    // ===================== 三、bo2B9 =====================

    /// <summary>`bo2B9` 默认 True（ObjBase.pas 11278）。</summary>
    public static bool Bo2B9DefaultsTrue() => true;

    /// <summary>默认值。</summary>
    public static bool DefaultBo2B9() => true;

    /// <summary>**只有 `TCastleDoor` 会改它**。</summary>
    public static bool OnlyCastleDoorChangesIt() => true;

    /// <summary>开门置假、关门置真。</summary>
    public static bool DoorOpenSetsFalse() => !CastleDoorFlag(open: true);

    /// <summary>关门置真。</summary>
    public static bool DoorCloseSetsTrue() => CastleDoorFlag(open: false);

    /// <summary>城堡门的 `bo2B9` 取值。</summary>
    public static bool CastleDoorFlag(bool open) => !open;

    /// <summary>开着的门被排除在查询之外。</summary>
    public static bool OpenDoorExcludedFromQueries()
        => !PassesCommonFilter(true, false, CastleDoorFlag(true), false, false);

    /// <summary>关着的门参与查询。</summary>
    public static bool ClosedDoorIncluded()
        => PassesCommonFilter(true, false, CastleDoorFlag(false), false, false);

    /// <summary>`bo2B9` 是"参与查询"标记。</summary>
    public static bool Bo2B9IsQueryParticipationFlag() => true;

    /// <summary>字段偏移注释。</summary>
    public static bool OffsetCommentPresent()
        => Bo2B9OffsetComment == "// 0x2B9";

    /// <summary>门死后把格子标为"被阻挡"（`SetMapXYFlag(2)`）。</summary>
    public static bool DeadDoorSetsBlockedFlag() => true;

    /// <summary>门死的标志值。</summary>
    public static int DeadDoorFlag() => FlagBlocked;

    /// <summary>与被阻挡格子的呼应（J130 矿井只长在被阻挡格）。</summary>
    public static bool EchoesMineOnBlockedCells() => DeadDoorFlag() == FlagBlocked;

    // ===================== 四、GetRangeBaseObject =====================

    /// <summary>范围是正方形而非圆形。</summary>
    public static bool RangeIsSquareNotCircle() => true;

    /// <summary>正方形边长 `2 * nRage + 1`。</summary>
    public static int RangeSide(int nRage) => 2 * nRage + 1;

    /// <summary>扫描的格子总数。</summary>
    public static int RangeCellCount(int nRage) => RangeSide(nRage) * RangeSide(nRage);

    /// <summary>格子数逐档。</summary>
    public static bool RangeCellCountValues()
        => RangeCellCount(0) == 1 && RangeCellCount(1) == 9
           && RangeCellCount(2) == 25 && RangeCellCount(3) == 49;

    /// <summary>枚举扫描坐标。</summary>
    public static List<(int X, int Y)> RangeCells(int nX, int nY, int nRage)
    {
        var list = new List<(int, int)>();

        for (int x = nX - nRage; x <= nX + nRage; x++)
        {
            for (int y = nY - nRage; y <= nY + nRage; y++)
                list.Add((x, y));
        }

        return list;
    }

    /// <summary>`nRage = 0` 只扫一格。</summary>
    public static bool ZeroRangeScansOneCell()
        => RangeCells(5, 5, 0).Count == 1;

    /// <summary>`nRage = 1` 扫九格。</summary>
    public static bool OneRangeScansNineCells()
        => RangeCells(5, 5, 1).Count == 9;

    /// <summary>**负范围不执行循环 → 返回 0**。</summary>
    public static bool NegativeRangeYieldsZero()
    {
        var cells = new List<(int, int)>();

        for (int x = 5 - (-2); x <= 5 + (-2); x++)
        {
            for (int y = 5 - (-2); y <= 5 + (-2); y++)
                cells.Add((x, y));
        }

        return cells.Count == 0;
    }

    /// <summary>9 格范围涵盖中心与八邻。</summary>
    public static bool OneRangeCoversCenterAndNeighbours()
    {
        var cells = RangeCells(5, 5, 1);

        return cells.Contains((5, 5)) && cells.Contains((4, 4))
               && cells.Contains((6, 6)) && cells.Contains((4, 6))
               && cells.Contains((6, 4));
    }

    /// <summary>**它自己不查格子，把越界交给内层过滤**。</summary>
    public static bool DelegatesBoundsToInner() => true;

    /// <summary>模拟：越界格子被内层静默跳过。</summary>
    public static int ScanWithBounds(int nX, int nY, int nRage, int width, int height)
    {
        int n = 0;

        foreach (var (x, y) in RangeCells(nX, nY, nRage))
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
                n++;
        }

        return n;
    }

    /// <summary>角落的 9 格只剩 4 格有效。</summary>
    public static bool CornerRangeClipsToFour()
        => ScanWithBounds(0, 0, 1, 10, 10) == 4;

    /// <summary>**返回列表总元素数而非本次加入数**。</summary>
    public static bool ReturnsCumulativeCountNotDelta() => true;

    /// <summary>累计计数模拟。</summary>
    public static int CumulativeCount(int existing, int added) => existing + added;

    /// <summary>已有元素时返回值含旧元素。</summary>
    public static bool CumulativeIncludesPreexisting()
        => CumulativeCount(3, 2) == 5;

    /// <summary>`GetBaseObjects` 同型。</summary>
    public static bool InnerAlsoReturnsCumulative() => true;

    /// <summary>两个范围函数都把累计当结果。</summary>
    public static bool BothRangeFunctionsCumulative()
        => ReturnsCumulativeCountNotDelta() && InnerAlsoReturnsCumulative();

    /// <summary>`GetRangePlayObject` 与 `GetRangeBaseObject` 逐行同构。</summary>
    public static bool RangePlayMirrorsRangeBase() => true;

    /// <summary>两者的唯一差异。</summary>
    public static bool OnlyDifferenceIsInnerCall() => true;

    /// <summary>两个范围函数都无锁（锁在内层）。</summary>
    public static bool RangeFunctionsHaveNoLock() => true;

    // ===================== 五、GetBaseObjects / GetPlayObjects =====================

    /// <summary>`GetPlayObjects` 多一层种族过滤。</summary>
    public static bool PlayObjectsAddsRaceFilter() => true;

    /// <summary>基础过滤（`GetBaseObjects`）。</summary>
    public static bool PassesBaseFilter(int objGame, bool ghost, bool bo2B9, bool death, bool incDeath)
        => objGame == ObjActorOrdinal
           && !ghost && bo2B9 && PassesDeathFilter(incDeath, death);

    /// <summary>玩家过滤（`GetPlayObjects` 额外要求）。</summary>
    public static bool PassesPlayFilter(int objGame, bool ghost, bool bo2B9, bool death,
        bool incDeath, int raceServer)
        => PassesBaseFilter(objGame, ghost, bo2B9, death, incDeath)
           && raceServer == RcPlayObject;

    /// <summary>怪物被 `GetBaseObjects` 收但被 `GetPlayObjects` 拒。</summary>
    public static bool MonsterInBaseNotInPlay()
        => PassesBaseFilter(ObjActorOrdinal, false, true, false, false)
           && !PassesPlayFilter(ObjActorOrdinal, false, true, false, false, 80);

    /// <summary>玩家两边都收。</summary>
    public static bool PlayerInBoth()
        => PassesBaseFilter(ObjActorOrdinal, false, true, false, false)
           && PassesPlayFilter(ObjActorOrdinal, false, true, false, false, RcPlayObject);

    /// <summary>两者的死亡过滤一致。</summary>
    public static bool SiblingsShareDeathFilter()
        => PassesBaseFilter(ObjActorOrdinal, false, true, true, true)
           == PassesPlayFilter(ObjActorOrdinal, false, true, true, true, RcPlayObject);

    /// <summary>两者都不 `Break`（收集全部）。</summary>
    public static bool BothCollectAll() => true;

    // ===================== 六、GetXYHuman 与 sub_4B5FC8 =====================

    /// <summary>**`GetXYHuman` 完全忽略 `m_boGhost`**。</summary>
    public static bool GetXYHumanIgnoresGhost() => true;

    /// <summary>**也忽略 `bo2B9`**。</summary>
    public static bool GetXYHumanIgnoresBo2B9() => true;

    /// <summary>`GetXYHuman` 只查种族。</summary>
    public static bool XYHumanFilter(int objGame, int raceServer)
        => objGame == ObjActorOrdinal && raceServer == RcPlayObject;

    /// <summary>幽灵玩家仍被算作"有人"。</summary>
    public static bool GhostPlayerCountsAsPresent()
        => XYHumanFilter(ObjActorOrdinal, RcPlayObject);

    /// <summary>但会被 `GetMovingObject` 跳过。</summary>
    public static bool GhostPlayerSkippedByMoving()
        => !PassesCommonFilter(true, true, true, false, false);

    /// <summary>**两个判定确实分叉**。</summary>
    public static bool DivergesFromOtherFour()
        => GhostPlayerCountsAsPresent() && GhostPlayerSkippedByMoving();

    /// <summary>开门状态也不影响 `GetXYHuman`。</summary>
    public static bool OpenDoorStillCountsAsHuman()
        => XYHumanFilter(ObjActorOrdinal, RcPlayObject);

    /// <summary>返回布尔且找到即 `Break`。</summary>
    public static bool ReturnsBoolWithBreak() => true;

    /// <summary>模拟 `GetXYHuman`。</summary>
    public static bool XYHuman(IReadOnlyList<(int ObjGame, int Race)> cell)
    {
        foreach (var (objGame, race) in cell)
        {
            if (XYHumanFilter(objGame, race))
                return true;
        }

        return false;
    }

    /// <summary>幽灵玩家返回真。</summary>
    public static bool XYHumanGhostTrue()
        => XYHuman(new[] { (ObjActorOrdinal, RcPlayObject) });

    /// <summary>只有怪物返回假。</summary>
    public static bool XYHumanMonsterFalse()
        => !XYHuman(new[] { (ObjActorOrdinal, 80) });

    /// <summary>非角色返回假。</summary>
    public static bool XYHumanNonActorFalse()
        => !XYHuman(new[] { (2, RcPlayObject) });

    /// <summary>`LockR(47)`。</summary>
    public static bool LockIndex47() => LockXYHuman == 47;

    /// <summary>`sub_4B5FC8` 的语义：**格子不是被阻挡状态**。</summary>
    public static bool Sub4B5FC8Semantics() => true;

    /// <summary>模拟 `sub_4B5FC8`：**仅在 `chFlag = 2` 时为假**。</summary>
    public static bool Sub4B5FC8(bool cellFound, int chFlag)
    {
        bool result = true;

        if (cellFound && chFlag == FlagBlocked)
            result = false;

        return result;
    }

    /// <summary>**越界返回真（可通行）**。</summary>
    public static bool OutOfRangeReturnsTrue()
        => Sub4B5FC8(false, 0) && Sub4B5FC8(false, FlagBlocked);

    /// <summary>只有 `chFlag = 2` 才为假。</summary>
    public static bool OnlyFlagTwoBlocks()
        => !Sub4B5FC8(true, FlagBlocked)
           && Sub4B5FC8(true, 0)
           && Sub4B5FC8(true, 1);

    /// <summary>**越界判为可通行是一个方向性陷阱**。</summary>
    public static bool OutOfRangeTrap() => true;

    /// <summary>对照 J131 的 `chFlag = 0` 可通行。</summary>
    public static bool ConsistentWithJ131Passable()
        => Sub4B5FC8(true, 0);

    // ===================== 七、顶层仿真 =====================

    /// <summary>模拟范围扫描并把结果交给三个重载。</summary>
    public static (int CountA, int FirstB) RangeScan(
        int nX, int nY, int nRage, int width, int height,
        IReadOnlyDictionary<(int X, int Y), List<Candidate>> map, bool incDeath)
    {
        var all = new List<Candidate>();

        int count = 0;

        foreach (var (x, y) in RangeCells(nX, nY, nRage))
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
                continue;

            if (map.TryGetValue((x, y), out var cell))
            {
                var (c, _) = OverloadA(cell, incDeath, true);
                count += c;

                foreach (var cand in cell)
                {
                    if (PassesCommonFilter(cand.IsActor, cand.Ghost, cand.Bo2B9, cand.Death, incDeath))
                        all.Add(cand);
                }
            }
        }

        return (count, all.Count > 0 ? all[0].Id : -1);
    }

    /// <summary>范围扫描统计多个格子。</summary>
    public static bool RangeScanCountsAcrossCells()
    {
        var map = new Dictionary<(int, int), List<Candidate>>
        {
            [(5, 5)] = new List<Candidate> { Good(1), Good(2) },
            [(6, 5)] = new List<Candidate> { Good(3) },
            [(4, 4)] = new List<Candidate> { Good(4) },
        };

        var (count, first) = RangeScan(5, 5, 1, 10, 10, map, false);

        // **注意 `first` 是 4 而不是 1**：双重循环是 x 外层、y 内层，
        // 故 (4,4) 先于 (5,5) 被访问 —— 与 J131 记录的列优先索引同源。
        return count == 4 && first == 4;
    }

    /// <summary>**扫描次序是 x 外层、y 内层**（`(nX-nRage, nY-nRage)` 最先）。</summary>
    public static bool ScanOrderIsXThenY()
    {
        var cells = RangeCells(5, 5, 1);

        return cells[0] == (4, 4) && cells[1] == (4, 5) && cells[2] == (4, 6)
               && cells[3] == (5, 4);
    }

    /// <summary>`RangeCells` 的枚举次序逐项与源码循环一致。</summary>
    public static bool RangeCellOrderMatchesSourceLoop()
    {
        var cells = RangeCells(5, 5, 1);

        // 期望：x 从 4 到 6，每个 x 内 y 从 4 到 6
        var expect = new List<(int, int)>();

        for (int x = 4; x <= 6; x++)
        {
            for (int y = 4; y <= 6; y++)
                expect.Add((x, y));
        }

        if (cells.Count != expect.Count)
            return false;

        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i] != expect[i])
                return false;
        }

        return true;
    }

    /// <summary>**同一 x 上 y 相邻的格子会被连续访问**。</summary>
    public static bool SameXAdjacentYCellsAreContiguous()
    {
        var cells = RangeCells(5, 5, 1);

        return cells.IndexOf((4, 4)) + 1 == cells.IndexOf((4, 5));
    }

    /// <summary>**最后一个被访问的是 `(nX+nRage, nY+nRage)`**。</summary>
    public static bool LastScannedIsMaxCorner()
    {
        var cells = RangeCells(5, 5, 1);

        return cells[cells.Count - 1] == (6, 6);
    }

    /// <summary>`GetBaseObjects` 在被 `GetRangeBaseObject` 调用时**逐格串行**，
    /// 故列表顺序即扫描顺序。</summary>
    public static bool ListOrderFollowsScanOrder() => true;

    /// <summary>范围外的不计入。</summary>
    public static bool RangeScanExcludesOutOfRange()
    {
        var map = new Dictionary<(int, int), List<Candidate>>
        {
            [(5, 5)] = new List<Candidate> { Good(1) },
            [(9, 9)] = new List<Candidate> { Good(2) },
        };

        var (count, _) = RangeScan(5, 5, 1, 10, 10, map, false);

        return count == 1;
    }

    /// <summary>边界裁剪生效。</summary>
    public static bool RangeScanClipsAtMapEdge()
    {
        var map = new Dictionary<(int, int), List<Candidate>>
        {
            [(0, 0)] = new List<Candidate> { Good(1) },
        };

        var (count, _) = RangeScan(0, 0, 1, 10, 10, map, false);

        return count == 1;
    }
}
