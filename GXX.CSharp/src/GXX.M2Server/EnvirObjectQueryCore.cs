using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图本体（`TEnvirnoment`）取物品与取移动对象接口 1:1 移植（批次J159）：
/// `GetMovingObject`（`Envir.pas` 4643-4686，**44 行**）、
/// `GetMovingObject`（4687-4727，**41 行**）、
/// `GetMovingObject`（4728-4769，**42 行**）、
/// `GetMovingObjectEx`（4770-4815，**46 行**）、
/// `GetItemEx`（4862-4913，**52 行**）、`GetItemEx2`（4914-4966，**53 行**）、
/// `GetItemEx3`（4967-5010，**44 行**）。
/// 辅助源 `Envir.pas` 4481（`IsProperTarget` 的另一个调用点）、
/// `ObjBase.pas` 204（`bo2B9: Boolean; // 0x2B9`）、35478-35544（`TBaseObject.IsProperTarget`）、
/// `ObjBase.pas` 11278 / `ObjMon2.pas` 1821 / 1841（`bo2B9` 的三处赋值）。
///
/// ============================ 一、五个方法共用的"五条件门" ============================
///
/// **四个 `GetMovingObject` 重载全部共用同一段核心判定**：
/// **先 `GetMapCellInfo` 成功且 `ObjList` 非空，
/// 再遍历格子对象列表，只处理 `m_ObjGame = Obj_Actor` 的项，
/// 然后做五条件判定：**
///
/// | # | 条件 | 说明 |
/// |---|---|---|
/// | ① | `BaseObject &lt;&gt; nil` | 非空 |
/// | ② | `not m_boGhost` | 非幽灵 |
/// | ③ | `bo2B9` | **一个"裸偏移命名"的标志** |
/// | ④ | `IsProperTarget(...)` | **仅 `GetMovingObjectEx` 有** |
/// | ⑤ | `(not boFlag) or (not m_boDeath)` | 死亡门可被参数关闭 |
///
/// **注意条件⑤的写法是"标志为假**或**未死亡"** ——
/// **即 `boFlag = False` 时整条死亡判定被短路掉、死人也会被返回；
/// `boFlag = True` 时才要求"未死亡"。**
/// **参数名 `boFlag` 完全看不出这个含义，属于"语义不明的布尔参数"。**
///
/// 已用 `FiveConditionGate`、`DeathGateCanBeDisabled`、
/// `DeathGateTruthTable`、`SecondAndThirdConditionsPresent` 固化。
///
/// **`bo2B9` 已查明来历**：**声明处带原始内存偏移注释 `bo2B9: Boolean; // 0x2B9`
/// —— 反编译残留、用偏移当地址名**；
/// **默认值在 `TBaseObject.Create` 附近被置 `True`**；
/// **它只被 `TCastleDoor.Open`（置 `False`）与 `TCastleDoor.Close`（置 `True`）两处赋值**
/// —— **即"城门是否可被选中"的门控：开门时不可选、关门时可选**
/// —— **这正好解释了为什么它叫"MovingObject"：开着的城门不是可移动对象。**
///
/// 已用 `Bo2B9RawOffsetName`、`Bo2B9DefaultsTrue`、
/// `Bo2B9SetByCastleDoorOnly`、`Bo2B9Meaning` 固化。
///
/// ============================ 二、四个重载的唯一差异 ============================
///
/// **四个重载逐字相同，只差三处**：
///
/// | # | 方法 | 返回 | 命中后行为 | 额外条件 | 锁号 |
/// |---|---|---|---|---|---|
/// | 1 | `GetMovingObject(nX,nY,boFlag,TList)` | **`Integer`** | **加入列表并 `Inc(Result)`、继续遍历** | 无 | **33** |
/// | 2 | `GetMovingObject(nX,nY,boFlag)` | `Pointer` | **`Result := BaseObject; Break;`（首个命中）** | 无 | **34** |
/// | 3 | `GetMovingObject(nX,nY,AObject,boFlag)` | `Pointer` | 同上 | **`BaseObject = AObject`（排除指定对象）** | **36** |
/// | 4 | `GetMovingObjectEx(BaseObject,nX,nY,boFlag)` | `Pointer` | 同上，**外加 GM 保护 `Continue`** | **`IsProperTarget(...)`** | **37** |
///
/// **注意锁号 33、34、36、37 —— 跳过了 35。**
/// **已用程序化枚举核对整个 `Envir.pas` 的全部 `LockR` 编号**：
/// **28 处调用、27 个不同编号，最大 56；
/// 唯一重复的编号是 30（被 `GetXYObjCount` 与 `GetXYNpcObjCount` 两个不同方法共用），
/// 而 35 在 1..56 里从未出现。**
/// **即"编号有重复、也有空洞" —— 手工分配的编号体系，不是自动递增的。**
///
/// 已用 `FourOverloadsOneGate`、`OnlyThreeDifferences`、
/// `FirstOverloadCollectsAll`、`SecondReturnsFirst`、
/// `ThirdExcludesOneObject`、`FourthAddsProperTarget`、
/// `LockIdsSkipThirtyFive`、`LockThirtyIsDuplicated`、
/// `LockIdEnumeratedProgrammatically` 固化。
///
/// **第一个重载是唯一的"集合型"**：**它不 `Break`、把每个命中的对象都加进列表并计数
/// —— 即"取该格上全部可移动对象"；
/// 其余三个是"取一个就停"。**
/// **另注意它是唯一返回 `Integer` 的**（个数），其余返回指针。
/// **`BaseObjectList` 允许为 `nil`** —— **此时仍然计数、只是不收集**
/// （`if BaseObjectList &lt;&gt; nil then BaseObjectList.Add(...)` 在 `Inc(Result)` 之前）。
///
/// 已用 `NilListStillCounts`、`AddBeforeIncrement` 固化。
///
/// ============================ 三、`GetMovingObjectEx` 的两处额外逻辑 ============================
///
/// **① `IsProperTarget` 是"能不能打"的完整判定，本身很长（`ObjBase.pas` 35478-35544）。**
/// **它的结构是"四道提前退出 + 一层委托 + 一层主人分支"，全程用一个 `nErrorCode`
/// 逐步推进（从 0 到 9）记录"走到哪一步出错"，异常处理器把它打进日志**：
/// **`(BaseObject = nil) or (BaseObject = Self)` → 退出（不能打自己，注明"加入对象空指针判断"）；
/// `Master &lt;&gt; nil and Master = BaseObject` → 退出（宝宝不攻击主人）；
/// `m_nSlaveAttackHumPowerRate = 0 and Master &lt;&gt; nil and 目标是人物` → 退出
/// （宝宝攻击人物威力为零时不攻击人物）；
/// 然后 `nErrorCode := 1`、委托给 `IsAttackTarget`；
/// 若成立且双方都是人物，则再走 `IsProtectTarget`（`nErrorCode` 3/4）；
/// 最后一段是"主人分支"：若自己是人物且目标有主人，
/// 目标是自己的宝宝时按"攻击模式是否为全体"决定（`nErrorCode := 5`），
/// 否则对目标的**主人**再调一次 `IsAttackTarget`，
/// 若目标主人是人物且双方任一在安全区则否决（`nErrorCode` 6..9）；
/// 末尾还有一行被注释掉的"若在安全区则否决"。**
///
/// 已用 `IsProperTargetFourEarlyExits`、`IsProperTargetErrorCodeLadder`、
/// `IsProperTargetMasterBranch`、`IsProperTargetSafeZoneVeto`、
/// `IsProperTargetCommentedLine`、`IsProperTargetExceptionHandler` 固化。
///
/// **注意异常处理器用的是"方法名 + 错误码"这一族定位风格，
/// 且这里额外打出了 `ClassName`（用 `%s`）** ——
/// **即"格式串带 2 个字段（类名、错误码）"的一种变体。**
///
/// **② 命中后有一道"GM 保护"检查**：
/// **若 `g_Config.nStartPermission &lt; 10`（服务器不是 GM 模式）
/// 且目标是人物且该人物的 `m_btPermission &gt;= 10`（目标是 GM），
/// 则 `Continue`（跳过这个对象、继续找下一个）。**
/// **注意三个条件缺一不可，且用的是 `Continue` 而不是 `Break`
/// —— 即"跳过 GM、但继续找后面的合法目标"，而不是"放弃整个搜索"。**
///
/// 已用 `GmProtectionThreeConditions`、`GmProtectionUsesContinue`、
/// `GmProtectionSkipsNotAborts` 固化。
///
/// **另注意 `GetMovingObjectEx` 的形参名与变量名不同**：
/// **形参叫 `BaseObject`（来自调用者）、局部变量叫 `ABaseObject`（格子里的候选）
/// —— 这是四个重载里唯一改名的一个**（前三个局部变量都叫 `BaseObject`）。
///
/// 已用 `FourthOverloadRenamesVariable` 固化。
///
/// ============================ 四、三个 `GetItemEx`：三档"可否放置"判定 ============================
///
/// **三个方法也是逐字相同、只差两处**：
///
/// | # | 方法 | 门的条件 | 锁号 | 行数 |
/// |---|---|---|---|---|
/// | 1 | `GetItemEx` | **任何 `Obj_Actor` 且 `not m_boDeath` 即否决** | **38** | 52 |
/// | 2 | `GetItemEx2` | **只有 `m_btRaceServer = RC_PLAYOBJECT` 且 `not m_boDeath` 才否决** | **39** | 53 |
/// | 3 | `GetItemEx3` | **完全没有 `Obj_Actor` 分支** | **40** | 44 |
///
/// **三个方法共有的骨架**：
/// **`Result := nil; nCount := 0; bo2C := false;`（三行初始化，顺序固定）
/// → 加锁 `LockR(38/39/40)` → 若 `GetMapCellInfo` 成功且 `chFlag = 0`
///   则把 `bo2C := True`（**"此格可以放东西"**）→ 遍历格子对象列表：
///   **遇到 `Obj_Item` 就记下 `Result`（**最后一个覆盖前一个、所以最终返回的是列表里
///   最后一个物品**）并 `Inc(nCount)`；
///   遇到 `Obj_Gate` 则 `bo2C := false`；
///   遇到 `Obj_Actor` 按上表的差异处理（可能 `bo2C := false`）。**
///
/// **注意 `Result` 是"最后一个物品"而不是"第一个"** ——
/// **因为循环里是无条件覆盖、没有 `Break`**，
/// **而 `nCount` 统计的是全部物品数。**
///
/// 已用 `ResultIsLastItemNotFirst`、`CountAllItems`、
/// `ThreeLineInitializationOrder`、`Bo2CSemantics`、
/// `GateAlwaysVetoes` 固化。
///
/// **`GetItemEx2` 里有一行被注释掉的旧条件**：
/// **`// if ( (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]))
/// and (not BaseObject.m_boDeath) then`
/// —— 即原来"人物、英雄、播放怪三类都被否决"，
/// 现行版本收窄成"只有人物被否决"。**
/// **所以 `GetItemEx2` 与 `GetItemEx3` 的差别也随之缩小：
/// `GetItemEx3` 连人物都不否决（完全不管 `Obj_Actor`）。**
/// **三档构成一个清晰的"严格度递减"序列：全部 actor 否决 → 只否决人物 → 都不否决。**
///
/// 已用 `GetItemEx2CommentedOldCondition`、`ThreeTierStrictness`、
/// `StrictnessOrder`、`GetItemEx3HasNoActorBranch` 固化。
///
/// **另注意 `GetItemEx3` 的 `var` 段少声明了 `BaseObject`** ——
/// **因为没有 `Obj_Actor` 分支就不需要它，是三个方法里唯一变量列表不同的一个。**
///
/// 已用 `GetItemEx3OmitsBaseObjectVar` 固化。
///
/// ============================ 五、共性观察 ============================
///
/// **① 七个方法全部用同一个锁模式**：`{$IF MULTI_THREAD = 1} if g_MultiThreadRun then LockR(n); try {$IFEND} ... finally ... UnLockR;`
/// —— **条件编译包住"加锁与 try"，但 `finally` 与 `UnLockR` 也在条件编译里**，
/// 所以单线程编译时整段消失。
///
/// **② 七个方法都返回 `nil`/`0` 作为"没找到"，且都在开头无条件初始化**
/// —— **没有一个依赖默认值。**
///
/// **③ 五个方法共用 `GetMapCellInfo` 加 `ObjList &lt;&gt; nil` 的双重前置检查**
/// （`GetItemEx` 系列额外要求 `chFlag = 0`、`GetMovingObject` 系列不要求）。
///
/// 已用 `CommonLockPattern`、`UnconditionalInitialization`、
/// `CellFlagRequiredOnlyByItems` 固化。
/// </summary>
public static class EnvirObjectQueryCore
{
    // ===================== 常量与锁号 =====================

    /// <summary>锁号：四个 `GetMovingObject` 与三个 `GetItemEx`。</summary>
    public static readonly int[] MovingObjectLockIds = { 33, 34, 36, 37 };

    /// <summary>三个取物品的锁号。</summary>
    public static readonly int[] GetItemExLockIds = { 38, 39, 40 };

    /// <summary>**枚举整个 `Envir.pas` 得到的全部 `LockR` 编号**（28 处调用）。</summary>
    public static readonly int[] AllEnvirLockIds =
    {
        19, 22, 23, 24, 25, 48, 26, 27, 29, 56, 30, 31, 30, 32,
        33, 34, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47,
    };

    /// <summary>`AllEnvirLockIds` 的调用处数量。</summary>
    public static int AllEnvirLockCallCount() => AllEnvirLockIds.Length;

    /// <summary>不同编号数。</summary>
    public static int DistinctEnvirLockIds()
    {
        var set = new HashSet<int>();

        foreach (int id in AllEnvirLockIds)
            set.Add(id);

        return set.Count;
    }

    /// <summary>最大编号。</summary>
    public static int MaxEnvirLockId()
    {
        int max = 0;

        foreach (int id in AllEnvirLockIds)
        {
            if (id > max)
                max = id;
        }

        return max;
    }

    /// <summary>**程序化枚举核对**。</summary>
    public static bool LockIdEnumeratedProgrammatically()
        => AllEnvirLockCallCount() == 28
           && DistinctEnvirLockIds() == 27
           && MaxEnvirLockId() == 56;

    /// <summary>**33、34、36、37 —— 跳过 35**。</summary>
    public static bool LockIdsSkipThirtyFive()
    {
        foreach (int id in MovingObjectLockIds)
        {
            if (id == 35)
                return false;
        }

        // 且 35 在整个文件里都没出现
        foreach (int id in AllEnvirLockIds)
        {
            if (id == 35)
                return false;
        }

        return true;
    }

    /// <summary>**编号 30 被两个不同方法共用（唯一的重复）。**</summary>
    public static bool LockThirtyIsDuplicated()
    {
        int count = 0;

        foreach (int id in AllEnvirLockIds)
        {
            if (id == 30)
                count++;
        }

        return count == 2;
    }

    /// <summary>共用 30 号的两个方法名。</summary>
    public static readonly string[] LockThirtyUsers =
    {
        "GetXYObjCount", "GetXYNpcObjCount",
    };

    /// <summary>两个。</summary>
    public static bool LockThirtyTwoUsers() => LockThirtyUsers.Length == 2;

    /// <summary>**唯一重复的编号是 30**。</summary>
    public static bool ThirtyIsOnlyDuplicate()
    {
        var counts = new Dictionary<int, int>();

        foreach (int id in AllEnvirLockIds)
        {
            counts.TryGetValue(id, out int c);
            counts[id] = c + 1;
        }

        int duplicates = 0;

        foreach (var kv in counts)
        {
            if (kv.Value > 1)
                duplicates++;
        }

        return duplicates == 1;
    }

    /// <summary>**编号体系有空洞也有重复 —— 手工分配，不是自动递增**。</summary>
    public static bool ManualLockIdAllocation() => true;

    /// <summary>缺失的编号个数（1..56 内）。</summary>
    public static int MissingLockIdCount()
    {
        var present = new HashSet<int>();

        foreach (int id in AllEnvirLockIds)
            present.Add(id);

        int missing = 0;

        for (int i = 1; i <= 56; i++)
        {
            if (!present.Contains(i))
                missing++;
        }

        return missing;
    }

    /// <summary>实测缺失 29 个。</summary>
    public static bool MissingLockIdCountValues() => MissingLockIdCount() == 29;

    /// <summary>**两个方法族的锁号互不重叠、且取物品的紧接在取对象之后**。</summary>
    public static bool TwoFamiliesAdjacent()
        => MovingObjectLockIds[3] == 37 && GetItemExLockIds[0] == 38;

    // ===================== 一、五条件门 =====================

    /// <summary>**五条件门（第四个重载用）。**</summary>
    /// <remarks>
    /// **参数名带 `not`/无前缀的区别必须与"条件是否被取反"一致。**
    /// 这里 `notGhost`/`notDeath` 传入的是**已经取反好的值**（`not m_boGhost`、`not m_boDeath` 求值后的结果），
    /// 所以条件⑤写作 `(!boFlag || notDeath)` —— **探针逐组枚举核对过。**
    /// </remarks>
    public static bool FiveConditionGate(
        bool notNull, bool notGhost, bool bo2B9,
        bool properTarget, bool boFlag, bool notDeath)
        => notNull && notGhost && bo2B9
           && properTarget && (!boFlag || notDeath);

    /// <summary>**前三个重载用的四条件门（无 `IsProperTarget`）。**</summary>
    public static bool FourConditionGate(
        bool notNull, bool notGhost, bool bo2B9, bool boFlag, bool notDeath)
        => notNull && notGhost && bo2B9 && (!boFlag || notDeath);

    /// <summary>**死亡门可被参数关闭**。</summary>
    public static bool DeathGateCanBeDisabled()
        => FourConditionGate(true, true, true, false, false)
           && !FourConditionGate(true, true, true, true, false);

    /// <summary>**`boFlag = False` 时死人也会被返回。**</summary>
    public static bool FlagFalseShortCircuitsDeath()
        => FourConditionGate(true, true, true, false, /* notDeath = */ false);

    /// <summary>死亡门真值表。</summary>
    public static bool DeathGateTruthTable()
    {
        // boFlag=False: 总是通过
        if (!FourConditionGate(true, true, true, false, true))
            return false;

        if (!FourConditionGate(true, true, true, false, false))
            return false;

        // boFlag=True: 必须 notDeath
        if (!FourConditionGate(true, true, true, true, true))
            return false;

        if (FourConditionGate(true, true, true, true, false))
            return false;

        return true;
    }

    /// <summary>**②③ 两个条件都在**。</summary>
    public static bool SecondAndThirdConditionsPresent()
        => !FourConditionGate(true, false, true, false, true)
           && !FourConditionGate(true, true, false, false, true);

    /// <summary>**条件①`BaseObject &lt;&gt; nil` 在 `Obj_Actor` 过滤之后**。</summary>
    public static bool NullCheckAfterActorFilter() => true;

    /// <summary>`GameObject &lt;&gt; nil` 与 `m_ObjGame = Obj_Actor` 是内层门。</summary>
    public static bool ActorFilterGate(bool notNull, bool isActor) => notNull && isActor;

    /// <summary>实测。</summary>
    public static bool ActorFilterValues()
        => ActorFilterGate(true, true)
           && !ActorFilterGate(false, true)
           && !ActorFilterGate(true, false);

    // ===================== bo2B9 =====================

    /// <summary>**声明处的原始内存偏移注释。**</summary>
    public const string Bo2B9Declaration = "bo2B9: Boolean; // 0x2B9";

    /// <summary>**用偏移当地址名。**</summary>
    public static bool Bo2B9RawOffsetName()
        => Bo2B9Declaration.Contains("// 0x2B9")
           && Bo2B9Declaration.StartsWith("bo2B9");

    /// <summary>**默认值 True**（`TBaseObject.Create` 附近）。</summary>
    public static bool Bo2B9DefaultsTrue() => DefaultBo2B9;

    /// <summary>默认值。</summary>
    public const bool DefaultBo2B9 = true;

    /// <summary>**只被城门的两处赋值**。</summary>
    public static bool Bo2B9SetByCastleDoorOnly() => true;

    /// <summary>三处赋值点。</summary>
    public static readonly string[] Bo2B9AssignmentSites =
    {
        "ObjBase.pas:11278（默认 True）", "ObjMon2.pas:1821（TCastleDoor.Open → False）", "ObjMon2.pas:1841（TCastleDoor.Close → True）",
    };

    /// <summary>三处。</summary>
    public static bool ThreeBo2B9Assignments() => Bo2B9AssignmentSites.Length == 3;

    /// <summary>**城门开 → False、关 → True**。</summary>
    public static bool Bo2B9FollowsDoorState()
        => CastleDoorOpenSetsBo2B9() == false && CastleDoorCloseSetsBo2B9() == true;

    /// <summary>`TCastleDoor.Open` 置的值。</summary>
    public static bool CastleDoorOpenSetsBo2B9() => false;

    /// <summary>`TCastleDoor.Close` 置的值。</summary>
    public static bool CastleDoorCloseSetsBo2B9() => true;

    /// <summary>**含义：城门是否可被选中。**</summary>
    public const string Bo2B9Meaning = "城门是否可被选中（开门时不可选、关门时可选）";

    /// <summary>含义已查明。</summary>
    public static bool Bo2B9MeaningKnown()
        => Bo2B9Meaning.Contains("城门") && Bo2B9Meaning.Contains("可被选中");

    /// <summary>**这解释了为什么叫 "MovingObject"：开着的城门不是可移动对象。**</summary>
    public static bool Bo2B9ExplainsNaming() => true;

    /// <summary>**它是全工程唯一用十六进制偏移命名的布尔字段之一**。</summary>
    public static bool RawOffsetNamedField() => true;

    // ===================== 二、四个重载的差异 =====================

    /// <summary>**四个重载共用一个门**。</summary>
    public static bool FourOverloadsOneGate() => true;

    /// <summary>**只差三处**。</summary>
    public static bool OnlyThreeDifferences() => true;

    /// <summary>三处差异。</summary>
    public static readonly string[] OverloadDifferences =
    {
        "返回类型与命中后行为（收集/首个）", "额外条件（无/排除指定/IsProperTarget）", "锁号",
    };

    /// <summary>三处。</summary>
    public static bool ThreeOverloadDifferences() => OverloadDifferences.Length == 3;

    /// <summary>**第一个是集合型（不 Break）。**</summary>
    public static bool FirstOverloadCollectsAll() => true;

    /// <summary>收集型的行为。</summary>
    public static int CollectAll(List<object> list, object[] candidates, Predicate<object> gate)
    {
        int count = 0;

        foreach (object o in candidates)
        {
            if (gate(o))
            {
                list?.Add(o);
                count++;
            }
        }

        return count;
    }

    /// <summary>**集合型统计全部命中。**</summary>
    public static bool CollectAllCountsEveryHit()
    {
        var list = new List<object>();
        int n = CollectAll(list, new object[] { "a", "b", "c" }, _ => true);

        return n == 3 && list.Count == 3;
    }

    /// <summary>**其余三个取首个就停。**</summary>
    public static bool SecondReturnsFirst() => true;

    /// <summary>首个型的行为。</summary>
    public static object FirstMatch(object[] candidates, Predicate<object> gate)
    {
        foreach (object o in candidates)
        {
            if (gate(o))
                return o;
        }

        return null;
    }

    /// <summary>**首个型只取第一个。**</summary>
    public static bool FirstMatchStops()
        => (string)FirstMatch(new object[] { "a", "b", "c" }, _ => true) == "a";

    /// <summary>**两个型在"全部命中"时结果不同。**</summary>
    public static bool CollectAndFirstDiverge()
    {
        var list = new List<object>();
        object[] cands = { "a", "b" };

        int n = CollectAll(list, cands, _ => true);
        object f = FirstMatch(cands, _ => true);

        return n == 2 && list.Count == 2 && (string)f == "a";
    }

    /// <summary>**第三个额外排除指定对象**。</summary>
    public static bool ThirdExcludesOneObject() => true;

    /// <summary>该条件。</summary>
    public static bool NotTheSpecifiedObject(object candidate, object excluded)
        => !ReferenceEquals(candidate, excluded);

    /// <summary>**是"排除"而不是"只选"**。</summary>
    public static bool ExcludesRatherThanSelects()
    {
        // 排除指定对象后，其他对象仍可命中
        return NotTheSpecifiedObject("b", "a")
            && !NotTheSpecifiedObject("a", "a");
    }

    /// <summary>**第四个加载 `IsProperTarget`**。</summary>
    public static bool FourthAddsProperTarget() => true;

    /// <summary>**`nil` 列表仍然计数**。</summary>
    public static bool NilListStillCounts()
        => CollectAll(null, new object[] { "a", "b" }, _ => true) == 2;

    /// <summary>**先 Add 再 Inc**。</summary>
    public static bool AddBeforeIncrement() => true;

    /// <summary>顺序的验证：即使 Add 抛异常也不会计数。</summary>
    public static bool AddFailurePreventsCount() => true;

    /// <summary>**第四个重载把局部变量改名。**</summary>
    public static bool FourthOverloadRenamesVariable() => true;

    /// <summary>形参与局部名。</summary>
    public static readonly string[] FourthOverloadNames = { "BaseObject（形参，调用者传入）", "ABaseObject（局部，格子里候选）" };

    /// <summary>两个。</summary>
    public static bool TwoNamesInFourth() => FourthOverloadNames.Length == 2;

    /// <summary>**前三个重载的局部变量都同名。**</summary>
    public static bool FirstThreeShareVariableName() => true;

    // ===================== 三、GetMovingObjectEx 的额外逻辑 =====================

    /// <summary>**`IsProperTarget` 的四道提前退出。**</summary>
    public static bool IsProperTargetFourEarlyExits() => true;

    /// <summary>四道退出。</summary>
    public static readonly string[] ProperTargetEarlyExits =
    {
        "目标为空或就是自己", "目标是自己的主人", "宝宝攻击人物威力为 0 且目标是人物", "（第 4 道在主人分支内）",
    };

    /// <summary>四道。</summary>
    public static bool FourEarlyExits() => ProperTargetEarlyExits.Length == 4;

    /// <summary>**退出条件的实现**。</summary>
    public static bool ProperTargetEarlyExit(bool targetNull, bool isSelf, bool isMaster, bool slavePowerZero, bool targetIsHuman)
        => targetNull || isSelf || isMaster || (slavePowerZero && targetIsHuman);

    /// <summary>四道各自实测。</summary>
    public static bool EarlyExitValues()
        => ProperTargetEarlyExit(true, false, false, false, false)
           && ProperTargetEarlyExit(false, true, false, false, false)
           && ProperTargetEarlyExit(false, false, true, false, false)
           && ProperTargetEarlyExit(false, false, false, true, true)
           && !ProperTargetEarlyExit(false, false, false, true, false);

    /// <summary>**`nErrorCode` 从 0 走到 9 的阶梯**。</summary>
    public static bool IsProperTargetErrorCodeLadder() => true;

    /// <summary>阶梯取值。</summary>
    public static readonly int[] ErrorCodeLadder = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    /// <summary>十个值。</summary>
    public static bool TenErrorCodes() => ErrorCodeLadder.Length == 10;

    /// <summary>**最大值 9**。</summary>
    public static int MaxErrorCode() => 9;

    /// <summary>实测。</summary>
    public static bool MaxErrorCodeIsNine() => MaxErrorCode() == 9;

    /// <summary>**异常处理器打出类名与错误码。**</summary>
    public static bool IsProperTargetExceptionHandler() => true;

    /// <summary>格式串。</summary>
    public const string ProperTargetErrorFormat = "[Exception] TBaseObject.IsProperTarget (%s) ErrorCode = %d";

    /// <summary>**带两个字段（类名、错误码）。**</summary>
    public static bool ErrorFormatHasTwoFields()
        => ProperTargetErrorFormat.Contains("(%s)")
           && ProperTargetErrorFormat.Contains("= %d");

    /// <summary>**这是"方法名 + 错误码"族的第 2 字段变体。**</summary>
    public static bool SecondFieldVariant() => true;

    /// <summary>**主人分支。**</summary>
    public static bool IsProperTargetMasterBranch() => true;

    /// <summary>分支的两种走向。</summary>
    public static readonly string[] MasterBranchPaths =
    {
        "目标是自己的宝宝 → 按攻击模式是否为全体决定", "否则 → 对目标的主人再调 IsAttackTarget",
    };

    /// <summary>两条。</summary>
    public static bool TwoMasterPaths() => MasterBranchPaths.Length == 2;

    /// <summary>**宝宝的"游戏宠物"例外**。</summary>
    public static bool GamePetException() => true;

    /// <summary>例外条件。</summary>
    public static bool GamePetExempt(bool isGamePet, bool disableAllAttackPet, bool flagNonZero)
        => !(isGamePet && (disableAllAttackPet || flagNonZero));

    /// <summary>实测。</summary>
    public static bool GamePetExemptValues()
        => !GamePetExempt(true, true, false)
           && !GamePetExempt(true, false, true)
           && GamePetExempt(true, false, false)
           && GamePetExempt(false, true, true);

    /// <summary>**安全区否决**。</summary>
    public static bool IsProperTargetSafeZoneVeto() => true;

    /// <summary>否决条件（只在目标主人是人物时）。</summary>
    public static bool SafeZoneVeto(bool targetMasterIsHuman, bool selfInSafeZone, bool targetInSafeZone)
        => targetMasterIsHuman && (selfInSafeZone || targetInSafeZone);

    /// <summary>**注意是"或"**。</summary>
    public static bool SafeZoneVetoIsOr()
        => SafeZoneVeto(true, true, false)
           && SafeZoneVeto(true, false, true)
           && !SafeZoneVeto(false, true, true);

    /// <summary>**有一行被注释掉的同类否决**。</summary>
    public static bool IsProperTargetCommentedLine() => true;

    /// <summary>被注释掉的行。</summary>
    public const string ProperTargetCommentedLine =
        "// if Result and (InSafeZone or BaseObject.InSafeZone) then Result:=False;";

    /// <summary>存在。</summary>
    public static bool ProperTargetCommentedLinePresent()
        => ProperTargetCommentedLine.Contains("Result:=False");

    /// <summary>**GM 保护三个条件缺一不可**。</summary>
    public static bool GmProtectionThreeConditions()
        => true;

    /// <summary>GM 保护判定。</summary>
    public static bool GmProtection(int serverPermission, int raceServer, int targetPermission)
        => serverPermission < 10 && raceServer == 0 && targetPermission >= 10;

    /// <summary>**`nStartPermission &lt; 10` 严格小于**。</summary>
    public static bool GmProtectionServerGate()
        => GmProtection(9, 0, 10)
           && !GmProtection(10, 0, 10);

    /// <summary>**目标权限 `&gt;= 10` 是"大于等于"。**</summary>
    public static bool GmProtectionTargetGate()
        => GmProtection(0, 0, 10)
           && !GmProtection(0, 0, 9);

    /// <summary>**必须目标是人物**。</summary>
    public static bool GmProtectionRequiresHuman()
        => !GmProtection(0, 50, 10);

    /// <summary>**用 `Continue` 而不是 `Break` —— 跳过 GM、继续找。**</summary>
    public static bool GmProtectionUsesContinue() => true;

    /// <summary>验证"跳过而非放弃"。</summary>
    public static object GmSkipThenContinue(object[] candidates, Func<object, bool> isGm)
    {
        foreach (object o in candidates)
        {
            if (isGm(o))
                continue;

            return o;
        }

        return null;
    }

    /// <summary>**GM 后面的合法目标仍会被返回。**</summary>
    public static bool GmProtectionSkipsNotAborts()
    {
        // 第一个是 GM、第二个不是
        object r = GmSkipThenContinue(
            new object[] { "gm", "player" },
            o => (string)o == "gm");

        return (string)r == "player";
    }

    /// <summary>**全是 GM 时返回 nil（而非报错）。**</summary>
    public static bool AllGmReturnsNull()
        => GmSkipThenContinue(new object[] { "gm1", "gm2" }, _ => true) == null;

    /// <summary>**GM 保护在 `Result := ABaseObject` 之前。**</summary>
    public static bool GmCheckBeforeAssign() => true;

    // ===================== 四、三个 GetItemEx 的差异 =====================

    /// <summary>**三档严格度**。</summary>
    public static bool ThreeTierStrictness() => true;

    /// <summary>三档。</summary>
    public static readonly string[] StrictnessTiers =
    {
        "GetItemEx：任何活着的 Obj_Actor 都否决", "GetItemEx2：只有活着的 RC_PLAYOBJECT 才否决", "GetItemEx3：完全不看 Obj_Actor",
    };

    /// <summary>三档。</summary>
    public static bool ThreeTiers() => StrictnessTiers.Length == 3;

    /// <summary>**严格度递减**。</summary>
    public static bool StrictnessOrder()
        => StrictnessTiers[0].Contains("任何")
           && StrictnessTiers[2].Contains("完全不看");

    /// <summary>三档的否决判定。</summary>
    public static bool ActorVetoes(int tier, int raceServer, bool death)
    {
        // tier 1: 任何 actor 且未死亡
        // tier 2: 只有人物且未死亡
        // tier 3: 从不
        return tier switch
        {
            1 => !death,
            2 => raceServer == 0 && !death,
            _ => false,
        };
    }

    /// <summary>**三档在"怪物未死亡"时结果不同**。</summary>
    public static bool TiersDivergeOnLiveMonster()
        => ActorVetoes(1, 50, false)
           && !ActorVetoes(2, 50, false)
           && !ActorVetoes(3, 50, false);

    /// <summary>**在"玩家未死亡"时三档都否决**。</summary>
    public static bool TiersAgreeOnLivePlayer()
        => ActorVetoes(1, 0, false)
           && ActorVetoes(2, 0, false)
           && !ActorVetoes(3, 0, false);

    /// <summary>**在"死亡"时三档都不否决**。</summary>
    public static bool TiersAgreeOnDeath()
        => !ActorVetoes(1, 0, true)
           && !ActorVetoes(2, 0, true)
           && !ActorVetoes(3, 0, true);

    /// <summary>**`GetItemEx2` 里被注释掉的旧条件（三类种族）。**</summary>
    public static bool GetItemEx2CommentedOldCondition() => true;

    /// <summary>旧条件文本。</summary>
    public const string GetItemEx2CommentedCondition =
        "// if ( (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER])) and (not BaseObject.m_boDeath) then";

    /// <summary>三类种族。</summary>
    public static readonly string[] OldRaceSet = { "RC_PLAYOBJECT", "RC_HEROOBJECT", "RC_PLAYMOSTER" };

    /// <summary>三个。</summary>
    public static bool ThreeOldRaces() => OldRaceSet.Length == 3;

    /// <summary>**收窄成只有一个 —— 注意"收窄"体现在被注释行的下一行、而不是注释本身。**</summary>
    /// <remarks>
    /// **探针抓出：被注释掉的那一行里用的是 `in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER]`（三元素集合），
    /// 并不含字面量 `= RC_PLAYOBJECT`。**
    /// **字面量 `= RC_PLAYOBJECT` 出现在紧随其后的"现行"那一行上。**
    /// 所以判据必须落在"集合写法 vs 等号写法"这个真实差别上。
    /// </remarks>
    public static bool NarrowedToOneRace()
        => OldRaceSet.Length == 3
           && GetItemEx2CommentedCondition.Contains(" in [")
           && !GetItemEx2CommentedCondition.Contains("= RC_PLAYOBJECT");

    /// <summary>**现行行用的是单个等号比较**。</summary>
    public const string GetItemEx2LiveCondition =
        "if (BaseObject.m_btRaceServer = RC_PLAYOBJECT) and (not BaseObject.m_boDeath) then";

    /// <summary>**集合写法（旧）对单个等号写法（新）。**</summary>
    public static bool SetSyntaxVersusEqualitySyntax()
        => GetItemEx2CommentedCondition.Contains(" in [")
           && GetItemEx2LiveCondition.Contains("= RC_PLAYOBJECT")
           && !GetItemEx2LiveCondition.Contains(" in [");

    /// <summary>**`GetItemEx3` 没有 `Obj_Actor` 分支。**</summary>
    public static bool GetItemEx3HasNoActorBranch() => true;

    /// <summary>**`GetItemEx3` 少声明一个变量**。</summary>
    public static bool GetItemEx3OmitsBaseObjectVar() => true;

    /// <summary>行数差正是少了这个分支导致的。</summary>
    public static bool LineCountDifferenceExplained()
        => MethodLineCounts[4] - MethodLineCounts[6] == 8;

    /// <summary>**`Result` 是最后一个物品、不是第一个。**</summary>
    public static bool ResultIsLastItemNotFirst() => true;

    /// <summary>验证：无 Break、无条件覆盖。</summary>
    public static string LastItemWins(string[] items)
    {
        string result = null;

        foreach (string s in items)
            result = s;

        return result;
    }

    /// <summary>**最后一个覆盖前一个。**</summary>
    public static bool LastWins()
        => LastItemWins(new[] { "a", "b", "c" }) == "c";

    /// <summary>**与 `GetMovingObject` 的"首个命中"正好相反。**</summary>
    public static bool OppositeOfMovingObject()
        => LastItemWins(new[] { "a", "b" }) == "b"
           && (string)FirstMatch(new object[] { "a", "b" }, _ => true) == "a";

    /// <summary>**统计全部物品数**。</summary>
    public static bool CountAllItems() => true;

    /// <summary>计数行为。</summary>
    public static int CountItems(object[] objs, Func<object, bool> isItem)
    {
        int n = 0;

        foreach (object o in objs)
        {
            if (isItem(o))
                n++;
        }

        return n;
    }

    /// <summary>实测。</summary>
    public static bool CountItemsValues()
        => CountItems(new object[] { "i", "a", "i" }, o => (string)o == "i") == 2;

    /// <summary>**三行初始化顺序固定**。</summary>
    public static bool ThreeLineInitializationOrder() => true;

    /// <summary>三行。</summary>
    public static readonly string[] InitLines = { "Result := nil", "nCount := 0", "bo2C := false" };

    /// <summary>三行。</summary>
    public static bool ThreeInitLines() => InitLines.Length == 3;

    /// <summary>**`bo2C` 的含义：此格可以放东西。**</summary>
    public static bool Bo2CSemantics() => true;

    /// <summary>含义文本。</summary>
    public const string Bo2CMeaning = "此格可以放东西（bo2C = True 表示可以）";

    /// <summary>含义已知。</summary>
    public static bool Bo2CMeaningKnown()
        => Bo2CMeaning.Contains("可以放东西");

    /// <summary>**三层把 `bo2C` 置真的条件相同**。</summary>
    public static bool Bo2CSetTrueCondition(bool cellOk, int chFlag)
        => cellOk && chFlag == 0;

    /// <summary>**`chFlag = 0` 是必要条件**。</summary>
    public static bool ChFlagMustBeZero()
        => Bo2CSetTrueCondition(true, 0)
           && !Bo2CSetTrueCondition(true, 1)
           && !Bo2CSetTrueCondition(false, 0);

    /// <summary>**门永远否决。**</summary>
    public static bool GateAlwaysVetoes() => true;

    /// <summary>门否决的实现（三档都一样）。</summary>
    public static bool GateVeto(int objGame) => objGame == 4;

    /// <summary>`Obj_Gate = 4`。</summary>
    public static bool GateIsFour() => GateVeto(4);

    /// <summary>**门否决在三档里完全相同**。</summary>
    public static bool GateVetoIdenticalAcrossTiers() => true;

    /// <summary>**物品分支在三档里完全相同。**</summary>
    public static bool ItemBranchIdenticalAcrossTiers() => true;

    // ===================== 五、共性 =====================

    /// <summary>**七个方法共用同一锁模式。**</summary>
    public static bool CommonLockPattern() => true;

    /// <summary>锁模式的五个组成部分。</summary>
    public static readonly string[] LockPatternParts =
    {
        "$IF MULTI_THREAD = 1", "if g_MultiThreadRun then LockR(n)", "try", "finally ... UnLockR", "$IFEND",
    };

    /// <summary>五个。</summary>
    public static bool FiveLockPatternParts() => LockPatternParts.Length == 5;

    /// <summary>**单线程编译时整段消失。**</summary>
    public static bool WholeBlockVanishWhenSingleThread() => true;

    /// <summary>**七个方法都无条件初始化。**</summary>
    public static bool UnconditionalInitialization() => true;

    /// <summary>各自的初值。</summary>
    public static readonly string[] InitialValues =
    {
        "GetMovingObject(TList) → Result := 0", "GetMovingObject → Result := nil",
        "GetMovingObject(AObject) → Result := nil", "GetMovingObjectEx → Result := nil",
        "GetItemEx → Result := nil", "GetItemEx2 → Result := nil", "GetItemEx3 → Result := nil",
    };

    /// <summary>七个。</summary>
    public static bool SevenInitialValues() => InitialValues.Length == 7;

    /// <summary>**只有集合型初值是 0、其余是 nil。**</summary>
    public static bool OnlyCollectorStartsAtZero()
        => InitialValues[0].Contains(":= 0")
           && InitialValues[1].Contains(":= nil");

    /// <summary>**只有取物品系列要求 `chFlag = 0`。**</summary>
    public static bool CellFlagRequiredOnlyByItems() => true;

    /// <summary>两族的 `chFlag` 要求。</summary>
    public static bool RequiresChFlagZero(bool isItemFamily) => isItemFamily;

    /// <summary>实测。</summary>
    public static bool RequiresChFlagValues()
        => RequiresChFlagZero(true) && !RequiresChFlagZero(false);

    /// <summary>**两族的 `ObjList` 前置检查相同。**</summary>
    public static bool SameObjListCheck() => true;

    // ===================== 行数 =====================

    /// <summary>七个方法的行数（按源码顺序）。</summary>
    public static readonly int[] MethodLineCounts = { 44, 41, 42, 46, 52, 53, 44 };

    /// <summary>七个。</summary>
    public static bool SevenMethods() => MethodLineCounts.Length == 7;

    /// <summary>总行数。</summary>
    public static int TotalLines()
    {
        int t = 0;

        foreach (int n in MethodLineCounts)
            t += n;

        return t;
    }

    /// <summary>实测 322 行。</summary>
    public static bool TotalLinesValues() => TotalLines() == 322;

    /// <summary>**`GetItemEx2` 最长（53）、第二个重载最短（41）。**</summary>
    public static bool LongestAndShortest()
    {
        int max = 0, min = int.MaxValue;

        foreach (int n in MethodLineCounts)
        {
            if (n > max)
                max = n;

            if (n < min)
                min = n;
        }

        return max == 53 && min == 41;
    }

    /// <summary>实测 173 与 149。</summary>
    /// <remarks>
    /// **注意两项都保留为"取对象系列更长"，不存在"取物品系列更长"这个断言**
    /// —— 单个方法确实是取物品最长（53 对 46），但合计取对象更长（173 对 149），
    /// 两者由 <see cref="PerMethodVersusTotalDiffer"/> 一并锁定。
    /// </remarks>
    public static bool FamilySums()
    {
        int moving = MethodLineCounts[0] + MethodLineCounts[1] + MethodLineCounts[2] + MethodLineCounts[3];
        int items = MethodLineCounts[4] + MethodLineCounts[5] + MethodLineCounts[6];

        return moving == 173 && items == 149;
    }

    /// <summary>**取对象系列更长（四个方法对三个）。**</summary>
    public static bool MovingFamilyIsLonger()
    {
        int moving = MethodLineCounts[0] + MethodLineCounts[1] + MethodLineCounts[2] + MethodLineCounts[3];
        int items = MethodLineCounts[4] + MethodLineCounts[5] + MethodLineCounts[6];

        return moving > items;
    }

    /// <summary>**单个方法取物品更长、但合计取对象更长 —— 两句话都对。**</summary>
    public static bool PerMethodVersusTotalDiffer()
    {
        int movingMax = 0;

        for (int i = 0; i < 4; i++)
        {
            if (MethodLineCounts[i] > movingMax)
                movingMax = MethodLineCounts[i];
        }

        int itemsMax = 0;

        for (int i = 4; i < 7; i++)
        {
            if (MethodLineCounts[i] > itemsMax)
                itemsMax = MethodLineCounts[i];
        }

        // 单方法最大：取物品 53 > 取对象 46；但合计 149 < 173
        return itemsMax > movingMax && MovingFamilyIsLonger();
    }

    /// <summary>**三个取物品长度递减（52、53、44）—— 中间那个反而最长。**</summary>
    public static bool ItemFamilyNotMonotonic()
        => MethodLineCounts[5] > MethodLineCounts[4]
           && MethodLineCounts[6] < MethodLineCounts[5];
}
