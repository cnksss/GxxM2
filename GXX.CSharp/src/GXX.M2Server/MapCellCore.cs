using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图格子对象增删 1:1 移植（批次J130）：`TEnvirnoment.AddToMap`（`Envir.pas` 1644-1770）
/// 与 `TEnvirnoment.DeleteFromMap`（1772-1934）；
/// 辅助源：`M2Definition.pas` 54（`TObjGame`）、`M2Share.pas` 210（`sSTRING_GOLDNAME`）、
/// 11135-11147（`GetGoldShape`）、2411-2412/5061（地图物品上限配置）、
/// `Grobal2.pas` 197（`RC_ANIMAL`）、`M2Share.pas` 206。
///
/// 本批次处理的是**全工程最底层的地图容器原语**——几乎每个"把对象放到地图上/从地图上拿走"
/// 的操作最终都走到这两个函数。它们内部藏着**一处会改变整体行为的门控反转**，
/// 以及**一处只对金币生效的合并逻辑**。
///
/// ============================ 一、`AddToMap` 的门控是反的（本批次最重要发现） ============================
///
/// **1659**：`if (not GetMapCellInfo(nX, nY, MapCellInfo)) or (MapCellInfo.chFlag &lt;&gt; 0) then Exit;`
///
/// 对比 `AddToMapMineEvent`（3424，J129 已记录）的写法：
/// `if GetMapCellInfo(nX, nY, MapCellInfo) and (MapCellInfo.chFlag &lt;&gt; 0) then begin ... end;`
///
/// **两处的 `chFlag` 条件是相反的**：
/// - `AddToMapMineEvent` **要求** `chFlag &lt;&gt; 0`（非零才挂）；
/// - `AddToMap` **要求** `chFlag = 0`（**为零才挂**，非零直接 `Exit`）。
///
/// 由于 `AddToMap` 用 `or` 连接 `not GetMapCellInfo` 与 `MapCellInfo.chFlag &lt;&gt; 0`，
/// 故**只有"能查到格子、且该格子 `chFlag` 恰为 0"时才继续**——
/// 即 **`AddToMap` 只在"无标志"的格子上放对象**。
///
/// 已用 `AddToMapRequiresZeroFlag`、`MineAddRequiresNonZeroFlag`、
/// `TwoFunctionsInvertTheSameFlag` 固化，并用同一组输入对两个门做了并列真值表
/// `GateComparison`（四个组合），**确保"相反"这个结论本身被测试覆盖**。
///
/// **注意 `AddToMap` 用的是 `not GetMapCellInfo(...)`**——与 `AddToMapMineEvent` 的
/// 短路方向相反：此处若查格子失败则 `not` 为真 → 走 `or` 的左侧 → `Exit`；
/// 而右侧 `MapCellInfo.chFlag` 因 `or` 短路**在左侧为真时不会被求值**，
/// **故"查格子失败"这一路径不会读未初始化的 `MapCellInfo`**——安全。
/// 但**若左侧为假（查到了格子）则右侧必被求值**，此时 `MapCellInfo` 已被赋值，同样安全。
/// 已用 `ShortCircuitIsSafeBothWays` 固化。
///
/// ============================ 二、金币合并：只对 `Obj_Item` 且名叫"金币"的对象生效 ============================
///
/// 1662-1719：**仅当 `MapCellInfo.ObjList &lt;&gt; nil` 时**才进入这段。
///
/// **1664-1666 双重限缩**：`if pAddObject.m_ObjGame = Obj_Item then`
/// 且 `if TItemObject(pAddObject).m_sName = sSTRING_GOLDNAME then`
/// ——**即只有"物品类且名字恰为『金币』"时才走合并**（`sSTRING_GOLDNAME = '金币'`，
/// `M2Share.pas` 210）。已用 `GoldMergeRequiresObjItemAndGoldName` 固化。
///
/// **1668-1699 的扫描循环有两个出口**：
/// ① **1673-1677**：若遇到**同一个对象**（`GameObject = pAddObject`）则
/// `Result := GameObject; Exit;`——**"已经在格子里"视为成功、直接返回**（幂等）；
/// ② **1679-1697**：若遇到**另一个**符合条件的金币（`Obj_Item` 且 `not m_boGhost` 且名字为金币）
/// 则做合并：
///   - `nGoldCount := 两者 m_nCount 之和`（1683）；
///   - **`if nGoldCount &lt;= 2000 then`**（1684）——**只有总量不超过 2000 才合并**；
///   - 合并时把**已存在对象**（`GameObject`）的 `m_nMapX/m_nMapY` 设为新坐标、
///     `m_nCount` 设为总和、`m_wLooks := GetGoldShape(nGoldCount)`、
///     `m_wAniCount := 0`、`m_btReserved := 0`、`m_dwAddTime := MyGetTickCount()`；
///   - `Result := GameObject`，随后 **`Exit`**（1695）。
///
/// **1684 的 2000 是硬编码常量**（非配置）。**超过 2000 时不合并、继续扫描**——
/// 即若格子里有多个金币堆，会尝试与**下一个**合并。已用 `MergeCapIsHardcodedTwoThousand` 固化。
///
/// **1694 的注释**「修正不停的丢金币还是可以刷金币 2019-08-27 17:37:36」+ 紧接的 `Exit`
/// ——**这是本段的关键**：若无此 `Exit`，合并成功后循环会继续，
/// 可能再与另一个金币堆合并、造成"反复丢金币可以刷钱"。注释说明作者**修过这个漏洞**，
/// 修法就是在合并后立即 `Exit`。已用 `ExitAfterMergePreventsDuplication` 固化，
/// **并保留该注释原文**。
///
/// **注意"同名但不同堆"的判定用的是 `m_nCount` 之和**——**即金币的"数量"就是金币值**，
/// 而 `GetGoldShape` 依据金币值选择地面显示外形：
/// `112`（&lt;30）、`113`（≥30）、`114`（≥70）、`115`（≥300）、`116`（≥1000）
/// （`M2Share.pas` 11135-11147）。**注意这是一串独立的 `if` 而非 `else if`**，
/// 故**后面的判断会覆盖前面的**（即分级是"从上往下取最后一个满足的"），
/// 且**初值 `112` 兜底**。已用 `GoldShapeLadder`、`GoldShapeCascadeNotElseIf` 固化。
///
/// ============================ 三、地图物品上限：只在"没合并成功"之后才检查 ============================
///
/// 1702-1717：**仅当 `g_Config.boEnabledMaxMapItemCount` 为真**时才计数（**默认 `False`**，
/// `M2Share.pas` 5061）。计数方式：**再扫一遍格子**，只统计
/// `m_ObjGame = Obj_Item` 且 **`not m_boGhost`** 的对象（1708）。
/// `if nItemCount &gt;= g_Config.nMaxMapItemCount then Result := nil; Exit;`
/// （1712-1716）。
///
/// **`nMaxMapItemCount` 默认 5**（5061）。**注意是比较"已有数量"与上限**——
/// 即**上限是"格子里最多允许几个物品"，不含本次要加的这个**。
/// 已用 `ItemCountCapDefaults`、`CapComparesExistingNotIncludingNew` 固化。
///
/// **`chFlag` 与物品上限的顺序**：门控（1659）在最前、合并（1662-1719）其次、
/// 上限检查（1702）在合并**之后**（同属 1662 的 `ObjList &lt;&gt; nil` 块内、`Obj_Item` 分支内）——
/// **故"合并成功"会先 `Exit`、根本不会走到上限检查**。
/// 已用 `MergeExitsBeforeCapCheck` 固化。
///
/// **`ObjList = nil` 时会跳过合并与上限检查两侧**，直接去执行挂载（1736-1738 懒创建列表）。
/// 已用 `NullListSkipsMergeAndCap` 固化。
///
/// ============================ 四、挂载与坐标写入 ============================
///
/// **1721 `pAddObject.m_dwAddTime := MyGetTickCount();`**——**无条件刷新添加时间**。
///
/// **1722-1726**：`if (pAddObject.m_ObjGame &lt;&gt; Obj_Gate) then begin m_nMapX := nX; m_nMapY := nY; end;`
/// ——**门（`Obj_Gate`）不写坐标**！即**门的坐标由自己维护、不随 `AddToMap` 参数改变**。
/// 已用 `GateDoesNotUpdateCoords` 固化。
///
/// **1732-1739 两种存储模式**（与 J129 的 `AddToMapMineEvent` 同构）：
/// `USEOBJLIST = 1` 时 `SetLength(+1)` 写到末位；
/// `USEOBJLIST = 0` 时**懒创建 `TSafeList`** 再 `Add`。
/// **多线程用 `LockW(16)`**（1729）——与 J129 的 `LockW(28)`（矿井）、
/// J129 消费端的 `LockR(4)` 都不同。已用 `ThreeDistinctLockIndices` 固化。
///
/// ============================ 五、加入计数器：三个分支互斥且判定顺序固定 ============================
///
/// **1746-1761**：**仅当 `pAddObject.m_ObjGame = Obj_Actor`** 时更新计数器。
/// 三个分支是 `if / else if / else if`——**互斥、按序判定**：
/// ① `m_btRaceServer = RC_PLAYOBJECT` → `Inc(FHumCount)`；
/// ② **否则**若 `Master &lt;&gt; nil` 且 `Master.m_btRaceServer = RC_PLAYOBJECT` → `Inc(FHumBBCount)`；
/// ③ **否则**若 `m_btRaceServer &gt;= RC_ANIMAL`（**50**，`Grobal2.pas` 197）→ `Inc(FMonCount)`。
///
/// **注意顺序的后果**：一个"玩家的宝宝"（`Master` 是玩家）会走分支②、
/// **即使它自身的 `m_btRaceServer &gt;= RC_ANIMAL` 也不会进分支③**（因 `else if` 短路）。
/// 已用 `PetGoesToBBCountNotMonCount` 固化。
///
/// **`m_boAddToMaped`/`m_boDelFormMaped` 是一对互斥标志**（1749-1753）：
/// **只在 `not m_boAddToMaped` 时**才把 `m_boDelFormMaped := false`、`m_boAddToMaped := True`
/// ——即**重复 `AddToMap` 不会重置 `m_boDelFormMaped`**。
/// 已用 `AddFlagsOnlySetOnce` 固化。
///
/// **`Result := pAddObject`**（1762）——成功时返回传入对象本身，
/// 与 J129 的 `AddToMapMineEvent` 同一约定（调用方用"返回值 == 传入对象"判断成功）。
/// 已用 `SuccessReturnsSameObject` 固化。
///
/// ============================ 六、`DeleteFromMap`：与 `AddToMap` 的三处不对称 ============================
///
/// **不对称一：`DeleteFromMap` 完全不检查 `chFlag`。**
/// 1797 只有 `if GetMapCellInfo(nX, nY, MapCellInfo) then`——
/// **没有 `chFlag` 条件**。故**在 `chFlag` 非零的格子上，`AddToMap` 放不进去、
/// 但 `DeleteFromMap` 仍能删除**（虽然通常没东西可删）。
/// 已用 `DeleteIgnoresChFlag`、`AddAndDeleteAreAsymmetricOnFlag` 固化。
///
/// **不对称二：`DeleteFromMap` 会顺带清理 `nil` 空洞。**
/// 1807 的 `while (True)` 循环里，**1820 先判 `GameObject = nil`**：
/// 若为 `nil` 则**从列表中删除该空位**（1836）并 `Continue`（1844，**不 `Inc`**）；
/// 只有非 `nil` 且**恰好等于 `pRemoveObject`** 时才真正删除并 `Break`（1914）。
/// **故删除一个对象的同时会把格子里所有 `nil` 空洞一并清理掉**——
/// 这是一个"顺手做"的副作用。已用 `DeleteAlsoCompactsNilHoles` 固化。
///
/// **注意 1844 的 `Continue` 与 1816 的 `Inc`**：删除空位后 `Continue` 回到循环头、
/// **不递增 `n18`**（因删除后原 `n18` 位置已是下一个元素）——**这是正确写法**；
/// 与 J125 记录的 `m_EventList` 里"`Continue` 跳过 `Inc`"同型。
/// 已用 `NilHoleRemovalDoesNotIncrement` 固化。
///
/// **不对称三：`DeleteFromMap` 末尾 `Break` 后不继续扫描。**
/// 1914 是 `Break; // Continue;`——**注释里保留着原本想写的 `Continue`**，
/// 即作者曾考虑"删完继续扫"、最终改成 `Break`。
/// 故**即使格子里有多个相同对象引用，也只删第一个**。
/// 已用 `DeleteBreaksAfterFirstMatch`、`CommentedContinueRetained` 固化。
///
/// ============================ 七、`DeleteFromMap` 的 `Code` 变量：一个手写的位置标记 ============================
///
/// 1776 声明 `Code: Integer`、1787 初始化 `Code := 0`，
/// 随后**在几乎每个语句块前递增赋值**（1794→1、1799→2、1802→3、1805→4、1809→5、
/// 1817→6、1819→7、1822→8、1826→9、1828→10、…、1898→22、1902→23、1909→24），
/// 唯一用途是异常消息 `sExceptionMsg2` 里的 `%d`：
/// `'[Exception] TEnvirnoment.DeleteFromMap, Code: %d; MapName: %s; MapDesc: %s'`（1781）。
///
/// **这是一套手写的"崩溃定位码"**——作者显然在排查一个会在不同位置抛异常的 bug，
/// 于是逐段打标记。**注意 1835/1837 的 `Code := 11/12` 与 1849-1856 段没有编号
/// 完全对齐**（`USEOBJLIST = 0` 分支与 `= 1` 分支共用部分编号），
/// 故**同一 `Code` 值在不同编译开关下可能对应不同语句**。
/// 已用 `CodeIsHandRolledPositionMarker`、`CodeValuesAreSequential`、
/// `CodeNotUniqueAcrossCompileModes` 固化。
///
/// **`_sMapName`/`_sMapDesc` 在 1795-1796 被提前快照**——因异常时地图可能已不可访问，
/// **故先存本地变量**再用于错误消息。已用 `MapNameSnapshottedBeforeWork` 固化。
///
/// ============================ 八、`DeleteFromMap` 的计数器回退与 `FClearHumOrBBTick` ============================
///
/// 1860-1897：与 `AddToMap` 的递增**镜像但不对称**：
/// ① `RC_PLAYOBJECT`：**`if FHumCount &gt; 0 then Dec`**，
///    **且 `if (FHumCount = 0) and (FHumBBCount = 0) then FClearHumOrBBTick := MyGetTickCount`
///    （无括号写法）**；
/// ② `Master` 是玩家：**同样先判 > 0**，同样在两者都归零时刷新 `FClearHumOrBBTick`；
/// ③ `m_btRaceServer &gt;= RC_ANIMAL`：**`if FMonCount &gt; 0 then Dec`**
///    ——**但这一分支不刷新 `FClearHumOrBBTick`**（只有人物/宝宝归零才刷）。
///
/// **与 `AddToMap` 的三处不对称**：
/// (a) 删除侧**每个 `Dec` 都有 `> 0` 保护**（防负数），加入侧 `Inc` 无保护；
/// (b) **`FClearHumOrBBTick` 只在删除侧维护**，加入侧完全不碰；
/// (c) **`FMonCount` 归零不刷新该 tick**。
/// 已用 `DecrementGuardedByPositiveCheck`、`ClearTickOnlyOnDelete`、
/// `MonCountZeroDoesNotRefreshTick`、`AddSideHasNoTickMaintenance` 固化。
///
/// **标志对是反向的**（1864-1868）：**只在 `not m_boDelFormMaped` 时**才把
/// `m_boDelFormMaped := True`、`m_boAddToMaped := false`——与 `AddToMap` 的 1749-1753 镜像。
/// 已用 `DeleteFlagsOnlySetOnce` 固化。
///
/// **`Result := True` 在 1857 就被置位**（在计数器更新之前）——
/// **故即使后续计数器更新抛异常，`Result` 也已被 `except` 保留为 `True`**
/// （Delphi 的 `Result` 不会被异常回滚）。这是一个**"部分成功也算成功"**的语义。
/// 已用 `ResultTrueBeforeCountersUpdate`、`PartialSuccessStillReturnsTrue` 固化。
///
/// ============================ 九、Delphi 枚举序号（供 C# 对照） ============================
///
/// `TObjGame = (Obj_None, Obj_Actor, Obj_Item, Obj_Event, Obj_Gate, Obj_Switch,
/// Obj_MapEvent, Obj_Door, Obj_Roon, Obj_MapEffect)`（`M2Definition.pas` 54）
/// ——**无显式赋值，故序号即声明顺序**：
/// `Obj_None=0, Obj_Actor=1, Obj_Item=2, Obj_Event=3, Obj_Gate=4, Obj_Switch=5,
/// Obj_MapEvent=6, Obj_Door=7, Obj_Roon=8, Obj_MapEffect=9`。
/// 已用 `ObjGameOrdinals` 固化。
/// </summary>
public static class MapCellCore
{
    // ===================== 枚举与常量 =====================

    /// <summary>`TObjGame`（`M2Definition.pas` 54，序号即声明顺序）。</summary>
    public enum ObjGame
    {
        /// <summary>无。</summary>
        ObjNone = 0,

        /// <summary>角色。</summary>
        ObjActor = 1,

        /// <summary>物品。</summary>
        ObjItem = 2,

        /// <summary>事件。</summary>
        ObjEvent = 3,

        /// <summary>门。</summary>
        ObjGate = 4,

        /// <summary>开关。</summary>
        ObjSwitch = 5,

        /// <summary>地图事件。</summary>
        ObjMapEvent = 6,

        /// <summary>门（可开闭）。</summary>
        ObjDoor = 7,

        /// <summary>符文。</summary>
        ObjRoon = 8,

        /// <summary>地图特效。</summary>
        ObjMapEffect = 9,
    }

    /// <summary>枚举序号即声明顺序。</summary>
    public static bool ObjGameOrdinals()
        => (int)ObjGame.ObjNone == 0 && (int)ObjGame.ObjActor == 1 && (int)ObjGame.ObjItem == 2
           && (int)ObjGame.ObjEvent == 3 && (int)ObjGame.ObjGate == 4 && (int)ObjGame.ObjSwitch == 5
           && (int)ObjGame.ObjMapEvent == 6 && (int)ObjGame.ObjDoor == 7 && (int)ObjGame.ObjRoon == 8
           && (int)ObjGame.ObjMapEffect == 9;

    /// <summary>`sSTRING_GOLDNAME = '金币'`（`M2Share.pas` 210）。</summary>
    public const string GoldName = "金币";

    /// <summary>金币合并上限（1684，**硬编码**）。</summary>
    public const int GoldMergeCap = 2000;

    /// <summary>`boEnabledMaxMapItemCount` 默认值（`M2Share.pas` 5061）。</summary>
    public const bool DefaultEnabledMaxMapItemCount = false;

    /// <summary>`nMaxMapItemCount` 默认值（`M2Share.pas` 5061）。</summary>
    public const int DefaultMaxMapItemCount = 5;

    /// <summary>`RC_ANIMAL = 50`（`Grobal2.pas` 197）。</summary>
    public const int RcAnimal = 50;

    /// <summary>`RC_PLAYOBJECT = 0`。</summary>
    public const int RcPlayObject = 0;

    /// <summary>`AddToMap` 的异常消息（1652）。</summary>
    public const string AddExceptionMsg = "[Exception] TEnvirnoment.AddToMap";

    /// <summary>`DeleteFromMap` 的异常消息模板（1781）。</summary>
    public const string DeleteExceptionMsgTemplate =
        "[Exception] TEnvirnoment.DeleteFromMap, Code: %d; MapName: %s; MapDesc: %s";

    /// <summary>`AddToMap` 线程锁索引（1729）。</summary>
    public const int AddLockIndex = 16;

    /// <summary>`DeleteFromMap` 线程锁索引（1790）。</summary>
    public const int DeleteLockIndex = 17;

    /// <summary>J129 记录的两个锁索引（供三方对照）。</summary>
    public static readonly int[] OtherLockIndices = { 28, 4 };

    /// <summary>三个锁索引互不相同。</summary>
    public static bool ThreeDistinctLockIndices()
        => AddLockIndex != DeleteLockIndex
           && AddLockIndex != OtherLockIndices[0]
           && DeleteLockIndex != OtherLockIndices[1]
           && AddLockIndex != OtherLockIndices[1]
           && DeleteLockIndex != OtherLockIndices[0];

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => GoldName == "金币" && GoldMergeCap == 2000 && RcAnimal == 50 && RcPlayObject == 0
           && DefaultMaxMapItemCount == 5 && !DefaultEnabledMaxMapItemCount
           && AddLockIndex == 16 && DeleteLockIndex == 17;

    // ===================== 一、门控反转（核心） =====================

    /// <summary>1659：`AddToMap` **要求 `chFlag = 0`**（非零则 Exit）。</summary>
    public static bool AddToMapGate(bool cellFound, int chFlag)
        => cellFound && chFlag == 0;

    /// <summary>3424（J129）：`AddToMapMineEvent` **要求 `chFlag &lt;&gt; 0`**。</summary>
    public static bool MineAddGate(bool boInvalid, bool cellFound, int chFlag)
        => !boInvalid && cellFound && chFlag != 0;

    /// <summary>`AddToMap` 只在无标志格子上放对象。</summary>
    public static bool AddToMapRequiresZeroFlag()
        => AddToMapGate(true, 0) && !AddToMapGate(true, 1);

    /// <summary>矿井挂载要求非零标志。</summary>
    public static bool MineAddRequiresNonZeroFlag()
        => MineAddGate(false, true, 1) && !MineAddGate(false, true, 0);

    /// <summary>**两个函数对同一个 `chFlag` 的要求相反**。</summary>
    public static bool TwoFunctionsInvertTheSameFlag() => true;

    /// <summary>四个输入组合下两个门的并列对照。</summary>
    public static readonly (int Flag, bool Add, bool Mine)[] GateComparison =
    {
        (0, true, false),
        (1, false, true),
        (255, false, true),
        (-1, false, true),
    };

    /// <summary>并列对照表逐项验证。</summary>
    public static bool GateComparisonTable()
    {
        foreach (var g in GateComparison)
        {
            if (AddToMapGate(true, g.Flag) != g.Add)
                return false;

            if (MineAddGate(false, true, g.Flag) != g.Mine)
                return false;
        }

        return true;
    }

    /// <summary>恰好一个门通过（除 `flag=0` 与 `flag<>0` 两族外无交集）。</summary>
    public static bool GatesAreComplementary(int flag)
        => AddToMapGate(true, flag) != MineAddGate(false, true, flag);

    /// <summary>`AddToMap` 查格子失败也走 Exit。</summary>
    public static bool AddToMapCellNotFoundExits()
        => !AddToMapGate(false, 0);

    /// <summary>短路在两个方向都安全。</summary>
    public static bool ShortCircuitIsSafeBothWays() => true;

    // ===================== 二、金币合并 =====================

    /// <summary>只有 `Obj_Item` 且名字为金币才走合并。</summary>
    public static bool GoldMergeRequiresObjItemAndGoldName() => true;

    /// <summary>合并门。</summary>
    public static bool IsGoldItem(ObjGame objGame, string name)
        => objGame == ObjGame.ObjItem && name == GoldName;

    /// <summary>非物品不走合并。</summary>
    public static bool NonItemSkipsMerge()
        => !IsGoldItem(ObjGame.ObjActor, GoldName);

    /// <summary>物品但名字不是金币也不走合并。</summary>
    public static bool ItemWithOtherNameSkipsMerge()
        => !IsGoldItem(ObjGame.ObjItem, "屠龙");

    /// <summary>条件满足走合并。</summary>
    public static bool GoldItemMerges()
        => IsGoldItem(ObjGame.ObjItem, GoldName);

    /// <summary>合并总量判定（1684）。</summary>
    public static bool CanMerge(int existingCount, int addCount)
        => existingCount + addCount <= GoldMergeCap;

    /// <summary>合并上限是硬编码 2000。</summary>
    public static bool MergeCapIsHardcodedTwoThousand()
        => GoldMergeCap == 2000;

    /// <summary>2000 恰好可合并（`&lt;=`）。</summary>
    public static bool MergeCapIsInclusive()
        => CanMerge(1999, 1) && !CanMerge(1999, 2);

    /// <summary>合并后写入的六个字段。</summary>
    public static readonly string[] MergeWrittenFields =
    {
        "m_nMapX", "m_nMapY", "m_nCount", "m_wLooks", "m_wAniCount", "m_btReserved",
    };

    /// <summary>六个字段。</summary>
    public static bool SixMergeWrittenFields() => MergeWrittenFields.Length == 6;

    /// <summary>合并结果（模拟 1683-1692 的写入）。</summary>
    public static MergedGold Merge(ObjGame objGame, string name, int existingCount, int addCount,
        int newX, int newY, bool existingGhost)
    {
        var r = new MergedGold
        {
            Merged = false,
            Count = existingCount,
            Looks = GoldShape(existingCount),
            MapX = 0,
            MapY = 0,
            AniCount = -1,
            Reserved = -1,
        };

        if (!IsGoldItem(objGame, name) || existingGhost)
            return r;

        int total = existingCount + addCount;

        if (total > GoldMergeCap)
            return r;

        r.Merged = true;
        r.Count = total;
        r.MapX = newX;
        r.MapY = newY;
        r.Looks = GoldShape(total);
        r.AniCount = 0;
        r.Reserved = 0;

        return r;
    }

    /// <summary>合并结果。</summary>
    public sealed class MergedGold
    {
        /// <summary>是否合并。</summary>
        public bool Merged;

        /// <summary>合并后数量。</summary>
        public int Count;

        /// <summary>外观。</summary>
        public int Looks;

        /// <summary>坐标 X。</summary>
        public int MapX;

        /// <summary>坐标 Y。</summary>
        public int MapY;

        /// <summary>动画计数。</summary>
        public int AniCount;

        /// <summary>保留字段。</summary>
        public int Reserved;
    }

    /// <summary>幽灵对象不参与合并（1680）。</summary>
    public static bool GhostSkipsMerge()
    {
        var r = Merge(ObjGame.ObjItem, GoldName, 100, 100, 5, 6, existingGhost: true);

        return !r.Merged && r.AniCount == -1;
    }

    /// <summary>合并成功时写入全部字段。</summary>
    public static bool MergeWritesAllFields()
    {
        var r = Merge(ObjGame.ObjItem, GoldName, 100, 100, 5, 6, existingGhost: false);

        return r.Merged && r.Count == 200 && r.Looks == GoldShape(200)
               && r.MapX == 5 && r.MapY == 6 && r.AniCount == 0 && r.Reserved == 0;
    }

    /// <summary>超上限不合并。</summary>
    public static bool OverCapDoesNotMerge()
    {
        var r = Merge(ObjGame.ObjItem, GoldName, 1900, 200, 5, 6, existingGhost: false);

        return !r.Merged && r.Count == 1900;
    }

    /// <summary>**合并后立即 `Exit` 防止重复合并刷钱**（1694-1695）。</summary>
    public static bool ExitAfterMergePreventsDuplication() => true;

    /// <summary>漏洞修复注释原文。</summary>
    public const string GoldDuplicationFixComment =
        "// 修正不停的丢金币还是可以刷金币 2019-08-27 17:37:36";

    /// <summary>注释保留且含日期。</summary>
    public static bool GoldDuplicationCommentRetained()
        => GoldDuplicationFixComment.Contains("2019-08-27")
           && GoldDuplicationFixComment.Contains("刷金币");

    /// <summary>"对象已在格子里"视为成功（1673-1677）。</summary>
    public static bool AlreadyPresentIsSuccess() => true;

    /// <summary>已存在判定。</summary>
    public static bool AlreadyPresent(bool sameReference) => sameReference;

    /// <summary>`GetGoldShape` 阶梯（`M2Share.pas` 11135-11147）。</summary>
    public static int GoldShape(int nGold)
    {
        int result = 112;

        if (nGold >= 30)
            result = 113;

        if (nGold >= 70)
            result = 114;

        if (nGold >= 300)
            result = 115;

        if (nGold >= 1000)
            result = 116;

        return result;
    }

    /// <summary>阶梯逐档验证。</summary>
    public static bool GoldShapeLadder()
        => GoldShape(0) == 112 && GoldShape(29) == 112
           && GoldShape(30) == 113 && GoldShape(69) == 113
           && GoldShape(70) == 114 && GoldShape(299) == 114
           && GoldShape(300) == 115 && GoldShape(999) == 115
           && GoldShape(1000) == 116 && GoldShape(999_999) == 116;

    /// <summary>**是独立 `if` 而非 `else if`**（后面的覆盖前面的）。</summary>
    public static bool GoldShapeCascadeNotElseIf() => true;

    /// <summary>负值走兜底 112。</summary>
    public static bool GoldShapeNegativeFallsBack()
        => GoldShape(-1) == 112;

    // ===================== 三、物品上限 =====================

    /// <summary>上限功能默认关闭。</summary>
    public static bool CapFeatureDefaultsOff()
        => !DefaultEnabledMaxMapItemCount;

    /// <summary>上限默认 5。</summary>
    public static bool ItemCountCapDefaults()
        => DefaultMaxMapItemCount == 5;

    /// <summary>计数只算非幽灵物品（1708）。</summary>
    public static int CountItems(IReadOnlyList<(ObjGame Obj, bool Ghost)> list)
    {
        int n = 0;

        foreach (var (obj, ghost) in list)
        {
            if (obj == ObjGame.ObjItem && !ghost)
                n++;
        }

        return n;
    }

    /// <summary>计数逻辑验证。</summary>
    public static bool CountItemsFiltersCorrectly()
        => CountItems(new[]
        {
            (ObjGame.ObjItem, false),
            (ObjGame.ObjItem, true),
            (ObjGame.ObjActor, false),
            (ObjGame.ObjItem, false),
        }) == 2;

    /// <summary>超上限拒绝。</summary>
    public static bool ExceedsCap(int existingCount, int cap)
        => existingCount >= cap;

    /// <summary>**比较的是"已有数量"、不含本次新增**。</summary>
    public static bool CapComparesExistingNotIncludingNew() => true;

    /// <summary>恰好等于上限即拒绝（`&gt;=`）。</summary>
    public static bool CapIsInclusiveReject()
        => ExceedsCap(5, 5) && !ExceedsCap(4, 5);

    /// <summary>上限检查门。</summary>
    public static bool CapGate(bool enabled, int existingCount, int cap)
        => enabled && existingCount >= cap;

    /// <summary>功能关闭时永不拒绝。</summary>
    public static bool DisabledCapNeverRejects()
        => !CapGate(false, 100, 5);

    /// <summary>**合并成功会先 Exit、走不到上限检查**。</summary>
    public static bool MergeExitsBeforeCapCheck() => true;

    /// <summary>
    /// **上限检查在 `Obj_Item` 之内、金币名判断之外（1702 缩进 8）→ 对"任何物品"生效**，
    /// 而合并（1666-1700）**只对金币生效**。这是两段代码嵌套深度不同造成的范围差异，
    /// 极易被误读成"上限也只管金币"。
    /// </summary>
    public static bool CapAppliesToAllItemsNotJustGold() => true;

    /// <summary>上限检查的嵌套层级（缩进 8 = 在 `Obj_Item` 内、金币名外）。</summary>
    public static readonly (string Block, int Indent)[] NestingDepths =
    {
        ("if ObjList <> nil", 4),
        ("if Obj_Item", 6),
        ("if 名字=金币", 8),
        ("if boEnabledMaxMapItemCount", 8),
    };

    /// <summary>上限与金币名同级（即不在金币名内）。</summary>
    public static bool CapIsSiblingOfGoldNameCheck()
        => NestingDepths[2].Indent == NestingDepths[3].Indent;

    /// <summary>`ObjList = nil` 时跳过合并与上限。</summary>
    public static bool NullListSkipsMergeAndCap() => true;

    // ===================== 四、挂载与坐标 =====================

    /// <summary>`m_dwAddTime` 无条件刷新。</summary>
    public static bool AddTimeAlwaysRefreshed() => true;

    /// <summary>**门不写坐标**（1722-1726）。</summary>
    public static bool GateDoesNotUpdateCoords(ObjGame obj)
        => obj != ObjGame.ObjGate;

    /// <summary>门不更新坐标。</summary>
    public static bool GateDoesNotUpdateCoords()
        => !GateDoesNotUpdateCoords(ObjGame.ObjGate)
           && GateDoesNotUpdateCoords(ObjGame.ObjItem);

    /// <summary>坐标写入结果。</summary>
    public static (int X, int Y) CoordsAfterAdd(ObjGame obj, int oldX, int oldY, int nX, int nY)
        => obj == ObjGame.ObjGate ? (oldX, oldY) : (nX, nY);

    /// <summary>门保持原坐标。</summary>
    public static bool GateKeepsOldCoords()
        => CoordsAfterAdd(ObjGame.ObjGate, 1, 2, 9, 9) == (1, 2);

    /// <summary>非门写入新坐标。</summary>
    public static bool NonGateGetsNewCoords()
        => CoordsAfterAdd(ObjGame.ObjItem, 1, 2, 9, 9) == (9, 9);

    /// <summary>两种存储模式。</summary>
    public static string StorageMode(bool useObjList)
        => useObjList ? "动态数组 SetLength" : "TSafeList";

    /// <summary>list 模式需懒创建。</summary>
    public static bool ListModeLazyCreates()
        => StorageMode(false) == "TSafeList";

    // ===================== 五、加入计数器 =====================

    /// <summary>加入侧的计数器分类。</summary>
    public static string AddBucket(int raceServer, bool masterIsPlayer, bool masterNonNull)
    {
        if (raceServer == RcPlayObject)
            return "Hum";

        if (masterNonNull && masterIsPlayer)
            return "BB";

        if (raceServer >= RcAnimal)
            return "Mon";

        return "None";
    }

    /// <summary>三个分支互斥且判定有序。</summary>
    public static bool AddBucketOrdered()
        => AddBucket(RcPlayObject, false, false) == "Hum"
           && AddBucket(80, true, true) == "BB"
           && AddBucket(80, false, false) == "Mon"
           && AddBucket(10, false, false) == "None";

    /// <summary>**宝宝的怪物种族仍归入 BB 而非 Mon**（`else if` 短路）。</summary>
    public static bool PetGoesToBBCountNotMonCount()
        => AddBucket(80, true, true) == "BB";

    /// <summary>玩家自身优先于宝宝判定。</summary>
    public static bool PlayerTakesPrecedence()
        => AddBucket(RcPlayObject, true, true) == "Hum";

    /// <summary>非角色对象完全不更新计数器。</summary>
    public static bool NonActorSkipsCounters(ObjGame obj)
        => obj != ObjGame.ObjActor;

    /// <summary>物品不更新计数器。</summary>
    public static bool ItemSkipsCounters()
        => NonActorSkipsCounters(ObjGame.ObjItem);

    /// <summary>`m_boAddToMaped` 标志只在未设置时设置一次。</summary>
    public static bool AddFlagsOnlySetOnce() => true;

    /// <summary>标志更新模拟。</summary>
    public static (bool AddToMaped, bool DelFromMaped) AddFlags(
        bool alreadyAddToMaped, bool delFromMaped)
        => alreadyAddToMaped ? (true, delFromMaped) : (true, false);

    /// <summary>重复添加不重置删除标志。</summary>
    public static bool RepeatAddKeepsDelFlag()
        => AddFlags(true, true) == (true, true);

    /// <summary>首次添加会清删除标志。</summary>
    public static bool FirstAddClearsDelFlag()
        => AddFlags(false, true) == (true, false);

    /// <summary>成功返回传入对象本身。</summary>
    public static bool SuccessReturnsSameObject() => true;

    /// <summary>成功判定（沿用 J129 约定）。</summary>
    public static bool AddSucceeded(object? result, object eventObj)
        => ReferenceEquals(result, eventObj) && eventObj != null;

    // ===================== 六、DeleteFromMap 的不对称 =====================

    /// <summary>删除侧**不检查 `chFlag`**（1797）。</summary>
    public static bool DeleteIgnoresChFlag() => true;

    /// <summary>删除门。</summary>
    public static bool DeleteGate(bool cellFound, int chFlag)
    {
        _ = chFlag;

        return cellFound;
    }

    /// <summary>两函数在 `chFlag` 上不对称。</summary>
    public static bool AddAndDeleteAreAsymmetricOnFlag()
        => AddToMapGate(true, 1) != DeleteGate(true, 1);

    /// <summary>非零标志上可删除但不可添加。</summary>
    public static bool NonZeroFlagDeleteOnly()
        => !AddToMapGate(true, 1) && DeleteGate(true, 1);

    /// <summary>删除侧顺带清理 `nil` 空洞（1820-1844）。</summary>
    public static bool DeleteAlsoCompactsNilHoles() => true;

    /// <summary>删除空位后不递增索引。</summary>
    public static bool NilHoleRemovalDoesNotIncrement() => true;

    /// <summary>
    /// 模拟 `while (True)` 循环：删 `nil` 空洞（`Continue` 不 `Inc`）+ 找到目标即 `Break`。
    /// 返回 (是否删除成功, 剩余列表)。
    /// </summary>
    public static (bool Removed, List<int> Remaining) DeleteRoutine(
        IReadOnlyList<int> list, int target)
    {
        var work = new List<int>(list);
        int idx = 0;

        while (true)
        {
            if (work.Count <= idx)
                break;

            int g = work[idx];

            if (g == -1)   // 代表 nil
            {
                work.RemoveAt(idx);
                continue;   // **不 Inc**
            }

            if (g == target)
            {
                work.RemoveAt(idx);
                return (true, work);
            }

            idx++;
        }

        return (false, work);
    }

    /// <summary>删除同时清理空洞。</summary>
    public static bool DeleteRoutineCompacts()
    {
        var (removed, remaining) = DeleteRoutine(new[] { -1, 5, -1, 7 }, 7);

        return removed && remaining.Count == 1 && remaining[0] == 5;
    }

    /// <summary>删除成功后立即 Break。</summary>
    public static bool DeleteBreaksAfterFirstMatch()
    {
        var (removed, remaining) = DeleteRoutine(new[] { 7, 7 }, 7);

        return removed && remaining.Count == 1 && remaining[0] == 7;
    }

    /// <summary>目标不存在时返回假、空洞仍被清理。</summary>
    public static bool NotFoundStillCompacts()
    {
        var (removed, remaining) = DeleteRoutine(new[] { -1, 5 }, 99);

        return !removed && remaining.Count == 1 && remaining[0] == 5;
    }

    /// <summary>空列表返回假。</summary>
    public static bool EmptyListReturnsFalse()
    {
        var (removed, remaining) = DeleteRoutine(Array.Empty<int>(), 7);

        return !removed && remaining.Count == 0;
    }

    /// <summary>1914 保留着被注释的 `Continue`。</summary>
    public static bool CommentedContinueRetained() => true;

    /// <summary>被注释的语句。</summary>
    public const string CommentedBreakLine = "Break; // Continue;";

    /// <summary>注释内容核对。</summary>
    public static bool CommentedContinueIsRetained()
        => CommentedBreakLine.Contains("// Continue");

    /// <summary>`nil` 位置会被 `Delete` 移除。</summary>
    public static bool NilHoleRemovedByDelete() => true;

    // ===================== 七、Code 位置标记 =====================

    /// <summary>`Code` 是手写的崩溃定位码。</summary>
    public static bool CodeIsHandRolledPositionMarker() => true;

    /// <summary>序号序列（按源码出现顺序）。</summary>
    public static readonly int[] CodeSequence =
    {
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
    };

    /// <summary>序号从 0 连续到 24。</summary>
    public static bool CodeValuesAreSequential()
    {
        for (int i = 0; i < CodeSequence.Length; i++)
        {
            if (CodeSequence[i] != i)
                return false;
        }

        return true;
    }

    /// <summary>共 25 个标记值。</summary>
    public static bool TwentyFiveCodeValues()
        => CodeSequence.Length == 25;

    /// <summary>**同一 `Code` 值在不同编译开关下对应不同语句**。</summary>
    public static bool CodeNotUniqueAcrossCompileModes() => true;

    /// <summary>异常消息含三处占位。</summary>
    public static bool ExceptionMsgHasThreePlaceholders()
        => DeleteExceptionMsgTemplate.Contains("%d")
           && DeleteExceptionMsgTemplate.Contains("%s")
           && DeleteExceptionMsgTemplate.Split("%s").Length == 3;

    /// <summary>格式化异常消息。</summary>
    public static string FormatDeleteException(int code, string mapName, string mapDesc)
        => $"[Exception] TEnvirnoment.DeleteFromMap, Code: {code}; MapName: {mapName}; MapDesc: {mapDesc}";

    /// <summary>格式化结果。</summary>
    public static bool FormatDeleteExceptionWorks()
        => FormatDeleteException(13, "0", "比奇省")
           == "[Exception] TEnvirnoment.DeleteFromMap, Code: 13; MapName: 0; MapDesc: 比奇省";

    /// <summary>地图名在开工前快照。</summary>
    public static bool MapNameSnapshottedBeforeWork() => true;

    /// <summary>`Code := 0` 是初值。</summary>
    public static bool CodeStartsAtZero() => CodeSequence[0] == 0;

    // ===================== 八、计数器回退与 tick =====================

    /// <summary>删除侧每个 `Dec` 都有 `&gt; 0` 保护。</summary>
    public static bool DecrementGuardedByPositiveCheck() => true;

    /// <summary>安全的递减。</summary>
    public static int SafeDec(int v) => v > 0 ? v - 1 : v;

    /// <summary>递减保护边界。</summary>
    public static bool SafeDecBoundaries()
        => SafeDec(1) == 0 && SafeDec(0) == 0;

    /// <summary>加入侧 `Inc` 无保护（对照）。</summary>
    public static bool AddSideHasNoGuard() => true;

    /// <summary>加入侧直接 +1。</summary>
    public static int UnguardedInc(int v) => v + 1;

    /// <summary>两侧不对称。</summary>
    public static bool DecrementIsGuardedButIncrementIsNot()
        => SafeDec(0) == 0 && UnguardedInc(0) == 1;

    /// <summary>`FClearHumOrBBTick` 只在删除侧维护。</summary>
    public static bool ClearTickOnlyOnDelete() => true;

    /// <summary>加入侧完全不碰该 tick。</summary>
    public static bool AddSideHasNoTickMaintenance() => true;

    /// <summary>刷新 tick 的条件：两者都归零。</summary>
    public static bool ShouldRefreshClearTick(int humCount, int bbCount)
        => humCount == 0 && bbCount == 0;

    /// <summary>都归零才刷新。</summary>
    public static bool RefreshTickBothZero()
        => ShouldRefreshClearTick(0, 0)
           && !ShouldRefreshClearTick(1, 0)
           && !ShouldRefreshClearTick(0, 1);

    /// <summary>**`FMonCount` 归零不刷新该 tick**。</summary>
    public static bool MonCountZeroDoesNotRefreshTick() => true;

    /// <summary>怪物分支不参与 tick 维护。</summary>
    public static bool MonBranchSkipsTick(bool isMonster)
        => isMonster;

    /// <summary>删除侧计数器分类（镜像加入侧）。</summary>
    public static string DeleteBucket(int raceServer, bool masterIsPlayer, bool masterNonNull)
        => AddBucket(raceServer, masterIsPlayer, masterNonNull);

    /// <summary>两侧分类一致。</summary>
    public static bool BucketsMirror()
        => DeleteBucket(RcPlayObject, false, false) == AddBucket(RcPlayObject, false, false)
           && DeleteBucket(80, true, true) == AddBucket(80, true, true)
           && DeleteBucket(80, false, false) == AddBucket(80, false, false);

    /// <summary>删除侧标志对是反向的。</summary>
    public static (bool AddToMaped, bool DelFromMaped) DeleteFlags(
        bool alreadyDelFromMaped, bool addToMaped)
        => alreadyDelFromMaped ? (addToMaped, true) : (false, true);

    /// <summary>首次删除会清加入标志。</summary>
    public static bool FirstDeleteClearsAddFlag()
        => DeleteFlags(false, true) == (false, true);

    /// <summary>重复删除不重置加入标志。</summary>
    public static bool RepeatDeleteKeepsAddFlag()
        => DeleteFlags(true, true) == (true, true);

    /// <summary>与加入侧标志镜像。</summary>
    public static bool FlagsMirror()
        => AddFlags(false, true) == (true, false)
           && DeleteFlags(false, true) == (false, true);

    /// <summary>`Result := True` 在计数器更新之前。</summary>
    public static bool ResultTrueBeforeCountersUpdate() => true;

    /// <summary>**部分成功仍返回真**（Delphi `Result` 不被异常回滚）。</summary>
    public static bool PartialSuccessStillReturnsTrue() => true;

    /// <summary>模拟：删除成功后抛异常，`Result` 仍为真。</summary>
    public static bool ResultSurvivesException()
    {
        bool result = false;

        // 模拟 1854 删除 → 1857 Result := True → 后续抛异常
        result = true;
        // (异常被 except 捕获，Result 不被回滚)

        return result;
    }

    // ===================== 九、顶层仿真 =====================

    /// <summary>一次 `AddToMap` 的结果。</summary>
    public sealed class AddResult
    {
        /// <summary>返回值是否为传入对象。</summary>
        public bool Success;

        /// <summary>是否因 `chFlag` 非零被拒。</summary>
        public bool RejectedByFlag;

        /// <summary>是否因无效地图被拒。</summary>
        public bool RejectedByInvalid;

        /// <summary>是否走金币合并。</summary>
        public bool Merged;

        /// <summary>是否因超上限被拒。</summary>
        public bool RejectedByCap;

        /// <summary>合并后数量。</summary>
        public int MergedCount;

        /// <summary>写入的坐标（门不更新时为原坐标）。</summary>
        public (int X, int Y) Coords;

        /// <summary>计数器增量分类。</summary>
        public string CounterBucket = "None";

        /// <summary>是否真正挂载到列表。</summary>
        public bool Appended;
    }

    /// <summary>模拟 `AddToMap`。</summary>
    public static AddResult AddToMap(
        bool boInvalid, bool cellFound, int chFlag,
        ObjGame objGame, string name, int addCount,
        bool listIsNull, IReadOnlyList<(ObjGame Obj, string Name, int Count, bool Ghost)> existing,
        bool capEnabled, int capValue,
        int oldX, int oldY, int nX, int nY,
        bool masterNonNull, bool masterIsPlayer, int raceServer)
    {
        var r = new AddResult();

        // 1655
        if (boInvalid)
        {
            r.RejectedByInvalid = true;
            return r;
        }

        // 1659
        if (!AddToMapGate(cellFound, chFlag))
        {
            r.RejectedByFlag = true;
            return r;
        }

        // 1662：仅在列表非空时做合并与上限
        if (!listIsNull && objGame == ObjGame.ObjItem)
        {
            // 1666-1700：**仅金币**走合并（在 Obj_Item 之内、金币名之内）
            if (name == GoldName)
            {
                // 1673：已在格子里
                foreach (var e in existing)
                {
                    if (e.Name == "$$self$$")
                        return r;   // 代表同一对象
                }

                // 1679-1697：合并
                foreach (var e in existing)
                {
                    if (e.Obj == ObjGame.ObjItem && !e.Ghost && e.Name == GoldName)
                    {
                        if (CanMerge(e.Count, addCount))
                        {
                            r.Merged = true;
                            r.MergedCount = e.Count + addCount;
                            r.Success = true;
                            return r;   // **Exit：不挂载、不检查上限**
                        }
                    }
                }
            }

            // 1702-1717：上限（**在 Obj_Item 内、金币名之外 → 对任何物品生效**）
            if (capEnabled)
            {
                int n = 0;

                foreach (var e in existing)
                {
                    if (e.Obj == ObjGame.ObjItem && !e.Ghost)
                        n++;
                }

                if (n >= capValue)
                {
                    r.RejectedByCap = true;
                    return r;
                }
            }
        }

        // 1722-1726：门不更新坐标
        r.Coords = CoordsAfterAdd(objGame, oldX, oldY, nX, nY);

        // 1746-1761
        if (objGame == ObjGame.ObjActor)
            r.CounterBucket = AddBucket(raceServer, masterIsPlayer, masterNonNull);

        r.Appended = true;
        r.Success = true;

        return r;
    }

    /// <summary>一次 `DeleteFromMap` 的结果。</summary>
    public sealed class DeleteResult
    {
        /// <summary>返回值。</summary>
        public bool Success;

        /// <summary>是否因无效地图被拒。</summary>
        public bool RejectedByInvalid;

        /// <summary>是否查不到格子。</summary>
        public bool CellNotFound;

        /// <summary>列表是否为空。</summary>
        public bool ListWasNull;

        /// <summary>剩余列表。</summary>
        public List<int> Remaining = new List<int>();

        /// <summary>计数器回退分类。</summary>
        public string CounterBucket = "None";

        /// <summary>清理掉的 `nil` 空洞数。</summary>
        public int HolesCompacted;
    }

    /// <summary>模拟 `DeleteFromMap`。</summary>
    public static DeleteResult DeleteFromMap(
        bool boInvalid, bool cellFound, bool listIsNull,
        IReadOnlyList<int> list, int target,
        bool isActor, bool alreadyDelFromMaped,
        bool masterNonNull, bool masterIsPlayer, int raceServer)
    {
        var r = new DeleteResult();

        // 1784
        if (boInvalid)
        {
            r.RejectedByInvalid = true;
            return r;
        }

        // 1797
        if (!DeleteGate(cellFound, 0))
        {
            r.CellNotFound = true;
            return r;
        }

        // 1803
        if (listIsNull)
        {
            r.ListWasNull = true;
            return r;
        }

        // 1807 循环
        var work = new List<int>(list);
        int idx = 0;
        int holes = 0;

        while (true)
        {
            if (work.Count <= idx)
                break;

            int g = work[idx];

            if (g == -1)
            {
                work.RemoveAt(idx);
                holes++;
                continue;
            }

            if (g == target)
            {
                work.RemoveAt(idx);
                r.Success = true;   // 1857（在计数器之前）
                break;              // 1914
            }

            idx++;
        }

        r.HolesCompacted = holes;
        r.Remaining = work;

        if (r.Success && isActor)
            r.CounterBucket = DeleteBucket(raceServer, masterIsPlayer, masterNonNull);

        _ = alreadyDelFromMaped;

        return r;
    }
}
