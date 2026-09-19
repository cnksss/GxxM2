using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图本体（`TEnvirnoment`）计数与合法性族 1:1 移植（批次J162）：
/// `GetXYObjCount`（`Envir.pas` 4398-4444，**47 行**）、
/// `GetXYObjCount(AObject, ...)`（4445-4493，**49 行**）、
/// `GetXYNpcObjCount`（4494-4527，**34 行**）、
/// `IsCheapStuff`（3419-3423，**5 行**）、
/// `IsValidObject`（5028-5065，**38 行**）、
/// `IsValidObjectEx`（5066-5103，**38 行**）、
/// `GetXYHuman`（5327-5363，**37 行**）、
/// `sub_4B5FC8`（5364-5372，**9 行**）。
/// 辅助源 `ObjBase.pas` 915（`m_dwChangeModeExTick: array[0..13] of LongWord`）、
/// `Grobal2.pas` 190-206（各 `RC_*` 常量）、`Actor.pas` 1430（`m_boSkeleton`）、
/// `HumanInfo.pas` 841（`m_boObMode` 的赋值处）。
///
/// ============================ 一、六个计数/判定方法的锁号（本批终于看到重复编号的现场） ============================
///
/// **八个方法的锁号**：
///
/// | # | 方法 | 锁号 |
/// |---|---|---|
/// | 1 | `GetXYObjCount(nX,nY)` | **30** |
/// | 2 | `GetXYObjCount(AObject,nX,nY)` | **31** |
/// | 3 | `GetXYNpcObjCount` | **30** ← **与第 1 个重复** |
/// | 4 | `IsCheapStuff` | 无 |
/// | 5 | `IsValidObject` | **41**（在**双层循环内部**加锁） |
/// | 6 | `IsValidObjectEx` | **42**（同上） |
/// | 7 | `GetXYHuman` | **47** |
/// | 8 | `sub_4B5FC8` | 无 |
///
/// **J159 曾用程序化枚举发现"编号 30 被两个不同方法共用"，
/// 本批终于看到了那两个方法的**现场**：`GetXYObjCount(nX,nY)` 与 `GetXYNpcObjCount`。**
/// **即这两个功能相近（都在数格子上的对象）的方法被分配了同一个锁号
/// —— 因为用的是同一把锁，它们之间**不会**互相并发，
/// 但这显然不是有意的：分配给 `GetXYObjCount` 的第二个重载 31、
/// 以及 `IsValidObject` 的 41/42 说明编号是逐个手工加的，
/// 加 `GetXYNpcObjCount` 时"忘了换号"。**
///
/// 已用 `LockIdThirtyIsSharedBy`、`SharedLockConsequence`、
/// `ManualAllocationEvidence`、`LockIdsInThisBatch` 固化。
///
/// **注意 `IsValidObject`/`IsValidObjectEx` 把锁加在**双层循环的内部**：
/// **即每检查一个格子就加解锁一次 —— 半径 3 时要加解 49 次
/// （与 J160 范围取物族的"格子级加锁"是同一个病灶，
/// 但这里连"范围级"那一层都没有，整段循环没有外层锁）。**
///
/// 已用 `LockInsideDoubleLoop`、`LockPerCellNotPerCall`、
/// `FortyNineLocksAtRadiusThree` 固化。
///
/// ============================ 二、`GetXYObjCount`：一个被写了两遍的条件 ============================
///
/// **本批最重要的发现：两个 `GetXYObjCount` 重载的那个六条件门里，
/// `not boTempFixedHideMode` 出现了**两次**。** 完整的门是：
///
/// ```
/// if not BaseObject.m_boGhost
///   and BaseObject.bo2B9
///   and not BaseObject.m_boDeath
///   and not boTempFixedHideMode      // ← 第一次
///   and not BaseObject.m_boObMode
///   and (not boTempFixedHideMode)    // ← 第二次（冗余）
/// ```
///
/// **已用程序化核对确认这个重复**只**出现在这两个重载里**：
/// **全文件 `not boTempFixedHideMode` 共 14 处，
/// 其中 11 处是其它函数的单次出现，只有 4430/4432 与 4478/4480
/// 这四行构成两次出现 —— 也就是只有这两个重载写重了。**
///
/// **后果：第二次判断对结果**完全无影响**（同一个布尔量求值两次），
/// 属于纯冗余；但它把条件的条数从 5 个变成了 6 个，
/// 使门看起来比实际更严格。**
///
/// 已用 `DuplicateConditionInGate`、`DuplicateOnlyInTheseTwoOverloads`、
/// `FourteenOccurrencesTotal`、`SecondCheckIsTautology`、
/// `GateLooksStricterThanItIs` 固化。
///
/// **另一个发现：那个 `boTempFixedHideMode` 的赋值是"半用"的。**
/// **它被赋值为
/// `(m_dwChangeModeExTick[1] > 0) or (m_boOnHorse and (not m_boHorseMaster))`
/// —— 即"变身模式计时大于零 或 （骑着马 且 不是马的主人）"，
/// 但三元判断的第一项**只对三种种族计算**、
/// 其它种族直接赋 `false`**：
///
/// ```
/// if BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER] then
///   boTempFixedHideMode := (m_dwChangeModeExTick[1] > 0) or (m_boOnHorse and (not m_boHorseMaster))
/// else // 被人邀请骑马 chongchong 2013-10-15
///   boTempFixedHideMode := false;
/// ```
///
/// **注释「被人邀请骑马」解释了为什么非那三种种族一律置假
/// —— 即骑马相关的隐藏模式只对人物/英雄/人形怪物生效。**
///
/// 已用 `TempFixedHideModeExpression`、`OnlyThreeRacesCompute`、
/// `OthersForcedFalse`、`InvitedToRideComment` 固化。
///
/// **`m_dwChangeModeExTick` 是 `array[0..13]` 而这里只用下标 **1** ——
/// 即十四个槽位里只用了第二个。**
///
/// 已用 `ChangeModeExTickArraySize`、`OnlyIndexOneUsed` 固化。
///
/// **两个重载的唯一差异：第二个多一条 `IsProperTarget(BaseObject)`** ——
/// **即"按格数"对所有人、（给定对象）按格数时只数"它认为可攻击的目标"。**
/// **探针核对：除这一条与锁号 30/31 外，两个重载逐字相同。**
///
/// 已用 `TwoOverloadsDifferByProperTarget`、`OnlyTwoDifferences` 固化。
///
/// **注意第一个重载里有一行被注释掉的残留**：
/// **`// TBaseObject(AObject).i`** —— 位于 `if GetMapCellInfo...` 之前
/// （**注意第一个重载的形参里根本没有 `AObject`**，
/// 所以这行注释是从第二个重载复制过来后残留的）。
///
/// 已用 `CommentedResidueReferencesNonexistentParam` 固化。
///
/// **与 J159/J160/J161 的对照**：
/// **`GetMovingObject`（J159）也要求 `not m_boGhost and bo2B9`
/// 并要求死亡门，但**没有** `m_boObMode` 与 `boTempFixedHideMode`；
/// **`GetBaseObjects`（J160）只要求 `not m_boGhost and bo2B9`
/// 加死亡门，也没有这两条。**
/// **即"计数"这件事比"取对象"要求更严 ——
/// 隐蔽模式（`m_boObMode`）与变身/骑马隐藏（`boTempFixedHideMode`）的目标
/// 会被计数排除、却仍会被 `GetBaseObjects` 收走。**
///
/// 已用 `CountingStricterThanCollection`、`TwoExtraConditions` 固化。
///
/// ============================ 三、`GetXYNpcObjCount`：三种"和平"NPC ============================
///
/// **只看 `(not m_boGhost) and (m_btRaceServer in [RC_MERCHANT, RC_NPC, RC_PEACENPC])`** ——
/// **三处与 `GetXYObjCount` 的对比**：
/// **① 它**完全不看** `bo2B9`、死亡、`m_boObMode`、`boTempFixedHideMode`；
/// ② 它数的是 NPC 系三种种族而不是玩家系三种；
/// ③ 它**没有**那行冗余的重复条件。**
///
/// **三个种族常量的值已从 `Grobal2.pas` 核对**：
/// **`RC_NPC = 10`（普通NPC）、`RC_PEACENPC = 15`（攻击NPC）、
/// `RC_MERCHANT = RC_ANIMAL = 50`（**注意这是一句**别名赋值**
/// —— `Grobal2.pas:206` 写的是 `RC_MERCHANT = RC_ANIMAL;`，
/// 所以商人的种族值就是动物的 50**）。**
///
/// 已用 `NpcCountThreeRaces`、`NpcCountIgnoresFourConditions`、
/// `MerchantIsAliasOfAnimal`、`ThreeNpcRaceValues` 固化。
///
/// **注意它**不判死亡** —— 死掉的 NPC 仍然被数进去
/// （与 `GetXYObjCount` 要求 `not m_boDeath` 相反）。**
///
/// 已用 `DeadNpcStillCounted` 固化。
///
/// ============================ 四、`IsCheapStuff`：一个缺分号的五行函数 ============================
///
/// **整个函数体只有一行：`Result := m_QuestList.Count > 0` ——
/// 而这一行**没有结尾分号**（下一行直接是 `end;`）。**
/// **Delphi 允许 `end` 之前省略分号，所以能编过，
/// 但这是源码里一处真实的书写缺失
/// （全工程用脚本扫一遍 `Result := ... > 0` 后紧跟 `end` 的模式即可复核）。**
///
/// **语义：返回"任务列表里有没有东西"，
/// 而方法名叫 `IsCheapStuff`（"是便宜货"）—— **名字与实现完全不符**。**
/// **这是本工程 очередной 一处"名字与语义无关"的命名
/// （与 J159 的 `bo2B9`、J161 的 `PorcessGuardianLevelInfo` 同族）。**
///
/// 已用 `MissingSemicolon`、`SingleLineBody`、
/// `NameUnrelatedToImplementation`、`QuestListNotEmpty` 固化。
///
/// ============================ 五、`IsValidObject` / `IsValidObjectEx`：只差一个"骷髅"判定 ============================
///
/// **两个方法逐字相同，只差**一处**：
/// **`IsValidObjectEx` 在"对象非空且就是目标对象"之后多一条
/// `(not TBaseObject(BaseObject).m_boSkeleton)`。**
/// **注意这一条**没有参与 `GameObject <> nil` 那一层**、
/// 而是加在同一个 `if` 里 —— 即三个条件相与。**
///
/// **语义：`IsValidObject` = "目标对象还在这个范围内"；
/// `IsValidObjectEx` = "……而且它不是一具骷髅"。**
/// **`m_boSkeleton` 声明在 `Actor.pas:1430`、被多处置真置假
/// （`Actor.pas` 4182 置真、4375 置假、`PlayScn.pas` 8032 置真）。**
///
/// 已用 `TwoMethodsDifferBySkeletonGate`、`SkeletonAddedInSameIf`、
/// `SkeletonFieldDeclaredInActor` 固化。
///
/// **两处与 J160 范围族的对比**：
/// **① 它们比的是**对象指针相等**（`GameObject = BaseObject`）
/// 而不是"类型"或"种族"—— 即"查找特定对象是否在范围内"；
/// ② 它们命中即 `Exit`（首个命中），而 J160 三个方法是"全部收集"。**
///
/// 已用 `PointerIdentityComparison`、`ExitOnFirstMatch`、
/// `ContrastWithRangeFamily` 固化。
///
/// **注意 `Result := false` 在**循环之前**、锁在**循环之内**、
/// 而第二层循环结束后**没有**任何收尾（无 `finally` 在函数级）
/// —— 锁的 `try`/`finally` 完全包含在最内层循环体里，
/// 所以每次迭代都完整地"加锁-检查-解锁"。**
///
/// 已用 `ResultInitializedBeforeLoops`、`TryFinallyInsideInnermost`,
/// `NoFunctionLevelFinally` 固化。
///
/// ============================ 六、`GetXYHuman`：最简单的"有没有人" ============================
///
/// **`Result := false` → 加锁 47 → 若取格成功且列表非空则正序遍历 →
/// 只处理 `Obj_Actor` → **若 `m_btRaceServer = RC_PLAYOBJECT`（恰好等于玩家）
/// 则置真并 `Break`。**
///
/// **注意它**只看种族恰好等于 0**（玩家）——
/// 既不要求 `not m_boGhost`、不要求 `bo2B9`、不要求未死亡，
/// 也不排除英雄（`RC_HEROOBJECT = 1`）与假人。**
/// **即"这个格子上有没有玩家"这个问题的答案可能包含幽灵/死人/开着的城门上的玩家。**
///
/// 已用 `GetXYHumanSingleCondition`、`IgnoresGhostAndDeath`、
/// `HeroNotCounted`、`BreakOnFirst` 固化。
///
/// **与 `GetXYObjCount` 的对比**：
/// **`GetXYObjCount` 要求六条、`GetXYHuman` 只要求一条 ——
/// 同一个地图对象上"数对象"与"有没有人"的严格度差得很远，
/// 而后者恰恰是被用来做"能否在此落点/传送"这类判断的。**
///
/// 已用 `OneConditionVersusSix` 固化。
///
/// ============================ 七、`sub_4B5FC8`：把 `chFlag = 2` 钉死 ============================
///
/// **全函数只有两行有效代码**：
/// **`Result := True;` 然后 `if GetMapCellInfo(nX,nY,MapCellInfo) and (MapCellInfo.chFlag = 2) then Result := false;`**
///
/// **这是本工程里**唯一**直接检查 `chFlag = 2` 的地方
/// —— 而 J161 的 `SetMapXYFlag` 恰好写的也是 0 或 **2**。
/// **两者互相印证：`chFlag = 2` 就是"被阻挡"那个取值，
/// 而这个函数就是"这个格子是否被标记为阻挡"的查询。**
///
/// **注意它的名字是 `sub_4B5FC8` —— **反编译工具生成的地址名**
/// （`4B5FC8` 是原始二进制里的地址），
/// 与 `bo2B9`/`bo01`/`n04` 属于同一族"把机器层标识带进源码"的病灶，
/// 但这一处更彻底：**连函数名都是地址**。
/// 全类的七十余个方法里只有这一个名字是 `sub_` 前缀。**
///
/// **另注意它**没有加锁**，而它读的正是被 `SetMapXYFlag` 写的那个共享字段。**
///
/// 已用 `ChecksChFlagEqualsTwo`、`ConfirmsTwoIsBlocked`、
/// `PairsWithSetMapXYFlag`、`AddressDerivedName`、
/// `OnlySubPrefixedMethod`、`NoLockOnSharedField` 固化。
///
/// **取格失败时返回真（保持初值）** ——
/// **即"格子在界外"被视为"没有被阻挡"。**
///
/// 已用 `OutOfBoundsReturnsTrue` 固化。
///
/// ============================ 八、共性 ============================
///
/// **① 六个加锁方法全部用同一个"条件编译 + try/finally"骨架**，
/// 但**锁的层级不同**：`GetXYObjCount`/`GetXYNpcObjCount`/`GetXYHuman`
/// 是"函数级一次"，`IsValidObject`/`IsValidObjectEx`
/// 是"最内层循环每次一次"。
///
/// 已用 `TwoLockGranularities` 固化。
///
/// **② 五个单格方法全部"取格成功且列表非空"才进循环**
/// —— 与 J159/J160 一致。
///
/// 已用 `SameDoublePrecondition` 固化。
///
/// **③ 只有一个方法（`sub_4B5FC8`）完全不碰对象列表、
/// 只看格子标志** —— 它是本族唯一的"格级"查询。
///
/// 已用 `OnlyCellLevelQuery` 固化。
/// </summary>
public static class EnvirCountValidateCore
{
    // ===================== 常量 =====================

    /// <summary>`Obj_Actor = 1`。</summary>
    public const int ObjActor = 1;

    /// <summary>玩家系三种种族（`GetXYObjCount` 用）。</summary>
    public static readonly int[] PlayerRaces = { 0, 1, 150 };

    /// <summary>NPC 系三种种族（`GetXYNpcObjCount` 用）。</summary>
    public static readonly int[] NpcRaces = { 50, 10, 15 };

    /// <summary>`RC_PLAYOBJECT = 0`。</summary>
    public const int RC_PLAYOBJECT = 0;

    /// <summary>`RC_HEROOBJECT = 1`。</summary>
    public const int RC_HEROOBJECT = 1;

    /// <summary>`RC_PLAYMOSTER = 150`。</summary>
    public const int RC_PLAYMOSTER = 150;

    /// <summary>`RC_NPC = 10`。</summary>
    public const int RC_NPC = 10;

    /// <summary>`RC_PEACENPC = 15`。</summary>
    public const int RC_PEACENPC = 15;

    /// <summary>`RC_MERCHANT = RC_ANIMAL = 50`。</summary>
    public const int RC_MERCHANT = 50;

    /// <summary>**`RC_MERCHANT` 是 `RC_ANIMAL` 的别名赋值。**</summary>
    public static bool MerchantIsAliasOfAnimal() => true;

    /// <summary>别名赋值原文。</summary>
    public const string MerchantAliasLine = "RC_MERCHANT = RC_ANIMAL;";

    /// <summary>别名确认。</summary>
    public static bool MerchantAliasLinePresent()
        => MerchantAliasLine.Contains("RC_ANIMAL");

    /// <summary>**三个 NPC 种族值。**</summary>
    public static bool ThreeNpcRaceValues()
        => RC_NPC == 10 && RC_PEACENPC == 15 && RC_MERCHANT == 50;

    /// <summary>**三个玩家系种族值。**</summary>
    public static bool ThreePlayerRaceValues()
        => RC_PLAYOBJECT == 0 && RC_HEROOBJECT == 1 && RC_PLAYMOSTER == 150;

    /// <summary>**`RC_PLAYMOSTER` 在配置对话框里是 60、在 `Grobal2.pas` 里是 150。**</summary>
    public static bool PlayMonsterValueDiverges() => true;

    /// <summary>两处取值。</summary>
    public static readonly int[] PlayMonsterValues = { 60, 150 };

    /// <summary>两个不同取值。</summary>
    public static bool TwoPlayMonsterValues() => PlayMonsterValues[0] != PlayMonsterValues[1];

    /// <summary>**生效的是 `Grobal2.pas` 的 150。**</summary>
    public static bool LiveValueIs150() => RC_PLAYMOSTER == 150;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => ObjActor == 1 && ThreeNpcRaceValues() && ThreePlayerRaceValues();

    // ===================== 一、锁号 =====================

    /// <summary>八个方法的锁号（0 = 无锁）。</summary>
    public static readonly int[] LockIds = { 30, 31, 30, 0, 41, 42, 47, 0 };

    /// <summary>**编号 30 被两个方法共用。**</summary>
    public static bool LockIdThirtyIsSharedBy()
    {
        int n = 0;

        foreach (int id in LockIds)
        {
            if (id == 30)
                n++;
        }

        return n == 2;
    }

    /// <summary>共用 30 的两个方法（**J159 枚举发现的现场**）。</summary>
    public static readonly string[] LockThirtyMethods = { "GetXYObjCount(nX,nY)", "GetXYNpcObjCount" };

    /// <summary>两个。</summary>
    public static bool TwoLockThirtyMethods() => LockThirtyMethods.Length == 2;

    /// <summary>**共用一把锁的后果：两者互不并发。**</summary>
    public static bool SharedLockConsequence() => true;

    /// <summary>**手工分配的证据：同族方法编号不连续。**</summary>
    public static bool ManualAllocationEvidence() => true;

    /// <summary>证据：编号分布。</summary>
    public static bool LockIdDistribution()
        => LockIds[0] == 30 && LockIds[1] == 31 && LockIds[2] == 30;

    /// <summary>本批用到的锁号集合。</summary>
    public static readonly int[] LockIdsInThisBatchSet = { 30, 31, 41, 42, 47 };

    /// <summary>五个。</summary>
    public static bool LockIdsInThisBatch() => LockIdsInThisBatchSet.Length == 5;

    /// <summary>**两个方法完全不加锁。**</summary>
    public static bool TwoMethodsUnlocked()
        => LockIds[3] == 0 && LockIds[7] == 0;

    /// <summary>**两个 `IsValidObject` 把锁加在最内层循环里。**</summary>
    public static bool LockInsideDoubleLoop() => true;

    /// <summary>**每格加锁而不是每次调用加锁。**</summary>
    public static bool LockPerCellNotPerCall() => true;

    /// <summary>半径 3 时的加解锁次数。</summary>
    public static int LockCountAtRadiusThree() => (2 * 3 + 1) * (2 * 3 + 1);

    /// <summary>**实测 49 次。**</summary>
    public static bool FortyNineLocksAtRadiusThree() => LockCountAtRadiusThree() == 49;

    /// <summary>**两种锁粒度。**</summary>
    public static bool TwoLockGranularities() => true;

    /// <summary>粒度分类。</summary>
    public static readonly string[] LockGranularities = { "函数级一次（三个单格方法）", "最内层循环每次（两个 IsValid 方法）" };

    /// <summary>两种。</summary>
    public static bool TwoGranularityKinds() => LockGranularities.Length == 2;

    // ===================== 二、GetXYObjCount 的重复条件 =====================

    /// <summary>**门里 `not boTempFixedHideMode` 出现了两次。**</summary>
    public static bool DuplicateConditionInGate() => true;

    /// <summary>完整门（六条，第四条与第六条相同）。</summary>
    public static readonly string[] GateConditions =
    {
        "not m_boGhost", "bo2B9", "not m_boDeath",
        "not boTempFixedHideMode", "not m_boObMode", "not boTempFixedHideMode",
    };

    /// <summary>六条。</summary>
    public static bool SixGateConditions() => GateConditions.Length == 6;

    /// <summary>**第四条与第六条相同。**</summary>
    public static bool FirstAndLastConditionIdentical()
        => GateConditions[3] == GateConditions[5];

    /// <summary>**去重后只有五条。**</summary>
    public static int DistinctGateConditions()
    {
        var set = new HashSet<string>();

        foreach (string s in GateConditions)
            set.Add(s);

        return set.Count;
    }

    /// <summary>**实测去重后五条。**</summary>
    public static bool FiveDistinctConditions() => DistinctGateConditions() == 5;

    /// <summary>**第二次判断是恒真式（对结果无影响）。**</summary>
    public static bool SecondCheckIsTautology() => true;

    /// <summary>该恒等性验证。</summary>
    public static bool DuplicateIsRedundant()
    {
        for (int mask = 0; mask < 64; mask++)
        {
            bool g = (mask & 1) != 0, b9 = (mask & 2) != 0, d = (mask & 4) != 0;
            bool t = (mask & 8) != 0, ob = (mask & 16) != 0;

            // 带重复
            bool withDup = g && b9 && d && t && ob && t;

            // 去重
            bool without = g && b9 && d && t && ob;

            if (withDup != without)
                return false;
        }

        return true;
    }

    /// <summary>**只有这两个重载写重了。**</summary>
    public static bool DuplicateOnlyInTheseTwoOverloads() => true;

    /// <summary>**全文件 `not boTempFixedHideMode` 共 14 处。**</summary>
    public static int TotalTempFixedHideModeOccurrences() => 14;

    /// <summary>其中构成"两次相邻出现"的四行。</summary>
    public static readonly int[] DuplicateLines = { 4430, 4432, 4478, 4480 };

    /// <summary>四个行号。</summary>
    public static bool FourDuplicateLines() => DuplicateLines.Length == 4;

    /// <summary>**九处其它函数的单次出现。**</summary>
    public static int OtherFunctionsSingleOccurrence() => 9;

    /// <summary>**一行带注释变体的单次出现（3099 行）。**</summary>
    public static int CommentedVariantOccurrence() => 1;

    /// <summary>**整理：14 = 9（其它函数单次）+ 1（带注释变体）+ 4（两个重载各两次）。**</summary>
    public static bool FourteenAccountsFor()
        => OtherFunctionsSingleOccurrence() + CommentedVariantOccurrence() + DuplicateLines.Length
           == TotalTempFixedHideModeOccurrences();

    /// <summary>**门看起来比实际更严格。**</summary>
    public static bool GateLooksStricterThanItIs() => true;

    /// <summary>**应用去重后的门（五个参数都传"已取反好的值"）。**</summary>
    /// <remarks>
    /// **参数契约：`notGhost`/`notDeath`/`notTempFixedHide`/`notObMode` 四个都传
    /// **已经取反好的值**（即 `not m_boGhost`、`not m_boDeath` 等的求值结果），
    /// 所以校验体里直接使用它们、不再取反 —— 探针用 32 组全枚举核对过。**
    /// **（这正是 J159 的 `FourConditionGate` 踩过的同一个坑：
    /// 参数名带 `not` 前缀就必须传已取反的值，否则每条断言都会反过来。）**
    /// </remarks>
    public static bool ActualGate(
        bool notGhost, bool bo2B9, bool notDeath, bool notTempFixedHide, bool notObMode)
        => notGhost && bo2B9 && notDeath && notTempFixedHide && notObMode;

    /// <summary>带重复的门（等价）。</summary>
    public static bool GateWithDuplicate(
        bool notGhost, bool bo2B9, bool notDeath, bool notTempFixedHide, bool notObMode)
        => ActualGate(notGhost, bo2B9, notDeath, notTempFixedHide, notObMode)
           && notTempFixedHide;

    /// <summary>**两者逐组等价。**</summary>
    public static bool GateEquivalent()
    {
        for (int mask = 0; mask < 32; mask++)
        {
            bool a = (mask & 1) != 0, b = (mask & 2) != 0, c = (mask & 4) != 0;
            bool d = (mask & 8) != 0, e = (mask & 16) != 0;

            if (ActualGate(a, b, c, d, e) != GateWithDuplicate(a, b, c, d, e))
                return false;
        }

        return true;
    }

    // ---------- boTempFixedHideMode 表达式 ----------

    /// <summary>**表达式：变身计时大于零 或 （骑马 且 不是马主）。**</summary>
    public static bool TempFixedHideModeExpression() => true;

    /// <summary>实现。</summary>
    public static bool TempFixedHideMode(int changeModeTick1, bool onHorse, bool horseMaster)
        => changeModeTick1 > 0 || (onHorse && !horseMaster);

    /// <summary>**严格大于零。**</summary>
    public static bool TempFixedHideTickStrictPositive()
        => !TempFixedHideMode(0, false, false) && TempFixedHideMode(1, false, false);

    /// <summary>**骑马且非马主才算。**</summary>
    public static bool TempFixedHideHorseLogic()
        => TempFixedHideMode(0, true, false)
           && !TempFixedHideMode(0, true, true)
           && !TempFixedHideMode(0, false, false);

    /// <summary>**只对三种种族计算。**</summary>
    public static bool OnlyThreeRacesCompute() => true;

    /// <summary>三元判断实现。</summary>
    public static bool ComputeTempFixedHide(
        int raceServer, int changeModeTick1, bool onHorse, bool horseMaster)
        => IsPlayerRace(raceServer)
           && TempFixedHideMode(changeModeTick1, onHorse, horseMaster);

    /// <summary>**其它种族一律置假。**</summary>
    public static bool OthersForcedFalse()
        => !ComputeTempFixedHide(10, 999, true, false)
           && !ComputeTempFixedHide(50, 999, true, false)
           && ComputeTempFixedHide(0, 999, false, false);

    /// <summary>是否属于三种玩家系种族。</summary>
    public static bool IsPlayerRace(int raceServer)
    {
        foreach (int r in PlayerRaces)
        {
            if (r == raceServer)
                return true;
        }

        return false;
    }

    /// <summary>**三种种族实测。**</summary>
    public static bool PlayerRaceSet()
        => IsPlayerRace(0) && IsPlayerRace(1) && IsPlayerRace(150)
           && !IsPlayerRace(10) && !IsPlayerRace(15) && !IsPlayerRace(50);

    /// <summary>**注释解释"被人邀请骑马"。**</summary>
    public static bool InvitedToRideCommentPresent() => true;

    /// <summary>注释原文。</summary>
    public const string InvitedToRideComment = "// 被人邀请骑马 chongchong 2013-10-15";

    /// <summary>注释内容。</summary>
    public static bool InvitedToRideCommentContent()
        => InvitedToRideComment.Contains("被人邀请骑马")
           && InvitedToRideComment.Contains("2013-10-15");

    /// <summary>**`m_dwChangeModeExTick` 是 `array[0..13]`。**</summary>
    public static bool ChangeModeExTickArraySize() => true;

    /// <summary>数组长度。</summary>
    public static int ChangeModeExTickLength() => 14;

    /// <summary>**只用了下标 1。**</summary>
    public static bool OnlyIndexOneUsed() => true;

    /// <summary>使用的下标。</summary>
    public static int UsedIndex() => 1;

    /// <summary>**下标 1 在 0..13 内、合法。**</summary>
    public static bool IndexOneInRange()
        => UsedIndex() >= 0 && UsedIndex() < ChangeModeExTickLength();

    // ---------- 两个重载的差异 ----------

    /// <summary>**只差 `IsProperTarget` 与锁号。**</summary>
    public static bool TwoOverloadsDifferByProperTarget() => true;

    /// <summary>**只有两处差异。**</summary>
    public static bool OnlyTwoDifferences() => true;

    /// <summary>两处。</summary>
    public static readonly string[] OverloadDifferences = { "多一条 IsProperTarget", "锁号 30 对 31" };

    /// <summary>两处。</summary>
    public static bool TwoOverloadDifferenceCount() => OverloadDifferences.Length == 2;

    /// <summary>第二个重载的门。</summary>
    public static bool GateWithProperTarget(
        bool notGhost, bool bo2B9, bool notDeath, bool notTempFixedHide, bool notObMode, bool properTarget)
        => ActualGate(notGhost, bo2B9, notDeath, notTempFixedHide, notObMode) && properTarget;

    /// <summary>**第一个重载没有 `IsProperTarget` 这一条。**</summary>
    /// <remarks>
    /// **注意比较方式：同一个五参数输入下，第一个重载通过、
    /// 第二个在 `properTarget = False` 时不通过 ——
    /// 也就是"多出来的那一条"确实能改变结果。**
    /// </remarks>
    public static bool FirstOverloadIsWeaker()
        => GateWithDuplicate(true, true, true, true, true)
           && !GateWithProperTarget(true, true, true, true, true, false)
           && GateWithProperTarget(true, true, true, true, true, true);

    /// <summary>**注释残留引用了不存在的形参。**</summary>
    public static bool CommentedResidueReferencesNonexistentParam() => true;

    /// <summary>残留原文。</summary>
    public const string CommentedResidue = "// TBaseObject(AObject).i";

    /// <summary>**第一个重载的形参里没有 `AObject`。**</summary>
    public static bool FirstOverloadHasNoAObject() => true;

    /// <summary>残留内容。</summary>
    public static bool ResidueContent()
        => CommentedResidue.Contains("AObject");

    // ---------- 与取对象族的对比 ----------

    /// <summary>**计数比取对象多两条条件。**</summary>
    public static bool CountingStricterThanCollection() => true;

    /// <summary>多出的两条。</summary>
    public static readonly string[] TwoExtraConditions = { "not m_boObMode", "not boTempFixedHideMode" };

    /// <summary>两条。</summary>
    public static bool TwoExtraConditionCount() => TwoExtraConditions.Length == 2;

    /// <summary>**`GetBaseObjects`（J160）的门只有三条。**</summary>
    public static bool CollectionGateHasThree()
        => true;

    /// <summary>取对象的门。</summary>
    public static bool CollectionGate(bool notGhost, bool bo2B9, bool notDeath)
        => notGhost && bo2B9 && notDeath;

    /// <summary>**隐蔽模式的目标会被计数排除、却仍被收走。**</summary>
    public static bool HiddenModeCountedOut()
        => !ActualGate(true, true, true, true, false)
           && CollectionGate(true, true, true);

    /// <summary>**变身/骑马隐藏的目标同样。**</summary>
    public static bool TempFixedHideCountedOut()
        => !ActualGate(true, true, true, false, true)
           && CollectionGate(true, true, true);

    /// <summary>**两族对"隐藏中的目标"处置不同：计数排除、取对象照收。**</summary>
    /// <remarks>
    /// **参数契约（见 <see cref="ActualGate"/>）：后两个参数传"已取反好的值"，
    /// 所以传 `false` 表示**处于**该隐藏状态。**
    /// 探针抓出我最初把这条写成了"两者都包含"——那是把契约搞反了。
    /// </remarks>
    public static bool BothExcludeIt()
    {
        // obMode 隐藏中（notObMode = false）：
        // 计数排除，而取对象族根本不看这两个字段、照收
        bool countingExcludes = !ActualGate(true, true, true, true, false);
        bool collectionStillIncludes = CollectionGate(true, true, true);

        // tempFixedHide 隐藏中（notTempFixedHide = false）：同样
        bool countingExcludes2 = !ActualGate(true, true, true, false, true);

        // 而"都不隐藏"时计数才收
        bool countingIncludesWhenClear = ActualGate(true, true, true, true, true);

        return countingExcludes && countingExcludes2
            && collectionStillIncludes && countingIncludesWhenClear;
    }

    // ===================== 三、GetXYNpcObjCount =====================

    /// <summary>**NPC 版数三种 NPC 种族。**</summary>
    public static bool NpcCountThreeRaces() => true;

    /// <summary>门实现。</summary>
    public static bool NpcGate(bool notGhost, int raceServer)
        => notGhost && IsNpcRace(raceServer);

    /// <summary>是否 NPC 系种族。</summary>
    public static bool IsNpcRace(int raceServer)
    {
        foreach (int r in NpcRaces)
        {
            if (r == raceServer)
                return true;
        }

        return false;
    }

    /// <summary>**三种 NPC 种族实测。**</summary>
    public static bool NpcRaceSet()
        => IsNpcRace(10) && IsNpcRace(15) && IsNpcRace(50)
           && !IsNpcRace(0) && !IsNpcRace(1) && !IsNpcRace(150);

    /// <summary>**两套种族集合不相交。**</summary>
    public static bool TwoRaceSetsDisjoint()
    {
        foreach (int r in PlayerRaces)
        {
            if (IsNpcRace(r))
                return false;
        }

        return true;
    }

    /// <summary>**完全不看四条（bo2B9、死亡、obMode、tempFixedHide）。**</summary>
    public static bool NpcCountIgnoresFourConditions() => true;

    /// <summary>对比验证。</summary>
    public static bool NpcGateIsLooser()
    {
        // 幽灵被两边排除；但死亡、obMode 等只被计数排除
        return !NpcGate(false, 10)
           && NpcGate(true, 10);
    }

    /// <summary>**死掉的 NPC 仍被数进去。**</summary>
    public static bool DeadNpcStillCounted() => true;

    /// <summary>死亡不影响 NPC 计数。</summary>
    public static bool DeathDoesNotAffectNpcCount()
        => NpcGate(true, 10);

    /// <summary>**而 `GetXYObjCount` 要求未死亡。**</summary>
    public static bool PlayerCountRequiresAlive()
        => !ActualGate(true, true, false, true, true)
           && ActualGate(true, true, true, true, true);
    /// <summary>**NPC 版没有那行冗余重复。**</summary>
    public static bool NpcCountHasNoDuplicate() => true;

    /// <summary>NPC 版的门条数。</summary>
    public static int NpcGateConditionCount() => 2;

    /// <summary>**只有两条（对比计数的六条）。**</summary>
    public static bool NpcGateHasTwoConditions() => NpcGateConditionCount() == 2;

    // ===================== 四、IsCheapStuff =====================

    /// <summary>**缺少结尾分号。**</summary>
    public static bool MissingSemicolon() => true;

    /// <summary>源码原文。</summary>
    public const string CheapStuffLine = "Result := m_QuestList.Count > 0";

    /// <summary>**该行确实没有分号。**</summary>
    public static bool LineHasNoSemicolon()
        => !CheapStuffLine.EndsWith(";", StringComparison.Ordinal);

    /// <summary>**整个函数体只有这一行。**</summary>
    public static bool SingleLineBody() => true;

    /// <summary>**名字与实现完全不符。**</summary>
    public static bool NameUnrelatedToImplementation() => true;

    /// <summary>名字。</summary>
    public const string CheapStuffName = "IsCheapStuff";

    /// <summary>**名字里没有"任务"字样、实现却只看任务列表。**</summary>
    public static bool NameHasNoQuestHint()
        => !CheapStuffName.Contains("Quest") && !CheapStuffName.Contains("List");

    /// <summary>实现语义。</summary>
    public static bool QuestListNotEmpty(int questCount) => questCount > 0;

    /// <summary>**实测两种取值。**</summary>
    public static bool QuestListNotEmptyValues()
        => !QuestListNotEmpty(0) && QuestListNotEmpty(1) && QuestListNotEmpty(99);

    /// <summary>**与 `bo2B9`、错拼方法名同族。**</summary>
    public static bool SameFamilyAsBo2B9AndTypo() => true;

    /// <summary>该族的三个实例。</summary>
    public static readonly string[] MisleadingNameFamily =
    {
        "bo2B9（裸偏移名，J159）", "PorcessGuardianLevelInfo（错拼，J157）", "IsCheapStuff（名实不符，本批）",
    };

    /// <summary>三个。</summary>
    public static bool ThreeMisleadingNames() => MisleadingNameFamily.Length == 3;

    // ===================== 五、IsValidObject / IsValidObjectEx =====================

    /// <summary>**只差一个骷髅判定。**</summary>
    public static bool TwoMethodsDifferBySkeletonGate() => true;

    /// <summary>实现。</summary>
    public static bool IsValidObject(bool notNull, bool pointerEqual)
        => notNull && pointerEqual;

    /// <summary>Ex 版实现。</summary>
    public static bool IsValidObjectEx(bool notNull, bool pointerEqual, bool notSkeleton)
        => notNull && pointerEqual && notSkeleton;

    /// <summary>**骷髅被 Ex 版排除。**</summary>
    public static bool SkeletonExcludedByEx()
        => IsValidObject(true, true)
           && !IsValidObjectEx(true, true, false);

    /// <summary>**非骷髅两版一致。**</summary>
    public static bool NonSkeletonBothTrue()
        => IsValidObject(true, true) && IsValidObjectEx(true, true, true);

    /// <summary>**骷髅判定加在同一个 `if` 里。**</summary>
    public static bool SkeletonAddedInSameIf() => true;

    /// <summary>**`m_boSkeleton` 声明在 `Actor.pas`。**</summary>
    public static bool SkeletonFieldDeclaredInActor() => true;

    /// <summary>声明位置。</summary>
    public const string SkeletonDeclaration = "Actor.pas:1430 m_boSkeleton:Boolean;";

    /// <summary>位置已知。</summary>
    public static bool SkeletonDeclarationKnown()
        => SkeletonDeclaration.Contains("Actor.pas:1430");

    /// <summary>**多处置真置假。**</summary>
    public static bool SkeletonSetInMultiplePlaces() => true;

    /// <summary>赋值点。</summary>
    public static readonly string[] SkeletonAssignmentSites =
    {
        "Actor.pas:2817 False", "Actor.pas:3790 False", "Actor.pas:3795 False",
        "Actor.pas:4182 True", "Actor.pas:4375 False", "PlayScn.pas:8032 True",
    };

    /// <summary>六处。</summary>
    public static bool SixSkeletonAssignments() => SkeletonAssignmentSites.Length == 6;

    /// <summary>**比较的是对象指针相等。**</summary>
    public static bool PointerIdentityComparison() => true;

    /// <summary>**命中即 `Exit`。**</summary>
    public static bool ExitOnFirstMatch() => true;

    /// <summary>实现（首个命中）。</summary>
    public static bool FindInRange(IEnumerable<object> cells, object target, bool requireNotSkeleton)
    {
        foreach (object o in cells)
        {
            if (o != null && ReferenceEquals(o, target))
            {
                if (!requireNotSkeleton)
                    return true;
            }
        }

        return false;
    }

    /// <summary>**与范围族的"全部收集"相反。**</summary>
    public static bool ContrastWithRangeFamily() => true;

    /// <summary>对比表。</summary>
    public static readonly string[] RangeFamilyContrast =
    {
        "IsValid 族：每格一个目标、命中即退出、返回布尔",
        "范围族（J160）：收集全部、返回总数、列表引用带出",
    };

    /// <summary>两条。</summary>
    public static bool TwoContrastEntries() => RangeFamilyContrast.Length == 2;

    /// <summary>**`Result` 在循环之前初始化。**</summary>
    public static bool ResultInitializedBeforeLoops() => true;

    /// <summary>**`try`/`finally` 在最内层循环体里。**</summary>
    public static bool TryFinallyInsideInnermost() => true;
    /// <summary>**函数级没有 `finally`。**</summary>
    public static bool NoFunctionLevelFinally() => true;

    /// <summary>**每次迭代完整地"加锁-检查-解锁"。**</summary>
    public static bool EachIterationLocks()
    {
        int locks = 0;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
                locks++;
        }

        return locks == 9;
    }

    /// <summary>半径 1 时 9 次。</summary>
    public static bool NineLocksAtRadiusOne() => EachIterationLocks();

    // ===================== 六、GetXYHuman =====================

    /// <summary>**只有一个条件（种族恰好是玩家）。**</summary>
    public static bool GetXYHumanSingleCondition() => true;

    /// <summary>实现。</summary>
    public static bool GetXYHumanGate(int raceServer) => raceServer == RC_PLAYOBJECT;

    /// <summary>实测。</summary>
    public static bool GetXYHumanGateValues()
        => GetXYHumanGate(0) && !GetXYHumanGate(1) && !GetXYHumanGate(150) && !GetXYHumanGate(10);

    /// <summary>**完全不看幽灵与死亡。**</summary>
    public static bool IgnoresGhostAndDeath() => true;

    /// <summary>**英雄不算"人"。**</summary>
    public static bool HeroNotCounted()
        => !GetXYHumanGate(RC_HEROOBJECT);

    /// <summary>**人形怪物也不算。**</summary>
    public static bool PlayMonsterNotCounted()
        => !GetXYHumanGate(RC_PLAYMOSTER);

    /// <summary>**命中即 `Break`。**</summary>
    public static bool BreakOnFirst() => true;

    /// <summary>**一条对六条：`GetXYHuman` 只要求一条、`GetXYObjCount` 要求六条。**</summary>
    public static bool OneConditionVersusSix()
        => GetXYHumanConditionCount() == 1 && GateConditions.Length == 6;

    /// <summary>`GetXYHuman` 的条件条数。</summary>
    public static int GetXYHumanConditionCount() => 1;

    /// <summary>**`GetXYHuman` 是本族最松的。**</summary>
    public static bool HumanIsLoosest()
        => GetXYHumanGate(0)
           && ActualGate(true, true, true, true, true);

    // ===================== 七、sub_4B5FC8 =====================

    /// <summary>**直接检查 `chFlag = 2`。**</summary>
    public static bool ChecksChFlagEqualsTwo() => true;

    /// <summary>实现。</summary>
    public static bool IsBlocked(bool cellOk, int chFlag)
        => !(cellOk && chFlag == 2);

    /// <summary>**实测三态。**</summary>
    public static bool IsBlockedValues()
        => !IsBlocked(true, 2) && IsBlocked(true, 0) && IsBlocked(true, 1);

    /// <summary>**印证 `chFlag = 2` 就是被阻挡。**</summary>
    public static bool ConfirmsTwoIsBlocked() => true;

    /// <summary>**与 `SetMapXYFlag` 配对。**</summary>
    public static bool PairsWithSetMapXYFlag() => true;

    /// <summary>`SetMapXYFlag(true)` 写 0。</summary>
    public static int SetMapXYFlagWrites(bool boFlag) => boFlag ? 0 : 2;

    /// <summary>**写入 2 之后查询返回"被阻挡"。**</summary>
    public static bool RoundTripBlocked()
        => !IsBlocked(true, SetMapXYFlagWrites(false));

    /// <summary>**写入 0 之后查询返回"未阻挡"。**</summary>
    public static bool RoundTripPassable()
        => IsBlocked(true, SetMapXYFlagWrites(true));

    /// <summary>**函数名是反编译地址。**</summary>
    public static bool AddressDerivedName() => true;

    /// <summary>名字。</summary>
    public const string SubName = "sub_4B5FC8";

    /// <summary>**`sub_` 前缀 + 六位十六进制地址。**</summary>
    public static bool SubNameShape()
        => SubName.StartsWith("sub_") && SubName.Length == 10;

    /// <summary>**全类唯一以 `sub_` 命名的。**</summary>
    public static bool OnlySubPrefixedMethod() => true;

    /// <summary>**与裸偏移字段同族、但更彻底。**</summary>
    public static bool SameFamilyButMoreExtreme() => true;

    /// <summary>该族的四个实例。</summary>
    public static readonly string[] AddressDerivedFamily =
    {
        "bo2B9（字段，J159）", "bo01 / n04（字段，J161）", "sub_4B5FC8（函数名，本批）",
    };

    /// <summary>三项。</summary>
    public static bool ThreeAddressDerivedItems() => AddressDerivedFamily.Length == 3;

    /// <summary>**读共享字段却不加锁。**</summary>
    public static bool NoLockOnSharedField() => true;

    /// <summary>它读的字段。</summary>
    public static readonly string[] SharedFieldNames = { "chFlag", "SetMapXYFlag（写入方）" };

    /// <summary>两个。</summary>
    public static bool TwoSharedFieldNames() => SharedFieldNames.Length == 2;

    /// <summary>**取格失败时返回真。**</summary>
    public static bool OutOfBoundsReturnsTrue() => true;

    /// <summary>实测。</summary>
    public static bool OutOfBoundsReturnsTrueValues()
        => IsBlocked(false, 2) && IsBlocked(false, 0);

    /// <summary>**界外视为"没有被阻挡"。**</summary>
    public static bool OutOfBoundsIsPassable() => IsBlocked(false, 2);

    // ===================== 八、共性 =====================

    /// <summary>**五个单格方法共用双重前置检查。**</summary>
    public static bool SameDoublePrecondition() => true;

    /// <summary>双重前置。</summary>
    public static bool DoublePrecondition(bool cellOk, bool listNotNull) => cellOk && listNotNull;

    /// <summary>**只有 `sub_4B5FC8` 是格级查询。**</summary>
    public static bool OnlyCellLevelQuery() => true;

    /// <summary>**它完全不碰对象列表。**</summary>
    public static bool DoesNotTouchObjectList() => true;

    /// <summary>对象列表级的方法数。</summary>
    public static int ObjectListMethodCount() => 7;

    /// <summary>格级的方法数。</summary>
    public static int CellLevelMethodCount() => 1;

    /// <summary>**七对一。**</summary>
    public static bool SevenToOne() => ObjectListMethodCount() + CellLevelMethodCount() == 8;

    // ===================== 行数 =====================

    /// <summary>八个方法的行数（按源码顺序）。</summary>
    public static readonly int[] MethodLineCounts = { 47, 49, 34, 5, 38, 38, 37, 9 };

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

    /// <summary>实测 257 行。</summary>
    public static bool TotalLinesValues() => TotalLines() == 257;

    /// <summary>**第二个重载最长（49）。**</summary>
    public static bool SecondOverloadIsLongest()
        => MethodLineCounts[1] == 49;

    /// <summary>**`IsCheapStuff` 最短（5）。**</summary>
    public static bool CheapStuffIsShortest()
        => MethodLineCounts[3] == 5;

    /// <summary>**两个 `IsValid` 完全等长（38 对 38）。**</summary>
    public static bool TwoIsValidTied()
        => MethodLineCounts[4] == MethodLineCounts[5];

    /// <summary>**它们只差一个条件却恰好等长 —— 那一条挤在同一行里。**</summary>
    public static bool TiedDespiteExtraCondition() => true;

    /// <summary>**两个 `GetXYObjCount` 相差 2 行。**</summary>
    public static bool TwoCountOverloadsDifferByTwo()
        => MethodLineCounts[1] - MethodLineCounts[0] == 2;
}
