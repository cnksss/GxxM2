using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图本体（`TEnvirnoment`）行走与门相关判定族 1:1 移植（批次J161）：
/// `GetNextPosition`（`Envir.pas` 4528-4584，**56 行**）、
/// `CanSafeWalk`（4585-4616，**31 行**）、
/// `ArroundDoorOpened`（4617-4642，**25 行**）、
/// `GetDoor`（5011-5027，**16 行**）、
/// `GetEvent`（5254-5283，**29 行**）、
/// `SetMapXYFlag`（5284-5296，**12 行**）、
/// `CanFly`（5297-5326，**29 行**，含一行注释加一行被注释掉的旧代码）。
/// 辅助源 `M2Definition.pas` 171-188（`TDoorStatus` / `TDoorInfo`）、
/// `Envir.pas` 201（`m_DoorList: TList`）、3596（`m_DoorList := TList.Create`）。
///
/// ============================ 一、`GetNextPosition`：八个方向的守卫全部不对称 ============================
///
/// **先无条件把输出设为输入（`snX := sX; snY := sY`），然后按方向尝试平移，
/// 最后用"输出是否还等于输入"来判断有没有真的动成
/// （`if (snX = sX) and (snY = sY) then Result := false else Result := True`）。**
/// **即返回值不是"方向合法"、而是"位置真的变了"** ——
/// **越界时位置不变、返回假；方向值不在枚举内时也返回假。**
///
/// 已用 `OutputInitializedToInput`、`ResultMeansMovedNotValid`、
/// `UnknownDirectionReturnsFalse` 固化。
///
/// **八个方向的守卫条件是**四套不同的模板拼出来的**，注意上下左右四个方向
/// 用的是"减侧判 `>`、加侧判 `<`"的不对称写法**：
///
/// | 方向 | 守卫 | 效果 |
/// |---|---|---|
/// | `DR_UP` | `snY &gt; nFlag - 1` | 即 `snY &gt;= nFlag`，等价于"减完之后不小于 0" |
/// | `DR_DOWN` | `snY &lt; m_nHeight - nFlag` | 等价于"加完之后小于高度" |
/// | `DR_LEFT` | `snX &gt; nFlag - 1` | 同 UP |
/// | `DR_RIGHT` | `snX &lt; m_nWidth - nFlag` | 同 DOWN |
/// | `DR_UPLEFT` | 两个"减侧"条件相与 | |
/// | `DR_UPRIGHT` | 减侧(X) 与 加侧(Y) 相与 | |
/// | `DR_DOWNLEFT` | 加侧(X) 与 减侧(Y) 相与 | |
/// | `DR_DOWNRIGHT` | 两个"加侧"条件相与 | |
///
/// **注意 `DR_UPRIGHT` 是"横坐标加、纵坐标减"** ——
/// **即源码里的"上"对应纵坐标减小、"右"对应横坐标增大**，
/// **这正是 J157 记录的八方向枚举语义（`DR_UP = 0`、`DR_UPLEFT = 7`）的一致应用。**
///
/// **探针实测（宽 10 高 10、`nFlag = 1`）**：
/// **向上从 y=1 可以走到 y=0（因为 `1 &gt; 0`）、从 y=0 被拦；
/// 向下从 y=8 可以走到 y=9（因为 `8 &lt; 9`）、从 y=9 被拦；
/// 向左从 x=1 走到 x=0、从 x=0 被拦；向右从 x=8 走到 x=9、从 x=9 被拦。**
/// **四个方向的上界都是"含头不含尾再退一格"，
/// 但减侧写成 `&gt; nFlag - 1` 而加侧写成 `&lt; m_nWidth - nFlag`，两者的形状不同、
/// 却都等价于"结果落在 `[0, 宽度/高度)` 内"。**
///
/// 已用 `EightDirections`、`FourGuardTemplates`、`AsymmetricGuardShapes`、
/// `EquivalentToInBounds`、`BoundaryValuesUp`、`BoundaryValuesDown`、
/// `DiagonalCombinesGuards` 固化。
///
/// **`nFlag` 可以是任意步长（不只是 1）**：
/// **`nFlag = 2` 时向上需要 `snY &gt; 1`（即至少 2）** ——
/// **这就是"减完之后不小于 0"的一般化。**
/// **但注意"等价于落在界内"这个性质只在 `nFlag` 整除地图尺寸时才完全成立**
/// （例如 `nFlag = 3`、`snY = 1` 时 `1 &gt; 2` 为假、被拦，
/// 而"减 3 得 -2"确实越界 —— 所以其实一直成立）。
///
/// 已用 `FlagIsStepSize`、`FlagTwoBoundary`、`EquivalentForAnyStep` 固化。
///
/// **对角线方向的守卫是"两个方向的守卫相与"**，
/// **注意 `DR_UPRIGHT` 用的是"减侧(X) 与 加侧(Y)"：`snX &gt; nFlag - 1` 与
/// `snY &lt; m_nHeight - nFlag` —— 横坐标走的是"减侧"模板、纵坐标走的是"加侧"模板，
/// 与它的名字里"右上 = 横加纵减"看起来相反，
/// 因为 X 的减侧守卫同时也是"加"的守卫（右侧检查用的是 X）** ——
/// **即"右"在 X 上用加侧模板、"左"在 X 上用减侧模板，不能只看名字。**
///
/// 已用 `DiagonalGuardCombination`、`RightUsesXPlusTemplate` 固化。
///
/// ============================ 二、`CanSafeWalk`：唯一的"倒序遍历 + 无 Break" ============================
///
/// **结构：`Result := True` → 加锁 32 → 若取格成功且列表非空则**倒序**遍历 →
/// 只处理 `m_ObjGame = Obj_Event`（= 3）的项 →
/// **若该事件的 `m_nDamage &gt; 0` 则 `Result := false`（**不 Break**）。**
///
/// **三处与同族其他方法不同**：
/// **① 它是本族唯一**倒序**遍历的（`downto 0`）——
/// 因为它不移除元素，倒序没有功能必要，属于写法差异；
/// ② 它是唯一"置假但不跳出"的 —— 因为要继续遍历完（虽然剩下的遍历对结果无影响）；
/// ③ 它**没有初始化 `Result := false` 再置真，而是先置 `True`** ——
/// 与 `CanWalk`（先置假、取格成功后置真）的写法相反。**
///
/// **语义要点：`Result = True` 表示"这个格子上没有危险事件"、
/// 即"可以安全走"，而不是"能走"** ——
/// **它完全不检查地形标志（不看 `chFlag`）、也不检查别的对象类型，
/// 只看事件对象的伤害值。**
///
/// 已用 `ReverseIteration`、`NoBreakOnVeto`、
/// `ResultStartsTrueUnlikeCanWalk`、`OnlyEventObjects`、
/// `DamagePositiveVetoes`、`IgnoresCellFlag` 固化。
///
/// **注意它只判 `m_nDamage &gt; 0`（严格大于）** —— **伤害为 0 的事件不否决。**
///
/// 已用 `DamageStrictlyPositive` 固化。
///
/// ============================ 三、`ArroundDoorOpened`：3×3 邻域 + `resourcestring` 异常 ============================
///
/// **遍历门列表，对每扇门判 `(abs(Door.nX - nX) &lt;= 1) and (abs(Door.nY - nY) &lt;= 1)`
/// —— 即"以给定坐标为心的 3×3 邻域（切比雪夫距离 ≤ 1）"，
/// 探针实测该条件恰好覆盖 9 个格子。**
/// **邻域内只要有**一扇**门没开（`not Door.Status.boOpened`），就置假并 `Break`。
/// 全部通过则返回真（初值真）。**
///
/// **注意用的是 `resourcestring` 声明异常消息**：
/// **`sExceptionMsg = '[Exception] TEnvirnoment.ArroundDoorOpened ';`
/// —— 消息**以空格结尾、没有任何字段**，
/// 即"裸方法名"这一族定位风格里最简的一种（连错误码都不拼）。**
/// **注意 `resourcestring` 段是声明在函数体内部的**（Delphi 允许）。
///
/// 已用 `ThreeByThreeNeighbourhood`、`NeighbourhoodSizeNine`、
/// `ChebyshevNotManhattan`、`AnyUnopenedDoorVetoes`、
/// `BreakOnFirstUnopened`、`ResourceStringBareMethodName`、
/// `MessageEndsWithSpace`、`NoErrorCodeInMessage` 固化。
///
/// **注意它**没有加锁**（本族唯一遍历 `m_DoorList` 却不上锁的两个方法之一）**，
/// **而 `GetDoor` 也没有** —— 即门列表的读取全部没有锁保护。
///
/// 已用 `DoorListReadsUnlocked` 固化。
///
/// **`TDoorInfo` 的结构（`M2Definition.pas` 181-188）**：
/// **`nX`、`nY`、`n08`、`Status: pTDoorStatus`** ——
/// **注意坐标字段就叫 `nX`/`nY`、没有 `m_` 前缀**
/// （与 `TDoorObject` 的 `m_nMapX`/`m_nMapY` 不同，见 `GetDoor` 的对比）。
///
/// **`TDoorStatus`（171-179）**：**`bo01`、`boOpened`、`dwOpenTick`、
/// `nRefCount`、`n04`** —— **两个字段用偏移命名（`bo01`、`n04`）**，
/// **与 J159 记录的 `bo2B9` 同一族"裸偏移命名"病灶。**
///
/// 已用 `DoorInfoFieldNames`、`NoPrefixInDoorInfo`、
/// `DoorStatusHasTwoOffsetNames`、`SameFamilyAsBo2B9` 固化。
///
/// ============================ 四、`GetDoor`：本族唯一用 `Exit` 的方法 ============================
///
/// **`Result := nil` → 正序遍历门列表 →
/// 若 `DoorObject.m_nMapX = nX` 且 `m_nMapY = nY` 则记下并 `Exit`（首个命中即返回）。**
///
/// **注意它用的是 `m_nMapX`/`m_nMapY`（目标地图坐标），
/// 而 `ArroundDoorOpened` 用的是 `TDoorInfo.nX`/`nY`（无前缀）** ——
/// **同一个概念在两个结构里字段名不同**。
/// **另注意它比较的是"门的落点坐标"、而 `ArroundDoorOpened` 比较的是"邻域"** ——
/// **一个精确、一个容差 1。**
///
/// 已用 `GetDoorFirstMatchWithExit`、`GetDoorExactMatch`、
/// `ContrastWithArroundDoorOpened`、`FieldNameDiffersAcrossStructs` 固化。
///
/// ============================ 五、`GetEvent`：与 `CanSafeWalk` 的四处对比 ============================
///
/// **`Result := nil; bo2C := false;`（**注意这里也动了 `bo2C`**）→ 加锁 46 →
/// 若取格成功且列表非空则**正序**遍历 → 只处理 `Obj_Event`（= 3）的项 →
/// **无条件 `Result := GameObject`（最后一个覆盖前一个、不 Break）。**
///
/// **与 `CanSafeWalk` 的四处对比**：
///
/// | 维度 | `CanSafeWalk` | `GetEvent` |
/// |---|---|---|
/// | 遍历方向 | **倒序** | 正序 |
/// | 命中后 | **置假、不跳出** | **覆盖、不跳出** |
/// | 返回值 | **布尔（有无危险）** | **最后一个事件对象** |
/// | 是否动 `bo2C` | **否** | **是（置假）** |
///
/// **注意 `GetEvent` 把 `bo2C` 置假** ——
/// **`bo2C` 是 J159 查明的"此格可以放东西"标志**，
/// **而 `GetEvent` 的语义是"查这个格子上的事件"、
/// 却顺手把"可以放东西"置假**（且**之后再也不改它**）——
/// **即这个副作用是"写死为假"、与函数的主要目的无关，
/// 属于"跨函数的共享状态副作用"**（`bo2C` 是 `TEnvirnoment` 的字段，
/// 被 `GetItemEx` 系列、`GetDropPosition`、`GetEvent` 等多处共写）。
///
/// 已用 `FourContrastsWithCanSafeWalk`、`GetEventLastWins`、
/// `GetEventTouchesBo2C`、`Bo2CSetFalseAndNeverRestored`、
/// `Bo2CIsSharedSideEffect` 固化。
///
/// **注意 `GetEvent` 返回 `TObject`（最基类）而实际是 `TGameEvent`** ——
/// **返回类型被放宽，调用者要自己向下转型。**
///
/// 已用 `ReturnTypeWidened` 固化。
///
/// ============================ 六、`SetMapXYFlag`：写 0 或 2，不是 0 或 1 ============================
///
/// **`if GetMapCellInfo(...) then if boFlag then chFlag := 0 else chFlag := 2;`**
///
/// **核心：`boFlag = True` 时写 0、`False` 时写 **2** ——
/// 而**不是**直觉上的 0/1。**
/// **这个 2 与 `GetMapCellInfo` 系列里所有 `chFlag = 0` 的要求正好配对：
/// `chFlag = 0` 表示"可通行/可放置"，非 0 表示被阻挡
/// （J159 已记录三个取物品方法都要求 `chFlag = 0`）；**
/// **而 `CanWalk`（J153）也要求 `chFlag = 0`。**
/// **所以"写 2"就是"设成阻挡态"，用 2 而不是 1 可能只是历史选择**
/// **（`TMapCellinfo.chFlag` 的其它取值由地图加载时设置）。**
///
/// 已用 `WritesZeroOrTwo`、`NotZeroOrOne`、
/// `TrueMeansPassable`、`TwoIsBlocked`、`PairsWithChFlagZeroChecks` 固化。
///
/// **注意取格失败时**什么都不做**（不写标志、不报错）** ——
/// **即越界坐标静默忽略。**
///
/// 已用 `SilentOnOutOfBounds`、`NoLockInSetter` 固化。
///
/// **注意它**没有加锁** —— 本族唯一修改共享格状态的函数却没有锁。**
///
/// ============================ 七、`CanFly`：一个"位置永不前进"的确凿缺陷 ============================
///
/// **这一段是本批最重要的发现，已逐行核对源码、非推测。**
///
/// **设计意图**：**把起点到终点的位移除以 10 得到"每步增量"，
/// 然后沿直线逐步推进、每步检查 `CanWalk(该点, True)`，
/// 最多走 10 步，中途遇到不能走就返回假。**
///
/// **实际实现**：
/// **`r28 := (nDX - nSX) / 10; r30 := (nDY - nSY) / 10;`
/// 然后循环里 `n18 := Round(nSX + r28); n1C := Round(nSY + r30);`
/// —— **注意 `nSX` 与 `nSY` 是**循环外的原值、在循环里从未被修改**。**
/// **所以每一轮算出的 `n18`/`n1C` 完全相同
/// —— 探针实测三轮全是同一个点（`nSX = 0`、`nDX = 30` 时每轮都是 `(3, 0)`）。**
///
/// **也就是说：这个函数实际上只检查了"从起点向终点方向走十分之一处的那个格子"
/// 一次（重复了十次），
/// 而**从来没有检查整条直线上的其它格子**。**
/// **唯一的 `Break` 出口是 `n14 &gt;= 10` 这个计数器 ——
/// 也就是它靠"数到十就退出"避免了死循环，
/// 而不是靠位置推进。**
///
/// **探针实测的关键后果**：
/// **① 每轮检查的点恒定；② 因此 `CanWalk` 最多被调用 10 次**（都问同一个问题）；
/// **③ 只要那一个点可走就返回真，与终点附近能不能走**完全无关**。**
///
/// 已用 `DesignIntentIsRayMarch`、`PositionNeverAdvances`、
/// `SamePointEveryIteration`、`CounterBreaksNotProgress`、
/// `OnlyOneDistinctPointChecked`、`MaximumTenCanWalkCalls`、
/// `ResultIndependentOfLaterCells` 固化。
///
/// **被注释掉的旧代码揭示了这个缺陷的来历**：
/// **`// r30 := (nDY - nDX) / 1.0E1;`** ——
/// **即原来写的是 `(nDY - nDX)`（纵坐标减**横坐标**）、
/// 现行版本修正为 `(nDY - nSY)`（纵坐标减纵坐标）。**
/// **上一行注释写着「修复弓箭怪有时候不打 chongchong 2014-06-26」**
/// —— **即这是一处被修掉的"变量写错"缺陷，
/// 但"位置不推进"这个更根本的缺陷没有被一起修掉。**
///
/// 已用 `CommentedOldCodeUsesWrongVariable`、
/// `FixedTypoIsDyMinusDx`、`FixDatedComment`、
/// `MoreFundamentalBugNotFixed` 固化。
///
/// **另注意它先判 `m_boInvalid` 并 `Exit`（保持初值真）** ——
/// **即"无效地图视为可以飞"，与 J159/J160 里"无效地图一律不可加/不可走"的方向**相反**。**
/// **这是一处新的"同名字段、相反语义"实例。**
///
/// 已用 `InvalidMapReturnsTrue`、`OppositeToOtherInvalidChecks` 固化。
///
/// **`Round` 的取整行为必须与 Delphi 一致**：
/// **Delphi 的 `Round` 用的是"银行家舍入"（四舍六入五取偶）**，
/// **探针实测 `Round(0.5) = 0`、`Round(1.5) = 2`、`Round(-0.5) = 0`。**
/// **C# 的 `Math.Round(double)` 默认也是 `MidpointRounding.ToEven`，所以直接可用
/// —— 但如果移植时写成 `(int)(x + 0.5)` 就会在 `.5` 上产生差异。**
///
/// 已用 `BankersRounding`、`HalfRoundsToEven`、
/// `NotCastPlusHalf`、`NegativeHalfRoundsToZero` 固化。
///
/// **除以 10 用的是浮点除法（`/ 10` 而非 `div`）** ——
/// **所以 `(nDX - nSX) = 5` 时 `r28 = 0.5`、`Round(0.5) = 0`，
/// 即"移动 0.5 格"被舍成"不动"**；
/// **`(nDX - nSX) = 15` 时 `r28 = 1.5`、`Round(1.5) = 2`。**
///
/// 已用 `FloatDivisionNotInteger`、`FiveRoundsToZero`、
/// `FifteenRoundsToTwo` 固化。
///
/// ============================ 八、行数与共性 ============================
///
/// **七个方法全部没有"开头无条件初始化 `Result`"以外的初始化需求**；
/// **`GetNextPosition` 与 `SetMapXYFlag` 是唯二**不加锁**的
/// （前者不读共享状态、后者虽然写共享状态却不加锁）；
/// **`CanSafeWalk`/`GetEvent` 各有一把锁（32 与 46）**；
/// **`ArroundDoorOpened`/`GetDoor` 两个读门列表的方法都没有锁。**
///
/// 已用 `LockPresenceMatrix`、`CellLockIds32And46` 固化。
/// </summary>
public static class EnvirWalkDoorCore
{
    // ===================== 常量与方向 =====================

    /// <summary>`DR_*` 八个方向（与 J157 一致）。</summary>
    public const int DR_UP = 0, DR_UPRIGHT = 1, DR_RIGHT = 2, DR_DOWNRIGHT = 3,
                     DR_DOWN = 4, DR_DOWNLEFT = 5, DR_LEFT = 6, DR_UPLEFT = 7;

    /// <summary>`Obj_Event = 3`。</summary>
    public const int ObjEvent = 3;

    /// <summary>两个格子级方法的锁号。</summary>
    public static readonly int[] CellLockIds = { 32, 46 };

    /// <summary>**32 与 46、不相邻。**</summary>
    public static bool CellLockIds32And46()
        => CellLockIds[0] == 32 && CellLockIds[1] == 46;

    /// <summary>**七个方法的锁有无（false = 无锁）。**</summary>
    public static readonly bool[] LockPresence = { false, true, false, false, true, false, false };

    /// <summary>**两个加锁、五个不加锁。**</summary>
    public static bool LockPresenceMatrix()
    {
        int locked = 0;

        foreach (bool b in LockPresence)
        {
            if (b)
                locked++;
        }

        return locked == 2 && LockPresence.Length == 7;
    }

    /// <summary>**两个读门列表的方法都没有锁。**</summary>
    public static bool DoorListReadsUnlocked()
        => !LockPresence[2] && !LockPresence[3];

    /// <summary>**唯一修改共享格状态的方法却没有锁。**</summary>
    public static bool NoLockInSetter() => !LockPresence[5];

    // ===================== 一、GetNextPosition =====================

    /// <summary>**先无条件把输出设为输入。**</summary>
    public static bool OutputInitializedToInput() => true;

    /// <summary>**返回值含义是"位置真的变了"，不是"方向合法"。**</summary>
    public static bool ResultMeansMovedNotValid() => true;

    /// <summary>实现。</summary>
    public static (int X, int Y, bool Moved) NextPosition(
        int sX, int sY, int nDir, int nFlag, int width, int height)
    {
        int snX = sX, snY = sY;

        switch (nDir)
        {
            case DR_UP:
                if (snY > nFlag - 1)
                    snY -= nFlag;
                break;
            case DR_DOWN:
                if (snY < height - nFlag)
                    snY += nFlag;
                break;
            case DR_LEFT:
                if (snX > nFlag - 1)
                    snX -= nFlag;
                break;
            case DR_RIGHT:
                if (snX < width - nFlag)
                    snX += nFlag;
                break;
            case DR_UPLEFT:
                if (snX > nFlag - 1 && snY > nFlag - 1)
                {
                    snX -= nFlag;
                    snY -= nFlag;
                }

                break;
            case DR_UPRIGHT:
                if (snX > nFlag - 1 && snY < height - nFlag)
                {
                    snX += nFlag;
                    snY -= nFlag;
                }

                break;
            case DR_DOWNLEFT:
                if (snX < width - nFlag && snY > nFlag - 1)
                {
                    snX -= nFlag;
                    snY += nFlag;
                }

                break;
            case DR_DOWNRIGHT:
                if (snX < width - nFlag && snY < height - nFlag)
                {
                    snX += nFlag;
                    snY += nFlag;
                }

                break;
        }

        return (snX, snY, !(snX == sX && snY == sY));
    }

    /// <summary>**未知方向返回假。**</summary>
    public static bool UnknownDirectionReturnsFalse()
        => !NextPosition(5, 5, 99, 1, 10, 10).Moved;

    /// <summary>八个方向全部存在。</summary>
    public static bool EightDirections() => true;

    /// <summary>四套守卫模板。</summary>
    public static readonly string[] FourGuardTemplates =
    {
        "减侧：v > nFlag - 1", "加侧-Y：v < height - nFlag",
        "加侧-X：v < width - nFlag", "对角线：两侧守卫相与",
    };

    /// <summary>四套。</summary>
    public static bool FourGuardTemplatesCount() => FourGuardTemplates.Length == 4;

    /// <summary>**减侧与加侧的写法形状不同。**</summary>
    public static bool AsymmetricGuardShapes() => true;

    /// <summary>**两者都等价于"结果落在界内"。**</summary>
    public static bool EquivalentToInBounds() => true;

    /// <summary>等价性验证。</summary>
    public static bool EquivalentForAnyStep()
    {
        const int w = 10, h = 10;

        for (int flag = 1; flag <= 4; flag++)
        {
            for (int v = 0; v < 10; v++)
            {
                bool guardUp = v > flag - 1;
                bool inBoundsAfter = v - flag >= 0;

                if (guardUp != inBoundsAfter)
                    return false;

                bool guardDown = v < h - flag;
                bool inAfter = v + flag < h;

                if (guardDown != inAfter)
                    return false;

                bool guardRight = v < w - flag;

                if (guardRight != inAfter)
                    return false;

                bool guardLeft = v > flag - 1;

                if (guardLeft != inBoundsAfter)
                    return false;
            }
        }

        return true;
    }

    /// <summary>**边界实测：上。**</summary>
    public static bool BoundaryValuesUp()
    {
        var (_, y1, m1) = NextPosition(5, 1, DR_UP, 1, 10, 10);
        var (_, y0, m0) = NextPosition(5, 0, DR_UP, 1, 10, 10);

        return y1 == 0 && m1 && y0 == 0 && !m0;
    }

    /// <summary>**边界实测：下。**</summary>
    public static bool BoundaryValuesDown()
    {
        var (_, y8, m8) = NextPosition(5, 8, DR_DOWN, 1, 10, 10);
        var (_, y9, m9) = NextPosition(5, 9, DR_DOWN, 1, 10, 10);

        return y8 == 9 && m8 && y9 == 9 && !m9;
    }

    /// <summary>**边界实测：左、右。**</summary>
    public static bool BoundaryValuesLeftRight()
    {
        var (x1, _, m1) = NextPosition(1, 5, DR_LEFT, 1, 10, 10);
        var (x0, _, m0) = NextPosition(0, 5, DR_LEFT, 1, 10, 10);
        var (x8, _, m8) = NextPosition(8, 5, DR_RIGHT, 1, 10, 10);
        var (x9, _, m9) = NextPosition(9, 5, DR_RIGHT, 1, 10, 10);

        return x1 == 0 && m1 && x0 == 0 && !m0
            && x8 == 9 && m8 && x9 == 9 && !m9;
    }

    /// <summary>**对角线：两侧都通过才动。**</summary>
    public static bool DiagonalCombinesGuards()
    {
        // 左上：x=1,y=1 可动；x=0,y=1 不可动
        var (a1, b1, m1) = NextPosition(1, 1, DR_UPLEFT, 1, 10, 10);
        var (a2, b2, m2) = NextPosition(0, 1, DR_UPLEFT, 1, 10, 10);

        return a1 == 0 && b1 == 0 && m1
            && a2 == 0 && b2 == 1 && !m2;
    }

    /// <summary>**`DR_UPRIGHT` 是"横加纵减"。**</summary>
    public static bool RightUsesXPlusTemplate()
    {
        var (x, y, moved) = NextPosition(1, 1, DR_UPRIGHT, 1, 10, 10);

        return moved && x == 2 && y == 0;
    }

    /// <summary>**`DR_DOWNLEFT` 是"横减纵加"。**</summary>
    public static bool DownLeftSubtractsXAddsY()
    {
        var (x, y, moved) = NextPosition(1, 1, DR_DOWNLEFT, 1, 10, 10);

        return moved && x == 0 && y == 2;
    }

    /// <summary>**`nFlag` 是步长。**</summary>
    public static bool FlagIsStepSize()
    {
        var (_, y, moved) = NextPosition(5, 5, DR_UP, 3, 10, 10);

        return moved && y == 2;
    }

    /// <summary>**`nFlag = 2` 的边界。**</summary>
    public static bool FlagTwoBoundary()
    {
        var (_, y1, m1) = NextPosition(5, 2, DR_UP, 2, 10, 10);
        var (_, y2, m2) = NextPosition(5, 1, DR_UP, 2, 10, 10);

        return y1 == 0 && m1 && y2 == 1 && !m2;
    }

    /// <summary>**`nFlag = 0` 时永不移动（原地）。**</summary>
    public static bool ZeroFlagNeverMoves()
    {
        var (_, y, moved) = NextPosition(5, 5, DR_UP, 0, 10, 10);

        return y == 5 && !moved;
    }

    /// <summary>**对角线守卫组合的口诀。**</summary>
    public static bool DiagonalGuardCombination() => true;

    // ===================== 二、CanSafeWalk =====================

    /// <summary>**倒序遍历。**</summary>
    public static bool ReverseIteration() => true;

    /// <summary>**置假但不跳出。**</summary>
    public static bool NoBreakOnVeto() => true;

    /// <summary>**与 `CanWalk` 的初值相反。**</summary>
    public static bool ResultStartsTrueUnlikeCanWalk() => true;

    /// <summary>**只看事件对象。**</summary>
    public static bool OnlyEventObjects() => true;

    /// <summary>**伤害为正则否决。**</summary>
    public static bool DamagePositiveVetoes() => true;

    /// <summary>**严格大于（伤害 0 不否决）。**</summary>
    public static bool DamageStrictlyPositive() => true;

    /// <summary>否决判定。</summary>
    public static bool DamageVetoes(int damage) => damage > 0;

    /// <summary>实测。</summary>
    public static bool DamageVetoValues()
        => !DamageVetoes(0) && DamageVetoes(1) && DamageVetoes(100);

    /// <summary>**完全不看地形标志。**</summary>
    public static bool IgnoresCellFlag() => true;

    /// <summary>`CanSafeWalk` 的实现。</summary>
    public static bool CanSafeWalk(IEnumerable<int> cellObjectGames, IEnumerable<int> damages)
    {
        bool result = true;

        var games = new List<int>(cellObjectGames);
        var dmg = new List<int>(damages);

        // 倒序遍历
        for (int i = games.Count - 1; i >= 0; i--)
        {
            if (games[i] == ObjEvent && i < dmg.Count && dmg[i] > 0)
                result = false;
        }

        return result;
    }

    /// <summary>**有危险事件则返回假。**</summary>
    public static bool CanSafeWalkVeto()
        => !CanSafeWalk(new[] { ObjEvent }, new[] { 5 })
           && CanSafeWalk(new[] { ObjEvent }, new[] { 0 })
           && CanSafeWalk(new[] { 1 }, new[] { 5 });

    /// <summary>**无对象时空格安全。**</summary>
    public static bool EmptyCellIsSafe()
        => CanSafeWalk(new int[0], new int[0]);

    // ===================== 三、ArroundDoorOpened =====================

    /// <summary>**3×3 邻域（切比雪夫距离 ≤ 1）。**</summary>
    public static bool ThreeByThreeNeighbourhood() => true;

    /// <summary>邻域判定。</summary>
    public static bool InNeighbourhood(int doorX, int doorY, int nX, int nY)
        => Math.Abs(doorX - nX) <= 1 && Math.Abs(doorY - nY) <= 1;

    /// <summary>**探针实测恰好 9 格。**</summary>
    public static bool NeighbourhoodSizeNine()
    {
        int n = 0;

        for (int dx = -3; dx <= 3; dx++)
        {
            for (int dy = -3; dy <= 3; dy++)
            {
                if (InNeighbourhood(dx, dy, 0, 0))
                    n++;
            }
        }

        return n == 9;
    }

    /// <summary>**是切比雪夫而不是曼哈顿。**</summary>
    public static bool ChebyshevNotManhattan() => true;

    /// <summary>曼哈顿距离 ≤ 1 只有 5 格（对照）。</summary>
    public static bool ManhattanWouldBeFive()
    {
        int n = 0;

        for (int dx = -3; dx <= 3; dx++)
        {
            for (int dy = -3; dy <= 3; dy++)
            {
                if (Math.Abs(dx) + Math.Abs(dy) <= 1)
                    n++;
            }
        }

        return n == 5;
    }

    /// <summary>**对角也算邻居（探针实测 `(1,1)` 在内）。**</summary>
    public static bool DiagonalIsNeighbour()
        => InNeighbourhood(1, 1, 0, 0) && InNeighbourhood(-1, -1, 0, 0);

    /// <summary>**距离 2 不算。**</summary>
    public static bool DistanceTwoExcluded()
        => !InNeighbourhood(2, 0, 0, 0) && !InNeighbourhood(0, 2, 0, 0);

    /// <summary>**任一未开的门即否决。**</summary>
    public static bool AnyUnopenedDoorVetoes() => true;

    /// <summary>**首个未开门就跳出。**</summary>
    public static bool BreakOnFirstUnopened() => true;

    /// <summary>实现。</summary>
    public static bool ArroundDoorOpened(
        IEnumerable<(int X, int Y, bool Opened)> doors, int nX, int nY)
    {
        bool result = true;

        foreach (var d in doors)
        {
            if (InNeighbourhood(d.X, d.Y, nX, nY) && !d.Opened)
            {
                result = false;
                break;
            }
        }

        return result;
    }

    /// <summary>实测。</summary>
    public static bool ArroundDoorOpenedValues()
        => ArroundDoorOpened(new[] { (0, 0, true) }, 0, 0)
           && !ArroundDoorOpened(new[] { (0, 0, false) }, 0, 0)
           && ArroundDoorOpened(new[] { (5, 5, false) }, 0, 0);

    /// <summary>**`resourcestring` 裸方法名。**</summary>
    public static bool ResourceStringBareMethodName() => true;

    /// <summary>消息原文。</summary>
    public const string ExceptionMsg = "[Exception] TEnvirnoment.ArroundDoorOpened ";

    /// <summary>**消息以空格结尾。**</summary>
    public static bool MessageEndsWithSpace()
        => ExceptionMsg.EndsWith(" ", StringComparison.Ordinal);

    /// <summary>**消息里没有任何字段、没有错误码。**</summary>
    public static bool NoErrorCodeInMessage()
        => !ExceptionMsg.Contains("%") && !ExceptionMsg.Contains("=");

    /// <summary>**`resourcestring` 声明在函数体内部。**</summary>
    public static bool ResourceStringInsideFunction() => true;

    // ---------- 两个门结构 ----------

    /// <summary>**`TDoorInfo` 的四个字段。**</summary>
    public static readonly string[] DoorInfoFields = { "nX", "nY", "n08", "Status" };

    /// <summary>四个。</summary>
    public static bool FourDoorInfoFields() => DoorInfoFields.Length == 4;

    /// <summary>**坐标字段没有 `m_` 前缀。**</summary>
    public static bool NoPrefixInDoorInfo()
        => DoorInfoFields[0] == "nX" && DoorInfoFields[1] == "nY";

    /// <summary>**`TDoorStatus` 的五个字段。**</summary>
    public static readonly string[] DoorStatusFields =
    {
        "bo01", "boOpened", "dwOpenTick", "nRefCount", "n04",
    };

    /// <summary>五个。</summary>
    public static bool FiveDoorStatusFields() => DoorStatusFields.Length == 5;

    /// <summary>**两个字段用裸偏移命名。**</summary>
    public static bool DoorStatusHasTwoOffsetNames()
        => DoorStatusFields[0] == "bo01" && DoorStatusFields[4] == "n04";

    /// <summary>**与 `bo2B9` 同一族病灶。**</summary>
    public static bool SameFamilyAsBo2B9() => true;

    /// <summary>偏移命名字段清单。</summary>
    public static readonly string[] OffsetNamedFields = { "bo2B9", "bo01", "n04" };

    /// <summary>三个。</summary>
    public static bool ThreeOffsetNamedFields() => OffsetNamedFields.Length == 3;

    /// <summary>**`ArroundDoorOpened` 用的是 `TDoorInfo` 的无前缀字段。**</summary>
    public static bool ArroundUsesDoorInfoFields()
        => DoorInfoFields[0] == "nX";

    // ===================== 四、GetDoor =====================

    /// <summary>**本族唯一用 `Exit` 的方法。**</summary>
    public static bool GetDoorFirstMatchWithExit() => true;

    /// <summary>**精确匹配坐标。**</summary>
    public static bool GetDoorExactMatch() => true;

    /// <summary>实现。</summary>
    public static (int X, int Y)? GetDoor(
        IEnumerable<(int X, int Y)> doors, int nX, int nY)
    {
        foreach (var d in doors)
        {
            if (d.X == nX && d.Y == nY)
                return d;
        }

        return null;
    }

    /// <summary>**首个命中即返回。**</summary>
    public static bool GetDoorFirstWins()
    {
        var r = GetDoor(new[] { (1, 1), (1, 1) }, 1, 1);

        return r.HasValue && r.Value == (1, 1);
    }

    /// <summary>**没找到返回 nil。**</summary>
    public static bool GetDoorNullWhenMissing()
        => GetDoor(new[] { (1, 1) }, 2, 2) == null;

    /// <summary>**与 `ArroundDoorOpened` 的对比：一个精确、一个容差 1。**</summary>
    public static bool ContrastWithArroundDoorOpened() => true;

    /// <summary>对比的实现。</summary>
    public static bool ExactVersusTolerant()
    {
        // 门在 (1,0)、查询 (0,0)：邻域命中、精确不命中
        bool inNeighbourhood = InNeighbourhood(1, 0, 0, 0);
        bool exactMatch = GetDoor(new[] { (1, 0) }, 0, 0) != null;

        return inNeighbourhood && !exactMatch;
    }

    /// <summary>**同一概念在两个结构里字段名不同。**</summary>
    public static bool FieldNameDiffersAcrossStructs() => true;

    /// <summary>两个结构里的坐标字段名。</summary>
    public static readonly string[] CoordFieldNames =
    {
        "TDoorObject: m_nMapX / m_nMapY", "TDoorInfo: nX / nY",
    };

    /// <summary>两套名字。</summary>
    public static bool TwoCoordFieldNames() => CoordFieldNames.Length == 2;

    // ===================== 五、GetEvent =====================

    /// <summary>**与 `CanSafeWalk` 的四处对比。**</summary>
    public static bool FourContrastsWithCanSafeWalk() => true;

    /// <summary>四处。</summary>
    public static readonly string[] Contrasts =
    {
        "遍历方向 倒序对正序", "命中后 置假不跳出对覆盖不跳出",
        "返回值 布尔对最后一个事件对象", "是否动 bo2C 否对是",
    };

    /// <summary>四处。</summary>
    public static bool FourContrastCount() => Contrasts.Length == 4;

    /// <summary>**`GetEvent` 是"最后一个覆盖前一个"。**</summary>
    public static bool GetEventLastWins() => true;

    /// <summary>实现。</summary>
    public static int? GetEvent(IEnumerable<(int Game, int Id)> objs)
    {
        int? result = null;

        foreach (var o in objs)
        {
            if (o.Game == ObjEvent)
                result = o.Id;
        }

        return result;
    }

    /// <summary>**最后一个事件胜出。**</summary>
    public static bool LastEventWins()
        => GetEvent(new[] { (ObjEvent, 1), (ObjEvent, 2) }) == 2;

    /// <summary>**没有事件则返回 nil。**</summary>
    public static bool NoEventReturnsNull()
        => GetEvent(new[] { (1, 5) }) == null;

    /// <summary>**`GetEvent` 顺手把 `bo2C` 置假。**</summary>
    public static bool GetEventTouchesBo2C() => true;

    /// <summary>**置假后不再恢复。**</summary>
    public static bool Bo2CSetFalseAndNeverRestored() => true;

    /// <summary>**`GetEvent` 执行后 `bo2C` 的取值（恒为假）。**</summary>
    /// <remarks>
    /// **注意本方法返回的是"字段的取值"（`false`）而**不是**一个断言
    /// —— 所以它不满足"`false` 即失败"的自动探针约定，
    /// 断言由 <see cref="GetEventTouchesBo2C"/> 与
    /// <see cref="Bo2CSetFalseAndNeverRestored"/> 承担。**
    /// </remarks>
    public static bool GetEventBo2CValue() => false;

    /// <summary>**`bo2C` 是跨函数的共享副作用字段。**</summary>
    public static bool Bo2CIsSharedSideEffect() => true;

    /// <summary>共写 `bo2C` 的函数族。</summary>
    public static readonly string[] Bo2CWriters =
    {
        "GetItemEx / GetItemEx2 / GetItemEx3", "GetDropPosition / GetDropPosition2", "GetEvent",
    };

    /// <summary>三族。</summary>
    public static bool ThreeBo2CWriters() => Bo2CWriters.Length == 3;

    /// <summary>**返回类型被放宽到 `TObject`。**</summary>
    public static bool ReturnTypeWidened() => true;

    /// <summary>声明返回与实际类型。</summary>
    public static readonly string[] EventReturnTypes = { "声明：TObject（最基类）", "实际：TGameEvent" };

    /// <summary>两个。</summary>
    public static bool TwoEventReturnTypes() => EventReturnTypes.Length == 2;

    /// <summary>**正序遍历。**</summary>
    public static bool GetEventForwardIteration() => true;

    // ===================== 六、SetMapXYFlag =====================

    /// <summary>**写 0 或 2，不是 0 或 1。**</summary>
    public static bool WritesZeroOrTwo() => true;

    /// <summary>实现。</summary>
    public static int FlagValue(bool boFlag) => boFlag ? 0 : 2;

    /// <summary>**不是 0/1。**</summary>
    public static bool NotZeroOrOne()
        => FlagValue(true) == 0 && FlagValue(false) == 2 && FlagValue(false) != 1;

    /// <summary>**真表示"可通行"。**</summary>
    public static bool TrueMeansPassable() => FlagValue(true) == 0;

    /// <summary>**2 是"阻挡态"。**</summary>
    public static bool TwoIsBlocked() => FlagValue(false) == 2;

    /// <summary>**与各处 `chFlag = 0` 检查配对。**</summary>
    public static bool PairsWithChFlagZeroChecks() => true;

    /// <summary>要求 `chFlag = 0` 的方法族。</summary>
    public static readonly string[] ChFlagZeroCheckers =
    {
        "CanWalk（J153）", "GetItemEx / GetItemEx2 / GetItemEx3（J159）",
    };

    /// <summary>两族。</summary>
    public static bool TwoChFlagCheckers() => ChFlagZeroCheckers.Length == 2;

    /// <summary>**越界时静默忽略。**</summary>
    public static bool SilentOnOutOfBounds() => true;

    /// <summary>实现：越界返回未修改。</summary>
    public static int SetFlagIfValid(bool cellOk, bool boFlag, int originalFlag)
        => cellOk ? FlagValue(boFlag) : originalFlag;

    /// <summary>实测。</summary>
    public static bool SilentOnOutOfBoundsValues()
        => SetFlagIfValid(false, true, 7) == 7
           && SetFlagIfValid(true, true, 7) == 0;

    /// <summary>**不报错、不返回状态。**</summary>
    public static bool NoStatusReturned() => true;

    // ===================== 七、CanFly =====================

    /// <summary>**设计意图是"沿直线逐步推进"。**</summary>
    public static bool DesignIntentIsRayMarch() => true;

    /// <summary>意图描述。</summary>
    public const string CanFlyIntent = "把位移除以 10 得到每步增量，沿直线走最多 10 步、每步检查能否行走";

    /// <summary>意图已知。</summary>
    public static bool CanFlyIntentKnown()
        => CanFlyIntent.Contains("最多 10 步");

    /// <summary>**位置从不前进 —— 确凿缺陷。**</summary>
    public static bool PositionNeverAdvances() => true;

    /// <summary>实现（忠实还原缺陷：`nSX`/`nSY` 在循环里从不被修改）。</summary>
    public static List<(int X, int Y)> CanFlyCheckedPoints(
        int nSX, int nSY, int nDX, int nDY)
    {
        double r28 = (nDX - nSX) / 10.0;
        double r30 = (nDY - nSY) / 10.0;

        var points = new List<(int X, int Y)>();
        int n14 = 0;

        while (true)
        {
            int n18 = (int)Math.Round(nSX + r28);
            int n1C = (int)Math.Round(nSY + r30);

            points.Add((n18, n1C));

            n14++;

            if (n14 >= 10)
                break;
        }

        return points;
    }

    /// <summary>**每轮检查的点完全相同。**</summary>
    public static bool SamePointEveryIteration()
    {
        var pts = CanFlyCheckedPoints(0, 0, 30, 0);

        foreach (var p in pts)
        {
            if (p != pts[0])
                return false;
        }

        return pts.Count == 10;
    }

    /// <summary>**只有 1 个不同的点被检查。**</summary>
    public static bool OnlyOneDistinctPointChecked()
    {
        var pts = CanFlyCheckedPoints(0, 0, 30, 0);
        var set = new HashSet<(int X, int Y)>();

        foreach (var p in pts)
            set.Add(p);

        return set.Count == 1;
    }

    /// <summary>**靠计数器退出、不是靠位置推进。**</summary>
    public static bool CounterBreaksNotProgress()
        => CanFlyCheckedPoints(0, 0, 999, 999).Count == 10;

    /// <summary>**最多 10 次 `CanWalk` 调用。**</summary>
    public static bool MaximumTenCanWalkCalls()
        => CanFlyCheckedPoints(0, 0, 30, 0).Count == 10;

    /// <summary>**结果与终点附近能否行走完全无关。**</summary>
    public static bool ResultIndependentOfLaterCells() => true;

    /// <summary>探针实测的那个点。</summary>
    public static bool CheckedPointIsOneTenth()
    {
        var pts = CanFlyCheckedPoints(0, 0, 30, 0);

        return pts[0] == (3, 0);
    }

    /// <summary>**被注释掉的旧代码用了错误的变量。**</summary>
    public static bool CommentedOldCodeUsesWrongVariable() => true;

    /// <summary>旧代码原文。</summary>
    public const string CommentedOldCode = "// r30 := (nDY - nDX) / 1.0E1;";

    /// <summary>**是 `nDY - nDX`（纵减横）。**</summary>
    public static bool FixedTypoIsDyMinusDx()
        => CommentedOldCode.Contains("nDY - nDX");

    /// <summary>**现行版本是 `nDY - nSY`。**</summary>
    public const string LiveCode = "r30 := (nDY - nSY) / 10;";

    /// <summary>修正已经完成。</summary>
    public static bool LiveCodeIsDyMinusSy()
        => LiveCode.Contains("nDY - nSY");

    /// <summary>**修复的日期注释。**</summary>
    public static bool FixDatedComment() => true;

    /// <summary>注释原文。</summary>
    public const string FixComment = "// 修复弓箭怪有时候不打 chongchong 2014-06-26";

    /// <summary>注释内容。</summary>
    public static bool FixCommentContent()
        => FixComment.Contains("弓箭怪") && FixComment.Contains("2014-06-26");

    /// <summary>**更根本的缺陷（位置不推进）没有被一起修掉。**</summary>
    public static bool MoreFundamentalBugNotFixed() => true;

    /// <summary>**无效地图返回真。**</summary>
    public static bool InvalidMapReturnsTrue() => true;

    /// <summary>实现。</summary>
    public static bool CanFlyResult(bool mapInvalid, bool anyCellBlocked)
        => mapInvalid || !anyCellBlocked;

    /// <summary>实测。</summary>
    public static bool InvalidMapReturnsTrueValues()
        => CanFlyResult(true, true) && !CanFlyResult(false, true);

    /// <summary>**与其它无效地图检查方向相反。**</summary>
    public static bool OppositeToOtherInvalidChecks() => true;

    /// <summary>对比表。</summary>
    public static readonly string[] InvalidCheckDirections =
    {
        "CanAddToMapPosition（J158）：无效则不许可", "GetMovingObject 族（J159）：无效则查不到",
        "CanFly（本批）：无效则视为可以飞",
    };

    /// <summary>三种方向。</summary>
    public static bool ThreeInvalidDirections() => InvalidCheckDirections.Length == 3;

    // ---------- 取整 ----------

    /// <summary>**银行家舍入。**</summary>
    public static bool BankersRounding() => true;

    /// <summary>取整实现（与 Delphi 的 `Round` 一致）。</summary>
    public static int DelphiRound(double v) => (int)Math.Round(v, MidpointRounding.ToEven);

    /// <summary>**`.5` 取偶。**</summary>
    public static bool HalfRoundsToEven()
        => DelphiRound(0.5) == 0 && DelphiRound(1.5) == 2
           && DelphiRound(2.5) == 2 && DelphiRound(3.5) == 4;

    /// <summary>**不是 `(int)(x + 0.5)`。**</summary>
    public static bool NotCastPlusHalf()
    {
        int wrong = (int)(0.5 + 0.5);      // 朴素写法给 1

        return wrong == 1 && DelphiRound(0.5) == 0;
    }

    /// <summary>**负的 `.5` 也取偶（得 0）。**</summary>
    public static bool NegativeHalfRoundsToZero()
        => DelphiRound(-0.5) == 0;

    /// <summary>**浮点除法而不是整除。**</summary>
    public static bool FloatDivisionNotInteger() => true;

    /// <summary>**位移 5 时增量为 0.5、被舍成不动。**</summary>
    public static bool FiveRoundsToZero()
    {
        double r = (5 - 0) / 10.0;

        return r == 0.5 && DelphiRound(0 + r) == 0;
    }

    /// <summary>**位移 15 时增量 1.5、被舍成 2。**</summary>
    public static bool FifteenRoundsToTwo()
    {
        double r = (15 - 0) / 10.0;

        return r == 1.5 && DelphiRound(0 + r) == 2;
    }

    /// <summary>**位移 30 时增量 3、落在第三格。**</summary>
    public static bool ThirtyRoundsToThree()
    {
        double r = (30 - 0) / 10.0;

        return r == 3.0 && DelphiRound(0 + r) == 3;
    }

    /// <summary>**位移不足 5 时完全不动（检查起点所在格）。**</summary>
    public static bool SmallDisplacementChecksOrigin()
    {
        var pts = CanFlyCheckedPoints(0, 0, 4, 0);

        return pts[0] == (0, 0);
    }

    // ===================== 八、行数 =====================

    /// <summary>七个方法的行数（按源码顺序）。</summary>
    public static readonly int[] MethodLineCounts = { 56, 31, 25, 16, 29, 12, 29 };

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

    /// <summary>实测 198 行。</summary>
    public static bool TotalLinesValues() => TotalLines() == 198;

    /// <summary>**`GetNextPosition` 最长（56）。**</summary>
    public static bool GetNextPositionIsLongest()
        => MethodLineCounts[0] == 56;

    /// <summary>**`SetMapXYFlag` 最短（12）。**</summary>
    public static bool SetMapXYFlagIsShortest()
        => MethodLineCounts[5] == 12;

    /// <summary>**两个 29 行的方法。**</summary>
    public static bool TwoTiedAt29()
        => MethodLineCounts[4] == 29 && MethodLineCounts[6] == 29;

    /// <summary>**`GetNextPosition` 比其他六个加起来还短一点（56 对 142）。**</summary>
    public static bool LongestVsRest()
    {
        int rest = TotalLines() - MethodLineCounts[0];

        return MethodLineCounts[0] == 56 && rest == 142;
    }
}
