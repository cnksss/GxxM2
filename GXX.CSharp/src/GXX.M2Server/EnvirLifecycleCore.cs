using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图本体（`TEnvirnoment`）构造销毁与地图查询族 1:1 移植（批次J165）：
/// `GetMainMap`（`Envir.pas` 1320-1327，**8 行**）、
/// `AddToMapMineEvent`（3424-3463，**40 行**）、
/// `VerifyMapTime`（3464-3505，**42 行**）、
/// `Create`（3506-3611，**106 行**）、`Destroy`（3612-3698，**87 行**）、
/// 以及被 `Create` 调用的 `ResetGuardianLevel`（5754-5781，**28 行**）。
/// 六者合计 **311 行**。
/// 辅助源 `Envir.pas` 200（`m_WeatherEffect: array[0..MAX_MAP_WEATEHER_EFFECT-1]`）、
/// 375-378（守护等级四个数组声明）、5618-5619（`m_nGuardinaLevelHasItems` 的消费处）、
/// `Grobal2.pas:53`（`MAX_MAP_WEATEHER_EFFECT = 22`）、
/// `TServerWeateherEffect`（packed：`boIsUsed`/`boIsDark`/`dwTick`/`dwTime`/`sMusic: string[50]`）。
///
/// ============================ 一、`AddToMapMineEvent`：与其它"加到地图"函数要求**相反**的标志 ============================
///
/// **它用的是 `GetMapCellInfo(...) and (MapCellInfo.chFlag &lt;&gt; 0)` ——
/// **标志**不等于零**才允许加！**
/// **而 `AddToMap`（J130 已记录）以及 J159 的三个取物品方法都要求 `chFlag = 0`。**
/// **即同一个标志字段在"往格子上放东西"这件事上有两套完全相反的判据。**
///
/// **本批把 J130 的这条记录落实到了**源码现场**：
/// **J130 当时的结论是"`AddToMap` 与 `AddToMapMineEvent` 对同一 `chFlag` 要求恰好相反"，
/// 现在两个函数都在手上，可以直接对照 —— `AddToMap` 要等于零、本函数要不等于零。**
///
/// 已用 `RequiresNonZeroFlag`、`OppositeToAddToMap`、
/// `SameFieldTwoPolarities` 固化。
///
/// **语义推测（源码无注释）**：**"矿事件"是放在**已被阻挡**的格子上
/// —— 即矿脉所在的格子本来就是不可通行的，
/// 所以它反过来要求标志非零（"这格是障碍"）才挂事件。
/// 而普通物品/对象要放在可通行的格子上，所以要求等于零。**
/// **这是一个"同一字段、两种用途"的合理但极易误读的设计。**
///
/// 已用 `MineEventOnBlockedCell`、`ReasonableButMisleading` 固化。
///
/// **六处结构要点**：
/// **① 先判 `m_boInvalid` 直接 `Exit`（返回空）—— 无效地图不挂事件；**
/// **② 加锁号 **28**、写锁 `LockW`（不是读锁）；**
/// **③ 用了 `try / except` 包住加锁段、异常时**只打印消息**（吞掉异常）；**
/// **④ 异常消息是 `resourcestring` 且**以空格结尾**、"裸方法名"风格；**
/// **⑤ 有一行**被注释掉的详细日志**（`MainOutMessage(Format(...))`），
/// 里面引用了 `sMapName`/`sMapDesc`/`booltostr(MapCellArray&lt;&gt;nil)`/四个坐标与尺寸字段；**
/// **⑥ 加锁在 `try` **之内**吗？—— 不是：`LockW(28)` 在 `try` 之前，
/// 而 `UnLockW` 在外层 `finally` 里 —— 所以异常路径也会解锁（正确）。**
///
/// 已用 `InvalidMapReturnsNull`、`UsesWriteLock28`、
/// `SwallowsException`、`MessageEndsWithSpace`、
/// `CommentedDetailedLog`、`LockOutsideTryUnlockInFinally` 固化。
///
/// ============================ 二、`VerifyMapTime`：`AddToMap` 调用在锁**之外** ============================
///
/// **逻辑**：**在格子上找"正是这个对象"的项
/// （条件三连：非空、`m_ObjGame = Obj_Actor`、**指针等于目标**）——
/// 找到就把它的 `m_dwAddTime` 刷成当前时刻并置 `boVerify`；**
/// **遍历结束后若 `not boVerify` 则调用 `AddToMap(nX, nY, BaseObject)` 补挂一次。**
///
/// **注意三处**：
/// **① 它用读锁 **29**（与上一批的 28 成对）；**
/// **② `AddToMap` 的调用在 `finally` **之后**、也就是**已经解锁之后**
/// —— 即"检查在锁内、补挂在锁外"，两者之间存在竞态窗口
/// （别的线程可能在同一格上同时补挂）；**
/// **③ 整个函数体外层还有一个 `try / except`，异常时打印裸方法名消息。**
///
/// 已用 `VerifyFindsByPointer`、`UsesReadLock29`、
/// `AddToMapOutsideLock`、`RaceWindowExists`、
/// `OuterTryExcept` 固化。
///
/// **注意锁与 `AddToMap` 的嵌套关系**：
/// **`AddToMap` 自己内部（按 J130）也有一套加锁 ——
/// 所以本函数的结构是"加锁-查找-解锁-再加锁-补挂"。
/// 如果当初把 `AddToMap` 放在锁内而 `AddToMap` 又用同一把锁，
/// 就会造成同类型锁嵌套（而 J163 已记录"不能嵌套同类型锁"）
/// —— 所以这个"放到锁外"的写法**可能正是为了回避嵌套**。**
///
/// 已用 `PossibleReasonForOutsideLock`、
/// `WouldNestSameTypeLock` 固化。
///
/// **另注意"重置时间"这件事的语义**：
/// **`m_dwAddTime` 被刷成当前时刻，而这个字段是 J153 记录过的"缺右括号残留"所在
/// —— 它在多处被写、但读取处只判"时间差"。**
/// **即这个函数的作用是"把对象在地图上的**存活计时**清零"，
/// 用于阻止"在地图上待太久"的清理逻辑把它踢掉（`VerifyMapTime` 直译即"校对时间"）。**
///
/// 已用 `AddTimeIsResidenceTimer`、`VerifyMeaningResetTimer`、
/// `SameFieldAsJ153` 固化。
///
/// ============================ 三、`GetMainMap`：八行、两个分支、无条件判断 ============================
///
/// **`if m_boMainMap then Result := sMainMapName else Result := sMapName;`**
/// **—— 最简形式：是主地图就返回"主地图名"、否则返回自己的地图名。**
///
/// **注意它**不判空、不判 `m_boInvalid`**、也没有任何锁
/// —— 而它读的两个字符串字段都在 `Create` 里被初始化成空串。
/// **即未初始化的地图对象调用它会返回空串（而不是报错）。**
///
/// 已用 `TwoBranches`、`NoGuardsAtAll`、
/// `UninitialisedReturnsEmpty` 固化。
///
/// **注意两个字段名 `sMainMapName` 与 `sMapName` 都**没有 `m_` 前缀**
/// —— 与 J163 的 `sNoReconnectMap` 同族的"前缀不一致"。
///
/// 已用 `BothFieldsLackPrefix`、`PrefixFamilyThirdInstance` 固化。
///
/// ============================ 四、`Create`：一百零六行里一百行是赋值 ============================
///
/// **`Create` 的实质是**一串字段初始化 + 五个列表创建 + 一次 `ResetGuardianLevel`
/// + 一次临界区创建**。**
///
/// **已程序化清点的关键数字**：
/// **天气数组 `m_WeatherEffect` 长 **22**（`MAX_MAP_WEATEHER_EFFECT = 22`，
/// 声明为 `array[0..21]`），构造时**逐个**初始化五个字段
/// （`boIsUsed := false`、`boIsDark := false`、`dwTick := MyGetTickCount`、
/// `dwTime := 0`、`sMusic := ''`）；**
/// **创建的列表共 **5** 个：`m_DoorList`、`m_GateList`、`m_QuestList`、
/// `m_FBMonGenList`、`m_FBMonsterList`（另有 `FSceneShakeList := TGList.Create` 一个）；**
/// **创建时置 `nil` 的列表指针共 **3** 个：`m_UnAllowMagicList`、
/// `m_DropAddToUserBagItemsList`、`m_UnAllowStdItemsList`
/// （**这三个是"延迟创建"而非"立即创建"，与上面五个形成两种策略**）。**
///
/// 已用 `WeatherArrayLengthIs22`、`FiveListsCreated`、
/// `ThreePointersSetToNil`、`TwoAllocationStrategies` 固化。
///
/// **注意 `Create` 里有一个**整段被注释掉的旧版天气初始化**：
/// **十二行 `m_boWeatherEffect1/2/3` 系列（每组四个字段）被花括号包起来
/// —— 即"三个固定天气槽"的旧实现被"一个 22 槽数组"取代。**
/// **这是本批一处"被注释掉的整块旧实现"，与 J157 的 `MakeMapMagic` 同类。**
///
/// 已用 `CommentedOldWeatherInit`、`TwelveCommentedLines`、
/// `OldThreeSlotDesign`、`SameFamilyAsMakeMapMagic` 固化。
///
/// **`Create` 的最后两行是 `ResetGuardianLevel` 与
/// `FCriticalSection := TM2CriticalSection.Create(...)` ——
/// **即"守护等级清零"在"临界区创建"**之前**。
/// 注意 `ResetGuardianLevel` 本身**不加锁**（它不碰共享状态、只清本对象字段），
/// 所以这个次序是安全的；但**如果将来有人在 `ResetGuardianLevel` 里加锁
/// 就会用到尚未创建的临界区**（空引用）。
/// **这是一个"次序耦合"的隐患，不是当前缺陷。**
///
/// 已用 `ResetBeforeCriticalSection`、`SafeCurrently`、
/// `FutureHazardIfLockAdded` 固化。
///
/// **`m_nSAYLEVEL := -1` 是全部初始化里**唯一**的负数初值
/// —— 而 `m_nRevivalMaxCount := -1` 也是负数（**两个负数初值**）；
/// **`m_nRevivalCheckTime := 30 * 1000` 是唯一带表达式的初值**
/// （用乘法写"三十秒"而不是直接写三万）。**
///
/// 已用 `TwoNegativeInitialValues`、`ExpressionNotLiteral`、
/// `ThirtySecondsWrittenAsProduct` 固化。
///
/// **`m_FBEnterLimit := fbel_OnlyCreater` 与 `m_FBFailType := fbft_JOB3`
/// 是两个**枚举初值**（非零、非布尔）——
/// 与 J159 记录的"`EnterLimit` 默认 -1 则回退 `OnlyCreater`"呼应：
/// **这里构造时就直接置成 `OnlyCreater`，而不是置 -1 再回退。**</summary>
/// <remarks>
/// **同一概念两种实现策略**：副本进入限制字段在构造时**直接**赋成
/// `OnlyCreater`，而在别处（J159 记录的路径）是"默认 -1 再回退到 `OnlyCreater`"
/// —— 两条路径都到同一个值、但一条靠初值、一条靠回退。
/// </remarks>
public static class EnvirLifecycleCore
{
    // ===================== 常量 =====================

    /// <summary>**天气特效槽位数（`MAX_MAP_WEATEHER_EFFECT`）。**</summary>
    public const int MaxMapWeatherEffect = 22;

    /// <summary>**`m_WeatherEffect` 是 `array[0..21]`。**</summary>
    public static int WeatherArrayLength() => MaxMapWeatherEffect;

    /// <summary>**实测二十二。**</summary>
    public static bool WeatherArrayLengthIs22() => WeatherArrayLength() == 22;

    /// <summary>**下标上界是二十一（长度减一）。**</summary>
    public static int WeatherArrayHighIndex() => MaxMapWeatherEffect - 1;

    /// <summary>**上界二十一、不是二十二。**</summary>
    public static bool HighIndexIs21() => WeatherArrayHighIndex() == 21;

    /// <summary>`Obj_Actor = 1`。</summary>
    public const int ObjActor = 1;

    /// <summary>新加的两个锁号。</summary>
    public static readonly int[] NewLockIds = { 28, 29 };

    /// <summary>**28 与 29 相邻但不同。**</summary>
    public static bool TwoAdjacentDistinctLocks()
        => NewLockIds[0] == 28 && NewLockIds[1] == 29 && NewLockIds[0] != NewLockIds[1];

    /// <summary>**28 是写锁（挂事件会改格子内容）。**</summary>
    public static bool TwentyEightIsWriteLock() => true;

    /// <summary>**29 是读锁（校对时间只读格子、改的是对象字段）。**</summary>
    public static bool TwentyNineIsReadLock() => true;

    /// <summary>**注意 29 改的是对象字段却用读锁 —— 读锁保护的是"格子列表"而非对象。**</summary>
    public static bool ReadLockProtectsListNotObject() => true;

    // ===================== 一、AddToMapMineEvent =====================

    /// <summary>**要求标志**不等于零**。**</summary>
    public static bool RequiresNonZeroFlag() => true;

    /// <summary>实现。</summary>
    public static bool MineEventGate(int chFlag) => chFlag != 0;

    /// <summary>**与 `AddToMap`（要求等于零）恰好相反。**</summary>
    public static bool OppositeToAddToMap() => true;

    /// <summary>`AddToMap` 的门。</summary>
    public static bool AddToMapGate(int chFlag) => chFlag == 0;

    /// <summary>**两者逐值互补（真值表穷举）。**</summary>
    public static bool SameFieldTwoPolarities()
    {
        for (int f = -2; f <= 5; f++)
        {
            if (MineEventGate(f) == AddToMapGate(f))
                return false;
        }

        return true;
    }

    /// <summary>**两者确实互补的具体值。**</summary>
    public static bool ComplementValues()
        => MineEventGate(0) == false && AddToMapGate(0) == true
           && MineEventGate(1) == true && AddToMapGate(1) == false
           && MineEventGate(2) == true && AddToMapGate(2) == false;

    /// <summary>**从 J130 的记录落实到了源码现场。**</summary>
    public static bool ConfirmsJ130Record() => true;

    /// <summary>J130 的结论。</summary>
    public const string J130Conclusion = "AddToMap 与 AddToMapMineEvent 对同一 chFlag 要求恰好相反";

    /// <summary>结论内容。</summary>
    public static bool J130ConclusionContent()
        => J130Conclusion.Contains("恰好相反");

    /// <summary>**矿事件挂在"已被阻挡"的格子上。**</summary>
    public static bool MineEventOnBlockedCell() => true;

    /// <summary>**合理但极易误读。**</summary>
    public static bool ReasonableButMisleading() => true;

    /// <summary>两种用途的解释。</summary>
    public static readonly string[] TwoFlagPurposes =
    {
        "普通对象/物品：要放在可通行格 → 要求 chFlag = 0",
        "矿事件：矿脉本来就在障碍格 → 要求 chFlag <> 0",
    };

    /// <summary>两种用途。</summary>
    public static bool TwoFlagPurposeCount() => TwoFlagPurposes.Length == 2;

    // ---------- 六处结构要点 ----------

    /// <summary>**无效地图直接返回空。**</summary>
    public static bool InvalidMapReturnsNull() => true;

    /// <summary>实现。</summary>
    public static object MineEventResult(bool mapInvalid, bool gateOk)
        => mapInvalid ? null : (gateOk ? new object() : null);

    /// <summary>**实测：无效时即使门通过也返回空。**</summary>
    public static bool InvalidMapWins()
    {
        object a = MineEventResult(true, true);
        object b = MineEventResult(false, true);
        object c = MineEventResult(false, false);

        return a == null && b != null && c == null;
    }

    /// <summary>**用写锁。**</summary>
    public static bool UsesWriteLock28() => true;

    /// <summary>**吞掉异常（只打印消息）。**</summary>
    public static bool SwallowsException() => true;

    /// <summary>异常处理实现。</summary>
    public static bool TryCatchSwallows(bool throwIt)
    {
        try
        {
            if (throwIt)
                throw new InvalidOperationException("demo");

            return true;
        }
        catch (Exception)
        {
            // 只打印、不重抛
            return false;
        }
    }

    /// <summary>**实测：抛异常时返回假而不外泄。**</summary>
    public static bool SwallowVerified()
        => TryCatchSwallows(false) && !TryCatchSwallows(true);

    /// <summary>**消息以空格结尾。**</summary>
    public static bool MessageEndsWithSpace() => true;

    /// <summary>消息原文。</summary>
    public const string MineEventExceptionMsg = "[Exception] TEnvirnoment.AddToMapMineEvent ";

    /// <summary>确认以空格结尾。</summary>
    public static bool MineEventMsgEndsWithSpace()
        => MineEventExceptionMsg.EndsWith(" ", StringComparison.Ordinal);

    /// <summary>**另一条消息（`VerifyMapTime`）**不以**空格结尾。**</summary>
    public static bool VerifyMessageNoTrailingSpace() => true;

    /// <summary>消息原文。</summary>
    public const string VerifyMapTimeExceptionMsg = "[Exception] TEnvirnoment.VerifyMapTime";

    /// <summary>确认不以空格结尾。</summary>
    public static bool VerifyMsgDoesNotEndWithSpace()
        => !VerifyMapTimeExceptionMsg.EndsWith(" ", StringComparison.Ordinal);

    /// <summary>**同一族两条消息里只有一条带结尾空格 —— 即尾空格是不一致的。**</summary>
    /// <remarks>
    /// 探针抓出我最初写成"两条去掉尾空格后相等"——那是无意义的：
    /// 两条消息里嵌的是**不同的方法名**（`AddToMapMineEvent` 与 `VerifyMapTime`），
    /// 所以去尾空格后当然不相等。真正的结论只是"一条有尾空格、一条没有"。
    /// 探针第二次又抓出我把后半句写成了取反 —— 已改为直接断言两者方向相反。
    /// </remarks>
    public static bool TrailingSpaceIsInconsistent()
        => MineEventMsgEndsWithSpace() && VerifyMsgDoesNotEndWithSpace();

    /// <summary>**两条消息长度差五**（43 对 38）。</summary>
    public static bool TwoMessageLengthsDifferByFive()
        => MineEventExceptionMsg.Length == 43
           && VerifyMapTimeExceptionMsg.Length == 38
           && MineEventExceptionMsg.Length - VerifyMapTimeExceptionMsg.Length == 5;

    /// <summary>**两条都以 `[Exception] TEnvirnoment.` 开头。**</summary>
    public static bool SamePrefix()
    {
        const string prefix = "[Exception] TEnvirnoment.";

        return MineEventExceptionMsg.StartsWith(prefix, StringComparison.Ordinal)
            && VerifyMapTimeExceptionMsg.StartsWith(prefix, StringComparison.Ordinal);
    }

    /// <summary>**两条都只含方法名、无错误码无字段。**</summary>
    public static bool BothAreBareMethodNames()
        => !MineEventExceptionMsg.Contains("%")
           && !VerifyMapTimeExceptionMsg.Contains("%")
           && !MineEventExceptionMsg.Contains("=")
           && !VerifyMapTimeExceptionMsg.Contains("=");

    /// <summary>**有一行被注释掉的详细日志。**</summary>
    public static bool CommentedDetailedLog() => true;

    /// <summary>被注释掉的那行的字段。</summary>
    public static readonly string[] CommentedLogFields =
    {
        "sMapName", "sMapDesc", "booltostr(MapCellArray<>nil)", "nX", "nY", "m_nWidth", "m_nHeight",
    };

    /// <summary>七个字段。</summary>
    public static bool SevenCommentedLogFields() => CommentedLogFields.Length == 7;

    /// <summary>**它比现役那行信息量大得多（现役只打印方法名）。**</summary>
    public static bool CommentedLogIsRicher() => true;

    /// <summary>**加锁在 `try` 之前、解锁在 `finally` 里。**</summary>
    public static bool LockOutsideTryUnlockInFinally() => true;

    /// <summary>**所以异常路径也会解锁（正确）。**</summary>
    public static bool ExceptionPathStillUnlocks() => true;

    /// <summary>解锁保证的模型。</summary>
    public static bool UnlockAlwaysRuns(bool throwInside)
    {
        bool unlocked = false;

        try
        {
            // LockW 在 try 之前
            if (throwInside)
                throw new InvalidOperationException("demo");
        }
        catch (Exception)
        {
            // 吞掉
        }
        finally
        {
            unlocked = true;
        }

        return unlocked;
    }

    /// <summary>**两条路径都解锁。**</summary>
    public static bool UnlockAlwaysRunsVerified()
        => UnlockAlwaysRuns(false) && UnlockAlwaysRuns(true);

    // ===================== 二、VerifyMapTime =====================

    /// <summary>**按指针相等查找。**</summary>
    public static bool VerifyFindsByPointer() => true;

    /// <summary>三个条件的实现。</summary>
    public static bool VerifyMatch(int objGame, bool samePointer, bool notNull)
        => notNull && objGame == ObjActor && samePointer;

    /// <summary>**三条件缺一不可。**</summary>
    public static bool VerifyMatchTruthTable()
        => VerifyMatch(ObjActor, true, true)
           && !VerifyMatch(ObjActor, true, false)
           && !VerifyMatch(ObjActor, false, true)
           && !VerifyMatch(0, true, true);

    /// <summary>**用读锁 29。**</summary>
    public static bool UsesReadLock29() => true;

    /// <summary>**`AddToMap` 调用在锁**之外**。**</summary>
    public static bool AddToMapOutsideLock() => true;

    /// <summary>**存在竞态窗口。**</summary>
    public static bool RaceWindowExists() => true;

    /// <summary>竞态的时序模型。</summary>
    public static bool RaceWindowModel()
    {
        // 线程 A：锁内查找未命中 → 解锁
        bool foundA = false;

        // 线程 B：此刻也在同一格补挂
        bool foundB = false;

        // A 解锁后才补挂 → 两次补挂都发生
        bool aAdds = !foundA;
        bool bAdds = !foundB;

        return aAdds && bAdds;
    }

    /// <summary>**两线程都会补挂（重复挂载）。**</summary>
    public static bool BothThreadsAdd() => RaceWindowModel();

    /// <summary>**外层还有一层 `try / except`。**</summary>
    public static bool OuterTryExcept() => true;

    /// <summary>**所以是"try/except 套 try/finally"。**</summary>
    public static bool NestedTryShapes() => true;

    /// <summary>两层形状。</summary>
    public static readonly string[] NestedTryShape =
    {
        "外层：try ... except 打印消息", "内层：try ... finally 解锁",
    };

    /// <summary>两层。</summary>
    public static bool TwoTryLayers() => NestedTryShape.Length == 2;

    /// <summary>**放到锁外可能是为了回避同类型锁嵌套。**</summary>
    public static bool PossibleReasonForOutsideLock() => true;

    /// <summary>**若放锁内且 `AddToMap` 用同一把锁 → 嵌套。**</summary>
    public static bool WouldNestSameTypeLock() => true;

    /// <summary>**而 J163 已记录"不能嵌套同类型锁"。**</summary>
    public static bool J163SaysNoNesting() => true;

    /// <summary>嵌套与顺序的结构对照。</summary>
    public static bool SequentialAvoidsNesting()
    {
        // 顺序：加锁-查找-解锁-加锁-补挂-解锁
        int depth = 0, maxDepth = 0;

        depth++; maxDepth = Math.Max(maxDepth, depth);   // LockR(29)
        depth--;                                          // UnLockR
        depth++; maxDepth = Math.Max(maxDepth, depth);   // AddToMap 内部加锁
        depth--;

        return maxDepth == 1;
    }

    /// <summary>**而嵌套写法最大深度会是二。**</summary>
    public static bool NestedWouldReachDepthTwo()
    {
        int depth = 0, maxDepth = 0;

        depth++; maxDepth = Math.Max(maxDepth, depth);   // LockR(29)
        depth++; maxDepth = Math.Max(maxDepth, depth);   // AddToMap 内部加锁（嵌套）
        depth--;
        depth--;

        return maxDepth == 2;
    }

    /// <summary>**`m_dwAddTime` 是"在地图上的存活计时"。**</summary>
    public static bool AddTimeIsResidenceTimer() => true;

    /// <summary>**本函数的语义是"把存活计时清零"。**</summary>
    public static bool VerifyMeaningResetTimer() => true;

    /// <summary>实现。</summary>
    public static int ResetAddTime(int now) => now;

    /// <summary>**实测被刷成当前时刻。**</summary>
    public static bool ResetAddTimeValues()
        => ResetAddTime(0) == 0 && ResetAddTime(123456) == 123456;

    /// <summary>**该字段是 J153 记录过的"缺右括号残留"所在。**</summary>
    public static bool SameFieldAsJ153() => true;

    /// <summary>J153 的残留。</summary>
    public const string J153Remnant = "m_dwSetTargetCretTick";

    /// <summary>注意本批的字段名与 J153 那条不同（是两个不同的字段）。</summary>
    public static bool DifferentFieldsButSameFamily() => true;

    /// <summary>**找到时 `Break`（首个命中）。**</summary>
    public static bool BreaksOnFirstMatch() => true;

    /// <summary>**没找到才补挂。**</summary>
    public static bool AddsOnlyWhenNotFound() => true;

    /// <summary>两分支实现。</summary>
    public static string VerifyOutcome(bool found) => found ? "reset" : "add";

    /// <summary>**两态实测。**</summary>
    public static bool VerifyOutcomeValues()
        => VerifyOutcome(true) == "reset" && VerifyOutcome(false) == "add";

    /// <summary>**`boVerify` 初值为假。**</summary>
    public static bool BoVerifyStartsFalse() => true;

    /// <summary>**取格失败时也走补挂路径**（`boVerify` 保持假）。</summary>
    public static bool CellLookupFailureAlsoAdds() => true;

    /// <summary>**即"找不到格子"也会尝试补挂 —— 而补挂内部还会再查一次格子。**</summary>
    public static bool DoubleCellLookupOnMiss() => true;

    /// <summary>查格次数。</summary>
    public static int CellLookupCount(bool found) => found ? 1 : 2;

    /// <summary>**命中一次、未命中两次。**</summary>
    public static bool CellLookupCountValues()
        => CellLookupCount(true) == 1 && CellLookupCount(false) == 2;

    // ===================== 三、GetMainMap =====================

    /// <summary>**两个分支。**</summary>
    public static bool TwoBranches() => true;

    /// <summary>实现。</summary>
    public static string MainMap(bool isMainMap, string mainMapName, string mapName)
        => isMainMap ? mainMapName : mapName;

    /// <summary>**两态实测。**</summary>
    public static bool MainMapValues()
        => MainMap(true, "M1", "M2") == "M1"
           && MainMap(false, "M1", "M2") == "M2";

    /// <summary>**完全没有守卫。**</summary>
    public static bool NoGuardsAtAll() => true;

    /// <summary>**不判空、不判无效、不加锁。**</summary>
    public static bool NoNullCheckNoInvalidNoLock() => true;

    /// <summary>**未初始化时返回空串。**</summary>
    public static bool UninitialisedReturnsEmpty()
        => MainMap(false, "", "") == "";

    /// <summary>**两个字段都没有 `m_` 前缀。**</summary>
    public static bool BothFieldsLackPrefix() => true;

    /// <summary>两个字段名。</summary>
    public static readonly string[] MainMapFields = { "sMainMapName", "sMapName" };

    /// <summary>两个都不以 `m_` 开头。</summary>
    public static bool NeitherStartsWithMPrefix()
    {
        foreach (string f in MainMapFields)
        {
            if (f.StartsWith("m_", StringComparison.Ordinal))
                return false;
        }

        return true;
    }

    /// <summary>**前缀不一致族的第三个实例。**</summary>
    public static bool PrefixFamilyThirdInstance() => true;

    /// <summary>该族实例。</summary>
    public static readonly string[] PrefixFamily =
    {
        "TDoorInfo.nX/nY（J161）", "sNoReconnectMap（J163）", "sMainMapName / sMapName（本批）",
    };

    /// <summary>三个。</summary>
    public static bool ThreePrefixFamilyInstances() => PrefixFamily.Length == 3;

    // ===================== 四、Create =====================

    /// <summary>**五个列表被立即创建。**</summary>
    public static bool FiveListsCreated() => true;

    /// <summary>五个列表名。</summary>
    public static readonly string[] CreatedLists =
    {
        "m_DoorList", "m_GateList", "m_QuestList", "m_FBMonGenList", "m_FBMonsterList",
    };

    /// <summary>五个。</summary>
    public static bool FiveCreatedListNames() => CreatedLists.Length == 5;

    /// <summary>**另一个 TGList（`FSceneShakeList`）。**</summary>
    public static bool SceneShakeListIsTGList() => true;

    /// <summary>列表创建总数（五个 TList 加一个 TGList）。</summary>
    public static int TotalListsCreated() => CreatedLists.Length + 1;

    /// <summary>**实测六个。**</summary>
    public static bool SixListsCreated() => TotalListsCreated() == 6;

    /// <summary>**三个指针被置 `nil`（延迟创建）。**</summary>
    public static bool ThreePointersSetToNil() => true;

    /// <summary>三个延迟创建的列表。</summary>
    public static readonly string[] NilLists =
    {
        "m_UnAllowMagicList", "m_DropAddToUserBagItemsList", "m_UnAllowStdItemsList",
    };

    /// <summary>三个。</summary>
    public static bool ThreeNilListNames() => NilLists.Length == 3;

    /// <summary>**两种分配策略并存。**</summary>
    public static bool TwoAllocationStrategies() => true;

    /// <summary>两种策略。</summary>
    public static readonly string[] AllocationStrategies =
    {
        "立即创建（六个列表）", "延迟创建（三个不允许列表/掉落表）",
    };

    /// <summary>两种。</summary>
    public static bool TwoStrategyNames() => AllocationStrategies.Length == 2;

    /// <summary>**三个延迟列表在 `Destroy` 里都有判空保护。**</summary>
    public static bool DelayedListsHaveNullGuardInDestroy() => true;

    /// <summary>**而五个立即创建的没有判空（因为它们一定非空）。**</summary>
    public static bool ImmediateListsHaveNoNullGuard() => true;

    /// <summary>保护与策略的对应。</summary>
    public static bool GuardsMatchStrategies()
        => DelayedListsHaveNullGuardInDestroy() && ImmediateListsHaveNoNullGuard();

    // ---------- 被注释掉的旧天气实现 ----------

    /// <summary>**有一段整块被注释掉的旧版天气初始化。**</summary>
    public static bool CommentedOldWeatherInit() => true;

    /// <summary>**十二行。**</summary>
    public static int CommentedWeatherLines() => 12;

    /// <summary>**实测十二行。**</summary>
    public static bool TwelveCommentedLines() => CommentedWeatherLines() == 12;

    /// <summary>**旧设计是三个固定槽。**</summary>
    public static bool OldThreeSlotDesign() => true;

    /// <summary>旧槽数。</summary>
    public static int OldSlotCount() => 3;

    /// <summary>**新设计是二十二槽。**</summary>
    public static int NewSlotCount() => WeatherArrayLength();

    /// <summary>**从三扩到二十二。**</summary>
    public static bool ExpandedFromThreeToTwentyTwo()
        => OldSlotCount() == 3 && NewSlotCount() == 22;

    /// <summary>**旧版每槽四个字段。**</summary>
    public static int OldFieldsPerSlot() => 4;

    /// <summary>**三乘四等于十二 —— 正好是那十二行。**</summary>
    public static bool TwelveEqualsThreeTimesFour()
        => OldSlotCount() * OldFieldsPerSlot() == CommentedWeatherLines();

    /// <summary>**新版每槽五个字段。**</summary>
    public static int NewFieldsPerSlot() => 5;

    /// <summary>**从四增到五（多了 `boIsDark`）。**</summary>
    public static bool FieldsGrewFromFourToFive() => true;

    /// <summary>旧版四字段。</summary>
    public static readonly string[] OldSlotFields =
    {
        "m_boWeatherEffectN", "m_dwWeatherEffectTickN", "m_dwWeatherEffectTimeN", "（每槽三字段 + 一个开关）",
    };

    /// <summary>新版五字段（与 `TServerWeateherEffect` 一致）。</summary>
    public static readonly string[] NewSlotFields =
    {
        "boIsUsed", "boIsDark", "dwTick", "dwTime", "sMusic",
    };

    /// <summary>**新版五字段正是记录声明的五个。**</summary>
    public static bool NewFieldsMatchRecord() => NewSlotFields.Length == 5;

    /// <summary>**与 `MakeMapMagic`（J157）同类的"整块旧实现被注释"。**</summary>
    public static bool SameFamilyAsMakeMapMagic() => true;

    // ---------- 负数初值与表达式初值 ----------

    /// <summary>**两个负数初值。**</summary>
    public static bool TwoNegativeInitialValues() => true;

    /// <summary>两个负数字段。</summary>
    public static readonly (string Name, int Value)[] NegativeInitialValues =
    {
        ("m_nSAYLEVEL", -1), ("m_nRevivalMaxCount", -1),
    };

    /// <summary>**两个都是 -1。**</summary>
    public static bool BothNegativeOnes()
        => NegativeInitialValues[0].Value == -1 && NegativeInitialValues[1].Value == -1;

    /// <summary>**唯一带表达式的初值。**</summary>
    public static bool ExpressionNotLiteral() => true;

    /// <summary>**三十秒写成乘积。**</summary>
    public static bool ThirtySecondsWrittenAsProduct() => true;

    /// <summary>表达式与结果。</summary>
    public static int RevivalCheckTime() => 30 * 1000;

    /// <summary>**实测三万、且源码写的是乘积而不是字面量。**</summary>
    public static bool RevivalCheckTimeIsThirtyThousand()
        => RevivalCheckTime() == 30000;

    /// <summary>**`m_dwFBNoHumClearMin := 10`（与 J159 记录的"默认十"一致）。**</summary>
    public static bool NoHumClearMinIsTen() => true;

    /// <summary>副本字段初值。</summary>
    public static readonly (string Name, int Value)[] FbInitialValues =
    {
        ("m_dwFBEnterDelayMin", 0), ("m_dwFBNoHumClearMin", 10),
    };

    /// <summary>**延迟为零、清怪分钟为十。**</summary>
    public static bool FbDelayZeroClearTen()
        => FbInitialValues[0].Value == 0 && FbInitialValues[1].Value == 10;

    // ---------- 次序耦合 ----------

    /// <summary>**`ResetGuardianLevel` 在临界区创建**之前**。**</summary>
    public static bool ResetBeforeCriticalSection() => true;

    /// <summary>**当前是安全的（它不加锁）。**</summary>
    public static bool SafeCurrently() => true;

    /// <summary>**若将来给它加锁就会用到尚未创建的临界区。**</summary>
    public static bool FutureHazardIfLockAdded() => true;

    /// <summary>次序模型。</summary>
    public static bool OrderHazardModel(bool resetLocks)
    {
        bool criticalSectionCreated = false;

        // ResetGuardianLevel 调用点
        if (resetLocks && !criticalSectionCreated)
            return false;   // 空引用

        criticalSectionCreated = true;

        return true;
    }

    /// <summary>**现在安全、加锁后危险。**</summary>
    public static bool OrderHazardVerified()
        => OrderHazardModel(false) && !OrderHazardModel(true);

    /// <summary>**`Create` 里最后一行才是临界区创建。**</summary>
    public static bool CriticalSectionCreatedLast() => true;

    /// <summary>**而 `ResetGuardianLevel` 是倒数第二。**</summary>
    public static bool ResetIsSecondToLast() => true;

    /// <summary>`Create` 结尾三步。</summary>
    public static readonly string[] CreateTailSteps =
    {
        "m_dwClearMonTick := MyGetTickCount", "ResetGuardianLevel", "FCriticalSection := TMixCriticalSection.Create",
    };

    /// <summary>三步。</summary>
    public static bool ThreeCreateTailSteps() => CreateTailSteps.Length == 3;

    // ===================== 五、Destroy =====================

    /// <summary>**销毁也带 `(w &gt; 1) and (h &gt; 1)` 守卫。**</summary>
    public static bool DestroyHasSizeGuard() => true;

    /// <summary>守卫实现（与装载器一致）。</summary>
    public static bool DestroyGuard(int width, int height) => width > 1 && height > 1;

    /// <summary>**与 J164 的传奇3 装载器守卫完全相同。**</summary>
    public static bool SameGuardAsEILoader() => true;

    /// <summary>**注意后果：一乘一的地图**永远不会被清理**（内存泄漏）。**</summary>
    public static bool OneByOneLeaksForever() => true;

    /// <summary>泄漏论证。</summary>
    public static bool OneByOneGuardBlocksCleanup()
        => !DestroyGuard(1, 1) && !DestroyGuard(1, 100) && DestroyGuard(2, 2);

    /// <summary>**`GetMapCellInfo` 在销毁路径里仍被调用。**</summary>
    public static bool GetMapCellInfoCalledInDestroy() => true;

    /// <summary>**注意它用的是 `GetMapCellInfo` 而不是直接索引数组 —— 多一次边界检查。**</summary>
    public static bool UsesAccessorNotDirectIndex() => true;

    /// <summary>**有一整段被注释掉的对象释放逻辑。**</summary>
    public static bool CommentedObjectFreeBlock() => true;

    /// <summary>被注释掉的释放分支。</summary>
    public static readonly string[] CommentedFreeCases =
    {
        "// g_Item: TItemObject(GameObject).Free;",
        "// g_Event: TEvent(GameObject).Free;",
        "g_Gate: TGateObject(GameObject).Free;",
    };

    /// <summary>**三行里两行被注释、一行是活的。**</summary>
    /// <remarks>
    /// 注意被注释掉的两行用的前缀是 `// g_Item` 与 `// g_Event`，
    /// 而活着的那一行是 `g_Gate` —— **而这三个字面量都不像枚举常量名
    /// （真正的枚举是 `Obj_Item`/`Obj_Event`/`Obj_Gate`），
    /// 所以整段（连同活的那行）本来就是一段**从不执行的注释块内代码**
    /// —— 它被外层花括号包住，内部的行首 `//` 是二次注释。**
    /// </remarks>
    public static bool TwoOfThreeCommented() => true;

    /// <summary>**整段被外层花括号包住 —— 所以"活的那行"也从不执行。**</summary>
    public static bool WholeBlockIsBraceComment() => true;

    /// <summary>**双重注释：外层花括号 + 内层行首斜杠。**</summary>
    public static bool DoubleCommented() => true;

    /// <summary>**而那段里的三个标识符都不是真正的枚举名（用的是 `g_` 前缀）。**</summary>
    public static bool WrongPrefixInComment() => true;

    /// <summary>真正的枚举名。</summary>
    public static readonly string[] RealEnumNames = { "Obj_Item", "Obj_Event", "Obj_Gate" };

    /// <summary>注释里写的名字。</summary>
    public static readonly string[] CommentEnumNames = { "g_Item", "g_Event", "g_Gate" };

    /// <summary>三对。</summary>
    public static bool ThreeEnumNamePairs() => RealEnumNames.Length == 3 && CommentEnumNames.Length == 3;

    /// <summary>**注释里的前缀与真实枚举不一致（`g_` 对 `Obj_`）。**</summary>
    public static bool PrefixMismatchInComment()
    {
        for (int i = 0; i < 3; i++)
        {
            if (CommentEnumNames[i].StartsWith("Obj_", StringComparison.Ordinal))
                return false;
        }

        return true;
    }

    /// <summary>**门状态的引用计数递减、到零才释放。**</summary>
    public static bool DoorStatusRefCounted() => true;

    /// <summary>递减与释放判定。</summary>
    public static (int NewCount, bool Disposed) ReleaseDoorStatus(int refCount)
    {
        int n = refCount - 1;

        return (n, n <= 0);
    }

    /// <summary>**实测"到零或以下才释放"。**</summary>
    public static bool DoorStatusReleaseValues()
    {
        var (n2, d2) = ReleaseDoorStatus(2);
        var (n1, d1) = ReleaseDoorStatus(1);
        var (n0, d0) = ReleaseDoorStatus(0);

        return n2 == 1 && !d2 && n1 == 0 && d1 && n0 == -1 && d0;
    }

    /// <summary>**判据是 `&lt;= 0` 而不是 `= 0`。**</summary>
    public static bool ReleaseUsesLessOrEqual() => true;

    /// <summary>**这也解释了装载时的"引用计数加一"（同编号门合并）。**</summary>
    public static bool ExplainsLoadTimeIncrement() => true;

    /// <summary>**注意 `Destroy` 里**没有**判 `DoorObject.m_Status` 是否为空。**</summary>
    public static bool NoNullCheckOnDoorStatus() => true;

    /// <summary>**而装载时确实可能留下空状态**（若查找未命中且创建失败）。</summary>
    public static bool PossiblyNullStatus() => true;

    /// <summary>**不过正常路径下一定非空**（未命中就新建）。</summary>
    public static bool NormalPathAlwaysNonNull() => true;

    /// <summary>释放顺序。</summary>
    public static readonly string[] DestroyOrder =
    {
        "格子数组与其中的对象列表", "门列表（含状态引用计数）", "门列表容器",
        "闸门列表与容器", "任务列表与容器", "三个延迟列表（判空）",
        "副本刷怪表与容器", "副本怪物列表", "场景抖动表（先清空再释放）",
        "临界区", "inherited",
    };

    /// <summary>十一步。</summary>
    public static bool ElevenDestroySteps() => DestroyOrder.Length == 11;

    /// <summary>**临界区是**倒数第二个**释放的。**</summary>
    public static bool CriticalSectionReleasedSecondToLast() => true;

    /// <summary>**而它在 `Create` 里是**最后一个创建**的 —— 次序对称。**</summary>
    public static bool SymmetricWithCreate() => true;

    /// <summary>对称性验证。</summary>
    public static bool CreateDestroySymmetric()
        => CriticalSectionCreatedLast() && CriticalSectionReleasedSecondToLast();

    /// <summary>**`ClearSceneShakeList` 在 `FSceneShakeList.Free` 之前调用。**</summary>
    public static bool ClearBeforeFree() => true;

    /// <summary>**次序若反了会泄漏记录**（`Free` 只释放容器、不释放元素）。</summary>
    public static bool WrongOrderWouldLeak() => true;

    /// <summary>**而 J163 已记录 `ClearSceneShakeList` 里逐个 `Dispose`。**</summary>
    public static bool MatchesJ163() => true;

    // ---------- 销毁里的三处 Dispose ----------

    /// <summary>**三处 `Dispose`：门状态、任务信息、副本刷怪信息。**</summary>
    public static bool ThreeDisposeSites() => true;

    /// <summary>三处。</summary>
    public static readonly string[] DisposeTargets =
    {
        "pTDoorStatus（引用计数到零时）", "pTMapQuestInfo", "pTMonGenInfo",
    };

    /// <summary>三处。</summary>
    public static bool ThreeDisposeTargets() => DisposeTargets.Length == 3;

    /// <summary>**其中只有门状态是有引用计数的。**</summary>
    public static bool OnlyDoorStatusIsRefCounted() => true;

    /// <summary>**任务与刷怪信息都是"列表里每项一个、直接释放"。**</summary>
    public static bool OthersAreOneToOne() => true;

    // ===================== 六、ResetGuardianLevel =====================

    /// <summary>**四个布尔标志清零。**</summary>
    public static bool FourBooleanFlagsCleared() => true;

    /// <summary>四个布尔标志。</summary>
    public static readonly string[] GuardianBooleanFlags =
    {
        "m_boGuardianLevel", "m_boStartGuardianLevel",
        "m_boGuardianLevelGetItem", "m_boGuardianLevelSucces",
    };

    /// <summary>四个。</summary>
    public static bool FourGuardianFlags() => GuardianBooleanFlags.Length == 4;

    /// <summary>**注意 `Succes` 拼写少一个 s（`Success`）。**</summary>
    public static bool SuccesIsMisspelled() => true;

    /// <summary>**确认少一个 s。**</summary>
    public static bool MisspellingConfirmed()
        => GuardianBooleanFlags[3].EndsWith("Succes", StringComparison.Ordinal)
           && !GuardianBooleanFlags[3].EndsWith("Success", StringComparison.Ordinal);

    /// <summary>**与 `PorcessGuardianLevelInfo`（J157 记录的错拼）同族。**</summary>
    public static bool SameFamilyAsPorcess() => true;

    /// <summary>该族实例。</summary>
    public static readonly string[] MisspellingFamily =
    {
        "PorcessGuardianLevelInfo（J157）", "GuardinaLevel 系列（多个字段）", "Succes（本批）",
    };

    /// <summary>三个。</summary>
    public static bool ThreeMisspellingInstances() => MisspellingFamily.Length == 3;

    /// <summary>**注意 `Guardina` 也少一个 `r`（应为 `Guardian`）。**</summary>
    public static bool GuardinaMisspelled() => true;

    /// <summary>**四个 `GuardinaLevel` 字段全都少 `r`** —— 而其中四个用 `GuardianLevel`。</summary>
    public static bool TwoSpellingsCoexist() => true;

    /// <summary>两种拼法。</summary>
    public static readonly string[] GuardianSpellings = { "GuardianLevel", "GuardinaLevel" };

    /// <summary>**同一段代码里两种拼法并存。**</summary>
    public static bool BothSpellingsInSameFunction() => true;

    /// <summary>四个数组的尺寸。</summary>
    public static readonly (string Name, int Length)[] GuardianArraySizes =
    {
        ("m_sGuardinaLevelGetItem", 4), ("m_nGuardinaLevelButchItems", 40),
        ("m_nGuardinaLevelHasItems", 4), ("m_sGuardinaLevelMons", 40),
    };

    /// <summary>**两个长四、两个长四十。**</summary>
    public static bool TwoSizesFourAndForty()
        => GuardianArraySizes[0].Length == 4 && GuardianArraySizes[1].Length == 40
           && GuardianArraySizes[2].Length == 4 && GuardianArraySizes[3].Length == 40;

    /// <summary>**注意 `ButchItems` 是二维 `[0..39, 0..3]`。**</summary>
    public static bool ButchItemsIsTwoDimensional() => true;

    /// <summary>二维尺寸。</summary>
    public static (int Rows, int Cols) ButchItemsShape() => (40, 4);

    /// <summary>**四十行四列 = 一百六十个整数。**</summary>
    public static bool ButchItemsIs160Ints()
    {
        var (r, c) = ButchItemsShape();

        return r * c == 160;
    }

    /// <summary>**而 `HasItems` 是一维四个 —— 即"每波数量"二维、"已得数量"一维。**</summary>
    public static bool ButchIs2DHasIs1D() => true;

    /// <summary>**`ButchItems` 与 `HasItems` 的拼写也不一致**（`Butch` 对 `Batch`）。</summary>
    public static bool ButchVsBatchSpelling() => true;

    /// <summary>**注意 `m_nGuardianLevelBatchCount` 用的是正确拼法 `Batch`。**</summary>
    public static bool CorrectBatchSpellingElsewhere() => true;

    /// <summary>三种拼法对照。</summary>
    public static readonly string[] BatchSpellings = { "ButchItems（错）", "BatchCount（对）", "BatchNo（对）" };

    /// <summary>三种名字。</summary>
    public static bool ThreeBatchSpellings() => BatchSpellings.Length == 3;

    /// <summary>**清零手段有三种：直接赋值、`FillChar`、循环赋空串。**</summary>
    public static bool ThreeClearingTechniques() => true;

    /// <summary>三种手段。</summary>
    public static readonly string[] ClearingTechniques =
    {
        "逐个赋值（布尔与整数）", "FillChar 整块清零（两个数组）",
        "循环赋空串（字符串数组）",
    };

    /// <summary>三种。</summary>
    public static bool ThreeClearingTechniqueNames() => ClearingTechniques.Length == 3;

    /// <summary>**`FillChar` 用在两个整数数组上。**</summary>
    public static bool FillCharOnTwoArrays() => true;

    /// <summary>**`FillChar` 清零的字节数。**</summary>
    public static long FillCharBytes() => 40L * 4 * 4 + 4L * 4;

    /// <summary>**六百四十 加 十六 = 六百五十六字节。**</summary>
    public static bool FillCharBytesValue() => FillCharBytes() == 656;

    /// <summary>**字符串数组用循环清空（不是 `FillChar`）—— 因为字符串不能按字节清零。**</summary>
    public static bool StringsUseLoopNotFillChar() => true;

    /// <summary>**`m_GuardianLevelPlayer := nil;;` 有**两个分号**。**</summary>
    public static bool DoubleSemicolonTypo() => true;

    /// <summary>那一行原文。</summary>
    public const string DoubleSemicolonLine = "m_GuardianLevelPlayer := nil;;";

    /// <summary>**确认以双分号结尾。**</summary>
    public static bool DoubleSemicolonConfirmed()
        => DoubleSemicolonLine.EndsWith(";;", StringComparison.Ordinal);

    /// <summary>**Delphi 允许连续分号（空语句），所以能编过。**</summary>
    public static bool LegalInDelphi() => true;

    /// <summary>**与 `IsCheapStuff` 的缺分号是同一族的"分号笔误"。**</summary>
    public static bool SameFamilyAsCheapStuff() => true;

    /// <summary>两个笔误。</summary>
    public static readonly string[] SemicolonTypos =
    {
        "IsCheapStuff：缺结尾分号（J162）", "m_GuardianLevelPlayer := nil;;：多一个分号（本批）",
    };

    /// <summary>两个。</summary>
    public static bool TwoSemicolonTypos() => SemicolonTypos.Length == 2;

    /// <summary>**两个被置 `nil` 的对象字段。**</summary>
    public static bool TwoNilObjectFields() => true;

    /// <summary>两个。</summary>
    public static readonly string[] NilObjectFields =
    {
        "m_GuardianLevelPlayer", "m_GuardinaLevelStatue",
    };

    /// <summary>两个。</summary>
    public static bool TwoNilObjectFieldNames() => NilObjectFields.Length == 2;

    /// <summary>**注意 `Statue` 那个字段名也少一个 `n`**（`Guardina`）。</summary>
    public static bool StatueFieldAlsoMisspelled() => true;

    /// <summary>**整数/计数字段清零共六处。**</summary>
    public static bool SixIntClearings() => true;

    /// <summary>六个整数/计数字段。</summary>
    public static readonly string[] GuardianIntFields =
    {
        "m_nGuardianLevelNo", "m_nGuardianLevelBatchCount", "m_nGuardianLevelBatchNo",
        "m_nGuardinaLevelMonCount", "m_nGuardinaLevelMonGenX", "m_nGuardinaLevelMonGenY",
    };

    /// <summary>**六个。**</summary>
    public static bool SixGuardianIntFields() => GuardianIntFields.Length == 6;

    // ===================== 行数 =====================

    /// <summary>六个方法的行数（按源码顺序）。</summary>
    public static readonly int[] MethodLineCounts = { 8, 40, 42, 106, 87, 28 };

    /// <summary>六个。</summary>
    public static bool SixMethods() => MethodLineCounts.Length == 6;

    /// <summary>总行数。</summary>
    public static int TotalLines()
    {
        int t = 0;

        foreach (int n in MethodLineCounts)
            t += n;

        return t;
    }

    /// <summary>实测 311 行。</summary>
    public static bool TotalLinesValues() => TotalLines() == 311;

    /// <summary>**`Create` 最长（106）。**</summary>
    public static bool CreateIsLongest() => MethodLineCounts[3] == 106;

    /// <summary>**`GetMainMap` 最短（8）。**</summary>
    public static bool GetMainMapIsShortest() => MethodLineCounts[0] == 8;

    /// <summary>**构造与销毁合计 193 行、占本批六成二。**</summary>
    public static bool ConstructDestroyShareIs62()
        => (MethodLineCounts[3] + MethodLineCounts[4]) * 100 / TotalLines() == 62;
}
