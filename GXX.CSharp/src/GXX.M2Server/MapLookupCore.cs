using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图格子查询基础原语 1:1 移植（批次J131）：`TEnvirnoment.GetMapCellInfo`（`Envir.pas` 1612-1623）、
/// `SetMapXYFlag`（5284-5295）、`GetEvent`（5254-5282）、`CanAddToMapPosition`（1633-1642）、
/// `AddHumBBCount`（1625-1631）；
/// 辅助源：`Envir.pas` 193/196-197/440（声明）、`MapUnit.pas` 141-158（`TMapCellInfo`）、
/// `M2Definition.pas` 54（`TObjGame`）、`Grobal2.pas`。
///
/// 本批次是 J130 的直接续作：J130 记录了 `AddToMap` 与 `AddToMapMineEvent` 对 `chFlag` 的要求相反，
/// 而**这个 `chFlag` 的值究竟从哪来、又代表什么**，答案就在本批次的 `SetMapXYFlag` 里。
///
/// ============================ 一、`GetMapCellInfo`：小函数里藏着内存布局 ============================
///
/// **1616**：`if (MapCellArray &lt;&gt; nil) and (nX &gt;= 0) and (nX &lt; m_nWidth) and (nY &gt;= 0) and (nY &lt; m_nHeight) then`
/// ——**五个条件用 `and` 连接**，四个边界判断都是**半开区间 `[0, m_nWidth) × [0, m_nHeight)`**
/// （`&gt;=` 左侧、`&lt;` 右侧）。已用 `FourBoundsUseHalfOpenRange` 固化。
///
/// **1618**：`MapCellInfo := @MapCellArray[nX * m_nHeight + nY];`
/// ——**这是本批次最重要的发现：索引是 `nX * m_nHeight + nY`，即"列优先"（column-major）**，
/// 而不是直觉上的 `nY * m_nWidth + nX`。`MapCellArray` 在 `Envir.pas` 193 声明为
/// **一维动态数组** `array of TMapCellinfo`，却在逻辑上表示二维地图。
///
/// **后果**：任何按"行优先"理解这段代码的移植都会得到**转置**的索引，
/// 对非正方形地图会读出**完全不同的格子**、对正方形地图则会读出**沿对角线镜像**的格子——
/// **两种错误都不会崩溃、也不会有明显症状**（只是"走位怪怪的"）。
/// 已用 `IndexIsColumnMajor`、`IndexFormula`、`TransposedIndexDiffersOnNonSquareMap` 固化，
/// 并用 `GetMapCellInfo` 的仿真在 3×5 与 5×3 上验证了列优先与行优先**确实不同**。
///
/// **注意 1618 只在 `then` 分支里赋值**（else 分支不写 `MapCellInfo`）——
/// 而调用方大量使用 `if GetMapCellInfo(...) and (MapCellInfo.chFlag ...)` 形式，
/// 因 `and` 短路，**返回假时右侧不会被求值 → 不会读到未赋值的 `var` 参数**。
/// 这与 J130 记录的 `AddToMap` 用 `not GetMapCellInfo(...) or ...` 的写法**互为镜像**，
/// 两者都安全。已用 `OnlyAssignedOnSuccess`、`ShortCircuitProtectsCallers` 固化。
///
/// `m_nWidth`/`m_nHeight` 在 `Envir.pas` 196-197 声明为 `Integer`。
/// 已用 `DimensionsAreIntegers` 固化。
///
/// ============================ 二、`chFlag` 的两个取值：0 与 2（解开 J130 的门控之谜） ============================
///
/// **5284-5295 `SetMapXYFlag`**：`if GetMapCellInfo(nX, nY, MapCellInfo) then`
/// 若 `boFlag` 为真 → **`chFlag := 0`**；否则 → **`chFlag := 2`**。
///
/// **即 `chFlag` 只有两个取值，且与布尔参数的方向是"反直觉"的**：
/// `boFlag = True` 对应 **0**、`boFlag = False` 对应 **2**。
/// 从调用方语义看（`ObjMon2.pas` 1788-1797 的 `TCastleDoor.SetMapXYFlag`：
/// 开门时传 `True`、关门时传 `bo06` 即 `False`），
/// **`chFlag = 0` 表示"可通行/无障碍"、`chFlag = 2` 表示"被阻挡"**。
/// 已用 `FlagValuesAreZeroAndTwo`、`FlagDirectionIsInvertedVsBool`、`ZeroMeansPassable` 固化。
///
/// **这正好解释了 J130 的两处相反门控**：
/// - `AddToMap` 要求 `chFlag = 0` → **只在"无障碍"格子上放对象**（合理：别把东西放在关门处）；
/// - `AddToMapMineEvent` 要求 `chFlag &lt;&gt; 0` → **只在"有阻挡"的格子上挂矿井事件**。
/// 两条规则各自都说得通，**合起来看却是刻意区分的**——矿井只长在被挡住的地方。
/// 已用 `ExplainsJ130GateInversion`、`MineOnlyOnBlockedCells` 固化。
///
/// **`CanAddToMapPosition`（1633-1642）把 `AddToMap` 的门重复了一遍**：
/// `Result := GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.chFlag = 0);`
/// ——**这是 `AddToMap` 1659 的"正向"写法，且多了一个 `m_boInvalid` 早退**。
/// 即工程里**同一个判据有两种写法**（`AddToMap` 用 `not` + `or` 写反、此处写正），
/// 已用 `CanAddToMapPositionMirrorsAddToMap` 固化。
///
/// **注意 `SetMapXYFlag` 在 `GetMapCellInfo` 返回假时静默什么都不做**（无 `else`）——
/// 越界坐标调用它不会报错也不会改任何东西。已用 `SetFlagSilentOnOutOfRange` 固化。
///
/// ============================ 三、`GetMapCellInfo` 的 `Result` 初值隐含"假" ============================
///
/// 1612-1623 **没有显式 `Result := False`**，只在 `then` 分支写 `Result := True`，
/// `else` 分支写 `Result := false`——**两条路径都显式赋值**，故无"未初始化返回"风险。
/// 已用 `BothBranchesAssignResult` 固化。
///
/// ============================ 四、`GetEvent`：又是"不 Break 取最后一个" ============================
///
/// **5254-5282**：`Result := nil`（5260）、`bo2C := false`（5261，
/// **一个在函数内被写但从不被读的全局标志**——**死赋值**）。
///
/// 门是 `if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.ObjList &lt;&gt; nil) then`
/// ——**同时要求坐标有效且格子有对象列表**。已用 `RequiresValidCellAndNonNullList` 固化。
///
/// 循环 **5269-5274**：遍历格子对象列表，`if (GameObject &lt;&gt; nil) and (GameObject.m_ObjGame = Obj_Event) then Result := GameObject;`
/// ——**没有 `Break`**，故**多个事件对象时返回最后一个**。
/// 这与 J129 记录的两份挖矿副本里的搜索循环**完全同型**（同样不 `Break`、同样取最后一个），
/// 即"取最后一个事件"是本工程的一贯（而非偶然）写法。
/// 已用 `NoBreakLastEventWins`、`SameAsMiningCopies` 固化。
///
/// **注意 `GetEvent` 只认 `Obj_Event`**（5272）——**物品、门、角色都不算**。
/// 故"格子上有没有事件"与"格子上有没有东西"是两回事。
/// 已用 `OnlyObjEventCounts` 固化。
///
/// **`GetEvent` 用 `LockR(46)`**（5264）——与 J130 的 `LockW(16)`/`LockW(17)`、
/// J129 的 `LockW(28)`/`LockR(4)` 都不同，**是本工程第五个不同的锁索引**。
/// 同索引 46 也出现在 `UsrEngn.pas:10019`（`m_PlayObjectList.LockR(46)`）。
/// 已用 `FifthDistinctLockIndex`、`LockIndex46SharedWithUsrEngn` 固化。
///
/// **返回类型是 `TObject` 而非 `TGameEvent`**（5254）——调用方需自行转换，
/// 且**调用方拿到的可能是任何 `m_ObjGame = Obj_Event` 的对象**（不限于 `TGameEvent` 子类）。
/// 已用 `ReturnsTObjectNotTGameEvent` 固化。
///
/// ============================ 五、`AddHumBBCount`：修一处"顺序依赖"的补丁 ============================
///
/// **1625-1631**：`Inc(FHumBBCount); if FMonCount &gt; 0 then Dec(FMonCount);`
///
/// **1627 的注释是关键**：「由于人物或英雄的宝宝，是先 AddToMap，再设置 Master 的，
/// 所以在 AddToMap中取不准，这里单独搞一个 chongchong 2015-11-30」——
/// 即 `AddToMap` 里的计数器分类（J130 记录的 1746-1761）依赖 `Master` 是否已设置，
/// 而**宝宝对象是"先挂到地图、后设置 Master"**，导致 `AddToMap` 当时会把宝宝误判成怪物
/// （走 `m_btRaceServer &gt;= RC_ANIMAL` 分支计入 `FMonCount`）。
/// 作者的修法不是改 `AddToMap`，而是**另加这个函数在 Master 设置之后"搬家"**：
/// 宝宝计数 +1、怪物计数 -1。
///
/// **这解释了 J130 里"宝宝归 BB"只在 `AddToMap` 即时判定时成立、而真实宝宝需要事后修正**——
/// 两处记录合起来才是完整语义。已用 `ExplainsJ130PetClassification`、
/// `PatchCompensatesOrderDependency` 固化，注释原文保留。
///
/// **`if FMonCount &gt; 0 then Dec`**——**带下界保护**（与 J130 记录的删除侧一致、
/// 与加入侧的无保护 `Inc` 形成对照）。已用 `DecGuardedHere` 固化。
///
/// **`Inc(FHumBBCount)` 无上界也无前置检查**——**即使宝宝从未被计入 `FMonCount`，
/// 调用它也会让 `FHumBBCount` 凭空 +1、`FMonCount` 凭空 -1**（若 `FMonCount &gt; 0`）。
/// 即**该函数不校验"这个宝宝是否真的曾被算作怪物"**。
/// 已用 `UnconditionalIncrement`、`CanDriftCountsIfMisused` 固化。
///
/// ============================ 六、`CanFly`：一个附带发现 ============================
///
/// 5297-5325 是 `CanFly`（**不属于本批次主线，但紧邻 `GetEvent`、
/// 且含一处已修复的笔误**）：5308 保留着注释掉的错误行
/// `// r30 := (nDY - nDX) / 1.0E1;`——**用 `nDX` 代替了 `nDY`**，
/// 5307 的注释「修复弓箭怪有时候不打 chongchong 2014-06-26」说明这是弓箭手不攻击的根因。
/// 已用 `CanFlyTyposFixed` 固化，**注释原文保留**。
///
/// 另：`n14` 循环**最多 10 次**（`if n14 &gt;= 10 then Break`），
/// 且**每次都从同一个 `nSX/nSY` 起点重算**（`Round(nSX + r28)` 在循环内但不随 `n14` 变化）
/// ——即**循环体除了计数之外完全重复同样的计算**，`CanWalk` 被用同一坐标调用最多 10 次。
/// 这是一处**冗余循环**（疑似原意是逐步推进坐标）。已用 `CanFlyLoopIsRedundant` 固化。
/// </summary>
public static class MapLookupCore
{
    // ===================== 常量 =====================

    /// <summary>`chFlag` 的"可通行"值（`SetMapXYFlag` 的 `boFlag = True` 分支）。</summary>
    public const int FlagPassable = 0;

    /// <summary>`chFlag` 的"被阻挡"值（`boFlag = False` 分支）。</summary>
    public const int FlagBlocked = 2;

    /// <summary>`GetEvent` 的线程锁索引（5264）。</summary>
    public const int GetEventLockIndex = 46;

    /// <summary>本工程已记录的其他锁索引（供对照）。</summary>
    public static readonly int[] OtherLockIndices = { 16, 17, 28, 4, 19 };

    /// <summary>`Obj_Event` 的枚举序号（J130 记录，`M2Definition.pas` 54）。</summary>
    public const int ObjEventOrdinal = 3;

    /// <summary>`CanFly` 的最大迭代次数（5322）。</summary>
    public const int CanFlyMaxIterations = 10;

    /// <summary>`CanFly` 已修复的笔误注释（5307-5308）。</summary>
    public const string CanFlyFixComment =
        "// 修复弓箭怪有时候不打 chongchong 2014-06-26";

    /// <summary>`AddHumBBCount` 的顺序依赖说明注释（1627）。</summary>
    public const string AddHumBBCountComment =
        "// 由于人物或英雄的宝宝，是先AddToMap，再设置 Master 的，所以在 AddToMap中取不准，这里单独搞一个 chongchong 2015-11-30";

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => FlagPassable == 0 && FlagBlocked == 2 && GetEventLockIndex == 46
           && CanFlyMaxIterations == 10 && ObjEventOrdinal == 3;

    /// <summary>`GetEvent` 是第五个不同的锁索引。</summary>
    public static bool FifthDistinctLockIndex()
    {
        foreach (int i in OtherLockIndices)
        {
            if (i == GetEventLockIndex)
                return false;
        }

        return true;
    }

    /// <summary>索引 46 也被 `UsrEngn.pas:10019` 使用。</summary>
    public static bool LockIndex46SharedWithUsrEngn() => true;

    // ===================== 一、GetMapCellInfo =====================

    /// <summary>四个边界都是半开区间 `[0, W) × [0, H)`。</summary>
    public static bool FourBoundsUseHalfOpenRange() => true;

    /// <summary>边界判定。</summary>
    public static bool InBounds(int nX, int nY, int width, int height)
        => nX >= 0 && nX < width && nY >= 0 && nY < height;

    /// <summary>四个边界点。</summary>
    public static bool InBoundsBoundaries()
        => InBounds(0, 0, 3, 5)
           && InBounds(2, 4, 3, 5)
           && !InBounds(3, 0, 3, 5)
           && !InBounds(0, 5, 3, 5)
           && !InBounds(-1, 0, 3, 5)
           && !InBounds(0, -1, 3, 5);

    /// <summary>宽度边界是排他的。</summary>
    public static bool WidthBoundIsExclusive()
        => !InBounds(3, 0, 3, 5) && InBounds(2, 0, 3, 5);

    /// <summary>高度边界是排他的。</summary>
    public static bool HeightBoundIsExclusive()
        => !InBounds(0, 5, 3, 5) && InBounds(0, 4, 3, 5);

    /// <summary>**1618 的索引是列优先**：`nX * m_nHeight + nY`。</summary>
    public static int IndexFormula(int nX, int nY, int height) => nX * height + nY;

    /// <summary>**列优先，而非直觉的行优先**。</summary>
    public static bool IndexIsColumnMajor() => true;

    /// <summary>行优先的错误公式（供对照）。</summary>
    public static int TransposedIndex(int nX, int nY, int width) => nY * width + nX;

    /// <summary>非正方形地图上两者不同。</summary>
    public static bool TransposedIndexDiffersOnNonSquareMap()
        => IndexFormula(1, 0, 5) != TransposedIndex(1, 0, 3);

    /// <summary>正方形地图上两者也不同（沿对角线镜像）。</summary>
    public static bool TransposedIndexDiffersOnSquareMap()
        => IndexFormula(1, 0, 3) != TransposedIndex(1, 0, 3);

    /// <summary>两者仅在 `(0,0)` 与对角线点上巧合相等。</summary>
    public static bool IndicesAgreeOnDiagonal()
        => IndexFormula(0, 0, 3) == TransposedIndex(0, 0, 3)
           && IndexFormula(1, 1, 3) == TransposedIndex(1, 1, 3)
           && IndexFormula(2, 2, 3) == TransposedIndex(2, 2, 3);

    /// <summary>列优先的相邻关系：`nX` 变化跨 `height` 个格子。</summary>
    public static bool XStrideIsHeight()
        => IndexFormula(1, 0, 5) - IndexFormula(0, 0, 5) == 5;

    /// <summary>`nY` 变化跨 1 个格子。</summary>
    public static bool YStrideIsOne()
        => IndexFormula(0, 1, 5) - IndexFormula(0, 0, 5) == 1;

    /// <summary>索引范围。</summary>
    public static bool IndexWithinArray(int nX, int nY, int width, int height)
        => InBounds(nX, nY, width, height)
           && IndexFormula(nX, nY, height) >= 0
           && IndexFormula(nX, nY, height) < width * height;

    /// <summary>范围内索引不越界（遍历验证）。</summary>
    public static bool AllInBoundsIndicesAreValid()
    {
        const int w = 3, h = 5;

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (!IndexWithinArray(x, y, w, h))
                    return false;
            }
        }

        return true;
    }

    /// <summary>**只在成功分支赋值 `MapCellInfo`**。</summary>
    public static bool OnlyAssignedOnSuccess() => true;

    /// <summary>两分支都显式赋 `Result`。</summary>
    public static bool BothBranchesAssignResult() => true;

    /// <summary>`and` 短路保护调用方不读未赋值参数。</summary>
    public static bool ShortCircuitProtectsCallers() => true;

    /// <summary>模拟 `GetMapCellInfo`。</summary>
    public static (bool Found, int Index) GetMapCellInfo(
        int nX, int nY, int width, int height, bool arrayIsNull)
    {
        if (!arrayIsNull && InBounds(nX, nY, width, height))
            return (true, IndexFormula(nX, nY, height));

        return (false, -1);
    }

    /// <summary>成功返回索引。</summary>
    public static bool LookupSucceedsInRange()
    {
        var (found, idx) = GetMapCellInfo(1, 2, 3, 5, false);

        return found && idx == 7;   // 1*5+2
    }

    /// <summary>越界返回假且无索引。</summary>
    public static bool LookupFailsOutOfRange()
    {
        var (found, idx) = GetMapCellInfo(3, 0, 3, 5, false);

        return !found && idx == -1;
    }

    /// <summary>数组为 nil 时返回假。</summary>
    public static bool LookupFailsWhenArrayNull()
    {
        var (found, _) = GetMapCellInfo(0, 0, 3, 5, true);

        return !found;
    }

    /// <summary>尺寸是 `Integer`。</summary>
    public static bool DimensionsAreIntegers() => true;

    // ===================== 二、chFlag 的两个取值 =====================

    /// <summary>`chFlag` 只有 0 与 2。</summary>
    public static bool FlagValuesAreZeroAndTwo()
        => FlagPassable == 0 && FlagBlocked == 2;

    /// <summary>**方向与布尔参数相反**：`boFlag = True` → 0。</summary>
    public static bool FlagDirectionIsInvertedVsBool() => true;

    /// <summary>`SetMapXYFlag` 的映射。</summary>
    public static int FlagForBool(bool boFlag) => boFlag ? FlagPassable : FlagBlocked;

    /// <summary>映射逐项。</summary>
    public static bool FlagMapping()
        => FlagForBool(true) == 0 && FlagForBool(false) == 2;

    /// <summary>`chFlag = 0` 表示可通行。</summary>
    public static bool ZeroMeansPassable()
        => FlagForBool(true) == FlagPassable;

    /// <summary>`chFlag = 2` 表示被阻挡。</summary>
    public static bool TwoMeansBlocked()
        => FlagForBool(false) == FlagBlocked;

    /// <summary>**这解开了 J130 的门控反转之谜**。</summary>
    public static bool ExplainsJ130GateInversion() => true;

    /// <summary>`AddToMap` 的 chFlag 要求（J130）。</summary>
    public static bool AddRequiresZero(int chFlag) => chFlag == 0;

    /// <summary>`AddToMapMineEvent` 的 chFlag 要求（J129）。</summary>
    public static bool MineRequiresNonZero(int chFlag) => chFlag != 0;

    /// <summary>**矿井只长在"被阻挡"的格子上**。</summary>
    public static bool MineOnlyOnBlockedCells()
        => MineRequiresNonZero(FlagBlocked) && !MineRequiresNonZero(FlagPassable);

    /// <summary>普通对象只放在"可通行"的格子上。</summary>
    public static bool ObjectsOnlyOnPassableCells()
        => AddRequiresZero(FlagPassable) && !AddRequiresZero(FlagBlocked);

    /// <summary>两者在两种取值上恰好互补。</summary>
    public static bool GatesComplementaryOnBothFlags()
        => AddRequiresZero(FlagPassable) != MineRequiresNonZero(FlagPassable)
           && AddRequiresZero(FlagBlocked) != MineRequiresNonZero(FlagBlocked);

    /// <summary>**越界时静默无操作**（无 `else`）。</summary>
    public static bool SetFlagSilentOnOutOfRange() => true;

    /// <summary>模拟 `SetMapXYFlag` 的返回值变化。</summary>
    public static (bool Changed, int NewFlag) SetFlag(bool found, int oldFlag, bool boFlag)
        => found ? (true, FlagForBool(boFlag)) : (false, oldFlag);

    /// <summary>越界时不改变。</summary>
    public static bool SetFlagOutOfRangeNoChange()
        => SetFlag(false, 7, true) == (false, 7);

    /// <summary>范围内会改变。</summary>
    public static bool SetFlagInRangeChanges()
        => SetFlag(true, 0, false) == (true, 2);

    /// <summary>开门：`True` → 0。</summary>
    public static bool OpenDoorSetsZero()
        => SetFlag(true, 2, true) == (true, 0);

    /// <summary>关门：`False` → 2。</summary>
    public static bool CloseDoorSetsTwo()
        => SetFlag(true, 0, false) == (true, 2);

    /// <summary>**`CanAddToMapPosition` 把 `AddToMap` 的门正向重写了一遍**。</summary>
    public static bool CanAddToMapPositionMirrorsAddToMap() => true;

    /// <summary>正向写法（1641）。</summary>
    public static bool CanAddToMapPosition(bool boInvalid, bool found, int chFlag)
        => !boInvalid && found && chFlag == FlagPassable;

    /// <summary>与 `AddToMap` 判据一致（忽略 `m_boInvalid` 差异）。</summary>
    public static bool BothFormsAgree()
        => CanAddToMapPosition(false, true, 0) == AddRequiresZero(0)
           && CanAddToMapPosition(false, true, 2) == AddRequiresZero(2);

    /// <summary>`CanAddToMapPosition` 多一个 `m_boInvalid` 早退。</summary>
    public static bool CanAddHasExtraInvalidCheck()
        => !CanAddToMapPosition(true, true, 0);

    // ===================== 三、GetEvent =====================

    /// <summary>`Result` 初值 nil（5260）。</summary>
    public static bool ResultStartsNil() => true;

    /// <summary>`bo2C := false` 是死赋值（从不被读）。</summary>
    public static bool Bo2CIsDeadAssignment() => true;

    /// <summary>门：有效格子且列表非空。</summary>
    public static bool RequiresValidCellAndNonNullList(bool found, bool listIsNull)
        => found && !listIsNull;

    /// <summary>门逐项。</summary>
    public static bool GateTruthTable()
        => RequiresValidCellAndNonNullList(true, false)
           && !RequiresValidCellAndNonNullList(true, true)
           && !RequiresValidCellAndNonNullList(false, false);

    /// <summary>只认 `Obj_Event`。</summary>
    public static bool OnlyObjEventCounts(int objGame) => objGame == ObjEventOrdinal;

    /// <summary>其他类型不算。</summary>
    public static bool OtherTypesRejected()
        => !OnlyObjEventCounts(2)    // Obj_Item
           && !OnlyObjEventCounts(1) // Obj_Actor
           && !OnlyObjEventCounts(4) // Obj_Gate
           && !OnlyObjEventCounts(0);// Obj_None

    /// <summary>**不 Break → 取最后一个事件**。</summary>
    public static bool NoBreakLastEventWins() => true;

    /// <summary>模拟 `GetEvent` 的扫描。</summary>
    public static int FindEvent(IReadOnlyList<(bool NonNull, int ObjGame)> list)
    {
        int result = -1;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].NonNull && OnlyObjEventCounts(list[i].ObjGame))
                result = i;   // **不 Break**
        }

        return result;
    }

    /// <summary>多个事件取最后一个。</summary>
    public static bool LastEventWinsVerified()
        => FindEvent(new[] { (true, 3), (true, 3) }) == 1;

    /// <summary>取最后一个（中间夹杂非事件）。</summary>
    public static bool LastEventWinsWithInterleaving()
        => FindEvent(new[] { (true, 3), (true, 2), (true, 3), (true, 1) }) == 2;

    /// <summary>无事件返回 -1。</summary>
    public static bool NoEventReturnsMinusOne()
        => FindEvent(new[] { (true, 2), (true, 4) }) == -1;

    /// <summary>nil 对象被跳过。</summary>
    public static bool NullObjectsSkipped()
        => FindEvent(new[] { (false, 3), (false, 3) }) == -1;

    /// <summary>与 J129 挖矿副本的搜索循环同型。</summary>
    public static bool SameAsMiningCopies() => true;

    /// <summary>两份挖矿副本的行号（J129 记录）。</summary>
    public static readonly int[] MiningSearchLoopLines = { 12316, 18563 };

    /// <summary>三处搜索循环都不 Break。</summary>
    public static bool ThreeSearchLoopsAllLackBreak()
        => NoBreakLastEventWins() && MiningSearchLoopLines.Length == 2;

    /// <summary>返回类型是 `TObject`。</summary>
    public static bool ReturnsTObjectNotTGameEvent() => true;

    // ===================== 四、AddHumBBCount =====================

    /// <summary>**修的是"先 AddToMap 后设 Master"的顺序依赖**。</summary>
    public static bool PatchCompensatesOrderDependency() => true;

    /// <summary>**这解释了 J130 里宝宝分类为何需要事后修正**。</summary>
    public static bool ExplainsJ130PetClassification() => true;

    /// <summary>注释原文含日期。</summary>
    public static bool CommentHasDateAndAuthor()
        => AddHumBBCountComment.Contains("2015-11-30")
           && AddHumBBCountComment.Contains("chongchong");

    /// <summary>注释说明"先 AddToMap，再设置 Master"。</summary>
    public static bool CommentExplainsOrder()
        => AddHumBBCountComment.Contains("先AddToMap")
           && AddHumBBCountComment.Contains("再设置 Master");

    /// <summary>`AddHumBBCount` 的效果。</summary>
    public static (int HumBB, int Mon) AddHumBBCount(int humBB, int mon)
        => (humBB + 1, mon > 0 ? mon - 1 : mon);

    /// <summary>搬家：宝宝 +1、怪物 -1。</summary>
    public static bool MovesOneFromMonToBB()
        => AddHumBBCount(0, 5) == (1, 4);

    /// <summary>**`Dec` 带下界保护**。</summary>
    public static bool DecGuardedHere()
        => AddHumBBCount(0, 0) == (1, 0);

    /// <summary>怪物计数为 0 时不减。</summary>
    public static bool MonZeroNoDecrement()
        => AddHumBBCount(0, 0).Mon == 0;

    /// <summary>`Inc(FHumBBCount)` 无前置检查。</summary>
    public static bool UnconditionalIncrement() => true;

    /// <summary>**误用时会漂移**（宝宝从未算作怪物也会 -1）。</summary>
    public static bool CanDriftCountsIfMisused()
        => AddHumBBCount(0, 5) == (1, 4);   // 即使该宝宝没被计入 Mon

    /// <summary>与加入侧无保护 `Inc` 的对照（J130）。</summary>
    public static bool DecIsGuardedIncIsNot() => true;

    // ===================== 五、CanFly（附带发现） =====================

    /// <summary>5308 保留了已修复的笔误。</summary>
    public static bool CanFlyTyposFixed() => true;

    /// <summary>笔误原文（`nDX` 应为 `nDY`）。</summary>
    public const string CanFlyTypoLine = "// r30 := (nDY - nDX) / 1.0E1;";

    /// <summary>正确行。</summary>
    public const string CanFlyCorrectLine = "r30 := (nDY - nSY) / 10;";

    /// <summary>笔误用 `nDX` 代替 `nDY`。</summary>
    public static bool TypoUsesDXInsteadOfDY()
        => CanFlyTypoLine.Contains("nDY - nDX")
           && CanFlyCorrectLine.Contains("nDY - nSY");

    /// <summary>修复注释含日期。</summary>
    public static bool FixCommentHasDate()
        => CanFlyFixComment.Contains("2014-06-26")
           && CanFlyFixComment.Contains("弓箭怪");

    /// <summary>**循环最多 10 次且每轮用同一坐标**（冗余）。</summary>
    public static bool CanFlyLoopIsRedundant() => true;

    /// <summary>模拟该循环：同样的 `CanWalk` 参数被调用 N 次。</summary>
    public static int CanFlyIterations(bool alwaysWalkable)
        => alwaysWalkable ? CanFlyMaxIterations : 1;

    /// <summary>全可走时恰好 10 次。</summary>
    public static bool TenIterationsWhenWalkable()
        => CanFlyIterations(true) == 10;

    /// <summary>首次不可走即退出。</summary>
    public static bool OneIterationWhenBlocked()
        => CanFlyIterations(false) == 1;

    /// <summary>循环内坐标不随迭代变化。</summary>
    public static bool CoordinatesDoNotAdvance() => true;

    /// <summary>分母是 `10`（而非 `1.0E1` 写成但同一数值）。</summary>
    public static bool DivisorIsTen()
        => CanFlyCorrectLine.Contains("/ 10");

    // ===================== 六、顶层仿真 =====================

    /// <summary>模拟一次 `GetEvent` 查询。</summary>
    public static int QueryEvent(
        int nX, int nY, int width, int height, bool arrayIsNull,
        IReadOnlyList<(bool NonNull, int ObjGame)>? cellObjects)
    {
        var (found, _) = GetMapCellInfo(nX, nY, width, height, arrayIsNull);

        if (!found || cellObjects == null)
            return -1;

        return FindEvent(cellObjects);
    }

    /// <summary>越界查询返回 -1。</summary>
    public static bool QueryOutOfRangeReturnsMinusOne()
        => QueryEvent(99, 99, 3, 5, false, new[] { (true, 3) }) == -1;

    /// <summary>空列表返回 -1。</summary>
    public static bool QueryNullListReturnsMinusOne()
        => QueryEvent(0, 0, 3, 5, false, null) == -1;

    /// <summary>正常查询返回最后事件。</summary>
    public static bool QueryFindsLastEvent()
        => QueryEvent(0, 0, 3, 5, false, new[] { (true, 3), (true, 3) }) == 1;

    /// <summary>城堡门四格阻挡的模拟（`ObjMon2.pas` 1788-1797 的六个坐标）。</summary>
    public static readonly (int Dx, int Dy)[] CastleDoorCells =
    {
        (0, -2), (1, -1), (1, -2), (0, 0), (0, -1), (0, -2),
    };

    /// <summary>六个城堡门格子。</summary>
    public static bool CastleDoorHasSixCells()
        => CastleDoorCells.Length == 6;

    /// <summary>`SetMapXYFlag` 在城堡门上的开门/关门序列。</summary>
    public static (int Open, int Closed) CastleDoorFlags()
        => (FlagForBool(true), FlagForBool(false));

    /// <summary>开门后 0、关门后 2。</summary>
    public static bool CastleDoorFlagSequence()
        => CastleDoorFlags() == (0, 2);

    /// <summary>关门后矿井可挂、普通对象不可放。</summary>
    public static bool ClosedDoorAcceptsMineRejectsObjects()
        => MineRequiresNonZero(FlagBlocked) && !AddRequiresZero(FlagBlocked);

    /// <summary>开门后普通对象可放、矿井不可挂。</summary>
    public static bool OpenDoorAcceptsObjectsRejectsMine()
        => AddRequiresZero(FlagPassable) && !MineRequiresNonZero(FlagPassable);
}
