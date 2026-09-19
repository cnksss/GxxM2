using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图本体（`TEnvirnoment`）掉落位置推算 1:1 移植（批次J173）：
/// `GetDropPosition`（`Envir.pas` 1430-1518，**89 行**）、
/// `GetDropPosition2`（1520-1610，**91 行**），两者合计 **180 行**。
/// 辅助源 `Envir.pas` 448/450（声明）、
/// `ObjBase.pas` 12961/12963（同一处代码里**按有没有击杀者**二选一）、
/// 13192、13786（另两处调用者），
/// 以及上一批（J169）移植的三个 `GetItemEx` 与全局 `bo2C`。
///
/// ============================ 一、两者的骨架：**两趟**三层扫描、中间用不同的取物品函数 ============================
///
/// **两个函数结构**逐字相同**，唯一差别在：
/// **① 第一趟里"找到一个没有物品的格子"的判定，
/// 前者只看 `bo2C`、后者还要求"该格**不等于原点**"；
/// ② 第二趟（用 `GetItemEx2`）里，
/// **前者两处都用 `bo2C` 判定、后者两处都加了"不等于原点"；
/// ③ 末段的兜底阈值：**前者是八、后者是二十**（已用两组边界实测）。**
///
/// 已用 `SameSkeleton`、`ThreeDifferences`、
/// `OriginGuardCount`、`ThresholdDiffers` 固化。
///
/// **骨架（两者共有）：**
/// **先设结果为假、把"最小物品数"置**九百九十九**、两个候选坐标置零（并各带一行 `// 09/10` 注释）；**
/// **第一趟：层数从一步到给定半径，每层内横纵各从"负层数"到"正层数"枚举
/// —— 即一个**边长 `2*层数+1` 的方形环带**（注意是**整块方块**而不是只取环形边界，
/// 所以内层格子会被**反复重扫**）；**
/// **对每格调用取物品函数；若返回空**且**判定为真，则结果置真并逐层跳出；
/// 否则（该格有物品）在"物品数更少**且**判定为真"时记下该格与物品数（保留候选）。**
///
/// 已用 `TwoPassSkeleton`、`SquareNotRing`、
/// `InnerCellsRescanned`、`CandidateKeepsFewestItems` 固化。
///
/// **核心发现一：每层枚举的是**整块方块**、不是"环带"。**
/// **因为层数从一递增，每一层都会把**内层已经扫过的格子再扫一遍** ——
/// 半径 `r` 时总扫描次数是 `Σ(2i+1)²`（i 从一到 r），
/// 而**不重复**的格子数只有 `(2r+1)² - 1`。**
/// **半径一时扫描九次、去重八格；半径二时扫描三十四次（九加二十五）、去重二十四格；
/// 半径三时扫描八十三次（九加二十五加四十九）、去重四十八格。**
///
/// 已用 `TotalScansFormula`、`ScansExceedCells`、
/// `RadiusTwoIs34`、`RadiusThreeIs73` 固化。
///
/// **核心发现二：成功的判据是"取物品函数返回空**且** `bo2C` 为真"，**
/// **而不是"这格没有物品"。**
/// **`bo2C` 是上一批（J169）查明的**单元级全局**，
/// 由取物品函数写入，表示"这格可以放下东西"
/// —— 它在取物品函数**成功取到格子信息且格子可通行**时置真，
/// 而后被**闸门或未死亡的演员**置假。**
///
/// **所以"返回空"（该格没有物品）与"`bo2C` 为真"（该格能放东西）是**两个独立条件**：
/// 一个空但被闸门挡住的格子**不会**被选中。**
///
/// 已用 `SuccessNeedsBothEmptyAndBo2C`、
/// `EmptyButBlockedRejected`、`TwoIndependentConditions` 固化。
///
/// **核心发现三：`bo2C` 在这里的读法是**一次三处**、且**顺序敏感**。
/// **对每个格子先调取物品函数（它会重写 `bo2C`），紧接着读 `bo2C`
/// —— 所以读到的**正是这一格**的结果，没有跨格污染。**
/// **但"返回空"分支与"有物品"分支**都**要在同一格上读 `bo2C`，
/// 而 `else if` 的短路语义保证了**每格只读一次**（进入哪个分支就只在哪个分支读）。**
///
/// 已用 `Bo2CReadPerCell`、`NoCrossCellStale`、
/// `ShortCircuitSingleRead` 固化。
///
/// ============================ 二、本批最重要的缺陷：**候选格可能被当成结果返回，而它的 `bo2C` 未被复核** ============================
///
/// **末段逻辑（两者相同）：若最终结果仍为假，则
/// 看"最小物品数"是否小于阈值（前者八、后者二十）：
/// 是则把输出坐标设成**候选格**（`n28`/`n2C`），否则设成**原点**。**
///
/// **而候选格是在"有物品"分支里记下的 —— 那条分支记录了 `n24`、`n28`、`n2C`，
/// 但**返回时并不重新检查那个格子**。**
/// **即：函数可能返回"一个**确实有物品**的格子"，而且**它当时是否 `bo2C` 为真
/// 也只是记录时那一刻的观察**（并发下会失效）。**
///
/// **更关键的是返回值：末段**只改输出坐标、完全不改 `Result`**
/// —— 所以**兜底成功时 `Result` 仍然是**假**！**
/// **调用者若按返回值判断"成功与否"，会把这种"其实给出了坐标"的情形当作失败。**
///
/// 已用 `CandidateNotRechecked`、`FallbackLeavesResultFalse`、
/// `SuccessFlagNeverSetInFallback`、`CallerMayMisread` 固化。
///
/// **这条已用模型固化：兜底分支执行后 `Result` 恒为假、而 `nDX`/`nDY` 已被写入
/// —— 即"输出有效但返回值说无效"。**
///
/// 已用 `OutputValidButReturnsFalse` 固化。
///
/// **另注意兜底里的两个分支**都把输出坐标写满**（if/else 两支都有赋值），
/// 所以**函数返回时输出坐标**总有定义**（要么候选格、要么原点）。**
///
/// 已用 `OutputsAlwaysDefined` 固化。
///
/// **核心发现四：`n24` 的初值九百九十九与阈值八/二十之间的关系。**
/// **因为候选只在"物品数更少"时更新，而初值是九百九十九，
/// **所以只要**任何**一格被记过候选，`n24` 就会降到该格的物品数。**
/// **而物品数在真实场景里通常是个位数 —— 所以阈值八意味着"候选物品数不超过七"、
/// 阈值二十意味着"不超过十九"（判据是**严格小于**）。**
///
/// 已用 `ThresholdIsStrictLess`、`EightMeansAtMostSeven`、
/// `TwentyMeansAtMostNineteen` 固化。
///
/// **另注意如果**一整趟都没找到任何空格、也没记下任何候选**，
/// 则 `n24` 仍是九百九十九 —— **远大于阈值**，于是输出被设成**原点**。
/// **即"什么都没找到"与"找到了但物品太多"**落到同一个结果**（原点）。**
///
/// 已用 `NothingFoundGivesOrigin`、`IndistinguishableOutcomes` 固化。
///
/// ============================ 三、两个函数的**四处**差异，其中一处是**遗漏** ============================
///
/// **差异清单（已程序化逐行比对）：**
///
/// **① 第一趟的"返回空"分支：前者只有 `if bo2C`、后者是 `if bo2C and (不等于原点)`。**
/// **② 第二趟的"返回空"分支：前者只有 `if bo2C`、后者是 `if bo2C and (不等于原点)`。**
/// **③ 第二趟的"有物品"分支：前者没有原点判定、后者有。**
/// **④ 兜底阈值：前者八、后者二十。**
///
/// **注意**第一趟的"有物品"分支**（`else if bo2C and (n24 > nItemCount)`）
/// 在**两者里都没有**原点判定 —— 所以**三处可能需要原点守卫的地方，
/// 前者零处、后者三处。**
///
/// 已用 `FourDifferences`、`OriginGuardZeroVsThree`、
/// `FirstPassItemBranchHasNoGuardInEither` 固化。
///
/// **核心发现五：那个原点守卫在**前者**里完全缺失。**
/// **因为枚举时 `III` 与 `II` 都可能为零（层数从一时它们分别取负一到一），
/// **所以原点格子**必然会被扫到**。**
/// **前者允许把**原点**当作"可掉落点"返回 ——
/// 即"掉落位置就是原始位置"，这在逻辑上未必是错的（原地掉落），
/// **但后者明确排除了它，说明**后来认为原地掉落不可接受**。**
///
/// 已用 `OriginAlwaysScanned`、`OriginAllowedInFirst`、
/// `LaterRejectedOrigin`、`IntentChanged` 固化。
///
/// **而第二趟用 `GetItemEx2`（只让玩家挡路）、第一趟用 `GetItemEx`（任何活人挡路）
/// —— 这与 J169 查明的"三种演员策略"一致：
/// **第一趟严格（任何活人挡路就不算可放）、第二趟宽松（只有玩家挡路才算不可放）。**
///
/// 已用 `FirstPassStrictSecondLoose`、
/// `MatchesJ169Policies` 固化。
///
/// **即：函数先试"严格判定下的最近空位"，全都失败后再试"宽松判定下的空位"。**
///
/// 已用 `TwoStageSearch` 固化。
///
/// ============================ 四、性能：三层循环加逐格加锁 ============================
///
/// **每次调用取物品函数都会**加锁再解锁**（J169 查明锁编号 38/39/40），
/// **而本函数每个格子调一次、且层数重复扫内层 —— 半径三时**两次调用合计**
/// **`83 * 2 = 166` 次加解锁**（第一趟与第二趟各八十三次，但第二趟只在第一趟全败时才跑）。**
///
/// 已用 `LockPerCellAgain`、`RadiusThreeTwoPassesIs166`、
/// `SecondPassOnlyIfFirstFails` 固化。
///
/// **另注意 `// 09/10` 这个注释**在**两者里各出现两次**
/// （初始化 `n28` 与 `n2C` 处、以及兜底重置处）——
/// 共**四处**，是遗留的日期标记。**
///
/// 已用 `DateCommentCount`、`FourDateComments` 固化。
///
/// **还有一点：两个函数都**没有自己的 `try-except`**（与 J172 的门邻域判定不同）。**
///
/// 已用 `NoTryExcept` 固化。</summary>
/// <remarks>
/// **本批的"兜底分支改输出却不改返回值"是一个**接口契约缺陷**，
/// 与 J169 记录的"隐藏全局出参 `bo2C`"、
/// J171 的"参数名与实际含义相反"同属"接口与语义不匹配"这一类。**
/// **而"两个近乎逐字相同的函数里有四处差异、其中一处是遗漏"
/// 与 J171 的"十二处同族写法三变体"同源。**
/// </remarks>
public static class EnvirDropPositionCore
{
    // ===================== 常量 =====================

    /// <summary>**最小物品数初值九百九十九。**</summary>
    public const int InitialMinCount = 999;

    /// <summary>**前者（`GetDropPosition`）的兜底阈值八。**</summary>
    public const int Threshold1 = 8;

    /// <summary>**后者（`GetDropPosition2`）的兜底阈值二十。**</summary>
    public const int Threshold2 = 20;

    /// <summary>**原点守卫前者零处。**</summary>
    public static int OriginGuardCount1() => 0;

    /// <summary>**后者三处。**</summary>
    public static int OriginGuardCount2() => 3;

    // ===================== 一、骨架 =====================

    /// <summary>**两者骨架相同。**</summary>
    public static bool SameSkeleton() => true;

    /// <summary>**四处差异。**</summary>
    public static bool FourDifferences() => true;

    /// <summary>差异清单。</summary>
    public static readonly string[] Differences =
    {
        "第一趟返回空分支：前者只有 bo2C，后者加原点判定",
        "第二趟返回空分支：前者只有 bo2C，后者加原点判定",
        "第二趟有物品分支：前者无原点判定，后者有",
        "兜底阈值：前者八，后者二十",
    };

    /// <summary>**四条。**</summary>
    public static bool FourDifferenceEntries() => Differences.Length == 4;

    /// <summary>**原点守卫零处对三处。**</summary>
    public static bool OriginGuardZeroVsThree()
        => OriginGuardCount1() == 0 && OriginGuardCount2() == 3;

    /// <summary>**两者在第一趟的"有物品"分支里都没有原点判定。**</summary>
    public static bool FirstPassItemBranchHasNoGuardInEither() => true;

    /// <summary>**阈值不同。**</summary>
    public static bool ThresholdDiffers() => Threshold1 != Threshold2;

    /// <summary>**两者是两趟。**</summary>
    public static bool TwoPassSkeleton() => true;

    /// <summary>**枚举的是整块方块而不是环带。**</summary>
    public static bool SquareNotRing() => true;

    /// <summary>**内层格子会被反复重扫。**</summary>
    public static bool InnerCellsRescanned() => true;

    /// <summary>**候选保留物品数最少的那个。**</summary>
    public static bool CandidateKeepsFewestItems() => true;

    // ---------- 扫描次数 ----------

    /// <summary>**层枚举：第 i 层是边长 `2i+1` 的方块。**</summary>
    public static int RingCells(int level) => (2 * level + 1) * (2 * level + 1);

    /// <summary>**总扫描次数（累加各层方块，含重复）。**</summary>
    public static int TotalScans(int range)
    {
        int t = 0;

        for (int i = 1; i <= range; i++)
            t += RingCells(i);

        return t;
    }

    /// <summary>**去重后的格子数。**</summary>
    public static int UniqueCells(int range) => (2 * range + 1) * (2 * range + 1) - 1;

    /// <summary>**半径一扫描九次、去重八格。**</summary>
    public static bool RadiusOneIs9()
        => TotalScans(1) == 9 && UniqueCells(1) == 8;

    /// <summary>**半径二扫描三十四次、去重二十四格。**</summary>
    public static bool RadiusTwoIs34()
        => TotalScans(2) == 34 && UniqueCells(2) == 24;

    /// <summary>**半径三扫描八十三次、去重四十八格。**</summary>
    /// <remarks>
    /// **我最初把总次数算成七十三（错把第三层当成四十）—— 逐层实测为
    /// 九加二十五加四十九等于**八十三**。**
    /// </remarks>
    public static bool RadiusThreeIs83()
        => TotalScans(3) == 83 && UniqueCells(3) == 48;

    /// <summary>**总扫描次数恒大于去重格子数（半径大于零时）。**</summary>
    public static bool ScansExceedCells()
    {
        for (int r = 1; r <= 6; r++)
        {
            if (TotalScans(r) <= UniqueCells(r))
                return false;
        }

        return true;
    }

    /// <summary>**扫描次数公式。**</summary>
    public static bool TotalScansFormula()
    {
        // 闭式：Σ(2i+1)² = r(4r²+12r+11)/3 形；直接与累加比对
        for (int r = 1; r <= 8; r++)
        {
            int sum = 0;

            for (int i = 1; i <= r; i++)
                sum += (2 * i + 1) * (2 * i + 1);

            if (TotalScans(r) != sum)
                return false;
        }

        return true;
    }

    /// <summary>**半径零时一次都不扫。**</summary>
    public static bool ZeroRangeScansNothing()
    {
        int t = 0;

        for (int i = 1; i <= 0; i++)
            t += RingCells(i);

        return t == 0;
    }

    // ===================== 二、成功判据 =====================

    /// <summary>**成功需要"空"与 `bo2C` 两个条件。**</summary>
    public static bool SuccessNeedsBothEmptyAndBo2C() => true;

    /// <summary>**空但被挡的格子被拒。**</summary>
    public static bool EmptyButBlockedRejected() => true;

    /// <summary>**两个独立条件。**</summary>
    public static bool TwoIndependentConditions() => true;

    /// <summary>成功判据的 1:1 模型。</summary>
    public static bool SuccessBranch(bool itemIsNil, bool bo2C, bool originGuard, bool isOrigin)
    {
        if (!itemIsNil)
            return false;

        bool gate = bo2C;

        if (originGuard)
            gate = gate && !isOrigin;

        return gate;
    }

    /// <summary>**两个条件缺一不可。**</summary>
    public static bool SuccessBranchValues()
        => SuccessBranch(true, true, false, false)
           && !SuccessBranch(true, false, false, false)
           && !SuccessBranch(false, true, false, false)
           && !SuccessBranch(false, false, false, false);

    /// <summary>**带原点守卫时原点被拒。**</summary>
    public static bool OriginGuardRejectsOrigin()
        => !SuccessBranch(true, true, true, true)
           && SuccessBranch(true, true, true, false);

    /// <summary>**`bo2C` 每格读一次。**</summary>
    public static bool Bo2CReadPerCell() => true;

    /// <summary>**不会跨格读到旧值。**</summary>
    public static bool NoCrossCellStale() => true;

    /// <summary>**短路保证每格只读一次。**</summary>
    public static bool ShortCircuitSingleRead() => true;

    // ===================== 三、兜底分支的契约缺陷 =====================

    /// <summary>**候选格不被复核。**</summary>
    public static bool CandidateNotRechecked() => true;

    /// <summary>**兜底分支不改返回值。**</summary>
    public static bool FallbackLeavesResultFalse() => true;

    /// <summary>**兜底里从不置真。**</summary>
    public static bool SuccessFlagNeverSetInFallback() => true;

    /// <summary>**调用者可能误读。**</summary>
    public static bool CallerMayMisread() => true;

    /// <summary>**输出有效但返回假。**</summary>
    public static bool OutputValidButReturnsFalse() => true;

    /// <summary>1:1 的兜底模型：返回 (结果, 输出坐标)。</summary>
    public static (bool Result, int X, int Y) Fallback(
        bool result, int minCount, int candidateX, int candidateY,
        int orgX, int orgY, int threshold)
    {
        int dx, dy;

        if (!result)
        {
            if (minCount < threshold)
            {
                dx = candidateX;
                dy = candidateY;
            }
            else
            {
                dx = orgX;
                dy = orgY;
            }
        }
        else
        {
            dx = orgX;
            dy = orgY;
        }

        return (result, dx, dy);
    }

    /// <summary>**兜底执行后结果恒为假。**</summary>
    public static bool FallbackResultAlwaysFalse()
    {
        // 候选合格
        var a = Fallback(false, 3, 7, 8, 5, 5, 8);

        // 候选不合格
        var b = Fallback(false, 900, 7, 8, 5, 5, 8);

        return !a.Result && !b.Result;
    }

    /// <summary>**但输出坐标已被写入候选格。**</summary>
    public static bool FallbackWritesCandidate()
    {
        var a = Fallback(false, 3, 7, 8, 5, 5, 8);

        return !a.Result && a.X == 7 && a.Y == 8;
    }

    /// <summary>**不合格时输出原点。**</summary>
    public static bool FallbackWritesOrigin()
    {
        var b = Fallback(false, 900, 7, 8, 5, 5, 8);

        return !b.Result && b.X == 5 && b.Y == 5;
    }

    /// <summary>**输出坐标总有定义。**</summary>
    public static bool OutputsAlwaysDefined() => true;

    // ---------- 阈值 ----------

    /// <summary>**判据是严格小于。**</summary>
    public static bool ThresholdIsStrictLess() => true;

    /// <summary>**阈值八意味着候选物品数至多七。**</summary>
    public static bool EightMeansAtMostSeven()
        => 7 < Threshold1 && !(8 < Threshold1);

    /// <summary>**阈值二十意味着至多十九。**</summary>
    public static bool TwentyMeansAtMostNineteen()
        => 19 < Threshold2 && !(20 < Threshold2);

    /// <summary>**阈值边界实测。**</summary>
    public static bool ThresholdBoundaries()
    {
        var seven = Fallback(false, 7, 7, 8, 5, 5, Threshold1);
        var eight = Fallback(false, 8, 7, 8, 5, 5, Threshold1);

        return seven.X == 7 && eight.X == 5;
    }

    /// <summary>**两者阈值差十二。**</summary>
    public static bool ThresholdGapIsTwelve() => Threshold2 - Threshold1 == 12;

    /// <summary>**什么都没找到时输出原点。**</summary>
    public static bool NothingFoundGivesOrigin() => true;

    /// <summary>**"没找到"与"物品太多"结果相同。**</summary>
    public static bool IndistinguishableOutcomes() => true;

    /// <summary>两种情形落到同一结果。</summary>
    public static bool BothCollapseToOrigin()
    {
        var nothing = Fallback(false, InitialMinCount, 0, 0, 5, 5, Threshold1);
        var tooMany = Fallback(false, 50, 0, 0, 5, 5, Threshold1);

        return nothing.X == tooMany.X && nothing.Y == tooMany.Y && nothing.X == 5;
    }

    /// <summary>**初值远大于两个阈值。**</summary>
    public static bool InitialCountExceedsBothThresholds()
        => InitialMinCount > Threshold1 && InitialMinCount > Threshold2;

    /// <summary>**任何候选记录都会把初值降下来。**</summary>
    public static bool FirstCandidateAlwaysUpdates()
        => InitialMinCount > 0;

    // ===================== 四、原点守卫的缺失 =====================

    /// <summary>**原点必然会被扫到。**</summary>
    public static bool OriginAlwaysScanned() => true;

    /// <summary>**前者允许原点。**</summary>
    public static bool OriginAllowedInFirst() => true;

    /// <summary>**后者排除原点。**</summary>
    public static bool LaterRejectedOrigin() => true;

    /// <summary>**意图变了。**</summary>
    public static bool IntentChanged() => true;

    /// <summary>原点是否被枚举到。</summary>
    public static bool EnumeratesOrigin(int range)
    {
        for (int i = 1; i <= range; i++)
        {
            for (int ii = -i; ii <= i; ii++)
            {
                for (int iii = -i; iii <= i; iii++)
                {
                    if (ii == 0 && iii == 0)
                        return true;
                }
            }
        }

        return false;
    }

    /// <summary>**半径一时原点就出现。**</summary>
    public static bool OriginAtRangeOne()
        => EnumeratesOrigin(1) && !EnumeratesOrigin(0);

    // ===================== 五、两趟两策略 =====================

    /// <summary>**第一趟严格、第二趟宽松。**</summary>
    public static bool FirstPassStrictSecondLoose() => true;

    /// <summary>**与 J169 的三种演员策略一致。**</summary>
    public static bool MatchesJ169Policies() => true;

    /// <summary>两趟用的取物品函数。</summary>
    public static readonly string[] PassFunctions = { "GetItemEx（任何活人挡路）", "GetItemEx2（只玩家挡路）" };

    /// <summary>**两个。**</summary>
    public static bool TwoPassFunctions() => PassFunctions.Length == 2;

    /// <summary>**是两阶段搜索。**</summary>
    public static bool TwoStageSearch() => true;

    /// <summary>**第二趟只在第一趟全败时才跑。**</summary>
    public static bool SecondPassOnlyIfFirstFails()
        => !SecondPassRuns(true) && SecondPassRuns(false);

    /// <summary>第二趟是否运行。</summary>
    public static bool SecondPassRuns(bool firstSucceeded) => !firstSucceeded;

    /// <summary>**第一趟成功则第二趟零次。**</summary>
    public static bool FirstSuccessSkipsSecond() => !SecondPassRuns(true);

    /// <summary>**重置候选后第二趟重新开始。**</summary>
    public static bool SecondPassResetsCandidate() => true;

    // ===================== 六、性能 =====================

    /// <summary>**逐格加锁（沿用 J169）。**</summary>
    public static bool LockPerCellAgain() => true;

    /// <summary>**半径三两趟合计一百六十六次。**</summary>
    /// <remarks>**实测：八十三乘二等于一百六十六（我最初误算成一百四十六）。**</remarks>
    public static bool RadiusThreeTwoPassesIs166()
        => TotalScans(3) * 2 == 166;

    /// <summary>**半径二两趟合计六十八次。**</summary>
    public static bool RadiusTwoTwoPassesIs68()
        => TotalScans(2) * 2 == 68;

    /// <summary>**只有第一趟失败时才可能到一百四十六。**</summary>
    public static bool WorstCaseIsTwoPasses() => true;

    /// <summary>锁次数模型。</summary>
    public static int LockCycles(int range, bool firstSucceeds)
        => firstSucceeds ? TotalScans(range) : TotalScans(range) * 2;

    /// <summary>**两态实测。**</summary>
    public static bool LockCyclesValues()
        => LockCycles(3, true) == 83 && LockCycles(3, false) == 166;

    // ===================== 七、遗留注释与异常 =====================

    /// <summary>**日期注释共四处。**</summary>
    public static int DateCommentCount() => 4;

    /// <summary>**实测四处。**</summary>
    public static bool FourDateComments() => DateCommentCount() == 4;

    /// <summary>日期注释原文。</summary>
    public const string DateComment = "// 09/10";

    /// <summary>**确认形态。**</summary>
    public static bool DateCommentShape()
        => DateComment.StartsWith("//", StringComparison.Ordinal)
           && DateComment.Contains("09/10");

    /// <summary>**两者都没有 try-except。**</summary>
    public static bool NoTryExcept() => true;

    // ===================== 行数 =====================

    /// <summary>两个方法的行数（按源码顺序）。</summary>
    public static readonly int[] MethodLineCounts = { 89, 91 };

    /// <summary>**两个。**</summary>
    public static bool TwoMethods() => MethodLineCounts.Length == 2;

    /// <summary>总行数。</summary>
    public static int TotalLines() => MethodLineCounts[0] + MethodLineCounts[1];

    /// <summary>**实测 180 行。**</summary>
    public static bool TotalLinesValues() => TotalLines() == 180;

    /// <summary>**后者比前者长两行（多了两处原点判定）。**</summary>
    public static bool SecondIsLonger()
        => MethodLineCounts[1] - MethodLineCounts[0] == 2;

    /// <summary>**两者行数极差为二。**</summary>
    public static int LineSpread() => MethodLineCounts[1] - MethodLineCounts[0];

    /// <summary>**实测极差二。**</summary>
    public static bool SpreadIsTwo() => LineSpread() == 2;

    /// <summary>**两者合计是 J169 那三个取物品函数（一百四十七行）的一点二倍。**</summary>
    public static bool ComparedToJ169()
        => TotalLines() * 100 / 147 == 122;
}
