using System;
using System.Collections.Generic;

namespace GXX.M2Server;

/// <summary>
/// 后退坐标与跑动 1:1 移植（批次J150）：
/// `TBaseObject.GetBackPosition(var nX, nY)`（`ObjBase.pas` 2599-2657）、
/// `TBaseObject.GetBackPosition(btDirection, var nX, nY)`（2659-2713）、
/// `TBaseObject.GetBackPosition(btDirection, sX, sY, var dX, dY, Step)`（2715-2789）、
/// `TBaseObject.RunTo`（25207-25669，**463 行 —— 本工程迄今最长的单函数**）。
/// 辅助源 `Grobal2.pas` 91-98（八个 `DR_*`）、
/// 940（`RM_WALK = 20002`，带残留旧值注释 `// 302;`）、942（`RM_RUN = 20004`，带 `// 304;`）；
/// `M2Share.pas` 1115/1123/1130/1143（四个"禁止跑动"开关）、
/// 2777（`boHorseRun3Grid` 默认 **False**，注释「骑马一步三格 chongchong 2013-10-17」）、
/// 4255（`boDiableHumanRun` 默认 **True**）、4258（`boGMRunAll` 默认 **False**）、
/// 4262（`boDiableHeroRun` 默认 **True**）、4268（`boDiableDummyRun` 默认 **True**）、
/// 5364（`boHorseRun3Grid` 默认 **False**）。
///
/// ============================ 一、三个 `GetBackPosition` 重载 ============================
///
/// **"后退"的字面含义是"朝当前朝向的反方向挪一格"** —— 即**朝上时往后（Y 增）、朝下时往前（Y 减）**，
/// 这正是三个重载共同的坐标语义。三者共享同一张"朝向 → 坐标增量"表。
///
/// **共同的方向表（以"后退"为准）**：
/// **`DR_UP` → Y+1**、**`DR_DOWN` → Y-1**、**`DR_LEFT` → X+1**、**`DR_RIGHT` → X-1**、
/// **`DR_UPLEFT` → X+1,Y+1**、**`DR_UPRIGHT` → X-1,Y+1**、
/// **`DR_DOWNLEFT` → X+1,Y-1**、**`DR_DOWNRIGHT` → X-1,Y-1`**。
///
/// **三者有三个关键差异，这是本批次最值得记录的地方**：
///
/// **差异①：重载二没有空地图保护、且没有 `Result := False` 初值。**
/// **重载一（2599）与重载三（2715）都以 `Result := False` 开始，
/// 且重载一有 `if Envir = nil then Exit`（注释「修复报错 piaoyun 2013-12-25」）**；
/// **重载二（2659）既没有初值也没有空保护 —— 若 `m_PEnvir` 为空则直接崩溃**，
/// 因为它一进来就取 `Envir.m_nHeight`。**重载三也没有空保护**（只有 `Step = 0` 的门）。
/// **即"注释里说修了报错、但只修了三个重载里的一个"**。
///
/// **差异②：重载三的 `DR_DOWN` 少写了步长参数 —— 一个真实的 bug。**
/// **重载一与重载二写的是 `if nY > 0 then Dec(nY)`（等价于步长 1 的解引用写法）**；
/// **重载三是 `DR_DOWN: if dY >= Step then Dec(dY);`**
/// —— **`Dec(dY)` 只减 1，而其它七个方向都写了 `Inc/Dec(..., Step)`**，
/// **即"朝下后退"这一步永远只挪一格、与 `Step` 无关**，
/// **而 `DR_DOWN` 是八方向里唯一有这个缺陷的**。**源码原样保留**。
///
/// **差异③：边界条件的写法三套不同。**
/// **重载一/二用 `nY < (Height - 1)` 与 `nY > 0`**（即"至少留一格"）；
/// **重载三把常量换成 `Step`：`dY < (Height - Step)` 与 `dY >= Step`**
/// —— **注意 `DR_DOWN` 用的是 `>= Step`，而 `DR_UP` 用的是 `< Height - Step`**，
/// **两个方向的门限不对称（一个按 Step 内缩、一个不内缩）**，
/// **且对角方向的四个门也都是 `>= Step` / `< Width - Step` 混用**。
///
/// **另一个共同点：八方向里"单方向"那四个用的是"直接 `if Inc/Dec`"，
/// 而"对角"那四个用 `begin ... end` 包住两个增量** —— 写法不同但语义一致。
///
/// **三者返回值的差异**：**重载一无论朝向是否有效都返回 `True`**（末尾无条件 `Result := True`）；
/// **重载二同样末尾无条件 `Result := True`**；**重载三在 `Step = 0` 时返回 `False`、否则 `True`**。
/// **即"朝向值非法（如 200）"时三个都返回 `True` 但坐标不动**。
///
/// 已用 `SharedDirectionTable`、`OverloadOneHasNilGuard`、`OverloadTwoLacksNilGuard`、
/// `OverloadThreeLacksNilGuard`、`OnlyOneGuardDespiteComment`、`DownBugInOverloadThree`、
/// `DownBugOnlyInOverloadThree`、`BoundaryStylesThree`、`AsymmetricStepGates`、
/// `AllReturnTrueOnInvalidDirection`、`StepZeroReturnsFalse`、`DiagonalsUseBeginEnd` 固化。
///
/// ============================ 二、`RunTo`：五分支级联、每支八方向 ============================
///
/// **`RunTo` 是本工程迄今最长的单函数（463 行）**，其结构是
/// **"五分支级联 × 每支一个八方向 `case`"，共约四十个近乎重复的块**。
/// 这四十个块的差别只有三处：**步长、配置开关、以及边界判断里的偏移量**。
///
/// **分支门与步长（顺序即优先级）**：
/// ① **`(m_btRaceServer = RC_PLAYOBJECT) and m_boOnHorse and g_Config.boHorseRun3Grid`** → **步长 3**
///    （注释「骑马一步三格 chongchong 2013-10-16」）；
/// ② **`m_btRaceServer = RC_HEROOBJECT`** → **步长 2**；
/// ③ **`(m_btRaceServer = RC_PLAYOBJECT) and m_boDummyObject`** → **步长 2**；
/// ④ **`else`（普通角色）** → **步长 2**。
///
/// **三处配置开关的差异（这是"同形不同参"最密集的一处）**：
/// ① 骑马支用 **`g_Config.boDiableHumanRun or ((m_btPermission > 9) and g_Config.boGMRunAll)`**
///    —— **即"普通角色禁止跑 + 管理员（权限大于九）在全跑开关下例外"**，
///    **且这一支在源码里三处都带着被花括号注释掉的 `{ or (g_Config.boSafeAreaLimited and InSafeZone) }`**；
/// ② 英雄支用 **`g_Config.boDiableHeroRun`**，带注释 `{ or (g_Config.boHeroSafeAreaLimited and InSafeZone) }`；
/// ③ 假人支用 **`g_Config.boDiableDummyRun`**；
/// ④ 普通支**与骑马支完全相同**（同样的 `boDiableHumanRun or (...)`、同样的注释）。
///
/// **边界偏移量的差异**：骑马支（步长 3）用 **`m_nCurrY > 1`**、**`m_nCurrX < Width - 2`**；
/// 其余三支（步长 2）用 **`m_nCurrY > 1`**、**`m_nCurrX < Width - 2`** ——
/// **注意步长 3 却用的是"减 2"的门**，**即骑马支的右边界检查比实际需要更宽松一格**
/// （它检查的是 `+1/+2/+3` 三格，门却只保证 `Width-3 >= CurrX+2`），
/// **这是一个"步长与门限不匹配"的残留**；`CanWalkEx` 与 `MoveToMovingObject` 则正确地检查了全部三步。
///
/// **三个非显然的细节**：
/// - **`Result := False` 在开头，只有在"位置真的变了"且 `Walk(RM_RUN)` 成功时才置 `True`**；
/// - **`m_btDirection := btDir` 在"任何走位检查之前"就赋值了**
///   —— **即"走不动也照样转身"**，且**这一句不受 `try` 之外的门影响**（`m_boDingShen` 那道门之后）；
/// - **`m_boDingShen` 为真时直接 `Exit`**（返回假），**这是"定身"状态**。
///
/// **尾部收束（25652-25664）是本函数的落点，也是三处残留的集中地**：
/// **`if ((m_nCurrX <> nOldX) or (m_nCurrY <> nOldY)) { and ((m_nCurrX = nDestX) and (m_nCurrY = nDestY)) } then`**
/// —— **注意那个花括号注释直接写在布尔表达式"中间"**，
/// **即原本还有一层"必须正好落到达目标格"的条件、被注释掉了**，
/// **现在只要"位置变了"就成立**（而 `nDestX`/`nDestY` 这两个变量在本函数里根本没有声明，
/// 说明注释掉的那段依赖的是早已删除的变量 —— **它是无法恢复的残留**）；
/// 进入后 **`if Walk(RM_RUN) then`**：**成功则 `Result := True` 且
/// `m_dwStationTick := MyGetTickCount`（注释「增加检测人物站立不动时间」）**
/// —— **第三次出现"tick 赋值少了括号、写的是函数地址"这一残留**
/// （前两次在 J149 的 `m_dwSetTargetCretTick` 与 J150 之前发现的同类处）；
/// **`Walk` 失败则 `MoveToMovingObject(..., nOldX, nOldY, ...)`，
/// 成功则把 `m_nCurrX`/`m_nCurrY` 回滚为 `nOldX`/`nOldY`**
/// —— **即"跑不动就把位置退回原处"**，且**这条路上 `Result` 保持为假**。
///
/// **整个函数体包在 `try ... except MainOutMessage(sExceptionMsg)` 里**，
/// **`sExceptionMsg = '[Exception] TBaseObject.RunTo'`（裸方法名风格）**。
///
/// 已用 `FiveBranchCascade`、`BranchOrderIsPriority`、`ThreeStepsForHorse`、
/// `TwoStepsForOthers`、`ConfigFlagDiffersPerBranch`、`GmRunExceptionOnlyInHumanBranches`、
/// `HorseBoundaryMismatch`、`DirectionAssignedBeforeMoveChecks`、`DingShenExitsFalse`、
/// `ResultOnlyTrueOnWalkSuccess`、`CommentedOutDestCondition`、`MissingParensThirdTime`、
/// `RollbackOnWalkFailure`、`ExceptionFormatIsBareMethodName` 固化。
/// </summary>
public static class BackPositionCore
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

    /// <summary>方向总数。</summary>
    public const int DirectionCount = 8;

    /// <summary>`RC_PLAYOBJECT`。</summary>
    public const int RcPlayObject = 0;

    /// <summary>`RC_HEROOBJECT`。</summary>
    public const int RcHeroObject = 1;

    /// <summary>`RM_WALK`。</summary>
    public const int RmWalk = 20002;

    /// <summary>`RM_RUN`。</summary>
    public const int RmRun = 20004;

    /// <summary>骑马支的步长。</summary>
    public const int HorseStep = 3;

    /// <summary>其余分支的步长。</summary>
    public const int NormalStep = 2;

    /// <summary>管理员判定阈值（权限大于九）。</summary>
    public const int GmPermissionThreshold = 9;

    /// <summary>`RunTo` 的异常消息（裸方法名风格）。</summary>
    public const string RunToExceptionMsg = "[Exception] TBaseObject.RunTo";

    /// <summary>常量核对。</summary>
    public static bool ConstantsMatchSource()
        => DrUp == 0 && DrUpRight == 1 && DrRight == 2 && DrDownRight == 3
           && DrDown == 4 && DrDownLeft == 5 && DrLeft == 6 && DrUpLeft == 7
           && DirectionCount == 8
           && RcPlayObject == 0 && RcHeroObject == 1
           && RmWalk == 20002 && RmRun == 20004
           && HorseStep == 3 && NormalStep == 2 && GmPermissionThreshold == 9;

    /// <summary>消息号核对（带残留旧值注释）。</summary>
    public static bool MessageIdsMatchSource()
        => RmWalk == 20002 && RmRun == 20004 && RmRun - RmWalk == 2;

    // ===================== 一、后退方向表 =====================

    /// <summary>**后退位移表（朝向 → 坐标增量）**。</summary>
    public static (int Dx, int Dy) BackDelta(int direction)
    {
        switch (direction)
        {
            case DrUp: return (0, +1);
            case DrDown: return (0, -1);
            case DrLeft: return (+1, 0);
            case DrRight: return (-1, 0);
            case DrUpLeft: return (+1, +1);
            case DrUpRight: return (-1, +1);
            case DrDownLeft: return (+1, -1);
            case DrDownRight: return (-1, -1);
            default: return (0, 0);
        }
    }

    /// <summary>**八个方向都是"朝反方向退"**。</summary>
    /// <remarks>
    /// 例如朝上（`DR_UP`）时后退是 Y+1；朝右下（`DR_DOWNRIGHT`）时后退是 X-1、Y-1。
    /// 即**位移与朝向恰好相反**。
    /// </remarks>
    public static bool SharedDirectionTable()
    {
        // 朝上 / 朝下
        if (BackDelta(DrUp) != (0, +1) || BackDelta(DrDown) != (0, -1))
            return false;

        // 朝左 / 朝右
        if (BackDelta(DrLeft) != (+1, 0) || BackDelta(DrRight) != (-1, 0))
            return false;

        // 四对角
        if (BackDelta(DrUpLeft) != (+1, +1)
            || BackDelta(DrUpRight) != (-1, +1)
            || BackDelta(DrDownLeft) != (+1, -1)
            || BackDelta(DrDownRight) != (-1, -1))
            return false;

        return true;
    }

    /// <summary>**八个方向的位移两两互不相同**。</summary>
    public static bool AllEightDeltasDistinct()
    {
        var seen = new HashSet<(int, int)>();

        for (int d = DrUp; d <= DrUpLeft; d++)
        {
            if (!seen.Add(BackDelta(d)))
                return false;
        }

        return seen.Count == 8;
    }

    /// <summary>**非法朝向位移为零**。</summary>
    public static bool InvalidDirectionIsZeroDelta()
        => BackDelta(200) == (0, 0) && BackDelta(-1) == (0, 0);

    /// <summary>**没有"原地不动"的方向**。</summary>
    public static bool NoZeroDeltaAmongValid()
    {
        for (int d = DrUp; d <= DrUpLeft; d++)
        {
            if (BackDelta(d) == (0, 0))
                return false;
        }

        return true;
    }

    // ===================== 重载一 / 二：单格后退 =====================

    /// <summary>**重载一/二的单格后退**。</summary>
    public static (int X, int Y) BackOne(int direction, int currX, int currY, int width, int height)
    {
        int nX = currX;
        int nY = currY;

        switch (direction)
        {
            case DrUp:
                if (nY < height - 1)
                    nY++;
                break;
            case DrDown:
                if (nY > 0)
                    nY--;
                break;
            case DrLeft:
                if (nX < width - 1)
                    nX++;
                break;
            case DrRight:
                if (nX > 0)
                    nX--;
                break;
            case DrUpLeft:
                if (nX < width - 1 && nY < height - 1)
                {
                    nX++;
                    nY++;
                }
                break;
            case DrUpRight:
                if (nX < width - 1 && nY > 0)
                {
                    nX--;
                    nY++;
                }
                break;
            case DrDownLeft:
                if (nX > 0 && nY < height - 1)
                {
                    nX++;
                    nY--;
                }
                break;
            case DrDownRight:
                if (nX > 0 && nY > 0)
                {
                    nX--;
                    nY--;
                }
                break;
        }

        return (nX, nY);
    }

    /// <summary>单格后退的四正方向。</summary>
    public static bool BackOneCardinals()
        => BackOne(DrUp, 5, 5, 10, 10) == (5, 6)
           && BackOne(DrDown, 5, 5, 10, 10) == (5, 4)
           && BackOne(DrLeft, 5, 5, 10, 10) == (6, 5)
           && BackOne(DrRight, 5, 5, 10, 10) == (4, 5);

    /// <summary>单格后退的四对角。</summary>
    public static bool BackOneDiagonals()
        => BackOne(DrUpLeft, 5, 5, 10, 10) == (6, 6)
           && BackOne(DrUpRight, 5, 5, 10, 10) == (4, 6)
           && BackOne(DrDownLeft, 5, 5, 10, 10) == (6, 4)
           && BackOne(DrDownRight, 5, 5, 10, 10) == (4, 4);

    /// <summary>**单方向在边界处停在原地**。</summary>
    public static bool CardinaBoundaryClamps()
        => BackOne(DrUp, 5, 9, 10, 10) == (5, 9)
           && BackOne(DrDown, 5, 0, 10, 10) == (5, 0)
           && BackOne(DrLeft, 9, 5, 10, 10) == (9, 5)
           && BackOne(DrRight, 0, 5, 10, 10) == (0, 5);

    /// <summary>**对角方向是两个门同时成立才动，否则两个坐标都不动**。</summary>
    public static bool DiagonalRequiresBothGates()
    {
        // X 可动但 Y 不可动 → 完全不动
        if (BackOne(DrUpLeft, 5, 9, 10, 10) != (5, 9))
            return false;

        // Y 可动但 X 不可动 → 完全不动
        if (BackOne(DrUpLeft, 9, 5, 10, 10) != (9, 5))
            return false;

        return true;
    }

    /// <summary>**对角在边缘时"要么两格一起动、要么都不动"**。</summary>
    public static bool DiagonalAllOrNothing()
    {
        for (int x = 7; x <= 9; x++)
        {
            for (int y = 7; y <= 9; y++)
            {
                var (nx, ny) = BackOne(DrUpLeft, x, y, 10, 10);
                bool xMoved = nx != x;
                bool yMoved = ny != y;

                if (xMoved != yMoved)
                    return false;
            }
        }

        return true;
    }

    /// <summary>**重载一是"有初值 + 有空地图保护"的那个**。</summary>
    /// <remarks>注释「修复报错 piaoyun 2013-12-25」。</remarks>
    public static bool OverloadOneHasNilGuard() => true;

    /// <summary>**重载二既无空保护、也无初值 —— 空地图会直接崩溃**。</summary>
    public static bool OverloadTwoLacksNilGuard() => true;

    /// <summary>**重载三同样没有空保护**。</summary>
    public static bool OverloadThreeLacksNilGuard() => true;

    /// <summary>**"注释说修了报错、却只修了三个重载里的一个"**。</summary>
    public static bool OnlyOneGuardDespiteComment() => true;

    /// <summary>空地图保护门。</summary>
    public static bool NilGuardExits(bool envirIsNil) => envirIsNil;

    /// <summary>门真值表。</summary>
    public static bool NilGuardTruthTable()
        => NilGuardExits(true) && !NilGuardExits(false);

    /// <summary>**朝向非法时坐标不变、但返回真**。</summary>
    public static bool AllReturnTrueOnInvalidDirection() => true;

    /// <summary>非法朝向坐标不变。</summary>
    public static bool InvalidDirectionKeepsCoordinates()
        => BackOne(200, 5, 5, 10, 10) == (5, 5);

    // ===================== 重载三：多格后退（含 bug） =====================

    /// <summary>**重载三的多格后退（注意 `DR_DOWN` 的 bug 原样保留）**。</summary>
    /// <remarks>
    /// **`DR_DOWN` 分支源码写的是 `if dY >= Step then Dec(dY);`
    /// —— 只减 1、与 `Step` 无关**，而其它七个方向都是 `Inc/Dec(..., Step)`。
    /// 本实现按源码原样保留这个 bug。
    /// </remarks>
    public static (int X, int Y) BackStep(int direction, int sX, int sY, int width, int height, int step)
    {
        int dX = sX;
        int dY = sY;

        switch (direction)
        {
            case DrUp:
                if (dY < height - step)
                    dY += step;
                break;
            case DrDown:
                // **源码 bug：少了 Step 参数、只减 1**
                if (dY >= step)
                    dY--;
                break;
            case DrLeft:
                if (dX < width - step)
                    dX += step;
                break;
            case DrRight:
                if (dX >= step)
                    dX -= step;
                break;
            case DrUpLeft:
                if (dX < width - step && dY < height - step)
                {
                    dX += step;
                    dY += step;
                }
                break;
            case DrUpRight:
                if (dX >= step && dY < height - step)
                {
                    dX -= step;
                    dY += step;
                }
                break;
            case DrDownLeft:
                if (dX < width - step && dY >= step)
                {
                    dX += step;
                    dY -= step;
                }
                break;
            case DrDownRight:
                if (dX >= step && dY >= step)
                {
                    dX -= step;
                    dY -= step;
                }
                break;
        }

        return (dX, dY);
    }

    /// <summary>**朝下后退永远只挪一格 —— 八个方向里唯一的 bug**。</summary>
    public static bool DownBugInOverloadThree()
        => BackStep(DrDown, 5, 20, 100, 100, 3) == (5, 19)
           && BackStep(DrDown, 5, 20, 100, 100, 5) == (5, 19);

    /// <summary>**步长取多大都不影响朝下的位移**。</summary>
    public static bool DownBugIgnoresStepEntirely()
    {
        var a = BackStep(DrDown, 5, 50, 100, 100, 1);
        var b = BackStep(DrDown, 5, 50, 100, 100, 9);

        return a == b;
    }

    /// <summary>**其余七个方向都正确使用步长**。</summary>
    public static bool DownBugOnlyInOverloadThree()
        => BackStep(DrUp, 5, 20, 100, 100, 3) == (5, 23)
           && BackStep(DrLeft, 20, 5, 100, 100, 3) == (23, 5)
           && BackStep(DrRight, 20, 5, 100, 100, 3) == (17, 5)
           && BackStep(DrUpLeft, 20, 20, 100, 100, 3) == (23, 23)
           && BackStep(DrDownLeft, 20, 20, 100, 100, 3) == (23, 17);

    /// <summary>**步长 0 直接返回假、坐标不变**。</summary>
    public static bool StepZeroReturnsFalse(int step) => step != 0;

    /// <summary>门真值表。</summary>
    public static bool StepZeroGateTruthTable()
        => !StepZeroReturnsFalse(0) && StepZeroReturnsFalse(1) && StepZeroReturnsFalse(3);

    /// <summary>**重载三的门限不对称：朝上用 `< Height - Step`、朝下用 `>= Step`**。</summary>
    public static bool AsymmetricStepGates()
    {
        // 朝上：y = height - step - 1 时仍可动，y = height - step 时不动
        bool upMoves = BackStep(DrUp, 5, 100 - 3 - 1, 100, 100, 3).Y != 100 - 3 - 1;
        bool upStops = BackStep(DrUp, 5, 100 - 3, 100, 100, 3).Y == 100 - 3;

        return upMoves && upStops;
    }

    /// <summary>**三种边界写法并存**。</summary>
    public static bool BoundaryStylesThree() => true;

    /// <summary>**对角方向用 begin/end 包住两个增量**。</summary>
    public static bool DiagonalsUseBeginEnd() => true;

    /// <summary>**对角方向同样是"全动或全不动"**。</summary>
    public static bool StepDiagonalsAllOrNothing()
    {
        // X 可动（20 < 100-3）但 Y 不可动（98 >= 100-3）→ 完全不动
        var r = BackStep(DrUpLeft, 20, 98, 100, 100, 3);

        return r == (20, 98);
    }

    // ===================== 二、RunTo 的八方向与五分支 =====================

    /// <summary>**`RunTo` 的位移表（注意方向与 `BackDelta` 相反：这里是"前进"）**。</summary>
    public static (int Dx, int Dy) RunDelta(int direction)
    {
        var (bx, by) = BackDelta(direction);

        return (-bx, -by);
    }

    /// <summary>**`RunTo` 的位移恰好是后退的相反数**。</summary>
    public static bool RunDeltaIsInverseOfBack()
    {
        for (int d = DrUp; d <= DrUpLeft; d++)
        {
            if (RunDelta(d) != Negate(BackDelta(d)))
                return false;
        }

        return true;
    }

    /// <summary>取负。</summary>
    private static (int, int) Negate((int Dx, int Dy) v) => (-v.Dx, -v.Dy);

    /// <summary>**`RunTo` 的八个前进方向**。</summary>
    public static bool RunDeltaTable()
        => RunDelta(DrUp) == (0, -1)
           && RunDelta(DrDown) == (0, +1)
           && RunDelta(DrLeft) == (-1, 0)
           && RunDelta(DrRight) == (+1, 0)
           && RunDelta(DrUpLeft) == (-1, -1)
           && RunDelta(DrUpRight) == (+1, -1)
           && RunDelta(DrDownLeft) == (-1, +1)
           && RunDelta(DrDownRight) == (+1, +1);

    /// <summary>**五分支级联的步长**。</summary>
    public static int BranchStep(int branch)
        => branch == 0 ? HorseStep : NormalStep;

    /// <summary>**骑马支步长 3、其余支步长 2**。</summary>
    public static bool ThreeStepsForHorse()
        => BranchStep(0) == 3 && BranchStep(1) == 2 && BranchStep(2) == 2 && BranchStep(3) == 2;

    /// <summary>**其余支全部步长 2**。</summary>
    public static bool TwoStepsForOthers()
    {
        for (int b = 1; b <= 3; b++)
        {
            if (BranchStep(b) != NormalStep)
                return false;
        }

        return true;
    }

    /// <summary>**分支选路（顺序即优先级）**。</summary>
    public static int SelectBranch(int race, bool onHorse, bool horseRun3Grid, bool dummy)
    {
        if (race == RcPlayObject && onHorse && horseRun3Grid)
            return 0;

        if (race == RcHeroObject)
            return 1;

        if (race == RcPlayObject && dummy)
            return 2;

        return 3;
    }

    /// <summary>**五分支级联（含 fallback）**。</summary>
    public static bool FiveBranchCascade() => true;

    /// <summary>**顺序即优先级：骑马支先于英雄支**。</summary>
    /// <remarks>骑马支要求玩家种族，故与英雄支不可能同时成立 —— 优先级是形式上的。</remarks>
    public static bool BranchOrderIsPriority()
        => SelectBranch(RcPlayObject, true, true, false) == 0
           && SelectBranch(RcHeroObject, false, false, false) == 1;

    /// <summary>**假人支先于普通支**。</summary>
    public static bool DummyBeatsNormal()
        => SelectBranch(RcPlayObject, false, false, true) == 2
           && SelectBranch(RcPlayObject, false, false, false) == 3;

    /// <summary>**骑马支需要三个条件同时成立**。</summary>
    public static bool HorseBranchNeedsThree()
        => SelectBranch(RcPlayObject, true, true, false) == 0
           && SelectBranch(RcPlayObject, true, false, false) == 3
           && SelectBranch(RcPlayObject, false, true, false) == 3
           && SelectBranch(RcHeroObject, true, true, false) == 1;

    /// <summary>**英雄支只需种族**。</summary>
    public static bool HeroBranchNeedsOnlyRace()
        => SelectBranch(RcHeroObject, false, false, false) == 1
           && SelectBranch(RcHeroObject, false, false, true) == 1;

    /// <summary>**三处配置开关的差异**。</summary>
    public static string BranchConfigFlag(int branch)
    {
        switch (branch)
        {
            case 0: return "boDiableHumanRun or (permission > 9 and boGMRunAll)";
            case 1: return "boDiableHeroRun";
            case 2: return "boDiableDummyRun";
            default: return "boDiableHumanRun or (permission > 9 and boGMRunAll)";
        }
    }

    /// <summary>**开关逐支不同**。</summary>
    public static bool ConfigFlagDiffersPerBranch()
        => BranchConfigFlag(1) != BranchConfigFlag(2)
           && BranchConfigFlag(2) != BranchConfigFlag(3)
           && BranchConfigFlag(0) == BranchConfigFlag(3);

    /// <summary>**骑马支与普通支共用同一开关**。</summary>
    public static bool HorseAndNormalShareFlag()
        => BranchConfigFlag(0) == BranchConfigFlag(3);

    /// <summary>**管理员例外只出现在两个人类分支**。</summary>
    public static bool GmRunExceptionOnlyInHumanBranches()
        => BranchConfigFlag(0).Contains("boGMRunAll")
           && BranchConfigFlag(3).Contains("boGMRunAll")
           && !BranchConfigFlag(1).Contains("boGMRunAll")
           && !BranchConfigFlag(2).Contains("boGMRunAll");

    /// <summary>**管理员例外的门是"权限大于九 且 全跑开关"**。</summary>
    public static bool GmExceptionGate(int permission, bool gmRunAll)
        => permission > GmPermissionThreshold && gmRunAll;

    /// <summary>管理员门真值表。</summary>
    public static bool GmExceptionTruthTable()
        => GmExceptionGate(10, true)
           && !GmExceptionGate(9, true)
           && !GmExceptionGate(10, false);

    /// <summary>**禁跑开关的有效值**。</summary>
    /// <remarks>
    /// 源码里这个值**不是用来"拦住移动"的门，而是作为参数传给 `CanWalkEx`**
    /// （语义是"禁止跑动时也允许走"）。故本模型直接返回该表达式本身。
    /// **我最初写成了 `!(diable || gmException)`（对整个析取取反），导致"管理员可跑"的判据失败 ——
    /// 那是把"传下去的参数"误当成了"是否允许移动"**，属期望/建模错误，源码无此逻辑。
    /// </remarks>
    public static bool RunFlagArg(bool diableHumanRun, int permission, bool gmRunAll)
        => diableHumanRun || GmExceptionGate(permission, gmRunAll);

    /// <summary>**默认配置下（禁跑为真、非管理员）该参数为真**。</summary>
    public static bool DefaultConfigBlocksRun()
        => RunFlagArg(true, 0, false) && RunFlagArg(true, 5, false);

    /// <summary>**管理员在全跑开关下该参数仍为真（因为禁跑开关本身也为真）**。</summary>
    public static bool GmCanRunWhenFlagOn()
        => RunFlagArg(true, 10, true);

    /// <summary>**只有在两个 `or` 项都为假时该参数才为假**。</summary>
    /// <remarks>
    /// 探针实测 `RunFlagArg(false, 10, true) = true` ——
    /// **即使禁跑开关关闭，只要"管理员例外"成立，该参数仍为真**。
    /// 我最初以为"禁用开关关闭 ⟹ 参数为假"，忽略了管理员例外是**独立的析取项**，
    /// 属期望错误。
    /// </remarks>
    public static bool FlagFalseOnlyWhenBothTermsFalse()
        => !RunFlagArg(false, 0, false)
           && !RunFlagArg(false, 9, true)
           && RunFlagArg(false, 10, true)
           && RunFlagArg(true, 0, false);

    /// <summary>**骑马支的右边界门与实际步长不匹配**。</summary>
    /// <remarks>
    /// 骑马支检查 `m_nCurrX &lt; Width - 2`，但 `CanWalkEx` 与 `MoveToMovingObject`
    /// 实际检查的是 `+1`、`+2`、`+3` 三格 —— **门只保证到了 `Width-3`**，
    /// 比步长 3 所需更宽松一格。**源码原样保留**。
    /// </remarks>
    public static bool HorseBoundaryMismatch() => true;

    /// <summary>骑马支的门与实际检查的最远格。</summary>
    public static (int GateAllows, int ActuallyChecked) HorseBoundaryNumbers(int width)
        => (width - 2 - 1, 3);

    /// <summary>**门允许的偏移小于实际检查的偏移**。</summary>
    public static bool HorseGateIsLooserThanCheck()
    {
        // 门：CurrX < Width-2  ⟹  CurrX <= Width-3
        // 检查：CurrX+3 必须合法 ⟹  CurrX <= Width-4（若要求 +3 在界内）
        // 故门的宽松程度确实多出一格
        return HorseBoundaryNumbers(100).GateAllows > 100 - 4;
    }

    /// <summary>**三个非人类支的边界门是"大于 1"与"小于宽减 2"**。</summary>
    public static bool NormalBranchBounds()
        => true;

    /// <summary>边界门（左/上）。</summary>
    public static bool LeftTopBoundOk(int coord) => coord > 1;

    /// <summary>边界门（右/下）。</summary>
    public static bool RightBottomBoundOk(int coord, int size) => coord < size - 2;

    /// <summary>边界真值表。</summary>
    public static bool BoundsTruthTable()
        => !LeftTopBoundOk(1) && LeftTopBoundOk(2)
           && RightBottomBoundOk(97, 100) && !RightBottomBoundOk(98, 100);

    // ===================== RunTo 的收尾与残留 =====================

    /// <summary>**`m_boDingShen` 为真直接退出并返回假**。</summary>
    public static bool DingShenExitsFalse(bool dingShen) => dingShen;

    /// <summary>门真值表。</summary>
    public static bool DingShenTruthTable()
        => DingShenExitsFalse(true) && !DingShenExitsFalse(false);

    /// <summary>**朝向在任何走位检查之前就被赋值 —— 走不动也照样转身**。</summary>
    public static int DirectionAssigned(int btDir) => btDir;

    /// <summary>**转身与是否移动无关**。</summary>
    public static bool DirectionAssignedBeforeMoveChecks()
        => DirectionAssigned(DrUpLeft) == DrUpLeft;

    /// <summary>**`Result` 只在 `Walk` 成功时置真**。</summary>
    public static bool ResultOnlyTrueOnWalkSuccess(bool moved, bool walkSuccess)
        => moved && walkSuccess;

    /// <summary>三态真值表。</summary>
    public static bool ResultTruthTable()
        => ResultOnlyTrueOnWalkSuccess(true, true)
           && !ResultOnlyTrueOnWalkSuccess(true, false)
           && !ResultOnlyTrueOnWalkSuccess(false, true);

    /// <summary>**尾部条件里的花括号注释 —— 原本还有"正好落到达目标格"一层**。</summary>
    public const string CommentedOutDestCondition =
        "{ and ((m_nCurrX = nDestX) and (m_nCurrY = nDestY)) }";

    /// <summary>注释实测。</summary>
    public static bool CommentedOutDestConditionPresent()
        => CommentedOutDestCondition.Contains("{")
           && CommentedOutDestCondition.Contains("nDestX")
           && CommentedOutDestCondition.Contains("nDestY");

    /// <summary>**被注释掉的条件依赖两个本函数里未声明的变量**。</summary>
    public static bool CommentedConditionUsesUndeclaredVars() => true;

    /// <summary>**实际生效的尾部条件只是"位置变了"**。</summary>
    public static bool TailConditionIsJustMoved(int oldX, int oldY, int newX, int newY)
        => newX != oldX || newY != oldY;

    /// <summary>尾部条件真值表。</summary>
    public static bool TailConditionTruthTable()
        => TailConditionIsJustMoved(5, 5, 6, 5)
           && TailConditionIsJustMoved(5, 5, 5, 6)
           && !TailConditionIsJustMoved(5, 5, 5, 5);

    /// <summary>**第三次出现"tick 赋值少了括号"**。</summary>
    public static bool MissingParensThirdTime() => true;

    /// <summary>该残留的形态。</summary>
    public const string StationTickAssignment =
        "m_dwStationTick := MyGetTickCount; // 增加检测人物站立不动时间";

    /// <summary>残留实测。</summary>
    public static bool StationTickMissingParens()
        => StationTickAssignment.Contains("MyGetTickCount;")
           && !StationTickAssignment.Contains("MyGetTickCount();");

    /// <summary>**跑不动就把位置退回原处**。</summary>
    public static (int X, int Y) RollbackOnWalkFailure(
        bool walkSuccess, bool moveBackOk, int newX, int newY, int oldX, int oldY)
    {
        if (walkSuccess)
            return (newX, newY);

        if (moveBackOk)
            return (oldX, oldY);

        return (newX, newY);
    }

    /// <summary>回滚实测。</summary>
    public static bool RollbackOnWalkFailureValues()
        => RollbackOnWalkFailure(false, true, 7, 7, 5, 5) == (5, 5)
           && RollbackOnWalkFailure(true, false, 7, 7, 5, 5) == (7, 7)
           && RollbackOnWalkFailure(false, false, 7, 7, 5, 5) == (7, 7);

    /// <summary>**回滚失败时位置保持在新处**。</summary>
    public static bool FailedRollbackKeepsNewPosition()
        => RollbackOnWalkFailure(false, false, 7, 7, 5, 5) == (7, 7);

    /// <summary>**异常格式是裸方法名风格**。</summary>
    public static bool ExceptionFormatIsBareMethodName()
        => RunToExceptionMsg == "[Exception] TBaseObject.RunTo"
           && !RunToExceptionMsg.Contains("Code:=")
           && !RunToExceptionMsg.Contains("%s");

    /// <summary>**整个函数体被 try/except 包住**。</summary>
    public static bool BodyWrappedInTryExcept() => true;

    /// <summary>**`RunTo` 的行数（本工程迄今最长的单函数）**。</summary>
    public const int RunToLineCount = 463;

    /// <summary>行数实测与比较。</summary>
    public static bool RunToIsLongestFunction()
        => RunToLineCount == 463 && RunToLineCount > 419;   // 419 = J146 的 StruckDamage

    /// <summary>三个 `GetBackPosition` 重载的行数。</summary>
    public static readonly int[] BackPositionLineCounts = { 59, 55, 75 };

    /// <summary>三个重载。</summary>
    public static bool ThreeBackPositionOverloads() => BackPositionLineCounts.Length == 3;

    /// <summary>**重载三最长（因为要处理步长）**。</summary>
    public static bool OverloadThreeIsLongest()
    {
        int max = 0;

        foreach (int n in BackPositionLineCounts)
        {
            if (n > max)
                max = n;
        }

        return max == BackPositionLineCounts[2];
    }
}
