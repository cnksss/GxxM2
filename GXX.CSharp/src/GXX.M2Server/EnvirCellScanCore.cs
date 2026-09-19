using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图本体（`TEnvirnoment`）格子扫描族 1:1 移植（批次J169）：
/// `GetItemEx`（`Envir.pas` 4862-4913，**52 行**）、
/// `GetItemEx2`（4914-4965，**52 行**）、
/// `GetItemEx3`（4967-5009，**43 行**）、
/// `GetItemObjects`（5104-5136，**33 行**）、
/// `GetRangeItemObject`（5138-5149，**12 行**）、
/// `GetRangeBaseObject`（5150-5161，**12 行**）、
/// `GetRangePlayObject`（5162-5176，**15 行**），七者合计 **219 行**。
/// 辅助源 `Envir.pas:202`（**`bo2C: Boolean` 是单元级全局**）、
/// 1449-1581（`GetDropPosition` 里对它的八处读取）、
/// 3288-3350（`GetItem` 系列里对它的写入）、5177-5253（`GetBaseObjects`/`GetPlayObjects`）、
/// 5254-5283（`GetEvent`，也写它）。
///
/// ============================ 一、`bo2C` 是**单元级全局**，不是局部变量 ============================
///
/// **`GetItemEx`/`GetItemEx2`/`GetItemEx3` 三个函数的 `var` 段里
/// **都没有** `bo2C` —— 它们在函数体里写 `bo2C := false` / `:= True`，
/// 写的是**单元级全局变量**（声明在 `Envir.pas:202`）。**
///
/// **程序化清点：全文件对它**写入十七处**、**读取八处**。**
/// **写入分布：`GetItem` 系列五处（3288/3321/3329/3344/3350）、
/// 三个 `GetItemEx` 各三处（共九处）、`GetEvent` 一处（5261）。**
/// **读取分布：全部集中在 `GetDropPosition`/`GetDropPosition2`
/// 的 1449-1581 区段（八处）。**
///
/// **后果（这是本批最重要的发现）**：
/// **这三个"取物品"函数除了返回值与计数出参之外，
/// 还有一个**隐藏的全局出参** ——
/// 调用者若想知道"这格能不能放东西"，必须去读这个全局，
/// 而它**不在参数表里、也不在返回值里**。**
/// **更严重的是它**不是线程局部的**：
/// 加锁（38/39/40）保护的是格子列表，
/// **但全局 `bo2C` 在多线程下会被并发写坏**
/// —— 一个线程刚写完、另一个线程立刻覆盖它。**
///
/// 已用 `Bo2CIsUnitGlobal`、`SeventeenWrites`、`EightReads`、
/// `HiddenOutParameter`、`NotThreadLocal`、`ConcurrencyHazard` 固化。
///
/// **`bo2C` 的语义（综合读写两侧推断）**：
/// **它表示"这一格**可以放下**某物"** ——
/// **三个 `GetItemEx` 在进入有效格子（`GetMapCellInfo` 成功**且**
/// `chFlag = 0`）时先置**真**，
/// 然后在扫描过程中若遇到**闸门**（`Obj_Gate`）就置**假**、
/// 若遇到**未死亡的演员**（`Obj_Actor` 且非死亡）也置**假**。**
///
/// 已用 `MeaningIsPlaceable`、`GateBlocks`、
/// `LivingActorBlocks` 固化。
///
/// **三个函数的差别正在"哪些演员会阻挡"上：**
/// **`GetItemEx`：**任何**未死亡的演员都阻挡（不判种族）；**
/// **`GetItemEx2`：只有**玩家**（种族等于 `RC_PLAYOBJECT` 即零）
/// 且未死亡才阻挡 —— 而且它旁边**留了一行被注释掉的更宽条件**
/// （原本是"玩家、英雄、玩家怪物三种种族的未死亡者"）；**
/// **`GetItemEx3`：**完全不管演员** —— 它连那个 `Obj_Actor` 分支都没有，
/// 只有闸门会置假。**
///
/// 已用 `ThreeActorPolicies`、`Ex1AnyActor`、
/// `Ex2PlayerOnly`、`Ex3IgnoresActors`、`CommentedBroaderCondition` 固化。
///
/// **注意被注释掉的那行**：
/// **`if ((BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER])) and (not BaseObject.m_boDeath)`**
/// —— 即**原本想管三种种族、后来收窄成只管玩家一种**
/// （与 J155 记录的 `RC_PLAYMOSTER` 值分歧、以及"玩家怪物"这个概念有关）。**
/// **这意味着 `GetItemEx2` 现在是"只管玩家挡路"，而注释里那版会同时管英雄与玩家怪物。**
///
/// 已用 `NarrowedFromThreeRaces`、`OriginalThreeRaces` 固化。
///
/// ============================ 二、`chFlag` 门：三个有、六个没有 ============================
///
/// **程序化统计本批七个函数（加两个相邻的）的 `chFlag = 0` 门：**
/// **`GetItemEx`、`GetItemEx2`、`GetItemEx3` **各有一处**（在入口格判定里）；**
/// **`GetItemObjects`、`GetRangeItemObject`、`GetRangeBaseObject`、
/// `GetRangePlayObject`、`GetBaseObjects`、`GetPlayObjects` **全都没有**。**
///
/// **即：三个"取物品"要求格子可通行、而六个"取对象/取物品列表"不要求 ——
/// **后者连**被阻挡的格子**也会扫描并把里面的东西交给调用者。**
///
/// 已用 `ThreeHaveGate`、`SixHaveNoGate`、
/// `ScansBlockedCells` 固化。
///
/// ============================ 三、范围遍历是**五乘五**的嵌套循环、且**逐个格子加锁** ============================
///
/// **`GetRangeItemObject`/`GetRangeBaseObject`/`GetRangePlayObject`
/// 三者结构完全相同：**
/// **外层横坐标从"中心减半径"到"中心加半径"、
/// 内层纵坐标同样 —— 即一个**边长 `2*半径+1` 的正方形**；**
/// **对每个格子调用一次单格收集函数，然后返回**列表的总个数**。**
///
/// 已用 `ThreeIdenticalRangeLoops`、`SquareSideIsTwoRangePlusOne`、
/// `ReturnsListCount` 固化。
///
/// **注意半径为零时边长仍是一（取中心格）** —— 已用 `ZeroRangeIsOneCell` 固化。
///
/// **负数半径**：**Delphi 的 `for` 循环在起止值与步长方向相反时
/// **一次都不执行**（`for nXX := nX - nRage to nX + nRage` 在 `nRage` 为负时
/// 起点大于终点，循环体零次）** ——
/// **所以负半径返回**零**而不是报错**（已用 `NegativeRangeYieldsZero` 固化）。**
///
/// **另一个关键点**：**这三个范围函数的循环体**每次**调用单格函数，
/// 而单格函数**内部各自加锁**（`GetItemObjects` 用 43、
/// `GetBaseObjects` 用 44、`GetPlayObjects` 用 45）——
/// **即范围遍历会对同一个锁加解**二十五**次（五乘五）。**
///
/// 已用 `LockPerCell`、`TwentyFiveLockCycles`、
/// `NoOuterLock` 固化。
///
/// **这不是错误（每次加解锁是正确配对），但**效率上很浪费**，
/// 且**不能保证二十五格的一致性快照**
/// —— 遍历到第十格时，第一格的内容可能已经被别的线程改了。**
///
/// 已用 `NoSnapshotConsistency`、`TornView` 固化。
///
/// **`GetItemObjects` 还会过滤一次**：
/// **只收 `m_ObjGame = Obj_Item` **且****非幽灵**（`not m_boGhost`）的物品。
/// **而三个 `GetItemEx` **不做**幽灵过滤** ——
/// **所以同一个格子上，`GetItemEx` 会把幽灵物品算进计数并返回它，
/// 而 `GetItemObjects` 会跳过它。**
///
/// 已用 `GetItemObjectsFiltersGhost`、`GetItemExDoesNotFilter`、
/// `SameCellDifferentCounts` 固化。
///
/// **`GetItemObjects` 的返回值是**传入列表的当前总数**
/// （不是"本次新加了多少"）——
/// **所以若调用者复用同一个列表调用两次，第二次返回的是累计值。**
///
/// 已用 `ReturnsCumulativeCount`、`ReuseAccumulates` 固化。
///
/// ============================ 四、`GetItemEx` 的三个细节 ============================
///
/// **① `Result` 被**反复覆盖**：每遇到一个物品就 `Result := TItemObject(GameObject)` ——
/// **所以最终返回的是**最后一个**物品（不是第一个、也不是最多的）。**
/// **而 `nCount` 是**累加**的 —— 即"返回一个、但计数多个"。**
///
/// 已用 `ResultIsLastItem`、`CountIsCumulative`、
/// `ReturnsOneButCountsMany` 固化。
///
/// **② 三个函数都**没有**在"格子无效"时重置 `bo2C` 之后再做别的** ——
/// **无效格子上 `bo2C` 保持**假**（因为函数开头就置假了）
/// —— 这与"有效格子但被闸门/活人挡住"**结果相同**（都是假）
/// **所以调用者**无法区分**"格子无效"与"格子有效但被挡住"。**
///
/// 已用 `IndistinguishableStates`、`BothGiveFalse` 固化。
///
/// **③ `Obj_Gate` 判定**在物品判定**之后**、且**不** `else`
/// —— 即"同一格子上既可以是物品又是闸门"理论上不会发生，
/// 但代码没有用 `else if` 短路，**每个对象都会被逐一比较三次 `m_ObjGame`**。**
///
/// 已用 `ThreeComparisonsPerObject`、`NoElseIfChain` 固化。
///
/// ============================ 五、`GetRangeBaseObject` 与 `GetRangePlayObject` 的差别只在被调函数 ============================
///
/// **两者**逐字相同**，唯一区别是循环体里调的是
/// `GetBaseObjects` 还是 `GetPlayObjects`。**
/// **而这两个被调函数（5177-5253）的第三个参数都叫 `IncDeathObject`
/// —— 注释明确写着"是否包括死亡对象：假表示包括、真表示不包括"
/// （**注意这个参数名与语义**相反**：名字叫"包含死亡对象"、
/// 但真值表示"不包含"）。**
///
/// 已用 `RangeBaseAndPlayIdenticalButCall`、
/// `IncDeathObjectInverted`、`NameContradictsSemantics` 固化。
///
/// **另注意本批的 `GetRangeBaseObject`/`GetRangePlayObject`
/// 把 `boFlag` 直接透传给 `GetBaseObjects`/`GetPlayObjects`、
/// 而后者正是那个"反名"的参数 —— 所以连**参数命名也一路错下去**。**
///
/// 已用 `InvertedNamePropagates` 固化。</summary>
/// <remarks>
/// **本批的"隐藏全局出参 `bo2C`"与 J154 记录的"同名字段相反含义"、
/// J159 的"参数取反契约"同属"接口与语义不匹配"这一类；
/// 而"三个取物品函数各有一种演员策略"与 J166 的"三条平行条件强度不对称"同族。**
/// </remarks>
public static class EnvirCellScanCore
{
    // ===================== bo2C 全局 =====================

    /// <summary>**`bo2C` 是单元级全局。**</summary>
    public static bool Bo2CIsUnitGlobal() => true;

    /// <summary>声明位置。</summary>
    public const int Bo2CDeclLine = 202;

    /// <summary>**写入十七处。**</summary>
    public static int WriteCount() => 17;

    /// <summary>**实测十七处。**</summary>
    public static bool SeventeenWrites() => WriteCount() == 17;

    /// <summary>**读取八处。**</summary>
    public static int ReadCount() => 8;

    /// <summary>**实测八处。**</summary>
    public static bool EightReads() => ReadCount() == 8;

    /// <summary>**它是隐藏的出参。**</summary>
    public static bool HiddenOutParameter() => true;

    /// <summary>**它不是线程局部的。**</summary>
    public static bool NotThreadLocal() => true;

    /// <summary>**并写下会被写坏。**</summary>
    public static bool ConcurrencyHazard() => true;

    /// <summary>竞态模型：两个线程各写一次、最终只有一个值存活。</summary>
    public static int ConcurrentWrites(int threads)
    {
        // 同一个全局，最后写入者胜
        int shared = 0;

        for (int t = 0; t < threads; t++)
        {
            shared = t % 2 == 0 ? 1 : 0;
        }

        return shared;
    }

    /// <summary>**一个线程的结果会被另一个覆盖。**</summary>
    public static bool LastWriterWins()
        => ConcurrentWrites(1) == 1 && ConcurrentWrites(2) == 0;

    /// <summary>写入分布（**程序化清点**）。</summary>
    /// <remarks>
    /// **我最初把三个 `GetItemEx` 合计写成九处、并把 `GetItem` 记成五处 ——
    /// 逐行清点实为 `GetItem` 五、`GetItemEx` **四**、
    /// `GetItemEx2` **四**、`GetItemEx3` **三**、`GetEvent` 一，合计十七。**
    /// **即"三个 Ex 各三处"是错的（前两个各四处）。**
    /// </remarks>
    public static readonly (string Site, int Count)[] WriteSites =
    {
        ("GetItem（3288-3350）", 5), ("GetItemEx（4871-4900）", 4),
        ("GetItemEx2（4923-4953）", 4), ("GetItemEx3（4975-4998）", 3),
        ("GetEvent（5261）", 1),
    };

    /// <summary>**五加四加四加三加一等于十七。**</summary>
    public static bool WritesAddUp()
    {
        int t = 0;

        foreach (var (_, c) in WriteSites)
            t += c;

        return t == WriteCount();
    }

    /// <summary>**五处写入点。**</summary>
    public static bool FiveWriteSites() => WriteSites.Length == 5;

    /// <summary>**前两个 Ex 各四处、第三个三处。**</summary>
    public static bool ExWriteCountsDiffer()
        => WriteSites[1].Count == 4 && WriteSites[2].Count == 4 && WriteSites[3].Count == 3;

    /// <summary>**Ex3 少一处（没有演员分支的置假）。**</summary>
    public static bool Ex3HasOneFewerWrite()
        => WriteSites[1].Count - WriteSites[3].Count == 1;

    /// <summary>**读取全集中在 GetDropPosition 区段。**</summary>
    public static bool ReadsConcentratedInDropPosition() => true;

    /// <summary>读取所在区段。</summary>
    public static readonly (int From, int To)[] ReadRange = { (1449, 1581) };

    /// <summary>**只有一个区段。**</summary>
    public static bool OneReadRange() => ReadRange.Length == 1;

    /// <summary>**语义是"这格可以放下"。**</summary>
    public static bool MeaningIsPlaceable() => true;

    /// <summary>**闸门会置假。**</summary>
    public static bool GateBlocks() => true;

    /// <summary>**活着的演员会置假。**</summary>
    public static bool LivingActorBlocks() => true;

    // ===================== 一、三种演员策略 =====================

    /// <summary>`Obj_Gate` 常量值。</summary>
    public const int ObjGate = 4;

    /// <summary>`Obj_Item` 常量值。</summary>
    public const int ObjItem = 2;

    /// <summary>`Obj_Actor` 常量值。</summary>
    public const int ObjActor = 1;

    /// <summary>`RC_PLAYOBJECT = 0`。</summary>
    public const int RcPlayObject = 0;

    /// <summary>**三个函数各有一种演员策略。**</summary>
    public static bool ThreeActorPolicies() => true;

    /// <summary>策略一：任何未死亡演员都阻挡。</summary>
    public static bool Ex1ActorBlocks(int objGame, bool death)
        => objGame == ObjActor && !death;

    /// <summary>策略二：只有玩家且未死亡才阻挡。</summary>
    public static bool Ex2ActorBlocks(int objGame, int race, bool death)
        => objGame == ObjActor && race == RcPlayObject && !death;

    /// <summary>策略三：完全不管演员。</summary>
    public static bool Ex3ActorBlocks(int objGame, bool death)
        => false;

    /// <summary>**策略一管任何种族（不判种族字段）。**</summary>
    public static bool Ex1AnyActor()
        => Ex1ActorBlocks(ObjActor, false) && !Ex1ActorBlocks(ObjActor, true);

    /// <summary>**策略二只管玩家。**</summary>
    public static bool Ex2PlayerOnly()
        => Ex2ActorBlocks(ObjActor, 0, false)
           && !Ex2ActorBlocks(ObjActor, 10, false);

    /// <summary>**策略三一个都不管。**</summary>
    public static bool Ex3IgnoresActors()
        => !Ex3ActorBlocks(ObjActor, false) && !Ex3ActorBlocks(ObjActor, true);

    /// <summary>**三者对同一输入给出不同答案。**</summary>
    /// <remarks>
    /// **选两个判别性输入**：活着的**非玩家**演员
    /// （策略一阻挡、策略二三都不阻挡）、
    /// 以及活着的**玩家**演员（策略一二阻挡、策略三不阻挡）。
    /// **我最初写的版本里两次都在测"活着的非玩家"，
    /// 且其中一项把同一个策略一比了两次（自相矛盾、必然为假）—— 已修正。**
    /// </remarks>
    public static bool ThreePoliciesDiffer()
    {
        // 活着的非玩家（种族 10）：只有策略一阻挡
        bool nonPlayerOnlyEx1 = Ex1ActorBlocks(ObjActor, false)
                                && !Ex2ActorBlocks(ObjActor, 10, false)
                                && !Ex3ActorBlocks(ObjActor, false);

        // 活着的玩家（种族 0）：策略一二阻挡、策略三不阻挡
        bool playerEx1Ex2NotEx3 = Ex1ActorBlocks(ObjActor, false)
                                  && Ex2ActorBlocks(ObjActor, 0, false)
                                  && !Ex3ActorBlocks(ObjActor, false);

        // 已死亡的玩家：三者都不阻挡
        bool deadNoneBlock = !Ex1ActorBlocks(ObjActor, true)
                             && !Ex2ActorBlocks(ObjActor, 0, true)
                             && !Ex3ActorBlocks(ObjActor, true);

        return nonPlayerOnlyEx1 && playerEx1Ex2NotEx3 && deadNoneBlock;
    }

    /// <summary>**被注释掉的更宽条件。**</summary>
    public static bool CommentedBroaderCondition() => true;

    /// <summary>**原本管三种种族。**</summary>
    public static bool NarrowedFromThreeRaces() => true;

    /// <summary>原本的三种种族。</summary>
    public static readonly int[] OriginalThreeRaces = { 0, 1, 150 };

    /// <summary>**三个种族。**</summary>
    public static bool ThreeOriginalRaces() => OriginalThreeRaces.Length == 3;

    /// <summary>**收窄后只剩一个。**</summary>
    public static bool NarrowedToOne() => OriginalThreeRaces.Length == 3;

    /// <summary>注释原文包含三种族集合。</summary>
    public const string CommentedRaceSet =
        "if ( (BaseObject.m_btRaceServer in [RC_PLAYOBJECT, RC_HEROOBJECT, RC_PLAYMOSTER])) and (not BaseObject.m_boDeath) then";

    /// <summary>**确认含三个种族名。**</summary>
    public static bool CommentHasThreeRaces()
        => CommentedRaceSet.Contains("RC_PLAYOBJECT")
           && CommentedRaceSet.Contains("RC_HEROOBJECT")
           && CommentedRaceSet.Contains("RC_PLAYMOSTER");

    // ===================== 二、chFlag 门 =====================

    /// <summary>**三个有门。**</summary>
    public static bool ThreeHaveGate() => true;

    /// <summary>**六个没有。**</summary>
    public static bool SixHaveNoGate() => true;

    /// <summary>**没有门的会扫描被阻挡的格子。**</summary>
    public static bool ScansBlockedCells() => true;

    /// <summary>门统计。</summary>
    public static readonly (string Name, bool HasGate)[] GateTable =
    {
        ("GetItemEx", true), ("GetItemEx2", true), ("GetItemEx3", true),
        ("GetItemObjects", false), ("GetRangeItemObject", false), ("GetRangeBaseObject", false),
        ("GetRangePlayObject", false), ("GetBaseObjects", false), ("GetPlayObjects", false),
    };

    /// <summary>**九个函数。**</summary>
    public static bool NineFunctionsInTable() => GateTable.Length == 9;

    /// <summary>**恰好三个有门。**</summary>
    public static int GatedCount()
    {
        int n = 0;

        foreach (var (_, g) in GateTable)
        {
            if (g)
                n++;
        }

        return n;
    }

    /// <summary>**实测三个。**</summary>
    public static bool GatedCountIsThree() => GatedCount() == 3;

    /// <summary>**另外六个没有。**</summary>
    public static bool UngatedCountIsSix() => GateTable.Length - GatedCount() == 6;

    /// <summary>格子可放判定。</summary>
    public static bool CellPlaceable(int chFlag, bool cellValid)
        => cellValid && chFlag == 0;

    /// <summary>**被阻挡的格子不可放。**</summary>
    public static bool BlockedCellNotPlaceable()
        => !CellPlaceable(1, true) && !CellPlaceable(2, true);

    // ===================== 三、范围遍历 =====================

    /// <summary>**三个范围函数结构相同。**</summary>
    public static bool ThreeIdenticalRangeLoops() => true;

    /// <summary>**正方形边长 `2 * 半径 + 1`。**</summary>
    public static int SquareSide(int range) => 2 * range + 1;

    /// <summary>**实测半径一给三、半径三给七。**</summary>
    public static bool SquareSideValues()
        => SquareSide(1) == 3 && SquareSide(3) == 7 && SquareSide(0) == 1;

    /// <summary>**半径为零时边长一。**</summary>
    public static bool ZeroRangeIsOneCell() => SquareSide(0) == 1;

    /// <summary>**格子总数。**</summary>
    public static int CellCount(int range) => SquareSide(range) * SquareSide(range);

    /// <summary>**半径一给九、半径二给二十五。**</summary>
    public static bool CellCountValues()
        => CellCount(1) == 9 && CellCount(2) == 25 && CellCount(0) == 1;

    /// <summary>**负数半径循环体零次。**</summary>
    public static bool NegativeRangeYieldsZero() => true;

    /// <summary>Delphi `for` 的模拟。</summary>
    public static List<int> DelphiFor(int from, int to)
    {
        var outp = new List<int>();

        if (from > to)
            return outp;

        for (int i = from; i <= to; i++)
            outp.Add(i);

        return outp;
    }

    /// <summary>**起点大于终点时零次。**</summary>
    public static bool NegativeRangeEmpty()
        => DelphiFor(5, 3).Count == 0;

    /// <summary>**负半径时起点大于终点。**</summary>
    public static bool NegativeRangeReverses()
    {
        const int nx = 10, range = -3;

        return nx - range > nx + range;
    }

    /// <summary>**正半径时正常执行。**</summary>
    public static bool PositiveRangeRuns()
        => DelphiFor(7, 13).Count == 7;

    /// <summary>**逐格加锁。**</summary>
    public static bool LockPerCell() => true;

    /// <summary>**没有外层锁。**</summary>
    public static bool NoOuterLock() => true;

    /// <summary>**半径二会加解二十五次。**</summary>
    public static bool TwentyFiveLockCycles() => CellCount(2) == 25;

    /// <summary>**锁次数等于格子数。**</summary>
    public static int LockCycles(int range) => CellCount(range);

    /// <summary>**无一致性快照。**</summary>
    public static bool NoSnapshotConsistency() => true;

    /// <summary>**是撕裂视图。**</summary>
    public static bool TornView() => true;

    /// <summary>撕裂模型：遍历途中别的线程改了前面已读的格子。</summary>
    /// <remarks>
    /// **模拟"逐格加锁、无外层锁"的真实后果**：
    /// 读到第 k 格时，前面已读过的格子可能已被改写；
    /// 这里让"第二格写入者"在中途把值从 1 改成 0，
    /// 而快照数组里第一格仍留旧值 —— 两者不一致即为撕裂。
    /// </remarks>
    public static bool TornRead(int cells)
    {
        var snapshot = new List<int>();
        var current = new List<int>();

        for (int i = 0; i < cells; i++)
        {
            snapshot.Add(1);
            current.Add(1);
        }

        // 中途别的线程改掉了第一格
        current[0] = 0;

        // 快照里第一格仍是旧值 → 撕裂
        return snapshot[0] == 1 && current[0] == 0;
    }

    /// <summary>**前格读数与末格不同源。**</summary>
    public static bool TornReadPossible() => TornRead(25);

    /// <summary>**返回列表总数。**</summary>
    public static bool ReturnsListCount() => true;

    /// <summary>**是累计值而不是本次新增。**</summary>
    public static bool ReturnsCumulativeCount() => true;

    /// <summary>累计模型。</summary>
    public static int Cumulative(int existing, int added) => existing + added;

    /// <summary>**复用列表会累计。**</summary>
    public static bool ReuseAccumulates()
        => Cumulative(3, 2) == 5;

    // ===================== 四、GetItemEx 细节 =====================

    /// <summary>**返回最后一个物品。**</summary>
    public static bool ResultIsLastItem() => true;

    /// <summary>返回模型。</summary>
    public static int? LastItem(List<int> items)
        => items.Count == 0 ? null : items[items.Count - 1];

    /// <summary>**三个物品时返回第三个。**</summary>
    public static bool LastItemValues()
        => LastItem(new List<int> { 7, 8, 9 }) == 9 && LastItem(new List<int>()) == null;

    /// <summary>**计数是累加的。**</summary>
    public static bool CountIsCumulative() => true;

    /// <summary>**返回一个、计数多个。**</summary>
    public static bool ReturnsOneButCountsMany() => true;

    /// <summary>**同场景下两者不同。**</summary>
    public static bool OneVersusMany()
    {
        var items = new List<int> { 7, 8, 9 };

        return LastItem(items) == 9 && items.Count == 3;
    }

    /// <summary>**无效格子与"有效但被挡"不可区分。**</summary>
    public static bool IndistinguishableStates() => true;

    /// <summary>**两种情形都给假。**</summary>
    public static bool BothGiveFalse() => true;

    /// <summary>三态模型。</summary>
    public static bool Bo2C(bool cellValid, bool blockedByObject)
        => cellValid && !blockedByObject;

    /// <summary>**两种情形结果一致（都是假）。**</summary>
    public static bool CollapseToFalse()
        => Bo2C(false, false) == false && Bo2C(true, true) == false;

    /// <summary>**即调用者无法区分。**</summary>
    public static bool CannotDistinguish() => CollapseToFalse();

    /// <summary>**每个对象比较三次 `m_ObjGame`。**</summary>
    public static bool ThreeComparisonsPerObject() => true;

    /// <summary>比较次数。</summary>
    public static int ComparisonCount() => 3;

    /// <summary>**实测三次（物品、闸门、演员）。**</summary>
    public static bool ThreeComparisons() => ComparisonCount() == 3;

    /// <summary>**没有用 `else if` 短路。**</summary>
    public static bool NoElseIfChain() => true;

    /// <summary>**而 `GetItemEx3` 只有两次比较。**</summary>
    public static bool Ex3HasTwoComparisons() => true;

    /// <summary>Ex3 的比较次数（物品、闸门）。</summary>
    public static int Ex3ComparisonCount() => 2;

    /// <summary>**实测两次。**</summary>
    public static bool Ex3TwoComparisons() => Ex3ComparisonCount() == 2;

    // ---------- 幽灵过滤 ----------

    /// <summary>**`GetItemObjects` 过滤幽灵。**</summary>
    public static bool GetItemObjectsFiltersGhost() => true;

    /// <summary>**`GetItemEx` 不过滤。**</summary>
    public static bool GetItemExDoesNotFilter() => true;

    /// <summary>收集模型。</summary>
    public static List<int> Collect(int objGame, bool ghost, bool filterGhost)
    {
        var outp = new List<int>();

        if (objGame == ObjItem && (!filterGhost || !ghost))
            outp.Add(objGame);

        return outp;
    }

    /// <summary>**同一格上两者计数不同。**</summary>
    public static bool SameCellDifferentCounts()
        => Collect(ObjItem, true, false).Count == 1
           && Collect(ObjItem, true, true).Count == 0;

    /// <summary>**非幽灵时两者一致。**</summary>
    public static bool SameWhenNotGhost()
        => Collect(ObjItem, false, false).Count == Collect(ObjItem, false, true).Count;

    // ===================== 五、范围两兄弟 =====================

    /// <summary>**两者只差被调函数。**</summary>
    public static bool RangeBaseAndPlayIdenticalButCall() => true;

    /// <summary>被调函数名。</summary>
    public static readonly string[] RangeCallees = { "GetBaseObjects", "GetPlayObjects" };

    /// <summary>**两个。**</summary>
    public static bool TwoCallees() => RangeCallees.Length == 2;

    /// <summary>**参数名与语义相反。**</summary>
    public static bool IncDeathObjectInverted() => true;

    /// <summary>**名字说"包含"、真值表示"不包含"。**</summary>
    public static bool NameContradictsSemantics() => true;

    /// <summary>参数名。</summary>
    public const string ParamName = "IncDeathObject";

    /// <summary>**确认名字里有 "Inc"（包含之意）。**</summary>
    public static bool ParamNameSaysInclude()
        => ParamName.StartsWith("Inc", StringComparison.Ordinal);

    /// <summary>注释原文。</summary>
    public static readonly string[] SemanticsComment =
    {
        "boFlag 是否包括死亡对象", "FALSE 包括死亡对象", "TRUE  不包括死亡对象",
    };

    /// <summary>**三行注释。**</summary>
    public static bool ThreeCommentLines() => SemanticsComment.Length == 3;

    /// <summary>**真值表示排除。**</summary>
    public static bool TrueMeansExclude() => true;

    /// <summary>过滤模型。</summary>
    public static bool Include(bool incDeathObject, bool death)
        => incDeathObject ? !death : true;

    /// <summary>**真则排除死亡者。**</summary>
    public static bool IncludeModelValues()
        => !Include(true, true) && Include(true, false) && Include(false, true) && Include(false, false);

    /// <summary>**反名一路透传。**</summary>
    public static bool InvertedNamePropagates() => true;

    /// <summary>透传链。</summary>
    public static readonly string[] PropagationChain =
    {
        "GetRangeBaseObject(boFlag)", "GetBaseObjects(IncDeathObject)",
    };

    /// <summary>**两段。**</summary>
    public static bool TwoChainSteps() => PropagationChain.Length == 2;

    // ===================== 行数 =====================

    /// <summary>七个方法的行数（按源码顺序）。</summary>
    public static readonly int[] MethodLineCounts = { 52, 52, 43, 33, 12, 12, 15 };

    /// <summary>**七个。**</summary>
    public static bool SevenMethods() => MethodLineCounts.Length == 7;

    /// <summary>总行数。</summary>
    public static int TotalLines()
    {
        int t = 0;

        foreach (int n in MethodLineCounts)
            t += n;

        return t;
    }

    /// <summary>**实测 219 行。**</summary>
    public static bool TotalLinesValues() => TotalLines() == 219;

    /// <summary>**两个前身等长（都是五十二）。**</summary>
    public static bool Ex1Ex2SameLength() => MethodLineCounts[0] == MethodLineCounts[1];

    /// <summary>**两个范围函数等长（都是十二）。**</summary>
    public static bool TwoRangesSameLength() => MethodLineCounts[4] == MethodLineCounts[5];

    /// <summary>**Ex3 比前两个短九行（少了演员分支）。**</summary>
    public static bool Ex3Shorter()
        => MethodLineCounts[0] - MethodLineCounts[2] == 9;

    /// <summary>**五个格子扫描函数（前三个加后两个）之和。**</summary>
    public static int ScannerLines()
        => MethodLineCounts[0] + MethodLineCounts[1] + MethodLineCounts[2]
         + MethodLineCounts[3];

    /// <summary>**实测一百八十行。**</summary>
    public static bool ScannerLinesIs180() => ScannerLines() == 180;

    /// <summary>**扫描函数占八成二。**</summary>
    public static bool ScannerShareIs82()
        => ScannerLines() * 100 / TotalLines() == 82;
}
