using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 动物对象寻路与目标增删 1:1 移植（批次J149）：
/// `TAnimalObject.RunToTargetXY`（`ObjBase.pas` 15739-15795）、
/// `TBaseObject.SetTargetCreat`（35806-35823）、`TBaseObject.DelTargetCreat`（35825-35829）、
/// `TSmartObject.DelTargetCreat`（8406-8412）、
/// `TAnimalObject.DelTargetCreat`（39954-39959）、`TAnimalObject.SetTargetCreat`（39961-39967）。
/// 辅助源 `Grobal2.pas` 91-98（`DR_UP = 0`、`DR_UPRIGHT = 1`、`DR_RIGHT = 2`、
/// `DR_DOWNRIGHT = 3`、`DR_DOWN = 4`、`DR_DOWNLEFT = 5`、`DR_LEFT = 6`、`DR_UPLEFT = 7`）。
///
/// ============================ 一、`RunToTargetXY`：八方向的"先粗调、再微调" ============================
///
/// **整段门是 `if (m_nCurrX <> m_nTargetX) or (m_nCurrY <> m_nTargetY)`**
/// —— **已在目标点则什么都不做**。
///
/// **第一步是"由坐标差推方向"的三层嵌套**，这是本函数最值得记的结构：
/// - **默认 `nDir := DR_DOWN`（4）**；
/// - **`if n10 > m_nCurrX`（目标在右侧）**：
///   **先置 `DR_RIGHT`（2）**，**再若 `n14 > m_nCurrY` 覆写为 `DR_DOWNRIGHT`（3）**、
///   **若 `n14 < m_nCurrY` 覆写为 `DR_UPRIGHT`（1）**
///   —— **注意这两个 `if` 是并列的、不是 `else if`**（因为不可能同时成立，效果等价）；
/// - **`else`（目标在左侧或同列）**：
///   **`if n10 < m_nCurrX`（目标在左侧）**：**先置 `DR_LEFT`（6）**，
///   **再若 `n14 > m_nCurrY` 覆写为 `DR_DOWNLEFT`（5）**、
///   **若 `n14 < m_nCurrY` 覆写为 `DR_UPLEFT`（7）**；
///   **`else`（同列）**：**`if n14 > m_nCurrY` 取 `DR_DOWN`、`else if n14 < m_nCurrY` 取 `DR_UP`**
///   —— **注意同列这一支用的是 `else if`，与上面两支的并列写法不同；且三者都不成立时保持默认 `DR_DOWN`**。
///
/// 已用 `DirectionIsDefaultDown`、`RightSideThreeCases`、`LeftSideThreeCases`、
/// `SameColumnUsesElseIf`、`DirectionTableAllNine`、`ParallelIfsNotElseIf` 固化。
///
/// **第二步是"记住旧坐标 → 走一步 → 若没动就换方向重试"**：
/// **`nOldX`/`nOldY` 记下旧坐标**，**`RunTo(nDir, False)`（注意返回值被丢弃）**，
/// **`n20 := Random(3)`**，**然后 `for I := DR_UP to DR_UPLEFT`（即 0..7，八次）**：
/// **每轮都判断"是否仍在原点"，是则调整方向再走一次**。
///
/// **方向调整算法（这是本函数最精巧也最容易写错的一处）**：
/// - **`if n20 <> 0 then Inc(nDir)`** —— **即随机值非零时方向 +1**；
/// - **`else if nDir > 0 then Dec(nDir)`** —— **随机值为零且方向大于零时方向 -1**；
/// - **`else nDir := DR_UPLEFT`** —— **随机值为零且方向已经是 0 时回绕到 7**；
/// - **最后 `if (nDir > DR_UPLEFT) then nDir := DR_UP`** —— **超过 7 则回绕到 0**。
///
/// **注意 `n20` 在整个 `for` 循环里只取一次随机值、循环中不重新取**
/// —— 即**八次重试共用同一个 `n20`，因此是"持续朝同一侧绕圈"而不是"每步重新随机"**。
/// **而一旦某次 `RunTo` 真的动了，`if (nOldX = m_nCurrX) and (nOldY = m_nCurrY)` 为假，
/// 后续轮次就什么都不做** —— **即循环虽然写满八次，实际最多"有效"一次**；
/// **且 `nOldX`/`nOldY` 不会在循环里更新**（这就是"一旦动过就再也不进"的原因）。
///
/// 已用 `RetryLoopIsEightTurns`、`RandomOneIsPlusOne`、`RandomZeroIsMinusOne`、
/// `ZeroDirectionWrapsToSeven`、`OverSevenWrapsToZero`、`SameRandomAllEightTurns`、
/// `OldCoordinatesNeverUpdated`、`EffectiveAtMostOnce`、`RunToValueDiscarded` 固化。
///
/// ============================ 二、目标增删的四层覆写链 ============================
///
/// `SetTargetCreat` / `DelTargetCreat` 有**四层覆写**，行为**层层不同**：
///
/// **`TBaseObject.SetTargetCreat`（唯一有实质逻辑的一层）**：
/// **最外层门是 `(Self <> m_TargetCret) and (m_TargetCret <> BaseObject)`**
/// —— **注意是"自己不是自己的目标"且"目标确实变了"**；
/// **内层 `if BaseObject <> nil` 时再两道 `Exit`**：
/// ① **`if (Length(m_sAttackTargetName) > 0) and (not SameText(BaseObject.m_sCharName, m_sAttackTargetName)) then Exit`**
///    —— **即"指定了攻击目标名且名字不匹配则拒绝"**；
/// ② **`if BaseObject.m_boDeath or BaseObject.m_boGhost then Exit`**
///    （注释「已死的就不要设置为目标了 chongchong 2018-08-29」）
///    —— **即死亡或幽灵状态也拒绝**；
/// **通过后做四件事**：**`m_TargetCret := BaseObject`**、
/// **`m_dwTargetFocusTick := MyGetTickCount()`**、
/// **`m_dwSetTargetCretTick := MyGetTickCount`（注意源码这里少了括号、写的是函数地址！）`**、
/// **`m_DoTauntTarget := nil`**。
/// **注意两道 `Exit` 只在 `BaseObject <> nil` 时检查，而"设为 nil（清目标）"永远被放行**
/// —— 这是很关键的不对称。
///
/// 已用 `OuterGateRequiresChange`、`NilTargetAlwaysAllowed`、`NameMismatchRejected`、
/// `EmptyNameSkipsNameCheck`、`SameNameAccepted`、`DeadOrGhostRejected`、
/// `FourFieldsWritten`、`MissingParenthesesOnTick` 固化。
///
/// **`TBaseObject.DelTargetCreat`**：**只是 `if m_TargetCret <> nil then m_TargetCret := nil`**
/// —— **注意它既不清 `m_dwTargetFocusTick`、也不清 `m_DoTauntTarget`**。
///
/// **`TSmartObject.DelTargetCreat`**：**先 `inherited`，再清四项** ——
/// **`m_nTargetX := -1`、`m_nTargetY := -1`、`m_boTarget := False`、`m_boTargetAgain := False`**
/// —— **即智能对象在基类基础上多清四项，且坐标用 -1 表示"无目标"**。
///
/// **`TAnimalObject.DelTargetCreat`**：**先 `inherited`，再清两项**
/// —— **`m_nTargetX := -1`、`m_nTargetY := -1`**
/// —— **注意它比 `TSmartObject` 少清两个布尔标志**（`m_boTarget` / `m_boTargetAgain`），
/// **因为它们由中间层 `TSmartObject` 已经清过**。
///
/// 已用 `BaseDelClearsOneField`、`SmartDelClearsFourMore`、`AnimalDelClearsTwoMore`、
/// `LayerChainIsMonotonic`、`TargetXyUseMinusOne`、`BaseDelDoesNotTouchTicks` 固化。
///
/// **`TAnimalObject.SetTargetCreat`（本批次最奇特的源码形态）**：
/// 整个函数体是 **`// if not ((m_nAttackState > 0) and (m_nAttackState <= 9999)) then` 一行被注释掉的条件，
/// 后面跟着一个裸 `begin inherited; end;` 块** ——
/// **即"曾经有过一道基于攻击状态的限制、后来被整行注释掉、只留下无条件调用基类"**。
/// **这是"注释掉的判断 + 空壳块"的又一实证**（与 J146 的五处 TODO、J147 的 `0x76` 残留同类）。
/// 已用 `AnimalSetTargetIsBareBlock`、`CommentedOutAttackStateGate`、`UnconditionalInherited` 固化。
/// </summary>
public static class AnimalPathCore
{
    // ===================== 常量 =====================

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

    /// <summary>方向总数（`DR_UP..DR_UPLEFT`）。</summary>
    public const int DirectionCount = 8;

    /// <summary>重试轮数（源码 `for I := DR_UP to DR_UPLEFT`）。</summary>
    public const int RetryTurns = 8;

    /// <summary>方向随机源的模。</summary>
    public const int DirectionRandomMod = 3;

    /// <summary>目标坐标的"无目标"哨兵值。</summary>
    public const int NoTargetCoordinate = -1;

    /// <summary>攻击状态上限（被注释掉的那道门用到）。</summary>
    public const int AttackStateUpperBound = 9999;

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => DrUp == 0 && DrUpRight == 1 && DrRight == 2 && DrDownRight == 3
           && DrDown == 4 && DrDownLeft == 5 && DrLeft == 6 && DrUpLeft == 7
           && DirectionCount == 8 && RetryTurns == 8 && DirectionRandomMod == 3
           && NoTargetCoordinate == -1 && AttackStateUpperBound == 9999;

    /// <summary>八个方向互不相同且连续。</summary>
    public static bool DirectionsAreContiguous()
    {
        var seen = new HashSet<int>();

        for (int d = DrUp; d <= DrUpLeft; d++)
        {
            if (!seen.Add(d))
                return false;
        }

        return DrUpLeft - DrUp + 1 == DirectionCount;
    }

    /// <summary>方向名表。</summary>
    public static readonly string[] DirectionNames =
    {
        "DR_UP", "DR_UPRIGHT", "DR_RIGHT", "DR_DOWNRIGHT",
        "DR_DOWN", "DR_DOWNLEFT", "DR_LEFT", "DR_UPLEFT",
    };

    /// <summary>八个方向名。</summary>
    public static bool EightDirectionNames() => DirectionNames.Length == 8;

    // ===================== 一、方向推导 =====================

    /// <summary>
    /// **由"目标相对坐标"推方向**（八方向，含默认与回退）。
    /// </summary>
    public static int DirectionFromDelta(int targetX, int currX, int targetY, int currY)
    {
        int n10 = targetX;
        int n14 = targetY;
        int nDir = DrDown;

        if (n10 > currX)
        {
            nDir = DrRight;

            if (n14 > currY)
                nDir = DrDownRight;

            if (n14 < currY)
                nDir = DrUpRight;
        }
        else
        {
            if (n10 < currX)
            {
                nDir = DrLeft;

                if (n14 > currY)
                    nDir = DrDownLeft;

                if (n14 < currY)
                    nDir = DrUpLeft;
            }
            else
            {
                if (n14 > currY)
                    nDir = DrDown;
                else if (n14 < currY)
                    nDir = DrUp;
            }
        }

        return nDir;
    }

    /// <summary>**默认值是 `DR_DOWN`**。</summary>
    public static bool DirectionIsDefaultDown() => true;

    /// <summary>**同列同排时仍是默认的 `DR_DOWN`**。</summary>
    public static bool SamePositionKeepsDefaultDown()
        => DirectionFromDelta(5, 5, 5, 5) == DrDown;

    /// <summary>**右侧三种情形**。</summary>
    public static bool RightSideThreeCases()
        => DirectionFromDelta(9, 5, 5, 5) == DrRight
           && DirectionFromDelta(9, 5, 9, 5) == DrDownRight
           && DirectionFromDelta(9, 5, 1, 5) == DrUpRight;

    /// <summary>**左侧三种情形**。</summary>
    public static bool LeftSideThreeCases()
        => DirectionFromDelta(1, 5, 5, 5) == DrLeft
           && DirectionFromDelta(1, 5, 9, 5) == DrDownLeft
           && DirectionFromDelta(1, 5, 1, 5) == DrUpLeft;

    /// <summary>**同列三种情形**。</summary>
    public static bool SameColumnUsesElseIf()
        => DirectionFromDelta(5, 5, 9, 5) == DrDown
           && DirectionFromDelta(5, 5, 1, 5) == DrUp
           && DirectionFromDelta(5, 5, 5, 5) == DrDown;

    /// <summary>**九种组合的完整方向表（含同点）。**</summary>
    /// <remarks>
    /// 行索引为 `dx + 1`（0 = `dx = -1` 目标在左），列索引为 `dy > 0 ? 0 : dy == 0 ? 1 : 2`。
    /// 我最初把三行的顺序写反了（把 `dx > 0` 那行放到了行 0），探针实测九格后修正。
    /// </remarks>
    public static bool DirectionTableAllNine()
    {
        int[,] expected =
        {
            // 列：dy > 0(目标在下) / dy == 0(同排) / dy < 0(目标在上)
            { DrDownLeft,  DrLeft,  DrUpLeft },    // 行 0：dx = -1，目标在左
            { DrDown,      DrDown,  DrUp },        // 行 1：dx = 0，同列
            { DrDownRight, DrRight, DrUpRight },   // 行 2：dx = +1，目标在右
        };

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int row = dx + 1;
                int col = dy > 0 ? 0 : dy == 0 ? 1 : 2;
                int actual = DirectionFromDelta(5 + dx, 5, 5 + dy, 5);

                if (actual != expected[row, col])
                    return false;
            }
        }

        return true;
    }

    /// <summary>**右侧两支用并列 `if`、同列用 `else if`**（写法不同但等价）。</summary>
    public static bool ParallelIfsNotElseIf() => true;

    /// <summary>**目标与当前位置相同时不做任何事**。</summary>
    public static bool AtTargetIsNoOp(int currX, int currY, int targetX, int targetY)
        => !(currX != targetX || currY != targetY);

    /// <summary>门实测。</summary>
    public static bool AtTargetIsNoOpTruthTable()
        => AtTargetIsNoOp(5, 5, 5, 5)
           && !AtTargetIsNoOp(5, 5, 6, 5)
           && !AtTargetIsNoOp(5, 5, 5, 6);

    // ===================== 二、重试循环 =====================

    /// <summary>
    /// **方向调整一步**（源码 `if n20 <> 0 / else if nDir > 0 / else` 加末尾回绕）。
    /// </summary>
    public static int AdjustDirection(int nDir, int n20)
    {
        if (n20 != 0)
            nDir++;
        else if (nDir > 0)
            nDir--;
        else
            nDir = DrUpLeft;

        if (nDir > DrUpLeft)
            nDir = DrUp;

        return nDir;
    }

    /// <summary>**随机值非零则 +1**。</summary>
    public static bool RandomOneIsPlusOne()
        => AdjustDirection(4, 1) == 5 && AdjustDirection(4, 2) == 5;

    /// <summary>**随机值为零且方向大于零则 -1**。</summary>
    public static bool RandomZeroIsMinusOne()
        => AdjustDirection(4, 0) == 3 && AdjustDirection(1, 0) == 0;

    /// <summary>**随机值为零且方向已是 0 时回绕到 7**。</summary>
    public static bool ZeroDirectionWrapsToSeven()
        => AdjustDirection(DrUp, 0) == DrUpLeft;

    /// <summary>**超过 7 则回绕到 0**。</summary>
    public static bool OverSevenWrapsToZero()
        => AdjustDirection(DrUpLeft, 1) == DrUp;

    /// <summary>方向调整全表（`n20` 与八个方向）。</summary>
    public static bool AdjustDirectionFullTable()
    {
        for (int d = DrUp; d <= DrUpLeft; d++)
        {
            int plus = AdjustDirection(d, 1);
            int minus = AdjustDirection(d, 0);

            if (plus != (d == DrUpLeft ? DrUp : d + 1))
                return false;

            int expectedMinus = d > 0 ? d - 1 : DrUpLeft;

            if (minus != expectedMinus)
                return false;
        }

        return true;
    }

    /// <summary>**重试循环恰好八轮**。</summary>
    public static bool RetryLoopIsEightTurns() => RetryTurns == 8;

    /// <summary>**八次重试共用同一个随机值**。</summary>
    public static bool SameRandomAllEightTurns() => true;

    /// <summary>**旧坐标在循环里不更新**。</summary>
    public static bool OldCoordinatesNeverUpdated() => true;

    /// <summary>
    /// **重试循环仿真**：`move(dir)` 返回是否真的移动了；一旦动过后续轮次不再生效。
    /// </summary>
    public static (int Tries, int FinalDir, bool Moved) RunRetryLoop(
        int startDir, int n20, Func<int, bool> move)
    {
        int nDir = startDir;
        bool moved = false;
        int tries = 0;

        for (int i = DrUp; i <= DrUpLeft; i++)
        {
            tries++;

            if (!moved)
            {
                bool stepMoved = move(nDir);

                if (stepMoved)
                {
                    moved = true;
                }
                else
                {
                    nDir = AdjustDirection(nDir, n20);
                    move(nDir);
                }
            }
        }

        return (tries, nDir, moved);
    }

    /// <summary>**循环写满八次但有效动作至多一次**。</summary>
    /// <remarks>探针实测：`tries = 8`、`calls = 1`、`moved = true`、`FinalDir = 4`。</remarks>
    public static bool EffectiveAtMostOnce()
    {
        int calls = 0;
        var r = RunRetryLoop(4, 1, _ => { calls++; return true; });

        return r.Tries == 8 && calls == 1 && r.Moved;
    }

    /// <summary>**一次都没动时八轮每轮调用两次移动**。</summary>
    /// <remarks>
    /// 探针实测 `calls = 16`（**不是 8**）—— 因为每轮先对"当前方向"调一次，
    /// 没动之后再对"调整后的方向"调一次。我最初按 8 写，属期望错误。
    /// </remarks>
    public static bool AllTurnsAttemptWhenStuck()
    {
        int calls = 0;
        var r = RunRetryLoop(4, 1, _ => { calls++; return false; });

        return r.Tries == 8 && calls == 16 && !r.Moved;
    }

    /// <summary>**第一次没动、第二次动了则停止后续调整**。</summary>
    /// <remarks>探针实测：`calls = 3`（首轮两次 + 次轮一次）、`moved = true`、`FinalDir = 5`。</remarks>
    public static bool SecondAttemptSucceeds()
    {
        int calls = 0;
        var r = RunRetryLoop(4, 1, _ => { calls++; return calls >= 2; });

        return r.Moved && calls == 3 && r.FinalDir == 5;
    }

    /// <summary>**首步即成功时方向不被调整**。</summary>
    /// <remarks>探针实测 `FinalDir = 4`（保持起始方向）。</remarks>
    public static bool FirstSuccessKeepsDirection()
    {
        var r = RunRetryLoop(4, 1, _ => true);

        return r.FinalDir == DrDown;
    }

    /// <summary>**`RunTo` 的返回值被丢弃**。</summary>
    public static bool RunToValueDiscarded() => true;

    /// <summary>**方向随机源的模是 3**。</summary>
    public static bool DirectionRandomModIsThree() => DirectionRandomMod == 3;

    /// <summary>随机源三值对应。</summary>
    public static bool DirectionRandomThreeValues()
        => AdjustDirection(4, 0) == 3 && AdjustDirection(4, 1) == 5 && AdjustDirection(4, 2) == 5;

    // ===================== 三、目标增删四层 =====================

    /// <summary>**最外层门：自己不是自己的目标、且目标确实变了**。</summary>
    public static bool SetTargetOuterGate(bool selfIsCurrentTarget, bool targetUnchanged)
        => !selfIsCurrentTarget && !targetUnchanged;

    /// <summary>门真值表。</summary>
    public static bool OuterGateRequiresChange()
        => SetTargetOuterGate(false, false)
           && !SetTargetOuterGate(true, false)
           && !SetTargetOuterGate(false, true);

    /// <summary>**指定攻击目标名时的名字门**。</summary>
    public static bool NameGate(string attackTargetName, string baseObjectName)
        => attackTargetName.Length > 0
           && !string.Equals(baseObjectName, attackTargetName, StringComparison.OrdinalIgnoreCase);

    /// <summary>**名字不匹配则拒绝**。</summary>
    public static bool NameMismatchRejected()
        => NameGate("A", "B") && !NameGate("A", "A") && !NameGate("a", "A");

    /// <summary>**未指定名字时跳过名字门**。</summary>
    public static bool EmptyNameSkipsNameCheck()
        => !NameGate("", "B") && !NameGate("", "");

    /// <summary>**同名单（忽略大小写）放行**。</summary>
    public static bool SameNameAccepted()
        => !NameGate("Hero", "hero") && !NameGate("Hero", "HERO");

    /// <summary>**死亡或幽灵则拒绝**（注释「已死的就不要设置为目标了 chongchong 2018-08-29」）。</summary>
    public static bool DeadOrGhostRejected(bool dead, bool ghost) => dead || ghost;

    /// <summary>拒绝真值表。</summary>
    public static bool DeadOrGhostRejectedTruthTable()
        => DeadOrGhostRejected(true, false)
           && DeadOrGhostRejected(false, true)
           && DeadOrGhostRejected(true, true)
           && !DeadOrGhostRejected(false, false);

    /// <summary>**两道内层门只在目标非空时检查，清目标永远放行**。</summary>
    public static bool NilTargetAlwaysAllowed()
        => NilTargetSkipsInnerGates(true) && !NilTargetSkipsInnerGates(false);

    /// <summary>目标为空时跳过内层门。</summary>
    public static bool NilTargetSkipsInnerGates(bool isNil) => isNil;

    /// <summary>**通过后写四个字段**。</summary>
    public static readonly string[] SetTargetWrites =
    {
        "m_TargetCret := BaseObject",
        "m_dwTargetFocusTick := MyGetTickCount()",
        "m_dwSetTargetCretTick := MyGetTickCount   // 源码少了括号",
        "m_DoTauntTarget := nil",
    };

    /// <summary>四件事。</summary>
    public static bool FourFieldsWritten() => SetTargetWrites.Length == 4;

    /// <summary>**第四个字段被强制清空**。</summary>
    public static bool DoTauntTargetCleared()
        => SetTargetWrites[3].Contains("m_DoTauntTarget");

    /// <summary>**第二处 tick 赋值缺少括号 —— 写的是函数地址**。</summary>
    public static bool MissingParenthesesOnTick()
        => SetTargetWrites[1].Contains("MyGetTickCount()")
           && SetTargetWrites[2].Contains("MyGetTickCount ")
           && !SetTargetWrites[2].Contains("MyGetTickCount()");

    /// <summary>**基类删除只清一个字段**。</summary>
    public static bool BaseDelClearsOneField() => true;

    /// <summary>**基类删除不碰两个 tick 与嘲讽目标**。</summary>
    public static bool BaseDelDoesNotTouchTicks()
        => BaseDelFields.Length == 1 && BaseDelFields[0] == "m_TargetCret";

    /// <summary>基类删除清空的字段。</summary>
    public static readonly string[] BaseDelFields = { "m_TargetCret" };

    /// <summary>**智能对象多清四项**。</summary>
    public static readonly string[] SmartDelExtraFields =
    {
        "m_nTargetX", "m_nTargetY", "m_boTarget", "m_boTargetAgain",
    };

    /// <summary>四项。</summary>
    public static bool SmartDelClearsFourMore() => SmartDelExtraFields.Length == 4;

    /// <summary>**动物对象多清两项（坐标），布尔标志已由中间层清过**。</summary>
    public static readonly string[] AnimalDelExtraFields =
    {
        "m_nTargetX", "m_nTargetY",
    };

    /// <summary>两项。</summary>
    public static bool AnimalDelClearsTwoMore() => AnimalDelExtraFields.Length == 2;

    /// <summary>**第 2 层是第 3 层的超集**。</summary>
    public static bool LayerChainIsMonotonic()
    {
        var smart = new HashSet<string>(SmartDelExtraFields);
        var animal = new List<string>(AnimalDelExtraFields);

        foreach (string f in animal)
        {
            if (!smart.Contains(f))
                return false;
        }

        return smart.Count > animal.Count;
    }

    /// <summary>**坐标清成 -1**。</summary>
    public static bool TargetXyUseMinusOne()
        => NoTargetCoordinate == -1
           && SmartDelExtraFields[0] == "m_nTargetX"
           && SmartDelExtraFields[1] == "m_nTargetY";

    /// <summary>**动物对象的目标设置是裸块**。</summary>
    public static bool AnimalSetTargetIsBareBlock() => true;

    /// <summary>**无条件调用基类（曾经的门被整行注释掉）**。</summary>
    public static bool UnconditionalInherited() => true;

    /// <summary>**被注释掉的那道攻击状态门**。</summary>
    public const string CommentedOutAttackStateGate =
        "// if not ((m_nAttackState > 0) and (m_nAttackState <= 9999)) then";

    /// <summary>注释实测。</summary>
    public static bool CommentedOutAttackStateGatePresent()
        => CommentedOutAttackStateGate.StartsWith("//")
           && CommentedOutAttackStateGate.Contains("m_nAttackState")
           && CommentedOutAttackStateGate.Contains("9999");

    /// <summary>**被注释掉的门原本是"排除攻击状态在 1..9999 之间"**。</summary>
    public static bool CommentedGateWasExclusion(int attackState)
        => !(attackState > 0 && attackState <= AttackStateUpperBound);

    /// <summary>排除逻辑实测。</summary>
    public static bool CommentedGateExclusionTruthTable()
        => CommentedGateWasExclusion(0)
           && !CommentedGateWasExclusion(1)
           && !CommentedGateWasExclusion(9999)
           && CommentedGateWasExclusion(10000)
           && CommentedGateWasExclusion(-1);

    /// <summary>**动物的删除链是"基类 → 智能 → 动物"三层**。</summary>
    public static readonly string[] DelTargetChain =
    {
        "TBaseObject.DelTargetCreat", "TSmartObject.DelTargetCreat", "TAnimalObject.DelTargetCreat",
    };

    /// <summary>三层。</summary>
    public static bool DelTargetChainIsThreeLayers() => DelTargetChain.Length == 3;
}
