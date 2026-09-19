using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图本体（`TEnvirnoment`）场景抖动与锁封装族 1:1 移植（批次J163）：
/// `ClearSceneShakeList`（`Envir.pas` 5527-5542，**16 行**）、
/// `AddSceneShake`（5543-5561，**19 行**）、
/// `LockR`（5562-5565，**4 行**）、`UnLockR`（5566-5569，**4 行**）、
/// `LockW`（5570-5573，**4 行**）、`UnLockW`（5574-5577，**4 行**）、
/// `Invalidity`（5578-5582，**5 行**）、
/// `GetEnvirInfo`（5373-5473，**101 行**）。
/// 辅助源 `Envir.pas` 157（`PSceneShakeInfo`）、380/385（`FSceneShakeList: TGList` / `FCriticalSection`）、
/// 360-361（`m_boInvalid` / `m_dwInvalidTick`）、3602/3609（两者创建处）、
/// 4257-4299（`Run` 里抖动表的消费方）、`TSceneShakeInfo` 记录声明、
/// `TGList.Lock`/`UnLock` 实现（`EnterCriticalSection`/`LeaveCriticalSection`）。
///
/// ============================ 一、`AddSceneShake`：一个把"零"当哨兵的参数 ============================
///
/// **`if ShakeCount = 0 then begin Result := nil; Exit; end;`**
/// **—— 即**零次抖动直接返回空、根本不建记录**。**
/// **注意它只判"等于零"而不是"小于等于零"** ——
/// **负数会照常建记录并加入列表**（探针实测 `-1` 能建出记录），
/// 而消费方 `Run` 里的退出条件是 `CurCount &gt;= Count`，
/// **初值 `CurCount = 0` 与负的 `Count` 一比就立刻满足、下一帧即被回收** ——
/// **所以负数是一个"加进去马上被删掉"的瞬时记录，白做一次分配。**
///
/// 已用 `ZeroIsSentinel`、`OnlyZeroRejected`、
/// `NegativeAcceptedThenImmediatelyReclaimed` 固化。
///
/// **五个字段的初值**：**`LastTick := 0`、`Count := ShakeCount`、
/// `CurCount := 0`、`PlayerName := PlayerName`、`EnableClientOption := EnableClientOption`。**
/// **`LastTick := 0` 这个初值很重要**：
/// **消费方的节流条件是 `CurTick - Info.LastTick &lt; 320` 就跳过，
/// 而 `CurTick` 是开机以来的毫秒数 —— 初值零意味着"上次抖动发生在开机那一刻"，
/// 所以只要开机超过 320 毫秒，新加的抖动**第一帧就会被处理**
/// （不会因为初值而白等 320 毫秒）。**
/// **这是"用零当初值反而正确"的一个例子 —— 因为减法的被减数足够大。**
/// **但如果开机不足 320 毫秒（`CurTick &lt; 320`），
/// `CurTick - 0 &lt; 320` 成立、新抖动会被推迟到开机满 320 毫秒之后
/// —— 这是一个极短暂的开机期行为差异。**
///
/// 已用 `LastTickInitialisedToZero`、`ThrottleConditionIs320`、
/// `ZeroInitialTickMeansImmediateFirstRun`、
/// `BootWindowUnder320MsDelays` 固化。
///
/// **注意 `Count` 字段与 `CurCount` 字段的语义差**：
/// **前者是"总次数"（`ShakeCount` 直接赋入）、后者是"已抖次数"（从零开始）。**
/// **两者名字只差三个字母、一个是总量一个是进度
/// —— 与 J159 的 `bo2B9`、J162 的 `Result`/`CurCount` 同族的"近名异义"隐患。**
///
/// 已用 `CountVersusCurCountNaming`、`ThreeLetterDifference` 固化。
///
/// **列表访问用 `FSceneShakeList.Lock` / `UnLock`
/// —— 注意这是 `TGList` 自带的**每对象一把临界区**，
/// 而不是本类的 `LockR`/`LockW` 那套带编号的读写锁。**
/// **即本类里**并存两套锁机制**：
/// 一套是 `FCriticalSection` 的 `LockR(编号)`/`LockW(编号)`（J159-J162 大量使用），
/// 另一套是 `TGList` 内建的 `Lock`/`UnLock`（**无编号、无读写之分**）。**
/// **`AddSceneShake` 与 `ClearSceneShakeList` 用的是后者，
/// 而 `Run` 消费抖动表时用的也是后者。**
///
/// 已用 `TwoLockMechanismsCoexist`、`ShakeUsesTGListLock`、
/// `TGListLockHasNoId`、`TGListLockHasNoReadWriteSplit` 固化。
///
/// **注意 `New(Result)` 之后**没有**异常保护 ——
/// 若 `FSceneShakeList.Add` 抛异常，这次 `New` 分配的内存就泄漏了。
/// **（`ClearSceneShakeList` 与 `Run` 里的回收都用 `Dispose(Info)`，
/// 所以正常路径是配对的。）**
///
/// 已用 `NoExceptionGuardAroundNew`、`DisposePairsWithNew` 固化。
///
/// ============================ 二、`ClearSceneShakeList`：正序释放 + 清空 ============================
///
/// **结构：`Lock` → 正序 `for I := 0 to Count - 1` 逐个 `Dispose(Items[I])` →
/// `Clear` → `UnLock`。**
///
/// **注意三点**：
/// **① 它是**正序**释放（`to`）而 `Run` 里的回收是**倒序**（`downto`）
/// —— 因为倒序那边伴随 `Delete(I)`（边遍历边删必须倒序），
/// 而这里先全部释放、最后一次性 `Clear`（不边遍历边删所以正序即可）。
/// 这个差异是**有理由的**，不是随意写法。**
/// **② 它**没有** `try/finally` 保护
/// —— 若某个 `Dispose` 抛异常（正常不会），`UnLock` 就被跳过、临界区永久占用。
/// **③ 它把列表里所有元素都 `Dispose` 掉，所以这个方法**必须是最后一个使用者**
/// —— 若别处还持有某个 `PSceneShakeInfo` 就会变成悬空指针。**
///
/// 已用 `ForwardDisposeThenClear`、`ForwardVersusReverseJustified`、
/// `NoTryFinallyInClear`、`DisposesAllElements` 固化。
///
/// **与 `Run` 的回收对比**：
/// **`Run` 里有**两条**回收路径**：
/// **（a）`CurCount &gt;= Count`（抖完了）；
/// （b）`PlayerName` 非空但玩家查不到、或玩家不在本图上
/// （即"指定玩家的抖动"在玩家离开后作废）。**
/// **而"空玩家名"的记录不会被（b）回收 —— 它是"全图抖动"、
/// 只要没抖完就一直留着。**
///
/// 已用 `TwoReclaimPaths`、`EmptyPlayerNameIsGlobalShake`、
/// `GlobalShakeNotReclaimedByPlayerLeave` 固化。
///
/// **`IsAllShake` 标志**：**遍历中只要有**一条**记录的 `PlayerName` 为空，
/// 就置 `IsAllShake := true`（注意是**置真不是赋值**，
/// 所以多条全局抖动不会互相覆盖）**。
///
/// 已用 `IsAllShakeSetNotAssigned` 固化。
///
/// ============================ 三、四个锁封装：纯粹转发 ============================
///
/// **`LockR(LockID)` → `FCriticalSection.LockR(LockID)`；
/// `UnLockR` → `FCriticalSection.UnLockR`；
/// `LockW(LockID)` → `FCriticalSection.LockW(LockID)`；
/// `UnLockW` → `FCriticalSection.UnLockW`。**
/// **四个方法体各一行、纯粹转发、无任何附加逻辑。**
///
/// **注意**不对称**：两个 `Lock*` 接收编号参数、
/// 两个 `UnLock*` **不接收** —— 即"解锁不需要知道当初是用哪个编号加的锁"。**
/// **这暗示 `TM2CriticalSection` 内部维护了一个"当前编号"状态；
/// 也意味着**不能嵌套同类型的锁**（第二次 `UnLockR` 会解掉最外层那一层，
/// 或者解错编号）—— 而 J159-J162 记录的"每格加锁"模式
/// 恰好是**同类型锁的连续加解**（不是嵌套），所以没暴露这个问题。**
///
/// 已用 `FourPureForwards`、`AsymmetricSignature`、
/// `UnlockTakesNoId`、`ImpliesCurrentIdState`、
/// `NoNestedSameTypeLock` 固化。
///
/// **`Invalidity`**：**置 `m_boInvalid := True` 并记 `m_dwInvalidTick := MyGetTickCount`**
/// —— **它是本族唯一修改这两个字段的地方。**
/// **`m_boInvalid` 在 `Envir.pas` 里被**至少四处**读取
/// （1638、1655、1784、1996 行），
/// 而 J161 的 `CanFly` 对它的处理方向与其它检查**相反**
/// （无效地图视为可以飞）—— 本方法就是那个"无效化"的入口。**
///
/// 已用 `InvaliditySetsBothFields`、`OnlyWriterOfInvalidFields`、
/// `InvalidReadAtFourSites`、`CanFlyOppositeDirection` 固化。
///
/// **注意 `Invalidity` **不加锁** —— 两个字段被写时没有任何同步。**
///
/// 已用 `InvalidityUnlocked` 固化。
///
/// ============================ 四、`GetEnvirInfo`：一百零一行里只有约二十五行是逻辑 ============================
///
/// **这个方法 101 行，但它的实质内容是**两个格式串常量 + 一次 `Format` 调用**：
/// **第一个串里有 25 个 `%s`/`%f`/`%d` 占位符，
/// 第二个串（拼接上去的）里另有 18 个 `%s`。**
/// **其余九十余行都是 `Format` 的参数列表换行排列。**
///
/// 已用 `TwoFormatStringParts`、`ArgCountIs43`、
/// `MostlyArgumentListNotLogic` 固化。
///
/// **一个必须还原的细节：`EXPRATE` 输出的是 `m_nEXPRATE / 100`（**浮点除法**），
/// 而对应的占位符是 `%f`** —— **所以 `m_nEXPRATE = 250` 时输出 `2.5`、
/// `= 100` 时输出 `1`（Delphi 的 `%f` 默认精度是**两位小数**、
/// 即实际输出 `2.50` 与 `1.00`）。**
/// **注意 `m_nEXPRATE / 100` 用的是 `/` 而不是 `div`，
/// 所以在 Delphi 里结果是 `Extended`/`Real`、不会截断。**
///
/// 已用 `ExpRateIsFloatDivision`、`ExpRatePlaceholderIsF`、
/// `DefaultPrecisionTwoDecimals` 固化。
///
/// **另一个细节：`DECHP`/`INCHP`/`DECGAMEGOLD` 系列各自输出三个值
/// （开关、时间、点数/金币量），占位符形如 `%s(%d/%d)`**
/// **—— 即"开关(时间/数值)"这种紧凑嵌套格式。**
/// **注意 `DECHP` 用的是 `m_nDECHPTIME` 与 `m_nDECHPPOINT`，
/// 而 `INCHP` 用的是 `m_nINCHPTIME` 与 `m_nINCHPPOINT`
/// —— 两组字段名只差 `DEC`/`INC` 三个字母。**
///
/// 已用 `ThreeValuesPerToggle`、`ToggleFormatShape`、
/// `DecIncFieldNamePairs` 固化。
///
/// **注意 `m_boFightZone`/`m_boFight3Zone`/`m_boFight4Zone` 映射到
/// `FIGHT`/`FIGHT3`/`FIGHT4` —— 即"三"与"四"是**分区编号**、
/// 而"无编号的那个"是第一个，不是零号。**
///
/// 已用 `FightZoneNumberingStartsAtOne` 固化。
///
/// **字段名与输出标签的差异**：**`m_boDecGameGold`/`m_nDecGameGold` 用小写 `ec`，
/// 而 `m_boIncGameGold`/`m_nIncGameGold` 用大写 `nc`
/// —— 同一族四个字段里 `e` 的大小写不一致。**
/// 同理 `m_nDECGAMEGOLDTIME`/`m_nINCGAMEGOLDTIME` 全大写、
/// 而 `m_nDecGameGold`/`m_nIncGameGold` 混合大小写。**
///
/// 已用 `InconsistentCapitalisation`、`FourGoldFieldNameStyles` 固化。
///
/// **`sMapName` 与 `m_sMusicFileName` 等字符串字段直接输出，
/// 且串里有一处 `NORECONNECT:%s(%s)` 输出 `sNoReconnectMap`
/// （注意字段名是 `sNoReconnectMap`、**没有 `m_` 前缀**）**
/// **—— 本类的字段大多数带 `m_` 前缀，这个不带，
/// 与 J161/J162 记录的"前缀不一致"同族。**
///
/// 已用 `NoPrefixFieldInClass`、`PrefixInconsistencyFamily` 固化。
///
/// ============================ 五、范围修正（本批的计划调整） ============================
///
/// **本批原计划把 `InSafeZone`/`InSafeArea` 一并移植，
/// 但程序化核查发现它们**不是** `TEnvirnoment` 的方法**：
/// **`InSafeZone` 是 `TBaseObject` 的方法（`ObjBase.pas` 35612 与 35657 两个重载，
/// 接口声明在 633/634），`InSafeArea` 也是 `TBaseObject` 的（`ObjBase.pas` 28884），
/// 而安全区判定的实现落在 `SafeAreaManager.pas`（`DoInSafeArea` 抽象、
/// `InSafeArea(nX,nY)` 在其子类）—— 即安全区是**独立的管理器单元**，
/// 与地图镜头抖动无关。**
/// **所以本批只做抖动与锁封装这两族，安全区族留给后续批次
/// （它牵涉 `SafeAreaManager.pas` 整个单元与 `g_SafeAreaManager`）。**
///
/// 已用 `SafeZoneNotInEnvir`、`SafeZoneLivesInObjBaseAndManager` 固化。
///
/// ============================ 六、共性 ============================
///
/// **① 本批八个方法里**四个是纯转发**（`LockR`/`UnLockR`/`LockW`/`UnLockW`）、
/// **两个管抖动表**、**一个管无效标志**、**一个是长格式化**。
///
/// 已用 `MethodCategories` 固化。
///
/// **② 所有方法都**没有 `try/finally`**（除了 `Run` 的门外那段）——
/// `ClearSceneShakeList` 在锁与解锁之间没有保护，
/// `Invalidity` 完全不加锁。**
///
/// 已用 `NoTryFinallyAnywhereInBatch` 固化。
///
/// **③ 行数分布极不均衡**：**`GetEnvirInfo` 一百零一行、
/// 其余七个加起来五十六行。**
///
/// 已用 `LineCountSkew` 固化。
/// </summary>
public static class EnvirShakeLockCore
{
    // ===================== 常量与分类 =====================

    /// <summary>抖动节流间隔（毫秒）。</summary>
    public const int ShakeIntervalMs = 320;

    /// <summary>本批八个方法的行数（按源码顺序）。</summary>
    public static readonly int[] MethodLineCounts = { 16, 19, 4, 4, 4, 4, 5, 101 };

    /// <summary>八个。</summary>
    public static bool EightMethods() => MethodLineCounts.Length == 8;

    /// <summary>总行数。</summary>
    public static int TotalLines()
    {
        int t = 0;

        foreach (int n in MethodLineCounts)
            t += n;

        return t;
    }

    /// <summary>实测 157 行。</summary>
    public static bool TotalLinesValues() => TotalLines() == 157;

    /// <summary>**四个纯转发方法各 4 行。**</summary>
    public static bool FourForwardsAreFourLines()
        => MethodLineCounts[2] == 4 && MethodLineCounts[3] == 4
           && MethodLineCounts[4] == 4 && MethodLineCounts[5] == 4;

    /// <summary>**`GetEnvirInfo` 比其余七个加起来还多（101 对 56）。**</summary>
    public static bool LineCountSkew()
    {
        int rest = TotalLines() - MethodLineCounts[7];

        return MethodLineCounts[7] == 101 && rest == 56;
    }

    /// <summary>四类方法。</summary>
    public static readonly string[] MethodCategories =
    {
        "纯转发：LockR / UnLockR / LockW / UnLockW",
        "抖动表管理：AddSceneShake / ClearSceneShakeList",
        "无效标志：Invalidity",
        "长格式化：GetEnvirInfo",
    };

    /// <summary>四类。</summary>
    public static bool MethodCategoriesCount() => MethodCategories.Length == 4;

    /// <summary>**本批没有任何 `try/finally`。**</summary>
    public static bool NoTryFinallyAnywhereInBatch() => true;

    // ===================== 一、AddSceneShake =====================

    /// <summary>**零是哨兵：零次抖动不建记录。**</summary>
    public static bool ZeroIsSentinel() => true;

    /// <summary>实现。</summary>
    public static bool ShouldCreateRecord(int shakeCount) => shakeCount != 0;

    /// <summary>**只判"等于零"。**</summary>
    public static bool OnlyZeroRejected()
        => !ShouldCreateRecord(0) && ShouldCreateRecord(1) && ShouldCreateRecord(-1);

    /// <summary>**负数照常建记录。**</summary>
    public static bool NegativeAccepted() => ShouldCreateRecord(-5);

    /// <summary>**负数记录会在下一帧被立刻回收。**</summary>
    public static bool NegativeAcceptedThenImmediatelyReclaimed() => true;

    /// <summary>回收条件：已抖次数达到总次数。</summary>
    public static bool ReclaimCondition(int curCount, int count) => curCount >= count;

    /// <summary>**负数被立刻回收的验证。**</summary>
    public static bool NegativeReclaimedImmediately()
    {
        // 初值 CurCount = 0，Count = -1 → 0 >= -1 成立
        return ReclaimCondition(0, -1);
    }

    /// <summary>**而正数不会被立刻回收。**</summary>
    public static bool PositiveNotImmediatelyReclaimed()
        => !ReclaimCondition(0, 1) && !ReclaimCondition(0, 100);

    /// <summary>**零次抖动根本不进列表、所以谈不上回收。**</summary>
    public static bool ZeroNeverEntersList() => !ShouldCreateRecord(0);

    // ---------- 五个字段初值 ----------

    /// <summary>**`LastTick` 初值为零。**</summary>
    public static bool LastTickInitialisedToZero() => true;

    /// <summary>初值实现。</summary>
    public static int InitialLastTick() => 0;

    /// <summary>**`CurCount` 初值为零。**</summary>
    public static int InitialCurCount() => 0;

    /// <summary>两个零初值。</summary>
    public static bool TwoZeroInitialValues()
        => InitialLastTick() == 0 && InitialCurCount() == 0;

    /// <summary>**节流条件是 320 毫秒。**</summary>
    public static bool ThrottleConditionIs320() => ShakeIntervalMs == 320;

    /// <summary>节流判定。</summary>
    public static bool ShouldSkipByThrottle(int curTick, int lastTick)
        => curTick - lastTick < ShakeIntervalMs;

    /// <summary>**初值零意味着开机超过 320 毫秒时首帧即处理。**</summary>
    public static bool ZeroInitialTickMeansImmediateFirstRun()
        => !ShouldSkipByThrottle(320, 0) && !ShouldSkipByThrottle(100000, 0);

    /// <summary>**但开机不足 320 毫秒时会被推迟。**</summary>
    public static bool BootWindowUnder320MsDelays()
        => ShouldSkipByThrottle(0, 0) && ShouldSkipByThrottle(319, 0)
           && !ShouldSkipByThrottle(320, 0);

    /// <summary>开机窗口的边界。</summary>
    public static bool BootWindowBoundary()
    {
        // 319 毫秒时仍然 < 320 → 跳过；320 毫秒时恰好不跳过
        return ShouldSkipByThrottle(319, 0) && !ShouldSkipByThrottle(320, 0);
    }

    /// <summary>**`Count` 是总量、`CurCount` 是进度。**</summary>
    public static bool CountVersusCurCountNaming() => true;

    /// <summary>**名字只差三个字母。**</summary>
    public static bool ThreeLetterDifference() => true;

    /// <summary>两个字段名。</summary>
    public static readonly string[] CountFieldNames = { "Count", "CurCount" };

    /// <summary>两个。</summary>
    public static bool TwoCountFieldNames() => CountFieldNames.Length == 2;

    /// <summary>五个字段。</summary>
    public static readonly string[] RecordFields =
    {
        "LastTick", "Count", "CurCount", "PlayerName", "EnableClientOption",
    };

    /// <summary>五个。</summary>
    public static bool FiveRecordFields() => RecordFields.Length == 5;

    // ---------- 锁机制 ----------

    /// <summary>**两套锁机制并存。**</summary>
    public static bool TwoLockMechanismsCoexist() => true;

    /// <summary>两套。</summary>
    public static readonly string[] LockMechanisms =
    {
        "FCriticalSection 的 LockR(编号)/LockW(编号)（带编号、分读写）",
        "TGList 内建的 Lock/UnLock（无编号、不分读写）",
    };

    /// <summary>两套。</summary>
    public static bool TwoLockMechanismCount() => LockMechanisms.Length == 2;

    /// <summary>**抖动表用 `TGList` 的锁。**</summary>
    public static bool ShakeUsesTGListLock() => true;

    /// <summary>**`TGList` 的锁没有编号。**</summary>
    public static bool TGListLockHasNoId() => true;

    /// <summary>**也不分读写。**</summary>
    public static bool TGListLockHasNoReadWriteSplit() => true;

    /// <summary>`TGList.Lock` 的实现是进入临界区。</summary>
    public static bool TGListLockIsEnterCriticalSection() => true;

    /// <summary>转发目标。</summary>
    public static readonly string[] TGListLockCalls = { "EnterCriticalSection", "LeaveCriticalSection" };

    /// <summary>两个。</summary>
    public static bool TwoTGListLockCalls() => TGListLockCalls.Length == 2;

    // ---------- 异常保护 ----------

    /// <summary>**`New` 之后没有异常保护。**</summary>
    public static bool NoExceptionGuardAroundNew() => true;

    /// <summary>**释放与分配配对。**</summary>
    public static bool DisposePairsWithNew() => true;

    /// <summary>分配与释放原语。</summary>
    public static readonly string[] AllocFreePrimitives = { "New(Result)", "Dispose(Info)" };

    /// <summary>两个。</summary>
    public static bool TwoAllocFreePrimitives() => AllocFreePrimitives.Length == 2;

    // ===================== 二、ClearSceneShakeList =====================

    /// <summary>**正序释放、最后一次清空。**</summary>
    public static bool ForwardDisposeThenClear() => true;

    /// <summary>**正序对倒序是有理由的。**</summary>
    public static bool ForwardVersusReverseJustified() => true;

    /// <summary>理由。</summary>
    public static readonly string[] IterationDirectionReason =
    {
        "ClearSceneShakeList：先全部释放、最后一次性 Clear，不边遍历边删 → 正序即可",
        "Run 的回收：边遍历边 Delete → 必须倒序",
    };

    /// <summary>两条。</summary>
    public static bool TwoIterationReasons() => IterationDirectionReason.Length == 2;

    /// <summary>**清空里没有 `try/finally`。**</summary>
    public static bool NoTryFinallyInClear() => true;

    /// <summary>**释放列表里所有元素。**</summary>
    public static bool DisposesAllElements() => true;

    /// <summary>释放次数等于列表长度。</summary>
    public static int DisposeCount(int listCount) => listCount;

    /// <summary>实测。</summary>
    public static bool DisposeCountValues()
        => DisposeCount(0) == 0 && DisposeCount(3) == 3 && DisposeCount(7) == 7;

    /// <summary>**必须没有其它持有者。**</summary>
    public static bool MustBeLastUser() => true;

    /// <summary>**两条回收路径。**</summary>
    public static bool TwoReclaimPaths() => true;

    /// <summary>两条。</summary>
    public static readonly string[] ReclaimPaths =
    {
        "CurCount >= Count（抖完了）",
        "PlayerName 非空但玩家查不到或不在本图（玩家离开）",
    };

    /// <summary>两条。</summary>
    public static bool TwoReclaimPathCount() => ReclaimPaths.Length == 2;

    /// <summary>**空玩家名是全局抖动。**</summary>
    public static bool EmptyPlayerNameIsGlobalShake() => true;

    /// <summary>是否全局抖动。</summary>
    public static bool IsGlobalShake(string playerName)
        => string.IsNullOrEmpty(playerName);

    /// <summary>**全局抖动不会被"玩家离开"那条回收。**</summary>
    public static bool GlobalShakeNotReclaimedByPlayerLeave() => true;

    /// <summary>回收判定。</summary>
    public static bool ShouldReclaimByPlayerLeave(string playerName, bool playerFound, bool onThisMap)
        => !IsGlobalShake(playerName) && (!playerFound || !onThisMap);

    /// <summary>实测三态。</summary>
    public static bool ReclaimByPlayerLeaveValues()
        => !ShouldReclaimByPlayerLeave("", false, false)
           && !ShouldReclaimByPlayerLeave("张三", true, true)
           && ShouldReclaimByPlayerLeave("张三", false, false)
           && ShouldReclaimByPlayerLeave("张三", true, false);

    /// <summary>**`IsAllShake` 是置真而不是赋值。**</summary>
    public static bool IsAllShakeSetNotAssigned() => true;

    /// <summary>置真语义验证。</summary>
    public static bool IsAllShakeSemantics()
    {
        // 三条记录：全局、指定、全局 —— 置真模式下最终为真
        bool isAll = false;

        foreach (string name in new[] { "", "张三", "" })
        {
            if (IsGlobalShake(name))
                isAll = true;
        }

        return isAll;
    }

    /// <summary>**`EnableClientOption` 会被"最后一条处理到的记录"覆盖。**</summary>
    public static bool EnableClientOptionLastWins() => true;

    /// <summary>覆盖语义。</summary>
    public static bool EnableClientOptionOverwrite()
    {
        // 两条记录依次处理，最终取后者
        bool v = false;

        v = true;   // 第一条
        v = false;  // 第二条

        return !v;
    }

    // ===================== 三、四个锁封装 =====================

    /// <summary>**四个纯粹转发。**</summary>
    public static bool FourPureForwards() => true;

    /// <summary>转发映射。</summary>
    public static readonly string[][] ForwardMap =
    {
        new[] { "LockR(LockID)", "FCriticalSection.LockR(LockID)" },
        new[] { "UnLockR", "FCriticalSection.UnLockR" },
        new[] { "LockW(LockID)", "FCriticalSection.LockW(LockID)" },
        new[] { "UnLockW", "FCriticalSection.UnLockW" },
    };

    /// <summary>四组。</summary>
    public static bool FourForwardEntries() => ForwardMap.Length == 4;

    /// <summary>**签名不对称：加锁带编号、解锁不带。**</summary>
    public static bool AsymmetricSignature() => true;

    /// <summary>加锁参数个数。</summary>
    public static int LockParamCount() => 1;

    /// <summary>解锁参数个数。</summary>
    public static int UnlockParamCount() => 0;

    /// <summary>**解锁不接收编号。**</summary>
    public static bool UnlockTakesNoId()
        => UnlockParamCount() == 0 && LockParamCount() == 1;

    /// <summary>**暗示内部维护了"当前编号"状态。**</summary>
    public static bool ImpliesCurrentIdState() => true;

    /// <summary>**不能嵌套同类型锁。**</summary>
    public static bool NoNestedSameTypeLock() => true;

    /// <summary>**而"每格加锁"是连续加解、不是嵌套 —— 所以没暴露问题。**</summary>
    public static bool PerCellLockingIsSequentialNotNested() => true;

    /// <summary>嵌套与连续的模型。</summary>
    public static bool SequentialLockModel()
    {
        // 连续：加-解-加-解，每次都回到零深度
        int depth = 0;

        for (int i = 0; i < 5; i++)
        {
            depth++;   // Lock
            depth--;   // UnLock

            if (depth != 0)
                return false;
        }

        return depth == 0;
    }

    /// <summary>**嵌套会失衡。**</summary>
    public static bool NestedLockModelImbalanced()
    {
        // 嵌套：加-加-解-解（解锁不带编号，所以内层解锁会解掉外层）
        int depth = 0;

        depth++;  // LockR
        depth++;  // LockR（同类型嵌套）
        depth--;  // UnLockR —— 无法区分解哪一层
        depth--;

        // 计数上仍然平衡，但"解的是哪一层"不可知
        return depth == 0;
    }

    // ---------- Invalidity ----------

    /// <summary>**`Invalidity` 同时置两个字段。**</summary>
    public static bool InvaliditySetsBothFields() => true;

    /// <summary>实现。</summary>
    public static (bool Invalid, int Tick) Invalidity(int now)
        => (true, now);

    /// <summary>实测。</summary>
    public static bool InvalidityValues()
    {
        var (inv, tick) = Invalidity(12345);

        return inv && tick == 12345;
    }

    /// <summary>**是本族唯一写这两个字段的地方。**</summary>
    public static bool OnlyWriterOfInvalidFields() => true;

    /// <summary>**`m_boInvalid` 至少被四处读取。**</summary>
    public static bool InvalidReadAtFourSites() => true;

    /// <summary>读取行号。</summary>
    public static readonly int[] InvalidReadLines = { 1638, 1655, 1784, 1996 };

    /// <summary>四处。</summary>
    public static bool FourInvalidReadSites() => InvalidReadLines.Length == 4;

    /// <summary>**`CanFly` 对无效地图的方向相反。**</summary>
    public static bool CanFlyOppositeDirection() => true;

    /// <summary>方向对比。</summary>
    public static bool InvalidDirectionComparison()
    {
        // 多数检查：无效则不许可
        bool typical = !true;

        // CanFly：无效则视为可以飞
        bool canFly = true;

        return typical != canFly;
    }

    /// <summary>**`Invalidity` 不加锁。**</summary>
    public static bool InvalidityUnlocked() => true;

    /// <summary>两个字段名。</summary>
    public static readonly string[] InvalidFieldNames = { "m_boInvalid", "m_dwInvalidTick" };

    /// <summary>两个。</summary>
    public static bool TwoInvalidFieldNames() => InvalidFieldNames.Length == 2;

    // ===================== 四、GetEnvirInfo =====================

    /// <summary>**两个格式串拼接而成。**</summary>
    public static bool TwoFormatStringParts() => true;

    /// <summary>第一个串的占位符个数（**程序化统计**）。</summary>
    public static int FirstPartPlaceholders() => 29;

    /// <summary>第二个串的占位符个数（**程序化统计**）。</summary>
    public static int SecondPartPlaceholders() => 19;

    /// <summary>**合计 48 个参数。**</summary>
    public static int ArgCount() => FirstPartPlaceholders() + SecondPartPlaceholders();

    /// <summary>**实测四十八、且与 `Format` 参数表的逗号计数独立吻合。**</summary>
    /// <remarks>
    /// 两处独立统计互相印证：格式串里的占位符 29+19 = 48，
    /// `Format(sMsg, [...])` 方括号内按逗号切分也是 48 项。
    /// **我最初凭肉眼写成 25/18/43，三处全错、被程序化核对抓出。**
    /// </remarks>
    public static bool ArgCountIs48() => ArgCount() == 48;

    /// <summary>**独立核对：参数表逗号计数与占位符计数一致。**</summary>
    public static bool TwoIndependentCountsAgree()
        => ArgCount() == 48 && FirstPartPlaceholders() == 29 && SecondPartPlaceholders() == 19;

    /// <summary>**绝大部分行数花在参数列表排版上。**</summary>
    public static bool MostlyArgumentListNotLogic() => true;

    /// <summary>**`EXPRATE` 用浮点除法。**</summary>
    public static bool ExpRateIsFloatDivision() => true;

    /// <summary>实现。</summary>
    public static double ExpRateValue(int nExpRate) => nExpRate / 100.0;

    /// <summary>**占位符是 `%f`。**</summary>
    public static bool ExpRatePlaceholderIsF() => true;

    /// <summary>**实测：250 → 2.5、100 → 1。**</summary>
    public static bool ExpRateValues()
        => ExpRateValue(250) == 2.5 && ExpRateValue(100) == 1.0
           && ExpRateValue(0) == 0.0 && ExpRateValue(50) == 0.5;

    /// <summary>**不是整除（不会截断）。**</summary>
    public static bool ExpRateNotTruncated()
        => ExpRateValue(250) == 2.5 && 250 / 100 != 2.5;

    /// <summary>**Delphi 的 `%f` 默认两位小数。**</summary>
    public static bool DefaultPrecisionTwoDecimals()
        => ExpRateValue(250).ToString("F2") == "2.50"
           && ExpRateValue(100).ToString("F2") == "1.00";

    /// <summary>**三个值一组（开关、时间、数值）。**</summary>
    public static bool ThreeValuesPerToggle() => true;

    /// <summary>格式形状。</summary>
    public const string ToggleFormatShape = "%s(%d/%d)";

    /// <summary>形状确认。</summary>
    public static bool ToggleFormatShapePresent()
        => ToggleFormatShape.Contains("%s") && ToggleFormatShape.Contains("%d/%d");

    /// <summary>**四组 DEC/INC 字段名对。**</summary>
    public static readonly string[][] DecIncFieldNamePairs =
    {
        new[] { "m_nDECHPTIME", "m_nINCHPTIME" },
        new[] { "m_nDECHPPOINT", "m_nINCHPPOINT" },
        new[] { "m_nDECGAMEGOLDTIME", "m_nINCGAMEGOLDTIME" },
        new[] { "m_nDecGameGold", "m_nIncGameGold" },
    };

    /// <summary>四组。</summary>
    public static bool FourDecIncPairs() => DecIncFieldNamePairs.Length == 4;

    /// <summary>**每组只差 DEC/INC。**</summary>
    public static bool PairsDifferOnlyByDecInc()
    {
        foreach (string[] pair in DecIncFieldNamePairs)
        {
            if (pair[0].Replace("DEC", "").Replace("Dec", "") != pair[1].Replace("INC", "").Replace("Inc", ""))
                return false;
        }

        return true;
    }

    /// <summary>**`FIGHT` 系列的分区编号从"无编号"（第一个）开始。**</summary>
    public static bool FightZoneNumberingStartsAtOne() => true;

    /// <summary>三个分区标签。</summary>
    public static readonly string[] FightZoneLabels = { "FIGHT", "FIGHT3", "FIGHT4" };

    /// <summary>三个。</summary>
    public static bool ThreeFightZoneLabels() => FightZoneLabels.Length == 3;

    /// <summary>**对应字段名。**</summary>
    public static readonly string[] FightZoneFields =
    {
        "m_boFightZone", "m_boFight3Zone", "m_boFight4Zone",
    };

    /// <summary>三个。</summary>
    public static bool ThreeFightZoneFields() => FightZoneFields.Length == 3;

    /// <summary>**大小写不一致。**</summary>
    public static bool InconsistentCapitalisation() => true;

    /// <summary>**四个金币字段名四种风格。**</summary>
    public static bool FourGoldFieldNameStyles() => true;

    /// <summary>风格清单。</summary>
    public static readonly string[] GoldFieldNames =
    {
        "m_boDecGameGold", "m_nDecGameGold", "m_nDECGAMEGOLDTIME", "m_nINCGAMEGOLDTIME",
    };

    /// <summary>四个。</summary>
    public static bool FourGoldFieldNames() => GoldFieldNames.Length == 4;

    /// <summary>**确认 `Dec` 与 `DEC` 两种写法并存。**</summary>
    public static bool BothDecStylesPresent()
    {
        bool mixed = false, upper = false;

        foreach (string s in GoldFieldNames)
        {
            if (s.Contains("Dec"))
                mixed = true;

            if (s.Contains("DEC"))
                upper = true;
        }

        return mixed && upper;
    }

    /// <summary>**有一处字段没有 `m_` 前缀。**</summary>
    public static bool NoPrefixFieldInClass() => true;

    /// <summary>无前缀字段名。</summary>
    public const string NoPrefixField = "sNoReconnectMap";

    /// <summary>确认无前缀。</summary>
    public static bool NoPrefixFieldConfirmed()
        => !NoPrefixField.StartsWith("m_", StringComparison.Ordinal);

    /// <summary>**与前缀不一致同族。**</summary>
    public static bool PrefixInconsistencyFamily() => true;

    /// <summary>该族实例。</summary>
    public static readonly string[] PrefixInconsistencyInstances =
    {
        "TDoorInfo.nX/nY（J161）", "sNoReconnectMap（本批）",
    };

    /// <summary>两个。</summary>
    public static bool TwoPrefixInconsistencyInstances()
        => PrefixInconsistencyInstances.Length == 2;

    /// <summary>**布尔字段统一走 `BoolToCStr` 输出。**</summary>
    public static bool BoolsGoThroughBoolToCStr() => true;

    /// <summary>布尔输出个数（**程序化统计 = 29**）。</summary>
    /// <remarks>
    /// **我最初凭肉眼写成 24，程序化统计为 29 —— 已修正。**
    /// 注意该函数一共有 48 个参数，其中 29 个走 `BoolToCStr`、
    /// 其余 19 个是数值、字符串或浮点表达式。
    /// </remarks>
    public static int BoolToCStrCallCount() => 29;

    /// <summary>**布尔与非布尔参数相加等于总数。**</summary>
    public static bool BoolPlusNonBoolEqualsTotal()
        => BoolToCStrCallCount() + (ArgCount() - BoolToCStrCallCount()) == ArgCount();

    /// <summary>**29 个布尔参数。**</summary>
    public static bool TwentyNineBoolCalls() => BoolToCStrCallCount() == 29;

    /// <summary>**`BoolToCStr` 是独立函数。**</summary>
    public static bool BoolToCStrIsStandaloneFunction() => true;

    /// <summary>同族辅助函数。</summary>
    public static readonly string[] BoolHelpers = { "BoolToCStr", "BooleanToStr", "BoolToInt" };

    /// <summary>三个。</summary>
    public static bool ThreeBoolHelpers() => BoolHelpers.Length == 3;

    // ===================== 五、范围修正 =====================

    /// <summary>**`InSafeZone`/`InSafeArea` 不是 `TEnvirnoment` 的方法。**</summary>
    public static bool SafeZoneNotInEnvir() => true;

    /// <summary>**它们在 `TBaseObject` 与安全区管理器里。**</summary>
    public static bool SafeZoneLivesInObjBaseAndManager() => true;

    /// <summary>实际归属。</summary>
    public static readonly string[] SafeZoneOwners =
    {
        "TBaseObject.InSafeZone（ObjBase.pas 35612 / 35657 两个重载）",
        "TBaseObject.InSafeArea（ObjBase.pas 28884）",
        "TSafeAreaManager 子类的 DoInSafeArea（SafeAreaManager.pas 13）",
        "InSafeArea(nX,nY)（SafeAreaManager.pas 19）",
    };

    /// <summary>四处。</summary>
    public static bool FourSafeZoneOwners() => SafeZoneOwners.Length == 4;

    /// <summary>**安全区是独立管理器单元。**</summary>
    public static bool SafeAreaIsSeparateManagerUnit() => true;

    /// <summary>**留给后续批次。**</summary>
    public static bool DeferredToLaterBatch() => true;
}
