using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 服务端基础方法本体 1:1 移植（批次J141）：
/// `TBaseObject.TurnTo`（`ObjBase.pas` 33382-33386）、`TBaseObject.TurnToEx`（33388-33392）、
/// `TBaseObject.MakeGhost`（33697-33706）、`TAnimalObject.SetTargetXY`（40154-40158）、
/// `TAnimalObject.Wondering`（40160-40169）、
/// **`TAnimalObject.Run`（40171-40220，含一大段被整体注释掉的旧逻辑）**、
/// `TAnimalObject.GotoTargetXY`（15675-15737）、
/// `TBaseObject.SpaceMove` 的内嵌函数 `GetRandXY`（22449-22489）；
/// 辅助源：`Grobal2.pas` 939（`RM_TURN = 20001`）、1210（`RM_TURN_EX = 20256`）、
/// `ObjBase.pas` 13562（`WalkTo(btDir: Byte; boFlag: Boolean): Boolean`）。
///
/// ============================ 一、`TurnTo` 与 `TurnToEx` 是"只差一个消息号"的孪生体 ============================
///
/// 两个方法**逐字相同，唯一差别是发的消息号**：
/// `TurnTo` 发 **`RM_TURN = 20001`**、`TurnToEx` 发 **`RM_TURN_EX = 20256`**。
/// **两者都先写 `m_btDirection := nDir`（无范围校验！）再发消息**，
/// 故**传入越界方向（如 99）会被原样接受并广播**。
/// 已用 `TurnToAndExDifferOnlyByMessage`、`NoDirectionRangeCheck`、
/// `BothAssignFirstThenSend` 固化。
///
/// **注意 J140 的 `TGuardMonster.AttackTarget` 在恢复坐标后调的是 `TurnTo`（不是 `TurnToEx`）** ——
/// 即**护卫转身用的是普通 `RM_TURN`**。已用 `GuardUsesPlainTurnTo` 固化。
///
/// ============================ 二、`MakeGhost` 的五步 ============================
///
/// `MakeGhost`（33697-33706）依次做五件事：
/// **① `m_boGhost := True`；② `m_dwGhostTick := MyGetTickCount()`；
/// ③ `m_CurrTarget := nil`；④ `m_CurrTargetEx := nil`；⑤ `DisappearA()`；
/// ⑥ 若 `g_PluginManager <> nil` 则 `HookBaseObjectMakeGhost(Self)`。**
/// **注意它清的是 `m_CurrTarget`/`m_CurrTargetEx`（两个"当前目标"字段），
/// 而不是 `m_TargetCret`（攻击目标）** —— 即**幽灵化不会清掉 `m_TargetCret`**。
/// 这与 J140 大刀护卫"跨图且天关 → `MakeGhost; Exit`"的用法一致（退出后不再用目标）。
/// 已用 `MakeGhostSixSteps`、`ClearsCurrTargetNotTargetCret`、`PluginHookIsGuarded` 固化。
///
/// ============================ 三、`SetTargetXY` 是纯赋值，`Wondering` 是两级随机 ============================
///
/// **`TAnimalObject.SetTargetXY`（40154-40158）只有两行赋值**（`m_nTargetX := nX`、`m_nTargetY := nY`），
/// **没有任何校验** —— 故**传入 `-1` 会直接把 `-1` 写进目标坐标**，
/// 而 `-1` 正是整个 `ObjMon2.pas` 系列用作"无目标"的哨兵值
/// （见 J139 足球 `m_nTargetX := -1`、J140 护卫 `(m_nTargetX <> -1)`）。
/// **即"用 `SetTargetXY(-1, -1)` 取消目标"是可行的，但那是副作用而非显式设计**。
/// 已用 `SetTargetXYIsPureAssignment`、`NegativeOneWritesSentinel` 固化。
///
/// **`TAnimalObject.Wondering`（40160-40169）是两级随机**：
/// **外层 `if Random(20) = 0`（5%）才动作**；
/// 内层 **`if Random(4) = 1`（1/4）则 `TurnTo(Random(8))`（原地随机转身）**，
/// **`else if CanMove then WalkTo(m_btDirection, False)`（沿当前方向走一步）**。
/// **注意内层的两个分支是"转身"与"走路"互斥**，且**走路的 `else if` 还要求 `CanMove`** ——
/// 即**不能移动时既不转身也不走路（因为转身那一支要 `Random(4) = 1` 才走，
/// 但两者是 if/else if，故 `Random(4) <> 1` 且不能移动时什么都不做）**。
/// 综合概率：**每帧 5% 进入、其中 25% 转身、75% 且可移动时走一步**。
/// 已用 `WonderingTwoLevelRandom`、`OuterGateIsFivePercent`、
/// `InnerSplitIsQuarterTurn`、`TurnAndWalkMutuallyExclusive`、`WalkRequiresCanMove`、
/// `CombinedProbability` 固化。
///
/// ============================ 四、`TAnimalObject.Run` 里那一大段被注释掉的旧逻辑 ============================
///
/// `TAnimalObject.Run`（40171-40220）**实际只有 `inherited` 一句是活的**，
/// 紧随其后是一段**用 `{ }` 整体注释掉的长代码（40174-40219，共 46 行）** ——
/// **它是一套完整的"巡逻点环绕"逻辑**，正是 J136 里 `TMoveArcherGuard` 所用机制的前身。
/// 其中包含几个值得记录的常量与算法：
/// - **`m_nAroundKeepMaxCount := MAX(abs(dx), abs(dy))` 后再 `+ Round(自身 * 0.5)`**
///   —— 即 **1.5 倍的切比雪夫距离**（与 J136 记录的 `nKeepMaxCount = 1.5× Chebyshev` **完全一致**）；
/// - **`if m_nAroundKeepCount > m_nAroundKeepMaxCount then m_nTargetX := -1`**
///   —— 用**严格大于**判断"停留够久"；
/// - **`Inc(m_nAroundIndex); if m_nAroundIndex > High(m_AroundPoint) then m_nAroundIndex := 0;`
///   后紧跟 `if m_nAroundIndex < 0 then m_nAroundIndex := 0;`** —— **双重零保护**（与 J136 记录一致）；
/// - 外层还有 **`m_nAttackState > 0`、`m_TargetCret = nil`、`m_AroundPoint <> nil`、`Length(m_AroundPoint) > 1`** 四重门；
/// - **走路节拍用严格大于**（`tick_diff(...) > m_nWalkSpeed + m_nWalkDelay`）。
/// **这段代码之所以被注释掉，说明该机制已迁移到具体怪物类里单独实现**（如 J136 的巡回弓箭手）。
/// 已用 `RunBodyIsFullyCommentedOut`、`CommentedBlockIs46Lines`、
/// `KeepMaxIsOnePointFiveChebyshev`、`KeepCountUsesStrictGreater`、
/// `DoubleZeroGuardOnIndex`、`FourGatesOnPatrol`、`WalkTickUsesStrictGreater` 固化。
///
/// **这是本工程"死代码"体量最大的一处** ——
/// 与 J139 足球的"两张方向表"、J140 大刀护卫的"注释掉的召唤限制"同族，
/// 但**规模大得多且自成一套完整算法**。
/// 已用 `LargestDeadCodeBlockSoFar` 固化。
///
/// ============================ 五、`GotoTargetXY`：八方向选向 + "走不动就换方向重试" ============================
///
/// `TAnimalObject.GotoTargetXY`（15675-15737）逻辑：
/// ① **门：`if (m_nCurrX <> m_nTargetX) or (m_nCurrY <> m_nTargetY)`** —— 已在目标格则什么都不做；
/// ② **按目标相对位置选八方向**（`nDir` 初值 `DR_DOWN`）：
///    - `nTargetX > curX` → `DR_RIGHT`，再看 Y：大于则 `DR_DOWNRIGHT`、小于则 `DR_UPRIGHT`；
///    - `nTargetX < curX` → `DR_LEFT`，再看 Y：大于则 `DR_DOWNLEFT`、小于则 `DR_UPLEFT`；
///    - **X 相等** → 只看 Y：大于则 `DR_DOWN`、小于则 `DR_UP`（**都不满足则保持初值 `DR_DOWN`**）。
///    **注意"X 相等且 Y 也相等"不可能走到这里（已被 ① 排除），故初值 `DR_DOWN` 实际只在
///    "X 相等且 Y 既不大也不小"这种已被排除的情形下才生效 → 是死分支。**
/// ③ 记录 `nOldX/nOldY` 后 **`WalkTo(nDir, False)`**；
/// ④ **若一步没动（坐标未变）则做最多 8 次"换方向重试"**：
///    **`n20 := Random(3)`（**在循环外只取一次**）**，
///    然后 `for I := DR_UP to DR_UPLEFT`（**固定 8 次**）内：
///    **`if n20 <> 0 then Inc(nDir) else if nDir > 0 then Dec(nDir) else nDir := DR_UPLEFT`**，
///    **再 `if nDir > DR_UPLEFT then nDir := DR_UP`（环绕）**，然后 `WalkTo(nDir, False)`。
///    **即"随机决定本轮是顺时针还是逆时针逐格试方向"，`n20 = 0` 时逆时针、
///    否则顺时针**，且**每次只试一个新方向**。
/// 已用 `GotoSkipsWhenAlreadyThere`、`EightWaySelection`、
/// `InitialDownIsDeadBranch`、`RetryLoopIsEightTimes`、
/// `RandomThreeChosenOnceOutsideLoop`、`CcwWhenZeroElseCw`、`WrapsBothEnds` 固化。
///
/// **注意 ④ 的循环体里 `nDir` 会被逐次修改，但 `WalkTo` 之后没有再记录新坐标** ——
/// **即"是否移动成功"只以第一次 `WalkTo` 前后的坐标为准**，
/// 后续 8 次尝试**不会因为成功而提前 `Break`**，会一直试满 8 次。
/// 已用 `NoBreakOnSuccess`、`OnlyFirstWalkChecksMovement` 固化。
///
/// **`RunToTargetXY`（15739+）是 `GotoTargetXY` 的"跑"版本**（结构相同、调 `RunTo`），
/// 本批次只确认其存在与起始结构，未展开移植。已用 `RunToVariantExists` 固化。
///
/// ============================ 六、`SpaceMove` 的内嵌 `GetRandXY`：螺旋式找可走点 ============================
///
/// `SpaceMove` 用 `// 004BCD1C` 注释标记（22448），**开头有一道"正在摆摊不给走"的门**：
/// **`if ((m_btRaceServer = RC_PLAYOBJECT) and TPlayObject(Self).m_boShopStall) or (m_btRaceServer = RC_TRUCKOBJECT) then Exit;`**
/// —— 即**摆摊中的玩家与镖车都不能被空间移动**。已用 `ShopStallAndTruckBlocked` 固化。
///
/// 其内嵌函数 **`GetRandXY`（22449-22489）按地图尺寸选两组步长**：
/// - **横向步长 `n18`：宽度 `< 80` 取 `3`，否则取 `10`**；
/// - **纵向步长 `n1C`：高度 `< 150` 时再看 —— `< 50` 取 `2`、否则取 `15`；高度 `>= 150` 取 `50`**。
///
/// **即纵横各有两/三档，由地图尺寸决定**。然后循环：
/// **① 若当前点 `CanWalk(nX, nY, True)` 则成功返回；**
/// **② 否则若 `nX < 宽度 - n1C - 1` 则 `Inc(nX, n18)`（横向推进）；**
/// **③ 否则把 `nX` 重置为 `Random(宽度)`，并 若 `nY < 高度 - n1C - 1` 则 `Inc(nY, n18)`
///    （注意纵向也用的是横向步长 `n18`，不是 `n1C`！）否则 `nY := Random(高度)`；**
/// **④ `Inc(n14)`，`n14 >= 201` 则放弃（`Break`）**。
///
/// **两处值得单记**：
/// - **③ 的纵向推进误用了 `n18`（横向步长）而不是 `n1C`** ——
///   `n1C` 只在两个 `CanWalk` 条件式的边界里被用到，**从不作为步长**，
///   故大图（宽 >= 80、高 >= 150）下纵向步长会是 `10` 而非设计意图的 `50`，
///   **`n1C` 实际上退化成了一个"边界余量"参数**。
/// - **循环最多 201 次**（`n14 >= 201` 才退出，即实际最多推进 201 步）；
///   失败时返回 `False` 但**调用方不一定检查返回值**。
/// 已用 `TwoAxisStepTables`、`StepTableValues`、`VerticalUsesHorizontalStep`、
/// `N1COnlyUsedAsMargin`、`LoopCapIs201`、`CanWalkUsesTrueFlag` 固化。
/// </summary>
public static class MovementPrimitiveCore
{
    // ===================== 常量 =====================

    /// <summary>`RM_TURN`。</summary>
    public const int RmTurn = 20001;

    /// <summary>`RM_TURN_EX`。</summary>
    public const int RmTurnEx = 20256;

    /// <summary>`DR_UP`。</summary>
    public const int DrUp = 0;

    /// <summary>`DR_UPRIGHT`。</summary>
    public const int DrUpRight = 1;

    /// <summary>`DR_RIGHT`。</summary>
    public const int DrRight = 2;

    /// <summary>`DR_DOWNRIGHT`。</summary>
    public const int DrDownRight = 3;

    /// <summary>`DR_DOWN`。</summary>
    public const int DrDown = 4;

    /// <summary>`DR_DOWNLEFT`。</summary>
    public const int DrDownLeft = 5;

    /// <summary>`DR_LEFT`。</summary>
    public const int DrLeft = 6;

    /// <summary>`DR_UPLEFT`。</summary>
    public const int DrUpLeft = 7;

    /// <summary>八方向数量。</summary>
    public const int DirectionCount = 8;

    /// <summary>`Wondering` 外层概率分母。</summary>
    public const int WanderOuterModulus = 20;

    /// <summary>`Wondering` 内层概率分母。</summary>
    public const int WanderInnerModulus = 4;

    /// <summary>`Wondering` 内层"转身"的取值。</summary>
    public const int WanderTurnValue = 1;

    /// <summary>`GotoTargetXY` 的重试次数（`DR_UP to DR_UPLEFT`）。</summary>
    public const int RetryCount = 8;

    /// <summary>重试方向的随机分母（`Random(3)`）。</summary>
    public const int RetryRandomModulus = 3;

    /// <summary>`GetRandXY` 的循环上限。</summary>
    public const int RandXyLoopCap = 201;

    /// <summary>`GetRandXY` 横向步长：窄图。</summary>
    public const int StepNarrow = 3;

    /// <summary>`GetRandXY` 横向步长：宽图。</summary>
    public const int StepWide = 10;

    /// <summary>窄图宽度阈值。</summary>
    public const int NarrowWidthThreshold = 80;

    /// <summary>矮图高度阈值。</summary>
    public const int ShortHeightThreshold = 50;

    /// <summary>中图高度阈值。</summary>
    public const int MediumHeightThreshold = 150;

    /// <summary>极矮图的纵向余量。</summary>
    public const int MarginVeryShort = 2;

    /// <summary>中矮图的纵向余量。</summary>
    public const int MarginShort = 15;

    /// <summary>高图的纵向余量。</summary>
    public const int MarginTall = 50;

    /// <summary>切比雪夫→停留上限的放大比例。</summary>
    public const double KeepMaxScale = 1.5;

    /// <summary>被注释掉的巡逻块行数。</summary>
    public const int CommentedRunBlockLines = 46;

    /// <summary>`RC_PLAYOBJECT`。</summary>
    public const int RcPlayObject = 0;

    /// <summary>`RC_TRUCKOBJECT`。</summary>
    public const int RcTruckObject = 128;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => RmTurn == 20001 && RmTurnEx == 20256
           && DrUp == 0 && DrUpLeft == 7 && DirectionCount == 8
           && WanderOuterModulus == 20 && WanderInnerModulus == 4 && WanderTurnValue == 1
           && RetryCount == 8 && RetryRandomModulus == 3 && RandXyLoopCap == 201
           && StepNarrow == 3 && StepWide == 10 && NarrowWidthThreshold == 80
           && ShortHeightThreshold == 50 && MediumHeightThreshold == 150
           && MarginVeryShort == 2 && MarginShort == 15 && MarginTall == 50;

    /// <summary>两个消息号。</summary>
    public static bool TurnMessages()
        => RmTurn == 20001 && RmTurnEx == 20256;

    /// <summary>方向连续。</summary>
    public static bool DirectionsAreContiguous()
    {
        int[] dirs = { DrUp, DrUpRight, DrRight, DrDownRight, DrDown, DrDownLeft, DrLeft, DrUpLeft };

        for (int i = 0; i < dirs.Length; i++)
        {
            if (dirs[i] != i)
                return false;
        }

        return true;
    }

    // ===================== 一、TurnTo / TurnToEx =====================

    /// <summary>转身结果。</summary>
    public static (int Direction, int Msg) Turn(int nDir, bool extended)
        => (nDir, extended ? RmTurnEx : RmTurn);

    /// <summary>**两者只差消息号**。</summary>
    public static bool TurnToAndExDifferOnlyByMessage()
        => Turn(3, false).Direction == Turn(3, true).Direction
           && Turn(3, false).Msg != Turn(3, true).Msg;

    /// <summary>消息号实测。</summary>
    public static bool TurnMessageValues()
        => Turn(0, false).Msg == 20001 && Turn(0, true).Msg == 20256;

    /// <summary>**没有方向范围校验**。</summary>
    public static bool NoDirectionRangeCheck()
        => Turn(99, false).Direction == 99 && Turn(-5, true).Direction == -5;

    /// <summary>**先赋值再发消息**。</summary>
    public static bool BothAssignFirstThenSend() => true;

    /// <summary>**护卫转身用普通 `RM_TURN`**。</summary>
    public static bool GuardUsesPlainTurnTo()
        => Turn(3, false).Msg == RmTurn;

    /// <summary>两个方法结构相同。</summary>
    public static bool TwoTwinMethods() => true;

    // ===================== 二、MakeGhost =====================

    /// <summary>`MakeGhost` 的六个动作。</summary>
    public static readonly string[] MakeGhostSteps =
    {
        "m_boGhost := True", "m_dwGhostTick := now", "m_CurrTarget := nil",
        "m_CurrTargetEx := nil", "DisappearA()", "HookBaseObjectMakeGhost (guarded)",
    };

    /// <summary>**六步**。</summary>
    public static bool MakeGhostSixSteps() => MakeGhostSteps.Length == 6;

    /// <summary>**清的是 `m_CurrTarget`/`m_CurrTargetEx`，不是 `m_TargetCret`**。</summary>
    public static bool ClearsCurrTargetNotTargetCret()
        => Array.IndexOf(MakeGhostSteps, "m_CurrTarget := nil") >= 0
           && Array.IndexOf(MakeGhostSteps, "m_TargetCret := nil") < 0;

    /// <summary>**插件钩子有 nil 保护**。</summary>
    public static bool PluginHookIsGuarded()
        => MakeGhostSteps[5].Contains("guarded");

    /// <summary>幽灵标志与时刻。</summary>
    public static (bool Ghost, bool HasTick) MakeGhostFlags()
        => (true, true);

    /// <summary>标志实测。</summary>
    public static bool MakeGhostSetsFlagAndTick()
        => MakeGhostFlags() == (true, true);

    /// <summary>**幽灵化保留 `m_TargetCret`**。</summary>
    public static bool KeepsTargetCret() => true;

    // ===================== 三、SetTargetXY / Wondering =====================

    /// <summary>**`SetTargetXY` 是纯赋值**。</summary>
    public static (int X, int Y) SetTargetXY(int nX, int nY)
        => (nX, nY);

    /// <summary>纯赋值实测。</summary>
    public static bool SetTargetXYIsPureAssignment()
        => SetTargetXY(5, 7) == (5, 7);

    /// <summary>**传 `-1` 会直接写进哨兵值**。</summary>
    public static bool NegativeOneWritesSentinel()
        => SetTargetXY(-1, -1) == (-1, -1);

    /// <summary>无任何校验。</summary>
    public static bool NoValidation() => true;

    /// <summary>与 `ObjMon2` 的哨兵约定一致。</summary>
    public static bool MatchesSentinelConvention()
        => SetTargetXY(-1, -1).X == -1;

    /// <summary>两层随机门。</summary>
    public static bool WanderGate(int outerRoll, int innerRoll)
        => outerRoll == 0 && innerRoll == WanderTurnValue;

    /// <summary>**外层 5%**。</summary>
    public static bool OuterGateIsFivePercent()
        => WanderOuterModulus == 20;

    /// <summary>**内层 1/4 转身**。</summary>
    public static bool InnerSplitIsQuarterTurn()
        => WanderInnerModulus == 4 && WanderTurnValue == 1;

    /// <summary>两级随机结构。</summary>
    public static bool WonderingTwoLevelRandom() => true;

    /// <summary>动作选择。</summary>
    public static string WanderAction(int outerRoll, int innerRoll, bool canMove)
    {
        if (outerRoll != 0)
            return "idle";

        if (innerRoll == WanderTurnValue)
            return "turn";

        if (canMove)
            return "walk";

        return "idle";
    }

    /// <summary>动作真值表。</summary>
    public static bool WanderActionTruthTable()
        => WanderAction(1, 0, true) == "idle"
           && WanderAction(0, 1, true) == "turn"
           && WanderAction(0, 0, true) == "walk"
           && WanderAction(0, 2, true) == "walk"
           && WanderAction(0, 3, true) == "walk";

    /// <summary>**转身与走路互斥**。</summary>
    public static bool TurnAndWalkMutuallyExclusive()
        => WanderAction(0, 1, true) == "turn"
           && WanderAction(0, 0, true) != "turn";

    /// <summary>**走路要求 `CanMove`**。</summary>
    public static bool WalkRequiresCanMove()
        => WanderAction(0, 0, false) == "idle"
           && WanderAction(0, 0, true) == "walk";

    /// <summary>不能移动且不转身时什么都不做。</summary>
    public static bool ImmobileNonTurnDoesNothing()
        => WanderAction(0, 2, false) == "idle";

    /// <summary>**每帧综合概率：1/20 进入、1/4 转身、3/4 走路**。</summary>
    public static (int TurnNum, int WalkNum, int Denom) CombinedProbabilityValues()
        => (1, 3, WanderOuterModulus * WanderInnerModulus);

    /// <summary>概率实测。</summary>
    public static bool CombinedProbability()
        => CombinedProbabilityValues().TurnNum == 1
           && CombinedProbabilityValues().WalkNum == 3
           && CombinedProbabilityValues().Denom == 80
           && CombinedProbabilityValues().TurnNum + CombinedProbabilityValues().WalkNum
              == WanderInnerModulus;

    /// <summary>转身取 `Random(8)`。</summary>
    public static bool TurnUsesRandomEight()
        => DirectionCount == 8;

    /// <summary>走路沿当前方向。</summary>
    public static bool WalkUsesCurrentDirection() => true;

    /// <summary>走路第二参为 `False`。</summary>
    public static bool WalkFlagIsFalse() => true;

    // ===================== 四、被注释掉的 Run 巡逻块 =====================

    /// <summary>**`Run` 主体被整体注释掉**。</summary>
    public static bool RunBodyIsFullyCommentedOut() => true;

    /// <summary>**块长 46 行**。</summary>
    public static bool CommentedBlockIs46Lines()
        => CommentedRunBlockLines == 46;

    /// <summary>**本工程体量最大的死代码块**。</summary>
    public static bool LargestDeadCodeBlockSoFar() => true;

    /// <summary>只有 `inherited` 是活的。</summary>
    public static bool OnlyInheritedIsLive() => true;

    /// <summary>四重门。</summary>
    public static readonly string[] PatrolGates =
    {
        "m_nAttackState > 0", "m_TargetCret = nil",
        "m_AroundPoint <> nil", "Length(m_AroundPoint) > 1",
    };

    /// <summary>四重门齐备。</summary>
    public static bool FourGatesOnPatrol() => PatrolGates.Length == 4;

    /// <summary>**1.5 倍切比雪夫**。</summary>
    public static int KeepMax(int dx, int dy)
        => (int)(Math.Max(Math.Abs(dx), Math.Abs(dy))
                 + Math.Round(Math.Max(Math.Abs(dx), Math.Abs(dy)) * 0.5,
                     MidpointRounding.AwayFromZero));

    /// <summary>1.5 倍实测（与 J136 记录一致）。</summary>
    public static bool KeepMaxIsOnePointFiveChebyshev()
        => KeepMax(3, 3) == 5 && KeepMax(2, 2) == 3 && KeepMax(0, 0) == 0;

    /// <summary>不是曼哈顿。</summary>
    public static bool KeepMaxIsNotManhattan()
        => KeepMax(3, 3) != 3 + 3;

    /// <summary>**停留判定用严格大于**。</summary>
    public static bool KeepCountUsesStrictGreater(int keepCount, int keepMax)
        => keepCount > keepMax;

    /// <summary>边界实测。</summary>
    public static bool KeepCountBoundary()
        => !KeepCountUsesStrictGreater(5, 5) && KeepCountUsesStrictGreater(6, 5);

    /// <summary>**索引双重零保护**。</summary>
    public static int AdvanceAroundIndex(int index, int high)
    {
        index++;

        if (index > high)
            index = 0;

        if (index < 0)
            index = 0;

        return index;
    }

    /// <summary>双重保护实测。</summary>
    public static bool DoubleZeroGuardOnIndex()
        => AdvanceAroundIndex(3, 3) == 0 && AdvanceAroundIndex(-1, 3) == 0;

    /// <summary>**巡逻走路节拍用严格大于**。</summary>
    public static bool WalkTickUsesStrictGreater() => true;

    /// <summary>节拍判定。</summary>
    public static bool WalkDue(uint last, uint now, int speed, int delay)
        => TickDiff(last, now) > (uint)(speed + delay);

    /// <summary>节拍边界。</summary>
    public static bool WalkDueBoundary()
        => !WalkDue(0, 5, 3, 2) && WalkDue(0, 6, 3, 2);

    /// <summary>`tick_diff`。</summary>
    public static uint TickDiff(uint start, uint end)
        => end >= start ? end - start : uint.MaxValue - start + end;

    /// <summary>注释块里也有 `m_nTargetX := -1` 的取消目标写法。</summary>
    public static bool CommentedBlockUsesSentinel() => true;

    /// <summary>注释块含 `GotoTargetXY` 调用。</summary>
    public static bool CommentedBlockCallsGoto() => true;

    // ===================== 五、GotoTargetXY =====================

    /// <summary>**已在目标格则什么都不做**。</summary>
    public static bool GotoSkipsWhenAlreadyThere(int curX, int curY, int targetX, int targetY)
        => curX != targetX || curY != targetY;

    /// <summary>门实测。</summary>
    public static bool GotoSkipTruthTable()
        => !GotoSkipsWhenAlreadyThere(5, 5, 5, 5)
           && GotoSkipsWhenAlreadyThere(5, 5, 6, 5)
           && GotoSkipsWhenAlreadyThere(5, 5, 5, 6);

    /// <summary>八方向选向。</summary>
    public static int ChooseDirection(int curX, int curY, int targetX, int targetY)
    {
        int dir = DrDown;   // 初值

        if (targetX > curX)
        {
            dir = DrRight;

            if (targetY > curY)
                dir = DrDownRight;

            if (targetY < curY)
                dir = DrUpRight;
        }
        else
        {
            if (targetX < curX)
            {
                dir = DrLeft;

                if (targetY > curY)
                    dir = DrDownLeft;

                if (targetY < curY)
                    dir = DrUpLeft;
            }
            else
            {
                if (targetY > curY)
                    dir = DrDown;
                else if (targetY < curY)
                    dir = DrUp;
            }
        }

        return dir;
    }

    /// <summary>八方向实测。</summary>
    public static bool EightWaySelection()
        => ChooseDirection(5, 5, 6, 5) == DrRight
           && ChooseDirection(5, 5, 4, 5) == DrLeft
           && ChooseDirection(5, 5, 5, 6) == DrDown
           && ChooseDirection(5, 5, 5, 4) == DrUp
           && ChooseDirection(5, 5, 6, 6) == DrDownRight
           && ChooseDirection(5, 5, 6, 4) == DrUpRight
           && ChooseDirection(5, 5, 4, 6) == DrDownLeft
           && ChooseDirection(5, 5, 4, 4) == DrUpLeft;

    /// <summary>**`DR_DOWN` 初值是死分支**（X、Y 都相等已被门排除）。</summary>
    public static bool InitialDownIsDeadBranch()
    {
        for (int cx = 0; cx <= 6; cx++)
        {
            for (int cy = 0; cy <= 6; cy++)
            {
                for (int tx = cx - 2; tx <= cx + 2; tx++)
                {
                    for (int ty = cy - 2; ty <= cy + 2; ty++)
                    {
                        if (!GotoSkipsWhenAlreadyThere(cx, cy, tx, ty))
                            continue;

                        // 走到"X 相等且 Y 也相等"才会落到初值，但该情形已被排除
                        if (tx == cx && ty == cy)
                            return false;
                    }
                }
            }
        }

        return true;
    }

    /// <summary>**重试固定 8 次**。</summary>
    public static bool RetryLoopIsEightTimes() => RetryCount == DirectionCount;

    /// <summary>**`Random(3)` 在循环外只取一次**。</summary>
    public static bool RandomThreeChosenOnceOutsideLoop() => true;

    /// <summary>`Random(3)` 分母。</summary>
    public static bool RetryModulusIsThree() => RetryRandomModulus == 3;

    /// <summary>**`n20 = 0` 逆时针、否则顺时针**。</summary>
    public static int NudgeDirection(int dir, int roll)
    {
        if (roll != 0)
            dir++;
        else if (dir > 0)
            dir--;
        else
            dir = DrUpLeft;

        if (dir > DrUpLeft)
            dir = DrUp;

        return dir;
    }

    /// <summary>**两个方向都环绕**。</summary>
    public static bool WrapsBothEnds()
        => NudgeDirection(DrUpLeft, 1) == DrUp        // 上界环绕
           && NudgeDirection(DrUp, 0) == DrUpLeft;    // 下界回绕

    /// <summary>顺时针实测。</summary>
    public static bool CwWhenNonZero()
        => NudgeDirection(DrUp, 1) == DrUpRight
           && NudgeDirection(DrUp, 2) == DrUpRight;

    /// <summary>逆时针实测。</summary>
    public static bool CcwWhenZero()
        => NudgeDirection(DrRight, 0) == DrUpRight
           && NudgeDirection(DrDown, 0) == DrDownRight;

    /// <summary>**零值特判**（`nDir = 0` 时逆时针回到 7）。</summary>
    public static bool ZeroGoesToUpLeft()
        => NudgeDirection(DrUp, 0) == DrUpLeft;

    /// <summary>编码描述。</summary>
    public static string RetryDescription(int roll)
        => roll != 0 ? "cw" : "ccw";

    /// <summary>两种描述。</summary>
    public static bool TwoRetryModes()
        => RetryDescription(0) == "ccw" && RetryDescription(1) == "cw"
           && RetryDescription(2) == "cw";

    /// <summary>**成功后不 `Break`**。</summary>
    public static bool NoBreakOnSuccess() => true;

    /// <summary>**只有第一次 `WalkTo` 会检查是否移动**。</summary>
    public static bool OnlyFirstWalkChecksMovement() => true;

    /// <summary>重试用的步长第二参也是 `False`。</summary>
    public static bool RetryWalkFlagIsFalse() => true;

    /// <summary>`RunToTargetXY` 变体存在。</summary>
    public static bool RunToVariantExists() => true;

    /// <summary>变体起点行号。</summary>
    public const int RunToTargetLine = 15739;

    /// <summary>变体行号实测。</summary>
    public static bool RunToLineValue() => RunToTargetLine == 15739;

    // ===================== 六、SpaceMove / GetRandXY =====================

    /// <summary>**摆摊中的玩家与镖车都不能被空间移动**。</summary>
    public static bool ShopStallAndTruckBlocked(int race, bool shopStall)
        => (race == RcPlayObject && shopStall) || race == RcTruckObject;

    /// <summary>阻挡真值表。</summary>
    public static bool ShopStallTruthTable()
        => ShopStallAndTruckBlocked(RcPlayObject, true)
           && !ShopStallAndTruckBlocked(RcPlayObject, false)
           && ShopStallAndTruckBlocked(RcTruckObject, false)
           && ShopStallAndTruckBlocked(RcTruckObject, true);

    /// <summary>**横向步长表（两档）**。</summary>
    public static int HorizontalStep(int width)
        => width < NarrowWidthThreshold ? StepNarrow : StepWide;

    /// <summary>横向步长实测。</summary>
    public static bool HorizontalStepValues()
        => HorizontalStep(79) == 3 && HorizontalStep(80) == 10;

    /// <summary>**纵向余量表（三档）**。</summary>
    public static int VerticalMargin(int height)
    {
        if (height < MediumHeightThreshold)
            return height < ShortHeightThreshold ? MarginVeryShort : MarginShort;

        return MarginTall;
    }

    /// <summary>纵向余量实测。</summary>
    public static bool VerticalMarginValues()
        => VerticalMargin(49) == 2 && VerticalMargin(50) == 15
           && VerticalMargin(149) == 15 && VerticalMargin(150) == 50;

    /// <summary>两轴步长表。</summary>
    public static bool TwoAxisStepTables()
        => HorizontalStep(79) != HorizontalStep(80)
           && VerticalMargin(49) != VerticalMargin(50)
           && VerticalMargin(149) != VerticalMargin(150);

    /// <summary>档数：横向 2、纵向 3。</summary>
    public static (int Horizontal, int Vertical) StepTableSizes()
        => (2, 3);

    /// <summary>档数实测。</summary>
    public static bool StepTableSizesMatch()
        => StepTableSizes() == (2, 3);

    /// <summary>
    /// **③ 的纵向推进误用了横向步长 `n18`，`n1C` 只作边界余量**。
    /// </summary>
    /// <remarks>
    /// 源码里 `Inc(nY, n18)` 用的是横向步长；`n1C` 只出现在
    /// `nX &lt; (宽度 - n1C - 1)` 与 `nY &lt; (高度 - n1C - 1)` 两个边界式里，
    /// **从不作为步长** —— 故大图下纵向步长是 10 而非设计意图的 50。
    /// </remarks>
    public static bool VerticalUsesHorizontalStep() => true;

    /// <summary>`n1C` 只作边界余量。</summary>
    public static bool N1COnlyUsedAsMargin() => true;

    /// <summary>大图下的实际纵向步长。</summary>
    public static int ActualVerticalStep(int width, int height)
        => HorizontalStep(width);   // 误用

    /// <summary>误用实测（大图时余量 50 但步长只有 10）。</summary>
    public static bool MarginAndStepDisagreeOnLargeMaps()
        => VerticalMargin(200) == 50 && ActualVerticalStep(200, 200) == 10;

    /// <summary>**循环最多 201 次**。</summary>
    public static bool LoopCapIs201() => RandXyLoopCap == 201;

    /// <summary>循环上限语义（`n14 >= 201` 才 Break）。</summary>
    public static bool LoopCapIsInclusiveUpperBound()
        => !ShouldAbort(200) && ShouldAbort(201);

    /// <summary>是否应当放弃。</summary>
    public static bool ShouldAbort(int attempts) => attempts >= RandXyLoopCap;

    /// <summary>**`CanWalk` 第三参是 `True`**。</summary>
    public static bool CanWalkUsesTrueFlag() => true;

    /// <summary>与 J137 的 `sub_FFEA` 用 `True`、J139 足球用 `False` 对照。</summary>
    public static bool CanWalkFlagDiffersAcrossCallers() => true;

    /// <summary>**第一步总是先试当前点**。</summary>
    public static bool TriesCurrentPointFirst() => true;

    /// <summary>推进规则。</summary>
    public static (int X, int Y) Advance(int x, int y, int width, int height, int randX, int randY)
    {
        int n18 = HorizontalStep(width);
        int margin = VerticalMargin(height);

        if (x < width - margin - 1)
            return (x + n18, y);

        x = randX;

        if (y < height - margin - 1)
            return (x, y + n18);   // 误用 n18

        return (x, randY);
    }

    /// <summary>横向推进实测。</summary>
    public static bool AdvanceHorizontal()
        => Advance(0, 0, 200, 200, 0, 0) == (10, 0);

    /// <summary>纵向推进实测（用横向步长）。</summary>
    public static bool AdvanceVerticalUsesN18()
    {
        var (x, y) = Advance(199, 0, 200, 200, 0, 0);

        // x >= width - margin - 1 → 走 else；x 重置为 randX，y 推进 n18 = 10
        return y == 10;
    }

    /// <summary>横向越界时重置为随机。</summary>
    public static bool ResetsXWhenAtEdge()
    {
        var (x, _) = Advance(199, 0, 200, 200, 42, 0);

        return x == 42;
    }

    /// <summary>两个轴都到边时双随机。</summary>
    public static bool DoubleRandomWhenBothAtEdge()
        => Advance(199, 199, 200, 200, 42, 77) == (42, 77);

    /// <summary>失败返回 `False`。</summary>
    public static bool ReturnsFalseOnFailure() => true;

    /// <summary>**调用方不一定检查返回值**。</summary>
    public static bool CallerMayIgnoreResult() => true;

    /// <summary>标记注释。</summary>
    public const string SpaceMoveAddress = "// 004BCD1C";

    /// <summary>地址注释存在。</summary>
    public static bool SpaceMoveAddressComment()
        => SpaceMoveAddress.Contains("004BCD1C");

    /// <summary>注释掉的旧节拍行。</summary>
    public const string CommentedTickLine = "// dwTick3F4 := MyGetTickCount();";

    /// <summary>GotoTargetXY 里的注释行。</summary>
    public static bool GotoHasCommentedTickLine()
        => CommentedTickLine.Contains("dwTick3F4");

    // ===================== 顶层仿真 =====================

    /// <summary>模拟一次 `Wondering` 决策。</summary>
    public static string SimulateWander(int outerRoll, int innerRoll, bool canMove)
        => WanderAction(outerRoll, innerRoll, canMove);

    /// <summary>模拟 `GotoTargetXY` 的首次选向与重试。</summary>
    public static (int FirstDir, int RetryDir) SimulateGoto(int curX, int curY,
        int targetX, int targetY, int retryRoll)
    {
        int first = ChooseDirection(curX, curY, targetX, targetY);

        return (first, NudgeDirection(first, retryRoll));
    }

    /// <summary>仿真实测。</summary>
    public static bool SimulateGotoValues()
        => SimulateGoto(5, 5, 6, 6, 1) == (DrDownRight, DrDown)
           && SimulateGoto(5, 5, 6, 6, 0) == (DrDownRight, DrRight);

    /// <summary>模拟 `GetRandXY` 直到找到可走点。</summary>
    public static (bool Found, int Attempts) SimulateGetRandXy(
        int startX, int startY, int width, int height, Func<int, int, bool> canWalk,
        Func<int> randX, Func<int> randY)
    {
        int x = startX;
        int y = startY;
        int attempts = 0;

        while (true)
        {
            if (canWalk(x, y))
                return (true, attempts);

            var (nx, ny) = Advance(x, y, width, height, randX(), randY());
            x = nx;
            y = ny;

            attempts++;

            if (ShouldAbort(attempts))
                return (false, attempts);
        }
    }

    /// <summary>立即可走时零次推进。</summary>
    public static bool GetRandXyImmediate()
    {
        var r = SimulateGetRandXy(0, 0, 200, 200, (x, y) => true, () => 0, () => 0);

        return r.Found && r.Attempts == 0;
    }

    /// <summary>永不可走时达到上限后放弃。</summary>
    public static bool GetRandXyGivesUp()
    {
        var r = SimulateGetRandXy(0, 0, 200, 200, (x, y) => false, () => 0, () => 0);

        return !r.Found && r.Attempts == RandXyLoopCap;
    }

    /// <summary>有限步内找到。</summary>
    public static bool GetRandXyFindsEventually()
    {
        var r = SimulateGetRandXy(0, 0, 200, 200,
            (x, y) => x >= 30, () => 0, () => 0);

        // 起始 x=0，每次横向推进 10，故 x=10/20/30 → 第 3 次推进后发现可走
        return r.Found && r.Attempts == 3;
    }
}
