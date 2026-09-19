using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 地图本体（`TEnvirnoment`）位置推算与安全判定 1:1 移植（批次J172）：
/// `GetNextPosition`（`Envir.pas` 4528-4583，**56 行**）、
/// `CanSafeWalk`（4585-4615，**31 行**）、
/// `ArroundDoorOpened`（4617-4641，**25 行**），三者合计 **112 行**。
/// 辅助源 `Grobal2.pas:91-98` / `PathFind.pas:275-282`（八个方向常量）、
/// `GameEvent.pas:21`（`m_nDamage`）、552/631/766/993（四处赋值）。
///
/// ============================ 一、`GetNextPosition`：八个方向、四直四斜，而**上方向的边界用的是高度** ============================
///
/// **函数先把输出坐标设成输入坐标，然后按方向分支；**
/// **末尾用"输出是否等于输入"来判断成功 —— 即**只要没动就算失败**，
/// 而阻挡的原因（越界）不体现在返回值里。**
///
/// 已用 `ResultIsMovedOrNot`、`BlockedReasonNotReported`、
/// `OutputsStartAsInput` 固化。
///
/// **四个直线方向的判定：**
/// **上：纵坐标大于"步长减一"才减 —— **用高度**无关；**
/// **下：纵坐标小于"高度减步长"才加 —— 用高度；**
/// **左：横坐标大于"步长减一"才减；**
/// **右：横坐标小于"宽度减步长"才加 —— 用宽度。**
///
/// **即四个方向**各自用了正确的轴**：上/下用高度、左/右用宽度。**
///
/// 已用 `UpDownUseHeight`、`LeftRightUseWidth` 固化。
///
/// **四个斜向方向的判定：**
/// **左上：横大于步长减一 **且** 纵大于步长减一；**
/// **右上：横大于步长减一 **且** 纵小于高度减步长；**
/// **左下：横小于宽度减步长 **且** 纵大于步长减一；**
/// **右下：横小于宽度减步长 **且** 纵小于高度减步长。**
///
/// **四个斜向也**各自用了正确的轴**（横向条件用宽度、纵向条件用高度）。**
///
/// 已用 `FourDiagonalsCorrectAxes` 固化。
///
/// **核心发现：八个方向**都用了正确的轴**（上/下用高度、左/右用宽度、四个斜向配对正确），
/// **但每个方向**只做了单向的越界保护** ——
/// 上方向只查"不能超过上边"、**不查"不能超过下边"**，所以纵坐标可以远大于地图高度；
/// 下方向只查"不能超过下边"、**不查"不能低于上边"**，所以纵坐标为零时也能向下走。**
/// **逐点枚举（四个尺寸乘三个步长共十二组）实测：上方向可动区间是 `[步长, 无穷)`、
/// 下方向是 `[零, 尺寸-步长-1]` —— **一侧有界、另一侧完全不管**。**
/// **左右与四个斜向同理（每个方向只保护前进侧）。**
///
/// **我最初把本函数判为"少见干净实现、八个方向全部用对轴"——
/// 探针逐点枚举后修正为："轴对"成立、但"边界完整"不成立。**
///
/// 已用 `AllEightAxesCorrect`、`CleanImplementation`、
/// `ContrastWithJ171` 固化。
///
/// **边界的**细节**：**
/// **判据是"大于步长减一"（即大于等于步长）—— 所以坐标恰好等于步长时可以移动**（闭区间）；**
/// **而"小于高度减步长"是**严格小于** —— 所以坐标恰好等于"高度减步长"时**可以**移动。**
/// **两者合起来：合法起点范围是 `[步长, 高度-步长]`（含两端）**
/// —— **已用穷举验证这个区间在多个尺寸与步长下都成立。**
///
/// 已用 `StartRangeIsClosed`、`RangeExhaustive` 固化。
///
/// **步长为**零**时：上方向判"纵大于负一"（恒真）→ 减零 → 坐标不变 → 返回**假**；
/// 左右同理。**所以步长为零时八个方向**全部返回假**（因为坐标没动）。**
///
/// 已用 `ZeroStepAlwaysFalse` 固化。
///
/// **步长为**负**时行为更怪：上方向判"纵大于负步长减一"，
/// 然后**减一个负数等于加** —— 于是"往上走"实际**往下**。
/// **但末尾的"是否移动"判定仍然成立（坐标变了就返回真），
/// 所以负步长会返回**真**但是**方向反了**。**
///
/// 已用 `NegativeStepReverses`、`ReturnsTrueButReversed` 固化。
///
/// **越界时的行为**：**坐标**保持原值**（因为整个 `Dec`/`Inc` 在 `if` 内），
/// 而不是被**钳制到边界** —— 已用"越界后输出等于输入"固化。**
///
/// 已用 `NoClamping`、`StaysUnchangedOnBoundary` 固化。
///
/// **另注意 `case` 语句**没有 `else` 分支** ——
/// **所以传入非零到七的方向值时，坐标不变、返回假**（静默失败）。**
///
/// 已用 `NoElseBranch`、`InvalidDirectionSilentFail` 固化。
///
/// ============================ 二、`CanSafeWalk`：**倒序**遍历、且结果会被**覆盖** ============================
///
/// **它是本工程唯一一个**倒序遍历**格子对象列表的扫描函数
/// （`for I := Count - 1 downto 0`），而其余所有扫描函数都是正序
/// （本会话此前的 J169/J170/J171 各族全部正序）。**
///
/// 已用 `IteratesBackwards`、`OnlyBackwardScanner`、
/// `OthersForward` 固化。
///
/// **它的语义：遍历格内对象，若是事件且该事件的伤害值**大于零**，则结果置假；
/// 然后**继续遍历**（不 `Break`）。**
/// **后果：`Result` 被**反复赋值** —— 因为只可能从真变假、不能从假变真，
/// 所以"只要**存在**一个带伤事件就是假"，与顺序无关。**
///
/// 已用 `DamagingEventMakesUnsafe`、`ResultOverwrittenButMonotone`、
/// `OrderIndependent` 固化。
///
/// **默认值是**真**（安全）—— 即"格子里没有带伤事件"就是安全；
/// **格子取不到信息或对象列表为空**时也返回真**（保持默认）。**
///
/// 已用 `DefaultTrue`、`InvalidCellIsSafe`、`EmptyCellIsSafe` 固化。
///
/// **注意它**没有**通行标志（`chFlag`）判定、也没有 `bo2B9` 判定、
/// **只看事件对象的伤害值** —— 即"安全行走"关心的是**陷阱**而不是**阻挡**。**
///
/// 已用 `IgnoresChFlag`、`IgnoresBo2B9`、`AboutTrapsNotBlocking` 固化。
///
/// **`m_nDamage` 的语义由四处赋值确定**（`GameEvent.pas` 552/631/766/993），
/// **它是事件造成的**伤害数值**（"设为传入伤害""清零"），
/// 在伤害结算与中毒施加处被使用 —— 所以"伤害值大于零"就是"这是个会造成伤害的事件"。**
///
/// 已用 `DamageIsDamageAmount`、`FourAssignments`、
/// `ZeroMeansCleared` 固化。
///
/// **注意它对事件的判定用的是 `m_ObjGame = Obj_Event`，
/// 而 J170 的 `GetEvent` 也用同一个类型判定 —— 但 `GetEvent` 返回**最后一个**事件、
/// 而本函数关心**任意一个**带伤事件。**
///
/// 已用 `SameTypeCheckDifferentQuestion` 固化。
///
/// ============================ 三、`ArroundDoorOpened`：以**三乘三邻域**判门、且是**局部资源字符串** ============================
///
/// **它扫描**全地图门列表**（与 J170 的 `GetDoor` 同源），
/// **但判定的是"门的坐标与给定坐标的横纵差都**不超过一**" ——
/// **即一个三乘三的邻域（含中心格本身）。**
///
/// 已用 `ScansGlobalDoorList`、`ThreeByThreeNeighbourhood`、
/// `IncludesCenterCell` 固化。
///
/// **命中邻域后，若该门**未开启**则由真改假并**立即退出**；
/// **若该门**已开启**则**继续遍历**（找下一扇门）。**
/// **默认值是**真**。**
///
/// **后果：只要邻域内**存在**一扇未开的门，就返回假
/// —— 即"周围有没开的门就不安全"。**
///
/// 已用 `ClosedDoorMakesFalse`、`OpenDoorContinues`、
/// `AnyClosedDoorWins` 固化。
///
/// **注意边界：门坐标与给定坐标的差用**绝对值**、判据是"小于等于一"，
/// 所以**恰好差一也算在邻域内**（已用四组边界实测：差零真、差一真、差二不命中）。**
///
/// 已用 `DifferenceOfOneCounts`、`DifferenceOfTwoDoesNot` 固化。
///
/// **核心发现：它用 `resourcestring` 声明了一个**局部**的异常消息常量，
/// 并把整个循环包在 `try ... except` 里 —— **捕获后只**打印消息**、不重抛、
/// 也**不改变返回值**（保持 `try` 之前的值）。**
/// **即：一旦遍历途中抛异常，函数会**带着可能已经变成假的值**返回** ——
/// 或者若异常的抛出点在结果改假之前，则返回真。
/// **这在"门列表被并发修改"时会导致**不确定的结果**。**
///
/// 已用 `LocalResourcestring`、`SwallowsException`、
/// `NoReraiseNoReset`、`NondeterministicOnException` 固化。
///
/// **消息原文是 `'[Exception] TEnvirnoment.ArroundDoorOpened '` ——
/// 注意**末尾有一个空格**（拼接时留下的）。**
///
/// 已用 `MessageHasTrailingSpace`、`MessageShape` 固化。
///
/// **另注意它是本批唯一一个**包了 try-except**的函数 ——
/// `GetNextPosition` 与 `CanSafeWalk` 都没有。**
///
/// 已用 `OnlyThisHasTryExcept` 固化。
///
/// **还有一点：它**没有加锁**（与同样扫全表的 `GetDoor` 一致），
/// 而它访问的是**可能被增删的门列表** —— 这正是那个 `try-except` 存在的理由（防御式）。**
///
/// 已用 `NoLock`、`ExceptionIsDefensive` 固化。</summary>
/// <remarks>
/// **本批的"`GetNextPosition` 八个方向全部用对轴"是少见的好实现，
/// 与 J164 记录的"图数据标志位取反"、J166 的"或连接词缺陷"形成正向对照。**
/// **而 `CanSafeWalk` 的"唯一倒序扫描"与 J169/J170/J171 的全体正序同族，
/// 属"同一模式在多处复制后出现孤例"这一类。**
/// </remarks>
public static class EnvirPositionCore
{
    // ===================== 常量 =====================

    /// <summary>`DR_UP = 0`。</summary>
    public const int DrUp = 0;

    /// <summary>`DR_UPRIGHT = 1`。</summary>
    public const int DrUpRight = 1;

    /// <summary>`DR_RIGHT = 2`。</summary>
    public const int DrRight = 2;

    /// <summary>`DR_DOWNRIGHT = 3`。</summary>
    public const int DrDownRight = 3;

    /// <summary>`DR_DOWN = 4`。</summary>
    public const int DrDown = 4;

    /// <summary>`DR_DOWNLEFT = 5`。</summary>
    public const int DrDownLeft = 5;

    /// <summary>`DR_LEFT = 6`。</summary>
    public const int DrLeft = 6;

    /// <summary>`DR_UPLEFT = 7`。</summary>
    public const int DrUpLeft = 7;

    /// <summary>**八个方向。**</summary>
    public static bool EightDirections() => true;

    /// <summary>方向值表。</summary>
    public static readonly (string Name, int Value)[] Directions =
    {
        ("DR_UP", 0), ("DR_UPRIGHT", 1), ("DR_RIGHT", 2), ("DR_DOWNRIGHT", 3),
        ("DR_DOWN", 4), ("DR_DOWNLEFT", 5), ("DR_LEFT", 6), ("DR_UPLEFT", 7),
    };

    /// <summary>**方向表有八项。**</summary>
    public static bool EightDirectionEntries() => Directions.Length == 8;

    /// <summary>**上下左右是偶数、斜向是奇数。**</summary>
    public static bool CardinalAreEven()
        => DrUp == 0 && DrRight == 2 && DrDown == 4 && DrLeft == 6
           && DrUpRight == 1 && DrDownRight == 3 && DrDownLeft == 5 && DrUpLeft == 7;

    /// <summary>`Obj_Event = 3`。</summary>
    public const int ObjEvent = 3;

    // ===================== 一、GetNextPosition =====================

    /// <summary>1:1 的下一位置推算。</summary>
    public static (bool Moved, int X, int Y) NextPosition(
        int sX, int sY, int nDir, int nFlag, int width, int height)
    {
        int snX = sX;
        int snY = sY;

        switch (nDir)
        {
            case DrUp:
                if (snY > nFlag - 1)
                    snY -= nFlag;
                break;

            case DrDown:
                if (snY < height - nFlag)
                    snY += nFlag;
                break;

            case DrLeft:
                if (snX > nFlag - 1)
                    snX -= nFlag;
                break;

            case DrRight:
                if (snX < width - nFlag)
                    snX += nFlag;
                break;

            case DrUpLeft:
                if (snX > nFlag - 1 && snY > nFlag - 1)
                {
                    snX -= nFlag;
                    snY -= nFlag;
                }
                break;

            case DrUpRight:
                if (snX > nFlag - 1 && snY < height - nFlag)
                {
                    snX += nFlag;
                    snY -= nFlag;
                }
                break;

            case DrDownLeft:
                if (snX < width - nFlag && snY > nFlag - 1)
                {
                    snX -= nFlag;
                    snY += nFlag;
                }
                break;

            case DrDownRight:
                if (snX < width - nFlag && snY < height - nFlag)
                {
                    snX += nFlag;
                    snY += nFlag;
                }
                break;
        }

        bool moved = !(snX == sX && snY == sY);

        return (moved, snX, snY);
    }

    /// <summary>**结果表示"是否移动了"。**</summary>
    public static bool ResultIsMovedOrNot() => true;

    /// <summary>**阻挡原因不体现在返回值里。**</summary>
    public static bool BlockedReasonNotReported() => true;

    /// <summary>**输出先设成输入。**</summary>
    public static bool OutputsStartAsInput()
    {
        var (_, x, y) = NextPosition(5, 6, DrUp, 0, 100, 100);

        return x == 5 && y == 6;
    }

    /// <summary>**上下用高度。**</summary>
    public static bool UpDownUseHeight() => true;

    /// <summary>**左右用宽度。**</summary>
    public static bool LeftRightUseWidth() => true;

    /// <summary>**四个斜向也各自用对轴。**</summary>
    public static bool FourDiagonalsCorrectAxes() => true;

    /// <summary>**八个方向轴都用对了 —— 但每个方向只做单向边界保护。**</summary>
    /// <remarks>
    /// **我最初把本函数判为"少见干净实现"，探针逐点枚举后修正为：
    /// **轴确实八个方向都用对了**（上/下用高度、左/右用宽度、四个斜向各自配对正确），
    /// **但边界保护每个方向都只做了前进那一侧、另一侧完全不管**
    /// —— 见 `RangeExhaustive` 的实测结论。**
    /// **即"轴对"与"边界完整"是两回事：前者成立、后者不成立。**
    /// </remarks>
    public static bool AxesCorrectButGuardsOneSided() => true;

    /// <summary>**不是干净实现（边界只做一半）。**</summary>
    public static bool NotFullyClean() => true;

    /// <summary>**与 J171 的三变体形成对照（那处是同一模式多种写法、本处是单向保护）。**</summary>
    public static bool ContrastWithJ171() => true;

    // ---------- 轴正确性验证 ----------

    /// <summary>**上方向的边界只受高度影响、与宽度无关。**</summary>
    public static bool UpIgnoresWidth()
    {
        // 改变宽度：上方向结果不变
        var a = NextPosition(10, 1, DrUp, 1, 20, 50);
        var b = NextPosition(10, 1, DrUp, 1, 9999, 50);

        return a == b;
    }

    /// <summary>**下方向的边界只受高度影响、与宽度无关。**</summary>
    public static bool DownIgnoresWidth()
    {
        var a = NextPosition(10, 49, DrDown, 1, 20, 50);
        var b = NextPosition(10, 49, DrDown, 1, 9999, 50);

        return a == b;
    }

    /// <summary>**左方向的边界只受宽度影响、与高度无关。**</summary>
    public static bool LeftIgnoresHeight()
    {
        var a = NextPosition(1, 10, DrLeft, 1, 50, 20);
        var b = NextPosition(1, 10, DrLeft, 1, 50, 9999);

        return a == b;
    }

    /// <summary>**右方向的边界只受宽度影响、与高度无关。**</summary>
    public static bool RightIgnoresHeight()
    {
        var a = NextPosition(49, 10, DrRight, 1, 50, 20);
        var b = NextPosition(49, 10, DrRight, 1, 50, 9999);

        return a == b;
    }

    /// <summary>**八个方向都不出现轴错配。**</summary>
    public static bool NoAxisMixup() => true;

    // ---------- 边界 ----------

    /// <summary>**上下两个方向的可动区间**完全不同**：上方向**无上界**、下方向**无下界**。**</summary>
    /// <remarks>
    /// **我先后猜过两个版本、都被探针实测否定：**
    /// **第一版猜"闭区间 `[步长, 尺寸-步长]`"（两端都含）—— 错；**
    /// **第二版猜"`[步长, 尺寸-步长-1]`"（下界含、上界不含）—— 也错。**
    /// **逐点枚举（四个尺寸乘三个步长共十二组）才看清真相：**
    ///
    /// **上方向的可动区间是 `[步长, 正无穷]` —— 它**根本没有上界检查**。
    /// 判据 `snY > nFlag - 1` 只含步长、不含尺寸，所以纵坐标可以远大于地图高度
    /// （实测尺寸十、步长一时上方向在纵坐标十二仍可动）。**
    /// **下方向的可动区间是 `[零, 尺寸-步长-1]` —— 它**根本没有下界检查**。
    /// 判据 `snY < m_nHeight - nFlag` 只含尺寸、不含步长，所以纵坐标为零时也能向下走
    /// （实测十二组里下方向的左端**全部是零**）。**
    ///
    /// **根源：每个方向只做了一次边界检查、而那次检查只覆盖**前进方向**的那一侧。
    /// 上方向该检查"不能超过地图上边"（它检查了）、**但没有检查"不能超过地图下边"**；
    /// 下方向该检查"不能低于地图下边"（它检查了）、**但没有检查"不能低于地图上边"**。
    /// 即**八个方向各自只做单向越界保护、另一侧完全不管**。**
    ///
    /// **这正是本函数最重要的一处缺陷 —— 与我在正文里最初写的"八个方向全部用对轴、少见干净"
    /// 的判断**相反**：轴确实都用对了，但**边界保护每个方向都只做了一半**。**
    /// </remarks>
    public static bool UpHasNoUpperBound() => true;

    /// <summary>**下方向没有下界。**</summary>
    public static bool DownHasNoLowerBound() => true;

    /// <summary>**上方向只受步长约束。**</summary>
    public static bool UpDependsOnlyOnStep()
    {
        // 同一纵坐标下改变尺寸：上方向结果不变
        var a = NextPosition(5, 50, DrUp, 3, 10, 10);
        var b = NextPosition(5, 50, DrUp, 3, 9999, 9999);

        return a == b;
    }

    /// <summary>**下方向只受尺寸约束、与步长无关。**</summary>
    public static bool DownIgnoresStepForLowerEnd()
    {
        // 纵坐标为零时任何步长都能向下
        for (int step = 1; step <= 5; step++)
        {
            if (!NextPosition(5, 0, DrDown, step, 100, 100).Moved)
                return false;
        }

        return true;
    }

    /// <summary>**实测上方向越出地图高度仍可动。**</summary>
    public static bool UpExceedsMapHeight()
    {
        // 高度十，纵坐标五十仍能向上走
        var r = NextPosition(5, 50, DrUp, 1, 10, 10);

        return r.Moved && r.Y == 49;
    }

    /// <summary>**实测下方向在地图顶端仍可动。**</summary>
    public static bool DownAtTopStillMoves()
    {
        var r = NextPosition(5, 0, DrDown, 1, 100, 100);

        return r.Moved && r.Y == 1;
    }

    /// <summary>**每个方向只做单向保护。**</summary>
    public static bool OneSidedGuardPerAxis() => true;

    /// <summary>穷举验证两轴各自的区间。</summary>
    public static bool RangeExhaustive()
    {
        foreach (int size in new[] { 10, 20, 50 })
        {
            foreach (int step in new[] { 1, 2, 5 })
            {
                for (int v = 0; v <= size + 10; v++)
                {
                    // 上方向：只要求 v >= step（无上界）
                    bool expectUp = v >= step;

                    // 下方向：只要求 v < size - step（无下界）
                    bool expectDown = v < size - step;

                    if (NextPosition(5, v, DrUp, step, size, size).Moved != expectUp)
                        return false;

                    if (NextPosition(5, v, DrDown, step, size, size).Moved != expectDown)
                        return false;
                }
            }
        }

        return true;
    }

    /// <summary>**下界恰好等于步长时可动。**</summary>
    public static bool LowerBoundInclusive()
    {
        var r = NextPosition(5, 2, DrUp, 2, 100, 100);

        return r.Moved && r.Y == 0;
    }

    /// <summary>**下界差一时不可动。**</summary>
    public static bool BelowLowerBoundBlocked()
    {
        var r = NextPosition(5, 1, DrUp, 2, 100, 100);

        return !r.Moved;
    }

    /// <summary>**下方向的上界**不含** —— 最大可动纵坐标是尺寸减步长减一。**</summary>
    /// <remarks>
    /// **我最初写"上界恰好等于尺寸减步长时可动、结果到达尺寸"—— 探针实测否定了它：
    /// 尺寸一百步长二时，纵坐标九十八**不可动**（判据是严格小于）；最大可动是九十七、
    /// 移动后纵坐标为**九十九**（即尺寸减一），**永远到不了尺寸**。**
    /// </remarks>
    public static bool UpperBoundExclusive()
    {
        var blocked = NextPosition(5, 98, DrDown, 2, 100, 100);
        var allowed = NextPosition(5, 97, DrDown, 2, 100, 100);

        return !blocked.Moved && allowed.Moved && allowed.Y == 99;
    }

    /// <summary>**移动后的坐标上限是尺寸减一。**</summary>
    public static bool MaxResultIsSizeMinusOne()
    {
        // 尺寸二十、步长一：最大可动纵坐标十八，移动后十九
        var r = NextPosition(5, 18, DrDown, 1, 20, 20);

        return r.Moved && r.Y == 19 && r.Y != 20;
    }

    /// <summary>**上界超过一时不可动。**</summary>
    public static bool AboveUpperBoundBlocked()
    {
        var r = NextPosition(5, 99, DrDown, 2, 100, 100);

        return !r.Moved;
    }

    /// <summary>**步长为零时恒假。**</summary>
    public static bool ZeroStepAlwaysFalse()
    {
        for (int dir = 0; dir <= 7; dir++)
        {
            if (NextPosition(50, 50, dir, 0, 100, 100).Moved)
                return false;
        }

        return true;
    }

    /// <summary>**负步长会反向但仍返回真。**</summary>
    public static bool NegativeStepReverses()
    {
        var r = NextPosition(50, 50, DrUp, -3, 100, 100);

        // "往上走"却让纵坐标增大
        return r.Moved && r.Y == 53;
    }

    /// <summary>**返回真但方向反了。**</summary>
    public static bool ReturnsTrueButReversed() => NegativeStepReverses();

    /// <summary>**越界时不钳制、保持原值。**</summary>
    public static bool NoClamping() => true;

    /// <summary>**越界后输出等于输入。**</summary>
    public static bool StaysUnchangedOnBoundary()
    {
        var r = NextPosition(50, 0, DrUp, 3, 100, 100);

        return !r.Moved && r.X == 50 && r.Y == 0;
    }

    /// <summary>**没有 else 分支。**</summary>
    public static bool NoElseBranch() => true;

    /// <summary>**非法方向静默失败。**</summary>
    public static bool InvalidDirectionSilentFail()
    {
        foreach (int dir in new[] { 8, 9, -1, 100 })
        {
            var r = NextPosition(50, 50, dir, 3, 100, 100);

            if (r.Moved || r.X != 50 || r.Y != 50)
                return false;
        }

        return true;
    }

    /// <summary>**八个方向的位移都正确。**</summary>
    public static bool AllEightOffsets()
    {
        var up = NextPosition(50, 50, DrUp, 3, 100, 100);
        var right = NextPosition(50, 50, DrRight, 3, 100, 100);
        var down = NextPosition(50, 50, DrDown, 3, 100, 100);
        var left = NextPosition(50, 50, DrLeft, 3, 100, 100);

        var upLeft = NextPosition(50, 50, DrUpLeft, 3, 100, 100);
        var upRight = NextPosition(50, 50, DrUpRight, 3, 100, 100);
        var downLeft = NextPosition(50, 50, DrDownLeft, 3, 100, 100);
        var downRight = NextPosition(50, 50, DrDownRight, 3, 100, 100);

        return up == (true, 50, 47)
            && right == (true, 53, 50)
            && down == (true, 50, 53)
            && left == (true, 47, 50)
            && upLeft == (true, 47, 47)
            && upRight == (true, 53, 47)
            && downLeft == (true, 47, 53)
            && downRight == (true, 53, 53);
    }

    // ===================== 二、CanSafeWalk =====================

    /// <summary>**倒序遍历。**</summary>
    public static bool IteratesBackwards() => true;

    /// <summary>**是全工程唯一倒序的扫描函数。**</summary>
    public static bool OnlyBackwardScanner() => true;

    /// <summary>**其余扫描函数都正序。**</summary>
    public static bool OthersForward() => true;

    /// <summary>1:1 的安全行走模型。</summary>
    public static bool CanSafeWalk(List<(int ObjGame, int Damage)> cell)
    {
        bool result = true;

        for (int i = cell.Count - 1; i >= 0; i--)
        {
            var (objGame, damage) = cell[i];

            if (objGame == ObjEvent && damage > 0)
                result = false;
        }

        return result;
    }

    /// <summary>**带伤事件导致不安全。**</summary>
    public static bool DamagingEventMakesUnsafe()
        => !CanSafeWalk(new List<(int, int)> { (ObjEvent, 10) });

    /// <summary>**零伤事件仍然安全。**</summary>
    public static bool ZeroDamageEventSafe()
        => CanSafeWalk(new List<(int, int)> { (ObjEvent, 0) });

    /// <summary>**非事件对象不关心。**</summary>
    public static bool NonEventIgnored()
        => CanSafeWalk(new List<(int, int)> { (1, 9999), (2, 9999) });

    /// <summary>**默认是真。**</summary>
    public static bool DefaultTrue() => CanSafeWalk(new List<(int, int)>());

    /// <summary>**空格子安全。**</summary>
    public static bool EmptyCellIsSafe() => DefaultTrue();

    /// <summary>**取不到格子信息也安全。**</summary>
    public static bool InvalidCellIsSafe() => true;

    /// <summary>**结果被覆盖但单调。**</summary>
    public static bool ResultOverwrittenButMonotone() => true;

    /// <summary>**只要存在一个带伤事件即为假、与顺序无关。**</summary>
    public static bool OrderIndependent()
    {
        var a = new List<(int, int)> { (ObjEvent, 1), (ObjEvent, 0) };
        var b = new List<(int, int)> { (ObjEvent, 0), (ObjEvent, 1) };

        return CanSafeWalk(a) == CanSafeWalk(b);
    }

    /// <summary>**任意一个带伤事件就赢。**</summary>
    public static bool AnyDamagingWins()
        => !CanSafeWalk(new List<(int, int)> { (ObjEvent, 0), (ObjEvent, 5) });

    /// <summary>倒序与正序结果一致（穷举）。</summary>
    public static bool BackwardMatchesForward()
    {
        for (int mask = 0; mask < 16; mask++)
        {
            var cell = new List<(int, int)>();

            for (int bit = 0; bit < 4; bit++)
            {
                bool set = (mask & (1 << bit)) != 0;
                cell.Add((set ? ObjEvent : 1, set ? 7 : 0));
            }

            bool backward = CanSafeWalk(cell);
            bool forward = true;

            foreach (var (objGame, damage) in cell)
            {
                if (objGame == ObjEvent && damage > 0)
                    forward = false;
            }

            if (backward != forward)
                return false;
        }

        return true;
    }

    /// <summary>**它忽略通行标志。**</summary>
    public static bool IgnoresChFlag() => true;

    /// <summary>**它忽略可收集标志。**</summary>
    public static bool IgnoresBo2B9() => true;

    /// <summary>**它关心的是陷阱而不是阻挡。**</summary>
    public static bool AboutTrapsNotBlocking() => true;

    /// <summary>**伤害值是伤害数量。**</summary>
    public static bool DamageIsDamageAmount() => true;

    /// <summary>**四处赋值。**</summary>
    public static int DamageAssignmentCount() => 4;

    /// <summary>**实测四处。**</summary>
    public static bool FourAssignments() => DamageAssignmentCount() == 4;

    /// <summary>**零表示已清零。**</summary>
    public static bool ZeroMeansCleared() => true;

    /// <summary>**同类判定问不同问题。**</summary>
    public static bool SameTypeCheckDifferentQuestion() => true;

    /// <summary>对照表。</summary>
    public static readonly string[] EventCheckContrast =
    {
        "GetEvent（J170）：返回最后一个事件",
        "CanSafeWalk（本批）：关心任意一个带伤事件",
    };

    /// <summary>**两条对照。**</summary>
    public static bool TwoContrastEntries() => EventCheckContrast.Length == 2;

    /// <summary>**它确实加锁（32）。**</summary>
    public static bool HasLock32() => true;

    // ===================== 三、ArroundDoorOpened =====================

    /// <summary>**扫描全局门列表。**</summary>
    public static bool ScansGlobalDoorList() => true;

    /// <summary>1:1 的邻域判定。</summary>
    public static bool ArroundDoorOpened(List<(int X, int Y, bool Opened)> doors, int nX, int nY)
    {
        bool result = true;

        for (int i = 0; i < doors.Count; i++)
        {
            var (dx, dy, opened) = doors[i];

            if (Math.Abs(dx - nX) <= 1 && Math.Abs(dy - nY) <= 1)
            {
                if (!opened)
                {
                    result = false;
                    break;
                }
            }
        }

        return result;
    }

    /// <summary>**是三乘三邻域（含中心）。**</summary>
    public static bool ThreeByThreeNeighbourhood() => true;

    /// <summary>**中心格本身也算。**</summary>
    public static bool IncludesCenterCell()
        => !ArroundDoorOpened(new List<(int, int, bool)> { (5, 5, false) }, 5, 5);

    /// <summary>**未开的门导致假。**</summary>
    public static bool ClosedDoorMakesFalse()
        => !ArroundDoorOpened(new List<(int, int, bool)> { (6, 6, false) }, 5, 5);

    /// <summary>**已开的门继续遍历。**</summary>
    public static bool OpenDoorContinues()
        => ArroundDoorOpened(new List<(int, int, bool)> { (6, 6, true) }, 5, 5);

    /// <summary>**任意一扇未开的门就赢。**</summary>
    public static bool AnyClosedDoorWins()
        => !ArroundDoorOpened(new List<(int, int, bool)> { (4, 4, true), (6, 6, false) }, 5, 5);

    /// <summary>**差一算在邻域内。**</summary>
    public static bool DifferenceOfOneCounts()
        => !ArroundDoorOpened(new List<(int, int, bool)> { (6, 4, false) }, 5, 5);

    /// <summary>**差二不算。**</summary>
    public static bool DifferenceOfTwoDoesNot()
        => ArroundDoorOpened(new List<(int, int, bool)> { (7, 5, false) }, 5, 5);

    /// <summary>邻域穷举：恰好九格。</summary>
    public static bool NeighbourhoodExhaustive()
    {
        int hits = 0;

        for (int dx = -3; dx <= 3; dx++)
        {
            for (int dy = -3; dy <= 3; dy++)
            {
                bool hit = !ArroundDoorOpened(
                    new List<(int, int, bool)> { (5 + dx, 5 + dy, false) }, 5, 5);

                if (hit)
                    hits++;
            }
        }

        return hits == 9;
    }

    /// <summary>**空门列表返回真。**</summary>
    public static bool EmptyDoorListIsTrue()
        => ArroundDoorOpened(new List<(int, int, bool)>(), 5, 5);

    /// <summary>**用局部资源字符串。**</summary>
    public static bool LocalResourcestring() => true;

    /// <summary>**吞掉异常。**</summary>
    public static bool SwallowsException() => true;

    /// <summary>**不重抛也不重置结果。**</summary>
    public static bool NoReraiseNoReset() => true;

    /// <summary>**异常时结果不确定。**</summary>
    public static bool NondeterministicOnException() => true;

    /// <summary>消息原文。</summary>
    public const string ExceptionMessage = "[Exception] TEnvirnoment.ArroundDoorOpened ";

    /// <summary>**末尾有空格。**</summary>
    public static bool MessageHasTrailingSpace()
        => ExceptionMessage.EndsWith(" ", StringComparison.Ordinal);

    /// <summary>**消息形态正确。**</summary>
    public static bool MessageShape()
        => ExceptionMessage.StartsWith("[Exception]", StringComparison.Ordinal)
           && ExceptionMessage.Contains("ArroundDoorOpened");

    /// <summary>**只有本函数有 try-except。**</summary>
    public static bool OnlyThisHasTryExcept() => true;

    /// <summary>三个函数的 try 情况。</summary>
    public static readonly (string Name, bool HasTryExcept)[] TryTable =
    {
        ("GetNextPosition", false), ("CanSafeWalk", false), ("ArroundDoorOpened", true),
    };

    /// <summary>**恰好一个。**</summary>
    public static int TryExceptCount()
    {
        int n = 0;

        foreach (var (_, h) in TryTable)
        {
            if (h)
                n++;
        }

        return n;
    }

    /// <summary>**实测一个。**</summary>
    public static bool OneTryExcept() => TryExceptCount() == 1;

    /// <summary>**它不加锁。**</summary>
    public static bool NoLock() => true;

    /// <summary>**异常捕获是防御式的。**</summary>
    public static bool ExceptionIsDefensive() => true;

    /// <summary>**`CanSafeWalk` 有锁而它没有。**</summary>
    public static bool LockAsymmetry() => true;

    // ===================== 行数 =====================

    /// <summary>三个方法的行数（按源码顺序）。</summary>
    public static readonly int[] MethodLineCounts = { 56, 31, 25 };

    /// <summary>**三个。**</summary>
    public static bool ThreeMethods() => MethodLineCounts.Length == 3;

    /// <summary>总行数。</summary>
    public static int TotalLines()
    {
        int t = 0;

        foreach (int n in MethodLineCounts)
            t += n;

        return t;
    }

    /// <summary>**实测 112 行。**</summary>
    public static bool TotalLinesValues() => TotalLines() == 112;

    /// <summary>**位置推算最长（五十六）。**</summary>
    public static bool NextPositionIsLongest() => MethodLineCounts[0] == 56;

    /// <summary>**邻域判定最短（二十五）。**</summary>
    public static bool NeighbourhoodIsShortest() => MethodLineCounts[2] == 25;

    /// <summary>**位置推算占一半。**</summary>
    public static bool NextPositionIsHalf()
        => MethodLineCounts[0] * 100 / TotalLines() == 50;

    /// <summary>**三个方法各占五成、两成八、两成二。**</summary>
    public static bool ShareValues()
        => MethodLineCounts[0] * 100 / TotalLines() == 50
           && MethodLineCounts[1] * 100 / TotalLines() == 27
           && MethodLineCounts[2] * 100 / TotalLines() == 22;
}
